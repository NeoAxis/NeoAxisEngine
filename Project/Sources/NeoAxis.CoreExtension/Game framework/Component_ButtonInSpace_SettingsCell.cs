// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Component_ButtonInSpace_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonClick;

		//

		protected override void OnInit()
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
