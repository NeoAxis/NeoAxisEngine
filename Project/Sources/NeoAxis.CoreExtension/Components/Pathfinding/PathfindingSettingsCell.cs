// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents an additional GUI under properties for pathfinding component.
	/// </summary>
	public class PathfindingSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonBuild;
		ProcedureUI.Button buttonTest;

		TransformTool.ModeEnum transformToolModeRestore = TransformTool.ModeEnum.PositionRotation;
		//string workareaModeNameRestore = "";
		//DocumentWindowWithViewport.WorkareaModeClass workareaModeRestore;

		//

		protected override void OnInit()
		{
			buttonBuild = ProcedureForm.CreateButton( "Build" );
			buttonBuild.Click += ButtonBuild_Click;

			buttonTest = ProcedureForm.CreateButton( "Test" );
			buttonTest.Click += ButtonTest_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonBuild, buttonTest } );
		}

		Pathfinding GetObject()
		{
			foreach( var obj in Provider.SelectedObjects )
			{
				var pathfinding = obj as Pathfinding;
				if( pathfinding != null )
					return pathfinding;
			}
			return null;
		}

		private void ButtonBuild_Click( ProcedureUI.Button sender )
		{
			var pathfinding = GetObject();
			if( pathfinding == null )
				return;

			var oldPrecompiledData = pathfinding.PrecompiledData;

			if( !pathfinding.BuildPrecompiledData( out var error ) )
			{
				Log.Error( error );
				return;
			}

			//undo
			{
				var property = (Metadata.Property)pathfinding.MetadataGetMemberBySignature( "property:PrecompiledData" );

				var undoItems = new List<UndoActionPropertiesChange.Item>();
				undoItems.Add( new UndoActionPropertiesChange.Item( pathfinding, property, oldPrecompiledData, null ) );
				var undoAction = new UndoActionPropertiesChange( undoItems.ToArray() );

				Provider.DocumentWindow.Document.UndoSystem.CommitAction( undoAction );
				Provider.DocumentWindow.Document.Modified = true;
			}
		}

		private void ButtonTest_Click( ProcedureUI.Button sender )
		{
			var pathfinding = GetObject();
			if( pathfinding == null )
				return;

			//PropertyGet, PropertySet, MethodInvoke are used to remove reference to the WinForms dll

			var sceneDocumentWindow = (object)Provider.DocumentWindow;
			if( sceneDocumentWindow == null )
				return;

			if( sceneDocumentWindow.PropertyGet<string>( "WorkareaModeName" ) != "Pathfinding Test" )
			{
				var instance = new PathfindingTestMode( (SceneEditor)sceneDocumentWindow, GetObject() );

				sceneDocumentWindow.MethodInvoke( "WorkareaModeSet", "Pathfinding Test", instance );

				var transformTool = sceneDocumentWindow.PropertyGet<TransformTool>( "TransformTool" );
				transformToolModeRestore = transformTool.Mode;
				transformTool.Mode = TransformTool.ModeEnum.Undefined;
			}
			else
			{
				sceneDocumentWindow.MethodInvoke( "WorkareaModeSet", "", null );

				var transformTool = sceneDocumentWindow.PropertyGet<TransformTool>( "TransformTool" );
				transformTool.Mode = transformToolModeRestore;
			}

			//var sceneDocumentWindow = Provider.DocumentWindow as SceneEditor;
			//if( sceneDocumentWindow == null )
			//	return;

			//if( sceneDocumentWindow.WorkareaModeName != "Pathfinding Test" )
			//{
			//	var instance = new Pathfinding_TestMode( sceneDocumentWindow, GetObject() );
			//	sceneDocumentWindow.WorkareaModeSet( "Pathfinding Test", instance );

			//	transformToolModeRestore = sceneDocumentWindow.TransformTool.Mode;
			//	sceneDocumentWindow.TransformTool.Mode = TransformTool.ModeEnum.Undefined;
			//}
			//else
			//{
			//	sceneDocumentWindow.WorkareaModeSet( "" );
			//	sceneDocumentWindow.TransformTool.Mode = transformToolModeRestore;
			//}

		}
	}
}
#endif