// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using Xilium.CefGlue;

namespace NeoAxis.UIWebBrowserControl
{
	class WebDisplayHandler : CefDisplayHandler
	{
		private readonly UIWebBrowser owner;

		public WebDisplayHandler( UIWebBrowser owner )
		{
			if( owner == null )
				throw new ArgumentNullException( "owner" );

			this.owner = owner;
		}

		protected override void OnTitleChange( CefBrowser browser, string title )
		{
			owner.OnTitleChanged( title );
		}

		protected override void OnAddressChange( CefBrowser browser, CefFrame frame, string url )
		{
			if( frame.IsMain )
			{
				owner.OnAddressChanged( url );
			}
		}

		protected override void OnStatusMessage( CefBrowser browser, string value )
		{
			owner.OnTargetUrlChanged( value );
		}

		protected override bool OnTooltip( CefBrowser browser, string text )
		{
			return owner.OnTooltip( text );
		}
	}
}
