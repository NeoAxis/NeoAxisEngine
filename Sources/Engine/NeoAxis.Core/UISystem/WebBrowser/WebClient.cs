// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace NeoAxis.UIWebBrowserControl
{
	/*public */class EmptyCefContextMenuHandlerImpl : CefContextMenuHandler
	{
		protected override void OnBeforeContextMenu( CefBrowser browser, CefFrame frame, CefContextMenuParams state, CefMenuModel model )
		{
			model.Clear();
		}
	}

	class WebClient : CefClient
	{
		private readonly UIWebBrowser owner;
		private readonly WebLifeSpanHandler lifeSpanHandler;
		private readonly WebDisplayHandler displayHandler;
		private readonly WebRenderHandler renderHandler;
		private readonly WebLoadHandler loadHandler;
		private readonly WebDownloadHandler downloadHandler;

		public WebClient( UIWebBrowser owner )
		{
			if( owner == null )
				throw new ArgumentNullException( "owner" );

			this.owner = owner;
			lifeSpanHandler = new WebLifeSpanHandler( this.owner );
			displayHandler = new WebDisplayHandler( this.owner );
			renderHandler = new WebRenderHandler( this.owner );
			loadHandler = new WebLoadHandler( this.owner );
			downloadHandler = new WebDownloadHandler( this.owner );
		}

		protected override CefLifeSpanHandler GetLifeSpanHandler()
		{
			return lifeSpanHandler;
		}

		protected override CefDisplayHandler GetDisplayHandler()
		{
			return displayHandler;
		}

		protected override CefRenderHandler GetRenderHandler()
		{
			return renderHandler;
		}

		protected override CefLoadHandler GetLoadHandler()
		{
			return loadHandler;
		}

		protected override CefContextMenuHandler GetContextMenuHandler()
		{
			return new EmptyCefContextMenuHandlerImpl();
		}

		protected override CefDownloadHandler GetDownloadHandler()
		{
			return downloadHandler;
		}
	}
}
