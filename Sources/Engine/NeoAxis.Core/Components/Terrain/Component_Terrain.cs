// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using System.Collections;
using NeoAxis.Editor;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Heightmap based terrain.
	/// </summary>
	[EditorSettingsCell( typeof( Component_Terrain_SettingsCell ) )]
	public class Component_Terrain : Component
	{
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

		int tileSize = 32;

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

		Component_RenderingPipeline.RenderSceneData.LayerItem[] currentLayers;

		/////////////////////////////////////////

		//[FieldSerialize( "ambientOcclusion" )]
		//AmbientOcclusionProperties ambientOcclusion;

		////masks textures
		//int masksTextureSize;
		//float masksTextureSizeInv;
		//int masksTexturesCount;
		//int tilesCountPerMasksTexture;
		//int needMaskPixelsForTile;
		//internal MaskTextureItem[,] masksTextures;

		////normals and height textures
		//internal int normalsTextureSize;
		//int normalsTexturesCount;
		//internal int tilesCountPerNormalsTexture;
		//internal TextureData[,] normalsTextures;
		////for shared vertex buffers mode
		//internal TextureData[,] heightTextures;

		////ambient occlusion
		//int aoTextureSize;
		//float aoTextureSizeInv;
		//int aoTexturesCount;
		//int tilesCountPerAOTexture;
		//int needAOPixelsForTile;
		//bool needUpdateAOTextures;
		//internal TextureData[,] aoTextures;

		////shared vertex index buffers
		//VertexData[] sharedVertexDatas;
		//IndexData[] sharedIndexDatas;

		////collision bodies
		//Body heightFieldBody;
		//Body[,] bodies;

		//bool needReloadLayersTextures;

		//bool needUpdateAfterDeviceLost;

		//float[] cachedLODLevelDistances;

		//[FieldSerialize( "heightmapTerrainManager" )]
		//internal HeightmapTerrainManager heightmapTerrainManager;
		//[FieldSerialize( "heightmapTerrainManagerIndex" )]
		//internal Vector2I heightmapTerrainManagerIndex;

		////streaming load
		//Task streamingLoadTask;
		//List<ResourceBackgroundQueue.TextureBackgroundProcessTicket> streamingLoadTextureTasks =
		//	new List<ResourceBackgroundQueue.TextureBackgroundProcessTicket>();
		//bool streamingLoadError;

		////streaming enable
		//Task streamingEnableTask;
		//bool streamingEnableError;
		//bool streamingEnableMustStop;
		//int streamingEnableFinishCounter;
		//Dictionary<TextureData, IntPtr> streamingEnableDataForTextures = new Dictionary<TextureData, IntPtr>();
		//float lastFinishedStreamingEnableTime;

		//Thread mainThread;

		//bool modified;

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
				if( _position.BeginSet( ref value ) )
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
		public event Action<Component_Terrain> PositionChanged;
		ReferenceField<Vector3> _position;

		//!!!!default
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
				if( _horizontalSize.BeginSet( ref value ) )
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
		public event Action<Component_Terrain> HorizontalSizeChanged;
		ReferenceField<double> _horizontalSize = 500;

		public enum HeightmapSizeEnum
		{
			_128x128,
			_256x256,
			_512x512,
			_1024x1024,
			_2048x2048,
			//!!!!
			//_4096x4096,
		}

		/// <summary>
		/// Resolution of the height map.
		/// </summary>
		[DefaultValue( HeightmapSizeEnum._512x512 )]
		[UndoDependentProperty( nameof( HeightmapSize_UndoDependentData ) )]
		[Category( "Geometry" )]
		public Reference<HeightmapSizeEnum> HeightmapSize
		{
			get { if( _heightmapSize.BeginGet() ) HeightmapSize = _heightmapSize.Get( this ); return _heightmapSize.value; }
			set
			{
				if( _heightmapSize.BeginSet( ref value ) )
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
		public event Action<Component_Terrain> HeightmapSizeChanged;
		ReferenceField<HeightmapSizeEnum> _heightmapSize = HeightmapSizeEnum._512x512;//2048;

		/// <summary>
		/// The list of objects that are shapes for cutting holes.
		/// </summary>
		[Cloneable( CloneType.Deep )]
		[Category( "Geometry" )]
		[Serialize]
		public ReferenceList<Component_MeshInSpace> Holes
		{
			get { return _holes; }
		}
		public delegate void BaseObjectsChangedDelegate( Component_Terrain sender );
		public event BaseObjectsChangedDelegate HolesChanged;
		ReferenceList<Component_MeshInSpace> _holes;

		/// <summary>
		/// Whether to enable holes.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Geometry" )]
		public Reference<bool> HolesEnabled
		{
			get { if( _holesEnabled.BeginGet() ) HolesEnabled = _holesEnabled.Get( this ); return _holesEnabled.value; }
			set { if( _holesEnabled.BeginSet( ref value ) ) { try { HolesEnabledChanged?.Invoke( this ); RecreateInternalData( true ); } finally { _holesEnabled.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HolesEnabled"/> property value changes.</summary>
		public event Action<Component_Terrain> HolesEnabledChanged;
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
				if( _visible.BeginSet( ref value ) )
				{
					try
					{
						VisibleChanged?.Invoke( this );

						foreach( var tile in GetTiles() )
						{
							if( tile.ObjectInSpace != null )
								tile.ObjectInSpace.Visible = _visible.value;
						}
					}
					finally { _visible.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Visible"/> property value changes.</summary>
		public event Action<Component_Terrain> VisibleChanged;
		ReferenceField<bool> _visible = true;

		/// <summary>
		/// Base material of a terrain.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Display" )]
		public Reference<Component_Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set
			{
				if( _material.BeginSet( ref value ) )
				{
					try
					{
						MaterialChanged?.Invoke( this );

						foreach( var tile in GetTiles() )
						{
							if( tile.ObjectInSpace != null )
								tile.ObjectInSpace.ReplaceMaterial = _material.value;
						}
					}
					finally { _material.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<Component_Terrain> MaterialChanged;
		ReferenceField<Component_Material> _material = null;

		/// <summary>
		/// The number of UV tiles per unit.
		/// </summary>
		[DefaultValue( "1 1" )]
		[Serialize]
		[DisplayName( "Material UV 0" )]
		[Category( "Display" )]
		public Reference<Vector2> MaterialUV0
		{
			get { if( _materialUV0.BeginGet() ) MaterialUV0 = _materialUV0.Get( this ); return _materialUV0.value; }
			set
			{
				if( _materialUV0.BeginSet( ref value ) )
				{
					try
					{
						MaterialUV0Changed?.Invoke( this );
						//!!!!после оптимизаций полностью не нужно будет обновлять
						RecreateInternalData();
					}
					finally { _materialUV0.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialUV0"/> property value changes.</summary>
		public event Action<Component_Terrain> MaterialUV0Changed;
		ReferenceField<Vector2> _materialUV0 = Vector2.One;

		/// <summary>
		/// The number of UV tiles per unit.
		/// </summary>
		[DefaultValue( "1 1" )]
		[Serialize]
		[DisplayName( "Material UV 1" )]
		[Category( "Display" )]
		public Reference<Vector2> MaterialUV1
		{
			get { if( _materialUV1.BeginGet() ) MaterialUV1 = _materialUV1.Get( this ); return _materialUV1.value; }
			set
			{
				if( _materialUV1.BeginSet( ref value ) )
				{
					try
					{
						MaterialUV1Changed?.Invoke( this );
						//!!!!после оптимизаций полностью не нужно будет обновлять
						RecreateInternalData();
					}
					finally { _materialUV1.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialUV1"/> property value changes.</summary>
		public event Action<Component_Terrain> MaterialUV1Changed;
		ReferenceField<Vector2> _materialUV1 = Vector2.One;

		public enum PaintMaskSizeEnum
		{
			//_128x128,
			//_256x256,
			_512x512,
			_1024x1024,
			_2048x2048,
			_4096x4096,
		}

		//!!!!default
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
				if( _paintMaskSize.BeginSet( ref value ) )
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
		public event Action<Component_Terrain> PaintMaskSizeChanged;
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
				if( _castShadows.BeginSet( ref value ) )
				{
					try
					{
						CastShadowsChanged?.Invoke( this );

						foreach( var tile in GetTiles() )
						{
							if( tile.ObjectInSpace != null )
								tile.ObjectInSpace.CastShadows = _castShadows.value;
						}
					}
					finally { _castShadows.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CastShadows"/> property value changes.</summary>
		public event Action<Component_Terrain> CastShadowsChanged;
		ReferenceField<bool> _castShadows = false;

		/// <summary>
		/// Whether it is possible to apply decals the surface.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Display" )]
		public Reference<bool> ReceiveDecals
		{
			get { if( _receiveDecals.BeginGet() ) ReceiveDecals = _receiveDecals.Get( this ); return _receiveDecals.value; }
			set
			{
				if( _receiveDecals.BeginSet( ref value ) )
				{
					try
					{
						ReceiveDecalsChanged?.Invoke( this );

						foreach( var tile in GetTiles() )
						{
							if( tile.ObjectInSpace != null )
								tile.ObjectInSpace.ReceiveDecals = _receiveDecals.value;
						}
					}
					finally { _receiveDecals.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<Component_Terrain> ReceiveDecalsChanged;
		ReferenceField<bool> _receiveDecals = true;

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
				if( _collision.BeginSet( ref value ) )
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
		public event Action<Component_Terrain> CollisionChanged;
		ReferenceField<bool> _collision = true;

		/// <summary>
		/// The physical material used by the rigidbody.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		[Category( "Physics" )]
		public Reference<Component_PhysicalMaterial> CollisionMaterial
		{
			get { if( _collisionMaterial.BeginGet() ) CollisionMaterial = _collisionMaterial.Get( this ); return _collisionMaterial.value; }
			set
			{
				if( _collisionMaterial.BeginSet( ref value ) )
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
		public event Action<Component_Terrain> CollisionMaterialChanged;
		ReferenceField<Component_PhysicalMaterial> _collisionMaterial;

		/// <summary>
		/// The type of friction applied on the rigidbody.
		/// </summary>
		[DefaultValue( Component_PhysicalMaterial.FrictionModeEnum.Simple )]
		[Serialize]
		[Category( "Physics" )]
		public Reference<Component_PhysicalMaterial.FrictionModeEnum> CollisionFrictionMode
		{
			get { if( _collisionFrictionMode.BeginGet() ) CollisionFrictionMode = _collisionFrictionMode.Get( this ); return _collisionFrictionMode.value; }
			set
			{
				if( _collisionFrictionMode.BeginSet( ref value ) )
				{
					try
					{
						CollisionFrictionModeChanged?.Invoke( this );
						SetCollisionMaterial();
					}
					finally { _collisionFrictionMode.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionFrictionMode"/> property value changes.</summary>
		public event Action<Component_Terrain> CollisionFrictionModeChanged;
		ReferenceField<Component_PhysicalMaterial.FrictionModeEnum> _collisionFrictionMode = Component_PhysicalMaterial.FrictionModeEnum.Simple;

		/// <summary>
		/// The amount of friction applied on the rigidbody.
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
				if( _collisionFriction.BeginSet( ref value ) )
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
		public event Action<Component_Terrain> CollisionFrictionChanged;
		ReferenceField<double> _collisionFriction = 0.5;

		/// <summary>
		/// The amount of directional friction applied on the rigidbody.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Serialize]
		//[ApplicableRange( 0, 1 )]
		[Category( "Physics" )]
		public Reference<Vector3> CollisionAnisotropicFriction
		{
			get { if( _collisionAnisotropicFriction.BeginGet() ) CollisionAnisotropicFriction = _collisionAnisotropicFriction.Get( this ); return _collisionAnisotropicFriction.value; }
			set
			{
				if( _collisionAnisotropicFriction.BeginSet( ref value ) )
				{
					try { CollisionAnisotropicFrictionChanged?.Invoke( this ); }
					finally { _collisionAnisotropicFriction.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AnisotropicFriction"/> property value changes.</summary>
		public event Action<Component_Terrain> CollisionAnisotropicFrictionChanged;
		ReferenceField<Vector3> _collisionAnisotropicFriction = Vector3.One;

		/// <summary>
		/// The amount of friction applied when rigidbody is spinning.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Serialize]
		[Range( 0, 1 )]
		[Category( "Physics" )]
		public Reference<double> CollisionSpinningFriction
		{
			get { if( _collisionSpinningFriction.BeginGet() ) CollisionSpinningFriction = _collisionSpinningFriction.Get( this ); return _collisionSpinningFriction.value; }
			set
			{
				if( _collisionSpinningFriction.BeginSet( ref value ) )
				{
					try
					{
						CollisionSpinningFrictionChanged?.Invoke( this );
						SetCollisionMaterial();
					}
					finally { _collisionSpinningFriction.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionSpinningFriction"/> property value changes.</summary>
		public event Action<Component_Terrain> CollisionSpinningFrictionChanged;
		ReferenceField<double> _collisionSpinningFriction = 0.5;

		/// <summary>
		/// The amount of friction applied when rigidbody is rolling.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Serialize]
		[Range( 0, 1 )]
		[Category( "Physics" )]
		public Reference<double> CollisionRollingFriction
		{
			get { if( _collisionRollingFriction.BeginGet() ) CollisionRollingFriction = _collisionRollingFriction.Get( this ); return _collisionRollingFriction.value; }
			set
			{
				if( _collisionRollingFriction.BeginSet( ref value ) )
				{
					try
					{
						CollisionRollingFrictionChanged?.Invoke( this );
						SetCollisionMaterial();
					}
					finally { _collisionRollingFriction.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionRollingFriction"/> property value changes.</summary>
		public event Action<Component_Terrain> CollisionRollingFrictionChanged;
		ReferenceField<double> _collisionRollingFriction = 0.5;

		/// <summary>
		/// The ratio of the final relative velocity to initial relative velocity of the rigidbody after collision.
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
				if( _collisionRestitution.BeginSet( ref value ) )
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
		public event Action<Component_Terrain> CollisionRestitutionChanged;
		ReferenceField<double> _collisionRestitution;

		public enum TileSizeEnum
		{
			_8x8,
			_16x16,
			_32x32,
			_64x64,
			_128x128,
		}

		/// <summary>
		/// The size of internal tile cells. These cells divide the entire geometry of the landscape into pieces.
		/// </summary>
		[DefaultValue( TileSizeEnum._32x32 )]
		[Category( "Optimization" )]
		public Reference<TileSizeEnum> TileSize
		{
			get { if( _tileSize.BeginGet() ) TileSize = _tileSize.Get( this ); return _tileSize.value; }
			set
			{
				if( _tileSize.BeginSet( ref value ) )
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
		public event Action<Component_Terrain> TileSizeChanged;
		ReferenceField<TileSizeEnum> _tileSize = TileSizeEnum._32x32;

		/// <summary>
		/// Whether to enable level of detail.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Optimization" )]
		[DisplayName( "LOD Enabled" )]
		public Reference<bool> LODEnabled
		{
			get { if( _lODEnabled.BeginGet() ) LODEnabled = _lODEnabled.Get( this ); return _lODEnabled.value; }
			set { if( _lODEnabled.BeginSet( ref value ) ) { try { LODEnabledChanged?.Invoke( this ); RecreateInternalData( false ); } finally { _lODEnabled.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODEnabled"/> property value changes.</summary>
		public event Action<Component_Terrain> LODEnabledChanged;
		ReferenceField<bool> _lODEnabled = true;

		[DefaultValue( 4 )]
		[Category( "Optimization" )]
		[DisplayName( "LOD Count" )]
		[Range( 1, 6 )]
		public Reference<int> LODCount
		{
			get { if( _lODCount.BeginGet() ) LODCount = _lODCount.Get( this ); return _lODCount.value; }
			set { if( _lODCount.BeginSet( ref value ) ) { try { LODCountChanged?.Invoke( this ); RecreateInternalData( false ); } finally { _lODCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODCount"/> property value changes.</summary>
		public event Action<Component_Terrain> LODCountChanged;
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
		//	set { if( _lODRange.BeginSet( ref value ) ) { try { LODRangeChanged?.Invoke( this ); } finally { _lODRange.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LODRange"/> property value changes.</summary>
		//public event Action<Component_Terrain> LODRangeChanged;
		//ReferenceField<RangeI> _lODRange = new RangeI( 0, 10 );

		/// <summary>
		/// The distance from the previous to the next level of detail.
		/// </summary>
		[DisplayName( "LOD Distance" )]
		[Category( "Optimization" )]
		[DefaultValue( 20.0 )]
		[Range( 1, 50, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> LODDistance
		{
			get { if( _lODDistance.BeginGet() ) LODDistance = _lODDistance.Get( this ); return _lODDistance.value; }
			set
			{
				if( _lODDistance.BeginSet( ref value ) )
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
		public event Action<Component_Terrain> LODDistanceChanged;
		ReferenceField<double> _lODDistance = 20.0;


		/////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		struct Vertex
		{
			public Vector3F Position;
			//!!!!сжимать
			public Vector3F Normal;
			public Vector4F Tangent;
			public Vector2F TexCoord0;
			public Vector2F TexCoord1;
			public Vector2F TexCoord2;
		}

		/////////////////////////////////////////

		class Tile
		{
			public Component_Terrain owner;

			public Vector2I index;
			public Vector2I cellIndexMin;
			public Vector2I cellIndexMax;

			public Component_MeshInSpace ObjectInSpace;
			public Component_Mesh Mesh;
			public bool? RenderingDataCreatedWithExtractData;

			public Vector3[] Vertices;
			public int[] Indices;
			public SpaceBounds Bounds;
			public Vector3 Position;

			public Component_RigidBody CollisionBody;

			////cached for masks
			//Vector2I masksTextureCoordMin = new Vector2I( -10000, -10000 );
			//Vector2I masksIndexMin = new Vector2I( -10000, -10000 );
			//Vector2I aoTextureCoordMin = new Vector2I( -10000, -10000 );
			//Vector2I aoIndexMin = new Vector2I( -10000, -10000 );

			//

			public Tile( Component_Terrain owner, Vector2I index )
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
					Log.Fatal( "Component_Terrain: Tile: GenerateTileVerticesInfo: lodLevel >= cachedLodLevelCount." );

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


				//calculate Vertices, Indices
				Vertices = new Vector3[ vertexCount ];
				for( int n = 0; n < vertexCount; n++ )
					owner.GetPosition( cellIndices[ n ], out Vertices[ n ] );
				Indices = owner.GenerateTileIndicesWithoutHoles( 0 );

				//calculate Bounds, Position
				var bounds = NeoAxis.Bounds.Cleared;
				for( int n = 0; n < Vertices.Length; n++ )
					bounds.Add( ref Vertices[ n ] );
				Bounds = new SpaceBounds( bounds );
				Position = bounds.GetCenter();

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
							Net3dBool.Solid currentSolid = null;

							foreach( var hole in holes )
							{
								////еще не посчитан при загрузке. объект еще не EnabledInHierarchy, т.к. ниже в иерархии
								if( hole.SpaceBounds.CalculatedBoundingBox.Intersects( ref bounds ) )
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
													var boolVertices2 = new Net3dBool.Vector3[ Vertices.Length ];
													for( int n = 0; n < Vertices.Length; n++ )
													{
														ref var v = ref Vertices[ n ];
														boolVertices2[ n ] = new Net3dBool.Vector3( v.X, v.Y, v.Z );
													}
													currentSolid = new Net3dBool.Solid( boolVertices2, Indices );
												}

												Net3dBool.Solid holeSolid;
												{
													var transform = hole.TransformV;

													var boolVertices = new Net3dBool.Vector3[ mesh.Result.ExtractedVertices.Length ];
													for( int n = 0; n < boolVertices.Length; n++ )
													{
														//!!!!slowly
														var v = transform * mesh.Result.ExtractedVertices[ n ].Position;
														boolVertices[ n ] = new Net3dBool.Vector3( v.X, v.Y, v.Z );
													}
													holeSolid = new Net3dBool.Solid( boolVertices, mesh.Result.ExtractedIndices );
												}

												var modeller = new Net3dBool.BooleanModeller( currentSolid, holeSolid );
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

								//calculate vertex buffer data
								vertices = new Vertex[ Vertices.Length ];
								{
									Parallel.For( 0, vertices.Length, delegate ( int index )
									{
										var position = Vertices[ index ];
										var normal = owner.GetNormal( position.ToVector2() + new Vector2F( 0.001f, 0.001f ) );

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

							//calculate vertex buffer data
							vertices = new Vertex[ Vertices.Length ];
							{
								Parallel.For( 0, vertices.Length, delegate ( int index )
								{
									var position = Vertices[ index ];
									var normal = owner.GetNormal( position.ToVector2() + new Vector2F( 0.001f, 0.001f ) );

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
								} );
							}

							supportsLODs = false;
						}
					}
				}

				//set out verticesBytes
				fixed ( Vertex* pVertices = vertices )
				{
					verticesBytes = new byte[ vertices.Length * sizeof( Vertex ) ];
					fixed ( byte* pVerticesBytes = verticesBytes )
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
							} );
						}

						//set out verticesBytes
						byte[] lodVerticesBytes;
						fixed ( Vertex* pVertices = lodVertices )
						{
							lodVerticesBytes = new byte[ lodVertices.Length * sizeof( Vertex ) ];
							fixed ( byte* pVerticesBytes = lodVerticesBytes )
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

				//	var geometries = firstMeshInSpace.Mesh.Value.GetComponents<Component_MeshGeometry>();
				//	var resultGeometries = new List<(Vector3F[] positions, int[] indices, MeshData.MeshGeometryFormat format)>();
				//	var geometriesToDelete = new List<Component_MeshGeometry>();

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
				//	//calculate temp Bounds. full calculation from streaming thread
				//	bounds.Add( GetPosition( tile.cellIndexMin ) );
				//	bounds.Add( GetPosition( tile.cellIndexMax ) );
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

				//if( !streamingEnable )
				//{
				//	tile.UpdateGpuParameters();
				//	tile.UpdateUseStaticLightingData();
				//}

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
						sectors.DisplayInEditor = false;
						owner.AddComponent( sectors, -1 );
						//sectors = owner.CreateComponent<Component>();

						sectors.Name = "__Sectors";
						sectors.SaveSupport = false;
						sectors.CloneSupport = false;
						sectors.Enabled = true;
					}

					//!!!!раскладывать во вложенную группу, чтобы меньше было в одном списке?

					ObjectInSpace = sectors.CreateComponent<Component_MeshInSpace>( enabled: false );
					ObjectInSpace.AnyData = this;
					ObjectInSpace.Name = index.ToString();
					ObjectInSpace.SaveSupport = false;
					ObjectInSpace.CloneSupport = false;
					ObjectInSpace.CanBeSelected = false;
					ObjectInSpace.Transform = new Transform( Position );
					ObjectInSpace.SpaceBoundsUpdateEvent += ObjectInSpace_SpaceBoundsUpdateEvent;
					ObjectInSpace.GetRenderSceneData += ObjectInSpace_GetRenderSceneData;

					Mesh = ObjectInSpace.CreateComponent<Component_Mesh>();
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
				var meshGeometry = Mesh.CreateComponent<Component_MeshGeometry>();
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

						var lod = Mesh.CreateComponent<Component_MeshLevelOfDetail>();
						lod.Name = "LOD " + index.ToString();
						lod.Distance = owner.LODDistance.Value * index;

						var lodMesh = lod.CreateComponent<Component_Mesh>();
						lodMesh.Name = "Mesh";

						var lodMeshGeometry = lodMesh.CreateComponent<Component_MeshGeometry>();
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
					ObjectInSpace.ReplaceMaterial = owner.Material;
					ObjectInSpace.CastShadows = owner.CastShadows;
					ObjectInSpace.ReceiveDecals = owner.ReceiveDecals;
				}
				//!!!!только нужные добавлять
				ObjectInSpace.PaintLayersReplace = owner.currentLayers;

				//update rendering data status
				RenderingDataCreatedWithExtractData = meshExtractData;
			}

			public void DestroyRenderingData()
			{
				if( ObjectInSpace != null )
				{
					ObjectInSpace.SpaceBoundsUpdateEvent -= ObjectInSpace_SpaceBoundsUpdateEvent;
					ObjectInSpace.GetRenderSceneData -= ObjectInSpace_GetRenderSceneData;
					ObjectInSpace.RemoveFromParent( false );
					ObjectInSpace = null;
					RenderingDataCreatedWithExtractData = null;
				}
			}

			public void Destroy()
			{
				DestroyCollisionBody();
				DestroyRenderingData();
			}

			private void ObjectInSpace_SpaceBoundsUpdateEvent( Component_ObjectInSpace obj, ref SpaceBounds newBounds )
			{
				var sector = obj.AnyData as Tile;
				if( sector != null && sector.Bounds != null )
					newBounds = sector.Bounds;
			}

			private void ObjectInSpace_GetRenderSceneData( Component_ObjectInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode )
			{
				if( owner.Visible )
				{
					var sector = sender.AnyData as Tile;
					if( sector != null )
						owner.SectorGetRenderSceneData( sector, context );
				}
			}

			public void UpdateCollisionBody()
			{
				DestroyCollisionBody();

				if( owner.Collision && ObjectInSpace != null && Indices.Length != 0 )
				{
					CollisionBody = ObjectInSpace.CreateComponent<Component_RigidBody>( enabled: false );
					CollisionBody.AnyData = this;
					CollisionBody.Name = "Collision Body";
					CollisionBody.SpaceBoundsUpdateEvent += CollisionBody_SpaceBoundsUpdateEvent;

					//if( existsHoles || true )
					//{

					CollisionBody.Transform = ObjectInSpace.Transform;

					var shape = CollisionBody.CreateComponent<Component_CollisionShape_Mesh>();
					shape.ShapeType = Component_CollisionShape_Mesh.ShapeTypeEnum.TriangleMesh;

					var vertices = new Vector3F[ Vertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						vertices[ n ] = ( Vertices[ n ] - Position ).ToVector3F();
					shape.Vertices = vertices;

					shape.Indices = Indices;
					shape.CheckValidData = false;
					shape.MergeEqualVerticesRemoveInvalidTriangles = false;

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

					//	var shape = CollisionBody.CreateComponent<Component_CollisionShape_Heightfield>();
					//	shape.VertexCount = new Vector2I( vertexCountByAxis, vertexCountByAxis );
					//	shape.Heights = heights;

					//	var scl = owner.cachedCellSize * owner.tileSize;
					//	shape.TransformRelativeToParent = new Transform( Vector3.Zero, Quaternion.Identity, new Vector3( scl, scl, 1 ) );
					//}

					SetCollisionMaterial();

					CollisionBody.Enabled = true;
				}
			}

			public void DestroyCollisionBody()
			{
				if( CollisionBody != null )
				{
					CollisionBody.SpaceBoundsUpdateEvent -= CollisionBody_SpaceBoundsUpdateEvent;
					CollisionBody.Dispose();
					CollisionBody = null;
				}
			}

			private void CollisionBody_SpaceBoundsUpdateEvent( Component_ObjectInSpace obj, ref SpaceBounds newBounds )
			{
				var sector = obj.AnyData as Tile;
				if( sector != null && sector.Bounds != null )
					newBounds = sector.Bounds;
			}

			public void SetCollisionMaterial()
			{
				if( CollisionBody != null )
				{
					CollisionBody.Material = owner.CollisionMaterial;
					CollisionBody.MaterialFrictionMode = owner.CollisionFrictionMode;
					CollisionBody.MaterialFriction = owner.CollisionFriction;
					CollisionBody.MaterialAnisotropicFriction = owner.CollisionAnisotropicFriction;
					CollisionBody.MaterialSpinningFriction = owner.CollisionSpinningFriction;
					CollisionBody.MaterialRollingFriction = owner.CollisionRollingFriction;
					CollisionBody.MaterialRestitution = owner.CollisionRestitution;
				}
			}

			public void UpdateLayers()
			{
				if( ObjectInSpace != null )
				{
					//!!!!только нужные добавлять
					ObjectInSpace.PaintLayersReplace = owner.currentLayers;
				}
			}

			public void SetLODDistances()
			{
				if( Mesh != null )
				{
					var lods = Mesh.GetComponents<Component_MeshLevelOfDetail>();
					for( int n = 0; n < lods.Length; n++ )
					{
						var lod = lods[ n ];
						var index = n + 1;
						lod.Distance = owner.LODDistance.Value * index;
					}

					//!!!!?
					//update
					if( Mesh.Result != null )
						Mesh.ResultCompile();
				}
			}
		}

		/////////////////////////////////////////

		public Component_Terrain()
		{
			_holes = new ReferenceList<Component_MeshInSpace>( this, () => HolesChanged?.Invoke( this ) );

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

				case nameof( CollisionFrictionMode ):
					if( !Collision || CollisionMaterial.Value != null )
						skip = true;
					break;

				case nameof( CollisionFriction ):
					if( !Collision || CollisionMaterial.Value != null )
						skip = true;
					break;

				case nameof( CollisionRollingFriction ):
				case nameof( CollisionSpinningFriction ):
				case nameof( CollisionAnisotropicFriction ):
					if( !Collision || CollisionFrictionMode.Value == Component_PhysicalMaterial.FrictionModeEnum.Simple || CollisionMaterial.Value != null )
						skip = true;
					break;

				case nameof( CollisionRestitution ):
					if( !Collision || CollisionMaterial.Value != null )
						skip = true;
					break;
				}
			}
		}

		static int ParseHeightmapSize( HeightmapSizeEnum enumValue )
		{
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
			var str = enumValue.ToString();
			var index = str.IndexOf( "x" );
			var str2 = str.Substring( 1, index - 1 );
			return int.Parse( str2 );
		}

		public int GetPaintMaskSizeInteger()
		{
			return ParsePaintMaskSize( PaintMaskSize );
		}

		string GetLoadVirtualDirectory()
		{
			string name = GetPathFromRoot();
			foreach( char c in new string( Path.GetInvalidFileNameChars() ) + new string( Path.GetInvalidPathChars() ) )
				name = name.Replace( c.ToString(), "_" );
			name = name.Replace( " ", "_" );
			return Path.Combine( ComponentUtility.GetOwnedFileNameOfComponent( this ) + "_Files", name );
		}

		string GetSaveRealDirectory( string realFileName )
		{
			string name = GetPathFromRoot();
			foreach( char c in new string( Path.GetInvalidFileNameChars() ) + new string( Path.GetInvalidPathChars() ) )
				name = name.Replace( c.ToString(), "_" );
			name = name.Replace( " ", "_" );
			return Path.Combine( realFileName + "_Files", name );
		}

		unsafe bool LoadHeightmapBuffer( string virtualDirectory, out string error )
		{
			string virtualFileName = Path.Combine( virtualDirectory, "Heightmap.exr" );

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

					fixed ( byte* pData2 = data )
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

					fixed ( byte* pData2 = data )
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

			//load heightmap buffer
			var virtualDirectory = GetLoadVirtualDirectory();
			if( !LoadHeightmapBuffer( virtualDirectory, out error ) )
				return false;

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
									fixed ( Vector3* pVertices = vertices )
									fixed ( byte* pVerticesBytes = verticesBytes )
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

			return true;
		}

		unsafe bool SaveHeightmapBuffer( string realDirectory, out string error )
		{
			if( heightmapBuffer != null )
			{
				string realFileName = Path.Combine( realDirectory, "Heightmap.exr" );

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

				fixed ( float* pData = data )
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
				var realDirectory = GetSaveRealDirectory( context.realFileName );
				//var realDirectory = VirtualPathUtility.GetRealPathByVirtual( GetLoadVirtualDirectory( context.realFileName ) );

				try
				{
					if( !Directory.Exists( realDirectory ) )
						Directory.CreateDirectory( realDirectory );
				}
				catch( Exception e )
				{
					error = e.Message;
					return false;
				}

				//if( loadingStatus != LoadingStatuses.Loading )
				//{
				if( !SaveHeightmapBuffer( realDirectory, out error ) )
					return false;
				//	SaveMasks();
				//	SaveAOData();
				//}
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
						fixed ( Vector3* pVertices = vertices )
						{
							verticesBytes = new byte[ vertices.Length * sizeof( Vector3 ) ];
							fixed ( byte* pVerticesBytes = verticesBytes )
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

			var scene = FindParent<Component_Scene>();
			if( scene != null )
			{
				if( EnabledInHierarchy )
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
				else
					scene.GetRenderSceneData -= Scene_GetRenderSceneData;
			}

			//double t = EngineApp.GetSystemTime();
			CreateTiles();
			CreateCollisionBodies();
			//Log.Warning( EngineApp.GetSystemTime() - t );

			//из OnPostCreate

			//RenderSystem.Instance.RenderSystemEvent += RenderSystemEvent;

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

			//	if( EntitySystemWorld.Instance.WorldSimulationType != WorldSimulationTypes.Editor )
			//	{
			//		if( AmbientOcclusion.Enabled && !AOIsEnabledAndValid() )
			//		{
			//			Log.Warning( Translate( "HeightmapTerrain: Ambient Occlusion is enabled but is not valid. Need recalculate." ) );
			//		}
			//	}
			//}



			//OnDestroy

			//if( loadingStatus == LoadingStatuses.Loading )
			//{
			//	float startWaitTime = EngineApp.Instance.Time;
			//	while( streamingLoadTask != null && streamingLoadTask.Status == TaskStatus.Running )
			//	{
			//		StreamingLoadUpdate();
			//		if( ( EngineApp.Instance.Time - startWaitTime ) > 10 )
			//			break;
			//	}
			//}
			//if( streamingEnable )
			//{
			//	float startWaitTime = EngineApp.Instance.Time;
			//	while( streamingEnableTask != null && streamingEnableTask.Status == TaskStatus.Running )
			//	{
			//		StreamingEnableUpdate();
			//		if( ( EngineApp.Instance.Time - startWaitTime ) > 10 )
			//			break;
			//	}
			//}

			//RenderSystem.Instance.RenderSystemEvent -= RenderSystemEvent;


			//при изменении Enabled

			//if( IsPostCreated && loadingStatus == LoadingStatuses.Loaded && !streamingEnable )
			//{
			//	UpdateAllRecreatableDataForRendering();
			//	CreateCollisionBodies();
			//}
		}

		void SectorGetRenderSceneData( Tile sector, ViewportRenderingContext context )
		{
			//hide label
			var context2 = context.objectInSpaceRenderingContext;
			context2.disableShowingLabelForThisObject = true;

			//var cameraSettings = context.Owner.CameraSettings;

			//!!!!

			//foreach( var objectMeshIndex in sector.ObjectsMesh )
			//{
			//	ref var meshObject = ref ObjectsMesh[ objectMeshIndex ];

			//	var mesh = GetMesh( meshObject.ElementType, meshObject.Variation );
			//	ref var position = ref meshObject.SourcePosition;
			//	ref var rotation = ref meshObject.SourceRotation;
			//	ref var scale = ref meshObject.SourceScale;
			//	var transform = new Transform( position, rotation, scale );




			//	//!!!!
			//	//bool skip = false;
			//	//var maxDistance = Math.Min( VisibilityDistance, mesh.VisibilityDistance );
			//	//if( maxDistance < cameraSettings.FarClipDistance && ( cameraSettings.Position - tr.Position ).LengthSquared() > maxDistance * maxDistance )
			//	//	skip = true;
			//	//if( !skip )
			//	{
			//		var item = new Component_RenderingPipeline.RenderSceneData.MeshItem();
			//		item.Creator = this;

			//		//!!!!
			//		item.BoundingBox = new Bounds( position - new Vector3( 1, 1, 1 ), position + new Vector3( 1, 1, 1 ) );
			//		item.BoundingSphere = new Sphere( position, 1 );
			//		//item.BoundingBox = SpaceBounds.CalculatedBoundingBox;
			//		//item.BoundingSphere = SpaceBounds.CalculatedBoundingSphere;
			//		item.MeshData = mesh.Result.MeshData;

			//		item.CastShadows = CastShadows;


			//		ref var matrix = ref transform.ToMatrix4();

			//		//!!!!double
			//		matrix.ToMatrix4F( out item.MeshInstanceOne );

			//		//var meshData = new Component_RenderingPipeline.RenderSceneData.MeshItem.MeshInstanceData();
			//		//var tr = Transform.Value;
			//		//meshData.Position = tr.Position;
			//		//meshData.Rotation = tr.Rotation.ToQuaternionF();
			//		//meshData.Scale = tr.Scale.ToVector3F();
			//		//item.MeshInstanceOne = meshData;

			//		context.FrameData.RenderSceneData.Meshes.Add( ref item );

			//	}
			//}
		}

		//public Bounds CalculateTotalBounds()
		//{
		//	Bounds result = Bounds.Cleared;

		//	foreach( var sector in sectors.Values )
		//	{
		//		if( sector.ObjectsBounds != null )
		//			result.Add( sector.ObjectsBounds.CalculatedBoundingBox );
		//	}
		//	return result;
		//}

		private void Scene_GetRenderSceneData( Component_Scene scene, ViewportRenderingContext context )
		{
			var context2 = context.objectInSpaceRenderingContext;

			if( context2.selectedObjects.Contains( this ) )
			{
				var bounds = CalculateBounds();// CalculateTotalBounds();
				if( !bounds.IsCleared() )
				{
					if( context.Owner.Simple3DRenderer != null )
					{
						ColorValue color = ProjectSettings.Get.SelectedColor;
						context.Owner.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
						RendererUtility.AddBoundsSegmented( context.Owner.Simple3DRenderer, bounds );
					}
				}
			}
		}

		//void UpdateSectors( ESet<Tile> sectorsToUpdate )
		//{
		//	foreach( var sector in sectorsToUpdate )
		//		sector.Update();
		//}

		void CreateTiles( bool updateHoles = false )
		{
			//!!!!где еще такое полезно?
			if( !EnabledInHierarchyAndIsNotResource )
				return;

			DestroyTiles();

			//!!!!так? где еще
			CalculateCachedGeneralData();

			//calculate tile count
			tileCount = heightmapSize / tileSize;
			if( heightmapSize % tileSize != 0 )
				Log.Fatal( "HeightmapTerrain: GenerateTiles: heightmapSize % tileSize != 0." );

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
				tile.UpdateRenderingData( true, calculatedPositionsForGetNormals, updateHoles, true );


			//ElementTypesCacheNeedUpdate();

			//var sectorsToUpdate = new ESet<Tile>();

			////ObjectsMesh
			//{
			//	int capacity = ObjectsMeshGetCapacity();
			//	for( int index = 0; index < capacity; index++ )
			//	{
			//		//check is removed
			//		if( objectsMeshRemovedObjects != null && objectsMeshRemovedObjects.Data[ index ] )
			//			continue;

			//		ObjectsMeshAddToSectors( index, sectorsToUpdate );
			//	}
			//}

			//UpdateSectors( sectorsToUpdate );
		}

		void DestroyTiles()
		{
			//!!!!дождаться трединга в секторах

			if( tiles != null )
			{
				int count = tiles.GetLength( 0 );
				for( int tileY = 0; tileY < count; tileY++ )
					for( int tileX = 0; tileX < count; tileX++ )
						tiles[ tileX, tileY ].Destroy();
				tiles = null;
			}

			//DisposeSharedVertexDatas();
			//DisposeSharedIndexDatas();

			//foreach( var sector in sectors.Values )
			//	sector.Destroy();
			//sectors.Clear();
		}

		//void DestroyHeightmapBuffer()
		//{
		//	heightmapBuffer = null;
		//}

		//public float GetHeightCoefficient( Vector2I index )
		//{
		//   ushort v = heightmapBuffer[ index.X, heightmapSize - index.Y ];
		//   return (float)v * ( 1.0f / 65535.0f );
		//}

		public delegate void GetHeightWithoutPositionOverrideDelegate( Component_Terrain sender, Vector2I index, ref float value );
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

		//public ushort GetHeight16Bit( Vector2I index )
		//{
		//   return heightmapBuffer[ index.X, heightmapSize - index.Y ];
		//}

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

		public Vector2I GetCellIndexByPosition( Vector2 pos )
		{
			if( cachedCellSize == 0 )
				CalculateCachedGeneralData();
			Vector2 floatIndex = ( pos - cachedBounds.Minimum.ToVector2() ) * cachedCellSizeInv;
			return new Vector2I( (int)floatIndex.X, (int)floatIndex.Y );
		}

		public Vector2I GetMaskIndexByPosition( Vector2 pos )
		{
			if( cachedCellSize == 0 )
				CalculateCachedGeneralData();
			Vector2 floatIndex = ( pos - cachedBounds.Minimum.ToVector2() ) * cachedMaskCellSizeInv;
			return new Vector2I( (int)floatIndex.X, (int)floatIndex.Y );
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

		//public void ClampAOIndex( ref Vector2I aoIndex )
		//{
		//	if( aoIndex.X < 0 ) aoIndex.X = 0;
		//	if( aoIndex.X >= ambientOcclusion.size ) aoIndex.X = ambientOcclusion.size - 1;
		//	if( aoIndex.Y < 0 ) aoIndex.Y = 0;
		//	if( aoIndex.Y >= ambientOcclusion.size ) aoIndex.Y = ambientOcclusion.size - 1;
		//}

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

		//Vector2I GetTileIndexByAOIndex( Vector2I index )
		//{
		//	return index * tileCount / ambientOcclusion.size;
		//}

		Vector2I GetMaskIndexByCellIndex( Vector2I index )
		{
			return ( index * cachedPaintMaskSize ) / heightmapSize;
		}

		//Vector2I GetAOIndexByCellIndex( Vector2I index )
		//{
		//	return ( index * ambientOcclusion.size ) / heightmapSize;
		//}

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

		//public void SetHeightCoefficient( Vector2I index, float coef )
		//{
		//   float v = coef * 65535.0f + .5f;
		//   heightmapBuffer[ index.X, heightmapSize - index.Y ] = (ushort)v;
		//   SetModified( true );
		//}

		//public void SetHeight16Bit( Vector2I index, ushort value )
		//{
		//   heightmapBuffer[ index.X, heightmapSize - index.Y ] = (ushort)value;
		//   SetModified( true );
		//}

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
		/// <returns>if hole, Single.MinValue will be returned.</returns>
		public double GetHeight( Vector2 position, bool considerHoles )
		{
			Vector2I index = GetCellIndexByPosition( position );

			GetPositionWithClamp( index, out var p0 );

			//!!!!considerHoles

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
				Vector3 pvec = Vector3.Cross( ray.Direction, edge2 );
				double det = Vector3.Dot( edge1, pvec );
				Vector3 tvec = ray.Origin - p0;
				Vector3 qvec = Vector3.Cross( tvec, edge1 );
				double t = Vector3.Dot( edge2, qvec ) * ( 1.0f / det );
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

		public Vector3F GetNormal( Vector2 pos )
		{
			//!!!!

			Vector2I cellIndex = GetCellIndexByPosition( pos );
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
			var scene = FindParent<Component_Scene>();
			if( scene != null )
			{
				//!!!!фильтр
				//!!!!маски в менеджере сцены
				var item = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, ray );
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

		//public void UpdateCollision( RectangleI updatedCellIndices )
		//{
		//	if( PhysicsWorld.Instance.IsHeightFieldShapeSupported )
		//	{
		//		if( heightFieldBody != null )
		//			UpdateHeightFieldBody();
		//	}
		//	else
		//	{
		//		if( bodies != null )
		//		{
		//			const int border = 2;

		//			Vector2I bodyIndexMin = GetBodyIndexByCellIndex( updatedCellIndices.Minimum -
		//				new Vector2I( border, border ) );
		//			Vector2I bodyIndexMax = GetBodyIndexByCellIndex( updatedCellIndices.Maximum +
		//				new Vector2I( border, border ) );
		//			ClampBodyIndex( ref bodyIndexMin );
		//			ClampBodyIndex( ref bodyIndexMax );

		//			for( int y = bodyIndexMin.Y; y <= bodyIndexMax.Y; y++ )
		//				for( int x = bodyIndexMin.X; x <= bodyIndexMax.X; x++ )
		//					UpdateCollisionBody( new Vector2I( x, y ) );
		//		}
		//	}
		//}

		//void ReloadLayersTextures()
		//{
		//	foreach( HeightmapTerrain.Layer layer in Layers )
		//	{
		//		for( int n = 0; n < 6; n++ )
		//		{
		//			string textureName = "";
		//			switch( n )
		//			{
		//			case 0: textureName = layer.BaseMapGetFullPath(); break;
		//			case 1: textureName = layer.DetailMapGetFullPath(); break;
		//			case 2: textureName = layer.BaseNormalMapGetFullPath(); break;
		//			case 3: textureName = layer.DetailNormalMapGetFullPath(); break;
		//			case 4: textureName = layer.BaseSpecularMapGetFullPath(); break;
		//			case 5: textureName = layer.DetailSpecularMapGetFullPath(); break;
		//			}

		//			if( !string.IsNullOrEmpty( textureName ) )
		//			{
		//				TextureManager.Instance.Unload( textureName );
		//				TextureManager.Instance.Load( textureName );
		//			}
		//		}
		//	}
		//}

		//protected override void OnRenderFrame()
		//{
		//	base.OnRenderFrame();

		//	if( loadingStatus == LoadingStatuses.Loading )
		//		StreamingLoadUpdate();
		//	if( streamingEnable )
		//		StreamingEnableUpdate();

		//	if( Enabled )
		//	{
		//		if( needUpdateAfterDeviceLost )
		//		{
		//			UpdateNormalsHeightTextures( new RectangleI( Vector2I.Zero, new Vector2I( heightmapSize - 1, heightmapSize - 1 ) ), false );
		//			UpdateHoleTextures( new RectangleI( Vector2I.Zero, new Vector2I( heightmapSize - 1, heightmapSize - 1 ) ), false );

		//			ClearMasksTextures();
		//			UpdateMasksTextures( Vector2I.Zero, new Vector2I( masksSize, masksSize ), false );
		//		}

		//		if( needUpdateAOTextures )
		//		{
		//			if( aoTextures == null )
		//				CreateAOTextures();
		//			ClearsAOTextures();
		//			UpdateAOTextures( false );
		//			needUpdateAOTextures = false;
		//		}

		//		if( needReloadLayersTextures && !layerCollectionEditorShow )
		//		{
		//			needReloadLayersTextures = false;

		//			DestroyGeneratedMaterials();
		//			ReloadLayersTextures();
		//			SetNeedUpdateGeneratedMaterials();
		//		}

		//		needUpdateAfterDeviceLost = false;

		//		if( !layerCollectionEditorShow )
		//		{
		//			if( needUpdateGeneratedMaterials )
		//				UpdateGeneratedMaterials();
		//			if( needUpdateGpuParametersOfGeneratedMaterials )
		//				UpdateGpuParametersOfGeneratedMaterials();
		//		}
		//	}
		//}

		//protected override void OnRender( Camera camera )
		//{
		//	base.OnRender( camera );

		//	//debug draw physics
		//	if( EngineDebugSettings.DrawStaticPhysics && camera.Purpose == Camera.Purposes.MainCamera )
		//		DoEditorSelectionDebugRender( camera, false );

		//	//if( camera.Purpose == Camera.Purposes.MainCamera && lodSettings.Enabled && lodSettings.ShowLevels )
		//	//   RenderLODLevels( camera );
		//}

		//public void DoEditorSelectionDebugRender( Camera camera, bool depthTest )
		//{
		//	if( !depthTest )
		//		camera.DebugGeometry.SetSpecialDepthSettings( false, true );

		//	if( heightFieldBody != null )
		//	{
		//		Vector3 cameraPosition = camera.Position;
		//		float farClipDistance = camera.FarClipDistance;

		//		float distanceSqr = heightFieldBody.GetGlobalBounds().GetPointDistanceSqr( cameraPosition );
		//		if( distanceSqr < farClipDistance * farClipDistance )
		//		{
		//			HeightFieldShape shape = (HeightFieldShape)heightFieldBody.Shapes[ 0 ];

		//			Vector2I cellIndex = GetCellIndexByPosition( camera.Position.ToVector2() );

		//			const int size = 100;
		//			Vector2I min = cellIndex - new Vector2I( size, size );
		//			Vector2I max = cellIndex + new Vector2I( size, size );
		//			min.Clamp( Vector2I.Zero, shape.SampleCount - new Vector2I( 1, 1 ) );
		//			max.Clamp( Vector2I.Zero, shape.SampleCount - new Vector2I( 1, 1 ) );

		//			if( max.X > min.X && max.Y > min.Y )
		//			{
		//				RectangleI sampleRange = new RectangleI( min, max );

		//				Vector3[] vertices;
		//				int[] indices;
		//				shape.GetVerticesAndIndices( false, sampleRange, out vertices, out indices );

		//				Mat4 transform = heightFieldBody.GetTransform();
		//				if( !shape.IsIdentityTransform )
		//					transform *= shape.GetTransform();
		//				camera.DebugGeometry.Color = new ColorValue( 0, 0, 1, .5f );
		//				camera.DebugGeometry.AddVertexIndexBuffer( vertices, indices, transform, true, true );
		//			}
		//		}

		//		//heightFieldBody.DebugRender( camera.DebugGeometry, 0, .5f, true, false, ColorValue.Zero );
		//	}

		//	if( bodies != null )
		//	{
		//		Vector3 cameraPosition = camera.Position;
		//		float farClipDistance = camera.FarClipDistance;

		//		int bodyCount = GetBodyCountByAxis();
		//		for( int y = 0; y < bodyCount; y++ )
		//		{
		//			for( int x = 0; x < bodyCount; x++ )
		//			{
		//				Body body = bodies[ x, y ];
		//				if( body != null )
		//				{
		//					float distanceSqr = body.GetGlobalBounds().GetPointDistanceSqr( cameraPosition );
		//					if( distanceSqr < farClipDistance * farClipDistance )
		//					{
		//						const float drawAsBoundsDistance = 100;
		//						if( distanceSqr < drawAsBoundsDistance * drawAsBoundsDistance )
		//						{
		//							body.DebugRender( camera.DebugGeometry, 0, .5f, true, false, ColorValue.Zero );
		//						}
		//						else
		//						{
		//							camera.DebugGeometry.Color = new ColorValue( 0, 0, 1, .5f );
		//							camera.DebugGeometry.AddBounds( body.GetGlobalBounds() );
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}

		//	if( !depthTest )
		//		camera.DebugGeometry.RestoreDefaultDepthSettings();
		//}

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

		//void CalculateNeedForMasksFields()
		//{
		//	needMaskPixelsForTile = ( tileSize * masksSize ) / heightmapSize + 3;

		//	masksTextureSize = GetDefaultSizeForGeneratedTextures();
		//	while( needMaskPixelsForTile > masksTextureSize )
		//		masksTextureSize *= 2;
		//	masksTextureSizeInv = 1.0f / (float)masksTextureSize;

		//	tilesCountPerMasksTexture = masksTextureSize / needMaskPixelsForTile;

		//	masksTexturesCount = tileCount / tilesCountPerMasksTexture;
		//	if( tileCount % tilesCountPerMasksTexture != 0 )
		//		masksTexturesCount++;

		//	if( GetMasksTextureIndexByTileIndex( new Vector2I( tileCount - 1, 0 ) ).X >= masksTexturesCount )
		//	{
		//		Log.Fatal( "HeightmapTerrain: CalculateNeedForMasksFields: " +
		//			"GetMasksTextureIndexByTileIndex( new Vector2I( tileCount - 1, 0 ) ).X >= " +
		//			"masksTexturesCount." );
		//	}
		//}

		//void CalculateNeedForAOFields()
		//{
		//	needAOPixelsForTile = ( tileSize * ambientOcclusion.size ) / heightmapSize + 3;

		//	aoTextureSize = GetDefaultSizeForGeneratedTextures();
		//	while( needAOPixelsForTile > aoTextureSize )
		//		aoTextureSize *= 2;
		//	aoTextureSizeInv = 1.0f / (float)aoTextureSize;
		//	tilesCountPerAOTexture = aoTextureSize / needAOPixelsForTile;
		//	aoTexturesCount = tileCount / tilesCountPerAOTexture;
		//	if( tileCount % tilesCountPerAOTexture != 0 )
		//		aoTexturesCount++;

		//	if( GetAOTextureIndexByTileIndex( new Vector2I( tileCount - 1, 0 ) ).X >= aoTexturesCount )
		//	{
		//		Log.Fatal( "HeightmapTerrain: CalculateNeedForAmbientOcclusionFields: " +
		//			"GetAmbientOcclusionTextureIndexByTileIndex( new Vector2I( tileCount - 1, 0 ) ).X >= " +
		//			"ambientOcclusionTexturesCount." );
		//	}
		//}

		//static string GetFixedLayerFileName( string name )
		//{
		//	char[] invalidChars = Path.GetInvalidFileNameChars();

		//	string trimmedName = name.Trim();
		//	StringBuilder builder = new StringBuilder();
		//	foreach( char c in trimmedName )
		//	{
		//		char fixedChar = c;
		//		if( Array.IndexOf<char>( invalidChars, fixedChar ) != -1 )
		//			fixedChar = '_';
		//		builder.Append( fixedChar );
		//	}
		//	return builder.ToString();
		//}

		//bool LoadMasks( bool fromStreamingThread )
		//{
		//	if( !fromStreamingThread )
		//		LongOperationCallbackManager.CallCallback( "HeightmapTerrain: LoadMasks" );

		//	string fullDataDirectory = Path.Combine( Map.Instance.GetVirtualFileDirectory(),
		//		dataDirectory );

		//	for( int n = 1; n < layers.Count; n++ )
		//	{
		//		Layer layer = layers[ n ];

		//		layer.PrepareMask();

		//		string fileName = Path.Combine( fullDataDirectory,
		//			string.Format( "LayerMask_{0}.png", GetFixedLayerFileName( layer.Name ) ) );
		//		if( !VirtualFile.Exists( fileName ) )
		//			fileName = Path.Combine( fullDataDirectory, string.Format( "Layer{0}Mask.png", n ) );

		//		//if( !VirtualFile.Exists( fileName ) )
		//		//{
		//		//   Log.Error( "HeightmapTerrain: Layer mask file not exists (Layer: {0}) ({1}).",
		//		//      layer.Name, fileName );
		//		//   return false;
		//		//}

		//		if( VirtualFile.Exists( fileName ) )
		//		{
		//			if( !fromStreamingThread )
		//				LongOperationCallbackManager.CallCallback( "HeightmapTerrain: LoadMasks: Load mask: " + fileName );

		//			try
		//			{
		//				bool loaded = false;
		//				byte[] data = null;
		//				Vector2I size = Vector2I.Zero;
		//				PixelFormat format = PixelFormat.Unknown;
		//				string error = null;

		//				if( fromStreamingThread )
		//				{
		//					try
		//					{
		//						Bitmap bitmap = new Bitmap( VirtualFileSystem.GetRealPathByVirtual( fileName ) );
		//						loaded = true;
		//						size = new Vector2I( bitmap.Size.Width, bitmap.Size.Height );
		//						data = new byte[ size.X * size.Y ];
		//						for( int y = 0; y < size.Y; y++ )
		//							for( int x = 0; x < size.X; x++ )
		//								data[ y * size.X + x ] = bitmap.GetPixel( x, y ).R;
		//						format = PixelFormat.L8;
		//						error = null;


		//						//bitmap.GetPixel(
		//						//Bitmap bitmap = new Bitmap(
		//						//Bitmap bitmap = new Bitmap(@"C:\image.png");
		//						//Color clr = bitmap.GetPixel(0, 0);

		//						//using( Stream stream = VirtualFile.Open( fileName ) )
		//						//{
		//						//PngBitmapDecoder decoder = new PngBitmapDecoder( stream, BitmapCreateOptions.PreservePixelFormat,
		//						//   BitmapCacheOption.Default );

		//						//BitmapFrame frame = decoder.Frames[ 0 ];

		//						//if( frame.Format.BitsPerPixel != 8 )
		//						//   throw new Exception( "frame.Format.BitsPerPixel != 8" );

		//						//loaded = true;
		//						//size = new Vector2I( frame.PixelWidth, frame.PixelHeight );
		//						//data = new byte[ size.X * size.Y ];
		//						//frame.CopyPixels( data, frame.PixelWidth, 0 );
		//						//format = PixelFormat.L8;
		//						//error = null;
		//						//}
		//					}
		//					catch( Exception e )
		//					{
		//						loaded = false;
		//						error = string.Format( "Masks buffer loading failed: {0} ({1}).", e.Message, fileName );
		//					}
		//				}
		//				else
		//				{
		//					int depth;
		//					int numFaces;
		//					int numMipmaps;
		//					loaded = ImageManager.LoadFromFile( fileName, out data, out size, out depth, out format,
		//						out numFaces, out numMipmaps, out error );
		//				}
		//				if( !loaded )
		//				{
		//					Log.Error( "HeightmapTerrain: " + error );
		//					continue;
		//				}

		//				if( size.X != masksSize || size.Y != masksSize )
		//				{
		//					Log.Error( "HeightmapTerrain: Invalid size of a layer mask  (Layer: {0}) ({1}).",
		//						layer.Name, fileName );
		//					continue;
		//				}

		//				if( format != PixelFormat.L8 && format != PixelFormat.A8 &&
		//					format != PixelFormat.ByteL && format != PixelFormat.ByteA )
		//				{
		//					Log.Error( "HeightmapTerrain: Invalid format of a layer mask (Layer: {0}) ({1}).",
		//						layer.Name, fileName );
		//					continue;
		//				}

		//				for( int y = 0; y < masksSize; y++ )
		//					for( int x = 0; x < masksSize; x++ )
		//						layer.mask[ x, y ] = data[ y * masksSize + x ];
		//			}
		//			catch( Exception e )
		//			{
		//				Log.Error( "HeightmapTerrain: Load mask failed \"{0}\" ({1}).", fileName, e.Message );
		//			}
		//		}
		//	}

		//	return true;
		//}

		//void SaveMasks()
		//{
		//	try
		//	{
		//		string mapRealFileDirectory = VirtualFileSystem.GetRealPathByVirtual(
		//			Map.Instance.GetVirtualFileDirectory() );
		//		string realFullDataDirectory = Path.Combine( mapRealFileDirectory, dataDirectory );

		//		byte[] data = new byte[ masksSize * masksSize ];

		//		List<string> usedFileNamesLowerCase = new List<string>();

		//		for( int n = 1; n < layers.Count; n++ )
		//		{
		//			Layer layer = layers[ n ];

		//			if( layer.mask == null )
		//				continue;

		//			for( int y = 0; y < masksSize; y++ )
		//				for( int x = 0; x < masksSize; x++ )
		//					data[ y * masksSize + x ] = layer.mask[ x, y ];

		//			string name = string.Format( "LayerMask_{0}.png", GetFixedLayerFileName( layer.Name ) );
		//			usedFileNamesLowerCase.Add( name.ToLower() );
		//			string realFileName = Path.Combine( realFullDataDirectory, name );
		//			//string realFileName = Path.Combine( realFullDataDirectory,
		//			//   string.Format( "LayerMask_{0}.png", GetFixedLayerFileName( layer.Name ) ) );
		//			//string realFileName = Path.Combine( realFullDataDirectory, string.Format( "Layer{0}Mask.png", n ) );
		//			string error;
		//			if( !ImageManager.Save( realFileName, data, new Vector2I( masksSize, masksSize ),
		//				1, PixelFormat.A8, 1, 0, out error ) )
		//			{
		//				throw new Exception( error );
		//			}
		//		}

		//		//Delete old files
		//		{
		//			string[] paths = Directory.GetFiles( realFullDataDirectory, "LayerMask_*.png", SearchOption.TopDirectoryOnly );
		//			foreach( string path in paths )
		//			{
		//				string fileNameLowerCase = Path.GetFileName( path ).ToLower();
		//				bool found = false;
		//				foreach( string name in usedFileNamesLowerCase )
		//				{
		//					if( name == fileNameLowerCase )
		//						found = true;
		//				}
		//				if( !found )
		//					File.Delete( path );
		//			}
		//		}

		//		//Delete old files (old format compatibility)
		//		for( int n = 1; n < 100; n++ )
		//		{
		//			string realFileName = Path.Combine( realFullDataDirectory, string.Format( "Layer{0}Mask.png", n ) );
		//			if( File.Exists( realFileName ) )
		//				File.Delete( realFileName );
		//		}
		//	}
		//	catch( Exception e )
		//	{
		//		Log.Error( "HeightmapTerrain: Save mask failed ({0}).", e.Message );
		//	}
		//}

		//void CreateMasksTextures( bool clearMasksTextures )
		//{
		//	DestroyMasksTextures();

		//	CalculateNeedForMasksFields();

		//	Texture.Usage usage;
		//	if( EntitySystemWorld.Instance.IsEditor() )
		//		usage = Texture.Usage.DynamicWriteOnly;
		//	else
		//		usage = Texture.Usage.StaticWriteOnly;

		//	masksTextures = new MaskTextureItem[ masksTexturesCount, masksTexturesCount ];

		//	for( int y = 0; y < masksTexturesCount; y++ )
		//	{
		//		for( int x = 0; x < masksTexturesCount; x++ )
		//		{
		//			MaskTextureItem textureItem = new MaskTextureItem();

		//			if( !IsFixedPipelineFallback() )
		//			{
		//				string textureName = TextureManager.Instance.GetUniqueName( "_GeneratedHeightmapTerrainMasks" );
		//				textureItem.shaderTexture = CreateTextureAsTextureData( textureName,
		//					new Vector2I( masksTextureSize, masksTextureSize ), PixelFormat.A8R8G8B8, usage );
		//			}
		//			else
		//			{
		//				textureItem.ffpTextures = new TextureData[ 5 ];
		//				for( int n = 0; n < 5; n++ )
		//				{
		//					string textureName = TextureManager.Instance.GetUniqueName( "_GeneratedHeightmapTerrainMasks" );
		//					textureItem.ffpTextures[ n ] = CreateTextureAsTextureData( textureName,
		//						new Vector2I( masksTextureSize, masksTextureSize ), PixelFormat.ByteLA, usage );
		//				}
		//			}

		//			masksTextures[ x, y ] = textureItem;
		//		}
		//	}

		//	if( clearMasksTextures )
		//		ClearMasksTextures();
		//}

		//unsafe void ClearMasksTextureItem( MaskTextureItem textureItem )
		//{
		//	int textureCount = IsFixedPipelineFallback() ? 5 : 1;
		//	for( int nTexture = 0; nTexture < textureCount; nTexture++ )
		//	{
		//		TextureData textureData;
		//		if( !IsFixedPipelineFallback() )
		//			textureData = textureItem.shaderTexture;
		//		else
		//			textureData = textureItem.ffpTextures[ nTexture ];

		//		if( textureData != null )
		//		{
		//			HardwarePixelBuffer buffer = textureData.texture.GetBuffer();
		//			IntPtr pointer = buffer.Lock( HardwareBuffer.LockOptions.Normal );
		//			PixelBox pixelBox = buffer.GetCurrentLock();
		//			int bytesPerPixel = PixelFormatUtils.GetNumElemBytes( textureData.texture.Format );
		//			NativeUtils.FillMemory( pointer, pixelBox.SlicePitch * bytesPerPixel, 255 );
		//			buffer.Unlock();
		//		}
		//	}
		//}

		//void ClearMasksTextures()
		//{
		//	if( masksTextures == null )
		//		return;

		//	int count = masksTextures.GetLength( 0 );
		//	for( int y = 0; y < count; y++ )
		//		for( int x = 0; x < count; x++ )
		//			ClearMasksTextureItem( masksTextures[ x, y ] );
		//}

		//unsafe void UpdateMasksTextures( Vector2I maskIndexMin, Vector2I maskIndexMax, bool fromStreamingThread )
		//{
		//	if( !fromStreamingThread )
		//		LongOperationCallbackManager.CallCallback( "HeightmapTerrain: UpdateMasksTextures" );

		//	if( masksTextures == null )
		//		return;

		//	Vector2I tileIndexMin = GetTileIndexByMaskIndex( maskIndexMin );
		//	Vector2I tileIndexMax = GetTileIndexByMaskIndex( maskIndexMax );
		//	ClampTileIndex( ref tileIndexMin );
		//	ClampTileIndex( ref tileIndexMax );

		//	List<Vector2I> needUpdateList = new List<Vector2I>( 16 );
		//	{
		//		GetTiles( tileIndexMin, tileIndexMax, delegate ( Tile tile )
		//		{
		//			Vector2I masksTextureIndex = GetMasksTextureIndexByTileIndex( tile.index );

		//			if( !needUpdateList.Contains( masksTextureIndex ) )
		//				needUpdateList.Add( masksTextureIndex );
		//		} );
		//	}

		//	foreach( Vector2I masksTextureIndex in needUpdateList )
		//	{
		//		MaskTextureItem textureItem = masksTextures[ masksTextureIndex.X, masksTextureIndex.Y ];

		//		int textureCount = IsFixedPipelineFallback() ? 5 : 1;
		//		for( int nTexture = 0; nTexture < textureCount; nTexture++ )
		//		{
		//			TextureData textureData;
		//			if( !IsFixedPipelineFallback() )
		//				textureData = textureItem.shaderTexture;
		//			else
		//				textureData = textureItem.ffpTextures[ nTexture ];
		//			if( textureData == null )
		//				continue;

		//			HardwarePixelBuffer buffer = null;
		//			byte* pointer;
		//			if( fromStreamingThread )
		//			{
		//				IntPtr data = NativeUtils.Alloc( NativeMemoryAllocationType.Renderer, textureData.bufferSizeInBytes );
		//				//clear mask textures (ClearMasksTextures())
		//				NativeUtils.FillMemory( data, textureData.bufferSizeInBytes, 255 );

		//				streamingEnableDataForTextures.Add( textureData, data );
		//				pointer = (byte*)data;
		//			}
		//			else
		//			{
		//				buffer = textureData.texture.GetBuffer();
		//				pointer = (byte*)buffer.Lock( HardwareBuffer.LockOptions.Normal );
		//			}
		//			//HardwarePixelBuffer buffer = textureData.texture.GetBuffer();
		//			//buffer.Lock( HardwareBuffer.LockOptions.Normal );

		//			//PixelBox pixelBox = buffer.GetCurrentLock();

		//			Vector2I tileMin = masksTextureIndex * tilesCountPerMasksTexture;
		//			Vector2I tileMax = ( masksTextureIndex + new Vector2I( 1, 1 ) ) * tilesCountPerMasksTexture;
		//			ClampTileIndex( ref tileMin );
		//			ClampTileIndex( ref tileMax );

		//			GetTiles( tileMin, tileMax, delegate ( Tile tile )
		//			{
		//				if( !fromStreamingThread )
		//				{
		//					LongOperationCallbackManager.CallCallback(
		//						"HeightmapTerrain: UpdateMasksTextures: Tile: " + tile.index.ToString() );
		//				}

		//				if( GetMasksTextureIndexByTileIndex( tile.index ) != masksTextureIndex )
		//					return;

		//				Vector2I tileIndexInMasksTexture = tile.index - masksTextureIndex * tilesCountPerMasksTexture;
		//				Vector2I tileMasksTextureCoordMin = tileIndexInMasksTexture * needMaskPixelsForTile +
		//					new Vector2I( 1, 1 );

		//				//fill
		//				unsafe
		//				{
		//					//byte* pointer = (byte*)buffer.GetUnmanagedLockPointer();

		//					Vector2I tileMaskIndexMin = GetMaskIndexByCellIndex( tile.cellIndexMin );
		//					Vector2I tileMaskIndexMax = GetMaskIndexByCellIndex( tile.cellIndexMax );

		//					//loop with border

		//					for( int maskY = tileMaskIndexMin.Y - 1; maskY <= tileMaskIndexMax.Y + 1; maskY++ )
		//					{
		//						for( int maskX = tileMaskIndexMin.X - 1; maskX <= tileMaskIndexMax.X + 1; maskX++ )
		//						{
		//							if( maskX < maskIndexMin.X || maskX > maskIndexMax.X )
		//								continue;
		//							if( maskY < maskIndexMin.Y || maskY > maskIndexMax.Y )
		//								continue;

		//							Vector2I masksIndex = new Vector2I( maskX, maskY );
		//							ClampMaskIndex( ref masksIndex );

		//							Vector2I masksTextureCoord = tileMasksTextureCoordMin +
		//								new Vector2I( maskX, maskY ) - tileMaskIndexMin;

		//							if( masksTextureCoord.X < 0 || masksTextureCoord.X >= masksTextureSize )
		//							{
		//								Log.Fatal( "HeightmapTerrain: UpdateMasksTextures: masksTextureCoord" +
		//									".X < 0 || masksTextureCoord.X >= masksTextureSize." );
		//							}
		//							if( masksTextureCoord.Y < 0 || masksTextureCoord.Y >= masksTextureSize )
		//							{
		//								Log.Fatal( "HeightmapTerrain: UpdateMasksTextures: masksTextureCoord" +
		//									".Y < 0 || masksTextureCoord.Y >= masksTextureSize." );
		//							}

		//							byte maskLayer0;
		//							byte maskLayer1;
		//							byte maskLayer2;
		//							byte maskLayer3;
		//							tile.CalculateMaskPixel( masksIndex,
		//								out maskLayer0, out maskLayer1, out maskLayer2, out maskLayer3 );

		//							if( !IsFixedPipelineFallback() )
		//							{
		//								byte* p = pointer + ( masksTextureCoord.Y * textureData.bufferRowPitch + masksTextureCoord.X ) * 4;
		//								//byte* p = pointer + ( masksTextureCoord.Y * pixelBox.RowPitch + masksTextureCoord.X ) * 4;
		//								if( textureData.format == PixelFormat.A8B8G8R8 )//  pixelBox.Format == PixelFormat.A8B8G8R8 )
		//								{
		//									//android new. true?
		//									*( p + 2 ) = maskLayer3;
		//									*( p + 1 ) = maskLayer2;
		//									*( p + 0 ) = maskLayer1;
		//									*( p + 3 ) = maskLayer0;
		//								}
		//								else
		//								{
		//									*( p + 0 ) = maskLayer3;
		//									*( p + 1 ) = maskLayer2;
		//									*( p + 2 ) = maskLayer1;
		//									*( p + 3 ) = maskLayer0;
		//								}
		//							}
		//							else
		//							{
		//								byte mask = 0;
		//								switch( nTexture )
		//								{
		//								case 0: mask = maskLayer0; break;
		//								case 1: mask = maskLayer1; break;
		//								case 2: mask = maskLayer2; break;
		//								case 3: mask = maskLayer3; break;
		//								case 4: mask = (byte)( 255 - ( maskLayer0 + maskLayer1 + maskLayer2 + maskLayer3 ) ); break;
		//								}

		//								byte* p = pointer + ( masksTextureCoord.Y * textureData.bufferRowPitch + masksTextureCoord.X ) * 2;
		//								//byte* p = pointer + ( masksTextureCoord.Y * pixelBox.RowPitch + masksTextureCoord.X ) * 2;
		//								*( p + 0 ) = mask;
		//								*( p + 1 ) = 255;
		//							}
		//						}
		//					}
		//				}
		//			} );

		//			if( buffer != null )
		//				buffer.Unlock();
		//		}
		//	}
		//}

		//void DestroyMasksTextures()
		//{
		//	if( masksTextures != null )
		//	{
		//		int count = masksTextures.GetLength( 0 );
		//		for( int y = 0; y < count; y++ )
		//		{
		//			for( int x = 0; x < count; x++ )
		//			{
		//				MaskTextureItem textureItem = masksTextures[ x, y ];

		//				if( textureItem.shaderTexture != null )
		//				{
		//					textureItem.shaderTexture.texture.Dispose();
		//					textureItem.shaderTexture = null;
		//				}

		//				if( textureItem.ffpTextures != null )
		//				{
		//					foreach( TextureData textureData in textureItem.ffpTextures )
		//					{
		//						if( textureData != null )
		//							textureData.texture.Dispose();
		//					}
		//					textureItem.ffpTextures = null;
		//				}
		//			}
		//		}
		//		masksTextures = null;
		//	}

		//	masksTextureSize = 0;
		//	masksTextureSizeInv = 0;
		//	masksTexturesCount = 0;
		//	tilesCountPerMasksTexture = 0;
		//	needMaskPixelsForTile = 0;
		//}

		//bool LoadAOData( bool fromStreamingThread )
		//{
		//	if( !fromStreamingThread )
		//		LongOperationCallbackManager.CallCallback( "HeightmapTerrain: LoadAOData" );

		//	aoData = null;

		//	string fullDataDirectory = Path.Combine( Map.Instance.GetVirtualFileDirectory(),
		//		dataDirectory );
		//	string fileName = Path.Combine( fullDataDirectory, "AmbientOcclusion.raw" );

		//	if( VirtualFile.Exists( fileName ) )
		//	{
		//		try
		//		{
		//			using( Stream stream = VirtualFile.Open( fileName ) )
		//			{
		//				int fileSize = (int)stream.Length;
		//				if( ambientOcclusion.size != (int)Math.Sqrt( fileSize ) )
		//				{
		//					Log.Error( "Invalid ambient occlusion map size \"{0}\". Must be a \"{1}\".",
		//						(int)Math.Sqrt( fileSize ), ambientOcclusion.size );
		//					return false;
		//				}

		//				aoData = new byte[ ambientOcclusion.size * ambientOcclusion.size ];

		//				if( stream.Read( aoData, 0, aoData.Length ) != aoData.Length )
		//					throw new Exception( "Reading error" );
		//			}
		//		}
		//		catch( Exception e )
		//		{
		//			aoData = null;
		//			Log.Error( string.Format( "HeightmapTerrain: Ambient occlusion map loading failed: " +
		//				"{0} ({1}).", e.Message, fileName ) );
		//			return false;
		//		}
		//	}

		//	return true;
		//}

		//void SaveAOData()
		//{
		//	string mapRealFileDirectory = VirtualFileSystem.GetRealPathByVirtual(
		//		Map.Instance.GetVirtualFileDirectory() );
		//	string realFullDataDirectory = Path.Combine( mapRealFileDirectory, dataDirectory );

		//	string realFileName = Path.Combine( realFullDataDirectory, "AmbientOcclusion.raw" );

		//	if( AOIsEnabledAndValid() )
		//	{
		//		try
		//		{
		//			using( FileStream stream = new FileStream( realFileName, FileMode.Create ) )
		//			{
		//				stream.Write( aoData, 0, aoData.Length );
		//			}
		//		}
		//		catch( Exception e )
		//		{
		//			Log.Error( "HeightmapTerrain: Save ambient occlusion map failed \"{0}\" ({1}).", realFileName,
		//				e.Message );
		//		}
		//	}
		//	else
		//	{
		//		if( File.Exists( realFileName ) )
		//		{
		//			try
		//			{
		//				File.Delete( realFileName );
		//			}
		//			catch( Exception e )
		//			{
		//				Log.Error( "HeightmapTerrain: Unable to delete file \"{0}\" ({1}).", realFileName,
		//					e.Message );
		//			}
		//		}
		//	}
		//}

		//void CreateAOTextures()
		//{
		//	DestroyAOTextures();

		//	if( !AOIsEnabledAndValid() )
		//		return;

		//	CalculateNeedForAOFields();

		//	aoTextures = new TextureData[ aoTexturesCount, aoTexturesCount ];

		//	for( int y = 0; y < aoTexturesCount; y++ )
		//	{
		//		for( int x = 0; x < aoTexturesCount; x++ )
		//		{
		//			string textureName = TextureManager.Instance.GetUniqueName( "_GeneratedHeightmapTerrainAO" );
		//			aoTextures[ x, y ] = CreateTextureAsTextureData( textureName, new Vector2I( aoTextureSize, aoTextureSize ),
		//				PixelFormat.ByteLA, Texture.Usage.StaticWriteOnly );
		//		}
		//	}

		//	ClearsAOTextures();
		//}

		//void ClearAOTexture( Texture texture )
		//{
		//	HardwarePixelBuffer buffer = texture.GetBuffer();
		//	IntPtr pointer = buffer.Lock( HardwareBuffer.LockOptions.Normal );
		//	PixelBox pixelBox = buffer.GetCurrentLock();
		//	int bytesPerPixel = PixelFormatUtils.GetNumElemBytes( texture.Format );
		//	NativeUtils.FillMemory( pointer, pixelBox.SlicePitch * bytesPerPixel, 255 );
		//	buffer.Unlock();
		//}

		//void ClearsAOTextures()
		//{
		//	if( aoTextures == null )
		//		return;

		//	for( int y = 0; y < aoTexturesCount; y++ )
		//	{
		//		for( int x = 0; x < aoTexturesCount; x++ )
		//		{
		//			TextureData textureData = aoTextures[ x, y ];
		//			if( textureData != null )
		//				ClearAOTexture( textureData.texture );
		//		}
		//	}
		//}

		//unsafe void UpdateAOTextures( bool fromStreamingThread )
		//{
		//	if( !fromStreamingThread )
		//		LongOperationCallbackManager.CallCallback( "HeightmapTerrain: UpdateAOTextures" );

		//	if( !AOIsEnabledAndValid() )
		//		return;
		//	if( aoTextures == null )
		//		return;

		//	Vector2I aoIndexMin = Vector2I.Zero;
		//	Vector2I aoIndexMax = new Vector2I( ambientOcclusion.size, ambientOcclusion.size );

		//	Vector2I tileIndexMin = GetTileIndexByAOIndex( aoIndexMin );
		//	Vector2I tileIndexMax = GetTileIndexByAOIndex( aoIndexMax );
		//	ClampTileIndex( ref tileIndexMin );
		//	ClampTileIndex( ref tileIndexMax );

		//	List<Vector2I> needUpdateList = new List<Vector2I>( 16 );
		//	{
		//		GetTiles( tileIndexMin, tileIndexMax, delegate ( Tile tile )
		//		{
		//			Vector2I aoTextureIndex = GetAOTextureIndexByTileIndex( tile.index );

		//			if( !needUpdateList.Contains( aoTextureIndex ) )
		//				needUpdateList.Add( aoTextureIndex );
		//		} );
		//	}

		//	foreach( Vector2I aoTextureIndex in needUpdateList )
		//	{
		//		TextureData textureData = aoTextures[ aoTextureIndex.X, aoTextureIndex.Y ];
		//		if( textureData == null )
		//			continue;

		//		HardwarePixelBuffer buffer = null;
		//		byte* pointer;
		//		if( fromStreamingThread )
		//		{
		//			IntPtr data = NativeUtils.Alloc( NativeMemoryAllocationType.Renderer, textureData.bufferSizeInBytes );
		//			streamingEnableDataForTextures.Add( textureData, data );
		//			pointer = (byte*)data;
		//		}
		//		else
		//		{
		//			buffer = textureData.texture.GetBuffer();
		//			pointer = (byte*)buffer.Lock( HardwareBuffer.LockOptions.Normal );
		//		}
		//		//HardwarePixelBuffer buffer = textureData.texture.GetBuffer();
		//		//buffer.Lock( HardwareBuffer.LockOptions.Normal );

		//		//PixelBox pixelBox = buffer.GetCurrentLock();

		//		Vector2I tileMin = aoTextureIndex * tilesCountPerAOTexture;
		//		Vector2I tileMax = ( aoTextureIndex + new Vector2I( 1, 1 ) ) * tilesCountPerAOTexture;
		//		ClampTileIndex( ref tileMin );
		//		ClampTileIndex( ref tileMax );

		//		GetTiles( tileMin, tileMax, delegate ( Tile tile )
		//		{
		//			if( !fromStreamingThread )
		//			{
		//				LongOperationCallbackManager.CallCallback(
		//					"HeightmapTerrain: UpdateAOTextures: Tile: " + tile.index.ToString() );
		//			}

		//			if( GetAOTextureIndexByTileIndex( tile.index ) != aoTextureIndex )
		//				return;

		//			Vector2I tileIndexInAOTexture = tile.index - aoTextureIndex * tilesCountPerAOTexture;
		//			Vector2I tileAOTextureCoordMin = tileIndexInAOTexture * needAOPixelsForTile + new Vector2I( 1, 1 );

		//			//fill
		//			//unsafe
		//			//{
		//			//byte* pointer = (byte*)buffer.GetUnmanagedLockPointer();

		//			Vector2I tileAOIndexMin = GetAOIndexByCellIndex( tile.cellIndexMin );
		//			Vector2I tileAOIndexMax = GetAOIndexByCellIndex( tile.cellIndexMax );

		//			//loop with border

		//			for( int aoY = tileAOIndexMin.Y - 1; aoY <= tileAOIndexMax.Y + 1; aoY++ )
		//			{
		//				for( int aoX = tileAOIndexMin.X - 1; aoX <= tileAOIndexMax.X + 1; aoX++ )
		//				{
		//					if( aoX < aoIndexMin.X || aoX > aoIndexMax.X )
		//						continue;
		//					if( aoY < aoIndexMin.Y || aoY > aoIndexMax.Y )
		//						continue;

		//					Vector2I aoIndex = new Vector2I( aoX, aoY );
		//					ClampAOIndex( ref aoIndex );

		//					Vector2I aoTextureCoord = tileAOTextureCoordMin +
		//						new Vector2I( aoX, aoY ) - tileAOIndexMin;

		//					if( aoTextureCoord.X < 0 || aoTextureCoord.X >= aoTextureSize )
		//					{
		//						Log.Fatal( "HeightmapTerrain: UpdateAOTextures: aoTextureCoord" +
		//							".X < 0 || aoTextureCoord.X >= aoTextureSize." );
		//					}
		//					if( aoTextureCoord.Y < 0 || aoTextureCoord.Y >= aoTextureSize )
		//					{
		//						Log.Fatal( "HeightmapTerrain: UpdateAOTextures: aoTextureCoord" +
		//							".Y < 0 || aoTextureCoord.Y >= aoTextureSize." );
		//					}

		//					byte value = aoData[ aoIndex.Y * ambientOcclusion.size + aoIndex.X ];

		//					if( ambientOcclusion.Power != 1 )
		//					{
		//						float fValue = (float)value / 255;
		//						fValue = MathFunctions.Pow( fValue, ambientOcclusion.Power );
		//						MathFunctions.Saturate( ref fValue );
		//						value = (byte)( fValue * 255 );
		//					}

		//					byte* p = pointer + ( aoTextureCoord.Y * textureData.bufferRowPitch + aoTextureCoord.X ) * 2;
		//					//byte* p = pointer + ( aoTextureCoord.Y * pixelBox.RowPitch + aoTextureCoord.X ) * 2;
		//					*( p + 0 ) = value;
		//					*( p + 1 ) = 255;
		//				}
		//			}
		//			//}
		//		} );

		//		if( buffer != null )
		//			buffer.Unlock();
		//	}
		//}

		//void DestroyAOTextures()
		//{
		//	if( aoTextures != null )
		//	{
		//		int count = aoTextures.GetLength( 0 );
		//		for( int y = 0; y < count; y++ )
		//		{
		//			for( int x = 0; x < count; x++ )
		//			{
		//				TextureData textureData = aoTextures[ x, y ];
		//				if( textureData != null )
		//					textureData.texture.Dispose();
		//			}
		//		}
		//		aoTextures = null;
		//	}

		//	aoTextureSize = 0;
		//	aoTextureSizeInv = 0;
		//	aoTexturesCount = 0;
		//	tilesCountPerAOTexture = 0;
		//	needAOPixelsForTile = 0;
		//}

		//_HeightmapTerrainMaterial GenerateMaterialForTile( Tile tile )
		//{
		//	if( tile.layers.Count == 0 )
		//		return null;

		//	if( tilesCountPerMasksTexture == 0 )
		//		CalculateNeedForMasksFields();
		//	//if( IsUseSharedVertexBuffers() && tilesCountPerNormalsTexture == 0 )
		//	if( tilesCountPerNormalsTexture == 0 )
		//		CalculateNeedForNormalsHeightTexturesFields();
		//	if( tilesCountPerHoleTexture == 0 )
		//		CalculateNeedForHoleTexturesFields();
		//	if( AOIsEnabledAndValid() && tilesCountPerAOTexture == 0 )
		//		CalculateNeedForAOFields();

		//	string materialName;
		//	{
		//		StringBuilder name = new StringBuilder( 64 );

		//		name.Append( "_GeneratedHeightmapTerrain_" );

		//		//layers
		//		foreach( Layer layer in tile.layers )
		//			name.AppendFormat( "L{0}", layer.uniqueIdentifier );

		//		//masks texture
		//		{
		//			Vector2I masksTextureIndex = GetMasksTextureIndexByTileIndex( tile.index );
		//			name.AppendFormat( "_M{0}_{1}", masksTextureIndex.X, masksTextureIndex.Y );
		//		}

		//		//normals texture
		//		{
		//			Vector2I normalsTextureIndex = GetNormalsTextureIndexByTileIndex( tile.index );
		//			name.AppendFormat( "_N{0}_{1}", normalsTextureIndex.X, normalsTextureIndex.Y );
		//		}

		//		//AO texture
		//		if( AOIsEnabledAndValid() )
		//		{
		//			Vector2I aoTextureIndex = GetAOTextureIndexByTileIndex( tile.index );
		//			name.AppendFormat( "_AO{0}_{1}", aoTextureIndex.X, aoTextureIndex.Y );
		//		}

		//		//hole texture
		//		if( tile.existsHoles )
		//		{
		//			Vector2I holeTextureIndex = GetHoleTextureIndexByTileIndex( tile.index );
		//			name.AppendFormat( "_H{0}_{1}", holeTextureIndex.X, holeTextureIndex.Y );
		//		}
		//		else
		//			name.Append( "_H_NO" );

		//		materialName = name.ToString();
		//	}

		//	_HeightmapTerrainMaterial material = (_HeightmapTerrainMaterial)
		//		HighLevelMaterialManager.Instance.GetMaterialByName( materialName );

		//	if( material != null && material.terrain != this )
		//		Log.Fatal( "HeightmapTerrain: GenerateMaterialForTile: material.terrain != this." );

		//	if( material == null )
		//	{
		//		material = (_HeightmapTerrainMaterial)
		//			HighLevelMaterialManager.Instance.CreateMaterial( materialName,
		//			typeof( _HeightmapTerrainMaterial ).Name );

		//		Vector2I masksTextureIndex = GetMasksTextureIndexByTileIndex( tile.index );
		//		Vector2I normalsTextureIndex = GetNormalsTextureIndexByTileIndex( tile.index );
		//		Vector2I ambientOcclusionTextureIndex = Vector2I.Zero;
		//		if( AOIsEnabledAndValid() )
		//			ambientOcclusionTextureIndex = GetAOTextureIndexByTileIndex( tile.index );
		//		Vector2I holeTextureIndex = Vector2I.Zero;
		//		if( tilesCountPerHoleTexture != 0 )
		//			holeTextureIndex = GetHoleTextureIndexByTileIndex( tile.index );
		//		material.Init( this, tile.layers, masksTextureIndex, normalsTextureIndex,
		//			ambientOcclusionTextureIndex, tile.existsHoles, holeTextureIndex );

		//		generatedMaterials.Add( material );

		//		material.UpdateBaseMaterial();
		//	}

		//	return material;
		//}

		//void UpdateGeneratedMaterials()
		//{
		//	if( streamingEnable )
		//		Log.Fatal( "HeightmapTerrain: UpdateGeneratedMaterials: streamingEnable == True." );

		//	if( tiles == null )
		//		return;

		//	Set<_HeightmapTerrainMaterial> updatedMaterials = new Set<_HeightmapTerrainMaterial>();

		//	GetTiles( delegate ( Tile tile )
		//	{
		//		//staticMeshObject material
		//		{
		//			_HeightmapTerrainMaterial material = GenerateMaterialForTile( tile );//, false );
		//			if( material != null )
		//			{
		//				if( !updatedMaterials.Contains( material ) )
		//				{
		//					material.UpdateBaseMaterial();
		//					updatedMaterials.Add( material );
		//				}
		//			}
		//			if( tile.staticMeshObject != null )
		//			{
		//				tile.staticMeshObject.Material = ( material != null ) ? material.BaseMaterial : null;
		//				tile.UpdateMaterialTechniquesInfo();
		//			}
		//		}

		//	} );

		//	needUpdateGeneratedMaterials = false;
		//	needUpdateGpuParametersOfGeneratedMaterials = false;
		//}

		//void UpdateGpuParametersOfGeneratedMaterials()
		//{
		//	foreach( _HeightmapTerrainMaterial material in generatedMaterials )
		//	{
		//		if( material.IsBaseMaterialInitialized() )
		//			material.UpdateGpuParametersForMaterial();
		//	}
		//	needUpdateGpuParametersOfGeneratedMaterials = false;
		//}

		//void DestroyGeneratedMaterials()
		//{
		//	//detach materials from StaticMeshObject's
		//	GetTiles( delegate ( Tile tile )
		//	{
		//		if( tile.staticMeshObject != null )
		//		{
		//			tile.staticMeshObject.Material = null;
		//			tile.staticMeshObject.ForceTechnique = null;
		//			tile.UpdateMaterialTechniquesInfo();
		//		}
		//	} );

		//	foreach( _HeightmapTerrainMaterial material in generatedMaterials )
		//		material.Dispose();
		//	generatedMaterials.Clear();
		//}

		//void RenderSystemEvent( RenderSystemEvents name )
		//{
		//	if( name == RenderSystemEvents.DeviceRestored )
		//	{
		//		if( heightmapTerrainManager != null && !Enabled )
		//		{
		//			//stop streaming enable thread
		//			if( streamingEnable )
		//			{
		//				//stop the task
		//				streamingEnableMustStop = true;
		//				streamingEnableTask.Wait();

		//				//disable mode
		//				streamingEnable = false;
		//				streamingEnableTask.Dispose();
		//				streamingEnableTask = null;
		//				streamingEnableError = false;
		//				streamingEnableMustStop = false;
		//				streamingEnableFinishCounter = 0;

		//				foreach( KeyValuePair<TextureData, IntPtr> pair in streamingEnableDataForTextures )
		//					NativeUtils.Free( pair.Value );
		//				streamingEnableDataForTextures = new Dictionary<TextureData, IntPtr>();
		//			}

		//			DestroyAllRecreatableDataForRendering();
		//			DestroyCollisionBodies();
		//		}
		//		else
		//		{
		//			SetNeedUpdateGeneratedMaterials();
		//			needUpdateAOTextures = true;
		//			needReloadLayersTextures = true;
		//			needUpdateAfterDeviceLost = true;
		//		}
		//	}
		//}

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

		//public static HeightmapSizeTypes GetHeightmapSizeEnumFromInteger( int size )
		//{
		//	switch( size )
		//	{
		//	case 128: return HeightmapSizeTypes.Size128x128;
		//	case 256: return HeightmapSizeTypes.Size256x256;
		//	case 512: return HeightmapSizeTypes.Size512x512;
		//	case 1024: return HeightmapSizeTypes.Size1024x1024;
		//	case 2048: return HeightmapSizeTypes.Size2048x2048;
		//	case 4096: return HeightmapSizeTypes.Size4096x4096;
		//	}
		//	Log.Fatal( "HeightmapTerrain: GetHeightmapSizeEnumFromInteger: Invalid size." );
		//	return HeightmapSizeTypes.Size128x128;
		//}

		//public static int GetHeightmapSizeFromEnum( HeightmapSizeTypes size )
		//{
		//	switch( size )
		//	{
		//	case HeightmapSizeTypes.Size128x128: return 128;
		//	case HeightmapSizeTypes.Size256x256: return 256;
		//	case HeightmapSizeTypes.Size512x512: return 512;
		//	case HeightmapSizeTypes.Size1024x1024: return 1024;
		//	case HeightmapSizeTypes.Size2048x2048: return 2048;
		//	case HeightmapSizeTypes.Size4096x4096: return 4096;
		//	}
		//	Log.Fatal( "HeightmapTerrain: GetHeightmapSizeFromEnum: is not implemented." );
		//	return 0;
		//}

		void ResizeHeightmap( int newSize )
		{
			//if( IsPostCreated && loadingStatus == LoadingStatuses.Loaded )
			//{

			//!!!!
			//DestroyAllRecreatableDataForRendering();
			//DestroyCollisionBodies();
			//ClearStaticLightingData();
			//aoData = null;

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

			//}
			//else
			//{
			//	heightmapSize = newHeightmapSize;
			//}

			//SetModified( true );
		}

		///// <summary>
		///// The parameter indicates the dimension of the terrain. The higher the value, the more detailed the landscape will be.
		///// </summary>
		//[LocalizedDescription( "The parameter indicates the dimension of the terrain. The higher the value, the more detailed the landscape will be.", "HeightmapTerrain" )]
		//[Category( "Geometry" )]
		//[DefaultValue( HeightmapSizeTypes.Size1024x1024 )]
		//public HeightmapSizeTypes HeightmapSize
		//{
		//	get { return GetHeightmapSizeEnumFromInteger( heightmapSize ); }
		//	set
		//	{
		//		if( HeightmapSize == value )
		//			return;

		//		int newHeightmapSize = GetHeightmapSizeFromEnum( value );

		//		if( IsPostCreated && loadingStatus == LoadingStatuses.Loaded )
		//		{
		//			if( EntitySystemWorld.Instance.IsEditor() && masksSize < newHeightmapSize )
		//			{
		//				Log.Warning( Translate( "The masks map size (MaskSize property) must be less than or equal to heightmap size." ) );
		//				return;
		//			}

		//			if( EntitySystemWorld.Instance.IsEditor() && tileSize > newHeightmapSize )
		//			{
		//				Log.Warning( Translate( "The tile size (TileSize property) must be less than or equal to heightmap size." ) );
		//				return;
		//			}

		//			if( EntitySystemWorld.Instance.IsEditor() )
		//			{
		//				string text = string.Format(
		//					Translate( "Are you sure you want to resize heightmap to {0}x{1}?" ),
		//					newHeightmapSize, newHeightmapSize );

		//				//if( newHeightmapSize > 1024 )
		//				//{
		//				//   text += "\n";
		//				//   text += "\n" + Translate(
		//				//      "For older hardware the more simple terrain renderer will used" +
		//				//      "Please note that the more simple terrain rendering technique demands 4 times " +
		//				//      "more memory. For some systems it is not recommended to use a heightmap size " +
		//				//      "of more than 1024x1024. " +
		//				//      "Minimum card version / specification for NVIDIA is GeForce 7 (7xxx) series. " +
		//				//      "Minimum card version / specification for AMD is Radeon R600 (HD 2xxx) series. " +
		//				//      "OpenGL rendering system is not yet supported." );
		//				//}

		//				text += "\n";
		//				text += "\n" + Translate( "It is impossible to undo this action!" );

		//				if( EditorMessageBox.Show( text, "Map Editor", EditorMessageBox.Buttons.YesNo,
		//					EditorMessageBox.Icon.Question ) != EditorMessageBox.Result.Yes )
		//					return;
		//			}

		//			ResizeHeightmap( value );

		//			//updates to the editor
		//			if( UndoSystem.Instance != null )
		//				UndoSystem.Instance.Clear();
		//			if( MapEditorInterface.Instance != null )
		//				MapEditorInterface.Instance.SetMapModified();
		//		}
		//		else
		//		{
		//			heightmapSize = newHeightmapSize;
		//		}

		//		SetModified( true );
		//	}
		//}

		///// <summary>
		///// Specifies the size of cell of map to blending layers. The greater this parameter is, the more quality the blending between layers will be.
		///// </summary>
		//[LocalizedDescription( "Specifies the size of cell of map to blending layers. The greater this parameter is, the more quality the blending between layers will be.", "HeightmapTerrain" )]
		//[Category( "Painting" )]
		//[DefaultValue( MaskSizeTypes.Size2048x2048 )]
		//public MaskSizeTypes MasksSize
		//{
		//	get
		//	{
		//		switch( masksSize )
		//		{
		//		case 256: return MaskSizeTypes.Size256x256;
		//		case 512: return MaskSizeTypes.Size512x512;
		//		case 1024: return MaskSizeTypes.Size1024x1024;
		//		case 2048: return MaskSizeTypes.Size2048x2048;
		//		case 4096: return MaskSizeTypes.Size4096x4096;
		//		default: throw new Exception( "MaskSizeTypes MasksSize: is not implemented" );
		//		}
		//	}
		//	set
		//	{
		//		if( MasksSize == value )
		//			return;

		//		int newMasksSize;
		//		{
		//			switch( value )
		//			{
		//			case MaskSizeTypes.Size256x256: newMasksSize = 256; break;
		//			case MaskSizeTypes.Size512x512: newMasksSize = 512; break;
		//			case MaskSizeTypes.Size1024x1024: newMasksSize = 1024; break;
		//			case MaskSizeTypes.Size2048x2048: newMasksSize = 2048; break;
		//			case MaskSizeTypes.Size4096x4096: newMasksSize = 4096; break;
		//			default: throw new Exception( "MaskSizeTypes MasksSize: is not implemented" );
		//			}
		//		}

		//		if( IsPostCreated && loadingStatus == LoadingStatuses.Loaded )
		//		{
		//			if( EntitySystemWorld.Instance.IsEditor() && newMasksSize < heightmapSize )
		//			{
		//				Log.Warning( Translate( "The masks map size must be more than or equal to heightmap size (HeightmapSize property)." ) );
		//				return;
		//			}

		//			if( EntitySystemWorld.Instance.IsEditor() )
		//			{
		//				string text = string.Format(
		//					Translate( "Are you sure you want to resize masks size to {0}x{1}?" ),
		//					newMasksSize, newMasksSize );
		//				text += "\n";
		//				text += "\n" + Translate( "It is impossible to undo this action!" );

		//				if( EditorMessageBox.Show( text, "Map Editor", EditorMessageBox.Buttons.YesNo,
		//					EditorMessageBox.Icon.Question ) != EditorMessageBox.Result.Yes )
		//					return;
		//			}

		//			DestroyAllRecreatableDataForRendering();

		//			//resize
		//			while( newMasksSize > masksSize )
		//				ResizeMaskBuffersMultiple2();
		//			while( newMasksSize < masksSize )
		//				ResizeMaskBuffersDivide2();

		//			UpdateAllRecreatableDataForRendering();

		//			//updates to the editor
		//			if( UndoSystem.Instance != null )
		//				UndoSystem.Instance.Clear();
		//			if( MapEditorInterface.Instance != null )
		//				MapEditorInterface.Instance.SetMapModified();
		//		}
		//		else
		//		{
		//			masksSize = newMasksSize;
		//		}

		//		SetModified( true );
		//	}
		//}

		///// <summary>
		///// Number of cells in the one terrain tile (a piece of the landscape).
		///// </summary>
		///// <remarks>
		///// Tile is a part of the terrain, which is drawn as a single unit in a single draw call. For example, 16x16 - it is 256 cells, which are drawn together. If you see at least one cell of the tile, the engine draws them all. You also have to remember that one tile cannot have more than 5 painting layers. This property is recommended to choose in the process of optimizing performance of the map.
		///// </remarks>
		//[LocalizedDescription( "Number of cells in the one terrain tile (a piece of the landscape). Tile is a part of the terrain, which is drawn as a single unit in a single draw call. For example, 16x16 - it is 256 cells, which are drawn together. If you see at least one cell of the tile, the engine draws them all. You also have to remember that one tile cannot have more than 5 painting layers. This property is recommended to choose in the process of optimizing performance of the map.", "HeightmapTerrain" )]
		//[Category( "Geometry" )]
		//[DefaultValue( TileSizeTypes.Size64x64 )]
		//public TileSizeTypes TileSize
		//{
		//	get
		//	{
		//		switch( tileSize )
		//		{
		//		case 16: return TileSizeTypes.Size16x16;
		//		case 32: return TileSizeTypes.Size32x32;
		//		case 64: return TileSizeTypes.Size64x64;
		//		case 128: return TileSizeTypes.Size128x128;
		//		case 256: return TileSizeTypes.Size256x256;
		//		default: throw new Exception( "TileSizeTypes TileSize: is not implemented" );
		//		}
		//	}
		//	set
		//	{
		//		if( TileSize == value )
		//			return;

		//		int newTileSize;
		//		{
		//			switch( value )
		//			{
		//			case TileSizeTypes.Size16x16: newTileSize = 16; break;
		//			case TileSizeTypes.Size32x32: newTileSize = 32; break;
		//			case TileSizeTypes.Size64x64: newTileSize = 64; break;
		//			case TileSizeTypes.Size128x128: newTileSize = 128; break;
		//			case TileSizeTypes.Size256x256: newTileSize = 256; break;
		//			default: throw new Exception( "TileSizeTypes TileSize: is not implemented" );
		//			}
		//		}

		//		if( EntitySystemWorld.Instance.IsEditor() && newTileSize > heightmapSize )
		//		{
		//			Log.Warning( Translate( "The tile size must be less than or equal to heightmap size (HeightmapSize property)." ) );
		//			return;
		//		}

		//		if( IsPostCreated && Enabled )
		//		{
		//			DestroyAllRecreatableDataForRendering();
		//			DestroyCollisionBodies();
		//		}

		//		tileSize = newTileSize;

		//		if( IsPostCreated && Enabled )
		//		{
		//			UpdateAllRecreatableDataForRendering();
		//			CreateCollisionBodies();
		//		}

		//		SetModified( true );
		//	}
		//}

		Tile GetTileByObjectInSpace( Component_ObjectInSpace objectInSpace )
		{
			var tile = objectInSpace.AnyData as Tile;
			if( tile != null && tile.owner == this )
				return tile;
			return null;
		}

		Vector2I[] GenerateTileVerticesCellIndices( Tile tile, int lodLevel )
		{
			if( lodLevel >= cachedLodLevelCount )
				Log.Fatal( "Component_Terrain: GenerateTileVerticesCellIndices: lodLevel >= cachedLodLevelCount." );

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
				Log.Fatal( "Component_Terrain: GenerateTileVerticesCellIndices: current != cellIndices.Length." );

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
				Log.Fatal( "Component_Terrain: GenerateTileIndicesWithoutHoles: lodLevel >= cachedLodLevelCount." );

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
				Log.Fatal( "Component_Terrain: GenerateTileIndicesWithoutHoles: current != indices.Length." );

			//add to the cache
			if( lodLevel == 0 )
				generateTileIndicesWithoutHolesCache[ tileSize ] = indices;

			return indices;
		}

		//List<int> GenerateTileIndicesWithHoles( Vector2I cellIndexMin )
		//{
		//	var indices = new List<int>( tileSize * tileSize * 6 );
		//	for( int y = 0; y < tileSize; y++ )
		//	{
		//		for( int x = 0; x < tileSize; x++ )
		//		{
		//			Vector2I holeIndex = cellIndexMin + new Vector2I( x, y );
		//			//if( !GetHoleFlag( holeIndex ) )
		//			//{
		//			int sizeX = tileSize + 1;
		//			indices.Add( y * sizeX + x );
		//			indices.Add( y * sizeX + ( x + 1 ) );
		//			indices.Add( ( y + 1 ) * sizeX + ( x + 1 ) );
		//			indices.Add( ( y + 1 ) * sizeX + ( x + 1 ) );
		//			indices.Add( ( y + 1 ) * sizeX + x );
		//			indices.Add( y * sizeX + x );
		//			//}
		//		}
		//	}
		//	return indices;
		//}

		//VertexData GetSharedVertexData( int neededLodLevel )
		//{
		//	if( sharedVertexDatas == null )
		//	{
		//		//create vertex data for each lod level

		//		sharedVertexDatas = new VertexData[ cachedLodLevelCount ];

		//		for( int lodLevel = 0; lodLevel < cachedLodLevelCount; lodLevel++ )
		//		{
		//			int vertexCountByAxis = ( tileSize >> lodLevel ) + 1;
		//			int vertexCount = vertexCountByAxis * vertexCountByAxis;

		//			//create vertexData
		//			VertexData vertexData = CreateTileVertexData( vertexCount );
		//			sharedVertexDatas[ lodLevel ] = vertexData;

		//			//fill vertex buffer

		//			//calculate vertex positions and normals
		//			Vector3[,] positions = new Vector3[ vertexCountByAxis, vertexCountByAxis ];
		//			for( int y = 0; y < vertexCountByAxis; y++ )
		//			{
		//				for( int x = 0; x < vertexCountByAxis; x++ )
		//				{
		//					float multiplier = cachedCellSize * (float)( 1 << lodLevel );
		//					positions[ x, y ] = new Vector3( (float)x * multiplier, (float)y * multiplier, 0 );
		//				}
		//			}

		//			//calculate lod data (morph offsets and normals)
		//			Vector2[,] morphOffsets = null;
		//			//!!!!
		//			//if( LODSettings.Enabled )
		//			//{
		//			//	morphOffsets = new Vector2[ vertexCountByAxis, vertexCountByAxis ];
		//			//	for( int y = 0; y < vertexCountByAxis; y++ )
		//			//	{
		//			//		for( int x = 0; x < vertexCountByAxis; x++ )
		//			//		{
		//			//			Vector2 morphOffset = Vector2.Zero;
		//			//			if( x % 2 == 1 || y % 2 == 1 )
		//			//			{
		//			//				int x2 = ( x / 2 ) * 2;
		//			//				int y2 = ( y / 2 ) * 2;
		//			//				morphOffset = positions[ x2, y2 ].ToVector2() - positions[ x, y ].ToVector2();
		//			//			}
		//			//			morphOffsets[ x, y ] = morphOffset;
		//			//		}
		//			//	}
		//			//}

		//			//Copy data to buffer
		//			unsafe
		//			{
		//				HardwareVertexBuffer vertexBuffer = vertexData.VertexBufferBinding.GetBuffer( 0 );
		//				int vertexSize = vertexBuffer.VertexSizeInBytes;

		//				byte* pointer = (byte*)vertexBuffer.Lock( HardwareBuffer.LockOptions.Normal );
		//				int indexX = 0;
		//				int indexY = 0;

		//				for( int nVertex = 0; nVertex < vertexCount; nVertex++ )
		//				{
		//					*(Vector3*)( pointer + sharedVertexBuffers_positionChannelOffset ) = positions[ indexX, indexY ];
		//					//!!!!
		//					//if( LODSettings.Enabled )
		//					//{
		//					//	*(Vector2*)( pointer + sharedVertexBuffers_morphOffsetChannelOffset ) =
		//					//		morphOffsets[ indexX, indexY ];
		//					//}

		//					pointer += vertexSize;
		//					indexX++;
		//					if( indexX >= vertexCountByAxis )
		//					{
		//						indexY++;
		//						indexX = 0;
		//					}
		//				}

		//				vertexBuffer.Unlock();
		//			}
		//		}
		//	}

		//	return sharedVertexDatas[ neededLodLevel ];
		//}

		//IndexData GetSharedIndexData( int lodLevel )
		//{
		//	if( sharedIndexDatas == null )
		//	{
		//		sharedIndexDatas = new IndexData[ cachedLodLevelCount ];

		//		for( int level = 0; level < cachedLodLevelCount; level++ )
		//		{
		//			List<int> indices = GenerateTileIndicesWithoutHoles( level );
		//			if( indices.Count == 0 )
		//				Log.Fatal( "HeightmapTerrain: GetSharedIndexData: indices.Count == 0." );

		//			bool optimizeVertexCache = !EntitySystemWorld.Instance.IsEditor() && heightmapTerrainManager == null;
		//			IndexData indexData = IndexData.CreateFromList( indices, optimizeVertexCache );
		//			if( indexData == null )
		//				Log.Fatal( "Component_Terrain: GetSharedIndexData: IndexData.CreateFromList failed." );

		//			sharedIndexDatas[ level ] = indexData;
		//		}
		//	}

		//	return sharedIndexDatas[ lodLevel ];
		//}

		//void DisposeSharedVertexDatas()
		//{
		//	if( sharedVertexDatas != null )
		//	{
		//		foreach( VertexData vertexData in sharedVertexDatas )
		//			vertexData.Dispose();
		//		sharedVertexDatas = null;
		//	}
		//}

		//void DisposeSharedIndexDatas()
		//{
		//	if( sharedIndexDatas != null )
		//	{
		//		foreach( IndexData indexData in sharedIndexDatas )
		//			indexData.Dispose();
		//		sharedIndexDatas = null;
		//	}
		//}

		//void UpdateTechniqueForStaticMeshObject( Tile tile, StaticMeshObject staticMeshObject, Camera camera, float cameraDistance )
		//{
		//	Material material = staticMeshObject.Material;
		//	if( material == null )
		//		return;

		//	Technique technique = null;

		//	//find technique
		//	if( !camera.IsForShadowMapGeneration() )
		//	{
		//		bool allowDetail = cameraDistance < detailMapsDistance;

		//		for( int n = 0; n < tile.materialTechniquesInfo.Length; n++ )
		//		{
		//			Tile.MaterialTechniquesInfoItem item = tile.materialTechniquesInfo[ n ];
		//			if( simpleRendering == item.simpleRendering && allowDetail == item.allowDetail )
		//			{
		//				technique = material.Techniques[ n ];
		//				break;
		//			}
		//		}

		//		if( technique == null )
		//		{
		//			for( int n = 0; n < tile.materialTechniquesInfo.Length; n++ )
		//			{
		//				Tile.MaterialTechniquesInfoItem item = tile.materialTechniquesInfo[ n ];
		//				if( simpleRendering == item.simpleRendering )
		//				{
		//					technique = material.Techniques[ n ];
		//					break;
		//				}
		//			}
		//		}

		//		if( technique == null )
		//			technique = material.Techniques[ 0 ];

		//		if( technique == null )
		//			Log.Fatal( "HeightmapTerrain: UpdateTechniqueForStaticMeshObject: technique == null." );
		//	}

		//	staticMeshObject.ForceTechnique = technique;
		//}

		//void AddToRenderQueue_UpdateTile( Tile tile, Camera updatingCamera, float cameraDistance )
		//{
		//	//double call check (shadow generation, main camera)
		//	float renderTime = RendererWorld.Instance.FrameRenderTime;
		//	if( tile.addToRenderQueueTime == renderTime && tile.addToRenderQueueCamera == updatingCamera )
		//		return;
		//	tile.addToRenderQueueTime = renderTime;
		//	tile.addToRenderQueueCamera = updatingCamera;

		//	if( LODSettings.Enabled && !IsFixedPipelineFallback() )
		//	{
		//		//lods are enabled

		//		int lodLevel = tile.CalculateLODLevel( cameraDistance );

		//		//update vertex/index data
		//		{
		//			VertexData vertexData;
		//			if( tile.vertexDatasForNonSharedVertexBuffersMode != null )
		//				vertexData = tile.vertexDatasForNonSharedVertexBuffersMode[ lodLevel ];
		//			else
		//				vertexData = GetSharedVertexData( lodLevel );
		//			IndexData indexData = GetSharedIndexData( lodLevel );
		//			tile.staticMeshObject.SetVertexIndexData( vertexData, indexData );
		//		}

		//		//float[] levelDistances = GetLODLevelDistances();

		//		////set morphParameters
		//		//{
		//		//	Vector2 value;

		//		//	if( lodLevel < levelDistances.Length )
		//		//	{
		//		//		float start = 0;
		//		//		if( lodLevel > 0 )
		//		//			start = levelDistances[ lodLevel - 1 ];
		//		//		float morphEnd = levelDistances[ lodLevel ];
		//		//		float morphSize = ( morphEnd - start ) * lodSettings.MorphAreaPercent;
		//		//		if( morphSize == 0 )
		//		//			morphSize = .0001f;
		//		//		float morphSizeInv = 1.0f / morphSize;
		//		//		//float morphStart = morphEnd - morphSize;
		//		//		value = new Vector2( morphEnd * morphSizeInv, morphSizeInv );
		//		//		//value = new Vector2( morphEnd / morphSize, 1.0f / morphSize );
		//		//	}
		//		//	else
		//		//	{
		//		//		//latest lod. disable morphing
		//		//		value = new Vector2( 1, 0 );
		//		//	}

		//		//	tile.staticMeshObject.SetCustomGpuParameter(
		//		//		(int)_HeightmapTerrainMaterial.GpuParameters.morphParameters,
		//		//		new Vec4( value.X, value.Y, 0, 0 ) );
		//		//}

		//		//morphCameraPosition
		//		{
		//			Vector3 value = updatingCamera.DerivedPosition;
		//			tile.staticMeshObject.SetCustomGpuParameter(
		//				(int)_HeightmapTerrainMaterial.GpuParameters.morphCameraPosition,
		//				new Vec4( value.X, value.Y, value.Z, 0 ) );
		//		}

		//		//showLODLevelColor
		//		{
		//			Vec4 value = new Vec4( -1, -1, -1, -1 );
		//			if( lodSettings.ShowLevels )
		//			{
		//				Vec4[] colors = new Vec4[]
		//			   {
		//				  new Vec4( 1, 0, 0, 1 ),
		//				  new Vec4( 0, 1, 0, 1 ),
		//				  new Vec4( 0, 0, 1, 1 )
		//			   };
		//				value = colors[ lodLevel % 3 ];
		//			}
		//			tile.staticMeshObject.SetCustomGpuParameter(
		//				(int)_HeightmapTerrainMaterial.GpuParameters.showLODLevelColor, value );
		//		}
		//	}
		//	else
		//	{
		//		//no lods

		//		//update vertex/index data
		//		VertexData vertexData;
		//		if( tile.vertexDatasForNonSharedVertexBuffersMode != null )
		//			vertexData = tile.vertexDatasForNonSharedVertexBuffersMode[ 0 ];
		//		else
		//			vertexData = GetSharedVertexData( 0 );
		//		IndexData indexData = GetSharedIndexData( 0 );
		//		tile.staticMeshObject.SetVertexIndexData( vertexData, indexData );
		//	}
		//}

		//void staticMeshObject_AddToRenderQueue( StaticMeshObject staticMeshObject, Camera camera, ref bool allowRender )
		//{
		//	//check by Visible flag
		//	if( !visible || streamingEnable )
		//	{
		//		allowRender = false;
		//		return;
		//	}

		//	////check by bottomClipHeight
		//	//if( staticMeshObject.Bounds.Maximum.Z < bottomClipHeight )
		//	//{
		//	//   allowRender = false;
		//	//   return;
		//	//}

		//	Tile tile = GetTileByStaticMeshObject( staticMeshObject );

		//	//check for no geometry
		//	if( tile.allHoles )
		//	{
		//		allowRender = false;
		//		return;
		//	}

		//	Camera updatingCamera = SceneManager.Instance.CurrentUpdatingCamera;
		//	float cameraDistance = tile.GetCameraDistance( updatingCamera );
		//	AddToRenderQueue_UpdateTile( tile, updatingCamera, cameraDistance );
		//	UpdateTechniqueForStaticMeshObject( tile, staticMeshObject, camera, cameraDistance );
		//	staticMeshObject.RenderQueueGroup = renderQueueGroup;
		//}

		//public static HeightmapTerrain GetTerrainByBody( Body body )
		//{
		//	return body._InternalUserData as HeightmapTerrain;
		//}

		//int GetMaxPaintedLayerByCellIndex( Vector2I cellIndex )
		//{
		//	Vector2I maskIndex = ( cellIndex * masksSize + new Vector2I( masksSize / 2, masksSize / 2 ) ) / heightmapSize;
		//	//Vector2I maskIndex = GetMaskIndexByCellIndex( cellIndex );
		//	ClampMaskIndex( ref maskIndex );

		//	int maxLayerIndex = -1;
		//	int maxValue = 0;

		//	int remainder = 255;

		//	for( int n = layers.Count - 1; n >= 0; n-- )
		//	{
		//		Layer layer = layers[ n ];

		//		int mask;
		//		if( layer.mask != null )
		//			mask = layer.GetMaskValueAsByte( maskIndex );
		//		else
		//			mask = 255;

		//		if( mask != 0 )
		//		{
		//			if( mask > remainder )
		//				mask = remainder;

		//			if( mask > maxValue )
		//			{
		//				maxLayerIndex = n;
		//				maxValue = mask;
		//			}

		//			remainder -= mask;
		//			if( remainder == 0 )
		//				break;
		//		}
		//	}

		//	return maxLayerIndex;
		//}

		//public LayerPointInfo[] GetLayersInfoByPosition( Vector2 pos )
		//{
		//	Vector2I maskIndex = GetMaskIndexByPosition( pos );
		//	ClampMaskIndex( ref maskIndex );

		//	List<LayerPointInfo> layerPointInfos = new List<LayerPointInfo>( 4 );
		//	//List<LayerPointInfo> layerPointInfos = listLayerPointInfoAllocator.Alloc();

		//	int remainder = 255;

		//	for( int n = layers.Count - 1; n >= 0; n-- )
		//	{
		//		Layer layer = layers[ n ];

		//		int mask;
		//		if( layer.mask != null )
		//			mask = layer.GetMaskValueAsByte( maskIndex );
		//		else
		//			mask = 255;

		//		if( mask != 0 )
		//		{
		//			if( mask > remainder )
		//				mask = remainder;

		//			layerPointInfos.Add( new LayerPointInfo( layer, (float)mask * ( 1.0f / 255.0f ) ) );

		//			remainder -= mask;
		//			if( remainder == 0 )
		//				break;
		//		}
		//	}

		//	LayerPointInfo[] info = layerPointInfos.ToArray();

		//	//listLayerPointInfoAllocator.Free( layerPointInfos );

		//	return info;
		//}

		void CreateCollisionBodies()
		{
			if( !EnabledInHierarchyAndIsNotResource )
				return;

			DestroyCollisionBodies();

			if( Collision )
			{
				foreach( var tile in GetTiles() )
					tile.UpdateCollisionBody();
			}

			//!!!!
			//if( Enabled && loadingStatus == LoadingStatuses.Loaded && Collision &&
			//	EngineApp.Instance.ApplicationType != EngineApp.ApplicationTypes.ShaderCacheCompiler )
			//{
			//	LongOperationCallbackManager.CallCallback( "HeightmapTerrain: CreateCollisionBodies" );

			//	if( PhysicsWorld.Instance.IsHeightFieldShapeSupported )
			//	{
			//		//float t = EngineApp.Instance.Time;

			//		UpdateHeightFieldBody();

			//		//Log.Info( "Physics: " + ( EngineApp.Instance.Time - t ).ToString() );
			//	}
			//	else
			//	{
			//		int bodyCount = GetBodyCountByAxis();
			//		bodies = new Body[ bodyCount, bodyCount ];
			//		for( int y = 0; y < bodyCount; y++ )
			//			for( int x = 0; x < bodyCount; x++ )
			//				UpdateCollisionBody( new Vector2I( x, y ) );
			//	}
			//}
		}

		void DestroyCollisionBodies()
		{
			foreach( var tile in GetTiles() )
				tile.DestroyCollisionBody();

			//if( heightFieldBody != null )
			//{
			//	heightFieldBody.Dispose();
			//	heightFieldBody = null;
			//}

			//if( bodies != null )
			//{
			//	int count = bodies.GetLength( 0 );
			//	for( int y = 0; y < count; y++ )
			//	{
			//		for( int x = 0; x < count; x++ )
			//		{
			//			Body body = bodies[ x, y ];
			//			if( body != null )
			//				body.Dispose();
			//		}
			//	}
			//	bodies = null;
			//}
		}

		//void UpdateCollisionBody( Vector2I bodyIndex )
		//{
		//	if( cachedCellSize == 0 )
		//		CalculateCachedGeneralData();

		//	LongOperationCallbackManager.CallCallback(
		//		"HeightmapTerrain: UpdateBody: " + bodyIndex.ToString() );

		//	//destroy old
		//	if( bodies[ bodyIndex.X, bodyIndex.Y ] != null )
		//	{
		//		bodies[ bodyIndex.X, bodyIndex.Y ].Dispose();
		//		bodies[ bodyIndex.X, bodyIndex.Y ] = null;
		//	}

		//	int bodyCellSize = GetBodyCellSize();
		//	Vector2I cellIndexMin = bodyIndex * bodyCellSize;
		//	Vector2I cellIndexMax = cellIndexMin + new Vector2I( bodyCellSize, bodyCellSize );
		//	ClampCellIndex( ref cellIndexMin );
		//	ClampCellIndex( ref cellIndexMax );

		//	Vector2I size = cellIndexMax - cellIndexMin;
		//	int sizeX = cellIndexMax.X - cellIndexMin.X + 1;

		//	int indexCount = 0;
		//	for( int y = 0; y < size.Y; y++ )
		//	{
		//		for( int x = 0; x < size.X; x++ )
		//		{
		//			Vector2I holeIndex = cellIndexMin + new Vector2I( x, y );
		//			if( !GetHoleFlag( holeIndex ) )
		//				indexCount += 6;
		//		}
		//	}

		//	if( indexCount != 0 )
		//	{
		//		bool useMaterialIndices = false;
		//		{
		//			for( int nLayer = 0; nLayer < layers.Count; nLayer++ )
		//			{
		//				Layer layer = layers[ nLayer ];
		//				if( !string.IsNullOrEmpty( layer.OverrideCollisionMaterial ) )
		//				{
		//					useMaterialIndices = true;
		//					break;
		//				}
		//			}
		//		}

		//		int[] layerIndexToMaterialIndex = null;
		//		List<MeshShape.PerTriangleMaterial> perTriangleMaterials = null;
		//		if( useMaterialIndices )
		//		{
		//			layerIndexToMaterialIndex = new int[ layers.Count ];
		//			perTriangleMaterials = new List<MeshShape.PerTriangleMaterial>( 1 + layers.Count );

		//			perTriangleMaterials.Add( new MeshShape.PerTriangleMaterial( CollisionMaterialName, true ) );
		//			for( int nLayer = 0; nLayer < layers.Count; nLayer++ )
		//			{
		//				Layer layer = layers[ nLayer ];
		//				if( !string.IsNullOrEmpty( layer.OverrideCollisionMaterial ) )
		//				{
		//					perTriangleMaterials.Add( new MeshShape.PerTriangleMaterial( layer.OverrideCollisionMaterial, true ) );
		//					layerIndexToMaterialIndex[ nLayer ] = perTriangleMaterials.Count - 1;
		//				}
		//			}
		//		}

		//		Vector3[] vertices = new Vector3[ ( size.X + 1 ) * ( size.Y + 1 ) ];
		//		int[] indices = new int[ indexCount ];
		//		short[] materialIndices = null;
		//		if( useMaterialIndices )
		//			materialIndices = new short[ indices.Length / 3 ];

		//		int vertexPosition = 0;
		//		int indexPosition = 0;
		//		int materialIndexPosition = 0;

		//		//vertices
		//		for( int y = cellIndexMin.Y; y <= cellIndexMax.Y; y++ )
		//		{
		//			for( int x = cellIndexMin.X; x <= cellIndexMax.X; x++ )
		//			{
		//				vertices[ vertexPosition ] = GetPosition( new Vector2I( x, y ) );
		//				vertexPosition++;
		//			}
		//		}

		//		//indices
		//		for( int y = 0; y < size.Y; y++ )
		//		{
		//			for( int x = 0; x < size.X; x++ )
		//			{
		//				Vector2I cellIndex = cellIndexMin + new Vector2I( x, y );
		//				if( !GetHoleFlag( cellIndex ) )
		//				{
		//					indices[ indexPosition ] = y * sizeX + x;
		//					indexPosition++;
		//					indices[ indexPosition ] = y * sizeX + x + 1;
		//					indexPosition++;
		//					indices[ indexPosition ] = ( y + 1 ) * sizeX + x + 1;
		//					indexPosition++;
		//					indices[ indexPosition ] = ( y + 1 ) * sizeX + x + 1;
		//					indexPosition++;
		//					indices[ indexPosition ] = ( y + 1 ) * sizeX + x;
		//					indexPosition++;
		//					indices[ indexPosition ] = y * sizeX + x;
		//					indexPosition++;

		//					if( useMaterialIndices )
		//					{
		//						int layerIndex = GetMaxPaintedLayerByCellIndex( cellIndex );
		//						int materialIndex = layerIndexToMaterialIndex[ layerIndex ];
		//						materialIndices[ materialIndexPosition ] = (short)materialIndex;
		//						materialIndexPosition++;
		//						materialIndices[ materialIndexPosition ] = (short)materialIndex;
		//						materialIndexPosition++;
		//					}
		//				}
		//			}
		//		}

		//		if( vertices.Length != vertexPosition )
		//			Log.Fatal( "HeightmapTerrain: UpdateBody: vertices.Length != vertexPosition." );
		//		if( indices.Length != indexPosition )
		//			Log.Fatal( "HeightmapTerrain: UpdateBody: indices.Length != indexPosition." );
		//		if( materialIndices != null )
		//		{
		//			if( materialIndices.Length != materialIndexPosition )
		//				Log.Fatal( "HeightmapTerrain: UpdateBody: materialIndices.Length != materialIndexPosition." );
		//		}

		//		Body body = PhysicsWorld.Instance.CreateBody();
		//		body.Static = true;
		//		body._InternalUserData = this;

		//		string customMeshName = PhysicsWorld.Instance.AddCustomMeshGeometry( vertices, indices,
		//			materialIndices, MeshShape.MeshTypes.TriangleMesh, 0, 0, false );

		//		MeshShape shape = body.CreateMeshShape();
		//		if( useMaterialIndices )
		//		{
		//			shape.PerTriangleMaterials = perTriangleMaterials.ToArray();
		//			shape.PerTriangleMaterialIndices = materialIndices;
		//		}
		//		shape.MeshName = customMeshName;
		//		shape.MaterialName = CollisionMaterialName;
		//		shape.ContactGroup = (int)ContactGroup.Collision;
		//		shape.VehicleDrivableSurface = true;

		//		body.PushedToWorld = true;

		//		bodies[ bodyIndex.X, bodyIndex.Y ] = body;
		//	}
		//}

		//void UpdateHeightFieldBody()
		//{
		//	LongOperationCallbackManager.CallCallback( "HeightmapTerrain: UpdateHeightFieldBody" );

		//	if( heightFieldBody != null )
		//	{
		//		heightFieldBody.Dispose();
		//		heightFieldBody = null;
		//	}

		//	heightFieldBody = PhysicsWorld.Instance.CreateBody();
		//	heightFieldBody.Static = true;
		//	heightFieldBody._InternalUserData = this;

		//	heightFieldBody.Position = new Vector3( Position.X - horizontalSize / 2, Position.Y - horizontalSize / 2,
		//		GetHeightMinMax().Minimum );

		//	float heightMinWithoutPosition = GetHeightMinMaxWithoutPosition().Minimum;
		//	float heightMinMaxSize = GetHeightMinMax().Size();
		//	if( heightMinMaxSize < .1f )
		//		heightMinMaxSize = .1f;
		//	float heightMinMaxSizeInv = 1.0f / heightMinMaxSize;

		//	HeightFieldShape shape = heightFieldBody.CreateHeightFieldShape();

		//	//per triangle materials
		//	int[] layerIndexToMaterialIndex = new int[ layers.Count ];
		//	{
		//		List<HeightFieldShape.PerTriangleMaterial> materials = new List<HeightFieldShape.PerTriangleMaterial>( 1 + layers.Count );

		//		materials.Add( new HeightFieldShape.PerTriangleMaterial( CollisionMaterialName, true ) );

		//		for( int nLayer = 0; nLayer < layers.Count; nLayer++ )
		//		{
		//			Layer layer = layers[ nLayer ];
		//			if( !string.IsNullOrEmpty( layer.OverrideCollisionMaterial ) )
		//			{
		//				if( materials.Count >= 127 )
		//				{
		//					Log.Warning( Translate( "HeightmapTerrain: The amount of different physical materials for collision detection can't be more than 126." ) );
		//					break;
		//				}

		//				materials.Add( new HeightFieldShape.PerTriangleMaterial( layer.OverrideCollisionMaterial, true ) );
		//				layerIndexToMaterialIndex[ nLayer ] = materials.Count - 1;
		//			}
		//		}

		//		if( materials.Count > 1 )
		//			shape.PerTriangleMaterials = materials.ToArray();
		//	}

		//	int length = heightmapSize + 1;
		//	HeightFieldShape.Sample[] samples = new HeightFieldShape.Sample[ length * length ];
		//	for( int y = 0; y < length; y++ )
		//	{
		//		for( int x = 0; x < length; x++ )
		//		{
		//			HeightFieldShape.Sample sample = new HeightFieldShape.Sample();

		//			sample.Height = (ushort)( ( GetHeightWithoutPosition( new Vector2I( x, y ) ) - heightMinWithoutPosition ) *
		//				heightMinMaxSizeInv * 65535.0f );

		//			Vector2I cellIndex = new Vector2I( x, y );
		//			if( IsValidHoleIndex( cellIndex ) && GetHoleFlag( cellIndex ) )
		//			{
		//				sample.MaterialIndex0 = HeightFieldShape.Sample.HoleMaterialIndex;
		//				sample.MaterialIndex1 = HeightFieldShape.Sample.HoleMaterialIndex;
		//			}
		//			else
		//			{
		//				if( shape.PerTriangleMaterials != null )
		//				{
		//					int layerIndex = GetMaxPaintedLayerByCellIndex( cellIndex );
		//					int materialIndex = layerIndexToMaterialIndex[ layerIndex ];
		//					sample.MaterialIndex0 = (byte)materialIndex;
		//					sample.MaterialIndex1 = (byte)materialIndex;
		//				}
		//				else
		//				{
		//					sample.MaterialIndex0 = 0;
		//					sample.MaterialIndex1 = 0;
		//				}
		//			}

		//			//Vector2I holeIndex = new Vector2I( x, y );
		//			//if( IsValidHoleIndex( holeIndex ) )
		//			//   sample.Hole = GetHoleFlag( holeIndex );

		//			samples[ y * length + x ] = sample;
		//		}
		//	}

		//	shape.SampleCount = new Vector2I( length, length );
		//	shape.SamplesScale = new Vector3( horizontalSize / heightmapSize, horizontalSize / heightmapSize, heightMinMaxSize );
		//	shape.Samples = samples;
		//	shape.Thickness = -collisionHeightfieldThickness;

		//	shape.MaterialName = CollisionMaterialName;
		//	shape.ContactGroup = (int)ContactGroup.Collision;
		//	shape.VehicleDrivableSurface = true;

		//	heightFieldBody.PushedToWorld = true;
		//}

		//public void _DebugRenderOverflowPaintLayerTiles( Camera camera )
		//{
		//	GetTiles( delegate ( Tile tile )
		//	{
		//		if( tile.overflowLayers && tile.staticMeshObject != null )
		//		{
		//			Bounds bounds = tile.staticMeshObject.Bounds;
		//			bounds.Expand( .1f );

		//			camera.DebugGeometry.Color = new ColorValue( 1, 0, 0 );
		//			camera.DebugGeometry.AddBounds( bounds );
		//		}
		//	} );
		//}

		//public bool _IsExistsOverflowPaintLayerTiles()
		//{
		//	bool exists = false;
		//	GetTiles( delegate ( Tile tile )
		//	{
		//		if( tile.overflowLayers )
		//			exists = true;
		//	} );
		//	return exists;
		//}

		//internal void SetNeedUpdateGeneratedMaterials()
		//{
		//	needUpdateGeneratedMaterials = true;
		//}

		//internal void SetNeedUpdateGpuParametersOfGeneratedMaterials()
		//{
		//	needUpdateGpuParametersOfGeneratedMaterials = true;
		//}

		//static int GetVertexDeclarationMaxTexCoordIndex( VertexDeclaration declaration )
		//{
		//	int maxTexCoordIndex = 0;
		//	foreach( VertexElement element in declaration.GetElements() )
		//	{
		//		if( element.Semantic == VertexElementSemantic.TextureCoordinates )
		//		{
		//			if( element.Index > maxTexCoordIndex )
		//				maxTexCoordIndex = element.Index;
		//		}
		//	}
		//	return maxTexCoordIndex;
		//}

		//VertexData CreateTileVertexData( int vertexCount )
		//{
		//	VertexData vertexData = new VertexData();

		//	//shared vertex buffers mode
		//	vertexData.VertexDeclaration.AddElement( 0, sharedVertexBuffers_positionChannelOffset,
		//		VertexElementType.Float3, VertexElementSemantic.Position );
		//	//!!!!
		//	//if( LODSettings.Enabled )
		//	//{
		//	//	vertexData.VertexDeclaration.AddElement( 0, sharedVertexBuffers_morphOffsetChannelOffset,
		//	//		VertexElementType.Float2, VertexElementSemantic.TextureCoordinates, 0 );
		//	//}

		//	int vertexSize = vertexData.VertexDeclaration.GetVertexSizeInBytes( 0 );

		//	HardwareBuffer.Usage usage = EntitySystemWorld.Instance.IsEditor() ?
		//		HardwareBuffer.Usage.DynamicWriteOnly : HardwareBuffer.Usage.StaticWriteOnly;
		//	HardwareVertexBuffer buffer = HardwareBufferManager.Instance.
		//		CreateVertexBuffer( vertexSize, vertexCount, usage );
		//	vertexData.VertexBufferBinding.SetBinding( 0, buffer, true );

		//	vertexData.VertexCount = vertexCount;

		//	return vertexData;
		//}

		//protected override void Editor_GetDataForStaticLightingCalculation(
		//	List<StaticLightingManager.CalculationInputMesh> meshes,
		//	List<StaticLightingManager.CalculationInputMeshObject> meshObjects, List<string> errors )
		//{
		//	base.Editor_GetDataForStaticLightingCalculation( meshes, meshObjects, errors );

		//	GetTiles( delegate ( Tile tile )
		//	{
		//		Bounds tileBounds = Bounds.Cleared;
		//		{
		//			for( int y = tile.cellIndexMin.Y; y <= tile.cellIndexMax.Y; y++ )
		//				for( int x = tile.cellIndexMin.X; x <= tile.cellIndexMax.X; x++ )
		//					tileBounds.Add( GetPosition( new Vector2I( x, y ) ) );
		//		}

		//		Vector2I originalLightmapSize;
		//		{
		//			Vector2 s = tileBounds.GetSize().ToVector2() * StaticLightingManager.Instance.PixelsPerUnit;
		//			originalLightmapSize = new Vector2I( (int)( s.X + .9999f ), (int)( s.Y + .9999f ) );
		//		}

		//		Vector2I lightmapSize;
		//		{
		//			//expand for 2 pixel border
		//			lightmapSize = originalLightmapSize + new Vector2I( 4, 4 );
		//		}

		//		Rect texCoordBounds;
		//		{
		//			Vector2 size = tileBounds.GetSize().ToVector2() * lightmapSize.ToVector2() /
		//				originalLightmapSize.ToVector2();
		//			texCoordBounds = new Rect( tileBounds.GetCenter().ToVector2() );
		//			texCoordBounds.Expand( size * .5f );
		//		}

		//		Vector2I cellIndexMin;
		//		Vector2I cellIndexMax;
		//		{
		//			cellIndexMin = GetCellIndexByPosition( texCoordBounds.Minimum );
		//			cellIndexMax = GetCellIndexByPosition( texCoordBounds.Maximum ) + new Vector2I( 1, 1 );
		//			ClampCellIndex( ref cellIndexMin );
		//			ClampCellIndex( ref cellIndexMax );
		//		}

		//		//calculate vertices and indices
		//		Vector3[] positions;
		//		Vector3[] normals;
		//		int[] indices;
		//		{
		//			Vector2I size = cellIndexMax - cellIndexMin;
		//			int sizeX = cellIndexMax.X - cellIndexMin.X + 1;

		//			positions = new Vector3[ ( size.X + 1 ) * ( size.Y + 1 ) ];
		//			normals = new Vector3[ ( size.X + 1 ) * ( size.Y + 1 ) ];
		//			indices = new int[ size.X * size.Y * 6 ];

		//			int vertexPosition = 0;
		//			int indexPosition = 0;

		//			//vertices
		//			for( int y = cellIndexMin.Y; y <= cellIndexMax.Y; y++ )
		//			{
		//				for( int x = cellIndexMin.X; x <= cellIndexMax.X; x++ )
		//				{
		//					positions[ vertexPosition ] = GetPosition( new Vector2I( x, y ) );
		//					normals[ vertexPosition ] = GetNormal( new Vector2I( x, y ) );// GetNormal( tile, new Vector2I( x, y ) );
		//					vertexPosition++;
		//				}
		//			}

		//			//indices
		//			for( int y = 0; y < size.Y; y++ )
		//			{
		//				for( int x = 0; x < size.X; x++ )
		//				{
		//					indices[ indexPosition ] = y * sizeX + x;
		//					indexPosition++;
		//					indices[ indexPosition ] = y * sizeX + x + 1;
		//					indexPosition++;
		//					indices[ indexPosition ] = ( y + 1 ) * sizeX + x + 1;
		//					indexPosition++;
		//					indices[ indexPosition ] = ( y + 1 ) * sizeX + x + 1;
		//					indexPosition++;
		//					indices[ indexPosition ] = ( y + 1 ) * sizeX + x;
		//					indexPosition++;
		//					indices[ indexPosition ] = y * sizeX + x;
		//					indexPosition++;
		//				}
		//			}

		//			if( positions.Length != vertexPosition )
		//			{
		//				Log.Fatal( "HeightmapTerrain: _OnEditorGetDataForStaticLighting: " +
		//					"positions.Length != vertexPosition." );
		//			}
		//			if( indices.Length != indexPosition )
		//			{
		//				Log.Fatal( "HeightmapTerrain: _OnEditorGetDataForStaticLighting: " +
		//					"indices.Length != indexPosition." );
		//			}
		//		}

		//		//initialize mesh
		//		string description = string.Format( "HeightmapTerrain tile: {0}", tile.index );

		//		StaticLightingManager.CalculationInputMesh mesh = new StaticLightingManager.
		//			CalculationInputMesh( positions, normals, indices );

		//		//calculate lightmap texture coordinates
		//		if( ReceiveStaticShadows )
		//		{
		//			Vector2[] lightmapTexCoords = new Vector2[ positions.Length ];
		//			for( int n = 0; n < positions.Length; n++ )
		//			{
		//				lightmapTexCoords[ n ] = new Vector2(
		//					( positions[ n ].X - texCoordBounds.Minimum.X ) / texCoordBounds.Size.X,
		//					( positions[ n ].Y - texCoordBounds.Minimum.Y ) / texCoordBounds.Size.Y );
		//			}
		//			mesh.LightmapTexCoords = lightmapTexCoords;
		//		}

		//		meshes.Add( mesh );

		//		//initialize meshObject
		//		StaticLightingManager.CalculationInputMeshObject meshObject =
		//			new StaticLightingManager.CalculationInputMeshObject( Vector3.Zero,
		//			Quat.Identity, new Vector3( 1, 1, 1 ), mesh, description );
		//		if( ReceiveStaticShadows )
		//			meshObject.LightmapSize = lightmapSize;
		//		meshObject.UserData = tile;

		//		meshObjects.Add( meshObject );
		//	} );
		//}

		//protected override void Editor_ApplyDataAfterStaticLightingCalculation(
		//	List<StaticLightingManager.CalculationOutputMeshObject> meshObjects )
		//{
		//	base.Editor_ApplyDataAfterStaticLightingCalculation( meshObjects );

		//	staticLightingItems = new Dictionary<Vector2I, StaticLightingItem>();

		//	Vector2I packedLightmapsSize = StaticLightingManager.Instance._GetPackedLightmapsSize();

		//	foreach( StaticLightingManager.CalculationOutputMeshObject outputItem in meshObjects )
		//	{
		//		Tile tile = outputItem.MeshObject.UserData as Tile;
		//		if( tile == null || tile.terrain != this )
		//			continue;

		//		StaticLightingItem item = new StaticLightingItem();

		//		item.textureNumber = outputItem.TextureNumber;

		//		Vector2I pos = outputItem.TexCoordPosition;
		//		Vector2I size = outputItem.TexCoordSize;

		//		//2 pixel border
		//		pos += new Vector2I( 2, 2 );
		//		size -= new Vector2I( 4, 4 );

		//		//it is necessary to consider that textures can to be reduced in during calculation
		//		item.texCoordScroll = pos.ToVector2() / packedLightmapsSize.ToVector2();
		//		item.texCoordScale = size.ToVector2() / packedLightmapsSize.ToVector2();

		//		staticLightingItems.Add( tile.index, item );
		//	}

		//	if( staticLightingItems.Count == 0 )
		//		staticLightingItems = null;

		//	//update terrain
		//	GetTiles( delegate ( Tile tile )
		//	{
		//		if( tile.staticMeshObject != null )
		//		{
		//			tile.UpdateGpuParameters();
		//			tile.UpdateUseStaticLightingData();
		//		}
		//	} );
		//	SetNeedUpdateGeneratedMaterials();
		//}

		//protected override void Editor_StaticLightingEnableChange()
		//{
		//	base.Editor_StaticLightingEnableChange();

		//	//update terrain
		//	GetTiles( delegate ( Tile tile )
		//	{
		//		if( tile.staticMeshObject != null )
		//		{
		//			tile.UpdateGpuParameters();
		//			tile.UpdateUseStaticLightingData();
		//		}
		//	} );
		//	SetNeedUpdateGeneratedMaterials();
		//}

		//void ClearStaticLightingData()
		//{
		//	staticLightingItems = null;
		//}

		//public Layer AddLayer()
		//{
		//	Layer layer = new Layer( this );

		//	for( int counter = 1; ; counter++ )
		//	{
		//		string name = "layer" + counter.ToString();
		//		bool exists = layers.Exists( delegate ( Layer l )
		//		{
		//			return l.Name == name;
		//		} );
		//		if( !exists )
		//		{
		//			layer.Name = name;
		//			break;
		//		}
		//	}

		//	layers.Add( layer );

		//	layer.PrepareMask();

		//	if( IsPostCreated )
		//	{
		//		if( Enabled )
		//		{
		//			UpdateGeneratedMaterials();
		//			layers.DoUpdateEvent();
		//			GetTiles( delegate ( Tile tile )
		//			{
		//				tile.layers.Clear();
		//			} );
		//			SetNeedUpdateGeneratedMaterials();
		//			UpdateDataForRenderingForChangedLayerMasks( new RectangleI( 0, 0, masksSize, masksSize ), false );
		//		}

		//		SetModified( true );
		//	}

		//	return layer;
		//}

		//public void RemoveLayer( Layer layer )
		//{
		//	layers.Remove( layer );

		//	foreach( Layer l in layers )
		//		l.PrepareMask();

		//	if( IsPostCreated )
		//	{
		//		if( Enabled )
		//		{
		//			UpdateGeneratedMaterials();
		//			layers.DoUpdateEvent();
		//			GetTiles( delegate ( Tile tile )
		//			{
		//				tile.layers.Clear();
		//			} );
		//			SetNeedUpdateGeneratedMaterials();
		//			UpdateDataForRenderingForChangedLayerMasks( new RectangleI( 0, 0, masksSize, masksSize ), false );
		//		}

		//		SetModified( true );
		//	}
		//}

		//public void MoveLayer( Layer layer, int newIndex )
		//{
		//	if( layers.IndexOf( layer ) == newIndex )
		//		return;

		//	layers.Remove( layer );
		//	layers.Insert( newIndex, layer );

		//	foreach( Layer l in layers )
		//		l.PrepareMask();

		//	UpdateGeneratedMaterials();
		//	layers.DoUpdateEvent();
		//	GetTiles( delegate ( Tile tile )
		//	{
		//		tile.layers.Clear();
		//	} );
		//	SetNeedUpdateGeneratedMaterials();
		//	UpdateDataForRenderingForChangedLayerMasks( new RectangleI( 0, 0, masksSize, masksSize ), false );

		//	SetModified( true );
		//}

		//public void _SetLayerCollectionEditorShow( bool value )
		//{
		//	layerCollectionEditorShow = value;
		//}

		//public void _AfterLayerCollectionEditorUpdate( bool layersQueueUpdated )
		//{
		//	foreach( HeightmapTerrain.Layer layer in Layers )
		//		layer.PrepareMask();

		//	UpdateGeneratedMaterials();
		//	Layers.DoUpdateEvent();

		//	if( layersQueueUpdated )
		//	{
		//		//for update
		//		GetTiles( delegate ( Tile tile )
		//		{
		//			tile.layers.Clear();
		//		} );

		//		SetNeedUpdateGeneratedMaterials();
		//		UpdateDataForRenderingForChangedLayerMasks( new RectangleI( 0, 0, masksSize, masksSize ), false );
		//	}
		//}

		//static void PreloadTexture( string textureName )
		//{
		//	Texture texture = TextureManager.Instance.Load( textureName );
		//	if( texture != null )
		//		texture.Touch();
		//}

		//protected override void OnPreloadResources()
		//{
		//	base.OnPreloadResources();

		//	if( Enabled )
		//	{
		//		foreach( Layer layer in layers )
		//		{
		//			if( !string.IsNullOrEmpty( layer.BaseMapGetFullPath() ) )
		//				PreloadTexture( layer.BaseMapGetFullPath() );
		//			if( !string.IsNullOrEmpty( layer.DetailMapGetFullPath() ) )
		//				PreloadTexture( layer.DetailMapGetFullPath() );
		//			if( !string.IsNullOrEmpty( layer.BaseNormalMapGetFullPath() ) )
		//				PreloadTexture( layer.BaseNormalMapGetFullPath() );
		//			if( !string.IsNullOrEmpty( layer.DetailNormalMapGetFullPath() ) )
		//				PreloadTexture( layer.DetailNormalMapGetFullPath() );
		//			if( !string.IsNullOrEmpty( layer.BaseSpecularMapGetFullPath() ) )
		//				PreloadTexture( layer.BaseSpecularMapGetFullPath() );
		//			if( !string.IsNullOrEmpty( layer.DetailSpecularMapGetFullPath() ) )
		//				PreloadTexture( layer.DetailSpecularMapGetFullPath() );
		//		}
		//	}
		//}

		//public IList<_HeightmapTerrainMaterial> _GetGeneratedMaterials()
		//{
		//	return new ReadOnlyCollection<_HeightmapTerrainMaterial>( generatedMaterials );
		//}

		//public static IList<HeightmapTerrain> Instances
		//{
		//	get
		//	{
		//		if( instancesReadOnly == null )
		//			instancesReadOnly = new ReadOnlyCollection<HeightmapTerrain>( instances );
		//		return instancesReadOnly;
		//	}
		//}

		//internal static AmbientOcclusionSizes GetAmbientOcclusionSize( int size )
		//{
		//	switch( size )
		//	{
		//	case 128: return AmbientOcclusionSizes.Size128x128;
		//	case 256: return AmbientOcclusionSizes.Size256x256;
		//	case 512: return AmbientOcclusionSizes.Size512x512;
		//	case 1024: return AmbientOcclusionSizes.Size1024x1024;
		//	case 2048: return AmbientOcclusionSizes.Size2048x2048;
		//	case 4096: return AmbientOcclusionSizes.Size4096x4096;
		//	}
		//	return AmbientOcclusionSizes.Size1024x1024;
		//}

		//internal static int GetAmbientOcclusionSize( AmbientOcclusionSizes size )
		//{
		//	switch( size )
		//	{
		//	case AmbientOcclusionSizes.Size128x128: return 128;
		//	case AmbientOcclusionSizes.Size256x256: return 256;
		//	case AmbientOcclusionSizes.Size512x512: return 512;
		//	case AmbientOcclusionSizes.Size1024x1024: return 1024;
		//	case AmbientOcclusionSizes.Size2048x2048: return 2048;
		//	case AmbientOcclusionSizes.Size4096x4096: return 4096;
		//	default:
		//		Log.Fatal( "HeightmapTerrain: GetAmbientOcclusionMapSize: Invalid value." );
		//		return 0;
		//	}
		//}

		//public byte[] _GetAmbientOcclusionData()
		//{
		//	return aoData;
		//}

		//public void SetAmbientOcclusionData( byte[] data )
		//{
		//	aoData = data;
		//	SetModified( true );
		//}

		//internal bool AOIsEnabledAndValid()
		//{
		//	if( !AmbientOcclusion.Enabled )
		//		return false;

		//	if( aoData == null )
		//		return false;

		//	if( aoData.Length != ambientOcclusion.size * ambientOcclusion.size )
		//		return false;

		//	return true;
		//}

		/*public */
		int GetHeightmapSizeAsInteger()
		{
			return heightmapSize;
		}

		//float[] GetLODLevelDistances()
		//{
		//	if( cachedLODLevelDistances == null )
		//	{
		//		cachedLODLevelDistances = new float[ cachedLodLevelCount - 1 ];

		//		bool warningShowed = false;

		//		for( int level = 0; level < cachedLODLevelDistances.Length; level++ )
		//		{
		//			float value = 100 + (float)level * 200;

		//			Dictionary<string, double> parameters = new Dictionary<string, double>();
		//			parameters[ "level" ] = level;

		//			ExpressionCalculator calculator = new ExpressionCalculator();
		//			double result;
		//			string error;
		//			if( calculator.Calculate( lodSettings.LodLevelDistances, parameters, out result, out error ) )
		//			{
		//				value = (float)result;
		//			}
		//			else
		//			{
		//				if( !warningShowed )
		//				{
		//					warningShowed = true;
		//					Log.Warning( error );
		//				}
		//			}

		//			cachedLODLevelDistances[ level ] = value;
		//		}
		//	}
		//	return cachedLODLevelDistances;
		//}

		//string Translate( string text )
		//{
		//	return ToolsLocalization.Translate( "HeightmapTerrain", text );
		//}

		//void CalculateNeedForHoleTexturesFields()
		//{
		//	holeTextureSize = GetDefaultSizeForGeneratedTextures();
		//	if( holeTextureSize > heightmapSize )
		//		holeTextureSize = heightmapSize;
		//	holeTexturesCount = heightmapSize / holeTextureSize;
		//	tilesCountPerHoleTexture = holeTextureSize / tileSize;
		//}

		//TextureData CreateTextureAsTextureData( string textureName, Vector2I size, PixelFormat format, Texture.Usage usage )
		//{
		//	TextureData textureData = new TextureData();
		//	textureData.texture = TextureManager.Instance.Create( textureName, Texture.Type.Type2D, size, 1, 0, format, usage );
		//	textureData.format = textureData.texture.Format;
		//	HardwarePixelBuffer buffer = textureData.texture.GetBuffer();
		//	textureData.bufferSizeInBytes = buffer.GetSizeInBytes();
		//	textureData.bufferRowPitch = buffer.GetRowPitch();
		//	return textureData;
		//}

		//void CreateHoleTextures()
		//{
		//	DestroyHoleTextures();

		//	CalculateNeedForHoleTexturesFields();

		//	if( !IsFixedPipelineFallback() )
		//	{
		//		holeTextures = new TextureData[ holeTexturesCount, holeTexturesCount ];

		//		Texture.Usage usage;
		//		if( EntitySystemWorld.Instance.IsEditor() )
		//			usage = Texture.Usage.DynamicWriteOnly;
		//		else
		//			usage = Texture.Usage.StaticWriteOnly;

		//		for( int y = 0; y < holeTexturesCount; y++ )
		//		{
		//			for( int x = 0; x < holeTexturesCount; x++ )
		//			{
		//				string textureName = TextureManager.Instance.GetUniqueName(
		//					string.Format( "_GeneratedHeightmapTerrainHole{0}x{1}", x, y ) );
		//				holeTextures[ x, y ] = CreateTextureAsTextureData( textureName, new Vector2I( holeTextureSize, holeTextureSize ),
		//					PixelFormat.ByteLA, usage );
		//			}
		//		}
		//	}
		//}

		//void DestroyHoleTextures()
		//{
		//	if( holeTextures != null )
		//	{
		//		int count = holeTextures.GetLength( 0 );
		//		for( int y = 0; y < count; y++ )
		//			for( int x = 0; x < count; x++ )
		//				holeTextures[ x, y ].texture.Dispose();
		//		holeTextures = null;
		//	}

		//	holeTextureSize = 0;
		//	holeTexturesCount = 0;
		//	tilesCountPerHoleTexture = 0;
		//}

		//public void UpdateHoleTextures( RectangleI holeRectangle, bool fromStreamingThread )
		//{
		//	RectangleI clampedRect = holeRectangle;
		//	ClampHoleIndexRectangle( ref clampedRect );

		//	if( !fromStreamingThread )
		//		LongOperationCallbackManager.CallCallback( "HeightmapTerrain: UpdateHoleTextures" );

		//	if( holeTextures != null )
		//	{
		//		Vector2I textureIndexMin = GetHoleTextureIndexByCellIndex( clampedRect.Minimum );
		//		Vector2I textureIndexMax = GetHoleTextureIndexByCellIndex( clampedRect.Maximum );

		//		for( int textureIndexY = textureIndexMin.Y; textureIndexY <= textureIndexMax.Y; textureIndexY++ )
		//		{
		//			for( int textureIndexX = textureIndexMin.X; textureIndexX <= textureIndexMax.X; textureIndexX++ )
		//			{
		//				TextureData textureData = holeTextures[ textureIndexX, textureIndexY ];

		//				Vector2I textureMin = new Vector2I( textureIndexX, textureIndexY ) * holeTextureSize;
		//				RectangleI localRect = clampedRect - textureMin;
		//				if( localRect.Left < 0 )
		//					localRect.Left = 0;
		//				if( localRect.Top < 0 )
		//					localRect.Top = 0;
		//				if( localRect.Right >= holeTextureSize )
		//					localRect.Right = holeTextureSize - 1;
		//				if( localRect.Bottom >= holeTextureSize )
		//					localRect.Bottom = holeTextureSize - 1;

		//				unsafe
		//				{
		//					HardwarePixelBuffer buffer = null;
		//					byte* pointer;
		//					if( fromStreamingThread )
		//					{
		//						IntPtr data = NativeUtils.Alloc( NativeMemoryAllocationType.Renderer, textureData.bufferSizeInBytes );
		//						streamingEnableDataForTextures.Add( textureData, data );
		//						pointer = (byte*)data;
		//					}
		//					else
		//					{
		//						buffer = textureData.texture.GetBuffer();
		//						pointer = (byte*)buffer.Lock( HardwareBuffer.LockOptions.Normal );
		//					}

		//					for( int y = localRect.Top; y <= localRect.Bottom; y++ )
		//					{
		//						for( int x = localRect.Left; x <= localRect.Right; x++ )
		//						{
		//							byte* p = pointer + ( y * holeTextureSize + x ) * 2;
		//							bool hole = GetHoleFlag( textureMin + new Vector2I( x, y ) );
		//							p[ 0 ] = (byte)( hole ? 0 : 255 );
		//							p[ 1 ] = 1;
		//						}
		//					}

		//					if( buffer != null )
		//						buffer.Unlock();

		//					//HardwarePixelBuffer buffer = texture.GetBuffer();
		//					//byte* pointer = (byte*)buffer.Lock( HardwareBuffer.LockOptions.Normal );

		//					//for( int y = localRect.Top; y <= localRect.Bottom; y++ )
		//					//{
		//					//   for( int x = localRect.Left; x <= localRect.Right; x++ )
		//					//   {
		//					//      byte* p = pointer + ( y * holeTextureSize + x ) * 2;
		//					//      bool hole = GetHoleFlag( textureMin + new Vector2I( x, y ) );
		//					//      p[ 0 ] = (byte)( hole ? 0 : 255 );
		//					//      p[ 1 ] = 1;
		//					//   }
		//					//}
		//					//buffer.Unlock();
		//				}
		//			}
		//		}
		//	}
		//}

		//internal Vector2I GetHoleTextureIndexByTileIndex( Vector2I tileIndex )
		//{
		//	return tileIndex / tilesCountPerHoleTexture;
		//}

		//Vector2I GetHoleTextureIndexByCellIndex( Vector2I index )
		//{
		//	return index / holeTextureSize;
		//}

		//public void _EditorShowInformationMessageBox()
		//{
		//	List<string> lines = new List<string>();

		//	int systemMemory = 0;
		//	int staticMeshObjectCount = 0;
		//	int textureCount = 0;
		//	int textureSize = 0;
		//	int vertexBufferCount = 0;
		//	int vertexBufferMemory = 0;
		//	int indexBufferCount = 0;
		//	int indexBufferMemory = 0;
		//	int materialCount = 0;
		//	int collisionBodyCount = 0;


		//	foreach( Layer layer in layers )
		//	{
		//		if( layer.mask != null )
		//			systemMemory += layer.mask.Length;

		//		if( !string.IsNullOrEmpty( layer.BaseMapGetFullPath() ) )
		//		{
		//			Texture texture = TextureManager.Instance.GetByName( layer.BaseMapGetFullPath() );
		//			if( texture != null )
		//			{
		//				textureCount++;
		//				textureSize += texture.GetSizeInBytes();
		//			}
		//		}
		//		if( !string.IsNullOrEmpty( layer.DetailMapGetFullPath() ) )
		//		{
		//			Texture texture = TextureManager.Instance.GetByName( layer.DetailMapGetFullPath() );
		//			if( texture != null )
		//			{
		//				textureCount++;
		//				textureSize += texture.GetSizeInBytes();
		//			}
		//		}
		//		if( !string.IsNullOrEmpty( layer.BaseNormalMapGetFullPath() ) )
		//		{
		//			Texture texture = TextureManager.Instance.GetByName( layer.BaseNormalMapGetFullPath() );
		//			if( texture != null )
		//			{
		//				textureCount++;
		//				textureSize += texture.GetSizeInBytes();
		//			}
		//		}
		//		if( !string.IsNullOrEmpty( layer.DetailNormalMapGetFullPath() ) )
		//		{
		//			Texture texture = TextureManager.Instance.GetByName( layer.DetailNormalMapGetFullPath() );
		//			if( texture != null )
		//			{
		//				textureCount++;
		//				textureSize += texture.GetSizeInBytes();
		//			}
		//		}
		//		if( !string.IsNullOrEmpty( layer.BaseSpecularMapGetFullPath() ) )
		//		{
		//			Texture texture = TextureManager.Instance.GetByName( layer.BaseSpecularMapGetFullPath() );
		//			if( texture != null )
		//			{
		//				textureCount++;
		//				textureSize += texture.GetSizeInBytes();
		//			}
		//		}
		//		if( !string.IsNullOrEmpty( layer.DetailSpecularMapGetFullPath() ) )
		//		{
		//			Texture texture = TextureManager.Instance.GetByName( layer.DetailSpecularMapGetFullPath() );
		//			if( texture != null )
		//			{
		//				textureCount++;
		//				textureSize += texture.GetSizeInBytes();
		//			}
		//		}

		//	}

		//	if( heightmapBuffer != null )
		//		systemMemory += heightmapBuffer.Length * 2;
		//	if( holeBuffer != null )
		//		systemMemory += holeBuffer.Length / 8;

		//	if( aoData != null )
		//		systemMemory += aoData.Length;

		//	for( int y = 0; y < aoTexturesCount; y++ )
		//	{
		//		for( int x = 0; x < aoTexturesCount; x++ )
		//		{
		//			TextureData textureData = aoTextures[ x, y ];
		//			if( textureData != null )
		//			{
		//				textureCount++;
		//				textureSize += textureData.texture.GetSizeInBytes();
		//			}
		//		}
		//	}

		//	if( tiles != null )
		//	{
		//		GetTiles( delegate ( Tile tile )
		//		{
		//			if( tile.staticMeshObject != null )
		//				staticMeshObjectCount++;

		//			if( tile.vertexDatasForNonSharedVertexBuffersMode != null )
		//			{
		//				foreach( VertexData vertexData in tile.vertexDatasForNonSharedVertexBuffersMode )
		//				{
		//					vertexBufferCount++;
		//					vertexBufferMemory += vertexData.VertexBufferBinding.GetBuffer( 0 ).GetSizeInBytes();
		//				}
		//			}
		//		} );
		//	}

		//	if( masksTextures != null )
		//	{
		//		for( int y = 0; y < masksTexturesCount; y++ )
		//		{
		//			for( int x = 0; x < masksTexturesCount; x++ )
		//			{
		//				MaskTextureItem textureItem = masksTextures[ x, y ];
		//				if( textureItem.shaderTexture != null )
		//				{
		//					textureCount++;
		//					textureSize += textureItem.shaderTexture.texture.GetSizeInBytes();
		//				}
		//				if( textureItem.ffpTextures != null )
		//				{
		//					foreach( TextureData textureData in textureItem.ffpTextures )
		//					{
		//						if( textureData != null )
		//						{
		//							textureCount++;
		//							textureSize += textureData.texture.GetSizeInBytes();
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}

		//	if( normalsTextures != null )
		//	{
		//		for( int y = 0; y < normalsTexturesCount; y++ )
		//		{
		//			for( int x = 0; x < normalsTexturesCount; x++ )
		//			{
		//				TextureData textureData = normalsTextures[ x, y ];
		//				if( textureData != null )
		//				{
		//					textureCount++;
		//					textureSize += textureData.texture.GetSizeInBytes();
		//				}
		//			}
		//		}
		//	}

		//	if( heightTextures != null )
		//	{
		//		for( int y = 0; y < normalsTexturesCount; y++ )
		//		{
		//			for( int x = 0; x < normalsTexturesCount; x++ )
		//			{
		//				TextureData textureData = heightTextures[ x, y ];
		//				if( textureData != null )
		//				{
		//					textureCount++;
		//					textureSize += textureData.texture.GetSizeInBytes();
		//				}
		//			}
		//		}
		//	}

		//	if( holeTextures != null )
		//	{
		//		for( int y = 0; y < holeTexturesCount; y++ )
		//		{
		//			for( int x = 0; x < holeTexturesCount; x++ )
		//			{
		//				TextureData textureData = holeTextures[ x, y ];
		//				if( textureData != null )
		//				{
		//					textureCount++;
		//					textureSize += textureData.texture.GetSizeInBytes();
		//				}
		//			}
		//		}
		//	}

		//	if( sharedVertexDatas != null )
		//	{
		//		foreach( VertexData vertexData in sharedVertexDatas )
		//		{
		//			vertexBufferCount++;
		//			vertexBufferMemory += vertexData.VertexBufferBinding.GetBuffer( 0 ).GetSizeInBytes();
		//		}
		//	}

		//	if( sharedIndexDatas != null )
		//	{
		//		foreach( IndexData indexData in sharedIndexDatas )
		//		{
		//			indexBufferCount++;
		//			indexBufferMemory += indexData.IndexBuffer.GetSizeInBytes();
		//		}
		//	}

		//	materialCount += generatedMaterials.Count;

		//	if( heightFieldBody != null )
		//		collisionBodyCount++;
		//	if( bodies != null )
		//		collisionBodyCount += bodies.Length;

		//	int totalMemory = systemMemory + textureSize + vertexBufferMemory + indexBufferMemory;

		//	string renderingMode;
		//	if( IsUseSharedVertexBuffers() )
		//		renderingMode = "Shared vertex buffers (VTF) (best mode)";
		//	else if( IsFixedPipelineFallback() )
		//		renderingMode = "Fixed pipeline fallback (old hardware mode)";
		//	else
		//		renderingMode = "Non shared vertex buffers (old hardware mode or OpenGL)";

		//	lines.Add( Translate( "Technical information:" ) );
		//	lines.Add( "" );
		//	lines.Add( string.Format( "Rendering mode: {0}", renderingMode ) );
		//	if( IsUseSharedVertexBuffers() && heightTextures != null )
		//		lines.Add( string.Format( "Height texture format: {0}", heightTextures[ 0, 0 ].texture.Format.ToString() ) );
		//	lines.Add( string.Format( "System memory: {0} mb", systemMemory / 1024 / 1024 ) );
		//	lines.Add( string.Format( "StaticMeshObject count: {0}", staticMeshObjectCount ) );
		//	lines.Add( "Textures:" );
		//	lines.Add( string.Format( "    Count: {0}", textureCount ) );
		//	lines.Add( string.Format( "    Memory: {0} mb", textureSize / 1024 / 1024 ) );
		//	lines.Add( "Vertex/index buffers:" );
		//	lines.Add( string.Format( "    Vertex buffer count: {0}", vertexBufferCount ) );
		//	lines.Add( string.Format( "    Vertex buffer memory: {0} mb", vertexBufferMemory / 1024 / 1024 ) );
		//	lines.Add( string.Format( "    Index buffer count: {0}", indexBufferCount ) );
		//	lines.Add( string.Format( "    Index buffer memory: {0} mb", indexBufferMemory / 1024 / 1024 ) );
		//	lines.Add( string.Format( "Material count: {0}", materialCount ) );
		//	lines.Add( string.Format( "Collision bodies count: {0}", collisionBodyCount ) );
		//	lines.Add( "" );
		//	lines.Add( string.Format( "Total memory: {0} mb", totalMemory / 1024 / 1024 ) );

		//	StringBuilder text = new StringBuilder();
		//	foreach( string line in lines )
		//	{
		//		if( text.Length != 0 )
		//			text.Append( "\n" );
		//		text.Append( line );
		//	}
		//	EditorMessageBox.Show( text.ToString(), Translate( "Technical Information" ), EditorMessageBox.Buttons.OK,
		//		EditorMessageBox.Icon.Information );
		//}

		//internal void CalculateNeedForNormalsHeightTexturesFields()
		//{
		//	normalsTextureSize = GetDefaultSizeForGeneratedTextures();

		//	int pixelsPerTile = tileSize + 3;

		//	tilesCountPerNormalsTexture = normalsTextureSize / pixelsPerTile;
		//	if( tilesCountPerNormalsTexture == 0 )
		//		Log.Fatal( "HeightmapTerrain: CalculateNeedForHeightTexturesFields: tilesCountPerHeightTexture == 0." );

		//	normalsTexturesCount = tileCount / tilesCountPerNormalsTexture;
		//	if( tileCount % tilesCountPerNormalsTexture != 0 )
		//		normalsTexturesCount++;
		//}

		//void CreateNormalsHeightTextures()
		//{
		//	DestroyHeightNormalsTextures();

		//	CalculateNeedForNormalsHeightTexturesFields();

		//	Texture.Usage usage;
		//	if( EntitySystemWorld.Instance.IsEditor() )
		//		usage = Texture.Usage.DynamicWriteOnly;
		//	else
		//		usage = Texture.Usage.StaticWriteOnly;

		//	{
		//		normalsTextures = new TextureData[ normalsTexturesCount, normalsTexturesCount ];
		//		for( int y = 0; y < normalsTexturesCount; y++ )
		//		{
		//			for( int x = 0; x < normalsTexturesCount; x++ )
		//			{
		//				string textureName = TextureManager.Instance.GetUniqueName(
		//					string.Format( "_GeneratedHeightmapTerrainNormal{0}x{1}", x, y ) );

		//				//TO DO: android will have another format?
		//				normalsTextures[ x, y ] = CreateTextureAsTextureData( textureName,
		//					new Vector2I( normalsTextureSize, normalsTextureSize ), PixelFormat.A8R8G8B8, usage );
		//			}
		//		}
		//	}

		//	{
		//		PixelFormat format;
		//		//if( useFloat16BitHeightTexture && TextureManager.Instance.IsVertexTextureFormatSupported( PixelFormat.Float16R ) )
		//		//	format = PixelFormat.Float16R;
		//		//else
		//		format = PixelFormat.Float32R;

		//		heightTextures = new TextureData[ normalsTexturesCount, normalsTexturesCount ];
		//		for( int y = 0; y < normalsTexturesCount; y++ )
		//		{
		//			for( int x = 0; x < normalsTexturesCount; x++ )
		//			{
		//				string textureName = TextureManager.Instance.GetUniqueName(
		//					string.Format( "_GeneratedHeightmapTerrainHeight{0}x{1}", x, y ) );
		//				heightTextures[ x, y ] = CreateTextureAsTextureData( textureName,
		//					new Vector2I( normalsTextureSize, normalsTextureSize ), format, usage );
		//			}
		//		}
		//	}
		//}

		//void DestroyHeightNormalsTextures()
		//{
		//	if( normalsTextures != null )
		//	{
		//		int count = normalsTextures.GetLength( 0 );
		//		for( int y = 0; y < count; y++ )
		//			for( int x = 0; x < count; x++ )
		//				normalsTextures[ x, y ].texture.Dispose();
		//		normalsTextures = null;
		//	}

		//	if( heightTextures != null )
		//	{
		//		int count = heightTextures.GetLength( 0 );
		//		for( int y = 0; y < count; y++ )
		//			for( int x = 0; x < count; x++ )
		//				heightTextures[ x, y ].texture.Dispose();
		//		heightTextures = null;
		//	}

		//	normalsTextureSize = 0;
		//	normalsTexturesCount = 0;
		//	tilesCountPerNormalsTexture = 0;
		//}

		//public void UpdateNormalsHeightTextures( RectangleI cellRectangle, bool fromStreamingThread )
		//{
		//	//float time = EngineApp.Instance.Time;

		//	bool fullUpdate = cellRectangle == new RectangleI( Vector2I.Zero, new Vector2I( heightmapSize - 1, heightmapSize - 1 ) );
		//	Vector3[,] calculatedPositionsForGetNormals = null;
		//	if( fullUpdate )
		//	{
		//		int length = heightmapSize + 1;
		//		calculatedPositionsForGetNormals = new Vector3[ length, length ];
		//		for( int y = 0; y < length; y++ )
		//			for( int x = 0; x < length; x++ )
		//				calculatedPositionsForGetNormals[ x, y ] = GetPosition_ForGetNormal( new Vector2I( x, y ), null );
		//	}

		//	if( !fromStreamingThread )
		//		LongOperationCallbackManager.CallCallback( "HeightmapTerrain: UpdateNormalsHeightTextures" );

		//	if( normalsTextures != null )
		//	{
		//		Dictionary<Vector2I, List<Tile>> textureIndexes = new Dictionary<Vector2I, List<Tile>>( 16 );
		//		{
		//			GetTiles( delegate ( Tile tile )
		//			{
		//				RectangleI rect = GetCellIndexesForTileForNormalsTextures( tile );
		//				if( cellRectangle.IsIntersectsRect( rect ) )
		//				{
		//					Vector2I textureIndex = GetNormalsTextureIndexByTileIndex( tile.index );

		//					List<Tile> affectedTiles;
		//					if( !textureIndexes.TryGetValue( textureIndex, out affectedTiles ) )
		//					{
		//						affectedTiles = new List<Tile>();
		//						textureIndexes.Add( textureIndex, affectedTiles );
		//					}
		//					affectedTiles.Add( tile );
		//				}
		//			} );
		//		}

		//		foreach( KeyValuePair<Vector2I, List<Tile>> pair in textureIndexes )
		//		{
		//			Vector2I textureIndex = pair.Key;
		//			List<Tile> affectedTiles = pair.Value;

		//			//normals texture
		//			{
		//				TextureData textureData = normalsTextures[ textureIndex.X, textureIndex.Y ];

		//				unsafe
		//				{
		//					HardwarePixelBuffer buffer = null;
		//					byte* pointer;
		//					if( fromStreamingThread )
		//					{
		//						IntPtr data = NativeUtils.Alloc( NativeMemoryAllocationType.Renderer, textureData.bufferSizeInBytes );
		//						streamingEnableDataForTextures.Add( textureData, data );
		//						pointer = (byte*)data;
		//					}
		//					else
		//					{
		//						buffer = textureData.texture.GetBuffer();
		//						pointer = (byte*)buffer.Lock( HardwareBuffer.LockOptions.Normal );
		//					}
		//					//HardwarePixelBuffer buffer = textureData.texture.GetBuffer();
		//					//byte* pointer = (byte*)buffer.Lock( HardwareBuffer.LockOptions.Normal );
		//					PixelFormat format = textureData.texture.Format;

		//					foreach( Tile tile in affectedTiles )
		//					{
		//						Vector2I tileIndexInTexture = new Vector2I(
		//							tile.index.X % tilesCountPerNormalsTexture,
		//							tile.index.Y % tilesCountPerNormalsTexture );
		//						RectangleI localRect = new RectangleI(
		//							tileIndexInTexture * ( tileSize + 3 ),
		//							( tileIndexInTexture + new Vector2I( 1, 1 ) ) * ( tileSize + 3 ) - new Vector2I( 1, 1 ) );
		//						if( localRect.Maximum.X >= normalsTextureSize )
		//							Log.Fatal( "HeightmapTerrain: UpdateNormalsHeightTextures: localRect.Maximum.X >= heightTextureSize." );
		//						if( localRect.Maximum.Y >= normalsTextureSize )
		//							Log.Fatal( "HeightmapTerrain: UpdateNormalsHeightTextures: localRect.Maximum.Y >= heightTextureSize." );

		//						Vector2I textureMin = tile.cellIndexMin - new Vector2I( 1, 1 );
		//						Vector2I cellIndexFrom = textureMin - localRect.Minimum;

		//						for( int y = localRect.Top; y <= localRect.Bottom; y++ )
		//						{
		//							for( int x = localRect.Left; x <= localRect.Right; x++ )
		//							{
		//								Vector2I cellIndex = cellIndexFrom + new Vector2I( x, y );
		//								ClampCellIndex( ref cellIndex );

		//								Vector3 v = ( GetNormal( cellIndex, calculatedPositionsForGetNormals ) + new Vector3( 1, 1, 1 ) ) * .5f;
		//								byte* p = pointer + ( y * normalsTextureSize + x ) * 4;
		//								*( p + 0 ) = 255;
		//								*( p + 1 ) = (byte)( (float)v.X * 255.0f );
		//								*( p + 2 ) = (byte)( (float)v.Y * 255.0f );
		//								*( p + 3 ) = (byte)( (float)v.Z * 255.0f );
		//							}
		//						}
		//					}

		//					if( buffer != null )
		//						buffer.Unlock();
		//				}
		//			}

		//			if( heightTextures != null )
		//			{
		//				TextureData textureData = heightTextures[ textureIndex.X, textureIndex.Y ];

		//				unsafe
		//				{
		//					HardwarePixelBuffer buffer = null;
		//					byte* pointer;
		//					if( fromStreamingThread )
		//					{
		//						IntPtr data = NativeUtils.Alloc( NativeMemoryAllocationType.Renderer, textureData.bufferSizeInBytes );
		//						streamingEnableDataForTextures.Add( textureData, data );
		//						pointer = (byte*)data;
		//					}
		//					else
		//					{
		//						buffer = textureData.texture.GetBuffer();
		//						pointer = (byte*)buffer.Lock( HardwareBuffer.LockOptions.Normal );
		//					}
		//					//HardwarePixelBuffer buffer = textureData.texture.GetBuffer();
		//					//byte* pointer = (byte*)buffer.Lock( HardwareBuffer.LockOptions.Normal );
		//					PixelFormat format = textureData.texture.Format;

		//					foreach( Tile tile in affectedTiles )
		//					{
		//						Vector2I tileIndexInTexture = new Vector2I(
		//							tile.index.X % tilesCountPerNormalsTexture,
		//							tile.index.Y % tilesCountPerNormalsTexture );
		//						RectangleI localRect = new RectangleI(
		//							tileIndexInTexture * ( tileSize + 3 ),
		//							( tileIndexInTexture + new Vector2I( 1, 1 ) ) * ( tileSize + 3 ) - new Vector2I( 1, 1 ) );
		//						if( localRect.Maximum.X >= normalsTextureSize )
		//							Log.Fatal( "HeightmapTerrain: UpdateNormalsHeightTextures: localRect.Maximum.X >= heightTextureSize." );
		//						if( localRect.Maximum.Y >= normalsTextureSize )
		//							Log.Fatal( "HeightmapTerrain: UpdateNormalsHeightTextures: localRect.Maximum.Y >= heightTextureSize." );

		//						Vector2I textureMin = tile.cellIndexMin - new Vector2I( 1, 1 );
		//						Vector2I cellIndexFrom = textureMin - localRect.Minimum;

		//						for( int y = localRect.Top; y <= localRect.Bottom; y++ )
		//						{
		//							for( int x = localRect.Left; x <= localRect.Right; x++ )
		//							{
		//								Vector2I cellIndex = cellIndexFrom + new Vector2I( x, y );
		//								ClampCellIndex( ref cellIndex );

		//								//!!!!double. где еще
		//								var v = GetHeight( cellIndex );
		//								//if( format == PixelFormat.Float16R )
		//								//{
		//								//	byte* p = pointer + ( y * normalsTextureSize + x ) * 2;
		//								//	*(ushort*)p = Half.SingleToHalf( v ).Value;
		//								//}
		//								//else
		//								//{
		//								byte* p = pointer + ( y * normalsTextureSize + x ) * 4;
		//								//!!!!тут по сути смещение относительное от Position
		//								*(float*)p = (float)v;
		//								//}
		//							}
		//						}
		//					}

		//					if( buffer != null )
		//						buffer.Unlock();
		//				}
		//			}

		//		}
		//	}

		//	//Log.Info( "time: " + ( EngineApp.Instance.Time - time ).ToString() );
		//}

		RectangleI GetCellIndexesForTileForNormalsTextures( Tile tile )
		{
			Vector2I min = tile.cellIndexMin - new Vector2I( 1, 1 );
			Vector2I max = tile.cellIndexMax + new Vector2I( 1, 1 );
			ClampCellIndex( ref min );
			ClampCellIndex( ref max );
			return new RectangleI( min, max );
		}

		//internal Vector2I GetNormalsTextureIndexByTileIndex( Vector2I tileIndex )
		//{
		//	return tileIndex / tilesCountPerNormalsTexture;
		//}

		int GetDefaultSizeForGeneratedTextures()
		{
			//this value must be less than TileSize.

			return 512;
		}

		//int GetBodyCellSize()
		//{
		//	return EntitySystemWorld.Instance.IsEditor() ? 32 : 128;
		//}

		//int GetBodyCountByAxis()
		//{
		//	int bodyCellSize = GetBodyCellSize();
		//	int bodyCount = heightmapSize / bodyCellSize;
		//	if( heightmapSize % bodyCellSize != 0 )
		//		bodyCount++;
		//	return bodyCount;
		//}

		//public void UpdateAllRecreatableDataForRendering()
		//{
		//	if( loadingStatus != LoadingStatuses.Loaded )
		//		Log.Fatal( "HeightmapTerrain: UpdateAllRecreatableDataForRendering: loadingStatus != LoadingStatuses.Loaded." );
		//	if( streamingEnable )
		//		Log.Fatal( "HeightmapTerrain: UpdateAllRecreatableDataForRendering: streamingEnable == True." );

		//	DestroyAllRecreatableDataForRendering();

		//	if( Enabled )
		//	{
		//		CalculateCachedGeneralData();
		//		GenerateTiles();
		//		CreateNormalsHeightTextures();
		//		CreateMasksTextures( true );
		//		CreateHoleTextures();
		//		CreateAOTextures();

		//		UpdateDataForRenderingForChangedLayerMasks( new RectangleI( 0, 0, masksSize, masksSize ), false );
		//		UpdateNormalsHeightTextures( new RectangleI( Vector2I.Zero, new Vector2I( heightmapSize - 1, heightmapSize - 1 ) ), false );
		//		UpdateHoleTextures( new RectangleI( Vector2I.Zero, new Vector2I( heightmapSize - 1, heightmapSize - 1 ) ), false );
		//		UpdateAOTextures( false );
		//	}
		//}

		//public void DestroyAllRecreatableDataForRendering()
		//{
		//	DestroyTiles();
		//	DestroyGeneratedMaterials();
		//	DestroyHeightNormalsTextures();
		//	DestroyMasksTextures();
		//	DestroyHoleTextures();
		//	DestroyAOTextures();
		//	cachedLODLevelDistances = null;

		//	foreach( KeyValuePair<TextureData, IntPtr> pair in streamingEnableDataForTextures )
		//		NativeUtils.Free( pair.Value );
		//	streamingEnableDataForTextures = new Dictionary<TextureData, IntPtr>();
		//}

		//void RenderLODLevels( Camera camera )
		//{
		//   ColorValue[] colors = new ColorValue[]
		//   {
		//      new ColorValue( 1, 0, 0, 1 ),
		//      new ColorValue( 0, 1, 0, 1 ),
		//      new ColorValue( 0, 0, 1, 1 )
		//   };

		//   GetTiles( delegate( Tile tile )
		//   {
		//      float cameraDistance = tile.GetCameraDistance( camera );
		//      int lodLevel = tile.CalculateLODLevel( cameraDistance );

		//      if( tile.staticMeshObject != null )
		//      {
		//         camera.DebugGeometry.Color = colors[ lodLevel % 3 ];
		//         Bounds bounds = tile.staticMeshObject.Bounds;
		//         bounds.Expand( -.2f );
		//         camera.DebugGeometry.AddBounds( bounds );
		//      }
		//   } );
		//}

		public void UpdateRenderingData( RectangleI cellRectangle, bool meshExtractData, bool updateHoles, bool allowHoles )
		//public void UpdateRenderingData( RectangleI cellRectangle, bool meshExtractData = true, bool allowHoles = true )
		{
			//if( streamingEnable )
			//	Log.Fatal( "HeightmapTerrain: UpdateDataForRenderingForChangedHeights: streamingEnable == True." );

			//LongOperationCallbackManager.CallCallback( "HeightmapTerrain: UpdateDataForRenderingForChangedHeights" );

			const int border = 2;
			Vector2I cellIndexMin = cellRectangle.Minimum - new Vector2I( border, border );
			Vector2I cellIndexMax = cellRectangle.Maximum + new Vector2I( border, border );
			ClampCellIndex( ref cellIndexMin );
			ClampCellIndex( ref cellIndexMax );

			//!!!!
			//UpdateNormalsHeightTextures( new RectangleI( cellIndexMin, cellIndexMax ), false );

			Vector2I tileMin = GetTileIndexByCellIndex( cellIndexMin );
			Vector2I tileMax = GetTileIndexByCellIndex( cellIndexMax );
			ClampTileIndex( ref tileMin );
			ClampTileIndex( ref tileMax );

			GetTiles( tileMin, tileMax, delegate ( Tile tile )
			{
				if( tile.ObjectInSpace != null )
					tile.UpdateRenderingData( meshExtractData, null, updateHoles, allowHoles );

				////update staticMeshObject bounds
				//if( tile.staticMeshObject != null )
				//{
				//	Bounds bounds = Bounds.Cleared;
				//	for( int y = tile.cellIndexMin.Y; y <= tile.cellIndexMax.Y; y++ )
				//		for( int x = tile.cellIndexMin.X; x <= tile.cellIndexMax.X; x++ )
				//			bounds.Add( GetPosition( new Vector2I( x, y ) ) );
				//	if( !bounds.Equals( tile.staticMeshObject.Bounds, .001f ) )
				//		tile.staticMeshObject.UpdateBounds( bounds );
				//}
			} );
		}

		public void UpdateCollisionData( RectangleI cellRectangle )
		{
			if( Collision )
			{
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
					if( tile.ObjectInSpace != null )
						tile.UpdateCollisionBody();
				} );
			}
		}

		//public void UpdateDataForRenderingForChangedLayerMasks( RectangleI maskIndexRectangle, bool fromStreamingThread )
		//{
		//	if( !fromStreamingThread )
		//		LongOperationCallbackManager.CallCallback( "HeightmapTerrain: UpdateDataForRenderingForChangedLayerMasks" );

		//	const int border = 2;
		//	Vector2I maskIndexMin = maskIndexRectangle.Minimum - new Vector2I( border, border );
		//	Vector2I maskIndexMax = maskIndexRectangle.Maximum + new Vector2I( border, border );
		//	ClampMaskIndex( ref maskIndexMin );
		//	ClampMaskIndex( ref maskIndexMax );

		//	//update tile.layers lists and materials
		//	{
		//		Vector2I tileIndexMin = GetTileIndexByMaskIndex( maskIndexMin );
		//		Vector2I tileIndexMax = GetTileIndexByMaskIndex( maskIndexMax );
		//		ClampTileIndex( ref tileIndexMin );
		//		ClampTileIndex( ref tileIndexMax );

		//		GetTiles( tileIndexMin, tileIndexMax, delegate ( Tile tile )
		//		{
		//			if( !fromStreamingThread )
		//			{
		//				LongOperationCallbackManager.CallCallback(
		//					"HeightmapTerrain: UpdateDataForRenderingForChangedLayerMasks: Tile: " +
		//					tile.index.ToString() );
		//			}

		//			if( tile.UpdateLayersList() && tile.staticMeshObject != null )
		//			{
		//				//layers list is changed

		//				//expand maskIndexMin, maskIndexMax
		//				{
		//					Vector2I i;
		//					i = GetMaskIndexByCellIndex( tile.cellIndexMin );
		//					if( i.X < maskIndexMin.X ) maskIndexMin.X = i.X;
		//					if( i.Y < maskIndexMin.Y ) maskIndexMin.Y = i.Y;
		//					if( i.X > maskIndexMax.X ) maskIndexMax.X = i.X;
		//					if( i.Y > maskIndexMax.Y ) maskIndexMax.Y = i.Y;
		//					i = GetMaskIndexByCellIndex( tile.cellIndexMax + new Vector2I( 1, 1 ) ) - new Vector2I( 1, 1 );
		//					if( i.X < maskIndexMin.X ) maskIndexMin.X = i.X;
		//					if( i.Y < maskIndexMin.Y ) maskIndexMin.Y = i.Y;
		//					if( i.X > maskIndexMax.X ) maskIndexMax.X = i.X;
		//					if( i.Y > maskIndexMax.Y ) maskIndexMax.Y = i.Y;
		//				}

		//				//update material
		//				if( !fromStreamingThread )
		//				{
		//					_HeightmapTerrainMaterial material = GenerateMaterialForTile( tile );//, false );
		//					tile.staticMeshObject.Material = ( material != null ) ? material.BaseMaterial : null;
		//					tile.UpdateMaterialTechniquesInfo();
		//				}
		//			}

		//			//update vertices for fixed pipeline mode
		//			if( IsFixedPipelineFallback() )
		//				UpdateTileVertexDatasForNonSharedVertexBuffersMode( tile );//, false );
		//		} );
		//	}

		//	//update generated textures
		//	if( !fromStreamingThread )
		//		UpdateMasksTextures( maskIndexMin, maskIndexMax, fromStreamingThread );
		//}

		//public Body[] GetBodies()
		//{
		//	List<Body> list = new List<Body>();
		//	if( heightFieldBody != null )
		//		list.Add( heightFieldBody );
		//	if( bodies != null )
		//	{
		//		int bodyCount = GetBodyCountByAxis();
		//		for( int y = 0; y < bodyCount; y++ )
		//		{
		//			for( int x = 0; x < bodyCount; x++ )
		//			{
		//				Body body = bodies[ x, y ];
		//				if( body != null )
		//					list.Add( body );
		//			}
		//		}
		//	}
		//	return list.ToArray();
		//}

		//public List<StaticMeshObject> GetStaticMeshObjects()
		//{
		//	List<StaticMeshObject> result = new List<StaticMeshObject>( 128 );
		//	GetTiles( delegate ( Tile tile )
		//	{
		//		if( tile.staticMeshObject != null )
		//			result.Add( tile.staticMeshObject );
		//	} );
		//	return result;
		//}

		//public bool LoadAllData( bool useStreaming )
		//{
		//	if( loadingStatus == LoadingStatuses.Loading )
		//		Log.Fatal( "HeightmapTerrain: LoadAllData: loadingStatus == LoadingStatuses.Loading." );
		//	if( loadingStatus == LoadingStatuses.Loaded )
		//		Log.Fatal( "HeightmapTerrain: LoadAllData: loadingStatus == LoadingStatuses.Loaded." );

		//	UnloadAllData();

		//	if( useStreaming )
		//	{
		//		loadingStatus = LoadingStatuses.Loading;

		//		streamingLoadError = false;
		//		streamingLoadTextureTasks.Clear();

		//		foreach( Layer layer in layers )
		//		{
		//			for( int n = 0; n < 6; n++ )
		//			{
		//				string textureName = "";
		//				switch( n )
		//				{
		//				case 0: textureName = layer.BaseMapGetFullPath(); break;
		//				case 1: textureName = layer.DetailMapGetFullPath(); break;
		//				case 2: textureName = layer.BaseNormalMapGetFullPath(); break;
		//				case 3: textureName = layer.DetailNormalMapGetFullPath(); break;
		//				case 4: textureName = layer.BaseSpecularMapGetFullPath(); break;
		//				case 5: textureName = layer.DetailSpecularMapGetFullPath(); break;
		//				}

		//				if( !string.IsNullOrEmpty( textureName ) )
		//				{
		//					if( TextureManager.Instance.GetByName( textureName ) == null )
		//					{
		//						var ticket = ResourceBackgroundQueue.Instance.LoadTexture( textureName );
		//						if( ticket != null )
		//							streamingLoadTextureTasks.Add( ticket );
		//					}
		//				}
		//			}
		//		}

		//		streamingLoadTask = new Task( StreamingLoadFunction );
		//		streamingLoadTask.Start();
		//	}
		//	else
		//	{
		//		if( !LoadHeightmapBuffer( false ) )
		//			return false;

		//		if( !LoadHoleBuffer( false ) )
		//		{
		//			DestroyHeightmapBuffer();
		//			return false;
		//		}

		//		if( !LoadMasks( false ) )
		//		{
		//			DestroyHeightmapBuffer();
		//			DestroyHoleBuffer();
		//			return false;
		//		}

		//		if( AmbientOcclusion.Enabled )
		//		{
		//			if( !LoadAOData( false ) )
		//			{
		//				DestroyHeightmapBuffer();
		//				DestroyHoleBuffer();
		//				return false;
		//			}
		//		}

		//		loadingStatus = LoadingStatuses.Loaded;
		//	}

		//	return true;
		//}

		//public void UnloadAllData()
		//{
		//	if( loadingStatus != LoadingStatuses.Unloaded )
		//	{
		//		if( loadingStatus == LoadingStatuses.Loading )
		//			Log.Fatal( "HeightmapTerrain: UnloadAllData: loadingStatus == LoadingStatuses.Loading." );

		//		Enabled = false;

		//		if( layers != null )
		//		{
		//			foreach( Layer layer in layers )
		//				layer.mask = null;
		//		}
		//		heightmapBuffer = null;
		//		holeBuffer = null;
		//		staticLightingItems = null;
		//		aoData = null;

		//		loadingStatus = LoadingStatuses.Unloaded;

		//		//UnloadBaseMapsWhenTerrainUnloaded 
		//		foreach( Layer layer in Layers )
		//		{
		//			if( layer.UnloadBaseMapsWhenTerrainUnloaded )
		//			{
		//				for( int n = 0; n < 3; n++ )
		//				{
		//					string textureName = "";
		//					switch( n )
		//					{
		//					case 0: textureName = layer.BaseMapGetFullPath(); break;
		//					case 1: textureName = layer.BaseNormalMapGetFullPath(); break;
		//					case 2: textureName = layer.BaseSpecularMapGetFullPath(); break;
		//					}
		//					if( !string.IsNullOrEmpty( textureName ) )
		//						TextureManager.Instance.Unload( textureName );
		//				}
		//			}
		//		}
		//	}
		//}

		//[Browsable( false )]
		//public LoadingStatuses LoadingStatus
		//{
		//	get { return loadingStatus; }
		//}

		//void StreamingLoadFunction()
		//{
		//	if( !LoadHeightmapBuffer( true ) )
		//	{
		//		streamingLoadError = true;
		//		return;
		//	}

		//	if( !LoadHoleBuffer( true ) )
		//	{
		//		DestroyHeightmapBuffer();
		//		streamingLoadError = true;
		//		return;
		//	}

		//	if( !LoadMasks( true ) )
		//	{
		//		DestroyHeightmapBuffer();
		//		DestroyHoleBuffer();
		//		streamingLoadError = true;
		//		return;
		//	}

		//	if( AmbientOcclusion.Enabled )
		//	{
		//		if( !LoadAOData( true ) )
		//		{
		//			DestroyHeightmapBuffer();
		//			DestroyHoleBuffer();
		//			streamingLoadError = true;
		//			return;
		//		}
		//	}

		//	//wait for load textures of the layers
		//	bool existNotFinished;
		//	do
		//	{
		//		existNotFinished = false;
		//		List<ResourceBackgroundQueue.TextureBackgroundProcessTicket> list =
		//			new List<ResourceBackgroundQueue.TextureBackgroundProcessTicket>( streamingLoadTextureTasks );
		//		foreach( ResourceBackgroundQueue.TextureBackgroundProcessTicket ticket in list )
		//		{
		//			if( ticket.Status == ResourceBackgroundQueue.BackgroundProcessTicket.Statuses.Loading )
		//				existNotFinished = true;
		//		}

		//		if( existNotFinished )
		//			Thread.Sleep( 0 );
		//	} while( existNotFinished );
		//}

		//void StreamingLoadUpdate()
		//{
		//	if( streamingLoadTask.IsCompleted || streamingLoadTask.IsCanceled )
		//	{
		//		if( streamingLoadTask.IsCanceled )
		//			streamingLoadError = true;

		//		if( streamingLoadError )
		//		{
		//			Log.Warning( "HeightmapTerrain: StreamingLoadUpdate: streamingLoadError == true." );

		//			UnloadAllData();
		//			loadingStatus = LoadingStatuses.Unloaded;
		//		}
		//		else
		//		{
		//			loadingStatus = LoadingStatuses.Loaded;
		//		}

		//		streamingLoadTask.Dispose();
		//		streamingLoadTask = null;
		//		streamingLoadError = false;
		//		streamingLoadTextureTasks.Clear();
		//	}
		//}

		//[Browsable( false )]
		//public bool StreamingEnable
		//{
		//	get { return streamingEnable; }
		//}

		//public void BeginStreamingEnable()
		//{
		//	if( !IsPostCreated )
		//		Log.Fatal( "HeightmapTerrain: BeginStreamingEnable: IsPostCreated == False." );
		//	if( enabled )
		//		Log.Fatal( "HeightmapTerrain: BeginStreamingEnable: Enabled == True." );
		//	if( streamingEnable )
		//		Log.Fatal( "HeightmapTerrain: BeginStreamingEnable: StreamingEnable == True." );

		//	streamingEnable = true;
		//	//Log.Info( "streamingEnable = true " + heightmapTerrainManagerIndex.ToString() );

		//	//float t = EngineApp.Instance.Time;

		//	CalculateCachedGeneralData();
		//	GenerateTiles();

		//	CreateNormalsHeightTextures();
		//	CreateMasksTextures( false );
		//	CreateHoleTextures();
		//	CreateAOTextures();

		//	//Log.Info( "Begin " + ( EngineApp.Instance.Time - t ).ToString() );

		//	foreach( KeyValuePair<TextureData, IntPtr> pair in streamingEnableDataForTextures )
		//		NativeUtils.Free( pair.Value );
		//	streamingEnableDataForTextures = new Dictionary<TextureData, IntPtr>();

		//	//float t3 = EngineApp.Instance.Time;

		//	//UpdateDataForRenderingForChangedLayerMasks( new RectangleI( 0, 0, masksSize, masksSize ), true );

		//	//Log.Info( "CC3 " + ( EngineApp.Instance.Time - t3 ).ToString() );

		//	streamingEnableError = false;
		//	streamingEnableMustStop = false;
		//	streamingEnableFinishCounter = 0;
		//	streamingEnableTask = new Task( StreamingEnableFunction );
		//	streamingEnableTask.Start();
		//}

		//void StreamingEnableFunction()
		//{
		//	//call UpdateLayersList()
		//	UpdateDataForRenderingForChangedLayerMasks( new RectangleI( 0, 0, masksSize, masksSize ), true );
		//	if( streamingEnableMustStop )
		//		return;

		//	UpdateMasksTextures( Vector2I.Zero, new Vector2I( masksSize, masksSize ), true );
		//	if( streamingEnableMustStop )
		//		return;

		//	UpdateNormalsHeightTextures( new RectangleI( Vector2I.Zero, new Vector2I( heightmapSize - 1, heightmapSize - 1 ) ), true );
		//	if( streamingEnableMustStop )
		//		return;

		//	UpdateHoleTextures( new RectangleI( Vector2I.Zero, new Vector2I( heightmapSize - 1, heightmapSize - 1 ) ), true );
		//	if( streamingEnableMustStop )
		//		return;

		//	UpdateAOTextures( true );
		//}

		//void StreamingEnableUpdate()
		//{
		//	if( streamingEnableTask.IsCompleted || streamingEnableTask.IsCanceled )
		//	{
		//		if( streamingEnableTask.IsCanceled )
		//			streamingEnableError = true;

		//		if( streamingEnableError )
		//		{
		//			Log.Warning( "HeightmapTerrain: StreamingEnableUpdate: streamingEnableError == true." );

		//			DestroyAllRecreatableDataForRendering();

		//			//Log.Info( "StreamingEnable = false " + heightmapTerrainManagerIndex.ToString() );
		//		}
		//		else
		//		{
		//			//float t = EngineApp.Instance.Time;

		//			List<Tile> tiles = GetTiles();

		//			//1. each tile
		//			//2. each texture
		//			int totalFinishCounts = tiles.Count + streamingEnableDataForTextures.Count;
		//			int finishCounter = 0;

		//			//1. each tile
		//			for( int nTile = 0; nTile < tiles.Count; nTile++ )
		//			{
		//				if( streamingEnableFinishCounter == finishCounter )
		//				{
		//					Tile tile = tiles[ nTile ];

		//					//calculate staticMeshObject.Bounds for tiles
		//					GenerateTile_FinishStreamingUpdate( tile );

		//					//UpdateDataForRenderingForChangedLayerMasks_FinishStreamingUpdate
		//					{
		//						if( tile.staticMeshObject != null )
		//						//if( tile.UpdateLayersList() && tile.staticMeshObject != null )
		//						{
		//							//layers list is changed

		//							//update material
		//							_HeightmapTerrainMaterial material = GenerateMaterialForTile( tile );//, false );
		//							tile.staticMeshObject.Material = ( material != null ) ? material.BaseMaterial : null;
		//							tile.UpdateMaterialTechniquesInfo();
		//						}

		//						//update vertices for fixed pipeline mode
		//						if( IsFixedPipelineFallback() )
		//							UpdateTileVertexDatasForNonSharedVertexBuffersMode( tile );//, false );
		//					}
		//				}
		//				finishCounter++;
		//			}

		//			//Log.Info( "Finish 1 " + ( EngineApp.Instance.Time - t ).ToString() );
		//			//t = EngineApp.Instance.Time;

		//			//2. each texture
		//			//copy data to textures
		//			//finish update all recreatable data for rendering (UpdateAllRecreatableDataForRendering)
		//			foreach( KeyValuePair<TextureData, IntPtr> pair in streamingEnableDataForTextures )
		//			{
		//				if( streamingEnableFinishCounter == finishCounter )
		//				{
		//					TextureData textureData = pair.Key;
		//					IntPtr data = pair.Value;

		//					HardwarePixelBuffer buffer = textureData.texture.GetBuffer();
		//					IntPtr pointer = buffer.Lock( HardwareBuffer.LockOptions.Normal );
		//					//IntPtr pointer = buffer.Lock( HardwareBuffer.LockOptions.Discard );
		//					NativeUtils.CopyMemory( pointer, data, textureData.bufferSizeInBytes );
		//					buffer.Unlock();
		//				}
		//				finishCounter++;
		//			}

		//			if( finishCounter != totalFinishCounts )
		//				Log.Fatal( "HeightmapTerrain: StreamingEnableUpdate: finishCounter != totalFinishCounts." );
		//			streamingEnableFinishCounter++;
		//			if( streamingEnableFinishCounter < totalFinishCounts )
		//				return;

		//			enabled = true;

		//			//test not filled masks textures. all must filled
		//			if( masksTextures != null )
		//			{
		//				for( int y = 0; y < masksTexturesCount; y++ )
		//				{
		//					for( int x = 0; x < masksTexturesCount; x++ )
		//					{
		//						MaskTextureItem textureItem = masksTextures[ x, y ];

		//						if( textureItem != null )
		//						{
		//							if( textureItem.shaderTexture != null )
		//							{
		//								if( !streamingEnableDataForTextures.ContainsKey( textureItem.shaderTexture ) )
		//									Log.Fatal( "HeightmapTerrain: StreamingEnableUpdate: !streamingEnableDataForTextures.ContainsKey( textureItem.shaderTexture )." );
		//							}
		//							if( textureItem.ffpTextures != null )
		//							{
		//								for( int n = 0; n < 5; n++ )
		//								{
		//									TextureData textureData = textureItem.ffpTextures[ n ];
		//									if( textureData != null && !streamingEnableDataForTextures.ContainsKey( textureData ) )
		//										Log.Fatal( "HeightmapTerrain: StreamingEnableUpdate: !streamingEnableDataForTextures.ContainsKey( textureData )." );
		//								}
		//							}
		//						}
		//					}
		//				}
		//			}

		//			CreateCollisionBodies();

		//			//Log.Info( "Finish 2 " + ( EngineApp.Instance.Time - t ).ToString() );
		//			//Log.Info( "Finish " + ( EngineApp.Instance.Time - t ).ToString() );
		//			//Log.Info( "enabled = true " + heightmapTerrainManagerIndex.ToString() );
		//		}

		//		streamingEnable = false;
		//		streamingEnableTask.Dispose();
		//		streamingEnableTask = null;
		//		streamingEnableError = false;
		//		streamingEnableMustStop = false;
		//		streamingEnableFinishCounter = 0;

		//		foreach( KeyValuePair<TextureData, IntPtr> pair in streamingEnableDataForTextures )
		//			NativeUtils.Free( pair.Value );
		//		streamingEnableDataForTextures = new Dictionary<TextureData, IntPtr>();

		//		lastFinishedStreamingEnableTime = EngineApp.Instance.Time;
		//	}
		//}

		//[Browsable( false )]
		//public float LastFinishedStreamingEnableTime
		//{
		//	get { return lastFinishedStreamingEnableTime; }
		//}

		//[Browsable( false )]
		//public HeightmapTerrainManager HeightmapTerrainManager
		//{
		//	get { return heightmapTerrainManager; }
		//}

		//[Browsable( false )]
		//public Vector2I HeightmapTerrainManagerIndex
		//{
		//	get { return heightmapTerrainManagerIndex; }
		//}

		//[Browsable( false )]
		//public bool Modified
		//{
		//	get { return modified; }
		//}

		//void SetModified( bool value )
		//{
		//	//if( heightmapTerrainManagerIndex == Vector2I.Zero )
		//	//{
		//	//   if( value )
		//	//      Log.Info( "Dfdfg" );
		//	//}

		//	modified = value;
		//}

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

		//public void SetNeedUpdateHeightMinMax()
		//{
		//	heightMinMaxNeedUpdate = true;
		//}

		void RecreateInternalData( bool updateHoles = false )
		{
			CreateTiles( updateHoles );
			CreateCollisionBodies();
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
			set { heightmapBuffer = value; }
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
				if( tile.Vertices != null && tile.Bounds != null && tile.Bounds.CalculatedBoundingBox.Intersects( ray ) )
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

		public static bool GetTerrainByRay( Component_Scene scene, Ray ray, out Component_Terrain terrain, out Vector3 position )
		{
			var item = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, ray );
			scene.GetObjectsInSpace( item );
			if( item.Result != null )
			{
				foreach( var resultItem in item.Result )
				{
					var tile = resultItem.Object.AnyData as Tile;
					if( tile != null )
					{
						var terrain2 = tile.owner;

						if( tile.Vertices != null && tile.Bounds != null && tile.Bounds.CalculatedBoundingBox.Intersects( ray ) )
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

			CheckForUpdatePaintLayers();
		}

		Component_RenderingPipeline.RenderSceneData.LayerItem[] CalculateLayers()
		{
			var items = new List<Component_RenderingPipeline.RenderSceneData.LayerItem>();

			foreach( var layer in GetComponents<Component_PaintLayer>() )
			{
				if( layer.Enabled )
				{
					//if( layer.MaskImage.Value != null || layer.Mask.Value != null )
					var image = layer.GetImage( out var uniqueMaskDataCounter );
					if( image != null )
					{
						var item = new Component_RenderingPipeline.RenderSceneData.LayerItem();
						item.Material = layer.Material;
						item.Mask = image;
						item.UniqueMaskDataCounter = uniqueMaskDataCounter;
						item.Color = layer.Color;
						items.Add( item );
					}
				}
			}

			if( items.Count != 0 )
				return items.ToArray();
			else
				return null;
		}

		void UpdateCurrentLayers()
		{
			currentLayers = CalculateLayers();
		}

		void CheckForUpdatePaintLayers()
		{
			var newLayers = CalculateLayers();

			bool update = false;
			if( newLayers != null && currentLayers == null )
				update = true;
			else if( newLayers == null && currentLayers != null )
				update = true;
			else if( newLayers != null && currentLayers != null && !newLayers.SequenceEqual( currentLayers ) )
				update = true;

			if( update )
			{
				currentLayers = newLayers;

				foreach( var tile in GetTiles() )
					tile.UpdateLayers();
			}
		}

		public List<Component_MeshInSpace> GetHoleObjects()
		{
			var holes = new List<Component_MeshInSpace>( Holes.Count );
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

			var newTerrain = (Component_Terrain)newObject;
			if( heightmapBuffer != null )
				newTerrain.heightmapBuffer = (float[,])heightmapBuffer.Clone();
		}

	}
}
