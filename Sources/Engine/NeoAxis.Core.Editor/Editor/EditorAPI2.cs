// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal;
using System.Linq;

namespace NeoAxis.Editor
{
	//!!!!name
	/// <summary>
	/// Provides an interface to work with the editor.
	/// </summary>
	public static class EditorAPI2
	{
		static Stack<bool> applicationDoEventsStack = new Stack<bool>();

		internal static bool needShowProjectSettings;

		//

		public static bool DarkTheme
		{
			get
			{
#if !DEPLOY
				return KryptonManager._globalPaletteMode == PaletteModeManager.NeoAxisBlack;
#else
				return true;
#endif
			}
		}

#if !DEPLOY
		static bool needRestartApplication;
		static bool needRestartApplication_ResetWindowsSettings;

		//

		/*public */
		static EditorForm EditorForm
		{
			get { return EditorForm.Instance; }
		}

		public static bool IsEditor
		{
			get { return EngineApp.IsEditor; }
		}

		public static DockWindow FindWindow( Type windowClass )
		{
			return EditorForm?.WorkspaceController.FindWindow( windowClass );
		}

		public static T FindWindow<T>() where T : DockWindow
		{
			return (T)FindWindow( typeof( T ) );
		}

		public static void ShowDockWindow( Type windowClass )
		{
			EditorForm?.WorkspaceController.ShowDockWindow( windowClass );
		}

		public static void ShowDockWindow<T>() where T : DockWindow
		{
			ShowDockWindow( typeof( T ) );
		}

		//выделяется отложенно, т.к. из файловой системы еще не пришел эвент о создании файла.
		public static void SelectFilesOrDirectoriesInMainResourcesWindow( string[] realPaths, bool expandNodes = false )
		{
			var window = FindWindow<ResourcesWindow>();
			if( window != null )//&& window.Visible )
			{
				window.ContentBrowser1?.NeedSelectFilesOrDirectories( realPaths, expandNodes );
				SelectDockWindow( window );
			}
		}

		//!!!!по сути не надо указывать documentWindow
		public static void SelectComponentsInMainObjectsWindow( DocumentWindow documentWindow, Component[] components )
		//public static void SelectComponentsInMainObjectsWindow( DocumentInstance document, Component[] components )
		{
			if( documentWindow == null || components.Length == 0 )
				return;

			var window = FindWindow<ObjectsWindow>();
			if( window != null && window.Visible )
			{
				var panel = window.GetPanel( documentWindow );
				if( panel != null )
				{
					var browser = panel.control as ContentBrowser;
					if( browser != null )
						ContentBrowserUtility.SelectComponentItems( browser, components );
				}
			}
		}

		public static void GetRestartApplication( out bool needRestart, out bool resetWindowsSettings )
		{
			needRestart = needRestartApplication;
			resetWindowsSettings = needRestartApplication_ResetWindowsSettings;
		}

		public static void SetRestartApplication( bool needRestart, bool resetWindowsSettings = false )
		{
			needRestartApplication = needRestart;
			needRestartApplication_ResetWindowsSettings = resetWindowsSettings;
		}

		public static void BeginCloseApplication()
		{
			if( EditorForm != null )
				EditorForm.needClose = true;
		}

		public static void BeginRestartApplication( bool resetWindowsSettings = false )
		{
			SetRestartApplication( true, resetWindowsSettings );
			BeginCloseApplication();
		}

		public static List<DocumentInstance> Documents
		{
			get { return EditorForm.Instance.Documents; }
		}

		public static DocumentInstance GetDocumentByObject( object obj )
		{
			if( obj == null )
				Log.Fatal( "EditorAPI: GetDocumentByObject: obj == null." );

			foreach( var document in Documents )
			{
				if( document.ResultObject == obj )
					return document;
			}

			//find by child components
			{
				var component = obj as Component;
				if( component != null )
				{
					var rootComponent = component.ParentRoot;
					foreach( var document in Documents )
					{
						if( document.ResultObject == rootComponent )
							return document;
					}
				}
			}

			return null;
		}

		public static void CloseAllDocumentWindowsOnSecondLevel( DocumentInstance document )
		{
			var toClose = new List<DocumentWindow>();

			foreach( var dockWindow in EditorForm.Instance.WorkspaceController.GetDockWindowsRecursive() )
			{
				var window = dockWindow as DocumentWindow;
				if( window != null && window.Document2 == document && window.IsWindowInWorkspace && window.ObjectOfWindow != document.ResultObject )
					toClose.Add( window );
			}

			foreach( var window in toClose )
				window.Close();
		}

		public static void CloseAllDocumentWindowsOnSecondLevelWithDeletedObjects( IDocumentInstance document )
		{
			var toClose = new List<DocumentWindow>();

			foreach( var dockWindow in EditorForm.Instance.WorkspaceController.GetDockWindowsRecursive() )
			{
				var window = dockWindow as DocumentWindow;
				if( window != null && window.Document2 == document && window.IsWindowInWorkspace && window.ObjectOfWindow != document.ResultObject && window.ObjectOfWindowIsDeleted )
					toClose.Add( window );
			}

			foreach( var window in toClose )
				window.Close();
		}

		public static IDocumentWindow[] FindDocumentWindowsWithObject( object obj )
		{
			var result = new List<IDocumentWindow>();

			foreach( var dockWindow in EditorForm.Instance.WorkspaceController.GetDockWindowsRecursive() )
			{
				var window = dockWindow as DocumentWindow;
				if( window != null && window.ObjectOfWindow == obj )
					result.Add( window );
			}

			return result.ToArray();
		}

		//public static List<DocumentWindow> GetDocumentWindowsOnSecondLevelWithObject( object obj )
		//{
		//	if( obj == null )
		//		Log.Fatal( "EditorAPI: GetDocumentWindowsOnSecondLevelWithObject: obj == null." );

		//	var result = new List<DocumentWindow>();

		//	foreach( var dockWindow in EditorForm.Instance.WorkspaceController.GetDockWindowsRecursive() )
		//	{
		//		var documentWindow = dockWindow as DocumentWindow;
		//		if( documentWindow != null && documentWindow.IsWindowInWorkspace && documentWindow.ObjectOfWindow == obj )
		//			result.Add( documentWindow );
		//	}

		//	return result;
		//}

		//public static void CloseDockWindowsOnSecondLevelWithObject( object obj )
		//{
		//	foreach( var window in GetDocumentWindowsOnSecondLevelWithObject( obj ) )
		//		window.Close();
		//}

		public static void SelectDockWindow( DockWindow window )
		{
			EditorForm.Instance.WorkspaceController.SelectDockWindow( window );
		}

		//!!!!good? игнорирует OpenAsSettings
		public static DocumentWindow SelectedDocumentWindow
		{
			get { return EditorForm.Instance.WorkspaceController.SelectedDocumentWindow; }
		}

		static DocumentWindow lastSelectedDocumentWindow;
		public static event Action SelectedDocumentWindowChanged;

		internal static void SelectedDocumentWindowChangedUpdate()
		{
			var selected = SelectedDocumentWindow;
			if( lastSelectedDocumentWindow != selected )
			{
				lastSelectedDocumentWindow = selected;
				SelectedDocumentWindowChanged?.Invoke();
			}
		}

		public static DocumentInstance SelectedDocument
		{
			get
			{
				var window = SelectedDocumentWindow;
				if( window != null )
					return window.Document2;
				return null;
			}
		}

		public static bool ExistsModifiedDocuments()
		{
			foreach( var doc in Documents )
			{
				if( doc.Modified )
					return true;
			}
			return false;
		}

		public static bool SaveDocuments( /*Func<DocumentInstance, bool> filter = null*/ )
		{
			foreach( var doc in Documents )
			{
				//if( filter != null && !filter( doc ) )
				//	continue;

				if( doc.Modified )
				{
					if( !doc.Save() )
					{
						//!!!!
						return false;
					}
				}
			}
			return true;
		}

		public static DocumentInstance GetDocumentByRealFileName( string realFileName, string specialMode )
		{
			realFileName = VirtualPathUtility.NormalizePath( realFileName );
			if( specialMode == null )
				specialMode = "";

			foreach( var document in Documents )
			{
				if( !string.IsNullOrEmpty( document.RealFileName ) )
				{
					if( string.Compare( VirtualPathUtility.NormalizePath( document.RealFileName ), realFileName, true ) == 0 && document.SpecialMode == specialMode )
						return document;
				}
			}

			return null;
		}

		public static DocumentInstance GetDocumentByResource( Resource.Instance instance )
		{
			foreach( var document in Documents )
			{
				if( document.LoadedResource == instance )
					return document;
			}
			return null;
		}

		public delegate void EditorActionGetStateEventDelegate( EditorActionGetStateContext context );
		public static event EditorActionGetStateEventDelegate EditorActionGetStateEvent;

		public delegate void EditorActionClickEventDelegate( EditorActionClickContext context );
		public static event EditorActionClickEventDelegate EditorActionClickEvent;
		public static event EditorActionClickEventDelegate EditorActionClick2Event;

		public static EditorActionGetStateContext EditorActionGetState( EditorActionHolder holder, EditorAction action )
		{
			var objectsInFocus = GetObjectsInFocus( action.CommonType == EditorAction.CommonTypeEnum.Document );
			var context = new EditorActionGetStateContext( holder, objectsInFocus, action );

			//common
			action.PerformGetState( context );
			//selected document
			( (DocumentInstance)objectsInFocus.DocumentWindow?.Document )?.EditorActionGetState( context );
			//selected window
			( (DocumentWindow)objectsInFocus.DocumentWindow )?.EditorActionGetState( context );
			//event
			EditorActionGetStateEvent?.Invoke( context );

			if( !EditorUtility.PerformEditorActionVisibleFilter( action ) )
				context.Enabled = false;

			//context.Enabled = true;

			return context;
		}

		public static EditorActionGetStateContext EditorActionGetState( EditorActionHolder holder, string actionName )
		{
			var action = EditorActions.GetByName( actionName );
			if( action == null || action.CompletelyDisabled )
				return null;
			return EditorActionGetState( holder, action );
		}

		public static void EditorActionClick( EditorActionHolder holder, EditorAction action )
		{
			var state = EditorActionGetState( holder, action );
			if( state.Enabled )
			{
				var objectsInFocus = state.ObjectsInFocus;
				//var objectsInFocus = GetObjectsInFocus();

				var context = new EditorActionClickContext( holder, objectsInFocus, action );

				//common
				action.PerformClick( context );
				//selected document
				( (DocumentInstance)objectsInFocus.DocumentWindow?.Document )?.EditorActionClick( context );
				//selected window
				( (DocumentWindow)objectsInFocus.DocumentWindow )?.EditorActionClick( context );
				//event
				EditorActionClickEvent?.Invoke( context );
			}
		}

		public static void EditorActionClick( EditorActionHolder holder, string actionName )
		{
			var action = EditorActions.GetByName( actionName );
			if( action == null || action.CompletelyDisabled )
				return;
			EditorActionClick( holder, action );
		}

		public static void EditorActionClick2( EditorActionHolder holder, EditorAction action )
		{
			var state = EditorActionGetState( holder, action );
			if( state.Enabled )
			{
				var objectsInFocus = state.ObjectsInFocus;
				//var objectsInFocus = GetObjectsInFocus();

				var context = new EditorActionClickContext( holder, objectsInFocus, action );

				//common
				action.PerformClick2( context );
				//selected document
				( (DocumentInstance)objectsInFocus.DocumentWindow?.Document )?.EditorActionClick2( context );
				//selected window
				( (DocumentWindow)objectsInFocus.DocumentWindow )?.EditorActionClick2( context );
				//event
				EditorActionClick2Event?.Invoke( context );
			}
		}

		public static void EditorActionClick2( EditorActionHolder holder, string actionName )
		{
			var action = EditorActions.GetByName( actionName );
			if( action == null || action.CompletelyDisabled )
				return;
			EditorActionClick2( holder, action );
		}

		public static void ShowProjectSettings()
		{
			OpenFileAsDocument( VirtualPathUtility.GetRealPathByVirtual( ProjectSettings.FileName ), true, true, true, "ProjectSettingsUserMode" );
		}

		public static void NeedShowProjectSettings()
		{
			needShowProjectSettings = true;
		}

		public static float DPI
		{
			get { return DpiHelper.Default.Dpi; }
		}

		public static float DPIScale
		{
			get { return DpiHelper.Default.DpiScaleFactor; }
		}

		//public static float DPI
		//{
		//	get { return EditorForm.Instance.dpi; }
		//}

		//public static float DPIScale
		//{
		//	get { return DPI / 96.0f; }
		//}

		public static ObjectsInFocus GetObjectsInFocus( bool useOnlySelectedDocumentWindow )
		{
			return EditorForm.Instance.GetObjectsInFocus( useOnlySelectedDocumentWindow );
		}

		static Dictionary<string, Type> documentWindowClassByFileExtension;
		public static Dictionary<string, Type> DocumentWindowClassByFileExtension
		{
			get
			{
				if( documentWindowClassByFileExtension == null )
					documentWindowClassByFileExtension = EditorAssemblyInterface.Instance.DocumentWindowClassByFileExtension;
				return documentWindowClassByFileExtension;
			}
		}

		public static void OpenNewObjectWindow( NewObjectWindow.CreationDataClass initData )
		{
			var window = new NewObjectWindow();
			window.creationData = initData;
			EditorForm.Instance.WorkspaceController.AddDockWindow( window, true, true );
		}

		//public static void OpenImportWindow( string initialDestinationFolder )
		//{
		//	var window = new ImportWindow();
		//	window.InitialDestinationFolder = initialDestinationFolder;
		//	EditorForm.Instance.WorkspaceController.AddDockWindow( window, true, true );
		//}

		public static void OpenSetReferenceWindow( DocumentWindow documentWindow, Component[] selectedComponents, object[] propertyOwners, Metadata.Property property, object[] propertyIndexes )
		//, Metadata.TypeInfo demandedType, bool allowNull )
		{
			//!!!!

			var window = new SetReferenceWindow();
			window.documentWindow = documentWindow;

			var data = new ContentBrowser.SetReferenceModeDataClass();
			data.selectedComponents = selectedComponents;
			data.propertyOwners = propertyOwners;
			data.property = property;
			data.propertyIndexes = propertyIndexes;
			window.setReferenceModeData = data;

			//!!!!начальное положение, настройки. везде так

			EditorForm.Instance.WorkspaceController.AddDockWindow( window, true, true );
		}

		////!!!! rename: OpenSetResourceWindow
		//public void ShowSetResourceWindow( DocumentWindow documentWindow, object[] selectedObjects, Metadata.Property property, object[] propertyIndexes, bool readOnly )
		////, Metadata.TypeInfo demandedType, bool allowNull )
		//{
		//	var window = new SetResourceWindow();
		//	window.documentWindow = documentWindow;
		//	window.initData = new SetResourceWindow.InitDataClass();
		//	window.initData.selectedObjects = selectedObjects;
		//	window.initData.property = property;
		//	window.initData.propertyIndexes = propertyIndexes;
		//	window.initData.readOnly = readOnly;

		//	//!!!!начальное положение, настройки. везде так

		//	workspaceController.AddDockWindow( window, true, true );
		//}

		//!!!!надо закрывать все окна, когда документ закрывается. или как-то так
		//!!!! rename: OpenObjectSettingsWindow
		public static DocumentWindow ShowObjectSettingsWindow( DocumentInstance document, object obj, bool canUseAlreadyOpened )
		{
			//!!!!если ссылка настроена, то нельзя редактировать

			//check already opened
			if( canUseAlreadyOpened )
			{
				var openedWindow = EditorForm.Instance.WorkspaceController.FindWindowRecursive( document, obj, typeof( ObjectSettingsWindow ) );
				if( openedWindow != null )
				{
					EditorForm.Instance.WorkspaceController.SelectDockWindow( openedWindow );
					return openedWindow;
				}
			}

			var window = new ObjectSettingsWindow();
			window.InitDocumentWindow( document, obj, true, null );

			//!!!!

			EditorForm.Instance.WorkspaceController.AddDockWindow( window, true, true );

			return window;
		}

		static internal DocumentInstance CreateDocument( string realFileName, string specialMode = "" )
		{
			if( specialMode == "TextEditor" )
			{
				var document = new DocumentInstance( realFileName, null, specialMode );
				Documents.Add( document );
				return document;
			}

			string virtualFileName = VirtualPathUtility.GetVirtualPathByReal( realFileName );
			if( !string.IsNullOrEmpty( virtualFileName ) && IsDocumentFileSupport( realFileName ) )
			//if( !string.IsNullOrEmpty( virtualFileName ) && IsDocumentFileSupport( virtualFileName ) )
			{
				Resource.Instance resourceIns = null;

				var ext = Path.GetExtension( virtualFileName ).ToLower();
				if( ext != "" && ext[ 0 ] == '.' )
					ext = ext.Substring( 1 );
				if( ResourceManager.GetTypeByFileExtension( ext ) != null )
				{
					//unload resource if it file was deleted
					{
						var res = ResourceManager.GetByName( virtualFileName );
						if( res != null && res.FileWasDeleted )
							res.Dispose();
					}

					resourceIns = ResourceManager.LoadSeparateInstance( virtualFileName, true, true, null );
					//resourceIns = ResourceManager.LoadResource( virtualFileName, true );

					if( resourceIns == null )
						return null;
				}

				var document = new DocumentInstance( realFileName, resourceIns, specialMode );
				Documents.Add( document );

				return document;
			}

			return null;
		}

		public static IDocumentWindow/*DockWindow*/ OpenFileAsDocument( string realFileName, bool canUseAlreadyOpened, bool select, bool floatingWindow = false, string specialMode = "" )
		{
			if( string.IsNullOrEmpty( specialMode ) && !IsDocumentFileSupport( realFileName ) )
				return null;

			realFileName = VirtualPathUtility.NormalizePath( realFileName );

			//!!!!!грузить не сразу

			//check for already opened
			if( canUseAlreadyOpened )
			{
				var document2 = GetDocumentByRealFileName( realFileName, specialMode );
				if( document2 != null )
				{
					var openedWindow = (DockWindow)EditorForm.Instance.WorkspaceController.FindWindow( document2 );
					if( openedWindow != null )
					{
						EditorForm.Instance.WorkspaceController.SelectDockWindow( openedWindow );
						return openedWindow as IDocumentWindow;
					}
				}
			}

			string virtualFileName = VirtualPathUtility.GetVirtualPathByReal( realFileName );
			var insideAssets = !string.IsNullOrEmpty( virtualFileName );
			var specialMode2 = specialMode;

			//open in text editor for resources which are outside Assets folder
			if( !insideAssets )
			{
				try
				{
					var extension = Path.GetExtension( realFileName ).ToLower();
					if( extension == ".txt" || extension == ".settings" || extension == ".config" || extension == ".info" || extension == ".log" || extension == ".json" || extension == ".cs" || extension == ".h" || extension == ".c" || extension == ".cpp" )
						specialMode2 = "TextEditor";
				}
				catch { }
			}

			if( insideAssets || specialMode2 == "TextEditor" )
			{
				//!!!!открывать один ресурс множество раз
				//в контекстном меню типа такого "Open new instance"

				var document = CreateDocument( realFileName, specialMode2 );
				if( document == null )
				{
					//Log.Warning( "impl" );
					return null;
				}

				var window = CreateWindow( document );
				EditorForm.Instance.WorkspaceController.AddDockWindow( window, floatingWindow, select /*!!!! select - not used*/ );

				if( specialMode2 == "ProjectSettingsUserMode" )
				{
					window.CloseByReturn = true;
					window.CloseByEscape = true;
				}

				return window as IDocumentWindow;
			}
			else
			{
				Log.Error( "Can't load resource outside Assets folder." );
				return null;
			}
		}

		public static DocumentWindow OpenDocumentWindowForObject( DocumentInstance document, object obj )//, bool canUseAlreadyOpened )
		{
			if( !IsDocumentObjectSupport( obj ) )
				return null;

			//another document or no document
			{
				var objectToDocument = GetDocumentByObject( obj );
				if( objectToDocument == null || objectToDocument != document )
				{
					var component = obj as Component;
					if( component != null )
					{
						var fileName = ComponentUtility.GetOwnedFileNameOfComponent( component );
						if( !string.IsNullOrEmpty( fileName ) )
						{
							var realFileName = VirtualPathUtility.GetRealPathByVirtual( fileName );

							if( IsDocumentFileSupport( realFileName ) )
							{
								var documentWindow = OpenFileAsDocument( realFileName, true, true ) as DocumentWindow;
								if( documentWindow != null )
								{
									var newDocument = documentWindow.Document2;
									var newObject = newDocument.ResultComponent.Components[ component.GetPathFromRoot() ];
									if( newObject != null )
									{
										return OpenDocumentWindowForObject( newDocument, newObject );
									}
								}

								return null;
							}
						}
					}

					return null;
				}
			}

			//check for already opened
			var canUseAlreadyOpened = !EditorForm.ModifierKeys.HasFlag( Keys.Shift );
			if( canUseAlreadyOpened )
			{
				var openedWindow = EditorForm.Instance.WorkspaceController.FindWindowRecursive( document, obj );
				if( openedWindow != null )
				{
					EditorForm.Instance.WorkspaceController.SelectDockWindow( openedWindow );
					return openedWindow;
				}
			}

			//create document window
			var window = CreateWindowImpl( document, obj, false );

			//!!!!
			bool floatingWindow = false;
			bool select = true;

			EditorForm.Instance.WorkspaceController.AddDockWindow( window, floatingWindow, select );

			return window;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="title"></param>
		/// <param name="select"></param>
		/// <param name="readOnly"></param>
		/// <param name="highlightingScheme">This is the prefix for file path. Specify \'CSharp\'. 'Base\Tools\Highlighting\{CSharp}Dark.xshd'</param>
		/// <param name="selectLine"></param>
		/// <returns></returns>
		public static DocumentInstance OpenTextAsDocument( string text, string title, bool select, bool readOnly = false, string highlightingScheme = "", int selectLine = 0 )//RangeFrom = 0, int selectRangeTo = 0 )
		{
			//!!!!TODO: check for already opened

			var document = new DocumentInstance( "", null, "" );
			Documents.Add( document );

			var type = EditorAssemblyInterface.Instance.GetTypeByName( "NeoAxis.Editor.TextEditorDocumentWindow" );
			var window = (DocumentWindow)Activator.CreateInstance( type );
			//TextEditorDocumentWindow window = new TextEditorDocumentWindow();

			var windowTypeSpecificOptions = new Dictionary<string, object>();
			windowTypeSpecificOptions[ "ReadOnly" ] = readOnly;
			windowTypeSpecificOptions[ "HighlightingScheme" ] = highlightingScheme;
			windowTypeSpecificOptions[ "SelectLine" ] = selectLine;
			//windowTypeSpecificOptions[ "SelectRange" ] = new RangeI( selectRangeFrom, selectRangeTo );

			window.InitDocumentWindow( document, null, false, windowTypeSpecificOptions );
			window.PropertySet( "Data", text );//window.Data = text;
			window.WindowTitle = title;

			EditorForm.Instance.WorkspaceController.AddDockWindow( window, false, select );

			return document;
		}

		// window config support.
		internal static DockWindow CreateWindow( DocumentInstance document )
		{
			DockWindow window = null;

			if( document.IsEditorDocumentConfigurationExist )
			{
				try
				{
					window = new WorkspaceWindow( EditorForm.Instance );
					( (WorkspaceWindow)window ).InitFromConfig( document );
				}
				catch( Exception e )
				{
					//!!!!
					Log.Info( $"Warning: Layout for '{document.Name}' doesnt restored." );
					Log.Info( e.Message );

					if( window != null )
						window.Dispose();

					// just create window without workspace.
					window = CreateWindowImpl( document, document.ResultObject, false );
				}
			}
			else
			{
				//create document window
				window = CreateWindowImpl( document, document.ResultObject, false );
			}

			return window;
		}

		static internal DocumentWindow CreateWindowImpl( DocumentInstance document, string objectPath, bool openAsSettings )
		{
			object obj = null;
			if( string.IsNullOrEmpty( objectPath ) )
			{
				obj = document.ResultComponent; // root component
			}
			else
			{
				obj = document.ResultComponent.Components.GetByPath( objectPath );
				if( obj == null )
					throw new Exception( $"Object with path '{objectPath}' for window '{document.Name}' not found." );
			}

			return CreateWindowImpl( document, obj, openAsSettings );
		}

		static DocumentWindow CreateWindowImpl( DocumentInstance document, object obj, bool openAsSettings )
		{
			Type type;
			if( openAsSettings )
				type = typeof( ObjectSettingsWindow );
			else if( document.SpecialMode == "TextEditor" )
				type = EditorAssemblyInterface.Instance.GetTypeByName( "NeoAxis.Editor.TextEditorDocumentWindow" );// typeof( TextEditorDocumentWindow );
			else
				type = GetDocumentWindowClass( obj ) ?? GetWindowClassFromFileName( document.RealFileName ) ?? typeof( DocumentWindow );

			DocumentWindow window;
			if( typeof( CanvasBasedEditor ).IsAssignableFrom( type ) )
			{
				var editor = (CanvasBasedEditor)Activator.CreateInstance( type );
				var window2 = new DocumentWindowWithViewport_CanvasBasedEditor( editor );
				editor.owner = window2;
				window = window2;
			}
			else
				window = (DocumentWindow)Activator.CreateInstance( type );

			window.InitDocumentWindow( document, obj, openAsSettings, null );
			return window;
		}

		static internal bool IsDocumentFileSupport( string documentFileName )
		{
			var ext = Path.GetExtension( documentFileName ).ToLower();
			if( ext != "" && ext[ 0 ] == '.' )
				ext = ext.Substring( 1 );

			if( ResourceManager.GetTypeByFileExtension( ext ) != null )
			{
				//can open only which inside Assets for resource types
				var virtualPath = VirtualPathUtility.GetVirtualPathByReal( documentFileName, true );
				if( !string.IsNullOrEmpty( virtualPath ) )
					return true;
			}

			//cs, txt
			return DocumentWindowClassByFileExtension.ContainsKey( ext );
		}

		//!!!!name
		public static bool IsDocumentObjectSupport( object obj )
		{
			//!!!!а если класс не C#ный? такое может быть? тогда его тоже уметь добавлять сюда как-то нужно

			Type windowClass = GetDocumentWindowClass( obj );
			return windowClass != null && windowClass != typeof( DocumentWindow );
		}

		internal static Type GetDocumentWindowClass( object obj )
		{
			if( obj != null )
			{
				var attribs = (EditorControlAttribute[])obj.GetType().GetCustomAttributes( typeof( EditorControlAttribute ), true );
				if( attribs.Length != 0 )
				{
					var attrib = attribs[ 0 ];

					Type type;

					if( !string.IsNullOrEmpty( attrib.DocumentClassName ) )
					{
						var type2 = EditorUtility.GetTypeByName( attrib.DocumentClassName );
						if( type2 == null )
							Log.Warning( $"PreviewWindow: GetDocumentWindowClass: Class with name \"{attrib.DocumentClassName}\" is not found." );
						type = type2;
					}
					else
						type = attrib.DocumentClass;

					if( type != null && attrib.OnlyWhenRootComponent && obj is Component c && c.Parent != null )
						type = null;

					if( type != null )
						return type;
				}
			}
			return null;
		}

		static Type GetWindowClassFromFileName( string documentFileName )
		{
			var ext = Path.GetExtension( documentFileName ).ToLower();
			if( ext != "" && ext[ 0 ] == '.' )
				ext = ext.Substring( 1 );

			Type windowClass = null;
			if( !string.IsNullOrEmpty( ext ) )
				DocumentWindowClassByFileExtension.TryGetValue( ext, out windowClass );

			return windowClass;
		}

		public static DocumentWindow[] GetAllDocumentWindowsOfDocument( DocumentInstance document )
		{
			var result = new List<DocumentWindow>();

			foreach( var dockWindow in EditorForm.Instance.WorkspaceController.GetDockWindowsRecursive() )
			{
				var window = dockWindow as DocumentWindow;
				if( window != null && window.Document2 == document )
					result.Add( window );
			}

			return result.ToArray();
		}

		//public static DocumentInstance OpenStore( bool openBasicContent = false )
		//{
		//	//select already opened
		//	var w = FindWindow<StoreDocumentWindow>();
		//	if( w != null )
		//	{
		//		SelectDockWindow( w );
		//		if( openBasicContent )
		//			w.LoadURL( StoreDocumentWindow.homeURLBasicContent );
		//		return w.Document;
		//	}

		//	var document = new DocumentInstance( "", null, "" );
		//	Documents.Add( document );

		//	var window = new StoreDocumentWindow();
		//	window.InitDocumentWindow( document, null, false, null );
		//	if( openBasicContent )
		//		window.StartURL = StoreDocumentWindow.homeURLBasicContent;

		//	EditorForm.Instance.WorkspaceController.AddDockWindow( window, false, true );

		//	return document;
		//}

		//public static void OpenOrCloseStore()
		//{
		//	var w = FindWindow<StoreDocumentWindow>();
		//	if( w != null )
		//		w.Close();
		//	else
		//		OpenStore();
		//}

		public static DocumentInstance OpenStartPage()
		{
			//select already opened
			var w = FindWindow<StartPageWindow>();
			if( w != null )
			{
				SelectDockWindow( w );
				return w.Document2;
			}

			var document = new DocumentInstance( "", null, "" );
			Documents.Add( document );

			var window = new StartPageWindow();
			window.InitDocumentWindow( document, null, false, null );

			EditorForm.Instance.WorkspaceController.AddDockWindow( window, false, true );

			return document;
		}

		public static void OpenOrCloseStartPage()
		{
			var w = FindWindow<StartPageWindow>();
			if( w != null )
				w.Close();
			else
				OpenStartPage();
		}

		public static void OpenStoresWindow()
		{
			var window = FindWindow<StoresWindow>();
			if( window != null )
				SelectDockWindow( window );
		}

		public static void OpenPackages( string selectPackage, bool install )
		{
			var window = FindWindow<StoresWindow>();
			if( window != null )
			{
				SelectDockWindow( window );
				if( !string.IsNullOrEmpty( selectPackage ) )
					window.NeedSelectPackage( selectPackage, install );
			}
		}

		public static Keys[] GetActionShortcuts( string name )
		{
			if( EditorForm != null )
			{
				var actionItem = ProjectSettings.Get.Shortcuts.ShortcutSettings.GetActionItem( name );
				if( actionItem != null )
					return EditorUtility2.ConvertKeys( actionItem.ToArray() );
			}
			return null;
		}

		public static bool ProcessShortcuts( Keys keyCode, bool allowKeysWithoutModifiers )
		{
			if( EditorForm != null )
			{
				Keys keys = keyCode | Control.ModifierKeys;

				var shortcuts = new Dictionary<Keys, ProjectSettingsPage_Shortcuts.ShortcutSettingsClass.ActionItem>( 64 );
				foreach( var a in ProjectSettings.Get.Shortcuts.ShortcutSettings.Actions )
				{
					if( a.Shortcut1 != ProjectSettingsPage_Shortcuts.Keys2.None )
						shortcuts[ (Keys)a.Shortcut1 ] = a;
					if( a.Shortcut2 != ProjectSettingsPage_Shortcuts.Keys2.None )
						shortcuts[ (Keys)a.Shortcut2 ] = a;
				}

				shortcuts.TryGetValue( keys, out var actionItem );

				if( actionItem != null )
				{
					var action = EditorActions.GetByName( actionItem.Name );
					if( action != null && !action.CompletelyDisabled )
					{
						foreach( var shortcut in EditorUtility2.ConvertKeys( actionItem.ToArray() ) )
						{
							if( shortcut != Keys.None && shortcut == keys )
							{
								if( allowKeysWithoutModifiers || ( shortcut & ( Keys.Control | Keys.Shift | Keys.Alt ) ) != 0 ||
									( keys >= Keys.F1 && keys <= Keys.F24 ) )
								{
									var c = EditorActionGetState( EditorActionHolder.ShortcutKey, action );
									if( c.Enabled )
									{
										EditorActionClick( EditorActionHolder.ShortcutKey, action.Name );
										return true;
									}
								}
							}
						}
					}
				}
			}

			return false;
		}

		public static Image GetImageForDispalyScale( Image image16px, Image image32px )//, bool useCache )
		{
			//!!!!it not works because ResourceManager gives a new images each call
			//if( useCache )
			//	return EditorResourcesCache.GetImageForDispalyScale( image16px, image32px );
			//else
			return RenderStandard.GetImageForDispalyScale( image16px, image32px );
		}

		public static Image GetImageForDispalyScale( string name )
		{
			var image16px = (Bitmap)Properties.Resources.ResourceManager.GetObject( name + "_16", Properties.Resources.Culture );
			var image32px = (Bitmap)Properties.Resources.ResourceManager.GetObject( name + "_32", Properties.Resources.Culture );

			return RenderStandard.GetImageForDispalyScale( image16px, image32px );
		}

		public delegate void GetObjectToCreateByContentBrowserItemEventDelegate( ContentBrowser.Item item, ref Metadata.TypeInfo objectType, ref string referenceToObject, ref object anyData );
		public static event GetObjectToCreateByContentBrowserItemEventDelegate GetObjectToCreateByContentBrowserItemEvent;

		public static (Metadata.TypeInfo objectType, string referenceToObject, object anyData, string objectName) GetObjectToCreateByContentBrowserItem( ContentBrowser.Item item )
		{
			Metadata.TypeInfo objectType = null;
			string referenceToObject = "";
			object anyData = null;
			var objectName = "";
			//string memberFullSignature = "";
			//Component createNodeWithComponent = null;

			//!!!!не все итемы можно создать.

			//_Type
			var typeItem = item as ContentBrowserItem_Type;
			if( typeItem != null )
			{
				var type = typeItem.Type;

				//!!!!генериковому нужно указать типы

				if( MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( type ) && !type.Abstract )
				{
					objectType = type;
					//referenceToObject = objectType.Name;
				}
			}

			//!!!!
			//var memberItem = item as ContentBrowserItem_Member;
			//if( memberItem != null )
			//{
			//	var member = memberItem.Member;

			//	var type = member.Creator as Metadata.TypeInfo;
			//	if( type != null )
			//		memberFullSignature = string.Format( "{0}|{1}", type.Name, member.Signature );
			//}

			//_File
			var fileItem = item as ContentBrowserItem_File;
			if( fileItem != null && !fileItem.IsDirectory )
			{
				//!!!!не делать предпросмотр для сцены, т.к. долго. что еще?
				var ext = Path.GetExtension( fileItem.FullPath );
				if( ResourceManager.GetTypeByFileExtension( ext ) != null )
				{
					var res = ResourceManager.GetByName( VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath ) );
					var type = res?.PrimaryInstance?.ResultComponent?.GetProvidedType();
					if( type != null )
					{
						objectType = type;
						referenceToObject = res.Name;
					}
				}
			}

			//_Component
			var componentItem = item as ContentBrowserItem_Component;
			if( componentItem != null )
			{
				var component = componentItem.Component;

				if( component.ParentRoot.HierarchyController != null &&
					component.ParentRoot.HierarchyController.CreatedByResource.InstanceType == Resource.InstanceType.Resource )
				{
					objectType = component.GetProvidedType();
					if( objectType != null )
						referenceToObject = objectType.Name;
				}
				//else
				//{
				//if( Scene.ParentRoot == component.ParentRoot )
				//{
				//!!!!

				////add node with component
				//createNodeWithComponent = component;
				//}
				//}
			}

			//_StoreItem
			var storeItem = item as StoresWindow.ContentBrowserItem_StoreItem;
			if( storeItem != null )
			{
				var (type, reference) = storeItem.GetFileToDrop( false );
				//var allowAutoInstall = SceneEditor.CreateObjectsMode == SceneEditor.CreateObjectsModeEnum.Drop;
				//var (type, reference) = storeItem.GetFileToDrop( allowAutoInstall );

				switch( type )
				{
				case PackageManager.PackageInfo.FileTypeToDrop.Mesh:
					objectType = MetadataManager.GetTypeOfNetType( typeof( Mesh ) );
					referenceToObject = reference;
					objectName = storeItem.Text;
					break;

				//for terrain paint
				case PackageManager.PackageInfo.FileTypeToDrop.Material:
					{
						var component = ResourceManager.LoadResource<Component>( reference, out _ );
						if( component != null )
							objectType = component.GetProvidedType();
					}
					break;

				case PackageManager.PackageInfo.FileTypeToDrop.Environment:
					{
						var component = ResourceManager.LoadResource<Component>( reference, out _ );
						if( component != null )
							objectType = component.GetProvidedType();
					}
					break;

				//!!!!check
				//for terrain paint
				case PackageManager.PackageInfo.FileTypeToDrop.Surface:
					{
						var component = ResourceManager.LoadResource<Component>( reference, out _ );
						if( component != null )
							objectType = component.GetProvidedType();
					}
					break;
				}
			}

			GetObjectToCreateByContentBrowserItemEvent?.Invoke( item, ref objectType, ref referenceToObject, ref anyData );

			return (objectType, referenceToObject, anyData, objectName);
		}

		public static (Metadata.TypeInfo objectType, string referenceToObject, object anyData, string objectName) GetObjectToCreateByDropData( DragEventArgs e )
		{
			var dragDropData = ContentBrowser.GetDroppingItemData( e.Data );
			if( dragDropData != null )
			{
				var droppingItem = dragDropData.Item;

				var contentBrowserItem = droppingItem;// as ContentBrowser.Item;
				if( contentBrowserItem != null )
					return GetObjectToCreateByContentBrowserItem( contentBrowserItem );
			}

			return (null, "", null, "");
		}

		public static ContentBrowser.Item CreateObjectGetSelectedContentBrowserItem()
		{
			var checkOrder = new List<DockWindow>();
			{
				var control = FindWindow<StoresWindow>().ContentBrowser1;
				if( WinFormsUtility.IsControlVisibleInHierarchy( control ) && WinFormsUtility.IsPhysicalVisibleCheckBy5Points( control ) )
				{
					checkOrder.Add( FindWindow<StoresWindow>() );
					checkOrder.Add( FindWindow<ResourcesWindow>() );
				}
				else
				{
					checkOrder.Add( FindWindow<ResourcesWindow>() );
					checkOrder.Add( FindWindow<StoresWindow>() );
				}
			}

			foreach( var window in checkOrder )
			{
				var selectedObjects = window.GetObjectsInFocus().Objects;
				if( selectedObjects.Length == 1 )
				{
					object selectedObject = selectedObjects[ 0 ];

					var item = selectedObject as ContentBrowser.Item;
					if( item != null )
						return item;
				}
			}

			////Resources Window
			//{
			//	var window = FindWindow<ResourcesWindow>();
			//	var selectedObjects = window.GetObjectsInFocus().Objects;
			//	if( selectedObjects.Length == 1 )
			//	{
			//		object selectedObject = selectedObjects[ 0 ];

			//		var item = selectedObject as ContentBrowser.Item;
			//		if( item != null )
			//			return item;
			//	}
			//}

			////Stores Window
			//{
			//	var window = FindWindow<StoresWindow>();
			//	var selectedObjects = window.GetObjectsInFocus().Objects;
			//	if( selectedObjects.Length == 1 )
			//	{
			//		object selectedObject = selectedObjects[ 0 ];

			//		var item = selectedObject as ContentBrowser.Item;
			//		if( item != null )
			//			return item;
			//	}
			//}

			return null;
		}

		public static (Metadata.TypeInfo objectType, string referenceToObject, object anyData, string objectName) GetSelectedObjectToCreate()
		{
			var checkOrder = new List<DockWindow>();
			{
				var control = FindWindow<StoresWindow>().ContentBrowser1;
				if( WinFormsUtility.IsControlVisibleInHierarchy( control ) && WinFormsUtility.IsPhysicalVisibleCheckBy5Points( control ) )
				{
					checkOrder.Add( FindWindow<StoresWindow>() );
					checkOrder.Add( FindWindow<ResourcesWindow>() );
				}
				else
				{
					checkOrder.Add( FindWindow<ResourcesWindow>() );
					checkOrder.Add( FindWindow<StoresWindow>() );
				}
			}

			foreach( var window in checkOrder )
			{
				var selectedObjects = window.GetObjectsInFocus().Objects;
				if( selectedObjects.Length == 1 )
				{
					object selectedObject = selectedObjects[ 0 ];

					var item = selectedObject as ContentBrowser.Item;
					if( item != null )
					{
						var value = GetObjectToCreateByContentBrowserItem( item );
						if( value.objectType != null )
							return value;
					}
				}
			}

			////Resources Window
			//{
			//	var window = FindWindow<ResourcesWindow>();
			//	var selectedObjects = window.GetObjectsInFocus().Objects;
			//	if( selectedObjects.Length == 1 )
			//	{
			//		object selectedObject = selectedObjects[ 0 ];

			//		var item = selectedObject as ContentBrowser.Item;
			//		if( item != null )
			//		{
			//			var value = GetObjectToCreateByContentBrowserItem( item );
			//			if( value.objectType != null )
			//				return value;
			//		}
			//	}
			//}

			////Stores Window
			//{
			//	var window = FindWindow<StoresWindow>();
			//	var selectedObjects = window.GetObjectsInFocus().Objects;
			//	if( selectedObjects.Length == 1 )
			//	{
			//		object selectedObject = selectedObjects[ 0 ];

			//		var item = selectedObject as ContentBrowser.Item;
			//		if( item != null )
			//		{
			//			var value = GetObjectToCreateByContentBrowserItem( item );
			//			if( value.objectType != null )
			//				return value;
			//		}
			//	}
			//}

			return (null, "", null, "");
		}

		public static void ResetSelectedObjectToCreate()
		{
			//Resources Window
			{
				var window = FindWindow<ResourcesWindow>();
				window.ContentBrowser1.SelectItems( null, false, true );
			}

			//Stores Window
			{
				var window = FindWindow<StoresWindow>();
				window.ContentBrowser1.SelectItemsList( null, true );
			}
		}

		public static void ShowTips()
		{
			var wc = EditorForm.Instance.WorkspaceController;
			var tipsWindow = wc.FindWindow<TipsWindow>();
			if( tipsWindow != null )
				wc.SelectDockWindow( tipsWindow );
			else
			{
				var document = new DocumentInstance( "", null, "" );
				Documents.Add( document );

				var window = new TipsWindow();
				window.InitDocumentWindow( document, null, false, null );

				wc.AddDockWindow( window, true, true );
			}
		}

		static DockWindow GetDockWindowOfDocument( DocumentInstance document )
		{
			foreach( var dockWindow in EditorForm.Instance.WorkspaceController.GetDockWindowsRecursive() )
			{
				var workspaceWindow = dockWindow as WorkspaceWindow;
				if( workspaceWindow != null && workspaceWindow.Document == document )
					return dockWindow;

				var documentWindow = dockWindow as DocumentWindow;
				if( documentWindow != null && documentWindow.Document2 == document && documentWindow.IsMainWindowInWorkspace )
					return dockWindow;
			}

			return null;
		}

		public static void CloseDocument( DocumentInstance document, bool askToSave )
		{
			var dockWindow = GetDockWindowOfDocument( document );
			if( dockWindow != null )
			{
				if( askToSave )
				{
					var dockingManager = EditorForm.Instance.WorkspaceController.DockingManager;
					var page = dockWindow.KryptonPage;
					if( page != null )
						dockingManager.CloseRequest( new string[] { page.UniqueName } );
				}
				else
					EditorForm.Instance.WorkspaceController.RemoveDockWindow( dockWindow, true );
			}
		}

		public static bool BuildProjectSolution( bool reloadDocuments )
		{
			SaveDocuments();
			ScreenNotifications2.ShowAllImmediately();

			CSharpProjectFileUtility.CheckToRemoveNotExistsFilesFromProject();

			var rebuild = Control.ModifierKeys.HasFlag( Keys.Control );

			if( rebuild || CSharpProjectFileUtility.CompilationIsRequired( false ) )
			{
				ScreenNotifications2.Show( EditorLocalization2.Translate( "General", "Building Project.sln..." ) );
				ScreenNotifications2.ShowAllImmediately();

				if( CSharpProjectFileUtility.Compile( false, rebuild, out var outputDllFilePath ) )
				{
					//reload Project assembly
					{
						var oldAssembly = EngineApp.ProjectAssembly;
						var assembly = AssemblyUtility.LoadAssemblyByRealFileName( outputDllFilePath, true, loadWithoutLocking: true, reloadingOldAssembly: oldAssembly );
						EngineApp.ProjectAssembly = assembly;
					}

					//update Resources window
					{
						var window = FindWindow<ResourcesWindow>();
						var browser = window?.ContentBrowser1;
						if( browser != null )
						{
							//!!!!Resources window types can be updated per item
							browser.UpdateDataIfResourcesWindowTypesChanged();
							browser.UpdateAllTypesItem();
						}
					}

					//ScreenNotifications.Show( EditorLocalization.Translate( "General", "Project.sln was built successfully." ) );

					//reload documents
					if( reloadDocuments )
					{
						var documentsToReload = new List<DocumentInstance>();
						foreach( var document in Documents )
						{
							if( document.ResultComponent != null )
								documentsToReload.Add( document );
						}

						if( documentsToReload.Count > 0 )
						{
							//ScreenNotifications.ShowAllImmediately();
							//ScreenNotifications.Show( EditorLocalization.Translate( "General", "Reloading documents..." ) );
							//ScreenNotifications.ShowAllImmediately();

							KryptonWinFormsUtility.EditorFormStartTemporaryLockUpdate();

							//!!!!save order of documents

							var selectDocument = SelectedDocument;

							foreach( var document in documentsToReload )
							{
								CloseDocument( document, false );
								if( document.Destroyed )
								{
									var window = OpenFileAsDocument( document.RealFileName, true, true, specialMode: document.SpecialMode );
									if( window != null && selectDocument == document )
										selectDocument = (DocumentInstance)window.Document;
								}
							}

							if( selectDocument != null )
							{
								var dockWindow = GetDockWindowOfDocument( selectDocument );
								if( dockWindow != null )
									SelectDockWindow( dockWindow );
							}
						}

						//if(EditorForm.Instance != null)
						//{
						//	EditorForm.Instance.SaveDockingStateAndUnloadDocuments();

						//	//!!!!выгрузить ресурсы

						//	EditorForm.Instance.LoadDockingState();

						//}

					}

					ScreenNotifications2.Show( EditorLocalization2.Translate( "General", "Project.sln was built successfully." ) );


					//compile Project.Client.dll
					if( File.Exists( CSharpProjectFileUtility.GetProjectSlnFullPath( true ) ) )
					{
						if( rebuild || CSharpProjectFileUtility.CompilationIsRequired( true ) )
						{
							ScreenNotifications2.Show( EditorLocalization2.Translate( "General", "Building Project.Client.sln..." ) );
							ScreenNotifications2.ShowAllImmediately();

							if( CSharpProjectFileUtility.Compile( true, rebuild, out var outputDllFilePath2 ) )
							{
								ScreenNotifications2.Show( EditorLocalization2.Translate( "General", "Project.Client.sln was built successfully." ) );
							}
						}
					}

					return true;
				}
				else
					return false;
			}
			else
			{
				ScreenNotifications2.Show( EditorLocalization2.Translate( "General", "The solution is up-to-date. Hold Ctrl to rebuild." ) );
				return true;
			}
		}

		public static bool ApplicationDoEventsIsAdditionalCall
		{
			get
			{
				lock( applicationDoEventsStack )
				{
					if( applicationDoEventsStack.Count > 0 )
						return applicationDoEventsStack.Peek();
					else
						return false;
				}
			}
		}

		public static void ApplicationDoEvents( bool additionalCall )
		{
			lock( applicationDoEventsStack )
			{
				if( applicationDoEventsStack.Count < 10 )
					applicationDoEventsStack.Push( additionalCall );
			}

			try
			{
				Application.DoEvents();
			}
			finally
			{
				lock( applicationDoEventsStack )
				{
					if( applicationDoEventsStack.Count > 0 )
						applicationDoEventsStack.Pop();
				}
			}
		}
#endif

		public static DocumentInstance GetDocumentByComponent( Component component )
		{
			var parentRoot = component.ParentRoot;
			foreach( var document in EditorForm.Instance.Documents )
			{
				if( document.ResultComponent == parentRoot )
					return document;
			}
			return null;
		}
	}
}
