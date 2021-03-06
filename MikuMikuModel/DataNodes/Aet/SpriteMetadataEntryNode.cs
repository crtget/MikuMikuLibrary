﻿using System.Drawing;
using System.Linq;
using MikuMikuLibrary.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    public class SpriteMetadataEntryNode : DataNode<SpriteMetadataEntry>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Leaf;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        public override Bitmap Icon => Properties.Resources.Texture;

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        public SpriteMetadataEntryNode(string name, SpriteMetadataEntry data) : base(data.ReferencedSprites.FirstOrDefault()?.Name ?? name, data)
        {
            return;
        }
    }

}