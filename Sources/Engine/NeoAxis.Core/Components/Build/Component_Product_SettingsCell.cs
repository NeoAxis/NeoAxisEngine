// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Component_Product_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonBuild;
		ProcedureUI.Button buttonBuildAndRun;

		//

		protected override void OnInit()
		{
			buttonBuild = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Build" ) );
			buttonBuild.Click += ButtonBuild_Click;

			buttonBuildAndRun = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Build and Run" ) );
			buttonBuildAndRun.Click += ButtonBuildAndRun_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonBuild, buttonBuildAndRun } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			buttonBuild.Enabled = GetObjects<Component_Product>().Length == 1;
			buttonBuildAndRun.Enabled = buttonBuild.Enabled;
		}

		private void ButtonBuild_Click( ProcedureUI.Button sender )
		{
			GoBuild( GetFirstObject<Component_Product>(), false );
		}

		private void ButtonBuildAndRun_Click( ProcedureUI.Button sender )
		{
			GoBuild( GetFirstObject<Component_Product>(), true );
		}

		void GoBuild( Component_Product product, bool run )
		{
			if( Provider.DocumentWindow.Document.Modified )
			{
				if( !Provider.DocumentWindow.SaveDocument() )
					return;
			}

			BackstageMenu.needStartBuildProduct = ComponentUtility.GetOwnedFileNameOfComponent( product );
			BackstageMenu.needStartBuildProductAndRun = run;

			EditorForm.Instance.OpenBackstage();
		}
	}
}
