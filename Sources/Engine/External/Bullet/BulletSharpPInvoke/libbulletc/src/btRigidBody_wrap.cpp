#include <BulletCollision/CollisionShapes/btCollisionShape.h>
#include <BulletDynamics/ConstraintSolver/btTypedConstraint.h>
#include <BulletDynamics/Dynamics/btRigidBody.h>

#include "conversion.h"
#include "btRigidBody_wrap.h"

btRigidBody_btRigidBodyConstructionInfo* btRigidBody_btRigidBodyConstructionInfo_new(
	btScalar mass, btMotionState* motionState, btCollisionShape* collisionShape)
{
	return  ALIGNED_NEW(btRigidBody::btRigidBodyConstructionInfo)(mass, motionState, collisionShape);
}

btRigidBody_btRigidBodyConstructionInfo* btRigidBody_btRigidBodyConstructionInfo_new2(
	btScalar mass, btMotionState* motionState, btCollisionShape* collisionShape, const btVector3* localInertia)
{
	BTVECTOR3_IN(localInertia);
	return ALIGNED_NEW(btRigidBody::btRigidBodyConstructionInfo)(mass, motionState, collisionShape,
		BTVECTOR3_USE(localInertia));
}

btScalar btRigidBody_btRigidBodyConstructionInfo_getAdditionalAngularDampingFactor(
	btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_additionalAngularDampingFactor;
}

btScalar btRigidBody_btRigidBodyConstructionInfo_getAdditionalAngularDampingThresholdSqr(
	btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_additionalAngularDampingThresholdSqr;
}

bool btRigidBody_btRigidBodyConstructionInfo_getAdditionalDamping(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_additionalDamping;
}

btScalar btRigidBody_btRigidBodyConstructionInfo_getAdditionalDampingFactor(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_additionalDampingFactor;
}

btScalar btRigidBody_btRigidBodyConstructionInfo_getAdditionalLinearDampingThresholdSqr(
	btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_additionalLinearDampingThresholdSqr;
}

btScalar btRigidBody_btRigidBodyConstructionInfo_getAngularDamping(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_angularDamping;
}

btScalar btRigidBody_btRigidBodyConstructionInfo_getAngularSleepingThreshold(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_angularSleepingThreshold;
}

btCollisionShape* btRigidBody_btRigidBodyConstructionInfo_getCollisionShape(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_collisionShape;
}

btScalar btRigidBody_btRigidBodyConstructionInfo_getFriction(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_friction;
}

btScalar btRigidBody_btRigidBodyConstructionInfo_getLinearDamping(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_linearDamping;
}

btScalar btRigidBody_btRigidBodyConstructionInfo_getLinearSleepingThreshold(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_linearSleepingThreshold;
}

void btRigidBody_btRigidBodyConstructionInfo_getLocalInertia(btRigidBody_btRigidBodyConstructionInfo* obj,
	btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->m_localInertia);
}

btScalar btRigidBody_btRigidBodyConstructionInfo_getMass(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_mass;
}

btMotionState* btRigidBody_btRigidBodyConstructionInfo_getMotionState(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_motionState;
}

btScalar btRigidBody_btRigidBodyConstructionInfo_getRestitution(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_restitution;
}

btScalar btRigidBody_btRigidBodyConstructionInfo_getRollingFriction(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	return obj->m_rollingFriction;
}

void btRigidBody_btRigidBodyConstructionInfo_getStartWorldTransform(btRigidBody_btRigidBodyConstructionInfo* obj,
	btTransform* value)
{
	BTTRANSFORM_SET(value, obj->m_startWorldTransform);
}

void btRigidBody_btRigidBodyConstructionInfo_setAdditionalAngularDampingFactor(btRigidBody_btRigidBodyConstructionInfo* obj,
	btScalar value)
{
	obj->m_additionalAngularDampingFactor = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setAdditionalAngularDampingThresholdSqr(
	btRigidBody_btRigidBodyConstructionInfo* obj, btScalar value)
{
	obj->m_additionalAngularDampingThresholdSqr = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setAdditionalDamping(btRigidBody_btRigidBodyConstructionInfo* obj,
	bool value)
{
	obj->m_additionalDamping = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setAdditionalDampingFactor(btRigidBody_btRigidBodyConstructionInfo* obj,
	btScalar value)
{
	obj->m_additionalDampingFactor = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setAdditionalLinearDampingThresholdSqr(
	btRigidBody_btRigidBodyConstructionInfo* obj, btScalar value)
{
	obj->m_additionalLinearDampingThresholdSqr = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setAngularDamping(btRigidBody_btRigidBodyConstructionInfo* obj,
	btScalar value)
{
	obj->m_angularDamping = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setAngularSleepingThreshold(btRigidBody_btRigidBodyConstructionInfo* obj,
	btScalar value)
{
	obj->m_angularSleepingThreshold = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setCollisionShape(btRigidBody_btRigidBodyConstructionInfo* obj,
	btCollisionShape* value)
{
	obj->m_collisionShape = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setFriction(btRigidBody_btRigidBodyConstructionInfo* obj,
	btScalar value)
{
	obj->m_friction = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setLinearDamping(btRigidBody_btRigidBodyConstructionInfo* obj,
	btScalar value)
{
	obj->m_linearDamping = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setLinearSleepingThreshold(btRigidBody_btRigidBodyConstructionInfo* obj,
	btScalar value)
{
	obj->m_linearSleepingThreshold = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setLocalInertia(btRigidBody_btRigidBodyConstructionInfo* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_localInertia, value);
}

void btRigidBody_btRigidBodyConstructionInfo_setMass(btRigidBody_btRigidBodyConstructionInfo* obj,
	btScalar value)
{
	obj->m_mass = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setMotionState(btRigidBody_btRigidBodyConstructionInfo* obj,
	btMotionState* value)
{
	obj->m_motionState = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setRestitution(btRigidBody_btRigidBodyConstructionInfo* obj,
	btScalar value)
{
	obj->m_restitution = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setRollingFriction(btRigidBody_btRigidBodyConstructionInfo* obj,
	btScalar value)
{
	obj->m_rollingFriction = value;
}

void btRigidBody_btRigidBodyConstructionInfo_setStartWorldTransform(btRigidBody_btRigidBodyConstructionInfo* obj,
	const btTransform* value)
{
	BTTRANSFORM_COPY(&obj->m_startWorldTransform, value);
}

void btRigidBody_btRigidBodyConstructionInfo_delete(btRigidBody_btRigidBodyConstructionInfo* obj)
{
	ALIGNED_FREE(obj);
}


btRigidBody* btRigidBody_new(const btRigidBody_btRigidBodyConstructionInfo* constructionInfo)
{
	return new btRigidBody(*constructionInfo);
}

void btRigidBody_addConstraintRef(btRigidBody* obj, btTypedConstraint* c)
{
	obj->addConstraintRef(c);
}

void btRigidBody_applyCentralForce(btRigidBody* obj, const btVector3* force)
{
	BTVECTOR3_IN(force);
	obj->applyCentralForce(BTVECTOR3_USE(force));
}

void btRigidBody_applyCentralImpulse(btRigidBody* obj, const btVector3* impulse)
{
	BTVECTOR3_IN(impulse);
	obj->applyCentralImpulse(BTVECTOR3_USE(impulse));
}

void btRigidBody_applyDamping(btRigidBody* obj, btScalar timeStep)
{
	obj->applyDamping(timeStep);
}

void btRigidBody_applyForce(btRigidBody* obj, const btVector3* force, const btVector3* rel_pos)
{
	BTVECTOR3_IN(force);
	BTVECTOR3_IN(rel_pos);
	obj->applyForce(BTVECTOR3_USE(force), BTVECTOR3_USE(rel_pos));
}

void btRigidBody_applyGravity(btRigidBody* obj)
{
	obj->applyGravity();
}

void btRigidBody_applyImpulse(btRigidBody* obj, const btVector3* impulse, const btVector3* rel_pos)
{
	BTVECTOR3_IN(impulse);
	BTVECTOR3_IN(rel_pos);
	obj->applyImpulse(BTVECTOR3_USE(impulse), BTVECTOR3_USE(rel_pos));
}

void btRigidBody_applyTorque(btRigidBody* obj, const btVector3* torque)
{
	BTVECTOR3_IN(torque);
	obj->applyTorque(BTVECTOR3_USE(torque));
}

void btRigidBody_applyTorqueImpulse(btRigidBody* obj, const btVector3* torque)
{
	BTVECTOR3_IN(torque);
	obj->applyTorqueImpulse(BTVECTOR3_USE(torque));
}

void btRigidBody_clearForces(btRigidBody* obj)
{
	obj->clearForces();
}

btScalar btRigidBody_computeAngularImpulseDenominator(btRigidBody* obj, const btVector3* axis)
{
	BTVECTOR3_IN(axis);
	return obj->computeAngularImpulseDenominator(BTVECTOR3_USE(axis));
}

void btRigidBody_computeGyroscopicForceExplicit(btRigidBody* obj, btScalar maxGyroscopicForce,
	btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->computeGyroscopicForceExplicit(maxGyroscopicForce);
	BTVECTOR3_SET(value, temp);
}

void btRigidBody_computeGyroscopicImpulseImplicit_Body(btRigidBody* obj, btScalar step,
	btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->computeGyroscopicImpulseImplicit_Body(step);
	BTVECTOR3_SET(value, temp);
}

void btRigidBody_computeGyroscopicImpulseImplicit_World(btRigidBody* obj, btScalar dt,
	btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->computeGyroscopicImpulseImplicit_World(dt);
	BTVECTOR3_SET(value, temp);
}

btScalar btRigidBody_computeImpulseDenominator(btRigidBody* obj, const btVector3* pos,
	const btVector3* normal)
{
	BTVECTOR3_IN(pos);
	BTVECTOR3_IN(normal);
	return obj->computeImpulseDenominator(BTVECTOR3_USE(pos), BTVECTOR3_USE(normal));
}

void btRigidBody_getAabb(btRigidBody* obj, btVector3* aabbMin, btVector3* aabbMax)
{
	BTVECTOR3_DEF(aabbMin);
	BTVECTOR3_DEF(aabbMax);
	obj->getAabb(BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
	BTVECTOR3_DEF_OUT(aabbMin);
	BTVECTOR3_DEF_OUT(aabbMax);
}

btScalar btRigidBody_getAngularDamping(btRigidBody* obj)
{
	return obj->getAngularDamping();
}

void btRigidBody_getAngularFactor(btRigidBody* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAngularFactor());
}

btScalar btRigidBody_getAngularSleepingThreshold(btRigidBody* obj)
{
	return obj->getAngularSleepingThreshold();
}

void btRigidBody_getAngularVelocity(btRigidBody* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getAngularVelocity());
}

btBroadphaseProxy* btRigidBody_getBroadphaseProxy(btRigidBody* obj)
{
	return obj->getBroadphaseProxy();
}

void btRigidBody_getCenterOfMassPosition(btRigidBody* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getCenterOfMassPosition());
}

void btRigidBody_getCenterOfMassTransform(btRigidBody* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getCenterOfMassTransform());
}

btTypedConstraint* btRigidBody_getConstraintRef(btRigidBody* obj, int index)
{
	return obj->getConstraintRef(index);
}

int btRigidBody_getContactSolverType(btRigidBody* obj)
{
	return obj->m_contactSolverType;
}

int btRigidBody_getFlags(btRigidBody* obj)
{
	return obj->getFlags();
}

int btRigidBody_getFrictionSolverType(btRigidBody* obj)
{
	return obj->m_frictionSolverType;
}

void btRigidBody_getGravity(btRigidBody* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getGravity());
}

void btRigidBody_getInvInertiaDiagLocal(btRigidBody* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getInvInertiaDiagLocal());
}

void btRigidBody_getInvInertiaTensorWorld(btRigidBody* obj, btMatrix3x3* value)
{
	BTMATRIX3X3_OUT(value, &obj->getInvInertiaTensorWorld());
}

btScalar btRigidBody_getInvMass(btRigidBody* obj)
{
	return obj->getInvMass();
}

btScalar btRigidBody_getLinearDamping(btRigidBody* obj)
{
	return obj->getLinearDamping();
}

void btRigidBody_getLinearFactor(btRigidBody* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getLinearFactor());
}

btScalar btRigidBody_getLinearSleepingThreshold(btRigidBody* obj)
{
	return obj->getLinearSleepingThreshold();
}

void btRigidBody_getLinearVelocity(btRigidBody* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getLinearVelocity());
}

void btRigidBody_getLocalInertia(btRigidBody* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getLocalInertia();
	BTVECTOR3_SET(value, temp);
}

btMotionState* btRigidBody_getMotionState(btRigidBody* obj)
{
	return obj->getMotionState();
}

int btRigidBody_getNumConstraintRefs(btRigidBody* obj)
{
	return obj->getNumConstraintRefs();
}

void btRigidBody_getOrientation(btRigidBody* obj, btQuaternion* value)
{
	ATTRIBUTE_ALIGNED16(btQuaternion) temp = obj->getOrientation();
	BTQUATERNION_SET(value, temp);
}

void btRigidBody_getTotalForce(btRigidBody* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getTotalForce());
}

void btRigidBody_getTotalTorque(btRigidBody* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getTotalTorque());
}

void btRigidBody_getVelocityInLocalPoint(btRigidBody* obj, const btVector3* rel_pos,
	btVector3* value)
{
	BTVECTOR3_IN(rel_pos);
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->getVelocityInLocalPoint(BTVECTOR3_USE(rel_pos));
	BTVECTOR3_SET(value, temp);
}

void btRigidBody_integrateVelocities(btRigidBody* obj, btScalar step)
{
	obj->integrateVelocities(step);
}

bool btRigidBody_isInWorld(btRigidBody* obj)
{
	return obj->isInWorld();
}

void btRigidBody_predictIntegratedTransform(btRigidBody* obj, btScalar step, btTransform* predictedTransform)
{
	BTTRANSFORM_DEF(predictedTransform);
	obj->predictIntegratedTransform(step, BTTRANSFORM_USE(predictedTransform));
	BTTRANSFORM_DEF_OUT(predictedTransform);
}

void btRigidBody_proceedToTransform(btRigidBody* obj, const btTransform* newTrans)
{
	BTTRANSFORM_IN(newTrans);
	obj->proceedToTransform(BTTRANSFORM_USE(newTrans));
}

void btRigidBody_removeConstraintRef(btRigidBody* obj, btTypedConstraint* c)
{
	obj->removeConstraintRef(c);
}

void btRigidBody_saveKinematicState(btRigidBody* obj, btScalar step)
{
	obj->saveKinematicState(step);
}

void btRigidBody_setAngularFactor(btRigidBody* obj, const btVector3* angFac)
{
	BTVECTOR3_IN(angFac);
	obj->setAngularFactor(BTVECTOR3_USE(angFac));
}

void btRigidBody_setAngularFactor2(btRigidBody* obj, btScalar angFac)
{
	obj->setAngularFactor(angFac);
}

void btRigidBody_setAngularVelocity(btRigidBody* obj, const btVector3* ang_vel)
{
	BTVECTOR3_IN(ang_vel);
	obj->setAngularVelocity(BTVECTOR3_USE(ang_vel));
}

void btRigidBody_setCenterOfMassTransform(btRigidBody* obj, const btTransform* xform)
{
	BTTRANSFORM_IN(xform);
	obj->setCenterOfMassTransform(BTTRANSFORM_USE(xform));
}

void btRigidBody_setContactSolverType(btRigidBody* obj, int value)
{
	obj->m_contactSolverType = value;
}

void btRigidBody_setDamping(btRigidBody* obj, btScalar lin_damping, btScalar ang_damping)
{
	obj->setDamping(lin_damping, ang_damping);
}

void btRigidBody_setFlags(btRigidBody* obj, int flags)
{
	obj->setFlags(flags);
}

void btRigidBody_setFrictionSolverType(btRigidBody* obj, int value)
{
	obj->m_frictionSolverType = value;
}

void btRigidBody_setGravity(btRigidBody* obj, const btVector3* acceleration)
{
	BTVECTOR3_IN(acceleration);
	obj->setGravity(BTVECTOR3_USE(acceleration));
}

void btRigidBody_setInvInertiaDiagLocal(btRigidBody* obj, const btVector3* diagInvInertia)
{
	BTVECTOR3_IN(diagInvInertia);
	obj->setInvInertiaDiagLocal(BTVECTOR3_USE(diagInvInertia));
}

void btRigidBody_setLinearFactor(btRigidBody* obj, const btVector3* linearFactor)
{
	BTVECTOR3_IN(linearFactor);
	obj->setLinearFactor(BTVECTOR3_USE(linearFactor));
}

void btRigidBody_setLinearVelocity(btRigidBody* obj, const btVector3* lin_vel)
{
	BTVECTOR3_IN(lin_vel);
	obj->setLinearVelocity(BTVECTOR3_USE(lin_vel));
}

void btRigidBody_setMassProps(btRigidBody* obj, btScalar mass, const btVector3* inertia)
{
	BTVECTOR3_IN(inertia);
	obj->setMassProps(mass, BTVECTOR3_USE(inertia));
}

void btRigidBody_setMotionState(btRigidBody* obj, btMotionState* motionState)
{
	obj->setMotionState(motionState);
}

void btRigidBody_setNewBroadphaseProxy(btRigidBody* obj, btBroadphaseProxy* broadphaseProxy)
{
	obj->setNewBroadphaseProxy(broadphaseProxy);
}

void btRigidBody_setSleepingThresholds(btRigidBody* obj, btScalar linear, btScalar angular)
{
	obj->setSleepingThresholds(linear, angular);
}

void btRigidBody_translate(btRigidBody* obj, const btVector3* v)
{
	BTVECTOR3_IN(v);
	obj->translate(BTVECTOR3_USE(v));
}

btRigidBody* btRigidBody_upcast(btCollisionObject* colObj)
{
	return btRigidBody::upcast(colObj);
}

void btRigidBody_updateDeactivation(btRigidBody* obj, btScalar timeStep)
{
	obj->updateDeactivation(timeStep);
}

void btRigidBody_updateInertiaTensor(btRigidBody* obj)
{
	obj->updateInertiaTensor();
}

bool btRigidBody_wantsSleeping(btRigidBody* obj)
{
	return obj->wantsSleeping();
}
