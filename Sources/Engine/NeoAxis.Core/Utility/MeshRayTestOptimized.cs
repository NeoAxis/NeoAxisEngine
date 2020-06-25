// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Class for quickly determining the intersection of the ray with the mesh.
	/// </summary>
	public class MeshRayTestOptimized : IDisposable
	{
		OctreeContainer octreeContainer;
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
		/// Represents result data for <see cref="MeshRayTestOptimized"/>.
		/// </summary>
		public struct ResultItem
		{
			internal float scale;
			public float Scale
			{
				get { return scale; }
				set { scale = value; }
			}

			internal int triangleIndex;
			public int TriangleIndex
			{
				get { return triangleIndex; }
				set { triangleIndex = value; }
			}

			public ResultItem( float scale, int triangleIndex )
			{
				this.scale = scale;
				this.triangleIndex = triangleIndex;
			}
		}

		/////////////////////////////////////////

		public MeshRayTestOptimized( Vector3F[] vertices, int[] indices )
		{
			this.vertices = vertices;
			this.indices = indices;

			if( vertices.Length != 0 && indices.Length != 0 )
			{
				var bounds = Bounds.Cleared;
				foreach( var vertex in vertices )
					bounds.Add( vertex );

				var initSettings = new OctreeContainer.InitSettings();
				initSettings.InitialOctreeBounds = bounds;
				initSettings.OctreeBoundsRebuildExpand = Vector3.Zero;
				initSettings.MinNodeSize = bounds.GetSize() / 50;
				octreeContainer = new OctreeContainer( initSettings );

				for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
				{
					var vertex0 = vertices[ indices[ nTriangle * 3 + 0 ] ];
					var vertex1 = vertices[ indices[ nTriangle * 3 + 1 ] ];
					var vertex2 = vertices[ indices[ nTriangle * 3 + 2 ] ];

					var triangleBounds = new Bounds( vertex0 );
					triangleBounds.Add( vertex1 );
					triangleBounds.Add( vertex2 );

					octreeContainer.AddObject( triangleBounds, 1 );
				}
			}
		}

		public void Dispose()
		{
			octreeContainer?.Dispose();
			octreeContainer = null;
		}

		public ResultItem[] RayTest( RayF ray, Mode mode, bool twoSided )
		{
			if( vertices.Length == 0 || indices.Length == 0 )
				return new ResultItem[ 0 ];
			if( ray.Direction == Vector3F.Zero )
				return new ResultItem[ 0 ];

			var octreeObjects = octreeContainer.GetObjects( ray, 0xFFFFFFFF );
			var resultList = new List<ResultItem>( octreeObjects.Length );
			foreach( var data in octreeObjects )
			{
				int triangleIndex = data.ObjectIndex;
				ref var vertex0 = ref vertices[ indices[ triangleIndex * 3 + 0 ] ];
				ref var vertex1 = ref vertices[ indices[ triangleIndex * 3 + 1 ] ];
				ref var vertex2 = ref vertices[ indices[ triangleIndex * 3 + 2 ] ];

				if( MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex1, ref vertex2, ref ray, out float scale ) ||
					twoSided && MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex2, ref vertex1, ref ray, out scale ) )
				{
					var item = new ResultItem( scale, triangleIndex );

					if( mode == Mode.One )
					{
						//mode One
						return new ResultItem[] { item };
					}
					else
					{
						//modes OneClosest, All
						if( resultList.Count == 0 )
							resultList.Add( item );
						else
						{
							if( mode == Mode.All )
								resultList.Add( item );
							else
							{
								if( item.scale < resultList[ 0 ].scale )
									resultList[ 0 ] = item;
							}
						}
					}
				}
			}

			ResultItem[] array = resultList.ToArray();
			if( mode == Mode.All )
			{
				CollectionUtility.SelectionSort( array, delegate ( ResultItem r1, ResultItem r2 )
				{
					if( r1.scale < r2.scale )
						return -1;
					if( r1.scale > r2.scale )
						return 1;
					return 0;
				} );
			}

			return resultList.ToArray();
		}

		//!!!!
		internal bool _Intersects( Plane[] planes, ref Bounds bounds )
		{
			if( vertices.Length == 0 || indices.Length == 0 )
				return false;
			var octreeObjects = octreeContainer.GetObjects( planes, bounds, 0xFFFFFFFF, OctreeContainer.ModeEnum.One );

			//!!!!сами треугольники тоже нужно проверять

			return octreeObjects.Length != 0;
		}

		//!!!!
		//public ResultItem[] _SweepTest( RayF ray, Mode mode )
		//{
		//	//new ConvexPolyhedron(

		//	//xx xx;
		//}

	}
}
