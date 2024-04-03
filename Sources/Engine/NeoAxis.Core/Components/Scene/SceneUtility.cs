// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	public static class SceneUtility
	{
		//!!!!подобное для Brush режима
		//public delegate void CalculateCreateObjectPositionUnderCursorEventDelegate( ObjectInSpace objectInSpace, ref bool found, ref Vector3 position );
		//public static event CalculateCreateObjectPositionUnderCursorEventDelegate CalculateCreateObjectPositionUnderCursorEvent;

		//public class CalculateCreateObjectPositionByRayResult
		//{
		//	public bool Found;
		//	public Vector3 Position;
		//	public ObjectInSpace CollidedWith;
		//	public Vector3F Normal;
		//}

		public static (bool found, Vector3 position, Vector3F normal, ObjectInSpace collidedWith) CalculateCreateObjectPositionByRay( Scene scene, ObjectInSpace objectInSpace, Ray ray, bool allowSnap )
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
				if( !( objectInSpace is ParticleSystemInSpace ) )
					localBounds = objectInSpace.SpaceBounds.BoundingBox - objectInSpace.Transform.Value.Position;

				//Light, Camera, Decal, Grid specific. use bounds without scaling

				if( objectInSpace is Light || objectInSpace is Camera )
				{
					var bounds = new Bounds( objectInSpace.TransformV.Position );
					bounds.Expand( 0.5 );
					localBounds = bounds - objectInSpace.Transform.Value.Position;
				}
				else if( objectInSpace is Decal || objectInSpace is SoundSource )//|| objectInSpace is Grid )
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
			Vector3F resultMinScaleNormal = Vector3F.ZAxis;
			ObjectInSpace resultMinScaleCollidedWith = null;

			if( objectInSpace != null )
			{
				//when objectInSpace != null

				Plane[] planes;
				{
					var b1 = localBounds + ray.Origin;
					var b2 = b1 + ray.Direction;
					var points = CollectionUtility.Merge( b1.ToPoints(), b2.ToPoints() );
					MathAlgorithms.ConvexHullFromMesh( points, out _, out _, out planes );
				}

				var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, planes );
				scene.GetObjectsInSpace( item );

				foreach( var resultItem in item.Result )
				{
					if( objectInSpace != resultItem.Object && !resultItem.Object.GetAllParents().Contains( objectInSpace ) )
					{
						//mesh in space
						if( resultItem.Object is MeshInSpace meshInSpace )
						{
							Vector3[] verticesFull;
							int[] indices;
							{
								var b1 = localBounds + ray.Origin;
								var b2 = b1 + ray.Direction;
								var points = CollectionUtility.Merge( b1.ToPoints(), b2.ToPoints() );
								MathAlgorithms.ConvexHullFromMesh( points, out verticesFull, out indices );
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

									//!!!!?
									resultMinScaleNormal = Vector3F.ZAxis;//normal;

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

				var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, ray );
				scene.GetObjectsInSpace( item );

				foreach( var resultItem in item.Result )
				{
					//mesh in space
					if( resultItem.Object is MeshInSpace meshInSpace )
					{
						var rayCastResult = meshInSpace.RayCast( ray, Mesh.CompiledData.RayCastModes.Auto );
						if( rayCastResult != null )
						{
							if( rayCastResult.Scale <= 1 && rayCastResult.Scale < resultMinScale )
							{
								resultMinScale = rayCastResult.Scale;
								resultMinScaleNormal = rayCastResult.Normal;
								resultMinScaleCollidedWith = meshInSpace;
								//if( triangleIndex != -1 )
								//{
								//}
							}
						}

						//if( meshInSpace.RayCast( ray, Mesh.CompiledData.RayCastMode.Auto, out var scale, out var normal, out var triangleIndex ) )
						//{
						//	if( scale <= 1 && scale < resultMinScale )
						//	{
						//		resultMinScale = scale;
						//		resultMinScaleNormal = normal;
						//		resultMinScaleCollidedWith = meshInSpace;
						//		//if( triangleIndex != -1 )
						//		//{
						//		//}
						//	}
						//}
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
			if( scene.Mode.Value == Scene.ModeEnum._2D )
				pos.Z = Math.Ceiling( pos.Z );

#if !DEPLOY
			//snap
			if( allowSnap )
			{
				double snap;

				//if( Form.ModifierKeys.HasFlag( Keys.Control ) )
				snap = ProjectSettings.Get.SceneEditor.SceneEditorStepMovement;
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
#endif

			//CalculateCreateObjectPositionUnderCursorEvent?.Invoke( objectInSpace, ref found, ref pos );

			return (found, pos, resultMinScaleNormal, resultMinScaleCollidedWith);
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

		//!!!!где юзается? где не нужно теперь?
		public static (bool found, double positionZ, Vector3F normal) CalculateObjectPositionZ( Scene scene, GroupOfObjects toGroupOfObjects, double defaultPositionZ, Vector2 position, List<Component> destinationCachedBaseObjects = null )
		{
			var ray = new Ray( new Vector3( position, defaultPositionZ + 10000 ), new Vector3( 0, 0, -20000 ) );

			if( toGroupOfObjects != null )
			{
				//when destination != null

				double? maxPositionZ = null;
				Vector3F maxNormal = Vector3F.Zero;

				var baseObjects = destinationCachedBaseObjects ?? toGroupOfObjects.GetBaseObjects();
				foreach( var baseObject in baseObjects )
				{
					//terrain
					var terrain = baseObject as Terrain;
					if( terrain != null )
					{
						if( terrain.GetBounds2().Contains( position ) )
						{
							var height = terrain.GetHeight( position, false );
							if( maxPositionZ == null || height > maxPositionZ.Value )
							{
								maxPositionZ = height;
								maxNormal = terrain.GetNormal( position );
							}
						}
					}

					//mesh in space
					var meshInSpace = baseObject as MeshInSpace;
					if( meshInSpace != null )
					{
						var rayCastResult = meshInSpace.RayCast( ray, Mesh.CompiledData.RayCastModes.Auto );
						if( rayCastResult != null )
						{
							var height = ray.GetPointOnRay( rayCastResult.Scale ).Z;
							if( maxPositionZ == null || height > maxPositionZ.Value )
							{
								maxPositionZ = height;
								maxNormal = rayCastResult.Normal;
							}
						}

						//if( meshInSpace.RayCast( ray, Mesh.CompiledData.RayCastMode.Auto, out var scale, out var normal, out _ ) )
						//{
						//	var height = ray.GetPointOnRay( scale ).Z;
						//	if( maxPositionZ == null || height > maxPositionZ.Value )
						//	{
						//		maxPositionZ = height;
						//		maxNormal = normal;
						//	}
						//}
					}

					//group of objects
					var groupOfObjects = baseObject as GroupOfObjects;
					if( groupOfObjects != null )
					{
						//!!!!
					}

				}

				if( maxPositionZ != null )
					return (true, maxPositionZ.Value, maxNormal);
			}
			else
			{
				//when destination == null
				var result = CalculateCreateObjectPositionByRay( scene, null, ray, false );
				if( result.found )
					return (true, result.position.Z, result.normal);
			}

			return (false, defaultPositionZ, Vector3F.ZAxis);
		}

		public class PaintToLayerData
		{
			public Component[] AffectObjects;

			public Vector3 Position;
			public Quaternion Rotation;
			public double Radius;
			public double Height;

			public double Strength;
			//!!!!
			//public qqShape Shape;
			//public double Hardness;

			public bool Removing;

			//!!!!
			public bool CanCreateLayer;

			public Reference<Material> Material;
			public Reference<Surface> Surface;
		}

		//!!!!как с референсами ссылаться внутри сцены?
		static PaintLayer FindPaintLayer( Component component, Reference<Material> material, Reference<Surface> surface )
		{
			foreach( var layer in component.GetComponents<PaintLayer>() )
			{
				//!!!!?
				if( layer.Material.GetByReference == material.GetByReference )
					return layer;
				if( layer.Surface.GetByReference == surface.GetByReference )
					return layer;
				//if( material != null && layer.Material.Value == material )
				//	return layer;
				//if( surface != null && layer.Surface.Value == surface )
				//	return layer;
			}
			return null;
		}

		//!!!!в MathAlgoriths
		static bool IntersectTriangleCircle( ref Triangle2 triangle, ref Circle circle )
		{
			//!!!!slowly

			var radius2 = circle.Radius * circle.Radius;

			var v0 = triangle.A;
			var v1 = triangle.B;
			var v2 = triangle.C;

			return
				v0.LengthSquared() < radius2 ||
				v1.LengthSquared() < radius2 ||
				v2.LengthSquared() < radius2 ||
				MathAlgorithms.PointInTriangle( circle.Center, v0, v1, v2 ) ||
				circle.Intersects( new Line2( v0, v1 ) ) ||
				circle.Intersects( new Line2( v1, v2 ) ) ||
				circle.Intersects( new Line2( v0, v2 ) );
		}

		//!!!!undo постепенный. не сразу
		public static void PaintToLayer( PaintToLayerData data )//!!!!undo , Editor.DocumentInstance editorDocument )
		{
			//!!!!

			foreach( var affectObject in data.AffectObjects )
			{

				var layer = FindPaintLayer( affectObject, data.Material, data.Surface );
				if( layer == null )
				{
					//!!!!новый слой в undo

					layer = affectObject.CreateComponent<PaintLayer>( enabled: false );
					layer.Name = ComponentUtility.GetNewObjectUniqueName( layer );

					//!!!!
					if( affectObject is MeshInSpace )
						layer.MaskFormat = PaintLayer.MaskFormatEnum.Triangles;

					layer.Material = data.Material;
					layer.Surface = data.Surface;
					layer.Enabled = true;
				}

				//MeshInSpace
				var meshInSpace = affectObject as MeshInSpace;
				if( meshInSpace != null )
				{

					//!!!!

					var mesh = meshInSpace.MeshOutput;
					if( mesh != null && mesh.Result != null )
					{

						//!!!!сделать в MeshInSpace, Mesh норм способ выбирать треугольники

						var matrix = meshInSpace.TransformV.ToMatrix4();
						matrix.GetInverse( out var invMatrix );

						//!!!!slowly. лишнее выбирает

						//Transform.Value.ToMatrix4().GetInverse( out var transInv );
						//Ray localRay = transInv * ray;

						var extents = new Vector3( data.Height, data.Radius * 2, data.Radius * 2 );
						var box = new Box( data.Position, extents, data.Rotation.ToMatrix3() );

						var points = box.ToPoints();

						var localPoints = new Vector3[ points.Length ];
						for( int n = 0; n < localPoints.Length; n++ )
							localPoints[ n ] = invMatrix * points[ n ];

						var localBounds = new Bounds( localPoints[ 0 ] );
						localBounds.Add( localPoints );

						//!!!!лишнее выбирает
						var triangles = mesh.Result._GetIntersectedTrianglesFast( ref localBounds );

						var meshVertices = mesh.Result.ExtractedVerticesPositions;
						var meshIndices = mesh.Result.ExtractedIndices;

						//!!!!
						var mask = new byte[ 262144 ];
						//var mask = new byte[ triangleCount ];

						//!!!!slowly

						var dataRotationInverse = data.Rotation.GetInverse().ToMatrix3();

						var halfHeight = data.Height / 2;

						foreach( var triangleIndex in triangles )
						{
							var index0 = meshIndices[ triangleIndex * 3 + 0 ];
							var index1 = meshIndices[ triangleIndex * 3 + 1 ];
							var index2 = meshIndices[ triangleIndex * 3 + 2 ];

							ref var localVertex0 = ref meshVertices[ index0 ];
							ref var localVertex1 = ref meshVertices[ index1 ];
							ref var localVertex2 = ref meshVertices[ index2 ];

							var worldVertex0 = matrix * localVertex0;
							var worldVertex1 = matrix * localVertex1;
							var worldVertex2 = matrix * localVertex2;

							var toolSpaceVertex0 = dataRotationInverse * ( worldVertex0 - data.Position );
							var toolSpaceVertex1 = dataRotationInverse * ( worldVertex1 - data.Position );
							var toolSpaceVertex2 = dataRotationInverse * ( worldVertex2 - data.Position );

							if( toolSpaceVertex0.X < -halfHeight && toolSpaceVertex1.X < -halfHeight && toolSpaceVertex2.X < -halfHeight )
								continue;
							if( toolSpaceVertex0.X > halfHeight && toolSpaceVertex1.X > halfHeight && toolSpaceVertex2.X > halfHeight )
								continue;

							var v0 = toolSpaceVertex0.ToVector2();
							var v1 = toolSpaceVertex1.ToVector2();
							var v2 = toolSpaceVertex2.ToVector2();

							var triangle = new Triangle2( v0, v1, v2 );
							var circle = new Circle( Vector2.Zero, data.Radius );

							if( IntersectTriangleCircle( ref triangle, ref circle ) )
							{

								//!!!!

								mask[ triangleIndex ] = 255;
							}
						}

						layer.Mask = mask;

					}
				}

				//Terrain
				var terrain = affectObject as Terrain;
				if( terrain != null )
				{
					//!!!!
				}

				////!!!!
				//layer = affectObject.GetComponent<PaintLayer>();
				//if( layer != null )
				//{

				//	//!!!!

				//	var size = 256;

				//	var mask = new byte[ size * size ];

				//	//!!!!
				//	for( int y = 0; y < 255; y++ )
				//		for( int x = 0; x < 255; x++ )
				//			mask[ x + y * 256 ] = (byte)x;

				//	layer.Mask = mask;
				//}

			}
		}

	}
}
