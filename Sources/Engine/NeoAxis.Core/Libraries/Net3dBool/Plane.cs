/*
Copyright (c) 2014, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
*/

using System;

namespace Net3dBool
{
    public class Plane
    {
        public double distanceToPlaneFromOrigin;
        public Vector3 planeNormal;
        private const double TreatAsZero = .000000001;
        public Plane(Vector3 planeNormal, double distanceFromOrigin)
        {
            this.planeNormal = planeNormal.GetNormal();
            this.distanceToPlaneFromOrigin = distanceFromOrigin;
        }

        public Plane(Vector3 point0, Vector3 point1, Vector3 point2)
        {
            this.planeNormal = Vector3.Cross((point1 - point0), (point2 - point0)).GetNormal();
            this.distanceToPlaneFromOrigin = Vector3.Dot(planeNormal, point0);
        }

        public Plane(Vector3 planeNormal, Vector3 pointOnPlane)
        {
            this.planeNormal = planeNormal.GetNormal();
            this.distanceToPlaneFromOrigin = Vector3.Dot(planeNormal, pointOnPlane);
        }

        public double GetDistanceFromPlane(Vector3 positionToCheck)
        {
            double distanceToPointFromOrigin = Vector3.Dot(positionToCheck, planeNormal);
            return distanceToPointFromOrigin - distanceToPlaneFromOrigin;
        }

        public double GetDistanceToIntersection(Ray ray, out bool inFront)
        {
            inFront = false;
            double normalDotRayDirection = Vector3.Dot(planeNormal, ray.directionNormal);
            if (normalDotRayDirection < TreatAsZero && normalDotRayDirection > -TreatAsZero) // the ray is parallel to the plane
            {
                return double.PositiveInfinity;
            }

            if (normalDotRayDirection < 0)
            {
                inFront = true;
            }

            return (distanceToPlaneFromOrigin - Vector3.Dot(planeNormal, ray.origin)) / normalDotRayDirection;
        }

        public double GetDistanceToIntersection(Vector3 pointOnLine, Vector3 lineDirection)
        {
            double normalDotRayDirection = Vector3.Dot(planeNormal, lineDirection);
            if (normalDotRayDirection < TreatAsZero && normalDotRayDirection > -TreatAsZero) // the ray is parallel to the plane
            {
                return double.PositiveInfinity;
            }

            double planeNormalDotPointOnLine = Vector3.Dot(planeNormal, pointOnLine);
            return (distanceToPlaneFromOrigin - planeNormalDotPointOnLine) / normalDotRayDirection;
        }

        public bool RayHitPlane(Ray ray, out double distanceToHit, out bool hitFrontOfPlane)
        {
            distanceToHit = double.PositiveInfinity;
            hitFrontOfPlane = false;

            double normalDotRayDirection = Vector3.Dot(planeNormal, ray.directionNormal);
            if (normalDotRayDirection < TreatAsZero && normalDotRayDirection > -TreatAsZero) // the ray is parallel to the plane
            {
                return false;
            }

            if (normalDotRayDirection < 0)
            {
                hitFrontOfPlane = true;
            }

            double distanceToRayOriginFromOrigin = Vector3.Dot(planeNormal, ray.origin);

            double distanceToPlaneFromRayOrigin = distanceToPlaneFromOrigin - distanceToRayOriginFromOrigin;

            bool originInFrontOfPlane = distanceToPlaneFromRayOrigin < 0;

            bool originAndHitAreOnSameSide = originInFrontOfPlane == hitFrontOfPlane;
            if (!originAndHitAreOnSameSide)
            {
                return false;
            }

            distanceToHit = distanceToPlaneFromRayOrigin / normalDotRayDirection;
            return true;
        }

        public bool LineHitPlane(Vector3 start, Vector3 end, out Vector3 intersectionPosition)
        {
            double distanceToStartFromOrigin = Vector3.Dot(planeNormal, start);
            if (distanceToStartFromOrigin == 0)
            {
                intersectionPosition = start;
                return true;
            }

            double distanceToEndFromOrigin = Vector3.Dot(planeNormal, end);
            if (distanceToEndFromOrigin == 0)
            {
                intersectionPosition = end;
                return true;
            }

            if ((distanceToStartFromOrigin < 0 && distanceToEndFromOrigin > 0)
                || (distanceToStartFromOrigin > 0 && distanceToEndFromOrigin < 0))
            {
                Vector3 direction = (end - start).GetNormal();

                double startDistanceFromPlane = distanceToStartFromOrigin - distanceToPlaneFromOrigin;
                double endDistanceFromPlane = distanceToEndFromOrigin - distanceToPlaneFromOrigin;
                double lengthAlongPlanNormal = endDistanceFromPlane - startDistanceFromPlane;

                double ratioToPlanFromStart = startDistanceFromPlane / lengthAlongPlanNormal;
                intersectionPosition = start + direction * ratioToPlanFromStart;

                return true;
            }

            intersectionPosition = Vector3.PositiveInfinity;
            return false;
        }
    }
}