// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using SharpBgfx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	/// <summary>
	/// A set of mathematical algorithms.
	/// </summary>
	public static class MathAlgorithms
	{
		public delegate void DrawPixelDelegate( Vector2I point );

		public static void Draw2DLine( Vector2I start, Vector2I end, DrawPixelDelegate drawPixelCallback )
		{
			int x1 = start.X;
			int x2 = end.X;
			int y1 = start.Y;
			int y2 = end.Y;

			int dx = ( x2 - x1 >= 0 ) ? 1 : -1;
			int dy = ( y2 - y1 >= 0 ) ? 1 : -1;

			int lengthX = Math.Abs( x2 - x1 );
			int lengthY = Math.Abs( y2 - y1 );

			int length = Math.Max( lengthX, lengthY );

			if( length == 0 )
				drawPixelCallback( new Vector2I( x1, y1 ) );

			if( lengthY <= lengthX )
			{
				int x = x1;
				int y = y1;
				int d = -lengthX;

				length++;
				while( length != 0 )
				{
					length--;

					drawPixelCallback( new Vector2I( x, y ) );
					x += dx;
					d += 2 * lengthY;
					if( d > 0 )
					{
						d -= 2 * lengthX;
						y += dy;
					}
				}
			}
			else
			{
				int x = x1;
				int y = y1;
				int d = -lengthY;

				length++;
				while( length != 0 )
				{
					length--;

					drawPixelCallback( new Vector2I( x, y ) );
					y += dy;
					d += 2 * lengthX;
					if( d > 0 )
					{
						d -= 2 * lengthY;
						x += dx;
					}
				}
			}
		}

		static void RenderEdge( int[] edgeMin, int[] edgeMax, RectangleI clipRectangle, Vector2I start, Vector2I end )
		{
			Draw2DLine( start, end, delegate ( Vector2I point )
			{
				if( point.Y >= clipRectangle.Top && point.Y < clipRectangle.Bottom )
				{
					Vector2I p = point;
					if( p.X < clipRectangle.Left )
						p.X = clipRectangle.Left;
					if( p.X >= clipRectangle.Right )
						p.X = clipRectangle.Right - 1;

					if( p.X < edgeMin[ p.Y ] )
						edgeMin[ p.Y ] = p.X;
					if( p.X > edgeMax[ p.Y ] )
						edgeMax[ p.Y ] = p.X;
				}
			} );
		}

		public static void Fill2DTriangle( Vector2I point0, Vector2I point1, Vector2I point2, RectangleI clipRectangle, DrawPixelDelegate drawPixelCallback )
		{
			if( clipRectangle.Left < 0 )
			{
				throw new NotSupportedException(
					"Geometry2D.FillTriangle: clipRectangle.Left < 0 is not supported." );
			}
			if( clipRectangle.Top < 0 )
			{
				throw new NotSupportedException(
					"Geometry2D.FillTriangle: clipRectangle.Top < 0 is not supported." );
			}

			int minY = Math.Min( Math.Min( point0.Y, point1.Y ), point2.Y );
			int maxY = Math.Max( Math.Max( point0.Y, point1.Y ), point2.Y );
			MathEx.Clamp( ref minY, 0, clipRectangle.Bottom - 1 );
			MathEx.Clamp( ref maxY, 0, clipRectangle.Bottom - 1 );

			int[] edgeMin = new int[ clipRectangle.Bottom + 1 ];
			int[] edgeMax = new int[ clipRectangle.Bottom + 1 ];

			for( int y = minY; y <= maxY; y++ )
			{
				edgeMin[ y ] = int.MaxValue;
				edgeMax[ y ] = int.MinValue;
			}

			RenderEdge( edgeMin, edgeMax, clipRectangle, point0, point1 );
			RenderEdge( edgeMin, edgeMax, clipRectangle, point1, point2 );
			RenderEdge( edgeMin, edgeMax, clipRectangle, point2, point0 );

			for( int y = minY; y <= maxY; y++ )
			{
				int minX = edgeMin[ y ];
				int maxX = edgeMax[ y ];

				if( minX != int.MaxValue )
				{
					for( int x = minX; x <= maxX; x++ )
						drawPixelCallback( new Vector2I( x, y ) );
				}
			}
		}

		public static Vector2F ProjectPointToLine( Vector2F lineStart, Vector2F lineEnd, Vector2F point )
		{
			Vector2F result;
			ProjectPointToLine( ref lineStart, ref lineEnd, ref point, out result );
			return result;
		}

		public static void ProjectPointToLine( ref Vector2F lineStart, ref Vector2F lineEnd, ref Vector2F point,
			out Vector2F result )
		{
			Vector2F d;
			Vector2F.Subtract( ref lineEnd, ref lineStart, out d );
			d.Normalize();
			//Vec2 d = Vec2.Normalize( lineEnd - lineStart );
			float distPoint;
			Vector2F.Dot( ref point, ref d, out distPoint );
			float distVb;
			Vector2F.Dot( ref lineStart, ref d, out distVb );

			result.X = lineStart.X + d.X * ( distPoint - distVb );
			result.Y = lineStart.Y + d.Y * ( distPoint - distVb );
			//result = lineStart + d * ( distPoint - distVb );
		}

		public static Vector3F ProjectPointToLine( Vector3F lineStart, Vector3F lineEnd, Vector3F point )
		{
			Vector3F result;
			ProjectPointToLine( ref lineStart, ref lineEnd, ref point, out result );
			return result;
		}

		public static void ProjectPointToLine( ref Vector3F lineStart, ref Vector3F lineEnd, ref Vector3F point,
			out Vector3F result )
		{
			Vector3F d;
			Vector3F.Subtract( ref lineEnd, ref lineStart, out d );
			d.Normalize();
			//Vec3 d = Vec3.Normalize( lineEnd - lineStart );
			float distPoint;
			Vector3F.Dot( ref point, ref d, out distPoint );
			float distVb;
			Vector3F.Dot( ref lineStart, ref d, out distVb );

			result.X = lineStart.X + d.X * ( distPoint - distVb );
			result.Y = lineStart.Y + d.Y * ( distPoint - distVb );
			result.Z = lineStart.Z + d.Z * ( distPoint - distVb );
			//result = lineStart + d * ( distPoint - distVb );
		}

		public static bool IntersectTriangleRay( ref Vector3F p0, ref Vector3F p1, ref Vector3F p2, ref RayF ray )
		{
			float u, v, t;

			Vector3F edge1;
			Vector3F.Subtract( ref p1, ref p0, out edge1 );
			//Vec3 edge1 = p1 - p0;

			Vector3F edge2;
			Vector3F.Subtract( ref p2, ref p0, out edge2 );
			//Vec3 edge2 = p2 - p0;

			Vector3F pvec;
			Vector3F.Cross( ref ray.Direction, ref edge2, out pvec );
			//Vec3 pvec = Vec3.Cross( ray.Direction, edge2 );

			float det;
			Vector3F.Dot( ref edge1, ref pvec, out det );
			//float det = Vec3.Dot( edge1, pvec );

			if( det < MathEx.Epsilon )
				return false;

			Vector3F tvec;
			Vector3F.Subtract( ref ray.Origin, ref p0, out tvec );
			//Vec3 tvec = ray.Origin - p0;

			Vector3F.Dot( ref tvec, ref pvec, out u );
			//u = Vec3.Dot( tvec, pvec );

			if( u < 0.0 || u > det )
				return false;

			Vector3F qvec;
			Vector3F.Cross( ref tvec, ref edge1, out qvec );
			//Vec3 qvec = Vec3.Cross( tvec, edge1 );

			Vector3F.Dot( ref ray.Direction, ref qvec, out v );
			//v = Vec3.Dot( ray.Direction, qvec );
			if( v < 0.0 || u + v > det )
				return false;

			Vector3F.Dot( ref edge2, ref qvec, out t );
			//t = Vec3.Dot( edge2, qvec );

			t *= det;//important only "minus" or "plus"
					 //float inv_det = 1.0f / det;
					 //t *= inv_det;
					 //u *= inv_det;
					 //v *= inv_det;
			return t >= 0;
		}

		public static bool IntersectTriangleRay( Vector3F p0, Vector3F p1, Vector3F p2, RayF ray )
		{
			return IntersectTriangleRay( ref p0, ref p1, ref p2, ref ray );
		}

		public static bool IntersectTriangleRay( ref Vector3F p0, ref Vector3F p1, ref Vector3F p2, ref RayF ray, out float scale )
		{
			scale = 0;

			float u, v, t;

			Vector3F edge1;
			Vector3F.Subtract( ref p1, ref p0, out edge1 );
			//Vec3 edge1 = p1 - p0;

			Vector3F edge2;
			Vector3F.Subtract( ref p2, ref p0, out edge2 );
			//Vec3 edge2 = p2 - p0;

			Vector3F pvec;
			Vector3F.Cross( ref ray.Direction, ref edge2, out pvec );
			//Vec3 pvec = Vec3.Cross( ray.Direction, edge2 );

			float det;
			Vector3F.Dot( ref edge1, ref pvec, out det );
			//float det = Vec3.Dot( edge1, pvec );

			if( det < MathEx.Epsilon )
				return false;

			Vector3F tvec;
			Vector3F.Subtract( ref ray.Origin, ref p0, out tvec );
			//Vec3 tvec = ray.Origin - p0;

			Vector3F.Dot( ref tvec, ref pvec, out u );
			//u = Vec3.Dot( tvec, pvec );

			if( u < 0.0 || u > det )
				return false;

			Vector3F qvec;
			Vector3F.Cross( ref tvec, ref edge1, out qvec );
			//Vec3 qvec = Vec3.Cross( tvec, edge1 );

			Vector3F.Dot( ref ray.Direction, ref qvec, out v );
			//v = Vec3.Dot( ray.Direction, qvec );
			if( v < 0.0 || u + v > det )
				return false;

			Vector3F.Dot( ref edge2, ref qvec, out t );
			//t = Vec3.Dot( edge2, qvec );

			float inv_det = 1.0f / det;
			t *= inv_det;
			//u *= inv_det;
			//v *= inv_det;
			if( t < 0 )
				return false;

			scale = t;

			return true;
		}

		public static bool IntersectTriangleRay( Vector3F p0, Vector3F p1, Vector3F p2, RayF ray, out float scale )
		{
			return IntersectTriangleRay( ref p0, ref p1, ref p2, ref ray, out scale );
		}

		public static bool IntersectLineLine( Vector2F pt11, Vector2F pt12, Vector2F pt21, Vector2F pt22, out Vector2F intersectPoint )
		{
			float d = ( pt12.Y - pt11.Y ) * ( pt21.X - pt22.X ) -
				( pt21.Y - pt22.Y ) * ( pt12.X - pt11.X );
			if( d == 0.0f )
			{
				intersectPoint = Vector2F.Zero;
				return false;
			}

			float d1 = ( pt12.Y - pt11.Y ) * ( pt21.X - pt11.X ) -
				( pt21.Y - pt11.Y ) * ( pt12.X - pt11.X );
			float d2 = ( pt21.Y - pt11.Y ) * ( pt21.X - pt22.X ) -
				( pt21.Y - pt22.Y ) * ( pt21.X - pt11.X );

			float dInv = 1.0f / d;
			float t1 = d1 * dInv;
			float t2 = d2 * dInv;
			if( !( t1 <= 1 && t1 >= 0 && t2 >= 0 && t2 <= 1 ) )
			{
				intersectPoint = Vector2F.Zero;
				return false;
			}

			intersectPoint.X = pt11.X + ( pt12.X - pt11.X ) * t2;
			intersectPoint.Y = pt11.Y + ( pt12.Y - pt11.Y ) * t2;
			return true;
		}

		public static bool IntersectLineLine( Vector2F pt11, Vector2F pt12, Vector2F pt21, Vector2F pt22,
			out float scale )
		{
			float d = ( pt12.Y - pt11.Y ) * ( pt21.X - pt22.X ) -
				( pt21.Y - pt22.Y ) * ( pt12.X - pt11.X );
			if( d == 0.0f )
			{
				scale = 0;
				return false;
			}

			float d1 = ( pt12.Y - pt11.Y ) * ( pt21.X - pt11.X ) -
				( pt21.Y - pt11.Y ) * ( pt12.X - pt11.X );
			float d2 = ( pt21.Y - pt11.Y ) * ( pt21.X - pt22.X ) -
				( pt21.Y - pt22.Y ) * ( pt21.X - pt11.X );

			float dInv = 1.0f / d;
			float t1 = d1 * dInv;
			float t2 = d2 * dInv;
			if( !( t1 <= 1 && t1 >= 0 && t2 >= 0 && t2 <= 1 ) )
			{
				scale = 0;
				return false;
			}

			scale = t2;
			return true;
		}

		public static bool IntersectRayRay( Vector2F pt11, Vector2F pt12, Vector2F pt21,
			Vector2F pt22, out Vector2F intersectPoint )
		{
			float d = ( pt12.Y - pt11.Y ) * ( pt21.X - pt22.X ) -
				( pt21.Y - pt22.Y ) * ( pt12.X - pt11.X );
			if( d == 0.0f )
			{
				intersectPoint = Vector2F.Zero;
				return false;
			}

			float d1 = ( pt12.Y - pt11.Y ) * ( pt21.X - pt11.X ) -
				( pt21.Y - pt11.Y ) * ( pt12.X - pt11.X );
			float d2 = ( pt21.Y - pt11.Y ) * ( pt21.X - pt22.X ) -
				( pt21.Y - pt22.Y ) * ( pt21.X - pt11.X );

			float dInv = 1.0f / d;
			//float t1 = d1 * dInv;
			float t2 = d2 * dInv;

			intersectPoint.X = pt11.X + ( pt12.X - pt11.X ) * t2;
			intersectPoint.Y = pt11.Y + ( pt12.Y - pt11.Y ) * t2;
			return true;
		}

		public static int IntersectRectangleLine( RectangleF rectangle, Vector2F pt1, Vector2F pt2,
			out Vector2F intersectPoint1, out Vector2F intersectPoint2 )
		{
			intersectPoint1 = Vector2F.Zero;
			intersectPoint2 = Vector2F.Zero;

			int count = 0;
			Vector2F p = Vector2F.Zero;
			if( IntersectLineLine( pt1, pt2, rectangle.LeftTop, rectangle.RightTop, out p ) )
			{
				if( count == 0 ) intersectPoint1 = p; else intersectPoint2 = p;
				count++;
			}
			if( IntersectLineLine( pt1, pt2, rectangle.LeftBottom, rectangle.RightBottom, out p ) )
			{
				if( count == 0 ) intersectPoint1 = p; else intersectPoint2 = p;
				count++;
			}
			if( count >= 2 ) return count;
			if( IntersectLineLine( pt1, pt2, rectangle.LeftTop, rectangle.LeftBottom, out p ) )
			{
				if( count == 0 ) intersectPoint1 = p; else intersectPoint2 = p;
				count++;
			}
			if( count >= 2 ) return count;
			if( IntersectLineLine( pt1, pt2, rectangle.RightTop, rectangle.RightBottom, out p ) )
			{
				if( count == 0 ) intersectPoint1 = p; else intersectPoint2 = p;
				count++;
			}
			return count;
		}

		public static bool IsDegenerateTriangle( ref Vector3F p0, ref Vector3F p1, ref Vector3F p2, float epsilon = float.Epsilon )
		{
			if( p0 == p1 || p0 == p2 || p1 == p2 )
				return true;

			Vector3F v01;
			Vector3F.Subtract( ref p1, ref p0, out v01 );
			//Vec3 v01 = v1 - v0;
			Vector3F v02;
			Vector3F.Subtract( ref p2, ref p0, out v02 );
			//Vec3 v02 = v2 - v0;

			float len = v01.LengthSquared() * v02.LengthSquared();
			if( len == 0.0f )
				return true;

			float lensq = MathEx.Sqrt( len );
			float vcos = ( v01.X * v02.X + v01.Y * v02.Y + v01.Z * v02.Z ) / lensq;
			return Math.Abs( MathEx.Acos( vcos ) ) <= epsilon;// .000001f;
		}

		public static bool IsDegenerateTriangle( Vector3F p0, Vector3F p1, Vector3F p2 )
		{
			return IsDegenerateTriangle( ref p0, ref p1, ref p2 );
		}

		/// <summary>
		/// Original indices array will not changed.
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="indices"></param>
		/// <param name="processedTrianglesToSourceIndex"></param>
		/// <param name="epsilon"></param>
		/// <returns></returns>
		public static bool RemoveCollinearDegenerateTriangles( Vector3F[] vertices, ref int[] indices, out int[] processedTrianglesToSourceIndex, float epsilon = float.Epsilon )
		{
			int[] tempIndices = new int[ indices.Length ];
			int indexCount = 0;

			processedTrianglesToSourceIndex = new int[ indices.Length / 3 ];

			for( int index = 0; index < indices.Length; index += 3 )
			{
				int index0 = indices[ index + 0 ];
				int index1 = indices[ index + 1 ];
				int index2 = indices[ index + 2 ];

				ref var p0 = ref vertices[ index0 ];
				ref var p1 = ref vertices[ index1 ];
				ref var p2 = ref vertices[ index2 ];

				if( !IsCollinearTriangle( ref p0, ref p1, ref p2, epsilon ) && !IsDegenerateTriangle( ref p0, ref p1, ref p2, epsilon ) )
				{
					//map new triangle to old triangle index
					processedTrianglesToSourceIndex[ indexCount / 3 ] = index / 3;

					tempIndices[ indexCount++ ] = index0;
					tempIndices[ indexCount++ ] = index1;
					tempIndices[ indexCount++ ] = index2;
				}
			}

			//nothing changed
			if( indexCount == indices.Length )
			{
				processedTrianglesToSourceIndex = null;
				return false;
			}

			Array.Resize( ref processedTrianglesToSourceIndex, indexCount / 3 );
			Array.Resize( ref tempIndices, indexCount );

			indices = tempIndices;
			return true;
		}

		public static int[] RemoveDegenerateTriangles( Vector3F[] vertices, int[] indices )
		{
			int[] tempIndices = new int[ indices.Length ];
			int indexCount = 0;

			for( int index = 0; index < indices.Length; index += 3 )
			{
				int index0 = indices[ index + 0 ];
				int index1 = indices[ index + 1 ];
				int index2 = indices[ index + 2 ];

				Vector3F p0 = vertices[ index0 ];
				Vector3F p1 = vertices[ index1 ];
				Vector3F p2 = vertices[ index2 ];

				if( !IsDegenerateTriangle( ref p0, ref p1, ref p2 ) )
				{
					tempIndices[ indexCount++ ] = index0;
					tempIndices[ indexCount++ ] = index1;
					tempIndices[ indexCount++ ] = index2;
				}
			}

			int[] result = new int[ indexCount ];
			Array.Copy( tempIndices, 0, result, 0, indexCount );
			return result;
		}

		public static int RemoveDegenerateTriangles( List<Vector3F> vertices, List<int> indices )
		{
			int count = 0;

			for( int index = indices.Count - 3; index >= 0; index -= 3 )
			{
				Vector3F p0 = vertices[ indices[ index + 0 ] ];
				Vector3F p1 = vertices[ indices[ index + 1 ] ];
				Vector3F p2 = vertices[ indices[ index + 2 ] ];

				if( IsDegenerateTriangle( ref p0, ref p1, ref p2 ) )
				{
					count++;

					indices.RemoveAt( index );
					indices.RemoveAt( index );
					indices.RemoveAt( index );
				}
			}

			return count;
		}

		public static RadianF GetVectorsAngle( Vector2F vector1, Vector2F vector2 )
		{
			float cos = Vector2F.Dot( vector1, vector2 ) / ( vector1.Length() * vector2.Length() );
			MathEx.Clamp( ref cos, -1.0f, 1.0f );
			return MathEx.Acos( cos );
		}

		public static RadianF GetVectorsAngle( Vector3F vector1, Vector3F vector2 )
		{
			float cos = Vector3F.Dot( vector1, vector2 ) / ( vector1.Length() * vector2.Length() );
			MathEx.Clamp( ref cos, -1.0f, 1.0f );
			return MathEx.Acos( cos );
		}

		public static Vector3F CalculateTangentSpaceVector( Vector3F position1, Vector2F texCoord1,
			Vector3F position2, Vector2F texCoord2, Vector3F position3, Vector2F texCoord3 )
		{
			Vector3F result;
			CalculateTangentSpaceVector( ref position1, ref texCoord1, ref position2,
				ref texCoord2, ref position3, ref texCoord3, out result );
			return result;
		}

		public static void CalculateTangentSpaceVector( ref Vector3F position1, ref Vector2F texCoord1,
			ref Vector3F position2, ref Vector2F texCoord2, ref Vector3F position3, ref Vector2F texCoord3,
			out Vector3F result )
		{
			//side0 is the vector along one side of the triangle of vertices passed in, 
			//and side1 is the vector along another side. Taking the cross product of these returns the normal.
			Vector3F side0;
			Vector3F.Subtract( ref position1, ref position2, out side0 );
			//Vec3 side0 = position1 - position2;

			Vector3F side1;
			Vector3F.Subtract( ref position3, ref position1, out side1 );
			//Vec3 side1 = position3 - position1;

			//Calculate face normal
			Vector3F normal;
			Vector3F.Cross( ref side1, ref side0, out normal );
			//Vec3 normal = Vec3.Cross( side1, side0 );
			normal.Normalize();

			//Now we use a formula to calculate the tangent. 
			float deltaV0 = texCoord1.Y - texCoord2.Y;//v1 - v2;
			float deltaV1 = texCoord3.Y - texCoord1.Y; //v3 - v1;
			Vector3F tangent = deltaV1 * side0 - deltaV0 * side1;
			tangent.Normalize();
			//Calculate binormal
			float deltaU0 = texCoord1.X - texCoord2.X;// u1 - u2;
			float deltaU1 = texCoord3.X - texCoord1.X; //u3 - u1;
			Vector3F binormal = deltaU1 * side0 - deltaU0 * side1;
			binormal.Normalize();
			//Now, we take the cross product of the tangents to get a vector which 
			//should point in the same direction as our normal calculated above. 
			//If it points in the opposite direction (the dot product between the normals is less than zero), 
			//then we need to reverse the s and t tangents. 
			//This is because the triangle has been mirrored when going from tangent space to object space.
			//reverse tangents if necessary
			Vector3F tangentCross;
			Vector3F.Cross( ref tangent, ref binormal, out tangentCross );
			//Vec3 tangentCross = Vec3.Cross( tangent, binormal );
			if( Vector3F.Dot( tangentCross, normal ) < 0.0f )
			{
				tangent = -tangent;
				binormal = -binormal;
			}

			result = tangent;
		}

		public static int IntersectCircleLine( Vector2F center, float radius, Vector2F point1, Vector2F point2,
			out float outScale1, out float outScale2 )
		{
			outScale1 = 0;
			outScale2 = 0;

			float x01 = point1.X - center.X;
			float y01 = point1.Y - center.Y;
			float x02 = point2.X - center.X;
			float y02 = point2.Y - center.Y;

			float dx = x02 - x01;
			float dy = y02 - y01;

			float a = dx * dx + dy * dy;

			if( a == 0 )
				return 0;

			float b = 2.0f * ( x01 * dx + y01 * dy );
			float c = x01 * x01 + y01 * y01 - radius * radius;

			float d = b * b - 4.0f * a * c;

			if( d >= 0 )
			{
				float sqrtd = MathEx.Sqrt( d );

				float t;
				int count = 0;

				//1

				t = ( -b + sqrtd ) / ( 2.0f * a );
				if( t >= 0 && t <= 1.0f )
				{
					outScale1 = t;
					count++;
				}

				//2

				t = ( -b - sqrtd ) / ( 2.0f * a );

				if( t >= 0 && t <= 1.0f )
				{
					if( count == 1 )
						outScale2 = t;
					else
						outScale1 = t;
					count++;
				}
				return count;
			}
			return 0;
		}

		static void Triangulate( Vector3F[] vertices, float[] a, int[] b )
		{
			for( int k = 2; k < vertices.Length; ++k )
			{
				for( int i1 = vertices.Length - k; --i1 >= 0; )
				{
					int i2 = i1 + k;
					Vector3F v = vertices[ i2 ] - vertices[ i1 ];
					Vector3F u = Vector3F.Cross( vertices[ i2 ], vertices[ i1 ] );

					int im = i1 + 1;
					float s = ( Vector3F.Cross( vertices[ im ], v ) + u ).Length();
					int j = ( i2 - 1 ) * ( i2 - 2 ) / 2 + i1;
					if( k == 2 )
					{
						b[ j ] = im;
						a[ j ] = s;
						continue;
					}
					float sum = s + a[ j + 1 ];
					int ik = i2 - 1;
					for( int i = i1 + 2; i < ik; ++i )
					{
						float ss = a[ ( i - 1 ) * ( i - 2 ) / 2 + i1 ] + a[ j + i - i1 ];
						if( sum <= ss )
							continue;
						ss += ( Vector3F.Cross( vertices[ i ], v ) + u ).Length();
						if( sum > ss )
						{
							sum = ss;
							im = i;
						}
					}
					s = ( Vector3F.Cross( vertices[ ik ], v ) + u ).Length() +
						a[ ( ik - 1 ) * ( ik - 2 ) / 2 + i1 ];
					if( sum > s )
					{
						sum = s; im = ik;
					}
					b[ j ] = im;
					a[ j ] = sum;
				}
			}
		}

		public static int[] TriangulatePolygon( Vector3F[] vertices )
		{
			//slowly

			if( vertices.Length < 3 )
				return new int[ 0 ];
			if( vertices.Length == 3 )
				return new int[] { 0, 1, 2 };

			//slowly
			List<int> result = new List<int>();

			//const int arr_size = 820;
			//float darr[arr_size];
			//int iarr[arr_size+42];
			//float* a = darr;
			//int* b = iarr;
			int n2 = ( vertices.Length - 1 ) * ( vertices.Length - 2 ) / 2;

			float[] a = new float[ n2 ];
			int[] b = new int[ n2 + n2 ];

			//if( n2 > arr_size )
			//{
			//   a = new float[ n2 ];
			//   b = new int[ n2 + n ];
			//}
			Triangulate( vertices, a, b );

			//int k = 0;
			int count = 0;
			//int* c = b + n2;
			b[ n2 + count ] = 0;
			count++;
			b[ n2 + count ] = vertices.Length - 1;
			count++;
			while( count > 0 )
			{
				count--;
				int j2 = b[ n2 + count ];
				count--;
				int j1 = b[ n2 + count ];
				int j = b[ ( j2 - 1 ) * ( j2 - 2 ) / 2 + j1 ];

				result.Add( j1 ); result.Add( j ); result.Add( j2 );
				//res[ k++ ] = Triplet<int>( j1, j, j2 );

				if( j2 - j > 1 )
				{
					b[ n2 + count ] = j; count++;
					b[ n2 + count ] = j2; count++;
				}
				if( j - j1 > 1 )
				{
					b[ n2 + count ] = j1; count++;
					b[ n2 + count ] = j; count++;
				}
			}

			return result.ToArray();
		}

		public static Vector2 ProjectPointToLine( Vector2 lineStart, Vector2 lineEnd, Vector2 point )
		{
			Vector2 result;
			ProjectPointToLine( ref lineStart, ref lineEnd, ref point, out result );
			return result;
		}

		public static void ProjectPointToLine( ref Vector2 lineStart, ref Vector2 lineEnd, ref Vector2 point,
			out Vector2 result )
		{
			Vector2 d;
			Vector2.Subtract( ref lineEnd, ref lineStart, out d );
			d.Normalize();
			//Vec2D d = Vec2D.Normalize( lineEnd - lineStart );
			double distPoint;
			Vector2.Dot( ref point, ref d, out distPoint );
			double distVb;
			Vector2.Dot( ref lineStart, ref d, out distVb );

			result.X = lineStart.X + d.X * ( distPoint - distVb );
			result.Y = lineStart.Y + d.Y * ( distPoint - distVb );
			//result = lineStart + d * ( distPoint - distVb );
		}

		public static Vector3 ProjectPointToLine( Vector3 lineStart, Vector3 lineEnd, Vector3 point )
		{
			Vector3 result;
			ProjectPointToLine( ref lineStart, ref lineEnd, ref point, out result );
			return result;
		}

		public static void ProjectPointToLine( ref Vector3 lineStart, ref Vector3 lineEnd, ref Vector3 point,
			out Vector3 result )
		{
			Vector3 d;
			Vector3.Subtract( ref lineEnd, ref lineStart, out d );
			d.Normalize();
			//Vec3D d = Vec3D.Normalize( lineEnd - lineStart );
			double distPoint;
			Vector3.Dot( ref point, ref d, out distPoint );
			double distVb;
			Vector3.Dot( ref lineStart, ref d, out distVb );

			result.X = lineStart.X + d.X * ( distPoint - distVb );
			result.Y = lineStart.Y + d.Y * ( distPoint - distVb );
			result.Z = lineStart.Z + d.Z * ( distPoint - distVb );
			//result = lineStart + d * ( distPoint - distVb );
		}

		public static bool IntersectTriangleRay( ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Ray ray )
		{
			double u, v, t;

			Vector3 edge1;
			Vector3.Subtract( ref p1, ref p0, out edge1 );
			//Vec3D edge1 = p1 - p0;

			Vector3 edge2;
			Vector3.Subtract( ref p2, ref p0, out edge2 );
			//Vec3D edge2 = p2 - p0;

			Vector3 pvec;
			Vector3.Cross( ref ray.Direction, ref edge2, out pvec );
			//Vec3D pvec = Vec3D.Cross( ray.Direction, edge2 );

			double det;
			Vector3.Dot( ref edge1, ref pvec, out det );
			//double det = Vec3D.Dot( edge1, pvec );

			if( det < MathEx.Epsilon )
				return false;

			Vector3 tvec;
			Vector3.Subtract( ref ray.Origin, ref p0, out tvec );
			//Vec3D tvec = ray.Origin - p0;

			Vector3.Dot( ref tvec, ref pvec, out u );
			//u = Vec3D.Dot( tvec, pvec );

			if( u < 0.0 || u > det )
				return false;

			Vector3 qvec;
			Vector3.Cross( ref tvec, ref edge1, out qvec );
			//Vec3D qvec = Vec3D.Cross( tvec, edge1 );

			Vector3.Dot( ref ray.Direction, ref qvec, out v );
			//v = Vec3D.Dot( ray.Direction, qvec );
			if( v < 0.0 || u + v > det )
				return false;

			Vector3.Dot( ref edge2, ref qvec, out t );
			//t = Vec3D.Dot( edge2, qvec );

			t *= det;//important only "minus" or "plus"
					 //double inv_det = 1.0 / det;
					 //t *= inv_det;
					 //u *= inv_det;
					 //v *= inv_det;
			return t >= 0;
		}

		public static bool IntersectTriangleRay( Vector3 p0, Vector3 p1, Vector3 p2, Ray ray )
		{
			return IntersectTriangleRay( ref p0, ref p1, ref p2, ref ray );
		}

		public static bool IntersectTriangleRay( ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Ray ray, out double scale )
		{
			scale = 0;

			double u, v, t;

			Vector3 edge1;
			Vector3.Subtract( ref p1, ref p0, out edge1 );
			//Vec3D edge1 = p1 - p0;

			Vector3 edge2;
			Vector3.Subtract( ref p2, ref p0, out edge2 );
			//Vec3D edge2 = p2 - p0;

			Vector3 pvec;
			Vector3.Cross( ref ray.Direction, ref edge2, out pvec );
			//Vec3D pvec = Vec3D.Cross( ray.Direction, edge2 );

			double det;
			Vector3.Dot( ref edge1, ref pvec, out det );
			//double det = Vec3D.Dot( edge1, pvec );

			if( det < MathEx.Epsilon )
				return false;

			Vector3 tvec;
			Vector3.Subtract( ref ray.Origin, ref p0, out tvec );
			//Vec3D tvec = ray.Origin - p0;

			Vector3.Dot( ref tvec, ref pvec, out u );
			//u = Vec3D.Dot( tvec, pvec );

			if( u < 0.0 || u > det )
				return false;

			Vector3 qvec;
			Vector3.Cross( ref tvec, ref edge1, out qvec );
			//Vec3D qvec = Vec3D.Cross( tvec, edge1 );

			Vector3.Dot( ref ray.Direction, ref qvec, out v );
			//v = Vec3D.Dot( ray.Direction, qvec );
			if( v < 0.0 || u + v > det )
				return false;

			Vector3.Dot( ref edge2, ref qvec, out t );
			//t = Vec3D.Dot( edge2, qvec );

			double inv_det = 1.0 / det;
			t *= inv_det;
			//u *= inv_det;
			//v *= inv_det;
			if( t < 0 )
				return false;

			scale = t;

			return true;
		}

		public static bool IntersectTriangleRay( Vector3 p0, Vector3 p1, Vector3 p2, Ray ray, out double scale )
		{
			return IntersectTriangleRay( ref p0, ref p1, ref p2, ref ray, out scale );
		}

		public static bool IntersectLineLine( Vector2 pt11, Vector2 pt12, Vector2 pt21,
		Vector2 pt22, out Vector2 intersectPoint )
		{
			double d = ( pt12.Y - pt11.Y ) * ( pt21.X - pt22.X ) -
				( pt21.Y - pt22.Y ) * ( pt12.X - pt11.X );
			if( d == 0.0 )
			{
				intersectPoint = Vector2.Zero;
				return false;
			}

			double d1 = ( pt12.Y - pt11.Y ) * ( pt21.X - pt11.X ) -
				( pt21.Y - pt11.Y ) * ( pt12.X - pt11.X );
			double d2 = ( pt21.Y - pt11.Y ) * ( pt21.X - pt22.X ) -
				( pt21.Y - pt22.Y ) * ( pt21.X - pt11.X );

			double dInv = 1.0 / d;
			double t1 = d1 * dInv;
			double t2 = d2 * dInv;
			if( !( t1 <= 1 && t1 >= 0 && t2 >= 0 && t2 <= 1 ) )
			{
				intersectPoint = Vector2.Zero;
				return false;
			}

			intersectPoint.X = pt11.X + ( pt12.X - pt11.X ) * t2;
			intersectPoint.Y = pt11.Y + ( pt12.Y - pt11.Y ) * t2;
			return true;
		}

		public static bool IntersectLineLine( Vector2 pt11, Vector2 pt12, Vector2 pt21, Vector2 pt22,
			out double scale )
		{
			double d = ( pt12.Y - pt11.Y ) * ( pt21.X - pt22.X ) -
				( pt21.Y - pt22.Y ) * ( pt12.X - pt11.X );
			if( d == 0.0 )
			{
				scale = 0;
				return false;
			}

			double d1 = ( pt12.Y - pt11.Y ) * ( pt21.X - pt11.X ) -
				( pt21.Y - pt11.Y ) * ( pt12.X - pt11.X );
			double d2 = ( pt21.Y - pt11.Y ) * ( pt21.X - pt22.X ) -
				( pt21.Y - pt22.Y ) * ( pt21.X - pt11.X );

			double dInv = 1.0 / d;
			double t1 = d1 * dInv;
			double t2 = d2 * dInv;
			if( !( t1 <= 1 && t1 >= 0 && t2 >= 0 && t2 <= 1 ) )
			{
				scale = 0;
				return false;
			}

			scale = t2;
			return true;
		}

		public static bool IntersectRayRay( Vector2 pt11, Vector2 pt12, Vector2 pt21,
			Vector2 pt22, out Vector2 intersectPoint )
		{
			double d = ( pt12.Y - pt11.Y ) * ( pt21.X - pt22.X ) -
				( pt21.Y - pt22.Y ) * ( pt12.X - pt11.X );
			if( d == 0.0 )
			{
				intersectPoint = Vector2.Zero;
				return false;
			}

			double d1 = ( pt12.Y - pt11.Y ) * ( pt21.X - pt11.X ) -
				( pt21.Y - pt11.Y ) * ( pt12.X - pt11.X );
			double d2 = ( pt21.Y - pt11.Y ) * ( pt21.X - pt22.X ) -
				( pt21.Y - pt22.Y ) * ( pt21.X - pt11.X );

			double dInv = 1.0 / d;
			//double t1 = d1 * dInv;
			double t2 = d2 * dInv;

			intersectPoint.X = pt11.X + ( pt12.X - pt11.X ) * t2;
			intersectPoint.Y = pt11.Y + ( pt12.Y - pt11.Y ) * t2;
			return true;
		}

		public static int IntersectRectangleLine( Rectangle rectangle, Vector2 pt1, Vector2 pt2,
			out Vector2 intersectPoint1, out Vector2 intersectPoint2 )
		{
			intersectPoint1 = Vector2.Zero;
			intersectPoint2 = Vector2.Zero;

			int count = 0;
			Vector2 p = Vector2.Zero;
			if( IntersectLineLine( pt1, pt2, rectangle.LeftTop, rectangle.RightTop, out p ) )
			{
				if( count == 0 ) intersectPoint1 = p; else intersectPoint2 = p;
				count++;
			}
			if( IntersectLineLine( pt1, pt2, rectangle.LeftBottom, rectangle.RightBottom, out p ) )
			{
				if( count == 0 ) intersectPoint1 = p; else intersectPoint2 = p;
				count++;
			}
			if( count >= 2 ) return count;
			if( IntersectLineLine( pt1, pt2, rectangle.LeftTop, rectangle.LeftBottom, out p ) )
			{
				if( count == 0 ) intersectPoint1 = p; else intersectPoint2 = p;
				count++;
			}
			if( count >= 2 ) return count;
			if( IntersectLineLine( pt1, pt2, rectangle.RightTop, rectangle.RightBottom, out p ) )
			{
				if( count == 0 ) intersectPoint1 = p; else intersectPoint2 = p;
				count++;
			}
			return count;
		}

		public static bool IsDegenerateTriangle( Vector3 p0, Vector3 p1, Vector3 p2, double epsilon = double.Epsilon )
		{
			return IsDegenerateTriangle( ref p0, ref p1, ref p2 );
		}

		public static bool IsDegenerateTriangle( ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, double epsilon = double.Epsilon )
		{
			if( p0 == p1 || p0 == p2 || p1 == p2 )
				return true;

			Vector3 v01;
			Vector3.Subtract( ref p1, ref p0, out v01 );
			//Vec3D v01 = v1 - v0;
			Vector3 v02;
			Vector3.Subtract( ref p2, ref p0, out v02 );
			//Vec3D v02 = v2 - v0;

			double len = v01.LengthSquared() * v02.LengthSquared();
			if( len == 0.0 )
				return true;

			double lensq = Math.Sqrt( len );
			double vcos = ( v01.X * v02.X + v01.Y * v02.Y + v01.Z * v02.Z ) / lensq;
			return Math.Abs( Math.Acos( vcos ) ) <= epsilon;// .000001;
		}

		public static int[] RemoveDegenerateTriangles( Vector3[] vertices, int[] indices )
		{
			int[] tempIndices = new int[ indices.Length ];
			int indexCount = 0;

			for( int index = 0; index < indices.Length; index += 3 )
			{
				int index0 = indices[ index + 0 ];
				int index1 = indices[ index + 1 ];
				int index2 = indices[ index + 2 ];

				Vector3 p0 = vertices[ index0 ];
				Vector3 p1 = vertices[ index1 ];
				Vector3 p2 = vertices[ index2 ];

				if( !IsDegenerateTriangle( ref p0, ref p1, ref p2 ) )
				{
					tempIndices[ indexCount++ ] = index0;
					tempIndices[ indexCount++ ] = index1;
					tempIndices[ indexCount++ ] = index2;
				}
			}

			int[] result = new int[ indexCount ];
			Array.Copy( tempIndices, 0, result, 0, indexCount );
			return result;
		}

		public static int RemoveDegenerateTriangles( List<Vector3> vertices, List<int> indices )
		{
			int count = 0;

			for( int index = indices.Count - 3; index >= 0; index -= 3 )
			{
				Vector3 p0 = vertices[ indices[ index + 0 ] ];
				Vector3 p1 = vertices[ indices[ index + 1 ] ];
				Vector3 p2 = vertices[ indices[ index + 2 ] ];

				if( IsDegenerateTriangle( ref p0, ref p1, ref p2 ) )
				{
					count++;

					indices.RemoveAt( index );
					indices.RemoveAt( index );
					indices.RemoveAt( index );
				}
			}

			return count;
		}

		public static Radian GetVectorsAngle( Vector2 vector1, Vector2 vector2 )
		{
			double cos = Vector2.Dot( vector1, vector2 ) / ( vector1.Length() * vector2.Length() );
			MathEx.Clamp( ref cos, -1.0, 1.0 );
			return Math.Acos( cos );
		}

		public static Radian GetVectorsAngle( Vector3 vector1, Vector3 vector2 )
		{
			double cos = Vector3.Dot( vector1, vector2 ) / ( vector1.Length() * vector2.Length() );
			MathEx.Clamp( ref cos, -1.0, 1.0 );
			return Math.Acos( cos );
		}

		public static Vector3 CalculateTangentSpaceVector( Vector3 position1, Vector2 texCoord1,
			Vector3 position2, Vector2 texCoord2, Vector3 position3, Vector2 texCoord3 )
		{
			Vector3 result;
			CalculateTangentSpaceVector( ref position1, ref texCoord1, ref position2,
				ref texCoord2, ref position3, ref texCoord3, out result );
			return result;
		}

		public static void CalculateTangentSpaceVector( ref Vector3 position1, ref Vector2 texCoord1,
			ref Vector3 position2, ref Vector2 texCoord2, ref Vector3 position3, ref Vector2 texCoord3,
			out Vector3 result )
		{
			//side0 is the vector along one side of the triangle of vertices passed in, 
			//and side1 is the vector along another side. Taking the cross product of these returns the normal.
			Vector3 side0;
			Vector3.Subtract( ref position1, ref position2, out side0 );
			//Vec3D side0 = position1 - position2;

			Vector3 side1;
			Vector3.Subtract( ref position3, ref position1, out side1 );
			//Vec3D side1 = position3 - position1;

			//Calculate face normal
			Vector3 normal;
			Vector3.Cross( ref side1, ref side0, out normal );
			//Vec3D normal = Vec3D.Cross( side1, side0 );
			normal.Normalize();

			//Now we use a formula to calculate the tangent. 
			double deltaV0 = texCoord1.Y - texCoord2.Y;//v1 - v2;
			double deltaV1 = texCoord3.Y - texCoord1.Y; //v3 - v1;
			Vector3 tangent = deltaV1 * side0 - deltaV0 * side1;
			tangent.Normalize();
			//Calculate binormal
			double deltaU0 = texCoord1.X - texCoord2.X;// u1 - u2;
			double deltaU1 = texCoord3.X - texCoord1.X; //u3 - u1;
			Vector3 binormal = deltaU1 * side0 - deltaU0 * side1;
			binormal.Normalize();
			//Now, we take the cross product of the tangents to get a vector which 
			//should point in the same direction as our normal calculated above. 
			//If it points in the opposite direction (the dot product between the normals is less than zero), 
			//then we need to reverse the s and t tangents. 
			//This is because the triangle has been mirrored when going from tangent space to object space.
			//reverse tangents if necessary
			Vector3 tangentCross;
			Vector3.Cross( ref tangent, ref binormal, out tangentCross );
			//Vec3D tangentCross = Vec3D.Cross( tangent, binormal );
			if( Vector3.Dot( tangentCross, normal ) < 0.0 )
			{
				tangent = -tangent;
				binormal = -binormal;
			}

			result = tangent;
		}

		public static int IntersectCircleLine( Vector2 center, double radius, Vector2 point1, Vector2 point2,
			out double outScale1, out double outScale2 )
		{
			outScale1 = 0;
			outScale2 = 0;

			double x01 = point1.X - center.X;
			double y01 = point1.Y - center.Y;
			double x02 = point2.X - center.X;
			double y02 = point2.Y - center.Y;

			double dx = x02 - x01;
			double dy = y02 - y01;

			double a = dx * dx + dy * dy;

			if( a == 0 )
				return 0;

			double b = 2.0 * ( x01 * dx + y01 * dy );
			double c = x01 * x01 + y01 * y01 - radius * radius;

			double d = b * b - 4.0 * a * c;

			if( d >= 0 )
			{
				double sqrtd = Math.Sqrt( d );

				double t;
				int count = 0;

				//1

				t = ( -b + sqrtd ) / ( 2.0 * a );
				if( t >= 0 && t <= 1.0 )
				{
					outScale1 = t;
					count++;
				}

				//2

				t = ( -b - sqrtd ) / ( 2.0 * a );

				if( t >= 0 && t <= 1.0 )
				{
					if( count == 1 )
						outScale2 = t;
					else
						outScale1 = t;
					count++;
				}
				return count;
			}
			return 0;
		}

		static void Triangulate( Vector3[] vertices, double[] a, int[] b )
		{
			for( int k = 2; k < vertices.Length; ++k )
			{
				for( int i1 = vertices.Length - k; --i1 >= 0; )
				{
					int i2 = i1 + k;
					Vector3 v = vertices[ i2 ] - vertices[ i1 ];
					Vector3 u = Vector3.Cross( vertices[ i2 ], vertices[ i1 ] );

					int im = i1 + 1;
					double s = ( Vector3.Cross( vertices[ im ], v ) + u ).Length();
					int j = ( i2 - 1 ) * ( i2 - 2 ) / 2 + i1;
					if( k == 2 )
					{
						b[ j ] = im;
						a[ j ] = s;
						continue;
					}
					double sum = s + a[ j + 1 ];
					int ik = i2 - 1;
					for( int i = i1 + 2; i < ik; ++i )
					{
						double ss = a[ ( i - 1 ) * ( i - 2 ) / 2 + i1 ] + a[ j + i - i1 ];
						if( sum <= ss )
							continue;
						ss += ( Vector3.Cross( vertices[ i ], v ) + u ).Length();
						if( sum > ss )
						{
							sum = ss;
							im = i;
						}
					}
					s = ( Vector3.Cross( vertices[ ik ], v ) + u ).Length() +
						a[ ( ik - 1 ) * ( ik - 2 ) / 2 + i1 ];
					if( sum > s )
					{
						sum = s; im = ik;
					}
					b[ j ] = im;
					a[ j ] = sum;
				}
			}
		}

		public static int[] TriangulatePolygon( Vector3[] vertices )
		{
			//slowly

			if( vertices.Length < 3 )
				return new int[ 0 ];
			if( vertices.Length == 3 )
				return new int[] { 0, 1, 2 };

			//slowly
			List<int> result = new List<int>();

			//const int arr_size = 820;
			//double darr[arr_size];
			//int iarr[arr_size+42];
			//double* a = darr;
			//int* b = iarr;
			int n2 = ( vertices.Length - 1 ) * ( vertices.Length - 2 ) / 2;

			double[] a = new double[ n2 ];
			int[] b = new int[ n2 + n2 ];

			//if( n2 > arr_size )
			//{
			//   a = new double[ n2 ];
			//   b = new int[ n2 + n ];
			//}
			Triangulate( vertices, a, b );

			//int k = 0;
			int count = 0;
			//int* c = b + n2;
			b[ n2 + count ] = 0;
			count++;
			b[ n2 + count ] = vertices.Length - 1;
			count++;
			while( count > 0 )
			{
				count--;
				int j2 = b[ n2 + count ];
				count--;
				int j1 = b[ n2 + count ];
				int j = b[ ( j2 - 1 ) * ( j2 - 2 ) / 2 + j1 ];

				result.Add( j1 ); result.Add( j ); result.Add( j2 );
				//res[ k++ ] = Triplet<int>( j1, j, j2 );

				if( j2 - j > 1 )
				{
					b[ n2 + count ] = j; count++;
					b[ n2 + count ] = j2; count++;
				}
				if( j - j1 > 1 )
				{
					b[ n2 + count ] = j1; count++;
					b[ n2 + count ] = j; count++;
				}
			}

			return result.ToArray();
		}

		public static bool CheckPointInsideEllipse( Rectangle ellipse, Vector2 point )
		{
			if( !ellipse.Contains( point ) )
				return false;

			var center = ellipse.GetCenter();
			var size = ellipse.GetSize();
			double xRadius = size.X * .5;
			double yRadius = size.Y * .5;
			var normalized = point - center;
			return ( ( normalized.X * normalized.X ) / ( xRadius * xRadius ) ) + ( ( normalized.Y * normalized.Y ) / ( yRadius * yRadius ) ) <= 1.0;
		}

		public static Sphere BoundingSphereFromPoints( Vector3[] points )
		{
			//!!!!can be better

			Bounds bounds = Bounds.Cleared;
			foreach( var p in points )
				bounds.Add( p );

			var center = bounds.GetCenter();
			double radiusSquared = 0;

			foreach( var p in points )
			{
				var r = ( p - center ).LengthSquared();
				if( r > radiusSquared )
					radiusSquared = r;
			}

			return new Sphere( center, Math.Sqrt( radiusSquared ) );
		}

		public static SphereF BoundingSphereFromPoints( Vector3F[] points )
		{
			//!!!!can be better

			BoundsF bounds = BoundsF.Cleared;
			foreach( var p in points )
				bounds.Add( p );

			var center = bounds.GetCenter();
			float radiusSquared = 0;
			foreach( var p in points )
			{
				var r = ( p - center ).LengthSquared();
				if( r > radiusSquared )
					radiusSquared = r;
			}

			return new SphereF( center, MathEx.Sqrt( radiusSquared ) );
		}

		public static int[] TriangleListToLineList( int[] indices )
		{
			RenderingSystem.NativeDLLsPreload();

			unsafe
			{
				int destInBytes = indices.Length * 2 * sizeof( uint );
				int* dest = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, destInBytes );

				int destCount;
				fixed ( int* pIndices = indices )
				{
					destCount = (int)Bgfx.TopologyConvert( TopologyConvert.TriListToLineList, (IntPtr)dest, (uint)destInBytes, (IntPtr)pIndices, (uint)indices.Length, true );
				}

				var result = new int[ destCount ];
				Marshal.Copy( (IntPtr)dest, result, 0, destCount );

				NativeUtility.Free( dest );

				return result;
			}
		}

		public static bool IsMeshConvex( Vector3[] vertices, int[] indices, double epsilon = double.Epsilon )
		{
			for( int i = 0; i < indices.Length; i += 3 )
			{
				var nVertex0 = indices[ i ];
				var nVertex1 = indices[ i + 1 ];
				var nVertex2 = indices[ i + 2 ];

				Plane.FromPoints( ref vertices[ nVertex0 ], ref vertices[ nVertex1 ], ref vertices[ nVertex2 ], out Plane plane );
				if( IsPlaneSplitsVertices( vertices, plane, epsilon ) )
					return false;
			}

			return true;
		}

		public static bool IsMeshConvex( Vector3F[] vertices, int[] indices, float epsilon = float.Epsilon )
		{
			for( int i = 0; i < indices.Length; i += 3 )
			{
				var nVertex0 = indices[ i ];
				var nVertex1 = indices[ i + 1 ];
				var nVertex2 = indices[ i + 2 ];

				PlaneF.FromPoints( ref vertices[ nVertex0 ], ref vertices[ nVertex1 ], ref vertices[ nVertex2 ], out PlaneF plane );
				if( IsPlaneSplitsVertices( vertices, plane, epsilon ) )
					return false;
			}

			return true;
		}

		public static bool IsVertexInsideConvexHull( Plane[] convexPlanes, Vector3 position, double epsilon = double.Epsilon )
		{
			for( int i = 0; i < convexPlanes.Length; i++ )
				if( convexPlanes[ i ].GetSide( ref position, epsilon ) == Plane.Side.Positive )
					return false;

			return true;
		}

		public static bool IsPlaneSplitsVertices( Vector3[] vertices, Plane plane, double epsilon = double.Epsilon )
		{
			var side = plane.GetSide( ref vertices[ 0 ], epsilon );

			for( int i = 1; i < vertices.Length; i++ )
			{
				var side1 = plane.GetSide( ref vertices[ i ], epsilon );

				if( side1 == Plane.Side.No )
					continue;

				if( side == Plane.Side.No )
				{
					side = side1;
					continue;
				}

				if( side1 == side )
					continue;

				return true;
			}

			return false;
		}

		public static bool IsPlaneSplitsVertices( Vector3F[] vertices, PlaneF plane, float epsilon = float.Epsilon )
		{
			var side = plane.GetSide( ref vertices[ 0 ], epsilon );

			for( int i = 1; i < vertices.Length; i++ )
			{
				var side1 = plane.GetSide( ref vertices[ i ], epsilon );

				if( side1 == PlaneF.Side.No )
					continue;

				if( side == PlaneF.Side.No )
				{
					side = side1;
					continue;
				}

				if( side1 == side )
					continue;

				return true;
			}

			return false;
		}

		public static bool IsPlaneMesh( Vector3[] vertices, int[] indices, double epsilon = double.Epsilon )
		{
			Plane plane = Plane.Zero;

			for( int i = 0; i < indices.Length; i += 3 )
			{
				ref var v0 = ref vertices[ indices[ i + 0 ] ];
				ref var v1 = ref vertices[ indices[ i + 1 ] ];
				ref var v2 = ref vertices[ indices[ i + 2 ] ];

				if( IsDegenerateTriangle( ref v0, ref v1, ref v2, epsilon ) )
					continue;
				if( IsCollinearTriangle( ref v0, ref v1, ref v2, epsilon ) )
					continue;

				Plane.FromPoints( ref v0, ref v1, ref v2, out plane );
				break;
			}

			if( plane == Plane.Zero )
				return false;

			return IsVerticesOnPlane( vertices, plane, epsilon );
		}

		public static bool IsPlaneMesh( Vector3F[] vertices, int[] indices, float epsilon = float.Epsilon )
		{
			PlaneF plane = PlaneF.Zero;

			for( int i = 0; i < indices.Length; i += 3 )
			{
				ref var v0 = ref vertices[ indices[ i + 0 ] ];
				ref var v1 = ref vertices[ indices[ i + 1 ] ];
				ref var v2 = ref vertices[ indices[ i + 2 ] ];

				if( IsDegenerateTriangle( ref v0, ref v1, ref v2, epsilon ) )
					continue;
				if( IsCollinearTriangle( ref v0, ref v1, ref v2, epsilon ) )
					continue;

				PlaneF.FromPoints( ref v0, ref v1, ref v2, out plane );
				break;
			}

			if( plane == PlaneF.Zero )
				return false;

			return IsVerticesOnPlane( vertices, plane, epsilon );
		}

		public static bool IsVerticesOnPlane( Vector3[] vertices, Plane plane, double epsilon = double.Epsilon )
		{
			for( int i = 0; i < vertices.Length; i++ )
			{
				var side1 = plane.GetSide( ref vertices[ i ], epsilon );
				if( side1 != Plane.Side.No )
					return false;
			}
			return true;
		}

		public static bool IsVerticesOnPlane( Vector3F[] vertices, PlaneF plane, float epsilon = float.Epsilon )
		{
			for( int i = 0; i < vertices.Length; i++ )
			{
				var side1 = plane.GetSide( ref vertices[ i ], epsilon );
				if( side1 != PlaneF.Side.No )
					return false;
			}
			return true;
		}

		public static bool IsCollinearTriangle( ref Vector3 v1, ref Vector3 v2, ref Vector3 v3, double epsilon )
		{
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

		public static bool IsCollinearTriangle( Vector3 v1, Vector3 v2, Vector3 v3, double epsilon )
		{
			return IsCollinearTriangle( ref v1, ref v2, ref v3, epsilon );
		}

		public static bool IsCollinearTriangle( ref Vector3F v1, ref Vector3F v2, ref Vector3F v3, float epsilon )
		{
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

		/// <summary>
		/// Original arrays will not changed.
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="indices"></param>
		/// <param name="epsilon"></param>
		public static void MergeEqualVertices( ref Vector3[] vertices, ref int[] indices, double epsilon )
		{
			//!!!!slowly. есть метод, где по одной оси сравнивается сначала
			//!!!!можно еще если мало индексов просто пробежать

			if( vertices.Length == 0 || indices.Length == 0 )
				return;

			var found = false;
			var newVertices = new List<Vector3>( vertices.Length );
			var newIndices = new List<int>( indices.Length );

			var bounds = Bounds.Cleared;
			foreach( var vertex in vertices )
				bounds.Add( vertex );
			bounds.Expand( epsilon * 2 );

			var initSettings = new OctreeContainer.InitSettings();
			initSettings.InitialOctreeBounds = bounds;
			initSettings.OctreeBoundsRebuildExpand = Vector3.Zero;
			initSettings.MinNodeSize = bounds.GetSize() / 50;
			var octreeContainer = new OctreeContainer( initSettings );

			foreach( var index in indices )
			{
				var p = vertices[ index ];
				var b = new Bounds( p - new Vector3( epsilon, epsilon, epsilon ), p + new Vector3( epsilon, epsilon, epsilon ) );

				int newIndex;

				//!!!!check by Vec3 position
				int[] result = octreeContainer.GetObjects( b, 0xFFFFFFFF, OctreeContainer.ModeEnum.All );
				if( result.Length != 0 )
				{
					found = true;
					newIndex = result[ 0 ];
				}
				else
				{
					newIndex = newVertices.Count;
					octreeContainer.AddObject( b, 1 );
					newVertices.Add( p );
				}

				newIndices.Add( newIndex );
			}

			octreeContainer.Dispose();

			if( found )
			{
				vertices = newVertices.ToArray();
				indices = newIndices.ToArray();
			}
		}

		/// <summary>
		/// Original arrays will not changed.
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="indices"></param>
		/// <param name="epsilon"></param>
		public static void MergeEqualVertices( ref Vector3F[] vertices, ref int[] indices, float epsilon )
		{
			//!!!!slowly. есть метод, где по одной оси сравнивается сначала

			if( vertices.Length == 0 || indices.Length == 0 )
				return;

			var found = false;
			var newVertices = new List<Vector3F>( vertices.Length );
			var newIndices = new List<int>( indices.Length );

			var bounds = Bounds.Cleared;
			foreach( var vertex in vertices )
				bounds.Add( vertex );
			bounds.Expand( epsilon * 2 );

			var initSettings = new OctreeContainer.InitSettings();
			initSettings.InitialOctreeBounds = bounds;
			initSettings.OctreeBoundsRebuildExpand = Vector3.Zero;
			initSettings.MinNodeSize = bounds.GetSize() / 50;
			var octreeContainer = new OctreeContainer( initSettings );

			foreach( var index in indices )
			{
				var p = vertices[ index ];
				var b = new Bounds( p - new Vector3F( epsilon, epsilon, epsilon ), p + new Vector3F( epsilon, epsilon, epsilon ) );

				int newIndex;

				//!!!!check by Vec3 position
				int[] result = octreeContainer.GetObjects( b, 0xFFFFFFFF, OctreeContainer.ModeEnum.All );
				if( result.Length != 0 )
				{
					found = true;
					newIndex = result[ 0 ];
				}
				else
				{
					newIndex = newVertices.Count;
					octreeContainer.AddObject( b, 1 );
					newVertices.Add( p );
				}

				newIndices.Add( newIndex );
			}

			octreeContainer.Dispose();

			if( found )
			{
				vertices = newVertices.ToArray();
				indices = newIndices.ToArray();
			}
		}

		//public static void MergeEqualVertices( ref Vec3[] vertices, double epsilon )
		//{
		//	if( vertices.Length < 2 )
		//		return;

		//	var found = false;
		//	var newVertices = new List<Vec3>( vertices.Length );

		//	var bounds = Bounds.Cleared;
		//	foreach( var vertex in vertices )
		//		bounds.Add( vertex );
		//	bounds.Expand( epsilon * 2 );

		//	var initSettings = new OctreeContainer.InitSettings();
		//	initSettings.InitialOctreeBounds = bounds;
		//	initSettings.OctreeBoundsRebuildExpand = Vec3.Zero;
		//	initSettings.MinNodeSize = bounds.GetSize() / 50;
		//	var octreeContainer = new OctreeContainer( initSettings );

		//	foreach( var p in vertices )
		//	{
		//		var b = new Bounds( p - new Vec3( epsilon, epsilon, epsilon ), p + new Vec3( epsilon, epsilon, epsilon ) );

		//		//!!!!check by Vec3 position
		//		int[] result = octreeContainer.GetObjects( b, 0xFFFFFFFF );
		//		if( result.Length != 0 )
		//			found = true;
		//		else
		//		{
		//			octreeContainer.AddObject( b, 1 );
		//			newVertices.Add( p );
		//		}
		//	}

		//	octreeContainer.Dispose();

		//	if( found )
		//		vertices = newVertices.ToArray();
		//}

		/// <summary>
		/// Original arrays will not changed.
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="indices"></param>
		/// <returns></returns>
		public static bool RemoveUnusedVertices( ref Vector3F[] vertices, ref int[] indices )
		{
			//check exists unused
			var bits = new bool[ vertices.Length ];
			foreach( var index in indices )
				bits[ index ] = true;
			bool found = false;
			for( int n = 0; n < bits.Length; n++ )
			{
				if( !bits[ n ] )
				{
					found = true;
					break;
				}
			}
			if( !found )
				return false;

			var newVertices = new List<Vector3F>( vertices.Length );
			for( int n = 0; n < vertices.Length; n++ )
				if( bits[ n ] )
					newVertices.Add( vertices[ n ] );

			var newIndices = (int[])indices.Clone();
			for( int nVertex = vertices.Length - 1; nVertex >= 0; nVertex-- )
			{
				if( !bits[ nVertex ] )
				{
					for( int n = 0; n < newIndices.Length; n++ )
					{
						if( newIndices[ n ] > nVertex )
							newIndices[ n ]--;
					}
				}
			}

			vertices = newVertices.ToArray();
			indices = newIndices;
			return true;
		}

		public static bool CheckValidVertexIndexBuffer( int vertexCount, int[] indices, bool fatal )
		{
			foreach( var index in indices )
			{
				if( index < 0 || index >= vertexCount )
				{
					if( fatal )
						Log.Fatal( "MathAlgorithms: CheckValidVertexIndexBuffer: Invalid indices." );
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// If no changes then return same arrays.
		/// </summary>
		/// <param name="sourceVertices"></param>
		/// <param name="sourceIndices"></param>
		/// <param name="epsilon"></param>
		/// <param name="processedVertices"></param>
		/// <param name="processedIndices"></param>
		/// <param name="processedTrianglesToSourceIndex"></param>
		public static void MergeEqualVerticesRemoveInvalidTriangles( Vector3F[] sourceVertices, int[] sourceIndices, float epsilon, out Vector3F[] processedVertices, out int[] processedIndices, out int[] processedTrianglesToSourceIndex )
		{
			var vertices = sourceVertices;
			var indices = sourceIndices;
			int[] trianglesToSourceIndex = null;

			RemoveCollinearDegenerateTriangles( vertices, ref indices, out trianglesToSourceIndex, epsilon );
			RemoveUnusedVertices( ref vertices, ref indices );
			MergeEqualVertices( ref vertices, ref indices, epsilon );

			CheckValidVertexIndexBuffer( vertices.Length, indices, true );

			processedVertices = vertices;
			processedIndices = indices;
			processedTrianglesToSourceIndex = trianglesToSourceIndex;
		}

		public static bool GetTriangleData( int triangleID, Vector3F[] vertices, int[] indices, out Triangle triangle )
		{
			if( vertices == null )
			{
				triangle = new Triangle();
				return false;
			}

			var i1 = triangleID * 3;
			var i2 = i1 + 1;
			var i3 = i1 + 2;
			if( indices != null )
			{
				if( i3 >= indices.Length )
				{
					triangle = new Triangle();
					return false;
				}
				i1 = indices[ i1 ];
				i2 = indices[ i2 ];
				i3 = indices[ i3 ];
			}

			if( i1 >= vertices.Length || i2 >= vertices.Length || i3 >= vertices.Length )
			{
				triangle = new Triangle();
				return false;
			}

			var vertex1 = vertices[ i1 ].ToVector3();
			var vertex2 = vertices[ i2 ].ToVector3();
			var vertex3 = vertices[ i3 ].ToVector3();
			triangle = new Triangle( vertex1, vertex2, vertex3 );
			return true;
		}

		public static bool GetTriangleData( int triangleID, ref Matrix4 transform, Vector3F[] vertices, int[] indices, out Triangle triangle )
		{
			if( vertices == null )
			{
				triangle = new Triangle();
				return false;
			}

			var i1 = triangleID * 3;
			var i2 = i1 + 1;
			var i3 = i1 + 2;
			if( indices != null )
			{
				if( i3 >= indices.Length )
				{
					triangle = new Triangle();
					return false;
				}
				i1 = indices[ i1 ];
				i2 = indices[ i2 ];
				i3 = indices[ i3 ];
			}

			if( i1 >= vertices.Length || i2 >= vertices.Length || i3 >= vertices.Length )
			{
				triangle = new Triangle();
				return false;
			}

			var vertex1 = transform * vertices[ i1 ].ToVector3();
			var vertex2 = transform * vertices[ i2 ].ToVector3();
			var vertex3 = transform * vertices[ i3 ].ToVector3();
			triangle = new Triangle( vertex1, vertex2, vertex3 );
			return true;
		}

		public static bool GetTriangleData( int triangleID, Matrix4 transform, Vector3F[] vertices, int[] indices, out Triangle triangle )
		{
			return GetTriangleData( triangleID, ref transform, vertices, indices, out triangle );
		}

		public static bool GetTriangleData( int triangleID, Transform transform, Vector3F[] vertices, int[] indices, out Triangle triangle )
		{
			if( transform != null && !transform.IsIdentity )
			{
				Matrix4 mat = transform.ToMatrix4();
				return GetTriangleData( triangleID, ref mat, vertices, indices, out triangle );
			}
			else
				return GetTriangleData( triangleID, vertices, indices, out triangle );
		}

		public static bool IntersectsConvexHull( Plane[] convexPlanes, Bounds bounds )
		{
			//!!!!slowly

			foreach( var plane in convexPlanes )
			{
				if( bounds.GetPlaneSide( plane ) == Plane.Side.Positive )
					return false;
			}
			return true;
		}

		// Returns the convex hull, assuming that each points[i] <= points[i + 1]. Runs in O(n) time.
		static List<Vector2> MakeHullPresorted( IList<Vector2> points )
		{
			if( points.Count <= 1 )
				return new List<Vector2>( points );

			// Andrew's monotone chain algorithm. Positive y coordinates correspond to "up"
			// as per the mathematical convention, instead of "down" as per the computer
			// graphics convention. This doesn't affect the correctness of the result.

			List<Vector2> upperHull = new List<Vector2>();
			foreach( var p in points )
			{
				while( upperHull.Count >= 2 )
				{
					var q = upperHull[ upperHull.Count - 1 ];
					var r = upperHull[ upperHull.Count - 2 ];
					if( ( q.X - r.X ) * ( p.Y - r.Y ) >= ( q.Y - r.Y ) * ( p.X - r.X ) )
						upperHull.RemoveAt( upperHull.Count - 1 );
					else
						break;
				}
				upperHull.Add( p );
			}
			upperHull.RemoveAt( upperHull.Count - 1 );

			IList<Vector2> lowerHull = new List<Vector2>();
			for( int i = points.Count - 1; i >= 0; i-- )
			{
				var p = points[ i ];
				while( lowerHull.Count >= 2 )
				{
					var q = lowerHull[ lowerHull.Count - 1 ];
					var r = lowerHull[ lowerHull.Count - 2 ];
					if( ( q.X - r.X ) * ( p.Y - r.Y ) >= ( q.Y - r.Y ) * ( p.X - r.X ) )
						lowerHull.RemoveAt( lowerHull.Count - 1 );
					else
						break;
				}
				lowerHull.Add( p );
			}
			lowerHull.RemoveAt( lowerHull.Count - 1 );

			if( !( upperHull.Count == 1 && Enumerable.SequenceEqual( upperHull, lowerHull ) ) )
				upperHull.AddRange( lowerHull );
			return upperHull;
		}

		// Returns a new list of points representing the convex hull of the given set of points. The convex hull excludes collinear points. This algorithm runs in O(n log n) time.
		static List<Vector2> MakeHull( IList<Vector2> points )
		{
			List<Vector2> newPoints = new List<Vector2>( points );
			CollectionUtility.MergeSort( newPoints, delegate ( Vector2 p1, Vector2 p2 )
			{
				if( p1.X < p2.X )
					return -1;
				else if( p1.X > p2.X )
					return +1;
				else if( p1.Y < p2.Y )
					return -1;
				else if( p1.Y > p2.Y )
					return +1;
				else
					return 0;
			} );
			return MakeHullPresorted( newPoints );
		}

		public static List<Vector2> GetConvexByPoints( IList<Vector2> points )
		{
			return MakeHull( points );
		}

		public static bool IsPointInPolygon( IList<Vector2> polygon, Vector2 point )
		{
			int polygonLength = polygon.Count, i = 0;
			bool inside = false;
			// x, y for tested point.
			double pointX = point.X, pointY = point.Y;
			// start / end point for the current polygon segment.
			double startX, startY, endX, endY;
			Vector2 endPoint = polygon[ polygonLength - 1 ];
			endX = endPoint.X;
			endY = endPoint.Y;
			while( i < polygonLength )
			{
				startX = endX; startY = endY;
				endPoint = polygon[ i++ ];
				endX = endPoint.X; endY = endPoint.Y;
				//
				inside ^= ( endY > pointY ^ startY > pointY ) /* ? pointY inside [startY;endY] segment ? */
					&& /* if so, test if it is under the segment */
					( ( pointX - endX ) < ( pointY - endY ) * ( startX - endX ) / ( startY - endY ) );
			}
			return inside;
		}

		public static Vector2[] GenerateCapsuleConvex( Capsule2 capsule, int edges )
		{
			var pointsSide = Math.Max( ( edges - 2 ) / 2, 1 ) + 1;

			var diff = capsule.Point2 - capsule.Point1;
			var capsuleAngle = Math.Atan2( -diff.Y, diff.X );

			var result = new Vector2[ pointsSide * 2 ];
			for( int n = 0; n < pointsSide; n++ )
			{
				double factor = (double)n / (double)( pointsSide - 1 );
				var angle = capsuleAngle + factor * Math.PI;

				result[ n ] = capsule.Point1 - new Vector2( MathEx.Sin( angle ), MathEx.Cos( angle ) ) * capsule.Radius;
				result[ pointsSide + n ] = capsule.Point2 + new Vector2( MathEx.Sin( angle ), MathEx.Cos( angle ) ) * capsule.Radius;
			}
			return result;
		}

	}
}
