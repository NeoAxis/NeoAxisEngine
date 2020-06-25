// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Input;

namespace NeoAxis
{
	/// <summary>
	/// Viewport class, i.e. a rendering region on a render target.
	/// </summary>
	public sealed partial class Viewport
	{
		//!!!!!чтобы не пересоздавалось когда не нужно

		//internal unsafe OgreViewport* realObject;
		internal RenderTarget parent;

		bool disposed;

		CameraSettingsClass cameraSettings;

		//!!!!!
		//bool enable3DSceneRendering = true;

		Rectangle dimensions = new Rectangle( 0, 0, 1, 1 );
		bool dimensionsDefault = true;

		ModeEnum mode = ModeEnum.Default;
		Component_Scene attachedScene;

		internal Simple3DRendererImpl simple3DRenderer;

		//!!!!!что у неё вызывать еще?
		CanvasRendererImpl canvasRenderer;
		//!!!!!что у неё вызывать еще?
		UIContainer uiContainer;

		//user interaction
		static EKeys[] allKeys;
		object lockerKeysMouse = new object();
		bool[] mouseButtons = new bool[ 5 ];
		bool[] keys;
		Vector2 mousePosition = new Vector2( -10000, -10000 );
		bool mouseRelativeMode;
		Vector2 restoreMousePosAfterRelativeMode;
		bool mouseRelativeModeSkipOneMouseMove;

		//bool focused;

		//!!!!тут? так?
		//!!!!!когда сбрасывать? где еще подобное сбрасывать?
		double lastUpdateTime;
		double lastUpdateTimeStep;
		double previousUpdateTime;

		Matrix4? viewMatrixPreviousFrame;
		Matrix4? projectionMatrixPreviousFrame;

		ViewportRenderingContext renderingContext;
		Component_RenderingPipeline renderingPipelineCreated;

		/// <summary>
		/// Represents an item for <see cref="LastFrameScreenLabels"/> list.
		/// </summary>
		public class LastFrameScreenLabelItem
		{
			//!!!!обязательно ли Object in space
			public Component_ObjectInSpace ObjectInSpace;
			public RectangleF ScreenRectangle;
			public ColorValue Color;
		}
		List<LastFrameScreenLabelItem> lastFrameScreenLabels = new List<LastFrameScreenLabelItem>();

		//!!!!new. так?
		bool allowRenderScreenLabels = true;

		ColorValue backgroundColorDefault = new ColorValue( 0, 0, 0 );

		///////////////////////////////////////////

		public enum ModeEnum
		{
			Default,
			ReflectionProbeCubemap,
		}

		/////////////////////////////////////////

		//!!!!!

		public delegate void UpdateDefaultDelegate( Viewport viewport );
		//public event Update1_DefaultDelegate Update1_SetCameraSettings;
		//public static event Update1_DefaultDelegate AllViewports_Update1_SetCameraSettings;

		//public delegate void Update2_GetRenderingDataDelegate( Viewport viewport, RenderingDataClass sceneRenderingData );
		//public event Update2_GetRenderingDataDelegate Update2_GetRenderingData;
		//public static event Update2_GetRenderingDataDelegate AllViewports_Update2_GetRenderingData;

		//public delegate void Update3_BeforeVisualizationDelegate( Viewport viewport, RenderingDataClass sceneRenderingData );
		//public event Update3_BeforeVisualizationDelegate Update3_BeforeVisualization;
		//public static event Update3_BeforeVisualizationDelegate AllViewports_Update3_BeforeVisualization;

		public event UpdateDefaultDelegate UpdateBefore;
		public static event UpdateDefaultDelegate AllViewports_UpdateBefore;

		public event UpdateDefaultDelegate UpdateBegin;
		public static event UpdateDefaultDelegate AllViewports_UpdateBegin;

		//!!!!new
		//!!!!name
		public delegate void UpdateGetObjectInSceneRenderingContextDelegate( Viewport viewport, ref Component_ObjectInSpace.RenderingContext context );
		public event UpdateGetObjectInSceneRenderingContextDelegate UpdateGetObjectInSceneRenderingContext;
		public static event UpdateGetObjectInSceneRenderingContextDelegate AllViewports_UpdateGetObjectInSceneRenderingContext;

		public event UpdateDefaultDelegate UpdateBeforeOutput;
		public static event UpdateDefaultDelegate AllViewports_UpdateBeforeOutput;

		public event UpdateDefaultDelegate UpdateEnd;
		public static event UpdateDefaultDelegate AllViewports_UpdateEnd;

		///////////////////////////////////////////

		internal Viewport()// OgreViewport* realObject )
		{
			cameraSettings = new CameraSettingsClass( this, 1, 75, 0.1, 10000, Vector3.Zero, Vector3.XAxis, Vector3.ZAxis, ProjectionType.Perspective, 1, 1, 1 );

			//this.realObject = realObject;

			lock( RenderingSystem.viewports )
				RenderingSystem.viewports.Add( this );
			//lock( RendererWorld.viewports )
			//	RendererWorld.viewports.Add( (IntPtr)realObject, this );

			//!!!!было
			//unsafe
			//{
			//	OgreViewport.setClearEveryFrame( realObject, false, (uint)( FrameBufferTypes.Color | FrameBufferTypes.Depth | FrameBufferTypes.Stencil ) );
			//	//OgreViewport.setClearEveryFrame( realObject, clearEveryFrame, (uint)clearEveryFrameBuffers );

			//	ColorValue black = new ColorValue( 0, 0, 0 );
			//	OgreViewport.setBackgroundColour( realObject, ref black );
			//}

			//allKeys
			if( allKeys == null )
			{
				Array values = Enum.GetValues( typeof( EKeys ) );
				allKeys = new EKeys[ values.Length ];
				for( int n = 0; n < allKeys.Length; n++ )
					allKeys[ n ] = (EKeys)values.GetValue( n );
			}
		}

		internal void OnAdd( bool createSimple3DRenderer, bool createCanvasRenderer )
		{
			if( createSimple3DRenderer )
				simple3DRenderer = new Simple3DRendererImpl( this );

			if( createCanvasRenderer )
			{
				canvasRenderer = new CanvasRendererImpl( this );

				uiContainer = new UIContainer( this );
				ComponentUtility.CreateHierarchyControllerForRootComponent( uiContainer, null, true );//, true );

				//!!!!вызывать все методы для controlManager
			}

			UpdateAspectRatio();
		}

		internal void UpdateAspectRatio()
		{
			if( SizeInPixels.X != 0 && SizeInPixels.Y != 0 )
			{
				double aspectRatio = (double)SizeInPixels.X / (double)SizeInPixels.Y;

				//!!!!было
				//if( Camera != null )
				//	Camera.AspectRatio = aspectRatio;

				if( canvasRenderer != null )
					canvasRenderer.SetAspectRatio( (float)aspectRatio );
			}
		}

		/// <summary>
		/// Occurs before object is disposed.
		/// </summary>
		public event Action<Viewport> Disposing;

		/// <summary>Releases the resources that are used by the object.</summary>
		public void Dispose()
		{
			unsafe
			{
				//if( realObject != null )
				{
					//after shutdown check
					if( RenderingSystem.Disposed )
					{
						//waiting for .NET Standard 2.0
						Log.Fatal( "Renderer: Dispose after Shutdown." );
						//Log.Fatal( "Renderer: Dispose after Shutdown: {0}()", System.Reflection.MethodInfo.GetCurrentMethod().Name );
					}

					Disposing?.Invoke( this );

					RenderingPipelineDestroyCreated();

					//!!!!так? т.е. каждому виюпорту - свой инстанс класса?
					if( renderingContext != null )
					{
						renderingContext.Dispose();
						renderingContext = null;
					}

					uiContainer?.Dispose();
					uiContainer = null;

					if( canvasRenderer != null )
					{
						canvasRenderer.Dispose();
						canvasRenderer = null;
					}

					if( simple3DRenderer != null )
					{
						simple3DRenderer.Dispose();
						simple3DRenderer = null;
					}

					if( parent != null )
					{
						//OgreRenderTarget.removeViewport( parent.realObject, realObject );

						parent.viewports.Remove( this );
						parent = null;
					}

					lock( RenderingSystem.viewports )
						RenderingSystem.viewports.Remove( this );
					//lock( RendererWorld.viewports )
					//	RendererWorld.viewports.Remove( (IntPtr)realObject );

					//realObject = null;
				}
			}

			disposed = true;
		}

		public bool Disposed
		{
			get { return disposed; }
		}

		///// <summary>
		///// Determines if the viewport is cleared before every frame.
		///// </summary>
		//public void GetClearEveryFrame( out bool clear, out FrameBufferTypes buffers )
		//{
		//	clear = clearEveryFrame;
		//	buffers = clearEveryFrameBuffers;
		//}

		///// <summary>
		///// Determines whether to clear the viewport before rendering.
		///// </summary>
		///// <param name="clear">Whether or not to clear any buffers.</param>
		///// <param name="buffers">
		///// One or more values from <see cref="FrameBufferTypes"/> denoting
		///// which buffers to clear, if clear is set to true. Note you should
		///// not clear the stencil buffer here unless you know what you're doing.
		///// </param>
		///// <remarks>
		///// You can use this method to set which buffers are cleared
		///// (if any) before rendering every frame.
		///// </remarks>
		//public void SetClearEveryFrame( bool clear, FrameBufferTypes buffers )
		//{
		//	if( clearEveryFrame == clear && clearEveryFrameBuffers == buffers )
		//		return;
		//	clearEveryFrame = clear;
		//	clearEveryFrameBuffers = buffers;

		//	unsafe
		//	{
		//		OgreViewport.setClearEveryFrame( realObject, clearEveryFrame, (uint)clearEveryFrameBuffers );
		//	}
		//}

		//unsafe internal void UpdateNativeBackgroundColor()
		//{
		//	OgreViewport.setBackgroundColour( realObject, ref backgroundColor );
		//}

		///// <summary>
		///// Gets or sets the initial background colour of the viewport (before rendering).
		///// </summary>
		//public ColorValue BackgroundColor
		//{
		//	get { return backgroundColor; }
		//	set
		//	{
		//		if( backgroundColor == value )
		//			return;
		//		backgroundColor = value;
		//		UpdateNativeBackgroundColor();
		//	}
		//}

		/// <summary>
		/// Gets or sets the dimensions (after creation).
		/// </summary>
		public Rectangle Dimensions
		{
			get { return dimensions; }
			set
			{
				if( dimensions == value )
					return;
				dimensions = value;
				dimensionsDefault = dimensions == new Rectangle( 0, 0, 1, 1 );
				unsafe
				{
					RectangleF r = dimensions.ToRectangleF();
					//!!!!
					//OgreViewport.setDimensions( realObject, ref r );
				}
			}
		}

		/// <summary>
		/// Gets the dimensions of the viewport, a value in pixels.
		/// </summary>
		public RectangleI DimensionsInPixels
		{
			get
			{
				if( dimensionsDefault )
					return new RectangleI( Vector2I.Zero, parent.Size );
				else
				{
					//epsilon?
					return ( parent.Size.ToVector2() * dimensions ).ToRectangleI();
				}
			}
		}

		public Vector2I SizeInPixels
		{
			get
			{
				if( dimensionsDefault )
					return parent.Size;
				else
					return DimensionsInPixels.Size;
			}
		}

		/// <summary>
		/// Gets last used camera settings for this viewport.
		/// </summary>
		public CameraSettingsClass CameraSettings
		{
			get { return cameraSettings; }
			set { cameraSettings = value; }
		}

		/// <summary>
		/// Gets simple 3D renderer.
		/// </summary>
		public Simple3DRenderer Simple3DRenderer
		{
			get { return simple3DRenderer; }
		}

		/// <summary>
		/// Gets GUI renderer for this viewport.
		/// </summary>
		public CanvasRenderer CanvasRenderer
		{
			get { return canvasRenderer; }
		}

		/// <summary>
		/// Gets the root of GUI controls of this viewport.
		/// </summary>
		public UIContainer UIContainer
		{
			get { return uiContainer; }
		}

		/// <summary>
		/// Gets the parent render target.
		/// </summary>
		public RenderTarget Parent
		{
			get { return parent; }
		}

		///// <summary>
		///// Gets or sets this viewport whether it should display shadows.
		///// </summary>
		///// <remarks>
		///// This setting enables you to disable shadow rendering for a given viewport. The global
		///// shadow technique set on SceneManager still controls the type and nature of shadows,
		///// but this flag can override the setting so that no shadows are rendered for a given
		///// viewport to save processing time where they are not required.
		///// </remarks>
		//public bool ShadowsEnabled
		//{
		//	get { return shadowsEnabled; }
		//	set
		//	{
		//		if( shadowsEnabled == value )
		//			return;
		//		shadowsEnabled = true;
		//		unsafe
		//		{
		//			OgreViewport.setShadowsEnabled( realObject, value );
		//		}
		//	}
		//}

		//public bool Visible
		//{
		//	get { return visible; }
		//	set { visible = value; }
		//}

		public ModeEnum Mode
		{
			get { return mode; }
			set { mode = value; }
		}

		public Component_Scene AttachedScene
		{
			get { return attachedScene; }
			set { attachedScene = value; }
		}

		//public bool Enable3DSceneRendering
		//{
		//	get { return enable3DSceneRendering; }
		//	set { enable3DSceneRendering = value; }
		//}

		//public ObjectsVisibilityLevels ObjectsVisibilityLevel
		//{
		//	get { return objectsVisibilityLevel; }
		//	set { objectsVisibilityLevel = value; }
		//}

		public ViewportRenderingContext RenderingContext
		{
			get { return renderingContext; }
			set { renderingContext = value; }
		}

		//unsafe void RenderingData_ApplyToNative( RenderingDataClass renderingData )
		//{
		//	////!!!!несколько раз прокидываются объекты?

		//	////!!!!!тут по сути уже конечные данные. нельзя ведь EngineDebugSettings. тут просто копируется DataForRendering. в ней все настройки

		//	////!!!!!Debug.DrawShadowDebugging
		//	//MyOgreSceneManager.setDrawShadowDebugging( SceneManager.realObject, EngineSettings.Debug.DrawShadowDebugging );
		//	//OgreRenderSystem.setAllowHardwareInstancing( RendererWorld.realRoot, renderingData.allowHardwareInstancing );

		//	////!!!!!!
		//	////совсем убрать параметр?
		//	////	  //!!!!!по сути нельзя свойство менять
		//	////viewport.Camera.PolygonMode = EngineSettings.DrawInWireframeMode ? PolygonMode.Wireframe : PolygonMode.Solid;
		//	////OgreCamera.setPolygonMode( viewport.Camera.realObject, renderingData.polygonMode );

		//	////!!!!!что еще?

		//	////fog
		//	//MyOgreSceneManager.setFog( SceneManager.realObject, renderingData.fogMode, ref renderingData.fogColor, renderingData.fogExpDensity, renderingData.fogLinearStart, renderingData.fogLinearEnd );

		//	////lighting
		//	//MyOgreSceneManager.setAmbientLight( SceneManager.realObject, ref renderingData.ambientLight );

		//	////shadows
		//	//{
		//	//	//!!!!!
		//	//	//public bool drawShadows = true;
		//	//	////!!!!!//shadowTechnique и другие

		//	//	ColorValue shadowColor = new ColorValue( renderingData.shadowIntensity, renderingData.shadowIntensity, renderingData.shadowIntensity );
		//	//	MyOgreSceneManager.setShadowColour( SceneManager.realObject, ref shadowColor );
		//	//	MyOgreSceneManager.setShadowFarDistance( SceneManager.realObject, renderingData.shadowFarDistance );
		//	//	MyOgreSceneManager.setShadowTextureFadeStart( SceneManager.realObject, renderingData.shadowTextureFadeStart );

		//	//	//PSSM split distances
		//	//	float[] splitDistances = new float[ 4 ];
		//	//	splitDistances[ 0 ] = Camera.NearClipDistance;
		//	//	splitDistances[ 1 ] = renderingData.shadowPSSMSplitFactors[ 0 ] * renderingData.shadowFarDistance;
		//	//	splitDistances[ 2 ] = renderingData.shadowPSSMSplitFactors[ 1 ] * renderingData.shadowFarDistance;
		//	//	splitDistances[ 3 ] = renderingData.shadowFarDistance;
		//	//	fixed ( float* pSplitDistances = splitDistances )
		//	//	{
		//	//		MyOgreSceneManager.setShadowDirectionalLightSplitDistances( SceneManager.realObject, pSplitDistances, splitDistances.Length );
		//	//	}

		//	//	//shadow biases
		//	//	{
		//	//		for( int n = 0; n < 3; n++ )
		//	//		{
		//	//			MyOgreSceneManager.setShadowLightBias( SceneManager.realObject, LightType.Directional, n,
		//	//				renderingData.shadowLightBiasDirectionalLight[ n ].X, renderingData.shadowLightBiasDirectionalLight[ n ].Y );
		//	//		}
		//	//		MyOgreSceneManager.setShadowLightBias( SceneManager.realObject, LightType.Point, 0,
		//	//			renderingData.shadowLightBiasPointLight.X, renderingData.shadowLightBiasPointLight.Y );
		//	//		MyOgreSceneManager.setShadowLightBias( SceneManager.realObject, LightType.Spot, 0,
		//	//			renderingData.shadowLightBiasSpotLight.X, renderingData.shadowLightBiasSpotLight.Y );
		//	//	}

		//	//	MyOgreSceneManager.setDrawShadowDebugging( SceneManager.realObject, renderingData.shadowDrawDebugging );
		//	//}

		//	////!!!!!!
		//	//////set OgreViewport::sceneRenderingData
		//	////{
		//	////	SceneObject.VisibilityFlags sceneObjectsVisibilityMask = 0;
		//	////	if( renderingData.drawMeshSky )
		//	////		sceneObjectsVisibilityMask |= SceneObject.VisibilityFlags.MeshSky;
		//	////	if( renderingData.drawMeshStatic )
		//	////		sceneObjectsVisibilityMask |= SceneObject.VisibilityFlags.MeshStatic;
		//	////	if( renderingData.drawMeshUsual )
		//	////		sceneObjectsVisibilityMask |= SceneObject.VisibilityFlags.MeshUsual;
		//	////	if( renderingData.drawParticleSystem )
		//	////		sceneObjectsVisibilityMask |= SceneObject.VisibilityFlags.ParticleSystem;
		//	////	if( renderingData.drawBillboardSet )
		//	////		sceneObjectsVisibilityMask |= SceneObject.VisibilityFlags.BillboardSet;
		//	////	if( renderingData.drawRibbonTrail )
		//	////		sceneObjectsVisibilityMask |= SceneObject.VisibilityFlags.RibbonTrail;
		//	////	if( renderingData.draw3DGUI )
		//	////		sceneObjectsVisibilityMask |= SceneObject.VisibilityFlags.GUI;

		//	////	//lights
		//	////	void** pLights = (void**)NativeUtils.Alloc( NativeUtils.MemoryAllocationType.Renderer, renderingData.lights.Length * sizeof( void* ) );
		//	////	for( int n = 0; n < renderingData.lights.Length; n++ )
		//	////		pLights[ n ] = renderingData.lights[ n ].light.realObject;

		//	////	//scene nodes in frustum without scene nodes which outside the frustum (shadow casters)
		//	////	int sceneNodesCount = 0;
		//	////	void** pSceneNodes = (void**)NativeUtils.Alloc( NativeUtils.MemoryAllocationType.Renderer, renderingData.allSceneNodes.Length * sizeof( void* ) );
		//	////	for( int n = 0; n < renderingData.allSceneNodes.Length; n++ )
		//	////	{
		//	////		if( !renderingData.allSceneNodes[ n ].shadowCasterOutsideFrustum )
		//	////			pSceneNodes[ sceneNodesCount++ ] = renderingData.allSceneNodes[ n ].sceneNode.realObject;
		//	////	}

		//	////	//!!!!!debug geometry: пачкой сортировать внутри огра
		//	////	//!!!!!!как-то полностью отрубать дебажную геометрию? или выше? в общем управлять нужно этим уметь всем

		//	////	fixed ( IntPtr* pDebugGeometryRenderables = renderingData.debugGeometryRenderables )
		//	////	{
		//	////		OgreViewport.setSceneRenderingData( realObject,
		//	////			renderingData.lights.Length, pLights,
		//	////			(int)sceneObjectsVisibilityMask,
		//	////			sceneNodesCount, pSceneNodes,
		//	////			renderingData.debugGeometryRenderables.Length, (void**)pDebugGeometryRenderables );
		//	////	}

		//	////	NativeUtils.Free( (IntPtr)pLights );
		//	////	NativeUtils.Free( (IntPtr)pSceneNodes );
		//	////}

		//	////!!!!!!!
		//	//////set OgreLight::sceneRenderingData. must call after OgreViewport.setSceneRenderingData.
		//	////foreach( RenderingDataClass.LightItem lightItem in renderingData.lights )
		//	////{
		//	////	OgreLight.SceneRenderingData_AffectedSceneNodeItem* pAffectedSceneNodes = (OgreLight.SceneRenderingData_AffectedSceneNodeItem*)
		//	////		NativeUtils.Alloc( NativeUtils.MemoryAllocationType.Renderer, lightItem.affectedSceneNodes.Length * sizeof( void* ) );

		//	////	for( int n = 0; n < lightItem.affectedSceneNodes.Length; n++ )
		//	////	{
		//	////		RenderingDataClass.LightItem.AffectedSceneNodeItem affectedSceneNodeItem = lightItem.affectedSceneNodes[ n ];
		//	////		OgreLight.SceneRenderingData_AffectedSceneNodeItem item = new OgreLight.SceneRenderingData_AffectedSceneNodeItem();
		//	////		item.sceneNode = (IntPtr)affectedSceneNodeItem.sceneNode.realObject;
		//	////		item.shadowMapMaskForPSSMOrFaceOfPointLight = affectedSceneNodeItem.shadowMapMaskForPSSMOrFaceOfPointLight;
		//	////		pAffectedSceneNodes[ n ] = item;
		//	////	}

		//	////	OgreLight.setSceneRenderingData( lightItem.light.realObject, pAffectedSceneNodes, lightItem.affectedSceneNodes.Length,
		//	////		lightItem.prepareShadowTextures );

		//	////	NativeUtils.Free( (IntPtr)pAffectedSceneNodes );
		//	////}
		//}

		//unsafe void RenderingData_ClearNative( RenderingDataClass sceneRenderingData )
		//{
		//	//!!!!!

		//	//OgreViewport.clearSceneRenderingData( realObject );

		//	////clear SceneNode.sceneRenderingData_affectedLights
		//	//{
		//	//	void** pSceneNodes = (void**)NativeUtils.Alloc( NativeUtils.MemoryAllocationType.Renderer, sceneRenderingData.allSceneNodes.Length * sizeof( void* ) );
		//	//	int n = 0;
		//	//	foreach( RenderingDataClass.SceneNodeItem item in sceneRenderingData.allSceneNodes )
		//	//	{
		//	//		pSceneNodes[ n ] = item.sceneNode.realObject;
		//	//		n++;
		//	//	}
		//	//	MyOgreSceneManager.resetDataBeforeRendering( SceneManager.realObject, sceneRenderingData.allSceneNodes.Length, pSceneNodes );
		//	//	NativeUtils.Free( (IntPtr)pSceneNodes );
		//	//}
		//}

		//!!!!
		//!!!!!так? public?
		//public void _ViewportRendering_Clear( FrameBufferTypes buffers, ColorValue backgroundColor, float depth = 1, uint stencil = 0 )
		//{
		//	EngineThreading.CheckMainThread();

		//	unsafe
		//	{
		//		OgreViewport.clear( realObject, (uint)buffers, ref backgroundColor, depth, stencil );
		//	}
		//}

		static int GetEKeysMaxIndex()
		{
			int maxIndex = 0;
			foreach( EKeys eKey in Enum.GetValues( typeof( EKeys ) ) )
			{
				int index = (int)eKey;
				if( index > maxIndex )
					maxIndex = index;
			}
			return maxIndex;
		}

		public static EKeys[] AllKeys
		{
			get { return allKeys; }
		}

		public delegate void KeyDownDelegate( Viewport viewport, KeyEvent e, ref bool handled );
		public event KeyDownDelegate KeyDown;

		public void PerformKeyDown( KeyEvent e, ref bool handled )
		{
			//!!!!так? везде так сделать
			EngineThreading.CheckMainThread();

			lock( lockerKeysMouse )
			{
				if( keys == null )
					keys = new bool[ GetEKeysMaxIndex() + 1 ];

				keys[ (int)e.Key ] = true;
			}

			KeyDown?.Invoke( this, e, ref handled );
			if( handled )
				return;

			if( uiContainer != null && uiContainer.PerformKeyDown( e ) )
			{
				handled = true;
				return;
			}
		}

		public delegate void KeyPressDelegate( Viewport viewport, KeyPressEvent e, ref bool handled );
		public event KeyPressDelegate KeyPress;

		public void PerformKeyPress( KeyPressEvent e, ref bool handled )
		{
			EngineThreading.CheckMainThread();

			KeyPress?.Invoke( this, e, ref handled );
			if( handled )
				return;

			if( uiContainer != null && uiContainer.PerformKeyPress( e ) )
			{
				handled = true;
				return;
			}
		}

		public delegate void KeyUpDelegate( Viewport viewport, KeyEvent e, ref bool handled );
		public event KeyUpDelegate KeyUp;

		public void PerformKeyUp( KeyEvent e, ref bool handled )
		{
			EngineThreading.CheckMainThread();

			lock( lockerKeysMouse )
			{
				if( keys == null )
					keys = new bool[ GetEKeysMaxIndex() + 1 ];

				if( !keys[ (int)e.Key ] )
					return;

				keys[ (int)e.Key ] = false;
			}

			KeyUp?.Invoke( this, e, ref handled );

			if( uiContainer != null && uiContainer.PerformKeyUp( e ) )
				handled = true;
		}

		public delegate void MouseDownDelegate( Viewport viewport, EMouseButtons button, ref bool handled );
		public event MouseDownDelegate MouseDown;

		public void PerformMouseDown( EMouseButtons button, ref bool handled )
		{
			EngineThreading.CheckMainThread();

			lock( lockerKeysMouse )
			{
				if( mouseButtons[ (int)button ] )
					return;

				mouseButtons[ (int)button ] = true;
			}

			MouseDown?.Invoke( this, button, ref handled );
			if( handled )
				return;

			if( uiContainer != null && uiContainer.PerformMouseDown( button ) )
			{
				handled = true;
				return;
			}
		}

		public delegate void MouseUpDelegate( Viewport viewport, EMouseButtons button, ref bool handled );
		public event MouseUpDelegate MouseUp;

		public void PerformMouseUp( EMouseButtons button, ref bool handled )
		{
			EngineThreading.CheckMainThread();

			lock( lockerKeysMouse )
			{
				if( !mouseButtons[ (int)button ] )
					return;

				mouseButtons[ (int)button ] = false;
			}

			MouseUp?.Invoke( this, button, ref handled );

			if( uiContainer != null && uiContainer.PerformMouseUp( button ) )
				handled = true;
		}

		public delegate void MouseDoubleClickDelegate( Viewport viewport, EMouseButtons button, ref bool handled );
		public event MouseDoubleClickDelegate MouseDoubleClick;

		public void PerformMouseDoubleClick( EMouseButtons button, ref bool handled )
		{
			EngineThreading.CheckMainThread();

			MouseDoubleClick?.Invoke( this, button, ref handled );
			if( handled )
				return;

			if( uiContainer != null && uiContainer.PerformMouseDoubleClick( button ) )
			{
				handled = true;
				return;
			}
		}

		public delegate void MouseMoveDelegate( Viewport viewport, Vector2 mouse );//, ref bool handled );
		public event MouseMoveDelegate MouseMove;

		//!!!!!погонять очень часто-приходящий
		public void PerformMouseMove( Vector2 mouse )
		{
			//!!!!если не обновилось, то всё равно вызывать?
			//!!!!mousemove вызывается при нажатиях кнопок

			EngineThreading.CheckMainThread();

			lock( lockerKeysMouse )
			{
				if( !mouseRelativeMode )
					mousePosition = mouse;
				else
					mousePosition = new Vector2( .5, .5 );
			}

			bool skip = false;
			if( MouseRelativeMode && mouseRelativeModeSkipOneMouseMove )
			{
				skip = true;
				mouseRelativeModeSkipOneMouseMove = false;
			}

			if( !skip )
			{
				MouseMove?.Invoke( this, mouse );//, ref handled );

				if( uiContainer != null )
					uiContainer.PerformMouseMove( mouse );
			}
		}

		public delegate void MouseWheelDelegate( Viewport viewport, int delta, ref bool handled );
		public event MouseWheelDelegate MouseWheel;

		public void PerformMouseWheel( int delta, ref bool handled )
		{
			EngineThreading.CheckMainThread();

			MouseWheel?.Invoke( this, delta, ref handled );
			if( handled )
				return;

			if( uiContainer != null && uiContainer.PerformMouseWheel( delta ) )
			{
				handled = true;
				return;
			}
		}

		public delegate void JoystickEventDelegate( Viewport viewport, JoystickInputEvent e, ref bool handled );
		public event JoystickEventDelegate JoystickEvent;

		public void PerformJoystickEvent( JoystickInputEvent e, ref bool handled )
		{
			EngineThreading.CheckMainThread();

			JoystickEvent?.Invoke( this, e, ref handled );
			if( handled )
				return;

			if( uiContainer != null && uiContainer.PerformJoystickEvent( e ) )
			{
				handled = true;
				return;
			}
		}

		public delegate void TouchEventDelegate( Viewport viewport, TouchEventData e, ref bool handled );
		public event TouchEventDelegate TouchEvent;

		public void PerformTouchEvent( TouchEventData e, ref bool handled )
		{
			EngineThreading.CheckMainThread();

			TouchEvent?.Invoke( this, e, ref handled );
			if( handled )
				return;

			if( uiContainer != null && uiContainer.PerformTouchEvent( e ) )
			{
				handled = true;
				return;
			}
		}

		public delegate void SpecialInputDeviceEventDelegate( Viewport viewport, InputEvent e, ref bool handled );
		public event SpecialInputDeviceEventDelegate SpecialInputDeviceEvent;

		public void PerformSpecialInputDeviceEvent( InputEvent e, ref bool handled )
		{
			EngineThreading.CheckMainThread();

			SpecialInputDeviceEvent?.Invoke( this, e, ref handled );
			if( handled )
				return;

			if( uiContainer != null && uiContainer.PerformSpecialInputDeviceEvent( e ) )
			{
				handled = true;
				return;
			}
		}

		public bool IsKeyPressed( EKeys key )
		{
			lock( lockerKeysMouse )
			{
				if( keys != null )
					return keys[ (int)key ];
				else
					return false;
			}
		}

		public bool IsKeyLocked( EKeys key )
		{
			if( key != EKeys.Insert && key != EKeys.NumLock && key != EKeys.Capital && key != EKeys.Scroll )
				Log.Fatal( "Viewport: IsKeyLocked: Invalid key value. Next keys can be checked by this method: EKeys.Insert, EKeys.NumLock, EKeys.Capital, EKeys.Scroll." );
			return EngineApp.platform.IsKeyLocked( key );
		}

		public bool IsMouseButtonPressed( EMouseButtons button )
		{
			lock( lockerKeysMouse )
				return mouseButtons[ (int)button ];
		}

		public bool IsJoystickButtonPressed( JoystickButtons button )
		{
			//!!!!!
			EngineThreading.CheckMainThread();

			if( InputDeviceManager.Instance != null )
			{
				for( int n = 0; n < InputDeviceManager.Instance.Devices.Count; n++ )
				{
					InputDevice device = InputDeviceManager.Instance.Devices[ n ];
					JoystickInputDevice joystickInputDevice = device as JoystickInputDevice;
					if( joystickInputDevice != null )
					{
						if( joystickInputDevice.IsButtonPressed( button ) )
							return true;
					}
				}
			}
			return false;
		}

		public void KeysAndMouseButtonUpAll()
		{
			EngineThreading.CheckMainThread();

			if( keys != null )
			{
				for( int key = 0; key < keys.Length; key++ )
				{
					KeyEvent keyEvent = new KeyEvent( (EKeys)key );
					bool handled = false;
					PerformKeyUp( keyEvent, ref handled );
				}
			}

			for( int n = 0; n < mouseButtons.Length; n++ )
			{
				bool handled = false;
				PerformMouseUp( (EMouseButtons)n, ref handled );
			}
		}

		internal delegate void MousePositionSetImplDelegate( Vector2 mouse );
		internal MousePositionSetImplDelegate MousePositionSetImpl;

		internal delegate void MouseRelativeModeSetImplDelegate( bool enable );
		internal MouseRelativeModeSetImplDelegate MouseRelativeModeSetImpl;

		public Vector2 MousePosition
		{
			get
			{
				lock( lockerKeysMouse )
				{
					if( mouseRelativeMode )
						return new Vector2( .5f, .5f );
					else
						return mousePosition;
				}
			}
			set
			{
				lock( lockerKeysMouse )
				{
					if( mouseRelativeMode )
						mousePosition = new Vector2( .5f, .5f );
					else
						mousePosition = value;
				}

				MousePositionSetImpl?.Invoke( mousePosition );
			}
		}

		public delegate void MouseRelativeModeChangedDelegate( Viewport viewport, ref bool handled );
		public event MouseRelativeModeChangedDelegate MouseRelativeModeChanged;

		public bool MouseRelativeMode
		{
			get
			{
				lock( lockerKeysMouse )
					return mouseRelativeMode;
			}
			set
			{
				lock( lockerKeysMouse )
				{
					if( mouseRelativeMode == value )
						return;

					if( value )
					{
						//enable
						restoreMousePosAfterRelativeMode = MousePosition;
						MousePosition = new Vector2( .5f, .5f );
					}

					mouseRelativeMode = value;
					mouseRelativeModeSkipOneMouseMove = true;

					if( !value )
					{
						//disable
						MousePosition = restoreMousePosAfterRelativeMode;
					}
				}

				MouseRelativeModeSetImpl?.Invoke( mouseRelativeMode );

				bool handled = false;
				MouseRelativeModeChanged?.Invoke( this, ref handled );

				////!!!!?
				//if( handled )
				//	return;

				//!!!!new
				if( EngineApp.CreatedInsideEngineWindow != null )
				{
					Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
					if( viewport == this )
					{
						if( EngineApp.platform != null && EngineApp.Created && !EngineApp.Closing && EngineApp.platform.IsFocused() )
							EngineApp.platform.CreatedWindow_OnMouseRelativeModeChange();
					}
				}

				////!!!!new
				//if( EngineApp.CreatedInsideEngineWindow != null )
				//{
				//	Viewport viewport = RendererWorld.ApplicationRenderTarget.Viewports[ 0 ];
				//	if( viewport == this )
				//	{
				//		if( EngineApp.platform != null && EngineApp.Created && !EngineApp.Closing && EngineApp.platform.IsFocused() )
				//			EngineApp.platform.UpdateShowSystemCursor();
				//	}
				//}

				//!!!!!!control manager?
			}
		}

		public delegate void TickDelegate( Viewport sender, float delta );
		public event TickDelegate Tick;

		public void PerformTick( float delta )
		{
			EngineThreading.CheckMainThread();

			Tick?.Invoke( this, delta );
			if( attachedScene != null && attachedScene.EnabledInHierarchy )
				attachedScene.PerformUpdate( delta );
			uiContainer?.PerformUpdate( delta );
			uiContainer?.HierarchyController.ProcessDelayedOperations();

			//if( controlManager != null )
			//	controlManager.PerformSpecialInputDeviceEvent( e );
		}


		//!!!!!!xx xx;//Resize event?

		//bool IsEnable3DSceneRendering()
		//{
		//	return Enable3DSceneRendering && SceneManager.Instance.Enable3DSceneRendering;
		//}

		//void InitGeneralRenderingData( RenderingDataClass renderingData )
		//{
		//	xx xx;

		//	if( attachedMap != null )
		//	{
		//		//fog
		//		renderingData.fogMode = attachedMap.FogMode;
		//		renderingData.fogColor = attachedMap.FogColor;
		//		renderingData.fogExpDensity = attachedMap.FogExpDensity;
		//		renderingData.fogLinearStart = attachedMap.FogLinearStart;
		//		renderingData.fogLinearEnd = attachedMap.FogLinearEnd;

		//		//ambient lighting
		//		renderingData.ambientLight = attachedMap.AmbientLight;

		//		//shadows
		//		renderingData.drawShadows = false;
		//		//!!!!

		//		//dataForRendering.drawShadows = SceneManager.Instance.ShadowTechnique != ShadowTechniques.None && viewport.ShadowsEnabled &&
		//		//   EngineDebugSettings.DrawShadows && enable3DSceneRendering;

		//		//!!!!!было
		//		//if( viewport.AttachedMap != null )
		//		//{
		//		//   dataForRendering.shadowIntensity = viewport.AttachedMap.ShadowIntensity;
		//		//   dataForRendering.shadowFarDistance = viewport.AttachedMap.Instance.ShadowFarDistance;
		//		//   dataForRendering.shadowPSSMSplitFactors = viewport.AttachedMap.ShadowPSSMSplitFactors;
		//		//}

		//		//!!!!!было
		//		////shadow biases
		//		//if( dataForRendering.drawShadows )
		//		//{
		//		//   //!!!!!всё тут temp, новые тени нужны. по сути быстро их сделать


		//		//   //bool atiHardwareShadows = false;
		//		//   bool nvidiaHardwareShadows = false;
		//		//   if( TextureManager.Instance.IsFormatSupported( Texture.Type.Type2D, PixelFormats.Depth24, Texture.Usage.RenderTarget ) )
		//		//   {
		//		//      //if( RenderSystem.Instance.Capabilities.Vendor == GPUVendors.ATI )
		//		//      //   atiHardwareShadows = true;
		//		//      if( RenderSystem.Instance.Capabilities.Vendor == GPUVendors.NVidia )
		//		//         nvidiaHardwareShadows = true;
		//		//   }

		//		//   float qualityFactor;
		//		//   {
		//		//      if( SceneManager.Instance.ShadowTechnique == ShadowTechniques.ShadowmapHigh ||
		//		//         SceneManager.Instance.ShadowTechnique == ShadowTechniques.ShadowmapHighPSSM ||
		//		//         SceneManager.Instance.ShadowTechnique == ShadowTechniques.ShadowmapMedium ||
		//		//         SceneManager.Instance.ShadowTechnique == ShadowTechniques.ShadowmapMediumPSSM )
		//		//      {
		//		//         qualityFactor = 1.5f;
		//		//      }
		//		//      else
		//		//      {
		//		//         qualityFactor = 1;
		//		//      }
		//		//   }

		//		//   if( RenderSystem.Instance.IsDirect3D() )
		//		//   {
		//		//      //Direct3D

		//		//      //directional light
		//		//      {
		//		//         //NVIDIA: Depth24 texture format
		//		//         //ATI: Float32 texture format

		//		//         float[] factors = null;
		//		//         switch( SceneManager.Instance.ShadowTechnique )
		//		//         {
		//		//         case ShadowTechniques.ShadowmapLow: factors = new float[] { 1.0f }; break;
		//		//         case ShadowTechniques.ShadowmapMedium: factors = new float[] { 1.5f }; break;
		//		//         case ShadowTechniques.ShadowmapHigh: factors = new float[] { 1.5f }; break;
		//		//         case ShadowTechniques.ShadowmapLowPSSM: factors = new float[] { 1.0f, 1.0f, 1.0f }; break;
		//		//         case ShadowTechniques.ShadowmapMediumPSSM: factors = new float[] { 1.5f, 1.0f, 1.0f }; break;
		//		//         case ShadowTechniques.ShadowmapHighPSSM: factors = new float[] { 1.5f, 1.5f, 1.0f }; break;
		//		//         }

		//		//         float iterationCount = SceneManager.Instance.IsShadowTechniquePSSM() ? 3 : 1;
		//		//         for( int index = 0; index < iterationCount; index++ )
		//		//         {
		//		//            if( nvidiaHardwareShadows )
		//		//            {
		//		//               //Depth24 texture format
		//		//               dataForRendering.shadowLightBiasDirectionalLight[ index ] = new Vec2( .0001f + .00005f * (float)index, factors[ index ] );
		//		//            }
		//		//            else
		//		//            {
		//		//               //Float32 texture format
		//		//               dataForRendering.shadowLightBiasDirectionalLight[ index ] = new Vec2( .0001f + .00005f * (float)index, factors[ index ] );
		//		//            }
		//		//         }
		//		//      }

		//		//      //point light
		//		//      {
		//		//         //Float32 texture format (both for NVIDIA and ATI)
		//		//         dataForRendering.shadowLightBiasPointLight = new Vec2( .05f * qualityFactor, 0 );
		//		//         //shadowLightBiasPointLight = new Vec2( .2f * qualityFactor, .5f * qualityFactor );
		//		//      }

		//		//      //spot light
		//		//      {
		//		//         if( nvidiaHardwareShadows )
		//		//         {
		//		//            //Depth24 texture format
		//		//            float textureSize = Map.Instance.GetShadowSpotLightTextureSizeAsInteger();
		//		//            float textureSizeFactor = 1024.0f / textureSize;
		//		//            dataForRendering.shadowLightBiasSpotLight = new Vec2( .001f * qualityFactor * textureSizeFactor, .001f * qualityFactor );
		//		//         }
		//		//         else
		//		//         {
		//		//            //Float32 texture format
		//		//            dataForRendering.shadowLightBiasSpotLight = new Vec2( .1f * qualityFactor, 1.0f * qualityFactor );
		//		//         }
		//		//      }
		//		//   }
		//		//   else
		//		//   {
		//		//      //OpenGL
		//		//      dataForRendering.shadowLightBiasDirectionalLight[ 0 ] = new Vec2( .0003f * qualityFactor, 0 );
		//		//      dataForRendering.shadowLightBiasPointLight = new Vec2( .15f * qualityFactor, 0 );
		//		//      dataForRendering.shadowLightBiasSpotLight = new Vec2( .15f * qualityFactor, 0 );
		//		//   }
		//		//}


		//		renderingData.shadowDrawDebugging = EngineSettings.Debug.DrawShadowDebugging;

		//		//scene nodes
		//		renderingData.drawMeshSky = EngineSettings.Debug.DrawSky && IsEnable3DSceneRendering();
		//		renderingData.drawMeshStatic = EngineSettings.Debug.DrawModels && IsEnable3DSceneRendering();
		//		renderingData.drawMeshUsual = EngineSettings.Debug.DrawModels && IsEnable3DSceneRendering();
		//		renderingData.drawParticleSystem = EngineSettings.Debug.DrawEffects && IsEnable3DSceneRendering();
		//		renderingData.drawBillboardSet = EngineSettings.Debug.DrawEffects && IsEnable3DSceneRendering();
		//		renderingData.drawRibbonTrail = EngineSettings.Debug.DrawEffects && IsEnable3DSceneRendering();
		//		renderingData.draw3DGUI = EngineSettings.Debug.DrawInGame3DGUI && IsEnable3DSceneRendering();

		//		//!!!!!это было после переопределения объектов. как делать?
		//		//shadowDirectionalLightExtrusionDistanceForCastersDetection
		//		if( renderingData.shadowDirectionalLightExtrusionDistanceForCastersDetection == 0 )
		//		{
		//			//!!!!!norm?
		//			renderingData.shadowDirectionalLightExtrusionDistanceForCastersDetection = renderingData.shadowFarDistance * 2;
		//			//dataForRendering.shadowDirectionalLightExtrusionDistanceForCastersDetection = viewport.Camera.FarClipDistance;
		//		}

		//	}
		//}

		///// <summary>
		///// Call AddToRender events and remove if need exclude from rendering.
		///// </summary>
		//void PerformAddToRenderEventsAndRemoveSkipped( RenderingDataClass renderingData )
		//{
		//	//!!!!!

		//	////call RenderLight.AddToRender event
		//	//foreach( RenderingDataClass.LightItem item in renderingData.lights )
		//	//{
		//	//	bool allowRender = true;
		//	//	item.light.PerformAddToRender( this, ref allowRender );
		//	//	item.light.tempAllowRender = allowRender;
		//	//}

		//	////call SceneNode.AddToRender event
		//	//foreach( RenderingDataClass.SceneNodeItem item in renderingData.allSceneNodes )
		//	//{
		//	//	bool allowRender = true;
		//	//	item.sceneNode.PerformAddToRender( this, ref allowRender );
		//	//	item.sceneNode.tempAllowRender = allowRender;
		//	//}

		//	////remove from lights
		//	//{
		//	//	int newCount = 0;
		//	//	foreach( RenderingDataClass.LightItem item in renderingData.lights )
		//	//	{
		//	//		if( item.light.tempAllowRender )
		//	//			newCount++;
		//	//	}
		//	//	if( newCount != renderingData.lights.Length )
		//	//	{
		//	//		RenderingDataClass.LightItem[] newBuffer = new RenderingDataClass.LightItem[ newCount ];
		//	//		int index = 0;
		//	//		foreach( RenderingDataClass.LightItem item in renderingData.lights )
		//	//		{
		//	//			if( item.light.tempAllowRender )
		//	//				newBuffer[ index++ ] = item;
		//	//		}
		//	//		renderingData.lights = newBuffer;
		//	//	}
		//	//}

		//	////remove from allSceneNodes
		//	//{
		//	//	int newCount = 0;
		//	//	foreach( RenderingDataClass.SceneNodeItem item in renderingData.allSceneNodes )
		//	//	{
		//	//		if( item.sceneNode.tempAllowRender )
		//	//			newCount++;
		//	//	}
		//	//	if( newCount != renderingData.allSceneNodes.Length )
		//	//	{
		//	//		RenderingDataClass.SceneNodeItem[] newBuffer = new RenderingDataClass.SceneNodeItem[ newCount ];
		//	//		int index = 0;
		//	//		foreach( RenderingDataClass.SceneNodeItem item in renderingData.allSceneNodes )
		//	//		{
		//	//			if( item.sceneNode.tempAllowRender )
		//	//				newBuffer[ index++ ] = item;
		//	//		}
		//	//		renderingData.allSceneNodes = newBuffer;
		//	//	}
		//	//}

		//	////remove from LightItem.affectedSceneNodes
		//	//foreach( RenderingDataClass.LightItem lightItem in renderingData.lights )
		//	//{
		//	//	int newCount = 0;
		//	//	foreach( RenderingDataClass.LightItem.AffectedSceneNodeItem item in lightItem.affectedSceneNodes )
		//	//	{
		//	//		if( item.sceneNode.tempAllowRender )
		//	//			newCount++;
		//	//	}
		//	//	if( newCount != lightItem.affectedSceneNodes.Length )
		//	//	{
		//	//		RenderingDataClass.LightItem.AffectedSceneNodeItem[] newBuffer = new RenderingDataClass.LightItem.AffectedSceneNodeItem[ newCount ];
		//	//		int index = 0;
		//	//		foreach( RenderingDataClass.LightItem.AffectedSceneNodeItem item in lightItem.affectedSceneNodes )
		//	//		{
		//	//			if( item.sceneNode.tempAllowRender )
		//	//				newBuffer[ index++ ] = item;
		//	//		}
		//	//		lightItem.affectedSceneNodes = newBuffer;
		//	//	}
		//	//}
		//}

		//void FinishRenderingData( RenderingDataClass renderingData )
		//{
		//	//что получить?
		//	//геометрия мешей
		//	//простую 3д сцену без освещения - одним цветом
		//	////далее forward rendering без теней
		//	//////далее простые тени? ну и всё. DS? после этого уже надо сразу самый современный рендер цеплять
		//	//debug geometry - всю

		//	//no hardware instancing
		//	if( !EngineSettings.Debug.AllowHardwareInstancing )
		//		renderingData.allowHardwareInstancing = false;

		//	//wireframe mode
		//	if( EngineSettings.Debug.DrawInWireframeMode )
		//		renderingData.polygonMode = PolygonMode.Wireframe;

		//	////default objects visibility detection
		//	if( attachedMap != null && IsEnable3DSceneRendering() && renderingData.sceneObjects == null )
		//	{
		//		//!!!!temp

		//		List<SceneObject> sceneObjects = new List<SceneObject>();
		//		foreach( var s in attachedMap.SceneObjectsAll )
		//		{
		//			if( s.Visible )
		//				sceneObjects.Add( s );
		//		}
		//		renderingData.sceneObjects = new ESet<SceneObject>( sceneObjects );

		//		//SceneObjectVisiblityDetection_Default.DefaultImplObjectVisibilityDetection( this, renderingData );
		//	}

		//	//!!!!!
		//	////check valid lights array
		//	//if( renderingData.lights != null )
		//	//{
		//	//	foreach( RenderingDataClass.LightItem item in renderingData.lights )
		//	//	{
		//	//		if( item.affectedSceneNodes == null )
		//	//			Log.Fatal( "Viewport: FinishRenderingData: item.affectedSceneNodes == null." );
		//	//	}
		//	//}

		//	//!!!!!
		//	//if( renderingData.lights != null && renderingData.allSceneNodes != null )
		//	//	PerformAddToRenderEventsAndRemoveSkipped( renderingData );

		//	//no ambient lighting
		//	if( !EngineSettings.Debug.DrawAmbientLighting )
		//		renderingData.ambientLight = new ColorValue( 0, 0, 0 );

		//	//no fog
		//	if( !EngineSettings.Debug.DrawFog )
		//		renderingData.fogMode = FogMode.None;

		//	//!!!!!
		//	////no dynamic lighting
		//	//if( !EngineSettings.Debug.DrawDynamicLighting && renderingData.lights != null )
		//	//	renderingData.lights = new RenderingDataClass.LightItem[ 0 ];

		//	//!!!!!можно раньше выключать
		//	////no 3D scene rendering
		//	//if( !IsEnable3DSceneRendering() )
		//	//{
		//	//	if( renderingData.lights != null )
		//	//		renderingData.lights = new RenderingDataClass.LightItem[ 0 ];
		//	//	if( renderingData.allSceneNodes != null )
		//	//		renderingData.allSceneNodes = new RenderingDataClass.SceneNodeItem[ 0 ];
		//	//}

		//	//shadowPSSMSplitFactors
		//	renderingData.shadowPSSMSplitFactors.Clamp( Vec2F.Zero, Vec2F.One );
		//	if( renderingData.shadowPSSMSplitFactors[ 0 ] > renderingData.shadowPSSMSplitFactors[ 1 ] )
		//		renderingData.shadowPSSMSplitFactors = new Vec2( renderingData.shadowPSSMSplitFactors[ 0 ], renderingData.shadowPSSMSplitFactors[ 0 ] );

		//	//!!!!!
		//	////!!!!!!!это потом для всех лучше сделать?
		//	//SceneObjectVisiblityDetection_Default.SortLightsByTypeAndDistance( this, renderingData );
		//	//SceneObjectVisiblityDetection_Default.FixPrepareShadowTexturesFlag( this, renderingData );

		//	//!!!!
		//	//renderingData.canUseObjectsDictionaries = true;
		//}

		//!!!!callBgfxFrame
		/// <summary>
		/// Updates viewport with the rendering of attached map and GUI rendering.
		/// </summary>
		public void Update( bool callBgfxFrame, CameraSettingsClass overrideCameraSettings = null )
		{
			UpdateBefore?.Invoke( this );
			AllViewports_UpdateBefore?.Invoke( this );
			AttachedScene?.PerformViewportUpdateBefore( this, overrideCameraSettings );

			if( AttachedScene != null && !AttachedScene.EnabledInHierarchy )
				return;

			EngineThreading.CheckMainThread();

			if( RenderingSystem.viewportsDuringUpdate.Contains( this ) )
				Log.Fatal( "Viewport: Update: The viewport is already during updating. RendererWorld.currentViewportsInUpdateStack.Contains( viewport )." );
			RenderingSystem.viewportsDuringUpdate.Add( this );

			//remove reference to scene when destroyed
			if( attachedScene != null && Array.IndexOf( Component_Scene.All, attachedScene ) == -1 )
				attachedScene = null;

			//!!!!!тут? так?
			//!!!!создавать так? надо ли Enabled= true или типа того
			if( renderingContext == null )
				renderingContext = new ViewportRenderingContext( this );

			ViewportRenderingContext.current = renderingContext;

			//begin update
			renderingContext.uniquePerFrameObjectToDetectNewFrame = new object();
			renderingContext.objectsDuringUpdate = new ViewportRenderingContext.ObjectsDuringUpdateClass();

			renderingContext.currentViewNumber = -1;

			previousUpdateTime = lastUpdateTime;

			//!!!!!тут? так?
			//lastUpdateTime
			double time = EngineApp.EngineTime;
			double oldTime = lastUpdateTime;
			lastUpdateTime = time;
			lastUpdateTimeStep = lastUpdateTime - oldTime;

			renderingContext.updateStatisticsPrevious = renderingContext.updateStatisticsCurrent;
			renderingContext.updateStatisticsCurrent = new ViewportRenderingContext.StatisticsClass();

			//statistics FPS
			{
				var stats = renderingContext.updateStatisticsCurrent;

				stats.previousTimeStep[ 0 ] = lastUpdateTimeStep;
				for( int n = 0; n < stats.previousTimeStep.Length - 1; n++ )
					stats.previousTimeStep[ n + 1 ] = renderingContext.updateStatisticsPrevious.previousTimeStep[ n ];

				stats.updateFPSCounter = renderingContext.updateStatisticsPrevious.updateFPSCounter;
				stats.updateFPSCounter--;
				if( stats.updateFPSCounter < 0 )
				{
					stats.updateFPSCounter = stats.previousTimeStep.Length;

					double timeStep = 0;
					foreach( var v in stats.previousTimeStep )
						timeStep += v;
					timeStep /= stats.previousTimeStep.Length;
					stats.FPS = timeStep != 0 ? 1.0f / timeStep : 0;
				}
				else
					stats.FPS = renderingContext.updateStatisticsPrevious.FPS;

				//stats.previousFPS[ 0 ] = lastUpdateTimeStep != 0 ? 1.0 / lastUpdateTimeStep : 0;
				//for( int n = 0; n < stats.previousFPS.Length - 1; n++ )
				//	stats.previousFPS[ n + 1 ] = renderingContext.updateStatisticsPrevious.previousFPS[ n ];

				//stats.updateFPSCounter = renderingContext.updateStatisticsPrevious.updateFPSCounter;
				//stats.updateFPSCounter--;
				//if( stats.updateFPSCounter < 0 )
				//{
				//	stats.updateFPSCounter = stats.previousFPS.Length;

				//	double fps = 0;
				//	foreach( var v in stats.previousFPS )
				//		fps += v;
				//	fps /= stats.previousFPS.Length;
				//	stats.FPS = fps;
				//}
				//else
				//	stats.FPS = renderingContext.updateStatisticsPrevious.FPS;

				//renderingContext.UpdateStatisticsCurrent.FPS = lastUpdateTimeStep != 0 ? 1.0 / lastUpdateTimeStep : double.PositiveInfinity;
			}

			UpdateBegin?.Invoke( this );
			AllViewports_UpdateBegin?.Invoke( this );

			//!!!!!!where sound listener?

			//!!!!new тут. было ниже
			AttachedScene?.PerformViewportUpdateBegin( this, overrideCameraSettings );

			Component_RenderingPipeline pipeline = null;
			if( cameraSettings != null && cameraSettings.RenderingPipelineOverride != null )
				pipeline = cameraSettings.RenderingPipelineOverride;
			else if( AttachedScene != null )
				pipeline = AttachedScene.RenderingPipeline;

			if( pipeline == null )
			{
				if( renderingPipelineCreated == null )
					RenderingPipelineCreate();
				pipeline = renderingPipelineCreated;
			}
			else
				RenderingPipelineDestroyCreated();

			//!!!!теперь выше
			////!!!!!что там?
			//AttachedScene?.PerformViewportUpdateBegin( this, overrideCameraSettings );


			////!!!!так?
			////update reflection probes in Capture mode
			//if( AttachedScene != null && Mode == ModeEnum.Default )
			//{
			//	//!!!!slowly. еще одна выборка из octree

			//	var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All,
			//		MetadataManager.GetTypeOfNetType( typeof( Component_ReflectionProbe ) ), true, CameraSettings.Frustum );
			//	AttachedScene.GetObjectsInSpace( getObjectsItem );

			//	foreach( var item in getObjectsItem.Result )
			//	{
			//		var probe = item.Object as Component_ReflectionProbe;
			//		if( probe != null && probe.Mode.Value == Component_ReflectionProbe.ModeEnum.Capture )
			//			probe.Update( false );
			//	}
			//}

			//!!!!
			//rendering data
			//RenderingDataClass renderingData = new RenderingDataClass();

			//{
			//	//InitGeneralRenderingData( renderingData );
			//	Update2_GetRenderingData?.Invoke( this, renderingData );
			//	AllViewports_Update2_GetRenderingData?.Invoke( this, renderingData );
			//	//FinishRenderingData( renderingData );
			//}

			//ComponentScene.OnRender, Component_ObjectInScene.OnRender
			{
				//get rendering context for objects in space
				Component_ObjectInSpace.RenderingContext objectInSpaceRenderingContext = null;
				UpdateGetObjectInSceneRenderingContext?.Invoke( this, ref objectInSpaceRenderingContext );
				AllViewports_UpdateGetObjectInSceneRenderingContext?.Invoke( this, ref objectInSpaceRenderingContext );
				if( objectInSpaceRenderingContext == null )
					objectInSpaceRenderingContext = new Component_ObjectInSpace.RenderingContext( this );

				renderingContext.objectInSpaceRenderingContext = objectInSpaceRenderingContext;

				lastFrameScreenLabels.Clear();

				//!!!!это правильнее рисовать до transform tool. а значит между двумя UpdateBeforeOutput
				if( AttachedScene != null && AttachedScene.EnabledInHierarchy )
				{
					AttachedScene.PerformRender( this );

					//foreach( var obj in AttachedScene.GetComponents<Component_ObjectInSpace>( false, true, true ) )
					//{
					//	//!!!!new: obj.VisibleInHierarchy
					//	if( obj.EnabledInHierarchy && obj.VisibleInHierarchy )//second check if will updated during enumeration
					//	{
					//		//reset this object parameters
					//		objectInSpaceRenderingContext.disableShowingLabelForThisObject = false;
					//		//context.thisObjectWasDisplayed = false;
					//		//context.disableShowingThisObject = false;
					//		//context.thisObjectRaySelectionDetalization_Ray = null;
					//		//context.thisObjectRaySelectionDetalization_RayScaleResult = 0;

					//		obj.PerformRender( objectInSpaceRenderingContext );

					//		//render screen label if not displayed

					//		if( AttachedScene.GetDisplayDevelopmentDataInThisApplication() && AttachedScene.DisplayLabels && obj.EnabledSelectionByCursor && !objectInSpaceRenderingContext.disableShowingLabelForThisObject && AllowRenderScreenLabels && objectInSpaceRenderingContext.viewport.CanvasRenderer != null )
					//		//if( AttachedScene.DisplayDevelopmentDataInEditor && AttachedScene.DisplayLabels && obj.EnabledSelectionByCursor && !context.disableShowingLabelForThisObject && AllowRenderScreenLabels && context.viewport.CanvasRenderer != null )
					//		{
					//			//if( !context.thisObjectWasDisplayed )
					//			//{
					//			//!!!!когда не надо, не рисовать

					//			if( obj.DrawObjectScreenLabel( objectInSpaceRenderingContext, out Rectangle labelScreenRectangle ) )
					//			{
					//				var item = new LastFrameScreenLabelItem();
					//				item.obj = obj;
					//				item.screenRectangle = labelScreenRectangle;
					//				lastFrameScreenLabels.Add( item );
					//			}

					//			//!!!!а если уже нарисовал bouding box дебажно

					//			//draw bounding box for selected and can be selected

					//			//if( ParentScene.DrawObjectInSpaceBounds )
					//			//{
					//			//	//!!!!цвета. ProjectSettings
					//			//	//!!!!!!как вариант: в настройках портянкой всё. а показывается уже страницами
					//			//	ColorValue color = new ColorValue( 1, 1, 0, .7 );
					//			//	context.viewport.DebugGeometry.SetColor( color, color * new ColorValue( 1, 1, 1, color.Alpha * .5 ) );
					//			//	context.viewport.DebugGeometry.AddBounds( SpaceBounds.CalculatedBoundingBox );
					//			//}

					//			//obj.SpaceBounds.CalculatedBoundingBox;
					//			//xx xx;//если есть bounding box
					//			//xx xx;
					//			//ColorValue color = new ColorValue( 1, 1, 0, .7 );
					//			//context.viewport.DebugGeometry.SetColor( color, color * new ColorValue( 1, 1, 1, color.Alpha * .5 ) );
					//			//context.viewport.DebugGeometry.AddBounds( SpaceBounds.CalculatedBoundingBox );




					//			//!!!!если не пустой bounding box

					//			//if( context.selectedObjects.Contains( obj ) || context.canSelectObjects.Contains( obj ) )
					//			//{
					//			//	ColorValue color;
					//			//	if( context.selectedObjects.Contains( obj ) )
					//			//		color = ProjectSettings.Get.SelectedColor;
					//			//	else ///if( context.canSelectObjects.Contains( this ) )
					//			//		color = ProjectSettings.Get.CanSelectColor;
					//			//	//else
					//			//	//	color = ProjectSettings.Get.DebugDrawObjectInSpaceBoundsColor;

					//			//	var viewport = context.viewport;
					//			//	viewport.DebugGeometry.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
					//			//	context.viewport.DebugGeometry.AddBounds( obj.SpaceBounds.CalculatedBoundingBox );
					//			//}



					//			//context.thisObjectWasDisplayed = true;
					//			//}

					//		}
					//	}
					//}

					AttachedScene.OnRenderBeforeOutput( this );
				}
			}

			//UpdateBeforeOutput?.Invoke( this );
			//AllViewports_UpdateBeforeOutput?.Invoke( this );

			renderingContext.renderingPipeline = pipeline;
			pipeline.Render( renderingContext );
			renderingContext.renderingPipeline = null;
			//renderingContext.Render( pipeline );// renderingData );

			//!!!!
			if( callBgfxFrame )
			{
				RenderingSystem.CallBgfxFrame();
				renderingContext.ResetViews();
			}

			//clear
			simple3DRenderer?._Clear();
			canvasRenderer?.Clear( LastUpdateTime );

			AttachedScene?.PerformViewportUpdateEnd( this );

			UpdateEnd?.Invoke( this );
			AllViewports_UpdateEnd?.Invoke( this );

			//!!!!по идее, можно не чистить до следующего апдейта
			renderingContext.objectsDuringUpdate = null;
			renderingContext.objectInSpaceRenderingContext = null;

			renderingContext.MultiRenderTarget_DestroyAll();
			renderingContext.DynamicTexture_FreeAllEndUpdate();

			if( cameraSettings != null )
			{
				viewMatrixPreviousFrame = cameraSettings.ViewMatrix;
				projectionMatrixPreviousFrame = cameraSettings.ProjectionMatrix;
			}
			else
			{
				viewMatrixPreviousFrame = null;
				projectionMatrixPreviousFrame = null;
			}

			//!!!!что чистить?

			ViewportRenderingContext.current = null;

			if( RenderingSystem.viewportsDuringUpdate.Count == 0 )
				Log.Fatal( "Viewport: Update: RendererWorld.viewportsDuringUpdate.Count == 0." );
			if( RenderingSystem.viewportsDuringUpdate[ RenderingSystem.viewportsDuringUpdate.Count - 1 ] != this )
				Log.Fatal( "Viewport: Update: RendererWorld.viewportsDuringUpdate[ RendererWorld.viewportsDuringUpdate.Count - 1 ] != viewport." );
			RenderingSystem.viewportsDuringUpdate.RemoveAt( RenderingSystem.viewportsDuringUpdate.Count - 1 );
		}

		public void PerformUpdateBeforeOutputEvents()
		{
			UpdateBeforeOutput?.Invoke( this );
			AllViewports_UpdateBeforeOutput?.Invoke( this );
		}

		public double LastUpdateTime
		{
			get { return lastUpdateTime; }
		}

		public double LastUpdateTimeStep
		{
			get { return lastUpdateTimeStep; }
		}

		public void ResetLastUpdateTime()
		{
			lastUpdateTime = EngineApp.EngineTime;
			lastUpdateTimeStep = 0;
			previousUpdateTime = lastUpdateTime;
		}

		public double PreviousUpdateTime
		{
			get { return previousUpdateTime; }
		}

		/// <summary>
		/// The list of object labels on the screen that were shown in the last frame.
		/// </summary>
		public List<LastFrameScreenLabelItem> LastFrameScreenLabels
		{
			get { return lastFrameScreenLabels; }
		}

		public bool AllowRenderScreenLabels
		{
			get { return allowRenderScreenLabels; }
			set { allowRenderScreenLabels = value; }
		}

		public Component_RenderingPipeline RenderingPipelineCreated
		{
			get { return renderingPipelineCreated; }
		}

		public void RenderingPipelineCreate( Metadata.TypeInfo type = null )
		{
			RenderingPipelineDestroyCreated();

			if( type == null )
				type = RenderingSystem.RenderingPipelineDefault;
			renderingPipelineCreated = (Component_RenderingPipeline)ComponentUtility.CreateComponent( type, null, true, true );
		}

		public void RenderingPipelineDestroyCreated()
		{
			renderingPipelineCreated?.Dispose();
			renderingPipelineCreated = null;
		}

		public Matrix4 ViewMatrixPreviousFrame
		{
			get
			{
				if( viewMatrixPreviousFrame != null )
					return viewMatrixPreviousFrame.Value;
				if( cameraSettings != null )
					return cameraSettings.ViewMatrix;
				return Matrix4.Identity;
			}
		}

		public Matrix4 ProjectionMatrixPreviousFrame
		{
			get
			{
				if( projectionMatrixPreviousFrame != null )
					return projectionMatrixPreviousFrame.Value;
				if( cameraSettings != null )
					return cameraSettings.ProjectionMatrix;
				return Matrix4.Identity;
			}
		}

		public ColorValue BackgroundColorDefault
		{
			get { return backgroundColorDefault; }
			set { backgroundColorDefault = value; }
		}
	}
}
