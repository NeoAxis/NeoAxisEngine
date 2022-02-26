//using Internal.BulletSharp.Math;

//namespace Internal.BulletSharp
//{
//	internal class MyCallback : TriangleRaycastCallback
//	{
//		private readonly int _ignorePart;
//		private readonly int _ignoreTriangleIndex;

//		public MyCallback(ref BVector3 from, ref BVector3 to, int ignorePart, int ignoreTriangleIndex)
//			: base(ref from, ref to)
//		{
//			_ignorePart = ignorePart;
//			_ignoreTriangleIndex = ignoreTriangleIndex;
//		}

//		public override double ReportHit(ref BVector3 hitNormalLocal, double hitFraction, int partId, int triangleIndex)
//		{
//			if (partId != _ignorePart || triangleIndex != _ignoreTriangleIndex)
//			{
//				if (hitFraction < HitFraction)
//					return hitFraction;
//			}

//			return HitFraction;
//		}
//	}

//	internal class MyInternalTriangleIndexCallback : InternalTriangleIndexCallback
//	{
//		private readonly CompoundShape _collisionShape;
//		private readonly double _depth;
//		private readonly GImpactMeshShape _meshShape;
//		//private readonly static Vector3 _redColor = new Vector3(1, 0, 0);

//		public MyInternalTriangleIndexCallback(CompoundShape collisionShape, GImpactMeshShape meshShape, double depth)
//		{
//			_collisionShape = collisionShape;
//			_depth = depth;
//			_meshShape = meshShape;
//		}

//		public override void InternalProcessTriangleIndex(ref BVector3 vertex0, ref BVector3 vertex1, ref BVector3 vertex2, int partId, int triangleIndex)
//		{
//			BVector3 scale = _meshShape.LocalScaling;
//			BVector3 v0 = vertex0 * scale;
//			BVector3 v1 = vertex1 * scale;
//			BVector3 v2 = vertex2 * scale;

//			BVector3 centroid = (v0 + v1 + v2) / 3;
//			BVector3 normal = (v1 - v0).Cross(v2 - v0);
//			normal.Normalize();
//			BVector3 rayFrom = centroid;
//			BVector3 rayTo = centroid - normal * _depth;

//			using (var cb = new MyCallback(ref rayFrom, ref rayTo, partId, triangleIndex))
//			{
//				_meshShape.ProcessAllTrianglesRayRef(cb, ref rayFrom, ref rayTo);
//				if (cb.HitFraction < 1)
//				{
//					rayTo = BVector3.Lerp(cb.From, cb.To, cb.HitFraction);
//					//rayTo = cb.From;
//					//Vector3 to = centroid + normal;
//					//debugDraw.DrawLine(ref centroid, ref to, ref _redColor);
//				}
//			}

//			var triangle = new BuSimplex1To4(v0, v1, v2, rayTo);
//			_collisionShape.AddChildShape(BMatrix.Identity, triangle);
//		}
//	}

//	public static class CompoundFromGImpact
//	{
//		public static CompoundShape Create(GImpactMeshShape impactMesh, double depth)
//		{
//			var shape = new CompoundShape();
//			using (var callback = new MyInternalTriangleIndexCallback(shape, impactMesh, depth))
//			{
//				BVector3 aabbMin, aabbMax;
//				impactMesh.GetAabb(BMatrix.Identity, out aabbMin, out aabbMax);
//				impactMesh.MeshInterface.InternalProcessAllTriangles(callback, aabbMin, aabbMax);
//			}
//			return shape;
//		}
//	}
//}
