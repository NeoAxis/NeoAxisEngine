// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents an additional GUI under properties for pathfinding component.
	/// </summary>
	public class Component_Pathfinding_SettingsCell : SettingsCellProcedureUI
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

		Component_Pathfinding GetObject()
		{
			foreach( var obj in Provider.SelectedObjects )
			{
				var pathfinding = obj as Component_Pathfinding;
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

			var oldNavMeshData = pathfinding.NavMeshData;

			if( !pathfinding.BuildNavMesh( out var error ) )
			{
				Log.Error( error );
				return;
			}

			//undo
			{
				var property = (Metadata.Property)pathfinding.MetadataGetMemberBySignature( "property:NavMeshData" );

				var undoItems = new List<UndoActionPropertiesChange.Item>();
				undoItems.Add( new UndoActionPropertiesChange.Item( pathfinding, property, oldNavMeshData, null ) );
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
				var instance = new Component_Pathfinding_TestMode( (Component_Scene_DocumentWindow)sceneDocumentWindow, GetObject() );

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

			//var sceneDocumentWindow = Provider.DocumentWindow as Component_Scene_DocumentWindow;
			//if( sceneDocumentWindow == null )
			//	return;

			//if( sceneDocumentWindow.WorkareaModeName != "Pathfinding Test" )
			//{
			//	var instance = new Component_Pathfinding_TestMode( sceneDocumentWindow, GetObject() );
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
