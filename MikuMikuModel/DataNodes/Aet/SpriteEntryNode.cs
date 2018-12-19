using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MikuMikuLibrary.Aet;
using MikuMikuModel.GUI.Controls;

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
                if (Data.Name != null)
                {
                    var parentAet = Parent.Parent.Data as AetSet;

                    // should be checked based on SpriteId
                    var sprite = parentAet.AssociatedSpriteSet.Sprites.FirstOrDefault(s => Data.Name.Contains(s.Name));

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

        public SpriteEntryNode(string name, SpriteEntry data) : base(data.Name ?? name, data)
        {
            return;
        }
    }

}