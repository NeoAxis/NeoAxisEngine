//#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections;

namespace NeoAxis.Editor
{
	public class TerrainGeometryChangeUndoAction : UndoSystem.Action
	{
		Terrain terrain;

		BitArray added = new BitArray( 4097 * 4097 );
		Dictionary<int, float> values = new Dictionary<int, float>( 512 );

		//

		public TerrainGeometryChangeUndoAction( Terrain terrain )
		{
			this.terrain = terrain;
		}

		public Terrain Terrain
		{
			get { return terrain; }
		}

		internal protected override void DoUndo()
		{
			var updateRectangle = RectangleI.Cleared;

			var oldValues = new Dictionary<int, float>( values.Count );
			foreach( var pair in values )
			{
				var cellIndex = GetCellIndex( pair.Key );

				float coef = terrain.GetHeightWithoutPosition( cellIndex, false );
				oldValues.Add( pair.Key, coef );

				terrain.SetHeightWithoutPosition( cellIndex, pair.Value );

				updateRectangle.Add( cellIndex );
			}

			values = oldValues;

			terrain.SetNeedUpdateTilesByCellIndices( updateRectangle, Terrain.NeedUpdateEnum.Geometry | Terrain.NeedUpdateEnum.Collision );
		}

		internal protected override void DoRedo()
		{
			DoUndo();
		}

		internal protected override void Destroy()
		{
		}

		int GetIndex( Vector2I cellIndex )
		{
			return cellIndex.X * 4097 + cellIndex.Y;
		}

		Vector2I GetCellIndex( int index )
		{
			return new Vector2I( index / 4097, index % 4097 );
		}

		public void SaveValue( Vector2I cellIndex, float heightCoef )
		{
			int index = GetIndex( cellIndex );
			if( !added[ index ] )
			{
				added[ index ] = true;
				values.Add( index, heightCoef );
			}
		}

		public void RestoreOldValues()
		{
			DoUndo();
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class TerrainPaintChangeUndoAction : UndoSystem.Action
	{
		Terrain terrain;
		List<LayerItem> data = new List<LayerItem>();

		/////////////////////////////////////////

		class LayerItem
		{
			public PaintLayer layer;

			//!!!!достаточно byte
			//key: mask index, value: paint value
			public Dictionary<Vector2I, float> values = new Dictionary<Vector2I, float>( 512 );
		}

		/////////////////////////////////////////

		public TerrainPaintChangeUndoAction( Terrain terrain )
		{
			this.terrain = terrain;
		}

		public Terrain Terrain
		{
			get { return terrain; }
		}

		internal protected override void DoUndo()
		{
			var updateRectangle = RectangleI.Cleared;

			var oldItems = new List<LayerItem>();

			foreach( var item in data )
			{
				var oldValues = new Dictionary<Vector2I, float>( item.values.Count );
				foreach( var pair in item.values )
				{
					float value = item.layer.GetMaskValue( pair.Key );
					oldValues.Add( pair.Key, value );

					item.layer.SetMaskValue( pair.Key, pair.Value );

					updateRectangle.Add( pair.Key );
				}

				var oldItem = new LayerItem();
				oldItem.layer = item.layer;
				oldItem.values = oldValues;
				oldItems.Add( oldItem );
			}

			data = oldItems;

			terrain.SetNeedUpdateTilesByMaskIndices( updateRectangle, Terrain.NeedUpdateEnum.Layers );
		}

		internal protected override void DoRedo()
		{
			DoUndo();
		}

		internal protected override void Destroy()
		{
		}

		LayerItem GetOrCreateLayerItem( PaintLayer layer )
		{
			for( int n = 0; n < data.Count; n++ )
				if( data[ n ].layer == layer )
					return data[ n ];

			{
				var item = new LayerItem();
				item.layer = layer;
				data.Add( item );
				return item;
			}
		}

		public void SaveValue( PaintLayer layer, Vector2I maskIndex, float paintValue )
		{
			var item = GetOrCreateLayerItem( layer );

			if( !item.values.ContainsKey( maskIndex ) )
				item.values.Add( maskIndex, paintValue );
		}

		public void PerformUndo()
		{
			DoUndo();
		}
	}
}
//#endif