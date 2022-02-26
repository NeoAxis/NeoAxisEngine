// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//!!!!Add original copyright notice
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Class for calculating convex hull from a mesh.
	/// </summary>
	class ConvexHullAlgorithm
	{
		/// <summary>
		/// Creates a convex hull of the input data.
		/// </summary>
		/// <typeparam name="TVertex">The type of the t vertex.</typeparam>
		/// <typeparam name="TFace">The type of the t face.</typeparam>
		/// <param name="data">The data.</param>
		/// <param name="PlaneDistanceTolerance">The plane distance tolerance (default is 1e-10). If too high, points 
		/// will be missed. If too low, the algorithm may break. Only adjust if you notice problems.</param>
		/// <returns>
		/// ConvexHull&lt;TVertex, TFace&gt;.
		/// </returns>
		/*public */
		static ConvexHull<TVertex, TFace> Create<TVertex, TFace>( IList<TVertex> data,
 double PlaneDistanceTolerance = Constants.DefaultPlaneDistanceTolerance )
 where TVertex : IVertex
 where TFace : ConvexFace<TVertex, TFace>, new()
		{
			return ConvexHull<TVertex, TFace>.Create( data, PlaneDistanceTolerance );
		}

		/// <summary>
		/// Creates a convex hull of the input data.
		/// </summary>
		/// <typeparam name="TVertex">The type of the t vertex.</typeparam>
		/// <param name="data">The data.</param>
		/// <param name="PlaneDistanceTolerance">The plane distance tolerance (default is 1e-10). If too high, points 
		/// will be missed. If too low, the algorithm may break. Only adjust if you notice problems.</param>
		/// <returns>
		/// ConvexHull&lt;TVertex, DefaultConvexFace&lt;TVertex&gt;&gt;.
		/// </returns>
		/*public */
		static ConvexHull<TVertex, ConvexFace<TVertex>> Create<TVertex>( IList<TVertex> data,
double PlaneDistanceTolerance = Constants.DefaultPlaneDistanceTolerance )
where TVertex : IVertex
		{
			return ConvexHull<TVertex, ConvexFace<TVertex>>.Create( data, PlaneDistanceTolerance );
		}

		/// <summary>
		/// Creates a convex hull of the input data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="PlaneDistanceTolerance">The plane distance tolerance (default is 1e-10). If too high, points 
		/// will be missed. If too low, the algorithm may break. Only adjust if you notice problems.</param>
		/// <returns>
		/// ConvexHull&lt;DefaultVertex, DefaultConvexFace&lt;DefaultVertex&gt;&gt;.
		/// </returns>
		/*public */
		static ConvexHull<ConvexVertex, ConvexFace<ConvexVertex>> Create( IList<double[]> data,
double PlaneDistanceTolerance = Constants.DefaultPlaneDistanceTolerance )
		{
			var points = data.Select( p => new ConvexVertex { Position = p } ).ToList();
			return ConvexHull<ConvexVertex, ConvexFace<ConvexVertex>>.Create( points, PlaneDistanceTolerance );
		}

		/// <summary>
		/// Creates a convex hull of the input data.
		/// </summary>
		/*public */
		static ConvexHull<ConvexVertex, ConvexFace<ConvexVertex>> Create( Vector3[] data,
double PlaneDistanceTolerance = Constants.DefaultPlaneDistanceTolerance )
		{
			var points = data.Select( p => new ConvexVertex { Position = new double[] { p.X, p.Y, p.Z } } ).ToList();

			return ConvexHull<ConvexVertex, ConvexFace<ConvexVertex>>.Create( points, PlaneDistanceTolerance );
		}

		/// <summary>
		/// Creates a convex hull of the input data.
		/// </summary>
		/*public */
		static ConvexHull<ConvexVertex, ConvexFace<ConvexVertex>> Create( Vector3F[] data,
double PlaneDistanceTolerance = Constants.DefaultPlaneDistanceTolerance )
		{
			var points = data.Select( p => new ConvexVertex { Position = new double[] { p.X, p.Y, p.Z } } ).ToList();

			return ConvexHull<ConvexVertex, ConvexFace<ConvexVertex>>.Create( points, PlaneDistanceTolerance );
		}

		/// <summary>
		/// Creates a convex hull of the input data.
		/// </summary>
		/*public */
		static ConvexHull<ConvexVertex, ConvexFace<ConvexVertex>> Create( Vector3[] vdata, int[] idata,
double PlaneDistanceTolerance = Constants.DefaultPlaneDistanceTolerance )
		{
			var points = idata.Select( i => new ConvexVertex { Position = new double[] { vdata[ i ].X, vdata[ i ].Y, vdata[ i ].Z } } ).ToList();

			return ConvexHull<ConvexVertex, ConvexFace<ConvexVertex>>.Create( points, PlaneDistanceTolerance );
		}

		/// <summary>
		/// Creates a convex hull of the input data.
		/// </summary>
		/*public */
		static ConvexHull<ConvexVertex, ConvexFace<ConvexVertex>> Create( Vector3F[] vdata, int[] idata,
double PlaneDistanceTolerance = Constants.DefaultPlaneDistanceTolerance )
		{
			var points = idata.Select( i => new ConvexVertex { Position = new double[] { vdata[ i ].X, vdata[ i ].Y, vdata[ i ].Z } } ).ToList();

			return ConvexHull<ConvexVertex, ConvexFace<ConvexVertex>>.Create( points, PlaneDistanceTolerance );
		}

		/// <summary>
		/// Representation of a convex hull.
		/// </summary>
		/// <typeparam name="TVertex">The type of the t vertex.</typeparam>
		/// <typeparam name="TFace">The type of the t face.</typeparam>
		/*public */
		class ConvexHull<TVertex, TFace>
where TVertex : IVertex
where TFace : ConvexFace<TVertex, TFace>, new()
		{
			/// <summary>
			/// Can only be created using a factory method.
			/// </summary>
			internal ConvexHull()
			{
			}

			/// <summary>
			/// Points of the convex hull.
			/// </summary>
			/// <value>The points.</value>
			public TVertex[] Points { get; internal set; }

			/// <summary>
			/// Faces of the convex hull.
			/// </summary>
			/// <value>The faces.</value>
			public TFace[] Faces { get; internal set; }

			/// <summary>
			/// Creates the convex hull.
			/// </summary>
			/// <param name="data">The data.</param>
			/// <param name="PlaneDistanceTolerance">The plane distance tolerance.</param>
			/// <returns>
			/// ConvexHull&lt;TVertex, TFace&gt;.
			/// </returns>
			/// <exception cref="System.ArgumentNullException">The supplied data is null.</exception>
			/// <exception cref="ArgumentNullException">data</exception>
			public static ConvexHull<TVertex, TFace> Create( IList<TVertex> data, double PlaneDistanceTolerance )
			{
				if( data == null )
					throw new ArgumentNullException( "The supplied data is null." );

				return GetConvexHull<TVertex, TFace>( data, PlaneDistanceTolerance );
			}
		}

		/// <summary>
		/// An interface for a structure with nD position.
		/// </summary>
		/*public */
		interface IVertex
		{
			/// <summary>
			/// Position of the vertex.
			/// </summary>
			/// <value>The position.</value>
			double[] Position { get; }
		}

		/// <summary>
		/// A convex face representation containing adjacency information.
		/// </summary>
		/// <typeparam name="TVertex">The type of the t vertex.</typeparam>
		/// <typeparam name="TFace">The type of the t face.</typeparam>
		/*public */
		abstract class ConvexFace<TVertex, TFace>
where TVertex : IVertex
where TFace : ConvexFace<TVertex, TFace>
		{
			/// <summary>
			/// Adjacency. Array of length "dimension".
			/// If F = Adjacency[i] then the vertices shared with F are Vertices[j] where j != i.
			/// In the context of triangulation, can be null (indicates the cell is at boundary).
			/// </summary>
			/// <value>The adjacency.</value>
			public TFace[] Adjacency { get; set; }

			/// <summary>
			/// The vertices stored in clockwise order for dimensions 2 - 4, in higher dimensions the order is arbitrary.
			/// Unless I accidentally switch some index somewhere in which case the order is CCW. Either way, it is consistent.
			/// 3D Normal = (V[1] - V[0]) x (V[2] - V[1]).
			/// </summary>
			/// <value>The vertices.</value>
			public TVertex[] Vertices { get; set; }

			/// <summary>
			/// The normal vector of the face. Null if used in triangulation.
			/// </summary>
			/// <value>The normal.</value>
			public double[] Normal { get; set; }
		}

		internal static class Constants
		{
			/// <summary>
			/// The default plane distance tolerance
			/// </summary>
			internal const double DefaultPlaneDistanceTolerance = 1e-10;
			/// <summary>
			/// The starting delta dot product in simplex
			/// </summary>
			internal const double StartingDeltaDotProductInSimplex = 0.5;
			/// <summary>
			/// The connector table size
			/// </summary>
			internal const int ConnectorTableSize = 2017;
		}

		/// <summary>
		/// A helper class for object allocation/storage.
		/// This helps the GC a lot as it prevents the creation of about 75% of
		/// new face objects (in the case of ConvexFaceInternal). In the case of
		/// FaceConnectors and DefferedFaces, the difference is even higher (in most
		/// cases O(1) vs O(number of created faces)).
		/// </summary>
		internal class MemManager
		{
			/// <summary>
			/// The dimension
			/// </summary>
			private readonly int Dimension;
			/// <summary>
			/// The connector stack
			/// </summary>
			private FaceConnector ConnectorStack;
			/// <summary>
			/// The deferred face stack
			/// </summary>
			private readonly SimpleList<DeferredFace> DeferredFaceStack;
			/// <summary>
			/// The empty buffer stack
			/// </summary>
			private readonly SimpleList<IndexBuffer> EmptyBufferStack;
			/// <summary>
			/// The face pool
			/// </summary>
			private ConvexFaceInternal[] FacePool;
			/// <summary>
			/// The face pool size
			/// </summary>
			private int FacePoolSize;
			/// <summary>
			/// The face pool capacity
			/// </summary>
			private int FacePoolCapacity;
			/// <summary>
			/// The free face indices
			/// </summary>
			private readonly IndexBuffer FreeFaceIndices;

			/// <summary>
			/// The hull
			/// </summary>
			private readonly ConvexHullAlgorithm Hull;

			/// <summary>
			/// Create the manager.
			/// </summary>
			/// <param name="hull">The hull.</param>
			public MemManager( ConvexHullAlgorithm hull )
			{
				Dimension = hull.NumOfDimensions;
				Hull = hull;
				FacePool = hull.FacePool;
				FacePoolSize = 0;
				FacePoolCapacity = hull.FacePool.Length;
				FreeFaceIndices = new IndexBuffer();

				EmptyBufferStack = new SimpleList<IndexBuffer>();
				DeferredFaceStack = new SimpleList<DeferredFace>();
			}

			/// <summary>
			/// Return the face to the pool for later use.
			/// </summary>
			/// <param name="faceIndex">Index of the face.</param>
			public void DepositFace( int faceIndex )
			{
				var face = FacePool[ faceIndex ];
				var af = face.AdjacentFaces;
				for( var i = 0; i < af.Length; i++ )
				{
					af[ i ] = -1;
				}
				FreeFaceIndices.Push( faceIndex );
			}

			/// <summary>
			/// Reallocate the face pool, including the AffectedFaceFlags
			/// </summary>
			private void ReallocateFacePool()
			{
				var newPool = new ConvexFaceInternal[ 2 * FacePoolCapacity ];
				var newTags = new bool[ 2 * FacePoolCapacity ];
				Array.Copy( FacePool, newPool, FacePoolCapacity );
				Buffer.BlockCopy( Hull.AffectedFaceFlags, 0, newTags, 0, FacePoolCapacity * sizeof( bool ) );
				FacePoolCapacity = 2 * FacePoolCapacity;
				Hull.FacePool = newPool;
				FacePool = newPool;
				Hull.AffectedFaceFlags = newTags;
			}

			/// <summary>
			/// Create a new face and put it in the pool.
			/// </summary>
			/// <returns>System.Int32.</returns>
			private int CreateFace()
			{
				var index = FacePoolSize;
				var face = new ConvexFaceInternal( Dimension, index, GetVertexBuffer() );
				FacePoolSize++;
				if( FacePoolSize > FacePoolCapacity ) ReallocateFacePool();
				FacePool[ index ] = face;
				return index;
			}

			/// <summary>
			/// Return index of an unused face or creates a new one.
			/// </summary>
			/// <returns>System.Int32.</returns>
			public int GetFace()
			{
				if( FreeFaceIndices.Count > 0 ) return FreeFaceIndices.Pop();
				return CreateFace();
			}

			/// <summary>
			/// Store a face connector in the "embedded" linked list.
			/// </summary>
			/// <param name="connector">The connector.</param>
			public void DepositConnector( FaceConnector connector )
			{
				if( ConnectorStack == null )
				{
					connector.Next = null;
					ConnectorStack = connector;
				}
				else
				{
					connector.Next = ConnectorStack;
					ConnectorStack = connector;
				}
			}

			/// <summary>
			/// Get an unused face connector. If none is available, create it.
			/// </summary>
			/// <returns>FaceConnector.</returns>
			public FaceConnector GetConnector()
			{
				if( ConnectorStack == null ) return new FaceConnector( Dimension );

				var ret = ConnectorStack;
				ConnectorStack = ConnectorStack.Next;
				ret.Next = null;
				return ret;
			}

			/// <summary>
			/// Deposit the index buffer.
			/// </summary>
			/// <param name="buffer">The buffer.</param>
			public void DepositVertexBuffer( IndexBuffer buffer )
			{
				buffer.Clear();
				EmptyBufferStack.Push( buffer );
			}

			/// <summary>
			/// Get a store index buffer or create a new instance.
			/// </summary>
			/// <returns>IndexBuffer.</returns>
			public IndexBuffer GetVertexBuffer()
			{
				return EmptyBufferStack.Count != 0 ? EmptyBufferStack.Pop() : new IndexBuffer();
			}

			/// <summary>
			/// Deposit the deferred face.
			/// </summary>
			/// <param name="face">The face.</param>
			public void DepositDeferredFace( DeferredFace face )
			{
				DeferredFaceStack.Push( face );
			}

			/// <summary>
			/// Get the deferred face.
			/// </summary>
			/// <returns>DeferredFace.</returns>
			public DeferredFace GetDeferredFace()
			{
				return DeferredFaceStack.Count != 0 ? DeferredFaceStack.Pop() : new DeferredFace();
			}
		}

		/// <summary>
		/// A helper class mostly for normal computation. If convex hulls are computed
		/// in higher dimensions, it might be a good idea to add a specific
		/// FindNormalVectorND function.
		/// </summary>
		internal class MathHelper
		{
			/// <summary>
			/// The dimension
			/// </summary>
			private readonly int Dimension;
			/// <summary>
			/// The matrix pivots
			/// </summary>
			private readonly int[] matrixPivots;
			/// <summary>
			/// The n d matrix
			/// </summary>
			private readonly double[] nDMatrix;
			/// <summary>
			/// The n d normal helper vector
			/// </summary>
			private readonly double[] nDNormalHelperVector;

			/// <summary>
			/// The nt x
			/// </summary>
			private readonly double[] ntX;
			/// <summary>
			/// The nt y
			/// </summary>
			private readonly double[] ntY;
			/// <summary>
			/// The nt z
			/// </summary>
			private readonly double[] ntZ;

			/// <summary>
			/// The position data
			/// </summary>
			private readonly double[] PositionData;

			/// <summary>
			/// Initializes a new instance of the <see cref="MathHelper"/> class.
			/// </summary>
			/// <param name="dimension">The dimension.</param>
			/// <param name="positions">The positions.</param>
			internal MathHelper( int dimension, double[] positions )
			{
				PositionData = positions;
				Dimension = dimension;

				ntX = new double[ Dimension ];
				ntY = new double[ Dimension ];
				ntZ = new double[ Dimension ];

				nDNormalHelperVector = new double[ Dimension ];
				nDMatrix = new double[ Dimension * Dimension ];
				matrixPivots = new int[ Dimension ];
			}

			/// <summary>
			/// Calculates the normal and offset of the hyper-plane given by the face's vertices.
			/// </summary>
			/// <param name="face">The face.</param>
			/// <param name="center">The center.</param>
			/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
			internal bool CalculateFacePlane( ConvexFaceInternal face, double[] center )
			{
				var vertices = face.Vertices;
				var normal = face.Normal;
				FindNormalVector( vertices, normal );

				if( double.IsNaN( normal[ 0 ] ) )
				{
					return false;
				}

				var offset = 0.0;
				var centerDistance = 0.0;
				var fi = vertices[ 0 ] * Dimension;
				for( var i = 0; i < Dimension; i++ )
				{
					var n = normal[ i ];
					offset += n * PositionData[ fi + i ];
					centerDistance += n * center[ i ];
				}
				face.Offset = -offset;
				centerDistance -= offset;

				if( centerDistance > 0 )
				{
					for( var i = 0; i < Dimension; i++ ) normal[ i ] = -normal[ i ];
					face.Offset = offset;
					face.IsNormalFlipped = true;
				}
				else face.IsNormalFlipped = false;

				return true;
			}

			/// <summary>
			/// Check if the vertex is "visible" from the face.
			/// The vertex is "over face" if the return value is &gt; Constants.PlaneDistanceTolerance.
			/// </summary>
			/// <param name="v">The v.</param>
			/// <param name="f">The f.</param>
			/// <returns>The vertex is "over face" if the result is positive.</returns>
			internal double GetVertexDistance( int v, ConvexFaceInternal f )
			{
				var normal = f.Normal;
				var x = v * Dimension;
				var distance = f.Offset;
				for( var i = 0; i < normal.Length; i++ ) distance += normal[ i ] * PositionData[ x + i ];
				return distance;
			}

			/// <summary>
			/// Returns the vector the between vertices.
			/// </summary>
			/// <param name="toIndex">To index.</param>
			/// <param name="fromIndex">From index.</param>
			/// <returns>System.Double[].</returns>
			internal double[] VectorBetweenVertices( int toIndex, int fromIndex )
			{
				var target = new double[ Dimension ];
				VectorBetweenVertices( toIndex, fromIndex, target );
				return target;
			}
			/// <summary>
			/// Returns the vector the between vertices.
			/// </summary>
			/// <param name="fromIndex">From index.</param>
			/// <param name="toIndex">To index.</param>
			/// <param name="target">The target.</param>
			/// <returns></returns>
			private void VectorBetweenVertices( int toIndex, int fromIndex, double[] target )
			{
				int u = toIndex * Dimension, v = fromIndex * Dimension;
				for( var i = 0; i < Dimension; i++ )
				{
					target[ i ] = PositionData[ u + i ] - PositionData[ v + i ];
				}
			}

			internal void RandomOffsetToLift( int index, double maxHeight )
			{
				var random = new System.Random();
				var liftIndex = ( index * Dimension ) + Dimension - 1;
				PositionData[ liftIndex ] += 0.0001 * maxHeight * ( random.NextDouble() - 0.5 );
			}
			#region Find the normal vector of the face
			/// <summary>
			/// Finds normal vector of a hyper-plane given by vertices.
			/// Stores the results to normalData.
			/// </summary>
			/// <param name="vertices">The vertices.</param>
			/// <param name="normalData">The normal data.</param>
			private void FindNormalVector( int[] vertices, double[] normalData )
			{
				switch( Dimension )
				{
				case 2:
					FindNormalVector2D( vertices, normalData );
					break;
				case 3:
					FindNormalVector3D( vertices, normalData );
					break;
				case 4:
					FindNormalVector4D( vertices, normalData );
					break;
				default:
					FindNormalVectorND( vertices, normalData );
					break;
				}
			}
			/// <summary>
			/// Finds 2D normal vector.
			/// </summary>
			/// <param name="vertices">The vertices.</param>
			/// <param name="normal">The normal.</param>
			private void FindNormalVector2D( int[] vertices, double[] normal )
			{
				VectorBetweenVertices( vertices[ 1 ], vertices[ 0 ], ntX );

				var nx = -ntX[ 1 ];
				var ny = ntX[ 0 ];

				var norm = Math.Sqrt( nx * nx + ny * ny );

				var f = 1.0 / norm;
				normal[ 0 ] = f * nx;
				normal[ 1 ] = f * ny;
			}
			/// <summary>
			/// Finds 3D normal vector.
			/// </summary>
			/// <param name="vertices">The vertices.</param>
			/// <param name="normal">The normal.</param>
			private void FindNormalVector3D( int[] vertices, double[] normal )
			{
				VectorBetweenVertices( vertices[ 1 ], vertices[ 0 ], ntX );
				VectorBetweenVertices( vertices[ 2 ], vertices[ 1 ], ntY );

				var nx = ntX[ 1 ] * ntY[ 2 ] - ntX[ 2 ] * ntY[ 1 ];
				var ny = ntX[ 2 ] * ntY[ 0 ] - ntX[ 0 ] * ntY[ 2 ];
				var nz = ntX[ 0 ] * ntY[ 1 ] - ntX[ 1 ] * ntY[ 0 ];

				var norm = Math.Sqrt( nx * nx + ny * ny + nz * nz );

				var f = 1.0 / norm;
				normal[ 0 ] = f * nx;
				normal[ 1 ] = f * ny;
				normal[ 2 ] = f * nz;
			}
			/// <summary>
			/// Finds 4D normal vector.
			/// </summary>
			/// <param name="vertices">The vertices.</param>
			/// <param name="normal">The normal.</param>
			private void FindNormalVector4D( int[] vertices, double[] normal )
			{
				VectorBetweenVertices( vertices[ 1 ], vertices[ 0 ], ntX );
				VectorBetweenVertices( vertices[ 2 ], vertices[ 1 ], ntY );
				VectorBetweenVertices( vertices[ 3 ], vertices[ 2 ], ntZ );

				var x = ntX;
				var y = ntY;
				var z = ntZ;

				// This was generated using Mathematica
				var nx = x[ 3 ] * ( y[ 2 ] * z[ 1 ] - y[ 1 ] * z[ 2 ] )
						 + x[ 2 ] * ( y[ 1 ] * z[ 3 ] - y[ 3 ] * z[ 1 ] )
						 + x[ 1 ] * ( y[ 3 ] * z[ 2 ] - y[ 2 ] * z[ 3 ] );
				var ny = x[ 3 ] * ( y[ 0 ] * z[ 2 ] - y[ 2 ] * z[ 0 ] )
						 + x[ 2 ] * ( y[ 3 ] * z[ 0 ] - y[ 0 ] * z[ 3 ] )
						 + x[ 0 ] * ( y[ 2 ] * z[ 3 ] - y[ 3 ] * z[ 2 ] );
				var nz = x[ 3 ] * ( y[ 1 ] * z[ 0 ] - y[ 0 ] * z[ 1 ] )
						 + x[ 1 ] * ( y[ 0 ] * z[ 3 ] - y[ 3 ] * z[ 0 ] )
						 + x[ 0 ] * ( y[ 3 ] * z[ 1 ] - y[ 1 ] * z[ 3 ] );
				var nw = x[ 2 ] * ( y[ 0 ] * z[ 1 ] - y[ 1 ] * z[ 0 ] )
						 + x[ 1 ] * ( y[ 2 ] * z[ 0 ] - y[ 0 ] * z[ 2 ] )
						 + x[ 0 ] * ( y[ 1 ] * z[ 2 ] - y[ 2 ] * z[ 1 ] );

				var norm = Math.Sqrt( nx * nx + ny * ny + nz * nz + nw * nw );

				var f = 1.0 / norm;
				normal[ 0 ] = f * nx;
				normal[ 1 ] = f * ny;
				normal[ 2 ] = f * nz;
				normal[ 3 ] = f * nw;
			}

			/// <summary>
			/// Finds the normal vector nd.
			/// </summary>
			/// <param name="vertices">The vertices.</param>
			/// <param name="normal">The normal.</param>
			private void FindNormalVectorND( int[] vertices, double[] normal )
			{
				/* We need to solve the matrix A n = B where
				 *  - A contains coordinates of vertices as columns
				 *  - B is vector with all 1's. Really, it should be the distance of 
				 *      the plane from the origin, but - since we're not worried about that
				 *      here and we will normalize the normal anyway - all 1's suffices.
				 */
				var iPiv = matrixPivots;
				var data = nDMatrix;
				var norm = 0.0;

				// Solve determinants by replacing x-th column by all 1.
				for( var x = 0; x < Dimension; x++ )
				{
					for( var i = 0; i < Dimension; i++ )
					{
						var offset = vertices[ i ] * Dimension;
						for( var j = 0; j < Dimension; j++ )
						{
							// maybe I got the i/j mixed up here regarding the representation Math.net uses...
							// ...but it does not matter since Det(A) = Det(Transpose(A)).
							data[ Dimension * i + j ] = j == x ? 1.0 : PositionData[ offset + j ];
						}
					}
					LUFactor( data, Dimension, iPiv, nDNormalHelperVector );
					var coord = 1.0;
					for( var i = 0; i < Dimension; i++ )
					{
						if( iPiv[ i ] != i ) coord *= -data[ Dimension * i + i ]; // the determinant sign changes on row swap.
						else coord *= data[ Dimension * i + i ];
					}
					normal[ x ] = coord;
					norm += coord * coord;
				}

				// Normalize the result
				var f = 1.0 / Math.Sqrt( norm );
				for( var i = 0; i < normal.Length; i++ ) normal[ i ] *= f;
			}
			#endregion

			#region Simplex Volume
			/// <summary>
			/// Gets the simplex volume. Prior to having enough edge vectors, the method pads the remaining with all
			/// "other numbers". So, yes, this method is not really finding the volume. But a relative volume-like measure. It
			/// uses the magnitude of the determinant as the volume stand-in following the Cayley-Menger theorem.
			/// </summary>
			/// <param name="edgeVectors">The edge vectors.</param>
			/// <param name="lastIndex">The last index.</param>
			/// <param name="bigNumber">The big number.</param>
			/// <returns>System.Double.</returns>
			internal double GetSimplexVolume( double[][] edgeVectors, int lastIndex, double bigNumber )
			{
				var A = new double[ Dimension * Dimension ];
				var index = 0;
				for( int i = 0; i < Dimension; i++ )
					for( int j = 0; j < Dimension; j++ )
						if( i <= lastIndex )
							A[ index++ ] = edgeVectors[ i ][ j ];
						else A[ index ] = ( Math.Pow( -1, index ) * index++ ) / bigNumber;
				// this last term is used for all the vertices in the comparison for the yet determined vertices
				// the idea is to come up with sets of numbers that are orthogonal so that an non-zero value will result
				// and to choose smallish numbers since the choice of vectors will affect what the end volume is.
				// A better way (todo?) is to solve a smaller matrix. However, cases were found in which the obvious smaller vector
				// (the upper left) had too many zeros. So, one would need to find the right subset. Indeed choosing a subset
				// biases the first dimensions of the others. Perhaps a larger volume would be created from a different vertex
				// if another subset of dimensions were used. 
				return Math.Abs( DeterminantDestructive( A ) );
			}

			/// <summary>
			/// Determinants the destructive.
			/// </summary>
			/// <param name="A">a.</param>
			/// <returns>System.Double.</returns>
			private double DeterminantDestructive( double[] A )
			{
				switch( Dimension )
				{
				case 0:
					return 0.0;
				case 1:
					return A[ 0 ];
				case 2:
					return A[ 0 ] * A[ 3 ] - A[ 1 ] * A[ 2 ];
				case 3:
					return A[ 0 ] * A[ 4 ] * A[ 8 ] + A[ 1 ] * A[ 5 ] * A[ 6 ] + A[ 2 ] * A[ 3 ] * A[ 7 ]
						   - A[ 0 ] * A[ 5 ] * A[ 7 ] - A[ 1 ] * A[ 3 ] * A[ 8 ] - A[ 2 ] * A[ 4 ] * A[ 6 ];
				default:
					{
						var iPiv = new int[ Dimension ];
						var helper = new double[ Dimension ];
						LUFactor( A, Dimension, iPiv, helper );
						var det = 1.0;
						for( var i = 0; i < iPiv.Length; i++ )
						{
							det *= A[ Dimension * i + i ];
							if( iPiv[ i ] != i ) det *= -1; // the determinant sign changes on row swap.
						}
						return det;
					}
				}
			}
			#endregion


			// Modified from Math.NET
			// Copyright (c) 2009-2013 Math.NET
			/// <summary>
			/// Lus the factor.
			/// </summary>
			/// <param name="data">The data.</param>
			/// <param name="order">The order.</param>
			/// <param name="ipiv">The ipiv.</param>
			/// <param name="vecLUcolj">The vec l ucolj.</param>
			private static void LUFactor( double[] data, int order, int[] ipiv, double[] vecLUcolj )
			{
				// Initialize the pivot matrix to the identity permutation.
				for( var i = 0; i < order; i++ )
				{
					ipiv[ i ] = i;
				}

				// Outer loop.
				for( var j = 0; j < order; j++ )
				{
					var indexj = j * order;
					var indexjj = indexj + j;

					// Make a copy of the j-th column to localize references.
					for( var i = 0; i < order; i++ )
					{
						vecLUcolj[ i ] = data[ indexj + i ];
					}

					// Apply previous transformations.
					for( var i = 0; i < order; i++ )
					{
						// Most of the time is spent in the following dot product.
						var kmax = Math.Min( i, j );
						var s = 0.0;
						for( var k = 0; k < kmax; k++ )
						{
							s += data[ k * order + i ] * vecLUcolj[ k ];
						}

						data[ indexj + i ] = vecLUcolj[ i ] -= s;
					}

					// Find pivot and exchange if necessary.
					var p = j;
					for( var i = j + 1; i < order; i++ )
					{
						if( Math.Abs( vecLUcolj[ i ] ) > Math.Abs( vecLUcolj[ p ] ) )
						{
							p = i;
						}
					}

					if( p != j )
					{
						for( var k = 0; k < order; k++ )
						{
							var indexk = k * order;
							var indexkp = indexk + p;
							var indexkj = indexk + j;
							var temp = data[ indexkp ];
							data[ indexkp ] = data[ indexkj ];
							data[ indexkj ] = temp;
						}

						ipiv[ j ] = p;
					}

					// Compute multipliers.
					if( j < order & data[ indexjj ] != 0.0 )
					{
						for( var i = j + 1; i < order; i++ )
						{
							data[ indexj + i ] /= data[ indexjj ];
						}
					}
				}
			}
		}

		/// <summary>
		/// For deferred face addition.
		/// </summary>
		internal sealed class DeferredFace
		{
			/// <summary>
			/// The faces.
			/// </summary>
			public ConvexFaceInternal Face, Pivot, OldFace;

			/// <summary>
			/// The indices.
			/// </summary>
			public int FaceIndex, PivotIndex;
		}

		/// <summary>
		/// A helper class used to connect faces.
		/// </summary>
		internal sealed class FaceConnector
		{
			/// <summary>
			/// The edge to be connected.
			/// </summary>
			public int EdgeIndex;

			/// <summary>
			/// The face.
			/// </summary>
			public ConvexFaceInternal Face;

			/// <summary>
			/// The hash code computed from indices.
			/// </summary>
			public uint HashCode;

			/// <summary>
			/// Next node in the list.
			/// </summary>
			public FaceConnector Next;

			/// <summary>
			/// Prev node in the list.
			/// </summary>
			public FaceConnector Previous;

			/// <summary>
			/// The vertex indices.
			/// </summary>
			public int[] Vertices;

			/// <summary>
			/// Ctor.
			/// </summary>
			/// <param name="dimension">The dimension.</param>
			public FaceConnector( int dimension )
			{
				Vertices = new int[ dimension - 1 ];
			}

			/// <summary>
			/// Updates the connector.
			/// </summary>
			/// <param name="face">The face.</param>
			/// <param name="edgeIndex">Index of the edge.</param>
			/// <param name="dim">The dim.</param>
			public void Update( ConvexFaceInternal face, int edgeIndex, int dim )
			{
				Face = face;
				EdgeIndex = edgeIndex;

				uint hashCode = 23;

				unchecked
				{
					var vs = face.Vertices;
					int i, c = 0;
					for( i = 0; i < edgeIndex; i++ )
					{
						Vertices[ c++ ] = vs[ i ];
						hashCode += 31 * hashCode + (uint)vs[ i ];
					}
					for( i = edgeIndex + 1; i < vs.Length; i++ )
					{
						Vertices[ c++ ] = vs[ i ];
						hashCode += 31 * hashCode + (uint)vs[ i ];
					}
				}

				HashCode = hashCode;
			}

			/// <summary>
			/// Can two faces be connected.
			/// </summary>
			/// <param name="a">a.</param>
			/// <param name="b">The b.</param>
			/// <param name="dim">The dim.</param>
			/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
			public static bool AreConnectable( FaceConnector a, FaceConnector b, int dim )
			{
				if( a.HashCode != b.HashCode ) return false;

				var av = a.Vertices;
				var bv = b.Vertices;
				for( var i = 0; i < av.Length; i++ )
				{
					if( av[ i ] != bv[ i ] ) return false;
				}

				return true;
			}

			/// <summary>
			/// Connect two faces.
			/// </summary>
			/// <param name="a">a.</param>
			/// <param name="b">The b.</param>
			public static void Connect( FaceConnector a, FaceConnector b )
			{
				a.Face.AdjacentFaces[ a.EdgeIndex ] = b.Face.Index;
				b.Face.AdjacentFaces[ b.EdgeIndex ] = a.Face.Index;
			}
		}

		/// <summary>
		/// This internal class manages the faces of the convex hull. It is a
		/// separate class from the desired user class.
		/// </summary>
		internal sealed class ConvexFaceInternal
		{
			/// <summary>
			/// Gets or sets the adjacent face data.
			/// </summary>
			public int[] AdjacentFaces;

			/// <summary>
			/// The furthest vertex.
			/// </summary>
			public int FurthestVertex;

			/// <summary>
			/// Index of the face inside the pool.
			/// </summary>
			public int Index;

			/// <summary>
			/// Is it present in the list.
			/// </summary>
			public bool InList;

			/// <summary>
			/// Is the normal flipped?
			/// </summary>
			public bool IsNormalFlipped;

			/// <summary>
			/// Next node in the list.
			/// </summary>
			public ConvexFaceInternal Next;

			/// <summary>
			/// Gets or sets the normal vector.
			/// </summary>
			public double[] Normal;

			/// <summary>
			/// Face plane constant element.
			/// </summary>
			public double Offset;

			//public int UnprocessedIndex;

			/// <summary>
			/// Prev node in the list.
			/// </summary>
			public ConvexFaceInternal Previous;

			/// <summary>
			/// Used to traverse affected faces and create the Delaunay representation.
			/// </summary>
			public int Tag;

			/// <summary>
			/// Gets or sets the vertices.
			/// </summary>
			public int[] Vertices;

			/// <summary>
			/// Gets or sets the vertices beyond.
			/// </summary>
			public IndexBuffer VerticesBeyond;

			/// <summary>
			/// Initializes a new instance of the <see cref="ConvexFaceInternal" /> class.
			/// </summary>
			/// <param name="dimension">The dimension.</param>
			/// <param name="index">The index.</param>
			/// <param name="beyondList">The beyond list.</param>
			public ConvexFaceInternal( int dimension, int index, IndexBuffer beyondList )
			{
				Index = index;
				AdjacentFaces = new int[ dimension ];
				VerticesBeyond = beyondList;
				Normal = new double[ dimension ];
				Vertices = new int[ dimension ];
			}
		}

		/// <summary>
		/// A more lightweight alternative to List of T.
		/// On clear, only resets the count and does not clear the references
		/// =&gt; this works because of the ObjectManager.
		/// Includes a stack functionality.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		internal class SimpleList<T>
		{
			/// <summary>
			/// The capacity
			/// </summary>
			private int capacity;

			/// <summary>
			/// The count
			/// </summary>
			public int Count;
			/// <summary>
			/// The items
			/// </summary>
			private T[] items;

			/// <summary>
			/// Get the i-th element.
			/// </summary>
			/// <param name="i">The i.</param>
			/// <returns>T.</returns>
			public T this[ int i ]
			{
				get { return items[ i ]; }
				set { items[ i ] = value; }
			}

			/// <summary>
			/// Size matters.
			/// </summary>
			private void EnsureCapacity()
			{
				if( capacity == 0 )
				{
					capacity = 32;
					items = new T[ 32 ];
				}
				else
				{
					var newItems = new T[ capacity * 2 ];
					Array.Copy( items, newItems, capacity );
					capacity = 2 * capacity;
					items = newItems;
				}
			}

			/// <summary>
			/// Adds a vertex to the buffer.
			/// </summary>
			/// <param name="item">The item.</param>
			public void Add( T item )
			{
				if( Count + 1 > capacity ) EnsureCapacity();
				items[ Count++ ] = item;
			}

			/// <summary>
			/// Pushes the value to the back of the list.
			/// </summary>
			/// <param name="item">The item.</param>
			public void Push( T item )
			{
				if( Count + 1 > capacity ) EnsureCapacity();
				items[ Count++ ] = item;
			}

			/// <summary>
			/// Pops the last value from the list.
			/// </summary>
			/// <returns>T.</returns>
			public T Pop()
			{
				return items[ --Count ];
			}

			/// <summary>
			/// Sets the Count to 0, otherwise does nothing.
			/// </summary>
			public void Clear()
			{
				Count = 0;
			}
		}

		/// <summary>
		/// Class IndexBuffer.
		/// A fancy name for a list of integers.
		/// </summary>
		internal class IndexBuffer : SimpleList<int>
		{
		}

		/// <summary>
		/// A priority based linked list.
		/// </summary>
		internal sealed class FaceList
		{
			/// <summary>
			/// The last
			/// </summary>
			private ConvexFaceInternal last;

			/// <summary>
			/// Get the first element.
			/// </summary>
			/// <value>The first.</value>
			public ConvexFaceInternal First { get; private set; }

			/// <summary>
			/// Adds the element to the beginning.
			/// </summary>
			/// <param name="face">The face.</param>
			private void AddFirst( ConvexFaceInternal face )
			{
				face.InList = true;
				First.Previous = face;
				face.Next = First;
				First = face;
			}

			/// <summary>
			/// Adds a face to the list.
			/// </summary>
			/// <param name="face">The face.</param>
			public void Add( ConvexFaceInternal face )
			{
				if( face.InList )
				{
					if( First.VerticesBeyond.Count < face.VerticesBeyond.Count )
					{
						Remove( face );
						AddFirst( face );
					}
					return;
				}

				face.InList = true;

				if( First != null && First.VerticesBeyond.Count < face.VerticesBeyond.Count )
				{
					First.Previous = face;
					face.Next = First;
					First = face;
				}
				else
				{
					if( last != null )
					{
						last.Next = face;
					}
					face.Previous = last;
					last = face;
					if( First == null )
					{
						First = face;
					}
				}
			}

			/// <summary>
			/// Removes the element from the list.
			/// </summary>
			/// <param name="face">The face.</param>
			public void Remove( ConvexFaceInternal face )
			{
				if( !face.InList ) return;

				face.InList = false;

				if( face.Previous != null )
				{
					face.Previous.Next = face.Next;
				}
				else if( /*first == face*/ face.Previous == null )
				{
					First = face.Next;
				}

				if( face.Next != null )
				{
					face.Next.Previous = face.Previous;
				}
				else if( /*last == face*/ face.Next == null )
				{
					last = face.Previous;
				}

				face.Next = null;
				face.Previous = null;
			}
		}

		/// <summary>
		/// Connector list.
		/// </summary>
		internal sealed class ConnectorList
		{
			/// <summary>
			/// The last
			/// </summary>
			private FaceConnector last;

			/// <summary>
			/// Get the first element.
			/// </summary>
			/// <value>The first.</value>
			public FaceConnector First { get; private set; }

			/// <summary>
			/// Adds the element to the beginning.
			/// </summary>
			/// <param name="connector">The connector.</param>
			private void AddFirst( FaceConnector connector )
			{
				First.Previous = connector;
				connector.Next = First;
				First = connector;
			}

			/// <summary>
			/// Adds a face to the list.
			/// </summary>
			/// <param name="element">The element.</param>
			public void Add( FaceConnector element )
			{
				if( last != null )
				{
					last.Next = element;
				}
				element.Previous = last;
				last = element;
				if( First == null )
				{
					First = element;
				}
			}

			/// <summary>
			/// Removes the element from the list.
			/// </summary>
			/// <param name="connector">The connector.</param>
			public void Remove( FaceConnector connector )
			{
				if( connector.Previous != null )
				{
					connector.Previous.Next = connector.Next;
				}
				else if( /*first == face*/ connector.Previous == null )
				{
					First = connector.Next;
				}

				if( connector.Next != null )
				{
					connector.Next.Previous = connector.Previous;
				}
				else if( /*last == face*/ connector.Next == null )
				{
					last = connector.Previous;
				}

				connector.Next = null;
				connector.Previous = null;
			}
		}

		#region Starting functions and constructor

		/// <summary>
		/// The main function for the Convex Hull algorithm. It is static, but it creates
		/// an instantiation of this class in order to allow for parallel execution.
		/// Following this simple function, the constructor and the main function "FindConvexHull" is listed.
		/// </summary>
		/// <typeparam name="TVertex">The type of the vertices in the data.</typeparam>
		/// <typeparam name="TFace">The desired type of the faces.</typeparam>
		/// <param name="data">The data is the vertices as a collection of IVertices.</param>
		/// <param name="PlaneDistanceTolerance">The plane distance tolerance.</param>
		/// <returns>
		/// MIConvexHull.ConvexHull&lt;TVertex, TFace&gt;.
		/// </returns>
		static ConvexHull<TVertex, TFace> GetConvexHull<TVertex, TFace>( IList<TVertex> data,
					double PlaneDistanceTolerance )
					where TFace : ConvexFace<TVertex, TFace>, new()
					where TVertex : IVertex
		{
			var ch = new ConvexHullAlgorithm( data.Cast<IVertex>().ToArray(), false, PlaneDistanceTolerance );

			ch.GetConvexHull();

			if( ch.NumOfDimensions == 2 )
				return ch.Return2DResultInOrder<TVertex, TFace>( data );

			return new ConvexHull<TVertex, TFace>
			{
				Points = ch.GetHullVertices( data ),
				Faces = ch.GetConvexFaces<TVertex, TFace>()
			};
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConvexHullAlgorithm" /> class.
		/// </summary>
		/// <param name="vertices">The vertices.</param>
		/// <param name="lift">if set to <c>true</c> [lift].</param>
		/// <param name="PlaneDistanceTolerance">The plane distance tolerance.</param>
		/// <exception cref="System.InvalidOperationException">Dimension of the input must be 2 or greater.</exception>
		/// <exception cref="System.ArgumentException">There are too few vertices (m) for the n-dimensional space. (m must be greater  +
		/// than the n, but m is  + NumberOfVertices +  and n is  + NumOfDimensions</exception>
		/// <exception cref="InvalidOperationException">PointTranslationGenerator cannot be null if PointTranslationType is enabled.
		/// or
		/// Dimension of the input must be 2 or greater.</exception>
		/// <exception cref="ArgumentException">There are too few vertices (m) for the n-dimensional space. (m must be greater " +
		/// "than the n, but m is " + NumberOfVertices + " and n is " + Dimension</exception>
		private ConvexHullAlgorithm( IVertex[] vertices, bool lift, double PlaneDistanceTolerance )
		{
			IsLifted = lift;
			Vertices = vertices;
			NumberOfVertices = vertices.Length;

			NumOfDimensions = DetermineDimension();
			if( IsLifted ) NumOfDimensions++;
			if( NumOfDimensions < 2 ) throw new InvalidOperationException( "Dimension of the input must be 2 or greater." );
			if( NumberOfVertices <= NumOfDimensions )
				throw new ArgumentException(
					"There are too few vertices (m) for the n-dimensional space. (m must be greater " +
					"than the n, but m is " + NumberOfVertices + " and n is " + NumOfDimensions );
			this.PlaneDistanceTolerance = PlaneDistanceTolerance;
			UnprocessedFaces = new FaceList();
			ConvexFaces = new IndexBuffer();

			FacePool = new ConvexFaceInternal[ ( NumOfDimensions + 1 ) * 10 ]; // must be initialized before object manager
			AffectedFaceFlags = new bool[ ( NumOfDimensions + 1 ) * 10 ];
			ObjectManager = new MemManager( this );

			Center = new double[ NumOfDimensions ];
			TraverseStack = new IndexBuffer();
			UpdateBuffer = new int[ NumOfDimensions ];
			UpdateIndices = new int[ NumOfDimensions ];
			EmptyBuffer = new IndexBuffer();
			AffectedFaceBuffer = new IndexBuffer();
			ConeFaceBuffer = new SimpleList<DeferredFace>();
			SingularVertices = new HashSet<int>();
			BeyondBuffer = new IndexBuffer();

			ConnectorTable = new ConnectorList[ Constants.ConnectorTableSize ];
			for( var i = 0; i < Constants.ConnectorTableSize; i++ ) ConnectorTable[ i ] = new ConnectorList();

			VertexVisited = new bool[ NumberOfVertices ];
			Positions = new double[ NumberOfVertices * NumOfDimensions ];
			boundingBoxPoints = new List<int>[ NumOfDimensions ];
			minima = new double[ NumOfDimensions ];
			maxima = new double[ NumOfDimensions ];
			mathHelper = new MathHelper( NumOfDimensions, Positions );
		}

		/// <summary>
		/// Check the dimensionality of the input data.
		/// </summary>
		/// <returns>System.Int32.</returns>
		/// <exception cref="ArgumentException">Invalid input data (non-uniform dimension).</exception>
		private int DetermineDimension()
		{
			var r = new System.Random();
			var dimensions = new List<int>();
			for( var i = 0; i < 10; i++ )
				dimensions.Add( Vertices[ r.Next( NumberOfVertices ) ].Position.Length );
			var dimension = dimensions.Min();
			if( dimension != dimensions.Max() )
				throw new ArgumentException( "Invalid input data (non-uniform dimension)." );
			return dimension;
		}


		/// <summary>
		/// Gets/calculates the convex hull. This is 
		/// </summary>
		private void GetConvexHull()
		{
			// accessing a 1D array is quicker than a jagged array, so the first step is to make this array
			SerializeVerticesToPositions();
			// next the bounding box extremes are found. This is used to shift, scale and find the starting simplex.
			FindBoundingBoxPoints();
			// the positions are shifted to avoid divide by zero problems
			// and if Delaunay or Voronoi, then the parabola terms are scaled back to match the size of the other coords
			ShiftAndScalePositions();
			// Find the (dimension+1) initial points and create the simplexes.
			CreateInitialSimplex();

			// Now, the main loop. These initial faces of a simplex are replaced and expanded 
			// outwards to make the convex hull and faces.
			while( UnprocessedFaces.First != null )
			{
				var currentFace = UnprocessedFaces.First;
				CurrentVertex = currentFace.FurthestVertex;

				UpdateCenter();

				// The affected faces get tagged
				TagAffectedFaces( currentFace );

				// Create the cone from the currentVertex and the affected faces horizon.
				if( !SingularVertices.Contains( CurrentVertex ) && CreateCone() ) CommitCone();
				else HandleSingular();

				// Need to reset the tags
				var count = AffectedFaceBuffer.Count;
				for( var i = 0; i < count; i++ ) AffectedFaceFlags[ AffectedFaceBuffer[ i ] ] = false;
			}
		}

		#endregion

		/// <summary>
		/// Serializes the vertices into the 1D array, Positions. The 1D array has much quicker access in C#.
		/// </summary>
		private void SerializeVerticesToPositions()
		{
			var index = 0;
			if( IsLifted ) // "Lifted" means that the last dimension is the sum of the squares of the others.
			{
				foreach( var v in Vertices )
				{
					var parabolaTerm = 0.0; // the lifted term is a sum of squares.
					var origNumDim = NumOfDimensions - 1;
					for( var i = 0; i < origNumDim; i++ )
					{
						var coordinate = v.Position[ i ];
						Positions[ index++ ] = coordinate;
						parabolaTerm += coordinate * coordinate;
					}
					Positions[ index++ ] = parabolaTerm;
				}
			}
			else
				foreach( var v in Vertices )
				{
					for( var i = 0; i < NumOfDimensions; i++ )
						Positions[ index++ ] = v.Position[ i ];
				}
		}

		/// <summary>
		/// Finds the bounding box points.
		/// </summary>
		private void FindBoundingBoxPoints()
		{
			indexOfDimensionWithLeastExtremes = -1;
			var minNumExtremes = int.MaxValue;
			for( var i = 0; i < NumOfDimensions; i++ )
			{
				var minIndices = new List<int>();
				var maxIndices = new List<int>();
				double min = double.PositiveInfinity, max = double.NegativeInfinity;
				for( var j = 0; j < NumberOfVertices; j++ )
				{
					var v = GetCoordinate( j, i );
					var difference = min - v;
					if( difference >= PlaneDistanceTolerance )
					{
						// you found a better solution than before, clear out the list and store new value
						min = v;
						minIndices.Clear();
						minIndices.Add( j );
					}
					else if( difference > 0 )
					{
						// you found a solution slightly better than before, clear out those that are no longer on the list and store new value
						min = v;
						minIndices.RemoveAll( index => min - GetCoordinate( index, i ) > PlaneDistanceTolerance );
						minIndices.Add( j );
					}
					else if( difference > -PlaneDistanceTolerance )
					{
						//same or almost as good as current limit, so store it
						minIndices.Add( j );
					}
					difference = v - max;
					if( difference >= PlaneDistanceTolerance )
					{
						// you found a better solution than before, clear out the list and store new value
						max = v;
						maxIndices.Clear();
						maxIndices.Add( j );
					}
					else if( difference > 0 )
					{
						// you found a solution slightly better than before, clear out those that are no longer on the list and store new value
						max = v;
						maxIndices.RemoveAll( index => min - GetCoordinate( index, i ) > PlaneDistanceTolerance );
						maxIndices.Add( j );
					}
					else if( difference > -PlaneDistanceTolerance )
					{
						//same or almost as good as current limit, so store it
						maxIndices.Add( j );
					}
				}
				minima[ i ] = min;
				maxima[ i ] = max;
				minIndices.AddRange( maxIndices );
				if( minIndices.Count < minNumExtremes )
				{
					minNumExtremes = minIndices.Count;
					indexOfDimensionWithLeastExtremes = i;
				}
				boundingBoxPoints[ i ] = minIndices;
			}
		}

		/// <summary>
		/// Shifts and scales the Positions to avoid future errors. This does not alter the original data.
		/// </summary>
		private void ShiftAndScalePositions()
		{
			var positionsLength = Positions.Length;
			if( IsLifted )
			{
				var origNumDim = NumOfDimensions - 1;
				var parabolaScale = 2 / ( minima.Sum( x => Math.Abs( x ) ) + maxima.Sum( x => Math.Abs( x ) )
					- Math.Abs( maxima[ origNumDim ] ) - Math.Abs( minima[ origNumDim ] ) );
				// the parabola scale is 1 / average of the sum of the other dimensions.
				// multiplying this by the parabola will scale it back to be on near similar size to the
				// other dimensions. Without this, the term is much larger than the others, which causes
				// problems for roundoff error and finding the normal of faces.
				minima[ origNumDim ] *= parabolaScale; // change the extreme values as well
				maxima[ origNumDim ] *= parabolaScale;
				// it is done here because
				for( int i = origNumDim; i < positionsLength; i += NumOfDimensions )
					Positions[ i ] *= parabolaScale;
			}
			var shiftAmount = new double[ NumOfDimensions ];
			for( int i = 0; i < NumOfDimensions; i++ )
				// now the entire model is shifted to all positive numbers...plus some more.
				// why? 
				// 1) to avoid dealing with a point at the origin {0,0,...,0} which causes problems 
				//    for future normal finding
				// 2) note that weird shift that is used (max - min - min). This is to avoid scaling
				//    issues. this shift means that the minima in a dimension will always be a positive
				//    number (no points at zero), and the minima [in a given dimension] will always be
				//    half of the maxima. 'Half' is much preferred to 'thousands of times'
				//    Think of the first term as the range (max - min), then the second term avoids cases
				//    where there are both positive and negative numbers.
				if( maxima[ i ] == minima[ i ] ) shiftAmount[ i ] = 0.0;
				else shiftAmount[ i ] = ( maxima[ i ] - minima[ i ] ) - minima[ i ];
			for( int i = 0; i < positionsLength; i++ )
				Positions[ i ] += shiftAmount[ i % NumOfDimensions ];
		}

		/// <summary>
		/// Find the (dimension+1) initial points and create the simplexes.
		/// Creates the initial simplex of n+1 vertices by using points from the bounding box.
		/// Special care is taken to ensure that the vertices chosen do not result in a degenerate shape
		/// where vertices are collinear (co-planar, etc). This would technically be resolved when additional
		/// vertices are checked in the main loop, but: 1) a degenerate simplex would not eliminate any other
		/// vertices (thus no savings there), 2) the creation of the face normal is prone to error.
		/// </summary>
		private void CreateInitialSimplex()
		{
			var initialPoints = FindInitialPoints();
			#region Create the first faces from (dimension + 1) vertices.

			var faces = new int[ NumOfDimensions + 1 ];

			for( var i = 0; i < NumOfDimensions + 1; i++ )
			{
				var vertices = new int[ NumOfDimensions ];
				for( int j = 0, k = 0; j <= NumOfDimensions; j++ )
				{
					if( i != j ) vertices[ k++ ] = initialPoints[ j ];
				}
				var newFace = FacePool[ ObjectManager.GetFace() ];
				newFace.Vertices = vertices;
				Array.Sort( vertices );
				mathHelper.CalculateFacePlane( newFace, Center );
				faces[ i ] = newFace.Index;
			}
			// update the adjacency (check all pairs of faces)
			for( var i = 0; i < NumOfDimensions; i++ )
				for( var j = i + 1; j < NumOfDimensions + 1; j++ ) UpdateAdjacency( FacePool[ faces[ i ] ], FacePool[ faces[ j ] ] );

			#endregion

			#region Init the vertex beyond buffers.

			foreach( var faceIndex in faces )
			{
				var face = FacePool[ faceIndex ];
				FindBeyondVertices( face );
				if( face.VerticesBeyond.Count == 0 ) ConvexFaces.Add( face.Index ); // The face is on the hull
				else UnprocessedFaces.Add( face );
			}

			#endregion

			// Set all vertices to false (unvisited).
			foreach( var vertex in initialPoints ) VertexVisited[ vertex ] = false;
		}

		/// <summary>
		/// Finds (dimension + 1) initial points.
		/// </summary>
		/// <returns>List&lt;System.Int32&gt;.</returns>
		/// <exception cref="System.ArgumentException">The input data is degenerate. It appears to exist in " + NumOfDimensions +
		///                     " dimensions, but it is a " + (NumOfDimensions - 1) + " dimensional set (i.e. the point of collinear,"
		///                     + " coplanar, or co-hyperplanar.)</exception>
		private List<int> FindInitialPoints()
		{
			var bigNumber = maxima.Sum() * NumOfDimensions * NumberOfVertices;
			// the first two points are taken from the dimension that had the fewest extremes
			// well, in most cases there will only be 2 in all dimensions: one min and one max
			// but a lot of engineering part shapes are nice and square and can have hundreds of 
			// parallel vertices at the extremes
			var vertex1 = boundingBoxPoints[ indexOfDimensionWithLeastExtremes ].First(); // these are min and max vertices along
			var vertex2 = boundingBoxPoints[ indexOfDimensionWithLeastExtremes ].Last(); // the dimension that had the fewest points
			boundingBoxPoints[ indexOfDimensionWithLeastExtremes ].RemoveAt( 0 );
			boundingBoxPoints[ indexOfDimensionWithLeastExtremes ].RemoveAt( boundingBoxPoints[ indexOfDimensionWithLeastExtremes ].Count - 1 );
			var initialPoints = new List<int> { vertex1, vertex2 };
			VertexVisited[ vertex1 ] = VertexVisited[ vertex2 ] = true;
			CurrentVertex = vertex1; UpdateCenter();
			CurrentVertex = vertex2; UpdateCenter();
			var edgeVectors = new double[ NumOfDimensions ][];
			edgeVectors[ 0 ] = mathHelper.VectorBetweenVertices( vertex2, vertex1 );
			// now the remaining vertices are just combined in one big list
			var extremes = boundingBoxPoints.SelectMany( x => x ).ToList();
			// otherwise find the remaining points by maximizing the initial simplex volume
			var index = 1;
			while( index < NumOfDimensions && extremes.Any() )
			{
				var bestVertex = -1;
				var bestEdgeVector = new double[] { };
				var maxVolume = Constants.DefaultPlaneDistanceTolerance;
				for( var i = extremes.Count - 1; i >= 0; i-- )
				{
					// count backwards in order to remove potential duplicates
					var vIndex = extremes[ i ];
					if( initialPoints.Contains( vIndex ) ) extremes.RemoveAt( i );
					else
					{
						edgeVectors[ index ] = mathHelper.VectorBetweenVertices( vIndex, vertex1 );
						var volume = mathHelper.GetSimplexVolume( edgeVectors, index, bigNumber );
						if( maxVolume < volume )
						{
							maxVolume = volume;
							bestVertex = vIndex;
							bestEdgeVector = edgeVectors[ index ];
						}
					}
				}
				extremes.Remove( bestVertex );
				if( bestVertex == -1 ) break;
				initialPoints.Add( bestVertex );
				edgeVectors[ index++ ] = bestEdgeVector;
				CurrentVertex = bestVertex; UpdateCenter();
			}
			// hmm, there are not enough points on the bounding box to make a simplex. It is rare but entirely possibly.
			// As an extreme, the bounding box can be made in n dimensions from only 2 unique points. When we can't find
			// enough unique points, we start again with ALL the vertices. The following is a near replica of the code 
			// above, but instead of extremes, we consider "allVertices".
			if( initialPoints.Count <= NumOfDimensions && !IsLifted )
			{
				var allVertices = Enumerable.Range( 0, NumberOfVertices ).ToList();
				while( index < NumOfDimensions && allVertices.Any() )
				{
					var bestVertex = -1;
					var bestEdgeVector = new double[] { };
					var maxVolume = 0.0;
					for( var i = allVertices.Count - 1; i >= 0; i-- )
					{
						// count backwards in order to remove potential duplicates
						var vIndex = allVertices[ i ];
						if( initialPoints.Contains( vIndex ) ) allVertices.RemoveAt( i );
						else
						{
							edgeVectors[ index ] = mathHelper.VectorBetweenVertices( vIndex, vertex1 );
							var volume = mathHelper.GetSimplexVolume( edgeVectors, index, bigNumber );
							if( maxVolume < volume )
							{
								maxVolume = volume;
								bestVertex = vIndex;
								bestEdgeVector = edgeVectors[ index ];
							}
						}
					}
					allVertices.Remove( bestVertex );
					if( bestVertex == -1 ) break;
					initialPoints.Add( bestVertex );
					edgeVectors[ index++ ] = bestEdgeVector;
					CurrentVertex = bestVertex; UpdateCenter();
				}
			}
			if( initialPoints.Count <= NumOfDimensions && IsLifted )
			{
				var allVertices = Enumerable.Range( 0, NumberOfVertices ).ToList();
				while( index < NumOfDimensions && allVertices.Any() )
				{
					var bestVertex = -1;
					var bestEdgeVector = new double[] { };
					var maxVolume = 0.0;
					for( var i = allVertices.Count - 1; i >= 0; i-- )
					{
						// count backwards in order to remove potential duplicates
						var vIndex = allVertices[ i ];
						if( initialPoints.Contains( vIndex ) ) allVertices.RemoveAt( i );
						else
						{
							mathHelper.RandomOffsetToLift( vIndex, maxima.Last() - minima.Last() );
							edgeVectors[ index ] = mathHelper.VectorBetweenVertices( vIndex, vertex1 );
							var volume = mathHelper.GetSimplexVolume( edgeVectors, index, bigNumber );
							if( maxVolume < volume )
							{
								maxVolume = volume;
								bestVertex = vIndex;
								bestEdgeVector = edgeVectors[ index ];
							}
						}
					}
					allVertices.Remove( bestVertex );
					if( bestVertex == -1 ) break;
					initialPoints.Add( bestVertex );
					edgeVectors[ index++ ] = bestEdgeVector;
					CurrentVertex = bestVertex; UpdateCenter();
				}
			}
			if( initialPoints.Count <= NumOfDimensions && IsLifted )
				throw new ArgumentException( "The input data is degenerate. It appears to exist in " + NumOfDimensions +
					" dimensions, but it is a " + ( NumOfDimensions - 1 ) + " dimensional set (i.e. the point of collinear,"
					+ " coplanar, or co-hyperplanar.)" );
			return initialPoints;
		}

		/// <summary>
		/// Check if 2 faces are adjacent and if so, update their AdjacentFaces array.
		/// </summary>
		/// <param name="l">The l.</param>
		/// <param name="r">The r.</param>
		private void UpdateAdjacency( ConvexFaceInternal l, ConvexFaceInternal r )
		{
			var lv = l.Vertices;
			var rv = r.Vertices;
			int i;

			// reset marks on the 1st face
			for( i = 0; i < lv.Length; i++ ) VertexVisited[ lv[ i ] ] = false;

			// mark all vertices on the 2nd face
			for( i = 0; i < rv.Length; i++ ) VertexVisited[ rv[ i ] ] = true;

			// find the 1st false index
			for( i = 0; i < lv.Length; i++ ) if( !VertexVisited[ lv[ i ] ] ) break;

			// no vertex was marked
			if( i == NumOfDimensions ) return;

			// check if only 1 vertex wasn't marked
			for( var j = i + 1; j < lv.Length; j++ ) if( !VertexVisited[ lv[ j ] ] ) return;

			// if we are here, the two faces share an edge
			l.AdjacentFaces[ i ] = r.Index;

			// update the adj. face on the other face - find the vertex that remains marked
			for( i = 0; i < lv.Length; i++ ) VertexVisited[ lv[ i ] ] = false;
			for( i = 0; i < rv.Length; i++ )
			{
				if( VertexVisited[ rv[ i ] ] ) break;
			}
			r.AdjacentFaces[ i ] = l.Index;
		}

		/// <summary>
		/// Used in the "initialization" code.
		/// </summary>
		/// <param name="face">The face.</param>
		private void FindBeyondVertices( ConvexFaceInternal face )
		{
			var beyondVertices = face.VerticesBeyond;
			MaxDistance = double.NegativeInfinity;
			FurthestVertex = 0;
			for( var i = 0; i < NumberOfVertices; i++ )
			{
				if( VertexVisited[ i ] ) continue;
				IsBeyond( face, beyondVertices, i );
			}

			face.FurthestVertex = FurthestVertex;
		}

		/// <summary>
		/// Tags all faces seen from the current vertex with 1.
		/// </summary>
		/// <param name="currentFace">The current face.</param>
		private void TagAffectedFaces( ConvexFaceInternal currentFace )
		{
			AffectedFaceBuffer.Clear();
			AffectedFaceBuffer.Add( currentFace.Index );
			TraverseAffectedFaces( currentFace.Index );
		}

		/// <summary>
		/// Recursively traverse all the relevant faces.
		/// </summary>
		/// <param name="currentFace">The current face.</param>
		private void TraverseAffectedFaces( int currentFace )
		{
			TraverseStack.Clear();
			TraverseStack.Push( currentFace );
			AffectedFaceFlags[ currentFace ] = true;

			while( TraverseStack.Count > 0 )
			{
				var top = FacePool[ TraverseStack.Pop() ];
				for( var i = 0; i < NumOfDimensions; i++ )
				{
					var adjFace = top.AdjacentFaces[ i ];

					if( !AffectedFaceFlags[ adjFace ] &&
						mathHelper.GetVertexDistance( CurrentVertex, FacePool[ adjFace ] ) >= PlaneDistanceTolerance )
					{
						AffectedFaceBuffer.Add( adjFace );
						AffectedFaceFlags[ adjFace ] = true;
						TraverseStack.Push( adjFace );
					}
				}
			}
		}

		/// <summary>
		/// Creates a new deferred face.
		/// </summary>
		/// <param name="face">The face.</param>
		/// <param name="faceIndex">Index of the face.</param>
		/// <param name="pivot">The pivot.</param>
		/// <param name="pivotIndex">Index of the pivot.</param>
		/// <param name="oldFace">The old face.</param>
		/// <returns>DeferredFace.</returns>
		private DeferredFace MakeDeferredFace( ConvexFaceInternal face, int faceIndex, ConvexFaceInternal pivot,
			int pivotIndex, ConvexFaceInternal oldFace )
		{
			var ret = ObjectManager.GetDeferredFace();

			ret.Face = face;
			ret.FaceIndex = faceIndex;
			ret.Pivot = pivot;
			ret.PivotIndex = pivotIndex;
			ret.OldFace = oldFace;

			return ret;
		}

		/// <summary>
		/// Connect faces using a connector.
		/// </summary>
		/// <param name="connector">The connector.</param>
		private void ConnectFace( FaceConnector connector )
		{
			var index = connector.HashCode % Constants.ConnectorTableSize;
			var list = ConnectorTable[ index ];

			for( var current = list.First; current != null; current = current.Next )
			{
				if( FaceConnector.AreConnectable( connector, current, NumOfDimensions ) )
				{
					list.Remove( current );
					FaceConnector.Connect( current, connector );
					current.Face = null;
					connector.Face = null;
					ObjectManager.DepositConnector( current );
					ObjectManager.DepositConnector( connector );
					return;
				}
			}

			list.Add( connector );
		}

		/// <summary>
		/// Removes the faces "covered" by the current vertex and adds the newly created ones.
		/// </summary>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool CreateCone()
		{
			var currentVertexIndex = CurrentVertex;
			ConeFaceBuffer.Clear();

			for( var fIndex = 0; fIndex < AffectedFaceBuffer.Count; fIndex++ )
			{
				var oldFaceIndex = AffectedFaceBuffer[ fIndex ];
				var oldFace = FacePool[ oldFaceIndex ];

				// Find the faces that need to be updated
				var updateCount = 0;
				for( var i = 0; i < NumOfDimensions; i++ )
				{
					var af = oldFace.AdjacentFaces[ i ];
					if( !AffectedFaceFlags[ af ] ) // Tag == false when oldFaces does not contain af
					{
						UpdateBuffer[ updateCount ] = af;
						UpdateIndices[ updateCount ] = i;
						++updateCount;
					}
				}

				for( var i = 0; i < updateCount; i++ )
				{
					var adjacentFace = FacePool[ UpdateBuffer[ i ] ];

					var oldFaceAdjacentIndex = 0;
					var adjFaceAdjacency = adjacentFace.AdjacentFaces;
					for( var j = 0; j < adjFaceAdjacency.Length; j++ )
					{
						if( oldFaceIndex == adjFaceAdjacency[ j ] )
						{
							oldFaceAdjacentIndex = j;
							break;
						}
					}

					var forbidden = UpdateIndices[ i ]; // Index of the face that corresponds to this adjacent face

					int oldVertexIndex;
					int[] vertices;

					var newFaceIndex = ObjectManager.GetFace();
					var newFace = FacePool[ newFaceIndex ];
					vertices = newFace.Vertices;
					for( var j = 0; j < NumOfDimensions; j++ ) vertices[ j ] = oldFace.Vertices[ j ];
					oldVertexIndex = vertices[ forbidden ];

					int orderedPivotIndex;

					// correct the ordering
					if( currentVertexIndex < oldVertexIndex )
					{
						orderedPivotIndex = 0;
						for( var j = forbidden - 1; j >= 0; j-- )
						{
							if( vertices[ j ] > currentVertexIndex ) vertices[ j + 1 ] = vertices[ j ];
							else
							{
								orderedPivotIndex = j + 1;
								break;
							}
						}
					}
					else
					{
						orderedPivotIndex = NumOfDimensions - 1;
						for( var j = forbidden + 1; j < NumOfDimensions; j++ )
						{
							if( vertices[ j ] < currentVertexIndex ) vertices[ j - 1 ] = vertices[ j ];
							else
							{
								orderedPivotIndex = j - 1;
								break;
							}
						}
					}

					vertices[ orderedPivotIndex ] = CurrentVertex;

					if( !mathHelper.CalculateFacePlane( newFace, Center ) )
					{
						return false;
					}

					ConeFaceBuffer.Add( MakeDeferredFace( newFace, orderedPivotIndex, adjacentFace, oldFaceAdjacentIndex,
						oldFace ) );
				}
			}

			return true;
		}

		/// <summary>
		/// Commits a cone and adds a vertex to the convex hull.
		/// </summary>
		private void CommitCone()
		{
			// Fill the adjacency.
			for( var i = 0; i < ConeFaceBuffer.Count; i++ )
			{
				var face = ConeFaceBuffer[ i ];

				var newFace = face.Face;
				var adjacentFace = face.Pivot;
				var oldFace = face.OldFace;
				var orderedPivotIndex = face.FaceIndex;

				newFace.AdjacentFaces[ orderedPivotIndex ] = adjacentFace.Index;
				adjacentFace.AdjacentFaces[ face.PivotIndex ] = newFace.Index;

				// let there be a connection.
				for( var j = 0; j < NumOfDimensions; j++ )
				{
					if( j == orderedPivotIndex ) continue;
					var connector = ObjectManager.GetConnector();
					connector.Update( newFace, j, NumOfDimensions );
					ConnectFace( connector );
				}

				// the id adjacent face on the hull? If so, we can use simple method to find beyond vertices.
				if( adjacentFace.VerticesBeyond.Count == 0 )
					FindBeyondVertices( newFace, oldFace.VerticesBeyond );
				// it is slightly more effective if the face with the lower number of beyond vertices comes first.
				else if( adjacentFace.VerticesBeyond.Count < oldFace.VerticesBeyond.Count )
					FindBeyondVertices( newFace, adjacentFace.VerticesBeyond, oldFace.VerticesBeyond );
				else
					FindBeyondVertices( newFace, oldFace.VerticesBeyond, adjacentFace.VerticesBeyond );

				// This face will definitely lie on the hull
				if( newFace.VerticesBeyond.Count == 0 )
				{
					ConvexFaces.Add( newFace.Index );
					UnprocessedFaces.Remove( newFace );
					ObjectManager.DepositVertexBuffer( newFace.VerticesBeyond );
					newFace.VerticesBeyond = EmptyBuffer;
				}
				else // Add the face to the list
				{
					UnprocessedFaces.Add( newFace );
				}

				// recycle the object.
				ObjectManager.DepositDeferredFace( face );
			}

			// Recycle the affected faces.
			for( var fIndex = 0; fIndex < AffectedFaceBuffer.Count; fIndex++ )
			{
				var face = AffectedFaceBuffer[ fIndex ];
				UnprocessedFaces.Remove( FacePool[ face ] );
				ObjectManager.DepositFace( face );
			}
		}

		/// <summary>
		/// Check whether the vertex v is beyond the given face. If so, add it to beyondVertices.
		/// </summary>
		/// <param name="face">The face.</param>
		/// <param name="beyondVertices">The beyond vertices.</param>
		/// <param name="v">The v.</param>
		private void IsBeyond( ConvexFaceInternal face, IndexBuffer beyondVertices, int v )
		{
			var distance = mathHelper.GetVertexDistance( v, face );
			if( distance >= PlaneDistanceTolerance )
			{
				if( distance > MaxDistance )
				{
					// If it's within the tolerance distance, use the lex. larger point
					if( distance - MaxDistance < PlaneDistanceTolerance )
					{ // todo: why is this LexCompare necessary. Would seem to favor x over y over z (etc.)?
						if( LexCompare( v, FurthestVertex ) > 0 )
						{
							MaxDistance = distance;
							FurthestVertex = v;
						}
					}
					else
					{
						MaxDistance = distance;
						FurthestVertex = v;
					}
				}
				beyondVertices.Add( v );
			}
		}


		/// <summary>
		/// Compares the values of two vertices. The return value (-1, 0 or +1) are found
		/// by first checking the first coordinate and then progressing through the rest.
		/// In this way {2, 8} will be a "-1" (less than) {3, 1}.
		/// </summary>
		/// <param name="u">The base vertex index, u.</param>
		/// <param name="v">The compared vertex index, v.</param>
		/// <returns>System.Int32.</returns>
		private int LexCompare( int u, int v )
		{
			int uOffset = u * NumOfDimensions, vOffset = v * NumOfDimensions;
			for( var i = 0; i < NumOfDimensions; i++ )
			{
				double x = Positions[ uOffset + i ], y = Positions[ vOffset + i ];
				var comp = x.CompareTo( y );
				if( comp != 0 ) return comp;
			}
			return 0;
		}


		/// <summary>
		/// Used by update faces.
		/// </summary>
		/// <param name="face">The face.</param>
		/// <param name="beyond">The beyond.</param>
		/// <param name="beyond1">The beyond1.</param>
		private void FindBeyondVertices( ConvexFaceInternal face, IndexBuffer beyond, IndexBuffer beyond1 )
		{
			var beyondVertices = BeyondBuffer;

			MaxDistance = double.NegativeInfinity;
			FurthestVertex = 0;
			int v;

			for( var i = 0; i < beyond1.Count; i++ ) VertexVisited[ beyond1[ i ] ] = true;
			VertexVisited[ CurrentVertex ] = false;
			for( var i = 0; i < beyond.Count; i++ )
			{
				v = beyond[ i ];
				if( v == CurrentVertex ) continue;
				VertexVisited[ v ] = false;
				IsBeyond( face, beyondVertices, v );
			}

			for( var i = 0; i < beyond1.Count; i++ )
			{
				v = beyond1[ i ];
				if( VertexVisited[ v ] ) IsBeyond( face, beyondVertices, v );
			}

			face.FurthestVertex = FurthestVertex;

			// Pull the old switch a roo (switch the face beyond buffers)
			var temp = face.VerticesBeyond;
			face.VerticesBeyond = beyondVertices;
			if( temp.Count > 0 ) temp.Clear();
			BeyondBuffer = temp;
		}

		/// <summary>
		/// Finds the beyond vertices.
		/// </summary>
		/// <param name="face">The face.</param>
		/// <param name="beyond">The beyond.</param>
		private void FindBeyondVertices( ConvexFaceInternal face, IndexBuffer beyond )
		{
			var beyondVertices = BeyondBuffer;

			MaxDistance = double.NegativeInfinity;
			FurthestVertex = 0;
			int v;

			for( var i = 0; i < beyond.Count; i++ )
			{
				v = beyond[ i ];
				if( v == CurrentVertex ) continue;
				IsBeyond( face, beyondVertices, v );
			}

			face.FurthestVertex = FurthestVertex;

			// Pull the old switch a roo (switch the face beyond buffers)
			var temp = face.VerticesBeyond;
			face.VerticesBeyond = beyondVertices;
			if( temp.Count > 0 ) temp.Clear();
			BeyondBuffer = temp;
		}

		/// <summary>
		/// Recalculates the centroid of the current hull.
		/// </summary>
		private void UpdateCenter()
		{
			for( var i = 0; i < NumOfDimensions; i++ ) Center[ i ] *= ConvexHullSize;
			ConvexHullSize += 1;
			var f = 1.0 / ConvexHullSize;
			var co = CurrentVertex * NumOfDimensions;
			for( var i = 0; i < NumOfDimensions; i++ ) Center[ i ] = f * ( Center[ i ] + Positions[ co + i ] );
		}

		/// <summary>
		/// Removes the last vertex from the center.
		/// </summary>
		private void RollbackCenter()
		{
			for( var i = 0; i < NumOfDimensions; i++ ) Center[ i ] *= ConvexHullSize;
			ConvexHullSize -= 1;
			var f = ConvexHullSize > 0 ? 1.0 / ConvexHullSize : 0.0;
			var co = CurrentVertex * NumOfDimensions;
			for( var i = 0; i < NumOfDimensions; i++ ) Center[ i ] = f * ( Center[ i ] - Positions[ co + i ] );
		}

		/// <summary>
		/// Handles singular vertex.
		/// </summary>
		private void HandleSingular()
		{
			RollbackCenter();
			SingularVertices.Add( CurrentVertex );

			// This means that all the affected faces must be on the hull and that all their "vertices beyond" are singular.
			for( var fIndex = 0; fIndex < AffectedFaceBuffer.Count; fIndex++ )
			{
				var face = FacePool[ AffectedFaceBuffer[ fIndex ] ];
				var vb = face.VerticesBeyond;
				for( var i = 0; i < vb.Count; i++ )
				{
					SingularVertices.Add( vb[ i ] );
				}

				ConvexFaces.Add( face.Index );
				UnprocessedFaces.Remove( face );
				ObjectManager.DepositVertexBuffer( face.VerticesBeyond );
				face.VerticesBeyond = EmptyBuffer;
			}
		}

		/// <summary>
		/// Get a vertex coordinate. In order to reduce speed, all vertex coordinates
		/// have been placed in a single array.
		/// </summary>
		/// <param name="vIndex">The vertex index.</param>
		/// <param name="dimension">The index of the dimension.</param>
		/// <returns>System.Double.</returns>
		private double GetCoordinate( int vIndex, int dimension )
		{
			return Positions[ vIndex * NumOfDimensions + dimension ];
		}

		#region Returning the Results in the proper format

		/// <summary>
		/// Gets the hull vertices.
		/// </summary>
		/// <typeparam name="TVertex">The type of the t vertex.</typeparam>
		/// <param name="data">The data.</param>
		/// <returns>TVertex[].</returns>
		private TVertex[] GetHullVertices<TVertex>( IList<TVertex> data )
		{
			var cellCount = ConvexFaces.Count;
			var hullVertexCount = 0;

			for( var i = 0; i < NumberOfVertices; i++ ) VertexVisited[ i ] = false;

			for( var i = 0; i < cellCount; i++ )
			{
				var vs = FacePool[ ConvexFaces[ i ] ].Vertices;
				for( var j = 0; j < vs.Length; j++ )
				{
					var v = vs[ j ];
					if( !VertexVisited[ v ] )
					{
						VertexVisited[ v ] = true;
						hullVertexCount++;
					}
				}
			}

			var result = new TVertex[ hullVertexCount ];
			for( var i = 0; i < NumberOfVertices; i++ )
			{
				if( VertexVisited[ i ] ) result[ --hullVertexCount ] = data[ i ];
			}

			return result;
		}

		/// <summary>
		/// Finds the convex hull and creates the TFace objects.
		/// </summary>
		/// <typeparam name="TFace">The type of the t face.</typeparam>
		/// <typeparam name="TVertex">The type of the t vertex.</typeparam>
		/// <returns>TFace[].</returns>
		private TFace[] GetConvexFaces<TVertex, TFace>()
			where TFace : ConvexFace<TVertex, TFace>, new()
			where TVertex : IVertex
		{
			var faces = ConvexFaces;
			var cellCount = faces.Count;
			var cells = new TFace[ cellCount ];

			for( var i = 0; i < cellCount; i++ )
			{
				var face = FacePool[ faces[ i ] ];
				var vertices = new TVertex[ NumOfDimensions ];
				for( var j = 0; j < NumOfDimensions; j++ )
				{
					vertices[ j ] = (TVertex)Vertices[ face.Vertices[ j ] ];
				}

				cells[ i ] = new TFace
				{
					Vertices = vertices,
					Adjacency = new TFace[ NumOfDimensions ],
					Normal = IsLifted ? null : face.Normal
				};
				face.Tag = i;
			}

			for( var i = 0; i < cellCount; i++ )
			{
				var face = FacePool[ faces[ i ] ];
				var cell = cells[ i ];
				for( var j = 0; j < NumOfDimensions; j++ )
				{
					if( face.AdjacentFaces[ j ] < 0 ) continue;
					cell.Adjacency[ j ] = cells[ FacePool[ face.AdjacentFaces[ j ] ].Tag ];
				}

				// Fix the vertex orientation.
				if( face.IsNormalFlipped )
				{
					var tempVert = cell.Vertices[ 0 ];
					cell.Vertices[ 0 ] = cell.Vertices[ NumOfDimensions - 1 ];
					cell.Vertices[ NumOfDimensions - 1 ] = tempVert;

					var tempAdj = cell.Adjacency[ 0 ];
					cell.Adjacency[ 0 ] = cell.Adjacency[ NumOfDimensions - 1 ];
					cell.Adjacency[ NumOfDimensions - 1 ] = tempAdj;
				}
			}

			return cells;
		}

		/// <summary>
		/// For 2D only: Returns the result in counter-clockwise order starting with the element with the lowest X value.
		/// If there are multiple vertices with the same minimum X, then the one with the lowest Y is chosen.
		/// </summary>
		/// <typeparam name="TVertex">The type of the vertex.</typeparam>
		/// <typeparam name="TFace">The type of the face.</typeparam>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		private ConvexHull<TVertex, TFace> Return2DResultInOrder<TVertex, TFace>( IList<TVertex> data )
			where TVertex : IVertex
			where TFace : ConvexFace<TVertex, TFace>, new()
		{
			TFace[] faces = GetConvexFaces<TVertex, TFace>();
			var numPoints = faces.Length;
			var orderDictionary = new Dictionary<TVertex, TFace>();
			foreach( var face in faces )
				orderDictionary.Add( face.Vertices[ 1 ], face );
			var firstPoint = faces[ 0 ].Vertices[ 1 ];
			var nextPoint = faces[ 0 ].Vertices[ 0 ];
			var orderedPointList = new List<TVertex>();
			orderedPointList.Add( firstPoint );
			var orderedFaceList = new List<TFace>();
			orderedFaceList.Add( faces[ 1 ] );
			var lowestXMinIndex = 0;
			var k = 0;
			while( !nextPoint.Equals( firstPoint ) )
			{
				orderedPointList.Add( nextPoint );
				var nextFace = orderDictionary[ nextPoint ];
				orderedFaceList.Add( nextFace );
				if( nextPoint.Position[ 0 ] < orderedPointList[ lowestXMinIndex ].Position[ 0 ]
					|| ( nextPoint.Position[ 0 ] == orderedPointList[ lowestXMinIndex ].Position[ 0 ]
						&& nextPoint.Position[ 1 ] <= orderedPointList[ lowestXMinIndex ].Position[ 1 ] ) )
					lowestXMinIndex = k;
				k++;
				nextPoint = nextFace.Vertices[ 0 ];
			}
			TVertex[] points = new TVertex[ numPoints ];
			for( int i = 0; i < numPoints; i++ )
			{
				var j = ( i + lowestXMinIndex ) % numPoints;
				points[ i ] = orderedPointList[ j ];
				faces[ i ] = orderedFaceList[ j ];
			}
			return new ConvexHull<TVertex, TFace>
			{
				Points = points,
				Faces = faces
			};
		}

		#endregion

		#region Fields

		/// <summary>
		/// Corresponds to the dimension of the data.
		/// When the "lifted" hull is computed, Dimension is automatically incremented by one.
		/// </summary>
		internal readonly int NumOfDimensions;

		/// <summary>
		/// Are we on a paraboloid?
		/// </summary>
		private readonly bool IsLifted;

		/// <summary>
		/// Explained in ConvexHullComputationConfig.
		/// </summary>
		private readonly double PlaneDistanceTolerance;

		/*
         * Representation of the input vertices.
         * 
         * - In the algorithm, a vertex is represented by its index in the Vertices array.
         *   This makes the algorithm a lot faster (up to 30%) than using object reference everywhere.
         * - Positions are stored as a single array of values. Coordinates for vertex with index i
         *   are stored at indices <i * Dimension, (i + 1) * Dimension)
         * - VertexMarks are used by the algorithm to help identify a set of vertices that is "above" (or "beyond") 
         *   a specific face.
         */
		/// <summary>
		/// The vertices
		/// </summary>
		private readonly IVertex[] Vertices;
		/// <summary>
		/// The positions
		/// </summary>
		private double[] Positions;
		/// <summary>
		/// The vertex marks
		/// </summary>
		private readonly bool[] VertexVisited;

		private readonly int NumberOfVertices;

		/*
         * The triangulation faces are represented in a single pool for objects that are being reused.
         * This allows for represent the faces as integers and significantly speeds up many computations.
         * - AffectedFaceFlags are used to mark affected faces/
         */
		/// <summary>
		/// The face pool
		/// </summary>
		internal ConvexFaceInternal[] FacePool;
		/// <summary>
		/// The affected face flags
		/// </summary>
		internal bool[] AffectedFaceFlags;

		/// <summary>
		/// Used to track the size of the current hull in the Update/RollbackCenter functions.
		/// </summary>
		private int ConvexHullSize;

		/// <summary>
		/// A list of faces that that are not a part of the final convex hull and still need to be processed.
		/// </summary>
		private readonly FaceList UnprocessedFaces;

		/// <summary>
		/// A list of faces that form the convex hull.
		/// </summary>
		private readonly IndexBuffer ConvexFaces;

		/// <summary>
		/// The vertex that is currently being processed.
		/// </summary>
		private int CurrentVertex;

		/// <summary>
		/// A helper variable to determine the furthest vertex for a particular convex face.
		/// </summary>
		private double MaxDistance;

		/// <summary>
		/// A helper variable to help determine the index of the vertex that is furthest from the face that is currently being
		/// processed.
		/// </summary>
		private int FurthestVertex;

		/// <summary>
		/// The centroid of the currently computed hull.
		/// </summary>
		private readonly double[] Center;

		/*
         * Helper arrays to store faces for adjacency update.
         * This is just to prevent unnecessary allocations.
         */
		/// <summary>
		/// The update buffer
		/// </summary>
		private readonly int[] UpdateBuffer;
		/// <summary>
		/// The update indices
		/// </summary>
		private readonly int[] UpdateIndices;

		/// <summary>
		/// Used to determine which faces need to be updated at each step of the algorithm.
		/// </summary>
		private readonly IndexBuffer TraverseStack;

		/// <summary>
		/// Used for VerticesBeyond for faces that are on the convex hull.
		/// </summary>
		private readonly IndexBuffer EmptyBuffer;

		/// <summary>
		/// Used to determine which vertices are "above" (or "beyond") a face
		/// </summary>
		private IndexBuffer BeyondBuffer;

		/// <summary>
		/// Stores faces that are visible from the current vertex.
		/// </summary>
		private readonly IndexBuffer AffectedFaceBuffer;

		/// <summary>
		/// Stores faces that form a "cone" created by adding new vertex.
		/// </summary>
		private readonly SimpleList<DeferredFace> ConeFaceBuffer;

		/// <summary>
		/// Stores a list of "singular" (or "generate", "planar", etc.) vertices that cannot be part of the hull.
		/// </summary>
		private readonly HashSet<int> SingularVertices;

		/// <summary>
		/// The connector table helps to determine the adjacency of convex faces.
		/// Hashing is used instead of pairwise comparison. This significantly speeds up the computations,
		/// especially for higher dimensions.
		/// </summary>
		private readonly ConnectorList[] ConnectorTable;

		/// <summary>
		/// Manages the memory allocations and storage of unused objects.
		/// Saves the garbage collector a lot of work.
		/// </summary>
		private readonly MemManager ObjectManager;

		/// <summary>
		/// Helper class for handling math related stuff.
		/// </summary>
		private readonly MathHelper mathHelper;
		private readonly List<int>[] boundingBoxPoints;
		private int indexOfDimensionWithLeastExtremes;
		private readonly double[] minima;
		private readonly double[] maxima;
		#endregion

		/////////////////////////////////////////

		/// <summary>
		/// Represents a vertex.
		/// </summary>
		/// <seealso cref="MIConvexHull.IVertex" />
		public class ConvexVertex : IVertex
		{
			/// <summary>
			/// Position of the vertex.
			/// </summary>
			/// <value>The position.</value>
			public double[] Position { get; set; }

			public Vector3 ToVec3() => new Vector3( Position[ 0 ], Position[ 1 ], Position[ 2 ] );

			public Vector3F ToVec3F() => new Vector3F( (float)Position[ 0 ], (float)Position[ 1 ], (float)Position[ 2 ] );
		}

		/// <summary>
		/// A default convex face representation.
		/// </summary>
		/// <typeparam name="TVertex">The type of the t vertex.</typeparam>
		/*public */
		class ConvexFace<TVertex> : ConvexFace<TVertex, ConvexFace<TVertex>> where TVertex : IVertex
		{
		}

		///////////////////////////////////////////

		//public static void Calculate( Vector3[] vertices, int[] indices, out Vector3[] resultVertices, double epsilon = Constants.DefaultPlaneDistanceTolerance )
		//{
		//	//!!!!, bool removeEqualVertices = true

		//	var convex = indices != null ? Create( vertices, indices, epsilon ) : Create( vertices, epsilon );

		//	var vlist = new List<Vector3>( convex.Faces.Length * 3 );
		//	foreach( var f in convex.Faces )
		//		for( int v = 0; v < f.Vertices.Length; v++ )
		//			vlist.Add( f.Vertices[ v ].ToVec3() );

		//	resultVertices = vlist.ToArray();

		//	////!!!!planeDistanceTolerance
		//	//if( mergeEqualVertices )
		//	//	MathAlgorithms.MergeEqualVertices( ref resultVertices, planeDistanceTolerance );
		//}

		//public static void Calculate( Vector3[] vertices, out Vector3[] resultVertices, double epsilon = Constants.DefaultPlaneDistanceTolerance )
		//{
		//	Calculate( vertices, null, out resultVertices, epsilon );
		//}

		public static void Calculate( Vector3[] vertices, int[] indices, out Vector3[] resultVertices, out int[] resultIndices, double epsilon = Constants.DefaultPlaneDistanceTolerance )
		{
			var convex = indices != null ? Create( vertices, indices, epsilon ) : Create( vertices, epsilon );

			var vlist = new List<Vector3>( convex.Faces.Length * 3 );
			foreach( var f in convex.Faces )
				for( int v = 0; v < f.Vertices.Length; v++ )
					vlist.Add( f.Vertices[ v ].ToVec3() );

			resultVertices = vlist.ToArray();
			resultIndices = new int[ resultVertices.Length ];
			for( int n = 0; n < resultIndices.Length; n++ )
				resultIndices[ n ] = n;

			MathAlgorithms.MergeEqualVertices( ref resultVertices, ref resultIndices, epsilon, true );
		}

		//public static void Calculate( Vector3[] vertices, out Vector3[] resultVertices, out int[] resultIndices, double epsilon = Constants.DefaultPlaneDistanceTolerance )
		//{
		//	Calculate( vertices, null, out resultVertices, out resultIndices, epsilon );
		//}

		static bool PlanesContains( List<Plane> list, ref Plane plane, double epsilon )
		{
			for( int n = 0; n < list.Count; n++ )
				if( list[ n ].Equals( ref plane, epsilon ) )
					return true;
			return false;
		}

		public static void Calculate( Vector3[] vertices, int[] indices, out Vector3[] resultVertices, out int[] resultIndices, out Plane[] resultPlanes, double epsilon = Constants.DefaultPlaneDistanceTolerance )
		{
			Calculate( vertices, indices, out resultVertices, out resultIndices, epsilon );

			var planes = new List<Plane>( resultIndices.Length / 3 );
			for( int nTriangle = 0; nTriangle < resultIndices.Length / 3; nTriangle++ )
			{
				ref var v0 = ref resultVertices[ resultIndices[ nTriangle * 3 + 0 ] ];
				ref var v1 = ref resultVertices[ resultIndices[ nTriangle * 3 + 1 ] ];
				ref var v2 = ref resultVertices[ resultIndices[ nTriangle * 3 + 2 ] ];
				Plane.FromPoints( ref v0, ref v1, ref v2, out var plane );

				if( !PlanesContains( planes, ref plane, epsilon ) )
					planes.Add( plane );
			}

			resultPlanes = planes.ToArray();
		}

		//public static void Calculate( Vector3[] vertices, out Vector3[] resultVertices, out int[] resultIndices, out Plane[] resultPlanes, double epsilon = Constants.DefaultPlaneDistanceTolerance )
		//{
		//	Calculate( vertices, null, out resultVertices, out resultIndices, epsilon );

		//	var planes = new List<Plane>( resultIndices.Length / 3 );
		//	for( int nTriangle = 0; nTriangle < resultIndices.Length / 3; nTriangle++ )
		//	{
		//		var v0 = resultVertices[ resultIndices[ nTriangle * 3 + 0 ] ];
		//		var v1 = resultVertices[ resultIndices[ nTriangle * 3 + 1 ] ];
		//		var v2 = resultVertices[ resultIndices[ nTriangle * 3 + 2 ] ];
		//		var plane = Plane.FromPoints( v0, v1, v2 );

		//		if( !PlanesContains( planes, ref plane, epsilon ) )
		//			planes.Add( plane );
		//	}

		//	resultPlanes = planes.ToArray();
		//}

		//public static void Calculate( Vector3[] vertices, out Plane[] resultPlanes, double epsilon = Constants.DefaultPlaneDistanceTolerance )
		//{
		//	Calculate( vertices, out _, out _, out resultPlanes, epsilon );
		//}
	}
}
