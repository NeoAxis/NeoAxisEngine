using System.Drawing;
using System.Drawing.Printing;

namespace System.Windows.Forms
{
	public class PrintControllerWithStatusDialog : PrintController
	{
		public override bool IsPreview
		{
			get
			{
				throw null;
			}
		}

		public PrintControllerWithStatusDialog(PrintController underlyingController)
		{
			throw null;
		}

		public PrintControllerWithStatusDialog(PrintController underlyingController, string dialogTitle)
		{
			throw null;
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
	}
}
