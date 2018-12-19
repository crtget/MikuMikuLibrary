using System.Drawing;
using MikuMikuLibrary.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    public class AetObjPairNode : DataNode<AetObjPair>
    {
        public override DataNodeFlags Flags
        {
            get => DataNodeFlags.Branch;
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
            foreach (var animation in Data.AetObjects)
            {
                Add(DataNodeFactory.Create<AetObj>(animation.Name ?? "NULL", animation));
            }
        }

        public AetObjPairNode(string name, AetObjPair data) : base(data.PairName ?? name, data)
        {
            return;
        }
    }
}