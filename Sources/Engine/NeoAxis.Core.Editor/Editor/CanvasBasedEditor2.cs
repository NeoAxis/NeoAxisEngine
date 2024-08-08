// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

//!!!! is not fully implemented. use DocumentWindowWithViewport when need working area, creation mode, drag and drop

namespace NeoAxis.Editor
{
	public class DocumentWindowWithViewport_CanvasBasedEditor : DocumentWindowWithViewport, IDocumentWindowWithViewport_CanvasBasedEditor
	{
		CanvasBasedEditor editor;

		//

		public DocumentWindowWithViewport_CanvasBasedEditor( CanvasBasedEditor editor )
		{
			this.editor = editor;

			SelectedObjectsChanged += DocumentWindowWithViewport_CanvasBasedEditor_SelectedObjectsChanged;
		}

		public CanvasBasedEditor Editor
		{
			get { return editor; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
			editor.PerformOnCreate();
		}

		protected override void OnDestroy()
		{
			editor.PerformOnDestroy();
		}
		public void PerformBaseOnDestroy()
		{
			base.OnDestroy();
		}

		protected override void Viewport_KeyDown( Viewport viewport, KeyEvent e, ref bool handled )
		{
			editor.PerformKeyDown( e, ref handled );
		}
		public void PerformBaseViewportKeyDown( KeyEvent e, ref bool handled )
		{
			base.Viewport_KeyDown( Viewport, e, ref handled );
		}

		protected override void Viewport_KeyPress( Viewport viewport, KeyPressEvent e, ref bool handled )
		{
			editor.PerformKeyPress( e, ref handled );
		}
		public void PerformBaseViewportKeyPress( KeyPressEvent e, ref bool handled )
		{
			base.Viewport_KeyPress( Viewport, e, ref handled );
		}

		protected override void Viewport_KeyUp( Viewport viewport, KeyEvent e, ref bool handled )
		{
			editor.PerformKeyUp( e, ref handled );
		}
		public void PerformBaseViewportKeyUp( KeyEvent e, ref bool handled )
		{
			base.Viewport_KeyUp( Viewport, e, ref handled );
		}

		protected override void Viewport_MouseDown( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			editor.PerformMouseDown( button, ref handled );
		}
		public void PerformBaseViewportMouseDown( EMouseButtons button, ref bool handled )
		{
			base.Viewport_MouseDown( Viewport, button, ref handled );
		}

		protected override void Viewport_MouseUp( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			editor.PerformMouseUp( button, ref handled );
		}
		public void PerformBaseViewportMouseUp( EMouseButtons button, ref bool handled )
		{
			base.Viewport_MouseUp( Viewport, button, ref handled );
		}

		protected override void Viewport_MouseDoubleClick( Viewport viewport, EMouseButtons button, ref bool handled )
		{
			editor.PerformMouseDoubleClick( button, ref handled );
		}
		public void PerformBaseViewportMouseDoubleClick( EMouseButtons button, ref bool handled )
		{
			base.Viewport_MouseDoubleClick( Viewport, button, ref handled );
		}

		protected override void Viewport_MouseMove( Viewport viewport, Vector2 mouse )
		{
			editor.PerformMouseMove( mouse );
		}
		public void PerformBaseViewportMouseMove( Vector2 mouse )
		{
			base.Viewport_MouseMove( Viewport, mouse );
		}

		protected override void Viewport_MouseRelativeModeChanged( Viewport viewport, ref bool handled )
		{
			editor.PerformMouseRelativeModeChanged( ref handled );
		}
		public void PerformBaseViewportMouseRelativeModeChanged( ref bool handled )
		{
			base.Viewport_MouseRelativeModeChanged( Viewport, ref handled );
		}

		protected override void Viewport_MouseWheel( Viewport viewport, int delta, ref bool handled )
		{
			editor.PerformMouseWheel( delta, ref handled );
		}
		public void PerformBaseViewportMouseWheel( int delta, ref bool handled )
		{
			base.Viewport_MouseWheel( Viewport, delta, ref handled );
		}

		protected override void Viewport_JoystickEvent( Viewport viewport, JoystickInputEvent e, ref bool handled )
		{
			editor.PerformJoystickEvent( e, ref handled );
		}
		public void PerformBaseViewportJoystickEvent( JoystickInputEvent e, ref bool handled )
		{
			base.Viewport_JoystickEvent( Viewport, e, ref handled );
		}

		protected override void Viewport_SpecialInputDeviceEvent( Viewport viewport, InputEvent e, ref bool handled )
		{
			editor.PerformSpecialInputDeviceEvent( e, ref handled );
		}
		public void PerformBaseViewportSpecialInputDeviceEvent( InputEvent e, ref bool handled )
		{
			base.Viewport_SpecialInputDeviceEvent( Viewport, e, ref handled );
		}

		protected override void Viewport_Tick( Viewport viewport, float delta )
		{
			editor.PerformTick( delta );
		}
		public void PerformBaseViewportTick( float delta )
		{
			base.Viewport_Tick( Viewport, delta );
		}

		protected override void OnTimer10MsTick()
		{
			editor.PerformOnTimer10MsTick();
		}
		public void PerformOnTimer10MsTick()
		{
			base.OnTimer10MsTick();
		}

		protected override void Viewport_UpdateBegin( Viewport viewport )
		{
			editor.PerformViewportUpdateBegin();
		}
		public void PerformBaseViewportUpdateBegin()
		{
			base.Viewport_UpdateBegin( Viewport );
		}

		protected override void Viewport_UpdateEnd( Viewport viewport )
		{
			editor.PerformViewportUpdateEnd();
		}
		public void PerformBaseViewportUpdateEnd()
		{
			base.Viewport_UpdateEnd( Viewport );
		}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			editor.PerformViewportCreated();
		}
		public void PerformBaseViewportCreated()
		{
			base.ViewportControl_ViewportCreated( ViewportControl2 );
		}

		protected override void ViewportControl_ViewportDestroyed( EngineViewportControl sender )
		{
			editor.PerformViewportDestroyed();
		}
		public void PerformBaseViewportDestroyed()
		{
			base.ViewportControl_ViewportDestroyed( ViewportControl2 );
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			editor.PerformViewportUpdateBeforeOutput();
		}
		public void PerformBaseViewportUpdateBeforeOutput()
		{
			base.Viewport_UpdateBeforeOutput( Viewport );
		}

		protected override void Viewport_UpdateBeforeOutput2( Viewport viewport )
		{
			editor.PerformViewportUpdateBeforeOutput2();
		}
		public void PerformBaseViewportUpdateBeforeOutput2()
		{
			base.Viewport_UpdateBeforeOutput2( Viewport );
		}

		protected override void Viewport_UpdateGetObjectInSceneRenderingContext( Viewport viewport, ref ObjectInSpace.RenderingContext context )
		{
			editor.PerformViewportUpdateGetObjectInSceneRenderingContext( ref context );
		}
		public void PerformBaseViewportUpdateGetObjectInSceneRenderingContext( ref ObjectInSpace.RenderingContext context )
		{
			base.Viewport_UpdateGetObjectInSceneRenderingContext( Viewport, ref context );
		}

		protected override void Scene_ViewportUpdateGetCameraSettings( Scene scene, Viewport viewport, ref bool processed )
		{
			editor.PerformSceneViewportUpdateGetCameraSettings( ref processed );
		}
		public void PerformBaseSceneViewportUpdateGetCameraSettings( ref bool processed )
		{
			base.Scene_ViewportUpdateGetCameraSettings( Scene, Viewport, ref processed );
		}

		protected override void GetTextInfoLeftTopCorner( List<string> lines )
		{
			editor.PerformOnGetTextInfoLeftTopCorner( lines );
		}
		public void PerformBaseGetTextInfoLeftTopCorner( List<string> lines )
		{
			base.GetTextInfoLeftTopCorner( lines );
		}

		protected override void GetTextInfoRightBottomCorner( List<string> lines )
		{
			editor.PerformOnGetTextInfoRightBottomCorner( lines );
		}
		public void PerformBaseGetTextInfoRightBottomCorner( List<string> lines )
		{
			base.GetTextInfoRightBottomCorner( lines );
		}

		protected override void GetTextInfoCenterBottomCorner( List<string> lines )
		{
			editor.PerformOnGetTextInfoCenterBottomCorner( lines );
		}
		public void PerformBaseGetTextInfoCenterBottomCorner( List<string> lines )
		{
			base.GetTextInfoCenterBottomCorner( lines );
		}

		private void DocumentWindowWithViewport_CanvasBasedEditor_SelectedObjectsChanged( IDocumentWindow sender, object[] oldSelectedObjects )
		{
			editor.PerformSelectedObjectsChanged( oldSelectedObjects );
		}

		public override void EditorActionGetState( EditorActionGetStateContext context )
		{
			editor.PerformOnEditorActionGetState( context );
		}
		public void PerformEditorActionGetState( EditorActionGetStateContext context )
		{
			base.EditorActionGetState( context );
		}

		public override void EditorActionClick( EditorActionClickContext context )
		{
			editor.PerformOnEditorActionClick( context );
		}
		public void PerformEditorActionClick( EditorActionClickContext context )
		{
			base.EditorActionClick( context );
		}

		public override void EditorActionClick2( EditorActionClickContext context )
		{
			editor.PerformOnEditorActionClick2( context );
		}
		public void PerformEditorActionClick2( EditorActionClickContext context )
		{
			base.EditorActionClick2( context );
		}
	}
}