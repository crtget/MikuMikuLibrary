using MikuMikuLibrary.Aet.Body;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aet
{
    /// <summary>
    /// A container for an <see cref="AetObjBody"/>.
    /// </summary>
    public sealed class AetObj
    {
        internal int ThisOffset { get; set; }

        public string Name { get; set; }

        public float FrameStartTime { get; set; }
        public float FrameLoopDuration { get; set; }
        public float FrameStartTimeAlt { get; set; }
        public float PlaybackSpeed { get; set; }

        public byte UnkownByte0 { get; set; }
        public byte UnkownByte1 { get; set; }
        public byte UnkownByte2 { get; set; }

        public AetObjBody ObjectBody { get; set; }

        internal void Read(EndianBinaryReader reader, AetSet parentAet)
        {
            ThisOffset = (int)reader.Position;

            Name = reader.ReadStringPtr(StringBinaryFormat.NullTerminated);

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
                    ObjectBody = new NopBody();
                    break;
                case BodyType.PIC:
                    ObjectBody = new PicBody();
                    break;
                case BodyType.AIF:
                    ObjectBody = new AifBody();
                    break;
                case BodyType.EFF:
                    ObjectBody = new EffBody();
                    break;
                default:
                    ObjectBody = null;
                    break;
            }

            ObjectBody?.Read(reader);
        }

        internal void Write(EndianBinaryWriter writer, AetSet parentAet)
        {
            ThisOffset = (int)writer.Position;

            writer.AddStringToStringTable(Name);

            writer.Write(FrameStartTime);
            writer.Write(FrameLoopDuration);
            writer.Write(FrameStartTimeAlt);
            writer.Write(PlaybackSpeed);

            writer.Write(UnkownByte0);
            writer.Write(UnkownByte1);
            writer.Write(UnkownByte2);

            writer.Write((byte)ObjectBody.BodyType);
            ObjectBody.Write(writer, parentAet);
        }

        public override string ToString()
        {
            return $"[{ObjectBody?.BodyType}] - {Name ?? "NULL"}";
        }
    }
}
