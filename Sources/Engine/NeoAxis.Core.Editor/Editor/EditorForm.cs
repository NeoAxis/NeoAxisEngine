#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Docking;
using Microsoft.Win32;
using Internal.ComponentFactory.Krypton.Ribbon;
using System.Diagnostics;
using NeoAxis.Networking;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the main editor form.
	/// </summary>
	public partial class EditorForm : KryptonForm
	{
		const string dockingConfigFileName = @"user:Configs\EditorDocking.config";
		const string dockingConfigFileNameDefault = @"Base\Tools\EditorDockingDefault.config";

		public static EditorForm instance;

		List<DocumentInstance> documents = new List<DocumentInstance>();

		bool canSaveConfig;
		//bool forceCloseForm;

		bool loaded;
		bool firstTick = true;
		bool firstActivate = true;

		//public static string needSelectResource;

		WorkspaceControllerForForm workspaceController;

		static bool qatInitialized;

		//internal float dpi;

		public string ribbonLastSelectedTabTypeByUser = "";
		public bool ribbonLastSelectedTabTypeByUser_DisableUpdate = true;

		ObjectsInFocus getObjectsInFocusLastResult;

		public bool needClose;

		Control coverControl;

		double lastSlowUpdatingTime;

		public DateTime? unlockFormUpdateInTimer;

		public int skipPaintCounter = 2;

		bool insideTimer;

		double configAutoSaveLastTime;

		/////////////////////////////////////////

		//internal class CoverControl : Control
		//{
		//	Label label;
		//	public CoverControl()
		//	{
		//		label = new Label() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
		//		Controls.Add( label );
		//		BackColor = Color.Black;
		//		Dock = DockStyle.Fill;
		//	}

		//	public void UpdateText( string text )
		//	{
		//		label.Text = text;
		//		Application.DoEvents();
		//	}
		//}

		/////////////////////////////////////////

		public EditorForm()
		{
			instance = this;

			//get CustomWindowsStyle project settings
			if( !WinFormsUtility.IsDesignerHosted( this ) )
			{
				var customizeWindowsStyle = ProjectSettings.ReadParameterDirectly( "General", "CustomizeWindowsStyle", "" );
				if( !string.IsNullOrEmpty( customizeWindowsStyle ) )
				{
					try
					{
						KryptonToolkitSettings.CustomizeWindowsStyle = (ProjectSettingsPage_General.CustomizeWindowsStyleEnum)SimpleTypes.ParseValue( typeof( ProjectSettingsPage_General.CustomizeWindowsStyleEnum ), customizeWindowsStyle );
					}
					catch { }
				}
			}
			AllowFormChrome = KryptonToolkitSettings.GetResultCustomizeWindowsStyle( true );

			////splash screen
			//var splashScreenAtStartup = ProjectSettings.ReadParameterFromFile( "SplashScreenAtStartup" );
			//if( string.IsNullOrEmpty( splashScreenAtStartup ) && !Debugger.IsAttached )
			//{
			//	var splashForm = new SplashForm();
			//	splashForm.Show();
			//}
			//Application.EnterThreadModal += delegate ( object sender, EventArgs e )
			//{
			//	SplashForm.Instance?.Close();
			//};

			InitializeComponent();
			Icon = Properties.Resources.EditorLogo;

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			////!!!!new
			//buttonQATFormIntegration.Enabled = DWM.IsCompositionEnabled && AllowFormChrome;
			//allowFormChromeButton.Enabled = KryptonManager.AllowFormChrome;

			workspaceController = new WorkspaceControllerForForm( kryptonPanel, this );

			//EnableLocalization();

			EditorAssemblyInterface.Instance.InitializeWPFApplicationAndScriptEditor();
			//InitializeWPFApplication();
			//InitializeScriptEditor();

			//!!!!!тут?
			//QATButtonsInit();

			//apply editor settings
			EditorSettingsSerialization.Init();
			EditorFavorites.Init();
		}

		//static void InitializeScriptEditor()
		//{
		//	if( EngineApp.IsEditor )
		//		ScriptEditorEngine.Instance.Initialize();
		//}

		//static void InitializeWPFApplication()
		//{
		//	if( System.Windows.Application.Current != null )
		//		return; // already initialized.

		//	// create the Application object for WPF support
		//	new WPFApp() { ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown };
		//}

		//is not used
		//static void ShutdownWPFApplication()
		//{
		//	if( System.Windows.Application.Current != null )
		//		System.Windows.Application.Current.Shutdown();
		//}

		public static EditorForm Instance
		{
			get { return instance; }
		}

		public static event Action RegisterAdditionalDockWindows;

		private void EditorForm_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			if( EngineApp.IsEditor )
				PackageManager.DeleteFilesAsStartup();

			//hide ribbon to avoid redrawing
			kryptonRibbon.Visible = false;

			// create cover
			coverControl = new Control();
			coverControl.BackColor = Color.FromArgb( 40, 40, 40 );
			coverControl.Dock = DockStyle.Fill;
			Controls.Add( coverControl );
			coverControl.BringToFront();
			EditorAPI2.ApplicationDoEvents( false );//Application.DoEvents();

			////dpi
			//try
			//{
			//	using( Graphics graphics = CreateGraphics() )
			//	{
			//		dpi = graphics.DpiX;
			//	}
			//}
			//catch( Exception ex )
			//{
			//	dpi = 96;
			//	Log.Warning( "EditorForm: CreateGraphics: Call failed. " + ex.Message );
			//}

			kryptonRibbon.RibbonTabs.Clear();

			StoreManager.Init();

			{
				EngineApp.InitSettings.UseApplicationWindowHandle = Handle;

				if( !EngineApp.Create() )
				{
					Log.Fatal( "EngineApp.Create() failed." );
					Close();
					return;
				}

				//эксепшен не генегируется, просто падает

				//bool created = false;

				//if( Debugger.IsAttached )
				//	created = EngineApp.Create();
				//else
				//{
				//	try
				//	{
				//		//!!!!
				//		Log.Info( "dd" );

				//		created = EngineApp.Create();

				//		//!!!!
				//		Log.Info( "tt" );

				//	}
				//	catch( Exception e2 )
				//	{
				//		//!!!!
				//		Log.Info( "ee" );

				//		Log.FatalAsException( e2.ToString() );
				//	}
				//}

				//if( !created )
				//{
				//	Log.Fatal( "EngineApp.Create() failed." );
				//	Close();
				//	return;
				//}

			}

			EngineApp.DefaultSoundChannelGroup.Volume = 0;

			EnableLocalization();
			PreviewImagesManager.Init();

			//set theme
			if( ProjectSettings.Get.General.Theme.Value == ProjectSettingsPage_General.ThemeEnum.Dark )
				KryptonManager.GlobalPaletteMode = PaletteModeManager.NeoAxisBlack;
			else
				KryptonManager.GlobalPaletteMode = PaletteModeManager.NeoAxisBlue;

			KryptonDarkThemeUtility.DarkTheme = EditorAPI2.DarkTheme;
			if( EditorAPI2.DarkTheme )
				EditorAssemblyInterface.Instance.SetDarkTheme();
			Internal.Aga.Controls.Tree.NodeControls.BaseTextControl.DarkTheme = EditorAPI2.DarkTheme;
			ApplyWindows11DarkTheme();

			BackColor = EditorAPI2.DarkTheme ? Color.FromArgb( 40, 40, 40 ) : Color.FromArgb( 240, 240, 240 );

			//app button
			kryptonRibbon.RibbonAppButton.AppButtonText = EditorLocalization2.Translate( "AppButton", kryptonRibbon.RibbonAppButton.AppButtonText );
			if( EditorAPI2.DarkTheme )
			{
				kryptonRibbon.RibbonAppButton.AppButtonBaseColorDark = Color.FromArgb( 40, 40, 40 );
				kryptonRibbon.RibbonAppButton.AppButtonBaseColorLight = Color.FromArgb( 54, 54, 54 );
			}

			//!!!! default editor layout:

			// IsSystemWindow = true for this:
			// для этих "системных" окон используется отдельная логика сериализации (окна создаются до загрузки конфига) 
			// и отдельная логика закрытия (hide/remove)
			workspaceController.AddToDockspaceStack( new DockWindow[] { new ObjectsWindow(), new SettingsWindow() }, DockingEdge.Right );
			//workspaceController.AddDockspace(new MembersWindow(), "Members", DockingEdge.Right, new Size(300, 300));

			workspaceController.AddToDockspaceStack( new DockWindow[] { new ResourcesWindow(), new SolutionExplorer(), new PreviewWindow(), new StoresWindow() }, DockingEdge.Left );
			//workspaceController.AddToDockspaceStack( new DockWindow[] { new StoresWindow() }, DockingEdge.Bottom );


			workspaceController.AddToDockspace( new DockWindow[] { new MessageLogWindow(), new OutputWindow(), new DebugInfoWindow() }, DockingEdge.Bottom );

			RegisterAdditionalDockWindows?.Invoke();

			Log.Info( "Use Log.Info(), Log.Warning() methods to write to the window. These methods can be used from the editor and from the Player app." );
			OutputWindow.Print( "Use OutputWindow.Print() method to write to the window. Unlike Message Log window this window is not a list, you can add text in arbitrary format.\n" );

			//load docking state
			{
				string configFile = VirtualPathUtility.GetRealPathByVirtual( dockingConfigFileName );
				//default settings
				if( !File.Exists( configFile ) )
					configFile = VirtualPathUtility.GetRealPathByVirtual( dockingConfigFileNameDefault );

				if( File.Exists( configFile ) )
				{
					//no try catch to save the ability to work with debugger. in case when error happens during loading one of documents

					//try
					//{

					workspaceController.LoadLayoutFromFile( configFile );

					//!!!!
					//hack. unhide the page to load it correctly. after loading the page will hided
					foreach( var page in workspaceController.DockingManager.Pages )
					{
						if( page.needHideAfterLoading )
						{
							page.needHideAfterLoading = false;

							var window = page.GetDockWindow();
							if( window != null )
								workspaceController.SetDockWindowVisibility( window, false );
						}
					}

					//}
					//catch( Exception e2 )
					//{
					//	var text = $"Error loading docking settings.\n\n" + e2.Message;
					//	Log.Warning( text );
					//	EditorMessageBox.ShowWarning( text );
					//}
				}
			}

			InitQAT();
			InitRibbon();

			UpdateText();

			//apply editor settings
			EditorSettingsSerialization.InitAfterFormLoad();

			XmlDocumentationFiles.PreloadBaseAssemblies();

			EditorAPI2.SelectedDocumentWindowChanged += EditorAPI_SelectedDocumentWindowChanged;

			UpdateRecentProjectsInRegistry();

#if CLOUD
			if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient )
			{
				RepositoryServerState.Init( VirtualFileSystem.Directories.AllFiles );
				RepositoryLocal.Init( VirtualFileSystem.Directories.AllFiles );
				RepositoryIconCache.Init( VirtualFileSystem.Directories.AllFiles, null );
			}
#endif

			LoginUtility.RequestFullLicenseInfo();

			kryptonRibbon.BeforeMinimizedModeChanged += KryptonRibbon_BeforeMinimizedModeChanged;
			kryptonRibbon.MinimizedModeChanged += KryptonRibbon_MinimizedModeChanged;

			KryptonWinFormsUtility.editorFormStartTemporaryLockUpdateAction = delegate ()
			{
				if( IsHandleCreated && !EditorAPI.ClosingApplication )
				{
					KryptonWinFormsUtility.LockFormUpdate( this );
					unlockFormUpdateInTimer = DateTime.Now + TimeSpan.FromSeconds( 0.1 );
				}
			};

			SplashForm.Instance?.Close();

			loaded = true;
		}

		void SaveDockingState()
		{
			if( canSaveConfig )
			{
				try
				{
					string configFile = VirtualPathUtility.GetRealPathByVirtual( dockingConfigFileName );
					EditorAPI2.GetRestartApplication( out _, out var resetWindowsSettings );
					if( resetWindowsSettings )
					{
						if( File.Exists( configFile ) )
							File.Delete( configFile );
					}
					else
					{
						if( !Directory.Exists( Path.GetDirectoryName( configFile ) ) )
							Directory.CreateDirectory( Path.GetDirectoryName( configFile ) );

						workspaceController.SaveLayoutToFile( configFile );

						// temp experimental:
						//foreach (var wnd in workspaceController.OfType<WorkspaceWindow>())
						//{
						//	string configFileName = string.Format(workspaceConfigFileName, wnd.Name);
						//	string workspaceConfigFile = VirtualPathUtils.GetRealPathByVirtual(configFileName);
						//	var controller = wnd.WorkspaceController;
						//	controller.SaveLayoutToFile(workspaceConfigFile);
						//}
					}

					//save part to Editor.config
					workspaceController.SaveAdditionalConfig();
				}
				catch( Exception e )
				{
					Log.Warning( "EditorForm: SaveDockingState: Exception: " + e.Message );
				}
			}
		}

		private void EditorForm_FormClosing( object sender, FormClosingEventArgs e )
		{
			if( ShowDialogAndSaveDocuments( workspaceController.GetDockWindows() ) )
			{
				e.Cancel = true;
				return;
			}

			SaveDockingState();

			EditorAPI.ClosingApplication = true;

#if CLOUD
			CloudProjectEnteringClient.AppDestroy();
			CloudProjectCommitClient.AppDestroy();
#endif

			EditorLocalization2.Shutdown();

			StoreManager.Shutdown();

			//destroy all documents
			{
				//!!!!может окна документов сначала удалить?
				foreach( var document in Documents.ToArray() )
					document.Destroy();
			}

			//destroy all viewport controls
			foreach( var control in EngineViewportControl.AllInstances.ToArray() )
				control.Dispose();

			PreviewImagesManager.Shutdown();

			EditorUtility2.PurgeCachedImages();

#if CLOUD
			if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient )
				RepositoryIconCache.Shutdown();
#endif

			if( !canSaveConfig )
				EngineApp.NeedSaveConfig = false;
			EngineApp.Shutdown();
		}

		// сохранить только выбранные окна. не обязательно все. например может использоваться
		// из метода "Закрыть все окна кроме текущего".
		//return: Cancel
		bool ShowDialogAndSaveDocuments( IEnumerable<DockWindow> windows )
		{
			var unsavedDocs = new List<DocumentInstance>();

			foreach( var wnd in windows )
			{
				if( wnd is IDocumentWindow docWnd && !docWnd.IsDocumentSaved() )
					unsavedDocs.Add( (DocumentInstance)docWnd.Document );
			}

			if( unsavedDocs.Count == 0 )
				return false;

			var text = EditorLocalization2.Translate( "General", "Save changes to the following files?" ) + "\n";
			foreach( var doc in unsavedDocs )
				text += "\n" + doc.Name;

			switch( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNoCancel ) )
			{
			case EDialogResult.Cancel:
				return true;
			case EDialogResult.Yes:
				//!!!!check error, return null
				unsavedDocs.ForEach( doc => doc.Save() );
				return false;
			case EDialogResult.No:
				return false;
			}

			return false;
		}

		//return: Cancel
		internal bool ShowDialogAndSaveDocument( DockWindow window )
		{
			var documentWindow = window as DocumentWindow;
			if( documentWindow == null )
				return false;

			// If the page is dirty then we need to ask if it should be saved
			if( documentWindow.IsMainWindowInWorkspace && !documentWindow.IsDocumentSaved() )
			{
				EDialogResult result;
				if( window.ShowDialogAndSaveDocumentAutoAnswer.HasValue )
					result = window.ShowDialogAndSaveDocumentAutoAnswer.Value;
				else
				{
					var text = EditorLocalization2.Translate( "General", "Save changes to the following files?" ) + "\n";
					text += "\n" + documentWindow.Document2.Name;
					result = EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNoCancel );
				}

				switch( result )
				{
				case EDialogResult.Cancel:

					//!!!!тут?
					EditorAPI2.SetRestartApplication( false );

					return true;
				case EDialogResult.Yes:
					//!!!!check error, return null
					documentWindow.SaveDocument();
					return false;
				case EDialogResult.No:
					return false;
				}
			}

			return false;
		}

		public static string ReplaceCaption { get; set; } = "";

		void UpdateText()
		{
			if( !string.IsNullOrEmpty( ReplaceCaption ) )
			{
				Text = ReplaceCaption;
				return;
			}

			string projectName = "";
			var c = ProjectSettings.Get;//.GetComponent<ProjectSettings_General>();
			if( c != null )
			{
				if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient )
					projectName = c.General.CloudProjectName.Trim();
				else
					projectName = c.General.ProjectName.Value.Trim();
			}

			var postFix = "";// " " + ( EngineApp.IsProPlan ? "Pro" : "Personal" );
							 //if( EngineInfo.ExtendedEdition )
							 //	postFix += " " + EditorLocalization.Translate( "General", "Extended" );

			if( projectName != "" )
				Text = projectName + " - " + EngineInfo.NameWithVersion + postFix;
			else
				Text = EngineInfo.NameWithVersion + postFix;
		}

		private void exitToolStripMenuItem_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void kryptonRibbon_SelectedTabChanged( object sender, EventArgs e )
		{
			if( !ribbonLastSelectedTabTypeByUser_DisableUpdate && loaded )
			{
				var tab = kryptonRibbon.SelectedTab?.Tag as EditorRibbonDefaultConfiguration.Tab;
				ribbonLastSelectedTabTypeByUser = tab != null ? tab.Type : "";
			}
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( !loaded )
				return;
			if( EditorAPI2.ApplicationDoEventsIsAdditionalCall )
				return;
			if( insideTimer )
				return;

			insideTimer = true;
			try
			{
				//!!!!!slowly?

				//!!!!!!

				//if( needSelectResource != null )
				//{
				//	var selectResource = needSelectResource;
				//	needSelectResource = null;
				//	//ContentObjectSettings.Instance.SelectResource( selectResource );
				//}

				//UpdatePagesVisibility();

				//!!!!!
				//!!!!где еще вызывать?
				if( EngineApp.Instance != null )
					EngineApp.DoTick();

				if( EngineApp.GetSystemTime() - lastSlowUpdatingTime > 0.2 && !firstTick )
				{
					QATButtonsUpdate();
					RibbonUpdate();
					UpdateText();

					lastSlowUpdatingTime = EngineApp.GetSystemTime();
				}

				//save editor settings
				EditorSettingsSerialization.Dump();

				ScreenNotifications2.Update();

				if( !needClose )
					EditorAPI2.SelectedDocumentWindowChangedUpdate();

				if( !needClose )
				{
					foreach( var document in EditorAPI2.Documents )
						document.EditorUpdateWhenDocumentModified_Tick();
				}

				//if( !needClose )
				//	PreviewImagesManager.Update();

				////!!!!temp
				//CheckRestartApplicationToApplyChanged();

				if( !needClose )
					UpdateSoundSystem();

				//save editor settings
				if( !needClose )
					KryptonAutoHiddenSlidePanel.Animate = false;//ProjectSettings.Get.General.AnimateWindowsAutoHiding;

				//if( !needClose )
				//	ScriptEditorEngine.Instance.UpdateSettings();

				UpdateVisibilityOfFloatingWindows();

				if( needClose )
				{
					needClose = false;
					Close();
				}

				//open file at startup
				if( firstTick && !needClose )
				{
					var realFileName = EditorSettingsSerialization.OpenFileAtStartup;
					EditorSettingsSerialization.OpenFileAtStartup = "";

					if( File.Exists( realFileName ) )
					{
						//select new file in Resources window
						EditorAPI2.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { realFileName } );

						//open file
						EditorAPI2.OpenFileAsDocument( realFileName, true, true );
					}
				}

				if( firstTick )
				{
					firstTick = false;

					//if( SplashForm.Instance != null )
					//	SplashForm.Instance.AllowClose = true;

					// hide cover show ribbon.
					kryptonRibbon.Visible = true;

					if( coverControl != null )
						Controls.Remove( coverControl );
					//coverControl.Visible = false;

					Invalidate( true );

					if( EditorSettingsSerialization.ShowTipsAsStartup )
						EditorAPI2.ShowTips();
				}

				if( unlockFormUpdateInTimer.HasValue && ( DateTime.Now - unlockFormUpdateInTimer.Value ).TotalSeconds > 0 )
				{
					KryptonWinFormsUtility.LockFormUpdate( null );
					unlockFormUpdateInTimer = null;
				}

				if( !needClose )
					EngineToolTipManager.Update();

				if( !needClose && canSaveConfig )
					ProcessConfigAutoSave();

				if( EditorAPI2.needShowProjectSettings )
				{
					EditorAPI2.needShowProjectSettings = false;
					EditorAPI2.ShowProjectSettings();
				}

#if CLOUD
				CloudProjectEnteringClient.Update();
				CloudProjectCommitClient.Update();
#endif

				canSaveConfig = true;
			}
			finally
			{
				insideTimer = false;
			}
		}

		[Browsable( false )]
		internal List<DocumentInstance> Documents
		{
			get { return documents; }
		}

		//void UpdatePagesVisibility()
		//{
		//	//!!!!!

		//	//update Preview
		//	if( SettingsWindow.Instance != null )
		//	{
		//		//!!!!!temp disabled

		//		//bool showPreview = false;

		//		//var panel = ObjectSettings.Instance.SelectedPanel;
		//		//if( panel != null )
		//		//{
		//		//	if( panel.settingsProviderGroup.showPreview )
		//		//		showPreview = true;
		//		//}

		//		////!!!!!работает на окна могут пропасть совсем. например, когда сбоку в режиме Auto Hide
		//		////!!!!!еще можно крикреплять окно от автозакрывания. иконка кнопки для прикрепления.

		//		//if( showPreview )
		//		//{
		//		//	previewPage.page.Show();
		//		//}
		//		//else
		//		//{
		//		//	previewPage.page.Hide();
		//		//}
		//	}
		//}

		private void kryptonRibbon_AppButtonMenuOpening( object sender, CancelEventArgs e )
		{
			backstageMenu1.SelectDefaultPage();
			backstageMenu1.Refresh();
		}

		public void OpenBackstage()
		{
			kryptonRibbon.ClickAppButton();
		}

		public void RenderViewports( out bool existActiveViewports )
		{
			existActiveViewports = false;

			//!!!!

			//!!!!каким-то не нужно часто обновляться

			if( Visible && WindowState != FormWindowState.Minimized )
			{
				//get available controls
				List<EngineViewportControl> toRender = new List<EngineViewportControl>();
				List<EngineViewportControl> unvisible = new List<EngineViewportControl>();

				foreach( var control in EngineViewportControl.AllInstances )
				{
					if( control.IsAllowRender() )
					{
						if( control.AutomaticUpdateFPS != 0 )
							toRender.Add( control );
					}
					else
						unvisible.Add( control );
				}

				bool existsToDispose = false;
				foreach( var control in unvisible )
				{
					var context = control.Viewport?.RenderingContext;
					if( context != null )
						existsToDispose = true;
				}

				if( existsToDispose )
				{
					RenderingSystem.CallBgfxFrame();
					RenderingSystem.CallBgfxFrame();
				}

				//destroy render targets for unvisible controls
				foreach( var control in unvisible )
				{
					var context = control.Viewport?.RenderingContext;
					if( context != null )
					{
						control.Viewport.RenderingContext = null;
						context.Dispose();
					}

					//var context = control.Viewport?.RenderingContext;
					//if( context != null )
					//{
					//	if( context.DynamicTexturesAreExists() )
					//	{
					//		context.MultiRenderTarget_DestroyAll();
					//		context.DynamicTexture_DestroyAll();

					//		callFrame = true;
					//	}
					//}
				}

				if( existsToDispose )
				{
					RenderingSystem.CallBgfxFrame();
					RenderingSystem.CallBgfxFrame();
				}

				//preview images
				if( EngineApp.Instance != null && EngineApp.Created )
				{
					PreviewImagesManager.Update();
					if( PreviewImagesManager.ExistsWorkingProcessors() )
						existActiveViewports = true;
				}

				//render
				if( toRender.Count != 0 )
				{
					existActiveViewports = true;

					foreach( var control in toRender )
						control.TryRender();
				}
			}

			//bool allowRender = MainForm.Instance.Visible &&
			//	MainForm.Instance.WindowState != FormWindowState.Minimized &&
			//	MainForm.Instance.IsAllowRenderScene();

			//internal bool IsAllowRenderScene()
			//{
			//	if( runMapProcess != null )
			//		return false;
			//	if( !timer1.Enabled )
			//		return false;

			//	Form activeForm = ActiveForm;
			//	if( activeForm == null )
			//		return false;

			//	string fullName = activeForm.GetType().FullName;
			//	if( fullName == "System.Windows.Forms.PropertyGridInternal.PropertyGridView+DropDownHolder" )
			//		return true;
			//	if( fullName == "WeifenLuo.WinFormsUI.Docking.FloatWindow" )
			//		return true;

			//	//MapCompositorManager form
			//	{
			//		string helper = activeForm.Tag as string;
			//		if( helper != null && helper == "MapCompositorManagerForm" )
			//			return true;
			//	}

			//	Form form = activeForm;
			//	while( form != null )
			//	{
			//		if( form == this )
			//			return true;
			//		if( form.Modal )
			//			return false;
			//		form = form.Owner;
			//	}

			//	return false;
			//}


		}

		////!!!!temp
		//public static bool checkRestartApplicationToApplyChangedNeedCheck;
		//bool checkRestartApplicationToApplyChangedInside;
		//void CheckRestartApplicationToApplyChanged()
		//{
		//	if( !checkRestartApplicationToApplyChangedNeedCheck )
		//		return;

		//	if( checkRestartApplicationToApplyChangedInside )
		//		return;
		//	checkRestartApplicationToApplyChangedInside = true;

		//	checkRestartApplicationToApplyChangedNeedCheck = false;

		//	//!!!!
		//	//ScreenNotifications.Show( "(TEMP) Restart application to affect other resources.", true );

		//	//{
		//	//	string text = "Unimplemented feature.\n\n";
		//	//	text += "The automatic resource updating after making changes at this time is not supported. Need restart application.\r\r";
		//	//	text += "Restart?";

		//	//	var result = EditorMessageBox.ShowQuestion( text, MessageBoxButtons.YesNo );
		//	//	if( result == DialogResult.Yes )
		//	//	{
		//	//		EditorProgram.needRestartApplication = true;
		//	//		EditorForm.Instance.Close();
		//	//	}
		//	//}

		//	checkRestartApplicationToApplyChangedInside = false;
		//}

		[Browsable( false )]
		public WorkspaceControllerForForm WorkspaceController
		{
			get { return workspaceController; }
		}

		internal ObjectsInFocus GetObjectsInFocus( bool useOnlySelectedDocumentWindow = false )
		{
			ObjectsInFocus newResult = null;

			//get DockWindow
			var dockWindow = workspaceController.GetSelectedDockWindow();

			bool disableChange = false;
			if( dockWindow != null && dockWindow is PreviewWindow )
				disableChange = true;

			if( !disableChange )
			{
				//reset not document windows if necessary
				if( useOnlySelectedDocumentWindow && dockWindow as DocumentWindow == null )
					dockWindow = null;

				//get objects in focus in dock window
				if( dockWindow != null )
					newResult = dockWindow.GetObjectsInFocus();

				//get selected or last selected DocumentWindow
				if( newResult == null && EditorAPI2.SelectedDocumentWindow != null )
				{
					//!!!!new
					newResult = new ObjectsInFocus( EditorAPI2.SelectedDocumentWindow, EditorAPI2.SelectedDocumentWindow.SelectedObjects );
					//newResult = new ObjectsInFocus( EditorAPI.SelectedDocumentWindow, new object[ 0 ] );
				}

				//update getObjectsInFocusLastResult
				if( newResult != null )
					getObjectsInFocusLastResult = newResult;
				else
				{
					if( getObjectsInFocusLastResult != null )
					{
						var window = (DocumentWindow)getObjectsInFocusLastResult.DocumentWindow;
						if( window != null && !window.IsHandleCreated )
							getObjectsInFocusLastResult = null;
					}
				}
			}

			if( getObjectsInFocusLastResult != null )
				return getObjectsInFocusLastResult;
			else
				return new ObjectsInFocus( null, new object[ 0 ] );
		}

		void UpdateSoundSystem()
		{
			//listener
			{
				var documentWithViewport = EditorAPI2.SelectedDocumentWindow as DocumentWindowWithViewport;
				var viewport = documentWithViewport?.ViewportControl2?.Viewport;

				var scene = viewport?.AttachedScene;
				if( scene != null && scene.EnabledInHierarchy )
				{
					var settings = viewport.CameraSettings;
					SoundWorld.SetListener( scene, settings.Position, Vector3.Zero, settings.Rotation );
				}
				else
					SoundWorld.SetListenerReset();
			}

			//volume
			EngineApp.DefaultSoundChannelGroup.Volume = ProjectSettings.Get.General.SoundVolume;
		}

		private void EditorAPI_SelectedDocumentWindowChanged()
		{
			var window = EditorAPI2.SelectedDocumentWindow;
			if( window != null )
				window.SettingsWindowSelectObjects();
		}

		void UpdateRecentProjectsInRegistry()
		{
#if !DEPLOY
			string recentProjects = "";
			try
			{
				using( var key = Registry.CurrentUser.OpenSubKey( "Software\\NeoAxis" ) )
				{
					if( key != null )
					{
						var obj = key.GetValue( "RecentProjects" );
						if( obj != null )
							recentProjects = obj.ToString();
					}
				}
			}
			catch { }

			string currentProject = VirtualFileSystem.Directories.Project;

			var paths = recentProjects.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries );
			var path2 = new List<string>( paths.Where( p => p != currentProject ) );
			path2.Insert( 0, currentProject );

			string newValue = "";
			foreach( var path in path2 )
			{
				if( newValue != "" )
					newValue += "|";
				newValue += path;
			}

			try
			{
				using( var key = Registry.CurrentUser.CreateSubKey( "Software\\NeoAxis" ) )
				{
					if( key != null )
						key.SetValue( "RecentProjects", newValue );
				}
			}
			catch { }
#endif
		}

		private void EditorForm_Activated( object sender, EventArgs e )
		{
			if( firstActivate )
			{
				if( !needClose )
					EditorAPI2.SelectedDocumentWindowChangedUpdate();
			}

			firstActivate = false;
		}

		//[Browsable( false )]
		//internal bool DarkTheme
		//{
		//	get { return kryptonManager.GlobalPaletteMode == PaletteModeManager.NeoAxisBlack; }
		//}

		void EnableLocalization()
		{
			var language = ProjectSettings.Get.General.Language.Value.ToString();
			if( !string.IsNullOrEmpty( language ) && language != "English" && language != "New" )
				EditorLocalization2.Init( language, true );

			var strings = kryptonRibbon.RibbonStrings;
			strings.CustomizeQuickAccessToolbar = EditorLocalization2.Translate( "General", strings.CustomizeQuickAccessToolbar );
			strings.ShowBelowRibbon = EditorLocalization2.Translate( "General", strings.ShowBelowRibbon );
			strings.ShowAboveRibbon = EditorLocalization2.Translate( "General", strings.ShowAboveRibbon );
			strings.Minimize = EditorLocalization2.Translate( "General", strings.Minimize );

			var menus = kryptonDockableWorkspace.ContextMenus;
			menus.TextClose = EditorLocalization2.Translate( "Docking", menus.TextClose );
			menus.TextCloseAllButThis = EditorLocalization2.Translate( "Docking", menus.TextCloseAllButThis );
			menus.TextMovePrevious = EditorLocalization2.Translate( "Docking", menus.TextMovePrevious );
			menus.TextMoveNext = EditorLocalization2.Translate( "Docking", menus.TextMoveNext );
			menus.TextSplitHorizontal = EditorLocalization2.Translate( "Docking", menus.TextSplitHorizontal );
			menus.TextSplitVertical = EditorLocalization2.Translate( "Docking", menus.TextSplitVertical );
			menus.TextRebalance = EditorLocalization2.Translate( "Docking", menus.TextRebalance );
			menus.TextMaximize = EditorLocalization2.Translate( "Docking", menus.TextMaximize );
			menus.TextRestore = EditorLocalization2.Translate( "Docking", menus.TextRestore );

			var strings2 = workspaceController.DockingManager.Strings;
			strings2.TextTabbedDocument = EditorLocalization2.Translate( "Docking", strings2.TextTabbedDocument );
			strings2.TextAutoHide = EditorLocalization2.Translate( "Docking", strings2.TextAutoHide );
			strings2.TextClose = EditorLocalization2.Translate( "Docking", strings2.TextClose );
			strings2.TextCloseAllButThis = EditorLocalization2.Translate( "Docking", strings2.TextCloseAllButThis );
			strings2.TextDock = EditorLocalization2.Translate( "Docking", strings2.TextDock );
			strings2.TextFloat = EditorLocalization2.Translate( "Docking", strings2.TextFloat );
			strings2.TextHide = EditorLocalization2.Translate( "Docking", strings2.TextHide );
			strings2.TextWindowLocation = EditorLocalization2.Translate( "Docking", strings2.TextWindowLocation );
		}

		bool ContainsNotFloatingWindows( DocumentInstance document )
		{
			foreach( var documentWindow in EditorAPI2.GetAllDocumentWindowsOfDocument( document ) )
			{
				bool foundFloating = false;

				Control c = documentWindow;
				while( c != null )
				{
					if( c is KryptonFloatingWindow )
					{
						foundFloating = true;
						break;
					}

					c = c.Parent;
				}

				if( !foundFloating )
					return true;
			}

			return false;
		}

		void UpdateVisibilityOfFloatingWindows()
		{
			try
			{
				foreach( var form in Application.OpenForms )
				{
					if( form is KryptonFloatingWindow floatingWindow )
					{
						bool v = true;

						var space = floatingWindow.FloatspaceControl;
						if( space != null )
						{
							var pages = space.AllPages().Where( p => p.LastVisibleSet ).ToArray();

							if( pages.Length == 1 )
							{
								foreach( var page in pages )
								{
									var dockWindow = page.GetDockWindow();

									var documentWindow = dockWindow as DocumentWindow;
									if( documentWindow != null )
									{
										var document = documentWindow.Document2;
										if( document != null && !string.IsNullOrEmpty( document.RealFileName ) && ContainsNotFloatingWindows( document ) )
										{
											if( EditorAPI2.SelectedDocument != null && EditorAPI2.SelectedDocument != document )
												v = false;
										}
									}
								}
							}
							else if( pages.Length == 0 )
								v = false;
						}

						if( BackstageMenu.BackstageVisible )
							v = false;
						if( WindowState == FormWindowState.Minimized )
							v = false;

						if( floatingWindow.Visible != v )
							floatingWindow.Visible = v;
					}
				}
			}
			catch { }
		}

		private void KryptonRibbon_BeforeMinimizedModeChanged( object sender, EventArgs e )
		{
			KryptonWinFormsUtility.LockFormUpdate( this );
		}

		private void KryptonRibbon_MinimizedModeChanged( object sender, EventArgs e )
		{
			NeedRecreateRibbonButtons();
		}

		protected override void WndProc( ref Message m )
		{
			var processed = false;

			if( !IsDisposed && !Disposing )
			{
				switch( m.Msg )
				{
				case Internal.ComponentFactory.Krypton.Toolkit.PI.WM_ERASEBKGND:
				case Internal.ComponentFactory.Krypton.Toolkit.PI.WM_PAINT:
					if( skipPaintCounter > 0 )
					{
						using( var brush = new SolidBrush( Color.FromArgb( 40, 40, 40 ) ) )
						using( var graphics = CreateGraphics() )
							graphics.FillRectangle( brush, new System.Drawing.Rectangle( 0, 0, Width, Height ) );

						processed = true;

						if( m.Msg == Internal.ComponentFactory.Krypton.Toolkit.PI.WM_PAINT && skipPaintCounter > 0 )
							skipPaintCounter--;
					}
					break;
				}
			}

			if( !processed )
				base.WndProc( ref m );
		}





		///////////////////////////////////////////////
		//QAT

		ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass qatUpdatedForConfiguration;
		bool needRecreateQATButtons;

		//

		void AddActionToQAT( EditorAction action )
		{
			var button = new KryptonRibbonQATButton();
			//action.createdQATButton = button;
			button.Enabled = false;

			button.Image = EditorAPI2.GetImageForDispalyScale( action.GetImageSmall(), action.GetImageBig() );
			button.Tag = action;
			button.Text = action.Name;

			//set tool tip
			var toolTip = EditorLocalization2.Translate( "EditorAction.Name", action.Name );
			if( action.Description != "" )
				toolTip += "\n" + EditorLocalization2.Translate( "EditorAction.Description", action.Description );

			//default shortcuts
			var shortcutKeys = action.ShortcutKeys;

			//changed shortcuts
			if( !ProjectSettings.Get.Shortcuts.ShortcutSettings.UseDefaultSettings )
			{
				var newList = new List<ProjectSettingsPage_Shortcuts.Keys2>();
				var actionItem = ProjectSettings.Get.Shortcuts.ShortcutSettings.GetActionItem( action.Name );
				if( actionItem != null )
				{
					if( actionItem.Shortcut1 != ProjectSettingsPage_Shortcuts.Keys2.None )
						newList.Add( actionItem.Shortcut1 );
					if( actionItem.Shortcut2 != ProjectSettingsPage_Shortcuts.Keys2.None )
						newList.Add( actionItem.Shortcut2 );
				}
				shortcutKeys = newList.ToArray();
			}

			var keysString = EditorActions.ConvertShortcutKeysToString( shortcutKeys );// action.ShortcutKeys );
			if( keysString != "" )
				toolTip += " (" + keysString + ")";
			button.ToolTipBody = toolTip;

			if( action.ActionType == EditorAction.ActionTypeEnum.DropDown )
			{
				button.IsDropDownButton = true;
				button.KryptonContextMenu = action.DropDownContextMenu;
			}

			kryptonRibbon.QATButtons.Add( button );

			button.Click += QATButton_Click;
		}

		void QATButton_Click( object sender, EventArgs e )
		{
			var button = (KryptonRibbonQATButton)sender;

			var action = button.Tag as EditorAction;
			if( action != null )
				EditorAPI2.EditorActionClick( EditorActionHolder.RibbonQAT, action.Name );
		}

		void InitQAT()
		{
			qatInitialized = true;

			QATButtonsUpdate();
		}

		void QATButtonsUpdate()
		{
			if( qatInitialized )
			{
				bool needRepaint = false;
				IgnoreRepaint = true;

				try
				{
					QATButtonsCheckForRecreate( ref needRepaint );
					QATButtonsUpdateProperties( ref needRepaint );
				}
				finally
				{
					IgnoreRepaint = false;
					if( needRepaint )
						PerformNeedPaint( true );
				}
			}
		}

		void QATButtonsCheckForRecreate( ref bool needRepaint )
		{
			var config = ProjectSettings.Get.RibbonAndToolbar.RibbonAndToolbarActions;
			if( qatUpdatedForConfiguration == null || !config.Equals( qatUpdatedForConfiguration ) || needRecreateQATButtons )
			{
				qatUpdatedForConfiguration = config.Clone();
				needRecreateQATButtons = false;

				kryptonRibbon.QATButtons.Clear();

				foreach( var actionItem in config.ToolbarActions )
				{
					var action = EditorActions.GetByName( actionItem.Name );

					if( action != null && !action.CompletelyDisabled && action.QatSupport && actionItem.Enabled && EditorUtility.PerformEditorActionVisibleFilter( action ) )
						AddActionToQAT( action );
				}

				needRepaint = true;
			}
		}

		void QATButtonsUpdateProperties( ref bool needRepaint )
		{
			foreach( KryptonRibbonQATButton button in kryptonRibbon.QATButtons )
			{
				if( button.Visible )
				{
					var action = button.Tag as EditorAction;
					if( action != null )
					{
						var state = EditorAPI2.EditorActionGetState( EditorActionHolder.RibbonQAT, action );
						if( button.Enabled != state.Enabled )
						{
							button.Enabled = state.Enabled;
							needRepaint = true;
						}
						if( button.Checked != state.Checked )
						{
							button.Checked = state.Checked;
							needRepaint = true;
						}
					}
				}
			}
		}




		///////////////////////////////////////////////
		//Ribbon

		ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass ribbonUpdatedForConfiguration;
		bool needRecreateRibbonButtons;

		ESet<string> tabUsedKeyTips = new ESet<string>( new string[] { "P" } );

		//

		string alphabetNumbers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

		string GetTabKeyTip( string name )
		{
			foreach( var c in name + alphabetNumbers )
			{
				var s = c.ToString().ToUpper();
				if( s != " " && !tabUsedKeyTips.Contains( s ) )
				{
					tabUsedKeyTips.AddWithCheckAlreadyContained( s );
					return s;
				}
			}
			return "";
		}

		void RibbonSubGroupAddItemsRecursive( EditorRibbonDefaultConfiguration.Group subGroup, KryptonContextMenuCollection contextMenuItems )
		{
			var items = new List<KryptonContextMenuItemBase>();

			foreach( var child in subGroup.Children )
			{
				//!!!!impl
				////sub group
				//var subGroup = child as EditorRibbonDefaultConfiguration.Group;
				//if( subGroup != null )
				//{
				//	var tripple = new KryptonRibbonGroupTriple();
				//	ribbonGroup.Items.Add( tripple );

				//	var button = new KryptonRibbonGroupButton();
				//	//button.Tag = action;
				//	button.TextLine1 = subGroup.DropDownGroupText.Item1;
				//	button.TextLine2 = subGroup.DropDownGroupText.Item2;
				//	//button.ImageSmall = action.imageSmall;
				//	button.ImageLarge = subGroup.DropDownGroupImage;
				//	button.ToolTipBody = subGroup.Name;
				//	button.ButtonType = GroupButtonType.DropDown;

				//	button.KryptonContextMenu = new KryptonContextMenu();
				//	RibbonSubGroupAddItemsRecursive( subGroup, button.KryptonContextMenu.Items );

				//	tripple.Items.Add( button );
				//}

				//action
				var action = child as EditorAction;
				if( action == null )
				{
					var actionName = child as string;
					if( actionName != null )
						action = EditorActions.GetByName( actionName );
				}
				if( action != null )
				{
					if( !action.CompletelyDisabled )
					{
						EventHandler clickHandler = delegate ( object s, EventArgs e2 )
						{
							var item2 = (KryptonContextMenuItem)s;

							var action2 = item2.Tag as EditorAction;
							if( action2 != null )
								EditorAPI2.EditorActionClick( EditorActionHolder.RibbonQAT, action2.Name );
						};

						var item = new KryptonContextMenuItem( action.GetContextMenuText(), null, clickHandler );
						//var item = new KryptonContextMenuItem( action.GetContextMenuText(), action.imageSmall, clickHandler );
						item.Tag = action;
						items.Add( item );
					}
				}
				//separator
				else if( child == null )
					items.Add( new KryptonContextMenuSeparator() );
			}

			if( items.Count != 0 )
				contextMenuItems.Add( new KryptonContextMenuItems( items.ToArray() ) );
		}

		void InitRibbon()
		{
			kryptonRibbon.RibbonStrings.AppButtonKeyTip = "P";

			RibbonUpdate();
		}

		private void Button_Click( object sender, EventArgs e )
		{
			var control = (KryptonRibbonGroupButton)sender;

			var action = control.Tag as EditorAction;
			if( action != null )
				EditorAPI2.EditorActionClick( EditorActionHolder.RibbonQAT, action.Name );
		}

		private void Slider_ValueChanged( object sender, EventArgs e )
		{
			var control = (KryptonRibbonGroupSliderControl)sender;

			var action = control.Tag as EditorAction;
			if( action != null )
			{
				action.Slider.Value = control.GetValue();
				EditorAPI2.EditorActionClick( EditorActionHolder.RibbonQAT, action.Name );
			}
		}

		//private void ComboBox_SelectedIndexChanged( object sender, EventArgs e )
		//{
		//	var control = (KryptonRibbonGroupComboBox)sender;

		//	var action = control.Tag as EditorAction;
		//	if( action != null )
		//	{
		//		action.ComboBox.SelectedIndex = control.SelectedIndex;
		//		EditorAPI.EditorActionClick( EditorAction.HolderEnum.RibbonQAT, action.Name );
		//	}
		//}

		private void ListBrowser_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			var action = sender.Tag as EditorAction;
			if( action != null )
			{
				ContentBrowser.Item[] selectedItems;
				if( action.ListBox.Mode == EditorAction.ListBoxSettings.ModeEnum.Tiles )
					selectedItems = sender.SelectedItemsOnlyListView;
				else
					selectedItems = sender.SelectedItems;

				if( selectedItems.Length != 0 )
					action.ListBox.SelectedIndex = (int)selectedItems[ 0 ].Tag;
				else
					action.ListBox.SelectedIndex = -1;

				action.ListBox.LastSelectedIndexChangedByUser = selectedByUser;

				EditorAPI2.EditorActionClick( EditorActionHolder.RibbonQAT, action.Name );
			}
		}

		private void ListBrowser_ShowContextMenuEvent( ContentBrowser sender, ContentBrowser.Item contentItem, List<KryptonContextMenuItemBase> items )
		{
			var action = sender.Tag as EditorAction;
			if( action != null )
			{
				ContentBrowser.Item[] selectedItems;
				if( action.ListBox.Mode == EditorAction.ListBoxSettings.ModeEnum.Tiles )
					selectedItems = sender.SelectedItemsOnlyListView;
				else
					selectedItems = sender.SelectedItems;

				if( selectedItems.Length != 0 )
					action.ListBox.SelectedIndex = (int)selectedItems[ 0 ].Tag;
				else
					action.ListBox.SelectedIndex = -1;

				action.ListBox.LastSelectedIndexChangedByUser = true;//selectedByUser;

				EditorAPI2.EditorActionClick2( EditorActionHolder.RibbonQAT, action.Name );
			}
		}

		void RibbonButtonsCheckForRecreate()
		{
			var config = ProjectSettings.Get.RibbonAndToolbar.RibbonAndToolbarActions;
			if( ribbonUpdatedForConfiguration == null || !config.Equals( ribbonUpdatedForConfiguration ) || needRecreateRibbonButtons )
			{
				ribbonUpdatedForConfiguration = config.Clone();
				needRecreateRibbonButtons = false;

				ribbonLastSelectedTabTypeByUser_DisableUpdate = true;

				kryptonRibbon.RibbonTabs.Clear();

				foreach( var tabSettings in ProjectSettings.Get.RibbonAndToolbar.RibbonAndToolbarActions.RibbonTabs )
				{
					if( !tabSettings.Enabled )
						continue;

					//can be null
					EditorRibbonDefaultConfiguration.Tab tab = null;
					if( tabSettings.Type == ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem.TypeEnum.Basic )
						tab = EditorRibbonDefaultConfiguration.GetTab( tabSettings.Name );

					var ribbonTab = new KryptonRibbonTab();
					ribbonTab.Tag = tab;

					if( tabSettings.Type == ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem.TypeEnum.Basic )
						ribbonTab.Text = EditorLocalization2.Translate( "Ribbon.Tab", tabSettings.Name );
					else
						ribbonTab.Text = tabSettings.Name;

					ribbonTab.KeyTip = GetTabKeyTip( tabSettings.Name );

					kryptonRibbon.RibbonTabs.Add( ribbonTab );

					var usedKeyTips = new ESet<string>();

					string GetKeyTip( string name )
					{
						foreach( var c in name + alphabetNumbers )
						{
							var s = c.ToString().ToUpper();
							if( s != " " && !usedKeyTips.Contains( s ) )
							{
								usedKeyTips.AddWithCheckAlreadyContained( s );
								return s;
							}
						}
						return "";
					}

					foreach( var groupSettings in tabSettings.Groups )
					{
						if( !groupSettings.Enabled )
							continue;

						var ribbonGroup = new KryptonRibbonGroup();

						if( groupSettings.Type == ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem.TypeEnum.Basic )
							ribbonGroup.TextLine1 = EditorLocalization2.Translate( "Ribbon.Group", groupSettings.Name );
						else
							ribbonGroup.TextLine1 = groupSettings.Name;

						ribbonGroup.DialogBoxLauncher = false;//!!!!для группы Transform можно было бы в настройки снеппинга переходить
															  //ribbonTab.Groups.Add( ribbonGroup );

						foreach( var groupSettingsChild in groupSettings.Actions )
						{
							if( !groupSettingsChild.Enabled )
								continue;

							//sub group
							if( groupSettingsChild.Type == ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem.TypeEnum.SubGroupOfActions )
							{
								EditorRibbonDefaultConfiguration.Group subGroup = null;
								if( tab != null )
								{
									var group = tab.Groups2.Find( g => g.Name == groupSettings.Name );
									if( group != null )
									{
										foreach( var child in group.Children )
										{
											var g = child as EditorRibbonDefaultConfiguration.Group;
											if( g != null && g.Name == groupSettingsChild.Name )
											{
												subGroup = g;
												break;
											}
										}
									}
								}

								if( subGroup != null && !subGroup.AreAllChildrenCompletelyDisabled() )
								{
									var tripple = new KryptonRibbonGroupTriple();
									ribbonGroup.Items.Add( tripple );

									var button = new KryptonRibbonGroupButton();
									button.Tag = "SubGroup";
									//button.Tag = action;

									var str = subGroup.DropDownGroupText.Item1;
									if( subGroup.DropDownGroupText.Item2 != "" )
										str += "\n" + subGroup.DropDownGroupText.Item2;

									var str2 = EditorLocalization2.Translate( "Ribbon.Action", str );
									var strs = str2.Split( new char[] { '\n' } );

									button.TextLine1 = strs[ 0 ];
									if( strs.Length > 1 )
										button.TextLine2 = strs[ 1 ];

									//button.TextLine1 = subGroup.DropDownGroupText.Item1;
									//button.TextLine2 = subGroup.DropDownGroupText.Item2;

									if( subGroup.DropDownGroupImageSmall != null )
										button.ImageSmall = subGroup.DropDownGroupImageSmall;
									else if( subGroup.DropDownGroupImageLarge != null )
										button.ImageSmall = EditorAction.ResizeImage( subGroup.DropDownGroupImageLarge, 16, 16 );
									button.ImageLarge = subGroup.DropDownGroupImageLarge;

									//EditorLocalization.Translate( "EditorAction.Description",

									if( !string.IsNullOrEmpty( subGroup.DropDownGroupDescription ) )
										button.ToolTipBody = EditorLocalization2.Translate( "EditorAction.Description", subGroup.DropDownGroupDescription );
									else
										button.ToolTipBody = subGroup.Name;

									button.ButtonType = GroupButtonType.DropDown;
									button.ShowArrow = subGroup.ShowArrow;

									button.KryptonContextMenu = new KryptonContextMenu();
									RibbonSubGroupAddItemsRecursive( subGroup, button.KryptonContextMenu.Items );

									tripple.Items.Add( button );
								}
							}

							//action
							if( groupSettingsChild.Type == ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem.TypeEnum.Action )
							{
								var action = EditorActions.GetByName( groupSettingsChild.Name );

								if( action != null && !action.CompletelyDisabled )
								{
									if( action.ActionType == EditorAction.ActionTypeEnum.Button || action.ActionType == EditorAction.ActionTypeEnum.DropDown )
									{
										//Button, DropDown

										var tripple = new KryptonRibbonGroupTriple();
										ribbonGroup.Items.Add( tripple );

										var control = new KryptonRibbonGroupButton();

										//!!!!
										//control.ImageSmall = NeoAxis.Properties.Resources.Android_16;

										control.Tag = action;

										var str = action.RibbonText.Item1;
										if( action.RibbonText.Item2 != "" )
											str += "\n" + action.RibbonText.Item2;

										var str2 = EditorLocalization2.Translate( "Ribbon.Action", str );
										var strs = str2.Split( new char[] { '\n' } );

										control.TextLine1 = strs[ 0 ];
										if( strs.Length > 1 )
											control.TextLine2 = strs[ 1 ];

										//control.TextLine1 = action.RibbonText.Item1;
										//control.TextLine2 = action.RibbonText.Item2;

										control.ImageSmall = action.GetImageSmall();
										control.ImageLarge = action.GetImageBig();
										control.ToolTipBody = action.GetToolTip();
										control.KeyTip = GetKeyTip( action.Name );

										//_buttonType = GroupButtonType.Push;
										//_toolTipImageTransparentColor = Color.Empty;
										//_toolTipTitle = string.Empty;
										//_toolTipBody = string.Empty;
										//_toolTipStyle = LabelStyle.SuperTip;

										if( action.ActionType == EditorAction.ActionTypeEnum.DropDown )
										{
											control.ButtonType = GroupButtonType.DropDown;
											control.KryptonContextMenu = action.DropDownContextMenu;
										}

										control.Click += Button_Click;

										tripple.Items.Add( control );
									}
									else if( action.ActionType == EditorAction.ActionTypeEnum.Slider )
									{
										//Slider

										var tripple = new KryptonRibbonGroupTriple();
										ribbonGroup.Items.Add( tripple );

										var control = new KryptonRibbonGroupSlider();
										control.Tag = action;
										control.ToolTipBody = action.GetToolTip();

										control.Control.Size = new System.Drawing.Size( (int)( (float)control.Control.Size.Width * EditorAPI2.DPIScale ), control.Control.Size.Height );
										control.Control.kryptonSplitContainer2.Size = new System.Drawing.Size( (int)( (float)control.Control.kryptonSplitContainer2.Size.Width * EditorAPI2.DPIScale ), control.Control.Size.Height );
										control.Control.kryptonSplitContainer2.Panel1MinSize = (int)( (float)control.Control.kryptonSplitContainer2.Panel1MinSize * EditorAPI2.DPIScale );
										control.Control.kryptonSplitContainer1.Panel2MinSize = (int)( (float)control.Control.kryptonSplitContainer1.Panel2MinSize * EditorAPI2.DPIScale );
										control.Control.kryptonSplitContainer1.SplitterDistance = 10000;

										control.Control.kryptonLabel1.Text = EditorLocalization2.Translate( "Ribbon.Action", action.RibbonText.Item1 );
										control.Control.Init( action.Slider.Minimum, action.Slider.Maximum, action.Slider.ExponentialPower );
										control.Control.SetValue( action.Slider.Value );

										control.Control.Tag = action;
										control.Control.ValueChanged += Slider_ValueChanged;

										tripple.Items.Add( control );
									}
									//else if( action.ActionType == EditorAction.ActionTypeEnum.ComboBox )
									//{
									//	//ComboBox

									//	var tripple = new KryptonRibbonGroupTriple();
									//	ribbonGroup.Items.Add( tripple );

									//	var control = new KryptonRibbonGroupComboBox();
									//	control.Tag = action;

									//	control.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
									//	foreach( var item in action.ComboBox.Items )
									//		control.Items.Add( item );

									//	if( control.Items.Count != 0 )
									//		control.SelectedIndex = 0;

									//	//control.MinimumLength = action.Slider.Length;
									//	//control.MaximumLength = action.Slider.Length;

									//	control.SelectedIndexChanged += ComboBox_SelectedIndexChanged;

									//	tripple.Items.Add( control );
									//}
									else if( action.ActionType == EditorAction.ActionTypeEnum.ListBox )
									{
										//ListBox

										var tripple = new KryptonRibbonGroupTriple();
										ribbonGroup.Items.Add( tripple );

										var control = new KryptonRibbonGroupListBox();
										control.Tag = action;
										control.ToolTipBody = action.GetToolTip();

										control.Control.Size = new System.Drawing.Size( (int)( (float)action.ListBox.Length * EditorAPI2.DPIScale ), control.Control.Size.Height );
										control.Control.kryptonSplitContainer1.Size = new System.Drawing.Size( (int)( (float)action.ListBox.Length * EditorAPI2.DPIScale ), control.Control.Size.Height );
										control.Control.kryptonSplitContainer1.Panel2MinSize = (int)( (float)control.Control.kryptonSplitContainer1.Panel2MinSize * EditorAPI2.DPIScale );
										control.Control.kryptonSplitContainer1.SplitterDistance = 10000;
										//if( action.ListBox.Length != 172 )
										//	control.Control.Size = new System.Drawing.Size( action.ListBox.Length, control.Control.Size.Height );

										control.Control.kryptonLabel1.Text = EditorLocalization2.Translate( "Ribbon.Action", action.RibbonText.Item1 );

										var browser = control.Control.contentBrowser1;

										if( action.ListBox.Mode == EditorAction.ListBoxSettings.ModeEnum.Tiles )
										{
											browser.ListViewModeOverride = new ContentBrowserListModeTilesRibbon( browser );
											browser.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
											browser.Options.ListMode = ContentBrowser.ListModeEnum.Tiles;
											browser.UseSelectedTreeNodeAsRootForList = false;
											browser.Options.Breadcrumb = false;
											browser.ListViewBorderDraw = BorderSides.Left | BorderSides.Right | BorderSides.Bottom;
											browser.Options.TileImageSize = 22;
										}
										else
										{
											browser.RemoveTreeViewIconsColumn();
											browser.TreeView.RowHeight -= 2;
										}

										browser.Tag = action;

										//update items
										control.SetItems( action.ListBox.Items );

										browser.ItemAfterSelect += ListBrowser_ItemAfterSelect;
										browser.ShowContextMenuEvent += ListBrowser_ShowContextMenuEvent;

										if( browser.Items.Count != 0 )
											browser.SelectItems( new ContentBrowser.Item[] { browser.Items.ToArray()[ 0 ] } );

										//browser.ItemAfterSelect += ListBrowser_ItemAfterSelect;

										tripple.Items.Add( control );
									}

								}
							}
						}

						if( ribbonGroup.Items.Count != 0 )
							ribbonTab.Groups.Add( ribbonGroup );
					}

					//select
					var tabType = "";
					if( tab != null )
						tabType = tab.Type;
					if( ribbonLastSelectedTabTypeByUser != "" && tabType == ribbonLastSelectedTabTypeByUser )
						kryptonRibbon.SelectedTab = ribbonTab;
				}

				ribbonLastSelectedTabTypeByUser_DisableUpdate = false;
			}
		}

		void RibbonSubGroupUpdateItemsRecursive( KryptonContextMenuCollection contextMenuItems, out bool existsEnabled )
		{
			existsEnabled = false;

			foreach( var item in contextMenuItems )
			{
				var items = item as KryptonContextMenuItems;
				if( items != null )
				{
					foreach( var item2 in items.Items )
					{
						var item3 = item2 as KryptonContextMenuItem;
						if( item3 != null )
						{
							var action = item3.Tag as EditorAction;
							if( action != null )
							{
								var state = EditorAPI2.EditorActionGetState( EditorActionHolder.RibbonQAT, action );

								item3.Enabled = state.Enabled;
								if( item3.Checked != state.Checked )
									item3.Checked = state.Checked;

								if( item3.Enabled )
									existsEnabled = true;

								if( !EditorUtility.PerformEditorActionVisibleFilter( action ) )
									item3.Visible = false;
							}
						}
					}
				}
			}
		}

		void RibbonButtonsUpdateProperties()
		{
			//update tabs visibility
			{
				Metadata.TypeInfo selectedType = null;
				{
					var obj = workspaceController.SelectedDocumentWindow?.ObjectOfWindow;
					if( obj != null )
						selectedType = MetadataManager.MetadataGetType( obj );
				}
				foreach( var ribbonTab in kryptonRibbon.RibbonTabs )
				{
					var tab = ribbonTab.Tag as EditorRibbonDefaultConfiguration.Tab;
					if( tab != null )
					{
						bool visible = true;
						if( tab.VisibleOnlyForType != null && visible )
							visible = selectedType != null && tab.VisibleOnlyForType.IsAssignableFrom( selectedType );
						if( tab.VisibleCondition != null && visible )
							visible = tab.VisibleCondition();
						if( visible && !EditorUtility.PerformRibbonTabVisibleFilter( tab ) )
							visible = false;

						if( ribbonTab.Visible != visible )
						{
							ribbonLastSelectedTabTypeByUser_DisableUpdate = true;
							ribbonTab.Visible = visible;
							ribbonLastSelectedTabTypeByUser_DisableUpdate = false;
						}

						if( ribbonTab.Visible && ribbonLastSelectedTabTypeByUser != "" && ribbonLastSelectedTabTypeByUser == tab.Type )
						{
							ribbonLastSelectedTabTypeByUser_DisableUpdate = true;
							kryptonRibbon.SelectedTab = ribbonTab;
							ribbonLastSelectedTabTypeByUser_DisableUpdate = false;
						}
					}
				}
			}

			//update controls
			foreach( var ribbonTab in kryptonRibbon.RibbonTabs )
			{
				foreach( var ribbonGroup in ribbonTab.Groups )
				{
					foreach( var groupItem in ribbonGroup.Items )
					{
						var tripple = groupItem as KryptonRibbonGroupTriple;
						if( tripple != null )
						{
							foreach( var trippleItem in tripple.Items )
							{
								//Button, DropDown
								var button = trippleItem as KryptonRibbonGroupButton;
								if( button != null )
								{
									//sub group
									if( button.Tag as string == "SubGroup" && button.KryptonContextMenu != null )
									{
										RibbonSubGroupUpdateItemsRecursive( button.KryptonContextMenu.Items, out var existsEnabled );

										//disable group when all items are disabled
										button.Enabled = existsEnabled;
									}

									//action
									var action = button.Tag as EditorAction;
									if( action != null )
									{
										var state = EditorAPI2.EditorActionGetState( EditorActionHolder.RibbonQAT, action );

										button.Enabled = state.Enabled;

										//кнопка меняет тип, т.к. в действиях не указывается может ли она быть Checked.
										//сделано так, что меняет при первом изменении isChecked.
										if( state.Checked && action.ActionType == EditorAction.ActionTypeEnum.Button )
											button.ButtonType = GroupButtonType.Check;

										button.Checked = state.Checked;

										if( !EditorUtility.PerformEditorActionVisibleFilter( action ) )
											button.Visible = false;
									}
								}

								//Slider
								var slider = trippleItem as KryptonRibbonGroupSlider;
								if( slider != null )
								{
									var action = slider.Tag as EditorAction;
									if( action != null )
									{
										var lastValue = action.Slider.Value;

										var state = EditorAPI2.EditorActionGetState( EditorActionHolder.RibbonQAT, action );

										slider.Enabled = state.Enabled;
										if( lastValue != action.Slider.Value )
										{
											slider.Control.SetValue( action.Slider.Value );
											//trackBar.Value = action.Slider.Value;
										}

										if( !EditorUtility.PerformEditorActionVisibleFilter( action ) )
											slider.Visible = false;
									}
								}

								////ComboBox
								//var comboBox = trippleItem as KryptonRibbonGroupComboBox;
								//if( comboBox != null )
								//{
								//	var action = comboBox.Tag as EditorAction;
								//	if( action != null )
								//	{
								//		var lastItems = action.ComboBox.Items;
								//		var lastSelectedIndex = action.ComboBox.SelectedIndex;

								//		var state = EditorAPI.GetEditorActionState( EditorAction.HolderEnum.RibbonQAT, action );

								//		comboBox.Enabled = state.Enabled;

								//		if( !action.ComboBox.Items.SequenceEqual( lastItems ) )
								//		{
								//			var oldSelectedIndex = comboBox.SelectedIndex;

								//			comboBox.Items.Clear();
								//			foreach( var item in action.ComboBox.Items )
								//				comboBox.Items.Add( item );

								//			if( oldSelectedIndex >= 0 && oldSelectedIndex < comboBox.Items.Count )
								//				comboBox.SelectedIndex = oldSelectedIndex;
								//			else if( comboBox.Items.Count != 0 )
								//				comboBox.SelectedIndex = 0;
								//		}
								//		if( lastSelectedIndex != action.ComboBox.SelectedIndex )
								//			comboBox.SelectedIndex = action.ComboBox.SelectedIndex;
								//	}
								//}

								//ListBox
								var listBox = trippleItem as KryptonRibbonGroupListBox;
								if( listBox != null )
								{
									var action = listBox.Tag as EditorAction;
									if( action != null )
									{
										var lastItems = action.ListBox.Items;
										var lastSelectedIndex = action.ListBox.SelectedIndex;

										var state = EditorAPI2.EditorActionGetState( EditorActionHolder.RibbonQAT, action );

										//!!!!не listBox2?
										listBox.Enabled = state.Enabled;

										var browser = listBox.Control.contentBrowser1;

										//update items
										if( !action.ListBox.Items.SequenceEqual( lastItems ) )
											listBox.SetItems( action.ListBox.Items );

										//update selected item
										if( action.ListBox.SelectIndex != null )
										{
											int selectIndex = action.ListBox.SelectIndex.Value;

											var itemToSelect = browser.Items.FirstOrDefault( i => (int)i.Tag == selectIndex );
											if( itemToSelect != null )
											{
												var toSelect = new ContentBrowser.Item[] { itemToSelect };
												if( !toSelect.SequenceEqual( browser.SelectedItems ) )
													browser.SelectItems( toSelect );
											}
											else
											{
												if( browser.SelectedItems.Length != 0 )
													browser.SelectItems( null );
											}

											action.ListBox.SelectIndex = null;
										}

										if( !EditorUtility.PerformEditorActionVisibleFilter( action ) )
											listBox.Visible = false;

										//{
										//	int selectIndex = action.ListBox.SelectedIndex;
										//	int currentSelectedIndex = -1;
										//	if( browser.SelectedItems.Length == 1 )
										//		currentSelectedIndex = (int)browser.SelectedItems[ 0 ].Tag;
										//	//if( browser.SelectedItems.Length == 1 )
										//	//	currentSelectedIndex = browser.RootItems.IndexOf( browser.SelectedItems[ 0 ] );

										//	if( selectIndex != currentSelectedIndex )
										//	{
										//		var itemToSelect = browser.Items.FirstOrDefault( i => (int)i.Tag == selectIndex );
										//		if( itemToSelect != null )
										//			browser.SelectItems( new ContentBrowser.Item[] { itemToSelect } );
										//		else
										//			browser.SelectItems( null );
										//	}
										//}

										//!!!!?
										//if( lastSelectedIndex != action.ListBox.SelectedIndex )
										//{
										//	if(browser.Items
										//	browser.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );

										//	//browser.SelectedIndex = action.ListBox.SelectedIndex;
										//}

									}
								}

							}
						}

					}
				}
			}
		}

		void RibbonUpdate()
		{
			RibbonButtonsCheckForRecreate();
			RibbonButtonsUpdateProperties();
		}

		////!!!!
		//public void SaveDockingStateAndUnloadDocuments()
		//{
		//	{
		//		string configFile = VirtualPathUtility.GetRealPathByVirtual( dockingConfigFileName );
		//		EditorAPI.GetRestartApplication( out _, out var resetWindowsSettings );
		//		if( resetWindowsSettings )
		//		{
		//			if( File.Exists( configFile ) )
		//				File.Delete( configFile );
		//		}
		//		else
		//		{
		//			if( !Directory.Exists( Path.GetDirectoryName( configFile ) ) )
		//				Directory.CreateDirectory( Path.GetDirectoryName( configFile ) );

		//			workspaceController.SaveLayoutToFile( configFile );

		//			// temp experimental:
		//			//foreach (var wnd in workspaceController.OfType<WorkspaceWindow>())
		//			//{
		//			//	string configFileName = string.Format(workspaceConfigFileName, wnd.Name);
		//			//	string workspaceConfigFile = VirtualPathUtils.GetRealPathByVirtual(configFileName);
		//			//	var controller = wnd.WorkspaceController;
		//			//	controller.SaveLayoutToFile(workspaceConfigFile);
		//			//}
		//		}

		//		//save part to Editor.config
		//		workspaceController.SaveAdditionalConfig();
		//	}


		//	//!!!!на вторых левелах закрывать?
		//	// remove all managed windows. not only attached.
		//	//foreach( var window in workspaceController.GetDockWindows().ToList() )
		//	foreach( var window in workspaceController.GetDockWindows().ToList() )
		//	{
		//		var documentWindow = window as DocumentWindow;
		//		if( documentWindow != null && documentWindow.Document != null )
		//		{
		//			//!!!!
		//			//EditorAPI.CloseAllDocumentWindowsOnSecondLevel( zzzzz );

		//			workspaceController.RemoveDockWindow( window, true );
		//		}
		//	}



		//	//!!!!




		//}

		////!!!!
		//public void LoadDockingState()
		//{

		//	//load docking state
		//	{
		//		string configFile = VirtualPathUtility.GetRealPathByVirtual( dockingConfigFileName );
		//		//default settings
		//		if( !File.Exists( configFile ) )
		//			configFile = VirtualPathUtility.GetRealPathByVirtual( dockingConfigFileNameDefault );

		//		if( File.Exists( configFile ) )
		//		{
		//			//no try catch to save the ability to work with debugger. in case when error happens during loading one of documents

		//			//try
		//			//{

		//			workspaceController.LoadLayoutFromFile( configFile );

		//			//!!!!
		//			//hack. unhide the page to load it correctly. after loading the page will hided
		//			foreach( var page in workspaceController.DockingManager.Pages )
		//			{
		//				if( page.needHideAfterLoading )
		//				{
		//					page.needHideAfterLoading = false;

		//					var window = page.GetDockWindow();
		//					if( window != null )
		//						workspaceController.SetDockWindowVisibility( window, false );
		//				}
		//			}

		//			//}
		//			//catch( Exception e2 )
		//			//{
		//			//	var text = $"Error loading docking settings.\n\n" + e2.Message;
		//			//	Log.Warning( text );
		//			//	EditorMessageBox.ShowWarning( text );
		//			//}
		//		}
		//	}

		//}


		public void NeedRecreateRibbonButtons()
		{
			unlockFormUpdateInTimer = DateTime.Now + TimeSpan.FromSeconds( 0.1 );
			needRecreateRibbonButtons = true;
		}

		public void NeedRecreateQATButtons()
		{
			unlockFormUpdateInTimer = DateTime.Now + TimeSpan.FromSeconds( 0.1 );
			needRecreateQATButtons = true;
		}

		void ProcessConfigAutoSave()
		{
			if( configAutoSaveLastTime == 0 )
				configAutoSaveLastTime = EngineApp.EngineTime;

			//3 minutes
			if( EngineApp.EngineTime > configAutoSaveLastTime + 60.0 * 3.0 )
			{
				configAutoSaveLastTime = EngineApp.EngineTime;

				SaveDockingState();
				if( !string.IsNullOrEmpty( EngineApp.InitSettings.ConfigVirtualFileName ) )
					EngineConfig.Save();
			}
		}
	}
}
#endif