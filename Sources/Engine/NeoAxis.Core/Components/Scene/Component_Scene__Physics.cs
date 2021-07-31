// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Drawing.Design;
using System.CodeDom;
using System.CodeDom.Compiler;
using BulletSharp;
using BulletSharp.Math;
using BulletSharp.SoftBody;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	public partial class Component_Scene
	{
		/// <summary>
		/// Provides a data of Bullet physics engine.
		/// </summary>
		public class PhysicsWorldDataClass
		{
			//!!!!что-то в общее перенести?
			//!!!!DefaultCollisionConfiguration
			//!!!!что еще по другому
			//!!!!discrete
			public CollisionConfiguration collisionConfiguration;
			public CollisionDispatcher dispatcher;
			public BroadphaseInterface broadphase;
			public SoftRigidDynamicsWorld world;
			public SoftBodyWorldInfo softBodyWorldInfo;
			internal PhysicsDebugDraw debugDraw;
		}
		PhysicsWorldDataClass physicsWorldData;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		struct CollectContactsItem
		{
			public int simulationSubStep;
			public IntPtr bodyA;
			public IntPtr bodyB;
			public int numContacts;

			public Vector3 positionWorldOnA_0;
			public Vector3 positionWorldOnB_0;
			public float appliedImpulse_0;
			public float distance_0;

			public Vector3 positionWorldOnA_1;
			public Vector3 positionWorldOnB_1;
			public float appliedImpulse_1;
			public float distance_1;

			public Vector3 positionWorldOnA_2;
			public Vector3 positionWorldOnB_2;
			public float appliedImpulse_2;
			public float distance_2;

			public Vector3 positionWorldOnA_3;
			public Vector3 positionWorldOnB_3;
			public float appliedImpulse_3;
			public float distance_3;

			//

			public void GetContact( int index, out Vector3 positionWorldOnA, out Vector3 positionWorldOnB, out float appliedImpulse, out float distance )
			{
				switch( index )
				{
				case 0:
					positionWorldOnA = positionWorldOnA_0;
					positionWorldOnB = positionWorldOnB_0;
					appliedImpulse = appliedImpulse_0;
					distance = distance_0;
					break;

				case 1:
					positionWorldOnA = positionWorldOnA_1;
					positionWorldOnB = positionWorldOnB_1;
					appliedImpulse = appliedImpulse_1;
					distance = distance_1;
					break;

				case 2:
					positionWorldOnA = positionWorldOnA_2;
					positionWorldOnB = positionWorldOnB_2;
					appliedImpulse = appliedImpulse_2;
					distance = distance_2;
					break;

				default://case 3:
					positionWorldOnA = positionWorldOnA_3;
					positionWorldOnB = positionWorldOnB_3;
					appliedImpulse = appliedImpulse_3;
					distance = distance_3;
					break;
				}
			}
		};

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[Browsable( false )]
		public PhysicsWorldDataClass PhysicsWorldData
		{
			get { return physicsWorldData; }
		}

		void PhysicsWorldCreate()
		{
			physicsWorldData = new PhysicsWorldDataClass();
			var data = physicsWorldData;

			//!!!!переопределять создание мира

			data.collisionConfiguration = new SoftBodyRigidBodyCollisionConfiguration();

			//!!!!
			//data.dispatcher = new CollisionDispatcherMultiThreaded( data.collisionConfiguration );
			data.dispatcher = new CollisionDispatcher( data.collisionConfiguration );

			//!!!!?
			data.broadphase = new DbvtBroadphase();
			//!!!!
			//physicsBroadphase = new AxisSweep3( new Vector3( -1000, -1000, -1000 ), new Vector3( 1000, 1000, 1000 ), 1024 );// maxProxies );

			SoftBodySolver softBodySolver = null;
			data.world = new SoftRigidDynamicsWorld( data.dispatcher, data.broadphase, null, data.collisionConfiguration, softBodySolver );
			data.world.DispatchInfo.EnableSpu = true;
			//!!!!

			//data.world.DispatchInfo.DispatchFunc = DispatchFunc.Continuous;

			data.softBodyWorldInfo = new SoftBodyWorldInfo();
			//!!!!
			data.softBodyWorldInfo.AirDensity = 1.2f;
			data.softBodyWorldInfo.WaterDensity = 0;
			data.softBodyWorldInfo.WaterOffset = 0;
			data.softBodyWorldInfo.WaterNormal = BulletSharp.Math.Vector3.Zero;
			data.softBodyWorldInfo.Dispatcher = data.dispatcher;
			data.softBodyWorldInfo.Broadphase = data.broadphase;
			data.softBodyWorldInfo.SparseSdf.Initialize();

			//physicsCollisionConfiguration = new DefaultCollisionConfiguration();
			//physicsDispatcher = new CollisionDispatcher( physicsCollisionConfiguration );
			//physicsBroadphase = new DbvtBroadphase();
			//physicsWorld = new DiscreteDynamicsWorld( physicsDispatcher, physicsBroadphase, null, physicsCollisionConfiguration );

			//!!!!
			//RandomizeOrder = 1,
			//FrictionSeparate = 2,
			//UseWarmStarting = 4,
			//Use2FrictionDirections = 16,
			//EnableFrictionDirectionCaching = 32,
			//DisableVelocityDependentFrictionDirection = 64,
			//CacheFriendly = 128,
			//Simd = 256,
			//InterleaveContactAndFrictionConstraints = 512,
			//AllowZeroLengthFrictionDirections = 1024

			data.world.SolverInfo.SolverMode |= SolverModes.Use2FrictionDirections | SolverModes.RandomizeOrder;

			data.world.SolverInfo.NumIterations = PhysicsNumberIterations;

			PhysicsUpdateGravity();

			//не надо вроде как
			//physicsWorld.DispatchInfo.UseContinuous = true;

			physicsWorldData.debugDraw = new PhysicsDebugDraw();
			physicsWorldData.world.DebugDrawer = physicsWorldData.debugDraw;

			//!!!!эвент PhysicsWorldCreated
		}

		void PhysicsWorldDestroy()
		{
			if( physicsWorldData != null )
			{

				//!!!!эвент PhysicsWorldBeforeDestroy

				//!!!!

				//CleanupConstraints( simulation.World );
				//CleanupBodiesAndShapes( simulation.World );

				//var multiBodyWorld = simulation.World as MultiBodyDynamicsWorld;
				//if( multiBodyWorld != null )
				//{
				//	CleanupMultiBodyWorld( multiBodyWorld );
				//}

				physicsWorldData.softBodyWorldInfo?.Dispose();
				physicsWorldData.world?.Dispose();
				physicsWorldData.broadphase?.Dispose();
				physicsWorldData.dispatcher?.Dispose();
				physicsWorldData.collisionConfiguration?.Dispose();

				physicsWorldData = null;
			}
		}

		public event Action<Component_Scene> PhysicsSimulationStepAfter;

		unsafe void PhysicsSimulate()
		{
			if( physicsWorldData == null )
				return;

			//update settings
			if( physicsWorldData.world.SolverInfo.NumIterations != PhysicsNumberIterations )
				physicsWorldData.world.SolverInfo.NumIterations = PhysicsNumberIterations;

			//simulate

			physicsWorldData.world.CallCustomMethod( 1, IntPtr.Zero, IntPtr.Zero );

			float invStep = (float)ProjectSettings.Get.SimulationStepsPerSecondInv;

			int steps = PhysicsSimulationSteps;
			if( steps == 0 )
				steps = 1;
			for( int n = 0; n < steps; n++ )
			{
				physicsWorldData.world.StepSimulation( invStep / steps, 1, invStep / steps );

				physicsWorldData.world.CallCustomMethod( 2, (IntPtr)n, IntPtr.Zero );
			}

			//int substepsPassed = physicsWorldData.world.StepSimulation( invStep, 10, invStep );
			//int substepsPassed = physicsWorld.StepSimulation( (float)ProjectSettings.Get.SimulationStepsPerSecondInv, 1, 0.0166666675F );

			int updateObjectsCount = 0;
			var updateObjectsPointer = (IntPtr*)physicsWorldData.world.CallCustomMethod( 3, (IntPtr)( &updateObjectsCount ), IntPtr.Zero );

			//Log.Info( updateObjectsCount );

			//get updated data
			{
				//update bodies
				for( int n = 0; n < updateObjectsCount; n++ )
				{
					var collisionObject = CollisionObject.GetManaged( updateObjectsPointer[ n ] );
					if( collisionObject != null )
					{
						var body = collisionObject.UserObject as Component_PhysicalBody;
						if( body != null )
							body.UpdateDataFromPhysicsEngine();
					}
				}

				//update constraints
				var constraintCount = physicsWorldData.world.NumConstraints;
				for( int n = 0; n < constraintCount; n++ )
				{
					var c = physicsWorldData.world.GetConstraint( n );
					var c2 = c.Userobject as Component_Constraint;
					if( c2 != null )
						c2.UpdateDataFromPhysicsEngine();
				}

				//foreach( var body in GetComponents<Component_PhysicalBody>( false, true, true ) )
				//	body.UpdateDataFromPhysicsEngine();

				//foreach( var constraint in GetComponents<Component_Constraint>( false, true, true ) )
				//	constraint.UpdateDataFromPhysicsEngine();
			}

			//clear old collision contacts
			{
				//!!!!slowly

				foreach( var collisionObject in physicsWorldData.world.CollisionObjectArray )
				{
					var body = collisionObject.UserObject as Component_RigidBody;
					if( body != null && body.ContactsData != null )
						body.ContactsData.Clear();
				}
			}

			//process collision contacts
			{
				var size = sizeof( CollectContactsItem );
				if( sizeof( CollectContactsItem ) != 256 )
					Log.Fatal( "sizeof( CollectContactsItem ) != 256. Size: " + size.ToString() );
				//if( sizeof( CollectContactsItem ) != 4 + 8 + 8 + 4 + 56 * 4 )
				//	Log.Fatal( "sizeof( CollectContactsItem ) != 4 + 8 + 8 + 4 + 56 * 4" + size.ToString() );

				int contactsCount = 0;
				var contactsPointer = (CollectContactsItem*)physicsWorldData.world.CallCustomMethod( 4, (IntPtr)( &contactsCount ), IntPtr.Zero );

				//update bodies
				for( int nContact = 0; nContact < contactsCount; nContact++ )
				{
					var contactsItem = contactsPointer + nContact;

					var collisionObjectA = CollisionObject.GetManaged( contactsItem->bodyA );
					var collisionObjectB = CollisionObject.GetManaged( contactsItem->bodyB );
					if( collisionObjectA != null && collisionObjectB != null )
					{
						var bodyA = collisionObjectA.UserObject as Component_RigidBody;
						var bodyB = collisionObjectB.UserObject as Component_RigidBody;
						//var bodyA = collisionObjectA.UserObject as Component_PhysicalBody;
						//var bodyB = collisionObjectB.UserObject as Component_PhysicalBody;
						if( bodyA != null && !bodyA.Disposed && bodyB != null && !bodyB.Disposed )
						{
							if( bodyA.ContactsCollect )
							{
								for( int n = 0; n < contactsItem->numContacts; n++ )
								{
									var item = new Component_RigidBody.ContactsDataItem();
									item.SimulationSubStep = contactsItem->simulationSubStep;
									contactsItem->GetContact( n, out item.PositionWorldOnA, out item.PositionWorldOnB, out item.AppliedImpulse, out item.Distance );
									item.BodyB = bodyB;

									if( bodyA.ContactsData == null )
										bodyA.ContactsData = new List<Component_RigidBody.ContactsDataItem>();
									if( bodyA.ContactsData.Count < 128 )
										bodyA.ContactsData.Add( item );
								}
							}

							if( bodyB.ContactsCollect )
							{
								for( int n = 0; n < contactsItem->numContacts; n++ )
								{
									var item = new Component_RigidBody.ContactsDataItem();
									item.SimulationSubStep = contactsItem->simulationSubStep;
									contactsItem->GetContact( n, out item.PositionWorldOnB, out item.PositionWorldOnA, out item.AppliedImpulse, out item.Distance );
									item.BodyB = bodyA;

									if( bodyB.ContactsData == null )
										bodyB.ContactsData = new List<Component_RigidBody.ContactsDataItem>();
									if( bodyB.ContactsData.Count < 128 )
										bodyB.ContactsData.Add( item );
								}
							}
						}
					}
				}

				//var numManifolds = PhysicsWorldData.world.Dispatcher.NumManifolds;
				//for( int i = 0; i < numManifolds; i++ )
				//{
				//	var contactManifold = PhysicsWorldData.world.Dispatcher.GetManifoldByIndexInternal( i );
				//	var obA = contactManifold.Body0;
				//	var obB = contactManifold.Body1;

				//	var rigidBodyA = obA.UserObject as Component_RigidBody;
				//	var rigidBodyB = obB.UserObject as Component_RigidBody;

				//	if( rigidBodyA != rigidBodyB )
				//	{
				//		if( rigidBodyA != null && rigidBodyA.ContactsCollect )
				//		{
				//			int numContacts = contactManifold.NumContacts;
				//			for( int j = 0; j < numContacts; j++ )
				//			{
				//				var pt = contactManifold.GetContactPoint( j );
				//				if( pt.Distance < 0.0f )
				//				{
				//					var item = new Component_RigidBody.ContactsDataItem();

				//					item.PositionWorldOnA = BulletPhysicsUtility.Convert( pt.PositionWorldOnA );
				//					item.PositionWorldOnB = BulletPhysicsUtility.Convert( pt.PositionWorldOnB );
				//					item.BodyB = rigidBodyB;

				//					if( rigidBodyA.ContactsData == null )
				//						rigidBodyA.ContactsData = new List<Component_RigidBody.ContactsDataItem>();
				//					if( rigidBodyA.ContactsData.Count < 128 )
				//						rigidBodyA.ContactsData.Add( item );
				//				}
				//			}
				//		}

				//		if( rigidBodyB != null && rigidBodyB.ContactsCollect )
				//		{
				//			int numContacts = contactManifold.NumContacts;
				//			for( int j = 0; j < numContacts; j++ )
				//			{
				//				var pt = contactManifold.GetContactPoint( j );
				//				if( pt.Distance < 0.0f )
				//				{
				//					var item = new Component_RigidBody.ContactsDataItem();

				//					item.PositionWorldOnA = BulletPhysicsUtility.Convert( pt.PositionWorldOnB );
				//					item.PositionWorldOnB = BulletPhysicsUtility.Convert( pt.PositionWorldOnA );
				//					item.BodyB = rigidBodyA;

				//					if( rigidBodyB.ContactsData == null )
				//						rigidBodyB.ContactsData = new List<Component_RigidBody.ContactsDataItem>();
				//					if( rigidBodyB.ContactsData.Count < 128 )
				//						rigidBodyB.ContactsData.Add( item );
				//				}
				//			}
				//		}
				//	}
				//}
			}

			if( physicsWorldData.softBodyWorldInfo != null )
				physicsWorldData.softBodyWorldInfo.SparseSdf.GarbageCollect();

			PhysicsSimulationStepAfter?.Invoke( this );
		}

		void PhysicsUpdateGravity()
		{
			if( physicsWorldData != null )
			{
				physicsWorldData.world.Gravity = BulletPhysicsUtility.Convert( Gravity );
				if( physicsWorldData.softBodyWorldInfo != null )
					physicsWorldData.softBodyWorldInfo.Gravity = physicsWorldData.world.Gravity;
			}
		}

		void PhysicsViewportUpdateBegin( Viewport viewport )
		{
			//!!!!
			//PhysicsTest( viewport );


			if( physicsWorldData != null && viewport.simple3DRenderer != null )
			{
				physicsWorldData.debugDraw.renderer = viewport.simple3DRenderer;

				//update mode
				DebugDrawModes mode = DebugDrawModes.DrawConstraints | DebugDrawModes.DrawConstraintLimits;
				//DebugDrawModes mode = DebugDrawModes.DrawWireframe | DebugDrawModes.DrawConstraints | DebugDrawModes.DrawConstraintLimits;
				//DebugDrawModes mode = DebugDrawModes.DrawWireframe | DebugDrawModes.DrawConstraintLimits;
				if( DisplayPhysicsInternal )
					mode |= DebugDrawModes.All;
				physicsWorldData.debugDraw.DebugMode = mode;

				if( GetDisplayDevelopmentDataInThisApplication() && DisplayPhysicsInternal )
				{
					//draw all internal data

					for( int n = 0; n < physicsWorldData.world.NumConstraints; n++ )
					{
						var c = physicsWorldData.world.GetConstraint( n );
						var c2 = c.Userobject as Component_Constraint;
						if( c2 != null )
							c2.UpdateDebugDrawSize( viewport );
					}

					physicsWorldData.debugDraw.verticesRenderedCounter = 0;
					physicsWorldData.debugDraw.verticesRenderedCounterLimit = 100000;
					physicsWorldData.world.DebugDrawWorld();
					physicsWorldData.debugDraw.verticesRenderedCounter = 0;
					physicsWorldData.debugDraw.verticesRenderedCounterLimit = -1;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		class BulletRayResultCallback : RayResultCallback, IComparer<PhysicsRayTestItem.ResultItem>
		{
			PhysicsRayTestItem item;
			List<PhysicsRayTestItem.ResultItem> resultList = new List<PhysicsRayTestItem.ResultItem>();

			//last collision data
			BulletSharp.Math.Vector3 hitNormalWorld;
			int localShapeInfoPart;
			int localShapeInfoTriangle;

			public BulletRayResultCallback( PhysicsRayTestItem item )
			{
				this.item = item;
			}

			public override double AddSingleResult( LocalRayResult rayResult, bool normalInWorldSpace )
			{
				CollisionObject = rayResult.CollisionObject;//HasHit==true

				//will terminate further ray tests, once the closestHitFraction reached zero
				if( item.Mode == PhysicsRayTestItem.ModeEnum.One )
					ClosestHitFraction = 0;

				//save data of last collision
				if( normalInWorldSpace )
					hitNormalWorld = rayResult.HitNormalLocal;
				else
					hitNormalWorld = BulletSharp.Math.Vector3.TransformCoordinate( rayResult.HitNormalLocal, rayResult.CollisionObject.WorldTransform.Basis );

				if( rayResult.LocalShapeInfo.Native != IntPtr.Zero )
				{
					localShapeInfoPart = rayResult.LocalShapeInfo?.ShapePart ?? 0;
					localShapeInfoTriangle = rayResult.LocalShapeInfo?.TriangleIndex ?? 0;
				}
				else
				{
					localShapeInfoPart = 0;
					localShapeInfoTriangle = 0;
				}

				//processing found collision
				if( item.Mode == PhysicsRayTestItem.ModeEnum.OneClosest )
				{
					ClosestHitFraction = rayResult.HitFraction;
					return rayResult.HitFraction;
				}

				var resultItem = CreateResultItem( rayResult.CollisionObject, rayResult.HitFraction );
				resultList.Add( resultItem );
				return ClosestHitFraction;
			}

			PhysicsRayTestItem.ResultItem CreateResultItem( CollisionObject obj, double hitFraction )
			{
				var resultItem = new PhysicsRayTestItem.ResultItem();
				resultItem.TriangleIndexSource = -1;
				resultItem.TriangleIndexProcessed = -1;

				CollisionShape shape;

				//фича буллета. индекс шейпа приходит как TriangleIndex
				var shapeIdx = localShapeInfoPart < 0 ? localShapeInfoTriangle : 0;

				if( obj.CollisionShape is CompoundShape compoundShape )
				{
					if( compoundShape.NumChildShapes == 1 )
						shape = compoundShape.GetChildShape( 0 );
					else
						shape = compoundShape.GetChildShape( shapeIdx );
				}
				else
					shape = obj.CollisionShape;

				resultItem.Shape = shape?.UserObject as Component_CollisionShape;
				resultItem.Body = obj.UserObject as Component_ObjectInSpace;
				resultItem.Normal = BulletPhysicsUtility.Convert( hitNormalWorld );
				resultItem.DistanceScale = hitFraction;
				resultItem.Position = item.Ray.Origin + item.Ray.Direction * hitFraction;

				if( resultItem.Shape is Component_CollisionShape_Mesh csMesh && shape is TriangleMeshShape )
				{
					resultItem.TriangleIndexProcessed = localShapeInfoTriangle;

					csMesh.GetProcessedData( out _, out _, out var triToSource );

					if( triToSource == null )
						resultItem.TriangleIndexSource = localShapeInfoTriangle;
					else if( triToSource.Length > localShapeInfoTriangle )
						resultItem.TriangleIndexSource = triToSource[ localShapeInfoTriangle ];
				}
				else if( resultItem.Body is IComponent_SoftBody softBody )
				{
					resultItem.TriangleIndexProcessed = localShapeInfoTriangle;

					softBody.GetProcessedData( out _, out _, out var triToSource );

					if( triToSource == null )
						resultItem.TriangleIndexSource = localShapeInfoTriangle;
					else if( triToSource.Length > localShapeInfoTriangle )
						resultItem.TriangleIndexSource = triToSource[ localShapeInfoTriangle ];
				}

				return resultItem;
			}

			public void PostProcess()
			{
				item.Result = Array.Empty<PhysicsRayTestItem.ResultItem>();

				// if CollisionObject == null RayTest has no results
				if( CollisionObject == null )
					return;

				if( item.Mode == PhysicsRayTestItem.ModeEnum.OneClosest || item.Mode == PhysicsRayTestItem.ModeEnum.One )
				{
					var result0 = CreateResultItem( CollisionObject, ClosestHitFraction );
					item.Result = new PhysicsRayTestItem.ResultItem[] { result0 };
					return;
				}

				if( resultList.Count == 0 )
					return;

				if( resultList.Count == 1 || item.Mode == PhysicsRayTestItem.ModeEnum.One )
				{
					item.Result = new PhysicsRayTestItem.ResultItem[] { resultList[ 0 ] };
					return;
				}

				if( item.Mode == PhysicsRayTestItem.ModeEnum.OneForEach )
				{
					item.Result = GetResultsOnePerShape( resultList );
					return;
				}

				//sort by distance in any other case
				CollectionUtility.SelectionSort( resultList, this );

				if( item.Mode == PhysicsRayTestItem.ModeEnum.All )
				{
					item.Result = resultList.ToArray();
					return;
				}

				//item.Mode == PhysicsRayTestItem.ModeEnum.OneClosestForEach
				item.Result = GetResultsOnePerShape( resultList );
			}

			static PhysicsRayTestItem.ResultItem[] GetResultsOnePerShape( List<PhysicsRayTestItem.ResultItem> list )
			{
				var resultCount = 0;
				var result = new PhysicsRayTestItem.ResultItem[ list.Count ];

				for( int i = 0; i < list.Count; i++ )
				{
					object shapeBody = list[ i ].Shape;
					if( shapeBody == null )
						shapeBody = list[ i ].Body;

					var added = false;

					for( int idx = 0; idx < resultCount; idx++ )
					{
						object addedShapeBody = result[ idx ].Shape;
						if( addedShapeBody == null )
							addedShapeBody = result[ idx ].Body;

						if( ReferenceEquals( addedShapeBody, shapeBody ) )
						{
							added = true;
							break;
						}
					}

					if( !added )
						result[ resultCount++ ] = list[ i ];
				}

				if( resultCount != result.Length )
					Array.Resize( ref result, resultCount );

				return result;
			}

			public int Compare( PhysicsRayTestItem.ResultItem x, PhysicsRayTestItem.ResultItem y )
			{
				if( x.DistanceScale < y.DistanceScale )
					return -1;
				if( x.DistanceScale > y.DistanceScale )
					return 1;
				return 0;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public void PhysicsRayTest( PhysicsRayTestItem[] items )
		{
			if( physicsWorldData == null )
				return;

			//!!!!threading. выполнять когда включается режим "можно", например во время рендера.
			//!!!!!в других кастах тоже.

			//!!!!slowly. batching support. в других методах тоже

			//!!!!contactGroup. сами группы тоже пересмотреть
			//!!!!!!может указывать маску групп, т.е. список групп
			//!!!!!!может еще что-то

			foreach( var item in items )
			{
				using( var cb = new BulletRayResultCallback( item ) )
				{
					//!!!!
					cb.CollisionFilterGroup = item.CollisionFilterGroup;
					cb.CollisionFilterMask = item.CollisionFilterMask;

					var castObject = item.SingleCastCollisionObject;
					if( castObject != null )
					{
						//single object cast

						if( castObject is SoftBody softBody )
						{
							var cbResult = new SoftBodyRayCast();
							var r = softBody.RayTest( BulletPhysicsUtility.Convert( item.Ray.Origin ), BulletPhysicsUtility.Convert( item.Ray.GetEndPoint() ), cbResult );
							if( r )
							{
								var lsi = new LocalShapeInfo();
								lsi.TriangleIndex = cbResult.Index;
								var lrr = new LocalRayResult( softBody, lsi, BulletSharp.Math.Vector3.Zero, cbResult.Fraction );

								cb.AddSingleResult( lrr, true );
							}
						}
						else
						{
							var fromMatrix = BulletPhysicsUtility.Convert( Matrix4.FromTranslate( item.Ray.Origin ) );
							var toMatrix = BulletPhysicsUtility.Convert( Matrix4.FromTranslate( item.Ray.GetEndPoint() ) );
							var objectTransform = castObject.WorldTransform;

							CollisionWorld.RayTestSingleRef( ref fromMatrix, ref toMatrix, castObject, castObject.CollisionShape, ref objectTransform, cb );
						}
					}
					else
					{
						//usual all objects cast

						var from = BulletPhysicsUtility.Convert( item.Ray.Origin );
						var to = BulletPhysicsUtility.Convert( item.Ray.GetEndPoint() );

						physicsWorldData.world.RayTestRef( ref from, ref to, cb );
					}

					//sort results
					cb.PostProcess();
				}
			}
		}

		public void PhysicsRayTest( PhysicsRayTestItem item )
		{
			PhysicsRayTest( new PhysicsRayTestItem[] { item } );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		class BulletContactResultCallback : ContactResultCallback, IComparer<PhysicsContactTestItem.ResultItem>
		{
			public PhysicsContactTestItem item;
			public List<PhysicsContactTestItem.ResultItem> resultList = new List<PhysicsContactTestItem.ResultItem>();

			//for OneForEach, OneClosestForEach
			Dictionary<object, double> processedCollisionObjects = new Dictionary<object, double>();
			ManifoldPoint manifoldPoint;
			CollisionObjectWrapper wrappedObj;

			//

			public BulletContactResultCallback( PhysicsContactTestItem item )
			{
				this.item = item;

				ClosestDistanceThreshold = item.ClosestDistanceThreshold;

				//!!!!
				CollisionFilterGroup = (int)CollisionFilterGroups.DefaultFilter;// ( int)item.ContactGroup;
				CollisionFilterMask = (int)CollisionFilterGroups.AllFilter;// item.ContactMask;
			}

			public override bool NeedsCollision( BroadphaseProxy proxy0 )
			{
				if( item.Mode == PhysicsContactTestItem.ModeEnum.One && processedCollisionObjects.Count > 0 )
					return false;

				if( item.Mode == PhysicsContactTestItem.ModeEnum.One && wrappedObj != null )
					return false;

				var obj = proxy0.ClientObject;

				if( obj is SoftBody soft )
				{
					if( processedCollisionObjects.ContainsKey( soft ) )
						return false;

					processedCollisionObjects.Add( soft, .5 );
					var compSoft = soft.UserObject as IComponent_SoftBody;

					item.CollisionObject.CollisionShape.GetBoundingSphere( out var cPos, out _ );

					var cPosW = BulletPhysicsUtility.Convert( BulletSharp.Math.Vector3.TransformCoordinate( cPos, item.CollisionObject.WorldTransform ) );

					var nodeIdx = compSoft.FindClosestNodeIndex( cPosW );

					var resultItem = new PhysicsContactTestItem.ResultItem
					{
						Body = (Component_PhysicalBody)compSoft,
						Distance = .5,
						PositionWorldOnA = compSoft.GetNodePosition( nodeIdx ),
						PositionWorldOnB = cPosW
					};

					resultList.Add( resultItem );

					return false;
				}

				if( item.Mode != PhysicsContactTestItem.ModeEnum.OneForEach )
					return true;

				// if OneForEach and collision object was processed already
				if( processedCollisionObjects.ContainsKey( obj ) )
					return false;

				return true;
			}

			public override double AddSingleResult( ManifoldPoint cp, CollisionObjectWrapper colObj0Wrap, int partId0, int index0, CollisionObjectWrapper colObj1Wrap, int partId1, int index1 )
			{
				//!!!!slowly? некоторые режимы могут быть медленными

				//!!!!что с возвращаемым параметром
				//!!!!
				double returnValue = cp.Distance;

				if( item.Mode == PhysicsContactTestItem.ModeEnum.OneClosest && manifoldPoint?.Distance < cp.Distance )
					return returnValue;

				manifoldPoint = cp;
				wrappedObj = ( colObj0Wrap.CollisionObject == item.CollisionObject ) ? colObj1Wrap : colObj0Wrap;

				var collisionObject = wrappedObj.CollisionObject;

				if( item.Mode == PhysicsContactTestItem.ModeEnum.OneForEach )
				{
					if( processedCollisionObjects.ContainsKey( collisionObject ) )
						return returnValue;
				}
				if( item.Mode == PhysicsContactTestItem.ModeEnum.OneClosestForEach )
				{
					if( processedCollisionObjects.TryGetValue( collisionObject, out double distance ) && distance < cp.Distance )
						return returnValue;
				}

				if( item.Mode == PhysicsContactTestItem.ModeEnum.OneClosest )
					return returnValue;

				{
					var resultItem = CreateResultItem();
					if( resultItem != null )
					{
						switch( item.Mode )
						{
						case PhysicsContactTestItem.ModeEnum.One:
							if( resultList.Count == 0 )
								resultList.Add( resultItem );
							break;

						case PhysicsContactTestItem.ModeEnum.OneForEach:
							processedCollisionObjects[ collisionObject ] = resultItem.Distance;
							resultList.Add( resultItem );
							break;

						case PhysicsContactTestItem.ModeEnum.OneClosestForEach:
							{
								processedCollisionObjects[ collisionObject ] = resultItem.Distance;

								//!!!!slowly?

								bool wasAdded = false;
								for( int n = 0; n < resultList.Count; n++ )
								{
									if( resultList[ n ].Shape?.ParentRigidBody == resultItem.Shape.ParentRigidBody )
									{
										resultList[ n ] = resultItem;
										wasAdded = true;
										break;
									}
								}
								if( !wasAdded )
									resultList.Add( resultItem );
							}
							break;

						case PhysicsContactTestItem.ModeEnum.All:
							resultList.Add( resultItem );
							break;
						}
					}
				}

				return returnValue;
			}

			PhysicsContactTestItem.ResultItem CreateResultItem()
			{
				var resultItem = new PhysicsContactTestItem.ResultItem();

				var collisionObject = wrappedObj.CollisionObject;

				if( collisionObject != item.CollisionObject )
				{
					resultItem.LocalPointA = BulletPhysicsUtility.Convert( manifoldPoint.LocalPointA );
					resultItem.PositionWorldOnA = BulletPhysicsUtility.Convert( manifoldPoint.PositionWorldOnA );
					resultItem.LocalPointB = BulletPhysicsUtility.Convert( manifoldPoint.LocalPointB );
					resultItem.PositionWorldOnB = BulletPhysicsUtility.Convert( manifoldPoint.PositionWorldOnB );
				}
				else
				{
					resultItem.LocalPointA = BulletPhysicsUtility.Convert( manifoldPoint.LocalPointB );
					resultItem.PositionWorldOnA = BulletPhysicsUtility.Convert( manifoldPoint.PositionWorldOnB );
					resultItem.LocalPointB = BulletPhysicsUtility.Convert( manifoldPoint.LocalPointA );
					resultItem.PositionWorldOnB = BulletPhysicsUtility.Convert( manifoldPoint.PositionWorldOnA );
				}

				if( item.CheckPositionWorldOnB != null && !item.CheckPositionWorldOnB( resultItem.PositionWorldOnB ) )
					return null;

				//resultItem.Normal = BulletUtils.Convert( cp.NormalWorldOnB );
				resultItem.Distance = manifoldPoint.Distance;

				CollisionShape shape;

				var shapePart = wrappedObj.PartId;

				if( collisionObject.CollisionShape is CompoundShape compoundShape )
					shape = compoundShape.GetChildShape( shapePart >= 0 ? shapePart : 0 );
				else
					shape = collisionObject.CollisionShape;

				//resultItem.Shape == null for SoftBody
				resultItem.Shape = shape.UserObject as Component_CollisionShape;

				resultItem.Body = collisionObject.UserObject as Component_RigidBody;

				//if( shape is TriangleMeshShape || shape is SoftBodyCollisionShape )
				//	resultItem.TriangleIndexProcessed = wrappedObj.Index;

				if( wrappedObj.Index != -1 )
				{
					if( resultItem.Shape is Component_CollisionShape_Mesh csMesh )
					{
						resultItem.TriangleIndexProcessed = wrappedObj.Index;

						csMesh.GetProcessedData( out _, out _, out var triToSource );

						if( triToSource == null )
							resultItem.TriangleIndexSource = wrappedObj.Index;
						else if( triToSource.Length > wrappedObj.Index )
							resultItem.TriangleIndexSource = triToSource[ wrappedObj.Index ];
					}
					else if( resultItem.Body is IComponent_SoftBody softBody )
					{
						//!!!!not checked

						resultItem.TriangleIndexProcessed = wrappedObj.Index;

						softBody.GetProcessedData( out _, out _, out var triToSource );

						if( triToSource == null )
							resultItem.TriangleIndexSource = wrappedObj.Index;
						else if( triToSource.Length > wrappedObj.Index )
							resultItem.TriangleIndexSource = triToSource[ wrappedObj.Index ];
					}
				}

				return resultItem;
			}

			public void PostProcess()
			{
				if( item.CollisionObject != null && item.CollisionObjectAutoDispose )
				{
					item.CollisionObject.CollisionShape?.Dispose();
					item.CollisionObject.Dispose();
				}

				if( item.Mode == PhysicsContactTestItem.ModeEnum.OneClosest && manifoldPoint != null )
				{
					var resultItem = CreateResultItem();
					if( resultItem != null )
						item.Result = new PhysicsContactTestItem.ResultItem[] { resultItem };
					else
						item.Result = Array.Empty<PhysicsContactTestItem.ResultItem>();
					return;
				}

				if( resultList.Count == 0 )
				{
					item.Result = Array.Empty<PhysicsContactTestItem.ResultItem>();
					return;
				}

				if( resultList.Count == 1 )
				{
					item.Result = new PhysicsContactTestItem.ResultItem[] { resultList[ 0 ] };
					return;
				}

				//sort by deep penetration
				if( item.Mode == PhysicsContactTestItem.ModeEnum.OneClosestForEach || item.Mode == PhysicsContactTestItem.ModeEnum.All )
					CollectionUtility.SelectionSort( resultList, this );

				//initialize result array
				item.Result = resultList.ToArray();
			}

			public int Compare( PhysicsContactTestItem.ResultItem x, PhysicsContactTestItem.ResultItem y )
			{
				if( x.Distance < y.Distance )
					return -1;
				if( x.Distance > y.Distance )
					return 1;
				return 0;
			}

		}

		public void PhysicsContactTest( PhysicsContactTestItem[] items )
		{
			if( physicsWorldData == null )
				return;

			foreach( var item in items )
			{
				using( var cb = new BulletContactResultCallback( item ) )
				{
					physicsWorldData.world.ContactTest( item.CollisionObject, cb );
					cb.PostProcess();
				}
			}
		}

		public void PhysicsContactTest( PhysicsContactTestItem item )
		{
			PhysicsContactTest( new PhysicsContactTestItem[] { item } );
		}

		//!!!!
		//public Body[] PhysicsVolumeCast( Bounds bounds, int contactGroup )
		//public Body[] PhysicsVolumeCast( Box box, int contactGroup )
		//public Body[] PhysicsVolumeCast( Sphere sphere, int contactGroup )
		//public Body[] PhysicsVolumeCast( Capsule capsule, int contactGroup )

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		class BulletConvexResultCallback : ConvexResultCallback, IComparer<PhysicsConvexSweepTestItem.ResultItem>
		{
			PhysicsConvexSweepTestItem item;
			List<PhysicsConvexSweepTestItem.ResultItem> resultList = new List<PhysicsConvexSweepTestItem.ResultItem>();

			//last collision data
			CollisionObject hitObject;
			BulletSharp.Math.Vector3 hitNormalWorld;
			//Vector3 hitPointWorld;
			int localShapeInfoPart;
			int localShapeInfoTriangle;

			HashSet<object> processedObjects = new HashSet<object>();

			Plane[] hullPlanes;

			public BulletConvexResultCallback( PhysicsConvexSweepTestItem item )
			{
				this.item = item;
			}

			void CalculateHullPlanes()
			{
				BulletPhysicsUtility.Convert( ref item.transformedFrom, out var transformedFrom );
				item.Shape.GetAabbRef( ref transformedFrom, out var min, out var max );
				var boundsFrom = new Bounds( BulletPhysicsUtility.ToVector3( min ), BulletPhysicsUtility.ToVector3( max ) );

				BulletPhysicsUtility.Convert( ref item.transformedTo, out var transformedTo );
				item.Shape.GetAabbRef( ref transformedTo, out min, out max );
				var boundsTo = new Bounds( BulletPhysicsUtility.ToVector3( min ), BulletPhysicsUtility.ToVector3( max ) );

				var hullPositions = new Vector3[ 16 ];
				var pointsFrom = boundsFrom.ToPoints();
				var pointsTo = boundsTo.ToPoints();
				for( int n = 0; n < pointsFrom.Length; n++ )
					hullPositions[ n ] = pointsFrom[ n ];
				for( int n = 0; n < pointsTo.Length; n++ )
					hullPositions[ 8 + n ] = pointsTo[ n ];

				ConvexHullAlgorithm.Create( hullPositions, out hullPlanes );

				//ConvexHullAlgorithm.Create( hullPositions, out var hullVertices, out var hullIndices );

				//hullPlanes = new Plane[ hullIndices.Length / 3 ];
				//for( int i = 0; i < hullIndices.Length; i += 3 )
				//{
				//	var v0 = hullVertices[ hullIndices[ i ] ];
				//	var v1 = hullVertices[ hullIndices[ i + 1 ] ];
				//	var v2 = hullVertices[ hullIndices[ i + 2 ] ];

				//	hullPlanes[ i / 3 ] = Plane.FromPoints( v0, v1, v2 );
				//}
			}

			public override bool NeedsCollision( BroadphaseProxy proxy0 )
			{
				if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.One && processedObjects.Count > 0 )
					return false;

				var obj = proxy0.ClientObject;

				if( obj is SoftBody soft )
				{
					if( processedObjects.Contains( soft ) )
						return false;

					//!!!!slowly?
					if( hullPlanes == null )
						CalculateHullPlanes();
					//var bounds = new Bounds( BulletUtils.ToVec3( proxy0.AabbMin ), BulletUtils.ToVec3( proxy0.AabbMax ) );
					//soft.GetAabb( out var aabbMin, out var aabbMax );
					//var bounds = new Bounds( BulletUtils.ToVec3( aabbMin ), BulletUtils.ToVec3( aabbMax ) );
					//if( !MathAlgorithms.IntersectsConvexHull( hullPlanes, bounds ) )
					//	return false;
					if( !BulletPhysicsUtility.IntersectsConvexHull( hullPlanes, soft ) )
						return false;

					processedObjects.Add( soft );

					var compSoft = soft.UserObject as IComponent_SoftBody;
					var pos = BulletPhysicsUtility.ToVector3( ( ( proxy0.AabbMax + proxy0.AabbMin ) * .5 ) );

					var originPos = item.OriginalFrom.GetTranslation();
					//var closestNodeIdx = compSoft.FindClosestNodeIndex( originPos );
					//pos = compSoft.GetNodePosition( closestNodeIdx );

					var sweepLen = ( originPos - item.OriginalTo.GetTranslation() ).Length();
					var distToOrigin = ( pos - originPos ).Length();

					var resultItem = new PhysicsConvexSweepTestItem.ResultItem
					{
						Body = (Component_ObjectInSpace)compSoft,
						DistanceScale = distToOrigin / sweepLen,
						Position = compSoft.GetNodePosition( 0 )
					};

					resultList.Add( resultItem );

					return false;
				}

				return true;
			}

			public override double AddSingleResult( LocalConvexResult convexResult, bool normalInWorldSpace )
			{
				hitObject = convexResult.HitCollisionObject;

				//will terminate further ray tests, once the closestHitFraction reached zero
				if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.One )
					ClosestHitFraction = 0;

				//save data of last collision

				if( convexResult.LocalShapeInfo.Native != IntPtr.Zero )
				{
					localShapeInfoPart = convexResult.LocalShapeInfo?.ShapePart ?? 0;
					localShapeInfoTriangle = convexResult.LocalShapeInfo?.TriangleIndex ?? 0;
				}
				else
				{
					localShapeInfoPart = 0;
					localShapeInfoTriangle = 0;
				}

				if( normalInWorldSpace )
				{
					hitNormalWorld = convexResult.HitNormalLocal;
					//hitPointWorld = convexResult.HitPointLocal;
				}
				else
				{
					hitNormalWorld = BulletSharp.Math.Vector3.TransformCoordinate( convexResult.HitNormalLocal, hitObject.WorldTransform.Basis );
					//hitPointWorld = Vector3.TransformCoordinate( convexResult.HitPointLocal, hitObject.WorldTransform.Basis );
				}

				//processing found collision
				if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneClosest )
				{
					ClosestHitFraction = convexResult.HitFraction;
					return convexResult.HitFraction;
				}

				var resultItem = CreateResultItem( hitObject, convexResult.HitFraction );
				resultList.Add( resultItem );
				return ClosestHitFraction;
			}

			PhysicsConvexSweepTestItem.ResultItem CreateResultItem( CollisionObject obj, double hitFraction )
			{
				var resultItem = new PhysicsConvexSweepTestItem.ResultItem();
				resultItem.TriangleIndexProcessed = -1;
				resultItem.TriangleIndexSource = -1;

				CollisionShape shape;

				//LocalShapeInfo gives extra information for complex shapes
				//Currently, only btTriangleMeshShape is available, so it just contains triangleIndex and subpart

				//фича буллета. индекс шейпа приходит как TriangleIndex
				var shapeIdx = localShapeInfoPart < 0 ? localShapeInfoTriangle : 0;

				if( obj.CollisionShape is CompoundShape compoundShape )
				{
					if( compoundShape.NumChildShapes == 1 )
						shape = compoundShape.GetChildShape( 0 );
					else
						shape = compoundShape.GetChildShape( shapeIdx );
				}
				else
					shape = obj.CollisionShape;

				resultItem.Shape = shape.UserObject as Component_CollisionShape;

				resultItem.Body = obj.UserObject as Component_ObjectInSpace;
				resultItem.Normal = BulletPhysicsUtility.Convert( hitNormalWorld );
				resultItem.DistanceScale = hitFraction;
				resultItem.Position = Vector3.Lerp( item.originalFrom.GetTranslation(), item.originalTo.GetTranslation(), hitFraction );
				//resultItem.HitPoint = BulletUtils.Convert( hitPointWorld );

				//if( shape is TriangleMeshShape || shape is SoftBodyCollisionShape )
				//	resultItem.TriangleIndexProcessed = localShapeInfoTriangle;

				if( resultItem.Shape is Component_CollisionShape_Mesh csMesh )
				{
					resultItem.TriangleIndexProcessed = localShapeInfoTriangle;

					csMesh.GetProcessedData( out _, out _, out var triToSource );

					if( triToSource == null )
						resultItem.TriangleIndexSource = localShapeInfoTriangle;
					else if( triToSource.Length > localShapeInfoTriangle )
						resultItem.TriangleIndexSource = triToSource[ localShapeInfoTriangle ];
				}
				else if( resultItem.Body is IComponent_SoftBody softBody )
				{
					//!!!!not checked

					resultItem.TriangleIndexProcessed = localShapeInfoTriangle;

					softBody.GetProcessedData( out _, out _, out var triToSource );

					if( triToSource == null )
						resultItem.TriangleIndexSource = localShapeInfoTriangle;
					else if( triToSource.Length > localShapeInfoTriangle )
						resultItem.TriangleIndexSource = triToSource[ localShapeInfoTriangle ];
				}

				return resultItem;
			}

			public void PostProcess()
			{
				if( item.Shape != null && item.ShapeAutoDispose )
					item.Shape.Dispose();

				item.Result = Array.Empty<PhysicsConvexSweepTestItem.ResultItem>();

				// if hitObject == null ConvexTest has no results
				if( hitObject == null )
					return;

				if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneClosest )
				{
					var result0 = CreateResultItem( hitObject, ClosestHitFraction );
					item.Result = new PhysicsConvexSweepTestItem.ResultItem[] { result0 };
					return;
				}

				if( resultList.Count == 0 )
					return;

				if( resultList.Count == 1 || item.Mode == PhysicsConvexSweepTestItem.ModeEnum.One )
				{
					item.Result = new PhysicsConvexSweepTestItem.ResultItem[] { resultList[ 0 ] };
					return;
				}

				if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneForEach )
				{
					item.Result = GetResultsOnePerShape( resultList );
					return;
				}

				//sort by distance in any other case
				CollectionUtility.SelectionSort( resultList, this );

				if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.All )
				{
					item.Result = resultList.ToArray();
					return;
				}

				//item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneClosestForEach
				item.Result = GetResultsOnePerShape( resultList );
			}

			static PhysicsConvexSweepTestItem.ResultItem[] GetResultsOnePerShape( List<PhysicsConvexSweepTestItem.ResultItem> list )
			{
				var resultCount = 0;
				var result = new PhysicsConvexSweepTestItem.ResultItem[ list.Count ];

				for( int i = 0; i < list.Count; i++ )
				{
					object shapeBody = list[ i ].Shape;

					if( shapeBody == null )
						shapeBody = list[ i ].Body;

					var added = false;

					for( int idx = 0; idx < resultCount; idx++ )
					{
						object addedShapeBody = result[ idx ].Shape;

						if( addedShapeBody == null )
							addedShapeBody = result[ idx ].Body;

						if( ReferenceEquals( addedShapeBody, shapeBody ) )
						{
							added = true;
							break;
						}
					}

					if( !added )
						result[ resultCount++ ] = list[ i ];
				}

				if( resultCount != result.Length )
					Array.Resize( ref result, resultCount );

				return result;
			}

			public int Compare( PhysicsConvexSweepTestItem.ResultItem x, PhysicsConvexSweepTestItem.ResultItem y )
			{
				if( x.DistanceScale < y.DistanceScale )
					return -1;
				if( x.DistanceScale > y.DistanceScale )
					return 1;
				return 0;
			}
		}

		//class BulletConvexResultCallback_Before : ConvexResultCallback
		//{
		//	public override double AddSingleResult( LocalConvexResult convexResult, bool normalInWorldSpace )
		//	{
		//		if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneClosest )
		//			ClosestHitFraction = convexResult.HitFraction;

		//		var resultItem = new PhysicsConvexSweepTestItem.ResultItem();

		//		var collisionObject = convexResult.HitCollisionObject;
		//		var body = collisionObject as RigidBody;
		//		//!!!!soft body может быть
		//		if( body != null )
		//		{
		//			//Log.Fatal( "body == null." );
		//			//if( body == null )
		//			//	Log.Fatal( "impl. body == null." );

		//			CollisionShape shape;

		//			//!!!!temp. need get ShapePart

		//			var compoundShape = body.CollisionShape as CompoundShape;
		//			if( compoundShape != null )
		//				shape = compoundShape.GetChildShape( 0 );
		//			else
		//				shape = body.CollisionShape;

		//			resultItem.Shape = shape.UserObject as Component_CollisionShape;
		//		}

		//		Vector3 normal;
		//		if( normalInWorldSpace )
		//			normal = convexResult.HitNormalLocal;
		//		else
		//			normal = Vector3.TransformCoordinate( convexResult.HitNormalLocal, collisionObject.WorldTransform.Basis );
		//		resultItem.Normal = BulletUtils.Convert( normal );
		//		resultItem.DistanceScale = convexResult.HitFraction;
		//		resultItem.Position = Vec3.Lerp( item.originalFrom.GetTranslation(), item.originalTo.GetTranslation(), resultItem.DistanceScale );

		//		//!!!!крешится
		//		//	shapePart = rayResult.LocalShapeInfo.ShapePart;
		//		//resultItem.triangleIndex = rayResult.LocalShapeInfo.TriangleIndex;

		//		//!!!!может полезно
		//		//HitPointWorld = convexResult.HitPointLocal;



		//		if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneClosest )
		//			resultList.Clear();
		//		resultList.Add( resultItem );


		//		if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneClosest )
		//			return convexResult.HitFraction;
		//		else
		//			return ClosestHitFraction;
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public void PhysicsConvexSweepTest( PhysicsConvexSweepTestItem[] items )
		{
			if( physicsWorldData == null )
				return;

			//!!!!contactGroup. сами группы тоже пересмотреть
			//!!!!!!может указывать маску групп, т.е. список групп
			//!!!!!!может еще что-то

			foreach( var item in items )
			{
				using( var cb = new BulletConvexResultCallback( item ) )
				{
					BulletPhysicsUtility.Convert( ref item.transformedFrom, out Matrix from );
					BulletPhysicsUtility.Convert( ref item.transformedTo, out Matrix to );
					physicsWorldData.world.ConvexSweepTestRef( item.Shape, ref from, ref to, cb, item.AllowedCcdPenetration );
					cb.PostProcess();
				}
			}
		}

		public void PhysicsConvexSweepTest( PhysicsConvexSweepTestItem item )
		{
			PhysicsConvexSweepTest( new PhysicsConvexSweepTestItem[] { item } );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		////!!!!temp
		//void PhysicsTest( Viewport viewport )
		//{
		//	if( viewport.Simple3DRenderer == null )
		//		return;

		//	var rayTest = false;
		//	if( rayTest )
		//	{
		//		Vector3 from = new Vector3( 6, 10, 30 );
		//		Vector3 to = new Vector3( 6, 1, -1 );
		//		//Vec3 from = new Vec3( 1, 1, -1 );
		//		//Vec3 to = new Vec3( 3, 10, 30 );

		//		viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0, 1 ) );
		//		viewport.Simple3DRenderer.AddLine( from, to );

		//		var rayType = PhysicsRayTestItem.ModeEnum.OneClosest;
		//		if( viewport.IsKeyPressed( EKeys.H ) )
		//			rayType = PhysicsRayTestItem.ModeEnum.OneClosest;
		//		if( viewport.IsKeyPressed( EKeys.J ) )
		//			rayType = PhysicsRayTestItem.ModeEnum.OneForEach;
		//		if( viewport.IsKeyPressed( EKeys.K ) )
		//			rayType = PhysicsRayTestItem.ModeEnum.OneClosestForEach;
		//		if( viewport.IsKeyPressed( EKeys.L ) )
		//			rayType = PhysicsRayTestItem.ModeEnum.All;

		//		var item = new PhysicsRayTestItem( new Ray( from, to - from ), 1, -1, rayType );
		//		PhysicsRayTest( new PhysicsRayTestItem[] { item } );

		//		foreach( var resultItem in item.Result )
		//			viewport.Simple3DRenderer.AddSphere( resultItem.Position, .1 );

		//		//Bounds b = new Bounds( -.5, -.5, -.5, .5, .5, .5 );
		//		//var result = PhysicsConvexCastClosest_Box( 
		//		//	Mat4.FromTranslate( new Vec3( 2.4193549156189, 7.38709712028503, 20.5 + .1 ) ),
		//		//	Mat4.FromTranslate( new Vec3( 2.4193549156189, 7.38709712028503, 20.5 ) ), 0, b );
		//		//if( result != null )
		//		//	viewport.DebugGeometry.AddBounds( b + result.Position );
		//		//else
		//		//{
		//		//	viewport.DebugGeometry.SetColor( new ColorValue( 0, 1, 0, 1 ) );
		//		//	viewport.DebugGeometry.AddBounds( b + new Vec3( 2.4193549156189, 7.38709712028503, 20.5 ) );
		//		//}

		//		//Bounds b = new Bounds( -.5, -.5, -.5, .5, .5, .5 );
		//		//var result = PhysicsConvexCastClosest_Box( Mat4.FromTranslate( from ), Mat4.FromTranslate( to ), 0, b );
		//		//if( result != null )
		//		//viewport.DebugGeometry.AddBounds( b + result.Position );

		//		//double r = 0.5;
		//		//var result = PhysicsConvexCastClosest_Sphere( Mat4.FromTranslate( from ), Mat4.FromTranslate( to ), 0, r );
		//		//if( result != null )
		//		//	viewport.DebugGeometry.AddSphere( result.Position, r );
		//	}

		//	bool convexSweepTest = false;
		//	if( convexSweepTest )
		//	{
		//		Vector3 from = new Vector3( 3, 10, 30 );
		//		Vector3 to = new Vector3( 1, 1, -1 );

		//		viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0, 1 ) );
		//		//viewport.Simple3DRenderer.AddLine( from, to );

		//		Box b = new Box( new Vector3( 3, 0, 0 ), new Vector3( 1, 0.5, 0.5 ), Matrix3.FromRotateByX( 1 ) * Matrix3.FromRotateByY( 2 ) );

		//		var bFrom = b + from;
		//		var bTo = b + to;

		//		viewport.Simple3DRenderer.AddBox( bFrom );
		//		viewport.Simple3DRenderer.AddBox( bTo );

		//		var pp0 = bFrom.ToPoints();
		//		var pp1 = bTo.ToPoints();

		//		viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0, 1 ) );

		//		for( int i = 0; i < pp0.Length; i++ )
		//			viewport.Simple3DRenderer.AddLine( pp0[ i ], pp1[ i ] );

		//		viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0, 1 ) );
		//		//Box b = new Box( new Vec3( 3, 0, 0 ), new Vec3( 1, 0.5, 0.5 ), Mat3.Identity );

		//		//Box b = new Box( Vec3.Zero, new Vec3( 2, 1, 1 ), Mat3.Identity );
		//		//b += new Vec3( 1, 0, 0 );

		//		var castType = PhysicsConvexSweepTestItem.ModeEnum.One;
		//		if( viewport.IsKeyPressed( EKeys.H ) )
		//			castType = PhysicsConvexSweepTestItem.ModeEnum.OneClosest;
		//		if( viewport.IsKeyPressed( EKeys.J ) )
		//			castType = PhysicsConvexSweepTestItem.ModeEnum.OneForEach;
		//		if( viewport.IsKeyPressed( EKeys.K ) )
		//			castType = PhysicsConvexSweepTestItem.ModeEnum.OneClosestForEach;
		//		if( viewport.IsKeyPressed( EKeys.L ) )
		//			castType = PhysicsConvexSweepTestItem.ModeEnum.All;

		//		var item = new PhysicsConvexSweepTestItem( Matrix4.FromTranslate( from ), Matrix4.FromTranslate( to ), 1, -1, castType, b );
		//		PhysicsConvexSweepTest( new PhysicsConvexSweepTestItem[] { item } );
		//		foreach( var resultItem in item.Result )
		//			viewport.Simple3DRenderer.AddBox( b + resultItem.Position );
		//	}

		//	bool contactTest = false;
		//	if( contactTest )
		//	{
		//		var contactType = PhysicsContactTestItem.ModeEnum.One;
		//		if( viewport.IsKeyPressed( EKeys.H ) )
		//			contactType = PhysicsContactTestItem.ModeEnum.OneClosest;
		//		if( viewport.IsKeyPressed( EKeys.J ) )
		//			contactType = PhysicsContactTestItem.ModeEnum.OneForEach;
		//		if( viewport.IsKeyPressed( EKeys.K ) )
		//			contactType = PhysicsContactTestItem.ModeEnum.OneClosestForEach;
		//		if( viewport.IsKeyPressed( EKeys.L ) )
		//			contactType = PhysicsContactTestItem.ModeEnum.All;

		//		////sphere
		//		//Sphere sphere = new Sphere( new Vec3( 6, 10, 10 ), 10.0 );
		//		//viewport.DebugGeometry.SetColor( new ColorValue( 1, 0, 0 ) );
		//		//viewport.DebugGeometry.AddSphere( sphere );
		//		//var item = new PhysicsContactTestItem( 0, type, sphere );

		//		//box
		//		Box box = new Box( new Vector3( 6, 10, 10 ), new Vector3( 5, 7, 10 ), Matrix3.FromRotateByX( 23 ) );
		//		viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
		//		viewport.Simple3DRenderer.AddBox( box );
		//		var item = new PhysicsContactTestItem( 1, -1, contactType, box );

		//		////bounds
		//		//Bounds bounds = new Bounds( new Vec3( 6 - 3, 10 - 3, 10 - 6 ), new Vec3( 6 + 4, 10 + 6, 10 + 7 ) );
		//		//viewport.DebugGeometry.SetColor( new ColorValue( 1, 0, 0 ) );
		//		//viewport.DebugGeometry.AddBounds( bounds );
		//		//var item = new PhysicsContactTestItem( 0, type, bounds );

		//		////capsule
		//		//Capsule capsule = new Capsule( new Vec3( 6 - 3, 10 - 3, 10 - 6 ), new Vec3( 6 + 4, 10 + 6, 10 + 7 ), 3 );
		//		//viewport.DebugGeometry.SetColor( new ColorValue( 1, 0, 0 ) );
		//		//viewport.DebugGeometry.AddCapsule( capsule );
		//		//var item = new PhysicsContactTestItem( 0, type, capsule );

		//		PhysicsContactTest( new PhysicsContactTestItem[] { item } );

		//		viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 1, 0 ) );
		//		foreach( var resultItem in item.Result )
		//		{
		//			//!!!!

		//			viewport.Simple3DRenderer.AddLine( resultItem.PositionWorldOnA, resultItem.PositionWorldOnB );

		//			viewport.Simple3DRenderer.AddSphere( new Sphere( resultItem.PositionWorldOnA, .1 ) );
		//			viewport.Simple3DRenderer.AddSphere( new Sphere( resultItem.PositionWorldOnB, .1 ) );

		//			if( resultItem.Shape != null )
		//				resultItem.Shape.DebugRender( viewport, resultItem.Shape.ParentRigidBody.Transform, true );
		//		}
		//	}

		//	//Sphere s = new Sphere( Vec3.Zero, 1 );
		//	//var item = new PhysicsConvexCastItem( Mat4.FromTranslate( from ), Mat4.FromTranslate( to ), 0,
		//	//	PhysicsConvexCastItem.CastTypeEnum.Closest, s );
		//	//PhysicsConvexCast( new PhysicsConvexCastItem[] { item } );
		//	//foreach( var resultItem in item.result )
		//	//	viewport.DebugGeometry.AddSphere( s.Origin + resultItem.position, s.radius );



		//	//Bounds b = new Bounds( -.5, -.5, -.5, .5, .5, .5 );
		//	////b += new Vec3( 1, 0, 0 );
		//	//var item = new PhysicsConvexCastItem( Mat4.FromTranslate( from ), Mat4.FromTranslate( to ), 0,
		//	//	PhysicsConvexCastItem.CastTypeEnum.Closest, b );
		//	//PhysicsConvexCast( new PhysicsConvexCastItem[] { item } );
		//	//foreach( var resultItem in item.result )
		//	//	viewport.DebugGeometry.AddBounds( b + resultItem.position );

		//	//!!!!
		//	//Bounds b = new Bounds( -.5, -.5, -.5, .5, .5, .5 );
		//	//var result = PhysicsConvexCastClosestBox( Mat4.FromTranslate( from ), Mat4.FromTranslate( to ), 0, b );
		//	//if( result != null )
		//	//	viewport.DebugGeometry.AddBounds( b + result.Position );



		//	//double r = 0.5;
		//	//var result = PhysicsConvexCastClosest_Sphere( Mat4.FromTranslate( from ), Mat4.FromTranslate( to ), 0, r );
		//	//if( result != null )
		//	//	viewport.DebugGeometry.AddSphere( result.Position, r );
		//}
	}
}
