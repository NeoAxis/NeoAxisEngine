namespace System.Windows.Forms
{
	public class HandledMouseEventArgs : MouseEventArgs
	{
		public bool Handled
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

		public HandledMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
			:base(button, clicks,x,y,delta)
		{
			throw null;
		}

		public HandledMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, bool defaultHandledValue)
			:base(button, clicks,x,y,delta)
		{
			throw null;
		}
	}
}
