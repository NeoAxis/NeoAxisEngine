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
	public partial class ProcessingForm : EngineForm
	{
		public delegate void CancelDelegate();
		CancelDelegate cancelHandler;

		bool cancelledByUser;

		//

		public ProcessingForm( string caption, CancelDelegate cancelHandler )
		{
			this.cancelHandler = cancelHandler;

			InitializeComponent();

			if( string.IsNullOrEmpty( caption ) )
				Text = EngineInfo.NameWithVersion;
			else
				Text = caption;

			EditorThemeUtility.ApplyDarkThemeToForm( this );

			buttonCancel.Text = EditorLocalization2.Translate( "General", buttonCancel.Text );
		}

		private void ProcessingForm_Load( object sender, EventArgs e )
		{
			UpdateControls();
		}

		private void RenameResourceDialog_FormClosing( object sender, FormClosingEventArgs e )
		{
			if( DialogResult == DialogResult.Cancel )
			{
				cancelledByUser = true;
				if( cancelHandler != null )
					cancelHandler();
			}
		}

		void UpdateControls()
		{
			labelText.Size = new Size( ClientSize.Width, labelText.Size.Height );

			buttonCancel.Location = new Point( ( ClientSize.Width - buttonCancel.Size.Width ) / 2, ClientSize.Height - buttonCancel.Size.Height - DpiHelper.Default.ScaleValue( 12 ) );

			//buttonCancel.Location = new Point( ClientSize.Width - buttonCancel.Size.Width - DpiHelper.Default.ScaleValue( 12 ), ClientSize.Height - buttonCancel.Size.Height - DpiHelper.Default.ScaleValue( 12 ) );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( IsHandleCreated )
				UpdateControls();
		}

		public void CloseOK()
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		[Browsable( false )]
		public bool CancelledByUser
		{
			get { return cancelledByUser; }
		}
	}
}