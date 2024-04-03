// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents a General page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_General : ProjectSettingsPage
	{
		///// <summary>
		///// Enables the extended mode of the editor. All features of NeoAxis Editor are activated in this mode.
		///// </summary>
		//[DefaultValue( false )]
		//[Category( "General" )]
		//[DisplayName( "Extended Mode (Restart to apply changes)" )]
		//public Reference<bool> ExtendedMode
		//{
		//	get { if( _extendedMode.BeginGet() ) ExtendedMode = _extendedMode.Get( this ); return _extendedMode.value; }
		//	set { if( _extendedMode.BeginSet( this, ref value ) ) { try { ExtendedModeChanged?.Invoke( this ); } finally { _extendedMode.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ExtendedMode"/> property value changes.</summary>
		//public event Action<ProjectSettingsPage_General> ExtendedModeChanged;
		//ReferenceField<bool> _extendedMode = false;

		/// <summary>
		/// The name of the project.
		/// </summary>
		[DefaultValue( "Example Project" )]
		[Category( "Project" )]
		public Reference<string> ProjectName
		{
			get { if( _projectName.BeginGet() ) ProjectName = _projectName.Get( this ); return _projectName.value; }
			set { if( _projectName.BeginSet( this, ref value ) ) { try { ProjectNameChanged?.Invoke( this ); } finally { _projectName.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProjectName"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> ProjectNameChanged;
		ReferenceField<string> _projectName = "Example Project";

		/// <summary>
		/// The name of the world.
		/// </summary>
		[Category( "General" )]
		public string CloudProjectName
		{
			get
			{
				if( EngineInfo.CloudProjectInfo != null )
					return EngineInfo.CloudProjectInfo.Name;
				return "";
			}
		}

		/////////////////////////////////////////

		public enum ThemeEnum
		{
			Light,
			Dark,
		}

		/// <summary>
		/// The theme of the editor. Restart the editor to apply changes.
		/// </summary>
		[DefaultValue( ThemeEnum.Dark )]
		[Category( "Editor" )]
		[DisplayName( "Theme (Restart to apply changes)" )]
		public Reference<ThemeEnum> Theme
		{
			get { if( _theme.BeginGet() ) Theme = _theme.Get( this ); return _theme.value; }
			set { if( _theme.BeginSet( this, ref value ) ) { try { ThemeChanged?.Invoke( this ); } finally { _theme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Theme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> ThemeChanged;
		ReferenceField<ThemeEnum> _theme = ThemeEnum.Dark;

		[Flags]
		public enum CustomizeWindowsStyleEnum
		{
			None = 0,
			Auto = 1,
			MainForm = 2,
			AdditionalForms = 4,
		}

		/// <summary>
		/// Whether to use custom style for window title bars. Restart the editor to apply changes.
		/// </summary>
		[DefaultValue( CustomizeWindowsStyleEnum.Auto )]
		[Category( "Editor" )]
		[DisplayName( "Customize Windows Style (Restart to apply changes)" )]
		public Reference<CustomizeWindowsStyleEnum> CustomizeWindowsStyle
		{
			get { if( _customizeWindowsStyle.BeginGet() ) CustomizeWindowsStyle = _customizeWindowsStyle.Get( this ); return _customizeWindowsStyle.value; }
			set { if( _customizeWindowsStyle.BeginSet( this, ref value ) ) { try { CustomizeWindowsStyleChanged?.Invoke( this ); } finally { _customizeWindowsStyle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CustomizeWindowsStyle"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> CustomizeWindowsStyleChanged;
		ReferenceField<CustomizeWindowsStyleEnum> _customizeWindowsStyle = CustomizeWindowsStyleEnum.Auto;

		public enum LanguageEnum
		{
			English,
			Russian,
			New,
		}

		/// <summary>
		/// The language of the editor. Restart the editor to apply changes.
		/// </summary>
		[DefaultValue( LanguageEnum.English )]
		[Category( "Editor" )]
		[DisplayName( "Language (Restart to apply changes)" )]
		public Reference<LanguageEnum> Language
		{
			get { if( _language.BeginGet() ) Language = _language.Get( this ); return _language.value; }
			set { if( _language.BeginSet( this, ref value ) ) { try { LanguageChanged?.Invoke( this ); } finally { _language.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Language"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> LanguageChanged;
		ReferenceField<LanguageEnum> _language = LanguageEnum.English;

		/// <summary>
		/// The maximum FPS in the document.
		/// </summary>
		[DefaultValue( 100.0 )]
		[Category( "Editor" )]
		[Range( 20, 300 )]
		public Reference<double> MaxFramesPerSecondDocument
		{
			get { if( _maxFramesPerSecondDocument.BeginGet() ) MaxFramesPerSecondDocument = _maxFramesPerSecondDocument.Get( this ); return _maxFramesPerSecondDocument.value; }
			set { if( _maxFramesPerSecondDocument.BeginSet( this, ref value ) ) { try { MaxFramesPerSecondDocumentChanged?.Invoke( this ); } finally { _maxFramesPerSecondDocument.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxFramesPerSecondDocument"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> MaxFramesPerSecondDocumentChanged;
		ReferenceField<double> _maxFramesPerSecondDocument = 100.0;

		/// <summary>
		/// The maximum FPS in the preview.
		/// </summary>
		[DefaultValue( 50.0 )]
		[Category( "Editor" )]
		[Range( 20, 300 )]
		public Reference<double> MaxFramesPerSecondPreview
		{
			get { if( _maxFramesPerSecondPreview.BeginGet() ) MaxFramesPerSecondPreview = _maxFramesPerSecondPreview.Get( this ); return _maxFramesPerSecondPreview.value; }
			set { if( _maxFramesPerSecondPreview.BeginSet( this, ref value ) ) { try { MaxFramesPerSecondPreviewChanged?.Invoke( this ); } finally { _maxFramesPerSecondPreview.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxFramesPerSecondPreview"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> MaxFramesPerSecondPreviewChanged;
		ReferenceField<double> _maxFramesPerSecondPreview = 50.0;

		/// <summary>
		/// The volume of the sound playback.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Category( "Editor" )]
		[Range( 0, 1 )]
		public Reference<double> SoundVolume
		{
			get { if( _soundVolume.BeginGet() ) SoundVolume = _soundVolume.Get( this ); return _soundVolume.value; }
			set { if( _soundVolume.BeginSet( this, ref value ) ) { try { SoundVolumeChanged?.Invoke( this ); } finally { _soundVolume.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundVolume"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SoundVolumeChanged;
		ReferenceField<double> _soundVolume = 0.5;

		/// <summary>
		/// Whether to enable an additional objects tree in the Settings window. This makes it possible to work with objects more flexibly, although it takes up more space in the window.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Editor" )]
		public Reference<bool> DisplayHierarchyOfObjectsInSettingsWindow
		{
			get { if( _displayHierarchyOfObjectsInSettingsWindow.BeginGet() ) DisplayHierarchyOfObjectsInSettingsWindow = _displayHierarchyOfObjectsInSettingsWindow.Get( this ); return _displayHierarchyOfObjectsInSettingsWindow.value; }
			set { if( _displayHierarchyOfObjectsInSettingsWindow.BeginSet( this, ref value ) ) { try { DisplayHierarchyOfObjectsInSettingsWindowChanged?.Invoke( this ); } finally { _displayHierarchyOfObjectsInSettingsWindow.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayHierarchyOfObjectsInSettingsWindow"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> DisplayHierarchyOfObjectsInSettingsWindowChanged;
		ReferenceField<bool> _displayHierarchyOfObjectsInSettingsWindow = false;

		/// <summary>
		/// The maximum number of items that can be displayed for collections in the Settings window.
		/// </summary>
		[DefaultValue( 100 )]
		[Category( "Editor" )]
		public Reference<int> PropertiesMaxCountCollectionItemsToDisplay
		{
			get { if( _propertiesMaxCountCollectionItemsToDisplay.BeginGet() ) PropertiesMaxCountCollectionItemsToDisplay = _propertiesMaxCountCollectionItemsToDisplay.Get( this ); return _propertiesMaxCountCollectionItemsToDisplay.value; }
			set { if( _propertiesMaxCountCollectionItemsToDisplay.BeginSet( this, ref value ) ) { try { PropertiesMaxCountCollectionItemsToDisplayChanged?.Invoke( this ); } finally { _propertiesMaxCountCollectionItemsToDisplay.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PropertiesMaxCountCollectionItemsToDisplay"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> PropertiesMaxCountCollectionItemsToDisplayChanged;
		ReferenceField<int> _propertiesMaxCountCollectionItemsToDisplay = 100;

		///// <summary>
		///// Whether to animate windows auto-hiding in the editor.
		///// </summary>
		//[DefaultValue( false )]
		//[Category( "Editor" )]
		//public Reference<bool> AnimateWindowsAutoHiding
		//{
		//	get { if( _animateAutoHideWindows.BeginGet() ) AnimateWindowsAutoHiding = _animateAutoHideWindows.Get( this ); return _animateAutoHideWindows.value; }
		//	set { if( _animateAutoHideWindows.BeginSet( this, ref value ) ) { try { AnimateAutoHideWindowsChanged?.Invoke( this ); } finally { _animateAutoHideWindows.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AnimateWindowsAutoHiding"/> property value changes.</summary>
		//public event Action<ProjectSettingsPage_General> AnimateAutoHideWindowsChanged;
		//ReferenceField<bool> _animateAutoHideWindows = false;

		///// <summary>
		///// Shows the splash screen at start-up of the editor.
		///// </summary>
		//[Category( "Editor" )]
		//[DefaultValue( true )]
		//public Reference<bool> SplashScreenAtStartup
		//{
		//	get { if( _splashScreenAtStartup.BeginGet() ) SplashScreenAtStartup = _splashScreenAtStartup.Get( this ); return _splashScreenAtStartup.value; }
		//	set { if( _splashScreenAtStartup.BeginSet( this, ref value ) ) { try { SplashScreenAtStartupChanged?.Invoke( this ); } finally { _splashScreenAtStartup.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SplashScreenAtStartup"/> property value changes.</summary>
		//public event Action<ProjectSettingsPage_General> SplashScreenAtStartupChanged;
		//ReferenceField<bool> _splashScreenAtStartup = true;

		/// <summary>
		/// Whether to show centering borders for mesh and surface editors to help take screenshots for the store.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Editor" )]
		[DisplayName( "Show Centering Borders" )]
		public Reference<bool> ShowCenteringBorders
		{
			get { if( _showCenteringBorders.BeginGet() ) ShowCenteringBorders = _showCenteringBorders.Get( this ); return _showCenteringBorders.value; }
			set { if( _showCenteringBorders.BeginSet( this, ref value ) ) { try { ShowCenteringBordersChanged?.Invoke( this ); } finally { _showCenteringBorders.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShowCenteringBorders"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> ShowCenteringBordersChanged;
		ReferenceField<bool> _showCenteringBorders = false;

		/// <summary>
		/// Whether to launch the Player in fullscreen mode.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Editor" )]
		public Reference<bool> RunPlayerFromEditorInFullscreen
		{
			get { if( _runPlayerFromEditorInFullscreen.BeginGet() ) RunPlayerFromEditorInFullscreen = _runPlayerFromEditorInFullscreen.Get( this ); return _runPlayerFromEditorInFullscreen.value; }
			set { if( _runPlayerFromEditorInFullscreen.BeginSet( this, ref value ) ) { try { RunPlayerFromEditorInFullscreenChanged?.Invoke( this ); } finally { _runPlayerFromEditorInFullscreen.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunPlayerFromEditorInFullscreen"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> RunPlayerFromEditorInFullscreenChanged;
		ReferenceField<bool> _runPlayerFromEditorInFullscreen = false;

		/////////////////////////////////////////

		/// <summary>
		/// The number of simulation steps per second.
		/// </summary>
		[DefaultValue( 60.0 )]
		[Category( "Simulation" )]
		//[Category( "Project Application" )]
		public Reference<double> SimulationStepsPerSecond
		{
			get { if( _simulationStepsPerSecond.BeginGet() ) SimulationStepsPerSecond = _simulationStepsPerSecond.Get( this ); return _simulationStepsPerSecond.value; }
			set
			{
				if( _simulationStepsPerSecond.BeginSet( this, ref value ) )
				{
					try
					{
						SimulationStepsPerSecondChanged?.Invoke( this );

						if( _simulationStepsPerSecond.value != 0 )
							simulationStepsPerSecondInv = 1.0f / (float)_simulationStepsPerSecond.value;
					}
					finally { _simulationStepsPerSecond.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SimulationStepsPerSecond"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SimulationStepsPerSecondChanged;
		ReferenceField<double> _simulationStepsPerSecond = 60.0;

		[Browsable( false )]
		public float SimulationStepsPerSecondInv
		{
			get
			{
				//to update in case when reference is specified
				var r = SimulationStepsPerSecond.ReferenceSpecified;

				return simulationStepsPerSecondInv;
			}
		}
		float simulationStepsPerSecondInv = 1.0f / 60;

		/// <summary>
		/// Initial UI file when starting the project application.
		/// </summary>
		[DefaultValueReference( @"Base\UI\Screens\SplashScreen.ui" )]
		[Category( "Simulation" )]
		[DisplayName( "Initial UI Screen" )]
		public Reference<ReferenceValueType_Resource> InitialUIScreen
		{
			get { if( _initialUIScreen.BeginGet() ) InitialUIScreen = _initialUIScreen.Get( this ); return _initialUIScreen.value; }
			set { if( _initialUIScreen.BeginSet( this, ref value ) ) { try { InitialUIScreenChanged?.Invoke( this ); } finally { _initialUIScreen.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InitialUIScreen"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> InitialUIScreenChanged;
		ReferenceField<ReferenceValueType_Resource> _initialUIScreen = new Reference<ReferenceValueType_Resource>( null, @"Base\UI\Screens\SplashScreen.ui" );

		/// <summary>
		/// A scene to run automatically when the Player started.
		/// </summary>
		[Category( "Simulation" )]
		[DefaultValue( null )]
		public Reference<ReferenceValueType_Resource> SceneAutoPlay
		{
			get { if( _sceneAutoPlay.BeginGet() ) SceneAutoPlay = _sceneAutoPlay.Get( this ); return _sceneAutoPlay.value; }
			set { if( _sceneAutoPlay.BeginSet( this, ref value ) ) { try { SceneAutoPlayChanged?.Invoke( this ); } finally { _sceneAutoPlay.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneAutoPlay"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SceneAutoPlayChanged;
		ReferenceField<ReferenceValueType_Resource> _sceneAutoPlay;

		public enum WindowStateEnum
		{
			Auto,
			Normal,
			Minimized,
			Maximized,
			Fullscreen
		}

		/// <summary>
		/// The initial state of the project window. When set to Auto, it will enabled Fullscreen mode or Maximized mode when run from the editor.
		/// </summary>
		[Category( "Simulation" )]
		[DefaultValue( WindowStateEnum.Auto )]
		public Reference<WindowStateEnum> WindowState
		{
			get { if( _windowState.BeginGet() ) WindowState = _windowState.Get( this ); return _windowState.value; }
			set { if( _windowState.BeginSet( this, ref value ) ) { try { WindowStateChanged?.Invoke( this ); } finally { _windowState.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WindowState"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> WindowStateChanged;
		ReferenceField<WindowStateEnum> _windowState = WindowStateEnum.Auto;

		public static Vector2I WindowSizeDefault = new Vector2I( 1100, 750 );
		/// <summary>
		/// The initial window size of the project window for Normal window state.
		/// </summary>
		[Category( "Simulation" )]
		[DefaultValue( "1100 750" )]
		public Reference<Vector2I> WindowSize
		{
			get { if( _windowSize.BeginGet() ) WindowSize = _windowSize.Get( this ); return _windowSize.value; }
			set { if( _windowSize.BeginSet( this, ref value ) ) { try { WindowSizeChanged?.Invoke( this ); } finally { _windowSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WindowSize"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> WindowSizeChanged;
		ReferenceField<Vector2I> _windowSize = WindowSizeDefault;

		/// <summary>
		/// The minimal size of the app window.
		/// </summary>
		[Category( "Simulation" )]
		[DefaultValue( "300 200" )]
		public Reference<Vector2I> WindowSizeMinimal
		{
			get { if( _windowSizeMinimal.BeginGet() ) WindowSizeMinimal = _windowSizeMinimal.Get( this ); return _windowSizeMinimal.value; }
			set { if( _windowSizeMinimal.BeginSet( this, ref value ) ) { try { WindowSizeMinimalChanged?.Invoke( this ); } finally { _windowSizeMinimal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WindowSizeMinimal"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> WindowSizeMinimalChanged;
		ReferenceField<Vector2I> _windowSizeMinimal = new Vector2I( 300, 200 );

		/// <summary>
		/// Whether to apply the system font scale to Window Size.
		/// </summary>
		[Category( "Simulation" )]
		[DefaultValue( true )]
		public Reference<bool> WindowSizeApplySystemFontScale
		{
			get { if( _windowSizeApplySystemFontScale.BeginGet() ) WindowSizeApplySystemFontScale = _windowSizeApplySystemFontScale.Get( this ); return _windowSizeApplySystemFontScale.value; }
			set { if( _windowSizeApplySystemFontScale.BeginSet( this, ref value ) ) { try { WindowSizeApplySystemFontScaleChanged?.Invoke( this ); } finally { _windowSizeApplySystemFontScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WindowSizeApplySystemFontScale"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> WindowSizeApplySystemFontScaleChanged;
		ReferenceField<bool> _windowSizeApplySystemFontScale = true;

		public enum EngineSplashScreenStyleEnumWithoutDisabled
		{
			WhiteBackground,
			BlackBackground,
			WhiteBackgroundSmall,
			BlackBackgroundSmall,

			//can't disable with default license agreement
			//Disabled,
		}

		public enum EngineSplashScreenStyleEnum
		{
			WhiteBackground,
			BlackBackground,
			WhiteBackgroundSmall,
			BlackBackgroundSmall,
			Disabled,
		}

		/// <summary>
		/// The style of the engine splash screen when the application is launched.
		/// </summary>
		[Category( "Simulation" )]
		[DefaultValue( EngineSplashScreenStyleEnumWithoutDisabled.BlackBackground )]//WhiteBackground )]
		public Reference<EngineSplashScreenStyleEnumWithoutDisabled> EngineSplashScreenStyle
		{
			get { if( _engineSplashScreenStyle.BeginGet() ) EngineSplashScreenStyle = _engineSplashScreenStyle.Get( this ); return _engineSplashScreenStyle.value; }
			set { if( _engineSplashScreenStyle.BeginSet( this, ref value ) ) { try { EngineSplashScreenStyleChanged?.Invoke( this ); } finally { _engineSplashScreenStyle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EngineSplashScreenStyle"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> EngineSplashScreenStyleChanged;
		ReferenceField<EngineSplashScreenStyleEnumWithoutDisabled> _engineSplashScreenStyle = EngineSplashScreenStyleEnumWithoutDisabled.BlackBackground;//WhiteBackground;

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( UserMode && member is Metadata.Property )
			{
				//special app mode
				//if( !EngineInfo.SpecialAppMode && member.Name == nameof( ExtendedMode ) )
				//	skip = true;
				if( /*!ExtendedMode && */ProjectSettingsComponent.HidePropertiesForSpecialAppMode.Contains( member.Name ) )
					skip = true;

				//Cloud project mode
				if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient )
				{
					if( member.Name == nameof( ProjectName ) )
						skip = true;
				}
				else
				{
					if( member.Name == nameof( CloudProjectName ) )
						skip = true;
				}
			}
		}
	}
}
