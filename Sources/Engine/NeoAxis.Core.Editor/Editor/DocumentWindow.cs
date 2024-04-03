#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a docking window of the document.
	/// </summary>
	public partial class DocumentWindow : DockWindow, IDocumentWindow
	{
		DocumentInstance document;
		object objectOfWindow;
		bool openAsSettings;
		Dictionary<string, object> windowTypeSpecificOptions = new Dictionary<string, object>();

		bool isWindowInWorkspace;

		object[] selectedObjects = new object[ 0 ];
		ESet<object> selectedObjectsSet = new ESet<object>();

		/////////////////////////////////////////

		[Browsable( false )]
		public DocumentInstance Document2
		{
			get { return document; }
		}

		[Browsable( false )]
		public IDocumentInstance Document
		{
			get { return document; }
		}

		[Browsable( false )]
		public object ObjectOfWindow
		{
			get { return objectOfWindow; }
		}

		[Browsable( false )]
		public bool OpenAsSettings // now used only for ObjectSettingsWindow
		{
			get { return openAsSettings; }
		}

		[Browsable( false )]
		public Dictionary<string, object> WindowTypeSpecificOptions
		{
			get { return windowTypeSpecificOptions; }
		}

		public bool IsDocumentSaved()
		{
			return document == null || !document.Modified;
		}

		[Browsable( false )]
		public object[] SelectedObjects
		{
			get { return selectedObjects; }
		}

		[Browsable( false )]
		public ESet<object> SelectedObjectsSet
		{
			get { return selectedObjectsSet; }
		}

		/// <summary>
		/// Main/root window in workspace.
		/// Only document window can be main. not SettingsWindow.
		/// </summary>
		[Browsable( false )]
		internal bool IsMainWindowInWorkspace
		{
			get
			{
				if( openAsSettings )
					return false;

				if( ObjectOfWindow != null )
					return ObjectOfWindow == document.ResultObject;
				else
					return true; // TextEditorDocumentWindow for example ???
			}
		}

		[Browsable( false )]
		public bool IsWindowInWorkspace
		{
			get { return isWindowInWorkspace; }
			set
			{
				if( isWindowInWorkspace == value )
					return;
				isWindowInWorkspace = value;

				UpdateWindowTitle();
			}
		}

		public DocumentWindow()
		{
			InitializeComponent();
		}

		public virtual void InitDocumentWindow( DocumentInstance document, object objectOfWindow, bool openAsSettings, Dictionary<string, object> windowTypeSpecificOptions )
		{
			this.document = document;
			this.objectOfWindow = objectOfWindow;
			this.openAsSettings = openAsSettings;
			if( windowTypeSpecificOptions != null )
				this.windowTypeSpecificOptions = windowTypeSpecificOptions;
		}

		protected override string GetResultWindowTitle()
		{
			if( document != null )
			{
				var title = "";

				if( IsMainWindowInWorkspace && !string.IsNullOrEmpty( document.RealFileName ) )
					title = Path.GetFileName( document.RealFileName );
				else if( ObjectOfWindow != null )
					title = ObjectOfWindow.ToString();

				if( !string.IsNullOrEmpty( title ) )
				{
					if( IsWindowInWorkspace )
					{
						if( ObjectOfWindowIsDeleted )
							title += " (Deleted)";
						else if( IsMainWindowInWorkspace )
							title = "'Root object'";
					}
					else
					{
						if( document != null && document.Modified )
							title += "*";
					}

					return title;
				}
			}

			return base.GetResultWindowTitle();
		}

		private void DocumentWindow_Load( object sender, EventArgs e )
		{
			if( IsDesignerHosted )
				return;

			timer1.Start();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if( IsDesignerHosted )
				return;

			//close document
			if( IsMainWindowInWorkspace )
				document.Destroy();
		}

		internal protected override void OnShowTitleContextMenu( KryptonContextMenuItems items )
		{
			( (DocumentInstance)Document2 ).OnShowTitleContextMenu( this, items );
		}

		public bool SaveDocument()
		{
			return document.Save();
		}

		public bool IsObjectSelected( object obj )
		{
			return SelectedObjectsSet.Contains( obj );
		}

		static bool Equal( ICollection<object> v1, ICollection<object> v2 )
		{
			if( v1.Count != v2.Count )
				return false;

			var e1 = v1.GetEnumerator();
			var e2 = v2.GetEnumerator();
			while( e1.MoveNext() )
			{
				e2.MoveNext();
				if( !ReferenceEquals( e1.Current, e2.Current ) )
					return false;
			}
			return true;
		}

		//public delegate void SelectedObjectsChangedDelegate( DocumentWindow sender, object[] oldSelectedObjects );
		public event DocumentWindowSelectedObjectsChangedDelegate SelectedObjectsChanged;

		public void SelectObjects( ICollection<object> objects, bool updateForeachDocumentWindowContainers = true, bool updateSettingsWindowSelectObjects = true, bool forceUpdate = false )
		{
			if( objects == null )
				objects = new object[ 0 ];

			if( !Equal( objects, SelectedObjects ) || forceUpdate )
			{
				var oldSelectedObjects = SelectedObjects;

				selectedObjects = new object[ objects.Count ];
				objects.CopyTo( selectedObjects, 0 );
				selectedObjectsSet = new ESet<object>( selectedObjects );
				//SettingsWindow.Instance._SelectObjects( this, objects );

				//update ForeachDocumentWindowContainers
				if( updateForeachDocumentWindowContainers )
				{
					var windows = new List<ForeachDocumentWindowContainer>();

					foreach( var window in EditorForm.Instance.WorkspaceController.GetDockWindows() )
					{
						var window2 = window as ForeachDocumentWindowContainer;
						if( window2 != null )
							windows.Add( window2 );
					}

					foreach( var window in windows )
						window.OnDocumentWindowSelectedObjectsChangedByUser( this );
				}

				//update Settings Window
				if( updateSettingsWindowSelectObjects )
					SettingsWindowSelectObjects();

				SelectedObjectsChanged?.Invoke( this, oldSelectedObjects );
			}
		}

		protected virtual void OnTimer10MsTick()
		{
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			//!!!!так? еще в DocumentWindowWithViewport есть
			{
				var component = ObjectOfWindow as Component;
				if( component != null && component.HierarchyController != null )
					component.HierarchyController.ProcessDelayedOperations();
			}

			OnTimer10MsTick();
		}

		protected virtual bool CanUpdateSettingsWindowsSelectedObjects()
		{
			return true;
		}

		protected virtual object OnGetSelectObjectWhenNoSelectedObjects()
		{
			return ObjectOfWindow;
		}

		public void SettingsWindowSelectObjects()
		{
			if( CanUpdateSettingsWindowsSelectedObjects() )
			{
				var objects = SelectedObjects;

				if( objects.Length == 0 )
				{
					var obj = OnGetSelectObjectWhenNoSelectedObjects();
					if( obj != null )
						objects = new object[] { obj };
				}

				if( objects.Length == 0 && ObjectOfWindow != null )
					objects = new object[] { ObjectOfWindow };

				SettingsWindow.Instance?.SelectObjects( this, objects );
			}
		}

		public virtual void EditorActionGetState( EditorActionGetStateContext context )
		{
			switch( context.Action.Name )
			{
			case "Cut":
				if( CanCut() )
					context.Enabled = true;
				break;

			case "Copy":
				if( CanCopy() )
					context.Enabled = true;
				break;

			case "Paste":
				if( CanPaste( out _ ) )
					context.Enabled = true;
				break;

			case "Duplicate":
				if( CanCloneObjects( out _ ) )
					context.Enabled = true;
				break;

			case "Delete":
				if( CanDeleteObjects( out _ ) )
					context.Enabled = true;
				break;

			case "Rename":
				if( CanRename( out _ ) )
					context.Enabled = true;
				break;
			}
		}

		public virtual void EditorActionClick( EditorActionClickContext context )
		{
			switch( context.Action.Name )
			{
			case "Cut":
				Cut();
				break;

			case "Copy":
				Copy();
				break;

			case "Paste":
				Paste();
				break;

			case "Duplicate":
				TryCloneObjects();
				break;

			case "Delete":
				TryDeleteObjects();
				break;

			case "Rename":
				TryRename();
				break;
			}
		}

		public virtual void EditorActionClick2( EditorActionClickContext context )
		{
		}

		public override ObjectsInFocus GetObjectsInFocus()
		{
			if( SelectedObjects.Length != 0 )
				return new ObjectsInFocus( this, SelectedObjects );
			return base.GetObjectsInFocus();
		}

		[Browsable( false )]
		public virtual bool ObjectOfWindowIsDeleted
		{
			get
			{
				var component = ObjectOfWindow as Component;
				if( component != null && document != null && ObjectOfWindow != document.ResultComponent &&
					document.ResultComponent != null && !component.GetAllParents().Contains( document.ResultComponent ) )
				{
					return true;
				}

				return false;
			}
		}

		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( keyData == ( Keys.Control | Keys.F4 ) )
			{
				//close root window of the document when second level windows are exists
				if( IsMainWindowInWorkspace )
				{
					var workspaceWindow = EditorForm.Instance.WorkspaceController.FindWorkspaceWindow( this );
					if( workspaceWindow != null )
					{
						workspaceWindow.Close();
						return true;
					}
				}

				Close();
				return true;
			}

			return base.ProcessCmdKey( ref msg, keyData );
		}

		private void DocumentWindow_KeyDown( object sender, KeyEventArgs e )
		{
			if( GetType() == typeof( DocumentWindow ) && EditorAPI2.ProcessShortcuts( e.KeyCode, true ) )
			{
				e.Handled = true;
				return;
			}
		}

		string TranslateContextMenu( string text )
		{
			return EditorContextMenuWinForms.Translate( text );
		}

		void ShowContextMenu()
		{
			var items = new List<KryptonContextMenuItemBase>();

			Component oneSelectedComponent = ObjectOfWindow as Component;

			//Editor
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Editor" ), EditorResourcesCache.Edit, delegate ( object s, EventArgs e2 )
				{
					EditorAPI2.OpenDocumentWindowForObject( (DocumentInstance)Document2, oneSelectedComponent );
				} );
				item.Enabled = oneSelectedComponent != null && EditorAPI2.IsDocumentObjectSupport( oneSelectedComponent );
				items.Add( item );
			}

			//Settings
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Settings" ), EditorResourcesCache.Settings, delegate ( object s, EventArgs e2 )
				{
					EditorAPI2.SelectDockWindow( EditorAPI2.FindWindow<SettingsWindow>() );
				} );
				items.Add( item );
			}

			//Separate Settings
			if( EditorUtility.AllowSeparateSettings )
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Separate Settings" ), EditorResourcesCache.Settings, delegate ( object s, EventArgs e2 )
				{
					var obj = oneSelectedComponent ?? ObjectOfWindow;
					bool canUseAlreadyOpened = !ModifierKeys.HasFlag( Keys.Shift );
					EditorAPI2.ShowObjectSettingsWindow( (DocumentInstance)Document2, obj, canUseAlreadyOpened );
				} );
				item.Enabled = oneSelectedComponent != null || SelectedObjects.Length == 0;
				items.Add( item );
			}

			items.Add( new KryptonContextMenuSeparator() );

			//New object
			{
				EditorContextMenuWinForms.AddNewObjectItem( items, CanNewObject( out _ ), delegate ( Metadata.TypeInfo type, bool assetsFolderOnly )
				{
					TryNewObject( type );
				} );
			}

			//separator
			items.Add( new KryptonContextMenuSeparator() );

			//Cut
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Cut" ), EditorResourcesCache.Cut,
					delegate ( object s, EventArgs e2 )
					{
						//Cut();
					} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Cut" );
				item.Enabled = false;// CanCut();
				items.Add( item );
			}

			//Copy
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Copy" ), EditorResourcesCache.Copy,
					delegate ( object s, EventArgs e2 )
					{
						Copy();
					} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Copy" );
				item.Enabled = CanCopy();
				items.Add( item );
			}

			//Paste
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Paste" ), EditorResourcesCache.Paste,
					delegate ( object s, EventArgs e2 )
					{
						Paste();
					} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Paste" );
				item.Enabled = CanPaste( out _ );
				items.Add( item );
			}

			//Clone
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Duplicate" ), EditorResourcesCache.Clone,
					delegate ( object s, EventArgs e2 )
					{
						//TryCloneObjects();
					} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Duplicate" );
				item.Enabled = false;// CanCloneObjects( out List<Component> dummy );
				items.Add( item );
			}

			//separator
			items.Add( new KryptonContextMenuSeparator() );

			//Delete
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Delete" ), EditorResourcesCache.Delete,
					delegate ( object s, EventArgs e2 )
					{
						//TryDeleteObjects();
					} );
				item.Enabled = false;// CanDeleteObjects( out List<Component> dummy );
				items.Add( item );
			}

			//Rename
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Rename" ), null, delegate ( object s, EventArgs e2 )
				{
					EditorUtility2.ShowRenameComponentDialog( oneSelectedComponent );
				} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Rename" );
				//!!!!!
				item.Enabled = oneSelectedComponent != null;
				items.Add( item );
			}

			EditorContextMenuWinForms.AddActionsToMenu( EditorContextMenuWinForms.MenuTypeEnum.Document, items );//, this );

			EditorContextMenuWinForms.Show( items, this );
		}

		bool CanNewObject( out List<Component> parentsForNewObjects )
		{
			parentsForNewObjects = new List<Component>();

			var component = ObjectOfWindow as Component;
			if( component != null )
				parentsForNewObjects.Add( component );

			return true;
		}

		void TryNewObject( Metadata.TypeInfo lockType )
		{
			if( !CanNewObject( out List<Component> parentsForNewObjects ) )
				return;

			var data = new NewObjectWindow.CreationDataClass();
			data.initDocumentWindow = this;
			data.initParentObjects = new List<object>();
			data.initParentObjects.AddRange( parentsForNewObjects );

			//!!!!
			//уникальное имя

			data.initLockType = lockType;
			data.initSupportAutoCreateAndClose = lockType != null;

			//!!!!бывает что создавать другой объект? например для меша создавать mesh in space. где еще так

			//!!!!выделить после создания

			EditorAPI2.OpenNewObjectWindow( data );
		}

		private void DocumentWindow_MouseUp( object sender, MouseEventArgs e )
		{
			//context menu
			if( GetType() == typeof( DocumentWindow ) && e.Button == MouseButtons.Right )
				ShowContextMenu();
		}

		public virtual bool CanCut()
		{
			if( SelectedObjects.Length != 0 )
				return SelectedObjects.All( obj => obj is Component && ( (Component)obj ).Parent != null );
			return false;
		}

		public virtual bool CutCopy( bool cut )
		{
			if( !CanCut() )
				return false;
			var data = new ObjectCutCopyPasteData( this, cut, (object[])SelectedObjects.Clone() );
			ClipboardManager.CopyToClipboard( data );
			return true;
		}

		public bool Cut()
		{
			return CutCopy( true );
		}

		public virtual bool CanCopy()
		{
			if( SelectedObjects.Length != 0 )
				return SelectedObjects.All( obj => obj is Component );
			return false;
		}

		public bool Copy()
		{
			return CutCopy( false );
		}

		//bool CanCopy()
		//{
		//	return ObjectOfWindow as Component != null;
		//}

		//void Copy()
		//{
		//	if( CanCopy() )
		//	{
		//		var data = new ObjectCutCopyPasteData( this, false, new object[] { ObjectOfWindow } );
		//		ClipboardManager.CopyToClipboard( data );
		//	}
		//}

		public virtual bool CanPaste( out Component destinationParent )
		{
			if( ClipboardManager.CheckAvailableInClipboard<ObjectCutCopyPasteData>() )
			{
				if( SelectedObjects.Length == 0 )
				{
					var c = ObjectOfWindow as Component;
					if( c != null )
					{
						destinationParent = c;
						return true;
					}
				}
				else if( SelectedObjects.Length == 1 )
				{
					var c = SelectedObjects[ 0 ] as Component;
					if( c != null )
					{
						destinationParent = c;
						return true;
					}
				}
			}

			destinationParent = null;
			return false;
		}

		//bool CanPaste( out Component destinationParent )
		//{
		//	if( ClipboardManager.CheckAvailableInClipboard<ObjectCutCopyPasteData>() )
		//	{
		//		destinationParent = ObjectOfWindow as Component;
		//		if( destinationParent != null )
		//			return true;
		//	}

		//	destinationParent = null;
		//	return false;
		//}

		public virtual bool Paste()
		{
			if( !CanPaste( out var destinationParent ) )
				return false;

			var data = ClipboardManager.GetFromClipboard<ObjectCutCopyPasteData>();
			if( data != null )
			{
				var components = new List<Component>();
				foreach( var obj in data.objects )
				{
					var c = obj as Component;
					if( c != null )
						components.Add( c );
				}

				//create new objects

				var newObjects = new List<Component>();
				Vector3 addToPosition = Vector3.Zero;

				for( int n = 0; n < components.Count; n++ )
				{
					var c = components[ n ];

					var cloned = c.Clone();
					if( destinationParent.GetComponent( c.Name ) == null )
						cloned.Name = c.Name;
					else
						cloned.Name = destinationParent.Components.GetUniqueName( c.Name, true, 2 );
					destinationParent.AddComponent( cloned );

					newObjects.Add( cloned );
				}

				if( data.cut )
				{
					//cut
					if( data.documentWindow.Document2 != Document2 )
					{
						//another document
						{
							var action = new UndoActionComponentCreateDelete( data.documentWindow.Document2, components, false );
							data.documentWindow.Document2.UndoSystem.CommitAction( action );
							data.documentWindow.Document2.Modified = true;
						}
						{
							var action = new UndoActionComponentCreateDelete( Document2, newObjects, true );
							Document2.UndoSystem.CommitAction( action );
							Document2.Modified = true;
						}
					}
					else
					{
						//same document
						var multiAction = new UndoMultiAction();
						multiAction.AddAction( new UndoActionComponentCreateDelete( Document2, components, false ) );
						multiAction.AddAction( new UndoActionComponentCreateDelete( Document2, newObjects, true ) );
						Document2.UndoSystem.CommitAction( multiAction );
						Document2.Modified = true;
					}
				}
				else
				{
					//copy
					var action = new UndoActionComponentCreateDelete( Document2, newObjects, true );
					Document2.UndoSystem.CommitAction( action );
					Document2.Modified = true;
				}
			}

			return true;
		}

		public virtual bool CanCloneObjects( out List<Component> resultObjectsToClone )
		{
			resultObjectsToClone = new List<Component>( SelectedObjects.Length );

			foreach( var obj in SelectedObjects )
			{
				var component = obj as Component;
				if( component != null && component.Parent != null )
					resultObjectsToClone.Add( component );
			}

			//remove children which inside selected parents
			resultObjectsToClone = ComponentUtility.GetComponentsWithoutChildren( resultObjectsToClone );

			if( resultObjectsToClone.Count == 0 )
				return false;

			return true;
		}

		public static void AddClonedSelectableChildrenToList( List<Component> list, Component component )
		{
			foreach( var childComponent in component.GetComponents() )
			{
				if( childComponent.DisplayInEditor && childComponent.TypeSettingsIsPublic() && EditorUtility.PerformComponentDisplayInEditorFilter( childComponent ) )
				{
					//CurveInSpace specific
					if( childComponent is CurveInSpacePoint )
					{
						list.Add( childComponent );

						AddClonedSelectableChildrenToList( list, childComponent );
					}
				}
			}
		}

		public virtual void TryCloneObjects()
		{
			if( !CanCloneObjects( out var objectsToClone ) )
				return;

			var newObjects = new List<Component>();
			foreach( var obj in objectsToClone )
			{
				var newObject = EditorUtility.CloneComponent( obj );
				newObjects.Add( newObject );
				AddClonedSelectableChildrenToList( newObjects, newObject );
			}

			//select objects
			{
				var selectObjects = new List<object>();
				//!!!!все выделить?
				selectObjects.AddRange( newObjects );

				SelectObjects( selectObjects );
			}

			if( newObjects.Count == 0 )
				return;

			//add to undo with deletion
			var action = new UndoActionComponentCreateDelete( Document2, newObjects, true );
			Document2.UndoSystem.CommitAction( action );
			Document2.Modified = true;

			//add screen message
			EditorUtility.ShowScreenNotificationObjectsCloned( newObjects.Count );
		}

		public virtual bool CanDeleteObjects( out List<object> resultObjectsToDelete )
		{
			var resultObjectsToDelete2 = new List<Component>();

			foreach( var obj in SelectedObjects )
			{
				var component = obj as Component;
				if( component != null && component.Parent != null )
					resultObjectsToDelete2.Add( component );
			}

			//remove children which inside selected parents
			resultObjectsToDelete = ComponentUtility.GetComponentsWithoutChildren( resultObjectsToDelete2 ).Cast<object>().ToList();

			if( resultObjectsToDelete.Count == 0 )
				return false;

			return true;
		}

		public virtual bool TryDeleteObjects()
		{
			if( !CanDeleteObjects( out var objectsToDelete ) )
				return false;

			string text;
			if( objectsToDelete.Count == 1 )
			{
				string template = EditorLocalization2.Translate( "DocumentWindow", "Are you sure you want to delete \'{0}\'?" );
				var name = objectsToDelete[ 0 ].ToString();
				text = string.Format( template, name );
			}
			else
			{
				string template = EditorLocalization2.Translate( "DocumentWindow", "Are you sure you want to delete selected objects?" );
				text = string.Format( template, objectsToDelete.Count );
			}

			if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.No )
				return false;

			//!!!!может сцену выбрать? везде так
			//clear selected objects
			SelectObjects( null );

			//add to undo with deletion
			var action = new UndoActionComponentCreateDelete( Document2, objectsToDelete.Cast<Component>().ToArray(), false );
			Document2.UndoSystem.CommitAction( action );
			Document2.Modified = true;

			return true;
		}

		public virtual bool CanRename( out Component component )
		{
			component = null;

			//!!!!multiselection
			if( SelectedObjects.Length == 1 )
				component = SelectedObjects[ 0 ] as Component;

			return component != null;
		}

		public virtual void TryRename()
		{
			if( CanRename( out var component ) )
				EditorUtility2.ShowRenameComponentDialog( component );
		}
	}
}
#endif