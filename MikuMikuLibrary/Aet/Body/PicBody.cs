using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aet.Body
{
    public sealed class PicBody : AetObjBody, IAnimatable
    {
        public override BodyType BodyType
        {
            get => BodyType.PIC;
        }

        public int SpriteMetadataOffset { get; internal set; }
        public SpriteMetadataEntry SpriteMetadata { get; set; }

        public int PicReferenceOffset { get; internal set; }
        public AetObj ReferencedPic { get; set; }

        public int UnknownCount { get; internal set; }
        public int UnknownOffset { get; internal set; }

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

            reader.ReadAtOffset(AnimationDataOffset, () =>
            {
                AnimationData = AnimationData.Read(reader);
            });

            UnknownInt3 = reader.ReadInt32();
        }

        internal override void Write(EndianBinaryWriter writer, AetSection parentAet)
        {
            writer.Write(SpriteMetadata.ThisOffset);

            if (ReferencedPic != null)
            {
                writer.EnqueueDelayedWrite(sizeof(int), () =>
                {
                    writer.Write(ReferencedPic.ThisOffset);
                });
            }
            else
            {
                writer.Write(0x0);
            }

            //writer.Write(UnknownCount);
            writer.Write(0x0);
            //writer.Write(UnknownOffset);
            writer.Write(0x0);

            writer.ScheduleWriteOffset(() =>
            {
                AnimationData.Write(writer);
            });

            writer.Write(UnknownInt3);
        }
    }
}
