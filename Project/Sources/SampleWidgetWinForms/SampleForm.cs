// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NeoAxis;

namespace SampleWidgetWinForms
{
	public partial class SampleForm : Form
	{
		public SampleForm()
		{
			InitializeComponent();

			Font = new Font( new FontFamily( "Microsoft Sans Serif" ), 8f );
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			UpdateControls();
		}

		private void buttonClose_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void buttonNewForm_Click( object sender, EventArgs e )
		{
			if( Component_Scene.First == null )
			{
				MessageBox.Show( "The scene has not been created yet." );
				return;
			}

			var form = new AdditionalForm();
			form.Show();
		}

		void UpdateControls()
		{
			//!!!!WinForms on .NET Core works strange with anchors

			//update widget bounds manually
			if( widgetControl1 != null )
			{
				var offset = widgetControl1.Location;
				widgetControl1.SetBounds( offset.X, offset.Y, buttonClose.Location.X - offset.X * 2, ClientSize.Height - offset.Y * 2 );
			}
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			UpdateControls();
		}
	}
}
