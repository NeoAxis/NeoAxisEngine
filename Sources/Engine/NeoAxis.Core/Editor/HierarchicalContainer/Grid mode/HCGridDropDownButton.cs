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
	public interface IHCDropDownButton
	{
		KryptonDropButton Button { get; }
	}

	public partial class HCGridDropDownButton : UserControl, IHCDropDownButton
	{
		public HCGridDropDownButton()
		{
			InitializeComponent();

			if( EditorAPI.DarkTheme )
				kryptonDropButton.Images.Common = Properties.Resources.DropDownButton_Dark;
			else
				kryptonDropButton.Images.Common = Properties.Resources.DropDownButton;
		}

		public KryptonDropButton Button
		{
			get { return kryptonDropButton; }
		}
	}
}
