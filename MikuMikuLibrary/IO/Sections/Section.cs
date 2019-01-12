using MikuMikuLibrary.IO.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MikuMikuLibrary.IO.Sections
{
    public abstract class Section<T> : ISection, IDisposable where T : new()
    {
        private readonly List<ISection> mSections = new List<ISection>();
        private SectionInfo mSectionInfo;
        private T mDataObject;
        private long mDataOffset;
        private long mDataSize;
        private bool mObjectProcessed;
        private SectionMode mMode;
        private Stream mStream;

        private EndianBinaryReader mReader;
        private EndianBinaryWriter mWriter;

        public string Signature => SectionInfo.Signature;

        public T DataObject
        {
            get
            {
                if ( !mObjectProcessed )
                    ProcessDataObject();

                return mDataObject;
            }
        }

        object ISection.DataObject => DataObject;

        public Type DataType => typeof( T );

        public SectionInfo SectionInfo =>
            mSectionInfo ?? ( mSectionInfo = SectionRegistry.GetOrRegisterSectionInfo( GetType() ) );

        public IEnumerable<ISection> Sections => mSections;

        public BinaryFormat Format =>
            ( AddressSpace == AddressSpace.Int64 ) ? BinaryFormat.X : BinaryFormat.F2nd;

        public abstract SectionFlags Flags { get; }
        public virtual Endianness Endianness { get; protected set; }
        public virtual AddressSpace AddressSpace { get; protected set; }
        public SectionMode Mode => mMode;

        public long DataOffset => mDataOffset;
        public long DataSize => mDataSize;
        public Stream BaseStream => mStream;
        public EndianBinaryReader BaseReader => mReader;
        public EndianBinaryWriter BaseWriter => mWriter;

        public void Read( Stream source )
        {
            if ( mMode == SectionMode.Write )
                throw new InvalidOperationException( "Section is in write mode, cannot read" );

            if ( mStream != null )
                throw new InvalidOperationException( "Section has already been read before" );

            mStream = source;
            mReader = new EndianBinaryReader( mStream, Encoding.UTF8, true, Endianness.LittleEndian );
            mReader.PushBaseOffset();

            string signature = mReader.ReadString( StringBinaryFormat.FixedLength, 4 );
            if ( signature != Signature )
                throw new InvalidDataException( $"Invalid signature (expected {Signature}, got {signature})" );

            uint sectionSize = mReader.ReadUInt32();
            uint dataOffset = mReader.ReadUInt32();
            int endiannessFlag = mReader.ReadInt32();
            Endianness = ( endiannessFlag == 0x18000000 ) ? Endianness.BigEndian : Endianness.LittleEndian;
            int depth = mReader.ReadInt32();
            mDataSize = mReader.ReadUInt32();

            mDataOffset = mReader.BaseOffset + dataOffset;

            if ( ( sectionSize - mDataSize ) != 0 )
            {
                mReader.SeekBegin( mDataOffset + mDataSize );
                {
                    while ( mReader.Position < ( mDataOffset + sectionSize ) )
                    {
                        var subSectionSignature = mReader.PeekString( StringBinaryFormat.FixedLength, 4 );
                        var sectionInfo = SectionRegistry.SectionInfosBySignature[ subSectionSignature ];

                        var section = sectionInfo.Create( SectionMode.Read );
                        {
                            section.Read( source );
                            mSections.Add( section );
                        }

                        if ( section is RelocationTableSectionInt64 )
                            AddressSpace = AddressSpace.Int64;

                        else if ( section is EndOfFileSection )
                            break;
                    }
                }
            }
            else
                mReader.SeekBegin( mDataOffset + sectionSize );

            if ( AddressSpace == AddressSpace.Int64 )
                mReader.BaseOffset = mDataOffset;

            // If an object was provided for reading, process it regardless
            if ( !mObjectProcessed && mDataObject != null )
                ProcessDataObject();
        }

        private void Write( Stream destination, int depth )
        {
            if ( mMode == SectionMode.Read )
                throw new InvalidOperationException( "Section is in read mode, cannot write" );

            if ( mStream != null )
                throw new InvalidOperationException( "Section has already been written before" );

            mStream = destination;
        }

        public void Write( Stream destination ) => Write( destination, 0 );

        public void Write( Stream destination, Endianness endianness, AddressSpace addressSpace )
        {
            Endianness = endianness;
            AddressSpace = addressSpace;
            Write( destination );
        }

        public virtual void Dispose()
        {
            mReader?.Dispose();
            mWriter?.Dispose();
        }

        private void ProcessDataObject()
        {
            if ( mMode == SectionMode.Read )
            {
                if ( mStream == null || mReader == null )
                    throw new InvalidOperationException( "Section has not been read yet, cannot process data object" );

                if ( mDataObject == null )
                    mDataObject = new T();

                mReader.SeekBegin( mDataOffset );
                {
                    mReader.Endianness = Endianness;
                    mReader.AddressSpace = AddressSpace;
                    {
                        Read( mDataObject, mReader, mDataSize );
                    }
                    mReader.Endianness = Endianness.LittleEndian;
                    mReader.AddressSpace = AddressSpace.Int32;
                }

                foreach ( var section in mSections )
                {
                    if ( SectionInfo.SubSectionInfos.TryGetValue( section.SectionInfo, out var subSectionInfo ) )
                        subSectionInfo.ProcessPropertyForReading( section, mDataObject );
                }
            }
            else if ( mMode == SectionMode.Write )
            {
                if ( mStream == null || mWriter == null )
                    throw new InvalidOperationException( "Data object has not been written yet, cannot process data object" );

                mSections.Clear();
                if ( mWriter.OffsetPositions.Count > 0 && Flags.HasFlag( SectionFlags.HasRelocationTable ) )
                {
                    ISection relocationTableSection;

                    var offsets = mWriter.OffsetPositions.Select( x => x - mWriter.BaseOffset ).ToList();
                    if ( AddressSpace == AddressSpace.Int64 )
                        relocationTableSection = new RelocationTableSectionInt64( SectionMode.Write, offsets );
                    else
                        relocationTableSection = new RelocationTableSectionInt32( SectionMode.Write, offsets );

                    mSections.Add( relocationTableSection );
                }

                foreach ( var subSectionInfo in SectionInfo.SubSectionInfos.Values.OrderBy( x => x.Priority ) )
                    mSections.AddRange( subSectionInfo.ProcessPropertyForWriting( mDataObject ) );

                if ( mSections.Count > 0 )
                    mSections.Add( new EndOfFileSection( SectionMode.Write, this ) );
            }

            mObjectProcessed = true;
        }

        protected abstract void Read( T dataObject, EndianBinaryReader reader, long length );
        protected abstract void Write( T dataObject, EndianBinaryWriter writer );

        public Section( SectionMode mode, T dataObject = default( T ) )
        {
            mMode = mode;
            mDataObject = dataObject;

            if ( mMode == SectionMode.Write && dataObject == null )
                throw new ArgumentNullException( "Data object must be provided in write mode", nameof( dataObject ) );
        }
    }
}
