// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class TrafficSystemSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpdate;
		ProcedureUI.Button buttonTest;
		TransformToolMode transformToolModeRestore = TransformToolMode.PositionRotation;

		//

		protected override void OnInit()
		{
			buttonUpdate = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Update" ) );
			buttonUpdate.Click += ButtonUpdate_Click;

			buttonTest = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Path Test" ) );
			buttonTest.Click += ButtonTest_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate, buttonTest } );
		}

		private void ButtonUpdate_Click( ProcedureUI.Button sender )
		{
			var system = GetFirstObject<TrafficSystem>();
			system.UpdateObjects();
		}

		private void ButtonTest_Click( ProcedureUI.Button sender )
		{
			var system = GetFirstObject<TrafficSystem>();

			var sceneEditor = Provider.DocumentWindow as ISceneEditor;
			if( sceneEditor == null )
				return;

			if( sceneEditor.WorkareaModeName != "Pathfinding Test" )
			{
				var instance = new TrafficSystemTestMode( sceneEditor, system );
				sceneEditor.WorkareaModeSet( "Pathfinding Test", instance );

				transformToolModeRestore = sceneEditor.TransformTool.Mode;
				sceneEditor.TransformTool.Mode = TransformToolMode.Undefined;
			}
			else
			{
				sceneEditor.WorkareaModeSet( "", null );

				sceneEditor.TransformTool.Mode = transformToolModeRestore;
			}
		}
	}
}