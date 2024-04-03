#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Workspace;
using Internal.ComponentFactory.Krypton.Docking;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class SettingsHeader : EUserControl
	{
		//float headersSortingPriority;

		//

		public SettingsHeader()
		{
			InitializeComponent();
		}

		[Browsable( false )]
		public SettingsWindow.PanelData SettingsPanel
		{
			get
			{
				return FindControlInParentTags<SettingsWindow.PanelData>( this );
			}
		}

		private T FindControlInParentTags<T>( Control control ) where T : class
		{
			if( control.Parent == null )
				return null;

			if( control.Parent.Tag is T parentTag )
				return parentTag;
			else
				return FindControlInParentTags<T>( control.Parent );
		}

		//public virtual float HeadersSortingPriority
		//{
		//	get { return headersSortingPriority; }
		//	set { headersSortingPriority = value; }
		//}
	}
}

#endif