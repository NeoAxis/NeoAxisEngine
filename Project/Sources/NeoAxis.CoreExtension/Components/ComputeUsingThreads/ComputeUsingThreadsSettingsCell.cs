// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public class ComputeUsingThreadsSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonStart;
		ProcedureUI.Button buttonStop;

		//

		protected override void OnInit()
		{
			buttonStart = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Start" ) );
			buttonStart.Click += ButtonStart_Click;

			buttonStop = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Stop" ) );
			buttonStop.Click += ButtonStop_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonStart, buttonStop } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			var objects = GetObjects<ComputeUsingThreads>();
			var startedCount = objects.Count( obj => obj.Started );

			buttonStart.Enabled = objects.Length != startedCount;
			buttonStop.Enabled = startedCount != 0;
		}

		private void ButtonStart_Click( ProcedureUI.Button sender )
		{
			foreach( var obj in GetObjects<ComputeUsingThreads>() )
				obj.Start();
		}

		private void ButtonStop_Click( ProcedureUI.Button sender )
		{
			foreach( var obj in GetObjects<ComputeUsingThreads>() )
				obj.Stop();
		}
	}
}
#endif