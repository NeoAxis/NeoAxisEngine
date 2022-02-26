// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Input;

namespace NeoAxis.Editor
{
	public partial class CSharpScriptEditor : DocumentWindow
	{
		string sentTextToEngineUndoSystem = "";

		double disableUpdateRestoreTime;

		//bool disableTextChanged;

		/////////////////////////////////////////

		class ComponentScriptAdapter : ScriptDocument
		{
			CSharpScript script;

			// only CSharpScript support context
			//public override Type ContextType { get { return script.Context.GetType(); } }
			public override bool IsCSharpScript { get { return true; } }

			public ComponentScriptAdapter( CSharpScript script )
			{
				this.script = script;
				script.CodeChanged += ( s ) => RaiseCodeChanged( EventArgs.Empty );
			}

			public override string LoadText()
			{
				return script.Code;
			}

			//public override bool SaveText( Microsoft.CodeAnalysis.Text.SourceText text )
			//{
			//	//script.Code = text.ToString();

			//	return true;
			//}
		}

		/////////////////////////////////////////

		public CSharpScript Script
		{
			get { return (CSharpScript)ObjectOfWindow; }
		}

		public CSharpScriptEditor()
		{
			InitializeComponent();
		}

		public override void InitDocumentWindow( DocumentInstance document, object objectOfWindow, bool openAsSettings, Dictionary<string, object> windowTypeSpecificOptions )
		{
			base.InitDocumentWindow( document, objectOfWindow, openAsSettings, windowTypeSpecificOptions );
		}

		protected override void OnDestroy()
		{
			if( disableUpdateRestoreTime != 0 )
			{
				Script.DisableUpdate = false;
				disableUpdateRestoreTime = 0;

				//to refresh preview texture
				if( !RenderingSystem.Disposed )
					Script.RaiseCodeChangedEvent();
			}

			base.OnDestroy();
		}

		private void CSharpDocumentWindow_Load( object sender, EventArgs e )
		{
			sentTextToEngineUndoSystem = Script.Code.Value;

			try
			{
				scriptEditorControl.Initialize( new ComponentScriptAdapter( Script ) );
				//scriptEditorControl.Editor.TextChanged += Editor_TextChanged;
				//scriptEditorControl.Editor.PreviewKeyDown += Editor_PreviewKeyDown;
			}
			catch( Exception exc )
			{
				Log.Warning( "Script editor control init failed: \n\n" + exc.ToString() );
				Enabled = false;
			}

			timer1.Start();

			Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;
		}

		//private void Editor_PreviewKeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		//{
		//	if( e.Key == Key.Z && ModifierKeys.HasFlag( Keys.Control ) )
		//		Document.UndoSystem?.DoUndo();
		//	if( e.Key == Key.Y && ModifierKeys.HasFlag( Keys.Control ) )
		//		Document.UndoSystem?.DoRedo();
		//}

		[Browsable( false )]
		public ScriptEditorControl ScriptEditorControl
		{
			get { return scriptEditorControl; }
		}

		public override void EditorActionGetState( EditorAction.GetStateContext context )
		{
			switch( context.Action.Name )
			{
			//case "Undo":
			//	//context.Enabled = scriptEditorControl.Editor != null && scriptEditorControl.Editor.CanUndo;
			//	return;

			//case "Redo":
			//	//context.Enabled = scriptEditorControl.Editor != null && scriptEditorControl.Editor.CanRedo;
			//	return;

			case "Comment Selection":
				context.Enabled = true;
				break;

			case "Uncomment Selection":
				context.Enabled = true;
				break;

			case "Rename":
				context.Enabled = scriptEditorControl.GetRenameSymbol() != null;
				break;

			case "Format Document":
				context.Enabled = true;
				break;

			case "Go To Definition":
				context.Enabled = scriptEditorControl.CanGoToDefinition( out _ );
				break;
			}

			base.EditorActionGetState( context );
		}

		public override void EditorActionClick( EditorAction.ClickContext context )
		{
			switch( context.Action.Name )
			{
			//case "Undo":
			//	//scriptEditorControl.Editor.Undo();
			//	return;

			//case "Redo":
			//	//scriptEditorControl.Editor.Redo();
			//	return;

			case "Comment Selection":
				scriptEditorControl.CommentUncommentSelection( true );
				return;

			case "Uncomment Selection":
				scriptEditorControl.CommentUncommentSelection( false );
				return;

			case "Rename":
				scriptEditorControl.TryShowRenameDialog();
				return;

			case "Format Document":
				scriptEditorControl.FormatDocument();
				return;

			case "Go To Definition":
				scriptEditorControl.TryGoToDefinition();
				return;
			}

			base.EditorActionClick( context );
		}

		//private void Editor_TextChanged( object sender, EventArgs e )
		//{
		//if( disableTextChanged )
		//	return;

		//var newText = scriptEditorControl.Editor.Text;

		//if( Script.Code.Value != newText )
		//{
		//	var oldValue = Script.Code;

		//	//!!!!
		//	Script.DisableUpdate = true;

		//	Script.Code = newText;

		//	var undoItems = new List<UndoActionPropertiesChange.Item>();
		//	var property = (Metadata.Property)MetadataManager.GetTypeOfNetType(
		//		typeof( CSharpScript ) ).MetadataGetMemberBySignature( "property:Code" );
		//	undoItems.Add( new UndoActionPropertiesChange.Item( Script, property, oldValue ) );

		//	var undoAction = new UndoActionPropertiesChange( undoItems );
		//	Document.CommitUndoAction( undoAction );

		//	//!!!!
		//	Log.Info( "undo added" );

		//	//!!!!
		//	scriptEditorControl.Editor.Document.UndoStack.ClearAll();
		//}
		//}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			var newValue = scriptEditorControl.Editor.Text;
			if( sentTextToEngineUndoSystem != newValue )
			{
				sentTextToEngineUndoSystem = newValue;

				var oldValue = Script.Code;

				//!!!!в опции редактора
				var time = 3.0;
				disableUpdateRestoreTime = EngineApp.GetSystemTime() + time;
				Script.DisableUpdate = true;

				if( newValue != Script.Code.Value )
				{
					//update Code
					Script.Code = newValue;

					//add undo
					var undoItems = new List<UndoActionPropertiesChange.Item>();
					var property = (Metadata.Property)MetadataManager.GetTypeOfNetType(
						typeof( CSharpScript ) ).MetadataGetMemberBySignature( "property:Code" );
					undoItems.Add( new UndoActionPropertiesChange.Item( Script, property, oldValue ) );
					var undoAction = new UndoActionPropertiesChange( undoItems );
					Document.CommitUndoAction( undoAction );
				}
			}

			if( disableUpdateRestoreTime != 0 && EngineApp.GetSystemTime() > disableUpdateRestoreTime )
			{
				Script.DisableUpdate = false;
				disableUpdateRestoreTime = 0;

				//to refresh preview texture
				Script.RaiseCodeChangedEvent();
			}


			//scriptEditorControl.Editor.Document.UndoStack.ClearAll();

			////check for update Script.Code
			//{
			//	var editor = scriptEditorControl.Editor;
			//	if( Script.Code.Value != editor.Text )
			//	{
			//		////var caret = editor.TextArea.Caret.Location;
			//		//var selectionStart = editor.SelectionStart;
			//		//var selectionLength = editor.SelectionLength;

			//		disableTextChanged = true;

			//		//xx xx;
			//		editor.Document.Text = Script.Code.Value;
			//		scriptEditorControl.Editor.Document.UndoStack.ClearAll();
			//		disableTextChanged = false;

			//		//try
			//		//{
			//		//	//editor.TextArea.Caret.Location = caret;
			//		//	editor.Select( selectionStart, selectionLength );
			//		//}
			//		//catch { }
			//	}
			//}
		}

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			var editor = scriptEditorControl.Editor;
			if( Script.Code.Value != editor.Text )
			{
				////var caret = editor.TextArea.Caret.Location;
				//var selectionStart = editor.SelectionStart;
				//var selectionLength = editor.SelectionLength;

				//disableTextChanged = true;
				editor.Document.Text = Script.Code.Value;
				scriptEditorControl.Editor.Document.UndoStack.ClearAll();

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
	}
}
