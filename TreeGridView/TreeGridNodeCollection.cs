using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TreeGridView
{
	public class TreeGridNodeCollection : IList<TreeGridNode>
	{
		private readonly List<TreeGridNode> _list;
        private TreeGridView _gird;

        internal TreeGridNodeCollection(TreeGridNode owner)
		{
            OwningNode = owner;
			_list = new List<TreeGridNode>();
		}

        #region Properties

        /// <summary>
        /// 获取包含此单元格的节点
        /// </summary>
        public TreeGridNode OwningNode { get; private set; }

        /// <summary>
        /// 包含此节点集合的树形数据表格
        /// </summary>
        internal TreeGridView Grid
        {
            get { return _gird ?? (OwningNode != null ? OwningNode.Grid : null); }
            set { _gird = value; }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public TreeGridNode this[int index]
        {
            get { return _list[index]; }
            set
            {
                var existNode = _list[index];
                if (Grid != null)
                {
                    Grid.Rows.Remove(existNode);
                }

                existNode.OwningNodeCollection = null;

                value.OwningNodeCollection = this;
                value.Nodes.Grid = Grid;
                _list[index] = value;
                AddRow(value);
            }
        }

        #endregion

        #region Public Members

        public TreeGridNode Add(string text)
        {
            TreeGridNode node = new TreeGridNode();
            node.CreateCells(Grid);
            node.Cells[0].Value = text;
            Add(node);
            return node;
        }

        public void Add(TreeGridNode item)
		{
            if (item == null) return;
			// The row needs to exist in the child collection before the parent is notified.
            item.OwningNodeCollection = this;
            item.Nodes.Grid = Grid;

			_list.Add(item);
            AddRow(item);
        }

        public void Insert(int index, TreeGridNode item)
        {
            if (item == null) return;
            // The row needs to exist in the child collection before the parent is notified.
            item.OwningNodeCollection = this;
            item.Nodes.Grid = Grid;

            _list.Insert(index, item);
            AddRow(item);
        }

        public bool Remove(TreeGridNode node)
		{
            if (node == null) return false;
            var result = _list.Remove(node);
            node.OwningNodeCollection = null;

			// The parent is notified first then the row is removed from the child collection.
            if (Grid != null && Grid.Rows.Contains(node))
            {
                Grid.Rows.Remove(node);
            }

            return result;
        }

        public void RemoveAt(int index)
		{
            TreeGridNode node = _list[index];
			_list.RemoveAt(index);
            node.OwningNodeCollection = null;

			// The parent is notified first then the row is removed from the child collection.
            if (Grid != null && Grid.Rows.Contains(node))
            {
                Grid.Rows.Remove(node);
            }
        }

        public void Clear()
		{
            if (Grid != null)
            {
                foreach (TreeGridNode node in this)
                {
                    if (Grid.Rows.Contains(node))
                    {
                        Grid.Rows.Remove(node);
                    }

                    node.OwningNodeCollection = null;
                }
            }

            _list.Clear();
        }

        public int IndexOf(TreeGridNode item)
        {
            return _list.IndexOf(item);
        }

		public bool Contains(TreeGridNode item)
		{
			return Contains(item, false);
		}

        internal bool Contains(TreeGridNode node, bool searchChildren)
        {
            return searchChildren ? Contains(this, node) : _list.Contains(node);
        }

        public void CopyTo(TreeGridNode[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

        public IEnumerator<TreeGridNode> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region Private Members

        private void AddRow(TreeGridNode node)
        {
            if (Grid == null) return;
            if (!Grid.Rows.Contains(node))
            {
                var rowIndex = GetRowInsertIndex(node);
                Grid.Rows.Insert(rowIndex, node);
                node.Site();
                node.Visible = OwningNode == null || OwningNode.IsExpanded;
                Grid.InvalidateRow(rowIndex);
            }
        }

        private int GetRowInsertIndex(TreeGridNode node)
        {
            if (Grid == null || Grid.Rows.Contains(node)) return -1;
            var nodeIndex = _list.IndexOf(node);
            if (nodeIndex == 0)
            {
                if (OwningNode == null)
                {
                    return 0;
                }
                else
                {
                    return Grid.Rows.IndexOf(OwningNode) + 1;
                }
            }
            else
            {
                var targetNode = _list[nodeIndex - 1];
                return Grid.Rows.IndexOf(targetNode) + GetChildrenCount(targetNode, true) + 1;
            }
        }

        private bool Contains(TreeGridNodeCollection collection, TreeGridNode node)
        {
            return collection.Contains(node) || collection.Any(n => Contains(n.Nodes, node));
        }

        private int GetChildrenCount(TreeGridNode node, bool recursive)
        {
            if (recursive)
            {
                return node.Nodes.Count + node.Nodes.Sum(n => GetChildrenCount(n, true));
            }
            else
            {
                return node.Nodes.Count;
            }
        }

        #endregion
    }
}
