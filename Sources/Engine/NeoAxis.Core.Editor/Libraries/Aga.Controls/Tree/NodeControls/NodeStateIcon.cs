#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Internal.Aga.Controls.Properties;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace Internal.Aga.Controls.Tree.NodeControls
{
	public class NodeStateIcon: NodeIcon
	{
		private Image _leaf;
		private Image _opened;
		private Image _closed;

		public NodeStateIcon()
		{
			_leaf = MakeTransparentAndScale(Resources.Leaf);
			_opened = MakeTransparentAndScale(Resources.Folder);
			_closed = MakeTransparentAndScale(Resources.FolderClosed);
		}

		private static Image MakeTransparentAndScale(Bitmap bitmap)
		{
			bitmap.MakeTransparent(bitmap.GetPixel(0,0));

			DpiHelper.Default.ScaleBitmapLogicalToDevice(ref bitmap);

			return bitmap;
		}

		protected override Image GetIcon(TreeNodeAdv node)
		{
			Image icon = base.GetIcon(node);
			if (icon != null)
				return icon;
			else if (node.IsLeaf)
				return _leaf;
			else if (node.CanExpand && node.IsExpanded)
				return _opened;
			else
				return _closed;
		}
	}
}

#endif