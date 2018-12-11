using System.Collections.Generic;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Animations.Aet
{
    public class AnimationPointerEntry
    {
        public int AnimationPointerEntryOffset { get; set; }

        public int SubAnimationCount { get; set; }

        public int AnimationOffset { get; set; }

        /// <summary>
        /// The <see cref="AnimationPair"/> pointed to by this <see cref="AnimationPointerEntry"/>.
        /// </summary>
        public AnimationPair ReferencedAnimationPair { get; set; }

        public AnimationPointerEntry(int animationPointerEntryOffset, int count, int offset)
        {
            AnimationPointerEntryOffset = animationPointerEntryOffset;
            SubAnimationCount = count;
            AnimationOffset = offset;
            ReferencedAnimationPair = null;
        }

        internal static AnimationPointerEntry ReadEntry(EndianBinaryReader reader)
        {
            int animationPointerEntryOffset = (int)reader.Position;

            int count = reader.ReadInt32();
            int offset = reader.ReadInt32();

            return new AnimationPointerEntry(animationPointerEntryOffset, count, offset);
        }

        public override string ToString()
        {
            string animationString = SubAnimationCount == 1 ? "Animation" : "Animations";
            return $"{SubAnimationCount} {animationString} at 0x{AnimationOffset:X4}";
        }
    }
}
