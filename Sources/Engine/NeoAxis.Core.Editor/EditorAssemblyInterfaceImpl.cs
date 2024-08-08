// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using RoslynPad.Roslyn;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using NeoAxis.Import;
using Internal.ComponentFactory.Krypton.Toolkit;
using System.Linq;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace NeoAxis.Editor
{
	//NeoAxis.Core.Editor.dll is need to separate part of editor code, WinForms, WPF, RoslynPad classes from NeoAxis.Core.dll.

	class EditorAssemblyInterfaceImpl : EditorAssemblyInterface
	{
		public static void Init()
		{
			new EditorAssemblyInterfaceImpl();

			RoslynPadNeoAxisCoreExchangeImpl.Init();
		}

		/////////////////////////////////////////

		public override IScriptPrinter ScriptPrinterNew()
		{
			return new ScriptPrinter();// { BackgroundBrush = System.Windows.Media.Brushes.LightGray };
		}

		public override Dictionary<string, Type> DocumentWindowClassByFileExtension
		{
			get
			{
				return new Dictionary<string, Type>()
				{
					{ "txt", typeof( TextEditorDocumentWindow ) },
					{ "settings", typeof( TextEditorDocumentWindow ) },
					{ "config", typeof( TextEditorDocumentWindow ) },
					{ "info", typeof( TextEditorDocumentWindow ) },
					{ "log", typeof( TextEditorDocumentWindow ) },
					{ "json", typeof( TextEditorDocumentWindow ) },
					{ "h", typeof( TextEditorDocumentWindow ) },
					{ "c", typeof( TextEditorDocumentWindow ) },
					{ "cpp", typeof( TextEditorDocumentWindow ) },
					{ "cs", typeof( CSharpDocumentWindow ) }
				};
			}
		}

		public override void UpdateProjectFileForCSharpEditor( ICollection<string> addFiles, ICollection<string> removeFiles )
		{
			if( RoslynHost.Instance != null )
			{
				if( addFiles != null )
				{
					foreach( var file in addFiles )
					{
						string fullPath = file;
						if( !Path.IsPathRooted( fullPath ) )
							fullPath = Path.Combine( VirtualFileSystem.Directories.Project, fullPath );
						try
						{
							RoslynHost.Instance.OnAddCsFileToProject( fullPath );
						}
						catch { }
					}
				}
				if( removeFiles != null )
				{
					foreach( var file in removeFiles )
					{
						string fullPath = file;
						if( !Path.IsPathRooted( fullPath ) )
							fullPath = Path.Combine( VirtualFileSystem.Directories.Project, fullPath );
						try
						{
							RoslynHost.Instance.OnRemoveCsFileFromProject( fullPath );
						}
						catch { }
					}
				}
			}
		}

		public override void InitializeWPFApplicationAndScriptEditor()
		{
			if( System.Windows.Application.Current == null )
			{
				// create the Application object for WPF support
				new WPFApp() { ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown };
			}

			ScriptEditorEngine.Instance.Initialize();
		}

		public override Type GetTypeByName( string typeName )
		{
			return Assembly.GetExecutingAssembly().GetType( typeName );
		}

		public override void SetDarkTheme()
		{
			AvalonEditDarkThemeUtility.DarkTheme = true;
		}

		public override ITextEditorControl CreateTextEditorControl()
		{
			var control = new TextEditorControl();

			control.ManageColorsFromTextEditorProjectSettings = true;
			control.ManageFontFromTextEditorProjectSettings = true;

			control.InstallSearchReplacePanel();

			return control;
		}

		public override void ImportFBX( ImportGeneral.Settings settings, out string error )
		{
			Import.FBX.ImportFBX.DoImport( settings, out error );
		}

		public override void ImportAssimp( ImportGeneral.Settings settings, out string error )
		{
			Import.ImportAssimp.DoImport( settings, out error );
		}

		public override bool ExportToFBX( Mesh sourceMesh, string realFileName, out string error )
		{
			return MeshExportImport.ExportToFBX( sourceMesh, realFileName, out error );
		}

		//public override HCDropDownControl CreateColorValuePoweredSelectControl( HCItemProperty itemProperty )
		//{
		//	return new ColorValuePoweredSelectControl( itemProperty );
		//}

		//public override bool ColorValuePoweredSelectFormShowDialog( Point location, ColorValuePowered initialColor, out ColorValuePowered resultColor )
		//{
		//	var form = new ColorValuePoweredSelectForm();
		//	form.StartPosition = FormStartPosition.Manual;
		//	form.Location = location;
		//	form.Init( initialColor, false, false, null, false );

		//	if( form.ShowDialog( EditorForm.Instance ) == DialogResult.OK )
		//	{
		//		resultColor = form.CurrentValue;
		//		return true;
		//	}
		//	resultColor = ColorValuePowered.Zero;
		//	return false;
		//}


		public override EDialogResult ShowQuestion( string text, EMessageBoxButtons buttons, string caption = null )
		{
			return (EDialogResult)KryptonMessageBox.Show( text, caption ?? EngineInfo.OriginalName, (MessageBoxButtons)buttons, MessageBoxIcon.Question );
		}

		public override void ShowWarning( string text, string caption = null )
		{
			KryptonMessageBox.Show( text, caption ?? EngineInfo.OriginalName, MessageBoxButtons.OK, MessageBoxIcon.Warning );
		}

		public override void ShowInfo( string text, string caption = null )
		{
			KryptonMessageBox.Show( text, caption ?? EngineInfo.OriginalName, MessageBoxButtons.OK, MessageBoxIcon.Information );
		}

		//public override float GetDPI()
		//{
		//	return DpiHelper.Default.Dpi;
		//}

		//public override float GetDPIScale()
		//{
		//	return DpiHelper.Default.DpiScaleFactor;
		//}

		public override IDocumentInstance GetDocumentByComponent( Component component )
		{
			var parentRoot = component.ParentRoot;
			foreach( var document in EditorForm.Instance.Documents )
			{
				if( document.ResultComponent == parentRoot )
					return document;
			}
			return null;
		}

		public override IDocumentInstance GetDocumentByObject( object obj )
		{
			return EditorAPI2.GetDocumentByObject( obj );
		}

		public override string CreateEditorDocumentXmlConfiguration( IEnumerable<Component> components, Component selected = null )
		{
			return KryptonConfigGenerator.CreateEditorDocumentXmlConfiguration( components, selected );
		}

		public override bool GetDarkTheme()
		{
			return KryptonManager._globalPaletteMode == PaletteModeManager.NeoAxisBlack;
			//return EditorForm.Instance != null && EditorForm.Instance.DarkTheme;
		}

		public override string Translate( string group, string text )
		{
			return EditorLocalization2.Translate( group, text );
		}

		public override IDocumentWindow[] FindDocumentWindowsWithObject( object obj )
		{
			return EditorAPI2.FindDocumentWindowsWithObject( obj );
		}

		public override void CloseAllDocumentWindowsOnSecondLevelWithDeletedObjects( IDocumentInstance document )
		{
			EditorAPI2.CloseAllDocumentWindowsOnSecondLevelWithDeletedObjects( document );
		}

		public override ScreenNotifications.IStickyNotificationItem ShowScreenNotification( string text, bool error, bool sticky )
		{
			return ScreenNotifications2.Show( text, error, sticky ) as ScreenNotifications.IStickyNotificationItem;
		}

		public override void SelectDockWindow( IDockWindow window )
		{
			EditorAPI2.SelectDockWindow( (DockWindow)window );
		}

		public override IDocumentWindow OpenDocumentWindowForObject( IDocumentInstance document, object obj )
		{
			return EditorAPI2.OpenDocumentWindowForObject( (DocumentInstance)document, obj );
		}

		public override IEditorAction[] GetEditorActions()
		{
			return EditorActions.Actions.ToArray();
		}

		public override FlowGraphNodeStyle Get_FlowGraphNodeStyle_Rectangle_Instance()
		{
			return FlowGraphNodeStyle_Rectangle.Instance;
		}

		public override bool EditorCommandLineTools_PlatformProjectPatch_Process( string destFile, string baseProjectFileName, out string error, out bool changed )
		{
			error = "";

			//!!!!not implemented, not used
			changed = false;

			var exeFileName = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, "CommandLineTools\\CommandLineTools.exe" );
			var parameters = string.Format( "-platformProjectPatch \"{0}\" -baseProject \"{1}\"", destFile, baseProjectFileName );

			var processStartInfo = new ProcessStartInfo
			{
				FileName = exeFileName,
				Arguments = parameters,
				UseShellExecute = false,
				//CreateNoWindow = true
			};

			try
			{
				using( Process process = new Process() )
				{
					process.StartInfo = processStartInfo;

					process.Start();
					process.WaitForExit();

					int exitCode = process.ExitCode;
					if( exitCode != 0 )
					{
						error = "Exit code is " + exitCode.ToString() + ".";
						return false;
					}

					return true;
				}
			}
			catch( Exception ex )
			{
				error = ex.Message;
				return false;
			}

			//return EditorCommandLineTools.PlatformProjectPatch.Process( destFile, baseProjectFileName, out error, out changed );
		}

		public override void RegisterEditorAsembly( Assembly assembly, Type[] exportedTypes )
		{
			EditorUtility2.RegisterEditorExtensions( assembly, false );
			ResourcesWindowItems.RegisterAssembly( exportedTypes );
		}

		public override void UnregisterEditorAsembly( Assembly assembly )
		{
			EditorUtility2.RegisterEditorExtensions( assembly, true );
			ResourcesWindowItems.UnregisterAssembly( assembly );
		}

		public override IDocumentWindow[] GetAllDocumentWindowsOfDocument( IDocumentInstance document )
		{
			return EditorAPI2.GetAllDocumentWindowsOfDocument( (DocumentInstance)document );
		}

		public override void ContentBrowserUtility_AllContentBrowsers_SuspendChildrenChangedEvent()
		{
			ContentBrowserUtility.AllContentBrowsers_SuspendChildrenChangedEvent();
		}

		public override void ContentBrowserUtility_AllContentBrowsers_ResumeChildrenChangedEvent()
		{
			ContentBrowserUtility.AllContentBrowsers_ResumeChildrenChangedEvent();
		}

		public override IEditorAction GetEditorActionByName( string name )
		{
			return EditorActions.GetByName( name );
		}

		public override IEditorRibbonDefaultConfigurationTab[] GetEditorRibbonDefaultConfigurationTabs()
		{
			return EditorRibbonDefaultConfiguration.Tabs;//.ToArray();
		}

		public override void SurfaceEditorUtility_CreatePreviewObjects( Scene scene, Surface surface )
		{
			SurfaceEditorUtility.CreatePreviewObjects( scene, surface );
		}

		public override bool IsAnyTransformToolInModifyingMode()
		{
			if( EngineApp.IsEditor )
			{
				foreach( var instance in EngineViewportControl.AllInstances )
				{
					var transformTool = instance.TransformTool;
					if( transformTool != null && transformTool.Modifying )
						return true;
				}
			}

			return false;
		}

		public override void PreviewImagesManager_RegisterResourceType( string typeName )
		{
			PreviewImagesManager.RegisterResourceType( typeName );
		}

		public override void XmlDocumentationFiles_Load( string xmlFile )
		{
			XmlDocumentationFiles.Load( xmlFile );
		}

		public override void DocumentationLinksManager_AddNameByType( Type type, string name )
		{
			DocumentationLinksManager.AddNameByType( type, name );
		}

		public override void OpenSelectTypeWindow( SelectTypeWindowInitData initData )
		{
			var window = new SelectTypeWindow();
			window.initData = initData;
			EditorForm.Instance.WorkspaceController.AddDockWindow( window, true, true );
		}

		public override void Product_Store_ImageGenerator_WriteBitmapToStream( Stream writeStream, Product_Store.ImageGenerator.ImageFormat writeImageFormat, Vector2I imageSizeRender, Vector2I imageSizeOutput, IntPtr imageData )
		{
			const PixelFormat imageFormat = PixelFormat.A8R8G8B8;

			using( var bitmap = new Bitmap( imageSizeRender.X, imageSizeRender.Y, imageSizeRender.X * PixelFormatUtility.GetNumElemBytes( imageFormat ), System.Drawing.Imaging.PixelFormat.Format32bppArgb, imageData ) )
			{
				Bitmap ResizeImage( Image image, int width, int height )
				{
					Bitmap result = new Bitmap( width, height );
					using( Graphics g = Graphics.FromImage( result ) )
					{
						g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

						//downscale and clip
						var offsetX = (int)( (double)width * 0.05 );
						var offsetY = (int)( (double)height * 0.05 );
						g.DrawImage( image, -offsetX, -offsetY, width + offsetX * 2, height + offsetY * 2 );

						////fix borders
						//g.DrawImage( image, -2, -2, width + 4, height + 4 );
						////g.DrawImage( image, 0, 0, width, height );
					}
					return result;
				}

				using( var resized = ResizeImage( bitmap, imageSizeOutput.X, imageSizeOutput.Y ) )
				{
					if( writeImageFormat == Product_Store.ImageGenerator.ImageFormat.Png )
						resized.Save( writeStream, ImageFormat.Png );
					else if( writeImageFormat == Product_Store.ImageGenerator.ImageFormat.Jpeg )
					{
						var encoder = ImageCodecInfo.GetImageEncoders().First( codec => codec.FormatID == ImageFormat.Jpeg.Guid );
						var parameters = new EncoderParameters( 1 );
						parameters.Param[ 0 ] = new EncoderParameter( System.Drawing.Imaging.Encoder.Quality, 95L );
						resized.Save( writeStream, encoder, parameters );
					}

					//var ext = Path.GetExtension( writeRealFileName );
					//if( ext == ".png" )
					//	resized.Save( writeRealFileName, ImageFormat.Png );
					//else if( ext == ".jpg" )
					//{
					//	var encoder = ImageCodecInfo.GetImageEncoders().First( codec => codec.FormatID == ImageFormat.Jpeg.Guid );
					//	var parameters = new EncoderParameters( 1 );
					//	parameters.Param[ 0 ] = new EncoderParameter( System.Drawing.Imaging.Encoder.Quality, 95L );
					//	resized.Save( writeRealFileName, encoder, parameters );
					//}
				}
			}
		}

		public override EditorContextMenu.Item EditorContextMenuNewItem( string text, EventHandler clickHandler )
		{
			return new EditorContextMenu2.ItemImpl( text, clickHandler );
		}

		public override EditorContextMenu.Separator EditorContextMenuNewSeparator()
		{
			return new EditorContextMenu2.SeparatorImpl();
		}

		public override void EditorContextMenuShow( ICollection<EditorContextMenu.ItemBase> items, Vector2I? screenPosition )
		{
			if( items.Count == 0 )
				return;

			Vector2I screenPosition2;
			if( screenPosition.HasValue )
				screenPosition2 = screenPosition.Value;
			else
			{
				var p = Cursor.Position;
				screenPosition2 = new Vector2I( p.X, p.Y );
			}

			var realItems = new List<KryptonContextMenuItemBase>();
			foreach( var item in items )
			{
				KryptonContextMenuItemBase realItemBase = null;
				var itemImpl = item as EditorContextMenu2.ItemImpl;
				if( itemImpl != null )
					realItemBase = itemImpl.item;
				var separatorImpl = item as EditorContextMenu2.SeparatorImpl;
				if( separatorImpl != null )
					realItemBase = separatorImpl.item;

				realItems.Add( realItemBase );
			}

			var menu = new KryptonContextMenu();
			menu.Items.Add( new KryptonContextMenuItems( realItems.ToArray() ) );
			menu.Show( EditorForm.Instance, new Point( screenPosition2.X, screenPosition2.Y ) );
		}

		public override void AfterFatalShowDialogAndSaveDocuments( string errorText, ref bool skipLogFatal )
		{
			EditorForm.Instance?.AfterFatalShowDialogAndSaveDocuments( errorText, ref skipLogFatal );
		}
	}
}
