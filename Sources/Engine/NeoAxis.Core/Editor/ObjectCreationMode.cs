// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using NeoAxis.Input;

namespace NeoAxis.Editor
{
	public class ObjectCreationModeAttribute : Attribute
	{
		Type creationModeClass;

		public ObjectCreationModeAttribute( Type creationModeClass )
		{
			this.creationModeClass = creationModeClass;
		}

		public Type CreationModeClass
		{
			get { return creationModeClass; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class ObjectCreationMode
	{
		DocumentWindowWithViewport documentWindow;
		Component creatingObject;

		//

		protected ObjectCreationMode( DocumentWindowWithViewport documentWindow, Component creatingObject )
		{
			this.documentWindow = documentWindow;
			this.creatingObject = creatingObject;
		}

		public DocumentWindowWithViewport DocumentWindow
		{
			get { return documentWindow; }
		}

		public Component CreatingObject
		{
			get { return creatingObject; }
			set { creatingObject = value; }
		}

		protected virtual void OnDestroy() { }
		public delegate void DestroyDelegate( ObjectCreationMode sender );
		public event DestroyDelegate Destroy;
		internal void PerformDestroy()
		{
			OnDestroy();
			Destroy?.Invoke( this );
		}

		protected virtual void OnGetTextInfoRightBottomCorner( List<string> lines ) { }
		public delegate void GetTextInfoRightBottomCornerDelegate( ObjectCreationMode sender, List<string> lines );
		public event GetTextInfoRightBottomCornerDelegate GetTextInfoRightBottomCorner;
		internal void PerformGetTextInfoRightBottomCorner( List<string> lines )
		{
			OnGetTextInfoRightBottomCorner( lines );
			GetTextInfoRightBottomCorner?.Invoke( this, lines );
		}

		protected virtual void OnGetTextInfoCenterBottomCorner( List<string> lines ) { }
		public delegate void GetTextInfoCenterBottomCornerDelegate( ObjectCreationMode sender, List<string> lines );
		public event GetTextInfoCenterBottomCornerDelegate GetTextInfoCenterBottomCorner;
		internal void PerformGetTextInfoCenterBottomCorner( List<string> lines )
		{
			OnGetTextInfoCenterBottomCorner( lines );
			GetTextInfoCenterBottomCorner?.Invoke( this, lines );
		}

		protected virtual bool OnKeyDown( Viewport viewport, KeyEvent e ) { return false; }
		public delegate void KeyDownUpDelegate( ObjectCreationMode sender, Viewport viewport, KeyEvent e, ref bool handled );
		public event KeyDownUpDelegate KeyDown;
		internal bool PerformKeyDown( Viewport viewport, KeyEvent e )
		{
			var handled = OnKeyDown( viewport, e );
			if( !handled )
				KeyDown?.Invoke( this, viewport, e, ref handled );
			return handled;
		}

		protected virtual bool OnKeyPress( Viewport viewport, KeyPressEvent e ) { return false; }
		public delegate void KeyPressDelegate( ObjectCreationMode sender, Viewport viewport, KeyPressEvent e, ref bool handled );
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
		public delegate void MouseClickDelegate( ObjectCreationMode sender, Viewport viewport, EMouseButtons button, ref bool handled );
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
		public delegate void MouseMoveDelegate( ObjectCreationMode sender, Viewport viewport, Vector2 mouse );
		public event MouseMoveDelegate MouseMove;
		internal void PerformMouseMove( Viewport viewport, Vector2 mouse )
		{
			OnMouseMove( viewport, mouse );
			MouseMove?.Invoke( this, viewport, mouse );
		}

		protected virtual bool OnMouseRelativeModeChanged( Viewport viewport ) { return false; }
		public delegate void MouseRelativeModeChangedDelegate( ObjectCreationMode sender, Viewport viewport, ref bool handled );
		public event MouseRelativeModeChangedDelegate MouseRelativeModeChanged;
		internal bool PerformMouseRelativeModeChanged( Viewport viewport )
		{
			var handled = OnMouseRelativeModeChanged( viewport );
			if( !handled )
				MouseRelativeModeChanged?.Invoke( this, viewport, ref handled );
			return handled;
		}

		protected virtual bool OnMouseWheel( Viewport viewport, int delta ) { return false; }
		public delegate void MouseWheelDelegate( ObjectCreationMode sender, Viewport viewport, int delta, ref bool handled );
		public event MouseWheelDelegate MouseWheel;
		internal bool PerformMouseWheel( Viewport viewport, int delta )
		{
			var handled = OnMouseWheel( viewport, delta );
			if( !handled )
				MouseWheel?.Invoke( this, viewport, delta, ref handled );
			return handled;
		}

		protected virtual bool OnJoystickEvent( Viewport viewport, JoystickInputEvent e ) { return false; }
		public delegate void JoystickEventDelegate( ObjectCreationMode sender, Viewport viewport, JoystickInputEvent e, ref bool handled );
		public event JoystickEventDelegate JoystickEvent;
		internal bool PerformJoystickEvent( Viewport viewport, JoystickInputEvent e )
		{
			var handled = OnJoystickEvent( viewport, e );
			if( !handled )
				JoystickEvent?.Invoke( this, viewport, e, ref handled );
			return handled;
		}

		protected virtual bool OnSpecialInputDeviceEvent( Viewport viewport, InputEvent e ) { return false; }
		public delegate void SpecialInputDeviceEventDelegate( ObjectCreationMode sender, Viewport viewport, InputEvent e, ref bool handled );
		public event SpecialInputDeviceEventDelegate SpecialInputDeviceEvent;
		internal bool PerformSpecialInputDeviceEvent( Viewport viewport, InputEvent e )
		{
			var handled = OnSpecialInputDeviceEvent( viewport, e );
			if( !handled )
				SpecialInputDeviceEvent?.Invoke( this, viewport, e, ref handled );
			return handled;
		}

		protected virtual void OnTick( Viewport viewport, double delta ) { }
		public delegate void TickDelegate( ObjectCreationMode sender, Viewport viewport, double delta );
		public event TickDelegate Tick;
		internal void PerformTick( Viewport viewport, double delta )
		{
			OnTick( viewport, delta );
			Tick?.Invoke( this, viewport, delta );
		}

		protected virtual void OnUpdateBegin( Viewport viewport ) { }
		public delegate void UpdateBeginDelegate( ObjectCreationMode sender, Viewport viewport );
		public event UpdateBeginDelegate UpdateBegin;
		internal void PerformUpdateBegin( Viewport viewport )
		{
			OnUpdateBegin( viewport );
			UpdateBegin?.Invoke( this, viewport );
		}

		protected virtual void OnUpdateGetObjectInSceneRenderingContext( Viewport viewport, ref Component_ObjectInSpace.RenderingContext context ) { }
		public delegate void UpdateGetObjectInSceneRenderingContextDelegate( ObjectCreationMode sender, Viewport viewport, ref Component_ObjectInSpace.RenderingContext context );
		public event UpdateGetObjectInSceneRenderingContextDelegate UpdateGetObjectInSceneRenderingContext;
		internal void PerformUpdateGetObjectInSceneRenderingContext( Viewport viewport, ref Component_ObjectInSpace.RenderingContext context )
		{
			OnUpdateGetObjectInSceneRenderingContext( viewport, ref context );
			UpdateGetObjectInSceneRenderingContext?.Invoke( this, viewport, ref context );
		}

		protected virtual void OnUpdateBeforeOutput( Viewport viewport ) { }
		public delegate void UpdateBeforeOutputDelegate( ObjectCreationMode sender, Viewport viewport );
		public event UpdateBeforeOutputDelegate UpdateBeforeOutput;
		internal void PerformUpdateBeforeOutput( Viewport viewport )
		{
			OnUpdateBeforeOutput( viewport );
			UpdateBeforeOutput?.Invoke( this, viewport );
		}

		protected virtual void OnUpdateBeforeOutput2( Viewport viewport ) { }
		public delegate void UpdateBeforeOutput2Delegate( ObjectCreationMode sender, Viewport viewport );
		public event UpdateBeforeOutput2Delegate UpdateBeforeOutput2;
		internal void PerformUpdateBeforeOutput2( Viewport viewport )
		{
			OnUpdateBeforeOutput2( viewport );
			UpdateBeforeOutput2?.Invoke( this, viewport );
		}

		protected virtual void OnUpdateEnd( Viewport viewport ) { }
		public delegate void UpdateEndDelegate( ObjectCreationMode sender, Viewport viewport );
		public event UpdateEndDelegate UpdateEnd;
		internal void PerformUpdateEnd( Viewport viewport )
		{
			OnUpdateEnd( viewport );
			UpdateEnd?.Invoke( this, viewport );
		}

		protected virtual void OnViewportUpdateGetCameraSettings( ref Component_Camera camera ) { }
		public delegate void ViewportUpdateGetCameraSettingsDelegate( ObjectCreationMode sender, ref Component_Camera camera );
		public event ViewportUpdateGetCameraSettingsDelegate ViewportUpdateGetCameraSettings;
		internal void PerformViewportUpdateGetCameraSettings( ref Component_Camera camera )
		{
			OnViewportUpdateGetCameraSettings( ref camera );
			ViewportUpdateGetCameraSettings?.Invoke( this, ref camera );
		}

		//!!!!надо ли ref bool handled
		protected virtual void OnEditorActionGetState( EditorAction.GetStateContext context ) { }
		public delegate void EditorActionGetStateDelegate( ObjectCreationMode sender, EditorAction.GetStateContext context );
		public event EditorActionGetStateDelegate EditorActionGetState;
		internal void PerformEditorActionGetState( EditorAction.GetStateContext context )
		{
			OnEditorActionGetState( context );
			EditorActionGetState?.Invoke( this, context );
		}

		//!!!!надо ли ref bool handled
		protected virtual void OnEditorActionClick( EditorAction.ClickContext context ) { }
		public delegate void EditorActionClickDelegate( ObjectCreationMode sender, EditorAction.ClickContext context );
		public event EditorActionClickDelegate EditorActionClick;
		internal void PerformEditorActionClick( EditorAction.ClickContext context )
		{
			OnEditorActionClick( context );
			EditorActionClick?.Invoke( this, context );
		}

		public virtual void Finish( bool cancel )
		{
			if( !cancel )
			{
				//add to undo with deletion
				var document = DocumentWindow.Document;
				var action = new UndoActionComponentCreateDelete( document, new Component[] { CreatingObject }, true );
				document.UndoSystem.CommitAction( action );
				document.Modified = true;
			}
			else
			{
				CreatingObject?.Dispose();
				documentWindow.SelectObjects( null );
			}

			DocumentWindow.ObjectCreationModeSet( null );
		}

	}
}
