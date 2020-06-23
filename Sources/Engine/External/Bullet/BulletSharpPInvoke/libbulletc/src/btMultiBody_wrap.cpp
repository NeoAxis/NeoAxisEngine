#include <BulletDynamics/Featherstone/btMultiBody.h>
#include <BulletDynamics/Featherstone/btMultiBodyLinkCollider.h>
#include <LinearMath/btSerializer.h>

#include "conversion.h"
#include "btMultiBody_wrap.h"

btMultiBody* btMultiBody_new(int n_links, btScalar mass, const btVector3* inertia,
	bool fixedBase, bool canSleep)
{
	BTVECTOR3_IN(inertia);
	return new btMultiBody(n_links, mass, BTVECTOR3_USE(inertia), fixedBase, canSleep);
}

btMultiBody* btMultiBody_new2(int n_links, btScalar mass, const btVector3* inertia,
	bool fixedBase, bool canSleep, bool deprecatedMultiDof)
{
	BTVECTOR3_IN(inertia);
	return new btMultiBody(n_links, mass, BTVECTOR3_USE(inertia), fixedBase, canSleep,
		deprecatedMultiDof);
}

void btMultiBody_addBaseConstraintForce(btMultiBody* obj, const btVector3* f)
{
	BTVECTOR3_IN(f);
	obj->addBaseConstraintForce(BTVECTOR3_USE(f));
}

void btMultiBody_addBaseConstraintTorque(btMultiBody* obj, const btVector3* t)
{
	BTVECTOR3_IN(t);
	obj->addBaseConstraintTorque(BTVECTOR3_USE(t));
}

void btMultiBody_addBaseForce(btMultiBody* obj, const btVector3* f)
{
	BTVECTOR3_IN(f);
	obj->addBaseForce(BTVECTOR3_USE(f));
}

void btMultiBody_addBaseTorque(btMultiBody* obj, const btVector3* t)
{
	BTVECTOR3_IN(t);
	obj->addBaseTorque(BTVECTOR3_USE(t));
}

void btMultiBody_addJointTorque(btMultiBody* obj, int i, btScalar Q)
{
	obj->addJointTorque(i, Q);
}

void btMultiBody_addJointTorqueMultiDof(btMultiBody* obj, int i, const btScalar* Q)
{
	obj->addJointTorqueMultiDof(i, Q);
}

void btMultiBody_addJointTorqueMultiDof2(btMultiBody* obj, int i, int dof, btScalar Q)
{
	obj->addJointTorqueMultiDof(i, dof, Q);
}

void btMultiBody_addLinkConstraintForce(btMultiBody* obj, int i, const btVector3* f)
{
	BTVECTOR3_IN(f);
	obj->addLinkConstraintForce(i, BTVECTOR3_USE(f));
}

void btMultiBody_addLinkConstraintTorque(btMultiBody* obj, int i, const btVector3* t)
{
	BTVECTOR3_IN(t);
	obj->addLinkConstraintTorque(i, BTVECTOR3_USE(t));
}

void btMultiBody_addLinkForce(btMultiBody* obj, int i, const btVector3* f)
{
	BTVECTOR3_IN(f);
	obj->addLinkForce(i, BTVECTOR3_USE(f));
}

void btMultiBody_addLinkTorque(btMultiBody* obj, int i, const btVector3* t)
{
	BTVECTOR3_IN(t);
	obj->addLinkTorque(i, BTVECTOR3_USE(t));
}

void btMultiBody_applyDeltaVeeMultiDof(btMultiBody* obj, const btScalar* delta_vee,
	btScalar multiplier)
{
	obj->applyDeltaVeeMultiDof(delta_vee, multiplier);
}

void btMultiBody_applyDeltaVeeMultiDof2(btMultiBody* obj, const btScalar* delta_vee,
	btScalar multiplier)
{
	obj->applyDeltaVeeMultiDof2(delta_vee, multiplier);
}

void btMultiBody_calcAccelerationDeltasMultiDof(btMultiBody* obj, const btScalar* force,
	btScalar* output, btAlignedObjectArray_btScalar* scratch_r, btAlignedObjectArray_btVector3* scratch_v)
{
	obj->calcAccelerationDeltasMultiDof(force, output, *scratch_r, *scratch_v);
}

int btMultiBody_calculateSerializeBufferSize(btMultiBody* obj)
{
	return obj->calculateSerializeBufferSize();
}

void btMultiBody_checkMotionAndSleepIfRequired(btMultiBody* obj, btScalar timestep)
{
	obj->checkMotionAndSleepIfRequired(timestep);
}

void btMultiBody_clearConstraintForces(btMultiBody* obj)
{
	obj->clearConstraintForces();
}

void btMultiBody_clearForcesAndTorques(btMultiBody* obj)
{
	obj->clearForcesAndTorques();
}

void btMultiBody_clearVelocities(btMultiBody* obj)
{
	obj->clearVelocities();
}

void btMultiBody_computeAccelerationsArticulatedBodyAlgorithmMultiDof(btMultiBody* obj,
	btScalar dt, btAlignedObjectArray_btScalar* scratch_r, btAlignedObjectArray_btVector3* scratch_v,
	btAlignedObjectArray_btMatrix3x3* scratch_m, bool isConstraintPass)
{
	obj->computeAccelerationsArticulatedBodyAlgorithmMultiDof(dt, *scratch_r, *scratch_v,
		*scratch_m, isConstraintPass);
}

void btMultiBody_fillConstraintJacobianMultiDof(btMultiBody* obj, int link, const btVector3* contact_point,
	const btVector3* normal_ang, const btVector3* normal_lin, btScalar* jac, btAlignedObjectArray_btScalar* scratch_r,
	btAlignedObjectArray_btVector3* scratch_v, btAlignedObjectArray_btMatrix3x3* scratch_m)
{
	BTVECTOR3_IN(contact_point);
	BTVECTOR3_IN(normal_ang);
	BTVECTOR3_IN(normal_lin);
	obj->fillConstraintJacobianMultiDof(link, BTVECTOR3_USE(contact_point), BTVECTOR3_USE(normal_ang),
		BTVECTOR3_USE(normal_lin), jac, *scratch_r, *scratch_v, *scratch_m);
}

void btMultiBody_fillContactJacobianMultiDof(btMultiBody* obj, int link, const btVector3* contact_point,
	const btVector3* normal, btScalar* jac, btAlignedObjectArray_btScalar* scratch_r,
	btAlignedObjectArray_btVector3* scratch_v, btAlignedObjectArray_btMatrix3x3* scratch_m)
{
	BTVECTOR3_IN(contact_point);
	BTVECTOR3_IN(normal);
	obj->fillContactJacobianMultiDof(link, BTVECTOR3_USE(contact_point), BTVECTOR3_USE(normal),
		jac, *scratch_r, *scratch_v, *scratch_m);
}

void btMultiBody_finalizeMultiDof(btMultiBody* obj)
{
	obj->finalizeMultiDof();
}

void btMultiBody_forwardKinematics(btMultiBody* obj, btAlignedObjectArray_btQuaternion* scratch_q,
	btAlignedObjectArray_btVector3* scratch_m)
{
	obj->forwardKinematics(*scratch_q, *scratch_m);
}

btScalar btMultiBody_getAngularDamping(btMultiBody* obj)
{
	return obj->getAngularDamping();
}

void btMultiBody_getAngularMomentum(btMultiBody* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getAngularMomentum();
	BTVECTOR3_SET(value, temp);
}

btMultiBodyLinkCollider* btMultiBody_getBaseCollider(btMultiBody* obj)
{
	return obj->getBaseCollider();
}

void btMultiBody_getBaseForce(btMultiBody* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getBaseForce();
	BTVECTOR3_SET(value, temp);
}

void btMultiBody_getBaseInertia(btMultiBody* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getBaseInertia();
	BTVECTOR3_SET(value, temp);
}

btScalar btMultiBody_getBaseMass(btMultiBody* obj)
{
	return obj->getBaseMass();
}

const char* btMultiBody_getBaseName(btMultiBody* obj)
{
	return obj->getBaseName();
}

void btMultiBody_getBaseOmega(btMultiBody* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getBaseOmega();
	BTVECTOR3_SET(value, temp);
}

void btMultiBody_getBasePos(btMultiBody* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getBasePos();
	BTVECTOR3_SET(value, temp);
}

void btMultiBody_getBaseTorque(btMultiBody* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getBaseTorque();
	BTVECTOR3_SET(value, temp);
}

void btMultiBody_getBaseVel(btMultiBody* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getBaseVel();
	BTVECTOR3_SET(value, temp);
}

void btMultiBody_getBaseWorldTransform(btMultiBody* obj, btTransform* value)
{
	ATTRIBUTE_ALIGNED16(btTransform) temp = obj->getBaseWorldTransform();
	BTTRANSFORM_SET(value, temp);
}

bool btMultiBody_getCanSleep(btMultiBody* obj)
{
	return obj->getCanSleep();
}

int btMultiBody_getCompanionId(btMultiBody* obj)
{
	return obj->getCompanionId();
}

btScalar btMultiBody_getJointPos(btMultiBody* obj, int i)
{
	return obj->getJointPos(i);
}

btScalar* btMultiBody_getJointPosMultiDof(btMultiBody* obj, int i)
{
	return obj->getJointPosMultiDof(i);
}

btScalar btMultiBody_getJointTorque(btMultiBody* obj, int i)
{
	return obj->getJointTorque(i);
}

btScalar* btMultiBody_getJointTorqueMultiDof(btMultiBody* obj, int i)
{
	return obj->getJointTorqueMultiDof(i);
}

btScalar btMultiBody_getJointVel(btMultiBody* obj, int i)
{
	return obj->getJointVel(i);
}

btScalar* btMultiBody_getJointVelMultiDof(btMultiBody* obj, int i)
{
	return obj->getJointVelMultiDof(i);
}

btScalar btMultiBody_getKineticEnergy(btMultiBody* obj)
{
	return obj->getKineticEnergy();
}

btScalar btMultiBody_getLinearDamping(btMultiBody* obj)
{
	return obj->getLinearDamping();
}

btMultibodyLink* btMultiBody_getLink(btMultiBody* obj, int index)
{
	return &obj->getLink(index);
}

void btMultiBody_getLinkForce(btMultiBody* obj, int i, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getLinkForce(i));
}

void btMultiBody_getLinkInertia(btMultiBody* obj, int i, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getLinkInertia(i));
}

btScalar btMultiBody_getLinkMass(btMultiBody* obj, int i)
{
	return obj->getLinkMass(i);
}

void btMultiBody_getLinkTorque(btMultiBody* obj, int i, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getLinkTorque(i));
}

btScalar btMultiBody_getMaxAppliedImpulse(btMultiBody* obj)
{
	return obj->getMaxAppliedImpulse();
}

btScalar btMultiBody_getMaxCoordinateVelocity(btMultiBody* obj)
{
	return obj->getMaxCoordinateVelocity();
}

int btMultiBody_getNumDofs(btMultiBody* obj)
{
	return obj->getNumDofs();
}

int btMultiBody_getNumLinks(btMultiBody* obj)
{
	return obj->getNumLinks();
}

int btMultiBody_getNumPosVars(btMultiBody* obj)
{
	return obj->getNumPosVars();
}

int btMultiBody_getParent(btMultiBody* obj, int link_num)
{
	return obj->getParent(link_num);
}

void btMultiBody_getParentToLocalRot(btMultiBody* obj, int i, btQuaternion* value)
{
	BTQUATERNION_COPY(value, &obj->getParentToLocalRot(i));
}

void btMultiBody_getRVector(btMultiBody* obj, int i, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getRVector(i));
}

bool btMultiBody_getUseGyroTerm(btMultiBody* obj)
{
	return obj->getUseGyroTerm();
}

int btMultiBody_getUserIndex(btMultiBody* obj)
{
	return obj->getUserIndex();
}

int btMultiBody_getUserIndex2(btMultiBody* obj)
{
	return obj->getUserIndex2();
}

void* btMultiBody_getUserPointer(btMultiBody* obj)
{
	return obj->getUserPointer();
}

const btScalar* btMultiBody_getVelocityVector(btMultiBody* obj)
{
	return obj->getVelocityVector();
}

void btMultiBody_getWorldToBaseRot(btMultiBody* obj, btQuaternion* value)
{
	BTQUATERNION_COPY(value, &obj->getWorldToBaseRot());
}

void btMultiBody_goToSleep(btMultiBody* obj)
{
	obj->goToSleep();
}

bool btMultiBody_hasFixedBase(btMultiBody* obj)
{
	return obj->hasFixedBase();
}

bool btMultiBody_hasSelfCollision(btMultiBody* obj)
{
	return obj->hasSelfCollision();
}

bool btMultiBody_internalNeedsJointFeedback(btMultiBody* obj)
{
	return obj->internalNeedsJointFeedback();
}

bool btMultiBody_isAwake(btMultiBody* obj)
{
	return obj->isAwake();
}

bool btMultiBody_isPosUpdated(btMultiBody* obj)
{
	return obj->isPosUpdated();
}

bool btMultiBody_isUsingGlobalVelocities(btMultiBody* obj)
{
	return obj->isUsingGlobalVelocities();
}

bool btMultiBody_isUsingRK4Integration(btMultiBody* obj)
{
	return obj->isUsingRK4Integration();
}

void btMultiBody_localDirToWorld(btMultiBody* obj, int i, const btVector3* vec, btVector3* value)
{
	BTVECTOR3_IN(vec);
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->localDirToWorld(i, BTVECTOR3_USE(vec));
	BTVECTOR3_SET(value, temp);
}

void btMultiBody_localFrameToWorld(btMultiBody* obj, int i, const btMatrix3x3* mat,
	btMatrix3x3* value)
{
	BTMATRIX3X3_IN(mat);
	ATTRIBUTE_ALIGNED16(btMatrix3x3) temp = obj->localFrameToWorld(i, BTMATRIX3X3_USE(mat));
	BTMATRIX3X3_OUT(value, &temp);
}

void btMultiBody_localPosToWorld(btMultiBody* obj, int i, const btVector3* vec, btVector3* value)
{
	BTVECTOR3_IN(vec);
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->localPosToWorld(i, BTVECTOR3_USE(vec));
	BTVECTOR3_SET(value, temp);
}

void btMultiBody_processDeltaVeeMultiDof2(btMultiBody* obj)
{
	obj->processDeltaVeeMultiDof2();
}

const char* btMultiBody_serialize(btMultiBody* obj, void* dataBuffer, btSerializer* serializer)
{
	return obj->serialize(dataBuffer, serializer);
}

void btMultiBody_setAngularDamping(btMultiBody* obj, btScalar damp)
{
	obj->setAngularDamping(damp);
}

void btMultiBody_setBaseCollider(btMultiBody* obj, btMultiBodyLinkCollider* collider)
{
	obj->setBaseCollider(collider);
}

void btMultiBody_setBaseInertia(btMultiBody* obj, const btVector3* inertia)
{
	BTVECTOR3_IN(inertia);
	obj->setBaseInertia(BTVECTOR3_USE(inertia));
}

void btMultiBody_setBaseMass(btMultiBody* obj, btScalar mass)
{
	obj->setBaseMass(mass);
}

void btMultiBody_setBaseName(btMultiBody* obj, const char* name)
{
	obj->setBaseName(name);
}

void btMultiBody_setBaseOmega(btMultiBody* obj, const btVector3* omega)
{
	BTVECTOR3_IN(omega);
	obj->setBaseOmega(BTVECTOR3_USE(omega));
}

void btMultiBody_setBasePos(btMultiBody* obj, const btVector3* pos)
{
	BTVECTOR3_IN(pos);
	obj->setBasePos(BTVECTOR3_USE(pos));
}

void btMultiBody_setBaseVel(btMultiBody* obj, const btVector3* vel)
{
	BTVECTOR3_IN(vel);
	obj->setBaseVel(BTVECTOR3_USE(vel));
}

void btMultiBody_setBaseWorldTransform(btMultiBody* obj, const btTransform* tr)
{
	BTTRANSFORM_IN(tr);
	obj->setBaseWorldTransform(BTTRANSFORM_USE(tr));
}

void btMultiBody_setCanSleep(btMultiBody* obj, bool canSleep)
{
	obj->setCanSleep(canSleep);
}

void btMultiBody_setCompanionId(btMultiBody* obj, int id)
{
	obj->setCompanionId(id);
}

void btMultiBody_setHasSelfCollision(btMultiBody* obj, bool hasSelfCollision)
{
	obj->setHasSelfCollision(hasSelfCollision);
}

void btMultiBody_setJointPos(btMultiBody* obj, int i, btScalar q)
{
	obj->setJointPos(i, q);
}

void btMultiBody_setJointPosMultiDof(btMultiBody* obj, int i, btScalar* q)
{
	obj->setJointPosMultiDof(i, q);
}

void btMultiBody_setJointVel(btMultiBody* obj, int i, btScalar qdot)
{
	obj->setJointVel(i, qdot);
}

void btMultiBody_setJointVelMultiDof(btMultiBody* obj, int i, btScalar* qdot)
{
	obj->setJointVelMultiDof(i, qdot);
}

void btMultiBody_setLinearDamping(btMultiBody* obj, btScalar damp)
{
	obj->setLinearDamping(damp);
}

void btMultiBody_setMaxAppliedImpulse(btMultiBody* obj, btScalar maxImp)
{
	obj->setMaxAppliedImpulse(maxImp);
}

void btMultiBody_setMaxCoordinateVelocity(btMultiBody* obj, btScalar maxVel)
{
	obj->setMaxCoordinateVelocity(maxVel);
}

void btMultiBody_setNumLinks(btMultiBody* obj, int numLinks)
{
	obj->setNumLinks(numLinks);
}

void btMultiBody_setPosUpdated(btMultiBody* obj, bool updated)
{
	obj->setPosUpdated(updated);
}

void btMultiBody_setupFixed(btMultiBody* obj, int linkIndex, btScalar mass, const btVector3* inertia,
	int parent, const btQuaternion* rotParentToThis, const btVector3* parentComToThisPivotOffset,
	const btVector3* thisPivotToThisComOffset, bool deprecatedDisableParentCollision)
{
	BTVECTOR3_IN(inertia);
	BTQUATERNION_IN(rotParentToThis);
	BTVECTOR3_IN(parentComToThisPivotOffset);
	BTVECTOR3_IN(thisPivotToThisComOffset);
	obj->setupFixed(linkIndex, mass, BTVECTOR3_USE(inertia), parent, BTQUATERNION_USE(rotParentToThis),
		BTVECTOR3_USE(parentComToThisPivotOffset), BTVECTOR3_USE(thisPivotToThisComOffset),
		deprecatedDisableParentCollision);
}

void btMultiBody_setupPlanar(btMultiBody* obj, int i, btScalar mass, const btVector3* inertia,
	int parent, const btQuaternion* rotParentToThis, const btVector3* rotationAxis,
	const btVector3* parentComToThisComOffset, bool disableParentCollision)
{
	BTVECTOR3_IN(inertia);
	BTQUATERNION_IN(rotParentToThis);
	BTVECTOR3_IN(rotationAxis);
	BTVECTOR3_IN(parentComToThisComOffset);
	obj->setupPlanar(i, mass, BTVECTOR3_USE(inertia), parent, BTQUATERNION_USE(rotParentToThis),
		BTVECTOR3_USE(rotationAxis), BTVECTOR3_USE(parentComToThisComOffset), disableParentCollision);
}

void btMultiBody_setupPrismatic(btMultiBody* obj, int i, btScalar mass, const btVector3* inertia,
	int parent, const btQuaternion* rotParentToThis, const btVector3* jointAxis, const btVector3* parentComToThisPivotOffset,
	const btVector3* thisPivotToThisComOffset, bool disableParentCollision)
{
	BTVECTOR3_IN(inertia);
	BTQUATERNION_IN(rotParentToThis);
	BTVECTOR3_IN(jointAxis);
	BTVECTOR3_IN(parentComToThisPivotOffset);
	BTVECTOR3_IN(thisPivotToThisComOffset);
	obj->setupPrismatic(i, mass, BTVECTOR3_USE(inertia), parent, BTQUATERNION_USE(rotParentToThis),
		BTVECTOR3_USE(jointAxis), BTVECTOR3_USE(parentComToThisPivotOffset), BTVECTOR3_USE(thisPivotToThisComOffset),
		disableParentCollision);
}

void btMultiBody_setupRevolute(btMultiBody* obj, int linkIndex, btScalar mass, const btVector3* inertia,
	int parentIndex, const btQuaternion* rotParentToThis, const btVector3* jointAxis,
	const btVector3* parentComToThisPivotOffset, const btVector3* thisPivotToThisComOffset,
	bool disableParentCollision)
{
	BTVECTOR3_IN(inertia);
	BTQUATERNION_IN(rotParentToThis);
	BTVECTOR3_IN(jointAxis);
	BTVECTOR3_IN(parentComToThisPivotOffset);
	BTVECTOR3_IN(thisPivotToThisComOffset);
	obj->setupRevolute(linkIndex, mass, BTVECTOR3_USE(inertia), parentIndex, BTQUATERNION_USE(rotParentToThis),
		BTVECTOR3_USE(jointAxis), BTVECTOR3_USE(parentComToThisPivotOffset), BTVECTOR3_USE(thisPivotToThisComOffset),
		disableParentCollision);
}

void btMultiBody_setupSpherical(btMultiBody* obj, int linkIndex, btScalar mass,
	const btVector3* inertia, int parent, const btQuaternion* rotParentToThis, const btVector3* parentComToThisPivotOffset,
	const btVector3* thisPivotToThisComOffset, bool disableParentCollision)
{
	BTVECTOR3_IN(inertia);
	BTQUATERNION_IN(rotParentToThis);
	BTVECTOR3_IN(parentComToThisPivotOffset);
	BTVECTOR3_IN(thisPivotToThisComOffset);
	obj->setupSpherical(linkIndex, mass, BTVECTOR3_USE(inertia), parent, BTQUATERNION_USE(rotParentToThis),
		BTVECTOR3_USE(parentComToThisPivotOffset), BTVECTOR3_USE(thisPivotToThisComOffset),
		disableParentCollision);
}

void btMultiBody_setUseGyroTerm(btMultiBody* obj, bool useGyro)
{
	obj->setUseGyroTerm(useGyro);
}

void btMultiBody_setUserIndex(btMultiBody* obj, int index)
{
	obj->setUserIndex(index);
}

void btMultiBody_setUserIndex2(btMultiBody* obj, int index)
{
	obj->setUserIndex2(index);
}

void btMultiBody_setUserPointer(btMultiBody* obj, void* userPointer)
{
	obj->setUserPointer(userPointer);
}

void btMultiBody_setWorldToBaseRot(btMultiBody* obj, const btQuaternion* rot)
{
	BTQUATERNION_IN(rot);
	obj->setWorldToBaseRot(BTQUATERNION_USE(rot));
}

void btMultiBody_stepPositionsMultiDof(btMultiBody* obj, btScalar dt, btScalar* pq,
	btScalar* pqd)
{
	obj->stepPositionsMultiDof(dt, pq, pqd);
}

void btMultiBody_stepVelocitiesMultiDof(btMultiBody* obj, btScalar dt, btAlignedObjectArray_btScalar* scratch_r,
	btAlignedObjectArray_btVector3* scratch_v, btAlignedObjectArray_btMatrix3x3* scratch_m,
	bool isConstraintPass)
{
	obj->stepVelocitiesMultiDof(dt, *scratch_r, *scratch_v, *scratch_m, isConstraintPass);
}

void btMultiBody_updateCollisionObjectWorldTransforms(btMultiBody* obj, btAlignedObjectArray_btQuaternion* scratch_q,
	btAlignedObjectArray_btVector3* scratch_m)
{
	obj->updateCollisionObjectWorldTransforms(*scratch_q, *scratch_m);
}

void btMultiBody_useGlobalVelocities(btMultiBody* obj, bool use)
{
	obj->useGlobalVelocities(use);
}

void btMultiBody_useRK4Integration(btMultiBody* obj, bool use)
{
	obj->useRK4Integration(use);
}

void btMultiBody_wakeUp(btMultiBody* obj)
{
	obj->wakeUp();
}

void btMultiBody_worldDirToLocal(btMultiBody* obj, int i, const btVector3* vec, btVector3* value)
{
	BTVECTOR3_IN(vec);
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->worldDirToLocal(i, BTVECTOR3_USE(vec));
	BTVECTOR3_SET(value, temp);
}

void btMultiBody_worldPosToLocal(btMultiBody* obj, int i, const btVector3* vec, btVector3* value)
{
	BTVECTOR3_IN(vec);
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->worldPosToLocal(i, BTVECTOR3_USE(vec));
	BTVECTOR3_SET(value, temp);
}

void btMultiBody_delete(btMultiBody* obj)
{
	delete obj;
}
