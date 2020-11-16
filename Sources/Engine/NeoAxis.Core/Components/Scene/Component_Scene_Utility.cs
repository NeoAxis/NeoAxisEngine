// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NeoAxis
{
	static class Component_Scene_Utility
	{
		//!!!!подобное для Brush режима
		//public delegate void CalculateCreateObjectPositionUnderCursorEventDelegate( Component_ObjectInSpace objectInSpace, ref bool found, ref Vector3 position );
		//public static event CalculateCreateObjectPositionUnderCursorEventDelegate CalculateCreateObjectPositionUnderCursorEvent;

		//public class CalculateCreateObjectPositionByRayResult
		//{
		//	public bool Found;
		//	public Vector3 Position;
		//	public Component_ObjectInSpace CollidedWith;
		//	public Vector3F Normal;
		//}

		public static (bool found, Vector3 position, Component_ObjectInSpace collidedWith) CalculateCreateObjectPositionByRay( Component_Scene scene, Component_ObjectInSpace objectInSpace, Ray ray, bool allowSnap )
		{
			//var viewport = ViewportControl.Viewport;

			//Vector2 mouse;
			//if( overrideMouse.HasValue )
			//	mouse = overrideMouse.Value;
			//else
			//{
			//	mouse = viewport.MousePosition;
			//	if( !new Rectangle( 0, 0, 1, 1 ).Contains( mouse ) )
			//		mouse = new Vector2( 0.5, 0.5 );
			//}

			//Ray ray;
			//if( overrideRay.HasValue )
			//	ray = overrideRay.Value;
			//else
			//	ray = viewport.CameraSettings.GetRayByScreenCoordinates( mouse );
			//!!!!? clamp max distance
			//ray.Direction = ray.Direction.GetNormalize() * 100;

			//!!!!можно конвекс форму делать вместо бокса
			Bounds localBounds = new Bounds();
			if( objectInSpace != null )
			{
				//particle system specific
				if( !( objectInSpace is Component_ParticleSystemInSpace ) )
					localBounds = objectInSpace.SpaceBounds.CalculatedBoundingBox - objectInSpace.Transform.Value.Position;

				//Light, Camera, Decal, Grid specific. use bounds without scaling

				if( objectInSpace is Component_Light || objectInSpace is Component_Camera )
				{
					var bounds = new Bounds( objectInSpace.TransformV.Position );
					bounds.Expand( 0.5 );
					localBounds = bounds - objectInSpace.Transform.Value.Position;
				}
				else if( objectInSpace is Component_Decal || objectInSpace is Component_SoundSource || objectInSpace.GetType().Name == "Component_Grid" )
				{
					var bounds = new Bounds( objectInSpace.TransformV.Position );
					bounds.Expand( 0.25 );
					localBounds = bounds - objectInSpace.Transform.Value.Position;
				}
			}
			if( localBounds.GetSize().X < 0.001 )
				localBounds.Expand( new Vector3( 0.001, 0, 0 ) );
			if( localBounds.GetSize().Y < 0.001 )
				localBounds.Expand( new Vector3( 0, 0.001, 0 ) );
			if( localBounds.GetSize().Z < 0.001 )
				localBounds.Expand( new Vector3( 0, 0, 0.001 ) );

			double resultMinScale = 1.01;
			Component_ObjectInSpace resultMinScaleCollidedWith = null;

			if( objectInSpace != null )
			{
				//when objectInSpace != null

				Plane[] planes;
				{
					var b1 = localBounds + ray.Origin;
					var b2 = b1 + ray.Direction;
					var points = CollectionUtility.Merge( b1.ToPoints(), b2.ToPoints() );
					ConvexHullAlgorithm.Create( points, out planes );
				}

				var item = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, planes );
				scene.GetObjectsInSpace( item );

				foreach( var resultItem in item.Result )
				{
					if( objectInSpace != resultItem.Object && !resultItem.Object.GetAllParents( false ).Contains( objectInSpace ) )
					{
						//mesh in space
						if( resultItem.Object is Component_MeshInSpace meshInSpace )
						{
							Vector3[] verticesFull;
							int[] indices;
							{
								var b1 = localBounds + ray.Origin;
								var b2 = b1 + ray.Direction;
								var points = CollectionUtility.Merge( b1.ToPoints(), b2.ToPoints() );
								ConvexHullAlgorithm.Create( points, out verticesFull, out indices );
							}

							if( meshInSpace._Intersects( verticesFull, indices ) )
							{
								double minScale = 1.01;

								double currentScale = 0.5;
								//!!!!?
								const double threshold = 0.00001;

								double step = 0.25;
								while( step > threshold )
								{
									Vector3[] vertices = new Vector3[ verticesFull.Length ];
									for( int n = 0; n < vertices.Length; n++ )
										vertices[ n ] = verticesFull[ n ] - ray.Direction + ray.Direction * currentScale;
									//Vector3[] vertices;
									//int[] indices;
									//{
									//	var b1 = localBounds + ray.Origin;
									//	var b2 = b1 + ray.Direction * currentScale;
									//	var points = CollectionUtility.Merge( b1.ToPoints(), b2.ToPoints() );
									//	ConvexHullAlgorithm.Create( points, out vertices, out indices );
									//}

									bool intersects = meshInSpace._Intersects( vertices, indices );

									if( !intersects )
										minScale = currentScale;

									if( intersects )
										currentScale -= step;
									else
										currentScale += step;
									step /= 2;
								}

								if( minScale <= 1 && minScale < resultMinScale )
								{
									resultMinScale = minScale;
									resultMinScaleCollidedWith = meshInSpace;
								}
							}
						}

						//!!!!какие еще
					}
				}
			}
			else
			{
				//when objectInSpace == null

				var item = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, ray );
				scene.GetObjectsInSpace( item );

				foreach( var resultItem in item.Result )
				{
					//mesh in space
					if( resultItem.Object is Component_MeshInSpace meshInSpace )
					{
						if( meshInSpace.RayCast( ray, Component_Mesh.CompiledData.RayCastMode.Auto, out var scale, out var triangleIndex ) )
						{
							if( scale <= 1 && scale < resultMinScale )
							{
								resultMinScale = scale;
								resultMinScaleCollidedWith = meshInSpace;
								//if( triangleIndex != -1 )
								//{
								//}
							}
						}
					}

					//!!!!какие еще
				}

			}

			bool found;
			Vector3 pos;
			if( resultMinScale <= 1 )
			{
				found = true;
				pos = ray.GetPointOnRay( resultMinScale );
			}
			else
			{
				found = false;
				pos = ray.Origin + ray.Direction.GetNormalize() * Math.Max( localBounds.GetBoundingSphere().Radius, 1 ) * 20;
			}

			//snap for 2D mode
			if( scene.Mode.Value == Component_Scene.ModeEnum._2D )
				pos.Z = Math.Ceiling( pos.Z );

			//snap
			if( allowSnap )
			{
				double snap;

				//if( Form.ModifierKeys.HasFlag( Keys.Control ) )
				snap = ProjectSettings.Get.SceneEditorStepMovement;
				//else
				//	snap = 0;
				if( snap != 0 )
				{
					Vector3 snapVec = new Vector3( snap, snap, snap );
					pos += snapVec / 2;
					pos /= snapVec;
					pos = new Vector3I( (int)pos.X, (int)pos.Y, (int)pos.Z ).ToVector3();
					pos *= snapVec;
				}
			}

			//CalculateCreateObjectPositionUnderCursorEvent?.Invoke( objectInSpace, ref found, ref pos );

			return (found, pos, resultMinScaleCollidedWith);
			//objectToTransform.Transform = new Transform( pos, objectToTransform.Transform.Value.Rotation, objectToTransform.Transform.Value.Scale );
			//}
			//else
			//{
			//	var localBounds = objectInSpace.SpaceBounds.CalculatedBoundingBox - objectInSpace.Transform.Value.Position;

			//	//disable object to disable collisions
			//	var disable = ContainsPhysicsBodies( objectInSpace );
			//	if( disable )
			//		objectInSpace.Enabled = false;

			//	//!!!!contact group
			//	PhysicsConvexSweepTestItem castItem = new PhysicsConvexSweepTestItem( Matrix4.FromTranslate( ray.Origin ),
			//		Matrix4.FromTranslate( ray.Origin + ray.Direction ), 1, -1, PhysicsConvexSweepTestItem.ModeEnum.OneClosest, localBounds );
			//	Scene.PhysicsConvexSweepTest( new PhysicsConvexSweepTestItem[] { castItem } );

			//	//restore disabled object
			//	if( disable )
			//		objectInSpace.Enabled = true;

			//	Vector3 pos;
			//	if( castItem.Result.Length != 0 )
			//		pos = castItem.Result[ 0 ].Position;
			//	else
			//	{
			//		pos = ray.Origin + ray.Direction.GetNormalize() * Math.Max( localBounds.GetBoundingSphere().Radius, 1 ) * 20;
			//	}

			//	objectInSpace.Transform = new Transform( pos, objectInSpace.Transform.Value.Rotation, objectInSpace.Transform.Value.Scale );
			//}
		}

		public static (bool found, double positionZ) CalculateObjectPositionZ( Component_Scene scene, Component_GroupOfObjects toGroupOfObjects, double defaultPositionZ, Vector2 position, List<Component> destinationCachedBaseObjects = null )
		{
			var ray = new Ray( new Vector3( position, defaultPositionZ + 10000 ), new Vector3( 0, 0, -20000 ) );

			if( toGroupOfObjects != null )
			{
				//when destination != null

				double? maxPositionZ = null;

				var baseObjects = destinationCachedBaseObjects ?? toGroupOfObjects.GetBaseObjects();
				foreach( var baseObject in baseObjects )
				{
					//terrain
					var terrain = baseObject as Component_Terrain;
					if( terrain != null )
					{
						if( terrain.GetBounds2().Contains( position ) )
						{
							var height = terrain.GetHeight( position, false );
							if( maxPositionZ == null || height > maxPositionZ.Value )
								maxPositionZ = height;
						}
					}

					//mesh in space
					var meshInSpace = baseObject as Component_MeshInSpace;
					if( meshInSpace != null )
					{
						if( meshInSpace.RayCast( ray, Component_Mesh.CompiledData.RayCastMode.Auto, out var scale, out _ ) )
						{
							var height = ray.GetPointOnRay( scale ).Z;
							if( maxPositionZ == null || height > maxPositionZ.Value )
								maxPositionZ = height;
						}
					}

					//group of objects
					var groupOfObjects = baseObject as Component_GroupOfObjects;
					if( groupOfObjects != null )
					{
						//!!!!
					}

				}

				if( maxPositionZ != null )
					return (true, maxPositionZ.Value);
			}
			else
			{
				//when destination == null
				var result = CalculateCreateObjectPositionByRay( scene, null, ray, false );
				if( result.found )
					return (true, result.position.Z);
			}

			return (false, defaultPositionZ);
		}

	}
}
