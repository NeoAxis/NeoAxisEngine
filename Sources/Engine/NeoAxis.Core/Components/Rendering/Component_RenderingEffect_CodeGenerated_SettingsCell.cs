// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public class Component_RenderingEffect_CodeGenerated_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpdate;
		ProcedureUI.Check checkAutoUpdate;

		//

		protected override void OnInit()
		{
			buttonUpdate = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Update" ) );
			buttonUpdate.Click += ButtonUpdate_Click;

			checkAutoUpdate = ProcedureForm.CreateCheck( EditorLocalization.Translate( "General", "Auto update" ) );
			checkAutoUpdate.Click += CheckAutoUpdate_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate, checkAutoUpdate } );
		}

		private void ButtonUpdate_Click( ProcedureUI.Button sender )
		{
			foreach( var effect in GetObjects<Component_RenderingEffect_CodeGenerated>() )
				effect.ResultCompile();
		}

		private void CheckAutoUpdate_Click( ProcedureUI.Check obj )
		{
			if( checkAutoUpdate.Checked == ProcedureUI.Check.CheckValue.Indeterminate )
				return;

			var undoItems = new List<UndoActionPropertiesChange.Item>();
			foreach( var effect in GetObjects<Component_RenderingEffect_CodeGenerated>() )
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
			var objects = GetObjects<Component_RenderingEffect_CodeGenerated>();

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
