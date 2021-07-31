// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using NeoAxis.Editor;
using System.Linq;
using System.Threading.Tasks;

namespace NeoAxis
{
	/// <summary>
	/// An object in a scene designed to store and display a large number of similar objects.
	/// </summary>
	[EditorSettingsCell( typeof( Component_GroupOfObjects_SettingsCell ) )]
	public partial class Component_GroupOfObjects : Component, IComponent_VisibleInHierarchy
	{
		Dictionary<Vector3I, Sector> sectors = new Dictionary<Vector3I, Sector>();

		//Count = capacity of Object or can be null.
		OpenList<bool> removedObjects;
		Stack<int> freeObjects = new Stack<int>();

		ElementTypesCache elementTypesCache;
		bool elementTypesCacheNeedUpdate = true;

		long uniqueIdentifierCounter = 1;

		/////////////////////////////////////////

		/// <summary>
		/// Whether the object is visible in the scene.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> Visible
		{
			get { if( _visible.BeginGet() ) Visible = _visible.Get( this ); return _visible.value; }
			set { if( _visible.BeginSet( ref value ) ) { try { VisibleChanged?.Invoke( this ); } finally { _visible.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Visible"/> property value changes.</summary>
		public event Action<Component_GroupOfObjects> VisibleChanged;
		ReferenceField<bool> _visible = true;

		[Browsable( false )]
		public bool VisibleInHierarchy
		{
			get
			{
				if( !Visible )
					return false;

				var p = Parent as IComponent_VisibleInHierarchy;
				if( p != null )
					return p.VisibleInHierarchy;
				else
					return true;
			}
		}

		//скорее всего без этого, т.к. _GroupOfObjects можно будет в меше иметь
		//может пока без этого? тогда проще будет работать с этим всем. а что с террейном тогда?
		//может тогда тулу для сдвига?
		//need?
		//заюзать
		//[DefaultValue( "0 0 0" )]
		//public Reference<Vector3> Position
		//{
		//	get { if( _position.BeginGet() ) Position = _position.Get( this ); return _position.value; }
		//	set
		//	{
		//		if( _position.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				PositionChanged?.Invoke( this );

		//				x;
		//			}
		//			finally { _position.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_GroupOfObjects> PositionChanged;
		//ReferenceField<Vector3> _position;

		/// <summary>
		/// The list of base objects. When creating objects, the editor uses base objects as a background on which objects are placed.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		public ReferenceList<Component> BaseObjects
		{
			get { return _baseObjects; }
		}
		public delegate void BaseObjectsChangedDelegate( Component_GroupOfObjects sender );
		public event BaseObjectsChangedDelegate BaseObjectsChanged;
		ReferenceList<Component> _baseObjects;

		//!!!!default value
		/// <summary>
		/// The size of the sector in the scene. The sector size allows to configure the grouping of objects.
		/// </summary>
		[DefaultValue( "200 200 10000" )]
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
				if( _sectorSize.BeginSet( ref value ) )
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
		public event Action<Component_GroupOfObjects> SectorSizeChanged;
		ReferenceField<Vector3> _sectorSize = new Vector3( 200, 200, 10000 );

		///// <summary>
		///// Whether to enable batching rendering optimization. The batching is supported only for non-transparent objects.
		///// </summary>
		//[DefaultValue( true )]
		//public Reference<bool> Batching
		//{
		//	get { if( batching.BeginGet() ) Batching = batching.Get( this ); return batching.value; }
		//	set
		//	{
		//		if( batching.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				BatchingChanged?.Invoke( this );
		//				UpdateSectors( null );
		//			}
		//			finally { batching.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="Batching"/> property value changes.</summary>
		//public event Action<Component_GroupOfObjects> BatchingChanged;
		//ReferenceField<bool> batching = true;

		///// <summary>
		///// Whether to enable rendering by means groups. The batching is supported only for non-transparent objects.
		///// </summary>
		//[DefaultValue( true )]
		//public Reference<bool> Grouping
		//{
		//	get { if( _grouping.BeginGet() ) Grouping = _grouping.Get( this ); return _grouping.value; }
		//	set { if( _grouping.BeginSet( ref value ) ) { try { GroupingChanged?.Invoke( this ); CreateRenderingData(); } finally { _grouping.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Grouping"/> property value changes.</summary>
		//public event Action<Component_GroupOfObjects> GroupingChanged;
		//ReferenceField<bool> _grouping = true;

		//!!!!default value
		/// <summary>
		/// The maximal amount of objects in one group/batch.
		/// </summary>
		[DefaultValue( 1000 )]
		public Reference<int> MaxObjectsInGroup
		{
			get { if( _maxObjectsInGroup.BeginGet() ) MaxObjectsInGroup = _maxObjectsInGroup.Get( this ); return _maxObjectsInGroup.value; }
			set { if( _maxObjectsInGroup.BeginSet( ref value ) ) { try { MaxObjectsInGroupChanged?.Invoke( this ); CreateSectors(); } finally { _maxObjectsInGroup.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxObjectsInGroup"/> property value changes.</summary>
		public event Action<Component_GroupOfObjects> MaxObjectsInGroupChanged;
		ReferenceField<int> _maxObjectsInGroup = 1000;

		///// <summary>
		///// Whether to use GPU batching rendering. The batching is supported only for non-transparent objects.
		///// </summary>
		//[DefaultValue( true )]
		//[DisplayName( "GPU Batching" )]
		//public Reference<bool> GPUBatching
		//{
		//	get { if( _gPUBatching.BeginGet() ) GPUBatching = _gPUBatching.Get( this ); return _gPUBatching.value; }
		//	set { if( _gPUBatching.BeginSet( ref value ) ) { try { GPUBatchingChanged?.Invoke( this ); CreateSectors(); } finally { _gPUBatching.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="GPUBatching"/> property value changes.</summary>
		//public event Action<Component_GroupOfObjects> GPUBatchingChanged;
		//ReferenceField<bool> _gPUBatching = true;

		[Category( "Debug" )]
		[DefaultValue( false )]
		public Reference<bool> DrawGroupBounds
		{
			get { if( _drawGroupBounds.BeginGet() ) DrawGroupBounds = _drawGroupBounds.Get( this ); return _drawGroupBounds.value; }
			set { if( _drawGroupBounds.BeginSet( ref value ) ) { try { DrawGroupBoundsChanged?.Invoke( this ); } finally { _drawGroupBounds.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DrawGroupBounds"/> property value changes.</summary>
		public event Action<Component_GroupOfObjects> DrawGroupBoundsChanged;
		ReferenceField<bool> _drawGroupBounds = false;

		[Browsable( false )]
		public bool EditorAllowUsePaintBrush { get; set; } = true;

		Object[] Objects { get; set; }

		/////////////////////////////////////////

		/// <summary>
		/// Represents an object for <see cref="Component_GroupOfObjects"/>.
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

			////////////

			[Flags]
			public enum FlagsEnum// : byte
			{
				Enabled = 1,
				Visible = 2,
			}

			////////////

			public Object( ushort element, byte variationGroup, byte variationElement, FlagsEnum flags, Vector3 position, QuaternionF rotation, Vector3F scale, Vector4F anyData, ColorValue color, Vector4F special1, Vector4F special2 )
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
			}

			public Object( ushort element, byte variationGroup, byte variationElement, FlagsEnum flags, ref Vector3 position, ref QuaternionF rotation, ref Vector3F scale, ref Vector4F anyData, ref ColorValue color, ref Vector4F special1, ref Vector4F special2 )
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

		class Sector
		{
			public Component_GroupOfObjects owner;
			public Vector3I Index;

			//!!!!state. loading, loaded.

			public ESet<int> Objects = new ESet<int>();

			public SpaceBounds ObjectsBounds;
			public Bounds ObjectsBoundingBox;
			public bool objectsBoundsNeedUpdateAfterRemove;

			public Component_ObjectInSpace ObjectInSpace;

			//rendering data
			public bool RenderingDataNeedUpdate = true;
			//sorted by group.VisibilityDistance from far to nearest
			public List<Group> Groups = new List<Group>();

			////////////

			public class Group
			{
				public Sector Owner;

				public bool NoBatchingGroup;
				//batching
				public ushort Element;
				public byte VariationGroup;
				public byte VariationElement;

				public struct ObjectItem
				{
					public int ObjectIndex;
					public Matrix3F MeshMatrix3;
					public Bounds BoundingBox;
					public Sphere BoundingSphere;
				}
				public OpenList<ObjectItem> Objects = new OpenList<ObjectItem>();

				public Bounds BoundingBox;
				public Sphere BoundingSphere;
				//public SpaceBounds Bounds;

				public float VisibilityDistance;
				public float VisibilityDistanceSquared;

				public GpuVertexBuffer BatchingInstanceBufferMesh;
				public GpuVertexBuffer BatchingInstanceBufferBillboard;

				//for RenderingDataUpdate() method
				public int NextSplitAxis;
			}

			////////////

			public Sector( Component_GroupOfObjects owner )
			{
				this.owner = owner;
			}

			public override string ToString()
			{
				return Index.ToString();
			}

			public bool IsEmpty()
			{
				return Objects.Count == 0;
			}

			public void AddToObjectsBounds( ref Object obj )
			{
				//!!!!не только меши
				owner.GetMesh( obj.Element, obj.VariationGroup, obj.VariationElement, out _, out var mesh, out _, out _, out _, out _ );

				//!!!!enabled

				var transform = new Transform( obj.Position, obj.Rotation, obj.Scale );
				var meshSpaceBounds = mesh.Result.SpaceBounds;
				//!!!!можно считать оптимальнее?
				var b = SpaceBounds.Multiply( transform, meshSpaceBounds );

				ObjectsBounds = SpaceBounds.Merge( ObjectsBounds, b );
				if( ObjectsBounds != null )
					ObjectsBounds.GetCalculatedBoundingBox( out ObjectsBoundingBox );
				else
					ObjectsBoundingBox = Bounds.Cleared;
			}

			public void Update()
			{
				//!!!!трединг

				//!!!!не только меши

				if( !IsEmpty() )
				{
					//update ObjectsBounds
					if( objectsBoundsNeedUpdateAfterRemove )
					{
						objectsBoundsNeedUpdateAfterRemove = false;
						ObjectsBounds = null;
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

						ObjectInSpace = sectors.CreateComponent<Component_ObjectInSpace>( enabled: false );
						ObjectInSpace.AnyData = this;
						ObjectInSpace.Name = Index.ToString();
						ObjectInSpace.SaveSupport = false;
						ObjectInSpace.CloneSupport = false;
						ObjectInSpace.CanBeSelected = false;
						ObjectInSpace.SpaceBoundsUpdateEvent += ObjectInSpace_SpaceBoundsUpdateEvent;
						ObjectInSpace.Transform = new Transform( ObjectsBounds.CalculatedBoundingBox.GetCenter(), Quaternion.Identity );
						ObjectInSpace.GetRenderSceneData += ObjectInSpace_GetRenderSceneData;
						ObjectInSpace.Enabled = true;
					}
					else
					{
						ObjectInSpace.Transform = new Transform( ObjectsBounds.CalculatedBoundingBox.GetCenter(), Quaternion.Identity );
						ObjectInSpace.SpaceBoundsUpdate();
					}
				}

				RenderingDataNeedUpdate = true;
			}

			public void Destroy()
			{
				if( ObjectInSpace != null )
				{
					ObjectInSpace.GetRenderSceneData -= ObjectInSpace_GetRenderSceneData;
					ObjectInSpace.RemoveFromParent( false );
					//ObjectInSpace.RemoveFromParent( true );
					ObjectInSpace = null;
				}

				RenderingDataDestroy();
			}

			private void ObjectInSpace_SpaceBoundsUpdateEvent( Component_ObjectInSpace obj, ref SpaceBounds newBounds )
			{
				var sector = obj.AnyData as Sector;
				if( sector != null && sector.ObjectsBounds != null )
					newBounds = sector.ObjectsBounds;
			}

			private void ObjectInSpace_GetRenderSceneData( Component_ObjectInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem )
			{
				if( owner.VisibleInHierarchy )
				{
					var sector = sender.AnyData as Sector;
					if( sector != null )
						owner.SectorGetRenderSceneData( context, mode, modeGetObjectsItem, sector );
				}
			}

			void RenderingDataDestroy()
			{
				foreach( var batch in Groups )
				{
					batch.BatchingInstanceBufferMesh?.Dispose();
					batch.BatchingInstanceBufferMesh = null;
					batch.BatchingInstanceBufferBillboard?.Dispose();
					batch.BatchingInstanceBufferBillboard = null;
				}
				Groups.Clear();
			}

			Group GetGroup( bool noBatchingGroup, ushort elementIndex, byte variationGroup, byte variationElement )
			{
				for( int nGroup = 0; nGroup < Groups.Count; nGroup++ )
				{
					var group = Groups[ nGroup ];

					if( noBatchingGroup == group.NoBatchingGroup )
					{
						if( noBatchingGroup )
							return group;
						else if( group.Element == elementIndex && group.VariationGroup == variationGroup && group.VariationElement == variationElement )
							return group;
					}
				}
				return null;
			}

			bool SupportsBatching( Component_Mesh mesh )
			{
				if( mesh.Result != null )
				{
					var meshData = mesh.Result.MeshData;

					foreach( var oper in meshData.RenderOperations )
						if( oper.Material != null && oper.Material.Result != null && oper.Material.Result.Transparent )
							return false;

					if( meshData.LODs != null )
					{
						foreach( var lod in meshData.LODs )
							if( lod.Mesh != null && !SupportsBatching( lod.Mesh ) )
								return false;
					}
				}

				return true;
			}

			public unsafe void CreateBatchingInstanceBufferMesh( Group batch, float lodValue, float visibilityDistance, bool receiveDecals )
			{
				var vertices = new byte[ sizeof( Component_RenderingPipeline.RenderSceneData.ObjectInstanceData ) * batch.Objects.Count ];
				fixed( byte* pVertices = vertices )
				{
					for( int n = 0; n < batch.Objects.Count; n++ )
					{
						var instanceData = (Component_RenderingPipeline.RenderSceneData.ObjectInstanceData*)pVertices + n;

						ref var objectItem = ref batch.Objects.Data[ n ];
						ref var obj = ref owner.Objects[ objectItem.ObjectIndex ];

						//!!!!double
						var pos = obj.Position.ToVector3F();
						var matrix = new Matrix4F( ref objectItem.MeshMatrix3, ref pos );

						matrix.GetTranslation( out var positionPreviousFrame );
						instanceData->Init( ref matrix, ref positionPreviousFrame, ref obj.Color, lodValue, visibilityDistance, receiveDecals );
					}
				}

				var vertexElements = new VertexElement[] {
					new VertexElement( 0, 0, VertexElementType.Float4, VertexElementSemantic.TextureCoordinate7 ),
					new VertexElement( 0, 16, VertexElementType.Float4, VertexElementSemantic.TextureCoordinate6 ),
					new VertexElement( 0, 32, VertexElementType.Float4, VertexElementSemantic.TextureCoordinate5 ) ,
					new VertexElement( 0, 48, VertexElementType.Float4, VertexElementSemantic.TextureCoordinate4 ),
					new VertexElement( 0, 64, VertexElementType.Float4, VertexElementSemantic.TextureCoordinate3 ) };

				var vertexDeclaration = vertexElements.CreateVertexDeclaration( 0 );

				batch.BatchingInstanceBufferMesh = GpuBufferManager.CreateVertexBuffer( vertices, vertexDeclaration, 0 );
			}

			public unsafe void CreateBatchingInstanceBufferBillboard( Group batch, float lodValue, float visibilityDistance, bool receiveDecals )
			{
				var vertices = new byte[ sizeof( Component_RenderingPipeline.RenderSceneData.ObjectInstanceData ) * batch.Objects.Count ];
				fixed( byte* pVertices = vertices )
				{
					for( int n = 0; n < batch.Objects.Count; n++ )
					{
						var instanceData = (Component_RenderingPipeline.RenderSceneData.ObjectInstanceData*)pVertices + n;

						ref var objectItem = ref batch.Objects.Data[ n ];
						ref var obj = ref owner.Objects[ objectItem.ObjectIndex ];

						ref var position = ref obj.Position;
						ref var rotation = ref obj.Rotation;
						ref var scale = ref obj.Scale;

						var scaleH = Math.Max( scale.X, scale.Y );

						var matrix = new Matrix4F();
						matrix.Item0.X = scaleH;
						matrix.Item0.Y = 0;
						matrix.Item0.Z = 0;
						matrix.Item0.W = 0;
						matrix.Item1.X = 0;
						matrix.Item1.Y = scaleH;
						matrix.Item1.Z = 0;
						matrix.Item1.W = 0;
						matrix.Item2.X = 0;
						matrix.Item2.Y = 0;
						matrix.Item2.Z = scale.Z;
						matrix.Item2.W = 0;
						//!!!!double. где еще
						matrix.Item3.X = (float)position.X;
						matrix.Item3.Y = (float)position.Y;
						matrix.Item3.Z = (float)position.Z;
						matrix.Item3.W = 1;

						matrix.GetTranslation( out var positionPreviousFrame );
						instanceData->Init( ref matrix, ref positionPreviousFrame, ref obj.Color, lodValue, visibilityDistance, receiveDecals );
					}
				}

				var vertexElements = new VertexElement[] {
					new VertexElement( 0, 0, VertexElementType.Float4, VertexElementSemantic.TextureCoordinate7 ),
					new VertexElement( 0, 16, VertexElementType.Float4, VertexElementSemantic.TextureCoordinate6 ),
					new VertexElement( 0, 32, VertexElementType.Float4, VertexElementSemantic.TextureCoordinate5 ) ,
					new VertexElement( 0, 48, VertexElementType.Float4, VertexElementSemantic.TextureCoordinate4 ),
					new VertexElement( 0, 64, VertexElementType.Float4, VertexElementSemantic.TextureCoordinate3 ) };

				var vertexDeclaration = vertexElements.CreateVertexDeclaration( 0 );

				batch.BatchingInstanceBufferBillboard = GpuBufferManager.CreateVertexBuffer( vertices, vertexDeclaration, 0 );
			}

			public unsafe void RenderingDataUpdate()
			{
				RenderingDataDestroy();

				foreach( var objectIndex in Objects )
				{
					ref var obj = ref owner.Objects[ objectIndex ];

					if( !obj.Flags.HasFlag( Object.FlagsEnum.Enabled ) )
						continue;
					if( !obj.Flags.HasFlag( Object.FlagsEnum.Visible ) )
						continue;

					owner.GetMesh( obj.Element, obj.VariationGroup, obj.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out _, out _, out _ );

					if( !enabled )
						continue;

					bool allowBatching = true;
					if( allowBatching && replaceMaterial != null && replaceMaterial.Result != null && replaceMaterial.Result.Transparent )
						allowBatching = false;
					if( allowBatching && !SupportsBatching( mesh ) )
						allowBatching = false;

					var group = GetGroup( !allowBatching, obj.Element, obj.VariationGroup, obj.VariationElement );
					if( group == null )
					{
						group = new Group();
						group.Owner = this;
						group.NoBatchingGroup = !allowBatching;
						group.Element = obj.Element;
						group.VariationGroup = obj.VariationGroup;
						group.VariationElement = obj.VariationElement;
						Groups.Add( group );
					}

					var objectItem = new Group.ObjectItem();
					objectItem.ObjectIndex = objectIndex;

					obj.Rotation.ToMatrix3( out var rot );
					Matrix3F.FromScale( ref obj.Scale, out var scl );
					Matrix3F.Multiply( ref rot, ref scl, out objectItem.MeshMatrix3 );

					var transform = new Transform( obj.Position, obj.Rotation, obj.Scale );
					//!!!!можно считать оптимальнее?
					var b = SpaceBounds.Multiply( transform, mesh.Result.SpaceBounds );
					objectItem.BoundingBox = b.CalculatedBoundingBox;
					objectItem.BoundingSphere = b.CalculatedBoundingSphere;

					group.Objects.Add( objectItem );
				}

				//split groups by max objects count
				if( owner.MaxObjectsInGroup > 1 )
				{
					bool needProcess = true;
					do
					{
						needProcess = false;

						var currentGroups = Groups;
						Groups = new List<Group>();

						foreach( var group in currentGroups )
						{
							if( group.Objects.Count > owner.MaxObjectsInGroup )
							{
								var objects = group.Objects.ToArray();

								CollectionUtility.MergeSort( objects, delegate ( Group.ObjectItem item1, Group.ObjectItem item2 )
								{
									ref var obj1 = ref owner.Objects[ item1.ObjectIndex ];
									ref var obj2 = ref owner.Objects[ item2.ObjectIndex ];

									var pos1 = obj1.Position[ group.NextSplitAxis ];
									var pos2 = obj2.Position[ group.NextSplitAxis ];

									if( pos1 < pos2 )
										return -1;
									if( pos1 > pos2 )
										return 1;
									return 0;

								}, true );

								int middle = objects.Length / 2;

								for( int nGroup = 0; nGroup < 2; nGroup++ )
								{
									var newGroup = new Group();
									newGroup.Owner = this;
									newGroup.NoBatchingGroup = group.NoBatchingGroup;
									newGroup.Element = group.Element;
									newGroup.VariationGroup = group.VariationGroup;
									newGroup.VariationElement = group.VariationElement;

									if( nGroup == 0 )
									{
										for( int n = 0; n < middle; n++ )
											newGroup.Objects.Add( objects[ n ] );
									}
									else
									{
										for( int n = middle; n < objects.Length; n++ )
											newGroup.Objects.Add( objects[ n ] );
									}

									newGroup.NextSplitAxis = group.NextSplitAxis + 1;
									//quadtree
									if( newGroup.NextSplitAxis > 1 )
										newGroup.NextSplitAxis = 0;

									Groups.Add( newGroup );
								}

								needProcess = true;
							}
							else
							{
								Groups.Add( group );
							}
						}

					} while( needProcess );
				}

				//calculate Bounds, VisibilityDistance
				foreach( var group in Groups )
				{
					var bounds = Bounds.Cleared;
					//var maxLocalObjectsBounds = Bounds.Cleared;
					float maxVisibilityDistance = float.MinValue;

					for( int nObject = 0; nObject < group.Objects.Count; nObject++ )
					{
						ref var objectItem = ref group.Objects.Data[ nObject ];
						ref var obj = ref owner.Objects[ objectItem.ObjectIndex ];

						if( !obj.Flags.HasFlag( Object.FlagsEnum.Enabled ) )
							continue;
						if( !obj.Flags.HasFlag( Object.FlagsEnum.Visible ) )
							continue;

						//!!!!не только меши
						owner.GetMesh( obj.Element, obj.VariationGroup, obj.VariationElement, out var enabled, out var mesh, out _, out var visibilityDistance, out _, out _ );

						if( !enabled )
							continue;

						bounds.Add( objectItem.BoundingBox );
						maxVisibilityDistance = Math.Max( maxVisibilityDistance, (float)visibilityDistance );
					}

					group.BoundingBox = bounds;
					group.BoundingSphere = group.BoundingBox.GetBoundingSphere();
					group.VisibilityDistance = maxVisibilityDistance;
					group.VisibilityDistanceSquared = group.VisibilityDistance * group.VisibilityDistance;
				}

				//sort by VisibilityDistance from far to nearest
				CollectionUtility.MergeSort( Groups, delegate ( Group group1, Group group2 )
				{
					if( group1.VisibilityDistance > group2.VisibilityDistance )
						return -1;
					if( group1.VisibilityDistance < group2.VisibilityDistance )
						return 1;
					return 0;
				}, true );

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
			//	public Component_Mesh mesh;
			//	//!!!!billboard
			//}

			////////////

			public class Element
			{
				public bool enabled;
				public VariationGroup[] variationGroups;

				//

				public class Variation
				{
					//mesh
					public Component_Mesh mesh;
					public Component_Material replaceMaterial;
					public double visibilityDistance;
					public bool castShadows;
					public bool receiveDecals;

					//!!!!другие типы объектов
				}

				public class VariationGroup
				{
					public Variation[] variations;

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

			public Element GetElement( int index )
			{
				if( index < elements.Length )
					return elements[ index ];
				return null;
			}

			public bool CheckNeedUpdate()
			{
				foreach( var element in elements )
					if( element != null && element.CheckNeedUpdate() )
						return true;
				return false;
			}
		}

		/////////////////////////////////////////

		public Component_GroupOfObjects()
		{
			_baseObjects = new ReferenceList<Component>( this, () => BaseObjectsChanged?.Invoke( this ) );

			ComponentsChanged += Component_GroupOfObjects_ComponentsChanged;

			unsafe
			{
				if( sizeof( Object ) != 132 )
					Log.Fatal( "Component_GroupOfObjects: Constructor: sizeof( ObjectMesh ) != 132." );
			}
		}

		private void Component_GroupOfObjects_ComponentsChanged( Component obj )
		{
			ElementTypesCacheNeedUpdate();
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

			if( EnabledInHierarchyAndIsNotResource )
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
				Objects = FromByteArray( Convert.FromBase64String( str ) );

				//for( int n = 0; n < Objects.Length; n++ )
				//	Objects[ n ].Flags = Object.FlagsEnum.Enabled | Object.FlagsEnum.Visible;

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

			if( Objects != null )
			{
				Object[] array;
				if( removedObjects != null )
					array = ObjectsGetData( ObjectsGetAll() );
				else
					array = Objects;
				block.SetAttribute( nameof( Objects ), Convert.ToBase64String( ToByteArray( array ), Base64FormattingOptions.None ) );
			}

			return true;
		}

		void ObjectAddToSectors( int index, ESet<Sector> sectorsToUpdate )
		{
			if( !EnabledInHierarchyAndIsNotResource )
				return;

			ref var obj = ref Objects[ index ];

			var sectorIndex = GetSectorIndexByPosition( ref obj.Position );
			var sector = GetSectorByIndex( ref sectorIndex, true );
			sector.Objects.Add( index );

			if( !sector.objectsBoundsNeedUpdateAfterRemove )
				sector.AddToObjectsBounds( ref obj );

			sectorsToUpdate.AddWithCheckAlreadyContained( sector );
		}

		void ObjectsRemoveFromSectors( int index, ESet<Sector> sectorsToUpdate )
		{
			if( !EnabledInHierarchyAndIsNotResource )
				return;

			ref var obj = ref Objects[ index ];

			var sectorIndex = GetSectorIndexByPosition( ref obj.Position );
			var sector = GetSectorByIndex( ref sectorIndex, false );
			if( sector != null )
			{
				sector.Objects.Remove( index );
				sector.objectsBoundsNeedUpdateAfterRemove = true;

				sectorsToUpdate.AddWithCheckAlreadyContained( sector );
			}
		}

		void UpdateSectors( ESet<Sector> sectorsToUpdate )
		{
			if( sectorsToUpdate == null )
				sectorsToUpdate = new ESet<Sector>( sectors.Values );

			foreach( var sector in sectorsToUpdate )
			{
				if( !sector.IsEmpty() )
					sector.Update();
				else
				{
					sector.Destroy();
					sectors.Remove( sector.Index );
				}
			}
		}

		public void CreateSectors()
		{
			if( !EnabledInHierarchyAndIsNotResource )
				return;

			DestroySectors();

			ElementTypesCacheNeedUpdate();

			var sectorsToUpdate = new ESet<Sector>();

			int capacity = ObjectsGetCapacity();
			for( int index = 0; index < capacity; index++ )
			{
				//check is removed
				if( removedObjects != null && removedObjects.Data[ index ] )
					continue;

				ObjectAddToSectors( index, sectorsToUpdate );
			}

			UpdateSectors( sectorsToUpdate );
		}

		void DestroySectors()
		{
			//!!!!дождаться трединга в секторах

			foreach( var sector in sectors.Values )
				sector.Destroy();
			sectors.Clear();
		}

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

		struct LocalItem
		{
			public int added;
			public float cameraDistanceMinSquared;
			public float cameraDistanceMin;
			public float cameraDistanceMaxSquared;
			public int onlyForShadowGeneration;
		}

		void RenderObjectsSeparately( ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem, Sector.Group group )
		{
			var cameraSettings = context.Owner.CameraSettings;

			var localItems = new LocalItem[ group.Objects.Count ];

			Parallel.For( 0, group.Objects.Count, delegate ( int nObject )
			{
				ref var objectItem = ref group.Objects.Data[ nObject ];

				//if( !obj.Flags.HasFlag( Object.FlagsEnum.Enabled ) )
				//	continue;
				//if( !obj.Flags.HasFlag( Object.FlagsEnum.Visible ) )
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

			for( int nObject = 0; nObject < group.Objects.Count; nObject++ )
			{
				ref var localItem = ref localItems[ nObject ];
				if( localItem.added == 0 )
					continue;
				var onlyForShadowGeneration = localItem.onlyForShadowGeneration != 0;

				ref var objectItem = ref group.Objects.Data[ nObject ];
				ref var obj = ref Objects[ objectItem.ObjectIndex ];

				GetMesh( obj.Element, obj.VariationGroup, obj.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out var visibilityDistance, out var castShadows, out var receiveDecals );

				if( !enabled )
					continue;
				if( onlyForShadowGeneration && !castShadows )
					continue;

				if( localItem.cameraDistanceMinSquared < visibilityDistance * visibilityDistance || mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum )
				{
					ref var position = ref obj.Position;
					ref var rotation = ref obj.Rotation;
					ref var scale = ref obj.Scale;

					var item = new Component_RenderingPipeline.RenderSceneData.MeshItem();
					item.Creator = this;
					item.BoundingSphere = objectItem.BoundingSphere;
					item.BoundingBoxCenter = item.BoundingSphere.Origin;//objectItem.BoundingBox.GetCenter( out item.BoundingBoxCenter );
					item.MeshData = mesh.Result.MeshData;
					item.CastShadows = castShadows;// && item.MeshData.CastShadows;
					item.ReceiveDecals = receiveDecals;
					item.ReplaceMaterial = replaceMaterial;
					//impl?
					//if( ReplaceMaterialSelectively.Count != 0 )
					//{
					//	item.ReplaceMaterialSelectively = new Component_Material[ ReplaceMaterialSelectively.Count ];
					//	for( int n = 0; n < ReplaceMaterialSelectively.Count; n++ )
					//		item.ReplaceMaterialSelectively[ n ] = ReplaceMaterialSelectively[ n ].Value;
					//}
					item.Color = obj.Color;
					item.VisibilityDistance = (float)visibilityDistance;
					item.OnlyForShadowGeneration = onlyForShadowGeneration;

					if( onlyForShadowGeneration && !item.CastShadows )
						continue;

					int item0BillboardMode = 0;

					SceneLODUtility.GetDemandedLODs( context, mesh, localItem.cameraDistanceMinSquared, localItem.cameraDistanceMaxSquared, out var lodState );
					for( int nLodItem = 0; nLodItem < lodState.Count; nLodItem++ )
					{
						lodState.GetItem( nLodItem, out var lodLevel, out var lodRange );


						item.MeshData = mesh.Result.MeshData;
						if( lodLevel > 0 )
						{
							ref var lod = ref mesh.Result.MeshData.LODs[ lodLevel - 1 ];
							var lodMeshData = lod.Mesh?.Result?.MeshData;
							if( lodMeshData != null )
								item.MeshData = lodMeshData;
						}

						item.CastShadows = castShadows && item.MeshData.CastShadows;
						item.LODValue = SceneLODUtility.GetLodValue( lodRange, localItem.cameraDistanceMin );
						//item.LODRange = lodRange;


						//calculate MeshInstanceOne, PositionPreviousFrame
						if( nLodItem == 0 )
							item0BillboardMode = item.MeshData.BillboardMode;
						if( nLodItem == 0 || item0BillboardMode != item.MeshData.BillboardMode )
						{
							if( item.MeshData.BillboardMode != 0 )
							{
								var scaleH = Math.Max( scale.X, scale.Y );

								ref var result = ref item.Transform;
								result.Item0.X = scaleH;
								result.Item0.Y = 0;
								result.Item0.Z = 0;
								result.Item0.W = 0;
								result.Item1.X = 0;
								result.Item1.Y = scaleH;
								result.Item1.Z = 0;
								result.Item1.W = 0;
								result.Item2.X = 0;
								result.Item2.Y = 0;
								result.Item2.Z = scale.Z;
								result.Item2.W = 0;
								//!!!!double. где еще
								result.Item3.X = (float)position.X;
								result.Item3.Y = (float)position.Y;
								result.Item3.Z = (float)position.Z;
								result.Item3.W = 1;
							}
							else
							{
								//!!!!double
								var pos = position.ToVector3F();
								item.Transform = new Matrix4F( ref objectItem.MeshMatrix3, ref pos );
							}

							//PositionPreviousFrame
							item.Transform.GetTranslation( out item.PositionPreviousFrame );
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

		unsafe void SectorGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem, Sector sector )
		{
			//hide label
			var context2 = context.objectInSpaceRenderingContext;
			context2.disableShowingLabelForThisObject = true;

			//update rendering data
			if( sector.RenderingDataNeedUpdate )
			{
				sector.RenderingDataNeedUpdate = false;
				sector.RenderingDataUpdate();
			}

			var cameraSettings = context.Owner.CameraSettings;

			int groupsCountToCheck = 0;
			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var cameraDistanceToSectorSquared = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, ref sector.ObjectsBoundingBox );
				for( int nGroup = 0; nGroup < sector.Groups.Count; nGroup++ )
				{
					var group = sector.Groups[ nGroup ];
					if( cameraDistanceToSectorSquared > group.VisibilityDistanceSquared )
						break;
					groupsCountToCheck++;
				}
			}
			else if( mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum )
				groupsCountToCheck = sector.Groups.Count;

			var groupsAdded = stackalloc int/*bool*/[ groupsCountToCheck ];
			for( int nGroup = 0; nGroup < groupsCountToCheck; nGroup++ )
				groupsAdded[ nGroup ] = 0;
			var groupsCameraDistanceMinSquared = stackalloc float[ groupsCountToCheck ];
			var groupsOnlyForShadowGeneration = stackalloc int[ groupsCountToCheck ];
			Parallel.For( 0, groupsCountToCheck, delegate ( int nGroup )
			{
				var group = sector.Groups[ nGroup ];

				var cameraDistanceMinSquared = SceneLODUtility.GetCameraDistanceMinSquared( cameraSettings, ref group.BoundingBox );

				if( cameraDistanceMinSquared < group.VisibilityDistanceSquared )//|| mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum )
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
						groupsAdded[ nGroup ] = 1;
						groupsCameraDistanceMinSquared[ nGroup ] = cameraDistanceMinSquared;
						groupsOnlyForShadowGeneration[ nGroup ] = onlyForShadowGeneration ? 1 : 0;
					}
				}
			} );

			for( int nGroup = 0; nGroup < groupsCountToCheck; nGroup++ )
			{
				if( groupsAdded[ nGroup ] == 0 )
					continue;

				var group = sector.Groups[ nGroup ];
				var cameraDistanceMinSquared = groupsCameraDistanceMinSquared[ nGroup ];
				var onlyForShadowGeneration = groupsOnlyForShadowGeneration[ nGroup ] != 0;

				if( group.NoBatchingGroup )
				{
					RenderObjectsSeparately( context, mode, modeGetObjectsItem, group );
				}
				else
				{
					//if( !obj.Flags.HasFlag( Object.FlagsEnum.Enabled ) )
					//	continue;
					//if( !obj.Flags.HasFlag( Object.FlagsEnum.Visible ) )
					//	continue;

					GetMesh( group.Element, group.VariationGroup, group.VariationElement, out var enabled, out var mesh, out var replaceMaterial, out var visibilityDistance, out var castShadows, out var receiveDecals );

					if( !enabled )
						continue;
					if( onlyForShadowGeneration && !castShadows )
						continue;

					var cameraDistanceMaxSquared = SceneLODUtility.GetCameraDistanceMax( cameraSettings, ref group.BoundingSphere );
					cameraDistanceMaxSquared *= cameraDistanceMaxSquared;

					SceneLODUtility.GetDemandedLODs( context, mesh, cameraDistanceMinSquared, cameraDistanceMaxSquared, out var lodState );

					var lods = mesh.Result.MeshData.LODs;

					bool useBatching = false;
					{
						if( lods == null )
							useBatching = true;
						else
						{
							//check for only one last lod
							if( lodState.Count == 1 )
							{
								var lodCount = lods.Length + 1;
								lodState.GetItem( 0, out var level, out var range );

								if( level == lodCount - 1 )
									useBatching = true;
							}
						}
					}

					if( useBatching )
					{
						var item = new Component_RenderingPipeline.RenderSceneData.MeshItem();
						item.Creator = this;
						item.BoundingSphere = group.BoundingSphere;
						item.BoundingBoxCenter = item.BoundingSphere.Origin;
						//group.Bounds.CalculatedBoundingBox.GetCenter( out item.BoundingBoxCenter );

						//item.MeshData = mesh.Result.MeshData;
						//item.CastShadows = castShadows && item.MeshData.CastShadows;
						item.ReceiveDecals = receiveDecals;
						item.ReplaceMaterial = replaceMaterial;
						//impl?
						//if( ReplaceMaterialSelectively.Count != 0 )
						//{
						//	item.ReplaceMaterialSelectively = new Component_Material[ ReplaceMaterialSelectively.Count ];
						//	for( int n = 0; n < ReplaceMaterialSelectively.Count; n++ )
						//		item.ReplaceMaterialSelectively[ n ] = ReplaceMaterialSelectively[ n ].Value;
						//}
						item.Color = ColorValue.One;
						item.VisibilityDistance = (float)visibilityDistance;
						item.OnlyForShadowGeneration = onlyForShadowGeneration;


						for( int nLodItem = 0; nLodItem < lodState.Count; nLodItem++ )
						{
							lodState.GetItem( nLodItem, out var lodLevel, out _ );


							item.MeshData = mesh.Result.MeshData;
							if( lodLevel > 0 )
							{
								ref var lod = ref lods[ lodLevel - 1 ];
								var lodMeshData = lod.Mesh?.Result?.MeshData;
								if( lodMeshData != null )
									item.MeshData = lodMeshData;
							}

							item.CastShadows = castShadows && item.MeshData.CastShadows;
							item.LODValue = 0;
							// = SceneLODUtility.GetLodValue( lodRange, objectCameraDistance );
							//item.LODRange = lodRange;

							if( onlyForShadowGeneration && !item.CastShadows )
								continue;

							//set BatchingInstanceBuffer
							if( item.MeshData.BillboardMode != 0 )
							{
								if( group.BatchingInstanceBufferBillboard == null )
									sector.CreateBatchingInstanceBufferBillboard( group, item.LODValue, item.VisibilityDistance, item.ReceiveDecals );
								item.BatchingInstanceBuffer = group.BatchingInstanceBufferBillboard;
							}
							else
							{
								if( group.BatchingInstanceBufferMesh == null )
									sector.CreateBatchingInstanceBufferMesh( group, item.LODValue, item.VisibilityDistance, item.ReceiveDecals );
								item.BatchingInstanceBuffer = group.BatchingInstanceBufferMesh;
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
						RenderObjectsSeparately( context, mode, modeGetObjectsItem, group );
					}
				}
			}

			//visualize groups
			if( DrawGroupBounds )
			{
				var renderer = context.Owner.Simple3DRenderer;
				renderer.SetColor( new ColorValue( 0, 0, 1 ) );

				foreach( var group in sector.Groups )
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

			//					viewport.Simple3DRenderer.AddMesh( Component_Billboard.GetBillboardMesh().Result, worldMatrix, false, false );
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

			for( int n = 0; n < count; n++ )
			{
				var index = freeObjects.Pop();

				ref var obj = ref Objects[ index ];
				obj = data[ n ];
				if( obj.UniqueIdentifier == 0 )
					obj.UniqueIdentifier = GetUniqueIdentifier();

				removedObjects.Data[ index ] = false;

				ObjectAddToSectors( index, sectorsToUpdate );

				addedIndexes[ n ] = index;
			}

			UpdateSectors( sectorsToUpdate );

			return addedIndexes;
		}

		public int[] ObjectsAdd( ArraySegment<Object> data )
		{
			unsafe
			{
				fixed( Object* pData = data.Array )
					return ObjectsAdd( pData + data.Offset, data.Count );
			}
		}

		public int[] ObjectsAdd( Object[] data )
		{
			unsafe
			{
				fixed( Object* pData = data )
					return ObjectsAdd( pData, data.Length );
			}
		}

		public unsafe void ObjectsRemove( int* indexes, int count )
		{
			RemovedObjectsInit();

			var sectorsToUpdate = new ESet<Sector>();

			for( int n = count - 1; n >= 0; n-- )
			{
				var index = indexes[ n ];
				freeObjects.Push( index );
				removedObjects.Data[ index ] = true;

				ObjectsRemoveFromSectors( index, sectorsToUpdate );
			}

			UpdateSectors( sectorsToUpdate );
		}

		public void ObjectsRemove( ArraySegment<int> indexes )
		{
			unsafe
			{
				fixed( int* pData = indexes.Array )
					ObjectsRemove( pData + indexes.Offset, indexes.Count );
			}
		}

		public void ObjectsRemove( int[] indexes )
		{
			unsafe
			{
				fixed( int* pData = indexes )
					ObjectsRemove( pData, indexes.Length );
			}
		}

		int ObjectsGetCapacity()
		{
			return Objects != null ? Objects.Length : 0;
		}

		int ObjectsGetCount()
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
					Log.Fatal( "Component_GroupOfObjects: ObjectsRemove: data[ n ].UniqueIdentifier == 0." );
				set.Add( data[ n ].UniqueIdentifier );
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

			var elements = GetComponents<Component_GroupOfObjectsElement>();

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
				var elementMesh = element as Component_GroupOfObjectsElement_Mesh;
				if( elementMesh != null )
				{
					elementData.variationGroups = new ElementTypesCache.Element.VariationGroup[ 1 ];

					var group = new ElementTypesCache.Element.VariationGroup();
					{
						var variation = new ElementTypesCache.Element.Variation();
						variation.mesh = elementMesh.Mesh.Value;
						variation.replaceMaterial = elementMesh.ReplaceMaterial;
						variation.visibilityDistance = elementMesh.VisibilityDistance;
						variation.castShadows = elementMesh.CastShadows;
						variation.receiveDecals = elementMesh.ReceiveDecals;

						group.variations = new ElementTypesCache.Element.Variation[] { variation };
					}
					elementData.variationGroups[ 0 ] = group;
				}

				//surface
				var elementSurface = element as Component_GroupOfObjectsElement_Surface;
				if( elementSurface != null )
				{
					var surface = elementSurface.Surface.Value;
					if( surface != null )
					{
						var surfaceGroups = surface.GetComponents<Component_SurfaceGroupOfElements>();

						elementData.variationGroups = new ElementTypesCache.Element.VariationGroup[ surfaceGroups.Length ];

						for( int nGroup = 0; nGroup < surfaceGroups.Length; nGroup++ )
						{
							var surfaceGroup = surfaceGroups[ nGroup ];
							var surfaceElements = surfaceGroup.GetComponents<Component_SurfaceElement>();

							var group = new ElementTypesCache.Element.VariationGroup();
							group.variations = new ElementTypesCache.Element.Variation[ surfaceElements.Length ];

							for( int nVariation = 0; nVariation < group.variations.Length; nVariation++ )
							{
								var surfaceElement = surfaceElements[ nVariation ];

								var variation = new ElementTypesCache.Element.Variation();

								//mesh
								var surfaceElementMesh = surfaceElement as Component_SurfaceElement_Mesh;
								if( surfaceElementMesh != null )
								{
									variation.mesh = surfaceElementMesh.Mesh;
									variation.replaceMaterial = surfaceElementMesh.ReplaceMaterial;
									variation.visibilityDistance = surfaceElementMesh.VisibilityDistance;
									variation.castShadows = surfaceElementMesh.CastShadows;
									variation.receiveDecals = surfaceElementMesh.ReceiveDecals;
								}

								group.variations[ nVariation ] = variation;
							}

							elementData.variationGroups[ nGroup ] = group;
						}

						//element.variationMeshes = new Component_Mesh[ surfaceElements.Length ];
						//for( int n = 0; n < surfaceElements.Length; n++ )
						//{
						//	var brushElement = surfaceElements[ n ];

						//	var brushElementMesh = brushElement as Component_SurfaceElement_Mesh;
						//	if( brushElementMesh != null )
						//		element.variationMeshes[ n ] = brushElementMesh.Mesh;
						//}


						//var surfaceElements = surface.GetComponents<Component_SurfaceElement>();

						//element.variationMeshes = new Component_Mesh[ surfaceElements.Length ];
						//for( int n = 0; n < surfaceElements.Length; n++ )
						//{
						//	var brushElement = surfaceElements[ n ];

						//	var brushElementMesh = brushElement as Component_SurfaceElement_Mesh;
						//	if( brushElementMesh != null )
						//		element.variationMeshes[ n ] = brushElementMesh.Mesh;
						//}
					}
				}
			}

			//!!!!по идее секторы тоже обновить, т.к. bounds
		}

		void GetMesh( ushort elementIndex, byte variationGroup, byte variationElement, out bool enabled, out Component_Mesh mesh, out Component_Material replaceMaterial, out double visibilityDistance, out bool castShadows, out bool receiveDecals )
		{
			enabled = false;
			mesh = null;
			replaceMaterial = null;
			visibilityDistance = 10000;
			castShadows = false;
			receiveDecals = false;

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
						visibilityDistance = variation.visibilityDistance;
						castShadows = variation.castShadows;
						receiveDecals = variation.receiveDecals;
					}
				}
			}

			if( mesh == null || mesh.Result == null )
				mesh = ResourceUtility.MeshInvalid;

			var meshData = mesh.Result.MeshData;
			visibilityDistance = Math.Min( visibilityDistance, meshData.VisibilityDistance );
			castShadows = castShadows && meshData.CastShadows;
		}

		public void ElementTypesCacheNeedUpdate()
		{
			elementTypesCacheNeedUpdate = true;
		}

		public Bounds CalculateTotalBounds()
		{
			Bounds result = Bounds.Cleared;
			foreach( var sector in sectors.Values )
			{
				if( sector.ObjectsBounds != null )
					result.Add( sector.ObjectsBounds.CalculatedBoundingBox );
			}
			return result;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			ElementTypesCacheCheckNeedUpdate();
		}

		private void Scene_GetRenderSceneData( Component_Scene scene, ViewportRenderingContext context )
		{
			var context2 = context.objectInSpaceRenderingContext;
			if( context2.selectedObjects.Contains( this ) )
			{
				var bounds = CalculateTotalBounds();
				if( !bounds.IsCleared() )
				{
					if( context.Owner.Simple3DRenderer != null )
					{
						ColorValue color = ProjectSettings.Get.SelectedColor;
						context.Owner.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
						context.Owner.Simple3DRenderer.AddBounds( bounds );
					}
				}
			}
		}

		public Component_GroupOfObjectsElement GetElement( int elementIndex )
		{
			//!!!!slowly? кешировать?

			foreach( var element in GetComponents<Component_GroupOfObjectsElement>() )
			{
				if( element.Index == elementIndex )
					return element;
			}
			return null;
		}

		//public int GetElementIndex( Component_GroupOfObjectsElement element )
		//{
		//	//!!!!slowly?
		//	var elements = GetComponents<Component_GroupOfObjectsElement>();
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
			foreach( var element in GetComponents<Component_GroupOfObjectsElement>() )
				maxIndex = Math.Max( maxIndex, element.Index );
			return maxIndex + 1;
		}

		//public void ResetLodTransitionStates( ViewportRenderingContext resetOnlySpecifiedContext = null )
		//{
		//	foreach( var sector in sectors.Values )
		//		foreach( var batch in sector.Batches )
		//			SceneLODUtility.ResetLodTransitionStates( ref batch.renderingContextItems, resetOnlySpecifiedContext );
		//}
	}
}
