// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoAxis
{
	public enum MeshVoxelizationMode
	{
		None,
		Auto,
		Basic,
		//!!!!что бейкится?
		//!!!!Baked
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum MeshVoxelizationSize
	{
		_16,
		_32,
		_64,
		_128,
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	class MeshVoxelization
	{
		public static int GetVoxelizationSizeInteger( MeshVoxelizationSize size )
		{
			switch( size )
			{
			case MeshVoxelizationSize._16: return 16;
			case MeshVoxelizationSize._32: return 32;
			case MeshVoxelizationSize._64: return 64;
			case MeshVoxelizationSize._128: return 128;
			}
			return 32;
		}

		struct VoxelizeMeshGeometryData
		{
			public Vector3F[] VertexPositions;
			public Vector2F[] VertexTexCoords;
			public int[] Indices;
		}

		public unsafe static void Voxelize( Mesh mesh, MeshVoxelizationMode mode, MeshVoxelizationSize sizeEnum )
		{
			var sizeInteger = GetVoxelizationSizeInteger( sizeEnum );
			//!!!!поддержать не кубические. выравнивать или хранить габариты вокселей
			var size = new Vector3I( sizeInteger, sizeInteger, sizeInteger );
			var voxelCount = size.X * size.Y * size.Z;


			var data = new byte[ sizeof( MeshVoxelized.DataHeader ) + voxelCount * 8 ];
			fixed( byte* pDataByte = data )
			{
				var header = (MeshVoxelized.DataHeader*)pDataByte;

				header->Version = 0;
				//!!!!mode
				header->Mode = MeshVoxelized.ModeEnum.Basic;
				header->Size = size;


				var geometries = mesh.GetComponents<MeshGeometry>();
				var geometryDatas = new VoxelizeMeshGeometryData[ geometries.Length ];
				var meshBounds = BoundsF.Cleared;

				for( int n = 0; n < geometries.Length; n++ )
				{
					var geometry = geometries[ n ];
					ref var geometryData = ref geometryDatas[ n ];

					if( geometry.VertexStructure.Value != null && geometry.Vertices.Value != null && geometry.Indices.Value != null )
					{
						var positions = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
						if( positions != null )
						{
							geometryData.VertexPositions = positions;
							meshBounds.Add( positions );
						}

						var texCoords = geometry.VerticesExtractChannel<Vector2F>( VertexElementSemantic.TextureCoordinate0 );
						if( texCoords != null )
							geometryData.VertexTexCoords = texCoords;

						geometryData.Indices = geometry.Indices.Value;
					}
				}

				var boundsMaxSide = meshBounds.GetSize().MaxComponent();
				var cellSize = boundsMaxSide / size.ToVector3F();// .MaxComponent();


				HalfType* pDataBody = (HalfType*)( pDataByte + sizeof( MeshVoxelized.DataHeader ) );

				//clear the grid
				var halfNegative = new HalfType( -1.0f );
				for( int z = 0; z < size.Z; z++ )
				{
					for( int y = 0; y < size.Y; y++ )
					{
						for( int x = 0; x < size.X; x++ )
						{
							var index = ( z * size.X * size.Y ) + ( y * size.X ) + x;
							pDataBody[ index * 4 + 0 ] = halfNegative;
						}
					}
				}

				for( int nGeometry = 0; nGeometry < geometries.Length; nGeometry++ )
				{
					var geometry = geometries[ nGeometry ];
					var geometryData = geometryDatas[ nGeometry ];

					using( var meshTest = new MeshTest( geometryData.VertexPositions, geometryData.Indices ) )
					{
						var indexes = new Vector3I[ voxelCount ];
						{
							var counter = 0;
							for( int z = 0; z < size.Z; z++ )
								for( int y = 0; y < size.Y; y++ )
									for( int x = 0; x < size.X; x++ )
										indexes[ counter++ ] = new Vector3I( x, y, z );
						}

						Parallel.ForEach( indexes, delegate ( Vector3I index3 )
						{
							var x = index3.X;
							var y = index3.Y;
							var z = index3.Z;
							var index = ( z * size.X * size.Y ) + ( y * size.X ) + x;


							var cellBoundsMin = meshBounds.Minimum + new Vector3F( x, y, z ) * cellSize;
							var cellBoundsMax = cellBoundsMin + cellSize;
							var cellCenter = ( cellBoundsMin + cellBoundsMax ) * 0.5f;

							for( int axis = 0; axis < 3; axis++ )
							{
								RayF ray = new RayF();
								switch( axis )
								{
								case 0:
									ray = new RayF( new Vector3F( cellBoundsMin.X, cellCenter.Y, cellCenter.Z ), new Vector3F( cellSize.X, 0, 0 ) );
									break;

								case 1:
									ray = new RayF( new Vector3F( cellCenter.X, cellBoundsMin.Y, cellCenter.Z ), new Vector3F( 0, cellSize.Y, 0 ) );
									break;

								case 2:
									ray = new RayF( new Vector3F( cellCenter.X, cellCenter.Y, cellBoundsMin.Z ), new Vector3F( 0, 0, cellSize.Z ) );
									break;
								}

								//!!!!можно все перебирать
								var resultList = meshTest.RayCast( ray, MeshTest.Mode.OneClosest, true );

								if( resultList != null && resultList.Length != 0 )
								{
									var result = resultList[ 0 ];

									var nTriangle = result.TriangleIndex;

									var index0 = geometryData.Indices[ nTriangle * 3 + 0 ];
									var index1 = geometryData.Indices[ nTriangle * 3 + 1 ];
									var index2 = geometryData.Indices[ nTriangle * 3 + 2 ];

									ref var v0 = ref geometryData.VertexPositions[ index0 ];
									ref var v1 = ref geometryData.VertexPositions[ index1 ];
									ref var v2 = ref geometryData.VertexPositions[ index2 ];

									PlaneF.FromPoints( ref v0, ref v1, ref v2, out var plane );
									var normal = plane.Normal;
									SphericalDirectionF.FromVector( ref normal, out var sphericalDirection );

									var horizontal = sphericalDirection.Horizontal;
									if( horizontal < 0 )
										horizontal += MathEx.PI * 2;
									var vertical = sphericalDirection.Vertical;

									horizontal /= MathEx.PI * 2.0f;
									MathEx.Clamp( ref horizontal, 0, 0.999f );
									vertical /= MathEx.PI * 2.0f;


									//normal
									pDataBody[ index * 4 + 0 ] = new HalfType( horizontal + (float)nGeometry );
									pDataBody[ index * 4 + 1 ] = new HalfType( vertical );

									//texCoord
									if( geometryData.VertexTexCoords != null )
									{
										ref var texCoord0 = ref geometryData.VertexTexCoords[ index0 ];
										ref var texCoord1 = ref geometryData.VertexTexCoords[ index1 ];
										ref var texCoord2 = ref geometryData.VertexTexCoords[ index2 ];

										var p = ray.GetPointOnRay( result.Scale );
										MathAlgorithms.CalculateBarycentricCoordinates( ref v0, ref v1, ref v2, ref p, out var u, out var v, out var w );
										var texCoord = u * texCoord0 + v * texCoord1 + w * texCoord2;
										pDataBody[ index * 4 + 2 ] = new HalfType( texCoord.X );
										pDataBody[ index * 4 + 3 ] = new HalfType( texCoord.Y );
									}

									//!!!!можно лучший найти
									//!!!!про несколько геометрий тоже
									break;
								}
							}

						} );
					}
				}
			}

			var voxelized = mesh.CreateComponent<MeshVoxelized>( enabled: false );
			voxelized.Name = "Voxelized";
			voxelized.Data = data;

			foreach( var geometry in mesh.GetComponents<MeshGeometry>() )
			{
				var element = voxelized.CreateComponent<MeshVoxelizedElement>();
				element.Name = geometry.Name;
				element.Material = geometry.Material;
			}

			voxelized.Enabled = true;
		}
	}
}
