using System.Drawing;
using MikuMikuLibrary.Aet;
using MikuMikuLibrary.Aet.Body;
using MikuMikuModel.DataNodes.Aet.Body;

namespace MikuMikuModel.DataNodes.Aet
{
    public class AetObjNode : DataNode<AetObj>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        public override Bitmap Icon => Properties.Resources.Folder;

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
            AetObjBody body = Data.ObjectBody;
            string name = body.BodyType.ToString();
            
            if (body is NopBody nop)
            {
                Add(new NopNode(name, nop));
            }
            else if (body is PicBody pic)
            {
                Add(new PicNode(name, pic));
            }
            else if (body is AifBody aif)
            {
                Add(new AifNode(name, aif));
            }
            else if (body is EffBody eff)
            {
                Add(new EffNode(name, eff));
            }

            if (body is IAnimatable animatable)
            {
                Add(new AnimationDataNode("Key Frames", animatable.AnimationData));
            }
        }

        public AetObjNode(string name, AetObj data) : base(data.Name ?? name, data)
        {
            return;
        }
    }
}