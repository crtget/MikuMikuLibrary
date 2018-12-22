using System.Drawing;
using MikuMikuLibrary.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    public class AetSetNode : BinaryFileNode<AetSet>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.Export;

        protected override void InitializeCore()
        {
            /// TODO: look for spr, <see cref="ModelNode.InitializeViewCore()"/>

            RegisterExportHandler<AetSet>((path) => Data.Save(path));
        }

        protected override void InitializeViewCore()
        {
            if (Data.MainSection != null)
                Add(new AetSectionNode(Data.MainSection.Name, Data.MainSection));

            if (Data.TouchSection != null)
                Add(new AetSectionNode(Data.TouchSection.Name, Data.TouchSection));
        }

        public AetSetNode(string name, AetSet data) : base(name, data)
        {
            return;
        }
    }
}