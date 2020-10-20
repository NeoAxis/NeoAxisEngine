// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class EngineToolTipForm : Form//EngineForm
	{
		//const double opacityMax = 0.93;

		Color borderColor = Color.FromArgb( 100, 100, 100 );
		//double opacity;

		//

		public EngineToolTipForm(/*string title, */string body )//, bool error, int duration )
		{
			InitializeComponent();

			//var duration2 = duration;
			//if( duration2 < 0 )
			//	duration2 = int.MaxValue;
			//else
			//	duration2 = duration2 * 1000;

			//lifeTimer.Interval = duration2;
			//labelTitle.Text = title;
			labelBody.Text = body;

			//if( error )
			//	BackColor = Color.FromArgb( 206, 0, 0 );

			//if( duration == -1 )
			//{
			//	opacity = opacityMax;
			//	Opacity = opacity;
			//}

			if( EditorAPI.DarkTheme )
			{
				//if( !error )
				BackColor = Color.FromArgb( 10, 10, 10 );
				borderColor = Color.FromArgb( 90, 90, 90 );
				labelBody.ForeColor = Color.White;
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

			ControlPaint.DrawBorder( e.Graphics, ClientRectangle, borderColor, ButtonBorderStyle.Solid );
		}

		private void EngineToolTipForm_Load( object sender, EventArgs e )
		{
			//lifeTimer.Start();
			//timer1.Start();

			using( var g = CreateGraphics() )
			{
				//https://stackoverflow.com/questions/1203087/why-is-graphics-measurestring-returning-a-higher-than-expected-number

				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

				var textSize = g.MeasureString( labelBody.Text, labelBody.Font );
				var size = new Vector2I( (int)( textSize.Width + EditorAPI.DPIScale * 7.0f ), (int)( textSize.Height + EditorAPI.DPIScale * 7.0f ) );

				//fix Location when outsize the screen
				var screenSize = SystemSettings.AllDisplaysBounds.RightBottom;
				var location = Location;
				if( Location.X + size.X + (int)( EditorAPI.DPIScale * 2.0f ) > screenSize.X )
					location.X = screenSize.X - size.X - (int)( EditorAPI.DPIScale * 2.0f );
				if( Location.Y + size.Y + (int)( EditorAPI.DPIScale * 2.0f ) > screenSize.Y )
					location.Y = screenSize.Y - size.Y - (int)( EditorAPI.DPIScale * 2.0f );
				if( location != Location )
					Location = location;

				Size = new Size( size.X, size.Y );
				labelBody.Location = new Point( (int)( EditorAPI.DPIScale * 2.0f ), 0 );
				labelBody.Size = new Size( Size.Width + 100, Size.Height );
			}
		}

		//private void lifeTimer_Tick( object sender, EventArgs e )
		//{
		//Close();
		//}

		private void EngineToolTipForm_Click( object sender, EventArgs e )
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

		//private void timer1_Tick( object sender, EventArgs e )
		//{
		//if( opacity < opacityMax )
		//{
		//	opacity += 10.0 / 1000.0 * 8;
		//	if( opacity > opacityMax )
		//		opacity = opacityMax;
		//	Opacity = opacity;
		//}
		//}
	}
}