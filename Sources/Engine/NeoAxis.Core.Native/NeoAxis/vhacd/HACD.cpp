// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#include "VHACD.h"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////


EXPORT VHACD::IVHACD* VHACD_Compute64(const double* const points, const uint32_t pointCount, const uint32_t* const triangles, const uint32_t triangleCount,
	int maxConvexHulls,//Maximum number of convex hulls to produce
	int maxConvexTriangles,//Controls the maximum number of triangles per convex-hull (default = 64, range = 4 - 1024)
	double minConvexVolume,//Controls the adaptive sampling of the generated convex-hulls (default=0.0001, range=0.0-0.01)
	bool convexApproximation,//Enable / disable approximation when computing convex-hulls (default = 1)
	int maxResolution,//Maximum number of voxels generated during the voxelization stage (default = 100, 000, range = 10, 000 - 16, 000, 000)
	double maxConcavity,//Maximum allowed concavity (default=0.0025, range=0.0-1.0)
	double alpha,//Controls the bias toward clipping along symmetry planes (default=0.05, range=0.0-1.0)
	double beta,//Controls the bias toward clipping along revolution axes (default=0.05, range=0.0-1.0)
	int planeDownsampling,//Controls the granularity of the search for the \"best\" clipping plane (default=4, range=1-16)
	int hullDownsampling,//Controls the precision of the convex-hull generation process during the clipping plane selection stage (default=4, range=1-16)
	bool normalizeMesh,//Enable / disable normalizing the mesh before applying the convex decomposition (default = 0)
	bool tetrahedronMode//0: voxel-based approximate convex decomposition, 1: tetrahedron-based approximate convex decomposition (default=0)
	)
{
	VHACD::IVHACD::Parameters desc;
	VHACD::IVHACD* myHACD = 0;

	//myHACD = VHACD::CreateVHACD_ASYNC();
	myHACD = VHACD::CreateVHACD();

	desc.Init(maxConvexHulls, maxConvexTriangles, minConvexVolume, convexApproximation, maxResolution, maxConcavity, alpha, beta, planeDownsampling, hullDownsampling, normalizeMesh, tetrahedronMode);

	bool success = myHACD->Compute(points, pointCount, triangles, triangleCount, desc);

	if (!success)
	{
		myHACD->Release();

		return NULL;
	}

	return myHACD;
}

EXPORT VHACD::IVHACD* VHACD_Compute32(const float* const points, const uint32_t pointCount, const uint32_t* const triangles, const uint32_t triangleCount,
	int maxConvexHulls,//Maximum number of convex hulls to produce
	int maxConvexTriangles,//Controls the maximum number of triangles per convex-hull (default = 64, range = 4 - 1024)
	double minConvexVolume,//Controls the adaptive sampling of the generated convex-hulls (default=0.0001, range=0.0-0.01)
	bool convexApproximation,//Enable / disable approximation when computing convex-hulls (default = 1)
	int maxResolution,//Maximum number of voxels generated during the voxelization stage (default = 100, 000, range = 10, 000 - 16, 000, 000)
	double maxConcavity,//Maximum allowed concavity (default=0.0025, range=0.0-1.0)
	double alpha,//Controls the bias toward clipping along symmetry planes (default=0.05, range=0.0-1.0)
	double beta,//Controls the bias toward clipping along revolution axes (default=0.05, range=0.0-1.0)
	int planeDownsampling,//Controls the granularity of the search for the \"best\" clipping plane (default=4, range=1-16)
	int hullDownsampling,//Controls the precision of the convex-hull generation process during the clipping plane selection stage (default=4, range=1-16)
	bool normalizeMesh,//Enable / disable normalizing the mesh before applying the convex decomposition (default = 0)
	bool tetrahedronMode//0: voxel-based approximate convex decomposition, 1: tetrahedron-based approximate convex decomposition (default=0)
)
{
	VHACD::IVHACD::Parameters desc;
	VHACD::IVHACD* myHACD = VHACD::CreateVHACD();
	//VHACD::IVHACD* myHACD = VHACD::CreateVHACD_ASYNC();

	desc.Init(maxConvexHulls, maxConvexTriangles, minConvexVolume, convexApproximation, maxResolution, maxConcavity, alpha, beta, planeDownsampling, hullDownsampling, normalizeMesh, tetrahedronMode);

	bool success = myHACD->Compute(points, pointCount, triangles, triangleCount, desc);

	if (!success)
	{
		myHACD->Release();

		return NULL;
	}

	return myHACD;
}

EXPORT void VHACD_Cancel(VHACD::IVHACD* objHACD)
{
	if (objHACD)
		objHACD->Cancel();
}

EXPORT void VHACD_Delete(VHACD::IVHACD* objHACD)
{
	if (objHACD)
	{
		//objHACD->Clean();
		objHACD->Release();
	}
}

// In synchronous mode (non-multi-threaded) the state is always 'ready'
// In asynchronous mode, this returns true if the background thread is not still actively computing a new solution.
// In an asynchronous config the 'IsReady' call will report any update or log messages in the caller's current thread.
EXPORT bool VHACD_IsReady(VHACD::IVHACD* objHACD)
{
	if (objHACD)
		objHACD->IsReady();
	
	return true;
}

EXPORT int VHACD_GetClusterCount(VHACD::IVHACD* objHACD)
{
	if (objHACD)
		return (int)objHACD->GetNConvexHulls();

	return 0;
}

EXPORT void VHACD_GetBufferSize(VHACD::IVHACD* objHACD, int cluster, int* pointCount, int* triangleCount)
{
	VHACD::IVHACD::ConvexHull hull;

	objHACD->GetConvexHull(cluster, hull);

	*pointCount = (int)hull.m_nPoints;
	*triangleCount = (int)hull.m_nTriangles;
}

// copy buffers
EXPORT void VHACD_GetBuffer64(VHACD::IVHACD* objHACD, int cluster, double* points, int* triangles)
{
	VHACD::IVHACD::ConvexHull hull;

	objHACD->GetConvexHull(cluster, hull);

	memcpy(points, hull.m_points, sizeof(double) * hull.m_nPoints * 3);
	memcpy(triangles, hull.m_triangles, sizeof(int) * hull.m_nTriangles * 3);
}
// copy buffers
EXPORT void VHACD_GetBuffer32(VHACD::IVHACD* objHACD, int cluster, float* points, int* triangles)
{
	VHACD::IVHACD::ConvexHull hull;

	objHACD->GetConvexHull(cluster, hull);

	double* pPtr = hull.m_points;

	for (int i = 0; i < hull.m_nPoints; i++)
	{
		*points++ = (float)*pPtr++;
		*points++ = (float)*pPtr++;
		*points++ = (float)*pPtr++;
	}

	memcpy(triangles, hull.m_triangles, sizeof(int) * hull.m_nTriangles * 3);
}

//EXPORT double* VHACD_GetConvexVertices(VHACD::IVHACD* objHACD, int cluster, int* vertexCount)
//{
//	if (!objHACD)
//	{
//		*vertexCount = 0;
//
//		return 0;
//	}
//
//	VHACD::IVHACD::ConvexHull hull;
//
//	objHACD->GetConvexHull(cluster, hull);
//
//	*vertexCount = hull.m_nPoints;
//
//	return hull.m_points;
//}
//
//EXPORT uint32_t* VHACD_GetConvexFaces(VHACD::IVHACD* objHACD, int cluster, int* triangleCount)
//{
//	if (!objHACD)
//	{
//		*triangleCount = 0;
//
//		return 0;
//	}
//
//	VHACD::IVHACD::ConvexHull hull;
//
//	objHACD->GetConvexHull(cluster, hull);
//
//	*triangleCount = hull.m_nTriangles;
//
//	return hull.m_triangles;
//}

