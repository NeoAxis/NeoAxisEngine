// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Drawing;
using ComponentFactory.Krypton.Toolkit;
using SharpBgfx.Common;

namespace NeoAxis.Editor
{
	//!!!!name
	/// <summary>
	/// Provides an interface to work with the editor.
	/// </summary>
	public static class EditorAPI
	{
		static bool needRestartApplication;
		static bool needRestartApplication_ResetWindowsSettings;
		static bool closingApplication;

		//

		/*public */
		static EditorForm EditorForm
		{
			get { return EditorForm.Instance; }
		}

		public static bool IsEditor
		{
			get { return EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor; }
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
				window.ContentBrowser1?.NeedSelectFilesOrDirectories( realPaths, expandNodes );
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
				if( window != null && window.Document == document && window.IsWindowInWorkspace && window.ObjectOfWindow != document.ResultObject )
					toClose.Add( window );
			}

			foreach( var window in toClose )
				window.Close();
		}

		public static void CloseAllDocumentWindowsOnSecondLevelWithDeletedObjects( DocumentInstance document )
		{
			var toClose = new List<DocumentWindow>();

			foreach( var dockWindow in EditorForm.Instance.WorkspaceController.GetDockWindowsRecursive() )
			{
				var window = dockWindow as DocumentWindow;
				if( window != null && window.Document == document && window.IsWindowInWorkspace && window.ObjectOfWindow != document.ResultObject && window.ObjectOfWindowIsDeleted )
					toClose.Add( window );
			}

			foreach( var window in toClose )
				window.Close();
		}

		public static List<DocumentWindow> FindDocumentWindowsWithObject( object obj )
		{
			var result = new List<DocumentWindow>();

			foreach( var dockWindow in EditorForm.Instance.WorkspaceController.GetDockWindowsRecursive() )
			{
				var window = dockWindow as DocumentWindow;
				if( window != null && window.ObjectOfWindow == obj )
					result.Add( window );
			}

			return result;
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
					return window.Document;
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

		public static DocumentInstance GetDocumentByResource( Resource.Instance ins )
		{
			foreach( var document in Documents )
			{
				if( document.LoadedResource == ins )
					return document;
			}
			return null;
		}

		public delegate void EditorActionGetStateEventDelegate( EditorAction.GetStateContext context );
		public static event EditorActionGetStateEventDelegate EditorActionGetStateEvent;

		public delegate void EditorActionClickEventDelegate( EditorAction.ClickContext context );
		public static event EditorActionClickEventDelegate EditorActionClickEvent;

		public static EditorAction.GetStateContext EditorActionGetState( EditorAction.HolderEnum holder, EditorAction action )
		{
			var objectsInFocus = GetObjectsInFocus( action.CommonType == EditorAction.CommonTypeEnum.Document );
			var context = new EditorAction.GetStateContext( holder, objectsInFocus, action );

			//common
			action.PerformGetState( context );
			//selected document
			objectsInFocus.DocumentWindow?.Document?.EditorActionGetState( context );
			//selected window
			objectsInFocus.DocumentWindow?.EditorActionGetState( context );
			//event
			EditorActionGetStateEvent?.Invoke( context );

			if( !EditorUtility.PerformEditorActionVisibleFilter( action ) )
				context.Enabled = false;

			//context.Enabled = true;

			return context;
		}

		public static EditorAction.GetStateContext EditorActionGetState( EditorAction.HolderEnum holder, string actionName )
		{
			var action = EditorActions.GetByName( actionName );
			if( action == null || action.CompletelyDisabled )
				return null;
			return EditorActionGetState( holder, action );
		}

		public static void EditorActionClick( EditorAction.HolderEnum holder, EditorAction action )
		{
			var state = EditorActionGetState( holder, action );
			if( state.Enabled )
			{
				var objectsInFocus = state.ObjectsInFocus;
				//var objectsInFocus = GetObjectsInFocus();

				var context = new EditorAction.ClickContext( holder, objectsInFocus, action );

				//common
				action.PerformClick( context );
				//selected document
				objectsInFocus.DocumentWindow?.Document?.EditorActionClick( context );
				//selected window
				objectsInFocus.DocumentWindow?.EditorActionClick( context );
				//event
				EditorActionClickEvent?.Invoke( context );
			}
		}

		public static void EditorActionClick( EditorAction.HolderEnum holder, string actionName )
		{
			var action = EditorActions.GetByName( actionName );
			if( action == null || action.CompletelyDisabled )
				return;
			EditorActionClick( holder, action );
		}

		public static void ShowProjectSettings()
		{
			OpenFileAsDocument( VirtualPathUtility.GetRealPathByVirtual( ProjectSettings.FileName ), true, true, true, "ProjectSettingsUserMode" );
		}

		public static event Action ClosingApplicationChanged;

		//!!!!правильно везде расставлены проверки?
		public static bool ClosingApplication
		{
			get { return closingApplication; }
			set
			{
				if( closingApplication == value )
					return;
				closingApplication = value;

				ClosingApplicationChanged?.Invoke();
			}
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

		public static void OpenImportWindow( string initialDestinationFolder )
		{
			var window = new ImportWindow();
			window.InitialDestinationFolder = initialDestinationFolder;
			EditorForm.Instance.WorkspaceController.AddDockWindow( window, true, true );
		}

		public static void OpenSelectTypeWindow( SelectTypeWindow.CreationDataClass initData )
		{
			var window = new SelectTypeWindow();
			window.creationData = initData;
			EditorForm.Instance.WorkspaceController.AddDockWindow( window, true, true );
		}

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
			if( !string.IsNullOrEmpty( virtualFileName ) && IsDocumentFileSupport( virtualFileName ) )
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

					//!!!!!good?
					//!!!!wait

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

		//!!!! replace return type to IDocumentWindow ? it can be WorkspaceWindow or DocumentWindow 
		public static DockWindow OpenFileAsDocument( string realFileName, bool canUseAlreadyOpened, bool select, bool floatingWindow = false, string specialMode = "" )
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
						return openedWindow;
					}
				}
			}

			string virtualFileName = VirtualPathUtility.GetVirtualPathByReal( realFileName );
			if( !string.IsNullOrEmpty( virtualFileName ) )
			{
				//!!!!открывать один ресурс множество раз
				//в контекстном меню типа такого "Open new instance"

				var document = CreateDocument( realFileName, specialMode );
				if( document == null )
				{
					//Log.Warning( "impl" );
					return null;
				}

				var window = CreateWindow( document );
				EditorForm.Instance.WorkspaceController.AddDockWindow( window, floatingWindow, select /*!!!! select - not used*/ );

				return window;
			}
			else
			{
				//!!!!или для каких-то можно? типа текстовых
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
									var newDocument = documentWindow.Document;
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
					//Log.Info( $"Layout for '{document.Name}' restored from config!" );
				}
				catch( Exception exc ) //TODO: use more specific exc type.
				{
					//!!!!
					Log.Info( $"Warning: Layout for '{document.Name}' doesnt restored." );
					Log.Info( exc.Message );

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
				return true;

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
				var attribs = (EditorDocumentWindowAttribute[])obj.GetType().GetCustomAttributes( typeof( EditorDocumentWindowAttribute ), true );
				if( attribs.Length != 0 )
				{
					var attrib = attribs[ 0 ];

					if( !string.IsNullOrEmpty( attrib.DocumentClassName ) )
					{
						var type = EditorUtility.GetTypeByName( attrib.DocumentClassName );
						if( type == null )
							Log.Warning( $"PreviewWindow: GetDocumentWindowClass: Class with name \"{attrib.DocumentClassName}\" is not found." );
						return type;
					}
					else
						return attrib.DocumentClass;
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

		public static List<DocumentWindow> GetAllDocumentWindowsOfDocument( DocumentInstance document )
		{
			var result = new List<DocumentWindow>();

			foreach( var dockWindow in EditorForm.Instance.WorkspaceController.GetDockWindowsRecursive() )
			{
				var window = dockWindow as DocumentWindow;
				if( window != null && window.Document == document )
					result.Add( window );
			}

			return result;
		}

		public static DocumentInstance OpenStore()
		{
			//select already opened
			var w = FindWindow<StoreDocumentWindow>();
			if( w != null )
			{
				SelectDockWindow( w );
				return w.Document;
			}

			var document = new DocumentInstance( "", null, "" );
			Documents.Add( document );

			var window = new StoreDocumentWindow();
			window.InitDocumentWindow( document, null, false, null );

			EditorForm.Instance.WorkspaceController.AddDockWindow( window, false, true );

			return document;
		}

		public static void OpenOrCloseStore()
		{
			var w = FindWindow<StoreDocumentWindow>();
			if( w != null )
				w.Close();
			else
				OpenStore();
		}

		public static DocumentInstance OpenStartPage()
		{
			//select already opened
			var w = FindWindow<StartPageWindow>();
			if( w != null )
			{
				SelectDockWindow( w );
				return w.Document;
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

		public static DocumentInstance OpenPackages( string selectPackage )
		{
			//select already opened
			var w = FindWindow<PackagesWindow>();
			if( w != null )
			{
				SelectDockWindow( w );
				if( !string.IsNullOrEmpty( selectPackage ) )
					w.NeedSelectPackage( selectPackage );
				return w.Document;
			}

			var document = new DocumentInstance( "", null, "" );
			Documents.Add( document );

			var window = new PackagesWindow();
			window.InitDocumentWindow( document, null, false, null );

			EditorForm.Instance.WorkspaceController.AddDockWindow( window, false, true );

			if( !string.IsNullOrEmpty( selectPackage ) )
				window.NeedSelectPackage( selectPackage );

			return document;
		}

		public static void OpenOrClosePackages()
		{
			var w = FindWindow<PackagesWindow>();
			if( w != null )
				w.Close();
			else
				OpenPackages( null );
		}

		public static Keys[] GetActionShortcuts( string name )
		{
			var actionItem = ProjectSettings.Get.ShortcutSettings.GetActionItem( name );
			if( actionItem != null )
				return actionItem.ToArray();
			return null;
		}

		public static bool ProcessShortcuts( Keys keyCode )//!!!!из старого, bool processCharactersWithoutModifiers )
		{
			Keys keys = keyCode | Control.ModifierKeys;

			var shortcuts = new Dictionary<Keys, Component_ProjectSettings.ShortcutSettingsClass.ActionItem>( 64 );
			foreach( var a in ProjectSettings.Get.ShortcutSettings.Actions )
			{
				if( a.Shortcut1 != Keys.None )
					shortcuts[ a.Shortcut1 ] = a;
				if( a.Shortcut2 != Keys.None )
					shortcuts[ a.Shortcut2 ] = a;
			}

			shortcuts.TryGetValue( keys, out var actionItem );

			if( actionItem != null )
			{
				var action = EditorActions.GetByName( actionItem.Name );
				if( action != null && !action.CompletelyDisabled )
				{
					foreach( var shortcut in actionItem.ToArray() )
					{
						if( shortcut != Keys.None && shortcut == keys )
						{
							var c = EditorActionGetState( EditorAction.HolderEnum.ShortcutKey, action );
							if( c.Enabled )
							{
								EditorActionClick( EditorAction.HolderEnum.ShortcutKey, action.Name );
								return true;
							}
						}
					}
				}
			}

			//if( action.ShortcutKeys != null && Array.IndexOf( action.ShortcutKeys, keys ) != -1 )
			//{
			//	var c = EditorActionGetState( EditorAction.HolderEnum.ShortcutKey, action );
			//	if( c.Enabled )
			//	{
			//		EditorActionClick( EditorAction.HolderEnum.ShortcutKey, action.Name );
			//		return true;
			//	}
			//}
			//}

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

		//!!!!new

		public static (Metadata.TypeInfo objectType, string referenceToObject) GetObjectToCreateByContentBrowserItem( ContentBrowser.Item item )
		{
			Metadata.TypeInfo objectType = null;
			string referenceToObject = "";
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

			return (objectType, referenceToObject);
		}

		public static (Metadata.TypeInfo objectType, string referenceToObject) GetObjectToCreateByDropData( DragEventArgs e )
		{
			var dragDropData = ContentBrowser.GetDroppingItemData( e.Data );
			if( dragDropData != null )
			{
				var droppingItem = dragDropData.Item;

				var contentBrowserItem = droppingItem as ContentBrowser.Item;
				if( contentBrowserItem != null )
					return GetObjectToCreateByContentBrowserItem( contentBrowserItem );
			}

			return (null, "");
		}

		public static (Metadata.TypeInfo objectType, string referenceToObject) GetSelectedObjectToCreate()
		{
			//!!!!может откуда-то еще можно создавать, не только из этих окон

			//Resources Window
			{
				var window = FindWindow<ResourcesWindow>();
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

			//!!!!
			////Objects Window
			//{
			//	var window = EditorAPI.FindWindow<ObjectsWindow>();
			//	var selectedObjects = window.GetObjectsInFocus().Objects;
			//	if( selectedObjects.Length == 1 )
			//	{
			//		object selectedObject = selectedObjects[ 0 ];

			//		//!!!!

			//		var contentBrowserItem = selectedObject as ContentBrowser.Item;
			//		if( contentBrowserItem != null )
			//		{
			//			var value = GetObjectToCreateByContentBrowserItem( contentBrowserItem );
			//			if( value.objectType != null )
			//				return value;
			//		}
			//	}
			//}

			return (null, "");
		}

		public static void ResetSelectedObjectToCreate()
		{
			//Resources Window
			{
				var window = FindWindow<ResourcesWindow>();
				window.ContentBrowser1.SelectItems( null, false, true );
			}

			//!!!!Objects Window
			//{
			//}

			//что еще
		}

		public static bool DarkTheme
		{
			get { return EditorForm.Instance != null && EditorForm.Instance.DarkTheme; }
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
	}
}
