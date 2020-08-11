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
	public class Component_Terrain_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpdate;
		ProcedureUI.Button buttonResizeMasks;

		//

		protected override void OnInit()
		{
			buttonUpdate = ProcedureForm.CreateButton( EditorLocalization.Translate( "Terrain", "Update" ) );
			buttonUpdate.Click += ButtonUpdate_Click;

			buttonResizeMasks = ProcedureForm.CreateButton( EditorLocalization.Translate( "Terrain", "Resize Masks" ) );
			buttonResizeMasks.Click += ButtonResizeMasks_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate, buttonResizeMasks } );
		}

		Component_Terrain GetTerrain()
		{
			var terrains = GetObjects<Component_Terrain>();
			if( terrains.Length == 1 )
				return terrains[ 0 ];
			return null;
		}

		List<Component_PaintLayer> GetLayersToResizeMask( Component_Terrain terrain )
		{
			var result = new List<Component_PaintLayer>();
			foreach( var layer in terrain.GetComponents<Component_PaintLayer>() )
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

					layer.Mask = Component_PaintLayer.ResizeMask( layer.Mask, terrain.GetPaintMaskSizeInteger() );

					var property = (Metadata.Property)layer.MetadataGetMemberBySignature( "property:Mask" );
					var undoItem = new UndoActionPropertiesChange.Item( layer, property, oldValue );
					undoMultiAction.AddAction( new UndoActionPropertiesChange( undoItem ) );
				}

				if( undoMultiAction.Actions.Count != 0 )
					Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );
			}
		}
	}
}
