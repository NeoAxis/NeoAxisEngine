// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using NeoAxis.Editor;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;
using System.Runtime.CompilerServices;
using Internal.SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// A heightmap based terrain.
	/// </summary>
#if !DEPLOY
	[SettingsCell( "NeoAxis.Editor.TerrainSettingsCell" )]
	//because sorting is added from ResourcesWindowItems.cs [AddToResourcesWindow( @"Base\Scene objects\Terrain", 0 )]
#endif
	public class Terrain : Component
	{
		const double timeLimitToDestroyFarSurfaceObjectGroups = 10;

		/////////////////////////////////////////

		int heightmapSize = 512;
		float[,] heightmapBuffer;

		float[,] heightmapSize_UndoDependentData;

		class HoleCacheItem
		{
			public Vector3[] vertices;
			public int[] indices;
		}
		Dictionary<Vector2I, HoleCacheItem> holesCache = new Dictionary<Vector2I, HoleCacheItem>();

		/////////////////////////////////////////

		int tileSize = 64;

		int tileCount;
		Tile[,] tiles;

		//cached data
		Vector3 cachedPosition;
		Bounds cachedBounds;
		internal float cachedCellSize;
		float cachedCellSizeInv;
		int cachedPaintMaskSize;
		float cachedMaskCellSize;
		float cachedMaskCellSizeInv;
		int cachedLodLevelCount;

		bool heightMinMaxNeedUpdate = true;
		Range heightMinMax;

		BaseSurfaceObjectsDataClass baseSurfaceObjectsDataCache;
		RenderingPipeline.RenderSceneData.LayerItem[] currentLayersCache = Array.Empty<RenderingPipeline.RenderSceneData.LayerItem>();

		bool allowRecreateInternalData = true;
		bool allowUpdateTiles = true;

		/////////////////////////////////////////

		/// <summary>
		/// The position of the object in the scene.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Geometry" )]
		public Reference<Vector3> Position
		{
			get { if( _position.BeginGet() ) Position = _position.Get( this ); return _position.value; }
			set
			{
				if( _position.BeginSet( this, ref value ) )
				{
					try
					{
						PositionChanged?.Invoke( this );
						RecreateInternalData();
					}
					finally { _position.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Position"/> property value changes.</summary>
		public event Action<Terrain> PositionChanged;
		ReferenceField<Vector3> _position;

		/// <summary>
		/// The size along the axes X and Y.
		/// </summary>
		[DefaultValue( 500 )]
		[Category( "Geometry" )]
		public Reference<double> HorizontalSize
		{
			get { if( _horizontalSize.BeginGet() ) HorizontalSize = _horizontalSize.Get( this ); return _horizontalSize.value; }
			set
			{
				if( value <= 0.1 )
					value = 0.1;
				if( _horizontalSize.BeginSet( this, ref value ) )
				{
					try
					{
						HorizontalSizeChanged?.Invoke( this );
						RecreateInternalData();
					}
					finally { _horizontalSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="HorizontalSize"/> property value changes.</summary>
		public event Action<Terrain> HorizontalSizeChanged;
		ReferenceField<double> _horizontalSize = 500;

		public enum HeightmapSizeEnum
		{
			_128x128,
			_256x256,
			_512x512,
			_1024x1024,
			_2048x2048,
			//_4096x4096,
		}

		/// <summary>
		/// The resolution of the height map.
		/// </summary>
		[DefaultValue( HeightmapSizeEnum._512x512 )]
		[UndoDependentProperty( nameof( HeightmapSize_UndoDependentData ) )]
		[Category( "Geometry" )]
		public Reference<HeightmapSizeEnum> HeightmapSize
		{
			get { if( _heightmapSize.BeginGet() ) HeightmapSize = _heightmapSize.Get( this ); return _heightmapSize.value; }
			set
			{
				if( _heightmapSize.BeginSet( this, ref value ) )
				{
					try
					{
						HeightmapSizeChanged?.Invoke( this );

						if( heightmapSize_UndoDependentData != null )
						{
							heightmapSize = ParseHeightmapSize( _heightmapSize.value );
							heightmapBuffer = heightmapSize_UndoDependentData;
							heightmapSize_UndoDependentData = null;

							RecreateInternalData();
						}
						else
						{
							var newSize = ParseHeightmapSize( _heightmapSize.value );
							if( newSize != heightmapSize )
								ResizeHeightmap( newSize );
						}
					}
					finally { _heightmapSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="HeightmapSize"/> property value changes.</summary>
		public event Action<Terrain> HeightmapSizeChanged;
		ReferenceField<HeightmapSizeEnum> _heightmapSize = HeightmapSizeEnum._512x512;

		/// <summary>
		/// The list of objects that are shapes for cutting holes.
		/// </summary>
		[Cloneable( CloneType.Deep )]
		[Category( "Geometry" )]
		[Serialize]
		public ReferenceList<MeshInSpace> Holes
		{
			get { return _holes; }
		}
		public delegate void HolesChangedDelegate( Terrain sender );
		public event HolesChangedDelegate HolesChanged;
		ReferenceList<MeshInSpace> _holes;

		/// <summary>
		/// Whether to enable holes.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Geometry" )]
		public Reference<bool> HolesEnabled
		{
			get { if( _holesEnabled.BeginGet() ) HolesEnabled = _holesEnabled.Get( this ); return _holesEnabled.value; }
			set { if( _holesEnabled.BeginSet( this, ref value ) ) { try { HolesEnabledChanged?.Invoke( this ); RecreateInternalData( true ); } finally { _holesEnabled.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HolesEnabled"/> property value changes.</summary>
		public event Action<Terrain> HolesEnabledChanged;
		ReferenceField<bool> _holesEnabled = true;

		/// <summary>
		/// Whether the object is visible in the scene.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Display" )]
		public Reference<bool> Visible
		{
			get { if( _visible.BeginGet() ) Visible = _visible.Get( this ); return _visible.value; }
			set
			{
				if( _visible.BeginSet( this, ref value ) )
				{
					try
					{
						VisibleChanged?.Invoke( this );

						foreach( var tile in GetTiles() )
						{
							if( tile.ObjectInSpace != null )
								tile.ObjectInSpace.Visible = _visible.value;
							if( tile.SurfaceObjectsObjectInSpace != null )
								tile.SurfaceObjectsObjectInSpace.Visible = _visible.value;
						}

						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Layers );
							UpdateTiles();
						}
					}
					finally { _visible.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Visible"/> property value changes.</summary>
		public event Action<Terrain> VisibleChanged;
		ReferenceField<bool> _visible = true;

		/// <summary>
		/// Base material of a terrain.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Display" )]
		public Reference<Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set
			{
				if( _material.BeginSet( this, ref value ) )
				{
					try
					{
						MaterialChanged?.Invoke( this );

						var surface = Surface.Value;
						if( surface == null || !surface.Material.ReferenceOrValueSpecified )
						{
							foreach( var tile in GetTiles() )
							{
								if( tile.ObjectInSpace != null )
									tile.ObjectInSpace.ReplaceMaterial = _material.value;
							}
						}
					}
					finally { _material.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<Terrain> MaterialChanged;
		ReferenceField<Material> _material = null;

		/// <summary>
		/// The number of UV tiles per unit for texture coordinates 0.
		/// </summary>
		[DefaultValue( "1 1" )]
		[Serialize]
		[DisplayName( "Material UV 0" )]
		[Category( "Display" )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<Vector2> MaterialUV0
		{
			get { if( _materialUV0.BeginGet() ) MaterialUV0 = _materialUV0.Get( this ); return _materialUV0.value; }
			set
			{
				if( _materialUV0.BeginSet( this, ref value ) )
				{
					try
					{
						MaterialUV0Changed?.Invoke( this );

						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Geometry );
							UpdateTiles();
						}
					}
					finally { _materialUV0.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialUV0"/> property value changes.</summary>
		public event Action<Terrain> MaterialUV0Changed;
		ReferenceField<Vector2> _materialUV0 = Vector2.One;

		/// <summary>
		/// The number of UV tiles per unit for texture coordinates 1.
		/// </summary>
		[DefaultValue( "1 1" )]
		[Serialize]
		[DisplayName( "Material UV 1" )]
		[Category( "Display" )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<Vector2> MaterialUV1
		{
			get { if( _materialUV1.BeginGet() ) MaterialUV1 = _materialUV1.Get( this ); return _materialUV1.value; }
			set
			{
				if( _materialUV1.BeginSet( this, ref value ) )
				{
					try
					{
						MaterialUV1Changed?.Invoke( this );

						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Geometry );
							UpdateTiles();
						}
					}
					finally { _materialUV1.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialUV1"/> property value changes.</summary>
		public event Action<Terrain> MaterialUV1Changed;
		ReferenceField<Vector2> _materialUV1 = Vector2.One;

		/// <summary>
		/// The intensity of the curvature in the calculation of texture coordinates. The curvature is intended to reduce the tiling effect.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 4 )]
		[DisplayName( "Material UV Curvature Intensity" )]
		[Category( "Display" )]
		public Reference<double> MaterialUVCurvatureIntensity
		{
			get { if( _materialUVCurvatureIntensity.BeginGet() ) MaterialUVCurvatureIntensity = _materialUVCurvatureIntensity.Get( this ); return _materialUVCurvatureIntensity.value; }
			set
			{
				if( _materialUVCurvatureIntensity.BeginSet( this, ref value ) )
				{
					try
					{
						MaterialUVCurvatureIntensityChanged?.Invoke( this );

						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Geometry );
							UpdateTiles();
						}
					}
					finally { _materialUVCurvatureIntensity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialUVCurvatureIntensity"/> property value changes.</summary>
		public event Action<Terrain> MaterialUVCurvatureIntensityChanged;
		ReferenceField<double> _materialUVCurvatureIntensity = 0.0;

		/// <summary>
		/// The frequency of the curvature in the calculation of texture coordinates. The randomness is intended to reduce the tiling effect.
		/// </summary>
		[DefaultValue( 0.1 )]
		[Range( 0, 1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DisplayName( "Material UV Curvature Frequency" )]
		[Category( "Display" )]
		public Reference<double> MaterialUVCurvatureFrequency
		{
			get { if( _materialUVCurvatureFrequency.BeginGet() ) MaterialUVCurvatureFrequency = _materialUVCurvatureFrequency.Get( this ); return _materialUVCurvatureFrequency.value; }
			set
			{
				if( _materialUVCurvatureFrequency.BeginSet( this, ref value ) )
				{
					try
					{
						MaterialUVCurvatureFrequencyChanged?.Invoke( this );

						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Geometry );
							UpdateTiles();
						}
					}
					finally { _materialUVCurvatureFrequency.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialUVCurvatureFrequency"/> property value changes.</summary>
		public event Action<Terrain> MaterialUVCurvatureFrequencyChanged;
		ReferenceField<double> _materialUVCurvatureFrequency = 0.1;

		/// <summary>
		/// The base color multiplier for base material.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Display" )]
		public Reference<ColorValue> MaterialColor
		{
			get { if( _materialColor.BeginGet() ) MaterialColor = _materialColor.Get( this ); return _materialColor.value; }
			set
			{
				if( _materialColor.BeginSet( this, ref value ) )
				{
					try
					{
						MaterialColorChanged?.Invoke( this );

						foreach( var tile in GetTiles() )
						{
							if( tile.ObjectInSpace != null )
								tile.ObjectInSpace.Color = _materialColor.value;
						}
					}
					finally { _materialColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialColor"/> property value changes.</summary>
		public event Action<Terrain> MaterialColorChanged;
		ReferenceField<ColorValue> _materialColor = ColorValue.One;

		public enum PaintMaskSizeEnum
		{
			//_128x128,
			//_256x256,
			_512x512,
			_1024x1024,
			_2048x2048,
			_4096x4096,
		}

		/// <summary>
		/// Resolution of the paint masks of the layers.
		/// </summary>
		[DefaultValue( PaintMaskSizeEnum._1024x1024 )]
		[Category( "Display" )]
		public Reference<PaintMaskSizeEnum> PaintMaskSize
		{
			get { if( _paintMaskSize.BeginGet() ) PaintMaskSize = _paintMaskSize.Get( this ); return _paintMaskSize.value; }
			set
			{
				if( _paintMaskSize.BeginSet( this, ref value ) )
				{
					try
					{
						PaintMaskSizeChanged?.Invoke( this );
						RecreateInternalData();
					}
					finally { _paintMaskSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="PaintMaskSize"/> property value changes.</summary>
		public event Action<Terrain> PaintMaskSizeChanged;
		ReferenceField<PaintMaskSizeEnum> _paintMaskSize = PaintMaskSizeEnum._1024x1024;

		/// <summary>
		/// Whether to cast shadows on the other surfaces.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Display" )]
		public Reference<bool> CastShadows
		{
			get { if( _castShadows.BeginGet() ) CastShadows = _castShadows.Get( this ); return _castShadows.value; }
			set
			{
				if( _castShadows.BeginSet( this, ref value ) )
				{
					try
					{
						CastShadowsChanged?.Invoke( this );

						foreach( var tile in GetTiles() )
						{
							if( tile.ObjectInSpace != null )
								tile.ObjectInSpace.CastShadows = _castShadows.value;

							//!!!!paint layer objects
						}
					}
					finally { _castShadows.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CastShadows"/> property value changes.</summary>
		public event Action<Terrain> CastShadowsChanged;
		ReferenceField<bool> _castShadows = false;

		/// <summary>
		/// Whether it is possible to apply decals on the surface.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Display" )]
		public Reference<bool> ReceiveDecals
		{
			get { if( _receiveDecals.BeginGet() ) ReceiveDecals = _receiveDecals.Get( this ); return _receiveDecals.value; }
			set
			{
				if( _receiveDecals.BeginSet( this, ref value ) )
				{
					try
					{
						ReceiveDecalsChanged?.Invoke( this );

						foreach( var tile in GetTiles() )
						{
							if( tile.ObjectInSpace != null )
								tile.ObjectInSpace.ReceiveDecals = _receiveDecals.value;

							//!!!!paint layer objects
						}
					}
					finally { _receiveDecals.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<Terrain> ReceiveDecalsChanged;
		ReferenceField<bool> _receiveDecals = true;

		/// <summary>
		/// Base surface of the terrain.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Display" )]
		public Reference<Surface> Surface
		{
			get { if( _surface.BeginGet() ) Surface = _surface.Get( this ); return _surface.value; }
			set
			{
				if( _surface.BeginSet( this, ref value ) )
				{
					try
					{
						SurfaceChanged?.Invoke( this );

						baseSurfaceObjectsDataCache = null;

						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Layers );
							UpdateTiles();
						}
					}
					finally { _surface.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Surface"/> property value changes.</summary>
		public event Action<Terrain> SurfaceChanged;
		ReferenceField<Surface> _surface = null;

		/// <summary>
		/// Whether to create objects of the base surface.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Display" )]
		public Reference<bool> SurfaceObjects
		{
			get { if( _surfaceObjects.BeginGet() ) SurfaceObjects = _surfaceObjects.Get( this ); return _surfaceObjects.value; }
			set
			{
				if( _surfaceObjects.BeginSet( this, ref value ) )
				{
					try
					{
						SurfaceObjectsChanged?.Invoke( this );

						baseSurfaceObjectsDataCache = null;

						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Layers );
							UpdateTiles();
						}
					}
					finally { _surfaceObjects.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SurfaceObjects"/> property value changes.</summary>
		public event Action<Terrain> SurfaceObjectsChanged;
		ReferenceField<bool> _surfaceObjects = true;

		/// <summary>
		/// The scale the distribution of base surface objects.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Display" )]
		public Reference<double> SurfaceObjectsDistribution
		{
			get { if( _surfaceObjectsDistribution.BeginGet() ) SurfaceObjectsDistribution = _surfaceObjectsDistribution.Get( this ); return _surfaceObjectsDistribution.value; }
			set
			{
				if( _surfaceObjectsDistribution.BeginSet( this, ref value ) )
				{
					try
					{
						SurfaceObjectsDistributionChanged?.Invoke( this );

						baseSurfaceObjectsDataCache = null;

						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Layers );
							UpdateTiles();
						}
					}
					finally { _surfaceObjectsDistribution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SurfaceObjectsDistribution"/> property value changes.</summary>
		public event Action<Terrain> SurfaceObjectsDistributionChanged;
		ReferenceField<double> _surfaceObjectsDistribution = 1.0;

		/// <summary>
		/// The scale of base surface objects size.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Display" )]
		public Reference<double> SurfaceObjectsScale
		{
			get { if( _surfaceObjectsScale.BeginGet() ) SurfaceObjectsScale = _surfaceObjectsScale.Get( this ); return _surfaceObjectsScale.value; }
			set
			{
				if( _surfaceObjectsScale.BeginSet( this, ref value ) )
				{
					try
					{
						SurfaceObjectsScaleChanged?.Invoke( this );

						baseSurfaceObjectsDataCache = null;

						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Layers );
							UpdateTiles();
						}
					}
					finally { _surfaceObjectsScale.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SurfaceObjectsScale"/> property value changes.</summary>
		public event Action<Terrain> SurfaceObjectsScaleChanged;
		ReferenceField<double> _surfaceObjectsScale = 1.0;

		/// <summary>
		/// The base color multiplier for objects of base surface.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Display" )]
		public Reference<ColorValue> SurfaceObjectsColor
		{
			get { if( _surfaceObjectsColor.BeginGet() ) SurfaceObjectsColor = _surfaceObjectsColor.Get( this ); return _surfaceObjectsColor.value; }
			set
			{
				if( _surfaceObjectsColor.BeginSet( this, ref value ) )
				{
					try
					{
						SurfaceObjectsColorChanged?.Invoke( this );

						baseSurfaceObjectsDataCache = null;

						//!!!!не всё обновлять. где еще так
						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Layers );
							UpdateTiles();
						}
					}
					finally { _surfaceObjectsColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SurfaceObjectsColor"/> property value changes.</summary>
		public event Action<Terrain> SurfaceObjectsColorChanged;
		ReferenceField<ColorValue> _surfaceObjectsColor = ColorValue.One;

		/// <summary>
		/// The factor of maximum visibility distance for objects of base surface. The maximum distance is calculated based on the size of the object on the screen.
		/// </summary>
		[Category( "Display" )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DefaultValue( 1.0 )]
		public Reference<double> SurfaceObjectsVisibilityDistanceFactor
		{
			get { if( _surfaceObjectsVisibilityDistanceFactor.BeginGet() ) SurfaceObjectsVisibilityDistanceFactor = _surfaceObjectsVisibilityDistanceFactor.Get( this ); return _surfaceObjectsVisibilityDistanceFactor.value; }
			set
			{
				if( _surfaceObjectsVisibilityDistanceFactor.BeginSet( this, ref value ) )
				{
					try
					{
						SurfaceObjectsVisibilityDistanceFactorChanged?.Invoke( this );

						baseSurfaceObjectsDataCache = null;

						//!!!!не всё обновлять. где еще так
						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Layers );
							UpdateTiles();
						}
					}
					finally { _surfaceObjectsVisibilityDistanceFactor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SurfaceObjectsVisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<Terrain> SurfaceObjectsVisibilityDistanceFactorChanged;
		ReferenceField<double> _surfaceObjectsVisibilityDistanceFactor = 1.0;

		/// <summary>
		/// Whether to cast shadows on the other surfaces for objects of base surface.
		/// </summary>
		[Category( "Display" )]
		[DefaultValue( true )]
		public Reference<bool> SurfaceObjectsCastShadows
		{
			get { if( _surfaceObjectsCastShadows.BeginGet() ) SurfaceObjectsCastShadows = _surfaceObjectsCastShadows.Get( this ); return _surfaceObjectsCastShadows.value; }
			set
			{
				if( _surfaceObjectsCastShadows.BeginSet( this, ref value ) )
				{
					try
					{
						SurfaceObjectsCastShadowsChanged?.Invoke( this );

						baseSurfaceObjectsDataCache = null;

						//!!!!не всё обновлять. где еще так
						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Layers );
							UpdateTiles();
						}
					}
					finally { _surfaceObjectsCastShadows.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SurfaceObjectsCastShadows"/> property value changes.</summary>
		public event Action<Terrain> SurfaceObjectsCastShadowsChanged;
		ReferenceField<bool> _surfaceObjectsCastShadows = true;

		/// <summary>
		/// Whether to enable a collision detection. A collision definition of the mesh is used.
		/// </summary>
		[Category( "Display" )]
		[DefaultValue( false )]
		public Reference<bool> SurfaceObjectsCollision
		{
			get { if( _surfaceObjectsCollision.BeginGet() ) SurfaceObjectsCollision = _surfaceObjectsCollision.Get( this ); return _surfaceObjectsCollision.value; }
			set
			{
				if( _surfaceObjectsCollision.BeginSet( this, ref value ) )
				{
					try
					{
						SurfaceObjectsCollisionChanged?.Invoke( this );

						baseSurfaceObjectsDataCache = null;

						//!!!!не всё обновлять. где еще так
						if( EnabledInHierarchyAndIsInstance )
						{
							SetNeedUpdateAllTiles( NeedUpdateEnum.Layers );
							UpdateTiles();
						}
					}
					finally { _surfaceObjectsCollision.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SurfaceObjectsCollision"/> property value changes.</summary>
		public event Action<Terrain> SurfaceObjectsCollisionChanged;
		ReferenceField<bool> _surfaceObjectsCollision = false;


		//!!!!
		//LODScale
		//public Material replaceMaterial;
		//public bool receiveDecals;
		//public float motionBlurFactor;



		/// <summary>
		/// The multiplier of the motion blur effect.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Display" )]
		public Reference<double> MotionBlurFactor
		{
			get { if( _motionBlurFactor.BeginGet() ) MotionBlurFactor = _motionBlurFactor.Get( this ); return _motionBlurFactor.value; }
			set
			{
				if( _motionBlurFactor.BeginSet( this, ref value ) )
				{
					try
					{
						MotionBlurFactorChanged?.Invoke( this );

						foreach( var tile in GetTiles() )
						{
							if( tile.ObjectInSpace != null )
								tile.ObjectInSpace.MotionBlurFactor = _motionBlurFactor.value;

							//!!!!paint layer objects
						}
					}
					finally { _motionBlurFactor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MotionBlurFactor"/> property value changes.</summary>
		public event Action<Terrain> MotionBlurFactorChanged;
		ReferenceField<double> _motionBlurFactor = 1.0;

		/// <summary>
		/// Whether to have a collision body.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Physics" )]
		public Reference<bool> Collision
		{
			get { if( _collision.BeginGet() ) Collision = _collision.Get( this ); return _collision.value; }
			set
			{
				if( _collision.BeginSet( this, ref value ) )
				{
					try
					{
						CollisionChanged?.Invoke( this );
						CreateCollisionBodies();
					}
					finally { _collision.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Collision"/> property value changes.</summary>
		public event Action<Terrain> CollisionChanged;
		ReferenceField<bool> _collision = true;

		/// <summary>
		/// The physical material used by the rigid body.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		[Category( "Physics" )]
		public Reference<PhysicalMaterial> CollisionMaterial
		{
			get { if( _collisionMaterial.BeginGet() ) CollisionMaterial = _collisionMaterial.Get( this ); return _collisionMaterial.value; }
			set
			{
				if( _collisionMaterial.BeginSet( this, ref value ) )
				{
					try
					{
						CollisionMaterialChanged?.Invoke( this );
						SetCollisionMaterial();
					}
					finally { _collisionMaterial.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionMaterial"/> property value changes.</summary>
		public event Action<Terrain> CollisionMaterialChanged;
		ReferenceField<PhysicalMaterial> _collisionMaterial;

		//!!!!
		///// <summary>
		///// The type of friction applied on the rigid body.
		///// </summary>
		//[DefaultValue( PhysicalMaterial.FrictionModeEnum.Simple )]
		//[Serialize]
		//[Category( "Physics" )]
		//public Reference<PhysicalMaterial.FrictionModeEnum> CollisionFrictionMode
		//{
		//	get { if( _collisionFrictionMode.BeginGet() ) CollisionFrictionMode = _collisionFrictionMode.Get( this ); return _collisionFrictionMode.value; }
		//	set
		//	{
		//		if( _collisionFrictionMode.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				CollisionFrictionModeChanged?.Invoke( this );
		//				SetCollisionMaterial();
		//			}
		//			finally { _collisionFrictionMode.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="CollisionFrictionMode"/> property value changes.</summary>
		//public event Action<Terrain> CollisionFrictionModeChanged;
		//ReferenceField<PhysicalMaterial.FrictionModeEnum> _collisionFrictionMode = PhysicalMaterial.FrictionModeEnum.Simple;

		/// <summary>
		/// The amount of friction applied on the rigid body.
		/// </summary>
		[Serialize]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		[Category( "Physics" )]
		public Reference<double> CollisionFriction
		{
			get { if( _collisionFriction.BeginGet() ) CollisionFriction = _collisionFriction.Get( this ); return _collisionFriction.value; }
			set
			{
				if( _collisionFriction.BeginSet( this, ref value ) )
				{
					try
					{
						CollisionFrictionChanged?.Invoke( this );
						SetCollisionMaterial();
					}
					finally { _collisionFriction.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionFriction"/> property value changes.</summary>
		public event Action<Terrain> CollisionFrictionChanged;
		ReferenceField<double> _collisionFriction = 0.5;

		//!!!!
		///// <summary>
		///// The amount of directional friction applied on the rigid body.
		///// </summary>
		//[DefaultValue( "1 1 1" )]
		//[Serialize]
		////[ApplicableRange( 0, 1 )]
		//[Category( "Physics" )]
		//public Reference<Vector3> CollisionAnisotropicFriction
		//{
		//	get { if( _collisionAnisotropicFriction.BeginGet() ) CollisionAnisotropicFriction = _collisionAnisotropicFriction.Get( this ); return _collisionAnisotropicFriction.value; }
		//	set
		//	{
		//		if( _collisionAnisotropicFriction.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				CollisionAnisotropicFrictionChanged?.Invoke( this );
		//				SetCollisionMaterial();
		//			}
		//			finally { _collisionAnisotropicFriction.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="CollisionAnisotropicFriction"/> property value changes.</summary>
		//public event Action<Terrain> CollisionAnisotropicFrictionChanged;
		//ReferenceField<Vector3> _collisionAnisotropicFriction = Vector3.One;

		//!!!!
		///// <summary>
		///// The amount of friction applied when rigid body is spinning.
		///// </summary>
		//[DefaultValue( 0.5 )]
		//[Serialize]
		//[Range( 0, 1 )]
		//[Category( "Physics" )]
		//public Reference<double> CollisionSpinningFriction
		//{
		//	get { if( _collisionSpinningFriction.BeginGet() ) CollisionSpinningFriction = _collisionSpinningFriction.Get( this ); return _collisionSpinningFriction.value; }
		//	set
		//	{
		//		if( _collisionSpinningFriction.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				CollisionSpinningFrictionChanged?.Invoke( this );
		//				SetCollisionMaterial();
		//			}
		//			finally { _collisionSpinningFriction.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="CollisionSpinningFriction"/> property value changes.</summary>
		//public event Action<Terrain> CollisionSpinningFrictionChanged;
		//ReferenceField<double> _collisionSpinningFriction = 0.5;

		//!!!!
		///// <summary>
		///// The amount of friction applied when rigid body is rolling.
		///// </summary>
		//[DefaultValue( 0.5 )]
		//[Serialize]
		//[Range( 0, 1 )]
		//[Category( "Physics" )]
		//public Reference<double> CollisionRollingFriction
		//{
		//	get { if( _collisionRollingFriction.BeginGet() ) CollisionRollingFriction = _collisionRollingFriction.Get( this ); return _collisionRollingFriction.value; }
		//	set
		//	{
		//		if( _collisionRollingFriction.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				CollisionRollingFrictionChanged?.Invoke( this );
		//				SetCollisionMaterial();
		//			}
		//			finally { _collisionRollingFriction.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="CollisionRollingFriction"/> property value changes.</summary>
		//public event Action<Terrain> CollisionRollingFrictionChanged;
		//ReferenceField<double> _collisionRollingFriction = 0.5;

		/// <summary>
		/// The ratio of the final relative velocity to initial relative velocity of the rigid body after collision.
		/// </summary>
		[Serialize]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Category( "Physics" )]
		public Reference<double> CollisionRestitution
		{
			get { if( _collisionRestitution.BeginGet() ) CollisionRestitution = _collisionRestitution.Get( this ); return _collisionRestitution.value; }
			set
			{
				if( _collisionRestitution.BeginSet( this, ref value ) )
				{
					try
					{
						CollisionRestitutionChanged?.Invoke( this );
						SetCollisionMaterial();
					}
					finally { _collisionRestitution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionRestitution"/> property value changes.</summary>
		public event Action<Terrain> CollisionRestitutionChanged;
		ReferenceField<double> _collisionRestitution;

		public enum TileSizeEnum
		{
			_8x8,
			_16x16,
			_32x32,
			_64x64,
			_128x128,
			_256x256,
		}

		/// <summary>
		/// The size of internal tile cells. These cells divide the entire geometry of the landscape into pieces.
		/// </summary>
		[DefaultValue( TileSizeEnum._64x64 )]
		[Category( "Optimization" )]
		public Reference<TileSizeEnum> TileSize
		{
			get { if( _tileSize.BeginGet() ) TileSize = _tileSize.Get( this ); return _tileSize.value; }
			set
			{
				if( _tileSize.BeginSet( this, ref value ) )
				{
					try
					{
						TileSizeChanged?.Invoke( this );
						tileSize = ParseTileSize( _tileSize.value );
						RecreateInternalData( true );
					}
					finally { _tileSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="TileSize"/> property value changes.</summary>
		public event Action<Terrain> TileSizeChanged;
		ReferenceField<TileSizeEnum> _tileSize = TileSizeEnum._64x64;

		/// <summary>
		/// Whether to enable level of detail.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Optimization" )]
		[DisplayName( "LOD Enabled" )]
		public Reference<bool> LODEnabled
		{
			get { if( _lODEnabled.BeginGet() ) LODEnabled = _lODEnabled.Get( this ); return _lODEnabled.value; }
			set { if( _lODEnabled.BeginSet( this, ref value ) ) { try { LODEnabledChanged?.Invoke( this ); RecreateInternalData( false ); } finally { _lODEnabled.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODEnabled"/> property value changes.</summary>
		public event Action<Terrain> LODEnabledChanged;
		ReferenceField<bool> _lODEnabled = true;

		[DefaultValue( 4 )]
		[Category( "Optimization" )]
		[DisplayName( "LOD Count" )]
		[Range( 1, 6 )]
		public Reference<int> LODCount
		{
			get { if( _lODCount.BeginGet() ) LODCount = _lODCount.Get( this ); return _lODCount.value; }
			set { if( _lODCount.BeginSet( this, ref value ) ) { try { LODCountChanged?.Invoke( this ); RecreateInternalData( false ); } finally { _lODCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODCount"/> property value changes.</summary>
		public event Action<Terrain> LODCountChanged;
		ReferenceField<int> _lODCount = 4;

		///// <summary>
		///// The range of levels of detail.
		///// </summary>
		//[DefaultValue( "0 10" )]
		//[Range( 0, 10 )]
		//[DisplayName( "LOD Range" )]
		//[Category( "Optimization" )]
		//public Reference<RangeI> LODRange
		//{
		//	get { if( _lODRange.BeginGet() ) LODRange = _lODRange.Get( this ); return _lODRange.value; }
		//	set { if( _lODRange.BeginSet( this, ref value ) ) { try { LODRangeChanged?.Invoke( this ); } finally { _lODRange.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LODRange"/> property value changes.</summary>
		//public event Action<Terrain> LODRangeChanged;
		//ReferenceField<RangeI> _lODRange = new RangeI( 0, 10 );

		//!!!!default
		/// <summary>
		/// The distance from the previous to the next level of detail.
		/// </summary>
		[DisplayName( "LOD Distance" )]
		[Category( "Optimization" )]
		[DefaultValue( 50.0 )]
		[Range( 1, 100 )]
		public Reference<double> LODDistance
		{
			get { if( _lODDistance.BeginGet() ) LODDistance = _lODDistance.Get( this ); return _lODDistance.value; }
			set
			{
				if( _lODDistance.BeginSet( this, ref value ) )
				{
					try
					{
						LODDistanceChanged?.Invoke( this );
						foreach( var tile in GetTiles() )
							tile.SetLODDistances();
					}
					finally { _lODDistance.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LODDistance"/> property value changes.</summary>
		public event Action<Terrain> LODDistanceChanged;
		ReferenceField<double> _lODDistance = 50.0;

		/// <summary>
		/// Whether to precalculate data of objects from layers in the simulation. When the property is disabled, the objects will calculate when it needed and destroyed after 10 seconds when are not visible by the camera.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Optimization" )]
		public Reference<bool> PrecalculateObjects
		{
			get { if( _precalculateObjects.BeginGet() ) PrecalculateObjects = _precalculateObjects.Get( this ); return _precalculateObjects.value; }
			set { if( _precalculateObjects.BeginSet( this, ref value ) ) { try { PrecalculateObjectsChanged?.Invoke( this ); } finally { _precalculateObjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PrecalculateObjects"/> property value changes.</summary>
		public event Action<Terrain> PrecalculateObjectsChanged;
		ReferenceField<bool> _precalculateObjects = true;

		///// <summary>
		///// The maximal amount of objects created from layers in one rendering group.
		///// </summary>
		//[DefaultValue( 2000 )]
		//[Category( "Optimization" )]
		//public Reference<int> MaxObjectsInGroup
		//{
		//	get { if( _maxObjectsInGroup.BeginGet() ) MaxObjectsInGroup = _maxObjectsInGroup.Get( this ); return _maxObjectsInGroup.value; }
		//	set
		//	{
		//		if( _maxObjectsInGroup.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				MaxObjectsInGroupChanged?.Invoke( this );

		//				if( EnabledInHierarchyAndIsInstance )
		//				{
		//					SetNeedUpdateAllTiles( NeedUpdateEnum.Layers );
		//					UpdateTiles();
		//				}
		//				//var groupOfObjects = GetOrCreateGroupOfObjects( false );
		//				//if( groupOfObjects != null )
		//				//	groupOfObjects.MaxObjectsInGroup = MaxObjectsInGroup;
		//			}
		//			finally { _maxObjectsInGroup.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="MaxObjectsInGroup"/> property value changes.</summary>
		//public event Action<Terrain> MaxObjectsInGroupChanged;
		//ReferenceField<int> _maxObjectsInGroup = 2000;

		//!!!!cut volumes

		/// <summary>
		/// Whether to object should be used as an occluder for occlusion culling.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Optimization" )]
		public Reference<bool> Occluder
		{
			get { if( _occluder.BeginGet() ) Occluder = _occluder.Get( this ); return _occluder.value; }
			set
			{
				if( _occluder.BeginSet( this, ref value ) )
				{
					try
					{
						OccluderChanged?.Invoke( this );
						SetNeedUpdateAllTiles( NeedUpdateEnum.Geometry );
					}
					finally { _occluder.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Occluder"/> property value changes.</summary>
		public event Action<Terrain> OccluderChanged;
		ReferenceField<bool> _occluder = false;

		/// <summary>
		/// Whether to visualize the bounds of groups for objects created by surfaces.
		/// </summary>
		[Category( "Debug" )]
		[DefaultValue( false )]
		public Reference<bool> DebugDrawSurfaceObjectsBounds
		{
			get { if( _debugDrawSurfaceObjectsBounds.BeginGet() ) DebugDrawSurfaceObjectsBounds = _debugDrawSurfaceObjectsBounds.Get( this ); return _debugDrawSurfaceObjectsBounds.value; }
			set { if( _debugDrawSurfaceObjectsBounds.BeginSet( this, ref value ) ) { try { DebugDrawSurfaceObjectsBoundsChanged?.Invoke( this ); } finally { _debugDrawSurfaceObjectsBounds.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawSurfaceObjectsBounds"/> property value changes.</summary>
		public event Action<Terrain> DebugDrawSurfaceObjectsBoundsChanged;
		ReferenceField<bool> _debugDrawSurfaceObjectsBounds = false;

		[Browsable( false )]
		public bool AllowFullUpdateGeometryCollisionCurrentLayers { get; set; } = true;

		/////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		struct Vertex
		{
			public Vector3F Position;
			//!!!!compress
			public Vector3F Normal;
			public Vector4F Tangent;
			public Vector2F TexCoord0;
			public Vector2F TexCoord1;
			public Vector2F TexCoord2;
		}

		/////////////////////////////////////////

		public enum NeedUpdateEnum
		{
			Geometry = 1,
			Layers = 2,
			Collision = 4,
			All = Geometry | Layers | Collision,
		}

		/////////////////////////////////////////

		internal class BaseSurfaceObjectsDataClass
		{
			public Surface Surface;
			public bool SurfaceObjects;
			public float SurfaceObjectsDistribution;
			public float SurfaceObjectsScale;
			public ColorValue SurfaceObjectsColor;
			public float SurfaceObjectsVisibilityDistanceFactor;
			public bool SurfaceObjectsCastShadows;
			public bool SurfaceObjectsCollision;

			//!!!!
		}

		/////////////////////////////////////////

		internal class Tile
		{
			public Terrain owner;

			public Vector2I index;
			public Vector2I cellIndexMin;
			public Vector2I cellIndexMax;

			public MeshInSpace ObjectInSpace;
			public Mesh Mesh;
			public bool? RenderingDataCreatedWithExtractData;

			public Vector3[] Vertices;
			public int[] Indices;
			public SpaceBounds Bounds;
			public Vector3 Position;
			public bool ContainsHoles;

			public Occluder Occluder;

			public Scene.PhysicsWorldClass.Body CollisionBody;
			//public RigidBody CollisionBody;

			//!!!!когда слоя нет в тайле. результирующая маска нулевая
			//!!!!!для материалов тоже полезно

			//!!!!impl. OnUpdate
			public double LastTimeVisibledByCamera;

			public SpaceBounds SurfaceObjectsBounds;

			public ObjectInSpace SurfaceObjectsObjectInSpace;
			public SurfaceObjectsItem[] SurfaceObjectsItems;
			public List<Scene.PhysicsWorldClass.Body> SurfaceObjectsCollisionBodies = new List<Scene.PhysicsWorldClass.Body>();
			//public RigidBody SurfaceObjectsCollisionBody;

			public NeedUpdateEnum NeedUpdate;

			////cached for masks
			//Vector2I masksTextureCoordMin = new Vector2I( -10000, -10000 );
			//Vector2I masksIndexMin = new Vector2I( -10000, -10000 );
			//Vector2I aoTextureCoordMin = new Vector2I( -10000, -10000 );
			//Vector2I aoIndexMin = new Vector2I( -10000, -10000 );

			/////////////////////

			public class SurfaceObjectsItem
			{
				public Tile Owner;

				public BaseSurfaceObjectsDataClass BaseSurfaceObjectsData;
				public RenderingPipeline.RenderSceneData.LayerItem Layer;

				//public float MaxVisibilityDistanceSquared;

				public Bounds Bounds;
				public float ObjectsMaxSize;
				public double ObjectsMaxVisibilityDistanceFactor;

				//!!!!bounding sphere?

				//!!!!camera context
				//!!!!как делать быструю загрузку вначале?

				public double RenderGroupsLastVisibleTime;
				public SurfaceObjectsRenderGroup[] RenderGroups;
				//public volatile bool RenderGroupsTaskData;
				//public volatile SurfaceObjectsRenderGroup[] RenderGroupsTaskResult;

				//

				public Surface GetSurface()
				{
					return BaseSurfaceObjectsData != null ? BaseSurfaceObjectsData.Surface : Layer.Surface;
				}

				public void GetObjectsColor( out ColorValue color )
				{
					color = BaseSurfaceObjectsData != null ? BaseSurfaceObjectsData.SurfaceObjectsColor : Layer.SurfaceObjectsColor;
				}

				public float GetObjectsDistribution()
				{
					return BaseSurfaceObjectsData != null ? BaseSurfaceObjectsData.SurfaceObjectsDistribution : Layer.SurfaceObjectsDistribution;
				}

				public float GetObjectsScale()
				{
					return BaseSurfaceObjectsData != null ? BaseSurfaceObjectsData.SurfaceObjectsScale : Layer.SurfaceObjectsScale;
				}

				public float GetObjectsVisibilityDistanceFactor()
				{
					return BaseSurfaceObjectsData != null ? BaseSurfaceObjectsData.SurfaceObjectsVisibilityDistanceFactor : Layer.SurfaceObjectsVisibilityDistanceFactor;
				}

				public bool GetObjectsCastShadows()
				{
					return BaseSurfaceObjectsData != null ? BaseSurfaceObjectsData.SurfaceObjectsCastShadows : Layer.SurfaceObjectsCastShadows;
				}

				public bool GetObjectsCollision()
				{
					return BaseSurfaceObjectsData != null ? BaseSurfaceObjectsData.SurfaceObjectsCollision : Layer.SurfaceObjectsCollision;
				}
			}

			/////////////////////

			public class SurfaceObjectsRenderGroup
			{
				//for transparent materials
				public bool NoBatchingGroup;

				//!!!!может хранить MeshResult и параметры
				public int/*byte */VariationGroup;
				public int/*byte */VariationElement;

				public struct ObjectItem
				{
					public Vector3 Position;
					public QuaternionF Rotation;//!!!!need only for billboard
					public Vector3F Scale;//!!!!need only for billboard

					public Matrix3F MeshMatrix3;
					//public Vector3 MeshPosition;
					//public Matrix4 MeshMatrix;

					//!!!!need only for separate rendering
					public Bounds BoundingBox;
					public Sphere BoundingSphere;
					//public ColorByte ColorForInstancingData;
				}
				public OpenList<ObjectItem> Objects = new OpenList<ObjectItem>();

				public Bounds BoundingBox;
				public Sphere BoundingSphere;

				public double ObjectsMaxScale;
				//public float VisibilityDistance;
				//public float VisibilityDistanceSquared;

				public GpuVertexBuffer BatchingInstanceBufferMesh;
				public GpuVertexBuffer BatchingInstanceBufferBillboard;
				public Vector3 BatchingInstancePositionOffset;

				////for RenderingDataUpdate() method
				//public int NextSplitAxis;
			}

			/////////////////////

			public Tile( Terrain owner, Vector2I index )
			{
				this.owner = owner;
				this.index = index;

				cellIndexMin = index * owner.tileSize;
				cellIndexMax = cellIndexMin + new Vector2I( owner.tileSize, owner.tileSize );
			}

			public override string ToString()
			{
				return index.ToString();
			}

			void GenerateTileVerticesInfo( int lodLevel, Vector2I[,] cellIndexes )
			{
				if( lodLevel >= owner.cachedLodLevelCount )
					Log.Fatal( "Terrain: Tile: GenerateTileVerticesInfo: lodLevel >= cachedLodLevelCount." );

				int step = 1 << lodLevel;
				for( int y = 0, indexY = 0; y < owner.tileSize + 1; y += step, indexY++ )
					for( int x = 0, indexX = 0; x < owner.tileSize + 1; x += step, indexX++ )
						cellIndexes[ indexX, indexY ] = cellIndexMin + new Vector2I( x, y );
			}

			unsafe void CalculateGeometryAndBounds( Vector3[,] calculatedPositionsForGetNormals, bool updateHoles, bool allowHoles, out byte[] verticesBytes, out bool supportsLODs, out List<(byte[], int[])> lods )//, out int[] indices )
			{
				//int lodLevel = 0;
				int vertexCountByAxis = ( owner.tileSize /*>> lodLevel*/ ) + 1;
				int vertexCount = vertexCountByAxis * vertexCountByAxis;
				var cellIndices = owner.GenerateTileVerticesCellIndices( this, 0 );// lodLevel );

				var boundsSize = owner.cachedBounds.GetSize().ToVector2();
				var cachedBoundsSizeInv = Vector2.Zero;
				if( boundsSize.X != 0 && boundsSize.Y != 0 )
					cachedBoundsSizeInv = 1.0 / boundsSize;
				var materialUV0 = owner.MaterialUV0.Value;
				var materialUV1 = owner.MaterialUV1.Value;
				var materialUVCurvatureIntensity = (float)owner.MaterialUVCurvatureIntensity.Value;
				var materialUVCurvatureFrequency = (float)owner.MaterialUVCurvatureFrequency.Value;


				//calculate Vertices, Indices
				Vertices = new Vector3[ vertexCount ];
				for( int n = 0; n < vertexCount; n++ )
					owner.GetPosition( cellIndices[ n ], out Vertices[ n ] );
				Indices = owner.GenerateTileIndicesWithoutHoles( 0 );

				//calculate Bounds, Position
				var bounds = NeoAxis.Bounds.Cleared;
				for( int n = 0; n < Vertices.Length; n++ )
					bounds.Add( ref Vertices[ n ] );
				//!!!!bounding sphere
				Bounds = new SpaceBounds( bounds );
				Position = bounds.GetCenter();

				ContainsHoles = false;

				void AddCurvature( ref Vertex vertex )
				{
					if( materialUVCurvatureIntensity != 0 )
					{
						var offset = new Vector2F( MathEx.Sin( vertex.Position.Y * materialUVCurvatureFrequency ), MathEx.Sin( vertex.Position.X * materialUVCurvatureFrequency ) ) * materialUVCurvatureIntensity;
						//var offset = new Vector2F( MathEx.Sin( vertex.Position.Y * materialUVCurvatureFrequency ), MathEx.Cos( vertex.Position.X * materialUVCurvatureFrequency ) ) * materialUVCurvatureIntensity;
						vertex.TexCoord0 += offset;
						vertex.TexCoord1 += offset;

						//vertex.TexCoord0 += new Vector2F( MathEx.Sin( vertex.Position.Y * materialUVCurvatureFrequency ), MathEx.Cos( vertex.Position.X * materialUVCurvatureFrequency ) ) * materialUVCurvatureIntensity;
						//vertex.TexCoord1 += new Vector2F( MathEx.Sin( vertex.Position.Y * materialUVCurvatureFrequency ), MathEx.Cos( vertex.Position.X *
						//materialUVCurvatureFrequency ) ) * materialUVCurvatureIntensity;
					}
				}

				//calculate vertex buffer data
				var vertices = new Vertex[ Vertices.Length ];
				{
					Parallel.For( 0, vertices.Length, delegate ( int index )
					{
						var position = Vertices[ index ];
						owner.GetNormal( cellIndices[ index ], calculatedPositionsForGetNormals, out var normal );

						ref var vertex = ref vertices[ index ];

						vertex.Position = ( position - Position ).ToVector3F();
						vertex.Normal = normal;
						vertex.Tangent = new Vector4F( normal.Z, normal.Y, -normal.X, -1 );

						var offset = ( position - owner.cachedBounds.Minimum ).ToVector2();
						var uv0 = materialUV0 * offset;
						vertex.TexCoord0 = new Vector2( uv0.X, 1.0 - uv0.Y ).ToVector2F();
						var uv1 = materialUV1 * offset;
						vertex.TexCoord1 = new Vector2( uv1.X, 1.0 - uv1.Y ).ToVector2F();
						var uv2 = offset * cachedBoundsSizeInv;
						vertex.TexCoord2 = new Vector2( uv2.X, 1.0 - uv2.Y ).ToVector2F();
						AddCurvature( ref vertex );
					} );
				}

				supportsLODs = true;

				if( updateHoles )
					owner.HolesCacheSetItem( index, null, null );

				//holes
				if( allowHoles && owner.HolesEnabled )
				{
					if( updateHoles )
					{
						//update holes
						var holes = owner.GetHoleObjects();
						if( holes.Count != 0 )
						{
							var foundHole = false;
							var noData = false;
							Internal.Net3dBool.Solid currentSolid = null;

							foreach( var hole in holes )
							{
								////еще не посчитан при загрузке. объект еще не EnabledInHierarchy, т.к. ниже в иерархии
								if( hole.SpaceBounds.BoundingBox.Intersects( ref bounds ) )
								{
									var mesh = hole.Mesh.Value;
									if( mesh != null )
									{
										try
										{
											////is not best solution
											////mesh can be still not initialized
											//if( mesh.Result == null )
											//	mesh.ResultCompile();

											if( mesh.Result != null && mesh.Result.ExtractedIndices.Length != 0 )
											{
												//get initial data
												if( currentSolid == null )
												{
													var boolVertices2 = new Internal.Net3dBool.Vector3[ Vertices.Length ];
													for( int n = 0; n < Vertices.Length; n++ )
													{
														ref var v = ref Vertices[ n ];
														boolVertices2[ n ] = new Internal.Net3dBool.Vector3( v.X, v.Y, v.Z );
													}
													currentSolid = new Internal.Net3dBool.Solid( boolVertices2, Indices );
												}

												Internal.Net3dBool.Solid holeSolid;
												{
													var transform = hole.TransformV;

													var boolVertices = new Internal.Net3dBool.Vector3[ mesh.Result.ExtractedVerticesPositions.Length ];
													for( int n = 0; n < boolVertices.Length; n++ )
													{
														//!!!!slowly
														var v = transform * mesh.Result.ExtractedVerticesPositions[ n ];
														boolVertices[ n ] = new Internal.Net3dBool.Vector3( v.X, v.Y, v.Z );
													}
													holeSolid = new Internal.Net3dBool.Solid( boolVertices, mesh.Result.ExtractedIndices );
												}

												var modeller = new Internal.Net3dBool.BooleanModeller( currentSolid, holeSolid );
												var result = modeller.GetDifference();

												if( !result.isEmpty() )
												{
													currentSolid = result;
													foundHole = true;
												}
												else
												{
													//no data
													noData = true;
													goto end;
												}
											}

										}
										catch( Exception e )
										{
											Log.Warning( "Terrain: CalculateGeometryAndBounds: Unable to process hole generation. " + e.Message );
										}

									}
								}
							}

							//update data
							if( foundHole )
							{
								var resultVertices = currentSolid.getVertices();
								var resultIndices = currentSolid.getIndices();

								//calculate Vertices, Indices
								Vertices = new Vector3[ resultVertices.Length ];
								for( int n = 0; n < Vertices.Length; n++ )
								{
									ref var v = ref resultVertices[ n ];
									Vertices[ n ] = new Vector3( v.x, v.y, v.z );
								}
								Indices = resultIndices;

								//calculate Bounds, Position
								bounds = NeoAxis.Bounds.Cleared;
								for( int n = 0; n < Vertices.Length; n++ )
									bounds.Add( ref Vertices[ n ] );
								Bounds = new SpaceBounds( bounds );
								Position = bounds.GetCenter();
								ContainsHoles = true;

								//calculate vertex buffer data
								vertices = new Vertex[ Vertices.Length ];
								{
									Parallel.For( 0, vertices.Length, delegate ( int index )
									{
										var position = Vertices[ index ];

										var position2 = position.ToVector2() + new Vector2F( 0.001f, 0.001f );
										owner.GetNormal( ref position2, calculatedPositionsForGetNormals, out var normal );
										//var normal = owner.GetNormal( position.ToVector2() + new Vector2F( 0.001f, 0.001f ) );

										ref var vertex = ref vertices[ index ];

										vertex.Position = ( position - Position ).ToVector3F();
										vertex.Normal = normal;
										vertex.Tangent = new Vector4F( normal.Z, normal.Y, -normal.X, -1 );

										var offset = ( position - owner.cachedBounds.Minimum ).ToVector2();
										var uv0 = materialUV0 * offset;
										vertex.TexCoord0 = new Vector2( uv0.X, 1.0 - uv0.Y ).ToVector2F();
										var uv1 = materialUV1 * offset;
										vertex.TexCoord1 = new Vector2( uv1.X, 1.0 - uv1.Y ).ToVector2F();
										var uv2 = offset * cachedBoundsSizeInv;
										vertex.TexCoord2 = new Vector2( uv2.X, 1.0 - uv2.Y ).ToVector2F();
										AddCurvature( ref vertex );
									} );
								}

								supportsLODs = false;

								//save to the cache
								owner.HolesCacheSetItem( index, Vertices, Indices );
							}

end:;

							if( noData )
							{
								vertices = new Vertex[ 0 ];
								Vertices = new Vector3[ 0 ];
								Indices = new int[ 0 ];
								supportsLODs = false;

								//save to the cache
								owner.HolesCacheSetItem( index, Vertices, Indices );
							}
						}
					}
					else
					{
						//try get from the cache
						if( owner.HolesCacheGetItem( index, out var holeVertices, out var holeIndices ) )
						{
							//calculate Vertices, Indices
							Vertices = holeVertices;
							Indices = holeIndices;

							//calculate Bounds, Position
							bounds = NeoAxis.Bounds.Cleared;
							for( int n = 0; n < Vertices.Length; n++ )
								bounds.Add( ref Vertices[ n ] );
							Bounds = new SpaceBounds( bounds );
							Position = bounds.GetCenter();
							ContainsHoles = true;

							//calculate vertex buffer data
							vertices = new Vertex[ Vertices.Length ];
							{
								Parallel.For( 0, vertices.Length, delegate ( int index )
								{
									var position = Vertices[ index ];

									var position2 = position.ToVector2() + new Vector2F( 0.001f, 0.001f );
									owner.GetNormal( ref position2, calculatedPositionsForGetNormals, out var normal );
									//var normal = owner.GetNormal( position.ToVector2() + new Vector2F( 0.001f, 0.001f ) );

									ref var vertex = ref vertices[ index ];

									vertex.Position = ( position - Position ).ToVector3F();
									vertex.Normal = normal;
									vertex.Tangent = new Vector4F( normal.Z, normal.Y, -normal.X, -1 );

									var offset = ( position - owner.cachedBounds.Minimum ).ToVector2();
									var uv0 = materialUV0 * offset;
									vertex.TexCoord0 = new Vector2( uv0.X, 1.0 - uv0.Y ).ToVector2F();
									var uv1 = materialUV1 * offset;
									vertex.TexCoord1 = new Vector2( uv1.X, 1.0 - uv1.Y ).ToVector2F();
									var uv2 = offset * cachedBoundsSizeInv;
									vertex.TexCoord2 = new Vector2( uv2.X, 1.0 - uv2.Y ).ToVector2F();
									AddCurvature( ref vertex );
								} );
							}

							supportsLODs = false;
						}
					}
				}

				//set out verticesBytes
				fixed( Vertex* pVertices = vertices )
				{
					verticesBytes = new byte[ vertices.Length * sizeof( Vertex ) ];
					fixed( byte* pVerticesBytes = verticesBytes )
						NativeUtility.CopyMemory( pVerticesBytes, pVertices, verticesBytes.Length );
				}

				if( !owner.LODEnabled )
					supportsLODs = false;

				//generate LODs
				if( supportsLODs )
				{
					lods = new List<(byte[], int[])>();

					for( int lodIndex = 1; lodIndex < ( owner.LODCount.Value - 1 ); lodIndex++ )
					{
						var level = lodIndex;// * 2;

						//!!!!лишние вершины

						var fixedCellIndices = new List<Vector2I>( cellIndices.Length );

						var centerIndex = ( cellIndexMin + cellIndexMax ) / 2;

						foreach( var cellIndex in cellIndices )
						{
							var index = cellIndex - cellIndexMin;

							if( cellIndex.X != cellIndexMin.X && cellIndex.X != cellIndexMax.X &&
								cellIndex.Y != cellIndexMin.Y && cellIndex.Y != cellIndexMax.Y )
							{
								if( cellIndex.X > centerIndex.X )
								{
									int x = index.X >> level;
									index.X = x << level;
								}
								else
								{
									int x = ( -index.X ) >> level;
									index.X = -( x << level );
								}

								if( cellIndex.Y > centerIndex.Y )
								{
									int y = index.Y >> level;
									index.Y = y << level;
								}
								else
								{
									int y = ( -index.Y ) >> level;
									index.Y = -( y << level );
								}
							}

							fixedCellIndices.Add( cellIndexMin + index );
						}

						//calculate Vertices, Indices
						var lodVerticesPosition = new Vector3[ fixedCellIndices.Count ];
						for( int n = 0; n < fixedCellIndices.Count; n++ )
							owner.GetPosition( fixedCellIndices[ n ], out lodVerticesPosition[ n ] );

						//!!!!
						var lodIndices = owner.GenerateTileIndicesWithoutHoles( 0 );

						var fixedLodIndices = new List<int>( lodIndices.Length );
						for( int nTriangle = 0; nTriangle < lodIndices.Length / 3; nTriangle++ )
						{
							var index0 = lodIndices[ nTriangle * 3 + 0 ];
							var index1 = lodIndices[ nTriangle * 3 + 1 ];
							var index2 = lodIndices[ nTriangle * 3 + 2 ];
							var v0 = lodVerticesPosition[ index0 ];
							var v1 = lodVerticesPosition[ index1 ];
							var v2 = lodVerticesPosition[ index2 ];

							if( v0 != v1 && v0 != v2 && v1 != v2 )
							{
								fixedLodIndices.Add( index0 );
								fixedLodIndices.Add( index1 );
								fixedLodIndices.Add( index2 );
							}
						}

						//calculate vertex buffer data
						var lodVertices = new Vertex[ lodVerticesPosition.Length ];
						{
							Parallel.For( 0, lodVertices.Length, delegate ( int index )
							{
								var position = lodVerticesPosition[ index ];
								owner.GetNormal( fixedCellIndices[ index ], calculatedPositionsForGetNormals, out var normal );

								//var position = Vertices[ index ];
								//var normal = owner.GetNormal( position.ToVector2() + new Vector2F( 0.001f, 0.001f ) );

								ref var vertex = ref lodVertices[ index ];

								vertex.Position = ( position - Position ).ToVector3F();
								vertex.Normal = normal;
								vertex.Tangent = new Vector4F( normal.Z, normal.Y, -normal.X, -1 );

								var offset = ( position - owner.cachedBounds.Minimum ).ToVector2();
								var uv0 = materialUV0 * offset;
								vertex.TexCoord0 = new Vector2( uv0.X, 1.0 - uv0.Y ).ToVector2F();
								var uv1 = materialUV1 * offset;
								vertex.TexCoord1 = new Vector2( uv1.X, 1.0 - uv1.Y ).ToVector2F();
								var uv2 = offset * cachedBoundsSizeInv;
								vertex.TexCoord2 = new Vector2( uv2.X, 1.0 - uv2.Y ).ToVector2F();
								AddCurvature( ref vertex );
							} );
						}

						//set out verticesBytes
						byte[] lodVerticesBytes;
						fixed( Vertex* pVertices = lodVertices )
						{
							lodVerticesBytes = new byte[ lodVertices.Length * sizeof( Vertex ) ];
							fixed( byte* pVerticesBytes = lodVerticesBytes )
								NativeUtility.CopyMemory( pVerticesBytes, pVertices, lodVerticesBytes.Length );
						}

						lods.Add( (lodVerticesBytes, fixedLodIndices.ToArray()) );
					}
				}
				else
					lods = null;


				//var vertexCellIndexes = new Vector2I[ vertexCountByAxis, vertexCountByAxis ];
				//GenerateTileVerticesInfo( lodLevel, vertexCellIndexes );

				//Create vertexData
				//VertexData vertexData = CreateTileVertexData( vertexCount, tile.layers.Count );
				//tile.vertexDatasForNonSharedVertexBuffersMode[ lodLevel ] = vertexData;

				////calculate vertex positions and normals
				//var positions = new Vector3[ vertexCountByAxis, vertexCountByAxis ];
				//var normals = new Vector3[ vertexCountByAxis, vertexCountByAxis ];
				//for( int y = 0; y < vertexCountByAxis; y++ )
				//{
				//	for( int x = 0; x < vertexCountByAxis; x++ )
				//	{
				//		var cellIndex = vertexCellIndexes[ x, y ];
				//		positions[ x, y ] = owner.GetPosition( cellIndex );
				//		if( normals != null )
				//			normals[ x, y ] = owner.GetNormal( cellIndex );// GetNormal( tile, cellIndex );
				//	}
				//}


				//{

				//	bool needUndoForNextActions = true;
				//	CommonFunctions.ConvertProceduralMeshGeometries( document, firstMeshInSpace.Mesh, undo, ref needUndoForNextActions );

				//	List<(Vector3F[] positions, int[] indices)> data1List = GetData( firstMeshInSpace );
				//	(Vector3F[] positions, int[] indices) data2 = MergeData( GetData( secondMeshInSpace ) );

				//	//convert the second mesh in space, to the transform of first mesh in space
				//	var matrix = firstMeshInSpace.Transform.Value.ToMatrix4().GetInverse() * secondMeshInSpace.Transform.Value.ToMatrix4();
				//	Net3dBool.Vector3[] vertices2 = new Net3dBool.Vector3[ data2.positions.Length ];
				//	for( int i = 0; i < data2.positions.Length; i++ )
				//		vertices2[ i ] = ToNet3DBoolVector3( ( matrix * data2.positions[ i ] ).ToVector3F() );
				//	var operand2 = new Net3dBool.Solid( vertices2, data2.indices );

				//	var geometries = firstMeshInSpace.Mesh.Value.GetComponents<MeshGeometry>();
				//	var resultGeometries = new List<(Vector3F[] positions, int[] indices, MeshData.MeshGeometryFormat format)>();
				//	var geometriesToDelete = new List<MeshGeometry>();

				//	for( int geomIndex = 0; geomIndex < data1List.Count; geomIndex++ )
				//	{
				//		var data1 = data1List[ geomIndex ];
				//		var vertices1 = data1.positions.Select( ToNet3DBoolVector3 ).ToArray();

				//		var modeller = new Net3dBool.BooleanModeller( new Net3dBool.Solid( vertices1, data1.indices ), operand2 );
				//		var result = modeller.GetDifference();

				//		var newVertices = result.getVertices().Select( ToVector3F ).ToArray();
				//		if( 0 < newVertices.Length )
				//			resultGeometries.Add( (newVertices, result.getIndices(), new MeshData.MeshGeometryFormat( geometries[ geomIndex ].VertexStructure )) );
				//		else
				//			geometriesToDelete.Add( geometries[ geomIndex ] );
				//	}

				//	//var meshData = MeshData.BuildFromRaw( resultGeometries );
				//	//meshData?.Save( firstMeshInSpace.Mesh.Value, needUndoForNextActions ? undo : null );
				//	//firstMeshInSpace.Mesh.Value?.RebuildStructure();


				//}



				//foreach( Vector2I cellIndex in cellIndices )
				//{
				//	var position = owner.GetPosition( cellIndex );
				//	var normal = owner.GetNormal( cellIndex );

				//	Vertex vertex = new Vertex();
				//	//!!!!double
				//	vertex.Position = position.ToVector3F();
				//	vertex.Normal = normal;
				//	vertex.Tangent = new Vector4F( normal.Z, normal.Y, -normal.X, -1 );
				//	//!!!!double. или врапать
				//	var offset = position - owner.cachedBounds.Minimum;
				//	var uv = owner.MaterialUVTilesPerUnit.Value * offset.ToVector2();
				//	vertex.TexCoord0 = new Vector2( uv.X, 1.0 - uv.Y ).ToVector2F();

				//	vertices.Add( ref vertex );
				//}

				//List<Vector3> vertices = GenerateTileVertices( tile, 0 );

				//fixed ( Vertex* pVertices = vertices )
				//{
				//	verticesBytes = new byte[ vertices.Length * sizeof( Vertex ) ];
				//	fixed ( byte* pVerticesBytes = verticesBytes )
				//		NativeUtility.CopyMemory( pVerticesBytes, pVertices, verticesBytes.Length );
				//}

				//if( streamingEnable )
				//{
				//}
				//else
				//{

				//var heightMin = double.MaxValue;
				//var heightMax = double.MinValue;
				//for( int y = cellIndexMin.Y; y <= cellIndexMax.Y; y++ )
				//{
				//	for( int x = cellIndexMin.X; x <= cellIndexMax.X; x++ )
				//	{
				//		var height = owner.GetHeight( new Vector2I( x, y ) );
				//		if( height < heightMin )
				//			heightMin = height;
				//		if( height > heightMax )
				//			heightMax = height;
				//	}
				//}

				//bounds.Add( new Vector3( owner.GetPosition( cellIndexMin ).X, owner.GetPosition( cellIndexMin ).Y, heightMin ) );
				//bounds.Add( new Vector3( owner.GetPosition( cellIndexMax ).X, owner.GetPosition( cellIndexMax ).Y, heightMax ) );

				//}

				//	tile.UpdateGpuParameters();
				//	tile.UpdateUseStaticLightingData();

				////indices
				//Indices = owner.GenerateTileIndicesWithoutHoles( 0 );
				//List<int> indices;
				//if( considerHoles && tile.existsHoles )
				//	indices = owner.GenerateTileIndicesWithHoles( tile.cellIndexMin );
				//else
				//indices = owner.GenerateTileIndicesWithoutHoles( 0 );
			}

			static VertexElement[] vertexStructure;
			VertexElement[] GetVertexStructure()
			{
				if( vertexStructure == null )
				{
					var structure = new List<VertexElement>();
					structure.Add( new VertexElement( 0, 0, VertexElementType.Float3, VertexElementSemantic.Position ) );
					structure.Add( new VertexElement( 0, 12, VertexElementType.Float3, VertexElementSemantic.Normal ) );
					structure.Add( new VertexElement( 0, 24, VertexElementType.Float4, VertexElementSemantic.Tangent ) );
					structure.Add( new VertexElement( 0, 40, VertexElementType.Float2, VertexElementSemantic.TextureCoordinate0 ) );
					structure.Add( new VertexElement( 0, 48, VertexElementType.Float2, VertexElementSemantic.TextureCoordinate1 ) );
					structure.Add( new VertexElement( 0, 56, VertexElementType.Float2, VertexElementSemantic.TextureCoordinate2 ) );
					vertexStructure = structure.ToArray();
				}
				return vertexStructure;
			}

			public void UpdateRenderingData( bool meshExtractData, Vector3[,] calculatedPositionsForGetNormals, bool updateHoles, bool allowHoles )
			{
				if( !RenderingDataCreatedWithExtractData.HasValue || RenderingDataCreatedWithExtractData.Value != meshExtractData )
					DestroyRenderingData();

				////update ObjectsBounds
				//if( objectsBoundsNeedUpdateAfterRemove )
				//{
				//	objectsBoundsNeedUpdateAfterRemove = false;
				//	ObjectsBounds = null;
				//	foreach( var index in ObjectsMesh )
				//		AddToObjectsBounds( ref owner.ObjectsMesh[ index ] );
				//}

				CalculateGeometryAndBounds( calculatedPositionsForGetNormals, updateHoles, allowHoles, out var vertices, out var supportsLODs, out var lods );

				var isUpdate = ObjectInSpace != null;

				//create mesh in space, mesh
				if( ObjectInSpace == null )
				{
					//create internal group for objects in space
					var sectors = owner.GetComponent( "__Sectors" );
					if( sectors == null )
					{
						//create sectors components

						//need set ShowInEditor = false before AddComponent
						sectors = ComponentUtility.CreateComponent<Component>( null, false, false );
						sectors.NetworkMode = NetworkModeEnum.False;
						sectors.DisplayInEditor = false;
						owner.AddComponent( sectors, -1 );
						//sectors = owner.CreateComponent<Component>();

						sectors.Name = "__Sectors";
						sectors.SaveSupport = false;
						sectors.CloneSupport = false;
						sectors.Enabled = true;
					}

					//!!!!раскладывать во вложенную группу, чтобы меньше было в одном списке?

					ObjectInSpace = sectors.CreateComponent<MeshInSpace>( enabled: false );
					ObjectInSpace.AnyData = this;
					ObjectInSpace.Name = index.ToString();
					ObjectInSpace.SaveSupport = false;
					ObjectInSpace.CloneSupport = false;
					ObjectInSpace.CanBeSelected = false;
					ObjectInSpace.Transform = new Transform( Position );
					ObjectInSpace.SpaceBoundsUpdateEvent += ObjectInSpace_SpaceBoundsUpdateEvent;
					ObjectInSpace.GetRenderSceneData += ObjectInSpace_GetRenderSceneData;

					Mesh = ObjectInSpace.CreateComponent<Mesh>();
					Mesh.Name = "Mesh";
					//need for scene editor CalculateDropPosition
					Mesh.CalculateExtractedDataAndBounds = meshExtractData;
					ObjectInSpace.Mesh = ReferenceUtility.MakeThisReference( ObjectInSpace, Mesh );
				}
				else
				{
					ObjectInSpace.Enabled = false;
					Mesh.RemoveAllComponents( false );
					ObjectInSpace.Transform = new Transform( Position );
				}

				//create mesh geometry
				var meshGeometry = Mesh.CreateComponent<MeshGeometry>();
				meshGeometry.Name = "Mesh Geometry";
				//!!!!копии индексных массивов
				meshGeometry.VertexStructure = GetVertexStructure();
				meshGeometry.UnwrappedUV = UnwrappedUVEnum.TextureCoordinate2;
				meshGeometry.Vertices = vertices;
				meshGeometry.Indices = Indices;

				//initialize lods
				if( supportsLODs )
				{

					//!!!!LODRange
					//!!!!когда LODRange.Minimum > 0


					for( int n = 0; n < lods.Count; n++ )
					{
						var lodItem = lods[ n ];
						var index = n + 1;

						var lod = Mesh.CreateComponent<MeshLevelOfDetail>();
						lod.Name = "LOD " + index.ToString();
						lod.Distance = owner.LODDistance.Value * index;

						var lodMesh = lod.CreateComponent<Mesh>();
						lodMesh.Name = "Mesh";

						var lodMeshGeometry = lodMesh.CreateComponent<MeshGeometry>();
						lodMeshGeometry.Name = "Mesh Geometry";
						lodMeshGeometry.VertexStructure = GetVertexStructure();
						lodMeshGeometry.UnwrappedUV = UnwrappedUVEnum.TextureCoordinate2;
						lodMeshGeometry.Vertices = lodItem.Item1;
						lodMeshGeometry.Indices = lodItem.Item2;

						lod.Mesh = ReferenceUtility.MakeThisReference( lod, lodMesh );
					}
				}

				//update space bounds
				if( isUpdate )
					ObjectInSpace.SpaceBoundsUpdate();

				//enable mesh in space
				ObjectInSpace.Enabled = Indices.Length != 0;

				//update parameters
				if( !isUpdate )
				{
					ObjectInSpace.Visible = owner.Visible;

					//call from UpdateLayers
					//var surface = owner.Surface.Value;
					//if( surface != null && surface.Material.ReferenceOrValueSpecified )
					//	ObjectInSpace.ReplaceMaterial = surface.Material;
					//else
					//	ObjectInSpace.ReplaceMaterial = owner.Material;

					ObjectInSpace.Color = owner.MaterialColor;
					ObjectInSpace.CastShadows = owner.CastShadows;
					ObjectInSpace.ReceiveDecals = owner.ReceiveDecals;
					ObjectInSpace.MotionBlurFactor = owner.MotionBlurFactor;
				}

				//update rendering data status
				RenderingDataCreatedWithExtractData = meshExtractData;

				if( owner.Occluder )
				{
					var bounds = Bounds.BoundingBox;

					var minZ = owner.cachedBounds.Minimum.Z;
					//1.0 is offset
					var maxZ = bounds.Minimum.Z - 1.0;

					if( minZ > maxZ - 1.0 )
						minZ = maxZ - 1.0;

					var occluderBounds = new Bounds( bounds.Minimum.X, bounds.Minimum.Y, minZ, bounds.Maximum.X, bounds.Maximum.Y, maxZ );

					var tr = new Transform( occluderBounds.GetCenter(), Quaternion.Identity, occluderBounds.GetSize() );

					Occluder = ObjectInSpace.CreateComponent<Occluder>( enabled: false );
					Occluder.Transform = tr;
					Occluder.Enabled = true;
				}
				else
				{
					Occluder?.RemoveFromParent( false );
					Occluder = null;
				}

				ResetLightStaticShadowsCache( ObjectInSpace );
				ResetLightStaticShadowsCache( SurfaceObjectsObjectInSpace );
			}

			public void DestroyRenderingData()
			{
				Occluder?.RemoveFromParent( false );
				Occluder = null;

				if( ObjectInSpace != null )
				{
					ResetLightStaticShadowsCache( ObjectInSpace );

					ObjectInSpace.SpaceBoundsUpdateEvent -= ObjectInSpace_SpaceBoundsUpdateEvent;
					ObjectInSpace.GetRenderSceneData -= ObjectInSpace_GetRenderSceneData;
					ObjectInSpace.RemoveFromParent( false );
					ObjectInSpace = null;
					RenderingDataCreatedWithExtractData = null;
				}
			}

			public void Destroy()
			{
				DestroySurfaceObjectsData();
				DestroyCollisionBody();
				DestroyRenderingData();
			}

			private void ObjectInSpace_SpaceBoundsUpdateEvent( ObjectInSpace obj, ref SpaceBounds newBounds )
			{
				var tile = obj.AnyData as Tile;
				if( tile != null && tile.Bounds != null )
					newBounds = tile.Bounds;
			}

			private void ObjectInSpace_GetRenderSceneData( ObjectInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
			{
				if( owner.Visible )
				{
					var tile = sender.AnyData as Tile;
					if( tile != null )
						owner.TileGetRenderSceneData( tile, context );
				}
			}

			public void UpdateCollisionBody()
			{
				DestroyCollisionBody();

				if( owner.Collision && ObjectInSpace != null && Indices.Length != 0 )
				{
					var scene = owner.FindParent<Scene>();
					if( scene == null )
						return;

					var shapeData = new RigidBody();

					//if( existsHoles || true )
					//{

					var meshShape = shapeData.CreateComponent<CollisionShape_Mesh>();
					meshShape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.TriangleMesh;

					var vertices = new Vector3F[ Vertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						vertices[ n ] = ( Vertices[ n ] - Position ).ToVector3F();
					meshShape.Vertices = vertices;

					meshShape.Indices = Indices;
					meshShape.CheckValidData = false;
					meshShape.MergeEqualVerticesRemoveInvalidTriangles = false;

					//!!!!
					{
						shapeData.Material = owner.CollisionMaterial;
						//!!!!
						//CollisionBody.MaterialFrictionMode = owner.CollisionFrictionMode;
						shapeData.MaterialFriction = owner.CollisionFriction;
						//!!!!
						//CollisionBody.MaterialAnisotropicFriction = owner.CollisionAnisotropicFriction;
						//CollisionBody.MaterialSpinningFriction = owner.CollisionSpinningFriction;
						//CollisionBody.MaterialRollingFriction = owner.CollisionRollingFriction;
						shapeData.MaterialRestitution = owner.CollisionRestitution;
					}
					//SetCollisionMaterial();

					var shapeItem = scene.PhysicsWorld.AllocateShape( shapeData, Vector3.One );
					if( shapeItem != null )
					{
						var bodyItem = scene.PhysicsWorld.CreateRigidBodyStatic( shapeItem, true, this, Position, QuaternionF.Identity );
						if( bodyItem != null )
							CollisionBody = bodyItem;
					}

					//}
					//else
					//{
					//	var positions = Mesh.Result.ExtractedVerticesPositions;
					//	var heights = new float[ positions.Length ];
					//	for( int n = 0; n < positions.Length; n++ )
					//	{
					//		heights[ n ] = positions[ n ].Z;

					//		//!!!!
					//		heights[ n ] = 0;
					//	}

					//	int vertexCountByAxis = owner.tileSize + 1;

					//	//!!!!что с minHeight, maxHeight и position.Z

					//	var pos = owner.GetPosition( cellIndexMin );

					//	//!!!!
					//	CollisionBody.Transform = new Transform( new Vector3( pos.X, pos.Y, 0 ), Quaternion.Identity );
					//	//CollisionBody.Transform = new Transform( new Vector3( pos.X, pos.Y, 0 ), Quaternion.Identity, new Vector3( scl, scl, 1 ) );

					//	var shape = CollisionBody.CreateComponent<CollisionShape_Heightfield>();
					//	shape.VertexCount = new Vector2I( vertexCountByAxis, vertexCountByAxis );
					//	shape.Heights = heights;

					//	var scl = owner.cachedCellSize * owner.tileSize;
					//	shape.TransformRelativeToParent = new Transform( Vector3.Zero, Quaternion.Identity, new Vector3( scl, scl, 1 ) );
					//}


					//CollisionBody = ObjectInSpace.CreateComponent<RigidBody>( enabled: false );
					//CollisionBody.AnyData = this;
					//CollisionBody.Name = "Collision Body";
					//CollisionBody.SpaceBoundsUpdateEvent += CollisionBody_SpaceBoundsUpdateEvent;

					////if( existsHoles || true )
					////{

					//CollisionBody.Transform = ObjectInSpace.Transform;

					//var shape = CollisionBody.CreateComponent<CollisionShape_Mesh>();
					//shape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.TriangleMesh;

					//var vertices = new Vector3F[ Vertices.Length ];
					//for( int n = 0; n < vertices.Length; n++ )
					//	vertices[ n ] = ( Vertices[ n ] - Position ).ToVector3F();
					//shape.Vertices = vertices;

					//shape.Indices = Indices;
					//shape.CheckValidData = false;
					//shape.MergeEqualVerticesRemoveInvalidTriangles = false;

					////}
					////else
					////{
					////	var positions = Mesh.Result.ExtractedVerticesPositions;
					////	var heights = new float[ positions.Length ];
					////	for( int n = 0; n < positions.Length; n++ )
					////	{
					////		heights[ n ] = positions[ n ].Z;

					////		//!!!!
					////		heights[ n ] = 0;
					////	}

					////	int vertexCountByAxis = owner.tileSize + 1;

					////	//!!!!что с minHeight, maxHeight и position.Z

					////	var pos = owner.GetPosition( cellIndexMin );

					////	//!!!!
					////	CollisionBody.Transform = new Transform( new Vector3( pos.X, pos.Y, 0 ), Quaternion.Identity );
					////	//CollisionBody.Transform = new Transform( new Vector3( pos.X, pos.Y, 0 ), Quaternion.Identity, new Vector3( scl, scl, 1 ) );

					////	var shape = CollisionBody.CreateComponent<CollisionShape_Heightfield>();
					////	shape.VertexCount = new Vector2I( vertexCountByAxis, vertexCountByAxis );
					////	shape.Heights = heights;

					////	var scl = owner.cachedCellSize * owner.tileSize;
					////	shape.TransformRelativeToParent = new Transform( Vector3.Zero, Quaternion.Identity, new Vector3( scl, scl, 1 ) );
					////}

					//SetCollisionMaterial();

					//CollisionBody.Enabled = true;
				}
			}

			public void DestroyCollisionBody()
			{
				if( CollisionBody != null )
				{
					CollisionBody.Dispose();
					CollisionBody = null;
				}

				//if( CollisionBody != null )
				//{
				//	CollisionBody.SpaceBoundsUpdateEvent -= CollisionBody_SpaceBoundsUpdateEvent;
				//	CollisionBody.Dispose();
				//	CollisionBody = null;
				//}
			}

			//private void CollisionBody_SpaceBoundsUpdateEvent( ObjectInSpace obj, ref SpaceBounds newBounds )
			//{
			//	var sector = obj.AnyData as Tile;
			//	if( sector != null && sector.Bounds != null )
			//		newBounds = sector.Bounds;
			//}

			public void SetCollisionMaterial()
			{
				//!!!!impl

				//if( CollisionBody != null )
				//{
				//	CollisionBody.Material = owner.CollisionMaterial;
				//	//!!!!
				//	//CollisionBody.MaterialFrictionMode = owner.CollisionFrictionMode;
				//	CollisionBody.MaterialFriction = owner.CollisionFriction;
				//	//!!!!
				//	//CollisionBody.MaterialAnisotropicFriction = owner.CollisionAnisotropicFriction;
				//	//CollisionBody.MaterialSpinningFriction = owner.CollisionSpinningFriction;
				//	//CollisionBody.MaterialRollingFriction = owner.CollisionRollingFriction;
				//	CollisionBody.MaterialRestitution = owner.CollisionRestitution;
				//}
			}

			public void UpdateLayers()
			{
				if( ObjectInSpace != null )
				{

					var surface = owner.Surface.Value;
					if( surface != null && surface.Material.ReferenceOrValueSpecified )
						ObjectInSpace.ReplaceMaterial = surface.Material;
					else
						ObjectInSpace.ReplaceMaterial = owner.Material;


					var currentLayers = owner.currentLayersCache;//owner.GetCurrentLayers( false );

					if( currentLayers.Length != 0 )
					{
						var newList = new List<RenderingPipeline.RenderSceneData.LayerItem>();

						var bounds = Bounds.BoundingBox;

						foreach( var layer in currentLayers )
						{
							var skip = false;

							byte[] layerMask = null;
							{
								var data = layer.Mask?.Result?.GetData();
								if( data != null && data.Length != 0 )
									layerMask = data[ 0 ].Data.Array;
							}

							var maskSize = owner.GetPaintMaskSizeInteger();

							if( layerMask != null && layerMask.Length == maskSize * maskSize )
							{
								var min = owner.GetMaskIndexByPosition( bounds.Minimum.ToVector2() ) - new Vector2I( 1, 1 );
								var max = owner.GetMaskIndexByPosition( bounds.Maximum.ToVector2() ) + new Vector2I( 2, 2 );
								MathEx.Clamp( ref min.X, 0, maskSize - 1 );
								MathEx.Clamp( ref min.Y, 0, maskSize - 1 );
								MathEx.Clamp( ref max.X, 0, maskSize - 1 );
								MathEx.Clamp( ref max.Y, 0, maskSize - 1 );

								var allZero = true;

								if( maskSize > 0 )
								{
									for( int y = min.Y; y <= max.Y; y++ )
									{
										for( int x = min.X; x <= max.X; x++ )
										{
											//!!!!слой может быть полностью перекрыт, тогда не нужен

											if( layerMask[ ( maskSize - 1 - y ) * maskSize + x ] != 0 )
											{
												allZero = false;
												goto toSkip;
											}
										}
									}
								}
toSkip:

								if( allZero )
									skip = true;
							}

							if( !skip )
								newList.Add( layer );
						}

						if( newList.Count != 0 )
							ObjectInSpace.PaintLayersReplace = newList.Count != currentLayers.Length ? newList.ToArray() : currentLayers;
						else
							ObjectInSpace.PaintLayersReplace = null;
					}
					else
						ObjectInSpace.PaintLayersReplace = null;
				}
			}

			public void SetLODDistances()
			{
				if( Mesh != null )
				{
					var lods = Mesh.GetComponents<MeshLevelOfDetail>();
					for( int n = 0; n < lods.Length; n++ )
					{
						var lod = lods[ n ];
						var index = n + 1;
						lod.Distance = owner.LODDistance.Value * index;
					}

					//!!!!?
					//update
					if( Mesh.Result != null )
						Mesh.PerformResultCompile();
				}
			}

			struct SurfaceObjectsObjectItem
			{
				//!!!!
				public int Initialized;

				public int VariationGroup;
				public int VariationElement;
				public Vector3 Position;
				public QuaternionF Rotation;
				public Vector3F Scale;
				//public ColorValue Color;
			}

			static SurfaceObjectsRenderGroup GetSurfaceObjectsGroup( List<SurfaceObjectsRenderGroup> renderGroups, bool noBatchingGroup, int variationGroup, int variationElement )
			{
				for( int nGroup = 0; nGroup < renderGroups.Count; nGroup++ )
				{
					var group = renderGroups[ nGroup ];

					if( noBatchingGroup == group.NoBatchingGroup )
					{
						if( noBatchingGroup )
							return group;
						else if( group.VariationGroup == variationGroup && group.VariationElement == variationElement )
							return group;
					}
				}
				return null;
			}

			static SurfaceObjectsObjectItem[] CalculateSurfaceObjects( SurfaceObjectsItem surfaceObjectsItem, Vector3[,] calculatedPositionsForGetNormals )
			{
				var surface = surfaceObjectsItem.GetSurface().Result;
				if( surface == null )
					return null;

				surfaceObjectsItem.GetObjectsColor( out var objectsColor );
				var objectsDistribution = surfaceObjectsItem.GetObjectsDistribution();
				var objectsScale = surfaceObjectsItem.GetObjectsScale();

				var tile = surfaceObjectsItem.Owner;
				var owner = tile.owner;

				SurfaceObjectsObjectItem[] data = null;
				//OpenList<SurfaceObjectsObjectItem> data = null;
				//var data = new OpenList<SurfaceObjectsObjectItem>( 1024 );// count );



				var fillPattern = surface.FillPattern;
				if( fillPattern != null )
				{
					var fillPatternSize = fillPattern.Size * objectsDistribution;
					if( fillPatternSize.X == 0.0 )
						fillPatternSize.X = 0.01;
					if( fillPatternSize.Y == 0.0 )
						fillPatternSize.Y = 0.01;

					var groups = surface.Groups;

					//if( groupOfObjects == null )
					//	groupOfObjects = owner.GetOrCreateGroupOfObjects( true );
					//var element = GetOrCreateElement( groupOfObjects, surface );

					//!!!!может есть общий множитель для всех слоев
					//var color = surfaceObjectsItem.Layer != null ? surfaceObjectsItem.Layer.Value.SurfaceObjectsColor : owner.SurfaceObjectsColor.Value;
					//var color = owner.SurfaceObjectsColor.Value;
					//if( layer != null )
					//	color *= layer.Value.SurfaceObjectsColor;


					var bounds = tile.Bounds.BoundingBox;
					var bounds2 = bounds.ToRectangle();

					var min = bounds.Minimum.ToVector2();
					for( int n = 0; n < 2; n++ )
					{
						min[ n ] /= fillPatternSize[ n ];
						min[ n ] = Math.Floor( min[ n ] );
						min[ n ] *= fillPatternSize[ n ];
					}

					var max = bounds.Maximum.ToVector2();
					for( int n = 0; n < 2; n++ )
					{
						max[ n ] /= fillPatternSize[ n ];
						max[ n ] = Math.Ceiling( max[ n ] );
						max[ n ] *= fillPatternSize[ n ];
					}


					int count = 0;
					for( var y = min.Y; y < max.Y - fillPatternSize.Y * 0.5; y += fillPatternSize.Y )
						for( var x = min.X; x < max.X - fillPatternSize.X * 0.5; x += fillPatternSize.X )
							count += fillPattern.Objects.Count;

					data = new SurfaceObjectsObjectItem[ count ];
					//data = new OpenList<SurfaceObjectsObjectItem>( count );

					var objectsToCreate = new (Vector2 positionXY, int groupIndex)[ count ];

					int counter = 0;
					for( var y = min.Y; y < max.Y - fillPatternSize.Y * 0.5; y += fillPatternSize.Y )
					{
						for( var x = min.X; x < max.X - fillPatternSize.X * 0.5; x += fillPatternSize.X )
						{
							for( int nObjectItem = 0; nObjectItem < fillPattern.Objects.Count; nObjectItem++ )
							{
								var objectItem = fillPattern.Objects[ nObjectItem ];

								var positionXY = new Vector2( x, y ) + objectItem.Position * objectsDistribution;
								var groupIndex = objectItem.Group;

								var regularAlignment = groups[ groupIndex ].RegularAlignment;
								if( regularAlignment != 0 )
								{
									positionXY /= regularAlignment;
									positionXY = new Vector2( (int)positionXY.X, (int)positionXY.Y );
									positionXY *= regularAlignment;
								}

								objectsToCreate[ counter++ ] = (positionXY, groupIndex);
							}
						}
					}

					//!!!!
					//if( data == null && counter != 0 )
					//	data = new OpenList<GroupOfObjects.Object>( 2048 );

					//var touchGroups = surface.Groups;

					Parallel.For( 0, counter, delegate ( int nObjectItem )
					{
						ref var item = ref objectsToCreate[ nObjectItem ];
						ref var positionXY = ref item.positionXY;
						var groupIndex = (byte)item.groupIndex;

						if( bounds2.Contains( ref positionXY ) )
						{
							var random = new FastRandom( unchecked(nObjectItem * 77 + (int)( positionXY.X * 12422.7 + positionXY.Y * 1234.2 )), true );
							//works bad for Forest template scene
							//var randomSeed = new FastRandom( unchecked(nObjectItem * 12 + (int)( positionXY.X * 11.7 + positionXY.Y * 13.2 )) );
							//var random = new FastRandom( randomSeed.NextInteger() );

							owner.GetNormal( ref positionXY, calculatedPositionsForGetNormals, out var surfaceNormal );

							var options = new Surface.GetRandomVariationOptions( groupIndex, surfaceNormal );

							surface.GetRandomVariation( options, random, out _, out var elementIndex, out var positionZ, out var rotation, out var scale );

							try
							{
								var terrainHeight = owner.GetHeight( positionXY, tile.ContainsHoles );
								if( terrainHeight != double.MinValue )
								{
									//check by mask
									var maskValue = owner.GetLayerInfoByPosition( positionXY, surfaceObjectsItem.BaseSurfaceObjectsData != null ? null : surfaceObjectsItem.Layer.Layer );
									if( maskValue != 0 && ( maskValue == 255 || random.Next( 255 ) <= maskValue ) )
									{
										ref var obj = ref data[ nObjectItem ];
										//var obj = new SurfaceObjectsObjectItem();

										obj.Initialized = 1;

										obj.VariationGroup = groupIndex;
										obj.VariationElement = elementIndex;

										obj.Position = new Vector3( positionXY, terrainHeight + positionZ );
										obj.Rotation = rotation;

										obj.Scale = scale * objectsScale;
										//obj.Color = objectsColor;

										//lock( data )
										//	data.Add( ref obj );
									}
								}
							}
							catch { }
						}
					} );
				}

				return data;
			}

			//!!!!impl threading task
			//called from thread in threading mode. minimize using data of the terrain and other components
			public static SurfaceObjectsRenderGroup[] CalculateSurfaceObjectsRenderGroups( SurfaceObjectsItem surfaceObjectsItem )
			{
				var renderGroups = new List<SurfaceObjectsRenderGroup>();

				//!!!!calculatedPositionsForGetNormals
				var data = CalculateSurfaceObjects( surfaceObjectsItem, null );
				var surface = surfaceObjectsItem.GetSurface().Result;

				if( data != null && surface != null )
				{
					var tile = surfaceObjectsItem.Owner;
					var owner = tile.owner;

					for( int nObject = 0; nObject < data.Length; nObject++ )
					{
						ref var obj = ref data[ nObject ];
						if( obj.Initialized == 0 )
							continue;
						//ref var obj = ref data.Data[ nObject ];

						surface.GetMesh( /*obj.Element, */obj.VariationGroup, obj.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out _, out _, out _, out _ );

						if( !enabled )
							continue;

						bool allowBatching = true;
						if( allowBatching && replaceMaterial?.Result != null && replaceMaterial.Result.Transparent )
							allowBatching = false;
						if( allowBatching && !mesh.SupportsBatching() )
							allowBatching = false;

						var group = GetSurfaceObjectsGroup( renderGroups, !allowBatching, obj.VariationGroup, obj.VariationElement );
						if( group == null )
						{
							group = new SurfaceObjectsRenderGroup();
							group.NoBatchingGroup = !allowBatching;
							group.VariationGroup = obj.VariationGroup;
							group.VariationElement = obj.VariationElement;
							renderGroups.Add( group );
						}

						var objectItem = new SurfaceObjectsRenderGroup.ObjectItem();

						objectItem.Position = obj.Position;
						objectItem.Rotation = obj.Rotation;
						objectItem.Scale = obj.Scale;

						obj.Rotation.ToMatrix3( out var rot );
						Matrix3F.FromScale( ref obj.Scale, out var scl );
						Matrix3F.Multiply( ref rot, ref scl, out objectItem.MeshMatrix3 );
						//objectItem.MeshPosition = obj.Position;
						//Matrix3F.Multiply( ref rot, ref scl, out objectItem.MeshMatrix3 );

						//var pos = obj.Position.ToVector3F();
						//obj.Rotation.ToMatrix3( out var rot );
						//Matrix3F.FromScale( ref obj.Scale, out var scl );
						//Matrix3F.Multiply( ref rot, ref scl, out var mat3 );
						//objectItem.MeshMatrix = new Matrix4F( ref mat3, ref pos );
						////Matrix3F.Multiply( ref rot, ref scl, out objectItem.MeshMatrix3 );


						//!!!!slowly


						var transform = new Transform( obj.Position, obj.Rotation, obj.Scale );
						if( mesh.Result != null )
						{
							//!!!!можно считать оптимальнее?
							var b = SpaceBounds.Multiply( transform, mesh.Result.SpaceBounds );
							objectItem.BoundingBox = b.BoundingBox;
							objectItem.BoundingSphere = b.boundingSphere;
						}
						else
						{
							objectItem.BoundingBox = new Bounds( objectItem.Position );
							objectItem.BoundingBox.Expand( 0.1 );
							objectItem.BoundingSphere = new Sphere( objectItem.Position, 0.1 );
						}

						//objectItem.ColorForInstancingData = RenderingPipeline.ColorOneForInstancingData;
						//objectItem.ColorForInstancingData = RenderingPipeline.GetColorForInstancingData( ref obj.Color );

						group.Objects.Add( ref objectItem );
					}

					////split groups by max objects count
					//if( owner.MaxObjectsInGroup > 1 )//!!!!
					//{
					//	bool needProcess = true;
					//	do
					//	{
					//		needProcess = false;

					//		var currentGroups = renderGroups;
					//		renderGroups = new List<SurfaceObjectsRenderGroup>( currentGroups.Capacity );

					//		foreach( var group in currentGroups )
					//		{
					//			if( group.Objects.Count > owner.MaxObjectsInGroup )
					//			{
					//				unsafe
					//				{
					//					//!!!!если равномерно то можно раскладывать на две стороны без сортировки

					//					//!!!!группы объектов сделать тоже как тут

					//					var objectsArray = group.Objects.Data;
					//					var objectCount = group.Objects.Count;

					//					//var objects = group.Objects.ToArray();

					//					//!!!!GC
					//					var indexes = new int[ objectCount ];
					//					for( int n = 0; n < objectCount; n++ )
					//						indexes[ n ] = n;

					//					CollectionUtility.MergeSortUnmanaged( indexes, delegate ( int index1, int index2 )
					//					{
					//						var pos1 = objectsArray[ index1 ].Position[ group.NextSplitAxis ];
					//						var pos2 = objectsArray[ index2 ].Position[ group.NextSplitAxis ];

					//						if( pos1 < pos2 )
					//							return -1;
					//						if( pos1 > pos2 )
					//							return 1;
					//						return 0;
					//					}, true );

					//					int middle = objectCount / 2;
					//					//int middle = objects.Length / 2;

					//					for( int nGroup = 0; nGroup < 2; nGroup++ )
					//					{
					//						var newGroup = new SurfaceObjectsRenderGroup();
					//						newGroup.NoBatchingGroup = group.NoBatchingGroup;
					//						newGroup.VariationGroup = group.VariationGroup;
					//						newGroup.VariationElement = group.VariationElement;

					//						if( nGroup == 0 )
					//						{
					//							for( int n = 0; n < middle; n++ )
					//								newGroup.Objects.Add( ref objectsArray[ indexes[ n ] ] );
					//						}
					//						else
					//						{
					//							for( int n = middle; n < objectCount; n++ )
					//								newGroup.Objects.Add( ref objectsArray[ indexes[ n ] ] );
					//						}

					//						newGroup.NextSplitAxis = group.NextSplitAxis + 1;
					//						//quadtree
					//						if( newGroup.NextSplitAxis > 1 )
					//							newGroup.NextSplitAxis = 0;

					//						renderGroups.Add( newGroup );
					//					}


					//					//var objects = group.Objects.ToArray();

					//					//CollectionUtility.MergeSort( objects, delegate ( SurfaceObjectsRenderGroup.ObjectItem item1, SurfaceObjectsRenderGroup.ObjectItem item2 )
					//					//{
					//					//	var pos1 = item1.Position[ group.NextSplitAxis ];
					//					//	var pos2 = item2.Position[ group.NextSplitAxis ];

					//					//	if( pos1 < pos2 )
					//					//		return -1;
					//					//	if( pos1 > pos2 )
					//					//		return 1;
					//					//	return 0;

					//					//}, true );

					//					//int middle = objects.Length / 2;

					//					//for( int nGroup = 0; nGroup < 2; nGroup++ )
					//					//{
					//					//	var newGroup = new SurfaceObjectsRenderGroup();
					//					//	newGroup.NoBatchingGroup = group.NoBatchingGroup;
					//					//	newGroup.VariationGroup = group.VariationGroup;
					//					//	newGroup.VariationElement = group.VariationElement;

					//					//	if( nGroup == 0 )
					//					//	{
					//					//		for( int n = 0; n < middle; n++ )
					//					//			newGroup.Objects.Add( objects[ n ] );
					//					//	}
					//					//	else
					//					//	{
					//					//		for( int n = middle; n < objects.Length; n++ )
					//					//			newGroup.Objects.Add( objects[ n ] );
					//					//	}

					//					//	newGroup.NextSplitAxis = group.NextSplitAxis + 1;
					//					//	//quadtree
					//					//	if( newGroup.NextSplitAxis > 1 )
					//					//		newGroup.NextSplitAxis = 0;

					//					//	renderGroups.Add( newGroup );
					//					//}
					//				}

					//				needProcess = true;
					//			}
					//			else
					//			{
					//				renderGroups.Add( group );
					//			}
					//		}

					//	} while( needProcess );
					//}

					//calculate Bounds, VisibilityDistance
					foreach( var group in renderGroups )
					{
						var bounds = NeoAxis.Bounds.Cleared;
						//var maxLocalObjectsBounds = Bounds.Cleared;
						//float maxVisibilityDistance = float.MinValue;

						for( int nObject = 0; nObject < group.Objects.Count; nObject++ )
						{
							ref var objectItem = ref group.Objects.Data[ nObject ];

							//!!!!не только меши
							surface.GetMesh( group.VariationGroup, group.VariationElement, out var enabled, out var mesh, out _, out var visibilityDistanceFactor, out _, out _, out _ );

							if( !enabled )
								continue;

							bounds.Add( ref objectItem.BoundingBox );

							group.ObjectsMaxScale = Math.Max( group.ObjectsMaxScale, objectItem.Scale.MaxComponent() );
							//maxVisibilityDistance = Math.Max( maxVisibilityDistance, visibilityDistance );
						}

						group.BoundingBox = bounds;
						group.BoundingBox.GetBoundingSphere( out group.BoundingSphere );
						//group.VisibilityDistance = maxVisibilityDistance;
						//group.VisibilityDistanceSquared = group.VisibilityDistance * group.VisibilityDistance;
					}

					////sort by VisibilityDistance from far to nearest
					//CollectionUtility.MergeSort( renderGroups, delegate ( SurfaceObjectsRenderGroup group1, SurfaceObjectsRenderGroup group2 )
					//{
					//	if( group1.VisibilityDistance > group2.VisibilityDistance )
					//		return -1;
					//	if( group1.VisibilityDistance < group2.VisibilityDistance )
					//		return 1;
					//	return 0;
					//}, true );
				}

				return renderGroups.ToArray();
			}

			public void DestroySurfaceObjectsRenderGroups( SurfaceObjectsItem item )
			{
				if( item.RenderGroups != null )
				{
					foreach( var group in item.RenderGroups )
					{
						group.BatchingInstanceBufferMesh?.Dispose();
						group.BatchingInstanceBufferBillboard?.Dispose();
					}
					item.RenderGroups = null;
				}
			}

			public void UpdateSurfaceObjectsData( Vector3[,] calculatedPositionsForGetNormals )
			{
				DestroySurfaceObjectsData();

				if( ObjectInSpace == null || !owner.Visible )
					return;
				var scene = owner.FindParent<Scene>();
				if( scene == null )
					return;


				//update SurfaceObjectsItems and calculate total bounds

				var allItemsBounds = Bounds.BoundingBox;

				{
					int layerCount = 1;
					if( ObjectInSpace.PaintLayersReplace != null )
						layerCount += ObjectInSpace.PaintLayersReplace.Length;

					var surfaceObjectsItems = new List<SurfaceObjectsItem>( layerCount );

					for( int nLayer = 0; nLayer < layerCount; nLayer++ )
					{
						BaseSurfaceObjectsDataClass baseSurfaceObjectsData = null;
						RenderingPipeline.RenderSceneData.LayerItem? layer = null;
						Surface surface = null;

						if( nLayer == 0 )
						{
							var cache = owner.GetBaseSurfaceObjectsDataCache();
							if( cache?.Surface != null && cache.SurfaceObjects )
							{
								baseSurfaceObjectsData = cache;
								surface = cache.Surface;
							}
						}
						else
						{
							ref var layer2 = ref ObjectInSpace.PaintLayersReplace[ nLayer - 1 ];
							if( layer2.Surface != null && layer2.SurfaceObjects )
							{
								layer = layer2;
								surface = layer2.Surface;
							}
						}

						if( surface?.Result != null )
						{
							var item = new SurfaceObjectsItem();
							item.Owner = this;
							item.BaseSurfaceObjectsData = baseSurfaceObjectsData;
							if( layer != null )
								item.Layer = layer.Value;

							////var surface = item.GetSurface().Result;
							//if( surface.Result != null )
							//{
							//	item.MaxVisibilityDistanceSquared = surface.Result.GetMaxVisibilityDistance();
							//	item.MaxVisibilityDistanceSquared *= item.MaxVisibilityDistanceSquared;
							//}

							//calculate item.ObjectsMaxSize
							{
								var maxRadius = 0.0;
								foreach( var mesh in surface.Result.GetAllMeshes() )
								{
									if( mesh.Result != null )
										maxRadius = Math.Max( maxRadius, mesh.Result.SpaceBounds.boundingSphere.Radius );
								}
								item.ObjectsMaxSize = (float)( maxRadius * 2.0 * surface.Result.GetMaxScale() * item.GetObjectsScale() );
							}

							//calculate item.ObjectsMaxVisibilityDistanceFactor
							item.ObjectsMaxVisibilityDistanceFactor = surface.Result.GetMaxVisibilityDistanceFactor() * item.GetObjectsVisibilityDistanceFactor();

							//calculate item.Bounds
							item.Bounds = Bounds.BoundingBox;
							//expand by maximal objects scale
							item.Bounds.Expand( item.ObjectsMaxSize * 0.5 );

							allItemsBounds.Add( item.Bounds );
							//item.Bounds = itemBounds;

							surfaceObjectsItems.Add( item );
						}
					}

					if( surfaceObjectsItems.Count != 0 )
						SurfaceObjectsItems = surfaceObjectsItems.ToArray();
				}

				SurfaceObjectsBounds = new SpaceBounds( allItemsBounds );
				////SurfaceObjectsBounds
				//{
				//	var bounds = Bounds.CalculatedBoundingBox;
				//	if( SurfaceObjectsItems != null )
				//	{
				//		foreach( var item in SurfaceObjectsItems )
				//			bounds.Add( ref item.Bounds );
				//	}
				//	SurfaceObjectsBounds = new SpaceBounds( bounds );
				//}

				//create object in space
				SurfaceObjectsObjectInSpace = ObjectInSpace.CreateComponent<ObjectInSpace>( enabled: false );
				SurfaceObjectsObjectInSpace.AnyData = this;
				SurfaceObjectsObjectInSpace.SaveSupport = false;
				SurfaceObjectsObjectInSpace.CloneSupport = false;
				SurfaceObjectsObjectInSpace.CanBeSelected = false;
				SurfaceObjectsObjectInSpace.Transform = new Transform( Position );
				SurfaceObjectsObjectInSpace.SpaceBoundsUpdateEvent += SurfaceObjectsObjectInSpace_SpaceBoundsUpdateEvent;
				SurfaceObjectsObjectInSpace.GetRenderSceneData += SurfaceObjectsObjectInSpace_GetRenderSceneData;
				SurfaceObjectsObjectInSpace.Enabled = true;


				//collision
				if( SurfaceObjectsItems != null )
				{
					var collisionDefinitionByMesh = new Dictionary<Mesh, RigidBody>();

					foreach( var surfaceObjectsItem in SurfaceObjectsItems )
					{
						if( surfaceObjectsItem.GetObjectsCollision() )
						{
							var data = Tile.CalculateSurfaceObjects( surfaceObjectsItem, calculatedPositionsForGetNormals );
							if( data != null )
							{
								var surface = surfaceObjectsItem.GetSurface().Result;
								if( surface != null )
								{
									var tile = surfaceObjectsItem.Owner;
									var owner = tile.owner;

									for( int nObject = 0; nObject < data.Length; nObject++ )
									{
										ref var obj = ref data[ nObject ];
										if( obj.Initialized == 0 )
											continue;
										//ref var obj = ref data.Data[ nObject ];

										surface.GetMesh( /*obj.Element, */obj.VariationGroup, obj.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out _, out _, out _, out _ );

										if( enabled && mesh != null )//&& objCollision )
										{
											if( !collisionDefinitionByMesh.TryGetValue( mesh, out var collisionDefinition ) )
											{
												collisionDefinition = mesh.GetComponent<RigidBody>( "Collision Definition" );
												if( collisionDefinition == null )
													collisionDefinition = mesh?.Result.GetAutoCollisionDefinition();
												collisionDefinitionByMesh[ mesh ] = collisionDefinition;
											}

											if( collisionDefinition != null )
											{
												var shapeItem = scene.PhysicsWorld.AllocateShape( collisionDefinition, obj.Scale );
												if( shapeItem != null )
												{
													var rigidBodyItem = scene.PhysicsWorld.CreateRigidBodyStatic( shapeItem, true, this, ref obj.Position, ref obj.Rotation );
													if( rigidBodyItem != null )
													{
														SurfaceObjectsCollisionBodies.Add( rigidBodyItem );
													}
												}
											}
										}
									}
								}
							}
						}
					}


					//var bodiesToCreate = new List<(RigidBody, Transform)>();

					//foreach( var surfaceObjectsItem in SurfaceObjectsItems )
					//{
					//	if( surfaceObjectsItem.GetObjectsCollision() )
					//	{
					//		var data = Tile.CalculateSurfaceObjects( surfaceObjectsItem, calculatedPositionsForGetNormals );
					//		if( data != null )
					//		{
					//			var surface = surfaceObjectsItem.GetSurface().Result;
					//			if( surface != null )
					//			{
					//				var tile = surfaceObjectsItem.Owner;
					//				var owner = tile.owner;

					//				for( int nObject = 0; nObject < data.Length; nObject++ )
					//				{
					//					ref var obj = ref data[ nObject ];
					//					if( obj.Initialized == 0 )
					//						continue;
					//					//ref var obj = ref data.Data[ nObject ];

					//					surface.GetMesh( /*obj.Element, */obj.VariationGroup, obj.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out _, out _, out _, out _ );

					//					if( enabled && mesh != null )//&& objCollision )
					//					{
					//						var collisionBody = mesh.GetComponent<RigidBody>( "Collision Definition" );
					//						if( collisionBody != null )
					//						{
					//							var objTransformLocal = new Transform( obj.Position - Position, obj.Rotation, obj.Scale );
					//							bodiesToCreate.Add( (collisionBody, objTransformLocal) );
					//						}
					//					}
					//				}
					//			}
					//		}
					//	}
					//}

					//if( bodiesToCreate.Count != 0 )
					//{
					//	var body = SurfaceObjectsObjectInSpace.CreateComponent<RigidBody>( enabled: false );
					//	SurfaceObjectsCollisionBody = body;
					//	body.AnyData = this;
					//	body.Name = "Collision Body";
					//	body.Transform = SurfaceObjectsObjectInSpace.Transform;

					//	foreach( var item in bodiesToCreate )
					//	{
					//		var sourceBody = item.Item1;
					//		var objTransformLocal = item.Item2;

					//		//!!!!несколько тел создавать если разные свойства
					//		body.Material = sourceBody.Material.Value;
					//		//!!!!
					//		//body.MaterialFrictionMode = sourceBody.MaterialFrictionMode.Value;
					//		body.MaterialFriction = sourceBody.MaterialFriction.Value;
					//		//!!!!
					//		//body.MaterialAnisotropicFriction = sourceBody.MaterialAnisotropicFriction.Value;
					//		//body.MaterialSpinningFriction = sourceBody.MaterialSpinningFriction.Value;
					//		//body.MaterialRollingFriction = sourceBody.MaterialRollingFriction.Value;
					//		body.MaterialRestitution = sourceBody.MaterialRestitution.Value;
					//		body.CollisionGroup = sourceBody.CollisionGroup.Value;
					//		body.CollisionMask = sourceBody.CollisionMask.Value;

					//		foreach( var sourceShape in sourceBody.GetComponents<CollisionShape>() )
					//		{
					//			var shape = (CollisionShape)sourceShape.Clone();
					//			shape.TransformRelativeToParent = objTransformLocal * sourceShape.TransformRelativeToParent.Value;
					//			body.AddComponent( shape );
					//		}
					//	}

					//	body.SpaceBoundsUpdateEvent += SurfaceObjectsCollisionBody_SpaceBoundsUpdateEvent;
					//	body.Enabled = true;
					//}
				}


				//!!!!
				//!!!!check by distance

				////no threading mode
				//if( EngineApp.RenderVideoToFileData != null )
				//{
				//	foreach( var surfaceObjectsItem in SurfaceObjectsItems )
				//		CalculateSurfaceObjectsRenderGroups( surfaceObjectsItem );
				//}
			}

			void DestroySurfaceObjectsData()
			{
				if( SurfaceObjectsCollisionBodies.Count != 0 )
				{
					foreach( var body in SurfaceObjectsCollisionBodies )
					{
						//!!!!
						//body.SpaceBoundsUpdateEvent -= CollisionBody_SpaceBoundsUpdateEvent;

						body.PhysicsWorld.DestroyRigidBody( body );
					}
					SurfaceObjectsCollisionBodies.Clear();
				}

				//if( SurfaceObjectsCollisionBody != null )
				//{
				//	SurfaceObjectsCollisionBody.SpaceBoundsUpdateEvent -= SurfaceObjectsCollisionBody_SpaceBoundsUpdateEvent;
				//	SurfaceObjectsCollisionBody.RemoveFromParent( false );
				//	SurfaceObjectsCollisionBody = null;
				//}

				if( SurfaceObjectsObjectInSpace != null )
				{
					ResetLightStaticShadowsCache( SurfaceObjectsObjectInSpace );

					SurfaceObjectsObjectInSpace.SpaceBoundsUpdateEvent -= SurfaceObjectsObjectInSpace_SpaceBoundsUpdateEvent;
					SurfaceObjectsObjectInSpace.GetRenderSceneData -= SurfaceObjectsObjectInSpace_GetRenderSceneData;
					SurfaceObjectsObjectInSpace.RemoveFromParent( false );
					SurfaceObjectsObjectInSpace = null;
				}

				SurfaceObjectsBounds = null;

				if( SurfaceObjectsItems != null )
				{
					foreach( var item in SurfaceObjectsItems )
						DestroySurfaceObjectsRenderGroups( item );
					SurfaceObjectsItems = null;
				}
			}

			private void SurfaceObjectsObjectInSpace_SpaceBoundsUpdateEvent( ObjectInSpace obj, ref SpaceBounds newBounds )
			{
				var tile = obj.AnyData as Tile;
				if( tile != null && tile.SurfaceObjectsBounds != null )
					newBounds = tile.SurfaceObjectsBounds;
			}

			private void SurfaceObjectsCollisionBody_SpaceBoundsUpdateEvent( ObjectInSpace obj, ref SpaceBounds newBounds )
			{
				SurfaceObjectsObjectInSpace_SpaceBoundsUpdateEvent( obj, ref newBounds );
			}

			private void SurfaceObjectsObjectInSpace_GetRenderSceneData( ObjectInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
			{
				if( owner.Visible )
				{
					var tile = sender.AnyData as Tile;
					if( tile != null )
						owner.TileGetRenderSceneDataSurfaceObjects( tile, context, mode, modeGetObjectsItem );
				}
			}

			void ResetLightStaticShadowsCache( ObjectInSpace objectInSpace )
			{
				if( objectInSpace != null )
				{
					var scene = owner.ParentRoot as Scene;
					if( scene != null )
					{
						var bounds = objectInSpace.SpaceBounds.BoundingBox;
						var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, bounds );
						scene.GetObjectsInSpace( item );

						foreach( var resultItem in item.Result )
						{
							var light = resultItem.Object as Light;
							light?.ResetStaticShadowsCache();
						}
					}
				}
			}
		}

		/////////////////////////////////////////

		public struct LayersInfoByPositionItem
		{
			public PaintLayer Layer;
			public byte MaskValue;

			public LayersInfoByPositionItem( PaintLayer layer, byte maskValue )
			{
				Layer = layer;
				MaskValue = maskValue;
			}
		}

		/////////////////////////////////////////

		public Terrain()
		{
			_holes = new ReferenceList<MeshInSpace>( this, () => HolesChanged?.Invoke( this ) );

			heightmapBuffer = new float[ heightmapSize + 1, heightmapSize + 1 ];
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( CollisionMaterial ):
					if( !Collision )
						skip = true;
					break;

				//!!!!
				//case nameof( CollisionFrictionMode ):
				//	if( !Collision || CollisionMaterial.Value != null )
				//		skip = true;
				//	break;

				case nameof( CollisionFriction ):
					if( !Collision || CollisionMaterial.Value != null )
						skip = true;
					break;

				//!!!!
				//case nameof( CollisionRollingFriction ):
				//case nameof( CollisionSpinningFriction ):
				//case nameof( CollisionAnisotropicFriction ):
				//	if( !Collision || CollisionFrictionMode.Value == PhysicalMaterial.FrictionModeEnum.Simple || CollisionMaterial.Value != null )
				//		skip = true;
				//	break;

				case nameof( CollisionRestitution ):
					if( !Collision || CollisionMaterial.Value != null )
						skip = true;
					break;

				case nameof( MaterialUVCurvatureFrequency ):
					if( MaterialUVCurvatureIntensity.Value == 0 )
						skip = true;
					break;

				case nameof( SurfaceObjects ):
					if( !Surface.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( SurfaceObjectsDistribution ):
				case nameof( SurfaceObjectsScale ):
				case nameof( SurfaceObjectsColor ):
				case nameof( SurfaceObjectsVisibilityDistanceFactor ):
				case nameof( SurfaceObjectsCastShadows ):
				case nameof( SurfaceObjectsCollision ):
					if( !Surface.ReferenceOrValueSpecified || !SurfaceObjects )
						skip = true;
					break;

				case nameof( LODCount ):
				case nameof( LODDistance ):
					if( !LODEnabled )
						skip = true;
					break;
				}
			}
		}

		static int ParseHeightmapSize( HeightmapSizeEnum enumValue )
		{
			switch( enumValue )
			{
			case HeightmapSizeEnum._128x128: return 128;
			case HeightmapSizeEnum._256x256: return 256;
			case HeightmapSizeEnum._512x512: return 512;
			case HeightmapSizeEnum._1024x1024: return 1024;
			case HeightmapSizeEnum._2048x2048: return 2048;
				//case HeightmapSizeEnum._4096x4096: return 4096;
			}

			var str = enumValue.ToString();
			var index = str.IndexOf( "x" );
			var str2 = str.Substring( 1, index - 1 );
			return int.Parse( str2 );
		}

		public int GetHeightmapSizeInteger()
		{
			return ParseHeightmapSize( HeightmapSize );
		}

		static int ParsePaintMaskSize( PaintMaskSizeEnum enumValue )
		{
			switch( enumValue )
			{
			case PaintMaskSizeEnum._512x512: return 512;
			case PaintMaskSizeEnum._1024x1024: return 1024;
			case PaintMaskSizeEnum._2048x2048: return 2048;
			case PaintMaskSizeEnum._4096x4096: return 4096;
			}

			var str = enumValue.ToString();
			var index = str.IndexOf( "x" );
			var str2 = str.Substring( 1, index - 1 );
			return int.Parse( str2 );
		}

		public int GetPaintMaskSizeInteger()
		{
			return ParsePaintMaskSize( PaintMaskSize );
		}

		static float[,] HeightmapFromByteArray( byte[] array )
		{
			int count = array.Length / 4;
			var sideSize = (int)Math.Sqrt( count );
			var result = new float[ sideSize, sideSize ];
			Buffer.BlockCopy( array, 0, result, 0, array.Length );
			return result;
		}

		static byte[] HeightmapToByteArray( float[,] array )
		{
			var result = new byte[ array.Length * 4 ];
			Buffer.BlockCopy( array, 0, result, 0, result.Length );
			return result;
		}

		//old format
		string GetLoadVirtualDirectory()
		{
			string name = GetPathFromRoot();
			foreach( char c in new string( Path.GetInvalidFileNameChars() ) + new string( Path.GetInvalidPathChars() ) )
				name = name.Replace( c.ToString(), "_" );
			name = name.Replace( " ", "_" );
			return PathUtility.Combine( ComponentUtility.GetOwnedFileNameOfComponent( this ) + "_Files", name );
		}

		//old format
		//string GetSaveRealDirectory( string realFileName )
		//{
		//	string name = GetPathFromRoot();
		//	foreach( char c in new string( Path.GetInvalidFileNameChars() ) + new string( Path.GetInvalidPathChars() ) )
		//		name = name.Replace( c.ToString(), "_" );
		//	name = name.Replace( " ", "_" );
		//	return PathUtility.Combine( realFileName + "_Files", name );
		//}

		public unsafe static bool LoadHeightmapBuffer( string realFileName, int demandedHeightmapSize, out float[,] heightmapBuffer, out string error )
		{
			heightmapBuffer = null;
			error = "";

			if( !ImageUtility.LoadFromRealFile( realFileName, out var data, out var size, out _, out var format, out _, out _, out error ) )
				return false;

			heightmapBuffer = new float[ demandedHeightmapSize + 1, demandedHeightmapSize + 1 ];

			if( format == PixelFormat.Float32R )
			{
				int length = demandedHeightmapSize + 1;
				if( size.X != length || size.Y != length )
				{
					error = $"Invalid heightmap size \'{size}\'. Must be \'{length} {length}\'.";
					return false;
				}

				fixed( byte* pData2 = data )
				{
					float* pData = (float*)pData2;

					int current = 0;
					for( int y = 0; y < length; y++ )
					{
						for( int x = 0; x < length; x++ )
						{
							heightmapBuffer[ x, y ] = pData[ current ];
							current++;
						}
					}
				}
			}
			else if( format == PixelFormat.Float32RGBA )
			{
				int length = demandedHeightmapSize + 1;
				if( size.X != length || size.Y != length )
				{
					error = $"Invalid heightmap size \"{size}\". Must be \"{length} {length}\".";
					return false;
				}

				fixed( byte* pData2 = data )
				{
					float* pData = (float*)pData2;

					int current = 0;
					for( int y = 0; y < length; y++ )
					{
						for( int x = 0; x < length; x++ )
						{
							heightmapBuffer[ x, y ] = pData[ current * 4 ];
							current++;
						}
					}
				}
			}
			else
			{
				error = $"Invalid heightmap format \"{format}\". Must be {PixelFormat.Float32R} or {PixelFormat.Float32RGBA}.";
				return false;
			}

			return true;
		}

		unsafe bool LoadHeightmapBufferOldFormat( string virtualDirectory, out string error )
		{
			string virtualFileName = PathUtility.Combine( virtualDirectory, "Heightmap.exr" );

			if( VirtualFile.Exists( virtualFileName ) )
			{
				if( !ImageUtility.LoadFromVirtualFile( virtualFileName, out var data, out var size, out _, out var format, out _, out _, out error ) )
					return false;

				if( format == PixelFormat.Float32R )
				{
					int length = heightmapSize + 1;
					if( size.X != length || size.Y != length )
					{
						error = $"Invalid heightmap size \"{size}\". Must be \"{length} {length}\".";
						return false;
					}

					fixed( byte* pData2 = data )
					{
						float* pData = (float*)pData2;

						int current = 0;
						for( int y = 0; y < length; y++ )
						{
							for( int x = 0; x < length; x++ )
							{
								heightmapBuffer[ x, y ] = pData[ current ];
								current++;
							}
						}
					}
				}
				else if( format == PixelFormat.Float32RGBA )
				{
					int length = heightmapSize + 1;
					if( size.X != length || size.Y != length )
					{
						error = $"Invalid heightmap size \"{size}\". Must be \"{length} {length}\".";
						return false;
					}

					fixed( byte* pData2 = data )
					{
						float* pData = (float*)pData2;

						int current = 0;
						for( int y = 0; y < length; y++ )
						{
							for( int x = 0; x < length; x++ )
							{
								heightmapBuffer[ x, y ] = pData[ current * 4 ];
								current++;
							}
						}
					}
				}
				else
				{
					error = $"Invalid heightmap format \"{format}\". Must be {PixelFormat.Float32R} or {PixelFormat.Float32RGBA}.";
					return false;
				}
			}

			//if( format != PixelFormat.Float32R )
			//{
			//	error = $"Invalid heightmap format \"{format}\". Must be \"{PixelFormat.Float32R}\".";
			//	return false;
			//}

			//int length = heightmapSize + 1;
			//if( size.X != length || size.Y != length )
			//{
			//	error = $"Invalid heightmap size \"{size}\". Must be \"{length} {length}\".";
			//	return false;
			//}

			//fixed ( byte* pData2 = data )
			//{
			//	float* pData = (float*)pData2;

			//	int current = 0;
			//	for( int y = 0; y < length; y++ )
			//	{
			//		for( int x = 0; x < length; x++ )
			//		{
			//			heightmapBuffer[ x, y ] = pData[ current ];
			//			current++;
			//		}
			//	}
			//}

			error = "";
			return true;
		}

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			try
			{
				//load heightmap buffer
				{
					string str = block.GetAttribute( nameof( HeightmapBuffer ), "" );
					if( !string.IsNullOrEmpty( str ) )
					{
						var byteArray = Convert.FromBase64String( str );

						if( bool.Parse( block.GetAttribute( nameof( HeightmapBuffer ) + "Zip", false.ToString() ) ) )
							byteArray = IOUtility.Unzip( byteArray );

						heightmapBuffer = HeightmapFromByteArray( byteArray );
					}
					else
					{
						//old format
						var virtualDirectory = GetLoadVirtualDirectory();
						if( !LoadHeightmapBufferOldFormat( virtualDirectory, out error ) )
							return false;
					}
				}

				//load holes cache
				{
					var holesCacheBlock = block.FindChild( "HolesCache" );
					if( holesCacheBlock != null )
					{
						foreach( var itemBlock in holesCacheBlock.Children )
						{
							if( itemBlock.Name == "Item" )
							{
								var index = Vector2I.Parse( itemBlock.GetAttribute( "Index", "0 0" ) );

								Vector3[] vertices = null;
								unsafe
								{
									string str = itemBlock.GetAttribute( "Vertices" );
									if( !string.IsNullOrEmpty( str ) )
									{
										var verticesBytes = Convert.FromBase64String( str );
										var count = verticesBytes.Length / sizeof( Vector3 );

										vertices = new Vector3[ count ];
										fixed( Vector3* pVertices = vertices )
										fixed( byte* pVerticesBytes = verticesBytes )
											NativeUtility.CopyMemory( pVertices, pVerticesBytes, verticesBytes.Length );
									}
								}

								int[] indices = null;
								{
									string str = itemBlock.GetAttribute( "Indices" );
									if( !string.IsNullOrEmpty( str ) )
									{
										var strings = str.Split( new char[] { ' ' } );

										indices = new int[ strings.Length ];
										for( int n = 0; n < strings.Length; n++ )
											indices[ n ] = int.Parse( strings[ n ] );
									}
								}

								if( vertices == null || indices == null )
								{
									vertices = new Vector3[ 0 ];
									indices = new int[ 0 ];
								}

								var item = new HoleCacheItem();
								item.vertices = vertices;
								item.indices = indices;
								holesCache[ index ] = item;
							}
						}
					}
				}
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			return true;
		}

		//unsafe bool SaveHeightmapBuffer( string realDirectory, out string error )
		public unsafe bool SaveHeightmapBuffer( string realFileName, out string error )
		{
			if( heightmapBuffer != null )
			{
				//string realFileName = PathUtility.Combine( realDirectory, "Heightmap.exr" );

				int length = heightmapSize + 1;
				var data = new float[ length * length ];

				int current = 0;
				for( int y = 0; y < length; y++ )
				{
					for( int x = 0; x < length; x++ )
					{
						data[ current ] = heightmapBuffer[ x, y ];
						current++;
					}
				}

				fixed( float* pData = data )
				{
					if( !ImageUtility.Save( realFileName, (IntPtr)pData, new Vector2I( length, length ), 1, PixelFormat.Float32R, 1, 0, out error ) )
						return false;
				}
			}

			error = "";
			return true;
		}

		protected override bool OnSave( Metadata.SaveContext context, TextBlock block, ref bool skipSave, out string error )
		{
			if( !base.OnSave( context, block, ref skipSave, out error ) )
				return false;

			//save heightmap buffer
			{
				var byteArray = HeightmapToByteArray( HeightmapBuffer );

				var zipped = IOUtility.Zip( byteArray, System.IO.Compression.CompressionLevel.Fastest );
				if( zipped.Length < (int)( (float)byteArray.Length / 1.25 ) )
				{
					block.SetAttribute( nameof( HeightmapBuffer ), Convert.ToBase64String( zipped, Base64FormattingOptions.None ) );
					block.SetAttribute( nameof( HeightmapBuffer ) + "Zip", true.ToString() );
				}
				else
					block.SetAttribute( nameof( HeightmapBuffer ), Convert.ToBase64String( byteArray, Base64FormattingOptions.None ) );


				//old format

				//var realDirectory = GetSaveRealDirectory( context.RealFileName );
				////var realDirectory = VirtualPathUtility.GetRealPathByVirtual( GetLoadVirtualDirectory( context.realFileName ) );

				//try
				//{
				//	if( !Directory.Exists( realDirectory ) )
				//		Directory.CreateDirectory( realDirectory );
				//}
				//catch( Exception e )
				//{
				//	error = e.Message;
				//	return false;
				//}

				////if( loadingStatus != LoadingStatuses.Loading )
				////{
				//if( !SaveHeightmapBuffer( realDirectory, out error ) )
				//	return false;
				////}
			}

			//save holes cache
			if( holesCache.Count != 0 )
			{
				var holesCacheBlock = block.AddChild( "HolesCache" );

				foreach( var pair in holesCache )
				{
					var itemBlock = holesCacheBlock.AddChild( "Item" );

					itemBlock.SetAttribute( "Index", pair.Key.ToString() );

					unsafe
					{
						var vertices = pair.Value.vertices;

						byte[] verticesBytes;
						fixed( Vector3* pVertices = vertices )
						{
							verticesBytes = new byte[ vertices.Length * sizeof( Vector3 ) ];
							fixed( byte* pVerticesBytes = verticesBytes )
								NativeUtility.CopyMemory( pVerticesBytes, pVertices, verticesBytes.Length );
						}

						string str = Convert.ToBase64String( verticesBytes, Base64FormattingOptions.None );
						itemBlock.SetAttribute( "Vertices", str );
					}

					{
						var indices = pair.Value.indices;

						var builder = new StringBuilder( indices.Length * 3 );
						for( int n = 0; n < indices.Length; n++ )
						{
							var v = indices[ n ];
							if( builder.Length != 0 )
								builder.Append( ' ' );
							builder.Append( v.ToString() );
						}

						itemBlock.SetAttribute( "Indices", builder.ToString() );
					}
				}
			}

			return true;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = FindParent<Scene>();
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
				else
					scene.GetRenderSceneData -= Scene_GetRenderSceneData;
			}

			//double t = EngineApp.GetSystemTime();
			try
			{
				allowRecreateInternalData = false;

				if( EnabledInHierarchyAndIsInstance )
				{
					CreateTiles();
					CreateCollisionBodies();

					if( NeedPrecalculateObjects() )
					{
						foreach( var tile in GetTiles() )
							TileGetRenderSceneDataSurfaceObjects( tile, null, GetRenderSceneDataMode.InsideFrustum, null );
					}
				}
				else
				{
					DestroyCollisionBodies();
					DestroyTiles();
				}
			}
			finally
			{
				allowRecreateInternalData = true;
			}
			//Log.Warning( EngineApp.GetSystemTime() - t );

			//из OnPostCreate

			//if( !Editor_IsExcludedFromWorld() )
			//{
			//	//no load when HeightmapTerrainManager exists
			//	if( loaded )
			//	{
			//		if( heightmapTerrainManager == null ||
			//			EngineApp.Instance.ApplicationType == EngineApp.ApplicationTypes.ShaderCacheCompiler )
			//		{
			//			if( !LoadAllData( false ) )
			//				return;

			//			loadingStatus = LoadingStatuses.Loaded;
			//		}
			//		else
			//		{
			//			enabled = false;
			//		}
			//	}
			//	else
			//	{
			//		loadingStatus = LoadingStatuses.Loaded;
			//	}
			//}

			//if( loadingStatus == LoadingStatuses.Loaded )
			//{
			//	UpdateAllRecreatableDataForRendering();
			//	CreateCollisionBodies();

			//}



			//OnDestroy


			//при изменении Enabled

			//if( IsPostCreated && loadingStatus == LoadingStatuses.Loaded && !streamingEnable )
			//{
			//	UpdateAllRecreatableDataForRendering();
			//	CreateCollisionBodies();
			//}
		}

		void TileGetRenderSceneData( Tile tile, ViewportRenderingContext context )
		{
			//hide label
			var context2 = context.ObjectInSpaceRenderingContext;
			context2.disableShowingLabelForThisObject = true;
		}

		private void Scene_GetRenderSceneData( Scene scene, ViewportRenderingContext context )
		{
			var context2 = context.ObjectInSpaceRenderingContext;

			if( context2.selectedObjects.Contains( this ) )
			{
				var bounds = CalculateBounds();
				if( !bounds.IsCleared() )
				{
					if( context.Owner.Simple3DRenderer != null )
					{
						ColorValue color = ProjectSettings.Get.Colors.SelectedColor;
						context.Owner.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
						RendererUtility.AddBoundsSegmented( context.Owner.Simple3DRenderer, bounds );
					}
				}
			}
		}

		void CreateTiles( bool updateHoles = false )
		{
			DestroyTiles();

			if( !EnabledInHierarchyAndIsInstance )
				return;

			//!!!!так? где еще
			CalculateCachedGeneralData();

			//calculate tile count
			tileCount = heightmapSize / tileSize;
			if( heightmapSize % tileSize != 0 )
				Log.Fatal( "Terrain: CreateTiles: heightmapSize % tileSize != 0." );

			//create arrays
			tiles = new Tile[ tileCount, tileCount ];
			for( int tileY = 0; tileY < tileCount; tileY++ )
			{
				for( int tileX = 0; tileX < tileCount; tileX++ )
				{
					//LongOperationCallbackManager.CallCallback( "HeightmapTerrain: GenerateTile: " + tileIndex.ToString() );

					var tile = new Tile( this, new Vector2I( tileX, tileY ) );
					tiles[ tileX, tileY ] = tile;
					//tile.UpdateRenderingData( true );
				}
			}

			//update rendering data

			if( updateHoles )
				HolesCacheClear();

			UpdateCurrentLayers();

			Vector3[,] calculatedPositionsForGetNormals = null;
			{
				int length = heightmapSize + 1;
				calculatedPositionsForGetNormals = new Vector3[ length, length ];
				Parallel.For( 0, length, delegate ( int y )
				{
					for( int x = 0; x < length; x++ )
						GetPosition_ForGetNormal( new Vector2I( x, y ), null, out calculatedPositionsForGetNormals[ x, y ] );
				} );
				//for( int y = 0; y < length; y++ )
				//	for( int x = 0; x < length; x++ )
				//		GetPosition_ForGetNormal( new Vector2I( x, y ), null, out calculatedPositionsForGetNormals[ x, y ] );
			}

			foreach( var tile in GetTiles() )
			{
				tile.UpdateRenderingData( true, calculatedPositionsForGetNormals, updateHoles, true );
				tile.UpdateLayers();
				tile.UpdateSurfaceObjectsData( calculatedPositionsForGetNormals );
				tile.NeedUpdate = 0;
			}

			//!!!!тут? где еще?
			NetworkSendData( null );
		}

		void DestroyTiles()
		{
			if( tiles != null )
			{
				int count = tiles.GetLength( 0 );
				for( int tileY = 0; tileY < count; tileY++ )
					for( int tileX = 0; tileX < count; tileX++ )
						tiles[ tileX, tileY ].Destroy();
				tiles = null;
			}
		}

		public delegate void GetHeightWithoutPositionOverrideDelegate( Terrain sender, Vector2I index, ref float value );
		public event GetHeightWithoutPositionOverrideDelegate GetHeightWithoutPositionOverride;

		public float GetHeightWithoutPosition( Vector2I index, bool applyOverride )
		{
			var result = heightmapBuffer[ index.X, heightmapSize - index.Y ];
			if( applyOverride )
				GetHeightWithoutPositionOverride?.Invoke( this, new Vector2I( index.X, heightmapSize - index.Y ), ref result );
			//GetHeightWithoutPositionOverride?.Invoke( this, index, ref result );
			return result;
		}

		public double GetHeight( Vector2I index )
		{
			if( cachedCellSize == 0 )
				CalculateCachedGeneralData();
			return cachedPosition.Z + GetHeightWithoutPosition( index, true );
			//return cachedPosition.Z + heightmapBuffer[ index.X, heightmapSize - index.Y ];
		}

		public void ClampCellIndex( ref Vector2I index )
		{
			int bufferSize = heightmapSize + 1;
			if( index.X < 0 ) index.X = 0;
			if( index.X >= bufferSize ) index.X = bufferSize - 1;
			if( index.Y < 0 ) index.Y = 0;
			if( index.Y >= bufferSize ) index.Y = bufferSize - 1;
		}

		bool IsValidCellIndex( Vector2I index )
		{
			int bufferSize = heightmapSize + 1;
			if( index.X < 0 || index.X >= bufferSize || index.Y < 0 || index.Y >= bufferSize )
				return false;
			return true;
		}

		double GetHeightWithClamp( Vector2I index )
		{
			ClampCellIndex( ref index );
			return GetHeight( index );
		}

		public Vector2I GetCellIndexByPosition( ref Vector2 position )
		{
			if( cachedCellSize == 0 )
				CalculateCachedGeneralData();
			double floatIndexX = ( position.X - cachedBounds.Minimum.X ) * cachedCellSizeInv;
			double floatIndexY = ( position.Y - cachedBounds.Minimum.Y ) * cachedCellSizeInv;
			return new Vector2I( (int)floatIndexX, (int)floatIndexY );
			//Vector2 floatIndex = ( position - cachedBounds.Minimum.ToVector2() ) * cachedCellSizeInv;
			//return new Vector2I( (int)floatIndex.X, (int)floatIndex.Y );
		}

		public Vector2I GetCellIndexByPosition( Vector2 position )
		{
			return GetCellIndexByPosition( ref position );
		}

		public Vector2I GetMaskIndexByPosition( ref Vector2 position )
		{
			if( cachedCellSize == 0 )
				CalculateCachedGeneralData();
			double floatIndexX = ( position.X - cachedBounds.Minimum.X ) * cachedMaskCellSizeInv;
			double floatIndexY = ( position.Y - cachedBounds.Minimum.Y ) * cachedMaskCellSizeInv;
			return new Vector2I( (int)floatIndexX, (int)floatIndexY );
			//Vector2 floatIndex = ( position - cachedBounds.Minimum.ToVector2() ) * cachedMaskCellSizeInv;
			//return new Vector2I( (int)floatIndex.X, (int)floatIndex.Y );
		}

		public Vector2I GetMaskIndexByPosition( Vector2 position )
		{
			return GetMaskIndexByPosition( ref position );
		}

		public float GetCellSize()
		{
			if( cachedCellSize == 0 )
				CalculateCachedGeneralData();
			return cachedCellSize;
		}

		void ClampTileIndex( ref Vector2I tileIndex )
		{
			if( tileIndex.X < 0 ) tileIndex.X = 0;
			if( tileIndex.X >= tileCount ) tileIndex.X = tileCount - 1;
			if( tileIndex.Y < 0 ) tileIndex.Y = 0;
			if( tileIndex.Y >= tileCount ) tileIndex.Y = tileCount - 1;
		}

		//void ClampBodyIndex( ref Vector2I bodyIndex )
		//{
		//	int bodyCount = GetBodyCountByAxis();
		//	if( bodyIndex.X < 0 ) bodyIndex.X = 0;
		//	if( bodyIndex.X >= bodyCount ) bodyIndex.X = bodyCount - 1;
		//	if( bodyIndex.Y < 0 ) bodyIndex.Y = 0;
		//	if( bodyIndex.Y >= bodyCount ) bodyIndex.Y = bodyCount - 1;
		//}

		public void ClampMaskIndex( ref Vector2I maskIndex )
		{
			if( maskIndex.X < 0 ) maskIndex.X = 0;
			if( maskIndex.X > cachedPaintMaskSize ) maskIndex.X = cachedPaintMaskSize;
			if( maskIndex.Y < 0 ) maskIndex.Y = 0;
			if( maskIndex.Y > cachedPaintMaskSize ) maskIndex.Y = cachedPaintMaskSize;
		}

		Vector2I GetTileIndexByCellIndex( Vector2I index )
		{
			return index / tileSize;
		}

		//internal Vector2I GetMasksTextureIndexByTileIndex( Vector2I tileIndex )
		//{
		//	return tileIndex / tilesCountPerMasksTexture;
		//}

		//internal Vector2I GetAOTextureIndexByTileIndex( Vector2I tileIndex )
		//{
		//	return tileIndex / tilesCountPerAOTexture;
		//}

		Vector2I GetTileIndexByMaskIndex( Vector2I index )
		{
			return index * tileCount / cachedPaintMaskSize;
		}

		Vector2I GetMaskIndexByCellIndex( Vector2I index )
		{
			return ( index * cachedPaintMaskSize ) / heightmapSize;
		}

		//Vector2I GetBodyIndexByCellIndex( Vector2I index )
		//{
		//	return index / GetBodyCellSize();
		//}

		public void SetHeightWithoutPosition( Vector2I index, float value )
		{
			heightmapBuffer[ index.X, heightmapSize - index.Y ] = value;

			//SetModified( true );

			if( !heightMinMaxNeedUpdate )
			{
				if( value < heightMinMax.Minimum || value > heightMinMax.Maximum )
					heightMinMaxNeedUpdate = true;
			}
		}

		void GetPosition( Vector2I cellIndex, out Vector3 result )
		{
			var height = GetHeight( cellIndex );

			result = new Vector3(
				cachedBounds.Minimum.X + (float)cellIndex.X * cachedCellSize,
				cachedBounds.Minimum.Y + (float)cellIndex.Y * cachedCellSize,
				height );
		}

		public Vector2 GetPositionXY( Vector2I index )
		{
			if( cachedCellSize == 0 )
				CalculateCachedGeneralData();
			return cachedBounds.Minimum.ToVector2() + new Vector2( (float)index.X * cachedCellSize, (float)index.Y * cachedCellSize );
		}

		public Vector2 GetPositionXYByMaskIndex( Vector2I index )
		{
			if( cachedCellSize == 0 )
				CalculateCachedGeneralData();
			return cachedBounds.Minimum.ToVector2() + new Vector2( (float)index.X * cachedMaskCellSize, (float)index.Y * cachedMaskCellSize );
		}

		void GetPositionWithClamp( Vector2I cellIndex, out Vector3 result )
		{
			ClampCellIndex( ref cellIndex );
			GetPosition( cellIndex, out result );
		}

		/// <summary>
		/// </summary>
		/// <param name="position"></param>
		/// <param name="considerHoles"></param>
		/// <returns>if hole and considerHoles == true, then Double.MinValue will be returned.</returns>
		public double GetHeight( Vector2 position, bool considerHoles )
		{
			Vector2I index = GetCellIndexByPosition( ref position );
			GetPositionWithClamp( index, out var p0 );

			if( considerHoles && HolesEnabled && tiles != null )
			{
				var tileIndex = GetTileIndexByCellIndex( index );
				ClampTileIndex( ref tileIndex );

				var tile = tiles[ tileIndex.X, tileIndex.Y ];
				if( tile != null && tile.ContainsHoles )
				{
					var ray = new Ray( Position + new Vector3( position, 100000 ), new Vector3( 0.0001, 0, -200000 ) );
					if( FindMeshRayIntersection( ray, tile.Vertices, tile.Indices, out var p ) )
						return p.Z;
					else
						return double.MinValue;
				}
			}

			Vector3 p1;
			Vector3 p2;
			{
				bool xTriangle = position.X - p0.X > position.Y - p0.Y;

				Vector3 pXY = new Vector3( p0.X + cachedCellSize, p0.Y + cachedCellSize, GetHeightWithClamp( new Vector2I( index.X + 1, index.Y + 1 ) ) );

				if( xTriangle )
				{
					p1 = new Vector3( p0.X + cachedCellSize, p0.Y, GetHeightWithClamp( new Vector2I( index.X + 1, index.Y ) ) );
					p2 = pXY;
				}
				else
				{
					p1 = pXY;
					p2 = new Vector3( p0.X, p0.Y + cachedCellSize, GetHeightWithClamp( new Vector2I( index.X, index.Y + 1 ) ) );
				}
			}

			double height;
			{
				//triangle ray intersection
				Ray ray = new Ray(
					new Vector3( position.X, position.Y, Position.Value.Z + 10000.0f ),
					new Vector3( 0, 0, -20000.0f ) );
				//Ray ray = new Ray(
				//   new Vector3( pos.X, pos.Y, position.Z + verticalSize + .5f ),
				//   new Vector3( 0, 0, -verticalSize - 1 ) );

				Vector3 edge1 = p1 - p0;
				Vector3 edge2 = p2 - p0;
				Vector3.Cross( ref ray.Direction, ref edge2, out var pvec );
				double det = Vector3.Dot( ref edge1, ref pvec );
				Vector3 tvec = ray.Origin - p0;
				Vector3.Cross( ref tvec, ref edge1, out var qvec );
				double t = Vector3.Dot( ref edge2, ref qvec ) * ( 1.0f / det );
				height = ray.Direction.Z * t + ray.Origin.Z;
			}

			return height;
		}

		static void GetPlaneNormal( ref Vector3 point0, ref Vector3 point1, ref Vector3 point2, out Vector3 normal )
		{
			Vector3 edge1;
			Vector3.Subtract( ref point1, ref point0, out edge1 );
			Vector3 edge2;
			Vector3.Subtract( ref point2, ref point0, out edge2 );
			Vector3.Cross( ref edge1, ref edge2, out normal );
			normal.Normalize();
		}

		//Vector3 GetNormal_Old( Vector2I cellIndex )
		//{
		//   if( cachedCellSize == 0 )
		//      CalculateCachedGeneralData();

		//   int length = heightmapSize + 1;

		//   int x = cellIndex.X;
		//   int y = cellIndex.Y;

		//   Vector3 xPlus = Vector3.Zero;
		//   if( x + 1 < length )
		//      xPlus = GetPosition( new Vector2I( x + 1, y ) );
		//   Vector3 yPlus = Vector3.Zero;
		//   if( y + 1 < length )
		//      yPlus = GetPosition( new Vector2I( x, y + 1 ) );
		//   Vector3 xMinus = Vector3.Zero;
		//   if( x > 0 )
		//      xMinus = GetPosition( new Vector2I( x - 1, y ) );
		//   Vector3 yMinus = Vector3.Zero;
		//   if( y > 0 )
		//      yMinus = GetPosition( new Vector2I( x, y - 1 ) );

		//   Vector3 normal = Vector3.Zero;

		//   if( x > 0 && y > 0 )
		//   {
		//      Vector3 v = GetPosition( new Vector2I( x - 1, y - 1 ) );
		//      Vector3 n;
		//      GetPlaneNormal( ref xMinus, ref v, ref yMinus, out n );
		//      normal += n;
		//   }
		//   if( x + 1 < length && y > 0 )
		//   {
		//      Vector3 v = GetPosition( new Vector2I( x + 1, y - 1 ) );
		//      Vector3 n;
		//      GetPlaneNormal( ref yMinus, ref v, ref xPlus, out n );
		//      normal += n;
		//   }
		//   if( x + 1 < length && y + 1 < length )
		//   {
		//      Vector3 v = GetPosition( new Vector2I( x + 1, y + 1 ) );
		//      Vector3 n;
		//      GetPlaneNormal( ref xPlus, ref v, ref yPlus, out n );
		//      normal += n;
		//   }
		//   if( x > 0 && y + 1 < length )
		//   {
		//      Vector3 v = GetPosition( new Vector2I( x - 1, y + 1 ) );
		//      Vector3 n;
		//      GetPlaneNormal( ref yPlus, ref v, ref xMinus, out n );
		//      normal += n;
		//   }

		//   normal.Normalize();

		//   return normal;
		//}

		void GetPosition_ForGetNormal( Vector2I cellIndex, Vector3[,] calculatedPositionsForGetNormals, out Vector3 result )
		{
			if( calculatedPositionsForGetNormals != null )
				result = calculatedPositionsForGetNormals[ cellIndex.X, cellIndex.Y ];
			else
			{
				result = new Vector3(
					(float)cellIndex.X * cachedCellSize,
					(float)cellIndex.Y * cachedCellSize,
					GetHeightWithoutPosition( cellIndex, true ) );//GetHeight( cellIndex ) );
			}
		}

		void GetNormal( Vector2I cellIndex, Vector3[,] calculatedPositionsForGetNormals, out Vector3F result )
		{
			if( cachedCellSize == 0 )
				CalculateCachedGeneralData();

			int length = heightmapSize + 1;

			int x = cellIndex.X;
			int y = cellIndex.Y;

			Vector3 xPlus;
			if( x + 1 < length )
				GetPosition_ForGetNormal( new Vector2I( x + 1, y ), calculatedPositionsForGetNormals, out xPlus );
			else
				xPlus = Vector3.Zero;
			Vector3 yPlus;
			if( y + 1 < length )
				GetPosition_ForGetNormal( new Vector2I( x, y + 1 ), calculatedPositionsForGetNormals, out yPlus );
			else
				yPlus = Vector3.Zero;
			Vector3 xMinus;
			if( x > 0 )
				GetPosition_ForGetNormal( new Vector2I( x - 1, y ), calculatedPositionsForGetNormals, out xMinus );
			else
				xMinus = Vector3.Zero;
			Vector3 yMinus;
			if( y > 0 )
				GetPosition_ForGetNormal( new Vector2I( x, y - 1 ), calculatedPositionsForGetNormals, out yMinus );
			else
				yMinus = Vector3.Zero;

			Vector3 normal = Vector3.Zero;

			if( x > 0 && y > 0 )
			{
				GetPosition_ForGetNormal( new Vector2I( x - 1, y - 1 ), calculatedPositionsForGetNormals, out var v );
				GetPlaneNormal( ref xMinus, ref v, ref yMinus, out var n );
				normal += n;
			}
			if( x + 1 < length && y > 0 )
			{
				GetPosition_ForGetNormal( new Vector2I( x + 1, y - 1 ), calculatedPositionsForGetNormals, out var v );
				GetPlaneNormal( ref yMinus, ref v, ref xPlus, out var n );
				normal += n;
			}
			if( x + 1 < length && y + 1 < length )
			{
				GetPosition_ForGetNormal( new Vector2I( x + 1, y + 1 ), calculatedPositionsForGetNormals, out var v );
				GetPlaneNormal( ref xPlus, ref v, ref yPlus, out var n );
				normal += n;
			}
			if( x > 0 && y + 1 < length )
			{
				GetPosition_ForGetNormal( new Vector2I( x - 1, y + 1 ), calculatedPositionsForGetNormals, out var v );
				GetPlaneNormal( ref yPlus, ref v, ref xMinus, out var n );
				normal += n;
			}

			normal.Normalize();

			result = normal.ToVector3F();
		}

		public Vector3F GetNormal( Vector2I cellIndex )
		{
			GetNormal( cellIndex, null, out var result );
			return result;
		}

		void GetNormal( ref Vector2 pos, Vector3[,] calculatedPositionsForGetNormals, out Vector3F result )
		{
			Vector2I cellIndex = GetCellIndexByPosition( ref pos );
			if( IsValidCellIndex( cellIndex ) )
				GetNormal( cellIndex, calculatedPositionsForGetNormals, out result );
			else
				result = Vector3F.ZAxis;
		}

		public Vector3F GetNormal( Vector2 pos )
		{
			Vector2I cellIndex = GetCellIndexByPosition( ref pos );
			if( IsValidCellIndex( cellIndex ) )
				return GetNormal( cellIndex );
			return Vector3F.ZAxis;
		}

		//Vector2 CalculateMasksTexCoord( Tile tile, Vector2I cellIndex )
		//{
		//	if( tilesCountPerMasksTexture == 0 )
		//		CalculateNeedForMasksFields();

		//	Vector2I masksTextureCoord = tile.GetMasksTextureCoordMin() +
		//		( GetMaskIndexByCellIndex( cellIndex ) - tile.GetMasksIndexMin() );

		//	Vector2 masksTextureCoordFloat = masksTextureCoord.ToVector2() + new Vector2( .5f, .5f );

		//	return masksTextureCoordFloat * masksTextureSizeInv;
		//}

		//Vector2 CalculateAOTexCoord( Tile tile, Vector2I cellIndex )
		//{
		//	Vector2I aoTextureCoord = tile.GetAOTextureCoordMin() +
		//		( GetAOIndexByCellIndex( cellIndex ) - tile.GetAOIndexMin() );

		//	Vector2 aoTextureCoordFloat = aoTextureCoord.ToVector2() + new Vector2( .5f, .5f );

		//	return aoTextureCoordFloat * aoTextureSizeInv;
		//}

		//void GenerateTile_FinishStreamingUpdate( Tile tile )
		//{
		//	//calculate Bounds for staticMeshObject
		//	if( tile.staticMeshObject != null )
		//	{
		//		float minHeight = float.MaxValue;
		//		float maxHeight = float.MinValue;
		//		for( int y = tile.cellIndexMin.Y; y <= tile.cellIndexMax.Y; y++ )
		//		{
		//			for( int x = tile.cellIndexMin.X; x <= tile.cellIndexMax.X; x++ )
		//			{
		//				float h = GetHeight( new Vector2I( x, y ) );
		//				if( h < minHeight )
		//					minHeight = h;
		//				if( h > maxHeight )
		//					maxHeight = h;
		//			}
		//		}
		//		Bounds bounds = new Bounds(
		//			new Vector3( GetPosition( tile.cellIndexMin ).ToVector2(), minHeight ),
		//			new Vector3( GetPosition( tile.cellIndexMax ).ToVector2(), maxHeight ) );

		//		//Bounds bounds = Bounds.Cleared;
		//		//for( int y = tile.cellIndexMin.Y; y <= tile.cellIndexMax.Y; y++ )
		//		//   for( int x = tile.cellIndexMin.X; x <= tile.cellIndexMax.X; x++ )
		//		//      bounds.Add( GetPosition( new Vector2I( x, y ) ) );

		//		tile.staticMeshObject.UpdateBounds( bounds );
		//	}

		//	tile.UpdateGpuParameters();
		//	if( !IsUseSharedVertexBuffers() )
		//		UpdateTileVertexDatasForNonSharedVertexBuffersMode( tile );//, false );
		//	tile.UpdateUseStaticLightingData();
		//}

		Tile GetTileByIndex( Vector2I tileIndex )
		{
			return tiles[ tileIndex.X, tileIndex.Y ];
		}

		bool GetTiles( Ray ray, Predicate<Tile> callback )
		{
			var scene = FindParent<Scene>();
			if( scene != null )
			{
				//!!!!фильтр
				//!!!!маски в менеджере сцены
				var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, ray );
				scene.GetObjectsInSpace( item );

				foreach( var resultItem in item.Result )
				{
					var tile = GetTileByObjectInSpace( resultItem.Object );
					if( tile != null && !callback( tile ) )
						return false;
				}
			}

			return true;
		}

		bool FindMeshRayIntersection( Ray ray, Vector3[] vertices, IList<int> indices, out Vector3 pos )
		{
			pos = Vector3.Zero;
			double distanceSqr = -1;

			for( int tri = 0; tri < indices.Count; tri += 3 )
			{
				ref var p0 = ref vertices[ indices[ tri + 0 ] ];
				ref var p1 = ref vertices[ indices[ tri + 1 ] ];
				ref var p2 = ref vertices[ indices[ tri + 2 ] ];

				if( MathAlgorithms.IntersectTriangleRay( ref p0, ref p1, ref p2, ref ray, out var scale ) )
				{
					var p = ray.GetPointOnRay( scale );
					var c = ( p0 + p1 + p2 ) * ( 1.0f / 3.0f );
					var sqr = ( ray.Origin - c ).LengthSquared();
					if( distanceSqr == -1 || sqr < distanceSqr )
					{
						distanceSqr = sqr;
						pos = p;
					}
				}
			}

			return distanceSqr != -1;
		}

		void GetTiles( Vector2I tileIndexMin, Vector2I tileIndexMax, Action<Tile> callback )
		{
			for( int tileY = tileIndexMin.Y; tileY <= tileIndexMax.Y; tileY++ )
				for( int tileX = tileIndexMin.X; tileX <= tileIndexMax.X; tileX++ )
					callback( tiles[ tileX, tileY ] );
		}

		List<Tile> GetTiles()
		{
			List<Tile> result = new List<Tile>( tileCount * tileCount );
			if( tiles != null )
			{
				for( int tileY = 0; tileY <= tileCount - 1; tileY++ )
					for( int tileX = 0; tileX <= tileCount - 1; tileX++ )
						result.Add( tiles[ tileX, tileY ] );
			}
			return result;
		}

		void GetTiles( Action<Tile> callback )
		{
			if( tiles != null )
				GetTiles( Vector2I.Zero, new Vector2I( tileCount - 1, tileCount - 1 ), callback );
		}

		void CalculateCachedGeneralData()
		{
			//cachedLODLevelDistances = null;

			var position = Position.Value;
			var horizontalSize = (float)HorizontalSize.Value;

			cachedPosition = position;

			cachedCellSize = horizontalSize / (float)heightmapSize;
			cachedCellSizeInv = 1.0f / cachedCellSize;
			cachedPaintMaskSize = ParsePaintMaskSize( PaintMaskSize );
			cachedMaskCellSize = horizontalSize / (float)cachedPaintMaskSize;
			cachedMaskCellSizeInv = 1.0f / cachedMaskCellSize;

			cachedBounds = new Bounds(
				new Vector3( position.X - horizontalSize * .5f, position.Y - horizontalSize * .5f, GetHeightMinMax().Minimum ),
				new Vector3( position.X + horizontalSize * .5f, position.Y + horizontalSize * .5f, GetHeightMinMax().Maximum ) );

			cachedLodLevelCount = 1;
			//if( LODSettings.Enabled && !IsFixedPipelineFallback() )
			//{
			//	int s = 1;
			//	while( s < tileSize )
			//	{
			//		cachedLodLevelCount++;
			//		s *= 2;
			//	}
			//	if( cachedLodLevelCount > lodSettings.MaxCount )
			//		cachedLodLevelCount = lodSettings.MaxCount;
			//}
		}

		void ResizeHeightmapBufferDivide2()
		{
			int newSize = heightmapSize / 2;
			int newBufferSize = newSize + 1;

			float[,] newBuffer = new float[ newBufferSize, newBufferSize ];
			for( int y = 0; y < newBufferSize; y++ )
				for( int x = 0; x < newBufferSize; x++ )
					newBuffer[ x, y ] = heightmapBuffer[ x * 2, y * 2 ];

			heightmapBuffer = newBuffer;

			//SetModified( true );
		}

		void ResizeHeightmapBufferMultiple2()
		{
			int newSize = heightmapSize * 2;
			int newBufferSize = newSize + 1;

			float[,] newBuffer = new float[ newBufferSize, newBufferSize ];
			for( int y = 0; y < newBufferSize; y++ )
				for( int x = 0; x < newBufferSize; x++ )
					newBuffer[ x, y ] = heightmapBuffer[ x / 2, y / 2 ];

			//smooth
			float[,] newBuffer2 = new float[ newBufferSize, newBufferSize ];
			for( int y = 0; y < newBufferSize; y++ )
			{
				for( int x = 0; x < newBufferSize; x++ )
				{
					float value = 0;

					Vector2I index;
					index = new Vector2I( Math.Max( 0, x - 1 ), y );
					value += newBuffer[ index.X, index.Y ];
					index = new Vector2I( Math.Min( newBufferSize - 1, x + 1 ), y );
					value += newBuffer[ index.X, index.Y ];
					index = new Vector2I( x, Math.Max( 0, y - 1 ) );
					value += newBuffer[ index.X, index.Y ];
					index = new Vector2I( x, Math.Min( newBufferSize - 1, y + 1 ) );
					value += newBuffer[ index.X, index.Y ];
					value /= 4;

					newBuffer2[ x, y ] = value;
				}
			}
			newBuffer = newBuffer2;

			heightmapBuffer = newBuffer;

			//SetModified( true );
		}

		//void ResizeMaskBuffersDivide2()
		//{
		//	int newSize = masksSize / 2;

		//	foreach( Layer layer in layers )
		//	{
		//		if( layer.mask == null )
		//			continue;

		//		byte[,] newBuffer = new byte[ newSize, newSize ];
		//		for( int y = 0; y < newSize; y++ )
		//			for( int x = 0; x < newSize; x++ )
		//				newBuffer[ x, y ] = layer.mask[ x * 2, y * 2 ];

		//		layer.mask = newBuffer;
		//	}

		//	masksSize = newSize;

		//	SetModified( true );
		//}

		//void ResizeMaskBuffersMultiple2()
		//{
		//	int newSize = masksSize * 2;

		//	foreach( Layer layer in layers )
		//	{
		//		if( layer.mask == null )
		//			continue;

		//		byte[,] newBuffer = new byte[ newSize, newSize ];
		//		for( int y = 0; y < newSize; y++ )
		//			for( int x = 0; x < newSize; x++ )
		//				newBuffer[ x, y ] = layer.mask[ x / 2, y / 2 ];

		//		//smooth
		//		byte[,] newBuffer2 = new byte[ newSize, newSize ];

		//		for( int y = 0; y < newSize; y++ )
		//		{
		//			for( int x = 0; x < newSize; x++ )
		//			{
		//				int value = 0;

		//				Vector2I index;
		//				index = new Vector2I( Math.Max( 0, x - 1 ), y );
		//				value += newBuffer[ index.X, index.Y ];
		//				index = new Vector2I( Math.Min( newSize - 1, x + 1 ), y );
		//				value += newBuffer[ index.X, index.Y ];
		//				index = new Vector2I( x, Math.Max( 0, y - 1 ) );
		//				value += newBuffer[ index.X, index.Y ];
		//				index = new Vector2I( x, Math.Min( newSize - 1, y + 1 ) );
		//				value += newBuffer[ index.X, index.Y ];
		//				value /= 4;

		//				newBuffer2[ x, y ] = (byte)value;
		//			}
		//		}

		//		layer.mask = newBuffer2;
		//	}

		//	masksSize = newSize;

		//	SetModified( true );
		//}

		void ResizeHeightmap( int newSize )
		{
			//resize
			while( newSize > heightmapSize )
			{
				ResizeHeightmapBufferMultiple2();
				//!!!!
				//ResizeHoleBufferMultiple2();
				heightmapSize *= 2;
			}
			while( newSize < heightmapSize )
			{
				ResizeHeightmapBufferDivide2();
				//!!!!
				//ResizeHoleBufferDivide2();
				heightmapSize /= 2;
			}

			RecreateInternalData();
		}

		Tile GetTileByObjectInSpace( ObjectInSpace objectInSpace )
		{
			var tile = objectInSpace.AnyData as Tile;
			if( tile != null && tile.owner == this )
				return tile;
			return null;
		}

		Vector2I[] GenerateTileVerticesCellIndices( Tile tile, int lodLevel )
		{
			if( lodLevel >= cachedLodLevelCount )
				Log.Fatal( "Terrain: GenerateTileVerticesCellIndices: lodLevel >= cachedLodLevelCount." );

			int step = 1 << lodLevel;
			int count = 0;
			for( int y = 0; y < tileSize + 1; y += step )
				for( int x = 0; x < tileSize + 1; x += step )
					count++;

			var cellIndices = new Vector2I[ count ];
			int current = 0;
			for( int y = 0; y < tileSize + 1; y += step )
				for( int x = 0; x < tileSize + 1; x += step )
					cellIndices[ current++ ] = tile.cellIndexMin + new Vector2I( x, y );
			if( current != cellIndices.Length )
				Log.Fatal( "Terrain: GenerateTileVerticesCellIndices: current != cellIndices.Length." );

			return cellIndices;
		}

		Vector3[] GenerateTileVertices( Tile tile, int lodLevel )
		{
			var cellIndices = GenerateTileVerticesCellIndices( tile, lodLevel );
			var vertices = new Vector3[ cellIndices.Length ];
			int current = 0;

			foreach( Vector2I cellIndex in cellIndices )
			{
				Vector2 pos = GetPositionXY( cellIndex );
				var height = GetHeight( cellIndex );
				vertices[ current++ ] = new Vector3( pos.X, pos.Y, height );
			}

			return vertices;
		}

		static ConcurrentDictionary<int, int[]> generateTileIndicesWithoutHolesCache = new ConcurrentDictionary<int, int[]>();

		int[] GenerateTileIndicesWithoutHoles( int lodLevel )
		{
			if( lodLevel >= cachedLodLevelCount )
				Log.Fatal( "Terrain: GenerateTileIndicesWithoutHoles: lodLevel >= cachedLodLevelCount." );

			//get from the cache
			if( lodLevel == 0 )
			{
				if( generateTileIndicesWithoutHolesCache.TryGetValue( tileSize, out var v ) )
					return v;
			}

			int step = 1 << lodLevel;
			int countX = tileSize / step;
			int countY = tileSize / step;

			var indices = new int[ countX * countY * 6 ];
			int current = 0;
			for( int y = 0; y < countY; y++ )
			{
				for( int x = 0; x < countX; x++ )
				{
					int sizeX = countX + 1;
					indices[ current++ ] = y * sizeX + x;
					indices[ current++ ] = y * sizeX + ( x + 1 );
					indices[ current++ ] = ( y + 1 ) * sizeX + ( x + 1 );
					indices[ current++ ] = ( y + 1 ) * sizeX + ( x + 1 );
					indices[ current++ ] = ( y + 1 ) * sizeX + x;
					indices[ current++ ] = y * sizeX + x;
				}
			}
			if( current != indices.Length )
				Log.Fatal( "Terrain: GenerateTileIndicesWithoutHoles: current != indices.Length." );

			//add to the cache
			if( lodLevel == 0 )
				generateTileIndicesWithoutHolesCache[ tileSize ] = indices;

			return indices;
		}

		public void GetLayersInfoByPosition( Vector2 position, List<LayersInfoByPositionItem> result )
		{
			var maskSize = GetPaintMaskSizeInteger();

			result.Clear();

			int remaining = 255;

			if( tiles != null && maskSize > 0 )
			{
				var maskIndex = GetMaskIndexByPosition( ref position );
				ClampMaskIndex( ref maskIndex );

				var tileIndex = GetTileIndexByMaskIndex( maskIndex );
				ClampTileIndex( ref tileIndex );

				var tile = tiles[ tileIndex.X, tileIndex.Y ];

				var layers = tile?.ObjectInSpace?.PaintLayersReplace;
				if( layers != null )
				{
					for( int n = layers.Length - 1; n >= 0; n-- )
					{
						ref var layer = ref layers[ n ];

						byte[] layerMask = null;
						{
							var data2 = layer.Mask?.Result?.GetData();
							if( data2 != null && data2.Length != 0 )
								layerMask = data2[ 0 ].Data.Array;
						}

						if( layerMask != null && layerMask.Length == maskSize * maskSize )
						{
							var maskValue = layerMask[ ( maskSize - 1 - maskIndex.Y ) * maskSize + maskIndex.X ];

							if( maskValue > 0 )
							{
								var min = Math.Min( maskValue, remaining );

								result.Add( new LayersInfoByPositionItem( layer.Layer, (byte)min ) );
								remaining -= min;

								if( remaining <= 0 )
									break;
							}
						}
					}
				}
			}

			if( remaining > 0 )
				result.Add( new LayersInfoByPositionItem( null, (byte)remaining ) );
		}

		public LayersInfoByPositionItem[] GetLayersInfoByPosition( Vector2 position )
		{
			var list = new List<LayersInfoByPositionItem>();
			GetLayersInfoByPosition( position, list );
			return list.ToArray();
		}

		public byte GetLayerInfoByPosition( Vector2 position, PaintLayer layerToCheck )
		{
			var maskSize = GetPaintMaskSizeInteger();

			int remaining = 255;

			if( tiles != null && maskSize > 0 )
			{
				var maskIndex = GetMaskIndexByPosition( ref position );
				ClampMaskIndex( ref maskIndex );

				var tileIndex = GetTileIndexByMaskIndex( maskIndex );
				ClampTileIndex( ref tileIndex );

				var tile = tiles[ tileIndex.X, tileIndex.Y ];

				var layers = tile?.ObjectInSpace?.PaintLayersReplace;
				if( layers != null )
				{
					for( int n = layers.Length - 1; n >= 0; n-- )
					{
						ref var layer = ref layers[ n ];

						byte[] layerMask = null;
						{
							var data2 = layer.Mask?.Result?.GetData();
							if( data2 != null && data2.Length != 0 )
								layerMask = data2[ 0 ].Data.Array;
						}

						if( layerMask != null && layerMask.Length == maskSize * maskSize )
						{
							var maskValue = layerMask[ ( maskSize - 1 - maskIndex.Y ) * maskSize + maskIndex.X ];

							if( maskValue > 0 )
							{
								var min = Math.Min( maskValue, remaining );

								if( layerToCheck != null && layer.Layer == layerToCheck )
									return (byte)min;

								if( layer.BlendMode != PaintLayer.BlendModeEnum.NoBlend )
									remaining -= min;

								if( remaining <= 0 )
									break;
							}
						}
					}
				}
			}

			if( remaining > 0 && layerToCheck == null )
				return (byte)remaining;

			return 0;
		}

		void CreateCollisionBodies()
		{
			DestroyCollisionBodies();

			if( !EnabledInHierarchyAndIsInstance )
				return;

			if( Collision )
			{
				foreach( var tile in GetTiles() )
					tile.UpdateCollisionBody();
			}
		}

		void DestroyCollisionBodies()
		{
			foreach( var tile in GetTiles() )
				tile.DestroyCollisionBody();
		}

		public int GetHeightmapSizeAsInteger()
		{
			return heightmapSize;
		}

		RectangleI GetCellIndexesForTileForNormalsTextures( Tile tile )
		{
			Vector2I min = tile.cellIndexMin - new Vector2I( 1, 1 );
			Vector2I max = tile.cellIndexMax + new Vector2I( 1, 1 );
			ClampCellIndex( ref min );
			ClampCellIndex( ref max );
			return new RectangleI( min, max );
		}

		int GetDefaultSizeForGeneratedTextures()
		{
			//this value must be less than TileSize.

			return 512;
		}

		public void SetNeedUpdateTilesByCellIndices( RectangleI cellRectangle, NeedUpdateEnum flags )
		{
			//LongOperationCallbackManager.CallCallback( "HeightmapTerrain: UpdateDataForRenderingForChangedHeights" );

			const int border = 2;
			Vector2I cellIndexMin = cellRectangle.Minimum - new Vector2I( border, border );
			Vector2I cellIndexMax = cellRectangle.Maximum + new Vector2I( border, border );
			ClampCellIndex( ref cellIndexMin );
			ClampCellIndex( ref cellIndexMax );

			Vector2I tileMin = GetTileIndexByCellIndex( cellIndexMin );
			Vector2I tileMax = GetTileIndexByCellIndex( cellIndexMax );
			ClampTileIndex( ref tileMin );
			ClampTileIndex( ref tileMax );

			GetTiles( tileMin, tileMax, delegate ( Tile tile )
			{
				tile.NeedUpdate |= flags;
			} );
		}

		public void SetNeedUpdateTilesByMaskIndices( RectangleI maskRectangle, NeedUpdateEnum flags )
		{
			const int border = 2;
			Vector2I maskIndexMin = maskRectangle.Minimum - new Vector2I( border, border );
			Vector2I maskIndexMax = maskRectangle.Maximum + new Vector2I( border, border );
			ClampMaskIndex( ref maskIndexMin );
			ClampMaskIndex( ref maskIndexMax );

			Vector2I tileMin = GetTileIndexByMaskIndex( maskIndexMin );
			Vector2I tileMax = GetTileIndexByMaskIndex( maskIndexMax );
			ClampTileIndex( ref tileMin );
			ClampTileIndex( ref tileMax );

			GetTiles( tileMin, tileMax, delegate ( Tile tile )
			{
				tile.NeedUpdate |= flags;
			} );
		}

		public void SetNeedUpdateAllTiles( NeedUpdateEnum flags )
		{
			GetTiles( delegate ( Tile tile )
			{
				tile.NeedUpdate |= flags;
			} );
		}

		public void UpdateTiles()
		{
			if( !allowUpdateTiles || !allowRecreateInternalData )
				return;

			try
			{
				allowUpdateTiles = false;

				var allowFullUpdate = AllowFullUpdateGeometryCollisionCurrentLayers;

				//update layers cache
				var layersUpdated = UpdateCurrentLayers();

				//need update all tiles when layers were updated
				if( layersUpdated && allowFullUpdate )
				{
					foreach( var tile in GetTiles() )
						tile.NeedUpdate |= NeedUpdateEnum.Layers;
				}


				//update marked tiles
				{
					//var groupOfObjects = GetOrCreateGroupOfObjects( false );
					//groupOfObjects?.BeginUpdate();

					foreach( var tile in GetTiles() )
					{
						if( ( tile.NeedUpdate & NeedUpdateEnum.Geometry ) != 0 )
						{
							if( tile.ObjectInSpace != null )
								tile.UpdateRenderingData( allowFullUpdate, null, allowFullUpdate, allowFullUpdate );
							tile.NeedUpdate &= ~NeedUpdateEnum.Geometry;

							//need update layers when geometry was updated
							tile.NeedUpdate |= NeedUpdateEnum.Layers;
						}

						if( ( tile.NeedUpdate & NeedUpdateEnum.Layers ) != 0 )
						{
							tile.UpdateLayers();
							tile.UpdateSurfaceObjectsData( null );
							tile.NeedUpdate &= ~NeedUpdateEnum.Layers;
						}

						if( allowFullUpdate && ( ( tile.NeedUpdate & NeedUpdateEnum.Collision ) != 0 ) )
						{
							if( tile.ObjectInSpace != null )
								tile.UpdateCollisionBody();
							tile.NeedUpdate &= ~NeedUpdateEnum.Collision;
						}
					}

					//groupOfObjects?.EndUpdate();
				}
			}
			finally
			{
				allowUpdateTiles = true;
			}
		}

		public Range GetHeightMinMaxWithoutPosition()
		{
			//!!!!когда еще обновлять heightMinMaxNeedUpdate

			if( heightMinMaxNeedUpdate && heightmapBuffer != null )
			{
				float min = float.MaxValue;
				float max = float.MinValue;

				try
				{
					int length = heightmapSize + 1;
					for( int y = 0; y < length; y++ )
					{
						for( int x = 0; x < length; x++ )
						{
							float height = heightmapBuffer[ x, y ];
							GetHeightWithoutPositionOverride?.Invoke( this, new Vector2I( x, y ), ref height );
							if( height < min )
								min = height;
							if( height > max )
								max = height;
						}
					}
				}
				catch { }

				heightMinMax = new Range( min, max );
				heightMinMaxNeedUpdate = false;
			}

			return heightMinMax;
		}

		public Range GetHeightMinMax()
		{
			Range range = GetHeightMinMaxWithoutPosition();
			var position = Position.Value;
			return new Range( position.Z + range.Minimum, position.Z + range.Maximum );
		}

		public Rectangle GetBounds2()
		{
			var position = Position.Value;
			var horizontalSize = HorizontalSize.Value;
			return new Rectangle(
				new Vector2( position.X - horizontalSize * .5f, position.Y - horizontalSize * .5f ),
				new Vector2( position.X + horizontalSize * .5f, position.Y + horizontalSize * .5f ) );
		}

		public Bounds CalculateBounds()
		{
			//!!!!есть в cachedBounds

			var position = Position.Value;
			var horizontalSize = HorizontalSize.Value;
			return new Bounds(
				new Vector3( position.X - horizontalSize * .5f, position.Y - horizontalSize * .5f, GetHeightMinMax().Minimum ),
				new Vector3( position.X + horizontalSize * .5f, position.Y + horizontalSize * .5f, GetHeightMinMax().Maximum ) );
		}

		void RecreateInternalData( bool updateHoles = false )
		{
			if( !allowRecreateInternalData )
				return;
			try
			{
				allowRecreateInternalData = false;

				CreateTiles( updateHoles );
				CreateCollisionBodies();
			}
			finally
			{
				allowRecreateInternalData = true;
			}
		}

		public void UpdateRenderingAndCollisionData( bool updateHoles = false )
		{
			RecreateInternalData( updateHoles );
		}

		void SetCollisionMaterial()
		{
			foreach( var tile in GetTiles() )
				tile.SetCollisionMaterial();
		}

		[Browsable( false )]
		[Cloneable( CloneType.Disable )]//in the OnClone CloneType.Deep )]//!!!!так? или копию делать в set
		public float[,] HeightmapBuffer
		{
			get { return heightmapBuffer; }
			set
			{
				if( heightmapBuffer == value )
					return;
				heightmapBuffer = value;

				SetNeedUpdateAllTiles( NeedUpdateEnum.All );
			}
		}

		[Browsable( false )]
		public float[,] HeightmapSize_UndoDependentData
		{
			get { return heightmapBuffer; }
			set
			{
				heightmapSize_UndoDependentData = value;
			}
		}

		public bool GetPositionByRay( Ray ray, bool considerHoles, out Vector3 position )
		{
			var p = Vector3.Zero;
			bool intersected = false;

			GetTiles( ray, delegate ( Tile tile )
			{
				if( tile.Vertices != null && tile.Bounds != null && tile.Bounds.BoundingBox.Intersects( ray ) )
				{
					intersected = FindMeshRayIntersection( ray, tile.Vertices, tile.Indices, out p );

					if( intersected )
						return false;

					////!!!!может кешировать данные в тайле

					//var vertices = GenerateTileVertices( tile, 0 );

					//IList<int> indices;
					//if( considerHoles && tile.existsHoles )
					//	indices = GenerateTileIndicesWithHoles( tile.cellIndexMin );
					//else
					//	indices = GenerateTileIndicesWithoutHoles( 0 );

					////!!!!double
					//intersected = FindMeshRayIntersection( ray, vertices, indices, out p );

					//if( intersected )
					//	return false;
				}
				return true;
			} );

			position = p;
			return intersected;
		}

		public static bool GetTerrainByRay( Scene scene, Ray ray, out Terrain terrain, out Vector3 position )
		{
			var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, ray );
			scene.GetObjectsInSpace( item );
			if( item.Result != null )
			{
				foreach( var resultItem in item.Result )
				{
					var tile = resultItem.Object.AnyData as Tile;
					if( tile != null )
					{
						var terrain2 = tile.owner;

						if( tile.Vertices != null && tile.Bounds != null && tile.Bounds.BoundingBox.Intersects( ray ) )
						{
							////!!!!может кешировать данные в тайле

							//var vertices = ter.GenerateTileVertices( tile, 0 );

							//IList<int> indices;
							//if( considerHoles && tile.existsHoles )
							//	indices = GenerateTileIndicesWithHoles( tile.cellIndexMin );
							//else
							//	indices = GenerateTileIndicesWithoutHoles( 0 );

							if( terrain2.FindMeshRayIntersection( ray, tile.Vertices, tile.Indices, out var p ) )
							{
								terrain = terrain2;
								position = p;
								return true;
							}
						}
					}
				}
			}

			terrain = null;
			position = Vector3.Zero;
			return false;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			UpdateTiles();
			//CheckForUpdatePaintLayers();
		}

		RenderingPipeline.RenderSceneData.LayerItem[] CalculateLayers()
		{
			var items = new List<RenderingPipeline.RenderSceneData.LayerItem>();

			foreach( var layer in GetComponents<PaintLayer>() )
			{
				if( layer.Enabled )
				{
					var mask = layer.GetMaskImage( out var uniqueMaskDataCounter );
					if( mask != null )
					{
						//calculate ResultMaterial later in UpdateCurrentLayers()
						items.Add( new RenderingPipeline.RenderSceneData.LayerItem( layer, mask, uniqueMaskDataCounter, false ) );
					}
				}
			}

			return items.ToArray();
		}

		bool UpdateCurrentLayers()
		{
			var updated = false;

			var newList = CalculateLayers();

			if( !newList.SequenceEqual( currentLayersCache ) )
			{
				updated = true;

				currentLayersCache = newList;

				//calculate ResultMaterial
				for( int n = 0; n < currentLayersCache.Length; n++ )
				{
					ref var item = ref currentLayersCache[ n ];
					item.CalculateResultMaterial();
				}
			}

			return updated;
		}

		public List<MeshInSpace> GetHoleObjects()
		{
			var holes = new List<MeshInSpace>( Holes.Count );
			for( int n = 0; n < Holes.Count; n++ )
			{
				var component = Holes[ n ].Value;
				if( component != null )
					holes.Add( component );
			}
			return holes;
		}

		void HolesCacheSetItem( Vector2I tileIndex, Vector3[] vertices, int[] indices )
		{
			if( vertices != null )
			{
				var item = new HoleCacheItem();
				item.vertices = vertices;
				item.indices = indices;
				holesCache[ tileIndex ] = item;
			}
			else
				holesCache.Remove( tileIndex );
		}

		bool HolesCacheGetItem( Vector2I tileIndex, out Vector3[] vertices, out int[] indices )
		{
			if( holesCache.TryGetValue( tileIndex, out var item ) )
			{
				vertices = item.vertices;
				indices = item.indices;
				return true;
			}
			else
			{
				vertices = null;
				indices = null;
				return false;
			}
		}

		void HolesCacheClear()
		{
			holesCache.Clear();
		}

		static int ParseTileSize( TileSizeEnum enumValue )
		{
			var str = enumValue.ToString();
			var index = str.IndexOf( "x" );
			var str2 = str.Substring( 1, index - 1 );
			return int.Parse( str2 );
		}

		protected override void OnClone( Metadata.CloneContext context, Component newObject )
		{
			base.OnClone( context, newObject );

			var newTerrain = (Terrain)newObject;
			if( heightmapBuffer != null )
				newTerrain.heightmapBuffer = (float[,])heightmapBuffer.Clone();
		}

		public Bounds GetBoundsFromTiles()
		{
			var result = Bounds.Cleared;

			GetTiles( delegate ( Tile tile )
			{
				if( tile.Bounds != null )
					result.Add( tile.Bounds.BoundingBox );
			} );

			return result;
		}

		public delegate void GetGeometryFromTilesCallbackDelegate( SpaceBounds tileBounds, Vector3[] tileVertices, int[] tileIndices );

		public void GetGeometryFromTiles( GetGeometryFromTilesCallbackDelegate callback )
		{
			GetTiles( delegate ( Tile tile )
			{
				if( tile.Vertices != null && tile.Bounds != null )
					callback( tile.Bounds, tile.Vertices, tile.Indices );
			} );
		}

		public static Terrain GetTerrainByMeshInSpace( MeshInSpace meshInSpace )
		{
			return meshInSpace.Parent?.Parent as Terrain;
		}


		///////////////////////////////////////////////

		struct SurfaceObjectsLocalItem
		{
			public int added;
			public float cameraDistanceMinSquared;
			public float cameraDistanceMin;
			public float cameraDistanceMaxSquared;
			public int onlyForShadowGeneration;
			public float boundingSize;
			public float visibilityDistance;
			public SceneLODUtility.LodState lodState;
		}

		struct LodItem
		{
			public OpenListNative<ObjectItem> List;
			public float MaxVisibilityDistance;
			public int MeshesListIndex;
			public int InstancingStart;

			//

			public struct ObjectItem
			{
				public int ObjectIndex;
				public float LODValue;
				public float VisibilityDistance;
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void RenderObjectsDynamicBatched( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem, Tile.SurfaceObjectsItem surfaceObjectsItem, Tile.SurfaceObjectsRenderGroup group )
		{
			//!!!!те что по одному не инстансить. а может и больше чем 1

			var surface = surfaceObjectsItem.GetSurface().Result;
			if( surface == null )
				return;
			surfaceObjectsItem.GetObjectsColor( out var color );
			RenderingPipeline.GetColorForInstancingData( ref color, out var colorForInstancingData1, out var colorForInstancingData2 );
			//var colorForInstancingData = RenderingPipeline.GetColorForInstancingData( ref color );
			var surfaceObjectsVisibilityDistanceFactor = surfaceObjectsItem.GetObjectsVisibilityDistanceFactor();
			var surfaceObjectsCastShadows = surfaceObjectsItem.GetObjectsCastShadows();

			var cameraSettings = context.Owner.CameraSettings;
			var objectCount = group.Objects.Count;


			surface.GetMesh( group.VariationGroup, group.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out var visibilityDistanceFactor, out var castShadows, out var receiveDecals, out var motionBlurFactor );
			castShadows = castShadows && surfaceObjectsCastShadows;

			if( mesh?.Result == null )
				mesh = ResourceUtility.MeshInvalid;
			if( !enabled )
				return;
			if( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && !castShadows )
				return;

			//touch mesh result before multithreading
			var meshResult = mesh.Result;
			var meshDataLOD0 = meshResult.MeshData;
			var meshDataLods = meshDataLOD0.LODs;

			SurfaceObjectsLocalItem* localItems = (SurfaceObjectsLocalItem*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, sizeof( SurfaceObjectsLocalItem ) * objectCount );
			for( int n = 0; n < objectCount; n++ )
				localItems[ n ].added = 0;

			try
			{
				Parallel.For( 0, objectCount, delegate ( int nObject )
				{
					ref var objectItem = ref group.Objects.Data[ nObject ];

					var add = false;
					var onlyForShadowGeneration = false;
					if( mode == GetRenderSceneDataMode.InsideFrustum )
					{
						add = true;
						onlyForShadowGeneration = !cameraSettings.Frustum.Intersects( ref group.BoundingSphere );
					}
					else if( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && modeGetObjectsItem.Intersects( ref group.BoundingBox ) )
					{
						add = true;
						onlyForShadowGeneration = true;
					}

					if( !add )
						return;
					if( onlyForShadowGeneration && !castShadows )
						return;

					ref var localItem = ref localItems[ nObject ];

					var cameraDistanceMinSquared = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, ref objectItem.BoundingBox );

					var boundingSize = (float)( meshResult.SpaceBounds.boundingSphere.Radius * 2 * group.ObjectsMaxScale );
					//var boundingSize = (float)( meshResult.SpaceBounds.boundingSphere.Radius * 2 * objectItem.Scale.MaxComponent() );
					var visibilityDistance = context.GetVisibilityDistanceByObjectSize( boundingSize ) * visibilityDistanceFactor * meshDataLOD0.VisibilityDistanceFactor;

					if( cameraDistanceMinSquared < visibilityDistance * visibilityDistance )
					{
						if( onlyForShadowGeneration )
						{
							if( !castShadows || cameraDistanceMinSquared > context.GetShadowVisibilityDistanceSquared( visibilityDistance ) )
								return;
						}

						var cameraDistanceMin = MathEx.Sqrt( cameraDistanceMinSquared );
						var cameraDistanceMaxSquared = SceneLODUtility.GetCameraDistanceMax( cameraSettings, ref objectItem.BoundingSphere );
						cameraDistanceMaxSquared *= cameraDistanceMaxSquared;

						localItem.added = 1;
						//some are not used
						localItem.cameraDistanceMinSquared = cameraDistanceMinSquared;
						localItem.cameraDistanceMin = cameraDistanceMin;
						localItem.cameraDistanceMaxSquared = cameraDistanceMaxSquared;
						localItem.onlyForShadowGeneration = onlyForShadowGeneration ? 1 : 0;
						localItem.boundingSize = boundingSize;
						localItem.visibilityDistance = visibilityDistance;

						SceneLODUtility.GetDemandedLODs( context, meshDataLOD0, cameraDistanceMinSquared, cameraDistanceMaxSquared, localItem.boundingSize, out localItem.lodState );
					}
				} );

				int maxLods = 1;
				if( meshDataLOD0.LODs != null )
					maxLods += meshDataLOD0.LODs.Length;

				var allLodItems = new LodItem[ maxLods * 2 ];
				var allLodItemsFound = false;

				for( int nObject = 0; nObject < objectCount; nObject++ )
				{
					ref var localItem = ref localItems[ nObject ];
					if( localItem.added == 0 )
						continue;

					var onlyForShadowGeneration = localItem.onlyForShadowGeneration != 0;

					for( int nLodItem = 0; nLodItem < localItem.lodState.Count; nLodItem++ )
					{
						localItem.lodState.GetItem( nLodItem, out var lodLevel, out var lodRange );

						var meshItemIndex = lodLevel * 2 + ( onlyForShadowGeneration ? 1 : 0 );
						ref var lodItem = ref allLodItems[ meshItemIndex ];

						//!!!!GC little bit
						if( lodItem.List == null )
							lodItem.List = new OpenListNative<LodItem.ObjectItem>( objectCount );

						var lodObjectItem = new LodItem.ObjectItem();
						lodObjectItem.ObjectIndex = nObject;
						//!!!!threading. может потом посчитать
						lodObjectItem.LODValue = SceneLODUtility.GetLodValue( context, lodRange, localItem.cameraDistanceMin );
						lodObjectItem.VisibilityDistance = localItem.visibilityDistance;

						lodItem.List.Add( ref lodObjectItem );

						if( lodObjectItem.VisibilityDistance > lodItem.MaxVisibilityDistance )
							lodItem.MaxVisibilityDistance = lodObjectItem.VisibilityDistance;

						allLodItemsFound = true;
					}
				}

				if( !allLodItemsFound )
					return;

				var templateItem = new RenderingPipeline.RenderSceneData.MeshItem();
				templateItem.Creator = this;
				templateItem.ReceiveDecals = true;
				templateItem.MotionBlurFactor = 1.0f;
				templateItem.ReplaceMaterial = replaceMaterial;
				templateItem.Color = ColorValue.One;
				templateItem.ColorForInstancingData1 = RenderingPipeline.ColorOneForInstancingData1;
				templateItem.ColorForInstancingData2 = RenderingPipeline.ColorOneForInstancingData2;
				templateItem.StaticShadows = true;

				var outputMeshes = context.FrameData.RenderSceneData.Meshes;

				var totalInstanceCount = 0;

				//MeshDataLastVoxelLOD
				if( meshDataLods != null )
				{
					var lastLOD = meshDataLods[ meshDataLods.Length - 1 ];
					if( lastLOD.VoxelGridSize != 0 )
						templateItem.MeshDataLastVoxelLOD = lastLOD.Mesh?.Result?.MeshData;
				}

				//add mesh items and save indexes
				for( int meshItemIndex = 0; meshItemIndex < allLodItems.Length; meshItemIndex++ )
				{
					ref var lodItem = ref allLodItems[ meshItemIndex ];

					if( lodItem.List != null )
					{
						//add mesh item. with template data
						outputMeshes.Add( ref templateItem );
						lodItem.MeshesListIndex = outputMeshes.Count - 1;

						ref var item = ref outputMeshes.Data[ lodItem.MeshesListIndex ];

						var lodLevel = meshItemIndex / 2;

						item.MeshData = meshDataLOD0;
						item.MeshDataLOD0 = meshDataLOD0;

						//MeshDataShadows, MeshDataShadowsForceBestLOD
						var lodScaleShadows = context.LODScaleShadowsSquared * meshDataLOD0.LODScaleShadows;
						if( lodScaleShadows <= lodLevel )
						{
							//select last LOD for shadows
							item.MeshDataShadows = item.MeshDataLastVoxelLOD;
						}
						else if( lodScaleShadows >= 100 )
						{
							//select the best LOD for shadows
							item.MeshDataShadows = meshDataLOD0;
							item.MeshDataShadowsForceBestLOD = true;
						}

						if( lodLevel > 0 )
						{
							ref var lod = ref meshDataLods[ lodLevel - 1 ];
							//!!!!can't thread
							var lodMeshData = lod.Mesh?.Result?.MeshData;
							if( lodMeshData != null )
								item.MeshData = lodMeshData;
						}

						//!!!!need uncomment. need split lodItems then same as onlyForShadowGeneration
						//!!!!!or maybe no big sense to do it because all of this is near to camera
						item.CastShadows = castShadows && item.MeshData.CastShadows;// && cameraDistanceMinSquared < context.GetShadowVisibilityDistanceSquared( visibilityDistance );


						//!!!!slower with sorting in some cases

						////sort by distance
						//CollectionUtility.MergeSortUnmanaged( lodItem.List.Data, lodItem.List.Count, delegate ( LodItem.ObjectItem* a, LodItem.ObjectItem* b )
						//{
						//	ref var localItemA = ref localItems[ a->ObjectIndex ];
						//	ref var localItemB = ref localItems[ b->ObjectIndex ];
						//	if( localItemA.cameraDistanceMin < localItemB.cameraDistanceMin )
						//		return -1;
						//	if( localItemA.cameraDistanceMin > localItemB.cameraDistanceMin )
						//		return 1;
						//	return 0;
						//}, true );

						//tessellation
						if( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum || mode == GetRenderSceneDataMode.InsideFrustum )
						{
							var enable = false;

							if( item.MeshData == meshDataLOD0 )
							{
								if( item.ReplaceMaterial != null )
								{
									if( item.ReplaceMaterial.Result != null )
										enable = item.ReplaceMaterial.Result.tessellationQuality != 0;
								}
								else
									enable = meshDataLOD0.ContainsTessellation;

								//if( item.ReplaceMaterialSelectively != null )
								//{
								//	for( int n = 0; n < item.ReplaceMaterialSelectively.Length; n++ )
								//	{
								//		var result = item.ReplaceMaterialSelectively[ n ].Result;
								//		if( result != null && result.tessellationQuality != 0 )
								//			enable = true;
								//	}
								//}
							}

							//set true only for first lod, set false to second (it can be true because item memory is shared)
							item.Tessellation = enable;
						}

						lodItem.InstancingStart = totalInstanceCount;
						totalInstanceCount += lodItem.List.Count;
					}
					else
						lodItem.MeshesListIndex = -1;
				}

				if( totalInstanceCount == 0 )
					return;


				//allocate instancing buffer data
				int instanceStride = sizeof( RenderingPipeline.RenderSceneData.ObjectInstanceData );
				var instanceBuffer = new InstanceDataBuffer( totalInstanceCount, instanceStride );
				if( !instanceBuffer.Valid )
					return;
				var instancingData = (RenderingPipeline.RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;


				Parallel.For( 0, allLodItems.Length, delegate ( int meshItemIndex )
				{
					ref var lodItem = ref allLodItems[ meshItemIndex ];

					if( lodItem.List != null )
					{
						var lodLevel = meshItemIndex / 2;
						var onlyForShadowGeneration = ( meshItemIndex % 2 ) == 1;
						var instanceCount = lodItem.List.Count;

						ref var item = ref outputMeshes.Data[ lodItem.MeshesListIndex ];

						//!!!!нужно собрать из тех что есть в списке? или ничего не делать, это же только сфера
						item.BoundingSphere = group.BoundingSphere;
						//item.BoundingBoxCenter = item.BoundingSphere.Center;
						//item.BoundingSphere = objectItem.BoundingSphere;
						//item.BoundingBoxCenter = item.BoundingSphere.Center;//objectItem.BoundingBox.GetCenter( out item.BoundingBoxCenter );

						item.VisibilityDistance = lodItem.MaxVisibilityDistance;
						item.OnlyForShadowGeneration = onlyForShadowGeneration;

						var objectsMaxScale = 0.0;

						//!!!!maybe parallel. blocks of items

						int currentIndex = lodItem.InstancingStart;

						for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
						{
							ref var lodObjectItem = ref lodItem.List.Data[ nRenderableItem ];
							var nObject = lodObjectItem.ObjectIndex;

							ref var objectItem = ref group.Objects.Data[ nObject ];
							//!!!!
							ref var obj = ref objectItem;
							//ref var obj = ref Objects[ objectItem.ObjectIndex ];

							ref var data = ref instancingData[ currentIndex++ ];

							if( item.MeshData.BillboardMode != 0 )
							{
								var scaleH = Math.Max( obj.Scale.X, obj.Scale.Y );
								var scaleV = obj.Scale.Z;

								Vector3F offset;
								if( item.MeshData.BillboardPositionOffset != Vector3F.Zero )
									offset = obj.Rotation * ( item.MeshData.BillboardPositionOffset * Math.Max( scaleH, scaleV ) );
								else
									offset = Vector3F.Zero;

								var itemTransform = new Matrix4F();
								itemTransform.Item0.X = scaleH;
								itemTransform.Item0.Y = 0;
								itemTransform.Item0.Z = obj.Rotation.X;
								itemTransform.Item0.W = 0;
								itemTransform.Item1.X = obj.Rotation.Y;
								itemTransform.Item1.Y = scaleH;
								itemTransform.Item1.Z = obj.Rotation.Z;
								itemTransform.Item1.W = 0;
								itemTransform.Item2.X = obj.Rotation.W;
								itemTransform.Item2.Y = 0;
								itemTransform.Item2.Z = obj.Scale.Z;
								itemTransform.Item2.W = 0;

								context.ConvertToRelative( obj.Position + offset, out var positionRelative );
								itemTransform.Item3.X = positionRelative.X;
								itemTransform.Item3.Y = positionRelative.Y;
								itemTransform.Item3.Z = positionRelative.Z;

								itemTransform.Item3.W = 1;

								//!!!!slowly
								itemTransform.GetTranspose( out data.TransformRelative );

								//data.TransformRelative.GetTranslation( out data.PositionPreviousFrameRelative );
								//data.PositionPreviousFrameRelative += context.OwnerCameraSettingsPositionPreviousChange;
								////data.PositionPreviousFrameRelative = itemTransform.Item3.ToVector3F();
							}
							else
							{
								context.ConvertToRelative( ref obj.Position, out var positionRelative );
								Matrix3x4F.ConstructTransposeMatrix3( ref objectItem.MeshMatrix3, ref positionRelative, out data.TransformRelative );

								//data.PositionPreviousFrameRelative = positionRelative + context.OwnerCameraSettingsPositionPreviousChange;


								//objectItem.MeshMatrix.GetTranspose( out data.Transform );

								////!!!!double
								////var pos = obj.Position.ToVector3F();
								////var matrix = new Matrix4F( ref objectItem.MeshMatrix3, ref pos );

								//////!!!!slowly?
								////matrix.GetTranspose( out data.Transform );

								////!!!!double
								//data.PositionPreviousFrame = obj.Position.ToVector3F();
								//////matrix.GetTranslation( out data.PositionPreviousFrame );
							}

							data.PreviousFramePositionChange = Vector3F.Zero;

							data.ColorPackedValue1 = colorForInstancingData1;// objectItem.ColorForInstancingData;
							data.ColorPackedValue2 = colorForInstancingData2;
							data.LodValue = lodObjectItem.LODValue;
							data.VisibilityDistance = lodObjectItem.VisibilityDistance;
							//!!!!
							data.ReceiveDecals = receiveDecals ? (byte)255 : (byte)0;
							data.MotionBlurFactor = (byte)( motionBlurFactor * 255 );
							//data.ReceiveDecals = receiveDecals ? 1.0f : 0.0f;
							//data.MotionBlurFactor = motionBlurFactor;
							//data.CullingByCameraDirectionData = 0;

							var maxScale = obj.Scale.MaxComponent();
							if( maxScale > objectsMaxScale )
								objectsMaxScale = maxScale;
						}


						//!!!!?
						//if( renderableItem.X == 0 )

						//int currentMatrix = 0;
						//for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
						//{
						//	var renderableItem = outputItem.renderableItems[ nRenderableItem ];

						//	if( renderableItem.X == 0 )
						//	{
						//		//meshes
						//		ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
						//		//!!!!slowly because no threading? where else
						//		meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
						//	}
						//	else if( renderableItem.X == 1 )
						//	{
						//		//billboards
						//		ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
						//		billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
						//	}
						//}

						item.InstancingEnabled = true;
						item.InstancingDataBuffer = instanceBuffer;
						item.InstancingStart = lodItem.InstancingStart;
						item.InstancingCount = instanceCount;
						item.InstancingMaxLocalBounds = (float)( objectsMaxScale * item.MeshData.SpaceBounds.boundingSphere.Radius );
					}
				} );

				for( int meshItemIndex = 0; meshItemIndex < allLodItems.Length; meshItemIndex++ )
				{
					ref var lodItem = ref allLodItems[ meshItemIndex ];
					lodItem.List?.Dispose();
				}
			}
			finally
			{
				NativeUtility.Free( localItems );
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void RenderObjectsNoBatchingGroup( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem, Tile.SurfaceObjectsItem surfaceObjectsItem, Tile.SurfaceObjectsRenderGroup group )
		{
			var surface = surfaceObjectsItem.GetSurface().Result;
			if( surface == null )
				return;
			surfaceObjectsItem.GetObjectsColor( out var color );
			var surfaceObjectsVisibilityDistanceFactor = surfaceObjectsItem.GetObjectsVisibilityDistanceFactor();
			var surfaceObjectsCastShadows = surfaceObjectsItem.GetObjectsCastShadows();

			var cameraSettings = context.Owner.CameraSettings;
			var objectCount = group.Objects.Count;

			SurfaceObjectsLocalItem* localItems = (SurfaceObjectsLocalItem*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, sizeof( SurfaceObjectsLocalItem ) * objectCount );
			for( int n = 0; n < objectCount; n++ )
				localItems[ n ].added = 0;

			try
			{
				Parallel.For( 0, objectCount, delegate ( int nObject )
				{
					ref var objectItem = ref group.Objects.Data[ nObject ];

					var add = false;
					var onlyForShadowGeneration = false;
					if( mode == GetRenderSceneDataMode.InsideFrustum )
					{
						add = true;
						onlyForShadowGeneration = !cameraSettings.Frustum.Intersects( ref group.BoundingSphere );
					}
					else if( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && modeGetObjectsItem.Intersects( ref group.BoundingBox ) )
					{
						add = true;
						onlyForShadowGeneration = true;
					}

					if( add )
					{
						ref var localItem = ref localItems[ nObject ];

						var cameraDistanceMinSquared = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, ref objectItem.BoundingBox );
						var cameraDistanceMin = MathEx.Sqrt( cameraDistanceMinSquared );
						var cameraDistanceMaxSquared = SceneLODUtility.GetCameraDistanceMax( cameraSettings, ref objectItem.BoundingSphere );
						cameraDistanceMaxSquared *= cameraDistanceMaxSquared;

						localItem.added = 1;
						localItem.cameraDistanceMinSquared = cameraDistanceMinSquared;
						localItem.cameraDistanceMin = cameraDistanceMin;
						localItem.cameraDistanceMaxSquared = cameraDistanceMaxSquared;
						localItem.onlyForShadowGeneration = onlyForShadowGeneration ? 1 : 0;
					}
				} );

				for( int nObject = 0; nObject < objectCount; nObject++ )
				{
					ref var localItem = ref localItems[ nObject ];
					if( localItem.added == 0 )
						continue;
					var onlyForShadowGeneration = localItem.onlyForShadowGeneration != 0;

					ref var objectItem = ref group.Objects.Data[ nObject ];

					//!!!!что-то в Parallel. GetMesh нужно предрасчитать заранее

					surface.GetMesh( group.VariationGroup, group.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out var visibilityDistanceFactor, out var castShadows, out var receiveDecals, out var motionBlurFactor );
					if( mesh?.Result == null )
						mesh = ResourceUtility.MeshInvalid;

					if( !enabled )
						continue;
					if( onlyForShadowGeneration && ( !castShadows || !surfaceObjectsCastShadows ) )
						continue;

					var meshDataLOD0 = mesh.Result.MeshData;

					var boundingSize = (float)( mesh.Result.SpaceBounds.boundingSphere.Radius * 2 * objectItem.Scale.MaxComponent() );
					var visibilityDistance = context.GetVisibilityDistanceByObjectSize( boundingSize ) * visibilityDistanceFactor * meshDataLOD0.VisibilityDistanceFactor * surfaceObjectsVisibilityDistanceFactor;

					if( localItem.cameraDistanceMinSquared < visibilityDistance * visibilityDistance )
					{
						ref var position = ref objectItem.Position;
						ref var rotation = ref objectItem.Rotation;
						ref var scale = ref objectItem.Scale;

						var item = new RenderingPipeline.RenderSceneData.MeshItem();
						item.Creator = this;
						item.BoundingSphere = objectItem.BoundingSphere;
						//item.MeshData = mesh.Result.MeshData;
						//!!!!выключать тени. где еще так. что еще помимо теней
						//item.CastShadows = castShadows && item.MeshData.CastShadows && localItem.cameraDistanceMinSquared < context.GetShadowVisibilityDistanceSquared( visibilityDistance ); ;
						item.ReceiveDecals = receiveDecals;
						item.MotionBlurFactor = motionBlurFactor;
						item.ReplaceMaterial = replaceMaterial;
						item.Color = color;// obj.Color;
						RenderingPipeline.GetColorForInstancingData( ref item.Color, out item.ColorForInstancingData1, out item.ColorForInstancingData2 );
						item.VisibilityDistance = visibilityDistance;
						item.OnlyForShadowGeneration = onlyForShadowGeneration;
						item.StaticShadows = true;

						if( onlyForShadowGeneration && ( !item.CastShadows || !surfaceObjectsCastShadows ) )
							continue;

						int item0BillboardMode = 0;

						//MeshDataLastVoxelLOD
						var meshDataLods = meshDataLOD0.LODs;
						if( meshDataLods != null )
						{
							var lastLOD = meshDataLods[ meshDataLods.Length - 1 ];
							if( lastLOD.VoxelGridSize != 0 )
								item.MeshDataLastVoxelLOD = lastLOD.Mesh?.Result?.MeshData;
						}

						SceneLODUtility.GetDemandedLODs( context, meshDataLOD0, localItem.cameraDistanceMinSquared, localItem.cameraDistanceMaxSquared, boundingSize, out var lodState );
						for( int nLodItem = 0; nLodItem < lodState.Count; nLodItem++ )
						{
							lodState.GetItem( nLodItem, out var lodLevel, out var lodRange );

							item.MeshData = meshDataLOD0;
							item.MeshDataLOD0 = meshDataLOD0;

							//MeshDataShadows, MeshDataShadowsForceBestLOD
							var lodScaleShadows = context.LODScaleShadowsSquared * meshDataLOD0.LODScaleShadows;
							if( lodScaleShadows <= lodLevel )
							{
								//select last LOD for shadows
								item.MeshDataShadows = item.MeshDataLastVoxelLOD;
							}
							else if( lodScaleShadows >= 100 )
							{
								//select the best LOD for shadows
								item.MeshDataShadows = meshDataLOD0;
								item.MeshDataShadowsForceBestLOD = true;
							}

							if( lodLevel > 0 )
							{
								ref var lod = ref meshDataLods[ lodLevel - 1 ];
								var lodMeshData = lod.Mesh?.Result?.MeshData;
								if( lodMeshData != null )
									item.MeshData = lodMeshData;
							}

							item.CastShadows = castShadows && surfaceObjectsCastShadows && item.MeshData.CastShadows && localItem.cameraDistanceMinSquared < context.GetShadowVisibilityDistanceSquared( visibilityDistance );
							item.LODValue = SceneLODUtility.GetLodValue( context, lodRange, localItem.cameraDistanceMin );

							//calculate MeshInstanceOne, PositionPreviousFrame
							if( nLodItem == 0 )
								item0BillboardMode = item.MeshData.BillboardMode;
							if( nLodItem == 0 || item0BillboardMode != item.MeshData.BillboardMode )
							{
								if( item.MeshData.BillboardMode != 0 )
								{
									var scaleH = Math.Max( scale.X, scale.Y );
									var scaleV = scale.Z;

									Vector3F offset;
									if( item.MeshData.BillboardPositionOffset != Vector3F.Zero )
										offset = rotation * ( item.MeshData.BillboardPositionOffset * Math.Max( scaleH, scaleV ) );
									else
										offset = Vector3F.Zero;

									ref var result = ref item.TransformRelative;
									result.Item0.X = scaleH;
									result.Item0.Y = 0;
									result.Item0.Z = rotation.X;
									result.Item0.W = 0;
									result.Item1.X = rotation.Y;
									result.Item1.Y = scaleH;
									result.Item1.Z = rotation.Z;
									result.Item1.W = 0;
									result.Item2.X = rotation.W;
									result.Item2.Y = 0;
									result.Item2.Z = scale.Z;
									result.Item2.W = 0;
									result.Item3.X = (float)( position.X - context.OwnerCameraSettingsPosition.X + offset.X );
									result.Item3.Y = (float)( position.Y - context.OwnerCameraSettingsPosition.Y + offset.Y );
									result.Item3.Z = (float)( position.Z - context.OwnerCameraSettingsPosition.Z + offset.Z );
									result.Item3.W = 1;
								}
								else
								{
									context.ConvertToRelative( ref objectItem.Position, out var positionRelative );
									item.TransformRelative = new Matrix4F( ref objectItem.MeshMatrix3, ref positionRelative );
									//item.Transform = objectItem.MeshMatrix;
								}

								////PositionPreviousFrame
								//item.TransformRelative.GetTranslation( out item.PositionPreviousFrameRelative );
								//item.PositionPreviousFrameRelative += context.OwnerCameraSettingsPositionPreviousChange;
							}

							//tessellation
							if( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum || mode == GetRenderSceneDataMode.InsideFrustum )
							{
								var enable = false;

								if( item.MeshData == meshDataLOD0 )
								{
									if( item.ReplaceMaterial != null )
									{
										if( item.ReplaceMaterial.Result != null )
											enable = item.ReplaceMaterial.Result.tessellationQuality != 0;
									}
									else
										enable = meshDataLOD0.ContainsTessellation;

									//if( item.ReplaceMaterialSelectively != null )
									//{
									//	for( int n = 0; n < item.ReplaceMaterialSelectively.Length; n++ )
									//	{
									//		var result = item.ReplaceMaterialSelectively[ n ].Result;
									//		if( result != null && result.tessellationQuality != 0 )
									//			enable = true;
									//	}
									//}
								}

								//set true only for first lod, set false to second (it can be true because item memory is shared)
								item.Tessellation = enable;
							}

							//add to render
							{
								////set AnimationData from event
								//GetRenderSceneDataAddToFrameData?.Invoke( this, context, mode, ref item );

								context.FrameData.RenderSceneData.Meshes.Add( ref item );
							}
						}
					}
				}
			}
			finally
			{
				NativeUtility.Free( localItems );
			}
		}

		//!!!!impl
		//static void SurfaceObjectsPrepareRenderGroupsTask( object data )
		//{
		//	var surfaceObjectsItem = (Tile.SurfaceObjectsItem)data;

		//	//!!!!как для теней?

		//	surfaceObjectsItem.RenderGroupsTaskResult = Tile.CalculateSurfaceObjectsRenderGroups( surfaceObjectsItem );
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void CreateBatchingInstanceBufferMesh( Tile.SurfaceObjectsItem surfaceObjectsItem, Tile.SurfaceObjectsRenderGroup batch, float lodValue/*, float visibilityDistance*/, bool receiveDecals, float motionBlurFactor )
		{
			surfaceObjectsItem.GetObjectsColor( out var color );

			var instancePositionOffset = new Vector3( double.MaxValue, double.MaxValue, double.MaxValue );
			for( int n = 0; n < batch.Objects.Count; n++ )
			{
				ref var objectItem = ref batch.Objects.Data[ n ];
				instancePositionOffset.X = Math.Min( instancePositionOffset.X, objectItem.Position.X );
				instancePositionOffset.Y = Math.Min( instancePositionOffset.Y, objectItem.Position.Y );
				instancePositionOffset.Z = Math.Min( instancePositionOffset.Z, objectItem.Position.Z );
			}

			var vertices = new byte[ sizeof( RenderingPipeline.RenderSceneData.ObjectInstanceData ) * batch.Objects.Count ];
			fixed( byte* pVertices = vertices )
			{
				for( int n = 0; n < batch.Objects.Count; n++ )
				{
					var instanceData = (RenderingPipeline.RenderSceneData.ObjectInstanceData*)pVertices + n;

					ref var objectItem = ref batch.Objects.Data[ n ];

					var matrix = new Matrix4F( objectItem.MeshMatrix3, ( objectItem.Position - instancePositionOffset ).ToVector3F() );

					var vector3FZero = Vector3F.Zero;
					//matrix.GetTranslation( out var positionPreviousFrame );

					//!!!!CullingByCameraDirectionData

					instanceData->Init( ref matrix, ref vector3FZero, ref color, lodValue, -1/*visibilityDistance*/, receiveDecals, motionBlurFactor, 0 );


					//ref var matrix = ref objectItem.MeshMatrix;
					//////!!!!double
					////var pos = objectItem.Position.ToVector3F();
					////var matrix = new Matrix4F( ref objectItem.MeshMatrix3, ref pos );

					//matrix.GetTranslation( out var positionPreviousFrame );
					////!!!!CullingByCameraDirectionData
					//instanceData->Init( ref matrix, ref positionPreviousFrame, ref color, lodValue, -1/*visibilityDistance*/, receiveDecals, motionBlurFactor, 0 );
				}
			}

			batch.BatchingInstanceBufferMesh = GpuBufferManager.CreateVertexBuffer( vertices, GpuBufferManager.InstancingVertexDeclaration, GpuBufferFlags.ComputeRead );
			batch.BatchingInstancePositionOffset = instancePositionOffset;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void CreateBatchingInstanceBufferBillboard( Tile.SurfaceObjectsItem surfaceObjectsItem, Tile.SurfaceObjectsRenderGroup batch, float lodValue, /*float visibilityDistance, */bool receiveDecals, float motionBlurFactor, RenderingPipeline.RenderSceneData.IMeshData meshData )
		{
			surfaceObjectsItem.GetObjectsColor( out var color );

			var instancePositionOffset = new Vector3( double.MaxValue, double.MaxValue, double.MaxValue );
			for( int n = 0; n < batch.Objects.Count; n++ )
			{
				ref var objectItem = ref batch.Objects.Data[ n ];
				instancePositionOffset.X = Math.Min( instancePositionOffset.X, objectItem.Position.X );
				instancePositionOffset.Y = Math.Min( instancePositionOffset.Y, objectItem.Position.Y );
				instancePositionOffset.Z = Math.Min( instancePositionOffset.Z, objectItem.Position.Z );
			}

			var vertices = new byte[ sizeof( RenderingPipeline.RenderSceneData.ObjectInstanceData ) * batch.Objects.Count ];
			fixed( byte* pVertices = vertices )
			{
				for( int n = 0; n < batch.Objects.Count; n++ )
				{
					var instanceData = (RenderingPipeline.RenderSceneData.ObjectInstanceData*)pVertices + n;

					ref var objectItem = ref batch.Objects.Data[ n ];

					ref var position = ref objectItem.Position;
					ref var rotation = ref objectItem.Rotation;
					ref var scale = ref objectItem.Scale;

					var scaleH = Math.Max( scale.X, scale.Y );
					var scaleV = scale.Z;

					Vector3F offset;
					if( meshData.BillboardPositionOffset != Vector3F.Zero )
						offset = rotation * ( meshData.BillboardPositionOffset * Math.Max( scaleH, scaleV ) );
					else
						offset = Vector3F.Zero;

					var matrix = new Matrix4F();
					matrix.Item0.X = scaleH;
					matrix.Item0.Y = 0;
					matrix.Item0.Z = rotation.X;
					matrix.Item0.W = 0;
					matrix.Item1.X = rotation.Y;
					matrix.Item1.Y = scaleH;
					matrix.Item1.Z = rotation.Z;
					matrix.Item1.W = 0;
					matrix.Item2.X = rotation.W;
					matrix.Item2.Y = 0;
					matrix.Item2.Z = scaleV;
					matrix.Item2.W = 0;
					matrix.Item3.X = (float)( position.X - instancePositionOffset.X + offset.X );
					matrix.Item3.Y = (float)( position.Y - instancePositionOffset.Y + offset.Y );
					matrix.Item3.Z = (float)( position.Z - instancePositionOffset.Z + offset.Z );
					matrix.Item3.W = 1;

					var vector3FZero = Vector3F.Zero;
					//matrix.GetTranslation( out var positionPreviousFrame );

					//!!!!CullingByCameraDirectionData

					instanceData->Init( ref matrix, ref vector3FZero, ref color, lodValue, -1/*visibilityDistance*/, receiveDecals, motionBlurFactor, 0 );
				}
			}

			batch.BatchingInstanceBufferBillboard = GpuBufferManager.CreateVertexBuffer( vertices, GpuBufferManager.InstancingVertexDeclaration, GpuBufferFlags.ComputeRead );
			batch.BatchingInstancePositionOffset = instancePositionOffset;
		}

		struct GroupItem
		{
			public int Added;
			public float CameraDistanceMinSquared;
			public float BoundingSize;
			public float VisibilityDistance;
			public int OnlyForShadowGeneration;
			public SceneLODUtility.LodState LodState;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void RenderSurfaceObjectsItem( Tile tile, ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem, Tile.SurfaceObjectsItem surfaceObjectsItem )
		{
			var surface = surfaceObjectsItem.GetSurface().Result;

			if( surfaceObjectsItem.RenderGroups != null && surfaceObjectsItem.RenderGroups.Length != 0 && surface != null )
			{
				var cameraSettings = context.Owner.CameraSettings;
				var surfaceObjectsVisibilityDistanceFactor = surfaceObjectsItem.GetObjectsVisibilityDistanceFactor();
				var surfaceObjectsCastShadows = surfaceObjectsItem.GetObjectsCastShadows();
				var groups = surfaceObjectsItem.RenderGroups;
				var groupCount = groups.Length;

				var groupItems = stackalloc GroupItem[ groupCount ];
				var groupIndexesAdded = stackalloc int[ groupCount ];
				var groupIndexesAddedCount = 0;

				//first single threaded pass
				for( int nGroup = 0; nGroup < groupCount; nGroup++ )
				{
					var group = groups[ nGroup ];
					ref var groupItem = ref groupItems[ nGroup ];

					groupItem.Added = 0;

					if( group.NoBatchingGroup )
					{
						//no batching group is already got result here
						RenderObjectsNoBatchingGroup( context, mode, modeGetObjectsItem, surfaceObjectsItem, group );
						//groupsAdded[ nGroup ] = 1;
					}
					else
					{
						surface.GetMesh( group.VariationGroup, group.VariationElement, out var enabled, out var mesh, out _, out var visibilityDistanceFactor, out var castShadows, out _, out _ );
						if( mesh?.Result == null )
							mesh = ResourceUtility.MeshInvalid;

						if( !enabled )
							continue;
						if( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && ( !castShadows || !surfaceObjectsCastShadows ) )
							continue;

						var meshResult = mesh.Result;

						var cameraDistanceMinSquared = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, ref group.BoundingBox );

						var boundingSize = (float)( meshResult.SpaceBounds.boundingSphere.Radius * 2 * group.ObjectsMaxScale );
						var visibilityDistance = context.GetVisibilityDistanceByObjectSize( boundingSize ) * visibilityDistanceFactor * meshResult.MeshData.VisibilityDistanceFactor * surfaceObjectsVisibilityDistanceFactor;

						if( cameraDistanceMinSquared < visibilityDistance * visibilityDistance )
						{
							groupItem.Added = 1;
							groupItem.VisibilityDistance = visibilityDistance;
							groupItem.CameraDistanceMinSquared = cameraDistanceMinSquared;
							groupItem.BoundingSize = boundingSize;

							groupIndexesAdded[ groupIndexesAddedCount++ ] = nGroup;
						}
					}
				}

				//multithreaded pass
				Parallel.For( 0, groupIndexesAddedCount, delegate ( int groupIndexesIndex )//Parallel.For( 0, groupCount, delegate ( int nGroup )
				{
					var nGroup = groupIndexesAdded[ groupIndexesIndex ];
					var group = groups[ nGroup ];
					ref var groupItem = ref groupItems[ nGroup ];

					//if( groupsAdded[ nGroup ] != 0 )
					{
						//if( group.NoBatchingGroup )
						//{
						//	groupsAdded[ nGroup ] = 2;
						//}
						//else
						{
							var add = false;
							var onlyForShadowGeneration = false;
							if( mode == GetRenderSceneDataMode.InsideFrustum )
							{
								add = true;
								onlyForShadowGeneration = !cameraSettings.Frustum.Intersects( ref group.BoundingSphere );
							}
							else if( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && modeGetObjectsItem.Intersects( ref group.BoundingBox ) )
							{
								add = true;
								onlyForShadowGeneration = true;
							}

							if( add )
							{
								groupItem.Added = 2;
								groupItem.OnlyForShadowGeneration = onlyForShadowGeneration ? 1 : 0;

								surface.GetMesh( group.VariationGroup, group.VariationElement, out var enabled, out var mesh );
								if( mesh?.Result == null )
									mesh = ResourceUtility.MeshInvalid;

								var cameraDistanceMaxSquared = SceneLODUtility.GetCameraDistanceMax( cameraSettings, ref group.BoundingSphere );
								cameraDistanceMaxSquared *= cameraDistanceMaxSquared;

								SceneLODUtility.GetDemandedLODs( context, mesh.Result.MeshData, groupItem.CameraDistanceMinSquared, cameraDistanceMaxSquared, groupItem.BoundingSize, out groupItem.LodState );
							}
						}
					}
				} );

				//result pass
				for( int nGroup = 0; nGroup < groupCount; nGroup++ )
				{
					ref var groupItem = ref groupItems[ nGroup ];
					if( groupItem.Added != 2 )
						continue;

					var group = groups[ nGroup ];

					//if( group.NoBatchingGroup )
					//{
					//	RenderObjectsNoBatchingGroup( context, mode, modeGetObjectsItem, surfaceObjectsItem, group );
					//}
					//else
					{
						var onlyForShadowGeneration = groupItem.OnlyForShadowGeneration != 0;
						ref var lodState = ref groupItem.LodState;

						surface.GetMesh( group.VariationGroup, group.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out var visibilityDistanceFactor, out var castShadows, out var receiveDecals, out var motionBlurFactor );
						if( mesh?.Result == null )
							mesh = ResourceUtility.MeshInvalid;

						if( !enabled )
							continue;
						if( onlyForShadowGeneration && ( !castShadows || !surfaceObjectsCastShadows ) )
							continue;

						var meshDataLOD0 = mesh.Result.MeshData;
						var meshDataLods = meshDataLOD0.LODs;

						bool useBatching = false;
						{
							if( meshDataLods == null )
								useBatching = true;
							else
							{
								//check for only one last lod
								if( lodState.Count == 1 )
								{
									var lodCount = meshDataLods.Length + 1;
									lodState.GetItem( 0, out var level, out var range );

									if( level == lodCount - 1 )
										useBatching = true;
								}
							}
						}

						if( useBatching )
						{
							surfaceObjectsItem.GetObjectsColor( out var color );

							var item = new RenderingPipeline.RenderSceneData.MeshItem();
							item.Creator = this;
							item.BoundingSphere = group.BoundingSphere;
							//item.MeshData = mesh.Result.MeshData;
							//item.CastShadows = castShadows && item.MeshData.CastShadows;
							item.ReceiveDecals = receiveDecals;
							item.MotionBlurFactor = motionBlurFactor;
							item.ReplaceMaterial = replaceMaterial;
							item.Color = color;// ColorValue.One;
							RenderingPipeline.GetColorForInstancingData( ref item.Color, out item.ColorForInstancingData1, out item.ColorForInstancingData2 );
							item.VisibilityDistance = groupItem.VisibilityDistance;
							item.OnlyForShadowGeneration = onlyForShadowGeneration;
							item.StaticShadows = true;

							for( int nLodItem = 0; nLodItem < lodState.Count; nLodItem++ )
							{
								lodState.GetItem( nLodItem, out var lodLevel, out _ );

								item.MeshData = meshDataLOD0;
								item.MeshDataLOD0 = meshDataLOD0;

								//MeshDataShadows, MeshDataShadowsForceBestLOD
								var lodScaleShadows = context.LODScaleShadowsSquared * meshDataLOD0.LODScaleShadows;
								if( lodScaleShadows <= lodLevel )
								{
									//select last LOD for shadows
									item.MeshDataShadows = item.MeshDataLastVoxelLOD;
								}
								else if( lodScaleShadows >= 100 )
								{
									//select the best LOD for shadows
									item.MeshDataShadows = meshDataLOD0;
									item.MeshDataShadowsForceBestLOD = true;
								}

								if( lodLevel > 0 )
								{
									ref var lod = ref meshDataLods[ lodLevel - 1 ];
									var lodMeshData = lod.Mesh?.Result?.MeshData;
									if( lodMeshData != null )
										item.MeshData = lodMeshData;
								}

								item.CastShadows = castShadows && surfaceObjectsCastShadows && item.MeshData.CastShadows && groupItem.CameraDistanceMinSquared < context.GetShadowVisibilityDistanceSquared( groupItem.VisibilityDistance );
								item.LODValue = 0;

								if( onlyForShadowGeneration && !item.CastShadows )
									continue;

								//set BatchingInstanceBuffer
								if( item.MeshData.BillboardMode != 0 )
								{
									if( group.BatchingInstanceBufferBillboard == null )
										CreateBatchingInstanceBufferBillboard( surfaceObjectsItem, group, item.LODValue, item.ReceiveDecals, item.MotionBlurFactor, item.MeshData );

									item.InstancingEnabled = true;
									item.InstancingVertexBuffer = group.BatchingInstanceBufferBillboard;
									item.InstancingStart = 0;
									item.InstancingCount = -1;
									item.InstancingMaxLocalBounds = (float)( group.ObjectsMaxScale * item.MeshData.SpaceBounds.boundingSphere.Radius );
									context.ConvertToRelative( ref group.BatchingInstancePositionOffset, out item.InstancingPositionOffsetRelative );
								}
								else
								{
									if( group.BatchingInstanceBufferMesh == null )
										CreateBatchingInstanceBufferMesh( surfaceObjectsItem, group, item.LODValue, item.ReceiveDecals, item.MotionBlurFactor );

									item.InstancingEnabled = true;
									item.InstancingVertexBuffer = group.BatchingInstanceBufferMesh;
									item.InstancingStart = 0;
									item.InstancingCount = -1;
									item.InstancingMaxLocalBounds = (float)( group.ObjectsMaxScale * item.MeshData.SpaceBounds.boundingSphere.Radius );
									context.ConvertToRelative( ref group.BatchingInstancePositionOffset, out item.InstancingPositionOffsetRelative );
								}

								//add to render
								{
									////set AnimationData from event
									//GetRenderSceneDataAddToFrameData?.Invoke( this, context, mode, ref item );

									context.FrameData.RenderSceneData.Meshes.Add( ref item );
								}
							}
						}
						else
						{
							bool containTransparent = replaceMaterial?.Result != null && replaceMaterial.Result.Transparent || meshDataLOD0.ContainsTransparent;
							if( containTransparent )
								RenderObjectsNoBatchingGroup( context, mode, modeGetObjectsItem, surfaceObjectsItem, group );
							else
								RenderObjectsDynamicBatched( context, mode, modeGetObjectsItem, surfaceObjectsItem, group );
						}
					}
				}

				//visualize groups
				if( DebugDrawSurfaceObjectsBounds )
				{
					var renderer = context.Owner.Simple3DRenderer;
					renderer.SetColor( new ColorValue( 0, 0, 1 ) );

					foreach( var group in surfaceObjectsItem.RenderGroups )
						renderer.AddBounds( group.BoundingBox, false );
				}
			}
		}

		unsafe void TileGetRenderSceneDataSurfaceObjects( Tile tile, ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			//when context == null means precalculation of data

			//hide label
			if( context != null )
			{
				var context2 = context.ObjectInSpaceRenderingContext;
				context2.disableShowingLabelForThisObject = true;
			}


			if( tile.SurfaceObjectsItems == null )
				return;

			var cameraSettings = context?.Owner.CameraSettings;

			//create and destroy render groups
			foreach( var surfaceObjectsItem in tile.SurfaceObjectsItems )
			{
				if( context != null )
				{
					//!!!!чтобы при загрузке камеры сразу загрузилось
					//!!!!метод для подгрузки заранее и сразу

					var cameraDistanceToItemSquared = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, ref surfaceObjectsItem.Bounds );

					var visibilityDistance = context.GetVisibilityDistanceByObjectSize( surfaceObjectsItem.ObjectsMaxSize ) * surfaceObjectsItem.ObjectsMaxVisibilityDistanceFactor;// * already applied surfaceObjectsItem.GetObjectsVisibilityDistanceFactor();

					//!!!!что с тенями
					if( cameraDistanceToItemSquared < visibilityDistance * visibilityDistance/* || mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum*/ )
					{
						//if( cameraDistanceToItemSquared < surfaceObjectsItem.MaxVisibilityDistanceSquared )
						//{

						//start to create render groups

						surfaceObjectsItem.RenderGroupsLastVisibleTime = EngineApp.EngineTime;

						if( surfaceObjectsItem.RenderGroups == null )
						{
							//!!!!impl
							//if( EngineApp.RenderVideoToFileData != null || EngineApp.IsEditor )
							//{

							////!!!!
							//var time = EngineApp.GetSystemTime();

							surfaceObjectsItem.RenderGroups = Tile.CalculateSurfaceObjectsRenderGroups( surfaceObjectsItem );

							////!!!!
							//Log.Info( ( EngineApp.GetSystemTime() - time ).ToString() );

							//}
							//else
							//{
							//	//start a task
							//	if( !surfaceObjectsItem.RenderGroupsTaskData )
							//	{
							//		surfaceObjectsItem.RenderGroupsTaskData = true;

							//		var task = new Task( SurfaceObjectsPrepareRenderGroupsTask, surfaceObjectsItem );
							//		task.Start();
							//	}

							//	//render group were calculated
							//	if( surfaceObjectsItem.RenderGroupsTaskResult != null )
							//	{
							//		surfaceObjectsItem.RenderGroups = surfaceObjectsItem.RenderGroupsTaskResult;
							//		surfaceObjectsItem.RenderGroupsTaskData = false;
							//		surfaceObjectsItem.RenderGroupsTaskResult = null;
							//	}
							//}
						}
					}
					else
					{
						//can destroy render groups
						if( surfaceObjectsItem.RenderGroups != null && !NeedPrecalculateObjects() )
						{
							//check time limit
							if( EngineApp.EngineTime > surfaceObjectsItem.RenderGroupsLastVisibleTime + timeLimitToDestroyFarSurfaceObjectGroups )
							{
								//destroy
								tile.DestroySurfaceObjectsRenderGroups( surfaceObjectsItem );
								surfaceObjectsItem.RenderGroupsLastVisibleTime = EngineApp.EngineTime;
							}
						}
					}
				}
				else
				{
					if( NeedPrecalculateObjects() )
					{
						if( surfaceObjectsItem.RenderGroups == null )
							surfaceObjectsItem.RenderGroups = Tile.CalculateSurfaceObjectsRenderGroups( surfaceObjectsItem );
					}
				}
			}

			//!!!!precalculate instance buffers too?
			if( context != null )
			{
				//draw render groups
				foreach( var surfaceObjectsItem in tile.SurfaceObjectsItems )
					RenderSurfaceObjectsItem( tile, context, mode, modeGetObjectsItem, surfaceObjectsItem );
			}
		}

		BaseSurfaceObjectsDataClass GetBaseSurfaceObjectsDataCache()
		{
			if( baseSurfaceObjectsDataCache == null )
			{
				var cache = new BaseSurfaceObjectsDataClass();
				cache.Surface = Surface;
				cache.SurfaceObjects = SurfaceObjects;
				cache.SurfaceObjectsDistribution = (float)SurfaceObjectsDistribution;
				cache.SurfaceObjectsScale = (float)SurfaceObjectsScale;
				cache.SurfaceObjectsColor = SurfaceObjectsColor;
				cache.SurfaceObjectsVisibilityDistanceFactor = (float)SurfaceObjectsVisibilityDistanceFactor;
				cache.SurfaceObjectsCastShadows = SurfaceObjectsCastShadows;
				cache.SurfaceObjectsCollision = SurfaceObjectsCollision;

				//!!!!

				baseSurfaceObjectsDataCache = cache;
			}

			return baseSurfaceObjectsDataCache;
		}

		void NetworkSendData( ServerNetworkService_Components.ClientItem client )
		{
			if( NetworkIsServer && HeightmapBuffer != null )// && ObjectsNetworkMode.Value )
			{

				//!!!!частичное обновление

				{
					var writer = client != null ? BeginNetworkMessage( client, "HeightmapBuffer" ) : BeginNetworkMessageToEveryone( "HeightmapBuffer" );

					var byteArray = HeightmapToByteArray( HeightmapBuffer );
					var zipped = IOUtility.Zip( byteArray, System.IO.Compression.CompressionLevel.Fastest );

					writer.Write( zipped.Length );
					writer.Write( zipped );

					EndNetworkMessage();
				}

				unsafe
				{
					var writer = client != null ? BeginNetworkMessage( client, "HolesCache" ) : BeginNetworkMessageToEveryone( "HolesCache" );

					writer.WriteVariableInt32( holesCache.Count );

					foreach( var item in holesCache )
					{
						writer.Write( item.Key );

						writer.Write( item.Value.vertices.Length );
						fixed( Vector3* pVertices = item.Value.vertices )
							writer.Write( pVertices, item.Value.vertices.Length * sizeof( Vector3 ) );

						writer.Write( item.Value.indices.Length );
						fixed( int* pIndices = item.Value.indices )
							writer.Write( pIndices, item.Value.indices.Length * sizeof( int ) );
					}

					EndNetworkMessage();
				}
			}
		}

		protected override void OnClientConnectedBeforeRootComponentEnabled( ServerNetworkService_Components.ClientItem client )
		{
			base.OnClientConnectedBeforeRootComponentEnabled( client );

			NetworkSendData( client );
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "HeightmapBuffer" )
			{
				var length = reader.ReadInt32();
				var array = new byte[ length ];
				reader.ReadBuffer( array, 0, length );

				if( !reader.Complete() )
					return false;

				try
				{
					var unzipped = IOUtility.Unzip( array );
					var heightmapBuffer = HeightmapFromByteArray( unzipped );

					HeightmapBuffer = heightmapBuffer;
				}
				catch
				{
					return false;
				}
			}

			unsafe
			{
				if( message == "HolesCache" )
				{
					holesCache.Clear();

					var count = reader.ReadVariableInt32();

					for( int n = 0; n < count; n++ )
					{
						var index = reader.ReadVector2I();

						var vertexCount = reader.ReadInt32();
						var vertices = new Vector3[ vertexCount ];
						fixed( Vector3* pVertices = vertices )
							reader.ReadBuffer( pVertices, vertexCount * sizeof( Vector3 ) );

						var indexCount = reader.ReadInt32();
						var indices = new int[ indexCount ];
						fixed( int* pIndices = indices )
							reader.ReadBuffer( pIndices, indexCount * sizeof( int ) );

						var item = new HoleCacheItem();
						item.vertices = vertices;
						item.indices = indices;
						holesCache[ index ] = item;
					}

					if( !reader.Complete() )
						return false;
				}
			}

			return true;
		}

		bool NeedPrecalculateObjects()
		{
			if( EngineApp.IsSimulation )
				return PrecalculateObjects;
			return false;
		}

	}
}
