using System.ComponentModel;

namespace System.Drawing.Printing
{
	public class PrintDocument : Component
	{
		public PageSettings DefaultPageSettings
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

		public string DocumentName
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

		public bool OriginAtMargins
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

		public PrintController PrintController
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

		public PrinterSettings PrinterSettings
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

		public event PrintEventHandler BeginPrint
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

		public event PrintEventHandler EndPrint
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

		public event PrintPageEventHandler PrintPage
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

		public event QueryPageSettingsEventHandler QueryPageSettings
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

		public PrintDocument()
		{
			throw null;
		}

		protected virtual void OnBeginPrint(PrintEventArgs e)
		{
			throw null;
		}

		protected virtual void OnEndPrint(PrintEventArgs e)
		{
			throw null;
		}

		protected virtual void OnPrintPage(PrintPageEventArgs e)
		{
			throw null;
		}

		protected virtual void OnQueryPageSettings(QueryPageSettingsEventArgs e)
		{
			throw null;
		}

		public void Print()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
