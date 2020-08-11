// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using NeoAxis.Input;
using NeoAxis.Widget;

//!!!! is not fully implemented. use DocumentWindowWithViewport when need working area, creation mode

namespace NeoAxis.Editor
{
	public class DocumentWindowWithViewport_CanvasBasedEditor : DocumentWindowWithViewport
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
			base.ViewportControl_ViewportCreated( ViewportControl );
		}

		protected override void ViewportControl_ViewportDestroyed( EngineViewportControl sender )
		{
			editor.PerformViewportDestroyed();
		}
		public void PerformBaseViewportDestroyed()
		{
			base.ViewportControl_ViewportDestroyed( ViewportControl );
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

		protected override void Viewport_UpdateGetObjectInSceneRenderingContext( Viewport viewport, ref Component_ObjectInSpace.RenderingContext context )
		{
			editor.PerformViewportUpdateGetObjectInSceneRenderingContext( ref context );
		}
		public void PerformBaseViewportUpdateGetObjectInSceneRenderingContext( ref Component_ObjectInSpace.RenderingContext context )
		{
			base.Viewport_UpdateGetObjectInSceneRenderingContext( Viewport, ref context );
		}

		protected override void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
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

		private void DocumentWindowWithViewport_CanvasBasedEditor_SelectedObjectsChanged( DocumentWindow sender, object[] oldSelectedObjects )
		{
			editor.PerformSelectedObjectsChanged( oldSelectedObjects );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class CanvasBasedEditor
	{
		internal DocumentWindowWithViewport_CanvasBasedEditor owner;

		//

		public CanvasBasedEditor()
		{
		}

		public DocumentWindowWithViewport Owner
		{
			get { return owner; }
		}

		/////////////////////////////////////////

		public DocumentInstance Document
		{
			get { return Owner?.Document; }
		}

		public object ObjectOfEditor
		{
			get { return owner.ObjectOfWindow; }
		}

		public bool OpenAsSettings // now used only for ObjectSettingsWindow
		{
			get { return owner.OpenAsSettings; }
		}

		public Dictionary<string, object> WindowTypeSpecificOptions
		{
			get { return owner.WindowTypeSpecificOptions; }
		}

		public bool IsDocumentSaved()
		{
			return owner.IsDocumentSaved();
		}

		public object[] SelectedObjects
		{
			get { return owner.SelectedObjects; }
		}

		public ESet<object> SelectedObjectsSet
		{
			get { return owner.SelectedObjectsSet; }
		}

		public bool IsWindowInWorkspace
		{
			get { return owner.IsWindowInWorkspace; }
			set { owner.IsWindowInWorkspace = value; }
		}

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

		//protected override string GetResultWindowTitle()
		//{
		//	if( document != null )
		//	{
		//		var title = "";

		//		if( IsMainWindowInWorkspace && !string.IsNullOrEmpty( document.RealFileName ) )
		//			title = Path.GetFileName( document.RealFileName );
		//		else if( ObjectOfWindow != null )
		//			title = ObjectOfWindow.ToString();

		//		if( !string.IsNullOrEmpty( title ) )
		//		{
		//			if( IsWindowInWorkspace )
		//			{
		//				if( ObjectOfWindowIsDeleted )
		//					title += " (Deleted)";
		//				else if( IsMainWindowInWorkspace )
		//					title = "'Root object'";
		//			}
		//			else
		//			{
		//				if( document != null && document.Modified )
		//					title += "*";
		//			}

		//			return title;
		//		}
		//	}

		//	return base.GetResultWindowTitle();
		//}

		//internal protected override void OnShowTitleContextMenu( KryptonContextMenuItems items )
		//{
		//	Document.OnShowTitleContextMenu( this, items );
		//}

		public bool SaveDocument()
		{
			return owner.SaveDocument();
		}

		public bool IsObjectSelected( object obj )
		{
			return owner.IsObjectSelected( obj );
		}

		public delegate void SelectedObjectsChangedDelegate( CanvasBasedEditor sender, object[] oldSelectedObjects );
		public event SelectedObjectsChangedDelegate SelectedObjectsChanged;
		internal void PerformSelectedObjectsChanged( object[] oldSelectedObjects )
		{
			SelectedObjectsChanged?.Invoke( this, oldSelectedObjects );
		}

		public void SelectObjects( ICollection<object> objects, bool updateForeachDocumentWindowContainers = true, bool updateSettingsWindowSelectObjects = true, bool forceUpdate = false )
		{
			owner.SelectObjects( objects, updateForeachDocumentWindowContainers, updateSettingsWindowSelectObjects, forceUpdate );
		}

		protected virtual void OnTimer10MsTick()
		{
			owner.PerformOnTimer10MsTick();
		}
		internal void PerformOnTimer10MsTick()
		{
			OnTimer10MsTick();
		}

		//!!!!

		//protected virtual bool CanUpdateSettingsWindowsSelectedObjects()
		//{
		//	return true;
		//}

		//protected virtual object OnGetSelectObjectWhenNoSelectedObjects()
		//{
		//	return ObjectOfWindow;
		//}

		public void SettingsWindowSelectObjects()
		{
			owner.SettingsWindowSelectObjects();
		}

		//!!!!

		//public virtual void EditorActionGetState( EditorAction.GetStateContext context )
		//{
		//	switch( context.Action.Name )
		//	{
		//	case "Cut":
		//		if( CanCut() )
		//			context.Enabled = true;
		//		break;

		//	case "Copy":
		//		if( CanCopy() )
		//			context.Enabled = true;
		//		break;

		//	case "Paste":
		//		if( CanPaste( out _ ) )
		//			context.Enabled = true;
		//		break;

		//	case "Duplicate":
		//		if( CanCloneObjects( out _ ) )
		//			context.Enabled = true;
		//		break;

		//	case "Delete":
		//		if( CanDeleteObjects( out _ ) )
		//			context.Enabled = true;
		//		break;

		//	case "Rename":
		//		if( CanRename( out _ ) )
		//			context.Enabled = true;
		//		break;
		//	}
		//}

		//public virtual void EditorActionClick( EditorAction.ClickContext context )
		//{
		//	switch( context.Action.Name )
		//	{
		//	case "Cut":
		//		Cut();
		//		break;

		//	case "Copy":
		//		Copy();
		//		break;

		//	case "Paste":
		//		Paste();
		//		break;

		//	case "Duplicate":
		//		TryCloneObjects();
		//		break;

		//	case "Delete":
		//		TryDeleteObjects();
		//		break;

		//	case "Rename":
		//		TryRename();
		//		break;
		//	}
		//}

		//public override ObjectsInFocus GetObjectsInFocus()
		//{
		//	if( SelectedObjects.Length != 0 )
		//		return new ObjectsInFocus( this, SelectedObjects );
		//	return base.GetObjectsInFocus();
		//}

		//public virtual bool ObjectOfWindowIsDeleted
		//{
		//	get
		//	{
		//		var component = ObjectOfWindow as Component;
		//		if( component != null && document != null && ObjectOfWindow != document.ResultComponent &&
		//			document.ResultComponent != null && !component.GetAllParents( false ).Contains( document.ResultComponent ) )
		//		{
		//			return true;
		//		}

		//		return false;
		//	}
		//}

		//protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		//{
		//	if( keyData == ( Keys.Control | Keys.F4 ) )
		//	{
		//		//close root window of the document when second level windows are exists
		//		if( IsMainWindowInWorkspace )
		//		{
		//			var workspaceWindow = EditorForm.Instance.WorkspaceController.FindWorkspaceWindow( this );
		//			if( workspaceWindow != null )
		//			{
		//				workspaceWindow.Close();
		//				return true;
		//			}
		//		}

		//		Close();
		//		return true;
		//	}

		//	return base.ProcessCmdKey( ref msg, keyData );
		//}

		//public virtual bool CanCut()
		//{
		//	if( SelectedObjects.Length != 0 )
		//		return SelectedObjects.All( obj => obj is Component && ( (Component)obj ).Parent != null );
		//	return false;
		//}

		//public virtual bool CutCopy( bool cut )
		//{
		//	if( !CanCut() )
		//		return false;
		//	var data = new ObjectCutCopyPasteData( this, cut, (object[])SelectedObjects.Clone() );
		//	ClipboardManager.CopyToClipboard( data );
		//	return true;
		//}

		//public bool Cut()
		//{
		//	return CutCopy( true );
		//}

		//public virtual bool CanCopy()
		//{
		//	if( SelectedObjects.Length != 0 )
		//		return SelectedObjects.All( obj => obj is Component );
		//	return false;
		//}

		//public bool Copy()
		//{
		//	return CutCopy( false );
		//}

		//public virtual bool CanPaste( out Component destinationParent )
		//{
		//	if( ClipboardManager.CheckAvailableInClipboard<ObjectCutCopyPasteData>() )
		//	{
		//		if( SelectedObjects.Length == 0 )
		//		{
		//			var c = ObjectOfWindow as Component;
		//			if( c != null )
		//			{
		//				destinationParent = c;
		//				return true;
		//			}
		//		}
		//		else if( SelectedObjects.Length == 1 )
		//		{
		//			var c = SelectedObjects[ 0 ] as Component;
		//			if( c != null )
		//			{
		//				destinationParent = c;
		//				return true;
		//			}
		//		}
		//	}

		//	destinationParent = null;
		//	return false;
		//}

		//public virtual bool Paste()
		//{
		//	if( !CanPaste( out var destinationParent ) )
		//		return false;

		//	var data = ClipboardManager.GetFromClipboard<ObjectCutCopyPasteData>();
		//	if( data != null )
		//	{
		//		var components = new List<Component>();
		//		foreach( var obj in data.objects )
		//		{
		//			var c = obj as Component;
		//			if( c != null )
		//				components.Add( c );
		//		}

		//		//create new objects

		//		var newObjects = new List<Component>();
		//		Vector3 addToPosition = Vector3.Zero;

		//		for( int n = 0; n < components.Count; n++ )
		//		{
		//			var c = components[ n ];

		//			var cloned = c.Clone();
		//			if( destinationParent.GetComponent( c.Name ) == null )
		//				cloned.Name = c.Name;
		//			else
		//				cloned.Name = destinationParent.Components.GetUniqueName( c.Name, true, 2 );
		//			destinationParent.AddComponent( cloned );

		//			newObjects.Add( cloned );
		//		}

		//		if( data.cut )
		//		{
		//			//cut
		//			if( data.documentWindow.Document != Document )
		//			{
		//				//another document
		//				{
		//					var action = new UndoActionComponentCreateDelete( data.documentWindow.Document, components, false );
		//					data.documentWindow.Document.UndoSystem.CommitAction( action );
		//					data.documentWindow.Document.Modified = true;
		//				}
		//				{
		//					var action = new UndoActionComponentCreateDelete( Document, newObjects, true );
		//					Document.UndoSystem.CommitAction( action );
		//					Document.Modified = true;
		//				}
		//			}
		//			else
		//			{
		//				//same document
		//				var multiAction = new UndoMultiAction();
		//				multiAction.AddAction( new UndoActionComponentCreateDelete( Document, components, false ) );
		//				multiAction.AddAction( new UndoActionComponentCreateDelete( Document, newObjects, true ) );
		//				Document.UndoSystem.CommitAction( multiAction );
		//				Document.Modified = true;
		//			}
		//		}
		//		else
		//		{
		//			//copy
		//			var action = new UndoActionComponentCreateDelete( Document, newObjects, true );
		//			Document.UndoSystem.CommitAction( action );
		//			Document.Modified = true;
		//		}
		//	}

		//	return true;
		//}

		//public virtual bool CanCloneObjects( out List<Component> resultObjectsToClone )
		//{
		//	resultObjectsToClone = new List<Component>();

		//	//!!!!mutliselection

		//	//!!!!вложеные друг в друга убрать. где еще так

		//	//!!!!или из transform tool брать?
		//	foreach( var obj in SelectedObjects )
		//	{
		//		var component = obj as Component;
		//		if( component != null && component.Parent != null )
		//			resultObjectsToClone.Add( component );
		//	}

		//	if( resultObjectsToClone.Count == 0 )
		//		return false;

		//	return true;
		//}

		//public virtual void TryCloneObjects()
		//{
		//	//!!!!!игнорить выделенные-вложенные. где еще так

		//	if( !CanCloneObjects( out var objectsToClone ) )
		//		return;

		//	List<Component> newObjects = new List<Component>();
		//	foreach( var obj in objectsToClone )
		//	{
		//		var newObject = EditorUtility.CloneComponent( obj );
		//		newObjects.Add( newObject );
		//	}

		//	//select objects
		//	{
		//		var selectObjects = new List<object>();
		//		//!!!!все выделить?
		//		selectObjects.AddRange( newObjects );

		//		SelectObjects( selectObjects );
		//	}

		//	if( newObjects.Count == 0 )
		//		return;

		//	//add to undo with deletion
		//	var action = new UndoActionComponentCreateDelete( Document, newObjects, true );
		//	Document.UndoSystem.CommitAction( action );
		//	Document.Modified = true;

		//	//add screen message
		//	EditorUtility.ShowScreenNotificationObjectsCloned( newObjects.Count );
		//}

		//public virtual bool CanDeleteObjects( out List<object> resultObjectsToDelete )
		//{
		//	resultObjectsToDelete = new List<object>();

		//	foreach( var obj in SelectedObjects )
		//	{
		//		var component = obj as Component;
		//		if( component != null && component.Parent != null )
		//			resultObjectsToDelete.Add( component );
		//	}

		//	if( resultObjectsToDelete.Count == 0 )
		//		return false;

		//	return true;
		//}

		//public virtual bool TryDeleteObjects()
		//{
		//	//!!!!!игнорить выделенные-вложенные. где еще так

		//	if( !CanDeleteObjects( out var objectsToDelete ) )
		//		return false;

		//	string text;
		//	if( objectsToDelete.Count == 1 )
		//	{
		//		string template = EditorLocalization.Translate( "DocumentWindow", "Are you sure you want to delete \'{0}\'?" );
		//		var name = objectsToDelete[ 0 ].ToString();
		//		text = string.Format( template, name );
		//	}
		//	else
		//	{
		//		string template = EditorLocalization.Translate( "DocumentWindow", "Are you sure you want to delete selected objects?" );
		//		text = string.Format( template, objectsToDelete.Count );
		//	}

		//	if( EditorMessageBox.ShowQuestion( text, MessageBoxButtons.YesNo ) == DialogResult.No )
		//		return false;

		//	//!!!!может сцену выбрать? везде так
		//	//clear selected objects
		//	SelectObjects( null );

		//	//add to undo with deletion
		//	var action = new UndoActionComponentCreateDelete( Document, objectsToDelete.Cast<Component>().ToArray(), false );
		//	Document.UndoSystem.CommitAction( action );
		//	Document.Modified = true;

		//	return true;
		//}

		//public virtual bool CanRename( out Component component )
		//{
		//	component = null;

		//	//!!!!multiselection
		//	if( SelectedObjects.Length == 1 )
		//		component = SelectedObjects[ 0 ] as Component;

		//	return component != null;
		//}

		//public virtual void TryRename()
		//{
		//	if( CanRename( out var component ) )
		//		EditorUtility.ShowRenameComponentDialog( component );
		//}

		/////////////////////////////////////////

		//public abstract class WorkareaModeClass
		//{
		//	DocumentWindowWithViewport documentWindow;

		//	//

		//	protected WorkareaModeClass( DocumentWindowWithViewport documentWindow )
		//	{
		//		this.documentWindow = documentWindow;
		//	}

		//	public DocumentWindowWithViewport DocumentWindow
		//	{
		//		get { return documentWindow; }
		//	}

		//	public virtual bool AllowControlCamera
		//	{
		//		get { return true; }
		//	}

		//	public virtual bool AllowSelectObjects
		//	{
		//		get { return false; }
		//	}

		//	public virtual bool DisplaySelectedObjects
		//	{
		//		get { return AllowSelectObjects; }
		//	}

		//	public virtual bool AllowCreateObjectsByDrop
		//	{
		//		get { return AllowSelectObjects; }
		//	}

		//	public virtual bool AllowCreateObjectsByClick
		//	{
		//		get { return AllowSelectObjects; }
		//	}

		//	public virtual bool AllowCreateObjectsByBrush
		//	{
		//		get { return AllowSelectObjects; }
		//	}

		//	protected virtual void OnDestroy() { }
		//	public delegate void DestroyDelegate( WorkareaModeClass sender );
		//	public event DestroyDelegate Destroy;
		//	internal void PerformDestroy()
		//	{
		//		OnDestroy();
		//		Destroy?.Invoke( this );
		//	}

		//	protected virtual void OnGetTextInfoRightBottomCorner( List<string> lines ) { }
		//	public delegate void GetTextInfoRightBottomCornerDelegate( WorkareaModeClass sender, List<string> lines );
		//	public event GetTextInfoRightBottomCornerDelegate GetTextInfoRightBottomCorner;
		//	internal void PerformGetTextInfoRightBottomCorner( List<string> lines )
		//	{
		//		OnGetTextInfoRightBottomCorner( lines );
		//		GetTextInfoRightBottomCorner?.Invoke( this, lines );
		//	}

		//	protected virtual bool OnKeyDown( Viewport viewport, KeyEvent e ) { return false; }
		//	public delegate void KeyDownUpDelegate( WorkareaModeClass sender, Viewport viewport, KeyEvent e, ref bool handled );
		//	public event KeyDownUpDelegate KeyDown;
		//	internal bool PerformKeyDown( Viewport viewport, KeyEvent e )
		//	{
		//		var handled = OnKeyDown( viewport, e );
		//		if( !handled )
		//			KeyDown?.Invoke( this, viewport, e, ref handled );
		//		return handled;
		//	}

		//	protected virtual bool OnKeyPress( Viewport viewport, KeyPressEvent e ) { return false; }
		//	public delegate void KeyPressDelegate( WorkareaModeClass sender, Viewport viewport, KeyPressEvent e, ref bool handled );
		//	public event KeyPressDelegate KeyPress;
		//	internal bool PerformKeyPress( Viewport viewport, KeyPressEvent e )
		//	{
		//		var handled = OnKeyPress( viewport, e );
		//		if( !handled )
		//			KeyPress?.Invoke( this, viewport, e, ref handled );
		//		return handled;
		//	}

		//	protected virtual bool OnKeyUp( Viewport viewport, KeyEvent e ) { return false; }
		//	public event KeyDownUpDelegate KeyUp;
		//	internal bool PerformKeyUp( Viewport viewport, KeyEvent e )
		//	{
		//		var handled = OnKeyUp( viewport, e );
		//		if( !handled )
		//			KeyUp?.Invoke( this, viewport, e, ref handled );
		//		return handled;
		//	}

		//	protected virtual bool OnMouseDown( Viewport viewport, EMouseButtons button ) { return false; }
		//	public delegate void MouseClickDelegate( WorkareaModeClass sender, Viewport viewport, EMouseButtons button, ref bool handled );
		//	public event MouseClickDelegate MouseDown;
		//	internal bool PerformMouseDown( Viewport viewport, EMouseButtons button )
		//	{
		//		var handled = OnMouseDown( viewport, button );
		//		if( !handled )
		//			MouseDown?.Invoke( this, viewport, button, ref handled );
		//		return handled;
		//	}

		//	protected virtual bool OnMouseUp( Viewport viewport, EMouseButtons button ) { return false; }
		//	public event MouseClickDelegate MouseUp;
		//	internal bool PerformMouseUp( Viewport viewport, EMouseButtons button )
		//	{
		//		var handled = OnMouseUp( viewport, button );
		//		if( !handled )
		//			MouseUp?.Invoke( this, viewport, button, ref handled );
		//		return handled;
		//	}

		//	protected virtual bool OnMouseDoubleClick( Viewport viewport, EMouseButtons button ) { return false; }
		//	public event MouseClickDelegate MouseDoubleClick;
		//	internal bool PerformMouseDoubleClick( Viewport viewport, EMouseButtons button )
		//	{
		//		var handled = OnMouseDoubleClick( viewport, button );
		//		if( !handled )
		//			MouseDoubleClick?.Invoke( this, viewport, button, ref handled );
		//		return handled;
		//	}

		//	protected virtual void OnMouseMove( Viewport viewport, Vector2 mouse ) { }
		//	public delegate void MouseMoveDelegate( WorkareaModeClass sender, Viewport viewport, Vector2 mouse );
		//	public event MouseMoveDelegate MouseMove;
		//	internal void PerformMouseMove( Viewport viewport, Vector2 mouse )
		//	{
		//		OnMouseMove( viewport, mouse );
		//		MouseMove?.Invoke( this, viewport, mouse );
		//	}

		//	protected virtual bool OnMouseRelativeModeChanged( Viewport viewport ) { return false; }
		//	public delegate void MouseRelativeModeChangedDelegate( WorkareaModeClass sender, Viewport viewport, ref bool handled );
		//	public event MouseRelativeModeChangedDelegate MouseRelativeModeChanged;
		//	internal bool PerformMouseRelativeModeChanged( Viewport viewport )
		//	{
		//		var handled = OnMouseRelativeModeChanged( viewport );
		//		if( !handled )
		//			MouseRelativeModeChanged?.Invoke( this, viewport, ref handled );
		//		return handled;
		//	}

		//	protected virtual bool OnMouseWheel( Viewport viewport, int delta ) { return false; }
		//	public delegate void MouseWheelDelegate( WorkareaModeClass sender, Viewport viewport, int delta, ref bool handled );
		//	public event MouseWheelDelegate MouseWheel;
		//	internal bool PerformMouseWheel( Viewport viewport, int delta )
		//	{
		//		var handled = OnMouseWheel( viewport, delta );
		//		if( !handled )
		//			MouseWheel?.Invoke( this, viewport, delta, ref handled );
		//		return handled;
		//	}

		//	protected virtual bool OnJoystickEvent( Viewport viewport, JoystickInputEvent e ) { return false; }
		//	public delegate void JoystickEventDelegate( WorkareaModeClass sender, Viewport viewport, JoystickInputEvent e, ref bool handled );
		//	public event JoystickEventDelegate JoystickEvent;
		//	internal bool PerformJoystickEvent( Viewport viewport, JoystickInputEvent e )
		//	{
		//		var handled = OnJoystickEvent( viewport, e );
		//		if( !handled )
		//			JoystickEvent?.Invoke( this, viewport, e, ref handled );
		//		return handled;
		//	}

		//	protected virtual bool OnSpecialInputDeviceEvent( Viewport viewport, InputEvent e ) { return false; }
		//	public delegate void SpecialInputDeviceEventDelegate( WorkareaModeClass sender, Viewport viewport, InputEvent e, ref bool handled );
		//	public event SpecialInputDeviceEventDelegate SpecialInputDeviceEvent;
		//	internal bool PerformSpecialInputDeviceEvent( Viewport viewport, InputEvent e )
		//	{
		//		var handled = OnSpecialInputDeviceEvent( viewport, e );
		//		if( !handled )
		//			SpecialInputDeviceEvent?.Invoke( this, viewport, e, ref handled );
		//		return handled;
		//	}

		//	protected virtual void OnTick( Viewport viewport, double delta ) { }
		//	public delegate void TickDelegate( WorkareaModeClass sender, Viewport viewport, double delta );
		//	public event TickDelegate Tick;
		//	internal void PerformTick( Viewport viewport, double delta )
		//	{
		//		OnTick( viewport, delta );
		//		Tick?.Invoke( this, viewport, delta );
		//	}

		//	protected virtual void OnUpdateBegin( Viewport viewport ) { }
		//	public delegate void UpdateBeginDelegate( WorkareaModeClass sender, Viewport viewport );
		//	public event UpdateBeginDelegate UpdateBegin;
		//	internal void PerformUpdateBegin( Viewport viewport )
		//	{
		//		OnUpdateBegin( viewport );
		//		UpdateBegin?.Invoke( this, viewport );
		//	}

		//	protected virtual void OnUpdateGetObjectInSceneRenderingContext( Viewport viewport, ref Component_ObjectInSpace.RenderingContext context ) { }
		//	public delegate void UpdateGetObjectInSceneRenderingContextDelegate( WorkareaModeClass sender, Viewport viewport, ref Component_ObjectInSpace.RenderingContext context );
		//	public event UpdateGetObjectInSceneRenderingContextDelegate UpdateGetObjectInSceneRenderingContext;
		//	internal void PerformUpdateGetObjectInSceneRenderingContext( Viewport viewport, ref Component_ObjectInSpace.RenderingContext context )
		//	{
		//		OnUpdateGetObjectInSceneRenderingContext( viewport, ref context );
		//		UpdateGetObjectInSceneRenderingContext?.Invoke( this, viewport, ref context );
		//	}

		//	protected virtual void OnUpdateBeforeOutput( Viewport viewport ) { }
		//	public delegate void UpdateBeforeOutputDelegate( WorkareaModeClass sender, Viewport viewport );
		//	public event UpdateBeforeOutputDelegate UpdateBeforeOutput;
		//	internal void PerformUpdateBeforeOutput( Viewport viewport )
		//	{
		//		OnUpdateBeforeOutput( viewport );
		//		UpdateBeforeOutput?.Invoke( this, viewport );
		//	}

		//	protected virtual void OnUpdateBeforeOutput2( Viewport viewport ) { }
		//	public delegate void UpdateBeforeOutput2Delegate( WorkareaModeClass sender, Viewport viewport );
		//	public event UpdateBeforeOutput2Delegate UpdateBeforeOutput2;
		//	internal void PerformUpdateBeforeOutput2( Viewport viewport )
		//	{
		//		OnUpdateBeforeOutput2( viewport );
		//		UpdateBeforeOutput2?.Invoke( this, viewport );
		//	}

		//	protected virtual void OnUpdateEnd( Viewport viewport ) { }
		//	public delegate void UpdateEndDelegate( WorkareaModeClass sender, Viewport viewport );
		//	public event UpdateEndDelegate UpdateEnd;
		//	internal void PerformUpdateEnd( Viewport viewport )
		//	{
		//		OnUpdateEnd( viewport );
		//		UpdateEnd?.Invoke( this, viewport );
		//	}

		//	protected virtual void OnViewportUpdateGetCameraSettings( ref Component_Camera camera ) { }
		//	public delegate void ViewportUpdateGetCameraSettingsDelegate( WorkareaModeClass sender, ref Component_Camera camera );
		//	public event ViewportUpdateGetCameraSettingsDelegate ViewportUpdateGetCameraSettings;
		//	internal void PerformViewportUpdateGetCameraSettings( ref Component_Camera camera )
		//	{
		//		OnViewportUpdateGetCameraSettings( ref camera );
		//		ViewportUpdateGetCameraSettings?.Invoke( this, ref camera );
		//	}

		//	//!!!!надо ли ref bool handled
		//	protected virtual void OnEditorActionGetState( EditorAction.GetStateContext context ) { }
		//	public delegate void EditorActionGetStateDelegate( WorkareaModeClass sender, EditorAction.GetStateContext context );
		//	public event EditorActionGetStateDelegate EditorActionGetState;
		//	internal void PerformEditorActionGetState( EditorAction.GetStateContext context )
		//	{
		//		OnEditorActionGetState( context );
		//		EditorActionGetState?.Invoke( this, context );
		//	}

		//	//!!!!надо ли ref bool handled
		//	protected virtual void OnEditorActionClick( EditorAction.ClickContext context ) { }
		//	public delegate void EditorActionClickDelegate( WorkareaModeClass sender, EditorAction.ClickContext context );
		//	public event EditorActionClickDelegate EditorActionClick;
		//	internal void PerformEditorActionClick( EditorAction.ClickContext context )
		//	{
		//		OnEditorActionClick( context );
		//		EditorActionClick?.Invoke( this, context );
		//	}
		//}

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

		protected virtual void OnViewportUpdateGetObjectInSceneRenderingContext( ref Component_ObjectInSpace.RenderingContext context )
		{
			owner.PerformBaseViewportUpdateGetObjectInSceneRenderingContext( ref context );
		}
		internal void PerformViewportUpdateGetObjectInSceneRenderingContext( ref Component_ObjectInSpace.RenderingContext context )
		{
			OnViewportUpdateGetObjectInSceneRenderingContext( ref context );
		}

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

		protected virtual void OnGetTextInfoRightBottomCorner( List<string> lines )
		{
			owner.PerformBaseGetTextInfoRightBottomCorner( lines );
		}
		internal void PerformOnGetTextInfoRightBottomCorner( List<string> lines )
		{
			OnGetTextInfoRightBottomCorner( lines );
		}

		public void AddScreenMessage( string text, ColorValue color )
		{
			owner.AddScreenMessage( text, color );
		}

		public void AddScreenMessage( string text )
		{
			owner.AddScreenMessage( text );
		}

		////public static float SoundVolume
		////{
		////	get { return soundVolume; }
		////	set
		////	{
		////		MathFunctions.Clamp( ref value, 0, 1 );

		////		soundVolume = value;

		////		if( EngineApp.Instance.DefaultSoundChannelGroup != null )
		////			EngineApp.Instance.DefaultSoundChannelGroup.Volume = soundVolume;
		////	}
		////}

		public Component_Scene Scene
		{
			get { return owner.Scene; }
			set { owner.Scene = value; }
		}

		public bool SceneNeedDispose
		{
			get { return owner.SceneNeedDispose; }
			set { owner.SceneNeedDispose = value; }
		}

		public Component_Scene CreateScene( bool enable )
		{
			return owner.CreateScene( enable );
		}

		public void DestroyScene()
		{
			owner.DestroyScene();
		}

		public bool CameraRotating
		{
			get { return owner.CameraRotating; }
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

		public void AddTextWithShadow( Component_Font font, double fontSize, string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			owner.AddTextWithShadow( font, fontSize, text, position, horizontalAlign, verticalAlign, color );
		}

		public void AddTextWithShadow( string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			owner.AddTextWithShadow( text, position, horizontalAlign, verticalAlign, color );
		}

		public void AddTextLinesWithShadow( Component_Font font, double fontSize, IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			owner.AddTextLinesWithShadow( font, fontSize, lines, rectangle, horizontalAlign, verticalAlign, color );
		}

		public void AddTextLinesWithShadow( IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			owner.AddTextLinesWithShadow( lines, rectangle, horizontalAlign, verticalAlign, color );
		}

		public int AddTextWordWrapWithShadow( Component_Font font, double fontSize, string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			return owner.AddTextWordWrapWithShadow( font, fontSize, text, rectangle, horizontalAlign, verticalAlign, color );
		}

		public int AddTextWordWrapWithShadow( string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			return owner.AddTextWordWrapWithShadow( text, rectangle, horizontalAlign, verticalAlign, color );
		}

		public string WorkareaModeName
		{
			get { return owner.WorkareaModeName; }
		}

		//!!!!

		//public WorkareaModeClass WorkareaMode
		//{
		//	get { return workareaMode; }
		//}

		//public virtual void WorkareaModeSet( string name, WorkareaModeClass instance = null )
		//{
		//	workareaMode?.PerformDestroy();

		//	workareaModeName = name;
		//	workareaMode = instance;
		//}

		public bool AllowCameraControl
		{
			get { return owner.AllowCameraControl; }
		}

		public bool AllowSelectObjects
		{
			get { return owner.AllowSelectObjects; }
		}

		public bool DisplaySelectedObjects
		{
			get { return owner.DisplaySelectedObjects; }
		}

		//!!!!

		//public override void EditorActionGetState( EditorAction.GetStateContext context )
		//{
		//	base.EditorActionGetState( context );

		//	objectCreationMode?.PerformEditorActionGetState( context );
		//	workareaMode?.PerformEditorActionGetState( context );
		//}

		//public override void EditorActionClick( EditorAction.ClickContext context )
		//{
		//	base.EditorActionClick( context );

		//	objectCreationMode?.PerformEditorActionClick( context );
		//	workareaMode?.PerformEditorActionClick( context );
		//}

		//public ObjectCreationMode ObjectCreationMode
		//{
		//	get { return objectCreationMode; }
		//}

		//public virtual void ObjectCreationModeSet( ObjectCreationMode mode )
		//{
		//	objectCreationMode?.PerformDestroy();

		//	objectCreationMode = mode;
		//}

	}
}
