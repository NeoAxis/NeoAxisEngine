#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Workspace;
using Internal.ComponentFactory.Krypton.Docking;
using System.IO;

namespace NeoAxis.Editor
{
	//!!!!по сути можно вертикально, можно горизонтально. есть похожесть с SettingsWindow

	public partial class PreviewControl : EUserControl
	{
		PreviewWindow.PanelData panel;

		public PreviewControl()
		{
			InitializeComponent();
		}

		[Browsable( false )]
		public PreviewWindow.PanelData Panel
		{
			get { return panel; }
			set { panel = value; }
		}

		[Browsable( false )]
		public object ObjectOfPreview
		{
			get
			{
				return panel.objects[ 0 ];
				//return panel.settingsPanel.selectedObjects[ 0 ];
			}
		}
	}
}
#endif