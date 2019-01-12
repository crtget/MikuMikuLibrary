using MikuMikuLibrary.IO.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace MikuMikuLibrary.IO.Sections
{
    public interface ISection
    {
        AddressSpace AddressSpace { get; }
        EndianBinaryReader BaseReader { get; }
        Stream BaseStream { get; }
        EndianBinaryWriter BaseWriter { get; }
        object DataObject { get; }
        long DataOffset { get; }
        long DataSize { get; }
        Type DataType { get; }
        Endianness Endianness { get; }
        SectionFlags Flags { get; }
        BinaryFormat Format { get; }
        SectionMode Mode { get; }
        SectionInfo SectionInfo { get; }
        IEnumerable<ISection> Sections { get; }
        string Signature { get; }

        void Read( Stream source );
        void Write( Stream destination );
    }

    public enum SectionMode { Read, Write };
    public enum SectionFlags
    {
        None = 0,
        HasRelocationTable = 1 << 0,
        HasEndianReverserTable = 1 << 0,
    };
}