using System.Drawing;
using System.Linq;
using MikuMikuLibrary.Aet;
using MikuMikuLibrary.Aet.Body;

namespace MikuMikuModel.DataNodes.Aet
{
    public class KeyFrameDataNode : DataNode<KeyFrameData>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.Add;

        public override Bitmap Icon => Properties.Resources.Folder;

        protected bool reAddingNodes = false;

        protected override void InitializeCore()
        {
            NodeRemoved += (e, o) =>
            {
                if (reAddingNodes)
                    return;

                base.Data.KeyFrames.Remove((KeyFrame)o.ChildNode.Data);

                ReAddNodes();
            };

            RegisterCustomHandler("Add Start Key Frame", () =>
            {
                var first = Data.KeyFrames.FirstOrDefault();
                var newNode = first != null ? new KeyFrame(first.Frame, first.Value) : new KeyFrame(0);

                Data.KeyFrames.Insert(0, newNode);

                ReAddNodes();
            });
            RegisterCustomHandler("Add End Key Frame", () =>
            {
                var last = Data.KeyFrames.LastOrDefault();
                var newNode = last != null ? new KeyFrame(last.Frame, last.Value) : new KeyFrame(0);

                Data.KeyFrames.Add(newNode);

                ReAddNodes();
            });
        }

        protected void ReAddNodes()
        {
            reAddingNodes = true;
            {
                Clear();
                AddKeyFrameNodes();
            }
            reAddingNodes = false;

            HasPendingChanges = true;
        }

        protected void AddKeyFrameNodes()
        {
            for (int i = 0; i < Data.KeyFrames.Count; i++)
            {
                Add(new KeyFrameNode($"{nameof(KeyFrame)} #{i}", Data.KeyFrames[i]));
            }
        }

        protected override void InitializeViewCore()
        {
            AddKeyFrameNodes();
        }

        public KeyFrameDataNode(string name, KeyFrameData data) : base(name, data)
        {
        }
    }
}