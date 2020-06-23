#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
#ifndef BULLETC_DISABLE_SOFTBODY_HELPERS
	EXPORT float btSoftBodyHelpers_CalculateUV(int resx, int resy, int ix, int iy, int id);
	EXPORT btSoftBody* btSoftBodyHelpers_CreateEllipsoid(btSoftBodyWorldInfo* worldInfo, const btVector3* center, const btVector3* radius, int res);
#endif
	EXPORT btSoftBody* btSoftBodyHelpers_CreateFromConvexHull(btSoftBodyWorldInfo* worldInfo, const btScalar* vertices, int nvertices, bool randomizeConstraints);
#ifndef BULLETC_DISABLE_SOFTBODY_HELPERS
	EXPORT btSoftBody* btSoftBodyHelpers_CreateFromTetGenData(btSoftBodyWorldInfo* worldInfo, const char* ele, const char* face, const char* node, bool bfacelinks, bool btetralinks, bool bfacesfromtetras);
	EXPORT btSoftBody* btSoftBodyHelpers_CreateFromTriMesh(btSoftBodyWorldInfo* worldInfo, const btScalar* vertices, const int* triangles, int ntriangles, bool randomizeConstraints);
	EXPORT btSoftBody* btSoftBodyHelpers_CreatePatch(btSoftBodyWorldInfo* worldInfo, const btVector3* corner00, const btVector3* corner10, const btVector3* corner01, const btVector3* corner11, int resx, int resy, int fixeds, bool gendiags);
#endif
	EXPORT btSoftBody* btSoftBodyHelpers_CreatePatchUV(btSoftBodyWorldInfo* worldInfo, const btVector3* corner00, const btVector3* corner10, const btVector3* corner01, const btVector3* corner11, int resx, int resy, int fixeds, bool gendiags, float* tex_coords);
#ifndef BULLETC_DISABLE_SOFTBODY_HELPERS
	EXPORT btSoftBody* btSoftBodyHelpers_CreateRope(btSoftBodyWorldInfo* worldInfo, const btVector3* from, const btVector3* to, int res, int fixeds);
#endif
	EXPORT void btSoftBodyHelpers_Draw(btSoftBody* psb, btIDebugDraw* idraw, int drawflags);
	EXPORT void btSoftBodyHelpers_DrawClusterTree(btSoftBody* psb, btIDebugDraw* idraw, int mindepth, int maxdepth);
	EXPORT void btSoftBodyHelpers_DrawFaceTree(btSoftBody* psb, btIDebugDraw* idraw, int mindepth, int maxdepth);
	EXPORT void btSoftBodyHelpers_DrawFrame(btSoftBody* psb, btIDebugDraw* idraw);
	EXPORT void btSoftBodyHelpers_DrawInfos(btSoftBody* psb, btIDebugDraw* idraw, bool masses, bool areas, bool stress);
	EXPORT void btSoftBodyHelpers_DrawNodeTree(btSoftBody* psb, btIDebugDraw* idraw, int mindepth, int maxdepth);
#ifndef BULLETC_DISABLE_SOFTBODY_HELPERS
	EXPORT void btSoftBodyHelpers_ReoptimizeLinkOrder(btSoftBody* psb);
#endif
#ifdef __cplusplus
}
#endif
