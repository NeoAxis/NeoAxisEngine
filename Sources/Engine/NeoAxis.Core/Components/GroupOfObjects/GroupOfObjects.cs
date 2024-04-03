// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;
using Internal.SharpBgfx;
using System.Dynamic;

namespace NeoAxis
{
	/// <summary>
	/// An object in a scene designed to store a large number of similar objects.
	/// </summary>
#if !DEPLOY
	[SettingsCell( "NeoAxis.Editor.GroupOfObjectsSettingsCell" )]
#endif
	public partial class GroupOfObjects : Component, IVisibleInHierarchy
	{
		const int formatVersion = 2;

		Dictionary<Vector3I, Sector> sectors = new Dictionary<Vector3I, Sector>();

		//!!!!про когда обновлять
		//!!!!удалять
		//Dictionary<Vector4I, Sector> sectorLevels = new Dictionary<Vector4I, Sector>();

		//Count = capacity of Object or can be null.
		OpenList<bool> removedObjects;
		Stack<int> freeObjects = new Stack<int>();

		ElementTypesCache elementTypesCache;
		bool elementTypesCacheNeedUpdate = true;

		bool needUpdate;

		long uniqueIdentifierCounter = 1;

		bool updating;
		ESet<Sector> updatingSectorsToUpdate;

		bool needRemoveEmptyElements;

		//!!!!cuncurrent?
		Queue<SubGroupAction> subGroupActionQueue = new Queue<SubGroupAction>();

		bool needUpdateStaticShadows;

		/////////////////////////////////////////

		/// <summary>
		/// Whether the object is visible in the scene.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> Visible
		{
			get { if( _visible.BeginGet() ) Visible = _visible.Get( this ); return _visible.value; }
			set { if( _visible.BeginSet( this, ref value ) ) { try { VisibleChanged?.Invoke( this ); } finally { _visible.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Visible"/> property value changes.</summary>
		public event Action<GroupOfObjects> VisibleChanged;
		ReferenceField<bool> _visible = true;

		[Browsable( false )]
		public bool VisibleInHierarchy
		{
			get
			{
				if( !Visible )
					return false;

				var p = Parent as IVisibleInHierarchy;
				if( p != null )
					return p.VisibleInHierarchy;
				else
					return true;
			}
		}

		/// <summary>
		/// The list of base objects. When creating objects, the editor uses base objects as a background on which objects are placed.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		public ReferenceList<Component> BaseObjects
		{
			get { return _baseObjects; }
		}
		public delegate void BaseObjectsChangedDelegate( GroupOfObjects sender );
		public event BaseObjectsChangedDelegate BaseObjectsChanged;
		ReferenceList<Component> _baseObjects;

		//!!!!может он сам будет определять размеры секторов типа как octree
		//!!!!default value
		/// <summary>
		/// The size of the sector in the scene. The sector size allows to optimize the culling and rendering of objects.
		/// </summary>
		[DefaultValue( "200 200 10000" )]//[DefaultValue( "100 100 10000" )]
		public Reference<Vector3> SectorSize
		{
			get { if( _sectorSize.BeginGet() ) SectorSize = _sectorSize.Get( this ); return _sectorSize.value; }
			set
			{
				var v = value.Value;
				if( v.X < 1.0 || v.Y < 1.0 || v.Z < 1.0 )
				{
					if( v.X < 1.0 ) v.X = 1.0;
					if( v.Y < 1.0 ) v.Y = 1.0;
					if( v.Z < 1.0 ) v.Z = 1.0;
					value = new Reference<Vector3>( v, value.GetByReference );
				}
				if( _sectorSize.BeginSet( this, ref value ) )
				{
					try
					{
						SectorSizeChanged?.Invoke( this );
						CreateSectors();
					}
					finally { _sectorSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SectorSize"/> property value changes.</summary>
		public event Action<GroupOfObjects> SectorSizeChanged;
		ReferenceField<Vector3> _sectorSize = new Vector3( 200, 200, 10000 );// = new Vector3( 100, 100, 10000 );

		//!!!!support several levels?
		/// <summary>
		/// The maximal amount of sub sectors along the axis in the sector.
		/// </summary>
		[DefaultValue( 3 )]
		[Range( 1, 8 )]
		public Reference<int> SectorSubdivide
		{
			get { if( _sectorSubdivide.BeginGet() ) SectorSubdivide = _sectorSubdivide.Get( this ); return _sectorSubdivide.value; }
			set
			{
				var v = value.Value;
				if( v < 1 || v > 8 )
				{
					v = MathEx.Clamp( v, 1, 8 );
					value = new Reference<int>( v, value.GetByReference );
				}

				if( _sectorSubdivide.BeginSet( this, ref value ) )
				{
					try
					{
						SectorSubdivideChanged?.Invoke( this );
						CreateSectors();
					}
					finally { _sectorSubdivide.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SectorSubdivide"/> property value changes.</summary>
		public event Action<GroupOfObjects> SectorSubdivideChanged;
		ReferenceField<int> _sectorSubdivide = 3;

		////!!!!default
		///// <summary>
		///// The amount of levels of sectors. The levels are like LODs but for instances.
		///// </summary>
		//[DefaultValue( 3 )]
		//[Range( 1, 6 )]
		//public Reference<int> SectorLevels
		//{
		//	get { if( _sectorLevels.BeginGet() ) SectorLevels = _sectorLevels.Get( this ); return _sectorLevels.value; }
		//	set { if( _sectorLevels.BeginSet( this, ref value ) ) { try { SectorLevelsChanged?.Invoke( this ); CreateSectors(); } finally { _sectorLevels.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SectorLevels"/> property value changes.</summary>
		//public event Action<GroupOfObjects> SectorLevelsChanged;
		//ReferenceField<int> _sectorLevels = 3;

		////!!!!default value
		///// <summary>
		///// The maximal amount of objects in one group/batch.
		///// </summary>
		//[DefaultValue( 100000 )]// 2000 )]
		//public Reference<int> MaxObjectsInGroup
		//{
		//	get { if( _maxObjectsInGroup.BeginGet() ) MaxObjectsInGroup = _maxObjectsInGroup.Get( this ); return _maxObjectsInGroup.value; }
		//	set { if( _maxObjectsInGroup.BeginSet( this, ref value ) ) { try { MaxObjectsInGroupChanged?.Invoke( this ); CreateSectors(); } finally { _maxObjectsInGroup.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MaxObjectsInGroup"/> property value changes.</summary>
		//public event Action<GroupOfObjects> MaxObjectsInGroupChanged;
		//ReferenceField<int> _maxObjectsInGroup = 100000;//2000;

		/// <summary>
		/// Whether to enable serialization of objects data.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> ObjectsSerialize
		{
			get { if( _objectsSerialize.BeginGet() ) ObjectsSerialize = _objectsSerialize.Get( this ); return _objectsSerialize.value; }
			set { if( _objectsSerialize.BeginSet( this, ref value ) ) { try { ObjectsSerializeChanged?.Invoke( this ); } finally { _objectsSerialize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ObjectsSerialize"/> property value changes.</summary>
		public event Action<GroupOfObjects> ObjectsSerializeChanged;
		ReferenceField<bool> _objectsSerialize = true;

		/// <summary>
		/// Whether to enable synchronization of objects data between server and clients.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> ObjectsNetworkMode
		{
			get { if( _objectsNetworkMode.BeginGet() ) ObjectsNetworkMode = _objectsNetworkMode.Get( this ); return _objectsNetworkMode.value; }
			set { if( _objectsNetworkMode.BeginSet( this, ref value ) ) { try { ObjectsNetworkModeChanged?.Invoke( this ); } finally { _objectsNetworkMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ObjectsNetworkMode"/> property value changes.</summary>
		public event Action<GroupOfObjects> ObjectsNetworkModeChanged;
		ReferenceField<bool> _objectsNetworkMode = true;

		/// <summary>
		/// Whether to visialize bounds of the groups.
		/// </summary>
		//[Category( "Debug" )]
		[DefaultValue( false )]
		public Reference<bool> DrawGroupBounds
		{
			get { if( _drawGroupBounds.BeginGet() ) DrawGroupBounds = _drawGroupBounds.Get( this ); return _drawGroupBounds.value; }
			set { if( _drawGroupBounds.BeginSet( this, ref value ) ) { try { DrawGroupBoundsChanged?.Invoke( this ); } finally { _drawGroupBounds.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DrawGroupBounds"/> property value changes.</summary>
		public event Action<GroupOfObjects> DrawGroupBoundsChanged;
		ReferenceField<bool> _drawGroupBounds = false;

		[Browsable( false )]
		public bool EditorAllowUsePaintBrush { get; set; } = true;

		Object[] Objects { get; set; }

		//struct TempSectorGetSceneData
		//{
		//	public Sector sector;
		//	public GetRenderSceneDataMode mode;
		//	public Scene.GetObjectsInSpaceItem modeGetObjectsItem;
		//}
		//OpenList<TempSectorGetSceneData> _tempSectorsGetSceneData = new OpenList<TempSectorGetSceneData>();

		/////////////////////////////////////////

		/// <summary>
		/// Represents an object for <see cref="GroupOfObjects"/>.
		/// </summary>
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public struct Object
		{
			public long UniqueIdentifier;

			public ushort Element;
			public byte VariationGroup;
			public byte VariationElement;
			public FlagsEnum Flags;

			public Vector3 Position;
			public QuaternionF Rotation;
			public Vector3F Scale;

			public Vector4F AnyData;

			public ColorValue Color;
			public Vector4F Special1;
			public Vector4F Special2;

			public uint CullingByCameraDirectionData;

			////////////

			[Flags]
			public enum FlagsEnum// : byte
			{
				Enabled = 1,
				Visible = 2,
			}

			////////////

			public Object( ushort element, byte variationGroup, byte variationElement, FlagsEnum flags, Vector3 position, QuaternionF rotation, Vector3F scale, Vector4F anyData, ColorValue color, Vector4F special1, Vector4F special2, uint cullingByCameraDirectionData )
			{
				UniqueIdentifier = 0;
				Element = element;
				VariationGroup = variationGroup;
				VariationElement = variationElement;
				Flags = flags;
				Position = position;
				Rotation = rotation;
				Scale = scale;
				AnyData = anyData;
				Color = color;
				Special1 = special1;
				Special2 = special2;
				CullingByCameraDirectionData = cullingByCameraDirectionData;
			}

			public Object( ushort element, byte variationGroup, byte variationElement, FlagsEnum flags, ref Vector3 position, ref QuaternionF rotation, ref Vector3F scale, ref Vector4F anyData, ref ColorValue color, ref Vector4F special1, ref Vector4F special2, uint cullingByCameraDirectionData )
			{
				UniqueIdentifier = 0;
				Element = element;
				VariationGroup = variationGroup;
				VariationElement = variationElement;
				Flags = flags;
				Position = position;
				Rotation = rotation;
				Scale = scale;
				AnyData = anyData;
				Color = color;
				Special1 = special1;
				Special2 = special2;
				CullingByCameraDirectionData = cullingByCameraDirectionData;
			}

			//public Object( ushort element, byte variationGroup, byte variationElement, FlagsEnum flags, ref Vector3 position, ref QuaternionF rotation, ref Vector3F scale )
			//{
			//	UniqueIdentifier = 0;
			//	Element = element;
			//	VariationGroup = variationGroup;
			//	VariationElement = variationElement;
			//	Flags = flags;
			//	Position = position;
			//	Rotation = rotation;
			//	Scale = scale;
			//	AnyData = Vector4F.Zero;
			//	Color = ColorValue.Zero;
			//	Special1 = Vector4F.Zero;
			//	Special2 = Vector4F.Zero;
			//}
		}

		/////////////////////////////////////////

		internal class Sector
		{
			public GroupOfObjects owner;
			public Vector3I Index;

			//!!!!state. loading, loaded.

			public ESet<int> Objects = new ESet<int>();

			public bool ObjectsBoundsCalculated;
			public Bounds ObjectsBounds;
			//public SpaceBounds ObjectsBounds;
			//public Bounds ObjectsBoundingBox;
			public bool objectsBoundsNeedUpdateAfterRemove;

			public ObjectInSpace ObjectInSpace;
			public List<Scene.PhysicsWorldClass.Body> CollisionBodies = new List<Scene.PhysicsWorldClass.Body>();

			//rendering data
			public bool RenderingDataNeedUpdate = true;
			//still sorted? sorted by group.VisibilityDistance from far to nearest
			public List<RenderGroup> RenderGroupsOne = new List<RenderGroup>();
			public List<RenderGroup> RenderGroupsSubdivided = new List<RenderGroup>();
			//public List<Group> Groups = new List<Group>();

			//!!!!maybe per group
			//can be Cleared
			public Bounds StaticShadowsAffectedBounds = Bounds.Cleared;
			public int StaticShadowsAffectedBoundsHash;

			////////////

			public class RenderGroup
			{
				public Sector Owner;
				public Vector3I SubdivideIndex;

				//for transparent materials
				public bool NoBatchingGroup;
				//batching
				public int/*ushort*/ Element;
				public int/*byte*/ VariationGroup;
				public int/*byte*/ VariationElement;

				//!!!!не многовато данных?
				public struct ObjectItem
				{
					public int ObjectIndex;

					public Matrix3F MeshMatrix3;
					//public Vector3 MeshPosition;
					//public Matrix4 MeshMatrix;

					public Bounds BoundingBox;
					public Sphere BoundingSphere;

					public uint ColorForInstancingData1;
					public byte ColorForInstancingData2;
				}
				public OpenList<ObjectItem> Objects = new OpenList<ObjectItem>();

				public Bounds BoundingBox;
				public Sphere BoundingSphere;
				//public SpaceBounds Bounds;

				public double ObjectsMaxScale;
				//public float VisibilityDistance;
				//public float VisibilityDistanceSquared;

				public GpuVertexBuffer BatchingInstanceBufferMesh;
				public GpuVertexBuffer BatchingInstanceBufferBillboard;
				public Vector3 BatchingInstancePositionOffset;

				////for RenderingDataUpdate() method
				//public int NextSplitAxis;
			}

			////////////

			public Sector( GroupOfObjects owner )
			{
				this.owner = owner;
			}

			public override string ToString()
			{
				return Index.ToString();
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public bool IsEmpty()
			{
				return Objects.Count == 0;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public void AddToObjectsBounds( ref Object obj )
			{
				//!!!!не только меши
				owner.GetMesh( obj.Element, obj.VariationGroup, obj.VariationElement, out _, out var mesh, out _, out _, out _, out _, out _, out _, out _ );

				//!!!!enabled

				ref var meshSpaceBounds = ref mesh.Result.SpaceBounds.boundingBox;
				//mesh.Result.SpaceBounds.GetCalculatedBoundingBox( out var meshSpaceBounds );

				unsafe
				{
					Vector3* points = stackalloc Vector3[ 8 ];
					meshSpaceBounds.ToPoints( points );

					//var transform = new Transform( obj.Position, obj.Rotation, obj.Scale );

					for( int n = 0; n < 8; n++ )
					{
						var point = points[ n ];

						MathEx.TransformMultiply( ref obj.Position, ref obj.Rotation, ref obj.Scale, ref point, out var pointTransformed );

						//var p2 = transform * point;
						//Log.Info( p2.ToString() );

						if( ObjectsBoundsCalculated )
							ObjectsBounds.Add( ref pointTransformed );
						else
						{
							ObjectsBounds = new Bounds( pointTransformed );
							ObjectsBoundsCalculated = true;
						}
					}
				}


				//var transform = new Transform( obj.Position, obj.Rotation, obj.Scale );
				//var meshSpaceBounds = mesh.Result.SpaceBounds;

				////!!!!можно считать оптимальнее?
				//var b = SpaceBounds.Multiply( transform, meshSpaceBounds );

				//ObjectsBounds = SpaceBounds.Merge( ObjectsBounds, b );
				//if( ObjectsBounds != null )
				//	ObjectsBounds.GetCalculatedBoundingBox( out ObjectsBoundingBox );
				//else
				//	ObjectsBoundingBox = Bounds.Cleared;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public void UpdateNotEmpty()
			{
				//update ObjectsBounds
				if( objectsBoundsNeedUpdateAfterRemove )
				{
					objectsBoundsNeedUpdateAfterRemove = false;
					ObjectsBoundsCalculated = false;
					ObjectsBounds = Bounds.Zero;
					//ObjectsBounds = null;
					foreach( var index in Objects )
						AddToObjectsBounds( ref owner.Objects[ index ] );
				}

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

					ObjectInSpace = sectors.CreateComponent<ObjectInSpace>( enabled: false );
					ObjectInSpace.AnyData = this;
					ObjectInSpace.Name = Index.ToString();
					ObjectInSpace.SaveSupport = false;
					ObjectInSpace.CloneSupport = false;
					ObjectInSpace.CanBeSelected = false;
					ObjectInSpace.SpaceBoundsUpdateEvent += ObjectInSpace_SpaceBoundsUpdateEvent;
					ObjectInSpace.Transform = new Transform( ObjectsBounds.GetCenter(), Quaternion.Identity );
					//ObjectInSpace.Transform = new Transform( ObjectsBounds.CalculatedBoundingBox.GetCenter(), Quaternion.Identity );
					ObjectInSpace.GetRenderSceneData += ObjectInSpace_GetRenderSceneData;
					ObjectInSpace.Enabled = true;
				}
				else
				{
					ObjectInSpace.Transform = new Transform( ObjectsBounds.GetCenter(), Quaternion.Identity );
					//ObjectInSpace.Transform = new Transform( ObjectsBounds.CalculatedBoundingBox.GetCenter(), Quaternion.Identity );
					ObjectInSpace.SpaceBoundsUpdate();
				}

				//update collision bodies
				{

					//!!!!slowly. maybe faster to do partial update

					CollisionBodiesDestroy();

					var sectorCenter = ObjectsBounds.GetCenter();
					var scene = owner.FindParent<Scene>();

					var collisionDefinitionByMesh = new Dictionary<Mesh, RigidBody>( 64 );

					foreach( var objectIndex in Objects )
					{
						ref var obj = ref owner.Objects[ objectIndex ];

						owner.GetMesh( obj.Element, obj.VariationGroup, obj.VariationElement, out var enabled, out var mesh, out _, out _, out _, out _, out _, out _, out var objCollision );

						if( enabled && mesh != null && objCollision )
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
										CollisionBodies.Add( rigidBodyItem );
									}
								}
							}
							//!!!!где еще такое?
							//else
							//{
							////maybe is better demand to have prepared collision definition. better for optimization
							////or anyway make collision body from mesh
							//}
						}
					}
				}

				RenderingDataNeedUpdate = true;
			}

			public void DestroyData()
			{
				CollisionBodiesDestroy();

				if( ObjectInSpace != null )
				{
					if( !StaticShadowsAffectedBounds.IsCleared() )
					{
						ResetLightStaticShadowsCache( ref StaticShadowsAffectedBounds );
						StaticShadowsAffectedBounds = Bounds.Cleared;
						StaticShadowsAffectedBoundsHash = 0;
					}

					ObjectInSpace.SpaceBoundsUpdateEvent -= ObjectInSpace_SpaceBoundsUpdateEvent;
					ObjectInSpace.GetRenderSceneData -= ObjectInSpace_GetRenderSceneData;
					ObjectInSpace.RemoveFromParent( false );
					ObjectInSpace = null;
				}

				RenderingDataDestroy();
			}

			[MethodImpl( (MethodImplOptions)512 )]
			private void ObjectInSpace_SpaceBoundsUpdateEvent( ObjectInSpace obj, ref SpaceBounds newBounds )
			{
				var sector = obj.AnyData as Sector;
				if( sector != null && sector.ObjectsBoundsCalculated )
					newBounds = new SpaceBounds( sector.ObjectsBounds );

				//if( sector != null && sector.ObjectsBounds != null )
				//	newBounds = sector.ObjectsBounds;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			private void ObjectInSpace_GetRenderSceneData( ObjectInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
			{
				if( owner.VisibleInHierarchy )
				{
					var sector = sender.AnyData as Sector;
					if( sector != null )
					{
						//if( owner.SectorLevels > 1 && !EngineApp._DebugCapsLock )
						//{
						//	var item = new TempSectorGetSceneData();
						//	item.sector = sector;
						//	item.mode = mode;
						//	item.modeGetObjectsItem = modeGetObjectsItem;
						//	owner._tempSectorsGetSceneData.Add( ref item );

						//	//_tempSectors.Add( sector );
						//}
						//else

						owner.SectorGetRenderSceneData( context, mode, modeGetObjectsItem, sector );
					}
				}
			}

			void RenderingDataDestroy()
			{
				foreach( var batch in RenderGroupsOne )
				{
					batch.BatchingInstanceBufferMesh?.Dispose();
					batch.BatchingInstanceBufferMesh = null;
					batch.BatchingInstanceBufferBillboard?.Dispose();
					batch.BatchingInstanceBufferBillboard = null;
				}
				RenderGroupsOne.Clear();

				foreach( var batch in RenderGroupsSubdivided )
				{
					batch.BatchingInstanceBufferMesh?.Dispose();
					batch.BatchingInstanceBufferMesh = null;
					batch.BatchingInstanceBufferBillboard?.Dispose();
					batch.BatchingInstanceBufferBillboard = null;
				}
				RenderGroupsSubdivided.Clear();
			}

			void CollisionBodiesDestroy()
			{
				if( CollisionBodies.Count != 0 )
				{
					foreach( var body in CollisionBodies )
						body.Dispose();
					CollisionBodies.Clear();
				}
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			static RenderGroup GetGroup( List<RenderGroup> groupList, bool noBatchingGroup, ushort elementIndex, byte variationGroup, byte variationElement, Vector3I subdivideIndex )
			{
				for( int nGroup = 0; nGroup < groupList/*Groups*/.Count; nGroup++ )
				{
					var group = groupList/*Groups*/[ nGroup ];

					if( noBatchingGroup == group.NoBatchingGroup )
					{
						if( noBatchingGroup )
							return group;
						else if( group.Element == elementIndex && group.VariationGroup == variationGroup && group.VariationElement == variationElement && group.SubdivideIndex == subdivideIndex )
							return group;
					}
				}
				return null;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public unsafe void CreateBatchingInstanceBufferMesh( RenderGroup batch, float lodValue, /*float visibilityDistance, */bool receiveDecals, float motionBlurFactor )
			{
				var instancePositionOffset = new Vector3( double.MaxValue, double.MaxValue, double.MaxValue );
				for( int n = 0; n < batch.Objects.Count; n++ )
				{
					ref var objectItem = ref batch.Objects.Data[ n ];
					ref var obj = ref owner.Objects[ objectItem.ObjectIndex ];
					instancePositionOffset.X = Math.Min( instancePositionOffset.X, obj.Position.X );
					instancePositionOffset.Y = Math.Min( instancePositionOffset.Y, obj.Position.Y );
					instancePositionOffset.Z = Math.Min( instancePositionOffset.Z, obj.Position.Z );
				}

				var vertices = new byte[ sizeof( RenderingPipeline.RenderSceneData.ObjectInstanceData ) * batch.Objects.Count ];
				fixed( byte* pVertices = vertices )
				{
					for( int n = 0; n < batch.Objects.Count; n++ )
					{
						var instanceData = (RenderingPipeline.RenderSceneData.ObjectInstanceData*)pVertices + n;

						ref var objectItem = ref batch.Objects.Data[ n ];
						ref var obj = ref owner.Objects[ objectItem.ObjectIndex ];

						var matrix = new Matrix4F( objectItem.MeshMatrix3, ( obj.Position - instancePositionOffset ).ToVector3F() );

						var vector3FZero = Vector3F.Zero;
						//matrix.GetTranslation( out var positionPreviousFrame );

						instanceData->Init( ref matrix, ref vector3FZero, ref obj.Color, lodValue, -1/*visibilityDistance*/, receiveDecals, motionBlurFactor, obj.CullingByCameraDirectionData );


						//ref var matrix = ref objectItem.MeshMatrix;
						////var pos = obj.Position.ToVector3F();
						////var matrix = new Matrix4F( ref objectItem.MeshMatrix3, ref pos );

						//matrix.GetTranslation( out var positionPreviousFrame );
						//instanceData->Init( ref matrix, ref positionPreviousFrame, ref obj.Color, lodValue, -1/*visibilityDistance*/, receiveDecals, motionBlurFactor, obj.CullingByCameraDirectionData );
					}
				}

				//!!!!new GpuBufferFlags.ComputeRead
				batch.BatchingInstanceBufferMesh = GpuBufferManager.CreateVertexBuffer( vertices, GpuBufferManager.InstancingVertexDeclaration, GpuBufferFlags.ComputeRead );
				batch.BatchingInstancePositionOffset = instancePositionOffset;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public unsafe void CreateBatchingInstanceBufferBillboard( RenderGroup batch, float lodValue, /*float visibilityDistance, */bool receiveDecals, float motionBlurFactor, RenderingPipeline.RenderSceneData.IMeshData meshData )
			{
				var instancePositionOffset = new Vector3( double.MaxValue, double.MaxValue, double.MaxValue );
				for( int n = 0; n < batch.Objects.Count; n++ )
				{
					ref var objectItem = ref batch.Objects.Data[ n ];
					ref var obj = ref owner.Objects[ objectItem.ObjectIndex ];
					instancePositionOffset.X = Math.Min( instancePositionOffset.X, obj.Position.X );
					instancePositionOffset.Y = Math.Min( instancePositionOffset.Y, obj.Position.Y );
					instancePositionOffset.Z = Math.Min( instancePositionOffset.Z, obj.Position.Z );
				}

				var vertices = new byte[ sizeof( RenderingPipeline.RenderSceneData.ObjectInstanceData ) * batch.Objects.Count ];
				fixed( byte* pVertices = vertices )
				{
					for( int n = 0; n < batch.Objects.Count; n++ )
					{
						var instanceData = (RenderingPipeline.RenderSceneData.ObjectInstanceData*)pVertices + n;

						ref var objectItem = ref batch.Objects.Data[ n ];
						ref var obj = ref owner.Objects[ objectItem.ObjectIndex ];

						ref var position = ref obj.Position;
						ref var rotation = ref obj.Rotation;
						ref var scale = ref obj.Scale;

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

						instanceData->Init( ref matrix, ref vector3FZero, ref obj.Color, lodValue, -1/*visibilityDistance*/, receiveDecals, motionBlurFactor, obj.CullingByCameraDirectionData );
					}
				}

				batch.BatchingInstanceBufferBillboard = GpuBufferManager.CreateVertexBuffer( vertices, GpuBufferManager.InstancingVertexDeclaration, GpuBufferFlags.ComputeRead );
				batch.BatchingInstancePositionOffset = instancePositionOffset;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public unsafe void RenderingDataUpdate()
			{
				RenderingDataDestroy();

				var newStaticShadowsAffectedBounds = Bounds.Cleared;
				var newStaticShadowsAffectedBoundsHash = 0;

				//!!!!создавать вариацию только когда нужна

				for( var nSubdivisionStep = 0; nSubdivisionStep < 2; nSubdivisionStep++ )
				{
					if( nSubdivisionStep == 1 && owner.SectorSubdivide <= 1 )
						continue;

					var groupList = nSubdivisionStep == 1 ? RenderGroupsSubdivided : RenderGroupsOne;

					foreach( var objectIndex in Objects )
					{
						ref var obj = ref owner.Objects[ objectIndex ];

						if( ( obj.Flags & Object.FlagsEnum.Enabled ) == 0 )
							continue;
						if( ( obj.Flags & Object.FlagsEnum.Visible ) == 0 )
							continue;

						owner.GetMesh( obj.Element, obj.VariationGroup, obj.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out _, out _, out _, out _, out var staticShadows, out _ );

						if( !enabled )
							continue;

						bool allowBatching = true;
						if( allowBatching && replaceMaterial?.Result != null && replaceMaterial.Result.Transparent )
							allowBatching = false;
						if( allowBatching && !mesh.SupportsBatching() )
							allowBatching = false;

						var subdivideIndex = Vector3I.Zero;
						if( nSubdivisionStep == 1 )
						{
							owner.GetSectorBounds( ref Index, out var sectorBounds );
							var sectorSize = sectorBounds.GetSize();

							for( int n = 0; n < 3; n++ )
							{
								//!!!!good?
								if( sectorSize[ n ] < 10000 )
								{
									var p = ( obj.Position[ n ] - sectorBounds.Minimum[ n ] ) / sectorSize[ n ];
									p *= (double)owner.SectorSubdivide.Value;

									subdivideIndex[ n ] = MathEx.Clamp( (int)p, 0, owner.SectorSubdivide.Value - 1 );
								}
							}
						}


						var group = GetGroup( groupList, !allowBatching, obj.Element, obj.VariationGroup, obj.VariationElement, subdivideIndex );
						if( group == null )
						{
							group = new RenderGroup();
							group.Owner = this;
							group.SubdivideIndex = subdivideIndex;
							group.NoBatchingGroup = !allowBatching;
							group.Element = obj.Element;
							group.VariationGroup = obj.VariationGroup;
							group.VariationElement = obj.VariationElement;
							groupList/*Groups*/.Add( group );
						}

						var objectItem = new RenderGroup.ObjectItem();
						objectItem.ObjectIndex = objectIndex;

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
						//!!!!можно считать оптимальнее?
						var b = SpaceBounds.Multiply( transform, mesh.Result.SpaceBounds );
						objectItem.BoundingBox = b.boundingBox;
						objectItem.BoundingSphere = b.boundingSphere;

						RenderingPipeline.GetColorForInstancingData( ref obj.Color, out objectItem.ColorForInstancingData1, out objectItem.ColorForInstancingData2 );
						//objectItem.ColorForInstancingData = RenderingPipeline.GetColorForInstancingData( ref obj.Color );

						group.Objects.Add( ref objectItem );

						if( staticShadows )
						{
							newStaticShadowsAffectedBounds.Add( ref objectItem.BoundingBox );

							//!!!!what else
							newStaticShadowsAffectedBoundsHash ^= mesh.GetHashCode() ^ transform.GetHashCode();
						}
					}

					////split groups by max objects count
					////!!!!не нарезать которые не батчатся
					//if( owner.MaxObjectsInGroup > 1 ) 1?
					//{
					//	bool needProcess = true;
					//	do
					//	{
					//		needProcess = false;

					//		var currentGroups = groupList;
					//		groupList = new List<Group>();
					//		if( nSubdivisionStep == 1 )
					//			GroupsSubdivided = groupList;
					//		else
					//			GroupsOne = groupList;
					//		//var currentGroups = Groups;
					//		//Groups = new List<Group>();

					//		foreach( var group in currentGroups )
					//		{
					//			if( group.Objects.Count > owner.MaxObjectsInGroup )
					//			{
					//				var objects = group.Objects.ToArray();

					//				//!!!!CollectionUtility.MergeSortUnmanaged

					//				//!!!!slowly? copy large items. use indices or make ref support
					//				CollectionUtility.MergeSort( objects, delegate ( Group.ObjectItem item1, Group.ObjectItem item2 )
					//				{
					//					ref var obj1 = ref owner.Objects[ item1.ObjectIndex ];
					//					ref var obj2 = ref owner.Objects[ item2.ObjectIndex ];

					//					var pos1 = obj1.Position[ group.NextSplitAxis ];
					//					var pos2 = obj2.Position[ group.NextSplitAxis ];

					//					if( pos1 < pos2 )
					//						return -1;
					//					if( pos1 > pos2 )
					//						return 1;
					//					return 0;

					//				}, true );

					//				int middle = objects.Length / 2;

					//				for( int nGroup = 0; nGroup < 2; nGroup++ )
					//				{
					//					var newGroup = new Group();
					//					newGroup.Owner = this;
					//					newGroup.SubdivideIndex = group.SubdivideIndex;
					//					newGroup.NoBatchingGroup = group.NoBatchingGroup;
					//					newGroup.Element = group.Element;
					//					newGroup.VariationGroup = group.VariationGroup;
					//					newGroup.VariationElement = group.VariationElement;

					//					if( nGroup == 0 )
					//					{
					//						for( int n = 0; n < middle; n++ )
					//							newGroup.Objects.Add( objects[ n ] );
					//					}
					//					else
					//					{
					//						for( int n = middle; n < objects.Length; n++ )
					//							newGroup.Objects.Add( objects[ n ] );
					//					}

					//					newGroup.NextSplitAxis = group.NextSplitAxis + 1;
					//					//quadtree
					//					if( newGroup.NextSplitAxis > 1 )
					//						newGroup.NextSplitAxis = 0;

					//					groupList/*Groups*/.Add( newGroup );
					//				}

					//				needProcess = true;
					//			}
					//			else
					//			{
					//				groupList/*Groups*/.Add( group );
					//			}
					//		}

					//	} while( needProcess );
					//}

					//calculate Bounds, ObjectsMaxScale
					foreach( var group in groupList/*Groups*/ )
					{
						var bounds = Bounds.Cleared;
						//var maxLocalObjectsBounds = Bounds.Cleared;
						//float maxVisibilityDistance = float.MinValue;

						for( int nObject = 0; nObject < group.Objects.Count; nObject++ )
						{
							ref var objectItem = ref group.Objects.Data[ nObject ];
							ref var obj = ref owner.Objects[ objectItem.ObjectIndex ];

							if( ( obj.Flags & Object.FlagsEnum.Enabled ) == 0 )
								continue;
							if( ( obj.Flags & Object.FlagsEnum.Visible ) == 0 )
								continue;

							owner.GetMesh( obj.Element, obj.VariationGroup, obj.VariationElement, out var enabled, out var mesh, out _, out var visibilityDistanceFactor, out _, out _, out _, out _, out _ );

							if( !enabled )
								continue;

							bounds.Add( ref objectItem.BoundingBox );
							group.ObjectsMaxScale = Math.Max( group.ObjectsMaxScale, obj.Scale.MaxComponent() );
							//maxVisibilityDistance = Math.Max( maxVisibilityDistance, (float)visibilityDistance );
						}

						group.BoundingBox = bounds;
						group.BoundingBox.GetBoundingSphere( out group.BoundingSphere );
						//group.VisibilityDistance = maxVisibilityDistance;
						//group.VisibilityDistanceSquared = group.VisibilityDistance * group.VisibilityDistance;
					}
				}

				////sort by VisibilityDistance from far to nearest
				//CollectionUtility.MergeSort( Groups, delegate ( Group group1, Group group2 )
				//{
				//	if( group1.VisibilityDistance > group2.VisibilityDistance )
				//		return -1;
				//	if( group1.VisibilityDistance < group2.VisibilityDistance )
				//		return 1;
				//	return 0;
				//}, true );


				if( StaticShadowsAffectedBoundsHash != newStaticShadowsAffectedBoundsHash )
				{
					var affectBounds = StaticShadowsAffectedBounds;
					affectBounds.Add( ref newStaticShadowsAffectedBounds );
					if( !affectBounds.IsCleared() )
						ResetLightStaticShadowsCache( ref affectBounds );
				}

				StaticShadowsAffectedBounds = newStaticShadowsAffectedBounds;
				StaticShadowsAffectedBoundsHash = newStaticShadowsAffectedBoundsHash;
			}

			internal void ResetLightStaticShadowsCache( ref Bounds bounds )
			{
				if( ObjectInSpace != null )//&& !StaticShadowsAffectedBounds.IsCleared() )
				{
					var scene = owner.ParentRoot as Scene;
					if( scene != null )
					{
						var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, bounds );// StaticShadowsAffectedBounds );
						scene.GetObjectsInSpace( item );

						foreach( var resultItem in item.Result )
						{
							var light = resultItem.Object as Light;
							light?.ResetStaticShadowsCache();
						}

						//var bounds = ObjectInSpace.SpaceBounds.BoundingBox;
						//var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, bounds );
						//scene.GetObjectsInSpace( item );

						//foreach( var resultItem in item.Result )
						//{
						//	var light = resultItem.Object as Light;
						//	light?.ResetStaticShadowsCache();
						//}
					}

					//StaticShadowsAffectedBounds = Bounds.Cleared;
				}
			}

			//public Vector3 GetPosition()
			//{
			//	//!!!!check

			//	var sectorSize = owner.SectorSize.Value;
			//	var v = sectorSize * Index.ToVector3();
			//	v -= sectorSize * 0.5;
			//	return v;

			//	//var sectorSize = SectorSize.Value;
			//	//var v = ( position + sectorSize * 0.5 ) / sectorSize;
			//	//var index = v.ToVector3I();
			//	//if( v.X < 0 ) index.X--;
			//	//if( v.Y < 0 ) index.Y--;
			//	//if( v.Z < 0 ) index.Z--;
			//	//return index;

			//	//return Vector3.Zero;
			//}
		}

		/////////////////////////////////////////

		class ElementTypesCache
		{
			//!!!!структурами?

			public Element[] elements;

			//public struct Variation
			//{
			//	public Mesh mesh;
			//	//!!!!billboard
			//}

			////////////

			public class Element
			{
				public bool enabled;
				public VariationGroup[] variationGroups;
				public bool collision;

				//

				public class Variation
				{
					//mesh
					public Mesh mesh;
					public Material replaceMaterial;
					public float visibilityDistanceFactor;//public float visibilityDistance;
					public bool castShadows;
					public bool receiveDecals;
					public float motionBlurFactor;
					public bool staticShadows;

					//!!!!другие типы объектов
				}

				public class VariationGroup
				{
					public Variation[] variations;

					[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
					public bool CheckNeedUpdate()
					{
						if( variations != null )
						{
							foreach( var variation in variations )
							{
								if( variation != null )
								{
									if( variation.mesh != null && variation.mesh.Disposed )
										return true;
								}
							}
						}
						return false;
					}
				}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public Variation GetVariation( int groupIndex, int elementIndex )
				{
					if( variationGroups != null && groupIndex < variationGroups.Length )
					{
						var group = variationGroups[ groupIndex ];
						if( group != null && elementIndex < group.variations.Length )
							return group.variations[ elementIndex ];
					}
					return null;
				}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public bool CheckNeedUpdate()
				{
					if( variationGroups != null )
					{
						foreach( var group in variationGroups )
							if( group != null && group.CheckNeedUpdate() )
								return true;
					}
					return false;
				}
			}

			////////////

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public Element GetElement( int index )
			{
				if( index < elements.Length )
					return elements[ index ];
				return null;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public bool CheckNeedUpdate()
			{
				foreach( var element in elements )
					if( element != null && element.CheckNeedUpdate() )
						return true;
				return false;
			}
		}

		/////////////////////////////////////////

		public GroupOfObjects()
		{
			_baseObjects = new ReferenceList<Component>( this, () => BaseObjectsChanged?.Invoke( this ) );

			ComponentsChanged += GroupOfObjects_ComponentsChanged;

			unsafe
			{
				if( sizeof( Object ) != 136 )
					Log.Fatal( "GroupOfObjects: Constructor: sizeof( ObjectMesh ) != 136." );
				//if( sizeof( Object ) != 132 )
				//	Log.Fatal( "GroupOfObjects: Constructor: sizeof( ObjectMesh ) != 132." );
			}
		}

		private void GroupOfObjects_ComponentsChanged( Component obj )
		{
			NeedUpdate();
			//ElementTypesCacheNeedUpdate();
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = FindParent<Scene>();
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
				{
					scene.UpdateEvent += Scene_UpdateEvent;
					scene.GetRenderSceneData3ForGroupOfObjects += Scene_GetRenderSceneData3ForGroupOfObjects;
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
					//scene.GetRenderSceneDataAfterObjects += Scene_GetRenderSceneDataAfterObjects;
				}
				else
				{
					scene.UpdateEvent -= Scene_UpdateEvent;
					scene.GetRenderSceneData3ForGroupOfObjects -= Scene_GetRenderSceneData3ForGroupOfObjects;
					scene.GetRenderSceneData -= Scene_GetRenderSceneData;
					//scene.GetRenderSceneDataAfterObjects -= Scene_GetRenderSceneDataAfterObjects;
				}
			}

			if( EnabledInHierarchyAndIsInstance )
				CreateSectors();
			else
				DestroySectors();
		}

		unsafe static Object[] FromByteArray( byte[] array )
		{
			int count = array.Length / sizeof( Object );
			var result = new Object[ count ];
			fixed( byte* pArray = array )
			fixed( Object* pResult = result )
				NativeUtility.CopyMemory( pResult, pArray, count * sizeof( Object ) );
			return result;
		}

		unsafe static byte[] ToByteArray( Object[] array )
		{
			var result = new byte[ array.Length * sizeof( Object ) ];
			fixed( Object* pArray = array )
			fixed( byte* pResult = result )
				NativeUtility.CopyMemory( pResult, pArray, result.Length );
			return result;
		}

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			string str = block.GetAttribute( nameof( Objects ), "" );
			if( !string.IsNullOrEmpty( str ) )
			{
				var byteArray = Convert.FromBase64String( str );
				if( bool.Parse( block.GetAttribute( nameof( Objects ) + "Zip", false.ToString() ) ) )
					byteArray = IOUtility.Unzip( byteArray );

				var version = int.Parse( block.GetAttribute( nameof( Objects ) + "Version", "1" ) );
				if( version == 1 )
				{
					//convert old array format to new

					unsafe
					{
						var sizeNew = sizeof( Object );
						var sizeOld = sizeNew - 4;

						int count = byteArray.Length / sizeOld;

						var newArray = new byte[ sizeNew * count ];
						for( int n = 0; n < count; n++ )
							Array.Copy( byteArray, n * sizeOld, newArray, n * sizeNew, sizeOld );

						byteArray = newArray;
					}
				}

				Objects = FromByteArray( byteArray );

				//fill unique identifiers
				for( int n = 0; n < Objects.Length; n++ )
					Objects[ n ].UniqueIdentifier = GetUniqueIdentifier();
			}

			return true;
		}

		protected override bool OnSave( Metadata.SaveContext context, TextBlock block, ref bool skipSave, out string error )
		{
			if( !base.OnSave( context, block, ref skipSave, out error ) )
				return false;

			if( Objects != null && ObjectsSerialize )
			{
				Object[] array;
				if( removedObjects != null )
					array = ObjectsGetData( ObjectsGetAll() );
				else
					array = Objects;

				var byteArray = ToByteArray( array );

				var zipped = IOUtility.Zip( byteArray, System.IO.Compression.CompressionLevel.Fastest );
				if( zipped.Length < (int)( (float)byteArray.Length / 1.25 ) )
				{
					block.SetAttribute( nameof( Objects ), Convert.ToBase64String( zipped, Base64FormattingOptions.None ) );
					block.SetAttribute( nameof( Objects ) + "Zip", true.ToString() );
				}
				else
					block.SetAttribute( nameof( Objects ), Convert.ToBase64String( byteArray, Base64FormattingOptions.None ) );

				block.SetAttribute( nameof( Objects ) + "Version", formatVersion.ToString() );

				//block.SetAttribute( nameof( Objects ), Convert.ToBase64String( ToByteArray( array ), Base64FormattingOptions.None ) );
			}

			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void ObjectAddToSectors( int index, ESet<Sector> sectorsToUpdate )
		{
			//if( !EnabledInHierarchyAndIsInstance )
			//	return;

			ref var obj = ref Objects[ index ];

			var sectorIndex = GetSectorIndexByPosition( ref obj.Position );
			var sector = GetSectorByIndex( ref sectorIndex, true );
			sector.Objects.AddWithCheckAlreadyContained( index );

			if( !sector.objectsBoundsNeedUpdateAfterRemove )
				sector.AddToObjectsBounds( ref obj );

			sectorsToUpdate.AddWithCheckAlreadyContained( sector );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void ObjectsRemoveFromSectors( int index, ESet<Sector> sectorsToUpdate )
		{
			//if( !EnabledInHierarchyAndIsInstance )
			//	return;

			ref var obj = ref Objects[ index ];

			var sectorIndex = GetSectorIndexByPosition( ref obj.Position );
			var sector = GetSectorByIndex( ref sectorIndex, false );
			if( sector != null )
			{
				//!!!!slowly?
				sector.Objects.Remove( index );
				sector.objectsBoundsNeedUpdateAfterRemove = true;

				//!!!!slowly? can add bool Sector.addedToSectorsToUpdate
				sectorsToUpdate.AddWithCheckAlreadyContained( sector );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void UpdateSectors( ESet<Sector> sectorsToUpdate )
		{
			if( updating )
			{
				updatingSectorsToUpdate.AddRangeWithCheckAlreadyContained( sectorsToUpdate );
				return;
			}

			foreach( var sector in sectorsToUpdate )
			{
				if( !sector.IsEmpty() )
					sector.UpdateNotEmpty();
				else
				{
					sector.DestroyData();
					sectors.Remove( sector.Index );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		bool IsIndexValid( int index )
		{
			return index >= 0 && index < Objects.Length;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		bool IsIndexRemoved( int index )
		{
			return removedObjects != null && removedObjects.Data[ index ];
		}

		public void CreateSectors()
		{
			DestroySectors();

			if( !EnabledInHierarchyAndIsInstance )
				return;

			ElementTypesCacheNeedUpdate();

			var sectorsToUpdate = new ESet<Sector>();

			int capacity = ObjectsGetCapacity();
			for( int index = 0; index < capacity; index++ )
			{
				if( !IsIndexRemoved( index ) )
					ObjectAddToSectors( index, sectorsToUpdate );
			}

			UpdateSectors( sectorsToUpdate );

			//!!!!тут? где еще?
			if( NetworkMode.Value != NetworkModeEnum.False )
				NetworkSendObjects( null );
		}

		void DestroySectors()
		{
			foreach( var sector in sectors.Values )
				sector.DestroyData();
			sectors.Clear();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		Vector3I GetSectorIndexByPosition( ref Vector3 position )
		{
			var sectorSize = SectorSize.Value;
			var v = ( position + sectorSize * 0.5 ) / sectorSize;
			var index = v.ToVector3I();
			if( v.X < 0 ) index.X--;
			if( v.Y < 0 ) index.Y--;
			if( v.Z < 0 ) index.Z--;
			return index;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		Sector GetSectorByIndex( ref Vector3I index, bool canCreate )
		{
			if( !sectors.TryGetValue( index, out var sector ) )
			{
				if( canCreate )
				{
					sector = new Sector( this );
					sector.Index = index;
					sectors[ index ] = sector;
				}
			}
			return sector;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void GetSectorBounds( ref Vector3I index, out Bounds bounds )
		{
			var sectorSize = SectorSize.Value;
			var min = ( index.ToVector3() - new Vector3( 0.5, 0.5, 0.5 ) ) * sectorSize;
			var max = min + sectorSize;
			//min -= sectorSize * 0.5;
			//max -= sectorSize * 0.5;
			bounds = new Bounds( min, max );
		}

		struct LocalItem
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
		unsafe void RenderObjectsDynamicBatched( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem, Sector.RenderGroup group )
		{
			//!!!!те что по одному не инстансить. а может и больше чем 1

			var cameraSettings = context.Owner.CameraSettings;
			var objectCount = group.Objects.Count;

			GetMesh( group.Element, group.VariationGroup, group.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out var visibilityDistanceFactor, out var castShadows, out var receiveDecals, out var motionBlurFactor, out var staticShadows, out var collision );
			if( !enabled )
				return;
			if( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && !castShadows )
				return;

			//touch mesh result before multithreading
			var meshResult = mesh.Result;
			var meshDataLOD0 = meshResult.MeshData;

			LocalItem* localItems = (LocalItem*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, sizeof( LocalItem ) * objectCount );
			for( int n = 0; n < objectCount; n++ )
				localItems[ n ].added = 0;

			try
			{
				Parallel.For( 0, objectCount, delegate ( int nObject )
				{
					ref var objectItem = ref group.Objects.Data[ nObject ];
					ref var obj = ref Objects[ objectItem.ObjectIndex ];

					var add = false;
					var onlyForShadowGeneration = false;
					if( mode == GetRenderSceneDataMode.InsideFrustum )
					{
						add = true;
						onlyForShadowGeneration = !cameraSettings.Frustum.Intersects( ref objectItem.BoundingSphere );
					}
					else if( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && modeGetObjectsItem.Intersects( ref objectItem.BoundingBox ) )
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
					//var boundingSize = (float)( meshResult.SpaceBounds.boundingSphere.Radius * 2 * obj.Scale.MaxComponent() );
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

				//!!!!GC little bit
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
				templateItem.StaticShadows = staticShadows;

				var outputMeshes = context.FrameData.RenderSceneData.Meshes;

				var totalInstanceCount = 0;

				//MeshDataLastVoxelLOD
				var meshDataLods = meshDataLOD0.LODs;
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
						//!!!!why squared
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
							ref var lod = ref meshDataLOD0.LODs[ lodLevel - 1 ];
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

						int currentIndex = lodItem.InstancingStart;

						for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
						{
							ref var lodObjectItem = ref lodItem.List.Data[ nRenderableItem ];
							var nObject = lodObjectItem.ObjectIndex;

							ref var objectItem = ref group.Objects.Data[ nObject ];
							ref var obj = ref Objects[ objectItem.ObjectIndex ];

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

								//!!!!
								//Log.Info( context.OwnerCameraSettingsPositionPreviousChange.ToString() );

								//!!!!
								//var offset = ( context.OwnerCameraSettingsPosition - context.OwnerCameraSettingsPositionPrevious ).ToVector3F();
								//data.PositionPreviousFrameRelative = positionRelative + offset;

								//data.PositionPreviousFrameRelative = positionRelative;

								//objectItem.MeshMatrix.GetTranspose( out data.Transform );
								//data.PositionPreviousFrame = obj.Position.ToVector3F();
							}

							data.PreviousFramePositionChange = Vector3F.Zero;

							data.ColorPackedValue1 = objectItem.ColorForInstancingData1;
							data.ColorPackedValue2 = objectItem.ColorForInstancingData2;
							data.LodValue = lodObjectItem.LODValue;
							data.VisibilityDistance = lodObjectItem.VisibilityDistance;
							data.ReceiveDecals = receiveDecals ? (byte)255 : (byte)0;
							data.MotionBlurFactor = (byte)( motionBlurFactor * 255 );
							//data.ReceiveDecals = receiveDecals ? 1.0f : 0.0f;
							//data.MotionBlurFactor = motionBlurFactor;
							data.CullingByCameraDirectionData = obj.CullingByCameraDirectionData;

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
						//!!!!если sphere center не по нулям? везде так
						//!!!!когда не нужно, не считать? везде так
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
		unsafe void RenderObjectsNoBatchingGroup( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem, Sector.RenderGroup group )
		{
			var cameraSettings = context.Owner.CameraSettings;
			var objectCount = group.Objects.Count;

			LocalItem* localItems = (LocalItem*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, sizeof( LocalItem ) * objectCount );
			for( int n = 0; n < objectCount; n++ )
				localItems[ n ].added = 0;
			try
			{
				Parallel.For( 0, objectCount, delegate ( int nObject )
				{
					ref var objectItem = ref group.Objects.Data[ nObject ];

					//if( ( obj.Flags & Object.FlagsEnum.Enabled ) == 0 )
					//	continue;
					//if( ( obj.Flags & Object.FlagsEnum.Visible ) == 0 )
					//	continue;

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

				//!!!!что-то в Parallel. GetMesh предрасчитать

				for( int nObject = 0; nObject < objectCount; nObject++ )
				{
					ref var localItem = ref localItems[ nObject ];
					if( localItem.added == 0 )
						continue;
					var onlyForShadowGeneration = localItem.onlyForShadowGeneration != 0;

					ref var objectItem = ref group.Objects.Data[ nObject ];
					ref var obj = ref Objects[ objectItem.ObjectIndex ];

					GetMesh( obj.Element, obj.VariationGroup, obj.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out var visibilityDistanceFactor, out var castShadows, out var receiveDecals, out var motionBlurFactor, out var staticShadows, out var collision );

					if( !enabled )
						continue;
					if( onlyForShadowGeneration && !castShadows )
						continue;

					var meshDataLOD0 = mesh.Result.MeshData;

					var boundingSize = (float)( mesh.Result.SpaceBounds.boundingSphere.Radius * 2 * obj.Scale.MaxComponent() );
					var visibilityDistance = context.GetVisibilityDistanceByObjectSize( boundingSize ) * visibilityDistanceFactor * meshDataLOD0.VisibilityDistanceFactor;

					if( localItem.cameraDistanceMinSquared < visibilityDistance * visibilityDistance /*|| mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum*/ )
					{
						ref var position = ref obj.Position;
						ref var rotation = ref obj.Rotation;
						ref var scale = ref obj.Scale;

						var item = new RenderingPipeline.RenderSceneData.MeshItem();
						item.Creator = this;
						item.BoundingSphere = objectItem.BoundingSphere;
						//item.MeshData = meshDataLOD0;
						//item.MeshDataLOD0 = item.MeshData;
						item.CastShadows = castShadows && localItem.cameraDistanceMinSquared < context.GetShadowVisibilityDistanceSquared( visibilityDistance );// && item.MeshData.CastShadows;
						item.ReceiveDecals = receiveDecals;
						item.MotionBlurFactor = motionBlurFactor;
						item.ReplaceMaterial = replaceMaterial;
						item.Color = obj.Color;
						RenderingPipeline.GetColorForInstancingData( ref item.Color, out item.ColorForInstancingData1, out item.ColorForInstancingData2 );
						//item.ColorForInstancingData = RenderingPipeline.GetColorForInstancingData( ref item.Color );
						item.VisibilityDistance = visibilityDistance;
						item.OnlyForShadowGeneration = onlyForShadowGeneration;
						item.CullingByCameraDirectionData = obj.CullingByCameraDirectionData;
						item.StaticShadows = staticShadows;

						if( onlyForShadowGeneration && !item.CastShadows )
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

							item.CastShadows = castShadows && item.MeshData.CastShadows && localItem.cameraDistanceMinSquared < context.GetShadowVisibilityDistanceSquared( visibilityDistance );
							item.LODValue = SceneLODUtility.GetLodValue( context, lodRange, localItem.cameraDistanceMin );
							//item.LODRange = lodRange;

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
									context.ConvertToRelative( ref obj.Position, out var positionRelative );
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
		unsafe void SectorGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem, Sector sector )
		{
			//!!!!может сам метод вызывать мультипоточно, только нужно потом как-то их собрать
			//!!!!может GetMesh можно закешировать. и внутри его
			//!!!!может для всей группы объектов подготовить что-то. типа GetMesh
			//!!!!тени еще

			//hide label
			var context2 = context.ObjectInSpaceRenderingContext;
			context2.disableShowingLabelForThisObject = true;

			//update rendering data
			if( sector.RenderingDataNeedUpdate )
			{
				sector.RenderingDataNeedUpdate = false;
				sector.RenderingDataUpdate();
			}

			var cameraSettings = context.Owner.CameraSettings;


			//!!!!software culling buffer for subdivided


			var useSubdivided = false;
			if( SectorSubdivide >= 2 )
			{
				var cameraFrustum = cameraSettings.Frustum;

				//check by camera distance
				var cameraDistanceSquared = sector.ObjectsBounds.GetPointDistanceSquared( cameraFrustum.Origin );
				if( cameraDistanceSquared < MathEx.Square( SectorSize.Value.MinComponent() ) )
					useSubdivided = true;


				////making a lot of draw calls
				////check on the edges of the screen
				//if( !useSubdivided )
				//{
				//	var planes = cameraFrustum.Planes;

				//	sector.ObjectsBounds.GetCenter( out var boundsCenter );
				//	var boundsHalfSize = sector.ObjectsBounds.GetSize() * 0.5;

				//	if( planes[ 2 ].GetSide( ref boundsCenter, ref boundsHalfSize ) != Plane.Side.Negative ||
				//		planes[ 3 ].GetSide( ref boundsCenter, ref boundsHalfSize ) != Plane.Side.Negative ||
				//		planes[ 4 ].GetSide( ref boundsCenter, ref boundsHalfSize ) != Plane.Side.Negative ||
				//		planes[ 5 ].GetSide( ref boundsCenter, ref boundsHalfSize ) != Plane.Side.Negative )
				//	{
				//		//if( !EngineApp._DebugCapsLock )
				//		useSubdivided = true;
				//	}

				//	//if( planes[ 2 ].GetSide( ref sector.ObjectsBounds ) != Plane.Side.Negative || 
				//	//	planes[ 3 ].GetSide( ref sector.ObjectsBounds ) != Plane.Side.Negative || 
				//	//	planes[ 4 ].GetSide( ref sector.ObjectsBounds ) != Plane.Side.Negative || 
				//	//	planes[ 5 ].GetSide( ref sector.ObjectsBounds ) != Plane.Side.Negative )
				//	//{
				//	//		useSubdivided = true;
				//	//}
				//}


				////if( EngineApp._DebugCapsLock )
				////	if( cameraDistance < SectorSize.Value.MinComponent() * 1.5 )
				////		useSubdivided = true;

				////if( EngineApp._DebugCapsLock )
				////	useSubdivided = true;
			}

			var groupList = useSubdivided ? sector.RenderGroupsSubdivided : sector.RenderGroupsOne;


			var groupCount = groupList/*sector.Groups*/.Count;

			var groupItems = stackalloc GroupItem[ groupCount ];
			var groupIndexesAdded = stackalloc int[ groupCount ];
			var groupIndexesAddedCount = 0;

			for( int nGroup = 0; nGroup < groupCount; nGroup++ )
			{
				var group = groupList/*sector.Groups*/[ nGroup ];
				ref var groupItem = ref groupItems[ nGroup ];

				groupItem.Added = 0;

				if( group.NoBatchingGroup )
				{
					//no batching group is already got result here
					RenderObjectsNoBatchingGroup( context, mode, modeGetObjectsItem, group );
					//groupsAdded[ nGroup ] = 1;
				}
				else
				{
					GetMesh( group.Element, group.VariationGroup, group.VariationElement, out var enabled, out var mesh, out _, out var visibilityDistanceFactor, out var castShadows, out _, out _, out _, out _ );

					if( !enabled )
						continue;
					if( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum && !castShadows )
						continue;

					var meshResult = mesh.Result;

					var cameraDistanceMinSquared = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, ref group.BoundingBox );

					var boundingSize = (float)( meshResult.SpaceBounds.boundingSphere.Radius * 2 * group.ObjectsMaxScale );
					var visibilityDistance = context.GetVisibilityDistanceByObjectSize( boundingSize ) * visibilityDistanceFactor * meshResult.MeshData.VisibilityDistanceFactor;

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

			Parallel.For( 0, groupIndexesAddedCount, delegate ( int groupIndexesIndex )//Parallel.For( 0, groupCount, delegate ( int nGroup )
			{
				var nGroup = groupIndexesAdded[ groupIndexesIndex ];
				var group = groupList/*sector.Groups*/[ nGroup ];
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

							GetMesh( group.Element, group.VariationGroup, group.VariationElement, out var enabled, out var mesh );

							var cameraDistanceMaxSquared = SceneLODUtility.GetCameraDistanceMax( cameraSettings, ref group.BoundingSphere );
							cameraDistanceMaxSquared *= cameraDistanceMaxSquared;

							SceneLODUtility.GetDemandedLODs( context, mesh.Result.MeshData, groupItem.CameraDistanceMinSquared, cameraDistanceMaxSquared, groupItem.BoundingSize, out groupItem.LodState );
						}
					}
				}
			} );

			for( int nGroup = 0; nGroup < groupList/*sector.Groups*/.Count; nGroup++ )
			{
				ref var groupItem = ref groupItems[ nGroup ];
				if( groupItem.Added != 2 )
					continue;

				var group = groupList/*sector.Groups*/[ nGroup ];

				//if( group.NoBatchingGroup )
				//{
				//	RenderObjectsNoBatchingGroup( context, mode, modeGetObjectsItem, group );
				//}
				//else
				{
					var onlyForShadowGeneration = groupItem.OnlyForShadowGeneration != 0;
					ref var lodState = ref groupItem.LodState;

					//if( ( obj.Flags & Object.FlagsEnum.Enabled ) == 0 )
					//	continue;
					//if( ( obj.Flags & Object.FlagsEnum.Visible ) == 0 )
					//	continue;

					GetMesh( group.Element, group.VariationGroup, group.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out var visibilityDistanceFactor, out var castShadows, out var receiveDecals, out var motionBlurFactor, out var staticShadows, out var collision );

					if( !enabled )
						continue;
					if( onlyForShadowGeneration && !castShadows )
						continue;

					var cameraDistanceMaxSquared = SceneLODUtility.GetCameraDistanceMax( cameraSettings, ref group.BoundingSphere );
					cameraDistanceMaxSquared *= cameraDistanceMaxSquared;

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
						var item = new RenderingPipeline.RenderSceneData.MeshItem();
						item.Creator = this;
						item.BoundingSphere = group.BoundingSphere;
						//item.MeshData = mesh.Result.MeshData;
						//item.CastShadows = castShadows && item.MeshData.CastShadows;
						item.ReceiveDecals = receiveDecals;
						item.MotionBlurFactor = motionBlurFactor;
						item.ReplaceMaterial = replaceMaterial;
						item.Color = ColorValue.One;
						item.ColorForInstancingData1 = RenderingPipeline.ColorOneForInstancingData1;
						item.ColorForInstancingData2 = RenderingPipeline.ColorOneForInstancingData2;
						item.VisibilityDistance = groupItem.VisibilityDistance;
						item.OnlyForShadowGeneration = onlyForShadowGeneration;
						item.StaticShadows = staticShadows;

						//MeshDataLastVoxelLOD
						if( meshDataLods != null )
						{
							var lastLOD = meshDataLods[ meshDataLods.Length - 1 ];
							if( lastLOD.VoxelGridSize != 0 )
								item.MeshDataLastVoxelLOD = lastLOD.Mesh?.Result?.MeshData;
						}

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

							item.CastShadows = castShadows && item.MeshData.CastShadows && groupItem.CameraDistanceMinSquared < context.GetShadowVisibilityDistanceSquared( groupItem.VisibilityDistance );
							item.LODValue = 0;

							if( onlyForShadowGeneration && !item.CastShadows )
								continue;

							//set BatchingInstanceBuffer
							if( item.MeshData.BillboardMode != 0 )
							{
								//!!!!сразу создавать, чтобы потом не тормозило? или в потоке. опцию для создания при загрузке. где еще
								if( group.BatchingInstanceBufferBillboard == null )
									sector.CreateBatchingInstanceBufferBillboard( group, item.LODValue, /*item.VisibilityDistance, */item.ReceiveDecals, item.MotionBlurFactor, item.MeshData );

								item.InstancingEnabled = true;
								item.InstancingVertexBuffer = group.BatchingInstanceBufferBillboard;
								//!!!!может один буфер на всех собирать. где еще
								item.InstancingStart = 0;
								item.InstancingCount = -1;
								item.InstancingMaxLocalBounds = (float)( group.ObjectsMaxScale * item.MeshData.SpaceBounds.boundingSphere.Radius );
								context.ConvertToRelative( ref group.BatchingInstancePositionOffset, out item.InstancingPositionOffsetRelative );
							}
							else
							{
								if( group.BatchingInstanceBufferMesh == null )
									sector.CreateBatchingInstanceBufferMesh( group, item.LODValue, /*item.VisibilityDistance, */item.ReceiveDecals, item.MotionBlurFactor );

								item.InstancingEnabled = true;
								item.InstancingVertexBuffer = group.BatchingInstanceBufferMesh;
								item.InstancingStart = 0;
								item.InstancingCount = -1;
								item.InstancingMaxLocalBounds = (float)( group.ObjectsMaxScale * item.MeshData.SpaceBounds.boundingSphere.Radius );
								context.ConvertToRelative( ref group.BatchingInstancePositionOffset, out item.InstancingPositionOffsetRelative );

								//!!!!
								//item.PositionPreviousFrameRelative
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
							RenderObjectsNoBatchingGroup( context, mode, modeGetObjectsItem, group );
						else
							RenderObjectsDynamicBatched( context, mode, modeGetObjectsItem, group );
					}
				}
			}

			//visualize groups
			if( DrawGroupBounds )
			{
				var renderer = context.Owner.Simple3DRenderer;
				renderer.SetColor( new ColorValue( 0, 0, 1 ) );

				foreach( var group in groupList/*sector.Groups*/ )
					renderer.AddBounds( group.BoundingBox, false );
			}

			////display editor selection
			//if( mode == GetRenderSceneDataMode.InsideFrustum )
			//{
			//	//var context2 = context.objectInSpaceRenderingContext;
			//	//context2.disableShowingLabelForThisObject = true;

			//	if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
			//	{
			//		ColorValue color;
			//		if( context2.selectedObjects.Contains( this ) )
			//			color = ProjectSettings.Get.SelectedColor;
			//		else
			//			color = ProjectSettings.Get.CanSelectColor;

			//		var renderer = context.Owner.Simple3DRenderer;
			//		if( renderer != null )
			//		{
			//			//!!!!use DynamicMeshManager
			//			//!!!!!!или может проще сам меш особо рисовать

			//			color.Alpha *= .5f;
			//			renderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

			//			if( mesh.Result.MeshData.BillboardMode != 0 )
			//			{
			//				var positions = mesh.Result.ExtractedVerticesPositions;
			//				if( positions.Length != 0 )
			//				{
			//					var viewport = context.Owner;

			//					//!!!!не так

			//					Vector2 size2 = Vector2.Zero;
			//					foreach( var p in positions )
			//					{
			//						var h = p.ToVector2().Length();
			//						var v = Math.Abs( p.Z );
			//						if( h > size2.X )
			//							size2.X = h;
			//						if( v > size2.Y )
			//							size2.Y = v;
			//					}

			//					var position = tr.Position;
			//					var size = ( size2 * tr.Scale.MaxComponent() ).ToVector2F();

			//					var lookAt = Matrix3F.LookAt( viewport.CameraSettings.Direction.ToVector3F(), viewport.CameraSettings.Up.ToVector3F() );
			//					var scale = Matrix3F.FromScale( new Vector3F( 1, size.X, size.Y ) );
			//					var worldMatrix = new Matrix4( lookAt * scale, position );

			//					viewport.Simple3DRenderer.AddMesh( Billboard.GetBillboardMesh().Result, worldMatrix, false, false );
			//				}
			//				else
			//					renderer.AddBounds( SpaceBounds.CalculatedBoundingBox, true );
			//			}
			//			else
			//			{
			//				if( Math.Max( context2.selectedObjects.Count, context2.canSelectObjects.Count ) > 1000 )
			//					renderer.AddBounds( SpaceBounds.CalculatedBoundingBox, true );
			//				else
			//					renderer.AddMesh( mesh.Result, Transform.Value.ToMatrix4(), false, true );
			//			}
			//		}
			//	}
			//}

		}

		/////////////////////////////////////////

		void RemovedObjectsInit()
		{
			if( removedObjects == null )
			{
				int currentCapacity = ObjectsGetCapacity();

				removedObjects = new OpenList<bool>( currentCapacity );
				for( int index = 0; index < currentCapacity; index++ )
					removedObjects.Add( false );

				foreach( var index in freeObjects )
					removedObjects.Data[ index ] = true;
			}
		}

		//!!!!fullRecreateInternalData
		public unsafe void ObjectsSet( Object* data, int count )//, bool fullRecreateInternalData )
		{
			//if( fullRecreateInternalData )
			//{

			var array = new Object[ count ];
			if( count != 0 )
			{
				fixed( Object* pArray = array )
					NativeUtility.CopyMemory( pArray, data, array.Length * sizeof( Object ) );
			}
			Objects = array;
			removedObjects = null;
			freeObjects.Clear();

			//!!!!по идее не всегда нужно
			//fill unique identifiers
			for( int n = 0; n < Objects.Length; n++ )
				Objects[ n ].UniqueIdentifier = GetUniqueIdentifier();

			CreateSectors();

			//}
			//else
			//{
			//}
		}

		public void ObjectsSet( Object[] data, bool useDataArrayWithoutCopying = false )//, bool fullRecreateInternalData )
		{
			if( data == null )
				data = Array.Empty<Object>();

			if( useDataArrayWithoutCopying )
			{
				Objects = data;
				removedObjects = null;
				freeObjects.Clear();

				//!!!!по идее не всегда нужно
				//fill unique identifiers
				for( int n = 0; n < Objects.Length; n++ )
					Objects[ n ].UniqueIdentifier = GetUniqueIdentifier();

				CreateSectors();
			}
			else
			{
				unsafe
				{
					fixed( Object* pData = data )
						ObjectsSet( pData, data.Length );//, fullRecreateInternalData );
				}
			}
		}

		public void ObjectsSet( ArraySegment<Object> data )//, bool fullRecreateInternalData )
		{
			unsafe
			{
				fixed( Object* pData = data.Array )
					ObjectsSet( pData + data.Offset, data.Count );//, fullRecreateInternalData );
			}
		}

		public unsafe int[] ObjectsAdd( Object* data, int count )
		{
			RemovedObjectsInit();

			//expand capacity
			if( freeObjects.Count < count )
			{
				int currentCapacity = ObjectsGetCapacity();
				int currentCount = ObjectsGetCount();
				int demandCount = currentCount + count;
				int demandCapacity = Math.Max( MathEx.NextPowerOfTwo( demandCount ), currentCapacity );
				if( demandCapacity < 4 ) demandCapacity = 4;

				var newData = new Object[ demandCapacity ];
				if( Objects != null )
				{
					for( int n = 0; n < currentCapacity; n++ )
						newData[ n ] = Objects[ n ];
				}
				Objects = newData;

				for( int index = demandCapacity - 1; index >= currentCapacity; index-- )
				{
					freeObjects.Push( index );
					removedObjects.Add( true );
				}
			}

			var sectorsToUpdate = new ESet<Sector>();
			var addedIndexes = new int[ count ];

			var enabledInHierarchyAndIsInstance = EnabledInHierarchyAndIsInstance;

			for( int n = 0; n < count; n++ )
			{
				var index = freeObjects.Pop();

				ref var obj = ref Objects[ index ];
				obj = data[ n ];
				if( obj.UniqueIdentifier == 0 )
					obj.UniqueIdentifier = GetUniqueIdentifier();

				removedObjects.Data[ index ] = false;

				if( enabledInHierarchyAndIsInstance )
					ObjectAddToSectors( index, sectorsToUpdate );

				addedIndexes[ n ] = index;
			}

			UpdateSectors( sectorsToUpdate );

			return addedIndexes;
		}

		public int[] ObjectsAdd( ArraySegment<Object> data )
		{
			if( data.Count != 0 )
			{
				unsafe
				{
					fixed( Object* pData = data.Array )
						return ObjectsAdd( pData + data.Offset, data.Count );
				}
			}
			else
				return Array.Empty<int>();
		}

		public int[] ObjectsAdd( Object[] data )
		{
			if( data.Length != 0 )
			{
				unsafe
				{
					fixed( Object* pData = data )
						return ObjectsAdd( pData, data.Length );
				}
			}
			else
				return Array.Empty<int>();
		}

		public unsafe void ObjectsRemove( int* indexes, int count )
		{
			if( count == 0 )
				return;

			RemovedObjectsInit();

			var sectorsToUpdate = new ESet<Sector>();

			var enabledInHierarchyAndIsInstance = EnabledInHierarchyAndIsInstance;

			for( int n = count - 1; n >= 0; n-- )
			{
				var index = indexes[ n ];

				if( IsIndexValid( index ) && !IsIndexRemoved( index ) )
				{
					freeObjects.Push( index );
					removedObjects.Data[ index ] = true;

					if( enabledInHierarchyAndIsInstance )
						ObjectsRemoveFromSectors( index, sectorsToUpdate );
				}
			}

			UpdateSectors( sectorsToUpdate );
		}

		public void ObjectsRemove( ArraySegment<int> indexes )
		{
			if( indexes.Count == 0 )
				return;

			unsafe
			{
				fixed( int* pData = indexes.Array )
					ObjectsRemove( pData + indexes.Offset, indexes.Count );
			}
		}

		public void ObjectsRemove( int[] indexes )
		{
			if( indexes.Length == 0 )
				return;

			unsafe
			{
				fixed( int* pData = indexes )
					ObjectsRemove( pData, indexes.Length );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		int ObjectsGetCapacity()
		{
			return Objects != null ? Objects.Length : 0;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int ObjectsGetCount()
		{
			return ObjectsGetCapacity() - freeObjects.Count;
		}

		//public void ObjectsShrink()
		//{
		//}

		public List<int> ObjectsGetAll()
		{
			int capacity = ObjectsGetCapacity();
			var result = new List<int>( capacity );
			for( int index = 0; index < capacity; index++ )
			{
				//check is removed
				if( removedObjects != null && removedObjects.Data[ index ] )
					continue;
				result.Add( index );
			}
			return result;
		}

		public Object[] ObjectsGetData( IList<int> indexes )
		{
			var result = new Object[ indexes.Count ];
			if( indexes.Count != 0 )
			{
				int currentResult = 0;
				foreach( var index in indexes )
					result[ currentResult++ ] = Objects[ index ];
			}
			return result;
		}

		//public void ObjectsGetData( int index, out ObjectMesh data )
		//{
		//	data = ObjectsMesh[ index ];
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ref Object ObjectGetData( int index )
		{
			return ref Objects[ index ];
		}

		/// <summary>
		/// Removes objects by their unique identifiers.
		/// </summary>
		/// <param name="data"></param>
		public int ObjectsRemove( Object[] data )
		{
			var set = new ESet<long>( data.Length );
			for( int n = 0; n < data.Length; n++ )
			{
				if( data[ n ].UniqueIdentifier == 0 )
					Log.Fatal( "GroupOfObjects: ObjectsRemove: data[ n ].UniqueIdentifier == 0." );
				set.AddWithCheckAlreadyContained( data[ n ].UniqueIdentifier );
			}

			var indexesToRemove = new List<int>( data.Length );

			foreach( var index in ObjectsGetAll() )
			{
				ref var obj = ref ObjectGetData( index );
				if( set.Contains( obj.UniqueIdentifier ) )
					indexesToRemove.Add( index );
			}

			ObjectsRemove( indexesToRemove.ToArray() );

			return indexesToRemove.Count;
		}

		/////////////////////////////////////////

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool ObjectsExists()
		{
			return ObjectsGetCount() != 0;
		}

		public void ClearObjects()
		{
			ObjectsSet( new Object[ 0 ] );//, true );
		}

		/////////////////////////////////////////

		void ElementTypesCacheCheckNeedUpdate()
		{
			if( !elementTypesCacheNeedUpdate && elementTypesCache != null )
			{
				if( elementTypesCache.CheckNeedUpdate() )
					elementTypesCacheNeedUpdate = true;
			}
		}

		void ElementTypesCacheUpdate()
		{
			elementTypesCacheNeedUpdate = false;

			var elements = GetComponents<GroupOfObjectsElement>();

			int maxElementIndex = 0;
			foreach( var element in elements )
				maxElementIndex = Math.Max( maxElementIndex, element.Index );

			elementTypesCache = new ElementTypesCache();
			elementTypesCache.elements = new ElementTypesCache.Element[ maxElementIndex + 1 ];

			foreach( var element in elements )
			{
				var elementData = new ElementTypesCache.Element();
				elementData.enabled = element.Enabled;
				elementTypesCache.elements[ element.Index ] = elementData;

				//mesh
				var elementMesh = element as GroupOfObjectsElement_Mesh;
				if( elementMesh != null )
				{
					elementData.collision = elementMesh.Collision;

					elementData.variationGroups = new ElementTypesCache.Element.VariationGroup[ 1 ];
					var group = new ElementTypesCache.Element.VariationGroup();
					{
						var variation = new ElementTypesCache.Element.Variation();
						variation.mesh = elementMesh.Mesh.Value;
						variation.replaceMaterial = elementMesh.ReplaceMaterial;
						variation.visibilityDistanceFactor = (float)elementMesh.VisibilityDistanceFactor;
						variation.castShadows = elementMesh.CastShadows;
						variation.receiveDecals = elementMesh.ReceiveDecals;
						variation.motionBlurFactor = (float)elementMesh.MotionBlurFactor;
						variation.staticShadows = elementMesh.StaticShadows;

						group.variations = new ElementTypesCache.Element.Variation[] { variation };
					}
					elementData.variationGroups[ 0 ] = group;
				}

				//surface
				var elementSurface = element as GroupOfObjectsElement_Surface;
				if( elementSurface != null )
				{
					elementData.collision = elementSurface.Collision;

					var surface = elementSurface.Surface.Value;
					if( surface != null )
					{
						var surfaceGroups = surface.GetComponents<SurfaceGroupOfElements>();

						elementData.variationGroups = new ElementTypesCache.Element.VariationGroup[ surfaceGroups.Length ];

						for( int nGroup = 0; nGroup < surfaceGroups.Length; nGroup++ )
						{
							var surfaceGroup = surfaceGroups[ nGroup ];
							var surfaceElements = surfaceGroup.GetComponents<SurfaceElement>();

							var group = new ElementTypesCache.Element.VariationGroup();
							group.variations = new ElementTypesCache.Element.Variation[ surfaceElements.Length ];

							for( int nVariation = 0; nVariation < group.variations.Length; nVariation++ )
							{
								var surfaceElement = surfaceElements[ nVariation ];

								var variation = new ElementTypesCache.Element.Variation();

								//mesh
								var surfaceElementMesh = surfaceElement as SurfaceElement_Mesh;
								if( surfaceElementMesh != null )
								{
									variation.mesh = surfaceElementMesh.Mesh;
									variation.replaceMaterial = surfaceElementMesh.ReplaceMaterial;

									variation.visibilityDistanceFactor = (float)surfaceElementMesh.VisibilityDistanceFactor * (float)elementSurface.VisibilityDistanceFactor;
									variation.castShadows = surfaceElementMesh.CastShadows & elementSurface.CastShadows;
									variation.receiveDecals = surfaceElementMesh.ReceiveDecals;
									variation.motionBlurFactor = (float)surfaceElementMesh.MotionBlurFactor;
									variation.staticShadows = surfaceElementMesh.StaticShadows;
								}

								group.variations[ nVariation ] = variation;
							}

							elementData.variationGroups[ nGroup ] = group;
						}

						//element.variationMeshes = new Mesh[ surfaceElements.Length ];
						//for( int n = 0; n < surfaceElements.Length; n++ )
						//{
						//	var brushElement = surfaceElements[ n ];

						//	var brushElementMesh = brushElement as SurfaceElement_Mesh;
						//	if( brushElementMesh != null )
						//		element.variationMeshes[ n ] = brushElementMesh.Mesh;
						//}


						//var surfaceElements = surface.GetComponents<SurfaceElement>();

						//element.variationMeshes = new Mesh[ surfaceElements.Length ];
						//for( int n = 0; n < surfaceElements.Length; n++ )
						//{
						//	var brushElement = surfaceElements[ n ];

						//	var brushElementMesh = brushElement as SurfaceElement_Mesh;
						//	if( brushElementMesh != null )
						//		element.variationMeshes[ n ] = brushElementMesh.Mesh;
						//}
					}
				}
			}

			//!!!!по идее секторы тоже обновить, т.к. bounds
		}

		//class GetMeshData
		//{
		//	public bool enabled;
		//	public Mesh mesh;
		//	public Material replaceMaterial;
		//	public float visibilityDistanceFactor;
		//	public bool castShadows;
		//	public bool receiveDecals;
		//	public float motionBlurFactor;
		//	public bool collision;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void GetMesh( int elementIndex, int variationGroup, int variationElement, out bool enabled, out Mesh mesh, out Material replaceMaterial, out float visibilityDistanceFactor, out bool castShadows, out bool receiveDecals, out float motionBlurFactor, out bool staticShadows, out bool collision )
		{
			enabled = false;
			mesh = null;
			replaceMaterial = null;
			visibilityDistanceFactor = 1;//visibilityDistanceFactor = 10000;
			castShadows = false;
			receiveDecals = false;
			motionBlurFactor = 1.0f;
			staticShadows = true;
			collision = false;

			if( elementTypesCacheNeedUpdate )
				ElementTypesCacheUpdate();

			if( elementTypesCache != null )
			{
				var elementData = elementTypesCache.GetElement( elementIndex );
				if( elementData != null && elementData.enabled )
				{
					enabled = true;

					var variation = elementData.GetVariation( variationGroup, variationElement );
					if( variation != null )
					{
						mesh = variation.mesh;
						replaceMaterial = variation.replaceMaterial;
						visibilityDistanceFactor = variation.visibilityDistanceFactor;
						castShadows = variation.castShadows;
						receiveDecals = variation.receiveDecals;
						motionBlurFactor = variation.motionBlurFactor;
						staticShadows = variation.staticShadows;
					}

					collision = elementData.collision;
				}
			}

			if( mesh == null || mesh.Result == null )
				mesh = ResourceUtility.MeshInvalid;

			var meshData = mesh.Result.MeshData;
			visibilityDistanceFactor = visibilityDistanceFactor * meshData.VisibilityDistanceFactor;
			//visibilityDistance = Math.Min( visibilityDistance, meshData.VisibilityDistance );
			//!!!!LODScale?
			castShadows = castShadows && meshData.CastShadows;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void GetMesh( int elementIndex, int variationGroup, int variationElement, out bool enabled, out Mesh mesh )
		{
			enabled = false;
			mesh = null;

			if( elementTypesCacheNeedUpdate )
				ElementTypesCacheUpdate();

			if( elementTypesCache != null )
			{
				var elementData = elementTypesCache.GetElement( elementIndex );
				if( elementData != null && elementData.enabled )
				{
					enabled = true;

					var variation = elementData.GetVariation( variationGroup, variationElement );
					if( variation != null )
						mesh = variation.mesh;
				}
			}

			if( mesh == null || mesh.Result == null )
				mesh = ResourceUtility.MeshInvalid;
		}

		public void ElementTypesCacheNeedUpdate()
		{
			elementTypesCacheNeedUpdate = true;
		}

		public void NeedUpdate()
		{
			needUpdate = true;
			elementTypesCacheNeedUpdate = true;
		}

		public Bounds CalculateTotalBounds()
		{
			Bounds result = Bounds.Cleared;
			foreach( var sector in sectors.Values )
			{
				if( sector.ObjectsBoundsCalculated )
					result.Add( sector.ObjectsBounds );
				//if( sector.ObjectsBounds != null )
				//	result.Add( sector.ObjectsBounds.CalculatedBoundingBox );
			}
			return result;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			ElementTypesCacheCheckNeedUpdate();

			if( needRemoveEmptyElements )
			{
				needRemoveEmptyElements = false;
				RemoveEmptyElements();
			}

			if( needUpdate )
			{
				needUpdate = false;
				CreateSectors();
			}

			if( needUpdateStaticShadows )
			{
				foreach( var sector in sectors.Values )
				{
					//!!!!?
					if( !sector.StaticShadowsAffectedBounds.IsCleared() )
						sector.ResetLightStaticShadowsCache( ref sector.StaticShadowsAffectedBounds );
				}
				needUpdateStaticShadows = false;
			}
		}

		private void Scene_UpdateEvent( Component sender, float delta )
		{
			ProcessSubGroupActionQueue();
		}

		private void Scene_GetRenderSceneData3ForGroupOfObjects( Scene scene, ViewportRenderingContext context )
		{
			ProcessSubGroupActionQueue();
		}

		private void Scene_GetRenderSceneData( Scene scene, ViewportRenderingContext context )
		{
			//_tempSectorsGetSceneData.Clear();

#if !DEPLOY
			var context2 = context.ObjectInSpaceRenderingContext;
			if( context2.selectedObjects.Contains( this ) )
			{
				var bounds = CalculateTotalBounds();
				if( !bounds.IsCleared() )
				{
					if( context.Owner.Simple3DRenderer != null )
					{
						ColorValue color = ProjectSettings.Get.Colors.SelectedColor;
						context.Owner.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
						context.Owner.Simple3DRenderer.AddBounds( bounds );
					}
				}
			}
#endif
		}

		//private void Scene_GetRenderSceneDataAfterObjects( Scene scene, ViewportRenderingContext context )
		//{
		//	if( _tempSectorsGetSceneData.Count != 0 )
		//	{
		//		for( int n = 0; n < _tempSectorsGetSceneData.Count; n++ )
		//		{
		//			ref var item = ref _tempSectorsGetSceneData.Data[ n ];
		//			SectorGetRenderSceneData( context, item.mode, item.modeGetObjectsItem, item.sector );
		//		}

		//		//foreach( var sector in sectors )
		//		//	SectorGetRenderSceneData( context, mode, modeGetObjectsItem, sector );

		//		_tempSectorsGetSceneData.Clear();
		//	}
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public GroupOfObjectsElement GetElement( int elementIndex )
		{
			//!!!!slowly? кешировать?

			foreach( var element in GetComponents<GroupOfObjectsElement>() )
			{
				if( element.Index == elementIndex )
					return element;
			}
			return null;
		}

		//public int GetElementIndex( GroupOfObjectsElement element )
		//{
		//	//!!!!slowly?
		//	var elements = GetComponents<GroupOfObjectsElement>();
		//	return Array.IndexOf( elements, element );
		//}

		long GetUniqueIdentifier()
		{
			var result = uniqueIdentifierCounter;
			unchecked
			{
				uniqueIdentifierCounter++;
			}
			return result;
		}

		public List<Component> GetBaseObjects()
		{
			var baseObjects = new List<Component>( BaseObjects.Count );
			for( int n = 0; n < BaseObjects.Count; n++ )
			{
				var component = BaseObjects[ n ].Value;
				if( component != null )
					baseObjects.Add( component );
			}
			return baseObjects;
		}

		public int GetFreeElementIndex()
		{
			int maxIndex = -1;
			foreach( var element in GetComponents<GroupOfObjectsElement>() )
				maxIndex = Math.Max( maxIndex, element.Index );
			return maxIndex + 1;
		}

		//public void ResetLodTransitionStates( ViewportRenderingContext resetOnlySpecifiedContext = null )
		//{
		//	foreach( var sector in sectors.Values )
		//		foreach( var batch in sector.Batches )
		//			SceneLODUtility.ResetLodTransitionStates( ref batch.renderingContextItems, resetOnlySpecifiedContext );
		//}

		void BeginUpdate()//public void BeginUpdate()
		{
			if( updating )
				return;

			updating = true;
			updatingSectorsToUpdate = new ESet<Sector>();
		}

		void EndUpdate()//public void EndUpdate()
		{
			if( !updating )
				return;

			var sectorsToUpdate = updatingSectorsToUpdate;
			updating = false;
			updatingSectorsToUpdate = null;

			UpdateSectors( sectorsToUpdate );
		}

		public void NeedRemoveEmptyElements()
		{
			needRemoveEmptyElements = true;
		}

		void RemoveEmptyElements()
		{
			var usedElements = new ESet<int>();

			foreach( var index in ObjectsGetAll() )
			{
				ref var obj = ref ObjectGetData( index );
				usedElements.AddWithCheckAlreadyContained( obj.Element );
			}

			foreach( var element in GetComponents<GroupOfObjectsElement>() )
			{
				if( !usedElements.Contains( element.Index ) )
					element.RemoveFromParent( true );
			}
		}

		void NetworkSendObjects( ServerNetworkService_Components.ClientItem client )
		{
			if( NetworkIsServer && ObjectsNetworkMode.Value && Objects != null )
			{

				//!!!!частичное обновление

				var writer = client != null ? BeginNetworkMessage( client, "Objects" ) : BeginNetworkMessageToEveryone( "Objects" );
				if( writer != null )
				{
					var count = 0;
					int capacity = ObjectsGetCapacity();
					for( int index = 0; index < capacity; index++ )
					{
						//check is removed
						if( removedObjects != null && removedObjects.Data[ index ] )
							continue;
						count++;
					}

					writer.Write( count );

					if( count > 0 )
					{
						unsafe
						{
							fixed( Object* pObjects = Objects )
							{
								for( int index = 0; index < capacity; index++ )
								{
									//check is removed
									if( removedObjects != null && removedObjects.Data[ index ] )
										continue;

									//ref var obj = ref Objects[ index ];

									//!!!!check
									writer.Write( pObjects + index, sizeof( Object ) );
								}
							}
						}
					}

					EndNetworkMessage();
				}
			}
		}

		protected override void OnClientConnectedBeforeRootComponentEnabled( ServerNetworkService_Components.ClientItem client )
		{
			base.OnClientConnectedBeforeRootComponentEnabled( client );

			if( NetworkMode.Value != NetworkModeEnum.False )
				NetworkSendObjects( client );
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "Objects" )
			{
				var count = reader.ReadInt32();

				var data = new Object[ count ];
				unsafe
				{
					fixed( Object* pData = data )
					{
						for( int n = 0; n < count; n++ )
							reader.ReadBuffer( pData + n, sizeof( Object ) );
					}
				}

				ObjectsSet( data, true );

				if( !reader.Complete() )
					return false;
			}

			return true;
		}

		///////////////////////////////////////////////

		public class SubGroup
		{
			//!!!!

			internal bool Added;
			internal Dictionary<ushort, int> elementsInUse = new Dictionary<ushort, int>();


			public ArraySegment<Object> Objects;
			//!!!!
			//public Bounds Bounds = Bounds.Cleared;

			//!!!!
			//public delegate void GetDataDelegate( SubGroup sender );
			//public event GetDataDelegate GetData;

			internal int[] ObjectIndexes;

			//!!!!
			//bool Dynamic;

			//


			//!!!!
			//UpdateTransform
			//interpolation. two transforms
			//partial update

			public SubGroup()
			{
			}

			//!!!!Bounds?
			public SubGroup( ArraySegment<Object> objects )
			{
				Objects = objects;
			}
		}

		///////////////////////////////////////////////

		struct SubGroupAction
		{
			public ActionEnum Action;

			public SubGroup SubGroup;
			public GroupOfObjectsElement Element;

			public enum ActionEnum
			{
				AddSubGroup,
				RemoveSubGroup,
				RemoveElement,
			}
		}

		///////////////////////////////////////////////

		public void AddSubGroupToQueue( SubGroup subGroup )
		{
			var action = new SubGroupAction();
			action.SubGroup = subGroup;
			action.Action = SubGroupAction.ActionEnum.AddSubGroup;
			subGroupActionQueue.Enqueue( action );
		}

		public void RemoveSubGroupToQueue( SubGroup subGroup )
		{
			var action = new SubGroupAction();
			action.SubGroup = subGroup;
			action.Action = SubGroupAction.ActionEnum.RemoveSubGroup;
			subGroupActionQueue.Enqueue( action );
		}

		public void RemoveElementToQueue( GroupOfObjectsElement element )// ushort elementIndex )
		{
			var action = new SubGroupAction();
			action.Element = element;
			action.Action = SubGroupAction.ActionEnum.RemoveElement;
			subGroupActionQueue.Enqueue( action );
		}

		public void ProcessSubGroupActionQueue()
		{
			if( subGroupActionQueue.Count != 0 )
			{

				//!!!!slowly
				//without using Objects
				//threading
				//copy to gpu
				//dynamic update
				//sub group bounds
				//sub group get data when need


				BeginUpdate();

				while( subGroupActionQueue.Count != 0 )
				{
					var action = subGroupActionQueue.Dequeue();
					var subGroup = action.SubGroup;

					switch( action.Action )
					{
					case SubGroupAction.ActionEnum.AddSubGroup:
						{
							var process = true;

							//skip when it will removed later
							foreach( var a in subGroupActionQueue )
							{
								if( a.SubGroup == subGroup )
								{
									if( a.Action == SubGroupAction.ActionEnum.RemoveSubGroup )
										process = false;
									break;
								}
							}

							if( process )
							{
								subGroup.ObjectIndexes = ObjectsAdd( subGroup.Objects );
								subGroup.Objects = null;
							}
						}
						break;

					case SubGroupAction.ActionEnum.RemoveSubGroup:
						{
							if( subGroup.ObjectIndexes != null )
							{
								ObjectsRemove( subGroup.ObjectIndexes );
								subGroup.ObjectIndexes = null;
							}
						}
						break;

					case SubGroupAction.ActionEnum.RemoveElement:
						action.Element.RemoveFromParent( false );
						break;
					}
				}

				EndUpdate();
			}
		}

		public void NeedUpdateStaticShadows()
		{
			needUpdateStaticShadows = true;
		}
	}
}
