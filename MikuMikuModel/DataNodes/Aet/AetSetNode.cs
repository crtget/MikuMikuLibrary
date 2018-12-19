using MikuMikuLibrary.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    public class AetSetNode : BinaryFileNode<AetSet>
    {
        public override DataNodeFlags Flags
        {
            get => DataNodeFlags.Branch;
        }

        public override DataNodeActionFlags ActionFlags
        {
            get => DataNodeActionFlags.Export;
        }

        protected override void InitializeCore()
        {
            /// TODO: look for spr, <see cref="ModelNode.InitializeViewCore()"/>

            RegisterExportHandler<AetSet>((path) => Data.Save(path));
        }

        protected override void InitializeViewCore()
        {
            Add(new ListNode<SpriteEntry>("Sprite Entries", Data.SpriteEntries));
            Add(new ListNode<SpriteMetadataEntry>("Sprite Metadata", Data.SpriteMetadataEntries));
            Add(new ListNode<AetObjPairPointer>("Aet Object Pair Pointer Table", Data.AetObjPairPointerTable));

            Add(new ListNode<AetObjPair>("Aet Object Pairs", Data.AetObjPairs));
        }

        public AetSetNode(string name, AetSet data) : base(name, data)
        {
            return;
        }
    }
}