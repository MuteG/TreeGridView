/* ------------------------------------------------------------------
 * 
 *  Copyright (c) Microsoft Corporation.  All rights reserved.
 * 
 *  THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
 *  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 *  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 *  PARTICULAR PURPOSE.
 * 
 * ------------------------------------------------------------------- */
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedDataGridView
{
    public class TreeGridColumn : DataGridViewTextBoxColumn
    {
        internal Image _defaultNodeImage;

        public TreeGridColumn()
        {
            this.CellTemplate = new TreeGridCell();
        }

        // Need to override Clone for design-time support.
        public override object Clone()
        {
            TreeGridColumn c = (TreeGridColumn)base.Clone();
            c._defaultNodeImage = this._defaultNodeImage;
            return c;
        }

        public Image DefaultNodeImage
        {
            get { return _defaultNodeImage; }
            set { _defaultNodeImage = value; }
        }
    }
}
