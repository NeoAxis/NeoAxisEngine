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
	public partial class HCGridCheckBox : UserControl, IHCCheckBox
	{
		public HCGridCheckBox()
		{
			InitializeComponent();

			if( !WinFormsUtility.IsDesignerHosted( this ) )
			{
				checkBox1.AutoSize = false;
				checkBox1.Size = new Size( 1, 1 );
			}
		}

		public KryptonCheckBox CheckBox1
		{
			get { return checkBox1; }
		}

		public bool CheckBox1SetText
		{
			get { return false; }
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			if( !WinFormsUtility.IsDesignerHosted( this ) )
			{
				if( checkBox1.AutoSize != true )
					checkBox1.AutoSize = true;
			}
		}
	}
}
