using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuLibrary.Scripts
{
    /// <summary>
    /// A collecton of <see cref="PvCommand"/>s.
    /// </summary>
    public sealed class PvScript : BinaryFile
    {
        public override BinaryFileFlags Flags => BinaryFileFlags.Load | BinaryFileFlags.Save;

        public uint ScriptFormat { get; private set; }

        public List<PvCommand> Commands { get; set; }

        private void ReadFormat(EndianBinaryReader reader)
        {
            ScriptFormat = reader.ReadUInt32();
        }

        private void ParseModernHeader(EndianBinaryReader reader)
        {
            reader.ReadAtOffset(0x40, () => ReadFormat(reader));
            reader.SeekBegin(0x48);
        }

        private void ReadCommands(EndianBinaryReader reader)
        {
            reader.Endianness = Endianness;

            var descriptor = GetFormatDescriptor();

            int predictedCapacity = (int)reader.Length / 2;
            Commands = new List<PvCommand>(predictedCapacity);

            int currentTime = default(int);

            for (int i = 0; i < reader.Length / sizeof(int); i++)
            {
                int opcode = reader.ReadInt32();
                bool valid = descriptor.TryGetCommandInfo(opcode, out CommandInfo commandInfo);

                int[] arguments = reader.ReadInt32s(Math.Max(commandInfo.ArgumentCount, 0));

                if (opcode == 1) // TIME
                    currentTime = arguments[0];

                Commands.Add(new PvCommand(currentTime, opcode, arguments));

                if (!valid)
                    throw new Exception($"Invalid opcode 0x{opcode:X8}");

                if (!valid || opcode == 0) // END
                    break;
            }
        }

        public FormatDescriptor GetFormatDescriptor()
        {
            return ScriptUtilities.GetFormatDescriptor(Format);
        }

        public StringBuilder FormatText()
        {
            if (Commands == null)
                return null;

            var descriptor = GetFormatDescriptor();

            var builder = new StringBuilder(Commands.Count * 32);
            {
                for (int i = 0; i < Commands.Count; i++)
                    builder.AppendLine(Commands[i].FormatString(descriptor));
            }
            return builder;
        }

        public override void Read(EndianBinaryReader reader, ISection section = null)
        {
            if (reader.Length <= sizeof(int))
                return;

            Endianness = Endianness.LittleEndian;

            byte[] magic = reader.ReadBytes(sizeof(uint));

            if (DetermineIsModern(magic))
            {
                ParseModernHeader(reader);

                Format = BinaryFormat.X;
            }
            else
            {
                reader.SeekBegin(0);
                ReadFormat(reader);

                Format = DetermineFormat(ScriptFormat);
            }

            int firstOpcode = reader.ReadInt32();
            if (firstOpcode != 0 && firstOpcode > ushort.MaxValue)
                Endianness = Endianness.BigEndian;

            reader.SeekCurrent(-sizeof(int));

            ReadCommands(reader);
        }

        private void WriteCommands(EndianBinaryWriter writer)
        {
            writer.Endianness = Endianness;

            if (Commands == null)
                return;

            var descriptor = GetFormatDescriptor();

            foreach (var command in Commands)
            {
                if (!descriptor.TryGetCommandInfo(command.Opcode, out CommandInfo commandInfo))
                    continue;

                writer.Write(command.Opcode);

                for (int i = 0; i < commandInfo.ArgumentCount; i++)
                {
                    int value = i >= 0 && i < command.Arguments.Length ? command.Arguments[i] : -1;
                    writer.Write(value);
                }
            }

            // Add END if necessary
            if (Commands.Last().Opcode != 0)
            {
                writer.Write(0x00000000);
            }
        }

        public override void Write(EndianBinaryWriter writer, ISection section = null)
        {
            // Update the format if needed for future parsing
            if (!ScriptUtilities.GetScriptFormats(Format).Contains(ScriptFormat))
            {
                ScriptFormat = ScriptUtilities.GetDefaultScriptFormat(Format);
            }

            bool modern = BinaryFormatUtilities.IsModern(Format);
            if (modern)
            {
                // TODO: write header
                throw new NotImplementedException();
            }
            else
            {
                writer.Write(ScriptFormat);
            }

            WriteCommands(writer);

            if (modern)
            {
                // TODO: write EOFC
                throw new NotImplementedException();
            }
        }

        private static bool DetermineIsModern(byte[] magic)
        {
            for (int i = 0; i < magic.Length; i++)
            {
                if (magic[i] != ScriptUtilities.PVSC_MAGIC[i])
                    return false;
            }

            return true;
        }

        private static BinaryFormat DetermineFormat(uint scriptFormat)
        {
            for (BinaryFormat i = BinaryFormat.DT; i < BinaryFormat.X; i++)
            {
                foreach (var format in ScriptUtilities.GetScriptFormats(i))
                {
                    if (format == scriptFormat)
                        return i;
                }
            }

            throw new Exception($"Unknown script format: 0x{scriptFormat:X8}");
        }
    }
}
