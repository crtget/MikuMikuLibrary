using System.Windows.Forms;

namespace MikuMikuModel.GUI.Forms.Style
{
    public class DarkModeToolStripRenderer : ToolStripProfessionalRenderer
    {
        DarkModeColorTable DarkModeColorTable { get; }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Selected ? DarkModeColorTable.SelectedTextColor : DarkModeColorTable.TextColor;
            base.OnRenderItemText(e);
        }

        public DarkModeToolStripRenderer(DarkModeColorTable colorTable) : base(colorTable)
        {
            DarkModeColorTable = colorTable;
        }
    }
}