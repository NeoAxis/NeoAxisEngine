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
	public partial class HCGridTextBoxNumeric : UserControl, IHCTextBoxNumeric
	{
		public HCGridTextBoxNumeric()
		{
			InitializeComponent();
		}

		public HCKryptonTextBox TextBox
		{
			get { return textBox1; }
		}

		public KryptonTrackBar TrackBar
		{
			get { return trackBar; }
		}
	}
}
