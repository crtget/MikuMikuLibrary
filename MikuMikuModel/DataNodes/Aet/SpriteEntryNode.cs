using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MikuMikuLibrary.Aet;
using MikuMikuModel.GUI.Controls;

namespace MikuMikuModel.DataNodes.Aet
{
    public class SpriteEntryNode : DataNode<SpriteEntry>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Leaf;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        public override Bitmap Icon => Properties.Resources.Texture;

        public override Control Control
        {
            get
            {
#if false
                if (Data.Name != null)
                {
                    var parentAet = Parent.Parent.Parent.Data as AetSet;

                    // should be checked based on SpriteId
                    var sprite = parentAet?.AssociatedSpriteSet?.Sprites.FirstOrDefault(s => Data.Name.EndsWith(s.Name));

                    if (sprite != null)
                    {
                        TextureViewControl.Instance.SetSprite(sprite);
                        return TextureViewControl.Instance;
                    }
                }
#endif

                return base.Control;
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