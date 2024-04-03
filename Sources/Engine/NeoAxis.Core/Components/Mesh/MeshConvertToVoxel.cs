// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	class MeshConvertToVoxel
	{
		public Mesh Mesh;
		public int InitialGridSize;
		public double ThinFactor;
		public bool BakeMaterialOpacity;
		public bool OptimizeMaterials;
		public double FillHolesDistance;

		///////////////////////////////////////////////

		class VoxelizeMeshGeometryData
		{
			public Vector3F[] Position;
			public Vector3F[] Normal;
			public Vector4F[] Tangent;
			public Vector2F[] TexCoord0;
			public Vector2F[] TexCoord1;
			public Vector2F[] TexCoord2;
			public ColorValue[] Color0;

			public int[] Indices;

			//for baking opacity
			public Material Material;
			object opacityTextureProcessLock = new object();
			bool opacityTextureProcessed;
			ImageUtility.Image2D opacityTexture;

			//

			public ImageUtility.Image2D GetOpacityTexture()
			{
				lock( opacityTextureProcessLock )
				{
					if( !opacityTextureProcessed )
					{
						opacityTextureProcessed = true;

						if( Material != null )
						{

							//!!!!formula blocks and other blocks support


							Material.Opacity.GetMember( Material, out var outObject, out _ );

							var sample = outObject as ShaderTextureSample;
							if( sample != null )
							{
								var texture = sample.Texture.Value;
								if( texture != null )
								{
									var resourceName = texture.LoadFile.Value?.ResourceName;
									if( !string.IsNullOrEmpty( resourceName ) )
									{
										if( ImageUtility.LoadFromVirtualFile( resourceName, out var data, out var size, out _, out var format, out _, out _, out var error ) )
										{
											opacityTexture = new ImageUtility.Image2D( format, size, data );
										}
										else
										{
											//error
										}
									}
								}
							}
						}
					}
				}

				return opacityTexture;
			}

			//!!!!referenceValue
			//called from multi threads
			public double GetMaterialOpacity( ref VoxelWithData voxelData, string referenceValue )
			{
				var opacityImage = GetOpacityTexture();
				if( opacityImage != null )
				{
					try
					{
						var value = opacityImage.GetPixel( ( voxelData.TexCoord0 * opacityImage.Size.ToVector2F() ).ToVector2I() );

						//!!!!not only texCoord0

						//!!!!

						var channelIndex = 3;

						if( referenceValue.Length > 1 )
						{
							if( referenceValue[ referenceValue.Length - 2 ] == '\\' )
							{
								var c = referenceValue[ referenceValue.Length - 1 ];
								if( c == 'R' )
									channelIndex = 0;
								else if( c == 'G' )
									channelIndex = 1;
								else if( c == 'B' )
									channelIndex = 2;
								else if( c == 'A' )
									channelIndex = 3;
							}
						}

						return value[ channelIndex ];//.W;
					}
					catch
					{
						//!!!!
					}
				}

				return 1;
			}
		}

		enum WhereEnum
		{
			Unknown,
			OnTriangle,
			Inside,
			Outside,
		}

		struct Voxel
		{
			public int DataIndexInListOnTriangle;
			public ulong AffectedDetailVoxels;

			//nGeometry, nTriangle
			public ESet<(int, int)> IntersectedTriangles;
			//public List<int> IntersectedTriangles;

			public int DataIndexInListResult;
			public Vector3I NearestVoxelWithData;

			public WhereEnum Where;
		}

		struct VoxelWithData
		{
			public Vector3I Index;

			public int MaterialIndex;
			public Vector3F Normal;
			public Vector4F Tangent;
			public ColorValue Color0;
			public Vector2F TexCoord0;
			public Vector2F TexCoord1;
			public Vector2F TexCoord2;

			//

			public override int GetHashCode()
			{
				return Index.GetHashCode() ^ MaterialIndex.GetHashCode() ^ Normal.GetHashCode() ^ Tangent.GetHashCode() ^ Color0.GetHashCode() ^ TexCoord0.GetHashCode() ^ TexCoord1.GetHashCode() ^ TexCoord2.GetHashCode();
			}

			public override bool Equals( object obj )
			{
				return ( obj is VoxelWithData && this == (VoxelWithData)obj );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public bool Equals( ref VoxelWithData v, float epsilon )
			{
				if( !Normal.Equals( ref v.Normal, epsilon ) )
					return false;
				if( !Tangent.Equals( ref v.Tangent, epsilon ) )
					return false;
				if( !Color0.Equals( ref v.Color0, epsilon ) )
					return false;
				if( !TexCoord0.Equals( ref v.TexCoord0, epsilon ) )
					return false;
				if( !TexCoord1.Equals( ref v.TexCoord1, epsilon ) )
					return false;
				if( !TexCoord2.Equals( ref v.TexCoord2, epsilon ) )
					return false;
				return true;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public static bool operator ==( VoxelWithData v1, VoxelWithData v2 )
			{
				return
					v1.Index == v2.Index &&
					v1.MaterialIndex == v2.MaterialIndex &&
					v1.Normal == v2.Normal &&
					v1.Tangent == v2.Tangent &&
					v1.Color0 == v2.Color0 &&
					v1.TexCoord0 == v2.TexCoord0 &&
					v1.TexCoord1 == v2.TexCoord1 &&
					v1.TexCoord2 == v2.TexCoord2;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public static bool operator !=( VoxelWithData v1, VoxelWithData v2 )
			{
				return
					v1.Index != v2.Index ||
					v1.MaterialIndex != v2.MaterialIndex ||
					v1.Normal != v2.Normal ||
					v1.Tangent != v2.Tangent ||
					v1.Color0 != v2.Color0 ||
					v1.TexCoord0 != v2.TexCoord0 ||
					v1.TexCoord1 != v2.TexCoord1 ||
					v1.TexCoord2 != v2.TexCoord2;
			}
		}

		class VoxelWithDataSector
		{
			public OpenList<Vector3I> Voxels = new OpenList<Vector3I>( 512 );
			public BoundsF Bounds = BoundsF.Cleared;
		}

		///////////////////////////////////////////////

		internal static int GetNearestVoxelWithDataIndex256( ref Vector3I index3D )
		{
			var index = ( index3D.Z * 256 * 256 ) + ( index3D.Y * 256 ) + index3D.X;
			return index;
		}

		//static int GetIndexInVoxelData( ref Vector3I size, int x, int y, int z )
		//{
		//	var index = ( z * size.X * size.Y ) + ( y * size.X ) + x;
		//	return index;
		//}

		//static int GetIndexInVoxelData( ref Vector3I size, ref Vector3I index3D )
		//{
		//	return GetIndexInVoxelData( ref size, index3D.X, index3D.Y, index3D.Z );
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		static ulong FindAffectedDetailVoxels( ref BoundsF bounds, ref TriangleF triangle )
		{
			ulong result = 0UL;

			var cellSize = bounds.GetSize() / 4;

			for( int z = 0; z < 4; z++ )
			{
				for( int y = 0; y < 4; y++ )
				{
					for( int x = 0; x < 4; x++ )
					{
						var min = bounds.Minimum + cellSize * new Vector3F( x, y, z );
						var max = bounds.Minimum + cellSize * new Vector3F( x + 1, y + 1, z + 1 );
						var b = new BoundsF( min, max );

						var bExpanded = b;
						bExpanded.Expand( cellSize * 0.001f );

						if( bExpanded.Intersects( ref triangle ) )
						{
							var index = ( z * 4 + y ) * 4 + x;

							var affectedBit = 1UL << index;
							result |= affectedBit;
						}
					}
				}
			}

			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static bool IsBitEnabled( ulong value, int index )
		{
			return ( value & ( 1UL << index ) ) != 0;
		}

		static float[,,] thinPattern;
		static float[,,] ThinPattern
		{
			get
			{
				if( thinPattern == null )
				{
					thinPattern = new float[ 2, 2, 2 ];

					var step = 1.0f / 8.0f;
					var current = step / 2;
					thinPattern[ 0, 0, 0 ] = current;
					current += step;
					thinPattern[ 1, 1, 1 ] = current;
					current += step;
					thinPattern[ 0, 1, 0 ] = current;
					current += step;
					thinPattern[ 1, 0, 1 ] = current;
					current += step;
					thinPattern[ 0, 0, 1 ] = current;
					current += step;
					thinPattern[ 1, 1, 0 ] = current;
					current += step;
					thinPattern[ 0, 1, 1 ] = current;
					current += step;
					thinPattern[ 1, 0, 0 ] = current;
				}

				return thinPattern;
			}
		}

		//private static Random rng = new Random();

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static void Shuffle<T>( T[] array, FastRandom random )
		{
			int n = array.Length;
			while( n > 1 )
			{
				n--;
				int k = random.Next( n + 1 );
				T value = array[ k ];
				array[ k ] = array[ n ];
				array[ n ] = value;
			}
		}

		class ReduceVoxelPositionItem
		{
			public List<Vector3F> Normals = new List<Vector3F>();
			public Vector3F ResultOffset;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe byte[] CreateVoxelData( MeshGeometry[] geometries, out BoundsF meshBoundsOut )
		{
			meshBoundsOut = BoundsF.Cleared;

			try
			{
				var random = new FastRandom( 0 );

				byte[] resultData;

				//detect format
				var fullFormat = false;
				var transparent = false;
				{
					foreach( var geometry in geometries )
					{
						var structure = geometry.VertexStructure.Value;
						if( structure != null )
						{
							if( structure.GetElementBySemantic( VertexElementSemantic.Color0, out _ ) )
								fullFormat = true;
							else if( structure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate1, out _ ) )
								fullFormat = true;
							else if( structure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate2, out _ ) )
								fullFormat = true;
						}

						if( BakeMaterialOpacity )
						{
							var material = geometry.Material.Value;
							if( material != null )
							{
								var blendMode = material.BlendMode.Value;
								if( blendMode == Material.BlendModeEnum.Masked || blendMode == Material.BlendModeEnum.Transparent )
								{
									fullFormat = true;
									transparent = true;
								}
							}
						}
					}
				}
				VoxelFormatEnum format = 0;
				//if( transparent )
				//	format = VoxelFormatEnum.FullTransparent;
				//else
				if( fullFormat )
					format |= VoxelFormatEnum.Full;
				//else
				//	format = VoxelFormatEnum.Basic;
				if( BakeMaterialOpacity && transparent )
					format |= VoxelFormatEnum.BakedOpacity;


				MeshTest meshTest = null;

				try
				{
					//get geometry data

					var geometryDatas = new VoxelizeMeshGeometryData[ geometries.Length ];
					var meshBounds = BoundsF.Cleared;

					for( int n = 0; n < geometries.Length; n++ )
					{
						var geometry = geometries[ n ];

						var geometryData = new VoxelizeMeshGeometryData();
						geometryDatas[ n ] = geometryData;

						geometryData.Position = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
						geometryData.Normal = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Normal );
						geometryData.Tangent = geometry.VerticesExtractChannel<Vector4F>( VertexElementSemantic.Tangent );
						geometryData.TexCoord0 = geometry.VerticesExtractChannel<Vector2F>( VertexElementSemantic.TextureCoordinate0 );
						geometryData.TexCoord1 = geometry.VerticesExtractChannel<Vector2F>( VertexElementSemantic.TextureCoordinate1 );
						geometryData.TexCoord2 = geometry.VerticesExtractChannel<Vector2F>( VertexElementSemantic.TextureCoordinate2 );
						geometryData.Color0 = geometry.VerticesExtractChannel<ColorValue>( VertexElementSemantic.Color0 );
						geometryData.Indices = geometry.Indices.Value;
						geometryData.Material = geometry.Material;

						meshBounds.Add( geometryData.Position );
					}


					////портит геометрию. внутренние части вылазят наружу
					////decrease mesh geometry to half of cell size
					//if( !meshBounds.IsCleared() && EngineApp._DebugCapsLock )
					//{
					//	var maxSide2 = meshBounds.GetSize().MaxComponent();
					//	var estimatedCellSize = maxSide2 / InitialGridSize;

					//	var initSettings = new OctreeContainer.InitSettings();
					//	initSettings.InitialOctreeBounds = meshBounds.GetExpanded( 0.001f );
					//	initSettings.MinNodeSize = meshBounds.GetSize() / 50;
					//	using( var octree = new OctreeContainer( initSettings ) )
					//	{
					//		var positionItems = new List<ReduceVoxelPositionItem>( 8192 );

					//		//collect triangle normals in position items
					//		for( int nGeometry = 0; nGeometry < geometries.Length; nGeometry++ )
					//		{
					//			var geometryData = geometryDatas[ nGeometry ];

					//			for( int tri = 0; tri < geometryData.Indices.Length / 3; tri++ )
					//			{
					//				var position0 = geometryData.Position[ geometryData.Indices[ tri * 3 + 0 ] ];
					//				var position1 = geometryData.Position[ geometryData.Indices[ tri * 3 + 1 ] ];
					//				var position2 = geometryData.Position[ geometryData.Indices[ tri * 3 + 2 ] ];

					//				var triangleNormal = MathAlgorithms.CalculateTriangleNormal( position0, position1, position2 );

					//				var positions = new Vector3F[] { position0, position1, position2 };
					//				foreach( var position in positions )
					//				{
					//					var b = new Bounds( position ).GetExpanded( 0.0001 );
					//					var objects = octree.GetObjects( b, 1, OctreeContainer.ModeEnum.One );
					//					if( objects.Length != 0 )
					//					{
					//						var objectIndex = objects[ 0 ];
					//						var positionItem = positionItems[ objectIndex ];
					//						positionItem.Normals.Add( triangleNormal );
					//					}
					//					else
					//					{
					//						var objectIndex = octree.AddObject( new Bounds( position ), 1 );
					//						var positionItem = new ReduceVoxelPositionItem();
					//						positionItems.Add( positionItem );
					//						positionItem.Normals.Add( triangleNormal );
					//					}
					//				}
					//			}
					//		}

					//		//calculate result normals
					//		foreach( var positionItem in positionItems )
					//		{
					//			var b = BoundsF.Cleared;
					//			foreach( var normal in positionItem.Normals )
					//				b.Add( -normal );

					//			var center = b.GetCenter();
					//			if( center != Vector3F.Zero )
					//				positionItem.ResultOffset = center.GetNormalize() * estimatedCellSize * 0.5f * 1.2f;// 1.41f;


					//			//var v = Vector3F.Zero;
					//			//foreach( var normal in positionItem.Normals )
					//			//	v += -normal;

					//			//var center = v / positionItem.Normals.Count;
					//			//if( center != Vector3F.Zero )
					//			//	positionItem.ResultOffset = center.GetNormalize() * estimatedCellSize * 0.5f * 1.2f;// 1.41f;
					//		}

					//		//apply offset to positions
					//		for( int nGeometry = 0; nGeometry < geometries.Length; nGeometry++ )
					//		{
					//			var geometryData = geometryDatas[ nGeometry ];

					//			for( int nPosition = 0; nPosition < geometryData.Position.Length; nPosition++ )
					//			{
					//				var position = geometryData.Position[ nPosition ];

					//				var b = new Bounds( position ).GetExpanded( 0.0001 );
					//				var objects = octree.GetObjects( b, 1, OctreeContainer.ModeEnum.One );
					//				if( objects.Length != 0 )
					//				{
					//					var objectIndex = objects[ 0 ];
					//					var positionItem = positionItems[ objectIndex ];
					//					geometryData.Position[ nPosition ] += positionItem.ResultOffset;
					//				}
					//			}
					//		}
					//	}

					//	//calculate new meshBounds
					//	{
					//		meshBounds = BoundsF.Cleared;
					//		foreach( var geometryData in geometryDatas )
					//			meshBounds.Add( geometryData.Position );
					//	}
					//}


					//epsilon bounds expand
					if( !meshBounds.IsCleared() )
						meshBounds.Expand( meshBounds.GetSize() * 0.001f );

					meshBoundsOut = meshBounds;

					//merge all geometries
					Vector3F[] allVertices;
					int[] allIndices;
					(int, int)[] meshTestItemByTriangle;
					{
						var totalVertexCount = 0;
						var totalIndexCount = 0;
						for( int nGeometry = 0; nGeometry < geometries.Length; nGeometry++ )
						{
							var geometryData = geometryDatas[ nGeometry ];

							totalVertexCount += geometryData.Position.Length;
							totalIndexCount += geometryData.Indices.Length;
						}

						allVertices = new Vector3F[ totalVertexCount ];
						allIndices = new int[ totalIndexCount ];
						meshTestItemByTriangle = new (int, int)[ allIndices.Length / 3 ];

						var currentVertex = 0;
						var currentIndex = 0;
						var currentTriangle = 0;

						for( int nGeometry = 0; nGeometry < geometries.Length; nGeometry++ )
						{
							var geometryData = geometryDatas[ nGeometry ];

							var startVertex = currentVertex;

							geometryData.Position.CopyTo( allVertices, currentVertex );
							currentVertex += geometryData.Position.Length;

							for( int tri = 0; tri < geometryData.Indices.Length / 3; tri++ )
							{
								allIndices[ currentIndex++ ] = geometryData.Indices[ tri * 3 + 0 ] + startVertex;
								allIndices[ currentIndex++ ] = geometryData.Indices[ tri * 3 + 1 ] + startVertex;
								allIndices[ currentIndex++ ] = geometryData.Indices[ tri * 3 + 2 ] + startVertex;

								meshTestItemByTriangle[ currentTriangle++ ] = (nGeometry, tri);
							}

							//foreach( var index in geometryData.Indices )
							//	allIndices[ currentIndex++ ] = index + startVertex;
						}
					}

					meshTest = new MeshTest( allVertices, allIndices );


					var maxSide = meshBounds.GetSize().MaxComponent();
					var cellSize = maxSide / InitialGridSize;

					var size = ( meshBounds.GetSize() / maxSide * InitialGridSize ).ToVector3I() + new Vector3I( 1, 1, 1 );
					if( size.X > InitialGridSize ) size.X = InitialGridSize;
					if( size.Y > InitialGridSize ) size.Y = InitialGridSize;
					if( size.Z > InitialGridSize ) size.Z = InitialGridSize;


					var voxelCount = size.X * size.Y * size.Z;

					//clear grid
					var voxels = new Voxel[ size.X, size.Y, size.Z ];
					for( int z = 0; z < size.Z; z++ )
					{
						for( int y = 0; y < size.Y; y++ )
						{
							for( int x = 0; x < size.X; x++ )
							{
								ref var voxel = ref voxels[ x, y, z ];

								voxel.DataIndexInListOnTriangle = -1;
								voxel.DataIndexInListResult = -1;
								voxel.NearestVoxelWithData = new Vector3I( -1, -1, -1 );
							}
						}
					}

					var voxelIndexes = new Vector3I[ voxelCount ];
					{
						var counter = 0;
						for( int z = 0; z < size.Z; z++ )
							for( int y = 0; y < size.Y; y++ )
								for( int x = 0; x < size.X; x++ )
									voxelIndexes[ counter++ ] = new Vector3I( x, y, z );
					}


					//rasterize geometry to voxel grid
					for( int nGeometry = 0; nGeometry < geometries.Length; nGeometry++ )
					{
						var geometry = geometries[ nGeometry ];
						var geometryData = geometryDatas[ nGeometry ];

						var lockIntersectedTriangles = new object();

						var triangleCount = geometryData.Indices.Length / 3;

						//preload opacity because loading can freeze
						geometryData.GetOpacityTexture();

						Parallel.For( 0, triangleCount, delegate ( int nTriangle )
						{
							var index0 = geometryData.Indices[ nTriangle * 3 + 0 ];
							var index1 = geometryData.Indices[ nTriangle * 3 + 1 ];
							var index2 = geometryData.Indices[ nTriangle * 3 + 2 ];

							ref var v0 = ref geometryData.Position[ index0 ];
							ref var v1 = ref geometryData.Position[ index1 ];
							ref var v2 = ref geometryData.Position[ index2 ];

							var triangleBounds = new BoundsF( v0 );
							triangleBounds.Add( ref v1 );
							triangleBounds.Add( ref v2 );


							//get grid position

							var gridPositionMin = ( triangleBounds.Minimum - meshBounds.Minimum ) / cellSize;
							var gridPositionMax = ( triangleBounds.Maximum - meshBounds.Minimum ) / cellSize;

							var gridIndexMin = gridPositionMin.ToVector3I();
							var gridIndexMax = gridPositionMax.ToVector3I() + Vector3I.One;

							for( int z = gridIndexMin.Z; z <= gridIndexMax.Z && z < size.Z; z++ )
							{
								for( int y = gridIndexMin.Y; y <= gridIndexMax.Y && y < size.Y; y++ )
								{
									for( int x = gridIndexMin.X; x <= gridIndexMax.X && x < size.X; x++ )
									{
										//get cell bounds
										var bMin = meshBounds.Minimum + cellSize * new Vector3F( x, y, z );
										var cellBounds = new BoundsF( bMin, bMin + new Vector3F( cellSize, cellSize, cellSize ) );

										var cellBoundsExpanded = cellBounds;
										cellBoundsExpanded.Expand( cellSize * 0.001f );

										var triangle = new TriangleF( v0, v1, v2 );

										var intersects = cellBoundsExpanded.Intersects( ref triangle );
										if( intersects )
										{
											var affectedDetailVoxels = FindAffectedDetailVoxels( ref cellBounds, ref triangle );
											if( affectedDetailVoxels != 0UL )
											{
												var add = true;

												if( BakeMaterialOpacity && geometryData.Material != null )
												{
													var material = geometryData.Material;

													var blendMode = material.BlendMode.Value;
													if( blendMode == Material.BlendModeEnum.Masked || blendMode == Material.BlendModeEnum.Transparent )
													{

														//calculate only TexCoord0 for opacity

														var voxelData = new VoxelWithData();
														//voxelData.Index = new Vector3I( x, y, z );

														var pointOnTriangle = cellBounds.GetCenter();

														MathAlgorithms.CalculateBarycentricCoordinates( ref v0, ref v1, ref v2, ref pointOnTriangle, out var u, out var v, out var w );

														//voxelData.MaterialIndex = nGeometry;

														////normal
														//if( geometryData.Normal != null )
														//{
														//	ref var normal0 = ref geometryData.Normal[ index0 ];
														//	ref var normal1 = ref geometryData.Normal[ index1 ];
														//	ref var normal2 = ref geometryData.Normal[ index2 ];
														//	voxelData.Normal = ( u * normal0 + v * normal1 + w * normal2 ).GetNormalize();
														//}
														//else
														//{
														//	PlaneF.FromPoints( ref v0, ref v1, ref v2, out var plane );
														//	voxelData.Normal = plane.Normal;
														//}

														////tangent
														//if( geometryData.Tangent != null )
														//{
														//	ref var t0 = ref geometryData.Tangent[ index0 ];
														//	ref var t1 = ref geometryData.Tangent[ index1 ];
														//	ref var t2 = ref geometryData.Tangent[ index2 ];
														//	var tangent3 = ( u * t0.ToVector3F() + v * t1.ToVector3F() + w * t2.ToVector3F() ).GetNormalize();
														//	voxelData.Tangent = new Vector4F( tangent3, t0.W );
														//}

														//texCoord0
														if( geometryData.TexCoord0 != null )
														{
															ref var texCoord0 = ref geometryData.TexCoord0[ index0 ];
															ref var texCoord1 = ref geometryData.TexCoord0[ index1 ];
															ref var texCoord2 = ref geometryData.TexCoord0[ index2 ];
															voxelData.TexCoord0 = u * texCoord0 + v * texCoord1 + w * texCoord2;
														}

														////texCoord1
														//if( geometryData.TexCoord1 != null )
														//{
														//	ref var texCoord0 = ref geometryData.TexCoord1[ index0 ];
														//	ref var texCoord1 = ref geometryData.TexCoord1[ index1 ];
														//	ref var texCoord2 = ref geometryData.TexCoord1[ index2 ];
														//	voxelData.TexCoord1 = u * texCoord0 + v * texCoord1 + w * texCoord2;
														//}

														////texCoord2
														//if( geometryData.TexCoord2 != null )
														//{
														//	ref var texCoord0 = ref geometryData.TexCoord2[ index0 ];
														//	ref var texCoord1 = ref geometryData.TexCoord2[ index1 ];
														//	ref var texCoord2 = ref geometryData.TexCoord2[ index2 ];
														//	voxelData.TexCoord2 = u * texCoord0 + v * texCoord1 + w * texCoord2;
														//}

														////color
														//if( geometryData.Color0 != null )
														//{
														//	ref var color0 = ref geometryData.Color0[ index0 ];
														//	ref var color1 = ref geometryData.Color0[ index1 ];
														//	ref var color2 = ref geometryData.Color0[ index2 ];
														//	voxelData.Color0 = u * color0 + v * color1 + w * color2;
														//}
														//else
														//	voxelData.Color0 = ColorValue.One;

														double opacity = 1;

														if( !material.Opacity.ReferenceSpecified )
														{
															opacity = material.Opacity.Value;

															////apply pattern
															//if( blendMode == Material.BlendModeEnum.Transparent && opacity > 0 && opacity < 1 )
															//{
															//	//x, y, z


															//	var c = 0.0;
															//	if( x % 2 == 1 )
															//		c += 1.0 / 3.0;
															//	if( y % 2 == 1 )
															//		c += 1.0 / 3.0;
															//	if( z % 2 == 0 )
															//		c += 1.0 / 3.0;

															//	//!!!!1.5. без этого часто стекло будет совсем без вокселей (пустым)
															//	opacity *= c * 1.5;



															//	////Log.Info( opacity.ToString() + " " + c.ToString() );

															//	////opacity *= c;

															//	////var xx = x % 2 == 0;
															//	////var yy = y % 2 == 0;
															//	////var zz = z % 2 == 0;

															//	////if( z % 2 == 0 )
															//	////	opacity = 1;
															//	////else
															//	////	opacity = 0;

															//}
														}
														else
														{
															//!!!!material.Opacity.GetByReference
															opacity = geometryData.GetMaterialOpacity( ref voxelData, material.Opacity.GetByReference );
														}

														if( blendMode == Material.BlendModeEnum.Masked )
														{
															if( opacity < material.OpacityMaskThreshold.Value )
																add = false;
														}
														else
														{
															//!!!!new
															if( opacity < 0.001 ) //if( opacity < 0.5 )
																add = false;
														}
													}
												}

												if( add )
												{
													lock( lockIntersectedTriangles )
													{
														ref var voxel = ref voxels[ x, y, z ];

														voxel.Where = WhereEnum.OnTriangle;
														voxel.AffectedDetailVoxels |= affectedDetailVoxels;

														if( voxel.IntersectedTriangles == null )
															voxel.IntersectedTriangles = new ESet<(int, int)>();
														voxel.IntersectedTriangles.Add( (nGeometry, nTriangle) );
													}
												}
											}
										}
									}
								}
							}
						} );
					}

					//calculate Outside, Inside
					{
						//get border voxels
						var startIndexes = new List<Vector3I>( InitialGridSize * 12 );
						foreach( var index in voxelIndexes )
						{
							if( index.X == 0 || index.X == size.X - 1 || index.Y == 0 || index.Y == size.Y - 1 || index.Z == 0 || index.Z == size.Z - 1 )
								startIndexes.Add( index );
						}

						var indexesToCheck = new Queue<Vector3I>( startIndexes );

						while( indexesToCheck.Count != 0 )
						{
							var indexToCheck = indexesToCheck.Dequeue();

							var index = indexToCheck;
							ref var voxel = ref voxels[ index.X, index.Y, index.Z ];

							if( voxel.Where == WhereEnum.Unknown )
							{
								voxel.Where = WhereEnum.Outside;

								var next = new List<Vector3I>();
								if( index.X > 0 )
									indexesToCheck.Enqueue( index + new Vector3I( -1, 0, 0 ) );
								if( index.X < size.X - 1 )
									indexesToCheck.Enqueue( index + new Vector3I( 1, 0, 0 ) );
								if( index.Y > 0 )
									indexesToCheck.Enqueue( index + new Vector3I( 0, -1, 0 ) );
								if( index.Y < size.Y - 1 )
									indexesToCheck.Enqueue( index + new Vector3I( 0, 1, 0 ) );
								if( index.Z > 0 )
									indexesToCheck.Enqueue( index + new Vector3I( 0, 0, -1 ) );
								if( index.Z < size.Z - 1 )
									indexesToCheck.Enqueue( index + new Vector3I( 0, 0, 1 ) );
							}
						}

						//all Unknown it is Inside
						foreach( var index in voxelIndexes )
						{
							ref var voxel = ref voxels[ index.X, index.Y, index.Z ];
							if( voxel.Where == WhereEnum.Unknown )
								voxel.Where = WhereEnum.Inside;
						}
					}

					//calculate vertex data for OnTriangle voxels and voxelsWithDataOnTriangle
					var voxelsWithDataOnTriangle = new OpenList<VoxelWithData>( size.X * size.Y * size.Z / 20 );


					Parallel.For( 0, voxelIndexes.Length, delegate ( int indexParallel ) //foreach( var index in voxelIndexes )
					{
						var index = voxelIndexes[ indexParallel ];

						ref var voxel = ref voxels[ index.X, index.Y, index.Z ];

						if( voxel.Where == WhereEnum.OnTriangle )
						{
							if( voxel.DataIndexInListOnTriangle != -1 )
								Log.Fatal( "MeshConvertToVoxel: CreateVoxelData: Internal error. voxel.DataIndexInListOnTriangle != -1." );

							//try to select best triangle


							//делаем рейкаст со всех сторон до ячейки. выбрали найденные треугольники
							//из них выбрать тот, у которого точка пересечения ближе всех


							//!!!!с обратной стороны отсекать


							//!!!!TwoSided


							//get cell bounds
							var bMin = meshBounds.Minimum + cellSize * index.ToVector3F();
							var cellBounds = new BoundsF( bMin, bMin + new Vector3F( cellSize, cellSize, cellSize ) );

							var cellCenter = cellBounds.GetCenter();


							var bestTriangleFound = false;
							var bestTriangleItem = (0, 0);
							var bestTriangleIntersection = Vector3F.Zero;
							var bestTriangleScale = 0.0;
							//var bestTriangleDistanceFromCenterSquared = 0.0;


							for( var h = 0.0f; h <= MathEx.PI * 2 + 0.001f; h += MathEx.PI / 4 )
							{
								for( var v = -MathEx.PI / 2; v <= MathEx.PI / 2 + 0.001f; v += MathEx.PI / 4 )
								{
									var directionFromCell = new SphericalDirectionF( h, v ).GetVector();

									var nearCellIndex = index;
									{
										var bestValue = -1.0f;

										if( directionFromCell.X > 0 && directionFromCell.X > bestValue )
										{
											nearCellIndex = index + new Vector3I( 1, 0, 0 );
											bestValue = directionFromCell.X;
										}
										if( directionFromCell.Y > 0 && directionFromCell.Y > bestValue )
										{
											nearCellIndex = index + new Vector3I( 0, 1, 0 );
											bestValue = directionFromCell.Y;
										}
										if( directionFromCell.Z > 0 && directionFromCell.Z > bestValue )
										{
											nearCellIndex = index + new Vector3I( 0, 0, 1 );
											bestValue = directionFromCell.Z;
										}
										if( directionFromCell.X < 0 && -directionFromCell.X > bestValue )
										{
											nearCellIndex = index + new Vector3I( -1, 0, 0 );
											bestValue = -directionFromCell.X;
										}
										if( directionFromCell.Y < 0 && -directionFromCell.Y > bestValue )
										{
											nearCellIndex = index + new Vector3I( 0, -1, 0 );
											bestValue = -directionFromCell.Y;
										}
										if( directionFromCell.Z < 0 && -directionFromCell.Z > bestValue )
										{
											nearCellIndex = index + new Vector3I( 0, 0, -1 );
											bestValue = -directionFromCell.Z;
										}

										if( bestValue < 0 )
											nearCellIndex = new Vector3I( 1, 0, 0 );
									}

									var nearVoxelExists =
										nearCellIndex.X >= 0 && nearCellIndex.X < size.X &&
										nearCellIndex.Y >= 0 && nearCellIndex.Y < size.Y &&
										nearCellIndex.Z >= 0 && nearCellIndex.Z < size.Z;

									if( !nearVoxelExists || voxels[ nearCellIndex.X, nearCellIndex.Y, nearCellIndex.Z ].Where != WhereEnum.Inside )
									{
										var rayLength = (float)maxSide * 10;
										var ray = new RayF( cellCenter + directionFromCell * rayLength * 0.5f, -directionFromCell * rayLength );


										//!!!!прозрачные пропускать?


										//!!!!twoSided


										var rayCastResults = meshTest.RayCast( ray, MeshTest.Mode.OneClosest, false, 64 );

										foreach( var rayCastResult in rayCastResults )
										{
											var item = meshTestItemByTriangle[ rayCastResult.TriangleIndex ];

											var containsIntersected = voxel.IntersectedTriangles.Contains( item );
											if( containsIntersected )
											{
												ray.GetPointOnRay( rayCastResult.Scale, out var p );

												if( cellBounds.Contains( ref p ) )
												{
													if( !bestTriangleFound || rayCastResult.Scale > bestTriangleScale )
													{
														bestTriangleFound = true;
														bestTriangleItem = item;
														bestTriangleIntersection = p;
														bestTriangleScale = rayCastResult.Scale;
													}

													//var distanceFromCenterSquared = ( cellCenter - p ).LengthSquared();

													//if( !bestTriangleFound || distanceFromCenterSquared > bestTriangleDistanceFromCenterSquared )
													//{
													//	bestTriangleFound = true;
													//	bestTriangleItem = item;
													//	bestTriangleIntersection = p;
													//	bestTriangleDistanceFromCenterSquared = distanceFromCenterSquared;
													//}
												}
											}
										}
									}
								}
							}

							if( !bestTriangleFound )
							{
								bestTriangleFound = true;
								bestTriangleItem = voxel.IntersectedTriangles.First();// voxel.IntersectedTriangles[ 0 ];
								bestTriangleIntersection = cellCenter;
							}


							{
								var nGeometry = bestTriangleItem.Item1;
								var nTriangle = bestTriangleItem.Item2;

								var geometryData = geometryDatas[ nGeometry ];
								var index0 = geometryData.Indices[ nTriangle * 3 + 0 ];
								var index1 = geometryData.Indices[ nTriangle * 3 + 1 ];
								var index2 = geometryData.Indices[ nTriangle * 3 + 2 ];

								var v0 = geometryData.Position[ index0 ];
								var v1 = geometryData.Position[ index1 ];
								var v2 = geometryData.Position[ index2 ];


								var voxelData = new VoxelWithData();
								voxelData.Index = index;

								var pointOnTriangle = bestTriangleIntersection;
								//var pointOnTriangle = cellCenter;

								MathAlgorithms.CalculateBarycentricCoordinates( ref v0, ref v1, ref v2, ref pointOnTriangle, out var u, out var v, out var w );

								voxelData.MaterialIndex = nGeometry;

								//normal
								if( geometryData.Normal != null )
								{
									ref var normal0 = ref geometryData.Normal[ index0 ];
									ref var normal1 = ref geometryData.Normal[ index1 ];
									ref var normal2 = ref geometryData.Normal[ index2 ];
									voxelData.Normal = ( u * normal0 + v * normal1 + w * normal2 ).GetNormalize();
								}
								else
								{
									PlaneF.FromPoints( ref v0, ref v1, ref v2, out var plane );
									voxelData.Normal = plane.Normal;
								}

								//tangent
								if( geometryData.Tangent != null )
								{
									ref var t0 = ref geometryData.Tangent[ index0 ];
									ref var t1 = ref geometryData.Tangent[ index1 ];
									ref var t2 = ref geometryData.Tangent[ index2 ];
									var tangent3 = ( u * t0.ToVector3F() + v * t1.ToVector3F() + w * t2.ToVector3F() ).GetNormalize();
									voxelData.Tangent = new Vector4F( tangent3, t0.W );
								}

								//texCoord0
								if( geometryData.TexCoord0 != null )
								{
									ref var texCoord0 = ref geometryData.TexCoord0[ index0 ];
									ref var texCoord1 = ref geometryData.TexCoord0[ index1 ];
									ref var texCoord2 = ref geometryData.TexCoord0[ index2 ];
									voxelData.TexCoord0 = u * texCoord0 + v * texCoord1 + w * texCoord2;
								}

								//texCoord1
								if( geometryData.TexCoord1 != null )
								{
									ref var texCoord0 = ref geometryData.TexCoord1[ index0 ];
									ref var texCoord1 = ref geometryData.TexCoord1[ index1 ];
									ref var texCoord2 = ref geometryData.TexCoord1[ index2 ];
									voxelData.TexCoord1 = u * texCoord0 + v * texCoord1 + w * texCoord2;
								}

								//texCoord2
								if( geometryData.TexCoord2 != null )
								{
									ref var texCoord0 = ref geometryData.TexCoord2[ index0 ];
									ref var texCoord1 = ref geometryData.TexCoord2[ index1 ];
									ref var texCoord2 = ref geometryData.TexCoord2[ index2 ];
									voxelData.TexCoord2 = u * texCoord0 + v * texCoord1 + w * texCoord2;
								}

								//color
								if( geometryData.Color0 != null )
								{
									ref var color0 = ref geometryData.Color0[ index0 ];
									ref var color1 = ref geometryData.Color0[ index1 ];
									ref var color2 = ref geometryData.Color0[ index2 ];
									voxelData.Color0 = u * color0 + v * color1 + w * color2;
								}
								else
									voxelData.Color0 = ColorValue.One;

								//bake opacity to Color0
								if( BakeMaterialOpacity && geometryData.Material != null )
								{
									var opacity = 1.0;

									var material = geometryData.Material;

									var blendMode = material.BlendMode.Value;
									if( blendMode == Material.BlendModeEnum.Masked || blendMode == Material.BlendModeEnum.Transparent )
									{
										if( !material.Opacity.ReferenceSpecified )
										{
											opacity = material.Opacity.Value;
										}
										else
										{
											//!!!!material.Opacity.GetByReference
											opacity = geometryData.GetMaterialOpacity( ref voxelData, material.Opacity.GetByReference );
										}

										//if( blendMode == Material.BlendModeEnum.Masked )
										//{
										//	if( opacity < material.OpacityMaskThreshold.Value )
										//		add = false;
										//}
										//else
										//{
										//	//!!!!new
										//	if( opacity < 0.01 ) //if( opacity < 0.5 )
										//		add = false;
										//}
									}

									voxelData.Color0.Alpha *= (float)opacity;
								}

								lock( voxelsWithDataOnTriangle )
								{
									var dataIndex = voxelsWithDataOnTriangle.Count;
									voxelsWithDataOnTriangle.Add( ref voxelData );

									voxel.DataIndexInListOnTriangle = dataIndex;
								}
							}
						}
					} );


					var voxelsWithDataResult = new OpenList<VoxelWithData>( size.X * size.Y * size.Z / 20 );
					foreach( var index in voxelIndexes )
					{
						ref var voxel = ref voxels[ index.X, index.Y, index.Z ];

						if( voxel.Where == WhereEnum.Inside )
						{
							var voxelData = new VoxelWithData();
							voxelData.Index = new Vector3I( index.X, index.Y, index.Z );

							var found = false;
							var foundMinDistanceSquared = float.MaxValue;

							//find nearest OnTriangle voxels
							for( int z = -1; z <= 1; z++ )
							{
								for( int y = -1; y <= 1; y++ )
								{
									for( int x = -1; x <= 1; x++ )
									{
										if( x == 0 && y == 0 && z == 0 )
											continue;

										var offset = new Vector3I( x, y, z );
										var indexToCheck = index + offset;

										if( indexToCheck.X >= 0 && indexToCheck.X < size.X &&
											indexToCheck.Y >= 0 && indexToCheck.Y < size.Y &&
											indexToCheck.Z >= 0 && indexToCheck.Z < size.Z )
										{
											ref var voxel2 = ref voxels[ indexToCheck.X, indexToCheck.Y, indexToCheck.Z ];

											if( voxel2.Where == WhereEnum.OnTriangle )
											{
												var distanceSquared = offset.ToVector3F().LengthSquared();
												if( distanceSquared < foundMinDistanceSquared )
												{
													found = true;
													foundMinDistanceSquared = distanceSquared;

													ref var voxelData2 = ref voxelsWithDataOnTriangle.Data[ voxel2.DataIndexInListOnTriangle ];

													voxelData.MaterialIndex = voxelData2.MaterialIndex;
													voxelData.Normal = voxelData2.Normal;
													voxelData.Tangent = voxelData2.Tangent;
													voxelData.Color0 = voxelData2.Color0;
													voxelData.TexCoord0 = voxelData2.TexCoord0;
													voxelData.TexCoord1 = voxelData2.TexCoord1;
													voxelData.TexCoord2 = voxelData2.TexCoord2;
												}
											}
										}
									}
								}
							}

							if( found )
							{
								var dataIndex = voxelsWithDataResult.Count;
								voxelsWithDataResult.Add( ref voxelData );
								voxel.DataIndexInListResult = dataIndex;
							}
						}
						else if( voxel.Where == WhereEnum.OnTriangle )
						{
							if( ThinFactor > 0 )
							{
								var add = false;

								if( ThinFactor >= 1 )
									add = true;
								else
								{
									var existsNearestInside = false;
									{
										for( int z = -1; z <= 1; z++ )
										{
											for( int y = -1; y <= 1; y++ )
											{
												for( int x = -1; x <= 1; x++ )
												{
													if( x == 0 && y == 0 && z == 0 )
														continue;

													var offset = new Vector3I( x, y, z );
													var indexToCheck = index + offset;

													if( indexToCheck.X >= 0 && indexToCheck.X < size.X &&
														indexToCheck.Y >= 0 && indexToCheck.Y < size.Y &&
														indexToCheck.Z >= 0 && indexToCheck.Z < size.Z )
													{
														ref var voxel2 = ref voxels[ indexToCheck.X, indexToCheck.Y, indexToCheck.Z ];

														if( voxel2.Where == WhereEnum.Inside )
														{
															existsNearestInside = true;
															goto end;
														}
													}
												}
											}
										}
end:;
									}

									if( !existsNearestInside )
									{
										var affectedVolumeFactor = 0.0;
										{
											var count = 0;
											for( int n = 0; n < 64; n++ )
											{
												if( IsBitEnabled( voxel.AffectedDetailVoxels, n ) )
													count++;
											}
											affectedVolumeFactor = count / 64.0;
										}

										//!!!!
										var factor = affectedVolumeFactor * ThinFactor * 2;

										var patternFactor = ThinPattern[ index.X % 2, index.Y % 2, index.Z % 2 ];
										if( factor > patternFactor )
											add = true;

										//if( factor > 0 && random.Next( 1.0 ) <= factor )
										//	add = true;

									}
								}

								if( add )
								{
									var dataIndex = voxelsWithDataResult.Count;

									var voxelData = new VoxelWithData();
									voxelData.Index = new Vector3I( index.X, index.Y, index.Z );

									ref var voxelData2 = ref voxelsWithDataOnTriangle.Data[ voxel.DataIndexInListOnTriangle ];

									voxelData.MaterialIndex = voxelData2.MaterialIndex;
									voxelData.Normal = voxelData2.Normal;
									voxelData.Tangent = voxelData2.Tangent;
									voxelData.Color0 = voxelData2.Color0;
									voxelData.TexCoord0 = voxelData2.TexCoord0;
									voxelData.TexCoord1 = voxelData2.TexCoord1;
									voxelData.TexCoord2 = voxelData2.TexCoord2;

									voxelsWithDataResult.Add( ref voxelData );

									voxel.DataIndexInListResult = dataIndex;
								}
							}
						}
					}


					//find nearest voxel with data

					//var now = DateTime.Now;
					//Log.Info( "START" );


					var voxelsWithoutData = new List<Vector3I>( voxelIndexes.Length );
					foreach( var index in voxelIndexes )
					{
						ref var voxel = ref voxels[ index.X, index.Y, index.Z ];
						if( voxel.DataIndexInListResult == -1 )
							voxelsWithoutData.Add( index );
					}
					var voxelsWithoutDataArray = voxelsWithoutData.ToArray();


					//!!!!slowly. можно в несколько итераций увеличивать радиус, скорее всего что-то попадется. не будет смысла все блоки перебирать


					var sectors = new Dictionary<Vector3I, VoxelWithDataSector>( 256 );
					for( int n = 0; n < voxelsWithDataResult.Count; n++ )
					{
						ref var voxel = ref voxelsWithDataResult.Data[ n ];
						var voxelIndex = voxel.Index;

						var sectorIndex = voxelIndex / 8;
						if( !sectors.TryGetValue( sectorIndex, out var sector ) )
						{
							sector = new VoxelWithDataSector();
							sectors[ sectorIndex ] = sector;
						}

						sector.Voxels.Add( ref voxelIndex );

						var voxelIndexF = voxelIndex.ToVector3F();
						var voxelBounds = new BoundsF( voxelIndexF, voxelIndexF + Vector3F.One );
						sector.Bounds.Add( ref voxelBounds );
					}

					var sectorsRandomizedOrder = sectors.Values.ToArray();
					Shuffle( sectorsRandomizedOrder, new FastRandom( 0 ) );

					//var sectorsArray = sectors.Values.ToArray();

					Parallel.For( 0, voxelsWithoutDataArray.Length, delegate ( int index )
					{
						var index3 = voxelsWithoutDataArray[ index ];
						int index3X = index3.X;
						int index3Y = index3.Y;
						int index3Z = index3.Z;

						var minDistanceSquared = int.MaxValue;
						var minIndex = new Vector3I( -1, -1, -1 );

						//var sortedSectors = (VoxelWithDataSector[])sectorsArray.Clone();
						//CollectionUtility.MergeSort( sortedSectors, delegate ( VoxelWithDataSector sector1, VoxelWithDataSector sector2 )
						//{
						//	var sectorDistanceSquared1 = sector1.Bounds.GetPointDistanceSquared( index3.ToVector3F() );
						//	var sectorDistanceSquared2 = sector2.Bounds.GetPointDistanceSquared( index3.ToVector3F() );

						//	if( sectorDistanceSquared1 > sectorDistanceSquared2 )
						//		return -1;
						//	if( sectorDistanceSquared1 < sectorDistanceSquared2 )
						//		return 1;
						//	return 0;
						//} );

						foreach( var sector in sectorsRandomizedOrder ) //sortedSectors )
						{
							var sectorDistanceSquared = sector.Bounds.GetPointDistanceSquared( index3.ToVector3F() );
							if( sectorDistanceSquared <= minDistanceSquared )
							{
								for( int n = 0; n < sector.Voxels.Count; n++ )
								{
									ref var indexToCheck = ref sector.Voxels.Data[ n ];

									var x = indexToCheck.X - index3X;
									var y = indexToCheck.Y - index3Y;
									var z = indexToCheck.Z - index3Z;
									var distanceSquared = x * x + y * y + z * z;
									//var distanceSquared = ( indexToCheck - index3 ).ToVector3F().LengthSquared();

									if( distanceSquared < minDistanceSquared )
									{
										minDistanceSquared = distanceSquared;
										minIndex = indexToCheck;
									}
								}
							}
							//else
							//	break;
						}

						if( minIndex.X == -1 )
							minIndex = new Vector3I( 0, 0, 0 );

						ref var voxel = ref voxels[ index3X, index3Y, index3Z ];
						voxel.NearestVoxelWithData = minIndex;
					} );


					//merge equal voxel data
					{
						var remap = new Dictionary<int, int>( voxelsWithDataResult.Count );
						var newList = new EDictionary<VoxelWithData, int>( voxelsWithDataResult.Count );

						for( int n = 0; n < voxelsWithDataResult.Count; n++ )
						{
							var fixedVoxel = voxelsWithDataResult.Data[ n ];
							//clear Index
							fixedVoxel.Index = Vector3I.Zero;

							if( !newList.TryGetValue( fixedVoxel, out var index ) )
							{
								index = newList.Count;
								newList.Add( fixedVoxel, index );
							}
							remap[ n ] = index;
						}

						//get updated list
						voxelsWithDataResult = new OpenList<VoxelWithData>( newList.Keys.ToArray() );

						//apply remap
						foreach( var index3 in voxelIndexes )
						{
							ref var voxel = ref voxels[ index3.X, index3.Y, index3.Z ];

							if( voxel.DataIndexInListResult != -1 )
								voxel.DataIndexInListResult = remap[ voxel.DataIndexInListResult ];
						}
					}

					////портит качество
					////merge equal voxel data
					//{
					//	var remap = new Dictionary<int, int>( voxelsWithDataResult.Count );
					//	var newList = new EDictionary<VoxelWithData, int>( voxelsWithDataResult.Count );

					//	for( int n = 0; n < voxelsWithDataResult.Count; n++ )
					//	{
					//		var fixedVoxel = voxelsWithDataResult.Data[ n ];
					//		//clear Index
					//		fixedVoxel.Index = Vector3I.Zero;

					//		//discrete to make better merging
					//		{
					//			fixedVoxel.Normal = ( ( fixedVoxel.Normal * 100 ).ToVector3I().ToVector3F() / 100 ).GetNormalize();
					//			fixedVoxel.Tangent = new Vector4F( ( ( fixedVoxel.Tangent.ToVector3F() * 100 ).ToVector3I().ToVector3F() / 100 ).GetNormalize(), fixedVoxel.Tangent.W );
					//			fixedVoxel.Color0 = ( ( fixedVoxel.Color0 * 255 ).ToVector4F().ToVector4I() / 255 ).ToVector4F().ToColorValue();
					//			fixedVoxel.TexCoord0 = ( fixedVoxel.TexCoord0 * 500 ).ToVector2I().ToVector2F() / 500;
					//			fixedVoxel.TexCoord1 = ( fixedVoxel.TexCoord1 * 500 ).ToVector2I().ToVector2F() / 500;
					//			fixedVoxel.TexCoord2 = ( fixedVoxel.TexCoord2 * 500 ).ToVector2I().ToVector2F() / 500;
					//		}

					//		if( !newList.TryGetValue( fixedVoxel, out var index ) )
					//		{
					//			index = newList.Count;
					//			newList.Add( fixedVoxel, index );
					//		}
					//		remap[ n ] = index;
					//	}

					//	//get updated list
					//	voxelsWithDataResult = new OpenList<VoxelWithData>( newList.Keys.ToArray() );

					//	//apply remap
					//	foreach( var index3 in voxelIndexes )
					//	{
					//		ref var voxel = ref voxels[ index3.X, index3.Y, index3.Z ];

					//		if( voxel.DataIndexInListResult != -1 )
					//			voxel.DataIndexInListResult = remap[ voxel.DataIndexInListResult ];
					//	}
					//}



					//var now2 = DateTime.Now;
					//Log.Info( "END " + ( now2 - now ).TotalSeconds.ToString() );


					//calculate result data

					var voxelWithDataFloats = fullFormat ? 8 : 4;

					var totalResultSize = sizeof( MeshGeometry.VoxelDataHeader );
					totalResultSize += size.X * size.Y * size.Z * 4;
					totalResultSize += voxelsWithDataResult.Count * voxelWithDataFloats * 4;

					resultData = new byte[ totalResultSize ];
					fixed( byte* pResultData = resultData )
					{
						//header
						var header = (MeshGeometry.VoxelDataHeader*)pResultData;
						header->Version = MeshGeometry.CurrentVoxelDataVersion;
						header->GridSize = size;
						header->BoundsMin = meshBounds.Minimum;
						header->CellSize = cellSize;
						header->Format = format;//fullFormat ? 1 : 0;
						header->VoxelCount = voxelsWithDataResult.Count;
						header->FillHolesDistance = (float)FillHolesDistance;

						//grid
						{
							var pVoxelData = pResultData + sizeof( MeshGeometry.VoxelDataHeader );
							float* pVoxelDataFloat = (float*)pVoxelData;

							float* current = pVoxelDataFloat;
							foreach( var index3 in voxelIndexes )
							{
								ref var voxel = ref voxels[ index3.X, index3.Y, index3.Z ];

								float v;
								if( voxel.DataIndexInListResult != -1 )
								{
									var floatIndexInBuffer = size.X * size.Y * size.Z + voxel.DataIndexInListResult * voxelWithDataFloats;
									v = floatIndexInBuffer + 0.5f;
								}
								else
								{
									var nearestIndex = GetNearestVoxelWithDataIndex256( ref voxel.NearestVoxelWithData );
									v = -( (float)nearestIndex + 0.5f );
								}

								*current++ = v;
							}
						}

						//voxels with data
						{
							var pVoxelWithData = pResultData + sizeof( MeshGeometry.VoxelDataHeader ) + size.X * size.Y * size.Z * 4;
							HalfType* pVoxelWithDataHalf = (HalfType*)pVoxelWithData;

							HalfType* current = pVoxelWithDataHalf;
							for( int n = 0; n < voxelsWithDataResult.Count; n++ )
							{
								ref var voxelData = ref voxelsWithDataResult.Data[ n ];

								var sphericalDirection = SphericalDirectionF.FromVector( voxelData.Normal );

								var h = sphericalDirection.Horizontal;
								h /= MathEx.PI * 2;
								h += 0.5f;
								MathEx.Clamp( ref h, 0, 0.999f );

								var v = sphericalDirection.Vertical;
								v /= MathEx.PI;
								v += 0.5f;
								MathEx.Clamp( ref v, 0, 0.999f );

								*current++ = (HalfType)( h + voxelData.MaterialIndex );
								*current++ = (HalfType)v;// ( v + ( fullFormat ? 1 : 0 ) );
								*current++ = (HalfType)voxelData.TexCoord0.X;
								*current++ = (HalfType)voxelData.TexCoord0.Y;

								*current++ = (HalfType)voxelData.Tangent.X;
								*current++ = (HalfType)voxelData.Tangent.Y;
								*current++ = (HalfType)voxelData.Tangent.Z;
								*current++ = (HalfType)voxelData.Tangent.W;

								if( fullFormat )
								{
									*current++ = (HalfType)voxelData.TexCoord1.X;
									*current++ = (HalfType)voxelData.TexCoord1.Y;
									*current++ = (HalfType)voxelData.TexCoord2.X;
									*current++ = (HalfType)voxelData.TexCoord2.Y;

									*current++ = (HalfType)voxelData.Color0.Red;
									*current++ = (HalfType)voxelData.Color0.Green;
									*current++ = (HalfType)voxelData.Color0.Blue;
									*current++ = (HalfType)voxelData.Color0.Alpha;
								}
							}
						}
					}
				}
				finally
				{
					meshTest?.Dispose();
				}

				return resultData;
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				return null;
			}
		}

		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		struct Vertex
		{
			public Vector3F Position;
			//public Vector2F TexCoord0;
		}

		static Material CloneMaterialAndModifyToSupportDeferred( Material sourceMaterial )
		{
			var material = (Material)sourceMaterial.Clone();
			material.Name = "Optimized of " + sourceMaterial.Name;

			//Transparent, Add
			if( material.BlendMode.Value == Material.BlendModeEnum.Transparent || material.BlendMode.Value == Material.BlendModeEnum.Add )
				material.BlendMode = Material.BlendModeEnum.Masked;

			//Cloth, Unlit
			if( material.ShadingModel.Value == Material.ShadingModelEnum.Cloth || material.ShadingModel.Value == Material.ShadingModelEnum.Unlit )
				material.ShadingModel = Material.ShadingModelEnum.Lit;

			//Subsurface + Emissive
			if( material.ShadingModel.Value == Material.ShadingModelEnum.Subsurface )
			{
				if( material.Emissive.ReferenceSpecified || material.Emissive.Value.ToVector3() != Vector3.Zero )
					material.ShadingModel = Material.ShadingModelEnum.Lit;
			}

			//Foliage + Emissive
			if( material.ShadingModel.Value == Material.ShadingModelEnum.Foliage )
			{
				if( material.Emissive.ReferenceSpecified || material.Emissive.Value.ToVector3() != Vector3.Zero )
					material.ShadingModel = Material.ShadingModelEnum.Lit;
			}

			material.ClearCoat = 0;
			material.Anisotropy = 0;
			material.ReceiveShadows = true;

			return material;
		}

		public void Convert()
		{
			//!!!!support procedural geometries

			var wasEnabled = Mesh.Enabled;
			Mesh.Enabled = false;

			try
			{
				var sourceGeometries = Mesh.GetComponents<MeshGeometry>().Where( g => g.Vertices.Value != null && g.VertexStructure.Value != null && g.Indices.Value != null ).ToArray();

				if( sourceGeometries.Length != 0 )
				{
					//sort group. combined deferred first
					CollectionUtility.MergeSort( sourceGeometries, delegate ( MeshGeometry g1, MeshGeometry g2 )
					{
						var m1 = g1.Material.Value;//materialByGeometry.TryGetValue( g1, out var m1 );
						var m2 = g2.Material.Value;//materialByGeometry.TryGetValue( g2, out var m2 );

						var points1 = 0;
						if( m1 != null )
						{
							points1++;
							if( string.IsNullOrEmpty( m1.PerformCheckDeferredShadingSupport() ) )
								points1 += 2;
						}

						var points2 = 0;
						if( m2 != null )
						{
							points2++;
							if( string.IsNullOrEmpty( m2.PerformCheckDeferredShadingSupport() ) )
								points2 += 2;
						}

						if( points1 > points2 )
							return -1;
						if( points1 < points2 )
							return 1;
						return 0;
					} );

					var voxelData = CreateVoxelData( sourceGeometries, out var meshBounds );

					//foreach( var geometry in geometries )
					//{
					//var voxelData = CreateVoxelData( geometry, out var meshBounds );

					var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.Position/* | StandardVertex.Components.TexCoord0*/, true, out int vertexSize );

					unsafe
					{
						if( vertexSize != sizeof( Vertex ) )
							Log.Fatal( "MeshConvertToConvex: Convert: vertexSize != sizeof( Vertex )." );
					}

					SimpleMeshGenerator.GenerateBox( meshBounds, out var positions, out var indices );

					var vertices = new byte[ vertexSize * positions.Length ];
					unsafe
					{
						fixed( byte* pVertices = vertices )
						{
							Vertex* pVertex = (Vertex*)pVertices;

							for( int n = 0; n < positions.Length; n++ )
							{
								pVertex->Position = positions[ n ];
								//pVertex->TexCoord0 = new Vector2F( 0.5f, 0.5f );

								pVertex++;
							}
						}
					}

					//SimpleMeshGenerator.GenerateBox( meshBounds, false, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out indices, out var faces );
					//if( faces != null )
					//	structure = SimpleMeshGenerator.CreateMeshStructure( faces );

					//var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

					//SimpleMeshGenerator.GenerateBox( meshBounds, out var positions, out var indices );

					////SimpleMeshGenerator.GenerateBox( meshBounds, false, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out indices, out var faces );

					////if( faces != null )
					////	structure = SimpleMeshGenerator.CreateMeshStructure( faces );

					//var vertices = new byte[ vertexSize * positions.Length ];
					//unsafe
					//{
					//	fixed( byte* pVertices = vertices )
					//	{
					//		StandardVertex.StaticOneTexCoord* pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;

					//		for( int n = 0; n < positions.Length; n++ )
					//		{
					//			pVertex->Position = positions[ n ];
					//			g pVertex->Normal = new Vector3F( 0, 0, 1 );//normals[ n ];
					//			pVertex->Tangent = new Vector4F( 1, 0, 0, -1 );//tangents[ n ];
					//			pVertex->Color = new ColorValue( 1, 1, 1, 1 );
					//			pVertex->TexCoord0 = new Vector2F( 0.5f, 0.5f );//texCoords[ n ];

					//			pVertex++;
					//		}
					//	}
					//}

					if( sourceGeometries.Length > 1 )
					{
						//make multi material

						var geometry = Mesh.CreateComponent<MeshGeometry>();
						geometry.Name = "Geometry";
						geometry.VertexStructure = vertexStructure;
						geometry.Vertices = vertices;
						geometry.Indices = indices.ToArray();
						geometry.VoxelData = voxelData;

						var material = geometry.CreateComponent<MultiMaterial>();
						material.Name = "Material";
						foreach( var sourceGeometry in sourceGeometries )
						{
							var subMaterialReference = sourceGeometry.Material;

							var sourceSubMaterial = sourceGeometry.Material.Value;
							if( OptimizeMaterials && sourceSubMaterial != null )
							{
								//check for deferred rendering support
								if( !string.IsNullOrEmpty( sourceSubMaterial.PerformCheckDeferredShadingSupport() ) )
								{
									//clone material and enable deferred
									var subMaterial = CloneMaterialAndModifyToSupportDeferred( sourceSubMaterial );
									material.AddComponent( subMaterial );

									subMaterialReference = ReferenceUtility.MakeThisReference( material, subMaterial );
								}
							}

							material.Materials.Add( subMaterialReference );

							//material.Materials.Add( sourceGeometry.Material );
						}

						geometry.Material = ReferenceUtility.MakeThisReference( geometry, material );

						//delete old geometries
						foreach( var g in sourceGeometries )
							g.Dispose();
					}
					else
					{
						var geometry = sourceGeometries[ 0 ];
						geometry.VertexStructure = vertexStructure;
						geometry.Vertices = vertices;
						geometry.Indices = indices.ToArray();
						geometry.VoxelData = voxelData;

						var sourceMaterial = geometry.Material.Value;
						if( OptimizeMaterials && sourceMaterial != null )
						{
							//check for deferred rendering support
							if( !string.IsNullOrEmpty( sourceMaterial.PerformCheckDeferredShadingSupport() ) )
							{
								//clone material and enable deferred
								var material = CloneMaterialAndModifyToSupportDeferred( sourceMaterial );
								geometry.AddComponent( material );

								geometry.Material = ReferenceUtility.MakeThisReference( geometry, material );
							}
						}
					}

					//}
				}
			}
			finally
			{
				Mesh.Enabled = wasEnabled;
			}
		}
	}
}

