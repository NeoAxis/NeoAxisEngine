#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btMultiBodyPoint2Point* btMultiBodyPoint2Point_new(btMultiBody* body, int link, btRigidBody* bodyB, const btVector3* pivotInA, const btVector3* pivotInB);
	EXPORT btMultiBodyPoint2Point* btMultiBodyPoint2Point_new2(btMultiBody* bodyA, int linkA, btMultiBody* bodyB, int linkB, const btVector3* pivotInA, const btVector3* pivotInB);
	EXPORT void btMultiBodyPoint2Point_getPivotInB(btMultiBodyPoint2Point* obj, btVector3* value);
	EXPORT void btMultiBodyPoint2Point_setPivotInB(btMultiBodyPoint2Point* obj, const btVector3* pivotInB);
#ifdef __cplusplus
}
#endif
