﻿using System;
using System.Linq;
using System.Collections.Generic;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Aet.Body;
using MikuMikuLibrary.Sprites;

namespace MikuMikuLibrary.Aet
{
    /// <summary>
    /// An <see cref="AetSet"/> is an <see cref="AetObj"/> container stored inside an aet file.
    /// </summary>
    public class AetSet : BinaryFile
    {
        public static readonly string MAIN_STRING = "MAIN";
        public static readonly string TOUCH_STRING = "TOUCH";

        public override BinaryFileFlags Flags
        {
            get => BinaryFileFlags.Load | BinaryFileFlags.Save;
        }

        public string MainString { get; set; }
        public string TouchString { get; set; }

        public float FrameDuration { get; set; }

        /// <summary>
        /// Defines the frame unit duration used by this <see cref="AetSet"/>'s <see cref="AetObj"/>s.
        /// </summary>
        public float FrameRate { get; set; }

        public uint UnknownColor { get; set; }

        /// <summary>
        /// The original screen width the animation coordinates were designed for.
        /// </summary>
        public int ResolutionWidth { get; set; }

        /// <summary>
        /// The original screen height the animation coordinates were designed for.
        /// </summary>
        public int ResolutionHeight { get; set; }

        public List<AetObjPairPointer> AetObjPairPointerTable { get; set; }

        public List<SpriteEntry> SpriteEntries { get; set; }

        public List<SpriteMetadataEntry> SpriteMetadataEntries { get; set; }

        public List<AetObjPair> AetObjPairs { get; set; }

        /// <summary>
        /// The <see cref="SpriteSet"/> associated with this <see cref="AetSet"/>.
        /// <see cref="AetSet"/>s can also draw <see cref="Sprite"/>s outside their scope.
        /// </summary>
        public SpriteSet AssociatedSpriteSet { get; set; }

        public AetSet()
        {
            MainString = MAIN_STRING;

            FrameRate = 60f;
            ResolutionWidth = 1280;
            ResolutionHeight = 720;
        }

        public override void Read(EndianBinaryReader reader, Section section = null)
        {
            // header
            int mainMetadataOffset = reader.ReadInt32();
            int touchMetadataOffset = reader.ReadInt32();

            reader.ReadInt32(); // unknown
            reader.ReadInt32(); // unknown

            int spriteMetadataTableSize = default(int);
            int spriteMetadataTableOffset = default(int);

            // metadata
            reader.ReadAtOffsetAndSeekBack(mainMetadataOffset, () =>
            {
                MainString = reader.ReadStringPtr(StringBinaryFormat.NullTerminated);

                reader.ReadSingle(); // unknown

                FrameDuration = reader.ReadSingle();
                FrameRate = reader.ReadSingle();

                UnknownColor = reader.ReadUInt32(); // maybe some color

                ResolutionWidth = reader.ReadInt32();
                ResolutionHeight = reader.ReadInt32();

                reader.ReadInt32(); // unknown, crash if != 0

                // object pointer table
                {
                    int objPairCount = reader.ReadInt32();
                    int tableOffset = reader.ReadInt32();

                    reader.ReadAtOffsetAndSeekBack(tableOffset, () =>
                    {
                        AetObjPairPointerTable = new List<AetObjPairPointer>(objPairCount);

                        for (int i = 0; i < objPairCount; i++)
                        {
                            var pairPointer = AetObjPairPointer.Read(reader);
                            AetObjPairPointerTable.Add(pairPointer);
                        }
                    });
                }

                // sprite metadata
                {
                    spriteMetadataTableSize = reader.ReadInt32();
                    spriteMetadataTableOffset = reader.ReadInt32();
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
                const int SIZE_OF_SPRITE_ENTRY = sizeof(int) * 2;
                int predictedCapacity = (spriteMetadataTableOffset - (int)reader.Position) / SIZE_OF_SPRITE_ENTRY;

                SpriteEntries = new List<SpriteEntry>(predictedCapacity);

                for (int spriteCount = 0; reader.Position < spriteMetadataTableOffset; spriteCount++)
                {
                    SpriteEntry spriteEntry = SpriteEntry.Read(reader);
                    SpriteEntries.Add(spriteEntry);
                }
            }

            // sprite metadata
            {
                reader.ReadAtOffset(spriteMetadataTableOffset, () =>
                {
                    SpriteMetadataEntries = new List<SpriteMetadataEntry>(spriteMetadataTableSize);

                    for (int i = 0; i < spriteMetadataTableSize; i++)
                    {
                        SpriteMetadataEntry spriteMetadataEntry = SpriteMetadataEntry.Read(reader, SpriteEntries);
                        SpriteMetadataEntries.Add(spriteMetadataEntry);
                    }
                });
            }

            // aet objects
            {
                AetObjPairs = new List<AetObjPair>(AetObjPairPointerTable.Count);

                for (int i = 0; i < AetObjPairPointerTable.Count; i++)
                {
                    var animationEntry = AetObjPairPointerTable[i];

                    reader.ReadAtOffset(animationEntry.AetObjPairOffset, () =>
                    {
                        int objectCount = animationEntry.AetObjCount;
                        var aetObjects = new List<AetObj>(objectCount);

                        for (int j = 0; j < objectCount; j++)
                        {
                            AetObj aetObj = new AetObj();
                            aetObj.Read(reader, this);

                            aetObjects.Add(aetObj);
                        }

                        AetObjPair aetObjPair = new AetObjPair(aetObjects);
                        AetObjPairs.Add(aetObjPair);

                        AetObjPairPointerTable[i].ReferencedObjectPair = aetObjPair;
                    });
                }
            }

            // link object references
            {
                foreach (var objectPair in AetObjPairs)
                {
                    foreach (var aetObj in objectPair.AetObjects)
                    {
                        if (aetObj.ObjectBody is PicBody picBody)
                        {
                            picBody.SpriteMetadata = SpriteMetadataEntries.FirstOrDefault(e => e.ThisOffset == picBody.SpriteMetadataOffset);
                        }
                        else if (aetObj.ObjectBody is EffBody effBody)
                        {
                            effBody.AnimationPointerEntry = AetObjPairPointerTable.FirstOrDefault(e => e.ThisOffset == effBody.AnimationPointerEntryOffset);

                            /// name the <see cref="AetObjPair.PairName"/> of the <see cref="AetObjPair"/> referenced by the EFF
                            /// I'm not sure if this is correct because technically multiple *different* EFFs can reference the same <see cref="AetObjPair"/>
                            /// but this greatly improves readability and it shouldn't be a problem with how normal aets are layed out

                            //string pairName = effBody.AnimationPointerEntry.ReferencedObjectPair.PairName;
                            //if (pairName != null && pairName != aetObj.Name)
                            //    System.Diagnostics.Debugger.Break();

                            effBody.AnimationPointerEntry.ReferencedObjectPair.PairName = aetObj.Name;
                        }
                    }
                }
            }
        }

        public override void Write(EndianBinaryWriter writer, Section section = null)
        {
            writer.Write(0x0); // MAIN
            writer.Write(0x0); // TOUCH
            writer.Write(0x0);
            writer.Write(0x0);

            foreach (var spriteEntry in SpriteEntries)
            {
                spriteEntry.Write(writer);
            }

            int spriteMetadataPointer = (int)writer.Position;
            foreach (var spriteMetadataEntry in SpriteMetadataEntries)
            {
                spriteMetadataEntry.Write(writer);
            }

            // metadata
            writer.WriteAtOffsetAndSeekBack(0x0, () =>
            {
                writer.EnqueueOffsetWrite(() =>
                {
                    writer.AddStringToStringTable(MainString);

                    writer.Write(0f);
                    writer.Write(FrameDuration);
                    writer.Write(FrameRate);
                    writer.Write(UnknownColor);

                    writer.Write(ResolutionWidth);
                    writer.Write(ResolutionHeight);

                    writer.Write(0x0);

                    // animation pointer table
                    writer.Write(AetObjPairPointerTable.Count);
                    writer.EnqueueOffsetWrite(() =>
                    {
                        foreach (var aetObjPtr in AetObjPairPointerTable)
                            aetObjPtr.Write(writer, this);
                    });

                    writer.Write(SpriteMetadataEntries.Count);
                    writer.Write(spriteMetadataPointer);

                    writer.Write(0x0); // size
                    writer.Write(0x0); // offset

                    writer.Write(0x0); // size
                    writer.Write(0x0); // offset
                });
            });
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
