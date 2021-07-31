using System.ComponentModel;

namespace System.Windows.Forms
{
	public class WebBrowserNavigatingEventArgs : CancelEventArgs
	{
		public Uri Url
		{
			get
			{
				throw null;
			}
		}

		public string TargetFrameName
		{
			get
			{
				throw null;
			}
		}

		public WebBrowserNavigatingEventArgs(Uri url, string targetFrameName)
		{
			throw null;
		}
	}
}
