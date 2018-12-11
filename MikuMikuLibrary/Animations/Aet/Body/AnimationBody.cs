using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Animations.Aet.Body
{
    public abstract class AnimationBody
    {
        public abstract BodyType BodyType { get; }

        internal abstract void Read(EndianBinaryReader reader);

        internal abstract void Write(EndianBinaryWriter writer);

        public override string ToString()
        {
            return $"{nameof(AnimationBody)} : {BodyType}";
        }
    }
}
