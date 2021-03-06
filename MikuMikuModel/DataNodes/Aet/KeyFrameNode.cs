﻿using System.Drawing;
using MikuMikuLibrary.Aet;
using MikuMikuLibrary.Aet.Body;

namespace MikuMikuModel.DataNodes.Aet
{
    public class KeyFrameNode : DataNode<KeyFrame>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Leaf;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.Move | DataNodeActionFlags.Remove;

        public override Bitmap Icon => Properties.Resources.Node;

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        public KeyFrameNode(string name, KeyFrame data) : base(name, data)
        {
            return;
        }
    }
}