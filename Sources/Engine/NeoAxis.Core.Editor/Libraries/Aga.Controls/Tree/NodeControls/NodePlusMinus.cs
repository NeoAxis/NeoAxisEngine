#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Internal.Aga.Controls.Properties;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace Internal.Aga.Controls.Tree.NodeControls
{
	internal class NodePlusMinus : NodeControl
	{
		public readonly static int ImageSize = DpiHelper.Default.ScaleValue(9);
		public readonly static int Width = DpiHelper.Default.ScaleValue(12);
		private Bitmap _plus;
		private Bitmap _minus;
		//!!!!betauser
		bool imagesForDarkTheme;

		private VisualStyleRenderer _openedRenderer;
		private VisualStyleRenderer OpenedRenderer
		{
			get
			{
				if (_openedRenderer == null)
					_openedRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
				return _openedRenderer;

			}
		}

		private VisualStyleRenderer _closedRenderer;
		private VisualStyleRenderer ClosedRenderer
		{
			get
			{
				if (_closedRenderer == null)
					_closedRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
				return _closedRenderer;
			}
		}

		public NodePlusMinus()
		{
			_plus = Resources.plus;
			_minus = Resources.minus;
		}

		static NodePlusMinus()
		{
			// use predifined ImageSize for common scales
			new Dictionary<float, int> {
				{ 1f, 9 }, { 1.25f, 9 }, { 1.5f, 11 }, { 1.75f, 13 }, { 2f, 15 }
			}.TryGetValue(DpiHelper.Default.DpiScaleFactor, out ImageSize);
		}

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			return new Size(Width, Width);
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			if (node.CanExpand)
			{
				Rectangle r = context.Bounds;

				//!!!!betauser
				if( imagesForDarkTheme != BaseTextControl.DarkTheme )
				{
					imagesForDarkTheme = BaseTextControl.DarkTheme;
					if( BaseTextControl.DarkTheme )
					{
						_plus = Resources.plus_Dark;
						_minus = Resources.minus_Dark;
					}
					else
					{
						_plus = Resources.plus;
						_minus = Resources.minus;
					}
				}

				int dy = (int)Math.Round((float)(r.Height - ImageSize) / 2);
				//!!!!betauser
				//if (Application.RenderWithVisualStyles)
				//{
				//	var renderer = node.IsExpanded ? OpenedRenderer : ClosedRenderer;
				//	renderer.DrawBackground(context.Graphics, new Rectangle(r.X, r.Y + dy, ImageSize, ImageSize));
				//}
				//else
				{
					context.Graphics.DrawImageUnscaled(node.IsExpanded ? _minus : _plus, new Point(r.X, r.Y + dy));
				}
			}
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
			if (args.Button == MouseButtons.Left)
			{
				args.Handled = true;
				if (args.Node.CanExpand)
					args.Node.IsExpanded = !args.Node.IsExpanded;
			}
		}

		public override void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
		{
			args.Handled = true; // Supress expand/collapse when double click on plus/minus
		}
	}
}

#endif