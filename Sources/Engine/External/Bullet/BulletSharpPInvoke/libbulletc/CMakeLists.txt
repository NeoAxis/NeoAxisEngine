CMAKE_MINIMUM_REQUIRED (VERSION 2.8.9)
PROJECT (libbulletc)

IF (NOT CMAKE_BUILD_TYPE)
    # SET(CMAKE_BUILD_TYPE "Debug")
    SET(CMAKE_BUILD_TYPE "Release")
ENDIF (NOT CMAKE_BUILD_TYPE)

FIND_PATH(BULLET_INCLUDE_DIR NAMES btBulletCollisionCommon.h
    PATHS
    ${PROJECT_SOURCE_DIR}/../../bullet/src/
    ${PROJECT_SOURCE_DIR}/../bullet/src/
    ${PROJECT_SOURCE_DIR}/../../bullet3/src/
    ${PROJECT_SOURCE_DIR}/../bullet3/src/
    ENV CPATH
    /usr/include
    /usr/local/include
    /opt/local/include
    NO_DEFAULT_PATH
)

OPTION(BUILD_BULLET2_DEMOS "" OFF)
OPTION(BUILD_BULLET3 "" OFF)
OPTION(BUILD_CPU_DEMOS "" OFF)
OPTION(BUILD_UNIT_TESTS "" OFF)
SET(CMAKE_POSITION_INDEPENDENT_CODE ON)

ADD_SUBDIRECTORY("${BULLET_INCLUDE_DIR}/.." "${CMAKE_CURRENT_BINARY_DIR}/bullet")

INCLUDE_DIRECTORIES(${BULLET_INCLUDE_DIR})
INCLUDE_DIRECTORIES(${BULLET_PHYSICS_SOURCE_DIR}/Extras/HACD/)
INCLUDE_DIRECTORIES(${BULLET_PHYSICS_SOURCE_DIR}/Extras/Serialize/BulletWorldImporter/)
INCLUDE_DIRECTORIES(${BULLET_PHYSICS_SOURCE_DIR}/Extras/Serialize/BulletXmlWorldImporter/)

IF(${CMAKE_GENERATOR} MATCHES "Unix Makefiles")
    SET(BULLETC_LIB bulletc)
ELSE()
    SET(BULLETC_LIB libbulletc)
ENDIF()

IF (USE_DOUBLE_PRECISION)
    ADD_DEFINITIONS( -DBT_USE_DOUBLE_PRECISION)
ENDIF (USE_DOUBLE_PRECISION)

IF(MSVC)
    IF (NOT USE_MSVC_RUNTIME_LIBRARY_DLL)
        #We statically link to reduce dependancies
        FOREACH(flag_var CMAKE_CXX_FLAGS CMAKE_CXX_FLAGS_DEBUG CMAKE_CXX_FLAGS_RELEASE CMAKE_CXX_FLAGS_MINSIZEREL CMAKE_CXX_FLAGS_RELWITHDEBINFO CMAKE_C_FLAGS CMAKE_C_FLAGS_DEBUG CMAKE_C_FLAGS_RELEASE CMAKE_C_FLAGS_MINSIZEREL CMAKE_C_FLAGS_RELWITHDEBINFO )
            IF(${flag_var} MATCHES "/MD")
                STRING(REGEX REPLACE "/MD" "/MT" ${flag_var} "${${flag_var}}")
            ENDIF(${flag_var} MATCHES "/MD")
            IF(${flag_var} MATCHES "/MDd")
                STRING(REGEX REPLACE "/MDd" "/MTd" ${flag_var} "${${flag_var}}")
            ENDIF(${flag_var} MATCHES "/MDd")
        ENDFOREACH(flag_var)
    ENDIF()

    IF (CMAKE_CL_64)
        ADD_DEFINITIONS(-D_WIN64)
    ELSE()
        IF (USE_MSVC_SSE)
            ADD_DEFINITIONS(/arch:SSE)
        ENDIF()
        IF (USE_MSVC_SSE2)
            ADD_DEFINITIONS(/arch:SSE2)
        ENDIF()
    ENDIF()

    IF (USE_MSVC_AVX)
        ADD_DEFINITIONS(/arch:AVX)
    ENDIF()

    IF (USE_MSVC_FAST_FLOATINGPOINT)
        ADD_DEFINITIONS(/fp:fast)
    ENDIF()
ENDIF(MSVC)

ADD_LIBRARY(${BULLETC_LIB} SHARED
    dllmain.cpp
    src/conversion.h
    src/main.h
    src/collections.cpp
    src/collections.h
    src/btActionInterface_wrap.cpp
    src/btActionInterface_wrap.h
    src/btAlignedObjectArray_wrap.cpp
    src/btAlignedObjectArray_wrap.h
    src/btAxisSweep3_wrap.cpp
    src/btAxisSweep3_wrap.h
    src/btBox2dBox2dCollisionAlgorithm_wrap.cpp
    src/btBox2dBox2dCollisionAlgorithm_wrap.h
    src/btBox2dShape_wrap.cpp
    src/btBox2dShape_wrap.h
    src/btBoxBoxCollisionAlgorithm_wrap.cpp
    src/btBoxBoxCollisionAlgorithm_wrap.h
    src/btBoxBoxDetector_wrap.cpp
    src/btBoxBoxDetector_wrap.h
    src/btBoxCollision_wrap.cpp
    src/btBoxCollision_wrap.h
    src/btBoxShape_wrap.cpp
    src/btBoxShape_wrap.h
    src/btBroadphaseInterface_wrap.cpp
    src/btBroadphaseInterface_wrap.h
    src/btBroadphaseProxy_wrap.cpp
    src/btBroadphaseProxy_wrap.h
    src/btBulletFile_wrap.cpp
    src/btBulletFile_wrap.h
    src/btBulletWorldImporter_wrap.cpp
    src/btBulletWorldImporter_wrap.h
    src/btBulletXmlWorldImporter_wrap.cpp
    src/btBulletXmlWorldImporter_wrap.h
    src/btBvhTriangleMeshShape_wrap.cpp
    src/btBvhTriangleMeshShape_wrap.h
    src/btCapsuleShape_wrap.cpp
    src/btCapsuleShape_wrap.h
    src/btCharacterControllerInterface_wrap.cpp
    src/btCharacterControllerInterface_wrap.h
    src/btCollisionAlgorithm_wrap.cpp
    src/btCollisionAlgorithm_wrap.h
    src/btCollisionConfiguration_wrap.cpp
    src/btCollisionConfiguration_wrap.h
    src/btCollisionCreateFunc_wrap.cpp
    src/btCollisionCreateFunc_wrap.h
    src/btCollisionDispatcher_wrap.cpp
    src/btCollisionDispatcher_wrap.h
    src/btCollisionDispatcherMt_wrap.cpp
    src/btCollisionDispatcherMt_wrap.h
    src/btCollisionObject_wrap.cpp
    src/btCollisionObject_wrap.h
    src/btCollisionObjectWrapper_wrap.cpp
    src/btCollisionObjectWrapper_wrap.h
    src/btCollisionShape_wrap.cpp
    src/btCollisionShape_wrap.h
    src/btCollisionWorld_wrap.cpp
    src/btCollisionWorld_wrap.h
    src/btCompoundCollisionAlgorithm_wrap.cpp
    src/btCompoundCollisionAlgorithm_wrap.h
    src/btCompoundCompoundCollisionAlgorithm_wrap.cpp
    src/btCompoundCompoundCollisionAlgorithm_wrap.h
    src/btCompoundFromGimpact_wrap.cpp
    src/btCompoundFromGimpact_wrap.h
    src/btCompoundShape_wrap.cpp
    src/btCompoundShape_wrap.h
    src/btConcaveShape_wrap.cpp
    src/btConcaveShape_wrap.h
    src/btConeShape_wrap.cpp
    src/btConeShape_wrap.h
    src/btConeTwistConstraint_wrap.cpp
    src/btConeTwistConstraint_wrap.h
    src/btConstraintSolver_wrap.cpp
    src/btConstraintSolver_wrap.h
    src/btContactConstraint_wrap.cpp
    src/btContactConstraint_wrap.h
    src/btContactSolverInfo_wrap.cpp
    src/btContactSolverInfo_wrap.h
    src/btContinuousConvexCollision_wrap.cpp
    src/btContinuousConvexCollision_wrap.h
    src/btConvex2dConvex2dAlgorithm_wrap.cpp
    src/btConvex2dConvex2dAlgorithm_wrap.h
    src/btConvex2dShape_wrap.cpp
    src/btConvex2dShape_wrap.h
    src/btConvexCast_wrap.cpp
    src/btConvexCast_wrap.h
    src/btConvexConcaveCollisionAlgorithm_wrap.cpp
    src/btConvexConcaveCollisionAlgorithm_wrap.h
    src/btConvexConvexAlgorithm_wrap.cpp
    src/btConvexConvexAlgorithm_wrap.h
    src/btConvexHullShape_wrap.cpp
    src/btConvexHullShape_wrap.h
    src/btConvexInternalShape_wrap.cpp
    src/btConvexInternalShape_wrap.h
    src/btConvexPenetrationDepthSolver_wrap.cpp
    src/btConvexPenetrationDepthSolver_wrap.h
    src/btConvexPlaneCollisionAlgorithm_wrap.cpp
    src/btConvexPlaneCollisionAlgorithm_wrap.h
    src/btConvexPointCloudShape_wrap.cpp
    src/btConvexPointCloudShape_wrap.h
    src/btConvexPolyhedron_wrap.cpp
    src/btConvexPolyhedron_wrap.h
    src/btConvexShape_wrap.cpp
    src/btConvexShape_wrap.h
    src/btConvexTriangleMeshShape_wrap.cpp
    src/btConvexTriangleMeshShape_wrap.h
    src/btCpuFeatureUtility_wrap.cpp
    src/btCpuFeatureUtility_wrap.h
    src/btCylinderShape_wrap.cpp
    src/btCylinderShape_wrap.h
    src/btDantzigSolver_wrap.cpp
    src/btDantzigSolver_wrap.h
    src/btDbvt_wrap.cpp
    src/btDbvt_wrap.h
    src/btDbvtBroadphase_wrap.cpp
    src/btDbvtBroadphase_wrap.h
    src/btDefaultCollisionConfiguration_wrap.cpp
    src/btDefaultCollisionConfiguration_wrap.h
    src/btDefaultMotionState_wrap.cpp
    src/btDefaultMotionState_wrap.h
    src/btDefaultSoftBodySolver_wrap.cpp
    src/btDefaultSoftBodySolver_wrap.h
    src/btDiscreteCollisionDetectorInterface_wrap.cpp
    src/btDiscreteCollisionDetectorInterface_wrap.h
    src/btDiscreteDynamicsWorld_wrap.cpp
    src/btDiscreteDynamicsWorld_wrap.h
    src/btDiscreteDynamicsWorldMt_wrap.cpp
    src/btDiscreteDynamicsWorldMt_wrap.h
    src/btDispatcher_wrap.cpp
    src/btDispatcher_wrap.h
    src/btDynamicsWorld_wrap.cpp
    src/btDynamicsWorld_wrap.h
    src/btEmptyCollisionAlgorithm_wrap.cpp
    src/btEmptyCollisionAlgorithm_wrap.h
    src/btEmptyShape_wrap.cpp
    src/btEmptyShape_wrap.h
    src/btFixedConstraint_wrap.cpp
    src/btFixedConstraint_wrap.h
    src/btGearConstraint_wrap.cpp
    src/btGearConstraint_wrap.h
    src/btGeneric6DofConstraint_wrap.cpp
    src/btGeneric6DofConstraint_wrap.h
    src/btGeneric6DofSpring2Constraint_wrap.cpp
    src/btGeneric6DofSpring2Constraint_wrap.h
    src/btGeneric6DofSpringConstraint_wrap.cpp
    src/btGeneric6DofSpringConstraint_wrap.h
    src/btGeometryUtil_wrap.cpp
    src/btGeometryUtil_wrap.h
    src/btGhostObject_wrap.cpp
    src/btGhostObject_wrap.h
    src/btGImpactBvh_wrap.cpp
    src/btGImpactBvh_wrap.h
    src/btGImpactCollisionAlgorithm_wrap.cpp
    src/btGImpactCollisionAlgorithm_wrap.h
    src/btGImpactQuantizedBvh_wrap.cpp
    src/btGImpactQuantizedBvh_wrap.h
    src/btGImpactShape_wrap.cpp
    src/btGImpactShape_wrap.h
    src/btGjkConvexCast_wrap.cpp
    src/btGjkConvexCast_wrap.h
    src/btGjkEpaPenetrationDepthSolver_wrap.cpp
    src/btGjkEpaPenetrationDepthSolver_wrap.h
    src/btGjkPairDetector_wrap.cpp
    src/btGjkPairDetector_wrap.h
    src/btHeightfieldTerrainShape_wrap.cpp
    src/btHeightfieldTerrainShape_wrap.h
    src/btHinge2Constraint_wrap.cpp
    src/btHinge2Constraint_wrap.h
    src/btHingeConstraint_wrap.cpp
    src/btHingeConstraint_wrap.h
    src/btIDebugDraw_wrap.cpp
    src/btIDebugDraw_wrap.h
    src/btKinematicCharacterController_wrap.cpp
    src/btKinematicCharacterController_wrap.h
    src/btLemkeSolver_wrap.cpp
    src/btLemkeSolver_wrap.h
    src/btManifoldPoint_wrap.cpp
    src/btManifoldPoint_wrap.h
    src/btManifoldResult_wrap.cpp
    src/btManifoldResult_wrap.h
    src/btMinkowskiPenetrationDepthSolver_wrap.cpp
    src/btMinkowskiPenetrationDepthSolver_wrap.h
    src/btMinkowskiSumShape_wrap.cpp
    src/btMinkowskiSumShape_wrap.h
    src/btMLCPSolver_wrap.cpp
    src/btMLCPSolver_wrap.h
    src/btMLCPSolverInterface_wrap.cpp
    src/btMLCPSolverInterface_wrap.h
    src/btMotionState_wrap.cpp
    src/btMotionState_wrap.h
    src/btMultiBody_wrap.cpp
    src/btMultiBody_wrap.h
    src/btMultiBodyConstraint_wrap.cpp
    src/btMultiBodyConstraint_wrap.h
    src/btMultiBodyConstraintSolver_wrap.cpp
    src/btMultiBodyConstraintSolver_wrap.h
    src/btMultiBodyDynamicsWorld_wrap.cpp
    src/btMultiBodyDynamicsWorld_wrap.h
    src/btMultiBodyFixedConstraint_wrap.cpp
    src/btMultiBodyFixedConstraint_wrap.h
    src/btMultiBodyJointLimitConstraint_wrap.cpp
    src/btMultiBodyJointLimitConstraint_wrap.h
    src/btMultiBodyJointMotor_wrap.cpp
    src/btMultiBodyJointMotor_wrap.h
    src/btMultiBodyLink_wrap.cpp
    src/btMultiBodyLink_wrap.h
    src/btMultiBodyLinkCollider_wrap.cpp
    src/btMultiBodyLinkCollider_wrap.h
    src/btMultiBodyPoint2Point_wrap.cpp
    src/btMultiBodyPoint2Point_wrap.h
    src/btMultiBodySliderConstraint_wrap.cpp
    src/btMultiBodySliderConstraint_wrap.h
    src/btMultiBodySolverConstraint_wrap.cpp
    src/btMultiBodySolverConstraint_wrap.h
    src/btMultimaterialTriangleMeshShape_wrap.cpp
    src/btMultimaterialTriangleMeshShape_wrap.h
    src/btMultiSphereShape_wrap.cpp
    src/btMultiSphereShape_wrap.h
    src/btNNCGConstraintSolver_wrap.cpp
    src/btNNCGConstraintSolver_wrap.h
    src/btOptimizedBvh_wrap.cpp
    src/btOptimizedBvh_wrap.h
    src/btOverlappingPairCache_wrap.cpp
    src/btOverlappingPairCache_wrap.h
    src/btOverlappingPairCallback_wrap.cpp
    src/btOverlappingPairCallback_wrap.h
    src/btPersistentManifold_wrap.cpp
    src/btPersistentManifold_wrap.h
    src/btPoint2PointConstraint_wrap.cpp
    src/btPoint2PointConstraint_wrap.h
    src/btPointCollector_wrap.cpp
    src/btPointCollector_wrap.h
    src/btPolarDecomposition_wrap.cpp
    src/btPolarDecomposition_wrap.h
    src/btPolyhedralConvexShape_wrap.cpp
    src/btPolyhedralConvexShape_wrap.h
    src/btQuantizedBvh_wrap.cpp
    src/btQuantizedBvh_wrap.h
    src/btRaycastVehicle_wrap.cpp
    src/btRaycastVehicle_wrap.h
    src/btRigidBody_wrap.cpp
    src/btRigidBody_wrap.h
    src/btScaledBvhTriangleMeshShape_wrap.cpp
    src/btScaledBvhTriangleMeshShape_wrap.h
    src/btSequentialImpulseConstraintSolver_wrap.cpp
    src/btSequentialImpulseConstraintSolver_wrap.h
    src/btSerializer_wrap.cpp
    src/btSerializer_wrap.h
    src/btShapeHull_wrap.cpp
    src/btShapeHull_wrap.h
    src/btSimulationIslandManager_wrap.cpp
    src/btSimulationIslandManager_wrap.h
    src/btSliderConstraint_wrap.cpp
    src/btSliderConstraint_wrap.h
    src/btSoftBody_wrap.cpp
    src/btSoftBody_wrap.h
    src/btSoftBodyConcaveCollisionAlgorithm_wrap.cpp
    src/btSoftBodyConcaveCollisionAlgorithm_wrap.h
    src/btSoftBodyHelpers_wrap.cpp
    src/btSoftBodyHelpers_wrap.h
    src/btSoftBodyRigidBodyCollisionConfiguration_wrap.cpp
    src/btSoftBodyRigidBodyCollisionConfiguration_wrap.h
    src/btSoftBodySolvers_wrap.cpp
    src/btSoftBodySolvers_wrap.h
    src/btSoftRigidCollisionAlgorithm_wrap.cpp
    src/btSoftRigidCollisionAlgorithm_wrap.h
    src/btSoftRigidDynamicsWorld_wrap.cpp
    src/btSoftRigidDynamicsWorld_wrap.h
    src/btSoftSoftCollisionAlgorithm_wrap.cpp
    src/btSoftSoftCollisionAlgorithm_wrap.h
    src/btSparseSdf3_wrap.cpp
    src/btSparseSdf3_wrap.h
    src/btSphereBoxCollisionAlgorithm_wrap.cpp
    src/btSphereBoxCollisionAlgorithm_wrap.h
    src/btSphereShape_wrap.cpp
    src/btSphereShape_wrap.h
    src/btSphereSphereCollisionAlgorithm_wrap.cpp
    src/btSphereSphereCollisionAlgorithm_wrap.h
    src/btSphereTriangleCollisionAlgorithm_wrap.cpp
    src/btSphereTriangleCollisionAlgorithm_wrap.h
    src/btStaticPlaneShape_wrap.cpp
    src/btStaticPlaneShape_wrap.h
    src/btStridingMeshInterface_wrap.cpp
    src/btStridingMeshInterface_wrap.h
    src/btTetrahedronShape_wrap.cpp
    src/btTetrahedronShape_wrap.h
    src/btThreads_wrap.cpp
    src/btThreads_wrap.h
    src/btTransformUtil_wrap.cpp
    src/btTransformUtil_wrap.h
    src/btTriangleBuffer_wrap.cpp
    src/btTriangleBuffer_wrap.h
    src/btTriangleCallback_wrap.cpp
    src/btTriangleCallback_wrap.h
    src/btTriangleIndexVertexArray_wrap.cpp
    src/btTriangleIndexVertexArray_wrap.h
    src/btTriangleIndexVertexMaterialArray_wrap.cpp
    src/btTriangleIndexVertexMaterialArray_wrap.h
    src/btTriangleMesh_wrap.cpp
    src/btTriangleMesh_wrap.h
    src/btTriangleMeshShape_wrap.cpp
    src/btTriangleMeshShape_wrap.h
    src/btTriangleShape_wrap.cpp
    src/btTriangleShape_wrap.h
    src/btTriangleShapeEx_wrap.cpp
    src/btTriangleShapeEx_wrap.h
    src/btTypedConstraint_wrap.cpp
    src/btTypedConstraint_wrap.h
    src/btUniformScalingShape_wrap.cpp
    src/btUniformScalingShape_wrap.h
    src/btUnionFind_wrap.cpp
    src/btUnionFind_wrap.h
    src/btUniversalConstraint_wrap.cpp
    src/btUniversalConstraint_wrap.h
    src/btVehicleRaycaster_wrap.cpp
    src/btVehicleRaycaster_wrap.h
    src/btVoronoiSimplexSolver_wrap.cpp
    src/btVoronoiSimplexSolver_wrap.h
    src/btWheelInfo_wrap.cpp
    src/btWheelInfo_wrap.h
    src/btWorldImporter_wrap.cpp
    src/btWorldImporter_wrap.h
    src/hacdHACD_wrap.cpp
    src/hacdHACD_wrap.h
)

TARGET_LINK_LIBRARIES(${BULLETC_LIB} BulletCollision BulletDynamics LinearMath BulletFileLoader BulletSoftBody BulletWorldImporter BulletXmlWorldImporter HACD)

ADD_SUBDIRECTORY(test)
