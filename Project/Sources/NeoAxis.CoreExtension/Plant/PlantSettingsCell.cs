// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace NeoAxis.Editor
{
	public class PlantSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonGenerateMeshes;

		//

		protected override void OnInit()
		{
			buttonGenerateMeshes = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Bake Meshes" ), ProcedureUI.Button.SizeEnum.Long );
			buttonGenerateMeshes.Click += ButtonGenerateMeshes_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonGenerateMeshes } );
		}

		static bool ExportToMeshes( PlantType plant, string writeToFolder, bool getFileNamesMode, List<string> fileNames )
		{
			var success = plant.ExportToMeshes( writeToFolder, getFileNamesMode, fileNames, out var error );

			if( !string.IsNullOrEmpty( error ) )
				EditorMessageBox.ShowWarning( error );

			return success;
		}

		private void ButtonGenerateMeshes_Click( ProcedureUI.Button sender )
		{
			foreach( var plantType in GetObjects<PlantType>() )
			{
				var typeFileName = ComponentUtility.GetOwnedFileNameOfComponent( plantType );
				if( string.IsNullOrEmpty( typeFileName ) )
				{
					EditorMessageBox.ShowWarning( "Unable to get file name from the object." );
					return;
				}

				var writeToFolder = Path.GetDirectoryName( VirtualPathUtility.GetRealPathByVirtual( typeFileName ) );

				var fileNames = new List<string>();
				if( !ExportToMeshes( plantType, writeToFolder, true, fileNames ) )
					return;

				var text = $"{fileNames.Count} files will created in the folder \'{writeToFolder}\'. Continue?";
				if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
					return;

				var notification = ScreenNotifications.ShowSticky( "Processing..." );
				try
				{
					if( !ExportToMeshes( plantType, writeToFolder, false, null ) )
						return;
				}
				finally
				{
					notification.Close();
				}
			}
		}
	}
}
#endif