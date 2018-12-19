using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aet
{
    public class AetObjPairPointer
    {
        internal int ThisOffset { get; set; }

        public int AetObjCount { get; set; }

        public int AetObjPairOffset { get; internal set; }

        /// <summary>
        /// The <see cref="AetObjPair"/> pointed to by this <see cref="AetObjPairPointer"/>.
        /// </summary>
        public AetObjPair ReferencedObjectPair { get; set; }

        public AetObjPairPointer()
        {
            return;
        }

        public AetObjPairPointer(int count, int offset)
        {
            AetObjCount = count;
            AetObjPairOffset = offset;
        }

        internal static AetObjPairPointer Read(EndianBinaryReader reader)
        {
            int aetObjPairPointerOffset = (int)reader.Position;

            int count = reader.ReadInt32();
            int offset = reader.ReadInt32();

            return new AetObjPairPointer(count, offset)
            {
                ThisOffset = aetObjPairPointerOffset,
            };
        }

        internal void Write(EndianBinaryWriter writer, AetSet parentAet)
        {
            ThisOffset = (int)writer.Position;

            writer.Write(ReferencedObjectPair.AetObjects.Count);

            writer.EnqueueOffsetWrite(() =>
            {
                AetObjPairOffset = (int)writer.Position;
                ReferencedObjectPair.Write(writer, parentAet);
            });
        }

        public override string ToString()
        {
            return $"{AetObjCount} {nameof(AetObj)}{(AetObjCount == 1 ? string.Empty : "s")} at 0x{AetObjPairOffset:X4}";
        }
    }
}
