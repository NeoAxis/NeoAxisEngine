// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	partial class PlantGenerator
	{
		class Geometry
		{
			public OpenList<StandardVertex.StaticOneTexCoord> Vertices = new OpenList<StandardVertex.StaticOneTexCoord>( 8192 );
			public OpenList<int> Indices = new OpenList<int>( 8192 );
			public Reference<Material> Material;
		}

		protected virtual void GenerateMeshData()
		{
			var geometries = new List<Geometry>();

			Geometry GetOrCreateGeometry( Reference<Material> material )
			{
				var g = geometries.Find( g2 => g2.Material.Equals( material ) );
				if( g == null )
				{
					g = new Geometry();
					g.Material = material;
					geometries.Add( g );
				}
				return g;
			}

			Bounds = Bounds.Cleared;

			foreach( ElementTypeEnum elementType in Enum.GetValues( typeof( ElementTypeEnum ) ) )
			{
				foreach( var element in GetElementsListByType( elementType ) )
				{
					if( !element.IsValidRecursive() || !element.GenerateMeshData )
						continue;

					for( int nCross = 0; nCross < 2; nCross++ )
					{
						if( !element.Cross && nCross == 1 )
							continue;

						Vector3F[] positions = null;
						Vector3F[] normals = null;
						Vector4F[] tangents = null;
						Vector2F[] texCoords = null;
						//!!!!add data for shader animation
						int[] indices = null;

						if( element.Shape == ShapeEnum.Cylinder )
						{
							//!!!!

							//Cylinder shape

							//if( element.Length <= 0.001 )
							//	continue;

							//!!!!
							if( LOD >= 2 && elementType == ElementTypeEnum.Twig )
								continue;
							//!!!!
							//if( LODQuality >= 3 && elementType == ElementTypeEnum.Branch )
							//	continue;

							var segmentsByMeter = PlantType.SegmentsByMeter.Value;
							var segmentsByCircle = PlantType.SegmentsByCircle.Value;

							var heightSegments = 1;
							var thickSegments = 1;

							switch( LOD )
							{
							case 0:
								heightSegments = (int)( segmentsByMeter * element.Length );
								thickSegments = elementType == ElementTypeEnum.Trunk ? segmentsByCircle : ( segmentsByCircle / 2 );
								//thickSegments = elementType == ElementTypeEnum.Trunk ? 32 : 16;
								break;

							case 1:
								heightSegments = (int)( segmentsByMeter / 3.0 * element.Length );
								thickSegments = elementType == ElementTypeEnum.Trunk ? ( segmentsByCircle / 3 ) : ( segmentsByCircle / 4 );
								//thickSegments = 8;
								break;

							case 2:
								heightSegments = (int)( segmentsByMeter / 6.0 * element.Length );
								thickSegments = elementType == ElementTypeEnum.Trunk ? ( segmentsByCircle / 6 ) : ( segmentsByCircle / 8 );
								break;
							}

							thickSegments = Math.Max( thickSegments, 3 );

							if( heightSegments < 1 )
								continue;

							var vertexCount = ( heightSegments + 1 ) * ( thickSegments + 1 );
							var indexCount = heightSegments * thickSegments * 6;
							//caps
							indexCount += thickSegments * 2 * 3;

							positions = new Vector3F[ vertexCount ];
							normals = new Vector3F[ vertexCount ];
							tangents = new Vector4F[ vertexCount ];
							texCoords = new Vector2F[ vertexCount ];

							var heightSteps = heightSegments + 1;
							var thickSteps = thickSegments + 1;

							int currentVertex = 0;

							for( int heightStep = 0; heightStep < heightSteps; heightStep++ )
							{
								var heightFactor = (double)heightStep / (double)( heightSteps - 1 );
								var tr = element.Curve.GetTransformByTime( heightFactor );

								for( int thickStep = 0; thickStep < thickSteps; thickStep++ )
								{
									var thickFactor = (double)thickStep / (double)( thickSteps - 1 );

									var p = new Vector2(
										Math.Cos( Math.PI * 2 * thickFactor ),
										Math.Sin( Math.PI * 2 * thickFactor ) );

									var offsetNotScaled = tr.Rotation * new Vector3( 0, p.X, p.Y );
									var offset = tr.Rotation * new Vector3( 0, p.X * tr.Scale.X, p.Y * tr.Scale.Y );

									var pos = tr.Position + offset;

									positions[ currentVertex ] = pos.ToVector3F();

									var normal = offsetNotScaled.GetNormalize();
									var tangent = tr.Rotation.GetForward().Cross( normal );

									normals[ currentVertex ] = normal.ToVector3F();
									tangents[ currentVertex ] = new Vector4F( tangent.ToVector3F(), -1 );

									var material = element.Material;
									if( material != null )
									{
										if( material.PartType.Value == PlantMaterial.PartTypeEnum.Bark && material.UVMode.Value == PlantMaterial.UVModeEnum.Point )
										{
											texCoords[ currentVertex ] = material.UVFrontPosition.Value.ToVector2F();
										}
										else
										{
											if( material.RealLength.Value != 0 )
											{
												//!!!!different thinkness depending by length

												//!!!!only when square texture
												var realWidth = material.RealLength;
												var perimeter = Math.PI * element.Width;

												var lengthOffset = element.Length * heightFactor;
												texCoords[ currentVertex ] = new Vector2( perimeter * thickFactor / realWidth , lengthOffset / material.RealLength ).ToVector2F();
											}
										}
									}

									currentVertex++;
								}
							}

							if( currentVertex != vertexCount )
								Log.Fatal( "Plant: GenerateMeshData: currentVertex != vertexCount." );


							//indices

							indices = new int[ indexCount ];
							int currentIndex = 0;

							for( int heightStep = 0; heightStep < heightSegments; heightStep++ )
							{
								for( int thickStep = 0; thickStep < thickSegments; thickStep++ )
								{
									indices[ currentIndex++ ] = ( thickSegments + 1 ) * heightStep + thickStep;
									indices[ currentIndex++ ] = ( thickSegments + 1 ) * heightStep + thickStep + 1;
									indices[ currentIndex++ ] = ( thickSegments + 1 ) * ( heightStep + 1 ) + thickStep + 1;
									indices[ currentIndex++ ] = ( thickSegments + 1 ) * ( heightStep + 1 ) + thickStep + 1;
									indices[ currentIndex++ ] = ( thickSegments + 1 ) * ( heightStep + 1 ) + thickStep;
									indices[ currentIndex++ ] = ( thickSegments + 1 ) * heightStep + thickStep;
								}
							}

							//caps
							{
								for( int thickStep = 0; thickStep < thickSegments; thickStep++ )
								{
									indices[ currentIndex++ ] = 0;
									indices[ currentIndex++ ] = thickStep + 1;
									indices[ currentIndex++ ] = thickStep;
								}

								for( int thickStep = 0; thickStep < thickSegments; thickStep++ )
								{
									var startIndex = ( thickSegments + 1 ) * ( heightSegments );
									indices[ currentIndex++ ] = startIndex + 0;
									indices[ currentIndex++ ] = startIndex + thickStep;
									indices[ currentIndex++ ] = startIndex + thickStep + 1;
								}
							}

							if( currentIndex != indexCount )
								Log.Fatal( "Plant: GenerateMeshData: currentIndex != indexCount." );
							foreach( var index in indices )
							{
								if( index < 0 || index >= vertexCount )
									Log.Fatal( "Plant: GenerateMeshData: index < 0 || index >= vertexCount." );
							}
						}
						else if( element.Shape == ShapeEnum.Ribbon )
						{
							//Ribbon shape

							//!!!!tesselation segments by width

							//!!!!element.BendingDownByLength != 0

							var material = element.Material;
							var uvLengthRange = material.UVLengthRange.Value;
							var uvWidth = material.UVWidth.Value;
							var containsBack = material != null && material.UVBack;

							//!!!!
							var steps = 2;
							var segments = steps - 1;

							var vertexCount = steps * 2;
							var indexCount = segments * 6;
							if( containsBack )
							{
								vertexCount *= 2;
								indexCount *= 2;
							}

							positions = new Vector3F[ vertexCount ];
							normals = new Vector3F[ vertexCount ];
							tangents = new Vector4F[ vertexCount ];
							texCoords = new Vector2F[ vertexCount ];
							indices = new int[ indexCount ];

							int currentVertex = 0;
							int currentIndex = 0;

							for( int nSide = 0; nSide < 2; nSide++ )
							{
								if( nSide == 1 && !containsBack )
									continue;
								var isFront = nSide == 0;

								var vertexStart = currentVertex;

								//vertices
								for( int nLengthStep = 0; nLengthStep < steps; nLengthStep++ )
								{
									//!!!!
									var timeScale = nLengthStep == 0 ? 0.0 : 1.0;
									var time = timeScale;

									//var time = ( (double)nStep / (double)( steps - 1 ) ) * element.Curve.MaxTime;
									var tr = element.Curve.GetTransformByTime( time );

									//rotate transform
									if( nCross == 1 )
										tr = tr.UpdateRotation( tr.Rotation * Quaternion.FromRotateByX( Math.PI / 2 ) );

									for( int nWidthStep = 0; nWidthStep < 2; nWidthStep++ )
									{
										//!!!!
										var widthScale = nWidthStep == 0 ? 0.0 : 1.0;

										double positionOffset = 0;
										if( uvLengthRange.Size != 0 )
											positionOffset = uvLengthRange.Minimum / uvLengthRange.Size;
										positionOffset *= element.Length;

										var corner = new Vector2( timeScale, nWidthStep == 0 ? -1 : 1 );

										positions[ currentVertex ] = ( tr * new Vector3( positionOffset, corner.Y, 0 ) ).ToVector3F();

										var normalsRot = tr.Rotation;

										//!!!!на краях риббон или посередине листа
										if( element.VolumeNormalsDegree != 0 )
										{
											{
												Quaternion volumeNormalsRotation;
												if( nWidthStep == 0 )
													volumeNormalsRotation = Quaternion.FromRotateByX( element.VolumeNormalsDegree.InRadians() );
												else
													volumeNormalsRotation = Quaternion.FromRotateByX( -element.VolumeNormalsDegree.InRadians() );

												normalsRot *= volumeNormalsRotation;
											}

											if( nLengthStep == 0 )
												normalsRot *= Quaternion.FromRotateByY( -element.VolumeNormalsDegree.InRadians() );
											if( nLengthStep == steps - 1 )
												normalsRot *= Quaternion.FromRotateByY( element.VolumeNormalsDegree.InRadians() );
										}

										normals[ currentVertex ] = ( normalsRot * new Vector3( 0, 0, isFront ? -1 : 1 ) ).ToVector3F();
										tangents[ currentVertex ] = new Vector4F( ( normalsRot * new Vector3( 0, 1, 0 ) ).ToVector3F(), -1 );

										//normals[ currentVertex ] = ( normalsRot * new Vector3( 0, 0, isFront ? 1 : -1 ) ).ToVector3F();
										//tangents[ currentVertex ] = new Vector4F( ( normalsRot * new Vector3( 0, 1, 0 ) ).ToVector3F(), -1 );

										//normals[ currentVertex ] = ( tr.Rotation * new Vector3( 0, 0, isFront ? 1 : -1 ) ).ToVector3F();
										//tangents[ currentVertex ] = new Vector4F( ( tr.Rotation * new Vector3( 0, 1, 0 ) ).ToVector3F(), -1 );

										//tangents[ currentVertex ] = new Vector4F( ( tr.Rotation * new Vector3( 1, 0, 0 ) ).ToVector3F(), -1 );

										if( material != null )
										{
											Vector2 pos;
											Radian dirAngle;
											if( isFront )
											{
												pos = material.UVFrontPosition.Value;
												dirAngle = material.UVFrontDirection.Value.InRadians();
											}
											else
											{
												pos = material.UVBackPosition.Value;
												dirAngle = material.UVBackDirection.Value.InRadians();
											}

											var rot = Quaternion.FromRotateByZ( -dirAngle );

											var x = uvLengthRange.Minimum + timeScale * uvLengthRange.Size;
											var y = ( widthScale - 0.5 ) * uvWidth;
											var p = pos + ( rot * new Vector3( x, y, 0 ) ).ToVector2();

											texCoords[ currentVertex ] = p.ToVector2F();
										}

										currentVertex++;
									}
								}

								//indices
								for( int nSegment = 0; nSegment < segments; nSegment++ )
								{
									if( isFront )
									{
										indices[ currentIndex++ ] = vertexStart + 0;
										indices[ currentIndex++ ] = vertexStart + 1;
										indices[ currentIndex++ ] = vertexStart + 3;
										indices[ currentIndex++ ] = vertexStart + 3;
										indices[ currentIndex++ ] = vertexStart + 2;
										indices[ currentIndex++ ] = vertexStart + 0;
									}
									else
									{
										indices[ currentIndex++ ] = vertexStart + 0;
										indices[ currentIndex++ ] = vertexStart + 3;
										indices[ currentIndex++ ] = vertexStart + 1;
										indices[ currentIndex++ ] = vertexStart + 3;
										indices[ currentIndex++ ] = vertexStart + 0;
										indices[ currentIndex++ ] = vertexStart + 2;
									}
								}
							}


							//var trStart = element.Curve.GetTransformByTime( 0 );
							//var trEnd = element.Curve.GetTransformByTime( 1 );

							//var p0 = trStart * new Vector3( 0, -1, 0 );
							//var p1 = trStart * new Vector3( 0, 1, 0 );
							//var p2 = trEnd * new Vector3( 0, -1, 0 );
							//var p3 = trEnd * new Vector3( 0, 1, 0 );


							//{
							//	positions[ currentVertex ] = ( trStart * new Vector3( 0, -1, 0 ) ).ToVector3F();
							//	//positions[ currentVertex ] = ( trStart * new Vector3( 0, -data.StartRadius, 0 ) ).ToVector3F();

							//	//!!!!
							//	normals[ currentVertex ] = new Vector3F( 0, 1, 0 );
							//	tangents[ currentVertex ] = new Vector4F( 1, 0, 0, -1 );

							//	texCoords[ currentVertex ] = new Vector2F( 0, 1 );

							//	currentVertex++;
							//}
							//{
							//	positions[ currentVertex ] = ( trStart * new Vector3( 0, 1, 0 ) ).ToVector3F();
							//	//positions[ currentVertex ] = ( trStart * new Vector3( 0, data.StartRadius, 0 ) ).ToVector3F();

							//	//!!!!
							//	normals[ currentVertex ] = new Vector3F( 0, 1, 0 );
							//	tangents[ currentVertex ] = new Vector4F( 1, 0, 0, -1 );

							//	texCoords[ currentVertex ] = new Vector2F( 1, 1 );

							//	currentVertex++;
							//}
							//{
							//	positions[ currentVertex ] = ( trEnd * new Vector3( 0, -1, 0 ) ).ToVector3F();
							//	//positions[ currentVertex ] = ( trEnd * new Vector3( 0, -data.StartRadius, 0 ) ).ToVector3F();

							//	//!!!!
							//	normals[ currentVertex ] = new Vector3F( 0, 1, 0 );
							//	tangents[ currentVertex ] = new Vector4F( 1, 0, 0, -1 );

							//	texCoords[ currentVertex ] = new Vector2F( 0, 0 );

							//	currentVertex++;
							//}
							//{
							//	positions[ currentVertex ] = ( trEnd * new Vector3( 0, 1, 0 ) ).ToVector3F();
							//	//positions[ currentVertex ] = ( trEnd * new Vector3( 0, data.StartRadius, 0 ) ).ToVector3F();

							//	//!!!!
							//	normals[ currentVertex ] = new Vector3F( 0, 1, 0 );
							//	tangents[ currentVertex ] = new Vector4F( 1, 0, 0, -1 );

							//	texCoords[ currentVertex ] = new Vector2F( 1, 0 );

							//	currentVertex++;
							//}



							//for( int heightStep = 0; heightStep < heightSteps; heightStep++ )
							//{
							//	var heightFactor = (double)heightStep / (double)( heightSteps - 1 );
							//	var tr = data.Curve.GetTransformByTime( heightFactor );

							//	for( int thickStep = 0; thickStep < thickSteps; thickStep++ )
							//	{
							//		var thickFactor = (double)thickStep / (double)( thickSteps - 1 );

							//		var p = new Vector2(
							//			Math.Cos( Math.PI * 2 * thickFactor ),// * tr.Scale.X,
							//			Math.Sin( Math.PI * 2 * thickFactor ) );// * tr.Scale.Y );

							//		var offsetNotScaled = tr.Rotation * new Vector3( 0, p.X, p.Y );
							//		var offset = tr.Rotation * new Vector3( 0, p.X * tr.Scale.X, p.Y * tr.Scale.Y );
							//		//var offset = tr.Rotation * new Vector3( 0, p.X, p.Y );

							//		var pos = tr.Position + offset;

							//		positions[ currentVertex ] = pos.ToVector3F();

							//		//!!!!normals, etc

							//		var normal = offsetNotScaled.GetNormalize();
							//		//!!!!
							//		var tangent = tr.Rotation.GetForward().Cross( normal );

							//		normals[ currentVertex ] = normal.ToVector3F();
							//		tangents[ currentVertex ] = new Vector4F( tangent.ToVector3F(), -1 );

							//		//!!!!scaling
							//		texCoords[ currentVertex ] = new Vector2( thickFactor, heightFactor * 20 ).ToVector2F();

							//		currentVertex++;
							//	}
							//}

							////indices

							//indices = new int[ indexCount ];
							//int currentIndex = 0;

							//indices[ currentIndex++ ] = 0;
							//indices[ currentIndex++ ] = 1;
							//indices[ currentIndex++ ] = 3;
							//indices[ currentIndex++ ] = 3;
							//indices[ currentIndex++ ] = 2;
							//indices[ currentIndex++ ] = 0;

							if( currentVertex != vertexCount )
								Log.Fatal( "Plant: GenerateMeshData: currentVertex != vertexCount." );
							if( currentIndex != indexCount )
								Log.Fatal( "Plant: GenerateMeshData: currentIndex != indexCount." );
							foreach( var index in indices )
							{
								if( index < 0 || index >= vertexCount )
									Log.Fatal( "Plant: GenerateMeshData: index < 0 || index >= vertexCount." );
							}

						}
						else if( element.Shape == ShapeEnum.Bowl )
						{
							//Bowl shape


							//!!!!



							////if( element.Length <= 0.001 )
							////	continue;

							////!!!!
							//if( LOD >= 2 && elementType == ElementTypeEnum.Twig )
							//	continue;
							////!!!!
							////if( LOD >= 3 && elementType == ElementTypeEnum.Branch )
							////	continue;

							//var heightSegments = 1;
							//var thickSegments = 1;

							//switch( LOD )
							//{
							//case 0:
							//	heightSegments = (int)( PlantType.SegmentsByMeter.Value * element.Length );
							//	thickSegments = elementType == ElementTypeEnum.Trunk ? 32 : 16;
							//	break;

							//case 1:
							//	heightSegments = (int)( PlantType.SegmentsByMeter.Value / 3.0 * element.Length );
							//	thickSegments = 8;
							//	break;

							//case 2:
							////!!!!
							//case 3:
							//	heightSegments = (int)( PlantType.SegmentsByMeter.Value / 6.0 * element.Length );
							//	thickSegments = elementType == ElementTypeEnum.Trunk ? 6 : 4;
							//	break;

							//	//!!!!
							//	//case 3:
							//	//	heightSegments = (int)( PlantType.SegmentsByMeter.Value / 8.0 * element.Length );
							//	//	thickSegments = 4;
							//	//	break;
							//}

							//if( heightSegments < 1 )
							//	continue;


							//foreach( var trunk in Trunks )
							//{
							//	var tr = trunk.Curve.GetTransformByTimeFactor( 1 );

							//	//trunk.Curve.GetTransformByTime( zzz );

							//}

							//foreach( var branch in Branches )
							//{
							//	//zzzzz;
							//}

							var segments = 10;
							if( LOD >= 1 )
								segments = 6;

							var vertexCount = 1 + segments;
							var indexCount = segments * 3;

							positions = new Vector3F[ vertexCount ];
							normals = new Vector3F[ vertexCount ];
							tangents = new Vector4F[ vertexCount ];
							texCoords = new Vector2F[ vertexCount ];
							indices = new int[ indexCount ];

							var uvFrontPosition = element.Material.UVFrontPosition.Value.ToVector2F();
							var uvRadius = (float)element.Material.UVRadius.Value;

							positions[ 0 ] = Vector3F.Zero;
							normals[ 0 ] = Vector3F.XAxis;
							tangents[ 0 ] = new Vector4F( Vector3F.YAxis, -1 );
							texCoords[ 0 ] = uvFrontPosition;


							var radius = (float)element.Width * 0.5f;

							float step = MathEx.PI * 2 / (float)segments;
							int steps = segments;

							float angle = 0;
							Vector3F lastPosition = Vector3F.Zero;
							Vector2F lastTexCoord = Vector2F.Zero;

							var currentVertex = 1;
							var currentIndex = 0;

							//vertices
							for( int n = 0; n < steps; n++ )
							{

								//!!!!normals, tangents when not a plane

								//!!!!
								var m = Matrix3F.FromRotateByX( -angle ) * Matrix3F.FromRotateByY( ( (float)element.Maturity - 1.0f ) * MathEx.PI / 4 );
								//var m = Matrix3F.FromRotateByX( -angle ) * Matrix3F.FromRotateByY( MathEx.PI / 4 );
								var direction = m.ToQuaternion().GetUp();
								var position = direction * radius;
								//var direction = new SphericalDirectionF( angle, 0 );
								//var position = direction.GetVector() * radius;

								var cossin = new Vector2F( MathEx.Cos( angle ), MathEx.Sin( angle ) );
								//var position = new Vector3F( 0, cossin.X, cossin.Y ) * radius;
								var texCoord = uvFrontPosition + cossin * uvRadius;

								positions[ currentVertex ] = position;
								normals[ currentVertex ] = Vector3F.XAxis;
								tangents[ currentVertex ] = new Vector4F( Vector3F.YAxis, -1 );
								texCoords[ currentVertex ] = texCoord;
								currentVertex++;

								angle += step;
								lastPosition = position;
								lastTexCoord = texCoord;
							}

							//transform vertices
							{
								ref var matrix = ref element.StartTransform.ToMatrix4();
								var matrix3 = matrix.ToMatrix3();

								for( int n = 0; n < positions.Length; n++ )
								{
									positions[ n ] = ( matrix * positions[ n ] ).ToVector3F();
									normals[ n ] = ( matrix3 * normals[ n ] ).ToVector3F();
									tangents[ n ] = new Vector4F( ( matrix3 * tangents[ n ].ToVector3F() ).ToVector3F(), tangents[ n ].W );
								}
							}

							//indices
							for( int n = 0; n < steps; n++ )
							{
								indices[ currentIndex++ ] = 0;
								indices[ currentIndex++ ] = n + 1;
								indices[ currentIndex++ ] = ( n + 1 ) % steps + 1;
							}


							//!!!!UVBack

						}


						var geometry = GetOrCreateGeometry( element.Material != null ? element.Material.Material : null );

						var vertexStartIndex = geometry.Vertices.Count;

						for( int n = 0; n < positions.Length; n++ )
						{
							var vertex = new StandardVertex.StaticOneTexCoord();
							vertex.Position = positions[ n ];
							vertex.Normal = normals[ n ];
							vertex.Tangent = tangents[ n ];
							vertex.Color = new ColorValue( 1, 1, 1, 1 );
							vertex.TexCoord0 = texCoords[ n ];
							geometry.Vertices.Add( ref vertex );

							Bounds.Add( vertex.Position );
						}

						foreach( var index in indices )
							geometry.Indices.Add( index + vertexStartIndex );
					}
				}
			}

			//apply world transform
			if( !WorldTransform.IsIdentity )
			{
				for( int nGeometry = 0; nGeometry < geometries.Count; nGeometry++ )
				{
					var geometry = geometries[ nGeometry ];

					ref var matrix = ref WorldTransform.ToMatrix4();
					var matrix3 = matrix.ToMatrix3();

					for( int n = 0; n < geometry.Vertices.Count; n++ )
					{
						var vertex = geometry.Vertices[ n ];

						vertex.Position = ( matrix * vertex.Position ).ToVector3F();
						vertex.Normal = ( matrix3 * vertex.Normal ).ToVector3F();
						vertex.Tangent = new Vector4F( ( matrix3 * vertex.Tangent.ToVector3F() ).ToVector3F(), vertex.Tangent.W );

						geometry.Vertices[ n ] = vertex;
					}
				}
			}

			for( int nGeometry = 0; nGeometry < geometries.Count; nGeometry++ )
			{
				var geometry = geometries[ nGeometry ];

				if( geometry.Vertices.Count == 0 || geometry.Indices.Count == 0 )
					continue;

				var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

				//convert vertices to byte array
				var vertices = new byte[ vertexSize * geometry.Vertices.Count ];
				unsafe
				{
					fixed( byte* pVertices = vertices )
					{
						var pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;
						for( int n = 0; n < geometry.Vertices.Count; n++ )
							*pVertex++ = geometry.Vertices[ n ];
					}
				}

				//add to result
				int vertexCount = vertices.Length / vertexSize;

				if( WriteToCompiledData != null )
				{
					var op = new RenderingPipeline.RenderSceneData.MeshDataRenderOperation( this, nGeometry );
					op.VertexStructure = vertexStructure;
					op.VertexStructureContainsColor = op.VertexStructure.Any( e => e.Semantic == VertexElementSemantic.Color0 );
					//op.UnwrappedUV = unwrappedUV;

					var vertexDeclaration = op.VertexStructure.CreateVertexDeclaration( 0 );
					op.VertexBuffers = new GpuVertexBuffer[] { GpuBufferManager.CreateVertexBuffer( vertices, vertexDeclaration, GpuBufferFlags.ComputeRead ) };
					op.VertexStartOffset = 0;
					op.VertexCount = vertexCount;

					op.IndexBuffer = GpuBufferManager.CreateIndexBuffer( geometry.Indices.ToArray(), GpuBufferFlags.ComputeRead );
					op.IndexStartOffset = 0;
					op.IndexCount = geometry.Indices.Count;

					op.Material = geometry.Material;

					op.GetMaterialIndexRangesFromVertexDataOrFromVirtualizedData();

					WriteToCompiledData.MeshData.RenderOperations.Add( op );
				}

				if( WriteToMesh != null )
				{
					var meshGeometry = WriteToMesh.CreateComponent<MeshGeometry>();
					meshGeometry.Name = "Geometry " + ( nGeometry + 1 ).ToString();
					meshGeometry.VertexStructure = vertexStructure;
					//meshGeometry.UnwrappedUV = unwrappedUV;
					meshGeometry.Vertices = vertices;
					meshGeometry.Indices = geometry.Indices.ToArray();
					meshGeometry.Material = geometry.Material;

					if( !WriteToMesh.Billboard )
					{
						//merge equal vertices
						{
							var optimizeThreshold = 0.001f;

							var vertexPositionEpsilon = optimizeThreshold;
							var vertexOtherChannelsEpsilon = vertexPositionEpsilon * 5;

							if( meshGeometry.VerticesExtractStandardVertex( out var vertices2, out var vertexComponents ) )
							{
								var indices = meshGeometry.Indices.Value;

								MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( vertices2, indices, 0, vertexPositionEpsilon, vertexOtherChannelsEpsilon, true, true, out var newVertices, out var newIndices, out _ );

								meshGeometry.SetVertexData( newVertices, vertexComponents );
								meshGeometry.Indices = newIndices;
							}
						}

						meshGeometry.CompressVertices();
						meshGeometry.OptimizeVertexCache();
						meshGeometry.OptimizeOverdraw();
						meshGeometry.OptimizeVertexFetch();
					}
				}

				//if( compiledData.MeshData.Structure == null )
				//	compiledData.MeshData.Structure = structure;
			}
		}
	}
}
#endif