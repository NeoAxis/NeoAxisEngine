// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
		/// <summary>
		/// Enables the extended mode of the editor. All features of NeoAxis Editor are activated in this mode.
		/// </summary>
		[DefaultValue( false )]
		[Category( "General" )]
		[DisplayName( "Extended Mode (Restart to apply changes)" )]
		public Reference<bool> ExtendedMode
		{
			get { if( _extendedMode.BeginGet() ) ExtendedMode = _extendedMode.Get( this ); return _extendedMode.value; }
			set { if( _extendedMode.BeginSet( ref value ) ) { try { ExtendedModeChanged?.Invoke( this ); } finally { _extendedMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ExtendedMode"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> ExtendedModeChanged;
		ReferenceField<bool> _extendedMode = false;

		/// <summary>
		/// The name of the project.
		/// </summary>
		[DefaultValue( "Example Project" )]
		[Category( "General" )]
		public Reference<string> ProjectName
		{
			get { if( _projectName.BeginGet() ) ProjectName = _projectName.Get( this ); return _projectName.value; }
			set { if( _projectName.BeginSet( ref value ) ) { try { ProjectNameChanged?.Invoke( this ); } finally { _projectName.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProjectName"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> ProjectNameChanged;
		ReferenceField<string> _projectName = "Example Project";

		/////////////////////////////////////////

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
			set { if( _language.BeginSet( ref value ) ) { try { LanguageChanged?.Invoke( this ); } finally { _language.EndSet(); } } }
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
			set { if( _maxFramesPerSecondDocument.BeginSet( ref value ) ) { try { MaxFramesPerSecondDocumentChanged?.Invoke( this ); } finally { _maxFramesPerSecondDocument.EndSet(); } } }
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
			set { if( _maxFramesPerSecondPreview.BeginSet( ref value ) ) { try { MaxFramesPerSecondPreviewChanged?.Invoke( this ); } finally { _maxFramesPerSecondPreview.EndSet(); } } }
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
			set { if( _soundVolume.BeginSet( ref value ) ) { try { SoundVolumeChanged?.Invoke( this ); } finally { _soundVolume.EndSet(); } } }
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
			set { if( _displayHierarchyOfObjectsInSettingsWindow.BeginSet( ref value ) ) { try { DisplayHierarchyOfObjectsInSettingsWindowChanged?.Invoke( this ); } finally { _displayHierarchyOfObjectsInSettingsWindow.EndSet(); } } }
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
			set { if( _propertiesMaxCountCollectionItemsToDisplay.BeginSet( ref value ) ) { try { PropertiesMaxCountCollectionItemsToDisplayChanged?.Invoke( this ); } finally { _propertiesMaxCountCollectionItemsToDisplay.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PropertiesMaxCountCollectionItemsToDisplay"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> PropertiesMaxCountCollectionItemsToDisplayChanged;
		ReferenceField<int> _propertiesMaxCountCollectionItemsToDisplay = 100;

		/// <summary>
		/// Whether to animate windows auto-hiding in the editor.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Editor" )]
		public Reference<bool> AnimateWindowsAutoHiding
		{
			get { if( _animateAutoHideWindows.BeginGet() ) AnimateWindowsAutoHiding = _animateAutoHideWindows.Get( this ); return _animateAutoHideWindows.value; }
			set { if( _animateAutoHideWindows.BeginSet( ref value ) ) { try { AnimateAutoHideWindowsChanged?.Invoke( this ); } finally { _animateAutoHideWindows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AnimateWindowsAutoHiding"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> AnimateAutoHideWindowsChanged;
		ReferenceField<bool> _animateAutoHideWindows = false;

		/// <summary>
		/// Whether to use custom style for window title bars. Windows only. Restart the editor to apply changes.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Editor" )]
		[DisplayName( "Custom Windows Style (Restart to apply changes)" )]
		public Reference<bool> CustomWindowsStyle
		{
			get { if( _customWindowsStyle.BeginGet() ) CustomWindowsStyle = _customWindowsStyle.Get( this ); return _customWindowsStyle.value; }
			set { if( _customWindowsStyle.BeginSet( ref value ) ) { try { CustomWindowsStyleChanged?.Invoke( this ); } finally { _customWindowsStyle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CustomWindowsStyle"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> CustomWindowsStyleChanged;
		ReferenceField<bool> _customWindowsStyle = true;

		/// <summary>
		/// Shows the splash screen at start-up of the editor.
		/// </summary>
		[Category( "Editor" )]
		[DefaultValue( true )]
		public Reference<bool> SplashScreenAtStartup
		{
			get { if( _splashScreenAtStartup.BeginGet() ) SplashScreenAtStartup = _splashScreenAtStartup.Get( this ); return _splashScreenAtStartup.value; }
			set { if( _splashScreenAtStartup.BeginSet( ref value ) ) { try { SplashScreenAtStartupChanged?.Invoke( this ); } finally { _splashScreenAtStartup.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SplashScreenAtStartup"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SplashScreenAtStartupChanged;
		ReferenceField<bool> _splashScreenAtStartup = true;

		/// <summary>
		/// Whether to show centering borders for Mesh and Surface editors to help take screenshots for the store.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Editor" )]
		[DisplayName( "Show Centering Borders" )]
		public Reference<bool> ShowCenteringBorders
		{
			get { if( _showCenteringBorders.BeginGet() ) ShowCenteringBorders = _showCenteringBorders.Get( this ); return _showCenteringBorders.value; }
			set { if( _showCenteringBorders.BeginSet( ref value ) ) { try { ShowCenteringBordersChanged?.Invoke( this ); } finally { _showCenteringBorders.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShowCenteringBorders"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> ShowCenteringBordersChanged;
		ReferenceField<bool> _showCenteringBorders = false;

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
		[Category( "Colors" )]
		[DisplayName( "Theme (Restart to apply changes)" )]
		public Reference<ThemeEnum> Theme
		{
			get { if( _theme.BeginGet() ) Theme = _theme.Get( this ); return _theme.value; }
			set { if( _theme.BeginSet( ref value ) ) { try { ThemeChanged?.Invoke( this ); } finally { _theme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Theme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> ThemeChanged;
		ReferenceField<ThemeEnum> _theme = ThemeEnum.Dark;

		//SelectedColor
		ReferenceField<ColorValue> _selectedColor = new ColorValue( 0, 1, 0 );
		/// <summary>
		/// The color of selected object in the scene view.
		/// </summary>
		[DefaultValue( "0 1 0" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SelectedColor
		{
			get
			{
				if( _selectedColor.BeginGet() )
					SelectedColor = _selectedColor.Get( this );
				return _selectedColor.value;
			}
			set
			{
				if( _selectedColor.BeginSet( ref value ) )
				{
					try { SelectedColorChanged?.Invoke( this ); }
					finally { _selectedColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SelectedColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SelectedColorChanged;


		//CanSelectColor
		ReferenceField<ColorValue> _canSelectColor = new ColorValue( 1, 1, 0 );
		/// <summary>
		/// The color of selectable object in the scene view.
		/// </summary>
		[DefaultValue( "1 1 0" )]
		[Category( "Colors" )]
		public Reference<ColorValue> CanSelectColor
		{
			get
			{
				if( _canSelectColor.BeginGet() )
					CanSelectColor = _canSelectColor.Get( this );
				return _canSelectColor.value;
			}
			set
			{
				if( _canSelectColor.BeginSet( ref value ) )
				{
					try { CanSelectColorChanged?.Invoke( this ); }
					finally { _canSelectColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CanSelectColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> CanSelectColorChanged;


		//!!!!надо каждой настройке такое. тогда их парами указывать
		//!!!!может просто "double AlphaMultiplier"
		//HiddenByOtherObjectsColorMultiplier
		ReferenceField<ColorValue> _hiddenByOtherObjectsColorMultiplier = new ColorValue( 1, 1, 1, .3 );
		/// <summary>
		/// The color multiplier applied when object is hidden by the other objects.
		/// </summary>
		[DefaultValue( "1 1 1 0.3" )]
		[Category( "Colors" )]
		public Reference<ColorValue> HiddenByOtherObjectsColorMultiplier
		{
			get
			{
				if( _hiddenByOtherObjectsColorMultiplier.BeginGet() )
					HiddenByOtherObjectsColorMultiplier = _hiddenByOtherObjectsColorMultiplier.Get( this );
				return _hiddenByOtherObjectsColorMultiplier.value;
			}
			set
			{
				if( _hiddenByOtherObjectsColorMultiplier.BeginSet( ref value ) )
				{
					try { HiddenByOtherObjectsColorMultiplierChanged?.Invoke( this ); }
					finally { _hiddenByOtherObjectsColorMultiplier.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="HiddenByOtherObjectsColorMultiplier"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> HiddenByOtherObjectsColorMultiplierChanged;


		//!!!!name: может без "Draw" или без "Debug", хотя Debug - это секция. везде так
		//SceneShowLightColor
		ReferenceField<ColorValue> _sceneShowLightColor = new ColorValue( 1, 1, 0, .8 );
		/// <summary>
		/// The color of light source objects in the scene view.
		/// </summary>
		[DefaultValue( "1 1 0 0.8" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowLightColor
		{
			get
			{
				if( _sceneShowLightColor.BeginGet() )
					SceneShowLightColor = _sceneShowLightColor.Get( this );
				return _sceneShowLightColor.value;
			}
			set
			{
				if( _sceneShowLightColor.BeginSet( ref value ) )
				{
					try { SceneShowLightColorChanged?.Invoke( this ); }
					finally { _sceneShowLightColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SceneShowLightColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SceneShowLightColorChanged;

		/// <summary>
		/// The color of decals in the scene view.
		/// </summary>
		[DefaultValue( "1 1 0" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowDecalColor
		{
			get { if( _sceneShowDecalColor.BeginGet() ) SceneShowDecalColor = _sceneShowDecalColor.Get( this ); return _sceneShowDecalColor.value; }
			set { if( _sceneShowDecalColor.BeginSet( ref value ) ) { try { SceneShowDecalColorChanged?.Invoke( this ); } finally { _sceneShowDecalColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneShowDecalColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SceneShowDecalColorChanged;
		ReferenceField<ColorValue> _sceneShowDecalColor = new ColorValue( 1, 1, 0 );

		//SceneShowPhysicsStaticColor
		ReferenceField<ColorValue> _sceneShowPhysicsStaticColor = new ColorValue( 0, 0, .4 );
		/// <summary>
		/// The color of static physics objects in the scene view.
		/// </summary>
		[DefaultValue( "0 0 0.4" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowPhysicsStaticColor
		{
			get
			{
				if( _sceneShowPhysicsStaticColor.BeginGet() )
					SceneShowPhysicsStaticColor = _sceneShowPhysicsStaticColor.Get( this );
				return _sceneShowPhysicsStaticColor.value;
			}
			set
			{
				if( _sceneShowPhysicsStaticColor.BeginSet( ref value ) )
				{
					try { SceneShowPhysicsStaticColorChanged?.Invoke( this ); }
					finally { _sceneShowPhysicsStaticColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SceneShowPhysicsStaticColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SceneShowPhysicsStaticColorChanged;


		//SceneShowPhysicsDynamicActiveColor
		ReferenceField<ColorValue> _sceneShowPhysicsDynamicActiveColor = new ColorValue( 0, 0, 1 );
		/// <summary>
		/// The color of active dynamic physics objects in the scene view.
		/// </summary>
		[DefaultValue( "0 0 1" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowPhysicsDynamicActiveColor
		{
			get
			{
				if( _sceneShowPhysicsDynamicActiveColor.BeginGet() )
					SceneShowPhysicsDynamicActiveColor = _sceneShowPhysicsDynamicActiveColor.Get( this );
				return _sceneShowPhysicsDynamicActiveColor.value;
			}
			set
			{
				if( _sceneShowPhysicsDynamicActiveColor.BeginSet( ref value ) )
				{
					try { SceneShowPhysicsDynamicActiveColorChanged?.Invoke( this ); }
					finally { _sceneShowPhysicsDynamicActiveColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SceneShowPhysicsDynamicActiveColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SceneShowPhysicsDynamicActiveColorChanged;


		//SceneShowPhysicsDynamicInactiveColor
		ReferenceField<ColorValue> _sceneShowPhysicsDynamicInactiveColor = new ColorValue( 0, 0, .6 );
		/// <summary>
		/// The color of inactive dynamic physics objects in the scene view.
		/// </summary>
		[DefaultValue( "0 0 0.6" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowPhysicsDynamicInactiveColor
		{
			get
			{
				if( _sceneShowPhysicsDynamicInactiveColor.BeginGet() )
					SceneShowPhysicsDynamicInactiveColor = _sceneShowPhysicsDynamicInactiveColor.Get( this );
				return _sceneShowPhysicsDynamicInactiveColor.value;
			}
			set
			{
				if( _sceneShowPhysicsDynamicInactiveColor.BeginSet( ref value ) )
				{
					try { SceneShowPhysicsDynamicInactiveColorChanged?.Invoke( this ); }
					finally { _sceneShowPhysicsDynamicInactiveColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SceneShowPhysicsDynamicInactiveColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SceneShowPhysicsDynamicInactiveColorChanged;

		/// <summary>
		/// The color of areas in the scene view.
		/// </summary>
		[DefaultValue( "0 0 1" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowAreaColor
		{
			get { if( _sceneShowAreaColor.BeginGet() ) SceneShowAreaColor = _sceneShowAreaColor.Get( this ); return _sceneShowAreaColor.value; }
			set { if( _sceneShowAreaColor.BeginSet( ref value ) ) { try { SceneShowAreaColorChanged?.Invoke( this ); } finally { _sceneShowAreaColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneShowAreaColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SceneShowAreaColorChanged;
		ReferenceField<ColorValue> _sceneShowAreaColor = new ColorValue( 0, 0, 1 );

		/// <summary>
		/// The color of volumes in the scene view.
		/// </summary>
		[DefaultValue( "0 0 1" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowVolumeColor
		{
			get { if( _sceneShowVolumeColor.BeginGet() ) SceneShowVolumeColor = _sceneShowVolumeColor.Get( this ); return _sceneShowVolumeColor.value; }
			set { if( _sceneShowVolumeColor.BeginSet( ref value ) ) { try { SceneShowVolumeColorChanged?.Invoke( this ); } finally { _sceneShowVolumeColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneShowVolumeColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SceneShowVolumeColorChanged;
		ReferenceField<ColorValue> _sceneShowVolumeColor = new ColorValue( 0, 0, 1 );

		/// <summary>
		/// The color of sound source objects in the scene view.
		/// </summary>
		[DefaultValue( "0 0 1" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowSoundSourceColor
		{
			get { if( _sceneShowSoundSourceColor.BeginGet() ) SceneShowSoundSourceColor = _sceneShowSoundSourceColor.Get( this ); return _sceneShowSoundSourceColor.value; }
			set { if( _sceneShowSoundSourceColor.BeginSet( ref value ) ) { try { SceneShowSoundSourceColorChanged?.Invoke( this ); } finally { _sceneShowSoundSourceColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneShowSoundSourceColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SceneShowSoundSourceColorChanged;
		ReferenceField<ColorValue> _sceneShowSoundSourceColor = new ColorValue( 0, 0, 1 );

		//SceneShowObjectInSpaceBoundsColor
		ReferenceField<ColorValue> _sceneShowObjectInSpaceBoundsColor = new ColorValue( 1, 1, 0, .8 );
		/// <summary>
		/// The color of abstract objects in the scene view.
		/// </summary>
		[DefaultValue( "1 1 0 0.8" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowObjectInSpaceBoundsColor
		{
			get
			{
				if( _sceneShowObjectInSpaceBoundsColor.BeginGet() )
					SceneShowObjectInSpaceBoundsColor = _sceneShowObjectInSpaceBoundsColor.Get( this );
				return _sceneShowObjectInSpaceBoundsColor.value;
			}
			set
			{
				if( _sceneShowObjectInSpaceBoundsColor.BeginSet( ref value ) )
				{
					try { SceneShowObjectInSpaceBoundsColorChanged?.Invoke( this ); }
					finally { _sceneShowObjectInSpaceBoundsColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SceneShowObjectInSpaceBoundsColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SceneShowObjectInSpaceBoundsColorChanged;


		//SceneShowReflectionProbeColor
		ReferenceField<ColorValue> _sceneShowReflectionProbeColor = new ColorValue( 0, 0, 1 );
		/// <summary>
		/// The color of reflection probe objects in the scene view.
		/// </summary>
		[DefaultValue( "0 0 1" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowReflectionProbeColor
		{
			get
			{
				if( _sceneShowReflectionProbeColor.BeginGet() )
					SceneShowReflectionProbeColor = _sceneShowReflectionProbeColor.Get( this );
				return _sceneShowReflectionProbeColor.value;
			}
			set
			{
				if( _sceneShowReflectionProbeColor.BeginSet( ref value ) )
				{
					try { SceneShowReflectionProbeColorChanged?.Invoke( this ); }
					finally { _sceneShowReflectionProbeColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SceneShowReflectionProbeColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> SceneShowReflectionProbeColorChanged;

		/////////////////////////////////////////

		/// <summary>
		/// The number of simulation steps per second.
		/// </summary>
		[DefaultValue( 60.0 )]
		[Category( "Project Application" )]
		public Reference<double> SimulationStepsPerSecond
		{
			get { if( _simulationStepsPerSecond.BeginGet() ) SimulationStepsPerSecond = _simulationStepsPerSecond.Get( this ); return _simulationStepsPerSecond.value; }
			set
			{
				if( _simulationStepsPerSecond.BeginSet( ref value ) )
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
		[Category( "Project Application" )]
		[DisplayName( "Initial UI Screen" )]
		public Reference<ReferenceValueType_Resource> InitialUIScreen
		{
			get { if( _initialUIScreen.BeginGet() ) InitialUIScreen = _initialUIScreen.Get( this ); return _initialUIScreen.value; }
			set { if( _initialUIScreen.BeginSet( ref value ) ) { try { InitialUIScreenChanged?.Invoke( this ); } finally { _initialUIScreen.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InitialUIScreen"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> InitialUIScreenChanged;
		ReferenceField<ReferenceValueType_Resource> _initialUIScreen = new Reference<ReferenceValueType_Resource>( null, @"Base\UI\Screens\SplashScreen.ui" );

		/// <summary>
		/// A scene to run automatically when the Player started.
		/// </summary>
		[Category( "Project Application" )]
		[DefaultValue( null )]
		public Reference<ReferenceValueType_Resource> AutorunScene
		{
			get { if( _autorunScene.BeginGet() ) AutorunScene = _autorunScene.Get( this ); return _autorunScene.value; }
			set { if( _autorunScene.BeginSet( ref value ) ) { try { AutorunSceneChanged?.Invoke( this ); } finally { _autorunScene.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AutorunScene"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> AutorunSceneChanged;
		ReferenceField<ReferenceValueType_Resource> _autorunScene;

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
		[Category( "Project Application" )]
		[DefaultValue( WindowStateEnum.Auto )]
		public Reference<WindowStateEnum> WindowState
		{
			get { if( _windowState.BeginGet() ) WindowState = _windowState.Get( this ); return _windowState.value; }
			set { if( _windowState.BeginSet( ref value ) ) { try { WindowStateChanged?.Invoke( this ); } finally { _windowState.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WindowState"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> WindowStateChanged;
		ReferenceField<WindowStateEnum> _windowState = WindowStateEnum.Auto;

		public static Vector2I WindowSizeDefault = new Vector2I( 1300, 900 );
		/// <summary>
		/// The initial window size of the project window for Normal window state.
		/// </summary>
		[Category( "Project Application" )]
		[DefaultValue( "1300 900" )]
		public Reference<Vector2I> WindowSize
		{
			get { if( _windowSize.BeginGet() ) WindowSize = _windowSize.Get( this ); return _windowSize.value; }
			set { if( _windowSize.BeginSet( ref value ) ) { try { WindowSizeChanged?.Invoke( this ); } finally { _windowSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WindowSize"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> WindowSizeChanged;
		ReferenceField<Vector2I> _windowSize = WindowSizeDefault;

		/////////////////////////////////////////

		/// <summary>
		/// Whether to launch the Player in fullscreen mode.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Simulation" )]
		public Reference<bool> RunSimulationInFullscreen
		{
			get { if( _runSimulationInFullscreen.BeginGet() ) RunSimulationInFullscreen = _runSimulationInFullscreen.Get( this ); return _runSimulationInFullscreen.value; }
			set { if( _runSimulationInFullscreen.BeginSet( ref value ) ) { try { RunSimulationInFullscreenChanged?.Invoke( this ); } finally { _runSimulationInFullscreen.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunSimulationInFullscreen"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> RunSimulationInFullscreenChanged;
		ReferenceField<bool> _runSimulationInFullscreen = false;

		/////////////////////////////////////////

		[Category( "Preview" )]
		[DefaultValue( 50000.0 )]
		[Range( 0, 100000 )]
		public Reference<double> PreviewAmbientLightBrightness
		{
			get { if( _previewAmbientLightBrightness.BeginGet() ) PreviewAmbientLightBrightness = _previewAmbientLightBrightness.Get( this ); return _previewAmbientLightBrightness.value; }
			set { if( _previewAmbientLightBrightness.BeginSet( ref value ) ) { try { PreviewAmbientLightBrightnessChanged?.Invoke( this ); } finally { _previewAmbientLightBrightness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PreviewAmbientLightBrightness"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> PreviewAmbientLightBrightnessChanged;
		ReferenceField<double> _previewAmbientLightBrightness = 50000.0;

		[Category( "Preview" )]
		[DefaultValue( 200000.0 )]
		[Range( 0, 1000000 )]
		public Reference<double> PreviewDirectionalLightBrightness
		{
			get { if( _previewDirectionalLightBrightness.BeginGet() ) PreviewDirectionalLightBrightness = _previewDirectionalLightBrightness.Get( this ); return _previewDirectionalLightBrightness.value; }
			set { if( _previewDirectionalLightBrightness.BeginSet( ref value ) ) { try { PreviewDirectionalLightBrightnessChanged?.Invoke( this ); } finally { _previewDirectionalLightBrightness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PreviewDirectionalLightBrightness"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> PreviewDirectionalLightBrightnessChanged;
		ReferenceField<double> _previewDirectionalLightBrightness = 200000.0;

		[DefaultValueReference( @"Content\Environments\Base\Forest.image" )]
		[Category( "Preview" )]
		[DisplayName( "Material Preview Environment for Light Theme" )]
		public Reference<ImageComponent> MaterialPreviewEnvironmentLightTheme
		{
			get { if( _materialPreviewEnvironmentLightTheme.BeginGet() ) MaterialPreviewEnvironmentLightTheme = _materialPreviewEnvironmentLightTheme.Get( this ); return _materialPreviewEnvironmentLightTheme.value; }
			set { if( _materialPreviewEnvironmentLightTheme.BeginSet( ref value ) ) { try { MaterialPreviewEnvironmentLightThemeChanged?.Invoke( this ); } finally { _materialPreviewEnvironmentLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewEnvironmentLightTheme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> MaterialPreviewEnvironmentLightThemeChanged;
		ReferenceField<ImageComponent> _materialPreviewEnvironmentLightTheme = new Reference<ImageComponent>( null, @"Content\Environments\Base\Forest.image" );

		[DefaultValueReference( @"Base\Tools\Material Preview\Sphere.mesh" )]
		[Category( "Preview" )]
		public Reference<Mesh> MaterialPreviewMesh
		{
			get { if( _materialPreviewMesh.BeginGet() ) MaterialPreviewMesh = _materialPreviewMesh.Get( this ); return _materialPreviewMesh.value; }
			set { if( _materialPreviewMesh.BeginSet( ref value ) ) { try { MaterialPreviewMeshChanged?.Invoke( this ); } finally { _materialPreviewMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewMesh"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> MaterialPreviewMeshChanged;
		ReferenceField<Mesh> _materialPreviewMesh = new Reference<Mesh>( null, @"Base\Tools\Material Preview\Sphere.mesh" );

		[DefaultValueReference( @"Content\Environments\Base\Forest.image" )]
		[Category( "Preview" )]
		[DisplayName( "Material Preview Environment for Dark Theme" )]
		public Reference<ImageComponent> MaterialPreviewEnvironmentDarkTheme
		{
			get { if( _materialPreviewEnvironmentDarkTheme.BeginGet() ) MaterialPreviewEnvironmentDarkTheme = _materialPreviewEnvironmentDarkTheme.Get( this ); return _materialPreviewEnvironmentDarkTheme.value; }
			set { if( _materialPreviewEnvironmentDarkTheme.BeginSet( ref value ) ) { try { MaterialPreviewEnvironmentDarkThemeChanged?.Invoke( this ); } finally { _materialPreviewEnvironmentDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewEnvironmentDarkTheme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> MaterialPreviewEnvironmentDarkThemeChanged;
		ReferenceField<ImageComponent> _materialPreviewEnvironmentDarkTheme = new Reference<ImageComponent>( null, @"Content\Environments\Base\Forest.image" );

		public Reference<ImageComponent> GetMaterialPreviewEnvironment()
		{
			if( EditorAPI.DarkTheme )
				return MaterialPreviewEnvironmentDarkTheme;
			else
				return MaterialPreviewEnvironmentLightTheme;
		}

		[DefaultValue( "1 1 1" )]
		[ApplicableRangeColorValuePower( 0, 4 )]
		[ColorValueNoAlpha]
		[Category( "Preview" )]
		public Reference<ColorValuePowered> MaterialPreviewEnvironmentMultiplier
		{
			get { if( _materialPreviewEnvironmentMultiplier.BeginGet() ) MaterialPreviewEnvironmentMultiplier = _materialPreviewEnvironmentMultiplier.Get( this ); return _materialPreviewEnvironmentMultiplier.value; }
			set { if( _materialPreviewEnvironmentMultiplier.BeginSet( ref value ) ) { try { MaterialPreviewEnvironmentMultiplierChanged?.Invoke( this ); } finally { _materialPreviewEnvironmentMultiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewEnvironmentMultiplier"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> MaterialPreviewEnvironmentMultiplierChanged;
		ReferenceField<ColorValuePowered> _materialPreviewEnvironmentMultiplier = new ColorValuePowered( 1, 1, 1 );

		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		[Category( "Preview" )]
		public Reference<double> MaterialPreviewEnvironmentAffectLighting
		{
			get { if( _materialPreviewEnvironmentAffectLighting.BeginGet() ) MaterialPreviewEnvironmentAffectLighting = _materialPreviewEnvironmentAffectLighting.Get( this ); return _materialPreviewEnvironmentAffectLighting.value; }
			set { if( _materialPreviewEnvironmentAffectLighting.BeginSet( ref value ) ) { try { MaterialPreviewEnvironmentAffectLightingChanged?.Invoke( this ); } finally { _materialPreviewEnvironmentAffectLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewEnvironmentAffectLighting"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> MaterialPreviewEnvironmentAffectLightingChanged;
		ReferenceField<double> _materialPreviewEnvironmentAffectLighting = 0.5;

		/////////////////////////////////////////

		//!!!!descriptions

		[DefaultValue( 24 )]
		[Category( "Screen Labels" )]
		[Range( 10.0, 100.0, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> ScreenLabelMaxSize
		{
			get { if( _screenLabelMaxSize.BeginGet() ) ScreenLabelMaxSize = _screenLabelMaxSize.Get( this ); return _screenLabelMaxSize.value; }
			set { if( _screenLabelMaxSize.BeginSet( ref value ) ) { try { ScreenLabelMaxSizeChanged?.Invoke( this ); } finally { _screenLabelMaxSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ScreenLabelMaxSize"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> ScreenLabelMaxSizeChanged;
		ReferenceField<double> _screenLabelMaxSize = 24;

		[DefaultValue( 0.25 )]
		[Category( "Screen Labels" )]
		[Range( 0, 1 )]
		public Reference<double> ScreenLabelMinSizeFactor
		{
			get { if( _screenLabelMinSizeFactor.BeginGet() ) ScreenLabelMinSizeFactor = _screenLabelMinSizeFactor.Get( this ); return _screenLabelMinSizeFactor.value; }
			set { if( _screenLabelMinSizeFactor.BeginSet( ref value ) ) { try { ScreenLabelMinSizeFactorChanged?.Invoke( this ); } finally { _screenLabelMinSizeFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ScreenLabelMinSizeFactor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> ScreenLabelMinSizeFactorChanged;
		ReferenceField<double> _screenLabelMinSizeFactor = 0.25;

		[DefaultValue( 100 )]
		[Category( "Screen Labels" )]
		[Range( 1, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> ScreenLabelMaxDistance
		{
			get { if( _screenLabelMaxDistance.BeginGet() ) ScreenLabelMaxDistance = _screenLabelMaxDistance.Get( this ); return _screenLabelMaxDistance.value; }
			set { if( _screenLabelMaxDistance.BeginSet( ref value ) ) { try { ScreenLabelMaxDistanceChanged?.Invoke( this ); } finally { _screenLabelMaxDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ScreenLabelMaxDistance"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> ScreenLabelMaxDistanceChanged;
		ReferenceField<double> _screenLabelMaxDistance = 100;

		/// <summary>
		/// The color of screen labels.
		/// </summary>
		[DefaultValue( "1 1 1 0.5" )]
		[Category( "Screen Labels" )]
		public Reference<ColorValue> ScreenLabelColor
		{
			get { if( _screenLabelColor.BeginGet() ) ScreenLabelColor = _screenLabelColor.Get( this ); return _screenLabelColor.value; }
			set { if( _screenLabelColor.BeginSet( ref value ) ) { try { ScreenLabelColorChanged?.Invoke( this ); } finally { _screenLabelColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ScreenLabelColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> ScreenLabelColorChanged;
		ReferenceField<ColorValue> _screenLabelColor = new ColorValue( 1, 1, 1, 0.5 );

		[DefaultValue( true )]
		[Category( "Screen Labels" )]
		public Reference<bool> ScreenLabelDisplayIcons
		{
			get { if( _screenLabelDisplayIcons.BeginGet() ) ScreenLabelDisplayIcons = _screenLabelDisplayIcons.Get( this ); return _screenLabelDisplayIcons.value; }
			set { if( _screenLabelDisplayIcons.BeginSet( ref value ) ) { try { ScreenLabelDisplayIconsChanged?.Invoke( this ); } finally { _screenLabelDisplayIcons.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ScreenLabelDisplayIcons"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_General> ScreenLabelDisplayIconsChanged;
		ReferenceField<bool> _screenLabelDisplayIcons = true;

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( UserMode )
			{
				if( member is Metadata.Property )
				{
					//special app mode
					if( !EngineInfo.SpecialAppMode && member.Name == nameof( ExtendedMode ) )
						skip = true;
					if( !ExtendedMode && ProjectSettingsComponent.HidePropertiesForSpecialAppMode.Contains( member.Name ) )
						skip = true;
				}
			}
		}
	}
}
