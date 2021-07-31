namespace System.Windows.Forms
{
	public class ApplicationContext : IDisposable
	{
		public Form MainForm
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

		public object Tag
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

		public event EventHandler ThreadExit
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public ApplicationContext()
		{
			throw null;
		}

		public ApplicationContext(Form mainForm)
		{
			throw null;
		}

		~ApplicationContext()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		protected virtual void Dispose(bool disposing)
		{
			throw null;
		}

		public void ExitThread()
		{
			throw null;
		}

		protected virtual void ExitThreadCore()
		{
			throw null;
		}

		protected virtual void OnMainFormClosed(object sender, EventArgs e)
		{
			throw null;
		}
	}
}
