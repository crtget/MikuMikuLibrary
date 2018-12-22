using System.Drawing;
using MikuMikuLibrary.Aet;

namespace MikuMikuModel.DataNodes.Aet
{
    public class AetSectionNode : DataNode<AetSection>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.Export;

        public override Bitmap Icon => Properties.Resources.Folder;

        protected override void InitializeCore()
        {
            return;
        }

        protected override void InitializeViewCore()
        {
            Add(new ListNode<SpriteEntry>("Sprite Entries", Data.SpriteEntries));
            Add(new ListNode<SpriteMetadataEntry>("Sprite Metadata", Data.SpriteMetadataEntries));
            Add(new ListNode<AetObjPairPointer>("Aet Object Pair Pointer Table", Data.AetObjPairPointerTable));

            Add(new ListNode<AetObjPair>("Aet Object Pairs", Data.AetObjPairs));
        }

        public AetSectionNode(string name, AetSection data) : base(name, data)
        {
            return;
        }
    }
}