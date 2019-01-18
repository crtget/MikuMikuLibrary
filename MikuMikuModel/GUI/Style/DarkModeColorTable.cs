using System.Drawing;
using System.Windows.Forms;

namespace MikuMikuModel.GUI.Forms.Style
{
    public class DarkModeColorTable : ProfessionalColorTable
    {
        private static DarkModeColorTable instance;

        public static DarkModeColorTable Instance
        {
            get
            {
                if (instance == null)
                    instance = new DarkModeColorTable();

                return instance;
            }
        }

        public Color TextColor { get; } = Color.FromArgb(241, 241, 241);

        public Color SelectedTextColor => TextColor;

        public Color ForeColor => TextColor;

        public Color BackColor { get; } = Color.FromArgb(37, 37, 38);

        public Color BaseBackColor { get; } = Color.FromArgb(30, 30, 30);

        public Color BorderColor { get; } = Color.FromArgb(45, 45, 48);

        public override Color ToolStripDropDownBackground { get; } = Color.FromArgb(27, 27, 28);

        public override Color MenuItemSelected { get; } = Color.FromArgb(51, 51, 52);

        public override Color MenuBorder { get; } = Color.Transparent;

        public override Color MenuItemBorder { get; } = Color.Transparent;

        public override Color CheckBackground => ToolStripDropDownBackground;

        public override Color MenuStripGradientBegin => BorderColor;

        public override Color MenuStripGradientEnd => MenuStripGradientBegin;

        public override Color MenuItemSelectedGradientBegin { get; } = Color.FromArgb(62, 62, 64);

        public override Color MenuItemSelectedGradientEnd => MenuItemSelectedGradientBegin;

        public override Color MenuItemPressedGradientBegin => ToolStripDropDownBackground;

        public override Color MenuItemPressedGradientEnd => MenuItemPressedGradientBegin;

        public override Color ImageMarginGradientBegin => ToolStripDropDownBackground;

        public override Color ImageMarginGradientMiddle => ImageMarginGradientBegin;

        public override Color ImageMarginGradientEnd => ImageMarginGradientMiddle;

        public override Color SeparatorDark { get; } = Color.FromArgb(51, 51, 55);

        public override Color SeparatorLight { get; } = Color.FromArgb(214, 241, 241);
    }
}