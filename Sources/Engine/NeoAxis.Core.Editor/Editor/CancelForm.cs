#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class CancelForm : EngineForm
	{
		//public delegate void CancelDelegate( CancelForm sender );
		//public event CancelDelegate Cancel;

		//

		public CancelForm( string labelText, string caption )
		{
			InitializeComponent();

			this.labelText.Text = labelText;

			if( string.IsNullOrEmpty( caption ) )
				Text = EngineInfo.NameWithVersion;
			else
				Text = caption;

			EditorThemeUtility.ApplyDarkThemeToForm( this );

			buttonCancel.Text = EditorLocalization2.Translate( "General", buttonCancel.Text );
		}

		private void OKCancelTextBoxForm_Load( object sender, EventArgs e )
		{
			UpdateControls();

			//Translate();
		}

		public string LabelText
		{
			set
			{
				if( IsDisposed )
					return;

				try
				{
					if( InvokeRequired )
						Invoke( new MethodInvoker( () => { labelText.Text = value; } ) );
					else
						labelText.Text = value;
				}
				catch { }
			}
		}

		//public string LabelText
		//{
		//	get { return labelText.Text; }
		//	set { labelText.Text = value; }
		//}

		private void CancelForm_FormClosing( object sender, FormClosingEventArgs e )
		{
			//if( DialogResult == DialogResult.Cancel )
			//	Cancel?.Invoke( this );
		}

		void UpdateControls()
		{
			labelText.Width = ClientSize.Width - labelText.Location.X * 2;
			buttonCancel.Location = new Point( ( ClientSize.Width - buttonCancel.Size.Width ) / 2, ClientSize.Height - buttonCancel.Size.Height - DpiHelper.Default.ScaleValue( 12 ) );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( IsHandleCreated )
				UpdateControls();
		}

		private void buttonCancel_Click( object sender, EventArgs e )
		{
			DialogResult = DialogResult.Cancel;
			Close();
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

		public void CloseMultithreaded()
		{
			if( IsDisposed )
				return;

			try
			{
				if( InvokeRequired )
					Invoke( new MethodInvoker( () => { Close(); } ) );
				else
					Close();
			}
			catch { }
		}

		public void SetTopMostMultithreaded( bool value )
		{
			if( IsDisposed )
				return;

			try
			{
				if( InvokeRequired )
					Invoke( new MethodInvoker( () => { TopMost = value; } ) );
				else
					TopMost = value;
			}
			catch { }
		}
	}
}
#endif