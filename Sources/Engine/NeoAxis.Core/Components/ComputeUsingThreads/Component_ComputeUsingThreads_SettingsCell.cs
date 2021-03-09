// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public class Component_ComputeUsingThreads_SettingsCell : SettingsCellProcedureUI
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

			var objects = GetObjects<Component_ComputeUsingThreads>();
			var startedCount = objects.Count( obj => obj.Started );

			buttonStart.Enabled = objects.Length != startedCount;
			buttonStop.Enabled = startedCount != 0;
		}

		private void ButtonStart_Click( ProcedureUI.Button sender )
		{
			foreach( var obj in GetObjects<Component_ComputeUsingThreads>() )
				obj.Start();
		}

		private void ButtonStop_Click( ProcedureUI.Button sender )
		{
			foreach( var obj in GetObjects<Component_ComputeUsingThreads>() )
				obj.Stop();
		}
	}
}
