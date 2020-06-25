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
	public partial class HCGridLabel : UserControl, IHCLabel
	{
		public HCGridLabel()
		{
			InitializeComponent();

			if( EditorAPI.DarkTheme )
				label2.StateCommon.Back.Color1 = Color.FromArgb( 54, 54, 54 );
			else
				label2.StateCommon.Back.Color1 = Color.FromArgb( 240, 240, 240 );
		}

		//public Label Label1
		//{
		//	get { return null; }
		//}

		public HCKryptonTextBox Label2
		{
			get { return label2; }
		}
	}
}
