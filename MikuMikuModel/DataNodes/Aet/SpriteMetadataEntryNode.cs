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
    public class SpriteMetadataEntryNode : DataNode<SpriteMetadataEntry>
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
            get => Properties.Resources.Texture;
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        public SpriteMetadataEntryNode(string name, SpriteMetadataEntry data) : base(data.SpriteEntries.FirstOrDefault()?.SpriteName ?? name, data)
        {
            return;
        }
    }

}