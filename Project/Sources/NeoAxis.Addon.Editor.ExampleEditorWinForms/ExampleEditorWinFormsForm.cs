// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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
		ExampleEditorWinForms component;

		//

		public ExampleEditorWinFormsForm( ExampleEditorWinForms component )
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
