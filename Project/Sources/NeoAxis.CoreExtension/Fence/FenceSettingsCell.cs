// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis.Editor
{
	public class FenceSettingsCell : SettingsCellProcedureUI
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
			foreach( var c in GetObjects<Fence>() )
				c.Update();
		}

		private void CheckAutoUpdate_Click( ProcedureUI.Check obj )
		{
			if( checkAutoUpdate.Checked == ProcedureUI.Check.CheckValue.Indeterminate )
				return;

			var undoItems = new List<UndoActionPropertiesChange.Item>();
			foreach( var fence in GetObjects<Fence>() )
			{
				var oldValue = fence.EditorAutoUpdate;

				fence.EditorAutoUpdate = checkAutoUpdate.Checked == ProcedureUI.Check.CheckValue.Checked;

				var property = (Metadata.Property)fence.MetadataGetMemberBySignature( "property:EditorAutoUpdate" );
				undoItems.Add( new UndoActionPropertiesChange.Item( fence, property, oldValue ) );
			}

			var undoAction = new UndoActionPropertiesChange( undoItems );
			Provider.DocumentWindow.Document.CommitUndoAction( undoAction );
		}

		protected override void OnUpdate()
		{
			var objects = GetObjects<Fence>();

			if( objects.All( m => m.EditorAutoUpdate ) )
				checkAutoUpdate.Checked = ProcedureUI.Check.CheckValue.Checked;
			else if( objects.All( m => !m.EditorAutoUpdate ) )
				checkAutoUpdate.Checked = ProcedureUI.Check.CheckValue.Unchecked;
			else
				checkAutoUpdate.Checked = ProcedureUI.Check.CheckValue.Indeterminate;
		}
	}
}
#endif