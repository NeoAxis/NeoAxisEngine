#include <BulletDynamics/Dynamics/btRigidBody.h>
#include <BulletDynamics/Vehicle/btWheelInfo.h>

#include "btWheelInfo_wrap.h"

#ifndef BULLETC_DISABLE_IACTION_CLASSES

#include "conversion.h"

btWheelInfoConstructionInfo* btWheelInfoConstructionInfo_new()
{
	return new btWheelInfoConstructionInfo();
}

bool btWheelInfoConstructionInfo_getBIsFrontWheel(btWheelInfoConstructionInfo* obj)
{
	return obj->m_bIsFrontWheel;
}

void btWheelInfoConstructionInfo_getChassisConnectionCS(btWheelInfoConstructionInfo* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_chassisConnectionCS);
}

btScalar btWheelInfoConstructionInfo_getFrictionSlip(btWheelInfoConstructionInfo* obj)
{
	return obj->m_frictionSlip;
}

btScalar btWheelInfoConstructionInfo_getMaxSuspensionForce(btWheelInfoConstructionInfo* obj)
{
	return obj->m_maxSuspensionForce;
}

btScalar btWheelInfoConstructionInfo_getMaxSuspensionTravelCm(btWheelInfoConstructionInfo* obj)
{
	return obj->m_maxSuspensionTravelCm;
}

btScalar btWheelInfoConstructionInfo_getSuspensionRestLength(btWheelInfoConstructionInfo* obj)
{
	return obj->m_suspensionRestLength;
}

btScalar btWheelInfoConstructionInfo_getSuspensionStiffness(btWheelInfoConstructionInfo* obj)
{
	return obj->m_suspensionStiffness;
}

void btWheelInfoConstructionInfo_getWheelAxleCS(btWheelInfoConstructionInfo* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_wheelAxleCS);
}

void btWheelInfoConstructionInfo_getWheelDirectionCS(btWheelInfoConstructionInfo* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_wheelDirectionCS);
}

btScalar btWheelInfoConstructionInfo_getWheelRadius(btWheelInfoConstructionInfo* obj)
{
	return obj->m_wheelRadius;
}

btScalar btWheelInfoConstructionInfo_getWheelsDampingCompression(btWheelInfoConstructionInfo* obj)
{
	return obj->m_wheelsDampingCompression;
}

btScalar btWheelInfoConstructionInfo_getWheelsDampingRelaxation(btWheelInfoConstructionInfo* obj)
{
	return obj->m_wheelsDampingRelaxation;
}

void btWheelInfoConstructionInfo_setBIsFrontWheel(btWheelInfoConstructionInfo* obj,
	bool value)
{
	obj->m_bIsFrontWheel = value;
}

void btWheelInfoConstructionInfo_setChassisConnectionCS(btWheelInfoConstructionInfo* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_chassisConnectionCS, value);
}

void btWheelInfoConstructionInfo_setFrictionSlip(btWheelInfoConstructionInfo* obj,
	btScalar value)
{
	obj->m_frictionSlip = value;
}

void btWheelInfoConstructionInfo_setMaxSuspensionForce(btWheelInfoConstructionInfo* obj,
	btScalar value)
{
	obj->m_maxSuspensionForce = value;
}

void btWheelInfoConstructionInfo_setMaxSuspensionTravelCm(btWheelInfoConstructionInfo* obj,
	btScalar value)
{
	obj->m_maxSuspensionTravelCm = value;
}

void btWheelInfoConstructionInfo_setSuspensionRestLength(btWheelInfoConstructionInfo* obj,
	btScalar value)
{
	obj->m_suspensionRestLength = value;
}

void btWheelInfoConstructionInfo_setSuspensionStiffness(btWheelInfoConstructionInfo* obj,
	btScalar value)
{
	obj->m_suspensionStiffness = value;
}

void btWheelInfoConstructionInfo_setWheelAxleCS(btWheelInfoConstructionInfo* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_wheelAxleCS, value);
}

void btWheelInfoConstructionInfo_setWheelDirectionCS(btWheelInfoConstructionInfo* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_wheelDirectionCS, value);
}

void btWheelInfoConstructionInfo_setWheelRadius(btWheelInfoConstructionInfo* obj,
	btScalar value)
{
	obj->m_wheelRadius = value;
}

void btWheelInfoConstructionInfo_setWheelsDampingCompression(btWheelInfoConstructionInfo* obj,
	btScalar value)
{
	obj->m_wheelsDampingCompression = value;
}

void btWheelInfoConstructionInfo_setWheelsDampingRelaxation(btWheelInfoConstructionInfo* obj,
	btScalar value)
{
	obj->m_wheelsDampingRelaxation = value;
}

void btWheelInfoConstructionInfo_delete(btWheelInfoConstructionInfo* obj)
{
	delete obj;
}


btWheelInfo_RaycastInfo* btWheelInfo_RaycastInfo_new()
{
	return new btWheelInfo::RaycastInfo();
}

void btWheelInfo_RaycastInfo_getContactNormalWS(btWheelInfo_RaycastInfo* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_contactNormalWS);
}

void btWheelInfo_RaycastInfo_getContactPointWS(btWheelInfo_RaycastInfo* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_contactPointWS);
}

void* btWheelInfo_RaycastInfo_getGroundObject(btWheelInfo_RaycastInfo* obj)
{
	return obj->m_groundObject;
}

void btWheelInfo_RaycastInfo_getHardPointWS(btWheelInfo_RaycastInfo* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_hardPointWS);
}

btScalar btWheelInfo_RaycastInfo_getSuspensionLength(btWheelInfo_RaycastInfo* obj)
{
	return obj->m_suspensionLength;
}

void btWheelInfo_RaycastInfo_getWheelAxleWS(btWheelInfo_RaycastInfo* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_wheelAxleWS);
}

void btWheelInfo_RaycastInfo_getWheelDirectionWS(btWheelInfo_RaycastInfo* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_wheelDirectionWS);
}

bool btWheelInfo_RaycastInfo_IsInContact(btWheelInfo_RaycastInfo* obj)
{
	return obj->m_isInContact;
}

void btWheelInfo_RaycastInfo_setContactNormalWS(btWheelInfo_RaycastInfo* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_contactNormalWS, value);
}

void btWheelInfo_RaycastInfo_setContactPointWS(btWheelInfo_RaycastInfo* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_contactPointWS, value);
}

void btWheelInfo_RaycastInfo_setGroundObject(btWheelInfo_RaycastInfo* obj, void* value)
{
	obj->m_groundObject = value;
}

void btWheelInfo_RaycastInfo_setHardPointWS(btWheelInfo_RaycastInfo* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_hardPointWS, value);
}

void btWheelInfo_RaycastInfo_setInContact(btWheelInfo_RaycastInfo* obj, bool value)
{
	obj->m_isInContact = value;
}

void btWheelInfo_RaycastInfo_setSuspensionLength(btWheelInfo_RaycastInfo* obj, btScalar value)
{
	obj->m_suspensionLength = value;
}

void btWheelInfo_RaycastInfo_setWheelAxleWS(btWheelInfo_RaycastInfo* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_wheelAxleWS, value);
}

void btWheelInfo_RaycastInfo_setWheelDirectionWS(btWheelInfo_RaycastInfo* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_wheelDirectionWS, value);
}

void btWheelInfo_RaycastInfo_delete(btWheelInfo_RaycastInfo* obj)
{
	delete obj;
}


btWheelInfo* btWheelInfo_new(btWheelInfoConstructionInfo* ci)
{
	return new btWheelInfo(*ci);
}

bool btWheelInfo_getBIsFrontWheel(btWheelInfo* obj)
{
	return obj->m_bIsFrontWheel;
}

btScalar btWheelInfo_getBrake(btWheelInfo* obj)
{
	return obj->m_brake;
}

void btWheelInfo_getChassisConnectionPointCS(btWheelInfo* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_chassisConnectionPointCS);
}

void* btWheelInfo_getClientInfo(btWheelInfo* obj)
{
	return obj->m_clientInfo;
}

btScalar btWheelInfo_getClippedInvContactDotSuspension(btWheelInfo* obj)
{
	return obj->m_clippedInvContactDotSuspension;
}

btScalar btWheelInfo_getDeltaRotation(btWheelInfo* obj)
{
	return obj->m_deltaRotation;
}

btScalar btWheelInfo_getEngineForce(btWheelInfo* obj)
{
	return obj->m_engineForce;
}

btScalar btWheelInfo_getFrictionSlip(btWheelInfo* obj)
{
	return obj->m_frictionSlip;
}

btScalar btWheelInfo_getMaxSuspensionForce(btWheelInfo* obj)
{
	return obj->m_maxSuspensionForce;
}

btScalar btWheelInfo_getMaxSuspensionTravelCm(btWheelInfo* obj)
{
	return obj->m_maxSuspensionTravelCm;
}

btWheelInfo_RaycastInfo* btWheelInfo_getRaycastInfo(btWheelInfo* obj)
{
	return &obj->m_raycastInfo;
}

btScalar btWheelInfo_getRollInfluence(btWheelInfo* obj)
{
	return obj->m_rollInfluence;
}

btScalar btWheelInfo_getRotation(btWheelInfo* obj)
{
	return obj->m_rotation;
}

btScalar btWheelInfo_getSkidInfo(btWheelInfo* obj)
{
	return obj->m_skidInfo;
}

btScalar btWheelInfo_getSteering(btWheelInfo* obj)
{
	return obj->m_steering;
}

btScalar btWheelInfo_getSuspensionRelativeVelocity(btWheelInfo* obj)
{
	return obj->m_suspensionRelativeVelocity;
}

btScalar btWheelInfo_getSuspensionRestLength(btWheelInfo* obj)
{
	return obj->getSuspensionRestLength();
}

btScalar btWheelInfo_getSuspensionRestLength1(btWheelInfo* obj)
{
	return obj->m_suspensionRestLength1;
}

btScalar btWheelInfo_getSuspensionStiffness(btWheelInfo* obj)
{
	return obj->m_suspensionStiffness;
}

void btWheelInfo_getWheelAxleCS(btWheelInfo* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_wheelAxleCS);
}

void btWheelInfo_getWheelDirectionCS(btWheelInfo* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_wheelDirectionCS);
}

btScalar btWheelInfo_getWheelsDampingCompression(btWheelInfo* obj)
{
	return obj->m_wheelsDampingCompression;
}

btScalar btWheelInfo_getWheelsDampingRelaxation(btWheelInfo* obj)
{
	return obj->m_wheelsDampingRelaxation;
}

btScalar btWheelInfo_getWheelsRadius(btWheelInfo* obj)
{
	return obj->m_wheelsRadius;
}

btScalar btWheelInfo_getWheelsSuspensionForce(btWheelInfo* obj)
{
	return obj->m_wheelsSuspensionForce;
}

void btWheelInfo_getWorldTransform(btWheelInfo* obj, btTransform* value)
{
	BTTRANSFORM_SET(value, obj->m_worldTransform);
}

void btWheelInfo_setBIsFrontWheel(btWheelInfo* obj, bool value)
{
	obj->m_bIsFrontWheel = value;
}

void btWheelInfo_setBrake(btWheelInfo* obj, btScalar value)
{
	obj->m_brake = value;
}

void btWheelInfo_setChassisConnectionPointCS(btWheelInfo* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_chassisConnectionPointCS, value);
}

void btWheelInfo_setClientInfo(btWheelInfo* obj, void* value)
{
	obj->m_clientInfo = value;
}

void btWheelInfo_setClippedInvContactDotSuspension(btWheelInfo* obj, btScalar value)
{
	obj->m_clippedInvContactDotSuspension = value;
}

void btWheelInfo_setDeltaRotation(btWheelInfo* obj, btScalar value)
{
	obj->m_deltaRotation = value;
}

void btWheelInfo_setEngineForce(btWheelInfo* obj, btScalar value)
{
	obj->m_engineForce = value;
}

void btWheelInfo_setFrictionSlip(btWheelInfo* obj, btScalar value)
{
	obj->m_frictionSlip = value;
}

void btWheelInfo_setMaxSuspensionForce(btWheelInfo* obj, btScalar value)
{
	obj->m_maxSuspensionForce = value;
}

void btWheelInfo_setMaxSuspensionTravelCm(btWheelInfo* obj, btScalar value)
{
	obj->m_maxSuspensionTravelCm = value;
}

void btWheelInfo_setRollInfluence(btWheelInfo* obj, btScalar value)
{
	obj->m_rollInfluence = value;
}

void btWheelInfo_setRotation(btWheelInfo* obj, btScalar value)
{
	obj->m_rotation = value;
}

void btWheelInfo_setSkidInfo(btWheelInfo* obj, btScalar value)
{
	obj->m_skidInfo = value;
}

void btWheelInfo_setSteering(btWheelInfo* obj, btScalar value)
{
	obj->m_steering = value;
}

void btWheelInfo_setSuspensionRelativeVelocity(btWheelInfo* obj, btScalar value)
{
	obj->m_suspensionRelativeVelocity = value;
}

void btWheelInfo_setSuspensionRestLength1(btWheelInfo* obj, btScalar value)
{
	obj->m_suspensionRestLength1 = value;
}

void btWheelInfo_setSuspensionStiffness(btWheelInfo* obj, btScalar value)
{
	obj->m_suspensionStiffness = value;
}

void btWheelInfo_setWheelAxleCS(btWheelInfo* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_wheelAxleCS, value);
}

void btWheelInfo_setWheelDirectionCS(btWheelInfo* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_wheelDirectionCS, value);
}

void btWheelInfo_setWheelsDampingCompression(btWheelInfo* obj, btScalar value)
{
	obj->m_wheelsDampingCompression = value;
}

void btWheelInfo_setWheelsDampingRelaxation(btWheelInfo* obj, btScalar value)
{
	obj->m_wheelsDampingRelaxation = value;
}

void btWheelInfo_setWheelsRadius(btWheelInfo* obj, btScalar value)
{
	obj->m_wheelsRadius = value;
}

void btWheelInfo_setWheelsSuspensionForce(btWheelInfo* obj, btScalar value)
{
	obj->m_wheelsSuspensionForce = value;
}

void btWheelInfo_setWorldTransform(btWheelInfo* obj, const btTransform* value)
{
	BTTRANSFORM_COPY(&obj->m_worldTransform, value);
}

void btWheelInfo_updateWheel(btWheelInfo* obj, const btRigidBody* chassis, btWheelInfo_RaycastInfo* raycastInfo)
{
	obj->updateWheel(*chassis, *raycastInfo);
}

void btWheelInfo_delete(btWheelInfo* obj)
{
	delete obj;
}

#endif
