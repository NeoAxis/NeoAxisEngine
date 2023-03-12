#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class MeshSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpdate;
		ProcedureUI.Button buttonExport;
		ProcedureUI.Button buttonBuildStructure;
		ProcedureUI.Button buttonDeleteStructure;

		//

		protected override void OnInit()
		{
			buttonUpdate = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Update" ) );
			buttonUpdate.Click += ButtonUpdate_Click;

			buttonExport = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Export to FBX" ) );
			buttonExport.Click += ButtonExport_Click;

			buttonBuildStructure = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Build Structure" ) );
			buttonBuildStructure.Click += ButtonBuildStructure_Click;

			buttonDeleteStructure = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Delete Structure" ) );
			buttonDeleteStructure.Click += ButtonDeleteStructure_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate, buttonExport } );
			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonBuildStructure, buttonDeleteStructure } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			Mesh oneMesh = null;
			if( GetObjects<Mesh>().Length == 1 )
				oneMesh = GetObjects<Mesh>()[ 0 ];

			buttonExport.Enabled = oneMesh != null;
			buttonBuildStructure.Enabled = oneMesh != null && oneMesh.Structure == null;
			buttonDeleteStructure.Enabled = oneMesh != null && oneMesh.Structure != null;
		}

		private void ButtonUpdate_Click( ProcedureUI.Button sender )
		{
			foreach( var mesh in GetObjects<Mesh>() )
				mesh.PerformResultCompile();
		}

		private void ButtonExport_Click( ProcedureUI.Button sender )
		{
			var meshes = GetObjects<Mesh>();
			if( meshes.Length != 1 )
				return;
			var mesh = meshes[ 0 ];

			if( !EditorUtility.ShowSaveFileDialog( "", "Mesh.fbx", "FBX files (*.fbx)|*.fbx", out var fileName ) )
				return;

			if( !EditorAssemblyInterface.Instance.ExportToFBX( mesh, fileName, out var error ) )
				EditorMessageBox.ShowWarning( error );
			//if( !mesh.ExportToFBX( fileName, out var error ) )
			//	EditorMessageBox.ShowWarning( error );
		}

		private void ButtonBuildStructure_Click( ProcedureUI.Button sender )
		{
			var meshes = GetObjects<Mesh>();
			if( meshes.Length != 1 )
				return;
			var mesh = meshes[ 0 ];

			var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Mesh.Structure ) );
			var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, mesh.Structure ) );

			mesh.BuildStructure();

			Provider.DocumentWindow.Document.CommitUndoAction( undoAction );
		}

		private void ButtonDeleteStructure_Click( ProcedureUI.Button sender )
		{
			var meshes = GetObjects<Mesh>();
			if( meshes.Length != 1 )
				return;
			var mesh = meshes[ 0 ];

			if( EditorMessageBox.ShowQuestion( EditorLocalization.Translate( "General", "Delete structure?" ), EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
			{
				var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Mesh.Structure ) );
				var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, mesh.Structure ) );

				mesh.Structure = null;

				Provider.DocumentWindow.Document.CommitUndoAction( undoAction );
			}
		}

		//private void buttonTempInfo_Click( object sender, EventArgs e )
		//{
		//foreach( var obj in Provider.SelectedObjects )
		//{
		//	var mesh = obj as Mesh;
		//	if( mesh != null )
		//	{
		//		List<string> lines = new List<string>();

		//		lines.Add( "Structure:" );
		//		var structure = mesh.VertexStructure.Value;
		//		if( structure != null )
		//		{
		//			foreach( var elem in structure )
		//				lines.Add( "    " + elem.ToString() );
		//		}
		//		else
		//			lines.Add( "    (null)" );
		//		var v = mesh.Vertices.Value;
		//		var i = mesh.Indices.Value;
		//		lines.Add( "Vertices: " + ( v != null ? v.Length.ToString() : "(null)" ) );
		//		lines.Add( "Indices: " + ( i != null ? i.Length.ToString() : "(null)" ) );

		//		string text = "";
		//		foreach( var line in lines )
		//			text += line + "\r\n";

		//		EditorMessageBox.ShowInfo( text );
		//	}
		//}
		//}
	}
}

#endif