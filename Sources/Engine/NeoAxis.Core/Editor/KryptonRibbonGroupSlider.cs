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
using ComponentFactory.Krypton.Ribbon;

namespace NeoAxis.Editor
{
	public partial class KryptonRibbonGroupSlider : KryptonRibbonGroupCustomControl
	{
		KryptonRibbonGroupSliderControl control;

		public KryptonRibbonGroupSlider()
		{
			InitializeComponent();

			control = new KryptonRibbonGroupSliderControl();
			CustomControl = control;
		}

		[Browsable( false )]
		public KryptonRibbonGroupSliderControl Control
		{
			get { return control; }
		}
	}
}
