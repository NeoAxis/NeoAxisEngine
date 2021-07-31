#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btBU_Simplex1to4* btBU_Simplex1to4_new();
	EXPORT btBU_Simplex1to4* btBU_Simplex1to4_new2(const btVector3* pt0);
	EXPORT btBU_Simplex1to4* btBU_Simplex1to4_new3(const btVector3* pt0, const btVector3* pt1);
	EXPORT btBU_Simplex1to4* btBU_Simplex1to4_new4(const btVector3* pt0, const btVector3* pt1, const btVector3* pt2);
	EXPORT btBU_Simplex1to4* btBU_Simplex1to4_new5(const btVector3* pt0, const btVector3* pt1, const btVector3* pt2, const btVector3* pt3);
	EXPORT void btBU_Simplex1to4_addVertex(btBU_Simplex1to4* obj, const btVector3* pt);
	EXPORT int btBU_Simplex1to4_getIndex(btBU_Simplex1to4* obj, int i);
	EXPORT void btBU_Simplex1to4_reset(btBU_Simplex1to4* obj);
#ifdef __cplusplus
}
#endif
