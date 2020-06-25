// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Xilium.CefGlue;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NeoAxis.UIWebBrowserControl
{
	class WebRenderHandler : CefRenderHandler
	{
		private readonly UIWebBrowser owner;

		public WebRenderHandler( UIWebBrowser owner )
		{
			if( owner == null )
				throw new ArgumentNullException( "owner" );

			this.owner = owner;
		}

		protected override bool GetRootScreenRect( CefBrowser browser, ref CefRectangle rect )
		{
			return owner.GetViewRect( ref rect );
		}

		protected override bool GetViewRect( CefBrowser browser, ref CefRectangle rect )
		{
			return owner.GetViewRect( ref rect );
		}

		protected override bool GetScreenPoint( CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY )
		{
			owner.GetScreenPoint( viewX, viewY, ref screenX, ref screenY );
			return true;
		}

		protected override bool GetScreenInfo( CefBrowser browser, CefScreenInfo screenInfo )
		{
			return false;
		}

		protected override void OnPopupShow( CefBrowser browser, bool show )
		{
			//owner.OnPopupShow(show);
		}

		protected override void OnPopupSize( CefBrowser browser, CefRectangle rect )
		{
			//owner.OnPopupSize(rect);
		}

		protected override void OnPaint( CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height )
		{
			//_logger.Debug("Type: {0} Buffer: {1:X8} Width: {2} Height: {3}", type, buffer, width, height);
			//foreach (var rect in dirtyRects)
			//{
			//    _logger.Debug("   DirtyRect: X={0} Y={1} W={2} H={3}", rect.X, rect.Y, rect.Width, rect.Height);
			//}

			owner.HandlePaint( browser, type, dirtyRects, buffer, width, height );
		}

		protected override void OnCursorChange( CefBrowser browser, IntPtr cursorHandle )
		{
			owner.HandleCursorChange( cursorHandle );
		}

		protected override void OnScrollOffsetChanged( CefBrowser browser )
		{
		}
	}
}
