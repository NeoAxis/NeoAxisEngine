// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoAxis.Editor
{
	public class DockWindowEventArgs : EventArgs
	{
		public DockWindow DockWindow { get; set; }
		public DockWindowEventArgs(DockWindow dockWindow)
		{
			DockWindow = dockWindow;
		}
	}
}
