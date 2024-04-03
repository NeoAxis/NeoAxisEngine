#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public class RenderingEffect_Script_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpdate;
		ProcedureUI.Check checkAutoUpdate;

		//

		protected override void OnInit()
		{
			buttonUpdate = ProcedureForm.CreateButton( EditorLocalization2.Translate( "General", "Update" ) );
			buttonUpdate.Click += ButtonUpdate_Click;

			checkAutoUpdate = ProcedureForm.CreateCheck( EditorLocalization2.Translate( "General", "Auto update" ) );
			checkAutoUpdate.Click += CheckAutoUpdate_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate, checkAutoUpdate } );
		}

		private void ButtonUpdate_Click( ProcedureUI.Button sender )
		{
			foreach( var effect in GetObjects<RenderingEffect_Script>() )
				effect.PerformResultCompile();
		}

		private void CheckAutoUpdate_Click( ProcedureUI.Check obj )
		{
			if( checkAutoUpdate.Checked == ProcedureUI.Check.CheckValue.Indeterminate )
				return;

			var undoItems = new List<UndoActionPropertiesChange.Item>();
			foreach( var effect in GetObjects<RenderingEffect_Script>() )
			{
				var oldValue = effect.EditorAutoUpdate;

				effect.EditorAutoUpdate = checkAutoUpdate.Checked == ProcedureUI.Check.CheckValue.Checked;

				var property = (Metadata.Property)effect.MetadataGetMemberBySignature( "property:EditorAutoUpdate" );
				undoItems.Add( new UndoActionPropertiesChange.Item( effect, property, oldValue ) );
			}

			var undoAction = new UndoActionPropertiesChange( undoItems );
			Provider.DocumentWindow.Document.CommitUndoAction( undoAction );
		}

		protected override void OnUpdate()
		{
			var objects = GetObjects<RenderingEffect_Script>();

			if( objects.All( effect => effect.EditorAutoUpdate ) )
				checkAutoUpdate.Checked = ProcedureUI.Check.CheckValue.Checked;
			else if( objects.All( effect => !effect.EditorAutoUpdate ) )
				checkAutoUpdate.Checked = ProcedureUI.Check.CheckValue.Unchecked;
			else
				checkAutoUpdate.Checked = ProcedureUI.Check.CheckValue.Indeterminate;

			//buttonUpdate.Enabled = checkAutoUpdate.Checked != ProcedureUI.Check.CheckValue.Checked;
		}
	}
}
#endif