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
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Net3dBool
{
    /// <summary>
    /// Class representing a 3D solid.
    /// </summary>
    public class Solid
    {
        /** array of indices for the vertices from the 'vertices' attribute */
        protected int[] indices;
        /** array of points defining the solid's vertices */
        protected Vector3[] vertices;

        //--------------------------------CONSTRUCTORS----------------------------------//

        /** Constructs an empty solid. */
        public Solid()
        {
            setInitialFeatures();
        }

        /**
        * Construct a solid based on data arrays. An exception may occur in the case of 
        * abnormal arrays (indices making references to inexistent vertices, there are less
        * colors than vertices...)
        * 
        * @param vertices array of points defining the solid vertices
        * @param indices array of indices for a array of vertices
        * @param colors array of colors defining the vertices colors 
        */
        public Solid(Vector3[] vertices, int[] indices)
            : this()
        {
            setData(vertices, indices);
        }

        /** Sets the initial features common to all constructors */
        protected void setInitialFeatures()
        {
            vertices = new Vector3[0];
            indices = new int[0];

            //            setCapability(Shape3D.ALLOW_GEOMETRY_WRITE);
            //            setCapability(Shape3D.ALLOW_APPEARANCE_WRITE);
            //            setCapability(Shape3D.ALLOW_APPEARANCE_READ);
        }

        //---------------------------------------GETS-----------------------------------//

        /**
        * Gets the solid vertices
        * 
        * @return solid vertices
        */
        public Vector3[] getVertices()
        {
            Vector3[] newVertices = new Vector3[vertices.Length];
            for (int i = 0; i < newVertices.Length; i++)
            {
                newVertices[i] = vertices[i];
            }
            return newVertices;
        }

        /** Gets the solid indices for its vertices
        * 
        * @return solid indices for its vertices
        */
        public int[] getIndices()
        {
            int[] newIndices = new int[indices.Length];
            Array.Copy(indices, 0, newIndices, 0, indices.Length);
            return newIndices;
        }

        /**
        * Gets if the solid is empty (without any vertex)
        * 
        * @return true if the solid is empty, false otherwise
        */
        public bool isEmpty()
        {
            if (indices.Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //---------------------------------------SETS-----------------------------------//

        /**
        * Sets the solid data. Each vertex may have a different color. An exception may 
        * occur in the case of abnormal arrays (e.g., indices making references to  
        * inexistent vertices, there are less colors than vertices...)
        * 
        * @param vertices array of points defining the solid vertices
        * @param indices array of indices for a array of vertices
        * @param colors array of colors defining the vertices colors 
        */
        public void setData(Vector3[] vertices, int[] indices)
        {
            this.vertices = new Vector3[vertices.Length];
            this.indices = new int[indices.Length];
            if (indices.Length != 0)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    this.vertices[i] = vertices[i];
                }
                Array.Copy(indices, 0, this.indices, 0, indices.Length);

                defineGeometry();
            }
        }

        //-------------------------GEOMETRICAL_TRANSFORMATIONS-------------------------//

        /**
        * Applies a translation into a solid
        * 
        * @param dx translation on the x axis
        * @param dy translation on the y axis
        */
        public void translate(double dx, double dy)
        {
            if (dx != 0 || dy != 0)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i].x += dx;
                    vertices[i].y += dy;
                }

                defineGeometry();
            }
        }

        /**
        * Applies a rotation into a solid
        * 
        * @param dx rotation on the x axis
        * @param dy rotation on the y axis
        */
        public void rotate(double dx, double dy)
        {
            double cosX = Math.Cos(dx);
            double cosY = Math.Cos(dy);
            double sinX = Math.Sin(dx);
            double sinY = Math.Sin(dy);

            if (dx != 0 || dy != 0)
            {
                //get mean
                Vector3 mean = getMean();

                double newX, newY, newZ;
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i].x -= mean.x;
                    vertices[i].y -= mean.y;
                    vertices[i].z -= mean.z;

                    //x rotation
                    if (dx != 0)
                    {
                        newY = vertices[i].y * cosX - vertices[i].z * sinX;
                        newZ = vertices[i].y * sinX + vertices[i].z * cosX;
                        vertices[i].y = newY;
                        vertices[i].z = newZ;
                    }

                    //y rotation
                    if (dy != 0)
                    {
                        newX = vertices[i].x * cosY + vertices[i].z * sinY;
                        newZ = -vertices[i].x * sinY + vertices[i].z * cosY;
                        vertices[i].x = newX;
                        vertices[i].z = newZ;
                    }

                    vertices[i].x += mean.x;
                    vertices[i].y += mean.y;
                    vertices[i].z += mean.z;
                }
            }

            defineGeometry();
        }

        /**
        * Applies a zoom into a solid
        * 
        * @param dz translation on the z axis
        */
        public void zoom(double dz)
        {
            if (dz != 0)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i].z += dz;
                }

                defineGeometry();
            }
        }

        /**
        * Applies a scale changing into the solid
        * 
        * @param dx scale changing for the x axis 
        * @param dy scale changing for the y axis
        * @param dz scale changing for the z axis
        */
        public void scale(double dx, double dy, double dz)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].x *= dx;
                vertices[i].y *= dy;
                vertices[i].z *= dz;
            }

            defineGeometry();
        }

        //-----------------------------------PRIVATES--------------------------------//

        /** Creates a geometry based on the indexes and vertices set for the solid */
        protected void defineGeometry()
        {
            //            GeometryInfo gi = new GeometryInfo(GeometryInfo.TRIANGLE_ARRAY);
            //            gi.setCoordinateIndices(indices);
            //            gi.setCoordinates(vertices);
            //            NormalGenerator ng = new NormalGenerator();
            //            ng.generateNormals(gi);
            //
            //            gi.setColors(colors);
            //            gi.setColorIndices(indices);
            //            gi.recomputeIndices();
            //
            //            setGeometry(gi.getIndexedGeometryArray());
        }

        /**
        * Gets the solid mean
        * 
        * @return point representing the mean
        */
        protected Vector3 getMean()
        {
            Vector3 mean = new Vector3();
            for (int i = 0; i < vertices.Length; i++)
            {
                mean.x += vertices[i].x;
                mean.y += vertices[i].y;
                mean.z += vertices[i].z;
            }
            mean.x /= vertices.Length;
            mean.y /= vertices.Length;
            mean.z /= vertices.Length;

            return mean;
        }
    }
}

