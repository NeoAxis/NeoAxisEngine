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

		EditorAssemblyInterface.ITextEditorControl kryptonRichTextBox1;

		//

		public OutputWindow()
		{
			InitializeComponent();

			{
				kryptonRichTextBox1 = EditorAssemblyInterface.Instance.CreateTextEditorControl();
				var control = (Control)this.kryptonRichTextBox1;

				this.Controls.Add( control );
				//control.Dock = System.Windows.Forms.DockStyle.Fill;
				//control.Location = new System.Drawing.Point( 0, 27 );
				//control.Location = new System.Drawing.Point( 0, 25 );
				control.Name = "kryptonRichTextBox1";
				this.kryptonRichTextBox1.EditorReadOnly = true;
				//control.Size = new System.Drawing.Size( 713, 165 );
				//!!!!
				//this.kryptonRichTextBox1.StateCommon.Content.Font = new System.Drawing.Font( "Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 204 ) ) );
				control.TabIndex = 2;
				this.kryptonRichTextBox1.EditorText = "";
				this.kryptonRichTextBox1.EditorWordWrap = true;//false;

				kryptonRichTextBox1.Border = true;

				UpdateTextEditorBounds();
			}

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

		private void richTextBox1_PreviewKeyDown( object sender, PreviewKeyDownEventArgs e )
		{
			//!!!!!!было в старом
			//if( e.KeyCode == Keys.F4 && e.Control )
			//	Hide();
		}

		private void OutputWindow_Load( object sender, EventArgs e )
		{
			toolStrip1.Padding = new Padding( (int)EditorAPI.DPIScale );
			toolStrip1.Size = new Size( 10, (int)( 21 * EditorAPI.DPIScale + 2 ) );
			toolStripButtonOptions.Size = new Size( (int)( 20 * EditorAPI.DPIScale ), (int)( 20 * EditorAPI.DPIScale + 2 ) );
			toolStripButtonClear.Size = new Size( (int)( 20 * EditorAPI.DPIScale ), (int)( 20 * EditorAPI.DPIScale + 2 ) );

			Translate();

			UpdateTextEditorBounds();

			var control = (Control)kryptonRichTextBox1;
			control.Select();
			//kryptonRichTextBox1.Select();
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

		void PrintInstance( string text )
		{
			kryptonRichTextBox1.EditorText += text;

			kryptonRichTextBox1.Select( kryptonRichTextBox1.EditorText.Length, 0 );
			kryptonRichTextBox1.ScrollToEnd();
			//kryptonRichTextBox1.ScrollToCaret();
		}

		public static void Print( string text )
		{
			var window = EditorAPI.FindWindow<OutputWindow>();
			window?.PrintInstance( text );
		}

		void ClearInstance()
		{
			kryptonRichTextBox1.EditorText = "";

			kryptonRichTextBox1.Select( 0, 0 );
			kryptonRichTextBox1.ScrollToHome();
			//kryptonRichTextBox1.ScrollToCaret();
		}

		public static void Clear()
		{
			var window = EditorAPI.FindWindow<OutputWindow>();
			window?.ClearInstance();
		}

		private void toolStripButtonClear_Click( object sender, EventArgs e )
		{
			ClearInstance();
		}

		void UpdateTextEditorBounds()
		{
			var control = (Control)kryptonRichTextBox1;
			control?.SetBounds( 0, toolStrip1.Height, ClientSize.Width, ClientSize.Height - toolStrip1.Height );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			UpdateTextEditorBounds();
		}

	}
}
