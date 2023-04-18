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

namespace System.Windows.Forms
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
