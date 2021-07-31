// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Component_RenderingEffect_Simple_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpdate;

		//

		protected override void OnInit()
		{
			buttonUpdate = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Update" ) );
			buttonUpdate.Click += ButtonUpdate_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate } );
		}

		private void ButtonUpdate_Click( ProcedureUI.Button sender )
		{
			foreach( var mesh in GetObjects<Component_RenderingEffect>() )
				mesh.ResultCompile();
		}
	}
}
