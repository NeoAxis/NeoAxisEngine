// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using NeoAxis.Editor;

namespace NeoAxis.Addon.ExampleEditorWinForms
{
	[DesignerCategory( "" )]
	public partial class ExampleEditorWinFormsWindow : DocumentWindow
	{
		public ExampleEditorWinFormsWindow()
		{
			InitializeComponent();
		}

		public Component_ExampleEditorWinForms Component
		{
			get { return (Component_ExampleEditorWinForms)ObjectOfWindow; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			//!!!!temporary need use additional form, because .NET Core designer is not supports DocumentWindow based class (UserControl).

			//create form
			var form = new ExampleEditorWinFormsForm( Component );
			form.TopLevel = false;
			Controls.Add( form );
			form.FormBorderStyle = FormBorderStyle.None;
			form.Dock = DockStyle.Fill;
			form.Show();
		}
	}
}
