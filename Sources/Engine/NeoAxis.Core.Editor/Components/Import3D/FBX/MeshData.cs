// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using Internal.Fbx;

namespace NeoAxis.Import.FBX
{
	struct VertexInfo
	{
		public StandardVertex Vertex;
		//Vertex positions in the original FbxMesh.
		public int ControlPointIndex;
		public int PolygonVertexIndex;
		public int PolygonIndex;
	}

	class MeshData
	{
		public FbxNode Node;
		public FbxMesh Mesh;
		public int MaterialIndex = -1;
		public string MaterialName;
		//!!!!new
		public string Name;

		public StandardVertex.Components VertexComponents;

		public VertexInfo[] Vertices;

		//Vertex count in polygons.
		public int PolygonSize;

		public TangentsAndNormalsSource NormalsSource;
		public TangentsAndNormalsSource TangentsSource;

		//CalcMeshCache calcCache;

		//public CalcMeshCache CalcCache
		//{
		//	get
		//	{
		//		if( calcCache == null )
		//			calcCache = new CalcMeshCache( Vertices );
		//		return calcCache;
		//	}
		//}

		//public void ClearCache()
		//{
		//	calcCache = null;
		//}
	}

	struct BoneAssignment
	{
		public int count;
		public int boneIndex0;
		public double weight0;
		public int boneIndex1;
		public double weight1;
		public int boneIndex2;
		public double weight2;
		public int boneIndex3;
		public double weight3;
	};

	//The results of the custom tangents calculation algoritm differ from FBX SDK - usually perpendicular to FBX SDK tangents
	enum NormalsAndTangentsLoadOptions { OnlyFromFile, FromFileIfPresentOrCalculate, AlwaysCalculate /*, FromFileIfPresentOrCalculateByFbxSdk, AlwaysCalculateByFbxSdk*/ }

	enum TangentsAndNormalsSource { None, FromFile, Calculated, /*CalculatedByFbxSdk*/ }

	[Flags]
	enum ImportPostProcessFlags
	{
		FixInfacingNormals = 0x1,
		SmoothNormals = 0x2,
		SmoothTangents = 0x4,
		FlipUVs = 0x8, //Todo : в данный момент изменяет только UV координаты в vertex, а в Assimp еще делались изменения в transform у текстур для Materials - это пока не сделано.
					   //MergeGeometriesByMaterials = 0x10,
	}

	class ImportOptions
	{
		public NormalsAndTangentsLoadOptions NormalsOptions;
		public NormalsAndTangentsLoadOptions TangentsOptions;
		public ImportPostProcessFlags ImportPostProcessFlags;
	}

	////caches the calculations for sharing between CalculateNormals and CalculateTangents, and can be used for the index calculation
	//class CalcMeshCache
	//{
	//	readonly VertexInfo[] vertices;
	//	public CalcMeshCache( VertexInfo[] vertices )
	//	{
	//		this.vertices = vertices;
	//	}

	//	SpatialSort vertexFinder;
	//	float? positionEpsilon;

	//	// a helper to quickly find locally close vertices among the vertex array
	//	public SpatialSort VertexFinder
	//	{
	//		get
	//		{
	//			if( vertexFinder == null )
	//				vertexFinder = new SpatialSort( vertices );
	//			return vertexFinder;
	//		}
	//	}

	//	public float PositionEpsilon
	//	{
	//		get
	//		{
	//			if( positionEpsilon == null )
	//				positionEpsilon = FbxMath.ComputePositionEpsilon( vertices );
	//			return positionEpsilon.Value;
	//		}
	//	}
	//}
}
