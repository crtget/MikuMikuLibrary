using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Animations.Aet
{
    public class SpriteEntry
    {
        public int SpriteEntryOffset { get; set; }

        public int SpriteNameOffset { get; set; }
        public string SpriteName { get; set; }

        public int SpriteId { get; set; }

        public SpriteEntry(int entryOffset, int nameOffset, string name, int id)
        {
            SpriteEntryOffset = entryOffset;
            SpriteNameOffset = nameOffset;
            SpriteName = name;
            SpriteId = id;
        }

        public static SpriteEntry ReadEntry(EndianBinaryReader reader)
        {
            long spriteEntryOffset = reader.Position;

            reader.ReadStringPtr(out long nameOffset, out string name, StringBinaryFormat.NullTerminated);
            int id = reader.ReadInt32();

            return new SpriteEntry((int)spriteEntryOffset, (int)nameOffset, name, id);
        }

        public override string ToString()
        {
            return $"ID: {SpriteId} - {SpriteName ?? "NULL"}";
        }
    }
}
