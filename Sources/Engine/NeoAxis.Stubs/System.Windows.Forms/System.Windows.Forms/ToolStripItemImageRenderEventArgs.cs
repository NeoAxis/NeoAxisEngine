using System.Drawing;

namespace System.Windows.Forms
{
	public class ToolStripItemImageRenderEventArgs : ToolStripItemRenderEventArgs
	{
		public Image Image
		{
			get
			{
				throw null;
			}
		}

		public Rectangle ImageRectangle
		{
			get
			{
				throw null;
			}
		}

		public ToolStripItemImageRenderEventArgs(Graphics g, ToolStripItem item, Rectangle imageRectangle)
			: base( g, item )
		{
			throw null;
		}

		public ToolStripItemImageRenderEventArgs(Graphics g, ToolStripItem item, Image image, Rectangle imageRectangle)
			:base(g, item)
		{
			throw null;
		}
	}
}
