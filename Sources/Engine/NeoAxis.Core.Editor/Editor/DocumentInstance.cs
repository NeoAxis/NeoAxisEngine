//#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public class DocumentInstance : IDocumentInstance
	{
#if !DEPLOY
		string realFileName;
		Resource.Instance loadedResource;
		string specialMode;
		bool modified;
		UndoSystem undoSystem;
		bool allowUndoRedo = true;
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

		public string Name
		{
			get { return PathUtility.GetFileName( RealFileName ); }
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

		public Component ResultComponent
		{
			get { return loadedResource?.ResultComponent; }
		}

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

		public bool AllowUndoRedo
		{
			get { return allowUndoRedo; }
			set { allowUndoRedo = value; }
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
				string realPathWithoutAddFileExtension;
				string realPath;
				if( !string.IsNullOrEmpty( saveAsFileName ) )
				{
					realPathWithoutAddFileExtension = saveAsFileName;
					realPath = saveAsFileName;
				}
				else
				{
					realPathWithoutAddFileExtension = VirtualPathUtility.GetRealPathByVirtual( LoadedResource.Owner.Name );
					realPath = realPathWithoutAddFileExtension + LoadedResource.Owner.GetSaveAddFileExtension();
					//string name = LoadedResource.Owner.Name + LoadedResource.Owner.GetSaveAddFileExtension();
					//realPath = VirtualPathUtility.GetRealPathByVirtual( name );
				}

				//!!!!new

				var formWorkspaceController = EditorForm.instance.WorkspaceController;
				UpdateEditorDocumentConfiguration( formWorkspaceController );

				string error;
				if( !ComponentUtility.SaveComponentToFile( LoadedResource.ResultComponent, realPath, null, out error ) )
				{
					//!!!!
					Log.Error( error );
					return false;
				}

				PreviewImagesManager.AddResourceToProcess( realPathWithoutAddFileExtension );

				return true;
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

			ScreenNotifications2.Show( EditorLocalization2.Translate( "General", "The document was saved successfully." ) );

			if( setModifiedFlag )
				Modified = false;

			if( SpecialMode == "ProjectSettingsUserMode" )
			{
				EditorForm.Instance?.NeedRecreateRibbonButtons();
				EditorForm.Instance?.NeedRecreateQATButtons();
			}

			return true;
		}

		public virtual void EditorActionGetState( EditorActionGetStateContext context )
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
				context.Enabled = AllowUndoRedo && undoSystem != null && undoSystem.GetTopUndoAction() != null;
				break;

			case "Redo":
				context.Enabled = AllowUndoRedo && undoSystem != null && undoSystem.GetTopRedoAction() != null;
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

		public virtual void EditorActionClick( EditorActionClickContext context )
		{
			switch( context.Action.Name )
			{
			case "Save":
				Save( null );
				break;

			case "Save As":
				{
					if( !EditorUtility2.ShowSaveFileDialog( Path.GetDirectoryName( RealFileName ), RealFileName, "All files (*.*)|*.*", out var saveAsFileName ) )
						return;

					if( string.Compare( RealFileName, saveAsFileName, true ) == 0 )
						Save();
					else
					{
						Save( saveAsFileName, false );
						EditorAPI2.OpenFileAsDocument( saveAsFileName, true, true );
					}
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
						if( !EditorAPI2.SaveDocuments() )
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
				{
					EditorAPI2.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { RealFileName } );
					EditorAPI2.SelectDockWindow( EditorAPI2.FindWindow<ResourcesWindow>() );
				}
				break;
			}
		}

		public virtual void EditorActionClick2( EditorActionClickContext context )
		{
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
			}
		}

		public delegate void ShowTitleContextMenuDelegate( DocumentInstance document, IDocumentWindow caller, KryptonContextMenuItems items );
		public static event ShowTitleContextMenuDelegate ShowTitleContextMenu;

		internal protected virtual void OnShowTitleContextMenu( IDocumentWindow caller, KryptonContextMenuItems items )
		{
			ShowTitleContextMenu?.Invoke( this, caller, items );

			if( !string.IsNullOrEmpty( RealFileName ) )
			{
				//var documentWindow = caller as DocumentWindow;
				//var workspaceWindow = caller as WorkspaceWindow;
				//bool isFirstLevel = ( documentWindow != null && !documentWindow.IsWindowInWorkspace ) || workspaceWindow != null;

				//if( isFirstLevel )
				{
					items.Items.Add( new KryptonContextMenuItem( EditorLocalization2.Translate( "General", "Find in Resources window" ), ( s, e ) =>
					{
						EditorAPI2.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { RealFileName } );
						EditorAPI2.SelectDockWindow( EditorAPI2.FindWindow<ResourcesWindow>() );
					} ) );
				}

				//if( isFirstLevel )
				{
					var item = new KryptonContextMenuItem( EditorLocalization2.Translate( "General", "Reload" ), ( s, e ) =>
					{
						var document = (DocumentInstance)caller.Document;

						//ask to save document before locking the editor form
						if( document.Modified )
						{
							var text = EditorLocalization2.Translate( "General", "Save changes to the following files?" ) + "\n";
							text += "\n" + document.Name;
							var result = EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNoCancel );

							switch( result )
							{
							case EDialogResult.Cancel:
								return;

							case EDialogResult.Yes:
								document.Save();
								break;

							case EDialogResult.No:
								break;
							}
						}

						KryptonWinFormsUtility.EditorFormStartTemporaryLockUpdate();

						//!!!!restore order of documents

						EditorAPI2.CloseDocument( document, false );
						if( document.Destroyed )
							EditorAPI2.OpenFileAsDocument( document.RealFileName, true, true, specialMode: document.SpecialMode );

					} );

					item.Enabled = caller.Document != null;
					items.Items.Add( item );
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

					var component2 = rootComponent as IEditorUpdateWhenDocumentModified;
					if( component2 != null )
						component2.EditorUpdateWhenDocumentModified();
					foreach( var component in rootComponent.GetComponents<IEditorUpdateWhenDocumentModified>( checkChildren: true ) )
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
		//				foreach( var component in rootComponent.GetComponents<IEditorUpdateWhenDocumentModified>( false, true, true ) )
		//					component.EditorUpdateWhenDocumentModified();
		//			}

		//			callEditorUpdateWhenDocumentModified_LastUpdateTime = EngineApp.GetSystemTime();
		//			callEditorUpdateWhenDocumentModified_NeedUpdate = false;
		//		}
		//	}
		//}
#endif
	}
}

//#endif