#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Editor
{
	public class PreviewControlWithViewport_CanvasBasedPreview : PreviewControlWithViewport
	{
		CanvasBasedPreview preview;

		//

		public PreviewControlWithViewport_CanvasBasedPreview( CanvasBasedPreview preview )
		{
			this.preview = preview;
		}

		public CanvasBasedPreview Preview
		{
			get { return preview; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
			preview.PerformOnCreate();
		}

		protected override void OnDestroy()
		{
			preview.PerformOnDestroy();
		}
		public void PerformBaseOnDestroy()
		{
			base.OnDestroy();
		}

		protected override void Viewport_KeyDown( Viewport viewport, KeyEvent e, ref bool handled )
		{
			preview.PerformKeyDown( e, ref handled );
		}
		public void PerformBaseViewportKeyDown( KeyEvent e, ref bool handled )
		{
			base.Viewport_KeyDown( Viewport, e, ref handled );
		}

		protected override void Viewport_KeyPress( Viewport viewport, KeyPressEvent e, ref bool handled )
		{
			preview.PerformKeyPress( e, ref handled );
		}
		public void PerformBaseViewportKeyPress( KeyPressEvent e, ref bool handled )
		{
			base.Viewport_KeyPress( Viewport, e, ref handled );
		}

		protected override void Viewport_KeyUp( Viewport viewport, KeyEvent e, ref bool handled )
		{
			preview.PerformKeyUp( e, ref handled );
		}
		public void PerformBaseViewportKeyUp( KeyEvent e, ref bool handled )
		{
			base.Viewport_KeyUp( Viewport, e, ref handled );
		}

		protected override void Viewport_MouseDown( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			preview.PerformMouseDown( button, ref handled );
		}
		public void PerformBaseViewportMouseDown( EMouseButtons button, ref bool handled )
		{
			base.Viewport_MouseDown( Viewport, button, ref handled );
		}

		protected override void Viewport_MouseUp( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			preview.PerformMouseUp( button, ref handled );
		}
		public void PerformBaseViewportMouseUp( EMouseButtons button, ref bool handled )
		{
			base.Viewport_MouseUp( Viewport, button, ref handled );
		}

		protected override void Viewport_MouseDoubleClick( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			preview.PerformMouseDoubleClick( button, ref handled );
		}
		public void PerformBaseViewportMouseDoubleClick( EMouseButtons button, ref bool handled )
		{
			base.Viewport_MouseDoubleClick( Viewport, button, ref handled );
		}

		protected override void Viewport_MouseMove( Viewport viewport, Vector2 mouse )
		{
			preview.PerformMouseMove( mouse );
		}
		public void PerformBaseViewportMouseMove( Vector2 mouse )
		{
			base.Viewport_MouseMove( Viewport, mouse );
		}

		protected override void Viewport_MouseRelativeModeChanged( Viewport viewport, ref bool handled )
		{
			preview.PerformMouseRelativeModeChanged( ref handled );
		}
		public void PerformBaseViewportMouseRelativeModeChanged( ref bool handled )
		{
			base.Viewport_MouseRelativeModeChanged( Viewport, ref handled );
		}

		protected override void Viewport_MouseWheel( Viewport viewport, int delta, ref bool handled )
		{
			preview.PerformMouseWheel( delta, ref handled );
		}
		public void PerformBaseViewportMouseWheel( int delta, ref bool handled )
		{
			base.Viewport_MouseWheel( Viewport, delta, ref handled );
		}

		protected override void Viewport_JoystickEvent( Viewport viewport, JoystickInputEvent e, ref bool handled )
		{
			preview.PerformJoystickEvent( e, ref handled );
		}
		public void PerformBaseViewportJoystickEvent( JoystickInputEvent e, ref bool handled )
		{
			base.Viewport_JoystickEvent( Viewport, e, ref handled );
		}

		protected override void Viewport_SpecialInputDeviceEvent( Viewport viewport, InputEvent e, ref bool handled )
		{
			preview.PerformSpecialInputDeviceEvent( e, ref handled );
		}
		public void PerformBaseViewportSpecialInputDeviceEvent( InputEvent e, ref bool handled )
		{
			base.Viewport_SpecialInputDeviceEvent( Viewport, e, ref handled );
		}

		protected override void Viewport_Tick( Viewport viewport, float delta )
		{
			preview.PerformTick( delta );
		}
		public void PerformBaseViewportTick( float delta )
		{
			base.Viewport_Tick( Viewport, delta );
		}

		protected override void Viewport_UpdateBegin( Viewport viewport )
		{
			preview.PerformViewportUpdateBegin();
		}
		public void PerformBaseViewportUpdateBegin()
		{
			base.Viewport_UpdateBegin( Viewport );
		}

		protected override void Viewport_UpdateEnd( Viewport viewport )
		{
			preview.PerformViewportUpdateEnd();
		}
		public void PerformBaseViewportUpdateEnd()
		{
			base.Viewport_UpdateEnd( Viewport );
		}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			preview.PerformViewportCreated();
		}
		public void PerformBaseViewportCreated()
		{
			base.ViewportControl_ViewportCreated( ViewportControl );
		}

		protected override void ViewportControl_ViewportDestroyed( EngineViewportControl sender )
		{
			preview.PerformViewportDestroyed();
		}
		public void PerformBaseViewportDestroyed()
		{
			base.ViewportControl_ViewportDestroyed( ViewportControl );
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			preview.PerformViewportUpdateBeforeOutput();
		}
		public void PerformBaseViewportUpdateBeforeOutput()
		{
			base.Viewport_UpdateBeforeOutput( Viewport );
		}

		protected override void Viewport_UpdateBeforeOutput2( Viewport viewport )
		{
			preview.PerformViewportUpdateBeforeOutput2();
		}
		public void PerformBaseViewportUpdateBeforeOutput2()
		{
			base.Viewport_UpdateBeforeOutput2( Viewport );
		}

		//protected override void Viewport_UpdateGetObjectInSceneRenderingContext( Viewport viewport, ref ObjectInSpace.RenderingContext context )
		//{
		//	preview.PerformViewportUpdateGetObjectInSceneRenderingContext( ref context );
		//}
		//public void PerformBaseViewportUpdateGetObjectInSceneRenderingContext( ref ObjectInSpace.RenderingContext context )
		//{
		//	base.Viewport_UpdateGetObjectInSceneRenderingContext( Viewport, ref context );
		//}

		protected override void Scene_ViewportUpdateGetCameraSettings( Scene scene, Viewport viewport, ref bool processed )
		{
			preview.PerformSceneViewportUpdateGetCameraSettings( ref processed );
		}
		public void PerformBaseSceneViewportUpdateGetCameraSettings( ref bool processed )
		{
			base.Scene_ViewportUpdateGetCameraSettings( Scene, Viewport, ref processed );
		}

		protected override void GetTextInfoLeftTopCorner( List<string> lines )
		{
			preview.PerformOnGetTextInfoLeftTopCorner( lines );
		}
		public void PerformBaseGetTextInfoLeftTopCorner( List<string> lines )
		{
			base.GetTextInfoLeftTopCorner( lines );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class CanvasBasedPreview
	{
		internal PreviewControlWithViewport_CanvasBasedPreview owner;

		//

		public CanvasBasedPreview()
		{
		}

		public PreviewControlWithViewport Owner
		{
			get { return owner; }
		}

		/////////////////////////////////////////

		public object ObjectOfPreview
		{
			get { return owner.ObjectOfPreview; }
		}

		//public PreviewWindow.PanelData Panel
		//{
		//	get { return panel; }
		//	set { panel = value; }
		//}

		protected virtual void OnCreate()
		{
		}
		internal void PerformOnCreate()
		{
			OnCreate();
		}

		protected virtual void OnDestroy()
		{
			owner.PerformBaseOnDestroy();
		}
		internal void PerformOnDestroy()
		{
			OnDestroy();
		}

		///////////////////////////////////////////

		public EngineViewportControl ViewportControl
		{
			get { return owner.ViewportControl; }
		}

		public Viewport Viewport
		{
			get { return owner.Viewport; }
		}

		protected virtual void OnKeyDown( KeyEvent e, ref bool handled )
		{
			owner.PerformBaseViewportKeyDown( e, ref handled );
		}
		internal void PerformKeyDown( KeyEvent e, ref bool handled )
		{
			OnKeyDown( e, ref handled );
		}

		protected virtual void OnKeyPress( KeyPressEvent e, ref bool handled )
		{
			owner.PerformBaseViewportKeyPress( e, ref handled );
		}
		internal void PerformKeyPress( KeyPressEvent e, ref bool handled )
		{
			OnKeyPress( e, ref handled );
		}

		protected virtual void OnKeyUp( KeyEvent e, ref bool handled )
		{
			owner.PerformBaseViewportKeyUp( e, ref handled );
		}
		internal void PerformKeyUp( KeyEvent e, ref bool handled )
		{
			OnKeyUp( e, ref handled );
		}

		protected virtual void OnMouseDown( EMouseButtons button, ref bool handled )
		{
			owner.PerformBaseViewportMouseDown( button, ref handled );
		}
		internal void PerformMouseDown( EMouseButtons button, ref bool handled )
		{
			OnMouseDown( button, ref handled );
		}

		protected virtual void OnMouseUp( EMouseButtons button, ref bool handled )
		{
			owner.PerformBaseViewportMouseUp( button, ref handled );
		}
		internal void PerformMouseUp( EMouseButtons button, ref bool handled )
		{
			OnMouseUp( button, ref handled );
		}

		protected virtual void OnMouseDoubleClick( EMouseButtons button, ref bool handled )
		{
			owner.PerformBaseViewportMouseDoubleClick( button, ref handled );
		}
		internal void PerformMouseDoubleClick( EMouseButtons button, ref bool handled )
		{
			OnMouseDoubleClick( button, ref handled );
		}

		protected virtual void OnMouseMove( Vector2 mouse )
		{
			owner.PerformBaseViewportMouseMove( mouse );
		}
		internal void PerformMouseMove( Vector2 mouse )
		{
			OnMouseMove( mouse );
		}

		protected virtual void OnMouseRelativeModeChanged( ref bool handled )
		{
			owner.PerformBaseViewportMouseRelativeModeChanged( ref handled );
		}
		internal void PerformMouseRelativeModeChanged( ref bool handled )
		{
			OnMouseRelativeModeChanged( ref handled );
		}

		protected virtual void OnMouseWheel( int delta, ref bool handled )
		{
			owner.PerformBaseViewportMouseWheel( delta, ref handled );
		}
		internal void PerformMouseWheel( int delta, ref bool handled )
		{
			OnMouseWheel( delta, ref handled );
		}

		protected virtual void OnJoystickEvent( JoystickInputEvent e, ref bool handled )
		{
			owner.PerformBaseViewportJoystickEvent( e, ref handled );
		}
		internal void PerformJoystickEvent( JoystickInputEvent e, ref bool handled )
		{
			OnJoystickEvent( e, ref handled );
		}

		protected virtual void OnSpecialInputDeviceEvent( InputEvent e, ref bool handled )
		{
			owner.PerformBaseViewportSpecialInputDeviceEvent( e, ref handled );
		}
		internal void PerformSpecialInputDeviceEvent( InputEvent e, ref bool handled )
		{
			OnSpecialInputDeviceEvent( e, ref handled );
		}

		protected virtual void OnTick( float delta )
		{
			owner.PerformBaseViewportTick( delta );
		}
		internal void PerformTick( float delta )
		{
			OnTick( delta );
		}

		protected virtual void OnViewportUpdateBegin()
		{
			owner.PerformBaseViewportUpdateBegin();
		}
		internal void PerformViewportUpdateBegin()
		{
			OnViewportUpdateBegin();
		}

		protected virtual void OnViewportUpdateEnd()
		{
			owner.PerformBaseViewportUpdateEnd();
		}
		internal void PerformViewportUpdateEnd()
		{
			OnViewportUpdateEnd();
		}

		protected virtual void OnViewportCreated()
		{
			owner.PerformBaseViewportCreated();
		}
		internal void PerformViewportCreated()
		{
			OnViewportCreated();
		}

		protected virtual void OnViewportDestroyed()
		{
			owner.PerformBaseViewportDestroyed();
		}
		internal void PerformViewportDestroyed()
		{
			OnViewportDestroyed();
		}

		protected virtual void OnViewportUpdateBeforeOutput()
		{
			owner.PerformBaseViewportUpdateBeforeOutput();
		}
		internal void PerformViewportUpdateBeforeOutput()
		{
			OnViewportUpdateBeforeOutput();
		}

		protected virtual void OnViewportUpdateBeforeOutput2()
		{
			owner.PerformBaseViewportUpdateBeforeOutput2();
		}
		internal void PerformViewportUpdateBeforeOutput2()
		{
			OnViewportUpdateBeforeOutput2();
		}

		//protected virtual void OnViewportUpdateGetObjectInSceneRenderingContext( ref ObjectInSpace.RenderingContext context )
		//{
		//	owner.PerformBaseViewportUpdateGetObjectInSceneRenderingContext( ref context );
		//}
		//internal void PerformViewportUpdateGetObjectInSceneRenderingContext( ref ObjectInSpace.RenderingContext context )
		//{
		//	OnViewportUpdateGetObjectInSceneRenderingContext( ref context );
		//}

		protected virtual void OnSceneViewportUpdateGetCameraSettings( ref bool processed )
		{
			owner.PerformBaseSceneViewportUpdateGetCameraSettings( ref processed );
		}
		internal void PerformSceneViewportUpdateGetCameraSettings( ref bool processed )
		{
			OnSceneViewportUpdateGetCameraSettings( ref processed );
		}

		protected virtual void OnGetTextInfoLeftTopCorner( List<string> lines )
		{
			owner.PerformBaseGetTextInfoLeftTopCorner( lines );
		}
		internal void PerformOnGetTextInfoLeftTopCorner( List<string> lines )
		{
			OnGetTextInfoLeftTopCorner( lines );
		}

		public Scene Scene
		{
			get { return owner.Scene; }
			set { owner.Scene = value; }
		}

		public bool SceneNeedDispose
		{
			get { return owner.SceneNeedDispose; }
			set { owner.SceneNeedDispose = value; }
		}

		public Vector3 CameraLookTo
		{
			get { return owner.CameraLookTo; }
			set { owner.CameraLookTo = value; }
		}

		public double CameraInitialDistance
		{
			get { return owner.CameraInitialDistance; }
			set { owner.CameraInitialDistance = value; }
		}

		public bool CameraRotationMode
		{
			get { return owner.CameraRotationMode; }
			set { owner.CameraRotationMode = value; }
		}

		public SphericalDirection CameraDirection
		{
			get { return owner.CameraDirection; }
			set { owner.CameraDirection = value; }
		}

		public Scene CreateScene( bool enable )
		{
			return owner.CreateScene( enable );
		}

		public void DestroyScene()
		{
			owner.DestroyScene();
		}

		public void SetCameraByBounds( Bounds bounds, double distanceScale = 1, bool mode2D = false )
		{
			owner.SetCameraByBounds( bounds, distanceScale, mode2D );
		}

		public double GetFontSize()
		{
			return owner.GetFontSize();
		}

		////public float FontSizeInPixels
		////{
		////	get { return fontSizeInPixels; }
		////	set { fontSizeInPixels = value; }
		////}

		////public EngineFont EditorFont
		////{
		////	get { return editorFont; }
		////}

		public void AddTextWithShadow( FontComponent font, double fontSize, string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			owner.AddTextWithShadow( font, fontSize, text, position, horizontalAlign, verticalAlign, color );
		}

		public void AddTextWithShadow( string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			owner.AddTextWithShadow( text, position, horizontalAlign, verticalAlign, color );
		}

		public void AddTextLinesWithShadow( FontComponent font, double fontSize, IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			owner.AddTextLinesWithShadow( font, fontSize, lines, rectangle, horizontalAlign, verticalAlign, color );
		}

		public void AddTextLinesWithShadow( IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			owner.AddTextLinesWithShadow( lines, rectangle, horizontalAlign, verticalAlign, color );
		}

		public int AddTextWordWrapWithShadow( FontComponent font, double fontSize, string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			return owner.AddTextWordWrapWithShadow( font, fontSize, text, rectangle, horizontalAlign, verticalAlign, color );
		}

		public int AddTextWordWrapWithShadow( string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			return owner.AddTextWordWrapWithShadow( text, rectangle, horizontalAlign, verticalAlign, color );
		}

	}
}

#endif