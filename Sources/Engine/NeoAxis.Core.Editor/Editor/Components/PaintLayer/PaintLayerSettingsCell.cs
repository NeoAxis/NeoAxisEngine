#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class PaintLayerSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonImportMask;
		ProcedureUI.Button buttonExportMask;
		ProcedureUI.Button buttonFillMask;

		//

		protected override void OnInit()
		{
			buttonImportMask = ProcedureForm.CreateButton( EditorLocalization2.Translate( "PaintLayer", "Import Mask" ), ProcedureUI.Button.SizeEnum.Long );
			buttonImportMask.Click += ButtonImportMask_Click;
			buttonExportMask = ProcedureForm.CreateButton( EditorLocalization2.Translate( "PaintLayer", "Export Mask" ), ProcedureUI.Button.SizeEnum.Long );
			buttonExportMask.Click += ButtonExportMask_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonImportMask, buttonExportMask } );

			buttonFillMask = ProcedureForm.CreateButton( EditorLocalization2.Translate( "PaintLayer", "Fill Mask" ), ProcedureUI.Button.SizeEnum.Long );
			buttonFillMask.Click += ButtonFillMask_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonFillMask } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			var paintLayer = GetFirstObject<PaintLayer>();
			buttonFillMask.Enabled = paintLayer != null;
			buttonImportMask.Enabled = paintLayer != null;
			buttonExportMask.Enabled = paintLayer != null && paintLayer.Mask.Value != null && paintLayer.Mask.Value.Length != 0;
		}

		private void ButtonFillMask_Click( ProcedureUI.Button sender )
		{
			var paintLayer = GetFirstObject<PaintLayer>();
			if( paintLayer == null )
				return;

			var form = new OKCancelTextBoxForm( "Mask value", "1", "Fill Mask", delegate ( string text, ref string error )
			{
				if( !float.TryParse( text, out var value ) || value < 0 || value > 1 )
				{
					error = "Must be value between 0 and 1.";
					return false;
				}
				return true;
			},
			delegate ( string text, ref string error )
			{
				if( !float.TryParse( text, out var valueFloat ) )
					return false;

				var valueByte = (byte)( valueFloat * 255 );

				var oldMask = paintLayer.Mask;
				var property = (Metadata.Property)paintLayer.MetadataGetMemberBySignature( "property:Mask" );
				var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( paintLayer, property, oldMask ) );
				Provider.DocumentWindow.Document.CommitUndoAction( undoAction );

				byte[] newMask;
				if( oldMask.Value != null )
					newMask = new byte[ oldMask.Value.Length ];
				else
				{
					var terrain = paintLayer.Parent as Terrain;
					if( terrain != null )
						newMask = new byte[ terrain.GetPaintMaskSizeInteger() * terrain.GetPaintMaskSizeInteger() ];
					else
					{
						//!!!!
						newMask = new byte[ 1 ];
					}
				}

				for( int n = 0; n < newMask.Length; n++ )
					newMask[ n ] = valueByte;

				paintLayer.Mask = newMask;

				return true;
			} );

			form.ShowDialog();
		}

		private void ButtonImportMask_Click( ProcedureUI.Button obj )
		{
			var paintLayer = GetFirstObject<PaintLayer>();
			if( paintLayer == null )
				return;

			if( !EditorUtility2.ShowOpenFileDialog( false, "", null, out string fileName ) )
				return;
			//if( !EditorUtility.ShowOpenFileDialog( false, "", new[] { ("PNG files (*.png)", "*.png") }, out string fileName ) )
			//	return;

			if( !PaintLayer.LoadMask( fileName, out var mask, out var error ) )
			{
				EditorMessageBox.ShowWarning( error );
				return;
			}

			var property = (Metadata.Property)paintLayer.MetadataGetMemberBySignature( "property:" + nameof( PaintLayer.Mask ) );
			var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( paintLayer, property, paintLayer.Mask ) );

			paintLayer.Mask = mask;

			Provider.DocumentWindow.Document.CommitUndoAction( undoAction );
		}

		private void ButtonExportMask_Click( ProcedureUI.Button obj )
		{
			var paintLayer = GetFirstObject<PaintLayer>();
			if( paintLayer == null || paintLayer.Mask.Value == null )
				return;

			if( !EditorUtility2.ShowSaveFileDialog( "", "Mask.png", "PNG files (*.png)|*.png", out var fileName ) )
				return;

			if( !paintLayer.SaveMask( fileName, out var error ) )
				EditorMessageBox.ShowWarning( error );
			else
				ScreenNotifications2.Show( EditorLocalization2.Translate( "PaintLayer", "The image was created successfully." ) );
		}
	}
}

#endif