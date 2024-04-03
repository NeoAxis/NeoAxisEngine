// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Soft physical body.
	/// </summary>
	[AddToResourcesWindow( @"Base\Physics\Soft Body", 0 )]
	public class SoftBody : PhysicalBody//, ISoftBody
	{
		//!!!!так? что с пересечением имен? или делать не чилдом?
		const string createMeshName = "Mesh Created";

		//!!!!
		//Internal.BulletSharp.SoftBody.SoftBody softBody;
		Mesh.CompiledData usedMeshDataWhenInitialized;
		//!!!!Matrix4 actualTransform;

		//!!!!bool updatePropertiesWithoutUpdatingBody;
		bool duringCreateDestroy;

		Vector3F[] processedVertices;
		//Vec3F[] processedVerticesNormals;
		int[] processedIndices;
		int[] processedTrianglesToSourceIndex;

		Vector3F[] simulatedVertices;
		////Vec3F[] simulatedVerticesNormals;

		/////////////////////////////////////////

		/// <summary>
		/// The mesh used by the soft body.
		/// </summary>
		[Serialize]
		[Category( "Construction" )]
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
						RecreateBody();
					}
					finally { _mesh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<SoftBody> MeshChanged;
		ReferenceField<Mesh> _mesh;

		///// <summary>
		///// Creates constraints in the soft body system that control how much the joints at each vertex can bend.
		///// </summary>
		//[DefaultValue( true )]
		//[Serialize]
		//[Category( "Construction" )]
		//public Reference<bool> BendingConstraints
		//{
		//	get { if( _bendingConstraints.BeginGet() ) BendingConstraints = _bendingConstraints.Get( this ); return _bendingConstraints.value; }
		//	set
		//	{
		//		if( _bendingConstraints.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				BendingConstraintsChanged?.Invoke( this );
		//				RecreateBody();
		//			}
		//			finally { _bendingConstraints.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<SoftBody> BendingConstraintsChanged;
		//ReferenceField<bool> _bendingConstraints = true;

		/// <summary>
		/// Whether to randomize constraints to reduce solver bias.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "Construction" )]
		public Reference<bool> RandomizeConstraints
		{
			get { if( _randomizeConstraints.BeginGet() ) RandomizeConstraints = _randomizeConstraints.Get( this ); return _randomizeConstraints.value; }
			set
			{
				if( _randomizeConstraints.BeginSet( this, ref value ) )
				{
					try
					{
						RandomizeConstraintsChanged?.Invoke( this );
						RecreateBody();
					}
					finally { _randomizeConstraints.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="RandomizeConstraints"/> property value changes.</summary>
		public event Action<SoftBody> RandomizeConstraintsChanged;
		ReferenceField<bool> _randomizeConstraints = true;

		/// <summary>
		/// The distance between two vertices that produce tension on a surface. Greater distance produces softer physical body.
		/// </summary>
		[DefaultValue( 2 )]
		[Range( 0, 16 )]
		[Serialize]
		[Category( "Construction" )]
		public Reference<int> BendingConstraintDistance
		{
			get { if( _bendingConstraintDistance.BeginGet() ) BendingConstraintDistance = _bendingConstraintDistance.Get( this ); return _bendingConstraintDistance.value; }
			set
			{
				if( _bendingConstraintDistance.BeginSet( this, ref value ) )
				{
					try
					{
						BendingConstraintDistanceChanged?.Invoke( this );
						RecreateBody();
					}
					finally { _bendingConstraintDistance.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BendingConstraintDistance"/> property value changes.</summary>
		public event Action<SoftBody> BendingConstraintDistanceChanged;
		ReferenceField<int> _bendingConstraintDistance = 2;

		//public enum ClusterModeEnum
		//{
		//	Manual,
		//	//Auto,
		//	ForEachTriangle,
		//}

		///// <summary>
		///// Generate clusters (K-mean)
		///// generateClusters with k=0 will create a convex cluster for each tetrahedron or triangle
		///// otherwise an approximation will be used (better performance).
		///// </summary>
		//[DefaultValue( ClusterModeEnum.Manual )]
		//[Serialize]
		//[Category( "Construction" )]
		//public Reference<ClusterModeEnum> GenerateClusters
		//{
		//	get { if( _generateClusters.BeginGet() ) GenerateClusters = _generateClusters.Get( this ); return _generateClusters.value; }
		//	set
		//	{
		//		if( _generateClusters.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				GenerateClustersChanged?.Invoke( this );
		//				RecreateBody();
		//			}
		//			finally { _generateClusters.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<SoftBody> GenerateClustersChanged;
		//ReferenceField<ClusterModeEnum> _generateClusters = ClusterModeEnum.Manual;

		/// <summary>
		/// The number of collision clusters generated.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 1, 256, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Serialize]
		[Category( "Construction" )]
		public Reference<int> ClustersNumber
		{
			get { if( _clustersNumber.BeginGet() ) ClustersNumber = _clustersNumber.Get( this ); return _clustersNumber.value; }
			set
			{
				if( _clustersNumber.BeginSet( this, ref value ) )
				{
					try
					{
						ClustersNumberChanged?.Invoke( this );
						RecreateBody();
					}
					finally { _clustersNumber.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ClustersNumber"/> property value changes.</summary>
		public event Action<SoftBody> ClustersNumberChanged;
		ReferenceField<int> _clustersNumber = 1;

		/// <summary>
		/// Causes the solver to detect and resolve collisions between different parts of the same soft body, such as when a patch of cloth folds back on itself. 
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "Construction" )]
		public Reference<bool> SelfCollision
		{
			get { if( _selfCollision.BeginGet() ) SelfCollision = _selfCollision.Get( this ); return _selfCollision.value; }
			set
			{
				if( _selfCollision.BeginSet( this, ref value ) )
				{
					try
					{
						SelfCollisionChanged?.Invoke( this );

						//!!!!
						RecreateBody();
						//if( softBody != null )
						//{
						//	if( value )
						//		softBody.Cfg.Collisions |= Collisions.ClusterSelf;
						//	else
						//		softBody.Cfg.Collisions &= ~Collisions.ClusterSelf;

						//	// Must regenerate clusters after changing collision options
						//	if( GenerateClusters )
						//		softBody.GenerateClusters( ClustersNumber );
						//}
					}
					finally { _selfCollision.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SelfCollision"/> property value changes.</summary>
		public event Action<SoftBody> SelfCollisionChanged;
		ReferenceField<bool> _selfCollision = true;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The mass of the soft body.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[Serialize]
		[Category( "Mass" )]
		public Reference<double> Mass
		{
			get { if( _mass.BeginGet() ) Mass = _mass.Get( this ); return _mass.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _mass.BeginSet( this, ref value ) )
				{
					try
					{
						MassChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.SetTotalMass( value, MassFromFaces );
					}
					finally { _mass.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mass"/> property value changes.</summary>
		public event Action<SoftBody> MassChanged;
		ReferenceField<double> _mass = 1.0;

		//!!!!masses при созданиия SoftBody
		//!!!!what is it?
		//!!!!default value
		/// <summary>
		/// Whether to use mass scaling factor for each vertex.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Mass" )]
		public Reference<bool> MassFromFaces
		{
			get { if( _massFromFaces.BeginGet() ) MassFromFaces = _massFromFaces.Get( this ); return _massFromFaces.value; }
			set
			{
				if( _massFromFaces.BeginSet( this, ref value ) )
				{
					try
					{
						MassFromFacesChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.SetTotalMass( Mass, value );
					}
					finally { _massFromFaces.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MassFromFaces"/> property value changes.</summary>
		public event Action<SoftBody> MassFromFacesChanged;
		ReferenceField<bool> _massFromFaces = false;

		/// <summary>
		/// The damping factor applied to the overall motion of the soft body. Too much damping prevents the soft body from moving.
		/// </summary>
		[DefaultValue( 0.01 )]
		[Range( 0.0, 1.0, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Serialize]
		[Category( "Mass" )]
		public Reference<double> Damping
		{
			get { if( _damping.BeginGet() ) Damping = _damping.Get( this ); return _damping.value; }
			set
			{
				if( _damping.BeginSet( this, ref value ) )
				{
					try
					{
						DampingChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.Damping = value;
					}
					finally { _damping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Damping"/> property value changes.</summary>
		public event Action<SoftBody> DampingChanged;
		ReferenceField<double> _damping = 0.01;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//Aerodynamics
		//These attributes affect how the soft body reacts to the air around it.
		//Note: To affect Soft Body objects by any Wind settings, the Aerodynamics values must be set to something other than zero.

		/// <summary>
		/// Determines how strongly a closed soft body, such as a sphere, maintains its shape and volume. High Pressure values cause the body to expand, while low Pressure values cause it to collapse. 
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( -10.0, 10.0 )]//!!!!так?
		[Serialize]
		[Category( "Aerodynamics" )]
		public Reference<double> Pressure
		{
			get { if( _pressure.BeginGet() ) Pressure = _pressure.Get( this ); return _pressure.value; }
			set
			{
				if( _pressure.BeginSet( this, ref value ) )
				{
					try
					{
						PressureChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.Pressure = value;
					}
					finally { _pressure.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Pressure"/> property value changes.</summary>
		public event Action<SoftBody> PressureChanged;
		ReferenceField<double> _pressure = 0.0;

		/// <summary>
		/// Controls the amount of drag acting on the soft body as it moves through the air.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0.0, 10.0 )]//!!!!так?
		[Serialize]
		[Category( "Aerodynamics" )]
		public Reference<double> Drag
		{
			get { if( _drag.BeginGet() ) Drag = _drag.Get( this ); return _drag.value; }
			set
			{
				if( _drag.BeginSet( this, ref value ) )
				{
					try
					{
						DragChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.Drag = value;
					}
					finally { _drag.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Drag"/> property value changes.</summary>
		public event Action<SoftBody> DragChanged;
		ReferenceField<double> _drag;

		/// <summary>
		/// Controls the amount of lift that is generated as the soft body moves through the air.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0.0, 1.0 )]//!!!!так?
		[Serialize]
		[Category( "Aerodynamics" )]
		public Reference<double> Lift
		{
			get { if( _lift.BeginGet() ) Lift = _lift.Get( this ); return _lift.value; }
			set
			{
				if( _lift.BeginSet( this, ref value ) )
				{
					try
					{
						LiftChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.Lift = value;
					}
					finally { _lift.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Lift"/> property value changes.</summary>
		public event Action<SoftBody> LiftChanged;
		ReferenceField<double> _lift;

		public enum AeroModel
		{
			VertexPoint,
			VertexTwoSided,
			VertexTwoSidedLiftDrag,
			VertexOneSided,
			FaceTwoSided,
			FaceTwoSidedLiftDrag,
			FaceOneSided
		}

		/// <summary>
		/// The aero model of the soft body.
		/// </summary>
		[DefaultValue( AeroModel.VertexPoint )]
		[Serialize]
		[Category( "Aerodynamics" )]
		public Reference<AeroModel> BodyAeroModel
		{
			get { if( _aeroModel.BeginGet() ) BodyAeroModel = _aeroModel.Get( this ); return _aeroModel.value; }
			set
			{
				if( _aeroModel.BeginSet( this, ref value ) )
				{
					try
					{
						AeroModelChanged?.Invoke( this );
					}
					finally { _aeroModel.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AeroModel"/> property value changes.</summary>
		public event Action<SoftBody> AeroModelChanged;
		ReferenceField<AeroModel> _aeroModel = AeroModel.VertexPoint;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//Pose Matching
		//These attributes let you setup the soft body pose matching system, which works on closed shapes.You can use any combination of shape and volume matching on a single soft body.

		//!!!!default
		/// <summary>
		/// When activated, the soft body tries to maintain its original shape.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "Pose Matching" )]
		public Reference<bool> ShapeMatching
		{
			get { if( _shapeMatching.BeginGet() ) ShapeMatching = _shapeMatching.Get( this ); return _shapeMatching.value; }
			set
			{
				if( _shapeMatching.BeginSet( this, ref value ) )
				{
					try
					{
						ShapeMatchingChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.SetPose( VolumeMatching, value );
					}
					finally { _shapeMatching.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ShapeMatching"/> property value changes.</summary>
		public event Action<SoftBody> ShapeMatchingChanged;
		ReferenceField<bool> _shapeMatching = true;

		//!!!!default
		/// <summary>
		/// Pose matching coefficient [0,1]. Specifies the degree to which the rigid body attempts to maintain its shape. Higher Shape Coefficient values cause the rigid body to adhere more strictly to its original shape. If the values are too high, the soft body may penetrate or even fall through other objects. 
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Pose Matching" )]
		public Reference<double> ShapeCoefficient
		{
			get { if( _shapeCoefficient.BeginGet() ) ShapeCoefficient = _shapeCoefficient.Get( this ); return _shapeCoefficient.value; }
			set
			{
				if( _shapeCoefficient.BeginSet( this, ref value ) )
				{
					try
					{
						ShapeCoefficientChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.PoseMatching = value;
					}
					finally { _shapeCoefficient.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ShapeCoefficient"/> property value changes.</summary>
		public event Action<SoftBody> ShapeCoefficientChanged;
		ReferenceField<double> _shapeCoefficient = 0.0;

		/// <summary>
		/// When on, the soft body tries to maintain its original volume.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Pose Matching" )]
		public Reference<bool> VolumeMatching
		{
			get { if( _volumeMatching.BeginGet() ) VolumeMatching = _volumeMatching.Get( this ); return _volumeMatching.value; }
			set
			{
				if( _volumeMatching.BeginSet( this, ref value ) )
				{
					try
					{
						VolumeMatchingChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.SetPose( value, ShapeMatching );
					}
					finally { _volumeMatching.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="VolumeMatching"/> property value changes.</summary>
		public event Action<SoftBody> VolumeMatchingChanged;
		ReferenceField<bool> _volumeMatching;

		//!!!!default
		/// <summary>
		/// Specifies the degree to which the rigid body attempts to maintain its initial volume. The higher Volume Coefficient value, the more the soft body resists changes to its original volume. High Volume Coefficient values can cause the simulation to become unstable. 
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0.0, 100.0 )]
		[Serialize]
		[Category( "Pose Matching" )]
		public Reference<double> VolumeCoefficient
		{
			get { if( _volumeCoefficient.BeginGet() ) VolumeCoefficient = _volumeCoefficient.Get( this ); return _volumeCoefficient.value; }
			set
			{
				if( _volumeCoefficient.BeginSet( this, ref value ) )
				{
					try
					{
						VolumeCoefficientChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.VolumeConversation = value;
					}
					finally { _volumeCoefficient.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="VolumeCoefficient"/> property value changes.</summary>
		public event Action<SoftBody> VolumeCoefficientChanged;
		ReferenceField<double> _volumeCoefficient = 0.0;

		/// <summary>
		/// Max volume ratio for volume-based pose matching.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]//!!!!
		[Serialize]
		[Category( "Pose Matching" )]
		public Reference<double> MaxVolumeRatio
		{
			get { if( _maxVolumeRatio.BeginGet() ) MaxVolumeRatio = _maxVolumeRatio.Get( this ); return _maxVolumeRatio.value; }
			set
			{
				if( _maxVolumeRatio.BeginSet( this, ref value ) )
				{
					try
					{
						MaxVolumeRatioChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.MaxVolume = value;
					}
					finally { _maxVolumeRatio.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaxVolumeRatio"/> property value changes.</summary>
		public event Action<SoftBody> MaxVolumeRatioChanged;
		ReferenceField<double> _maxVolumeRatio = 1.0;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The physical material used by the soft body.
		/// </summary>
		[DefaultValue( null )]
		[Serialize]
		[Category( "Material" )]
		[Browsable( false )]//!!!!disabled until all parameters fixed (SoftRigidContactHardness, etc).
		public Reference<PhysicalMaterial> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set
			{
				if( _material.BeginSet( this, ref value ) )
				{
					try
					{
						MaterialChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	SetMaterial( softBody );
					}
					finally { _material.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<SoftBody> MaterialChanged;
		ReferenceField<PhysicalMaterial> _material = null;

		//!!!!default. в буллете по дефолту 0.2
		/// <summary>
		/// The friction of the physical material.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> MaterialFriction
		{
			get { if( _materialFriction.BeginGet() ) MaterialFriction = _materialFriction.Get( this ); return _materialFriction.value; }
			set
			{
				if( _materialFriction.BeginSet( this, ref value ) )
				{
					try
					{
						MaterialFrictionChanged?.Invoke( this );
						//!!!!
						//if( softBody != null && Material.Value == null )
						//	softBody.Cfg.DynamicFriction = value;
					}
					finally { _materialFriction.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialFriction"/> property value changes.</summary>
		public event Action<SoftBody> MaterialFrictionChanged;
		ReferenceField<double> _materialFriction = 0.5;

		/// <summary>
		/// The stiffness of the physical material. The stiffness decides how hard the soft body will be during the collision.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> MaterialLinearStiffness
		{
			get { if( _materialLinearStiffness.BeginGet() ) MaterialLinearStiffness = _materialLinearStiffness.Get( this ); return _materialLinearStiffness.value; }
			set
			{
				if( _materialLinearStiffness.BeginSet( this, ref value ) )
				{
					try
					{
						MaterialLinearStiffnessChanged?.Invoke( this );
						//!!!!
						//if( softBody != null && Material.Value == null )
						//{
						//	//!!!!пока один материал. везде так
						//	if( softBody.Materials[ 0 ] != null )
						//		softBody.Materials[ 0 ].LinearStiffness = value;
						//}
					}
					finally { _materialLinearStiffness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialLinearStiffness"/> property value changes.</summary>
		public event Action<SoftBody> MaterialLinearStiffnessChanged;
		ReferenceField<double> _materialLinearStiffness = 1.0;

		/// <summary>
		/// The angular stiffness of the physical material.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> MaterialAngularStiffness
		{
			get { if( _materialAngularStiffness.BeginGet() ) MaterialAngularStiffness = _materialAngularStiffness.Get( this ); return _materialAngularStiffness.value; }
			set
			{
				if( _materialAngularStiffness.BeginSet( this, ref value ) )
				{
					try
					{
						MaterialAngularStiffnessChanged?.Invoke( this );
						//!!!!
						//if( softBody != null && Material.Value == null )
						//{
						//	if( softBody.Materials[ 0 ] != null )
						//		softBody.Materials[ 0 ].AngularStiffness = value;
						//}
					}
					finally { _materialAngularStiffness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialAngularStiffness"/> property value changes.</summary>
		public event Action<SoftBody> MaterialAngularStiffnessChanged;
		ReferenceField<double> _materialAngularStiffness = 1.0;

		/// <summary>
		/// The stiffness volume of the physical material.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> MaterialVolumeStiffness
		{
			get { if( _materialVolumeStiffness.BeginGet() ) MaterialVolumeStiffness = _materialVolumeStiffness.Get( this ); return _materialVolumeStiffness.value; }
			set
			{
				if( _materialVolumeStiffness.BeginSet( this, ref value ) )
				{
					try
					{
						MaterialVolumeStiffnessChanged?.Invoke( this );
						//!!!!
						//if( softBody != null && Material.Value == null )
						//{
						//	if( softBody.Materials[ 0 ] != null )
						//		softBody.Materials[ 0 ].VolumeStiffness = value;
						//}
					}
					finally { _materialVolumeStiffness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialVolumeStiffness"/> property value changes.</summary>
		public event Action<SoftBody> MaterialVolumeStiffnessChanged;
		ReferenceField<double> _materialVolumeStiffness = 1.0;

		// Contacts
		// these attributes determine the degree to which the solver resists changes to the soft body's shape when it comes in contact with various types of objects

		/// <summary>
		/// The amount of penetration correction applied to contacts with rigid bodies.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> RigidContactHardness
		{
			get { if( _rigidContactHardness.BeginGet() ) RigidContactHardness = _rigidContactHardness.Get( this ); return _rigidContactHardness.value; }
			set
			{
				if( _rigidContactHardness.BeginSet( this, ref value ) )
				{
					try
					{
						RigidContactHardnessChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.RigidContactHardness = value;
					}
					finally { _rigidContactHardness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="RigidContactHardness"/> property value changes.</summary>
		public event Action<SoftBody> RigidContactHardnessChanged;
		ReferenceField<double> _rigidContactHardness = 1.0;

		/// <summary>
		/// Soft vs rigid hardness [0,1] (cluster only).
		/// </summary>
		[DefaultValue( 0.1 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> SoftRigidContactHardness
		{
			get { if( _softRigidContactHardness.BeginGet() ) SoftRigidContactHardness = _softRigidContactHardness.Get( this ); return _softRigidContactHardness.value; }
			set
			{
				if( _softRigidContactHardness.BeginSet( this, ref value ) )
				{
					try
					{
						SoftRigidContactHardnessChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.SoftRigidHardness = value;
					}
					finally { _softRigidContactHardness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SoftRigidContactHardness"/> property value changes.</summary>
		public event Action<SoftBody> SoftRigidContactHardnessChanged;
		ReferenceField<double> _softRigidContactHardness = 0.1;

		/// <summary>
		/// The amount of penetration correction applied to contacts with static bodies.
		/// </summary>
		[DefaultValue( 0.1 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> KineticContactHardness
		{
			get { if( _kineticContactHardness.BeginGet() ) KineticContactHardness = _kineticContactHardness.Get( this ); return _kineticContactHardness.value; }
			set
			{
				if( _kineticContactHardness.BeginSet( this, ref value ) )
				{
					try
					{
						KineticContactHardnessChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.KineticContactHardness = value;
					}
					finally { _kineticContactHardness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="KineticContactHardness"/> property value changes.</summary>
		public event Action<SoftBody> KineticContactHardnessChanged;
		ReferenceField<double> _kineticContactHardness = 0.1;

		/// <summary>
		/// Soft vs rigid impulse split [0,1] (cluster only).
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> SoftRigidImpulseSplit
		{
			get { if( _softRigidImpulseSplit.BeginGet() ) SoftRigidImpulseSplit = _softRigidImpulseSplit.Get( this ); return _softRigidImpulseSplit.value; }
			set
			{
				if( _softRigidImpulseSplit.BeginSet( this, ref value ) )
				{
					try
					{
						SoftRigidImpulseSplitChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.SoftRigidImpulseSplit = value;
					}
					finally { _softRigidImpulseSplit.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SoftRigidImpulseSplit"/> property value changes.</summary>
		public event Action<SoftBody> SoftRigidImpulseSplitChanged;
		ReferenceField<double> _softRigidImpulseSplit = 0.5;

		/// <summary>
		/// Soft vs soft impulse split [0,1] (cluster only).
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> SoftSoftImpulseSplit
		{
			get { if( _softSoftImpulseSplit.BeginGet() ) SoftSoftImpulseSplit = _softSoftImpulseSplit.Get( this ); return _softSoftImpulseSplit.value; }
			set
			{
				if( _softSoftImpulseSplit.BeginSet( this, ref value ) )
				{
					try
					{
						SoftSoftImpulseSplitChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.SoftSoftImpulseSplit = value;
					}
					finally { _softSoftImpulseSplit.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SoftSoftImpulseSplit"/> property value changes.</summary>
		public event Action<SoftBody> SoftSoftImpulseSplitChanged;
		ReferenceField<double> _softSoftImpulseSplit = 0.5;

		/// <summary>
		/// Soft vs kinetic impulse split [0,1] (cluster only).
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> SoftKineticImpulseSplit
		{
			get { if( _softKineticImpulseSplit.BeginGet() ) SoftKineticImpulseSplit = _softKineticImpulseSplit.Get( this ); return _softKineticImpulseSplit.value; }
			set
			{
				if( _softKineticImpulseSplit.BeginSet( this, ref value ) )
				{
					try
					{
						SoftKineticImpulseSplitChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.SoftKineticImpulseSplit = value;
					}
					finally { _softKineticImpulseSplit.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SoftKineticImpulseSplit"/> property value changes.</summary>
		public event Action<SoftBody> SoftKineticImpulseSplitChanged;
		ReferenceField<double> _softKineticImpulseSplit = 0.5;

		/// <summary>
		/// Soft vs kinetic hardness[0,1] (cluster only).
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> SoftKineticContactHardness
		{
			get { if( _softKineticContactHardness.BeginGet() ) SoftKineticContactHardness = _softKineticContactHardness.Get( this ); return _softKineticContactHardness.value; }
			set
			{
				if( _softKineticContactHardness.BeginSet( this, ref value ) )
				{
					try
					{
						SoftKineticContactHardnessChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.SoftKineticHardness = value;
					}
					finally { _softKineticContactHardness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SoftKineticContactHardness"/> property value changes.</summary>
		public event Action<SoftBody> SoftKineticContactHardnessChanged;
		ReferenceField<double> _softKineticContactHardness = 1.0;

		/// <summary>
		/// Soft vs soft hardness [0,1] (cluster only).
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> SoftSoftContactHardness
		{
			get { if( _softSoftContactHardness.BeginGet() ) SoftSoftContactHardness = _softSoftContactHardness.Get( this ); return _softSoftContactHardness.value; }
			set
			{
				if( _softSoftContactHardness.BeginSet( this, ref value ) )
				{
					try
					{
						SoftSoftContactHardnessChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.SoftSoftHardness = value;
					}
					finally { _softSoftContactHardness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SoftSoftContactHardness"/> property value changes.</summary>
		public event Action<SoftBody> SoftSoftContactHardnessChanged;
		ReferenceField<double> _softSoftContactHardness = 1.0;

		/// <summary>
		/// The amount of penetration correction applied to contacts with other soft bodies.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> SoftContactHardness
		{
			get { if( _softContactHardness.BeginGet() ) SoftContactHardness = _softContactHardness.Get( this ); return _softContactHardness.value; }
			set
			{
				if( _softContactHardness.BeginSet( this, ref value ) )
				{
					try
					{
						SoftContactHardnessChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.SoftContactHardness = value;
					}
					finally { _softContactHardness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SoftContactHardness"/> property value changes.</summary>
		public event Action<SoftBody> SoftContactHardnessChanged;
		ReferenceField<double> _softContactHardness = 1.0;

		//!!!!defaults
		/// <summary>
		/// The amount of correction is applied to follow anchor constraints.
		/// </summary>
		[DefaultValue( 0.7 )]
		[Range( 0.0, 1.0 )]
		[Serialize]
		[Category( "Material" )]
		public Reference<double> AnchorContactHardness
		{
			get { if( _anchorContactHardness.BeginGet() ) AnchorContactHardness = _anchorContactHardness.Get( this ); return _anchorContactHardness.value; }
			set
			{
				if( _anchorContactHardness.BeginSet( this, ref value ) )
				{
					try
					{
						AnchorContactHardnessChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.AnchorHardness = value;
					}
					finally { _anchorContactHardness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AnchorContactHardness"/> property value changes.</summary>
		public event Action<SoftBody> AnchorContactHardnessChanged;
		ReferenceField<double> _anchorContactHardness = 0.7;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The number of position iterations performed during soft body simulation.
		/// </summary>
		[DefaultValue( 2 )]
		[Range( 1, 20 )]
		[Serialize]
		[Category( "Solver" )]
		public Reference<int> PositionIterations
		{
			get { if( _positionIterations.BeginGet() ) PositionIterations = _positionIterations.Get( this ); return _positionIterations.value; }
			set
			{
				if( _positionIterations.BeginSet( this, ref value ) )
				{
					try
					{
						PositionIterationsChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.PositionIterations = value;
					}
					finally { _positionIterations.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="PositionIterations"/> property value changes.</summary>
		public event Action<SoftBody> PositionIterationsChanged;
		ReferenceField<int> _positionIterations = 2;

		/// <summary>
		/// The number of velocity iterations performed during soft body simulation.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 0, 20 )]
		[Serialize]
		[Category( "Solver" )]
		public Reference<int> VelocityIterations
		{
			get { if( _velocityIterations.BeginGet() ) VelocityIterations = _velocityIterations.Get( this ); return _velocityIterations.value; }
			set
			{
				if( _velocityIterations.BeginSet( this, ref value ) )
				{
					try
					{
						VelocityIterationsChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.VelocityIterations = value;
					}
					finally { _velocityIterations.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="VelocityIterations"/> property value changes.</summary>
		public event Action<SoftBody> VelocityIterationsChanged;
		ReferenceField<int> _velocityIterations = 1;

		/// <summary>
		/// The number of drift iterations performed during soft body simulation.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 0, 20 )]
		[Serialize]
		[Category( "Solver" )]
		public Reference<int> DriftIterations
		{
			get { if( _driftIterations.BeginGet() ) DriftIterations = _driftIterations.Get( this ); return _driftIterations.value; }
			set
			{
				if( _driftIterations.BeginSet( this, ref value ) )
				{
					try
					{
						DriftIterationsChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.DriftIterations = value;
					}
					finally { _driftIterations.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DriftIterations"/> property value changes.</summary>
		public event Action<SoftBody> DriftIterationsChanged;
		ReferenceField<int> _driftIterations = 1;

		/// <summary>
		/// The number of collision cluster iterations performed during soft body simulation.
		/// </summary>
		[DefaultValue( 4 )]
		[Range( 0, 100 )]
		[Serialize]
		[Category( "Solver" )]
		public Reference<int> ClusterIterations
		{
			get { if( _clusterIterations.BeginGet() ) ClusterIterations = _clusterIterations.Get( this ); return _clusterIterations.value; }
			set
			{
				if( _clusterIterations.BeginSet( this, ref value ) )
				{
					try
					{
						ClusterIterationsChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.Cfg.ClusterIterations = value;
					}
					finally { _clusterIterations.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ClusterIterations"/> property value changes.</summary>
		public event Action<SoftBody> ClusterIterationsChanged;
		ReferenceField<int> _clusterIterations = 4;

		/// <summary>
		/// The minimum allowable distance between cloth and collision object.
		/// </summary>
		[DefaultValue( 0.25 )]
		[Range( 0.001, 1.0 )]
		[Serialize]
		[Category( "Solver" )]
		public Reference<double> CollisionMargin
		{
			get { if( _collisionMargin.BeginGet() ) CollisionMargin = _collisionMargin.Get( this ); return _collisionMargin.value; }
			set
			{
				//!!!!проверять значения. везде так

				if( _collisionMargin.BeginSet( this, ref value ) )
				{
					try
					{
						CollisionMarginChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	softBody.CollisionShape.Margin = value;
					}
					finally { _collisionMargin.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionMargin"/> property value changes.</summary>
		public event Action<SoftBody> CollisionMarginChanged;
		ReferenceField<double> _collisionMargin = 0.25;

		/// <summary>
		/// The amount of velocity correction applied on the soft body.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 10 )]
		[Serialize]
		[Category( "Solver" )]
		public Reference<double> VelocityCorrectionFactor
		{
			get { if( _velocityCorrectionFactor.BeginGet() ) VelocityCorrectionFactor = _velocityCorrectionFactor.Get( this ); return _velocityCorrectionFactor.value; }
			set
			{
				if( _velocityCorrectionFactor.BeginSet( this, ref value ) )
				{
					try
					{
						VelocityCorrectionFactorChanged?.Invoke( this );

						//!!!!
						//if( softBody?.Cfg != null )
						//	softBody.Cfg.VelocityCorrectionFactor = value;
					}
					finally { _velocityCorrectionFactor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="VelocityCorrectionFactor"/> property value changes.</summary>
		public event Action<SoftBody> VelocityCorrectionFactorChanged;
		ReferenceField<double> _velocityCorrectionFactor = 1.0;

		/// <summary>
		/// The time scale of the soft body simulation.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 10 )]
		[Serialize]
		[Category( "Solver" )]
		public Reference<double> TimeScale
		{
			get { if( _timeScale.BeginGet() ) TimeScale = _timeScale.Get( this ); return _timeScale.value; }
			set
			{
				if( _timeScale.BeginSet( this, ref value ) )
				{
					try
					{
						TimeScaleChanged?.Invoke( this );

						//!!!!
						//if( softBody?.Cfg != null )
						//	softBody.Cfg.Timescale = value;
					}
					finally { _timeScale.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="TimeScale"/> property value changes.</summary>
		public event Action<SoftBody> TimeScaleChanged;
		ReferenceField<double> _timeScale = 1.0;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!name: ContactGroup, ContactMask
		/// <summary>
		/// The collision filter group of the soft body.
		/// </summary>
		[DefaultValue( 1 )]
		[Serialize]
		[Category( "Collision Filtering" )]
		public Reference<int> CollisionGroup
		{
			get { if( _collisionGroup.BeginGet() ) CollisionGroup = _collisionGroup.Get( this ); return _collisionGroup.value; }
			set
			{
				if( _collisionGroup.BeginSet( this, ref value ) )
				{
					try
					{
						CollisionGroupChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	RecreateBody();
					}
					finally { _collisionGroup.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionGroup"/> property value changes.</summary>
		public event Action<SoftBody> CollisionGroupChanged;
		ReferenceField<int> _collisionGroup = 1;

		//!!!!default value
		/// <summary>
		/// The collision filter mask of the soft body.
		/// </summary>
		[DefaultValue( 1 )]
		[Serialize]
		[Category( "Collision Filtering" )]
		public Reference<int> CollisionMask
		{
			get { if( _collisionMask.BeginGet() ) CollisionMask = _collisionMask.Get( this ); return _collisionMask.value; }
			set
			{
				if( _collisionMask.BeginSet( this, ref value ) )
				{
					try
					{
						CollisionMaskChanged?.Invoke( this );
						//!!!!
						//if( softBody != null )
						//	RecreateBody();
					}
					finally { _collisionMask.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionMask"/> property value changes.</summary>
		public event Action<SoftBody> CollisionMaskChanged;
		ReferenceField<int> _collisionMask = 1;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The initial linear velocity of the soft body.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Serialize]
		[Category( "Initial" )]
		public Reference<Vector3> LinearVelocity
		{
			get { if( _linearVelocity.BeginGet() ) LinearVelocity = _linearVelocity.Get( this ); return _linearVelocity.value; }
			set
			{
				if( _linearVelocity.BeginSet( this, ref value ) )
				{
					try
					{
						LinearVelocityChanged?.Invoke( this );
						//!!!!
						//if( softBody != null && !updatePropertiesWithoutUpdatingBody )
						//	softBody.SetVelocity( BulletPhysicsUtility.Convert( value ) );
					}
					finally { _linearVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearVelocity"/> property value changes.</summary>
		public event Action<SoftBody> LinearVelocityChanged;
		ReferenceField<Vector3> _linearVelocity = Vector3.Zero;

		//!!!!AngularVelocity

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				//case nameof( ClustersNumber ):
				//	skip = GenerateClusters.Value != ClusterModeEnum.Manual;
				//	break;
				//case nameof( AnchorContactHardness ):
				//case nameof( KineticContactHardness ):
				//case nameof( RigidContactHardness ):
				//case nameof( SoftContactHardness ):
				//case nameof( SoftRigidContactHardness ):
				//case nameof( SoftSoftContactHardness ):
				//case nameof( SoftKineticContactHardness ):
				//case nameof( SoftKineticImpulseSplit ):
				//case nameof( SoftRigidImpulseSplit ):
				//case nameof( SoftSoftImpulseSplit ):
				//case nameof( ClusterIterations ):
				//	if( ClustersNumber.Value == 0 )
				//		skip = true;
				//	break;
				//case nameof( BodyAeroModel ):
				//case nameof( PositionIterations ):
				//case nameof( DriftIterations ):
				//case nameof( VelocityIterations ):
				//case nameof( VelocityCorrectionFactor ):
				//case nameof( TimeScale ):
				//case nameof( GenerateClusters ):
				//	if( EditMode.Value == EditModeEnum.Simple )
				//		skip = true;
				//	break;

				//!!!!disabled until all parameters fixed (SoftRigidContactHardness, etc).
				//case nameof( MaterialLinearStiffness ):
				//case nameof( MaterialAngularStiffness ):
				//case nameof( MaterialVolumeStiffness ):
				//case nameof( MaterialFriction ):
				//	if( Material.Value != null )
				//		skip = true;
				//	break;
				//case nameof( BendingConstraintDistance ):
				//	if( !BendingConstraints )
				//		skip = true;
				//	break;
				case nameof( ShapeCoefficient ):
					if( !ShapeMatching )
						skip = true;
					break;
				case nameof( VolumeCoefficient ):
				case nameof( MaxVolumeRatio ):
					if( !VolumeMatching )
						skip = true;
					break;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!
		//[Browsable( false )]
		//public Internal.BulletSharp.SoftBody.SoftBody InternalSoftBody
		//{
		//	get { return softBody; }
		//}

		public Scene.PhysicsWorldClass GetPhysicsWorldData()
		{
			//!!!!slowly?
			var scene = ParentScene;
			if( scene != null )
				return scene.PhysicsWorld;
			return null;
		}

		protected override void OnTransformChanged()
		{
			//calls SpaceBoundsUpdate();
			base.OnTransformChanged();

			//!!!!
			//if( softBody != null && !updatePropertiesWithoutUpdatingBody )
			//{
			//	var bodyTransform = Transform.Value;


			//	//!!!!fix ray cast bug
			//	RecreateBody();

			//	//before fix:

			//	////back to Identity
			//	//softBody.Transform( BulletUtils.Convert( actualTransform.GetInverse() ) );
			//	////set transform
			//	//actualTransform = bodyTransform.ToMat4();

			//	////softBody.Transform() internally calls updateNormals(), updateBounds(), updateConstants();
			//	//softBody.Transform( BulletUtils.Convert( actualTransform ) );



			//	//fix tries:

			//	//softBody.UpdateClusters();
			//	//softBody.UpdatePose();

			//	////to fix bug fix ray cast
			//	//GetPhysicsWorldData().world.RemoveSoftBody( softBody );
			//	//GetPhysicsWorldData().world.AddSoftBody( softBody, CollisionGroup.Value, CollisionMask.Value );

			//	//GetPhysicsWorldData().world.UpdateSingleAabb( softBody );
			//	//GetPhysicsWorldData().world.UpdateAabbs();

			//	////the computeOverlappingPairs is usually already called by performDiscreteCollisionDetection (or stepSimulation)
			//	////it can be useful to use if you perform ray tests without collision detection/simulation
			//	//GetPhysicsWorldData().world.ComputeOverlappingPairs();

			//	//!!!!update constraints
			//}
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			Mesh m = MeshOutput;
			if( m != null && m.Result != null )
			{
				var meshSpaceBounds = m.Result.SpaceBounds;
				var b = SpaceBounds.Multiply( Transform, meshSpaceBounds );
				newBounds = SpaceBounds.Merge( newBounds, b );
			}
		}

		protected override bool OnEnabledSelectionByCursor()
		{
			var scene = ParentScene;
			if( !scene.GetDisplayDevelopmentDataInThisApplication() )
				return false;
			//!!!!
			//if( softBody != null )
			//{
			//	if( !scene.DisplayPhysicalObjects )
			//		return false;
			//}
			//else
			{
				if( !scene.DisplayLabels )
					return false;
			}
			return base.OnEnabledSelectionByCursor();
		}

		protected override void OnCheckSelectionByRay( CheckSelectionByRayContext context )
		{
			base.OnCheckSelectionByRay( context );

			//!!!!
			//if( softBody != null )
			//{
			//	context.thisObjectWasChecked = true;

			//	var item = new PhysicsRayTestItem( context.ray, CollisionGroup.Value, -1, PhysicsRayTestItem.ModeEnum.OneClosest, softBody );
			//	ParentScene.PhysicsRayTest( item );
			//	if( item.Result.Length != 0 )
			//		context.thisObjectResultRayScale = item.Result[ 0 ].DistanceScale;
			//	//if( SpaceBounds.CalculatedBoundingBox.Intersects( context.ray, out var scale ) )
			//	//	context.thisObjectResultRayScale = scale;
			//}
		}

		void CreateMesh()
		{
			Mesh mesh = Mesh;

			//create mesh
			//need set ShowInEditor = false before AddComponent
			var meshCreated = ComponentUtility.CreateComponent<Mesh>( null, false, false );
			meshCreated.NetworkMode = NetworkModeEnum.False;
			meshCreated.DisplayInEditor = false;
			AddComponent( meshCreated, -1 );
			//var meshCreated = CreateComponent<Mesh>( -1, false );

			meshCreated.SaveSupport = false;
			meshCreated.CloneSupport = false;
			//meshCreated.ShowInEditor = false;
			meshCreated.Name = createMeshName;

			var geometry = meshCreated.CreateComponent<MeshGeometry>();

			meshCreated.MeshCompileEvent += delegate ( Mesh sender, Mesh.CompiledData compiledData )
			{
				GetProcessedData( out var processedVertices, /*out _, */out var processedIndices, out _ );

				var op = new RenderingPipeline.RenderSceneData.MeshDataRenderOperation( geometry, 0 );
				//op.Creator = geometry;//meshCreated;
				//op.disposeBuffersByCreator = true;
				op.VertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );
				op.VertexStructureContainsColor = true;

				var newVertices = new byte[ vertexSize * processedVertices.Length ];
				{
					unsafe
					{
						fixed( byte* pVertices = newVertices )
						{
							var pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;
							for( int n = 0; n < processedVertices.Length; n++ )
							{
								pVertex->Color = new ColorValue( 1, 1, 1, 1 );
								//pVertex->tangent = new Vec4F( 1, 0, 0, 1 );
								pVertex++;
							}
						}
					}
				}

				var vertexDeclaration = op.VertexStructure.CreateVertexDeclaration( 0 );

				var vertexBuffers = new List<GpuVertexBuffer>();//op.VertexBuffers = new List<GpuVertexBuffer>();
				vertexBuffers.Add( GpuBufferManager.CreateVertexBuffer( newVertices, vertexDeclaration, GpuBufferFlags.Dynamic | GpuBufferFlags.ComputeRead ) );
				op.VertexBuffers = vertexBuffers.ToArray();
				op.VertexStartOffset = 0;
				op.VertexCount = processedVertices.Length;

				op.IndexBuffer = GpuBufferManager.CreateIndexBuffer( processedIndices );
				op.IndexStartOffset = 0;
				op.IndexCount = processedIndices.Length;

				//op.material = ResourceManager.LoadResource<MaterialStandard>( @"Base\Materials\White.material" );
				//op.material = sourceOp.material;

				compiledData.MeshData.RenderOperations.Add( op );

				//foreach( var sourceItem in mesh.Result.RenderOperations )
				//{
				//	var sourceOp = sourceItem.operation;

				//	var op = new Mesh.CompiledData.RenderOperation();
				//	op.creator = geometry;//meshCreated;
				//	op.disposeBuffersByCreator = true;
				//	op.vertexStructure = sourceOp.vertexStructure;

				//	op.vertexBuffers = new List<GpuVertexBuffer>();
				//	foreach( var sourceBuffer in sourceOp.vertexBuffers )
				//	{
				//		var buffer = GpuBufferManager.CreateVertexBuffer( sourceBuffer.Vertices, sourceBuffer.VertexDeclaration, true );
				//		//var buffer = GpuBufferManager.CreateVertexBuffer(
				//		//	sourceBuffer.VertexSize, sourceBuffer.VertexCount, sourceBuffer.Vertices, true );
				//		op.vertexBuffers.Add( buffer );
				//	}
				//	op.vertexStartOffset = sourceOp.vertexStartOffset;
				//	op.vertexCount = sourceOp.vertexCount;

				//	if( sourceOp.indexBuffer != null )
				//		op.indexBuffer = GpuBufferManager.CreateIndexBuffer( sourceOp.indexBuffer.Indices, false );
				//	op.indexStartOffset = sourceOp.indexStartOffset;
				//	op.indexCount = sourceOp.indexCount;

				//	op.material = sourceOp.material;

				//	var item = new Mesh.CompiledData.RenderOperationItem();
				//	item.operation = op;
				//	//item.transform = NeoAxis.Transform.Identity;

				//	//apply transform
				//	//op.transform = NeoAxis.Transform.Identity;

				//	if( !sourceItem.transform.IsIdentity )
				//	{
				//		//!!!!check
				//		//!!!!slowly

				//		Mat4F mat4 = sourceItem.transform.ToMat4().ToMat4F();
				//		Mat3F mat3 = mat4.ToMat3();

				//		//positions
				//		{
				//			if( op.vertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out VertexElement element ) &&
				//				element.Type == VertexElementType.Float3 )
				//			{
				//				var buffer = op.vertexBuffers[ element.Source ];
				//				var ar = buffer.ExtractChannelFloat3( element.Offset );
				//				for( int n = 0; n < ar.Length; n++ )
				//					ar[ n ] = mat4 * ar[ n ];
				//				buffer.WriteChannel( element.Offset, ar );
				//			}
				//		}

				//		//normals
				//		{
				//			if( op.vertexStructure.GetElementBySemantic( VertexElementSemantic.Normal, out VertexElement element ) &&
				//				element.Type == VertexElementType.Float3 )
				//			{
				//				var buffer = op.vertexBuffers[ element.Source ];
				//				var ar = buffer.ExtractChannelFloat3( element.Offset );
				//				for( int n = 0; n < ar.Length; n++ )
				//					ar[ n ] = ( mat3 * ar[ n ] ).GetNormalize();
				//				buffer.WriteChannel( element.Offset, ar );
				//			}
				//		}

				//		//tangents
				//		{
				//			if( op.vertexStructure.GetElementBySemantic( VertexElementSemantic.Tangent, out VertexElement element ) &&
				//				element.Type == VertexElementType.Float4 )
				//			{
				//				var buffer = op.vertexBuffers[ element.Source ];
				//				var ar = buffer.ExtractChannelFloat4( element.Offset );
				//				for( int n = 0; n < ar.Length; n++ )
				//					ar[ n ] = new Vec4F( ( mat3 * ar[ n ].ToVec3F() ).GetNormalize(), ar[ n ].W );
				//				buffer.WriteChannel( element.Offset, ar );
				//			}
				//		}
				//	}

				//	compiledData.RenderOperations.Add( item );
				//}

				//!!!!tangents
				//SimpleMeshGenerator.GenerateBox( Vec3F.One, out vertices, out normals, out Vec4F[] tangents, out Vec2F[] texCoords, out indices );
				//meshCreated.Indices = indices;

				//meshCreated.Vertices = new byte[ vertexSize * vertices.Length ];
				//unsafe
				//{
				//	fixed ( byte* pVertices = meshCreated.Vertices.Value )
				//	{
				//		StandardVertexF* pVertex = (StandardVertexF*)pVertices;

				//		for( int n = 0; n < vertices.Length; n++ )
				//		{
				//			pVertex->position = vertices[ n ];
				//			pVertex->normal = normals[ n ];

				//			//!!!!
				//			//pVertex->tangent = xx;

				//			pVertex->color = ColorValue.FromColor( System.Drawing.Color.LightGray );

				//			//!!!!temp. как-то указывать способ натягивания? 
				//			pVertex->texCoord0 = pVertex->position.ToVec2() * 2;
				//			pVertex->texCoord1 = pVertex->texCoord0;
				//			pVertex->texCoord2 = pVertex->texCoord0;
				//			pVertex->texCoord3 = pVertex->texCoord0;

				//			pVertex++;
				//		}
				//	}
				//}
			};

			meshCreated.Enabled = true;
			meshCreated.PerformResultCompile();

			UpdateCreatedMeshData();
		}

		void CreateBody()
		{
			duringCreateDestroy = true;

			//clear data
			processedVertices = null;
			//processedVerticesNormals = null;
			processedIndices = null;
			processedTrianglesToSourceIndex = null;
			simulatedVertices = null;
			//simulatedVerticesNormals = null;

			var physicsWorldData = GetPhysicsWorldData();
			if( physicsWorldData != null )
			{

				//!!!!

				//if( softBody != null )
				//	Log.Fatal( "SoftBody: CreateBody: softBody != null." );
				//if( !EnabledInHierarchy )
				//	Log.Fatal( "SoftBody: CreateBody: !EnabledInHierarchy." );

				//Mesh mesh = Mesh;
				//if( mesh != null && mesh.Result != null )
				//{
				//	if( GetSourceData( out var sourceVertices, out var sourceIndices ) && sourceVertices.Length != 0 && sourceIndices.Length != 0 )
				//	{
				//		////!!!!Clone
				//		//vertices = (Vec3F[])mesh.Result.ExtractedVerticesPositions.Clone();
				//		//indices = (int[])mesh.Result.ExtractedIndices.Clone();
				//		//if( normals == null || normals.Length != vertices.Length )
				//		//	Array.Resize( ref normals, vertices.Length );

				//		//!!!!могут быть специально подготовленные для soft body данные в меше

				//		MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( sourceVertices, sourceIndices, 0.0001f, 0.0001f, true, true, out processedVertices, out processedIndices, out processedTrianglesToSourceIndex );

				//		////!!!!maybe temp. not good implementation. not same as bullet
				//		////calculate processed normals
				//		//{
				//		//	processedVerticesNormals = new Vec3F[ processedVertices.Length ];
				//		//	for( int nTriangle = 0; nTriangle < processedIndices.Length / 3; nTriangle++ )
				//		//	{
				//		//		var index0 = processedIndices[ nTriangle * 3 + 0 ];
				//		//		var index1 = processedIndices[ nTriangle * 3 + 1 ];
				//		//		var index2 = processedIndices[ nTriangle * 3 + 2 ];

				//		//		var v0 = processedVertices[ index0 ];
				//		//		var v1 = processedVertices[ index1 ];
				//		//		var v2 = processedVertices[ index2 ];

				//		//		var normal = Vec3F.Cross( v1 - v0, v2 - v0 ).GetNormalize();
				//		//		//!!!!not good
				//		//		processedVerticesNormals[ index0 ] = normal;
				//		//		processedVerticesNormals[ index1 ] = normal;
				//		//		processedVerticesNormals[ index2 ] = normal;
				//		//	}
				//		//}

				//		var wi = ParentScene.PhysicsWorldData.softBodyWorldInfo;
				//		var positions = BulletPhysicsUtility.Convert( processedVertices );

				//		//////////////

				//		////!!!!
				//		//var masses = new double[ processedVertices.Length ];
				//		//for( int n = 0; n < masses.Length; n++ )
				//		//	masses[ n ] = 1;
				//		//{
				//		//	masses[ 0 ] = 0;
				//		//	masses[ masses.Length - 1 ] = 0;
				//		//}

				//		//masses[ 0 ] = 0;
				//		//var masses = new double[ processedVertices.Length ];
				//		//for( int n = 0; n < masses.Length; n++ )
				//		//	masses[ n ] = 1;

				//		//!!!!
				//		//softBody = new SoftBody( wi, positions.Length, positions, masses );
				//		softBody = new Internal.BulletSharp.SoftBody.SoftBody( wi, positions.Length, positions, null );

				//		////!!!!
				//		//Log.Info( softBody.GetMass( 0 ).ToString() );
				//		//softBody.SetMass( 0, 0 );
				//		//softBody.SetMass( positions.Length - 1, 0 );

				//		var lines = MathAlgorithms.TriangleListToLineList( processedIndices );
				//		for( var l = 0; l < lines.Length; l += 2 )
				//			softBody.AppendLink( lines[ l + 0 ], lines[ l + 1 ], null, true );
				//		for( int i = 0; i < processedIndices.Length; i += 3 )
				//			softBody.AppendFace( processedIndices[ i + 0 ], processedIndices[ i + 1 ], processedIndices[ i + 2 ] );

				//		if( RandomizeConstraints.Value )
				//		{
				//			softBody.RandomizeConstraints();


				//			//!!!!need?

				//			processedIndices = (int[])processedIndices.Clone();

				//			//enumerate nodes
				//			for( int n = 0; n < softBody.Nodes.Count; n++ )
				//				softBody.Nodes[ n ].Tag = new IntPtr( n );

				//			var triIdx = 0;

				//			// update triangles after calling btSoftBody::randomizeConstraints()
				//			for( int f = 0; f < softBody.Faces.Count; f++ )
				//			{
				//				var face = softBody.Faces[ f ];

				//				processedIndices[ triIdx++ ] = face.Nodes[ 0 ].Tag.ToInt32();
				//				processedIndices[ triIdx++ ] = face.Nodes[ 1 ].Tag.ToInt32();
				//				processedIndices[ triIdx++ ] = face.Nodes[ 2 ].Tag.ToInt32();
				//			}
				//		}

				//		//////////////

				//		////creates nodes, faces and links, calls btSoftBody::randomizeConstraints() if needed
				//		//softBody = SoftBodyHelpers.CreateFromTriMesh( wi, positions, processedIndices, RandomizeConstraints.Value );
				//		////softBody = new SoftBody( wi, positions.Length, positions, null );

				//		//if( RandomizeConstraints.Value )
				//		//{
				//		//	processedIndices = (int[])processedIndices.Clone();

				//		//	//enumerate nodes
				//		//	for( int n = 0; n < softBody.Nodes.Count; n++ )
				//		//		softBody.Nodes[ n ].Tag = new IntPtr( n );

				//		//	var triIdx = 0;

				//		//	// update triangles after calling btSoftBody::randomizeConstraints()
				//		//	for( int f = 0; f < softBody.Faces.Count; f++ )
				//		//	{
				//		//		var face = softBody.Faces[ f ];

				//		//		processedIndices[ triIdx++ ] = face.Nodes[ 0 ].Tag.ToInt32();
				//		//		processedIndices[ triIdx++ ] = face.Nodes[ 1 ].Tag.ToInt32();
				//		//		processedIndices[ triIdx++ ] = face.Nodes[ 2 ].Tag.ToInt32();
				//		//	}
				//		//}

				//		//////////////

				//		//apply settings
				//		{
				//			SetMaterial( softBody );
				//			ApplyConfig();

				//			// Generate bending constraints based on distance in the adjency graph
				//			if( BendingConstraintDistance > 0 )
				//				softBody.GenerateBendingConstraints( BendingConstraintDistance, softBody.Materials[ 0 ] );

				//			// The minimum allowable distance between cloth and collision object.
				//			softBody.CollisionShape.Margin = CollisionMargin;

				//			//!!!!было ниже
				//			// Mass
				//			softBody.SetTotalMass( Mass, MassFromFaces );
				//			softBody.SetPose( VolumeMatching, ShapeMatching );

				//			//By default, soft bodies perform collision detection using between vertices (nodes) and triangles( faces). 
				//			//This requires a dense tessellation, otherwise collisions might be missed.
				//			//An improved method uses automatic decomposition into convex deformable clusters. 

				//			//To enable collision clusters, use: 

				//			//psb->generateClusters( numSubdivisions );
				//			//enable cluster collision between soft body and rigid body 
				//			//psb->m_cfg.collisions += btSoftBody::fCollision::CL_RS;

				//			//enable cluster collision between soft body and soft body 
				//			//psb->m_cfg.collisions += btSoftBody::fCollision::CL_SS;

				//			//clusters
				//			softBody.GenerateClusters( ClustersNumber );
				//			//{
				//			//	int k = 0;
				//			//	switch( GenerateClusters.Value )
				//			//	{
				//			//	case ClusterModeEnum.Manual: k = ClustersNumber; break;
				//			//	//case ClusterModeEnum.Auto: k = (int)Math.Sqrt( vertices.Length ); break;
				//			//	case ClusterModeEnum.ForEachTriangle: k = 0; break;
				//			//	}
				//			//	softBody.GenerateClusters( k );
				//			//}

				//			//!!!!теперь выше
				//			// Mass
				//			//softBody.SetTotalMass( Mass, MassFromFaces );
				//			//softBody.SetPose( VolumeMatching, ShapeMatching );
				//		}

				//		CreateMesh();

				//		//set transform, velocity
				//		{
				//			var bodyTransform = Transform.Value;

				//			actualTransform = bodyTransform.ToMatrix4();
				//			softBody.Transform( BulletPhysicsUtility.Convert( actualTransform ) );
				//			//softBody.Transform( BulletUtils.Convert( bodyTransform.ToMat4() ) );

				//			softBody.SetVelocity( BulletPhysicsUtility.Convert( LinearVelocity ) );
				//		}

				//		softBody.UserObject = this;
				//		physicsWorldData.world.AddSoftBody( softBody, CollisionGroup.Value, CollisionMask.Value );

				//		////!!!!
				//		//softBody.SetMass( 0, 0 );
				//		//softBody.SetMass( positions.Length - 1, 0 );

				//		usedMeshDataWhenInitialized = mesh.Result;

				//		//!!!!не было
				//		SpaceBoundsUpdate();
				//	}
				//}
			}

			duringCreateDestroy = false;
		}

		void DestroyBody()
		{
			duringCreateDestroy = true;

			usedMeshDataWhenInitialized = null;

			var physicsWorldData = GetPhysicsWorldData();
			if( physicsWorldData != null )
			{
				//!!!!что еще удалять?
				//!!!!правильно ли тут всё?

				//////destroy linked constraints
				////{
				////	if( rigidBody != null && rigidBody.NumConstraintRefs != 0 )
				////	{
				////		foreach( var c in GetLinkedCreatedConstraints() )
				////			c.DestroyConstraint();
				////	}

				////	////!!!!что еще также? и где?
				////	//if( rigidBody != null )
				////	//{
				////	//	//var cachedArray = actionsBeforeBodyDestroy.ToArray();
				////	//	//foreach( var action in cachedArray )
				////	//	//	action();
				////	//}
				////	//actionsBeforeBodyDestroy.Clear();
				////}

				//destroy created mesh
				var meshCreated = MeshCreated;
				if( meshCreated != null )
				{
					meshCreated.RemoveFromParent( true );
					meshCreated.Dispose();
				}

				//!!!!
				//if( softBody != null )
				//{
				//	physicsWorldData.world.RemoveSoftBody( softBody );
				//	softBody.Dispose();
				//	softBody = null;
				//}
			}

			duringCreateDestroy = false;
		}

		//!!!!
		//static int FindPose( Internal.BulletSharp.Math.BVector3[] srcArr, int srcIndex, double squaredDist = double.Epsilon )
		//{
		//	var i = 0;
		//	var pos = srcArr[ srcIndex ];
		//	while( i < srcIndex )
		//	{
		//		Internal.BulletSharp.Math.BVector3.DistanceSquared( ref srcArr[ i++ ], ref pos, out double dist );
		//		if( dist <= squaredDist )
		//			return i - 1;
		//	}
		//	return -1;
		//}

		public void RecreateBody()
		{
			if( EnabledInHierarchy && !duringCreateDestroy )
			{
				DestroyBody();
				CreateBody();
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged(); // in current engine implementation the method OnSpaceBoundsUpdate is called from base class.

			if( EnabledInHierarchy )
			{
				//!!!!
				////сделано по аналогии с rigid body, хотя может смысла нет.
				////после загрузки создается через шейп, т.к. срабатывает RecreateBody()
				//if( softBody == null )
				//	CreateBody();
			}
			else
				DestroyBody();
		}

		//!!!!
		public void RenderPhysicalObject( ViewportRenderingContext context, out int verticesRendered )
		//public override void RenderPhysicalObject( ViewportRenderingContext context, out int verticesRendered )
		{
			verticesRendered = 0;

			var context2 = context.ObjectInSpaceRenderingContext;

			//!!!!

			//var scene = ParentScene;
			////bool show = ( scene.GetDisplayDevelopmentDataInThisApplication() && scene.DisplayPhysicalObjects ) ||
			////	context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.dragDropCreateObject == this;
			//if( /*show && */softBody != null && context.Owner.Simple3DRenderer != null )
			//{
			//	//if( context2.displayPhysicalObjectsCounter < context2.displayPhysicalObjectsMax )
			//	//{
			//	//	context2.displayPhysicalObjectsCounter++;

			//	var viewport = context.Owner;

			//	//if( show )
			//	{
			//		ColorValue color;
			//		if( softBody.IsActive )
			//			color = ProjectSettings.Get.General.SceneShowPhysicsDynamicActiveColor;
			//		else
			//			color = ProjectSettings.Get.General.SceneShowPhysicsDynamicInactiveColor;
			//		viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

			//		if( GetSimulatedData( out var vertices, out var indices, out _ ) )
			//		{
			//			var m = Transform.Value.ToMatrix4();
			//			viewport.Simple3DRenderer.AddTriangles( vertices, indices, m, true, true );
			//			verticesRendered += vertices.Length;
			//		}
			//	}

			//	if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
			//	{
			//		ColorValue color;
			//		if( context2.selectedObjects.Contains( this ) )
			//			color = ProjectSettings.Get.Colors.SelectedColor;
			//		else
			//			color = ProjectSettings.Get.Colors.CanSelectColor;

			//		//!!!!или невидимое лучше габаритами подсвечивать?

			//		//color.Alpha *= .5f;
			//		viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

			//		if( GetSimulatedData( out var vertices, out var indices, out _ ) )
			//		{
			//			var m = Transform.Value.ToMatrix4();
			//			viewport.Simple3DRenderer.AddTriangles( vertices, indices, m, true, true );
			//			verticesRendered += vertices.Length;
			//		}
			//		else
			//		{
			//			context.Owner.Simple3DRenderer.AddBounds( SpaceBounds.CalculatedBoundingBox );
			//			verticesRendered += 96;
			//		}
			//	}

			//	//}
			//}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				//!!!!тут?
				CheckForUpdateDataWhenMeshChanged();

				var context2 = context.ObjectInSpaceRenderingContext;

				////var scene = ParentScene;
				////bool show = ( scene.GetDisplayDevelopmentDataInThisApplication() && scene.DisplayPhysicalObjects ) ||
				////	context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.dragDropCreateObject == this;
				////if( show && softBody != null && context.Owner.Simple3DRenderer != null )
				////{
				////	if( context2.displayPhysicalObjectsCounter < context2.displayPhysicalObjectsMax )
				////	{
				////		context2.displayPhysicalObjectsCounter++;

				////		var viewport = context.Owner;

				////		//if( show )
				////		{
				////			ColorValue color;
				////			if( softBody.IsActive )
				////				color = ProjectSettings.Get.SceneShowPhysicsDynamicActiveColor;
				////			else
				////				color = ProjectSettings.Get.SceneShowPhysicsDynamicInactiveColor;
				////			viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

				////			if( GetSimulatedData( out var vertices, out var indices, out _ ) )
				////			{
				////				var m = Transform.Value.ToMatrix4();
				////				viewport.Simple3DRenderer.AddTriangles( vertices, indices, m, true, true );
				////			}
				////		}

				////		if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
				////		{
				////			ColorValue color;
				////			if( context2.selectedObjects.Contains( this ) )
				////				color = ProjectSettings.Get.SelectedColor;
				////			else
				////				color = ProjectSettings.Get.CanSelectColor;

				////			//!!!!или невидимое лучше габаритами подсвечивать?

				////			//color.Alpha *= .5f;
				////			viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

				////			if( GetSimulatedData( out var vertices, out var indices, out _ ) )
				////			{
				////				var m = Transform.Value.ToMatrix4();
				////				viewport.Simple3DRenderer.AddTriangles( vertices, indices, m, true, true );
				////			}
				////			else
				////				context.Owner.Simple3DRenderer.AddBounds( SpaceBounds.CalculatedBoundingBox );
				////		}
				////	}
				////}


				//!!!!
				//var showLabels = /*show && */softBody == null;
				//if( !showLabels )
				//	context2.disableShowingLabelForThisObject = true;
			}
		}

		public bool GetSourceData( out Vector3F[] vertices, out int[] indices )
		{
			var mesh = Mesh.Value;
			if( mesh != null && mesh.Result != null )
			{
				vertices = mesh.Result.ExtractedVerticesPositions;
				indices = mesh.Result.ExtractedIndices;
			}
			else
			{
				vertices = null;
				indices = null;
			}
			return vertices != null && vertices.Length != 0 && indices != null && indices.Length != 0;

			//vertices = this.vertices;
			//indices = this.indices;
			//return true;
		}

		public bool GetProcessedData( out Vector3F[] processedVertices, /*out Vec3F[] processedVerticesNormals, */out int[] processedIndices, out int[] processedTrianglesToSourceIndex )
		{
			processedVertices = this.processedVertices;
			//processedVerticesNormals = this.processedVerticesNormals;
			processedIndices = this.processedIndices;
			processedTrianglesToSourceIndex = this.processedTrianglesToSourceIndex;
			return processedVertices != null;
		}

		public bool GetSimulatedData( out Vector3F[] vertices, /*out Vec3F[] normals, */out int[] indices, out int[] processedTrianglesToSourceIndex )
		{
			if( !GetProcessedData( out vertices, /*out normals, */out indices, out processedTrianglesToSourceIndex ) )
				return false;
			if( simulatedVertices != null )
			{
				vertices = simulatedVertices;
				//normals = simulatedVerticesNormals;
			}
			return true;
		}

		public bool GetTriangleSourceData( int triangleID, bool applyWorldTransform, out Triangle triangle )
		{
			if( !GetSourceData( out var vertices, out var indices ) )
			{
				triangle = new Triangle();
				return false;
			}
			return MathAlgorithms.GetTriangleData( triangleID, applyWorldTransform ? Transform.Value : null, vertices, indices, out triangle );
		}

		public bool GetTriangleProcessedData( int triangleID, bool applyWorldTransform, out Triangle triangle )
		{
			if( !GetProcessedData( out var vertices, /*out _, */out var indices, out _ ) )
			{
				triangle = new Triangle();
				return false;
			}
			return MathAlgorithms.GetTriangleData( triangleID, applyWorldTransform ? Transform.Value : null, vertices, indices, out triangle );
		}

		public bool GetTriangleSimulatedData( int triangleID, bool applyWorldTransform, out Triangle triangle )
		{
			//!!!!
			////!!!!new. hmhm
			//if( applyWorldTransform && softBody != null )
			//{
			//	var face = softBody.Faces[ triangleID ];
			//	triangle = new Triangle( BulletPhysicsUtility.Convert( face.Nodes[ 0 ].Position ), BulletPhysicsUtility.Convert( face.Nodes[ 1 ].Position ), BulletPhysicsUtility.Convert( face.Nodes[ 2 ].Position ) );
			//	return true;
			//}

			if( !GetSimulatedData( out var vertices, /*out _, */out var indices, out _ ) )
			{
				triangle = new Triangle();
				return false;
			}
			return MathAlgorithms.GetTriangleData( triangleID, applyWorldTransform ? Transform.Value : null, vertices, indices, out triangle );
		}

		public Vector3 GetNodePosition( int nodeIndex )
		{
			//!!!!
			//if( softBody != null )
			//{
			//	var nodes = softBody.Nodes;
			//	if( nodeIndex >= 0 && nodeIndex < nodes.Count )
			//		return BulletPhysicsUtility.Convert( nodes[ nodeIndex ].Position );
			//}
			return Vector3.Zero;
		}

		public int FindClosestNodeIndex( Vector3 worldPosition )
		{
			var result = -1;

			//!!!!
			//if( softBody != null )
			//{
			//	var worldPosition2 = BulletPhysicsUtility.ToVector3( worldPosition );
			//	var nodes = softBody.Nodes;
			//	var count = nodes.Count;

			//	var minDistance = double.MaxValue;
			//	for( int n = 0; n < count; n++ )
			//	{
			//		var dist = Internal.BulletSharp.Math.BVector3.DistanceSquared( nodes[ n ].Position, (Internal.BulletSharp.Math.BVector3)worldPosition2 );
			//		if( dist < minDistance )
			//		{
			//			result = n;
			//			minDistance = dist;
			//		}
			//	}
			//}

			return result;
		}

		//internal int FindClosestFaceIndex( Vec3 worldPos )
		//{
		//	if( softBody == null )
		//		return -1;

		//	var wPos = BulletUtils.ToVector3( worldPos );
		//	var faces = softBody.Faces;
		//	var cnt = faces.Count;
		//	var result = -1;
		//	var minDist = double.MaxValue;

		//	for( int i = 0; i < cnt; i++ )
		//	{
		//		var face = faces[ i ];
		//		var faceNodes = face.Nodes;

		//		var p = ( faceNodes[ 0 ].Position + faceNodes[ 1 ].Position ) * .5;
		//		p = ( p + faceNodes[ 2 ].Position ) * .5;

		//		var dist = Vector3.DistanceSquared( p, wPos );

		//		if( dist < minDist )
		//		{
		//			result = i;
		//			minDist = dist;
		//		}
		//	}

		//	return result;
		//}



		//!!!!
		//unsafe void UpdateGeometryFromPhysicsEngine( out Vector3 worldPosition )
		//{
		//	if( simulatedVertices == null || simulatedVertices.Length != softBody.Nodes.Count )
		//	{
		//		simulatedVertices = new Vector3F[ softBody.Nodes.Count ];
		//		//simulatedVerticesNormals = new Vec3F[ softBody.Nodes.Count ];
		//	}

		//	var worldVertices = new Vector3[ simulatedVertices.Length ];
		//	var worldBounds = Bounds.Cleared;

		//	//get data from Bullet
		//	for( int i = 0; i < softBody.Nodes.Count; i++ )
		//	{
		//		var p = softBody.Nodes[ i ].Position;
		//		var n = softBody.Nodes[ i ].Normal;

		//		worldVertices[ i ] = new Vector3( p.X, p.Y, p.Z );
		//		//simulatedVerticesNormals[ i ] = new Vec3F( (float)n.X, (float)n.Y, (float)n.Z );

		//		worldBounds.Add( ref worldVertices[ i ] );
		//	}

		//	var center = worldBounds.GetCenter();
		//	var localBounds = worldBounds - center;

		//	worldPosition = center;

		//	//calculate local vertices
		//	for( int n = 0; n < simulatedVertices.Length; n++ )
		//	{
		//		simulatedVertices[ n ] = ( worldVertices[ n ] - center ).ToVector3F();
		//		//Vec3F.Subtract( ref worldVertices[ n ], ref center, out vertices[ n ] );
		//	}
		//}

		unsafe void UpdateCreatedMeshData()
		{
			var meshCreated = MeshCreated;

			GetSimulatedData( out var vertices, /*out var normals, */out var indices, out _ );

			int vertexOffset = 0;
			//foreach( var item in meshCreated.Result.RenderOperations )
			{
				var oper = meshCreated.Result.MeshData.RenderOperations[ 0 ];
				//var oper = item.operation;

				//!!!!transform не учитывается

				//positions
				{
					if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out VertexElement element ) &&
						element.Type == VertexElementType.Float3 )
					{
						var buffer = oper.VertexBuffers[ element.Source ];

						var ar = new Vector3F[ buffer.VertexCount ];
						for( int n = 0; n < buffer.VertexCount; n++ )
							ar[ n ] = vertices[ vertexOffset + n ];
						buffer.WriteChannel( element.Offset, ar );
					}
				}

				//!!!!simple implemetation
				//!!!!no sense to calculate in all cases
				//calculate processed normals
				var normals = new Vector3F[ vertices.Length ];
				var tangents = new Vector4F[ vertices.Length ];
				{
					for( int nTriangle = 0; nTriangle < processedIndices.Length / 3; nTriangle++ )
					{
						var index0 = processedIndices[ nTriangle * 3 + 0 ];
						var index1 = processedIndices[ nTriangle * 3 + 1 ];
						var index2 = processedIndices[ nTriangle * 3 + 2 ];

						var v0 = processedVertices[ index0 ];
						var v1 = processedVertices[ index1 ];
						var v2 = processedVertices[ index2 ];

						var normal = Vector3F.Cross( v1 - v0, v2 - v0 ).GetNormalize();
						normals[ index0 ] = normal;
						normals[ index1 ] = normal;
						normals[ index2 ] = normal;

						var tangent = new Vector4F( Vector3F.Cross( ( v1 - v0 ), normal ).GetNormalize(), 1 );
						tangents[ index0 ] = tangent;
						tangents[ index1 ] = tangent;
						tangents[ index2 ] = tangent;
					}
				}

				//normals
				{
					if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Normal, out VertexElement element ) )
					{
						var buffer = oper.VertexBuffers[ element.Source ];

						if( element.Type == VertexElementType.Float3 )
						{
							var ar = new Vector3F[ buffer.VertexCount ];
							for( int n = 0; n < buffer.VertexCount; n++ )
								ar[ n ] = normals[ vertexOffset + n ];
							buffer.WriteChannel( element.Offset, ar );
						}
						else if( element.Type == VertexElementType.Half3 )
						{
							var ar = new Vector3H[ buffer.VertexCount ];
							for( int n = 0; n < buffer.VertexCount; n++ )
								ar[ n ] = normals[ vertexOffset + n ].ToVector3H();
							buffer.WriteChannel( element.Offset, ar );
						}
					}
				}

				//tangents
				{
					if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Tangent, out VertexElement element ) )
					{
						var buffer = oper.VertexBuffers[ element.Source ];

						if( element.Type == VertexElementType.Float4 )
						{
							var ar = new Vector4F[ buffer.VertexCount ];
							for( int n = 0; n < buffer.VertexCount; n++ )
								ar[ n ] = tangents[ vertexOffset + n ];
							buffer.WriteChannel( element.Offset, ar );
						}
						else if( element.Type == VertexElementType.Half4 )
						{
							var ar = new Vector4H[ buffer.VertexCount ];
							for( int n = 0; n < buffer.VertexCount; n++ )
								ar[ n ] = tangents[ vertexOffset + n ].ToVector4H();
							buffer.WriteChannel( element.Offset, ar );
						}
					}
				}



				//fixed ( byte* pVertices = vertexBuffer.Vertices )
				//{
				//	StandardVertexF* pVertex = (StandardVertexF*)pVertices;
				//	for( int n = 0; n < vertexBuffer.VertexCount; n++ )
				//	{
				//		pVertex->position = vertices[ indexOffset + n ];
				//		pVertex->normal = normals[ indexOffset + n ];
				//		pVertex++;
				//	}
				//}
				//vertexBuffer.SetData( vertexBuffer.Vertices );

				vertexOffset += oper.VertexCount;
			}

			//update bounds
			//!!!!maybe sense precalculate bounds
			BoundsF localBounds = BoundsF.Cleared;
			localBounds.Add( vertices );
			meshCreated.Result.SpaceBounds = new SpaceBounds( localBounds );
		}

		//!!!!
		//public override void UpdateDataFromPhysicsEngine()
		//{
		//if( softBody != null )
		//{
		//	UpdateGeometryFromPhysicsEngine( out Vector3 worldPosition );
		//	UpdateCreatedMeshData();

		//	//var bodyTransform = softBody.WorldTransform;
		//	//bodyTransform.Decompose( out Vector3 scale, out Quaternion rotation, out Vector3 translation );
		//	//Log.Info("{0}; {1}; {2}", translation, rotation, scale);

		//	try
		//	{
		//		updatePropertiesWithoutUpdatingBody = true;
		//		Transform = new Transform( worldPosition, Quaternion.Identity );
		//		//Transform = new Transform( BulletUtils.Convert( translation ), BulletUtils.Convert( rotation ), BulletUtils.Convert( scale ) );

		//		//!!!!"Interpolation"
		//		LinearVelocity = BulletPhysicsUtility.Convert( softBody.InterpolationLinearVelocity );

		//		//!!!!AngularVelocity
		//	}
		//	finally
		//	{
		//		updatePropertiesWithoutUpdatingBody = false;
		//	}

		//	//!!!!надо?
		//	SpaceBoundsUpdate();
		//}
		//}

		//public void AddActionBeforeBodyDestroy( Action action )
		//{
		//	actionsBeforeBodyDestroy.Add( action );
		//}

		[Browsable( false )]
		public Mesh MeshCreated
		{
			//!!!!slowly?
			get { return GetComponentByPath( createMeshName ) as Mesh; }
		}

		[Browsable( false )]
		public Mesh MeshOutput
		{
			get
			{
				var m = MeshCreated;
				if( m != null )
					return m;
				else
					return Mesh;
			}
		}

		//!!!!
		//void SetMaterial( Internal.BulletSharp.SoftBody.SoftBody b )
		//{
		//	//material settings
		//	Internal.BulletSharp.SoftBody.Config config = b.Cfg;
		//	//!!!!пока один материал
		//	Internal.BulletSharp.SoftBody.Material bMaterial = b.Materials[ 0 ];
		//	bMaterial.Flags = MaterialFlags.Default;

		//	//!!!!disabled until all parameters fixed (SoftRigidContactHardness, etc).
		//	//PhysicalMaterial material = Material;
		//	//if( material != null )
		//	//{
		//	//	bMaterial.LinearStiffness = material.SoftLinearStiffness;
		//	//	bMaterial.AngularStiffness = material.SoftAngularStiffness;
		//	//	bMaterial.VolumeStiffness = material.SoftVolumeStiffness;
		//	//	config.DynamicFriction = material.Friction;
		//	//}
		//	//else
		//	{
		//		bMaterial.LinearStiffness = MaterialLinearStiffness;
		//		bMaterial.AngularStiffness = MaterialAngularStiffness;
		//		bMaterial.VolumeStiffness = MaterialVolumeStiffness;
		//		config.DynamicFriction = MaterialFriction;
		//	}
		//}

		//!!!!
		//void ApplyConfig()
		//{
		//	Config config = softBody.Cfg;

		//	config.Damping = Damping;

		//	config.Collisions = Collisions.Default;// SdfRigidSoft

		//	//enable cluster collision between soft body and soft body 
		//	//config.Collisions |= Collisions.ClusterClusterSoftSoft;
		//	//enable cluster collision between soft body and rigid body 
		//	//config.Collisions |= Collisions.ClusterConvexRigidSoft;
		//	//config.Collisions |= Collisions.VertexFaceSoftSoft;
		//	//config.Collisions |= Collisions.RigidSoftMask;

		//	if( SelfCollision )
		//		config.Collisions |= Collisions.ClusterSelf;

		//	// Aerodynamics
		//	config.Pressure = Pressure;
		//	config.Drag = Drag;
		//	config.Lift = Lift;

		//	// Pose Matching
		//	if( ShapeMatching )
		//		config.PoseMatching = ShapeCoefficient;
		//	if( VolumeMatching )
		//		config.VolumeConversation = VolumeCoefficient;

		//	config.MaxVolume = MaxVolumeRatio;

		//	//Contacts
		//	config.RigidContactHardness = RigidContactHardness;
		//	config.KineticContactHardness = KineticContactHardness;
		//	config.SoftContactHardness = SoftContactHardness;
		//	config.AnchorHardness = AnchorContactHardness;

		//	//properties for andvanced mode
		//	config.AeroModel = BodyAeroModel;
		//	config.AnchorHardness = AnchorContactHardness;
		//	config.KineticContactHardness = KineticContactHardness;
		//	config.RigidContactHardness = RigidContactHardness;
		//	config.SoftContactHardness = SoftContactHardness;
		//	config.SoftRigidHardness = SoftRigidContactHardness;
		//	config.SoftSoftHardness = SoftSoftContactHardness;
		//	config.SoftKineticHardness = SoftKineticContactHardness;
		//	config.SoftKineticImpulseSplit = SoftKineticImpulseSplit;
		//	config.SoftRigidImpulseSplit = SoftRigidImpulseSplit;
		//	config.SoftSoftImpulseSplit = SoftSoftImpulseSplit;
		//	config.ClusterIterations = ClusterIterations;
		//	config.PositionIterations = PositionIterations;
		//	config.DriftIterations = DriftIterations;
		//	config.VelocityIterations = VelocityIterations;
		//	config.VelocityCorrectionFactor = VelocityCorrectionFactor;
		//	config.Timescale = TimeScale;
		//}

		public void CheckForUpdateDataWhenMeshChanged()
		{
			var needMeshData = Mesh.Value?.Result;

			if( needMeshData != usedMeshDataWhenInitialized )
				RecreateBody();
		}

		//!!!!
		//[Browsable( false )]
		//public override CollisionObject BulletBody
		//{
		//	get { return softBody; }
		//}
	}
}
