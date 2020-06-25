// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Component_ButtonInSpace_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonClick;

		//

		protected override void OnInitUI()
		{
			buttonClick = ProcedureForm.CreateButton( "Click" );
			buttonClick.Click += ButtonClick_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonClick } );
		}

		private void ButtonClick_Click( ProcedureUI.Button sender )
		{
			foreach( var button in GetObjects<Component_ButtonInSpace>() )
				button.ClickingBegin();
		}
	}
}
