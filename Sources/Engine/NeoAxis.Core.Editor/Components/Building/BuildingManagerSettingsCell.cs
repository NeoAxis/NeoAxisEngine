// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class BuildingManagerSettingsCell : SettingsCellProcedureUI
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
			var manager = GetFirstObject<BuildingManager>();

			var scene = manager?.ParentScene;
			if( scene != null )
			{
				foreach( var building in scene.GetComponents<Building>() )
					building.NeedUpdate();
			}
		}
	}
}