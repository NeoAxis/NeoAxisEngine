// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace NeoAxis.Editor
{
	public static class PreviewImagesManager
	{
		//!!!!config
		const int imageSizeRender = 512;//256
		const int imageEmptyBorder = 13;
		const int imageSizeResult = 128;

		const int maxProcessorCount = 10;
		const int maxQueueOfImagesToProcess = 100;

		const PixelFormat imageFormat = PixelFormat.A8R8G8B8;

		//

		static ESet<ResourceManager.ResourceType> supportedResourceTypes = new ESet<ResourceManager.ResourceType>();

		static Queue<ImageToProcessItem> imagesToProcess = new Queue<ImageToProcessItem>();
		static List<Processor> processors = new List<Processor>();

		//key: virtual file name
		static Dictionary<string, LoadedPreviewItem> loadedPreviews = new Dictionary<string, LoadedPreviewItem>();

		///////////////////////////////////////////////

		class ImageToProcessItem
		{
			public string virtualFileName;
		}

		///////////////////////////////////////////////

		class Processor
		{
			Component_Image texture;
			Viewport viewport;
			Component_Image textureRead;
			IntPtr imageData;

			ImageToProcessItem currentTask;
			PreviewImageGenerator generator;
			int demandedFrame;

			//

			public bool Init()
			{
				try
				{
					texture = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
					texture.CreateType = Component_Image.TypeEnum._2D;
					texture.CreateSize = new Vector2I( imageSizeRender, imageSizeRender );
					texture.CreateMipmaps = false;
					texture.CreateFormat = imageFormat;
					texture.CreateUsage = Component_Image.Usages.RenderTarget;
					texture.CreateFSAA = 0;
					texture.Enabled = true;

					var renderTexture = texture.Result.GetRenderTarget( 0, 0 );

					viewport = renderTexture.AddViewport( false, true );
					viewport.AllowRenderScreenLabels = false;

					viewport.UpdateBegin += Viewport_UpdateBegin;

					textureRead = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
					textureRead.CreateType = Component_Image.TypeEnum._2D;
					textureRead.CreateSize = texture.CreateSize;
					textureRead.CreateMipmaps = false;
					textureRead.CreateFormat = texture.CreateFormat;
					textureRead.CreateUsage = Component_Image.Usages.ReadBack | Component_Image.Usages.BlitDestination;
					textureRead.CreateFSAA = 0;
					textureRead.Enabled = true;
					//!!!!
					textureRead.Result.PrepareNativeObject();

					var imageDataBytes = PixelFormatUtility.GetNumElemBytes( imageFormat ) * imageSizeRender * imageSizeRender;
					imageData = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, imageDataBytes );

				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
					return false;
				}

				return true;
			}

			public bool Working
			{
				get { return currentTask != null; }
			}

			static Type GetPreviewClass( object obj )
			{
				var attribs = (EditorPreviewImageAttribute[])obj.GetType().GetCustomAttributes( typeof( EditorPreviewImageAttribute ), true );
				if( attribs.Length != 0 )
				{
					var attrib = attribs[ 0 ];
					if( !string.IsNullOrEmpty( attrib.PreviewClassName ) )
					{
						var type = EditorUtility.GetTypeByName( attrib.PreviewClassName );
						if( type == null )
							Log.Warning( $"PreviewImagesManager: GetPreviewClass: Class with name \"{attrib.PreviewClassName}\" is not found." );
						return type;
					}
					else
						return attrib.PreviewClass;
				}

				return null;
			}

			static void CreatePreviewGeneratorForResource( string virtualFileName, out object objectOfPreview, out PreviewImageGenerator generator )
			{
				generator = null;

				var resource = ResourceManager.LoadResource( virtualFileName, true );

				objectOfPreview = resource.ResultComponent;
				if( objectOfPreview != null )
				{
					var previewClass = GetPreviewClass( objectOfPreview );
					if( previewClass != null )
						generator = (PreviewImageGenerator)Activator.CreateInstance( previewClass );
				}
			}

			private void Viewport_UpdateBegin( Viewport viewport )
			{
				generator?.PerformUpdate();
			}

			public void StartTask( ImageToProcessItem imageToProcess )
			{
				currentTask = imageToProcess;

				try
				{
					CreatePreviewGeneratorForResource( imageToProcess.virtualFileName, out var objectOfPreview, out generator );

					if( generator != null )
					{
						generator.Init( viewport, objectOfPreview );

						viewport.Update( true );

						//clear temp data
						viewport.RenderingContext.MultiRenderTarget_DestroyAll();
						viewport.RenderingContext.DynamicTexture_DestroyAll();

						texture.Result.RealObject.BlitTo( viewport.RenderingContext.CurrentViewNumber, textureRead.Result.RealObject, 0, 0 );

						demandedFrame = textureRead.Result.RealObject.Read( imageData, 0 );
					}
					else
						ClearTask();
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
				}
			}

			static RectangleI FindActualData( ImageUtility.Image2D image )
			{
				int minX = 0;
				for( ; minX < image.Size.X; minX++ )
				{
					for( int y = 0; y < image.Size.Y; y++ )
					{
						if( image.GetPixel( new Vector2I( minX, y ) ).W >= 0.004 )
							goto endMinX;
					}
				}
				endMinX:;

				int minY = 0;
				for( ; minY < image.Size.Y; minY++ )
				{
					for( int x = 0; x < image.Size.X; x++ )
					{
						if( image.GetPixel( new Vector2I( x, minY ) ).W >= 0.004 )
							goto endMinY;
					}
				}
				endMinY:;

				int maxX = image.Size.X - 1;
				for( ; maxX >= 0; maxX-- )
				{
					for( int y = 0; y < image.Size.Y; y++ )
					{
						if( image.GetPixel( new Vector2I( maxX, y ) ).W >= 0.004 )
							goto endMaxX;
					}
				}
				endMaxX:;

				int maxY = image.Size.Y - 1;
				for( ; maxY >= 0; maxY-- )
				{
					for( int x = 0; x < image.Size.X; x++ )
					{
						if( image.GetPixel( new Vector2I( x, maxY ) ).W >= 0.004 )
							goto endMaxY;
					}
				}
				endMaxY:;

				//fix empty image
				if( maxX < minX )
					maxX = minX;
				if( maxY < minY )
					maxY = minY;

				var rect = new RectangleI( minX, minY, maxX + 2, maxY + 2 );

				//make square
				if( rect.Size.X < rect.Size.Y )
					rect.Expand( new Vector2I( ( rect.Size.Y - rect.Size.X ) / 2, 0 ) );
				else if( rect.Size.Y < rect.Size.X )
					rect.Expand( new Vector2I( 0, ( rect.Size.X - rect.Size.Y ) / 2 ) );

				return rect;
			}

			public void Update()
			{
				if( Working )
				{
					try
					{
						if( RenderingSystem.LastFrameNumber >= demandedFrame )
						{
							//task is done

							var imageRender = new ImageUtility.Image2D( imageFormat, new Vector2I( imageSizeRender, imageSizeRender ), imageData );

							//get clip rectangle
							RectangleI rectangle;
							if( generator.ClampImage )
							{
								rectangle = FindActualData( imageRender );

								//borders
								rectangle.Expand( imageEmptyBorder );

								//!!!!станет не квадратный
								////clamp
								//if( rectangle.Left < 0 )
								//	rectangle.Left = 0;
								//if( rectangle.Right > imageSizeRender )
								//	rectangle.Right = imageSizeRender;
								//if( rectangle.Top < 0 )
								//	rectangle.Top = 0;
								//if( rectangle.Bottom > imageSizeRender )
								//	rectangle.Bottom = imageSizeRender;
							}
							else
								rectangle = new RectangleI( 0, 0, imageSizeRender, imageSizeRender );

							var newSize = rectangle.Size;

							//make clamped image
							var image2 = new ImageUtility.Image2D( imageFormat, newSize );
							image2.Blit( Vector2I.Zero, imageRender, rectangle.LeftTop );

							//make bitmap from clamped image
							Bitmap bitmap2;
							unsafe
							{
								fixed( byte* pImage2 = image2.Data )
								{
									bitmap2 = new Bitmap( newSize.X, newSize.Y, newSize.X * PixelFormatUtility.GetNumElemBytes( imageFormat ), System.Drawing.Imaging.PixelFormat.Format32bppArgb, (IntPtr)pImage2 );
								}
							}
							//var bitmapRender = new Bitmap( imageSizeRender, imageSizeRender, imageSizeRender * PixelFormatUtility.GetNumElemBytes( imageFormat ), System.Drawing.Imaging.PixelFormat.Format32bppArgb, imageData );

							//make result bitmap
							var bitmapResult = ResizeBitmap( bitmap2, imageSizeResult, imageSizeResult );

							//save result bitmap
							var destRealFileName = GetCacheFileNameByResourceVirtualFileName( currentTask.virtualFileName );
							try
							{
								if( !Directory.Exists( Path.GetDirectoryName( destRealFileName ) ) )
									Directory.CreateDirectory( Path.GetDirectoryName( destRealFileName ) );

								bitmapResult.Save( destRealFileName, ImageFormat.Png );
							}
							catch( Exception e )
							{
								Log.Warning( $"PreviewImagesManager: Processor: Update: Unable to save bitmap to \'{destRealFileName}\'. " + e.Message );
							}
							finally
							{
								bitmap2.Dispose();
								bitmapResult.Dispose();
							}

							//task end
							ClearTask();

							//update content browsers
							foreach( var browser in ContentBrowser.AllInstances )
								browser.needUpdateImages = true;
						}

					}
					catch( Exception e )
					{
						Log.Warning( e.Message );
					}
				}
			}

			public void ClearTask()
			{
				if( viewport != null )
					viewport.AttachedScene = null;

				generator?.DetachAndOrDestroyScene();

				currentTask = null;
				generator = null;
				demandedFrame = -1;
			}

			public void Dispose()
			{
				ClearTask();

				if( imageData != IntPtr.Zero )
				{
					NativeUtility.Free( imageData );
					imageData = IntPtr.Zero;
				}

				texture?.Dispose();
				texture = null;
				viewport = null;
				textureRead?.Dispose();
				textureRead = null;
			}
		}

		///////////////////////////////////////////////

		class LoadedPreviewItem
		{
			public Image originalImage;
			public Image smallImageForTreeView;
			public bool needReload;

			//!!!!
			//public void DisposeImages()
			//{
			//	originalImage?.Dispose();
			//	smallImageForTreeView?.Dispose();
			//}
		}

		///////////////////////////////////////////////

		internal static void Init()
		{
			var types = new string[] { "Material", "Image", "Mesh", "Import 3D" };
			//!!!!
			//Character
			//Skybox
			//Character2D
			//Surface
			//Sprite

			foreach( var typeName in types )
			{
				var type = ResourceManager.GetTypeByName( typeName );
				if( type != null )
					RegisterResourceType( type );
			}
		}

		internal static void Shutdown()
		{
			try
			{
				foreach( var processor in processors )
					processor.Dispose();
				processors.Clear();
			}
			catch { }
		}

		public static void RegisterResourceType( ResourceManager.ResourceType resourceType )
		{
			supportedResourceTypes.AddWithCheckAlreadyContained( resourceType );
		}

		public static bool IsResourceTypeSupported( ResourceManager.ResourceType resourceType )
		{
			return supportedResourceTypes.Contains( resourceType );
		}

		static LoadedPreviewItem GetLoadedPreview( string virtualFileName )
		{
			if( loadedPreviews.TryGetValue( virtualFileName, out var item ) )
				return item;
			return null;
		}

		static Bitmap ResizeBitmap( Image bmp, int width, int height )
		{
			Bitmap result = new Bitmap( width, height );
			using( Graphics g = Graphics.FromImage( result ) )
			{
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.DrawImage( bmp, 0, 0, width, height );
			}
			return result;
		}

		static LoadedPreviewItem GetOrLoadPreview( string virtualFileName, out bool cacheNotExists )
		{
			cacheNotExists = false;

			var item = GetLoadedPreview( virtualFileName );

			if( item == null || item.needReload )
			{
				var cacheFileName = GetCacheFileNameByResourceVirtualFileName( virtualFileName );
				if( File.Exists( cacheFileName ) )
				{
					//!!!!
					////dispose old item
					//item?.DisposeImages();

					//new

					item = new LoadedPreviewItem();

					//load originalImage
					var bytes = File.ReadAllBytes( cacheFileName );
					var ms = new MemoryStream( bytes );
					item.originalImage = Image.FromStream( ms );

					//small image for tree view
					int treeSize;
					if( EditorAPI.DPIScale >= 2.0 )
						treeSize = 32;
					else if( EditorAPI.DPIScale >= 1.75 )
						treeSize = 28;
					else if( EditorAPI.DPIScale >= 1.5 )
						treeSize = 24;
					else
						treeSize = 16;
					item.smallImageForTreeView = ResizeBitmap( item.originalImage, treeSize, treeSize );

					// 16px for display scale >= 100%
					// 24px for display scale >= 150% (32px downscale)
					// 28px for display scale >= 175% (32px downscale)
					// 32px for display scale >= 200%

					loadedPreviews[ virtualFileName ] = item;
				}
				else
					cacheNotExists = true;
			}

			return item;
		}

		public static Image GetImageForResource( string realFileName, bool forTreeView )
		{
			try
			{
				var resourceType = ResourceManager.GetTypeByFileExtension( Path.GetExtension( realFileName ) );
				if( resourceType != null && IsResourceTypeSupported( resourceType ) )
				{
					var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( realFileName );
					if( !string.IsNullOrEmpty( virtualFileName ) )
					{
						var item = GetOrLoadPreview( virtualFileName, out var cacheNotExists );
						if( item != null )
							return forTreeView ? item.smallImageForTreeView : item.originalImage;

						if( cacheNotExists )
							AddResourceToProcess( realFileName );
					}
				}

				return null;
			}
			catch( Exception e )
			{
				Log.Warning( "PreviewImagesManager: GetImageForResource: " + e.Message );
				return null;
			}
		}

		static ImageToProcessItem GetImageToProcessItemInQueue( string virtualFileName )
		{
			return imagesToProcess.FirstOrDefault( item => item.virtualFileName == virtualFileName );
		}

		public static void AddResourceToProcess( string realFileName )
		{
			try
			{
				var resourceType = ResourceManager.GetTypeByFileExtension( Path.GetExtension( realFileName ) );
				if( resourceType != null && IsResourceTypeSupported( resourceType ) )
				{
					var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( realFileName );
					if( !string.IsNullOrEmpty( virtualFileName ) )
					{

						//reload already loaded preview
						var loadedItem = GetLoadedPreview( virtualFileName );
						if( loadedItem != null )
							loadedItem.needReload = true;

						//check already added
						if( GetImageToProcessItemInQueue( virtualFileName ) == null )
						{
							//too much in queue
							if( imagesToProcess.Count < maxQueueOfImagesToProcess )
							{
								//was added
								var item = new ImageToProcessItem();
								item.virtualFileName = virtualFileName;
								imagesToProcess.Enqueue( item );
							}
						}

					}
				}
			}
			catch( Exception e )
			{
				Log.Warning( "PreviewImagesManager: NeedRefreshResource: " + e.Message );
			}
		}

		static Processor GetFreeProcessor()
		{
			//find free
			{
				var processor = processors.FirstOrDefault( p => !p.Working );
				if( processor != null )
					return processor;
			}

			//create
			if( processors.Count < maxProcessorCount )
			{
				var processor = new Processor();
				if( !processor.Init() )
				{
					processor.Dispose();
					return null;
				}
				processors.Add( processor );
				return processor;
			}

			return null;
		}

		public static bool ExistsWorkingProcessors()
		{
			return processors.Exists( p => p.Working );
		}

		public static void Update()
		{
			//start tasks
			again:
			if( imagesToProcess.Count != 0 )
			{
				var processor = GetFreeProcessor();
				if( processor != null )
				{
					var imageToProcess = imagesToProcess.Dequeue();
					processor.StartTask( imageToProcess );
					goto again;
				}
			}

			//update processors
			foreach( var processor in processors )
				processor.Update();
		}

		static string GetCacheFileNameByResourceVirtualFileName( string virtualFileName )
		{
			return PathUtility.Combine( VirtualFileSystem.Directories.Project, @"Caches\Files", virtualFileName ) + ".preview.png";
		}


		//!!!!
		static Image defaultImage32;
		//static Image deleteImage;

		//!!!!specific
		public static Image GetImageForPaintLayer( Component_PaintLayer layer )//public static Image GetImage( Component component )
		{
			if( layer != null )
			{
				Component obj = layer.Material.Value;
				if( obj == null )
					obj = layer.Surface.Value;

				if( obj != null )
				{
					if( obj.Parent == null )
					{
						var virtualFileName = ComponentUtility.GetOwnedFileNameOfComponent( obj );
						if( !string.IsNullOrEmpty( virtualFileName ) )
						{
							var realFileName = VirtualPathUtility.GetRealPathByVirtual( virtualFileName );
							return GetImageForResource( realFileName, false );
						}
					}
				}
			}

			//!!!!

			if( defaultImage32 == null )
				defaultImage32 = Properties.Resources.Default_32;
			return defaultImage32;

			//if( Time.Current % 2.0 < 1 )
			//{
			//	if( newImage == null )
			//		newImage = Properties.Resources.Add_32;
			//	return newImage;
			//}
			//else
			//{
			//	if( deleteImage == null )
			//		deleteImage = Properties.Resources.Delete_32;
			//	return deleteImage;
			//}

			//if( Time.Current % 2.0 < 1 )
			//{
			//	return Properties.Resources.New_32;
			//}
			//else
			//{
			//	return Properties.Resources.Delete_32;
			//}

		}

	}
}
