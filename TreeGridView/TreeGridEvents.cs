using System;
using System.ComponentModel;

namespace TreeGridView
{
	public class TreeGridNodeEventBase : EventArgs
	{
        public TreeGridNodeEventBase(TreeGridNode node)
		{
			Node = node;
		}

		public TreeGridNode Node { get; private set; }
    }

	public class CollapsingEventArgs : CancelEventArgs
	{
        public CollapsingEventArgs(TreeGridNode node)
		{
			Node = node;
		}
		public TreeGridNode Node { get; private set; }
    }

	public class CollapsedEventArgs : TreeGridNodeEventBase
	{
		public CollapsedEventArgs(TreeGridNode node)
			: base(node)
		{
		}
	}

	public class ExpandingEventArgs:CancelEventArgs
	{
        public ExpandingEventArgs(TreeGridNode node)
        {
			Node = node;
		}
		public TreeGridNode Node { get; private set; }
    }

	public class ExpandedEventArgs : TreeGridNodeEventBase
	{
		public ExpandedEventArgs(TreeGridNode node):base(node)
		{
		}
	}

    public class CheckedEventArgs : TreeGridNodeEventBase
    {
        public CheckedEventArgs(TreeGridNode node)
            : base(node)
        {
        }

        public CheckedEventArgs(TreeGridNode node, bool isChangedByProgram)
            : this(node)
        {
            IsChangedByProgram = isChangedByProgram;
        }

        public bool IsChangedByProgram { get; private set; }
    }

	public delegate void ExpandingEventHandler(object sender, ExpandingEventArgs e);

	public delegate void ExpandedEventHandler(object sender, ExpandedEventArgs e);

	public delegate void CollapsingEventHandler(object sender, CollapsingEventArgs e);

	public delegate void CollapsedEventHandler(object sender, CollapsedEventArgs e);

    public delegate void CheckedEventHandler(object sender, CheckedEventArgs e);
}
