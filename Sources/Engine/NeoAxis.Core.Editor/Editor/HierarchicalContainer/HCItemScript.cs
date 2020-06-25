// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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

		// it is necessary to prevent the reopening of the dropDownHolder
		// when the button is pressed and the holder is already open
		// is relevant only for HCFormDropDownHolder
		//TODO: need to think about a more universal solution.
		bool dropDownHolderWasOpened;

		protected override void OnDropDownMouseButtonDown()
		{
			base.OnDropDownMouseButtonDown();

			dropDownHolderWasOpened = Owner.IsDropDownOpen();
		}

		protected override void OnDropDownMouseButtonUp()
		{
			if( !dropDownHolderWasOpened )
				Owner.ToggleDropDown( new ScriptDropDownControl( this ), this );
		}
	}
}
