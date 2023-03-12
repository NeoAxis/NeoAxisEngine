// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis
{
	/// <summary>
	/// Represents a Text Editor page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_TextEditor : ProjectSettingsPage
	{
		/// <summary>
		/// Whether to display line numbers.
		/// </summary>
		[Category( "Text Editor" )]
		[DisplayName( "Display Line Numbers" )]
		[DefaultValue( false )]
		public Reference<bool> TextEditorDisplayLineNumbers
		{
			get { if( _textEditorDisplayLineNumbers.BeginGet() ) TextEditorDisplayLineNumbers = _textEditorDisplayLineNumbers.Get( this ); return _textEditorDisplayLineNumbers.value; }
			set { if( _textEditorDisplayLineNumbers.BeginSet( ref value ) ) { try { TextEditorDisplayLineNumbersChanged?.Invoke( this ); } finally { _textEditorDisplayLineNumbers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorDisplayLineNumbers"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_TextEditor> TextEditorDisplayLineNumbersChanged;
		ReferenceField<bool> _textEditorDisplayLineNumbers = false;

		/// <summary>
		/// Whether to wrap words.
		/// </summary>
		[Category( "Text Editor" )]
		[DisplayName( "Word Wrap" )]
		[DefaultValue( true )]
		public Reference<bool> TextEditorWordWrap
		{
			get { if( _textEditorWordWrap.BeginGet() ) TextEditorWordWrap = _textEditorWordWrap.Get( this ); return _textEditorWordWrap.value; }
			set { if( _textEditorWordWrap.BeginSet( ref value ) ) { try { TextEditorWordWrapChanged?.Invoke( this ); } finally { _textEditorWordWrap.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorWordWrap"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_TextEditor> TextEditorWordWrapChanged;
		ReferenceField<bool> _textEditorWordWrap = true;

		/// <summary>
		/// The name of the font.
		/// </summary>
		[Category( "Text Editor" )]
		[DisplayName( "Font" )]
		[DefaultValue( "Consolas" )]
		public Reference<string> TextEditorFont
		{
			get { if( _textEditorFont.BeginGet() ) TextEditorFont = _textEditorFont.Get( this ); return _textEditorFont.value; }
			set { if( _textEditorFont.BeginSet( ref value ) ) { try { TextEditorFontChanged?.Invoke( this ); } finally { _textEditorFont.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextEditorFont"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_TextEditor> TextEditorFontChanged;
		ReferenceField<string> _textEditorFont = "Consolas";

		/// <summary>
		/// The font size.
		/// </summary>
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
		public event Action<ProjectSettingsPage_TextEditor> TextEditorFontSizeChanged;
		ReferenceField<double> _textEditorFontSize = 13.0;

		/// <summary>
		/// The color of editor background for the light theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_TextEditor> TextEditorBackgroundColorLightThemeChanged;
		ReferenceField<ColorValue> _textEditorBackgroundColorLightTheme = new ColorValue( 0.98, 0.98, 0.98 );

		/// <summary>
		/// The color of editor foreground for the light theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_TextEditor> TextEditorForegroundColorLightThemeChanged;
		ReferenceField<ColorValue> _textEditorForegroundColorLightTheme = new ColorValue( 0.1, 0.1, 0.1 );

		/// <summary>
		/// The color of editor background for the dark theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_TextEditor> TextEditorBackgroundColorDarkThemeChanged;
		ReferenceField<ColorValue> _textEditorBackgroundColorDarkTheme = new ColorValue( 0.1, 0.1, 0.1 );

		/// <summary>
		/// The color of editor foreground for the dark theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_TextEditor> TextEditorForegroundColorDarkThemeChanged;
		ReferenceField<ColorValue> _textEditorForegroundColorDarkTheme = new ColorValue( 0.9, 0.9, 0.9 );

		/// <summary>
		/// The color of background selection for the light theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_TextEditor> TextEditorSelectionBackgroundLightThemeChanged;
		ReferenceField<ColorValue> _textEditorSelectionBackgroundLightTheme = new ColorValue( 0.4, 0.6, 1 );

		/// <summary>
		/// The color of foreground selection for the light theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_TextEditor> TextEditorSelectionForegroundLightThemeChanged;
		ReferenceField<ColorValue> _textEditorSelectionForegroundLightTheme = new ColorValue( 0.1, 0.1, 0.1 );

		/// <summary>
		/// The color of background selection for the dark theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_TextEditor> TextEditorSelectionBackgroundDarkThemeChanged;
		ReferenceField<ColorValue> _textEditorSelectionBackgroundDarkTheme = new ColorValue( 0.25, 0.37, 0.62 );

		/// <summary>
		/// The color of foreground selection for the dark theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_TextEditor> TextEditorSelectionForegroundDarkThemeChanged;
		ReferenceField<ColorValue> _textEditorSelectionForegroundDarkTheme = new ColorValue( 0.9, 0.9, 0.9 );

		/// <summary>
		/// The color of search background for the light theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_TextEditor> TextEditorSearchBackgroundLightThemeChanged;
		ReferenceField<ColorValue> _textEditorSearchBackgroundLightTheme = new ColorValue( 1, 0, 0 );

		/// <summary>
		/// The color of search background for the dark theme.
		/// </summary>
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
		public event Action<ProjectSettingsPage_TextEditor> TextEditorSearchBackgroundDarkThemeChanged;
		ReferenceField<ColorValue> _textEditorSearchBackgroundDarkTheme = new ColorValue( 0.8, 0.16, 0.16 );

		//[Category( "Text Editor" )]
		//[DisplayName( "Highlighting Scheme" )]
		//public string TextEditorHighlightingScheme
		//{
		//	get { return @"Settings files are located in the 'Base\Tools\Highlighting' folder."; }
		//}
	}
}
