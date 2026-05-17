// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public partial class ChatPromptForm : EngineForm
	{
		public ChatPromptForm( ChatWindow.ChatModeEnum mode, IDocumentWindow documentWindow, Component[] selectedObjects, string fullPath )
		{
			InitializeComponent();

			if( mode == ChatWindow.ChatModeEnum.Assets )
				this.labelText.Text = $"Edit assets in \"{fullPath}\":";
			else
				this.labelText.Text = "Edit this document:";

			textBoxName.Text = "";
			textBoxName.MaxLength = 1000;

			checkBoxEditSelectedOnly.Enabled = selectedObjects != null && selectedObjects.Length > 0;

			EditorThemeUtility.ApplyDarkThemeToForm( this );

			labelError.ForeColor = Color.Gray;

			buttonOK.Text = EditorLocalization2.Translate( "General", buttonOK.Text );
			buttonCancel.Text = EditorLocalization2.Translate( "General", buttonCancel.Text );

			buttonOK.Enabled = false;
		}

		private void ChatPromptForm_Load( object sender, EventArgs e )
		{
			UpdateControls();
		}

		[Browsable( false )]
		public KryptonTextBox TextBoxName
		{
			get { return textBoxName; }
		}

		public string TextBoxText
		{
			get { return textBoxName.Text; }
		}

		private void textBoxName_TextChanged( object sender, EventArgs e )
		{
			var text = textBoxName.Text.Trim();
			buttonOK.Enabled = !string.IsNullOrEmpty( text );
		}

		private void ChatPromptForm_FormClosing( object sender, FormClosingEventArgs e )
		{
			//if( DialogResult == DialogResult.OK )
			//{
			//}
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

		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( keyData == ( Keys.Shift | Keys.Enter ) && textBoxName.Focused && textBoxName.Multiline && textBoxName.AcceptsReturn )
			{
				textBoxName.SelectedText = Environment.NewLine;
				return true;
			}

			return base.ProcessCmdKey( ref msg, keyData );
		}
	}
}