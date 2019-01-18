using System.Collections.Generic;
using System.Windows.Forms;
using MikuMikuLibrary.Scripts;
using MikuMikuModel.GUI.Controls;

namespace MikuMikuModel.DataNodes
{
    public class PvScriptNode : BinaryFileNode<PvScript>
    {
        public static RichTextBox TextBox;

        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.Export | DataNodeActionFlags.Import;

        public override Control Control
        {
            get
            {
                PvScriptControl.Instance.SetScript(Data);
                return PvScriptControl.Instance;
            }
        }

        public uint ScriptFormat
        {
            get => GetProperty<uint>();
            set => SetProperty(value);
        }

        public List<PvCommand> Commands
        {
            get => GetProperty<List<PvCommand>>();
            set => SetProperty(value);
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<PvScript>((path) => Data.Save(path));
        }

        protected override void InitializeViewCore()
        {
            var descriptor = Data.GetFormatDescriptor();

            Add(new ListNode<PvCommand>("PV Commands", Data.Commands, (c) => c.FormatString(descriptor)));
        }

        public PvScriptNode(string name, PvScript data) : base(name, data)
        {
            return;
        }
    }
}