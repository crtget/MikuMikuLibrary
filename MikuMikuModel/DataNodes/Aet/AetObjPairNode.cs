using System.Drawing;
using MikuMikuLibrary.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    public class AetObjPairNode : DataNode<AetObjPair>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        public override Bitmap Icon => Properties.Resources.Node;

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
            foreach (var aetObj in Data.AetObjects)
            {
                Add(new AetObjNode(aetObj.Name ?? "NULL", aetObj));
            }
        }

        public AetObjPairNode(string name, AetObjPair data) : base(data.PairName ?? name, data)
        {
            return;
        }
    }
}