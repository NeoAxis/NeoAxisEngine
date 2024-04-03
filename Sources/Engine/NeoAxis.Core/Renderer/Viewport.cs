// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Viewport class, i.e. a rendering region on a render target.
	/// </summary>
	public sealed partial class Viewport
	{
		//!!!!!чтобы не пересоздавалось когда не нужно

		internal RenderTarget parent;

		bool disposed;

		CameraSettingsClass cameraSettings;

		Rectangle dimensions = new Rectangle( 0, 0, 1, 1 );
		bool dimensionsDefault = true;

		ModeEnum mode = ModeEnum.Default;
		Scene attachedScene;

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
		double[] keysDownTime;
		double[] keysUpTime;
		Vector2 mousePosition = new Vector2( -10000, -10000 );
		bool mouseRelativeMode;
		Vector2 restoreMousePosAfterRelativeMode;
		bool mouseRelativeModeSkipOneMouseMove;

		//bool focused;

		double lastUpdateTime;
		double lastUpdateTimeStep;
		double[] lastUpdateTimeStepSmooth = new double[ 7 ];
		double lastUpdateTimeStepSmoothResult;
		double previousUpdateTime;

		Matrix4F? previousFrameViewMatrixRelative;
		Matrix4F? previousFrameProjectionMatrix;
		Vector3? previousFramePosition;
		Vector3F previousFramePositionChange;
		bool instantCameraMovementHappens;

		ViewportRenderingContext renderingContext;
		RenderingPipeline renderingPipelineCreated;

		/// <summary>
		/// Represents an item for <see cref="LastFrameScreenLabels"/> list.
		/// </summary>
		public class LastFrameScreenLabelItem
		{
			public Component Object;
			public float DistanceToCamera;
			public Rectangle ScreenRectangle;
			public ColorValue Color;
			public bool AlwaysVisible;

			public enum ShapeEnum
			{
				Rectangle,
				Ellipse
			}
			public ShapeEnum Shape = ShapeEnum.Ellipse;
		}
		LinkedList<LastFrameScreenLabelItem> lastFrameScreenLabels = new LinkedList<LastFrameScreenLabelItem>();
		Dictionary<Component, LastFrameScreenLabelItem> lastFrameScreenLabelByObjectInSpace = new Dictionary<Component, LastFrameScreenLabelItem>();

		//!!!!new. так?
		bool allowRenderScreenLabels = true;

		ColorValue backgroundColorDefault = new ColorValue( 0, 0, 0 );

		bool insideUpdate;

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
		public delegate void UpdateGetObjectInSceneRenderingContextDelegate( Viewport viewport, ref ObjectInSpace.RenderingContext context );
		public event UpdateGetObjectInSceneRenderingContextDelegate UpdateGetObjectInSceneRenderingContext;
		public static event UpdateGetObjectInSceneRenderingContextDelegate AllViewports_UpdateGetObjectInSceneRenderingContext;

		public event UpdateDefaultDelegate UpdateBeforeOutput;
		public static event UpdateDefaultDelegate AllViewports_UpdateBeforeOutput;

		public event UpdateDefaultDelegate UpdateEnd;
		public static event UpdateDefaultDelegate AllViewports_UpdateEnd;

		///////////////////////////////////////////

		internal Viewport()
		{
			cameraSettings = new CameraSettingsClass( this, 1, 75, 0.1, 10000, Vector3.Zero, Vector3.XAxis, Vector3.ZAxis, ProjectionType.Perspective, 1, 1, 1 );

			lock( RenderingSystem.viewports )
				RenderingSystem.viewports.Add( this );
			//lock( RendererWorld.viewports )
			//	RendererWorld.viewports.Add( (IntPtr)realObject, this );

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
						Log.Fatal( "Viewport: Dispose after shutdown." );

					Disposing?.Invoke( this );

					RenderingPipelineDestroyCreated();

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

		public ModeEnum Mode
		{
			get { return mode; }
			set { mode = value; }
		}

		public Scene AttachedScene
		{
			get { return attachedScene; }
			set { attachedScene = value; }
		}

		public ViewportRenderingContext RenderingContext
		{
			get { return renderingContext; }
			set { renderingContext = value; }
		}

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
				{
					keys = new bool[ GetEKeysMaxIndex() + 1 ];
					keysDownTime = new double[ GetEKeysMaxIndex() + 1 ];
					keysUpTime = new double[ GetEKeysMaxIndex() + 1 ];
				}

				if( !keys[ (int)e.Key ] )
				{
					keys[ (int)e.Key ] = true;
					keysDownTime[ (int)e.Key ] = EngineApp.EngineTime;
				}
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
				{
					keys = new bool[ GetEKeysMaxIndex() + 1 ];
					keysDownTime = new double[ GetEKeysMaxIndex() + 1 ];
					keysUpTime = new double[ GetEKeysMaxIndex() + 1 ];
				}

				if( !keys[ (int)e.Key ] )
					return;

				keys[ (int)e.Key ] = false;
				keysUpTime[ (int)e.Key ] = EngineApp.EngineTime;
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

		public delegate void TouchDelegate( Viewport viewport, TouchData e, ref bool handled );
		public event TouchDelegate Touch;

		public void PerformTouch( TouchData e, ref bool handled )
		{
			EngineThreading.CheckMainThread();

			Touch?.Invoke( this, e, ref handled );
			if( handled )
				return;

			if( uiContainer != null && uiContainer.PerformTouch( e ) )
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

		public double GetKeyDownTime( EKeys key )
		{
			lock( lockerKeysMouse )
			{
				if( keysDownTime != null )
					return keysDownTime[ (int)key ];
				else
					return 0;
			}
		}

		public double GetKeyUpTime( EKeys key )
		{
			lock( lockerKeysMouse )
			{
				if( keysUpTime != null )
					return keysUpTime[ (int)key ];
				else
					return 0;
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

			if( attachedScene != null && attachedScene.Disposed )
				attachedScene = null;

			Tick?.Invoke( this, delta );
			if( attachedScene != null && attachedScene.EnabledInHierarchy )
				attachedScene.PerformUpdate( delta );
			uiContainer?.PerformUpdate( delta );
			uiContainer?.HierarchyController.ProcessDelayedOperations();

			//if( controlManager != null )
			//	controlManager.PerformSpecialInputDeviceEvent( e );
		}

		//!!!!callBgfxFrame always true
		/// <summary>
		/// Updates viewport with the rendering of attached map and GUI rendering.
		/// </summary>
		public void Update( bool callBgfxFrame, CameraSettingsClass overrideCameraSettings = null, int parentCurrentViewNumber = -1 )
		{
			if( insideUpdate )
				return;

			insideUpdate = true;
			try
			{
				if( attachedScene != null && attachedScene.Disposed )
					attachedScene = null;

				//render video to file
				if( EngineApp.RenderVideoToFileData != null && !string.IsNullOrEmpty( EngineApp.RenderVideoToFileData.Camera ) && RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ] == this && AttachedScene != null )
				{
					var c = AttachedScene.GetComponentByPath( EngineApp.RenderVideoToFileData.Camera ) as Camera;
					if( c != null )
						overrideCameraSettings = new CameraSettingsClass( this, c );
				}

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
				if( attachedScene != null && !Scene.AllInstancesEnabledContains( attachedScene ) )
					attachedScene = null;

				//create rendering context
				if( renderingContext == null )
					renderingContext = new ViewportRenderingContext( this );

				//ViewportRenderingContext.current = renderingContext;

				//begin update
				renderingContext.uniquePerFrameObjectToDetectNewFrame = new object();
				renderingContext.ObjectsDuringUpdate = new ViewportRenderingContext.ObjectsDuringUpdateClass();
				renderingContext.AnimationBonesData.Clear();
				renderingContext.AnimationBonesDataTasks.Clear();

				//теперь ниже
				//renderingContext.currentViewNumber = -1;

				previousUpdateTime = lastUpdateTime;

				//!!!!!тут? так?
				//lastUpdateTime
				double time = EngineApp.EngineTime;
				double oldTime = lastUpdateTime;
				lastUpdateTime = time;
				lastUpdateTimeStep = lastUpdateTime - oldTime;

				for( int n = 0; n < lastUpdateTimeStepSmooth.Length - 1; n++ )
					lastUpdateTimeStepSmooth[ n ] = lastUpdateTimeStepSmooth[ n + 1 ];
				lastUpdateTimeStepSmooth[ lastUpdateTimeStepSmooth.Length - 1 ] = lastUpdateTimeStep;

				{
					var result = 0.0;
					for( int n = 0; n < lastUpdateTimeStepSmooth.Length; n++ )
						result += lastUpdateTimeStepSmooth[ n ];
					lastUpdateTimeStepSmoothResult = result / lastUpdateTimeStepSmooth.Length;
				}

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

				//!!!!name
				UpdateBegin?.Invoke( this );
				AllViewports_UpdateBegin?.Invoke( this );

				//!!!!new тут. было ниже
				AttachedScene?.PerformViewportUpdateBegin( this, overrideCameraSettings );

				RenderingPipeline pipeline = null;
				if( cameraSettings != null && cameraSettings.RenderingPipelineOverride != null )
					pipeline = cameraSettings.RenderingPipelineOverride;
				else if( AttachedScene != null )
					pipeline = AttachedScene.RenderingPipeline;

				//render video to file
				if( EngineApp.RenderVideoToFileData != null && !string.IsNullOrEmpty( EngineApp.RenderVideoToFileData.RenderingPipeline ) && RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ] == this && AttachedScene != null )
				{
					var p = AttachedScene.GetComponentByPath( EngineApp.RenderVideoToFileData.RenderingPipeline ) as RenderingPipeline;
					if( p != null )
						pipeline = p;
				}

				if( pipeline == null )
				{
					if( renderingPipelineCreated == null )
						RenderingPipelineCreate();
					pipeline = renderingPipelineCreated;
				}
				else
					RenderingPipelineDestroyCreated();

				//AttachedScene?.PerformViewportUpdateCameraSettingsReady( this );

				if( callBgfxFrame )
					renderingContext.currentViewNumber = -1;
				else
					renderingContext.currentViewNumber = parentCurrentViewNumber;

				AttachedScene?.PerformViewportUpdateCameraSettingsReady( this );


				//!!!!теперь выше
				////!!!!!что там?
				//AttachedScene?.PerformViewportUpdateBegin( this, overrideCameraSettings );


				//rendering data
				//RenderingDataClass renderingData = new RenderingDataClass();

				//{
				//	//InitGeneralRenderingData( renderingData );
				//	Update2_GetRenderingData?.Invoke( this, renderingData );
				//	AllViewports_Update2_GetRenderingData?.Invoke( this, renderingData );
				//	//FinishRenderingData( renderingData );
				//}

				//ComponentScene.OnRender, ObjectInScene.OnRender
				{
					//get rendering context for objects in space
					ObjectInSpace.RenderingContext objectInSpaceRenderingContext = null;
					UpdateGetObjectInSceneRenderingContext?.Invoke( this, ref objectInSpaceRenderingContext );
					AllViewports_UpdateGetObjectInSceneRenderingContext?.Invoke( this, ref objectInSpaceRenderingContext );
					if( objectInSpaceRenderingContext == null )
						objectInSpaceRenderingContext = new ObjectInSpace.RenderingContext( this );

					renderingContext.ObjectInSpaceRenderingContext = objectInSpaceRenderingContext;

					lastFrameScreenLabels.Clear();
					lastFrameScreenLabelByObjectInSpace.Clear();

					//!!!!это правильнее рисовать до transform tool. а значит между двумя UpdateBeforeOutput
					if( AttachedScene != null && AttachedScene.EnabledInHierarchy )
					{
						AttachedScene.PerformRender( this );

						//foreach( var obj in AttachedScene.GetComponents<ObjectInSpace>( false, true, true ) )
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

				renderingContext.renderingPipeline = pipeline as RenderingPipeline_Basic;
				//renderingContext.renderingPipeline = pipeline;
				pipeline.Render( renderingContext );
				renderingContext.renderingPipeline = null;
				//renderingContext.Render( pipeline );// renderingData );

				if( callBgfxFrame )
				{
					RenderingSystem.CallBgfxFrame();
					renderingContext.ResetViews();
				}

				//clear
				simple3DRenderer?.ViewportRendering_Clear();
				canvasRenderer?.ViewportRendering_Clear( LastUpdateTime );

				AttachedScene?.PerformViewportUpdateEnd( this );

				UpdateEnd?.Invoke( this );
				AllViewports_UpdateEnd?.Invoke( this );

				//!!!!по идее, можно не чистить до следующего апдейта
				renderingContext.ObjectsDuringUpdate = null;
				renderingContext.ObjectInSpaceRenderingContext = null;

				renderingContext.MultiRenderTarget_DestroyAll();
				renderingContext.DynamicTexture_FreeAllEndUpdate();
				renderingContext.OcclusionCullingBuffer_FreeAllEndUpdate();
				renderingContext.SceneOcclusionCullingBuffer = null;

				if( cameraSettings != null && previousFramePosition.HasValue )
					previousFramePositionChange = ( cameraSettings.Position - previousFramePosition.Value ).ToVector3F();
				else
					previousFramePositionChange = Vector3F.Zero;

				if( cameraSettings != null )
				{
					previousFrameViewMatrixRelative = cameraSettings.ViewMatrixRelative;
					previousFrameProjectionMatrix = cameraSettings.ProjectionMatrix;
					previousFramePosition = cameraSettings.Position;
				}
				else
				{
					previousFrameViewMatrixRelative = null;
					previousFrameProjectionMatrix = null;
					previousFramePosition = null;
				}
				instantCameraMovementHappens = false;

				//!!!!что чистить?

				//ViewportRenderingContext.current = null;

				if( RenderingSystem.viewportsDuringUpdate.Count == 0 )
					Log.Fatal( "Viewport: Update: RendererWorld.viewportsDuringUpdate.Count == 0." );
				if( RenderingSystem.viewportsDuringUpdate[ RenderingSystem.viewportsDuringUpdate.Count - 1 ] != this )
					Log.Fatal( "Viewport: Update: RendererWorld.viewportsDuringUpdate[ RendererWorld.viewportsDuringUpdate.Count - 1 ] != viewport." );
				RenderingSystem.viewportsDuringUpdate.RemoveAt( RenderingSystem.viewportsDuringUpdate.Count - 1 );
			}
			finally
			{
				insideUpdate = false;
			}
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

		public double LastUpdateTimeStepSmooth
		{
			get { return lastUpdateTimeStepSmoothResult; }
		}

		public void ResetLastUpdateTime()
		{
			lastUpdateTime = EngineApp.EngineTime;
			lastUpdateTimeStep = 0;
			for( int n = 0; n < lastUpdateTimeStepSmooth.Length; n++ )
				lastUpdateTimeStepSmooth[ 0 ] = 0;
			previousUpdateTime = lastUpdateTime;
		}

		public double PreviousUpdateTime
		{
			get { return previousUpdateTime; }
		}

		/// <summary>
		/// The list of object labels on the screen that were shown in the last frame.
		/// </summary>
		public LinkedList<LastFrameScreenLabelItem> LastFrameScreenLabels
		{
			get { return lastFrameScreenLabels; }
		}

		public Dictionary<Component, LastFrameScreenLabelItem> LastFrameScreenLabelByObjectInSpace
		{
			get { return lastFrameScreenLabelByObjectInSpace; }
		}

		public LastFrameScreenLabelItem GetLastFrameScreenLabelByObjectInSpace( Component obj )
		{
			lastFrameScreenLabelByObjectInSpace.TryGetValue( obj, out var item );
			return item;
		}

		public bool AllowRenderScreenLabels
		{
			get { return allowRenderScreenLabels; }
			set { allowRenderScreenLabels = value; }
		}

		public RenderingPipeline RenderingPipelineCreated
		{
			get { return renderingPipelineCreated; }
		}

		public void RenderingPipelineCreate( Metadata.TypeInfo type = null )
		{
			RenderingPipelineDestroyCreated();

			if( type == null )
				type = RenderingSystem.RenderingPipelineBasic;
			renderingPipelineCreated = (RenderingPipeline)ComponentUtility.CreateComponent( type, null, true, true );
		}

		public void RenderingPipelineDestroyCreated()
		{
			renderingPipelineCreated?.Dispose();
			renderingPipelineCreated = null;
		}

		public Matrix4F PreviousFrameViewMatrixRelative
		{
			get
			{
				if( previousFrameViewMatrixRelative.HasValue )
					return previousFrameViewMatrixRelative.Value;
				if( cameraSettings != null )
					return cameraSettings.ViewMatrixRelative;
				return Matrix4F.Identity;
			}
		}

		public Matrix4F PreviousFrameProjectionMatrix
		{
			get
			{
				if( previousFrameProjectionMatrix.HasValue )
					return previousFrameProjectionMatrix.Value;
				if( cameraSettings != null )
					return cameraSettings.ProjectionMatrix;
				return Matrix4F.Identity;
			}
		}

		public Vector3? PreviousFramePosition
		{
			get { return previousFramePosition; }
		}

		public Vector3F PreviousFramePositionChange
		{
			get { return previousFramePositionChange; }
		}

		public bool InstantCameraMovementHappens
		{
			get { return instantCameraMovementHappens; }
		}

		public ColorValue BackgroundColorDefault
		{
			get { return backgroundColorDefault; }
			set { backgroundColorDefault = value; }
		}

		public delegate void InstantCameraMovementDelegate( Viewport viewport );
		public event InstantCameraMovementDelegate InstantCameraMovement;
		public static event InstantCameraMovementDelegate AllViewports_InstantCameraMovement;

		public void NotifyInstantCameraMovement()
		{
			//viewMatrixPreviousFrameAbsolute = null;
			previousFrameViewMatrixRelative = null;
			previousFrameProjectionMatrix = null;
			instantCameraMovementHappens = true;

			InstantCameraMovement?.Invoke( this );
			AllViewports_InstantCameraMovement?.Invoke( this );
		}

		public bool OutputFlipY { get; set; }
		public Viewport OutputViewport { get; set; }

		public object AnyData { get; set; }
	}
}