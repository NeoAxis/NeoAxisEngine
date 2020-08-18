namespace System.Windows.Forms
{
	public class StatusBarPanelClickEventArgs : MouseEventArgs
	{
		public StatusBarPanel StatusBarPanel
		{
			get
			{
				throw null;
			}
		}

		public StatusBarPanelClickEventArgs(StatusBarPanel statusBarPanel, MouseButtons button, int clicks, int x, int y)
			: base( button, clicks,x,y,0 )
		{
			throw null;
		}
	}
}
