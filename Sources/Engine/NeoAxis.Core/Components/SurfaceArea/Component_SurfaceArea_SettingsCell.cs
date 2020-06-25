// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public class Component_SurfaceArea_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpdate;

		//

		protected override void OnInitUI()
		{
			buttonUpdate = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Update" ) );
			buttonUpdate.Click += ButtonUpdate_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			var areas = GetObjects<Component_SurfaceArea>();
			buttonUpdate.Enabled = areas.Any( obj => obj.IsDataInitialized() );
		}

		private unsafe void ButtonUpdate_Click( ProcedureUI.Button sender )
		{
			var areas = GetObjects<Component_SurfaceArea>();
			foreach( var area in areas )
				area.UpdateOutput();
		}
	}
}
