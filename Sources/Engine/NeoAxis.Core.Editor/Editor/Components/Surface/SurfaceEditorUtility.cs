#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public static class SurfaceEditorUtility
	{
		public static void CreatePreviewObjects( Scene scene, Surface surface )
		{
			DestroyPreviewObjects( scene );

			var center = Vector3.Zero;

			double maxOccupiedAreaRadius;
			double averageOccupiedAreaRadius;
			{
				var groups = surface.GetComponents<SurfaceGroupOfElements>();
				if( groups.Length != 0 )
				{
					maxOccupiedAreaRadius = 0;
					averageOccupiedAreaRadius = 0;
					foreach( var group in groups )
					{
						if( group.OccupiedAreaRadius > maxOccupiedAreaRadius )
							maxOccupiedAreaRadius = group.OccupiedAreaRadius;
						averageOccupiedAreaRadius += group.OccupiedAreaRadius;
					}
					averageOccupiedAreaRadius /= groups.Length;
				}
				else
				{
					maxOccupiedAreaRadius = 1;
					averageOccupiedAreaRadius = 1;
				}
			}

			var toolRadius = maxOccupiedAreaRadius * 10;
			var toolStrength = 1.0;// CreateObjectsBrushStrength;
			var toolHardness = 0;// CreateObjectsBrushHardness;
			var random = new FastRandom( 0 );

			double GetHardnessFactor( double length )
			{
				if( length == 0 || length <= toolHardness * toolRadius )
					return 1;
				else
				{
					double c;
					if( toolRadius - toolRadius * toolHardness != 0 )
						c = ( length - toolRadius * toolHardness ) / ( toolRadius - toolRadius * toolHardness );
					else
						c = 0;
					return (float)Math.Cos( Math.PI / 2 * c );
				}
			}

			//calculate object count
			int count;
			{
				var toolSquare = Math.PI * toolRadius * toolRadius;

				double radius = averageOccupiedAreaRadius;//maxOccupiedAreaRadius;
				double objectSquare = Math.PI * radius * radius;
				if( objectSquare < 0.1 )
					objectSquare = 0.1;

				double maxCount = toolSquare / objectSquare;
				//maxCount /= 10;

				count = (int)( toolStrength * (double)maxCount );
				count = Math.Max( count, 1 );

				count *= 4;
				//count *= 20;
			}

			//var data = new List<GroupOfObjects.Object>( count );

			var totalBounds = new Bounds( center );
			totalBounds.Expand( toolRadius + maxOccupiedAreaRadius * 4.01 );

			var initSettings = new OctreeContainer.InitSettings();
			initSettings.InitialOctreeBounds = totalBounds;
			initSettings.OctreeBoundsRebuildExpand = Vector3.Zero;
			initSettings.MinNodeSize = totalBounds.GetSize() / 40;
			var octree = new OctreeContainer( initSettings );

			var octreeOccupiedAreas = new List<Sphere>( 256 );


			for( int n = 0; n < count; n++ )
			{
				surface.GetRandomVariation( new Surface.GetRandomVariationOptions(), random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );
				var surfaceGroup = surface.GetGroup( groupIndex );

				Vector3? position = null;

				for( var nRadiusMultiplier = 0; nRadiusMultiplier < 3; nRadiusMultiplier++ )
				{
					var radiusMultiplier = 1.0;
					switch( nRadiusMultiplier )
					{
					case 0: radiusMultiplier = 4; break;
					case 1: radiusMultiplier = 2; break;
					case 2: radiusMultiplier = 1; break;
					}

					int counter = 0;
					while( counter < 10 )
					{
						var offset = new Vector2( random.Next( toolRadius * 2 ) - toolRadius, random.Next( toolRadius * 2 ) - toolRadius );

						//check by radius and by hardness
						var length = offset.Length();
						if( length <= toolRadius && random.NextDouble() <= GetHardnessFactor( length ) )
						{
							var position2 = center.ToVector2() + offset;

							var regularAlignment = surfaceGroup.RegularAlignment;//groups[ groupIndex ].RegularAlignment;
							if( regularAlignment != 0 )
							{
								position2 /= regularAlignment;
								position2 = new Vector2( (int)position2.X, (int)position2.Y );
								position2 *= regularAlignment;
							}

							//var result = Scene_Utility.CalculateObjectPositionZ( Scene, toGroupOfObjects, center.Z, position2, destinationCachedBaseObjects );
							//if( result.found )
							//{

							var p = new Vector3( position2, 0 );// result.positionZ );

							var objSphere = new Sphere( p, surfaceGroup.OccupiedAreaRadius );
							objSphere.ToBounds( out var objBounds );

							var occupied = false;

							foreach( var index in octree.GetObjects( objBounds, 0xFFFFFFFF, OctreeContainer.ModeEnum.All ) )
							{
								var sphere = octreeOccupiedAreas[ index ];
								sphere.Radius *= 0.25;//back to original
								sphere.Radius *= radiusMultiplier;//multiply

								if( ( p - sphere.Center ).LengthSquared() < ( sphere.Radius + objSphere.Radius ) * ( sphere.Radius + objSphere.Radius ) )
								{
									occupied = true;
									break;
								}
							}

							if( !occupied )
							{
								//found place to create
								position = p;
								goto end;
							}
						}

						counter++;
					}
				}

				end:;

				if( position != null )
				{
					//add object

					var objPosition = position.Value + new Vector3( 0, 0, positionZ );
					var objRotation = rotation;
					var objScale = scale;

					var surfaceElement = surfaceGroup.GetElement( elementIndex );

					var surfaceElementMesh = surfaceElement as SurfaceElement_Mesh;
					if( surfaceElementMesh != null )
					{
						var meshInSpace = scene.CreateComponent<MeshInSpace>( enabled: false );
						meshInSpace.Transform = new Transform( objPosition, objRotation, objScale );

						//!!!!good?
						meshInSpace.Mesh = surfaceElementMesh.Mesh;
						if( meshInSpace.Mesh.Value == null )
							meshInSpace.Mesh = ResourceUtility.MeshInvalid;

						//!!!!good?
						if( surfaceElementMesh.ReplaceMaterial.ReferenceSpecified )
							meshInSpace.ReplaceMaterial = surfaceElementMesh.ReplaceMaterial;

						meshInSpace.Enabled = true;
					}

					//add to the octree

					octreeOccupiedAreas.Add( new Sphere( position.Value, surfaceGroup.OccupiedAreaRadius ) );

					var b = new Bounds( position.Value );
					b.Expand( surfaceGroup.OccupiedAreaRadius * 4 );
					octree.AddObject( ref b, 1 );
				}
			}

			octree.Dispose();

			//add ground when material is exists
			if( !string.IsNullOrEmpty( surface.Material.GetByReference ) )
			{
				var meshInSpace = scene.CreateComponent<MeshInSpace>( enabled: false );

				var mesh = meshInSpace.CreateComponent<Mesh>();
				mesh.Name = "Name";

				var meshGeometry = mesh.CreateComponent<MeshGeometry>();
				meshGeometry.Name = "Mesh Geometry";

				var radius = (float)( toolRadius + maxOccupiedAreaRadius );

				SimpleMeshGenerator.GenerateCylinder( 2, radius, 0.01f, 128, true, true, true, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out var indices, out _ );

				var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.All, true, out int vertexSize );

				var materialUV0 = surface.MaterialUV0.Value.ToVector2F();
				var materialUV1 = surface.MaterialUV1.Value.ToVector2F();

				var vertices = new StandardVertex[ positions.Length ];
				for( int n = 0; n < vertices.Length; n++ )
				{
					ref var vertex = ref vertices[ n ];

					vertex.Position = positions[ n ];
					vertex.Normal = normals[ n ];
					vertex.Tangent = tangents[ n ];
					vertex.Color = ColorValue.One;
					vertex.TexCoord0 = texCoords[ n ] * materialUV0 * radius;
					vertex.TexCoord1 = texCoords[ n ] * materialUV1 * radius;
				}

				meshGeometry.VertexStructure = vertexStructure;
				meshGeometry.Vertices = CollectionUtility.ToByteArray( vertices );
				meshGeometry.Indices = indices;

				//var meshGeometry = mesh.CreateComponent<MeshGeometry_Cylinder>();
				//meshGeometry.Name = "Mesh Geometry";
				//meshGeometry.Radius = toolRadius + maxOccupiedAreaRadius;
				//meshGeometry.Height = 0.01;
				//meshGeometry.Segments = 128;

				//!!!!good?
				meshGeometry.Material = surface.Material;

				meshInSpace.Mesh = ReferenceUtility.MakeRootReference( mesh );

				meshInSpace.Enabled = true;
			}
		}

		public static void DestroyPreviewObjects( Scene scene )
		{
			foreach( var c in scene.GetComponents<MeshInSpace>() )
				c.Dispose();
		}
	}
}

#endif