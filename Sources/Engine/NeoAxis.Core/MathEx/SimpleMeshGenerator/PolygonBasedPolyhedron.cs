// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		public static void GeneratePolygonBasedPolyhedron( Vector3[] points, bool clockwise, double height, bool insideOut, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			if( points.Length < 3 )
			{
				GetEmptyData( out positions, out normals, out tangents, out texCoords, out indices, out faces );
				return;
			}

			var normal = Plane.FromPoints( points[ 0 ], points[ 1 ], points[ 2 ] ).Normal;
			if( clockwise )
			{
				normal = -normal;
				insideOut = !insideOut;
			}

			int segmentsHeight = 1;
			Point[] polygon = new Point[ points.Length + 1 ];
			for( int i = 0; i < points.Length; i++ )
				polygon[ i ] = new Point( points[ i ], i );
			polygon[ polygon.Length - 1 ] = polygon[ 0 ];
			int nextVertex = points.Length;

			//var halfVector = normal * height * 0.5;
			Matrix4[] m = GetTransformsOfLinePath( segmentsHeight + 1, Vector3.Zero, normal * height );
			Point[][] legs = GenerateLegs( polygon, m, nextVertex, false );

			Vector3[] pos = new Vector3[ points.Length ];
			for( int i = 0; i < points.Length; i++ )
				pos[ i ] = legs[ 0 ][ i ].Position;


			int[] triangles = MathAlgorithms.TriangulatePolygon( pos ); //the same for the top and the bottom


			int rawVertexCount = CommonPipeBuilder.GetLegsRawVertexCount( legs ) + 2 * triangles.Length;
			var builder = new CommonPipeBuilder( rawVertexCount, points.Length * ( legs.Length - 1 ) + 2 );
			builder.AddLegs( legs, insideOut );
			builder.AddPolygon( legs[ 0 ], triangles, !insideOut, !insideOut );
			builder.AddPolygon( legs[ legs.Length - 1 ], triangles, insideOut, insideOut );
			builder.GetData( out positions, out normals, out tangents, out texCoords, out indices, out faces );
		}

		public static void GeneratePolygonBasedPolyhedron( Vector3[] points, bool clockwise, double height, bool insideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			GeneratePolygonBasedPolyhedron( points, clockwise, height, insideOut, out Vector3[] positionsD, out Vector3[] normalsD, out Vector4[] tangentsD, out Vector2[] texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}
	}
}
