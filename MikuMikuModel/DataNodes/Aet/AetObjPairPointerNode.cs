using System.Drawing;
using MikuMikuLibrary.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    public class AetObjPairPointerNode : DataNode<AetObjPairPointer>
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

        public AetObjPairPointerNode(string name, AetObjPairPointer data) : base(data.ToString() ?? name, data)
        {
            return;
        }
    }

}