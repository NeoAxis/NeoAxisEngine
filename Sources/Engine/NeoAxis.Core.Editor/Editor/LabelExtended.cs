#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public class LabelExtended : Label
	{
		public LabelExtended()
		{
			//set controlToolTip = true
			try
			{
				{
					var field = typeof( Label ).GetField( "_controlToolTip", BindingFlags.NonPublic | BindingFlags.Instance );
					field?.SetValue( this, true );
				}

				{
					var field = typeof( Label ).GetField( "controlToolTip", BindingFlags.NonPublic | BindingFlags.Instance );
					field?.SetValue( this, true );
				}
			}
			catch { }
		}
	}
}

#endif