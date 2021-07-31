// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

using System;
using System.Collections.Generic;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		public static void GenerateDoor( int axis, double width, double height, double depth, double doorWidth, double doorHeight, bool insideOut, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{

			if( width <= doorWidth )
			{
				width = Math.Max( width, doorWidth );
				doorWidth = width - 1e-6;
			}
			if( height <= doorHeight )
			{
				height = Math.Max( height, doorHeight );
				doorHeight = height - 1e-6;
			}
			if( width <= 0 || height <= 0 || doorWidth <= 0 || doorHeight <= 0 )
			{
				GetEmptyData( out positions, out normals, out tangents, out texCoords, out indices, out faces );
				return;
			}

			double halfWidth = width * 0.5;
			double halfWidthInner = doorWidth * 0.5;
			double heightInner = doorHeight;
			int segmentsDepth = 1;

			Point[] pointsInner =
			{
				new Point(new Vector3( halfWidthInner, 0, 0), 0 ),
				new Point(new Vector3( halfWidthInner, heightInner, 0),  1),
				new Point(new Vector3( -halfWidthInner, heightInner, 0), 2 ),
				new Point(new Vector3( -halfWidthInner, 0, 0), 3),
			};
			Point[] pointsOuter =
			{
				new Point(new Vector3( halfWidth, 0, 0), 4 ),
				new Point(new Vector3( halfWidth, height, 0),  5),
				new Point(new Vector3( -halfWidth, height, 0), 6 ),
				new Point(new Vector3( -halfWidth, 0, 0), 7),
			};

			int nextVertex = 8;

			FromZAxisToAxis( pointsInner, axis );
			FromZAxisToAxis( pointsOuter, axis );

			var halfVector = 0.5 * depth * GetAxisVector( axis );
			Matrix4[] m = GetTransformsOfLinePath( segmentsDepth + 1, -halfVector, halfVector );
			Point[][] legsInner = GenerateLegs( pointsInner, m, nextVertex, false );
			Point[][] legsOuter = GenerateLegs( pointsOuter, m, nextVertex, false );

			int rawVertexCount = CommonPipeBuilder.GetLegsRawVertexCount( legsInner ) + 2 * CommonPipeBuilder.GetPolygon2RawVertexCount( legsOuter[ 0 ] );
			var builder = new CommonPipeBuilder( rawVertexCount, 3 + 2 );
			builder.AddLegs( legsInner, !insideOut );
			builder.AddPolygon2( legsOuter[ 0 ], legsInner[ 0 ], !insideOut, !insideOut ); //normal противоположна от направления смещения legs поэтому !insideOut
			builder.AddPolygon2( legsOuter[ legsOuter.Length - 1 ], legsInner[ legsInner.Length - 1 ], insideOut, insideOut );

			builder.GetData( out positions, out normals, out tangents, out texCoords, out indices, out faces );

		}

		public static void GenerateDoor( int axis, double width, double height, double depth, double doorWidth, double doorHeight, bool insideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			GenerateDoor( axis, width, height, depth, doorWidth, doorHeight, insideOut, out Vector3[] positionsD, out Vector3[] normalsD, out Vector4[] tangentsD, out Vector2[] texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}
	}
}
