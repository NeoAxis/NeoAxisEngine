// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using Xilium.CefGlue;

namespace NeoAxis.UIWebBrowserControl
{
	class WebLifeSpanHandler : CefLifeSpanHandler
	{
		private readonly UIWebBrowser owner;

		public WebLifeSpanHandler( UIWebBrowser owner )
		{
			if( owner == null )
				throw new ArgumentNullException( "owner" );

			this.owner = owner;
		}

		protected override void OnAfterCreated( CefBrowser browser )
		{
			base.OnAfterCreated( browser );

			this.owner.HandleAfterCreated( browser );
		}

		protected override bool DoClose( CefBrowser browser )
		{
			// TODO: ... dispose owner
			return false;
		}

		protected override bool OnBeforePopup( CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref bool noJavascriptAccess )
		{
			var e = new UIWebBrowser.BeforePopupEventArgs( frame, targetUrl, targetFrameName, popupFeatures, windowInfo, client, settings,
					 noJavascriptAccess );

			this.owner.OnBeforePopup( e );

			client = e.Client;
			noJavascriptAccess = e.NoJavascriptAccess;

			return e.Handled;
		}
	}
}
