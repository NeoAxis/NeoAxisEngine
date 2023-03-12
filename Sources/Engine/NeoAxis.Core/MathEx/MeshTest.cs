// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Class for quickly determining the intersection of the ray with the mesh. Internally octree is used.
	/// </summary>
	public class MeshTest : IDisposable
	{
		OctreeContainer octreeContainer;
		int objectCount;
		Vector3F[] vertices;
		int[] indices;

		/////////////////////////////////////////

		public enum Mode
		{
			One,
			OneClosest,
			All,
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents result data for <see cref="MeshTest"/>.
		/// </summary>
		public struct ResultItem
		{
			public float Scale;
			public Vector3F Normal;
			public int TriangleIndex;

			public ResultItem( float scale, Vector3F normal, int triangleIndex )
			{
				Scale = scale;
				Normal = normal;
				TriangleIndex = triangleIndex;
			}
		}

		/////////////////////////////////////////

		[MethodImpl( (MethodImplOptions)512 )]
		public MeshTest( Vector3F[] vertices, int[] indices )
		{
			this.vertices = vertices;
			this.indices = indices;

			if( vertices.Length != 0 && indices.Length != 0 )
			{
				var bounds = Bounds.Cleared;
				foreach( var vertex in vertices )
				{
					if( float.IsNaN( vertex.X ) || float.IsNaN( vertex.Y ) || float.IsNaN( vertex.Z ) )
						continue;
					bounds.Add( vertex );
				}

				var initSettings = new OctreeContainer.InitSettings();
				initSettings.InitialOctreeBounds = bounds;
				initSettings.OctreeBoundsRebuildExpand = Vector3.Zero;
				initSettings.MinNodeSize = bounds.GetSize() / 50;
				octreeContainer = new OctreeContainer( initSettings );

				for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
				{
					ref var vertex0 = ref vertices[ indices[ nTriangle * 3 + 0 ] ];
					ref var vertex1 = ref vertices[ indices[ nTriangle * 3 + 1 ] ];
					ref var vertex2 = ref vertices[ indices[ nTriangle * 3 + 2 ] ];

					if( float.IsNaN( vertex0.X ) || float.IsNaN( vertex0.Y ) || float.IsNaN( vertex0.Z ) )
						continue;
					if( float.IsNaN( vertex1.X ) || float.IsNaN( vertex1.Y ) || float.IsNaN( vertex1.Z ) )
						continue;
					if( float.IsNaN( vertex2.X ) || float.IsNaN( vertex2.Y ) || float.IsNaN( vertex2.Z ) )
						continue;

					var triangleBounds = new Bounds( vertex0 );
					triangleBounds.Add( vertex1 );
					triangleBounds.Add( vertex2 );

					octreeContainer.AddObject( ref triangleBounds, 1 );
					objectCount++;
				}
			}
		}

		public void Dispose()
		{
			octreeContainer?.Dispose();
			octreeContainer = null;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public ResultItem[] RayCast( RayF ray, Mode mode, bool twoSided )
		{
			unsafe
			{
				if( vertices.Length == 0 || indices.Length == 0 || ray.Direction == Vector3F.Zero )
					return Array.Empty<ResultItem>();

				OctreeContainer.GetObjectsRayOutputData* octreeObjects = null;
				try
				{
					octreeObjects = (OctreeContainer.GetObjectsRayOutputData*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, sizeof( OctreeContainer.GetObjectsRayOutputData ) * objectCount );
					var octreeSuccess = octreeContainer.GetObjects( ray, 0xFFFFFFFF, OctreeContainer.ModeEnum.All, octreeObjects, objectCount, out var octreeResultCount );

					if( !octreeSuccess || octreeResultCount == 0 )
						return Array.Empty<ResultItem>();

					using( var resultList = new OpenListNative<ResultItem>( mode == Mode.All ? octreeResultCount : 1 ) )
					{
						for( int n = 0; n < octreeResultCount; n++ )
						{
							int triangleIndex = octreeObjects[ n ].ObjectIndex;
							ref var vertex0 = ref vertices[ indices[ triangleIndex * 3 + 0 ] ];
							ref var vertex1 = ref vertices[ indices[ triangleIndex * 3 + 1 ] ];
							ref var vertex2 = ref vertices[ indices[ triangleIndex * 3 + 2 ] ];

							//!!!!надо ли два раз искать IntersectTriangleRay?
							var found = MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex1, ref vertex2, ref ray, out float scale );
							var normal = Vector3F.Zero;
							if( found )
								MathAlgorithms.CalculateTriangleNormal( ref vertex0, ref vertex1, ref vertex2, out normal );
							else
							{
								found = twoSided && MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex2, ref vertex1, ref ray, out scale );
								if( found )
									MathAlgorithms.CalculateTriangleNormal( ref vertex0, ref vertex2, ref vertex1, out normal );
							}

							//if( MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex1, ref vertex2, ref ray, out float scale ) ||
							//	twoSided && MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex2, ref vertex1, ref ray, out scale ) )
							if( found )
							{
								var item = new ResultItem( scale, normal, triangleIndex );

								if( mode == Mode.One )
								{
									//mode One
									return new ResultItem[] { item };
								}
								else
								{
									//modes OneClosest, All
									if( resultList.Count == 0 )
										resultList.Add( ref item );
									else
									{
										if( mode == Mode.All )
											resultList.Add( ref item );
										else
										{
											if( item.Scale < resultList[ 0 ].Scale )
												resultList.Data[ 0 ] = item;
										}
									}
								}
							}
						}

						if( resultList.Count == 0 )
							return Array.Empty<ResultItem>();

						var array = resultList.ToArray();

						if( mode == Mode.All )
						{
							CollectionUtility.SelectionSort( array, delegate ( ResultItem r1, ResultItem r2 )
							{
								if( r1.Scale < r2.Scale )
									return -1;
								if( r1.Scale > r2.Scale )
									return 1;
								return 0;
							} );
						}

						return array;
					}
				}
				finally
				{
					NativeUtility.Free( octreeObjects );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IntersectsFast( Plane[] planes, ref Bounds bounds )
		{
			if( vertices.Length == 0 || indices.Length == 0 )
				return false;

			//сами треугольники тоже можно проверять. тогда это не Fast method

			unsafe
			{
				octreeContainer.GetObjects( planes, bounds, 0xFFFFFFFF, OctreeContainer.ModeEnum.One, null, 0, out var count, IntPtr.Zero );
				return count != 0;
			}
			//var octreeObjects = octreeContainer.GetObjects( planes, bounds, 0xFFFFFFFF, OctreeContainer.ModeEnum.One, IntPtr.Zero );
			//return octreeObjects.Length != 0;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IntersectsFast( Plane[] planes, Bounds bounds )
		{
			return IntersectsFast( planes, ref bounds );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IntersectsFast( ref Bounds bounds )
		{
			if( vertices.Length == 0 || indices.Length == 0 )
				return false;

			//сами треугольники тоже можно проверять. тогда это не Fast method

			unsafe
			{
				octreeContainer.GetObjects( bounds, 0xFFFFFFFF, OctreeContainer.ModeEnum.One, null, 0, out var count );
				return count != 0;
			}
			//var octreeObjects = octreeContainer.GetObjects( bounds, 0xFFFFFFFF, OctreeContainer.ModeEnum.One );
			//return octreeObjects.Length != 0;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IntersectsFast( Bounds bounds )
		{
			return IntersectsFast( ref bounds );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int[] GetIntersectedTrianglesFast( Plane[] planes, ref Bounds bounds )
		{
			if( vertices.Length == 0 || indices.Length == 0 )
				return Array.Empty<int>();

			//сами треугольники тоже можно проверять. тогда это не Fast method

			var octreeObjects = octreeContainer.GetObjects( planes, bounds, 0xFFFFFFFF, OctreeContainer.ModeEnum.All, IntPtr.Zero );
			return octreeObjects;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int[] GetIntersectedTrianglesFast( Plane[] planes, Bounds bounds )
		{
			return GetIntersectedTrianglesFast( planes, ref bounds );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int[] GetIntersectedTrianglesFast( ref Bounds bounds )
		{
			if( vertices.Length == 0 || indices.Length == 0 )
				return Array.Empty<int>();

			//сами треугольники тоже можно проверять. тогда это не Fast method

			var octreeObjects = octreeContainer.GetObjects( bounds, 0xFFFFFFFF, OctreeContainer.ModeEnum.All );
			return octreeObjects;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int[] GetIntersectedTrianglesFast( Bounds bounds )
		{
			return GetIntersectedTrianglesFast( ref bounds );
		}
	}
}
