namespace System.Drawing.Printing
{
	public abstract class PrintController
	{
		public virtual bool IsPreview
		{
			get
			{
				throw null;
			}
		}

		protected PrintController()
		{
			throw null;
		}

		public virtual void OnStartPrint(PrintDocument document, PrintEventArgs e)
		{
			throw null;
		}

		public virtual Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
		{
			throw null;
		}

		public virtual void OnEndPage(PrintDocument document, PrintPageEventArgs e)
		{
			throw null;
		}

		public virtual void OnEndPrint(PrintDocument document, PrintEventArgs e)
		{
			throw null;
		}
	}
}
