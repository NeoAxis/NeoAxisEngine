#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using NeoAxis;
using System.ComponentModel;
using System.Drawing.Text;

namespace NeoAxis.Editor
{
	public partial class StoreItemPreview : PreviewControl//WithViewport
	{
		bool imageDisplayed;

		//Image texture;

		//

		public StoreItemPreview()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			//ViewportControl.Visible = false;

			//var scene = CreateScene( false );

			//var texture = ObjectOfPreview as Image;
			//if( texture?.Result?.TextureType == Image.TypeEnum.Cube )
			//{
			//	var type = MetadataManager.GetType( "NeoAxis.Skybox" );
			//	if( type != null )
			//	{
			//		var skybox = scene.CreateComponent( type );
			//		skybox.PropertySet( "Cubemap", texture );
			//		skybox.PropertySet( "AllowProcessEnvironmentCubemap", false );
			//	}
			//}

			//scene.Enabled = true;

			timer1.Start();
		}

		//protected override void OnDestroy()
		//{
		//	try
		//	{
		//		texture?.Dispose();
		//	}
		//	catch { }

		//	base.OnDestroy();
		//}

		StoresWindow.ContentBrowserItem_StoreItem GetStoreItem()
		{
			return ObjectOfPreview as StoresWindow.ContentBrowserItem_StoreItem;// PackageManager.PackageInfo;
		}

		Image GetSourceImage()
		{
			var storeItem = GetStoreItem();
			var package = storeItem.storesWindow.GetPackage( storeItem.packageId, false );

			if( package != null && !string.IsNullOrEmpty( package.Thumbnail ) )
				return StoreManager.ImageManager.GetSourceImage( package.Thumbnail, EngineApp.GetSystemTime() );

			return null;
		}

		//void GetTexture()
		//{
		//	if( texture == null )
		//	{
		//		var sourceImage = GetSourceImage();
		//		if( sourceImage != null )
		//		{
		//			using( Bitmap convertedImage = new Bitmap( sourceImage.Width, sourceImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb ) )
		//			{
		//				using( Graphics gr = Graphics.FromImage( convertedImage ) )
		//					gr.DrawImage( sourceImage, new System.Drawing.Rectangle( 0, 0, convertedImage.Width, convertedImage.Height ) );

		//				texture = ComponentUtility.CreateComponent<Image>( null, true, false );

		//				var converter = new ImageConverter();
		//				var bytes = (byte[])converter.ConvertTo( convertedImage, typeof( byte[] ) );

		//				bool mipmaps = false;

		//				texture.CreateType = Image.TypeEnum._2D;
		//				texture.CreateSize = new Vector2I( sourceImage.Width, sourceImage.Height );
		//				texture.CreateMipmaps = false;
		//				texture.CreateFormat = PixelFormat.A8R8G8B8;

		//				var usage = Image.Usages.WriteOnly;
		//				if( mipmaps )
		//					usage |= Image.Usages.AutoMipmaps;
		//				texture.CreateUsage = usage;

		//				texture.Enabled = true;

		//				GpuTexture gpuTexture = texture.Result;
		//				if( gpuTexture != null )
		//				{
		//					var d = new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( 0, 0, bytes ) };
		//					gpuTexture.SetData( d );
		//				}
		//			}
		//		}
		//	}
		//}

		//protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		//{
		//	base.Viewport_UpdateBeforeOutput( viewport );

		//	try
		//	{
		//		GetTexture();
		//	}
		//	catch { }

		//	//var texture = ObjectOfPreview as Image;
		//	//if( texture?.Result?.TextureType == Image.TypeEnum._2D )
		//	if( texture != null && !texture.Disposed )
		//	{
		//		double viewScale = 1;//0.95;

		//		double scale = Math.Min(
		//			(double)viewport.SizeInPixels.X / (double)texture.Result.ResultSize.X,
		//			(double)viewport.SizeInPixels.Y / (double)texture.Result.ResultSize.Y );
		//		Vector2 size = texture.Result.ResultSize.ToVector2() * scale * viewScale;
		//		Vector2 center = viewport.SizeInPixels.ToVector2() / 2;
		//		Rectangle rectInPixels = new Rectangle( center - size / 2, center + size / 2 );

		//		Rectangle rect = rectInPixels / viewport.SizeInPixels.ToVector2();

		//		var renderer = viewport.CanvasRenderer;

		//		var pointFiltering = false;
		//		if( rectInPixels.Size.X >= texture.Result.ResultSize.X && rectInPixels.Size.Y >= texture.Result.ResultSize.Y )
		//			pointFiltering = true;

		//		if( pointFiltering )
		//			renderer.PushTextureFilteringMode( CanvasRenderer.TextureFilteringMode.Point );
		//		renderer.AddQuad( rect, new Rectangle( 0, 0, 1, 1 ), texture );
		//		if( pointFiltering )
		//			renderer.PopTextureFilteringMode();
		//	}
		//}

		static string Translate( string text )
		{
			return EditorLocalization.Translate( "StoreItemPreviewControl", text );
		}

		//protected override void GetTextInfoLeftTopCorner( List<string> lines )
		//{
		//	//var texture = ObjectOfPreview as Image;
		//	//if( texture != null )
		//	//{
		//	//	var result = texture.Result;
		//	//	if( result != null )
		//	//	{
		//	//		lines.Add( Translate( "Source" ) + $": {result.SourceSize}, {TypeUtility.DisplayNameAddSpaces( result.SourceFormat.ToString() )}" );
		//	//		lines.Add( Translate( "Processed" ) + $": {result.ResultSize}, {TypeUtility.DisplayNameAddSpaces( result.ResultFormat.ToString() )}" );
		//	//		lines.Add( Translate( "Type" ) + $": " + TypeUtility.DisplayNameAddSpaces( result.TextureType.ToString() ) );
		//	//	}
		//	//	else
		//	//		lines.Add( Translate( "No data" ) );
		//	//}
		//}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			var image = GetSourceImage();
			if( image != null )
			{
				var viewportSize = new Vector2I( ClientSize.Width, ClientSize.Height );
				var imageSize = new Vector2I( image.Size.Width, image.Size.Height );

				double viewScale = 1;// 0.95;

				double scale = Math.Min(
					(double)viewportSize.X / (double)imageSize.X,
					(double)viewportSize.Y / (double)imageSize.Y );
				Vector2 size = imageSize.ToVector2() * scale * viewScale;
				Vector2 center = viewportSize.ToVector2() / 2;
				var rectInPixels = ( new Rectangle( center - size / 2, center + size / 2 ) ).ToRectangleI();
				//Rectangle rectInPixels = new Rectangle( center - size / 2, center + size / 2 );

				//Rectangle rect = rectInPixels / viewportSize.ToVector2();

				//var renderer = viewport.CanvasRenderer;

				//var pointFiltering = false;
				//if( rectInPixels.Size.X >= texture.Result.ResultSize.X && rectInPixels.Size.Y >= texture.Result.ResultSize.Y )
				//	pointFiltering = true;

				//if( pointFiltering )
				//	renderer.PushTextureFilteringMode( CanvasRenderer.TextureFilteringMode.Point );

				e.Graphics.DrawImage( image, new System.Drawing.Rectangle( rectInPixels.Left, rectInPixels.Top, rectInPixels.Size.X, rectInPixels.Size.Y ), new System.Drawing.Rectangle( 0, 0, imageSize.X, imageSize.Y ), GraphicsUnit.Pixel );

				var storeItem = GetStoreItem();

				var package = storeItem.storesWindow.GetPackage( storeItem.packageId, false );
				if( package != null )
				{
					var text = "";

					if( package.Triangles != 0 )
					{
						text += "Triangles " + PackageManager.PackageInfo.GetTrianglesVerticesAsString( package.Triangles );
						text += ", Vertices " + PackageManager.PackageInfo.GetTrianglesVerticesAsString( package.Vertices );
					}

					//var text = package.ShortDescription;
					//if( package.Triangles != 0 )
					//{
					//	text += "\r\nTriangles " + StoresWindow.GetTrianglesVerticesAsString( package.Triangles );
					//	text += ", Vertices " + StoresWindow.GetTrianglesVerticesAsString( package.Vertices );
					//}

					var margin = (int)( 2.0 * EditorAPI.DPIScale );
					var shadowOffset = (int)( 1.0 * EditorAPI.DPIScale );

					e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;//AntiAlias;

					var r = new System.Drawing.Rectangle( margin, margin, viewportSize.X - margin * 2, viewportSize.Y - margin * 2 );

					//new ColorValue( 1, 1, 1, 0.7 )

					e.Graphics.DrawString( text, DefaultFont, Brushes.Black, new System.Drawing.Rectangle( r.Left + shadowOffset, r.Top + shadowOffset, r.Width, r.Height ), StringFormat.GenericDefault );
					e.Graphics.DrawString( text, DefaultFont, Brushes.White, r, StringFormat.GenericDefault );

					//renderer.AddQuad( rect, new Rectangle( 0, 0, 1, 1 ), texture );

					//if( pointFiltering )
					//	renderer.PopTextureFilteringMode();

					imageDisplayed = true;
				}
			}
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			if( !imageDisplayed && GetSourceImage() != null )
				Invalidate();
		}

	}
}

#endif