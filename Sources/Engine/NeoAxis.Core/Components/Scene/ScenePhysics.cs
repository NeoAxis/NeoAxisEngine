// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Text;

namespace NeoAxis
{
	public partial class Scene
	{
		PhysicsWorldClass physicsWorld;

		/////////////////////

		/// <summary>
		/// Provides a data of Bullet physics engine.
		/// </summary>
		public class PhysicsWorldClass
		{
			internal Scene Scene;

			public IntPtr physicsSystem;
			internal Dictionary<string, Shape> nativeShapes = new Dictionary<string, Shape>();

			internal Dictionary<uint, Body> allRigidBodies = new Dictionary<uint, Body>( 256 );
			internal Dictionary<uint, Body> kinematicDynamicRigidBodies = new Dictionary<uint, Body>( 256 );
			internal ESet<Constraint> allConstraints = new ESet<Constraint>();
			internal ESet<TwoBodyConstraint> twoBodyConstraints = new ESet<TwoBodyConstraint>();

			////////////

			public class Shape
			{
				internal PhysicsWorldClass physicsWorld;
				internal RigidBody sourceData;
				internal string key;//internal (RigidBody, Vector3I) key;
				internal IntPtr nativeShape;
				internal CollisionShapeData[] collisionShapes;//it is structs
				internal Vector3F scale;//!!!!discretized

				internal int referenceCounter;

				//

				public struct CollisionShapeData
				{
					//Mesh shape specific
					public Vector3F[] meshShapeProcessedVertices;
					public int[] meshShapeProcessedIndices;
					public int[] meshShapeProcessedTrianglesToSourceIndex;
				}

				public PhysicsWorldClass PhysicsWorld
				{
					get { return physicsWorld; }
				}

				public void RayTest( PhysicsRayTestItem item, ref Vector3 bodyPosition, ref QuaternionF bodyRotation )
				{
					unsafe
					{
						if( physicsWorld == null )
						{
							item.Result = Array.Empty<PhysicsRayTestItem.ResultItem>();
							return;
						}

						//!!!!slowly
						var bodyTransform = new Matrix4( bodyRotation.ToMatrix3(), bodyPosition );
						bodyTransform.GetInverse( out var bodyTransformInv );

						var localRay = item.Ray * bodyTransformInv;

						PhysicsNative.JPhysicsSystem_RayTest( physicsWorld.physicsSystem, ref localRay, (int)item.Mode, (int)item.Flags, out var resultCount, out var results, nativeShape );

						if( resultCount != 0 )
						{
							var results2 = new PhysicsRayTestItem.ResultItem[ resultCount ];

							for( int n = 0; n < resultCount; n++ )
							{
								ref var result = ref results[ n ];
								ref var result2 = ref results2[ n ];

								//result2.Body = physicsWorld.allRigidBodies[ result.bodyId ];
								//!!!!
								//result2.ShapeIndex = result.shapeIndex;
								item.Ray.GetPointOnRay( result.distanceScale, out result2.Position );
								//result2.Position = bodyTransform * result.position;
								result2.DistanceScale = result.distanceScale;
								if( ( item.Flags & PhysicsRayTestItem.FlagsEnum.CalculateNormal ) != 0 )
									result2.Normal = bodyRotation * result.normal;
								//!!!!
								//result2.TriangleIndexSource = result.triangleIndexSource;
								//result2.TriangleIndexProcessed = result.triangleIndexProcessed;
							}

							item.Result = results2;
						}
						else
							item.Result = Array.Empty<PhysicsRayTestItem.ResultItem>();

						if( results != null )
							PhysicsNative.JPhysicsSystem_RayTestFree( physicsWorld.physicsSystem, results );
					}
				}

				public void RayTest( PhysicsRayTestItem item, Vector3 bodyPosition, QuaternionF bodyRotation )
				{
					RayTest( item, ref bodyPosition, ref bodyRotation );
				}

				public void RayTest( PhysicsRayTestItem[] items, Vector3 bodyPosition, QuaternionF bodyRotation, bool multithreaded )
				{
					if( multithreaded )
						Parallel.ForEach( items, i => RayTest( i, ref bodyPosition, ref bodyRotation ) );
					else
					{
						foreach( var item in items )
							RayTest( item, ref bodyPosition, ref bodyRotation );
					}
				}
			}

			/////////////////////

			public class Body : IPhysicalObject
			{
				internal PhysicsWorldClass physicsWorld;
				internal Shape shape;
				internal bool shapeAutoDestroy;
				internal object owner;
				internal IntPtr body;
				internal uint bodyId;
				internal PhysicsMotionType motionType;
				internal PhysicsMotionQuality motionQuality;
				float gravityFactor = 1.0f;
				float linearDamping = 0.05f;//0.1f;
				float angularDamping = 0.05f;//0.1f;
				float friction = 0.0f;
				float restitution = 0.0f;

				internal Vector3 position;
				internal QuaternionF rotation;
				Vector3F linearVelocity;
				Vector3F angularVelocity;
				internal bool active;
				//!!!!new
				internal int activeUpdateCount;

				internal bool disposed;

				internal List<Constraint> linkedConstraints;

				//character mode
				internal bool characterMode;
				float characterModeMaxStrength = 100;
				float characterModePredictiveContactDistance = 0.1f;
				float characterModeWalkUpHeight = 0.4f;
				float characterModeWalkDownHeight = 0.5f;
				float characterModeMaxSlopeAngle = MathEx.DegreeToRadian( 50 );
				float characterModeSetSupportingVolume = 1.0e10f;
				Vector2F characterModeDesiredVelocity;

				//public object AnyData;

				////////////////////

				public PhysicsWorldClass PhysicsWorld
				{
					get { return physicsWorld; }
				}

				public Shape Shape
				{
					get { return shape; }
				}

				public object Owner
				{
					get { return owner; }
					set { owner = value; }
				}

				//public Vector3F Scale
				//{
				//	get { return scale; }
				//}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public void SetTransform( ref Vector3 position, ref QuaternionF rotation, bool activate )
				{
					this.position = position;
					this.rotation = rotation;
					if( MotionType == PhysicsMotionType.Dynamic )
					{
						active = activate;
						if( !active )
							activeUpdateCount = 0;
					}

					PhysicsNative.JBody_SetTransform( body, ref position, ref rotation, activate );
				}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public void SetTransform( Vector3 position, QuaternionF rotation, bool activate )
				{
					SetTransform( ref position, ref rotation, activate );
				}

				public float GravityFactor
				{
					get { return gravityFactor; }
					set
					{
						if( gravityFactor == value )
							return;
						gravityFactor = value;
						if( body != IntPtr.Zero )
							PhysicsNative.JBody_SetGravityFactor( body, gravityFactor );
					}
				}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public void GetBounds( out Bounds bounds )
				{
					PhysicsNative.JBody_GetAABB( body, out bounds.Minimum, out bounds.Maximum );
				}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public void GetShapeCenterOfMass( out Vector3F centerOfMass )
				{
					PhysicsNative.JBody_GetShapeCenterOfMass( body, out centerOfMass );
				}

				public float LinearDamping
				{
					get { return linearDamping; }
					set
					{
						if( linearDamping == value )
							return;
						linearDamping = value;
						if( body != IntPtr.Zero )
							PhysicsNative.JBody_SetLinearDamping( body, linearDamping );
					}
				}

				public float AngularDamping
				{
					get { return angularDamping; }
					set
					{
						if( angularDamping == value )
							return;
						angularDamping = value;
						if( body != IntPtr.Zero )
							PhysicsNative.JBody_SetAngularDamping( body, angularDamping );
					}
				}

				public float Friction
				{
					get { return friction; }
					set
					{
						if( friction == value )
							return;
						friction = value;
						if( body != IntPtr.Zero )
							PhysicsNative.JBody_SetFriction( body, friction );
					}
				}

				public float Restitution
				{
					get { return restitution; }
					set
					{
						if( restitution == value )
							return;
						restitution = value;
						if( body != IntPtr.Zero )
							PhysicsNative.JBody_SetRestitution( body, restitution );
					}
				}

				public bool Active
				{
					get { return active; }
					set
					{
						if( active == value )
							return;
						if( motionType == PhysicsMotionType.Static )
							return;

						active = value;
						if( !active )
							activeUpdateCount = 0;

						if( body != IntPtr.Zero )
						{
							if( active )
								PhysicsNative.JBody_Activate( body );
							else
								PhysicsNative.JBody_Deactivate( body );
						}
					}
				}

				public int ActiveUpdateCount
				{
					get { return activeUpdateCount; }
				}

				public Vector3 Position
				{
					get { return position; }
				}

				public QuaternionF Rotation
				{
					get { return rotation; }
				}

				public Vector3F LinearVelocity
				{
					get { return linearVelocity; }
					[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
					set
					{
						if( linearVelocity == value )
							return;
						linearVelocity = value;
						PhysicsNative.JBody_SetLinearVelocity( body, ref linearVelocity );
					}
				}

				public Vector3F AngularVelocity
				{
					get { return angularVelocity; }
					[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
					set
					{
						if( angularVelocity == value )
							return;
						angularVelocity = value;
						PhysicsNative.JBody_SetAngularVelocity( body, ref angularVelocity );
					}
				}

				public Vector3F PreviousLinearVelocity { get; set; }
				public Vector3F PreviousAngularVelocity { get; set; }

				//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				//public void Activate()
				//{
				//	PhysicsNative.JBody_Activate( body );

				//	//!!!!update active?
				//}

				//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				//public void Deactivate()
				//{
				//	PhysicsNative.JBody_Deactivate( body );

				//	//!!!!update active?
				//}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				internal void UpdateDataFromPhysicsEngine()
				{
					PreviousLinearVelocity = linearVelocity;
					PreviousAngularVelocity = angularVelocity;

					PhysicsNative.JBody_GetData( body, out position, out rotation, out linearVelocity, out angularVelocity, out active );

					if( !active )
					{
						PreviousLinearVelocity = Vector3F.Zero;
						PreviousAngularVelocity = Vector3F.Zero;
						activeUpdateCount = 0;
					}
					else
					{
						unchecked
						{
							activeUpdateCount++;
						}
					}
				}

				public void RenderPhysicalObject( ViewportRenderingContext context, ref int verticesRendered )
				{
					var shapeInfoBody = shape.sourceData;//key.Item1;
					if( shapeInfoBody != null )
					{
						var tr = new Transform( Position, Rotation, shape.scale );
						shapeInfoBody.Render( context, Active, false, false, tr, ref verticesRendered );

						//GetData( out var pos, out var rot, out _, out _, out _ );
						//var tr = new Transform( pos, rot, shape.scale );
						//shapeInfoBody.Render( context, false, tr, out verticesRendered );
					}
					//else
					//	verticesRendered = 0;
				}

				public void ApplyForce( ref Vector3F force, ref Vector3F relativePosition )
				{
					PhysicsNative.JBody_ApplyForce( body, ref force, ref relativePosition );
				}

				public void ApplyForce( Vector3F force, Vector3F relativePosition )
				{
					PhysicsNative.JBody_ApplyForce( body, ref force, ref relativePosition );
				}

				public PhysicsMotionType MotionType
				{
					get { return motionType; }
				}

				public PhysicsMotionQuality MotionQuality
				{
					get { return motionQuality; }
					set
					{
						if( motionQuality == value )
							return;
						motionQuality = value;
						if( body != IntPtr.Zero )
							PhysicsNative.JBody_SetMotionQuality( body, (int)motionQuality );
					}
				}

				public bool Disposed
				{
					get { return disposed; }
				}

				public void Dispose()
				{
					PhysicsWorld.DestroyRigidBody( this );
				}

				/// <summary>
				/// Subscribes to getting and gets a contacts data of the last simulation step.
				/// </summary>
				/// <returns></returns>
				[MethodImpl( (MethodImplOptions)512 )]
				public unsafe void GetContacts( out int count, out ContactItem* buffer )
				{
					if( Disposed )
					{
						count = 0;
						buffer = null;
					}
					else
						PhysicsNative.JBody_GetContacts( body, out count, out buffer );
				}

				///// <summary>
				///// Subscribes to getting and gets a contacts data of the last simulation step.
				///// </summary>
				///// <returns></returns>
				//[MethodImpl( (MethodImplOptions)512 )]
				//public ArraySegment<ContactsItem> GetContacts()
				//{
				//	if( Disposed )
				//		return Array.Empty<ContactsItem>();

				//	//!!!!GC, caching, one array for all bodies

				//	unsafe
				//	{
				//		PhysicsNative.JBody_GetContacts( body, out var count, out var buffer );
				//		if( count == 0 )
				//			return Array.Empty<ContactsItem>();

				//		var array = new ContactsItem[ count ];
				//		for( int n = 0; n < count; n++ )
				//		{
				//			ref var input = ref buffer[ n ];
				//			ref var output = ref array[ n ];

				//			output.WorldPositionOn1 = input.worldPositionOn1;
				//			output.WorldPositionOn2 = input.worldPositionOn2;
				//			output.Body2 = physicsWorld.allRigidBodies[ input.body2Id ];
				//		}

				//		return array;
				//	}

				//	//if( !subscribedToGetContacts )
				//	//{
				//	//	subscribedToGetContacts = true;
				//	//}

				//	//return lastContacts;
				//}

				/// <summary>
				/// Subscribes to getting and checks the fact of exist contacts.
				/// </summary>
				/// <returns></returns>
				public bool ContactsExist()
				{
					if( Disposed )
						return false;
					return PhysicsNative.JBody_ContactsExist( body );
				}

				public void SetInverseInertia( ref Vector3F diagonal, ref QuaternionF rotation )
				{
					PhysicsNative.JBody_SetInverseInertia( body, ref diagonal, ref rotation );
				}

				public void SetInverseInertia( Vector3F diagonal, QuaternionF rotation )
				{
					SetInverseInertia( ref diagonal, ref rotation );
				}

				public void GetWorldSpaceSurfaceNormal( uint subShapeID, ref Vector3 position, out Vector3F normal )
				{
					PhysicsNative.JBody_GetWorldSpaceSurfaceNormal( body, subShapeID, ref position, out normal );
				}

				public Vector3F GetWorldSpaceSurfaceNormal( uint subShapeID, Vector3 position )
				{
					GetWorldSpaceSurfaceNormal( subShapeID, ref position, out var normal );
					return normal;
				}

				////////////////////

				public bool CharacterMode
				{
					get { return characterMode; }
				}

				public float CharacterModeMaxStrength
				{
					get { return characterModeMaxStrength; }
					set
					{
						if( characterModeMaxStrength == value )
							return;
						characterModeMaxStrength = value;
						PhysicsNative.JBody_SetCharacterModeMaxStrength( body, characterModeMaxStrength );
					}
				}

				public float CharacterModePredictiveContactDistance
				{
					get { return characterModePredictiveContactDistance; }
					set
					{
						if( characterModePredictiveContactDistance == value )
							return;
						characterModePredictiveContactDistance = value;
						PhysicsNative.JBody_SetCharacterModePredictiveContactDistance( body, characterModePredictiveContactDistance );
					}
				}

				public float CharacterModeWalkUpHeight
				{
					get { return characterModeWalkUpHeight; }
					set
					{
						if( characterModeWalkUpHeight == value )
							return;
						characterModeWalkUpHeight = value;
						PhysicsNative.JBody_SetCharacterModeWalkUpHeight( body, characterModeWalkUpHeight );
					}
				}

				public float CharacterModeWalkDownHeight
				{
					get { return characterModeWalkDownHeight; }
					set
					{
						if( characterModeWalkDownHeight == value )
							return;
						characterModeWalkDownHeight = value;
						PhysicsNative.JBody_SetCharacterModeWalkDownHeight( body, characterModeWalkDownHeight );
					}
				}

				public float CharacterModeMaxSlopeAngle
				{
					get { return characterModeMaxSlopeAngle; }
					set
					{
						if( characterModeMaxSlopeAngle == value )
							return;
						characterModeMaxSlopeAngle = value;
						PhysicsNative.JBody_SetCharacterModeMaxSlopeAngle( body, characterModeMaxSlopeAngle );
					}
				}

				public float CharacterModeSetSupportingVolume
				{
					get { return characterModeSetSupportingVolume; }
					set
					{
						if( characterModeSetSupportingVolume == value )
							return;
						characterModeSetSupportingVolume = value;
						PhysicsNative.JBody_SetCharacterModeSetSupportingVolume( body, characterModeSetSupportingVolume );
					}
				}

				public Vector2F CharacterModeDesiredVelocity
				{
					get { return characterModeDesiredVelocity; }
					set
					{
						if( characterModeDesiredVelocity == value )
							return;
						characterModeDesiredVelocity = value;
						PhysicsNative.JBody_SetCharacterModeSetDesiredVelocity( body, ref characterModeDesiredVelocity );
					}
				}

				public enum CharacterDataGroundState
				{
					/// <summary>
					/// Character is on the ground and can move freely.
					/// </summary>
					OnGround,
					/// <summary>
					/// Character is on a slope that is too steep and can't climb up any further. The caller should start applying downward velocity if sliding from the slope is desired.
					/// </summary>
					OnSteepGround,
					/// <summary>
					/// Character is touching an object, but is not supported by it and should fall. The GetGroundXXX functions will return information about the touched object.
					/// </summary>
					NotSupported,
					/// <summary>
					/// Character is in the air and is not touching anything.
					/// </summary>
					InAir,
				}

				public void GetCharacterModeData( out CharacterDataGroundState groundState, out uint groundBodyID, out uint groundBodySubShapeID, out Vector3 groundPosition, out Vector3F groundNormal, out Vector3F groundVelocity, out float walkUpDownLastChange )
				{
					PhysicsNative.JBody_GetCharacterModeData( body, out groundState, out groundBodyID, out groundBodySubShapeID, out groundPosition, out groundNormal, out groundVelocity, out walkUpDownLastChange );
				}
			}

			/////////////////////

			[StructLayout( LayoutKind.Sequential, Pack = 1 )]
			public struct ContactItem
			{
				public uint Body2ID;
				public Vector3F Normal;
				public float PenetrationDepth;
				public uint SubShapeID1;
				public uint SubShapeID2;
				public uint ContactPointCount;
				Vector3 ContactPointsOn10;
				Vector3 ContactPointsOn11;
				Vector3 ContactPointsOn12;
				Vector3 ContactPointsOn13;
				Vector3 ContactPointsOn20;
				Vector3 ContactPointsOn21;
				Vector3 ContactPointsOn22;
				Vector3 ContactPointsOn23;

				//

				public ref Vector3 GetContactPointOn1( int index )
				{
					unsafe
					{
						fixed( Vector3* v = &ContactPointsOn10 )
							return ref v[ index ];
					}
				}

				public ref Vector3 GetContactPointOn2( int index )
				{
					unsafe
					{
						fixed( Vector3* v = &ContactPointsOn20 )
							return ref v[ index ];
					}
				}

				//public Vector3 WorldPositionOn1;
				//public Vector3 WorldPositionOn2;
				//public Body Body2;

				////public int SimulationSubStep;
				////public Vector3 PositionWorldOnA;
				////public Vector3 PositionWorldOnB;
				////public float AppliedImpulse;
				////public float Distance;
				////public PhysicalBody BodyB;
			}

			/////////////////////

			public class Constraint : IPhysicalObject
			{
				internal PhysicsWorldClass physicsWorld;
				internal object owner;
				internal IntPtr constraint;
				internal Matrix4 constraintAFrame = Matrix4.Identity;
				internal Vector3 position;
				internal QuaternionF rotation;
				internal Vector3F visualScale;//only for drawing

				internal bool disposed;

				////////////////////

				public event Action<Constraint> DisposedEvent;

				////////////////////

				public PhysicsWorldClass PhysicsWorld
				{
					get { return physicsWorld; }
				}

				public object Owner
				{
					get { return owner; }
					set { owner = value; }
				}

				//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				//internal void UpdateDataFromPhysicsEngine()
				//{
				//	PhysicsNative.JConstraint_GetData( constraint, out position, out rotation, out linearVelocity, out angularVelocity, out active );
				//}

				public bool Disposed
				{
					get { return disposed; }
				}

				public void Dispose()
				{
					PhysicsWorld.DestroyConstraint( this );
				}

				public void RenderPhysicalObject( ViewportRenderingContext context, ref int verticesRendered )
				{
					var viewport = context.Owner;
					var renderer = viewport.Simple3DRenderer;

					//var t = Transform.Value;

					//var position = t.Position;

					var scale = Math.Max( visualScale.X, Math.Max( visualScale.Y, visualScale.Z ) );
					//var scale = Math.Max( t.Scale.X, Math.Max( t.Scale.Y, t.Scale.Z ) );

					//!!!!в конфиг
					//!!!!может указывать в пикселях? или вертикальным размером?
					//!!!!графиком настраивать
					//!!!!может картинку рисовать, может разным цветом
					double maxSize = 80;
					double minSize = 20;
					double maxDistance = 100;

					float drawConstraintSize;

					double distance = ( position - viewport.CameraSettings.Position ).Length();
					if( distance < maxDistance )
					{
						var size = MathEx.Lerp( maxSize, minSize, distance / maxDistance );
						var size2 = viewport.Simple3DRenderer.GetThicknessByPixelSize( position, size );

						drawConstraintSize = (float)( size2 * scale );
						//physicalConstraint.DebugDrawSize = size2 * scale;
					}
					else
					{
						//!!!!check: внутри не рисуется если 0?
						drawConstraintSize = 0;
						//physicalConstraint.DebugDrawSize = 0;
					}

					////!!!!TransformToolConfig
					//var toolSize = viewport.DebugGeometry.GetThicknessByPixelSize( t.Position, TransformToolConfig.size );
					//var size = toolSize / 1.5;
					//Constraint.DebugDrawSize = size;

					unsafe
					{
						PhysicsNative.JConstraint_Draw( constraint, drawConstraintSize, out var lineCount, out var lines );

						for( int n = 0; n < lineCount; n++ )
						{
							ref var line = ref lines[ n ];
							renderer.SetColor( line.Color.ToColorValue() );
							renderer.AddLineThin( line.From, line.To );
						}

						verticesRendered += lineCount * 2;

						PhysicsNative.JConstraint_DrawFree( constraint, lines );
					}
				}

				public Vector3 Position { get { return position; } }
				public QuaternionF Rotation { get { return rotation; } }
				public Vector3F VisualScale { get { return visualScale; } }

				public void SetNumVelocityStepsOverride( int value )
				{
					PhysicsNative.JConstraint_SetNumVelocityStepsOverride( constraint, value );
				}

				public void SetNumPositionStepsOverride( int value )
				{
					PhysicsNative.JConstraint_SetNumPositionStepsOverride( constraint, value );
				}

				public void SetSimulate( bool value )
				{
					PhysicsNative.JConstraint_SetSimulate( constraint, value );
				}

				internal void PerformDisposedEvent()
				{
					DisposedEvent?.Invoke( this );
				}
			}

			/////////////////////

			public class TwoBodyConstraint : Constraint
			{
				internal Body bodyA;
				internal Body bodyB;

				public Body BodyA { get { return bodyA; } }
				public Body BodyB { get { return bodyB; } }

				//

				public void SetCollisionsBetweenLinkedBodies( bool value )
				{
					PhysicsNative.JConstraint_SetCollisionsBetweenLinkedBodies( constraint, value );
				}
			}

			/////////////////////

			public class FixedConstraint : TwoBodyConstraint
			{
			}

			/////////////////////

			//!!!!юзать его когда только одна ось нужна в SixDOF?
			//maybe sense because dof6 constraint
			//public class HingeConstraint : TwoBodyConstraint
			//{
			//	//!!!!dynamic update
			//}

			/////////////////////

			public enum MotorModeEnum //it is EMotorState
			{
				None,
				Velocity,
				Position,
			}

			/////////////////////

			public class SixDOFConstraint : TwoBodyConstraint
			{
				public enum AxisEnum
				{
					TranslationX,
					TranslationY,
					TranslationZ,
					RotationX,              ///< When limited: MinLimit needs to be [-PI, 0], MaxLimit needs to be [0, PI]
					RotationY,              ///< When limited: MaxLimit between [0, PI]. MinLimit = -MaxLimit. Forms a cone shaped limit with Z.
					RotationZ,              ///< When limited: MaxLimit between [0, PI]. MinLimit = -MaxLimit. Forms a cone shaped limit with Y.
				}

				//

				/// <summary>
				/// 
				/// </summary>
				/// <param name="axis"></param>
				/// <param name="value">For rotation axis in radians.</param>
				public void SetLimit( AxisEnum axis, RangeF value )
				{
					PhysicsNative.JSixDOFConstraint_SetLimit( constraint, (int)axis, value.Minimum, value.Maximum );
				}

				public void SetFriction( AxisEnum axis, float value )
				{
					PhysicsNative.JSixDOFConstraint_SetFriction( constraint, (int)axis, value );
				}

				/// <summary>
				/// 
				/// </summary>
				/// <param name="axis"></param>
				/// <param name="mode"></param>
				/// <param name="frequency"></param>
				/// <param name="damping"></param>
				/// <param name="limit"></param>
				/// <param name="target">For rotation axis in radians.</param>
				public void SetMotor( AxisEnum axis, int mode, float frequency, float damping, RangeF limit, float target )
				{
					PhysicsNative.JSixDOFConstraint_SetMotor( constraint, (int)axis, mode, frequency, damping, limit.Minimum, limit.Maximum, target );
				}

				//public void SetMotor( AxisEnum axis, bool enable, float targetVelocity, float maxForce, bool servo, float servoTarget )
				//{
				//	PhysicsNative.JConstraint_SixDOF_SetMotor( constraint, (int)axis, enable, targetVelocity, maxForce, servo, servoTarget );
				//}
			}

			/////////////////////

			[StructLayout( LayoutKind.Sequential, Pack = 1 )]
			public struct VehicleWheelSettings
			{
				public Vector3F Position;
				public Vector3F Direction;
				public float SuspensionMinLength;// = 0.3f;
				public float SuspensionMaxLength;// = 0.5f;
				public float SuspensionPreloadLength;// = 0.0f;
				public float SuspensionFrequency;// = 1.5f;
				public float SuspensionDamping;// = 0.5f;
				public float Radius;
				public float Width;

				//Wheeled specific
				public float Inertia;// = 0.9f;
				public float AngularDamping;// = 0.2f;
				public float MaxSteerAngle;// = DegreesToRadians(70.0f);
				public int LongitudinalFrictionCount;
				public unsafe float* LongitudinalFrictionData;
				public int LateralFrictionCount;
				public unsafe float* LateralFrictionData;
				public float MaxBrakeTorque;// = 1500.0f;
				public float MaxHandBrakeTorque;// = 4000.0f;

				//Tracked specific
				//!!!!
			}

			/////////////////////

			[StructLayout( LayoutKind.Sequential, Pack = 1 )]
			public struct VehicleWheelData
			{
				public float Position;//SuspensionLength;
				public float SteerAngle;
				public float RotationAngle;
				public float AngularVelocity;
				public uint ContactBody;

				//public Vector3F Position;
				//public QuaternionF Rotation;
			}

			/////////////////////

			public class VehicleConstraint : Constraint
			{
				internal Body body;
				//bool stepListenerMustBeAdded;

				//

				public Body Body { get { return body; } }

				public void SetDriverInput( float forward, /*float left, */float right, float brake, float handBrake, bool activateBody )
				{
					PhysicsNative.JVehicleConstraint_SetDriverInput( constraint, forward, /*left, */right, brake, handBrake, activateBody );
				}

				//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				//public void SetStepListenerMustBeAdded( bool value )
				//{
				//	if( stepListenerMustBeAdded != value )
				//	{
				//		stepListenerMustBeAdded = value;
				//		PhysicsNative.JVehicleConstraint_SetStepListenerMustBeAdded( constraint, value );
				//	}
				//}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public unsafe void GetData( VehicleWheelData* wheelsData, out bool active, out int currentGear, out bool isSwitchingGear, out float currentRPM )
				{
					PhysicsNative.JVehicleConstraint_GetData( constraint, wheelsData, out active, out currentGear, out isSwitchingGear, out currentRPM );
				}

				//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				//public unsafe void GetData2( out Vector3 wheel0, out Vector3 wheel1, out Vector3 wheel2, out Vector3 wheel3 )
				//{
				//	PhysicsNative.JVehicleConstraint_GetData2( constraint, out wheel0, out wheel1, out wheel2, out wheel3 );
				//}
			}

			/////////////////////

			//public class MeshShapeCacheItem
			//{
			//	//parameters
			//	public Vector3 LocalScaling;
			//	public Vector3F[] SourceVertices;
			//	public int[] SourceIndices;
			//	public bool CheckValidData;
			//	public bool MergeEqualVerticesRemoveInvalidTriangles;
			//	public bool MakeConvex;

			//	public Internal.BulletSharp.CollisionShape Shape;
			//	public Vector3F[] ProcessedVertices;
			//	public int[] ProcessedIndices;
			//	public int[] ProcessedTrianglesToSourceIndex;

			//	public int Counter;

			//	//

			//	public bool Equal( ref Vector3 localScaling, Vector3F[] sourceVertices, int[] sourceIndices, bool checkValidData, bool mergeEqualVerticesRemoveInvalidTriangles, bool makeConvex )
			//	{
			//		if( CheckValidData != checkValidData )
			//			return false;
			//		if( MergeEqualVerticesRemoveInvalidTriangles != mergeEqualVerticesRemoveInvalidTriangles )
			//			return false;
			//		if( MakeConvex != makeConvex )
			//			return false;

			//		if( SourceVertices.Length != sourceVertices.Length )
			//			return false;
			//		if( SourceIndices.Length != sourceIndices.Length )
			//			return false;

			//		//!!!!need solve it in the bullet
			//		//discrete scaling
			//		if( ( LocalScaling * 100 ).ToVector3I() != ( localScaling * 100 ).ToVector3I() )
			//			return false;
			//		//if( LocalScaling != localScaling )
			//		//	return false;

			//		if( !ReferenceEquals( SourceVertices, sourceVertices ) && !SourceVertices.SequenceEqual( sourceVertices ) )
			//			return false;
			//		if( !ReferenceEquals( SourceIndices, sourceIndices ) && !SourceIndices.SequenceEqual( sourceIndices ) )
			//			return false;

			//		return true;
			//	}
			//}

			////////////

			//!!!!
			//public ShapeItem AllocateNativeShape( List<(CollisionShape, Matrix4)> componentShapes )
			//{

			//	//!!!!всегда добавлять в кеш?

			//	//!!!!кеш


			//	//!!!!mutable
			//	var nativeShape = PhysicsNative.JCreateShape( false );

			//	foreach( var tuple in componentShapes )
			//	{
			//		var componentShape = tuple.Item1;
			//		var transformShape = tuple.Item2;

			//		if( !transformShape.Decompose( out Vector3 pos, out Quaternion rot, out Vector3 scl ) )
			//		//if( !transformShape.Decompose( out Vector3 pos, out Matrix3 rot, out Vector3 scl ) )
			//		{
			//			//!!!!
			//		}

			//		var pos2 = pos.ToVector3F();
			//		var rot2 = rot.ToQuaternionF();

			//		//!!!!как сместится с учетом relative offset?

			//		var localScaling = scl.ToVector3F();


			//		//!!!!
			//		//if( MotionType.Value == MotionTypeEnum.Dynamic )
			//		//	tr *= centerOfMassOffsetInverted;

			//		componentShape.CreateShape( nativeShape, ref pos2, ref rot2, ref localScaling );


			//		//PhysicsNative.JCompoundShapeSettings_AddBox( compoundShapeSettings, zzz, zz, zz );


			//		//(var shape, var meshShapeCacheItem) = componentShape.CreateShape( physicsWorldData, ref scl );
			//		//if( shape != null )
			//		//{
			//		//	//!!!!what to do with scaling and big mesh cache
			//		//	shape.LocalScaling = BulletPhysicsUtility.Convert( scl );
			//		//	//shape.UserObject = componentShape;

			//		//	if( meshShapeCacheItem != null )
			//		//	{
			//		//		if( meshShapeCacheItemsToFree == null )
			//		//			meshShapeCacheItemsToFree = new List<Scene.PhysicsWorldDataClass.MeshShapeCacheItem>();
			//		//		meshShapeCacheItemsToFree.Add( meshShapeCacheItem );
			//		//	}
			//		//	else
			//		//	{
			//		//		if( collisionShapesToDispose == null )
			//		//			collisionShapesToDispose = new List<Internal.BulletSharp.CollisionShape>();
			//		//		collisionShapesToDispose.Add( shape );
			//		//	}

			//		//	//!!!!
			//		//	//var tr = new Matrix4( rot * Matrix3.FromScale( scl ), pos );
			//		//	var tr = new Matrix4( rot, pos );
			//		//	if( MotionType.Value == MotionTypeEnum.Dynamic )
			//		//		tr *= centerOfMassOffsetInverted;

			//		//	compoundShape.AddChildShape( BulletPhysicsUtility.Convert( tr ), shape );
			//		//}
			//	}

			//	if( !PhysicsNative.JShape_IsValid( nativeShape ) )
			//	{
			//		PhysicsNative.JDestroyShape( nativeShape );
			//		return null;
			//	}

			//	var item = new ShapeItem();
			//	item.physicsWorld = this;
			//	item.nativeShape = nativeShape;
			//	nativeShapes.Add( item );

			//	item.referenceCounter++;
			//	return item;
			//}

			//static void GetComponentShapesRecursive( CollisionShape shape, Matrix4 shapeTransform, List<(CollisionShape, Matrix4)> result )
			//{
			//	result.Add( (shape, shapeTransform) );

			//	//foreach( var child in shape.GetComponents<CollisionShape>( false, false, true ) )
			//	//{
			//	//	var childTransform = shapeTransform * child.TransformRelativeToParent.Value.ToMatrix4();
			//	//	//var childTransform = child.LocalTransform.Value.ToMat4() * shapeTransform;
			//	//	GetComponentShapesRecursive( child, childTransform, result );
			//	//}
			//}

			//!!!!maybe bool allowCaching
			public Shape AllocateShape( RigidBody shapeData, Vector3 scale )
			{
				var scaleKey = ( scale * 100 ).ToVector3I();

				var key = new StringBuilder( 256 );
				key.Append( shapeData.MotionType.Value.ToString() );
				if( shapeData.MotionType.Value != PhysicsMotionType.Static )
				{
					key.Append( ' ' );
					key.Append( shapeData.Mass.Value );
					key.Append( ' ' );
					key.Append( shapeData.CenterOfMassOffset.Value.ToString() );
					//key.Append( shapeData.CenterOfMassManual.Value.ToString() );
					//if( shapeData.CenterOfMassManual )
					//{
					//	key.Append( ' ' );
					//	key.Append( shapeData.CenterOfMassPosition.Value.ToString() );
					//}
				}
				key.Append( ' ' );
				key.Append( scaleKey.X );
				key.Append( ' ' );
				key.Append( scaleKey.Y );
				key.Append( ' ' );
				key.Append( scaleKey.Z );

				var shapeComponents = shapeData.GetComponents<CollisionShape>();

				foreach( var child in shapeComponents )
				{
					if( child.Enabled )
					{
						//key.Append( ' ' );
						child.GetShapeKey( key );

						//key.Append( ' ' );
						//var relativeTransform = child.LocalTransform.Value;
						//if( !relativeTransform.IsIdentity )
						//	key.Append( relativeTransform.ToString() );
						//else
						//	key.Append( '1' );
					}
				}

				var keyString = key.ToString();

				if( !nativeShapes.TryGetValue( keyString, out var item ) )
				{
					var bodyTransformScaleMatrix = Matrix3.FromScale( scale ).ToMatrix4();
					//var bodyTransform = Transform.Value;
					//var bodyTransformScaleMatrix = Matrix3.FromScale( bodyTransform.Scale ).ToMatrix4();

					//get shapes. calculate local transforms with applied body scaling.

					var componentShapes = new List<(CollisionShape, Matrix4)>( shapeComponents.Length );
					foreach( var child in shapeComponents )
					{
						if( child.Enabled )
							componentShapes.Add( (child, bodyTransformScaleMatrix * child.LocalTransform.Value.ToMatrix4()) );
					}

					//var componentShapes = new List<(CollisionShape, Matrix4)>();
					//foreach( var child in shapeData.GetComponents<CollisionShape>( false, false, true ) )
					//	GetComponentShapesRecursive( child, bodyTransformScaleMatrix * child.TransformRelativeToParent.Value.ToMatrix4(), componentShapes );

					if( componentShapes.Count == 0 )
						return null;

					//!!!!mutable
					var nativeShape = PhysicsNative.JCreateShape( false );

					var collisionShapes = new Shape.CollisionShapeData[ componentShapes.Count ];

					for( int n = 0; n < componentShapes.Count; n++ )//foreach( var tuple in componentShapes )
					{
						var tuple = componentShapes[ n ];
						ref var collisionShapeData = ref collisionShapes[ n ];

						var componentShape = tuple.Item1;
						var transformShape = tuple.Item2;

						if( !transformShape.Decompose( out Vector3 pos, out Quaternion rot, out Vector3 scl ) )
						//if( !transformShape.Decompose( out Vector3 pos, out Matrix3 rot, out Vector3 scl ) )
						{
							//!!!!
						}

						var pos2 = pos.ToVector3F();
						var rot2 = rot.ToQuaternionF();

						//!!!!как сместится с учетом relative offset?

						var localScaling = scl.ToVector3F();


						//!!!!
						//if( MotionType.Value == MotionTypeEnum.Dynamic )
						//	tr *= centerOfMassOffsetInverted;

						componentShape.CreateShape( Scene, nativeShape, ref pos2, ref rot2, ref localScaling, ref collisionShapeData );


						//PhysicsNative.JCompoundShapeSettings_AddBox( compoundShapeSettings, zzz, zz, zz );


						//(var shape, var meshShapeCacheItem) = componentShape.CreateShape( physicsWorldData, ref scl );
						//if( shape != null )
						//{
						//	//!!!!what to do with scaling and big mesh cache
						//	shape.LocalScaling = BulletPhysicsUtility.Convert( scl );
						//	//shape.UserObject = componentShape;

						//	if( meshShapeCacheItem != null )
						//	{
						//		if( meshShapeCacheItemsToFree == null )
						//			meshShapeCacheItemsToFree = new List<Scene.PhysicsWorldDataClass.MeshShapeCacheItem>();
						//		meshShapeCacheItemsToFree.Add( meshShapeCacheItem );
						//	}
						//	else
						//	{
						//		if( collisionShapesToDispose == null )
						//			collisionShapesToDispose = new List<Internal.BulletSharp.CollisionShape>();
						//		collisionShapesToDispose.Add( shape );
						//	}

						//	//!!!!
						//	//var tr = new Matrix4( rot * Matrix3.FromScale( scl ), pos );
						//	var tr = new Matrix4( rot, pos );
						//	if( MotionType.Value == MotionTypeEnum.Dynamic )
						//		tr *= centerOfMassOffsetInverted;

						//	compoundShape.AddChildShape( BulletPhysicsUtility.Convert( tr ), shape );
						//}
					}

					if( !PhysicsNative.JShape_IsValid( nativeShape ) )
					{
						PhysicsNative.JDestroyShape( nativeShape );
						return null;
					}

					item = new Shape();
					item.physicsWorld = this;
					item.sourceData = shapeData;
					item.key = keyString;
					item.nativeShape = nativeShape;
					item.collisionShapes = collisionShapes;
					item.scale = scale.ToVector3F();
					nativeShapes[ keyString ] = item;
				}

				item.referenceCounter++;
				return item;
			}

			public void FreeShape( Shape item )
			{
				item.referenceCounter--;
				if( item.referenceCounter <= 0 )
				{
					nativeShapes.Remove( item.key );

					if( item.nativeShape != IntPtr.Zero )
					{
						PhysicsNative.JDestroyShape( item.nativeShape );
						item.nativeShape = IntPtr.Zero;
					}
				}
			}

			public Body CreateRigidBody( Shape shape, bool shapeAutoDestroy, object owner, PhysicsMotionType motionType, float linearDamping, float angularDamping, ref Vector3 position, ref QuaternionF rotation, [MarshalAs( UnmanagedType.U1 )] bool activate, float mass, ref Vector3F centerOfMassOffset, /*[MarshalAs( UnmanagedType.U1 )] bool centerOfMassManual, ref Vector3F centerOfMassPosition, ref Vector3F inertiaTensorFactor, */PhysicsMotionQuality motionQuality, bool characterMode )
			{
				var item = new Body();
				item.physicsWorld = this;
				//item.scale = scale;
				item.shape = shape;
				item.shapeAutoDestroy = shapeAutoDestroy;
				item.owner = owner;

				item.body = PhysicsNative.JCreateBody( physicsSystem, shape.nativeShape, (int)motionType, linearDamping, angularDamping, ref position, ref rotation, activate, mass, ref centerOfMassOffset, /*centerOfMassManual, ref centerOfMassPosition, ref inertiaTensorFactor, */ref item.bodyId, (int)motionQuality, characterMode );

				if( item.body == IntPtr.Zero )
				{
					//max body count limit. it can be configured in scene settings from the editor
					Log.Warning( "Scene: PhysicsWorld: CreateRigidBody: Max body count reached. The limit can be configured in the scene settings by means PhysicsMaxBodies property." );
					return null;
				}

				item.motionType = motionType;
				item.motionQuality = motionQuality;
				item.active = motionType == PhysicsMotionType.Dynamic ? activate : false;
				item.position = position;
				item.rotation = rotation;
				item.characterMode = characterMode;

				allRigidBodies[ item.bodyId ] = item;
				if( item.motionType != PhysicsMotionType.Static )
					kinematicDynamicRigidBodies[ item.bodyId ] = item;

				return item;
			}

			public Body CreateRigidBodyStatic( Shape shape, bool shapeAutoDestroy, object owner, ref Vector3 position, ref QuaternionF rotation )
			{
				var centerOfMassOffset = Vector3F.Zero;
				//var centerOfMassPosition = Vector3F.Zero;
				//var inertiaTensorFactor = Vector3F.One;
				var item = CreateRigidBody( shape, shapeAutoDestroy, owner, PhysicsMotionType.Static, 0.1f, 0.1f, ref position, ref rotation, false, 0, ref centerOfMassOffset, /*false, ref centerOfMassPosition, ref inertiaTensorFactor, */PhysicsMotionQuality.Discrete, false );

				return item;
			}

			public Body CreateRigidBodyStatic( Shape shape, bool shapeAutoDestroy, object owner, Vector3 position, QuaternionF rotation )
			{
				return CreateRigidBodyStatic( shape, shapeAutoDestroy, owner, ref position, ref rotation );
			}

			public void DestroyRigidBody( Body item )
			{
				if( item.disposed )
					return;

				//destroy linked constraints
				if( item.linkedConstraints != null )
				{
					while( item.linkedConstraints.Count != 0 )
					{
						var c = item.linkedConstraints[ item.linkedConstraints.Count - 1 ];
						c.Dispose();
					}
				}

				allRigidBodies.Remove( item.bodyId );
				if( item.motionType != PhysicsMotionType.Static )
					kinematicDynamicRigidBodies.Remove( item.bodyId );

				PhysicsNative.JDestroyBody( physicsSystem, item.body );
				item.disposed = true;

				if( item.shapeAutoDestroy )
					FreeShape( item.shape );
			}

			void TwoBodyConstraintPostCreate( TwoBodyConstraint constraint, ref Vector3 position, ref QuaternionF rotation )
			{
				if( constraint.bodyA != null )
				{
					if( constraint.bodyA.linkedConstraints == null )
						constraint.bodyA.linkedConstraints = new List<Constraint>();
					constraint.bodyA.linkedConstraints.Add( constraint );
				}
				if( constraint.bodyB != null )
				{
					if( constraint.bodyB.linkedConstraints == null )
						constraint.bodyB.linkedConstraints = new List<Constraint>();
					constraint.bodyB.linkedConstraints.Add( constraint );
				}

				allConstraints.Add( constraint );
				twoBodyConstraints.Add( constraint );

				//!!!!slowly

				var t = new Matrix4( rotation.ToMatrix3(), position );
				//var transform = Transform.Value;
				//var t = new Matrix4( transform.Rotation.ToMatrix3(), transform.Position );

				if( constraint.bodyA != null )
				{
					var bodyATransform = new Matrix4( constraint.bodyA.Rotation.ToMatrix3(), constraint.bodyA.Position );
					constraint.constraintAFrame = bodyATransform.GetInverse() * t;

					//var bodyATransformFull = creationBodyA.Transform.Value;
					//var bodyATransform = new Matrix4( bodyATransformFull.Rotation.ToMatrix3(), bodyATransformFull.Position );
					//var bodyBTransformFull = creationBodyB.Transform.Value;
					//var bodyBTransform = new Matrix4( bodyBTransformFull.Rotation.ToMatrix3(), bodyBTransformFull.Position );
					//item.constraintAFrame = bodyATransform.GetInverse() * t;
					//BMatrix matA = BulletPhysicsUtility.Convert( constraintAFrame );
					//BMatrix matB = BulletPhysicsUtility.Convert( bodyBTransform.GetInverse() * t );

					////constraintAFrame = bodyA.Transform.Value.ToMat4().GetInverse() * t;
					////Matrix matA = BulletUtils.Convert( constraintAFrame );
					////Matrix matB = BulletUtils.Convert( bodyB.Transform.Value.ToMat4().GetInverse() * t );

					//constraintRigid = new Generic6DofSpring2Constraint( (Internal.BulletSharp.RigidBody)creationBodyA.BulletBody, (Internal.BulletSharp.RigidBody)creationBodyB.BulletBody, matA, matB );

				}
			}

			public SixDOFConstraint CreateConstraintSixDOF( object owner, Body bodyA, Body bodyB, bool transformInWorldSpace, ref Vector3 positionA, ref QuaternionF rotationA, ref Vector3 positionB, ref QuaternionF rotationB, ref Vector3F visualScale, PhysicsAxisMode linearAxisX, ref RangeF linearLimitX, PhysicsAxisMode linearAxisY, ref RangeF linearLimitY, PhysicsAxisMode linearAxisZ, ref RangeF linearLimitZ, PhysicsAxisMode angularAxisX, ref RangeF angularLimitX, PhysicsAxisMode angularAxisY, ref RangeF angularLimitY, PhysicsAxisMode angularAxisZ, ref RangeF angularLimitZ, float linearXFriction, float linearYFriction, float linearZFriction, float angularXFriction, float angularYFriction, float angularZFriction )
			{
				var item = new SixDOFConstraint();
				item.physicsWorld = this;
				item.owner = owner;
				item.bodyA = bodyA;
				item.bodyB = bodyB;
				item.visualScale = visualScale;

				var axisXA = rotationA.GetForward();
				var axisYA = rotationA.GetLeft();
				var axisXB = rotationB.GetForward();
				var axisYB = rotationB.GetLeft();

				////var axisX = rotation.GetForward();
				////var axisY = rotation.GetRight();
				////var axisX = rotation.GetForward();
				////var axisY = rotation.GetUp();

				item.constraint = PhysicsNative.JCreateConstraintSixDOF( physicsSystem, bodyA.body, bodyB.body, transformInWorldSpace, ref positionA, ref axisXA, ref axisYA, ref positionB, ref axisXB, ref axisYB, linearAxisX, ref linearLimitX, linearAxisY, ref linearLimitY, linearAxisZ, ref linearLimitZ, angularAxisX, ref angularLimitX, angularAxisY, ref angularLimitY, angularAxisZ, ref angularLimitZ, linearXFriction, linearYFriction, linearZFriction, angularXFriction, angularYFriction, angularZFriction );

				//!!!!может такое быть? везде так
				//if( item.constraint == IntPtr.Zero )
				//{
				//	//max body count limit. it can be configured in scene settings from the editor
				//	Log.Warning( "Scene: PhysicsWorld: CreateRigidBody: Max body count reached. The limit can be configured in the scene settings by means PhysicsMaxBodies property." );
				//	return null;
				//}

				//!!!!why using A? it is ok?
				TwoBodyConstraintPostCreate( item, ref positionA, ref rotationA );

				return item;
			}

			//public SixDOFConstraint CreateConstraintSixDOF( object owner, Body bodyA, Body bodyB, ref Vector3 position, ref QuaternionF rotation, ref Vector3F visualScale, ref RangeF linearLimitX, ref RangeF linearLimitY, ref RangeF linearLimitZ, ref RangeF angularLimitX, ref RangeF angularLimitY, ref RangeF angularLimitZ )
			//{
			//	var item = new SixDOFConstraint();
			//	item.physicsWorld = this;
			//	item.owner = owner;
			//	item.bodyA = bodyA;
			//	item.bodyB = bodyB;
			//	item.visualScale = visualScale;

			//	//!!!!
			//	var axisX = rotation.GetForward();
			//	var axisY = rotation.GetRight();
			//	//var axisX = rotation.GetForward();
			//	//var axisY = rotation.GetRight();
			//	//var axisX = rotation.GetForward();
			//	//var axisY = rotation.GetUp();

			//	item.constraint = PhysicsNative.JCreateConstraintSixDOF( physicsSystem, bodyA.body, bodyB.body, ref position, ref axisX, ref axisY, ref linearLimitX, ref linearLimitY, ref linearLimitZ, ref angularLimitX, ref angularLimitY, ref angularLimitZ );

			//	//!!!!может такое быть? везде так
			//	//if( item.constraint == IntPtr.Zero )
			//	//{
			//	//	//max body count limit. it can be configured in scene settings from the editor
			//	//	Log.Warning( "Scene: PhysicsWorld: CreateRigidBody: Max body count reached. The limit can be configured in the scene settings by means PhysicsMaxBodies property." );
			//	//	return null;
			//	//}

			//	ConstraintPostCreate( item, ref position, ref rotation );

			//	return item;
			//}

			public FixedConstraint CreateConstraintFixed( object owner, Body bodyA, Body bodyB, bool transformInWorldSpace, ref Vector3 positionA, ref QuaternionF rotationA, ref Vector3 positionB, ref QuaternionF rotationB, ref Vector3F visualScale )
			{
				var item = new FixedConstraint();
				item.physicsWorld = this;
				item.owner = owner;
				item.bodyA = bodyA;
				item.bodyB = bodyB;
				item.visualScale = visualScale;

				//!!!!

				var axisXA = rotationA.GetForward();
				var axisYA = rotationA.GetLeft();
				var axisXB = rotationB.GetForward();
				var axisYB = rotationB.GetLeft();

				//var axisXA = rotationA.GetForward();
				//var axisYA = rotationA.GetRight();
				//var axisXB = rotationB.GetForward();
				//var axisYB = rotationB.GetRight();

				item.constraint = PhysicsNative.JCreateConstraintFixed( physicsSystem, bodyA.body, bodyB.body, transformInWorldSpace, ref positionA, ref axisXA, ref axisYA, ref positionB, ref axisXB, ref axisYB );

				TwoBodyConstraintPostCreate( item, ref positionA, ref rotationA );

				return item;
			}

			//public FixedConstraint CreateConstraintFixed( object owner, Body bodyA, Body bodyB, ref Vector3 position, ref QuaternionF rotation, ref Vector3F visualScale )
			////public FixedConstraint CreateConstraintFixed( object owner, Body bodyA, Body bodyB, ref Vector3 position, ref Vector3F axisX, ref Vector3F axisY )
			//{
			//	var item = new FixedConstraint();
			//	item.physicsWorld = this;
			//	item.owner = owner;
			//	item.bodyA = bodyA;
			//	item.bodyB = bodyB;
			//	item.visualScale = visualScale;

			//	var axisX = rotation.GetForward();
			//	var axisY = rotation.GetUp();
			//	//var axisY = transform.Rotation.GetLeft().ToVector3F();
			//	//var axisY = transform.Rotation.GetRight().ToVector3F();

			//	item.constraint = PhysicsNative.JCreateConstraintFixed( physicsSystem, bodyA.body, bodyB.body, ref position, ref axisX, ref axisY );

			//	ConstraintPostCreate( item, ref position, ref rotation );

			//	return item;
			//}

			public unsafe VehicleConstraint CreateConstraintVehicle( object owner, Body body, int wheelCount, VehicleWheelSettings* wheelsSettings, ref Vector3F visualScale, float frontWheelAntiRollBarStiffness, float rearWheelAntiRollBarStiffness, float maxPitchRollAngle, float engineMaxTorque, float engineMinRPM, float engineMaxRPM, [MarshalAs( UnmanagedType.U1 )] bool transmissionAuto, int transmissionGearRatiosCount, double* transmissionGearRatios, int transmissionReverseGearRatiosCount, double* transmissionReverseGearRatios, float transmissionSwitchTime, float transmissionClutchReleaseTime, float transmissionSwitchLatency, float transmissionShiftUpRPM, float transmissionShiftDownRPM, float transmissionClutchStrength, bool frontWheelDrive, bool rearWheelDrive, float frontWheelDifferentialRatio, float frontWheelDifferentialLeftRightSplit, float frontWheelDifferentialLimitedSlipRatio, float frontWheelDifferentialEngineTorqueRatio, float rearWheelDifferentialRatio, float rearWheelDifferentialLeftRightSplit, float rearWheelDifferentialLimitedSlipRatio, float rearWheelDifferentialEngineTorqueRatio, RadianF maxSlopeAngle )
			{
				var constraint = new VehicleConstraint();
				constraint.physicsWorld = this;
				constraint.owner = owner;
				constraint.body = body;
				constraint.visualScale = visualScale;

				constraint.constraint = PhysicsNative.JCreateConstraintVehicle( physicsSystem, body.body, wheelCount, wheelsSettings, frontWheelAntiRollBarStiffness, rearWheelAntiRollBarStiffness, maxPitchRollAngle, engineMaxTorque, engineMinRPM, engineMaxRPM, transmissionAuto, transmissionGearRatiosCount, transmissionGearRatios, transmissionReverseGearRatiosCount, transmissionReverseGearRatios, transmissionSwitchTime, transmissionClutchReleaseTime, transmissionSwitchLatency, transmissionShiftUpRPM, transmissionShiftDownRPM, transmissionClutchStrength, frontWheelDrive, rearWheelDrive, frontWheelDifferentialRatio, frontWheelDifferentialLeftRightSplit, frontWheelDifferentialLimitedSlipRatio, frontWheelDifferentialEngineTorqueRatio, rearWheelDifferentialRatio, rearWheelDifferentialLeftRightSplit, rearWheelDifferentialLimitedSlipRatio, rearWheelDifferentialEngineTorqueRatio, maxSlopeAngle );

				if( constraint.body != null )
				{
					if( constraint.body.linkedConstraints == null )
						constraint.body.linkedConstraints = new List<Constraint>();
					constraint.body.linkedConstraints.Add( constraint );
				}

				allConstraints.Add( constraint );

				return constraint;
			}

			public void DestroyConstraint( Constraint item )
			{
				if( item.disposed )
					return;

				var twoBody = item as TwoBodyConstraint;
				if( twoBody != null )
				{
					twoBody.bodyA?.linkedConstraints.Remove( item );
					twoBody.bodyB?.linkedConstraints.Remove( item );

					twoBodyConstraints.Remove( twoBody );
				}
				else
				{
					var vehicleConstraint = item as VehicleConstraint;
					if( vehicleConstraint != null )
						vehicleConstraint.body?.linkedConstraints.Remove( item );
				}

				allConstraints.Remove( item );

				PhysicsNative.JDestroyConstraint( physicsSystem, item.constraint );
				item.disposed = true;

				item.PerformDisposedEvent();
			}


			//MeshShapeCacheItem FindShapeInCache( ref Vector3 localScaling, Vector3F[] sourceVertices, int[] sourceIndices, bool checkValidData, bool mergeEqualVerticesRemoveInvalidTriangles, bool makeConvex )
			//{
			//	//!!!!dictionary? but before need to remove localScaling
			//	foreach( var item in meshShapeCacheItems )
			//	{
			//		if( item.Equal( ref localScaling, sourceVertices, sourceIndices, checkValidData, mergeEqualVerticesRemoveInvalidTriangles, makeConvex ) )
			//		{
			//			return item;
			//		}
			//	}
			//	return null;
			//}

			//public MeshShapeCacheItem AllocateShapeInCache( ref Vector3 localScaling, Vector3F[] sourceVertices, int[] sourceIndices, bool checkValidData, bool mergeEqualVerticesRemoveInvalidTriangles, bool makeConvex )
			//{
			//	var epsilon = 0.0001f;

			//	//find in the cache
			//	var result = FindShapeInCache( ref localScaling, sourceVertices, sourceIndices, checkValidData, mergeEqualVerticesRemoveInvalidTriangles, makeConvex );

			//	//create a new item
			//	if( result == null )
			//	{
			//		//check valid data
			//		if( checkValidData && !MathAlgorithms.CheckValidVertexIndexBuffer( sourceVertices.Length, sourceIndices, false ) )
			//		{
			//			Log.Info( "CollisionShape_Mesh: CreateShape: Invalid source data." );
			//			return null;
			//		}

			//		Vector3F[] processedVertices;
			//		int[] processedIndices;
			//		int[] processedTrianglesToSourceIndex;

			//		//process geometry
			//		if( mergeEqualVerticesRemoveInvalidTriangles )
			//		{
			//			//!!!!slowly. later use cached precalculated bullet shape.
			//			MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( sourceVertices, sourceIndices, epsilon, epsilon, true, true, out processedVertices, out processedIndices, out processedTrianglesToSourceIndex );
			//		}
			//		else
			//		{
			//			processedVertices = sourceVertices;
			//			processedIndices = sourceIndices;
			//			processedTrianglesToSourceIndex = null;
			//		}

			//		//create bullet shape

			//		Internal.BulletSharp.CollisionShape shape;

			//		if( makeConvex )
			//		{
			//			if( MathAlgorithms.IsPlaneMesh( processedVertices, processedIndices, epsilon ) )
			//			{
			//				Log.Info( "CollisionShape_Mesh: CreateShape: Unable to create shape as convex hull. All vertices on the one plane." );
			//				return null;
			//			}


			//			//!!!!тут иначе? возможно лучше получить результирующие processed данные из буллета. как получить processedTrianglesToSourceIndex - это вопрос. возможно ли?
			//			//если нельзя то processedTrianglesToSourceIndex = new int[ 0 ]; - что означает нельзя сконвертировать.
			//			//если processedTrianglesToSourceIndex == null, то конвертация 1:1.

			//			try
			//			{
			//				//!!!!может оптимизирует плохо

			//				MathAlgorithms.ConvexHullFromMesh( CollectionUtility.ToVector3( processedVertices ), processedIndices, out var processedVertices2, out processedIndices );
			//				processedVertices = CollectionUtility.ToVector3F( processedVertices2 );


			//				//!!!!вершины потом как отдаются? зачем индексы?


			//				//!!!!
			//				//for( int nTriangle = 0; nTriangle < processedIndices.Length / 3; nTriangle++ )
			//				//{
			//				//	var q = processedIndices[ nTriangle * 3 + 1 ];
			//				//	processedIndices[ nTriangle * 3 + 1 ] = processedIndices[ nTriangle * 3 + 2 ];
			//				//	processedIndices[ nTriangle * 3 + 2 ] = q;
			//				//}


			//				//var convex = ConvexHullAlgorithm.Create( processedVertices, processedIndices );

			//				//var vlist = new List<Vec3F>( convex.Faces.Length * 3 );
			//				//foreach( var f in convex.Faces )
			//				//	for( int v = 0; v < f.Vertices.Length; v++ )
			//				//		vlist.Add( f.Vertices[ v ].ToVec3F() );

			//				//processedVertices = vlist.ToArray();
			//				//processedIndices = null;

			//				//BulletUtils.GetHullVertices( processedVertices.ToVec3Array(), processedIndices, out var processedVertices2, out processedIndices );
			//				//processedVertices = processedVertices2.ToVec3FArray();
			//				//BulletUtils.GetHullVertices( processedVertices, processedIndices, out processedVertices, out processedIndices );

			//				//если нельзя то processedTrianglesToSourceIndex = new int[ 0 ]; - что означает нельзя сконвертировать.
			//				processedTrianglesToSourceIndex = Array.Empty<int>();
			//			}
			//			catch( Exception e )
			//			{
			//				Log.Info( "CollisionShape_Mesh: CreateShape: Unable to create shape as convex hull. " + e.Message );
			//				return null;
			//			}


			//			//!!!!
			//			//var processedVerticesBullet = BulletPhysicsUtility.Convert( sourceVertices );
			//			var processedVerticesBullet = BulletPhysicsUtility.Convert( processedVertices );



			//			//!!!!
			//			var hullShape = new ConvexHullShape( processedVerticesBullet );

			//			//!!!!test
			//			//var hullShape = new ConvexHullShape( processedVerticesBullet, Math.Min( processedVerticesBullet.Length, 64 ) );

			//			shape = hullShape;


			//			//!!!!new
			//			//hullShape.OptimizeConvexHull();

			//		}
			//		else
			//		{
			//			//!!!проверки на ошибки данных

			//			//!!!!can create without making of Vector3[] array. IntPtr constructor? internally the memory will copied?
			//			var indexVertexArrays = new TriangleIndexVertexArray( processedIndices, BulletPhysicsUtility.Convert( processedVertices ) );

			//			//indexVertexArrays = new TriangleIndexVertexArray();
			//			//var indexedMesh = new IndexedMesh();
			//			//indexedMesh.Allocate( totalTriangles, totalVerts, triangleIndexStride, vertexStride );
			//			//indexedMesh SetData( ICollection<int> triangles, ICollection<Vector3> vertices );
			//			//indexVertexArrays.AddIndexedMesh( indexedMesh );


			//			//It is better to use "useQuantizedAabbCompression=true", because it makes the tree data structure 4 times smaller: sizeof( btOptimizedBvhNode ) = 64 and sizeof( btQuantizedBvhNode ) = 16 bytes.Note that the number of AABB tree nodes is twice the number of triangles.

			//			//Instead of creating the tree on the XBox 360 console, it is better to deserialize it directly from disk to memory. See btOptimizedBvh::deSerializeInPlace in Demos/ConcaveDemo/ConcavePhysicsDemo.cpp 

			//			//без useQuantizedAabbCompression в три раза быстрее создается

			//			//!!!!enable when cache support
			//			bool useQuantizedAabbCompression = false;
			//			//bool useQuantizedAabbCompression = true;
			//			bool buildBvh = true;

			//			//!!!!в другом конструкторе можно еще указать какие-то bound min max
			//			//public BvhTriangleMeshShape( StridingMeshInterface meshInterface, bool useQuantizedAabbCompression, Vector3 bvhAabbMin, Vector3 bvhAabbMax, bool buildBvh = true );

			//			shape = new BvhTriangleMeshShape( indexVertexArrays, useQuantizedAabbCompression, buildBvh );
			//		}

			//		result = new MeshShapeCacheItem();
			//		result.LocalScaling = localScaling;
			//		result.SourceVertices = sourceVertices;
			//		result.SourceIndices = sourceIndices;
			//		result.CheckValidData = checkValidData;
			//		result.MergeEqualVerticesRemoveInvalidTriangles = mergeEqualVerticesRemoveInvalidTriangles;
			//		result.MakeConvex = makeConvex;

			//		result.Shape = shape;
			//		result.ProcessedVertices = processedVertices;
			//		result.ProcessedIndices = processedIndices;
			//		result.ProcessedTrianglesToSourceIndex = processedTrianglesToSourceIndex;

			//		meshShapeCacheItems.Add( result );
			//	}

			//	if( result.Shape.IsDisposed )
			//		Log.Warning( "Scene.PhysicsWorldDataClass: AllocateShapeInCache: result.Shape.IsDisposed" );

			//	result.Counter++;
			//	return result;
			//}

			//public void FreeShapeInCache( MeshShapeCacheItem item )
			//{
			//	item.Counter--;

			//	//!!!!может оставлять когда итем никому не нужен, чтобы потом заного не создавать. процедурные могут копиться
			//	if( item.Counter == 0 )
			//	{
			//		var shape = item.Shape;
			//		if( shape != null )
			//		{
			//			var meshShape = shape as BvhTriangleMeshShape;
			//			meshShape?.MeshInterface?.Dispose();
			//			shape.Dispose();
			//		}

			//		meshShapeCacheItems.Remove( item );
			//	}
			//}

			//public void ClearShapeCache()
			//{
			//	foreach( var item in meshShapeCacheItems )
			//	{
			//		var shape = item.Shape;
			//		if( shape != null )
			//		{
			//			var meshShape = shape as BvhTriangleMeshShape;
			//			meshShape?.MeshInterface?.Dispose();
			//			shape.Dispose();
			//		}
			//	}
			//	meshShapeCacheItems.Clear();
			//}

			public void GetStatistics( out int shapes, out int allBodies, out int kinematicDynamicBodies, out int activeBodies )
			{
				shapes = nativeShapes.Count;

				allBodies = allRigidBodies.Count;
				kinematicDynamicBodies = kinematicDynamicRigidBodies.Count;

				activeBodies = 0;
				foreach( var body in kinematicDynamicRigidBodies.Values )
				{
					if( body.Active )
						activeBodies++;
				}
			}

			public Body GetBodyById( uint id )
			{
				allRigidBodies.TryGetValue( id, out var body );
				return body;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//[StructLayout( LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode )]
		//struct CollectContactsItem
		//{
		//	public int simulationSubStep;
		//	public IntPtr bodyA;
		//	public IntPtr bodyB;
		//	public int numContacts;

		//	public Vector3 positionWorldOnA_0;
		//	public Vector3 positionWorldOnB_0;
		//	public float appliedImpulse_0;
		//	public float distance_0;

		//	public Vector3 positionWorldOnA_1;
		//	public Vector3 positionWorldOnB_1;
		//	public float appliedImpulse_1;
		//	public float distance_1;

		//	public Vector3 positionWorldOnA_2;
		//	public Vector3 positionWorldOnB_2;
		//	public float appliedImpulse_2;
		//	public float distance_2;

		//	public Vector3 positionWorldOnA_3;
		//	public Vector3 positionWorldOnB_3;
		//	public float appliedImpulse_3;
		//	public float distance_3;

		//	//

		//	public void GetContact( int index, out Vector3 positionWorldOnA, out Vector3 positionWorldOnB, out float appliedImpulse, out float distance )
		//	{
		//		switch( index )
		//		{
		//		case 0:
		//			positionWorldOnA = positionWorldOnA_0;
		//			positionWorldOnB = positionWorldOnB_0;
		//			appliedImpulse = appliedImpulse_0;
		//			distance = distance_0;
		//			break;

		//		case 1:
		//			positionWorldOnA = positionWorldOnA_1;
		//			positionWorldOnB = positionWorldOnB_1;
		//			appliedImpulse = appliedImpulse_1;
		//			distance = distance_1;
		//			break;

		//		case 2:
		//			positionWorldOnA = positionWorldOnA_2;
		//			positionWorldOnB = positionWorldOnB_2;
		//			appliedImpulse = appliedImpulse_2;
		//			distance = distance_2;
		//			break;

		//		default://case 3:
		//			positionWorldOnA = positionWorldOnA_3;
		//			positionWorldOnB = positionWorldOnB_3;
		//			appliedImpulse = appliedImpulse_3;
		//			distance = distance_3;
		//			break;
		//		}
		//	}
		//};

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[Browsable( false )]
		public PhysicsWorldClass PhysicsWorld
		{
			get { return physicsWorld; }
		}

		void PhysicsWorldCreate()
		{
			unsafe
			{
				if( sizeof( PhysicsNative.RayTestResult ) != 56 )
					Log.Fatal( "Scene: PhysicsWorldCreate: sizeof( PhysicsNative.RayTestResult ) != 56." );
				if( sizeof( PhysicsNative.VolumeTestResult ) != 12 )
					Log.Fatal( "Scene: PhysicsWorldCreate: sizeof( PhysicsNative.VolumeTestResult ) != 12." );
				if( sizeof( IntPtr ) == 8 )
				{
					if( sizeof( PhysicsWorldClass.VehicleWheelSettings ) != 22 * 4 + 8 )
						Log.Fatal( "Scene: PhysicsWorldCreate: sizeof( PhysicsWorldClass.VehicleWheelSettings ) != 22 * 4 + 8." );
				}
				else
				{
					if( sizeof( PhysicsWorldClass.VehicleWheelSettings ) != 22 * 4 )
						Log.Fatal( "Scene: PhysicsWorldCreate: sizeof( PhysicsWorldClass.VehicleWheelSettings ) != 22 * 4." );
				}
				if( sizeof( PhysicsWorldClass.VehicleWheelData ) != 5 * 4 )
					Log.Fatal( "Scene: PhysicsWorldCreate: sizeof( PhysicsWorldClass.VehicleWheelData ) != 5 * 4." );
				if( sizeof( PhysicsWorldClass.ContactItem ) != 224 )
					Log.Fatal( "Scene: PhysicsWorldCreate: sizeof( PhysicsWorldClass.ContactItem ) != 224." );
			}

			physicsWorld = new PhysicsWorldClass();
			physicsWorld.Scene = this;
			var data = physicsWorld;

			var maxBodyPairs = PhysicsAdvancedSettings ? PhysicsMaxBodyPairs.Value : 0;
			if( maxBodyPairs == 0 )
				maxBodyPairs = PhysicsMaxBodies;

			var maxContactConstraints = PhysicsAdvancedSettings ? PhysicsMaxContactConstraints.Value : 0;
			if( maxContactConstraints == 0 )
				maxContactConstraints = PhysicsMaxBodies / 10;

			data.physicsSystem = PhysicsNative.JCreateSystem( PhysicsMaxBodies, maxBodyPairs, maxContactConstraints );

			PhysicsUpdateGravity();
			UpdateAdvancedPhysicsSettings();
		}

		void PhysicsWorldPostCreate()
		{
			if( physicsWorld != null )
				PhysicsNative.JPhysicsSystem_OptimizeBroadPhase( physicsWorld.physicsSystem );
		}

		void PhysicsWorldDestroy()
		{
			if( physicsWorld != null )
			{
				//!!!!
				//physicsWorldData.ClearShapeCache();


				//!!!!

				//CleanupConstraints( simulation.World );

				foreach( var body in physicsWorld.allRigidBodies.Values.ToArray() )
					body.Dispose();
				foreach( var shape in physicsWorld.nativeShapes.Values.ToArray() )
				{
					while( shape.referenceCounter > 0 )
						physicsWorld.FreeShape( shape );
				}

				PhysicsNative.JDestroySystem( physicsWorld.physicsSystem );

				physicsWorld = null;
			}
		}

		public event Action<Scene> PhysicsSimulationStepAfter;

		static List<PhysicsWorldClass.Body> tempBodiesToUpdateFromLibrary = null;

		[MethodImpl( (MethodImplOptions)512 )]
		internal unsafe void PhysicsSimulate( bool editorSimulateSelectedMode, ESet<IPhysicalObject> updateOnlySelected )
		{
			if( physicsWorld == null )
				return;

			//foreach( var c in physicsWorld.allConstraints )
			//{
			//	var vehicleConstraint = c as PhysicsWorldClass.VehicleConstraint;
			//	if( vehicleConstraint != null )
			//	{
			//		PhysicsNative.JVehicleConstraint_Update( vehicleConstraint.constraint );
			//	}
			//}

			//!!!!
			var debug = false;//EngineApp._DebugCapsLock;

			//simulate
			float invStep = (float)ProjectSettings.Get.General.SimulationStepsPerSecondInv;
			PhysicsNative.JPhysicsSystem_Update( physicsWorld.physicsSystem, invStep, PhysicsAdvancedSettings ? PhysicsCollisionSteps.Value : 2, debug );
			//PhysicsNative.JPhysicsSystem_Update( physicsWorld.physicsSystem, invStep, PhysicsAdvancedSettings ? PhysicsCollisionSteps.Value : 2, PhysicsAdvancedSettings ? PhysicsIntegrationSubSteps.Value : 1, debug );


			////get updated body states
			//if( tempBodiesToUpdateFromLibrary == null )
			//	tempBodiesToUpdateFromLibrary = new List<PhysicsWorldClass.Body>( 32 );

			//update lastActive field for active bodies
			PhysicsNative.JPhysicsSystem_GetActiveBodies( physicsWorld.physicsSystem, out var count, out var bodies );

			Parallel.For( 0, count, delegate ( int n ) //for( int n = 0; n < count; n++ )
			{
				var bodyId = bodies[ n ];
				if( physicsWorld.kinematicDynamicRigidBodies.TryGetValue( bodyId, out var bodyItem ) )
				{
					if( updateOnlySelected == null )
					{
						if( !bodyItem.active )
							bodyItem.active = true;
						//tempBodiesToUpdateFromLibrary.Add( bodyItem );
					}
					else
					{
						var bodyToUpdate = bodyItem.Owner as IPhysicalObject;
						if( bodyToUpdate != null && updateOnlySelected.Contains( bodyToUpdate ) )
						{
							if( !bodyItem.active )
								bodyItem.active = true;
							//tempBodiesToUpdateFromLibrary.Add( bodyItem );
						}
					}
				}
			} );
			PhysicsNative.JPhysicsSystem_GetActiveBodiesFree( physicsWorld.physicsSystem, bodies );


			//!!!!slowly. need get now sleeping bodies, but active == true

			//get updated body states
			if( tempBodiesToUpdateFromLibrary == null )
				tempBodiesToUpdateFromLibrary = new List<PhysicsWorldClass.Body>( 32 );

			foreach( var bodyItem in physicsWorld.kinematicDynamicRigidBodies.Values )
			{
				if( bodyItem.Active )
				{
					if( updateOnlySelected == null )
						tempBodiesToUpdateFromLibrary.Add( bodyItem );
					else
					{
						var bodyToUpdate = bodyItem.Owner as IPhysicalObject;
						if( bodyToUpdate != null && updateOnlySelected.Contains( bodyToUpdate ) )
							tempBodiesToUpdateFromLibrary.Add( bodyItem );
					}
				}
			}


			Parallel.ForEach( tempBodiesToUpdateFromLibrary, delegate ( PhysicsWorldClass.Body body )
			{
				body.UpdateDataFromPhysicsEngine();
			} );

			foreach( var body in tempBodiesToUpdateFromLibrary )
			{
				var owner = body.Owner;
				if( owner != null )
				{
					var rigidBody = owner as RigidBody;
					if( rigidBody != null )
						rigidBody.UpdateDataFromPhysicalBody();
					else
					{
						var meshInSpace = owner as MeshInSpace;
						if( meshInSpace != null )
							meshInSpace.UpdateDataFromPhysicalBody();
					}
				}
			}

			//foreach( var body in tempBodiesToUpdateFromLibrary )
			//{
			//	body.UpdateDataFromPhysicsEngine();

			//	var owner = body.Owner;
			//	if( owner != null )
			//	{
			//		var rigidBody = owner as RigidBody;
			//		if( rigidBody != null )
			//			rigidBody.UpdateDataFromPhysicalBody();
			//		else
			//		{
			//			var meshInSpace = owner as MeshInSpace;
			//			if( meshInSpace != null )
			//				meshInSpace.UpdateDataFromPhysicalBody();
			//		}
			//	}
			//}

			tempBodiesToUpdateFromLibrary.Clear();


			//!!!!slowly. use active bodies list
			foreach( var twoBody in physicsWorld.twoBodyConstraints )
			{
				if( twoBody.BodyA.Active || twoBody.BodyB.Active )
				{
					var owner = twoBody.Owner;
					if( owner != null )
					{
						var component = owner as Constraint_SixDOF;
						if( component != null && ( updateOnlySelected == null || updateOnlySelected.Contains( component ) ) )
							component.UpdateDataFromPhysicalConstraint();
					}
				}
			}

			//foreach( var c in physicsWorld.allConstraints )
			//{
			//	var twoBody = c as PhysicsWorldClass.TwoBodyConstraint;
			//	if( twoBody != null )
			//	{
			//		if( !twoBody.BodyA.Active && !twoBody.BodyB.Active )
			//			continue;
			//	}

			//	var owner = c.Owner;
			//	if( owner != null )
			//	{
			//		var component = owner as Constraint_SixDOF;
			//		if( component != null && ( updateOnlySelected == null || updateOnlySelected.Contains( component ) ) )
			//			component.UpdateDataFromPhysicalConstraint();
			//	}
			//}

			//////get updated body states
			////foreach( var bodyItem in physicsWorld.dynamicRigidBodies.Values )
			////{
			////	var bodyToUpdate = bodyItem.RigidBodyToUpdateFromLibrary;
			////	if( bodyToUpdate != null && ( updateOnlySelected == null || updateOnlySelected.Contains( bodyToUpdate ) ) )
			////		bodyToUpdate.UpdateDataFromPhysicsEngine();
			////}

			////foreach( var body in GetComponents<RigidBody>( checkChildren: true, onlyEnabledInHierarchy: true ) )
			////	body.UpdateDataFromPhysicsEngine();


			PhysicsSimulationStepAfter?.Invoke( this );
		}

		void PhysicsUpdateGravity()
		{
			if( physicsWorld != null )
			{
				var value = Gravity.Value.ToVector3F();
				PhysicsNative.JPhysicsSystem_SetGravity( physicsWorld.physicsSystem, ref value );
			}
		}

		unsafe void PhysicsViewportUpdateBegin( Viewport viewport )
		{
			//!!!!
			//PhysicsTest( viewport );

			//var renderer = viewport.simple3DRenderer;

			////draw internal data
			//if( physicsWorld != null && renderer != null )
			//{
			//	if( GetDisplayDevelopmentDataInThisApplication() && DisplayPhysicsInternal )
			//	{
			//		PhysicsNative.JPhysicsSystem_DebugDraw( physicsWorld.physicsSystem, out var lineCount, out var lines );

			//		if( lineCount > 0 )
			//		{
			//			//!!!!
			//			var lineCount2 = Math.Min( lineCount, 100000 );

			//			for( int n = 0; n < lineCount2; n++ )
			//			{
			//				ref var line = ref lines[ n ];

			//				renderer.SetColor( line.color.ToColorValue() );
			//				renderer.AddLineThin( line.from, line.to );
			//			}
			//		}


			//		//int lineCount = 0;
			//		//PhysicsNative.JPhysicsSystem_DebugDraw( physicsWorldData.physicsSystem, &lineCount, null );

			//		//if( lineCount > 0 )
			//		//{
			//		//	lineCount = Math.Min( lineCount, 100000 );

			//		//	var lines = new PhysicsNative.DebugDrawLineItem[ lineCount ];

			//		//	fixed( PhysicsNative.DebugDrawLineItem* pLines = lines )
			//		//		PhysicsNative.JPhysicsSystem_DebugDraw( physicsWorldData.physicsSystem, &lineCount, pLines );

			//		//	for( int n = 0; n < lineCount; n++ )
			//		//	{
			//		//		ref var line = ref lines[ n ];

			//		//		renderer.SetColor( line.color.ToColorValue() );
			//		//		renderer.AddLineThin( line.from, line.to );
			//		//	}
			//		//}
			//	}

			//	//physicsWorldData.debugDraw.renderer = viewport.simple3DRenderer;

			//	////update mode
			//	//DebugDrawModes mode = DebugDrawModes.DrawConstraints | DebugDrawModes.DrawConstraintLimits;
			//	////DebugDrawModes mode = DebugDrawModes.DrawWireframe | DebugDrawModes.DrawConstraints | DebugDrawModes.DrawConstraintLimits;
			//	////DebugDrawModes mode = DebugDrawModes.DrawWireframe | DebugDrawModes.DrawConstraintLimits;
			//	//if( DisplayPhysicsInternal )
			//	//	mode |= DebugDrawModes.All;
			//	//physicsWorldData.debugDraw.DebugMode = mode;

			//	//if( GetDisplayDevelopmentDataInThisApplication() && DisplayPhysicsInternal )
			//	//{
			//	//	//draw all internal data

			//	//	for( int n = 0; n < physicsWorldData.world.NumConstraints; n++ )
			//	//	{
			//	//		var c = physicsWorldData.world.GetConstraint( n );
			//	//		var c2 = c.Userobject as Constraint;
			//	//		if( c2 != null )
			//	//			c2.UpdateDebugDrawSize( viewport );
			//	//	}

			//	//	physicsWorldData.debugDraw.verticesRenderedCounter = 0;
			//	//	physicsWorldData.debugDraw.verticesRenderedCounterLimit = 100000;
			//	//	physicsWorldData.world.DebugDrawWorld();
			//	//	physicsWorldData.debugDraw.verticesRenderedCounter = 0;
			//	//	physicsWorldData.debugDraw.verticesRenderedCounterLimit = -1;
			//	//}
			//}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//class BulletRayResultCallback : RayResultCallback, IComparer<PhysicsRayTestItem.ResultItem>
		//{
		//	PhysicsRayTestItem item;
		//	List<PhysicsRayTestItem.ResultItem> resultList = new List<PhysicsRayTestItem.ResultItem>();

		//	//last collision data
		//	Internal.BulletSharp.Math.BVector3 hitNormalWorld;
		//	int localShapeInfoPart;
		//	int localShapeInfoTriangle;

		//	public BulletRayResultCallback( PhysicsRayTestItem item )
		//	{
		//		this.item = item;
		//	}

		//	public override double AddSingleResult( LocalRayResult rayResult, bool normalInWorldSpace )
		//	{
		//		CollisionObject = rayResult.CollisionObject;//HasHit==true

		//		//will terminate further ray tests, once the closestHitFraction reached zero
		//		if( item.Mode == PhysicsRayTestItem.ModeEnum.One )
		//			ClosestHitFraction = 0;

		//		//save data of last collision
		//		if( normalInWorldSpace )
		//			hitNormalWorld = rayResult.HitNormalLocal;
		//		else
		//			hitNormalWorld = Internal.BulletSharp.Math.BVector3.TransformCoordinate( rayResult.HitNormalLocal, rayResult.CollisionObject.WorldTransform.Basis );

		//		if( rayResult.LocalShapeInfo.Native != IntPtr.Zero )
		//		{
		//			localShapeInfoPart = rayResult.LocalShapeInfo?.ShapePart ?? 0;
		//			localShapeInfoTriangle = rayResult.LocalShapeInfo?.TriangleIndex ?? 0;
		//		}
		//		else
		//		{
		//			localShapeInfoPart = 0;
		//			localShapeInfoTriangle = 0;
		//		}

		//		//processing found collision
		//		if( item.Mode == PhysicsRayTestItem.ModeEnum.OneClosest )
		//		{
		//			ClosestHitFraction = rayResult.HitFraction;
		//			return rayResult.HitFraction;
		//		}

		//		var resultItem = CreateResultItem( rayResult.CollisionObject, rayResult.HitFraction );
		//		resultList.Add( resultItem );
		//		return ClosestHitFraction;
		//	}

		//	PhysicsRayTestItem.ResultItem CreateResultItem( CollisionObject obj, double hitFraction )
		//	{
		//		var resultItem = new PhysicsRayTestItem.ResultItem();
		//		resultItem.TriangleIndexSource = -1;
		//		resultItem.TriangleIndexProcessed = -1;

		//		resultItem.Body = obj.UserObject as PhysicalBody;

		//		if( resultItem.Body is RigidBody rigidBody )
		//		{
		//			resultItem.Shape = rigidBody.collisionShapeByIndex[ 0 ];
		//			if( obj.CollisionShape is CompoundShape compoundShape && compoundShape.NumChildShapes > 1 )
		//			{
		//				//фича буллета. индекс шейпа приходит как TriangleIndex
		//				var shapeIdx = localShapeInfoPart < 0 ? localShapeInfoTriangle : 0;

		//				if( rigidBody.collisionShapeByIndex != null && shapeIdx < rigidBody.collisionShapeByIndex.Length )
		//					resultItem.Shape = rigidBody.collisionShapeByIndex[ shapeIdx ];
		//			}
		//		}

		//		//Internal.BulletSharp.CollisionShape shape;

		//		//if( obj.CollisionShape is CompoundShape compoundShape )
		//		//{
		//		//	if( compoundShape.NumChildShapes == 1 )
		//		//		shape = compoundShape.GetChildShape( 0 );
		//		//	else
		//		//		shape = compoundShape.GetChildShape( shapeIdx );
		//		//}
		//		//else
		//		//	shape = obj.CollisionShape;

		//		//resultItem.Shape = shape?.UserObject as CollisionShape;
		//		//resultItem.Body = obj.UserObject as PhysicalBody;//ObjectInSpace;

		//		resultItem.Normal = BulletPhysicsUtility.Convert( hitNormalWorld );
		//		resultItem.DistanceScale = hitFraction;
		//		resultItem.Position = item.Ray.Origin + item.Ray.Direction * hitFraction;

		//		if( resultItem.Shape is CollisionShape_Mesh csMesh )//&& shape is TriangleMeshShape )
		//		{
		//			resultItem.TriangleIndexProcessed = localShapeInfoTriangle;

		//			csMesh.GetProcessedData( out _, out _, out var triToSource );

		//			if( triToSource == null )
		//				resultItem.TriangleIndexSource = localShapeInfoTriangle;
		//			else if( triToSource.Length > localShapeInfoTriangle )
		//				resultItem.TriangleIndexSource = triToSource[ localShapeInfoTriangle ];
		//		}
		//		else if( resultItem.Body is SoftBody softBody )
		//		{
		//			resultItem.TriangleIndexProcessed = localShapeInfoTriangle;

		//			softBody.GetProcessedData( out _, out _, out var triToSource );

		//			if( triToSource == null )
		//				resultItem.TriangleIndexSource = localShapeInfoTriangle;
		//			else if( triToSource.Length > localShapeInfoTriangle )
		//				resultItem.TriangleIndexSource = triToSource[ localShapeInfoTriangle ];
		//		}

		//		return resultItem;
		//	}

		//	public void PostProcess()
		//	{
		//		item.Result = Array.Empty<PhysicsRayTestItem.ResultItem>();

		//		// if CollisionObject == null RayCast has no results
		//		if( CollisionObject == null )
		//			return;

		//		if( item.Mode == PhysicsRayTestItem.ModeEnum.OneClosest || item.Mode == PhysicsRayTestItem.ModeEnum.One )
		//		{
		//			var result0 = CreateResultItem( CollisionObject, ClosestHitFraction );
		//			item.Result = new PhysicsRayTestItem.ResultItem[] { result0 };
		//			return;
		//		}

		//		if( resultList.Count == 0 )
		//			return;

		//		if( resultList.Count == 1 || item.Mode == PhysicsRayTestItem.ModeEnum.One )
		//		{
		//			item.Result = new PhysicsRayTestItem.ResultItem[] { resultList[ 0 ] };
		//			return;
		//		}

		//		if( item.Mode == PhysicsRayTestItem.ModeEnum.OneForEach )
		//		{
		//			item.Result = GetResultsOnePerShape( resultList );
		//			return;
		//		}

		//		//sort by distance in any other case
		//		CollectionUtility.SelectionSort( resultList, this );

		//		if( item.Mode == PhysicsRayTestItem.ModeEnum.All )
		//		{
		//			item.Result = resultList.ToArray();
		//			return;
		//		}

		//		//item.Mode == PhysicsRayCastItem.ModeEnum.OneClosestForEach
		//		item.Result = GetResultsOnePerShape( resultList );
		//	}

		//	static PhysicsRayTestItem.ResultItem[] GetResultsOnePerShape( List<PhysicsRayTestItem.ResultItem> list )
		//	{
		//		var resultCount = 0;
		//		var result = new PhysicsRayTestItem.ResultItem[ list.Count ];

		//		for( int i = 0; i < list.Count; i++ )
		//		{
		//			object shapeBody = list[ i ].Shape;
		//			if( shapeBody == null )
		//				shapeBody = list[ i ].Body;

		//			var added = false;

		//			for( int idx = 0; idx < resultCount; idx++ )
		//			{
		//				object addedShapeBody = result[ idx ].Shape;
		//				if( addedShapeBody == null )
		//					addedShapeBody = result[ idx ].Body;

		//				if( ReferenceEquals( addedShapeBody, shapeBody ) )
		//				{
		//					added = true;
		//					break;
		//				}
		//			}

		//			if( !added )
		//				result[ resultCount++ ] = list[ i ];
		//		}

		//		if( resultCount != result.Length )
		//			Array.Resize( ref result, resultCount );

		//		return result;
		//	}

		//	public int Compare( PhysicsRayTestItem.ResultItem x, PhysicsRayTestItem.ResultItem y )
		//	{
		//		if( x.DistanceScale < y.DistanceScale )
		//			return -1;
		//		if( x.DistanceScale > y.DistanceScale )
		//			return 1;
		//		return 0;
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!вариант без выделения GC. видимо писать в предоставленный массив или нативное выделение памяти

		//!!!!давать выделенный Result на входе. тогда нужно вернуть сколько реально. везде так

		public void PhysicsRayTest( PhysicsRayTestItem item )
		{
			unsafe
			{
				if( physicsWorld == null )
				{
					item.Result = Array.Empty<PhysicsRayTestItem.ResultItem>();
					return;
				}

				PhysicsNative.JPhysicsSystem_RayTest( physicsWorld.physicsSystem, ref item.Ray, (int)item.Mode, (int)item.Flags, out var resultCount, out var results, IntPtr.Zero );

				if( resultCount != 0 )
				{
					var results2 = new PhysicsRayTestItem.ResultItem[ resultCount ];

					for( int n = 0; n < resultCount; n++ )
					{
						ref var result = ref results[ n ];
						ref var result2 = ref results2[ n ];

						result2.Body = physicsWorld.allRigidBodies[ result.bodyId ];
						//!!!!
						//result2.ShapeIndex = result.shapeIndex;
						result2.Position = result.position;
						result2.Normal = result.normal;
						result2.DistanceScale = result.distanceScale;
						//!!!!
						//result2.TriangleIndexSource = result.triangleIndexSource;
						//result2.TriangleIndexProcessed = result.triangleIndexProcessed;
					}

					item.Result = results2;
				}
				else
					item.Result = Array.Empty<PhysicsRayTestItem.ResultItem>();

				if( results != null )
					PhysicsNative.JPhysicsSystem_RayTestFree( physicsWorld.physicsSystem, results );
			}

			//item.Result = Array.Empty<PhysicsRayTestItem.ResultItem>();

			//RayTest( item );
			//PhysicsRayTest( new PhysicsRayTestItem[] { item }, false );
		}

		public void PhysicsRayTest( PhysicsRayTestItem[] items, bool multithreaded )
		{
			if( multithreaded )
				Parallel.ForEach( items, i => PhysicsRayTest( i ) );
			else
			{
				foreach( var item in items )
					PhysicsRayTest( item );
			}


			//!!!!contactGroup. сами группы тоже пересмотреть
			//!!!!!!может указывать маску групп, т.е. список групп
			//!!!!!!может еще что-то



			//foreach( var item in items )
			//	item.Result = Array.Empty<PhysicsRayTestItem.ResultItem>();



			////!!!!threading. выполнять когда включается режим "можно", например во время рендера.
			////!!!!!в других кастах тоже.

			////!!!!slowly. batching support. в других методах тоже

			////!!!!contactGroup. сами группы тоже пересмотреть
			////!!!!!!может указывать маску групп, т.е. список групп
			////!!!!!!может еще что-то

			//foreach( var item in items )
			//{
			//	using( var cb = new BulletRayResultCallback( item ) )
			//	{
			//		//!!!!
			//		cb.CollisionFilterGroup = item.CollisionFilterGroup;
			//		cb.CollisionFilterMask = item.CollisionFilterMask;

			//		var castObject = item.SingleCastCollisionObject;
			//		zzzz;
			//		if( castObject != null )
			//		{
			//			//single object cast

			//			if( castObject is Internal.BulletSharp.SoftBody.SoftBody softBody )
			//			{
			//				var cbResult = new SoftBodyRayCast();
			//				var r = softBody.RayTest( BulletPhysicsUtility.Convert( item.Ray.Origin ), BulletPhysicsUtility.Convert( item.Ray.GetEndPoint() ), cbResult );
			//				if( r )
			//				{
			//					var lsi = new LocalShapeInfo();
			//					lsi.TriangleIndex = cbResult.Index;
			//					var lrr = new LocalRayResult( softBody, lsi, Internal.BulletSharp.Math.BVector3.Zero, cbResult.Fraction );

			//					cb.AddSingleResult( lrr, true );
			//				}
			//			}
			//			else
			//			{
			//				var fromMatrix = BulletPhysicsUtility.Convert( Matrix4.FromTranslate( item.Ray.Origin ) );
			//				var toMatrix = BulletPhysicsUtility.Convert( Matrix4.FromTranslate( item.Ray.GetEndPoint() ) );
			//				var objectTransform = castObject.WorldTransform;

			//				CollisionWorld.RayTestSingleRef( ref fromMatrix, ref toMatrix, castObject, castObject.CollisionShape, ref objectTransform, cb );
			//			}
			//		}
			//		else
			//		{
			//			//usual all objects cast

			//			var from = BulletPhysicsUtility.Convert( item.Ray.Origin );
			//			var to = BulletPhysicsUtility.Convert( item.Ray.GetEndPoint() );

			//			physicsWorldData.world.RayTestRef( ref from, ref to, cb );
			//		}

			//		//sort results
			//		cb.PostProcess();
			//	}
			//}
		}

		//public void PhysicsRayTest( PhysicsRayTestItem item )
		//{
		//	PhysicsRayTest( new PhysicsRayTestItem[] { item }, false );
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!
		//class BulletContactResultCallback : ContactResultCallback, IComparer<PhysicsContactTestItem.ResultItem>
		//{
		//	public PhysicsContactTestItem item;
		//	public List<PhysicsContactTestItem.ResultItem> resultList = new List<PhysicsContactTestItem.ResultItem>();

		//	//for OneForEach, OneClosestForEach
		//	Dictionary<object, double> processedCollisionObjects = new Dictionary<object, double>();
		//	ManifoldPoint manifoldPoint;
		//	CollisionObjectWrapper wrappedObj;

		//	//

		//	public BulletContactResultCallback( PhysicsContactTestItem item )
		//	{
		//		this.item = item;

		//		ClosestDistanceThreshold = item.ClosestDistanceThreshold;

		//		//!!!!
		//		CollisionFilterGroup = (int)CollisionFilterGroups.DefaultFilter;// ( int)item.ContactGroup;
		//		CollisionFilterMask = (int)CollisionFilterGroups.AllFilter;// item.ContactMask;
		//	}

		//	public override bool NeedsCollision( BroadphaseProxy proxy0 )
		//	{
		//		if( item.Mode == PhysicsContactTestItem.ModeEnum.One && processedCollisionObjects.Count > 0 )
		//			return false;

		//		if( item.Mode == PhysicsContactTestItem.ModeEnum.One && wrappedObj != null )
		//			return false;

		//		var obj = proxy0.ClientObject;

		//		if( obj is Internal.BulletSharp.SoftBody.SoftBody soft )
		//		{
		//			if( processedCollisionObjects.ContainsKey( soft ) )
		//				return false;

		//			processedCollisionObjects.Add( soft, .5 );
		//			var compSoft = soft.UserObject as SoftBody;

		//			item.CollisionObject.CollisionShape.GetBoundingSphere( out var cPos, out _ );

		//			var cPosW = BulletPhysicsUtility.Convert( Internal.BulletSharp.Math.BVector3.TransformCoordinate( cPos, item.CollisionObject.WorldTransform ) );

		//			var nodeIdx = compSoft.FindClosestNodeIndex( cPosW );

		//			var resultItem = new PhysicsContactTestItem.ResultItem
		//			{
		//				Body = (PhysicalBody)compSoft,
		//				Distance = .5,
		//				PositionWorldOnA = compSoft.GetNodePosition( nodeIdx ),
		//				PositionWorldOnB = cPosW
		//			};

		//			resultList.Add( resultItem );

		//			return false;
		//		}

		//		if( item.Mode != PhysicsContactTestItem.ModeEnum.OneForEach )
		//			return true;

		//		// if OneForEach and collision object was processed already
		//		if( processedCollisionObjects.ContainsKey( obj ) )
		//			return false;

		//		return true;
		//	}

		//	public override double AddSingleResult( ManifoldPoint cp, CollisionObjectWrapper colObj0Wrap, int partId0, int index0, CollisionObjectWrapper colObj1Wrap, int partId1, int index1 )
		//	{
		//		//!!!!slowly? некоторые режимы могут быть медленными

		//		//!!!!что с возвращаемым параметром
		//		//!!!!
		//		double returnValue = cp.Distance;

		//		if( item.Mode == PhysicsContactTestItem.ModeEnum.OneClosest && manifoldPoint?.Distance < cp.Distance )
		//			return returnValue;

		//		manifoldPoint = cp;
		//		wrappedObj = ( colObj0Wrap.CollisionObject == item.CollisionObject ) ? colObj1Wrap : colObj0Wrap;

		//		var collisionObject = wrappedObj.CollisionObject;

		//		if( item.Mode == PhysicsContactTestItem.ModeEnum.OneForEach )
		//		{
		//			if( processedCollisionObjects.ContainsKey( collisionObject ) )
		//				return returnValue;
		//		}
		//		if( item.Mode == PhysicsContactTestItem.ModeEnum.OneClosestForEach )
		//		{
		//			if( processedCollisionObjects.TryGetValue( collisionObject, out double distance ) && distance < cp.Distance )
		//				return returnValue;
		//		}

		//		if( item.Mode == PhysicsContactTestItem.ModeEnum.OneClosest )
		//			return returnValue;

		//		{
		//			var resultItem = CreateResultItem();
		//			if( resultItem != null )
		//			{
		//				switch( item.Mode )
		//				{
		//				case PhysicsContactTestItem.ModeEnum.One:
		//					if( resultList.Count == 0 )
		//						resultList.Add( resultItem );
		//					break;

		//				case PhysicsContactTestItem.ModeEnum.OneForEach:
		//					processedCollisionObjects[ collisionObject ] = resultItem.Distance;
		//					resultList.Add( resultItem );
		//					break;

		//				case PhysicsContactTestItem.ModeEnum.OneClosestForEach:
		//					{
		//						processedCollisionObjects[ collisionObject ] = resultItem.Distance;

		//						//!!!!slowly?

		//						bool wasAdded = false;
		//						for( int n = 0; n < resultList.Count; n++ )
		//						{
		//							if( resultList[ n ].Shape?.ParentRigidBody == resultItem.Shape.ParentRigidBody )
		//							{
		//								resultList[ n ] = resultItem;
		//								wasAdded = true;
		//								break;
		//							}
		//						}
		//						if( !wasAdded )
		//							resultList.Add( resultItem );
		//					}
		//					break;

		//				case PhysicsContactTestItem.ModeEnum.All:
		//					resultList.Add( resultItem );
		//					break;
		//				}
		//			}
		//		}

		//		return returnValue;
		//	}

		//	PhysicsContactTestItem.ResultItem CreateResultItem()
		//	{
		//		var resultItem = new PhysicsContactTestItem.ResultItem();

		//		var collisionObject = wrappedObj.CollisionObject;

		//		if( collisionObject != item.CollisionObject )
		//		{
		//			resultItem.LocalPointA = BulletPhysicsUtility.Convert( manifoldPoint.LocalPointA );
		//			resultItem.PositionWorldOnA = BulletPhysicsUtility.Convert( manifoldPoint.PositionWorldOnA );
		//			resultItem.LocalPointB = BulletPhysicsUtility.Convert( manifoldPoint.LocalPointB );
		//			resultItem.PositionWorldOnB = BulletPhysicsUtility.Convert( manifoldPoint.PositionWorldOnB );
		//		}
		//		else
		//		{
		//			resultItem.LocalPointA = BulletPhysicsUtility.Convert( manifoldPoint.LocalPointB );
		//			resultItem.PositionWorldOnA = BulletPhysicsUtility.Convert( manifoldPoint.PositionWorldOnB );
		//			resultItem.LocalPointB = BulletPhysicsUtility.Convert( manifoldPoint.LocalPointA );
		//			resultItem.PositionWorldOnB = BulletPhysicsUtility.Convert( manifoldPoint.PositionWorldOnA );
		//		}

		//		if( item.CheckPositionWorldOnB != null && !item.CheckPositionWorldOnB( resultItem.PositionWorldOnB ) )
		//			return null;

		//		//resultItem.Normal = BulletUtils.Convert( cp.NormalWorldOnB );
		//		resultItem.Distance = manifoldPoint.Distance;


		//		resultItem.Body = collisionObject.UserObject as PhysicalBody;

		//		if( resultItem.Body is RigidBody rigidBody )
		//		{
		//			resultItem.Shape = rigidBody.collisionShapeByIndex[ 0 ];
		//			if( collisionObject.CollisionShape is CompoundShape compoundShape && compoundShape.NumChildShapes > 1 )
		//			{
		//				var shapePart = wrappedObj.PartId;
		//				var shapeIdx = shapePart >= 0 ? shapePart : 0;

		//				if( rigidBody.collisionShapeByIndex != null && shapeIdx < rigidBody.collisionShapeByIndex.Length )
		//					resultItem.Shape = rigidBody.collisionShapeByIndex[ shapeIdx ];
		//			}
		//		}

		//		//Internal.BulletSharp.CollisionShape shape;

		//		//var shapePart = wrappedObj.PartId;

		//		//if( collisionObject.CollisionShape is CompoundShape compoundShape )
		//		//	shape = compoundShape.GetChildShape( shapePart >= 0 ? shapePart : 0 );
		//		//else
		//		//	shape = collisionObject.CollisionShape;

		//		////resultItem.Shape == null for SoftBody
		//		//resultItem.Shape = shape.UserObject as CollisionShape;

		//		//resultItem.Body = collisionObject.UserObject as RigidBody;


		//		//if( shape is TriangleMeshShape || shape is SoftBodyCollisionShape )
		//		//	resultItem.TriangleIndexProcessed = wrappedObj.Index;

		//		if( wrappedObj.Index != -1 )
		//		{
		//			if( resultItem.Shape is CollisionShape_Mesh csMesh )
		//			{
		//				resultItem.TriangleIndexProcessed = wrappedObj.Index;

		//				csMesh.GetProcessedData( out _, out _, out var triToSource );

		//				if( triToSource == null )
		//					resultItem.TriangleIndexSource = wrappedObj.Index;
		//				else if( triToSource.Length > wrappedObj.Index )
		//					resultItem.TriangleIndexSource = triToSource[ wrappedObj.Index ];
		//			}
		//			else if( resultItem.Body is SoftBody softBody )
		//			{
		//				//!!!!not checked

		//				resultItem.TriangleIndexProcessed = wrappedObj.Index;

		//				softBody.GetProcessedData( out _, out _, out var triToSource );

		//				if( triToSource == null )
		//					resultItem.TriangleIndexSource = wrappedObj.Index;
		//				else if( triToSource.Length > wrappedObj.Index )
		//					resultItem.TriangleIndexSource = triToSource[ wrappedObj.Index ];
		//			}
		//		}

		//		return resultItem;
		//	}

		//	public void PostProcess()
		//	{
		//		if( item.CollisionObject != null && item.CollisionObjectAutoDispose )
		//		{
		//			item.CollisionObject.CollisionShape?.Dispose();
		//			item.CollisionObject.Dispose();
		//		}

		//		if( item.Mode == PhysicsContactTestItem.ModeEnum.OneClosest && manifoldPoint != null )
		//		{
		//			var resultItem = CreateResultItem();
		//			if( resultItem != null )
		//				item.Result = new PhysicsContactTestItem.ResultItem[] { resultItem };
		//			else
		//				item.Result = Array.Empty<PhysicsContactTestItem.ResultItem>();
		//			return;
		//		}

		//		if( resultList.Count == 0 )
		//		{
		//			item.Result = Array.Empty<PhysicsContactTestItem.ResultItem>();
		//			return;
		//		}

		//		if( resultList.Count == 1 )
		//		{
		//			item.Result = new PhysicsContactTestItem.ResultItem[] { resultList[ 0 ] };
		//			return;
		//		}

		//		//sort by deep penetration
		//		if( item.Mode == PhysicsContactTestItem.ModeEnum.OneClosestForEach || item.Mode == PhysicsContactTestItem.ModeEnum.All )
		//			CollectionUtility.SelectionSort( resultList, this );

		//		//initialize result array
		//		item.Result = resultList.ToArray();
		//	}

		//	public int Compare( PhysicsContactTestItem.ResultItem x, PhysicsContactTestItem.ResultItem y )
		//	{
		//		if( x.Distance < y.Distance )
		//			return -1;
		//		if( x.Distance > y.Distance )
		//			return 1;
		//		return 0;
		//	}

		//}

		public void PhysicsVolumeTest( PhysicsVolumeTestItem item )
		{
			unsafe
			{
				if( physicsWorld == null )
				{
					item.Result = Array.Empty<PhysicsVolumeTestItem.ResultItem>();
					return;
				}

				//Sphere sphere
				//Box box
				//Bounds bounds
				//Capsule capsule
				//Cylinder cylinder

				int resultCount = 0;
				PhysicsNative.VolumeTestResult* results = null;

				if( item.ShapeSphere.HasValue )
				{
					var sphere = item.ShapeSphere.Value;
					PhysicsNative.JPhysicsSystem_VolumeTestSphere( physicsWorld.physicsSystem, (int)item.Mode, out resultCount, out results, ref item.Direction, ref sphere.Center, (float)sphere.Radius );
				}
				else if( item.ShapeBox.HasValue )
				{
					var box = item.ShapeBox.Value;
					var axis = box.Axis.ToQuaternion().ToQuaternionF();
					var extents = box.Extents.ToVector3F();
					PhysicsNative.JPhysicsSystem_VolumeTestBox( physicsWorld.physicsSystem, (int)item.Mode, out resultCount, out results, ref item.Direction, ref box.Center, ref axis, ref extents );
				}
				else if( item.ShapeCapsule.HasValue )
				{
					//!!!!check

					var capsule = item.ShapeCapsule.Value;
					var center = capsule.GetCenter();

					var rotation = QuaternionF.FromDirectionZAxisUp( ( capsule.Point2 - capsule.Point1 ).ToVector3F() );

					//!!!!rotation
					rotation *= QuaternionF.FromRotateByZ( MathEx.PI / 2 );

					PhysicsNative.JPhysicsSystem_VolumeTestCapsule( physicsWorld.physicsSystem, (int)item.Mode, out resultCount, out results, ref item.Direction, ref center, ref rotation, (float)capsule.GetLength(), (float)capsule.Radius );
				}
				else if( item.ShapeCylinder.HasValue )
				{
					//!!!!check

					var cylinder = item.ShapeCylinder.Value;
					var center = cylinder.GetCenter();

					var rotation = QuaternionF.FromDirectionZAxisUp( ( cylinder.Point2 - cylinder.Point1 ).ToVector3F() );

					//!!!!rotation
					rotation *= QuaternionF.FromRotateByZ( MathEx.PI / 2 );

					PhysicsNative.JPhysicsSystem_VolumeTestCylinder( physicsWorld.physicsSystem, (int)item.Mode, out resultCount, out results, ref item.Direction, ref center, ref rotation, (float)cylinder.GetLength(), (float)cylinder.Radius );
				}
				else
				{
					item.Result = Array.Empty<PhysicsVolumeTestItem.ResultItem>();
					return;
				}

				//switch( item.ShapeType )
				//{
				//case PhysicsContactTestItem.ShapeTypeEnum.Sphere:
				//	PhysicsNative.JPhysicsSystem_ContactTestSphere( physicsWorld.physicsSystem, (int)item.Mode, out resultCount, out results, ref item.ShapeSphere.Center, (float)item.ShapeSphere.Radius );
				//	break;

				//default:
				//	item.Result = Array.Empty<PhysicsContactTestItem.ResultItem>();
				//	return;
				//}

				if( resultCount != 0 )
				{
					var results2 = new PhysicsVolumeTestItem.ResultItem[ resultCount ];
					for( int n = 0; n < resultCount; n++ )
					{
						ref var result = ref results[ n ];
						ref var result2 = ref results2[ n ];
						result2.Body = physicsWorld.allRigidBodies[ result.bodyId ];
						result2.DistanceScale = result.fraction;
						//result2.BackFaceHit = result.backFaceHit != 0;
					}

					CollectionUtility.MergeSort( results2, delegate ( PhysicsVolumeTestItem.ResultItem item1, PhysicsVolumeTestItem.ResultItem item2 )
					 {
						 if( item1.DistanceScale < item2.DistanceScale )
							 return -1;
						 if( item1.DistanceScale > item2.DistanceScale )
							 return 1;
						 return 0;
					 } );

					item.Result = results2;
				}
				else
					item.Result = Array.Empty<PhysicsVolumeTestItem.ResultItem>();

				if( results != null )
					PhysicsNative.JPhysicsSystem_VolumeTestFree( physicsWorld.physicsSystem, results );
			}
		}

		public void PhysicsVolumeTest( PhysicsVolumeTestItem[] items, bool multithreaded )
		{
			if( multithreaded )
				Parallel.ForEach( items, i => PhysicsVolumeTest( i ) );
			else
			{
				foreach( var item in items )
					PhysicsVolumeTest( item );
			}
		}

		//public void PhysicsContactTest( PhysicsContactTestItem[] items, bool multithreaded )
		//{
		//	foreach( var item in items )
		//	{
		//		using( var cb = new BulletContactResultCallback( item ) )
		//		{
		//			physicsWorldData.world.ContactTest( item.CollisionObject, cb );
		//			cb.PostProcess();
		//		}
		//	}
		//}

		//public void PhysicsContactTest( PhysicsContactTestItem item )
		//{
		//	PhysicsContactTest( new PhysicsContactTestItem[] { item } );
		//}

		//!!!!
		//public Body[] PhysicsVolumeCast( Bounds bounds, int contactGroup )
		//public Body[] PhysicsVolumeCast( Box box, int contactGroup )
		//public Body[] PhysicsVolumeCast( Sphere sphere, int contactGroup )
		//public Body[] PhysicsVolumeCast( Capsule capsule, int contactGroup )

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!
		//class BulletConvexResultCallback : ConvexResultCallback, IComparer<PhysicsConvexSweepTestItem.ResultItem>
		//{
		//	PhysicsConvexSweepTestItem item;
		//	List<PhysicsConvexSweepTestItem.ResultItem> resultList = new List<PhysicsConvexSweepTestItem.ResultItem>();

		//	//last collision data
		//	CollisionObject hitObject;
		//	Internal.BulletSharp.Math.BVector3 hitNormalWorld;
		//	//Vector3 hitPointWorld;
		//	int localShapeInfoPart;
		//	int localShapeInfoTriangle;

		//	HashSet<object> processedObjects = new HashSet<object>();

		//	Plane[] hullPlanes;

		//	public BulletConvexResultCallback( PhysicsConvexSweepTestItem item )
		//	{
		//		this.item = item;
		//	}

		//	void CalculateHullPlanes()
		//	{
		//		BulletPhysicsUtility.Convert( ref item.transformedFrom, out var transformedFrom );
		//		item.Shape.GetAabbRef( ref transformedFrom, out var min, out var max );
		//		var boundsFrom = new Bounds( BulletPhysicsUtility.ToVector3( min ), BulletPhysicsUtility.ToVector3( max ) );

		//		BulletPhysicsUtility.Convert( ref item.transformedTo, out var transformedTo );
		//		item.Shape.GetAabbRef( ref transformedTo, out min, out max );
		//		var boundsTo = new Bounds( BulletPhysicsUtility.ToVector3( min ), BulletPhysicsUtility.ToVector3( max ) );

		//		var hullPositions = new Vector3[ 16 ];
		//		var pointsFrom = boundsFrom.ToPoints();
		//		var pointsTo = boundsTo.ToPoints();
		//		for( int n = 0; n < pointsFrom.Length; n++ )
		//			hullPositions[ n ] = pointsFrom[ n ];
		//		for( int n = 0; n < pointsTo.Length; n++ )
		//			hullPositions[ 8 + n ] = pointsTo[ n ];

		//		MathAlgorithms.ConvexHullFromMesh( hullPositions, out _, out _, out hullPlanes );

		//		//ConvexHullAlgorithm.Create( hullPositions, out var hullVertices, out var hullIndices );

		//		//hullPlanes = new Plane[ hullIndices.Length / 3 ];
		//		//for( int i = 0; i < hullIndices.Length; i += 3 )
		//		//{
		//		//	var v0 = hullVertices[ hullIndices[ i ] ];
		//		//	var v1 = hullVertices[ hullIndices[ i + 1 ] ];
		//		//	var v2 = hullVertices[ hullIndices[ i + 2 ] ];

		//		//	hullPlanes[ i / 3 ] = Plane.FromPoints( v0, v1, v2 );
		//		//}
		//	}

		//	public override bool NeedsCollision( BroadphaseProxy proxy0 )
		//	{
		//		if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.One && processedObjects.Count > 0 )
		//			return false;

		//		var obj = proxy0.ClientObject;

		//		if( obj is Internal.BulletSharp.SoftBody.SoftBody soft )
		//		{
		//			if( processedObjects.Contains( soft ) )
		//				return false;

		//			//!!!!slowly?
		//			if( hullPlanes == null )
		//				CalculateHullPlanes();
		//			//var bounds = new Bounds( BulletUtils.ToVec3( proxy0.AabbMin ), BulletUtils.ToVec3( proxy0.AabbMax ) );
		//			//soft.GetAabb( out var aabbMin, out var aabbMax );
		//			//var bounds = new Bounds( BulletUtils.ToVec3( aabbMin ), BulletUtils.ToVec3( aabbMax ) );
		//			//if( !MathAlgorithms.IntersectsConvexHull( hullPlanes, bounds ) )
		//			//	return false;
		//			if( !BulletPhysicsUtility.IntersectsConvexHull( hullPlanes, soft ) )
		//				return false;

		//			processedObjects.Add( soft );

		//			var compSoft = soft.UserObject as SoftBody;
		//			var pos = BulletPhysicsUtility.ToVector3( ( ( proxy0.AabbMax + proxy0.AabbMin ) * .5 ) );

		//			var originPos = item.OriginalFrom.GetTranslation();
		//			//var closestNodeIdx = compSoft.FindClosestNodeIndex( originPos );
		//			//pos = compSoft.GetNodePosition( closestNodeIdx );

		//			var sweepLen = ( originPos - item.OriginalTo.GetTranslation() ).Length();
		//			var distToOrigin = ( pos - originPos ).Length();

		//			var resultItem = new PhysicsConvexSweepTestItem.ResultItem
		//			{
		//				Body = (ObjectInSpace)compSoft,
		//				DistanceScale = distToOrigin / sweepLen,
		//				Position = compSoft.GetNodePosition( 0 )
		//			};

		//			resultList.Add( resultItem );

		//			return false;
		//		}

		//		return true;
		//	}

		//	public override double AddSingleResult( LocalConvexResult convexResult, bool normalInWorldSpace )
		//	{
		//		hitObject = convexResult.HitCollisionObject;

		//		//will terminate further ray tests, once the closestHitFraction reached zero
		//		if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.One )
		//			ClosestHitFraction = 0;

		//		//save data of last collision

		//		if( convexResult.LocalShapeInfo.Native != IntPtr.Zero )
		//		{
		//			localShapeInfoPart = convexResult.LocalShapeInfo?.ShapePart ?? 0;
		//			localShapeInfoTriangle = convexResult.LocalShapeInfo?.TriangleIndex ?? 0;
		//		}
		//		else
		//		{
		//			localShapeInfoPart = 0;
		//			localShapeInfoTriangle = 0;
		//		}

		//		if( normalInWorldSpace )
		//		{
		//			hitNormalWorld = convexResult.HitNormalLocal;
		//			//hitPointWorld = convexResult.HitPointLocal;
		//		}
		//		else
		//		{
		//			hitNormalWorld = Internal.BulletSharp.Math.BVector3.TransformCoordinate( convexResult.HitNormalLocal, hitObject.WorldTransform.Basis );
		//			//hitPointWorld = Vector3.TransformCoordinate( convexResult.HitPointLocal, hitObject.WorldTransform.Basis );
		//		}

		//		//processing found collision
		//		if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneClosest )
		//		{
		//			ClosestHitFraction = convexResult.HitFraction;
		//			return convexResult.HitFraction;
		//		}

		//		var resultItem = CreateResultItem( hitObject, convexResult.HitFraction );
		//		resultList.Add( resultItem );
		//		return ClosestHitFraction;
		//	}

		//	PhysicsConvexSweepTestItem.ResultItem CreateResultItem( CollisionObject obj, double hitFraction )
		//	{
		//		var resultItem = new PhysicsConvexSweepTestItem.ResultItem();
		//		resultItem.TriangleIndexProcessed = -1;
		//		resultItem.TriangleIndexSource = -1;

		//		//Internal.BulletSharp.CollisionShape shape;

		//		//LocalShapeInfo gives extra information for complex shapes
		//		//Currently, only btTriangleMeshShape is available, so it just contains triangleIndex and subpart

		//		resultItem.Body = obj.UserObject as PhysicalBody;

		//		if( resultItem.Body is RigidBody rigidBody )
		//		{
		//			resultItem.Shape = rigidBody.collisionShapeByIndex[ 0 ];
		//			if( obj.CollisionShape is CompoundShape compoundShape && compoundShape.NumChildShapes > 1 )
		//			{
		//				//фича буллета. индекс шейпа приходит как TriangleIndex
		//				var shapeIdx = localShapeInfoPart < 0 ? localShapeInfoTriangle : 0;

		//				if( rigidBody.collisionShapeByIndex != null && shapeIdx < rigidBody.collisionShapeByIndex.Length )
		//					resultItem.Shape = rigidBody.collisionShapeByIndex[ shapeIdx ];
		//			}
		//		}

		//		////фича буллета. индекс шейпа приходит как TriangleIndex
		//		//var shapeIdx = localShapeInfoPart < 0 ? localShapeInfoTriangle : 0;

		//		//if( obj.CollisionShape is CompoundShape compoundShape )
		//		//{
		//		//	if( compoundShape.NumChildShapes == 1 )
		//		//		shape = compoundShape.GetChildShape( 0 );
		//		//	else
		//		//		shape = compoundShape.GetChildShape( shapeIdx );
		//		//}
		//		//else
		//		//	shape = obj.CollisionShape;

		//		//resultItem.Shape = shape.UserObject as CollisionShape;
		//		//resultItem.Body = obj.UserObject as PhysicalBody;//ObjectInSpace;

		//		resultItem.Normal = BulletPhysicsUtility.Convert( hitNormalWorld );
		//		resultItem.DistanceScale = hitFraction;
		//		resultItem.Position = Vector3.Lerp( item.originalFrom.GetTranslation(), item.originalTo.GetTranslation(), hitFraction );
		//		//resultItem.HitPoint = BulletUtils.Convert( hitPointWorld );

		//		//if( shape is TriangleMeshShape || shape is SoftBodyCollisionShape )
		//		//	resultItem.TriangleIndexProcessed = localShapeInfoTriangle;

		//		if( resultItem.Shape is CollisionShape_Mesh csMesh )
		//		{
		//			resultItem.TriangleIndexProcessed = localShapeInfoTriangle;

		//			csMesh.GetProcessedData( out _, out _, out var triToSource );

		//			if( triToSource == null )
		//				resultItem.TriangleIndexSource = localShapeInfoTriangle;
		//			else if( triToSource.Length > localShapeInfoTriangle )
		//				resultItem.TriangleIndexSource = triToSource[ localShapeInfoTriangle ];
		//		}
		//		else if( resultItem.Body is SoftBody softBody )
		//		{
		//			//!!!!not checked

		//			resultItem.TriangleIndexProcessed = localShapeInfoTriangle;

		//			softBody.GetProcessedData( out _, out _, out var triToSource );

		//			if( triToSource == null )
		//				resultItem.TriangleIndexSource = localShapeInfoTriangle;
		//			else if( triToSource.Length > localShapeInfoTriangle )
		//				resultItem.TriangleIndexSource = triToSource[ localShapeInfoTriangle ];
		//		}

		//		return resultItem;
		//	}

		//	public void PostProcess()
		//	{
		//		if( item.Shape != null && item.ShapeAutoDispose )
		//			item.Shape.Dispose();

		//		item.Result = Array.Empty<PhysicsConvexSweepTestItem.ResultItem>();

		//		// if hitObject == null ConvexTest has no results
		//		if( hitObject == null )
		//			return;

		//		if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneClosest )
		//		{
		//			var result0 = CreateResultItem( hitObject, ClosestHitFraction );
		//			item.Result = new PhysicsConvexSweepTestItem.ResultItem[] { result0 };
		//			return;
		//		}

		//		if( resultList.Count == 0 )
		//			return;

		//		if( resultList.Count == 1 || item.Mode == PhysicsConvexSweepTestItem.ModeEnum.One )
		//		{
		//			item.Result = new PhysicsConvexSweepTestItem.ResultItem[] { resultList[ 0 ] };
		//			return;
		//		}

		//		if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneForEach )
		//		{
		//			item.Result = GetResultsOnePerShape( resultList );
		//			return;
		//		}

		//		//sort by distance in any other case
		//		CollectionUtility.SelectionSort( resultList, this );

		//		if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.All )
		//		{
		//			item.Result = resultList.ToArray();
		//			return;
		//		}

		//		//item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneClosestForEach
		//		item.Result = GetResultsOnePerShape( resultList );
		//	}

		//	static PhysicsConvexSweepTestItem.ResultItem[] GetResultsOnePerShape( List<PhysicsConvexSweepTestItem.ResultItem> list )
		//	{
		//		var resultCount = 0;
		//		var result = new PhysicsConvexSweepTestItem.ResultItem[ list.Count ];

		//		for( int i = 0; i < list.Count; i++ )
		//		{
		//			object shapeBody = list[ i ].Shape;

		//			if( shapeBody == null )
		//				shapeBody = list[ i ].Body;

		//			var added = false;

		//			for( int idx = 0; idx < resultCount; idx++ )
		//			{
		//				object addedShapeBody = result[ idx ].Shape;

		//				if( addedShapeBody == null )
		//					addedShapeBody = result[ idx ].Body;

		//				if( ReferenceEquals( addedShapeBody, shapeBody ) )
		//				{
		//					added = true;
		//					break;
		//				}
		//			}

		//			if( !added )
		//				result[ resultCount++ ] = list[ i ];
		//		}

		//		if( resultCount != result.Length )
		//			Array.Resize( ref result, resultCount );

		//		return result;
		//	}

		//	public int Compare( PhysicsConvexSweepTestItem.ResultItem x, PhysicsConvexSweepTestItem.ResultItem y )
		//	{
		//		if( x.DistanceScale < y.DistanceScale )
		//			return -1;
		//		if( x.DistanceScale > y.DistanceScale )
		//			return 1;
		//		return 0;
		//	}
		//}

		////class BulletConvexResultCallback_Before : ConvexResultCallback
		////{
		////	public override double AddSingleResult( LocalConvexResult convexResult, bool normalInWorldSpace )
		////	{
		////		if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneClosest )
		////			ClosestHitFraction = convexResult.HitFraction;

		////		var resultItem = new PhysicsConvexSweepTestItem.ResultItem();

		////		var collisionObject = convexResult.HitCollisionObject;
		////		var body = collisionObject as RigidBody;
		////		//!!!!soft body может быть
		////		if( body != null )
		////		{
		////			//Log.Fatal( "body == null." );
		////			//if( body == null )
		////			//	Log.Fatal( "impl. body == null." );

		////			CollisionShape shape;

		////			//!!!!temp. need get ShapePart

		////			var compoundShape = body.CollisionShape as CompoundShape;
		////			if( compoundShape != null )
		////				shape = compoundShape.GetChildShape( 0 );
		////			else
		////				shape = body.CollisionShape;

		////			resultItem.Shape = shape.UserObject as CollisionShape;
		////		}

		////		Vector3 normal;
		////		if( normalInWorldSpace )
		////			normal = convexResult.HitNormalLocal;
		////		else
		////			normal = Vector3.TransformCoordinate( convexResult.HitNormalLocal, collisionObject.WorldTransform.Basis );
		////		resultItem.Normal = BulletUtils.Convert( normal );
		////		resultItem.DistanceScale = convexResult.HitFraction;
		////		resultItem.Position = Vec3.Lerp( item.originalFrom.GetTranslation(), item.originalTo.GetTranslation(), resultItem.DistanceScale );

		////		//!!!!крешится
		////		//	shapePart = rayResult.LocalShapeInfo.ShapePart;
		////		//resultItem.triangleIndex = rayResult.LocalShapeInfo.TriangleIndex;

		////		//!!!!может полезно
		////		//HitPointWorld = convexResult.HitPointLocal;



		////		if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneClosest )
		////			resultList.Clear();
		////		resultList.Add( resultItem );


		////		if( item.Mode == PhysicsConvexSweepTestItem.ModeEnum.OneClosest )
		////			return convexResult.HitFraction;
		////		else
		////			return ClosestHitFraction;
		////	}
		////}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!impl

		////!!!!rename
		////!!!!threading
		//public void PhysicsConvexSweepTest( PhysicsConvexSweepTestItem[] items )
		//{
		//	//!!!!
		//	if( physicsWorld == null )
		//		return;

		//	//!!!!

		//	foreach( var item in items )
		//		item.Result = Array.Empty<PhysicsConvexSweepTestItem.ResultItem>();

		//	////!!!!contactGroup. сами группы тоже пересмотреть
		//	////!!!!!!может указывать маску групп, т.е. список групп
		//	////!!!!!!может еще что-то

		//	//foreach( var item in items )
		//	//{
		//	//	using( var cb = new BulletConvexResultCallback( item ) )
		//	//	{
		//	//		BulletPhysicsUtility.Convert( ref item.transformedFrom, out BMatrix from );
		//	//		BulletPhysicsUtility.Convert( ref item.transformedTo, out BMatrix to );
		//	//		physicsWorldData.world.ConvexSweepTestRef( item.Shape, ref from, ref to, cb, item.AllowedCcdPenetration );
		//	//		cb.PostProcess();
		//	//	}
		//	//}
		//}

		////!!!!rename
		//public void PhysicsConvexSweepTest( PhysicsConvexSweepTestItem item )
		//{
		//	PhysicsConvexSweepTest( new PhysicsConvexSweepTestItem[] { item } );
		//}

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

		void UpdateAdvancedPhysicsSettings()
		{
			if( physicsWorld != null )
			{
				var useDefault = !PhysicsAdvancedSettings.Value;

				if( !useDefault )
				{
					var maxInFlightBodyPairs = PhysicsAdvancedSettings ? PhysicsMaxInFlightBodyPairs.Value : 0;
					if( maxInFlightBodyPairs == 0 )
						maxInFlightBodyPairs = PhysicsMaxBodies / 10;

					PhysicsNative.JPhysicsSystem_SetPhysicsSettings( physicsWorld.physicsSystem, /*useDefault,*/
						maxInFlightBodyPairs,
						PhysicsStepListenersBatchSize,
						PhysicsStepListenerBatchesPerJob,
						(float)PhysicsBaumgarte,
						(float)PhysicsSpeculativeContactDistance,
						(float)PhysicsPenetrationSlop,
						(float)PhysicsLinearCastThreshold,
						(float)PhysicsLinearCastMaxPenetration,
						(float)( PhysicsManifoldTolerance * PhysicsManifoldTolerance ),
						(float)PhysicsMaxPenetrationDistance,
						(float)( PhysicsBodyPairCacheMaxDeltaPosition * PhysicsBodyPairCacheMaxDeltaPosition ),
						(float)PhysicsBodyPairCacheCosMaxDeltaRotationDiv2,
						(float)PhysicsContactNormalCosMaxDeltaRotation,
						(float)( PhysicsContactPointPreserveLambdaMaxDist * PhysicsContactPointPreserveLambdaMaxDist ),
						PhysicsNumVelocitySteps,
						PhysicsNumPositionSteps,
						(float)PhysicsMinVelocityForRestitution,
						(float)PhysicsTimeBeforeSleep,
						(float)PhysicsPointVelocitySleepThreshold,
						PhysicsConstraintWarmStart,
						PhysicsUseBodyPairContactCache,
						PhysicsUseManifoldReduction,
						PhysicsAllowSleeping,
						PhysicsCheckActiveEdges );
				}
				else
				{
					PhysicsNative.JPhysicsSystem_SetPhysicsSettings( physicsWorld.physicsSystem, /*useDefault,*/
						16384,
						8,
						1,
						(float)0.2,
						(float)0.01,
						(float)0.01,
						(float)0.1,//0.75,
						(float)0.05,//0.25,
						(float)( 0.001 * 0.001 ),
						(float)0.02,
						(float)( 0.001 * 0.001 ),
						(float)0.99984769515639123915701155881391,
						(float)0.99619469809174553229501040247389,
						(float)( 0.01 * 0.01 ),
						10,
						2,
						(float)1.0,
						(float)0.5,
						(float)0.01,
						true,
						true,
						true,
						true,
						true );
				}
			}
		}
	}
}
