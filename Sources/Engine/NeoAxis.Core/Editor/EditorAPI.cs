// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Provides an interface to work with the editor. Useful in the case when need to call editor functionality without adding a reference to NeoAxis.Core.Editor.dll.
	/// </summary>
	public static class EditorAPI
	{
		static bool closingApplication;
		public static event Action ClosingApplicationChanged;

		//

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

		public static bool DarkTheme
		{
			get
			{
#if !DEPLOY
				if( EditorAssemblyInterface.Instance != null )
					return EditorAssemblyInterface.Instance.GetDarkTheme();
#endif
				return true;
			}
		}

		//

		//public static bool IsEditor
		//{
		//	get { return EngineApp.IsEditor; }
		//}

		//public static float DPI
		//{
		//	get { return EditorAssemblyInterface.Instance.GetDPI(); }
		//}

		//public static float DPIScale
		//{
		//	get { return EditorAssemblyInterface.Instance.GetDPIScale(); }
		//}

		public static IDocumentInstance GetDocumentByComponent( Component component )
		{
			return EditorAssemblyInterface.Instance.GetDocumentByComponent( component );
		}

		public static IDocumentInstance GetDocumentByObject( object obj )
		{
			return EditorAssemblyInterface.Instance.GetDocumentByObject( obj );
		}

		public static string CreateEditorDocumentXmlConfiguration( IEnumerable<Component> components, Component selected = null )
		{
			return EditorAssemblyInterface.Instance.CreateEditorDocumentXmlConfiguration( components, selected );
		}

		public static IDocumentWindow[] FindDocumentWindowsWithObject( object obj )
		{
			return EditorAssemblyInterface.Instance.FindDocumentWindowsWithObject( obj );
		}

		public static void CloseAllDocumentWindowsOnSecondLevelWithDeletedObjects( IDocumentInstance document )
		{
			EditorAssemblyInterface.Instance.CloseAllDocumentWindowsOnSecondLevelWithDeletedObjects( document );
		}

		public static void SelectDockWindow( IDockWindow window )
		{
			EditorAssemblyInterface.Instance.SelectDockWindow( window );
		}

		public static IDocumentWindow OpenDocumentWindowForObject( IDocumentInstance document, object obj )
		{
			return EditorAssemblyInterface.Instance.OpenDocumentWindowForObject( document, obj );
		}

		public static IEditorAction[] GetEditorActions()
		{
			return EditorAssemblyInterface.Instance.GetEditorActions();
		}

		public static FlowGraphNodeStyle Get_FlowGraphNodeStyle_Rectangle_Instance()
		{
			return EditorAssemblyInterface.Instance.Get_FlowGraphNodeStyle_Rectangle_Instance();
		}

		public static bool EditorCommandLineTools_PlatformProjectPatch_Process( string destFile, string baseProjectFileName, out string error, out bool changed )
		{
			return EditorAssemblyInterface.Instance.EditorCommandLineTools_PlatformProjectPatch_Process( destFile, baseProjectFileName, out error, out changed );
		}

		public static void RegisterEditorAssembly( Assembly assembly, Type[] exportedTypes )
		{
			EditorAssemblyInterface.Instance.RegisterEditorAsembly( assembly, exportedTypes );
		}

		public static void UnregisterEditorAssembly( Assembly assembly )
		{
			EditorAssemblyInterface.Instance.UnregisterEditorAsembly( assembly );
		}

		public static IDocumentWindow[] GetAllDocumentWindowsOfDocument( IDocumentInstance document )
		{
			return EditorAssemblyInterface.Instance.GetAllDocumentWindowsOfDocument( document );
		}

		public static void ContentBrowserUtility_AllContentBrowsers_SuspendChildrenChangedEvent()
		{
			EditorAssemblyInterface.Instance.ContentBrowserUtility_AllContentBrowsers_SuspendChildrenChangedEvent();
		}

		public static void ContentBrowserUtility_AllContentBrowsers_ResumeChildrenChangedEvent()
		{
			EditorAssemblyInterface.Instance.ContentBrowserUtility_AllContentBrowsers_ResumeChildrenChangedEvent();
		}

		public static IEditorAction GetEditorActionByName( string name )
		{
			return EditorAssemblyInterface.Instance.GetEditorActionByName( name );
		}

		public static IEditorRibbonDefaultConfigurationTab[] GetEditorRibbonDefaultConfigurationTabs()
		{
			return EditorAssemblyInterface.Instance.GetEditorRibbonDefaultConfigurationTabs();
		}

		public static void SurfaceEditorUtility_CreatePreviewObjects( Scene scene, Surface surface )
		{
			EditorAssemblyInterface.Instance.SurfaceEditorUtility_CreatePreviewObjects( scene, surface );
		}

		public static bool IsAnyTransformToolInModifyingMode()
		{
			if( EngineApp.IsEditor )
				return EditorAssemblyInterface.Instance.IsAnyTransformToolInModifyingMode();
			else
				return false;
		}

		public static void PreviewImagesManager_RegisterResourceType( string typeName )
		{
			EditorAssemblyInterface.Instance.PreviewImagesManager_RegisterResourceType( typeName );
		}

		public static void XmlDocumentationFiles_Load( string xmlFile )
		{
			EditorAssemblyInterface.Instance.XmlDocumentationFiles_Load( xmlFile );
		}

		public static void DocumentationLinksManager_AddNameByType( Type type, string name )
		{
			EditorAssemblyInterface.Instance.DocumentationLinksManager_AddNameByType( type, name );
		}

		public static void OpenSelectTypeWindow( SelectTypeWindowInitData initData )
		{
			EditorAssemblyInterface.Instance.OpenSelectTypeWindow( initData );
		}

		public static void Product_Store_ImageGenerator_WriteBitmapToStream( Stream writeStream, Product_Store.ImageGenerator.ImageFormat writeImageFormat, Vector2I imageSizeRender, Vector2I imageSizeOutput, IntPtr imageData )
		{
			EditorAssemblyInterface.Instance.Product_Store_ImageGenerator_WriteBitmapToStream( writeStream, writeImageFormat, imageSizeRender, imageSizeOutput, imageData );
		}

		/// <summary>
		/// Register an editor action. Use this method for the code which not referencing to NeoAxis.Core.Editor.dll directly. As example when need to call the method from Project.csproj.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="imageOrImageHint">Right now only image hint is supported.</param>
		/// <param name="ribbonText"></param>
		/// <param name="getState"></param>
		/// <param name="click"></param>
		/// <param name="contextMenuSupport"></param>
		/// <returns></returns>
		public static IEditorAction RegisterEditorAction( string name, string description, object imageOrImageHint, (string, string) ribbonText, Action<EditorActionGetStateContext> getState, Action<EditorActionClickContext> click, EditorActionContextMenuType contextMenuSupport = EditorActionContextMenuType.None )
		{
			return EditorAssemblyInterface.Instance.RegisterEditorAction( name, description, imageOrImageHint, ribbonText, getState, click, contextMenuSupport );
		}

		public static void RibbonAddAction( string tab, string group, string action )
		{
			EditorAssemblyInterface.Instance?.RibbonAddAction( tab, group, action );
		}

		public static void QATAddAction( string action )
		{
			EditorAssemblyInterface.Instance?.QATAddAction( action );
		}

		public static event Action EditorFormLoaded;

		internal static void PerformEditorFormLoaded()
		{
			EditorFormLoaded?.Invoke();
		}

		public static ProcedureUI.Form CreateProcedureUIDialog( ProcedureUIDialogSettings settings )
		{
			return EditorAssemblyInterface.Instance.CreateProcedureUIDialog( settings );
		}

		public static void ShowDialog( ProcedureUI.Form form )
		{
			EditorAssemblyInterface.Instance.ShowDialog( form );
		}

		public static bool ShowOpenFileDialog( bool isFolderPicker, string initialDirectory, IEnumerable<(string rawDisplayName, string extensionList)> filters, IntPtr ownerWindowHandle, out string[] fileNames )
		{
			return EditorAssemblyInterface.Instance.ShowOpenFileDialog( isFolderPicker, initialDirectory, filters, ownerWindowHandle, out fileNames );
		}

		public static bool ShowOpenFileDialog( bool isFolderPicker, string initialDirectory, IEnumerable<(string rawDisplayName, string extensionList)> filters, IntPtr ownerWindowHandle, out string fileName )
		{
			return EditorAssemblyInterface.Instance.ShowOpenFileDialog( isFolderPicker, initialDirectory, filters, ownerWindowHandle, out fileName );
		}

		public static bool ShowSaveFileDialog( string initialDirectory, string initialFileName, string filter, IntPtr ownerWindowHandle, out string resultFileName )
		{
			return EditorAssemblyInterface.Instance.ShowSaveFileDialog( initialDirectory, initialFileName, filter, ownerWindowHandle, out resultFileName );
		}
	}
}
