// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

using System;
using System.Diagnostics;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		public static void GeneratePipe( int axis, double radius, double height, double thickness, int segments, int segmentsHeight, bool insideOut, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			if( radius < thickness )
				thickness = radius - 1e-6;
			if( radius <= 0 || segments < 3 )
			{
				GetEmptyData( out positions, out normals, out tangents, out texCoords, out indices, out faces );
				return;
			}

			bool hasTopBottom = 0 < thickness;
			int legPointCount = segments + 1;
			int vertexOffset = 0;
			Point[] polygonOuter = GenerateCircleZ( legPointCount, radius, 360, ref vertexOffset );
			FromZAxisToAxis( polygonOuter, axis );
			Point[] polygonInner = GenerateCircleZ( legPointCount, radius - thickness, 360, ref vertexOffset );
			FromZAxisToAxis( polygonInner, axis );

			var halfVector = 0.5 * height * GetAxisVector( axis );
			Matrix4[] m = GetTransformsOfLinePath( segmentsHeight + 1, -halfVector, halfVector );
			var legsOuter = GenerateLegs( polygonOuter, m, vertexOffset, false );
			var legsInner = GenerateLegs( polygonInner, m, vertexOffset, false );

			int rawVertexCount = 2 * CommonPipeBuilder.GetLegsRawVertexCount( legsOuter ) + ( hasTopBottom ? 2 * CommonPipeBuilder.GetPolygon2RawVertexCount( polygonOuter ) : 0 );
			int faceCount = 2 * segments * ( legsOuter.Length - 1 ) + ( hasTopBottom ? 2 : 0 );
			var builder = new CommonPipeBuilder( rawVertexCount, faceCount, true );
			builder.AddLegs( legsOuter, insideOut, 1 );
			builder.AddLegs( legsInner, !insideOut, 1 );

			if( hasTopBottom )
			{
				//top,bottom
				builder.AddPolygon2( legsOuter[ 0 ], legsInner[ 0 ], !insideOut, false );
				builder.AddPolygon2( legsOuter[ legsOuter.Length - 1 ], legsInner[ legsInner.Length - 1 ], insideOut, false );
			}

			builder.GetData( out positions, out normals, out tangents, out texCoords, out indices, out faces );

		}

		public static void GeneratePipe( int axis, double radius, double height, double thickness, int segments, int segmentsHeight, bool insideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			GeneratePipe( axis, radius, height, thickness, segments, segmentsHeight, insideOut, out Vector3[] positionsD, out Vector3[] normalsD, out Vector4[] tangentsD, out Vector2[] texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}

		

	}
}
