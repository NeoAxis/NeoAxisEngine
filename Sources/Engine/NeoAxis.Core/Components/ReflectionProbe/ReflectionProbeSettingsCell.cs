// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public class ReflectionProbeSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpdate;
		//ProcedureUI.Button buttonSaveToFile;

		//

		protected override void OnInit()
		{
			buttonUpdate = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Update" ) );
			buttonUpdate.Click += ButtonUpdate_Click;

			//buttonSaveToFile = ProcedureForm.CreateButton( "Save to File" );
			//buttonSaveToFile.Click += ButtonSaveToFile_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate } );//, buttonSaveToFile } );
		}

		protected override void OnUpdate()
		{
			bool enable = false;
			foreach( var p in GetObjects<ReflectionProbe>() )
			{
				if( p.Mode.Value == ReflectionProbe.ModeEnum.Capture /*&& !p.Realtime */)
					enable = true;
			}
			buttonUpdate.Enabled = enable;

			//buttonSaveToFile.Enabled = false;
		}

		private void ButtonUpdate_Click( ProcedureUI.Button sender )
		{
			foreach( var p in GetObjects<ReflectionProbe>() )
			{
				if( p.Mode.Value == ReflectionProbe.ModeEnum.Capture )
					p.UpdateCaptureCubemap();
				//if( p.Mode.Value == ReflectionProbe.ModeEnum.Capture && !p.Realtime )
				//	p.Update( true );
			}
		}

		//private void ButtonSaveToFile_Click( ProcedureUI.Button sender )
		//{
		//}
	}
}
