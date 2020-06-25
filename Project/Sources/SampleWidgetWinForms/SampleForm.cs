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
	}
}
