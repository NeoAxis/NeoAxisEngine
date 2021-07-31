#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btDefaultMotionState* btDefaultMotionState_new();
	EXPORT btDefaultMotionState* btDefaultMotionState_new2(const btTransform* startTrans);
	EXPORT btDefaultMotionState* btDefaultMotionState_new3(const btTransform* startTrans, const btTransform* centerOfMassOffset);
	EXPORT void btDefaultMotionState_getCenterOfMassOffset(btDefaultMotionState* obj, btTransform* value);
	EXPORT void btDefaultMotionState_getGraphicsWorldTrans(btDefaultMotionState* obj, btTransform* value);
	EXPORT void btDefaultMotionState_getStartWorldTrans(btDefaultMotionState* obj, btTransform* value);
	EXPORT void* btDefaultMotionState_getUserPointer(btDefaultMotionState* obj);
	EXPORT void btDefaultMotionState_setCenterOfMassOffset(btDefaultMotionState* obj, const btTransform* value);
	EXPORT void btDefaultMotionState_setGraphicsWorldTrans(btDefaultMotionState* obj, const btTransform* value);
	EXPORT void btDefaultMotionState_setStartWorldTrans(btDefaultMotionState* obj, const btTransform* value);
	EXPORT void btDefaultMotionState_setUserPointer(btDefaultMotionState* obj, void* value);
#ifdef __cplusplus
}
#endif
