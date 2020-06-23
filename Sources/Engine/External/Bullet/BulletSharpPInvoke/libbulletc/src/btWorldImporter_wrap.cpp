//!!!!betauser
//#include <BulletCollision/CollisionShapes/btBvhTriangleMeshShape.h>
//#include <BulletCollision/CollisionShapes/btOptimizedBvh.h>
//#include <BulletCollision/CollisionShapes/btStridingMeshInterface.h>
//#include <BulletDynamics/Dynamics/btDynamicsWorld.h>
//#include <BulletDynamics/Dynamics/btRigidBody.h>
//#include <btWorldImporter.h>
//
//#include "btWorldImporter_wrap.h"
//
//#ifndef BULLETC_DISABLE_WORLD_IMPORTERS
//
//#include "conversion.h"
//
//btWorldImporter* btWorldImporter_new(btDynamicsWorld* world)
//{
//	return new btWorldImporter(world);
//}
//
//btCollisionShape* btWorldImporter_createBoxShape(btWorldImporter* obj, const btVector3* halfExtents)
//{
//	BTVECTOR3_IN(halfExtents);
//	return obj->createBoxShape(BTVECTOR3_USE(halfExtents));
//}
//
//btBvhTriangleMeshShape* btWorldImporter_createBvhTriangleMeshShape(btWorldImporter* obj,
//	btStridingMeshInterface* trimesh, btOptimizedBvh* bvh)
//{
//	return obj->createBvhTriangleMeshShape(trimesh, bvh);
//}
//
//btCollisionShape* btWorldImporter_createCapsuleShapeZ(btWorldImporter* obj, btScalar radius,
//	btScalar height)
//{
//	return obj->createCapsuleShapeZ(radius, height);
//}
//
//btCollisionShape* btWorldImporter_createCapsuleShapeX(btWorldImporter* obj, btScalar radius,
//	btScalar height)
//{
//	return obj->createCapsuleShapeX(radius, height);
//}
//
//btCollisionShape* btWorldImporter_createCapsuleShapeY(btWorldImporter* obj, btScalar radius,
//	btScalar height)
//{
//	return obj->createCapsuleShapeY(radius, height);
//}
//
//btCollisionObject* btWorldImporter_createCollisionObject(btWorldImporter* obj, const btTransform* startTransform,
//	btCollisionShape* shape, const char* bodyName)
//{
//	BTTRANSFORM_IN(startTransform);
//	return obj->createCollisionObject(BTTRANSFORM_USE(startTransform), shape, bodyName);
//}
//
//btCompoundShape* btWorldImporter_createCompoundShape(btWorldImporter* obj)
//{
//	return obj->createCompoundShape();
//}
//
//btCollisionShape* btWorldImporter_createConeShapeZ(btWorldImporter* obj, btScalar radius,
//	btScalar height)
//{
//	return obj->createConeShapeZ(radius, height);
//}
//
//btCollisionShape* btWorldImporter_createConeShapeX(btWorldImporter* obj, btScalar radius,
//	btScalar height)
//{
//	return obj->createConeShapeX(radius, height);
//}
//
//btCollisionShape* btWorldImporter_createConeShapeY(btWorldImporter* obj, btScalar radius,
//	btScalar height)
//{
//	return obj->createConeShapeY(radius, height);
//}
//
//btConeTwistConstraint* btWorldImporter_createConeTwistConstraint(btWorldImporter* obj,
//	btRigidBody* rbA, btRigidBody* rbB, const btTransform* rbAFrame, const btTransform* rbBFrame)
//{
//	BTTRANSFORM_IN(rbAFrame);
//	BTTRANSFORM_IN(rbBFrame);
//	return obj->createConeTwistConstraint(*rbA, *rbB, BTTRANSFORM_USE(rbAFrame),
//		BTTRANSFORM_USE(rbBFrame));
//}
//
//btConeTwistConstraint* btWorldImporter_createConeTwistConstraint2(btWorldImporter* obj,
//	btRigidBody* rbA, const btTransform* rbAFrame)
//{
//	BTTRANSFORM_IN(rbAFrame);
//	return obj->createConeTwistConstraint(*rbA, BTTRANSFORM_USE(rbAFrame));
//}
//
//btConvexHullShape* btWorldImporter_createConvexHullShape(btWorldImporter* obj)
//{
//	return obj->createConvexHullShape();
//}
//
//btCollisionShape* btWorldImporter_createConvexTriangleMeshShape(btWorldImporter* obj,
//	btStridingMeshInterface* trimesh)
//{
//	return obj->createConvexTriangleMeshShape(trimesh);
//}
//
//btCollisionShape* btWorldImporter_createCylinderShapeZ(btWorldImporter* obj, btScalar radius,
//	btScalar height)
//{
//	return obj->createCylinderShapeZ(radius, height);
//}
//
//btCollisionShape* btWorldImporter_createCylinderShapeX(btWorldImporter* obj, btScalar radius,
//	btScalar height)
//{
//	return obj->createCylinderShapeX(radius, height);
//}
//
//btCollisionShape* btWorldImporter_createCylinderShapeY(btWorldImporter* obj, btScalar radius,
//	btScalar height)
//{
//	return obj->createCylinderShapeY(radius, height);
//}
//
//btGearConstraint* btWorldImporter_createGearConstraint(btWorldImporter* obj, btRigidBody* rbA,
//	btRigidBody* rbB, const btVector3* axisInA, const btVector3* axisInB, btScalar ratio)
//{
//	BTVECTOR3_IN(axisInA);
//	BTVECTOR3_IN(axisInB);
//	return obj->createGearConstraint(*rbA, *rbB, BTVECTOR3_USE(axisInA), BTVECTOR3_USE(axisInB),
//		ratio);
//}
//
//btGeneric6DofConstraint* btWorldImporter_createGeneric6DofConstraint(btWorldImporter* obj,
//	btRigidBody* rbA, btRigidBody* rbB, const btTransform* frameInA, const btTransform* frameInB,
//	bool useLinearReferenceFrameA)
//{
//	BTTRANSFORM_IN(frameInA);
//	BTTRANSFORM_IN(frameInB);
//	return obj->createGeneric6DofConstraint(*rbA, *rbB, BTTRANSFORM_USE(frameInA),
//		BTTRANSFORM_USE(frameInB), useLinearReferenceFrameA);
//}
//
//btGeneric6DofConstraint* btWorldImporter_createGeneric6DofConstraint2(btWorldImporter* obj,
//	btRigidBody* rbB, const btTransform* frameInB, bool useLinearReferenceFrameB)
//{
//	BTTRANSFORM_IN(frameInB);
//	return obj->createGeneric6DofConstraint(*rbB, BTTRANSFORM_USE(frameInB), useLinearReferenceFrameB);
//}
//
//btGeneric6DofSpring2Constraint* btWorldImporter_createGeneric6DofSpring2Constraint(
//	btWorldImporter* obj, btRigidBody* rbA, btRigidBody* rbB, const btTransform* frameInA,
//	const btTransform* frameInB, int rotateOrder)
//{
//	BTTRANSFORM_IN(frameInA);
//	BTTRANSFORM_IN(frameInB);
//	return obj->createGeneric6DofSpring2Constraint(*rbA, *rbB, BTTRANSFORM_USE(frameInA),
//		BTTRANSFORM_USE(frameInB), rotateOrder);
//}
//
//btGeneric6DofSpringConstraint* btWorldImporter_createGeneric6DofSpringConstraint(
//	btWorldImporter* obj, btRigidBody* rbA, btRigidBody* rbB, const btTransform* frameInA,
//	const btTransform* frameInB, bool useLinearReferenceFrameA)
//{
//	BTTRANSFORM_IN(frameInA);
//	BTTRANSFORM_IN(frameInB);
//	return obj->createGeneric6DofSpringConstraint(*rbA, *rbB, BTTRANSFORM_USE(frameInA),
//		BTTRANSFORM_USE(frameInB), useLinearReferenceFrameA);
//}
//
//btGImpactMeshShape* btWorldImporter_createGimpactShape(btWorldImporter* obj, btStridingMeshInterface* trimesh)
//{
//	return obj->createGimpactShape(trimesh);
//}
//
//btHingeConstraint* btWorldImporter_createHingeConstraint(btWorldImporter* obj, btRigidBody* rbA,
//	btRigidBody* rbB, const btTransform* rbAFrame, const btTransform* rbBFrame)
//{
//	BTTRANSFORM_IN(rbAFrame);
//	BTTRANSFORM_IN(rbBFrame);
//	return obj->createHingeConstraint(*rbA, *rbB, BTTRANSFORM_USE(rbAFrame), BTTRANSFORM_USE(rbBFrame));
//}
//
//btHingeConstraint* btWorldImporter_createHingeConstraint2(btWorldImporter* obj, btRigidBody* rbA,
//	btRigidBody* rbB, const btTransform* rbAFrame, const btTransform* rbBFrame, bool useReferenceFrameA)
//{
//	BTTRANSFORM_IN(rbAFrame);
//	BTTRANSFORM_IN(rbBFrame);
//	return obj->createHingeConstraint(*rbA, *rbB, BTTRANSFORM_USE(rbAFrame), BTTRANSFORM_USE(rbBFrame),
//		useReferenceFrameA);
//}
//
//btHingeConstraint* btWorldImporter_createHingeConstraint3(btWorldImporter* obj, btRigidBody* rbA,
//	const btTransform* rbAFrame)
//{
//	BTTRANSFORM_IN(rbAFrame);
//	return obj->createHingeConstraint(*rbA, BTTRANSFORM_USE(rbAFrame));
//}
//
//btHingeConstraint* btWorldImporter_createHingeConstraint4(btWorldImporter* obj, btRigidBody* rbA,
//	const btTransform* rbAFrame, bool useReferenceFrameA)
//{
//	BTTRANSFORM_IN(rbAFrame);
//	return obj->createHingeConstraint(*rbA, BTTRANSFORM_USE(rbAFrame), useReferenceFrameA);
//}
//
//btTriangleIndexVertexArray* btWorldImporter_createMeshInterface(btWorldImporter* obj,
//	btStridingMeshInterfaceData* meshData)
//{
//	return obj->createMeshInterface(*meshData);
//}
//
//btMultiSphereShape* btWorldImporter_createMultiSphereShape(btWorldImporter* obj,
//	const btVector3* positions, const btScalar* radi, int numSpheres)
//{
//	return obj->createMultiSphereShape(positions, radi, numSpheres);
//}
//
//btOptimizedBvh* btWorldImporter_createOptimizedBvh(btWorldImporter* obj)
//{
//	return obj->createOptimizedBvh();
//}
//
//btCollisionShape* btWorldImporter_createPlaneShape(btWorldImporter* obj, const btVector3* planeNormal,
//	btScalar planeConstant)
//{
//	BTVECTOR3_IN(planeNormal);
//	return obj->createPlaneShape(BTVECTOR3_USE(planeNormal), planeConstant);
//}
//
//btPoint2PointConstraint* btWorldImporter_createPoint2PointConstraint(btWorldImporter* obj,
//	btRigidBody* rbA, const btVector3* pivotInA)
//{
//	BTVECTOR3_IN(pivotInA);
//	return obj->createPoint2PointConstraint(*rbA, BTVECTOR3_USE(pivotInA));
//}
//
//btPoint2PointConstraint* btWorldImporter_createPoint2PointConstraint2(btWorldImporter* obj,
//	btRigidBody* rbA, btRigidBody* rbB, const btVector3* pivotInA, const btVector3* pivotInB)
//{
//	BTVECTOR3_IN(pivotInA);
//	BTVECTOR3_IN(pivotInB);
//	return obj->createPoint2PointConstraint(*rbA, *rbB, BTVECTOR3_USE(pivotInA),
//		BTVECTOR3_USE(pivotInB));
//}
//
//btRigidBody* btWorldImporter_createRigidBody(btWorldImporter* obj, bool isDynamic,
//	btScalar mass, const btTransform* startTransform, btCollisionShape* shape, const char* bodyName)
//{
//	BTTRANSFORM_IN(startTransform);
//	return obj->createRigidBody(isDynamic, mass, BTTRANSFORM_USE(startTransform),
//		shape, bodyName);
//}
//
//btScaledBvhTriangleMeshShape* btWorldImporter_createScaledTrangleMeshShape(btWorldImporter* obj,
//	btBvhTriangleMeshShape* meshShape, const btVector3* localScalingbtBvhTriangleMeshShape)
//{
//	BTVECTOR3_IN(localScalingbtBvhTriangleMeshShape);
//	return obj->createScaledTrangleMeshShape(meshShape, BTVECTOR3_USE(localScalingbtBvhTriangleMeshShape));
//}
//
//btSliderConstraint* btWorldImporter_createSliderConstraint(btWorldImporter* obj,
//	btRigidBody* rbA, btRigidBody* rbB, const btTransform* frameInA, const btTransform* frameInB,
//	bool useLinearReferenceFrameA)
//{
//	BTTRANSFORM_IN(frameInA);
//	BTTRANSFORM_IN(frameInB);
//	return obj->createSliderConstraint(*rbA, *rbB, BTTRANSFORM_USE(frameInA), BTTRANSFORM_USE(frameInB),
//		useLinearReferenceFrameA);
//}
//
//btSliderConstraint* btWorldImporter_createSliderConstraint2(btWorldImporter* obj,
//	btRigidBody* rbB, const btTransform* frameInB, bool useLinearReferenceFrameA)
//{
//	BTTRANSFORM_IN(frameInB);
//	return obj->createSliderConstraint(*rbB, BTTRANSFORM_USE(frameInB), useLinearReferenceFrameA);
//}
//
//btCollisionShape* btWorldImporter_createSphereShape(btWorldImporter* obj, btScalar radius)
//{
//	return obj->createSphereShape(radius);
//}
//
//btStridingMeshInterfaceData* btWorldImporter_createStridingMeshInterfaceData(btWorldImporter* obj,
//	btStridingMeshInterfaceData* interfaceData)
//{
//	return obj->createStridingMeshInterfaceData(interfaceData);
//}
//
//btTriangleInfoMap* btWorldImporter_createTriangleInfoMap(btWorldImporter* obj)
//{
//	return obj->createTriangleInfoMap();
//}
//
//btTriangleIndexVertexArray* btWorldImporter_createTriangleMeshContainer(btWorldImporter* obj)
//{
//	return obj->createTriangleMeshContainer();
//}
//
//void btWorldImporter_deleteAllData(btWorldImporter* obj)
//{
//	obj->deleteAllData();
//}
//
//btOptimizedBvh* btWorldImporter_getBvhByIndex(btWorldImporter* obj, int index)
//{
//	return obj->getBvhByIndex(index);
//}
//
//btCollisionShape* btWorldImporter_getCollisionShapeByIndex(btWorldImporter* obj,
//	int index)
//{
//	return obj->getCollisionShapeByIndex(index);
//}
//
//btCollisionShape* btWorldImporter_getCollisionShapeByName(btWorldImporter* obj, const char* name)
//{
//	return obj->getCollisionShapeByName(name);
//}
//
//btTypedConstraint* btWorldImporter_getConstraintByIndex(btWorldImporter* obj, int index)
//{
//	return obj->getConstraintByIndex(index);
//}
//
//btTypedConstraint* btWorldImporter_getConstraintByName(btWorldImporter* obj, const char* name)
//{
//	return obj->getConstraintByName(name);
//}
//
//const char* btWorldImporter_getNameForPointer(btWorldImporter* obj, const void* ptr)
//{
//	return obj->getNameForPointer(ptr);
//}
//
//int btWorldImporter_getNumBvhs(btWorldImporter* obj)
//{
//	return obj->getNumBvhs();
//}
//
//int btWorldImporter_getNumCollisionShapes(btWorldImporter* obj)
//{
//	return obj->getNumCollisionShapes();
//}
//
//int btWorldImporter_getNumConstraints(btWorldImporter* obj)
//{
//	return obj->getNumConstraints();
//}
//
//int btWorldImporter_getNumRigidBodies(btWorldImporter* obj)
//{
//	return obj->getNumRigidBodies();
//}
//
//int btWorldImporter_getNumTriangleInfoMaps(btWorldImporter* obj)
//{
//	return obj->getNumTriangleInfoMaps();
//}
//
//btCollisionObject* btWorldImporter_getRigidBodyByIndex(btWorldImporter* obj, int index)
//{
//	return obj->getRigidBodyByIndex(index);
//}
//
//btRigidBody* btWorldImporter_getRigidBodyByName(btWorldImporter* obj, const char* name)
//{
//	return obj->getRigidBodyByName(name);
//}
//
//btTriangleInfoMap* btWorldImporter_getTriangleInfoMapByIndex(btWorldImporter* obj,
//	int index)
//{
//	return obj->getTriangleInfoMapByIndex(index);
//}
//
//int btWorldImporter_getVerboseMode(btWorldImporter* obj)
//{
//	return obj->getVerboseMode();
//}
//
//void btWorldImporter_setDynamicsWorldInfo(btWorldImporter* obj, const btVector3* gravity,
//	const btContactSolverInfo* solverInfo)
//{
//	BTVECTOR3_IN(gravity);
//	obj->setDynamicsWorldInfo(BTVECTOR3_USE(gravity), *solverInfo);
//}
//
//void btWorldImporter_setVerboseMode(btWorldImporter* obj, int verboseMode)
//{
//	obj->setVerboseMode(verboseMode);
//}
//
//void btWorldImporter_delete(btWorldImporter* obj)
//{
//	delete obj;
//}
//
//#endif
