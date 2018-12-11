using MikuMikuLibrary.IO.Common;
using System.Linq;

namespace MikuMikuLibrary.Animations.Aet.Body
{
    public sealed class PicBody : AnimationBody
    {
        public override BodyType BodyType
        {
            get => BodyType.PIC;
        }

        public int SpriteMetadataOffset { get; set; }
        public SpriteMetadataEntry SpriteMetadata { get; set; }

        public int UnknownInt0 { get; set; }
        public int UnknownInt1 { get; set; }
        public int UnknownInt2 { get; set; }

        public int AnimationDataOffset { get; set; }
        public AnimationData AnimationData { get; set; }

        public int UnknownInt3 { get; set; }

        internal override void Read(EndianBinaryReader reader)
        {
            SpriteMetadataOffset = reader.ReadInt32();

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

        internal override void Write(EndianBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
