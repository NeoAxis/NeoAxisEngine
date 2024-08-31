//#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace NeoAxis.Editor
{
	static class EditorAssembly
	{
		public static void Init()
		{
			var assembly = Internal.AssemblyUtility.LoadAssemblyByRealFileName( "NeoAxis.Core.Editor.dll", false, false );

			var type = assembly.GetType( "NeoAxis.Editor.EditorAssemblyInterfaceImpl" );
			if( type == null )
				Log.Fatal( "EditorAssembly: Init: Type \"NeoAxis.Editor.EditorAssemblyInterfaceImpl\" is not exists." );

			var initMethod = type.GetMethod( "Init", BindingFlags.Public | BindingFlags.Static );
			if( initMethod == null )
				Log.Fatal( "EditorAssembly: Init: \"Init\" method of \"NeoAxis.Editor.EditorAssemblyInterfaceImpl\" type is not exists." );

			initMethod.Invoke( null, new object[ 0 ] );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	internal abstract class EditorAssemblyInterface
	{
		static EditorAssemblyInterface instance;
		public static EditorAssemblyInterface Instance { get { return instance; } }
		protected EditorAssemblyInterface() { instance = this; }

		/////////////////////////////////////////

		public abstract void SetDarkTheme();

		public abstract IScriptPrinter ScriptPrinterNew();

		public abstract Dictionary<string, Type> DocumentWindowClassByFileExtension { get; }

		public abstract void UpdateProjectFileForCSharpEditor( ICollection<string> addFiles, ICollection<string> removeFiles );

		public abstract void InitializeWPFApplicationAndScriptEditor();

		public abstract Type GetTypeByName( string typeName );

		public interface ITextEditorControl
		{
			string EditorText { get; set; }
			bool EditorReadOnly { get; set; }
			bool EditorWordWrap { get; set; }
			bool Border { get; set; }

			int SelectionStart { get; set; }
			int SelectionLength { get; set; }
			void Select( int start, int length );

			void ScrollToHome();
			void ScrollToEnd();
		}

		public abstract ITextEditorControl CreateTextEditorControl();

		/////////////////////////////////////////

		public interface IScriptPrinter
		{
			ImageComponent PrintToTexture( string code, Vector2I size );
		}

		/////////////////////////////////////////

#if !DEPLOY
		public abstract void ImportFBX( ImportGeneral.Settings settings, out string error );
		public abstract void ImportAssimp( ImportGeneral.Settings settings, out string error );
		public abstract bool ExportToFBX( Mesh sourceMesh, string realFileName, out string error );
#endif

		/////////////////////////////////////////

		//public abstract HCDropDownControl CreateColorValuePoweredSelectControl( HCItemProperty itemProperty );
		//public abstract bool ColorValuePoweredSelectFormShowDialog( Point location, ColorValuePowered initialColor, out ColorValuePowered resultColor );

		/////////////////////////////////////////

		public abstract EDialogResult ShowQuestion( string text, EMessageBoxButtons buttons, string caption = null );
		public abstract void ShowWarning( string text, string caption = null );
		public abstract void ShowInfo( string text, string caption = null );

		//public abstract float GetDPI();
		//public abstract float GetDPIScale();

		public abstract IDocumentInstance GetDocumentByComponent( Component component );
		public abstract IDocumentInstance GetDocumentByObject( object obj );
		public abstract string CreateEditorDocumentXmlConfiguration( IEnumerable<Component> components, Component selected = null );

		public abstract bool GetDarkTheme();

		public abstract string Translate( string group, string text );

		public abstract IDocumentWindow[] FindDocumentWindowsWithObject( object obj );
		public abstract void CloseAllDocumentWindowsOnSecondLevelWithDeletedObjects( IDocumentInstance document );
		public abstract ScreenNotifications.IStickyNotificationItem ShowScreenNotification( string text, bool error, bool sticky );
		public abstract void SelectDockWindow( IDockWindow window );
		public abstract IDocumentWindow OpenDocumentWindowForObject( IDocumentInstance document, object obj );
		public abstract IEditorAction[] GetEditorActions();
		public abstract FlowGraphNodeStyle Get_FlowGraphNodeStyle_Rectangle_Instance();

		public abstract bool EditorCommandLineTools_PlatformProjectPatch_Process( string destFile, string baseProjectFileName, out string error, out bool changed );

		public abstract void RegisterEditorAsembly( Assembly assembly, Type[] exportedTypes );
		public abstract void UnregisterEditorAsembly( Assembly assembly );

		public abstract IDocumentWindow[] GetAllDocumentWindowsOfDocument( IDocumentInstance document );

		public abstract void ContentBrowserUtility_AllContentBrowsers_SuspendChildrenChangedEvent();
		public abstract void ContentBrowserUtility_AllContentBrowsers_ResumeChildrenChangedEvent();

		public abstract IEditorAction GetEditorActionByName( string name );

		public abstract IEditorRibbonDefaultConfigurationTab[] GetEditorRibbonDefaultConfigurationTabs();

		public abstract void SurfaceEditorUtility_CreatePreviewObjects( Scene scene, Surface surface );

		public abstract bool IsAnyTransformToolInModifyingMode();

		public abstract void PreviewImagesManager_RegisterResourceType( string typeName );

		public abstract void XmlDocumentationFiles_Load( string xmlFile );

		public abstract void DocumentationLinksManager_AddNameByType( Type type, string name );

		public abstract void OpenSelectTypeWindow( SelectTypeWindowInitData initData );

		public abstract void Product_Store_ImageGenerator_WriteBitmapToStream( Stream writeStream, Product_Store.ImageGenerator.ImageFormat writeImageFormat, Vector2I imageSizeRender, Vector2I imageSizeOutput, IntPtr imageData );

#if !DEPLOY
		public abstract EditorContextMenu.Item EditorContextMenuNewItem( string text, EventHandler clickHandler );
		public abstract EditorContextMenu.Separator EditorContextMenuNewSeparator();
		public abstract void EditorContextMenuShow( ICollection<EditorContextMenu.ItemBase> items, Vector2I? screenPosition );
#endif

		public abstract void AfterFatalShowDialogAndSaveDocuments( string errorText, ref bool skipLogFatal );

		public abstract IEditorAction RegisterEditorAction( string name, string description, object imageOrImageHint, (string, string) ribbonText, Action<EditorActionGetStateContext> getState, Action<EditorActionClickContext> click, EditorActionContextMenuType contextMenuSupport );

		public abstract void RibbonAddAction( string tab, string group, string action );
		public abstract void QATAddAction( string action );

		public abstract ProcedureUI.Form CreateProcedureUIDialog( ProcedureUIDialogSettings settings );
		public abstract void ShowDialog( ProcedureUI.Form form );

		public abstract bool ShowOpenFileDialog( bool isFolderPicker, string initialDirectory, IEnumerable<(string rawDisplayName, string extensionList)> filters, out string[] fileNames );
		public abstract bool ShowOpenFileDialog( bool isFolderPicker, string initialDirectory, IEnumerable<(string rawDisplayName, string extensionList)> filters, out string fileName );
		public abstract bool ShowSaveFileDialog( string initialDirectory, string initialFileName, string filter, out string resultFileName );
	}
}

//#endif