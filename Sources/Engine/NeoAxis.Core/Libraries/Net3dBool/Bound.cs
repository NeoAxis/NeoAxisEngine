/*
The MIT License (MIT)

Copyright (c) 2014 Sebastian Loncar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

See:
D. H. Laidlaw, W. B. Trumbore, and J. F. Hughes.
"Constructive Solid Geometry for Polyhedral Objects"
SIGGRAPH Proceedings, 1986, p.161.

original author: Danilo Balby Silva Castanheira (danbalby@yahoo.com)

Ported from Java to C# by Sebastian Loncar, Web: http://www.loncar.de
Project: https://github.com/Arakis/Net3dBool

Optimized and refactored by: Lars Brubaker (larsbrubaker@matterhackers.com)
Project: https://github.com/MatterHackers/agg-sharp (an included library)
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Net3dBool
{
    /// <summary>
    /// Representation of a bound - the extremes of a 3d component for each coordinate.
    /// </summary>
    public class Bound
    {
        /** maximum from the x coordinate */
        private double xMax;
        /** minimum from the x coordinate */
        private double xMin;
        /** maximum from the y coordinate */
        private double yMax;
        /** minimum from the y coordinate */
        private double yMin;
        /** maximum from the z coordinate */
        private double zMax;
        /** minimum from the z coordinate */
        private double zMin;

        /** tolerance value to test equalities */
        private readonly static double EqualityTolerance = 1e-10f;

        //---------------------------------CONSTRUCTORS---------------------------------//

        /**
        * Bound constructor for a face
        * 
        * @param p1 point relative to the first vertex
        * @param p2 point relative to the second vertex
        * @param p3 point relative to the third vertex
        */
        public Bound(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            xMax = xMin = p1.x;
            yMax = yMin = p1.y;
            zMax = zMin = p1.z;

            CheckVertex(p2);
            CheckVertex(p3);
        }

        /**
        * Bound constructor for a object 3d
        * 
        * @param vertices the object vertices
        */
        public Bound(Vector3[] vertices)
        {
            xMax = xMin = vertices[0].x;
            yMax = yMin = vertices[0].y;
            zMax = zMin = vertices[0].z;

            for (int i = 1; i < vertices.Length; i++)
            {
                CheckVertex(vertices[i]);
            }
        }

        //----------------------------------OVERRIDES-----------------------------------//

        /**
        * Makes a string definition for the bound object
        * 
        * @return the string definition
        */
        public String toString()
        {
            return "x: " + xMin + " .. " + xMax + "\ny: " + yMin + " .. " + yMax + "\nz: " + zMin + " .. " + zMax;
        }

        //--------------------------------------OTHERS----------------------------------//

        /**
        * Checks if a bound overlaps other one
        * 
        * @param bound other bound to make the comparison
        * @return true if they insersect, false otherwise
        */
        public bool Overlap(Bound bound)
        {
            if ((xMin > bound.xMax + EqualityTolerance) || (xMax < bound.xMin - EqualityTolerance) || (yMin > bound.yMax + EqualityTolerance) || (yMax < bound.yMin - EqualityTolerance) || (zMin > bound.zMax + EqualityTolerance) || (zMax < bound.zMin - EqualityTolerance))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //-------------------------------------PRIVATES---------------------------------//

        /**
        * Checks if one of the coordinates of a vertex exceed the ones found before 
        * 
        * @param vertex vertex to be tested
        */
        private void CheckVertex(Vector3 vertex)
        {
            if (vertex.x > xMax)
            {
                xMax = vertex.x;
            }
            else if (vertex.x < xMin)
            {
                xMin = vertex.x;
            }

            if (vertex.y > yMax)
            {
                yMax = vertex.y;
            }
            else if (vertex.y < yMin)
            {
                yMin = vertex.y;
            }

            if (vertex.z > zMax)
            {
                zMax = vertex.z;
            }
            else if (vertex.z < zMin)
            {
                zMin = vertex.z;
            }
        }
    }
}

