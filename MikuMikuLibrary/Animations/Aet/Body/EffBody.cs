using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Animations.Aet.Body
{
    public sealed class EffBody : AnimationBody
    {
        public override BodyType BodyType
        {
            get => BodyType.EFF;
        }

        public int AnimationPointerEntryOffset { get; set; }
        public AnimationPointerEntry AnimationPointerEntry { get; set; }

        public int UnknownInt0 { get; set; }
        public int UnknownInt1 { get; set; }
        public int UnknownInt2 { get; set; }

        public int AnimationDataOffset { get; set; }
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

        internal override void Write(EndianBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
