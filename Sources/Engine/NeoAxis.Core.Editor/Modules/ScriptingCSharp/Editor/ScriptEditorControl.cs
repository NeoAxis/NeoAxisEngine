// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Toolkit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using RoslynPad.Roslyn;
using RoslynPad.Editor;
using RoslynPad.Roslyn.Rename;
using RoslynPad.Roslyn.GoToDefinition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Text;

namespace NeoAxis.Editor
{
	public class ScriptEditorControl : UserControl
	{
		readonly ErrorMargin errorMargin;
		ScriptDocument script;

		RoslynCodeEditor editor;

		DocumentId documentId;
		//!!!!
		//NuGetDocumentViewModel nuGet;
		private Timer timer1;
		private IContainer components;

		bool displayLineNumbers;
		bool wordWrap;
		bool braceCompletion;
		ColorValue backgroundColor = new ColorValue( -1, 0, 0, 0 );
		ColorValue textColor = new ColorValue( -1, 0, 0, 0 );
		string currentFont = "";
		double currentFontSize;
		ColorValue currentSelectionBackground = new ColorValue( -1, 0, 0, 0 );
		ColorValue currentSelectionForeground = new ColorValue( -1, 0, 0, 0 );
		bool displayQuickActions = true;

		static IHighlightingDefinition loadedHighlightingDefinition;
		private KryptonSplitContainer kryptonSplitContainer;
		private KryptonSplitContainer kryptonSplitContainerSub1;
		private EngineScrollBar engineScrollBarVertical;
		private KryptonSplitContainer kryptonSplitContainerSub2;
		private EngineScrollBar engineScrollBarHorizontal;

		//string currentHighlightingScheme;

		string initialCode = "";

		//

		public ScriptEditorControl()
		{
			InitializeComponent();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			editor = new RoslynCodeEditor();

			//editor.FontFamily = new FontFamily( "Consolas" );

			editor.ContextActionsIcon = (ImageSource)System.Windows.Application.Current.TryFindResource( "Bulb" );

			UpdateHighlightingScheme();

			displayLineNumbers = ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayLineNumbers;
			editor.ShowLineNumbers = displayLineNumbers;

			wordWrap = ProjectSettings.Get.CSharpEditor.CSharpEditorWordWrap;
			editor.WordWrap = wordWrap;

			editor.Options.ConvertTabsToSpaces = false;
			editor.TextArea.IndentationStrategy = new BlockIndentationStrategy();

			braceCompletion = ProjectSettings.Get.CSharpEditor.CSharpEditorBraceCompletion;
			editor.IsBraceCompletionEnabled = braceCompletion;

			editor.DisplayInfoMarkers = ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayInfoMarkers;
			editor.DisplayWarningMarkers = ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayWarningMarkers;
			editor.DisplayErrorMarkers = ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayErrorMarkers;

			errorMargin = new ErrorMargin
			{
				Visibility = Visibility.Collapsed,
				MarkerBrush = (Brush)System.Windows.Application.Current.TryFindResource( "ExceptionMarker" ),
				Width = 10
			};
			editor.TextArea.LeftMargins.Insert( 0, errorMargin );
			editor.PreviewMouseWheel += Editor_PreviewMouseWheel;
			//_editor.TextArea.Caret.PositionChanged += CaretOnPositionChanged;
			editor.Loaded += TextEditor_Loaded;
			editor.KeyDown += Editor_KeyDown;
			editor.TextArea.TextEntered += TextArea_TextEntered;

			editor.TextArea.SelectionBorder = null;
			editor.TextArea.SelectionCornerRadius = 0;

			editor.ContextMenuOpening += Editor_ContextMenuOpening;

			UpdateBackgroundForeground();
			UpdateFont();

			var codeEditorHost = new ElementHost();
			codeEditorHost.Dock = DockStyle.Fill;
			codeEditorHost.Child = editor;
			kryptonSplitContainerSub1.Panel1.Controls.Add( codeEditorHost );

			//SetScrollBarVisibilityToAuto();
		}

		[Browsable( false )]
		internal RoslynCodeEditor Editor
		{
			get { return editor; }
		}

		//public void SetScrollBarVisibilityToAuto()
		//{
		//	editor.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
		//	editor.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
		//}

		internal void Initialize( ScriptDocument script )
		{
			this.script = script;

			HighlightingColor defaultNamedColor = null;
			if( loadedHighlightingDefinition != null )
				defaultNamedColor = loadedHighlightingDefinition.GetNamedColor( "Default" );

			var colors = new ClassificationHighlightColors();
			if( defaultNamedColor != null )
				colors.DefaultBrush = defaultNamedColor;

			initialCode = script.LoadText();

			documentId = editor.Initialize( RoslynHost.Instance, colors, script.CsFileProjectPath,
				script.WorkingDirectory, initialCode, script.IsCSharpScript, typeof( CSharpScript.Context ) );// script.ContextType );

			var serviceProvider = ScriptEditorEngine.Instance.GetServiceProvider();
			//nuGet = serviceProvider.GetService<NuGetDocumentViewModel>();
			//nuGet.PackageInstalled += NuGetOnPackageInstalled;

			////ScriptEditorEngine.Instance.EditorFontSizeChanged += OnEditorFontSizeChanged;
			//editor.FontSize = ScriptEditorEngine.Instance.EditorFontSize;

			// if script code changed outside of this control
			script.CodeChanged += Script_CodeChanged;

			if( defaultNamedColor != null )
				editor.Foreground = defaultNamedColor.Foreground.GetBrush( null );
		}

		private void Script_CodeChanged( object sender, EventArgs e )
		{
			var script = (ScriptDocument)sender;
			var scriptText = script.LoadText();
			if( editor.Document.Text != scriptText )
				editor.Document.Text = scriptText;
		}

		//public bool SaveScript()
		//{
		//	return Save( script );
		//}

		//// not used.
		//void OnError( ExceptionResultObject e )
		//{
		//	if( e != null )
		//	{
		//		errorMargin.Visibility = Visibility.Visible;
		//		errorMargin.LineNumber = e.LineNumber;
		//		errorMargin.Message = "Exception: " + e.Message;
		//	}
		//	else
		//		errorMargin.Visibility = Visibility.Collapsed;
		//}

		//void OnEditorFontSizeChanged( double fontSize )
		//{
		//	editor.FontSize = fontSize;
		//}

		//!!!!
		//void NuGetOnPackageInstalled( NuGetInstallResult installResult )
		//{
		//	//!!!!?

		//	if( installResult.References.Count == 0 ) return;

		//	var text = string.Join( Environment.NewLine,
		//		installResult.References.Distinct().OrderBy( c => c )
		//		.Select( r => Path.Combine( ScriptEditorEngine.NuGetPathVariableName, r ) )
		//		.Concat( installResult.FrameworkReferences.Distinct() )
		//		.Where( r => !RoslynHost.Instance.HasReference( documentId, r ) )
		//		.Select( r => "#r \"" + r + "\"" )
		//		.Where( r => editor.Text.IndexOf( r, StringComparison.OrdinalIgnoreCase ) < 0 ) );

		//	//Dispatcher.InvokeAsync(() => textEditor.Document.Insert(0, text, ICSharpCode.AvalonEdit.Document.AnchorMovementType.Default));
		//	Task.Run( () => editor.Document.Insert( 0, text, ICSharpCode.AvalonEdit.Document.AnchorMovementType.Default ) );
		//}

		private void Editor_PreviewMouseWheel( object sender, MouseWheelEventArgs e )
		{
			if( Keyboard.Modifiers.HasFlag( System.Windows.Input.ModifierKeys.Control ) )
			{
				var v = ProjectSettings.Get.CSharpEditor.CSharpEditorFontSize.Value;
				v += e.Delta > 0 ? 1 : -1;
				v = MathEx.Clamp( v, 6, 40 );
				ProjectSettings.Get.CSharpEditor.CSharpEditorFontSize = v;

				ProjectSettings.SaveToFileAndUpdate();

				e.Handled = true;
			}
		}

		void TextEditor_Loaded( object sender, RoutedEventArgs e )
		{
			editor.Focus();
		}

		private void Editor_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			// So far this ternary remained pointless, might be useful in some very specific cases though
			var wpfKey = e.Key == System.Windows.Input.Key.System ? e.SystemKey : e.Key;
			//var winformModifiers = e.KeyboardDevice.Modifiers.ToWinforms();
			var winformKeys = (Keys)System.Windows.Input.KeyInterop.VirtualKeyFromKey( wpfKey );
			var args = new System.Windows.Forms.KeyEventArgs( winformKeys );//| winformModifiers );

			if( args.KeyCode != Keys.None )
			{
				if( EditorAPI2.ProcessShortcuts( args.KeyCode, false ) )
				{
					e.Handled = true;
					return;
				}
			}
		}

		void FormatDocument( TextSpan span )
		{
			var document = GetDocument();
			if( document == null )
				return;
			var options = document.GetOptionsAsync().Result;
			var options2 = options.WithChangedOption( FormattingOptions.UseTabs, document.Project.Language, true );
			var formattedDocument = Formatter.FormatAsync( document, span, options2 ).Result;
			RoslynHost.Instance.UpdateDocument( formattedDocument );
		}

		private void TextArea_TextEntered( object sender, TextCompositionEventArgs e )
		{
			if( ProjectSettings.Get.CSharpEditor.CSharpEditorAutomaticallyFormatStatementOnSemicolon && e.Text == ";" )
			{
				try
				{
					var caret = editor.TextArea.Caret;
					var text = editor.Document.Text;
					int to = caret.Offset - 1;

					//!!!!учитывать другие случаи, типа комментариев

					int from = -1;
					for( int current = caret.Offset - 2; current >= 0; current-- )
					{
						if( text[ current ] == ';' || text[ current ] == '\r' || text[ current ] == '\n' || current <= 0 )
						{
							from = current;
							break;
						}
					}

					if( from != -1 )
					{
						var span = new TextSpan( from, to - from );
						FormatDocument( span );
					}
				}
				catch( Exception e2 )
				{
					//can happen?
					Log.Warning( e2.Message );
				}
			}

			if( ProjectSettings.Get.CSharpEditor.CSharpEditorAutomaticallyFormatBlockOnBracket && e.Text == "}" )
			{
				try
				{
					var caret = editor.TextArea.Caret;
					var text = editor.Document.Text;
					int to = caret.Offset - 1;

					//!!!!учитывать другие случаи, типа скобка внутри комментария

					int from = -1;
					int toSkipBrackets = 0;
					for( int current = caret.Offset - 2; current >= 0; current-- )
					{
						if( text[ current ] == '}' )
							toSkipBrackets++;

						if( text[ current ] == '{' )
						{
							toSkipBrackets--;
							if( toSkipBrackets < 0 )
							{
								from = current;
								break;
							}
						}
					}

					if( from != -1 )
					{
						var span = new TextSpan( from, to - from );
						FormatDocument( span );
					}
				}
				catch( Exception e2 )
				{
					//can happen?
					Log.Warning( e2.Message );
				}
			}
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( documentId != null )
					RoslynHost.Instance.CloseDocument( documentId );

				////if( scriptEditor != null )
				//ScriptEditorEngine.Instance.EditorFontSizeChanged -= OnEditorFontSizeChanged;
			}
			base.Dispose( disposing );
		}


		//void OnTreeViewKeyDown( object sender, System.Windows.Forms.KeyEventArgs e )
		//{
		//	if( e.KeyCode == Keys.C && e.Control )
		//	{
		//		CopyToClipboard( sender );
		//	}
		//}

		//void CopyClick( object sender, RoutedEventArgs e )
		//{
		//	CopyToClipboard( sender );
		//}

		//static void CopyToClipboard( object sender )
		//{
		//	var element = (FrameworkElement)sender;
		//	var result = (ResultObject)element.DataContext;
		//	System.Windows.Clipboard.SetText( result.ToString() );
		//}

		Document GetDocument()
		{
			if( documentId != null )
				return RoslynHost.Instance.GetDocument( documentId );
			return null;
		}

		public void RenameSymbol( ISymbol symbol, string symbolName )
		{
			var document = GetDocument();
			if( document == null )
				return;
			var newSolution = Renamer.RenameSymbolAsync( document.Project.Solution, symbol, symbolName, null ).Result;
			var newDocument = newSolution.GetDocument( documentId );
			// TODO: possibly update entire solution
			RoslynHost.Instance.UpdateDocument( newDocument );
		}

		public void CommentUncommentSelection( bool comment )
		{
			const string singleLineCommentString = "//";
			var document = GetDocument();
			if( document == null )
				return;
			var selection = new TextSpan( editor.SelectionStart, editor.SelectionLength );
			var documentText = document.GetTextAsync().Result;
			var changes = new List<TextChange>();
			var lines = documentText.Lines.SkipWhile( x => !x.Span.IntersectsWith( selection ) )
				.TakeWhile( x => x.Span.IntersectsWith( selection ) ).ToArray();

			if( comment )
			{
				foreach( var line in lines )
				{
					if( !string.IsNullOrWhiteSpace( documentText.GetSubText( line.Span ).ToString() ) )
						changes.Add( new TextChange( new TextSpan( line.Start, 0 ), singleLineCommentString ) );
				}
			}
			else
			{
				foreach( var line in lines )
				{
					var text = documentText.GetSubText( line.Span ).ToString();
					if( text.TrimStart().StartsWith( singleLineCommentString, StringComparison.Ordinal ) )
					{
						changes.Add( new TextChange( new TextSpan(
							line.Start + text.IndexOf( singleLineCommentString, StringComparison.Ordinal ),
							singleLineCommentString.Length ), string.Empty ) );
					}
				}
			}

			if( changes.Count == 0 )
				return;

			RoslynHost.Instance.UpdateDocument( document.WithText( documentText.WithChanges( changes ) ) );
		}

		public void FormatDocument()
		{
			var document = GetDocument();
			if( document == null )
				return;
			var options = document.GetOptionsAsync().Result;
			var options2 = options.WithChangedOption( FormattingOptions.UseTabs, document.Project.Language, true );
			var formattedDocument = Formatter.FormatAsync( document, options2 ).Result;

			RoslynHost.Instance.UpdateDocument( formattedDocument );
		}

		//internal bool Save( ScriptDocument script )
		//{
		//	if( documentId == null )
		//		return true;

		//	var document = GetDocument();
		//	var text = document.GetTextAsync().Result;
		//	return script.SaveText( text );
		//}

		internal bool GetCode( out string code )
		{
			var document = GetDocument();
			if( document != null && document.TryGetText( out var text ) )
			{
				code = text.ToString();
				return true;
			}
			else
			{
				code = "";
				return false;
			}
		}

		[Browsable( false )]
		public string InitialCode
		{
			get { return initialCode; }
		}

		//async Task EnsureNuGetPackages()
		//{
		//	var nugetVariable = editor.NuGetConfiguration.PathVariableName;
		//	var pathToRepository = editor.NuGetConfiguration.PathToRepository;
		//	var directives = await editor.RoslynHost.GetDocument( DocumentId ).GetReferencesDirectivesAsync().ConfigureAwait( false );
		//	foreach( var directive in directives )
		//	{
		//		if( directive.StartsWith( nugetVariable, StringComparison.OrdinalIgnoreCase ) )
		//		{
		//			var directiveWithoutRoot = directive.Substring( nugetVariable.Length + 1 );
		//			if( !File.Exists( Path.Combine( pathToRepository, directiveWithoutRoot ) ) )
		//			{
		//				var sections = directiveWithoutRoot.Split( new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries );
		//				if( sections.Length > 2 && NuGetVersion.TryParse( sections[ 1 ], out var version ) )
		//				{
		//					await NuGet.InstallPackage( sections[ 0 ], version, reportInstalled: false ).ConfigureAwait( false );
		//				}
		//			}
		//		}
		//	}
		//}

		public ISymbol GetRenameSymbol()
		{
			var document = GetDocument();
			if( document == null )
				return null;
			return RenameHelper.GetRenameSymbol( document, editor.SelectionStart ).Result;
		}

		public bool AddMethod( string methodName, ParameterInfo[] parameters, out string error )
		{
			error = null;

			var document = GetDocument();
			if( document == null )
				return false;
			try
			{
				document = ScriptCodeGenerator.AddMethodToClass( document, ScriptCodeGenerator.GenerateMethodFromReflection( methodName, parameters ) );
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}
			RoslynHost.Instance.UpdateDocument( document );

			return true;
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer( this.components );
			this.kryptonSplitContainer = new Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.kryptonSplitContainerSub1 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.engineScrollBarVertical = new NeoAxis.Editor.EngineScrollBar();
			this.kryptonSplitContainerSub2 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.engineScrollBarHorizontal = new NeoAxis.Editor.EngineScrollBar();
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainer ) ).BeginInit();
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainer.Panel1 ) ).BeginInit();
			this.kryptonSplitContainer.Panel1.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainer.Panel2 ) ).BeginInit();
			this.kryptonSplitContainer.Panel2.SuspendLayout();
			this.kryptonSplitContainer.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainerSub1 ) ).BeginInit();
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainerSub1.Panel1 ) ).BeginInit();
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainerSub1.Panel2 ) ).BeginInit();
			this.kryptonSplitContainerSub1.Panel2.SuspendLayout();
			this.kryptonSplitContainerSub1.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainerSub2 ) ).BeginInit();
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainerSub2.Panel1 ) ).BeginInit();
			this.kryptonSplitContainerSub2.Panel1.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainerSub2.Panel2 ) ).BeginInit();
			this.kryptonSplitContainerSub2.SuspendLayout();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Interval = 25;
			this.timer1.Tick += new System.EventHandler( this.timer1_Tick );
			// 
			// kryptonSplitContainer
			// 
			this.kryptonSplitContainer.Cursor = System.Windows.Forms.Cursors.Default;
			this.kryptonSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.kryptonSplitContainer.IsSplitterFixed = true;
			this.kryptonSplitContainer.Location = new System.Drawing.Point( 0, 0 );
			this.kryptonSplitContainer.Name = "kryptonSplitContainer";
			this.kryptonSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// kryptonSplitContainer.Panel1
			// 
			this.kryptonSplitContainer.Panel1.Controls.Add( this.kryptonSplitContainerSub1 );
			// 
			// kryptonSplitContainer.Panel2
			// 
			this.kryptonSplitContainer.Panel2.Controls.Add( this.kryptonSplitContainerSub2 );
			this.kryptonSplitContainer.Panel2MinSize = 16;
			this.kryptonSplitContainer.Size = new System.Drawing.Size( 574, 323 );
			this.kryptonSplitContainer.SplitterDistance = 304;
			this.kryptonSplitContainer.SplitterPercent = 0.94117647058823528D;
			this.kryptonSplitContainer.SplitterWidth = 0;
			this.kryptonSplitContainer.TabIndex = 4;
			// 
			// kryptonSplitContainerSub1
			// 
			this.kryptonSplitContainerSub1.Cursor = System.Windows.Forms.Cursors.Default;
			this.kryptonSplitContainerSub1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonSplitContainerSub1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.kryptonSplitContainerSub1.IsSplitterFixed = true;
			this.kryptonSplitContainerSub1.Location = new System.Drawing.Point( 0, 0 );
			this.kryptonSplitContainerSub1.Name = "kryptonSplitContainerSub1";
			// 
			// kryptonSplitContainerSub1.Panel2
			// 
			this.kryptonSplitContainerSub1.Panel2.Controls.Add( this.engineScrollBarVertical );
			this.kryptonSplitContainerSub1.Panel2MinSize = 16;
			this.kryptonSplitContainerSub1.Size = new System.Drawing.Size( 574, 304 );
			this.kryptonSplitContainerSub1.SplitterDistance = 554;
			this.kryptonSplitContainerSub1.SplitterPercent = 0.96515679442508706D;
			this.kryptonSplitContainerSub1.SplitterWidth = 1;
			this.kryptonSplitContainerSub1.TabIndex = 0;
			// 
			// engineScrollBarVertical
			// 
			this.engineScrollBarVertical.Dock = System.Windows.Forms.DockStyle.Fill;
			this.engineScrollBarVertical.Location = new System.Drawing.Point( 0, 0 );
			this.engineScrollBarVertical.Name = "engineScrollBarVertical";
			this.engineScrollBarVertical.Size = new System.Drawing.Size( 19, 304 );
			this.engineScrollBarVertical.TabIndex = 0;
			// 
			// kryptonSplitContainerSub2
			// 
			this.kryptonSplitContainerSub2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonSplitContainerSub2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.kryptonSplitContainerSub2.IsSplitterFixed = true;
			this.kryptonSplitContainerSub2.Location = new System.Drawing.Point( 0, 0 );
			this.kryptonSplitContainerSub2.Name = "kryptonSplitContainerSub2";
			// 
			// kryptonSplitContainerSub2.Panel1
			// 
			this.kryptonSplitContainerSub2.Panel1.Controls.Add( this.engineScrollBarHorizontal );
			this.kryptonSplitContainerSub2.Panel2MinSize = 16;
			this.kryptonSplitContainerSub2.Size = new System.Drawing.Size( 574, 19 );
			this.kryptonSplitContainerSub2.SplitterDistance = 554;
			this.kryptonSplitContainerSub2.SplitterPercent = 0.96515679442508706D;
			this.kryptonSplitContainerSub2.SplitterWidth = 1;
			this.kryptonSplitContainerSub2.TabIndex = 0;
			// 
			// engineScrollBarHorizontal
			// 
			this.engineScrollBarHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.engineScrollBarHorizontal.Location = new System.Drawing.Point( 0, 0 );
			this.engineScrollBarHorizontal.MinimumSize = new System.Drawing.Size( 102, 0 );
			this.engineScrollBarHorizontal.Name = "engineScrollBarHorizontal";
			this.engineScrollBarHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.engineScrollBarHorizontal.Size = new System.Drawing.Size( 554, 19 );
			this.engineScrollBarHorizontal.TabIndex = 1;
			// 
			// ScriptEditorControl
			// 
			this.Controls.Add( this.kryptonSplitContainer );
			this.Name = "ScriptEditorControl";
			this.Size = new System.Drawing.Size( 574, 323 );
			this.Load += new System.EventHandler( this.ScriptEditorControl_Load );
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainer.Panel1 ) ).EndInit();
			this.kryptonSplitContainer.Panel1.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainer.Panel2 ) ).EndInit();
			this.kryptonSplitContainer.Panel2.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainer ) ).EndInit();
			this.kryptonSplitContainer.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainerSub1.Panel1 ) ).EndInit();
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainerSub1.Panel2 ) ).EndInit();
			this.kryptonSplitContainerSub1.Panel2.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainerSub1 ) ).EndInit();
			this.kryptonSplitContainerSub1.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainerSub2.Panel1 ) ).EndInit();
			this.kryptonSplitContainerSub2.Panel1.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainerSub2.Panel2 ) ).EndInit();
			( (System.ComponentModel.ISupportInitialize)( this.kryptonSplitContainerSub2 ) ).EndInit();
			this.kryptonSplitContainerSub2.ResumeLayout( false );
			this.ResumeLayout( false );

		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			if( displayLineNumbers != ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayLineNumbers )
			{
				displayLineNumbers = ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayLineNumbers;
				editor.ShowLineNumbers = displayLineNumbers;
			}

			if( wordWrap != ProjectSettings.Get.CSharpEditor.CSharpEditorWordWrap )
			{
				wordWrap = ProjectSettings.Get.CSharpEditor.CSharpEditorWordWrap;
				editor.WordWrap = wordWrap;
			}

			if( braceCompletion != ProjectSettings.Get.CSharpEditor.CSharpEditorBraceCompletion )
			{
				braceCompletion = ProjectSettings.Get.CSharpEditor.CSharpEditorBraceCompletion;
				editor.IsBraceCompletionEnabled = braceCompletion;
			}

			if( editor.DisplayInfoMarkers != ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayInfoMarkers )
			{
				editor.DisplayInfoMarkers = ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayInfoMarkers;
				editor.UpdateMarkers();
			}
			if( editor.DisplayWarningMarkers != ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayWarningMarkers )
			{
				editor.DisplayWarningMarkers = ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayWarningMarkers;
				editor.UpdateMarkers();
			}
			if( editor.DisplayErrorMarkers != ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayErrorMarkers )
			{
				editor.DisplayErrorMarkers = ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayErrorMarkers;
				editor.UpdateMarkers();
			}

			if( displayQuickActions != ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayQuickActions )
			{
				displayQuickActions = ProjectSettings.Get.CSharpEditor.CSharpEditorDisplayQuickActions;
				Editor.ContextActionsRenderer.ChangeIconImage( displayQuickActions ? Editor.ContextActionsIcon : null );
			}

			UpdateBackgroundForeground();
			UpdateFont();
			UpdateScrollBars();

			//UpdateHighlightingScheme();

			WinFormsUtility.InvalidateParentComposedStyleControl( this );
		}

		string Translate( string text )
		{
			return EditorLocalization2.Translate( "ScriptEditor", text );
		}

		public void TryShowRenameDialog()
		{
			var symbol = GetRenameSymbol();
			if( symbol != null )
			{
				var form = new OKCancelTextBoxForm( Translate( "Name" ) + ":", symbol.Name, Translate( "Rename Symbol" ),
					delegate ( string text, ref string error )
					{
						if( string.IsNullOrEmpty( text ) )
						{
							error = Translate( "The name is not specified." );
							return false;
						}

						var identifierRegex = new Regex( @"^(?:((?!\d)\w+(?:\.(?!\d)\w+)*)\.)?((?!\d)\w+)$" );
						if( !identifierRegex.IsMatch( text ) )
						{
							error = Translate( "Invalid symbol name." );
							return false;
						}

						return true;
					},
					delegate ( string text, ref string error )
					{
						if( text != symbol.Name )
							RenameSymbol( symbol, text );
						return true;
					}
				);

				form.ShowDialog();
			}
		}

		string TranslateContextMenu( string text )
		{
			return EditorContextMenuWinForms.Translate( text );
		}

		private void Editor_ContextMenuOpening( object sender, System.Windows.Controls.ContextMenuEventArgs e )
		{
			e.Handled = true;

			var items = new List<KryptonContextMenuItemBase>();

			//Rename
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Rename" ), null,
					delegate ( object s, EventArgs e2 )
					{
						TryShowRenameDialog();
					} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Rename" );
				item.Enabled = GetRenameSymbol() != null;
				items.Add( item );
			}

			//Quick Actions and Refactorings
			{
				var item = new KryptonContextMenuItem( Translate( "Quick Actions" ), null,
					delegate ( object s, EventArgs e2 )
					{
						Editor.ContextActionsRenderer?.ShowMenu();
					} );
				//item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Go To Definition" );
				item.Enabled = Editor.ContextActionsRenderer != null && Editor.ContextActionsRenderer.IsOpen;
				items.Add( item );
			}

			//separator
			items.Add( new KryptonContextMenuSeparator() );

			//Find
			{
				var item = new KryptonContextMenuItem( Translate( "Find and Replace" ), null,
					delegate ( object s, EventArgs e2 )
					{
						Editor.SearchReplacePanel?.ShowFindOrReplace( false );
					} );
				item.ShortcutKeyDisplayString = EditorActions.ConvertShortcutKeysToString( new Keys[] { Keys.Control | Keys.F } );
				//item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Find" );
				items.Add( item );
			}

			//Go To Definition
			{
				//xx xx;

				var item = new KryptonContextMenuItem( Translate( "Go To Definition" ), null,
					delegate ( object s, EventArgs e2 )
					{
						TryGoToDefinition();
					} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Go To Definition" );
				item.Enabled = CanGoToDefinition( out _ );
				items.Add( item );
			}

			//separator
			items.Add( new KryptonContextMenuSeparator() );

			//Cut
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Cut" ), EditorResourcesCache.Cut,
					delegate ( object s, EventArgs e2 )
					{
						editor.Cut();
					} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Cut" );
				item.Enabled = true;// CanCut();
				items.Add( item );
			}

			//Copy
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Copy" ), EditorResourcesCache.Copy,
					delegate ( object s, EventArgs e2 )
					{
						editor.Copy();
					} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Copy" );
				item.Enabled = true;// CanCopy();
				items.Add( item );
			}

			//Paste
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Paste" ), EditorResourcesCache.Paste,
					delegate ( object s, EventArgs e2 )
					{
						editor.Paste();
					} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Paste" );
				item.Enabled = true;// CanPaste( out _ );
				items.Add( item );
			}

			EditorContextMenuWinForms.AddActionsToMenu( EditorContextMenuWinForms.MenuTypeEnum.General, items );

			EditorContextMenuWinForms.Show( items, this );
		}

		void UpdateHighlightingScheme()
		{
			if( loadedHighlightingDefinition == null )
			{
				string path;
				if( EditorAPI2.DarkTheme )
					path = @"Base\Tools\Highlighting\CSharpDark.xshd";
				else
					path = @"Base\Tools\Highlighting\CSharpLight.xshd";

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
				editor.SyntaxHighlighting = loadedHighlightingDefinition;

			//var path = ProjectSettings.Get.CSharpEditor.CSharpEditorHighlightingSchemeLightTheme.Value;
			//if( currentHighlightingScheme != path )
			//{
			//	currentHighlightingScheme = path;

			//	try
			//	{

			//		if( !string.IsNullOrEmpty( path ) )
			//		{
			//			var fullPath = VirtualPathUtility.GetRealPathByVirtual( path );
			//			editor.SyntaxHighlighting = HighlightingManager.Instance.LoadFromFile( fullPath );

			//			//editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition( "C#" );
			//		}
			//		else
			//		{
			//			//!!!!check
			//			editor.SyntaxHighlighting = null;
			//		}

			//	}
			//	catch( Exception e )
			//	{
			//		Log.Warning( "Update highlighting scheme error: " + e.Message );
			//	}
			//}

		}

		void UpdateBackgroundForeground()
		{
			{
				var color = EditorAPI2.DarkTheme ? ProjectSettings.Get.CSharpEditor.CSharpEditorBackgroundColorDarkTheme.Value : ProjectSettings.Get.CSharpEditor.CSharpEditorBackgroundColorLightTheme.Value;
				if( backgroundColor != color )
				{
					backgroundColor = color;
					var packed = backgroundColor.ToColorPacked();
					editor.Background = new SolidColorBrush( Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
				}
			}

			{
				var color = EditorAPI2.DarkTheme ? ProjectSettings.Get.CSharpEditor.CSharpEditorSearchBackgroundDarkTheme.Value : ProjectSettings.Get.CSharpEditor.CSharpEditorSearchBackgroundLightTheme.Value;
				var packed = color.ToColorPacked();
				editor.TextArea.SearchBackgroundBrush = new SolidColorBrush( Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
			}

			//HighlightingColor defaultNamedColor = null;
			//if( loadedHighlightingDefinition != null )
			//	defaultNamedColor = loadedHighlightingDefinition.GetNamedColor( "Default" );

			//if( defaultNamedColor != null )
			//{
			//	editor.Foreground = new SolidColorBrush( Color.FromArgb( 255, 255, 0, 0 ) );

			//	//editor.Foreground = defaultNamedColor.Foreground.GetBrush( null );
			//}

			//{
			//	var color = EditorAPI.DarkTheme ? ProjectSettings.Get.CSharpEditor.CSharpEditorDefaultTextColorDarkTheme : ProjectSettings.Get.CSharpEditor.CSharpEditorDefaultTextColorLightTheme;
			//	if( textColor != color )
			//	{
			//		textColor = color;
			//		var packed = textColor.ToColorPacked();
			//		editor.Foreground = new SolidColorBrush( Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
			//	}
			//}
		}

		void UpdateFont()
		{
			if( currentFont != ProjectSettings.Get.CSharpEditor.CSharpEditorFont )
			{
				currentFont = ProjectSettings.Get.CSharpEditor.CSharpEditorFont;

				try
				{
					editor.FontFamily = new FontFamily( ProjectSettings.Get.CSharpEditor.CSharpEditorFont );
				}
				catch { }
			}

			if( currentFontSize != ProjectSettings.Get.CSharpEditor.CSharpEditorFontSize )
			{
				currentFontSize = ProjectSettings.Get.CSharpEditor.CSharpEditorFontSize;

				try
				{
					editor.FontSize = ProjectSettings.Get.CSharpEditor.CSharpEditorFontSize;
				}
				catch { }
			}

			var selectionBackground = EditorAPI2.DarkTheme ? ProjectSettings.Get.CSharpEditor.CSharpEditorSelectionBackgroundDarkTheme.Value : ProjectSettings.Get.CSharpEditor.CSharpEditorSelectionBackgroundLightTheme.Value;
			if( currentSelectionBackground != selectionBackground )
			{
				currentSelectionBackground = selectionBackground;

				try
				{
					var packed = selectionBackground.ToColorPacked();
					editor.TextArea.SelectionBrush = new SolidColorBrush( System.Windows.Media.Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
				}
				catch { }
			}

			var selectionForeground = EditorAPI2.DarkTheme ? ProjectSettings.Get.CSharpEditor.CSharpEditorSelectionForegroundDarkTheme.Value : ProjectSettings.Get.CSharpEditor.CSharpEditorSelectionForegroundLightTheme.Value;
			if( currentSelectionForeground != selectionForeground )
			{
				currentSelectionForeground = selectionForeground;

				try
				{
					var packed = selectionForeground.ToColorPacked();
					editor.TextArea.SelectionForeground = new SolidColorBrush( System.Windows.Media.Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
				}
				catch { }
			}

		}

		private void ScriptEditorControl_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			kryptonSplitContainer.Panel2MinSize = (int)( kryptonSplitContainer.Panel2MinSize * DpiHelper.Default.DpiScaleFactor );
			kryptonSplitContainer.SplitterDistance = 10000;
			kryptonSplitContainerSub1.Panel2MinSize = (int)( kryptonSplitContainerSub1.Panel2MinSize * DpiHelper.Default.DpiScaleFactor );
			kryptonSplitContainerSub1.SplitterDistance = 10000;
			kryptonSplitContainerSub2.Panel2MinSize = (int)( kryptonSplitContainerSub2.Panel2MinSize * DpiHelper.Default.DpiScaleFactor );
			kryptonSplitContainerSub2.SplitterDistance = 10000;

			engineScrollBarVertical.Scroll += EngineScrollBarVertical_Scroll;
			engineScrollBarHorizontal.Scroll += EngineScrollBarHorizontal_Scroll;

			if( EditorAPI2.DarkTheme )
			{
				kryptonSplitContainerSub1.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb( 40, 40, 40 );
				kryptonSplitContainerSub2.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb( 47, 47, 47 );
				kryptonSplitContainerSub2.Panel2.StateCommon.Color1 = System.Drawing.Color.FromArgb( 47, 47, 47 );
			}

			UpdateScrollBars();

			timer1.Start();
		}

		void UpdateScrollBars()
		{
			if( editor == null )
				return;

			editor.ApplyTemplate();
			var scrollViewer = editor.ScrollViewer;
			if( scrollViewer == null )
				return;

			bool updating1 = engineScrollBarVertical.MouseUpDownStatus;//&& engineScrollBar1.MouseScrollBarArea == EnhancedScrollBarMouseLocation.Thumb;
			if( !updating1 )
			{
				engineScrollBarVertical.Maximum = (int)scrollViewer.ScrollableHeight;
				engineScrollBarVertical.SmallChange = 30;
				engineScrollBarVertical.LargeChange = Height;
				engineScrollBarVertical.Value = (int)scrollViewer.VerticalOffset;
			}

			bool updating2 = engineScrollBarHorizontal.MouseUpDownStatus;//&& engineScrollBar2.MouseScrollBarArea == EnhancedScrollBarMouseLocation.Thumb;
			if( !updating2 )
			{
				engineScrollBarHorizontal.Maximum = (int)scrollViewer.ScrollableWidth;
				engineScrollBarHorizontal.SmallChange = 30;
				engineScrollBarHorizontal.LargeChange = Height;
				engineScrollBarHorizontal.Value = (int)scrollViewer.HorizontalOffset;
			}

			//!!!!workaround
			kryptonSplitContainer.Panel2Collapsed = Editor.WordWrap;
			//kryptonSplitContainer.Panel2Collapsed = scrollViewer.ScrollableWidth == 0;

			kryptonSplitContainerSub1.Panel2Collapsed = scrollViewer.ScrollableHeight == 0;
			kryptonSplitContainerSub2.Panel2Collapsed = scrollViewer.ScrollableHeight == 0;
		}

		private void EngineScrollBarVertical_Scroll( object sender, EngineScrollBarEventArgs e )
		{
			editor.ScrollViewer?.ScrollToVerticalOffset( engineScrollBarVertical.Value );
		}

		private void EngineScrollBarHorizontal_Scroll( object sender, EngineScrollBarEventArgs e )
		{
			editor.ScrollViewer?.ScrollToHorizontalOffset( engineScrollBarHorizontal.Value );
		}

		public void AddCodeToCurrentPosition( string code )
		{
			var document = GetDocument();
			if( document == null )
				return;

			var documentText = document.GetTextAsync().Result;
			var changes = new List<TextChange>();

			changes.Add( new TextChange( new TextSpan( editor.CaretOffset, 0 ), code ) );

			if( changes.Count == 0 )
				return;

			RoslynHost.Instance.UpdateDocument( document.WithText( documentText.WithChanges( changes ) ) );
		}

		public ISymbol GetGoToDefinitionSymbol()
		{
			var document = GetDocument();
			if( document == null )
				return null;
			return GoToDefinitionHelper.GetGoToDefinitionSymbol( document, editor.SelectionStart ).Result;
		}

		public bool CanGoToDefinition( out ISymbol symbol )
		{
			var document = GetDocument();
			if( document == null )
			{
				symbol = null;
				return false;
			}

			symbol = null;
			try
			{
				symbol = GetGoToDefinitionSymbol();
			}
			catch( Exception e )
			{
				Log.Warning( e.Message );
			}

			return symbol != null;
		}

		static bool IsRootNamespace( ISymbol symbol )
		{
			INamespaceSymbol s = null;
			return ( ( s = symbol as INamespaceSymbol ) != null ) && s.IsGlobalNamespace;
		}

		static string GetFullMetadataName( ISymbol s )
		{
			if( s == null || IsRootNamespace( s ) )
				return string.Empty;

			var sb = new StringBuilder( s.MetadataName );
			var last = s;

			s = s.ContainingSymbol;

			while( !IsRootNamespace( s ) )
			{
				if( s is ITypeSymbol && last is ITypeSymbol )
					sb.Insert( 0, '+' );
				else
					sb.Insert( 0, '.' );

				sb.Insert( 0, s.OriginalDefinition.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) );
				//sb.Insert(0, s.MetadataName);
				s = s.ContainingSymbol;
			}

			return sb.ToString();
		}

		public void TryGoToDefinition()
		{
			if( CanGoToDefinition( out var symbol ) )
			{
				var document = GetDocument();
				if( document == null )
					return;

				foreach( var location in symbol.Locations )
				{
					if( location.IsInSource )
					{
						var isScript = Parent != null && Parent is CSharpScriptEditor;
						if( isScript )
						{
							//CSharpScriptEditor

							var documentWindow = (CSharpScriptEditor)Parent;

							var scriptEditorControl = documentWindow.ScriptEditorControl;
							var editor2 = scriptEditorControl.Editor;

							bool skip = false;
							if( document.Project.IsSubmission )
							{
								var solution = document.Project.Solution;
								var projectId = solution.GetDocument( location.SourceTree ).Project.Id;
								if( solution.Projects.Any( p => p.IsSubmission && p.ProjectReferences.Any( r => r.ProjectId == projectId ) ) )
									skip = true;
							}

							if( !skip )
							{
								try
								{
									editor2.CaretOffset = location.SourceSpan.Start;
									editor2.Select( location.SourceSpan.Start, location.SourceSpan.Length );

									var text = editor2.Document.Text;
									if( text.Length >= location.SourceSpan.Start )
									{
										var text2 = text.Substring( 0, location.SourceSpan.Start );
										int line = text2.Count( f => f == '\n' ) + 1;
										editor2.ScrollToLine( line );
									}

									//location.GetLineSpan()
								}
								catch( Exception e )
								{
									Log.Warning( e.Message );
								}

								return;
							}
						}
						else
						{
							//CSharpDocumentWindow

							var path = location.SourceTree.FilePath;
							if( File.Exists( path ) )
							{
								var documentWindow = EditorAPI2.OpenFileAsDocument( path, true, true ) as CSharpDocumentWindow;
								if( documentWindow != null )
								{
									var scriptEditorControl = documentWindow.ScriptEditorControl;
									var editor2 = scriptEditorControl.Editor;

									bool skip = false;
									if( document.Project.IsSubmission )
									{
										var solution = document.Project.Solution;
										var projectId = solution.GetDocument( location.SourceTree ).Project.Id;
										if( solution.Projects.Any( p => p.IsSubmission && p.ProjectReferences.Any( r => r.ProjectId == projectId ) ) )
											skip = true;
									}

									if( !skip )
									{
										documentWindow.NeedSelectAndScrollToSpan = location.SourceSpan;

										//try
										//{
										//	editor2.CaretOffset = location.SourceSpan.Start;
										//	editor2.Select( location.SourceSpan.Start, location.SourceSpan.Length );

										//	var text = editor2.Document.Text;
										//	if( text.Length >= location.SourceSpan.Start )
										//	{
										//		var text2 = text.Substring( 0, location.SourceSpan.Start );
										//		int line = text2.Count( f => f == '\n' ) + 1;
										//		editor2.ScrollToLine( line );
										//	}

										//	//location.GetLineSpan()
										//}
										//catch( Exception e )
										//{
										//	Log.Warning( e.Message );
										//}

										return;
									}
								}
							}
						}
					}
				}

				//open metadata

				var typeName = "";
				Metadata.TypeInfo engineType = null;

				var fieldSymbol = symbol as IFieldSymbol;
				var propertySymbol = symbol as IPropertySymbol;
				var methodSymbol = symbol as IMethodSymbol;
				var eventSymbol = symbol as IEventSymbol;

				if( fieldSymbol != null )
				{
					typeName = GetFullMetadataName( fieldSymbol.ContainingType );
					engineType = MetadataManager.GetType( typeName );
				}
				else if( propertySymbol != null )
				{
					typeName = GetFullMetadataName( propertySymbol.ContainingType );
					engineType = MetadataManager.GetType( typeName );
				}
				else if( methodSymbol != null )
				{
					typeName = GetFullMetadataName( methodSymbol.ContainingType );
					engineType = MetadataManager.GetType( typeName );
				}
				else if( eventSymbol != null )
				{
					typeName = GetFullMetadataName( eventSymbol.ContainingType );
					engineType = MetadataManager.GetType( typeName );
				}
				else
				{
					typeName = GetFullMetadataName( symbol );
					engineType = MetadataManager.GetType( typeName );
				}

				if( engineType != null && engineType.GetNetType() != null )
				{
					var type = engineType.GetNetType();

					//!!!!по идее для поиска строки лучше метки ставить типа как CodeAnalysis. т.е. оно само потом найдет

					var metadataText = ScriptEditorUtility.GetMetadataText( type, symbol, out var selectLine );

					//Log.Info( selectLine );

					EditorAPI2.OpenTextAsDocument( metadataText, engineType.Name + " [from metadata]", true, true, "CSharp", selectLine );

					//EditorAPI.OpenTextAsDocument( metadataText, engineType.Name + " [from metadata]", true, true, "CSharp", selectLine.Minimum, selectLine.Maximum );
				}
				else
					Log.Warning( $"A type with name \'{typeName}\' is not found." );
			}
		}

	}
}
