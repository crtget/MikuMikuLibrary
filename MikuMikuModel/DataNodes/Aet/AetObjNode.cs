using System.Drawing;
using MikuMikuLibrary.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    public class AetObjNode : DataNode<AetObj>
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

        public AetObjNode(string name, AetObj data) : base(data.Name ?? name, data)
        {
            return;
        }
    }
}