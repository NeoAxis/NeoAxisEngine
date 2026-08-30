// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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
		ProcedureUI.Button buttonInfo;
		ProcedureUI.Button buttonTest;
		TransformToolMode transformToolModeRestore = TransformToolMode.PositionRotation;

		//

		protected override void OnInit()
		{
			buttonBuild = ProcedureForm.CreateButton( "Build" );
			buttonBuild.Click += ButtonBuild_Click;

			buttonInfo = ProcedureForm.CreateButton( "Info" );
			buttonInfo.Click += ButtonInfo_Click;

			buttonTest = ProcedureForm.CreateButton( "Test" );
			buttonTest.Click += ButtonTest_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonBuild, buttonInfo } );
			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonTest } );
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

			pathfinding.StartFullUpdate();


			////var oldPrecompiledData = pathfinding.PrecompiledData;

			////if( !pathfinding.BuildPrecompiledData( out var error ) )
			////{
			////	Log.Error( error );
			////	return;
			////}

			//////undo
			////{
			////	var property = (Metadata.Property)pathfinding.MetadataGetMemberBySignature( "property:PrecompiledData" );

			////	var undoItems = new List<UndoActionPropertiesChange.Item>();
			////	undoItems.Add( new UndoActionPropertiesChange.Item( pathfinding, property, oldPrecompiledData, null ) );
			////	var undoAction = new UndoActionPropertiesChange( undoItems.ToArray() );

			////	Provider.DocumentWindow.Document.UndoSystem.CommitAction( undoAction );
			////	Provider.DocumentWindow.Document.Modified = true;
			////}
		}

		private void ButtonTest_Click( ProcedureUI.Button sender )
		{
			var pathfinding = GetObject();
			if( pathfinding == null )
				return;

			var sceneEditor = Provider.DocumentWindow as ISceneEditor;
			if( sceneEditor == null )
				return;

			if( sceneEditor.WorkareaModeName != "Pathfinding Test" )
			{
				var instance = new PathfindingTestMode( sceneEditor, GetObject() );
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

		private void ButtonInfo_Click( ProcedureUI.Button obj )
		{
			var pathfinding = GetObject();
			if( pathfinding == null )
				return;

			var lines = new List<string>();

			var backgroundThreadData = pathfinding.GetBackgroundThreadData();
			var dynamicNavMesh = backgroundThreadData?.dynamicNavMesh;
			var precompiledData = backgroundThreadData?.precompiledData;

			if( dynamicNavMesh != null && precompiledData != null )
			{
				var settings = precompiledData.buildSettings;
				var tileSize = settings.tileSize;
				var cellSize = settings.cellSize;

				lines.Add( $"Cell size: {cellSize}" );
				lines.Add( $"Cell height: {settings.cellHeight}" );
				lines.Add( $"Tile size: {tileSize}" );
				lines.Add( $"Tile size in units: {Math.Round( tileSize * cellSize, 5 )}" );

				if( precompiledData.staticGeometry != null )
					lines.Add( $"Bounds: {precompiledData.staticGeometry.bounds.GetSize()}" );

				lines.Add( $"Tiles: {dynamicNavMesh.VoxelTiles().Count}" );

				lines.Add( $"Dynamic obstacles: {backgroundThreadData.dynamicObstaclesCountLockFree}" );
			}
			else
			{
				lines.Add( "No navigation mesh." );
			}

			var text = "";
			foreach( var line in lines )
				text += line + "\r\n";

			EditorMessageBox.ShowInfo( text, "Pathfinding Info" );
		}
	}
}