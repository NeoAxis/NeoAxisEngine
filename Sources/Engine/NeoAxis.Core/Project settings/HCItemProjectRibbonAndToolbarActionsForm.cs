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
	partial class HCItemProjectRibbonAndToolbarActionsForm : UserControl
	{
		public HCItemProjectRibbonAndToolbarActionsForm()
		{
			InitializeComponent();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			if( EditorAPI.DarkTheme )
				toolStrip1.Renderer = DarkThemeUtility.GetToolbarToolStripRenderer();

			//double distance = 22.0 * EditorAPI.DPIScale;
			//kryptonSplitContainer2.Panel1MinSize = (int)distance;
			//kryptonSplitContainer2.SplitterDistance = (int)distance;
		}
	}
}
