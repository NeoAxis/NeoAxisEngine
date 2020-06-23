#include <BulletCollision/BroadphaseCollision/btBroadphaseInterface.h>
#include <BulletCollision/BroadphaseCollision/btDispatcher.h>
#include <BulletCollision/CollisionDispatch/btCollisionObjectWrapper.h>
#include <BulletSoftBody/btSoftBody.h>
#include <BulletSoftBody/btSoftBodySolvers.h>

#include "conversion.h"
#include "btSoftBody_wrap.h"

btSoftBody_AJoint_IControlWrapper::btSoftBody_AJoint_IControlWrapper(p_btSoftBody_AJoint_IControl_Prepare PrepareCallback,
	p_btSoftBody_AJoint_IControl_Speed SpeedCallback)
{
	_PrepareCallback = PrepareCallback;
	_SpeedCallback = SpeedCallback;
	_wrapperData = 0;
}

void btSoftBody_AJoint_IControlWrapper::Prepare(btSoftBody_AJoint* aJoint)
{
	_PrepareCallback(aJoint);
}

btScalar btSoftBody_AJoint_IControlWrapper::Speed(btSoftBody_AJoint* aJoint,
	btScalar current)
{
	return _SpeedCallback(aJoint, current);
}

void* btSoftBody_AJoint_IControlWrapper::getWrapperData() const
{
	return _wrapperData;
}

void btSoftBody_AJoint_IControlWrapper::setWrapperData(void* data)
{
	_wrapperData = data;
}


btSoftBody_ImplicitFnWrapper::btSoftBody_ImplicitFnWrapper(p_btSoftBody_ImplicitFn_Eval EvalCallback)
{
	_EvalCallback = EvalCallback;
}

btScalar btSoftBody_ImplicitFnWrapper::Eval(const btVector3& x)
{
	return _EvalCallback(&x);
}


btSoftBodyWorldInfo* btSoftBodyWorldInfo_new()
{
	return new btSoftBodyWorldInfo();
}

btScalar btSoftBodyWorldInfo_getAir_density(btSoftBodyWorldInfo* obj)
{
	return obj->air_density;
}

btBroadphaseInterface* btSoftBodyWorldInfo_getBroadphase(btSoftBodyWorldInfo* obj)
{
	return obj->m_broadphase;
}

btDispatcher* btSoftBodyWorldInfo_getDispatcher(btSoftBodyWorldInfo* obj)
{
	return obj->m_dispatcher;
}

void btSoftBodyWorldInfo_getGravity(btSoftBodyWorldInfo* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_gravity);
}

btScalar btSoftBodyWorldInfo_getMaxDisplacement(btSoftBodyWorldInfo* obj)
{
	return obj->m_maxDisplacement;
}

btSparseSdf_3* btSoftBodyWorldInfo_getSparsesdf(btSoftBodyWorldInfo* obj)
{
	return &obj->m_sparsesdf;
}

btScalar btSoftBodyWorldInfo_getWater_density(btSoftBodyWorldInfo* obj)
{
	return obj->water_density;
}

void btSoftBodyWorldInfo_getWater_normal(btSoftBodyWorldInfo* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->water_normal);
}

btScalar btSoftBodyWorldInfo_getWater_offset(btSoftBodyWorldInfo* obj)
{
	return obj->water_offset;
}

void btSoftBodyWorldInfo_setAir_density(btSoftBodyWorldInfo* obj, btScalar value)
{
	obj->air_density = value;
}

void btSoftBodyWorldInfo_setBroadphase(btSoftBodyWorldInfo* obj, btBroadphaseInterface* value)
{
	obj->m_broadphase = value;
}

void btSoftBodyWorldInfo_setDispatcher(btSoftBodyWorldInfo* obj, btDispatcher* value)
{
	obj->m_dispatcher = value;
}

void btSoftBodyWorldInfo_setGravity(btSoftBodyWorldInfo* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_gravity, value);
}

void btSoftBodyWorldInfo_setMaxDisplacement(btSoftBodyWorldInfo* obj, btScalar value)
{
	obj->m_maxDisplacement = value;
}

void btSoftBodyWorldInfo_setWater_density(btSoftBodyWorldInfo* obj, btScalar value)
{
	obj->water_density = value;
}

void btSoftBodyWorldInfo_setWater_normal(btSoftBodyWorldInfo* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->water_normal, value);
}

void btSoftBodyWorldInfo_setWater_offset(btSoftBodyWorldInfo* obj, btScalar value)
{
	obj->water_offset = value;
}

void btSoftBodyWorldInfo_delete(btSoftBodyWorldInfo* obj)
{
	delete obj;
}


btSoftBody_AJoint_IControlWrapper* btSoftBody_AJoint_IControlWrapper_new(p_btSoftBody_AJoint_IControl_Prepare PrepareCallback,
	p_btSoftBody_AJoint_IControl_Speed SpeedCallback)
{
	return ALIGNED_NEW(btSoftBody_AJoint_IControlWrapper)(PrepareCallback, SpeedCallback);
}

void* btSoftBody_AJoint_IControlWrapper_getWrapperData(btSoftBody_AJoint_IControlWrapper* obj)
{
	return obj->getWrapperData();
}

void btSoftBody_AJoint_IControlWrapper_setWrapperData(btSoftBody_AJoint_IControlWrapper* obj, void* data)
{
	obj->setWrapperData(data);
}


btSoftBody_AJoint_IControl* btSoftBody_AJoint_IControl_new()
{
	return ALIGNED_NEW(btSoftBody_AJoint_IControl)();
}

btSoftBody_AJoint_IControl* btSoftBody_AJoint_IControl_Default()
{
	return btSoftBody_AJoint_IControl::Default();
}

void btSoftBody_AJoint_IControl_Prepare(btSoftBody_AJoint_IControl* obj, btSoftBody_AJoint* __unnamed0)
{
	obj->Prepare(__unnamed0);
}

btScalar btSoftBody_AJoint_IControl_Speed(btSoftBody_AJoint_IControl* obj, btSoftBody_AJoint* __unnamed0,
	btScalar current)
{
	return obj->Speed(__unnamed0, current);
}

void btSoftBody_AJoint_IControl_delete(btSoftBody_AJoint_IControl* obj)
{
	ALIGNED_FREE(obj);
}


btSoftBody_AJoint_Specs* btSoftBody_AJoint_Specs_new()
{
	return new btSoftBody::AJoint::Specs();
}

void btSoftBody_AJoint_Specs_getAxis(btSoftBody_AJoint_Specs* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->axis);
}

btSoftBody_AJoint_IControl* btSoftBody_AJoint_Specs_getIcontrol(btSoftBody_AJoint_Specs* obj)
{
	return obj->icontrol;
}

void btSoftBody_AJoint_Specs_setAxis(btSoftBody_AJoint_Specs* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->axis, value);
}

void btSoftBody_AJoint_Specs_setIcontrol(btSoftBody_AJoint_Specs* obj, btSoftBody_AJoint_IControl* value)
{
	obj->icontrol = value;
}


btVector3* btSoftBody_AJoint_getAxis(btSoftBody_AJoint* obj)
{
	return obj->m_axis;
}

btSoftBody_AJoint_IControl* btSoftBody_AJoint_getIcontrol(btSoftBody_AJoint* obj)
{
	return obj->m_icontrol;
}

void btSoftBody_AJoint_setIcontrol(btSoftBody_AJoint* obj, btSoftBody_AJoint_IControl* value)
{
	obj->m_icontrol = value;
}


btRigidBody* btSoftBody_Anchor_getBody(btSoftBody_Anchor* obj)
{
	return obj->m_body;
}

void btSoftBody_Anchor_getC0(btSoftBody_Anchor* obj, btMatrix3x3* value)
{
	BTMATRIX3X3_OUT(value, &obj->m_c0);
}

void btSoftBody_Anchor_getC1(btSoftBody_Anchor* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_c1);
}

btScalar btSoftBody_Anchor_getC2(btSoftBody_Anchor* obj)
{
	return obj->m_c2;
}

btScalar btSoftBody_Anchor_getInfluence(btSoftBody_Anchor* obj)
{
	return obj->m_influence;
}

void btSoftBody_Anchor_getLocal(btSoftBody_Anchor* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_local);
}

btSoftBody_Node* btSoftBody_Anchor_getNode(btSoftBody_Anchor* obj)
{
	return obj->m_node;
}

void btSoftBody_Anchor_setBody(btSoftBody_Anchor* obj, btRigidBody* value)
{
	obj->m_body = value;
}

void btSoftBody_Anchor_setC0(btSoftBody_Anchor* obj, const btMatrix3x3* value)
{
	BTMATRIX3X3_SET(&obj->m_c0, (btScalar*)value);
}

void btSoftBody_Anchor_setC1(btSoftBody_Anchor* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_c1, value);
}

void btSoftBody_Anchor_setC2(btSoftBody_Anchor* obj, btScalar value)
{
	obj->m_c2 = value;
}

void btSoftBody_Anchor_setInfluence(btSoftBody_Anchor* obj, btScalar value)
{
	obj->m_influence = value;
}

void btSoftBody_Anchor_setLocal(btSoftBody_Anchor* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_local, value);
}

void btSoftBody_Anchor_setNode(btSoftBody_Anchor* obj, btSoftBody_Node* value)
{
	obj->m_node = value;
}


btSoftBody_Body* btSoftBody_Body_new()
{
	return new btSoftBody::Body();
}

btSoftBody_Body* btSoftBody_Body_new2(const btCollisionObject* colObj)
{
	return new btSoftBody::Body(colObj);
}

btSoftBody_Body* btSoftBody_Body_new3(btSoftBody_Cluster* p)
{
	return new btSoftBody::Body(p);
}

void btSoftBody_Body_activate(btSoftBody_Body* obj)
{
	obj->activate();
}

void btSoftBody_Body_angularVelocity(btSoftBody_Body* obj, const btVector3* rpos,
	btVector3* value)
{
	BTVECTOR3_IN(rpos);
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->angularVelocity(BTVECTOR3_USE(rpos));
	BTVECTOR3_SET(value, temp);
}

void btSoftBody_Body_angularVelocity2(btSoftBody_Body* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->angularVelocity();
	BTVECTOR3_SET(value, temp);
}

void btSoftBody_Body_applyAImpulse(btSoftBody_Body* obj, const btSoftBody_Impulse* impulse)
{
	obj->applyAImpulse(*impulse);
}

void btSoftBody_Body_applyDAImpulse(btSoftBody_Body* obj, const btVector3* impulse)
{
	BTVECTOR3_IN(impulse);
	obj->applyDAImpulse(BTVECTOR3_USE(impulse));
}

void btSoftBody_Body_applyDCImpulse(btSoftBody_Body* obj, const btVector3* impulse)
{
	BTVECTOR3_IN(impulse);
	obj->applyDCImpulse(BTVECTOR3_USE(impulse));
}

void btSoftBody_Body_applyDImpulse(btSoftBody_Body* obj, const btVector3* impulse,
	const btVector3* rpos)
{
	BTVECTOR3_IN(impulse);
	BTVECTOR3_IN(rpos);
	obj->applyDImpulse(BTVECTOR3_USE(impulse), BTVECTOR3_USE(rpos));
}

void btSoftBody_Body_applyImpulse(btSoftBody_Body* obj, const btSoftBody_Impulse* impulse,
	const btVector3* rpos)
{
	BTVECTOR3_IN(rpos);
	obj->applyImpulse(*impulse, BTVECTOR3_USE(rpos));
}

void btSoftBody_Body_applyVAImpulse(btSoftBody_Body* obj, const btVector3* impulse)
{
	BTVECTOR3_IN(impulse);
	obj->applyVAImpulse(BTVECTOR3_USE(impulse));
}

void btSoftBody_Body_applyVImpulse(btSoftBody_Body* obj, const btVector3* impulse,
	const btVector3* rpos)
{
	BTVECTOR3_IN(impulse);
	BTVECTOR3_IN(rpos);
	obj->applyVImpulse(BTVECTOR3_USE(impulse), BTVECTOR3_USE(rpos));
}

const btCollisionObject* btSoftBody_Body_getCollisionObject(btSoftBody_Body* obj)
{
	return obj->m_collisionObject;
}

btRigidBody* btSoftBody_Body_getRigid(btSoftBody_Body* obj)
{
	return obj->m_rigid;
}

btSoftBody_Cluster* btSoftBody_Body_getSoft(btSoftBody_Body* obj)
{
	return obj->m_soft;
}

btScalar btSoftBody_Body_invMass(btSoftBody_Body* obj)
{
	return obj->invMass();
}

void btSoftBody_Body_invWorldInertia(btSoftBody_Body* obj, btMatrix3x3* value)
{
	BTMATRIX3X3_OUT(&obj->invWorldInertia(), value);
}

void btSoftBody_Body_linearVelocity(btSoftBody_Body* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->linearVelocity();
	BTVECTOR3_SET(value, temp);
}

void btSoftBody_Body_setCollisionObject(btSoftBody_Body* obj, const btCollisionObject* value)
{
	obj->m_collisionObject = value;
}

void btSoftBody_Body_setRigid(btSoftBody_Body* obj, btRigidBody* value)
{
	obj->m_rigid = value;
}

void btSoftBody_Body_setSoft(btSoftBody_Body* obj, btSoftBody_Cluster* value)
{
	obj->m_soft = value;
}

void btSoftBody_Body_velocity(btSoftBody_Body* obj, const btVector3* rpos, btVector3* value)
{
	BTVECTOR3_IN(rpos);
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->velocity(BTVECTOR3_USE(rpos));
	BTVECTOR3_SET(value, temp);
}

void btSoftBody_Body_xform(btSoftBody_Body* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->xform());
}

void btSoftBody_Body_delete(btSoftBody_Body* obj)
{
	delete obj;
}


btScalar btSoftBody_CJoint_getFriction(btSoftBody_CJoint* obj)
{
	return obj->m_friction;
}

int btSoftBody_CJoint_getLife(btSoftBody_CJoint* obj)
{
	return obj->m_life;
}

int btSoftBody_CJoint_getMaxlife(btSoftBody_CJoint* obj)
{
	return obj->m_maxlife;
}

void btSoftBody_CJoint_getNormal(btSoftBody_CJoint* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_normal);
}

btVector3* btSoftBody_CJoint_getRpos(btSoftBody_CJoint* obj)
{
	return obj->m_rpos;
}

void btSoftBody_CJoint_setFriction(btSoftBody_CJoint* obj, btScalar value)
{
	obj->m_friction = value;
}

void btSoftBody_CJoint_setLife(btSoftBody_CJoint* obj, int value)
{
	obj->m_life = value;
}

void btSoftBody_CJoint_setMaxlife(btSoftBody_CJoint* obj, int value)
{
	obj->m_maxlife = value;
}

void btSoftBody_CJoint_setNormal(btSoftBody_CJoint* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_normal, value);
}


btScalar btSoftBody_Cluster_getAdamping(btSoftBody_Cluster* obj)
{
	return obj->m_adamping;
}

void btSoftBody_Cluster_getAv(btSoftBody_Cluster* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_av);
}

int btSoftBody_Cluster_getClusterIndex(btSoftBody_Cluster* obj)
{
	return obj->m_clusterIndex;
}

bool btSoftBody_Cluster_getCollide(btSoftBody_Cluster* obj)
{
	return obj->m_collide;
}

void btSoftBody_Cluster_getCom(btSoftBody_Cluster* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_com);
}

bool btSoftBody_Cluster_getContainsAnchor(btSoftBody_Cluster* obj)
{
	return obj->m_containsAnchor;
}

btVector3* btSoftBody_Cluster_getDimpulses(btSoftBody_Cluster* obj)
{
	return obj->m_dimpulses;
}

btAlignedObjectArray_btVector3* btSoftBody_Cluster_getFramerefs(btSoftBody_Cluster* obj)
{
	return &obj->m_framerefs;
}

void btSoftBody_Cluster_getFramexform(btSoftBody_Cluster* obj, btTransform* value)
{
	BTTRANSFORM_SET(value, obj->m_framexform);
}

btScalar btSoftBody_Cluster_getIdmass(btSoftBody_Cluster* obj)
{
	return obj->m_idmass;
}

btScalar btSoftBody_Cluster_getImass(btSoftBody_Cluster* obj)
{
	return obj->m_imass;
}

void btSoftBody_Cluster_getInvwi(btSoftBody_Cluster* obj, btMatrix3x3* value)
{
	BTMATRIX3X3_OUT(value, obj->m_invwi);
}

btScalar btSoftBody_Cluster_getLdamping(btSoftBody_Cluster* obj)
{
	return obj->m_ldamping;
}

btDbvtNode* btSoftBody_Cluster_getLeaf(btSoftBody_Cluster* obj)
{
	return obj->m_leaf;
}

void btSoftBody_Cluster_getLocii(btSoftBody_Cluster* obj, btMatrix3x3* value)
{
	BTMATRIX3X3_OUT(value, obj->m_locii);
}

void btSoftBody_Cluster_getLv(btSoftBody_Cluster* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_lv);
}

btAlignedObjectArray_btScalar* btSoftBody_Cluster_getMasses(btSoftBody_Cluster* obj)
{
	return &obj->m_masses;
}

btScalar btSoftBody_Cluster_getMatching(btSoftBody_Cluster* obj)
{
	return obj->m_matching;
}

btScalar btSoftBody_Cluster_getMaxSelfCollisionImpulse(btSoftBody_Cluster* obj)
{
	return obj->m_maxSelfCollisionImpulse;
}

btScalar btSoftBody_Cluster_getNdamping(btSoftBody_Cluster* obj)
{
	return obj->m_ndamping;
}

int btSoftBody_Cluster_getNdimpulses(btSoftBody_Cluster* obj)
{
	return obj->m_ndimpulses;
}

btAlignedObjectArray_btSoftBody_NodePtr* btSoftBody_Cluster_getNodes(btSoftBody_Cluster* obj)
{
	return &obj->m_nodes;
}

int btSoftBody_Cluster_getNvimpulses(btSoftBody_Cluster* obj)
{
	return obj->m_nvimpulses;
}

btScalar btSoftBody_Cluster_getSelfCollisionImpulseFactor(btSoftBody_Cluster* obj)
{
	return obj->m_selfCollisionImpulseFactor;
}

btVector3* btSoftBody_Cluster_getVimpulses(btSoftBody_Cluster* obj)
{
	return obj->m_vimpulses;
}

void btSoftBody_Cluster_setAdamping(btSoftBody_Cluster* obj, btScalar value)
{
	obj->m_adamping = value;
}

void btSoftBody_Cluster_setAv(btSoftBody_Cluster* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_av, value);
}

void btSoftBody_Cluster_setClusterIndex(btSoftBody_Cluster* obj, int value)
{
	obj->m_clusterIndex = value;
}

void btSoftBody_Cluster_setCollide(btSoftBody_Cluster* obj, bool value)
{
	obj->m_collide = value;
}

void btSoftBody_Cluster_setCom(btSoftBody_Cluster* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_com, value);
}

void btSoftBody_Cluster_setContainsAnchor(btSoftBody_Cluster* obj, bool value)
{
	obj->m_containsAnchor = value;
}

void btSoftBody_Cluster_setFramexform(btSoftBody_Cluster* obj, const btTransform* value)
{
	BTTRANSFORM_COPY(&obj->m_framexform, value);
}

void btSoftBody_Cluster_setIdmass(btSoftBody_Cluster* obj, btScalar value)
{
	obj->m_idmass = value;
}

void btSoftBody_Cluster_setImass(btSoftBody_Cluster* obj, btScalar value)
{
	obj->m_imass = value;
}

void btSoftBody_Cluster_setInvwi(btSoftBody_Cluster* obj, const btMatrix3x3* value)
{
	BTMATRIX3X3_SET(&obj->m_invwi, (btScalar*)value);
}

void btSoftBody_Cluster_setLdamping(btSoftBody_Cluster* obj, btScalar value)
{
	obj->m_ldamping = value;
}

void btSoftBody_Cluster_setLeaf(btSoftBody_Cluster* obj, btDbvtNode* value)
{
	obj->m_leaf = value;
}

void btSoftBody_Cluster_setLocii(btSoftBody_Cluster* obj, const btMatrix3x3* value)
{
	BTMATRIX3X3_SET(&obj->m_locii, (btScalar*)value);
}

void btSoftBody_Cluster_setLv(btSoftBody_Cluster* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_lv, value);
}

void btSoftBody_Cluster_setMatching(btSoftBody_Cluster* obj, btScalar value)
{
	obj->m_matching = value;
}

void btSoftBody_Cluster_setMaxSelfCollisionImpulse(btSoftBody_Cluster* obj, btScalar value)
{
	obj->m_maxSelfCollisionImpulse = value;
}

void btSoftBody_Cluster_setNdamping(btSoftBody_Cluster* obj, btScalar value)
{
	obj->m_ndamping = value;
}

void btSoftBody_Cluster_setNdimpulses(btSoftBody_Cluster* obj, int value)
{
	obj->m_ndimpulses = value;
}

void btSoftBody_Cluster_setNvimpulses(btSoftBody_Cluster* obj, int value)
{
	obj->m_nvimpulses = value;
}

void btSoftBody_Cluster_setSelfCollisionImpulseFactor(btSoftBody_Cluster* obj, btScalar value)
{
	obj->m_selfCollisionImpulseFactor = value;
}


btSoftBody_eAeroModel btSoftBody_Config_getAeromodel(btSoftBody_Config* obj)
{
	return obj->aeromodel;
}

int btSoftBody_Config_getCiterations(btSoftBody_Config* obj)
{
	return obj->citerations;
}

int btSoftBody_Config_getCollisions(btSoftBody_Config* obj)
{
	return obj->collisions;
}

int btSoftBody_Config_getDiterations(btSoftBody_Config* obj)
{
	return obj->diterations;
}

btAlignedObjectArray_btSoftBody_ePSolver__* btSoftBody_Config_getDsequence(btSoftBody_Config* obj)
{
	return &obj->m_dsequence;
}

btScalar btSoftBody_Config_getKAHR(btSoftBody_Config* obj)
{
	return obj->kAHR;
}

btScalar btSoftBody_Config_getKCHR(btSoftBody_Config* obj)
{
	return obj->kCHR;
}

btScalar btSoftBody_Config_getKDF(btSoftBody_Config* obj)
{
	return obj->kDF;
}

btScalar btSoftBody_Config_getKDG(btSoftBody_Config* obj)
{
	return obj->kDG;
}

btScalar btSoftBody_Config_getKDP(btSoftBody_Config* obj)
{
	return obj->kDP;
}

btScalar btSoftBody_Config_getKKHR(btSoftBody_Config* obj)
{
	return obj->kKHR;
}

btScalar btSoftBody_Config_getKLF(btSoftBody_Config* obj)
{
	return obj->kLF;
}

btScalar btSoftBody_Config_getKMT(btSoftBody_Config* obj)
{
	return obj->kMT;
}

btScalar btSoftBody_Config_getKPR(btSoftBody_Config* obj)
{
	return obj->kPR;
}

btScalar btSoftBody_Config_getKSHR(btSoftBody_Config* obj)
{
	return obj->kSHR;
}

btScalar btSoftBody_Config_getKSK_SPLT_CL(btSoftBody_Config* obj)
{
	return obj->kSK_SPLT_CL;
}

btScalar btSoftBody_Config_getKSKHR_CL(btSoftBody_Config* obj)
{
	return obj->kSKHR_CL;
}

btScalar btSoftBody_Config_getKSR_SPLT_CL(btSoftBody_Config* obj)
{
	return obj->kSR_SPLT_CL;
}

btScalar btSoftBody_Config_getKSRHR_CL(btSoftBody_Config* obj)
{
	return obj->kSRHR_CL;
}

btScalar btSoftBody_Config_getKSS_SPLT_CL(btSoftBody_Config* obj)
{
	return obj->kSS_SPLT_CL;
}

btScalar btSoftBody_Config_getKSSHR_CL(btSoftBody_Config* obj)
{
	return obj->kSSHR_CL;
}

btScalar btSoftBody_Config_getKVC(btSoftBody_Config* obj)
{
	return obj->kVC;
}

btScalar btSoftBody_Config_getKVCF(btSoftBody_Config* obj)
{
	return obj->kVCF;
}

btScalar btSoftBody_Config_getMaxvolume(btSoftBody_Config* obj)
{
	return obj->maxvolume;
}

int btSoftBody_Config_getPiterations(btSoftBody_Config* obj)
{
	return obj->piterations;
}

btAlignedObjectArray_btSoftBody_ePSolver__* btSoftBody_Config_getPsequence(btSoftBody_Config* obj)
{
	return &obj->m_psequence;
}

btScalar btSoftBody_Config_getTimescale(btSoftBody_Config* obj)
{
	return obj->timescale;
}

int btSoftBody_Config_getViterations(btSoftBody_Config* obj)
{
	return obj->viterations;
}

btAlignedObjectArray_btSoftBody_eVSolver__* btSoftBody_Config_getVsequence(btSoftBody_Config* obj)
{
	return &obj->m_vsequence;
}

void btSoftBody_Config_setAeromodel(btSoftBody_Config* obj, btSoftBody_eAeroModel value)
{
	obj->aeromodel = value;
}

void btSoftBody_Config_setCiterations(btSoftBody_Config* obj, int value)
{
	obj->citerations = value;
}

void btSoftBody_Config_setCollisions(btSoftBody_Config* obj, int value)
{
	obj->collisions = value;
}

void btSoftBody_Config_setDiterations(btSoftBody_Config* obj, int value)
{
	obj->diterations = value;
}

void btSoftBody_Config_setKAHR(btSoftBody_Config* obj, btScalar value)
{
	obj->kAHR = value;
}

void btSoftBody_Config_setKCHR(btSoftBody_Config* obj, btScalar value)
{
	obj->kCHR = value;
}

void btSoftBody_Config_setKDF(btSoftBody_Config* obj, btScalar value)
{
	obj->kDF = value;
}

void btSoftBody_Config_setKDG(btSoftBody_Config* obj, btScalar value)
{
	obj->kDG = value;
}

void btSoftBody_Config_setKDP(btSoftBody_Config* obj, btScalar value)
{
	obj->kDP = value;
}

void btSoftBody_Config_setKKHR(btSoftBody_Config* obj, btScalar value)
{
	obj->kKHR = value;
}

void btSoftBody_Config_setKLF(btSoftBody_Config* obj, btScalar value)
{
	obj->kLF = value;
}

void btSoftBody_Config_setKMT(btSoftBody_Config* obj, btScalar value)
{
	obj->kMT = value;
}

void btSoftBody_Config_setKPR(btSoftBody_Config* obj, btScalar value)
{
	obj->kPR = value;
}

void btSoftBody_Config_setKSHR(btSoftBody_Config* obj, btScalar value)
{
	obj->kSHR = value;
}

void btSoftBody_Config_setKSK_SPLT_CL(btSoftBody_Config* obj, btScalar value)
{
	obj->kSK_SPLT_CL = value;
}

void btSoftBody_Config_setKSKHR_CL(btSoftBody_Config* obj, btScalar value)
{
	obj->kSKHR_CL = value;
}

void btSoftBody_Config_setKSR_SPLT_CL(btSoftBody_Config* obj, btScalar value)
{
	obj->kSR_SPLT_CL = value;
}

void btSoftBody_Config_setKSRHR_CL(btSoftBody_Config* obj, btScalar value)
{
	obj->kSRHR_CL = value;
}

void btSoftBody_Config_setKSS_SPLT_CL(btSoftBody_Config* obj, btScalar value)
{
	obj->kSS_SPLT_CL = value;
}

void btSoftBody_Config_setKSSHR_CL(btSoftBody_Config* obj, btScalar value)
{
	obj->kSSHR_CL = value;
}

void btSoftBody_Config_setKVC(btSoftBody_Config* obj, btScalar value)
{
	obj->kVC = value;
}

void btSoftBody_Config_setKVCF(btSoftBody_Config* obj, btScalar value)
{
	obj->kVCF = value;
}

void btSoftBody_Config_setMaxvolume(btSoftBody_Config* obj, btScalar value)
{
	obj->maxvolume = value;
}

void btSoftBody_Config_setPiterations(btSoftBody_Config* obj, int value)
{
	obj->piterations = value;
}

void btSoftBody_Config_setTimescale(btSoftBody_Config* obj, btScalar value)
{
	obj->timescale = value;
}

void btSoftBody_Config_setViterations(btSoftBody_Config* obj, int value)
{
	obj->viterations = value;
}


void* btSoftBody_Element_getTag(btSoftBody_Element* obj)
{
	return obj->m_tag;
}

void btSoftBody_Element_setTag(btSoftBody_Element* obj, void* value)
{
	obj->m_tag = value;
}

void btSoftBody_Element_delete(btSoftBody_Element* obj)
{
	delete obj;
}


btDbvtNode* btSoftBody_Face_getLeaf(btSoftBody_Face* obj)
{
	return obj->m_leaf;
}

btSoftBody_Node** btSoftBody_Face_getN(btSoftBody_Face* obj)
{
	return obj->m_n;
}

void btSoftBody_Face_getNormal(btSoftBody_Face* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_normal);
}

btScalar btSoftBody_Face_getRa(btSoftBody_Face* obj)
{
	return obj->m_ra;
}

void btSoftBody_Face_setLeaf(btSoftBody_Face* obj, btDbvtNode* value)
{
	obj->m_leaf = value;
}

void btSoftBody_Face_setNormal(btSoftBody_Face* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_normal, value);
}

void btSoftBody_Face_setRa(btSoftBody_Face* obj, btScalar value)
{
	obj->m_ra = value;
}


btSoftBody_Material* btSoftBody_Feature_getMaterial(btSoftBody_Feature* obj)
{
	return obj->m_material;
}

void btSoftBody_Feature_setMaterial(btSoftBody_Feature* obj, btSoftBody_Material* value)
{
	obj->m_material = value;
}


btSoftBody_ImplicitFnWrapper* btSoftBody_ImplicitFnWrapper_new(p_btSoftBody_ImplicitFn_Eval EvalCallback)
{
	return new btSoftBody_ImplicitFnWrapper(EvalCallback);
}


btScalar btSoftBody_ImplicitFn_Eval(btSoftBody_ImplicitFn* obj, const btVector3* x)
{
	BTVECTOR3_IN(x);
	return obj->Eval(BTVECTOR3_USE(x));
}

void btSoftBody_ImplicitFn_delete(btSoftBody_ImplicitFn* obj)
{
	delete obj;
}


btSoftBody_Impulse* btSoftBody_Impulse_new()
{
	return new btSoftBody::Impulse();
}

int btSoftBody_Impulse_getAsDrift(btSoftBody_Impulse* obj)
{
	return obj->m_asDrift;
}

int btSoftBody_Impulse_getAsVelocity(btSoftBody_Impulse* obj)
{
	return obj->m_asVelocity;
}

void btSoftBody_Impulse_getDrift(btSoftBody_Impulse* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_drift);
}

void btSoftBody_Impulse_getVelocity(btSoftBody_Impulse* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_velocity);
}

btSoftBody_Impulse* btSoftBody_Impulse_operator_n(btSoftBody_Impulse* obj)
{
	btSoftBody_Impulse* ret = new btSoftBody_Impulse;
	*ret = obj->operator-();
	return ret;
}

btSoftBody_Impulse* btSoftBody_Impulse_operator_m(btSoftBody_Impulse* obj, btScalar x)
{
	btSoftBody_Impulse* ret = new btSoftBody_Impulse;
	*ret = obj->operator*(x);
	return ret;
}

void btSoftBody_Impulse_setAsDrift(btSoftBody_Impulse* obj, int value)
{
	obj->m_asDrift = value;
}

void btSoftBody_Impulse_setAsVelocity(btSoftBody_Impulse* obj, int value)
{
	obj->m_asVelocity = value;
}

void btSoftBody_Impulse_setDrift(btSoftBody_Impulse* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_drift, value);
}

void btSoftBody_Impulse_setVelocity(btSoftBody_Impulse* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_velocity, value);
}

void btSoftBody_Impulse_delete(btSoftBody_Impulse* obj)
{
	delete obj;
}


btSoftBody::Joint::Specs* btSoftBody_Joint_Specs_new()
{
	return new btSoftBody::Joint::Specs();
}

btScalar btSoftBody_Joint_Specs_getCfm(btSoftBody_Joint_Specs* obj)
{
	return obj->cfm;
}

btScalar btSoftBody_Joint_Specs_getErp(btSoftBody_Joint_Specs* obj)
{
	return obj->erp;
}

btScalar btSoftBody_Joint_Specs_getSplit(btSoftBody_Joint_Specs* obj)
{
	return obj->split;
}

void btSoftBody_Joint_Specs_setCfm(btSoftBody_Joint_Specs* obj, btScalar value)
{
	obj->cfm = value;
}

void btSoftBody_Joint_Specs_setErp(btSoftBody_Joint_Specs* obj, btScalar value)
{
	obj->erp = value;
}

void btSoftBody_Joint_Specs_setSplit(btSoftBody_Joint_Specs* obj, btScalar value)
{
	obj->split = value;
}

void btSoftBody_Joint_Specs_delete(btSoftBody_Joint_Specs* obj)
{
	delete obj;
}


btSoftBody_Body* btSoftBody_Joint_getBodies(btSoftBody_Joint* obj)
{
	return obj->m_bodies;
}

btScalar btSoftBody_Joint_getCfm(btSoftBody_Joint* obj)
{
	return obj->m_cfm;
}

bool btSoftBody_Joint_getDelete(btSoftBody_Joint* obj)
{
	return obj->m_delete;
}

void btSoftBody_Joint_getDrift(btSoftBody_Joint* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_drift);
}

btScalar btSoftBody_Joint_getErp(btSoftBody_Joint* obj)
{
	return obj->m_erp;
}

void btSoftBody_Joint_getMassmatrix(btSoftBody_Joint* obj, btMatrix3x3* value)
{
	BTMATRIX3X3_OUT(value, obj->m_massmatrix);
}

btVector3* btSoftBody_Joint_getRefs(btSoftBody_Joint* obj)
{
	return obj->m_refs;
}

void btSoftBody_Joint_getSdrift(btSoftBody_Joint* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_sdrift);
}

btScalar btSoftBody_Joint_getSplit(btSoftBody_Joint* obj)
{
	return obj->m_split;
}

void btSoftBody_Joint_Prepare(btSoftBody_Joint* obj, btScalar dt, int iterations)
{
	obj->Prepare(dt, iterations);
}

void btSoftBody_Joint_setCfm(btSoftBody_Joint* obj, btScalar value)
{
	obj->m_cfm = value;
}

void btSoftBody_Joint_setDelete(btSoftBody_Joint* obj, bool value)
{
	obj->m_delete = value;
}

void btSoftBody_Joint_setDrift(btSoftBody_Joint* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_drift, value);
}

void btSoftBody_Joint_setErp(btSoftBody_Joint* obj, btScalar value)
{
	obj->m_erp = value;
}

void btSoftBody_Joint_setMassmatrix(btSoftBody_Joint* obj, const btMatrix3x3* value)
{
	BTMATRIX3X3_SET(&obj->m_massmatrix, (btScalar*)value);
}

void btSoftBody_Joint_setSdrift(btSoftBody_Joint* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_sdrift, value);
}

void btSoftBody_Joint_setSplit(btSoftBody_Joint* obj, btScalar value)
{
	obj->m_split = value;
}

void btSoftBody_Joint_Solve(btSoftBody_Joint* obj, btScalar dt, btScalar sor)
{
	obj->Solve(dt, sor);
}

void btSoftBody_Joint_Terminate(btSoftBody_Joint* obj, btScalar dt)
{
	obj->Terminate(dt);
}

btSoftBody_Joint_eType btSoftBody_Joint_Type(btSoftBody_Joint* obj)
{
	return obj->Type();
}

void btSoftBody_Joint_delete(btSoftBody_Joint* obj)
{
	delete obj;
}


btSoftBody_Link* btSoftBody_Link_new()
{
	return ALIGNED_NEW(btSoftBody::Link)();
}

btSoftBody_Link* btSoftBody_Link_new2(btSoftBody_Link* obj)
{
	return ALIGNED_NEW(btSoftBody::Link)(*obj);
}

int btSoftBody_Link_getBbending(btSoftBody_Link* obj)
{
	return obj->m_bbending;
}

btScalar btSoftBody_Link_getC0(btSoftBody_Link* obj)
{
	return obj->m_c0;
}

btScalar btSoftBody_Link_getC1(btSoftBody_Link* obj)
{
	return obj->m_c1;
}

btScalar btSoftBody_Link_getC2(btSoftBody_Link* obj)
{
	return obj->m_c2;
}

void btSoftBody_Link_getC3(btSoftBody_Link* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_c3);
}

btSoftBody_Node** btSoftBody_Link_getN(btSoftBody_Link* obj)
{
	return obj->m_n;
}

btScalar btSoftBody_Link_getRl(btSoftBody_Link* obj)
{
	return obj->m_rl;
}

void btSoftBody_Link_setBbending(btSoftBody_Link* obj, int value)
{
	obj->m_bbending = value;
}

void btSoftBody_Link_setC0(btSoftBody_Link* obj, btScalar value)
{
	obj->m_c0 = value;
}

void btSoftBody_Link_setC1(btSoftBody_Link* obj, btScalar value)
{
	obj->m_c1 = value;
}

void btSoftBody_Link_setC2(btSoftBody_Link* obj, btScalar value)
{
	obj->m_c2 = value;
}

void btSoftBody_Link_setC3(btSoftBody_Link* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_c3, value);
}

void btSoftBody_Link_setRl(btSoftBody_Link* obj, btScalar value)
{
	obj->m_rl = value;
}

void btSoftBody_Link_delete(btSoftBody_Link* obj)
{
	ALIGNED_FREE(obj);
}


btSoftBody_LJoint_Specs* btSoftBody_LJoint_Specs_new()
{
	return new btSoftBody::LJoint::Specs();
}

void btSoftBody_LJoint_Specs_getPosition(btSoftBody_LJoint_Specs* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->position);
}

void btSoftBody_LJoint_Specs_setPosition(btSoftBody_LJoint_Specs* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->position, value);
}


btVector3* btSoftBody_LJoint_getRpos(btSoftBody_LJoint* obj)
{
	return obj->m_rpos;
}


int btSoftBody_Material_getFlags(btSoftBody_Material* obj)
{
	return obj->m_flags;
}

btScalar btSoftBody_Material_getKAST(btSoftBody_Material* obj)
{
	return obj->m_kAST;
}

btScalar btSoftBody_Material_getKLST(btSoftBody_Material* obj)
{
	return obj->m_kLST;
}

btScalar btSoftBody_Material_getKVST(btSoftBody_Material* obj)
{
	return obj->m_kVST;
}

void btSoftBody_Material_setFlags(btSoftBody_Material* obj, int value)
{
	obj->m_flags = value;
}

void btSoftBody_Material_setKAST(btSoftBody_Material* obj, btScalar value)
{
	obj->m_kAST = value;
}

void btSoftBody_Material_setKLST(btSoftBody_Material* obj, btScalar value)
{
	obj->m_kLST = value;
}

void btSoftBody_Material_setKVST(btSoftBody_Material* obj, btScalar value)
{
	obj->m_kVST = value;
}


btScalar btSoftBody_Node_getArea(btSoftBody_Node* obj)
{
	return obj->m_area;
}

int btSoftBody_Node_getBattach(btSoftBody_Node* obj)
{
	return obj->m_battach;
}

void btSoftBody_Node_getF(btSoftBody_Node* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_f);
}

btScalar btSoftBody_Node_getIm(btSoftBody_Node* obj)
{
	return obj->m_im;
}

btDbvtNode* btSoftBody_Node_getLeaf(btSoftBody_Node* obj)
{
	return obj->m_leaf;
}

void btSoftBody_Node_getN(btSoftBody_Node* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_n);
}

void btSoftBody_Node_getQ(btSoftBody_Node* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_q);
}

void btSoftBody_Node_getV(btSoftBody_Node* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_v);
}

void btSoftBody_Node_getX(btSoftBody_Node* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_x);
}

void btSoftBody_Node_setArea(btSoftBody_Node* obj, btScalar value)
{
	obj->m_area = value;
}

void btSoftBody_Node_setBattach(btSoftBody_Node* obj, int value)
{
	obj->m_battach = value;
}

void btSoftBody_Node_setF(btSoftBody_Node* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_f, value);
}

void btSoftBody_Node_setIm(btSoftBody_Node* obj, btScalar value)
{
	obj->m_im = value;
}

void btSoftBody_Node_setLeaf(btSoftBody_Node* obj, btDbvtNode* value)
{
	obj->m_leaf = value;
}

void btSoftBody_Node_setN(btSoftBody_Node* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_n, value);
}

void btSoftBody_Node_setQ(btSoftBody_Node* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_q, value);
}

void btSoftBody_Node_setV(btSoftBody_Node* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_v, value);
}

void btSoftBody_Node_setX(btSoftBody_Node* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_x, value);
}


btScalar* btSoftBody_Note_getCoords(btSoftBody_Note* obj)
{
	return obj->m_coords;
}

btSoftBody_Node** btSoftBody_Note_getNodes(btSoftBody_Note* obj)
{
	return obj->m_nodes;
}

void btSoftBody_Note_getOffset(btSoftBody_Note* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_offset);
}

int btSoftBody_Note_getRank(btSoftBody_Note* obj)
{
	return obj->m_rank;
}

const char* btSoftBody_Note_getText(btSoftBody_Note* obj)
{
	return obj->m_text;
}

void btSoftBody_Note_setOffset(btSoftBody_Note* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_offset, value);
}

void btSoftBody_Note_setRank(btSoftBody_Note* obj, int value)
{
	obj->m_rank = value;
}

void btSoftBody_Note_setText(btSoftBody_Note* obj, const char* value)
{
	obj->m_text = value;
}


void btSoftBody_Pose_getAqq(btSoftBody_Pose* obj, btMatrix3x3* value)
{
	BTMATRIX3X3_OUT(value, obj->m_aqq);
}

bool btSoftBody_Pose_getBframe(btSoftBody_Pose* obj)
{
	return obj->m_bframe;
}

bool btSoftBody_Pose_getBvolume(btSoftBody_Pose* obj)
{
	return obj->m_bvolume;
}

void btSoftBody_Pose_getCom(btSoftBody_Pose* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_com);
}

btAlignedObjectArray_btVector3* btSoftBody_Pose_getPos(btSoftBody_Pose* obj)
{
	return &obj->m_pos;
}

void btSoftBody_Pose_getRot(btSoftBody_Pose* obj, btMatrix3x3* value)
{
	BTMATRIX3X3_OUT(value, obj->m_rot);
}

void btSoftBody_Pose_getScl(btSoftBody_Pose* obj, btMatrix3x3* value)
{
	BTMATRIX3X3_OUT(value, obj->m_scl);
}

btAlignedObjectArray_btScalar* btSoftBody_Pose_getWgh(btSoftBody_Pose* obj)
{
	return &obj->m_wgh;
}

btScalar btSoftBody_Pose_getVolume(btSoftBody_Pose* obj)
{
	return obj->m_volume;
}

void btSoftBody_Pose_setAqq(btSoftBody_Pose* obj, const btMatrix3x3* value)
{
	BTMATRIX3X3_SET(&obj->m_aqq, (btScalar*)value);
}

void btSoftBody_Pose_setBframe(btSoftBody_Pose* obj, bool value)
{
	obj->m_bframe = value;
}

void btSoftBody_Pose_setBvolume(btSoftBody_Pose* obj, bool value)
{
	obj->m_bvolume = value;
}

void btSoftBody_Pose_setCom(btSoftBody_Pose* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_com, value);
}

void btSoftBody_Pose_setRot(btSoftBody_Pose* obj, const btMatrix3x3* value)
{
	BTMATRIX3X3_SET(&obj->m_rot, (btScalar*)value);
}

void btSoftBody_Pose_setScl(btSoftBody_Pose* obj, const btMatrix3x3* value)
{
	BTMATRIX3X3_SET(&obj->m_scl, (btScalar*)value);
}

void btSoftBody_Pose_setVolume(btSoftBody_Pose* obj, btScalar value)
{
	obj->m_volume = value;
}


btSoftBody_RayFromToCaster* btSoftBody_RayFromToCaster_new(const btVector3* rayFrom,
	const btVector3* rayTo, btScalar mxt)
{
	BTVECTOR3_IN(rayFrom);
	BTVECTOR3_IN(rayTo);
	return new btSoftBody::RayFromToCaster(BTVECTOR3_USE(rayFrom), BTVECTOR3_USE(rayTo),
		mxt);
}

btSoftBody_Face* btSoftBody_RayFromToCaster_getFace(btSoftBody_RayFromToCaster* obj)
{
	return obj->m_face;
}

btScalar btSoftBody_RayFromToCaster_getMint(btSoftBody_RayFromToCaster* obj)
{
	return obj->m_mint;
}

void btSoftBody_RayFromToCaster_getRayFrom(btSoftBody_RayFromToCaster* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_rayFrom);
}

void btSoftBody_RayFromToCaster_getRayNormalizedDirection(btSoftBody_RayFromToCaster* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_rayNormalizedDirection);
}

void btSoftBody_RayFromToCaster_getRayTo(btSoftBody_RayFromToCaster* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_rayTo);
}

int btSoftBody_RayFromToCaster_getTests(btSoftBody_RayFromToCaster* obj)
{
	return obj->m_tests;
}

btScalar btSoftBody_RayFromToCaster_rayFromToTriangle(const btVector3* rayFrom, const btVector3* rayTo,
	const btVector3* rayNormalizedDirection, const btVector3* a, const btVector3* b,
	const btVector3* c)
{
	BTVECTOR3_IN(rayFrom);
	BTVECTOR3_IN(rayTo);
	BTVECTOR3_IN(rayNormalizedDirection);
	BTVECTOR3_IN(a);
	BTVECTOR3_IN(b);
	BTVECTOR3_IN(c);
	return btSoftBody::RayFromToCaster::rayFromToTriangle(BTVECTOR3_USE(rayFrom), BTVECTOR3_USE(rayTo),
		BTVECTOR3_USE(rayNormalizedDirection), BTVECTOR3_USE(a), BTVECTOR3_USE(b),
		BTVECTOR3_USE(c));
}

btScalar btSoftBody_RayFromToCaster_rayFromToTriangle2(const btVector3* rayFrom,
	const btVector3* rayTo, const btVector3* rayNormalizedDirection, const btVector3* a,
	const btVector3* b, const btVector3* c, btScalar maxt)
{
	BTVECTOR3_IN(rayFrom);
	BTVECTOR3_IN(rayTo);
	BTVECTOR3_IN(rayNormalizedDirection);
	BTVECTOR3_IN(a);
	BTVECTOR3_IN(b);
	BTVECTOR3_IN(c);
	return btSoftBody::RayFromToCaster::rayFromToTriangle(BTVECTOR3_USE(rayFrom), BTVECTOR3_USE(rayTo),
		BTVECTOR3_USE(rayNormalizedDirection), BTVECTOR3_USE(a), BTVECTOR3_USE(b),
		BTVECTOR3_USE(c), maxt);
}

void btSoftBody_RayFromToCaster_setFace(btSoftBody_RayFromToCaster* obj, btSoftBody_Face* value)
{
	obj->m_face = value;
}

void btSoftBody_RayFromToCaster_setMint(btSoftBody_RayFromToCaster* obj, btScalar value)
{
	obj->m_mint = value;
}

void btSoftBody_RayFromToCaster_setRayFrom(btSoftBody_RayFromToCaster* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_rayFrom, value);
}

void btSoftBody_RayFromToCaster_setRayNormalizedDirection(btSoftBody_RayFromToCaster* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_rayNormalizedDirection, value);
}

void btSoftBody_RayFromToCaster_setRayTo(btSoftBody_RayFromToCaster* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_rayTo, value);
}

void btSoftBody_RayFromToCaster_setTests(btSoftBody_RayFromToCaster* obj, int value)
{
	obj->m_tests = value;
}


btSoftBody_RContact* btSoftBody_RContact_new()
{
	return new btSoftBody::RContact();
}

void btSoftBody_RContact_getC0(btSoftBody_RContact* obj, btMatrix3x3* value)
{
	BTMATRIX3X3_OUT(value, obj->m_c0);
}

void btSoftBody_RContact_getC1(btSoftBody_RContact* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_c1);
}

btScalar btSoftBody_RContact_getC2(btSoftBody_RContact* obj)
{
	return obj->m_c2;
}

btScalar btSoftBody_RContact_getC3(btSoftBody_RContact* obj)
{
	return obj->m_c3;
}

btScalar btSoftBody_RContact_getC4(btSoftBody_RContact* obj)
{
	return obj->m_c4;
}

btSoftBody_sCti* btSoftBody_RContact_getCti(btSoftBody_RContact* obj)
{
	return &obj->m_cti;
}

btSoftBody_Node* btSoftBody_RContact_getNode(btSoftBody_RContact* obj)
{
	return obj->m_node;
}

void btSoftBody_RContact_setC0(btSoftBody_RContact* obj, const btMatrix3x3* value)
{
	BTMATRIX3X3_SET(&obj->m_c0, (btScalar*)value);
}

void btSoftBody_RContact_setC1(btSoftBody_RContact* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_c1, value);
}

void btSoftBody_RContact_setC2(btSoftBody_RContact* obj, btScalar value)
{
	obj->m_c2 = value;
}

void btSoftBody_RContact_setC3(btSoftBody_RContact* obj, btScalar value)
{
	obj->m_c3 = value;
}

void btSoftBody_RContact_setC4(btSoftBody_RContact* obj, btScalar value)
{
	obj->m_c4 = value;
}

void btSoftBody_RContact_setNode(btSoftBody_RContact* obj, btSoftBody_Node* value)
{
	obj->m_node = value;
}

void btSoftBody_RContact_delete(btSoftBody_RContact* obj)
{
	delete obj;
}


btSoftBody_SContact* btSoftBody_SContact_new()
{
	return new btSoftBody::SContact();
}

btScalar* btSoftBody_SContact_getCfm(btSoftBody_SContact* obj)
{
	return obj->m_cfm;
}

btSoftBody_Face* btSoftBody_SContact_getFace(btSoftBody_SContact* obj)
{
	return obj->m_face;
}

btScalar btSoftBody_SContact_getFriction(btSoftBody_SContact* obj)
{
	return obj->m_friction;
}

btScalar btSoftBody_SContact_getMargin(btSoftBody_SContact* obj)
{
	return obj->m_margin;
}

btSoftBody_Node* btSoftBody_SContact_getNode(btSoftBody_SContact* obj)
{
	return obj->m_node;
}

void btSoftBody_SContact_getNormal(btSoftBody_SContact* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_normal);
}

void btSoftBody_SContact_getWeights(btSoftBody_SContact* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_weights);
}

void btSoftBody_SContact_setFace(btSoftBody_SContact* obj, btSoftBody_Face* value)
{
	obj->m_face = value;
}

void btSoftBody_SContact_setFriction(btSoftBody_SContact* obj, btScalar value)
{
	obj->m_friction = value;
}

void btSoftBody_SContact_setMargin(btSoftBody_SContact* obj, btScalar value)
{
	obj->m_margin = value;
}

void btSoftBody_SContact_setNode(btSoftBody_SContact* obj, btSoftBody_Node* value)
{
	obj->m_node = value;
}

void btSoftBody_SContact_setNormal(btSoftBody_SContact* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_normal, value);
}

void btSoftBody_SContact_setWeights(btSoftBody_SContact* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_weights, value);
}

void btSoftBody_SContact_delete(btSoftBody_SContact* obj)
{
	delete obj;
}


btSoftBody_sCti* btSoftBody_sCti_new()
{
	return new btSoftBody::sCti();
}

const btCollisionObject* btSoftBody_sCti_getColObj(btSoftBody_sCti* obj)
{
	return obj->m_colObj;
}

void btSoftBody_sCti_getNormal(btSoftBody_sCti* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_normal);
}

btScalar btSoftBody_sCti_getOffset(btSoftBody_sCti* obj)
{
	return obj->m_offset;
}

void btSoftBody_sCti_setColObj(btSoftBody_sCti* obj, const btCollisionObject* value)
{
	obj->m_colObj = value;
}

void btSoftBody_sCti_setNormal(btSoftBody_sCti* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_normal, value);
}

void btSoftBody_sCti_setOffset(btSoftBody_sCti* obj, btScalar value)
{
	obj->m_offset = value;
}

void btSoftBody_sCti_delete(btSoftBody_sCti* obj)
{
	delete obj;
}


btScalar btSoftBody_SolverState_getIsdt(btSoftBody_SolverState* obj)
{
	return obj->isdt;
}

btScalar btSoftBody_SolverState_getRadmrg(btSoftBody_SolverState* obj)
{
	return obj->radmrg;
}

btScalar btSoftBody_SolverState_getSdt(btSoftBody_SolverState* obj)
{
	return obj->sdt;
}

btScalar btSoftBody_SolverState_getUpdmrg(btSoftBody_SolverState* obj)
{
	return obj->updmrg;
}

btScalar btSoftBody_SolverState_getVelmrg(btSoftBody_SolverState* obj)
{
	return obj->velmrg;
}

void btSoftBody_SolverState_setIsdt(btSoftBody_SolverState* obj, btScalar value)
{
	obj->isdt = value;
}

void btSoftBody_SolverState_setRadmrg(btSoftBody_SolverState* obj, btScalar value)
{
	obj->radmrg = value;
}

void btSoftBody_SolverState_setSdt(btSoftBody_SolverState* obj, btScalar value)
{
	obj->sdt = value;
}

void btSoftBody_SolverState_setUpdmrg(btSoftBody_SolverState* obj, btScalar value)
{
	obj->updmrg = value;
}

void btSoftBody_SolverState_setVelmrg(btSoftBody_SolverState* obj, btScalar value)
{
	obj->velmrg = value;
}


btSoftBody_sRayCast* btSoftBody_sRayCast_new()
{
	return new btSoftBody::sRayCast();
}

btSoftBody* btSoftBody_sRayCast_getBody(btSoftBody_sRayCast* obj)
{
	return obj->body;
}

btSoftBody_eFeature btSoftBody_sRayCast_getFeature(btSoftBody_sRayCast* obj)
{
	return obj->feature;
}

btScalar btSoftBody_sRayCast_getFraction(btSoftBody_sRayCast* obj)
{
	return obj->fraction;
}

int btSoftBody_sRayCast_getIndex(btSoftBody_sRayCast* obj)
{
	return obj->index;
}

void btSoftBody_sRayCast_setBody(btSoftBody_sRayCast* obj, btSoftBody* value)
{
	obj->body = value;
}

void btSoftBody_sRayCast_setFeature(btSoftBody_sRayCast* obj, btSoftBody_eFeature value)
{
	obj->feature = value;
}

void btSoftBody_sRayCast_setFraction(btSoftBody_sRayCast* obj, btScalar value)
{
	obj->fraction = value;
}

void btSoftBody_sRayCast_setIndex(btSoftBody_sRayCast* obj, int value)
{
	obj->index = value;
}

void btSoftBody_sRayCast_delete(btSoftBody_sRayCast* obj)
{
	delete obj;
}


btVector3* btSoftBody_Tetra_getC0(btSoftBody_Tetra* obj)
{
	return obj->m_c0;
}

btScalar btSoftBody_Tetra_getC1(btSoftBody_Tetra* obj)
{
	return obj->m_c1;
}

btScalar btSoftBody_Tetra_getC2(btSoftBody_Tetra* obj)
{
	return obj->m_c2;
}

btDbvtNode* btSoftBody_Tetra_getLeaf(btSoftBody_Tetra* obj)
{
	return obj->m_leaf;
}

btSoftBody_Node** btSoftBody_Tetra_getN(btSoftBody_Tetra* obj)
{
	return obj->m_n;
}

btScalar btSoftBody_Tetra_getRv(btSoftBody_Tetra* obj)
{
	return obj->m_rv;
}

void btSoftBody_Tetra_setC1(btSoftBody_Tetra* obj, btScalar value)
{
	obj->m_c1 = value;
}

void btSoftBody_Tetra_setC2(btSoftBody_Tetra* obj, btScalar value)
{
	obj->m_c2 = value;
}

void btSoftBody_Tetra_setLeaf(btSoftBody_Tetra* obj, btDbvtNode* value)
{
	obj->m_leaf = value;
}

void btSoftBody_Tetra_setRv(btSoftBody_Tetra* obj, btScalar value)
{
	obj->m_rv = value;
}


btSoftBody* btSoftBody_new(btSoftBodyWorldInfo* worldInfo, int node_count, const btScalar* x,
	const btScalar* m)
{
	btVector3* xTemp = Vector3ArrayIn(x, node_count);
	btSoftBody* ret = new btSoftBody(worldInfo, node_count, xTemp, m);
	delete[] xTemp;
	return ret;
}

btSoftBody* btSoftBody_new2(btSoftBodyWorldInfo* worldInfo)
{
	return new btSoftBody(worldInfo);
}

void btSoftBody_addAeroForceToFace(btSoftBody* obj, const btVector3* windVelocity,
	int faceIndex)
{
	BTVECTOR3_IN(windVelocity);
	obj->addAeroForceToFace(BTVECTOR3_USE(windVelocity), faceIndex);
}

void btSoftBody_addAeroForceToNode(btSoftBody* obj, const btVector3* windVelocity,
	int nodeIndex)
{
	BTVECTOR3_IN(windVelocity);
	obj->addAeroForceToNode(BTVECTOR3_USE(windVelocity), nodeIndex);
}

void btSoftBody_addForce(btSoftBody* obj, const btVector3* force)
{
	BTVECTOR3_IN(force);
	obj->addForce(BTVECTOR3_USE(force));
}

void btSoftBody_addForce2(btSoftBody* obj, const btVector3* force, int node)
{
	BTVECTOR3_IN(force);
	obj->addForce(BTVECTOR3_USE(force), node);
}

void btSoftBody_addVelocity(btSoftBody* obj, const btVector3* velocity)
{
	BTVECTOR3_IN(velocity);
	obj->addVelocity(BTVECTOR3_USE(velocity));
}

void btSoftBody_addVelocity2(btSoftBody* obj, const btVector3* velocity, int node)
{
	BTVECTOR3_IN(velocity);
	obj->addVelocity(BTVECTOR3_USE(velocity), node);
}

void btSoftBody_appendAnchor(btSoftBody* obj, int node, btRigidBody* body, const btVector3* localPivot,
	bool disableCollisionBetweenLinkedBodies, btScalar influence)
{
	BTVECTOR3_IN(localPivot);
	obj->appendAnchor(node, body, BTVECTOR3_USE(localPivot), disableCollisionBetweenLinkedBodies,
		influence);
}

void btSoftBody_appendAnchor2(btSoftBody* obj, int node, btRigidBody* body, bool disableCollisionBetweenLinkedBodies,
	btScalar influence)
{
	obj->appendAnchor(node, body, disableCollisionBetweenLinkedBodies, influence);
}

void btSoftBody_appendAngularJoint(btSoftBody* obj, const btSoftBody_AJoint_Specs* specs)
{
	obj->appendAngularJoint(*specs);
}

void btSoftBody_appendAngularJoint2(btSoftBody* obj, const btSoftBody_AJoint_Specs* specs,
	btSoftBody_Body* body)
{
	obj->appendAngularJoint(*specs, *body);
}

void btSoftBody_appendAngularJoint3(btSoftBody* obj, const btSoftBody_AJoint_Specs* specs,
	btSoftBody* body)
{
	obj->appendAngularJoint(*specs, body);
}

void btSoftBody_appendAngularJoint4(btSoftBody* obj, const btSoftBody_AJoint_Specs* specs,
	btSoftBody_Cluster* body0, btSoftBody_Body* body1)
{
	obj->appendAngularJoint(*specs, body0, *body1);
}

void btSoftBody_appendFace(btSoftBody* obj, int model, btSoftBody_Material* mat)
{
	obj->appendFace(model, mat);
}

void btSoftBody_appendFace2(btSoftBody* obj, int node0, int node1, int node2, btSoftBody_Material* mat)
{
	obj->appendFace(node0, node1, node2, mat);
}

void btSoftBody_appendLinearJoint(btSoftBody* obj, const btSoftBody_LJoint_Specs* specs,
	btSoftBody* body)
{
	obj->appendLinearJoint(*specs, body);
}

void btSoftBody_appendLinearJoint2(btSoftBody* obj, const btSoftBody_LJoint_Specs* specs)
{
	obj->appendLinearJoint(*specs);
}

void btSoftBody_appendLinearJoint3(btSoftBody* obj, const btSoftBody_LJoint_Specs* specs,
	btSoftBody_Body* body)
{
	obj->appendLinearJoint(*specs, *body);
}

void btSoftBody_appendLinearJoint4(btSoftBody* obj, const btSoftBody_LJoint_Specs* specs,
	btSoftBody_Cluster* body0, btSoftBody_Body* body1)
{
	obj->appendLinearJoint(*specs, body0, *body1);
}

void btSoftBody_appendLink(btSoftBody* obj, int node0, int node1, btSoftBody_Material* mat,
	bool bcheckexist)
{
	obj->appendLink(node0, node1, mat, bcheckexist);
}

void btSoftBody_appendLink2(btSoftBody* obj, int model, btSoftBody_Material* mat)
{
	obj->appendLink(model, mat);
}

void btSoftBody_appendLink3(btSoftBody* obj, btSoftBody_Node* node0, btSoftBody_Node* node1,
	btSoftBody_Material* mat, bool bcheckexist)
{
	obj->appendLink(node0, node1, mat, bcheckexist);
}

btSoftBody_Material* btSoftBody_appendMaterial(btSoftBody* obj)
{
	return obj->appendMaterial();
}

void btSoftBody_appendNode(btSoftBody* obj, const btVector3* x, btScalar m)
{
	BTVECTOR3_IN(x);
	obj->appendNode(BTVECTOR3_USE(x), m);
}

void btSoftBody_appendNote(btSoftBody* obj, const char* text, const btVector3* o,
	btSoftBody_Face* feature)
{
	BTVECTOR3_IN(o);
	obj->appendNote(text, BTVECTOR3_USE(o), feature);
}

void btSoftBody_appendNote2(btSoftBody* obj, const char* text, const btVector3* o,
	btSoftBody_Link* feature)
{
	BTVECTOR3_IN(o);
	obj->appendNote(text, BTVECTOR3_USE(o), feature);
}

void btSoftBody_appendNote3(btSoftBody* obj, const char* text, const btVector3* o,
	btSoftBody_Node* feature)
{
	BTVECTOR3_IN(o);
	obj->appendNote(text, BTVECTOR3_USE(o), feature);
}

void btSoftBody_appendNote4(btSoftBody* obj, const char* text, const btVector3* o)
{
	BTVECTOR3_IN(o);
	obj->appendNote(text, BTVECTOR3_USE(o));
}

void btSoftBody_appendNote5(btSoftBody* obj, const char* text, const btVector3* o,
	const btVector4* c, btSoftBody_Node* n0, btSoftBody_Node* n1, btSoftBody_Node* n2,
	btSoftBody_Node* n3)
{
	BTVECTOR3_IN(o);
	BTVECTOR4_IN(c);
	obj->appendNote(text, BTVECTOR3_USE(o), BTVECTOR4_USE(c), n0, n1, n2, n3);
}

void btSoftBody_appendTetra(btSoftBody* obj, int model, btSoftBody_Material* mat)
{
	obj->appendTetra(model, mat);
}

void btSoftBody_appendTetra2(btSoftBody* obj, int node0, int node1, int node2, int node3,
	btSoftBody_Material* mat)
{
	obj->appendTetra(node0, node1, node2, node3, mat);
}

void btSoftBody_applyClusters(btSoftBody* obj, bool drift)
{
	obj->applyClusters(drift);
}

void btSoftBody_applyForces(btSoftBody* obj)
{
	obj->applyForces();
}

bool btSoftBody_checkContact(btSoftBody* obj, const btCollisionObjectWrapper* colObjWrap,
	const btVector3* x, btScalar margin, btSoftBody_sCti* cti)
{
	BTVECTOR3_IN(x);
	return obj->checkContact(colObjWrap, BTVECTOR3_USE(x), margin, *cti);
}

bool btSoftBody_checkFace(btSoftBody* obj, int node0, int node1, int node2)
{
	return obj->checkFace(node0, node1, node2);
}

bool btSoftBody_checkLink(btSoftBody* obj, const btSoftBody_Node* node0, const btSoftBody_Node* node1)
{
	return obj->checkLink(node0, node1);
}

bool btSoftBody_checkLink2(btSoftBody* obj, int node0, int node1)
{
	return obj->checkLink(node0, node1);
}

void btSoftBody_cleanupClusters(btSoftBody* obj)
{
	obj->cleanupClusters();
}

void btSoftBody_clusterAImpulse(btSoftBody_Cluster* cluster, const btSoftBody_Impulse* impulse)
{
	btSoftBody::clusterAImpulse(cluster, *impulse);
}

void btSoftBody_clusterCom(btSoftBody* obj, int cluster, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->clusterCom(cluster);
	BTVECTOR3_SET(value, temp);
}

void btSoftBody_clusterCom2(const btSoftBody_Cluster* cluster, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = btSoftBody::clusterCom(cluster);
	BTVECTOR3_SET(value, temp);
}

int btSoftBody_clusterCount(btSoftBody* obj)
{
	return obj->clusterCount();
}

void btSoftBody_clusterDAImpulse(btSoftBody_Cluster* cluster, const btVector3* impulse)
{
	BTVECTOR3_IN(impulse);
	btSoftBody::clusterDAImpulse(cluster, BTVECTOR3_USE(impulse));
}

void btSoftBody_clusterDCImpulse(btSoftBody_Cluster* cluster, const btVector3* impulse)
{
	BTVECTOR3_IN(impulse);
	btSoftBody::clusterDCImpulse(cluster, BTVECTOR3_USE(impulse));
}

void btSoftBody_clusterDImpulse(btSoftBody_Cluster* cluster, const btVector3* rpos,
	const btVector3* impulse)
{
	BTVECTOR3_IN(rpos);
	BTVECTOR3_IN(impulse);
	btSoftBody::clusterDImpulse(cluster, BTVECTOR3_USE(rpos), BTVECTOR3_USE(impulse));
}

void btSoftBody_clusterImpulse(btSoftBody_Cluster* cluster, const btVector3* rpos,
	const btSoftBody_Impulse* impulse)
{
	BTVECTOR3_IN(rpos);
	btSoftBody::clusterImpulse(cluster, BTVECTOR3_USE(rpos), *impulse);
}

void btSoftBody_clusterVAImpulse(btSoftBody_Cluster* cluster, const btVector3* impulse)
{
	BTVECTOR3_IN(impulse);
	btSoftBody::clusterVAImpulse(cluster, BTVECTOR3_USE(impulse));
}

void btSoftBody_clusterVelocity(const btSoftBody_Cluster* cluster, const btVector3* rpos,
	btVector3* value)
{
	BTVECTOR3_IN(rpos);
	ATTRIBUTE_ALIGNED16(btVector3) temp = btSoftBody::clusterVelocity(cluster, BTVECTOR3_USE(rpos));
	BTVECTOR3_SET(value, temp);
}

void btSoftBody_clusterVImpulse(btSoftBody_Cluster* cluster, const btVector3* rpos,
	const btVector3* impulse)
{
	BTVECTOR3_IN(rpos);
	BTVECTOR3_IN(impulse);
	btSoftBody::clusterVImpulse(cluster, BTVECTOR3_USE(rpos), BTVECTOR3_USE(impulse));
}

bool btSoftBody_cutLink(btSoftBody* obj, const btSoftBody_Node* node0, const btSoftBody_Node* node1,
	btScalar position)
{
	return obj->cutLink(node0, node1, position);
}

bool btSoftBody_cutLink2(btSoftBody* obj, int node0, int node1, btScalar position)
{
	return obj->cutLink(node0, node1, position);
}

void btSoftBody_dampClusters(btSoftBody* obj)
{
	obj->dampClusters();
}

void btSoftBody_defaultCollisionHandler(btSoftBody* obj, const btCollisionObjectWrapper* pcoWrap)
{
	obj->defaultCollisionHandler(pcoWrap);
}

void btSoftBody_defaultCollisionHandler2(btSoftBody* obj, btSoftBody* psb)
{
	obj->defaultCollisionHandler(psb);
}

void btSoftBody_evaluateCom(btSoftBody* obj, btVector3* value)
{
	ATTRIBUTE_ALIGNED16(btVector3) temp = obj->evaluateCom();
	BTVECTOR3_SET(value, temp);
}

int btSoftBody_generateBendingConstraints(btSoftBody* obj, int distance, btSoftBody_Material* mat)
{
	return obj->generateBendingConstraints(distance, mat);
}

int btSoftBody_generateClusters(btSoftBody* obj, int k)
{
	return obj->generateClusters(k);
}

int btSoftBody_generateClusters2(btSoftBody* obj, int k, int maxiterations)
{
	return obj->generateClusters(k, maxiterations);
}

void btSoftBody_getAabb(btSoftBody* obj, btVector3* aabbMin, btVector3* aabbMax)
{
	BTVECTOR3_DEF(aabbMin);
	BTVECTOR3_DEF(aabbMax);
	obj->getAabb(BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
	BTVECTOR3_DEF_OUT(aabbMin);
	BTVECTOR3_DEF_OUT(aabbMax);
}

btAlignedObjectArray_btSoftBody_Anchor* btSoftBody_getAnchors(btSoftBody* obj)
{
	return &obj->m_anchors;
}

btVector3* btSoftBody_getBounds(btSoftBody* obj)
{
	return obj->m_bounds;
}

bool btSoftBody_getBUpdateRtCst(btSoftBody* obj)
{
	return obj->m_bUpdateRtCst;
}

btDbvt* btSoftBody_getCdbvt(btSoftBody* obj)
{
	return &obj->m_cdbvt;
}

btSoftBody_Config* btSoftBody_getCfg(btSoftBody* obj)
{
	return &obj->m_cfg;
}

btAlignedObjectArray_bool* btSoftBody_getClusterConnectivity(btSoftBody* obj)
{
	return &obj->m_clusterConnectivity;
}

btAlignedObjectArray_btSoftBody_ClusterPtr* btSoftBody_getClusters(btSoftBody* obj)
{
	return &obj->m_clusters;
}

btAlignedObjectArray_const_btCollisionObjectPtr* btSoftBody_getCollisionDisabledObjects(
	btSoftBody* obj)
{
	return &obj->m_collisionDisabledObjects;
}

btAlignedObjectArray_btSoftBody_Face* btSoftBody_getFaces(btSoftBody* obj)
{
	return &obj->m_faces;
}

btDbvt* btSoftBody_getFdbvt(btSoftBody* obj)
{
	return &obj->m_fdbvt;
}

void btSoftBody_getInitialWorldTransform(btSoftBody* obj, btTransform* value)
{
	BTTRANSFORM_SET(value, obj->m_initialWorldTransform);
}

btAlignedObjectArray_btSoftBody_JointPtr* btSoftBody_getJoints(btSoftBody* obj)
{
	return &obj->m_joints;
}

btAlignedObjectArray_btSoftBody_Link* btSoftBody_getLinks(btSoftBody* obj)
{
	return &obj->m_links;
}

btScalar btSoftBody_getMass(btSoftBody* obj, int node)
{
	return obj->getMass(node);
}

btAlignedObjectArray_btSoftBody_MaterialPtr* btSoftBody_getMaterials(btSoftBody* obj)
{
	return &obj->m_materials;
}

btDbvt* btSoftBody_getNdbvt(btSoftBody* obj)
{
	return &obj->m_ndbvt;
}

btAlignedObjectArray_btSoftBody_Node* btSoftBody_getNodes(btSoftBody* obj)
{
	return &obj->m_nodes;
}

btAlignedObjectArray_btSoftBody_Note* btSoftBody_getNotes(btSoftBody* obj)
{
	return &obj->m_notes;
}

btSoftBody_Pose* btSoftBody_getPose(btSoftBody* obj)
{
	return &obj->m_pose;
}

btAlignedObjectArray_btSoftBody_RContact* btSoftBody_getRcontacts(btSoftBody* obj)
{
	return &obj->m_rcontacts;
}

btScalar btSoftBody_getRestLengthScale(btSoftBody* obj)
{
	return obj->getRestLengthScale();
}

btAlignedObjectArray_btSoftBody_SContact* btSoftBody_getScontacts(btSoftBody* obj)
{
	return &obj->m_scontacts;
}

btSoftBodySolver* btSoftBody_getSoftBodySolver(btSoftBody* obj)
{
	return obj->getSoftBodySolver();
}
/*
** btSoftBody_getSolver(btSoftBody_ePSolver solver)
{
	return &btSoftBody_getSolver(*solver);
}

** btSoftBody_getSolver2(btSoftBody_eVSolver solver)
{
	return &btSoftBody_getSolver(*solver);
}
*/
btSoftBody_SolverState* btSoftBody_getSst(btSoftBody* obj)
{
	return &obj->m_sst;
}

void* btSoftBody_getTag(btSoftBody* obj)
{
	return obj->m_tag;
}

btAlignedObjectArray_btSoftBody_Tetra* btSoftBody_getTetras(btSoftBody* obj)
{
	return &obj->m_tetras;
}

btScalar btSoftBody_getTimeacc(btSoftBody* obj)
{
	return obj->m_timeacc;
}

btScalar btSoftBody_getTotalMass(btSoftBody* obj)
{
	return obj->getTotalMass();
}

btAlignedObjectArray_int* btSoftBody_getUserIndexMapping(btSoftBody* obj)
{
	return &obj->m_userIndexMapping;
}

void btSoftBody_getWindVelocity(btSoftBody* obj, btVector3* velocity)
{
	BTVECTOR3_COPY(velocity, &obj->getWindVelocity());
}

btScalar btSoftBody_getVolume(btSoftBody* obj)
{
	return obj->getVolume();
}

btSoftBodyWorldInfo* btSoftBody_getWorldInfo(btSoftBody* obj)
{
	return obj->getWorldInfo();
}

void btSoftBody_indicesToPointers(btSoftBody* obj, const int* map)
{
	obj->indicesToPointers(map);
}

void btSoftBody_initDefaults(btSoftBody* obj)
{
	obj->initDefaults();
}

void btSoftBody_initializeClusters(btSoftBody* obj)
{
	obj->initializeClusters();
}

void btSoftBody_initializeFaceTree(btSoftBody* obj)
{
	obj->initializeFaceTree();
}

void btSoftBody_integrateMotion(btSoftBody* obj)
{
	obj->integrateMotion();
}

void btSoftBody_pointersToIndices(btSoftBody* obj)
{
	obj->pointersToIndices();
}

void btSoftBody_predictMotion(btSoftBody* obj, btScalar dt)
{
	obj->predictMotion(dt);
}

void btSoftBody_prepareClusters(btSoftBody* obj, int iterations)
{
	obj->prepareClusters(iterations);
}

void btSoftBody_PSolve_Anchors(btSoftBody* psb, btScalar kst, btScalar ti)
{
	btSoftBody::PSolve_Anchors(psb, kst, ti);
}

void btSoftBody_PSolve_Links(btSoftBody* psb, btScalar kst, btScalar ti)
{
	btSoftBody::PSolve_Links(psb, kst, ti);
}

void btSoftBody_PSolve_RContacts(btSoftBody* psb, btScalar kst, btScalar ti)
{
	btSoftBody::PSolve_RContacts(psb, kst, ti);
}

void btSoftBody_PSolve_SContacts(btSoftBody* psb, btScalar __unnamed1, btScalar ti)
{
	btSoftBody::PSolve_SContacts(psb, __unnamed1, ti);
}

void btSoftBody_randomizeConstraints(btSoftBody* obj)
{
	obj->randomizeConstraints();
}

bool btSoftBody_rayTest(btSoftBody* obj, const btVector3* rayFrom, const btVector3* rayTo,
	btSoftBody_sRayCast* results)
{
	BTVECTOR3_IN(rayFrom);
	BTVECTOR3_IN(rayTo);
	return obj->rayTest(BTVECTOR3_USE(rayFrom), BTVECTOR3_USE(rayTo), *results);
}

int btSoftBody_rayTest2(btSoftBody* obj, const btVector3* rayFrom, const btVector3* rayTo,
	btScalar* mint, btSoftBody_eFeature* feature, int* index, bool bcountonly)
{
	BTVECTOR3_IN(rayFrom);
	BTVECTOR3_IN(rayTo);
	return obj->rayTest(BTVECTOR3_USE(rayFrom), BTVECTOR3_USE(rayTo), *mint, *feature,
		*index, bcountonly);
}

void btSoftBody_refine(btSoftBody* obj, btSoftBody_ImplicitFn* ifn, btScalar accurary,
	bool cut)
{
	obj->refine(ifn, accurary, cut);
}

void btSoftBody_releaseCluster(btSoftBody* obj, int index)
{
	obj->releaseCluster(index);
}

void btSoftBody_releaseClusters(btSoftBody* obj)
{
	obj->releaseClusters();
}

void btSoftBody_resetLinkRestLengths(btSoftBody* obj)
{
	obj->resetLinkRestLengths();
}

void btSoftBody_rotate(btSoftBody* obj, const btQuaternion* rot)
{
	BTQUATERNION_IN(rot);
	obj->rotate(BTQUATERNION_USE(rot));
}

void btSoftBody_scale(btSoftBody* obj, const btVector3* scl)
{
	BTVECTOR3_IN(scl);
	obj->scale(BTVECTOR3_USE(scl));
}

void btSoftBody_setBUpdateRtCst(btSoftBody* obj, bool value)
{
	obj->m_bUpdateRtCst = value;
}

void btSoftBody_setMass(btSoftBody* obj, int node, btScalar mass)
{
	obj->setMass(node, mass);
}

void btSoftBody_setPose(btSoftBody* obj, bool bvolume, bool bframe)
{
	obj->setPose(bvolume, bframe);
}

void btSoftBody_setRestLengthScale(btSoftBody* obj, btScalar restLength)
{
	obj->setRestLengthScale(restLength);
}

void btSoftBody_setSoftBodySolver(btSoftBody* obj, btSoftBodySolver* softBodySolver)
{
	obj->setSoftBodySolver(softBodySolver);
}

void btSoftBody_setSolver(btSoftBody* obj, btSoftBody_eSolverPresets preset)
{
	obj->setSolver(preset);
}

void btSoftBody_setTag(btSoftBody* obj, void* value)
{
	obj->m_tag = value;
}

void btSoftBody_setTimeacc(btSoftBody* obj, btScalar value)
{
	obj->m_timeacc = value;
}

void btSoftBody_setTotalDensity(btSoftBody* obj, btScalar density)
{
	obj->setTotalDensity(density);
}

void btSoftBody_setTotalMass(btSoftBody* obj, btScalar mass, bool fromfaces)
{
	obj->setTotalMass(mass, fromfaces);
}

void btSoftBody_setVelocity(btSoftBody* obj, const btVector3* velocity)
{
	BTVECTOR3_IN(velocity);
	obj->setVelocity(BTVECTOR3_USE(velocity));
}

void btSoftBody_setWindVelocity(btSoftBody* obj, const btVector3* velocity)
{
	BTVECTOR3_IN(velocity);
	obj->setWindVelocity(BTVECTOR3_USE(velocity));
}

void btSoftBody_setVolumeDensity(btSoftBody* obj, btScalar density)
{
	obj->setVolumeDensity(density);
}

void btSoftBody_setVolumeMass(btSoftBody* obj, btScalar mass)
{
	obj->setVolumeMass(mass);
}

void btSoftBody_setWorldInfo(btSoftBody* obj, btSoftBodyWorldInfo* value)
{
	obj->m_worldInfo = value;
}

void btSoftBody_solveClusters(const btAlignedObjectArray_btSoftBodyPtr* bodies)
{
	btSoftBody::solveClusters(*bodies);
}

void btSoftBody_solveClusters2(btSoftBody* obj, btScalar sor)
{
	obj->solveClusters(sor);
}

void btSoftBody_solveCommonConstraints(btSoftBody** bodies, int count, int iterations)
{
	btSoftBody::solveCommonConstraints(bodies, count, iterations);
}

void btSoftBody_solveConstraints(btSoftBody* obj)
{
	obj->solveConstraints();
}

void btSoftBody_staticSolve(btSoftBody* obj, int iterations)
{
	obj->staticSolve(iterations);
}

void btSoftBody_transform(btSoftBody* obj, const btTransform* trs)
{
	BTTRANSFORM_IN(trs);
	obj->transform(BTTRANSFORM_USE(trs));
}

void btSoftBody_translate(btSoftBody* obj, const btVector3* trs)
{
	BTVECTOR3_IN(trs);
	obj->translate(BTVECTOR3_USE(trs));
}

btSoftBody* btSoftBody_upcast(btCollisionObject* colObj)
{
	return btSoftBody::upcast(colObj);
}

void btSoftBody_updateArea(btSoftBody* obj, bool averageArea)
{
	obj->updateArea(averageArea);
}

void btSoftBody_updateBounds(btSoftBody* obj)
{
	obj->updateBounds();
}

void btSoftBody_updateClusters(btSoftBody* obj)
{
	obj->updateClusters();
}

void btSoftBody_updateConstants(btSoftBody* obj)
{
	obj->updateConstants();
}

void btSoftBody_updateLinkConstants(btSoftBody* obj)
{
	obj->updateLinkConstants();
}

void btSoftBody_updateNormals(btSoftBody* obj)
{
	obj->updateNormals();
}

void btSoftBody_updatePose(btSoftBody* obj)
{
	obj->updatePose();
}

void btSoftBody_VSolve_Links(btSoftBody* psb, btScalar kst)
{
	btSoftBody::VSolve_Links(psb, kst);
}

int btSoftBody_getFaceVertexData(btSoftBody* obj, btScalar* vertices)
{
	btAlignedObjectArray<btSoftBody_Face>* faceArray = &obj->m_faces;
	int faceCount = faceArray->size();
	if (faceCount == 0) {
		return 0;
	}

	int vertexCount = faceCount * 3;

	int i, j;
	for (i = 0; i < faceCount; i++) {
		for (j = 0; j < 3; j++) {
			btSoftBody_Node* n = faceArray->at(i).m_n[j];
			btVector3_copy((btVector3*)vertices, &n->m_x);
			vertices += 3;
		}
	}

	return vertexCount;
}

int btSoftBody_getFaceVertexNormalData(btSoftBody* obj, btScalar* vertices)
{
	btAlignedObjectArray<btSoftBody_Face>* faceArray = &obj->m_faces;
	int faceCount = faceArray->size();
	if (faceCount == 0) {
		return 0;
	}

	int vertexCount = faceCount * 3;

	int i, j;
	for (i = 0; i < faceCount; i++) {
		for (j = 0; j < 3; j++) {
			btSoftBody_Node* n = faceArray->at(i).m_n[j];
			btVector3_copy((btVector3*)vertices, &n->m_x);
			vertices += 3;
			btVector3_copy((btVector3*)vertices, &n->m_n);
			vertices += 3;
		}
	}

	return vertexCount;
}

int btSoftBody_getFaceVertexNormalData2(btSoftBody* obj, btScalar* vertices, btScalar* normals)
{
	btAlignedObjectArray<btSoftBody_Face>* faceArray = &obj->m_faces;
	int faceCount = faceArray->size();
	if (faceCount == 0) {
		return 0;
	}

	int vertexCount = faceCount * 3;

	int i, j;
	for (i = 0; i < faceCount; i++) {
		for (j = 0; j < 3; j++) {
			btSoftBody_Node* n = faceArray->at(i).m_n[j];
			btVector3_copy((btVector3*)vertices, &n->m_x);
			btVector3_copy((btVector3*)normals, &n->m_n);
			vertices += 3;
			normals += 3;
		}
	}

	return vertexCount;
}

int btSoftBody_getLinkVertexData(btSoftBody* obj, btScalar* vertices)
{
	btAlignedObjectArray<btSoftBody_Link>* linkArray = &obj->m_links;
	int linkCount = linkArray->size();
	if (linkCount == 0) {
		return 0;
	}

	int vertexCount = linkCount * 2;

	int i;
	for (i = 0; i < linkCount; i++) {
		btSoftBody_Link* l = &linkArray->at(i);
		btVector3_copy((btVector3*)vertices, &l->m_n[0]->m_x);
		vertices += 3;
		btVector3_copy((btVector3*)vertices, &l->m_n[1]->m_x);
		vertices += 3;
	}

	return vertexCount;
}

int btSoftBody_getLinkVertexNormalData(btSoftBody* obj, btScalar* vertices)
{
	btAlignedObjectArray<btSoftBody_Link>* linkArray = &obj->m_links;
	int linkCount = linkArray->size();
	if (linkCount == 0) {
		return 0;
	}

	int vertexCount = linkCount * 2;

	int i;
	for (i = 0; i < linkCount; i++) {
		btSoftBody_Link* l = &linkArray->at(i);
		btVector3_copy((btVector3*)vertices, &l->m_n[0]->m_x);
		vertices += 6;
		btVector3_copy((btVector3*)vertices, &l->m_n[1]->m_x);
		vertices += 6;
	}

	return vertexCount;
}

int btSoftBody_getTetraVertexData(btSoftBody* obj, btScalar* vertices)
{
	btAlignedObjectArray<btSoftBody_Tetra>* tetraArray = &obj->m_tetras;
	int tetraCount = tetraArray->size();
	if (tetraCount == 0) {
		return 0;
	}

	int vertexCount = tetraCount * 12;

	int i;
	for (i = 0; i < tetraCount; i++) {
		btSoftBody_Tetra* t = &tetraArray->at(i);
		btVector3_copy((btVector3*)&vertices[0], &t->m_n[0]->m_x);
		btVector3_copy((btVector3*)&vertices[3], &t->m_n[1]->m_x);
		btVector3_copy((btVector3*)&vertices[6], &t->m_n[2]->m_x);
		
		btVector3_copy((btVector3*)&vertices[9], &t->m_n[0]->m_x);
		btVector3_copy((btVector3*)&vertices[12], &t->m_n[1]->m_x);
		btVector3_copy((btVector3*)&vertices[15], &t->m_n[3]->m_x);

		btVector3_copy((btVector3*)&vertices[18], &t->m_n[1]->m_x);
		btVector3_copy((btVector3*)&vertices[21], &t->m_n[2]->m_x);
		btVector3_copy((btVector3*)&vertices[24], &t->m_n[3]->m_x);

		btVector3_copy((btVector3*)&vertices[27], &t->m_n[2]->m_x);
		btVector3_copy((btVector3*)&vertices[30], &t->m_n[0]->m_x);
		btVector3_copy((btVector3*)&vertices[33], &t->m_n[3]->m_x);

		vertices += 36;
	}

	return vertexCount;
}

int btSoftBody_getTetraVertexNormalData(btSoftBody* obj, btScalar* vertices)
{
	btAlignedObjectArray<btSoftBody_Tetra>* tetraArray = &obj->m_tetras;
	int tetraCount = tetraArray->size();
	if (tetraCount == 0) {
		return 0;
	}

	int vertexCount = tetraCount * 12;

	int i;
	btVector3 c1, c2, c3, normal;
	for (i = 0; i < tetraCount; i++) {
		btSoftBody_Tetra* t = &tetraArray->at(i);
		c1 = t->m_n[1]->m_x - t->m_n[0]->m_x;
		c2 = t->m_n[0]->m_x - t->m_n[2]->m_x;

		normal = c1.cross(c2);
		btVector3_copy((btVector3*)&vertices[0], &t->m_n[0]->m_x);
		btVector3_copy((btVector3*)&vertices[3], &normal);
		btVector3_copy((btVector3*)&vertices[6], &t->m_n[1]->m_x);
		btVector3_copy((btVector3*)&vertices[9], &normal);
		btVector3_copy((btVector3*)&vertices[12], &t->m_n[2]->m_x);
		btVector3_copy((btVector3*)&vertices[15], &normal);

		c3 = t->m_n[3]->m_x - t->m_n[0]->m_x;
		normal = c1.cross(c3);
		btVector3_copy((btVector3*)&vertices[18], &t->m_n[0]->m_x);
		btVector3_copy((btVector3*)&vertices[21], &normal);
		btVector3_copy((btVector3*)&vertices[24], &t->m_n[1]->m_x);
		btVector3_copy((btVector3*)&vertices[27], &normal);
		btVector3_copy((btVector3*)&vertices[30], &t->m_n[3]->m_x);
		btVector3_copy((btVector3*)&vertices[33], &normal);

		c1 = t->m_n[2]->m_x - t->m_n[1]->m_x;
		c3 = t->m_n[3]->m_x - t->m_n[1]->m_x;
		normal = c1.cross(c3);
		btVector3_copy((btVector3*)&vertices[36], &t->m_n[1]->m_x);
		btVector3_copy((btVector3*)&vertices[39], &normal);
		btVector3_copy((btVector3*)&vertices[42], &t->m_n[2]->m_x);
		btVector3_copy((btVector3*)&vertices[45], &normal);
		btVector3_copy((btVector3*)&vertices[48], &t->m_n[3]->m_x);
		btVector3_copy((btVector3*)&vertices[51], &normal);

		c3 = t->m_n[3]->m_x - t->m_n[2]->m_x;
		normal = c2.cross(c3);
		btVector3_copy((btVector3*)&vertices[54], &t->m_n[2]->m_x);
		btVector3_copy((btVector3*)&vertices[57], &normal);
		btVector3_copy((btVector3*)&vertices[60], &t->m_n[0]->m_x);
		btVector3_copy((btVector3*)&vertices[63], &normal);
		btVector3_copy((btVector3*)&vertices[66], &t->m_n[3]->m_x);
		btVector3_copy((btVector3*)&vertices[69], &normal);

		vertices += 72;
	}

	return vertexCount;
}

int btSoftBody_getTetraVertexNormalData2(btSoftBody* obj, btScalar* vertices, btScalar* normals)
{
	btAlignedObjectArray<btSoftBody_Tetra>* tetraArray = &obj->m_tetras;
	int tetraCount = tetraArray->size();
	if (tetraCount == 0) {
		return 0;
	}

	int vertexCount = tetraCount * 12;

	int i;
	btVector3 c1, c2, c3, normal;
	for (i = 0; i < tetraCount; i++) {
		btSoftBody_Tetra* t = &tetraArray->at(i);
		c1 = t->m_n[1]->m_x - t->m_n[0]->m_x;
		c2 = t->m_n[0]->m_x - t->m_n[2]->m_x;

		normal = c1.cross(c2);
		btVector3_copy((btVector3*)&vertices[0], &t->m_n[0]->m_x);
		btVector3_copy((btVector3*)&normals[0], &normal);
		btVector3_copy((btVector3*)&vertices[3], &t->m_n[1]->m_x);
		btVector3_copy((btVector3*)&normals[3], &normal);
		btVector3_copy((btVector3*)&vertices[6], &t->m_n[2]->m_x);
		btVector3_copy((btVector3*)&normals[6], &normal);

		c3 = t->m_n[3]->m_x - t->m_n[0]->m_x;
		normal = c1.cross(c3);
		btVector3_copy((btVector3*)&vertices[9], &t->m_n[0]->m_x);
		btVector3_copy((btVector3*)&normals[9], &normal);
		btVector3_copy((btVector3*)&vertices[12], &t->m_n[1]->m_x);
		btVector3_copy((btVector3*)&normals[12], &normal);
		btVector3_copy((btVector3*)&vertices[15], &t->m_n[3]->m_x);
		btVector3_copy((btVector3*)&normals[15], &normal);

		c1 = t->m_n[2]->m_x - t->m_n[1]->m_x;
		c3 = t->m_n[3]->m_x - t->m_n[1]->m_x;
		normal = c1.cross(c3);
		btVector3_copy((btVector3*)&vertices[18], &t->m_n[1]->m_x);
		btVector3_copy((btVector3*)&normals[18], &normal);
		btVector3_copy((btVector3*)&vertices[21], &t->m_n[2]->m_x);
		btVector3_copy((btVector3*)&normals[21], &normal);
		btVector3_copy((btVector3*)&vertices[24], &t->m_n[3]->m_x);
		btVector3_copy((btVector3*)&normals[24], &normal);

		c3 = t->m_n[3]->m_x - t->m_n[2]->m_x;
		normal = c2.cross(c3);
		btVector3_copy((btVector3*)&vertices[27], &t->m_n[2]->m_x);
		btVector3_copy((btVector3*)&normals[27], &normal);
		btVector3_copy((btVector3*)&vertices[30], &t->m_n[0]->m_x);
		btVector3_copy((btVector3*)&normals[30], &normal);
		btVector3_copy((btVector3*)&vertices[33], &t->m_n[3]->m_x);
		btVector3_copy((btVector3*)&normals[33], &normal);

		vertices += 36;
		normals += 36;
	}

	return vertexCount;
}
