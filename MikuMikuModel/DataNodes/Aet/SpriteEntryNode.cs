using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MikuMikuLibrary;
using MikuMikuLibrary.Animations.Aet;
using MikuMikuLibrary.Sprites;
using MikuMikuModel.GUI.Controls;
using LibAet = MikuMikuLibrary.Animations.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    public class SpriteEntryNode : DataNode<SpriteEntry>
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

        public override Control Control
        {
            get
            {
                if (Data.SpriteName != null)
                {
                    var parentAet = Parent.Parent.Data as LibAet.Aet;

                    // should be checked based on SpriteId
                    var sprite = parentAet.AssociatedSpriteSet.Sprites.FirstOrDefault(s => Data.SpriteName.Contains(s.Name));

                    if (sprite != null)
                    {
                        TextureViewControl.Instance.SetSprite(sprite);
                        return TextureViewControl.Instance;
                    }
                }

                return null;
            }
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        public SpriteEntryNode(string name, SpriteEntry data) : base(data.SpriteName ?? name, data)
        {
            return;
        }
    }

}