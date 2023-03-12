// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis
{
	/// <summary>
	/// Represents a Shader Editor page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_ShaderEditor : ProjectSettingsPage
	{
		/// <summary>
		/// Whether to display line numbers.
		/// </summary>
		[Category( "Shader Editor" )]
		[DisplayName( "Display Line Numbers" )]
		[DefaultValue( false )]
		public Reference<bool> ShaderEditorDisplayLineNumbers
		{
			get { if( _shaderEditorDisplayLineNumbers.BeginGet() ) ShaderEditorDisplayLineNumbers = _shaderEditorDisplayLineNumbers.Get( this ); return _shaderEditorDisplayLineNumbers.value; }
			set { if( _shaderEditorDisplayLineNumbers.BeginSet( ref value ) ) { try { ShaderEditorDisplayLineNumbersChanged?.Invoke( this ); } finally { _shaderEditorDisplayLineNumbers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorDisplayLineNumbers"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_ShaderEditor> ShaderEditorDisplayLineNumbersChanged;
		ReferenceField<bool> _shaderEditorDisplayLineNumbers = false;

		/// <summary>
		/// Whether to wrap words.
		/// </summary>
		[Category( "Shader Editor" )]
		[DisplayName( "Word Wrap" )]
		[DefaultValue( true )]
		public Reference<bool> ShaderEditorWordWrap
		{
			get { if( _shaderEditorWordWrap.BeginGet() ) ShaderEditorWordWrap = _shaderEditorWordWrap.Get( this ); return _shaderEditorWordWrap.value; }
			set { if( _shaderEditorWordWrap.BeginSet( ref value ) ) { try { ShaderEditorWordWrapChanged?.Invoke( this ); } finally { _shaderEditorWordWrap.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorWordWrap"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_ShaderEditor> ShaderEditorWordWrapChanged;
		ReferenceField<bool> _shaderEditorWordWrap = true;

		/// <summary>
		/// The background color for the light theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_ShaderEditor> ShaderEditorBackgroundColorLightThemeChanged;
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
		//public event Action<ProjectSettingsComponentPage_ShaderEditor> ShaderEditorDefaultTextColorLightThemeChanged;
		//ReferenceField<ColorValue> _shaderEditorDefaultTextColorLightTheme = new ColorValue( 0.1, 0.1, 0.1 );

		/// <summary>
		/// The name of the font.
		/// </summary>
		[Category( "Shader Editor" )]
		[DisplayName( "Font" )]
		[DefaultValue( "Consolas" )]
		public Reference<string> ShaderEditorFont
		{
			get { if( _shaderEditorFont.BeginGet() ) ShaderEditorFont = _shaderEditorFont.Get( this ); return _shaderEditorFont.value; }
			set { if( _shaderEditorFont.BeginSet( ref value ) ) { try { ShaderEditorFontChanged?.Invoke( this ); } finally { _shaderEditorFont.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderEditorFont"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_ShaderEditor> ShaderEditorFontChanged;
		ReferenceField<string> _shaderEditorFont = "Consolas";

		/// <summary>
		/// The size of the font.
		/// </summary>
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
		public event Action<ProjectSettingsPage_ShaderEditor> ShaderEditorFontSizeChanged;
		ReferenceField<double> _shaderEditorFontSize = 13.0;

		/// <summary>
		/// The background color for the dark theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_ShaderEditor> ShaderEditorBackgroundColorDarkThemeChanged;
		ReferenceField<ColorValue> _shaderEditorBackgroundColorDarkTheme = new ColorValue( 0.1, 0.1, 0.1 );

		/// <summary>
		/// The background color of selected text for the light theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_ShaderEditor> ShaderEditorSelectionBackgroundLightThemeChanged;
		ReferenceField<ColorValue> _shaderEditorSelectionBackgroundLightTheme = new ColorValue( 0.4, 0.6, 1 );

		/// <summary>
		/// The foreground color of selected text for the light theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_ShaderEditor> ShaderEditorSelectionForegroundLightThemeChanged;
		ReferenceField<ColorValue> _shaderEditorSelectionForegroundLightTheme = new ColorValue( 0.1, 0.1, 0.1 );

		/// <summary>
		/// The background color of selected text for the dark theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_ShaderEditor> ShaderEditorSelectionBackgroundDarkThemeChanged;
		ReferenceField<ColorValue> _shaderEditorSelectionBackgroundDarkTheme = new ColorValue( 0.25, 0.37, 0.62 );

		/// <summary>
		/// The foreground color of selected text for the dark theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_ShaderEditor> ShaderEditorSelectionForegroundDarkThemeChanged;
		ReferenceField<ColorValue> _shaderEditorSelectionForegroundDarkTheme = new ColorValue( 0.9, 0.9, 0.9 );

		/// <summary>
		/// The background color of search text for the light theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_ShaderEditor> ShaderEditorSearchBackgroundLightThemeChanged;
		ReferenceField<ColorValue> _shaderEditorSearchBackgroundLightTheme = new ColorValue( 1, 0, 0 );

		/// <summary>
		/// The background color of search text for the dark theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_ShaderEditor> ShaderEditorSearchBackgroundDarkThemeChanged;
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
		//public event Action<ProjectSettingsComponentPage_ShaderEditor> ShaderEditorDefaultTextColorDarkThemeChanged;
		//ReferenceField<ColorValue> _shaderEditorDefaultTextColorDarkTheme = new ColorValue( 0.9, 0.9, 0.9 );

		[Category( "Shader Editor" )]
		[DisplayName( "Highlighting Scheme" )]
		public string ShaderEditorHighlightingScheme
		{
			get { return @"Settings files are located in the 'Base\Tools\Highlighting' folder."; }
		}
	}
}
