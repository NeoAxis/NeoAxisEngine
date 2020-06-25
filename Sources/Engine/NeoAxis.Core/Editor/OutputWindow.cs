// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class OutputWindow : DockWindow
	{
		//!!!!
		//System.Drawing.Font richTextBox1FontOriginal;
		//string richTextBox1FontCurrent = "";

		//!!!!не заюзано
		public delegate void ProcessCmdKeyEventDelegate( OutputWindow sender, ref Message msg, Keys keyData, ref bool handled );
		public event ProcessCmdKeyEventDelegate ProcessCmdKeyEvent;

		//

		public OutputWindow()
		{
			InitializeComponent();

			toolStripButtonOptions.Image = EditorResourcesCache.Options;
			toolStripButtonClear.Image = EditorResourcesCache.Delete;

			//Log.Handlers.InfoHandler += Handlers_InfoHandler;
			////Log.Handlers.InvisibleInfoHandler += Handlers_InvisibleInfoHandler;
			//Log.Handlers.WarningHandler += Handlers_WarningHandler;
			//Log.Handlers.ErrorHandler += Handlers_ErrorHandler;

			WindowTitle = EditorLocalization.Translate( "Windows", WindowTitle );
			toolStripButtonOptions.Text = EditorLocalization.Translate( "OutputWindow", toolStripButtonOptions.Text );
			toolStripButtonClear.Text = EditorLocalization.Translate( "OutputWindow", toolStripButtonClear.Text );

			if( EditorAPI.DarkTheme )
				toolStrip1.Renderer = DarkThemeUtility.GetToolbarToolStripRenderer();
		}

		public override bool HideOnRemoving { get { return true; } }

		public void Clear()
		{
			kryptonRichTextBox1.Text = "";
		}

		private void richTextBox1_PreviewKeyDown( object sender, PreviewKeyDownEventArgs e )
		{
			//!!!!!!было в старом
			//if( e.KeyCode == Keys.F4 && e.Control )
			//	Hide();
		}

		private void OutputForm_Load( object sender, EventArgs e )
		{
			Translate();

			kryptonRichTextBox1.Select();
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

		public void Print( string text )
		{
			//!!!!что с переносами?

			kryptonRichTextBox1.Text += text;

			kryptonRichTextBox1.SelectionStart = kryptonRichTextBox1.Text.Length;
			kryptonRichTextBox1.SelectionLength = 0;
			kryptonRichTextBox1.ScrollToCaret();
		}

		private void toolStripButtonClear_Click( object sender, EventArgs e )
		{
			kryptonRichTextBox1.Text = "";
			kryptonRichTextBox1.SelectionStart = 0;
			kryptonRichTextBox1.SelectionLength = 0;
			kryptonRichTextBox1.ScrollToCaret();
		}
	}
}
