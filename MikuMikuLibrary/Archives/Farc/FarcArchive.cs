﻿using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MikuMikuLibrary.Archives.Farc
{
    public class FarcArchive : BinaryFile, IArchive<string>
    {
        private readonly Dictionary<string, InternalEntry> mEntries;
        private int mAlignment;

        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.UsesSourceStream;

        public override Endianness Endianness => Endianness.BigEndian;

        public bool CanAdd => true;
        public bool CanRemove => true;

        public IEnumerable<string> Entries => mEntries.Keys;

        public int Alignment
        {
            get => mAlignment;
            set
            {
                if ( ( value & ( value - 1 ) ) != 0 )
                    mAlignment = AlignmentUtilities.AlignToNextPowerOfTwo( value );
                else
                    mAlignment = value;
            }
        }

        public void Add( string handle, Stream source, bool leaveOpen, ConflictPolicy conflictPolicy = ConflictPolicy.RaiseError )
        {
            if ( mEntries.TryGetValue( handle, out var entry ) )
            {
                switch ( conflictPolicy )
                {
                    case ConflictPolicy.RaiseError:
                        throw new InvalidOperationException( $"Entry already exists ({handle})" );

                    case ConflictPolicy.Replace:
                        entry.Dispose();
                        entry.Stream = source;
                        entry.OwnsStream = !leaveOpen;
                        break;

                    case ConflictPolicy.Ignore:
                        break;
                }
            }

            else
            {
                mEntries.Add( handle, new InternalEntry
                {
                    Handle = handle,
                    Stream = source,
                    OwnsStream = !leaveOpen,
                } );
            }
        }

        public void Add( string handle, string fileName, ConflictPolicy conflictPolicy = ConflictPolicy.RaiseError )
        {
            Add( handle, File.OpenRead( fileName ), false, conflictPolicy );
        }

        public void Remove( string handle )
        {
            if ( mEntries.TryGetValue( handle, out var entry ) )
            {
                entry.Dispose();
                mEntries.Remove( handle );
            }
        }

        public EntryStream<string> Open( string handle, EntryStreamMode mode )
        {
            var entry = mEntries[ handle ];
            var entryStream = entry.Open( mStream );

            if ( mode == EntryStreamMode.MemoryStream )
            {
                var temp = entryStream;
                entryStream = new MemoryStream();
                temp.CopyTo( entryStream );
                entryStream.Position = 0;
            }

            return new EntryStream<string>( entry.Handle, entryStream );
        }

        public void Clear()
        {
            foreach ( var entry in mEntries.Values )
                entry.Dispose();

            mEntries.Clear();
        }

        public bool Contains( string handle )
        {
            return mEntries.ContainsKey( handle );
        }

        public IEnumerator<string> GetEnumerator()
        {
            return mEntries.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mEntries.Keys.GetEnumerator();
        }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            string signature = reader.ReadString( StringBinaryFormat.FixedLength, 4 );
            if ( signature != "FARC" && signature != "FArC" && signature != "FArc" )
                throw new InvalidDataException( "Invalid signature, excepted FARC/FArC/FArc" );

            uint headerSize = reader.ReadUInt32() + 0x08;
            Stream originalStream = reader.BaseStream;

            if ( signature == "FARC" )
            {
                int flags = reader.ReadInt32();
                bool isCompressed = ( flags & ( 1 << 1 ) ) != 0;
                bool isEncrypted = ( flags & ( 1 << 2 ) ) != 0;
                int padding = reader.ReadInt32();
                mAlignment = reader.ReadInt32();

                // Hacky way of checking Future Tone.
                // There's a very low chance this isn't going
                // to work, though.
                Format = isEncrypted && ( mAlignment & ( mAlignment - 1 ) ) != 0 ? BinaryFormat.FT : BinaryFormat.DT;

                if ( Format == BinaryFormat.FT )
                {
                    reader.SeekBegin( 0x10 );
                    var iv = reader.ReadBytes( 0x10 );
                    var aesManaged = CreateAesManagedForFT( iv );
                    var decryptor = aesManaged.CreateDecryptor();
                    var cryptoStream = new CryptoStream( reader.BaseStream, decryptor, CryptoStreamMode.Read );
                    reader = new EndianBinaryReader( cryptoStream, Encoding.UTF8, Endianness.BigEndian );
                    mAlignment = reader.ReadInt32();
                }

                // Since the first check worked only if the file was encrypted,
                // we are going to one extra check here, since not all FARC files
                // are necessarily encrypted. (see Len's lenitm FARCs from FT)
                Format = reader.ReadInt32() == 1 ? BinaryFormat.FT : BinaryFormat.DT;

                int entryCount = reader.ReadInt32();
                if ( Format == BinaryFormat.FT )
                    padding = reader.ReadInt32(); // No SeekCurrent!! CryptoStream does not support it.

                while ( originalStream.Position < headerSize )
                {
                    string name = reader.ReadString( StringBinaryFormat.NullTerminated );
                    uint offset = reader.ReadUInt32();
                    uint compressedSize = reader.ReadUInt32();
                    uint uncompressedSize = reader.ReadUInt32();

                    if ( Format == BinaryFormat.FT )
                    {
                        flags = reader.ReadInt32();
                        isCompressed = ( flags & ( 1 << 1 ) ) != 0;
                        isEncrypted = ( flags & ( 1 << 2 ) ) != 0;
                    }

                    long fixedSize = 0;
                    if ( isEncrypted )
                    {
                        if ( isCompressed )
                            fixedSize = AlignmentUtilities.Align( compressedSize, 16 );
                        else
                            fixedSize = AlignmentUtilities.Align( uncompressedSize, 16 );
                    }

                    else if ( isCompressed )
                        fixedSize = compressedSize;

                    else
                        fixedSize = uncompressedSize;

                    fixedSize = Math.Min( fixedSize, originalStream.Length - offset );

                    mEntries.Add( name, new InternalEntry
                    {
                        Handle = name,
                        Position = offset,
                        Length = fixedSize,
                        IsCompressed = isCompressed && ( compressedSize != uncompressedSize ),
                        IsEncrypted = isEncrypted,
                        IsFutureTone = Format == BinaryFormat.FT,
                    } );

                    // There's sometimes extra padding on some FARC files which
                    // causes this loop to throw an exception. This check fixes it.
                    if ( Format == BinaryFormat.FT && ( --entryCount ) == 0 )
                        break;
                }
            }

            else if ( signature == "FArC" )
            {
                mAlignment = reader.ReadInt32();

                while ( reader.Position < headerSize )
                {
                    string name = reader.ReadString( StringBinaryFormat.NullTerminated );
                    uint offset = reader.ReadUInt32();
                    uint compressedSize = reader.ReadUInt32();
                    uint uncompressedSize = reader.ReadUInt32();

                    long fixedSize = Math.Min( compressedSize, reader.Length - offset );

                    mEntries.Add( name, new InternalEntry
                    {
                        Handle = name,
                        Position = offset,
                        Length = fixedSize,
                        IsCompressed = compressedSize != uncompressedSize,
                    } );
                }
            }

            else if ( signature == "FArc" )
            {
                mAlignment = reader.ReadInt32();

                while ( reader.Position < headerSize )
                {
                    string name = reader.ReadString( StringBinaryFormat.NullTerminated );
                    uint offset = reader.ReadUInt32();
                    uint size = reader.ReadUInt32();

                    long fixedSize = Math.Min( size, reader.Length - offset );

                    mEntries.Add( name, new InternalEntry
                    {
                        Handle = name,
                        Position = offset,
                        Length = fixedSize,
                    } );
                }
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( "FArc", StringBinaryFormat.FixedLength, 4 );
            writer.ScheduleWriteOffset( OffsetMode.Size, () =>
            {
                writer.Write( mAlignment );

                foreach ( var entry in mEntries.Values )
                {
                    writer.Write( entry.Handle, StringBinaryFormat.NullTerminated );
                    writer.ScheduleWriteOffset( mAlignment, 0x78, AlignmentMode.Center, OffsetMode.OffsetAndSize, () =>
                    {
                        long position = writer.Position;

                        var entryStream = entry.Open( mStream );
                        if ( entryStream.CanSeek )
                            entryStream.Position = 0;

                        entryStream.CopyTo( writer.BaseStream );

                        entry.Position = position;
                        entry.Length = writer.Position - position;
                        entry.IsCompressed = entry.IsEncrypted = entry.IsFutureTone = false;

                        if ( entry.Stream != null )
                        {
                            if ( entry.OwnsStream )
                                entry.Stream.Dispose();

                            entry.Stream = null;
                            entry.OwnsStream = false;
                        }

                        entryStream.Close();
                    } );
                }
            } );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                foreach ( var entry in mEntries.Values )
                    entry.Dispose();
            }

            base.Dispose( disposing );
        }

        public static AesManaged CreateAesManaged()
        {
            return new AesManaged
            {
                KeySize = 128,
                Key = new byte[]
                {
                    // project_diva.bin
                    0x70, 0x72, 0x6F, 0x6A, 0x65, 0x63, 0x74, 0x5F, 0x64, 0x69, 0x76, 0x61, 0x2E, 0x62, 0x69, 0x6E
                },
                BlockSize = 128,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.Zeros,
                IV = new byte[ 16 ],
            };
        }

        public static AesManaged CreateAesManagedForFT( byte[] iv = null )
        {
            return new AesManaged
            {
                KeySize = 128,
                Key = new byte[]
                {
                    0x13, 0x72, 0xD5, 0x7B, 0x6E, 0x9E, 0x31, 0xEB, 0xA2, 0x39, 0xB8, 0x3C, 0x15, 0x57, 0xC6, 0xBB,
                },
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.Zeros,
                IV = iv ?? new byte[ 16 ],
            };
        }

        public FarcArchive()
        {
            mEntries = new Dictionary<string, InternalEntry>( StringComparer.OrdinalIgnoreCase );
            mAlignment = 0x10;
        }
    }
}
