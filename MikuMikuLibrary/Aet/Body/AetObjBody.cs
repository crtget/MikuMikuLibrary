using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aet.Body
{
    public abstract class AetObjBody
    {
        public abstract BodyType BodyType { get; }

        internal abstract void Read(EndianBinaryReader reader);

        internal abstract void Write(EndianBinaryWriter writer, AetSection parentAet);

        public override string ToString()
        {
            return $"{nameof(AetObjBody)} : {BodyType}";
        }
    }
}
