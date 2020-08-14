// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
// =====COPYRIGHT=====
// Code originally retrieved from http://www.vbforums.com/showthread.php?t=547778 - no license information supplied
// =====COPYRIGHT=====
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class ScreenNotificationForm : Form//EngineForm
	{
		const double opacityMax = 0.93;

		static readonly List<ScreenNotificationForm> openForms = new List<ScreenNotificationForm>();
		Color borderColor = Color.FromArgb( 144, 171, 231 );
		double opacity;

		//

		public ScreenNotificationForm( string title, string body, bool error, int duration )
		{
			InitializeComponent();

			var duration2 = duration;
			if( duration2 < 0 )
				duration2 = int.MaxValue;
			else
				duration2 = duration2 * 1000;

			lifeTimer.Interval = duration2;
			labelTitle.Text = title;
			labelBody.Text = body;

			if( error )
				BackColor = Color.FromArgb( 206, 0, 0 );

			if( duration == -1 )
			{
				opacity = opacityMax;
				Opacity = opacity;
			}

			if( EditorAPI.DarkTheme )
			{
				if( !error )
					BackColor = Color.FromArgb( 10, 10, 10 );
				//BackColor = Color.FromArgb( 40, 40, 40 );
				borderColor = Color.FromArgb( 90, 90, 90 );
			}
		}

		protected override bool ShowWithoutActivation
		{
			get { return true; }
		}

		const int WS_EX_TOPMOST = 0x00000008;
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= WS_EX_TOPMOST;
				return createParams;
			}
		}

		//protected override CreateParams CreateParams
		//{
		//	get
		//	{
		//		CreateParams baseParams = base.CreateParams;

		//		const int WS_EX_NOACTIVATE = 0x08000000;
		//		const int WS_EX_TOOLWINDOW = 0x00000080;
		//		baseParams.ExStyle |= (int)( WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW );

		//		return baseParams;
		//	}
		//}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			System.Drawing.Rectangle borderRect = ClientRectangle;
			//borderRect.Inflate( -regionOffset, -regionOffset );
			ControlPaint.DrawBorder( e.Graphics, borderRect, borderColor, ButtonBorderStyle.Solid );
		}

		private void ScreenNotificationForm_Load( object sender, EventArgs e )
		{
			// Display the form just above the system tray.
			Location = new Point( Screen.PrimaryScreen.WorkingArea.Width - Width - 2, Screen.PrimaryScreen.WorkingArea.Height - Height - 2 );

			// Move each open form upwards to make room for this one
			foreach( ScreenNotificationForm openForm in openForms )
				openForm.Top -= Height + 3;

			openForms.Add( this );
			lifeTimer.Start();
			timer1.Start();
		}

		private void ScreenNotificationForm_FormClosed( object sender, FormClosedEventArgs e )
		{
			// Move down any open forms above this one
			foreach( ScreenNotificationForm openForm in openForms )
			{
				if( openForm == this )
				{
					// Remaining forms are below this one
					break;
				}
				openForm.Top += Height + 3;
			}

			openForms.Remove( this );
		}

		private void lifeTimer_Tick( object sender, EventArgs e )
		{
			Close();
		}

		private void ScreenNotificationForm_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void labelTitle_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void labelRO_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( opacity < opacityMax )
			{
				opacity += 10.0 / 1000.0 * 8;
				if( opacity > opacityMax )
					opacity = opacityMax;
				Opacity = opacity;
			}
		}
	}
}