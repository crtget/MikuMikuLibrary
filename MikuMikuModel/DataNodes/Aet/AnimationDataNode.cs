using System.Drawing;
using MikuMikuLibrary.Aet;
using MikuMikuLibrary.Aet.Body;

namespace MikuMikuModel.DataNodes.Aet
{
    public class AnimationDataNode : DataNode<AnimationData>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        public override Bitmap Icon => Properties.Resources.Folder;

        public ShaderType Shader
        {
            get => GetProperty<ShaderType>();
            set => SetProperty(value);
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
            Add(new KeyFrameDataNode("X Position", Data.PositionX));
            Add(new KeyFrameDataNode("Y Position", Data.PositionY));

            Add(new KeyFrameDataNode("Scale X Origin", Data.ScaleXOrigin));
            Add(new KeyFrameDataNode("Scale Y Origin", Data.ScaleYOrigin));

            Add(new KeyFrameDataNode("Rotation", Data.Rotation));

            Add(new KeyFrameDataNode("Scale X", Data.ScaleX));
            Add(new KeyFrameDataNode("Scale Y", Data.ScaleY));

            Add(new KeyFrameDataNode("Opacity", Data.Opacity));
        }

        public AnimationDataNode(string name, AnimationData data) : base(name, data)
        {
            return;
        }
    }
}