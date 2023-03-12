// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		//ToDo : Логичнее когда главное направление для оси, в направлении входа. При axis=1,2 арка стоит, при axis=2 лежит на боку - тогда задать default axis=2
		//ToDo ??? Надо ли сделать чтобы нижние поверхности были паралельны земле при circumference<180 ?(тогда угол у внутреннего кольца будет меньше).
		public static void GenerateArch( int axis, double radius, double thickness, double depth, int segments, int segmentsDepth, Degree circumference, bool endCapes, bool insideOut, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			if( radius < thickness )
				thickness = radius;

			if( radius <= 0 || segments < 2 || segmentsDepth < 0 || circumference <= 0 )
			{
				GetEmptyData( out positions, out normals, out tangents, out texCoords, out indices, out faces );
				return;
			}

			//ToDo ??? endCapes это передняя/задняя или нижние поверхности ?
			bool frontBack = endCapes && 1e-8 < thickness;
			bool bottom = endCapes;

			int legPointCount = segments + 1;
			int nextVertex = 0;

			double startAngle = ( ( Math.PI - circumference.InRadians() ) / 2 );
			Point[] outer = GenerateCircleZ( legPointCount, radius, circumference, ref nextVertex, startAngle );
			FromZAxisToAxis( outer, axis );
			Point[] inner = GenerateCircleZ( legPointCount, radius - thickness, circumference, ref nextVertex, startAngle );
			FromZAxisToAxis( inner, axis );
			Point[] down1 = { inner[ 0 ], outer[ 0 ] };
			Point[] down2 = { outer[ outer.Length - 1 ], inner[ inner.Length - 1 ] };

			var halfVector = 0.5 * depth * GetAxisVector( axis );
			Matrix4[] m = GetTransformsOfLinePath( segmentsDepth + 1, -halfVector, halfVector );

			var legsOuter = GenerateLegs( outer, m, nextVertex, false );
			var legsInner = GenerateLegs( inner, m, nextVertex, false );
			Point[][] legsDown1 = null;
			Point[][] legsDown2 = null;
			if( bottom )
			{
				legsDown1 = GenerateLegs( down1, m, nextVertex, false );
				legsDown2 = GenerateLegs( down2, m, nextVertex, false );
			}

			int rawVertexCount = 2 * CommonPipeBuilder.GetLegsRawVertexCount( legsOuter ) +
				( legsDown1 == null ? 0 : 2 * CommonPipeBuilder.GetLegsRawVertexCount( legsDown1 ) ) +
				( frontBack ? 2 * CommonPipeBuilder.GetPolygon2RawVertexCount( outer ) : 0 );
			int faceCount = 2 * segments * ( legsOuter.Length - 1 ) + 2 * ( bottom ? legsOuter.Length - 1 : 0 ) + ( frontBack ? 2 : 0 );
			var builder = new CommonPipeBuilder( rawVertexCount, faceCount ,true );
			builder.AddLegs( legsOuter, insideOut, 1 );
			builder.AddLegs( legsInner, !insideOut, 1 );

			if( bottom )
			{
				builder.AddLegs( legsDown1, insideOut );
				builder.AddLegs( legsDown2, insideOut );
			}

			if( frontBack )
			{
				builder.AddPolygon2( legsOuter[ 0 ], legsInner[ 0 ], !insideOut, !insideOut ); //normal противоположна от направления смещения legs поэтому !insideOut
				builder.AddPolygon2( legsOuter[ legsOuter.Length - 1 ], legsInner[ legsInner.Length - 1 ], insideOut, insideOut );
			}

			builder.GetData( out positions, out normals, out tangents, out texCoords, out indices, out faces );
		}

		public static void GenerateArch( int axis, double radius, double thickness, double depth, int segments, int segmentsDepth, Degree circumference, bool endCapes, bool insideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			GenerateArch( axis, radius, thickness, depth, segments, segmentsDepth, circumference, endCapes, insideOut, out Vector3[] positionsD, out Vector3[] normalsD, out Vector4[] tangentsD, out Vector2[] texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}
	}
}
