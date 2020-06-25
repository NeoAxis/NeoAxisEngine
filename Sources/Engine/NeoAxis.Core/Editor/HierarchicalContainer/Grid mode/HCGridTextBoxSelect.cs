// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public partial class HCGridTextBoxSelect : UserControl, IHCTextBoxSelect
	{
		public HCGridTextBoxSelect()
		{
			InitializeComponent();

			if( textBox != null )
				textBox.LookLikeLabel = UseReadOnlyLabel;

			if( EditorAPI.DarkTheme )
				buttonSelect.Values.Image = Properties.Resources.DropDownButton_Dark;
		}

		bool useReadOnlyLabel = false;
		public bool UseReadOnlyLabel
		{
			get { return useReadOnlyLabel; }
			set
			{
				useReadOnlyLabel = value;
				if( textBox != null )
					textBox.LookLikeLabel = value;
			}
		}

		public HCKryptonTextBox TextBox
		{
			get { return textBox; }
		}

		public KryptonButton ButtonSelect
		{
			get { return buttonSelect; }
		}
	}
}
