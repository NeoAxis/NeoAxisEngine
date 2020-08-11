// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using System.IO;
using System.Runtime.InteropServices;
using SharpBgfx;
using NeoAxis.Widget;
using NeoAxis.Input;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a document window with a viewport.
	/// </summary>
	public partial class DocumentWindowWithViewport : DocumentWindow
	{
		//scene
		Component_Scene scene;
		bool sceneNeedDispose;

		//!!!!!
		//[Config( "Sound", "soundVolume" )]
		//static float soundVolume = 0;

		//camera rotating
		bool prepareToCameraRotating;
		Vector2 prepareToCameraRotatingMouseStart;
		bool cameraRotating;

		//screenMessages
		class ScreenMessage
		{
			public string text;
			public ColorValue color;
			public double timeRemaining;
		}
		List<ScreenMessage> screenMessages = new List<ScreenMessage>();

		bool lastTickControlKeyPressed;

		//float fontSizeInPixels;
		//EngineFont editorFont;

		//!!!!
		//Vec3 cameraLastPosition;
		//Quat cameraLastRotation;
		//internal float cameraLastMovedTime;

		float shadowDistanceInPixels = 1;

		string workareaModeName = "";
		WorkareaModeClass workareaMode;

		ObjectCreationMode objectCreationMode;

		/////////////////////////////////////////

		public abstract class WorkareaModeClass
		{
			DocumentWindowWithViewport documentWindow;

			//

			protected WorkareaModeClass( DocumentWindowWithViewport documentWindow )
			{
				this.documentWindow = documentWindow;
			}

			public DocumentWindowWithViewport DocumentWindow
			{
				get { return documentWindow; }
			}

			public virtual bool AllowControlCamera
			{
				get { return true; }
			}

			public virtual bool AllowSelectObjects
			{
				get { return false; }
			}

			public virtual bool DisplaySelectedObjects
			{
				get { return AllowSelectObjects; }
			}

			public virtual bool AllowCreateObjectsByDrop
			{
				get { return AllowSelectObjects; }
			}

			public virtual bool AllowCreateObjectsByClick
			{
				get { return AllowSelectObjects; }
			}

			public virtual bool AllowCreateObjectsByBrush
			{
				get { return AllowSelectObjects; }
			}

			protected virtual void OnDestroy() { }
			public delegate void DestroyDelegate( WorkareaModeClass sender );
			public event DestroyDelegate Destroy;
			internal void PerformDestroy()
			{
				OnDestroy();
				Destroy?.Invoke( this );
			}

			protected virtual void OnGetTextInfoRightBottomCorner( List<string> lines ) { }
			public delegate void GetTextInfoRightBottomCornerDelegate( WorkareaModeClass sender, List<string> lines );
			public event GetTextInfoRightBottomCornerDelegate GetTextInfoRightBottomCorner;
			internal void PerformGetTextInfoRightBottomCorner( List<string> lines )
			{
				OnGetTextInfoRightBottomCorner( lines );
				GetTextInfoRightBottomCorner?.Invoke( this, lines );
			}

			protected virtual bool OnKeyDown( Viewport viewport, KeyEvent e ) { return false; }
			public delegate void KeyDownUpDelegate( WorkareaModeClass sender, Viewport viewport, KeyEvent e, ref bool handled );
			public event KeyDownUpDelegate KeyDown;
			internal bool PerformKeyDown( Viewport viewport, KeyEvent e )
			{
				var handled = OnKeyDown( viewport, e );
				if( !handled )
					KeyDown?.Invoke( this, viewport, e, ref handled );
				return handled;
			}

			protected virtual bool OnKeyPress( Viewport viewport, KeyPressEvent e ) { return false; }
			public delegate void KeyPressDelegate( WorkareaModeClass sender, Viewport viewport, KeyPressEvent e, ref bool handled );
			public event KeyPressDelegate KeyPress;
			internal bool PerformKeyPress( Viewport viewport, KeyPressEvent e )
			{
				var handled = OnKeyPress( viewport, e );
				if( !handled )
					KeyPress?.Invoke( this, viewport, e, ref handled );
				return handled;
			}

			protected virtual bool OnKeyUp( Viewport viewport, KeyEvent e ) { return false; }
			public event KeyDownUpDelegate KeyUp;
			internal bool PerformKeyUp( Viewport viewport, KeyEvent e )
			{
				var handled = OnKeyUp( viewport, e );
				if( !handled )
					KeyUp?.Invoke( this, viewport, e, ref handled );
				return handled;
			}

			protected virtual bool OnMouseDown( Viewport viewport, EMouseButtons button ) { return false; }
			public delegate void MouseClickDelegate( WorkareaModeClass sender, Viewport viewport, EMouseButtons button, ref bool handled );
			public event MouseClickDelegate MouseDown;
			internal bool PerformMouseDown( Viewport viewport, EMouseButtons button )
			{
				var handled = OnMouseDown( viewport, button );
				if( !handled )
					MouseDown?.Invoke( this, viewport, button, ref handled );
				return handled;
			}

			protected virtual bool OnMouseUp( Viewport viewport, EMouseButtons button ) { return false; }
			public event MouseClickDelegate MouseUp;
			internal bool PerformMouseUp( Viewport viewport, EMouseButtons button )
			{
				var handled = OnMouseUp( viewport, button );
				if( !handled )
					MouseUp?.Invoke( this, viewport, button, ref handled );
				return handled;
			}

			protected virtual bool OnMouseDoubleClick( Viewport viewport, EMouseButtons button ) { return false; }
			public event MouseClickDelegate MouseDoubleClick;
			internal bool PerformMouseDoubleClick( Viewport viewport, EMouseButtons button )
			{
				var handled = OnMouseDoubleClick( viewport, button );
				if( !handled )
					MouseDoubleClick?.Invoke( this, viewport, button, ref handled );
				return handled;
			}

			protected virtual void OnMouseMove( Viewport viewport, Vector2 mouse ) { }
			public delegate void MouseMoveDelegate( WorkareaModeClass sender, Viewport viewport, Vector2 mouse );
			public event MouseMoveDelegate MouseMove;
			internal void PerformMouseMove( Viewport viewport, Vector2 mouse )
			{
				OnMouseMove( viewport, mouse );
				MouseMove?.Invoke( this, viewport, mouse );
			}

			protected virtual bool OnMouseRelativeModeChanged( Viewport viewport ) { return false; }
			public delegate void MouseRelativeModeChangedDelegate( WorkareaModeClass sender, Viewport viewport, ref bool handled );
			public event MouseRelativeModeChangedDelegate MouseRelativeModeChanged;
			internal bool PerformMouseRelativeModeChanged( Viewport viewport )
			{
				var handled = OnMouseRelativeModeChanged( viewport );
				if( !handled )
					MouseRelativeModeChanged?.Invoke( this, viewport, ref handled );
				return handled;
			}

			protected virtual bool OnMouseWheel( Viewport viewport, int delta ) { return false; }
			public delegate void MouseWheelDelegate( WorkareaModeClass sender, Viewport viewport, int delta, ref bool handled );
			public event MouseWheelDelegate MouseWheel;
			internal bool PerformMouseWheel( Viewport viewport, int delta )
			{
				var handled = OnMouseWheel( viewport, delta );
				if( !handled )
					MouseWheel?.Invoke( this, viewport, delta, ref handled );
				return handled;
			}

			protected virtual bool OnJoystickEvent( Viewport viewport, JoystickInputEvent e ) { return false; }
			public delegate void JoystickEventDelegate( WorkareaModeClass sender, Viewport viewport, JoystickInputEvent e, ref bool handled );
			public event JoystickEventDelegate JoystickEvent;
			internal bool PerformJoystickEvent( Viewport viewport, JoystickInputEvent e )
			{
				var handled = OnJoystickEvent( viewport, e );
				if( !handled )
					JoystickEvent?.Invoke( this, viewport, e, ref handled );
				return handled;
			}

			protected virtual bool OnSpecialInputDeviceEvent( Viewport viewport, InputEvent e ) { return false; }
			public delegate void SpecialInputDeviceEventDelegate( WorkareaModeClass sender, Viewport viewport, InputEvent e, ref bool handled );
			public event SpecialInputDeviceEventDelegate SpecialInputDeviceEvent;
			internal bool PerformSpecialInputDeviceEvent( Viewport viewport, InputEvent e )
			{
				var handled = OnSpecialInputDeviceEvent( viewport, e );
				if( !handled )
					SpecialInputDeviceEvent?.Invoke( this, viewport, e, ref handled );
				return handled;
			}

			protected virtual void OnTick( Viewport viewport, double delta ) { }
			public delegate void TickDelegate( WorkareaModeClass sender, Viewport viewport, double delta );
			public event TickDelegate Tick;
			internal void PerformTick( Viewport viewport, double delta )
			{
				OnTick( viewport, delta );
				Tick?.Invoke( this, viewport, delta );
			}

			protected virtual void OnUpdateBegin( Viewport viewport ) { }
			public delegate void UpdateBeginDelegate( WorkareaModeClass sender, Viewport viewport );
			public event UpdateBeginDelegate UpdateBegin;
			internal void PerformUpdateBegin( Viewport viewport )
			{
				OnUpdateBegin( viewport );
				UpdateBegin?.Invoke( this, viewport );
			}

			protected virtual void OnUpdateGetObjectInSceneRenderingContext( Viewport viewport, ref Component_ObjectInSpace.RenderingContext context ) { }
			public delegate void UpdateGetObjectInSceneRenderingContextDelegate( WorkareaModeClass sender, Viewport viewport, ref Component_ObjectInSpace.RenderingContext context );
			public event UpdateGetObjectInSceneRenderingContextDelegate UpdateGetObjectInSceneRenderingContext;
			internal void PerformUpdateGetObjectInSceneRenderingContext( Viewport viewport, ref Component_ObjectInSpace.RenderingContext context )
			{
				OnUpdateGetObjectInSceneRenderingContext( viewport, ref context );
				UpdateGetObjectInSceneRenderingContext?.Invoke( this, viewport, ref context );
			}

			protected virtual void OnUpdateBeforeOutput( Viewport viewport ) { }
			public delegate void UpdateBeforeOutputDelegate( WorkareaModeClass sender, Viewport viewport );
			public event UpdateBeforeOutputDelegate UpdateBeforeOutput;
			internal void PerformUpdateBeforeOutput( Viewport viewport )
			{
				OnUpdateBeforeOutput( viewport );
				UpdateBeforeOutput?.Invoke( this, viewport );
			}

			protected virtual void OnUpdateBeforeOutput2( Viewport viewport ) { }
			public delegate void UpdateBeforeOutput2Delegate( WorkareaModeClass sender, Viewport viewport );
			public event UpdateBeforeOutput2Delegate UpdateBeforeOutput2;
			internal void PerformUpdateBeforeOutput2( Viewport viewport )
			{
				OnUpdateBeforeOutput2( viewport );
				UpdateBeforeOutput2?.Invoke( this, viewport );
			}

			protected virtual void OnUpdateEnd( Viewport viewport ) { }
			public delegate void UpdateEndDelegate( WorkareaModeClass sender, Viewport viewport );
			public event UpdateEndDelegate UpdateEnd;
			internal void PerformUpdateEnd( Viewport viewport )
			{
				OnUpdateEnd( viewport );
				UpdateEnd?.Invoke( this, viewport );
			}

			protected virtual void OnViewportUpdateGetCameraSettings( ref Component_Camera camera ) { }
			public delegate void ViewportUpdateGetCameraSettingsDelegate( WorkareaModeClass sender, ref Component_Camera camera );
			public event ViewportUpdateGetCameraSettingsDelegate ViewportUpdateGetCameraSettings;
			internal void PerformViewportUpdateGetCameraSettings( ref Component_Camera camera )
			{
				OnViewportUpdateGetCameraSettings( ref camera );
				ViewportUpdateGetCameraSettings?.Invoke( this, ref camera );
			}

			//!!!!надо ли ref bool handled
			protected virtual void OnEditorActionGetState( EditorAction.GetStateContext context ) { }
			public delegate void EditorActionGetStateDelegate( WorkareaModeClass sender, EditorAction.GetStateContext context );
			public event EditorActionGetStateDelegate EditorActionGetState;
			internal void PerformEditorActionGetState( EditorAction.GetStateContext context )
			{
				OnEditorActionGetState( context );
				EditorActionGetState?.Invoke( this, context );
			}

			//!!!!надо ли ref bool handled
			protected virtual void OnEditorActionClick( EditorAction.ClickContext context ) { }
			public delegate void EditorActionClickDelegate( WorkareaModeClass sender, EditorAction.ClickContext context );
			public event EditorActionClickDelegate EditorActionClick;
			internal void PerformEditorActionClick( EditorAction.ClickContext context )
			{
				OnEditorActionClick( context );
				EditorActionClick?.Invoke( this, context );
			}
		}

		/////////////////////////////////////////

		public DocumentWindowWithViewport()
		{
			InitializeComponent();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			//CalculateFontSize();

			viewportControl.ViewportCreated += ViewportControl_ViewportCreated;
			viewportControl.ViewportDestroyed += ViewportControl_ViewportDestroyed;

			shadowDistanceInPixels = EditorAPI.DPI > 96 ? 2 : 1;
		}

		[Browsable( false )]
		public EngineViewportControl ViewportControl
		{
			get { return viewportControl; }
		}

		[Browsable( false )]
		public Viewport Viewport
		{
			get { return ViewportControl?.Viewport; }
		}

		protected virtual void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			Viewport viewport = sender.Viewport;

			viewport.KeyDown += Viewport_KeyDown;
			viewport.KeyPress += Viewport_KeyPress;
			viewport.KeyUp += Viewport_KeyUp;
			viewport.MouseDown += Viewport_MouseDown;
			viewport.MouseUp += Viewport_MouseUp;
			viewport.MouseDoubleClick += Viewport_MouseDoubleClick;
			viewport.MouseMove += Viewport_MouseMove;
			viewport.MouseRelativeModeChanged += Viewport_MouseRelativeModeChanged;
			viewport.MouseWheel += Viewport_MouseWheel;
			viewport.JoystickEvent += Viewport_JoystickEvent;
			viewport.SpecialInputDeviceEvent += Viewport_SpecialInputDeviceEvent;
			viewport.Tick += Viewport_Tick;
			viewport.UpdateBegin += Viewport_UpdateBegin;
			viewport.UpdateGetObjectInSceneRenderingContext += Viewport_UpdateGetObjectInSceneRenderingContext;
			viewport.UpdateBeforeOutput += Viewport_UpdateBeforeOutput;
			viewport.UpdateBeforeOutput += Viewport_UpdateBeforeOutput2;
			viewport.UpdateEnd += Viewport_UpdateEnd;

			//connect scene to viewport
			ViewportControl.Viewport.AttachedScene = scene;
		}

		protected virtual void Viewport_KeyDown( Viewport viewport, KeyEvent e, ref bool handled )
		{
			if( objectCreationMode != null && objectCreationMode.PerformKeyDown( viewport, e ) )
			{
				handled = true;
				return;
			}
			if( workareaMode != null && workareaMode.PerformKeyDown( viewport, e ) )
			{
				handled = true;
				return;
			}

			if( EditorAPI.ProcessShortcuts( (Keys)e.Key ) )
			{
				handled = true;
				return;
			}

			//if( MainForm.Instance.ToolsProcessKeyDownHotKeys( (Keys)e.Key, Form.ModifierKeys, true ) )
			//	return true;

			//return base.OnKeyDown( e );
		}

		protected virtual void Viewport_KeyPress( Viewport viewport, KeyPressEvent e, ref bool handled )
		{
			if( objectCreationMode != null && objectCreationMode.PerformKeyPress( viewport, e ) )
			{
				handled = true;
				return;
			}
			if( workareaMode != null && workareaMode.PerformKeyPress( viewport, e ) )
			{
				handled = true;
				return;
			}

			//!!!!
			//if( controlManager != null && !IsControlManagerLocked() )
			//	if( controlManager.DoKeyPress( e ) )
			//		return true;

		}

		protected virtual void Viewport_KeyUp( Viewport viewport, KeyEvent e, ref bool handled )
		{
			if( objectCreationMode != null && objectCreationMode.PerformKeyUp( viewport, e ) )
			{
				handled = true;
				return;
			}
			if( workareaMode != null && workareaMode.PerformKeyUp( viewport, e ) )
			{
				handled = true;
				return;
			}

			//!!!!!

			//!!!!было, но надо ли было
			////Alt key released
			//if( lastTickAltKeyPressed && ( System.Windows.Forms.Control.ModifierKeys & Keys.Alt ) == 0 )
			//{
			//	if( cameraRotating )
			//	{
			//		handled = true;
			//		return;
			//	}
			//}

			//if( controlManager != null && !IsControlManagerLocked() )
			//	if( controlManager.DoKeyUp( e ) )
			//		return true;

			//if( Map.Instance != null )
			//{
			//	if( EntitiesEditManager.Instance != null )
			//		if( EntitiesEditManager.Instance.OnKeyUp( e ) )
			//			return true;
			//}

			//return base.OnKeyUp( e );


		}

		protected virtual void Viewport_MouseDown( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			//!!!!!

			//MainForm.Instance.EngineAppControl.Focus();

			//if( controlManager != null && !IsControlManagerLocked() )
			//	if( controlManager.DoMouseDown( button ) )
			//		return true;

			//if( Map.Instance != null )
			//{
			//	if( EntitiesEditManager.Instance != null )
			//		if( EntitiesEditManager.Instance.OnMouseDown( button ) )
			//			return true;
			//}

			//&& ( workareaMode == null || workareaMode.AllowControlCamera )

			if( scene != null && AllowCameraControl )
			{
				//!!!!было
				if( button == EMouseButtons.Right || button == EMouseButtons.Middle )
				//if( ( button == EMouseButtons.Left && lastTickAltKeyPressed ) || button == EMouseButtons.Middle || button == EMouseButtons.Right )
				{
					prepareToCameraRotating = true;
					prepareToCameraRotatingMouseStart = viewport.MousePosition;

					//!!!!было, но пока еще ведь не наступило
					//handled = true;
					//return;
				}
			}

			if( objectCreationMode != null && objectCreationMode.PerformMouseDown( viewport, button ) )
			{
				handled = true;
				return;
			}
			if( workareaMode != null && workareaMode.PerformMouseDown( viewport, button ) )
			{
				handled = true;
				return;
			}

			//return base.OnMouseDown( button );
		}

		protected virtual void Viewport_MouseUp( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			//!!!!

			//if( controlManager != null && !IsControlManagerLocked() )
			//	if( controlManager.DoMouseUp( button ) )
			//		return true;

			if( scene != null && AllowCameraControl )
			{
				//!!!!
				if( button == EMouseButtons.Right || button == EMouseButtons.Middle )
				//if( button == EMouseButtons.Left || button == EMouseButtons.Right || button == EMouseButtons.Middle )
				{
					prepareToCameraRotating = false;

					if( cameraRotating )
					{
						viewport.MouseRelativeMode = false;
						cameraRotating = false;

						handled = true;
					}
				}
			}

			if( objectCreationMode != null && objectCreationMode.PerformMouseUp( viewport, button ) )
			{
				handled = true;
				return;
			}
			if( workareaMode != null && workareaMode.PerformMouseUp( viewport, button ) )
			{
				handled = true;
				return;
			}

			//if( Map.Instance != null )
			//{
			//	if( EntitiesEditManager.Instance != null )
			//		if( EntitiesEditManager.Instance.OnMouseUp( button ) )
			//			return true;
			//}

			//return base.OnMouseUp( button );

		}

		protected virtual void Viewport_MouseDoubleClick( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			if( objectCreationMode != null && objectCreationMode.PerformMouseDoubleClick( viewport, button ) )
			{
				handled = true;
				return;
			}
			if( workareaMode != null && workareaMode.PerformMouseDoubleClick( viewport, button ) )
			{
				handled = true;
				return;
			}

			//!!!!

			//MainForm.Instance.EngineAppControl.Focus();

			//if( controlManager != null && !IsControlManagerLocked() )
			//	if( controlManager.DoMouseDoubleClick( button ) )
			//		return true;

			//if( Map.Instance != null )
			//{
			//	if( EntitiesEditManager.Instance != null )
			//		if( EntitiesEditManager.Instance.OnMouseDoubleClick( button ) )
			//			return true;
			//}

			//return base.OnMouseDoubleClick( button );

		}

		protected virtual void Viewport_MouseMove( Viewport viewport, Vector2 mouse )//, ref bool handled )
		{
			//!!!!!
			//if( DirectInput.DirectInputMouseDevice.Instance != null )
			//{
			//	DirectInput.DirectInputMouseDevice.State state = DirectInput.DirectInputMouseDevice.Instance.GetState();
			//	Log.Info( state.Position.ToString() );
			//}


			//!!!!!

			//!!!!какие-то нюансы были с FPS и скоростью вращения

			//base.OnMouseMove( mouse );

			//if( controlManager != null && !IsControlManagerLocked() )
			//	controlManager.DoMouseMove( mouse );

			//prepare for camera rotation
			if( scene != null && prepareToCameraRotating && AllowCameraControl )
			{
				//!!!!по сути не надо проверять. оно само должно
				if( viewport.IsMouseButtonPressed( EMouseButtons.Right ) || viewport.IsMouseButtonPressed( EMouseButtons.Middle ) )
				//if( ( viewport.IsMouseButtonPressed( EMouseButtons.Left ) && lastTickAltKeyPressed ) ||
				//	 viewport.IsMouseButtonPressed( EMouseButtons.Middle ) ||
				//	 viewport.IsMouseButtonPressed( EMouseButtons.Right ) )
				{
					Vector2 diffPixels = ( mouse - prepareToCameraRotatingMouseStart ) * viewport.SizeInPixels.ToVector2();
					if( Math.Abs( diffPixels.X ) >= 3 || Math.Abs( diffPixels.Y ) >= 3 )
					{
						prepareToCameraRotating = false;

						cameraRotating = true;
						viewport.MouseRelativeMode = true;

						//!!!!было
						//return;
					}
				}
			}

			//process camera rotation
			if( scene != null && viewport.MouseRelativeMode && AllowCameraControl )
			{
				Component_Camera camera = scene.Mode.Value == Component_Scene.ModeEnum._3D ? scene.CameraEditor : scene.CameraEditor2D;
				if( camera != null && cameraRotating )
				{
					if( scene.Mode.Value == Component_Scene.ModeEnum._3D )
					{
						double coef = .001;

						//Move front/back
						if( viewport.IsMouseButtonPressed( EMouseButtons.Right ) && lastTickControlKeyPressed )
						{
							double sens = ( ( Form.ModifierKeys & Keys.Shift ) != 0 ) ? ProjectSettings.Get.CameraMouseMovementSensitivityFast : ProjectSettings.Get.CameraMouseMovementSensitivityNormal;
							double offset = ( mouse.X + mouse.Y ) * sens * coef * 5.0;

							Transform t = camera.Transform;
							camera.Transform = t.UpdatePosition( t.Position + t.Rotation.GetForward() * offset );
						}

						// Orbit rotate
						if( viewport.IsMouseButtonPressed( EMouseButtons.Left ) && lastTickControlKeyPressed )
						{
							//TODO: implement orbit rotation
						}

						//Free rotate
						if( viewport.IsMouseButtonPressed( EMouseButtons.Right ) && !lastTickControlKeyPressed )
						{
							Transform t = camera.Transform;
							SphericalDirection dir = SphericalDirection.FromVector( t.Rotation.GetForward() );

							dir.Horizontal -= mouse.X * ProjectSettings.Get.CameraMouseRotationSensitivityHorizontal.Value * coef;
							dir.Vertical -= mouse.Y * ProjectSettings.Get.CameraMouseRotationSensitivityVertical.Value * coef;
							//if( ( Form.ModifierKeys & Keys.Shift ) != 0 )
							//{
							//	sensHorizontal = ProjectSettings.Get.CameraMouseRotationSensitivityHorizontalFast;
							//	sensVertical = ProjectSettings.Get.CameraMouseRotationSensitivityVerticalFast;
							//}
							//else
							//{
							//	sensHorizontal = ProjectSettings.Get.CameraMouseRotationSensitivityHorizontalNormal;
							//	sensVertical = ProjectSettings.Get.CameraMouseRotationSensitivityVerticalNormal;
							//}
							//dir.Horizontal -= mouse.X * sensHorizontal * coef;
							//dir.Vertical -= mouse.Y * sensVertical * coef;

							dir.Horizontal = MathEx.RadianNormalize360( dir.Horizontal );

							const double vlimit = Math.PI / 2 - .01;
							if( dir.Vertical > vlimit ) dir.Vertical = vlimit;
							if( dir.Vertical < -vlimit ) dir.Vertical = -vlimit;

							camera.Transform = t.UpdateRotation( Quaternion.LookAt( dir.GetVector(), camera.FixedUp ) );
						}

						//Move horizontal/vertical (Track)
						if( viewport.IsMouseButtonPressed( EMouseButtons.Middle ) )
						{
							double sens = ( ( Form.ModifierKeys & Keys.Shift ) != 0 ) ? ProjectSettings.Get.CameraMouseTrackMovementSensitivityFast : ProjectSettings.Get.CameraMouseTrackMovementSensitivityNormal;

							double offsetHorizontal = mouse.X * sens * coef;
							double offsetVertical = -mouse.Y * sens * coef;

							Transform t = camera.Transform;
							Vector3 dir = t.Rotation.GetForward();

							Vector3 up = camera.FixedUp;
							Vector3 left = Vector3.Cross( dir, up );

							camera.Transform = t.UpdatePosition( t.Position + left * offsetHorizontal + up * offsetVertical );
						}

						////focus camera to object
						//if( IsKeyPressed( EKeys.F ) && ( Form.ModifierKeys & Keys.Control ) == 0 && ( Form.ModifierKeys & Keys.Alt ) == 0 )
						//	MainForm.Instance.FocusCameraToObject();
					}
					else
					{
						////Move front/back
						//if( viewport.IsMouseButtonPressed( EMouseButtons.Right ) && lastTickControlKeyPressed )
						//{
						//	double sens = ( ( Form.ModifierKeys & Keys.Shift ) != 0 ) ? ProjectSettings.Get.CameraMouseMovementSensitivityFast : ProjectSettings.Get.CameraMouseMovementSensitivityNormal;
						//	double offset = ( mouse.X + mouse.Y ) * sens * coef * 5.0;

						//	Transform t = camera.Transform;
						//	camera.Transform = t.UpdatePosition( t.Position + t.Rotation.GetForward() * offset );
						//}

						//Movement
						if( viewport.IsMouseButtonPressed( EMouseButtons.Right ) && !lastTickControlKeyPressed )
						{
							Transform t = camera.Transform;

							var pos = t.Position;

							double sens = ( ( Form.ModifierKeys & Keys.Shift ) != 0 ) ? ProjectSettings.Get.CameraMouseMovementSensitivityFast : ProjectSettings.Get.CameraMouseMovementSensitivityNormal;

							pos.X += mouse.X * sens * 0.005;
							pos.Y -= mouse.Y * sens * 0.005;

							camera.Transform = t.UpdatePosition( pos );
						}

						////Move horizontal/vertical (Track)
						//if( viewport.IsMouseButtonPressed( EMouseButtons.Middle ) )
						//{
						//	double sens = ( ( Form.ModifierKeys & Keys.Shift ) != 0 ) ? ProjectSettings.Get.CameraMouseTrackMovementSensitivityFast : ProjectSettings.Get.CameraMouseTrackMovementSensitivityNormal;

						//	double offsetHorizontal = mouse.X * sens * coef;
						//	double offsetVertical = -mouse.Y * sens * coef;

						//	Transform t = camera.Transform;
						//	Vector3 dir = t.Rotation.GetForward();

						//	Vector3 up = camera.FixedUp;
						//	Vector3 left = Vector3.Cross( dir, up );

						//	camera.Transform = t.UpdatePosition( t.Position + left * offsetHorizontal + up * offsetVertical );
						//}

					}
				}
			}

			workareaMode?.PerformMouseMove( viewport, mouse );
			objectCreationMode?.PerformMouseMove( viewport, mouse );
		}

		protected virtual void Viewport_MouseRelativeModeChanged( Viewport viewport, ref bool handled )
		{
			if( objectCreationMode != null && objectCreationMode.PerformMouseRelativeModeChanged( viewport ) )
			{
				handled = true;
				return;
			}
			if( workareaMode != null && workareaMode.PerformMouseRelativeModeChanged( viewport ) )
			{
				handled = true;
				return;
			}
		}

		protected virtual void Viewport_MouseWheel( Viewport viewport, int delta, ref bool handled )
		{
			if( scene != null && AllowCameraControl )
			{
				Component_Camera camera = scene.Mode.Value == Component_Scene.ModeEnum._3D ? scene.CameraEditor : scene.CameraEditor2D;
				if( camera != null )
				{
					double sens = ( ( Form.ModifierKeys & Keys.Shift ) != 0 ) ? ProjectSettings.Get.CameraMouseWheelMovementSensitivityFast : ProjectSettings.Get.CameraMouseWheelMovementSensitivityNormal;

					if( scene.Mode.Value == Component_Scene.ModeEnum._3D )
					{
						Transform t = camera.Transform;
						camera.Transform = t.UpdatePosition( t.Position + t.Rotation.GetForward() * (double)delta * .003 * sens );
					}
					else
					{
						double height = camera.Height;
						height -= (double)delta * sens * 0.01;
						if( height < 0.01 )
							height = 0.01;
						camera.Height = height;
					}
				}
			}

			if( objectCreationMode != null && objectCreationMode.PerformMouseWheel( viewport, delta ) )
			{
				handled = true;
				return;
			}
			if( workareaMode != null && workareaMode.PerformMouseWheel( viewport, delta ) )
			{
				handled = true;
				return;
			}
		}

		protected virtual void Viewport_JoystickEvent( Viewport viewport, JoystickInputEvent e, ref bool handled )
		{
			if( objectCreationMode != null && objectCreationMode.PerformJoystickEvent( viewport, e ) )
			{
				handled = true;
				return;
			}
			if( workareaMode != null && workareaMode.PerformJoystickEvent( viewport, e ) )
			{
				handled = true;
				return;
			}
		}

		protected virtual void Viewport_SpecialInputDeviceEvent( Viewport viewport, InputEvent e, ref bool handled )
		{
			if( objectCreationMode != null && objectCreationMode.PerformSpecialInputDeviceEvent( viewport, e ) )
			{
				handled = true;
				return;
			}
			if( workareaMode != null && workareaMode.PerformSpecialInputDeviceEvent( viewport, e ) )
			{
				handled = true;
				return;
			}
		}

		protected virtual void Viewport_Tick( Viewport viewport, float delta )
		{
			if( viewportControl != null )
				viewportControl.AutomaticUpdateFPS = (float)ProjectSettings.Get.MaxFramesPerSecondDocument;

			//!!!!!

			//!!!!где еще такое нужно?
			//process delayed operations
			{
				var component = ObjectOfWindow as Component;
				if( component != null && component.HierarchyController != null )
					component.HierarchyController.ProcessDelayedOperations();
			}
			//if( scene != null && scene.HierarchyController != null )
			//	scene.HierarchyController.ProcessDelayedOperations();

			lastTickControlKeyPressed = ( System.Windows.Forms.Control.ModifierKeys & Keys.Control ) != 0;
			//lastTickAltKeyPressed = ( System.Windows.Forms.Control.ModifierKeys & Keys.Alt ) != 0;

			//if( controlManager != null )
			//	controlManager.DoTick( delta );

			//if( EntitiesEditManager.Instance != null )
			//	EntitiesEditManager.Instance.OnTick( delta );

			//camera control
			if( scene != null && ( System.Windows.Forms.Control.ModifierKeys & Keys.Alt ) == 0 && AllowCameraControl )
			{
				Component_Camera camera = scene.Mode.Value == Component_Scene.ModeEnum._3D ? scene.CameraEditor : scene.CameraEditor2D;
				if( camera != null )
				{
					Transform transformOriginal = camera.Transform.Value;
					Vector3 posOriginal = transformOriginal.Position;
					SphericalDirection dirOriginal = SphericalDirection.FromVector( transformOriginal.Rotation.GetForward() );
					if( scene.Mode.Value == Component_Scene.ModeEnum._2D && Math.Abs( camera.FixedUp.Value.Z ) < 0.001 )
						dirOriginal = SphericalDirection.FromVector( new Vector3( 0, 0, -1 ) );

					Vector3 pos = posOriginal;
					SphericalDirection dir = dirOriginal;
					double heightOriginal = camera.Height;
					double height = heightOriginal;


					double movementSpeed;
					double rotationSpeed;
					if( ( System.Windows.Forms.Control.ModifierKeys & Keys.Shift ) != 0 || viewport.IsKeyLocked( EKeys.CapsLock ) )
					{
						movementSpeed = ProjectSettings.Get.CameraKeyboardMovementSpeedFast;
						rotationSpeed = ProjectSettings.Get.CameraKeyboardRotationSpeedFast.Value.InRadians();
					}
					else
					{
						movementSpeed = ProjectSettings.Get.CameraKeyboardMovementSpeedNormal;
						rotationSpeed = ProjectSettings.Get.CameraKeyboardRotationSpeedNormal.Value.InRadians();
					}
					var moveStep = movementSpeed * delta;
					var rotateStep = rotationSpeed * delta;
					//!!!!parameter to settings?
					var heightScale = moveStep * 4;

					if( ( Form.ModifierKeys & Keys.Control ) != 0 )
					{
						//rotate
						if( viewport.IsKeyPressed( EKeys.Up ) || viewport.IsKeyPressed( EKeys.NumPad8 ) )
							dir.Vertical += rotateStep;
						if( viewport.IsKeyPressed( EKeys.Down ) || viewport.IsKeyPressed( EKeys.NumPad2 ) )
							dir.Vertical -= rotateStep;
						if( viewport.IsKeyPressed( EKeys.Left ) || viewport.IsKeyPressed( EKeys.NumPad4 ) )
							dir.Horizontal += rotateStep;
						if( viewport.IsKeyPressed( EKeys.Right ) || viewport.IsKeyPressed( EKeys.NumPad6 ) )
							dir.Horizontal -= rotateStep;

						dir.Horizontal = MathEx.RadianNormalize360( dir.Horizontal );
						double vlimit = Math.PI / 2 - .01;
						if( dir.Vertical > vlimit ) dir.Vertical = vlimit;
						if( dir.Vertical < -vlimit ) dir.Vertical = -vlimit;
					}
					else
					{
						//move
						if( scene.Mode.Value == Component_Scene.ModeEnum._3D )
						{
							if( viewport.IsMouseButtonPressed( EMouseButtons.Left ) || viewport.IsMouseButtonPressed( EMouseButtons.Middle ) || viewport.IsMouseButtonPressed( EMouseButtons.Right ) )
							{
								if( viewport.IsKeyPressed( EKeys.W ) || viewport.IsKeyPressed( EKeys.NumPad8 ) )
									pos += dir.GetVector() * moveStep;
								if( ( viewport.IsKeyPressed( EKeys.S ) ) || viewport.IsKeyPressed( EKeys.NumPad2 ) )
									pos -= dir.GetVector() * moveStep;
								if( viewport.IsKeyPressed( EKeys.A ) || viewport.IsKeyPressed( EKeys.NumPad4 ) )
									pos += new SphericalDirection( dir.Horizontal + Math.PI / 2, 0 ).GetVector() * moveStep;
								if( viewport.IsKeyPressed( EKeys.D ) || viewport.IsKeyPressed( EKeys.NumPad6 ) )
									pos += new SphericalDirection( dir.Horizontal - Math.PI / 2, 0 ).GetVector() * moveStep;
								if( viewport.IsKeyPressed( EKeys.E ) || viewport.IsKeyPressed( EKeys.NumPad9 ) )
									pos += new SphericalDirection( dir.Horizontal, dir.Vertical + Math.PI / 2 ).GetVector() * moveStep;
								if( viewport.IsKeyPressed( EKeys.Q ) || viewport.IsKeyPressed( EKeys.NumPad3 ) )
									pos += new SphericalDirection( dir.Horizontal, dir.Vertical - Math.PI / 2 ).GetVector() * moveStep;
							}

							if( viewport.IsKeyPressed( EKeys.Up ) )
								pos += dir.GetVector() * moveStep;
							if( viewport.IsKeyPressed( EKeys.Down ) )
								pos -= dir.GetVector() * moveStep;
							if( viewport.IsKeyPressed( EKeys.Left ) )
								pos += new SphericalDirection( dir.Horizontal + Math.PI / 2, 0 ).GetVector() * moveStep;
							if( viewport.IsKeyPressed( EKeys.Right ) )
								pos += new SphericalDirection( dir.Horizontal - Math.PI / 2, 0 ).GetVector() * moveStep;
							if( viewport.IsKeyPressed( EKeys.PageUp ) )
								pos += new SphericalDirection( dir.Horizontal, dir.Vertical + Math.PI / 2 ).GetVector() * moveStep;
							if( viewport.IsKeyPressed( EKeys.PageDown ) )
								pos += new SphericalDirection( dir.Horizontal, dir.Vertical - Math.PI / 2 ).GetVector() * moveStep;

							////move
							//if( viewport.IsMouseButtonPressed( EMouseButtons.Left ) || viewport.IsMouseButtonPressed( EMouseButtons.Middle ) ||
							//	viewport.IsMouseButtonPressed( EMouseButtons.Right ) )
							//{
							//	if( viewport.IsKeyPressed( EKeys.W ) || viewport.IsKeyPressed( EKeys.Up ) || viewport.IsKeyPressed( EKeys.NumPad8 ) )
							//		pos += dir.GetVector() * moveStep;
							//	if( ( viewport.IsKeyPressed( EKeys.S ) ) || viewport.IsKeyPressed( EKeys.Down ) || viewport.IsKeyPressed( EKeys.NumPad2 ) )
							//		pos -= dir.GetVector() * moveStep;
							//	if( viewport.IsKeyPressed( EKeys.A ) || viewport.IsKeyPressed( EKeys.Left ) || viewport.IsKeyPressed( EKeys.NumPad4 ) )
							//		pos += new SphereDir( dir.Horizontal + Math.PI / 2, 0 ).GetVector() * moveStep;
							//	if( viewport.IsKeyPressed( EKeys.D ) || viewport.IsKeyPressed( EKeys.Right ) || viewport.IsKeyPressed( EKeys.NumPad6 ) )
							//		pos += new SphereDir( dir.Horizontal - Math.PI / 2, 0 ).GetVector() * moveStep;
							//	if( viewport.IsKeyPressed( EKeys.E ) || viewport.IsKeyPressed( EKeys.PageUp ) || viewport.IsKeyPressed( EKeys.NumPad9 ) )
							//		pos += new SphereDir( dir.Horizontal, dir.Vertical + Math.PI / 2 ).GetVector() * moveStep;
							//	if( viewport.IsKeyPressed( EKeys.Q ) || viewport.IsKeyPressed( EKeys.PageDown ) || viewport.IsKeyPressed( EKeys.NumPad3 ) )
							//		pos += new SphereDir( dir.Horizontal, dir.Vertical - Math.PI / 2 ).GetVector() * moveStep;
							//}
						}
						else
						{
							if( viewport.IsMouseButtonPressed( EMouseButtons.Left ) || viewport.IsMouseButtonPressed( EMouseButtons.Middle ) || viewport.IsMouseButtonPressed( EMouseButtons.Right ) )
							{
								if( viewport.IsKeyPressed( EKeys.W ) || viewport.IsKeyPressed( EKeys.NumPad8 ) )
									pos += new SphericalDirection( dir.Horizontal + Math.PI / 2, 0 ).GetVector() * moveStep;
								if( ( viewport.IsKeyPressed( EKeys.S ) ) || viewport.IsKeyPressed( EKeys.NumPad2 ) )
									pos += new SphericalDirection( dir.Horizontal - Math.PI / 2, 0 ).GetVector() * moveStep;
								if( viewport.IsKeyPressed( EKeys.A ) || viewport.IsKeyPressed( EKeys.NumPad4 ) )
									pos += new SphericalDirection( dir.Horizontal, dir.Vertical - Math.PI / 2 ).GetVector() * moveStep;
								if( viewport.IsKeyPressed( EKeys.D ) || viewport.IsKeyPressed( EKeys.NumPad6 ) )
									pos += new SphericalDirection( dir.Horizontal, dir.Vertical + Math.PI / 2 ).GetVector() * moveStep;
								if( viewport.IsKeyPressed( EKeys.E ) || viewport.IsKeyPressed( EKeys.NumPad9 ) )
									height += heightScale;
								if( viewport.IsKeyPressed( EKeys.Q ) || viewport.IsKeyPressed( EKeys.NumPad3 ) )
									height -= heightScale;
							}

							if( viewport.IsKeyPressed( EKeys.Up ) )
								pos += new SphericalDirection( dir.Horizontal + Math.PI / 2, 0 ).GetVector() * moveStep;
							if( viewport.IsKeyPressed( EKeys.Down ) )
								pos += new SphericalDirection( dir.Horizontal - Math.PI / 2, 0 ).GetVector() * moveStep;
							if( viewport.IsKeyPressed( EKeys.Left ) )
								pos += new SphericalDirection( dir.Horizontal, dir.Vertical - Math.PI / 2 ).GetVector() * moveStep;
							if( viewport.IsKeyPressed( EKeys.Right ) )
								pos += new SphericalDirection( dir.Horizontal, dir.Vertical + Math.PI / 2 ).GetVector() * moveStep;
							if( viewport.IsKeyPressed( EKeys.PageUp ) )
								height -= heightScale;
							if( viewport.IsKeyPressed( EKeys.PageDown ) )
								height += heightScale;
						}
					}

					if( scene.Mode.Value == Component_Scene.ModeEnum._2D )
					{
						pos.Z = scene.CameraEditor2DPositionZ;
						if( height < 0.01 )
							height = 0.01;
					}

					if( pos != posOriginal || dir != dirOriginal )
					{
						var t = transformOriginal.UpdatePosition( pos );
						if( dir != dirOriginal )
						{
							var dirVector = dir.GetVector();
							if( scene.Mode.Value == Component_Scene.ModeEnum._2D && Math.Abs( camera.FixedUp.Value.Z ) < 0.001 )
								dirVector = new Vector3( 0, 0, -1 );
							t = t.UpdateRotation( Quaternion.LookAt( dirVector, camera.FixedUp ) );
						}
						camera.Transform = t;
					}
					if( height != heightOriginal )
						camera.Height = height;
				}
			}

			//screenMessages
			{
				for( int n = 0; n < screenMessages.Count; n++ )
				{
					screenMessages[ n ].timeRemaining -= delta;
					if( screenMessages[ n ].timeRemaining <= 0 )
					{
						screenMessages.RemoveAt( n );
						n--;
					}
				}
			}

			workareaMode?.PerformTick( viewport, delta );
			objectCreationMode?.PerformTick( viewport, delta );

			//!!!!
			////mapCameraCurveTime
			//if( !IsKeyPressed( EKeys.C ) )
			//	mapCameraCurveTime = -100000.0f;
			//if( mapCameraCurveTime > -100000.0f )
			//	mapCameraCurveTime += delta;
		}

		protected virtual void Viewport_UpdateBegin( Viewport viewport )
		{
			workareaMode?.PerformUpdateBegin( viewport );
			objectCreationMode?.PerformUpdateBegin( viewport );
		}

		void DrawGetTextInfoLeftTopCorner()
		{
			//!!!!отключать?

			var lines = new List<string>();
			GetTextInfoLeftTopCorner( lines );
			var offset = new Vector2( GetFontSize() * Viewport.CanvasRenderer.AspectRatioInv * 0.8, GetFontSize() * 0.6 );
			AddTextLinesWithShadow( null, GetFontSize(), lines, new Rectangle( offset.X, offset.Y, 1, 1 ), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );
		}

		void DrawGetTextInfoRightBottomCorner()
		{
			var lines = new List<string>();
			GetTextInfoRightBottomCorner( lines );
			var offset = new Vector2( GetFontSize() * Viewport.CanvasRenderer.AspectRatioInv * 0.8, GetFontSize() * 0.6 );
			AddTextLinesWithShadow( null, GetFontSize(), lines, new Rectangle( 0, 0, 1.0 - offset.X, 1.0 - offset.Y ), EHorizontalAlignment.Right, EVerticalAlignment.Bottom, new ColorValue( 1, 1, 1 ) );

			////workarea mode text on screen
			//if( workareaMode != null && !string.IsNullOrEmpty( workareaMode.TextOnScreen ) )
			//{
			//	AddTextWithShadow( null, workareaMode.TextOnScreen, new Vec2( 0.99, 0.99 ), EHorizontalAlign.Right, EVerticalAlign.Bottom, new ColorValue( 1, 1, 1 ) );
			//}
		}

		void DrawScreenMessages()
		{
			//var font = EditorFont;
			var renderer = ViewportControl.Viewport.CanvasRenderer;
			var font = renderer.DefaultFont;
			if( font == null )
				return;

			var fontSize = GetFontSize();//renderer.DefaultFontSize
			var offset = new Vector2( GetFontSize() * Viewport.CanvasRenderer.AspectRatioInv * 0.8, GetFontSize() * 0.6 );

			double posY = 1.0 - offset.Y;// .98f;
			for( int n = screenMessages.Count - 1; n >= 0; n-- )
			{
				ScreenMessage message = screenMessages[ n ];

				Rectangle rectangle = new Rectangle( offset.X, -1, 1.0 - offset.X, posY );
				//Rectangle rectangle = new Rectangle( .02f, -1, .98f, posY );
				ColorValue color = message.color * new ColorValue( 1, 1, 1, message.timeRemaining );
				//bool yellow = n == screenMessages.Count - 1;
				//ColorValue color = message.color * new ColorValue( 1, 1, yellow ? 0 : 1, message.timeRemaining );
				if( color.Alpha > 1 )
					color.Alpha = 1;
				int lineCount = AddTextWordWrapWithShadow( font, fontSize, message.text, rectangle, EHorizontalAlignment.Left, EVerticalAlignment.Bottom, color );

				posY -= (float)fontSize * (float)lineCount;
				if( posY < 0 )
					break;
			}
		}

		protected virtual void Viewport_UpdateGetObjectInSceneRenderingContext( Viewport viewport, ref Component_ObjectInSpace.RenderingContext context )
		{
			workareaMode?.PerformUpdateGetObjectInSceneRenderingContext( viewport, ref context );
			objectCreationMode?.PerformUpdateGetObjectInSceneRenderingContext( viewport, ref context );
		}

		protected virtual void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			//UpdateFontSize();

			//if( ( Form.ModifierKeys & Keys.Control ) != 0 )
			//{
			//	if( viewport.IsKeyPressed( EKeys.D1 ) )
			//		Bgfx.SetDebugFeatures( DebugFeatures.DisplayText | DebugFeatures.DisplayStatistics | DebugFeatures.Profiler );
			//	else if( viewport.IsKeyPressed( EKeys.D2 ) )
			//		Bgfx.SetDebugFeatures( DebugFeatures.DisplayText );
			//	else if( viewport.IsKeyPressed( EKeys.D3 ) )
			//		Bgfx.SetDebugFeatures( DebugFeatures.Wireframe );
			//	else if( viewport.IsKeyPressed( EKeys.D4 ) )
			//		Bgfx.SetDebugFeatures( DebugFeatures.None );
			//}

			//!!!!!
			//if( scene != null )
			//	Log.Info( scene.EditorCameraPosition.Value.ToString() + "    " + scene.EditorCameraDirection.Value.ToString() );

			//!!!!в этом методе?


			//!!!!!!
			//if( component != null )
			//{
			//	List<string> list = new List<string>();

			//	var members = component.MetadataGetMembers( true );

			//	list.Add( component.ToString() );

			//	list.Add( "" );
			//	list.Add( "Events:" );
			//	foreach( var m in members )
			//	{
			//		var evn = m as Metadata.Event;
			//		if( evn != null )
			//			list.Add( evn.ToString() + " - " + evn.Signature );
			//	}

			//	list.Add( "" );
			//	list.Add( "Properties:" );
			//	foreach( var m in members )
			//	{
			//		var p = m as Metadata.Property;
			//		if( p != null )
			//			list.Add( p.ToString() + " - " + p.Signature );
			//	}

			//	list.Add( "" );
			//	list.Add( "Methods:" );
			//	foreach( var m in members )
			//	{
			//		var method = m as Metadata.Method;
			//		if( method != null )
			//			list.Add( method.ToString() + " - " + method.Signature );
			//	}

			//	//!!!!!!log out
			//	{
			//		Log.Info( "" );
			//		Log.Info( "" );
			//		Log.Info( "---------------------------------- START -------------------------------------" );

			//		foreach( var t in list )
			//			Log.Info( t );

			//		Log.Info( "----------------------------------- END --------------------------------------" );
			//	}

			//	viewport.GuiRenderer.AddTextLines( list, new Vec2( .03, .1 ), EHorizontalAlign.Left, EVerticalAlign.Top, 0,
			//		new ColorValue( 1, 1, 0 ) );
			//}

			workareaMode?.PerformUpdateBeforeOutput( viewport );
			objectCreationMode?.PerformUpdateBeforeOutput( viewport );
		}

		protected virtual void GetTextInfoLeftTopCorner( List<string> lines )
		{
		}

		protected virtual void GetTextInfoRightBottomCorner( List<string> lines )
		{
			workareaMode?.PerformGetTextInfoRightBottomCorner( lines );
			objectCreationMode?.PerformGetTextInfoRightBottomCorner( lines );
		}

		//!!!!объединить, два метода никчему? методы виртуальные, может таки надо. потом видно будет
		protected virtual void Viewport_UpdateBeforeOutput2( Viewport viewport )
		{
			workareaMode?.PerformUpdateBeforeOutput2( viewport );
			objectCreationMode?.PerformUpdateBeforeOutput2( viewport );

			DrawGetTextInfoLeftTopCorner();
			DrawGetTextInfoRightBottomCorner();

			DrawScreenMessages();

			//Component component = null;
			//Resource.Instance instance = null;
			//{
			//	component = ObjectOfWindow as Component;
			//	if( component != null && component.ParentRoot != null && component.ParentRoot.HierarchyController != null )
			//		instance = component.ParentRoot.HierarchyController.CreatedByResource;
			//}
			//if( instance != null )
			//{
			//	AddTextWithShadow( viewport.GuiRenderer, Path.GetFileName( instance.Owner.Name ), new Vec2( .03, .03 ), EHorizontalAlign.Left, EVerticalAlign.Top, new ColorValue( 1, 1, 1 ) );
			//	//viewport.GuiRenderer.AddText( Path.GetFileName( instance.Owner.Name ), new Vec2( .03, .03 ) );
			//}
		}

		//!!!!
		//protected override void OnRenderFrame()
		//{
		//	base.OnRenderFrame();

		//	Camera camera = RendererWorld.Instance.DefaultCamera;

		//	if( Map.Instance != null && Map.Instance.IsPostCreated )
		//	{
		//		UpdateCameraSettings();
		//		UpdateMapObjectsVisibility();
		//		RendererWorld.Instance.DefaultViewport.MaterialScheme = materialScheme;
		//	}

		//	//Update sound listener
		//	if( SoundWorld.Instance != null )
		//		SoundWorld.Instance.SetListener( camera.Position, Vec3.Zero, camera.Direction, camera.Up );

		//	//subscribe to Map.Render event
		//	if( Map.Instance != null && renderEventSubscribedTo != Map.Instance )
		//	{
		//		renderEventSubscribedTo = Map.Instance;
		//		Map.Instance.Render += Map_Render;
		//	}

		//	if( MainForm.Instance != null )
		//	{
		//		bool moving = camera.Position != cameraLastPosition || camera.Rotation != cameraLastRotation;
		//		if( moving )
		//			cameraLastMovedTime = EngineApp.Instance.Time;
		//		cameraLastPosition = camera.Position;
		//		cameraLastRotation = camera.Rotation;
		//	}

		//	if( EntitiesEditManager.Instance != null )
		//		EntitiesEditManager.Instance.OnRenderFrame();

		//	if( controlManager != null )
		//		controlManager.DoRender();
		//}

		//!!!!
		//void Map_Render( Map entity, Camera camera )
		//{
		//	if( EntitiesEditManager.Instance != null )
		//		EntitiesEditManager.Instance.OnRender( camera );
		//}

		//!!!!!
		//public void UpdateCameraSettings()
		//{
		//	if( Map.Instance != null && Map.Instance.IsPostCreated )
		//	{
		//		float nearClipDistance = Map.Instance.NearFarClipDistance.Minimum;
		//		float farClipDistance = Map.Instance.NearFarClipDistance.Maximum;
		//		Vec3 position = Map.Instance.EditorCameraPosition;
		//		Vec3 forward = Map.Instance.EditorCameraDirection.GetVector();
		//		Vec3 up = Vec3.ZAxis;
		//		Degree fov = Map.Instance.Fov;
		//		ProjectionTypes projectionType = Map.Instance.CameraProjectionType;
		//		float orthoWindowHeight = Map.Instance.CameraOrthoWindowHeight;

		//		//override camera settings for MapCamera, MapCameraCurvePoint, MapCameraCurve when "C" key is pressed.
		//		{
		//			MapObject mapObject = GetMapCameraPreviewObject();
		//			if( mapObject != null )
		//			{
		//				//MapCamera
		//				MapCamera mapCamera = mapObject as MapCamera;
		//				if( mapCamera != null )
		//				{
		//					position = mapObject.Position;
		//					forward = mapObject.Rotation * new Vec3( 1, 0, 0 );
		//					up = mapObject.Rotation * new Vec3( 0, 0, 1 );
		//					if( mapCamera.Fov != 0 )
		//						fov = mapCamera.Fov;
		//					projectionType = mapCamera.ProjectionType;
		//					orthoWindowHeight = mapCamera.OrthoWindowHeight;
		//				}

		//				//MapCameraCurvePoint
		//				MapCameraCurvePoint mapCameraCurvePoint = mapObject as MapCameraCurvePoint;
		//				if( mapCameraCurvePoint != null )
		//				{
		//					MapCameraCurve mapCameraCurve = mapCameraCurvePoint.Owner as MapCameraCurve;
		//					if( mapCameraCurve != null )
		//					{
		//						//animate whn owner curve of selected point is available

		//						Range range = mapCameraCurve.GetCurveTimeRange();

		//						if( mapCameraCurveTime < range.Minimum )
		//							mapCameraCurveTime = range.Minimum;
		//						if( mapCameraCurveTime > range.Maximum )
		//							mapCameraCurveTime = range.Maximum;
		//						Degree f;
		//						mapCameraCurve.CalculateCameraPositionByTime( mapCameraCurvePoint.Time + mapCameraCurveTime,
		//							out position, out forward, out up, out f );
		//						if( f != 0 )
		//							fov = f;
		//					}
		//					else
		//					{
		//						position = mapObject.Position;
		//						forward = mapObject.Rotation * new Vec3( 1, 0, 0 );
		//						up = mapObject.Rotation * new Vec3( 0, 0, 1 );
		//						if( mapCameraCurvePoint.Fov != 0 )
		//							fov = mapCameraCurvePoint.Fov;
		//					}
		//				}

		//				//MapCameraCurve
		//				{
		//					MapCameraCurve mapCameraCurve = mapObject as MapCameraCurve;
		//					if( mapCameraCurve != null )
		//					{
		//						Range range = mapCameraCurve.GetCurveTimeRange();

		//						if( mapCameraCurveTime < range.Minimum )
		//							mapCameraCurveTime = range.Minimum;
		//						if( mapCameraCurveTime > range.Maximum )
		//							mapCameraCurveTime = range.Maximum;
		//						Degree f;
		//						mapCameraCurve.CalculateCameraPositionByTime( mapCameraCurveTime, out position, out forward, out up,
		//							out f );
		//						if( f != 0 )
		//							fov = f;
		//					}
		//				}
		//			}
		//		}

		//		//update camera
		//		Camera camera = RendererWorld.Instance.DefaultCamera;
		//		camera.ProjectionType = projectionType;
		//		camera.OrthoWindowHeight = orthoWindowHeight;
		//		camera.NearClipDistance = nearClipDistance;
		//		camera.FarClipDistance = farClipDistance;
		//		camera.FixedUp = up;
		//		camera.Fov = fov;
		//		camera.Position = position;
		//		camera.Direction = forward;

		//		if( AddonManager.Instance != null )
		//		{
		//			foreach( MapEditorAddon addon in AddonManager.Instance.Addons )
		//				addon.OnUpdateCameraSettings( camera );
		//		}
		//	}
		//}

		protected virtual void Viewport_UpdateEnd( Viewport viewport )
		{
			workareaMode?.PerformUpdateEnd( viewport );
			objectCreationMode?.PerformUpdateEnd( viewport );
		}

		protected virtual void ViewportControl_ViewportDestroyed( EngineViewportControl sender )
		{
		}

		protected override void OnDestroy()
		{
			DestroyScene();

			base.OnDestroy();
		}

		//!!!!!
		//protected override void OnRenderScreenUI( GuiRenderer renderer )
		//{
		//	base.OnRenderScreenUI( renderer );

		//	if( Map.Instance != null )
		//		Map.Instance.DoRenderUI( renderer );

		//	if( Map.Instance != null )
		//	{
		//		if( EntitiesEditManager.Instance != null )
		//			EntitiesEditManager.Instance.OnRenderUI( renderer );
		//	}
		//	else
		//	{
		//		renderer.AddQuad( new Rect( 0, 0, 1, 1 ), new ColorValue( .2f, .2f, .2f ) );
		//		string text = ToolsLocalization.Translate( "Various", "No map loaded" );
		//		AddTextWithShadow( renderer, text, new Vec2( .5f, .5f ), HorizontalAlign.Center,
		//			VerticalAlign.Center, new ColorValue( 1, 1, 1 ) );
		//	}

		//	if( AddonManager.Instance != null )
		//	{
		//		foreach( MapEditorAddon addon in AddonManager.Instance.Addons )
		//			addon.OnRenderScreenUI( renderer );
		//	}

		//	if( controlManager != null )
		//		controlManager.DoRenderUI( renderer );
		//}

		//!!!!!
		//void DebugRenderUndoSystem( GuiRenderer renderer )
		//{
		//	if( UndoSystem.Instance == null )
		//		return;

		//	string[] lines = UndoSystem.Instance.DumpDebugToLines();
		//	renderer.AddTextLines( lines, new Vec2( .5f, .1f ), HorizontalAlign.Center, VerticalAlign.Top, 0,
		//		new ColorValue( 1, 1, 0 ) );
		//}

		//!!!!!!

		public void AddScreenMessage( string text, ColorValue color )
		{
			ScreenMessage message = new ScreenMessage();
			message.text = text;
			message.color = color;
			message.timeRemaining = 5;
			screenMessages.Add( message );

			//!!!!100
			while( screenMessages.Count > 100 )
				screenMessages.RemoveAt( 0 );
		}

		public void AddScreenMessage( string text )
		{
			AddScreenMessage( text, new ColorValue( 1, 1, 1 ) );
		}

		//!!!!
		//public static float SoundVolume
		//{
		//	get { return soundVolume; }
		//	set
		//	{
		//		MathFunctions.Clamp( ref value, 0, 1 );

		//		soundVolume = value;

		//		if( EngineApp.Instance.DefaultSoundChannelGroup != null )
		//			EngineApp.Instance.DefaultSoundChannelGroup.Volume = soundVolume;
		//	}
		//}

		[Browsable( false )]
		public Component_Scene Scene
		{
			get { return scene; }
			set { scene = value; }
		}

		[Browsable( false )]
		public bool SceneNeedDispose
		{
			get { return sceneNeedDispose; }
			set { sceneNeedDispose = value; }
		}

		public Component_Scene CreateScene( bool enable )
		{
			DestroyScene();

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

				double c = 1;
				double c2 = 1;

				if( EditorAPI.DarkTheme )
					scene.BackgroundColor = new ColorValue( 40.0 / 255 * c, 40.0 / 255 * c, 40.0 / 255 * c );
				else
					scene.BackgroundColor = new ColorValue( 22.0 / 255 * c, 44.0 / 255 * c, 66.0 / 255 * c );
				scene.BackgroundColorEnvironmentOverride = new ColorValue( 0.8, 0.8, 0.8 );

				var backgroundEffects = pipeline.CreateComponent<Component>();
				backgroundEffects.Name = "Background Effects";

				var vignetting = backgroundEffects.CreateComponent<Component_RenderingEffect_Vignetting>();
				if( EditorAPI.DarkTheme )
					vignetting.Color = new ColorValue( 45.0 / 255 * c2, 45.0 / 255 * c2, 45.0 / 255 * c2 );
				else
					vignetting.Color = new ColorValue( 24.0 / 255 * c2, 48.0 / 255 * c2, 72.0 / 255 * c2 );
				vignetting.Radius = 2;

				var noise = backgroundEffects.CreateComponent<Component_RenderingEffect_Noise>();
				noise.Multiply = new Range( 0.9, 1.1 );

				var sceneEffects = pipeline.CreateComponent<Component>();
				sceneEffects.Name = "Scene Effects";

				//antialiasing
				var toLDRType = MetadataManager.GetType( "NeoAxis.Component_RenderingEffect_ToLDR" );
				var antialiasingType = MetadataManager.GetType( "NeoAxis.Component_RenderingEffect_Antialiasing" );
				if( toLDRType != null && antialiasingType != null )
				{
					sceneEffects.CreateComponent( toLDRType );
					sceneEffects.CreateComponent( antialiasingType );
				}

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
			if( ViewportControl != null && ViewportControl.Viewport != null )
				ViewportControl.Viewport.AttachedScene = scene;

			return scene;
		}

		public void DestroyScene()
		{
			if( scene != null )
			{
				if( ViewportControl != null && ViewportControl.Viewport != null )
					ViewportControl.Viewport.AttachedScene = null;

				if( sceneNeedDispose )
					scene.Dispose();
				scene = null;
				sceneNeedDispose = false;
			}
		}

		protected virtual void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
		{
			Component_Camera camera;

			if( scene.Mode.Value == Component_Scene.ModeEnum._3D )
			{
				//update scene.CameraEditor. create if not exists.
				camera = scene.CameraEditor;
				if( camera == null )
				{
					string name = "Camera Editor";

					camera = scene.GetComponent( name ) as Component_Camera;
					if( camera == null )
					{
						camera = scene.CreateComponent<Component_Camera>();
						camera.Name = name;
						camera.Visible = false;
						camera.CanBeSelected = false;
					}
					scene.CameraEditor = new Reference<Component_Camera>( null, ReferenceUtility.CalculateThisReference( scene, camera ) );
				}
			}
			else
			{
				//update scene.CameraEditor2D. create if not exists.
				camera = scene.CameraEditor2D;
				if( camera == null )
				{
					string name = "Camera Editor 2D";

					camera = scene.GetComponent( name ) as Component_Camera;
					if( camera == null )
					{
						camera = scene.CreateComponent<Component_Camera>();
						camera.Name = name;
						camera.Visible = false;
						camera.Transform = new Transform( new Vector3( 2.64918580053222, 3.02745193504869, 10 ), new Quaternion( -0.502493739596367, 0.497493760429674, 0.497493760429674, 0.502493739596367 ), Vector3.One );
						camera.Projection = ProjectionType.Orthographic;
						camera.Height = 16.8586419336498;
						camera.FixedUp = Vector3.YAxis;
						camera.FarClipPlane = 100;
						camera.CanBeSelected = false;
					}
					scene.CameraEditor2D = new Reference<Component_Camera>( null, ReferenceUtility.CalculateThisReference( scene, camera ) );
				}
			}

			workareaMode?.PerformViewportUpdateGetCameraSettings( ref camera );
			objectCreationMode?.PerformViewportUpdateGetCameraSettings( ref camera );

			//!!!!!!

			//Vec3 from = scene.EditorCameraPosition;//center + cameraDirection.GetVector() * cameraDistance;
			//Vec3 dir = scene.EditorCameraDirection.Value.GetVector();
			////Vec3 to = ( scene.EditorCameraDirection.Value.GetVector() - scene.EditorCameraPosition ).GetNormalize();
			//Degree fov = 65;// 75;

			////!!!!near, far, ZAxis, etc

			////!!!!
			//Component_Camera camera = new Component_Camera();
			//camera.AspectRatio = (double)viewport.SizeInPixels.X / (double)viewport.SizeInPixels.Y;
			//camera.FieldOfView = fov;
			//camera.NearClipDistance = .1;
			//camera.FarClipDistance = 1000;
			//camera.Position = from;
			//camera.Direction = dir.GetNormalize();// ( to - from ).GetNormalize();
			//camera.FixedUp = Vec3.ZAxis;

			viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, camera, scene.FrustumCullingTest );

			//var settings = new Viewport.CameraSettingsClass( viewport, aspect, fov, .1f, 1000, from, dir.GetNormalize(), Vec3.ZAxis );
			////var settings = new Viewport.CameraSettingsClass( viewport, aspect, fov, .1f, 1000, from, ( to - from ).GetNormalize(), Vec3.ZAxis );

			//viewport.CameraSettings = settings;
			processed = true;
		}

		[Browsable( false )]
		public bool CameraRotating
		{
			get { return cameraRotating; }
		}

		public double GetFontSize()
		{
			double fontSizeInPixels = 14.0 * DpiHelper.Default.DpiScaleFactor;
			fontSizeInPixels = (int)fontSizeInPixels;

			var renderer = ViewportControl.Viewport.CanvasRenderer;

			int height = renderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y;
			float screenCellSize = (float)fontSizeInPixels / (float)height;
			float demandFontHeight = screenCellSize;// * GetZoom();

			return demandFontHeight;
		}

		//void CalculateFontSize()
		//{
		//	fontSizeInPixels = 14;

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

		public void AddTextWithShadow( Component_Font font, double fontSize, string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			var renderer = ViewportControl.Viewport.CanvasRenderer;

			if( font == null || font.Disposed )
				font = renderer.DefaultFont;
			if( font == null || font.Disposed )
				return;
			//if( font == null )
			//	font = EditorFont;
			if( fontSize < 0 )
				fontSize = renderer.DefaultFontSize;

			Vector2 shadowOffset = shadowDistanceInPixels / ViewportControl.Viewport.SizeInPixels.ToVector2();
			renderer.AddText( font, fontSize, text, position + shadowOffset, horizontalAlign, verticalAlign, new ColorValue( 0, 0, 0, color.Alpha / 2 ) );
			renderer.AddText( font, fontSize, text, position, horizontalAlign, verticalAlign, color );
		}

		public void AddTextWithShadow( string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			AddTextWithShadow( null, -1, text, position, horizontalAlign, verticalAlign, color );
		}

		public void AddTextLinesWithShadow( Component_Font font, double fontSize, IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			if( lines.Count == 0 )
				return;

			var renderer = ViewportControl.Viewport.CanvasRenderer;

			if( font == null || font.Disposed )
				font = renderer.DefaultFont;
			if( font == null || font.Disposed )
				return;
			//if( font == null )
			//	font = EditorFont;
			if( fontSize < 0 )
				fontSize = renderer.DefaultFontSize;

			Vector2 shadowOffset = shadowDistanceInPixels / ViewportControl.Viewport.SizeInPixels.ToVector2();
			float linesHeight = (float)lines.Count * (float)fontSize;

			double posY = 0;
			switch( verticalAlign )
			{
			case EVerticalAlignment.Top:
				posY = rectangle.Top;
				break;
			case EVerticalAlignment.Center:
				posY = rectangle.Top + ( rectangle.Size.Y - linesHeight ) / 2;
				break;
			case EVerticalAlignment.Bottom:
				posY = rectangle.Bottom - linesHeight;
				break;
			}

			for( int n = 0; n < lines.Count; n++ )
			{
				string line = lines[ n ];

				double posX = 0;
				switch( horizontalAlign )
				{
				case EHorizontalAlignment.Left:
					posX = rectangle.Left;
					break;
				case EHorizontalAlignment.Center:
					posX = rectangle.Left + ( rectangle.Size.X - font.GetTextLength( fontSize, renderer, line ) ) / 2;
					break;
				case EHorizontalAlignment.Right:
					posX = rectangle.Right - font.GetTextLength( fontSize, renderer, line );
					break;
				}

				Vector2 position = new Vector2( posX, posY );

				renderer.AddText( font, fontSize, line, position + shadowOffset, EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 0, 0, 0, color.Alpha / 2 ) );
				renderer.AddText( font, fontSize, line, position, EHorizontalAlignment.Left, EVerticalAlignment.Top, color );
				posY += fontSize;
			}
		}

		public void AddTextLinesWithShadow( IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			AddTextLinesWithShadow( null, -1, lines, rectangle, horizontalAlign, verticalAlign, color );
		}

		public int AddTextWordWrapWithShadow( Component_Font font, double fontSize, string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			var renderer = ViewportControl.Viewport.CanvasRenderer;

			if( font == null || font.Disposed )
				font = renderer.DefaultFont;
			if( font == null || font.Disposed )
				return 0;
			//if( font == null )
			//	font = EditorFont;
			if( fontSize < 0 )
				fontSize = renderer.DefaultFontSize;

			var items = font.GetWordWrapLines( fontSize, renderer, text, rectangle.Size.X );

			string[] lines = new string[ items.Length ];
			for( int n = 0; n < lines.Length; n++ )
				lines[ n ] = items[ n ].Text;

			AddTextLinesWithShadow( font, fontSize, lines, rectangle, horizontalAlign, verticalAlign, color );

			return lines.Length;
		}

		public int AddTextWordWrapWithShadow( string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			return AddTextWordWrapWithShadow( null, -1, text, rectangle, horizontalAlign, verticalAlign, color );
		}

		[Browsable( false )]
		public string WorkareaModeName
		{
			get { return workareaModeName; }
		}

		[Browsable( false )]
		public WorkareaModeClass WorkareaMode
		{
			get { return workareaMode; }
		}

		public virtual void WorkareaModeSet( string name, WorkareaModeClass instance = null )
		{
			workareaMode?.PerformDestroy();

			workareaModeName = name;
			workareaMode = instance;
		}

		[Browsable( false )]
		public bool AllowCameraControl
		{
			get
			{
				if( workareaMode != null && !workareaMode.AllowControlCamera )
					return false;
				return true;
			}
		}

		[Browsable( false )]
		public bool AllowSelectObjects
		{
			get
			{
				if( workareaMode != null && !workareaMode.AllowSelectObjects )
					return false;
				return true;
			}
		}

		[Browsable( false )]
		public bool DisplaySelectedObjects
		{
			get
			{
				if( workareaMode != null && !workareaMode.DisplaySelectedObjects )
					return false;
				return true;
			}
		}

		public override void EditorActionGetState( EditorAction.GetStateContext context )
		{
			base.EditorActionGetState( context );

			objectCreationMode?.PerformEditorActionGetState( context );
			workareaMode?.PerformEditorActionGetState( context );
		}

		public override void EditorActionClick( EditorAction.ClickContext context )
		{
			base.EditorActionClick( context );

			objectCreationMode?.PerformEditorActionClick( context );
			workareaMode?.PerformEditorActionClick( context );
		}

		[Browsable( false )]
		public ObjectCreationMode ObjectCreationMode
		{
			get { return objectCreationMode; }
		}

		public virtual void ObjectCreationModeSet( ObjectCreationMode mode )
		{
			objectCreationMode?.PerformDestroy();

			objectCreationMode = mode;
		}
	}
}
