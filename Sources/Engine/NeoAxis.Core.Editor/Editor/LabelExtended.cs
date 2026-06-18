// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public class LabelExtended : Label
	{
		static bool controlToolTipFieldsSearched;
		static FieldInfo controlToolTipField1;
		static FieldInfo controlToolTipField2;

		public LabelExtended()
		{
			//set controlToolTip = true
			try
			{
				if( !controlToolTipFieldsSearched )
				{
					controlToolTipField1 = typeof( Label ).GetField( "_controlToolTip", BindingFlags.NonPublic | BindingFlags.Instance );
					controlToolTipField2 = typeof( Label ).GetField( "controlToolTip", BindingFlags.NonPublic | BindingFlags.Instance );
					controlToolTipFieldsSearched = true;
				}

				controlToolTipField1?.SetValue( this, true );
				controlToolTipField2?.SetValue( this, true );

				//{
				//	var field = typeof( Label ).GetField( "_controlToolTip", BindingFlags.NonPublic | BindingFlags.Instance );
				//	field?.SetValue( this, true );
				//}

				//{
				//	var field = typeof( Label ).GetField( "controlToolTip", BindingFlags.NonPublic | BindingFlags.Instance );
				//	field?.SetValue( this, true );
				//}
			}
			catch { }
		}
	}
}