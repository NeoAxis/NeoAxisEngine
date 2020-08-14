// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using NeoAxis.Editor;
using ComponentFactory.Krypton.Toolkit;
using System.Windows.Forms;

namespace NeoAxis
{
	/// <summary>
	/// Component representing the settings of the project.
	/// </summary>
	[EditorDocumentWindow( typeof( ObjectSettingsWindow ) )]
	public class Component_ProjectSettings : Component
	{
		protected override void OnDispose()
		{
			base.OnDispose();

			ProjectSettings._SettingsComponentSetNull();
		}

		[Browsable( false )]
		public bool UserMode
		{
			get
			{
				if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
				{
					var document = EditorUtility.GetDocumentByComponent( this );
					if( document != null && document.SpecialMode == "ProjectSettingsUserMode" )
						return true;
				}
				return false;
			}
		}

		public class MetadataGetMembersContextForPage : Metadata.GetMembersContext
		{
			public bool IsPage;
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( UserMode )
			{
				if( member is Metadata.Property )
				{
					//no parameters for root
					if( context == null || !( context is MetadataGetMembersContextForPage ) )
						skip = true;


					if( member.Name == "Name" || member.Name == "Enabled" )
						skip = true;

					if( EditorAPI.DarkTheme )
					{
						if( member.Name.Contains( "LightTheme" ) )
							skip = true;
					}
					else
					{
						if( member.Name.Contains( "DarkTheme" ) )
							skip = true;
					}

					//switch( member.Name )
					//{
					//case nameof( WindowSize ):
					//	if( WindowState.Value != WindowStateEnum.Normal )
					//		skip = true;
					//	break;
					//}

					//switch( member.Name )
					//{
					////case nameof( SplashScreenImage ):
					////case nameof( CustomizeSplashScreen ):
					//case nameof( EngineSplashScreenTime ):
					//	if( !EngineApp.IsProPlan )
					//		skip = true;
					//	break;

					////case nameof( SplashScreenImageReadOnly ):
					////case nameof( CustomizeSplashScreenReadOnly ):
					//case nameof( EngineSplashScreenTimeReadOnly ):
					//	if( EngineApp.IsProPlan )
					//		skip = true;
					//	break;
					//}
				}
			}
		}

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
		public event Action<Component_ProjectSettings> ProjectNameChanged;
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
		public Reference<LanguageEnum> Language
		{
			get { if( _language.BeginGet() ) Language = _language.Get( this ); return _language.value; }
			set { if( _language.BeginSet( ref value ) ) { try { LanguageChanged?.Invoke( this ); } finally { _language.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Language"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> LanguageChanged;
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
		public event Action<Component_ProjectSettings> MaxFramesPerSecondDocumentChanged;
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
		public event Action<Component_ProjectSettings> MaxFramesPerSecondPreviewChanged;
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
		public event Action<Component_ProjectSettings> SoundVolumeChanged;
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
		public event Action<Component_ProjectSettings> DisplayHierarchyOfObjectsInSettingsWindowChanged;
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
		public event Action<Component_ProjectSettings> PropertiesMaxCountCollectionItemsToDisplayChanged;
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
		public event Action<Component_ProjectSettings> AnimateAutoHideWindowsChanged;
		ReferenceField<bool> _animateAutoHideWindows = false;

		/// <summary>
		/// Whether to use custom style for window title bars. Windows only. Restart the editor to apply changes.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Editor" )]
		public Reference<bool> CustomWindowsStyle
		{
			get { if( _customWindowsStyle.BeginGet() ) CustomWindowsStyle = _customWindowsStyle.Get( this ); return _customWindowsStyle.value; }
			set { if( _customWindowsStyle.BeginSet( ref value ) ) { try { CustomWindowsStyleChanged?.Invoke( this ); } finally { _customWindowsStyle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CustomWindowsStyle"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CustomWindowsStyleChanged;
		ReferenceField<bool> _customWindowsStyle = true;

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
		public Reference<ThemeEnum> Theme
		{
			get { if( _theme.BeginGet() ) Theme = _theme.Get( this ); return _theme.value; }
			set { if( _theme.BeginSet( ref value ) ) { try { ThemeChanged?.Invoke( this ); } finally { _theme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Theme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ThemeChanged;
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
		public event Action<Component_ProjectSettings> SelectedColorChanged;


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
		public event Action<Component_ProjectSettings> CanSelectColorChanged;


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
		public event Action<Component_ProjectSettings> HiddenByOtherObjectsColorMultiplierChanged;


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
		public event Action<Component_ProjectSettings> SceneShowLightColorChanged;

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
		public event Action<Component_ProjectSettings> SceneShowDecalColorChanged;
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
		public event Action<Component_ProjectSettings> SceneShowPhysicsStaticColorChanged;


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
		public event Action<Component_ProjectSettings> SceneShowPhysicsDynamicActiveColorChanged;


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
		public event Action<Component_ProjectSettings> SceneShowPhysicsDynamicInactiveColorChanged;

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
		public event Action<Component_ProjectSettings> SceneShowAreaColorChanged;
		ReferenceField<ColorValue> _sceneShowAreaColor = new ColorValue( 0, 0, 1 );

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
		public event Action<Component_ProjectSettings> SceneShowSoundSourceColorChanged;
		ReferenceField<ColorValue> _sceneShowSoundSourceColor = new ColorValue( 0, 0, 1 );

		//ScreenLabelColor
		ReferenceField<ColorValue> _screenLabelColor = new ColorValue( 1, 1, 1 );
		/// <summary>
		/// The color of UI label objects in the scene view.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Colors" )]
		public Reference<ColorValue> ScreenLabelColor
		{
			get
			{
				if( _screenLabelColor.BeginGet() )
					ScreenLabelColor = _screenLabelColor.Get( this );
				return _screenLabelColor.value;
			}
			set
			{
				if( _screenLabelColor.BeginSet( ref value ) )
				{
					try { ScreenLabelColorChanged?.Invoke( this ); }
					finally { _screenLabelColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ScreenLabelColor"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ScreenLabelColorChanged;


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
		public event Action<Component_ProjectSettings> SceneShowObjectInSpaceBoundsColorChanged;


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
		public event Action<Component_ProjectSettings> SceneShowReflectionProbeColorChanged;

		/////////////////////////////////////////

		/// <summary>
		/// The number of simulation steps per second.
		/// </summary>
		[DefaultValue( 50.0 )]
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
		public event Action<Component_ProjectSettings> SimulationStepsPerSecondChanged;
		ReferenceField<double> _simulationStepsPerSecond = 50.0;

		////!!!!why no reference support?
		///// <summary>
		///// The number of simulation steps per second.
		///// </summary>
		//[Browsable( true )]
		//[DefaultValue( 50.0 )]
		//[Category( "Project Application" )]
		//public double SimulationStepsPerSecond
		//{
		//	get { return simulationStepsPerSecond; }
		//	set
		//	{
		//		if( simulationStepsPerSecond == value ) return;
		//		simulationStepsPerSecond = value;

		//		if( simulationStepsPerSecond != 0 )
		//			simulationStepsPerSecondInv = 1.0 / simulationStepsPerSecond;
		//	}
		//}
		//double simulationStepsPerSecond = 50;

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
		float simulationStepsPerSecondInv = 1.0f / 50;

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
		public event Action<Component_ProjectSettings> InitialUIScreenChanged;
		ReferenceField<ReferenceValueType_Resource> _initialUIScreen = new Reference<ReferenceValueType_Resource>( null, @"Base\UI\Screens\SplashScreen.ui" );

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
		public event Action<Component_ProjectSettings> WindowStateChanged;
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
		public event Action<Component_ProjectSettings> WindowSizeChanged;
		ReferenceField<Vector2I> _windowSize = WindowSizeDefault;

		/////////////////////////////////////////

		[DefaultValue( false )]
		[Category( "Simulation" )]
		public Reference<bool> RunSimulationInFullscreen
		{
			get { if( _runSimulationInFullscreen.BeginGet() ) RunSimulationInFullscreen = _runSimulationInFullscreen.Get( this ); return _runSimulationInFullscreen.value; }
			set { if( _runSimulationInFullscreen.BeginSet( ref value ) ) { try { RunSimulationInFullscreenChanged?.Invoke( this ); } finally { _runSimulationInFullscreen.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunSimulationInFullscreen"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> RunSimulationInFullscreenChanged;
		ReferenceField<bool> _runSimulationInFullscreen = false;

		/////////////////////////////////////////

		[Category( "Rendering" )]
		[DefaultValue( 1.5 )]
		public Reference<double> LineThickness
		{
			get { if( _lineThickness.BeginGet() ) LineThickness = _lineThickness.Get( this ); return _lineThickness.value; }
			set { if( _lineThickness.BeginSet( ref value ) ) { try { LineThicknessChanged?.Invoke( this ); } finally { _lineThickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LineThickness"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> LineThicknessChanged;
		ReferenceField<double> _lineThickness = 1.5;

		//!!!!нельзя грузить до инициализации рендера, т.к. MaterialEnvironmentPreview загружается
		//public enum RenderingAPIEnum
		//{
		//	Auto,
		//	DirectX11,
		//	DirectX12,
		//}
		///// <summary>
		///// Rendering API used in the project. Changing the value of this parameter takes effect after the restart of the editor.
		///// </summary>
		//[Category( "Rendering" )]
		//[DefaultValue( RenderingAPIEnum.Auto )]
		//public Reference<RenderingAPIEnum> RenderingAPI
		//{
		//	get { if( _renderingAPI.BeginGet() ) RenderingAPI = _renderingAPI.Get( this ); return _renderingAPI.value; }
		//	set { if( _renderingAPI.BeginSet( ref value ) ) { try { RenderingAPIChanged?.Invoke( this ); } finally { _renderingAPI.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RenderingAPI"/> property value changes.</summary>
		//public event Action<Component_ProjectSettings> RenderingAPIChanged;
		//ReferenceField<RenderingAPIEnum> _renderingAPI = RenderingAPIEnum.Auto;

		/////////////////////////////////////////

		[Category( "Preview" )]
		[DefaultValue( 30000.0 )]
		[Range( 0, 100000 )]
		public Reference<double> PreviewAmbientLightBrightness
		{
			get { if( _previewAmbientLightBrightness.BeginGet() ) PreviewAmbientLightBrightness = _previewAmbientLightBrightness.Get( this ); return _previewAmbientLightBrightness.value; }
			set { if( _previewAmbientLightBrightness.BeginSet( ref value ) ) { try { PreviewAmbientLightBrightnessChanged?.Invoke( this ); } finally { _previewAmbientLightBrightness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PreviewAmbientLightBrightness"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> PreviewAmbientLightBrightnessChanged;
		ReferenceField<double> _previewAmbientLightBrightness = 30000.0;

		[Category( "Preview" )]
		[DefaultValue( 250000.0 )]
		[Range( 0, 1000000 )]
		public Reference<double> PreviewDirectionalLightBrightness
		{
			get { if( _previewDirectionalLightBrightness.BeginGet() ) PreviewDirectionalLightBrightness = _previewDirectionalLightBrightness.Get( this ); return _previewDirectionalLightBrightness.value; }
			set { if( _previewDirectionalLightBrightness.BeginSet( ref value ) ) { try { PreviewDirectionalLightBrightnessChanged?.Invoke( this ); } finally { _previewDirectionalLightBrightness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PreviewDirectionalLightBrightness"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> PreviewDirectionalLightBrightnessChanged;
		ReferenceField<double> _previewDirectionalLightBrightness = 250000.0;

		[DefaultValueReference( @"Base\Tools\Material Preview\Sphere.mesh" )]
		[Category( "Preview" )]
		public Reference<Component_Mesh> MaterialPreviewMesh
		{
			get { if( _materialPreviewMesh.BeginGet() ) MaterialPreviewMesh = _materialPreviewMesh.Get( this ); return _materialPreviewMesh.value; }
			set { if( _materialPreviewMesh.BeginSet( ref value ) ) { try { MaterialPreviewMeshChanged?.Invoke( this ); } finally { _materialPreviewMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewMesh"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> MaterialPreviewMeshChanged;
		ReferenceField<Component_Mesh> _materialPreviewMesh = new Reference<Component_Mesh>( null, @"Base\Tools\Material Preview\Sphere.mesh" );

		[DefaultValueReference( @"Base\Environments\Gradient.image" )]
		[Category( "Preview" )]
		[DisplayName( "Material Preview Environment for Light Theme" )]
		public Reference<Component_Image> MaterialPreviewEnvironmentLightTheme
		{
			get { if( _materialPreviewEnvironmentLightTheme.BeginGet() ) MaterialPreviewEnvironmentLightTheme = _materialPreviewEnvironmentLightTheme.Get( this ); return _materialPreviewEnvironmentLightTheme.value; }
			set { if( _materialPreviewEnvironmentLightTheme.BeginSet( ref value ) ) { try { MaterialPreviewEnvironmentLightThemeChanged?.Invoke( this ); } finally { _materialPreviewEnvironmentLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewEnvironmentLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> MaterialPreviewEnvironmentLightThemeChanged;
		ReferenceField<Component_Image> _materialPreviewEnvironmentLightTheme = new Reference<Component_Image>( null, @"Base\Environments\Gradient.image" );

		[DefaultValueReference( @"Base\Environments\Gradient.image" )]
		[Category( "Preview" )]
		[DisplayName( "Material Preview Environment for Dark Theme" )]
		public Reference<Component_Image> MaterialPreviewEnvironmentDarkTheme
		{
			get { if( _materialPreviewEnvironmentDarkTheme.BeginGet() ) MaterialPreviewEnvironmentDarkTheme = _materialPreviewEnvironmentDarkTheme.Get( this ); return _materialPreviewEnvironmentDarkTheme.value; }
			set { if( _materialPreviewEnvironmentDarkTheme.BeginSet( ref value ) ) { try { MaterialPreviewEnvironmentDarkThemeChanged?.Invoke( this ); } finally { _materialPreviewEnvironmentDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewEnvironmentDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> MaterialPreviewEnvironmentDarkThemeChanged;
		ReferenceField<Component_Image> _materialPreviewEnvironmentDarkTheme = new Reference<Component_Image>( null, @"Base\Environments\Gradient.image" );

		public Reference<Component_Image> GetMaterialPreviewEnvironment()
		{
			if( EditorAPI.DarkTheme )
				return MaterialPreviewEnvironmentDarkTheme;
			else
				return MaterialPreviewEnvironmentLightTheme;
		}

		/////////////////////////////////////////

		/// <summary>
		/// The snap value applied when object is moved.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Scene Editor: Snapping" )]
		[DisplayName( "Step Movement" )]
		public Reference<double> SceneEditorStepMovement
		{
			get { if( _sceneEditorStepMovement.BeginGet() ) SceneEditorStepMovement = _sceneEditorStepMovement.Get( this ); return _sceneEditorStepMovement.value; }
			set { if( _sceneEditorStepMovement.BeginSet( ref value ) ) { try { SceneEditorStepMovementChanged?.Invoke( this ); } finally { _sceneEditorStepMovement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneEditorStepMovement"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> SceneEditorStepMovementChanged;
		ReferenceField<double> _sceneEditorStepMovement = 1.0;

		/// <summary>
		/// The snap value applied when object is rotated.
		/// </summary>
		[DefaultValue( "10" )]
		[Category( "Scene Editor: Snapping" )]
		[DisplayName( "Step Rotation" )]
		public Reference<Degree> SceneEditorStepRotation
		{
			get { if( _sceneEditorStepRotation.BeginGet() ) SceneEditorStepRotation = _sceneEditorStepRotation.Get( this ); return _sceneEditorStepRotation.value; }
			set { if( _sceneEditorStepRotation.BeginSet( ref value ) ) { try { SceneEditorStepRotationChanged?.Invoke( this ); } finally { _sceneEditorStepRotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneEditorStepRotation"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> SceneEditorStepRotationChanged;
		ReferenceField<Degree> _sceneEditorStepRotation = new Degree( 10 );

		/// <summary>
		/// The snap value applied when object is scaled.
		/// </summary>
		[DefaultValue( 0.1 )]
		[Category( "Scene Editor: Snapping" )]
		[DisplayName( "Step Scaling" )]
		public Reference<double> SceneEditorStepScaling
		{
			get { if( _sceneEditorStepScaling.BeginGet() ) SceneEditorStepScaling = _sceneEditorStepScaling.Get( this ); return _sceneEditorStepScaling.value; }
			set { if( _sceneEditorStepScaling.BeginSet( ref value ) ) { try { SceneEditorStepScalingChanged?.Invoke( this ); } finally { _sceneEditorStepScaling.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneEditorStepScaling"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> SceneEditorStepScalingChanged;
		ReferenceField<double> _sceneEditorStepScaling = 0.1;

		/////////////////////////////////////////

		//TransformToolRotationSensitivity
		ReferenceField<double> _transformToolRotationSensitivity = 1.0;
		/// <summary>
		/// The sensitivity of the object rotation.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Scene Editor: Transform Tool" )]
		[DisplayName( "Rotation Sensitivity" )]
		[Range( 0.1, 10.0 )]
		public Reference<double> TransformToolRotationSensitivity
		{
			get { if( _transformToolRotationSensitivity.BeginGet() ) TransformToolRotationSensitivity = _transformToolRotationSensitivity.Get( this ); return _transformToolRotationSensitivity.value; }
			set { if( _transformToolRotationSensitivity.BeginSet( ref value ) ) { try { TransformToolRotationSensitivityChanged?.Invoke( this ); } finally { _transformToolRotationSensitivity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransformToolRotationSensitivity"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TransformToolRotationSensitivityChanged;

		//TransformToolSize
		ReferenceField<int> _transformToolSize = 125;
		/// <summary>
		/// The size of the transform tool.
		/// </summary>
		[DefaultValue( 125 )]
		[Category( "Scene Editor: Transform Tool" )]
		[DisplayName( "Size" )]
		[Range( 50, 300 )]
		public Reference<int> TransformToolSize
		{
			get { if( _transformToolSize.BeginGet() ) TransformToolSize = _transformToolSize.Get( this ); return _transformToolSize.value; }
			set { if( _transformToolSize.BeginSet( ref value ) ) { try { TransformToolSizeChanged?.Invoke( this ); } finally { _transformToolSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransformToolSize"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TransformToolSizeChanged;

		[Browsable( false )]
		public double TransformToolSizeScaled
		{
			get
			{
				var result = (double)TransformToolSize;
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
				{
					try
					{
						result *= DpiHelper.Default.DpiScaleFactor;
					}
					catch { }
				}
				return result;
			}
		}

		//TransformToolLineThickness
		ReferenceField<double> _transformToolLineThickness = 2.0;
		/// <summary>
		/// The thickness of the transform tool line.
		/// </summary>
		[DefaultValue( 2.0 )]
		[Category( "Scene Editor: Transform Tool" )]
		[DisplayName( "Line Thickness" )]
		[Range( 1.0, 5.0 )]
		public Reference<double> TransformToolLineThickness
		{
			get { if( _transformToolLineThickness.BeginGet() ) TransformToolLineThickness = _transformToolLineThickness.Get( this ); return _transformToolLineThickness.value; }
			set { if( _transformToolLineThickness.BeginSet( ref value ) ) { try { TransformToolLineThicknessChanged?.Invoke( this ); } finally { _transformToolLineThickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransformToolLineThickness"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TransformToolLineThicknessChanged;

		[Browsable( false )]
		public double TransformToolLineThicknessScaled
		{
			get
			{
				var result = (double)TransformToolLineThickness;
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
				{
					try
					{
						result *= DpiHelper.Default.DpiScaleFactor;
					}
					catch { }
				}
				return result;
			}
		}

		//TransformToolShadowIntensity
		ReferenceField<double> _transformToolShadowIntensity = 0.2;
		/// <summary>
		/// The intensity of the shadows drawn by the transform tool.
		/// </summary>
		[DefaultValue( 0.2 )]//0.3
		[Category( "Scene Editor: Transform Tool" )]
		[DisplayName( "Shadow Intensity" )]
		[Range( 0, 1 )]
		public Reference<double> TransformToolShadowIntensity
		{
			get { if( _transformToolShadowIntensity.BeginGet() ) TransformToolShadowIntensity = _transformToolShadowIntensity.Get( this ); return _transformToolShadowIntensity.value; }
			set { if( _transformToolShadowIntensity.BeginSet( ref value ) ) { try { TransformToolShadowIntensityChanged?.Invoke( this ); } finally { _transformToolShadowIntensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransformToolShadowIntensity"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TransformToolShadowIntensityChanged;

		/////////////////////////////////////////

		//CameraKeyboardMovementSpeedNormal
		ReferenceField<double> _cameraKeyboardMovementSpeedNormal = 5.0;
		/// <summary>
		/// The normal keyboard speed of the camera movement.
		/// </summary>
		[DefaultValue( 5.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Keyboard Movement Speed Normal" )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraKeyboardMovementSpeedNormal
		{
			get { if( _cameraKeyboardMovementSpeedNormal.BeginGet() ) CameraKeyboardMovementSpeedNormal = _cameraKeyboardMovementSpeedNormal.Get( this ); return _cameraKeyboardMovementSpeedNormal.value; }
			set { if( _cameraKeyboardMovementSpeedNormal.BeginSet( ref value ) ) { try { CameraKeyboardMovementSpeedNormalChanged?.Invoke( this ); } finally { _cameraKeyboardMovementSpeedNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraKeyboardMovementSpeedNormal"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CameraKeyboardMovementSpeedNormalChanged;

		//CameraKeyboardMovementSpeedFast
		ReferenceField<double> _cameraKeyboardMovementSpeedFast = 40.0;
		/// <summary>
		/// The keyboard speed of the camera movement in fast mode. Hold Shift key to turn the fast camera mode.
		/// </summary>
		[DefaultValue( 40.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Keyboard Movement Speed Fast" )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraKeyboardMovementSpeedFast
		{
			get { if( _cameraKeyboardMovementSpeedFast.BeginGet() ) CameraKeyboardMovementSpeedFast = _cameraKeyboardMovementSpeedFast.Get( this ); return _cameraKeyboardMovementSpeedFast.value; }
			set { if( _cameraKeyboardMovementSpeedFast.BeginSet( ref value ) ) { try { CameraKeyboardMovementSpeedFastChanged?.Invoke( this ); } finally { _cameraKeyboardMovementSpeedFast.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraKeyboardMovementSpeedFast"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CameraKeyboardMovementSpeedFastChanged;

		//CameraKeyboardRotationSpeedNormal
		ReferenceField<Degree> _cameraKeyboardRotationSpeedNormal = new Degree( 30 );
		/// <summary>
		/// The normal keyboard speed of the camera rotation.
		/// </summary>
		[DefaultValue( "30" )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Keyboard Rotation Speed Normal" )]
		[Range( 1, 360, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<Degree> CameraKeyboardRotationSpeedNormal
		{
			get { if( _cameraKeyboardRotationSpeedNormal.BeginGet() ) CameraKeyboardRotationSpeedNormal = _cameraKeyboardRotationSpeedNormal.Get( this ); return _cameraKeyboardRotationSpeedNormal.value; }
			set { if( _cameraKeyboardRotationSpeedNormal.BeginSet( ref value ) ) { try { CameraKeyboardRotationSpeedNormalChanged?.Invoke( this ); } finally { _cameraKeyboardRotationSpeedNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraKeyboardRotationSpeedNormal"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CameraKeyboardRotationSpeedNormalChanged;

		//CameraKeyboardRotationSpeedFast
		ReferenceField<Degree> _cameraKeyboardRotationSpeedFast = new Degree( 90 );
		/// <summary>
		/// The keyboard speed of the camera rotation in fast mode.
		/// </summary>
		[DefaultValue( "90" )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Keyboard Rotation Speed Fast" )]
		[Range( 1, 360, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<Degree> CameraKeyboardRotationSpeedFast
		{
			get { if( _cameraKeyboardRotationSpeedFast.BeginGet() ) CameraKeyboardRotationSpeedFast = _cameraKeyboardRotationSpeedFast.Get( this ); return _cameraKeyboardRotationSpeedFast.value; }
			set { if( _cameraKeyboardRotationSpeedFast.BeginSet( ref value ) ) { try { CameraKeyboardRotationSpeedFastChanged?.Invoke( this ); } finally { _cameraKeyboardRotationSpeedFast.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraKeyboardRotationSpeedFast"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CameraKeyboardRotationSpeedFastChanged;

		//CameraMouseMovementSensitivityNormal
		ReferenceField<double> _cameraMouseMovementSensitivityNormal = 1.0;
		/// <summary>
		/// The normal mouse sensitivity of the camera movement.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Movement Sensitivity Normal" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseMovementSensitivityNormal
		{
			get { if( _cameraMouseMovementSensitivityNormal.BeginGet() ) CameraMouseMovementSensitivityNormal = _cameraMouseMovementSensitivityNormal.Get( this ); return _cameraMouseMovementSensitivityNormal.value; }
			set { if( _cameraMouseMovementSensitivityNormal.BeginSet( ref value ) ) { try { CameraMouseMovementSensitivityNormalChanged?.Invoke( this ); } finally { _cameraMouseMovementSensitivityNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseMovementSensitivityNormal"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CameraMouseMovementSensitivityNormalChanged;

		//CameraMouseMovementSensitivityFast
		ReferenceField<double> _cameraMouseMovementSensitivityFast = 5.0;
		/// <summary>
		/// The mouse sensitivity of the camera movement in fast mode.
		/// </summary>
		[DefaultValue( 5.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Movement Sensitivity Fast" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseMovementSensitivityFast
		{
			get { if( _cameraMouseMovementSensitivityFast.BeginGet() ) CameraMouseMovementSensitivityFast = _cameraMouseMovementSensitivityFast.Get( this ); return _cameraMouseMovementSensitivityFast.value; }
			set { if( _cameraMouseMovementSensitivityFast.BeginSet( ref value ) ) { try { CameraMouseMovementSensitivityFastChanged?.Invoke( this ); } finally { _cameraMouseMovementSensitivityFast.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseMovementSensitivityFast"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CameraMouseMovementSensitivityFastChanged;

		//CameraMouseRotationSensitivityHorizontal
		ReferenceField<double> _cameraMouseRotationSensitivityHorizontal = 1.0;
		/// <summary>
		/// The horizontal mouse sensitivity of the camera rotation.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Rotation Sensitivity Horizontal" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseRotationSensitivityHorizontal
		{
			get { if( _cameraMouseRotationSensitivityHorizontal.BeginGet() ) CameraMouseRotationSensitivityHorizontal = _cameraMouseRotationSensitivityHorizontal.Get( this ); return _cameraMouseRotationSensitivityHorizontal.value; }
			set { if( _cameraMouseRotationSensitivityHorizontal.BeginSet( ref value ) ) { try { CameraMouseRotationSensitivityHorizontalChanged?.Invoke( this ); } finally { _cameraMouseRotationSensitivityHorizontal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseRotationSensitivityHorizontal"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CameraMouseRotationSensitivityHorizontalChanged;

		////CameraMouseRotationSensitivityHorizontalNormal
		//ReferenceField<double> _cameraMouseRotationSensitivityHorizontalNormal = 1.0;
		//[DefaultValue( 1.0 )]
		//[Category( "Camera" )]
		//[DisplayName( "Mouse Rotation Sensitivity Horizontal Normal" )]
		//[ApplicableRange( 0.1, 10, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> CameraMouseRotationSensitivityHorizontalNormal
		//{
		//	get { if( _cameraMouseRotationSensitivityHorizontalNormal.BeginGet() ) CameraMouseRotationSensitivityHorizontalNormal = _cameraMouseRotationSensitivityHorizontalNormal.Get( this ); return _cameraMouseRotationSensitivityHorizontalNormal.value; }
		//	set { if( _cameraMouseRotationSensitivityHorizontalNormal.BeginSet( ref value ) ) { try { CameraMouseRotationSensitivityHorizontalNormalChanged?.Invoke( this ); } finally { _cameraMouseRotationSensitivityHorizontalNormal.EndSet(); } } }
		//}
		//public event Action<Component_ProjectSettings> CameraMouseRotationSensitivityHorizontalNormalChanged;

		////CameraMouseRotationSensitivityHorizontalFast
		//ReferenceField<double> _cameraMouseRotationSensitivityHorizontalFast = 3.0;
		//[DefaultValue( 3.0 )]
		//[Category( "Camera" )]
		//[DisplayName( "Mouse Rotation Sensitivity Horizontal Fast" )]
		//[ApplicableRange( 0.1, 10, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> CameraMouseRotationSensitivityHorizontalFast
		//{
		//	get { if( _cameraMouseRotationSensitivityHorizontalFast.BeginGet() ) CameraMouseRotationSensitivityHorizontalFast = _cameraMouseRotationSensitivityHorizontalFast.Get( this ); return _cameraMouseRotationSensitivityHorizontalFast.value; }
		//	set { if( _cameraMouseRotationSensitivityHorizontalFast.BeginSet( ref value ) ) { try { CameraMouseRotationSensitivityHorizontalFastChanged?.Invoke( this ); } finally { _cameraMouseRotationSensitivityHorizontalFast.EndSet(); } } }
		//}
		//public event Action<Component_ProjectSettings> CameraMouseRotationSensitivityHorizontalFastChanged;

		//CameraMouseRotationSensitivityVertical
		ReferenceField<double> _cameraMouseRotationSensitivityVertical = 1.0;
		/// <summary>
		/// The vertical mouse sensitivity of the camera rotation.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Rotation Sensitivity Vertical" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseRotationSensitivityVertical
		{
			get { if( _cameraMouseRotationSensitivityVertical.BeginGet() ) CameraMouseRotationSensitivityVertical = _cameraMouseRotationSensitivityVertical.Get( this ); return _cameraMouseRotationSensitivityVertical.value; }
			set { if( _cameraMouseRotationSensitivityVertical.BeginSet( ref value ) ) { try { CameraMouseRotationSensitivityVerticalChanged?.Invoke( this ); } finally { _cameraMouseRotationSensitivityVertical.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseRotationSensitivityVertical"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CameraMouseRotationSensitivityVerticalChanged;

		////CameraMouseRotationSensitivityVerticalNormal
		//ReferenceField<double> _cameraMouseRotationSensitivityVerticalNormal = 1.0;
		//[DefaultValue( 1.0 )]
		//[Category( "Camera" )]
		//[DisplayName( "Mouse Rotation Sensitivity Vertical Normal" )]
		//[ApplicableRange( 0.1, 10, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> CameraMouseRotationSensitivityVerticalNormal
		//{
		//	get { if( _cameraMouseRotationSensitivityVerticalNormal.BeginGet() ) CameraMouseRotationSensitivityVerticalNormal = _cameraMouseRotationSensitivityVerticalNormal.Get( this ); return _cameraMouseRotationSensitivityVerticalNormal.value; }
		//	set { if( _cameraMouseRotationSensitivityVerticalNormal.BeginSet( ref value ) ) { try { CameraMouseRotationSensitivityVerticalNormalChanged?.Invoke( this ); } finally { _cameraMouseRotationSensitivityVerticalNormal.EndSet(); } } }
		//}
		//public event Action<Component_ProjectSettings> CameraMouseRotationSensitivityVerticalNormalChanged;

		////CameraMouseRotationSensitivityVerticalFast
		//ReferenceField<double> _cameraMouseRotationSensitivityVerticalFast = 3.0;
		//[DefaultValue( 3.0 )]
		//[Category( "Camera" )]
		//[DisplayName( "Mouse Rotation Sensitivity Vertical Fast" )]
		//[ApplicableRange( 0.1, 10, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> CameraMouseRotationSensitivityVerticalFast
		//{
		//	get { if( _cameraMouseRotationSensitivityVerticalFast.BeginGet() ) CameraMouseRotationSensitivityVerticalFast = _cameraMouseRotationSensitivityVerticalFast.Get( this ); return _cameraMouseRotationSensitivityVerticalFast.value; }
		//	set { if( _cameraMouseRotationSensitivityVerticalFast.BeginSet( ref value ) ) { try { CameraMouseRotationSensitivityVerticalFastChanged?.Invoke( this ); } finally { _cameraMouseRotationSensitivityVerticalFast.EndSet(); } } }
		//}
		//public event Action<Component_ProjectSettings> CameraMouseRotationSensitivityVerticalFastChanged;

		//CameraMouseTrackMovementSensitivityNormal
		ReferenceField<double> _cameraMouseTrackMovementSensitivityNormal = 5.0;
		/// <summary>
		/// The normal mouse sensitivity of the camera movement.
		/// </summary>
		[DefaultValue( 5.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Track Movement Sensitivity Normal" )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseTrackMovementSensitivityNormal
		{
			get { if( _cameraMouseTrackMovementSensitivityNormal.BeginGet() ) CameraMouseTrackMovementSensitivityNormal = _cameraMouseTrackMovementSensitivityNormal.Get( this ); return _cameraMouseTrackMovementSensitivityNormal.value; }
			set { if( _cameraMouseTrackMovementSensitivityNormal.BeginSet( ref value ) ) { try { CameraMouseTrackMovementSensitivityNormalChanged?.Invoke( this ); } finally { _cameraMouseTrackMovementSensitivityNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseTrackMovementSensitivityNormal"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CameraMouseTrackMovementSensitivityNormalChanged;

		//CameraMouseTrackMovementSensitivityFast
		ReferenceField<double> _cameraMouseTrackMovementSensitivityFast = 15.0;
		/// <summary>
		/// The mouse sensitivity of the camera movement in fast mode.
		/// </summary>
		[DefaultValue( 15.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Track Movement Sensitivity Fast" )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseTrackMovementSensitivityFast
		{
			get { if( _cameraMouseTrackMovementSensitivityFast.BeginGet() ) CameraMouseTrackMovementSensitivityFast = _cameraMouseTrackMovementSensitivityFast.Get( this ); return _cameraMouseTrackMovementSensitivityFast.value; }
			set { if( _cameraMouseTrackMovementSensitivityFast.BeginSet( ref value ) ) { try { CameraMouseTrackMovementSensitivityFastChanged?.Invoke( this ); } finally { _cameraMouseTrackMovementSensitivityFast.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseTrackMovementSensitivityFast"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CameraMouseTrackMovementSensitivityFastChanged;

		//CameraMouseWheelMovementSensitivityNormal
		ReferenceField<double> _cameraMouseWheelMovementSensitivityNormal = 1.0;
		/// <summary>
		/// The normal mouse wheel sensitivity of the camera movement.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Wheel Movement Sensitivity Normal" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseWheelMovementSensitivityNormal
		{
			get { if( _cameraMouseWheelMovementSensitivityNormal.BeginGet() ) CameraMouseWheelMovementSensitivityNormal = _cameraMouseWheelMovementSensitivityNormal.Get( this ); return _cameraMouseWheelMovementSensitivityNormal.value; }
			set { if( _cameraMouseWheelMovementSensitivityNormal.BeginSet( ref value ) ) { try { CameraMouseWheelMovementSensitivityNormalChanged?.Invoke( this ); } finally { _cameraMouseWheelMovementSensitivityNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseWheelMovementSensitivityNormal"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CameraMouseWheelMovementSensitivityNormalChanged;

		//CameraMouseWheelMovementSensitivityFast
		ReferenceField<double> _cameraMouseWheelMovementSensitivityFast = 5.0;
		/// <summary>
		/// The mouse wheel sensitivity of the camera movement in fast mode.
		/// </summary>
		[DefaultValue( 5.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Wheel Movement Sensitivity Fast" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseWheelMovementSensitivityFast
		{
			get { if( _cameraMouseWheelMovementSensitivityFast.BeginGet() ) CameraMouseWheelMovementSensitivityFast = _cameraMouseWheelMovementSensitivityFast.Get( this ); return _cameraMouseWheelMovementSensitivityFast.value; }
			set { if( _cameraMouseWheelMovementSensitivityFast.BeginSet( ref value ) ) { try { CameraMouseWheelMovementSensitivityFastChanged?.Invoke( this ); } finally { _cameraMouseWheelMovementSensitivityFast.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseWheelMovementSensitivityFast"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CameraMouseWheelMovementSensitivityFastChanged;

		/////////////////////////////////////////

		/// <summary>
		/// The radius of the selection of objects with a double click.
		/// </summary>
		[DefaultValue( 20.0 )]
		[Category( "Scene Editor: Select" )]
		[DisplayName( "Select By Double Click Radius" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> SceneEditorSelectByDoubleClickRadius
		{
			get { if( _sceneEditorSelectByDoubleClickRadius.BeginGet() ) SceneEditorSelectByDoubleClickRadius = _sceneEditorSelectByDoubleClickRadius.Get( this ); return _sceneEditorSelectByDoubleClickRadius.value; }
			set { if( _sceneEditorSelectByDoubleClickRadius.BeginSet( ref value ) ) { try { SceneEditorSelectByDoubleClickRadiusChanged?.Invoke( this ); } finally { _sceneEditorSelectByDoubleClickRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneEditorSelectByDoubleClickRadius"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> SceneEditorSelectByDoubleClickRadiusChanged;
		ReferenceField<double> _sceneEditorSelectByDoubleClickRadius = 20.0;

		/////////////////////////////////////////

		/// <summary>
		/// The aspect ratio of the canvas of UI Editor.
		/// </summary>
		[Category( "UI Editor: General" )]
		[DisplayName( "Editor Aspect Ratio" )]
		[DefaultValue( 1.77777777777 )]
		public Reference<double> UIEditorAspectRatio
		{
			get { if( _uiEditorAspectRatio.BeginGet() ) UIEditorAspectRatio = _uiEditorAspectRatio.Get( this ); return _uiEditorAspectRatio.value; }
			set { if( _uiEditorAspectRatio.BeginSet( ref value ) ) { try { UIEditorAspectRatioChanged?.Invoke( this ); } finally { _uiEditorAspectRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIEditorAspectRatio"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> UIEditorAspectRatioChanged;
		ReferenceField<double> _uiEditorAspectRatio = 1.77777777777;

		[Category( "UI Editor: General" )]
		[DisplayName( "Display Grid" )]
		[DefaultValue( true )]
		public Reference<bool> UIEditorDisplayGrid
		{
			get { if( _uIEditorDisplayGrid.BeginGet() ) UIEditorDisplayGrid = _uIEditorDisplayGrid.Get( this ); return _uIEditorDisplayGrid.value; }
			set { if( _uIEditorDisplayGrid.BeginSet( ref value ) ) { try { UIEditorDisplayGridChanged?.Invoke( this ); } finally { _uIEditorDisplayGrid.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIEditorDisplayGrid"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> UIEditorDisplayGridChanged;
		ReferenceField<bool> _uIEditorDisplayGrid = true;

		/////////////////////////////////////////

		[Category( "UI Editor: Snapping" )]
		[DisplayName( "Parent Measure: Step Movement" )]
		[DefaultValue( 0.01 )]
		public Reference<double> UIEditorParentMeasureStepMovement
		{
			get { if( _uIEditorParentMeasureStepMovement.BeginGet() ) UIEditorParentMeasureStepMovement = _uIEditorParentMeasureStepMovement.Get( this ); return _uIEditorParentMeasureStepMovement.value; }
			set { if( _uIEditorParentMeasureStepMovement.BeginSet( ref value ) ) { try { UIEditorParentMeasureStepMovementChanged?.Invoke( this ); } finally { _uIEditorParentMeasureStepMovement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIEditorParentMeasureStepMovement"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> UIEditorParentMeasureStepMovementChanged;
		ReferenceField<double> _uIEditorParentMeasureStepMovement = 0.01;

		[Category( "UI Editor: Snapping" )]
		[DisplayName( "Units Measure: Step Movement" )]
		[DefaultValue( 10.0 )]
		public Reference<double> UIEditorUnitsMeasureStepMovement
		{
			get { if( _uIEditorUnitsMeasureStepMovement.BeginGet() ) UIEditorUnitsMeasureStepMovement = _uIEditorUnitsMeasureStepMovement.Get( this ); return _uIEditorUnitsMeasureStepMovement.value; }
			set { if( _uIEditorUnitsMeasureStepMovement.BeginSet( ref value ) ) { try { UIEditorUnitsMeasureStepMovementChanged?.Invoke( this ); } finally { _uIEditorUnitsMeasureStepMovement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIEditorUnitsMeasureStepMovement"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> UIEditorUnitsMeasureStepMovementChanged;
		ReferenceField<double> _uIEditorUnitsMeasureStepMovement = 10.0;

		[Category( "UI Editor: Snapping" )]
		[DisplayName( "Pixels Measure: Step Movement" )]
		[DefaultValue( 10.0 )]
		public Reference<double> UIEditorPixelsMeasureStepMovement
		{
			get { if( _uIEditorPixelsMeasureStepMovement.BeginGet() ) UIEditorPixelsMeasureStepMovement = _uIEditorPixelsMeasureStepMovement.Get( this ); return _uIEditorPixelsMeasureStepMovement.value; }
			set { if( _uIEditorPixelsMeasureStepMovement.BeginSet( ref value ) ) { try { UIEditorPixelsMeasureStepMovementChanged?.Invoke( this ); } finally { _uIEditorPixelsMeasureStepMovement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIEditorPixelsMeasureStepMovement"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> UIEditorPixelsMeasureStepMovementChanged;
		ReferenceField<double> _uIEditorPixelsMeasureStepMovement = 10.0;

		[Category( "UI Editor: Snapping" )]
		[DisplayName( "Screen Measure: Step Movement" )]
		[DefaultValue( 0.01 )]
		public Reference<double> UIEditorScreenMeasureStepMovement
		{
			get { if( _uIEditorScreenMeasureStepMovement.BeginGet() ) UIEditorScreenMeasureStepMovement = _uIEditorScreenMeasureStepMovement.Get( this ); return _uIEditorScreenMeasureStepMovement.value; }
			set { if( _uIEditorScreenMeasureStepMovement.BeginSet( ref value ) ) { try { UIEditorScreenMeasureStepMovementChanged?.Invoke( this ); } finally { _uIEditorScreenMeasureStepMovement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIEditorScreenMeasureStepMovement"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> UIEditorScreenMeasureStepMovementChanged;
		ReferenceField<double> _uIEditorScreenMeasureStepMovement = 0.01;

		public double GetUIEditorStepMovement( UIMeasure measure )
		{
			switch( measure )
			{
			case UIMeasure.Parent: return UIEditorParentMeasureStepMovement;
			case UIMeasure.Units: return UIEditorUnitsMeasureStepMovement;
			case UIMeasure.Pixels: return UIEditorPixelsMeasureStepMovement;
			case UIMeasure.Screen: return UIEditorScreenMeasureStepMovement;
			}
			return 0;
		}

		/////////////////////////////////////////

		//!!!!нельзя в в опциях проекта, еще система компонент не проинициализирована. видать в другом месте такое сохранять
		//[Category( "C# Editor" )]
		//[DisplayName( "Build Configuration" )]
		//[DefaultValue( "Release" )]
		//public Reference<string> CSharpEditorBuildConfiguration
		//{
		//	get { if( _cSharpEditorBuildConfiguration.BeginGet() ) CSharpEditorBuildConfiguration = _cSharpEditorBuildConfiguration.Get( this ); return _cSharpEditorBuildConfiguration.value; }
		//	set { if( _cSharpEditorBuildConfiguration.BeginSet( ref value ) ) { try { CSharpEditorBuildConfigurationChanged?.Invoke( this ); } finally { _cSharpEditorBuildConfiguration.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CSharpEditorBuildConfiguration"/> property value changes.</summary>
		//public event Action<Component_ProjectSettings> CSharpEditorBuildConfigurationChanged;
		//ReferenceField<string> _cSharpEditorBuildConfiguration = "Release";

		[Category( "C# Editor" )]
		[DisplayName( "Display Line Numbers" )]
		[DefaultValue( false )]
		public Reference<bool> CSharpEditorDisplayLineNumbers
		{
			get { if( _cSharpEditorDisplayLineNumbers.BeginGet() ) CSharpEditorDisplayLineNumbers = _cSharpEditorDisplayLineNumbers.Get( this ); return _cSharpEditorDisplayLineNumbers.value; }
			set { if( _cSharpEditorDisplayLineNumbers.BeginSet( ref value ) ) { try { CSharpEditorDisplayLineNumbersChanged?.Invoke( this ); } finally { _cSharpEditorDisplayLineNumbers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorDisplayLineNumbers"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorDisplayLineNumbersChanged;
		ReferenceField<bool> _cSharpEditorDisplayLineNumbers = false;

		[Category( "C# Editor" )]
		[DisplayName( "Display Info Markers" )]
		[DefaultValue( false )]
		public Reference<bool> CSharpEditorDisplayInfoMarkers
		{
			get { if( _cSharpEditorDisplayInfoMarkers.BeginGet() ) CSharpEditorDisplayInfoMarkers = _cSharpEditorDisplayInfoMarkers.Get( this ); return _cSharpEditorDisplayInfoMarkers.value; }
			set { if( _cSharpEditorDisplayInfoMarkers.BeginSet( ref value ) ) { try { CSharpEditorDisplayInfoMarkersChanged?.Invoke( this ); } finally { _cSharpEditorDisplayInfoMarkers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorDisplayInfoMarkers"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorDisplayInfoMarkersChanged;
		ReferenceField<bool> _cSharpEditorDisplayInfoMarkers = false;

		[Category( "C# Editor" )]
		[DisplayName( "Display Warning Markers" )]
		[DefaultValue( true )]
		public Reference<bool> CSharpEditorDisplayWarningMarkers
		{
			get { if( _cSharpEditorDisplayWarningMarkers.BeginGet() ) CSharpEditorDisplayWarningMarkers = _cSharpEditorDisplayWarningMarkers.Get( this ); return _cSharpEditorDisplayWarningMarkers.value; }
			set { if( _cSharpEditorDisplayWarningMarkers.BeginSet( ref value ) ) { try { CSharpEditorDisplayWarningMarkersChanged?.Invoke( this ); } finally { _cSharpEditorDisplayWarningMarkers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorDisplayWarningMarkers"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorDisplayWarningMarkersChanged;
		ReferenceField<bool> _cSharpEditorDisplayWarningMarkers = true;

		[Category( "C# Editor" )]
		[DisplayName( "Display Error Markers" )]
		[DefaultValue( true )]
		public Reference<bool> CSharpEditorDisplayErrorMarkers
		{
			get { if( _cSharpEditorDisplayErrorMarkers.BeginGet() ) CSharpEditorDisplayErrorMarkers = _cSharpEditorDisplayErrorMarkers.Get( this ); return _cSharpEditorDisplayErrorMarkers.value; }
			set { if( _cSharpEditorDisplayErrorMarkers.BeginSet( ref value ) ) { try { CSharpEditorDisplayErrorMarkersChanged?.Invoke( this ); } finally { _cSharpEditorDisplayErrorMarkers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorDisplayErrorMarkers"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorDisplayErrorMarkersChanged;
		ReferenceField<bool> _cSharpEditorDisplayErrorMarkers = true;

		[Category( "C# Editor" )]
		[DisplayName( "Display Quick Actions" )]
		[DefaultValue( true )]
		public Reference<bool> CSharpEditorDisplayQuickActions
		{
			get { if( _cSharpEditorDisplayQuickActions.BeginGet() ) CSharpEditorDisplayQuickActions = _cSharpEditorDisplayQuickActions.Get( this ); return _cSharpEditorDisplayQuickActions.value; }
			set { if( _cSharpEditorDisplayQuickActions.BeginSet( ref value ) ) { try { CSharpEditorDisplayQuickActionsChanged?.Invoke( this ); } finally { _cSharpEditorDisplayQuickActions.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorDisplayQuickActions"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorDisplayQuickActionsChanged;
		ReferenceField<bool> _cSharpEditorDisplayQuickActions = true;

		[Category( "C# Editor" )]
		[DisplayName( "Word Wrap" )]
		[DefaultValue( true )]
		public Reference<bool> CSharpEditorWordWrap
		{
			get { if( _cSharpEditorWordWrap.BeginGet() ) CSharpEditorWordWrap = _cSharpEditorWordWrap.Get( this ); return _cSharpEditorWordWrap.value; }
			set { if( _cSharpEditorWordWrap.BeginSet( ref value ) ) { try { CSharpEditorWordWrapChanged?.Invoke( this ); } finally { _cSharpEditorWordWrap.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorWordWrap"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorWordWrapChanged;
		ReferenceField<bool> _cSharpEditorWordWrap = true;

		[Category( "C# Editor" )]
		[DisplayName( "Brace Completion" )]
		[DefaultValue( false )]
		public Reference<bool> CSharpEditorBraceCompletion
		{
			get { if( _cSharpEditorBraceCompletion.BeginGet() ) CSharpEditorBraceCompletion = _cSharpEditorBraceCompletion.Get( this ); return _cSharpEditorBraceCompletion.value; }
			set { if( _cSharpEditorBraceCompletion.BeginSet( ref value ) ) { try { CSharpEditorBraceCompletionChanged?.Invoke( this ); } finally { _cSharpEditorBraceCompletion.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorBraceCompletion"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorBraceCompletionChanged;
		ReferenceField<bool> _cSharpEditorBraceCompletion = false;

		[Category( "C# Editor" )]
		[DisplayName( "Automatically Format Statement On ;" )]
		[DefaultValue( true )]
		public Reference<bool> CSharpEditorAutomaticallyFormatStatementOnSemicolon
		{
			get { if( _cSharpEditorAutomaticallyFormatStatementOnSemicolon.BeginGet() ) CSharpEditorAutomaticallyFormatStatementOnSemicolon = _cSharpEditorAutomaticallyFormatStatementOnSemicolon.Get( this ); return _cSharpEditorAutomaticallyFormatStatementOnSemicolon.value; }
			set { if( _cSharpEditorAutomaticallyFormatStatementOnSemicolon.BeginSet( ref value ) ) { try { CSharpEditorAutomaticallyFormatStatementOnSemicolonChanged?.Invoke( this ); } finally { _cSharpEditorAutomaticallyFormatStatementOnSemicolon.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorAutomaticallyFormatStatementOnSemicolon"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorAutomaticallyFormatStatementOnSemicolonChanged;
		ReferenceField<bool> _cSharpEditorAutomaticallyFormatStatementOnSemicolon = true;

		[Category( "C# Editor" )]
		[DisplayName( "Automatically Format Block On }" )]
		[DefaultValue( true )]
		public Reference<bool> CSharpEditorAutomaticallyFormatBlockOnBracket
		{
			get { if( _cSharpEditorAutomaticallyFormatBlockOnBracket.BeginGet() ) CSharpEditorAutomaticallyFormatBlockOnBracket = _cSharpEditorAutomaticallyFormatBlockOnBracket.Get( this ); return _cSharpEditorAutomaticallyFormatBlockOnBracket.value; }
			set { if( _cSharpEditorAutomaticallyFormatBlockOnBracket.BeginSet( ref value ) ) { try { CSharpEditorAutomaticallyFormatBlockOnBracketChanged?.Invoke( this ); } finally { _cSharpEditorAutomaticallyFormatBlockOnBracket.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorAutomaticallyFormatBlockOnBracket"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorAutomaticallyFormatBlockOnBracketChanged;
		ReferenceField<bool> _cSharpEditorAutomaticallyFormatBlockOnBracket = true;

		//[Category( "C# Editor" )]
		//[DisplayName( "Default Text Color for Light Theme" )]
		//[DefaultValue( "0.1 0.1 0.1" )]
		//[ColorValueNoAlpha]
		//public Reference<ColorValue> CSharpEditorDefaultTextColorLightTheme
		//{
		//	get { if( _cSharpEditorDefaultTextColorLightTheme.BeginGet() ) CSharpEditorDefaultTextColorLightTheme = _cSharpEditorDefaultTextColorLightTheme.Get( this ); return _cSharpEditorDefaultTextColorLightTheme.value; }
		//	set { if( _cSharpEditorDefaultTextColorLightTheme.BeginSet( ref value ) ) { try { CSharpEditorDefaultTextColorLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorDefaultTextColorLightTheme.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CSharpEditorDefaultTextColorLightTheme"/> property value changes.</summary>
		//public event Action<Component_ProjectSettings> CSharpEditorDefaultTextColorLightThemeChanged;
		//ReferenceField<ColorValue> _cSharpEditorDefaultTextColorLightTheme = new ColorValue( 0.1, 0.1, 0.1 );

		[Category( "C# Editor" )]
		[DisplayName( "Font" )]
		[DefaultValue( "Consolas" )]
		public Reference<string> CSharpEditorFont
		{
			get { if( _cSharpEditorFont.BeginGet() ) CSharpEditorFont = _cSharpEditorFont.Get( this ); return _cSharpEditorFont.value; }
			set { if( _cSharpEditorFont.BeginSet( ref value ) ) { try { CSharpEditorFontChanged?.Invoke( this ); } finally { _cSharpEditorFont.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorFont"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorFontChanged;
		ReferenceField<string> _cSharpEditorFont = "Consolas";

		[Category( "C# Editor" )]
		[DisplayName( "Font Size" )]
		[DefaultValue( 13.0 )]
		[Range( 6, 40 )]
		public Reference<double> CSharpEditorFontSize
		{
			get { if( _cSharpEditorFontSize.BeginGet() ) CSharpEditorFontSize = _cSharpEditorFontSize.Get( this ); return _cSharpEditorFontSize.value; }
			set { if( _cSharpEditorFontSize.BeginSet( ref value ) ) { try { CSharpEditorFontSizeChanged?.Invoke( this ); } finally { _cSharpEditorFontSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorFontSize"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorFontSizeChanged;
		ReferenceField<double> _cSharpEditorFontSize = 13.0;

		[Category( "C# Editor" )]
		[DisplayName( "Background Color for Light Theme" )]
		[DefaultValue( "0.98 0.98 0.98" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorBackgroundColorLightTheme
		{
			get { if( _cSharpEditorBackgroundColorLightTheme.BeginGet() ) CSharpEditorBackgroundColorLightTheme = _cSharpEditorBackgroundColorLightTheme.Get( this ); return _cSharpEditorBackgroundColorLightTheme.value; }
			set { if( _cSharpEditorBackgroundColorLightTheme.BeginSet( ref value ) ) { try { CSharpEditorBackgroundColorLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorBackgroundColorLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorBackgroundColorLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorBackgroundColorLightThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorBackgroundColorLightTheme = new ColorValue( 0.98, 0.98, 0.98 );

		[Category( "C# Editor" )]
		[DisplayName( "Background Color for Dark Theme" )]
		[DefaultValue( "0.1 0.1 0.1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorBackgroundColorDarkTheme
		{
			get { if( _cSharpEditorBackgroundColorDarkTheme.BeginGet() ) CSharpEditorBackgroundColorDarkTheme = _cSharpEditorBackgroundColorDarkTheme.Get( this ); return _cSharpEditorBackgroundColorDarkTheme.value; }
			set { if( _cSharpEditorBackgroundColorDarkTheme.BeginSet( ref value ) ) { try { CSharpEditorBackgroundColorDarkThemeChanged?.Invoke( this ); } finally { _cSharpEditorBackgroundColorDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorBackgroundColorDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorBackgroundColorDarkThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorBackgroundColorDarkTheme = new ColorValue( 0.1, 0.1, 0.1 );

		[Category( "C# Editor" )]
		[DisplayName( "Selection Background for Light Theme" )]
		[DefaultValue( "0.4 0.6 1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorSelectionBackgroundLightTheme
		{
			get { if( _cSharpEditorSelectionBackgroundLightTheme.BeginGet() ) CSharpEditorSelectionBackgroundLightTheme = _cSharpEditorSelectionBackgroundLightTheme.Get( this ); return _cSharpEditorSelectionBackgroundLightTheme.value; }
			set { if( _cSharpEditorSelectionBackgroundLightTheme.BeginSet( ref value ) ) { try { CSharpEditorSelectionBackgroundLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorSelectionBackgroundLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorSelectionBackgroundLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorSelectionBackgroundLightThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorSelectionBackgroundLightTheme = new ColorValue( 0.4, 0.6, 1 );

		[Category( "C# Editor" )]
		[DisplayName( "Selection Foreground for Light Theme" )]
		[DefaultValue( "0.1 0.1 0.1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorSelectionForegroundLightTheme
		{
			get { if( _cSharpEditorSelectionForegroundLightTheme.BeginGet() ) CSharpEditorSelectionForegroundLightTheme = _cSharpEditorSelectionForegroundLightTheme.Get( this ); return _cSharpEditorSelectionForegroundLightTheme.value; }
			set { if( _cSharpEditorSelectionForegroundLightTheme.BeginSet( ref value ) ) { try { CSharpEditorSelectionForegroundLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorSelectionForegroundLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorSelectionForegroundLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorSelectionForegroundLightThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorSelectionForegroundLightTheme = new ColorValue( 0.1, 0.1, 0.1 );

		[Category( "C# Editor" )]
		[DisplayName( "Selection Background for Dark Theme" )]
		[DefaultValue( "0.25 0.37 0.62" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorSelectionBackgroundDarkTheme
		{
			get { if( _cSharpEditorSelectionBackgroundDarkTheme.BeginGet() ) CSharpEditorSelectionBackgroundDarkTheme = _cSharpEditorSelectionBackgroundDarkTheme.Get( this ); return _cSharpEditorSelectionBackgroundDarkTheme.value; }
			set { if( _cSharpEditorSelectionBackgroundDarkTheme.BeginSet( ref value ) ) { try { CSharpEditorSelectionBackgroundDarkThemeChanged?.Invoke( this ); } finally { _cSharpEditorSelectionBackgroundDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorSelectionBackgroundDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorSelectionBackgroundDarkThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorSelectionBackgroundDarkTheme = new ColorValue( 0.25, 0.37, 0.62 );

		[Category( "C# Editor" )]
		[DisplayName( "Selection Foreground for Dark Theme" )]
		[DefaultValue( "0.9 0.9 0.9" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorSelectionForegroundDarkTheme
		{
			get { if( _cSharpEditorSelectionForegroundDarkTheme.BeginGet() ) CSharpEditorSelectionForegroundDarkTheme = _cSharpEditorSelectionForegroundDarkTheme.Get( this ); return _cSharpEditorSelectionForegroundDarkTheme.value; }
			set { if( _cSharpEditorSelectionForegroundDarkTheme.BeginSet( ref value ) ) { try { CSharpEditorSelectionForegroundDarkThemeChanged?.Invoke( this ); } finally { _cSharpEditorSelectionForegroundDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorSelectionForegroundDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorSelectionForegroundDarkThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorSelectionForegroundDarkTheme = new ColorValue( 0.9, 0.9, 0.9 );

		[Category( "C# Editor" )]
		[DisplayName( "Search Background for Light Theme" )]
		[DefaultValue( "1 0 0" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorSearchBackgroundLightTheme
		{
			get { if( _cSharpEditorSearchBackgroundLightTheme.BeginGet() ) CSharpEditorSearchBackgroundLightTheme = _cSharpEditorSearchBackgroundLightTheme.Get( this ); return _cSharpEditorSearchBackgroundLightTheme.value; }
			set { if( _cSharpEditorSearchBackgroundLightTheme.BeginSet( ref value ) ) { try { CSharpEditorSearchBackgroundLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorSearchBackgroundLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorSearchBackgroundLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorSearchBackgroundLightThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorSearchBackgroundLightTheme = new ColorValue( 1, 0, 0 );

		[Category( "C# Editor" )]
		[DisplayName( "Search Background for Dark Theme" )]
		[DefaultValue( "0.8 0.16 0.16" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorSearchBackgroundDarkTheme
		{
			get { if( _cSharpEditorSearchBackgroundDarkTheme.BeginGet() ) CSharpEditorSearchBackgroundDarkTheme = _cSharpEditorSearchBackgroundDarkTheme.Get( this ); return _cSharpEditorSearchBackgroundDarkTheme.value; }
			set { if( _cSharpEditorSearchBackgroundDarkTheme.BeginSet( ref value ) ) { try { CSharpEditorSearchBackgroundDarkThemeChanged?.Invoke( this ); } finally { _cSharpEditorSearchBackgroundDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorSearchBackgroundDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> CSharpEditorSearchBackgroundDarkThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorSearchBackgroundDarkTheme = new ColorValue( 0.8, 0.16, 0.16 );

		//[Category( "C# Editor" )]
		//[DisplayName( "Default Text Color for Dark Theme" )]
		//[DefaultValue( "0.9 0.9 0.9" )]
		//[ColorValueNoAlpha]
		//public Reference<ColorValue> CSharpEditorDefaultTextColorDarkTheme
		//{
		//	get { if( _cSharpEditorDefaultTextColorDarkTheme.BeginGet() ) CSharpEditorDefaultTextColorDarkTheme = _cSharpEditorDefaultTextColorDarkTheme.Get( this ); return _cSharpEditorDefaultTextColorDarkTheme.value; }
		//	set { if( _cSharpEditorDefaultTextColorDarkTheme.BeginSet( ref value ) ) { try { CSharpEditorDefaultTextColorDarkThemeChanged?.Invoke( this ); } finally { _cSharpEditorDefaultTextColorDarkTheme.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CSharpEditorDefaultTextColorDarkTheme"/> property value changes.</summary>
		//public event Action<Component_ProjectSettings> CSharpEditorDefaultTextColorDarkThemeChanged;
		//ReferenceField<ColorValue> _cSharpEditorDefaultTextColorDarkTheme = new ColorValue( 0.9, 0.9, 0.9 );

		[Category( "C# Editor" )]
		[DisplayName( "Highlighting Scheme" )]
		public string CSharpEditorHighlightingScheme
		{
			get { return @"Settings files are located in the 'Base\Tools\Highlighting' folder."; }
		}

		//[Category( "C# Editor" )]
		//[DefaultValue( @"Base\Tools\Highlighting\CSharpLight.xshd" )]
		//[DisplayName( "Highlighting Scheme for Light Theme" )]
		//public Reference<string> CSharpEditorHighlightingSchemeLightTheme
		//{
		//	get { if( _cSharpEditorHighlightingSchemeLightTheme.BeginGet() ) CSharpEditorHighlightingSchemeLightTheme = _cSharpEditorHighlightingSchemeLightTheme.Get( this ); return _cSharpEditorHighlightingSchemeLightTheme.value; }
		//	set { if( _cSharpEditorHighlightingSchemeLightTheme.BeginSet( ref value ) ) { try { CSharpEditorHighlightingSchemeLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorHighlightingSchemeLightTheme.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CSharpEditorHighlightingSchemeLightTheme"/> property value changes.</summary>
		//public event Action<Component_ProjectSettings> CSharpEditorHighlightingSchemeLightThemeChanged;
		//ReferenceField<string> _cSharpEditorHighlightingSchemeLightTheme = @"Base\Tools\Highlighting\CSharpLight.xshd";

		//[Category( "C# Editor" )]
		//[DefaultValue( @"Assets\Base\Tools\Highlighting\CSharpLight.xshd" )]
		//public Reference<ReferenceValueType_Resource> CSharpEditorHighlightingLightTheme
		//{
		//	get { if( _cSharpEditorHighlightingLightTheme.BeginGet() ) CSharpEditorHighlightingLightTheme = _cSharpEditorHighlightingLightTheme.Get( this ); return _cSharpEditorHighlightingLightTheme.value; }
		//	set { if( _cSharpEditorHighlightingLightTheme.BeginSet( ref value ) ) { try { CSharpEditorHighlightingLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorHighlightingLightTheme.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CSharpEditorHighlightingLightTheme"/> property value changes.</summary>
		//public event Action<Component_ProjectSettings> CSharpEditorHighlightingLightThemeChanged;
		//ReferenceField<ReferenceValueType_Resource> _cSharpEditorHighlightingLightTheme = new ReferenceValueType_Resource( @"Assets\Base\Tools\Highlighting\CSharpLight.xshd" );

		//ReferenceField<ReferenceValueType_Resource> _cSharpEditorHighlightingLightTheme = new ReferenceValueType_Resource( @"Assets\Base\Tools\Highlighting\CSharpLight.xshd" );

		/////////////////////////////////////////

		[Category( "Shader Editor" )]
		[DisplayName( "Display Line Numbers" )]
		[DefaultValue( false )]
		public Reference<bool> ShaderEditorDisplayLineNumbers
		{
			get { if( _shaderEditorDisplayLineNumbers.BeginGet() ) ShaderEditorDisplayLineNumbers = _shaderEditorDisplayLineNumbers.Get( this ); return _shaderEditorDisplayLineNumbers.value; }
			set { if( _shaderEditorDisplayLineNumbers.BeginSet( ref value ) ) { try { ShaderEditorDisplayLineNumbersChanged?.Invoke( this ); } finally { _shaderEditorDisplayLineNumbers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorDisplayLineNumbers"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ShaderEditorDisplayLineNumbersChanged;
		ReferenceField<bool> _shaderEditorDisplayLineNumbers = false;

		[Category( "Shader Editor" )]
		[DisplayName( "Word Wrap" )]
		[DefaultValue( true )]
		public Reference<bool> ShaderEditorWordWrap
		{
			get { if( _shaderEditorWordWrap.BeginGet() ) ShaderEditorWordWrap = _shaderEditorWordWrap.Get( this ); return _shaderEditorWordWrap.value; }
			set { if( _shaderEditorWordWrap.BeginSet( ref value ) ) { try { ShaderEditorWordWrapChanged?.Invoke( this ); } finally { _shaderEditorWordWrap.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorWordWrap"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ShaderEditorWordWrapChanged;
		ReferenceField<bool> _shaderEditorWordWrap = true;

		[Category( "Shader Editor" )]
		[DisplayName( "Background Color for Light Theme" )]
		[DefaultValue( "0.98 0.98 0.98" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> ShaderEditorBackgroundColorLightTheme
		{
			get { if( _shaderEditorBackgroundColorLightTheme.BeginGet() ) ShaderEditorBackgroundColorLightTheme = _shaderEditorBackgroundColorLightTheme.Get( this ); return _shaderEditorBackgroundColorLightTheme.value; }
			set { if( _shaderEditorBackgroundColorLightTheme.BeginSet( ref value ) ) { try { ShaderEditorBackgroundColorLightThemeChanged?.Invoke( this ); } finally { _shaderEditorBackgroundColorLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorBackgroundColorLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ShaderEditorBackgroundColorLightThemeChanged;
		ReferenceField<ColorValue> _shaderEditorBackgroundColorLightTheme = new ColorValue( 0.98, 0.98, 0.98 );

		//[Category( "Shader Editor" )]
		//[DisplayName( "Default Text Color for Light Theme" )]
		//[DefaultValue( "0.1 0.1 0.1" )]
		//[ColorValueNoAlpha]
		//public Reference<ColorValue> ShaderEditorDefaultTextColorLightTheme
		//{
		//	get { if( _shaderEditorDefaultTextColorLightTheme.BeginGet() ) ShaderEditorDefaultTextColorLightTheme = _shaderEditorDefaultTextColorLightTheme.Get( this ); return _shaderEditorDefaultTextColorLightTheme.value; }
		//	set { if( _shaderEditorDefaultTextColorLightTheme.BeginSet( ref value ) ) { try { ShaderEditorDefaultTextColorLightThemeChanged?.Invoke( this ); } finally { _shaderEditorDefaultTextColorLightTheme.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ShaderEditorDefaultTextColorLightTheme"/> property value changes.</summary>
		//public event Action<Component_ProjectSettings> ShaderEditorDefaultTextColorLightThemeChanged;
		//ReferenceField<ColorValue> _shaderEditorDefaultTextColorLightTheme = new ColorValue( 0.1, 0.1, 0.1 );

		[Category( "Shader Editor" )]
		[DisplayName( "Font" )]
		[DefaultValue( "Consolas" )]
		public Reference<string> ShaderEditorFont
		{
			get { if( _shaderEditorFont.BeginGet() ) ShaderEditorFont = _shaderEditorFont.Get( this ); return _shaderEditorFont.value; }
			set { if( _shaderEditorFont.BeginSet( ref value ) ) { try { ShaderEditorFontChanged?.Invoke( this ); } finally { _shaderEditorFont.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorFont"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ShaderEditorFontChanged;
		ReferenceField<string> _shaderEditorFont = "Consolas";

		[Category( "Shader Editor" )]
		[DisplayName( "Font Size" )]
		[DefaultValue( 13.0 )]
		[Range( 6, 40 )]
		public Reference<double> ShaderEditorFontSize
		{
			get { if( _shaderEditorFontSize.BeginGet() ) ShaderEditorFontSize = _shaderEditorFontSize.Get( this ); return _shaderEditorFontSize.value; }
			set { if( _shaderEditorFontSize.BeginSet( ref value ) ) { try { ShaderEditorFontSizeChanged?.Invoke( this ); } finally { _shaderEditorFontSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorFontSize"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ShaderEditorFontSizeChanged;
		ReferenceField<double> _shaderEditorFontSize = 13.0;

		[Category( "Shader Editor" )]
		[DisplayName( "Background Color for Dark Theme" )]
		[DefaultValue( "0.1 0.1 0.1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> ShaderEditorBackgroundColorDarkTheme
		{
			get { if( _shaderEditorBackgroundColorDarkTheme.BeginGet() ) ShaderEditorBackgroundColorDarkTheme = _shaderEditorBackgroundColorDarkTheme.Get( this ); return _shaderEditorBackgroundColorDarkTheme.value; }
			set { if( _shaderEditorBackgroundColorDarkTheme.BeginSet( ref value ) ) { try { ShaderEditorBackgroundColorDarkThemeChanged?.Invoke( this ); } finally { _shaderEditorBackgroundColorDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorBackgroundColorDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ShaderEditorBackgroundColorDarkThemeChanged;
		ReferenceField<ColorValue> _shaderEditorBackgroundColorDarkTheme = new ColorValue( 0.1, 0.1, 0.1 );

		[Category( "Shader Editor" )]
		[DisplayName( "Selection Background for Light Theme" )]
		[DefaultValue( "0.4 0.6 1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> ShaderEditorSelectionBackgroundLightTheme
		{
			get { if( _shaderEditorSelectionBackgroundLightTheme.BeginGet() ) ShaderEditorSelectionBackgroundLightTheme = _shaderEditorSelectionBackgroundLightTheme.Get( this ); return _shaderEditorSelectionBackgroundLightTheme.value; }
			set { if( _shaderEditorSelectionBackgroundLightTheme.BeginSet( ref value ) ) { try { ShaderEditorSelectionBackgroundLightThemeChanged?.Invoke( this ); } finally { _shaderEditorSelectionBackgroundLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorSelectionBackgroundLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ShaderEditorSelectionBackgroundLightThemeChanged;
		ReferenceField<ColorValue> _shaderEditorSelectionBackgroundLightTheme = new ColorValue( 0.4, 0.6, 1 );

		[Category( "Shader Editor" )]
		[DisplayName( "Selection Foreground for Light Theme" )]
		[DefaultValue( "0.1 0.1 0.1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> ShaderEditorSelectionForegroundLightTheme
		{
			get { if( _shaderEditorSelectionForegroundLightTheme.BeginGet() ) ShaderEditorSelectionForegroundLightTheme = _shaderEditorSelectionForegroundLightTheme.Get( this ); return _shaderEditorSelectionForegroundLightTheme.value; }
			set { if( _shaderEditorSelectionForegroundLightTheme.BeginSet( ref value ) ) { try { ShaderEditorSelectionForegroundLightThemeChanged?.Invoke( this ); } finally { _shaderEditorSelectionForegroundLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorSelectionForegroundLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ShaderEditorSelectionForegroundLightThemeChanged;
		ReferenceField<ColorValue> _shaderEditorSelectionForegroundLightTheme = new ColorValue( 0.1, 0.1, 0.1 );

		[Category( "Shader Editor" )]
		[DisplayName( "Selection Background for Dark Theme" )]
		[DefaultValue( "0.25 0.37 0.62" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> ShaderEditorSelectionBackgroundDarkTheme
		{
			get { if( _shaderEditorSelectionBackgroundDarkTheme.BeginGet() ) ShaderEditorSelectionBackgroundDarkTheme = _shaderEditorSelectionBackgroundDarkTheme.Get( this ); return _shaderEditorSelectionBackgroundDarkTheme.value; }
			set { if( _shaderEditorSelectionBackgroundDarkTheme.BeginSet( ref value ) ) { try { ShaderEditorSelectionBackgroundDarkThemeChanged?.Invoke( this ); } finally { _shaderEditorSelectionBackgroundDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorSelectionBackgroundDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ShaderEditorSelectionBackgroundDarkThemeChanged;
		ReferenceField<ColorValue> _shaderEditorSelectionBackgroundDarkTheme = new ColorValue( 0.25, 0.37, 0.62 );

		[Category( "Shader Editor" )]
		[DisplayName( "Selection Foreground for Dark Theme" )]
		[DefaultValue( "0.9 0.9 0.9" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> ShaderEditorSelectionForegroundDarkTheme
		{
			get { if( _shaderEditorSelectionForegroundDarkTheme.BeginGet() ) ShaderEditorSelectionForegroundDarkTheme = _shaderEditorSelectionForegroundDarkTheme.Get( this ); return _shaderEditorSelectionForegroundDarkTheme.value; }
			set { if( _shaderEditorSelectionForegroundDarkTheme.BeginSet( ref value ) ) { try { ShaderEditorSelectionForegroundDarkThemeChanged?.Invoke( this ); } finally { _shaderEditorSelectionForegroundDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorSelectionForegroundDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ShaderEditorSelectionForegroundDarkThemeChanged;
		ReferenceField<ColorValue> _shaderEditorSelectionForegroundDarkTheme = new ColorValue( 0.9, 0.9, 0.9 );

		[Category( "Shader Editor" )]
		[DisplayName( "Search Background for Light Theme" )]
		[DefaultValue( "1 0 0" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> ShaderEditorSearchBackgroundLightTheme
		{
			get { if( _shaderEditorSearchBackgroundLightTheme.BeginGet() ) ShaderEditorSearchBackgroundLightTheme = _shaderEditorSearchBackgroundLightTheme.Get( this ); return _shaderEditorSearchBackgroundLightTheme.value; }
			set { if( _shaderEditorSearchBackgroundLightTheme.BeginSet( ref value ) ) { try { ShaderEditorSearchBackgroundLightThemeChanged?.Invoke( this ); } finally { _shaderEditorSearchBackgroundLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorSearchBackgroundLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ShaderEditorSearchBackgroundLightThemeChanged;
		ReferenceField<ColorValue> _shaderEditorSearchBackgroundLightTheme = new ColorValue( 1, 0, 0 );

		[Category( "Shader Editor" )]
		[DisplayName( "Search Background for Dark Theme" )]
		[DefaultValue( "0.8 0.16 0.16" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> ShaderEditorSearchBackgroundDarkTheme
		{
			get { if( _shaderEditorSearchBackgroundDarkTheme.BeginGet() ) ShaderEditorSearchBackgroundDarkTheme = _shaderEditorSearchBackgroundDarkTheme.Get( this ); return _shaderEditorSearchBackgroundDarkTheme.value; }
			set { if( _shaderEditorSearchBackgroundDarkTheme.BeginSet( ref value ) ) { try { ShaderEditorSearchBackgroundDarkThemeChanged?.Invoke( this ); } finally { _shaderEditorSearchBackgroundDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorSearchBackgroundDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> ShaderEditorSearchBackgroundDarkThemeChanged;
		ReferenceField<ColorValue> _shaderEditorSearchBackgroundDarkTheme = new ColorValue( 0.8, 0.16, 0.16 );

		//[Category( "Shader Editor" )]
		//[DisplayName( "Default Text Color for Dark Theme" )]
		//[DefaultValue( "0.9 0.9 0.9" )]
		//[ColorValueNoAlpha]
		//public Reference<ColorValue> ShaderEditorDefaultTextColorDarkTheme
		//{
		//	get { if( _shaderEditorDefaultTextColorDarkTheme.BeginGet() ) ShaderEditorDefaultTextColorDarkTheme = _shaderEditorDefaultTextColorDarkTheme.Get( this ); return _shaderEditorDefaultTextColorDarkTheme.value; }
		//	set { if( _shaderEditorDefaultTextColorDarkTheme.BeginSet( ref value ) ) { try { ShaderEditorDefaultTextColorDarkThemeChanged?.Invoke( this ); } finally { _shaderEditorDefaultTextColorDarkTheme.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ShaderEditorDefaultTextColorDarkTheme"/> property value changes.</summary>
		//public event Action<Component_ProjectSettings> ShaderEditorDefaultTextColorDarkThemeChanged;
		//ReferenceField<ColorValue> _shaderEditorDefaultTextColorDarkTheme = new ColorValue( 0.9, 0.9, 0.9 );

		[Category( "Shader Editor" )]
		[DisplayName( "Highlighting Scheme" )]
		public string ShaderEditorHighlightingScheme
		{
			get { return @"Settings files are located in the 'Base\Tools\Highlighting' folder."; }
		}

		/////////////////////////////////////////

		[Category( "Text Editor" )]
		[DisplayName( "Display Line Numbers" )]
		[DefaultValue( false )]
		public Reference<bool> TextEditorDisplayLineNumbers
		{
			get { if( _textEditorDisplayLineNumbers.BeginGet() ) TextEditorDisplayLineNumbers = _textEditorDisplayLineNumbers.Get( this ); return _textEditorDisplayLineNumbers.value; }
			set { if( _textEditorDisplayLineNumbers.BeginSet( ref value ) ) { try { TextEditorDisplayLineNumbersChanged?.Invoke( this ); } finally { _textEditorDisplayLineNumbers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorDisplayLineNumbers"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorDisplayLineNumbersChanged;
		ReferenceField<bool> _textEditorDisplayLineNumbers = false;

		[Category( "Text Editor" )]
		[DisplayName( "Word Wrap" )]
		[DefaultValue( true )]
		public Reference<bool> TextEditorWordWrap
		{
			get { if( _textEditorWordWrap.BeginGet() ) TextEditorWordWrap = _textEditorWordWrap.Get( this ); return _textEditorWordWrap.value; }
			set { if( _textEditorWordWrap.BeginSet( ref value ) ) { try { TextEditorWordWrapChanged?.Invoke( this ); } finally { _textEditorWordWrap.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorWordWrap"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorWordWrapChanged;
		ReferenceField<bool> _textEditorWordWrap = true;

		[Category( "Text Editor" )]
		[DisplayName( "Font" )]
		[DefaultValue( "Consolas" )]
		public Reference<string> TextEditorFont
		{
			get { if( _textEditorFont.BeginGet() ) TextEditorFont = _textEditorFont.Get( this ); return _textEditorFont.value; }
			set { if( _textEditorFont.BeginSet( ref value ) ) { try { TextEditorFontChanged?.Invoke( this ); } finally { _textEditorFont.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorFont"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorFontChanged;
		ReferenceField<string> _textEditorFont = "Consolas";

		[Category( "Text Editor" )]
		[DisplayName( "Font Size" )]
		[DefaultValue( 13.0 )]
		[Range( 6, 40 )]
		public Reference<double> TextEditorFontSize
		{
			get { if( _textEditorFontSize.BeginGet() ) TextEditorFontSize = _textEditorFontSize.Get( this ); return _textEditorFontSize.value; }
			set { if( _textEditorFontSize.BeginSet( ref value ) ) { try { TextEditorFontSizeChanged?.Invoke( this ); } finally { _textEditorFontSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorFontSize"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorFontSizeChanged;
		ReferenceField<double> _textEditorFontSize = 13.0;

		[Category( "Text Editor" )]
		[DisplayName( "Background Color for Light Theme" )]
		[DefaultValue( "0.98 0.98 0.98" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> TextEditorBackgroundColorLightTheme
		{
			get { if( _textEditorBackgroundColorLightTheme.BeginGet() ) TextEditorBackgroundColorLightTheme = _textEditorBackgroundColorLightTheme.Get( this ); return _textEditorBackgroundColorLightTheme.value; }
			set { if( _textEditorBackgroundColorLightTheme.BeginSet( ref value ) ) { try { TextEditorBackgroundColorLightThemeChanged?.Invoke( this ); } finally { _textEditorBackgroundColorLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorBackgroundColorLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorBackgroundColorLightThemeChanged;
		ReferenceField<ColorValue> _textEditorBackgroundColorLightTheme = new ColorValue( 0.98, 0.98, 0.98 );

		[Category( "Text Editor" )]
		[DisplayName( "Foreground Color for Light Theme" )]
		[DefaultValue( "0.1 0.1 0.1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> TextEditorForegroundColorLightTheme
		{
			get { if( _textEditorForegroundColorLightTheme.BeginGet() ) TextEditorForegroundColorLightTheme = _textEditorForegroundColorLightTheme.Get( this ); return _textEditorForegroundColorLightTheme.value; }
			set { if( _textEditorForegroundColorLightTheme.BeginSet( ref value ) ) { try { TextEditorForegroundColorLightThemeChanged?.Invoke( this ); } finally { _textEditorForegroundColorLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorForegroundColorLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorForegroundColorLightThemeChanged;
		ReferenceField<ColorValue> _textEditorForegroundColorLightTheme = new ColorValue( 0.1, 0.1, 0.1 );

		[Category( "Text Editor" )]
		[DisplayName( "Background Color for Dark Theme" )]
		[DefaultValue( "0.1 0.1 0.1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> TextEditorBackgroundColorDarkTheme
		{
			get { if( _textEditorBackgroundColorDarkTheme.BeginGet() ) TextEditorBackgroundColorDarkTheme = _textEditorBackgroundColorDarkTheme.Get( this ); return _textEditorBackgroundColorDarkTheme.value; }
			set { if( _textEditorBackgroundColorDarkTheme.BeginSet( ref value ) ) { try { TextEditorBackgroundColorDarkThemeChanged?.Invoke( this ); } finally { _textEditorBackgroundColorDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorBackgroundColorDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorBackgroundColorDarkThemeChanged;
		ReferenceField<ColorValue> _textEditorBackgroundColorDarkTheme = new ColorValue( 0.1, 0.1, 0.1 );

		[Category( "Text Editor" )]
		[DisplayName( "Foreground Color for Dark Theme" )]
		[DefaultValue( "0.9 0.9 0.9" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> TextEditorForegroundColorDarkTheme
		{
			get { if( _textEditorForegroundColorDarkTheme.BeginGet() ) TextEditorForegroundColorDarkTheme = _textEditorForegroundColorDarkTheme.Get( this ); return _textEditorForegroundColorDarkTheme.value; }
			set { if( _textEditorForegroundColorDarkTheme.BeginSet( ref value ) ) { try { TextEditorForegroundColorDarkThemeChanged?.Invoke( this ); } finally { _textEditorForegroundColorDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorForegroundColorDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorForegroundColorDarkThemeChanged;
		ReferenceField<ColorValue> _textEditorForegroundColorDarkTheme = new ColorValue( 0.9, 0.9, 0.9 );

		[Category( "Text Editor" )]
		[DisplayName( "Selection Background for Light Theme" )]
		[DefaultValue( "0.4 0.6 1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> TextEditorSelectionBackgroundLightTheme
		{
			get { if( _textEditorSelectionBackgroundLightTheme.BeginGet() ) TextEditorSelectionBackgroundLightTheme = _textEditorSelectionBackgroundLightTheme.Get( this ); return _textEditorSelectionBackgroundLightTheme.value; }
			set { if( _textEditorSelectionBackgroundLightTheme.BeginSet( ref value ) ) { try { TextEditorSelectionBackgroundLightThemeChanged?.Invoke( this ); } finally { _textEditorSelectionBackgroundLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorSelectionBackgroundLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorSelectionBackgroundLightThemeChanged;
		ReferenceField<ColorValue> _textEditorSelectionBackgroundLightTheme = new ColorValue( 0.4, 0.6, 1 );

		[Category( "Text Editor" )]
		[DisplayName( "Selection Foreground for Light Theme" )]
		[DefaultValue( "0.1 0.1 0.1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> TextEditorSelectionForegroundLightTheme
		{
			get { if( _textEditorSelectionForegroundLightTheme.BeginGet() ) TextEditorSelectionForegroundLightTheme = _textEditorSelectionForegroundLightTheme.Get( this ); return _textEditorSelectionForegroundLightTheme.value; }
			set { if( _textEditorSelectionForegroundLightTheme.BeginSet( ref value ) ) { try { TextEditorSelectionForegroundLightThemeChanged?.Invoke( this ); } finally { _textEditorSelectionForegroundLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorSelectionForegroundLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorSelectionForegroundLightThemeChanged;
		ReferenceField<ColorValue> _textEditorSelectionForegroundLightTheme = new ColorValue( 0.1, 0.1, 0.1 );

		[Category( "Text Editor" )]
		[DisplayName( "Selection Background for Dark Theme" )]
		[DefaultValue( "0.25 0.37 0.62" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> TextEditorSelectionBackgroundDarkTheme
		{
			get { if( _textEditorSelectionBackgroundDarkTheme.BeginGet() ) TextEditorSelectionBackgroundDarkTheme = _textEditorSelectionBackgroundDarkTheme.Get( this ); return _textEditorSelectionBackgroundDarkTheme.value; }
			set { if( _textEditorSelectionBackgroundDarkTheme.BeginSet( ref value ) ) { try { TextEditorSelectionBackgroundDarkThemeChanged?.Invoke( this ); } finally { _textEditorSelectionBackgroundDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorSelectionBackgroundDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorSelectionBackgroundDarkThemeChanged;
		ReferenceField<ColorValue> _textEditorSelectionBackgroundDarkTheme = new ColorValue( 0.25, 0.37, 0.62 );

		[Category( "Text Editor" )]
		[DisplayName( "Selection Foreground for Dark Theme" )]
		[DefaultValue( "0.9 0.9 0.9" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> TextEditorSelectionForegroundDarkTheme
		{
			get { if( _textEditorSelectionForegroundDarkTheme.BeginGet() ) TextEditorSelectionForegroundDarkTheme = _textEditorSelectionForegroundDarkTheme.Get( this ); return _textEditorSelectionForegroundDarkTheme.value; }
			set { if( _textEditorSelectionForegroundDarkTheme.BeginSet( ref value ) ) { try { TextEditorSelectionForegroundDarkThemeChanged?.Invoke( this ); } finally { _textEditorSelectionForegroundDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorSelectionForegroundDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorSelectionForegroundDarkThemeChanged;
		ReferenceField<ColorValue> _textEditorSelectionForegroundDarkTheme = new ColorValue( 0.9, 0.9, 0.9 );

		[Category( "Text Editor" )]
		[DisplayName( "Search Background for Light Theme" )]
		[DefaultValue( "1 0 0" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> TextEditorSearchBackgroundLightTheme
		{
			get { if( _textEditorSearchBackgroundLightTheme.BeginGet() ) TextEditorSearchBackgroundLightTheme = _textEditorSearchBackgroundLightTheme.Get( this ); return _textEditorSearchBackgroundLightTheme.value; }
			set { if( _textEditorSearchBackgroundLightTheme.BeginSet( ref value ) ) { try { TextEditorSearchBackgroundLightThemeChanged?.Invoke( this ); } finally { _textEditorSearchBackgroundLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorSearchBackgroundLightTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorSearchBackgroundLightThemeChanged;
		ReferenceField<ColorValue> _textEditorSearchBackgroundLightTheme = new ColorValue( 1, 0, 0 );

		[Category( "Text Editor" )]
		[DisplayName( "Search Background for Dark Theme" )]
		[DefaultValue( "0.8 0.16 0.16" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> TextEditorSearchBackgroundDarkTheme
		{
			get { if( _textEditorSearchBackgroundDarkTheme.BeginGet() ) TextEditorSearchBackgroundDarkTheme = _textEditorSearchBackgroundDarkTheme.Get( this ); return _textEditorSearchBackgroundDarkTheme.value; }
			set { if( _textEditorSearchBackgroundDarkTheme.BeginSet( ref value ) ) { try { TextEditorSearchBackgroundDarkThemeChanged?.Invoke( this ); } finally { _textEditorSearchBackgroundDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorSearchBackgroundDarkTheme"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> TextEditorSearchBackgroundDarkThemeChanged;
		ReferenceField<ColorValue> _textEditorSearchBackgroundDarkTheme = new ColorValue( 0.8, 0.16, 0.16 );

		//[Category( "Text Editor" )]
		//[DisplayName( "Highlighting Scheme" )]
		//public string TextEditorHighlightingScheme
		//{
		//	get { return @"Settings files are located in the 'Base\Tools\Highlighting' folder."; }
		//}

		/////////////////////////////////////////

		public sealed class RibbonAndToolbarActionsClass
		{
			[Serialize]
			public bool UseDefaultSettings { get; set; } = true;

			[Serialize]
			[Cloneable( CloneType.Deep )]
			public List<TabItem> RibbonTabs
			{
				get
				{
					//init default configuration
					if( UseDefaultSettings && ribbonTabs.Count == 0 )
					{
						foreach( var tab in EditorRibbonDefaultConfiguration.Tabs )
						{
							var tabItem = new TabItem();
							tabItem.Name = tab.Name;

							foreach( var group in tab.Groups )
							{
								var groupItem = new GroupItem();
								groupItem.Name = group.Name;

								foreach( var child in group.Children )
								{
									//sub group
									var subGroup = child as EditorRibbonDefaultConfiguration.Group;
									if( subGroup != null )
									{
										var actionItem = new ActionItem();
										actionItem.Type = ActionItem.TypeEnum.SubGroupOfActions;
										actionItem.Name = subGroup.Name;

										groupItem.Actions.Add( actionItem );
									}

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
										var actionItem = new ActionItem();
										actionItem.Name = action.Name;

										groupItem.Actions.Add( actionItem );
									}
								}

								tabItem.Groups.Add( groupItem );
							}

							ribbonTabs.Add( tabItem );
						}
					}
					return ribbonTabs;
				}
				set { ribbonTabs = value; }
			}
			List<TabItem> ribbonTabs = new List<TabItem>();

			[Serialize]
			[Cloneable( CloneType.Deep )]
			public List<ActionItem> ToolbarActions
			{
				get
				{
					//init default configuration
					if( UseDefaultSettings && toolbarActions.Count == 0 )
					{
						foreach( var action in EditorActions.Actions )
						{
							if( action.QatSupport && action.QatAddByDefault )
							{
								var item = new ActionItem();
								item.Name = action.Name;
								toolbarActions.Add( item );
							}
						}
					}
					return toolbarActions;
				}
				set { toolbarActions = value; }
			}
			List<ActionItem> toolbarActions = new List<ActionItem>();

			////////////

			public sealed class TabItem
			{
				public enum TypeEnum
				{
					Basic,
					Additional,
				}

				[Serialize]
				public TypeEnum Type = TypeEnum.Basic;

				[Serialize]
				public string Name;

				[Serialize]
				public bool Enabled = true;

				[Serialize]
				[Cloneable( CloneType.Deep )]
				public List<GroupItem> Groups = new List<GroupItem>();

				//

				public bool Equals( TabItem obj )
				{
					if( Type != obj.Type || Name != obj.Name || Enabled != obj.Enabled )
						return false;

					if( Groups.Count != obj.Groups.Count )
						return false;
					for( int n = 0; n < Groups.Count; n++ )
						if( !Groups[ n ].Equals( obj.Groups[ n ] ) )
							return false;

					return true;
				}

				public TabItem Clone()
				{
					var item = new TabItem();
					item.Type = Type;
					item.Name = Name;
					item.Enabled = Enabled;

					foreach( var group in Groups )
						item.Groups.Add( group.Clone() );

					return item;
				}
			}

			////////////

			public sealed class GroupItem
			{
				public enum TypeEnum
				{
					Basic,
					Additional,
				}

				[Serialize]
				public TypeEnum Type = TypeEnum.Basic;

				[Serialize]
				public string Name;

				[Serialize]
				public bool Enabled = true;

				[Serialize]
				[Cloneable( CloneType.Deep )]
				public List<ActionItem> Actions = new List<ActionItem>();

				//

				public bool Equals( GroupItem obj )
				{
					if( Type != obj.Type || Name != obj.Name || Enabled != obj.Enabled )
						return false;

					if( Actions.Count != obj.Actions.Count )
						return false;
					for( int n = 0; n < Actions.Count; n++ )
						if( !Actions[ n ].Equals( obj.Actions[ n ] ) )
							return false;

					return true;
				}

				public GroupItem Clone()
				{
					var item = new GroupItem();
					item.Type = Type;
					item.Name = Name;
					item.Enabled = Enabled;

					foreach( var action in Actions )
						item.Actions.Add( action.Clone() );

					return item;
				}
			}

			////////////

			public sealed class ActionItem
			{
				public enum TypeEnum
				{
					Action,
					SubGroupOfActions,
				}

				[Serialize]
				public TypeEnum Type = TypeEnum.Action;

				[Serialize]
				public string Name;

				[Serialize]
				public bool Enabled = true;

				//

				public bool Equals( ActionItem obj )
				{
					return Type == obj.Type && Name == obj.Name && Enabled == obj.Enabled;
				}

				public ActionItem Clone()
				{
					var item = new ActionItem();
					item.Type = Type;
					item.Name = Name;
					item.Enabled = Enabled;
					return item;
				}
			}

			////////////

			public void ResetToDefault()
			{
				UseDefaultSettings = false;
				ribbonTabs.Clear();
				toolbarActions.Clear();
				UseDefaultSettings = true;
			}

			public void SetToNotDefault()
			{
				var r = RibbonTabs;
				var a = ToolbarActions;
				UseDefaultSettings = false;
			}

			public bool Equals( RibbonAndToolbarActionsClass obj )
			{
				if( UseDefaultSettings != obj.UseDefaultSettings )
					return false;

				if( !UseDefaultSettings )
				{
					if( ribbonTabs.Count != obj.ribbonTabs.Count )
						return false;
					for( int n = 0; n < ribbonTabs.Count; n++ )
						if( !ribbonTabs[ n ].Equals( obj.ribbonTabs[ n ] ) )
							return false;

					if( toolbarActions.Count != obj.toolbarActions.Count )
						return false;
					for( int n = 0; n < toolbarActions.Count; n++ )
						if( !toolbarActions[ n ].Equals( obj.toolbarActions[ n ] ) )
							return false;
				}

				return true;
			}

			public RibbonAndToolbarActionsClass Clone()
			{
				var obj = new RibbonAndToolbarActionsClass();
				obj.UseDefaultSettings = UseDefaultSettings;

				if( !UseDefaultSettings )
				{
					obj.ribbonTabs.Clear();
					foreach( var tab in ribbonTabs )
						obj.ribbonTabs.Add( tab.Clone() );

					obj.toolbarActions.Clear();
					foreach( var item in toolbarActions )
						obj.toolbarActions.Add( item.Clone() );
				}

				return obj;
			}
		}

		[Category( "Ribbon and Toolbar Actions" )]
		[Serialize]
		public RibbonAndToolbarActionsClass RibbonAndToolbarActions
		{
			get { return ribbonAndToolbarSettings; }
			set { ribbonAndToolbarSettings = value; }
		}
		RibbonAndToolbarActionsClass ribbonAndToolbarSettings = new RibbonAndToolbarActionsClass();

		/////////////////////////////////////////

		public sealed class ShortcutSettingsClass
		{
			[Serialize]
			public bool UseDefaultSettings { get; set; } = true;

			[Serialize]
			[Cloneable( CloneType.Deep )]
			public List<ActionItem> Actions
			{
				get
				{
					//init default configuration
					if( UseDefaultSettings && actions.Count == 0 )
					{
						foreach( var action in EditorActions.Actions )
						{
							var item = new ActionItem();
							item.Name = action.Name;
							if( action.ShortcutKeys != null && action.ShortcutKeys.Length >= 1 )
							{
								item.Shortcut1 = action.ShortcutKeys[ 0 ];
								if( action.ShortcutKeys.Length >= 2 )
									item.Shortcut2 = action.ShortcutKeys[ 1 ];
							}
							actions.Add( item );
						}
					}
					return actions;
				}
				set { actions = value; }
			}
			List<ActionItem> actions = new List<ActionItem>();

			//////////////

			public sealed class ActionItem
			{
				[Serialize]
				public string Name;

				[Serialize]
				public Keys Shortcut1;

				[Serialize]
				public Keys Shortcut2;

				//

				public bool Equals( ActionItem obj )
				{
					return Name == obj.Name && Shortcut1 == obj.Shortcut1 && Shortcut2 == obj.Shortcut2;
				}

				public ActionItem Clone()
				{
					var item = new ActionItem();
					item.Name = Name;
					item.Shortcut1 = Shortcut1;
					item.Shortcut2 = Shortcut2;
					return item;
				}

				public Keys[] ToArray()
				{
					var list = new List<Keys>();
					if( Shortcut1 != Keys.None )
						list.Add( Shortcut1 );
					if( Shortcut2 != Keys.None )
						list.Add( Shortcut2 );
					return list.ToArray();
				}
			}

			////////////

			public void ResetToDefault()
			{
				UseDefaultSettings = false;
				actions.Clear();
				UseDefaultSettings = true;
			}

			public void SetToNotDefault()
			{
				var a = Actions;
				UseDefaultSettings = false;
			}

			public bool Equals( ShortcutSettingsClass obj )
			{
				if( UseDefaultSettings != obj.UseDefaultSettings )
					return false;

				if( !UseDefaultSettings )
				{
					if( actions.Count != obj.actions.Count )
						return false;
					for( int n = 0; n < actions.Count; n++ )
						if( !actions[ n ].Equals( obj.actions[ n ] ) )
							return false;
				}

				return true;
			}

			public ShortcutSettingsClass Clone()
			{
				var obj = new ShortcutSettingsClass();
				obj.UseDefaultSettings = UseDefaultSettings;

				if( !UseDefaultSettings )
				{
					obj.actions.Clear();
					foreach( var item in actions )
						obj.actions.Add( item.Clone() );
				}

				return obj;
			}

			public ActionItem GetActionItem( string name )
			{
				//!!!!slowly

				return Actions.Find( a => a.Name == name );
			}
		}

		[Category( "Shortcuts" )]
		[Serialize]
		public ShortcutSettingsClass ShortcutSettings
		{
			get { return shortcutSettings; }
			set { shortcutSettings = value; }
		}
		ShortcutSettingsClass shortcutSettings = new ShortcutSettingsClass();

		//

		///// <summary>
		///// The total time of engine splash screen in seconds. See SplashScreen.cs to make customized engine logo.
		///// </summary>
		/// <summary>
		/// The total time of engine splash screen in seconds.
		/// </summary>
		[Category( "Custom Splash Screen" )]
		//[DisplayName( "Engine Splash Screen Time (Pro)" )]
		[DefaultValue( 3.0 )]
		public Reference<double> EngineSplashScreenTime
		{
			get { if( _engineSplashScreenTime.BeginGet() ) EngineSplashScreenTime = _engineSplashScreenTime.Get( this ); return _engineSplashScreenTime.value; }
			set { if( _engineSplashScreenTime.BeginSet( ref value ) ) { try { EngineSplashScreenTimeChanged?.Invoke( this ); } finally { _engineSplashScreenTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EngineSplashScreenTime"/> property value changes.</summary>
		public event Action<Component_ProjectSettings> EngineSplashScreenTimeChanged;
		ReferenceField<double> _engineSplashScreenTime = 3.0;

		///// <summary>
		///// Whether to customize engine splash screen. Available only for Pro plan. See SplashScreen.cs to make customized engine logo.
		///// </summary>
		//[Category( "Custom Splash Screen" )]
		//[DefaultValue( false )]
		//public Reference<bool> CustomizeSplashScreen
		//{
		//	get { if( _customizeSplashScreen.BeginGet() ) CustomizeSplashScreen = _customizeSplashScreen.Get( this ); return _customizeSplashScreen.value; }
		//	set { if( _customizeSplashScreen.BeginSet( ref value ) ) { try { CustomizeSplashScreenChanged?.Invoke( this ); } finally { _customizeSplashScreen.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CustomizeSplashScreen"/> property value changes.</summary>
		//public event Action<Component_ProjectSettings> CustomizeSplashScreenChanged;
		//ReferenceField<bool> _customizeSplashScreen = false;

		///// <summary>
		///// The customized engine splash screen image. Available only for Pro plan.
		///// </summary>
		//[Category( "Custom Splash Screen" )]
		//[DefaultValue( null )]
		//public Reference<Component_Image> SplashScreenImage
		//{
		//	get { if( _splashScreenImage.BeginGet() ) SplashScreenImage = _splashScreenImage.Get( this ); return _splashScreenImage.value; }
		//	set { if( _splashScreenImage.BeginSet( ref value ) ) { try { SplashScreenImageChanged?.Invoke( this ); } finally { _splashScreenImage.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SplashScreenImage"/> property value changes.</summary>
		//public event Action<Component_ProjectSettings> SplashScreenImageChanged;
		//ReferenceField<Component_Image> _splashScreenImage = null;

		///// <summary>
		///// The total time of engine splash screen in seconds.
		///// </summary>
		//[Category( "Custom Splash Screen" )]
		//[DisplayName( "Engine Splash Screen Time (Pro)" )]
		//[DefaultValue( 3.0 )]
		//public double EngineSplashScreenTimeReadOnly
		//{
		//	get { return 3.0; }
		//}

		///// <summary>
		///// Whether to customize engine splash screen. Available only for Pro plan.
		///// </summary>
		//[Category( "Custom Splash Screen" )]
		//[DisplayName( "Customize Splash Screen" )]
		//[DefaultValue( false )]
		//public bool CustomizeSplashScreenReadOnly
		//{
		//	get { return false; }
		//}

		///// <summary>
		///// The customized engine splash screen image. Available only for Pro plan.
		///// </summary>
		//[Category( "Custom Splash Screen" )]
		//[DisplayName( "Splash Screen Image" )]
		//[DefaultValue( null )]
		//public Component_Image SplashScreenImageReadOnly
		//{
		//	get { return null; }
		//}

	}
}
