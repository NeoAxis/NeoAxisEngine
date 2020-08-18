using System.ComponentModel;

namespace System.Windows.Forms
{
	public sealed class LayoutEventArgs : EventArgs
	{
		public IComponent AffectedComponent
		{
			get
			{
				throw null;
			}
		}

		public Control AffectedControl
		{
			get
			{
				throw null;
			}
		}

		public string AffectedProperty
		{
			get
			{
				throw null;
			}
		}

		public LayoutEventArgs(IComponent affectedComponent, string affectedProperty)
		{
			throw null;
		}

		public LayoutEventArgs(Control affectedControl, string affectedProperty)
		{
			throw null;
		}
	}
}
