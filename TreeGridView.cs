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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace AdvancedDataGridView
{
    /// <summary>
    /// Summary description for TreeGridView.
    /// </summary>
    [System.ComponentModel.DesignerCategory("code"),
    Designer(typeof(System.Windows.Forms.Design.ControlDesigner)),
    ComplexBindingProperties(),
    Docking(DockingBehavior.Ask)]
    public class TreeGridView : DataGridView
    {
        private TreeGridNode root;
        private TreeGridColumn expandableColumn;
        internal ImageList imageList;
        private bool inExpandCollapse = false;
        internal bool inExpandCollapseMouseCapture = false;
        private Control hideScrollBarControl;
        private bool showLines = true;
        private bool showCheckBox = true;
        private bool virtualNodes = false;

        #region Constructor
        public TreeGridView()
        {
            // Control when edit occurs because edit mode shouldn't start when expanding/collapsing
            this.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.RowTemplate = new TreeGridNode() as DataGridViewRow;
            // This sample does not support adding or deleting rows by the user.
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.root = new TreeGridNode();
            this.root.Grid = this;

            // Ensures that all rows are added unshared by listening to the CollectionChanged event.
            base.Rows.CollectionChanged += delegate(object sender, System.ComponentModel.CollectionChangeEventArgs e) { };

            this.MultiSelect = false;
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
                if (e.KeyCode == Keys.F2 && this.CurrentCellAddress.X > -1 && this.CurrentCellAddress.Y > -1)
                {
                    if (!this.CurrentCell.Displayed)
                    {
                        this.FirstDisplayedScrollingRowIndex = this.CurrentCellAddress.Y;
                    }
                    else
                    {
                        // TODO:calculate if the cell is partially offscreen and if so scroll into view
                    }
                    this.SelectionMode = DataGridViewSelectionMode.CellSelect;
                    this.BeginEdit(true);
                }
                else if (e.KeyCode == Keys.Enter && !this.IsCurrentCellInEditMode)
                {
                    this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    this.CurrentCell.OwningRow.Selected = true;
                }
            }
        }
        #endregion

        #region Shadow and hide DGV properties

        // This sample does not support databinding
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        EditorBrowsable(EditorBrowsableState.Never)]
        public new object DataSource
        {
            get { return null; }
            set { throw new NotSupportedException("The TreeGridView does not support databinding"); }
        }

        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        EditorBrowsable(EditorBrowsableState.Never)]
        public new object DataMember
        {
            get { return null; }
            set { throw new NotSupportedException("The TreeGridView does not support databinding"); }
        }

        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        EditorBrowsable(EditorBrowsableState.Never)]
        public new DataGridViewRowCollection Rows
        {
            get { return base.Rows; }
        }

        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        EditorBrowsable(EditorBrowsableState.Never)]
        public new bool VirtualMode
        {
            get { return false; }
            set { throw new NotSupportedException("The TreeGridView does not support virtual mode"); }
        }

        // none of the rows/nodes created use the row template, so it is hidden.
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        EditorBrowsable(EditorBrowsableState.Never)]
        public new DataGridViewRow RowTemplate
        {
            get { return base.RowTemplate; }
            set { base.RowTemplate = value; }
        }

        #endregion

        #region Public methods
        [Description("Returns the TreeGridNode for the given DataGridViewRow")]
        public TreeGridNode GetNodeForRow(DataGridViewRow row)
        {
            return row as TreeGridNode;
        }

        [Description("Returns the TreeGridNode for the given DataGridViewRow")]
        public TreeGridNode GetNodeForRow(int index)
        {
            return GetNodeForRow(base.Rows[index]);
        }
        #endregion

        #region Public properties
        [Category("Data"),
        Description("The collection of root nodes in the treelist."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        public TreeGridNodeCollection Nodes
        {
            get
            {
                return this.root.Nodes;
            }
        }

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
            get { return virtualNodes; }
            set { virtualNodes = value; }
        }

        public TreeGridNode CurrentNode
        {
            get
            {
                return this.CurrentRow;
            }
        }

        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool ShowLines
        {
            get { return this.showLines; }
            set
            {
                if (value != this.showLines)
                {
                    this.showLines = value;
                    this.Invalidate();
                }
            }
        }

        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool ShowCheckBox
        {
            get { return this.showCheckBox; }
            set
            {
                if (value != this.showCheckBox)
                {
                    this.showCheckBox = value;
                    this.Invalidate();
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TreeGridNode SelectedNode
        {
            get
            {
                if (this.SelectedRows.Count > 0)
                {
                    return this.SelectedRows[0] as TreeGridNode;
                }
                else
                {
                    return null;
                }
            }
        }

        public ImageList ImageList
        {
            get { return this.imageList; }
            set
            {
                this.imageList = value;
                //TODO: should we invalidate cell styles when setting the image list?

            }
        }

        public new int RowCount
        {
            get { return this.Nodes.Count; }
            set
            {
                for (int i = 0; i < value; i++)
                    this.Nodes.Add(new TreeGridNode());

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
                row = base.Rows[e.RowIndex + count] as TreeGridNode;
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
            this.inExpandCollapseMouseCapture = false;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // while we are expanding and collapsing a node mouse moves are
            // supressed to keep selections from being messed up.
            if (!this.inExpandCollapseMouseCapture)
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
            if (this.NodeExpanding != null)
            {
                NodeExpanding(this, e);
            }
        }
        protected internal virtual void OnNodeExpanded(TreeGridNode node)
        {
            ExpandedEventArgs e = new ExpandedEventArgs(node);
            if (this.NodeExpanded != null)
            {
                NodeExpanded(this, e);
            }
        }
        protected internal virtual void OnNodeCollapsing(CollapsingEventArgs e)
        {
            if (this.NodeCollapsing != null)
            {
                NodeCollapsing(this, e);
            }

        }
        protected internal virtual void OnNodeCollapsed(TreeGridNode node)
        {
            CollapsedEventArgs e = new CollapsedEventArgs(node);
            if (this.NodeCollapsed != null)
            {
                NodeCollapsed(this, e);
            }
        }
        protected internal virtual void OnNodeChecked(TreeGridNode node, bool isChangedByProgram)
        {
            CheckedEventArgs e = new CheckedEventArgs(node, isChangedByProgram);
            if (this.NodeExpanded != null)
            {
                NodeChecked(this, e);
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
            hideScrollBarControl = new Control();
            hideScrollBarControl.Visible = false;
            hideScrollBarControl.Enabled = false;
            hideScrollBarControl.TabStop = false;
            // control is disposed automatically when the grid is disposed
            this.Controls.Add(hideScrollBarControl);
        }

        protected override void OnRowEnter(DataGridViewCellEventArgs e)
        {
            // ensure full row select
            base.OnRowEnter(e);
            if (this.SelectionMode == DataGridViewSelectionMode.CellSelect ||
                (this.SelectionMode == DataGridViewSelectionMode.FullRowSelect &&
                base.Rows[e.RowIndex].Selected == false))
            {
                this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                base.Rows[e.RowIndex].Selected = true;
            }
        }

        private void LockVerticalScrollBarUpdate(bool lockUpdate/*, bool delayed*/)
        {
            // Temporarly hide/show the vertical scroll bar by changing its parent
            if (!this.inExpandCollapse)
            {
                if (lockUpdate)
                {
                    this.VerticalScrollBar.Parent = hideScrollBarControl;
                }
                else
                {
                    this.VerticalScrollBar.Parent = this;
                }
            }
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            if (typeof(TreeGridColumn).IsAssignableFrom(e.Column.GetType()))
            {
                if (expandableColumn == null)
                {
                    // identify the expanding column.			
                    expandableColumn = (TreeGridColumn)e.Column;
                }
                else
                {
                    // this.Columns.Remove(e.Column);
                    //throw new InvalidOperationException("Only one TreeGridColumn per TreeGridView is supported.");
                }
            }

            // Expandable Grid doesn't support sorting. This is just a limitation of the sample.
            e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;

            base.OnColumnAdded(e);
        }
        #endregion
    }
}
