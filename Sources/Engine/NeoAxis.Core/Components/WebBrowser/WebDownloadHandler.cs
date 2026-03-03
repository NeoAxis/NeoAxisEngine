#if !NO_UI_WEB_BROWSER
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using Internal.Xilium.CefGlue;

namespace NeoAxis.UIWebBrowserControl
{
	internal sealed class WebDownloadHandler : CefDownloadHandler
	{
		readonly UIWebBrowser owner;

		public WebDownloadHandler( UIWebBrowser owner )
		{
			if( owner == null )
				throw new ArgumentNullException( "owner" );

			this.owner = owner;
		}

		protected override void OnBeforeDownload( CefBrowser browser, CefDownloadItem downloadItem, string suggestedName, CefBeforeDownloadCallback callback )
		{
			//public void Continue( string downloadPath, bool showDialog )

			owner.PerformDownloadBefore( downloadItem, suggestedName, callback );

			//Log.Info( "OnBeforeDownload: " + suggestedName );

			base.OnBeforeDownload( browser, downloadItem, suggestedName, callback );
		}

		protected override void OnDownloadUpdated( CefBrowser browser, CefDownloadItem downloadItem, CefDownloadItemCallback callback )
		{
			owner.PerformDownloadUpdated( downloadItem, callback );

			//Log.Info( "OnDownloadUpdated: " + downloadItem.TotalBytes.ToString() + " " + downloadItem.ReceivedBytes.ToString() + " " + downloadItem.PercentComplete.ToString() + " " + downloadItem.IsComplete.ToString() + " " + downloadItem.FullPath );

			base.OnDownloadUpdated( browser, downloadItem, callback );
		}
	}
}
#endif