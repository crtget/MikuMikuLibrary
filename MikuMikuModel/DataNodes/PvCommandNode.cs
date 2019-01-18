using System.Drawing;
using MikuMikuLibrary.Scripts;

namespace MikuMikuModel.DataNodes
{
    public class PvCommandNode : DataNode<PvCommand>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Leaf;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        public override Bitmap Icon => Properties.Resources.Node;

        public int Time
        {
            get => GetProperty<int>();
        }

        public int Opcode
        {
            get => GetProperty<int>();
            set => SetProperty(value);
        }

        public int[] Arguments
        {
            get => GetProperty<int[]>();
            set => SetProperty(value);
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        public PvCommandNode(string name, PvCommand data) : base(name, data)
        {
            return;
        }
    }
}