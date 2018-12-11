using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuLibrary;
using MikuMikuLibrary.Animations.Aet;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Sprites;
using LibAet = MikuMikuLibrary.Animations.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    public class AetNode : BinaryFileNode<LibAet.Aet>
    {
        public override DataNodeFlags Flags
        {
            get => DataNodeFlags.Branch;
        }

        public override DataNodeActionFlags ActionFlags
        {
            get => DataNodeActionFlags.None;
        }

        protected override void InitializeCore()
        {
            /// TODO: look for spr, <see cref="ModelNode.InitializeViewCore()"/>
        }

        protected override void InitializeViewCore()
        {
            Add(new ListNode<SpriteEntry>("Sprite Entries", Data.SpriteEntries));
            Add(new ListNode<SpriteMetadataEntry>("Sprite Metadata", Data.SpriteMetadataEntries));
            Add(new ListNode<AnimationPointerEntry>("Animation Pointer Table", Data.AnimationPointerTable));

            Add(new ListNode<AnimationPair>("Animation Pairs", Data.AnimationPairs));
        }

        public AetNode(string name, LibAet.Aet data) : base(name, data)
        {
            return;
        }
    }
}