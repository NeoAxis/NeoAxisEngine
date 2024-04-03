#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Editor
{
	internal interface IPreviewControlWithViewport_CanvasBasedPreview : IPreviewControlWithViewport
	{
		void PerformBaseOnDestroy();
		void PerformBaseViewportKeyDown( KeyEvent e, ref bool handled );
		void PerformBaseViewportKeyPress( KeyPressEvent e, ref bool handled );
		void PerformBaseViewportKeyUp( KeyEvent e, ref bool handled );
		void PerformBaseViewportMouseDown( EMouseButtons button, ref bool handled );
		void PerformBaseViewportMouseUp( EMouseButtons button, ref bool handled );
		void PerformBaseViewportMouseDoubleClick( EMouseButtons button, ref bool handled );
		void PerformBaseViewportMouseMove( Vector2 mouse );
		void PerformBaseViewportMouseRelativeModeChanged( ref bool handled );
		void PerformBaseViewportMouseWheel( int delta, ref bool handled );
		void PerformBaseViewportJoystickEvent( JoystickInputEvent e, ref bool handled );
		void PerformBaseViewportSpecialInputDeviceEvent( InputEvent e, ref bool handled );
		void PerformBaseViewportTick( float delta );
		void PerformBaseViewportUpdateBegin();
		void PerformBaseViewportUpdateEnd();
		void PerformBaseViewportCreated();
		void PerformBaseViewportDestroyed();
		void PerformBaseViewportUpdateBeforeOutput();
		void PerformBaseViewportUpdateBeforeOutput2();
		void PerformBaseSceneViewportUpdateGetCameraSettings( ref bool processed );
		void PerformBaseGetTextInfoLeftTopCorner( List<string> lines );
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class CanvasBasedPreview
	{
		internal IPreviewControlWithViewport_CanvasBasedPreview owner;

		//

		public CanvasBasedPreview()
		{
		}

		public IPreviewControlWithViewport Owner
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

		public IEngineViewportControl ViewportControl
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