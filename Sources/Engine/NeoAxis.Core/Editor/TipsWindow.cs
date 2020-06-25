// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using NeoAxis.Widget;
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
		double waitingStartTime;

		//

		public TipsWindow()
		{
			InitializeComponent();

			if( EditorUtility.IsDesignerHosted( this ) )
				return;

			WindowTitle = EditorLocalization.Translate( "TipsWindow", WindowTitle );
			EditorLocalization.TranslateForm( "TipsWindow", this );

			BackColor = Color.FromArgb( 54, 54, 54 );
			if( EditorAPI.DarkTheme )
				panel2.BackColor = BackColor;
			else
				panel2.BackColor = Color.FromArgb( 240, 240, 240 );//SystemColors.Control;

			kryptonCheckBoxShowTipsAtStartup.Checked = EditorSettingsSerialization.ShowTipsAsStartup;
		}

		private void TipsWindow_Load( object sender, EventArgs e )
		{
			if( EditorUtility.IsDesignerHosted( this ) )
				return;

			ViewportControl.Dock = panel1.Dock;
			ViewportControl.Anchor = panel1.Anchor;
			ViewportControl.Location = panel1.Location;
			ViewportControl.Size = panel1.Size;

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

			browser = uiContainer.CreateComponent<UIWebBrowser>( enabled: false );
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

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			waitingFirstTick = true;
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			if( waitingFirstTick )
				waitingStartTime = Time.Current;

			var show = waitingFirstTick || Time.Current - waitingStartTime < 0.8;
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
			if( !IsHandleCreated || EditorUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

		}
	}
}
