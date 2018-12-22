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

        protected override void InitializeCore()
        {
            RegisterCustomHandler("Add Start Key Frame", () =>
            {
                var first = Data.KeyFrames.FirstOrDefault();
                Data.KeyFrames.Insert(0, first != null ? new KeyFrame(first.Frame, first.Value) : new KeyFrame(0));

                Refresh();
            });
            RegisterCustomHandler("Add End Key Frame", () => 
            {
                var last = Data.KeyFrames.LastOrDefault();
                Data.KeyFrames.Add(last != null ? new KeyFrame(last.Frame, last.Value) : new KeyFrame(0));

                Refresh();
            });

            void Refresh()
            {
                Clear();
                InitializeViewCore();

                HasPendingChanges = true;
            }
        }

        protected override void InitializeViewCore()
        {
            for (int i = 0; i < Data.KeyFrames.Count; i++)
            {
                Add(new KeyFrameNode($"{nameof(Data.KeyFrames)} #{i}", Data.KeyFrames[i]));
            }
        }

        public KeyFrameDataNode(string name, KeyFrameData data) : base(name, data)
        {
        }
    }


}