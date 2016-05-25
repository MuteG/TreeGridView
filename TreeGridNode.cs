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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace AdvancedDataGridView
{
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    public class TreeGridNode : DataGridViewRow
    {
        internal bool isSited;
        private bool isChecked = true;
        internal Image image;
        internal int imageIndex;

        private TreeGridNodeCollection childrenNodes = null;

        public TreeGridNode()
        {
            this.IsExpanded = true;
            this.IsCheckStateChangedByProgram = false;
            isSited = false;
            imageIndex = -1;
        }

        public override object Clone()
        {
            TreeGridNode r = (TreeGridNode)base.Clone();
            r.Grid = this.Grid;
            r.Parent = this.Parent;
            r.IsExpanded = this.IsExpanded;

            r.imageIndex = this.imageIndex;
            if (r.imageIndex == -1)
            {
                r.Image = this.Image;
            }

            return r;
        }

        internal void Unsite()
        {
            foreach (TreeGridNode childNode in this.Nodes)
            {
                childNode.Unsite();
            }
            foreach (DataGridViewCell cell in this.Cells)
            {
                TreeGridCell treeCell = cell as TreeGridCell;
                if (treeCell != null)
                {
                    treeCell.UnSited();
                }
            }
            this.isSited = false;
        }

        internal void Site()
        {
            foreach (TreeGridNode childNode in this.Nodes)
            {
                childNode.Site();
            }
            // This row is being added to the grid.
            this.isSited = true;
            Debug.Assert(this.Grid != null);

            TreeGridCell cell;
            foreach (DataGridViewCell DGVcell in this.Cells)
            {
                cell = DGVcell as TreeGridCell;
                if (cell != null && !cell.IsSited)
                {
                    cell.Sited();// Level = this.Level;
                }
            }

        }

        /// <summary>
        /// Represents the index of this row in the Grid
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RowIndex
        {
            get
            {
                return base.Index;
            }
        }

        /// <summary>
        /// Represents the index of this row based upon its position in the collection.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int Index
        {
            get
            {
                if (this.Parent == null)
                {
                    return -1;
                }
                else
                {
                    return this.Parent.Nodes.IndexOf(this);
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageList ImageList
        {
            get
            {
                if (this.Grid != null)
                    return this.Grid.ImageList;
                else
                    return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Checked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    this.Grid.InvalidateCell(this.Cells[0]);
                    this.Grid.OnNodeChecked(this, this.IsCheckStateChangedByProgram);
                    this.IsCheckStateChangedByProgram = false;
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsCheckStateChangedByProgram { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsExpanded { get; private set; }

        [Category("Appearance")]
        [Description("..."), DefaultValue(-1)]
        [TypeConverter(typeof(ImageIndexConverter))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor", typeof(UITypeEditor))]
        public int ImageIndex
        {
            get { return imageIndex; }
            set
            {
                imageIndex = value;
                if (imageIndex != -1)
                {
                    this.image = null;
                }
                if (this.isSited)
                {
                    // when the image changes the cell's style must be updated
                    (this.Cells[0] as TreeGridCell).UpdateStyle();
                    if (this.Displayed)
                        this.Grid.InvalidateRow(this.RowIndex);
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image Image
        {
            get
            {
                if (image == null && imageIndex != -1)
                {
                    if (this.ImageList != null && this.imageIndex < this.ImageList.Images.Count)
                    {
                        // get image from image index
                        return this.ImageList.Images[this.imageIndex];
                    }
                    else
                        return null;
                }
                else
                {
                    // image from image property
                    return this.image;
                };
            }
            set
            {
                image = value;
                if (image != null)
                {
                    // when a image is provided we do not store the imageIndex.
                    this.imageIndex = -1;
                }
                if (this.isSited)
                {
                    // when the image changes the cell's style must be updated
                    (this.Cells[0] as TreeGridCell).UpdateStyle();
                    if (this.Displayed)
                        this.Grid.InvalidateRow(this.RowIndex);
                }
            }
        }

        [Category("Data")]
        [Description("The collection of root nodes in the treelist.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        public TreeGridNodeCollection Nodes
        {
            get
            {
                if (childrenNodes == null)
                {
                    childrenNodes = new TreeGridNodeCollection(this);
                }
                return childrenNodes;
            }
        }

        internal bool IsRoot
        {
            get
            {
                return this.Level == 0;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Level
        {
            get
            {
                if (this.Parent == null)
                {
                    return 0;
                }
                else
                {
                    return this.Parent.Level + 1;
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TreeGridView Grid { get; internal set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TreeGridNode Parent { get; internal set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasChildren
        {
            get
            {
                return this.Nodes.Count > 0;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSited { get; private set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFirstSibling
        {
            get
            {
                return (this.Index == 0);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsLastSibling
        {
            get
            {
                return
                    this.Index > -1 &&
                    this.Index == this.Parent.Nodes.Count - 1;
            }
        }

        public void Collapse()
        {
            this.Collapse(true, false);
        }

        public void CollapseAll()
        {
            this.Collapse(true, true);
        }

        private void Collapse(bool collapseSelf, bool collapseChild)
        {
            CollapsingEventArgs args = new CollapsingEventArgs(this);
            if (collapseSelf)
            {
                this.IsExpanded = false;
                this.Grid.OnNodeCollapsing(args);
            }
            if (!args.Cancel)
            {
                foreach (TreeGridNode node in this.Nodes)
                {
                    node.Collapse(collapseChild, collapseChild);
                    node.Visible = false;
                }
                this.Grid.InvalidateCell(this.Cells[0]);
                this.Grid.OnNodeCollapsed(this);
            }
        }

        public void Expand()
        {
            this.Expand(true, false);
        }

        public void ExpandAll()
        {
            this.Expand(true, true);
        }

        private void Expand(bool expandSelf, bool expandChild)
        {
            ExpandingEventArgs args = new ExpandingEventArgs(this);
            if (expandSelf)
            {
                this.IsExpanded = true;
                this.Grid.OnNodeExpanding(args);
            }
            if (!args.Cancel)
            {
                foreach (TreeGridNode node in this.Nodes)
                {
                    node.Expand(expandChild, expandChild);
                    node.Visible = true;
                }
                this.Grid.InvalidateCell(this.Cells[0]);
                this.Grid.OnNodeExpanded(this);
            }
        }

        public override string ToString()
        {
            return string.Format("TreeGridNode {{ Index={0}, RowIndex={1} }}", this.Index, this.RowIndex);
        }
    }
}