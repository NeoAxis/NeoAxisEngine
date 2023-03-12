#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace NeoAxis.Editor
{
	public class TerrainSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpdate;
		ProcedureUI.Button buttonResizeMasks;
		ProcedureUI.Button buttonImportHeightmap;
		ProcedureUI.Button buttonExportHeightmap;

		//

		protected override void OnInit()
		{
			buttonUpdate = ProcedureForm.CreateButton( EditorLocalization.Translate( "Terrain", "Update" ), ProcedureUI.Button.SizeEnum.Long );
			buttonUpdate.Click += ButtonUpdate_Click;

			buttonResizeMasks = ProcedureForm.CreateButton( EditorLocalization.Translate( "Terrain", "Resize Masks" ), ProcedureUI.Button.SizeEnum.Long );
			buttonResizeMasks.Click += ButtonResizeMasks_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate, buttonResizeMasks } );

			buttonImportHeightmap = ProcedureForm.CreateButton( EditorLocalization.Translate( "Terrain", "Import Heightmap" ), ProcedureUI.Button.SizeEnum.Long );
			buttonImportHeightmap.Click += ButtonImportHeightmap_Click;

			buttonExportHeightmap = ProcedureForm.CreateButton( EditorLocalization.Translate( "Terrain", "Export Heightmap" ), ProcedureUI.Button.SizeEnum.Long );
			buttonExportHeightmap.Click += ButtonExportHeightmap_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonImportHeightmap, buttonExportHeightmap } );
		}

		Terrain GetTerrain()
		{
			var terrains = GetObjects<Terrain>();
			if( terrains.Length == 1 )
				return terrains[ 0 ];
			return null;
		}

		List<PaintLayer> GetLayersToResizeMask( Terrain terrain )
		{
			var result = new List<PaintLayer>();
			foreach( var layer in terrain.GetComponents<PaintLayer>() )
			{
				if( layer.Enabled && layer.Mask.Value != null && layer.Mask.Value.Length != 0 )
				{
					if( layer.Mask.Value.Length != terrain.GetPaintMaskSizeInteger() * terrain.GetPaintMaskSizeInteger() )
						result.Add( layer );
				}
			}
			return result;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			var terrain = GetTerrain();
			buttonUpdate.Enabled = terrain != null;
			buttonResizeMasks.Enabled = terrain != null && GetLayersToResizeMask( terrain ).Count != 0;
			buttonImportHeightmap.Enabled = terrain != null;
			buttonExportHeightmap.Enabled = terrain != null;
		}

		private void ButtonUpdate_Click( ProcedureUI.Button sender )
		{
			var terrain = GetTerrain();
			if( terrain == null )
				return;

			terrain.UpdateRenderingAndCollisionData( true );
		}

		private void ButtonResizeMasks_Click( ProcedureUI.Button sender )
		{
			var terrain = GetTerrain();
			if( terrain == null )
				return;

			var layers = GetLayersToResizeMask( terrain );
			if( layers.Count == 0 )
				return;

			var text = string.Format( EditorLocalization.Translate( "Terrain", "Resize masks of selected layers to {0}x{0}?" ), terrain.GetPaintMaskSizeInteger() );
			if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.OK )
			{
				var undoMultiAction = new UndoMultiAction();

				foreach( var layer in layers )
				{
					var oldValue = layer.Mask;

					layer.Mask = PaintLayer.ResizeMask( layer.Mask, terrain.GetPaintMaskSizeInteger() );

					var property = (Metadata.Property)layer.MetadataGetMemberBySignature( "property:" + nameof( PaintLayer.Mask ) );
					var undoItem = new UndoActionPropertiesChange.Item( layer, property, oldValue );
					undoMultiAction.AddAction( new UndoActionPropertiesChange( undoItem ) );
				}

				if( undoMultiAction.Actions.Count != 0 )
					Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );
			}
		}

		private void ButtonImportHeightmap_Click( ProcedureUI.Button obj )
		{
			var terrain = GetTerrain();
			if( terrain == null )
				return;

			if( !EditorUtility.ShowOpenFileDialog( false, "", new[] { ("EXR files (*.exr)", "*.exr") }, out string fileName ) )
				return;

			if( !Terrain.LoadHeightmapBuffer( fileName, terrain.GetHeightmapSizeAsInteger(), out var heightmapBuffer, out var error ) )
			{
				EditorMessageBox.ShowWarning( error );
				return;
			}

			var property = (Metadata.Property)terrain.MetadataGetMemberBySignature( "property:" + nameof( Terrain.HeightmapBuffer ) );
			var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( terrain, property, terrain.HeightmapBuffer ) );

			terrain.HeightmapBuffer = heightmapBuffer;

			Provider.DocumentWindow.Document.CommitUndoAction( undoAction );
		}

		private void ButtonExportHeightmap_Click( ProcedureUI.Button obj )
		{
			var terrain = GetTerrain();
			if( terrain == null )
				return;

			if( !EditorUtility.ShowSaveFileDialog( "", "Heightmap.exr", "EXR files (*.exr)|*.exr", out var fileName ) )
				return;

			if( !terrain.SaveHeightmapBuffer( fileName, out var error ) )
				EditorMessageBox.ShowWarning( error );
			else
				ScreenNotifications.Show( EditorLocalization.Translate( "Terrain", "The image was created successfully." ) );
		}
	}
}

#endif