// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;
using RoslynPad.Editor;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Linq;

namespace NeoAxis.Editor
{
	public partial class TextEditorDocumentWindow : DocumentWindow
	{
		bool textChangedEventDisabled;

		bool displayLineNumbers;
		bool wordWrap;

		//!!!!по идее это в TextEditorControl. но он юзается и в шейдерах
		ColorValue backgroundColor = new ColorValue( -1, 0, 0, 0 );
		ColorValue cursorColor = new ColorValue( -1, 0, 0, 0 );
		string currentFont = "";
		double currentFontSize;
		ColorValue currentSelectionBackground = new ColorValue( -1, 0, 0, 0 );
		ColorValue currentSelectionForeground = new ColorValue( -1, 0, 0, 0 );

		bool firstTick = true;

		static Dictionary<string, IHighlightingDefinition> loadedHighlightingDefinitions = new Dictionary<string, IHighlightingDefinition>();

		//

		public TextEditorDocumentWindow()
		{
			InitializeComponent();

			avalonTextEditor.Editor.TextChanged += TextEditor_TextChanged;
			avalonTextEditor.Editor.KeyDown += TextEditor_KeyDown;

			displayLineNumbers = ProjectSettings.Get.TextEditorDisplayLineNumbers;
			avalonTextEditor.Editor.ShowLineNumbers = displayLineNumbers;

			wordWrap = ProjectSettings.Get.TextEditorWordWrap;
			avalonTextEditor.Editor.WordWrap = wordWrap;

			avalonTextEditor.Editor.PreviewMouseWheel += Editor_PreviewMouseWheel;

			UpdateBackgroundForeground();
			UpdateFont();

			avalonTextEditor.InstallSearchReplacePanel();
			//SearchReplacePanel.Install( avalonTextEditor.Editor.TextArea );
		}

		[Browsable( false )]
		public bool ReadOnly
		{
			get
			{
				if( WindowTypeSpecificOptions.TryGetValue( "ReadOnly", out var result ) )
					return (bool)result;
				return false;
			}
		}

		[Browsable( false )]
		public string HighlightingScheme
		{
			get
			{
				if( WindowTypeSpecificOptions.TryGetValue( "HighlightingScheme", out var result ) )
					return (string)result;
				return "";
			}
		}

		private void TextEditor_TextChanged( object sender, EventArgs e )
		{
			if( textChangedEventDisabled )
				return;

			if( !string.IsNullOrEmpty( Document.RealFileName ) )
				Document.Modified = true;
		}

		public override void InitDocumentWindow( DocumentInstance document, object objectOfWindow, bool openAsSettings, Dictionary<string, object> windowTypeSpecificOptions )
		{
			base.InitDocumentWindow( document, objectOfWindow, openAsSettings, windowTypeSpecificOptions );

			document.SaveEvent += Document_SaveEvent;

			if( !string.IsNullOrEmpty( HighlightingScheme ) )
				UpdateHighlightingScheme();

			if( ReadOnly )
				avalonTextEditor.Editor.IsReadOnly = ReadOnly;
		}

		public string Data
		{
			get { return avalonTextEditor.Editor.Text; }
			set { avalonTextEditor.Editor.Text = value; }
		}

		private void TextEditorDocumentWindow_Load( object sender, EventArgs e )
		{
			var realFileName = Document.RealFileName;

			if( string.IsNullOrEmpty( Data ) && !string.IsNullOrEmpty( realFileName ) )
			{
				textChangedEventDisabled = true;
				try
				{
					Data = File.ReadAllText( realFileName );
				}
				catch( Exception e2 )
				{
					Log.Error( $"Unable to read file \'{realFileName}\'. " + e2.Message );
				}
				textChangedEventDisabled = false;
			}
		}

		public override void EditorActionGetState( EditorAction.GetStateContext context )
		{
			switch( context.Action.Name )
			{
			case "Undo":
				context.Enabled = avalonTextEditor.Editor.CanUndo;
				return;

			case "Redo":
				context.Enabled = avalonTextEditor.Editor.CanRedo;
				return;
			}

			base.EditorActionGetState( context );
		}

		public override void EditorActionClick( EditorAction.ClickContext context )
		{
			switch( context.Action.Name )
			{
			case "Undo":
				avalonTextEditor.Editor.Undo();
				return;

			case "Redo":
				avalonTextEditor.Editor.Redo();
				return;
			}

			base.EditorActionClick( context );
		}

		private void Document_SaveEvent( DocumentInstance document, string saveAsFileName, ref bool handled, ref bool result )
		{
			if( Destroyed )
				return;

			var realFileName = Document.RealFileName;

			if( !string.IsNullOrEmpty( realFileName ) )
			{
				handled = true;
				try
				{
					File.WriteAllText( realFileName, Data );
					result = true;
				}
				catch( Exception e )
				{
					Log.Error( $"Unable to write file \'{realFileName}\'. " + e.Message );
					result = false;
				}
			}
		}

		private void TextEditor_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			// So far this ternary remained pointless, might be useful in some very specific cases though
			var wpfKey = e.Key == System.Windows.Input.Key.System ? e.SystemKey : e.Key;
			//var winformModifiers = e.KeyboardDevice.Modifiers.ToWinforms();
			var winformKeys = (Keys)System.Windows.Input.KeyInterop.VirtualKeyFromKey( wpfKey );
			var args = new System.Windows.Forms.KeyEventArgs( winformKeys );//| winformModifiers );

			if( args.KeyCode != Keys.None )
			{
				if( EditorAPI.ProcessShortcuts( args.KeyCode ) )
				{
					e.Handled = true;
					return;
				}
			}
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || EditorUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			if( displayLineNumbers != ProjectSettings.Get.TextEditorDisplayLineNumbers )
			{
				displayLineNumbers = ProjectSettings.Get.TextEditorDisplayLineNumbers;
				avalonTextEditor.Editor.ShowLineNumbers = displayLineNumbers;
			}

			if( wordWrap != ProjectSettings.Get.TextEditorWordWrap )
			{
				wordWrap = ProjectSettings.Get.TextEditorWordWrap;
				avalonTextEditor.Editor.WordWrap = wordWrap;
			}

			UpdateBackgroundForeground();
			UpdateFont();

			if( firstTick )
			{
				if( WindowTypeSpecificOptions.TryGetValue( "SelectLine", out var result ) )
				{
					var selectLine = (int)result;
					if( selectLine != 0 )
					{
						var editor2 = avalonTextEditor.Editor;

						var text = editor2.Text;

						int currentLine = 1;
						int from = 0;
						int to = 0;
						for( int n = 0; n < text.Length; n++ )
						{
							var c = text[ n ];

							if( c == '\n' )
								currentLine++;

							if( currentLine == selectLine - 1 )
								from = n + 2;
							if( currentLine == selectLine )
								to = n;
						}

						try
						{
							editor2.CaretOffset = from;
							editor2.Select( from, to - from );

							//var text = editor2.Document.Text;
							//if( text.Length >= selectLine.Minimum )
							//{
							//	var text2 = text.Substring( 0, selectLine.Minimum );
							//	int line = text2.Count( f => f == '\n' ) + 1;
							editor2.ScrollToLine( selectLine );
							//}
						}
						catch( Exception e2 )
						{
							Log.Warning( e2.Message );
						}
					}

					//var selectRange = (RangeI)result;
					//if( selectRange != RangeI.Zero )
					//{
					//	var editor2 = avalonTextEditor.Editor;

					//	editor2.CaretOffset = selectRange.Minimum;
					//	editor2.Select( selectRange.Minimum, selectRange.Size );

					//	var text = editor2.Document.Text;
					//	if( text.Length >= selectRange.Minimum )
					//	{
					//		var text2 = text.Substring( 0, selectRange.Minimum );
					//		int line = text2.Count( f => f == '\n' ) + 1;
					//		editor2.ScrollToLine( line );
					//	}

					//	//editor2.ScrollToLine( selectRange );
					//}
				}
			}

			firstTick = false;
		}

		void UpdateBackgroundForeground()
		{
			{
				var color = EditorAPI.DarkTheme ? ProjectSettings.Get.TextEditorBackgroundColorDarkTheme : ProjectSettings.Get.TextEditorBackgroundColorLightTheme;
				if( backgroundColor != color )
				{
					backgroundColor = color;
					var packed = backgroundColor.ToColorPacked();
					avalonTextEditor.Editor.Background = new SolidColorBrush( System.Windows.Media.Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
				}
			}

			{
				var color = EditorAPI.DarkTheme ? ProjectSettings.Get.TextEditorForegroundColorDarkTheme : ProjectSettings.Get.TextEditorForegroundColorLightTheme;
				if( cursorColor != color )
				{
					cursorColor = color;
					var packed = cursorColor.ToColorPacked();
					avalonTextEditor.Editor.Foreground = new SolidColorBrush( System.Windows.Media.Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
				}
			}

			{
				var color = EditorAPI.DarkTheme ? ProjectSettings.Get.TextEditorSearchBackgroundDarkTheme.Value : ProjectSettings.Get.TextEditorSearchBackgroundLightTheme.Value;
				var packed = color.ToColorPacked();
				avalonTextEditor.Editor.TextArea.SearchBackgroundBrush = new SolidColorBrush( System.Windows.Media.Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
			}
		}

		void UpdateFont()
		{
			if( currentFont != ProjectSettings.Get.TextEditorFont )
			{
				currentFont = ProjectSettings.Get.TextEditorFont;

				try
				{
					avalonTextEditor.Editor.FontFamily = new System.Windows.Media.FontFamily( ProjectSettings.Get.TextEditorFont );
				}
				catch { }
			}

			if( currentFontSize != ProjectSettings.Get.TextEditorFontSize )
			{
				currentFontSize = ProjectSettings.Get.TextEditorFontSize;

				try
				{
					avalonTextEditor.Editor.FontSize = ProjectSettings.Get.TextEditorFontSize;
				}
				catch { }
			}

			var selectionBackground = EditorAPI.DarkTheme ? ProjectSettings.Get.TextEditorSelectionBackgroundDarkTheme.Value : ProjectSettings.Get.TextEditorSelectionBackgroundLightTheme.Value;
			if( currentSelectionBackground != selectionBackground )
			{
				currentSelectionBackground = selectionBackground;

				try
				{
					var packed = selectionBackground.ToColorPacked();
					avalonTextEditor.Editor.TextArea.SelectionBrush = new SolidColorBrush( System.Windows.Media.Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
				}
				catch { }
			}

			var selectionForeground = EditorAPI.DarkTheme ? ProjectSettings.Get.TextEditorSelectionForegroundDarkTheme.Value : ProjectSettings.Get.TextEditorSelectionForegroundLightTheme.Value;
			if( currentSelectionForeground != selectionForeground )
			{
				currentSelectionForeground = selectionForeground;

				try
				{
					var packed = selectionForeground.ToColorPacked();
					avalonTextEditor.Editor.TextArea.SelectionForeground = new SolidColorBrush( System.Windows.Media.Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
				}
				catch { }
			}

		}

		private void Editor_PreviewMouseWheel( object sender, MouseWheelEventArgs e )
		{
			if( Keyboard.Modifiers.HasFlag( System.Windows.Input.ModifierKeys.Control ) )
			{
				var v = ProjectSettings.Get.TextEditorFontSize.Value;
				v += e.Delta > 0 ? 1 : -1;
				v = MathEx.Clamp( v, 6, 40 );
				ProjectSettings.Get.TextEditorFontSize = v;

				ProjectSettings.SaveToFileAndUpdate();

				e.Handled = true;
			}
		}

		void UpdateHighlightingScheme()
		{
			if( !loadedHighlightingDefinitions.TryGetValue( HighlightingScheme, out var definition ) )
			{
				string path;
				if( EditorAPI.DarkTheme )
					path = string.Format( @"Base\Tools\Highlighting\{0}Dark.xshd", HighlightingScheme );
				else
					path = string.Format( @"Base\Tools\Highlighting\{0}Light.xshd", HighlightingScheme );

				try
				{
					var fullPath = VirtualPathUtility.GetRealPathByVirtual( path );
					if( File.Exists( fullPath ) )
						definition = HighlightingManager.Instance.LoadFromFile( fullPath );
				}
				catch( Exception e )
				{
					Log.Warning( "Updating highlighting scheme error. " + e.Message );
				}

				if( definition != null )
					loadedHighlightingDefinitions[ HighlightingScheme ] = definition;
			}

			if( definition != null )
				avalonTextEditor.Editor.SyntaxHighlighting = definition;
		}
	}
}
