// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows.Media;
using System.IO;
using System.Windows.Input;
using RoslynPad.Editor;

namespace NeoAxis.Editor
{
	public partial class Component_ShaderScript_DocumentWindow : DocumentWindow
	{
		string sentTextToEngineUndoSystem = "";
		double disableUpdateRestoreTime;

		bool displayLineNumbers;
		bool wordWrap;

		//!!!!по идее это в TextEditorControl. но он юзается и в шейдерах
		//!!!!!теперь в TextEditorControl есть базовые опции
		ColorValue backgroundColor = new ColorValue( -1, 0, 0, 0 );
		//ColorValue textColor = new ColorValue( -1, 0, 0, 0 );
		string currentFont = "";
		double currentFontSize;
		ColorValue currentSelectionBackground = new ColorValue( -1, 0, 0, 0 );
		ColorValue currentSelectionForeground = new ColorValue( -1, 0, 0, 0 );

		static IHighlightingDefinition loadedHighlightingDefinition;

		//bool disableTextChanged;

		/////////////////////////////////////////

		public Component_ShaderScript Script
		{
			get { return (Component_ShaderScript)ObjectOfWindow; }
		}

		public Component_ShaderScript_DocumentWindow()
		{
			InitializeComponent();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			UpdateHighlightingScheme();
			UpdateBackgroundForeground();
			UpdateFont();

			HighlightingColor defaultNamedColor = null;
			if( loadedHighlightingDefinition != null )
				defaultNamedColor = loadedHighlightingDefinition.GetNamedColor( "Default" );

			if( defaultNamedColor != null )
				avalonTextEditor.Editor.Foreground = defaultNamedColor.Foreground.GetBrush( null );

			avalonTextEditor.Editor.PreviewMouseWheel += Editor_PreviewMouseWheel;

			avalonTextEditor.InstallSearchReplacePanel();
			//searchReplacePanel = SearchReplacePanel.Install( avalonTextEditor.Editor.TextArea );
		}

		protected override void OnDestroy()
		{
			if( disableUpdateRestoreTime != 0 )
			{
				Script.TemporarilyDisableUpdate = false;
				disableUpdateRestoreTime = 0;

				//!!!!
				////to refresh preview texture
				//if( !RendererWorld.Disposed )
				//	Script.RaiseCodeChangedEventAndSetNeedUpdate();
			}

			base.OnDestroy();
		}

		private void ShaderScriptDocumentWindow_Load( object sender, EventArgs e )
		{
			sentTextToEngineUndoSystem = Script.Code.Value;

			try
			{
				avalonTextEditor.Editor.Text = sentTextToEngineUndoSystem;

				avalonTextEditor.Editor.KeyDown += TextEditor_KeyDown;
				//avalonTextEditor.Editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition( "Shader" );

				displayLineNumbers = ProjectSettings.Get.ShaderEditorDisplayLineNumbers;
				avalonTextEditor.Editor.ShowLineNumbers = displayLineNumbers;

				wordWrap = ProjectSettings.Get.ShaderEditorWordWrap;
				avalonTextEditor.Editor.WordWrap = wordWrap;
			}
			catch( Exception exc )
			{
				Log.Warning( "Shader editor control init failed: \n\n" + exc.ToString() );
				Enabled = false;
			}

			timer1.Start();

			Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;
		}

		[Browsable( false )]
		public TextEditorControl TextEditor
		{
			get { return avalonTextEditor; }
		}

		public override void EditorActionGetState( EditorAction.GetStateContext context )
		{
			//switch( context.Action.Name )
			//{
			////case "Undo":
			////	//context.Enabled = scriptEditorControl.Editor != null && scriptEditorControl.Editor.CanUndo;
			////	return;

			////case "Redo":
			////	//context.Enabled = scriptEditorControl.Editor != null && scriptEditorControl.Editor.CanRedo;
			////	return;

			//case "Comment Selection":
			//	context.Enabled = true;
			//	break;

			//case "Uncomment Selection":
			//	context.Enabled = true;
			//	break;

			//case "Rename":
			//	context.Enabled = scriptEditorControl.GetRenameSymbol() != null;
			//	break;

			//case "Format Document":
			//	context.Enabled = true;
			//	break;
			//}

			base.EditorActionGetState( context );
		}

		public override void EditorActionClick( EditorAction.ClickContext context )
		{
			//switch( context.Action.Name )
			//{
			////case "Undo":
			////	//scriptEditorControl.Editor.Undo();
			////	return;

			////case "Redo":
			////	//scriptEditorControl.Editor.Redo();
			////	return;

			//case "Comment Selection":
			//	scriptEditorControl.CommentUncommentSelection( true );
			//	break;

			//case "Uncomment Selection":
			//	scriptEditorControl.CommentUncommentSelection( false );
			//	break;

			//case "Rename":
			//	scriptEditorControl.TryShowRenameDialog();
			//	break;

			//case "Format Document":
			//	scriptEditorControl.FormatDocument();
			//	break;
			//}

			base.EditorActionClick( context );
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			var newValue = avalonTextEditor.Editor.Text;
			if( sentTextToEngineUndoSystem != newValue )
			{
				sentTextToEngineUndoSystem = newValue;

				var oldValue = Script.Code;

				//!!!!в опции редактора
				var time = 3.0;
				disableUpdateRestoreTime = EngineApp.GetSystemTime() + time;
				Script.TemporarilyDisableUpdate = true;

				if( newValue != Script.Code.Value )
				{
					//update Code
					Script.Code = newValue;

					//add undo
					var undoItems = new List<UndoActionPropertiesChange.Item>();
					var property = (Metadata.Property)MetadataManager.GetTypeOfNetType(
						typeof( Component_ShaderScript ) ).MetadataGetMemberBySignature( "property:Code" );
					undoItems.Add( new UndoActionPropertiesChange.Item( Script, property, oldValue ) );
					var undoAction = new UndoActionPropertiesChange( undoItems );
					Document.CommitUndoAction( undoAction );
				}
			}

			if( disableUpdateRestoreTime != 0 && EngineApp.GetSystemTime() > disableUpdateRestoreTime )
			{
				Script.TemporarilyDisableUpdate = false;
				disableUpdateRestoreTime = 0;

				//!!!!
				////to refresh preview texture
				//Script.RaiseCodeChangedEventAndSetNeedUpdate();

				//update materials. EditorUpdateWhenDocumentModified
				Document.EditorUpdateWhenDocumentModified_NeedUpdate( EngineApp.GetSystemTime() + 0.1 );
			}

			if( displayLineNumbers != ProjectSettings.Get.ShaderEditorDisplayLineNumbers )
			{
				displayLineNumbers = ProjectSettings.Get.ShaderEditorDisplayLineNumbers;
				avalonTextEditor.Editor.ShowLineNumbers = displayLineNumbers;
			}

			if( wordWrap != ProjectSettings.Get.ShaderEditorWordWrap )
			{
				wordWrap = ProjectSettings.Get.ShaderEditorWordWrap;
				avalonTextEditor.Editor.WordWrap = wordWrap;
			}

			UpdateBackgroundForeground();
			UpdateFont();
		}

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			var editor = avalonTextEditor.Editor;
			if( Script.Code.Value != editor.Text )
			{
				////var caret = editor.TextArea.Caret.Location;
				//var selectionStart = editor.SelectionStart;
				//var selectionLength = editor.SelectionLength;

				//disableTextChanged = true;
				editor.Document.Text = Script.Code.Value;
				avalonTextEditor.Editor.Document.UndoStack.ClearAll();

				sentTextToEngineUndoSystem = Script.Code.Value;

				//disableTextChanged = false;

				//try
				//{
				//	//editor.TextArea.Caret.Location = caret;
				//	editor.Select( selectionStart, selectionLength );
				//}
				//catch { }
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
				if( EditorAPI.ProcessShortcuts( args.KeyCode, false ) )
				{
					e.Handled = true;
					return;
				}
			}
		}

		void UpdateHighlightingScheme()
		{
			if( loadedHighlightingDefinition == null )
			{
				string path;
				if( EditorAPI.DarkTheme )
					path = @"Base\Tools\Highlighting\ShaderDark.xshd";
				else
					path = @"Base\Tools\Highlighting\ShaderLight.xshd";

				try
				{
					var fullPath = VirtualPathUtility.GetRealPathByVirtual( path );
					if( File.Exists( fullPath ) )
						loadedHighlightingDefinition = HighlightingManager.Instance.LoadFromFile( fullPath );
				}
				catch( Exception e )
				{
					Log.Warning( "Updating highlighting scheme error. " + e.Message );
				}
			}

			if( loadedHighlightingDefinition != null )
				avalonTextEditor.Editor.SyntaxHighlighting = loadedHighlightingDefinition;
		}

		void UpdateBackgroundForeground()
		{
			{
				var color = EditorAPI.DarkTheme ? ProjectSettings.Get.ShaderEditorBackgroundColorDarkTheme : ProjectSettings.Get.ShaderEditorBackgroundColorLightTheme;
				if( backgroundColor != color )
				{
					backgroundColor = color;
					var packed = backgroundColor.ToColorPacked();
					avalonTextEditor.Editor.Background = new SolidColorBrush( Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
				}
			}

			{
				var color = EditorAPI.DarkTheme ? ProjectSettings.Get.ShaderEditorSearchBackgroundDarkTheme.Value : ProjectSettings.Get.ShaderEditorSearchBackgroundLightTheme.Value;
				var packed = color.ToColorPacked();
				avalonTextEditor.Editor.TextArea.SearchBackgroundBrush = new SolidColorBrush( Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
			}

			//{
			//	var color = EditorAPI.DarkTheme ? ProjectSettings.Get.ShaderEditorDefaultTextColorDarkTheme : ProjectSettings.Get.ShaderEditorDefaultTextColorLightTheme;
			//	if( textColor != color )
			//	{
			//		textColor = color;
			//		var packed = textColor.ToColorPacked();
			//		avalonTextEditor.Editor.Foreground = new SolidColorBrush( System.Windows.Media.Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
			//	}
			//}
		}

		void UpdateFont()
		{
			if( currentFont != ProjectSettings.Get.ShaderEditorFont )
			{
				currentFont = ProjectSettings.Get.ShaderEditorFont;

				try
				{
					avalonTextEditor.Editor.FontFamily = new System.Windows.Media.FontFamily( ProjectSettings.Get.ShaderEditorFont );
				}
				catch { }
			}

			if( currentFontSize != ProjectSettings.Get.ShaderEditorFontSize )
			{
				currentFontSize = ProjectSettings.Get.ShaderEditorFontSize;

				try
				{
					avalonTextEditor.Editor.FontSize = ProjectSettings.Get.ShaderEditorFontSize;
				}
				catch { }
			}

			var selectionBackground = EditorAPI.DarkTheme ? ProjectSettings.Get.ShaderEditorSelectionBackgroundDarkTheme.Value : ProjectSettings.Get.ShaderEditorSelectionBackgroundLightTheme.Value;
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

			var selectionForeground = EditorAPI.DarkTheme ? ProjectSettings.Get.ShaderEditorSelectionForegroundDarkTheme.Value : ProjectSettings.Get.ShaderEditorSelectionForegroundLightTheme.Value;
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
				var v = ProjectSettings.Get.ShaderEditorFontSize.Value;
				v += e.Delta > 0 ? 1 : -1;
				v = MathEx.Clamp( v, 6, 40 );
				ProjectSettings.Get.ShaderEditorFontSize = v;

				ProjectSettings.SaveToFileAndUpdate();

				e.Handled = true;
			}
		}
	}
}
