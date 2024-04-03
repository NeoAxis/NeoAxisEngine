#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class HCItemTextBoxDropMultiline : HCItemTextBoxSelect
	{
		public HCItemTextBoxDropMultiline( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		protected override void OnDropDownMouseButtonDown()
		{
			if( !Owner.IsDropDownOpen )
			{
				var control = (IHCTextBoxSelect)CreatedControlInsidePropertyItemControl;
				Owner.ToggleDropDown( new MultilineTextDropDownControl( this ), control.ButtonSelect );
			}
			else
				Owner.ToggleDropDown( null, null );
		}
	}
}
#endif