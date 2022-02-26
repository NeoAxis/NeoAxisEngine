// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Class for calculating convex shapes by a mesh. VHACD library is used.
	/// </summary>
	public static class ConvexDecomposition
	{
		/// <summary>
		/// Compute convex hulls
		/// </summary>
		/// <param name="points"></param>
		/// <param name="pointCount"></param>
		/// <param name="triangles"></param>
		/// <param name="triangleCount"></param>
		/// <param name="maxConvexHulls">Maximum number of convex hulls to produce</param>
		/// <param name="maxConvexTriangles">maximum number of triangles per convex-hull (default = 64, range = 4 - 1024)</param>
		/// <param name="minConvexVolume">Controls the adaptive sampling of the generated convex-hulls (default=0.0001, range=0.0-0.01)</param>
		/// <param name="convexApproximation">Enable/disable approximation when computing convex-hulls (default = 1)</param>
		/// <param name="maxResolution">Maximum number of voxels generated during the voxelization stage (default = 100,000, range = 10,000 - 16,000,000)</param>
		/// <param name="maxConcavity">Maximum allowed concavity (default=0.0025, range=0.0-1.0)</param>
		/// <param name="alpha">Controls the bias toward clipping along symmetry planes (default=0.05, range=0.0-1.0)</param>
		/// <param name="beta">Controls the bias toward clipping along revolution axes (default=0.05, range=0.0-1.0)</param>
		/// <param name="planeDownsampling">Controls the granularity of the search for the \"best\" clipping plane (default=4, range=1-16)</param>
		/// <param name="hullDownsampling">Controls the precision of the convex-hull generation process during the clipping plane selection stage (default=4, range=1-16)</param>
		/// <param name="normalizeMesh">Enable/disable normalizing the mesh before applying the convex decomposition (default = 0)</param>
		/// <param name="tetrahedronMode">0: voxel-based approximate convex decomposition, 1: tetrahedron-based approximate convex decomposition (default=0)</param>
		/// <returns></returns>
		[DllImport( OgreWrapper.library, CallingConvention = OgreWrapper.convention )]
		static extern unsafe IntPtr VHACD_Compute32( float* points, int pointCount, int* triangles, int triangleCount,
			int maxConvexHulls, int maxConvexTriangles = 64, double minConvexVolume = 0.0001,
			[MarshalAs( UnmanagedType.U1 )] bool convexApproximation = true, int maxResolution = 100000, double maxConcavity = 0.0025,
			double alpha = 0.05, double beta = 0.05, int planeDownsampling = 4, int hullDownsampling = 4,
			[MarshalAs( UnmanagedType.U1 )] bool normalizeMesh = false, [MarshalAs( UnmanagedType.U1 )] bool tetrahedronMode = false );

		/// <summary>
		/// Compute convex hulls
		/// </summary>
		/// <param name="points"></param>
		/// <param name="pointCount"></param>
		/// <param name="triangles"></param>
		/// <param name="triangleCount"></param>
		/// <param name="maxConvexHulls">Maximum number of convex hulls to produce</param>
		/// <param name="maxConvexTriangles">maximum number of triangles per convex-hull (default = 64, range = 4 - 1024)</param>
		/// <param name="minConvexVolume">Controls the adaptive sampling of the generated convex-hulls (default=0.0001, range=0.0-0.01)</param>
		/// <param name="convexApproximation">Enable/disable approximation when computing convex-hulls (default = 1)</param>
		/// <param name="maxResolution">Maximum number of voxels generated during the voxelization stage (default = 100,000, range = 10,000 - 16,000,000)</param>
		/// <param name="maxConcavity">Maximum allowed concavity (default=0.0025, range=0.0-1.0)</param>
		/// <param name="alpha">Controls the bias toward clipping along symmetry planes (default=0.05, range=0.0-1.0)</param>
		/// <param name="beta">Controls the bias toward clipping along revolution axes (default=0.05, range=0.0-1.0)</param>
		/// <param name="planeDownsampling">Controls the granularity of the search for the \"best\" clipping plane (default=4, range=1-16)</param>
		/// <param name="hullDownsampling">Controls the precision of the convex-hull generation process during the clipping plane selection stage (default=4, range=1-16)</param>
		/// <param name="normalizeMesh">Enable/disable normalizing the mesh before applying the convex decomposition (default = 0)</param>
		/// <param name="tetrahedronMode">0: voxel-based approximate convex decomposition, 1: tetrahedron-based approximate convex decomposition (default=0)</param>
		/// <returns></returns>
		[DllImport( OgreWrapper.library, CallingConvention = OgreWrapper.convention )]
		static extern unsafe IntPtr VHACD_Compute64( double* points, int pointCount, int* triangles, int triangleCount,
			int maxConvexHulls, int maxConvexTriangles = 64, double minConvexVolume = 0.0001,
			[MarshalAs( UnmanagedType.U1 )] bool convexApproximation = true, int maxResolution = 100000, double maxConcavity = 0.0025,
			double alpha = 0.05, double beta = 0.05, int planeDownsampling = 4, int hullDownsampling = 4,
			[MarshalAs( UnmanagedType.U1 )] bool normalizeMesh = false, [MarshalAs( UnmanagedType.U1 )] bool tetrahedronMode = false );

		[DllImport( OgreWrapper.library, CallingConvention = OgreWrapper.convention )]
		static extern int VHACD_GetClusterCount( IntPtr obj );

		[DllImport( OgreWrapper.library, CallingConvention = OgreWrapper.convention )]
		static extern void VHACD_GetBufferSize( IntPtr obj, int cluster, out int pointCount, out int triangleCount );

		[DllImport( OgreWrapper.library, CallingConvention = OgreWrapper.convention )]
		static extern unsafe void VHACD_GetBuffer64( IntPtr obj, int cluster, double* points, int* triangles );

		[DllImport( OgreWrapper.library, CallingConvention = OgreWrapper.convention )]
		static extern unsafe void VHACD_GetBuffer32( IntPtr obj, int cluster, float* points, int* triangles );

		[DllImport( OgreWrapper.library, CallingConvention = OgreWrapper.convention )]
		static extern void VHACD_Delete( IntPtr obj );

		/////////////////////////////////////////

		/// <summary>
		/// Represents a cluster of data for <see cref="ConvexDecomposition"/>.
		/// </summary>
		public class Cluster
		{
			Vector3F[] vertices;
			int[] indices;

			public Cluster( Vector3F[] vertices, int[] indices )
			{
				this.vertices = vertices;
				this.indices = indices;
			}

			public Vector3F[] Vertices
			{
				get { return vertices; }
			}

			public int[] Indices
			{
				get { return indices; }
			}

			public override string ToString()
			{
				return string.Format( "Vertices: {0}, Indices {1}", vertices.Length, indices.Length );
			}
		}

		/////////////////////////////////////////

		////!!!!maxTrianglesInDecimatedMesh, maxVerticesPerConvexHull
		//public static unsafe Cluster[] Decompose( Vec3F[] vertices, int[] indices, int maxTrianglesInDecimatedMesh, int maxVerticesPerConvexHull )
		//{
		//	//if( hacdInstance == null )
		//	//	hacdInstance = HACDWrapper.Init();

		//	double[] points = new double[ vertices.Length * 3 ];
		//	for( int n = 0; n < vertices.Length; n++ )
		//	{
		//		points[ n * 3 + 0 ] = vertices[ n ].X;
		//		points[ n * 3 + 1 ] = vertices[ n ].Y;
		//		points[ n * 3 + 2 ] = vertices[ n ].Z;
		//	}

		//	IntPtr obj;
		//	fixed ( double* pPoints = points )
		//	fixed ( int* pIndices = indices )
		//		obj = HACD_Compute( pPoints, vertices.Length, pIndices, indices.Length / 3, maxTrianglesInDecimatedMesh, maxVerticesPerConvexHull );

		//	if( obj == IntPtr.Zero )
		//		return null;

		//	int clusterCount = HACD_GetClusterCount( obj );
		//	Cluster[] items = new Cluster[ clusterCount ];

		//	for( int nCluster = 0; nCluster < clusterCount; nCluster++ )
		//	{
		//		HACD_GetBufferSize( obj, nCluster, out var clusterPointCount, out var clusterTriangleCount );

		//		double[] clusterPoints = new double[ clusterPointCount * 3 ];
		//		int[] clusterIndices = new int[ clusterTriangleCount * 3 ];

		//		fixed ( double* pPoints = clusterPoints )
		//		fixed ( int* pIndices = clusterIndices )
		//			HACD_GetBuffer( obj, nCluster, pPoints, pIndices );

		//		Vec3F[] clusterVertices = new Vec3F[ clusterPointCount ];
		//		for( int n = 0; n < clusterPointCount; n++ )
		//		{
		//			clusterVertices[ n ] = new Vec3F(
		//				(float)clusterPoints[ n * 3 + 0 ],
		//				(float)clusterPoints[ n * 3 + 1 ],
		//				(float)clusterPoints[ n * 3 + 2 ] );
		//		}

		//		items[ nCluster ] = new Cluster( clusterVertices, clusterIndices );
		//	}

		//	HACD_Delete( obj );

		//	return items;
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="indices"></param>
		/// <param name="maxConvexHulls">Maximum number of convex hulls to produce</param>
		/// <param name="maxConvexTriangles">maximum number of triangles per convex-hull (default = 64, range = 4 - 1024)</param>
		/// <param name="minConvexVolume">Controls the adaptive sampling of the generated convex-hulls (default=0.0001, range=0.0-0.01)</param>
		/// <param name="convexApproximation">Enable/disable approximation when computing convex-hulls (default = 1)</param>
		/// <param name="maxResolution">Maximum number of voxels generated during the voxelization stage (default = 100,000, range = 10,000 - 16,000,000)</param>
		/// <param name="maxConcavity">Maximum allowed concavity (default=0.0025, range=0.0-1.0)</param>
		/// <param name="alpha">Controls the bias toward clipping along symmetry planes (default=0.05, range=0.0-1.0)</param>
		/// <param name="beta">Controls the bias toward clipping along revolution axes (default=0.05, range=0.0-1.0)</param>
		/// <param name="planeDownsampling">Controls the granularity of the search for the \"best\" clipping plane (default=4, range=1-16)</param>
		/// <param name="hullDownsampling">Controls the precision of the convex-hull generation process during the clipping plane selection stage (default=4, range=1-16)</param>
		/// <param name="normalizeMesh">Enable/disable normalizing the mesh before applying the convex decomposition (default = 0)</param>
		/// <param name="tetrahedronMode">0: voxel-based approximate convex decomposition, 1: tetrahedron-based approximate convex decomposition (default=0)</param>
		/// <returns></returns>
		public static unsafe Cluster[] Decompose( Vector3F[] vertices, int[] indices, int maxConvexHulls, int maxConvexTriangles = 64, double minConvexVolume = 0.0001, bool convexApproximation = true, int maxResolution = 100000, double maxConcavity = 0.0025, double alpha = 0.05, double beta = 0.05, int planeDownsampling = 4, int hullDownsampling = 4, bool normalizeMesh = false, bool tetrahedronMode = false )
		{
			foreach( var index in indices )
				if( index < 0 || index >= vertices.Length )
					throw new ArgumentException( "Invalid indices." );

			IntPtr obj;
			fixed( void* pPoints = vertices )
			fixed( int* pIndices = indices )
				obj = VHACD_Compute32( (float*)pPoints, vertices.Length, pIndices, indices.Length / 3, maxConvexHulls, maxConvexTriangles, minConvexVolume, convexApproximation, maxResolution, maxConcavity, alpha, beta, planeDownsampling, hullDownsampling, normalizeMesh, tetrahedronMode );

			if( obj == IntPtr.Zero )
				return null;

			int clusterCount = VHACD_GetClusterCount( obj );
			Cluster[] items = new Cluster[ clusterCount ];

			for( int nCluster = 0; nCluster < clusterCount; nCluster++ )
			{
				VHACD_GetBufferSize( obj, nCluster, out var clusterPointCount, out var clusterTriangleCount );

				Vector3F[] clusterVertices = new Vector3F[ clusterPointCount ];
				int[] clusterIndices = new int[ clusterTriangleCount * 3 ];

				fixed( void* pPoints = clusterVertices )
				fixed( int* pIndices = clusterIndices )
					VHACD_GetBuffer32( obj, nCluster, (float*)pPoints, pIndices );

				items[ nCluster ] = new Cluster( clusterVertices, clusterIndices );
			}

			VHACD_Delete( obj );

			return items;
		}

		/// <summary>
		/// Represents a settings for <see cref="ConvexDecomposition"/>.
		/// </summary>
		public class Settings
		{
			int maxConvexHulls = 1;
			[DefaultValue( 1 )]
			[Range( 1, 16 )]
			[Category( "Basic" )]
			[Description( "Maximum number of convex hulls to produce." )]
			public int MaxConvexHulls
			{
				get { return maxConvexHulls; }
				set { maxConvexHulls = value; }
			}

			int maxConvexTriangles = 64;
			[DefaultValue( 64 )]
			[Range( 4, 1024, RangeAttribute.ConvenientDistributionEnum.Exponential )]
			[Category( "Basic" )]
			[Description( "Maximum number of triangles per convex-hull." )]
			public int MaxConvexTriangles
			{
				get { return maxConvexTriangles; }
				set { maxConvexTriangles = value; }
			}

			double minConvexVolume = 0.0001;
			[DefaultValue( 0.0001 )]
			[Range( 0.0, 0.01, RangeAttribute.ConvenientDistributionEnum.Exponential )]
			[Category( "Advanced" )]
			[Description( "Controls the adaptive sampling of the generated convex-hulls." )]
			public double MinConvexVolume
			{
				get { return minConvexVolume; }
				set { minConvexVolume = value; }
			}

			bool convexApproximation = true;
			[DefaultValue( true )]
			[Category( "Advanced" )]
			[Description( "Enables approximation when computing convex-hulls." )]
			public bool ConvexApproximation
			{
				get { return convexApproximation; }
				set { convexApproximation = value; }
			}

			int maxResolution = 100000;
			[DefaultValue( 100000 )]
			[Range( 10000, 16000000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
			[Category( "Advanced" )]
			[Description( "Maximum number of voxels generated during the voxelization stage." )]
			public int MaxResolution
			{
				get { return maxResolution; }
				set { maxResolution = value; }
			}

			double maxConcavity = 0.0025;
			[DefaultValue( 0.0025 )]
			[Range( 0.0, 1.0, RangeAttribute.ConvenientDistributionEnum.Exponential )]
			[Category( "Advanced" )]
			[Description( "Maximum allowed concavity." )]
			public double MaxConcavity
			{
				get { return maxConcavity; }
				set { maxConcavity = value; }
			}

			double alpha = 0.05;
			[DefaultValue( 0.05 )]
			[Range( 0.0, 1.0 )]
			[Category( "Advanced" )]
			[Description( "Controls the bias toward clipping along symmetry planes." )]
			public double Alpha
			{
				get { return alpha; }
				set { alpha = value; }
			}

			double beta = 0.05;
			[DefaultValue( 0.05 )]
			[Range( 0.0, 1.0 )]
			[Category( "Advanced" )]
			[Description( "Controls the bias toward clipping along revolution axes." )]
			public double Beta
			{
				get { return beta; }
				set { beta = value; }
			}

			int planeDownsampling = 4;
			[DefaultValue( 4 )]
			[Range( 1, 16 )]
			[Category( "Advanced" )]
			[Description( "Controls the granularity of the search for the \"best\" clipping plane." )]
			public int PlaneDownsampling
			{
				get { return planeDownsampling; }
				set { planeDownsampling = value; }
			}

			int hullDownsampling = 4;
			[DefaultValue( 4 )]
			[Range( 1, 16 )]
			[Category( "Advanced" )]
			[Description( "Controls the precision of the convex-hull generation process during the clipping plane selection stage." )]
			public int HullDownsampling
			{
				get { return hullDownsampling; }
				set { hullDownsampling = value; }
			}

			bool normalizeMesh = false;
			[DefaultValue( false )]
			[Category( "Advanced" )]
			[Description( "Enables normalizing the mesh before applying the convex decomposition." )]
			public bool NormalizeMesh
			{
				get { return normalizeMesh; }
				set { normalizeMesh = value; }
			}

			bool tetrahedronMode = false;
			[DefaultValue( false )]
			[Category( "Advanced" )]
			[Description( "False: voxel-based approximate convex decomposition, True: tetrahedron-based approximate convex decomposition." )]
			public bool TetrahedronMode
			{
				get { return tetrahedronMode; }
				set { tetrahedronMode = value; }
			}
		}

		public static unsafe Cluster[] Decompose( Vector3F[] vertices, int[] indices, Settings settings )
		{
			return Decompose( vertices, indices,
				settings.MaxConvexHulls,
				settings.MaxConvexTriangles,
				settings.MinConvexVolume,
				settings.ConvexApproximation,
				settings.MaxResolution,
				settings.MaxConcavity,
				settings.Alpha,
				settings.Beta,
				settings.PlaneDownsampling,
				settings.HullDownsampling,
				settings.NormalizeMesh,
				settings.TetrahedronMode );
		}
	}
}
