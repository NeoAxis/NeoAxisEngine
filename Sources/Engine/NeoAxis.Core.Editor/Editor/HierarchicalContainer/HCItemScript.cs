// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace NeoAxis.Editor
{
	public class HCItemScript : HCItemTextBoxSelect
	{
		public HCItemScript( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		protected override void OnDropDownMouseButtonDown()
		{
			if( !Owner.IsDropDownOpen )
			{
				var control = (IHCTextBoxSelect)CreatedControlInsidePropertyItemControl;
				Owner.ToggleDropDown( new ScriptDropDownControl( this ), control.ButtonSelect );
			}
			else
				Owner.ToggleDropDown( null, null );
		}
	}
}
