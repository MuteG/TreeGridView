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
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
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
        private static readonly VisualStyleRenderer RENDERER_OPEN = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
        private static readonly VisualStyleRenderer RENDERER_CLOSED = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);

        private Padding _previousPadding;
        private int _imageWidth, _imageHeight;

        public TreeGridCell()
        {
            IsSited = false;
        }

        internal bool IsSited { get; private set; }

        /// <summary>
        /// 获取包含此单元格的节点
        /// </summary>
        public TreeGridNode OwningNode => OwningRow as TreeGridNode;

        public int Level
        {
            get
            {
                TreeGridNode node = OwningNode;
                if (node != null)
                {
                    return node.Level;
                }

                return -1;
            }
        }

        private int GlyphMargin => Level * INDENT_WIDTH + INDENT_MARGIN;

        internal void UnSite()
        {
            if (IsSited)
            {
                IsSited = false;
                Style.Padding = _previousPadding;
            }
        }

        internal void Site()
        {
            if (!IsSited)
            {
                IsSited = true;
                _previousPadding = Style.Padding;

                UpdateStyle();
            }
        }

        internal void UpdateStyle()
        {
            if (IsSited == false)
            {
                return;
            }

            Image image = OwningNode.Image;
            if (image != null)
            {
                _imageWidth = image.Width;
                _imageHeight = image.Height;

            }
            else
            {
                _imageWidth = 0;
                _imageHeight = 0;
            }

            int checkBoxWidth = 0;
            using (Graphics g = OwningNode.Grid.CreateGraphics())
            {
                if (OwningNode.Grid.ShowCheckBox)
                {
                    checkBoxWidth = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.CheckedNormal).Width;
                }
            }

            Style.Padding = new Padding(
                _previousPadding.Left + (Level + 1) * INDENT_WIDTH + _imageWidth + checkBoxWidth + INDENT_MARGIN,
                _previousPadding.Top,
                _previousPadding.Right,
                _previousPadding.Bottom);
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds,
            int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue,
            string errorText, DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            TreeGridNode node = OwningNode;
            if (node == null) return;

            Image image = node.Image;

            if (_imageHeight == 0 && image != null) UpdateStyle();

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
            Rectangle glyphRect = new Rectangle(cellBounds.X + GlyphMargin, cellBounds.Y, INDENT_WIDTH, cellBounds.Height - 1);

            //TODO: Rehash this to take different Imagelayouts into account. This will speed up drawing
            //		for images of the same size (ImageLayout.None)
            if (image != null)
            {
                var pp = _imageHeight > cellBounds.Height
                    ? new Point(glyphRect.X + checkBoxWidth + GLYPH_WIDTH, cellBounds.Y)
                    : new Point(glyphRect.X + checkBoxWidth + GLYPH_WIDTH, (cellBounds.Height / 2 - _imageHeight / 2) + cellBounds.Y);

                // Graphics container to push/pop changes. This enables us to set clipping when painting
                // the cell's image -- keeps it from bleeding outsize of cells.
                GraphicsContainer gc = graphics.BeginContainer();
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
                    linePen.DashStyle = DashStyle.Dot;
                    bool isLastSibling = node.IsLastSibling;
                    bool isFirstSibling = node.IsFirstSibling;
                    // the Root nodes display their lines differently
                    if (isFirstSibling && isLastSibling)
                    {
                        // only node, both first and last. Just draw horizontal line
                        graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                        if (node.ParentNode != null && node.Level > 1)
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
                        if (node.ParentNode == null)
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
                    TreeGridNode previousNode = node.ParentNode;
                    int horizontalStop = glyphRect.X + 4 - INDENT_WIDTH;

                    while (previousNode != null)
                    {
                        if (previousNode.HasChildren && !previousNode.IsLastSibling)
                        {
                            // paint vertical line
                            graphics.DrawLine(linePen, horizontalStop, cellBounds.Top, horizontalStop, cellBounds.Bottom);
                        }
                        previousNode = previousNode.ParentNode;
                        horizontalStop -= INDENT_WIDTH;
                    }
                }
            }

            if (node.HasChildren || node.Grid.VirtualNodes)
            {
                var bound = new Rectangle(glyphRect.X, glyphRect.Y + (glyphRect.Height - 10) / 2, 10, 10);
                if (node.IsExpanded)
                    RENDERER_OPEN.DrawBackground(graphics, bound);
                else
                    RENDERER_CLOSED.DrawBackground(graphics, bound);
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

            TreeGridNode node = OwningNode;
            if (node != null)
            {
                node.Grid.InExpandCollapseMouseCapture = false;
            }
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (OwningNode == null)
            {
                base.OnMouseDown(e);
                return;
            }

            if (e.Location.X > GlyphMargin && e.Location.X < GlyphMargin + GLYPH_WIDTH)
            {
                TreeGridNode node = OwningNode;
                if (node != null)
                {
                    node.Grid.InExpandCollapseMouseCapture = true;
                    if (node.IsExpanded)
                    {
                        node.Collapse();
                    }
                    else
                    {
                        node.Expand();
                    }
                }
            }
            else if (e.Location.X > GlyphMargin + GLYPH_WIDTH &&
                     e.Location.X < GlyphMargin + GLYPH_WIDTH + GLYPH_WIDTH)
            {
                TreeGridNode node = OwningNode;
                // CheckBox
                if (node.Grid.ShowCheckBox)
                {
                    node.IsCheckStateChangedByProgram = true;
                    node.Checked = !node.Checked;
                }
            }
            else
            {
                base.OnMouseDown(e);
            }
        }
    }
}
