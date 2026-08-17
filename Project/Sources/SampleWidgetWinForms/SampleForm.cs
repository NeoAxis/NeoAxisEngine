// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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

			base.Font = new Font( new FontFamily( "Microsoft Sans Serif" ), 8f );
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
		}

		private void buttonClose_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void buttonNewForm_Click( object sender, EventArgs e )
		{
			if( Scene.GetFirst() == null )
			{
				MessageBox.Show( "The scene has not been created yet." );
				return;
			}

			var form = new AdditionalForm();
			form.Show();
		}
	}
}
