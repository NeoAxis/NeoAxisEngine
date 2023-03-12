// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

//!!!!
//humans
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
		bool needUpdateObjects;

		Dictionary<TrafficSystemElement, ElementCachedData> cachedElementsDictionary;
		ElementCachedData[] cachedGroundElements;
		ElementCachedData[] cachedFlyingElements;

		GroupOfObjectsUtility.GroupOfObjectsInstance groupOfObjects;
		List<GroupOfObjects.SubGroup> groupOfObjectsSubGroups = new List<GroupOfObjects.SubGroup>();

		ESet<ObjectInstance> parkedVehicles = new ESet<ObjectInstance>();
		ESet<ObjectInstance> movingVehicles = new ESet<ObjectInstance>();
		ESet<ObjectInstance> flyingObjects = new ESet<ObjectInstance>();
		double remainingTimeToUpdateFlyingObjects;
		double editorFlyingObjectsUpdateRemainingTime;

		Rectangle camerasBoundsLastSecond = Rectangle.Cleared;
		double camerasBoundsLastSecondRemainingTime;
		Queue<Rectangle> camerasBoundsLast10Seconds = new Queue<Rectangle>();
		//Vector2? lastCameraPosition;

		FastRandom componentRandom = new FastRandom();

		///////////////////////////////////////////////

		/// <summary>
		/// The amount of parked vehicles.
		/// </summary>
		[Category( "Ground Objects" )]
		[DefaultValue( 0 )]
		public Reference<int> ParkedVehicles
		{
			get { if( _parkedVehicles.BeginGet() ) ParkedVehicles = _parkedVehicles.Get( this ); return _parkedVehicles.value; }
			set { if( _parkedVehicles.BeginSet( ref value ) ) { try { ParkedVehiclesChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _parkedVehicles.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ParkedVehicles"/> property value changes.</summary>
		public event Action<TrafficSystem> ParkedVehiclesChanged;
		ReferenceField<int> _parkedVehicles = 0;

		//!!!!impl. сравнить

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
			set { if( _parkedVehiclesObjectMode.BeginSet( ref value ) ) { try { ParkedVehiclesObjectModeChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _parkedVehiclesObjectMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ParkedVehiclesObjectMode"/> property value changes.</summary>
		public event Action<TrafficSystem> ParkedVehiclesObjectModeChanged;
		ReferenceField<ObjectModeEnum> _parkedVehiclesObjectMode = ObjectModeEnum.VehicleComponent;

		/// <summary>
		/// The physics mode of parked vehicles.
		/// </summary>
		[Category( "Ground Objects" )]
		[DefaultValue( PhysicsModeEnum.Kinematic )]
		public Reference<PhysicsModeEnum> ParkedVehiclesPhysicsMode
		{
			get { if( _parkedVehiclesPhysicsMode.BeginGet() ) ParkedVehiclesPhysicsMode = _parkedVehiclesPhysicsMode.Get( this ); return _parkedVehiclesPhysicsMode.value; }
			set { if( _parkedVehiclesPhysicsMode.BeginSet( ref value ) ) { try { ParkedVehiclesPhysicsModeChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _parkedVehiclesPhysicsMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ParkedVehiclesPhysicsMode"/> property value changes.</summary>
		public event Action<TrafficSystem> ParkedVehiclesPhysicsModeChanged;
		ReferenceField<PhysicsModeEnum> _parkedVehiclesPhysicsMode = PhysicsModeEnum.Kinematic;


		//!!!!temp
		[Category( "Ground Objects" )]
		[DefaultValue( false )]
		public Reference<bool> ParkedVehiclesCanParkOnRoad
		{
			get { if( _parkedVehiclesCanParkOnRoad.BeginGet() ) ParkedVehiclesCanParkOnRoad = _parkedVehiclesCanParkOnRoad.Get( this ); return _parkedVehiclesCanParkOnRoad.value; }
			set { if( _parkedVehiclesCanParkOnRoad.BeginSet( ref value ) ) { try { ParkedVehiclesCanParkOnRoadChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _parkedVehiclesCanParkOnRoad.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ParkedVehiclesCanParkOnRoad"/> property value changes.</summary>
		public event Action<TrafficSystem> ParkedVehiclesCanParkOnRoadChanged;
		ReferenceField<bool> _parkedVehiclesCanParkOnRoad = false;


		//!!!!impl

		//[Category( "Ground Objects" )]
		//[DefaultValue( 0 )]
		//public Reference<int> MovingVehicles
		//{
		//	get { if( _movingVehicles.BeginGet() ) MovingVehicles = _movingVehicles.Get( this ); return _movingVehicles.value; }
		//	set { if( _movingVehicles.BeginSet( ref value ) ) { try { MovingVehiclesChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _movingVehicles.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MovingVehicles"/> property value changes.</summary>
		//public event Action<TrafficSystem> MovingVehiclesChanged;
		//ReferenceField<int> _movingVehicles = 0;

		//[Category( "Ground Objects" )]
		//[DefaultValue( PhysicsModeEnum.Kinematic )]
		//public Reference<PhysicsModeEnum> MovingVehiclesPhysicsMode
		//{
		//	get { if( _movingVehiclesPhysicsMode.BeginGet() ) MovingVehiclesPhysicsMode = _movingVehiclesPhysicsMode.Get( this ); return _movingVehiclesPhysicsMode.value; }
		//	set { if( _movingVehiclesPhysicsMode.BeginSet( ref value ) ) { try { MovingVehiclesPhysicsModeChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _movingVehiclesPhysicsMode.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MovingVehiclesPhysicsMode"/> property value changes.</summary>
		//public event Action<TrafficSystem> MovingVehiclesPhysicsModeChanged;
		//ReferenceField<PhysicsModeEnum> _movingVehiclesPhysicsMode = PhysicsModeEnum.Kinematic;



		//!!!!flying parked/stopped, flying moving

		/// <summary>
		/// The amount of flying vehicles.
		/// </summary>
		[Category( "Flying Objects" )]
		[DefaultValue( 0 )]
		public Reference<int> FlyingVehicles
		{
			get { if( _flyingVehicles.BeginGet() ) FlyingVehicles = _flyingVehicles.Get( this ); return _flyingVehicles.value; }
			set { if( _flyingVehicles.BeginSet( ref value ) ) { try { FlyingVehiclesChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _flyingVehicles.EndSet(); } } }
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
			set { if( _flyingVehiclesPhysicsMode.BeginSet( ref value ) ) { try { FlyingVehiclesPhysicsModeChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _flyingVehiclesPhysicsMode.EndSet(); } } }
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
			set { if( _flyingVehiclesCreateMaxPerStep.BeginSet( ref value ) ) { try { FlyingVehiclesCreateMaxPerStepChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _flyingVehiclesCreateMaxPerStep.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyingVehiclesCreateMaxPerStep"/> property value changes.</summary>
		public event Action<TrafficSystem> FlyingVehiclesCreateMaxPerStepChanged;
		ReferenceField<int> _flyingVehiclesCreateMaxPerStep = 10;

		//!!!!FlyingCreatures

		/// <summary>
		/// The distance from the camera for new flying objects.
		/// </summary>
		[Category( "Flying Objects" )]
		[DefaultValue( 1100.0 )]
		public Reference<double> FlyingObjectsCreateDistance
		{
			get { if( _flyingObjectsCreateDistance.BeginGet() ) FlyingObjectsCreateDistance = _flyingObjectsCreateDistance.Get( this ); return _flyingObjectsCreateDistance.value; }
			set { if( _flyingObjectsCreateDistance.BeginSet( ref value ) ) { try { FlyingObjectsCreateDistanceChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _flyingObjectsCreateDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyingObjectsCreateDistance"/> property value changes.</summary>
		public event Action<TrafficSystem> FlyingObjectsCreateDistanceChanged;
		ReferenceField<double> _flyingObjectsCreateDistance = 1100.0;

		//!!!!
		//[DefaultValue( 0 )]
		//public Reference<int> IdleHumans
		//{
		//	get { if( _idleHumans.BeginGet() ) IdleHumans = _idleHumans.Get( this ); return _idleHumans.value; }
		//	set { if( _idleHumans.BeginSet( ref value ) ) { try { IdleHumansChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _idleHumans.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="IdleHumans"/> property value changes.</summary>
		//public event Action<TrafficSystem> IdleHumansChanged;
		//ReferenceField<int> _idleHumans = 0;

		//[DefaultValue( 0 )]
		//public Reference<int> WalkingHumans
		//{
		//	get { if( _walkingHumans.BeginGet() ) WalkingHumans = _walkingHumans.Get( this ); return _walkingHumans.value; }
		//	set { if( _walkingHumans.BeginSet( ref value ) ) { try { WalkingHumansChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _walkingHumans.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="WalkingHumans"/> property value changes.</summary>
		//public event Action<TrafficSystem> WalkingHumansChanged;
		//ReferenceField<int> _walkingHumans = 0;

		//!!!!WalkingCreatures, FlyingCreatures


		//!!!!default
		/// <summary>
		/// The size of the sector in the scene. The sector size allows to optimize the culling and rendering of objects.
		/// </summary>
		[DefaultValue( "200 200 10000" )]
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
				if( _staticObjectsSectorSize.BeginSet( ref value ) ) { try { StaticObjectsSectorSizeChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _staticObjectsSectorSize.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="StaticObjectsSectorSize"/> property value changes.</summary>
		public event Action<TrafficSystem> StaticObjectsSectorSizeChanged;
		ReferenceField<Vector3> _staticObjectsSectorSize = new Vector3( 200, 200, 10000 );

		//!!!!default
		/// <summary>
		/// The maximal amount of objects in one group/batch.
		/// </summary>
		[DefaultValue( 1000 )]
		[Category( "Optimization" )]
		public Reference<int> StaticObjectsMaxObjectsInGroup
		{
			get { if( _staticObjectsMaxObjectsInGroup.BeginGet() ) StaticObjectsMaxObjectsInGroup = _staticObjectsMaxObjectsInGroup.Get( this ); return _staticObjectsMaxObjectsInGroup.value; }
			set { if( _staticObjectsMaxObjectsInGroup.BeginSet( ref value ) ) { try { StaticObjectsMaxObjectsInGroupChanged?.Invoke( this ); NeedUpdateObjects(); } finally { _staticObjectsMaxObjectsInGroup.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="StaticObjectsMaxObjectsInGroup"/> property value changes.</summary>
		public event Action<TrafficSystem> StaticObjectsMaxObjectsInGroupChanged;
		ReferenceField<int> _staticObjectsMaxObjectsInGroup = 1000;

		/// <summary>
		/// Whether to simulate dynamic objects.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Simulation" )]
		public Reference<bool> SimulateDynamicObjects
		{
			get { if( _simulateDynamicObjects.BeginGet() ) SimulateDynamicObjects = _simulateDynamicObjects.Get( this ); return _simulateDynamicObjects.value; }
			set { if( _simulateDynamicObjects.BeginSet( ref value ) ) { try { SimulateDynamicObjectsChanged?.Invoke( this ); } finally { _simulateDynamicObjects.EndSet(); } } }
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
				}
			}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			if( Components.Count == 0 )
			{
				var element = CreateComponent<TrafficSystemElement>();
				element.Name = element.BaseType.GetUserFriendlyNameForInstance();
				element.Roles = TrafficSystemElement.RolesEnum.Ground | TrafficSystemElement.RolesEnum.Flying;
				element.ObjectType = new Reference<Component>( null, @"Content\Vehicles\Default\Default Vehicle.vehicletype" );

				FlyingVehicles = 200;
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

				//!!!!here? maybe it simulation step when it is simulation
				camerasBoundsLastSecondRemainingTime -= delta;
				if( camerasBoundsLastSecondRemainingTime < 0 )
				{
					if( !camerasBoundsLastSecond.IsCleared() )
					{
						camerasBoundsLast10Seconds.Enqueue( camerasBoundsLastSecond );
						if( camerasBoundsLast10Seconds.Count > 10 )
							camerasBoundsLast10Seconds.Dequeue();
					}
					camerasBoundsLastSecond = Rectangle.Cleared;
					camerasBoundsLastSecondRemainingTime = 1;
				}

				if( EngineApp.IsEditor )
				{
					editorFlyingObjectsUpdateRemainingTime -= delta;
					if( editorFlyingObjectsUpdateRemainingTime < -0.5 )
						editorFlyingObjectsUpdateRemainingTime = -0.5;
					while( editorFlyingObjectsUpdateRemainingTime <= 0 )
					{
						editorFlyingObjectsUpdateRemainingTime += Time.SimulationDelta;
						SimulateFlyingObjects( false );
					}
				}
			}
		}

		void NeedUpdateObjects()
		{
			needUpdateObjects = true;
		}

		static Road.LogicalData[] GetRoads( Scene scene )
		{
			var roads = new List<Road.LogicalData>( 32 );

			foreach( var road in scene.GetComponents<Road>( onlyEnabledInHierarchy: true ) )
			{
				var roadData = road.GetLogicalData();
				if( roadData != null )
					roads.Add( roadData );
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
			cachedGroundElements = all.Where( t => ( t.Element.Roles.Value & TrafficSystemElement.RolesEnum.Ground ) != 0 ).ToArray();
			cachedFlyingElements = all.Where( t => ( t.Element.Roles.Value & TrafficSystemElement.RolesEnum.Flying ) != 0 ).ToArray();
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
						var pos = road.RoadData.GetPositionByTime( time );
						var dir = road.RoadData.GetDirectionByTime( time );

						//check it on start or on end of the road
						var distanceStart = ( road.RoadData.GetPositionByTime( selectedTimeClipRange.Minimum ) - pos ).Length();
						var distanceEnd = ( road.RoadData.GetPositionByTime( selectedTimeClipRange.Maximum ) - pos ).Length();

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
									var lane = random.Next( road.RoadData.Lanes - 2 - 1 ) + 1;
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

			return vehicleItems;
		}

		void UpdateParkedVehicles()
		{
			var scene = FindParent<Scene>();
			if( scene == null )
				return;

			var roads = GetRoads( scene );
			if( roads.Length == 0 )
				return;
			foreach( var road in roads )
				road.DropTimeClipRanges();

			if( cachedGroundElements == null )
				return;

			//!!!!what else?
			var elementsToCreate = cachedGroundElements.Where( e => e.Element.ObjectType.Value as VehicleType != null ).ToArray();
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

						//!!!!
						var isStatic = true;// false;

						////!!!!
						//break;

						var obj = CreateObject( element, ObjectListEnum.ParkedVehicles, tr, isStatic );// true );

						//!!!!
						//deactivate physics body
						var vehicle = obj.SceneObject as Vehicle;
						if( vehicle != null )
							vehicle.PhysicalBody.Active = false;
					}
				}
				else
				{
					groupOfObjects = GroupOfObjectsUtility.GetOrCreateGroupOfObjects( scene, "__GroupOfObjectsTrafficSystem", true, StaticObjectsSectorSize, StaticObjectsMaxObjectsInGroup );

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

									var elementIndex = groupOfObjects.GetOrCreateGroupOfObjectsElement( item.Mesh, null, true, 1, true, 1, collision );

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
			if( !GetCamerasPosition().HasValue )
				return;

			CalculateCachedElements();
			UpdateParkedVehicles();
			//UpdateCharacters();

			SimulateFlyingObjects( true );

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
		}

		public ObjectInstance CreateObject( TrafficSystemElement element, ObjectListEnum objectList, Transform transform, bool isStatic )
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
			}


			var objectType = element.ObjectType.Value;

			Metadata.TypeInfo typeToCreate = null;

			var vehicleType = objectType as VehicleType;
			if( vehicleType != null )
				typeToCreate = MetadataManager.GetTypeOfNetType( typeof( Vehicle ) );

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

				obj.Static = isStatic;

				obj.Enabled = true;

				objectInstance.SceneObject = obj;
			}


			return objectInstance;
		}

		protected virtual ObjectInstance CreateFlyingObject( bool initialization )
		{
			var camerasPosition = GetCamerasPosition();

			if( cachedFlyingElements != null && cachedFlyingElements.Length != 0 && camerasPosition.HasValue )
			{
				var typeIndex = componentRandom.Next( cachedFlyingElements.Length - 1 );
				var type = cachedFlyingElements[ typeIndex ];
				var element = type.Element;

				var heightRange = element.FlyingHeightRange.Value;
				var createDistance = FlyingObjectsCreateDistance.Value;

				//calculate position
				Vector3? pos = null;
				for( int nIteration = 0; nIteration < 20; nIteration++ )
				{
					//!!!!check for free place
					var height = componentRandom.Next( heightRange.Minimum, heightRange.Maximum );

					if( !initialization )
					{
						//create on far border

						var angle = componentRandom.Next( 0, Math.PI * 2 );

						var offset = new Vector2( Math.Cos( angle ), Math.Sin( angle ) ) * createDistance;
						pos = new Vector3( camerasPosition.Value + offset, height );
					}
					else
					{
						//create anywhere in radius

						var offset = new Vector2( componentRandom.Next( -createDistance, createDistance ), componentRandom.Next( -createDistance, createDistance ) );

						//check by distance
						if( offset.LengthSquared() < createDistance * createDistance )
							pos = new Vector3( camerasPosition.Value + offset, height );
					}
				}

				if( pos.HasValue )
				{
					var speedRange = element.FlyingSpeedRange.Value;
					var speed = componentRandom.Next( speedRange.Minimum, speedRange.Maximum );

					var destX = componentRandom.Next( camerasPosition.Value.X - createDistance, camerasPosition.Value.X + createDistance );
					var destY = componentRandom.Next( camerasPosition.Value.Y - createDistance, camerasPosition.Value.Y + createDistance );
					var destPos = new Vector2( destX, destY );
					if( destPos == pos.Value.ToVector2() )
						destPos.X += 1;
					var dir = ( destPos - pos.Value.ToVector2() ).GetNormalize();

					var rot = Quaternion.FromDirectionZAxisUp( new Vector3( dir, 0 ) );
					var scl = Vector3.One;

					var tr = new Transform( pos.Value, rot, scl );

					var objectInstance = CreateObject( element, ObjectListEnum.FlyingObjects, tr, false );
					objectInstance.TargetLinearVelocity = rot.GetForward() * speed;

					return objectInstance;
				}
			}

			return null;
		}

		public Vector2? GetCamerasPosition()
		{
			var result = camerasBoundsLastSecond;//Bounds.Cleared;
			foreach( var b in camerasBoundsLast10Seconds )
				result.Add( b );

			if( result.IsCleared() )
				return null;

			return result.GetCenter();
		}

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

		void SimulateFlyingObjects( bool initialization )
		{
			//simulate flying objects
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

			//create, destroy flying vehicles
			remainingTimeToUpdateFlyingObjects -= Time.SimulationDelta;
			if( remainingTimeToUpdateFlyingObjects <= 0 || initialization )
			{
				remainingTimeToUpdateFlyingObjects = 1 + componentRandom.Next( 0.1 );

				//var camerasBounds = GetCamerasBounds();
				//if( !camerasBounds.IsCleared() )

				var camerasPosition = GetCamerasPosition();
				if( camerasPosition.HasValue )
				{
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

								var distanceSquared = ( camerasPosition.Value - pos ).LengthSquared();
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

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( SimulateDynamicObjects )
				SimulateFlyingObjects( false );
		}

		private void Scene_GetRenderSceneData( Scene scene, ViewportRenderingContext context )
		{
			var viewport = context.Owner;
			var cameraSettings = viewport.CameraSettings;

			camerasBoundsLastSecond.Add( cameraSettings.Position.ToVector2() );
			//var bounds = new Rectangle( cameraSettings.Position.ToVector2() );
			//bounds.Expand( FlyingVehiclesCreateDistance );
			//camerasBoundsLastSecond.Add( bounds );
			////lastCameraPosition = cameraSettings.Position.ToVector2();

			//!!!!good?
			if( needUpdateObjects )
				UpdateObjects();
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
