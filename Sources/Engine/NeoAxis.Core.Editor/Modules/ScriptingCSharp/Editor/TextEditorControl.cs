// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using ComponentFactory.Krypton.Toolkit;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;
using RoslynPad.Editor;

namespace NeoAxis.Editor
{
	public class TextEditorControl : UserControl
	{
		private KryptonSplitContainer kryptonSplitContainer;
		private KryptonSplitContainer kryptonSplitContainerSub1;
		private EngineScrollBar engineScrollBarVertical;
		private KryptonSplitContainer kryptonSplitContainerSub2;
		private EngineScrollBar engineScrollBarHorizontal;
		private Timer timer1;
		private IContainer components;
		TextEditor editor;

		SearchReplacePanel searchReplacePanel;

		//

		public TextEditorControl()
		{
			InitializeComponent();

			editor = new TextEditor();

			//editor.TextArea.DefaultInputHandler.NestedInputHandlers.Add( new SearchInputHandler( editor.TextArea ) );

			editor.TextArea.SelectionBorder = null;
			editor.TextArea.SelectionCornerRadius = 0;

			editor.FontFamily = new System.Windows.Media.FontFamily( "Consolas" );
			//if( ScriptEditorEngine.Instance != null )
			//	editor.FontSize = ScriptEditorEngine.Instance.EditorFontSize;

			editor.ContextMenuOpening += Editor_ContextMenuOpening;

			var codeEditorHost = new ElementHost();
			codeEditorHost.Dock = DockStyle.Fill;
			codeEditorHost.Child = editor;
			kryptonSplitContainerSub1.Panel1.Controls.Add( codeEditorHost );
		}

		[Browsable( false )]
		public TextEditor Editor
		{
			get { return editor; }
		}

		string TranslateContextMenu( string text )
		{
			return EditorContextMenu.Translate( text );
		}

		private void Editor_ContextMenuOpening( object sender, System.Windows.Controls.ContextMenuEventArgs e )
		{
			e.Handled = true;

			var items = new List<KryptonContextMenuItemBase>();

			//Find
			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Find and Replace" ), null,
					delegate ( object s, EventArgs e2 )
					{
						SearchReplacePanel?.ShowFindOrReplace( false );
					} );
				item.ShortcutKeyDisplayString = EditorActions.ConvertShortcutKeysToString( new Keys[] { Keys.Control | Keys.F } );
				//item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Find" );
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
				item.Enabled = true;// editor.CanCut();
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

			EditorContextMenu.AddActionsToMenu( EditorContextMenu.MenuTypeEnum.General, items );

			EditorContextMenu.Show( items, this );
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.kryptonSplitContainer = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.kryptonSplitContainerSub1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.engineScrollBarVertical = new NeoAxis.Editor.EngineScrollBar();
			this.kryptonSplitContainerSub2 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.engineScrollBarHorizontal = new NeoAxis.Editor.EngineScrollBar();
			this.timer1 = new System.Windows.Forms.Timer( this.components );
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
			this.kryptonSplitContainer.Size = new System.Drawing.Size( 569, 314 );
			this.kryptonSplitContainer.SplitterDistance = 295;
			this.kryptonSplitContainer.SplitterPercent = 0.93949044585987262D;
			this.kryptonSplitContainer.SplitterWidth = 0;
			this.kryptonSplitContainer.TabIndex = 3;
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
			this.kryptonSplitContainerSub1.Size = new System.Drawing.Size( 569, 295 );
			this.kryptonSplitContainerSub1.SplitterDistance = 549;
			this.kryptonSplitContainerSub1.SplitterPercent = 0.96485061511423553D;
			this.kryptonSplitContainerSub1.SplitterWidth = 1;
			this.kryptonSplitContainerSub1.TabIndex = 0;
			// 
			// engineScrollBarVertical
			// 
			this.engineScrollBarVertical.Dock = System.Windows.Forms.DockStyle.Fill;
			this.engineScrollBarVertical.Location = new System.Drawing.Point( 0, 0 );
			this.engineScrollBarVertical.Name = "engineScrollBarVertical";
			this.engineScrollBarVertical.Size = new System.Drawing.Size( 19, 295 );
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
			this.kryptonSplitContainerSub2.Size = new System.Drawing.Size( 569, 19 );
			this.kryptonSplitContainerSub2.SplitterDistance = 549;
			this.kryptonSplitContainerSub2.SplitterPercent = 0.96485061511423553D;
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
			this.engineScrollBarHorizontal.Size = new System.Drawing.Size( 549, 19 );
			this.engineScrollBarHorizontal.TabIndex = 1;
			// 
			// timer1
			// 
			this.timer1.Interval = 25;
			this.timer1.Tick += new System.EventHandler( this.timer1_Tick );
			// 
			// TextEditorControl
			// 
			this.Controls.Add( this.kryptonSplitContainer );
			this.Name = "TextEditorControl";
			this.Size = new System.Drawing.Size( 569, 314 );
			this.Load += new System.EventHandler( this.TextEditorControl_Load );
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

		private void TextEditorControl_Load( object sender, EventArgs e )
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

			if( EditorAPI.DarkTheme )
			{
				kryptonSplitContainerSub1.StateCommon.Back.Color1 = Color.FromArgb( 40, 40, 40 );
				kryptonSplitContainerSub2.StateCommon.Back.Color1 = Color.FromArgb( 47, 47, 47 );
				kryptonSplitContainerSub2.Panel2.StateCommon.Color1 = Color.FromArgb( 47, 47, 47 );
			}

			UpdateScrollBars();

			timer1.Start();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			UpdateScrollBars();
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

		public void InstallSearchReplacePanel()
		{
			searchReplacePanel = SearchReplacePanel.Install( Editor.TextArea );
		}

		[Browsable( false )]
		public SearchReplacePanel SearchReplacePanel
		{
			get { return searchReplacePanel; }
		}

	}
}
