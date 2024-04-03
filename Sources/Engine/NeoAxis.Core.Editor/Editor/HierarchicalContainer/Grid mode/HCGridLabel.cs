#if !DEPLOY
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
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public partial class HCGridLabel : EUserControl, IHCLabel
	{
		public HCGridLabel()
		{
			InitializeComponent();

			if( EditorAPI2.DarkTheme )
				label2.StateCommon.Back.Color1 = Color.FromArgb( 54, 54, 54 );
			else
				label2.StateCommon.Back.Color1 = Color.FromArgb( 240, 240, 240 );

			label2.Location = new Point( 0, DpiHelper.Default.ScaleValue( 3 ) );
			label2.AutoSize = false;
			label2.Height = Math.Max( DpiHelper.Default.ScaleValue( 18 ), label2.PreferredSize.Height );
		}

		//public Label Label1
		//{
		//	get { return null; }
		//}

		public EngineTextBox Label2
		{
			get { return label2; }
		}
	}
}

#endif