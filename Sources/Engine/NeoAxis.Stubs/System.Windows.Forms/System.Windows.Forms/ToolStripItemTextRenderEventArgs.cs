using System.Drawing;

namespace System.Windows.Forms
{
	public class ToolStripItemTextRenderEventArgs : ToolStripItemRenderEventArgs
	{
		public string Text
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Color TextColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Font TextFont
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Rectangle TextRectangle
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public TextFormatFlags TextFormat
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public ToolStripTextDirection TextDirection
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public ToolStripItemTextRenderEventArgs(Graphics g, ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, TextFormatFlags format)
			:base(g, item)
		{
			throw null;
		}

		public ToolStripItemTextRenderEventArgs(Graphics g, ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, ContentAlignment textAlign)
			: base( g, item )
		{
			throw null;
		}
	}
}
