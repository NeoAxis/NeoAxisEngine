// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Linq;
using System.Drawing;

namespace NeoAxis.Editor
{
	public class ContentBrowserListModeList : EngineListView.ModeClass
	{
		ContentBrowser browser;
		int imageSize;
		public int MarginLeft;
		public int MarginImageText;
		public int MarginRight;

		public ContentBrowserListModeList( ContentBrowser browser )
			: base( browser.GetListView() )
		{
			this.browser = browser;
			imageSize = (int)( (float)browser.Options.ListImageSize * EditorAPI.DPIScale );

			ItemSize = new Vector2I( (int)( (float)browser.Options.ListColumnWidth * EditorAPI.DPIScale ), imageSize + (int)( EditorAPI.DPIScale * 10.0f ) );
			ClampItemWidthByListViewWidth = true;

			MarginLeft = (int)( EditorAPI.DPIScale * 4.0f );
			MarginImageText = (int)( EditorAPI.DPIScale * 2.0f );
			MarginRight = (int)( EditorAPI.DPIScale * 2.0f );
		}

		public override void PaintItem( PaintEventArgs e, int itemIndex )
		{
			var item = Owner.Items[ itemIndex ];
			if( !GetItemRectangle( itemIndex, out var rect ) )
				return;
			GetItemColors( itemIndex, out var backColor, out var textColor );

			//background
			using( Brush brush = new SolidBrush( backColor ) )
				e.Graphics.FillRectangle( brush, rect );


			int offset = MarginLeft;

			//image
			//if( DisplayImages )
			{
				var image = item.Image;
				if( image == null )
				{
					if( imageSize >= 32 )
						image = Properties.Resources.Default_32;
					else
						image = Properties.Resources.Default_16;
				}

				e.Graphics.DrawImage( image, rect.Left + offset, rect.Top + ( ItemSize.Y - imageSize ) / 2, imageSize, imageSize );

				offset += imageSize + MarginImageText;
			}

			var normalTextFormatFlags = TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsTranslateTransform;

			int fontHeight;
			{
				TextFormatFlags flags = normalTextFormatFlags;

				var size = TextRenderer.MeasureText( e.Graphics, item.Text, Owner.Font, new Size( int.MaxValue, int.MaxValue ), flags );
				fontHeight = size.Height;
			}

			bool twoLines = fontHeight * 2 + (int)( EditorAPI.DPIScale * 6.0f ) < rect.Height;

			if( twoLines )
			{
				//text
				if( !string.IsNullOrEmpty( item.Text ) )
				{
					var flags = normalTextFormatFlags | TextFormatFlags.Top | TextFormatFlags.Left;
					var offsetY = rect.Height / 2 - (int)( EditorAPI.DPIScale * 1.0f ) - fontHeight;

					var rect2 = new System.Drawing.Rectangle( rect.Left + offset, rect.Top + offsetY, rect.Width - offset - MarginRight, rect.Height - offsetY );
					TextRenderer.DrawText( e.Graphics, item.Text, Owner.Font, rect2, textColor, backColor, flags );
				}

				//description
				if( !string.IsNullOrEmpty( item.Description ) )
				{
					var textColor2 = Color.Gray;
					var flags = normalTextFormatFlags | TextFormatFlags.Top | TextFormatFlags.Left;
					var offsetY = rect.Height / 2 + (int)( EditorAPI.DPIScale * 1.0f );

					var rect2 = new System.Drawing.Rectangle( rect.Left + offset, rect.Top + offsetY, rect.Width - offset - MarginRight, rect.Height - offsetY );
					TextRenderer.DrawText( e.Graphics, item.Description, Owner.Font, rect2, textColor2, backColor, flags );
				}
			}
			else
			{
				//text
				if( !string.IsNullOrEmpty( item.Text ) )
				{
					var flags = normalTextFormatFlags | TextFormatFlags.VerticalCenter | TextFormatFlags.Left;

					var rect2 = new System.Drawing.Rectangle( rect.Left + offset, rect.Top, rect.Width - offset - MarginRight, rect.Height );
					TextRenderer.DrawText( e.Graphics, item.Text, Owner.Font, rect2, textColor, backColor, flags );

					var size = TextRenderer.MeasureText( e.Graphics, item.Text, Owner.Font, new Size( int.MaxValue, int.MaxValue ), flags );

					offset += size.Width + (int)( EditorAPI.DPIScale * 2.0f );
				}

				//description
				if( !string.IsNullOrEmpty( item.Description ) )
				{
					var textColor2 = Color.Gray;

					var flags = normalTextFormatFlags | TextFormatFlags.VerticalCenter | TextFormatFlags.Left;

					var rect2 = new System.Drawing.Rectangle( rect.Left + offset, rect.Top, rect.Width - offset - MarginRight, rect.Height );
					TextRenderer.DrawText( e.Graphics, item.Description, Owner.Font, rect2, textColor2, backColor, flags );
				}
			}

		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ContentBrowserListModeTiles : EngineListView.ModeClass
	{
		ContentBrowser browser;
		int imageSize;

		public ContentBrowserListModeTiles( ContentBrowser browser )
			: base( browser.GetListView() )
		{
			this.browser = browser;
			imageSize = (int)( (float)browser.Options.TileImageSize * EditorAPI.DPIScale );

			// image size + side padding + two lines of text for height
			var tilePadding = ( EditorAPI.DPIScale * new Vector2( 30, 40 ) ).ToVector2I();

			ItemSize = new Vector2I( imageSize, imageSize ) + tilePadding;
			ClampItemWidthByListViewWidth = false;
		}

		public override void PaintItem( PaintEventArgs e, int itemIndex )
		{
			var item = Owner.Items[ itemIndex ];
			if( !GetItemRectangle( itemIndex, out var rect ) )
				return;
			GetItemColors( itemIndex, out var backColor, out var textColor );

			//background
			using( Brush brush = new SolidBrush( backColor ) )
				e.Graphics.FillRectangle( brush, rect );


			//total 30 logical pixels for height margins

			//image
			{
				var image = item.Image;
				if( image == null )
					image = Properties.Resources.Default_32;

				e.Graphics.DrawImage( image, ( rect.Left + rect.Right ) / 2 - imageSize / 2, rect.Top + (int)( EditorAPI.DPIScale * 5.0f ), imageSize, imageSize );
			}

			//text
			if( !string.IsNullOrEmpty( item.Text ) )
			{
				var normalTextFormatFlags = TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsTranslateTransform;

				//text alignment
				var flags = normalTextFormatFlags | TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
				//word wrap
				flags |= TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

				var topOffset = (int)( EditorAPI.DPIScale * 5.0f ) + imageSize + (int)( EditorAPI.DPIScale * 5.0f );

				var rect2 = new System.Drawing.Rectangle(
					rect.Left + (int)( EditorAPI.DPIScale * 3.0f ),
					rect.Top + topOffset,
					rect.Width - (int)( EditorAPI.DPIScale * 6.0f ),
					rect.Height - topOffset - 1/*- (int)( EditorAPI.DPIScale * 6.0f )*/ );

				TextRenderer.DrawText( e.Graphics, item.Text, Owner.Font, rect2, textColor, backColor, flags );
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ContentBrowserListModeTilesRibbon : EngineListView.ModeClass
	{
		ContentBrowser browser;
		int imageSize;

		public ContentBrowserListModeTilesRibbon( ContentBrowser browser )
			: base( browser.GetListView() )
		{
			this.browser = browser;
			imageSize = (int)( (float)browser.Options.TileImageSize * EditorAPI.DPIScale );

			ItemSize = ( EditorAPI.DPIScale * new Vector2( 60, 41 ) ).ToVector2I();
			ClampItemWidthByListViewWidth = false;
		}

		public override void PaintItem( PaintEventArgs e, int itemIndex )
		{
			var item = Owner.Items[ itemIndex ];
			if( !GetItemRectangle( itemIndex, out var rect ) )
				return;
			GetItemColors( itemIndex, out var backColor, out var textColor );

			//background
			using( Brush brush = new SolidBrush( backColor ) )
				e.Graphics.FillRectangle( brush, rect );

			//image
			{
				var image = item.Image;
				if( image == null )
					image = Properties.Resources.Default_32;

				e.Graphics.DrawImage( image, ( rect.Left + rect.Right ) / 2 - imageSize / 2, rect.Top + (int)( EditorAPI.DPIScale * 2.0f ), imageSize, imageSize );
			}

			//text
			if( !string.IsNullOrEmpty( item.Text ) )
			{
				var normalTextFormatFlags = TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsTranslateTransform;

				//text alignment
				var flags = normalTextFormatFlags | TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
				//word wrap
				flags |= TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

				var topOffset = (int)( EditorAPI.DPIScale * 2.0f ) + imageSize + (int)( EditorAPI.DPIScale * 1.0f );

				var rect2 = new System.Drawing.Rectangle(
					rect.Left + (int)( EditorAPI.DPIScale * 2 ),
					rect.Top + topOffset,
					rect.Width - (int)( EditorAPI.DPIScale * 4 ),
					rect.Height - topOffset - 1 );

				TextRenderer.DrawText( e.Graphics, item.Text, Owner.Font, rect2, textColor, backColor, flags );
			}
		}
	}

}
