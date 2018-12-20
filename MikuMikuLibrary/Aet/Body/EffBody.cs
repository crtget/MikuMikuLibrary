using MikuMikuLibrary.IO.Common;
using System.Collections.Generic;

namespace MikuMikuLibrary.Aet.Body
{
    public sealed class EffBody : AetObjBody
    {
        public override BodyType BodyType
        {
            get => BodyType.EFF;
        }

        public int AetObjPairPointerOffset { get; internal set; }
        public AetObjPairPointer AetObjPairPointer { get; set; }

        public int UnknownInt0 { get; set; }

        public List<AetTimeEvent> TimeEvents { get; set; }

        public int AnimationDataOffset { get; internal set; }
        public AnimationData AnimationData { get; set; }

        public int UnknownEndOffset { get; set; }

        internal override void Read(EndianBinaryReader reader)
        {
            AetObjPairPointerOffset = reader.ReadInt32();

            UnknownInt0 = reader.ReadInt32();

            {
                int timeEventsCount = reader.ReadInt32();
                int timeEventsOffset = reader.ReadInt32();

                TimeEvents = new List<AetTimeEvent>(timeEventsCount);
                if (timeEventsOffset > 0 && timeEventsCount > 0)
                {
                    reader.ReadAtOffsetAndSeekBack(timeEventsOffset, () =>
                    {
                        for (int i = 0; i < timeEventsCount; i++)
                            TimeEvents.Add(AetTimeEvent.Read(reader));
                    });
                }
            }

            AnimationDataOffset = reader.ReadInt32();

            reader.ReadAtOffsetAndSeekBack(AnimationDataOffset, () =>
            {
                AnimationData = AnimationData.Read(reader);
            });

            UnknownEndOffset = reader.ReadInt32();
        }

        internal override void Write(EndianBinaryWriter writer, AetSection parentAet)
        {
            writer.Write(AetObjPairPointer.ThisOffset);

            writer.Write(0x0);

            writer.Write(TimeEvents.Count);
            if (TimeEvents.Count > 0)
            {
                writer.EnqueueOffsetWrite(() =>
                {
                    foreach (var timeEvent in TimeEvents)
                        timeEvent.Write(writer);
                });
            }
            else
            {
                writer.Write(0x0);
            }


            writer.EnqueueOffsetWrite(() =>
            {
                AnimationData.Write(writer);
            });

            //writer.Write(UnknownEndOffset);
            writer.Write(0x0);
        }
    }
}
