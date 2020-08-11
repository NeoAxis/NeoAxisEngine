// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using BrightIdeasSoftware;
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	internal static class OLVExtensions
	{
		internal static bool IsPrimary( this OLVColumn column )
		{
			return column.Index == 0;
		}

		internal static bool CheckWordWrap( this OLVColumn column )
		{
			return column.IsPrimary() ? column.WordWrap : true;
		}
	}

	/// <summary>
	///
	/// </summary>
	internal class ContentBrowserRendererList : ContentBrowserRendererBase
	{
		public Color DescriptionForeColor { get; set; } = Color.Gray;

		public ContentBrowserRendererList( ContentBrowserImageHelper imageHelper, int size )
			: base( imageHelper, size )
		{ }

#if !ANDROID

		public override void ConfigureItem( DrawListViewItemEventArgs e, System.Drawing.Rectangle itemBounds, object model )
		{
			base.ConfigureItem( e, itemBounds, model );

			// for Icons.
			ListView.CellVerticalAlignment = StringAlignment.Center;
			// for Text.
			TextAlignment = TextFormatFlags.Top;
		}

		public override Size GetTileSize( int imageSize, int tileColumnWidth, int tileWidthLimit )
		{
			int tileWidth = Math.Min( imageSize * 2 + tileColumnWidth, tileWidthLimit );
			return new Size( tileWidth, imageSize + DpiHelper.Default.ScaleValue( 10 ) );
		}

		public override bool RenderItem( DrawListViewItemEventArgs e, Graphics g, System.Drawing.Rectangle r, object model )
		{
			ConfigureItem( e, r, model );

			// Adjust the first columns rectangle to match the padding used by the native mode of the ListView
			if( ColumnIsPrimary && CellHorizontalAlignment == HorizontalAlignment.Left )
			{
				r.X += 3;
				r.Width -= 3;
			}

			DrawBackground( g, r );

			r = ApplyCellPadding( r );
			DrawImageAndText( g, r );

			if( DebugRender )
				g.DrawRectangle( Pens.Purple, r.X, r.Y, r.Width - 1, r.Height - 1 );

			return true;
		}

		protected override void DrawImageAndText( Graphics g, System.Drawing.Rectangle r )
		{
			int imageSize = DrawImage( g, r, GetImageSelector() );
			r.X += imageSize;
			r.Width -= imageSize;

			var sz = r.Size;
			sz.Height = Math.Min( imageSize + 2, sz.Height ); // limit text height by image height

			var tr = new System.Drawing.Rectangle( Point.Empty, GetAllTextSize( g, sz, out bool ellipsis ) );
			tr = AlignRectangle( r, tr );
			RenderAllText( g, tr );

			//!!!!
			// show tooltip if text ellipsed
			( (ContentBrowser.ListItem)RowObject ).ShowTooltip = ellipsis;

			if( DebugRender )
				g.DrawRectangle( Pens.Blue, tr.X, tr.Y, tr.Width - 1, tr.Height - 1 );
		}

		//Size GetTileTextSize( Graphics g, Size r )
		//{
		//	int height = 0;

		//	for( int i = 0; i < ListView.Columns.Count; i++ )
		//	{
		//		OLVColumn column = ListView.GetColumn( i );
		//		if( column.IsPrimary() || column.IsTileViewColumn )
		//		{
		//			string txt = column.GetStringValue( RowObject );
		//			if( string.IsNullOrEmpty( txt ) )
		//				break;
		//			Size size = CalculateTextSize( g, txt, r.Width, column.CheckWordWrap() );
		//			size.Height = Math.Min( size.Height, r.Height );
		//			if( height + size.Height > r.Height )
		//				break;
		//			height += size.Height + 2;
		//		}
		//	}

		//	return new Size( r.Width, height );
		//}

		Size GetAllTextSize( Graphics g, Size r, out bool ellipsis )
		{
			ellipsis = false;
			int totalHeight = 0;

			// title

			OLVColumn titleColumn = ListView.GetColumn( 0 );
			Debug.Assert( titleColumn.IsPrimary() );
			string titleText = titleColumn.GetStringValue( RowObject );
			bool wordWrap = r.Height < 20 ? false : titleColumn.CheckWordWrap();
			Size titleSize = CalculateTextSize( g, titleText, r.Width, wordWrap );
			titleSize.Height = Math.Min( titleSize.Height, r.Height );

			//!!!!new .NET Core
			totalHeight += titleSize.Height;
			//totalHeight += titleSize.Height + 2;

			// description

			if( ListView.Columns.Count > 1 )
			{
				OLVColumn descColumn = ListView.GetColumn( 1 );
				string descText = descColumn.GetStringValue( RowObject );
				if( !string.IsNullOrEmpty( descText ) )
				{
					Size descSize = CalculateTextSize( g, descText, r.Width, true );
					Size oneLineSize = CalculateTextSize( g, "Test", Int32.MaxValue, false ); // we can t use Font.Height
					for( int h = descSize.Height; h >= 0; h -= oneLineSize.Height )
					{
						if( totalHeight + h <= r.Height )
						{
							ellipsis = descSize.Height > ( h != 0 ? h : oneLineSize.Height );
							//descColumn.WordWrap = true;
							return new Size( r.Width, totalHeight + h );
						}
					}
				}
			}

			return new Size( r.Width, totalHeight );
		}

		void RenderAllText( Graphics g, System.Drawing.Rectangle r )
		{
			if( ListView.Columns.Count == 0 )
				return;

			// title

			OLVColumn titleColumn = ListView.GetColumn( 0 );
			Debug.Assert( titleColumn.IsPrimary() );
			string titleText = titleColumn.GetStringValue( RowObject );
			bool wordWrap = r.Height < 20 ? false : titleColumn.CheckWordWrap();
			Size titleSize = CalculateTextSize( g, titleText, r.Width, wordWrap );
			titleSize.Height = Math.Min( titleSize.Height, r.Height );
			DrawText( g, r, titleText, GetForegroundColor(), titleColumn.CheckWordWrap() );

			// description

			if( ListView.Columns.Count > 1 )
			{
				OLVColumn descColumn = ListView.GetColumn( 1 );
				string descText = descColumn.GetStringValue( RowObject );
				if( !string.IsNullOrEmpty( descText ) )
				{
					Size descSize = CalculateTextSize( g, descText, r.Width, descColumn.WordWrap );
					Size oneLineSize = CalculateTextSize( g, "Test", Int32.MaxValue, false );
					for( int h = descSize.Height; h >= 0; h -= oneLineSize.Height )
					{
						if( h > 0 && titleSize.Height + h <= r.Height )
						{
							var rect = r;
							rect.Y += titleSize.Height;
							rect.Height -= titleSize.Height;
							DrawText( g, rect, descText, GetDescriptionForegroundColor(), true /*descColumn.WordWrap*/ );
							return;
						}
					}

					// draw in one line with title
					var rect2 = r;
					rect2.X += titleSize.Width + 5;
					rect2.Width -= titleSize.Width + 5;
					DrawText( g, rect2, descText, GetDescriptionForegroundColor(), false );
				}
			}

		}

		//void RenderTileText( Graphics g, System.Drawing.Rectangle r )
		//{
		//	int height = 0;

		//	for( int i = 0; i < ListView.Columns.Count; i++ )
		//	{
		//		OLVColumn column = ListView.GetColumn( i );
		//		if( column.IsPrimary() || column.IsTileViewColumn )
		//		{
		//			string txt = column.GetStringValue( RowObject );
		//			//TODO: check for txt == ""
		//			Size size = CalculateTextSize( g, txt, r.Width, r.Height < 20 ? false : column.CheckWordWrap() );
		//			size.Height = Math.Min( size.Height, r.Height );
		//			if( height + size.Height > r.Height )
		//				break;

		//			var rect = r; rect.Y += height;
		//			var color = column.IsPrimary() ? GetForegroundColor() : GetDescriptionForegroundColor();
		//			DrawText( g, rect, txt, color, column.CheckWordWrap() );

		//			height += size.Height + 2;
		//		}
		//	}
		//}

		Color GetDescriptionForegroundColor()
		{
			if( IsItemSelected && !ListView.UseTranslucentSelection && ( ColumnIsPrimary || ListView.FullRowSelect ) )
				return GetSelectedForegroundColor();

			return DescriptionForeColor;
		}

#else //!ANDROID

		public override Size GetTileSize( int imageSize, int tileColumnWidth, int tileWidthLimit )
		{
			return Size.Empty;
		}

#endif //!ANDROID

	}
}