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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace System.Windows.Forms
{
	public class TreeGridViewRowCollection : DataGridViewRowCollection
	{
        public TreeGridViewRowCollection(DataGridView dataGridView)
            : base(dataGridView)
        {
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TreeGridView Grid => DataGridView as TreeGridView;

        #region Public Members

        #region Add

        public override int Add(DataGridViewRow dataGridViewRow)
        {
            int index = base.Add(dataGridViewRow);
            AddNode(dataGridViewRow as TreeGridNode);
            return index;
        }

        public override int Add()
        {
            int index = base.Add();
            AddNode(index);
            return index;
        }

        public override int Add(params object[] values)
        {
            int index = base.Add(values);
            AddNode(index);
            return index;
        }

        public override int Add(int count)
        {
            int index = base.Add(count);
            AddNode(Count - count, count);
            return index;
        }

        public override int AddCopies(int indexSource, int count)
        {
            int index = base.AddCopies(indexSource, count);
            AddNode(indexSource, count);
            return index;
        }

        public override int AddCopy(int indexSource)
        {
            int index = base.AddCopy(indexSource);
            AddNode(index);
            return index;
        }

        public override void AddRange(params DataGridViewRow[] dataGridViewRows)
        {
            base.AddRange(dataGridViewRows);
            foreach (var node in dataGridViewRows.OfType<TreeGridNode>())
            {
                AddNode(node);
            }
        }

        public TreeGridNode Add(string text)
        {
            var node = new TreeGridNode();
            node.CreateCells(Grid);
            node.Cells[0].Value = text;
            Add(node);
            return node;
        }

        #endregion

        #region Insert

        public override void Insert(int rowIndex, params object[] values)
        {
            base.Insert(rowIndex, values);
            InsertNode(rowIndex);
        }

        public override void Insert(int rowIndex, DataGridViewRow dataGridViewRow)
        {
            base.Insert(rowIndex, dataGridViewRow);
            InsertNode(rowIndex);
        }

        public override void Insert(int rowIndex, int count)
        {
            base.Insert(rowIndex, count);
            InsertNode(rowIndex, count);
        }

        public override void InsertCopies(int indexSource, int indexDestination, int count)
        {
            base.InsertCopies(indexSource, indexDestination, count);
            InsertNode(indexDestination, count);
        }

        public override void InsertCopy(int indexSource, int indexDestination)
        {
            base.InsertCopy(indexSource, indexDestination);
            InsertNode(indexDestination);
        }

        public override void InsertRange(int rowIndex, params DataGridViewRow[] dataGridViewRows)
        {
            base.InsertRange(rowIndex, dataGridViewRows);
            for (int i = 0; i < dataGridViewRows.Length; i++)
            {
                InsertNode(rowIndex + i);
            }
        }

        #endregion

        #region Remove

        public override void Remove(DataGridViewRow dataGridViewRow)
        {
            base.Remove(dataGridViewRow);
            Remove(dataGridViewRow as TreeGridNode);
        }

        public override void RemoveAt(int index)
        {
            var node = this[index] as TreeGridNode;
            base.RemoveAt(index);
            Remove(node);
        }

        #endregion

        #endregion

        #region Private Members

        private void AddNode(TreeGridNode node)
        {
            if (node == null || Grid == null || node.OwningNodeCollection != null) return;
            if (!Grid.Nodes.Contains(node, true))
            {
                Grid.Nodes.Add(node);
            }
        }

        private void AddNode(int index)
        {
            AddNode(this[index] as TreeGridNode);
        }

        private void AddNode(int skip, int take)
        {
            List<TreeGridNode> nodes = this.Cast<DataGridViewRow>()
                .Skip(skip)
                .Take(take)
                .OfType<TreeGridNode>()
                .ToList();
            foreach (var node in nodes)
            {
                AddNode(node);
            }
        }

        private void InsertNode(int index)
        {
            if ((index == 0 && Count == 1) || index == Count - 1)
            {
                AddNode(index);
            }
            else
            {
                var targetNode = this[index + 1] as TreeGridNode;
                if (targetNode == null)
                {
                    targetNode =this.Cast<DataGridViewRow>()
                        .Skip(index)
                        .OfType<TreeGridNode>()
                        .FirstOrDefault();
                }

                if (targetNode == null)
                {
                    AddNode(index);
                }
                else
                {
                    targetNode.OwningNodeCollection.Insert(targetNode.NodeIndex, this[index] as TreeGridNode);
                }
            }
        }

        private void InsertNode(int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                InsertNode(index + i);
            }
        }

        private void Remove(TreeGridNode node)
        {
            if (node == null || node.OwningNodeCollection == null) return;
            if (node.OwningNodeCollection.Contains(node))
            {
                node.OwningNodeCollection.Remove(node);
            }
        }

        #endregion
    }
}
