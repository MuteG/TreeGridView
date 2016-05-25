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
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace AdvancedDataGridView
{
    /// <summary>
    /// 用来显示树形结构的单元格
    /// </summary>
    public class TreeGridCell : DataGridViewTextBoxCell
    {
        /// <summary>
        /// 缩进宽度
        /// </summary>
        private const int INDENT_WIDTH = 20;
        /// <summary>
        /// 缩进间隔
        /// </summary>
        private const int INDENT_MARGIN = 5;
        /// <summary>
        /// 字符宽度
        /// </summary>
        private const int GLYPH_WIDTH = 15;
        private readonly static VisualStyleRenderer RENDERER_OPEN = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
        private readonly static VisualStyleRenderer RENDERER_CLOSED = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);

        private Padding previousPadding;
        private int imageWidth = 0, imageHeight = 0;

        internal bool IsSited { get; private set; }

        public TreeGridCell()
        {
            this.IsSited = false;
        }

        internal void UnSited()
        {
            if (this.IsSited)
            {
                this.IsSited = false;
                this.Style.Padding = this.previousPadding;
            }
        }

        internal void Sited()
        {
            if (!this.IsSited)
            {
                this.IsSited = true;
                this.previousPadding = this.Style.Padding;

                this.UpdateStyle();
            }
        }

        internal void UpdateStyle()
        {
            if (this.IsSited == false)
            {
                return;
            }

            Image image = this.OwningNode.Image;
            if (image != null)
            {
                imageWidth = image.Width;
                imageHeight = image.Height;

            }
            else
            {
                imageWidth = 0;
                imageHeight = 0;
            }

            int checkBoxWidth = 0;
            using (Graphics g = this.OwningNode.Grid.CreateGraphics())
            {
                if (this.OwningNode.Grid.ShowCheckBox)
                {
                    checkBoxWidth = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.CheckedNormal).Width;
                }
            }

            this.Style.Padding = new Padding(this.previousPadding.Left + (this.Level * INDENT_WIDTH) + imageWidth + checkBoxWidth + INDENT_MARGIN,
                this.previousPadding.Top, this.previousPadding.Right, this.previousPadding.Bottom);
        }

        public int Level
        {
            get
            {
                TreeGridNode row = this.OwningNode;
                if (row != null)
                {
                    return row.Level;
                }
                else
                    return -1;
            }
        }

        private int GlyphMargin
        {
            get
            {
                return ((this.Level - 1) * INDENT_WIDTH) + INDENT_MARGIN;
            }
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds,
            int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue,
            string errorText, DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {

            TreeGridNode node = this.OwningNode;
            if (node == null) return;

            Image image = node.Image;

            if (this.imageHeight == 0 && image != null) this.UpdateStyle();

            // paint the cell normally
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            int checkBoxWidth = 0, checkBoxHeight = 0;
            if (node.Grid.ShowCheckBox)
            {
                Size chkSize = CheckBoxRenderer.GetGlyphSize(graphics, CheckBoxState.CheckedNormal);
                checkBoxWidth = chkSize.Width;
                checkBoxHeight = chkSize.Height;
            }

            // TODO: Indent width needs to take image size into account
            Rectangle glyphRect = new Rectangle(cellBounds.X + this.GlyphMargin, cellBounds.Y, INDENT_WIDTH, cellBounds.Height - 1);

            //TODO: Rehash this to take different Imagelayouts into account. This will speed up drawing
            //		for images of the same size (ImageLayout.None)
            if (image != null)
            {
                Point pp;
                if (imageHeight > cellBounds.Height)
                    pp = new Point(glyphRect.X + checkBoxWidth + GLYPH_WIDTH, cellBounds.Y);
                else
                    pp = new Point(glyphRect.X + checkBoxWidth + GLYPH_WIDTH, (cellBounds.Height / 2 - imageHeight / 2) + cellBounds.Y);

                // Graphics container to push/pop changes. This enables us to set clipping when painting
                // the cell's image -- keeps it from bleeding outsize of cells.
                System.Drawing.Drawing2D.GraphicsContainer gc = graphics.BeginContainer();
                {
                    graphics.SetClip(cellBounds);
                    graphics.DrawImageUnscaled(image, pp);
                }
                graphics.EndContainer(gc);
            }

            // Paint tree lines			
            if (node.Grid.ShowLines)
            {
                using (Pen linePen = new Pen(SystemBrushes.ControlDark, 1.0f))
                {
                    linePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    bool isLastSibling = node.IsLastSibling;
                    bool isFirstSibling = node.IsFirstSibling;
                    // the Root nodes display their lines differently
                    if (isFirstSibling && isLastSibling)
                    {
                        // only node, both first and last. Just draw horizontal line
                        graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                        if (node.Parent != null && node.Level > 1)
                        {
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2);
                        }
                    }
                    else if (isLastSibling)
                    {
                        // last sibling doesn't draw the line extended below. Paint horizontal then vertical
                        graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                        graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2);
                    }
                    else if (isFirstSibling)
                    {
                        // first sibling doesn't draw the line extended above. Paint horizontal then vertical
                        graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                        if (node.Parent == null)
                        {
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.X + 4, cellBounds.Bottom);
                        }
                        else
                        {
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top, glyphRect.X + 4, cellBounds.Bottom);
                        }
                    }
                    else
                    {
                        // normal drawing draws extended from top to bottom. Paint horizontal then vertical
                        graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                        graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top, glyphRect.X + 4, cellBounds.Bottom);
                    }
                    // paint lines of previous levels to the root
                    TreeGridNode previousNode = node.Parent;
                    int horizontalStop = (glyphRect.X + 4) - INDENT_WIDTH;

                    while (!previousNode.IsRoot)
                    {
                        if (previousNode.HasChildren && !previousNode.IsLastSibling)
                        {
                            // paint vertical line
                            graphics.DrawLine(linePen, horizontalStop, cellBounds.Top, horizontalStop, cellBounds.Bottom);
                        }
                        previousNode = previousNode.Parent;
                        horizontalStop = horizontalStop - INDENT_WIDTH;
                    }
                }
            }

            if (node.HasChildren || node.Grid.VirtualNodes)
            {
                if (node.IsExpanded)
                    RENDERER_OPEN.DrawBackground(graphics, new Rectangle(glyphRect.X, glyphRect.Y + (glyphRect.Height - 10) / 2, 10, 10));
                else
                    RENDERER_CLOSED.DrawBackground(graphics, new Rectangle(glyphRect.X, glyphRect.Y + (glyphRect.Height - 10) / 2, 10, 10));
            }

            if (node.Grid.ShowCheckBox)
            {
                CheckBoxRenderer.DrawCheckBox(graphics,
                    new Point(glyphRect.Left + GLYPH_WIDTH, glyphRect.Top + (glyphRect.Height - checkBoxHeight) / 2),
                    node.Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);
            }
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseUp(e);

            TreeGridNode node = this.OwningNode;
            if (node != null)
                node.Grid.inExpandCollapseMouseCapture = false;
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (e.Location.X > this.GlyphMargin && e.Location.X < this.GlyphMargin + GLYPH_WIDTH)
            {
                TreeGridNode node = this.OwningNode;
                if (node != null)
                {
                    node.Grid.inExpandCollapseMouseCapture = true;
                    if (node.IsExpanded)
                        node.Collapse();
                    else
                        node.Expand();
                }
            }
            else if (e.Location.X > this.GlyphMargin + GLYPH_WIDTH && e.Location.X < this.Style.Padding.Left - GLYPH_WIDTH)
            {
                // CheckBox
                this.OwningNode.IsCheckStateChangedByProgram = true;
                this.OwningNode.Checked = !this.OwningNode.Checked;
            }
            else
            {
                base.OnMouseDown(e);
            }
        }

        public TreeGridNode OwningNode
        {
            get { return base.OwningRow as TreeGridNode; }
        }
    }
}
