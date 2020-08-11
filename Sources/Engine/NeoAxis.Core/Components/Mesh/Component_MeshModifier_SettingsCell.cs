// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NeoAxis.Editor
{
	public class Component_MeshModifier_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonBakeIntoMesh;

		//

		string Translate( string text )
		{
			return EditorLocalization.Translate( "MeshModifier", text );
		}

		protected override void OnInit()
		{
			buttonBakeIntoMesh = ProcedureForm.CreateButton( Translate( "Bake into Mesh" ) );
			buttonBakeIntoMesh.Click += ButtonBakeIntoMesh_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonBakeIntoMesh } );
		}

		Component_MeshModifier[] GetMeshModifiers()
		{
			return GetObjects<Component_MeshModifier>().Where( m => m.Parent as Component_Mesh != null ).ToArray();
		}

		private void ButtonBakeIntoMesh_Click( ProcedureUI.Button sender )
		{
			var document = Provider.DocumentWindow.Document;
			var undoMultiAction = new UndoMultiAction();

			var modifiers = GetMeshModifiers();
			if( modifiers.Length != 0 )
			{
				string text;
				if( modifiers.Length > 1 )
					text = Translate( "Bake selected geometries into the mesh?" );
				else
					text = Translate( "Bake selected geometry into the mesh?" );

				if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.OK )
				{
					//bake
					foreach( var modifier in modifiers )
						modifier.BakeIntoMesh( document, undoMultiAction );

					//delete and add to undo
					undoMultiAction.AddAction( new UndoActionComponentCreateDelete( document, modifiers, false ) );

					if( undoMultiAction.Actions.Count != 0 )
						document.CommitUndoAction( undoMultiAction );
				}
			}
		}

		protected override void OnUpdate()
		{
			buttonBakeIntoMesh.Enabled = GetMeshModifiers().Length != 0;
		}
	}
}
