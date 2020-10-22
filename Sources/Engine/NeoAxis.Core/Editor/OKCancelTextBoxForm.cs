// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class OKCancelTextBoxForm : EngineForm
	{
		public delegate bool CheckDelegate( string text, ref string error );
		CheckDelegate checkHandler;

		public delegate bool OKDelegate( string text, ref string error );
		OKDelegate okHandler;

		bool loaded;

		//

		public OKCancelTextBoxForm( string labelText, string textBoxText, string caption, CheckDelegate checkHandler, OKDelegate okHandler )
		{
			this.checkHandler = checkHandler;
			this.okHandler = okHandler;

			InitializeComponent();

			this.labelText.Text = labelText;
			textBoxName.Text = textBoxText;

			if( string.IsNullOrEmpty( caption ) )
				Text = EngineInfo.NameWithVersion;
			else
				Text = caption;

			labelError.Text = "";

			EditorThemeUtility.ApplyDarkThemeToForm( this );

			buttonOK.Text = EditorLocalization.Translate( "General", buttonOK.Text );
			buttonCancel.Text = EditorLocalization.Translate( "General", buttonCancel.Text );
		}

		private void OKCancelTextBoxForm_Load( object sender, EventArgs e )
		{
			loaded = true;

			UpdateControls();

			//Translate();
		}

		public string TextBoxText
		{
			get { return textBoxName.Text; }
		}

		private void textBoxName_TextChanged( object sender, EventArgs e )
		{
			if( !loaded )
				return;

			string error = "";
			if( checkHandler != null && !checkHandler( TextBoxText, ref error ) )
			{
				labelError.Text = error;
				buttonOK.Enabled = false;
			}
			else
			{
				labelError.Text = "";
				buttonOK.Enabled = true;
			}
		}

		private void RenameResourceDialog_FormClosing( object sender, FormClosingEventArgs e )
		{
			if( DialogResult == DialogResult.OK )
			{
				string error = "";
				if( okHandler != null && !okHandler( TextBoxText, ref error ) )
				{
					textBoxName.Focus();
					e.Cancel = true;
					labelError.Text = error;
					return;
				}
			}
		}

		void UpdateControls()
		{
			buttonCancel.Location = new Point( ClientSize.Width - buttonCancel.Size.Width - DpiHelper.Default.ScaleValue( 12 ), ClientSize.Height - buttonCancel.Size.Height - DpiHelper.Default.ScaleValue( 12 ) );
			buttonOK.Location = new Point( buttonCancel.Location.X - buttonOK.Size.Width - DpiHelper.Default.ScaleValue( 8 ), buttonCancel.Location.Y );
			textBoxName.Width = ClientSize.Width - textBoxName.Location.X - DpiHelper.Default.ScaleValue( 12 );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( IsHandleCreated )
				UpdateControls();
		}

		//void Translate()
		//{
		//	buttonOK.Text = ToolsLocalization.Translate( "OKCancelTextBoxDialog", buttonOK.Text );
		//	buttonCancel.Text = ToolsLocalization.Translate( "OKCancelTextBoxDialog", buttonCancel.Text );
		//}

		//public void UpdateFonts( string fontForm )
		//{
		//	if( !string.IsNullOrEmpty( fontForm ) && fontForm[ 0 ] != '(' )
		//	{
		//		try
		//		{
		//			System.Drawing.FontConverter fontConverter = new System.Drawing.FontConverter();
		//			Font = (System.Drawing.Font)fontConverter.ConvertFromString( fontForm );
		//		}
		//		catch { }
		//	}
		//}
	}
}