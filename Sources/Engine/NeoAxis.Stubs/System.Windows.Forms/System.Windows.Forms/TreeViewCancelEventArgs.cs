using System.ComponentModel;

namespace System.Windows.Forms
{
	public class TreeViewCancelEventArgs : CancelEventArgs
	{
		public TreeNode Node
		{
			get
			{
				throw null;
			}
		}

		public TreeViewAction Action
		{
			get
			{
				throw null;
			}
		}

		public TreeViewCancelEventArgs(TreeNode node, bool cancel, TreeViewAction action)
		{
			throw null;
		}
	}
}
