using MikuMikuLibrary.IO.Common;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuLibrary.Aet
{
    public class SpriteMetadataEntry
    {
        internal int ThisOffset { get; set; }

        public int UnkColor { get; set; }

        public short Width { get; set; }
        public short Height { get; set; }

        public float UnkownFloat { get; set; }

        public int SpritesOffset { get; internal set; }

        public List<SpriteEntry> ReferencedSprites { get; set; }

        public SpriteMetadataEntry()
        {
            return;
        }

        public SpriteMetadataEntry(int unkColor, short width, short height, float unkFloat, int offset)
        {
            UnkColor = unkColor;
            Width = width;
            Height = height;
            UnkownFloat = unkFloat;
            SpritesOffset = offset;
        }

        public static SpriteMetadataEntry Read(EndianBinaryReader reader, List<SpriteEntry> parentEntries)
        {
            int thisOffset = (int)reader.Position;

            int unkColor = reader.ReadInt32();

            short width = reader.ReadInt16();
            short height = reader.ReadInt16();

            float unkFloat = reader.ReadSingle();

            int spriteCount = reader.ReadInt32();
            int spritesOffset = reader.ReadInt32();

            var referencedSprites = new List<SpriteEntry>(spriteCount);
            for (int i = 0; i < spriteCount; i++)
            {
                SpriteEntry entry = parentEntries.FirstOrDefault(e => e.ThisOffset == spritesOffset);
                referencedSprites.Add(entry);
            }

            return new SpriteMetadataEntry(unkColor, width, height, unkFloat, spritesOffset)
            {
                ThisOffset = thisOffset,
                ReferencedSprites = referencedSprites,
            };
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write(UnkColor);

            writer.Write(Width);
            writer.Write(Height);

            writer.Write(UnkownFloat);

            SpritesOffset = (int)ReferencedSprites?.FirstOrDefault()?.ThisOffset;

            writer.Write((int)ReferencedSprites?.Count);
            writer.Write(SpritesOffset);
        }

        public override string ToString()
        {
            return $"{Width}x{Height} - {ReferencedSprites.FirstOrDefault()?.Name ?? "NULL"}";
        }
    }
}
