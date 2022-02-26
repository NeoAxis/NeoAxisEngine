// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class ProductSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonBuild;
		ProcedureUI.Button buttonBuildAndRun;

		//

		Product GetProduct()
		{
			return GetFirstObject<Product>();
		}

		protected override void OnInit()
		{
			buttonBuild = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Build" ) );
			buttonBuild.Click += ButtonBuild_Click;

			buttonBuildAndRun = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Build and Run" ), ProcedureUI.Button.SizeEnum.Long );
			buttonBuildAndRun.Click += ButtonBuildAndRun_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonBuild, buttonBuildAndRun } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			var product = GetProduct();
			if( product != null )
			{
				buttonBuild.Enabled = true;
				buttonBuildAndRun.Enabled = product.SupportsBuildAndRun;

				if( product as Product_Store != null )
					buttonBuildAndRun.Text = EditorLocalization.Translate( "General", "Build and Upload" );
			}
			else
			{
				buttonBuild.Enabled = false;
				buttonBuildAndRun.Enabled = false;
			}
		}

		private void ButtonBuild_Click( ProcedureUI.Button sender )
		{
			if( GetProduct() != null )
				GoBuild( GetProduct(), false );
		}

		private void ButtonBuildAndRun_Click( ProcedureUI.Button sender )
		{
			if( GetProduct() != null )
				GoBuild( GetProduct(), true );
		}

		void GoBuild( Product product, bool run )
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
#endif