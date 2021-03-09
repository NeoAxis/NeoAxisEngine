// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace NeoAxis.Editor
{
	public class Component_WorldGenerator_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonGenerate;
		////ProcedureUI.Button buttonExport;

		//

		protected override void OnInit()
		{
			buttonGenerate = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Generate" ) );
			buttonGenerate.Click += ButtonGenerate_Click;

			//buttonExport = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Export to FBX" ) );
			//buttonExport.Click += ButtonExport_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonGenerate } );
			//ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate, buttonExport } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			buttonGenerate.Enabled = GetObjects<Component_WorldGenerator>().Length == 1;
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
				foreach( var generator in GetObjects<Component_WorldGenerator>() )
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
