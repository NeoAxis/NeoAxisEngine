// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Button3DSettingsCell : SettingsCellProcedureUI
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
			foreach( var button in GetObjects<Button3D>() )
				button.TryClick( null );// ClickingBegin();
		}
	}
}