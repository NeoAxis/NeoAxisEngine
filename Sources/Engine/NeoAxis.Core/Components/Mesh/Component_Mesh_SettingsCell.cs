// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public class Component_Mesh_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpdate;

		//

		protected override void OnInitUI()
		{
			buttonUpdate = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Update" ) );
			buttonUpdate.Click += ButtonUpdate_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate } );
		}

		private void ButtonUpdate_Click( ProcedureUI.Button sender )
		{
			foreach( var mesh in GetObjects<Component_Mesh>() )
				mesh.ResultCompile();
		}

		//private void buttonTempInfo_Click( object sender, EventArgs e )
		//{
		//foreach( var obj in Provider.SelectedObjects )
		//{
		//	var mesh = obj as Component_Mesh;
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
