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
    public class AxisAlignedBoundingBox
    {
        public Vector3 minXYZ;
        public Vector3 maxXYZ;

        public AxisAlignedBoundingBox(Vector3 minXYZ, Vector3 maxXYZ)
        {
            if (maxXYZ.x < minXYZ.x || maxXYZ.y < minXYZ.y || maxXYZ.z < minXYZ.z)
            {
                throw new ArgumentException("All values of min must be less than all values in max.");
            }

            this.minXYZ = minXYZ;
            this.maxXYZ = maxXYZ;
        }

        public Vector3 Size
        {
            get
            {
                return maxXYZ - minXYZ;
            }
        }

        public double XSize
        {
            get
            {
                return maxXYZ.x - minXYZ.x;
            }
        }

        public double YSize
        {
            get
            {
                return maxXYZ.y - minXYZ.y;
            }
        }

        public double ZSize
        {
            get
            {
                return maxXYZ.z - minXYZ.z;
            }
        }

        /// <summary>
        /// Geth the corners by quadrant of the bottom
        /// </summary>
        /// <param name="quadrantIndex"></param>
        public Vector3 GetBottomCorner(int quadrantIndex)
        {
            switch (quadrantIndex)
            {
                case 0:
                    return new Vector3(maxXYZ.x, maxXYZ.y, minXYZ.z);

                case 1:
                    return new Vector3(minXYZ.x, maxXYZ.y, minXYZ.z);

                case 2:
                    return new Vector3(minXYZ.x, minXYZ.y, minXYZ.z);

                case 3:
                    return new Vector3(maxXYZ.x, minXYZ.y, minXYZ.z);
            }

            return Vector3.Zero;
        }

        public Vector3 Center
        {
            get
            {
                return (minXYZ + maxXYZ) / 2;
            }
        }

        /// <summary>
        /// This is the computation cost of doing an intersection with the given type.
        /// Attempt to give it in average CPU cycles for the intersecton.
        /// </summary>
        /// <returns></returns>
        public static double GetIntersectCost()
        {
            // it would be great to try and measure this more accurately.  This is a guess from looking at the intersect function.
            return 132;
        }

        public Vector3 GetCenter()
        {
            return (minXYZ + maxXYZ) * .5;
        }

        public double GetCenterX()
        {
            return (minXYZ.x + maxXYZ.x) * .5;
        }

        private double volumeCache = 0;

        public double GetVolume()
        {
            if (volumeCache == 0)
            {
                volumeCache = (maxXYZ.x - minXYZ.x) * (maxXYZ.y - minXYZ.y) * (maxXYZ.z - minXYZ.z);
            }

            return volumeCache;
        }

        private double surfaceAreaCache = 0;

        public double GetSurfaceArea()
        {
            if (surfaceAreaCache == 0)
            {
                double frontAndBack = (maxXYZ.x - minXYZ.x) * (maxXYZ.z - minXYZ.z) * 2;
                double leftAndRight = (maxXYZ.y - minXYZ.y) * (maxXYZ.z - minXYZ.z) * 2;
                double topAndBottom = (maxXYZ.x - minXYZ.x) * (maxXYZ.y - minXYZ.y) * 2;
                surfaceAreaCache = frontAndBack + leftAndRight + topAndBottom;
            }

            return surfaceAreaCache;
        }

        public Vector3 this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return minXYZ;
                }
                else if (index == 1)
                {
                    return maxXYZ;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public static AxisAlignedBoundingBox operator +(AxisAlignedBoundingBox A, AxisAlignedBoundingBox B)
        {
#if true
            return Union(A, B);
#else
            Vector3 calcMinXYZ = new Vector3();
            calcMinXYZ.x = Math.Min(A.minXYZ.x, B.minXYZ.x);
            calcMinXYZ.y = Math.Min(A.minXYZ.y, B.minXYZ.y);
            calcMinXYZ.z = Math.Min(A.minXYZ.z, B.minXYZ.z);

            Vector3 calcMaxXYZ = new Vector3();
            calcMaxXYZ.x = Math.Max(A.maxXYZ.x, B.maxXYZ.x);
            calcMaxXYZ.y = Math.Max(A.maxXYZ.y, B.maxXYZ.y);
            calcMaxXYZ.z = Math.Max(A.maxXYZ.z, B.maxXYZ.z);

            AxisAlignedBoundingBox combinedBounds = new AxisAlignedBoundingBox(calcMinXYZ, calcMaxXYZ);

            return combinedBounds;
#endif
        }

        public static AxisAlignedBoundingBox Union(AxisAlignedBoundingBox boundsA, AxisAlignedBoundingBox boundsB)
        {
            Vector3 minXYZ = Vector3.Zero;
            minXYZ.x = Math.Min(boundsA.minXYZ.x, boundsB.minXYZ.x);
            minXYZ.y = Math.Min(boundsA.minXYZ.y, boundsB.minXYZ.y);
            minXYZ.z = Math.Min(boundsA.minXYZ.z, boundsB.minXYZ.z);

            Vector3 maxXYZ = Vector3.Zero;
            maxXYZ.x = Math.Max(boundsA.maxXYZ.x, boundsB.maxXYZ.x);
            maxXYZ.y = Math.Max(boundsA.maxXYZ.y, boundsB.maxXYZ.y);
            maxXYZ.z = Math.Max(boundsA.maxXYZ.z, boundsB.maxXYZ.z);

            return new AxisAlignedBoundingBox(minXYZ, maxXYZ);
        }

        public static AxisAlignedBoundingBox Intersection(AxisAlignedBoundingBox boundsA, AxisAlignedBoundingBox boundsB)
        {
            Vector3 minXYZ = Vector3.Zero;
            minXYZ.x = Math.Max(boundsA.minXYZ.x, boundsB.minXYZ.x);
            minXYZ.y = Math.Max(boundsA.minXYZ.y, boundsB.minXYZ.y);
            minXYZ.z = Math.Max(boundsA.minXYZ.z, boundsB.minXYZ.z);

            Vector3 maxXYZ = Vector3.Zero;
            maxXYZ.x = Math.Max(minXYZ.x, Math.Min(boundsA.maxXYZ.x, boundsB.maxXYZ.x));
            maxXYZ.y = Math.Max(minXYZ.y, Math.Min(boundsA.maxXYZ.y, boundsB.maxXYZ.y));
            maxXYZ.z = Math.Max(minXYZ.z, Math.Min(boundsA.maxXYZ.z, boundsB.maxXYZ.z));

            return new AxisAlignedBoundingBox(minXYZ, maxXYZ);
        }

        public static AxisAlignedBoundingBox Union(AxisAlignedBoundingBox bounds, Vector3 vertex)
        {
            Vector3 minXYZ = Vector3.Zero;
            minXYZ.x = Math.Min(bounds.minXYZ.x, vertex.x);
            minXYZ.y = Math.Min(bounds.minXYZ.y, vertex.y);
            minXYZ.z = Math.Min(bounds.minXYZ.z, vertex.z);

            Vector3 maxXYZ = Vector3.Zero;
            maxXYZ.x = Math.Max(bounds.maxXYZ.x, vertex.x);
            maxXYZ.y = Math.Max(bounds.maxXYZ.y, vertex.y);
            maxXYZ.z = Math.Max(bounds.maxXYZ.z, vertex.z);

            return new AxisAlignedBoundingBox(minXYZ, maxXYZ);
        }

        public void Clamp(ref Vector3 positionToClamp)
        {
            if (positionToClamp.x < minXYZ.x)
            {
                positionToClamp.x = minXYZ.x;
            }
            else if (positionToClamp.x > maxXYZ.x)
            {
                positionToClamp.x = maxXYZ.x;
            }

            if (positionToClamp.y < minXYZ.y)
            {
                positionToClamp.y = minXYZ.y;
            }
            else if (positionToClamp.y > maxXYZ.y)
            {
                positionToClamp.y = maxXYZ.y;
            }

            if (positionToClamp.z < minXYZ.z)
            {
                positionToClamp.z = minXYZ.z;
            }
            else if (positionToClamp.z > maxXYZ.z)
            {
                positionToClamp.z = maxXYZ.z;
            }
        }

        public bool Contains(AxisAlignedBoundingBox bounds)
        {
            if (this.minXYZ.x <= bounds.minXYZ.x
                && this.maxXYZ.x >= bounds.maxXYZ.x
                && this.minXYZ.y <= bounds.minXYZ.y
                && this.maxXYZ.y >= bounds.maxXYZ.y
                && this.minXYZ.z <= bounds.minXYZ.z
                && this.maxXYZ.z >= bounds.maxXYZ.z)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("min {0} - max {1}", minXYZ, maxXYZ);
        }
    }
}