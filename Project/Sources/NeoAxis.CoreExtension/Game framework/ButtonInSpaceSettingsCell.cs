// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class ButtonInSpaceSettingsCell : SettingsCellProcedureUI
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
			foreach( var button in GetObjects<ButtonInSpace>() )
				button.ClickingBegin();
		}
	}
}
#endif