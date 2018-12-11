using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Animations.Aet.Body
{
    public sealed class AifBody : AnimationBody
    {
        public override BodyType BodyType
        {
            get => BodyType.AIF;
        }

        internal override void Read(EndianBinaryReader reader)
        {
            return;
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            return;
        }
    }
}
