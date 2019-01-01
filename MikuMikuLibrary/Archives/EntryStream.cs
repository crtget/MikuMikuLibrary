﻿using System.IO;

namespace MikuMikuLibrary.Archives
{
    public sealed class EntryStream<THandle> : Stream
    {
        private Stream source;

        public THandle Handle { get; }

        public override bool CanRead => source.CanRead;
        public override bool CanWrite => source.CanWrite;
        public override bool CanSeek => source.CanSeek;

        public override long Position
        {
            get => source.Position;
            set => source.Position = value;
        }

        public override long Length => source.Length;

        public override void Flush()
        {
            source.Flush();
        }

        public override int Read( byte[] buffer, int offset, int count )
        {
            return source.Read( buffer, 0, count );
        }

        public override long Seek( long offset, SeekOrigin origin )
        {
            return source.Seek( offset, origin );
        }

        public override void SetLength( long value )
        {
            source.SetLength( value );
        }

        public override void Write( byte[] buffer, int offset, int count )
        {
            source.Write( buffer, 0, count );
        }

        public EntryStream( THandle entry, Stream source )
        {
            Handle = entry;
            this.source = source;
        }
    }

    public enum EntryStreamMode
    {
        MemoryStream,
        OriginalStream,
    };
}
