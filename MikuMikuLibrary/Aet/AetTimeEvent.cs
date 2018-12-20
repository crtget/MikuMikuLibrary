using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aet
{
    public sealed class AetTimeEvent
    {
        public float Frame { get; set; }

        public string Name { get; set; }

        public AetTimeEvent()
        {
            return;
        }

        public AetTimeEvent(float frame, string name)
        {
            Frame = frame;
            Name = name;
        }

        internal static AetTimeEvent Read(EndianBinaryReader reader)
        {
            float frame = reader.ReadSingle();
            string name = reader.ReadStringPtr(StringBinaryFormat.NullTerminated);

            return new AetTimeEvent(frame, name);
        }
       
        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write(Frame);
            writer.AddStringToStringTable(Name);
        }

        public override string ToString()
        {
            return $"Frame: {Frame} : {Name}";
        }
    }
}
