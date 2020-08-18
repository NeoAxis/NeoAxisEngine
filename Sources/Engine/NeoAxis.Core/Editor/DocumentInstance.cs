// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;
#if !DEPLOY
using Microsoft.WindowsAPICodePack.Dialogs;
#endif

namespace NeoAxis.Editor
{
	public class DocumentInstance
	{
		string realFileName;
		Resource.Instance loadedResource;
		string specialMode;
		bool modified;
		UndoSystem undoSystem;
		bool destroyed;

		List<double> editorUpdateWhenDocumentModified_NeedUpdate = new List<double>();

		/////////////////////////////////////////

		public DocumentInstance( string realFileName, Resource.Instance loadedResource, string specialMode )
		{
			this.realFileName = realFileName;
			this.loadedResource = loadedResource;
			this.specialMode = specialMode;

			//!!!!max level
			undoSystem = new UndoSystem( 32 );
			undoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;
		}

		public string RealFileName
		{
			get { return realFileName; }
		}

		//!!!!
		public string Name
		{
			get { return Path.GetFileName( RealFileName ); }
		}

		//public DocumentWindow ParentDocument
		//{
		//	get { return parentDocument; }
		//	set { parentDocument = value; }
		//}

		public Resource.Instance LoadedResource
		{
			get { return loadedResource; }
		}

		public string SpecialMode
		{
			get { return specialMode; }
		}

		//!!!!
		public Component ResultComponent
		{
			get { return loadedResource?.ResultComponent; }
		}

		//!!!!
		public object ResultObject
		{
			get { return loadedResource?.ResultObject; }
		}

		public bool IsEditorDocumentConfigurationExist
		{
			get { return ResultComponent?.EditorDocumentConfiguration != null; }
		}

		//юзается UndoSystem.ListOfActionsChanged т.к. отслеживается отмена операций
		//public delegate void ModifiedSetCalledDelegate( DocumentInstance document );//, bool newValue );
		//public event ModifiedSetCalledDelegate ModifiedSetCalled;

		public bool Modified
		{
			get { return modified; }
			set
			{
				modified = value;
				//ModifiedSetCalled?.Invoke( this );//, value );

				//if( modified == value )
				//	return;
				//modified = value;
			}
		}

		public UndoSystem UndoSystem
		{
			get { return undoSystem; }
		}

		public bool Destroyed
		{
			get { return destroyed; }
		}

		public void Destroy()
		{
			if( destroyed )
				return;
			destroyed = true;

			if( undoSystem != null )
				undoSystem.Dispose();

			EditorForm.instance?.Documents.Remove( this );

			loadedResource?.Dispose();
			loadedResource = null;
		}

		//public void AddScreenMesssage( string text )
		//{
		//	//!!!!

		//	//!!!!removed
		//	/*
		//				foreach( var window in DocumentWindows )
		//				{
		//					var window2 = window as DocumentWindowWithViewport;
		//					if( window2 != null )
		//						window2.AddScreenMessage( text );
		//				}
		//	*/
		//}

		public virtual bool CanSaveAs()
		{
			//!!!!
			////event
			//{
			//	bool handled = false;
			//	bool result = false;
			//	CanSaveEvent?.Invoke( this, ref handled, ref result );
			//	if( handled )
			//		return result;
			//}

			if( LoadedResource != null && LoadedResource.ResultComponent != null && LoadedResource.Owner.LoadFromFile )
			{
				if( !string.IsNullOrEmpty( LoadedResource.Owner.GetSaveAddFileExtension() ) )
					return false;
			}

			return true;
		}

		public delegate void SaveEventDelegate( DocumentInstance document, string saveAsFileName, ref bool handled, ref bool result );// ref string error );
		public event SaveEventDelegate SaveEvent;

		protected virtual bool OnSave( string saveAsFileName )
		{
			//event
			{
				bool handled = false;
				bool result = false;
				SaveEvent?.Invoke( this, saveAsFileName, ref handled, ref result );
				if( handled )
					return result;
			}

			//!!!!?
			if( LoadedResource != null && LoadedResource.ResultComponent != null && LoadedResource.Owner.LoadFromFile )
			{
				string realPath;
				if( !string.IsNullOrEmpty( saveAsFileName ) )
				{
					realPath = saveAsFileName;
				}
				else
				{
					string name = LoadedResource.Owner.Name + LoadedResource.Owner.GetSaveAddFileExtension();
					realPath = VirtualPathUtility.GetRealPathByVirtual( name );
				}

				//!!!!new

				var formWorkspaceController = (WorkspaceControllerForForm)EditorForm.instance.WorkspaceController;
				UpdateEditorDocumentConfiguration( formWorkspaceController );

				string error;
				if( ComponentUtility.SaveComponentToFile( LoadedResource.ResultComponent, realPath, null, out error ) )
				{
					return true;
				}
				else
				{
					//!!!!
					Log.Error( error );
					return false;
				}
			}
			else
			{
				//!!!!!
				Log.Warning( "impl" );
			}

			return false;
		}

		//!!!!out string error
		public bool Save( string saveAsFileName = null, bool setModifiedFlag = true )
		{
			if( !OnSave( saveAsFileName ) )
				return false;

			ScreenNotifications.Show( EditorLocalization.Translate( "General", "The document was saved successfully." ) );

			if( setModifiedFlag )
				Modified = false;

			////!!!!temp
			//EditorForm.checkRestartApplicationToApplyChangedNeedCheck = true;

			return true;
		}

		public virtual void EditorActionGetState( EditorAction.GetStateContext context )
		{
			switch( context.Action.Name )
			{
			case "Save":
				if( !string.IsNullOrEmpty( RealFileName ) )
					context.Enabled = true;
				break;

			case "Save As":
				if( !string.IsNullOrEmpty( RealFileName ) )
					context.Enabled = CanSaveAs();
				break;

			case "Undo":
				context.Enabled = undoSystem != null && undoSystem.GetTopUndoAction() != null;
				break;

			case "Redo":
				context.Enabled = undoSystem != null && undoSystem.GetTopRedoAction() != null;
				break;

			case "Play":
				{
					var component = LoadedResource?.ResultComponent;
					if( component != null && RunSimulation.CheckTypeSupportedByPlayer( component.BaseType ) )
						context.Enabled = true;
				}
				break;

			case "Find Resource":
				context.Enabled = !string.IsNullOrEmpty( RealFileName );
				break;
			}
		}

		public virtual void EditorActionClick( EditorAction.ClickContext context )
		{
			switch( context.Action.Name )
			{
			case "Save":
				Save( null );
				break;

			case "Save As":
				{
#if !DEPLOY
					var dialog = new CommonSaveFileDialog();
					dialog.InitialDirectory = Path.GetDirectoryName( RealFileName );
					dialog.DefaultFileName = Path.GetFileName( RealFileName );
					dialog.Filters.Add( new CommonFileDialogFilter( "All Files", ".*" ) );
					if( dialog.ShowDialog() != CommonFileDialogResult.Ok )
						return;

					var saveAsFileName = dialog.FileName;

					//if( File.Exists( saveAsFileName ) )
					//{
					//	var text = string.Format( EditorLocalization.Translate( "General", "A file with the name \'{0}\' already exists. Overwrite?" ), saveAsFileName );
					//	if( EditorMessageBox.ShowQuestion( text, MessageBoxButtons.OKCancel ) != DialogResult.OK )
					//		return;
					//}

					if( string.Compare( RealFileName, saveAsFileName, true ) == 0 )
						Save();
					else
					{
						Save( saveAsFileName, false );
						EditorAPI.OpenFileAsDocument( saveAsFileName, true, true );
					}
#endif
				}
				break;

			case "Undo":
				if( undoSystem != null )
				{
					if( undoSystem.DoUndo() )
						Modified = true;
				}
				break;

			case "Redo":
				if( undoSystem != null )
				{
					if( undoSystem.DoRedo() )
						Modified = true;
				}
				break;

			case "Play":
				{
					var component = LoadedResource?.ResultComponent;
					if( component != null && RunSimulation.CheckTypeSupportedByPlayer( component.BaseType ) )
					{
						if( !EditorAPI.SaveDocuments() )
							return;
						//if( Modified )
						//{
						//	if( !Save( null ) )
						//		return;
						//}

						//!!!!не только standalone
						var realFileName = VirtualPathUtility.GetRealPathByVirtual( LoadedResource.Owner.Name );
						RunSimulation.Run( realFileName, RunSimulation.RunMethod.Player );
					}
				}
				break;

			case "Find Resource":
				if( !string.IsNullOrEmpty( RealFileName ) )
					EditorAPI.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { RealFileName } );
				break;
			}
		}

		private void UpdateEditorDocumentConfiguration( WorkspaceControllerForForm formWorkspaceController )
		{
			var component = this.ResultComponent;
			var workspaceWindow = formWorkspaceController.FindWorkspaceWindow( this );

			var workspaceController = workspaceWindow?.WorkspaceController;

			bool needSave = workspaceController != null;
			bool needClear = workspaceController == null && component.EditorDocumentConfiguration != null;
			if( needSave || needClear )
			{
				component.EditorDocumentConfiguration = workspaceController?.SaveLayoutToString();
				//!!!!temp. везде выключить
				//Log.Info( $"Layout for '{Name}' saved." );
			}
		}

		internal protected virtual void OnShowTitleContextMenu( IDocumentWindow caller, KryptonContextMenuItems items )
		{
			if( !string.IsNullOrEmpty( RealFileName ) )
			{
				//var documentWindow = caller as DocumentWindow;
				//var workspaceWindow = caller as WorkspaceWindow;
				//bool isFirstLevel = ( documentWindow != null && !documentWindow.IsWindowInWorkspace ) || workspaceWindow != null;

				//if( isFirstLevel )
				{
					items.Items.Add( new KryptonContextMenuItem( EditorLocalization.Translate( "General", "Find in Resources window" ), ( s, e ) =>
					{
						EditorAPI.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { RealFileName } );
					} ) );
				}

				//!!!!

				//!!!!только редактору сцен

				//if( caller is DocumentWindow )
				//{

				//	var item = new KryptonContextMenuItem( EditorLocalization.Translate( "General", "Use Camera" ), null );

				//	var items2 = new List<KryptonContextMenuItemBase>();

				//	items2.Add( new KryptonContextMenuItem( "Default", ( s, e ) =>
				//	{
				//		//EditorAPI.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { RealFileName } );
				//	} ) );

				//	items2.Add( new KryptonContextMenuItem( "Camera 1", ( s, e ) =>
				//	{
				//		//EditorAPI.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { RealFileName } );
				//	} ) );

				//	items2.Add( new KryptonContextMenuItem( "Camera 2", ( s, e ) =>
				//	{
				//		//EditorAPI.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { RealFileName } );
				//	} ) );

				//	item.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
				//	items.Items.Add( item );
				//}
			}
		}

		public void CommitUndoAction( UndoSystem.Action action, bool setModified = true )
		{
			UndoSystem?.CommitAction( action );
			if( setModified )
				Modified = true;
		}

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			if( !Destroyed )
			{
				//!!!!is not too much updates?
				EditorUpdateWhenDocumentModified_NeedUpdate( EngineApp.GetSystemTime() + 3.1 );
				EditorUpdateWhenDocumentModified_NeedUpdate( EngineApp.GetSystemTime() + 0.5 );
				EditorUpdateWhenDocumentModified_NeedUpdate( EngineApp.GetSystemTime() );
			}
		}

		//public void EditorUpdateWhenDocumentModified_SuspendUpdate( double suspendTime )
		//{
		//	xx;
		//	xx;//после окончания сделать обновление
		//}

		public void EditorUpdateWhenDocumentModified_NeedUpdate( double updateTime )//, bool clearBeforeThisTime )
		{
			//clear updates before this time
			again:;
			for( int n = 0; n < editorUpdateWhenDocumentModified_NeedUpdate.Count; n++ )
			{
				var time = editorUpdateWhenDocumentModified_NeedUpdate[ n ];
				if( time < updateTime )
				{
					editorUpdateWhenDocumentModified_NeedUpdate.RemoveAt( n );
					goto again;
				}
			}

			//add new
			editorUpdateWhenDocumentModified_NeedUpdate.Add( updateTime );
		}

		public void EditorUpdateWhenDocumentModified_Tick()
		{
			bool update = false;

			var currentTime = EngineApp.GetSystemTime();

			again:;
			for( int n = 0; n < editorUpdateWhenDocumentModified_NeedUpdate.Count; n++ )
			{
				var time = editorUpdateWhenDocumentModified_NeedUpdate[ n ];
				if( currentTime >= time )
				{
					editorUpdateWhenDocumentModified_NeedUpdate.RemoveAt( n );
					update = true;
					goto again;
				}
			}

			//update
			if( update )
			{
				if( loadedResource != null && loadedResource.ResultComponent != null )
				{
					var rootComponent = loadedResource.ResultComponent;

					var component2 = rootComponent as IComponent_EditorUpdateWhenDocumentModified;
					if( component2 != null )
						component2.EditorUpdateWhenDocumentModified();
					foreach( var component in rootComponent.GetComponents<IComponent_EditorUpdateWhenDocumentModified>( checkChildren: true ) )
						component.EditorUpdateWhenDocumentModified();
				}
			}
		}

		//public void CallEditorUpdateWhenDocumentModifiedUpdate( bool forceUpdate = false )
		//{
		//	if( callEditorUpdateWhenDocumentModified_NeedUpdate || forceUpdate )
		//	{
		//		var time = EngineApp.GetSystemTime();
		//		if( EngineApp.GetSystemTime() > callEditorUpdateWhenDocumentModified_LastUpdateTime + 3.0 || forceUpdate )
		//		{
		//			if( loadedResource != null && loadedResource.ResultComponent != null )
		//			{
		//				var rootComponent = loadedResource.ResultComponent;
		//				foreach( var component in rootComponent.GetComponents<IComponent_EditorUpdateWhenDocumentModified>( false, true, true ) )
		//					component.EditorUpdateWhenDocumentModified();
		//			}

		//			callEditorUpdateWhenDocumentModified_LastUpdateTime = EngineApp.GetSystemTime();
		//			callEditorUpdateWhenDocumentModified_NeedUpdate = false;
		//		}
		//	}
		//}
	}
}
