#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Text;

namespace Internal.Aga.Controls.Tree.NodeControls
{
	public class NodeControlValueEventArgs : NodeEventArgs
	{
		private object _value;
		public object Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public NodeControlValueEventArgs(TreeNodeAdv node)
			:base(node)
		{
		}
	}
}

#endif