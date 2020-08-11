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

namespace NeoAxis.Editor
{
	public partial class HCGridGroup : EUserControl, IHCGroup
	{
		public HCGridGroup()
		{
			InitializeComponent();

			if( EditorAPI.DarkTheme )
			{
				label1.ForeColor = Color.FromArgb( 160, 160, 160 );
				label1.BackColor = Color.FromArgb( 40, 40, 40 );
			}
		}

		public Label Label1
		{
			get { return label1; }
		}

		public override string ToString()
		{
			return nameof( HCGridGroup ) + ": " + label1.Text;
		}
	}
}
