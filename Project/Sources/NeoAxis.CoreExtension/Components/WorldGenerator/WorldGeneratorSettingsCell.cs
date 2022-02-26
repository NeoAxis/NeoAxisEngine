// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class WorldGeneratorSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonGenerate;

		//

		protected override void OnInit()
		{
			buttonGenerate = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Generate" ) );
			buttonGenerate.Click += ButtonGenerate_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonGenerate } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			buttonGenerate.Enabled = GetObjects<WorldGenerator>().Length == 1;
		}

		private void ButtonGenerate_Click( ProcedureUI.Button sender )
		{
			//!!!!support undo
			var text = "Generate world?\n\nUnable to undo the action.";
			if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
				return;

			var notification = ScreenNotifications.ShowSticky( "Processing..." );
			try
			{
				foreach( var generator in GetObjects<WorldGenerator>() )
					generator.Generate( Provider.DocumentWindow.Document );

				//!!!!
				//clear undo history
				Provider.DocumentWindow.Document?.UndoSystem.Clear();
			}
			finally
			{
				notification.Close();
			}
		}
	}
}
#endif