using System.Drawing;
using System.Windows.Forms;

namespace TreeGridView
{
    public sealed class TreeGridColumn : DataGridViewTextBoxColumn
    {
        public TreeGridColumn()
        {
            CellTemplate = new TreeGridCell();
        }

        // Need to override Clone for design-time support.
        public override object Clone()
        {
            TreeGridColumn c = (TreeGridColumn)base.Clone();
            if (c != null)
            {
                c.DefaultNodeImage = DefaultNodeImage;
            }

            return c;
        }

        public Image DefaultNodeImage { get; set; }
    }
}
