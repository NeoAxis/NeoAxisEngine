// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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