// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		public static void GeneratePrism( int axis, double radius, double height, int segments, bool insideOut, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			if( radius <= 0 || segments < 3 )
			{
				GetEmptyData( out positions, out normals, out tangents, out texCoords, out indices, out faces );
				return;
			}
			int segmentsHeight = 1;
			int nextVertex = 0;
			Point[] polygon = GenerateCircleZ( segments + 1, radius, 360, ref nextVertex );
			FromZAxisToAxis( polygon, axis );

			var halfVector = GetAxisVector( axis ) * height * 0.5;
			Matrix4[] m = GetTransformsOfLinePath( segmentsHeight + 1, -halfVector, halfVector );

			Point center0 = new Point( m[ 0 ] * Vector3.Zero, nextVertex++ );
			Point center1 = new Point( m[ m.Length - 1 ] * Vector3.Zero, nextVertex++ );
			Point[][] legs = GenerateLegs( polygon, m, nextVertex, false );

			int rawVertexCount = CommonPipeBuilder.GetLegsRawVertexCount( legs ) + 2 * CommonPipeBuilder.GetPolygonAdaptiveRawVertexCount( polygon );
			var builder = new CommonPipeBuilder( rawVertexCount, segments * ( legs.Length - 1 ) + 2 );
			builder.AddLegs( legs, insideOut );
			builder.AddPolygonAdaptive( legs[ 0 ], center0, !insideOut, !insideOut );
			builder.AddPolygonAdaptive( legs[ legs.Length - 1 ], center1, insideOut, insideOut );
			builder.GetData( out positions, out normals, out tangents, out texCoords, out indices, out faces );
		}
		
		public static void GeneratePrism( int axis, double radius, double height, int segments, bool insideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			GeneratePrism( axis, radius, height, segments, insideOut, out Vector3[] positionsD, out Vector3[] normalsD, out Vector4[] tangentsD, out Vector2[] texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}

		

	}
}
