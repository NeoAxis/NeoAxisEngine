#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using Internal.ComponentFactory.Krypton.Navigator;

namespace NeoAxis.Editor
{
	static class ExtensionMethods
	{
		public static DockWindow GetDockWindow( this KryptonPage page )
		{
			//Debug.Assert( page.Controls.Count <= 1 );
			if( page.Controls.Count == 0 )
				return null;
			return (DockWindow)page.Controls[ 0 ];
		}

		//public static bool HasDockWindow( this KryptonPage page )
		//{
		//	//Debug.Assert( page.Controls.Count <= 1 );
		//	return page.Controls.Count != 0;
		//}

		//public static DockWindow GetParentDockWindow( this Control control )
		//{
		//	var parent = control.Parent;

		//	while( parent != null )
		//	{
		//		if( parent is DockWindow )
		//			return (DockWindow)parent;
		//		parent = parent.Parent;
		//	}

		//	return null;
		//}

		//internal static int GetTotalControlsCount( this Control control )
		//{
		//	int count = 0;
		//	foreach( Control child in control.Controls )
		//		count += 1 + child.GetTotalControlsCount();
		//	return count;
		//}
	}
}

#endif