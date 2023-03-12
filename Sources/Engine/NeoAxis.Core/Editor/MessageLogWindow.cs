#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Docking;
using Internal.ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Message Log Window.
	/// </summary>
	public partial class MessageLogWindow : DockWindow
	{
		const int maxItemCount = 1000;

		const string WindowCaption = "Message Log";
		LogType captionIconType = LogType.Info;
		int warnAndErrorCount;

		MessageLogOptions options = new MessageLogOptions();
		int splitterDistanceFromConfig = -1;

		//!!!!
		//System.Drawing.Font richTextBox1FontOriginal;
		//string richTextBox1FontCurrent = "";

		//!!!!не заюзано
		public delegate void ProcessCmdKeyEventDelegate( MessageLogWindow sender, ref Message msg, Keys keyData, ref bool handled );
		public event ProcessCmdKeyEventDelegate ProcessCmdKeyEvent;

		EditorAssemblyInterface.ITextEditorControl kryptonRichTextBox1;

		/////////////////////////////////////////

		class ContentBrowserItem : ContentBrowserItem_Virtual
		{
			public LogType type;

			public ContentBrowserItem( ContentBrowser owner, ContentBrowser.Item parent, string text )
				: base( owner, parent, text )
			{
			}
		}

		/////////////////////////////////////////

		public enum LogType
		{
			//InvisibleInfo,
			Info,
			Warning,
			Error
		}

		public MessageLogWindow()
		{
			InitializeComponent();

			//kryptonRichTextBox1
			{
				kryptonRichTextBox1 = EditorAssemblyInterface.Instance.CreateTextEditorControl();
				var control = (Control)this.kryptonRichTextBox1;

				this.kryptonSplitContainer1.Panel2.Controls.Add( control );
				control.Dock = System.Windows.Forms.DockStyle.Fill;
				control.Location = new System.Drawing.Point( 0, 0 );
				control.Name = "kryptonRichTextBox1";
				this.kryptonRichTextBox1.EditorReadOnly = true;
				control.Size = new System.Drawing.Size( 363, 165 );
				//!!!!
				//this.kryptonRichTextBox1.StateCommon.Content.Font = new System.Drawing.Font( "Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 204 ) ) );
				control.TabIndex = 2;
				this.kryptonRichTextBox1.EditorText = "";
				this.kryptonRichTextBox1.EditorWordWrap = true;//false;

				kryptonRichTextBox1.Border = true;
			}

			toolStripButtonClear.Image = EditorResourcesCache.Delete;
			toolStripButtonOptions.Image = EditorResourcesCache.Options;

			Log.Handlers.InfoHandler += Handlers_InfoHandler;
			//Log.Handlers.InvisibleInfoHandler += Handlers_InvisibleInfoHandler;
			Log.Handlers.WarningHandler += Handlers_WarningHandler;
			Log.Handlers.ErrorHandler += Handlers_ErrorHandler;

			//configure list
			contentBrowser1.SetData( new ContentBrowser.Item[ 0 ], false );
			contentBrowser1.AddImageKey( "Info", Properties.Resources.Info_16, Properties.Resources.Info_32 );
			contentBrowser1.AddImageKey( "Warning", Properties.Resources.Warning_16, Properties.Resources.Warning_32 );
			contentBrowser1.AddImageKey( "Error", Properties.Resources.Error_16, Properties.Resources.Error_32 );

			Config_Load();
			EngineConfig.SaveEvent += Config_SaveEvent;

			WindowTitle = EditorLocalization.Translate( "Windows", WindowTitle );
			toolStripButtonOptions.Text = EditorLocalization.Translate( "MessageLogWindow", toolStripButtonOptions.Text );
			toolStripButtonClear.Text = EditorLocalization.Translate( "MessageLogWindow", toolStripButtonClear.Text );

			toolStrip1.Renderer = EditorThemeUtility.GetToolbarToolStripRenderer();
		}

		public override bool HideOnRemoving { get { return true; } }

		public void Clear()
		{
			kryptonRichTextBox1.EditorText = "";
		}

		private void richTextBox1_PreviewKeyDown( object sender, PreviewKeyDownEventArgs e )
		{
			//!!!!!!было в старом
			//if( e.KeyCode == Keys.F4 && e.Control )
			//	Hide();
		}

		void Translate()
		{
			//!!!!
			//TabText = ToolsLocalization.Translate( "OutputForm", TabText );
		}

		//!!!!не заюзано
		//public void UpdateFonts( string fontForm )
		//{
		//	if( richTextBox1FontOriginal == null )
		//		richTextBox1FontOriginal = richTextBox1.Font;

		//	if( richTextBox1FontCurrent != fontForm )
		//	{
		//		if( !string.IsNullOrEmpty( fontForm ) && fontForm[ 0 ] != '(' )
		//		{
		//			try
		//			{
		//				System.Drawing.FontConverter fontConverter = new System.Drawing.FontConverter();
		//				richTextBox1.Font = (System.Drawing.Font)fontConverter.ConvertFromString( fontForm );
		//			}
		//			catch { }
		//		}
		//		else
		//			richTextBox1.Font = richTextBox1FontOriginal;

		//		richTextBox1FontCurrent = fontForm;
		//	}
		//}

		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( ProcessCmdKeyEvent != null )
			{
				bool handled = false;
				ProcessCmdKeyEvent( this, ref msg, keyData, ref handled );
				if( handled )
					return true;
			}

			return base.ProcessCmdKey( ref msg, keyData );
		}

		//private void Handlers_InvisibleInfoHandler( string text, ref bool dumpToLogFile )
		//{
		//	//!!!!
		//	//if( !IsDisposed )
		//	//	Print( text, LogType.InvisibleInfo );
		//}

		private void Handlers_InfoHandler( string text, ref bool dumpToLogFile )
		{
			if( IsDisposed )
				return;

			Print( text, LogType.Info );
		}

		private void Handlers_WarningHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			if( IsDisposed )
				return;

			Print( text, LogType.Warning );
			handled = true;
			//!!!!?
			//dumpToLogFile = false;
		}

		private void Handlers_ErrorHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			if( IsDisposed )
				return;

			Print( text, LogType.Error );
			handled = true;
			//!!!!?
			//dumpToLogFile = false;

			//show screen notification
			{
				var t = text;
				var index = text.IndexOfAny( new char[] { '\r', '\n' } );
				if( index != -1 )
					t = t.Substring( 0, index );
				ScreenNotifications.Show( t, true );
			}
		}

		public delegate void PrintFilterDelegate( string text, LogType type, ref bool skip );
		public static event PrintFilterDelegate PrintFilter;

		public void Print( string text, LogType type )
		{
			bool skip = false;
			PrintFilter?.Invoke( text, type, ref skip );
			if( skip )
				return;

			//max item count
			while( contentBrowser1.RootItems.Count >= maxItemCount )
				contentBrowser1.RemoveItem( contentBrowser1.RootItems[ 0 ] );

			var trimmedText = text.Replace( "\r\n", " " ).Replace( "\r", " " ).Replace( "\n", "" ).Trim();
			var item = new ContentBrowserItem( contentBrowser1, null, trimmedText );
			item.type = type;
			item.Tag = text;
			item.imageKey = type.ToString();
			contentBrowser1.AddRootItem( item );
			contentBrowser1.SelectItems( new ContentBrowser.Item[] { item } );

			//calculate captionIconType, warnAndErrorCount
			{
				captionIconType = LogType.Info;
				warnAndErrorCount = 0;
				foreach( ContentBrowserItem item2 in contentBrowser1.RootItems )
				{
					if( item2.type > captionIconType )
						captionIconType = type;
					if( item2.type != LogType.Info )
						warnAndErrorCount++;
				}
			}

			UpdateCaption();

			if( type == LogType.Error )
			{
				//!!!!или показывать заголовок окна типа как уведомление
				EditorAPI.ShowDockWindow<MessageLogWindow>();

				//!!!!падает
				//EditorForm.Instance.SelectDockWindow( EditorAPI.FindWindow<MessageLogWindow>() );
			}
		}

		private void toolStripButtonClear_Click( object sender, EventArgs e )
		{
			captionIconType = LogType.Info;
			warnAndErrorCount = 0;
			contentBrowser1.SetData( new ContentBrowser.Item[ 0 ], false );
			UpdateSecondPanel();
			UpdateCaption();
		}

		private void contentBrowser1_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			UpdateSecondPanel();
		}

		void UpdateSecondPanel()
		{
			if( contentBrowser1.SelectedItems.Length == 1 )
				kryptonRichTextBox1.EditorText = (string)contentBrowser1.SelectedItems[ 0 ].Tag;
			else
				kryptonRichTextBox1.EditorText = "";

			kryptonRichTextBox1.Select( 0, 0 );
			//kryptonRichTextBox1.ScrollToCaret();
		}

		Image GetImage( LogType type )
		{
			if( type == LogType.Warning )
				return EditorResourcesCache.Warning;
			else if( type == LogType.Error )
				return EditorResourcesCache.Error;
			else
				return null;
		}

		void UpdateCaption()
		{
			if( captionIconType == LogType.Info )
				KryptonPage.ImageSmall = null;
			else
				KryptonPage.ImageSmall = GetImage( captionIconType );

			if( warnAndErrorCount > 0 )
				KryptonPage.Text = EditorLocalization.Translate( "Windows", WindowCaption ) + $" ({warnAndErrorCount})";
			else
				KryptonPage.Text = EditorLocalization.Translate( "Windows", WindowCaption );

			// force repaint caption for auto hidden page.
			EditorForm.Instance.WorkspaceController.RepaintAutoHiddenWindow( this );
		}

		void SaveSettings( TextBlock block )
		{
			options.Save( block );

			block.SetAttribute( "SplitterDistance", kryptonSplitContainer1.SplitterDistance.ToString() );
		}

		void Config_Load()
		{
			var block = EngineConfig.TextBlock.FindChild( nameof( MessageLogWindow ) );
			if( block != null )
			{
				options.Load( block );

				if( block.AttributeExists( "SplitterDistance" ) )
					splitterDistanceFromConfig = int.Parse( block.GetAttribute( "SplitterDistance" ) );
			}
		}

		void Config_SaveEvent()
		{
			var configBlock = EngineConfig.TextBlock;

			var old = configBlock.FindChild( nameof( MessageLogWindow ) );
			if( old != null )
				configBlock.DeleteChild( old );

			var block = configBlock.AddChild( nameof( MessageLogWindow ) );

			options.Save( block );
			block.SetAttribute( "SplitterDistance", kryptonSplitContainer1.SplitterDistance.ToString() );
		}

		private void MessageLogWindow_Load( object sender, EventArgs e )
		{
			toolStrip1.Padding = new Padding( (int)EditorAPI.DPIScale );
			toolStrip1.Size = new Size( 10, (int)( 21 * EditorAPI.DPIScale + 2 ) );
			toolStripButtonOptions.Size = new Size( (int)( 20 * EditorAPI.DPIScale ), (int)( 20 * EditorAPI.DPIScale + 2 ) );
			toolStripButtonClear.Size = new Size( (int)( 20 * EditorAPI.DPIScale ), (int)( 20 * EditorAPI.DPIScale + 2 ) );

			Translate();

			var control = (Control)kryptonRichTextBox1;
			control.Select();
			//kryptonRichTextBox1.Select();

			timer1.Start();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
				return;

			if( kryptonSplitContainer1.Orientation != options.SplitterOrientation )
				kryptonSplitContainer1.Orientation = options.SplitterOrientation;

			//set splitter position from config
			if( splitterDistanceFromConfig != -1 )
			{
				kryptonSplitContainer1.SplitterDistance = splitterDistanceFromConfig;
				splitterDistanceFromConfig = -1;
			}
		}

		private void toolStripButtonOptions_Click( object sender, EventArgs e )
		{
			var form = new MessageLogOptionsForm();
			form.Options = options;
			EditorForm.Instance.WorkspaceController.BlockAutoHideAndDoAction( this, () =>
			 {
				 form.ShowDialog();
			 } );
		}
	}
}

#endif