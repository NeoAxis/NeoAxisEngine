#include <LinearMath/btDefaultMotionState.h>

#include "conversion.h"
#include "btDefaultMotionState_wrap.h"

btDefaultMotionState* btDefaultMotionState_new()
{
	return ALIGNED_NEW(btDefaultMotionState)();
}

btDefaultMotionState* btDefaultMotionState_new2(const btTransform* startTrans)
{
	BTTRANSFORM_IN(startTrans);
	return ALIGNED_NEW(btDefaultMotionState)(BTTRANSFORM_USE(startTrans));
}

btDefaultMotionState* btDefaultMotionState_new3(const btTransform* startTrans, const btTransform* centerOfMassOffset)
{
	BTTRANSFORM_IN(startTrans);
	BTTRANSFORM_IN(centerOfMassOffset);
	return ALIGNED_NEW(btDefaultMotionState)(BTTRANSFORM_USE(startTrans), BTTRANSFORM_USE(centerOfMassOffset));
}

void btDefaultMotionState_getCenterOfMassOffset(btDefaultMotionState* obj, btTransform* value)
{
	BTTRANSFORM_SET(value, obj->m_centerOfMassOffset);
}

void btDefaultMotionState_getGraphicsWorldTrans(btDefaultMotionState* obj, btTransform* value)
{
	BTTRANSFORM_SET(value, obj->m_graphicsWorldTrans);
}

void btDefaultMotionState_getStartWorldTrans(btDefaultMotionState* obj, btTransform* value)
{
	BTTRANSFORM_SET(value, obj->m_startWorldTrans);
}

void* btDefaultMotionState_getUserPointer(btDefaultMotionState* obj)
{
	return obj->m_userPointer;
}

void btDefaultMotionState_setCenterOfMassOffset(btDefaultMotionState* obj, const btTransform* value)
{
	BTTRANSFORM_COPY(&obj->m_centerOfMassOffset, value);
}

void btDefaultMotionState_setGraphicsWorldTrans(btDefaultMotionState* obj, const btTransform* value)
{
	BTTRANSFORM_COPY(&obj->m_graphicsWorldTrans, value);
}

void btDefaultMotionState_setStartWorldTrans(btDefaultMotionState* obj, const btTransform* value)
{
	BTTRANSFORM_COPY(&obj->m_startWorldTrans, value);
}

void btDefaultMotionState_setUserPointer(btDefaultMotionState* obj, void* value)
{
	obj->m_userPointer = value;
}
