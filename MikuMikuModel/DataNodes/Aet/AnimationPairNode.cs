using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuLibrary;
using MikuMikuLibrary.Animations.Aet;
using LibAet = MikuMikuLibrary.Animations.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    [DataNodeSpecialName("Animation Pair")]
    public class AnimationPairNode : DataNode<AnimationPair>
    {
        public override DataNodeFlags Flags
        {
            get => DataNodeFlags.Branch;
        }

        public override DataNodeActionFlags ActionFlags
        {
            get => DataNodeActionFlags.None;
        }

        public override Bitmap Icon
        {
            get => Properties.Resources.Node;
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
            foreach (var animation in Data.Animations)
            {
                Add(DataNodeFactory.Create<Animation>(animation.AnimationName ?? "NULL", animation));
            }
        }

        public AnimationPairNode(string name, AnimationPair data) : base(data.PairName ?? name, data)
        {
            return;
        }
    }
}