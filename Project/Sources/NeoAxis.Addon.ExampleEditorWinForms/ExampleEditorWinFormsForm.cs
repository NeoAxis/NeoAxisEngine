// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NeoAxis.Addon.ExampleEditorWinForms
{
	public partial class ExampleEditorWinFormsForm : Form
	{
		Component_ExampleEditorWinForms component;

		//

		public ExampleEditorWinFormsForm( Component_ExampleEditorWinForms component )
		{
			this.component = component;

			InitializeComponent();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			textBox1.Text = component.ValueToDisplay;
		}
	}
}
