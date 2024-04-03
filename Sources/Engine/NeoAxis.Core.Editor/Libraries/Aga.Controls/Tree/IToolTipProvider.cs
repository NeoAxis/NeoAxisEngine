#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Text;
using Internal.Aga.Controls.Tree.NodeControls;

namespace Internal.Aga.Controls.Tree
{
	public interface IToolTipProvider
	{
		string GetToolTip(TreeNodeAdv node, NodeControl nodeControl);
	}
}

#endif