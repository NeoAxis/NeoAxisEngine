// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// An instance of building.
	/// </summary>
	[ResourceFileExtension( "building" )]
	[AddToResourcesWindow( @"Addons\Building\Building", 310 )]
#if !DEPLOY
	[SettingsCell( typeof( BuildingSettingsCell ) )]
	//[EditorControl( typeof( Editor.BuildingEditor ), true )]
	//[Preview( typeof( Editor.BuildingPreview ) )]
	//[PreviewImage( typeof( Editor.BuildingPreviewImage ) )]
#endif
	public class Building : ObjectInSpace
	{
		LogicalData logicalData;
		VisualData visualData;
		bool needUpdate;
		bool needUpdateAfterEndModifyingTransformTool;

		Transform occluderCachedTransform;
		Vector3[] occluderCachedBoxShapeVertices;
		int[] occluderCachedBoxShapeIndices;

		//

		/// <summary>
		/// The type of the building.
		/// </summary>
		[DefaultValue( null )]
		public Reference<BuildingType> BuildingType
		{
			get { if( _buildingType.BeginGet() ) BuildingType = _buildingType.Get( this ); return _buildingType.value; }
			set { if( _buildingType.BeginSet( this, ref value ) ) { try { BuildingTypeChanged?.Invoke( this ); NeedUpdate(); } finally { _buildingType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BuildingType"/> property value changes.</summary>
		public event Action<Building> BuildingTypeChanged;
		ReferenceField<BuildingType> _buildingType = null;

		/// <summary>
		/// The size of the building in cells.
		/// </summary>
		[DefaultValue( "8 3 4" )]
		public Reference<Vector3I> Size
		{
			get { if( _size.BeginGet() ) Size = _size.Get( this ); return _size.value; }
			set { if( _size.BeginSet( this, ref value ) ) { try { SizeChanged?.Invoke( this ); NeedUpdate(); } finally { _size.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Size"/> property value changes.</summary>
		public event Action<Building> SizeChanged;
		ReferenceField<Vector3I> _size = new Vector3I( 8, 3, 4 );

		public enum CollisionModeEnum
		{
			None,
			Simple,
		}

		/// <summary>
		/// The collision mode of the building.
		/// </summary>
		[DefaultValue( CollisionModeEnum.Simple )]
		public Reference<CollisionModeEnum> CollisionMode
		{
			get { if( _collisionMode.BeginGet() ) CollisionMode = _collisionMode.Get( this ); return _collisionMode.value; }
			set { if( _collisionMode.BeginSet( this, ref value ) ) { try { CollisionModeChanged?.Invoke( this ); NeedUpdate(); } finally { _collisionMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CollisionMode"/> property value changes.</summary>
		public event Action<Building> CollisionModeChanged;
		ReferenceField<CollisionModeEnum> _collisionMode = CollisionModeEnum.Simple;

		/// <summary>
		/// Whether to object should be used as an occluder for occlusion culling.
		/// </summary>
		[DefaultValue( false )]
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

						occluderCachedTransform = null;
						if( EnabledInHierarchy )
							ParentScene?.ObjectsInSpace_ObjectUpdateSceneObjectFlags( this );
					}
					finally { _occluder.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Occluder"/> property value changes.</summary>
		public event Action<Building> OccluderChanged;
		ReferenceField<bool> _occluder = false;

		///// <summary>
		///// The additional height of the occluder at bottom.
		///// </summary>
		//[DefaultValue( 4 )]
		//[Range( 0, 10 )]
		//public Reference<double> OccluderExtendBottom
		//{
		//	get { if( _occluderExtendBottom.BeginGet() ) OccluderExtendBottom = _occluderExtendBottom.Get( this ); return _occluderExtendBottom.value; }
		//	set
		//	{
		//		if( _occluderExtendBottom.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				OccluderExtendBottomChanged?.Invoke( this );
		//				occluderCachedTransform = null;
		//			}
		//			finally { _occluderExtendBottom.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="OccluderExtendBottom"/> property value changes.</summary>
		//public event Action<Building> OccluderExtendBottomChanged;
		//ReferenceField<double> _occluderExtendBottom = 4;

		/////// <summary>
		/////// The size of extending of the occluder.
		/////// </summary>
		////[DefaultValue( "-1 -1 4 -1 -1 0" )]
		////public Reference<Bounds> OccluderExtend
		////{
		////	get { if( _occluderExtend.BeginGet() ) OccluderExtend = _occluderExtend.Get( this ); return _occluderExtend.value; }
		////	set
		////	{
		////		if( _occluderExtend.BeginSet( this, ref value ) )
		////		{
		////			try
		////			{
		////				OccluderExtendChanged?.Invoke( this );
		////				occluderCachedTransform = null;
		////			}
		////			finally { _occluderExtend.EndSet(); }
		////		}
		////	}
		////}
		/////// <summary>Occurs when the <see cref="OccluderExtend"/> property value changes.</summary>
		////public event Action<Building> OccluderExtendChanged;
		////ReferenceField<Bounds> _occluderExtend = new Bounds( -1, -1, 4, -1, -1, 0 );

		/// <summary>
		/// Whether to cull visibility by the camera direction.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> CullingByCameraDirection
		{
			get { if( _cullingByCameraDirection.BeginGet() ) CullingByCameraDirection = _cullingByCameraDirection.Get( this ); return _cullingByCameraDirection.value; }
			set { if( _cullingByCameraDirection.BeginSet( this, ref value ) ) { try { CullingByCameraDirectionChanged?.Invoke( this ); NeedUpdate(); } finally { _cullingByCameraDirection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CullingByCameraDirection"/> property value changes.</summary>
		public event Action<Building> CullingByCameraDirectionChanged;
		ReferenceField<bool> _cullingByCameraDirection = true;

		/// <summary>
		/// The random seed number for the building generator.
		/// </summary>
		[DefaultValue( 0 )]
		[Range( 0, 100 )]
		public Reference<int> Seed
		{
			get { if( _seed.BeginGet() ) Seed = _seed.Get( this ); return _seed.value; }
			set { if( _seed.BeginSet( this, ref value ) ) { try { SeedChanged?.Invoke( this ); NeedUpdate(); } finally { _seed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Seed"/> property value changes.</summary>
		public event Action<Building> SeedChanged;
		ReferenceField<int> _seed = 0;

		///////////////////////////////////////////////

		public class LogicalData
		{
			public Building Owner;
			public BuildingType BuildingType;
			public int BuildingTypeUsedVersion;

			public BuildingElement[] UsedElements;
			public RenderingItem[] RenderingItems;
			public CollisionMeshItem[] CollisionMeshItems;

			//!!!!new
			public LevelInfo[] Levels;//public double[] LevelHeights;
			public double TotalHeight;

			public List<Scene.PhysicsWorldClass.Body> CollisionBodies = new List<Scene.PhysicsWorldClass.Body>();

			/////////////////////

			public struct RenderingItem
			{
				public BuildingElement Element;
				//public int ElementIndex;

				public Vector3 Position;
				public QuaternionF Rotation;
				public Vector3F Scale;
				//public Matrix4F Transform;

				public uint CullingByCameraDirectionData;
				//public Vector4F CullingByCameraDirectionData;

				//!!!!additional parameters. GroupOfObjects.ObjectItem
			}

			/////////////////////

			public struct CollisionMeshItem
			{
				public Mesh Mesh;
				public Transform Transform;
			}

			/////////////////////

			public struct LevelInfo
			{
				public double Height;
				public double HeightFromBottom;
			}

			/////////////////////

			public void UpdateCollisionBodies()
			{
				DestroyCollisionBodies();

				if( Owner.CollisionMode.Value == CollisionModeEnum.None )
					return;
				if( EditorAPI.IsAnyTransformToolInModifyingMode() )
				{
					Owner.needUpdateAfterEndModifyingTransformTool = true;
					return;
				}

				var manager = Owner.ParentScene?.GetComponent<BuildingManager>();
				if( manager == null || manager.Collision )
					Owner.OnCalculateCollisionBodies();
			}

			//public void UpdateCollisionMaterial()
			//{
			//	if( CollisionBody != null )
			//	{
			//		CollisionBody.Material = PipeType.CollisionMaterial;
			//		CollisionBody.MaterialFrictionMode = PipeType.CollisionFrictionMode;
			//		CollisionBody.MaterialFriction = PipeType.CollisionFriction;
			//		CollisionBody.MaterialAnisotropicFriction = PipeType.CollisionAnisotropicFriction;
			//		CollisionBody.MaterialSpinningFriction = PipeType.CollisionSpinningFriction;
			//		CollisionBody.MaterialRollingFriction = PipeType.CollisionRollingFriction;
			//		CollisionBody.MaterialRestitution = PipeType.CollisionRestitution;
			//	}
			//}

			public void DestroyCollisionBodies()
			{
				if( CollisionBodies.Count != 0 )
				{
					foreach( var body in CollisionBodies )
						body.Dispose();
					CollisionBodies.Clear();
				}
			}

			public void GetCellsBox( out Box box, out Quaternion andRotation )
			{
				var ownerTransform = Owner.TransformV;
				var elementSize = BuildingType.ElementSize.Value;
				var elementSizeH = elementSize.X;
				//var elementSizeV = elementSize.Y;
				var size = Owner.Size.Value;
				var totalSize = new Vector3( size.X * elementSizeH, size.Y * elementSizeH, TotalHeight );
				//var totalSize = size.ToVector3() * new Vector3( elementSizeH, elementSizeH, elementSizeV );
				var totalSizeHalf = totalSize * 0.5;

				var bounds = new Bounds( -totalSizeHalf.X, -totalSizeHalf.Y, 0, totalSizeHalf.X, totalSizeHalf.Y, totalSize.Z );

				var position = ownerTransform.Position;
				ownerTransform.Rotation.ToMatrix3( out var matrix3 );

				box = new Box( ref bounds, ref position, ref matrix3 );
				andRotation = ownerTransform.Rotation;
			}

			public double GetLevelHeight( int level )
			{
				var level2 = MathEx.Clamp( level, 0, Levels.Length - 1 );
				return Levels[ level2 ].Height;
			}

			public double GetLevelHeightFromBottom( int level )
			{
				var level2 = MathEx.Clamp( level, 0, Levels.Length - 1 );
				var result = Levels[ level2 ].HeightFromBottom;
				//for roof
				if( level == Levels.Length )
					result += Levels[ level2 ].Height;
				return result;
			}
		}

		///////////////////////////////////////////////

		public class VisualData
		{
			public GroupOfObjects groupOfObjects;
			public List<GroupOfObjects.SubGroup> groupOfObjectSubGroups = new List<GroupOfObjects.SubGroup>();

			//public OpenList<GroupOfObjects.SubGroup> groupOfObjectSubGroups = new OpenList<GroupOfObjects.SubGroup>();
			//public OpenList<int> groupOfObjectObjects = new OpenList<int>();
		}

		///////////////////////////////////////////////

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			if( Components.Count == 0 && !BuildingType.ReferenceSpecified )
			{
				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Side 1 Floor Window";
					element.ElementType = BuildingElement.ElementTypeEnum.Side;
					element.Levels = new RangeI( 0, 0 );
					element.Sides = BuildingElement.SidesEnum.MinusY | BuildingElement.SidesEnum.PlusY;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Window level 1.obj|$Mesh" );
				}

				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Side 1 Floor Wall";
					element.ElementType = BuildingElement.ElementTypeEnum.Side;
					element.Sides = BuildingElement.SidesEnum.MinusX | BuildingElement.SidesEnum.PlusX;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Wall.obj|$Mesh" );
				}

				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Side 2+ Floor Window";
					element.ElementType = BuildingElement.ElementTypeEnum.Side;
					element.Levels = new RangeI( 1, 10000 );
					element.Sides = BuildingElement.SidesEnum.MinusY | BuildingElement.SidesEnum.PlusY;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Window level 2+.obj|$Mesh" );
				}

				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Roof";
					element.ElementType = BuildingElement.ElementTypeEnum.Roof;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Roof.obj|$Mesh" );
				}

				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Roof Edge";
					element.ElementType = BuildingElement.ElementTypeEnum.RoofEdge;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Roof side.obj|$Mesh" );
				}

				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Roof Corner";
					element.ElementType = BuildingElement.ElementTypeEnum.RoofCorner;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Roof corner.obj|$Mesh" );
				}

				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Cell Level 1 1";
					element.ElementType = BuildingElement.ElementTypeEnum.Cell;
					element.Levels = new RangeI( 0, 0 );
					element.Sides = BuildingElement.SidesEnum.MinusY | BuildingElement.SidesEnum.PlusY;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Room 2.obj|$Mesh" );
				}

				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Cell Level 1 2";
					element.ElementType = BuildingElement.ElementTypeEnum.Cell;
					element.Levels = new RangeI( 0, 0 );
					element.Sides = BuildingElement.SidesEnum.MinusY | BuildingElement.SidesEnum.PlusY;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Room 3.obj|$Mesh" );
				}

				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Cell Level 1 3";
					element.ElementType = BuildingElement.ElementTypeEnum.Cell;
					element.Levels = new RangeI( 0, 0 );
					element.Sides = BuildingElement.SidesEnum.PlusXMinusY | BuildingElement.SidesEnum.MinusXPlusY;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Room 2.obj|$Mesh" );
					element.RotationOffset = Quaternion.FromRotateByZ( Math.PI / 2 );
				}

				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Cell Level 2+ 1";
					element.ElementType = BuildingElement.ElementTypeEnum.Cell;
					element.Levels = new RangeI( 1, 10000 );
					element.Sides = BuildingElement.SidesEnum.MinusY | BuildingElement.SidesEnum.PlusY;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Room 4.obj|$Mesh" );
				}

				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Cell Level 2+ 2";
					element.ElementType = BuildingElement.ElementTypeEnum.Cell;
					element.Levels = new RangeI( 1, 10000 );
					element.Sides = BuildingElement.SidesEnum.MinusY | BuildingElement.SidesEnum.PlusY;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Room 2.obj|$Mesh" );
					element.PositionOffset = new Vector3( 0, 0, 0.4 );
				}

				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Cell Level 2+ 3";
					element.ElementType = BuildingElement.ElementTypeEnum.Cell;
					element.Levels = new RangeI( 1, 10000 );
					element.Sides = BuildingElement.SidesEnum.MinusY | BuildingElement.SidesEnum.PlusY;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Room 3.obj|$Mesh" );
					element.PositionOffset = new Vector3( 0, 0, 0.4 );
				}

				{
					var element = CreateComponent<BuildingElement>();
					element.Name = "Element Cell Level 2+ 4";
					element.ElementType = BuildingElement.ElementTypeEnum.Cell;
					element.Levels = new RangeI( 1, 10000 );
					element.Sides = BuildingElement.SidesEnum.PlusXMinusY | BuildingElement.SidesEnum.MinusXPlusY;
					element.Mesh = new Reference<Mesh>( null, "Content\\Constructors\\Buildings\\Modern building\\Data\\Room 3.obj|$Mesh" );
					element.PositionOffset = new Vector3( 0, 0, 0.4 );
					element.RotationOffset = Quaternion.FromRotateByZ( Math.PI / 2 );
				}
			}
		}

		public delegate void CalculateLogicalDataBeforeDelegate( Building sender, LogicalData logicalData, ref bool handled );
		public event CalculateLogicalDataBeforeDelegate CalculateLogicalDataBefore;

		public delegate void CalculateLogicalDataAfterDelegate( Building sender, LogicalData logicalData );
		public event CalculateLogicalDataAfterDelegate CalculateLogicalDataAfter;

		enum ElementSideNotMask
		{
			MinusX,
			MinusY,
			PlusX,
			PlusY,
			MinusXMinusY,
			PlusXMinusY,
			MinusXPlusY,
			PlusXPlusY,
		}

		static int GetSideMask( ElementSideNotMask side )
		{
			switch( side )
			{
			case ElementSideNotMask.MinusX: return (int)BuildingElement.SidesEnum.MinusX;
			case ElementSideNotMask.MinusY: return (int)BuildingElement.SidesEnum.MinusY;
			case ElementSideNotMask.PlusX: return (int)BuildingElement.SidesEnum.PlusX;
			case ElementSideNotMask.PlusY: return (int)BuildingElement.SidesEnum.PlusY;

			case ElementSideNotMask.MinusXMinusY: return (int)BuildingElement.SidesEnum.MinusXMinusY;
			case ElementSideNotMask.PlusXMinusY: return (int)BuildingElement.SidesEnum.PlusXMinusY;
			case ElementSideNotMask.MinusXPlusY: return (int)BuildingElement.SidesEnum.MinusXPlusY;
			case ElementSideNotMask.PlusXPlusY: return (int)BuildingElement.SidesEnum.PlusXPlusY;
			}
			return 0;
		}

		//static int GetSideMask( int side )
		//{
		//	switch( side )
		//	{
		//	case 0: return (int)BuildingElement.SidesEnum.MinusX;
		//	case 1: return (int)BuildingElement.SidesEnum.MinusY;
		//	case 2: return (int)BuildingElement.SidesEnum.PlusX;
		//	case 3: return (int)BuildingElement.SidesEnum.PlusY;

		//	case 4: return (int)BuildingElement.SidesEnum.MinusXMinusY;
		//	case 5: return (int)BuildingElement.SidesEnum.PlusXMinusY;
		//	case 6: return (int)BuildingElement.SidesEnum.MinusXPlusY;
		//	case 7: return (int)BuildingElement.SidesEnum.PlusXPlusY;
		//	}
		//	return 0;
		//}

		static BuildingElement SelectElementByProbability( FastRandom random, IList<BuildingElement> elements )
		{
			if( elements.Count > 0 )
			{
				if( elements.Count > 1 )
				{
					unsafe
					{
						var groupProbabilities = stackalloc double[ elements.Count ];
						for( int n = 0; n < elements.Count; n++ )
							groupProbabilities[ n ] = elements[ n ].Probability;
						var index = RandomUtility.GetRandomIndexByProbabilities( random, groupProbabilities, elements.Count );

						return elements[ index ];
					}
				}
				else
					return elements[ 0 ];
			}
			return null;
		}

		static BuildingElement GetSuitableSideElement( FastRandom random, BuildingElement[] sideElements/*, BuildingModifier[] sideModifiers*/, ElementSideNotMask side, Vector2I position )
		{
			int sideMask = GetSideMask( side );

			//foreach( var modifier in sideModifiers )
			//{
			//	if( modifier.Position.Value == position )
			//	{
			//		var element2 = modifier.Element.Value;
			//		if( element2 != null )
			//			return element2;
			//	}
			//}

			var elements = new List<BuildingElement>( sideElements.Length );

			foreach( var element in sideElements )
			{
				var levels = element.Levels.Value;
				if( position.Y >= levels.Minimum && position.Y <= levels.Maximum && ( (int)element.Sides.Value & sideMask ) != 0 )
				{
					elements.Add( element );
				}
			}

			return SelectElementByProbability( random, elements );
		}

		static BuildingElement GetSuitableSideEdgeElement( FastRandom random, BuildingElement[] sideEdgeElements/*, BuildingModifier[] sideEdgeModifiers*/, ElementSideNotMask side, int level )
		{
			int sideMask = GetSideMask( side );

			var elements = new List<BuildingElement>( sideEdgeElements.Length );

			foreach( var element in sideEdgeElements )
			{
				var levels = element.Levels.Value;
				if( level >= levels.Minimum && level <= levels.Maximum && ( (int)element.Sides.Value & sideMask ) != 0 )
				{
					elements.Add( element );
				}
			}

			return SelectElementByProbability( random, elements );
		}

		static BuildingElement GetSuitableRoofEdgeElement( FastRandom random, BuildingElement[] roofEdgeElements/*, BuildingModifier[] roofEdgeModifiers*/ , ElementSideNotMask side, int position )
		{
			int sideMask = GetSideMask( side );

			var elements = new List<BuildingElement>( roofEdgeElements.Length );

			foreach( var element in roofEdgeElements )
			{
				//var levels = element.Levels.Value;
				if( /*level >= levels.Minimum && level <= levels.Maximum &&*/ ( (int)element.Sides.Value & sideMask ) != 0 )
				{
					elements.Add( element );
				}
			}

			return SelectElementByProbability( random, elements );
		}

		static BuildingElement GetSuitableRoofCornerElement( FastRandom random, BuildingElement[] roofCornerElements/*, BuildingModifier[] roofCornerModifiers*/ , ElementSideNotMask side )
		{
			int sideMask = GetSideMask( side );

			var elements = new List<BuildingElement>( roofCornerElements.Length );

			foreach( var element in roofCornerElements )
			{
				//var levels = element.Levels.Value;
				if( /*level >= levels.Minimum && level <= levels.Maximum &&*/ ( (int)element.Sides.Value & sideMask ) != 0 )
				{
					elements.Add( element );
				}
			}

			return SelectElementByProbability( random, elements );
		}

		static BuildingElement GetSuitableRoofElement( FastRandom random, BuildingElement[] roofElements, Vector2I position )
		{
			var elements = new List<BuildingElement>( roofElements.Length );

			foreach( var element in roofElements )
			{
				elements.Add( element );
			}

			return SelectElementByProbability( random, elements );
		}

		static BuildingElement GetSuitableCellElement( FastRandom random, BuildingElement[] cellElements, ElementSideNotMask side, Vector2I position )
		{
			int sideMask = GetSideMask( side );

			var elements = new List<BuildingElement>( cellElements.Length );

			foreach( var element in cellElements )
			{
				var levels = element.Levels.Value;
				if( position.Y >= levels.Minimum && position.Y <= levels.Maximum && ( (int)element.Sides.Value & sideMask ) != 0 )
				{
					elements.Add( element );
				}
			}

			return SelectElementByProbability( random, elements );
		}

		unsafe protected virtual void CalculateLogicalData( LogicalData logicalData )
		{
			var random = new FastRandom( Seed, true );

			//Basic type
			if( logicalData.BuildingType.Structure == NeoAxis.BuildingType.StructureEnum.Basic )
			{
				var elements = new List<BuildingElement>( 64 );
				elements.AddRange( logicalData.BuildingType.GetComponents<BuildingElement>().Where( c => c.Enabled && c.Probability > 0 ) );
				elements.AddRange( GetComponents<BuildingElement>().Where( c => c.Enabled && c.Probability > 0 ) );

				//var elements = GetComponents<BuildingElement>().Where( c => c.Enabled && c.Probability > 0 ).ToArray();
				////var modifiers = GetComponents<BuildingModifier>().Where( c => c.Enabled ).ToArray();

				if( elements.Count != 0 )
				{
					//var element = elements[ 0 ];
					//logicalData.UsedElements = new BuildingElement[] { element };

					//!!!!
					//var modifiersByPosition = new Dictionary<Vector3I, List<BuildingModifier>>();
					//foreach( var modifier in modifiers )
					//{
					//	if( !modifiersByPosition.TryGetValue( modifier.Position.Value, out var list ) )
					//	{
					//		list = new List<BuildingModifier>();
					//		modifiersByPosition[ modifier.Position.Value ] = list;
					//	}
					//	list.Add( modifier );
					//}

					var usedElements = new ESet<BuildingElement>();
					var renderingItems = new OpenList<LogicalData.RenderingItem>( 512 );
					var collisionMeshItems = new OpenList<LogicalData.CollisionMeshItem>( 512 );

					var ownerTransform = TransformV;
					var elementSize = logicalData.BuildingType.ElementSize.Value;
					var elementSizeH = elementSize.X;
					//var elementSizeV = elementSize.Y;

					var size = Size.Value;

					//calculate levels info
					var levels = new LogicalData.LevelInfo[ size.Z ];//var levelHeights = new double[ size.Z ];
					{
						for( int n = 0; n < levels.Length; n++ )
						{
							ref var item = ref levels[ n ];
							item.Height = elementSize.Y;
						}

						foreach( var element in elements )
						{
							if( element.OverrideHeight != 0 )
							{
								var range = element.Levels.Value;
								if( range.Minimum < 0 )
									range.Minimum = 0;
								if( range.Maximum >= levels.Length )
									range.Maximum = levels.Length - 1;

								for( int n = range.Minimum; n <= range.Maximum; n++ )
								{
									ref var item = ref levels[ n ];
									item.Height = element.OverrideHeight;
								}
							}
						}

						var current = 0.0;
						for( int nLevel = 0; nLevel < levels.Length; nLevel++ )
						{
							ref var level = ref levels[ nLevel ];
							level.HeightFromBottom = current;
							current += level.Height;
						}
					}

					var totalHeight = 0.0;
					foreach( var item in levels )
						totalHeight += item.Height;

					logicalData.Levels = levels;
					logicalData.TotalHeight = totalHeight;


					var totalSize = new Vector3( size.X * elementSizeH, size.Y * elementSizeH, totalHeight );
					//var totalSize = size.ToVector3() * new Vector3( elementSizeH, elementSizeH, elementSizeV );
					var totalSizeHalf = totalSize * 0.5;


					void AddItem( BuildingElement element, Vector3 localPosition, Quaternion localRotation, Vector3 cullingByCameraDirectionLocalNormal, double cullingByCameraDirectionViewAngleFactor, bool addCollision = false )
					{
						var localPosition2 = localPosition;
						var localRotation2 = localRotation;

						localPosition2 += localRotation * element.PositionOffset.Value;
						localRotation2 *= element.RotationOffset.Value;
						localPosition2 -= new Vector3( totalSizeHalf.X, totalSizeHalf.Y, 0 );

						var pos = ownerTransform.Position;
						var rot = ownerTransform.Rotation;
						var scl = ownerTransform.Scale;

						pos += rot * ( localPosition2 * scl );
						rot *= localRotation2;
						scl *= element.ScaleOffset.Value;

						usedElements.AddWithCheckAlreadyContained( element );

						var item = new LogicalData.RenderingItem();
						item.Element = element;
						item.Position = pos;
						item.Rotation = rot.ToQuaternionF();
						item.Scale = scl.ToVector3F();

						if( CullingByCameraDirection && element.CullingByCameraDirection && cullingByCameraDirectionLocalNormal != Vector3F.Zero )
						{
							item.CullingByCameraDirectionData = RenderingPipeline.EncodeCullingByCameraDirectionData( ( ownerTransform.Rotation * cullingByCameraDirectionLocalNormal ).ToVector3F(), (float)cullingByCameraDirectionViewAngleFactor );
							//item.CullingByCameraDirectionData = ( ownerTransform.Rotation * cullingByCameraDirectionLocalNormal ).ToVector3F();
						}

						renderingItems.Add( ref item );

						if( addCollision && CollisionMode.Value != CollisionModeEnum.None && element.Mesh.Value != null )
						{
							var collisionMeshItem = new LogicalData.CollisionMeshItem();
							collisionMeshItem.Mesh = element.Mesh.Value;
							collisionMeshItem.Transform = new Transform( pos, rot, scl );
							collisionMeshItems.Add( collisionMeshItem );
						}
					}


					var localRotations = stackalloc Quaternion[ 4 ];
					localRotations[ 0 ] = Quaternion.FromDirectionZAxisUp( new Vector3( -1, 0, 0 ) );
					localRotations[ 1 ] = Quaternion.FromDirectionZAxisUp( new Vector3( 0, -1, 0 ) );
					localRotations[ 2 ] = Quaternion.FromDirectionZAxisUp( new Vector3( 1, 0, 0 ) );
					localRotations[ 3 ] = Quaternion.FromDirectionZAxisUp( new Vector3( 0, 1, 0 ) );

					var fromRotateByZ90 = Quaternion.FromRotateByZ( new Degree( 90 ).InRadians() );

					//sides
					{
						var sideElements = elements.Where( e => e.ElementType.Value == BuildingElement.ElementTypeEnum.Side ).ToArray();
						//var sidesModifiers = modifiers.Where( m => m.Part.Value == BuildingModifier.PartEnum.Side ).ToArray();

						//sides:
						//0 -x
						//1 -y
						//2 +x
						//3 +y

						for( var side = ElementSideNotMask.MinusX; side <= ElementSideNotMask.PlusY; side++ )
						{
							//var sideModifiers = sidesModifiers.Where( m => m.Side.Value == side ).ToArray();

							for( int level = 0; level < size.Z; level++ )
							{
								var maxX = ( (int)side % 2 == 0 ) ? size.Y : size.X;
								for( int x = 0; x < maxX; x++ )
								{
									var element = GetSuitableSideElement( random, sideElements/*, sideModifiers*/, side, new Vector2I( x, level ) );
									if( element != null )
									{
										var localPosition = Vector3.Zero;
										//var localRotation = Quaternion.Identity;

										switch( side )
										{
										case ElementSideNotMask.MinusX:
											localPosition.X = 0;
											localPosition.Y = totalSize.Y - elementSizeH * x - elementSizeH * 0.5;
											//localRotation = Quaternion.FromDirectionZAxisUp( new Vector3( -1, 0, 0 ) );
											break;
										case ElementSideNotMask.MinusY:
											localPosition.Y = 0;
											localPosition.X = elementSizeH * x + elementSizeH * 0.5;
											//localRotation = Quaternion.FromDirectionZAxisUp( new Vector3( 0, -1, 0 ) );
											break;
										case ElementSideNotMask.PlusX:
											localPosition.X = totalSize.X;
											localPosition.Y = elementSizeH * x + elementSizeH * 0.5;
											//localRotation = Quaternion.FromDirectionZAxisUp( new Vector3( 1, 0, 0 ) );
											//localRotation = Quaternion.Identity;
											break;
										case ElementSideNotMask.PlusY:
											localPosition.Y = totalSize.Y;
											localPosition.X = totalSize.X - elementSizeH * x - elementSizeH * 0.5;
											//localRotation = Quaternion.FromDirectionZAxisUp( new Vector3( 0, 1, 0 ) );
											break;
										}

										localPosition.Z = logicalData.GetLevelHeightFromBottom( level );
										//localPosition.Z = elementSize.Y * level;

										var localRotation = localRotations[ (int)side ];

										//!!!!
										var cullingByCameraDirectionLocalNormal = localRotation.GetForward();
										var cullingByCameraDirectionViewAngleFactor = 0.0;

										AddItem( element, localPosition, localRotation, cullingByCameraDirectionLocalNormal, cullingByCameraDirectionViewAngleFactor );
									}
								}
							}
						}
					}

					//side edge
					{
						var sideEdgeElements = elements.Where( e => e.ElementType.Value == BuildingElement.ElementTypeEnum.SideEdge ).ToArray();
						//var sidesEdgeModifiers = modifiers.Where( m => m.Part.Value == BuildingModifier.PartEnum.Side ).ToArray();

						//sides:
						//0 -x
						//1 -y
						//2 +x
						//3 +y

						for( var side = ElementSideNotMask.MinusX; side <= ElementSideNotMask.PlusY; side++ )
						{
							//var sideModifiers = sidesEdgeModifiers.Where( m => m.Side.Value == side ).ToArray();

							for( int level = 0; level < size.Z; level++ )
							{
								var element = GetSuitableSideEdgeElement( random, sideEdgeElements/*, sideEdgeModifiers*/, side, level );
								if( element != null )
								{
									var localPosition = Vector3.Zero;

									switch( side )
									{
									case ElementSideNotMask.MinusX:
										localPosition.X = 0;
										localPosition.Y = totalSize.Y;
										break;
									case ElementSideNotMask.MinusY:
										localPosition.Y = 0;
										localPosition.X = 0;
										break;
									case ElementSideNotMask.PlusX:
										localPosition.X = totalSize.X;
										localPosition.Y = 0;
										break;
									case ElementSideNotMask.PlusY:
										localPosition.Y = totalSize.Y;
										localPosition.X = totalSize.X;
										break;
									}

									localPosition.Z = logicalData.GetLevelHeightFromBottom( level );
									//localPosition.Z = elementSize.Y * level;

									var localRotation = localRotations[ (int)side ] * fromRotateByZ90;

									//!!!!
									var cullingByCameraDirectionLocalNormal = Vector3.Zero;
									var cullingByCameraDirectionViewAngleFactor = 0.0;

									AddItem( element, localPosition, localRotation, cullingByCameraDirectionLocalNormal, cullingByCameraDirectionViewAngleFactor );
								}
							}
						}
					}

					//roof edge, corner
					unsafe
					{
						var roofEdgeElements = elements.Where( e => e.ElementType.Value == BuildingElement.ElementTypeEnum.RoofEdge ).ToArray();
						var roofCornerElements = elements.Where( e => e.ElementType.Value == BuildingElement.ElementTypeEnum.RoofCorner ).ToArray();

						if( roofEdgeElements.Length != 0 || roofCornerElements.Length != 0 )
						{
							//sides:
							//0 -x
							//1 -y
							//2 +x
							//3 +y

							for( var side = ElementSideNotMask.MinusX; side <= ElementSideNotMask.PlusY; side++ )
							{
								//!!!!
								//var sideModifiers = sidesModifiers.Where( m => m.Side.Value == side ).ToArray();

								var maxX = ( (int)side % 2 == 0 ) ? size.Y : size.X;
								for( int x = 0; x < maxX; x++ )
								{
									var element = GetSuitableRoofEdgeElement( random, roofEdgeElements, /*roofEdgeModifiers, */side, x );
									if( element != null )
									{
										var localPosition = Vector3.Zero;

										switch( side )
										{
										case ElementSideNotMask.MinusX:
											localPosition.X = 0;
											localPosition.Y = totalSize.Y - elementSizeH * x - elementSizeH * 0.5;
											break;
										case ElementSideNotMask.MinusY:
											localPosition.Y = 0;
											localPosition.X = elementSizeH * x + elementSizeH * 0.5;
											break;
										case ElementSideNotMask.PlusX:
											localPosition.X = totalSize.X;
											localPosition.Y = elementSizeH * x + elementSizeH * 0.5;
											break;
										case ElementSideNotMask.PlusY:
											localPosition.Y = totalSize.Y;
											localPosition.X = totalSize.X - elementSizeH * x - elementSizeH * 0.5;
											break;
										}

										localPosition.Z = logicalData.GetLevelHeightFromBottom( size.Z );
										//localPosition.Z = elementSize.Y * size.Z;// level;

										var localRotation = localRotations[ (int)side ];

										//!!!!
										var cullingByCameraDirectionLocalNormal = Vector3.Zero;
										var cullingByCameraDirectionViewAngleFactor = 0.0;

										AddItem( element, localPosition, localRotation, cullingByCameraDirectionLocalNormal, cullingByCameraDirectionViewAngleFactor );
									}
								}

								//corner
								{
									var element = GetSuitableRoofCornerElement( random, roofCornerElements, /*roofEdgeModifiers, */side );
									if( element != null )
									{
										var localPosition = Vector3.Zero;

										switch( side )
										{
										case ElementSideNotMask.MinusX:
											localPosition.X = 0;
											localPosition.Y = totalSize.Y;// - elementSizeH * 0.5;
											break;
										case ElementSideNotMask.MinusY:
											localPosition.Y = 0;
											localPosition.X = 0;// elementSizeH * 0.5;
											break;
										case ElementSideNotMask.PlusX:
											localPosition.X = totalSize.X;
											localPosition.Y = 0;// elementSizeH * 0.5;
											break;
										case ElementSideNotMask.PlusY:
											localPosition.Y = totalSize.Y;
											localPosition.X = totalSize.X;// - elementSizeH * 0.5;
											break;
										}

										localPosition.Z = logicalData.GetLevelHeightFromBottom( size.Z );
										//localPosition.Z = elementSize.Y * size.Z;// level;

										var localRotation = localRotations[ (int)side ] * fromRotateByZ90;

										//!!!!
										var cullingByCameraDirectionLocalNormal = Vector3.Zero;
										var cullingByCameraDirectionViewAngleFactor = 0.0;

										AddItem( element, localPosition, localRotation, cullingByCameraDirectionLocalNormal, cullingByCameraDirectionViewAngleFactor );
									}
								}
							}
						}
					}

					//roof
					{
						var roofElements = elements.Where( e => e.ElementType.Value == BuildingElement.ElementTypeEnum.Roof ).ToArray();

						for( int y = 0; y < size.Y; y++ )
						{
							for( int x = 0; x < size.X; x++ )
							{
								var element = GetSuitableRoofElement( random, roofElements, new Vector2I( x, y ) );
								if( element != null )
								{
									var localPosition = Vector3.Zero;
									localPosition.X = elementSizeH * x + elementSizeH * 0.5;
									localPosition.Y = elementSizeH * y + elementSizeH * 0.5;
									localPosition.Z = totalSize.Z;

									var localRotation = Quaternion.Identity;

									var cullingByCameraDirectionLocalNormal = Vector3.ZAxis;
									var cullingByCameraDirectionViewAngleFactor = 0.0;

									AddItem( element, localPosition, localRotation, cullingByCameraDirectionLocalNormal, cullingByCameraDirectionViewAngleFactor );
								}
							}
						}
					}

					//cells
					{
						var cellElements = elements.Where( e => e.ElementType.Value == BuildingElement.ElementTypeEnum.Cell ).ToArray();
						//var cellModifiers = modifiers.Where( m => m.Part.Value == BuildingModifier.PartEnum.Cell ).ToArray();

						if( cellElements.Length != 0 )//|| cellModifiers.Length != 0 )
						{
							//sides:
							//0 -x
							//1 -y
							//2 +x
							//3 +y

							for( var side = ElementSideNotMask.MinusX; side <= ElementSideNotMask.PlusY; side++ )
							{
								//var sideModifiers = sidesEdgeModifiers.Where( m => m.Side.Value == side ).ToArray();

								for( int level = 0; level < size.Z; level++ )
								{
									var maxX = ( (int)side % 2 == 0 ) ? size.Y : size.X;

									//!!!!
									maxX--;

									for( int x = 0; x < maxX; x++ )
									{
										var side2 = side;

										if( x == 0 )
										{
											switch( side )
											{
											case ElementSideNotMask.MinusX: side2 = ElementSideNotMask.MinusXPlusY; break;
											case ElementSideNotMask.MinusY: side2 = ElementSideNotMask.MinusXMinusY; break;
											case ElementSideNotMask.PlusX: side2 = ElementSideNotMask.PlusXMinusY; break;
											case ElementSideNotMask.PlusY: side2 = ElementSideNotMask.PlusXPlusY; break;
											}
										}
										//!!!!
										//else if( x == maxX - 1 )
										//{
										//	switch( side )
										//	{
										//	case ElementSideNotMask.MinusX: side2 = ElementSideNotMask.MinusXMinusY; break;
										//	case ElementSideNotMask.MinusY: side2 = ElementSideNotMask.PlusXMinusY; break;
										//	case ElementSideNotMask.PlusX: side2 = ElementSideNotMask.PlusXPlusY; break;
										//	case ElementSideNotMask.PlusY: side2 = ElementSideNotMask.MinusXPlusY; break;
										//	}
										//}

										BuildingElement element = null;
										//bool isCorner = false;

										//try to use corner if exists or use side
										if( side2 != side )
										{
											element = GetSuitableCellElement( random, cellElements/*, cellModifiers*/, side2, new Vector2I( x, level ) );

											//if( element != null )
											//	isCorner = true;
										}

										//use side
										if( element == null )
										{
											element = GetSuitableCellElement( random, cellElements/*, cellModifiers*/, side, new Vector2I( x, level ) );
										}

										if( element != null )
										{
											var localPosition = Vector3.Zero;

											switch( side )
											{
											case ElementSideNotMask.MinusX:
												localPosition.X = elementSizeH * 0.5;
												localPosition.Y = totalSize.Y - elementSizeH * x - elementSizeH * 0.5;
												break;
											case ElementSideNotMask.MinusY:
												localPosition.Y = elementSizeH * 0.5;
												localPosition.X = elementSizeH * x + elementSizeH * 0.5;
												break;
											case ElementSideNotMask.PlusX:
												localPosition.X = totalSize.X - elementSizeH * 0.5;
												localPosition.Y = elementSizeH * x + elementSizeH * 0.5;
												break;
											case ElementSideNotMask.PlusY:
												localPosition.Y = totalSize.Y - elementSizeH * 0.5;
												localPosition.X = totalSize.X - elementSizeH * x - elementSizeH * 0.5;
												break;
											}

											localPosition.Z = logicalData.GetLevelHeightFromBottom( level );
											//localPosition.Z = elementSize.Y * level;

											var localRotation = localRotations[ (int)side ];

											Vector3 cullingByCameraDirectionLocalNormal;
											double cullingByCameraDirectionViewAngleFactor;
											if( side2 != side )
											{
												//corner cell
												switch( side2 )
												{
												case ElementSideNotMask.MinusXMinusY:
													cullingByCameraDirectionLocalNormal = new Vector3( -1, -1, 0 ).GetNormalize();
													break;
												case ElementSideNotMask.PlusXMinusY:
													cullingByCameraDirectionLocalNormal = new Vector3( 1, -1, 0 ).GetNormalize();
													break;
												case ElementSideNotMask.MinusXPlusY:
													cullingByCameraDirectionLocalNormal = new Vector3( -1, 1, 0 ).GetNormalize();
													break;
												case ElementSideNotMask.PlusXPlusY:
													cullingByCameraDirectionLocalNormal = new Vector3( 1, 1, 0 ).GetNormalize();
													break;
												default:
													cullingByCameraDirectionLocalNormal = Vector3.Zero;
													break;
												}
												cullingByCameraDirectionViewAngleFactor = 0.5;
											}
											else
											{
												//side cell
												cullingByCameraDirectionLocalNormal = localRotation.GetForward();
												cullingByCameraDirectionViewAngleFactor = 0.0;
											}

											AddItem( element, localPosition, localRotation, cullingByCameraDirectionLocalNormal, cullingByCameraDirectionViewAngleFactor );
										}

									}
								}
							}


							//for( int z = 0; z < size.Z; z++ )
							//{
							//	for( int y = 0; y < size.Y; y++ )
							//	{
							//		for( int x = 0; x < size.X; x++ )
							//		{
							//			var add = false;
							//			if( x == 0 || x == size.X - 1 )
							//				add = true;
							//			else if( y == 0 || y == size.Y - 1 )
							//				add = true;

							//			if( add )
							//			{
							//				var position = new Vector3I( x, y, z );


							//				zzzzz;
							//				//!!!!угловая или нет. в угловой можно увидеть все стороны

							//				zzzzz;

							//				var element = GetSuitableCellElement( random, cellElements, cellModifiers, position );
							//				if( element != null )
							//				{
							//					var localPosition = Vector3.Zero;
							//					localPosition.X = elementSizeH * position.X + elementSizeH * 0.5;
							//					localPosition.Y = elementSizeH * position.Y + elementSizeH * 0.5;
							//					localPosition.Z = elementSizeV * z;

							//					//!!!!
							//					var localRotation = fromRotateByZ90;
							//					//var localRotation = Quaternion.FromRotateByZ( new Degree( 90 ).InRadians() );
							//					////var localRotation = Quaternion.Identity;

							//					//!!!!
							//					//var cullingByCameraDirectionLocalNormal = Vector3F.Zero;


							//					//!!!!угловые


							//					//!!!!
							//					int side = 0;
							//					if( x == 0 )
							//						side = 0;
							//					else if( x == size.X - 1 )
							//						side = 2;
							//					else if( y == 0 )
							//						side = 1;
							//					else if( y == size.Y - 1 )
							//						side = 3;

							//					//0 -x
							//					//1 -y
							//					//2 +x
							//					//3 +y

							//					//!!!!
							//					var cullingByCameraDirectionLocalNormal = localRotations[ side ].GetForward();
							//					var cullingByCameraDirectionViewAngleFactor = 0.0;

							//					AddItem( element, localPosition, localRotation, cullingByCameraDirectionLocalNormal, cullingByCameraDirectionViewAngleFactor );
							//				}
							//			}
							//		}
							//	}
							//}
						}
					}

					//surrounding
					{
						var surroundingElements = elements.Where( e => e.ElementType.Value == BuildingElement.ElementTypeEnum.Surrounding );

						foreach( var element in surroundingElements )
						{
							var distance = element.Distance.Value;
							var interval = element.Interval.Value;
							var step = element.Step.Value;
							var probability = element.Probability.Value;

							var rectSize = totalSize.ToVector2() + new Vector2( distance * 2, distance * 2 );
							var perimeter = ( rectSize.X + rectSize.Y ) * 2;

							var curve = new CurveLine();
							{
								var time = 0.0;
								curve.AddPoint( time, new Vector3( -distance, -distance, 0 ) );
								time += rectSize.X;
								curve.AddPoint( time, new Vector3( totalSize.X + distance, -distance, 0 ) );
								time += rectSize.Y;
								curve.AddPoint( time, new Vector3( totalSize.X + distance, totalSize.Y + distance, 0 ) );
								time += rectSize.X;
								curve.AddPoint( time, new Vector3( -distance, totalSize.Y + distance, 0 ) );
								time += rectSize.Y;
								curve.AddPoint( time, new Vector3( -distance, -distance, 0 ) );
							}

							var end = Math.Min( perimeter, interval.Maximum );
							for( var current = interval.Minimum; current < end; current += step )
							{
								if( probability >= 1 || random.NextDouble() <= probability )
								{
									var localPosition = curve.CalculateValueByTime( current );

									var localRotation = Quaternion.Identity;

									var pos2 = curve.CalculateValueByTime( current + 0.01 );
									var dir = pos2 - localPosition;
									if( dir != Vector3.Zero )
									{
										dir.Normalize();
										localRotation = Quaternion.LookAt( dir, Vector3.ZAxis );
									}

									//!!!!
									//AddItem( element, localPosition, localRotation, Vector3.Zero, 0 );
									AddItem( element, localPosition, localRotation, Vector3.Zero, 0, true );
								}
							}
						}
					}

					//roof surrounding
					{
						var roofSurroundingElements = elements.Where( e => e.ElementType.Value == BuildingElement.ElementTypeEnum.RoofSurrounding );

						var positionZ = logicalData.GetLevelHeightFromBottom( size.Z );
						//var positionZ = elementSize.Y * size.Z;

						foreach( var element in roofSurroundingElements )
						{
							var distance = element.Distance.Value;
							var interval = element.Interval.Value;
							var step = element.Step.Value;
							var probability = element.Probability.Value;

							var rectSize = totalSize.ToVector2() - new Vector2( distance * 2, distance * 2 );
							var perimeter = ( rectSize.X + rectSize.Y ) * 2;

							var curve = new CurveLine();
							{
								var time = 0.0;
								curve.AddPoint( time, new Vector3( distance, distance, positionZ ) );
								time += rectSize.X;
								curve.AddPoint( time, new Vector3( totalSize.X - distance, distance, positionZ ) );
								time += rectSize.Y;
								curve.AddPoint( time, new Vector3( totalSize.X - distance, totalSize.Y - distance, positionZ ) );
								time += rectSize.X;
								curve.AddPoint( time, new Vector3( distance, totalSize.Y - distance, positionZ ) );
								time += rectSize.Y;
								curve.AddPoint( time, new Vector3( distance, distance, positionZ ) );
							}

							var end = Math.Min( perimeter, interval.Maximum );
							for( var current = interval.Minimum; current < end; current += step )
							{
								if( probability >= 1 || random.NextDouble() <= probability )
								{
									var localPosition = curve.CalculateValueByTime( current );

									var localRotation = Quaternion.Identity;

									var pos2 = curve.CalculateValueByTime( current + 0.01 );
									var dir = pos2 - localPosition;
									if( dir != Vector3.Zero )
									{
										dir.Normalize();
										localRotation = Quaternion.LookAt( dir, Vector3.ZAxis );
									}

									//!!!!
									//AddItem( element, localPosition, localRotation, Vector3.Zero, 0 );
									AddItem( element, localPosition, localRotation, Vector3.Zero, 0, true );
								}
							}
						}
					}

					logicalData.UsedElements = usedElements.ToArray();
					logicalData.RenderingItems = renderingItems.ToArray();
					logicalData.CollisionMeshItems = collisionMeshItems.ToArray();
				}
			}
		}

		public LogicalData GetLogicalData()// bool canCreate = true )
		{
			//!!!!если сейчас внутри вызова. где еще так. visual data, другие компоненты

			if( logicalData == null && EnabledInHierarchyAndIsInstance )//&& canCreate )
			{
				logicalData = new LogicalData();
				logicalData.Owner = this;
				logicalData.BuildingType = BuildingType.Value;
				if( logicalData.BuildingType == null )
					logicalData.BuildingType = new BuildingType();
				logicalData.BuildingTypeUsedVersion = logicalData.BuildingType.Version;

				var handled = false;
				CalculateLogicalDataBefore?.Invoke( this, logicalData, ref handled );

				if( !handled )
					CalculateLogicalData( logicalData );

				CalculateLogicalDataAfter?.Invoke( this, logicalData );

				logicalData.UpdateCollisionBodies();
			}
			return logicalData;
		}

		void DeleteLogicalData()
		{
			if( logicalData != null )
			{
				logicalData.DestroyCollisionBodies();
				logicalData = null;
			}
		}

		internal static void UpdateGroupOfObjects( Scene scene, GroupOfObjects group )
		{
			var manager = scene.GetComponent<BuildingManager>();
			if( manager != null )
			{
				group.SectorSize = manager.SectorSize;
				//group.MaxObjectsInGroup = manager.MaxObjectsInGroup;
			}
			else
			{
				group.SectorSize = new Vector3( 200, 200, 10000 );
				//group.MaxObjectsInGroup = 100000;// 10000;
			}
		}

		GroupOfObjects GetOrCreateGroupOfObjects( bool canCreate )
		{
			var scene = ParentScene;
			if( scene == null )
				return null;

			var name = "__GroupOfObjectsBuildings";

			var group = scene.GetComponent<GroupOfObjects>( name );
			if( group == null && canCreate )
			{
				//need set ShowInEditor = false before AddComponent
				group = ComponentUtility.CreateComponent<GroupOfObjects>( null, false, false );
				group.DisplayInEditor = false;
				scene.AddComponent( group, -1 );
				//var group = scene.CreateComponent<GroupOfObjects>();

				group.Name = name;
				//group.CanBeSelected = false;
				group.SaveSupport = false;
				group.CloneSupport = false;
				group.NetworkMode = NetworkModeEnum.False;

				group.AnyData = new Dictionary<(Mesh, Material/*, bool*/), int>();

				UpdateGroupOfObjects( scene, group );

				group.Enabled = true;
			}

			return group;
		}

		//!!!!удалять элементы? где еще так
		ushort GetOrCreateGroupOfObjectsElement( GroupOfObjects group, Mesh mesh, Material replaceMaterial )//, bool collision )
		{
			var key = (mesh, replaceMaterial/*, collision*/);

			var dictionary = (Dictionary<(Mesh mesh, Material/*, bool*/), int>)group.AnyData;

			GroupOfObjectsElement_Mesh element = null;

			if( dictionary.TryGetValue( key, out var elementIndex2 ) )
				return (ushort)elementIndex2;

			if( element == null )
			{
				var elementIndex = group.GetFreeElementIndex();
				element = group.CreateComponent<GroupOfObjectsElement_Mesh>( enabled: false );
				element.Name = "Element " + elementIndex.ToString();
				element.Index = elementIndex;
				element.Mesh = mesh;
				element.ReplaceMaterial = replaceMaterial;
				element.AutoAlign = false;
				//element.Collision = collision;

				//!!!!
				//element.VisibilityDistanceFactor = ;

				element.Enabled = true;

				dictionary[ key ] = element.Index;

				//!!!!
				group.ElementTypesCacheNeedUpdate();
			}

			return (ushort)element.Index.Value;
		}

		//unsafe void CreateMeshObject( Mesh mesh, Transform transform, /*double? clipDistance, */Material replaceMaterial/*, bool useGroupOfObjects*/ )
		//{
		//	//if( useGroupOfObjects && !clipDistance.HasValue )
		//	//{

		//	var group = GetOrCreateGroupOfObjects( true );
		//	if( group != null )
		//	{
		//		var elementIndex = GetOrCreateGroupOfObjectsElement( group, mesh, replaceMaterial );
		//		var pos = transform.Position;
		//		var rot = transform.Rotation.ToQuaternionF();
		//		var scl = transform.Scale.ToVector3F();

		//		//!!!!
		//		var color = new ColorValue( 1, 1, 1 );//ColorMultiplier;

		//		var obj = new GroupOfObjects.Object( elementIndex, 0, 0, GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible, pos, rot, scl, Vector4F.Zero, color, Vector4F.Zero, Vector4F.Zero );

		//		//!!!!может обновление сразу вызывается, тогда не круто
		//		var objects = group.ObjectsAdd( &obj, 1 );
		//		visualData.groupOfObjectObjects.AddRange( objects );
		//	}

		//	//}
		//	//else
		//	//{
		//	//	//need set ShowInEditor = false before AddComponent
		//	//	var meshInSpace = ComponentUtility.CreateComponent<MeshInSpace>( null, false, false );
		//	//	//generator.MeshInSpace = meshInSpace;
		//	//	meshInSpace.DisplayInEditor = false;
		//	//	AddComponent( meshInSpace, -1 );
		//	//	//var meshInSpace = CreateComponent<MeshInSpace>();

		//	//	meshInSpace.Name = "__Mesh In Space";
		//	//	meshInSpace.CanBeSelected = false;
		//	//	meshInSpace.SaveSupport = false;
		//	//	meshInSpace.CloneSupport = false;
		//	//	meshInSpace.Transform = transform;
		//	//	meshInSpace.Color = ColorMultiplier;
		//	//	meshInSpace.ReplaceMaterial = replaceMaterial;

		//	//	if( clipDistance.HasValue )
		//	//	{
		//	//		//!!!!Plane always clip. bug

		//	//		var box = new Box( transform.Position, new Vector3( clipDistance.Value * 2, 10000, 10000 ), transform.Rotation.ToMatrix3() );
		//	//		var item = new RenderingPipeline.RenderSceneData.CutVolumeItem( box, CutVolumeFlags.Invert | CutVolumeFlags.CutScene | CutVolumeFlags.CutShadows );
		//	//		//var item = new RenderingPipeline.RenderSceneData.CutVolumeItem( box, true, true, true, false );

		//	//		//var item = new RenderingPipeline.RenderSceneData.CutVolumeItem();
		//	//		//item.Shape = CutVolumeShape.Plane;
		//	//		//item.CutScene = true;
		//	//		//item.CutShadows = true;

		//	//		//var dir = transform.Rotation.GetForward();
		//	//		//item.Plane = Plane.FromPointAndNormal( transform.Position + dir * clipDistance.Value, -dir );

		//	//		meshInSpace.CutVolumes = new[] { item };
		//	//	}

		//	//	meshInSpace.Mesh = mesh;// ReferenceUtility.MakeThisReference( meshInSpace, mesh );
		//	meshInSpace.StaticShadows = true;
		//	//	meshInSpace.Enabled = true;
		//	//}
		//}

		public VisualData GetVisualData()
		{
			if( visualData == null )
			{
				GetLogicalData();
				if( logicalData != null )
				{
					visualData = new VisualData();

					if( logicalData.RenderingItems != null && logicalData.RenderingItems.Length != 0 )
					{
						var manager = ParentScene?.GetComponent<BuildingManager>();
						if( manager == null || manager.Display )
						{
							var group = GetOrCreateGroupOfObjects( true );
							if( group != null )
							{
								visualData.groupOfObjects = group;

								var objects = new OpenList<GroupOfObjects.Object>( logicalData.RenderingItems.Length );

								//var allowCollision = CollisionMode.Value != CollisionModeEnum.None;
								//if( manager != null && !manager.Collision )
								//	allowCollision = false;

								foreach( var item in logicalData.RenderingItems )
								{
									var element = item.Element;
									if( element != null )//if( item.ElementIndex >= 0 && item.ElementIndex < logicalData.UsedElements.Length )
									{
										if( manager != null )
										{
											//!!!!
											if( !manager.DisplayFacade && element.ElementType.Value != BuildingElement.ElementTypeEnum.Cell && element.ElementType.Value != BuildingElement.ElementTypeEnum.Surrounding && element.ElementType.Value != BuildingElement.ElementTypeEnum.RoofSurrounding )
												continue;
											if( !manager.DisplayCells && element.ElementType.Value == BuildingElement.ElementTypeEnum.Cell )
												continue;
											if( !manager.DisplaySurrounding && ( element.ElementType.Value == BuildingElement.ElementTypeEnum.Surrounding || element.ElementType.Value == BuildingElement.ElementTypeEnum.RoofSurrounding ) )
												continue;
										}

										//var element = logicalData.UsedElements[ item.ElementIndex ];

										var mesh = element.Mesh.Value;
										if( mesh == null )
											mesh = ResourceUtility.MeshInvalid;

										Material replaceMaterial = null;

										var elementIndex = GetOrCreateGroupOfObjectsElement( group, mesh, replaceMaterial );

										//!!!!
										var color = new ColorValue( 1, 1, 1 );//ColorMultiplier;

										//uint cullingByCameraDirectionData = 0;
										//if( item.CullingByCameraDirectionData != Vector4F.Zero )
										//{
										//	cullingByCameraDirectionData = RenderingPipeline.EncodeCullingByCameraDirectionData( item.CullingByCameraDirectionData );
										//}
										////if( item.CullingByCameraDirectionData != Vector3F.Zero )
										////{
										////	cullingByCameraDirectionData = RenderingPipeline.EncodeCullingByCameraDirectionData( item.CullingByCameraDirectionData );
										////}

										var obj = new GroupOfObjects.Object( elementIndex, 0, 0, GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible, item.Position, item.Rotation, item.Scale, Vector4F.Zero, color, Vector4F.Zero, Vector4F.Zero, item.CullingByCameraDirectionData );

										objects.Add( ref obj );
									}
								}

								var subGroup = new GroupOfObjects.SubGroup( objects.ArraySegment );
								group.AddSubGroupToQueue( subGroup );
								visualData.groupOfObjectSubGroups.Add( subGroup );

								//foreach( var item in logicalData.GetMeshesToCreate() )
								//	CreateMeshObject( item.mesh, item.transform, /*item.clipDistancePanelLength * item.clipDistanceFactor, */item.replaceMaterial/*, true */);
							}
						}
					}
				}
			}
			return visualData;
		}

		void DeleteVisualData()
		{
			if( visualData != null )
			{
				var group = visualData.groupOfObjects;
				if( group != null && visualData.groupOfObjectSubGroups.Count != 0 )
				{
					foreach( var subGroup in visualData.groupOfObjectSubGroups )
						group.RemoveSubGroupToQueue( subGroup );
				}

				visualData = null;
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = ParentScene;
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
				{
					//!!!!new
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
					//scene.ViewportUpdateBefore += Scene_ViewportUpdateBefore;
#if !DEPLOY
					TransformToolUtility.AllInstances_ModifyCommit += TransformTool_AllInstances_ModifyCommit;
					TransformToolUtility.AllInstances_ModifyCancel += TransformTool_AllInstances_ModifyCancel;
#endif

					if( logicalData == null )
						Update();
				}
				else
				{
					//!!!!new
					scene.GetRenderSceneData -= Scene_GetRenderSceneData;
					//scene.ViewportUpdateBefore -= Scene_ViewportUpdateBefore;
#if !DEPLOY
					TransformToolUtility.AllInstances_ModifyCommit -= TransformTool_AllInstances_ModifyCommit;
					TransformToolUtility.AllInstances_ModifyCancel -= TransformTool_AllInstances_ModifyCancel;
#endif

					Update();
				}
			}
		}

#if !DEPLOY

		private void TransformTool_AllInstances_ModifyCommit( ITransformTool sender )
		{
			if( needUpdateAfterEndModifyingTransformTool )
			{
				NeedUpdate();
				//Update();
				needUpdateAfterEndModifyingTransformTool = false;
			}
		}

		private void TransformTool_AllInstances_ModifyCancel( ITransformTool sender )
		{
			if( needUpdateAfterEndModifyingTransformTool )
			{
				NeedUpdate();
				//Update();
				needUpdateAfterEndModifyingTransformTool = false;
			}
		}

#endif

		public void Update()
		{
			var size = Size.Value;
			var totalCells = size.X * size.Y * size.Z;
			if( totalCells > 500 )
			{
				if( EditorAPI.IsAnyTransformToolInModifyingMode() )
				{
					needUpdateAfterEndModifyingTransformTool = true;
					return;
				}
			}

			DeleteVisualData();
			DeleteLogicalData();
			occluderCachedTransform = null;

			if( EnabledInHierarchyAndIsInstance )
			{
				GetLogicalData();
				SpaceBoundsUpdate();
			}

			needUpdate = false;
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			var data = GetLogicalData();
			if( data != null && data.RenderingItems != null && data.RenderingItems.Length != 0 )
			{
				data.GetCellsBox( out var box, out _ );
				newBounds = new SpaceBounds( box.ToBounds() );
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EnabledInHierarchyAndIsInstance )
			{
				if( EngineApp.IsEditor && !needUpdate )
				{
					var b = BuildingType.Value;
					if( b != null && logicalData != null && ( logicalData.BuildingType != b || logicalData.BuildingTypeUsedVersion != b.Version ) )
						needUpdate = true;
				}

				if( needUpdate )
					Update();
			}
		}

		private void Scene_GetRenderSceneData( Scene scene, ViewportRenderingContext context )
		{
			if( VisibleInHierarchy )
				GetVisualData();
			else
				DeleteVisualData();
		}

		//private void Scene_ViewportUpdateBefore( Scene scene, Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		//{
		//	//!!!!лучше всё же в OnGetRenderSceneData
		//	//хотя SpaceBounds видимо больше должен быть, учитывать габариты мешей
		//	//когда долго не видим объект, данные можно удалять. лишь бы не было что создается/удаляется каждый раз

		//	//!!!!может VisibleInHierarchy менять флаг в объектах

		//	if( VisibleInHierarchy )
		//		GetVisualData();
		//	else
		//		DeleteVisualData();
		//}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

#if !DEPLOY
			//display editor selection
			if( EngineApp.IsEditor && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;
				if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
				{
					ColorValue color;
					if( context2.selectedObjects.Contains( this ) )
						color = ProjectSettings.Get.Colors.SelectedColor;
					else
						color = ProjectSettings.Get.Colors.CanSelectColor;

					var renderer = context.Owner.Simple3DRenderer;
					if( renderer != null )
					{
						color.Alpha *= .5f;
						renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

						if( logicalData != null )
						{
							logicalData.GetCellsBox( out var box, out _ );
							renderer.AddBox( box );
						}
					}
				}
			}
#endif
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			NeedUpdate();
		}

		protected override bool OnSave( Metadata.SaveContext context, TextBlock block, ref bool skipSave, out string error )
		{
			if( !base.OnSave( context, block, ref skipSave, out error ) )
				return false;

			//prevent update because the serialization touchs properties
			needUpdate = false;

			return true;
		}

		protected override void OnComponentAdded( Component component )
		{
			base.OnComponentAdded( component );

			if( component is BuildingElement )//|| component is BuildingModifier )
				NeedUpdate();
		}

		protected override void OnComponentRemoved( Component component )
		{
			base.OnComponentRemoved( component );

			if( component is BuildingElement )//|| component is BuildingModifier )
				NeedUpdate();
		}

		public void NeedUpdate()
		{
			needUpdate = true;
		}

		protected override Scene.SceneObjectFlags OnGetSceneObjectFlags()
		{
			var result = EngineApp.IsSimulation ? Scene.SceneObjectFlags.Logic : base.OnGetSceneObjectFlags();
			if( Occluder )
				result |= Scene.SceneObjectFlags.Occluder;
			return result;
		}

		//protected override bool OnOcclusionCullingDataContains()
		//{
		//	if( Occluder )
		//		return true;
		//	return false;
		//}

		bool IsVisualDataContainsObjects()
		{
			if( visualData != null )
			{
				var group = visualData.groupOfObjects;
				if( group != null && visualData.groupOfObjectSubGroups.Count != 0 )
					return true;
				//if( group != null && visualData.groupOfObjectObjects.Count != 0 )
				//	return true;
			}
			return false;
		}

		protected override void OnOcclusionCullingDataGet( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem, OpenList<RenderingPipeline.OccluderItem> occluders )
		{
			base.OnOcclusionCullingDataGet( context, mode, modeGetObjectsItem, occluders );

			if( Occluder && IsVisualDataContainsObjects() && logicalData != null )
			{
				var tr = Transform.Value;

				if( occluderCachedTransform == null || tr != occluderCachedTransform )
				{
					logicalData.GetCellsBox( out var box, out _ );

					var extendBottom = logicalData.BuildingType.OccluderExtendBottom.Value;
					var shrinkHorizontal = logicalData.BuildingType.OccluderShrinkHorizontal.Value;

					box.Extents.X -= 0.2 + shrinkHorizontal;
					box.Extents.Y -= 0.2 + shrinkHorizontal;
					box.Center.Z -= extendBottom * 0.5;
					box.Extents.Z += extendBottom * 0.5;

					//var extend = OccluderExtend.Value;
					//box.Center -= extend.GetCenter() * 0.5;
					//box.Extents += extend.GetSize();

					var worldMatrix = new Matrix4( box.Axis * Matrix3.FromScale( box.Extents * 2 ), box.Center );

					SimpleMeshGenerator.GenerateBox( new Vector3( 1, 1, 1 ), out Vector3[] vertices, out int[] indices );

					for( int n = 0; n < vertices.Length; n++ )
						vertices[ n ] = worldMatrix * vertices[ n ];

					occluderCachedTransform = tr;
					occluderCachedBoxShapeVertices = vertices;
					occluderCachedBoxShapeIndices = indices;
				}

				var item = new RenderingPipeline.OccluderItem();
				item.Center = occluderCachedTransform.Position;
				item.Vertices = occluderCachedBoxShapeVertices;
				item.Indices = occluderCachedBoxShapeIndices;
				occluders.Add( ref item );
			}
		}

		protected virtual void OnCalculateCollisionBodies()
		{
			var logicalData = GetLogicalData();
			var scene = ParentScene;
			if( logicalData != null && scene != null )
			{
				//simple box
				{
					logicalData.GetCellsBox( out var box, out var rotation );

					var shapeData = new RigidBody();
					var shape = shapeData.CreateComponent<CollisionShape_Box>();
					shape.Dimensions = box.Extents * 2;

					var shapeItem = scene.PhysicsWorld.AllocateShape( shapeData, Vector3.One );
					if( shapeItem != null )
					{
						var rigidBodyItem = scene.PhysicsWorld.CreateRigidBodyStatic( shapeItem, true, this, box.Center, rotation.ToQuaternionF() );
						if( rigidBodyItem != null )
							logicalData.CollisionBodies.Add( rigidBodyItem );
					}
				}

				//additional objects (surrounding, etc)
				if( logicalData.CollisionMeshItems != null )
				{
					var collisionDefinitionByMesh = new Dictionary<Mesh, RigidBody>( 32 );

					foreach( var item in logicalData.CollisionMeshItems )
					{
						if( !collisionDefinitionByMesh.TryGetValue( item.Mesh, out var collisionDefinition ) )
						{
							collisionDefinition = item.Mesh.GetComponent( "Collision Definition" ) as RigidBody;
							if( collisionDefinition == null )
								collisionDefinition = item.Mesh?.Result.GetAutoCollisionDefinition();
							collisionDefinitionByMesh[ item.Mesh ] = collisionDefinition;
						}

						if( collisionDefinition != null )
						{
							var shapeItem = scene.PhysicsWorld.AllocateShape( collisionDefinition, item.Transform.Scale );
							if( shapeItem != null )
							{
								var bodyItem = scene.PhysicsWorld.CreateRigidBodyStatic( shapeItem, true, this, item.Transform.Position, item.Transform.Rotation.ToQuaternionF() );
								if( bodyItem != null )
									logicalData.CollisionBodies.Add( bodyItem );
							}
						}
					}
				}
			}
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//var p = member as Metadata.Property;
			//if( p != null )
			//{
			//	switch( p.Name )
			//	{
			//	case nameof( OccluderExtendBottom ):
			//		if( !Occluder )
			//			skip = true;
			//		break;
			//	}
			//}
		}

	}
}
