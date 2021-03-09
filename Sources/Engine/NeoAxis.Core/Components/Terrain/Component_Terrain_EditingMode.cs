// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public class Component_Terrain_EditingMode : Component_Scene_DocumentWindow.WorkareaModeClass_Scene
	{
		ModeEnum mode;
		object modeExtensionData;
		Component_Scene scene;

		bool toolModify;
		Vector3 toolModifyStartPosition;
		float toolModifyStartMaskValue;

		public class NeedUpdateRectangleItem
		{
			public Component_Terrain Terrain;
			public RectangleI Rectangle;
		}
		List<NeedUpdateRectangleItem> needUpdateRectangle = new List<NeedUpdateRectangleItem>();

		//undo
		List<Component_Terrain_GeometryChangeUndoAction> geometryChangeUndoActions = new List<Component_Terrain_GeometryChangeUndoAction>();
		List<UndoActionPropertiesChange> paintSetPropertyUndoActions = new List<UndoActionPropertiesChange>();
		List<Component_Terrain_PaintChangeUndoAction> paintChangeUndoActions = new List<Component_Terrain_PaintChangeUndoAction>();

		const float toolRenderOffset = 0.2f;

		///////////////////////////////////////////

		public enum ModeEnum
		{
			GeometryRaise,
			GeometryLower,
			GeometrySmooth,
			GeometryFlatten,
			PaintPaint,
			PaintClear,
			PaintSmooth,
			PaintFlatten,
			//HoleAdd,
			//HoleRemove,

			Extension,
		}

		///////////////////////////////////////////

		public enum ToolShape
		{
			Circle,
			Square,
		}

		///////////////////////////////////////////

		public Component_Terrain_EditingMode( Component_Scene_DocumentWindow documentWindow, ModeEnum mode, object modeExtensionData = null )
			: base( documentWindow )
		{
			this.mode = mode;
			this.modeExtensionData = modeExtensionData;

			scene = documentWindow.Scene;
		}

		public ModeEnum Mode { get { return mode; } }
		public object ModeExtensionData { get { return modeExtensionData; } }
		public Component_Scene Scene { get { return scene; } }
		public bool ToolModify { get { return toolModify; } }
		public Vector3 ToolModifyStartPosition { get { return toolModifyStartPosition; } }
		public float ToolModifyStartMaskValue { get { return toolModifyStartMaskValue; } }
		public List<NeedUpdateRectangleItem> NeedUpdateRectangle { get { return needUpdateRectangle; } }
		public List<Component_Terrain_GeometryChangeUndoAction> GeometryChangeUndoActions { get { return geometryChangeUndoActions; } }
		public List<UndoActionPropertiesChange> PaintSetPropertyUndoActions { get { return paintSetPropertyUndoActions; } }
		public List<Component_Terrain_PaintChangeUndoAction> PaintChangeUndoActions { get { return paintChangeUndoActions; } }

		protected override void OnDestroy()
		{
			if( toolModify )
				StopToolModify( false );
		}

		protected override bool OnKeyDown( Viewport viewport, KeyEvent e )
		{
			if( toolModify && e.Key == EKeys.Escape )
			{
				StopToolModify( true );
				return true;
			}

			//disable workarea mode by Space or Escape
			if( ( e.Key == EKeys.Space || e.Key == EKeys.Escape ) && !toolModify )
			{
				DocumentWindow.ResetWorkareaMode();
				return true;
			}

			//disable shortcuts
			if( toolModify )
				return true;

			return false;
		}

		protected override bool OnMouseDown( Viewport viewport, EMouseButtons button )
		{
			if( button == EMouseButtons.Left )
			{
				if( GetToolPosition( viewport, out var terrain, out toolModifyStartPosition ) )
				{
					var layer = DocumentWindow.TerrainPaintLayersGetSelected();

					if( IsCurrentPaintTool() && layer != null )
					{
						if( layer.Mask.Value != null && layer.Mask.Value.Length != 0 )
						{
							if( layer.Mask.Value.Length != terrain.GetPaintMaskSizeInteger() * terrain.GetPaintMaskSizeInteger() )
							{
								EditorMessageBox.ShowWarning( EditorLocalization.Translate( "Terrain", "Unable to paint to selected layer because Mask size of the layer and MaskSize of the terrain are not equal." ) );
								return true;
							}
						}
					}

					if( mode == ModeEnum.PaintFlatten )
					{
						if( layer != null )
						{
							Vector2I maskIndex = terrain.GetMaskIndexByPosition( toolModifyStartPosition.ToVector2() );
							toolModifyStartMaskValue = layer.GetMaskValue( maskIndex );
						}
					}

					toolModify = true;
					return true;
				}
			}

			return false;
		}

		protected override bool OnMouseUp( Viewport viewport, EMouseButtons button )
		{
			if( button == EMouseButtons.Left )
			{
				if( toolModify )
				{
					StopToolModify( false );
					return true;
				}
			}

			return false;
		}

		protected override void OnTick( Viewport viewport, double delta )
		{
			if( toolModify && !viewport.IsMouseButtonPressed( EMouseButtons.Left ) )
				StopToolModify( false );

			if( toolModify )
			{
				if( IsCurrentGeometryTool() )
					ToolPutTickGeometry( viewport, delta );
				else if( IsCurrentPaintTool() )
					ToolPutTickPaint( viewport, delta );
				//else if( IsCurrentHoleTool() )
				//	ToolPutHole();
			}
		}

		protected override void OnUpdateBeforeOutput( Viewport viewport )
		{
			RenderTool( viewport );
		}

		protected virtual bool GetToolPosition( Viewport viewport, out Component_Terrain terrain, out Vector3 center )
		{
			if( !viewport.MouseRelativeMode )
			{
				var ray = viewport.CameraSettings.GetRayByScreenCoordinates( viewport.MousePosition );

				if( Component_Terrain.GetTerrainByRay( scene, ray, out terrain, out center ) )
					return true;
			}

			terrain = null;
			center = Vector3.Zero;
			return false;
		}

		protected virtual void RenderToolCircle( Viewport viewport, Component_Terrain terrain, Vector2 center )
		{
			var radius = Component_Scene_DocumentWindow.TerrainToolRadius;

			const double step = Math.PI / 32;
			Vector3 lastPos = Vector3.Zero;
			for( double angle = 0; angle <= Math.PI * 2 + step / 2; angle += step )
			{
				var pos = new Vector3(
					center.X + Math.Cos( angle ) * radius,
					center.Y + Math.Sin( angle ) * radius, 0 );
				pos.Z = terrain.GetHeight( pos.ToVector2(), false );

				if( angle != 0 )
					viewport.Simple3DRenderer.AddLine( lastPos + new Vector3( 0, 0, toolRenderOffset ), pos + new Vector3( 0, 0, toolRenderOffset ) );

				lastPos = pos;
			}
		}

		protected virtual void RenderToolLine( Viewport viewport, Component_Terrain terrain, Vector2 start, Vector2 end, int stepCount )
		{
			Vector2 diff = end - start;
			Vector2 dir = diff;
			var len = dir.Normalize();

			Vector3 lastPos = Vector3.Zero;
			for( int n = 0; n <= stepCount; n++ )
			{
				Vector3 pos = new Vector3(
					start.X + dir.X * ( (float)n / stepCount * len ),
					start.Y + dir.Y * ( (float)n / stepCount * len ), 0 );
				pos.Z = terrain.GetHeight( pos.ToVector2(), false );
				//if( selectedTerrain.HeightmapTerrainManager != null && HeightmapTerrainManager.Instance != null )
				//{
				//	float z;
				//	if( HeightmapTerrainManager.Instance.GetHeight( pos.ToVector2(), false, out z ) )
				//		pos.Z = z;
				//	else
				//		pos.Z = float.MinValue;
				//}
				//else
				//	pos.Z = selectedTerrain.GetHeight( pos.ToVector2(), false );

				if( n != 0 )
				{
					if( lastPos.Z != double.MinValue && pos.Z != double.MinValue )
						viewport.Simple3DRenderer.AddLine( lastPos + new Vector3( 0, 0, toolRenderOffset ), pos + new Vector3( 0, 0, toolRenderOffset ) );
				}

				lastPos = pos;
			}
		}

		protected virtual void RenderToolSquare( Viewport viewport, Component_Terrain terrain, Vector2 center )
		{
			var radius = Component_Scene_DocumentWindow.TerrainToolRadius;
			int stepCount = (int)( radius * 2 );
			RenderToolLine( viewport, terrain, center + new Vector2( -radius, -radius ), center + new Vector2( -radius, radius ), stepCount );
			RenderToolLine( viewport, terrain, center + new Vector2( -radius, radius ), center + new Vector2( radius, radius ), stepCount );
			RenderToolLine( viewport, terrain, center + new Vector2( radius, radius ), center + new Vector2( radius, -radius ), stepCount );
			RenderToolLine( viewport, terrain, center + new Vector2( radius, -radius ), center + new Vector2( -radius, -radius ), stepCount );
		}

		protected virtual void RenderTool( Viewport viewport )
		{
			if( GetToolPosition( viewport, out var terrain, out var center ) )
			{
				var deleting = false;
				if( Mode == ModeEnum.PaintPaint )
					deleting = ( Control.ModifierKeys & Keys.Shift ) != 0;
				if( Mode == ModeEnum.PaintClear )
					deleting = ( Control.ModifierKeys & Keys.Shift ) == 0;

				//!!!!в настройки редактора
				var color = !deleting ? new ColorValue( 1, 1, 0 ) : new ColorValue( 1, 0, 0 );

				//ColorValue color = GetCurrentToolType() == ToolTypes.Paint ? new ColorValue( 0, 1, 0 ) : new ColorValue( 1, 1, 0 );
				viewport.Simple3DRenderer.SetColor( color, color * new ColorValue( 1, 1, 1, 0.5 ) );

				if( Component_Scene_DocumentWindow.TerrainToolShape == ToolShape.Circle )
					RenderToolCircle( viewport, terrain, center.ToVector2() );
				else
					RenderToolSquare( viewport, terrain, center.ToVector2() );
			}
		}

		public static Vector2I GetClampedCellIndex( Component_Terrain terrain, Vector2I index )
		{
			terrain.ClampCellIndex( ref index );
			return index;
		}

		//public static Vector2I GetClampedMaskIndex( Component_Terrain terrain, Vector2I index )
		//{
		//	terrain.ClampMaskIndex( ref index );
		//	return index;
		//}

		bool IsCurrentGeometryTool()
		{
			return mode <= ModeEnum.GeometryFlatten;
		}

		public bool IsCurrentPaintTool()
		{
			return mode >= ModeEnum.PaintPaint && mode <= ModeEnum.PaintFlatten;
		}

		//public static bool IsCurrentHoleTool()
		//{
		//	return toolType >= ToolTypes.Hole_Add;
		//}

		//static Component_Terrain FindTerrainWithIndex( List<Component_Terrain> terrains, Vector2I index )
		//{
		//	foreach( var terrain in terrains )
		//	{
		//		if( terrain.HeightmapTerrainManagerIndex == index )
		//			return terrain;
		//	}
		//	return null;
		//}

		protected virtual void ToolPutTickGeometry( Viewport viewport, double delta )
		{
			if( !GetToolPosition( viewport, out var selectedTerrain, out var position ) )
				return;

			var toolRadius = (float)Component_Scene_DocumentWindow.TerrainToolRadius;
			var toolHardness = (float)Component_Scene_DocumentWindow.TerrainToolHardness;
			var toolShapeType = Component_Scene_DocumentWindow.TerrainToolShape;

			float strength = (float)( delta * Component_Scene_DocumentWindow.TerrainToolStrength * toolRadius * 0.5 );

			Vector2 positionMin = position.ToVector2() - new Vector2( toolRadius, toolRadius );
			Vector2 positionMax = position.ToVector2() + new Vector2( toolRadius, toolRadius );

			List<Component_Terrain> terrains;
			//!!!!
			//if( selectedTerrain.HeightmapTerrainManager != null && HeightmapTerrainManager.Instance != null )
			//{
			//	terrains = HeightmapTerrainManager.Instance.GetTerrainsByArea( positionMin, positionMax, true );
			//}
			//else
			//{
			terrains = new List<Component_Terrain>();
			terrains.Add( selectedTerrain );
			//}

			foreach( var terrain in terrains )
			{
				var action = geometryChangeUndoActions.Find( a => a.Terrain == terrain );

				Vector2I indexMin = terrain.GetCellIndexByPosition( positionMin );
				Vector2I indexMax = terrain.GetCellIndexByPosition( positionMax ) + new Vector2I( 1, 1 );
				terrain.ClampCellIndex( ref indexMin );
				terrain.ClampCellIndex( ref indexMax );

				for( int y = indexMin.Y; y <= indexMax.Y; y++ )
				{
					for( int x = indexMin.X; x <= indexMax.X; x++ )
					{
						Vector2 point = terrain.GetPositionXY( new Vector2I( x, y ) );

						float coef;
						{
							double length;
							if( toolShapeType == ToolShape.Circle )
								length = ( point - position.ToVector2() ).Length();
							else
								length = Math.Max( Math.Abs( point.X - position.X ), Math.Abs( point.Y - position.Y ) );

							if( length >= toolRadius )
								coef = 0;
							else if( length == 0 )
								coef = 1;
							else if( length <= toolHardness * toolRadius )
								coef = 1;
							else
							{
								double c;
								if( toolRadius - toolRadius * toolHardness != 0 )
									c = ( length - toolRadius * toolHardness ) / ( toolRadius - toolRadius * toolHardness );
								else
									c = 0;
								coef = (float)Math.Cos( Math.PI / 2 * c );
							}
						}

						if( coef != 0 )
						{
							float oldValue = terrain.GetHeightWithoutPosition( new Vector2I( x, y ), false );

							float value = oldValue;

							//near corner (x == 0 or y == 0) take value from near terrain.
							bool takeValueFromNearTerrain = false;
							//!!!!
							//if( selectedTerrain.HeightmapTerrainManager != null && HeightmapTerrainManager.Instance != null )
							//{
							//	if( x == 0 )
							//	{
							//		HeightmapTerrain terrain2 =
							//			FindTerrainWithIndex( terrains, terrain.HeightmapTerrainManagerIndex - new Vector2I( 1, 0 ) );
							//		if( terrain2 != null )
							//		{
							//			value = terrain2.GetHeight( terrain.GetPositionXY( new Vector2I( x, y ) ), false ) + terrain2.Position.Z;
							//			value -= terrain.Position.Z;

							//			takeValueFromNearTerrain = true;
							//		}
							//	}
							//	else if( y == 0 )
							//	{
							//		HeightmapTerrain terrain2 =
							//			FindTerrainWithIndex( terrains, terrain.HeightmapTerrainManagerIndex - new Vector2I( 0, 1 ) );
							//		if( terrain2 != null )
							//		{
							//			value = terrain2.GetHeight( terrain.GetPositionXY( new Vector2I( x, y ) ), false ) + terrain2.Position.Z;
							//			value -= terrain.Position.Z;

							//			takeValueFromNearTerrain = true;
							//		}
							//	}
							//}

							if( !takeValueFromNearTerrain )
							{
								switch( mode )
								{
								case ModeEnum.GeometryRaise:
								case ModeEnum.GeometryLower:
									{
										bool raise = mode == ModeEnum.GeometryRaise;
										if( ( Form.ModifierKeys & Keys.Shift ) != 0 )
											raise = !raise;

										if( raise )
											value = oldValue + strength * coef;
										else
											value = oldValue - strength * coef;
									}
									break;

								case ModeEnum.GeometrySmooth:
									{
										float needValue = 0;
										{
											{
												bool takeValueFromNearTerrain2 = false;
												//!!!!
												//if( selectedTerrain.HeightmapTerrainManager != null && HeightmapTerrainManager.Instance != null )
												//{
												//	if( x - 1 < 0 )
												//	{
												//		HeightmapTerrain terrain2 =
												//			FindTerrainWithIndex( terrains, terrain.HeightmapTerrainManagerIndex - new Vector2I( 1, 0 ) );
												//		if( terrain2 != null )
												//		{
												//			needValue += terrain2.GetHeight( terrain.GetPositionXY( new Vector2I( x - 1, y ) ), false ) +
												//				terrain2.Position.Z - terrain.Position.Z;
												//			takeValueFromNearTerrain2 = true;
												//		}
												//	}
												//}
												if( !takeValueFromNearTerrain2 )
													needValue += terrain.GetHeightWithoutPosition( GetClampedCellIndex( terrain, new Vector2I( x - 1, y ) ), false );
											}

											{
												bool takeValueFromNearTerrain2 = false;
												//!!!!
												//if( selectedTerrain.HeightmapTerrainManager != null && HeightmapTerrainManager.Instance != null )
												//{
												//	if( x + 1 >= terrain.GetHeightmapSizeAsInteger() + 1 )
												//	{
												//		HeightmapTerrain terrain2 =
												//			FindTerrainWithIndex( terrains, terrain.HeightmapTerrainManagerIndex + new Vector2I( 1, 0 ) );
												//		if( terrain2 != null )
												//		{
												//			needValue += terrain2.GetHeight( terrain.GetPositionXY( new Vector2I( x + 1, y ) ), false ) +
												//				terrain2.Position.Z - terrain.Position.Z;
												//			takeValueFromNearTerrain2 = true;
												//		}
												//	}
												//}
												if( !takeValueFromNearTerrain2 )
													needValue += terrain.GetHeightWithoutPosition( GetClampedCellIndex( terrain, new Vector2I( x + 1, y ) ), false );
											}

											{
												bool takeValueFromNearTerrain2 = false;
												//!!!!
												//if( selectedTerrain.HeightmapTerrainManager != null && HeightmapTerrainManager.Instance != null )
												//{
												//	if( y - 1 < 0 )
												//	{
												//		HeightmapTerrain terrain2 =
												//			FindTerrainWithIndex( terrains, terrain.HeightmapTerrainManagerIndex - new Vector2I( 0, 1 ) );
												//		if( terrain2 != null )
												//		{
												//			needValue += terrain2.GetHeight( terrain.GetPositionXY( new Vector2I( x, y - 1 ) ), false ) +
												//				terrain2.Position.Z - terrain.Position.Z;
												//			takeValueFromNearTerrain2 = true;
												//		}
												//	}
												//}
												if( !takeValueFromNearTerrain2 )
													needValue += terrain.GetHeightWithoutPosition( GetClampedCellIndex( terrain, new Vector2I( x, y - 1 ) ), false );
											}

											{
												bool takeValueFromNearTerrain2 = false;
												//!!!!
												//if( selectedTerrain.HeightmapTerrainManager != null && HeightmapTerrainManager.Instance != null )
												//{
												//	if( y + 1 >= terrain.GetHeightmapSizeAsInteger() + 1 )
												//	{
												//		HeightmapTerrain terrain2 =
												//			FindTerrainWithIndex( terrains, terrain.HeightmapTerrainManagerIndex + new Vector2I( 0, 1 ) );
												//		if( terrain2 != null )
												//		{
												//			needValue += terrain2.GetHeight( terrain.GetPositionXY( new Vector2I( x, y + 1 ) ), false ) +
												//				terrain2.Position.Z - terrain.Position.Z;
												//			takeValueFromNearTerrain2 = true;
												//		}
												//	}
												//}
												if( !takeValueFromNearTerrain2 )
													needValue += terrain.GetHeightWithoutPosition( GetClampedCellIndex( terrain, new Vector2I( x, y + 1 ) ), false );
											}


											//needValue += terrain.GetHeightWithoutPosition( GetClampedCellIndex( terrain, new Vector2I( x - 1, y ) ) );
											//needValue += terrain.GetHeightWithoutPosition( GetClampedCellIndex( terrain, new Vector2I( x + 1, y ) ) );
											//needValue += terrain.GetHeightWithoutPosition( GetClampedCellIndex( terrain, new Vector2I( x, y - 1 ) ) );
											//needValue += terrain.GetHeightWithoutPosition( GetClampedCellIndex( terrain, new Vector2I( x, y + 1 ) ) );
											needValue /= 4;
										}

										if( oldValue < needValue )
										{
											value = oldValue + strength * coef;
											if( value > needValue )
												value = needValue;
										}
										else if( oldValue > needValue )
										{
											value = oldValue - strength * coef;
											if( value < needValue )
												value = needValue;
										}
									}
									break;

								case ModeEnum.GeometryFlatten:
									{
										var needValue = toolModifyStartPosition.Z - terrain.Position.Value.Z;

										if( oldValue < needValue )
										{
											value = oldValue + strength * coef;
											if( value > needValue )
												value = (float)needValue;
										}
										else if( oldValue > needValue )
										{
											value = oldValue - strength * coef;
											if( value < needValue )
												value = (float)needValue;
										}
									}
									break;

								}
							}

							if( oldValue != value )
							{
								//undo
								if( action == null )
								{
									action = new Component_Terrain_GeometryChangeUndoAction( terrain );
									geometryChangeUndoActions.Add( action );
								}
								action.SaveValue( new Vector2I( x, y ), oldValue );

								//update terrain
								terrain.SetHeightWithoutPosition( new Vector2I( x, y ), value );
							}
						}
					}
				}

				var updateRectangle = new RectangleI( indexMin, indexMax );
				terrain.UpdateRenderingData( updateRectangle, false, false, false );

				bool foundItemForTerrain = false;
				foreach( var updateItem in needUpdateRectangle )
				{
					if( updateItem.Terrain == terrain )
					{
						updateItem.Rectangle.Add( updateRectangle );
						foundItemForTerrain = true;
						break;
					}
				}
				if( !foundItemForTerrain )
				{
					NeedUpdateRectangleItem item = new NeedUpdateRectangleItem();
					item.Terrain = terrain;
					item.Rectangle = updateRectangle;
					needUpdateRectangle.Add( item );
				}
			}
		}

		protected virtual void ToolPutTickPaint( Viewport viewport, double delta )
		{
			if( !GetToolPosition( viewport, out var selectedTerrain, out var position ) )
				return;

			var layer = DocumentWindow.TerrainPaintLayersGetSelected();
			if( layer == null )
				return;

			var toolRadius = (float)Component_Scene_DocumentWindow.TerrainToolRadius;
			var toolHardness = (float)Component_Scene_DocumentWindow.TerrainToolHardness;
			var toolShapeType = Component_Scene_DocumentWindow.TerrainToolShape;

			float strength = (float)( delta * Component_Scene_DocumentWindow.TerrainToolStrength * toolRadius * 0.5 * 2.0 );

			Vector2 positionMin = position.ToVector2() - new Vector2( toolRadius, toolRadius );
			Vector2 positionMax = position.ToVector2() + new Vector2( toolRadius, toolRadius );

			List<Component_Terrain> terrains;
			//!!!!
			//if( selectedTerrain.HeightmapTerrainManager != null && HeightmapTerrainManager.Instance != null )
			//{
			//	terrains = HeightmapTerrainManager.Instance.GetTerrainsByArea( positionMin, positionMax, true );
			//}
			//else
			//{
			terrains = new List<Component_Terrain>();
			terrains.Add( selectedTerrain );
			//}

			foreach( var terrain in terrains )
			{
				UndoActionPropertiesChange undoSetPropertyAction = paintSetPropertyUndoActions.Find( a => a.Items[ 0 ].Obj == layer );
				var undoChangeAction = paintChangeUndoActions.Find( a => a.Terrain == terrain );

				Vector2I indexMin = terrain.GetMaskIndexByPosition( positionMin );
				Vector2I indexMax = terrain.GetMaskIndexByPosition( positionMax ) + new Vector2I( 1, 1 );
				terrain.ClampMaskIndex( ref indexMin );
				terrain.ClampMaskIndex( ref indexMax );

				for( int y = indexMin.Y; y <= indexMax.Y; y++ )
				{
					for( int x = indexMin.X; x <= indexMax.X; x++ )
					{
						Vector2 point = terrain.GetPositionXYByMaskIndex( new Vector2I( x, y ) );

						float coef;
						{
							double length;
							if( toolShapeType == ToolShape.Circle )
								length = ( point - position.ToVector2() ).Length();
							else
								length = Math.Max( Math.Abs( point.X - position.X ), Math.Abs( point.Y - position.Y ) );

							if( length >= toolRadius )
								coef = 0;
							else if( length == 0 )
								coef = 1;
							else if( length <= toolHardness * toolRadius )
								coef = 1;
							else
							{
								double c;
								if( toolRadius - toolRadius * toolHardness != 0 )
									c = ( length - toolRadius * toolHardness ) / ( toolRadius - toolRadius * toolHardness );
								else
									c = 0;
								coef = (float)Math.Cos( Math.PI / 2 * c );
							}
						}

						if( coef != 0 )
						{
							float oldValue = layer.GetMaskValue( new Vector2I( x, y ) );

							float value = oldValue;

							//near corner (x == 0 or y == 0) take value from near terrain.
							bool takeValueFromNearTerrain = false;
							//!!!!
							//if( selectedTerrain.HeightmapTerrainManager != null && HeightmapTerrainManager.Instance != null )
							//{
							//	if( x == 0 )
							//	{
							//		HeightmapTerrain terrain2 =
							//			FindTerrainWithIndex( terrains, terrain.HeightmapTerrainManagerIndex - new Vector2I( 1, 0 ) );
							//		if( terrain2 != null )
							//		{
							//			value = terrain2.GetHeight( terrain.GetPositionXY( new Vector2I( x, y ) ), false ) + terrain2.Position.Z;
							//			value -= terrain.Position.Z;

							//			takeValueFromNearTerrain = true;
							//		}
							//	}
							//	else if( y == 0 )
							//	{
							//		HeightmapTerrain terrain2 =
							//			FindTerrainWithIndex( terrains, terrain.HeightmapTerrainManagerIndex - new Vector2I( 0, 1 ) );
							//		if( terrain2 != null )
							//		{
							//			value = terrain2.GetHeight( terrain.GetPositionXY( new Vector2I( x, y ) ), false ) + terrain2.Position.Z;
							//			value -= terrain.Position.Z;

							//			takeValueFromNearTerrain = true;
							//		}
							//	}
							//}

							if( !takeValueFromNearTerrain )
							{
								switch( mode )
								{
								case ModeEnum.PaintPaint:
								case ModeEnum.PaintClear:
									{
										bool paint = mode == ModeEnum.PaintPaint;
										if( ( Form.ModifierKeys & Keys.Shift ) != 0 )
											paint = !paint;

										if( paint )
											value = oldValue + strength * coef;
										else
											value = oldValue - strength * coef;
									}
									break;

								case ModeEnum.PaintSmooth:
									{
										float needValue = 0;
										{
											needValue += layer.GetMaskValue( new Vector2I( x - 1, y ) );
											needValue += layer.GetMaskValue( new Vector2I( x + 1, y ) );
											needValue += layer.GetMaskValue( new Vector2I( x, y - 1 ) );
											needValue += layer.GetMaskValue( new Vector2I( x, y + 1 ) );
											//needValue += layer.GetMaskValue( GetClampedMaskIndex( terrain, new Vec2I( x - 1, y ) ) );
											//needValue += layer.GetMaskValue( GetClampedMaskIndex( terrain, new Vec2I( x + 1, y ) ) );
											//needValue += layer.GetMaskValue( GetClampedMaskIndex( terrain, new Vec2I( x, y - 1 ) ) );
											//needValue += layer.GetMaskValue( GetClampedMaskIndex( terrain, new Vec2I( x, y + 1 ) ) );
											needValue /= 4;
										}

										if( oldValue < needValue )
										{
											value = oldValue + strength * coef;
											if( value > needValue )
												value = needValue;
										}
										else if( oldValue > needValue )
										{
											value = oldValue - strength * coef;
											if( value < needValue )
												value = needValue;
										}
									}
									break;

								case ModeEnum.PaintFlatten:
									{
										float needValue = toolModifyStartMaskValue;

										if( oldValue < needValue )
										{
											value = oldValue + strength * coef;
											if( value > needValue )
												value = needValue;
										}
										else if( oldValue > needValue )
										{
											value = oldValue - strength * coef;
											if( value < needValue )
												value = needValue;
										}
									}
									break;

								}
							}

							MathEx.Clamp( ref value, 0, 1 );

							if( oldValue != value )
							{
								//undo
								if( layer.Mask.Value == null || layer.Mask.Value.Length == 0 || undoSetPropertyAction != null )
								{
									if( undoSetPropertyAction == null )
									{
										var oldValue2 = layer.Mask;

										layer.Mask = new byte[ terrain.GetPaintMaskSizeInteger() * terrain.GetPaintMaskSizeInteger() ];

										var property = (Metadata.Property)layer.MetadataGetMemberBySignature( "property:Mask" );
										var undoItem = new UndoActionPropertiesChange.Item( layer, property, oldValue2 );
										undoSetPropertyAction = new UndoActionPropertiesChange( undoItem );
										paintSetPropertyUndoActions.Add( undoSetPropertyAction );
									}
								}
								else
								{
									if( undoChangeAction == null )
									{
										undoChangeAction = new Component_Terrain_PaintChangeUndoAction( terrain );
										paintChangeUndoActions.Add( undoChangeAction );
									}
									undoChangeAction.SaveValue( layer, new Vector2I( x, y ), oldValue );
								}

								//update terrain
								layer.SetMaskValue( new Vector2I( x, y ), value );
							}
						}
					}
				}

				//!!!!use mask indexes
				//var updateRectangle = new RectangleI( indexMin, indexMax );

				//terrain.UpdateRenderingData( updateRectangle, false );

				//bool foundItemForTerrain = false;
				//foreach( var updateItem in needUpdateRectangle )
				//{
				//	if( updateItem.Terrain == terrain )
				//	{
				//		updateItem.Rectangle.Add( updateRectangle );
				//		foundItemForTerrain = true;
				//		break;
				//	}
				//}
				//if( !foundItemForTerrain )
				//{
				//	NeedUpdateRectangleItem item = new NeedUpdateRectangleItem();
				//	item.Terrain = terrain;
				//	item.Rectangle = updateRectangle;
				//	needUpdateRectangle.Add( item );
				//}
			}
		}

		protected virtual void StopToolModify( bool cancel )
		{
			//update rendering data
			foreach( var item in needUpdateRectangle )
				item.Terrain.UpdateRenderingData( item.Rectangle, true, true, true );

			//update collision
			if( !cancel )
			{
				foreach( var item in needUpdateRectangle )
				{
					item.Terrain.UpdateCollisionData( item.Rectangle );

					////update MapObject's alignment
					//{
					//	const int border = 2;

					//	Vec2I indexMin = item.needUpdateCollisionRectangle.Minimum - new Vec2I( border, border );
					//	Vec2I indexMax = item.needUpdateCollisionRectangle.Maximum + new Vec2I( border, border );
					//	item.terrain.ClampCellIndex( ref indexMin );
					//	item.terrain.ClampCellIndex( ref indexMax );

					//	Rect rectangle = new Rect( item.terrain.GetPositionXY( indexMin ),
					//		item.terrain.GetPositionXY( indexMax ) );

					//	if( EntitiesEditManager.Instance != null )
					//		EntitiesEditManager.Instance.UpdateObjectsVerticalAlignment( rectangle );
					//}
				}
			}

			needUpdateRectangle.Clear();

			//add to undo
			if( geometryChangeUndoActions.Count != 0 )
			{
				if( cancel )
				{
					foreach( var action in geometryChangeUndoActions )
						action.RestoreOldValues();
				}
				else
				{
					var multiAction = new UndoMultiAction();
					foreach( var action in geometryChangeUndoActions )
						multiAction.AddAction( action );
					DocumentWindow.Document.CommitUndoAction( multiAction );
				}
				geometryChangeUndoActions.Clear();
			}

			if( paintSetPropertyUndoActions.Count != 0 )
			{
				if( cancel )
				{
					foreach( var action in paintSetPropertyUndoActions )
						action.PerformUndo();
				}
				else
				{
					var multiAction = new UndoMultiAction();
					foreach( var action in paintSetPropertyUndoActions )
						multiAction.AddAction( action );
					DocumentWindow.Document.CommitUndoAction( multiAction );
				}
				paintSetPropertyUndoActions.Clear();
			}

			if( paintChangeUndoActions.Count != 0 )
			{
				if( cancel )
				{
					foreach( var action in paintChangeUndoActions )
						action.PerformUndo();
				}
				else
				{
					var multiAction = new UndoMultiAction();
					foreach( var action in paintChangeUndoActions )
						multiAction.AddAction( action );
					DocumentWindow.Document.CommitUndoAction( multiAction );
				}
				paintChangeUndoActions.Clear();
			}

			//if( holeChangeUndoActions.Count != 0 )
			//{
			//	UndoMultiAction multiAction = new UndoMultiAction();
			//	foreach( UndoSystem.Action action in holeChangeUndoActions )
			//		multiAction.AddAction( action );
			//	UndoSystem.Instance.CommitAction( multiAction );
			//	holeChangeUndoActions.Clear();
			//}

			toolModify = false;
		}
	}
}
