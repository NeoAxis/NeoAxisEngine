#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;

namespace NeoAxis.Editor
{
	public class CharacterMakerSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonWriteMesh;

		//

		protected override void OnInit()
		{
			buttonWriteMesh = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Bake Mesh" ) );
			buttonWriteMesh.Click += ButtonWriteMesh_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonWriteMesh } );
		}

		static bool WriteMesh( CharacterMaker maker, string writeToFolder, bool getFileNamesMode, List<string> fileNames )
		{
			var success = maker.WriteMesh( writeToFolder, getFileNamesMode, fileNames, out var error );

			if( !string.IsNullOrEmpty( error ) )
				EditorMessageBox.ShowWarning( error );

			return success;
		}

		private void ButtonWriteMesh_Click( ProcedureUI.Button sender )
		{
			foreach( var maker in GetObjects<CharacterMaker>() )
			{
				var typeFileName = ComponentUtility.GetOwnedFileNameOfComponent( maker );
				if( string.IsNullOrEmpty( typeFileName ) )
				{
					EditorMessageBox.ShowWarning( "Unable to get file name from the object." );
					return;
				}

				var writeToFolder = Path.GetDirectoryName( VirtualPathUtility.GetRealPathByVirtual( typeFileName ) );

				var fileNames = new List<string>();
				if( !WriteMesh( maker, writeToFolder, true, fileNames ) )
					return;

				var text = $"{fileNames.Count} files will created in the folder \"{writeToFolder}\". Continue?";
				if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
					return;

				var notification = ScreenNotifications.ShowSticky( "Processing..." );
				try
				{
					if( !WriteMesh( maker, writeToFolder, false, null ) )
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