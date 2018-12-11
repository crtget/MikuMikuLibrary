using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuLibrary;
using MikuMikuLibrary.Animations.Aet;
using LibAet = MikuMikuLibrary.Animations.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    public class AnimationNode : DataNode<Animation>
    {
        public override DataNodeFlags Flags
        {
            get => DataNodeFlags.Leaf;
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
        }

        public AnimationNode(string name, Animation data) : base(data.AnimationName ?? name, data)
        {
            return;
        }
    }
}