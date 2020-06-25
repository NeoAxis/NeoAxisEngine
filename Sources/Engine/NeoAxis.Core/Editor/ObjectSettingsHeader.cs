// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class ObjectSettingsHeader : EUserControl
	{
		//float headersSortingPriority;

		//

		public ObjectSettingsHeader()
		{
			InitializeComponent();
		}

		[Browsable( false )]
		public ObjectSettingsWindow ObjectSettingsWindow
		{
			get
			{
				var p = Parent as ObjectSettingsWindow;
				if( p != null )
					return p;
				else
					return null;

				//var p = Parent as TableLayoutPanel;
				//if( p != null )
				//	return p.Parent as ObjectSettingsWindow;
				//else
				//	return null;
			}
		}

		//public virtual float HeadersSortingPriority
		//{
		//	get { return headersSortingPriority; }
		//	set { headersSortingPriority = value; }
		//}
	}
}
