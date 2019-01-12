using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System;
using System.IO;
using System.Text;
using System.Linq;

namespace MikuMikuLibrary.IO
{
    public abstract class BinaryFile : IBinaryFile
    {
        protected Stream mStream;
        protected bool mOwnsStream;

        public abstract BinaryFileFlags Flags { get; }
        public virtual BinaryFormat Format { get; set; }
        public virtual Endianness Endianness { get; set; }

        public static T Load<T>( Stream source, bool leaveOpen = false ) where T : IBinaryFile, new()
        {
            var instance = new T();
            instance.Load( source, leaveOpen );
            return instance;
        }

        public static T Load<T>( string filePath ) where T : IBinaryFile, new()
        {
            var instance = new T();
            instance.Load( filePath );
            return instance;
        }

        public static T LoadIfExist<T>( string filePath ) where T : IBinaryFile, new()
        {
            var instance = new T();

            if ( string.IsNullOrEmpty( filePath ) || !File.Exists( filePath ) )
                return instance;

            instance.Load( filePath );
            return instance;
        }

        public void Load( Stream source, bool leaveOpen = false )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Load ) )
                throw new NotSupportedException( "Binary file is not able to load" );

            if ( Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) )
            {
                mStream = source;
                mOwnsStream = !leaveOpen;
            }

            bool readAsSection = false;

            // Attempt to detect the section format and read with that
            if ( Flags.HasFlag( BinaryFileFlags.HasSectionedVersion ) )
            {
                long position = source.Position;
                var signatureBytes = new byte[ 4 ];
                source.Read( signatureBytes, 0, signatureBytes.Length );
                source.Seek( position, SeekOrigin.Begin );

                var signature = Encoding.ASCII.GetString( signatureBytes );
                if ( SectionRegistry.SectionInfosBySignature.TryGetValue( signature, out SectionInfo sectionInfo ) )
                {
                    var section = sectionInfo.Create( SectionMode.Read, this );
                    section.Read( source );

                    readAsSection = true;
                }
            }

            if ( !readAsSection )
            {
                // Or try to read in the old fashioned way
                using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness ) )
                {
                    reader.PushBaseOffset();
                    {
                        Read( reader );
                    }
                    reader.PopBaseOffset();
                }
            }

            if ( !leaveOpen && !Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) )
                source.Dispose();
        }

        public virtual void Load( string filePath )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Load ) )
                throw new NotSupportedException( "Binary file is not able to load" );

            Load( File.OpenRead( filePath ), false );
        }

        public void LoadIfExist( string filePath )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Load ) )
                throw new NotSupportedException( "Binary file is not able to load" );

            if ( string.IsNullOrEmpty( filePath ) || !File.Exists( filePath ) )
                return;

            Load( filePath );
        }

        public void Save( Stream destination, bool leaveOpen = false )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Save ) )
                throw new NotSupportedException( "Binary file is not able to save" );

            // See if we are supposed to write in sectioned format
            if ( Flags.HasFlag( BinaryFileFlags.HasSectionedVersion ) && BinaryFormatUtilities.IsModern( Format ) )
                GetSectionInstanceForWriting().Write( destination );

            else
            {
                // Or try to write in the old fashioned way
                using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness ) )
                {
                    writer.PushBaseOffset();
                    {
                        // Push a string table
                        writer.PushStringTable( 16, AlignmentMode.Center, StringBinaryFormat.NullTerminated );
                        {
                            Write( writer );
                        }
                        // Do the enqueued offset writes & string tables
                        writer.DoScheduledWriteOffsets();
                        writer.PopStringTablesReversed();
                    }
                    writer.PopBaseOffset();
                }
            }

            // Adopt this stream
            if ( Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) )
            {
                if ( mOwnsStream )
                    mStream.Dispose();

                mStream = destination;
                mOwnsStream = !leaveOpen;

                mStream.Flush();
            }
            else if ( !leaveOpen )
            {
                destination.Close();
            }
        }

        public virtual void Save( string filePath )
        {
            if ( !Flags.HasFlag( BinaryFileFlags.Save ) )
                throw new NotSupportedException( "Binary file is not able to save" );

            // Prevent any kind of conflict.
            if ( Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) && mStream is FileStream fileStream )
            {
                filePath = Path.GetFullPath( filePath );
                string thisFilePath = Path.GetFullPath( fileStream.Name );

                if ( filePath.Equals( thisFilePath, StringComparison.OrdinalIgnoreCase ) )
                {
                    do
                    {
                        thisFilePath += "_";
                    } while ( File.Exists( thisFilePath ) );

                    using ( var destination = File.Create( thisFilePath ) )
                        Save( destination, false );

                    fileStream.Close();

                    File.Delete( filePath );
                    File.Move( thisFilePath, filePath );

                    mStream = new FileStream( filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite );
                    mOwnsStream = true;

                    return;
                }
            }

            Save( new FileStream( filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite ), false );
        }

        protected virtual ISection GetSectionInstanceForWriting()
        {
            var type = GetType();
            var sectionInfo = SectionRegistry.SectionInfos.FirstOrDefault( x => x.DataType == type );

            if ( sectionInfo == null )
                throw new NotImplementedException();

            return sectionInfo.Create( SectionMode.Write, this );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Cleans up resources used by the object.
        /// </summary>
        /// <param name="disposing">Whether or not the managed objects are going to be disposed.</param>
        protected virtual void Dispose( bool disposing )
        {
            if ( disposing && Flags.HasFlag( BinaryFileFlags.UsesSourceStream ) && mOwnsStream )
                mStream?.Dispose();
        }

        ~BinaryFile()
        {
            Dispose( false );
        }

        public abstract void Read( EndianBinaryReader reader, ISection section = null );
        public abstract void Write( EndianBinaryWriter writer, ISection section = null );
    }
}
