//---------------------------------------------------------------------
// 
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
//KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//PARTICULAR PURPOSE.
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedDataGridView
{
	public class TreeGridNodeEventBase : EventArgs
	{
		private TreeGridNode _node;

		public TreeGridNodeEventBase(TreeGridNode node)
		{
			this._node = node;
		}

		public TreeGridNode Node
		{
			get { return _node; }
		}
	}
	public class CollapsingEventArgs : System.ComponentModel.CancelEventArgs
	{
		private TreeGridNode _node;

		private CollapsingEventArgs() { }
		public CollapsingEventArgs(TreeGridNode node)
			: base()
		{
			this._node = node;
		}
		public TreeGridNode Node
		{
			get { return _node; }
		}

	}
	public class CollapsedEventArgs : TreeGridNodeEventBase
	{
		public CollapsedEventArgs(TreeGridNode node)
			: base(node)
		{
		}
	}

	public class ExpandingEventArgs:System.ComponentModel.CancelEventArgs
	{
		private TreeGridNode _node;

		private ExpandingEventArgs() { }
		public ExpandingEventArgs(TreeGridNode node):base()
		{
			this._node = node;
		}
		public TreeGridNode Node
		{
			get { return _node; }
		}

	}
	public class ExpandedEventArgs : TreeGridNodeEventBase
	{
		public ExpandedEventArgs(TreeGridNode node):base(node)
		{
		}
	}

    public class CheckedEventArgs : TreeGridNodeEventBase
    {
        private bool isChangedByProgram;

        public CheckedEventArgs(TreeGridNode node)
            : base(node)
        {
        }

        public CheckedEventArgs(TreeGridNode node, bool isChangedByProgram)
            : this(node)
        {
            this.isChangedByProgram = isChangedByProgram;
        }

        public bool IsChangedByProgram
        {
            get
            {
                return this.isChangedByProgram;
            }
        }
    }

	public delegate void ExpandingEventHandler(object sender, ExpandingEventArgs e);
	public delegate void ExpandedEventHandler(object sender, ExpandedEventArgs e);

	public delegate void CollapsingEventHandler(object sender, CollapsingEventArgs e);
	public delegate void CollapsedEventHandler(object sender, CollapsedEventArgs e);

    public delegate void CheckedEventHandler(object sender, CheckedEventArgs e);
}
