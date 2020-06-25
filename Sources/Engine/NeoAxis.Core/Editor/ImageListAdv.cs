using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	//TODO: do rewrite, to provide license purity

	/// <summary>
	/// ImageList with support for images of different sizes and transparency.
	/// </summary>
	[Description( "Provides support for hosting images of different sizes and transparency." )]
	[ToolboxItem( true )]
	public class ImageListAdv : System.ComponentModel.Component, ICloneable
	{
		/// <summary>
		/// Default image size.
		/// </summary>
		private static readonly Size IMAGE_SIZE = new Size( 16, 16 );

		/// <summary>
		/// Collection of images.
		/// </summary>
		private ImageCollection images;

		/// <summary>
		/// Size of images. Used in drawing and for compatibility reasons.
		/// </summary>
		private Size imageSize = IMAGE_SIZE;

		/// <summary>
		/// Indicates whether images should be drawn using ImageSize property.
		/// </summary>
		private bool useImageSize = true;

		/// <summary>
		/// Tag object.
		/// </summary>
		private object tag;

		/// <summary>
		/// Gets collection of images.
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[Description( "Collection of images." )]
		public ImageCollection Images
		{
			get
			{
				return images;
			}
		}

		/// <summary>
		/// Gets or sets size of images. Used in drawing.
		/// </summary>
		[Description( "Size of images. Used in drawing." )]
		public Size ImageSize
		{
			get
			{
				return imageSize;
			}
			set
			{
				imageSize = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicating whether images should be drawn using ImageSize property.
		/// </summary>
		[DefaultValue( true )]
		[Description( "Indicates whether images should be drawn using ImageSize property." )]
		public bool UseImageSize
		{
			get
			{
				return useImageSize;
			}
			set
			{
				useImageSize = value;
			}
		}

		/// <summary>
		/// Gets or sets tag object.
		/// </summary>
		[DefaultValue( null )]
		[Description( "Tag object." )]
		public object Tag
		{
			get
			{
				return tag;
			}
			set
			{
				tag = value;
			}
		}

		/// <summary>
		/// Creates and initializes new ImageListAdv.
		/// </summary>
		public ImageListAdv()
		{
			images = new ImageCollection();
		}

		/// <summary>
		/// Creates and initializes new ImageListAdv.
		/// </summary>
		/// <param name="container">Container to add component to.</param>
		public ImageListAdv( IContainer container )
			: this()
		{
			container.Add( this );
		}

		/// <summary>
		/// Draws selected image to specified Graphics. If UseImageSize property is set to true, image is drawn using ImageSize property;
		/// otherwise it's drawn using original size.
		/// </summary>
		/// <param name="g">Graphics to draw to.</param>
		/// <param name="pt">Point to draw image at.</param>
		/// <param name="index">Index of image to draw.</param>
		public void Draw( Graphics g, Point pt, int index )
		{
			Draw( g, pt.X, pt.Y, index );
		}

		/// <summary>
		/// Draws selected image to specified Graphics. If UseImageSize property is set to true, image is drawn using ImageSize property;
		/// otherwise it's drawn using original size.
		/// </summary>
		/// <param name="g">Graphics to draw to.</param>
		/// <param name="x">X coordinate of point to draw image at.</param>
		/// <param name="y">Y coordinate of point to draw image at.</param>
		/// <param name="index">Index of image to draw.</param>
		public void Draw( Graphics g, int x, int y, int index )
		{
			if( index < 0 || index > images.Count - 1 )
				throw new ArgumentOutOfRangeException( "index" );

			if( useImageSize )
				Draw( g, x, y, imageSize.Width, imageSize.Height, index );
			else
				g.DrawImage( images[index], x, y );
		}

		/// <summary>
		/// Draws selected image to specified Graphics using given size.
		/// </summary>
		/// <param name="g">Graphics to draw to.</param>
		/// <param name="x">X coordinate of point to draw image at.</param>
		/// <param name="y">Y coordinate of point to draw image at.</param>
		/// <param name="width">Width of rectangle to draw image to.</param>
		/// <param name="height">Height of rectangle to draw image to.</param>
		/// <param name="index">Index of image to draw.</param>
		public void Draw( Graphics g, int x, int y, int width, int height, int index )
		{
			if( index < 0 || index > images.Count - 1 )
				throw new ArgumentOutOfRangeException( "index" );
			g.DrawImage( images[index], x, y, width, height );
		}

		/// <summary>
		/// Explicitly converts ImageList to ImageListAdv.
		/// </summary>
		/// <param name="list">ImageList to convert.</param>
		/// <returns>ImageListAdv with images from given ImageList.</returns>
		public static explicit operator ImageListAdv( ImageList list )
		{
			ImageListAdv imageListAdv = new ImageListAdv();
			imageListAdv.ImageSize = list.ImageSize;
			Image[] array = new Image[list.Images.Count];
			int i = 0;
			for( int count = list.Images.Count; i < count; i++ )
				array[i] = list.Images[i];
			imageListAdv.Images.AddRange( array );
			return imageListAdv;
		}

		/// <summary>
		/// Explicitly converts ImageListAdv to ImageList.
		/// </summary>
		/// <param name="list">ImageListAdv to convert.</param>
		/// <returns>ImageList with images from given ImageListAdv.</returns>
		public static explicit operator ImageList( ImageListAdv list )
		{
			ImageList imageList = new ImageList();
			imageList.ImageSize = list.ImageSize;
			foreach( Image image in list.Images )
				imageList.Images.Add( image );
			return imageList;
		}

		/// <summary>
		/// Converts ImageListAdv to ImageList.
		/// </summary>
		/// <returns>ImageList with images from ImageListAdv.</returns>
		public ImageList ToImageList()
		{
			return (ImageList)this;
		}

		/// <summary>
		/// creates ImageListAdv from ImageList.
		/// </summary>
		/// <param name="list">ImageList to create ImageListAdv from.</param>
		/// <returns>Created ImageListAdv.</returns>
		public static ImageListAdv FromImageList( ImageList list )
		{
			return (ImageListAdv)list;
		}

		/// <summary>
		/// creates resized ImageListAdv based on source ImageListAdv
		/// </summary>
		/// <param name="source"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static ImageListAdv MakeResizedImageList( ImageListAdv source, int width, int height )
		{
			if( width <= 0 || height <= 0 )
				throw new ArgumentException( "Size must be positive." );

			ImageListAdv il = new ImageListAdv();
			il.ImageSize = new Size( width, height );

			if( source == null )
				return il;

#if !ANDROID
			for( int i = 0; i < source.Images.Count; i++ )
				il.Images.Add( new Bitmap( source.Images[i], new Size( width, height ) ) );
#endif //!ANDROID

			foreach( var key in source.Images.Keys )
				il.Images.SetKeyName( source.Images.IndexOfKey( key ), key );

			return il;
		}

		/// <summary>
		/// Converts Icon to Image with correction of alpha channel.
		/// </summary>
		/// <param name="icon">Icon to convert.</param>
		/// <returns>Resulting Image.</returns>
		internal static Image IconToImageAlphaCorrect( Icon icon )
		{
			if( icon == null )
				throw new ArgumentNullException( "icon" );
			return icon.ToBitmap();
		}

		/// <summary>
		/// Checks whether Bitmap has	alpha channel.
		/// </summary>
		/// <param name="bmpData">BitmapData to check.</param>
		/// <returns>True if bitmap has alpha channel; otherwise false.</returns>
		private static bool BitmapHasAlpha( BitmapData bmpData )
		{
			for( int i = 0; i < bmpData.Height; i++ )
			{
				for( int j = 3; j < Math.Abs( bmpData.Stride ); j += 4 )
				{
					if( Marshal.ReadByte( (IntPtr)( bmpData.Scan0.ToInt64() + i * bmpData.Stride + j ) ) != 0 )
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Creates string with info about ImageListAdv.
		/// </summary>
		/// <returns>String with info about ImageListAdv</returns>
		public override string ToString()
		{
			return base.ToString() + " Images.Count: " + Images.Count.ToString();
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeImageSize()
		{
			return imageSize != IMAGE_SIZE;
		}

		/// <summary>
		///
		/// </summary>
		protected void ResetImageSize()
		{
			imageSize = IMAGE_SIZE;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			ImageListAdv imageListAdv = new ImageListAdv();
			imageListAdv.ImageSize = ImageSize;
			imageListAdv.UseImageSize = UseImageSize;
			for( int i = 0; i < Images.Count; i++ )
			{
				if( Images[i] != null )
					imageListAdv.Images.Add( (Image)Images[i].Clone() );
			}
			return imageListAdv;
		}
	}
}