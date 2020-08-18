using System.Drawing;

namespace System.Windows.Forms
{
	public class StatusBarDrawItemEventArgs : DrawItemEventArgs
	{
		public StatusBarPanel Panel
		{
			get
			{
				throw null;
			}
		}

		public StatusBarDrawItemEventArgs(Graphics g, Font font, Rectangle r, int itemId, DrawItemState itemState, StatusBarPanel panel)
			: base( g, font, r, itemId, itemState )
		{
			throw null;
		}

		public StatusBarDrawItemEventArgs(Graphics g, Font font, Rectangle r, int itemId, DrawItemState itemState, StatusBarPanel panel, Color foreColor, Color backColor)
			: base( g, font, r, itemId, itemState )
		{
			throw null;
		}
	}
}
