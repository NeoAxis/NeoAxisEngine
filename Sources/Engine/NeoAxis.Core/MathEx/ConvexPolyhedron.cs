// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Represents a convex polyhedron.
	/// </summary>
	public class ConvexPolyhedron
	{
		Vector3[] vertices;
		Face[] faces;
		double epsilon;

		Edge[] edges;
		Plane[] facePlanes;

		//////////////////////////////////////////

		/// <summary>
		/// Represents a face of <see cref="ConvexPolyhedron"/>.
		/// </summary>
		public struct Face
		{
			int vertex0;
			int vertex1;
			int vertex2;

			public Face( int vertex0, int vertex1, int vertex2 )
			{
				this.vertex0 = vertex0;
				this.vertex1 = vertex1;
				this.vertex2 = vertex2;
			}

			public int Vertex0
			{
				get { return vertex0; }
				set { vertex0 = value; }
			}

			public int Vertex1
			{
				get { return vertex1; }
				set { vertex1 = value; }
			}

			public int Vertex2
			{
				get { return vertex2; }
				set { vertex2 = value; }
			}
		}

		//////////////////////////////////////////

		/// <summary>
		/// Represents an edge of <see cref="ConvexPolyhedron"/>.
		/// </summary>
		public struct Edge
		{
			int vertex0;
			int vertex1;

			internal Edge( int vertex0, int vertex1 )
			{
				this.vertex0 = vertex0;
				this.vertex1 = vertex1;
			}

			public int Vertex0
			{
				get { return vertex0; }
				set { vertex0 = value; }
			}

			public int Vertex1
			{
				get { return vertex1; }
				set { vertex1 = value; }
			}
		}

		//////////////////////////////////////////

		public ConvexPolyhedron( Vector3[] vertices, Face[] faces, double epsilon )
		{
			this.vertices = vertices;
			this.faces = faces;
			this.epsilon = epsilon;
		}

		public ConvexPolyhedron( Vector3[] vertices, int[] indices, double epsilon )
		{
			this.vertices = vertices;

			faces = new Face[ indices.Length / 3 ];
			for( int n = 0; n < faces.Length; n++ )
			{
				faces[ n ] = new Face(
					indices[ n * 3 + 0 ],
					indices[ n * 3 + 1 ],
					indices[ n * 3 + 2 ] );
			}

			this.epsilon = epsilon;
		}

		public Vector3[] Vertices
		{
			get { return vertices; }
		}

		public Face[] Faces
		{
			get { return faces; }
		}

		public double Epsilon
		{
			get { return epsilon; }
		}

		//////////////////////////////////////////

		class HullInternal
		{
			//Vertex[] vertices;
			Vector3[] vertices;
			double epsilon;

			BitArray checkedTriangles;

			//

			public HullInternal( IList<Vector3> vertices, double epsilon )
			{
				this.vertices = new Vector3[ vertices.Count ];
				for( int n = 0; n < vertices.Count; n++ )
					this.vertices[ n ] = vertices[ n ];
				this.epsilon = epsilon;

				//this.vertices = new Vertex[ vertices.Count ];
				//for( int n = 0; n < vertices.Count; n++ )
				//{
				//   Vertex vertex = new Vertex();
				//   vertex.position = vertices[ n ];
				//   this.vertices[ n ] = vertex;
				//}

				checkedTriangles = new BitArray( vertices.Count * vertices.Count * vertices.Count );
			}

			public bool AreCollinear( ref Vector3 v1, ref Vector3 v2, ref Vector3 v3 )
			{
				//const double epsilon = .001;

				bool n1 = Math.Abs(
					( v3.Z - v1.Z ) * ( v2.Y - v1.Y ) -
					( v2.Z - v1.Z ) * ( v3.Y - v1.Y ) ) < epsilon;
				bool n2 = Math.Abs(
					( v2.Z - v1.Z ) * ( v3.X - v1.X ) -
					( v2.X - v1.X ) * ( v3.Z - v1.Z ) ) < epsilon;
				bool n3 = Math.Abs(
					( v2.X - v1.X ) * ( v3.Y - v1.Y ) -
					( v2.Y - v1.Y ) * ( v3.X - v1.X ) ) < epsilon;

				return n1 && n2 && n3;
			}

			//public bool AreCollinear( Vertex v1, Vertex v2, Vertex v3 )
			//{
			//   const double epsilon = .001f;

			//   bool n1 = Math.Abs(
			//      ( v3.position.Z - v1.position.Z ) * ( v2.position.Y - v1.position.Y ) -
			//      ( v2.position.Z - v1.position.Z ) * ( v3.position.Y - v1.position.Y ) ) < epsilon;
			//   bool n2 = Math.Abs(
			//      ( v2.position.Z - v1.position.Z ) * ( v3.position.X - v1.position.X ) -
			//      ( v2.position.X - v1.position.X ) * ( v3.position.Z - v1.position.Z ) ) < epsilon;
			//   bool n3 = Math.Abs(
			//      ( v2.position.X - v1.position.X ) * ( v3.position.Y - v1.position.Y ) -
			//      ( v2.position.Y - v1.position.Y ) * ( v3.position.X - v1.position.X ) ) < epsilon;

			//   return n1 && n2 && n3;
			//}

			bool IsPlaneIncludesAllVertices( ref Plane plane )
			{
				//const double epsilon = .001;

				foreach( Vector3 vertex in vertices )
				{
					if( plane.GetDistance( vertex ) > epsilon )
						return false;
				}
				//foreach( Vertex vertex in vertices )
				//{
				//   if( plane.GetDistance( ref vertex.position ) > epsilon )
				//      return false;
				//}

				return true;
			}

			int GetCheckedTriangleKey( int nVertex0, int nVertex1, int nVertex2 )
			{
				int n;

				if( nVertex1 < nVertex0 )
				{
					n = nVertex0;
					nVertex0 = nVertex1;
					nVertex1 = n;
				}
				if( nVertex2 < nVertex0 )
				{
					n = nVertex0;
					nVertex0 = nVertex2;
					nVertex2 = n;
				}
				if( nVertex2 < nVertex1 )
				{
					n = nVertex1;
					nVertex1 = nVertex2;
					nVertex2 = n;
				}

				return ( nVertex0 * vertices.Length + nVertex1 ) * vertices.Length + nVertex2;
			}

			public Plane[] Calculate()
			{
				//const double epsilon = .001;

				List<Plane> planes = new List<Plane>();

				for( int nVertex0 = 0; nVertex0 < vertices.Length; nVertex0++ )
				{
					//Vertex vertex0 = vertices[ nVertex0 ];
					Vector3 vertex0 = vertices[ nVertex0 ];

					for( int nVertex1 = 0; nVertex1 < vertices.Length; nVertex1++ )
					{
						if( nVertex1 == nVertex0 )
							continue;

						//Vertex vertex1 = vertices[ nVertex1 ];
						Vector3 vertex1 = vertices[ nVertex1 ];

						for( int nVertex2 = 0; nVertex2 < vertices.Length; nVertex2++ )
						{
							if( nVertex2 == nVertex0 || nVertex2 == nVertex1 )
								continue;

							//check for already processed
							int key = GetCheckedTriangleKey( nVertex0, nVertex1, nVertex2 );
							if( checkedTriangles[ key ] )
								continue;
							checkedTriangles[ key ] = true;

							//Vertex vertex2 = vertices[ nVertex2 ];
							Vector3 vertex2 = vertices[ nVertex2 ];

							//check for equal vertices
							if( vertex0.Equals( ref vertex1, epsilon ) )
								continue;
							if( vertex0.Equals( ref vertex2, epsilon ) )
								continue;
							if( vertex1.Equals( ref vertex2, epsilon ) )
								continue;

							//if( vertex0.position.Equals( ref vertex1.position, epsilon ) )
							//   continue;
							//if( vertex0.position.Equals( ref vertex2.position, epsilon ) )
							//   continue;
							//if( vertex1.position.Equals( ref vertex2.position, epsilon ) )
							//   continue;

							//check for collinear triangles
							//bool collinear = AreCollinear( vertex0, vertex1, vertex2 );
							bool collinear = AreCollinear( ref vertex0, ref vertex1, ref vertex2 );
							if( collinear )
								continue;

							Plane plane;
							//Plane.FromPoints( ref vertex0.position, ref vertex1.position,
							//   ref vertex2.position, out plane );
							Plane.FromPoints( ref vertex0, ref vertex1, ref vertex2, out plane );
							if( IsPlaneIncludesAllVertices( ref plane ) )
								planes.Add( plane );

							Plane negatePlane;
							Plane.Negate( ref plane, out negatePlane );
							if( IsPlaneIncludesAllVertices( ref negatePlane ) )
								planes.Add( negatePlane );
						}
					}
				}

				List<Plane> result = new List<Plane>( planes.Count );

				//delete equal planes
				foreach( Plane plane in planes )
				{
					bool exists = false;
					for( int n = 0; n < result.Count; n++ )
					{
						Plane plane2 = result[ n ];
						if( plane.Equals( ref plane2, epsilon ) )
						{
							exists = true;
							break;
						}
					}

					if( !exists )
						result.Add( plane );
				}

				return result.ToArray();
			}
		}

		public static Plane[] GetConvexPolyhedronPlanesFromVertices( IList<Vector3> vertices, double epsilon )
		{
			HullInternal hull = new HullInternal( vertices, epsilon );
			return hull.Calculate();
		}

		public static Plane[] GetConvexPolyhedronPlanesFromVertices( IList<Vector3F> vertices, double epsilon )
		{
			Vector3[] verticesD = new Vector3[ vertices.Count ];
			for( int n = 0; n < verticesD.Length; n++ )
				verticesD[ n ] = vertices[ n ].ToVector3();
			HullInternal hull = new HullInternal( verticesD, epsilon );
			return hull.Calculate();
		}

		///////////////////////////////////////////

		bool ContainsPoint( Vector3 point )
		{
			//const double epsilon = .001;

			foreach( Plane plane in FacePlanes )
			{
				if( plane.GetDistance( point ) > epsilon )
					return false;
			}

			return true;
		}

		bool IntersectsLine( Line3 line )
		{
			if( ContainsPoint( line.Start ) )
				return true;
			if( ContainsPoint( line.End ) )
				return true;

			Ray ray = new Ray( line.Start, line.End - line.Start );

			foreach( Face face in Faces )
			{
				ref Vector3 triangle0 = ref vertices[ face.Vertex0 ];
				ref Vector3 triangle1 = ref vertices[ face.Vertex1 ];
				ref Vector3 triangle2 = ref vertices[ face.Vertex2 ];

				//need epsilon?
				double scale;
				if( MathAlgorithms.IntersectTriangleRay( ref triangle0, ref triangle1, ref triangle2, ref ray, out scale ) )
				{
					if( scale >= 0 && scale <= 1 )
						return true;
				}
			}

			return false;
		}

		//int WhichSide( Plane plane )
		//{
		//	//const double epsilon = .001;

		//	// S vertices are projected to the form P+t*D. Return value is +1 if all t > 0,
		//	// -1 if all t < 0, 0 otherwise, in which case the line splits the polygon.
		//	int positive = 0;
		//	int negative = 0;
		//	foreach( Vector3 vertex in vertices )
		//	{
		//		double distance = plane.GetDistance( vertex );

		//		if( distance > epsilon )
		//			positive++;
		//		else if( distance < -epsilon )
		//			negative++;

		//		if( positive != 0 && negative != 0 )
		//			return 0;
		//	}
		//	return ( positive != 0 ? 1 : -1 );
		//}

		public Vector3 GetApproximateConvexCentroid()
		{
			Vector3 result = Vector3.Zero;
			foreach( Vector3 v in vertices )
				result += v;
			return result / (float)vertices.Length;
		}

		public static bool IsIntersects( ConvexPolyhedron polyhedron1, ConvexPolyhedron polyhedron2 )
		{
			Vector3 center1 = polyhedron1.GetApproximateConvexCentroid();
			if( polyhedron2.ContainsPoint( center1 ) )
				return true;
			Vector3 center2 = polyhedron2.GetApproximateConvexCentroid();
			if( polyhedron1.ContainsPoint( center2 ) )
				return true;

			//check for each edge interesction with polyhedron
			{
				bool intersectionDetected = false;

				foreach( Edge edge in polyhedron1.Edges )
				{
					Vector3 edge0 = polyhedron1.vertices[ edge.Vertex0 ];
					Vector3 edge1 = polyhedron1.vertices[ edge.Vertex1 ];

					if( polyhedron2.IntersectsLine( new Line3( edge0, edge1 ) ) )
					{
						intersectionDetected = true;
						break;
					}
				}

				if( !intersectionDetected )
				{
					foreach( Edge edge in polyhedron2.Edges )
					{
						Vector3 edge0 = polyhedron2.vertices[ edge.Vertex0 ];
						Vector3 edge1 = polyhedron2.vertices[ edge.Vertex1 ];

						if( polyhedron1.IntersectsLine( new Line3( edge0, edge1 ) ) )
						{
							intersectionDetected = true;
							break;
						}
					}
				}

				if( !intersectionDetected )
					return false;
			}

			return true;
		}

		void GenerateEdges()
		{
			if( edges != null )
				return;

			List<Edge> list = new List<Edge>( faces.Length * 3 / 2 );

			BitArray addedEdges = new BitArray( vertices.Length * vertices.Length );

			for( int faceIndex = 0; faceIndex < faces.Length; faceIndex++ )
			{
				Face face = faces[ faceIndex ];

				for( int side = 0; side < 3; side++ )
				{
					int vertexIndex0 = -1;
					int vertexIndex1 = -1;

					switch( side )
					{
					case 0: vertexIndex0 = face.Vertex0; vertexIndex1 = face.Vertex1; break;
					case 1: vertexIndex0 = face.Vertex1; vertexIndex1 = face.Vertex2; break;
					case 2: vertexIndex0 = face.Vertex2; vertexIndex1 = face.Vertex0; break;
					}

					if( vertexIndex1 < vertexIndex0 )
					{
						var z = vertexIndex0; 
						vertexIndex0 = vertexIndex1; 
						vertexIndex1 = z;
					}

					int key = vertexIndex0 * vertices.Length + vertexIndex1;

					if( !addedEdges[ key ] )
					{
						addedEdges[ key ] = true;

						list.Add( new Edge( vertexIndex0, vertexIndex1 ) );
					}
				}
			}


			edges = list.ToArray();
		}

		public Edge[] Edges
		{
			get
			{
				if( edges == null )
					GenerateEdges();
				return edges;
			}
		}

		void GenerateFacePlanes()
		{
			if( facePlanes != null )
				return;

			facePlanes = new Plane[ faces.Length ];

			for( int n = 0; n < faces.Length; n++ )
			{
				Face face = faces[ n ];

				ref var vertex0 = ref vertices[ face.Vertex0 ];
				ref var vertex1 = ref vertices[ face.Vertex1 ];
				ref var vertex2 = ref vertices[ face.Vertex2 ];

				Plane.FromPoints( ref vertex0, ref vertex1, ref vertex2, out var plane );

				facePlanes[ n ] = plane;
			}
		}

		public Plane[] FacePlanes
		{
			get
			{
				if( facePlanes == null )
					GenerateFacePlanes();
				return facePlanes;
			}
		}

	}
}
