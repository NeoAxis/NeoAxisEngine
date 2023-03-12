//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace NeoAxis.Editor
//{
//	public class CurveInSpacePointSettingsCell : SettingsCellProcedureUI
//	{
//		ProcedureUI.Button buttonAddHandles;
//		ProcedureUI.Button buttonRemoveHandles;

//		//

//		static string Translate( string text )
//		{
//			return EditorLocalization.Translate( "CurveInSpacePoint_SettingsCell", text );
//		}

//		protected override void OnInit()
//		{
//			buttonAddHandles = ProcedureForm.CreateButton( Translate( "Add Handles" ) );
//			buttonAddHandles.Click += ButtonAddHandles_Click;

//			buttonRemoveHandles = ProcedureForm.CreateButton( Translate( "Remove Handles" ) );
//			buttonRemoveHandles.Click += ButtonRemoveHandles_Click;

//			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonAddHandles, buttonRemoveHandles } );
//		}

//		public CurveInSpacePoint[] Points
//		{
//			get { return GetObjects<CurveInSpacePoint>(); }
//		}

//		protected override void OnUpdate()
//		{
//			base.OnUpdate();

//			var allowAdd = false;
//			var allowRemove = false;

//			foreach( var point in Points )
//			{
//				var handleCount = point.GetComponents<CurveInSpacePointHandle>().Length;
//				if( handleCount == 0 )
//					allowAdd = true;
//				if( handleCount != 0 )
//					allowRemove = true;
//			}

//			buttonAddHandles.Enabled = allowAdd;
//			buttonRemoveHandles.Enabled = allowRemove;
//		}

//		private void ButtonAddHandles_Click( ProcedureUI.Button sender )
//		{
//			//!!!!undo



//			//foreach( var mesh in GetObjects<Mesh>() )
//			//	mesh.ResultCompile();
//		}

//		private void ButtonRemoveHandles_Click( ProcedureUI.Button sender )
//		{
//			//!!!!undo



//			//var meshes = GetObjects<Mesh>();
//			//if( meshes.Length != 1 )
//			//	return;
//			//var mesh = meshes[ 0 ];

//			//if( !EditorUtility.ShowSaveFileDialog( "", "Mesh.fbx", "FBX files (*.fbx)|*.fbx", out var fileName ) )
//			//	return;

//			//if( !mesh.ExportToFBX( fileName, out var error ) )
//			//	EditorMessageBox.ShowWarning( error );
//		}
//	}
//}
