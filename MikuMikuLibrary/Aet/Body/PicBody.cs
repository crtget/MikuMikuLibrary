using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aet.Body
{
    public sealed class PicBody : AetObjBody
    {
        public override BodyType BodyType
        {
            get => BodyType.PIC;
        }

        public int SpriteMetadataOffset { get; internal set; }
        public SpriteMetadataEntry SpriteMetadata { get; internal set; }

        public int PicReferenceOffset { get; internal set; }
        public AetObj ReferencedPic { get; internal set; }

        public int UnknownCount { get; set; }
        public int UnknownOffset { get; set; }

        public int AnimationDataOffset { get; internal set; }
        public AnimationData AnimationData { get; set; }

        public int UnknownInt3 { get; set; }

        internal override void Read(EndianBinaryReader reader)
        {
            SpriteMetadataOffset = reader.ReadInt32();

            PicReferenceOffset = reader.ReadInt32();

            UnknownCount = reader.ReadInt32();
            UnknownOffset = reader.ReadInt32();

            AnimationDataOffset = reader.ReadInt32();

            reader.ReadAtOffsetAndSeekBack(AnimationDataOffset, () =>
            {
                AnimationData = AnimationData.Read(reader);
            });

            UnknownInt3 = reader.ReadInt32();
        }

        internal override void Write(EndianBinaryWriter writer, AetSet parentAet)
        {
            writer.Write(SpriteMetadata.ThisOffset);

            // TODO: handle forward references
            //writer.Write(0x0);
            writer.Write(ReferencedPic != null && ReferencedPic.ThisOffset < writer.Position ? ReferencedPic.ThisOffset : 0x0);

            //writer.Write(UnknownCount);
            //writer.Write(UnknownOffset);
            writer.Write(0x0);
            writer.Write(0x0);

            writer.EnqueueOffsetWrite(() =>
            {
                AnimationData.Write(writer);
            });

            writer.Write(UnknownInt3);
        }
    }
}
