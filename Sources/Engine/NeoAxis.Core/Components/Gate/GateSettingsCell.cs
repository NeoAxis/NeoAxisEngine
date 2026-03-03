// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class GateSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button gateClick;

		//

		protected override void OnInit()
		{
			gateClick = ProcedureForm.CreateButton( "Click" );
			gateClick.Click += GateClick_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { gateClick } );
		}

		private void GateClick_Click( ProcedureUI.Button sender )
		{
			//!!!!

			//foreach( var gate in GetObjects<Gate>() )
			//	gate.ClickingBegin();
		}
	}
}
#endif