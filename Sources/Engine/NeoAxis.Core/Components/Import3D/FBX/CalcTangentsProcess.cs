// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Import.FBX
{
	//Warning: Результаты расчета отличаются от tangents рассчитаных через FBX, они обычно перпендикулярны тем что в FBX.
	//?? В расчете tangents есть сглаживание, поэтому вопрос: Выполнять расчет до разбиения на подмеши по материалам, а также до триангуляции или после?
	//?? Для сглаживания используется сортировка, можно было бы ускорить, если сначала убрать одинаковые вертексы(проиндексировать), но результат изменится, т.к. используется сглаживание по близким вертексам.
	//В Assimp используются не ControlPoints из Fbx файла а PolygonPoints, т.к. у всех массивов(position, tanhent,normal,...) одинаковый размер.

	//Assimp algorithm
	static class CalcTangentsProcess
	{
		//const float AngleEpsilon = 0.9999f;

		public delegate Vector2F GetTextureCoord( ref StandardVertex vertex );

		/// <summary>
		/// Requirements : normals and UV coordinates must be present. 
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="tangents"></param>
		/// <param name="bitangents"></param>
		/// <param name="getTextureCoord">if null then texCoord0 is used, if not null this delegete than takes the necessary texCoord</param>
		public static void CalculateTangents( VertexInfo[] vertices, out Vector3F[] tangents, out Vector3F[] bitangents, GetTextureCoord getTextureCoord = null )
		{
			tangents = new Vector3F[ vertices.Length ];
			bitangents = new Vector3F[ vertices.Length ];
			if( vertices.Length == 0 )
				return;
			foreach( var polygonIndexRange in CalcMiscProcess.EnumeratePolygons( vertices ) )
				CalculateTangentForFace( vertices, polygonIndexRange, tangents, bitangents, getTextureCoord );
			//SmoothTangents( vertices, tangents, bitangents, FbxMath.ComputePositionEpsilon( vertices ), maxSmoothingAngleInRadians );
		}


		static void CalculateTangentForFace( VertexInfo[] vertices, RangeI indexRange, Vector3F[] tangents, Vector3F[] bitangents, GetTextureCoord getTextureCoord = null )
		{
			// triangle or polygon... we always use only the first three indices. A polygon
			// is supposed to be planar anyways....

			if( indexRange.Size < 3 )
			{
				for( int i = indexRange.Minimum; i < indexRange.Maximum; i++ )
				{
					tangents[ i ] = new Vector3F( float.NaN, float.NaN, float.NaN );
					bitangents[ i ] = new Vector3F( float.NaN, float.NaN, float.NaN );
				}
				FbxImportLog.LogWarning( "To calculate the tangents a polygon must have > 2 vertices" );
				return;
			}

			ref StandardVertex pt0 = ref vertices[ indexRange.Minimum ].Vertex;
			ref StandardVertex pt1 = ref vertices[ indexRange.Minimum + 1 ].Vertex;
			ref StandardVertex pt2 = ref vertices[ indexRange.Minimum + 2 ].Vertex;

			// position differences p1->p2 and p1->p3
			var v1 = pt1.Position - pt0.Position;
			var v2 = pt2.Position - pt0.Position;

			//t1 - corresponds to v1 in texture, t2 to v2.
			// ReSharper disable InconsistentNaming
			float t1x, t1y, t2x, t2y;
			// ReSharper restore InconsistentNaming
			// texture offset p1->p2 and p1->p3
			if( getTextureCoord == null )
			{
				t1x = pt1.TexCoord0.X - pt0.TexCoord0.X;
				t1y = pt1.TexCoord0.Y - pt0.TexCoord0.Y;
				t2x = pt2.TexCoord0.X - pt0.TexCoord0.X;
				t2y = pt2.TexCoord0.Y - pt0.TexCoord0.Y;
			}
			else
			{
				var tc0 = getTextureCoord( ref pt0 );
				var tc1 = getTextureCoord( ref pt1 );
				var tc2 = getTextureCoord( ref pt2 );
				t1x = tc1.X - tc0.X;
				t1y = tc1.Y - tc0.Y;
				t2x = tc2.X - tc0.X;
				t2y = tc2.Y - tc0.Y;
			}

			// //pseudoscalar product
			float pseudoscalarProduct = t2x * t1y - t2y * t1x; // t2 ^ t1 == |t2|*|t1|*Sin(a), where a is an angle from t2 to t1 counterclockwise
			float dirCorrection = pseudoscalarProduct < 0.0f ? -1.0f : 1.0f;

			//Warning: В Assimp сравнение без Epsilon. Хотя если при равенстве 0, выбирают произвольные значения, может и не надо Epsilon?
			// when pt1.texCoord, pt2.texCoord, pt3.texCoord in same position in UV space, just use default UV direction.
			if( pseudoscalarProduct == 0 )
			{
				t1x = 0.0f; t1y = 1.0f;
				t2x = 1.0f; t2y = 0.0f;
			}

			// tangent points in the direction where to positive X axis of the texture coord's would point in model space
			// bitangent's points along the positive Y axis of the texture coord's, respectively
			Vector3F tangent = Vector3F.Zero, bitangent = Vector3F.Zero;
			//tangent = v2 * t1y - v1 *t2y,
			//т.е. если один из векторов t2 или t1 направлен вдоль тектурной оси x, то tangent направлен в ту же сторону.
			tangent.X = ( v2.X * t1y - v1.X * t2y ) * dirCorrection;
			tangent.Y = ( v2.Y * t1y - v1.Y * t2y ) * dirCorrection;
			tangent.Z = ( v2.Z * t1y - v1.Z * t2y ) * dirCorrection;

			bitangent.X = ( v2.X * t1x - v1.X * t2x ) * dirCorrection;
			bitangent.Y = ( v2.Y * t1x - v1.Y * t2x ) * dirCorrection;
			bitangent.Z = ( v2.Z * t1x - v1.Z * t2x ) * dirCorrection;

			// store for every vertex of that face
			for( int i = indexRange.Minimum; i < indexRange.Maximum; ++i )
			{

				// project tangent and bitangent into the plane formed by the vertex' normal
				var normal = vertices[ i ].Vertex.Normal;
				Vector3F localTangent = tangent - normal * ( tangent * normal );
				Vector3F localBitangent = bitangent - normal * ( bitangent * normal );
				localTangent.Normalize();
				localBitangent.Normalize();

				// reconstruct tangent/bitangent according to normal and bitangent/tangent when it's infinite or NaN.
				bool invalidTangent = FbxMath.IsSpecialFloat( localTangent.X ) || FbxMath.IsSpecialFloat( localTangent.Y ) || FbxMath.IsSpecialFloat( localTangent.Z );
				bool invalidBitangent = FbxMath.IsSpecialFloat( localBitangent.X ) || FbxMath.IsSpecialFloat( localBitangent.Y ) || FbxMath.IsSpecialFloat( localBitangent.Z );
				if( invalidTangent != invalidBitangent )
				{
					if( invalidTangent )
					{
						localTangent = Vector3F.Cross( normal, localBitangent );
						localTangent.Normalize();
					}
					else
					{
						localBitangent = Vector3F.Cross( localTangent, normal );
						localBitangent.Normalize();
					}
				}

				// and write it into the mesh.
				tangents[ i ] = localTangent;
				bitangents[ i ] = localBitangent;
			}
		}

		///// <summary>
		///// </summary>
		///// <param name="vertices"></param>
		///// <param name="vertexFinder"></param>
		///// <param name="tangents"></param>
		///// <param name="bitangents"></param>
		///// <param name="posEpsilon"></param>
		///// <param name="maxSmoothingAngleInRadians">Maximum smoothing angle, in radians.</param>
		//public static void SmoothTangents( VertexInfo[] vertices, SpatialSort vertexFinder, Vector3F[] tangents, Vector3F[] bitangents, float posEpsilon, float maxSmoothingAngleInRadians = (float)( 45f * Math.PI / 180 ) )
		//{
		//	float fLimit = MathEx.Cos( maxSmoothingAngleInRadians );
		//	var closeVertices = new List<int>();
		//	bool[] vertexDone = new bool[ vertices.Length ];
		//	// in the second pass we now smooth out all tangents and bitangents at the same local position
		//	// if they are not too far off.
		//	for( int i = 0; i < vertices.Length; i++ )
		//	{
		//		if( vertexDone[ i ] )
		//			continue;

		//		Vector3F origPos = vertices[ i ].Vertex.Position;
		//		Vector3F origNorm = vertices[ i ].Vertex.Normal;
		//		Vector3F origTang = tangents[ i ];
		//		Vector3F origBitang = bitangents[ i ];

		//		// find all vertices close to that position
		//		var verticesFound = vertexFinder.FindPositions( origPos, posEpsilon );
		//		closeVertices.Clear();
		//		closeVertices.Add( i );

		//		// look among them for other vertices sharing the same normal and a close-enough tangent/bitangent
		//		for( int b = 0; b < verticesFound.Count; b++ )
		//		{
		//			int idx = verticesFound[ b ];
		//			if( vertexDone[ idx ] )
		//				continue;
		//			if( Vector3F.Dot( vertices[ idx ].Vertex.Normal, origNorm ) < AngleEpsilon )
		//				continue;
		//			if( Vector3F.Dot( tangents[ idx ], origTang ) < fLimit )
		//				continue;
		//			if( Vector3F.Dot( bitangents[ idx ], origBitang ) < fLimit )
		//				continue;

		//			// it's similar enough -> add it to the smoothing group
		//			closeVertices.Add( idx );
		//			vertexDone[ idx ] = true;
		//		}

		//		// smooth the tangents and bitangents of all vertices that were found to be close enough
		//		Vector3F smoothTangent = new Vector3F();
		//		Vector3F smoothBitangent = new Vector3F();
		//		for( int b = 0; b < closeVertices.Count; ++b )
		//		{
		//			smoothTangent += tangents[ closeVertices[ b ] ];
		//			smoothBitangent += bitangents[ closeVertices[ b ] ];
		//		}
		//		smoothTangent.Normalize();
		//		smoothBitangent.Normalize();

		//		// and write it back into all affected tangents
		//		for( int b = 0; b < closeVertices.Count; ++b )
		//		{
		//			tangents[ closeVertices[ b ] ] = smoothTangent;
		//			bitangents[ closeVertices[ b ] ] = smoothBitangent;
		//		}
		//	}
		//}
	}
}
