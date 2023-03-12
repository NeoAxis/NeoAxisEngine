// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// A component to make objects by path of a Curve In Space.
	/// </summary>
	public class CurveInSpaceObjects : Component
	{
		bool needUpdateObjects;
		List<int> createdGroupOfObjectsObjects = new List<int>();
		List<RigidBody> createdRigidBodies = new List<RigidBody>();
		List<ObjectInSpace> createdObjectsInSpace = new List<ObjectInSpace>();
		//List<ObjectInSpace> createdObjects = new List<ObjectInSpace>();

		///////////////////////////////////////////////

		/// <summary>
		/// The assigned mesh to the curve.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Object" )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( ref value ) ) { try { MeshChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> MeshChanged;
		ReferenceField<Mesh> _mesh = null;

		/// <summary>
		/// The assigned object in space to the curve.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Object" )]
		public Reference<ObjectInSpace> ObjectInSpace
		{
			get { if( _objectInSpace.BeginGet() ) ObjectInSpace = _objectInSpace.Get( this ); return _objectInSpace.value; }
			set { if( _objectInSpace.BeginSet( ref value ) ) { try { ObjectInSpaceChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _objectInSpace.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ObjectInSpace"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> ObjectInSpaceChanged;
		ReferenceField<ObjectInSpace> _objectInSpace = null;

		/// <summary>
		/// Replaces all geometries of the mesh by another material.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		[Category( "Object" )]
		public Reference<Material> ReplaceMaterial
		{
			get { if( _replaceMaterial.BeginGet() ) ReplaceMaterial = _replaceMaterial.Get( this ); return _replaceMaterial.value; }
			set
			{
				if( _replaceMaterial.BeginSet( ref value ) )
				{
					try
					{
						ReplaceMaterialChanged?.Invoke( this );

						//!!!!
						NeedUpdateObjects();
						//!!!!need
						//foreach( var obj in createdObjects )
						//{
						//	var meshInSpace = obj as MeshInSpace;
						//	if( meshInSpace != null )
						//		meshInSpace.ReplaceMaterial = _replaceMaterial.value;
						//}
					}
					finally { _replaceMaterial.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReplaceMaterial"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> ReplaceMaterialChanged;
		ReferenceField<Material> _replaceMaterial;

		///// <summary>
		///// Replaces selected geometries of the mesh by another material.
		///// </summary>
		//[Serialize]
		//[Cloneable( CloneType.Deep )]
		//[Category( "Object" )]
		//public ReferenceList<Material> ReplaceMaterialSelectively
		//{
		//	get { return _replaceMaterialSelectively; }
		//}
		//public delegate void ReplaceMaterialSelectivelyChangedDelegate( CurveInSpaceObjects sender );
		//public event ReplaceMaterialSelectivelyChangedDelegate ReplaceMaterialSelectivelyChanged;
		//ReferenceList<Material> _replaceMaterialSelectively;

		/// <summary>
		/// The base color and opacity multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Object" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set
			{
				if( _color.BeginSet( ref value ) )
				{
					try
					{
						ColorChanged?.Invoke( this );

						//!!!!
						NeedUpdateObjects();
						//!!!!need
						//foreach( var obj in createdObjects )
						//{
						//	var meshInSpace = obj as MeshInSpace;
						//	if( meshInSpace != null )
						//		meshInSpace.Color = _color.value;
						//}
					}
					finally { _color.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> ColorChanged;
		ReferenceField<ColorValue> _color = ColorValue.One;

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
				if( _visibilityDistanceFactor.BeginSet( ref value ) )
				{
					try
					{
						VisibilityDistanceFactorChanged?.Invoke( this );

						//!!!!
						NeedUpdateObjects();
						//!!!!need
						//foreach( var obj in createdObjects )
						//{
						//	var meshInSpace = obj as MeshInSpace;
						//	if( meshInSpace != null )
						//		meshInSpace.VisibilityDistance = _visibilityDistance.value;
						//}
					}
					finally { _visibilityDistanceFactor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="VisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> VisibilityDistanceFactorChanged;
		ReferenceField<double> _visibilityDistanceFactor = 1.0;

		///// <summary>
		///// Maximum visibility range of the object.
		///// </summary>
		//[DefaultValue( 10000.0 )]
		//[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//[Category( "Object" )]
		//public Reference<double> VisibilityDistance
		//{
		//	get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
		//	set
		//	{
		//		if( _visibilityDistance.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				VisibilityDistanceChanged?.Invoke( this );

		//				//!!!!
		//				NeedUpdateObjects();
		//				//!!!!need
		//				//foreach( var obj in createdObjects )
		//				//{
		//				//	var meshInSpace = obj as MeshInSpace;
		//				//	if( meshInSpace != null )
		//				//		meshInSpace.VisibilityDistance = _visibilityDistance.value;
		//				//}
		//			}
		//			finally { _visibilityDistance.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		//public event Action<CurveInSpaceObjects> VisibilityDistanceChanged;
		//ReferenceField<double> _visibilityDistance = 10000.0;

		/// <summary>
		/// Whether to cast shadows on the other surfaces.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Object" )]
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

						//!!!!
						NeedUpdateObjects();
						//!!!!need
						//foreach( var obj in createdObjects )
						//{
						//	var meshInSpace = obj as MeshInSpace;
						//	if( meshInSpace != null )
						//		meshInSpace.CastShadows = _castShadows.value;
						//}
					}
					finally { _castShadows.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CastShadows"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> CastShadowsChanged;
		ReferenceField<bool> _castShadows = true;

		/// <summary>
		/// Whether it is possible to apply decals on the surface.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Object" )]
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

						//!!!!
						NeedUpdateObjects();
						//!!!!need
						//foreach( var obj in createdObjects )
						//{
						//	var meshInSpace = obj as MeshInSpace;
						//	if( meshInSpace != null )
						//		meshInSpace.ReceiveDecals = _receiveDecals.value;
						//}
					}
					finally { _receiveDecals.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> ReceiveDecalsChanged;
		ReferenceField<bool> _receiveDecals = true;

		///// <summary>
		///// Specifies settings for special object effects, such as an outline effect.
		///// </summary>
		//[Cloneable( CloneType.Deep )]
		//[Category( "Object" )]
		//public Reference<List<ObjectSpecialRenderingEffect>> SpecialEffects
		//{
		//	get { if( _specialEffects.BeginGet() ) SpecialEffects = _specialEffects.Get( this ); return _specialEffects.value; }
		//	set { if( _specialEffects.BeginSet( ref value ) ) { try { SpecialEffectsChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _specialEffects.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SpecialEffects"/> property value changes.</summary>
		//public event Action<CurveInSpaceObjects> SpecialEffectsChanged;
		//ReferenceField<List<ObjectSpecialRenderingEffect>> _specialEffects = new List<ObjectSpecialRenderingEffect>();

		//[Browsable( false )]
		//public RenderingPipeline.RenderSceneData.LayerItem[] PaintLayersReplace
		//{
		//	get { return paintLayersReplace; }
		//	set
		//	{
		//		paintLayersReplace = value;

		//		//!!!!need
		//		//foreach( var obj in createdObjects )
		//		//{
		//		//	var meshInSpace = obj as MeshInSpace;
		//		//	if( meshInSpace != null )
		//		//		meshInSpace.PaintLayersReplace = paintLayersReplace;
		//		//}
		//	}
		//}
		//RenderingPipeline.RenderSceneData.LayerItem[] paintLayersReplace;

		//[Browsable( false )]
		//public RenderingPipeline.RenderSceneData.CutVolumeItem[] CutVolumes
		//{
		//	get { return cutVolumes; }
		//	set
		//	{
		//		cutVolumes = value;

		//		//!!!!need
		//		//foreach( var obj in createdObjects )
		//		//{
		//		//	var meshInSpace = obj as MeshInSpace;
		//		//	if( meshInSpace != null )
		//		//		meshInSpace.CutVolumes = cutVolumes;
		//		//}
		//	}
		//}
		//RenderingPipeline.RenderSceneData.CutVolumeItem[] cutVolumes;

		public enum StepMeasureEnum
		{
			Distance,
			CurveTime,
		}

		/// <summary>
		/// The measure of the step value.
		/// </summary>
		[DefaultValue( StepMeasureEnum.Distance )]
		[Category( "Distribution" )]
		public Reference<StepMeasureEnum> StepMeasure
		{
			get { if( _stepMeasure.BeginGet() ) StepMeasure = _stepMeasure.Get( this ); return _stepMeasure.value; }
			set { if( _stepMeasure.BeginSet( ref value ) ) { try { StepMeasureChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _stepMeasure.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="StepMeasure"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> StepMeasureChanged;
		ReferenceField<StepMeasureEnum> _stepMeasure = StepMeasureEnum.Distance;

		/// <summary>
		/// The step between objects.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.05, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Distribution" )]
		public Reference<double> Step
		{
			get { if( _step.BeginGet() ) Step = _step.Get( this ); return _step.value; }
			set { if( _step.BeginSet( ref value ) ) { try { StepChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _step.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Step"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> StepChanged;
		ReferenceField<double> _step = 1.0;

		/// <summary>
		/// The offset to objects position.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Transform" )]
		//!!!![Range( -10, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<Vector3> PositionOffset
		{
			get { if( _positionOffset.BeginGet() ) PositionOffset = _positionOffset.Get( this ); return _positionOffset.value; }
			set { if( _positionOffset.BeginSet( ref value ) ) { try { PositionOffsetChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _positionOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionOffset"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> PositionOffsetChanged;
		ReferenceField<Vector3> _positionOffset;

		/// <summary>
		/// The random value to position offset.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Transform" )]
		//!!!![Range( -10, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<Vector3> PositionOffsetRandom
		{
			get { if( _positionOffsetRandom.BeginGet() ) PositionOffsetRandom = _positionOffsetRandom.Get( this ); return _positionOffsetRandom.value; }
			set { if( _positionOffsetRandom.BeginSet( ref value ) ) { try { PositionOffsetRandomChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _positionOffsetRandom.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionOffsetRandom"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> PositionOffsetRandomChanged;
		ReferenceField<Vector3> _positionOffsetRandom;

		/// <summary>
		/// The offset to objects rotation.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Transform" )]
		public Reference<Angles> RotationOffset
		{
			get { if( _rotationOffset.BeginGet() ) RotationOffset = _rotationOffset.Get( this ); return _rotationOffset.value; }
			set { if( _rotationOffset.BeginSet( ref value ) ) { try { RotationOffsetChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _rotationOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotationOffset"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> RotationOffsetChanged;
		ReferenceField<Angles> _rotationOffset = Angles.Zero;

		/// <summary>
		/// The offset to objects rotation depending by step distance or by time.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Transform" )]
		public Reference<Angles> RotationOffsetByTime
		{
			get { if( _rotationOffsetByTime.BeginGet() ) RotationOffsetByTime = _rotationOffsetByTime.Get( this ); return _rotationOffsetByTime.value; }
			set { if( _rotationOffsetByTime.BeginSet( ref value ) ) { try { RotationOffsetByTimeChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _rotationOffsetByTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotationOffsetByTime"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> RotationOffsetByTimeChanged;
		ReferenceField<Angles> _rotationOffsetByTime = Angles.Zero;

		/// <summary>
		/// The random value to rotation offset.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Transform" )]
		public Reference<Angles> RotationOffsetRandom
		{
			get { if( _rotationOffsetRandom.BeginGet() ) RotationOffsetRandom = _rotationOffsetRandom.Get( this ); return _rotationOffsetRandom.value; }
			set { if( _rotationOffsetRandom.BeginSet( ref value ) ) { try { RotationOffsetRandomChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _rotationOffsetRandom.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotationOffsetRandom"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> RotationOffsetRandomChanged;
		ReferenceField<Angles> _rotationOffsetRandom = Angles.Zero;

		/// <summary>
		/// The range of the random scale multiplier.
		/// </summary>
		[DefaultValue( "1 1" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Transform" )]
		public Reference<Range> ScaleRandom
		{
			get { if( _scaleRandom.BeginGet() ) ScaleRandom = _scaleRandom.Get( this ); return _scaleRandom.value; }
			set { if( _scaleRandom.BeginSet( ref value ) ) { try { ScaleRandomChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _scaleRandom.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ScaleRandom"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> ScaleRandomChanged;
		ReferenceField<Range> _scaleRandom = new Range( 1, 1 );

		/// <summary>
		/// The seed for random parameters.
		/// </summary>
		[DefaultValue( 0 )]
		[Range( 0, 100 )]
		[Category( "Transform" )]
		public Reference<int> RandomSeed
		{
			get { if( _randomSeed.BeginGet() ) RandomSeed = _randomSeed.Get( this ); return _randomSeed.value; }
			set { if( _randomSeed.BeginSet( ref value ) ) { try { RandomSeedChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _randomSeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RandomSeed"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> RandomSeedChanged;
		ReferenceField<int> _randomSeed = 0;

		/// <summary>
		/// Whether to have a collision body.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Physics" )]
		public Reference<bool> Collision
		{
			get { if( _collision.BeginGet() ) Collision = _collision.Get( this ); return _collision.value; }
			set { if( _collision.BeginSet( ref value ) ) { try { CollisionChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Collision"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> CollisionChanged;
		ReferenceField<bool> _collision = false;

		/// <summary>
		/// The physical material used by the rigid body.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Physics" )]
		public Reference<PhysicalMaterial> CollisionMaterial
		{
			get { if( _collisionMaterial.BeginGet() ) CollisionMaterial = _collisionMaterial.Get( this ); return _collisionMaterial.value; }
			set { if( _collisionMaterial.BeginSet( ref value ) ) { try { CollisionMaterialChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CollisionMaterial"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> CollisionMaterialChanged;
		ReferenceField<PhysicalMaterial> _collisionMaterial;

		//!!!!
		///// <summary>
		///// The type of friction applied on the rigid body.
		///// </summary>
		//[DefaultValue( PhysicalMaterial.FrictionModeEnum.Simple )]
		//[Category( "Physics" )]
		//public Reference<PhysicalMaterial.FrictionModeEnum> CollisionFrictionMode
		//{
		//	get { if( _collisionFrictionMode.BeginGet() ) CollisionFrictionMode = _collisionFrictionMode.Get( this ); return _collisionFrictionMode.value; }
		//	set { if( _collisionFrictionMode.BeginSet( ref value ) ) { try { CollisionFrictionModeChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionFrictionMode.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CollisionFrictionMode"/> property value changes.</summary>
		//public event Action<CurveInSpaceObjects> CollisionFrictionModeChanged;
		//ReferenceField<PhysicalMaterial.FrictionModeEnum> _collisionFrictionMode = PhysicalMaterial.FrictionModeEnum.Simple;

		/// <summary>
		/// The amount of friction applied on the rigid body.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		[Category( "Physics" )]
		public Reference<double> CollisionFriction
		{
			get { if( _collisionFriction.BeginGet() ) CollisionFriction = _collisionFriction.Get( this ); return _collisionFriction.value; }
			set { if( _collisionFriction.BeginSet( ref value ) ) { try { CollisionFrictionChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionFriction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CollisionFriction"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> CollisionFrictionChanged;
		ReferenceField<double> _collisionFriction = 0.5;

		//!!!!
		///// <summary>
		///// The amount of directional friction applied on the rigid body.
		///// </summary>
		//[DefaultValue( "1 1 1" )]
		//[Category( "Physics" )]
		//public Reference<Vector3> CollisionAnisotropicFriction
		//{
		//	get { if( _collisionAnisotropicFriction.BeginGet() ) CollisionAnisotropicFriction = _collisionAnisotropicFriction.Get( this ); return _collisionAnisotropicFriction.value; }
		//	set { if( _collisionAnisotropicFriction.BeginSet( ref value ) ) { try { CollisionAnisotropicFrictionChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionAnisotropicFriction.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CollisionAnisotropicFriction"/> property value changes.</summary>
		//public event Action<CurveInSpaceObjects> CollisionAnisotropicFrictionChanged;
		//ReferenceField<Vector3> _collisionAnisotropicFriction = Vector3.One;

		//!!!!
		///// <summary>
		///// The amount of friction applied when rigid body is spinning.
		///// </summary>
		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//[Category( "Physics" )]
		//public Reference<double> CollisionSpinningFriction
		//{
		//	get { if( _collisionSpinningFriction.BeginGet() ) CollisionSpinningFriction = _collisionSpinningFriction.Get( this ); return _collisionSpinningFriction.value; }
		//	set { if( _collisionSpinningFriction.BeginSet( ref value ) ) { try { CollisionSpinningFrictionChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionSpinningFriction.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CollisionSpinningFriction"/> property value changes.</summary>
		//public event Action<CurveInSpaceObjects> CollisionSpinningFrictionChanged;
		//ReferenceField<double> _collisionSpinningFriction = 0.5;

		//!!!!
		///// <summary>
		///// The amount of friction applied when rigid body is rolling.
		///// </summary>
		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//[Category( "Physics" )]
		//public Reference<double> CollisionRollingFriction
		//{
		//	get { if( _collisionRollingFriction.BeginGet() ) CollisionRollingFriction = _collisionRollingFriction.Get( this ); return _collisionRollingFriction.value; }
		//	set { if( _collisionRollingFriction.BeginSet( ref value ) ) { try { CollisionRollingFrictionChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionRollingFriction.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CollisionRollingFriction"/> property value changes.</summary>
		//public event Action<CurveInSpaceObjects> CollisionRollingFrictionChanged;
		//ReferenceField<double> _collisionRollingFriction = 0.5;

		/// <summary>
		/// The ratio of the final relative velocity to initial relative velocity of the rigid body after collision.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Category( "Physics" )]
		public Reference<double> CollisionRestitution
		{
			get { if( _collisionRestitution.BeginGet() ) CollisionRestitution = _collisionRestitution.Get( this ); return _collisionRestitution.value; }
			set { if( _collisionRestitution.BeginSet( ref value ) ) { try { CollisionRestitutionChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionRestitution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CollisionRestitution"/> property value changes.</summary>
		public event Action<CurveInSpaceObjects> CollisionRestitutionChanged;
		ReferenceField<double> _collisionRestitution;

		///////////////////////////////////////////////

		public CurveInSpaceObjects()
		{
			//_replaceMaterialSelectively = new ReferenceList<Material>( this, delegate ()
			//{
			//	ReplaceMaterialSelectivelyChanged?.Invoke( this );

			//	//!!!!need
			//	//foreach( var obj in createdObjects )
			//	//{
			//	//	var meshInSpace = obj as MeshInSpace;
			//	//	if( meshInSpace != null )
			//	//	{
			//	//		if( ReplaceMaterialSelectively.Count != 0 )
			//	//		{
			//	//			meshInSpace.ReplaceMaterialSelectively.Clear();
			//	//			for( int n = 0; n < ReplaceMaterialSelectively.Count; n++ )
			//	//				meshInSpace.ReplaceMaterialSelectively.Add( ReplaceMaterialSelectively[ n ] );
			//	//		}
			//	//	}
			//	//}

			//} );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
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

				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
				UpdateObjects();
			else
				DestroyObjects();

			var curve = Parent as CurveInSpace;
			if( curve != null )
			{
				if( EnabledInHierarchyAndIsInstance )
					curve.UpdateObjectsEvent += Curve_UpdateObjectsEvent;
				else
					curve.UpdateObjectsEvent -= Curve_UpdateObjectsEvent;
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( needUpdateObjects )
				UpdateObjects();
		}

		private void Curve_UpdateObjectsEvent( CurveInSpace sender )
		{
			if( EnabledInHierarchyAndIsInstance )
				UpdateObjects();
		}

		[Browsable( false )]
		public CurveInSpace Curve
		{
			get { return Parent as CurveInSpace; }
		}

		//void SetMeshInSpaceParameters( MeshInSpace obj )
		//{
		//	obj.ReplaceMaterial = ReplaceMaterial;
		//	//if( ReplaceMaterialSelectively.Count != 0 )
		//	//{
		//	//	obj.ReplaceMaterialSelectively.Clear();
		//	//	for( int n = 0; n < ReplaceMaterialSelectively.Count; n++ )
		//	//		obj.ReplaceMaterialSelectively.Add( ReplaceMaterialSelectively[ n ] );
		//	//}
		//	obj.Color = Color;
		//	obj.VisibilityDistance = VisibilityDistance;
		//	obj.CastShadows = CastShadows;
		//	obj.ReceiveDecals = ReceiveDecals;
		//	//obj.SpecialEffects = SpecialEffects;
		//	//obj.PaintLayersReplace = PaintLayersReplace;
		//	//obj.CutVolumes = CutVolumes;
		//}

		public delegate void CreateObjectEventDelegate( CurveInSpaceObjects sender, double stepDistance, ref Vector3 position, ref Quaternion rotation, ref Vector3 scale, ref bool skip );
		public event CreateObjectEventDelegate CreateObjectEvent;

		void CreateObject( CurveInSpace curve, FastRandom random, double stepDistance, Vector3 position, Quaternion rotation, Vector3 scale, ref GroupOfObjects groupOfObjects, ref ushort? groupOfObjectsElementIndex )
		{
			var pos = position;
			var rot = rotation;
			var scl = scale;

			//modify transform

			//position offset
			var positionOffset = PositionOffset.Value;
			{
				var positionOffsetRandom = PositionOffsetRandom.Value;
				for( int n = 0; n < 3; n++ )
				{
					if( positionOffsetRandom[ n ] != 0 )
						positionOffset[ n ] += random.Next( -positionOffsetRandom[ n ], positionOffsetRandom[ n ] );
				}
			}
			if( positionOffset != Vector3.Zero )
				pos += rotation * positionOffset;

			//rotation offset
			var rotationOffset = RotationOffset.Value;
			{
				var rotationOffsetByTime = RotationOffsetByTime.Value;
				for( int n = 0; n < 3; n++ )
				{
					if( rotationOffsetByTime[ n ] != 0 )
						rotationOffset[ n ] += rotationOffsetByTime[ n ] * stepDistance;
				}

				var rotationOffsetRandom = RotationOffsetRandom.Value;
				for( int n = 0; n < 3; n++ )
				{
					if( rotationOffsetRandom[ n ] != 0 )
						rotationOffset[ n ] += random.Next( -rotationOffsetRandom[ n ], rotationOffsetRandom[ n ] );
				}
			}
			if( rotationOffset != Angles.Zero )
				rot *= rotationOffset.ToQuaternion();

			//random scale
			var randomScale = ScaleRandom.Value;
			if( randomScale != new Range( 1, 1 ) )
				scl *= random.Next( randomScale.Minimum, randomScale.Maximum );

			var skip = false;
			CreateObjectEvent?.Invoke( this, stepDistance, ref pos, ref rot, ref scl, ref skip );
			if( skip )
				return;

			var transform = new Transform( pos, rot, scl );


			var mesh = Mesh.Value;
			if( mesh != null )
			{
				//create mesh object
				unsafe
				{
					if( groupOfObjects == null )
						groupOfObjects = GetOrCreateGroupOfObjects( true );

					if( groupOfObjects != null )
					{
						if( !groupOfObjectsElementIndex.HasValue )
							groupOfObjectsElementIndex = GetOrCreateGroupOfObjectsElement( groupOfObjects );//, mesh, ReplaceMaterial );

						var p = transform.Position;
						var r = transform.Rotation.ToQuaternionF();
						var s = transform.Scale.ToVector3F();

						var obj = new GroupOfObjects.Object( groupOfObjectsElementIndex.Value, 0, 0, GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible, p, r, s, Vector4F.Zero, Color, Vector4F.Zero, Vector4F.Zero, 0 );

						//!!!!может обновление сразу вызывается, тогда не круто
						var objects = groupOfObjects.ObjectsAdd( &obj, 1 );
						createdGroupOfObjectsObjects.AddRange( objects );
					}
				}

				////need set ShowInEditor = false before AddComponent
				//var obj = ComponentUtility.CreateComponent<MeshInSpace>( null, false, false );
				//obj.DisplayInEditor = false;
				//AddComponent( obj, -1 );
				////var obj = CreateComponent<MeshInSpace>( enabled: false );

				//obj.SaveSupport = false;
				//obj.CloneSupport = false;
				//obj.CanBeSelected = false;
				//obj.ScreenLabel = ScreenLabelEnum.NeverDisplay;

				//obj.Transform = transform;
				//obj.Mesh = Mesh;
				//SetMeshInSpaceParameters( obj );
				//obj.Enabled = true;

				//createdObjects.Add( obj );

				if( Collision )
				{
					//!!!!use BodyItem

					RigidBody body;

					var obj = this;

					//!!!!slowly?

					var collisionDefinition = mesh.GetComponent( "Collision Definition" ) as RigidBody;
					if( collisionDefinition == null )
						collisionDefinition = mesh?.Result.GetAutoCollisionDefinition();

					if( collisionDefinition != null )
					{
						body = (RigidBody)collisionDefinition.Clone();
						body.Enabled = false;
						body.NetworkMode = NetworkModeEnum.False;
						obj.AddComponent( body );

						body.MotionType = PhysicsMotionType.Static;
					}
					else
					{
						//!!!!need?

						body = obj.CreateComponent<RigidBody>( enabled: false, networkMode: NetworkModeEnum.False );

						var shape = body.CreateComponent<CollisionShape_Mesh>();
						shape.Mesh = ReferenceUtility.MakeThisReference( shape, obj, "Mesh" );
					}

					body.SaveSupport = false;
					body.CloneSupport = false;
					body.CanBeSelected = false;
					body.ScreenLabel = ScreenLabelEnum.NeverDisplay;
					body.Transform = transform;

					UpdateCollisionMaterial( body );

					body.Enabled = true;

					createdRigidBodies.Add( body );
				}
			}

			var sourceObject = ObjectInSpace.Value;
			if( sourceObject != null )
			{
				var obj = (ObjectInSpace)sourceObject.Clone();
				obj.Enabled = false;
				obj.DisplayInEditor = false;
				AddComponent( obj, -1 );

				obj.SaveSupport = false;
				obj.CloneSupport = false;
				obj.CanBeSelected = false;
				obj.ScreenLabel = ScreenLabelEnum.NeverDisplay;

				obj.Transform = transform;
				//var meshInSpace = obj as MeshInSpace;
				//if( meshInSpace != null )
				//	SetMeshInSpaceParameters( meshInSpace );

				obj.Enabled = true;

				createdObjectsInSpace.Add( obj );
				//createdObjects.Add( obj );
			}
		}

		public void UpdateObjects()
		{
			needUpdateObjects = false;

			DestroyObjects();

			if( EnabledInHierarchyAndIsInstance )
			{
				var curve = Curve;
				if( curve != null && ( Mesh.ReferenceSpecified || ObjectInSpace.ReferenceSpecified ) )
				{
					var data = curve.GetData();
					if( data != null )
					{
						var random = new FastRandom( RandomSeed );
						//var randomScale = RandomScale.Value;

						GroupOfObjects groupOfObjects = null;
						ushort? groupOfObjectsElementIndex = null;

						switch( StepMeasure.Value )
						{
						case StepMeasureEnum.Distance:
							{
								var segmentsLength = Step.Value;

								var points = data.SourceData.Points;

								double totalLengthNoCurvature = 0;
								{
									for( int n = 1; n < points.Length; n++ )
										totalLengthNoCurvature += ( points[ n ].Transform.Position - points[ n - 1 ].Transform.Position ).Length();
									if( totalLengthNoCurvature <= 0.001 )
										totalLengthNoCurvature = 0.001;
								}

								//generate time steps
								List<double> timeSteps;
								{
									//this is precision of step
									var stepDivide = 10.0;//5.0;

									timeSteps = new List<double>( (int)( totalLengthNoCurvature / segmentsLength * stepDivide * 2 ) );

									for( int nPoint = 0; nPoint < points.Length - 1; nPoint++ )
									{
										var pointFrom = points[ nPoint ];
										var pointTo = points[ nPoint + 1 ];
										var timeFrom = (double)nPoint;
										var timeTo = (double)nPoint + 1;

										var length = ( pointTo.Transform.Position - pointFrom.Transform.Position ).Length();
										var timeStep = segmentsLength / length / stepDivide;

										var timeEnd = timeTo - timeStep * 0.1;
										for( double time = timeFrom; time < timeEnd; time += timeStep )
											timeSteps.Add( time );
									}
									timeSteps.Add( points.Length - 1 );
								}

								{
									double currentLength = 0;
									var previousPosition = data.GetPositionByNormalizedTime( 0 );
									var previousVector = ( data.GetPositionByNormalizedTime( 0.01 ) - previousPosition ).GetNormalize();
									var currentUp = points[ 0 ].Transform.Rotation.GetUp();
									if( currentUp.Equals( previousVector, 0.01 ) )
										currentUp = points[ 0 ].Transform.Rotation.GetLeft();

									double stepRemaining = 0;

									for( int nTime = 0; nTime < timeSteps.Count; nTime++ )
									{
										var time = timeSteps[ nTime ];

										var position = data.GetPositionByNormalizedTime( time );

										var vector = position - previousPosition;
										double stepLength = 0;
										if( vector != Vector3.Zero )
											stepLength = vector.Normalize();
										else
											vector = previousVector;

										if( currentUp.Equals( vector, 0.01 ) )
											currentUp = Quaternion.FromDirectionZAxisUp( vector ).GetUp();
										var rotation = Quaternion.LookAt( vector, currentUp );

										var scale = data.GetScaleByNormalizedTime( time );

										stepRemaining -= stepLength;

										var skip = stepRemaining > 0;
										if( !skip )
										{
											CreateObject( curve, random, currentLength, position, rotation, scale, ref groupOfObjects, ref groupOfObjectsElementIndex );

											stepRemaining += Step.Value;
										}

										currentLength += stepLength;
										previousPosition = position;
										previousVector = vector;
										currentUp = rotation.GetUp();
									}
								}
							}
							break;

						case StepMeasureEnum.CurveTime:
							{
								var range = curve.GetTimeRange();
								for( double t = range.Minimum; t <= range.Maximum; t += Step )
								{
									var tr = curve.GetTransformByTime( t );
									CreateObject( curve, random, t, tr.Position, tr.Rotation, tr.Scale, ref groupOfObjects, ref groupOfObjectsElementIndex );
								}
							}
							break;
						}
					}
				}
			}
		}

		void DestroyObjects()
		{
			if( createdGroupOfObjectsObjects.Count != 0 )
			{
				var group = GetOrCreateGroupOfObjects( false );
				if( group != null )
				{
					//!!!!может обновление сразу вызывается, тогда не круто
					group.ObjectsRemove( createdGroupOfObjectsObjects.ToArray() );

					group.NeedRemoveEmptyElements();
				}
				createdGroupOfObjectsObjects.Clear();
			}

			foreach( var body in createdRigidBodies )
				body.Dispose();
			createdRigidBodies.Clear();

			foreach( var obj in createdObjectsInSpace )
				obj.Dispose();
			createdObjectsInSpace.Clear();

			//foreach( var obj in createdObjects )
			//	obj.Dispose();
			//createdObjects.Clear();
		}

		void UpdateCollisionMaterial( RigidBody body )
		{
			body.Material = CollisionMaterial;
			//!!!!
			//body.MaterialFrictionMode = CollisionFrictionMode;
			body.MaterialFriction = CollisionFriction;
			//!!!!
			//body.MaterialAnisotropicFriction = CollisionAnisotropicFriction;
			//body.MaterialSpinningFriction = CollisionSpinningFriction;
			//body.MaterialRollingFriction = CollisionRollingFriction;
			body.MaterialRestitution = CollisionRestitution;
		}

		void UpdateCollisionMaterial()
		{
			foreach( var body in createdRigidBodies )
				UpdateCollisionMaterial( body );

			//foreach( var obj in createdObjects )
			//{
			//	if( obj is MeshInSpace )
			//	{
			//		var body = obj.GetComponent<RigidBody>();
			//		if( body != null )
			//			UpdateCollisionMaterial( body );
			//	}
			//}
		}

		public void NeedUpdateObjects()
		{
			needUpdateObjects = true;
		}

		GroupOfObjects GetOrCreateGroupOfObjects( bool canCreate )
		{
			var scene = Curve?.ParentScene;
			if( scene == null )
				return null;

			var name = "__GroupOfObjectsCurveInSpaceObjects";

			var group = scene.GetComponent<GroupOfObjects>( name );
			if( group == null && canCreate )
			{
				//need set ShowInEditor = false before AddComponent
				group = ComponentUtility.CreateComponent<GroupOfObjects>( null, false, false );
				group.NetworkMode = NetworkModeEnum.False;
				group.DisplayInEditor = false;
				scene.AddComponent( group, -1 );
				//var group = scene.CreateComponent<GroupOfObjects>();

				group.Name = name;
				group.SaveSupport = false;
				group.CloneSupport = false;

				//group.AnyData = new Dictionary<(Mesh, Material), int>();

				//!!!!параметром
				group.SectorSize = new Vector3( 50, 50, 10000 );

				group.Enabled = true;
			}

			return group;
		}

		ushort GetOrCreateGroupOfObjectsElement( GroupOfObjects group )//, Mesh mesh, Material replaceMaterial )
		{
			//var key = (mesh, replaceMaterial);

			//var dictionary = (Dictionary<(Mesh mesh, Material), int>)group.AnyData;

			GroupOfObjectsElement_Mesh element = null;

			//if( dictionary.TryGetValue( key, out var elementIndex2 ) )
			//	return (ushort)elementIndex2;

			var elements = group.GetComponents<GroupOfObjectsElement_Mesh>();
			foreach( var e in elements )
			{
				if( e.Mesh.Value == Mesh.Value && e.ReplaceMaterial.Value == ReplaceMaterial.Value && e.VisibilityDistanceFactor.Value == VisibilityDistanceFactor.Value && e.CastShadows.Value == CastShadows.Value && e.ReceiveDecals.Value == ReceiveDecals.Value )
				{
					element = e;
					break;
				}
			}

			if( element == null )
			{
				var elementIndex = group.GetFreeElementIndex();
				element = group.CreateComponent<GroupOfObjectsElement_Mesh>( enabled: false );
				element.Name = "Element " + elementIndex.ToString();
				element.Index = elementIndex;
				element.Mesh = Mesh;
				element.ReplaceMaterial = ReplaceMaterial;
				element.VisibilityDistanceFactor = VisibilityDistanceFactor;
				element.CastShadows = CastShadows;
				element.ReceiveDecals = ReceiveDecals;
				element.AutoAlign = false;
				element.Enabled = true;

				//dictionary[ key ] = element.Index;

				group.ElementTypesCacheNeedUpdate();
			}

			return (ushort)element.Index.Value;
		}
	}
}
