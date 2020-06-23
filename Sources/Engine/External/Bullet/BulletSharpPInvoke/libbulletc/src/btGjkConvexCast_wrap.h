#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btGjkConvexCast* btGjkConvexCast_new(const btConvexShape* convexA, const btConvexShape* convexB, btVoronoiSimplexSolver* simplexSolver);
#ifdef __cplusplus
}
#endif
