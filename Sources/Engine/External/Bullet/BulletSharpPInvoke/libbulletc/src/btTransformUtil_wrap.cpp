#include <LinearMath/btTransformUtil.h>

#include "conversion.h"
#include "btTransformUtil_wrap.h"

void btTransformUtil_calculateDiffAxisAngle(const btTransform* transform0, const btTransform* transform1,
	btVector3* axis, btScalar* angle)
{
	BTTRANSFORM_IN(transform0);
	BTTRANSFORM_IN(transform1);
	BTVECTOR3_DEF(axis);
	btTransformUtil::calculateDiffAxisAngle(BTTRANSFORM_USE(transform0), BTTRANSFORM_USE(transform1),
		BTVECTOR3_USE(axis), *angle);
	BTVECTOR3_DEF_OUT(axis);
}

void btTransformUtil_calculateDiffAxisAngleQuaternion(const btQuaternion* orn0, const btQuaternion* orn1a,
	btVector3* axis, btScalar* angle)
{
	BTQUATERNION_IN(orn0);
	BTQUATERNION_IN(orn1a);
	BTVECTOR3_DEF(axis);
	btTransformUtil::calculateDiffAxisAngleQuaternion(BTQUATERNION_USE(orn0), BTQUATERNION_USE(orn1a),
		BTVECTOR3_USE(axis), *angle);
	BTVECTOR3_DEF_OUT(axis);
}

void btTransformUtil_calculateVelocity(const btTransform* transform0, const btTransform* transform1,
	btScalar timeStep, btVector3* linVel, btVector3* angVel)
{
	BTTRANSFORM_IN(transform0);
	BTTRANSFORM_IN(transform1);
	BTVECTOR3_DEF(linVel);
	BTVECTOR3_DEF(angVel);
	btTransformUtil::calculateVelocity(BTTRANSFORM_USE(transform0), BTTRANSFORM_USE(transform1),
		timeStep, BTVECTOR3_USE(linVel), BTVECTOR3_USE(angVel));
	BTVECTOR3_DEF_OUT(linVel);
	BTVECTOR3_DEF_OUT(angVel);
}

void btTransformUtil_calculateVelocityQuaternion(const btVector3* pos0, const btVector3* pos1,
	const btQuaternion* orn0, const btQuaternion* orn1, btScalar timeStep, btVector3* linVel,
	btVector3* angVel)
{
	BTVECTOR3_IN(pos0);
	BTVECTOR3_IN(pos1);
	BTQUATERNION_IN(orn0);
	BTQUATERNION_IN(orn1);
	BTVECTOR3_DEF(linVel);
	BTVECTOR3_DEF(angVel);
	btTransformUtil::calculateVelocityQuaternion(BTVECTOR3_USE(pos0), BTVECTOR3_USE(pos1),
		BTQUATERNION_USE(orn0), BTQUATERNION_USE(orn1), timeStep, BTVECTOR3_USE(linVel),
		BTVECTOR3_USE(angVel));
	BTVECTOR3_DEF_OUT(linVel);
	BTVECTOR3_DEF_OUT(angVel);
}

void btTransformUtil_integrateTransform(const btTransform* curTrans, const btVector3* linvel,
	const btVector3* angvel, btScalar timeStep, btTransform* predictedTransform)
{
	BTTRANSFORM_IN(curTrans);
	BTVECTOR3_IN(linvel);
	BTVECTOR3_IN(angvel);
	BTTRANSFORM_DEF(predictedTransform);
	btTransformUtil::integrateTransform(BTTRANSFORM_USE(curTrans), BTVECTOR3_USE(linvel),
		BTVECTOR3_USE(angvel), timeStep, BTTRANSFORM_USE(predictedTransform));
	BTTRANSFORM_DEF_OUT(predictedTransform);
}


btConvexSeparatingDistanceUtil* btConvexSeparatingDistanceUtil_new(btScalar boundingRadiusA,
	btScalar boundingRadiusB)
{
	return new btConvexSeparatingDistanceUtil(boundingRadiusA, boundingRadiusB);
}

btScalar btConvexSeparatingDistanceUtil_getConservativeSeparatingDistance(btConvexSeparatingDistanceUtil* obj)
{
	return obj->getConservativeSeparatingDistance();
}

void btConvexSeparatingDistanceUtil_initSeparatingDistance(btConvexSeparatingDistanceUtil* obj,
	const btVector3* separatingVector, btScalar separatingDistance, const btTransform* transA,
	const btTransform* transB)
{
	BTVECTOR3_IN(separatingVector);
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	obj->initSeparatingDistance(BTVECTOR3_USE(separatingVector), separatingDistance,
		BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB));
}

void btConvexSeparatingDistanceUtil_updateSeparatingDistance(btConvexSeparatingDistanceUtil* obj,
	const btTransform* transA, const btTransform* transB)
{
	BTTRANSFORM_IN(transA);
	BTTRANSFORM_IN(transB);
	obj->updateSeparatingDistance(BTTRANSFORM_USE(transA), BTTRANSFORM_USE(transB));
}

void btConvexSeparatingDistanceUtil_delete(btConvexSeparatingDistanceUtil* obj)
{
	delete obj;
}
