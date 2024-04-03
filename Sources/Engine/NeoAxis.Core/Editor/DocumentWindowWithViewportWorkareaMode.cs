//#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public abstract class DocumentWindowWithViewportWorkareaMode
	{
		IDocumentWindowWithViewport documentWindow;

		//

		protected DocumentWindowWithViewportWorkareaMode( IDocumentWindowWithViewport documentWindow )
		{
			this.documentWindow = documentWindow;
		}

		public IDocumentWindowWithViewport DocumentWindow
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
		public delegate void DestroyDelegate( DocumentWindowWithViewportWorkareaMode sender );
		public event DestroyDelegate Destroy;
		internal void PerformDestroy()
		{
			OnDestroy();
			Destroy?.Invoke( this );
		}

		protected virtual void OnGetTextInfoRightBottomCorner( List<string> lines ) { }
		public delegate void GetTextInfoRightBottomCornerDelegate( DocumentWindowWithViewportWorkareaMode sender, List<string> lines );
		public event GetTextInfoRightBottomCornerDelegate GetTextInfoRightBottomCorner;
		internal void PerformGetTextInfoRightBottomCorner( List<string> lines )
		{
			OnGetTextInfoRightBottomCorner( lines );
			GetTextInfoRightBottomCorner?.Invoke( this, lines );
		}

		protected virtual void OnGetTextInfoCenterBottomCorner( List<string> lines ) { }
		public delegate void GetTextInfoCenterBottomCornerDelegate( DocumentWindowWithViewportWorkareaMode sender, List<string> lines );
		public event GetTextInfoCenterBottomCornerDelegate GetTextInfoCenterBottomCorner;
		internal void PerformGetTextInfoCenterBottomCorner( List<string> lines )
		{
			OnGetTextInfoCenterBottomCorner( lines );
			GetTextInfoCenterBottomCorner?.Invoke( this, lines );
		}

		protected virtual bool OnKeyDown( Viewport viewport, KeyEvent e ) { return false; }
		public delegate void KeyDownUpDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport, KeyEvent e, ref bool handled );
		public event KeyDownUpDelegate KeyDown;
		internal bool PerformKeyDown( Viewport viewport, KeyEvent e )
		{
			var handled = OnKeyDown( viewport, e );
			if( !handled )
				KeyDown?.Invoke( this, viewport, e, ref handled );
			return handled;
		}

		protected virtual bool OnKeyPress( Viewport viewport, KeyPressEvent e ) { return false; }
		public delegate void KeyPressDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport, KeyPressEvent e, ref bool handled );
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
		public delegate void MouseClickDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport, EMouseButtons button, ref bool handled );
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
		public delegate void MouseMoveDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport, Vector2 mouse );
		public event MouseMoveDelegate MouseMove;
		internal void PerformMouseMove( Viewport viewport, Vector2 mouse )
		{
			OnMouseMove( viewport, mouse );
			MouseMove?.Invoke( this, viewport, mouse );
		}

		protected virtual bool OnMouseRelativeModeChanged( Viewport viewport ) { return false; }
		public delegate void MouseRelativeModeChangedDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport, ref bool handled );
		public event MouseRelativeModeChangedDelegate MouseRelativeModeChanged;
		internal bool PerformMouseRelativeModeChanged( Viewport viewport )
		{
			var handled = OnMouseRelativeModeChanged( viewport );
			if( !handled )
				MouseRelativeModeChanged?.Invoke( this, viewport, ref handled );
			return handled;
		}

		protected virtual bool OnMouseWheel( Viewport viewport, int delta ) { return false; }
		public delegate void MouseWheelDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport, int delta, ref bool handled );
		public event MouseWheelDelegate MouseWheel;
		internal bool PerformMouseWheel( Viewport viewport, int delta )
		{
			var handled = OnMouseWheel( viewport, delta );
			if( !handled )
				MouseWheel?.Invoke( this, viewport, delta, ref handled );
			return handled;
		}

		protected virtual bool OnJoystickEvent( Viewport viewport, JoystickInputEvent e ) { return false; }
		public delegate void JoystickEventDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport, JoystickInputEvent e, ref bool handled );
		public event JoystickEventDelegate JoystickEvent;
		internal bool PerformJoystickEvent( Viewport viewport, JoystickInputEvent e )
		{
			var handled = OnJoystickEvent( viewport, e );
			if( !handled )
				JoystickEvent?.Invoke( this, viewport, e, ref handled );
			return handled;
		}

		protected virtual bool OnSpecialInputDeviceEvent( Viewport viewport, InputEvent e ) { return false; }
		public delegate void SpecialInputDeviceEventDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport, InputEvent e, ref bool handled );
		public event SpecialInputDeviceEventDelegate SpecialInputDeviceEvent;
		internal bool PerformSpecialInputDeviceEvent( Viewport viewport, InputEvent e )
		{
			var handled = OnSpecialInputDeviceEvent( viewport, e );
			if( !handled )
				SpecialInputDeviceEvent?.Invoke( this, viewport, e, ref handled );
			return handled;
		}

		protected virtual void OnTick( Viewport viewport, double delta ) { }
		public delegate void TickDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport, double delta );
		public event TickDelegate Tick;
		internal void PerformTick( Viewport viewport, double delta )
		{
			OnTick( viewport, delta );
			Tick?.Invoke( this, viewport, delta );
		}

		protected virtual void OnUpdateBegin( Viewport viewport ) { }
		public delegate void UpdateBeginDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport );
		public event UpdateBeginDelegate UpdateBegin;
		internal void PerformUpdateBegin( Viewport viewport )
		{
			OnUpdateBegin( viewport );
			UpdateBegin?.Invoke( this, viewport );
		}

		protected virtual void OnUpdateGetObjectInSceneRenderingContext( Viewport viewport, ref ObjectInSpace.RenderingContext context ) { }
		public delegate void UpdateGetObjectInSceneRenderingContextDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport, ref ObjectInSpace.RenderingContext context );
		public event UpdateGetObjectInSceneRenderingContextDelegate UpdateGetObjectInSceneRenderingContext;
		internal void PerformUpdateGetObjectInSceneRenderingContext( Viewport viewport, ref ObjectInSpace.RenderingContext context )
		{
			OnUpdateGetObjectInSceneRenderingContext( viewport, ref context );
			UpdateGetObjectInSceneRenderingContext?.Invoke( this, viewport, ref context );
		}

		protected virtual void OnUpdateBeforeOutput( Viewport viewport ) { }
		public delegate void UpdateBeforeOutputDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport );
		public event UpdateBeforeOutputDelegate UpdateBeforeOutput;
		internal void PerformUpdateBeforeOutput( Viewport viewport )
		{
			OnUpdateBeforeOutput( viewport );
			UpdateBeforeOutput?.Invoke( this, viewport );
		}

		protected virtual void OnUpdateBeforeOutput2( Viewport viewport ) { }
		public delegate void UpdateBeforeOutput2Delegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport );
		public event UpdateBeforeOutput2Delegate UpdateBeforeOutput2;
		internal void PerformUpdateBeforeOutput2( Viewport viewport )
		{
			OnUpdateBeforeOutput2( viewport );
			UpdateBeforeOutput2?.Invoke( this, viewport );
		}

		protected virtual void OnUpdateEnd( Viewport viewport ) { }
		public delegate void UpdateEndDelegate( DocumentWindowWithViewportWorkareaMode sender, Viewport viewport );
		public event UpdateEndDelegate UpdateEnd;
		internal void PerformUpdateEnd( Viewport viewport )
		{
			OnUpdateEnd( viewport );
			UpdateEnd?.Invoke( this, viewport );
		}

		protected virtual void OnViewportUpdateGetCameraSettings( ref Camera camera ) { }
		public delegate void ViewportUpdateGetCameraSettingsDelegate( DocumentWindowWithViewportWorkareaMode sender, ref Camera camera );
		public event ViewportUpdateGetCameraSettingsDelegate ViewportUpdateGetCameraSettings;
		internal void PerformViewportUpdateGetCameraSettings( ref Camera camera )
		{
			OnViewportUpdateGetCameraSettings( ref camera );
			ViewportUpdateGetCameraSettings?.Invoke( this, ref camera );
		}

		//!!!!надо ли ref bool handled
		protected virtual void OnEditorActionGetState( EditorActionGetStateContext context ) { }
		public delegate void EditorActionGetStateDelegate( DocumentWindowWithViewportWorkareaMode sender, EditorActionGetStateContext context );
		public event EditorActionGetStateDelegate EditorActionGetState;
		internal void PerformEditorActionGetState( EditorActionGetStateContext context )
		{
			OnEditorActionGetState( context );
			EditorActionGetState?.Invoke( this, context );
		}

		//!!!!надо ли ref bool handled
		protected virtual void OnEditorActionClick( EditorActionClickContext context ) { }
		public delegate void EditorActionClickDelegate( DocumentWindowWithViewportWorkareaMode sender, EditorActionClickContext context );
		public event EditorActionClickDelegate EditorActionClick;
		internal void PerformEditorActionClick( EditorActionClickContext context )
		{
			OnEditorActionClick( context );
			EditorActionClick?.Invoke( this, context );
		}

		//!!!!надо ли ref bool handled
		protected virtual void OnEditorActionClick2( EditorActionClickContext context ) { }
		public event EditorActionClickDelegate EditorActionClick2;
		internal void PerformEditorActionClick2( EditorActionClickContext context )
		{
			OnEditorActionClick2( context );
			EditorActionClick2?.Invoke( this, context );
		}
	}
}

//#endif