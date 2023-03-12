#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class CreatureMakerSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpdate;
		ProcedureUI.Button buttonClear;

		//

		protected override void OnInit()
		{
			buttonUpdate = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Update" ) );
			buttonUpdate.Click += ButtonUpdate_Click;

			buttonClear = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Clear" ) );
			buttonClear.Click += ButtonClear_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate, buttonClear } );
		}

		private void ButtonUpdate_Click( ProcedureUI.Button sender )
		{
			foreach( var maker in GetObjects<CreatureMaker>() )
			{
				if( !maker.UpdateMesh( false, out var error ) )
				{
					ScreenNotifications.Show( error, true );
					return;
				}
			}
		}

		private void ButtonClear_Click( ProcedureUI.Button sender )
		{
			foreach( var maker in GetObjects<CreatureMaker>() )
			{
				if( !maker.UpdateMesh( true, out var error ) )
				{
					ScreenNotifications.Show( error, true );
					return;
				}
			}
		}
	}
}
#endif