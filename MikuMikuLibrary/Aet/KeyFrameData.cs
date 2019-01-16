using MikuMikuLibrary.IO.Common;
using System.Collections.Generic;

namespace MikuMikuLibrary.Aet
{
    public class KeyFrameData
    {
        public int KeyFramesOffset { get; internal set; }

        public List<KeyFrame> KeyFrames { get; set; }

        public KeyFrameData()
        {
            return;
        }

        public static KeyFrameData Read(EndianBinaryReader reader)
        {
            int keyFrameCount = reader.ReadInt32();
            int keyFramesOffset = reader.ReadInt32();

            List<KeyFrame> keyFrames = new List<KeyFrame>(keyFrameCount);

            reader.ReadAtOffsetAndSeekBack(keyFramesOffset, () =>
            {
                keyFrames = new List<KeyFrame>(keyFrameCount);

                if (keyFrameCount == 1)
                {
                    float value = reader.ReadSingle();
                    keyFrames.Add(new KeyFrame(value));
                }
                else
                {
                    float[] frames = new float[keyFrameCount];

                    for (int i = 0; i < keyFrameCount; i++)
                        frames[i] = reader.ReadSingle();

                    for (int i = 0; i < keyFrameCount; i++)
                    {
                        keyFrames.Add(new KeyFrame(
                            frame: frames[i],
                            value: reader.ReadSingle(),
                            bounciness: reader.ReadSingle()));
                    }
                }
            });

            return new KeyFrameData()
            {
                KeyFramesOffset = keyFramesOffset,
                KeyFrames = keyFrames,
            };
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write(KeyFrames.Count);
            writer.ScheduleWriteOffset(() =>
            {
                if (KeyFrames.Count == 1)
                {
                    writer.Write(KeyFrames[0].Value);
                }
                else
                {
                    foreach (var keyFrame in KeyFrames)
                        writer.Write(keyFrame.Frame);

                    foreach (var keyFrame in KeyFrames)
                    {
                        writer.Write(keyFrame.Value);
                        writer.Write(keyFrame.Bounciness);
                    }
                }
            });
        }

        public override string ToString()
        {
            return $"{nameof(KeyFrame)}s: {KeyFrames?.Count}";
        }
    }
}
