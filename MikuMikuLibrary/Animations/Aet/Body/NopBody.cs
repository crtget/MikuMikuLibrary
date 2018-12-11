using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Animations.Aet.Body
{
    public sealed class NopBody : AnimationBody
    {
        public override BodyType BodyType
        {
            get => BodyType.NOP;
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
