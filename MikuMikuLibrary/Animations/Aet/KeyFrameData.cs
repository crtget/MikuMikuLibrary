using MikuMikuLibrary.IO.Common;
using System.Collections.Generic;

namespace MikuMikuLibrary.Animations.Aet
{
    public class KeyFrameData
    {
        public int KeyFrameCount { get; set; }

        public int KeyFramesOffset { get; set; }

        public List<KeyFrame> KeyFrames { get; set; }

        public static KeyFrameData Read(EndianBinaryReader reader)
        {
            KeyFrameData keyFrameData = new KeyFrameData
            {
                KeyFrameCount = reader.ReadInt32(),
                KeyFramesOffset = reader.ReadInt32()
            };

            reader.ReadAtOffsetAndSeekBack(keyFrameData.KeyFramesOffset, () =>
            {
                keyFrameData.KeyFrames = new List<KeyFrame>();

                int count = keyFrameData.KeyFrameCount;
                if (count != 1)
                {
                    float[] frames = new float[count];
                    for (int i = 0; i < count; i++)
                        frames[i] = reader.ReadSingle();

                    for (int i = 0; i < count; i++)
                    {
                        keyFrameData.KeyFrames.Add(new KeyFrame(
                            frame: frames[i],
                            value: reader.ReadSingle(),
                            bounciness: reader.ReadSingle()));
                    }
                }
                else
                {
                    keyFrameData.KeyFrames.Add(new KeyFrame(reader.ReadSingle()));
                }
            });

            return keyFrameData;
        }

        public override string ToString()
        {
            return $"Key Frames: {KeyFrameCount}";
        }
    }
}
