#if !DEPLOY
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
	public partial class ProcedureUIDialog : EngineForm
	{
		ProcedureUIDialogSettings settings;
		internal ProcedureUI.Form form;

		Timer timer;

		//

		public ProcedureUIDialog( ProcedureUIDialogSettings settings )
		{
			this.settings = settings;

			ClientSize = new Size( settings.Size.X, settings.Size.Y );
			InitializeComponent();

			if( string.IsNullOrEmpty( settings.Caption ) )
				Text = EngineInfo.NameWithVersion;
			else
				Text = settings.Caption;

			if( settings.Sizable )
			{
				FormBorderStyle = FormBorderStyle.Sizable;
				MinimumSize = new Size( 200, 140 );
				//MaximizeBox = true;
			}

			EditorThemeUtility.ApplyDarkThemeToForm( this );

			if( settings.DialogButtons == ProcedureUIDialogSettings.DialogButtonConfiguration.OKCancel )
			{
				buttonOK.Text = EditorLocalization2.Translate( "General", buttonOK.Text );
				buttonCancel.Text = EditorLocalization2.Translate( "General", buttonCancel.Text );
			}
			else
			if( settings.DialogButtons == ProcedureUIDialogSettings.DialogButtonConfiguration.Close )
			{
				buttonOK.Visible = false;
				buttonOK.Enabled = false;
				buttonCancel.Text = EditorLocalization2.Translate( "General", "Close" );
			}
			else if( settings.DialogButtons == ProcedureUIDialogSettings.DialogButtonConfiguration.None )
			{
				buttonOK.Visible = true;
				buttonCancel.Visible = false;
				buttonCancel.Enabled = false;
			}

			AcceptButton = GetButtonByName( settings.AcceptButton );
			CancelButton = GetButtonByName( settings.CancelButton );

			timer = new Timer();
			timer.Enabled = false;
			timer.Interval = 10;
			timer.Tick += timer_Tick;

			buttonOK.Click += ButtonOK_Click;
			buttonCancel.Click += ButtonCancel_Click;
		}

		private void ProcedureUIDialog_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			UpdateControls();

			//Translate();

			timer.Start();
		}

		private void ProcedureUIDialog_FormClosing( object sender, FormClosingEventArgs e )
		{
			//process clicking window close button
			if( e.CloseReason == CloseReason.UserClosing )
			{
				if( buttonCancel.Visible )
					ButtonCancel_Click( this, new EventArgs() );
			}
		}

		void UpdateControls()
		{
			buttonCancel.Location = new Point( ClientSize.Width - buttonCancel.Size.Width - DpiHelper.Default.ScaleValue( 12 ), ClientSize.Height - buttonCancel.Size.Height - DpiHelper.Default.ScaleValue( 12 ) );
			buttonOK.Location = new Point( buttonCancel.Location.X - buttonOK.Size.Width - DpiHelper.Default.ScaleValue( 8 ), buttonCancel.Location.Y );

			workareaControl.Location = new Point( DpiHelper.Default.ScaleValue( 10 ), DpiHelper.Default.ScaleValue( 10 ) );

			if( settings.DialogButtons != ProcedureUIDialogSettings.DialogButtonConfiguration.None )
			{
				workareaControl.Size = new Size( ClientSize.Width - workareaControl.Location.X - DpiHelper.Default.ScaleValue( 10 ), buttonCancel.Location.Y - DpiHelper.Default.ScaleValue( 10 ) );
			}
			else
			{
				workareaControl.Size = new Size( ClientSize.Width - workareaControl.Location.X - DpiHelper.Default.ScaleValue( 10 ), ClientSize.Height - workareaControl.Location.Y - DpiHelper.Default.ScaleValue( 10 ) );
			}
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( IsHandleCreated )
				UpdateControls();
		}

		[Browsable( false )]
		public Control WorkareaControl
		{
			get { return workareaControl; }
		}

		void timer_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			if( settings.DialogButtons == ProcedureUIDialogSettings.DialogButtonConfiguration.OKCancel )
			{
				{
					var enabled = true;
					settings.DialogButtonGetState?.Invoke( form, ProcedureUIDialogSettings.DialogButtonName.OK, ref enabled );
					buttonOK.Enabled = enabled;
				}
				{
					var enabled = true;
					settings.DialogButtonGetState?.Invoke( form, ProcedureUIDialogSettings.DialogButtonName.Cancel, ref enabled );
					buttonCancel.Enabled = enabled;
				}
			}
			else if( settings.DialogButtons == ProcedureUIDialogSettings.DialogButtonConfiguration.Close )
			{
				var enabled = true;
				settings.DialogButtonGetState?.Invoke( form, ProcedureUIDialogSettings.DialogButtonName.Close, ref enabled );
				buttonCancel.Enabled = enabled;
			}
		}

		private void ButtonOK_Click( object sender, EventArgs e )
		{
			var handled = false;

			if( settings.DialogButtons == ProcedureUIDialogSettings.DialogButtonConfiguration.OKCancel )
				settings.DialogButtonClick?.Invoke( form, ProcedureUIDialogSettings.DialogButtonName.OK, ref handled );

			if( handled )
				DialogResult = DialogResult.None;
		}

		private void ButtonCancel_Click( object sender, EventArgs e )
		{
			var handled = false;

			if( settings.DialogButtons == ProcedureUIDialogSettings.DialogButtonConfiguration.OKCancel )
				settings.DialogButtonClick?.Invoke( form, ProcedureUIDialogSettings.DialogButtonName.Cancel, ref handled );
			else if( settings.DialogButtons == ProcedureUIDialogSettings.DialogButtonConfiguration.Close )
				settings.DialogButtonClick?.Invoke( form, ProcedureUIDialogSettings.DialogButtonName.Close, ref handled );

			if( handled )
				DialogResult = DialogResult.None;
		}

		KryptonButton GetButtonByName( ProcedureUIDialogSettings.DialogButtonName name )
		{
			if( name == ProcedureUIDialogSettings.DialogButtonName.Close )
				return buttonCancel;
			else if( name == ProcedureUIDialogSettings.DialogButtonName.OK )
				return buttonOK;
			else if( name == ProcedureUIDialogSettings.DialogButtonName.Cancel )
				return buttonCancel;

			return null;
		}
	}
}
#endif