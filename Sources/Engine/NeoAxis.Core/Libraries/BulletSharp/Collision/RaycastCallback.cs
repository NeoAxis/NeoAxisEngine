using System;
using Internal.BulletSharp.Math;

namespace Internal.BulletSharp
{
	public abstract class TriangleRaycastCallback : TriangleCallback
	{
		[Flags]
		public enum EFlags
		{
			None = 0,
            FilterBackfaces = 1,
            KeepUnflippedNormal = 2,
            UseSubSimplexConvexCastRaytest = 4,
            UseGjkConvexCastRaytest = 8,
            Terminator = -1
		}

        public TriangleRaycastCallback(ref BVector3 from, ref BVector3 to, EFlags flags)
        {
            HitFraction = 1.0f;
        }

        public TriangleRaycastCallback(ref BVector3 from, ref BVector3 to)
            : this(ref from, ref to, EFlags.None)
        {
        }

        public override void ProcessTriangle(ref BVector3 point0, ref BVector3 point1, ref BVector3 point2, int partId, int triangleIndex)
        {
            BVector3 v10 = point1 - point0;
            BVector3 v20 = point2 - point0;

            BVector3 triangleNormal = v10.Cross(v20);

            double dist;
            point0.Dot(ref triangleNormal, out dist);
            double distA = triangleNormal.Dot(From) - dist;
            double distB = triangleNormal.Dot(To) - dist;

            if (distA * distB >= 0.0f)
            {
                return; // same sign
            }

            if (((Flags & EFlags.FilterBackfaces) != 0) && (distA <= 0.0f))
            {
                // Backface, skip check
                return;
            }


            double proj_length = distA - distB;
            double distance = (distA) / (proj_length);
            // Now we have the intersection point on the plane, we'll see if it's inside the triangle
            // Add an epsilon as a tolerance for the raycast,
            // in case the ray hits exacly on the edge of the triangle.
            // It must be scaled for the triangle size.

            if (distance < HitFraction)
            {
                double edgeTolerance = triangleNormal.LengthSquared;
                edgeTolerance *= -0.0001f;
                BVector3 point = BVector3.Lerp(From, To, distance);
                {
                    BVector3 v0p; v0p = point0 - point;
                    BVector3 v1p; v1p = point1 - point;
                    BVector3 cp0; cp0 = v0p.Cross(v1p);

                    double dot;
                    cp0.Dot(ref triangleNormal, out dot);
                    if (dot >= edgeTolerance)
                    {
                        BVector3 v2p; v2p = point2 - point;
                        BVector3 cp1;
                        cp1 = v1p.Cross(v2p);
                        cp1.Dot(ref triangleNormal, out dot);
                        if (dot >= edgeTolerance)
                        {
                            BVector3 cp2;
                            cp2 = v2p.Cross(v0p);

                            cp2.Dot(ref triangleNormal, out dot);
                            if (dot >= edgeTolerance)
                            {
                                //@BP Mod
                                // Triangle normal isn't normalized
                                triangleNormal.Normalize();

                                //@BP Mod - Allow for unflipped normal when raycasting against backfaces
                                if (((Flags & EFlags.KeepUnflippedNormal) == 0) && (distA <= 0.0f))
                                {
                                    triangleNormal = -triangleNormal;
                                }
                                HitFraction = ReportHit(ref triangleNormal, distance, partId, triangleIndex);
                            }
                        }
                    }
                }
            }
        }

        public abstract double ReportHit(ref BVector3 hitNormalLocal, double hitFraction, int partId, int triangleIndex);

        public EFlags Flags { get; set; }
        public BVector3 From { get; set; }
        public double HitFraction { get; set; }
        public BVector3 To { get; set; }
	}

	public abstract class TriangleConvexcastCallback : TriangleCallback
	{
        public TriangleConvexcastCallback(ConvexShape convexShape, ref BMatrix convexShapeFrom, ref BMatrix convexShapeTo, ref BMatrix triangleToWorld, double triangleCollisionMargin)
        {
            ConvexShape = convexShape;
            ConvexShapeFrom = convexShapeFrom;
            ConvexShapeTo = convexShapeTo;
            TriangleToWorld = triangleToWorld;
            TriangleCollisionMargin = triangleCollisionMargin;

            AllowedPenetration = 0.0f;
            HitFraction = 1.0f;
        }

        public override void ProcessTriangle(ref BVector3 point0, ref BVector3 point1, ref BVector3 point2, int partId, int triangleIndex)
        {
            throw new NotImplementedException();
        }

        public abstract double ReportHit(ref BVector3 hitNormalLocal, ref BVector3 hitPointLocal, double hitFraction, int partId, int triangleIndex);

        public double AllowedPenetration { get; set; }
        public ConvexShape ConvexShape { get; set; }
        public BMatrix ConvexShapeFrom { get; set; }
        public BMatrix ConvexShapeTo { get; set; }
        public double HitFraction { get; set; }
        public double TriangleCollisionMargin { get; set; }
        public BMatrix TriangleToWorld { get; set; }
	}
}
