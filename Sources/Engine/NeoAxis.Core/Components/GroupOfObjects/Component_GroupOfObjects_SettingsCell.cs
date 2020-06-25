// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public class Component_GroupOfObjects_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonClearAll;
		ProcedureUI.Button buttonClearItems;
		ProcedureUI.Button buttonUpdateAlignment;
		ProcedureUI.Button buttonStatistics;

		//

		string Translate( string text )
		{
			return EditorLocalization.Translate( "GroupOfObjects", text );
		}

		protected override void OnInitUI()
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

			var groupOfObjects = GetObjects<Component_GroupOfObjects>();
			buttonClearAll.Enabled = groupOfObjects.Any( obj => obj.ObjectsExists() || obj.Components.Count != 0 );
			buttonClearItems.Enabled = groupOfObjects.Any( obj => obj.ObjectsExists() );
			buttonUpdateAlignment.Enabled = groupOfObjects.Any( obj => obj.ObjectsExists() );

			buttonStatistics.Enabled = GetObjects<Component_GroupOfObjects>().Length == 1;
		}

		private void ButtonClearAll_Click( ProcedureUI.Button sender )
		{
			if( EditorMessageBox.ShowQuestion( Translate( "Delete all objects and child components?" ), MessageBoxButtons.YesNo ) != DialogResult.Yes )
				return;

			var undoMultiAction = new UndoMultiAction();

			foreach( var groupOfObjects in GetObjects<Component_GroupOfObjects>() )
			{
				var indexes = groupOfObjects.ObjectsGetAll();
				if( indexes.Count != 0 )
				{
					var action = new Component_GroupOfObjects_Editor.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true );
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
			if( EditorMessageBox.ShowQuestion( Translate( "Delete all objects?" ), MessageBoxButtons.YesNo ) != DialogResult.Yes )
				return;

			var undoMultiAction = new UndoMultiAction();

			foreach( var groupOfObjects in GetObjects<Component_GroupOfObjects>() )
			{
				var indexes = groupOfObjects.ObjectsGetAll();
				if( indexes.Count != 0 )
				{
					var action = new Component_GroupOfObjects_Editor.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true );
					undoMultiAction.AddAction( action );
				}
			}

			if( undoMultiAction.Actions.Count != 0 )
				Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );
		}

		private unsafe void ButtonUpdateAlignment_Click( ProcedureUI.Button sender )
		{
			var undoMultiAction = new UndoMultiAction();
			foreach( var groupOfObjects in GetObjects<Component_GroupOfObjects>() )
			{
				foreach( var element in groupOfObjects.GetComponents<Component_GroupOfObjectsElement>() )
				{
					var elementMesh = element as Component_GroupOfObjectsElement_Mesh;
					if( elementMesh != null )
						elementMesh.UpdateAlignment( undoMultiAction );

					var elementSurface = element as Component_GroupOfObjectsElement_Surface;
					if( elementSurface != null )
						elementSurface.UpdateAlignment( undoMultiAction );
				}

				groupOfObjects.CreateSectors();
			}
			if( undoMultiAction.Actions.Count != 0 )
				Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );

			//var undoMultiAction = new UndoMultiAction();
			//foreach( var groupOfObjects in GetObjects<Component_GroupOfObjects>() )
			//	groupOfObjects.UpdateItems( null, undoMultiAction );
			//if( undoMultiAction.Actions.Count != 0 )
			//	Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );

			//int counter = 0;
			//int counter2 = 0;

			//foreach( var groupOfObjects in GetObjects<Component_GroupOfObjects>() )
			//{
			//	{
			//		int count = 50;

			//		var data = new byte[ count * sizeof( Component_GroupOfObjects.ObjectMesh ) ];
			//		fixed ( byte* pData = data )
			//		{
			//			Component_GroupOfObjects.ObjectMesh* pMeshObjects = (Component_GroupOfObjects.ObjectMesh*)pData;

			//			for( int n = 0; n < count; n++ )
			//			{
			//				//pMeshObjects[ n ] = new Component_GroupOfObjects.ObjectMesh( 3, (ushort)( counter2 % 3 ),
			//				//	new Vector3( n * 3 - 25, counter * 3, 0 ), QuaternionF.Identity, Vector3F.One,
			//				//	new Vector3( n * 3 - 25, counter * 3, 0 ), QuaternionF.Identity, Vector3F.One );

			//				//pMeshObjects[ n ] = new Component_GroupOfObjects.ObjectMesh( (ushort)( counter2 % 3 ), 0,
			//				//	new Vector3( n * 3 - 25, counter * 3, 0 ), QuaternionF.Identity, Vector3F.One,
			//				//	new Vector3( n * 3 - 25, counter * 3, 0 ), QuaternionF.Identity, Vector3F.One );
			//				counter2++;
			//			}

			//			groupOfObjects.ObjectsMeshAdd( (Component_GroupOfObjects.ObjectMesh*)pData, count );
			//			//ObjectsMeshSet( (ObjectMesh*)pData, 50 );
			//		}
			//	}

			//	counter++;
			//}
		}

		private unsafe void ButtonStatistics_Click( ProcedureUI.Button sender )
		{
			var groupOfObjects = GetObjects<Component_GroupOfObjects>();
			if( groupOfObjects.Length != 1 )
				return;

			var groupOfObject = groupOfObjects[ 0 ];
			var indexes = groupOfObject.ObjectsGetAll();
			var objectSize = sizeof( Component_GroupOfObjects.Object );

			var lines = new List<string>();
			long totalCount = 0;

			foreach( var element in groupOfObject.GetComponents<Component_GroupOfObjectsElement>() )
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
