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
	public partial class HCGridTextBoxSelect : EUserControl, IHCTextBoxSelect
	{
		public HCGridTextBoxSelect()
		{
			InitializeComponent();

			if( textBox != null )
				textBox.LikeLabel = UseReadOnlyLabel;

			if( EditorAPI2.DarkTheme )
				buttonSelect.Values.Image = Properties.Resources.DropDownButton_Dark;

			textBox.Location = new Point( 0, DpiHelper.Default.ScaleValue( 3 ) );
			textBox.AutoSize = false;
			textBox.Height = DpiHelper.Default.ScaleValue( 18 );

			buttonSelect.Location = new Point( buttonSelect.Location.X, DpiHelper.Default.ScaleValue( 3 ) );
		}

		bool useReadOnlyLabel = false;
		public bool UseReadOnlyLabel
		{
			get { return useReadOnlyLabel; }
			set
			{
				useReadOnlyLabel = value;
				if( textBox != null )
					textBox.LikeLabel = value;
			}
		}

		public EngineTextBox TextBox
		{
			get { return textBox; }
		}

		public KryptonButton ButtonSelect
		{
			get { return buttonSelect; }
		}
	}
}

#endif