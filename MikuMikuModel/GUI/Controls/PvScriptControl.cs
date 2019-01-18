using MikuMikuLibrary.Scripts;
using MikuMikuModel.GUI.Forms.Style;
using System.Windows.Forms;

namespace MikuMikuModel.GUI.Controls
{
    public partial class PvScriptControl : UserControl
    {
        private static PvScriptControl sInstance;

        public static PvScriptControl Instance => sInstance ?? (sInstance = new PvScriptControl());

        public static void DisposeInstance()
        {
            sInstance?.Dispose();
        }

        public void SetScript(PvScript script)
        {
            mTextBox.Text = script.FormatText().ToString();
        }

        private PvScriptControl()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, System.EventArgs e)
        {
            DarkModeColorTable colorTable = DarkModeColorTable.Instance;

            mTextBox.BackColor = colorTable.BaseBackColor;
            mTextBox.ForeColor = colorTable.ForeColor;
        }
    }
}
