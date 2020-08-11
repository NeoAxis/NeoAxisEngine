// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Docking;
using System.Reflection;
using Microsoft.Win32;
using SharpBgfx;
using System.Diagnostics;
using NeoAxis.Widget;

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

		internal float dpi;

		public string ribbonLastSelectedTabTypeByUser = "";
		public bool ribbonLastSelectedTabTypeByUser_DisableUpdate = true;

		ObjectsInFocus getObjectsInFocusLastResult;

		public bool needClose;

		Control coverControl;

		double lastSlowUpdatingTime;

		public DateTime? unlockFormUpdateInTimer;

		public int skipPaintCounter = 2;

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
			var customWindowsStyle = ProjectSettings.ReadParameterFromFile( "CustomWindowsStyle" );
			if( !string.IsNullOrEmpty( customWindowsStyle ) )
			{
				try
				{
					KryptonToolkitSettings.AllowFormChrome = (bool)SimpleTypes.ParseValue( typeof( bool ), customWindowsStyle );
				}
				catch { }
			}
			AllowFormChrome = KryptonToolkitSettings.AllowFormChrome;

			//if( /*showSplashScreenAtStartup &&*/ !Debugger.IsAttached )
			//{
			//	//Image image = MapEditor.Properties.Resources.MapEditorSplash;
			//	var splashForm = new SplashForm();// image );
			//	splashForm.Show();
			//}

			InitializeComponent();

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
		//	if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
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

		private void EditorForm_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			//hide ribbon to avoid redrawing
			kryptonRibbon.Visible = false;

			// create cover
			coverControl = new Control();
			coverControl.BackColor = Color.FromArgb( 40, 40, 40 );
			coverControl.Dock = DockStyle.Fill;
			Controls.Add( coverControl );
			coverControl.BringToFront();
			Application.DoEvents();

			//dpi
			try
			{
				using( Graphics graphics = CreateGraphics() )
				{
					dpi = graphics.DpiX;
				}
			}
			catch( Exception ex )
			{
				dpi = 96;
				Log.Warning( "EditorForm: CreateGraphics: Call failed. " + ex.Message );
			}

			kryptonRibbon.RibbonTabs.Clear();

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

			//set theme
			if( ProjectSettings.Get.Theme.Value == Component_ProjectSettings.ThemeEnum.Dark )
				kryptonManager.GlobalPaletteMode = PaletteModeManager.NeoAxisBlack;
			else
				kryptonManager.GlobalPaletteMode = PaletteModeManager.NeoAxisBlue;

			KryptonDarkThemeUtility.DarkTheme = EditorAPI.DarkTheme;
			if( EditorAPI.DarkTheme )
				EditorAssemblyInterface.Instance.SetDarkTheme();
			Aga.Controls.Tree.NodeControls.BaseTextControl.DarkTheme = EditorAPI.DarkTheme;

			BackColor = EditorAPI.DarkTheme ? Color.FromArgb( 40, 40, 40 ) : Color.FromArgb( 240, 240, 240 );

			//app button
			kryptonRibbon.RibbonAppButton.AppButtonText = EditorLocalization.Translate( "AppButton", kryptonRibbon.RibbonAppButton.AppButtonText );
			if( DarkTheme )
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
			workspaceController.AddToDockspaceStack( new DockWindow[] { new ResourcesWindow(), new SolutionExplorer(), new PreviewWindow() }, DockingEdge.Left );
			workspaceController.AddToDockspace( new DockWindow[] { new MessageLogWindow(), new OutputWindow(), new DebugInfoWindow() }, DockingEdge.Bottom );

			Log.Info( "Use Log.Info(), Log.Warning() methods to write to the window. These methods can be used in the Player. Press '~' to open console of the Player." );
			OutputWindow.Print( "Use OutputWindow.Print() method to write to the window. Unlike Message Log window, this window is not a list. Here you can add text in arbitrary format.\n" );

			//!!!!
			//workspaceController.AddDockWindow( new TipsWindow(), true, false );

			//!!!!эвент чтобы свои добавлять. и пример

			//load docking state
			{
				string configFile = VirtualPathUtility.GetRealPathByVirtual( dockingConfigFileName );
				//default settings
				if( !File.Exists( configFile ) )
					configFile = VirtualPathUtility.GetRealPathByVirtual( dockingConfigFileNameDefault );

				if( File.Exists( configFile ) )
				{
					//try
					//{
					////!!!! If xml broken, we will not get an exception.
					//// the exception is swallowed inside the krypton.
					//// how do I know if an error has occurred?
					workspaceController.LoadLayoutFromFile( configFile );
					//}
					//	catch
					//	{
					//		//!!!!TODO: layout broken. fix this!
					//	}
				}
			}

			InitQAT();
			InitRibbon();

			UpdateText();

			//apply editor settings
			EditorSettingsSerialization.InitAfterFormLoad();

			XmlDocumentationFiles.PreloadBaseAssemblies();

			EditorAPI.SelectedDocumentWindowChanged += EditorAPI_SelectedDocumentWindowChanged;

			UpdateRecentProjectsInRegistry();

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

			loaded = true;
		}

		private void EditorForm_FormClosing( object sender, FormClosingEventArgs e )
		{
			bool? result = ShowDialogAndSaveDocuments( workspaceController.GetDockWindows() );
			if( result == null )
			{
				e.Cancel = true;
				return;
			}

			//save docking state
			if( canSaveConfig )
			//if( !forceCloseForm )
			{
				string configFile = VirtualPathUtility.GetRealPathByVirtual( dockingConfigFileName );
				EditorAPI.GetRestartApplication( out _, out var resetWindowsSettings );
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
			}

			EditorAPI.ClosingApplication = true;

			EditorLocalization.Shutdown();

			//destroy all documents
			{
				//!!!!может окна документов сначала удалить?
				foreach( var document in Documents.ToArray() )
					document.Destroy();
			}

			//destroy all viewport controls
			foreach( var control in EngineViewportControl.allInstances.ToArray() )
				control.Dispose();

			if( !canSaveConfig )
				EngineApp.NeedSaveConfig = false;
			EngineApp.Shutdown();
		}

		// сохранить только выбранные окна. не обязательно все. например может использоваться
		// из метода "Закрыть все окна кроме текущего".
		internal bool? ShowDialogAndSaveDocuments( IEnumerable<DockWindow> windows )
		{
			var unsavedDocs = new List<DocumentInstance>();

			foreach( var wnd in windows )
			{
				if( wnd is IDocumentWindow docWnd && !docWnd.IsDocumentSaved() )
					unsavedDocs.Add( docWnd.Document );
			}

			if( unsavedDocs.Count == 0 )
				return false;

			var text = EditorLocalization.Translate( "General", "Save changes to the following files?" ) + "\n";
			foreach( var doc in unsavedDocs )
				text += "\n" + doc.Name;

			switch( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNoCancel ) )
			{
			case EDialogResult.Cancel:
				return null;
			case EDialogResult.Yes:
				//!!!!check error, return null
				unsavedDocs.ForEach( doc => doc.Save() );
				return true;
			case EDialogResult.No:
				return false;
			}

			return false;
		}

		internal bool? ShowDialogAndSaveDocument( DockWindow window )
		{
			var documentWindow = window as DocumentWindow;
			if( documentWindow == null )
				return false;

			// If the page is dirty then we need to ask if it should be saved
			if( documentWindow.IsMainWindowInWorkspace && !documentWindow.IsDocumentSaved() )
			{
				var text = EditorLocalization.Translate( "General", "Save changes to the following files?" ) + "\n";
				text += "\n" + documentWindow.Document.Name;

				switch( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNoCancel ) )
				{
				case EDialogResult.Cancel:

					//!!!!тут?
					EditorAPI.SetRestartApplication( false );

					return null;
				case EDialogResult.Yes:
					//!!!!check error, return null
					documentWindow.SaveDocument();
					return true;
				case EDialogResult.No:
					return false;
				}
			}

			return false;
		}

		void UpdateText()
		{
			string projectName = "";
			var c = ProjectSettings.Get;//.GetComponent<Component_ProjectSettings_General>();
			if( c != null )
				projectName = c.ProjectName;

			var postFix = "";// " " + ( EngineApp.IsProPlan ? "Pro" : "Personal" );

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

			ScreenNotifications.Update();

			if( !needClose )
				EditorAPI.SelectedDocumentWindowChangedUpdate();

			if( !needClose )
			{
				foreach( var document in EditorAPI.Documents )
					document.EditorUpdateWhenDocumentModified_Tick();
			}

			if( !needClose )
				PreviewIconsManager.Update();

			////!!!!temp
			//CheckRestartApplicationToApplyChanged();

			if( !needClose )
				UpdateSoundSystem();

			//save editor settings
			if( !needClose )
				KryptonAutoHiddenSlidePanel.Animate = ProjectSettings.Get.AnimateWindowsAutoHiding;

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
					EditorAPI.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { realFileName } );

					//open file
					EditorAPI.OpenFileAsDocument( realFileName, true, true );
				}
			}

			if( firstTick )
			{
				firstTick = false;

				if( SplashForm.Instance != null )
					SplashForm.Instance.AllowClose = true;

				// hide cover show ribbon.
				kryptonRibbon.Visible = true;

				if( coverControl != null )
					Controls.Remove( coverControl );
				//coverControl.Visible = false;

				if( EditorSettingsSerialization.ShowTipsAsStartup )
					EditorAPI.ShowTips();
			}

			if( unlockFormUpdateInTimer.HasValue && ( DateTime.Now - unlockFormUpdateInTimer.Value ).TotalSeconds > 0 )
			{
				KryptonWinFormsUtility.LockFormUpdate( null );
				unlockFormUpdateInTimer = null;
			}

			canSaveConfig = true;
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

				foreach( var control in EngineViewportControl.allInstances )
				{
					if( control.IsAllowRender() )
					{
						if( control.AutomaticUpdateFPS != 0 )
							toRender.Add( control );
					}
					else
						unvisible.Add( control );
				}

				bool callFrame = false;

				//destroy render targets for unvisible controls
				foreach( var control in unvisible )
				{
					var context = control.Viewport?.RenderingContext;
					if( context != null )
					{
						if( context.DynamicTexturesAreExists() )
						{
							context.MultiRenderTarget_DestroyAll();
							context.DynamicTexture_DestroyAll();

							callFrame = true;
						}
					}
				}

				if( callFrame )
				{
					RenderingSystem.CallBgfxFrame();
					RenderingSystem.CallBgfxFrame();
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
		internal WorkspaceControllerForForm WorkspaceController
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
				if( newResult == null && EditorAPI.SelectedDocumentWindow != null )
				{
					//!!!!new
					newResult = new ObjectsInFocus( EditorAPI.SelectedDocumentWindow, EditorAPI.SelectedDocumentWindow.SelectedObjects );
					//newResult = new ObjectsInFocus( EditorAPI.SelectedDocumentWindow, new object[ 0 ] );
				}

				//update getObjectsInFocusLastResult
				if( newResult != null )
					getObjectsInFocusLastResult = newResult;
				else
				{
					if( getObjectsInFocusLastResult != null )
						if( getObjectsInFocusLastResult.DocumentWindow != null && !getObjectsInFocusLastResult.DocumentWindow.IsHandleCreated )
							getObjectsInFocusLastResult = null;
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
				var documentWithViewport = EditorAPI.SelectedDocumentWindow as DocumentWindowWithViewport;
				var viewport = documentWithViewport?.ViewportControl?.Viewport;

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
			EngineApp.DefaultSoundChannelGroup.Volume = ProjectSettings.Get.SoundVolume;
		}

		private void EditorAPI_SelectedDocumentWindowChanged()
		{
			var window = EditorAPI.SelectedDocumentWindow;
			if( window != null )
				window.SettingsWindowSelectObjects();
		}

		void UpdateRecentProjectsInRegistry()
		{
#if !PROJECT_DEPLOY
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
					EditorAPI.SelectedDocumentWindowChangedUpdate();
			}

			firstActivate = false;
		}

		[Browsable( false )]
		internal bool DarkTheme
		{
			get { return kryptonManager.GlobalPaletteMode == PaletteModeManager.NeoAxisBlack; }
		}

		void EnableLocalization()
		{
			var language = ProjectSettings.Get.Language.Value.ToString();
			if( !string.IsNullOrEmpty( language ) && language != "English" && language != "New" )
				EditorLocalization.Init( language, true );

			var strings = kryptonRibbon.RibbonStrings;
			strings.CustomizeQuickAccessToolbar = EditorLocalization.Translate( "General", strings.CustomizeQuickAccessToolbar );
			strings.ShowBelowRibbon = EditorLocalization.Translate( "General", strings.ShowBelowRibbon );
			strings.ShowAboveRibbon = EditorLocalization.Translate( "General", strings.ShowAboveRibbon );
			strings.Minimize = EditorLocalization.Translate( "General", strings.Minimize );

			var menus = kryptonDockableWorkspace.ContextMenus;
			menus.TextClose = EditorLocalization.Translate( "Docking", menus.TextClose );
			menus.TextCloseAllButThis = EditorLocalization.Translate( "Docking", menus.TextCloseAllButThis );
			menus.TextMovePrevious = EditorLocalization.Translate( "Docking", menus.TextMovePrevious );
			menus.TextMoveNext = EditorLocalization.Translate( "Docking", menus.TextMoveNext );
			menus.TextSplitHorizontal = EditorLocalization.Translate( "Docking", menus.TextSplitHorizontal );
			menus.TextSplitVertical = EditorLocalization.Translate( "Docking", menus.TextSplitVertical );
			menus.TextRebalance = EditorLocalization.Translate( "Docking", menus.TextRebalance );
			menus.TextMaximize = EditorLocalization.Translate( "Docking", menus.TextMaximize );
			menus.TextRestore = EditorLocalization.Translate( "Docking", menus.TextRestore );

			var strings2 = workspaceController.DockingManager.Strings;
			strings2.TextTabbedDocument = EditorLocalization.Translate( "Docking", strings2.TextTabbedDocument );
			strings2.TextAutoHide = EditorLocalization.Translate( "Docking", strings2.TextAutoHide );
			strings2.TextClose = EditorLocalization.Translate( "Docking", strings2.TextClose );
			strings2.TextCloseAllButThis = EditorLocalization.Translate( "Docking", strings2.TextCloseAllButThis );
			strings2.TextDock = EditorLocalization.Translate( "Docking", strings2.TextDock );
			strings2.TextFloat = EditorLocalization.Translate( "Docking", strings2.TextFloat );
			strings2.TextHide = EditorLocalization.Translate( "Docking", strings2.TextHide );
			strings2.TextWindowLocation = EditorLocalization.Translate( "Docking", strings2.TextWindowLocation );
		}

		bool ContainsNotFloatingWindows( DocumentInstance document )
		{
			foreach( var documentWindow in EditorAPI.GetAllDocumentWindowsOfDocument( document ) )
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
										var document = documentWindow.Document;
										if( document != null && !string.IsNullOrEmpty( document.RealFileName ) && ContainsNotFloatingWindows( document ) )
										{
											if( EditorAPI.SelectedDocument != null && EditorAPI.SelectedDocument != document )
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
			unlockFormUpdateInTimer = DateTime.Now + TimeSpan.FromSeconds( 0.1 );
			needRecreatedRibbonButtons = true;
		}

		protected override void WndProc( ref Message m )
		{
			var processed = false;

			if( !IsDisposed && !Disposing )
			{
				switch( m.Msg )
				{
				case ComponentFactory.Krypton.Toolkit.PI.WM_ERASEBKGND:
				case ComponentFactory.Krypton.Toolkit.PI.WM_PAINT:
					if( skipPaintCounter > 0 )
					{
						using( var brush = new SolidBrush( Color.FromArgb( 40, 40, 40 ) ) )
						using( var graphics = CreateGraphics() )
							graphics.FillRectangle( brush, new System.Drawing.Rectangle( 0, 0, Width, Height ) );

						processed = true;

						if( m.Msg == ComponentFactory.Krypton.Toolkit.PI.WM_PAINT && skipPaintCounter > 0 )
							skipPaintCounter--;
					}
					break;
				}
			}

			if( !processed )
				base.WndProc( ref m );
		}
	}
}