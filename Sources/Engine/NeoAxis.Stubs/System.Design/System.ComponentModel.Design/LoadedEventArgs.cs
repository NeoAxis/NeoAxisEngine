using System.Collections;

namespace System.ComponentModel.Design
{
	public sealed class LoadedEventArgs : EventArgs
	{
		public ICollection Errors
		{
			get
			{
				throw null;
			}
		}

		public bool HasSucceeded
		{
			get
			{
				throw null;
			}
		}

		public LoadedEventArgs(bool succeeded, ICollection errors)
		{
			throw null;
		}
	}
}
