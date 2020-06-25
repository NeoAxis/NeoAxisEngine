// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using BrightIdeasSoftware;
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	/// <summary>
	///
	/// </summary>
	internal class ContentBrowserRendererTilesRibbon : ContentBrowserRendererBase
	{
		// image size + side padding + two lines of text for height
		//!!!!
		internal static readonly Size TilePadding = DpiHelper.Default.ScaleValue( new Size( 60, 41 ) );//40 ) );
																									   //internal static readonly Size TilePadding = DpiHelper.Default.ScaleValue( new Size( 30, 18 ) );//40 ) );

		public ContentBrowserRendererTilesRibbon( ContentBrowserImageHelper imageHelper, int size )
			: base( imageHelper, size )
		{ }

#if !ANDROID

		public override void ConfigureItem( DrawListViewItemEventArgs e, System.Drawing.Rectangle itemBounds, object model )
		{
			base.ConfigureItem( e, itemBounds, model );

			// for Icons.
			ListView.CellVerticalAlignment = StringAlignment.Near;
			// for Text.
			TextAlignment = TextFormatFlags.HorizontalCenter;
		}

		// for Icons.
		public override HorizontalAlignment CellHorizontalAlignment
		{
			get { return HorizontalAlignment.Center; }
		}

		public override Size GetTileSize( int imageSize, int tileColumnWidth, int tileWidthLimit )
		{
			//!!!!
			return TilePadding;

			//var size = (int)( (float)imageSize * EditorAPI.DPIScale );
			//return new Size( size, size ) + TilePadding;

			//return new Size( imageSize, imageSize ) + TilePadding;
		}

		public override bool RenderItem( DrawListViewItemEventArgs e, Graphics g, System.Drawing.Rectangle r, object model )
		{
			ConfigureItem( e, r, model );

			DrawBackground( g, r );
			r = ApplyCellPadding( r );
			DrawImageAndText( g, ApplyCellPadding( r ) );

			if( DebugRender )
				g.DrawRectangle( Pens.Purple, r.X, r.Y, r.Width - 1, r.Height - 1 );

			return true;
		}

		protected override void DrawImageAndText( Graphics g, System.Drawing.Rectangle r )
		{
			const int ImageVertPadding = 1;// 6; //TODO: dpi scale?

			r.Y += ImageVertPadding;
			int imageSize = DrawImage( g, r, GetImageSelector() );
			r.Y += imageSize + ImageVertPadding;
			r.Height -= ( imageSize + ImageVertPadding * 2 );

			DrawText( g, r, GetText(), GetForegroundColor(), Column.WordWrap );

			if( DebugRender )
				g.DrawRectangle( Pens.Blue, r.X, r.Y, r.Width - 1, r.Height - 1 );
		}

#else //ANDROID

		public override Size GetTileSize( int imageSize, int tileColumnWidth, int tileWidthLimit )
		{
			return Size.Empty;
		}

#endif //!ANDROID

	}
}