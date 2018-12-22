using System.Drawing;
using MikuMikuLibrary.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    public class AetObjPairPointerNode : DataNode<AetObjPairPointer>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Leaf;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        public override Bitmap Icon => Properties.Resources.Node;

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        public AetObjPairPointerNode(string name, AetObjPairPointer data) : base(data.ToString() ?? name, data)
        {
            return;
        }
    }

}