using System;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using System.Collections.Generic;
using MikuMikuLibrary.IO.Sections;
using System.Linq;
using MikuMikuLibrary.Animations.Aet.Body;
using MikuMikuLibrary.Sprites;

namespace MikuMikuLibrary.Animations.Aet
{
    public class Aet : BinaryFile
    {
        public static readonly string MAIN_STRING = "MAIN";
        public static readonly string TOUCH_STRING = "TOUCH";

        public override BinaryFileFlags Flags
        {
            get => BinaryFileFlags.Load;
        }

        public int MainMetadataOffset { get; set; }
        public int TouchMetadataOffset { get; set; }

        public string TouchString { get; set; }
        public string MainString { get; set; }

        public float FrameDuration { get; set; }

        /// <summary>
        /// Defines the frame unit duration used by this <see cref="Aet"/>'s <see cref="Animation"/>s.
        /// </summary>
        public float FrameRate { get; set; }

        /// <summary>
        /// The original screen width the animation coordinates were designed for.
        /// </summary>
        public int ResolutionWidth { get; set; }

        /// <summary>
        /// The original screen height the animation coordinates were designed for.
        /// </summary>
        public int ResolutionHeight { get; set; }

        public int AnimationPointerTableSize { get; set; }
        public int AnimationPointerTableOffset { get; set; }

        public int SpriteMetadataTableSize { get; set; }
        public int SpriteMetadataTableOffset { get; set; }

        public List<AnimationPointerEntry> AnimationPointerTable { get; set; } = new List<AnimationPointerEntry>();

        public List<SpriteEntry> SpriteEntries { get; set; } = new List<SpriteEntry>();

        public List<SpriteMetadataEntry> SpriteMetadataEntries { get; set; } = new List<SpriteMetadataEntry>();

        public List<AnimationPair> AnimationPairs { get; set; } = new List<AnimationPair>();

        /// <summary>
        /// The <see cref="SpriteSet"/> associated with this <see cref="Aet"/>.
        /// <see cref="Aet"/>s can also draw <see cref="Sprite"/>s outside their scope.
        /// </summary>
        public SpriteSet AssociatedSpriteSet { get; set; }

        public override void Read(EndianBinaryReader reader, Section section = null)
        {
            // header
            {
                MainMetadataOffset = reader.ReadInt32();
                TouchMetadataOffset = reader.ReadInt32();

                reader.ReadInt32(); // unknown
                reader.ReadInt32(); // unknown
            }

            // metadata
            reader.ReadAtOffsetAndSeekBack(MainMetadataOffset, () =>
            {
                MainString = reader.ReadStringPtr(StringBinaryFormat.NullTerminated);

                reader.ReadSingle(); // unknown

                FrameDuration = reader.ReadSingle();
                FrameRate = reader.ReadSingle();

                reader.ReadInt32(); // maybe some color

                ResolutionWidth = reader.ReadInt32();
                ResolutionHeight = reader.ReadInt32();

                reader.ReadInt32(); // unknown, crash if != 0

                // animation pointer table
                {
                    AnimationPointerTableSize = reader.ReadInt32();
                    AnimationPointerTableOffset = reader.ReadInt32();

                    reader.ReadAtOffsetAndSeekBack(AnimationPointerTableOffset, () =>
                    {
                        for (int i = 0; i < AnimationPointerTableSize; i++)
                        {
                            var animationPointerEntry = AnimationPointerEntry.ReadEntry(reader);
                            AnimationPointerTable.Add(animationPointerEntry);
                        }
                    });
                }

                // sprite metadata
                {
                    SpriteMetadataTableSize = reader.ReadInt32();
                    SpriteMetadataTableOffset = reader.ReadInt32();
                }

                // unknown
                {
                    reader.ReadInt32(); // size
                    reader.ReadInt32(); // offset

                    reader.ReadInt32(); // size
                    reader.ReadInt32(); // offset
                }
            });

            // sprite atlas
            {
                for (int spriteCount = 0; reader.Position < SpriteMetadataTableOffset; spriteCount++)
                {
                    SpriteEntry spriteEntry = SpriteEntry.ReadEntry(reader);

                    SpriteEntries.Add(spriteEntry);
                }
            }

            // sprite metadata
            {
                reader.ReadAtOffset(SpriteMetadataTableOffset, () =>
                {
                    for (int i = 0; i < SpriteMetadataTableSize; i++)
                    {
                        SpriteMetadataEntry spriteMetadataEntry = SpriteMetadataEntry.ReadEntry(reader, SpriteEntries);
                        SpriteMetadataEntries.Add(spriteMetadataEntry);
                    }
                });
            }

            // animations
            {
                for (int i = 0; i < AnimationPointerTable.Count; i++)
                {
                    var animationEntry = AnimationPointerTable[i];

                    reader.ReadAtOffset(animationEntry.AnimationOffset, () =>
                    {
                        var subAnimations = new List<Animation>();

                        for (int j = 0; j < animationEntry.SubAnimationCount; j++)
                        {
                            Animation animation = new Animation();
                            animation.Read(reader, this);

                            subAnimations.Add(animation);
                        }

                        AnimationPair animationPair = new AnimationPair(subAnimations);
                        AnimationPairs.Add(animationPair);

                        AnimationPointerTable[i].ReferencedAnimationPair = animationPair;
                    });
                }
            }

            // link animations
            {
                foreach (var animationPair in AnimationPairs)
                {
                    foreach (var animation in animationPair.Animations)
                    {
                        if (animation.AnimationBody is PicBody picBody)
                        {
                            picBody.SpriteMetadata = SpriteMetadataEntries.FirstOrDefault(e => e.SpriteMetadataEntryOffset == picBody.SpriteMetadataOffset);
                        }
                        else if (animation.AnimationBody is EffBody effBody)
                        {
                            effBody.AnimationPointerEntry = AnimationPointerTable.FirstOrDefault(e => e.AnimationPointerEntryOffset == effBody.AnimationPointerEntryOffset);

                            /// name the <see cref="AnimationPair.PairName"/> of the <see cref="AnimationPair"/> referenced by the EFF
                            /// I'm not sure if this is correct because technically multiple *different* EFFs can reference the same <see cref="AnimationPair"/>
                            /// but this greatly improves readability and it shouldn't be a problem with how normal aets are layed out

                            //string pairName = effBody.AnimationPointerEntry.ReferencedAnimationPair.PairName;
                            //if (pairName != null && pairName != animation.AnimationName)
                            //    System.Diagnostics.Debugger.Break();

                            effBody.AnimationPointerEntry.ReferencedAnimationPair.PairName = animation.AnimationName;
                        }
                    }
                }
            }
        }

        public override void Write(EndianBinaryWriter writer, Section section = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts a frame unit to a <see cref="TimeSpan"/>.
        /// </summary>
        public TimeSpan GetTimeSpan(float frame)
        {
            return TimeSpan.FromSeconds(1f / FrameRate * frame);
        }

        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to a frame unit.
        /// </summary>
        public float GetFrame(TimeSpan time)
        {
            return (float)(time.TotalSeconds / 1f * FrameRate);
        }
    }
}
