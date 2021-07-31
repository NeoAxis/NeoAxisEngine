// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;

namespace NeoAxis
{
	/// <summary>
	/// Represents the data for an item for the NeoAxis Store.
	/// </summary>
	[ResourceFileExtension( "store" )]
	[EditorDocumentWindow( typeof( Component_StoreProduct_DocumentWindow ) )]
	[EditorSettingsCell( typeof( Component_StoreProduct_SettingsCell ) )]
	public class Component_StoreProduct : Component
	{

		//!!!!указывать файлы. текущую папку всю (опционально?)

		/// <summary>
		/// The unique identifier of the product. When the parameter is empty the identifier calculated by path name of this file.
		/// </summary>
		[DefaultValue( "" )]
		[Editor( typeof( HCItemTextBoxDropMultiline ), typeof( object ) )]
		public Reference<string> Identifier
		{
			get { if( _identifier.BeginGet() ) Identifier = _identifier.Get( this ); return _identifier.value; }
			set { if( _identifier.BeginSet( ref value ) ) { try { IdentifierChanged?.Invoke( this ); } finally { _identifier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Identifier"/> property value changes.</summary>
		public event Action<Component_StoreProduct> IdentifierChanged;
		ReferenceField<string> _identifier = "";

		/// <summary>
		/// The display name of the product.
		/// </summary>
		[Editor( typeof( HCItemTextBoxDropMultiline ), typeof( object ) )]
		public Reference<string> Title
		{
			get { if( _title.BeginGet() ) Title = _title.Get( this ); return _title.value; }
			set { if( _title.BeginSet( ref value ) ) { try { TitleChanged?.Invoke( this ); } finally { _title.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Title"/> property value changes.</summary>
		public event Action<Component_StoreProduct> TitleChanged;
		ReferenceField<string> _title = "";

		[DefaultValue( StoreProductLicense.None )]
		public Reference<StoreProductLicense> License
		{
			get { if( _license.BeginGet() ) License = _license.Get( this ); return _license.value; }
			set { if( _license.BeginSet( ref value ) ) { try { LicenseChanged?.Invoke( this ); } finally { _license.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="License"/> property value changes.</summary>
		public event Action<Component_StoreProduct> LicenseChanged;
		ReferenceField<StoreProductLicense> _license = StoreProductLicense.None;

		[DefaultValue( 0.0 )]
		public Reference<double> Cost
		{
			get { if( _cost.BeginGet() ) Cost = _cost.Get( this ); return _cost.value; }
			set { if( _cost.BeginSet( ref value ) ) { try { CostChanged?.Invoke( this ); } finally { _cost.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Cost"/> property value changes.</summary>
		public event Action<Component_StoreProduct> CostChanged;
		ReferenceField<double> _cost = 0.0;

		[DefaultValue( "" )]
		[Editor( typeof( HCItemTextBoxDropMultiline ), typeof( object ) )]
		public Reference<string> ShortDescription
		{
			get { if( _shortDescription.BeginGet() ) ShortDescription = _shortDescription.Get( this ); return _shortDescription.value; }
			set { if( _shortDescription.BeginSet( ref value ) ) { try { ShortDescriptionChanged?.Invoke( this ); } finally { _shortDescription.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShortDescription"/> property value changes.</summary>
		public event Action<Component_StoreProduct> ShortDescriptionChanged;
		ReferenceField<string> _shortDescription = "";

		[DefaultValue( "" )]
		[Editor( typeof( HCItemTextBoxDropMultiline ), typeof( object ) )]
		public Reference<string> FullDescription
		{
			get { if( _fullDescription.BeginGet() ) FullDescription = _fullDescription.Get( this ); return _fullDescription.value; }
			set { if( _fullDescription.BeginSet( ref value ) ) { try { FullDescriptionChanged?.Invoke( this ); } finally { _fullDescription.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FullDescription"/> property value changes.</summary>
		public event Action<Component_StoreProduct> FullDescriptionChanged;
		ReferenceField<string> _fullDescription = "";

		[DefaultValue( CategoryEnum.Uncategorized )]
		public Reference<CategoryEnum> Categories
		{
			get { if( _categories.BeginGet() ) Categories = _categories.Get( this ); return _categories.value; }
			set { if( _categories.BeginSet( ref value ) ) { try { CategoriesChanged?.Invoke( this ); } finally { _categories.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Categories"/> property value changes.</summary>
		public event Action<Component_StoreProduct> CategoriesChanged;
		ReferenceField<CategoryEnum> _categories = CategoryEnum.Uncategorized;

		/// <summary>
		/// The list of tags. Use comma to split tags.
		/// </summary>
		[DefaultValue( "" )]
		[Editor( typeof( HCItemTextBoxDropMultiline ), typeof( object ) )]
		public Reference<string> Tags
		{
			get { if( _tags.BeginGet() ) Tags = _tags.Get( this ); return _tags.value; }
			set { if( _tags.BeginSet( ref value ) ) { try { TagsChanged?.Invoke( this ); } finally { _tags.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Tags"/> property value changes.</summary>
		public event Action<Component_StoreProduct> TagsChanged;
		ReferenceField<string> _tags = "";

		[DefaultValue( "" )]
		public Reference<string> Version
		{
			get { if( _version.BeginGet() ) Version = _version.Get( this ); return _version.value; }
			set { if( _version.BeginSet( ref value ) ) { try { VersionChanged?.Invoke( this ); } finally { _version.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Version"/> property value changes.</summary>
		public event Action<Component_StoreProduct> VersionChanged;
		ReferenceField<string> _version = "";

		//!!!!default value
		[DefaultValue( true )]
		public Reference<bool> CreateScreenshots
		{
			get { if( _createScreenshots.BeginGet() ) CreateScreenshots = _createScreenshots.Get( this ); return _createScreenshots.value; }
			set { if( _createScreenshots.BeginSet( ref value ) ) { try { CreateScreenshotsChanged?.Invoke( this ); } finally { _createScreenshots.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CreateScreenshots"/> property value changes.</summary>
		public event Action<Component_StoreProduct> CreateScreenshotsChanged;
		ReferenceField<bool> _createScreenshots = true;

		[DefaultValue( CreateProductsEnum.MainProduct )]
		public Reference<CreateProductsEnum> CreateProducts
		{
			get { if( _createProducts.BeginGet() ) CreateProducts = _createProducts.Get( this ); return _createProducts.value; }
			set { if( _createProducts.BeginSet( ref value ) ) { try { CreateProductsChanged?.Invoke( this ); } finally { _createProducts.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CreateProducts"/> property value changes.</summary>
		public event Action<Component_StoreProduct> CreateProductsChanged;
		ReferenceField<CreateProductsEnum> _createProducts = CreateProductsEnum.MainProduct;

		///////////////////////////////////////////////

		[Flags]
		public enum CategoryEnum
		{
			Uncategorized = 0,

			_2D = 1 << 0,

			//Audio
			AmbientSounds = 1 << 1,
			Music = 1 << 2,
			SoundEffects = 1 << 3,

			Demos = 1 << 4,
			VisualEffects = 1 << 5,
			Environments = 1 << 6,

			//Extensions
			BasicExtensions = 1 << 7,
			Components = 1 << 8,
			Constructors = 1 << 9,
			Frameworks = 1 << 10,

			Images = 1 << 11,

			Materials = 1 << 12,

			//Models
			Animals = 1 << 13,
			Architecture = 1 << 14,
			Characters = 1 << 15,
			Exterior = 1 << 16,
			Food = 1 << 17,
			Industrial = 1 << 18,
			Interior = 1 << 19,
			//Plant = 1 << 20,
			Vehicles = 1 << 21,
			Nature = 1 << 22,
			UncategorizedModels = 1 << 23,

			Surfaces = 1 << 24,

			BasicContent = 1 << 25,
		}

		///////////////////////////////////////////////

		[Flags]
		public enum CreateProductsEnum
		{
			MainProduct = 1 << 0,
			SeparateProductForEachModel = 1 << 1,
			SeparateProductForEachMaterial = 1 << 2,
			SeparateProductForEachEnvironment = 1 << 3,
			//!!!!Image
		}

		///////////////////////////////////////////////

		class ImageGenerator
		{
			Vector2I imageSizeRender = new Vector2I( 1000 * 4, 562 * 4 );
			Vector2I imageSizeOutput = new Vector2I( 1000, 562 );
			const PixelFormat imageFormat = PixelFormat.A8R8G8B8;

			Component_Mesh mesh;
			Component_Surface surface;
			Component_Material material;
			Component_Skybox skybox;

			Component_Image texture;
			Viewport viewport;
			Component_Image textureRead;
			IntPtr imageData;

			Component_Scene scene;

			/////////////////////////////////////////

			public Component_Scene CreateScene( bool enable )
			{
				DetachAndOrDestroyScene();

				scene = ComponentUtility.CreateComponent<Component_Scene>( null, true, enable );

				scene.OctreeEnabled = false;

				//rendering pipeline
				{
					var pipeline = (Component_RenderingPipeline)scene.CreateComponent( RenderingSystem.RenderingPipelineDefault, -1, false );
					scene.RenderingPipeline = pipeline;

					pipeline.DeferredShading = AutoTrueFalse.False;
					pipeline.LODRange = new RangeI( 0, 0 );
					//pipeline.UseRenderTargets = false;

					scene.BackgroundColor = new ColorValue( 0.5, 0.5, 0.5 );

					scene.BackgroundColorAffectLighting = 1;
					scene.BackgroundColorEnvironmentOverride = new ColorValue( 0.8, 0.8, 0.8 );

					pipeline.Enabled = true;
				}

				//ambient light
				{
					var light = scene.CreateComponent<Component_Light>();
					light.Type = Component_Light.TypeEnum.Ambient;
					light.Brightness = ReferenceUtility.MakeReference( "Base\\ProjectSettings.component|PreviewAmbientLightBrightness" );
					//light.Brightness = ProjectSettings.Get.PreviewAmbientLightBrightness.Value;
				}

				//directional light
				{
					var light = scene.CreateComponent<Component_Light>();
					light.Type = Component_Light.TypeEnum.Directional;
					light.Transform = new Transform( new Vector3( 0, 0, 0 ), Quaternion.FromDirectionZAxisUp( new Vector3( 0, 0, -1 ) ), Vector3.One );
					light.Brightness = ReferenceUtility.MakeReference( "Base\\ProjectSettings.component|PreviewDirectionalLightBrightness" );
					//light.Brightness = ProjectSettings.Get.PreviewDirectionalLightBrightness.Value;
					//!!!!?
					light.Shadows = false;
					//light.Type = Component_Light.TypeEnum.Point;
					//light.Transform = new Transform( new Vec3( 0, 0, 2 ), Quat.Identity, Vec3.One );
				}

				scene.ViewportUpdateGetCameraSettings += Scene_ViewportUpdateGetCameraSettings;

				//connect scene to viewport
				if( viewport != null )
					viewport.AttachedScene = scene;

				return scene;
			}

			public void DetachAndOrDestroyScene()
			{
				if( scene != null )
				{
					if( viewport != null )
						viewport.AttachedScene = null;
					scene.Dispose();
					scene = null;
				}
			}

			protected virtual void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
			{
				//copy from Mesh document window code

				//var camera = scene.CameraEditor.Value;
				var bounds = scene.CalculateTotalBoundsOfObjectsInSpace();
				var cameraLookTo = bounds.GetCenter();

				double maxGararite = Math.Max( Math.Max( bounds.GetSize().X, bounds.GetSize().Y ), bounds.GetSize().Z );
				double distance = maxGararite * 2;// 2.3;
				if( distance < 2 )
					distance = 2;
				if( material != null )
					distance = 1.65;
				if( surface != null )
					distance /= 2;//!!!!

				double cameraZoomFactor = 1;
				SphericalDirection cameraDirection = new SphericalDirection( -3.83, -.47 );
				if( skybox != null )
					cameraDirection = new SphericalDirection( 0, 0 );

				var cameraPosition = cameraLookTo - cameraDirection.GetVector() * distance * cameraZoomFactor;
				var center = cameraLookTo;// GetSceneCenter();

				Vector3 from = cameraPosition;//center + cameraDirection.GetVector() * cameraDistance;
				Vector3 to = center;
				Degree fov = 65;// 75;
				if( material != null )
					fov = 40;

				var camera = new Component_Camera();
				//camera.AspectRatio = (double)ViewportControl.Viewport.SizeInPixels.X / (double)ViewportControl.Viewport.SizeInPixels.Y;
				camera.FieldOfView = fov;
				camera.NearClipPlane = Math.Max( distance / 10000, 0.01 );//.1;
				camera.FarClipPlane = Math.Max( 1000, distance * 2 );

				if( mesh?.EditorCameraTransform != null )
					camera.Transform = mesh.EditorCameraTransform;
				else if( surface?.EditorCameraTransform != null )
					camera.Transform = surface.EditorCameraTransform;
				else
					camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );
				camera.FixedUp = Vector3.ZAxis;

				viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, camera );


				//if( !cameraMode2D )
				//{

				//var cameraPosition = cameraLookTo - cameraDirection.GetVector() * cameraDistance;
				//var center = cameraLookTo;

				//Vector3 from = cameraPosition;//center + cameraDirection.GetVector() * cameraDistance;
				//Vector3 to = center;
				//Degree fov = 40;//!!!! 65;// 75;

				////!!!!
				//Component_Camera camera = new Component_Camera();
				//camera.AspectRatio = (double)viewport.SizeInPixels.X / (double)viewport.SizeInPixels.Y;
				//camera.FieldOfView = fov;
				//camera.NearClipPlane = Math.Max( cameraDistance / 10000, 0.01 );//.1;
				//camera.FarClipPlane = Math.Max( 1000, cameraDistance * 2 );

				//camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );
				////camera.Position = from;
				////camera.Direction = ( to - from ).GetNormalize();

				//camera.FixedUp = Vector3.ZAxis;
				//viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, camera );

				////!!!!в методе больше параметров
				//double aspect = (double)viewport.SizeInPixels.X / (double)viewport.SizeInPixels.Y;
				//var settings = new Viewport.CameraSettingsClass( viewport, aspect, fov, .1f, 1000, from, ( to - from ).GetNormalize(), Vec3.ZAxis );
				//viewport.CameraSettings = settings;

				//}
				//else
				//{
				//	var from = cameraLookTo + new Vector3( 0, 0, scene.CameraEditor2DPositionZ );
				//	var to = cameraLookTo;

				//	Component_Camera camera = new Component_Camera();
				//	camera.AspectRatio = (double)viewport.SizeInPixels.X / (double)viewport.SizeInPixels.Y;
				//	camera.NearClipPlane = 0.01;// Math.Max( cameraInitialDistance / 10000, 0.01 );//.1;
				//	camera.FarClipPlane = 1000;// Math.Max( 1000, cameraInitialDistance * 2 );
				//	camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.YAxis ) );
				//	camera.Projection = ProjectionType.Orthographic;
				//	camera.FixedUp = Vector3.YAxis;
				//	//!!!!need consider size by X
				//	camera.Height = cameraInitBounds.GetSize().Y * 1.4;

				//	viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, camera );
				//}

				processed = true;
			}

			void Init()
			{
				texture = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
				texture.CreateType = Component_Image.TypeEnum._2D;
				texture.CreateSize = imageSizeRender;// new Vector2I( imageSizeRender, imageSizeRender );
				texture.CreateMipmaps = false;
				texture.CreateFormat = imageFormat;
				texture.CreateUsage = Component_Image.Usages.RenderTarget;
				texture.CreateFSAA = 0;
				texture.Enabled = true;

				var renderTexture = texture.Result.GetRenderTarget( 0, 0 );

				viewport = renderTexture.AddViewport( false, true );
				viewport.AllowRenderScreenLabels = false;

				//viewport.UpdateBegin += Viewport_UpdateBegin;

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

				var imageDataSize = PixelFormatUtility.GetNumElemBytes( imageFormat ) * imageSizeRender.X * imageSizeRender.Y;
				imageData = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, imageDataSize );
			}

			//private void Viewport_UpdateBegin( Viewport viewport )
			//{
			//	//generator?.PerformUpdate();
			//}

			void Shutdown()
			{
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

			public void Generate( Component_Mesh mesh, string writeRealFileName )
			{
				this.mesh = mesh;
				Init();

				var scene = CreateScene( false );
				if( mesh != null )
				{
					var objInSpace = scene.CreateComponent<Component_MeshInSpace>();
					objInSpace.Mesh = mesh;
				}
				scene.Enabled = true;

				GenerateGeneral( writeRealFileName );
			}

			public void Generate( Component_Surface surface, string writeRealFileName )
			{
				this.surface = surface;
				Init();

				var scene = CreateScene( false );
				if( surface != null )
				{
					Component_SurfaceUtility.CreatePreviewObjects( scene, surface );
				}
				scene.Enabled = true;

				GenerateGeneral( writeRealFileName );
			}

			public void Generate( Component_Material material, string writeRealFileName )
			{
				this.material = material;
				Init();

				var scene = CreateScene( false );

				var mesh = scene.CreateComponent<Component_Mesh>( enabled: false );
				var sphere = mesh.CreateComponent<Component_MeshGeometry_Sphere>();
				sphere.SegmentsHorizontal = 64;
				sphere.SegmentsVertical = 64;
				mesh.Enabled = true;

				var objInSpace = scene.CreateComponent<Component_MeshInSpace>();
				objInSpace.Mesh = mesh;
				objInSpace.ReplaceMaterial = material;

				scene.Enabled = true;

				GenerateGeneral( writeRealFileName );
			}

			public void Generate( Component_Skybox skybox, string writeRealFileName )
			{
				this.skybox = skybox;
				Init();

				var scene = CreateScene( false );

				var instanceInScene = (Component_Skybox)skybox.Clone();
				scene.AddComponent( instanceInScene );

				scene.Enabled = true;

				GenerateGeneral( writeRealFileName );
			}

			void GenerateGeneral( string writeRealFileName )
			{
#if !DEPLOY
				//generate an image
				{
					viewport.Update( true );

					//clear temp data
					viewport.RenderingContext.MultiRenderTarget_DestroyAll();
					viewport.RenderingContext.DynamicTexture_DestroyAll();

					texture.Result.GetRealObject( true ).BlitTo( viewport.RenderingContext.CurrentViewNumber, textureRead.Result.GetRealObject( true ), 0, 0 );

					var demandedFrame = textureRead.Result.GetRealObject( true ).Read( imageData, 0 );

					while( RenderingSystem.CallBgfxFrame() < demandedFrame ) { }
				}

				//write png
				using( var bitmap = new Bitmap( imageSizeRender.X, imageSizeRender.Y, imageSizeRender.X * PixelFormatUtility.GetNumElemBytes( imageFormat ), System.Drawing.Imaging.PixelFormat.Format32bppArgb, imageData ) )
				{
					Bitmap ResizeImage( Image image, int width, int height )
					{
						Bitmap result = new Bitmap( width, height );
						using( Graphics g = Graphics.FromImage( result ) )
						{
							g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

							//downscale and clip
							var offsetX = (int)( (double)width * 0.05 );
							var offsetY = (int)( (double)height * 0.05 );
							g.DrawImage( image, -offsetX, -offsetY, width + offsetX * 2, height + offsetY * 2 );

							////fix borders
							//g.DrawImage( image, -2, -2, width + 4, height + 4 );
							////g.DrawImage( image, 0, 0, width, height );
						}
						return result;
					}

					using( var resized = ResizeImage( bitmap, imageSizeOutput.X, imageSizeOutput.Y ) )
					{
						var ext = Path.GetExtension( writeRealFileName );
						if( ext == ".png" )
							resized.Save( writeRealFileName, ImageFormat.Png );
						else if( ext == ".jpg" )
						{
							var encoder = ImageCodecInfo.GetImageEncoders().First( codec => codec.FormatID == ImageFormat.Jpeg.Guid );
							var parameters = new EncoderParameters( 1 );
							parameters.Param[ 0 ] = new EncoderParameter( System.Drawing.Imaging.Encoder.Quality, 95L );
							resized.Save( writeRealFileName, encoder, parameters );
						}
					}
				}

				DetachAndOrDestroyScene();
				Shutdown();
#endif
			}
		}

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Cost ):
					if( License.Value != StoreProductLicense.PaidPerSeat )
						skip = true;
					break;
				}
			}
		}

		public static string GetMD5( string input )
		{
			// Use input string to calculate MD5 hash
			using( System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create() )
			{
				byte[] inputBytes = Encoding.ASCII.GetBytes( input );
				byte[] hashBytes = md5.ComputeHash( inputBytes );

				// Convert the byte array to hexadecimal string
				var sb = new StringBuilder();
				for( int i = 0; i < hashBytes.Length; i++ )
					sb.Append( hashBytes[ i ].ToString( "X2" ) );
				return sb.ToString();
			}
		}

		public string GetIdentifier()
		{
			var result = Identifier.Value;

			if( string.IsNullOrEmpty( result ) )
			{
				result = Title.Value.Replace( ' ', '_' );

				var fileName = ComponentUtility.GetOwnedFileNameOfComponent( this );
				if( !string.IsNullOrEmpty( fileName ) )
					result += "_" + GetMD5( fileName + ":byfilename" ).ToLower().Substring( 0, 12 );
			}

			return result;
		}

		public string GetVersion()
		{
			var result = Version.Value;
			if( string.IsNullOrEmpty( result ) )
				result = "1.0.0.0";
			return result;
		}

		//!!!!impl. multi line text editor
		///// <summary>
		///// The list of folders to add from Assets folder. It is specified by the list, list items are separated by semicolon. Example: "Base;Samples\Starter Content;Samples\Simple Game". When the value is empty, the entire Assets folder is included.
		///// </summary>
		//[DefaultValue( "" )]
		//[Category( "Files" )]
		//public Reference<string> SelectedAssets
		//{
		//	get { if( _selectedAssets.BeginGet() ) SelectedAssets = _selectedAssets.Get( this ); return _selectedAssets.value; }
		//	set { if( _selectedAssets.BeginSet( ref value ) ) { try { SelectedAssetsChanged?.Invoke( this ); } finally { _selectedAssets.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SelectedAssets"/> property value changes.</summary>
		//public event Action<Component_StoreProduct> SelectedAssetsChanged;
		//ReferenceField<string> _selectedAssets = "";

		static IEnumerable<Enum> GetFlags( Enum e )
		{
			return Enum.GetValues( e.GetType() ).Cast<Enum>().Where( e.HasFlag );
		}

		//!!!!temp
		public static string writeToDirectory = @"C:\__TempToStore";

		public bool BuildArchives()//string writeToDirectory )
		{
			if( CreateProducts.Value.HasFlag( CreateProductsEnum.MainProduct ) )
				BuildArchive( "" );

			if( CreateProducts.Value.HasFlag( CreateProductsEnum.SeparateProductForEachMaterial ) )
			{
				var virtualSourceDirectory = Path.GetDirectoryName( ComponentUtility.GetOwnedFileNameOfComponent( this ) );
				foreach( var file in VirtualDirectory.GetFiles( virtualSourceDirectory, "*.material", SearchOption.AllDirectories ) )
				{
					if( !BuildArchive( file ) )
						return false;
				}
			}

			if( CreateProducts.Value.HasFlag( CreateProductsEnum.SeparateProductForEachEnvironment ) )
			{
				var virtualSourceDirectory = Path.GetDirectoryName( ComponentUtility.GetOwnedFileNameOfComponent( this ) );
				//!!!!другие расширения тоже
				foreach( var file in VirtualDirectory.GetFiles( virtualSourceDirectory, "*.skybox", SearchOption.AllDirectories ) )
				{
					if( !BuildArchive( file ) )
						return false;
				}
			}

			if( CreateProducts.Value.HasFlag( CreateProductsEnum.SeparateProductForEachModel ) )
			{
				//!!!!
			}

			return true;
		}

		void EnumerateAllChildren( TextBlock block, Action<TextBlock> callback )
		{
			foreach( var child in block.Children )
			{
				callback( child );
				EnumerateAllChildren( child, callback );
			}
		}

		string[] GetReferencedFilesForResource( string virtualFileName )
		{
			var result = new ESet<string>();
			result.AddWithCheckAlreadyContained( virtualFileName );

			var extension = Path.GetExtension( virtualFileName );

			if( extension == ".material" )
			{
				var block = TextBlockUtility.LoadFromVirtualFile( virtualFileName );

				EnumerateAllChildren( block, delegate ( TextBlock child )
				{
					if( child.Name == "Texture" )
					{
						var texture = child.GetAttribute( "GetByReference" );
						if( !string.IsNullOrEmpty( texture ) )
						{
							//!!!!локальные ссылки

							if( VirtualFile.Exists( texture ) )
								result.AddWithCheckAlreadyContained( texture );
						}
					}
				} );
			}

			if( extension == ".skybox" )
			{
				var block = TextBlockUtility.LoadFromVirtualFile( virtualFileName );

				EnumerateAllChildren( block, delegate ( TextBlock child )
				{
					if( child.Name == "Cubemap" )
					{
						var texture = child.GetAttribute( "GetByReference" );
						if( !string.IsNullOrEmpty( texture ) )
						{
							var postfixes = new string[] { "", "_Gen.info", "_GenEnv.dds", "_GenIrr.dds" };
							foreach( var postfix in postfixes )
							{
								var fileName = texture + postfix;
								if( VirtualFile.Exists( fileName ) )
									result.AddWithCheckAlreadyContained( fileName );
							}
						}
					}
				} );
			}

			return result.ToArray();
		}

		public bool BuildArchive( string specifiedFile )//string writeToDirectory )
		{
#if !DEPLOY
			//get info

			var license = EnumUtility.GetValueDisplayName( License.Value );

			var identifier = GetIdentifier();
			if( !string.IsNullOrEmpty( specifiedFile ) )
				identifier += "_" + Path.GetFileNameWithoutExtension( specifiedFile ).Replace( ' ', '_' );

			var tilte = Title.Value;
			if( !string.IsNullOrEmpty( specifiedFile ) )
				tilte = Path.GetFileNameWithoutExtension( specifiedFile );

			var sourceDirectory = Path.GetDirectoryName( VirtualPathUtility.GetRealPathByVirtual( ComponentUtility.GetOwnedFileNameOfComponent( this ) ) );

			string[] files;
			if( !string.IsNullOrEmpty( specifiedFile ) )
			{
				files = GetReferencedFilesForResource( specifiedFile );
				for( int n = 0; n < files.Length; n++ )
					files[ n ] = VirtualPathUtility.GetRealPathByVirtual( files[ n ] );
			}
			else
				files = Directory.GetFiles( sourceDirectory, "*", SearchOption.AllDirectories );


			//start creation

			if( !Directory.Exists( writeToDirectory ) )
				Directory.CreateDirectory( writeToDirectory );
			var destFileName = Path.Combine( writeToDirectory, identifier + "-" + GetVersion() + ".neoaxispackage" );

			//check file already exists
			if( File.Exists( destFileName ) )
			{
				var text = $"The file with name \'{destFileName}\' is already exists. Overwrite?";
				if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
					return false;
			}

			try
			{
				//make zip

				if( File.Exists( destFileName ) )
					File.Delete( destFileName );

				//prepare Package.info
				var packageInfoTempFileName = Path.GetTempFileName();
				{
					var block = new TextBlock();

					block.SetAttribute( "Identifier", identifier );// GetIdentifier() );
					block.SetAttribute( "Title", tilte );// Title.Value );

					block.SetAttribute( "Version", GetVersion() );

					//!!!!
					block.SetAttribute( "Author", "Share CC Attribution" );

					block.SetAttribute( "Description", ShortDescription );
					//"ShortDescription"

					block.SetAttribute( "FullDescription", FullDescription.Value );

					block.SetAttribute( "License", license );

					if( License.Value == StoreProductLicense.PaidPerSeat )
						block.SetAttribute( "Cost", Cost.Value.ToString() );

					//categories
					{
						var s = "";
						foreach( CategoryEnum flag in GetFlags( Categories.Value ) )
						{
							if( flag != 0 )
							{
								if( s.Length != 0 )
									s += ", ";
								s += TypeUtility.DisplayNameAddSpaces( flag.ToString() );
							}
						}
						block.SetAttribute( "Categories", s );
					}

					block.SetAttribute( "Tags", Tags.Value );

					string openAfterInstall;
					if( !string.IsNullOrEmpty( specifiedFile ) )
						openAfterInstall = Path.GetDirectoryName( specifiedFile );
					else
						openAfterInstall = sourceDirectory.Substring( VirtualFileSystem.Directories.Assets.Length + 1 );
					block.SetAttribute( "OpenAfterInstall", openAfterInstall );

					//MustRestart = True
					//AddCSharpFilesToProject = "Store\\Simple Level Generator"

					File.WriteAllText( packageInfoTempFileName, block.DumpToString() );
				}

				using( var archive = ZipFile.Open( destFileName, ZipArchiveMode.Create ) )
				{
					foreach( var file in files )
					{
						if( Path.GetExtension( file ) == ".store" )
							continue;

						var entryName = file.Substring( VirtualFileSystem.Directories.Project.Length + 1 );
						//var entryName = file.Substring( sourceDirectory.Length + 1 );
						archive.CreateEntryFromFile( file, entryName );
					}

					archive.CreateEntryFromFile( packageInfoTempFileName, "Package.info" );
				}

				if( File.Exists( packageInfoTempFileName ) )
					File.Delete( packageInfoTempFileName );

				int triangles = 0;
				int vertices = 0;

				//try to create screenshots
				if( CreateScreenshots )
				{
					if( Categories.Value.HasFlag( CategoryEnum.Surfaces ) )
					{
						//surface

						var resourceType = ResourceManager.GetTypeByName( "Surface" );
						var importExtensions = new ESet<string>();
						foreach( var e in resourceType.FileExtensions )
							importExtensions.AddWithCheckAlreadyContained( "." + e );

						var importVirtualFileNames = new List<string>();
						foreach( var file in files )
						{
							var ext = Path.GetExtension( file );
							if( !string.IsNullOrEmpty( ext ) && importExtensions.Contains( ext ) )
							{
								var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
								if( !string.IsNullOrEmpty( virtualFileName ) )
									importVirtualFileNames.Add( virtualFileName );
							}
						}

						if( importVirtualFileNames.Count == 1 )
						{
							var surface = ResourceManager.LoadResource<Component_Surface>( importVirtualFileNames[ 0 ] );
							if( surface != null )
							{
								var generator = new ImageGenerator();
								generator.Generate( surface, destFileName + ".logo.png" );
							}
						}
					}
					else if( ( Categories.Value & ( CategoryEnum.Animals | CategoryEnum.Architecture | CategoryEnum.Characters | CategoryEnum.Exterior | CategoryEnum.Food | CategoryEnum.Industrial | CategoryEnum.Interior | /*CategoryEnum.Plant | */CategoryEnum.Vehicles | CategoryEnum.Nature | CategoryEnum.UncategorizedModels ) ) != 0 )
					{
						//model

						//find in the folder one import file (FBX, etc). make screenshot of 'Mesh' object inside the import file.

						var resourceType = ResourceManager.GetTypeByName( "Import 3D" );
						var importExtensions = new ESet<string>();
						foreach( var e in resourceType.FileExtensions )
							importExtensions.AddWithCheckAlreadyContained( "." + e );

						var importVirtualFileNames = new List<string>();
						foreach( var file in files )
						{
							var ext = Path.GetExtension( file );
							if( !string.IsNullOrEmpty( ext ) && importExtensions.Contains( ext ) )
							{
								var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
								if( !string.IsNullOrEmpty( virtualFileName ) )
									importVirtualFileNames.Add( virtualFileName );
							}
						}

						if( importVirtualFileNames.Count == 1 )
						{
							var import3D = ResourceManager.LoadResource<Component_Import3D>( importVirtualFileNames[ 0 ] );
							if( import3D != null )
							{
								var mesh = import3D.GetComponent<Component_Mesh>( "Mesh" );
								if( mesh != null )
								{
									var generator = new ImageGenerator();
									generator.Generate( mesh, destFileName + ".logo.png" );

									//!!!!если не делаем скриншоты, тогда не будет
									if( mesh.Result != null )
									{
										triangles = mesh.Result.ExtractedIndices.Length / 3;
										vertices = mesh.Result.ExtractedVerticesPositions.Length;
									}
								}
							}
						}
					}
					else if( Categories.Value.HasFlag( CategoryEnum.Materials ) )
					{
						//material

						var resourceType = ResourceManager.GetTypeByName( "Material" );
						var importExtensions = new ESet<string>();
						foreach( var e in resourceType.FileExtensions )
							importExtensions.AddWithCheckAlreadyContained( "." + e );

						var importVirtualFileNames = new List<string>();
						foreach( var file in files )
						{
							var ext = Path.GetExtension( file );
							if( !string.IsNullOrEmpty( ext ) && importExtensions.Contains( ext ) )
							{
								var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
								if( !string.IsNullOrEmpty( virtualFileName ) )
									importVirtualFileNames.Add( virtualFileName );
							}
						}

						if( importVirtualFileNames.Count == 1 )
						{
							var material = ResourceManager.LoadResource<Component_Material>( importVirtualFileNames[ 0 ] );
							if( material != null )
							{
								var generator = new ImageGenerator();
								generator.Generate( material, destFileName + ".logo.png" );
							}
						}
					}
					else if( Categories.Value.HasFlag( CategoryEnum.Environments ) )
					{
						//skybox

						var resourceType = ResourceManager.GetTypeByName( "Skybox" );
						var importExtensions = new ESet<string>();
						foreach( var e in resourceType.FileExtensions )
							importExtensions.AddWithCheckAlreadyContained( "." + e );

						var importVirtualFileNames = new List<string>();
						foreach( var file in files )
						{
							var ext = Path.GetExtension( file );
							if( !string.IsNullOrEmpty( ext ) && importExtensions.Contains( ext ) )
							{
								var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
								if( !string.IsNullOrEmpty( virtualFileName ) )
									importVirtualFileNames.Add( virtualFileName );
							}
						}

						if( importVirtualFileNames.Count == 1 )
						{
							var skybox = ResourceManager.LoadResource<Component_Skybox>( importVirtualFileNames[ 0 ] );
							if( skybox != null )
							{
								var generator = new ImageGenerator();
								generator.Generate( skybox, destFileName + ".logo.jpg" );// png" );
							}
						}
					}
				}

				//write info json
				{
					var jsonFileName = destFileName + ".json";

					var sw = new StringWriter();
					using( JsonWriter writer = new JsonTextWriter( sw ) )
					{
						writer.Formatting = Formatting.Indented;
						writer.WriteStartObject();

						writer.WritePropertyName( "Author" );
						//!!!!
						//writer.WriteValue( "3" );
						writer.WriteValue( "Share CC Attribution" );

						writer.WritePropertyName( "Identifier" );
						writer.WriteValue( identifier );// GetIdentifier() );

						writer.WritePropertyName( "Title" );
						writer.WriteValue( tilte );// Title.Value );

						writer.WritePropertyName( "License" );
						//var license = EnumExtension.GetValueDisplayName( License.Value );
						//if( string.IsNullOrEmpty( license ) )
						//	license = TypeUtility.DisplayNameAddSpaces( license.ToString() );
						writer.WriteValue( license );

						if( License.Value == StoreProductLicense.PaidPerSeat )
						{
							writer.WritePropertyName( "Cost" );
							writer.WriteValue( Cost.Value.ToString() );
						}

						writer.WritePropertyName( "ShortDescription" );
						writer.WriteValue( ShortDescription.Value );

						writer.WritePropertyName( "FullDescription" );
						writer.WriteValue( FullDescription.Value );

						{
							writer.WritePropertyName( "Categories" );

							var s = "";
							foreach( CategoryEnum flag in GetFlags( Categories.Value ) )
							{
								if( flag != 0 )
								{
									if( s.Length != 0 )
										s += ", ";
									s += TypeUtility.DisplayNameAddSpaces( flag.ToString() );
								}
							}
							writer.WriteValue( s );
							//writer.WriteValue( Categories.Value.ToString() );
						}

						writer.WritePropertyName( "Tags" );
						writer.WriteValue( Tags.Value );

						writer.WritePropertyName( "Version" );
						writer.WriteValue( GetVersion() );

						if( triangles != 0 )
						{
							writer.WritePropertyName( "Triangles" );
							writer.WriteValue( triangles );
							writer.WritePropertyName( "Vertices" );
							writer.WriteValue( vertices );
						}

						//writer.WriteEnd();
						writer.WriteEndObject();
					}

					File.WriteAllText( jsonFileName, sw.ToString() );
				}

			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				return false;
			}

#endif
			return true;
		}
	}

}
