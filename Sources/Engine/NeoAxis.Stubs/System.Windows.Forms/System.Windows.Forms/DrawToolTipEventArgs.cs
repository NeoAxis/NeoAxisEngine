using System.Drawing;

namespace System.Windows.Forms
{
	public class DrawToolTipEventArgs : EventArgs
	{
		public Graphics Graphics
		{
			get
			{
				throw null;
			}
		}

		public IWin32Window AssociatedWindow
		{
			get
			{
				throw null;
			}
		}

		public Control AssociatedControl
		{
			get
			{
				throw null;
			}
		}

		public Rectangle Bounds
		{
			get
			{
				throw null;
			}
		}

		public string ToolTipText
		{
			get
			{
				throw null;
			}
		}

		public Font Font
		{
			get
			{
				throw null;
			}
		}

		public DrawToolTipEventArgs(Graphics graphics, IWin32Window associatedWindow, Control associatedControl, Rectangle bounds, string toolTipText, Color backColor, Color foreColor, Font font)
		{
			throw null;
		}

		public void DrawBackground()
		{
			throw null;
		}

		public void DrawText()
		{
			throw null;
		}

		public void DrawText(TextFormatFlags flags)
		{
			throw null;
		}

		public void DrawBorder()
		{
			throw null;
		}
	}
}
