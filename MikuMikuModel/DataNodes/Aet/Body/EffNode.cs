using System.Drawing;
using MikuMikuLibrary.Aet;
using MikuMikuLibrary.Aet.Body;

namespace MikuMikuModel.DataNodes.Aet.Body
{
    public class EffNode : DataNode<EffBody>
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

        public EffNode(string name, EffBody data) : base(name, data)
        {
            return;
        }
    }
}