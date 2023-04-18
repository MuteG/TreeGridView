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
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    /// <summary>
    /// 树形数据表格
    /// </summary>
    [DesignerCategory("code"),
    Designer(typeof(ControlDesigner)),
    ComplexBindingProperties,
    Docking(DockingBehavior.Ask)]
    public class TreeGridView : DataGridView
    {
        private TreeGridColumn _expandableColumn;
        private ImageList _imageList;
        internal bool InExpandCollapseMouseCapture;
        private Control _hideScrollBarControl;
        private bool _showLines = true;
        private bool _showCheckBox = true;
        private bool _virtualNodes;

        #region Constructor
        public TreeGridView()
        {
            // Control when edit occurs because edit mode shouldn't start when expanding/collapsing
            EditMode = DataGridViewEditMode.EditProgrammatically;
            RowTemplate = new TreeGridNode();
            // This sample does not support adding or deleting rows by the user.
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            MultiSelect = false;
            _virtualNodes = true;
            Nodes = new TreeGridNodeCollection(null)
            {
                Grid = this
            };
            Columns.Add(new TreeGridColumn
            {
                Name = "colDefault",
                HeaderText = string.Empty
            });
        }

        #endregion

        #region Keyboard F2 to begin edit support
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Cause edit mode to begin since edit mode is disabled to support 
            // expanding/collapsing 
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                if (e.KeyCode == Keys.F2 && CurrentCellAddress.X > -1 && CurrentCellAddress.Y > -1)
                {
                    if (!CurrentCell.Displayed)
                    {
                        FirstDisplayedScrollingRowIndex = CurrentCellAddress.Y;
                    }

                    // TODO:calculate if the cell is partially offscreen and if so scroll into view
                    SelectionMode = DataGridViewSelectionMode.CellSelect;
                    BeginEdit(true);
                }
                else if (e.KeyCode == Keys.Enter && !IsCurrentCellInEditMode)
                {
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    CurrentCell.OwningRow.Selected = true;
                }
            }
        }
        #endregion

        protected override DataGridViewRowCollection CreateRowsInstance()
        {
            return new TreeGridViewRowCollection(this);
        }

        #region Public methods
        [Description("Returns the TreeGridNode for the given DataGridViewRow")]
        public TreeGridNode GetNodeForRow(DataGridViewRow row)
        {
            return row as TreeGridNode;
        }

        [Description("Returns the TreeGridNode for the given DataGridViewRow")]
        public TreeGridNode GetNodeForRow(int index)
        {
            return GetNodeForRow(Rows[index]);
        }
        
        #endregion

        #region Public properties
        [Category("Data"),
        Description("The collection of root nodes in the treelist."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        public TreeGridNodeCollection Nodes { get; }

        public new TreeGridNode CurrentRow
        {
            get
            {
                return base.CurrentRow as TreeGridNode;
            }
        }

        [DefaultValue(false),
        Description("Causes nodes to always show as expandable. Use the NodeExpanding event to add nodes.")]
        public bool VirtualNodes
        {
            get { return _virtualNodes; }
            set { _virtualNodes = value; }
        }

        public TreeGridNode CurrentNode
        {
            get
            {
                return CurrentRow;
            }
        }

        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool ShowLines
        {
            get { return _showLines; }
            set
            {
                if (value != _showLines)
                {
                    _showLines = value;
                    Invalidate();
                }
            }
        }

        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool ShowCheckBox
        {
            get { return _showCheckBox; }
            set
            {
                if (value != _showCheckBox)
                {
                    _showCheckBox = value;
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TreeGridNode SelectedNode
        {
            get
            {
                if (SelectedRows.Count > 0)
                {
                    return SelectedRows[0] as TreeGridNode;
                }

                return null;
            }
        }

        public ImageList ImageList
        {
            get { return _imageList; }
            set
            {
                _imageList = value;
                //TODO: should we invalidate cell styles when setting the image list?

            }
        }

        public new int RowCount
        {
            get { return Nodes.Count; }
            set
            {
                for (int i = 0; i < value; i++)
                    Nodes.Add(new TreeGridNode());

            }
        }

        #endregion

        #region Site nodes and collapse/expand support
        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
        {
            base.OnRowsAdded(e);
            // Notify the row when it is added to the base grid 
            int count = e.RowCount - 1;
            TreeGridNode row;
            while (count >= 0)
            {
                row = Rows[e.RowIndex + count] as TreeGridNode;
                if (row != null)
                {
                    row.Site();
                }
                count--;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            // used to keep extra mouse moves from selecting more rows when collapsing
            base.OnMouseUp(e);
            InExpandCollapseMouseCapture = false;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // while we are expanding and collapsing a node mouse moves are
            // supressed to keep selections from being messed up.
            if (!InExpandCollapseMouseCapture)
                base.OnMouseMove(e);
        }
        #endregion

        #region Collapse/Expand events
        public event ExpandingEventHandler NodeExpanding;
        public event ExpandedEventHandler NodeExpanded;
        public event CollapsingEventHandler NodeCollapsing;
        public event CollapsedEventHandler NodeCollapsed;
        public event CheckedEventHandler NodeChecked;

        protected internal virtual void OnNodeExpanding(ExpandingEventArgs e)
        {
            if (NodeExpanding != null)
            {
                NodeExpanding(this, e);
            }
        }
        protected internal virtual void OnNodeExpanded(TreeGridNode node)
        {
            ExpandedEventArgs e = new ExpandedEventArgs(node);
            if (NodeExpanded != null)
            {
                NodeExpanded(this, e);
            }
        }
        protected internal virtual void OnNodeCollapsing(CollapsingEventArgs e)
        {
            if (NodeCollapsing != null)
            {
                NodeCollapsing(this, e);
            }

        }
        protected internal virtual void OnNodeCollapsed(TreeGridNode node)
        {
            CollapsedEventArgs e = new CollapsedEventArgs(node);
            if (NodeCollapsed != null)
            {
                NodeCollapsed(this, e);
            }
        }
        protected internal virtual void OnNodeChecked(TreeGridNode node, bool isChangedByProgram)
        {
            CheckedEventArgs e = new CheckedEventArgs(node, isChangedByProgram);
            if (NodeExpanded != null)
            {
                NodeChecked?.Invoke(this, e);
            }
        }
        #endregion

        #region Helper methods
        protected override void Dispose(bool disposing)
        {
            base.Dispose(Disposing);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // this control is used to temporarly hide the vertical scroll bar
            _hideScrollBarControl = new Control();
            _hideScrollBarControl.Visible = false;
            _hideScrollBarControl.Enabled = false;
            _hideScrollBarControl.TabStop = false;
            // control is disposed automatically when the grid is disposed
            Controls.Add(_hideScrollBarControl);
        }

        protected override void OnRowEnter(DataGridViewCellEventArgs e)
        {
            // ensure full row select
            base.OnRowEnter(e);
            if (SelectionMode == DataGridViewSelectionMode.CellSelect ||
                (SelectionMode == DataGridViewSelectionMode.FullRowSelect &&
                Rows[e.RowIndex].Selected == false))
            {
                SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                Rows[e.RowIndex].Selected = true;
            }
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            if (typeof(TreeGridColumn).IsAssignableFrom(e.Column.GetType()))
            {
                if (_expandableColumn == null)
                {
                    // identify the expanding column.			
                    _expandableColumn = (TreeGridColumn)e.Column;
                }
                // this.Columns.Remove(e.Column);
                //throw new InvalidOperationException("Only one TreeGridColumn per TreeGridView is supported.");
            }

            // Expandable Grid doesn't support sorting. This is just a limitation of the sample.
            e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;

            base.OnColumnAdded(e);
        }
        #endregion
    }
}
