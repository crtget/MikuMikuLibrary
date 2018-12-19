using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aet.Body
{
    public sealed class EffBody : AetObjBody
    {
        public override BodyType BodyType
        {
            get => BodyType.EFF;
        }

        public int AnimationPointerEntryOffset { get; internal set; }
        public AetObjPairPointer AnimationPointerEntry { get; set; }

        public int UnknownInt0 { get; set; }
        public int UnknownInt1 { get; set; }
        public int UnknownInt2 { get; set; }

        public int AnimationDataOffset { get; internal set; }
        public AnimationData AnimationData { get; set; }

        public int UnknownInt3 { get; set; }

        internal override void Read(EndianBinaryReader reader)
        {
            AnimationPointerEntryOffset = reader.ReadInt32();

            UnknownInt0 = reader.ReadInt32();
            UnknownInt1 = reader.ReadInt32();
            UnknownInt2 = reader.ReadInt32();

            AnimationDataOffset = reader.ReadInt32();

            reader.ReadAtOffsetAndSeekBack(AnimationDataOffset, () =>
            {
                AnimationData = AnimationData.Read(reader);
            });

            UnknownInt3 = reader.ReadInt32();
        }

        internal override void Write(EndianBinaryWriter writer, AetSet parentAet)
        {
            writer.Write(AnimationPointerEntry.ThisOffset);

            writer.Write(UnknownInt0);
            writer.Write(UnknownInt1);
            writer.Write(UnknownInt2);

            writer.EnqueueOffsetWrite(() =>
            {
                AnimationData.Write(writer);
            });

            writer.Write(UnknownInt3);
        }
    }
}
