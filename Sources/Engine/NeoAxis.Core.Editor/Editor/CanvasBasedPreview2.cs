// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class PreviewControlWithViewport_CanvasBasedPreview : PreviewControlWithViewport, IPreviewControlWithViewport_CanvasBasedPreview
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
			base.ViewportControl_ViewportCreated( ViewportControl2 );
		}

		protected override void ViewportControl_ViewportDestroyed( EngineViewportControl sender )
		{
			preview.PerformViewportDestroyed();
		}
		public void PerformBaseViewportDestroyed()
		{
			base.ViewportControl_ViewportDestroyed( ViewportControl2 );
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
}
