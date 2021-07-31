using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace System.Windows.Forms
{
	public class WebBrowser : WebBrowserBase
	{
		protected class WebBrowserSite : WebBrowserSiteBase
		{
			public WebBrowserSite(WebBrowser host)
			{
				throw null;
			}
		}

		public bool AllowNavigation
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

		public bool AllowWebBrowserDrop
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

		public bool ScriptErrorsSuppressed
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

		public bool WebBrowserShortcutsEnabled
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

		public bool CanGoBack
		{
			get
			{
				throw null;
			}
		}

		public bool CanGoForward
		{
			get
			{
				throw null;
			}
		}

		public HtmlDocument Document
		{
			get
			{
				throw null;
			}
		}

		public Stream DocumentStream
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

		public string DocumentText
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

		public string DocumentTitle
		{
			get
			{
				throw null;
			}
		}

		public string DocumentType
		{
			get
			{
				throw null;
			}
		}

		public WebBrowserEncryptionLevel EncryptionLevel
		{
			get
			{
				throw null;
			}
		}

		public bool IsBusy
		{
			get
			{
				throw null;
			}
		}

		public bool IsOffline
		{
			get
			{
				throw null;
			}
		}

		public bool IsWebBrowserContextMenuEnabled
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

		public object ObjectForScripting
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

		public new Padding Padding
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

		public WebBrowserReadyState ReadyState
		{
			get
			{
				throw null;
			}
		}

		public virtual string StatusText
		{
			get
			{
				throw null;
			}
		}

		public Uri Url
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

		public Version Version
		{
			get
			{
				throw null;
			}
		}

		public bool ScrollBarsEnabled
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

		public override bool Focused
		{
			get
			{
				throw null;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				throw null;
			}
		}

		public new event EventHandler PaddingChanged
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

		public event EventHandler CanGoBackChanged
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

		public event EventHandler CanGoForwardChanged
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

		public event WebBrowserDocumentCompletedEventHandler DocumentCompleted
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

		public event EventHandler DocumentTitleChanged
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

		public event EventHandler EncryptionLevelChanged
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

		public event EventHandler FileDownload
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

		public event WebBrowserNavigatedEventHandler Navigated
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

		public event WebBrowserNavigatingEventHandler Navigating
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

		public event CancelEventHandler NewWindow
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

		public event WebBrowserProgressChangedEventHandler ProgressChanged
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

		public event EventHandler StatusTextChanged
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

		public WebBrowser()
		{
			throw null;
		}

		public bool GoBack()
		{
			throw null;
		}

		public bool GoForward()
		{
			throw null;
		}

		public void GoHome()
		{
			throw null;
		}

		public void GoSearch()
		{
			throw null;
		}

		public void Navigate(Uri url)
		{
			throw null;
		}

		public void Navigate(string urlString)
		{
			throw null;
		}

		public void Navigate(Uri url, string targetFrameName)
		{
			throw null;
		}

		public void Navigate(string urlString, string targetFrameName)
		{
			throw null;
		}

		public void Navigate(Uri url, bool newWindow)
		{
			throw null;
		}

		public void Navigate(string urlString, bool newWindow)
		{
			throw null;
		}

		public void Navigate(Uri url, string targetFrameName, byte[] postData, string additionalHeaders)
		{
			throw null;
		}

		public void Navigate(string urlString, string targetFrameName, byte[] postData, string additionalHeaders)
		{
			throw null;
		}

		public void Print()
		{
			throw null;
		}

		public override void Refresh()
		{
			throw null;
		}

		public void Refresh(WebBrowserRefreshOption opt)
		{
			throw null;
		}

		public void ShowPageSetupDialog()
		{
			throw null;
		}

		public void ShowPrintDialog()
		{
			throw null;
		}

		public void ShowPrintPreviewDialog()
		{
			throw null;
		}

		public void ShowPropertiesDialog()
		{
			throw null;
		}

		public void ShowSaveAsDialog()
		{
			throw null;
		}

		public void Stop()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		protected override void AttachInterfaces(object nativeActiveXObject)
		{
			throw null;
		}

		protected override void DetachInterfaces()
		{
			throw null;
		}

		protected override WebBrowserSiteBase CreateWebBrowserSiteBase()
		{
			throw null;
		}

		protected override void CreateSink()
		{
			throw null;
		}

		protected override void DetachSink()
		{
			throw null;
		}

		protected virtual void OnCanGoBackChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnCanGoForwardChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnDocumentTitleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnEncryptionLevelChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnFileDownload(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnNavigated(WebBrowserNavigatedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnNavigating(WebBrowserNavigatingEventArgs e)
		{
			throw null;
		}

		protected virtual void OnNewWindow(CancelEventArgs e)
		{
			throw null;
		}

		protected virtual void OnProgressChanged(WebBrowserProgressChangedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnStatusTextChanged(EventArgs e)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
