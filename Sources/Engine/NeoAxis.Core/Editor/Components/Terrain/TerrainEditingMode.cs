#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public class TerrainEditingMode : SceneEditor.WorkareaModeClassScene
	{
		TerrainEditorMode mode;
		object modeExtensionData;
		Scene scene;

		bool toolModify;
		Vector3 toolModifyStartPosition;
		float toolModifyStartMaskValue;

		ESet<Terrain> needRestoreAllowFullUpdateCollisionCurrentLayers = new ESet<Terrain>();

		public class NeedFullUpdateRectangleItem
		{
			public Terrain Terrain;
			public RectangleI Rectangle;
			//public bool GeometryUpdated;
			//public bool PaintUpdated;
		}
		List<NeedFullUpdateRectangleItem> needFullUpdateRectangleItems = new List<NeedFullUpdateRectangleItem>();

		//undo
		List<TerrainGeometryChangeUndoAction> geometryChangeUndoActions = new List<TerrainGeometryChangeUndoAction>();
		List<UndoActionPropertiesChange> paintSetPropertyUndoActions = new List<UndoActionPropertiesChange>();
		List<TerrainPaintChangeUndoAction> paintChangeUndoActions = new List<TerrainPaintChangeUndoAction>();
		List<UndoActionComponentCreateDelete> paintLayerCreateUndoActions = new List<UndoActionComponentCreateDelete>();

		const float toolRenderOffset = 0.2f;

		///////////////////////////////////////////

		public enum TerrainEditorMode
		{
			GeometryRaise,
			GeometryLower,
			GeometrySmooth,
			GeometryFlatten,

			PaintPaint,
			PaintClear,
			PaintSmooth,
			PaintFlatten,

			Extension,
		}

		///////////////////////////////////////////

		public enum TerrainEditorToolShape
		{
			Circle,
			Square,
		}

		///////////////////////////////////////////

		public TerrainEditingMode( SceneEditor documentWindow, TerrainEditorMode mode, object modeExtensionData = null )
			: base( documentWindow )
		{
			this.mode = mode;
			this.modeExtensionData = modeExtensionData;

			scene = documentWindow.Scene;
		}

		public TerrainEditorMode Mode { get { return mode; } }
		public object ModeExtensionData { get { return modeExtensionData; } }
		public Scene Scene { get { return scene; } }
		public bool ToolModify { get { return toolModify; } }
		public Vector3 ToolModifyStartPosition { get { return toolModifyStartPosition; } }
		public float ToolModifyStartMaskValue { get { return toolModifyStartMaskValue; } }
		public List<NeedFullUpdateRectangleItem> NeedUpdateRectangleItems { get { return needFullUpdateRectangleItems; } }
		public List<TerrainGeometryChangeUndoAction> GeometryChangeUndoActions { get { return geometryChangeUndoActions; } }
		public List<UndoActionPropertiesChange> PaintSetPropertyUndoActions { get { return paintSetPropertyUndoActions; } }
		public List<TerrainPaintChangeUndoAction> PaintChangeUndoActions { get { return paintChangeUndoActions; } }
		public List<UndoActionComponentCreateDelete> PaintLayerCreateUndoActions { get { return paintLayerCreateUndoActions; } }

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

					if( mode == TerrainEditorMode.PaintFlatten )
					{
						if( layer != null )
						{
							var maskIndex = terrain.GetMaskIndexByPosition( toolModifyStartPosition.ToVector2() );
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
			}
		}

		protected override void OnUpdateBeforeOutput( Viewport viewport )
		{
			RenderTool( viewport );
		}

		protected virtual bool GetToolPosition( Viewport viewport, out Terrain terrain, out Vector3 center )
		{
			if( !viewport.MouseRelativeMode )
			{
				var ray = viewport.CameraSettings.GetRayByScreenCoordinates( viewport.MousePosition );

				if( Terrain.GetTerrainByRay( scene, ray, out terrain, out center ) )
					return true;
			}

			terrain = null;
			center = Vector3.Zero;
			return false;
		}

		protected virtual void RenderToolCircle( Viewport viewport, Terrain terrain, Vector2 center )
		{
			var radius = SceneEditor.TerrainToolRadius;

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

		protected virtual void RenderToolLine( Viewport viewport, Terrain terrain, Vector2 start, Vector2 end, int stepCount )
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

		protected virtual void RenderToolSquare( Viewport viewport, Terrain terrain, Vector2 center )
		{
			var radius = SceneEditor.TerrainToolRadius;
			int stepCount = (int)( radius * 2 );
			RenderToolLine( viewport, terrain, center + new Vector2( -radius, -radius ), center + new Vector2( -radius, radius ), stepCount );
			RenderToolLine( viewport, terrain, center + new Vector2( -radius, radius ), center + new Vector2( radius, radius ), stepCount );
			RenderToolLine( viewport, terrain, center + new Vector2( radius, radius ), center + new Vector2( radius, -radius ), stepCount );
			RenderToolLine( viewport, terrain, center + new Vector2( radius, -radius ), center + new Vector2( -radius, -radius ), stepCount );
		}

		(ReferenceNoValue reference, bool isSurface) GetSelectedMaterialOrSurfaceToCreate()
		{
			(var objectType, var referenceToObject, var anyData, var objectName) = EditorAPI.GetSelectedObjectToCreate();
			if( objectType != null )
			{
				var componentType = objectType as Metadata.ComponentTypeInfo;
				if( componentType != null && componentType.BasedOnObject != null )
				{
					//!!!!пока только ресурсные ссылки поддерживаются

					//Material
					var material = componentType.BasedOnObject as Material;
					if( material != null )
						return (ReferenceUtility.MakeResourceReference( material ), false);

					//Import3D
					if( componentType.BasedOnObject is Import3D )
					{
						material = componentType.BasedOnObject.GetComponent( "Material" ) as Material;
						if( material != null )
							return (ReferenceUtility.MakeResourceReference( material ), false);
					}

					//Surface
					var surface = componentType.BasedOnObject as Surface;
					if( surface != null )
						return (ReferenceUtility.MakeResourceReference( surface ), true);
				}
			}

			return (new ReferenceNoValue(), false);
		}

		protected virtual void RenderTool( Viewport viewport )
		{
			if( GetToolPosition( viewport, out var terrain, out var center ) )
			{
				var layer = DocumentWindow.TerrainPaintLayersGetSelected();
				var materialOrSurface = GetSelectedMaterialOrSurfaceToCreate();

				if( IsCurrentGeometryTool() || IsCurrentPaintTool() && ( layer != null || materialOrSurface.reference.ReferenceSpecified ) )
				{
					var deleting = false;
					if( Mode == TerrainEditorMode.PaintPaint )
						deleting = ( Control.ModifierKeys & Keys.Shift ) != 0;
					if( Mode == TerrainEditorMode.PaintClear )
						deleting = ( Control.ModifierKeys & Keys.Shift ) == 0;

					//!!!!в настройки редактора
					var color = !deleting ? new ColorValue( 1, 1, 0 ) : new ColorValue( 1, 0, 0 );

					//ColorValue color = GetCurrentToolType() == ToolTypes.Paint ? new ColorValue( 0, 1, 0 ) : new ColorValue( 1, 1, 0 );
					viewport.Simple3DRenderer.SetColor( color, color * new ColorValue( 1, 1, 1, 0.5 ) );

					if( SceneEditor.TerrainToolShape == TerrainEditorToolShape.Circle )
						RenderToolCircle( viewport, terrain, center.ToVector2() );
					else
						RenderToolSquare( viewport, terrain, center.ToVector2() );
				}
			}
		}

		public static Vector2I GetClampedCellIndex( Terrain terrain, Vector2I index )
		{
			terrain.ClampCellIndex( ref index );
			return index;
		}

		//public static Vector2I GetClampedMaskIndex( Terrain terrain, Vector2I index )
		//{
		//	terrain.ClampMaskIndex( ref index );
		//	return index;
		//}

		bool IsCurrentGeometryTool()
		{
			return mode <= TerrainEditorMode.GeometryFlatten;
		}

		public bool IsCurrentPaintTool()
		{
			return mode >= TerrainEditorMode.PaintPaint && mode <= TerrainEditorMode.PaintFlatten;
		}

		//static Terrain FindTerrainWithIndex( List<Terrain> terrains, Vector2I index )
		//{
		//	foreach( var terrain in terrains )
		//	{
		//		if( terrain.HeightmapTerrainManagerIndex == index )
		//			return terrain;
		//	}
		//	return null;
		//}

		void AddFullUpdateRectangle( Terrain terrain, RectangleI rectangle )
		{
			foreach( var updateItem in needFullUpdateRectangleItems )
			{
				if( updateItem.Terrain == terrain )
				{
					updateItem.Rectangle.Add( rectangle );
					return;
				}
			}

			{
				var item = new NeedFullUpdateRectangleItem();
				item.Terrain = terrain;
				item.Rectangle = rectangle;
				needFullUpdateRectangleItems.Add( item );
			}
		}

		protected virtual void ToolPutTickGeometry( Viewport viewport, double delta )
		{
			if( !GetToolPosition( viewport, out var selectedTerrain, out var position ) )
				return;

			var toolRadius = (float)SceneEditor.TerrainToolRadius;
			var toolHardness = (float)SceneEditor.TerrainToolHardness;
			var toolShapeType = SceneEditor.TerrainToolShape;

			float strength = (float)( delta * SceneEditor.TerrainToolStrength * toolRadius * 0.5 );

			Vector2 positionMin = position.ToVector2() - new Vector2( toolRadius, toolRadius );
			Vector2 positionMax = position.ToVector2() + new Vector2( toolRadius, toolRadius );

			List<Terrain> terrains;
			//!!!!
			//if( selectedTerrain.HeightmapTerrainManager != null && HeightmapTerrainManager.Instance != null )
			//{
			//	terrains = HeightmapTerrainManager.Instance.GetTerrainsByArea( positionMin, positionMax, true );
			//}
			//else
			//{
			terrains = new List<Terrain>();
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
							if( toolShapeType == TerrainEditorToolShape.Circle )
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
								case TerrainEditorMode.GeometryRaise:
								case TerrainEditorMode.GeometryLower:
									{
										bool raise = mode == TerrainEditorMode.GeometryRaise;
										if( ( Form.ModifierKeys & Keys.Shift ) != 0 )
											raise = !raise;

										if( raise )
											value = oldValue + strength * coef;
										else
											value = oldValue - strength * coef;
									}
									break;

								case TerrainEditorMode.GeometrySmooth:
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

								case TerrainEditorMode.GeometryFlatten:
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
									action = new TerrainGeometryChangeUndoAction( terrain );
									geometryChangeUndoActions.Add( action );
								}
								action.SaveValue( new Vector2I( x, y ), oldValue );

								//update terrain
								terrain.SetHeightWithoutPosition( new Vector2I( x, y ), value );
							}
						}
					}
				}

				terrain.AllowFullUpdateGeometryCollisionCurrentLayers = false;
				needRestoreAllowFullUpdateCollisionCurrentLayers.AddWithCheckAlreadyContained( terrain );

				var updateRectangle = new RectangleI( indexMin, indexMax );
				terrain.SetNeedUpdateTilesByCellIndices( updateRectangle, Terrain.NeedUpdateEnum.Geometry | Terrain.NeedUpdateEnum.Collision );

				//terrain.UpdateRenderingData( updateRectangle, false, false, false );

				AddFullUpdateRectangle( terrain, updateRectangle );

				//bool foundItemForTerrain = false;
				//foreach( var updateItem in needUpdateRectangle )
				//{
				//	if( updateItem.Terrain == terrain )
				//	{
				//		updateItem.Rectangle.Add( updateRectangle );
				//		updateItem.GeometryUpdated = true;
				//		foundItemForTerrain = true;
				//		break;
				//	}
				//}
				//if( !foundItemForTerrain )
				//{
				//	NeedUpdateRectangleItem item = new NeedUpdateRectangleItem();
				//	item.Terrain = terrain;
				//	item.Rectangle = updateRectangle;
				//	item.GeometryUpdated = true;
				//	needUpdateRectangle.Add( item );
				//}
			}
		}

		protected virtual void ToolPutTickPaint( Viewport viewport, double delta )
		{
			if( !GetToolPosition( viewport, out var selectedTerrain, out var position ) )
				return;

			var layer = DocumentWindow.TerrainPaintLayersGetSelected();
			var materialOrSurface = GetSelectedMaterialOrSurfaceToCreate();

			if( !( layer != null || materialOrSurface.reference.ReferenceSpecified ) )
				return;

			//find layer or add
			if( layer == null )
			{
				//!!!!если несколько террейнов

				var layers = selectedTerrain.GetComponents<PaintLayer>();
				foreach( var l in layers )
				{
					if( materialOrSurface.isSurface )
					{
						if( l.Surface.ReferenceSpecified && l.Surface.GetByReference == materialOrSurface.reference.GetByReference )
						{
							layer = l;
							break;
						}
					}
					else
					{
						if( l.Material.ReferenceSpecified && l.Material.GetByReference == materialOrSurface.reference.GetByReference )
						{
							layer = l;
							break;
						}
					}
				}

				if( layer == null )
				{
					layer = selectedTerrain.CreateComponent<PaintLayer>( enabled: false );
					layer.Name = EditorUtility.GetUniqueFriendlyName( layer );
					if( materialOrSurface.isSurface )
						layer.Surface = materialOrSurface.reference;
					else
						layer.Material = materialOrSurface.reference;
					layer.Enabled = true;

					var undoAction = new UndoActionComponentCreateDelete( DocumentWindow.Document, new Component[] { layer }, true );
					paintLayerCreateUndoActions.Add( undoAction );
				}
			}

			var toolRadius = (float)SceneEditor.TerrainToolRadius;
			var toolHardness = (float)SceneEditor.TerrainToolHardness;
			var toolShapeType = SceneEditor.TerrainToolShape;

			float strength = (float)( delta * SceneEditor.TerrainToolStrength * toolRadius * 0.5 * 2.0 );

			Vector2 positionMin = position.ToVector2() - new Vector2( toolRadius, toolRadius );
			Vector2 positionMax = position.ToVector2() + new Vector2( toolRadius, toolRadius );

			List<Terrain> terrains;
			//!!!!
			//if( selectedTerrain.HeightmapTerrainManager != null && HeightmapTerrainManager.Instance != null )
			//{
			//	terrains = HeightmapTerrainManager.Instance.GetTerrainsByArea( positionMin, positionMax, true );
			//}
			//else
			//{
			terrains = new List<Terrain>();
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
							if( toolShapeType == TerrainEditorToolShape.Circle )
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
								case TerrainEditorMode.PaintPaint:
								case TerrainEditorMode.PaintClear:
									{
										bool paint = mode == TerrainEditorMode.PaintPaint;
										if( ( Form.ModifierKeys & Keys.Shift ) != 0 )
											paint = !paint;

										if( paint )
											value = oldValue + strength * coef;
										else
											value = oldValue - strength * coef;
									}
									break;

								case TerrainEditorMode.PaintSmooth:
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

								case TerrainEditorMode.PaintFlatten:
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

										var property = (Metadata.Property)layer.MetadataGetMemberBySignature( "property:" + nameof( PaintLayer.Mask ) );
										var undoItem = new UndoActionPropertiesChange.Item( layer, property, oldValue2 );
										undoSetPropertyAction = new UndoActionPropertiesChange( undoItem );
										paintSetPropertyUndoActions.Add( undoSetPropertyAction );
									}
								}
								else
								{
									if( undoChangeAction == null )
									{
										undoChangeAction = new TerrainPaintChangeUndoAction( terrain );
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

				terrain.AllowFullUpdateGeometryCollisionCurrentLayers = false;
				needRestoreAllowFullUpdateCollisionCurrentLayers.AddWithCheckAlreadyContained( terrain );

				var updateRectangle = new RectangleI( indexMin, indexMax );
				terrain.SetNeedUpdateTilesByMaskIndices( updateRectangle, Terrain.NeedUpdateEnum.Layers );
				//terrain.UpdateRenderingData( updateRectangle, false, false, false );

				AddFullUpdateRectangle( terrain, updateRectangle );

				//bool foundItemForTerrain = false;
				//foreach( var updateItem in needUpdateRectangle )
				//{
				//	if( updateItem.Terrain == terrain )
				//	{
				//		updateItem.Rectangle.Add( updateRectangle );
				//		updateItem.PaintUpdated = true;
				//		foundItemForTerrain = true;
				//		break;
				//	}
				//}
				//if( !foundItemForTerrain )
				//{
				//	NeedUpdateRectangleItem item = new NeedUpdateRectangleItem();
				//	item.Terrain = terrain;
				//	item.Rectangle = updateRectangle;
				//	item.PaintUpdated = true;
				//	needUpdateRectangle.Add( item );
				//}

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
			foreach( var terrain in needRestoreAllowFullUpdateCollisionCurrentLayers )
				terrain.AllowFullUpdateGeometryCollisionCurrentLayers = true;
			needRestoreAllowFullUpdateCollisionCurrentLayers.Clear();

			foreach( var item in needFullUpdateRectangleItems )
				item.Terrain.SetNeedUpdateTilesByCellIndices( item.Rectangle, Terrain.NeedUpdateEnum.Geometry | Terrain.NeedUpdateEnum.Collision );
			needFullUpdateRectangleItems.Clear();

			////update rendering data
			//foreach( var item in needUpdateRectangle )
			//{
			//	if( item.GeometryUpdated )
			//		item.Terrain.UpdateRenderingData( item.Rectangle, true, true, true );
			//}

			////update layers

			////update group of objects

			////update collision
			//if( !cancel )
			//{
			//	foreach( var item in needUpdateRectangle )
			//	{
			//		if( item.GeometryUpdated )
			//		{
			//			item.Terrain.UpdateCollisionData( item.Rectangle );

			//			////update MapObject's alignment
			//			//{
			//			//	const int border = 2;

			//			//	Vec2I indexMin = item.needUpdateCollisionRectangle.Minimum - new Vec2I( border, border );
			//			//	Vec2I indexMax = item.needUpdateCollisionRectangle.Maximum + new Vec2I( border, border );
			//			//	item.terrain.ClampCellIndex( ref indexMin );
			//			//	item.terrain.ClampCellIndex( ref indexMax );

			//			//	Rect rectangle = new Rect( item.terrain.GetPositionXY( indexMin ),
			//			//		item.terrain.GetPositionXY( indexMax ) );

			//			//	if( EntitiesEditManager.Instance != null )
			//			//		EntitiesEditManager.Instance.UpdateObjectsVerticalAlignment( rectangle );
			//			//}
			//		}
			//	}
			//}

			//needUpdateRectangle.Clear();

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

			if( paintLayerCreateUndoActions.Count != 0 )
			{
				if( cancel )
				{
					foreach( var action in paintLayerCreateUndoActions )
						action.DoUndo();
				}
				else
				{
					var multiAction = new UndoMultiAction();
					foreach( var action in paintLayerCreateUndoActions )
						multiAction.AddAction( action );
					DocumentWindow.Document.CommitUndoAction( multiAction );
				}

				paintLayerCreateUndoActions.Clear();
			}

			toolModify = false;
		}
	}
}

#endif