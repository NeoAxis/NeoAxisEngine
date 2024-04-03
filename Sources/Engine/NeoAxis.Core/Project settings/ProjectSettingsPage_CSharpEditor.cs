// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Represents a C# Editor page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_CSharpEditor : ProjectSettingsPage
	{
		//!!!!(через метод чтени€ из файла напр€мую) нельз€ в в опци€х проекта, еще система компонент не проинициализирована. видать в другом месте такое сохран€ть
		//[Category( "C# Editor" )]
		//[DisplayName( "Build Configuration" )]
		//[DefaultValue( "Release" )]
		//public Reference<string> CSharpEditorBuildConfiguration
		//{
		//	get { if( _cSharpEditorBuildConfiguration.BeginGet() ) CSharpEditorBuildConfiguration = _cSharpEditorBuildConfiguration.Get( this ); return _cSharpEditorBuildConfiguration.value; }
		//	set { if( _cSharpEditorBuildConfiguration.BeginSet( this, ref value ) ) { try { CSharpEditorBuildConfigurationChanged?.Invoke( this ); } finally { _cSharpEditorBuildConfiguration.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CSharpEditorBuildConfiguration"/> property value changes.</summary>
		//public event Action<ProjectSettingsComponentPage_CSharpEditor> CSharpEditorBuildConfigurationChanged;
		//ReferenceField<string> _cSharpEditorBuildConfiguration = "Release";

		/// <summary>
		/// Whether to display line numbers.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Display Line Numbers" )]
		[DefaultValue( false )]
		public Reference<bool> CSharpEditorDisplayLineNumbers
		{
			get { if( _cSharpEditorDisplayLineNumbers.BeginGet() ) CSharpEditorDisplayLineNumbers = _cSharpEditorDisplayLineNumbers.Get( this ); return _cSharpEditorDisplayLineNumbers.value; }
			set { if( _cSharpEditorDisplayLineNumbers.BeginSet( this, ref value ) ) { try { CSharpEditorDisplayLineNumbersChanged?.Invoke( this ); } finally { _cSharpEditorDisplayLineNumbers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorDisplayLineNumbers"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorDisplayLineNumbersChanged;
		ReferenceField<bool> _cSharpEditorDisplayLineNumbers = false;

		/// <summary>
		/// Whether to display info markers.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Display Info Markers" )]
		[DefaultValue( false )]
		public Reference<bool> CSharpEditorDisplayInfoMarkers
		{
			get { if( _cSharpEditorDisplayInfoMarkers.BeginGet() ) CSharpEditorDisplayInfoMarkers = _cSharpEditorDisplayInfoMarkers.Get( this ); return _cSharpEditorDisplayInfoMarkers.value; }
			set { if( _cSharpEditorDisplayInfoMarkers.BeginSet( this, ref value ) ) { try { CSharpEditorDisplayInfoMarkersChanged?.Invoke( this ); } finally { _cSharpEditorDisplayInfoMarkers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorDisplayInfoMarkers"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorDisplayInfoMarkersChanged;
		ReferenceField<bool> _cSharpEditorDisplayInfoMarkers = false;

		/// <summary>
		/// Whether to display warning markers.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Display Warning Markers" )]
		[DefaultValue( true )]
		public Reference<bool> CSharpEditorDisplayWarningMarkers
		{
			get { if( _cSharpEditorDisplayWarningMarkers.BeginGet() ) CSharpEditorDisplayWarningMarkers = _cSharpEditorDisplayWarningMarkers.Get( this ); return _cSharpEditorDisplayWarningMarkers.value; }
			set { if( _cSharpEditorDisplayWarningMarkers.BeginSet( this, ref value ) ) { try { CSharpEditorDisplayWarningMarkersChanged?.Invoke( this ); } finally { _cSharpEditorDisplayWarningMarkers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorDisplayWarningMarkers"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorDisplayWarningMarkersChanged;
		ReferenceField<bool> _cSharpEditorDisplayWarningMarkers = true;

		/// <summary>
		/// Whether to display error markers.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Display Error Markers" )]
		[DefaultValue( true )]
		public Reference<bool> CSharpEditorDisplayErrorMarkers
		{
			get { if( _cSharpEditorDisplayErrorMarkers.BeginGet() ) CSharpEditorDisplayErrorMarkers = _cSharpEditorDisplayErrorMarkers.Get( this ); return _cSharpEditorDisplayErrorMarkers.value; }
			set { if( _cSharpEditorDisplayErrorMarkers.BeginSet( this, ref value ) ) { try { CSharpEditorDisplayErrorMarkersChanged?.Invoke( this ); } finally { _cSharpEditorDisplayErrorMarkers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorDisplayErrorMarkers"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorDisplayErrorMarkersChanged;
		ReferenceField<bool> _cSharpEditorDisplayErrorMarkers = true;

		/// <summary>
		/// Whether to display quick actions.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Display Quick Actions" )]
		[DefaultValue( true )]
		public Reference<bool> CSharpEditorDisplayQuickActions
		{
			get { if( _cSharpEditorDisplayQuickActions.BeginGet() ) CSharpEditorDisplayQuickActions = _cSharpEditorDisplayQuickActions.Get( this ); return _cSharpEditorDisplayQuickActions.value; }
			set { if( _cSharpEditorDisplayQuickActions.BeginSet( this, ref value ) ) { try { CSharpEditorDisplayQuickActionsChanged?.Invoke( this ); } finally { _cSharpEditorDisplayQuickActions.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorDisplayQuickActions"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorDisplayQuickActionsChanged;
		ReferenceField<bool> _cSharpEditorDisplayQuickActions = true;

		/// <summary>
		/// Whether to wrap words.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Word Wrap" )]
		[DefaultValue( true )]
		public Reference<bool> CSharpEditorWordWrap
		{
			get { if( _cSharpEditorWordWrap.BeginGet() ) CSharpEditorWordWrap = _cSharpEditorWordWrap.Get( this ); return _cSharpEditorWordWrap.value; }
			set { if( _cSharpEditorWordWrap.BeginSet( this, ref value ) ) { try { CSharpEditorWordWrapChanged?.Invoke( this ); } finally { _cSharpEditorWordWrap.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorWordWrap"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorWordWrapChanged;
		ReferenceField<bool> _cSharpEditorWordWrap = true;

		/// <summary>
		/// Whether to enable auto brace completion.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Brace Completion" )]
		[DefaultValue( false )]
		public Reference<bool> CSharpEditorBraceCompletion
		{
			get { if( _cSharpEditorBraceCompletion.BeginGet() ) CSharpEditorBraceCompletion = _cSharpEditorBraceCompletion.Get( this ); return _cSharpEditorBraceCompletion.value; }
			set { if( _cSharpEditorBraceCompletion.BeginSet( this, ref value ) ) { try { CSharpEditorBraceCompletionChanged?.Invoke( this ); } finally { _cSharpEditorBraceCompletion.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorBraceCompletion"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorBraceCompletionChanged;
		ReferenceField<bool> _cSharpEditorBraceCompletion = false;

		/// <summary>
		/// Whether to enable auto format when enter semicolon character.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Automatically Format Statement On ;" )]
		[DefaultValue( true )]
		public Reference<bool> CSharpEditorAutomaticallyFormatStatementOnSemicolon
		{
			get { if( _cSharpEditorAutomaticallyFormatStatementOnSemicolon.BeginGet() ) CSharpEditorAutomaticallyFormatStatementOnSemicolon = _cSharpEditorAutomaticallyFormatStatementOnSemicolon.Get( this ); return _cSharpEditorAutomaticallyFormatStatementOnSemicolon.value; }
			set { if( _cSharpEditorAutomaticallyFormatStatementOnSemicolon.BeginSet( this, ref value ) ) { try { CSharpEditorAutomaticallyFormatStatementOnSemicolonChanged?.Invoke( this ); } finally { _cSharpEditorAutomaticallyFormatStatementOnSemicolon.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorAutomaticallyFormatStatementOnSemicolon"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorAutomaticallyFormatStatementOnSemicolonChanged;
		ReferenceField<bool> _cSharpEditorAutomaticallyFormatStatementOnSemicolon = true;

		/// <summary>
		/// Whether to enable auto format when enter bracket character.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Automatically Format Block On }" )]
		[DefaultValue( true )]
		public Reference<bool> CSharpEditorAutomaticallyFormatBlockOnBracket
		{
			get { if( _cSharpEditorAutomaticallyFormatBlockOnBracket.BeginGet() ) CSharpEditorAutomaticallyFormatBlockOnBracket = _cSharpEditorAutomaticallyFormatBlockOnBracket.Get( this ); return _cSharpEditorAutomaticallyFormatBlockOnBracket.value; }
			set { if( _cSharpEditorAutomaticallyFormatBlockOnBracket.BeginSet( this, ref value ) ) { try { CSharpEditorAutomaticallyFormatBlockOnBracketChanged?.Invoke( this ); } finally { _cSharpEditorAutomaticallyFormatBlockOnBracket.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorAutomaticallyFormatBlockOnBracket"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorAutomaticallyFormatBlockOnBracketChanged;
		ReferenceField<bool> _cSharpEditorAutomaticallyFormatBlockOnBracket = true;

		//[Category( "C# Editor" )]
		//[DisplayName( "Default Text Color for Light Theme" )]
		//[DefaultValue( "0.1 0.1 0.1" )]
		//[ColorValueNoAlpha]
		//public Reference<ColorValue> CSharpEditorDefaultTextColorLightTheme
		//{
		//	get { if( _cSharpEditorDefaultTextColorLightTheme.BeginGet() ) CSharpEditorDefaultTextColorLightTheme = _cSharpEditorDefaultTextColorLightTheme.Get( this ); return _cSharpEditorDefaultTextColorLightTheme.value; }
		//	set { if( _cSharpEditorDefaultTextColorLightTheme.BeginSet( this, ref value ) ) { try { CSharpEditorDefaultTextColorLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorDefaultTextColorLightTheme.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CSharpEditorDefaultTextColorLightTheme"/> property value changes.</summary>
		//public event Action<ProjectSettingsComponentPage_CSharpEditor> CSharpEditorDefaultTextColorLightThemeChanged;
		//ReferenceField<ColorValue> _cSharpEditorDefaultTextColorLightTheme = new ColorValue( 0.1, 0.1, 0.1 );

		/// <summary>
		/// The name of the font.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Font" )]
		[DefaultValue( "Consolas" )]
		public Reference<string> CSharpEditorFont
		{
			get { if( _cSharpEditorFont.BeginGet() ) CSharpEditorFont = _cSharpEditorFont.Get( this ); return _cSharpEditorFont.value; }
			set { if( _cSharpEditorFont.BeginSet( this, ref value ) ) { try { CSharpEditorFontChanged?.Invoke( this ); } finally { _cSharpEditorFont.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorFont"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorFontChanged;
		ReferenceField<string> _cSharpEditorFont = "Consolas";

		/// <summary>
		/// The size of the font.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Font Size" )]
		[DefaultValue( 13.0 )]
		[Range( 6, 40 )]
		public Reference<double> CSharpEditorFontSize
		{
			get { if( _cSharpEditorFontSize.BeginGet() ) CSharpEditorFontSize = _cSharpEditorFontSize.Get( this ); return _cSharpEditorFontSize.value; }
			set { if( _cSharpEditorFontSize.BeginSet( this, ref value ) ) { try { CSharpEditorFontSizeChanged?.Invoke( this ); } finally { _cSharpEditorFontSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorFontSize"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorFontSizeChanged;
		ReferenceField<double> _cSharpEditorFontSize = 13.0;

		/// <summary>
		/// The background color for the light theme.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Background Color for Light Theme" )]
		[DefaultValue( "0.98 0.98 0.98" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorBackgroundColorLightTheme
		{
			get { if( _cSharpEditorBackgroundColorLightTheme.BeginGet() ) CSharpEditorBackgroundColorLightTheme = _cSharpEditorBackgroundColorLightTheme.Get( this ); return _cSharpEditorBackgroundColorLightTheme.value; }
			set { if( _cSharpEditorBackgroundColorLightTheme.BeginSet( this, ref value ) ) { try { CSharpEditorBackgroundColorLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorBackgroundColorLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorBackgroundColorLightTheme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorBackgroundColorLightThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorBackgroundColorLightTheme = new ColorValue( 0.98, 0.98, 0.98 );

		/// <summary>
		/// The background color for the dark theme.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Background Color for Dark Theme" )]
		[DefaultValue( "0.1 0.1 0.1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorBackgroundColorDarkTheme
		{
			get { if( _cSharpEditorBackgroundColorDarkTheme.BeginGet() ) CSharpEditorBackgroundColorDarkTheme = _cSharpEditorBackgroundColorDarkTheme.Get( this ); return _cSharpEditorBackgroundColorDarkTheme.value; }
			set { if( _cSharpEditorBackgroundColorDarkTheme.BeginSet( this, ref value ) ) { try { CSharpEditorBackgroundColorDarkThemeChanged?.Invoke( this ); } finally { _cSharpEditorBackgroundColorDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorBackgroundColorDarkTheme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorBackgroundColorDarkThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorBackgroundColorDarkTheme = new ColorValue( 0.1, 0.1, 0.1 );

		/// <summary>
		/// The background color of selected text for the light theme.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Selection Background for Light Theme" )]
		[DefaultValue( "0.4 0.6 1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorSelectionBackgroundLightTheme
		{
			get { if( _cSharpEditorSelectionBackgroundLightTheme.BeginGet() ) CSharpEditorSelectionBackgroundLightTheme = _cSharpEditorSelectionBackgroundLightTheme.Get( this ); return _cSharpEditorSelectionBackgroundLightTheme.value; }
			set { if( _cSharpEditorSelectionBackgroundLightTheme.BeginSet( this, ref value ) ) { try { CSharpEditorSelectionBackgroundLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorSelectionBackgroundLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorSelectionBackgroundLightTheme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorSelectionBackgroundLightThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorSelectionBackgroundLightTheme = new ColorValue( 0.4, 0.6, 1 );

		/// <summary>
		/// The foreground color of selected text for the light theme.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Selection Foreground for Light Theme" )]
		[DefaultValue( "0.1 0.1 0.1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorSelectionForegroundLightTheme
		{
			get { if( _cSharpEditorSelectionForegroundLightTheme.BeginGet() ) CSharpEditorSelectionForegroundLightTheme = _cSharpEditorSelectionForegroundLightTheme.Get( this ); return _cSharpEditorSelectionForegroundLightTheme.value; }
			set { if( _cSharpEditorSelectionForegroundLightTheme.BeginSet( this, ref value ) ) { try { CSharpEditorSelectionForegroundLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorSelectionForegroundLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorSelectionForegroundLightTheme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorSelectionForegroundLightThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorSelectionForegroundLightTheme = new ColorValue( 0.1, 0.1, 0.1 );

		/// <summary>
		/// The background color of selected text for the dark theme.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Selection Background for Dark Theme" )]
		[DefaultValue( "0.25 0.37 0.62" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorSelectionBackgroundDarkTheme
		{
			get { if( _cSharpEditorSelectionBackgroundDarkTheme.BeginGet() ) CSharpEditorSelectionBackgroundDarkTheme = _cSharpEditorSelectionBackgroundDarkTheme.Get( this ); return _cSharpEditorSelectionBackgroundDarkTheme.value; }
			set { if( _cSharpEditorSelectionBackgroundDarkTheme.BeginSet( this, ref value ) ) { try { CSharpEditorSelectionBackgroundDarkThemeChanged?.Invoke( this ); } finally { _cSharpEditorSelectionBackgroundDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorSelectionBackgroundDarkTheme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorSelectionBackgroundDarkThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorSelectionBackgroundDarkTheme = new ColorValue( 0.25, 0.37, 0.62 );

		/// <summary>
		/// The foreground color of selected text for the dark theme.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Selection Foreground for Dark Theme" )]
		[DefaultValue( "0.9 0.9 0.9" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorSelectionForegroundDarkTheme
		{
			get { if( _cSharpEditorSelectionForegroundDarkTheme.BeginGet() ) CSharpEditorSelectionForegroundDarkTheme = _cSharpEditorSelectionForegroundDarkTheme.Get( this ); return _cSharpEditorSelectionForegroundDarkTheme.value; }
			set { if( _cSharpEditorSelectionForegroundDarkTheme.BeginSet( this, ref value ) ) { try { CSharpEditorSelectionForegroundDarkThemeChanged?.Invoke( this ); } finally { _cSharpEditorSelectionForegroundDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorSelectionForegroundDarkTheme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorSelectionForegroundDarkThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorSelectionForegroundDarkTheme = new ColorValue( 0.9, 0.9, 0.9 );

		/// <summary>
		/// The background color of search text for the light theme.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Search Background for Light Theme" )]
		[DefaultValue( "1 0 0" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorSearchBackgroundLightTheme
		{
			get { if( _cSharpEditorSearchBackgroundLightTheme.BeginGet() ) CSharpEditorSearchBackgroundLightTheme = _cSharpEditorSearchBackgroundLightTheme.Get( this ); return _cSharpEditorSearchBackgroundLightTheme.value; }
			set { if( _cSharpEditorSearchBackgroundLightTheme.BeginSet( this, ref value ) ) { try { CSharpEditorSearchBackgroundLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorSearchBackgroundLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorSearchBackgroundLightTheme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorSearchBackgroundLightThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorSearchBackgroundLightTheme = new ColorValue( 1, 0, 0 );

		/// <summary>
		/// The background color of search text for the dark theme.
		/// </summary>
		[Category( "C# Editor" )]
		[DisplayName( "Search Background for Dark Theme" )]
		[DefaultValue( "0.8 0.16 0.16" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> CSharpEditorSearchBackgroundDarkTheme
		{
			get { if( _cSharpEditorSearchBackgroundDarkTheme.BeginGet() ) CSharpEditorSearchBackgroundDarkTheme = _cSharpEditorSearchBackgroundDarkTheme.Get( this ); return _cSharpEditorSearchBackgroundDarkTheme.value; }
			set { if( _cSharpEditorSearchBackgroundDarkTheme.BeginSet( this, ref value ) ) { try { CSharpEditorSearchBackgroundDarkThemeChanged?.Invoke( this ); } finally { _cSharpEditorSearchBackgroundDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CSharpEditorSearchBackgroundDarkTheme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_CSharpEditor> CSharpEditorSearchBackgroundDarkThemeChanged;
		ReferenceField<ColorValue> _cSharpEditorSearchBackgroundDarkTheme = new ColorValue( 0.8, 0.16, 0.16 );

		//[Category( "C# Editor" )]
		//[DisplayName( "Default Text Color for Dark Theme" )]
		//[DefaultValue( "0.9 0.9 0.9" )]
		//[ColorValueNoAlpha]
		//public Reference<ColorValue> CSharpEditorDefaultTextColorDarkTheme
		//{
		//	get { if( _cSharpEditorDefaultTextColorDarkTheme.BeginGet() ) CSharpEditorDefaultTextColorDarkTheme = _cSharpEditorDefaultTextColorDarkTheme.Get( this ); return _cSharpEditorDefaultTextColorDarkTheme.value; }
		//	set { if( _cSharpEditorDefaultTextColorDarkTheme.BeginSet( this, ref value ) ) { try { CSharpEditorDefaultTextColorDarkThemeChanged?.Invoke( this ); } finally { _cSharpEditorDefaultTextColorDarkTheme.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CSharpEditorDefaultTextColorDarkTheme"/> property value changes.</summary>
		//public event Action<ProjectSettingsComponentPage_CSharpEditor> CSharpEditorDefaultTextColorDarkThemeChanged;
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
		//	set { if( _cSharpEditorHighlightingSchemeLightTheme.BeginSet( this, ref value ) ) { try { CSharpEditorHighlightingSchemeLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorHighlightingSchemeLightTheme.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CSharpEditorHighlightingSchemeLightTheme"/> property value changes.</summary>
		//public event Action<ProjectSettingsComponentPage_CSharpEditor> CSharpEditorHighlightingSchemeLightThemeChanged;
		//ReferenceField<string> _cSharpEditorHighlightingSchemeLightTheme = @"Base\Tools\Highlighting\CSharpLight.xshd";

		//[Category( "C# Editor" )]
		//[DefaultValue( @"Assets\Base\Tools\Highlighting\CSharpLight.xshd" )]
		//public Reference<ReferenceValueType_Resource> CSharpEditorHighlightingLightTheme
		//{
		//	get { if( _cSharpEditorHighlightingLightTheme.BeginGet() ) CSharpEditorHighlightingLightTheme = _cSharpEditorHighlightingLightTheme.Get( this ); return _cSharpEditorHighlightingLightTheme.value; }
		//	set { if( _cSharpEditorHighlightingLightTheme.BeginSet( this, ref value ) ) { try { CSharpEditorHighlightingLightThemeChanged?.Invoke( this ); } finally { _cSharpEditorHighlightingLightTheme.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CSharpEditorHighlightingLightTheme"/> property value changes.</summary>
		//public event Action<ProjectSettingsComponentPage_CSharpEditor> CSharpEditorHighlightingLightThemeChanged;
		//ReferenceField<ReferenceValueType_Resource> _cSharpEditorHighlightingLightTheme = new ReferenceValueType_Resource( @"Assets\Base\Tools\Highlighting\CSharpLight.xshd" );

		//ReferenceField<ReferenceValueType_Resource> _cSharpEditorHighlightingLightTheme = new ReferenceValueType_Resource( @"Assets\Base\Tools\Highlighting\CSharpLight.xshd" );
	}
}
