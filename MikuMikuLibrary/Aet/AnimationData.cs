using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aet
{
    public class AnimationData
    {
        public ShaderType Shader { get; set; }

        public short UnkownFlag { get; set; }

        public KeyFrameData ScaleXOrigin { get; set; }
        public KeyFrameData ScaleYOrigin { get; set; }

        public KeyFrameData PositionX { get; set; }
        public KeyFrameData PositionY { get; set; }

        public KeyFrameData Rotation { get; set; }

        public KeyFrameData ScaleX { get; set; }
        public KeyFrameData ScaleY { get; set; }

        public KeyFrameData Opacity { get; set; }

        public int UnknownInt { get; set; }

        internal static AnimationData Read(EndianBinaryReader reader)
        {
            var animationData = new AnimationData
            {
                Shader = (ShaderType)reader.ReadInt16(),
                UnkownFlag = reader.ReadInt16(),

                ScaleXOrigin = KeyFrameData.Read(reader),
                ScaleYOrigin = KeyFrameData.Read(reader),

                PositionX = KeyFrameData.Read(reader),
                PositionY = KeyFrameData.Read(reader),

                Rotation = KeyFrameData.Read(reader),

                ScaleX = KeyFrameData.Read(reader),
                ScaleY = KeyFrameData.Read(reader),

                Opacity = KeyFrameData.Read(reader),

                UnknownInt = reader.ReadInt32()
            };

            return animationData;
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write((short)Shader);
            writer.Write(UnkownFlag);

            ScaleXOrigin.Write(writer);
            ScaleYOrigin.Write(writer);

            PositionX.Write(writer);
            PositionY.Write(writer);

            Rotation.Write(writer);

            ScaleX.Write(writer);
            ScaleY.Write(writer);

            Opacity.Write(writer);

            writer.Write(UnknownInt);
        }
    }
}
