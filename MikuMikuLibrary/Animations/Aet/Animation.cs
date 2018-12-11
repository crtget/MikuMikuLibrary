using MikuMikuLibrary.Animations.Aet.Body;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Animations.Aet
{
    /// <summary>
    /// A container for an <see cref="Body.AnimationBody"/>.
    /// </summary>
    public sealed class Animation
    {
        public int AnimationNameOffset { get; set; }
        public string AnimationName { get; set; }

        public float FrameStartTime { get; set; }
        public float FrameLoopDuration { get; set; }
        public float FrameStartTimeAlt { get; set; }
        public float PlaybackSpeed { get; set; }

        public byte UnkownByte0 { get; set; }
        public byte UnkownByte1 { get; set; }
        public byte UnkownByte2 { get; set; }

        public AnimationBody AnimationBody { get; set; }

        internal void Read(EndianBinaryReader reader, Aet parentAet)
        {
            AnimationName = reader.ReadStringPtr(StringBinaryFormat.NullTerminated);

            FrameStartTime = reader.ReadSingle();
            FrameLoopDuration = reader.ReadSingle();
            FrameStartTimeAlt = reader.ReadSingle();
            PlaybackSpeed = reader.ReadSingle();

            UnkownByte0 = reader.ReadByte();
            UnkownByte1 = reader.ReadByte();
            UnkownByte2 = reader.ReadByte();

            BodyType bodyType = (BodyType)reader.ReadByte();

            switch (bodyType)
            {
                case BodyType.NOP:
                    AnimationBody = new NopBody();
                    break;
                case BodyType.PIC:
                    AnimationBody = new PicBody();
                    break;
                case BodyType.AIF:
                    AnimationBody = new AifBody();
                    break;
                case BodyType.EFF:
                    AnimationBody = new EffBody();
                    break;
                default:
                    AnimationBody = null;
                    break;
            }

            AnimationBody?.Read(reader);
        }

        public override string ToString()
        {
            return $"[{AnimationBody?.BodyType}] - {AnimationName ?? "NULL"}";
        }
    }
}
