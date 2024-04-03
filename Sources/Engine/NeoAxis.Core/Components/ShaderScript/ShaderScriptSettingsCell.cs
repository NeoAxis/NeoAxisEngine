//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;

//namespace NeoAxis.Editor
//{
//	public class ShaderScript_SettingsCell : SettingsCellProcedureUI
//	{
//		ProcedureUI.Button buttonUpdate;
//		ProcedureUI.Check checkAutoUpdate;

//		//

//		protected override void OnInitUI()
//		{
//			buttonUpdate = ProcedureForm.CreateButton( "Update" );
//			buttonUpdate.Click += ButtonUpdate_Click;

//			checkAutoUpdate = ProcedureForm.CreateCheck( "Auto update" );
//			checkAutoUpdate.Click += CheckAutoUpdate_Click;

//			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate, checkAutoUpdate } );
//		}

//		private void ButtonUpdate_Click( ProcedureUI.Button sender )
//		{
//			//!!!!
//			//foreach( var script in GetObjects<ShaderScript>() )
//			//	script.Update( true );
//		}

//		private void CheckAutoUpdate_Click( ProcedureUI.Check obj )
//		{
//			if( checkAutoUpdate.Checked == ProcedureUI.Check.CheckValue.Indeterminate )
//				return;

//			var undoItems = new List<UndoActionPropertiesChange.Item>();
//			foreach( var script in GetObjects<ShaderScript>() )
//			{
//				var oldValue = script.EditorAutoUpdate;

//				script.EditorAutoUpdate = checkAutoUpdate.Checked == ProcedureUI.Check.CheckValue.Checked;

//				var property = (Metadata.Property)script.MetadataGetMemberBySignature( "property:EditorAutoUpdate" );
//				undoItems.Add( new UndoActionPropertiesChange.Item( script, property, oldValue ) );
//			}

//			var undoAction = new UndoActionPropertiesChange( undoItems );
//			Provider.DocumentWindow.Document.CommitUndoAction( undoAction );
//		}

//		protected override void OnUpdate()
//		{
//			var objects = GetObjects<ShaderScript>();

//			if( objects.All( m => m.EditorAutoUpdate ) )
//				checkAutoUpdate.Checked = ProcedureUI.Check.CheckValue.Checked;
//			else if( objects.All( m => !m.EditorAutoUpdate ) )
//				checkAutoUpdate.Checked = ProcedureUI.Check.CheckValue.Unchecked;
//			else
//				checkAutoUpdate.Checked = ProcedureUI.Check.CheckValue.Indeterminate;

//			buttonUpdate.Enabled = checkAutoUpdate.Checked != ProcedureUI.Check.CheckValue.Checked;
//		}
//	}
//}
