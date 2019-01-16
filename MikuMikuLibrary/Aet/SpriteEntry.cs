using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aet
{
    public class SpriteEntry
    {
        internal int ThisOffset { get; set; }

        /// <summary>
        /// <see cref="Databases.SpriteEntry.Name"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <see cref="Databases.SpriteEntry.ID"/>
        /// </summary>
        public int SpriteId { get; set; }

        public SpriteEntry(string name, int id)
        {
            Name = name;
            SpriteId = id;
        }

        public static SpriteEntry Read(EndianBinaryReader reader)
        {
            long thisOffset = reader.Position;

            string name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
            int id = reader.ReadInt32();

            return new SpriteEntry(name, id)
            {
                ThisOffset = (int)thisOffset,
            };
        }

        internal void Write(EndianBinaryWriter writer)
        {
            ThisOffset = (int)writer.Position;

            writer.AddStringToStringTable(Name);
            writer.Write(SpriteId);
        }

        public override string ToString()
        {
            return $"ID: {SpriteId} - {Name ?? "NULL"}";
        }
    }
}
