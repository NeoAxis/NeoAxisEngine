// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;

namespace NeoAxis.Import.FBX
{
	//Based on:  GenVertexNormalsProcess.h,GenFaceNormalsProcess.h,  Кроме этого есть : FixNormalsStep.h, , 
	static class CalcNormalsProcess
	{
		//const float DefaultSmoothingAngleInRadians = (float)( 175f * Math.PI / 180 );

		public static void CalculateNormals( VertexInfo[] vertices, bool normalizeVectorNormal )
		{
			if( vertices.Length == 0 )
				return;

			foreach( var polygonIndexRange in CalcMiscProcess.EnumeratePolygons( vertices ) )
				CalculateNormalForFace( vertices, polygonIndexRange, normalizeVectorNormal );
		}

		// Compute per-face normals but store them per-vertex
		static void CalculateNormalForFace( VertexInfo[] vertices, RangeI indexRange, bool normalize )
		{
			if( indexRange.Size < 3 )
			{
				for( int i = indexRange.Minimum; i < indexRange.Maximum; i++ )
					vertices[ i ].Vertex.Normal = new Vector3F( Single.NaN, Single.NaN, Single.NaN );
				FbxImportLog.LogWarning( "To calculate the tangents a polygon must have > 2 vertices" );
			}

			ref Vector3F p0 = ref vertices[ indexRange.Minimum ].Vertex.Position;
			Vector3F normal = Vector3F.Cross( vertices[ indexRange.Minimum + 1 ].Vertex.Position - p0, vertices[ indexRange.Maximum - 1 ].Vertex.Position - p0 );
			if( normalize )
				normal.Normalize();
			for( int i = indexRange.Minimum; i < indexRange.Maximum; i++ )
				vertices[ i ].Vertex.Normal = normal;
		}

		//public static void SmoothNormals( VertexInfo[] vertices, SpatialSort vertexFinder, float posEpsilon, float maxSmoothingAngleInRadians = DefaultSmoothingAngleInRadians )
		//{
		//	var pcNew = new Vector3F[ vertices.Length ];
		//	if( DefaultSmoothingAngleInRadians <= maxSmoothingAngleInRadians )
		//	{
		//		// There is no angle limit. Thus all vertices with positions close
		//		// to each other will receive the same vertex normal. This allows us
		//		// to optimize the whole algorithm a little bit ...
		//		var abHad = new bool[ vertices.Length ];
		//		for( int i = 0; i < vertices.Length; ++i )
		//		{
		//			if( abHad[ i ] )
		//				continue;

		//			// Get all vertices that share this one ...
		//			var verticesFound = vertexFinder.FindPositions( vertices[ i ].Vertex.Position, posEpsilon );

		//			Vector3F pcNor = new Vector3F();
		//			for( int a = 0; a < verticesFound.Count; ++a )
		//			{
		//				Vector3F v = vertices[ verticesFound[ a ] ].Vertex.Normal;
		//				if( !float.IsNaN( v.X ) )
		//					pcNor += v;
		//			}
		//			if( Vector3F.AnyNonZero( pcNor ) )
		//				pcNor.Normalize();

		//			// Write the smoothed normal back to all affected normals
		//			for( int a = 0; a < verticesFound.Count; ++a )
		//			{
		//				int vidx = verticesFound[ a ];
		//				pcNew[ vidx ] = pcNor;
		//				abHad[ vidx ] = true;
		//			}
		//		}
		//	}
		//	else
		//	{
		//		// Slower code path if a smooth angle is set. There are many ways to achieve
		//		// the effect, this one is the most straightforward one.

		//		float fLimit = MathEx.Cos( maxSmoothingAngleInRadians );
		//		for( int i = 0; i < vertices.Length; ++i )
		//		{
		//			// Get all vertices that share this one ...
		//			var verticesFound = vertexFinder.FindPositions( vertices[ i ].Vertex.Position, posEpsilon );

		//			Vector3F vr = vertices[ i ].Vertex.Normal;
		//			float vrlen = vr.Length();

		//			Vector3F pcNor = new Vector3F();
		//			for( int a = 0; a < verticesFound.Count; ++a )
		//			{
		//				Vector3F v = vertices[ verticesFound[ a ] ].Vertex.Normal;

		//				// check whether the angle between the two normals is not too large
		//				// HACK: if v.x is qnan the dot product will become qnan, too
		//				//   therefore the comparison against fLimit should be false
		//				//   in every case.
		//				if( Vector3F.Dot( v, vr ) >= fLimit * vrlen * v.Length() )
		//					pcNor += v;
		//			}
		//			if( Vector3F.AnyNonZero( pcNor ) )
		//				pcNor.Normalize();
		//			pcNew[ i ] = pcNor;
		//		}
		//	}

		//	for( int i = 0; i < vertices.Length; i++ )
		//		vertices[ i ].Vertex.Normal = pcNew[ i ];
		//}

		/// <summary>
		/// Whether to invert all normals in mesh with infacing normals
		/// </summary>
		/// <param name="vertices"></param>
		public static bool FixInfacingNormals( VertexInfo[] vertices )
		{
			// Compute the bounding box of both the model vertices + normals and
			// the unmodified model vertices. Then check whether the first BB
			// is smaller than the second. In this case we can assume that the
			// normals need to be flipped, although there are a few special cases ..
			// convex, concave, planar models ...

			Vector3F vMin0 = new Vector3F( 1e10f, 1e10f, 1e10f );
			Vector3F vMin1 = new Vector3F( 1e10f, 1e10f, 1e10f );
			Vector3F vMax0 = new Vector3F( -1e10f, -1e10f, -1e10f );
			Vector3F vMax1 = new Vector3F( -1e10f, -1e10f, -1e10f );

			for( int i = 0; i < vertices.Length; ++i )
			{
				ref Vector3F pos = ref vertices[ i ].Vertex.Position;
				vMin1.X = Math.Min( vMin1.X, pos.X );
				vMin1.Y = Math.Min( vMin1.Y, pos.Y );
				vMin1.Z = Math.Min( vMin1.Z, pos.Z );

				vMax1.X = Math.Max( vMax1.X, pos.X );
				vMax1.Y = Math.Max( vMax1.Y, pos.Y );
				vMax1.Z = Math.Max( vMax1.Z, pos.Z );

				Vector3F vWithNormal = pos + vertices[ i ].Vertex.Normal;

				vMin0.X = Math.Min( vMin0.X, vWithNormal.X );
				vMin0.Y = Math.Min( vMin0.Y, vWithNormal.Y );
				vMin0.Z = Math.Min( vMin0.Z, vWithNormal.Z );

				vMax0.X = Math.Max( vMax0.X, vWithNormal.X );
				vMax0.Y = Math.Max( vMax0.Y, vWithNormal.Y );
				vMax0.Z = Math.Max( vMax0.Z, vWithNormal.Z );
			}

			// ReSharper disable InconsistentNaming
			float fDelta0_x = ( vMax0.X - vMin0.X );
			float fDelta0_y = ( vMax0.Y - vMin0.Y );
			float fDelta0_z = ( vMax0.Z - vMin0.Z );

			float fDelta1_x = ( vMax1.X - vMin1.X );
			float fDelta1_y = ( vMax1.Y - vMin1.Y );
			float fDelta1_z = ( vMax1.Z - vMin1.Z );

			// Check whether the boxes are overlapping
			if( ( fDelta0_x > 0.0f ) != ( fDelta1_x > 0.0f ) ) return false;
			if( ( fDelta0_y > 0.0f ) != ( fDelta1_y > 0.0f ) ) return false;
			if( ( fDelta0_z > 0.0f ) != ( fDelta1_z > 0.0f ) ) return false;

			// Check whether this is a planar surface
			float fDelta1_yz = fDelta1_y * fDelta1_z;
			// ReSharper restore InconsistentNaming

			if( fDelta1_x < 0.05f * MathEx.Sqrt( fDelta1_yz ) ) return false;
			if( fDelta1_y < 0.05f * MathEx.Sqrt( fDelta1_z * fDelta1_x ) ) return false;
			if( fDelta1_z < 0.05f * MathEx.Sqrt( fDelta1_y * fDelta1_x ) ) return false;

			// now compare the volumes of the bounding boxes
			if( Math.Abs( fDelta0_x * fDelta0_y * fDelta0_z ) < Math.Abs( fDelta1_x * fDelta1_yz ) )
			{
				FbxImportLog.LogWarning( "Normals are facing inwards (or the mesh is planar)" );

				// Invert normals
				for( int i = 0; i < vertices.Length; ++i )
					vertices[ i ].Vertex.Normal *= -1.0f;

				// ... and flip faces
				foreach( var polygonIndexRange in CalcMiscProcess.EnumeratePolygons( vertices ) )
					FlipFace( vertices, polygonIndexRange );
				return true;
			}
			return false;
		}

		static void FlipFace( VertexInfo[] vertices, RangeI indexRange )
		{
			int halfCount = indexRange.Size / 2;
			for( int i = 0; i < halfCount; i++ )
				FbxMath.Swap( ref vertices[ indexRange.Minimum + i ], ref vertices[ indexRange.Maximum - 1 - i ] );
		}
	}
}
