using Internal.BulletSharp.Math;
using System.Collections.Generic;
using System.Linq;

namespace Internal.BulletSharp
{
	public static class GeometryUtil
	{
		public static bool AreVerticesBehindPlane(BVector3 planeNormal, double planeConstant, IEnumerable<BVector3> vertices,
			double margin)
		{
			return vertices.All(v => planeNormal.Dot(v) + planeConstant <= margin);
		}

		public static List<BVector4> GetPlaneEquationsFromVertices(ICollection<BVector3> vertices)
		{
			int numVertices = vertices.Count;
			BVector3[] vertexArray = vertices.ToArray();
			var planeEquations = new List<BVector4>();

			for (int i = 0; i < numVertices; i++)
			{
				for (int j = i + 1; j < numVertices; j++)
				{
					for (int k = j + 1; k < numVertices; k++)
					{
						BVector3 edge0 = vertexArray[j] - vertexArray[i];
						BVector3 edge1 = vertexArray[k] - vertexArray[i];

						BVector3 normal = edge0.Cross(edge1);
						if (normal.LengthSquared > 0.0001)
						{
							normal.Normalize();
							if (!Vector4EnumerableContainsVector3(planeEquations, normal))
							{
								double constant = -normal.Dot(vertexArray[i]);
								if (AreVerticesBehindPlane(normal, constant, vertexArray, 0.01f))
								{
									planeEquations.Add(new BVector4(normal, constant));
								}
							}

							normal = -normal;
							if (!Vector4EnumerableContainsVector3(planeEquations, normal))
							{
								double constant = -normal.Dot(vertexArray[i]);
								if (AreVerticesBehindPlane(normal, constant, vertexArray, 0.01f))
								{
									planeEquations.Add(new BVector4(normal, constant));
								}
							}
						}
					}
				}
			}

			return planeEquations;
		}

		private static bool Vector4EnumerableContainsVector3(IEnumerable<BVector4> vertices, BVector3 vertex)
		{
			return vertices.Any(v => {
				var v3 = new BVector3(v.X, v.Y, v.Z);
				return v3.Dot(vertex) > 0.999;
			});
		}

		public static List<BVector3> GetVerticesFromPlaneEquations(ICollection<BVector4> planeEquations)
		{
			int numPlanes = planeEquations.Count;
			BVector3[] planeNormals = new BVector3[numPlanes];
			double[] planeConstants = new double[numPlanes];
			int i = 0;
			foreach (BVector4 plane in planeEquations)
			{
				planeNormals[i] = new BVector3(plane.X, plane.Y, plane.Z);
				planeConstants[i] = plane.W;
				i++;
			}

			var vertices = new List<BVector3>();

			for (i = 0; i < numPlanes; i++)
			{
				for (int j = i + 1; j < numPlanes; j++)
				{
					for (int k = j + 1; k < numPlanes; k++)
					{
						BVector3 n2n3 = planeNormals[j].Cross(planeNormals[k]);
						BVector3 n3n1 = planeNormals[k].Cross(planeNormals[i]);
						BVector3 n1n2 = planeNormals[i].Cross(planeNormals[j]);

						if ((n2n3.LengthSquared > 0.0001f) &&
							 (n3n1.LengthSquared > 0.0001f) &&
							 (n1n2.LengthSquared > 0.0001f))
						{
							//point P out of 3 plane equations:

							//	  d1 ( N2 * N3 ) + d2 ( N3 * N1 ) + d3 ( N1 * N2 )  
							//P = ------------------------------------------------
							//	N1 . ( N2 * N3 )  

							double quotient = planeNormals[i].Dot(n2n3);
							if (System.Math.Abs(quotient) > 0.000001)
							{
								quotient = -1.0f / quotient;
								n2n3 *= planeConstants[i];
								n3n1 *= planeConstants[j];
								n1n2 *= planeConstants[k];
								BVector3 potentialPoint = quotient * (n2n3 + n3n1 + n1n2);

								//check if inside, and replace supportingVertexOut if needed
								if (IsPointInsidePlanes(planeEquations, potentialPoint, 0.01f))
								{
									vertices.Add(potentialPoint);
								}
							}
						}
					}
				}
			}

			return vertices;
		}

		public static bool IsPointInsidePlanes(IEnumerable<BVector4> planeEquations,
			BVector3 point, double margin)
		{
			return planeEquations.All(p => new BVector3(p.X, p.Y, p.Z).Dot(point) + p.W <= margin);
		}
	}
}
