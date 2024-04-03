#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public class GroupOfObjectsSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonClearAll;
		ProcedureUI.Button buttonClearItems;
		ProcedureUI.Button buttonUpdateAlignment;
		ProcedureUI.Button buttonStatistics;

		//

		string Translate( string text )
		{
			return EditorLocalization2.Translate( "GroupOfObjects", text );
		}

		protected override void OnInit()
		{
			buttonClearAll = ProcedureForm.CreateButton( Translate( "Clear" ), ProcedureUI.Button.SizeEnum.Long );
			buttonClearAll.Click += ButtonClearAll_Click;

			buttonClearItems = ProcedureForm.CreateButton( Translate( "Clear Objects" ), ProcedureUI.Button.SizeEnum.Long );
			buttonClearItems.Click += ButtonClearObjects_Click;

			buttonUpdateAlignment = ProcedureForm.CreateButton( Translate( "Update Alignment" ), ProcedureUI.Button.SizeEnum.Long );
			buttonUpdateAlignment.Click += ButtonUpdateAlignment_Click;

			buttonStatistics = ProcedureForm.CreateButton( Translate( "Statistics" ), ProcedureUI.Button.SizeEnum.Long );
			buttonStatistics.Click += ButtonStatistics_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonClearAll, buttonClearItems } );
			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdateAlignment, buttonStatistics } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			var groupOfObjects = GetObjects<GroupOfObjects>();
			buttonClearAll.Enabled = groupOfObjects.Any( obj => obj.ObjectsExists() || obj.Components.Count != 0 );
			buttonClearItems.Enabled = groupOfObjects.Any( obj => obj.ObjectsExists() );
			buttonUpdateAlignment.Enabled = groupOfObjects.Any( obj => obj.ObjectsExists() );

			buttonStatistics.Enabled = GetObjects<GroupOfObjects>().Length == 1;
		}

		private void ButtonClearAll_Click( ProcedureUI.Button sender )
		{
			if( EditorMessageBox.ShowQuestion( Translate( "Delete all objects and child components?" ), EMessageBoxButtons.YesNo ) != EDialogResult.Yes )
				return;

			var undoMultiAction = new UndoMultiAction();

			foreach( var groupOfObjects in GetObjects<GroupOfObjects>() )
			{
				var indexes = groupOfObjects.ObjectsGetAll();
				if( indexes.Count != 0 )
				{
					var action = new GroupOfObjectsUndo.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true );
					undoMultiAction.AddAction( action );
				}

				var components = groupOfObjects.GetComponents();
				undoMultiAction.AddAction( new UndoActionComponentCreateDelete( Provider.DocumentWindow.Document, components, false ) );
			}

			if( undoMultiAction.Actions.Count != 0 )
				Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );
		}

		private void ButtonClearObjects_Click( ProcedureUI.Button obj )
		{
			if( EditorMessageBox.ShowQuestion( Translate( "Delete all objects?" ), EMessageBoxButtons.YesNo ) != EDialogResult.Yes )
				return;

			var undoMultiAction = new UndoMultiAction();

			foreach( var groupOfObjects in GetObjects<GroupOfObjects>() )
			{
				var indexes = groupOfObjects.ObjectsGetAll();
				if( indexes.Count != 0 )
				{
					var action = new GroupOfObjectsUndo.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true );
					undoMultiAction.AddAction( action );
				}
			}

			if( undoMultiAction.Actions.Count != 0 )
				Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );
		}

		private unsafe void ButtonUpdateAlignment_Click( ProcedureUI.Button sender )
		{
			var undoMultiAction = new UndoMultiAction();
			foreach( var groupOfObjects in GetObjects<GroupOfObjects>() )
			{
				foreach( var element in groupOfObjects.GetComponents<GroupOfObjectsElement>() )
				{
					var elementMesh = element as GroupOfObjectsElement_Mesh;
					if( elementMesh != null )
						elementMesh.UpdateAlignment( undoMultiAction );

					var elementSurface = element as GroupOfObjectsElement_Surface;
					if( elementSurface != null )
					{
						elementSurface.UpdateVariationsAndTransform( false, undoMultiAction, false, true );
						//elementSurface.UpdateAlignment( undoMultiAction );
					}
				}

				groupOfObjects.CreateSectors();
			}
			if( undoMultiAction.Actions.Count != 0 )
				Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );

			//var undoMultiAction = new UndoMultiAction();
			//foreach( var groupOfObjects in GetObjects<GroupOfObjects>() )
			//	groupOfObjects.UpdateItems( null, undoMultiAction );
			//if( undoMultiAction.Actions.Count != 0 )
			//	Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );

			//int counter = 0;
			//int counter2 = 0;

			//foreach( var groupOfObjects in GetObjects<GroupOfObjects>() )
			//{
			//	{
			//		int count = 50;

			//		var data = new byte[ count * sizeof( GroupOfObjects.ObjectMesh ) ];
			//		fixed ( byte* pData = data )
			//		{
			//			GroupOfObjects.ObjectMesh* pMeshObjects = (GroupOfObjects.ObjectMesh*)pData;

			//			for( int n = 0; n < count; n++ )
			//			{
			//				//pMeshObjects[ n ] = new GroupOfObjects.ObjectMesh( 3, (ushort)( counter2 % 3 ),
			//				//	new Vector3( n * 3 - 25, counter * 3, 0 ), QuaternionF.Identity, Vector3F.One,
			//				//	new Vector3( n * 3 - 25, counter * 3, 0 ), QuaternionF.Identity, Vector3F.One );

			//				//pMeshObjects[ n ] = new GroupOfObjects.ObjectMesh( (ushort)( counter2 % 3 ), 0,
			//				//	new Vector3( n * 3 - 25, counter * 3, 0 ), QuaternionF.Identity, Vector3F.One,
			//				//	new Vector3( n * 3 - 25, counter * 3, 0 ), QuaternionF.Identity, Vector3F.One );
			//				counter2++;
			//			}

			//			groupOfObjects.ObjectsMeshAdd( (GroupOfObjects.ObjectMesh*)pData, count );
			//			//ObjectsMeshSet( (ObjectMesh*)pData, 50 );
			//		}
			//	}

			//	counter++;
			//}
		}

		private unsafe void ButtonStatistics_Click( ProcedureUI.Button sender )
		{
			var groupOfObjects = GetObjects<GroupOfObjects>();
			if( groupOfObjects.Length != 1 )
				return;

			var groupOfObject = groupOfObjects[ 0 ];
			var indexes = groupOfObject.ObjectsGetAll();
			var objectSize = sizeof( GroupOfObjects.Object );

			var lines = new List<string>();
			long totalCount = 0;

			foreach( var element in groupOfObject.GetComponents<GroupOfObjectsElement>() )
			{
				long count = 0;
				foreach( var index in indexes )
				{
					ref var data = ref groupOfObject.ObjectGetData( index );
					if( data.Element == element.Index )
						count++;
				}

				lines.Add( $"{element}: {count }" );
				//lines.Add( string.Format( $"Element: {element}, Objects: {count }, Size {count * objectSize / 1024 / 1024} MB.\r\n" ) );
				totalCount += count;
			}

			lines.Add( "" );
			lines.Add( string.Format( Translate( "Total objects: {0}" ), totalCount ) );
			lines.Add( string.Format( Translate( "Total data size: {0} MB." ), totalCount * objectSize / 1024 / 1024 ) );

			//lines.Add( string.Format( $"Total: Objects: {totalCount}, Size {totalCount * objectSize / 1024 / 1024} MB." ) );

			string text = "";
			foreach( var line in lines )
			{
				if( text != "" )
					text += "\r\n";
				text += line;
			}

			EditorMessageBox.ShowInfo( text );
		}

	}
}

#endif