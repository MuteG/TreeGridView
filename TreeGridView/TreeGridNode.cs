using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace TreeGridView
{
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    public class TreeGridNode : DataGridViewRow
    {
        private bool _isSited;
        private bool _isChecked = true;
        private Image _image;
        private int _imageIndex;

        public TreeGridNode()
        {
            IsExpanded = true;
            IsCheckStateChangedByProgram = false;
            _isSited = false;
            _imageIndex = -1;
            Nodes = new TreeGridNodeCollection(this);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TreeGridView Grid
        {
            get { return DataGridView as TreeGridView; }
        }

        /// <summary>
        /// 包含此节点的行集合
        /// </summary>
        public TreeGridViewRowCollection OwningRowCollection
        {
            get { return Grid != null ? (TreeGridViewRowCollection)Grid.Rows : null; }
        }

        /// <summary>
        /// 包含此节点的节点集合
        /// </summary>
        public TreeGridNodeCollection OwningNodeCollection { get; internal set; }

        /// <summary>
        /// 此节点的父节点
        /// <para>如果此节点为根节点，则返回 null。</para>
        /// </summary>
        public TreeGridNode ParentNode
        {
            get { return OwningNodeCollection != null ? OwningNodeCollection.OwningNode : null; }
        }

        /// <summary>
        /// 获取该节点在行集合中位置索引
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RowIndex
        {
            get { return Index; }
        }

        /// <summary>
        /// 获取该节点在节点集合中的位置索引
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int NodeIndex
        {
            get { return OwningNodeCollection != null ? OwningNodeCollection.IndexOf(this) : -1; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageList ImageList
        {
            get
            {
                if (Grid != null)
                    return Grid.ImageList;
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
                return _isChecked;
            }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    Grid.InvalidateCell(Cells[0]);
                    Grid.OnNodeChecked(this, IsCheckStateChangedByProgram);
                    IsCheckStateChangedByProgram = false;
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsCheckStateChangedByProgram { get; set; }

        /// <summary>
        /// 该节点是否已经展开
        /// </summary>
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
            get { return _imageIndex; }
            set
            {
                _imageIndex = value;
                if (_imageIndex != -1)
                {
                    _image = null;
                }
                if (_isSited)
                {
                    // when the image changes the cell's style must be updated
                    ((TreeGridCell)Cells[0]).UpdateStyle();
                    if (Displayed)
                        Grid.InvalidateRow(RowIndex);
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
                if (_image == null && _imageIndex != -1)
                {
                    if (ImageList != null && _imageIndex < ImageList.Images.Count)
                    {
                        // get image from image index
                        return ImageList.Images[_imageIndex];
                    }

                    return null;
                }

                // image from image property
                return _image;
            }
            set
            {
                _image = value;
                if (_image != null)
                {
                    // when a image is provided we do not store the imageIndex.
                    _imageIndex = -1;
                }
                if (_isSited)
                {
                    // when the image changes the cell's style must be updated
                    ((TreeGridCell)Cells[0]).UpdateStyle();
                    if (Displayed)
                        Grid.InvalidateRow(RowIndex);
                }
            }
        }

        [Category("Data")]
        [Description("The collection of root nodes in the treelist.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        public TreeGridNodeCollection Nodes { get; private set; }

        internal bool IsRoot
        {
            get { return Level == 0; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Level
        {
            get
            {
                if (ParentNode == null)
                {
                    return 0;
                }

                return ParentNode.Level + 1;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasChildren
        {
            get { return Nodes.Count > 0; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSited { get; private set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFirstSibling
        {
            get { return NodeIndex == 0; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsLastSibling
        {
            get
            {
                return NodeIndex > -1 &&
                       NodeIndex == OwningNodeCollection.Count - 1;
            }
        }

        #region Public Members

        /// <summary>
        /// 折叠当前节点
        /// </summary>
        public void Collapse()
        {
            Collapse(true, false);
        }

        /// <summary>
        /// 折叠当前节点以及所有子节点
        /// </summary>
        public void CollapseAll()
        {
            Collapse(true, true);
        }

        /// <summary>
        /// 展开当前节点
        /// </summary>
        public void Expand()
        {
            Expand(true, false);
        }

        /// <summary>
        /// 展开当前节点以及所有子节点
        /// </summary>
        public void ExpandAll()
        {
            Expand(true, true);
        }

        internal void UnSite()
        {
            foreach (TreeGridNode childNode in Nodes)
            {
                childNode.UnSite();
            }
            foreach (DataGridViewCell cell in Cells)
            {
                TreeGridCell treeCell = cell as TreeGridCell;
                if (treeCell != null)
                {
                    treeCell.UnSite();
                }
            }
            _isSited = false;
        }

        internal void Site()
        {
            foreach (TreeGridNode childNode in Nodes)
            {
                childNode.Site();
            }

            // This row is being added to the grid.
            _isSited = true;
            Debug.Assert(Grid != null);

            foreach (DataGridViewCell dgVcell in Cells)
            {
                var cell = dgVcell as TreeGridCell;
                if (cell != null && !cell.IsSited)
                {
                    cell.Site();// Level = this.Level;
                }
            }
        }

        public override object Clone()
        {
            TreeGridNode r = (TreeGridNode)base.Clone();
            if (r != null)
            {
                r.IsExpanded = IsExpanded;
                r._imageIndex = _imageIndex;
                if (r._imageIndex == -1)
                {
                    r.Image = Image;
                }
            }

            return r;
        }

        //protected override DataGridViewCellCollection CreateCellsInstance()
        //{
        //    return base.CreateCellsInstance();
        //}

        public override string ToString()
        {
            return string.Format("TreeGridNode {{ NodeIndex={0}, RowIndex={1}, Level={2} }}", NodeIndex, RowIndex,
                Level);
        }

        #endregion

        #region Private Member

        private void Collapse(bool collapseSelf, bool collapseChild)
        {
            CollapsingEventArgs args = new CollapsingEventArgs(this);
            if (collapseSelf)
            {
                IsExpanded = false;
                Grid.OnNodeCollapsing(args);
            }
            if (!args.Cancel)
            {
                foreach (TreeGridNode node in Nodes)
                {
                    node.Collapse(collapseChild, collapseChild);
                    node.Visible = false;
                }
                Grid.InvalidateCell(Cells[0]);
                Grid.OnNodeCollapsed(this);
            }
        }

        private void Expand(bool expandSelf, bool expandChild)
        {
            ExpandingEventArgs args = new ExpandingEventArgs(this);
            if (expandSelf)
            {
                IsExpanded = true;
                Grid.OnNodeExpanding(args);
            }
            if (!args.Cancel)
            {
                foreach (TreeGridNode node in Nodes)
                {
                    node.Expand(expandChild, expandChild);
                    node.Visible = true;
                }
                Grid.InvalidateCell(Cells[0]);
                Grid.OnNodeExpanded(this);
            }
        }

        #endregion
    }
}