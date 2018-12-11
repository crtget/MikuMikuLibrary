using System.Collections.Generic;
using MikuMikuLibrary.Animations.Aet.Body;

namespace MikuMikuLibrary.Animations.Aet
{
    /// <summary>
    /// A set of animations referenced by an <see cref="AnimationPointerEntry"/>.
    /// </summary>
    public class AnimationPair
    {
        /// <summary>
        /// The name given to this <see cref="AnimationPair"/> by an <see cref="EffBody"/>.
        /// </summary>
        public string PairName { get; set; }

        /// <summary>
        /// The <see cref="List{Animation}"/> that make up this <see cref="AnimationPair"/>.
        /// </summary>
        public List<Animation> Animations;

        public AnimationPair()
        {
            return;
        }

        public AnimationPair(List<Animation> animations)
        {
            Animations = animations;
        }
    }
}