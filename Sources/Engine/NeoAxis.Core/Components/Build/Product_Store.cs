// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.Json;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents the product build settings for NeoAxis Store.
	/// </summary>
	//[NewObjectSettings( typeof( NewObjectSettingsProductStore ) )]
	public class Product_Store : Product
	{
		///// <summary>
		///// The nickname of author of the product.
		///// </summary>
		//[DefaultValue( "NeoAxis" )]
		//public Reference<string> Author
		//{
		//	get { if( _author.BeginGet() ) Author = _author.Get( this ); return _author.value; }
		//	set { if( _author.BeginSet( ref value ) ) { try { AuthorChanged?.Invoke( this ); } finally { _author.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Author"/> property value changes.</summary>
		//public event Action<Product_Store> AuthorChanged;
		//ReferenceField<string> _author = "NeoAxis";

		//!!!!
		[Browsable( false )]
		public Reference<ProductTypeEnum> ProductType
		{
			get { return ProductTypeEnum.ProjectItem; }
		}
		///// <summary>
		///// The type of the product.
		///// </summary>
		//[DefaultValue( ProductTypeEnum.ProjectItem )]
		//public Reference<ProductTypeEnum> ProductType
		//{
		//	get { if( _productType.BeginGet() ) ProductType = _productType.Get( this ); return _productType.value; }
		//	set { if( _productType.BeginSet( ref value ) ) { try { ProductTypeChanged?.Invoke( this ); } finally { _productType.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ProductType"/> property value changes.</summary>
		//public event Action<Product_Store> ProductTypeChanged;
		//ReferenceField<ProductTypeEnum> _productType = ProductTypeEnum.ProjectItem;


		///// <summary>
		///// Whether to add all folders and files of the folder where this file placed.
		///// </summary>
		//[DefaultValue( false )]
		//[Category( "Files" )]
		//public Reference<bool> AddThisFolder
		//{
		//	get { if( _addThisFolder.BeginGet() ) AddThisFolder = _addThisFolder.Get( this ); return _addThisFolder.value; }
		//	set { if( _addThisFolder.BeginSet( ref value ) ) { try { AddThisFolderChanged?.Invoke( this ); } finally { _addThisFolder.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AddThisFolder"/> property value changes.</summary>
		//public event Action<Product_Store> AddThisFolderChanged;
		//ReferenceField<bool> _addThisFolder = false;

		/// <summary>
		/// Whether to use not modified shaders. When the parameter is enabled, the Shaders folder will not added to the build.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Files" )]
		public Reference<bool> OriginalShaders
		{
			get { if( _originalShaders.BeginGet() ) OriginalShaders = _originalShaders.Get( this ); return _originalShaders.value; }
			set { if( _originalShaders.BeginSet( ref value ) ) { try { OriginalShadersChanged?.Invoke( this ); } finally { _originalShaders.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OriginalShaders"/> property value changes.</summary>
		public event Action<Product_Store> OriginalShadersChanged;
		ReferenceField<bool> _originalShaders = true;

		///// <summary>
		///// Whether to add basic shaders. Best way to add only your new shaders.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> AddShaders
		//{
		//	get { if( _addShaders.BeginGet() ) AddShaders = _addShaders.Get( this ); return _addShaders.value; }
		//	set { if( _addShaders.BeginSet( ref value ) ) { try { AddShadersChanged?.Invoke( this ); } finally { _addShaders.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AddShaders"/> property value changes.</summary>
		//public event Action<Product_Store> AddShadersChanged;
		//ReferenceField<bool> _addShaders = false;

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
		public event Action<Product_Store> IdentifierChanged;
		ReferenceField<string> _identifier = "";

		///// <summary>
		///// The display name of the product.
		///// </summary>
		//[Editor( typeof( HCItemTextBoxDropMultiline ), typeof( object ) )]
		//public Reference<string> Title
		//{
		//	get { if( _title.BeginGet() ) Title = _title.Get( this ); return _title.value; }
		//	set { if( _title.BeginSet( ref value ) ) { try { TitleChanged?.Invoke( this ); } finally { _title.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Title"/> property value changes.</summary>
		//public event Action<Product_Store> TitleChanged;
		//ReferenceField<string> _title = "";

		[DefaultValue( "" )]
		[Editor( typeof( HCItemTextBoxDropMultiline ), typeof( object ) )]
		public Reference<string> ShortDescription
		{
			get { if( _shortDescription.BeginGet() ) ShortDescription = _shortDescription.Get( this ); return _shortDescription.value; }
			set { if( _shortDescription.BeginSet( ref value ) ) { try { ShortDescriptionChanged?.Invoke( this ); } finally { _shortDescription.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShortDescription"/> property value changes.</summary>
		public event Action<Product_Store> ShortDescriptionChanged;
		ReferenceField<string> _shortDescription = "";

		[DefaultValue( "" )]
		[Editor( typeof( HCItemTextBoxDropMultiline ), typeof( object ) )]
		public Reference<string> FullDescription
		{
			get { if( _fullDescription.BeginGet() ) FullDescription = _fullDescription.Get( this ); return _fullDescription.value; }
			set { if( _fullDescription.BeginSet( ref value ) ) { try { FullDescriptionChanged?.Invoke( this ); } finally { _fullDescription.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FullDescription"/> property value changes.</summary>
		public event Action<Product_Store> FullDescriptionChanged;
		ReferenceField<string> _fullDescription = "";

		[DefaultValue( ProjectItemCategoriesEnum.Uncategorized )]
		[DisplayName( "Categories" )]
		public Reference<ProjectItemCategoriesEnum> ProjectItemCategories
		{
			get { if( _projectItemCategories.BeginGet() ) ProjectItemCategories = _projectItemCategories.Get( this ); return _projectItemCategories.value; }
			set { if( _projectItemCategories.BeginSet( ref value ) ) { try { ProjectItemCategoriesChanged?.Invoke( this ); } finally { _projectItemCategories.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProjectItemCategories"/> property value changes.</summary>
		public event Action<Product_Store> ProjectItemCategoriesChanged;
		ReferenceField<ProjectItemCategoriesEnum> _projectItemCategories = ProjectItemCategoriesEnum.Uncategorized;

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
		public event Action<Product_Store> TagsChanged;
		ReferenceField<string> _tags = "";

		[DefaultValue( StoreProductLicense.None )]
		public Reference<StoreProductLicense> License
		{
			get { if( _license.BeginGet() ) License = _license.Get( this ); return _license.value; }
			set { if( _license.BeginSet( ref value ) ) { try { LicenseChanged?.Invoke( this ); } finally { _license.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="License"/> property value changes.</summary>
		public event Action<Product_Store> LicenseChanged;
		ReferenceField<StoreProductLicense> _license = StoreProductLicense.None;

		[DefaultValue( 0.0 )]
		public Reference<double> Cost
		{
			get { if( _cost.BeginGet() ) Cost = _cost.Get( this ); return _cost.value; }
			set { if( _cost.BeginSet( ref value ) ) { try { CostChanged?.Invoke( this ); } finally { _cost.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Cost"/> property value changes.</summary>
		public event Action<Product_Store> CostChanged;
		ReferenceField<double> _cost = 0.0;

		[DefaultValue( "1.0.0.0" )]
		public Reference<string> Version
		{
			get { if( _version.BeginGet() ) Version = _version.Get( this ); return _version.value; }
			set { if( _version.BeginSet( ref value ) ) { try { VersionChanged?.Invoke( this ); } finally { _version.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Version"/> property value changes.</summary>
		public event Action<Product_Store> VersionChanged;
		ReferenceField<string> _version = "1.0.0.0";

		[DefaultValue( true )]
		public Reference<bool> CreateScreenshots
		{
			get { if( _createScreenshots.BeginGet() ) CreateScreenshots = _createScreenshots.Get( this ); return _createScreenshots.value; }
			set { if( _createScreenshots.BeginSet( ref value ) ) { try { CreateScreenshotsChanged?.Invoke( this ); } finally { _createScreenshots.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CreateScreenshots"/> property value changes.</summary>
		public event Action<Product_Store> CreateScreenshotsChanged;
		ReferenceField<bool> _createScreenshots = true;

		[DefaultValue( CreateProductsEnum.MainProduct )]
		public Reference<CreateProductsEnum> CreateProducts
		{
			get { if( _createProducts.BeginGet() ) CreateProducts = _createProducts.Get( this ); return _createProducts.value; }
			set { if( _createProducts.BeginSet( ref value ) ) { try { CreateProductsChanged?.Invoke( this ); } finally { _createProducts.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CreateProducts"/> property value changes.</summary>
		public event Action<Product_Store> CreateProductsChanged;
		ReferenceField<CreateProductsEnum> _createProducts = CreateProductsEnum.MainProduct;

		/// <summary>
		/// The list of C# projects to add code of them. Separated by return.
		/// </summary>
		[DefaultValue( addCodeOfProjectsDefault )]
		[Editor( typeof( HCItemTextBoxDropMultiline ), typeof( object ) )]
		[Category( "Code" )]
		public Reference<string> AddCodeOfProjects
		{
			get { if( _addCodeOfProjects.BeginGet() ) AddCodeOfProjects = _addCodeOfProjects.Get( this ); return _addCodeOfProjects.value; }
			set { if( _addCodeOfProjects.BeginSet( ref value ) ) { try { AddCodeOfProjectsChanged?.Invoke( this ); } finally { _addCodeOfProjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AddCodeOfProjects"/> property value changes.</summary>
		public event Action<Product_Store> AddCodeOfProjectsChanged;
		ReferenceField<string> _addCodeOfProjects = addCodeOfProjectsDefault;
		const string addCodeOfProjectsDefault = "Sources\\NeoAxis.CoreExtension\\NeoAxis.CoreExtension.csproj\r\nProject.csproj";

		//!!!!
		//!!!!расширение можно указывать, можно не указывать
		/// <summary>
		/// The list of referenced assembly DLL names to compile sources. Separated by return.
		/// </summary>
		[DefaultValue( referenceAssembliesDefault )]
		[Editor( typeof( HCItemTextBoxDropMultiline ), typeof( object ) )]
		[Category( "Code" )]
		public Reference<string> ReferenceAssemblies
		{
			get { if( _referenceAssemblies.BeginGet() ) ReferenceAssemblies = _referenceAssemblies.Get( this ); return _referenceAssemblies.value; }
			set { if( _referenceAssemblies.BeginSet( ref value ) ) { try { ReferenceAssembliesChanged?.Invoke( this ); } finally { _referenceAssemblies.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ReferenceAssemblies"/> property value changes.</summary>
		public event Action<Product_Store> ReferenceAssembliesChanged;
		ReferenceField<string> _referenceAssemblies = referenceAssembliesDefault;
		const string referenceAssembliesDefault = "";

		//!!!!option to remove comments?

		//!!!!
		///// <summary>
		///// Whether to obfuscated code of the app.
		///// </summary>
		//[DefaultValue( true )]
		//[Category( "Code" )]
		//public Reference<bool> ObfuscateCode
		//{
		//	get { if( _obfuscateCode.BeginGet() ) ObfuscateCode = _obfuscateCode.Get( this ); return _obfuscateCode.value; }
		//	set { if( _obfuscateCode.BeginSet( ref value ) ) { try { ObfuscateCodeChanged?.Invoke( this ); } finally { _obfuscateCode.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ObfuscateCode"/> property value changes.</summary>
		//public event Action<Product_Store> ObfuscateCodeChanged;
		//ReferenceField<bool> _obfuscateCode = true;

		///////////////////////////////////////////////

		///// <summary>
		///// A set of settings for <see cref="Product_Store"/> creation in the editor.
		///// </summary>
		//public class NewObjectSettingsProductStore : NewObjectSettings
		//{
		//	[Category( "Product Store" )]
		//	[DefaultValue( ProductTypeEnum.ProjectItem )]
		//	public ProductTypeEnum ProductType
		//	{
		//		get { return productType; }
		//		set { productType = value; }
		//	}
		//	ProductTypeEnum productType = ProductTypeEnum.ProjectItem;

		//	//!!!!больше свойств добавить?

		//	//

		//	public override bool Creation( NewObjectCell.ObjectCreationContext context )
		//	{
		//		var product = (Product_Store)context.newObject;

		//		product.ProductType = ProductType;

		//		//if( ProductType == ProductTypeEnum.ProjectItem )
		//		//{
		//		//	product.Paths = "";
		//		//	product.AddThisFolder = true;
		//		//	//product.AddCodeOfProjects = "";
		//		//}

		//		return base.Creation( context );
		//	}
		//}

		///////////////////////////////////////////////

		public enum ProductTypeEnum
		{
			ProjectItem,
			//!!!!
			ProjectTemplate,
			App,
			Game,
		}

		///////////////////////////////////////////////

		[Flags]
		public enum ProjectItemCategoriesEnum
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
			Vehicles = 1 << 20,
			Nature = 1 << 21,
			Weapons = 1 << 22,
			UncategorizedModels = 1 << 23,

			Surfaces = 1 << 24,

			BasicContent = 1 << 25,

			FunctionalObjects = 1 << 26,
		}

		/////////////////////////////////////////

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

			Mesh mesh;
			Surface surface;
			Material material;
			Skybox skybox;
			ObjectInSpace objectInSpace;

			ImageComponent texture;
			Viewport viewport;
			ImageComponent textureRead;
			IntPtr imageData;

			Scene scene;

			/////////////////////////////////////////

			public Scene CreateScene( bool enable )
			{
				DetachAndOrDestroyScene();

				scene = ComponentUtility.CreateComponent<Scene>( null, true, enable );

				scene.OctreeEnabled = false;

				//rendering pipeline
				{
					var pipeline = (RenderingPipeline_Basic)scene.CreateComponent( RenderingSystem.RenderingPipelineBasic, -1, false );
					pipeline.Name = "Rendering Pipeline";
					scene.RenderingPipeline = pipeline;

					pipeline.DeferredShading = AutoTrueFalse.False;
					pipeline.LODRange = new RangeI( 0, 0 );
					//pipeline.UseRenderTargets = false;
					pipeline.ShadowDirectionalLightCascades = 4;

					scene.BackgroundColor = new ColorValue( 0.5, 0.5, 0.5 );

					scene.BackgroundColorAffectLighting = 1;
					scene.BackgroundColorEnvironmentOverride = new ColorValue( 0.8, 0.8, 0.8 );

					pipeline.Enabled = true;
				}

				//ambient light
				{
					var light = scene.CreateComponent<Light>();
					light.Name = "Ambient Light";
					light.Type = Light.TypeEnum.Ambient;
					light.Brightness = ReferenceUtility.MakeReference( "Base\\ProjectSettings.component|$Preview\\PreviewAmbientLightBrightness" );
					//light.Brightness = ProjectSettings.Get.PreviewAmbientLightBrightness.Value;
				}

				//directional light
				{
					var light = scene.CreateComponent<Light>();
					light.Name = "Directional Light";
					light.Type = Light.TypeEnum.Directional;
					light.Transform = new Transform( new Vector3( 0, 0, 0 ), Quaternion.FromDirectionZAxisUp( new Vector3( 0, 0, -1 ) ), Vector3.One );
					light.Brightness = ReferenceUtility.MakeReference( "Base\\ProjectSettings.component|$Preview\\PreviewDirectionalLightBrightness" );
					//light.Brightness = ProjectSettings.Get.PreviewDirectionalLightBrightness.Value;
					light.Shadows = false;
					//light.Type = Light.TypeEnum.Point;
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

			protected virtual void Scene_ViewportUpdateGetCameraSettings( Scene scene, Viewport viewport, ref bool processed )
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

				var camera = new Camera();
				//camera.AspectRatio = (double)ViewportControl.Viewport.SizeInPixels.X / (double)ViewportControl.Viewport.SizeInPixels.Y;
				camera.FieldOfView = fov;
				camera.NearClipPlane = Math.Max( distance / 10000, 0.01 );//.1;
				camera.FarClipPlane = Math.Max( 1000, distance * 2 );

				if( objectInSpace?.EditorCameraTransform != null )
					camera.Transform = objectInSpace.EditorCameraTransform;
				else if( mesh?.EditorCameraTransform != null )
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
				//Camera camera = new Camera();
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

				//	Camera camera = new Camera();
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


				//is not best place, because the method is made only for getting camera settings
				//update shadow far distance
				{
					var directionalLight = scene.GetComponent( "Directional Light" ) as Light;
					var pipeline = scene.GetComponent( "Rendering Pipeline" ) as RenderingPipeline_Basic;
					if( directionalLight != null && pipeline != null )
					{
						var farDistance = 1.0;
						foreach( var meshInSpace in scene.GetComponents<MeshInSpace>( checkChildren: true ) )
						{
							var sphere = meshInSpace.SpaceBounds.BoundingSphere;
							var d = ( sphere.Center - viewport.CameraSettings.Position ).Length() + sphere.Radius;
							farDistance = Math.Max( farDistance, d );
						}
						pipeline.ShadowFarDistance = farDistance * 1.15;
					}
				}
			}

			void Init()
			{
				texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
				texture.CreateType = ImageComponent.TypeEnum._2D;
				texture.CreateSize = imageSizeRender;// new Vector2I( imageSizeRender, imageSizeRender );
				texture.CreateMipmaps = false;
				texture.CreateFormat = imageFormat;
				texture.CreateUsage = ImageComponent.Usages.RenderTarget;
				texture.CreateFSAA = 0;
				texture.Enabled = true;

				var renderTexture = texture.Result.GetRenderTarget( 0, 0 );

				viewport = renderTexture.AddViewport( false, true );
				viewport.AllowRenderScreenLabels = false;

				//viewport.UpdateBegin += Viewport_UpdateBegin;

				textureRead = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
				textureRead.CreateType = ImageComponent.TypeEnum._2D;
				textureRead.CreateSize = texture.CreateSize;
				textureRead.CreateMipmaps = false;
				textureRead.CreateFormat = texture.CreateFormat;
				textureRead.CreateUsage = ImageComponent.Usages.ReadBack | ImageComponent.Usages.BlitDestination;
				textureRead.CreateFSAA = 0;
				textureRead.Enabled = true;

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

			public void Generate( Mesh mesh, Stream writeStream, ImageFormat writeImageFormat )//string writeRealFileName )
			{
				this.mesh = mesh;
				Init();

				var scene = CreateScene( false );
				if( mesh != null )
				{
					var objInSpace = scene.CreateComponent<MeshInSpace>();
					objInSpace.Mesh = mesh;

					//enable shadows
					var directionalLight = scene.GetComponent( "Directional Light" ) as Light;
					if( directionalLight != null )
						directionalLight.Shadows = true;
				}
				scene.Enabled = true;

				GenerateGeneral( writeStream, writeImageFormat );// writeRealFileName );
			}

			public void Generate( Surface surface, Stream writeStream, ImageFormat writeImageFormat )//string writeRealFileName )
			{
				this.surface = surface;
				Init();

				var scene = CreateScene( false );
				if( surface != null )
				{
					SurfaceEditorUtility.CreatePreviewObjects( scene, surface );

					//enable shadows
					var directionalLight = scene.GetComponent( "Directional Light" ) as Light;
					if( directionalLight != null )
						directionalLight.Shadows = true;
				}
				scene.Enabled = true;

				GenerateGeneral( writeStream, writeImageFormat );// writeRealFileName );
			}

			public void Generate( Material material, Stream writeStream, ImageFormat writeImageFormat )//string writeRealFileName )
			{
				this.material = material;
				Init();

				var scene = CreateScene( false );

				var mesh = scene.CreateComponent<Mesh>( enabled: false );
				var sphere = mesh.CreateComponent<MeshGeometry_Sphere>();
				sphere.SegmentsHorizontal = 64;
				sphere.SegmentsVertical = 64;
				mesh.Enabled = true;

				var objInSpace = scene.CreateComponent<MeshInSpace>();
				objInSpace.Mesh = mesh;
				objInSpace.ReplaceMaterial = material;

				scene.Enabled = true;

				GenerateGeneral( writeStream, writeImageFormat );// writeRealFileName );
			}

			public void Generate( Skybox skybox, Stream writeStream, ImageFormat writeImageFormat )//string writeRealFileName )
			{
				this.skybox = skybox;
				Init();

				var scene = CreateScene( false );

				var instanceInScene = (Skybox)skybox.Clone();
				scene.AddComponent( instanceInScene );

				scene.Enabled = true;

				GenerateGeneral( writeStream, writeImageFormat );// writeRealFileName );
			}

			public void Generate( ObjectInSpace objectInSpace, Stream writeStream, ImageFormat writeImageFormat )//string writeRealFileName )
			{
				this.objectInSpace = objectInSpace;
				Init();

				ObjectInSpace objectInSpaceInScene = null;

				var scene = CreateScene( false );
				if( objectInSpace != null )
				{
					objectInSpaceInScene = (ObjectInSpace)objectInSpace.Clone();
					scene.AddComponent( objectInSpaceInScene );

					//enable shadows
					var directionalLight = scene.GetComponent( "Directional Light" ) as Light;
					if( directionalLight != null )
						directionalLight.Shadows = true;
				}
				scene.Enabled = true;

				//set animation to the character
				if( objectInSpaceInScene != null )
				{
					var controller = objectInSpaceInScene.GetComponent<MeshInSpaceAnimationController>( checkChildren: true, onlyEnabledInHierarchy: true );
					if( controller != null )
					{
						var animation = ObjectEx.PropertyGet<Animation>( objectInSpaceInScene, "IdleAnimation" );
						if( animation != null )
						{
							var state = new MeshInSpaceAnimationController.AnimationStateClass();
							state.Animations.Add( new MeshInSpaceAnimationController.AnimationStateClass.AnimationItem() { Animation = animation, Speed = 1, AutoRewind = true } );

							controller.SetAnimationState( state, false );
						}
					}
				}

				GenerateGeneral( writeStream, writeImageFormat );// writeRealFileName );
			}

			void GenerateGeneral( Stream writeStream, ImageFormat writeImageFormat )// string writeRealFileName )
			{
#if !DEPLOY
				//generate an image
				{
					viewport.Update( true );

					//clear temp data
					viewport.RenderingContext.MultiRenderTarget_DestroyAll();
					viewport.RenderingContext.DynamicTexture_DestroyAll();

					texture.Result.GetNativeObject( true ).BlitTo( viewport.RenderingContext.CurrentViewNumber, textureRead.Result.GetNativeObject( true ), 0, 0 );

					var demandedFrame = textureRead.Result.GetNativeObject( true ).Read( imageData, 0 );

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
						if( writeImageFormat == ImageFormat.Png )
							resized.Save( writeStream, ImageFormat.Png );
						else if( writeImageFormat == ImageFormat.Jpeg )
						{
							var encoder = ImageCodecInfo.GetImageEncoders().First( codec => codec.FormatID == ImageFormat.Jpeg.Guid );
							var parameters = new EncoderParameters( 1 );
							parameters.Param[ 0 ] = new EncoderParameter( System.Drawing.Imaging.Encoder.Quality, 95L );
							resized.Save( writeStream, encoder, parameters );
						}

						//var ext = Path.GetExtension( writeRealFileName );
						//if( ext == ".png" )
						//	resized.Save( writeRealFileName, ImageFormat.Png );
						//else if( ext == ".jpg" )
						//{
						//	var encoder = ImageCodecInfo.GetImageEncoders().First( codec => codec.FormatID == ImageFormat.Jpeg.Guid );
						//	var parameters = new EncoderParameters( 1 );
						//	parameters.Param[ 0 ] = new EncoderParameter( System.Drawing.Imaging.Encoder.Quality, 95L );
						//	resized.Save( writeRealFileName, encoder, parameters );
						//}
					}
				}

				DetachAndOrDestroyScene();
				Shutdown();
#endif
			}
		}

		///////////////////////////////////////////////

		public override SystemSettings.Platform Platform
		{
			get { return SystemSettings.Platform.Store; }
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Paths ):
					if( ProductType.Value == ProductTypeEnum.ProjectItem )
						skip = true;
					break;

				//!!!!
				case nameof( FileCache ):
				case nameof( ShaderCache ):
					//!!!! if( !IsAppOrGame( ProductType ) )
					skip = true;
					break;

				case nameof( Cost ):
					if( License.Value != StoreProductLicense.PaidPerSeat )
						skip = true;
					break;

				case nameof( CreateScreenshots ):
				case nameof( CreateProducts ):
					if( !ProjectItemCategories.Value.HasFlag( ProjectItemCategoriesEnum.Surfaces ) && !CategoryIsModel( ProjectItemCategories ) && !ProjectItemCategories.Value.HasFlag( ProjectItemCategoriesEnum.Materials ) && !ProjectItemCategories.Value.HasFlag( ProjectItemCategoriesEnum.Environments ) )
						skip = true;
					break;

				case nameof( AddCodeOfProjects ):
				case nameof( ReferenceAssemblies ):
				case nameof( OriginalShaders ):
					if( !IsAppOrGame( ProductType ) )
						skip = true;
					break;

				case nameof( ProjectItemCategories ):
					if( ProductType.Value != ProductTypeEnum.ProjectItem )
						skip = true;
					break;
				}
			}
		}

		static List<string> GetCSProjectFiles( string projectFillPath )
		{
			var result = new List<string>();

			try
			{
				var directoryName = Path.GetDirectoryName( projectFillPath );


				var toInclude = new Dictionary<string, int>();
				{
					var xmldoc2 = new XmlDocument();
					xmldoc2.Load( projectFillPath );

					var mgr2 = new XmlNamespaceManager( xmldoc2.NameTable );
					mgr2.AddNamespace( "df", xmldoc2.DocumentElement.NamespaceURI );

					//EnableDefaultCompileItems
					{
						var defaultCompileItems = true;
						foreach( XmlNode node in xmldoc2.GetElementsByTagName( "EnableDefaultCompileItems" ) )
						{
							if( !string.IsNullOrEmpty( node.InnerText ) )
							{
								defaultCompileItems = bool.Parse( node.InnerText );
								break;
							}
						}

						if( defaultCompileItems )
						{
							foreach( var f in Directory.GetFiles( directoryName, "*.cs", SearchOption.AllDirectories ) )
							{
								var name = f.Replace( directoryName + Path.DirectorySeparatorChar, "" );
								toInclude[ name ] = 1;
							}
						}
					}

					//Compile Include
					{
						var list = xmldoc2.SelectNodes( "//df:Compile", mgr2 );
						foreach( XmlNode node in list )
						{
							var attr = node.Attributes[ "Include" ];
							if( attr != null )
							{
								var name = attr.Value;
								toInclude[ name ] = 1;
							}
						}
					}

					//Compile Remove
					{
						var list = xmldoc2.SelectNodes( "//df:Compile", mgr2 );
						foreach( XmlNode node in list )
						{
							var attr = node.Attributes[ "Remove" ];
							if( attr != null )
							{
								var t = attr.Value;

								if( t.Length >= 2 && t[ t.Length - 2 ] == '*' && t[ t.Length - 1 ] == '*' )
								{
									var t2 = t.Substring( 0, t.Length - 2 );

									again:;
									foreach( var name in toInclude.Keys )
									{
										if( name.Length >= t2.Length && name.Substring( 0, t2.Length ) == t2 )
										{
											toInclude.Remove( name );
											goto again;
										}
									}
								}
								else
									toInclude.Remove( t );
							}
						}
					}

				}

				foreach( var fileName in toInclude.Keys )
				{
					var fullPath = Path.Combine( directoryName, fileName );

					if( Path.GetFileName( fullPath ) != "AssemblyInfo.cs" )
						result.Add( fullPath );
				}


				//var xmldoc = new XmlDocument();
				//xmldoc.Load( fullPath );

				////!!!!
				////EnableDefaultCompileItems

				////!!!!
				////<EmbeddedResource Remove="Packages\**" />
				////<EmbeddedResource Remove="Sources\**" />
				////<EmbeddedResource Remove="User settings\**" />

				////!!!!new format
				////<Compile Remove="obj\**" />
				////<Compile Remove="Properties\Android\**" />


				////!!!!
				////{
				////	var list = xmldoc.SelectNodes( "//Reference" );
				////	foreach( XmlNode node in list )
				////	{
				////		var include = node.Attributes[ "Include" ].Value;
				////		projectFileReferences.Add( include );
				////	}
				////}

				////!!!!
				////{
				////	var list = xmldoc.SelectNodes( "//PackageReference" );
				////	foreach( XmlNode node in list )
				////	{
				////		var include = node.Attributes[ "Include" ].Value;
				////		projectFileReferences.Add( include );
				////	}
				////}

				//{
				//	var list = xmldoc.SelectNodes( "//Compile" );
				//	foreach( XmlNode node in list )
				//	{
				//		var include = node.Attributes[ "Include" ].Value;

				//		result.Add( Path.Combine( directoryName, include ) );
				//	}
				//}

			}
			catch( Exception e )
			{
				throw new Exception( $"Unable to read file \'{projectFillPath}\'. Error: {e.Message}" );
			}

			return result;
		}

		//static string MergeCode( IEnumerable<string> codes )
		//{
		//	var resultText = new StringBuilder( 3000000 );

		//	var usingStrings = new Dictionary<string, int>();

		//	//var counter = 0;

		//	foreach( var code in codes )
		//	{
		//		var lines = code.Replace( "\r", "" ).Split( '\n' );

		//		foreach( var line in lines )
		//		{
		//			if( line.Length > 6 && line.Substring( 0, 6 ) == "using " )
		//			{
		//				if( !usingStrings.ContainsKey( line ) )
		//					usingStrings.Add( line, 0 );
		//			}
		//			else
		//			{
		//				resultText.Append( line );
		//				resultText.Append( "\r\n" );
		//			}
		//		}

		//		//counter++;
		//		//if( counter > 1 )//7 )
		//		//	break;
		//	}

		//	var resultText2 = resultText.ToString();

		//	{
		//		var usingString = "";
		//		foreach( var str in usingStrings.Keys )
		//			usingString += str + "\r\n";
		//		resultText2 = usingString + "\r\n" + resultText2;
		//	}

		//	return resultText2;
		//}

		static void RemoveComments( ref string code )
		{
			var code2 = code;
			//var code2 = code.Replace( "/// ", "/$$/@@/ " );

			var blockComments = @"/\*(.*?)\*/";
			var lineComments = @"//(.*?)\r?\n";
			var strings = @"""((\\[^\n]|[^""\n])*)""";
			var verbatimStrings = @"@(""[^""]*"")+";

			string noComments = Regex.Replace( code2, blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
				me =>
				{
					if( me.Value.StartsWith( "/*" ) || me.Value.StartsWith( "//" ) )
						return me.Value.StartsWith( "//" ) ? Environment.NewLine : "";
					// Keep the literal strings
					return me.Value;
				},
				RegexOptions.Singleline );


			code = noComments;
			//code = noComments.Replace( "/$$/@@/ ", "/// " );
		}

		protected virtual void ProcessCode( ProductBuildInstance buildInstance )
		{
			//!!!!progress percents

			var fileNames = new List<string>( 256 );

			foreach( var fileName in AddCodeOfProjects.Value.Split( '\n', StringSplitOptions.RemoveEmptyEntries ) )
			{
				var fileName2 = fileName.Replace( "\r", "" ).Trim();
				if( fileName2 != "" )
				{
					var fullPath = Path.Combine( VirtualFileSystem.Directories.Project, fileName2 );

					var list = GetCSProjectFiles( fullPath );
					fileNames.AddRange( list );
				}
			}

			if( fileNames.Count != 0 )
			{
				var destFolder = Path.Combine( buildInstance.DestinationFolder, "Code" );
				Directory.CreateDirectory( destFolder );

				for( int n = 0; n < fileNames.Count; n++ )
				{
					var code = File.ReadAllText( fileNames[ n ] );
					RemoveComments( ref code );

					var destFileName = Path.Combine( destFolder, $"Code{n + 1}.cs" );
					File.WriteAllText( destFileName, code );
				}
			}


			//var fileNames = new List<string>( 256 );

			//foreach( var fileName in AddCodeOfProjects.Value.Split( '\n', StringSplitOptions.RemoveEmptyEntries ) )
			//{
			//	var fileName2 = fileName.Replace( "\r", "" ).Trim();
			//	if( fileName2 != "" )
			//	{
			//		var fullPath = Path.Combine( VirtualFileSystem.Directories.Project, fileName2 );

			//		var list = GetCSProjectFiles( fullPath );
			//		fileNames.AddRange( list );
			//	}
			//}

			//if( fileNames.Count != 0 )
			//{
			//	var codes = new List<string>( fileNames.Count );

			//	foreach( var fileName in fileNames )
			//	{
			//		var code = File.ReadAllText( fileName );
			//		RemoveComments2( ref code );

			//		codes.Add( code );
			//	}

			//	var resultCode = MergeCode( codes );

			//	var destFileName = Path.Combine( buildInstance.DestinationFolder, "Code.cs" );
			//	File.WriteAllText( destFileName, resultCode );
			//}
		}

		public override void BuildFunction( ProductBuildInstance buildInstance )
		{
			var authorEmail = "";
			var authorHash = "";
			{
				if( LoginUtility.GetCurrentLicense( out var email, out var hash ) )
				{
					if( LoginUtility.GetRequestedFullLicenseInfo( out var license, out _, out var error2 ) )
					{
						if( !string.IsNullOrEmpty( license ) )
						{
							authorEmail = email;
							authorHash = hash;
						}
					}
				}
			}
			if( string.IsNullOrEmpty( authorEmail ) )
			{
				EditorMessageBox.ShowWarning( "Please login to build store products." );
				return;
			}



			if( ProductType.Value == ProductTypeEnum.ProjectItem )
			{
				//ProjectItem

				//build
				var filesToUpload = new List<string>();
				if( !ProjectItemBuildArchives( buildInstance, authorEmail, filesToUpload ) )
					return;

				//upload
				if( buildInstance.Run )
				{
					foreach( var fileToUpload in filesToUpload )
					{
						var email = authorEmail;
						var hash = authorHash;


						//!!!!невалидный идентификатор

						//!!!!невалидная версия

						//!!!!product
						//!!!!version

						var email64 = StringUtility.EncodeToBase64URL( email );
						var hash64 = StringUtility.EncodeToBase64URL( hash );
						var parameters = $"email={email64}&hash={hash64}";

						var Client = new System.Net.WebClient();
						Client.Headers.Add( "Content-Type", "binary/octet-stream" );


						string resultString = "";

						var notification = ScreenNotifications.ShowSticky( "Uploading to the Store..." );
						try
						{
							var result = Client.UploadFile( EngineInfo.StoreAddress + "/api/product_processing_upload/?" + parameters, "POST", fileToUpload );
							resultString = Encoding.UTF8.GetString( result, 0, result.Length );
						}
						catch( Exception e )
						{
							resultString = e.Message;
						}
						finally
						{
							notification.Close();
						}

						if( resultString == "OK" )
						{
							ScreenNotifications.Show( EditorLocalization.Translate( "Backstage", "The product was uploaded successfully. Now you can check it on the website and submit to publish." ) );
						}
						else
						{
							if( resultString == "" )
								resultString = "Unable to upload. No error message.";
							ScreenNotifications.Show( resultString, true );
						}
					}
				}

			}
			else
			{
				//!!!!is not implemented

				//App, Game, ProjectTemplate

				//copy files
				try
				{
					var paths = GetPaths();
					CopyIncludeExcludePaths( paths, buildInstance, new Range( 0, 0.99 ) );

					if( CheckCancel( buildInstance ) )
						return;

					//process C# code
					ProcessCode( buildInstance );

					if( CheckCancel( buildInstance ) )
						return;


					var license = EnumUtility.GetValueDisplayName( License.Value );

					//prepare Package.info
					{
						var block = new TextBlock();

						//block.SetAttribute( "Identifier", GetIdentifier() );
						block.SetAttribute( "Title", Name );
						block.SetAttribute( "Version", GetVersion() );
						//block.SetAttribute( "Author", authorEmail );// "NeoAxis" );
						block.SetAttribute( "Description", ShortDescription );
						//"ShortDescription"
						block.SetAttribute( "FullDescription", FullDescription.Value );
						block.SetAttribute( "License", license );

						if( License.Value == StoreProductLicense.PaidPerSeat )
							block.SetAttribute( "Cost", Cost.Value.ToString() );

						block.SetAttribute( "Categories", ProductType.Value.ToString() + "s" );

						if( !string.IsNullOrEmpty( Tags.Value ) )
							block.SetAttribute( "Tags", Tags.Value );

						block.SetAttribute( "EngineVersion", EngineInfo.Version );

						var fileName = Path.Combine( buildInstance.DestinationFolder, "Package.info" );
						File.WriteAllText( fileName, block.DumpToString() );
					}

					//!!!!percents

					//make archive
					var zipFileTempFileName = Path.Combine( Path.GetTempPath(), "Temp" + Path.GetRandomFileName() );
					ZipFile.CreateFromDirectory( buildInstance.DestinationFolder, zipFileTempFileName );

					//clear folder
					IOUtility.ClearDirectory( buildInstance.DestinationFolder );

					//copy zip
					var destZipFileName = Path.Combine( buildInstance.DestinationFolder, GetIdentifier() + "-" + GetVersion() + ".neoaxispackage" );
					File.Copy( zipFileTempFileName, destZipFileName );

					//delete temp file
					if( File.Exists( zipFileTempFileName ) )
						File.Delete( zipFileTempFileName );

					//write info json
					{
						var jsonFileName = destZipFileName + ".json";

						//var sw = new StringWriter();
						using( var stream = new MemoryStream() )
						{
							var options = new JsonWriterOptions();
							options.Indented = true;

							using( var writer = new Utf8JsonWriter( stream, options ) )
							{
								//!!!!writer.Formatting = Formatting.Indented;
								writer.WriteStartObject();

								writer.WriteString( "Author", authorEmail );// "NeoAxis" );
								writer.WriteString( "Identifier", GetIdentifier() );
								writer.WriteString( "Title", Name );
								writer.WriteString( "License", license );

								if( License.Value == StoreProductLicense.PaidPerSeat )
									writer.WriteString( "Cost", Cost.Value.ToString() );

								writer.WriteString( "ShortDescription", ShortDescription.Value );
								writer.WriteString( "FullDescription", FullDescription.Value );
								writer.WriteString( "Categories", ProductType.Value.ToString() + "s" );

								if( !string.IsNullOrEmpty( Tags.Value ) )
									writer.WriteString( "Tags", Tags.Value );

								writer.WriteString( "Version", GetVersion() );
								writer.WriteString( "EngineVersion", EngineInfo.Version );

								//writer.WriteEnd();
								writer.WriteEndObject();
							}

							string json = Encoding.UTF8.GetString( stream.ToArray() );
							File.WriteAllText( jsonFileName, json );
						}
					}

				}
				catch( Exception e )
				{
					buildInstance.Error = e.Message;
					buildInstance.State = ProductBuildInstance.StateEnum.Error;
					return;
				}

			}

			//post build event
			if( !PeformPostBuild( buildInstance ) )
				return;
			if( CheckCancel( buildInstance ) )
				return;

			//done
			buildInstance.Progress = 1;
			buildInstance.State = ProductBuildInstance.StateEnum.Success;

			if( CheckCancel( buildInstance ) )
				return;

			if( !buildInstance.Run )
				ShowSuccessScreenNotification();
		}

		[Browsable( false )]
		public override bool SupportsBuildAndRun
		{
			get { return true; }
		}

		public static bool IsAppOrGame( ProductTypeEnum productType )
		{
			return productType == ProductTypeEnum.App || productType == ProductTypeEnum.Game;
		}

		public static bool CategoryIsModel( ProjectItemCategoriesEnum category )
		{
			return ( category & ( ProjectItemCategoriesEnum.Animals | ProjectItemCategoriesEnum.Architecture | ProjectItemCategoriesEnum.Characters | ProjectItemCategoriesEnum.Exterior | ProjectItemCategoriesEnum.Food | ProjectItemCategoriesEnum.Industrial | ProjectItemCategoriesEnum.Interior | ProjectItemCategoriesEnum.Vehicles | ProjectItemCategoriesEnum.Nature | ProjectItemCategoriesEnum.Weapons | ProjectItemCategoriesEnum.UncategorizedModels ) ) != 0;
		}

		static string GetMD5( string input )
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
				result = Name.Replace( ' ', '_' ).Replace( '-', '_' );

				var fileName = ComponentUtility.GetOwnedFileNameOfComponent( this );
				if( !string.IsNullOrEmpty( fileName ) )
					result += "_" + GetMD5( fileName + ":byfilename" ).ToLower().Substring( 0, 12 );
			}

			return result;
		}

		string GetVersion()
		{
			var result = Version.Value;
			if( string.IsNullOrEmpty( result ) )
				result = "1.0.0.0";
			return result;
		}

		static IEnumerable<Enum> GetFlags( Enum e )
		{
			return Enum.GetValues( e.GetType() ).Cast<Enum>().Where( e.HasFlag );
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

		bool ProjectItemBuildArchive( ProductBuildInstance buildInstance, string specifiedFile, string authorEmail, List<string> filesToUpload )
		{
			//get info

			var license = EnumUtility.GetValueDisplayName( License.Value );

			var identifier = GetIdentifier();
			if( !string.IsNullOrEmpty( specifiedFile ) )
				identifier += "_" + Path.GetFileNameWithoutExtension( specifiedFile ).Replace( ' ', '_' );

			var title = Name;// Title.Value;
			if( !string.IsNullOrEmpty( specifiedFile ) )
				title = Path.GetFileNameWithoutExtension( specifiedFile );

			//!!!!

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

			if( !Directory.Exists( buildInstance.DestinationFolder ) )
				Directory.CreateDirectory( buildInstance.DestinationFolder );
			var destFileName = Path.Combine( buildInstance.DestinationFolder, identifier + "-" + GetVersion() + ".neoaxispackage" );

			//check file already exists
			if( File.Exists( destFileName ) )
			{
				var text = $"The file is already exists. Overwrite?\r\n\r\n{destFileName}";
				//var text = $"The file with name \'{destFileName}\' is already exists. Overwrite?";
				if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
					return false;
			}

			try
			{
				//make zip

				if( File.Exists( destFileName ) )
					File.Delete( destFileName );


				using( var archive = ZipFile.Open( destFileName, ZipArchiveMode.Create ) )
				{

					//add files
					foreach( var file in files )
					{
						if( Path.GetExtension( file ) == ".product" )
							continue;

						var entryName = file.Substring( VirtualFileSystem.Directories.Project.Length + 1 );
						//var entryName = file.Substring( sourceDirectory.Length + 1 );
						archive.CreateEntryFromFile( file, entryName );
					}


					//add Package.info
					{
						var block = new TextBlock();

						//block.SetAttribute( "Identifier", identifier );// GetIdentifier() );
						block.SetAttribute( "Title", title );// Title.Value );
						block.SetAttribute( "Version", GetVersion() );
						//block.SetAttribute( "Author", authorEmail );//, "NeoAxis" );
						block.SetAttribute( "Description", ShortDescription );
						//"ShortDescription"

						block.SetAttribute( "FullDescription", FullDescription.Value );

						block.SetAttribute( "License", license );

						if( License.Value == StoreProductLicense.PaidPerSeat )
							block.SetAttribute( "Cost", Cost.Value.ToString() );

						//categories
						{
							var s = "";
							foreach( ProjectItemCategoriesEnum flag in GetFlags( ProjectItemCategories.Value ) )
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

						if( !string.IsNullOrEmpty( Tags.Value ) )
							block.SetAttribute( "Tags", Tags.Value );

						string openAfterInstall;
						if( !string.IsNullOrEmpty( specifiedFile ) )
							openAfterInstall = Path.GetDirectoryName( specifiedFile );
						else
							openAfterInstall = sourceDirectory.Substring( VirtualFileSystem.Directories.Assets.Length + 1 );
						block.SetAttribute( "OpenAfterInstall", openAfterInstall );

						block.SetAttribute( "EngineVersion", EngineInfo.Version );

						//MustRestart = True
						//AddCSharpFilesToProject = "Store\\Simple Level Generator"

						var entry = archive.CreateEntry( "Package.info" );
						using( var entryStream = entry.Open() )
						using( var streamWriter = new StreamWriter( entryStream ) )
							streamWriter.Write( block.DumpToString() );

						//archive.CreateEntryFromFile( packageInfoTempFileName, "Package.info" );
					}


					int triangles = 0;
					int vertices = 0;

					//try to create screenshots
					if( CreateScreenshots )
					{
						var objectInSpaceVirtualFileName = "";
						if( ProjectItemCategories.Value.HasFlag( ProjectItemCategoriesEnum.Components ) )
						{
							var objectInSpaceVirtualFileNames = new List<string>();
							foreach( var file in files )
							{
								var ext = Path.GetExtension( file );
								if( !string.IsNullOrEmpty( ext ) )
								{
									//!!!!

									var ext2 = ext.ToLower().Replace( ".", "" );

									//var resourceClass = resourceType?.ResourceClass;
									//if( resourceClass != null && typeof( ObjectInSpace ).IsAssignableFrom( resourceClass ) )
									//{
									if( ext2 == "objectinspace" || ext2 == "character" || ext2 == "vehicle" )
									{
										var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
										if( !string.IsNullOrEmpty( virtualFileName ) )
											objectInSpaceVirtualFileNames.Add( virtualFileName );
									}
								}
							}
							//if( objectInSpaceVirtualFileNames.Count == 1 )
							if( objectInSpaceVirtualFileNames.Count != 0 )
								objectInSpaceVirtualFileName = objectInSpaceVirtualFileNames[ 0 ];
						}

						if( !string.IsNullOrEmpty( objectInSpaceVirtualFileName ) )
						{
							//ObjectInSpace

							var objectInSpace = ResourceManager.LoadResource<ObjectInSpace>( objectInSpaceVirtualFileName );// importVirtualFileNames[ 0 ] );
							if( objectInSpace != null )
							{
								var generator = new ImageGenerator();

								var entry = archive.CreateEntry( "_ProductLogo.png" );
								using( var entryStream = entry.Open() )
									generator.Generate( objectInSpace, entryStream, ImageFormat.Png );
							}
						}
						else if( ProjectItemCategories.Value.HasFlag( ProjectItemCategoriesEnum.Surfaces ) )
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
								var surface = ResourceManager.LoadResource<Surface>( importVirtualFileNames[ 0 ] );
								if( surface != null )
								{
									var generator = new ImageGenerator();

									var entry = archive.CreateEntry( "_ProductLogo.png" );
									using( var entryStream = entry.Open() )
										generator.Generate( surface, entryStream, ImageFormat.Png );
									//generator.Generate( surface, destFileName + ".logo.png" );
								}
							}
						}
						else if( CategoryIsModel( ProjectItemCategories.Value ) )
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
								var import3D = ResourceManager.LoadResource<Import3D>( importVirtualFileNames[ 0 ] );
								if( import3D != null )
								{
									var mesh = import3D.GetComponent<Mesh>( "Mesh" );
									if( mesh != null )
									{
										var generator = new ImageGenerator();

										var entry = archive.CreateEntry( "_ProductLogo.png" );
										using( var entryStream = entry.Open() )
											generator.Generate( mesh, entryStream, ImageFormat.Png );
										//generator.Generate( mesh, destFileName + ".logo.png" );

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
						else if( ProjectItemCategories.Value.HasFlag( ProjectItemCategoriesEnum.Materials ) )
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
								var material = ResourceManager.LoadResource<Material>( importVirtualFileNames[ 0 ] );
								if( material != null )
								{
									var generator = new ImageGenerator();

									var entry = archive.CreateEntry( "_ProductLogo.png" );
									using( var entryStream = entry.Open() )
										generator.Generate( material, entryStream, ImageFormat.Png );
									//generator.Generate( material, destFileName + ".logo.png" );
								}
							}
						}
						else if( ProjectItemCategories.Value.HasFlag( ProjectItemCategoriesEnum.Environments ) )
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
								var skybox = ResourceManager.LoadResource<Skybox>( importVirtualFileNames[ 0 ] );
								if( skybox != null )
								{
									var generator = new ImageGenerator();

									var entry = archive.CreateEntry( "_ProductLogo.jpg" );
									using( var entryStream = entry.Open() )
										generator.Generate( skybox, entryStream, ImageFormat.Jpeg );
									//generator.Generate( skybox, destFileName + ".logo.jpg" );// png" );
								}
							}
						}
					}


					//write info json
					{
						//var jsonFileName = destFileName + ".json";

						//var sw = new StringWriter();
						using( var stream = new MemoryStream() )
						{
							var options = new JsonWriterOptions();
							options.Indented = true;

							using( var writer = new Utf8JsonWriter( stream, options ) )
							{
								//!!!!writer.Formatting = Formatting.Indented;
								writer.WriteStartObject();

								writer.WriteString( "Author", authorEmail ); //"NeoAxis" );
								writer.WriteString( "Identifier", identifier );// GetIdentifier() );
								writer.WriteString( "Title", title );// Title.Value );

								//var license = EnumExtension.GetValueDisplayName( License.Value );
								//if( string.IsNullOrEmpty( license ) )
								//	license = TypeUtility.DisplayNameAddSpaces( license.ToString() );
								writer.WriteString( "License", license );

								if( License.Value == StoreProductLicense.PaidPerSeat )
									writer.WriteString( "Cost", Cost.Value.ToString() );

								writer.WriteString( "ShortDescription", ShortDescription.Value );
								writer.WriteString( "FullDescription", FullDescription.Value );

								{
									var s = "";
									foreach( ProjectItemCategoriesEnum flag in GetFlags( ProjectItemCategories.Value ) )
									{
										if( flag != 0 )
										{
											if( s.Length != 0 )
												s += ", ";
											s += TypeUtility.DisplayNameAddSpaces( flag.ToString() );
										}
									}
									writer.WriteString( "Categories", s );
									//writer.WriteValue( Categories.Value.ToString() );
								}

								writer.WriteString( "Tags", Tags.Value );
								writer.WriteString( "Version", GetVersion() );
								writer.WriteString( "EngineVersion", EngineInfo.Version );

								if( triangles != 0 )
								{
									writer.WriteNumber( "Triangles", triangles );
									writer.WriteNumber( "Vertices", vertices );
								}

								//writer.WriteEnd();
								writer.WriteEndObject();
							}

							string json = Encoding.UTF8.GetString( stream.ToArray() );

							var entry = archive.CreateEntry( "_Product.json" );
							using( var entryStream = entry.Open() )
							using( var streamWriter = new StreamWriter( entryStream ) )
								streamWriter.Write( json );

							//File.WriteAllText( jsonFileName, json );
						}
					}

				}

				filesToUpload.Add( destFileName );


				////prepare Package.info
				//var packageInfoTempFileName = Path.Combine( Path.GetTempPath(), "Temp3" + Path.GetRandomFileName() );
				////var packageInfoTempFileName = Path.GetTempFileName();
				//{
				//	var block = new TextBlock();

				//	block.SetAttribute( "Identifier", identifier );// GetIdentifier() );
				//	block.SetAttribute( "Title", title );// Title.Value );
				//	block.SetAttribute( "Version", GetVersion() );
				//	//block.SetAttribute( "Author", authorEmail );//, "NeoAxis" );
				//	block.SetAttribute( "Description", ShortDescription );
				//	//"ShortDescription"

				//	block.SetAttribute( "FullDescription", FullDescription.Value );

				//	block.SetAttribute( "License", license );

				//	if( License.Value == StoreProductLicense.PaidPerSeat )
				//		block.SetAttribute( "Cost", Cost.Value.ToString() );

				//	//categories
				//	{
				//		var s = "";
				//		foreach( ProjectItemCategoriesEnum flag in GetFlags( ProjectItemCategories.Value ) )
				//		{
				//			if( flag != 0 )
				//			{
				//				if( s.Length != 0 )
				//					s += ", ";
				//				s += TypeUtility.DisplayNameAddSpaces( flag.ToString() );
				//			}
				//		}
				//		block.SetAttribute( "Categories", s );
				//	}

				//	if( !string.IsNullOrEmpty( Tags.Value ) )
				//		block.SetAttribute( "Tags", Tags.Value );

				//	string openAfterInstall;
				//	if( !string.IsNullOrEmpty( specifiedFile ) )
				//		openAfterInstall = Path.GetDirectoryName( specifiedFile );
				//	else
				//		openAfterInstall = sourceDirectory.Substring( VirtualFileSystem.Directories.Assets.Length + 1 );
				//	block.SetAttribute( "OpenAfterInstall", openAfterInstall );

				//	block.SetAttribute( "EngineVersion", EngineInfo.Version );

				//	//MustRestart = True
				//	//AddCSharpFilesToProject = "Store\\Simple Level Generator"

				//	File.WriteAllText( packageInfoTempFileName, block.DumpToString() );
				//}

				//using( var archive = ZipFile.Open( destFileName, ZipArchiveMode.Create ) )
				//{
				//	foreach( var file in files )
				//	{
				//		if( Path.GetExtension( file ) == ".product" )
				//			continue;

				//		var entryName = file.Substring( VirtualFileSystem.Directories.Project.Length + 1 );
				//		//var entryName = file.Substring( sourceDirectory.Length + 1 );
				//		archive.CreateEntryFromFile( file, entryName );
				//	}

				//	archive.CreateEntryFromFile( packageInfoTempFileName, "Package.info" );
				//}


				//if( File.Exists( packageInfoTempFileName ) )
				//	File.Delete( packageInfoTempFileName );


				//int triangles = 0;
				//int vertices = 0;

				////try to create screenshots
				//if( CreateScreenshots )
				//{
				//	if( ProjectItemCategories.Value.HasFlag( ProjectItemCategoriesEnum.Surfaces ) )
				//	{
				//		//surface

				//		var resourceType = ResourceManager.GetTypeByName( "Surface" );
				//		var importExtensions = new ESet<string>();
				//		foreach( var e in resourceType.FileExtensions )
				//			importExtensions.AddWithCheckAlreadyContained( "." + e );

				//		var importVirtualFileNames = new List<string>();
				//		foreach( var file in files )
				//		{
				//			var ext = Path.GetExtension( file );
				//			if( !string.IsNullOrEmpty( ext ) && importExtensions.Contains( ext ) )
				//			{
				//				var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
				//				if( !string.IsNullOrEmpty( virtualFileName ) )
				//					importVirtualFileNames.Add( virtualFileName );
				//			}
				//		}

				//		if( importVirtualFileNames.Count == 1 )
				//		{
				//			var surface = ResourceManager.LoadResource<Surface>( importVirtualFileNames[ 0 ] );
				//			if( surface != null )
				//			{
				//				var generator = new ImageGenerator();
				//				generator.Generate( surface, destFileName + ".logo.png" );
				//			}
				//		}
				//	}
				//	else if( CategoryIsModel( ProjectItemCategories.Value ) )
				//	{
				//		//model

				//		//find in the folder one import file (FBX, etc). make screenshot of 'Mesh' object inside the import file.

				//		var resourceType = ResourceManager.GetTypeByName( "Import 3D" );
				//		var importExtensions = new ESet<string>();
				//		foreach( var e in resourceType.FileExtensions )
				//			importExtensions.AddWithCheckAlreadyContained( "." + e );

				//		var importVirtualFileNames = new List<string>();
				//		foreach( var file in files )
				//		{
				//			var ext = Path.GetExtension( file );
				//			if( !string.IsNullOrEmpty( ext ) && importExtensions.Contains( ext ) )
				//			{
				//				var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
				//				if( !string.IsNullOrEmpty( virtualFileName ) )
				//					importVirtualFileNames.Add( virtualFileName );
				//			}
				//		}

				//		if( importVirtualFileNames.Count == 1 )
				//		{
				//			var import3D = ResourceManager.LoadResource<Import3D>( importVirtualFileNames[ 0 ] );
				//			if( import3D != null )
				//			{
				//				var mesh = import3D.GetComponent<Mesh>( "Mesh" );
				//				if( mesh != null )
				//				{
				//					var generator = new ImageGenerator();
				//					generator.Generate( mesh, destFileName + ".logo.png" );

				//					//!!!!если не делаем скриншоты, тогда не будет
				//					if( mesh.Result != null )
				//					{
				//						triangles = mesh.Result.ExtractedIndices.Length / 3;
				//						vertices = mesh.Result.ExtractedVerticesPositions.Length;
				//					}
				//				}
				//			}
				//		}
				//	}
				//	else if( ProjectItemCategories.Value.HasFlag( ProjectItemCategoriesEnum.Materials ) )
				//	{
				//		//material

				//		var resourceType = ResourceManager.GetTypeByName( "Material" );
				//		var importExtensions = new ESet<string>();
				//		foreach( var e in resourceType.FileExtensions )
				//			importExtensions.AddWithCheckAlreadyContained( "." + e );

				//		var importVirtualFileNames = new List<string>();
				//		foreach( var file in files )
				//		{
				//			var ext = Path.GetExtension( file );
				//			if( !string.IsNullOrEmpty( ext ) && importExtensions.Contains( ext ) )
				//			{
				//				var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
				//				if( !string.IsNullOrEmpty( virtualFileName ) )
				//					importVirtualFileNames.Add( virtualFileName );
				//			}
				//		}

				//		if( importVirtualFileNames.Count == 1 )
				//		{
				//			var material = ResourceManager.LoadResource<Material>( importVirtualFileNames[ 0 ] );
				//			if( material != null )
				//			{
				//				var generator = new ImageGenerator();
				//				generator.Generate( material, destFileName + ".logo.png" );
				//			}
				//		}
				//	}
				//	else if( ProjectItemCategories.Value.HasFlag( ProjectItemCategoriesEnum.Environments ) )
				//	{
				//		//skybox

				//		var resourceType = ResourceManager.GetTypeByName( "Skybox" );
				//		var importExtensions = new ESet<string>();
				//		foreach( var e in resourceType.FileExtensions )
				//			importExtensions.AddWithCheckAlreadyContained( "." + e );

				//		var importVirtualFileNames = new List<string>();
				//		foreach( var file in files )
				//		{
				//			var ext = Path.GetExtension( file );
				//			if( !string.IsNullOrEmpty( ext ) && importExtensions.Contains( ext ) )
				//			{
				//				var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( file );
				//				if( !string.IsNullOrEmpty( virtualFileName ) )
				//					importVirtualFileNames.Add( virtualFileName );
				//			}
				//		}

				//		if( importVirtualFileNames.Count == 1 )
				//		{
				//			var skybox = ResourceManager.LoadResource<Skybox>( importVirtualFileNames[ 0 ] );
				//			if( skybox != null )
				//			{
				//				var generator = new ImageGenerator();
				//				generator.Generate( skybox, destFileName + ".logo.jpg" );// png" );
				//			}
				//		}
				//	}
				//}


				////write info json
				//{
				//	var jsonFileName = destFileName + ".json";

				//	//var sw = new StringWriter();
				//	using( var stream = new MemoryStream() )
				//	{
				//		var options = new JsonWriterOptions();
				//		options.Indented = true;

				//		using( var writer = new Utf8JsonWriter( stream, options ) )
				//		{
				//			//!!!!writer.Formatting = Formatting.Indented;
				//			writer.WriteStartObject();

				//			writer.WriteString( "Author", authorEmail ); //"NeoAxis" );
				//			writer.WriteString( "Identifier", identifier );// GetIdentifier() );
				//			writer.WriteString( "Title", title );// Title.Value );

				//			//var license = EnumExtension.GetValueDisplayName( License.Value );
				//			//if( string.IsNullOrEmpty( license ) )
				//			//	license = TypeUtility.DisplayNameAddSpaces( license.ToString() );
				//			writer.WriteString( "License", license );

				//			if( License.Value == StoreProductLicense.PaidPerSeat )
				//				writer.WriteString( "Cost", Cost.Value.ToString() );

				//			writer.WriteString( "ShortDescription", ShortDescription.Value );
				//			writer.WriteString( "FullDescription", FullDescription.Value );

				//			{
				//				var s = "";
				//				foreach( ProjectItemCategoriesEnum flag in GetFlags( ProjectItemCategories.Value ) )
				//				{
				//					if( flag != 0 )
				//					{
				//						if( s.Length != 0 )
				//							s += ", ";
				//						s += TypeUtility.DisplayNameAddSpaces( flag.ToString() );
				//					}
				//				}
				//				writer.WriteString( "Categories", s );
				//				//writer.WriteValue( Categories.Value.ToString() );
				//			}

				//			writer.WriteString( "Tags", Tags.Value );
				//			writer.WriteString( "Version", GetVersion() );
				//			writer.WriteString( "EngineVersion", EngineInfo.Version );

				//			if( triangles != 0 )
				//			{
				//				writer.WriteNumber( "Triangles", triangles );
				//				writer.WriteNumber( "Vertices", vertices );
				//			}

				//			//writer.WriteEnd();
				//			writer.WriteEndObject();
				//		}

				//		string json = Encoding.UTF8.GetString( stream.ToArray() );
				//		File.WriteAllText( jsonFileName, json );
				//	}
				//}

			}
			catch( Exception e )
			{
				buildInstance.Error = e.Message;
				buildInstance.State = ProductBuildInstance.StateEnum.Error;
				return false;
			}

			return true;
		}

		bool ProjectItemBuildArchives( ProductBuildInstance buildInstance, string authorEmail, List<string> filesToUpload )
		{
			if( CreateProducts.Value.HasFlag( CreateProductsEnum.MainProduct ) )
			{
				if( !ProjectItemBuildArchive( buildInstance, "", authorEmail, filesToUpload ) )
					return false;
			}

			if( CreateProducts.Value.HasFlag( CreateProductsEnum.SeparateProductForEachMaterial ) )
			{
				var virtualSourceDirectory = Path.GetDirectoryName( ComponentUtility.GetOwnedFileNameOfComponent( this ) );
				foreach( var file in VirtualDirectory.GetFiles( virtualSourceDirectory, "*.material", SearchOption.AllDirectories ) )
				{
					if( !ProjectItemBuildArchive( buildInstance, file, authorEmail, filesToUpload ) )
						return false;
				}
			}

			if( CreateProducts.Value.HasFlag( CreateProductsEnum.SeparateProductForEachEnvironment ) )
			{
				var virtualSourceDirectory = Path.GetDirectoryName( ComponentUtility.GetOwnedFileNameOfComponent( this ) );
				//!!!!другие расширения тоже
				foreach( var file in VirtualDirectory.GetFiles( virtualSourceDirectory, "*.skybox", SearchOption.AllDirectories ) )
				{
					if( !ProjectItemBuildArchive( buildInstance, file, authorEmail, filesToUpload ) )
						return false;
				}
			}

			if( CreateProducts.Value.HasFlag( CreateProductsEnum.SeparateProductForEachModel ) )
			{
				//!!!!
			}

			return true;
		}

		protected override void OnGetPaths( List<string> paths )
		{
			base.OnGetPaths( paths );
			////Paths
			//foreach( var path in Paths.Value.Split( '\n', StringSplitOptions.RemoveEmptyEntries ) )
			//{
			//	var path2 = path.Replace( "\r", "" ).Trim();
			//	if( path2 != "" )
			//		paths.Add( path2 );
			//}

			////AddThisFolder
			//if( AddThisFolder )
			//{
			//	var sourceDirectory = Path.GetDirectoryName( VirtualPathUtility.GetRealPathByVirtual( ComponentUtility.GetOwnedFileNameOfComponent( this ) ) );
			//	paths.Add( sourceDirectory );
			//}

			//exclude cs files from Assets folder
			if( IsAppOrGame( ProductType ) )
			{
				foreach( var virtualPath in VirtualDirectory.GetFiles( "", "*.cs", SearchOption.AllDirectories ) )
				{
					var realPath = VirtualPathUtility.GetRealPathByVirtual( virtualPath );
					paths.Add( "exclude:" + realPath );
				}
			}

			//exclude Shaders folder
			if( OriginalShaders )
				paths.Add( @"exclude:Assets\Base\Shaders" );

			//!!!!
			////Caches
			//paths.Add( "Caches" );
			//if( !ShaderCache )
			//	paths.Add( @"exclude:Caches\ShaderCache" );
			//if( !FileCache )
			//	paths.Add( @"exclude:Caches\Files" );

		}

		public override bool CanBuildFromThread
		{
			get
			{
				//can't build from thread because need prepare preview images
				if( ProductType.Value == ProductTypeEnum.ProjectItem )
					return false;

				return base.CanBuildFromThread;
			}
		}
	}
}
#endif