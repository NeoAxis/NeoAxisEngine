// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Editor
{
	public abstract class PreviewImageGenerator
	{
		//initial
		Viewport viewport;
		object objectOfPreview;

		//scene
		Component_Scene scene;
		bool sceneNeedDispose;

		//camera management
		Bounds cameraInitBounds = new Bounds( -1, -1, -1, 1, 1, 1 );
		Vector3 cameraLookTo;
		bool cameraMode2D;
		double cameraDistance = .01;
		SphericalDirection cameraDirection = new SphericalDirection( -3.83, -.47 );

		//float fontSizeInPixels;
		//Component_Font editorFont;

		//float shadowDistanceInPixels = 1;

		/////////////////////////////////////////

		public PreviewImageGenerator()
		{
			//CalculateFontSize();

			//viewportControl.ViewportCreated += ViewportControl_ViewportCreated;
			//viewportControl.ViewportDestroyed += ViewportControl_ViewportDestroyed;

			//shadowDistanceInPixels = EditorAPI.DPI > 96 ? 2 : 1;
		}

		internal void Init( Viewport viewport, object objectOfPreview )
		{
			this.viewport = viewport;
			this.objectOfPreview = objectOfPreview;
		}

		public virtual bool ClampImage
		{
			get { return true; }
		}

		public Viewport Viewport
		{
			get { return viewport; }
		}

		public object ObjectOfPreview
		{
			get { return objectOfPreview; }
		}

		protected virtual void Update()
		{
		}
		public void PerformUpdate()
		{
			Update();
		}

		//protected virtual void Viewport_UpdateBeforeOutput( Viewport viewport )
		//{
		//UpdateFontSize();
		//}

		//protected virtual void Viewport_UpdateBeforeOutput2( Viewport viewport )
		//{
		//	DrawGetTextInfoLeftTopCorner();
		//}

		//protected virtual void Viewport_UpdateEnd( Viewport viewport )
		//{
		//}

		//protected virtual void ViewportControl_ViewportDestroyed( EngineViewportControl sender )
		//{
		//}

		//protected override void OnDestroy()
		//{
		//	DetachAndOrDestroyScene();

		//	base.OnDestroy();
		//}

		public Component_Scene Scene
		{
			get { return scene; }
			set { scene = value; }
		}

		public bool SceneNeedDispose
		{
			get { return sceneNeedDispose; }
			set { sceneNeedDispose = value; }
		}

		public Component_Scene CreateScene( bool enable )
		{
			DetachAndOrDestroyScene();

			scene = ComponentUtility.CreateComponent<Component_Scene>( null, true, enable );
			sceneNeedDispose = true;

			//!!!!что еще отключать?
			scene.OctreeEnabled = false;

			//rendering pipeline
			{
				var pipeline = (Component_RenderingPipeline)scene.CreateComponent( RenderingSystem.RenderingPipelineDefault, -1, false );
				scene.RenderingPipeline = pipeline;

				//!!!!что еще отключать?
				pipeline.DeferredShading = AutoTrueFalse.False;
				pipeline.LODRange = new RangeI( 0, 0 );
				//pipeline.UseRenderTargets = false;

				scene.BackgroundColor = new ColorValue( 0, 0, 0, 0 );
				scene.BackgroundColorAffectLighting = 1;
				//scene.BackgroundColorAffectLighting = 0.5;
				//scene.BackgroundColorEnvironmentOverride = new ColorValue( 0.5, 0.5, 0.5 );
				//scene.BackgroundColorEnvironmentOverride = new ColorValue( 1, 1, 1 );
				//scene.BackgroundColorEnvironmentOverride = new ColorValue( 0.75, 0.75, 0.75 );
				scene.BackgroundColorEnvironmentOverride = new ColorValue( 0.8, 0.8, 0.8 );
				//scene.BackgroundColorEnvironmentOverride = new ColorValue( 0, 0, 0, 0 );

				//var backgroundEffects = pipeline.CreateComponent<Component>();
				//backgroundEffects.Name = "Background Effects";

				//var vignetting = backgroundEffects.CreateComponent<Component_RenderingEffect_Vignetting>();
				//if( EditorAPI.DarkTheme )
				//	vignetting.Color = new ColorValue( 45.0 / 255 * c2, 45.0 / 255 * c2, 45.0 / 255 * c2 );
				//else
				//	vignetting.Color = new ColorValue( 24.0 / 255 * c2, 48.0 / 255 * c2, 72.0 / 255 * c2 );
				//vignetting.Radius = 2;

				//var noise = backgroundEffects.CreateComponent<Component_RenderingEffect_Noise>();
				//noise.Multiply = new Range( 0.9, 1.1 );

				//var sceneEffects = pipeline.CreateComponent<Component>();
				//sceneEffects.Name = "Scene Effects";

				////antialiasing
				//var toLDRType = MetadataManager.GetType( "NeoAxis.Component_RenderingEffect_ToLDR" );
				//var antialiasingType = MetadataManager.GetType( "NeoAxis.Component_RenderingEffect_Antialiasing" );
				//if( toLDRType != null && antialiasingType != null )
				//{
				//	sceneEffects.CreateComponent( toLDRType );
				//	sceneEffects.CreateComponent( antialiasingType );
				//}

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
				light.Shadows = false;
				//light.Type = Component_Light.TypeEnum.Point;
				//light.Transform = new Transform( new Vec3( 0, 0, 2 ), Quat.Identity, Vec3.One );
			}

			//!!!!как когда внешне сцена цепляется
			scene.ViewportUpdateGetCameraSettings += Scene_ViewportUpdateGetCameraSettings;

			//connect scene to viewport
			if( Viewport != null )
				Viewport.AttachedScene = scene;

			return scene;
		}

		public void DetachAndOrDestroyScene()
		{
			if( scene != null )
			{
				if( Viewport != null )
					Viewport.AttachedScene = null;

				if( sceneNeedDispose )
					scene.Dispose();
				scene = null;
				sceneNeedDispose = false;
			}
		}

		protected virtual void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
		{
			if( !cameraMode2D )
			{
				var cameraPosition = cameraLookTo - cameraDirection.GetVector() * cameraDistance;
				var center = cameraLookTo;

				Vector3 from = cameraPosition;//center + cameraDirection.GetVector() * cameraDistance;
				Vector3 to = center;
				Degree fov = 40;//!!!! 65;// 75;

				//!!!!
				Component_Camera camera = new Component_Camera();
				camera.AspectRatio = (double)viewport.SizeInPixels.X / (double)viewport.SizeInPixels.Y;
				camera.FieldOfView = fov;
				camera.NearClipPlane = Math.Max( cameraDistance / 10000, 0.01 );//.1;
				camera.FarClipPlane = Math.Max( 1000, cameraDistance * 2 );

				camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );
				//camera.Position = from;
				//camera.Direction = ( to - from ).GetNormalize();

				camera.FixedUp = Vector3.ZAxis;
				viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, camera );

				////!!!!в методе больше параметров
				//double aspect = (double)viewport.SizeInPixels.X / (double)viewport.SizeInPixels.Y;
				//var settings = new Viewport.CameraSettingsClass( viewport, aspect, fov, .1f, 1000, from, ( to - from ).GetNormalize(), Vec3.ZAxis );
				//viewport.CameraSettings = settings;
			}
			else
			{
				var from = cameraLookTo + new Vector3( 0, 0, scene.CameraEditor2DPositionZ );
				var to = cameraLookTo;

				Component_Camera camera = new Component_Camera();
				camera.AspectRatio = (double)viewport.SizeInPixels.X / (double)viewport.SizeInPixels.Y;
				camera.NearClipPlane = 0.01;// Math.Max( cameraInitialDistance / 10000, 0.01 );//.1;
				camera.FarClipPlane = 1000;// Math.Max( 1000, cameraInitialDistance * 2 );
				camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.YAxis ) );
				camera.Projection = ProjectionType.Orthographic;
				camera.FixedUp = Vector3.YAxis;
				//!!!!need consider size by X
				camera.Height = cameraInitBounds.GetSize().Y * 1.4;

				viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, camera );
			}

			processed = true;
		}

		public void SetCameraByBounds( Bounds bounds, double distanceScale = 1, bool mode2D = false )
		{
			cameraInitBounds = bounds;
			cameraLookTo = bounds.GetCenter();
			cameraMode2D = mode2D;

			double maxGararite = Math.Max( Math.Max( bounds.GetSize().X, bounds.GetSize().Y ), bounds.GetSize().Z );
			double distance = maxGararite * distanceScale;

			//cameraPosition = cameraLookAtForCenteredMode - cameraDirection.GetVector() * distance;

			cameraDistance = distance;

			//gridHeight = bounds.Minimum.Z - .01f;
		}

		public Vector3 CameraLookTo
		{
			get { return cameraLookTo; }
			set { cameraLookTo = value; }
		}

		public double CameraDistance
		{
			get { return cameraDistance; }
			set { cameraDistance = value; }
		}

		public SphericalDirection CameraDirection
		{
			get { return cameraDirection; }
			set { cameraDirection = value; }
		}

		//protected virtual void GetTextInfoLeftTopCorner( List<string> lines )
		//{
		//}

		//void DrawGetTextInfoLeftTopCorner()
		//{
		//	//!!!!отключать?

		//	var lines = new List<string>();
		//	GetTextInfoLeftTopCorner( lines );
		//	var offset = new Vector2( GetFontSize() * Viewport.CanvasRenderer.AspectRatioInv * 0.5, GetFontSize() * 0.3 );
		//	AddTextLinesWithShadow( null, GetFontSize(), lines, new Rectangle( offset.X, offset.Y, 1, 1 ), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1, 0.7 ) );
		//}

		//public double GetFontSize()
		//{
		//	double fontSizeInPixels = 12.0 * EditorAPI.DPIScale;//DpiHelper.Default.DpiScaleFactor;
		//	fontSizeInPixels = (int)fontSizeInPixels;

		//	var renderer = Viewport.CanvasRenderer;

		//	int height = renderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y;
		//	float screenCellSize = (float)fontSizeInPixels / (float)height;
		//	float demandFontHeight = screenCellSize;// * GetZoom();

		//	return demandFontHeight;
		//}

		//void CalculateFontSize()
		//{
		//	fontSizeInPixels = 12;

		//	float dpi = EditorAPI.DPI;
		//	if( dpi > 96 )
		//	{
		//		fontSizeInPixels *= dpi / 96;
		//		fontSizeInPixels = (int)fontSizeInPixels;
		//	}
		//}

		//[Browsable( false )]
		//public float FontSizeInPixels
		//{
		//	get { return fontSizeInPixels; }
		//	set { fontSizeInPixels = value; }
		//}

		//[Browsable( false )]
		//public EngineFont EditorFont
		//{
		//	get { return editorFont; }
		//}

		//void UpdateFontSize()
		//{
		//	var renderer = ViewportControl.Viewport.CanvasRenderer;

		//	int height = renderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y;
		//	float screenCellSize = (float)fontSizeInPixels / (float)height;
		//	float demandFontHeight = screenCellSize;// * GetZoom();

		//	if( editorFont == null || editorFont.Height != demandFontHeight )
		//		editorFont = EngineFontManager.Instance.LoadFont( "Default", demandFontHeight );
		//}

		////!!!!!так? везде так
		//public void AddTextWithShadow( Component_Font font, double fontSize, string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		//{
		//	var renderer = Viewport.CanvasRenderer;

		//	if( font == null || font.Disposed )
		//		font = renderer.DefaultFont;
		//	if( font == null || font.Disposed )
		//		return;
		//	//if( font == null )
		//	//	font = EditorFont;
		//	if( fontSize < 0 )
		//		fontSize = renderer.DefaultFontSize;

		//	Vector2 shadowOffset = shadowDistanceInPixels / Viewport.SizeInPixels.ToVector2();
		//	renderer.AddText( font, fontSize, text, position + shadowOffset, horizontalAlign, verticalAlign, new ColorValue( 0, 0, 0, color.Alpha / 2 ) );
		//	renderer.AddText( font, fontSize, text, position, horizontalAlign, verticalAlign, color );
		//}

		//public void AddTextWithShadow( string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		//{
		//	AddTextWithShadow( null, -1, text, position, horizontalAlign, verticalAlign, color );
		//}

		//public void AddTextLinesWithShadow( Component_Font font, double fontSize, IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		//{
		//	if( lines.Count == 0 )
		//		return;

		//	var renderer = Viewport.CanvasRenderer;

		//	if( font == null || font.Disposed )
		//		font = renderer.DefaultFont;
		//	if( font == null || font.Disposed )
		//		return;
		//	//if( font == null )
		//	//	font = EditorFont;
		//	if( fontSize < 0 )
		//		fontSize = renderer.DefaultFontSize;

		//	Vector2 shadowOffset = shadowDistanceInPixels / Viewport.SizeInPixels.ToVector2();
		//	float linesHeight = (float)lines.Count * (float)fontSize;

		//	double posY = 0;
		//	switch( verticalAlign )
		//	{
		//	case EVerticalAlignment.Top:
		//		posY = rectangle.Top;
		//		break;
		//	case EVerticalAlignment.Center:
		//		posY = rectangle.Top + ( rectangle.Size.Y - linesHeight ) / 2;
		//		break;
		//	case EVerticalAlignment.Bottom:
		//		posY = rectangle.Bottom - linesHeight;
		//		break;
		//	}

		//	for( int n = 0; n < lines.Count; n++ )
		//	{
		//		string line = lines[ n ];

		//		double posX = 0;
		//		switch( horizontalAlign )
		//		{
		//		case EHorizontalAlignment.Left:
		//			posX = rectangle.Left;
		//			break;
		//		case EHorizontalAlignment.Center:
		//			posX = rectangle.Left + ( rectangle.Size.X - font.GetTextLength( fontSize, renderer, line ) ) / 2;
		//			break;
		//		case EHorizontalAlignment.Right:
		//			posX = rectangle.Right - font.GetTextLength( fontSize, renderer, line );
		//			break;
		//		}

		//		Vector2 position = new Vector2( posX, posY );

		//		renderer.AddText( font, fontSize, line, position + shadowOffset, EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 0, 0, 0, color.Alpha / 2 ) );
		//		renderer.AddText( font, fontSize, line, position, EHorizontalAlignment.Left, EVerticalAlignment.Top, color );
		//		posY += fontSize;
		//	}
		//}

		//public void AddTextLinesWithShadow( IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		//{
		//	AddTextLinesWithShadow( null, -1, lines, rectangle, horizontalAlign, verticalAlign, color );
		//}

		//public int AddTextWordWrapWithShadow( Component_Font font, double fontSize, string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		//{
		//	var renderer = Viewport.CanvasRenderer;

		//	if( font == null || font.Disposed )
		//		font = renderer.DefaultFont;
		//	if( font == null || font.Disposed )
		//		return 0;
		//	//if( font == null )
		//	//	font = EditorFont;
		//	if( fontSize < 0 )
		//		fontSize = renderer.DefaultFontSize;

		//	var items = font.GetWordWrapLines( fontSize, renderer, text, rectangle.Size.X );

		//	string[] lines = new string[ items.Length ];
		//	for( int n = 0; n < lines.Length; n++ )
		//		lines[ n ] = items[ n ].Text;

		//	AddTextLinesWithShadow( font, fontSize, lines, rectangle, horizontalAlign, verticalAlign, color );

		//	return lines.Length;
		//}

		//public int AddTextWordWrapWithShadow( string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		//{
		//	return AddTextWordWrapWithShadow( null, -1, text, rectangle, horizontalAlign, verticalAlign, color );
		//}

	}
}
