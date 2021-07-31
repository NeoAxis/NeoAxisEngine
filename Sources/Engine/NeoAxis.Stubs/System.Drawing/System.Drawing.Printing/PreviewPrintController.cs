namespace System.Drawing.Printing
{
	public class PreviewPrintController : PrintController
	{
		public override bool IsPreview
		{
			get
			{
				throw null;
			}
		}

		public virtual bool UseAntiAlias
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

		public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
		{
			throw null;
		}

		public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
		{
			throw null;
		}

		public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
		{
			throw null;
		}

		public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
		{
			throw null;
		}

		public PreviewPageInfo[] GetPreviewPageInfo()
		{
			throw null;
		}

		public PreviewPrintController()
		{
			throw null;
		}
	}
}
