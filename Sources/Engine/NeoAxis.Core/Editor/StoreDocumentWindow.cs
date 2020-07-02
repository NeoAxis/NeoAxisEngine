// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NeoAxis.Widget;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Store Window.
	/// </summary>
	public partial class StoreDocumentWindow : DocumentWindowWithViewport
	{
		const string homeURL = "https://www.neoaxis.com/store/contents";

		//UIControl storeControl;
		UIWebBrowser browser;

		double toolbarMustVisibleForTime;

		volatile bool addressWasChanged;

		volatile string needOpenPackage;

		//

		public StoreDocumentWindow()
		{
			InitializeComponent();

			DarkThemeUtility.ApplyToForm( panelToolbar );
		}

		private void StoreDocumentWindow_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			WindowTitle = EditorLocalization.Translate( "StoreDocumentWindow", WindowTitle );

			toolbarMustVisibleForTime = Time.Current;
			timer1.Start();

			UpdateControls();
		}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );

			var uiContainer = sender.Viewport.UIContainer;

			//storeControl = ResourceManager.LoadSeparateInstance<UIControl>( @"Store.ui", false, false );
			//if( storeControl != null )
			//{
			//	uiContainer.AddComponent( storeControl );
			//	storeControl.Enabled = true;
			//}

			browser = uiContainer.CreateComponent<UIWebBrowser>( enabled: false );
			browser.StartURL = homeURL;
			browser.Enabled = true;

			browser.AddressChanged += Browser_AddressChanged;
			browser.TargetUrlChanged += Browser_TargetUrlChanged;
			browser.DownloadBefore += Browser_DownloadBefore;
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			//!!!!
			viewport.UIContainer.PerformRenderUI( viewport.CanvasRenderer );

			//update cursor
			if( EditorAPI.SelectedDocumentWindow == this )
			{
				if( browser != null && browser.CurrentCursor != null )
					ViewportControl.OneFrameChangeCursor = browser.CurrentCursor;
			}
		}

		void UpdateToolBarVisibility()
		{
			var coordinates = panelToolbar.PointToClient( Cursor.Position );

			Rectangle r = new Rectangle( 0, 0, panelToolbar.Size.Width, 0 );
			var mustVisible = r.GetPointDistance( new Vector2( coordinates.X, coordinates.Y ) ) < 10;

			if( panelToolbar.Visible )
			{
				Rectangle r2 = new Rectangle( 0, 0, panelToolbar.Size.Width, panelToolbar.Size.Height );
				if( r2.Contains( new Vector2( coordinates.X, coordinates.Y ) ) )
					mustVisible = true;
			}

			if( kryptonTextBoxAddress.Focused )
				mustVisible = true;

			if( mustVisible )
				toolbarMustVisibleForTime = Time.Current;

			var visible = toolbarMustVisibleForTime > Time.Current - 2.0;

			if( panelToolbar.Visible != visible )
				panelToolbar.Visible = visible;
		}

		void UpdateControls()
		{
			kryptonButtonBack.Enabled = browser != null && browser.CanGoBack;
			kryptonButtonForward.Enabled = browser != null && browser.CanGoForward;

			if( addressWasChanged )
			{
				addressWasChanged = false;
				kryptonTextBoxAddress.Text = browser != null ? browser.TargetURL : "";
			}

			kryptonTextBoxAddress.Width = Width - kryptonTextBoxAddress.Location.X - 6;
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			UpdateToolBarVisibility();

			UpdateControls();

			//update cursor
			if( EditorAPI.SelectedDocumentWindow == this )
			{
				if( browser != null && browser.CurrentCursor != null )
					ViewportControl.Cursor = browser.CurrentCursor;
			}

			if( !string.IsNullOrEmpty( needOpenPackage ) )
			{
				EditorAPI.OpenPackages( needOpenPackage );
				needOpenPackage = null;
			}
		}

		private void kryptonButtonBack_Click( object sender, EventArgs e )
		{
			browser?.GoBack();
		}

		private void kryptonButtonForward_Click( object sender, EventArgs e )
		{
			browser?.GoForward();
		}

		private void kryptonButtonRefresh_Click( object sender, EventArgs e )
		{
			browser?.Reload();
		}

		private void kryptonTextBoxAddress_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Return )
				browser?.LoadURL( kryptonTextBoxAddress.Text );
		}

		private void Browser_AddressChanged( UIWebBrowser sender, string address )
		{
			addressWasChanged = true;
		}

		private void Browser_TargetUrlChanged( UIWebBrowser sender, string targetUrl )
		{
		}

		private void kryptonButtonHome_Click( object sender, EventArgs e )
		{
			browser.LoadURL( homeURL );
		}

		private void Browser_DownloadBefore( UIWebBrowser sender, Xilium.CefGlue.CefDownloadItem downloadItem, string suggestedName, Xilium.CefGlue.CefBeforeDownloadCallback callback )
		{
			try
			{
				if( Path.GetExtension( suggestedName ).ToLower() == ".neoaxispackage" )
				{
					var fileBase = Path.GetFileNameWithoutExtension( suggestedName );
					var strings = fileBase.Split( new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries );
					if( strings.Length >= 2 )
					{
						var packageName = strings[ 0 ].Replace( '_', ' ' );

						needOpenPackage = packageName;
						//EditorAPI.OpenPackages( packageName );
					}
				}
			}
			catch( Exception e )
			{
				Log.Warning( e.Message );
				return;
			}
		}

	}
}
