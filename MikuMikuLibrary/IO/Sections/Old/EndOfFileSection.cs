﻿using MikuMikuLibrary.IO.Common;
using System.IO;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "EOFC", typeof( object ) )]
    public class EndOfFileSection : Section
    {
        public override Endianness Endianness => Endianness.LittleEndian;
        public override AddressSpace AddressSpace => AddressSpace.Int32;

        public override SectionFlags Flags => SectionFlags.None;

        protected override void Read( EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( EndianBinaryWriter writer )
        {
        }

        public EndOfFileSection( Stream source, object dataToRead = null ) : base( source, dataToRead )
        {
        }

        public EndOfFileSection( object dataToWrite ) : base( dataToWrite, Endianness.LittleEndian, AddressSpace.Int32 )
        {
        }

    }
}
