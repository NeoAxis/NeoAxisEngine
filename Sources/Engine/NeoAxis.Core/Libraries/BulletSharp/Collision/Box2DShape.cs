//using Internal.BulletSharp.Math;
//using static Internal.BulletSharp.UnsafeNativeMethods;

//namespace Internal.BulletSharp
//{
//	public class Box2DShape : PolyhedralConvexShape
//	{
//		private Vector3Array _normals;
//		private Vector3Array _vertices;

//		public Box2DShape(BVector3 boxHalfExtents)
//			: base(btBox2dShape_new(ref boxHalfExtents))
//		{
//		}

//		public Box2DShape(double boxHalfExtent)
//			: base(btBox2dShape_new2(boxHalfExtent))
//		{
//		}

//		public Box2DShape(double boxHalfExtentX, double boxHalfExtentY, double boxHalfExtentZ)
//			: base(btBox2dShape_new3(boxHalfExtentX, boxHalfExtentY, boxHalfExtentZ))
//		{
//		}

//		public void GetPlaneEquation(out BVector4 plane, int i)
//		{
//			btBox2dShape_getPlaneEquation(Native, out plane, i);
//		}

//		public BVector3 Centroid
//		{
//			get
//			{
//				BVector3 value;
//				btBox2dShape_getCentroid(Native, out value);
//				return value;
//			}
//		}

//		public BVector3 HalfExtentsWithMargin
//		{
//			get
//			{
//				BVector3 value;
//				btBox2dShape_getHalfExtentsWithMargin(Native, out value);
//				return value;
//			}
//		}

//		public BVector3 HalfExtentsWithoutMargin
//		{
//			get
//			{
//				BVector3 value;
//				btBox2dShape_getHalfExtentsWithoutMargin(Native, out value);
//				return value;
//			}
//		}

//		public Vector3Array Normals
//		{
//			get
//			{
//				if (_normals == null)
//				{
//					_normals = new Vector3Array(btBox2dShape_getNormals(Native), 4);
//				}
//				return _normals;
//			}
//		}

//		public Vector3Array Vertices
//		{
//			get
//			{
//				if (_vertices == null)
//				{
//					_vertices = new Vector3Array(btBox2dShape_getVertices(Native), 4);
//				}
//				return _vertices;
//			}
//		}
//	}
//}
