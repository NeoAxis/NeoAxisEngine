// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public class Component_StoreProduct_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpload;
		ProcedureUI.Button buttonUploadAndPublish;

		//

		protected override void OnInit()
		{
			buttonUpload = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Upload" ) );
			buttonUpload.Click += ButtonUpload_Click;

			buttonUploadAndPublish = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Upload and Publish" ), ProcedureUI.Button.SizeEnum.Long );
			buttonUploadAndPublish.Click += ButtonUploadAndPublish_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpload, buttonUploadAndPublish } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			//!!!!
			buttonUploadAndPublish.Enabled = false;
		}

		private void ButtonUpload_Click( ProcedureUI.Button sender )
		{
			if( EditorMessageBox.ShowQuestion( "Upload selected products to the store?", EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
				return;

			var item = ScreenNotifications.ShowSticky( "Processing..." );
			try
			{
				foreach( var product in GetObjects<Component_StoreProduct>() )
				{
					if( !product.BuildArchive() )
						return;
				}
			}
			catch( Exception e )
			{
				Log.Warning( e.Message );
				return;
			}
			finally
			{
				item.Close();
			}

			ScreenNotifications.Show( "The product was prepared successfully." );
		}

		private void ButtonUploadAndPublish_Click( ProcedureUI.Button sender )
		{
			//!!!!
		}
	}
}
