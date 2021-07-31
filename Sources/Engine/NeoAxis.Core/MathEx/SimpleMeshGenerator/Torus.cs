// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

using System;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		public static void GenerateTorus( int axis, double radius, int segments, Degree circumference, double tubeRadius, int tubeSegments, Degree tubeCircumference, /*bool smooth, */bool insideOut, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			if( radius < tubeRadius )
				tubeRadius = radius;
			bool loopOfLegs = Math.Abs( circumference.ToDouble() - 360 ) < 1e-5;
			bool endCapes = !loopOfLegs;

			if( radius <= 0 || tubeRadius <= 0 || segments < 3 || tubeSegments < 3 || circumference <= 0 || tubeCircumference <= 0 )
			{
				GetEmptyData( out positions, out normals, out tangents, out texCoords, out indices, out faces );
				return;
			}

			ReplaceAxes( out int axis0, out int axis1, axis );

			int nextVertex = 0;
			Point[] polygon = GenerateCircleZ( tubeSegments + 1, tubeRadius, tubeCircumference, ref nextVertex );
			FromZAxisToAxis( polygon, axis0 ); //сечение тора
			Matrix4[] m = GetTransformsOfRotationCircle( segments + 1, axis, circumference, ( radius - tubeRadius ) * GetAxisVector( axis1 ) );
			var legs = GenerateLegs( polygon, m, nextVertex, loopOfLegs );

			int rawVertexCount = CommonPipeBuilder.GetLegsRawVertexCount( legs ) + ( endCapes ? 2 * CommonPipeBuilder.GetPolygonAdaptiveRawVertexCount( polygon ) : 0 );
			var builder = new CommonPipeBuilder( rawVertexCount, ( legs.Length - 1 ) * tubeSegments + ( endCapes ? 2 : 0 ), true );
			builder.AddLegs( legs, insideOut, 1 );

			//ToDo ?? Нужны ли крышки когда circumference<360 ?  Может опцию endCapes ?
			//Add capes
			if( endCapes )
			{
				nextVertex *= legs.Length;
				Point center0 = new Point( m[ 0 ] * Vector3.Zero, nextVertex++ );
				Point center1 = new Point( m[ m.Length - 1 ] * Vector3.Zero, nextVertex++ );

				builder.AddPolygonAdaptive( legs[ 0 ], center0, !insideOut, !insideOut ); //normal противоположна от направления смещения legs поэтому !insideOut
				builder.AddPolygonAdaptive( legs[ legs.Length - 1 ], center1, insideOut, insideOut );
			}

			builder.GetData(out positions, out normals, out tangents, out texCoords, out indices, out faces);
		}

		public static void GenerateTorus( int axis, double radius, int segments, Degree circumference, double tubeRadius, int tubeSegments, Degree tubeCircumference, /*bool smooth, */bool insideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			GenerateTorus( axis, radius, segments, circumference, tubeRadius, tubeSegments, tubeCircumference, /*smooth, */insideOut, out Vector3[] positionsD, out Vector3[] normalsD, out Vector4[] tangentsD, out Vector2[] texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}
	}
}
