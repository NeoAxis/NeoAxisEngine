#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Tips Window.
	/// </summary>
	public partial class TipsWindow : DocumentWindowWithViewport
	{
		UIWebBrowser browser;
		UIControl backstage;

		List<string> tips;
		int currentTip;

		bool initialized;

		bool waitingFirstTick = true;
		//double waitingStartTime;

		bool firstLoading = true;
		bool firstWasLoaded;
		int backstageCounter;

		//

		public TipsWindow()
		{
			InitializeComponent();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			WindowTitle = EditorLocalization2.Translate( "TipsWindow", WindowTitle );
			EditorLocalization2.TranslateForm( "TipsWindow", panel2 );

			BackColor = Color.FromArgb( 54, 54, 54 );
			if( EditorAPI2.DarkTheme )
				panel2.BackColor = BackColor;
			else
				panel2.BackColor = Color.FromArgb( 240, 240, 240 );//SystemColors.Control;

			kryptonCheckBoxShowTipsAtStartup.Checked = EditorSettingsSerialization.ShowTipsAsStartup;

			CloseByEscape = true;
		}

		private void TipsWindow_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			ViewportControl2.Dock = panel1.Dock;
			ViewportControl2.Anchor = panel1.Anchor;
			ViewportControl2.Location = panel1.Location;
			ViewportControl2.Size = panel1.Size;

			timer1.Start();

			tips = GetTipFiles();
			ShowTip( 0 );

			initialized = true;
		}

		void ShowTip( int tipIndex )
		{
			if( tipIndex < 0 || tipIndex >= tips.Count )
				return;

			currentTip = tipIndex;
			var tip = tips[ tipIndex ];

			if( browser != null )
				browser.StartFile = tip;

			tipNumberLabel.Text = $"{tipIndex + 1}/{tips.Count}";
		}

		List<string> GetTipFiles()
		{
			var result = new List<string>();

			var folder = Path.Combine( VirtualFileSystem.Directories.EngineInternal, "Tips" );

			try
			{
				var files = Directory.GetFiles( folder, "*.html" );

				for( int counter = 1; ; counter++ )
				{
					string foundPath = "";

					foreach( var fullPath in files )
					{
						var fileName = Path.GetFileName( fullPath );

						if( fileName.Length > 3 && fileName[ 2 ] == '_' )
						{
							var t = fileName.Substring( 0, 2 );
							if( int.TryParse( t, out var number ) )
							{
								if( number == counter )
								{
									foundPath = fullPath;
									break;
								}
							}
						}
					}

					if( string.IsNullOrEmpty( foundPath ) )
						break;

					if( EditorLocalization2.Initialized )
					{
						try
						{
							var d = Path.GetDirectoryName( foundPath );
							var f = Path.GetFileName( foundPath );
							var newPath = Path.Combine( d, EditorLocalization2.Language + "_" + f );
							if( File.Exists( newPath ) )
								foundPath = newPath;
						}
						catch { }
					}

					result.Add( foundPath );
				}
			}
			catch { }

			return result;
		}

		private void kryptonButtonClose_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void kryptonButtonNext_Click( object sender, EventArgs e )
		{
			int index = currentTip + 1;
			if( index >= tips.Count )
				index = 0;
			ShowTip( index );
		}

		private void kryptonButtonPrevious_Click( object sender, EventArgs e )
		{
			int index = currentTip - 1;
			if( index < 0 )
				index = tips.Count - 1;
			ShowTip( index );
		}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );

			var uiContainer = sender.Viewport.UIContainer;
			uiContainer.AfterRenderUIWithChildren += UiContainer_AfterRenderUIWithChildren;

			browser = uiContainer.CreateComponent<UIWebBrowser>( enabled: false );
			browser.LoadStart += Browser_LoadStart;
			browser.LoadEnd += Browser_LoadEnd;
			ShowTip( currentTip );
			browser.Enabled = true;

			backstage = uiContainer.CreateComponent<UIControl>( enabled: false );
			backstage.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, Rectangle.Zero );
			backstage.Size = new UIMeasureValueVector2( UIMeasure.Screen, Vector2.One );
			backstage.BackgroundColor = new ColorValue( 54.0 / 255.0, 54.0 / 255.0, 54.0 / 255.0 );
			backstage.Enabled = true;

			//browser.AddressChanged += Browser_AddressChanged;
			//browser.TargetUrlChanged += Browser_TargetUrlChanged;
		}

		private void Browser_LoadStart( UIWebBrowser sender, object/*Internal.Xilium.CefGlue.CefFrame*/ cefFrame )
		{
			if( firstLoading )
				firstWasLoaded = false;
		}

		private void Browser_LoadEnd( UIWebBrowser sender, object/*Internal.Xilium.CefGlue.CefFrame*/ cefFrame, int httpStatusCode )
		{
			if( firstLoading )
			{
				firstWasLoaded = true;
				backstageCounter = 10;

				firstLoading = false;
			}
		}

		private void UiContainer_AfterRenderUIWithChildren( UIControl sender, CanvasRenderer renderer )
		{
			if( firstWasLoaded && backstageCounter > 0 )
				backstageCounter--;

			if( !firstWasLoaded || backstageCounter != 0 )
				renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new ColorValue( 54.0 / 255.0, 54.0 / 255.0, 54.0 / 255.0 ) );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			backstageCounter = 10;
			waitingFirstTick = true;
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			//if( waitingFirstTick )
			//	waitingStartTime = Time.Current;

			var show = waitingFirstTick;// || Time.Current - waitingStartTime < 0;// 0.8;
			if( backstage != null )
				backstage.Visible = show;

			waitingFirstTick = false;

			viewport.UIContainer.PerformRenderUI( viewport.CanvasRenderer );
		}

		private void kryptonCheckBoxShowTipsAtStartup_CheckedChanged( object sender, EventArgs e )
		{
			if( initialized )
				EditorSettingsSerialization.ShowTipsAsStartup = kryptonCheckBoxShowTipsAtStartup.Checked;
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

		}
	}
}

#endif