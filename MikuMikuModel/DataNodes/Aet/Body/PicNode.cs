using System.Drawing;
using MikuMikuLibrary.Aet;
using MikuMikuLibrary.Aet.Body;

namespace MikuMikuModel.DataNodes.Aet.Body
{
    public class PicNode : DataNode<PicBody>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Leaf;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        public override Bitmap Icon => Properties.Resources.Folder;

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        public PicNode(string name, PicBody data) : base(name, data)
        {
            return;
        }
    }
}