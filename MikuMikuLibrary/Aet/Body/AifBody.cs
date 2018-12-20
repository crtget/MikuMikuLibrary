using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aet.Body
{
    public sealed class AifBody : AetObjBody
    {
        public override BodyType BodyType
        {
            get => BodyType.AIF;
        }

        internal override void Read(EndianBinaryReader reader)
        {
            return;
        }

        internal override void Write(EndianBinaryWriter writer, AetSection parentAet)
        {
            return;
        }
    }
}
