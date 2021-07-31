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
    /// Represents a line segment resulting from a intersection of a face and a plane.
    /// </summary>
    public class Segment
    {
        /** line resulting from the two planes intersection */
        private Line line;
        /** shows how many ends were already defined */
        private int index;

        /** distance from the segment starting point to the point defining the plane */
        public double StartDist { get; private set; }
        /** distance from the segment ending point to the point defining the plane */
        private double endDist;

        /** starting point status relative to the face */
        private int startType;
        /** intermediate status relative to the face */
        private int middleType;
        /** ending point status relative to the face */
        private int endType;

        /** nearest vertex from the starting point */
        private Vertex startVertex;
        /** nearest vertex from the ending point */
        private Vertex endVertex;

        /** start of the intersection point */
        private Vector3 startPos;
        /** end of the intersection point */
        private Vector3 endPos;

        /** define as vertex one of the segment ends */
        public static int VERTEX = 1;
        /** define as face one of the segment ends */
        public static int FACE = 2;
        /** define as edge one of the segment ends */
        public static int EDGE = 3;

        /** tolerance value to test equalities */
        private static double TOL = 1e-10f;

        //---------------------------------CONSTRUCTORS---------------------------------//

        /**
        * Constructs a Segment based on elements obtained from the two planes relations 
        * 
        * @param line resulting from the two planes intersection
        * @param face face that intersects with the plane
        * @param sign1 position of the face vertex1 relative to the plane (-1 behind, 1 front, 0 on)
        * @param sign2 position of the face vertex1 relative to the plane (-1 behind, 1 front, 0 on)
        * @param sign3 position of the face vertex1 relative to the plane (-1 behind, 1 front, 0 on)  
        */
        public Segment(Line line, Face face, int sign1, int sign2, int sign3)
        {
            this.line = line;
            index = 0;

            //VERTEX is an end
            if (sign1 == 0)
            {
                SetVertex(face.v1);
                //other vertices on the same side - VERTEX-VERTEX VERTEX
                if (sign2 == sign3)
                {
                    SetVertex(face.v1);
                }
            }

            //VERTEX is an end
            if (sign2 == 0)
            {
                SetVertex(face.v2);
                //other vertices on the same side - VERTEX-VERTEX VERTEX
                if (sign1 == sign3)
                {
                    SetVertex(face.v2);
                }
            }

            //VERTEX is an end
            if (sign3 == 0)
            {
                SetVertex(face.v3);
                //other vertices on the same side - VERTEX-VERTEX VERTEX
                if (sign1 == sign2)
                {
                    SetVertex(face.v3);
                }
            }

            //There are undefined ends - one or more edges cut the planes intersection line
            if (GetNumEndsSet() != 2)
            {
                //EDGE is an end
                if ((sign1 == 1 && sign2 == -1) || (sign1 == -1 && sign2 == 1))
                {
                    SetEdge(face.v1, face.v2);
                }
                //EDGE is an end
                if ((sign2 == 1 && sign3 == -1) || (sign2 == -1 && sign3 == 1))
                {
                    SetEdge(face.v2, face.v3);
                }
                //EDGE is an end
                if ((sign3 == 1 && sign1 == -1) || (sign3 == -1 && sign1 == 1))
                {
                    SetEdge(face.v3, face.v1);
                }
            }
        }

        private Segment()
        {
        }

        //-----------------------------------OVERRIDES----------------------------------//

        /**
        * Clones the Segment object
        * 
        * @return cloned Segment object
        */
        public Segment Clone()
        {
            Segment clone = new Segment();
            clone.line = line;
            clone.index = index;
            clone.StartDist = StartDist;
            clone.endDist = endDist;
            clone.StartDist = startType;
            clone.middleType = middleType;
            clone.endType = endType;
            clone.startVertex = startVertex;
            clone.endVertex = endVertex;
            clone.startPos = startPos;
            clone.endPos = endPos;

            return clone;
        }

        //-------------------------------------GETS-------------------------------------//

        /**
        * Gets the start vertex
        * 
        * @return start vertex
        */
        public Vertex GetStartVertex()
        {
            return startVertex;
        }

        /**
        * Gets the end vertex
        * 
        * @return end vertex
        */
        public Vertex GetEndVertex()
        {
            return endVertex;
        }

        /**
        * Gets the distance from the origin until ending point
        * 
        * @return distance from the origin until the ending point
        */
        public double GetEndDistance()
        {
            return endDist;
        }

        /**
        * Gets the type of the starting point
        * 
        * @return type of the starting point
        */
        public int GetStartType()
        {
            return startType;
        }

        /**
        * Gets the type of the segment between the starting and ending points
        * 
        * @return type of the segment between the starting and ending points
        */
        public int GetIntermediateType()
        {
            return middleType;
        }

        /**
        * Gets the type of the ending point
        * 
        * @return type of the ending point
        */
        public int GetEndType()
        {
            return endType;
        }

        /**
        * Gets the number of ends already set
        *
        * @return number of ends already set
        */
        public int GetNumEndsSet()
        {
            return index;
        }

        /**
        * Gets the starting position
        * 
        * @return start position
        */
        public Vector3 GetStartPosition()
        {
            return startPos;
        }

        /**
        * Gets the ending position
        * 
        * @return ending position
        */
        public Vector3 GetEndPosition()
        {
            return endPos;
        }

        //------------------------------------OTHERS------------------------------------//

        /**
        * Checks if two segments intersect
        * 
        * @param segment the other segment to check the intesection
        * @return true if the segments intersect, false otherwise
        */
        public bool Intersect(Segment segment)
        {
            if (endDist < segment.StartDist + TOL || segment.endDist < StartDist + TOL)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //---------------------------------PRIVATES-------------------------------------//

        /**
        * Sets an end as vertex (starting point if none end were defined, ending point otherwise)
        * 
        * @param vertex the vertex that is an segment end 
        * @return false if all the ends were already defined, true otherwise
        */
        private bool SetVertex(Vertex vertex)
        {
            //none end were defined - define starting point as VERTEX
            if (index == 0)
            {
                startVertex = vertex;
                startType = VERTEX;
                StartDist = line.ComputePointToPointDistance(vertex.GetPosition());
                startPos = startVertex.GetPosition();
                index++;
                return true;
            }
            //starting point were defined - define ending point as VERTEX
            if (index == 1)
            {
                endVertex = vertex;
                endType = VERTEX;
                endDist = line.ComputePointToPointDistance(vertex.GetPosition());
                endPos = endVertex.GetPosition();
                index++;

                //defining middle based on the starting point
                //VERTEX-VERTEX-VERTEX
                if (startVertex.Equals(endVertex))
                {
                    middleType = VERTEX;
                }
                //VERTEX-EDGE-VERTEX
                else if (startType == VERTEX)
                {
                    middleType = EDGE;
                }

                //the ending point distance should be smaller than  starting point distance 
                if (StartDist > endDist)
                {
                    SwapEnds();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /**
        * Sets an end as edge (starting point if none end were defined, ending point otherwise)
        * 
        * @param vertex1 one of the vertices of the intercepted edge 
        * @param vertex2 one of the vertices of the intercepted edge
        * @return false if all ends were already defined, true otherwise
        */
        private bool SetEdge(Vertex vertex1, Vertex vertex2)
        {
            Vector3 point1 = vertex1.GetPosition();
            Vector3 point2 = vertex2.GetPosition();
            Vector3 edgeDirection = new Vector3(point2.x - point1.x, point2.y - point1.y, point2.z - point1.z);
            Line edgeLine = new Line(edgeDirection, point1);

            if (index == 0)
            {
                startVertex = vertex1;
                startType = EDGE;
                startPos = line.ComputeLineIntersection(edgeLine);
                StartDist = line.ComputePointToPointDistance(startPos);
                middleType = FACE;
                index++;
                return true;
            }
            else if (index == 1)
            {
                endVertex = vertex1;
                endType = EDGE;
                endPos = line.ComputeLineIntersection(edgeLine);
                endDist = line.ComputePointToPointDistance(endPos);
                middleType = FACE;
                index++;

                //the ending point distance should be smaller than  starting point distance 
                if (StartDist > endDist)
                {
                    SwapEnds();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /** Swaps the starting point and the ending point */
        private void SwapEnds()
        {
            double distTemp = StartDist;
            StartDist = endDist;
            endDist = distTemp;

            int typeTemp = startType;
            startType = endType;
            endType = typeTemp;

            Vertex vertexTemp = startVertex;
            startVertex = endVertex;
            endVertex = vertexTemp;

            Vector3 posTemp = startPos;
            startPos = endPos;
            endPos = posTemp;
        }
    }
}

