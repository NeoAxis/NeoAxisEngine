// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Component_Import3D_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonReimport;

		//


		static string Translate( string text )
		{
			return EditorLocalization.Translate( "Import3D", text );
		}

		protected override void OnInit()
		{
			buttonReimport = ProcedureForm.CreateButton( Translate( "Re-import" ) );
			buttonReimport.Click += ButtonReimport_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonReimport } );
		}

		void TryReimport()
		{
			var obj = Provider.SelectedObjects[ 0 ] as Component_Import3D;
			if( obj == null )
				return;

			var settings = new Component_Import3D.ReimportSettings();

			var form = new SpecifyParametersForm( Translate( "Re-import" ), settings );
			form.CheckHandler = delegate ( ref string error2 )
			{
				return true;
			};
			if( form.ShowDialog() != DialogResult.OK )
				return;


			//clear undo history
			Provider.DocumentWindow?.Document?.UndoSystem.Clear();

			if( !obj.DoUpdate( settings, out string error ) )
			{
				var virtualFileName = obj.ParentRoot.HierarchyController?.CreatedByResource?.Owner.Name;
				if( string.IsNullOrEmpty( virtualFileName ) )
					virtualFileName = "NO FILE NAME";
				var error2 = string.Format( Translate( "Unable to load or import resource \'{0}\'." ), virtualFileName ) + "\r\n\r\n" + error;
				Log.Error( error2 );
				return;
			}

			if( Provider?.DocumentWindow != null )
			{
				Provider.DocumentWindow.Document.Modified = true;

				//!!!!полезен вспомогательный метод/свойство чтобы получать CanvasBasedEditor
				var window = Provider.DocumentWindow as DocumentWindowWithViewport_CanvasBasedEditor;
				if( window != null )
				{
					var editor = window.Editor as Component_Import3D_Editor;
					editor?.NeedRecreateDisplayObject( true );
				}
				//var importDocumentWindow = Provider.DocumentWindow as Component_Import3D_Editor;
				//importDocumentWindow?.NeedRecreateDisplayObject( true );
			}
		}

		private void ButtonReimport_Click( ProcedureUI.Button sender )
		{
			TryReimport();
		}
	}
}
