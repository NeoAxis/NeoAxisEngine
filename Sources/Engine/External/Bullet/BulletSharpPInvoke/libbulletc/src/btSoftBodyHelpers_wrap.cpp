#include <BulletSoftBody/btSoftBodyHelpers.h>

#include "conversion.h"
#include "btSoftBodyHelpers_wrap.h"

#ifndef BULLETC_DISABLE_SOFTBODY_HELPERS
float btSoftBodyHelpers_CalculateUV(int resx, int resy, int ix, int iy, int id)
{
	return btSoftBodyHelpers::CalculateUV(resx, resy, ix, iy, id);
}

btSoftBody* btSoftBodyHelpers_CreateEllipsoid(btSoftBodyWorldInfo* worldInfo, const btVector3* center,
	const btVector3* radius, int res)
{
	BTVECTOR3_IN(center);
	BTVECTOR3_IN(radius);
	return btSoftBodyHelpers::CreateEllipsoid(*worldInfo, BTVECTOR3_USE(center),
		BTVECTOR3_USE(radius), res);
}
#endif
btSoftBody* btSoftBodyHelpers_CreateFromConvexHull(btSoftBodyWorldInfo* worldInfo,
	const btScalar* vertices, int nvertices, bool randomizeConstraints)
{
	btVector3* verticesTemp = Vector3ArrayIn(vertices, nvertices);
	btSoftBody* ret = btSoftBodyHelpers::CreateFromConvexHull(*worldInfo, verticesTemp, nvertices, randomizeConstraints);
	delete[] verticesTemp;
	return ret;
}
#ifndef BULLETC_DISABLE_SOFTBODY_HELPERS
btSoftBody* btSoftBodyHelpers_CreateFromTetGenData(btSoftBodyWorldInfo* worldInfo,
	const char* ele, const char* face, const char* node, bool bfacelinks, bool btetralinks,
	bool bfacesfromtetras)
{
	return btSoftBodyHelpers::CreateFromTetGenData(*worldInfo, ele, face, node, bfacelinks,
		btetralinks, bfacesfromtetras);
}

btSoftBody* btSoftBodyHelpers_CreateFromTriMesh(btSoftBodyWorldInfo* worldInfo,
	const btScalar* vertices, const int* triangles, int ntriangles, bool randomizeConstraints)
{
	return btSoftBodyHelpers::CreateFromTriMesh(*worldInfo, vertices, triangles,
		ntriangles, randomizeConstraints);
}

btSoftBody* btSoftBodyHelpers_CreatePatch(btSoftBodyWorldInfo* worldInfo, const btVector3* corner00,
	const btVector3* corner10, const btVector3* corner01, const btVector3* corner11,
	int resx, int resy, int fixeds, bool gendiags)
{
	BTVECTOR3_IN(corner00);
	BTVECTOR3_IN(corner10);
	BTVECTOR3_IN(corner01);
	BTVECTOR3_IN(corner11);
	return btSoftBodyHelpers::CreatePatch(*worldInfo, BTVECTOR3_USE(corner00), BTVECTOR3_USE(corner10),
		BTVECTOR3_USE(corner01), BTVECTOR3_USE(corner11), resx, resy, fixeds, gendiags);
}
#endif
btSoftBody* btSoftBodyHelpers_CreatePatchUV(btSoftBodyWorldInfo* worldInfo, const btVector3* corner00,
	const btVector3* corner10, const btVector3* corner01, const btVector3* corner11,
	int resx, int resy, int fixeds, bool gendiags, float* tex_coords)
{
	BTVECTOR3_IN(corner00);
	BTVECTOR3_IN(corner10);
	BTVECTOR3_IN(corner01);
	BTVECTOR3_IN(corner11);
	return btSoftBodyHelpers::CreatePatchUV(*worldInfo, BTVECTOR3_USE(corner00),
		BTVECTOR3_USE(corner10), BTVECTOR3_USE(corner01), BTVECTOR3_USE(corner11),
		resx, resy, fixeds, gendiags, tex_coords);
}
#ifndef BULLETC_DISABLE_SOFTBODY_HELPERS
btSoftBody* btSoftBodyHelpers_CreateRope(btSoftBodyWorldInfo* worldInfo, const btVector3* from,
	const btVector3* to, int res, int fixeds)
{
	BTVECTOR3_IN(from);
	BTVECTOR3_IN(to);
	return btSoftBodyHelpers::CreateRope(*worldInfo, BTVECTOR3_USE(from), BTVECTOR3_USE(to),
		res, fixeds);
}
#endif
void btSoftBodyHelpers_Draw(btSoftBody* psb, btIDebugDraw* idraw, int drawflags)
{
	btSoftBodyHelpers::Draw(psb, idraw, drawflags);
}

void btSoftBodyHelpers_DrawClusterTree(btSoftBody* psb, btIDebugDraw* idraw, int mindepth,
	int maxdepth)
{
	btSoftBodyHelpers::DrawClusterTree(psb, idraw, mindepth, maxdepth);
}

void btSoftBodyHelpers_DrawFaceTree(btSoftBody* psb, btIDebugDraw* idraw, int mindepth,
	int maxdepth)
{
	btSoftBodyHelpers::DrawFaceTree(psb, idraw, mindepth, maxdepth);
}

void btSoftBodyHelpers_DrawFrame(btSoftBody* psb, btIDebugDraw* idraw)
{
	btSoftBodyHelpers::DrawFrame(psb, idraw);
}

void btSoftBodyHelpers_DrawInfos(btSoftBody* psb, btIDebugDraw* idraw, bool masses,
	bool areas, bool stress)
{
	btSoftBodyHelpers::DrawInfos(psb, idraw, masses, areas, stress);
}

void btSoftBodyHelpers_DrawNodeTree(btSoftBody* psb, btIDebugDraw* idraw, int mindepth,
	int maxdepth)
{
	btSoftBodyHelpers::DrawNodeTree(psb, idraw, mindepth, maxdepth);
}
#ifndef BULLETC_DISABLE_SOFTBODY_HELPERS
void btSoftBodyHelpers_ReoptimizeLinkOrder(btSoftBody* psb)
{
	btSoftBodyHelpers::ReoptimizeLinkOrder(psb);
}
#endif
