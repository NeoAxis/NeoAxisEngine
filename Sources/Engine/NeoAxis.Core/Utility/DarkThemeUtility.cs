// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using NeoAxis.Editor;

namespace NeoAxis
{
	public static class DarkThemeUtility
	{
		class DarkToolStripRenderer : ToolStripRenderer
		{
			protected override void OnRenderItemBackground( ToolStripItemRenderEventArgs e )
			{
				//var brush = new SolidBrush( Color.Red );// e.Item.Bounds, Color.DarkBlue, Color.DarkGreen, 90 );
				//e.Graphics.FillRectangle( brush, 0, 0, e.Item.Width, e.Item.Height );
				//return;

				base.OnRenderItemBackground( e );
			}

			protected override void OnRenderButtonBackground( ToolStripItemRenderEventArgs e )
			{
				base.OnRenderButtonBackground( e );

				var item = e.Item as ToolStripButton;

				bool draw = false;
				Color color = Color.Black;

				if( item.Selected )
				{
					draw = true;
					color = Color.FromArgb( 90, 90, 90 );
				}

				if( item.Checked )
				{
					draw = true;
					color = Color.FromArgb( 90, 90, 90 );
				}

				if( item.Pressed )
				{
					draw = true;
					color = Color.FromArgb( 110, 110, 110 );
				}

				if( draw )
				{
#if !PROJECT_DEPLOY
					using( var brush = new SolidBrush( color ) )
					{
						var bounds = new System.Drawing.Rectangle( Point.Empty, item.Size );
						e.Graphics.FillRectangle( brush, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1 );
					}
#endif
				}
			}

			protected override void OnRenderDropDownButtonBackground( ToolStripItemRenderEventArgs e )
			{
				base.OnRenderDropDownButtonBackground( e );

				var item = e.Item as ToolStripDropDownButton;

				bool draw = false;
				Color color = Color.Black;

				if( item.Selected )
				{
					draw = true;
					color = Color.FromArgb( 90, 90, 90 );
				}

				//if( item.Checked )
				//{
				//	draw = true;
				//	color = Color.FromArgb( 90, 90, 90 );
				//}

				if( item.Pressed )
				{
					draw = true;
					color = Color.FromArgb( 110, 110, 110 );
				}

				if( draw )
				{
#if !PROJECT_DEPLOY
					using( var brush = new SolidBrush( color ) )
					{
						var bounds = new System.Drawing.Rectangle( Point.Empty, item.Size );
						e.Graphics.FillRectangle( brush, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1 );
					}
#endif
				}
			}

			protected override void OnRenderArrow( ToolStripArrowRenderEventArgs e )
			{
				if( EditorAPI.DarkTheme )
					e.ArrowColor = Color.FromArgb( 140, 140, 140 );

				base.OnRenderArrow( e );
			}

			protected override void OnRenderItemImage( ToolStripItemImageRenderEventArgs e )
			{
				System.Drawing.Rectangle imageRect = e.ImageRectangle;
				Image image = e.Image;

				if( imageRect != System.Drawing.Rectangle.Empty && image != null )
				{
					bool disposeImage = false;
					if( !e.Item.Enabled )
					{
						image = CreateDisabledImage( image );//, e.ImageAttributes );
						disposeImage = true;
					}

#if !PROJECT_DEPLOY
					if( e.Item.ImageScaling == ToolStripItemImageScaling.None )
						e.Graphics.DrawImage( image, imageRect, new System.Drawing.Rectangle( Point.Empty, imageRect.Size ), GraphicsUnit.Pixel );
					else
						e.Graphics.DrawImage( image, imageRect );
#endif

					if( disposeImage )
						image.Dispose();
				}
			}

			void DrawSeparator( Graphics g, ToolStripItem item, System.Drawing.Rectangle bounds, bool vertical )
			{
#if !PROJECT_DEPLOY
				using( Pen pen = new Pen( Color.FromArgb( 30, 30, 30 ) ) )
				{
					var bounds2 = bounds;
					bounds2.Y += 2;
					bounds2.Height = Math.Max( 0, bounds2.Height - 4 );

					int startX = bounds2.Width / 2;
					g.DrawLine( pen, startX, bounds2.Top, startX, bounds2.Bottom - 1 );
				}
#endif
			}

			protected override void OnRenderSeparator( ToolStripSeparatorRenderEventArgs e )
			{
				DrawSeparator( e.Graphics, e.Item, new System.Drawing.Rectangle( Point.Empty, e.Item.Size ), e.Vertical );
			}

			protected override void OnRenderItemText( ToolStripItemTextRenderEventArgs e )
			{
				//e.TextColor = Color.Red;
				//e.TextFont = new Font( "Helvetica", 7, FontStyle.Bold );

				base.OnRenderItemText( e );
			}
		}

		/////////////////////////////////////////

		public static void ApplyToForm( Control control )
		{
			if( EditorAPI.DarkTheme )
			{
				control.BackColor = Color.FromArgb( 54, 54, 54 );

				foreach( var child in control.Controls )
				{
					//KryptonLabel
					{
						var label = child as KryptonLabel;
						if( label != null )
						{
							label.StateCommon.ShortText.Color1 = Color.FromArgb( 230, 230, 230 );
							label.StateDisabled.ShortText.Color1 = Color.FromArgb( 90, 90, 90 );
						}
					}

					//Label
					{
						var label = child as Label;
						if( label != null )
							label.ForeColor = Color.FromArgb( 230, 230, 230 );
					}

					//LabelEx
					{
						var label = child as EngineLabel;
						if( label != null )
						{
							label.ForeColor = Color.FromArgb( 230, 230, 230 );
							label.StateCommon.Back.Color1 = Color.FromArgb( 54, 54, 54 );
						}
					}

					////KryptonSplitContainer
					//{
					//	var container = child as KryptonSplitContainer;
					//	if( container != null )
					//		container.StateNormal.Back.Color1 = Color.FromArgb( 90, 90, 90 );// 40, 40, 40 );
					//}



				}
			}
		}

		//public static void ApplyToSplitter( KryptonSplitContainer control )
		//{
		//	if( EditorAPI.DarkTheme )
		//		control.StateNormal.Back.Color1 = Color.FromArgb( 90, 90, 90 );// 40, 40, 40 );
		//}

		static void ToolTip1_Draw( object sender, DrawToolTipEventArgs e )
		{
			e.DrawBackground();
			e.DrawBorder();
			e.DrawText( TextFormatFlags.HidePrefix | TextFormatFlags.Left | TextFormatFlags.VerticalCenter );
		}

		public static void ApplyToToolTip( ToolTip control )
		{
			if( EditorAPI.DarkTheme && !control.OwnerDraw )
			{
				control.OwnerDraw = true;
				control.BackColor = Color.FromArgb( 20, 20, 20 );
				//control.BackColor = Color.FromArgb( 54, 54, 54 );
				control.ForeColor = Color.FromArgb( 230, 230, 230 );
				control.Draw += ToolTip1_Draw;
			}
		}

		//public static bool Enabled
		//{
		//	get { return EditorAPI.DarkTheme; }
		//}

		public static ToolStripRenderer GetToolbarToolStripRenderer()
		{
			return new DarkToolStripRenderer();
		}
	}
}
