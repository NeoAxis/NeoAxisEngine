// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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
		//IUIWebBrowser browser;
		//UIControl backstage;

		List<TipItem> tips;
		[EngineConfig( "TipsWindow", "currentTip" )]
		public static int currentTip;

		bool initialized;

		//bool waitingFirstTick = true;
		//double waitingStartTime;

		//bool firstLoading = true;
		//bool firstWasLoaded;
		//int backstageCounter;

		///////////////////////////////////////////////

		public class TipItem
		{
			public string Title;
			public string Text;
			public string Image;
		}

		///////////////////////////////////////////////

		public TipsWindow()
		{
			EngineConfig.RegisterClassParameters( typeof( TipsWindow ) );

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

			tips = GetTips();
			if( currentTip >= tips.Count )
				currentTip = tips.Count - 1;
			ShowTip( currentTip );
			//ShowTip( 0 );

			initialized = true;
		}

		protected override void OnDestroy()
		{
			//unload textures
			foreach( var resource in ResourceManager.GetAllResources() )
			{
				if( resource.Name.StartsWith( @"Base\Tools\Tips\" ) )
					resource.Dispose();
			}

			base.OnDestroy();
		}

		void ShowTip( int tipIndex )
		{
			if( tips.Count == 0 )
				tipNumberLabel.Text = "0/0";

			if( tipIndex >= 0 && tipIndex < tips.Count )
			{
				currentTip = tipIndex;
				var tip = tips[ tipIndex ];

				//if( browser != null )
				//	browser.StartFile = tip;

				tipNumberLabel.Text = $"{tipIndex + 1}/{tips.Count}";
			}
		}

		List<TipItem> GetTips()
		{
			var result = new List<TipItem>();

			var language = EditorLocalization2.Initialized ? EditorLocalization2.Language : "English";

			var virtualFilePath = $@"Base\Tools\Tips\{language}.block";
			if( !VirtualFile.Exists( virtualFilePath ) )
				virtualFilePath = @"Base\Tools\Tips\English.block";

			if( VirtualFile.Exists( virtualFilePath ) )
			{
				try
				{
					var rootBlock = TextBlockUtility.LoadFromVirtualFile( virtualFilePath );
					if( rootBlock != null )
					{
						foreach( var child in rootBlock.Children )
						{
							if( child.Name == "Tip" )
							{
								var tip = new TipItem();
								tip.Title = child.GetAttribute( "Title" );
								tip.Text = child.GetAttribute( "Text" );
								tip.Image = child.GetAttribute( "Image" );
								result.Add( tip );
							}
						}
					}
				}
				catch( Exception e )
				{
					Log.Warning( "TipsWindow: GetTips exception: " + e.Message );
				}
			}

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

			ShowTip( currentTip );

			//browser = (IUIWebBrowser)uiContainer.CreateComponent( MetadataManager.GetType( "NeoAxis.UIWebBrowser" ), enabled: false );
			////browser = uiContainer.CreateComponent<UIWebBrowser>( enabled: false );
			//browser.IUIWebBrowser_LoadStart += Browser_LoadStart;
			//browser.IUIWebBrowser_LoadEnd += Browser_LoadEnd;
			//ShowTip( currentTip );
			//( (UIControl)browser ).Enabled = true;

			//backstage = uiContainer.CreateComponent<UIControl>( enabled: false );
			//backstage.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, Rectangle.Zero );
			//backstage.Size = new UIMeasureValueVector2( UIMeasure.Screen, Vector2.One );
			//backstage.BackgroundColor = new ColorValue( 54.0 / 255.0, 54.0 / 255.0, 54.0 / 255.0 );
			//backstage.Enabled = true;

			//browser.AddressChanged += Browser_AddressChanged;
			//browser.TargetUrlChanged += Browser_TargetUrlChanged;
		}

		//private void Browser_LoadStart( IUIWebBrowser sender, object/*Internal.Xilium.CefGlue.CefFrame*/ cefFrame )
		//{
		//	if( firstLoading )
		//		firstWasLoaded = false;
		//}

		//private void Browser_LoadEnd( IUIWebBrowser sender, object/*Internal.Xilium.CefGlue.CefFrame*/ cefFrame, int httpStatusCode )
		//{
		//	if( firstLoading )
		//	{
		//		firstWasLoaded = true;
		//		backstageCounter = 10;

		//		firstLoading = false;
		//	}
		//}

		private void UiContainer_AfterRenderUIWithChildren( UIControl sender, CanvasRenderer renderer )
		{
			renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new ColorValue( 54.0 / 255.0, 54.0 / 255.0, 54.0 / 255.0 ) );

			if( currentTip >= 0 && currentTip < tips.Count )
			{
				var tip = tips[ currentTip ];

				var positionY = 0.0;

				//Title
				{
					var titleFontSize = 30.0 * EditorAPI2.DPIScale / renderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y;
					var titleResult = renderer.AddTextWordWrap( renderer.DefaultFont, titleFontSize, tip.Title, new Rectangle( 0, 0, 1, 1 ), EHorizontalAlignment.Center, false, EVerticalAlignment.Top, 0, new ColorValue( 1, 1, 1 ) );

					positionY += titleFontSize * ( titleResult.LinesCount + 1 );
				}

				//Text
				{
					var textFontSize = 20.0 * EditorAPI2.DPIScale / renderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y;
					var textResult = renderer.AddTextWordWrap( renderer.DefaultFont, textFontSize, tip.Text, new Rectangle( 0, positionY, 1, 1 ), EHorizontalAlignment.Center, false, EVerticalAlignment.Top, 0, new ColorValue( 1, 1, 1 ) );

					positionY += textFontSize * ( textResult.LinesCount + 2 );
				}

				//Image
				{
					var texture = ResourceManager.LoadResource<ImageComponent>( $@"Base\Tools\Tips\{tip.Image}" );
					if( texture?.Result != null )
					{
						var imageSize = texture.Result.SourceSize;
						var viewportSize = renderer.ViewportForScreenCanvasRenderer.SizeInPixels;

						var positionYInPixels = (int)( positionY * viewportSize.Y );

						var maxHeightInPixels = viewportSize.Y - positionYInPixels;
						if( maxHeightInPixels > imageSize.Y )
							maxHeightInPixels = imageSize.Y;
						//if( maxHeightInPixels > imageSize.Y * 2 )
						//	maxHeightInPixels = imageSize.Y * 2;

						var rectangleInPixels = new Rectangle( viewportSize.X / 2 - imageSize.X / 2, positionYInPixels, 0, positionYInPixels + maxHeightInPixels );
						rectangleInPixels.Right = rectangleInPixels.Left + imageSize.X;

						var rectangle = new Rectangle( rectangleInPixels.Left / viewportSize.X, rectangleInPixels.Top / viewportSize.Y, rectangleInPixels.Right / viewportSize.X, rectangleInPixels.Bottom / viewportSize.Y );

						renderer.PushTextureFilteringMode( CanvasRenderer.TextureFilteringMode.Point );
						renderer.AddQuad( rectangle, new RectangleF( 0, 0, 1, 1 ), texture, new ColorValue( 1, 1, 1 ), true );
						renderer.PopTextureFilteringMode();
					}
				}
			}


			//if( firstWasLoaded && backstageCounter > 0 )
			//	backstageCounter--;

			//if( !firstWasLoaded || backstageCounter != 0 )
			//	renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new ColorValue( 54.0 / 255.0, 54.0 / 255.0, 54.0 / 255.0 ) );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			//backstageCounter = 10;
			//waitingFirstTick = true;
		}

		protected override void Viewport_KeyDown( Viewport viewport, KeyEvent e, ref bool handled )
		{
			base.Viewport_KeyDown( viewport, e, ref handled );

			//add Copy of the tip text to the clipboard
			if( viewport.IsKeyPressed( EKeys.Control ) && e.Key == EKeys.C )
			{
				if( currentTip >= 0 && currentTip < tips.Count )
				{
					var tip = tips[ currentTip ];
					var text = $"{tip.Title}\n\n{tip.Text}";
					Clipboard.SetText( text );
				}
				handled = true;
			}
		}

		//protected override void OnKeyDown( KeyEventArgs e )
		//{
		//	base.OnKeyDown( e );

		//	//add Copy of the tip text to the clipboard
		//	if( e.Control && e.KeyCode == Keys.C )
		//	{
		//		if( currentTip >= 0 && currentTip < tips.Count )
		//		{
		//			var tip = tips[ currentTip ];
		//			var text = $"{tip.Title}\n\n{tip.Text}";
		//			Clipboard.SetText( text );
		//		}
		//		//e.Handled = true;
		//	}
		//}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			//if( waitingFirstTick )
			//	waitingStartTime = Time.Current;

			//var show = waitingFirstTick;// || Time.Current - waitingStartTime < 0;// 0.8;
			//if( backstage != null )
			//	backstage.Visible = show;

			//waitingFirstTick = false;

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