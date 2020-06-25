// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace NeoAxis.Editor
{
	//TODO: move all content browser classes to separate folder. like HierarchicalContainer
	//TODO: rename this class and childs ? ContentBrowserLVBaseRenderer or CBListViewBaseRenderer or ListViewBaseRenderer ?

	public /*internal */abstract class ContentBrowserRendererBase : BaseRenderer
	{
		internal bool DebugRender { get; set; }

		internal ContentBrowserImageHelper imageHelper;
		protected new ImageListAdv ImageList { get; set; }

		public TextFormatFlags TextAlignment { get; set; }

		internal ContentBrowserRendererBase( ContentBrowserImageHelper imageHelper, int size )
		{
			this.imageHelper = imageHelper;
			if( imageHelper != null )
				ImageList = imageHelper.ResizeImagesForListView( new Size( size, size ) );
		}

		public abstract Size GetTileSize( int imageSize, int tileColumnWidth, int tileWidthLimit );

		public void ResizeImageList( Size size )
		{
			// recreate imagelist. here we can ignore performance.
			if( imageHelper != null )
				ImageList = imageHelper.ResizeImagesForListView( size );
		}

#if !ANDROID

		protected override Size CalculateImageSize( Graphics g, object imageSelector )
		{
			if( imageSelector == null || imageSelector == DBNull.Value )
				return Size.Empty;

			// Check for the image in the image list (most common case)
			ImageListAdv il = ImageList;
			if( il != null )
			{
				if( imageSelector is string selectorAsString )
				{
					int selectorAsInt = il.Images.IndexOfKey( selectorAsString );
					if( selectorAsInt >= 0 )
						return il.ImageSize;
				}
			}

			// Is the selector actually an image?
			Image image = imageSelector as Image;
			if( image != null )
				return image.Size;

			return Size.Empty;
		}

		protected override int DrawImage( Graphics g, System.Drawing.Rectangle r, object imageSelector )
		{
			var image = GetImageFromList( imageSelector );
			if( image == null )
				return 0;

			if( image.Size.Width < r.Width )
				r.X = this.AlignHorizontally( r, new System.Drawing.Rectangle( Point.Empty, image.Size ) );

			if( image.Size.Height < r.Height )
				r.Y = this.AlignVertically( r, new System.Drawing.Rectangle( Point.Empty, image.Size ) );

			if( DebugRender )
				g.DrawRectangle( Pens.Red, r.X, r.Y, image.Width - 1, image.Height - 1 );

			if( this.ListItem.Enabled )//&& ListView.Enabled )
				g.DrawImageUnscaled( image, r.X, r.Y );
			else
				ControlPaint.DrawImageDisabled( g, image, r.X, r.Y, GetBackgroundColor() );

			return image.Width;
		}

		protected virtual void DrawText( Graphics g, System.Drawing.Rectangle r, string txt, Color foreColor, bool wordWrap )
		{
			EditorAssemblyInterface.Instance.ContentBrowserRendererBase_DrawText( this, g, r, txt, foreColor, wordWrap );

			//#if !W__!!__INDOWS_UWP
			//			if( string.IsNullOrEmpty( txt ) )
			//				return;

			//			Color backColor = Color.Transparent;
			//			if( this.IsDrawBackground && this.IsItemSelected && !this.ListView.FullRowSelect )
			//				backColor = this.GetSelectedBackgroundColor();

			//			TextFormatFlags flags = NormalTextFormatFlags | TextAlignment;

			//			if( wordWrap )
			//				flags |= TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

			//			TextRenderer.DrawText( g, txt, this.Font, r, foreColor, backColor, flags );
			//#endif
		}

		protected virtual Size CalculateTextSize( Graphics g, string txt, int width, bool wordWrap )
		{
			return EditorAssemblyInterface.Instance.ContentBrowserRendererBase_CalculateTextSize( this, g, txt, width, wordWrap );

			//#if !W__!!__INDOWS_UWP
			//			if( string.IsNullOrEmpty( txt ) )
			//				return Size.Empty;

			//			Size proposedSize = new Size( width, Int32.MaxValue );

			//			TextFormatFlags flags = NormalTextFormatFlags;

			//			if( wordWrap )
			//				flags |= TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

			//			return TextRenderer.MeasureText( g, txt, this.Font, proposedSize, flags );
			//#else
			//			return new Size( 1, 1 );
			//#endif
		}

		protected override void DrawBackground( Graphics g, System.Drawing.Rectangle r )
		{
			if( !IsDrawBackground )
				return;

			using( Brush brush = new SolidBrush( GetBackgroundColor() ) )
				g.FillRectangle( brush, r.X, r.Y, r.Width, r.Height );
		}

		Image GetImageFromList( object imageSelector )
		{
			if( imageSelector == null || imageSelector == DBNull.Value )
				return null;

			// Draw from the image list (most common case)
			ImageListAdv il = ImageList;
			if( il != null )
			{
				// Try to translate our imageSelector into a valid ImageList index
				int selectorAsInt = -1;
				if( imageSelector is int )
				{
					selectorAsInt = (int)imageSelector;
					if( selectorAsInt >= il.Images.Count )
						selectorAsInt = -1;
				}
				else
				{
					string selectorAsString = imageSelector as string;
					if( selectorAsString != null )
						selectorAsInt = il.Images.IndexOfKey( selectorAsString );
				}

				if( selectorAsInt >= 0 )
					imageSelector = il.Images[ selectorAsInt ];
			}

			// Is the selector actually an image?
			return imageSelector as Image;
		}

#endif //!ANDROID

		public override Color GetBackgroundColor()
		{
			if( EditorAPI.DarkTheme )
			{
				if( !this.ListView.Enabled )
					return Color.FromArgb( 40, 40, 40 );

				if( this.IsItemSelected && !this.ListView.UseTranslucentSelection && this.ListView.FullRowSelect )
					return Color.FromArgb( 70, 70, 70 );// this.GetSelectedBackgroundColor();

				if( this.SubItem == null || this.ListItem.UseItemStyleForSubItems )
					return Color.FromArgb( 40, 40, 40 ); //this.ListItem.BackColor;

				return Color.FromArgb( 40, 40, 40 );// this.SubItem.BackColor;
			}

			return base.GetBackgroundColor();
		}

		public override Color GetForegroundColor()
		{
			return base.GetForegroundColor();
		}

		public override Color GetSelectedBackgroundColor()
		{
			if( EditorAPI.DarkTheme )
				return Color.FromArgb( 70, 70, 70 );

			return base.GetSelectedBackgroundColor();
		}

		public override Color GetSelectedForegroundColor()
		{
			//if( EditorAPI.DarkTheme )
			//	return Color.Green;

			return base.GetSelectedForegroundColor();
		}
	}
}