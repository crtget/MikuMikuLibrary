using System.Drawing;
using MikuMikuLibrary.Aet;
using MikuMikuLibrary.Aet.Body;

namespace MikuMikuModel.DataNodes.Aet.Body
{
    public class AnimationDataNode : DataNode<AnimationData>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        public override Bitmap Icon => Properties.Resources.Folder;

        public ShaderType Shader
        {
            get { return GetProperty<ShaderType>(); }
            set { SetProperty(value); }
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
            Add(new ListNode<KeyFrame>("X Position", Data.PositionX.KeyFrames));
            Add(new ListNode<KeyFrame>("Y Position", Data.PositionY.KeyFrames));

            Add(new ListNode<KeyFrame>("Scale X Origin", Data.ScaleXOrigin.KeyFrames));
            Add(new ListNode<KeyFrame>("Scale Y Origin", Data.ScaleYOrigin.KeyFrames));

            Add(new ListNode<KeyFrame>("Rotation", Data.Rotation.KeyFrames));

            Add(new ListNode<KeyFrame>("Scale X", Data.ScaleX.KeyFrames));
            Add(new ListNode<KeyFrame>("Scale Y", Data.ScaleY.KeyFrames));

            Add(new ListNode<KeyFrame>("Opacity", Data.Opacity.KeyFrames));
        }

        public AnimationDataNode(string name, AnimationData data) : base(name, data)
        {
            return;
        }
    }
}