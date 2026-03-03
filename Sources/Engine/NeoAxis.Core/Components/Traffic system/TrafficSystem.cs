// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;
using System.Runtime.CompilerServices;

//!!!!
//creatures
//railroads
//serialization in simulation (optionally) ?

namespace NeoAxis
{
	/// <summary>
	/// A system to simulate ground and fly traffic.
	/// </summary>
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Traffic System\Traffic System", 10580 )]
	[SettingsCell( typeof( TrafficSystemSettingsCell ) )]
#endif
	public class TrafficSystem : Component
	{
		static FastRandom staticRandom = new FastRandom( 0 );

		//

		bool needUpdateObjects;

		Dictionary<TrafficSystemElement, ElementCachedData> cachedElementsDictionary;
		ElementCachedData[] cachedGroundVehicleElements;
		ElementCachedData[] cachedFlyingVehicleElements;
		ElementCachedData[] cachedPedestriansElements;

		GroupOfObjectsUtility.GroupOfObjectsInstance groupOfObjects;
		List<GroupOfObjects.SubGroup> groupOfObjectsSubGroups = new List<GroupOfObjects.SubGroup>();

		ESet<ObjectInstance> parkedVehicles = new ESet<ObjectInstance>();
		ESet<ObjectInstance> movingVehicles = new ESet<ObjectInstance>();

		ESet<ObjectInstance> flyingObjects = new ESet<ObjectInstance>();
		double flyingObjectsRemainingTimeToUpdate;
		double flyingObjectsRemainingTimeToUpdateEditor;

		ESet<ObjectInstance> walkingPedestrians = new ESet<ObjectInstance>();
		double walkingPedestriansRemainingTimeToUpdate;
		//double walkingPedestriansRemainingTimeToUpdateEditor;

		//!!!!need support for many camera contexts
		Vector3? lastCameraPosition;
		//Bounds camerasPositionBoundsLastSecond = Bounds.Cleared;
		//float camerasPositionBoundsLastSecondRemainingTime;
		//Queue<Bounds> camerasPositionBoundsLast10Seconds = new Queue<Bounds>();

		List<Road.LogicalData> allWalkableRoadsCache;

		///////////////////////////////////////////////

		/// <summary>
		/// The amount of parked vehicles.
		/// </summary>
		[Category( "Ground Objects" )]
		[DefaultValue( 0 )]
		public Reference<int> ParkedVehicles
		{
			get { if( _parkedVehicles.BeginGet() ) ParkedVehicles = _parkedVehicles.Get( this ); return _parkedVehicles.value; }
			set { if( _parkedVehicles.BeginSet( this, ref value ) ) { try { ParkedVehiclesChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _parkedVehicles.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ParkedVehicles"/> property value changes.</summary>
		public event Action<TrafficSystem> ParkedVehiclesChanged;
		ReferenceField<int> _parkedVehicles = 0;

		public enum ObjectModeEnum
		{
			VehicleComponent,
			StaticObject,
		}

		/// <summary>
		/// The creation mode of parked vehicles.
		/// </summary>
		[Category( "Ground Objects" )]
		[DefaultValue( ObjectModeEnum.VehicleComponent )]
		public Reference<ObjectModeEnum> ParkedVehiclesObjectMode
		{
			get { if( _parkedVehiclesObjectMode.BeginGet() ) ParkedVehiclesObjectMode = _parkedVehiclesObjectMode.Get( this ); return _parkedVehiclesObjectMode.value; }
			set { if( _parkedVehiclesObjectMode.BeginSet( this, ref value ) ) { try { ParkedVehiclesObjectModeChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _parkedVehiclesObjectMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ParkedVehiclesObjectMode"/> property value changes.</summary>
		public event Action<TrafficSystem> ParkedVehiclesObjectModeChanged;
		ReferenceField<ObjectModeEnum> _parkedVehiclesObjectMode = ObjectModeEnum.VehicleComponent;

		/// <summary>
		/// The physics mode of parked vehicles.
		/// </summary>
		[Category( "Ground Objects" )]
		[DefaultValue( PhysicsModeEnum.Basic )]//Kinematic )]
		public Reference<PhysicsModeEnum> ParkedVehiclesPhysicsMode
		{
			get { if( _parkedVehiclesPhysicsMode.BeginGet() ) ParkedVehiclesPhysicsMode = _parkedVehiclesPhysicsMode.Get( this ); return _parkedVehiclesPhysicsMode.value; }
			set { if( _parkedVehiclesPhysicsMode.BeginSet( this, ref value ) ) { try { ParkedVehiclesPhysicsModeChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _parkedVehiclesPhysicsMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ParkedVehiclesPhysicsMode"/> property value changes.</summary>
		public event Action<TrafficSystem> ParkedVehiclesPhysicsModeChanged;
		ReferenceField<PhysicsModeEnum> _parkedVehiclesPhysicsMode = PhysicsModeEnum.Basic;//Kinematic;


		//!!!!temp
		[Category( "Ground Objects" )]
		[DefaultValue( false )]
		public Reference<bool> ParkedVehiclesCanParkOnRoad
		{
			get { if( _parkedVehiclesCanParkOnRoad.BeginGet() ) ParkedVehiclesCanParkOnRoad = _parkedVehiclesCanParkOnRoad.Get( this ); return _parkedVehiclesCanParkOnRoad.value; }
			set { if( _parkedVehiclesCanParkOnRoad.BeginSet( this, ref value ) ) { try { ParkedVehiclesCanParkOnRoadChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _parkedVehiclesCanParkOnRoad.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ParkedVehiclesCanParkOnRoad"/> property value changes.</summary>
		public event Action<TrafficSystem> ParkedVehiclesCanParkOnRoadChanged;
		ReferenceField<bool> _parkedVehiclesCanParkOnRoad = false;

		///////////////////////////////////////////////

		//!!!!impl

		//[Category( "Ground Objects" )]
		//[DefaultValue( 0 )]
		//public Reference<int> MovingVehicles
		//{
		//	get { if( _movingVehicles.BeginGet() ) MovingVehicles = _movingVehicles.Get( this ); return _movingVehicles.value; }
		//	set { if( _movingVehicles.BeginSet( this, ref value ) ) { try { MovingVehiclesChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _movingVehicles.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MovingVehicles"/> property value changes.</summary>
		//public event Action<TrafficSystem> MovingVehiclesChanged;
		//ReferenceField<int> _movingVehicles = 0;

		//[Category( "Ground Objects" )]
		//[DefaultValue( PhysicsModeEnum.Kinematic )]
		//public Reference<PhysicsModeEnum> MovingVehiclesPhysicsMode
		//{
		//	get { if( _movingVehiclesPhysicsMode.BeginGet() ) MovingVehiclesPhysicsMode = _movingVehiclesPhysicsMode.Get( this ); return _movingVehiclesPhysicsMode.value; }
		//	set { if( _movingVehiclesPhysicsMode.BeginSet( this, ref value ) ) { try { MovingVehiclesPhysicsModeChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _movingVehiclesPhysicsMode.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MovingVehiclesPhysicsMode"/> property value changes.</summary>
		//public event Action<TrafficSystem> MovingVehiclesPhysicsModeChanged;
		//ReferenceField<PhysicsModeEnum> _movingVehiclesPhysicsMode = PhysicsModeEnum.Kinematic;



		///////////////////////////////////////////////

		//!!!!flying parked/stopped, flying moving

		/// <summary>
		/// The amount of flying vehicles.
		/// </summary>
		[Category( "Flying Objects" )]
		[DefaultValue( 0 )]
		public Reference<int> FlyingVehicles
		{
			get { if( _flyingVehicles.BeginGet() ) FlyingVehicles = _flyingVehicles.Get( this ); return _flyingVehicles.value; }
			set { if( _flyingVehicles.BeginSet( this, ref value ) ) { try { FlyingVehiclesChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _flyingVehicles.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyingVehicles"/> property value changes.</summary>
		public event Action<TrafficSystem> FlyingVehiclesChanged;
		ReferenceField<int> _flyingVehicles = 0;

		/// <summary>
		/// The physics mode of flying vehicles.
		/// </summary>
		[Category( "Flying Objects" )]
		[DefaultValue( PhysicsModeEnum.None )]
		public Reference<PhysicsModeEnum> FlyingVehiclesPhysicsMode
		{
			get { if( _flyingVehiclesPhysicsMode.BeginGet() ) FlyingVehiclesPhysicsMode = _flyingVehiclesPhysicsMode.Get( this ); return _flyingVehiclesPhysicsMode.value; }
			set { if( _flyingVehiclesPhysicsMode.BeginSet( this, ref value ) ) { try { FlyingVehiclesPhysicsModeChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _flyingVehiclesPhysicsMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyingVehiclesPhysicsMode"/> property value changes.</summary>
		public event Action<TrafficSystem> FlyingVehiclesPhysicsModeChanged;
		ReferenceField<PhysicsModeEnum> _flyingVehiclesPhysicsMode = PhysicsModeEnum.None;

		/// <summary>
		/// The maximal amount of new flying vehicle in one simulation step.
		/// </summary>
		[Category( "Flying Objects" )]
		[DefaultValue( 10 )]
		public Reference<int> FlyingVehiclesCreateMaxPerStep
		{
			get { if( _flyingVehiclesCreateMaxPerStep.BeginGet() ) FlyingVehiclesCreateMaxPerStep = _flyingVehiclesCreateMaxPerStep.Get( this ); return _flyingVehiclesCreateMaxPerStep.value; }
			set { if( _flyingVehiclesCreateMaxPerStep.BeginSet( this, ref value ) ) { try { FlyingVehiclesCreateMaxPerStepChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _flyingVehiclesCreateMaxPerStep.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyingVehiclesCreateMaxPerStep"/> property value changes.</summary>
		public event Action<TrafficSystem> FlyingVehiclesCreateMaxPerStepChanged;
		ReferenceField<int> _flyingVehiclesCreateMaxPerStep = 10;

		/// <summary>
		/// The distance from the camera for new flying objects.
		/// </summary>
		[Category( "Flying Objects" )]
		[DefaultValue( 1100.0 )]
		public Reference<double> FlyingObjectsCreateDistance
		{
			get { if( _flyingObjectsCreateDistance.BeginGet() ) FlyingObjectsCreateDistance = _flyingObjectsCreateDistance.Get( this ); return _flyingObjectsCreateDistance.value; }
			set { if( _flyingObjectsCreateDistance.BeginSet( this, ref value ) ) { try { FlyingObjectsCreateDistanceChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _flyingObjectsCreateDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyingObjectsCreateDistance"/> property value changes.</summary>
		public event Action<TrafficSystem> FlyingObjectsCreateDistanceChanged;
		ReferenceField<double> _flyingObjectsCreateDistance = 1100.0;

		///////////////////////////////////////////////

		/// <summary>
		/// The amount of walking pedestrians.
		/// </summary>
		[Category( "Walking Pedestrians" )]
		[DefaultValue( 0 )]
		public Reference<int> WalkingPedestrians
		{
			get { if( _walkingPedestrians.BeginGet() ) WalkingPedestrians = _walkingPedestrians.Get( this ); return _walkingPedestrians.value; }
			set { if( _walkingPedestrians.BeginSet( this, ref value ) ) { try { WalkingPedestriansChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _walkingPedestrians.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkingPedestrians"/> property value changes.</summary>
		public event Action<TrafficSystem> WalkingPedestriansChanged;
		ReferenceField<int> _walkingPedestrians = 0;

		//!!!!physics mode

		/// <summary>
		/// The maximal amount of new flying vehicle in one simulation step.
		/// </summary>
		[Category( "Walking Pedestrians" )]
		[DefaultValue( 10 )]
		public Reference<int> WalkingPedestriansCreateMaxPerStep
		{
			get { if( _walkingPedestriansCreateMaxPerStep.BeginGet() ) WalkingPedestriansCreateMaxPerStep = _walkingPedestriansCreateMaxPerStep.Get( this ); return _walkingPedestriansCreateMaxPerStep.value; }
			set { if( _walkingPedestriansCreateMaxPerStep.BeginSet( this, ref value ) ) { try { WalkingPedestriansCreateMaxPerStepChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _walkingPedestriansCreateMaxPerStep.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkingPedestriansCreateMaxPerStep"/> property value changes.</summary>
		public event Action<TrafficSystem> WalkingPedestriansCreateMaxPerStepChanged;
		ReferenceField<int> _walkingPedestriansCreateMaxPerStep = 10;

		//!!!!default distance

		/// <summary>
		/// The distance from the camera for new flying objects.
		/// </summary>
		[Category( "Walking Pedestrians" )]
		[DefaultValue( 150.0 )]
		public Reference<double> WalkingPedestriansCreateDistance
		{
			get { if( _walkingPedestriansCreateDistance.BeginGet() ) WalkingPedestriansCreateDistance = _walkingPedestriansCreateDistance.Get( this ); return _walkingPedestriansCreateDistance.value; }
			set { if( _walkingPedestriansCreateDistance.BeginSet( this, ref value ) ) { try { WalkingPedestriansCreateDistanceChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _walkingPedestriansCreateDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkingPedestriansCreateDistance"/> property value changes.</summary>
		public event Action<TrafficSystem> WalkingPedestriansCreateDistanceChanged;
		ReferenceField<double> _walkingPedestriansCreateDistance = 150.0;

		[Category( "Walking Pedestrians" )]
		[DefaultValue( 2.0 )]
		public Reference<double> WalkingPedestriansMinimalDistanceBetweenPedestrians
		{
			get { if( _walkingPedestriansMinimalDistanceBetweenPedestrians.BeginGet() ) WalkingPedestriansMinimalDistanceBetweenPedestrians = _walkingPedestriansMinimalDistanceBetweenPedestrians.Get( this ); return _walkingPedestriansMinimalDistanceBetweenPedestrians.value; }
			set { if( _walkingPedestriansMinimalDistanceBetweenPedestrians.BeginSet( this, ref value ) ) { try { WalkingPedestriansMinimalDistanceBetweenPedestriansChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _walkingPedestriansMinimalDistanceBetweenPedestrians.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkingPedestriansMinimalDistanceBetweenPedestrians"/> property value changes.</summary>
		public event Action<TrafficSystem> WalkingPedestriansMinimalDistanceBetweenPedestriansChanged;
		ReferenceField<double> _walkingPedestriansMinimalDistanceBetweenPedestrians = 2.0;

		[Category( "Walking Pedestrians" )]
		[DefaultValue( true )]
		public Reference<bool> WalkingPedestriansManageTasks
		{
			get { if( _walkingPedestriansManageTasks.BeginGet() ) WalkingPedestriansManageTasks = _walkingPedestriansManageTasks.Get( this ); return _walkingPedestriansManageTasks.value; }
			set { if( _walkingPedestriansManageTasks.BeginSet( this, ref value ) ) { try { WalkingPedestriansManageTasksChanged?.Invoke( this ); WalkingPedestriansUpdateManageTasks(); } finally { _walkingPedestriansManageTasks.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkingPedestriansManageTasks"/> property value changes.</summary>
		public event Action<TrafficSystem> WalkingPedestriansManageTasksChanged;
		ReferenceField<bool> _walkingPedestriansManageTasks = true;

		///////////////////////////////////////////////

		//!!!!?
		//[DefaultValue( 0 )]
		//public Reference<int> IdleHumans
		//{
		//	get { if( _idleHumans.BeginGet() ) IdleHumans = _idleHumans.Get( this ); return _idleHumans.value; }
		//	set { if( _idleHumans.BeginSet( this, ref value ) ) { try { IdleHumansChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _idleHumans.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="IdleHumans"/> property value changes.</summary>
		//public event Action<TrafficSystem> IdleHumansChanged;
		//ReferenceField<int> _idleHumans = 0;

		//!!!!?
		//WalkingCreatures, FlyingCreatures


		//!!!!default
		/// <summary>
		/// The size of the sector in the scene. The sector size allows to optimize the culling and rendering of objects.
		/// </summary>
		[DefaultValue( "200 200 10000" )]//[DefaultValue( "150 150 10000" )]
		[Category( "Optimization" )]
		public Reference<Vector3> StaticObjectsSectorSize
		{
			get { if( _staticObjectsSectorSize.BeginGet() ) StaticObjectsSectorSize = _staticObjectsSectorSize.Get( this ); return _staticObjectsSectorSize.value; }
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
				if( _staticObjectsSectorSize.BeginSet( this, ref value ) ) { try { StaticObjectsSectorSizeChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _staticObjectsSectorSize.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="StaticObjectsSectorSize"/> property value changes.</summary>
		public event Action<TrafficSystem> StaticObjectsSectorSizeChanged;
		ReferenceField<Vector3> _staticObjectsSectorSize = new Vector3( 200, 200, 10000 ); //new Vector3( 150, 150, 10000 );

		////!!!!default
		///// <summary>
		///// The maximal amount of objects in one group/batch.
		///// </summary>
		//[DefaultValue( 2000 )]
		//[Category( "Optimization" )]
		//public Reference<int> StaticObjectsMaxObjectsInGroup
		//{
		//	get { if( _staticObjectsMaxObjectsInGroup.BeginGet() ) StaticObjectsMaxObjectsInGroup = _staticObjectsMaxObjectsInGroup.Get( this ); return _staticObjectsMaxObjectsInGroup.value; }
		//	set { if( _staticObjectsMaxObjectsInGroup.BeginSet( this, ref value ) ) { try { StaticObjectsMaxObjectsInGroupChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _staticObjectsMaxObjectsInGroup.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="StaticObjectsMaxObjectsInGroup"/> property value changes.</summary>
		//public event Action<TrafficSystem> StaticObjectsMaxObjectsInGroupChanged;
		//ReferenceField<int> _staticObjectsMaxObjectsInGroup = 2000;

		//!!!!может разделить на отдельные свойства?
		/// <summary>
		/// Whether to simulate dynamic objects.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Simulation" )]
		public Reference<bool> SimulateDynamicObjects
		{
			get { if( _simulateDynamicObjects.BeginGet() ) SimulateDynamicObjects = _simulateDynamicObjects.Get( this ); return _simulateDynamicObjects.value; }
			set { if( _simulateDynamicObjects.BeginSet( this, ref value ) ) { try { SimulateDynamicObjectsChanged?.Invoke( this ); } finally { _simulateDynamicObjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SimulateDynamicObjects"/> property value changes.</summary>
		public event Action<TrafficSystem> SimulateDynamicObjectsChanged;
		ReferenceField<bool> _simulateDynamicObjects = true;

		///////////////////////////////////////////////

		class ElementCachedData
		{
			public TrafficSystemElement Element;

			//public List<(Mesh, Transform, CollisionModeEnum)> Meshes = new List<(Mesh, Transform, CollisionModeEnum)>();
			public Bounds LocalBounds;
		}

		///////////////////////////////////////////////

		public enum ObjectListEnum
		{
			ParkedVehicles,
			MovingVehicles,
			FlyingObjects,
			WalkingPedestrians,
		}

		///////////////////////////////////////////////

		public class ObjectInstance
		{
			public TrafficSystem Owner;
			public TrafficSystemElement Element;
			public ObjectListEnum ObjectList;

			//!!!!он может становится Static
			//!!!!список?
			public MeshInSpace SceneObject;
			//!!!!список?
			public GroupOfObjects.SubGroup GroupOfObjectsSubGroup;

			//AI
			//!!!!AI can be inside object
			public Vector3 TargetLinearVelocity;

			bool disposed;

			//public object AnyData;

			/////////////////////

			public bool Disposed
			{
				get { return disposed; }
			}

			public void Dispose()
			{
				if( disposed )
					return;

				if( SceneObject != null )
				{
					//!!!!?
					//v.Object.RemoveFromParent( true );
					SceneObject.Dispose();

					SceneObject = null;
				}

				if( GroupOfObjectsSubGroup != null && Owner.groupOfObjects != null )
				{
					Owner.groupOfObjects.RemoveSubGroup( GroupOfObjectsSubGroup );
					GroupOfObjectsSubGroup = null;
				}

				switch( ObjectList )
				{
				case ObjectListEnum.ParkedVehicles:
					Owner.parkedVehicles.Remove( this );
					break;
				case ObjectListEnum.MovingVehicles:
					Owner.movingVehicles.Remove( this );
					break;
				case ObjectListEnum.FlyingObjects:
					Owner.flyingObjects.Remove( this );
					break;
				case ObjectListEnum.WalkingPedestrians:
					Owner.walkingPedestrians.Remove( this );
					break;
				}

				disposed = true;
			}
		}

		///////////////////////////////////////////////

		struct VehicleCreateItem
		{
			public TrafficSystemElement VehicleType;
			public Vector3 Position;
			public QuaternionF Rotation;
		}

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( ParkedVehiclesPhysicsMode ):
				case nameof( ParkedVehiclesObjectMode ):
					if( ParkedVehicles.Value <= 0 )
						skip = true;
					break;

				case nameof( FlyingVehiclesPhysicsMode ):
				case nameof( FlyingVehiclesCreateMaxPerStep ):
				case nameof( FlyingObjectsCreateDistance ):
					if( FlyingVehicles.Value <= 0 )
						skip = true;
					break;

				case nameof( WalkingPedestriansCreateMaxPerStep ):
				case nameof( WalkingPedestriansCreateDistance ):
				case nameof( WalkingPedestriansMinimalDistanceBetweenPedestrians ):
				case nameof( WalkingPedestriansManageTasks ):
					if( WalkingPedestrians.Value <= 0 )
						skip = true;
					break;
				}
			}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			if( Components.Count == 0 )
			{
				{
					var element = CreateComponent<TrafficSystemElement>();
					element.Name = element.BaseType.GetUserFriendlyNameForInstance();
					element.Roles = TrafficSystemElement.RolesEnum.Ground;// | TrafficSystemElement.RolesEnum.Flying;
					element.ObjectType = new Reference<Component>( null, @"Content\Vehicles\Default\Default Vehicle.vehicletype" );
				}

				{
					var element = CreateComponent<TrafficSystemElement>();
					element.Name = element.BaseType.GetUserFriendlyNameForInstance() + " 2";
					element.Roles = TrafficSystemElement.RolesEnum.Ground;
					element.ObjectType = new Reference<Component>( null, @"Content\Characters\NeoAxis\Bryce\Bryce.charactertype" );
					//element.ObjectType = new Reference<Component>( null, @"Content\Characters\Default\Default.charactertype" );
				}
			}
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
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
			}

			if( EnabledInHierarchyAndIsInstance )
			{
				needUpdateObjects = true;
				//UpdateObjects();
			}
			else
			{
				DestroyObjects();

				//lastCameraPosition = null;
				//!!!!что еще очистить
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EnabledInHierarchyAndIsInstance )
			{
				if( needUpdateObjects )
					UpdateObjects();

				//!!!!
				////!!!!here? maybe it simulation step when it is simulation
				//camerasPositionBoundsLastSecondRemainingTime -= delta;
				//if( camerasPositionBoundsLastSecondRemainingTime < 0 )
				//{
				//	if( !camerasPositionBoundsLastSecond.IsCleared() )
				//	{
				//		camerasPositionBoundsLast10Seconds.Enqueue( camerasPositionBoundsLastSecond );
				//		if( camerasPositionBoundsLast10Seconds.Count > 10 )
				//			camerasPositionBoundsLast10Seconds.Dequeue();
				//	}
				//	camerasPositionBoundsLastSecond = Bounds.Cleared;
				//	camerasPositionBoundsLastSecondRemainingTime = 1;
				//}

				if( EngineApp.IsEditor )
				{
					flyingObjectsRemainingTimeToUpdateEditor -= delta;
					if( flyingObjectsRemainingTimeToUpdateEditor < -0.5 )
						flyingObjectsRemainingTimeToUpdateEditor = -0.5;
					while( flyingObjectsRemainingTimeToUpdateEditor <= 0 )
					{
						flyingObjectsRemainingTimeToUpdateEditor += Time.SimulationDelta;
						SimulateFlyingObjects( false );
					}

					//walkingPedestriansRemainingTimeToUpdateEditor -= delta;
					//if( walkingPedestriansRemainingTimeToUpdateEditor < -0.5 )
					//	walkingPedestriansRemainingTimeToUpdateEditor = -0.5;
					//while( walkingPedestriansRemainingTimeToUpdateEditor <= 0 )
					//{
					//	walkingPedestriansRemainingTimeToUpdateEditor += Time.SimulationDelta;
					//	SimulateWalkingPedestrians( false );
					//}
				}
			}
		}

		void NeedUpdateObjects()
		{
			needUpdateObjects = true;
		}

		static Road.LogicalData[] GetDrivingRoads( Scene scene )//, bool forGroundVehicles, bool forWalking )
		{
			var roads = new List<Road.LogicalData>( 64 );

			foreach( var road in scene.GetComponents<Road>( checkChildren: true, onlyEnabledInHierarchy: true ) )
			{
				var roadData = road.GetLogicalData();
				if( roadData != null )
				{
					var wayToUse = roadData.RoadData.RoadType.WayToUse.Value;
					if( wayToUse == RoadType.WayToUseEnum.Driving )//&& forGroundVehicles || wayToUse == RoadType.WayToUseEnum.Walking && forWalking )
						roads.Add( roadData );
				}
			}

			return roads.ToArray();
		}

		static ElementCachedData CreateElementCachedData( TrafficSystemElement element )
		{
			var objectType = element.ObjectType.Value;
			if( objectType != null )
			{
				var data = new ElementCachedData();
				data.Element = element;


				var vehicleType = data.Element.ObjectType.Value as VehicleType;
				if( vehicleType != null )
				{
					foreach( var item in vehicleType.GetMeshesInDefaultState() )
					{
						var b = item.Mesh.Result.SpaceBounds.BoundingBox;
						foreach( var p in b.ToPoints() )
							data.LocalBounds.Add( item.Transform * p );
					}
				}


				//foreach( var meshInSpace in objectType.GetComponents<MeshInSpace>() )
				//{
				//	var mesh = meshInSpace.Mesh.Value;
				//	if( mesh != null )
				//	{
				//		//!!!!
				//		var tr = Transform.Identity;
				//		//var tr = meshInSpace.TransformV;

				//		data.Meshes.Add( (mesh, tr, CollisionModeEnum.Basic) );

				//		//var b = mesh.Result.SpaceBounds.CalculatedBoundingBox;
				//		//foreach( var p in b.ToPoints() )
				//		//	data.LocalBounds.Add( tr * p );
				//	}
				//}

				////create wheels
				//{
				//	var wheelDrive = objectType.WheelDrive.Value;
				//	var front = wheelDrive == Vehicle.WheelDriveEnum.Front || wheelDrive == Vehicle.WheelDriveEnum.All;
				//	var rear = wheelDrive == Vehicle.WheelDriveEnum.Rear || wheelDrive == Vehicle.WheelDriveEnum.All;

				//	for( int n = 0; n < 4/*dynamicData.Wheels.Length*/; n++ )
				//	{
				//		//init basic data

				//		var wheel = new Vehicle.DynamicData.WheelData();
				//		wheel.Which = (Vehicle.DynamicData.WhichWheel)n;

				//		if( ( wheel.Which == Vehicle.DynamicData.WhichWheel.FrontLeft || wheel.Which == Vehicle.DynamicData.WhichWheel.FrontRight ) && front )
				//			wheel.WheelDrive = true;
				//		if( ( wheel.Which == Vehicle.DynamicData.WhichWheel.RearLeft || wheel.Which == Vehicle.DynamicData.WhichWheel.RearRight ) && rear )
				//			wheel.WheelDrive = true;

				//		switch( wheel.Which )
				//		{
				//		case Vehicle.DynamicData.WhichWheel.FrontLeft:
				//			wheel.LocalPosition = objectType.FrontWheelPosition.Value;
				//			if( wheel.LocalPosition.Y < 0 )
				//				wheel.LocalPosition.Y = -wheel.LocalPosition.Y;
				//			break;

				//		case Vehicle.DynamicData.WhichWheel.FrontRight:
				//			wheel.LocalPosition = objectType.FrontWheelPosition.Value;
				//			if( wheel.LocalPosition.Y > 0 )
				//				wheel.LocalPosition.Y = -wheel.LocalPosition.Y;
				//			break;

				//		case Vehicle.DynamicData.WhichWheel.RearLeft:
				//			wheel.LocalPosition = objectType.RearWheelPosition.Value;
				//			if( wheel.LocalPosition.Y < 0 )
				//				wheel.LocalPosition.Y = -wheel.LocalPosition.Y;
				//			break;

				//		case Vehicle.DynamicData.WhichWheel.RearRight:
				//			wheel.LocalPosition = objectType.RearWheelPosition.Value;
				//			if( wheel.LocalPosition.Y > 0 )
				//				wheel.LocalPosition.Y = -wheel.LocalPosition.Y;
				//			break;
				//		}

				//		switch( wheel.Which )
				//		{
				//		case Vehicle.DynamicData.WhichWheel.FrontLeft:
				//		case Vehicle.DynamicData.WhichWheel.FrontRight:
				//			wheel.Diameter = objectType.FrontWheelDiameter;
				//			wheel.Width = objectType.FrontWheelWidth;
				//			break;

				//		case Vehicle.DynamicData.WhichWheel.RearLeft:
				//		case Vehicle.DynamicData.WhichWheel.RearRight:
				//			wheel.Diameter = objectType.RearWheelDiameter;
				//			wheel.Width = objectType.RearWheelWidth;
				//			break;
				//		}

				//		var mesh = wheel.Front ? objectType.FrontWheelMesh : objectType.RearWheelMesh;

				//		var wheelTransform = new Transform( wheel.LocalPosition, Quaternion.Identity );
				//		if( wheel.Right )
				//			wheelTransform *= new Transform( Vector3.Zero, Quaternion.FromRotateByZ( Math.PI ) );

				//		if( mesh.Value != null )
				//			data.Meshes.Add( (mesh, wheelTransform, CollisionModeEnum.Full) );
				//	}
				//}

				////calculate local bounds
				//data.LocalBounds = Bounds.Cleared;
				//foreach( var item in data.Meshes )
				//{
				//	var mesh = item.Item1;
				//	var tr = item.Item2;

				//	var b = mesh.Result.SpaceBounds.CalculatedBoundingBox;
				//	foreach( var p in b.ToPoints() )
				//		data.LocalBounds.Add( tr * p );
				//}

				//if( data.Meshes.Count != 0 )
				//	return data;

				return data;
			}
			return null;
		}

		void CalculateCachedElements()
		{
			cachedElementsDictionary = new Dictionary<TrafficSystemElement, ElementCachedData>();
			foreach( var c in GetComponents<TrafficSystemElement>() )
			{
				if( c.Enabled && c.ObjectType.Value != null )
				{
					var data = CreateElementCachedData( c );
					if( data != null )
						cachedElementsDictionary[ c ] = data;
				}
			}

			var all = cachedElementsDictionary.Values.ToArray();
			cachedGroundVehicleElements = all.Where( t => ( t.Element.Roles.Value & TrafficSystemElement.RolesEnum.Ground ) != 0 && t.Element.ObjectType.Value as VehicleType != null ).ToArray();
			cachedFlyingVehicleElements = all.Where( t => ( t.Element.Roles.Value & TrafficSystemElement.RolesEnum.Flying ) != 0 && t.Element.ObjectType.Value as VehicleType != null ).ToArray();
			cachedPedestriansElements = all.Where( t => ( t.Element.Roles.Value & TrafficSystemElement.RolesEnum.Ground ) != 0 && t.Element.ObjectType.Value as CharacterType != null ).ToArray();
		}

		ElementCachedData GetElementCachedData( TrafficSystemElement element )
		{
			cachedElementsDictionary.TryGetValue( element, out var data );
			return data;
		}

		OpenList<VehicleCreateItem> GetVehiclesToCreate( Road.LogicalData[] roads, ElementCachedData[] elementsToCreate, bool movingVehicles )
		{
			//!!!!movingVehicles
			var maxVehiclesAtThisStep = ParkedVehicles.Value;

			var random = new FastRandom( 0 );

			var vehicleItems = new OpenList<VehicleCreateItem>( maxVehiclesAtThisStep );
			//var vehiclesAtThisStep = 0;

			if( maxVehiclesAtThisStep > 0 )
			{
				//!!!!
				var initSettings = new OctreeContainer.InitSettings();
				//!!!!
				//initSettings.InitialOctreeBounds = totalBounds;
				//initSettings.OctreeBoundsRebuildExpand = Vector3.Zero;
				//initSettings.MinNodeSize = totalBounds.GetSize() / 40;
				using( var octree = new OctreeContainer( initSettings ) )
				{
					var maxTryCount = maxVehiclesAtThisStep * 10;

					for( int n = 0; n < maxTryCount; n++ )
					{
						var vehicle = new VehicleCreateItem();

						//!!!!probability
						var typeIndex = random.Next( elementsToCreate.Length - 1 );
						vehicle.VehicleType = elementsToCreate[ typeIndex ].Element;


						var road = roads[ random.Next( roads.Length - 1 ) ];
						var maxTime = road.RoadData.LastPoint.TimeOnCurve;

						//!!!!
						if( movingVehicles && road.RoadData.Lanes <= 2 )
							continue;

						//var time = random.Next( maxTime );
						//var pos = road.GetPositionByTime( time );

						//!!!!
						//road.GetLaneOffset( 0 );


						var time = random.Next( maxTime );

						//check by time clip ranges
						var allowByTimeClipRanges = false;
						var selectedTimeClipRange = Range.Zero;
						{
							foreach( var timeClipRange in road.GetTimeClipRanges() )
							{
								if( time >= timeClipRange.Minimum && time <= timeClipRange.Maximum )
								{
									allowByTimeClipRanges = true;
									selectedTimeClipRange = timeClipRange;
									break;
								}
							}
						}

						if( allowByTimeClipRanges )
						{
							road.RoadData.GetPositionByTime( time, out var pos );
							road.RoadData.GetDirectionByTime( time, out var dir );

							//check it on start or on end of the road
							road.RoadData.GetPositionByTime( selectedTimeClipRange.Minimum, out var min );
							road.RoadData.GetPositionByTime( selectedTimeClipRange.Maximum, out var max );
							var distanceStart = ( min - pos ).Length();
							var distanceEnd = ( max - pos ).Length();
							//var distanceStart = ( road.RoadData.GetPositionByTime( selectedTimeClipRange.Minimum ) - pos ).Length();
							//var distanceEnd = ( road.RoadData.GetPositionByTime( selectedTimeClipRange.Maximum ) - pos ).Length();

							//!!!!property
							const double minOffset = 5.0;// 10.0;

							if( distanceStart > minOffset && distanceEnd > minOffset )
							{
								var laneOffset = 0.0;

								if( movingVehicles )
								{
									var lane = random.Next( road.RoadData.Lanes - 2 - 1 ) + 1;
									laneOffset = road.RoadData.GetLaneOffset( lane );
								}
								else
								{
									var onRoad = ParkedVehiclesCanParkOnRoad && random.Next( 3 ) == 0;
									//var onRoad = ParkedVehiclesCanParkOnRoad && random.NextBoolean();
									if( onRoad )
									{
										var lane = random.Next( Math.Max( road.RoadData.Lanes - 2 - 1, 0 ) ) + 1;
										laneOffset = road.RoadData.GetLaneOffset( lane );
									}
									else
									{
										var side = random.NextBoolean();
										laneOffset = road.RoadData.GetLaneOffset( side ? 0 : road.RoadData.Lanes - 1 );
										if( laneOffset < 0 )
											laneOffset -= 0.5;
										else
											laneOffset += 0.5;
									}
								}

								var dir2 = dir;
								if( laneOffset > 0 )
									dir2 = -dir2;

								var vehicleTypeData = GetElementCachedData( vehicle.VehicleType );

								vehicle.Rotation = QuaternionF.FromDirectionZAxisUp( dir2.ToVector3F() );

								vehicle.Position = pos + QuaternionF.FromDirectionZAxisUp( dir.ToVector3F() ) * new Vector3F( 0, (float)laneOffset, 0 );

								//!!!!
								vehicle.Position -= new Vector3( 0, 0, vehicleTypeData.LocalBounds.Minimum.Z );
								//vehicle.Position -= new Vector3( 0, 0, vehicleTypeData.LocalBounds.Minimum.Z + 0.02 );

								//!!!!precalculate
								var boundingRadius = 0.0;
								foreach( var p in vehicleTypeData.LocalBounds.ToPoints() )
									boundingRadius = Math.Max( boundingRadius, p.Length() );

								//var boundingRadius = mesh.Result.SpaceBounds.BoundingSphere.Value.Radius;
								var sphere = new Sphere( vehicle.Position, boundingRadius );

								if( octree.GetObjects( sphere, 1, OctreeContainer.ModeEnum.One ).Length == 0 )
								{
									octree.AddObject( sphere.ToBounds(), 1 );

									vehicleItems.Add( ref vehicle );
									//vehiclesAtThisStep++;

									if( vehicleItems.Count >= maxVehiclesAtThisStep )
										break;
									//if( vehiclesAtThisStep >= maxVehiclesAtThisStep )
									//	break;
								}
							}
						}
					}
				}
			}

			return vehicleItems;
		}

		void UpdateParkedVehicles()
		{
			var scene = FindParent<Scene>();
			if( scene == null )
				return;

			var roads = GetDrivingRoads( scene );//, true, false );
			if( roads.Length == 0 )
				return;
			foreach( var road in roads )
				road.DropTimeClipRanges();

			if( cachedGroundVehicleElements == null )
				return;

			var elementsToCreate = cachedGroundVehicleElements.Where( e => e.Element.ObjectType.Value as VehicleType != null ).ToArray();
			if( elementsToCreate.Length == 0 )
				return;

			var vehicleItems = GetVehiclesToCreate( roads, elementsToCreate, false );
			if( vehicleItems.Count != 0 )
			{
				if( ParkedVehiclesObjectMode.Value == ObjectModeEnum.VehicleComponent )
				{
					for( int n = 0; n < vehicleItems.Count; n++ )
					{
						var vehicleItem = vehicleItems[ n ];
						var element = vehicleItem.VehicleType;
						var tr = new Transform( vehicleItem.Position, vehicleItem.Rotation, Vector3.One );

						//create sleeping (static)

						var isStatic = true;

						var objectInstance = CreateObject( element, ObjectListEnum.ParkedVehicles, tr, isStatic );
						if( objectInstance != null )
						{
							//deactivate physics body
							var vehicle = objectInstance.SceneObject as Vehicle;
							if( vehicle != null && vehicle.PhysicalBody != null )
								vehicle.PhysicalBody.Active = false;
						}
					}
				}
				else
				{
					groupOfObjects = GroupOfObjectsUtility.GetOrCreateGroupOfObjects( scene, "__GroupOfObjectsTrafficSystem", true, StaticObjectsSectorSize );//, StaticObjectsMaxObjectsInGroup );

					if( groupOfObjects != null )
					{
						var objects = new OpenList<GroupOfObjects.Object>( vehicleItems.Count * 5 );

						for( int n = 0; n < vehicleItems.Count; n++ )
						{
							var vehicleItem = vehicleItems[ n ];
							var element = vehicleItem.VehicleType;
							var tr = new Transform( vehicleItem.Position, vehicleItem.Rotation, Vector3.One );

							var vehicleType = element.ObjectType.Value as VehicleType;
							if( vehicleType != null )
							{
								//!!!!slowly?
								var defaultState = vehicleType.GetMeshesInDefaultState();

								for( int nMesh = 0; nMesh < defaultState.Length; nMesh++ )//foreach( var item in vehicleType.GetMeshesInDefaultState() )
								{
									var item = defaultState[ nMesh ];

									var tr2 = tr * item.Transform;
									var pos = tr2.Position;
									var rot = tr2.Rotation;
									var scl = tr2.Scale;

									//make collision only for first mesh
									var collision = ParkedVehiclesPhysicsMode.Value != PhysicsModeEnum.None && nMesh == 0;

									var elementIndex = groupOfObjects.GetOrCreateGroupOfObjectsElement( item.Mesh, null, true, 1, true, 1, true, collision );

									var obj = new GroupOfObjects.Object( elementIndex, 0, 0, GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible, pos, rot.ToQuaternionF(), scl.ToVector3F(), Vector4F.Zero, ColorValue.One, Vector4F.Zero, Vector4F.Zero, 0 );

									objects.Add( ref obj );
								}
							}
						}

						if( objects.Count != 0 )
						{
							var subGroup = new GroupOfObjects.SubGroup( objects.ArraySegment );
							groupOfObjects.AddSubGroup( subGroup );
							groupOfObjectsSubGroups.Add( subGroup );
						}
					}
				}
			}
		}

		//void UpdateCharacters()
		//{
		//	DestroyCharacters();

		//	var scene = FindParent<Scene>();
		//	if( scene == null )
		//		return;

		//	//!!!!
		//}

		//void DestroyCharacters()
		//{
		//	//!!!!
		//}

		public void UpdateObjects()
		{
			DestroyObjects();

			if( !EnabledInHierarchyAndIsInstance )
				return;
			//!!!!
			if( !lastCameraPosition.HasValue )
				return;
			//if( !GetCamerasPosition().HasValue )
			//	return;

			CalculateCachedElements();
			UpdateParkedVehicles();

			SimulateFlyingObjects( true );
			SimulateWalkingPedestrians( true );

			needUpdateObjects = false;
		}

		void DestroyObjects()
		{
			while( parkedVehicles.Count != 0 )
				parkedVehicles.First().Dispose();
			while( movingVehicles.Count != 0 )
				movingVehicles.First().Dispose();
			while( flyingObjects.Count != 0 )
				flyingObjects.First().Dispose();
			while( walkingPedestrians.Count != 0 )
				walkingPedestrians.First().Dispose();

			if( groupOfObjects != null )
			{
				foreach( var subGroup in groupOfObjectsSubGroups )
					groupOfObjects.RemoveSubGroup( subGroup );
				groupOfObjectsSubGroups.Clear();

				//don't delete because can be several TrafficSystem components
				////delete group of objects to update sector size and max objects count
				//groupOfObjects?.Dispose();

				groupOfObjects = null;
			}

			allWalkableRoadsCache = null;
		}

		public ObjectInstance CreateObject( TrafficSystemElement element, ObjectListEnum objectList, Transform transform, bool isStatic )
		{
			var objectType = element.ObjectType.Value;
			if( objectType != null )
			{
				var objectInstance = new ObjectInstance();
				objectInstance.Owner = this;
				objectInstance.Element = element;
				objectInstance.ObjectList = objectList;

				switch( objectInstance.ObjectList )
				{
				case ObjectListEnum.ParkedVehicles:
					parkedVehicles.AddWithCheckAlreadyContained( objectInstance );
					break;
				case ObjectListEnum.MovingVehicles:
					movingVehicles.AddWithCheckAlreadyContained( objectInstance );
					break;
				case ObjectListEnum.FlyingObjects:
					flyingObjects.AddWithCheckAlreadyContained( objectInstance );
					break;
				case ObjectListEnum.WalkingPedestrians:
					walkingPedestrians.AddWithCheckAlreadyContained( objectInstance );
					break;
				}

				Metadata.TypeInfo typeToCreate = null;
				var vehicleType = objectType as VehicleType;
				if( vehicleType != null )
					typeToCreate = MetadataManager.GetTypeOfNetType( typeof( Vehicle ) );
				var characterType = objectType as CharacterType;
				if( characterType != null )
					typeToCreate = MetadataManager.GetTypeOfNetType( typeof( Character ) );

				//!!!!support all other types. clone component if no known type?

				if( typeToCreate != null && MetadataManager.GetTypeOfNetType( typeof( MeshInSpace ) ).IsAssignableFrom( typeToCreate ) )
				{
					//need set ShowInEditor = false before AddComponent
					var obj = (MeshInSpace)ComponentUtility.CreateComponent( typeToCreate, null, false, false );
					obj.DisplayInEditor = false;
					AddComponent( obj, -1 );
					//var obj = scene.CreateComponent<MeshInSpace>();

					obj.SaveSupport = false;
					obj.CloneSupport = false;
					obj.CanBeSelected = false;
					obj.NetworkMode = NetworkModeEnum.False;
					obj.Transform = transform;

					var vehicle = obj as Vehicle;
					if( vehicle != null )
					{
						if( vehicleType != null )
							vehicle.VehicleType = vehicleType;

						if( objectList == ObjectListEnum.ParkedVehicles )
							vehicle.PhysicsMode = ParkedVehiclesPhysicsMode;
						else if( objectList == ObjectListEnum.FlyingObjects )
							vehicle.PhysicsMode = FlyingVehiclesPhysicsMode;
						//else if( objectList == ObjectListEnum.MovingVehicles)
						//	vehicle.PhysicsMode = MovingVehiclesPhysicsMode;

						//!!!!
						//vehicle.PhysicsMode = PhysicsModeEnum.None;
					}

					var character = obj as Character;
					if( character != null )
					{
						if( characterType != null )
							character.CharacterType = characterType;

						//physics mode
					}

					obj.Static = isStatic;
					obj.Enabled = true;

					objectInstance.SceneObject = obj;
				}

				return objectInstance;
			}

			return null;
		}

		//!!!!
		////!!!!лучше отдельно их хранить если можем видеть несколько камер на большом расстоянии
		//public Vector2? GetCamerasPosition()
		//{
		//	zzzzz;

		//	var result = camerasPositionBoundsLastSecond;//Bounds.Cleared;
		//	foreach( var b in camerasPositionBoundsLast10Seconds )
		//		result.Add( b );

		//	if( result.IsCleared() )
		//		return null;

		//	return result.GetCenter();
		//}

		//public Rectangle GetCamerasBounds()
		//{
		//	var result = camerasBoundsLastSecond;//Bounds.Cleared;
		//	foreach( var b in camerasBoundsLast10Seconds )
		//		result.Add( b );
		//	return result;
		//}

		//public Vector2? GetCamerasCenter()
		//{
		//	var b = GetCamerasBounds();
		//	if( b.IsCleared() )
		//		return null;
		//	return b.GetCenter();
		//}

		protected virtual ObjectInstance CreateFlyingObject( bool initialization )
		{
			//var camerasPosition = GetCamerasPosition();

			if( cachedFlyingVehicleElements != null && cachedFlyingVehicleElements.Length != 0 && lastCameraPosition/*camerasPosition*/.HasValue )
			{
				var cameraPosition = lastCameraPosition.Value.ToVector2();

				var typeIndex = staticRandom.Next( cachedFlyingVehicleElements.Length - 1 );
				var type = cachedFlyingVehicleElements[ typeIndex ];
				var element = type.Element;

				var heightRange = element.FlyingHeightRange.Value;
				var createDistance = FlyingObjectsCreateDistance.Value;

				//calculate position
				Vector3? pos = null;
				for( int nIteration = 0; nIteration < 20; nIteration++ )
				{
					//!!!!check for free place
					var height = staticRandom.Next( heightRange.Minimum, heightRange.Maximum );

					if( !initialization )
					{
						//create on far border

						var angle = staticRandom.Next( 0, Math.PI * 2 );

						var offset = new Vector2( Math.Cos( angle ), Math.Sin( angle ) ) * createDistance;
						pos = new Vector3( cameraPosition + offset, height );
					}
					else
					{
						//create anywhere in radius

						var offset = new Vector2( staticRandom.Next( -createDistance, createDistance ), staticRandom.Next( -createDistance, createDistance ) );

						//check by distance
						if( offset.LengthSquared() < createDistance * createDistance )
							pos = new Vector3( cameraPosition + offset, height );
					}
				}

				if( pos.HasValue )
				{
					var speedRange = element.FlyingSpeedRange.Value;
					var speed = staticRandom.Next( speedRange.Minimum, speedRange.Maximum );

					var destX = staticRandom.Next( cameraPosition.X - createDistance, cameraPosition.X + createDistance );
					var destY = staticRandom.Next( cameraPosition.Y - createDistance, cameraPosition.Y + createDistance );
					var destPos = new Vector2( destX, destY );
					if( destPos == pos.Value.ToVector2() )
						destPos.X += 1;
					var dir = ( destPos - pos.Value.ToVector2() ).GetNormalize();

					var rot = Quaternion.FromDirectionZAxisUp( new Vector3( dir, 0 ) );
					var scl = Vector3.One;

					var tr = new Transform( pos.Value, rot, scl );

					var objectInstance = CreateObject( element, ObjectListEnum.FlyingObjects, tr, false );
					if( objectInstance != null )
					{
						objectInstance.TargetLinearVelocity = rot.GetForward() * speed;
						return objectInstance;
					}
				}
			}

			return null;
		}

		void SimulateFlyingObjects( bool initialization )
		{
			//simulate created objects
			foreach( var obj in flyingObjects.ToArray() ) //make copy to change flyingObjects
			{
				//check for already destroyed
				if( obj.SceneObject != null && obj.SceneObject.Parent == null )
				{
					obj.Dispose();
					continue;
				}

				if( obj.TargetLinearVelocity != Vector3.Zero )
				{
					if( obj.SceneObject != null )
					{

						//!!!!помимо предсказания bounds можно тут реже делать шаг для объектов вдалеке


						var previousTransform = obj.SceneObject.TransformV;

						var pos = previousTransform.Position + obj.TargetLinearVelocity * Time.SimulationDelta;
						//var rot = Quaternion.FromDirectionZAxisUp( v.TargetLinearVelocity );


						var tr = new Transform( pos, previousTransform.Rotation, previousTransform.Scale );

						var vehicle = obj.SceneObject as Vehicle;
						if( vehicle != null )
						{
							vehicle.LinearVelocityToPredictBounds = obj.TargetLinearVelocity;
							vehicle.SetTransform( tr, false );
							//vehicle.SetTransformWithoutRecreating( tr );
						}
						else
							obj.SceneObject.Transform = tr;
					}
				}
			}

			//create, destroy objects
			flyingObjectsRemainingTimeToUpdate -= Time.SimulationDelta;
			if( flyingObjectsRemainingTimeToUpdate <= 0 || initialization )
			{
				flyingObjectsRemainingTimeToUpdate = 1 + staticRandom.Next( 0.1 );

				//var camerasBounds = GetCamerasBounds();
				//if( !camerasBounds.IsCleared() )

				//var camerasPosition = GetCamerasPosition();
				if( lastCameraPosition.HasValue )
				{
					var cameraPosition = lastCameraPosition.Value.ToVector2();

					//create new vehicles
					var createObjectLimit = initialization ? FlyingVehicles : FlyingVehiclesCreateMaxPerStep;
					for( int n = 0; n < createObjectLimit; n++ )
					{
						if( flyingObjects.Count < FlyingVehicles )
						{
							var v = CreateFlyingObject( initialization );
							if( v == null )
								break;
						}
					}

					//destroy when far away
					{
						var toDestroy = new List<ObjectInstance>( 32 );

						var maxDistance = FlyingObjectsCreateDistance.Value * 1.3;
						var maxDistanceSquared = maxDistance * maxDistance;

						foreach( var v in flyingObjects )
						{
							if( v.SceneObject != null )
							{
								var pos = v.SceneObject.TransformV.Position.ToVector2();

								var distanceSquared = ( cameraPosition - pos ).LengthSquared();
								//var distanceSquared = camerasBounds.GetPointDistanceSquared( ref pos );
								if( distanceSquared > maxDistanceSquared )
									toDestroy.Add( v );
							}
						}

						foreach( var v in toDestroy )
							v.Dispose();
					}
				}
			}
		}

		List<Road.LogicalData> GetAllWalkableRoads()
		{
			if( allWalkableRoadsCache == null )
			{
				allWalkableRoadsCache = new List<Road.LogicalData>( 64 );

				var scene = FindParent<Scene>();
				if( scene != null )
				{
					foreach( var road in scene.GetComponents<Road>( checkChildren: true, onlyEnabledInHierarchy: true ) )
					{
						var roadData = road.GetLogicalData();
						if( roadData != null )
						{
							var wayToUse = roadData.RoadData.RoadType.WayToUse.Value;
							if( wayToUse == RoadType.WayToUseEnum.Walking )
								allWalkableRoadsCache.Add( roadData );
						}
					}
				}
			}

			return allWalkableRoadsCache;
		}

		List<Road.LogicalData> GetWalkableRoadsByCircle( Circle circle )// Sphere sphere )
		{
			//!!!!slowly?

			var result = new List<Road.LogicalData>( 64 );

			foreach( var road in GetAllWalkableRoads() )
			{
				var roadBounds = road.RoadData.GetBounds().ToRectangle();
				if( circle.Intersects( ref roadBounds ) )
				{
					//!!!!можно более точно определить хотя дольше

					result.Add( road );
				}
			}

			return result;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected virtual ObjectInstance CreateWalkingPedestrian( Scene scene, bool initialization, Vector3 cameraPosition, Circle circle, Circle circleFar, List<Road.LogicalData> roadsInRadius )
		{

			//!!!!Parallel, threading?

			//var camerasPosition = GetCamerasPosition();

			//get object type
			TrafficSystemElement element = null;
			if( cachedPedestriansElements != null && cachedPedestriansElements.Length != 0 )//&& camerasPosition.HasValue )
			{
				var cachedElement = cachedPedestriansElements[ staticRandom.Next( cachedPedestriansElements.Length - 1 ) ];
				element = cachedElement.Element;
			}

			if( element != null )
			{
				var characterType = element.ObjectType.Value as CharacterType;
				if( characterType != null )
				{
					//get walking road and place on it

					//calculate position
					Vector3? freePosition = null;
					Vector2 selectedDirection = Vector2.Zero;
					Road selectedRoad = null;
					int selectedRoadLane = 0;

					//!!!!10?
					for( int nIteration = 0; nIteration < 10; nIteration++ )//for( int nIteration = 0; nIteration < 20; nIteration++ )
					{
						//get random point in circles inverval
						Vector2 point;
						{
							var angle = staticRandom.Next( MathEx.PI * 2 );
							double radius;
							if( initialization )
								radius = staticRandom.Next( circle.Radius );
							else
								radius = staticRandom.Next( circle.Radius, circleFar.Radius );
							point = circle.Center + new Vector2( Math.Cos( angle ), Math.Sin( angle ) ) * radius;
						}

						//find nearest road to point
						Road.LogicalData nearestRoad = null;
						var nearestRoadDistanceToPointSquared = 0.0;
						var nearestRoadTimeOnCurve = 0.0;

						for( int n = 0; n < roadsInRadius.Count; n++ )
						{
							var road = roadsInRadius[ n ];
							var roadData = road.RoadData;

							//!!!!good?
							var point3 = new Vector3( point, cameraPosition.Z );

							if( roadData.GetClosestCurveTimeToPosition( point3, circleFar.Radius, 1.0, out var timeOnCurve ) )
							{
								roadData.GetPositionByTime( timeOnCurve, out var positionOnCurve );
								var positionOnCurve2 = positionOnCurve.ToVector2();

								var distanceToPointSquared = ( positionOnCurve2 - point ).LengthSquared();
								if( nearestRoad == null || distanceToPointSquared < nearestRoadDistanceToPointSquared )
								{
									bool radiusCheckGood;
									if( initialization )
										radiusCheckGood = circle.Contains( ref positionOnCurve2 );
									else
										radiusCheckGood = circleFar.Contains( ref positionOnCurve2 ) && !circle.Contains( ref positionOnCurve2 );

									if( radiusCheckGood )
									{
										nearestRoad = road;
										nearestRoadDistanceToPointSquared = distanceToPointSquared;
										nearestRoadTimeOnCurve = timeOnCurve;
									}
								}
							}
						}

						if( nearestRoad != null )
						{
							var roadData = nearestRoad.RoadData;

							//get lane
							var lane = staticRandom.Next( roadData.Lanes - 1 );
							var laneOffset = roadData.GetLaneOffset( lane );

							roadData.GetPositionByTime( nearestRoadTimeOnCurve, out var positionOnCurve );
							roadData.GetDirectionByTime( nearestRoadTimeOnCurve, out var roadDirection );

							var directionDependingLaneVector = roadDirection;
							if( laneOffset > 0 )
								directionDependingLaneVector = -directionDependingLaneVector;

							var positionOnLane = positionOnCurve + QuaternionF.FromDirectionZAxisUp( roadDirection.ToVector3F() ) * new Vector3F( 0, (float)laneOffset, 0 );
							var positionOnLane2 = positionOnLane.ToVector2();

							bool radiusCheckGood;
							if( initialization )
								radiusCheckGood = circle.Contains( ref positionOnLane2 );
							else
								radiusCheckGood = circleFar.Contains( ref positionOnLane2 ) && !circle.Contains( ref positionOnLane2 );

							if( radiusCheckGood )
							{
								if( CharacterUtility.FindFreePlace( scene, characterType.Height, characterType.Radius, positionOnLane, 0, -0.25, 0.25, null, out var freePosition2 ) )
								{
									if( WalkingPedestriansMinimalDistanceBetweenPedestrians.Value == 0 || CharacterUtility.FindClosestCharacter( scene, new Sphere( freePosition2, WalkingPedestriansMinimalDistanceBetweenPedestrians ) ) == null )
									{
										freePosition = freePosition2;
										selectedDirection = directionDependingLaneVector.ToVector2();
										selectedRoad = roadData.Owner;
										selectedRoadLane = lane;
										break;
									}
								}
							}
						}
					}


					//for( int nIteration = 0; nIteration < 20; nIteration++ )
					//{
					//	//select road
					//	var road = roadsInRadius[ staticRandom.Next( roadsInRadius.Count - 1 ) ];
					//	var roadData = road.RoadData;

					//	//get positon on curve
					//	var maxTime = roadData.LastPoint.TimeOnCurve;
					//	var time = staticRandom.Next( maxTime );
					//	roadData.GetPositionByTime( time, out var position );

					//	//get lane
					//	var lane = staticRandom.Next( roadData.Lanes - 1 );
					//	var laneOffset = road.RoadData.GetLaneOffset( lane );

					//	roadData.GetDirectionByTime( time, out var roadDirection );

					//	var directionDependingLaneVector = roadDirection;
					//	if( laneOffset > 0 )
					//		directionDependingLaneVector = -directionDependingLaneVector;

					//	var positionOnLane = position + QuaternionF.FromDirectionZAxisUp( roadDirection.ToVector3F() ) * new Vector3F( 0, (float)laneOffset, 0 );
					//	var positionOnLane2 = positionOnLane.ToVector2();

					//	bool radiusCheckGood;
					//	if( initialization )
					//		radiusCheckGood = circle.Contains( ref positionOnLane2 );
					//	else
					//		radiusCheckGood = circleFar.Contains( ref positionOnLane2 ) && !circle.Contains( ref positionOnLane2 );

					//	if( radiusCheckGood )
					//	{
					//		if( CharacterUtility.FindFreePlace( scene, characterType.Height, characterType.Radius, positionOnLane, 0, -0.25, 0.25, null, out var freePosition2 ) )
					//		{
					//			if( WalkingPedestriansMinimalDistanceBetweenPedestrians.Value == 0 || CharacterUtility.FindClosestCharacter( scene, new Sphere( freePosition2, WalkingPedestriansMinimalDistanceBetweenPedestrians ) ) == null )
					//			{
					//				freePosition = freePosition2;
					//				selectedDirection = directionDependingLaneVector.ToVector2();
					//				selectedRoad = road.Owner;
					//				selectedRoadLane = lane;
					//				break;
					//			}
					//		}
					//	}
					//}

					if( freePosition.HasValue )
					{
						var rot = Quaternion.FromDirectionZAxisUp( new Vector3( selectedDirection, 0 ) );
						var scl = Vector3.One;

						var tr = new Transform( freePosition.Value, rot, scl );

						var objectInstance = CreateObject( element, ObjectListEnum.WalkingPedestrians, tr, false );
						if( objectInstance != null )
						{
							//create character AI
							var character = objectInstance.SceneObject as Character;
							if( character != null )
							{
								var ai = character.CreateComponent<CharacterAI>( enabled: WalkingPedestriansManageTasks );
								ai.TrafficWalkingMode = true;
								ai.TrafficWalkingModeCurrentRoad = selectedRoad;
								ai.TrafficWalkingModeCurrentRoadLane = selectedRoadLane;
								//ai.TrafficWalkingModeDeleteObjectAtEndOfRoad = true;

								//!!!!temp
								//ai.DebugVisualization = true;
							}

							return objectInstance;
						}
					}
				}
			}

			return null;
		}

		void SimulateWalkingPedestrians( bool initialization )
		{
			var scene = FindParent<Scene>();
			if( scene == null )
				return;

			//delete before recreation
			if( initialization )
			{
				foreach( var obj in walkingPedestrians.ToArray() )
					obj.Dispose();
			}

			//simulate created objects
			foreach( var obj in walkingPedestrians.ToArray() ) //make copy to change walkingPedestrians
			{
				//check for already destroyed
				if( obj.SceneObject != null && obj.SceneObject.Parent == null )
				{
					obj.Dispose();
					continue;
				}

				//!!!!

				//if( obj.TargetLinearVelocity != Vector3.Zero )
				//{
				//	if( obj.SceneObject != null )
				//	{

				//		//!!!!помимо предсказания bounds можно тут реже делать шаг для объектов вдалеке


				//		var previousTransform = obj.SceneObject.TransformV;

				//		var pos = previousTransform.Position + obj.TargetLinearVelocity * Time.SimulationDelta;
				//		//var rot = Quaternion.FromDirectionZAxisUp( v.TargetLinearVelocity );


				//		var tr = new Transform( pos, previousTransform.Rotation, previousTransform.Scale );

				//		var vehicle = obj.SceneObject as Vehicle;
				//		if( vehicle != null )
				//		{
				//			vehicle.LinearVelocityToPredictBounds = obj.TargetLinearVelocity;
				//			vehicle.SetTransform( tr, false );
				//			//vehicle.SetTransformWithoutRecreating( tr );
				//		}
				//		else
				//			obj.SceneObject.Transform = tr;
				//	}
				//}
			}

			//create, destroy objects
			walkingPedestriansRemainingTimeToUpdate -= Time.SimulationDelta;
			if( walkingPedestriansRemainingTimeToUpdate <= 0 || initialization )
			{
				walkingPedestriansRemainingTimeToUpdate = 1 + staticRandom.Next( 0.1 );

				//var camerasPosition = GetCamerasPosition();
				if( lastCameraPosition.HasValue ) //if( camerasPosition.HasValue )
				{
					var cameraPosition = lastCameraPosition.Value;
					var cameraPosition2 = cameraPosition.ToVector2();

					//!!!!нужна поддержка нескольких камер если они на большом расстоянии
					var circle = new Circle( cameraPosition2, WalkingPedestriansCreateDistance.Value );
					//var sphere = new Sphere( camerasPosition.Value, WalkingPedestriansCreateDistance.Value );

					//var createInsideSpheres = new List<Sphere>();
					//foreach( var cameraPosition in camerasPosition.Value )
					//{
					//	createInsideSpheres.Add( new Sphere( cameraPosition, WalkingPedestriansCreateDistance.Value ) );
					//}

					List<Road.LogicalData> roadsInRadius = null;

					//create new
					var createObjectLimit = initialization ? WalkingPedestrians : WalkingPedestriansCreateMaxPerStep;
					for( int n = 0; n < createObjectLimit; n++ )
					{
						if( walkingPedestrians.Count < WalkingPedestrians )
						{
							if( roadsInRadius == null )
							{
								roadsInRadius = GetWalkableRoadsByCircle( circle );
								if( roadsInRadius.Count == 0 )
									break;
							}

							var circleFar = new Circle( circle.Center, circle.Radius * 1.15 );

							var v = CreateWalkingPedestrian( scene, initialization, cameraPosition, circle, circleFar, roadsInRadius );
							if( v == null )
								break;
						}
					}

					//destroy when far away
					{
						var toDestroy = new List<ObjectInstance>( 32 );

						var maxDistance = WalkingPedestriansCreateDistance.Value * 1.3;
						var maxDistanceSquared = maxDistance * maxDistance;

						foreach( var v in walkingPedestrians )
						{
							if( v.SceneObject != null )
							{
								var pos = v.SceneObject.TransformV.Position.ToVector2();

								var distanceSquared = ( cameraPosition2 - pos ).LengthSquared();
								//var distanceSquared = camerasBounds.GetPointDistanceSquared( ref pos );
								if( distanceSquared > maxDistanceSquared )
									toDestroy.Add( v );
							}
						}

						foreach( var v in toDestroy )
							v.Dispose();
					}
				}
			}
		}

		void WalkingPedestriansUpdateManageTasks()
		{
			var manage = WalkingPedestriansManageTasks.Value;

			foreach( var objectInstance in walkingPedestrians )
			{
				var obj = objectInstance.SceneObject;
				if( obj != null && obj.Parent != null )
				{
					var ai = obj.GetComponent<CharacterAI>();
					if( ai != null )
						ai.Enabled = manage;
				}
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( SimulateDynamicObjects )
			{
				SimulateFlyingObjects( false );
				SimulateWalkingPedestrians( false );
			}
		}

		private void Scene_GetRenderSceneData( Scene scene, ViewportRenderingContext context )
		{
			var viewport = context.Owner;
			var cameraSettings = viewport.CameraSettings;

			lastCameraPosition = cameraSettings.Position;
			//camerasPositionBoundsLastSecond.Add( cameraSettings.Position );

			////var bounds = new Rectangle( cameraSettings.Position.ToVector2() );
			////bounds.Expand( FlyingVehiclesCreateDistance );
			////camerasBoundsLastSecond.Add( bounds );
			//////lastCameraPosition = cameraSettings.Position.ToVector2();

			//!!!!good?
			if( needUpdateObjects )
				UpdateObjects();


			//!!!!temp
			//var test = ParentRoot.GetComponent( "TEST" ) as ObjectInSpace;
			//if( test != null )
			//{
			//	var position = test.TransformV.Position;


			//	foreach( var road in GetAllWalkableRoads() )
			//	{
			//		var roadData = road.RoadData;

			//		//double totalLengthNoCurvature = 0;
			//		//{
			//		//	for( int n = 1; n < roadData.Points.Length; n++ )
			//		//		totalLengthNoCurvature += ( roadData.Points[ n ].Transform.Position - roadData.Points[ n - 1 ].Transform.Position ).Length();
			//		//	if( totalLengthNoCurvature <= 0.001 )
			//		//		totalLengthNoCurvature = 0.001;
			//		//}

			//		//var totalTime = roadData.LastPoint.TimeOnCurve;

			//		//double timeStep;
			//		//{
			//		//	//!!!!
			//		//	var stepLength = 1.0;

			//		//	//!!!!? totalTime
			//		//	timeStep = stepLength / totalLengthNoCurvature * totalTime;
			//		//}

			//		if( roadData.GetClosestCurveTimeToPosition( position, 50, 1.1, out var timeOnCurve ) )
			//		{
			//			//roadData.GetClosestCurveTimeToPosition( position, timeStep, out var timeOnCurve );

			//			var p = roadData.GetPositionByTime( timeOnCurve );

			//			var renderer = context.Owner.Simple3DRenderer;
			//			renderer.SetColor( new ColorValue( 1, 0, 0 ), new ColorValue( 1, 0, 0, 0.5 ) );
			//			renderer.AddArrow( position, p );
			//		}
			//	}

			//}
		}

		public ESet<ObjectInstance> GetParkedVehicles()
		{
			return parkedVehicles;
		}

		public ESet<ObjectInstance> GetMovingVehicles()
		{
			return movingVehicles;
		}

		public ESet<ObjectInstance> GetFlyingObjects()
		{
			return flyingObjects;
		}

		public ESet<ObjectInstance> GetWalkingPedestrians()
		{
			return walkingPedestrians;
		}
	}
}


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//namespace NeoAxis
//{
//	public class RoadSystem : Component
//	{
//		LogicalData logicalData;

//		///////////////////////////////////////////////

//		class LogicalData
//		{
//			//!!!!
//			public Scene Scene;
//			public Road.LogicalData[] AllRoads;
//		}

//		///////////////////////////////////////////////

//		public class PathfindingInfo
//		{
//			public Vector3 From;
//			public Vector3 To;
//		}

//		///////////////////////////////////////////////

//		public class PathfindingResult
//		{
//			public Vector3[] Points;
//		}

//		///////////////////////////////////////////////

//		public class PathfindingTask
//		{
//			public PathfindingInfo Info;

//			public volatile PathfindingResult Result;
//		}

//		///////////////////////////////////////////////

//		public class RoadSystemPoint
//		{
//			public Road.LogicalData Road;
//			//!!!!
//			//public int Lane;
//			public double TimeRoadOnCurve;
//			public Vector3 Position;
//		}

//		///////////////////////////////////////////////

//		public static RoadSystem GetOrCreateRoadSystemForScene( Scene scene )
//		{
//			//!!!!slowly. кешировать выше

//			var system = scene.GetComponent<RoadSystem>( onlyEnabledInHierarchy: true );
//			if( system == null )
//				system = scene.CreateComponent<RoadSystem>( setUniqueName: true );

//			return system;
//		}

//		///////////////////////////////////////////////

//		LogicalData GetLogicalData()
//		{
//			if( logicalData == null )
//			{
//				var scene = Parent as Scene;
//				if( scene != null )
//				{
//					logicalData = new LogicalData();
//					logicalData.Scene = scene;

//					var allRoads = new List<Road.LogicalData>();

//					foreach( var road in scene.GetComponents<Road>( onlyEnabledInHierarchy: true ) )
//					{
//						var roadData = road.GetLogicalData();
//						if( roadData != null )
//							allRoads.Add( roadData );
//					}

//					logicalData.AllRoads = allRoads.ToArray();
//				}
//			}
//			return logicalData;
//		}

//		//!!!!
//		static void GetNearestPointToRoad( Road.LogicalData data, Vector3 point, out double nearestTime, out double nearestDistance, out Vector3 nearestPosition )
//		{
//			//!!!!

//			nearestTime = 0.0;
//			nearestDistance = double.MaxValue;
//			nearestPosition = Vector3.Zero;

//			var step = 0.01;

//			var maxTime = data.LastPoint.TimeOnCurve;
//			for( double time = 0.0; time <= maxTime; time += step )
//			{
//				var pos = data.GetPositionByTime( time );

//				var distance = ( point - pos ).Length();
//				if( distance < nearestDistance )
//				{
//					nearestTime = time;
//					nearestDistance = distance;
//					nearestPosition = pos;
//				}
//			}
//		}

//		//!!!!public. просто так можно ли вызывать. медленно
//		public RoadSystemPoint FindNearestRoad( Vector3 point )
//		{
//			//!!!!

//			var logicalData = GetLogicalData();

//			Road.LogicalData nearestRoad2 = null;
//			var nearestTime2 = 0.0;
//			var nearestDistance2 = double.MaxValue;
//			var nearestPosition2 = Vector3.Zero;

//			foreach( var road in logicalData.AllRoads )
//			{
//				GetNearestPointToRoad( road, point, out var nearestTime, out var nearestDistance, out var nearestPosition );

//				if( nearestDistance < nearestDistance2 )
//				{
//					nearestRoad2 = road;
//					nearestTime2 = nearestTime;
//					nearestDistance2 = nearestDistance;
//					nearestPosition2 = nearestPosition;
//				}
//			}

//			if( nearestRoad2 != null )
//			{
//				var result = new RoadSystemPoint();
//				result.Road = nearestRoad2;
//				result.TimeRoadOnCurve = nearestTime2;
//				result.Position = nearestPosition2;
//				return result;
//			}
//			else
//				return null;
//		}

//		//!!!!
//		public Vector3[] FindPath( RoadSystemPoint from, RoadSystemPoint to )
//		{
//			//!!!!

//			//!!!!
//			var step = 0.01;

//			if( from.Road == to.Road )
//			{
//				//!!!!GC
//				var result = new List<Vector3>();

//				if( from.TimeRoadOnCurve < to.TimeRoadOnCurve )
//				{
//					for( var time = from.TimeRoadOnCurve; time <= to.TimeRoadOnCurve; time += step )
//						result.Add( from.Road.GetPositionByTime( time ) );
//				}
//				else
//				{
//					for( var time = from.TimeRoadOnCurve; time <= to.TimeRoadOnCurve; time -= step )
//						result.Add( from.Road.GetPositionByTime( time ) );
//				}

//				return result.ToArray();
//			}
//			else
//			{
//				return null;
//			}
//		}

//		//!!!!
//		public PathfindingResult StartPathfinding( PathfindingInfo info )
//		//public PathfindingTask StartPathfinding( PathfindingInfo info )
//		{
//			var task = new PathfindingTask();
//			task.Info = info;

//			//!!!!

//			//var roadFrom = Road.LogicalData

//			var nearestRoadFrom = FindNearestRoad( info.From );
//			var nearestRoadTo = FindNearestRoad( info.To );

//			if( nearestRoadFrom != null && nearestRoadTo != null )
//			{
//				var pathOnRoads = FindPath( nearestRoadFrom, nearestRoadTo );

//				if( pathOnRoads != null )
//				{
//					var path = new List<Vector3>();
//					path.AddRange( pathOnRoads );
//					//path.Add( info.From );
//					path.Add( info.To );

//					var result = new PathfindingResult();
//					result.Points = path.ToArray();
//					return result;
//				}
//			}

//			//!!!!
//			return null;
//			//return task;
//		}
//	}
//}
