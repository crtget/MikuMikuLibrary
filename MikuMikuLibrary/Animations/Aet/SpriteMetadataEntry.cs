using MikuMikuLibrary.IO.Common;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuLibrary.Animations.Aet
{
    public class SpriteMetadataEntry
    {
        public int SpriteMetadataEntryOffset { get; set; }

        public int Unkcolor { get; set; }

        public short Width { get; set; }
        public short Height { get; set; }

        public float UnkownFloat { get; set; }

        public int SpriteCount { get; set; }
        public int SpriteOffset { get; set; }

        public List<SpriteEntry> SpriteEntries { get; set; }

        public SpriteMetadataEntry(int spriteMetadataEntryOffset, int unkColor, short width, short height, float unkFloat, int count, int offset, List<SpriteEntry> sprites)
        {
            SpriteMetadataEntryOffset = spriteMetadataEntryOffset;
            Unkcolor = unkColor;
            Width = width;
            Height = height;
            UnkownFloat = unkFloat;
            SpriteCount = count;
            SpriteOffset = offset;
            SpriteEntries = sprites;
        }

        public static SpriteMetadataEntry ReadEntry(EndianBinaryReader reader, List<SpriteEntry> parentEntries)
        {
            int spriteMetadataEntryOffset = (int)reader.Position;

            int unkColor = reader.ReadInt32();

            short width = reader.ReadInt16();
            short height = reader.ReadInt16();

            float unkFloat = reader.ReadSingle();

            int spriteCount = reader.ReadInt32();
            int spriteOffset = reader.ReadInt32();

            var spriteEntries = new List<SpriteEntry>();
            reader.ReadAtOffsetAndSeekBack(spriteOffset, () =>
            {
                for (int i = 0; i < spriteCount; i++)
                {
                    SpriteEntry entry = parentEntries.FirstOrDefault(e => e.SpriteEntryOffset == reader.Position);
                    spriteEntries.Add(entry);
                }
            });

            return new SpriteMetadataEntry(spriteMetadataEntryOffset, unkColor, width, height, unkFloat, spriteCount, spriteOffset, spriteEntries);
        }

        public override string ToString()
        {
            return $"{Width}x{Height} - {SpriteEntries.FirstOrDefault()?.SpriteName ?? "NULL"}";
        }
    }
}
