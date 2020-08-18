namespace System.Windows.Forms
{
	public class ScrollEventArgs : EventArgs
	{
		public ScrollOrientation ScrollOrientation
		{
			get
			{
				throw null;
			}
		}

		public ScrollEventType Type
		{
			get
			{
				throw null;
			}
		}

		public int NewValue
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

		public int OldValue
		{
			get
			{
				throw null;
			}
		}

		public ScrollEventArgs(ScrollEventType type, int newValue)
		{
			throw null;
		}

		public ScrollEventArgs(ScrollEventType type, int newValue, ScrollOrientation scroll)
		{
			throw null;
		}

		public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue)
		{
			throw null;
		}

		public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue, ScrollOrientation scroll)
		{
			throw null;
		}
	}
}
