using System.ComponentModel;

namespace System.Windows.Forms
{
	public class BindingCompleteEventArgs : CancelEventArgs
	{
		public Binding Binding
		{
			get
			{
				throw null;
			}
		}

		public BindingCompleteState BindingCompleteState
		{
			get
			{
				throw null;
			}
		}

		public BindingCompleteContext BindingCompleteContext
		{
			get
			{
				throw null;
			}
		}

		public string ErrorText
		{
			get
			{
				throw null;
			}
		}

		public Exception Exception
		{
			get
			{
				throw null;
			}
		}

		public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception, bool cancel)
		{
			throw null;
		}

		public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception)
		{
			throw null;
		}

		public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText)
		{
			throw null;
		}

		public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context)
		{
			throw null;
		}
	}
}
