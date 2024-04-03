// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Component for setting item type of <see cref="GroupOfObjects"/>.
	/// </summary>
#if !DEPLOY
	[SettingsCell( "NeoAxis.Editor.GroupOfObjectsElementSettingsCell" )]
#endif
	public abstract class GroupOfObjectsElement : Component
	{
		/// <summary>
		/// The identifier of the element in the group of objects.
		/// </summary>
		[DefaultValue( 0 )]
		public Reference<int> Index
		{
			get { if( _index.BeginGet() ) Index = _index.Get( this ); return _index.value; }
			set { if( _index.BeginSet( this, ref value ) ) { try { IndexChanged?.Invoke( this ); } finally { _index.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Index"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement> IndexChanged;
		ReferenceField<int> _index = 0;

		/// <summary>
		/// Whether to align objects position by base objects.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AutoAlign
		{
			get { if( _autoAlign.BeginGet() ) AutoAlign = _autoAlign.Get( this ); return _autoAlign.value; }
			set { if( _autoAlign.BeginSet( this, ref value ) ) { try { AutoAlignChanged?.Invoke( this ); } finally { _autoAlign.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AutoAlign"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement> AutoAlignChanged;
		ReferenceField<bool> _autoAlign = true;


		//!!!!потом может добавиться как именно выравнивать


		//

		protected override void OnEnabledChanged()
		{
			base.OnEnabledChanged();

			( Parent as GroupOfObjects )?.NeedUpdate();
			//( Parent as GroupOfObjects )?.ElementTypesCacheNeedUpdate();
		}

		public bool ObjectsExists()
		{
			var groupOfObjects = Parent as GroupOfObjects;
			if( groupOfObjects != null )
			{
				foreach( var index in groupOfObjects.ObjectsGetAll() )
				{
					ref var obj = ref groupOfObjects.ObjectGetData( index );
					if( obj.Element == Index )
						return true;
				}
			}
			return false;
		}

		public List<int> GetObjectsOfElement()
		{
			var groupOfObjects = Parent as GroupOfObjects;
			if( groupOfObjects != null )
			{
				var indexes = groupOfObjects.ObjectsGetAll();
				var list = new List<int>( indexes.Count );
				foreach( var index in indexes )
				{
					ref var obj = ref groupOfObjects.ObjectGetData( index );
					if( obj.Element == Index )
						list.Add( index );
				}
				return list;
			}
			return new List<int>();
		}

#if !DEPLOY

		public virtual void SetColors( UndoMultiAction undoMultiAction, ColorValue color )
		{
			var groupOfObjects = Parent as GroupOfObjects;
			if( groupOfObjects != null )
			{
				var indexes = GetObjectsOfElement();
				var newObjects = groupOfObjects.ObjectsGetData( indexes );

				for( int n = 0; n < indexes.Count; n++ )
				{
					ref var obj = ref newObjects[ n ];

					//!!!!?
					obj.UniqueIdentifier = 0;

					obj.Color = color;
				}

				//delete and undo to delete
				undoMultiAction.AddAction( new GroupOfObjectsUndo.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true ) );

				//add new data
				var newIndexes = groupOfObjects.ObjectsAdd( newObjects );
				undoMultiAction.AddAction( new GroupOfObjectsUndo.UndoActionCreateDelete( groupOfObjects, newIndexes, true, false ) );
			}
		}

#endif
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Component for setting mesh item type of <see cref="GroupOfObjects"/>.
	/// </summary>
	public class GroupOfObjectsElement_Mesh : GroupOfObjectsElement
	{
		/// <summary>
		/// The mesh used by the mesh object.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set
			{
				if( _mesh.BeginSet( this, ref value ) )
				{
					try
					{
						MeshChanged?.Invoke( this );
						( Parent as GroupOfObjects )?.ElementTypesCacheNeedUpdate();
						if( StaticShadows )
							( Parent as GroupOfObjects )?.NeedUpdateStaticShadows();
					}
					finally { _mesh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement_Mesh> MeshChanged;
		ReferenceField<Mesh> _mesh = null;

		/// <summary>
		/// Replaces all geometries of the mesh by another material.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		public Reference<Material> ReplaceMaterial
		{
			get { if( _replaceMaterial.BeginGet() ) ReplaceMaterial = _replaceMaterial.Get( this ); return _replaceMaterial.value; }
			set
			{
				if( _replaceMaterial.BeginSet( this, ref value ) )
				{
					try
					{
						ReplaceMaterialChanged?.Invoke( this );
						( Parent as GroupOfObjects )?.ElementTypesCacheNeedUpdate();
						if( StaticShadows )
							( Parent as GroupOfObjects )?.NeedUpdateStaticShadows();
					}
					finally { _replaceMaterial.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReplaceMaterial"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement_Mesh> ReplaceMaterialChanged;
		ReferenceField<Material> _replaceMaterial;

		/// <summary>
		/// The factor of maximum visibility distance. The maximum distance is calculated based on the size of the object on the screen.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> VisibilityDistanceFactor
		{
			get { if( _visibilityDistanceFactor.BeginGet() ) VisibilityDistanceFactor = _visibilityDistanceFactor.Get( this ); return _visibilityDistanceFactor.value; }
			set
			{
				if( _visibilityDistanceFactor.BeginSet( this, ref value ) )
				{
					try
					{
						VisibilityDistanceFactorChanged?.Invoke( this );
						( Parent as GroupOfObjects )?.ElementTypesCacheNeedUpdate();
					}
					finally { _visibilityDistanceFactor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="VisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement_Mesh> VisibilityDistanceFactorChanged;
		ReferenceField<double> _visibilityDistanceFactor = 1.0;

		///// <summary>
		///// Maximum visibility range of the object.
		///// </summary>
		//[DefaultValue( 10000.0 )]
		//[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> VisibilityDistance
		//{
		//	get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
		//	set
		//	{
		//		if( _visibilityDistance.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				VisibilityDistanceChanged?.Invoke( this );
		//				( Parent as GroupOfObjects )?.ElementTypesCacheNeedUpdate();
		//			}
		//			finally { _visibilityDistance.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		//public event Action<GroupOfObjectsElement_Mesh> VisibilityDistanceChanged;
		//ReferenceField<double> _visibilityDistance = 10000.0;

		/// <summary>
		/// Whether to cast shadows on the other surfaces.
		/// </summary>
		[DefaultValue( true )]
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
						( Parent as GroupOfObjects )?.ElementTypesCacheNeedUpdate();
						if( StaticShadows )
							( Parent as GroupOfObjects )?.NeedUpdateStaticShadows();
					}
					finally { _castShadows.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CastShadows"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement_Mesh> CastShadowsChanged;
		ReferenceField<bool> _castShadows = true;

		/// <summary>
		/// Whether it is possible to apply decals on the surface.
		/// </summary>
		[DefaultValue( true )]
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
						( Parent as GroupOfObjects )?.ElementTypesCacheNeedUpdate();
					}
					finally { _receiveDecals.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement_Mesh> ReceiveDecalsChanged;
		ReferenceField<bool> _receiveDecals = true;

		/// <summary>
		/// The multiplier of the motion blur effect.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
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
						( Parent as GroupOfObjects )?.ElementTypesCacheNeedUpdate();
					}
					finally { _motionBlurFactor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MotionBlurFactor"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement_Mesh> MotionBlurFactorChanged;
		ReferenceField<double> _motionBlurFactor = 1.0;

		/// <summary>
		/// Whether to enable the static shadows optimization.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> StaticShadows
		{
			get { if( _staticShadows.BeginGet() ) StaticShadows = _staticShadows.Get( this ); return _staticShadows.value; }
			set
			{
				if( _staticShadows.BeginSet( this, ref value ) )
				{
					try
					{
						StaticShadowsChanged?.Invoke( this );
						( Parent as GroupOfObjects )?.NeedUpdate();
					}
					finally { _staticShadows.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="StaticShadows"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement_Mesh> StaticShadowsChanged;
		ReferenceField<bool> _staticShadows = true;

		/// <summary>
		/// Whether to enable a collision detection. A collision rigidbody of the mesh is used.
		/// </summary>
		[DefaultValue( false )]
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
						( Parent as GroupOfObjects )?.NeedUpdate();
					}
					finally { _collision.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Collision"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement_Mesh> CollisionChanged;
		ReferenceField<bool> _collision = false;

		/////////////////////////////////////////

#if !DEPLOY
		public virtual void UpdateAlignment( UndoMultiAction undoMultiAction )
		{
			//var random = new FastRandom();

			var groupOfObjects = Parent as GroupOfObjects;
			if( groupOfObjects != null )
			{
				//var surface = Surface.Value;
				//if( surface != null )
				//{

				var indexes = GetObjectsOfElement();
				var scene = groupOfObjects.FindParent<Scene>();

				var newObjects = groupOfObjects.ObjectsGetData( indexes );

				for( int n = 0; n < indexes.Count; n++ )
				{
					var index = indexes[ n ];
					ref var obj = ref newObjects[ n ];

					double positionZ = 0;

					obj.UniqueIdentifier = 0;
					if( AutoAlign && scene != null )
					{
						var r = SceneUtility.CalculateObjectPositionZ( scene, groupOfObjects, obj.Position.Z, obj.Position.ToVector2() );
						if( r.found )
						{
							obj.Position.Z = r.positionZ + positionZ;
							//!!!!normal
							//obj.Rotation = ;
						}
					}
					//obj.Rotation = rotation;
					//obj.Scale = scale;
				}

				//delete and undo to delete
				undoMultiAction.AddAction( new GroupOfObjectsUndo.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true ) );

				//add new data
				var newIndexes = groupOfObjects.ObjectsAdd( newObjects );
				undoMultiAction.AddAction( new GroupOfObjectsUndo.UndoActionCreateDelete( groupOfObjects, newIndexes, true, false ) );

				//}
			}
		}
#endif
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Component for setting surface item type of <see cref="GroupOfObjects"/>.
	/// </summary>
	public class GroupOfObjectsElement_Surface : GroupOfObjectsElement
	{
		[DefaultValue( null )]
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
						( Parent as GroupOfObjects )?.ElementTypesCacheNeedUpdate();
						//if( StaticShadows )
						( Parent as GroupOfObjects )?.NeedUpdateStaticShadows();
					}
					finally { _surface.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Surface"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement_Surface> SurfaceChanged;
		ReferenceField<Surface> _surface = null;

		/// <summary>
		/// The factor of maximum visibility distance for objects of the surface. The maximum distance is calculated based on the size of the object on the screen.
		/// </summary>
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DefaultValue( 1.0 )]
		public Reference<double> VisibilityDistanceFactor
		{
			get { if( _visibilityDistanceFactor.BeginGet() ) VisibilityDistanceFactor = _visibilityDistanceFactor.Get( this ); return _visibilityDistanceFactor.value; }
			set
			{
				if( _visibilityDistanceFactor.BeginSet( this, ref value ) )
				{
					try
					{
						VisibilityDistanceFactorChanged?.Invoke( this );
						( Parent as GroupOfObjects )?.ElementTypesCacheNeedUpdate();
					}
					finally { _visibilityDistanceFactor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="VisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement_Surface> VisibilityDistanceFactorChanged;
		ReferenceField<double> _visibilityDistanceFactor = 1.0;

		/// <summary>
		/// Whether to cast shadows on the other surfaces for objects of the surface.
		/// </summary>
		[DefaultValue( true )]
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
						( Parent as GroupOfObjects )?.ElementTypesCacheNeedUpdate();
						//if( StaticShadows )
						( Parent as GroupOfObjects )?.NeedUpdateStaticShadows();
					}
					finally { _castShadows.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CastShadows"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement_Surface> CastShadowsChanged;
		ReferenceField<bool> _castShadows = true;

		/// <summary>
		/// Whether to enable a collision detection. A collision definition of the mesh is used.
		/// </summary>
		[DefaultValue( false )]
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
						( Parent as GroupOfObjects )?.NeedUpdate();
					}
					finally { _collision.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Collision"/> property value changes.</summary>
		public event Action<GroupOfObjectsElement_Surface> CollisionChanged;
		ReferenceField<bool> _collision = false;

		//!!!!
		//и другие какие в PaintLayer
		//LODScale
		//public Material replaceMaterial;
		//public bool receiveDecals;
		//public float motionBlurFactor;

		/////////////////////////////////////////

#if !DEPLOY

		public virtual void UpdateVariationsAndTransform( bool randomizeGroups, UndoMultiAction undoMultiAction, bool updateVariations, bool updateTransform )
		{
			var random = new FastRandom();

			var groupOfObjects = Parent as GroupOfObjects;
			if( groupOfObjects != null )
			{
				var surface = Surface.Value;
				if( surface != null )
				{
					var indexes = GetObjectsOfElement();
					var scene = groupOfObjects.FindParent<Scene>();

					var newObjects = groupOfObjects.ObjectsGetData( indexes );

					for( int n = 0; n < indexes.Count; n++ )
					{
						var index = indexes[ n ];
						ref var obj = ref newObjects[ n ];

						var alignFound = false;
						var alignPositionZ = 0.0;
						var alignNormal = Vector3F.Zero;
						if( AutoAlign && scene != null )
						{
							var r = SceneUtility.CalculateObjectPositionZ( scene, groupOfObjects, obj.Position.Z, obj.Position.ToVector2() );
							if( r.found )
							{
								alignFound = true;
								alignPositionZ = r.positionZ;
								alignNormal = r.normal;
							}
						}

						byte? setGroup = null;
						if( !randomizeGroups )
							setGroup = obj.VariationGroup;

						Vector3F? surfaceNormal = null;
						if( alignFound )
							surfaceNormal = alignNormal;

						var options = new Surface.GetRandomVariationOptions( setGroup, surfaceNormal );
						surface.GetRandomVariation( options, random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );

						obj.UniqueIdentifier = 0;

						if( updateVariations )
						{
							obj.VariationGroup = groupIndex;
							obj.VariationElement = elementIndex;
						}
						if( updateTransform )
						{
							if( alignFound )
								obj.Position.Z = alignPositionZ + positionZ;
							obj.Rotation = rotation;
							obj.Scale = scale;
						}


						//Surface.GetRandomVariationOptions options;
						//if( randomizeGroups )
						//	options = new Surface.GetRandomVariationOptions();
						//else
						//	options = new Surface.GetRandomVariationOptions( obj.VariationGroup, null );
						//surface.GetRandomVariation( options, random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );

						//obj.UniqueIdentifier = 0;
						//obj.VariationGroup = groupIndex;
						//obj.VariationElement = elementIndex;

						//if( AutoAlign && scene != null )
						//{
						//	var r = SceneUtility.CalculateObjectPositionZ( scene, groupOfObjects, obj.Position.Z, obj.Position.ToVector2() );
						//	if( r.found )
						//		obj.Position.Z = r.positionZ + positionZ;
						//}
						//obj.Rotation = rotation;
						//obj.Scale = scale;
					}

					//delete and undo to delete
					undoMultiAction.AddAction( new GroupOfObjectsUndo.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true ) );

					//add new data
					var newIndexes = groupOfObjects.ObjectsAdd( newObjects );
					undoMultiAction.AddAction( new GroupOfObjectsUndo.UndoActionCreateDelete( groupOfObjects, newIndexes, true, false ) );
				}
			}
		}

		//public virtual void UpdateAlignment( UndoMultiAction undoMultiAction )
		//{
		//	var random = new FastRandom();

		//	var groupOfObjects = Parent as GroupOfObjects;
		//	if( groupOfObjects != null )
		//	{
		//		var surface = Surface.Value;
		//		if( surface != null )
		//		{
		//			var indexes = GetObjectsOfElement();
		//			var scene = groupOfObjects.FindParent<Scene>();

		//			var newObjects = groupOfObjects.ObjectsGetData( indexes );

		//			for( int n = 0; n < indexes.Count; n++ )
		//			{
		//				var index = indexes[ n ];
		//				ref var obj = ref newObjects[ n ];

		//				double positionZ = 0;

		//				var group = surface.GetGroup( obj.VariationGroup );
		//				if( group != null )
		//				{
		//					//PositionZRange
		//					var positionZRange = group.PositionZRange.Value;
		//					if( positionZRange.Minimum != positionZRange.Maximum )
		//						positionZ = random.Next( positionZRange.Minimum, positionZRange.Maximum );
		//					else
		//						positionZ = positionZRange.Minimum;

		//				}

		//				//Surface.GetRandomVariationOptions options;
		//				//if( randomizeGroups )
		//				//	options = new Surface.GetRandomVariationOptions();
		//				//else
		//				//	options = new Surface.GetRandomVariationOptions( obj.VariationGroup );
		//				//surface.GetRandomVariation( options, random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );

		//				

		//				obj.UniqueIdentifier = 0;
		//				//obj.VariationGroup = groupIndex;
		//				//obj.VariationElement = elementIndex;
		//				if( AutoAlign && scene != null )
		//				{
		//					var r = SceneUtility.CalculateObjectPositionZ( scene, groupOfObjects, obj.Position.Z, obj.Position.ToVector2() );
		//					if( r.found )
		//						obj.Position.Z = r.positionZ + positionZ;
		//				}
		//				//obj.Rotation = rotation;
		//				//obj.Scale = scale;
		//			}

		//			//delete and undo to delete
		//			undoMultiAction.AddAction( new GroupOfObjectsUndo.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true ) );

		//			//add new data
		//			var newIndexes = groupOfObjects.ObjectsAdd( newObjects );
		//			undoMultiAction.AddAction( new GroupOfObjectsUndo.UndoActionCreateDelete( groupOfObjects, newIndexes, true, false ) );
		//		}
		//	}
		//}
#endif

	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//public class GroupOfObjectsElement_Billboard : GroupOfObjectsElement
	//{
	//	[DefaultValue( null )]
	//	public Reference<Material> Material
	//	{
	//		get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
	//		set { if( _material.BeginSet( this, ref value ) ) { try { MaterialChanged?.Invoke( this ); } finally { _material.EndSet(); } } }
	//	}
	//	public event Action<GroupOfObjectsElement_Billboard> MaterialChanged;
	//	ReferenceField<Material> _material = null;
	//}
}
