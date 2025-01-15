// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
// SPDX-FileCopyrightText: 2021 Jorrit Rouwe
// SPDX-License-Identifier: MIT
#include "OgreStableHeaders.h"
#include "MemoryManager.h"
#include "NeoAxisCoreNative.h"
#include "NeoAxisPhysicsNative.h"

// The Jolt headers don't include Jolt.h. Always include Jolt.h before including any other Jolt header.
// You can use Jolt.h in your precompiled header to speed up compilation.
#include <Jolt/Jolt.h>

// Jolt includes
#include <Jolt/RegisterTypes.h>
#include <Jolt/Core/Factory.h>
#include <Jolt/Core/TempAllocator.h>
#include <Jolt/Core/JobSystemThreadPool.h>
#include <Jolt/Renderer/DebugRenderer.h>
//#include <Jolt/Renderer/DebugRendererRecorder.h>
#include <Jolt/Physics/PhysicsSettings.h>
#include <Jolt/Physics/PhysicsSystem.h>
#include <Jolt/Physics/Collision/CollisionDispatch.h>
#include <Jolt/Physics/Collision/Shape/TriangleShape.h>
#include <Jolt/Physics/Collision/Shape/SphereShape.h>
#include <Jolt/Physics/Collision/Shape/BoxShape.h>
#include <Jolt/Physics/Collision/Shape/CapsuleShape.h>
#include <Jolt/Physics/Collision/Shape/TaperedCapsuleShape.h>
#include <Jolt/Physics/Collision/Shape/CylinderShape.h>
#include <Jolt/Physics/Collision/Shape/ScaledShape.h>
#include <Jolt/Physics/Collision/Shape/MeshShape.h>
#include <Jolt/Physics/Collision/Shape/ConvexHullShape.h>
#include <Jolt/Physics/Collision/Shape/HeightFieldShape.h>
#include <Jolt/Physics/Collision/Shape/RotatedTranslatedShape.h>
#include <Jolt/Physics/Collision/Shape/OffsetCenterOfMassShape.h>
#include <Jolt/Physics/Collision/Shape/MutableCompoundShape.h>
#include <Jolt/Physics/Collision/Shape/StaticCompoundShape.h>
#include <Jolt/Physics/Collision/RayCast.h>
#include <Jolt/Physics/Collision/CastResult.h>
#include <Jolt/Physics/Collision/CollisionCollectorImpl.h>
#include <Jolt/Physics/Body/BodyCreationSettings.h>
#include <Jolt/Physics/Body/BodyActivationListener.h>
#include <Jolt/Physics/Constraints/FixedConstraint.h>
#include <Jolt/Physics/Constraints/SixDOFConstraint.h>
#include <Jolt/Physics/Vehicle/VehicleConstraint.h>
#include <Jolt/Physics/Vehicle/WheeledVehicleController.h>
#include <Jolt/Physics/Vehicle/TrackedVehicleController.h>
#include <Jolt/Physics/Character/CharacterVirtual.h>
#include <Jolt/Math/Math.h>

// STL includes
#include <iostream>
#include <cstdarg>
#include <thread>

//// Disable common warnings triggered by Jolt, you can use JPH_SUPPRESS_WARNING_PUSH / JPH_SUPPRESS_WARNING_POP to store and restore the warning state
//JPH_SUPPRESS_WARNINGS

// All Jolt symbols are in the JPH namespace
using namespace JPH;
using namespace Ogre;

// If you want your code to compile using single or double precision write 0.0_r to get a Real value that compiles to double or float depending if JPH_DOUBLE_PRECISION is set or not.
using namespace JPH::literals;

// We're also using STL classes in this example
using namespace std;

class BodyItem;
class ConstraintItem;
class PhysicsSystemItem;
class VehicleConstraintItem;

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Vec3 ConvertToVec3(const Vector3& value)
{
	return Vec3(value.x, value.y, value.z);
}

Vec3 ConvertToVec3(const Vector3D& value)
{
	return Vec3((float)value.x, (float)value.y, (float)value.z);
}

DVec3 ConvertToDVec3(const Vector3D& value)
{
	return DVec3(value.x, value.y, value.z);
}

RVec3 ConvertToRVec3(const Vector3D& value)
{
	return RVec3(value.x, value.y, value.z);
}

Vector3 ConvertToVector3(const Vec3& value)
{
	return Vector3(value.GetX(), value.GetY(), value.GetZ());
}

Vector3D ConvertToVector3D(const DVec3& value)
{
	return Vector3D(value.GetX(), value.GetY(), value.GetZ());
}

Vector3D ConvertToVector3D(const Vec3& value)
{
	return Vector3D(value.GetX(), value.GetY(), value.GetZ());
}

Float3 ConvertToFloat3(const Vector3& value)
{
	return Float3(value.x, value.y, value.z);
}

RRayCast ConvertToRRay(const RayD& value)
{
	return RRayCast(ConvertToRVec3(value.getOrigin()), ConvertToVec3(value.getDirection()));
}

RayCast ConvertToRay(const RayD& value)
{
	return RayCast(ConvertToVec3(value.getOrigin()), ConvertToVec3(value.getDirection()));
}

Quat ConvertToQuat(const Vector4& value)
{
	return Quat(value.x, value.y, value.z, value.w);
}

Vector4 ConvertToQuaternion(const Quat& value)
{
	return Vector4(value.GetX(), value.GetY(), value.GetZ(), value.GetW());
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//!!!!native memory allocator uses crt right now

void* MemoryAlloc(size_t inSize)
{
	return Memory_Alloc(MemoryAllocationType_Physics, inSize, __FILE__, __LINE__);
}

void* MemoryAlignedAlloc(size_t inSize, size_t inAlignment)
{
	return Memory_AllocAligned(MemoryAllocationType_Physics, inSize, inAlignment, __FILE__, __LINE__);
}

void MemoryFree(void* inPointer)
{
	Memory_Free(inPointer);
}

void MemoryAlignedFree(void* inPointer)
{
	Memory_FreeAligned(inPointer);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// Layer that objects can be in, determines which other objects it can collide with
namespace Layers
{
	static constexpr ObjectLayer NON_MOVING = 0;
	static constexpr ObjectLayer MOVING = 1;
	static constexpr ObjectLayer CHARACTER = 2;//DEBRIS = 6; // Example: Debris collides only with NON_MOVING
	static constexpr ObjectLayer MOVING_AND_CHARACTER = 3;//DEBRIS = 6; // Example: Debris collides only with NON_MOVING
	static constexpr ObjectLayer NUM_LAYERS = 4;

	//static constexpr ObjectLayer UNUSED1 = 0; // 4 unused values so that broadphase layers values don't match with object layer values (for testing purposes)
	//static constexpr ObjectLayer UNUSED2 = 1;
	//static constexpr ObjectLayer UNUSED3 = 2;
	//static constexpr ObjectLayer UNUSED4 = 3;
	//static constexpr ObjectLayer NON_MOVING = 4;
	//static constexpr ObjectLayer MOVING = 5;
	//static constexpr ObjectLayer CHARACTER = 6;//DEBRIS = 6; // Example: Debris collides only with NON_MOVING
	//static constexpr ObjectLayer SENSOR = 7; // Sensors only collide with MOVING objects
	//static constexpr ObjectLayer NUM_LAYERS = 8;
};

/// Class that determines if two object layers can collide
class ObjectLayerPairFilterImpl : public ObjectLayerPairFilter
{
public:
	virtual bool ShouldCollide(ObjectLayer inObject1, ObjectLayer inObject2) const override
	{
		switch (inObject1)
		{
			//case Layers::UNUSED1:
			//case Layers::UNUSED2:
			//case Layers::UNUSED3:
			//case Layers::UNUSED4:
			//	return false;
		case Layers::NON_MOVING:
			return inObject2 == Layers::MOVING;// || inObject2 == Layers::DEBRIS;
		case Layers::MOVING:
			return inObject2 == Layers::NON_MOVING || inObject2 == Layers::MOVING;// || inObject2 == Layers::SENSOR;
		case Layers::CHARACTER://DEBRIS:
			return false;//return inObject2 == Layers::NON_MOVING;
		case Layers::MOVING_AND_CHARACTER:
			return inObject2 == Layers::NON_MOVING || inObject2 == Layers::MOVING || inObject2 == Layers::CHARACTER;// || inObject2 == Layers::SENSOR;
		//case Layers::SENSOR:
		//	return inObject2 == Layers::MOVING;
		default:
			JPH_ASSERT(false);
			return false;
		}
	}
};

/// Broadphase layers
namespace BroadPhaseLayers
{
	static constexpr BroadPhaseLayer NON_MOVING(0);
	static constexpr BroadPhaseLayer MOVING(1);
	static constexpr BroadPhaseLayer CHARACTER(2);//DEBRIS(2);
	static constexpr uint NUM_LAYERS(3);

	//static constexpr BroadPhaseLayer SENSOR(3);
	//static constexpr BroadPhaseLayer UNUSED(4);
	//static constexpr uint NUM_LAYERS(5);
};

/// BroadPhaseLayerInterface implementation
class BPLayerInterfaceImpl final : public BroadPhaseLayerInterface
{
public:
	BPLayerInterfaceImpl()
	{
		// Create a mapping table from object to broad phase layer
		//mObjectToBroadPhase[Layers::UNUSED1] = BroadPhaseLayers::UNUSED;
		//mObjectToBroadPhase[Layers::UNUSED2] = BroadPhaseLayers::UNUSED;
		//mObjectToBroadPhase[Layers::UNUSED3] = BroadPhaseLayers::UNUSED;
		//mObjectToBroadPhase[Layers::UNUSED4] = BroadPhaseLayers::UNUSED;
		mObjectToBroadPhase[Layers::NON_MOVING] = BroadPhaseLayers::NON_MOVING;
		mObjectToBroadPhase[Layers::MOVING] = BroadPhaseLayers::MOVING;
		mObjectToBroadPhase[Layers::CHARACTER] = BroadPhaseLayers::CHARACTER;//mObjectToBroadPhase[Layers::DEBRIS] = BroadPhaseLayers::DEBRIS;
		//mObjectToBroadPhase[Layers::SENSOR] = BroadPhaseLayers::SENSOR;
	}

	virtual uint GetNumBroadPhaseLayers() const override
	{
		return BroadPhaseLayers::NUM_LAYERS;
	}

	virtual BroadPhaseLayer GetBroadPhaseLayer(ObjectLayer inLayer) const override
	{
		JPH_ASSERT(inLayer < Layers::NUM_LAYERS);
		return mObjectToBroadPhase[inLayer];
	}

#if defined(JPH_EXTERNAL_PROFILE) || defined(JPH_PROFILE_ENABLED)
	virtual const char* GetBroadPhaseLayerName(BroadPhaseLayer inLayer) const override
	{
		switch ((BroadPhaseLayer::Type)inLayer)
		{
		case (BroadPhaseLayer::Type)BroadPhaseLayers::NON_MOVING:	return "NON_MOVING";
		case (BroadPhaseLayer::Type)BroadPhaseLayers::MOVING:		return "MOVING";
		case (BroadPhaseLayer::Type)BroadPhaseLayers::DEBRIS:		return "DEBRIS";
		case (BroadPhaseLayer::Type)BroadPhaseLayers::SENSOR:		return "SENSOR";
		case (BroadPhaseLayer::Type)BroadPhaseLayers::UNUSED:		return "UNUSED";
		default:													JPH_ASSERT(false); return "INVALID";
		}
	}
#endif // JPH_EXTERNAL_PROFILE || JPH_PROFILE_ENABLED

private:
	BroadPhaseLayer mObjectToBroadPhase[Layers::NUM_LAYERS];
};

/// Class that determines if an object layer can collide with a broadphase layer
class ObjectVsBroadPhaseLayerFilterImpl : public ObjectVsBroadPhaseLayerFilter
{
public:
	virtual bool ShouldCollide(ObjectLayer inLayer1, BroadPhaseLayer inLayer2) const override
	{
		switch (inLayer1)
		{
		case Layers::NON_MOVING:
			return inLayer2 == BroadPhaseLayers::MOVING;
		case Layers::MOVING:
			return inLayer2 == BroadPhaseLayers::NON_MOVING || inLayer2 == BroadPhaseLayers::MOVING;// || inLayer2 == BroadPhaseLayers::SENSOR;
		case Layers::CHARACTER://DEBRIS:
			return false;//return inLayer2 == BroadPhaseLayers::NON_MOVING;
		case Layers::MOVING_AND_CHARACTER:
			return inLayer2 == BroadPhaseLayers::NON_MOVING || inLayer2 == BroadPhaseLayers::MOVING || inLayer2 == BroadPhaseLayers::CHARACTER;// || inLayer2 == BroadPhaseLayers::SENSOR;
		//case Layers::SENSOR:
		//	return inLayer2 == BroadPhaseLayers::MOVING;
		//case Layers::UNUSED1:
		//case Layers::UNUSED2:
		//case Layers::UNUSED3:
		//	return false;
		default:
			JPH_ASSERT(false);
			return false;
		}
	}
};

class MyContactListener : public ContactListener
{
public:
	PhysicsSystemItem* system;
	std::mutex addMutex;

	//

	virtual ValidateResult OnContactValidate(const Body& inBody1, const Body& inBody2, RVec3Arg inBaseOffset, const CollideShapeResult& inCollisionResult) override;

	void AddContact(const Body& inBody1, const Body& inBody2, const ContactManifold& inManifold, ContactSettings& ioSettings);

	virtual void OnContactAdded(const Body& inBody1, const Body& inBody2, const ContactManifold& inManifold, ContactSettings& ioSettings) override;
	virtual void OnContactPersisted(const Body& inBody1, const Body& inBody2, const ContactManifold& inManifold, ContactSettings& ioSettings) override;
	virtual void OnContactRemoved(const SubShapeIDPair& inSubShapePair) override;
};

class MyBodyActivationListener : public BodyActivationListener
{
public:
	PhysicsSystemItem* system;

	virtual void OnBodyActivated(const BodyID& inBodyID, std::uint64_t inBodyUserData) override;
	virtual void OnBodyDeactivated(const BodyID& inBodyID, std::uint64_t inBodyUserData) override;
};

class PhysicsStepListenerForNewStepListeners : public PhysicsStepListener
{
public:
	PhysicsSystemItem* system;

	virtual void OnStep(float inDeltaTime, PhysicsSystem& inPhysicsSystem) override;
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct RayTestResultNative
{
	uint bodyId;
	int shapeIndex;
	Vector3D position;
	Vector3 normal;
	float distanceScale;

	//!!!!
	int triangleIndexSource;
	int triangleIndexProcessed;
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct VolumeTestResultNative
{
	uint bodyId;
	float distanceScale;
	int/*bool*/ backFaceHit;
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#pragma pack(push, 1)
struct ContactItem
{
	uint body2ID;
	Vector3 normal;
	float penetrationDepth;
	uint subShapeID1;
	uint subShapeID2;
	uint contactPointCount;
	Vector3D contactPointsOn1[4];
	Vector3D contactPointsOn2[4];

	//uint body1ID;
	//Vector3D baseOffset;
	//Vector3 relativeContactPointsOn1[4];
	//Vector3 relativeContactPointsOn2[4];

	//Vector3D worldPositionOn1;
	//Vector3D worldPositionOn2;
	//uint body2Id;
	////BodyItem* body2;
};
#pragma pack(pop)

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#pragma pack(push, 1)
struct VehicleWheelSettings
{
	Vector3 position;
	Vector3 direction;
	float suspensionMinLength;// = 0.3f;
	float suspensionMaxLength;// = 0.5f;
	float suspensionPreloadLength;// = 0.0f;
	float suspensionFrequency;// = 1.5f;
	float suspensionDamping;// = 0.5f;
	float radius;
	float width;

	//Wheeled specific
	float inertia;// = 0.9f;
	float angularDamping;// = 0.2f;
	float maxSteerAngle;// = DegreesToRadians(70.0f);
	int LongitudinalFrictionCount;
	float* LongitudinalFrictionData;
	int LateralFrictionCount;
	float* LateralFrictionData;
	float maxBrakeTorque;// = 1500.0f;
	float maxHandBrakeTorque;// = 4000.0f;

	//Tracked specific
	float trackLongitudinalFriction;
	float trackLateralFriction;
};
#pragma pack(pop)

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct VehicleWheelData
{
	float Position;//float SuspensionLength;//ContactLength
	float SteerAngle;
	float RotationAngle;
	float AngularVelocity;
	uint ContactBody;

	//Vector3 position;
	//Vector4 rotation;
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class PhysicsSystemItem
{
public:
	PhysicsSystem system;
	BPLayerInterfaceImpl broadPhaseLayerInterface;
	MyBodyActivationListener body_activation_listener;
	MyContactListener contactListener;
	PhysicsStepListenerForNewStepListeners physicsStepListenerForNewStepListeners;

	ObjectVsBroadPhaseLayerFilterImpl objectVsBroadPhaseLayerFilterImpl;
	ObjectLayerPairFilterImpl objectLayerPairFilter;

	//need?
	std::map<uint, BodyItem*> bodyById;
	std::map<uint, BodyItem*> bodyCharacterModeById;

	std::set<BodyItem*> bodiesContactsToClear;

	std::mutex vehiclesToActivateMutex;
	std::set<VehicleConstraintItem*> vehiclesToActivate;

	//DebugRendererImpl debugRenderer;
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class BodyItem
{
public:
	PhysicsSystem* system;
	PhysicsSystemItem* systemItem;
	Body* body;
	BodyID id;

	bool subscribedToGetContacts = false;
	std::vector<ContactItem> contacts;

	std::vector<ConstraintItem*> constraints;
	bool constraintsVehicleAttached = false;

	//character mode
	Ref<CharacterVirtual> character;
	CharacterVirtual::ExtendedUpdateSettings* characterUpdateSettings = nullptr;
	float characterWalkUpDownLastChange = 0;
	Vector2 characterDesiredVelocity = Vector2::ZERO;
	//Vector2 characterLastEffenciveVelocity = Vector2::ZERO;
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class ShapeItem
{
public:
	CompoundShapeSettings* compoundShapeSettings;
	OffsetCenterOfMassShapeSettings* offsetCenterOfMassShapeSettings;
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class ConstraintItem
{
public:
	//ConstraintTypeEnum type;
	PhysicsSystem* system;
	Ref<Constraint> constraint;

	bool collisionsBetweenLinkedBodies = true;

	virtual void OnDestroy() {}
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class TwoBodyConstraintItem : public ConstraintItem
{
public:
	BodyItem* bodyA = nullptr;
	BodyItem* bodyB = nullptr;

	virtual void OnDestroy() override
	{
		{
			auto& constraintsA = bodyA->constraints;
			std::vector<ConstraintItem*>::iterator position = std::find(constraintsA.begin(), constraintsA.end(), this);
			if (position != constraintsA.end())
				constraintsA.erase(position);
		}

		{
			auto& constraintsB = bodyB->constraints;
			std::vector<ConstraintItem*>::iterator position = std::find(constraintsB.begin(), constraintsB.end(), this);
			if (position != constraintsB.end())
				constraintsB.erase(position);
		}

		ConstraintItem::OnDestroy();
	}
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class SixDOFConstraintItem : public TwoBodyConstraintItem
{
public:
	float angularXVelocity = 0;
	float angularYVelocity = 0;
	float angularZVelocity = 0;

	float angularXPosition = 0;
	float angularYPosition = 0;
	float angularZPosition = 0;
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class VehicleConstraintItem : public ConstraintItem
{
public:
	BodyItem* body = nullptr;
	bool stepListenerAdded = false;
	//bool stepListenerMustBeAdded = false;
	Ref<VehicleCollisionTester> collisionTester;
	bool Wheels;
	bool Tracks;

	//

	virtual void OnDestroy() override
	{
		//!!!!no leaks?

		{
			auto& constraints = body->constraints;
			std::vector<ConstraintItem*>::iterator position = std::find(constraints.begin(), constraints.end(), this);
			if (position != constraints.end())
				constraints.erase(position);
		}

		if (stepListenerAdded)
		{
			VehicleConstraint* c = (VehicleConstraint*)constraint.GetPtr();
			system->RemoveStepListener(c);
			stepListenerAdded = false;
		}

		ConstraintItem::OnDestroy();
	}
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

enum class PhysicsAxisMode
{
	Locked,
	Limited,
	Free,
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct DebugDrawLineItem
{
	Vector3D From;
	Vector3D To;
	Color _Color;
	//int IsArrow;
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class DebugRendererImpl : public DebugRenderer
{
public:
	std::vector<DebugDrawLineItem> lines;

	//!!!!need?
	//Mutex mMutex;

	/// Next available ID
	uint32 mNextBatchID = 1;
	uint32 mNextGeometryID = 1;

	//

	////////////////////////////////////////////////

	//!!!!need?
	/// Implementation specific batch object
	class BatchImpl : public RefTargetVirtual
	{
	public:
		//!!!!linux: JPH_OVERRIDE_NEW_DELETE

		BatchImpl(uint32 inID) : mID(inID) {  }

		virtual void					AddRef() override { ++mRefCount; }
		virtual void					Release() override { if (--mRefCount == 0) delete this; }

		atomic<uint32>					mRefCount = 0;
		uint32							mID;
	};

	////////////////////////////////////////////////

	DebugRendererImpl()
	{
		lines.reserve(32);
	}

	virtual void DrawLine(RVec3Arg inFrom, RVec3Arg inTo, ColorArg inColor) override
	{
		//lock_guard lock(mMutex);

		DebugDrawLineItem item;
		item.From = ConvertToVector3D(inFrom);
		item.To = ConvertToVector3D(inTo);
		item._Color = inColor;
		//item.IsArrow = false;
		lines.push_back(item);
	}

	virtual void DrawTriangle(RVec3Arg inV1, RVec3Arg inV2, RVec3Arg inV3, ColorArg inColor) override
	{
	}

	virtual Batch CreateTriangleBatch(const Triangle* inTriangles, int inTriangleCount) override
	{
		return new BatchImpl(0);

		//if (inTriangles == nullptr || inTriangleCount == 0)
		//	return new BatchImpl(0);

		//lock_guard lock(mMutex);

		////mStream.Write(ECommand::CreateBatch);

		//uint32 batch_id = mNextBatchID++;
		////JPH_ASSERT(batch_id != 0);
		////mStream.Write(batch_id);
		////mStream.Write((uint32)inTriangleCount);
		////mStream.WriteBytes(inTriangles, inTriangleCount * sizeof(Triangle));

		//return new BatchImpl(batch_id);
	}

	virtual Batch CreateTriangleBatch(const Vertex* inVertices, int inVertexCount, const uint32* inIndices, int inIndexCount) override
	{
		return new BatchImpl(0);

		//if (inVertices == nullptr || inVertexCount == 0 || inIndices == nullptr || inIndexCount == 0)
		//	return new BatchImpl(0);

		//lock_guard lock(mMutex);

		////mStream.Write(ECommand::CreateBatchIndexed);

		//uint32 batch_id = mNextBatchID++;
		////JPH_ASSERT(batch_id != 0);
		////mStream.Write(batch_id);
		////mStream.Write((uint32)inVertexCount);
		////mStream.WriteBytes(inVertices, inVertexCount * sizeof(Vertex));
		////mStream.Write((uint32)inIndexCount);
		////mStream.WriteBytes(inIndices, inIndexCount * sizeof(uint32));

		//return new BatchImpl(batch_id);
	}

	virtual void DrawGeometry(RMat44Arg inModelMatrix, const AABox& inWorldSpaceBounds, float inLODScaleSq, ColorArg inModelColor, const GeometryRef& inGeometry, ECullMode inCullMode = ECullMode::CullBackFace, ECastShadow inCastShadow = ECastShadow::On, EDrawMode inDrawMode = EDrawMode::Solid) override
	{
	}

	virtual void DrawText3D(RVec3Arg inPosition, const string_view& inString, ColorArg inColor = Color::sWhite, float inHeight = 0.5f) override
	{
	}
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

///// Collision tester that tests collision using a sphere cast
//class VehicleCollisionTesterCastSphereMultiRadius : public VehicleCollisionTester
//{
//public:
//	JPH_OVERRIDE_NEW_DELETE
//
//		/// Constructor
//		/// @param inObjectLayer Object layer to test collision with
//		/// @param inUp World space up vector, used to avoid colliding with vertical walls.
//		/// @param inRadius Radius of sphere
//		/// @param inMaxSlopeAngle Max angle (rad) that is considered for colliding wheels. This is to avoid colliding with vertical walls.
//		VehicleCollisionTesterCastSphereMultiRadius(ObjectLayer inObjectLayer, /*float inRadius, */Vec3Arg inUp = Vec3::sAxisY(), float inMaxSlopeAngle = DegreesToRadians(80.0f)) : mObjectLayer(inObjectLayer), /*mRadius(inRadius), */mUp(inUp), mCosMaxSlopeAngle(Cos(inMaxSlopeAngle)) { }
//
//	// See: VehicleCollisionTester
//	virtual bool				Collide(PhysicsSystem& inPhysicsSystem, const VehicleConstraint& inVehicleConstraint, uint inWheelIndex, RVec3Arg inOrigin, Vec3Arg inDirection, const BodyID& inVehicleBodyID, Body*& outBody, SubShapeID& outSubShapeID, RVec3& outContactPosition, Vec3& outContactNormal, float& outSuspensionLength) const override;
//
//public:
//	std::vector<float> mRadiuses;
//
//private:
//	ObjectLayer					mObjectLayer;
//	//float						mRadius;
//	Vec3						mUp;
//	float						mCosMaxSlopeAngle;
//};
//
//bool VehicleCollisionTesterCastSphereMultiRadius::Collide(PhysicsSystem& inPhysicsSystem, const VehicleConstraint& inVehicleConstraint, uint inWheelIndex, RVec3Arg inOrigin, Vec3Arg inDirection, const BodyID& inVehicleBodyID, Body*& outBody, SubShapeID& outSubShapeID, RVec3& outContactPosition, Vec3& outContactNormal, float& outSuspensionLength) const
//{
//	DefaultBroadPhaseLayerFilter broadphase_layer_filter = inPhysicsSystem.GetDefaultBroadPhaseLayerFilter(mObjectLayer);
//	DefaultObjectLayerFilter object_layer_filter = inPhysicsSystem.GetDefaultLayerFilter(mObjectLayer);
//	IgnoreSingleBodyFilter body_filter(inVehicleBodyID);
//
//	auto mRadius = mRadiuses[inWheelIndex];
//
//	SphereShape sphere(mRadius);
//	sphere.SetEmbedded();
//
//	float cast_length = max(0.0f, inSuspensionMaxLength - mRadius);
//	RShapeCast shape_cast(&sphere, Vec3::sReplicate(1.0f), RMat44::sTranslation(inOrigin), inDirection * cast_length);
//
//	ShapeCastSettings settings;
//	settings.mUseShrunkenShapeAndConvexRadius = true;
//	settings.mReturnDeepestPoint = true;
//
//	class MyCollector : public CastShapeCollector
//	{
//	public:
//		MyCollector(PhysicsSystem& inPhysicsSystem, const RShapeCast& inShapeCast, Vec3Arg inUpDirection, float inCosMaxSlopeAngle) :
//			mPhysicsSystem(inPhysicsSystem),
//			mShapeCast(inShapeCast),
//			mUpDirection(inUpDirection),
//			mCosMaxSlopeAngle(inCosMaxSlopeAngle)
//		{
//		}
//
//		virtual void		AddHit(const ShapeCastResult& inResult) override
//		{
//			// Test if this collision is closer than the previous one
//			if (inResult.mFraction < GetEarlyOutFraction())
//			{
//				// Lock the body
//				BodyLockRead lock(mPhysicsSystem.GetBodyLockInterfaceNoLock(), inResult.mBodyID2);
//				JPH_ASSERT(lock.Succeeded()); // When this runs all bodies are locked so this should not fail
//				const Body* body = &lock.GetBody();
//
//				if (body->IsSensor())
//					return;
//
//				// Test that we're not hitting a vertical wall
//				Vec3 normal = -inResult.mPenetrationAxis.Normalized();
//				if (normal.Dot(mUpDirection) > mCosMaxSlopeAngle)
//				{
//					// Update early out fraction to this hit
//					UpdateEarlyOutFraction(inResult.mFraction);
//
//					// Get the contact properties
//					mBody = body;
//					mSubShapeID2 = inResult.mSubShapeID2;
//					mContactPosition = mShapeCast.mCenterOfMassStart.GetTranslation() + inResult.mContactPointOn2;
//					mContactNormal = normal;
//				}
//			}
//		}
//
//		// Configuration
//		PhysicsSystem& mPhysicsSystem;
//		const RShapeCast& mShapeCast;
//		Vec3				mUpDirection;
//		float				mCosMaxSlopeAngle;
//
//		// Resulting closest collision
//		const Body* mBody = nullptr;
//		SubShapeID			mSubShapeID2;
//		RVec3				mContactPosition;
//		Vec3				mContactNormal;
//	};
//
//	MyCollector collector(inPhysicsSystem, shape_cast, mUp, mCosMaxSlopeAngle);
//	inPhysicsSystem.GetNarrowPhaseQueryNoLock().CastShape(shape_cast, settings, shape_cast.mCenterOfMassStart.GetTranslation(), collector, broadphase_layer_filter, object_layer_filter, body_filter);
//	if (collector.mBody == nullptr)
//		return false;
//
//	outBody = const_cast<Body*>(collector.mBody);
//	outSubShapeID = collector.mSubShapeID2;
//	outContactPosition = collector.mContactPosition;
//	outContactNormal = collector.mContactNormal;
//	outSuspensionLength = min(inSuspensionMaxLength, cast_length * collector.GetEarlyOutFraction() + mRadius);
//
//	return true;
//}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifdef JPH_ENABLE_ASSERTS

// Callback for asserts, connect this to your own assert handler if you have one
static bool AssertFailedImpl(const char* inExpression, const char* inMessage, const char* inFile, uint inLine)
{
	string s = "JOLT PHYSICS ASSERT!\n\n" + string(inFile) + ":" + to_string(inLine) + ": (" + string(inExpression) + ") " + string(inMessage != nullptr ? inMessage : "");
	Fatal(s.c_str());

	// Print to the TTY
	//cout << inFile << ":" << inLine << ": (" << inExpression << ") " << (inMessage != nullptr ? inMessage : "") << endl;

	// Breakpoint
	return true;
};

#endif // JPH_ENABLE_ASSERTS


bool staticInitialized = false;
JobSystemThreadPool job_system;
TempAllocatorMalloc tempAllocator;

EXPORT PhysicsSystemItem* JCreateSystem(int maxBodies, int maxBodyPairs, int maxContactConstraints)
{
	if (sizeof(RayTestResultNative) != 56)
		Fatal("JCreateSystem: sizeof(RayTestResultNative) != 56.");
	if (sizeof(VolumeTestResultNative) != 12)
		Fatal("JCreateSystem: sizeof(VolumeTestResultNative) != 12.");
	if (sizeof(void*) == 8)
	{
		if (sizeof(VehicleWheelSettings) != 24 * 4 + 8)
			Fatal("JCreateSystem: sizeof(VehicleWheelSettings) != 24 * 4 + 8.");
	}
	else
	{
		if (sizeof(VehicleWheelSettings) != 24 * 4)
			Fatal("JCreateSystem: sizeof(VehicleWheelSettings) != 24 * 4.");
	}
	//if (sizeof(void*) == 8)
	//{
	//	if (sizeof(VehicleWheelSettings) != 22 * 4 + 8)
	//		Fatal("JCreateSystem: sizeof(VehicleWheelSettings) != 22 * 4 + 8.");
	//}
	//else
	//{
	//	if (sizeof(VehicleWheelSettings) != 22 * 4)
	//		Fatal("JCreateSystem: sizeof(VehicleWheelSettings) != 22 * 4.");
	//}
	if (sizeof(VehicleWheelData) != 5 * 4)
		Fatal("JCreateSystem: sizeof(VehicleWheelSettings) != 5 * 4.");
	if (sizeof(ContactItem) != 224)
		Fatal("JCreateSystem: sizeof(ContactItem) != 224.");

	if (maxBodies < 4)
		maxBodies = 4;

	if (!staticInitialized)
	{
		staticInitialized = true;

		// Register allocation hook
		Allocate = MemoryAlloc;
		Free = MemoryFree;
		AlignedAllocate = MemoryAlignedAlloc;
		AlignedFree = MemoryAlignedFree;
		//RegisterDefaultAllocator();

		// Install callbacks
		//Trace = TraceImpl;
		JPH_IF_ENABLE_ASSERTS(AssertFailed = AssertFailedImpl;)

		// Create a factory
		JPH::Factory::sInstance = new JPH::Factory();

		// Register all Jolt physics types
		RegisterTypes();

		job_system.Init(cMaxPhysicsJobs, cMaxPhysicsBarriers, thread::hardware_concurrency() - 1);
		//single threaded:
		//job_system.Init(cMaxPhysicsJobs, cMaxPhysicsBarriers, 0);
	}


	PhysicsSystemItem* system = new PhysicsSystemItem();

	//auto maxBodyPairs = maxBodies;// 65536;
	//auto maxContactConstraints = 1024 * 10;
	//auto maxContactConstraints = 1024;

	//!!!!maxActiveBodies может быть. общее количество тел и сколько активных

	system->system.Init(maxBodies, 0, maxBodyPairs, maxContactConstraints, system->broadPhaseLayerInterface, system->objectVsBroadPhaseLayerFilterImpl, system->objectLayerPairFilter);

	//system->system.Init(maxBodies, 0, maxBodyPairs, maxContactConstraints, system->broadPhaseLayerInterface, MyBroadPhaseCanCollide, MyObjectCanCollide);

	// Note that this is called from a job so whatever you do here needs to be thread safe.
	system->system.SetBodyActivationListener(&system->body_activation_listener);
	system->body_activation_listener.system = system;

	// Note that this is called from a job so whatever you do here needs to be thread safe.
	system->system.SetContactListener(&system->contactListener);
	system->contactListener.system = system;

	system->system.AddStepListener(&system->physicsStepListenerForNewStepListeners);
	system->physicsStepListenerForNewStepListeners.system = system;

	return system;
}

EXPORT void JDestroySystem(PhysicsSystemItem* system)
{
	//!!!!no leaks?

	system->system.RemoveStepListener(&system->physicsStepListenerForNewStepListeners);

	delete system;
}

EXPORT void JDestroy()
{
	job_system.SetNumThreads(0);
}

//EXPORT void JPhysicsSystem_SetPhysicsSettings(PhysicsSystemItem* system, /*bool useDefault,*/
//	int maxInFlightBodyPairs,
//	int stepListenersBatchSize,
//	int stepListenerBatchesPerJob,
//	float baumgarte,
//	float speculativeContactDistance,
//	float penetrationSlop,
//	float linearCastThreshold,
//	float linearCastMaxPenetration,
//	float manifoldToleranceSq,
//	float maxPenetrationDistance,
//	float bodyPairCacheMaxDeltaPositionSq,
//	float bodyPairCacheCosMaxDeltaRotationDiv2,
//	float contactNormalCosMaxDeltaRotation,
//	float contactPointPreserveLambdaMaxDistSq,
//	int numVelocitySteps,
//	int numPositionSteps,
//	float minVelocityForRestitution,
//	float timeBeforeSleep,
//	float pointVelocitySleepThreshold,
//	bool constraintWarmStart,
//	bool useBodyPairContactCache,
//	bool useManifoldReduction,
//	bool allowSleeping,
//	bool checkActiveEdges)
//{
//
//	PhysicsSettings settings;
//
//	//if (!useDefault)
//	//{
//
//	settings.mMaxInFlightBodyPairs = maxInFlightBodyPairs;
//	settings.mStepListenersBatchSize = stepListenersBatchSize;
//	settings.mStepListenerBatchesPerJob = stepListenerBatchesPerJob;
//	settings.mBaumgarte = baumgarte;
//	settings.mSpeculativeContactDistance = speculativeContactDistance;
//	settings.mPenetrationSlop = penetrationSlop;
//	settings.mLinearCastThreshold = linearCastThreshold;
//	settings.mLinearCastMaxPenetration = linearCastMaxPenetration;
//	settings.mManifoldToleranceSq = manifoldToleranceSq;
//	settings.mMaxPenetrationDistance = maxPenetrationDistance;
//	settings.mBodyPairCacheMaxDeltaPositionSq = bodyPairCacheMaxDeltaPositionSq;
//	settings.mBodyPairCacheCosMaxDeltaRotationDiv2 = bodyPairCacheCosMaxDeltaRotationDiv2;
//	settings.mContactNormalCosMaxDeltaRotation = contactNormalCosMaxDeltaRotation;
//	settings.mContactPointPreserveLambdaMaxDistSq = contactPointPreserveLambdaMaxDistSq;
//	settings.mNumVelocitySteps = numVelocitySteps;
//	settings.mNumPositionSteps = numPositionSteps;
//	settings.mMinVelocityForRestitution = minVelocityForRestitution;
//	settings.mTimeBeforeSleep = timeBeforeSleep;
//	settings.mPointVelocitySleepThreshold = pointVelocitySleepThreshold;
//	settings.mConstraintWarmStart = constraintWarmStart;
//	settings.mUseBodyPairContactCache = useBodyPairContactCache;
//	settings.mUseManifoldReduction = useManifoldReduction;
//	settings.mAllowSleeping = allowSleeping;
//	settings.mCheckActiveEdges = checkActiveEdges;
//
//	//settings.mBaumgarte = 1;
//	//settings.mNumVelocitySteps = 30;
//	//settings.mNumPositionSteps = 10;
//	//settings.mLinearCastThreshold = 0.05;
//	//settings.mLinearCastMaxPenetration = 0.05;
//	//settings.mMaxPenetrationDistance = 0.01;
//
//
//	//}
//
//	system->system.SetPhysicsSettings(settings);
//}

#ifdef _WIN32
#pragma pack(push, 1)
#else
#pragma pack(1)
#endif // _WIN32

struct PhysicsSettings2
{
	/// Size of body pairs array, corresponds to the maximum amount of potential body pairs that can be in flight at any time.
	/// Setting this to a low value will use less memory but slow down simulation as threads may run out of narrow phase work.
	int mMaxInFlightBodyPairs;

	/// How many PhysicsStepListeners to notify in 1 batch
	int mStepListenersBatchSize;

	/// How many step listener batches are needed before spawning another job (set to INT_MAX if no parallelism is desired)
	int mStepListenerBatchesPerJob;

	/// Baumgarte stabilization factor (how much of the position error to 'fix' in 1 update) (unit: dimensionless, 0 = nothing, 1 = 100%)
	float mBaumgarte;

	/// Radius around objects inside which speculative contact points will be detected. Note that if this is too big 
	/// you will get ghost collisions as speculative contacts are based on the closest points during the collision detection 
	/// step which may not be the actual closest points by the time the two objects hit (unit: meters)
	float mSpeculativeContactDistance;

	/// How much bodies are allowed to sink into eachother (unit: meters)
	float mPenetrationSlop;

	/// Fraction of its inner radius a body must move per step to enable casting for the LinearCast motion quality
	float mLinearCastThreshold;

	/// Fraction of its inner radius a body may penetrate another body for the LinearCast motion quality
	float mLinearCastMaxPenetration;

	/// Max squared distance to use to determine if two points are on the same plane for determining the contact manifold between two shape faces (unit: meter^2)
	float mManifoldToleranceSq;

	/// Maximum distance to correct in a single iteration when solving position constraints (unit: meters)
	float mMaxPenetrationDistance;

	/// Maximum relative delta position for body pairs to be able to reuse collision results from last frame (units: meter^2)
	float mBodyPairCacheMaxDeltaPositionSq; ///< 1 mm


	/// Maximum relative delta orientation for body pairs to be able to reuse collision results from last frame, stored as cos(max angle / 2)
	float mBodyPairCacheCosMaxDeltaRotationDiv2; ///< cos(2 degrees / 2)


	/// Maximum angle between normals that allows manifolds between different sub shapes of the same body pair to be combined
	float mContactNormalCosMaxDeltaRotation; ///< cos(5 degree)


	/// Maximum allowed distance between old and new contact point to preserve contact forces for warm start (units: meter^2)
	float mContactPointPreserveLambdaMaxDistSq; ///< 1 cm


	/// Number of solver velocity iterations to run
	/// Note that this needs to be >= 2 in order for friction to work (friction is applied using the non-penetration impulse from the previous iteration)
	int mNumVelocitySteps;

	/// Number of solver position iterations to run
	int mNumPositionSteps;

	/// Minimal velocity needed before a collision can be elastic (unit: m)
	float mMinVelocityForRestitution;

	/// Time before object is allowed to go to sleep (unit: seconds)
	float mTimeBeforeSleep;

	/// Velocity of points on bounding box of object below which an object can be considered sleeping (unit: m/s)
	float mPointVelocitySleepThreshold;

	/// By default the simulation is deterministic, it is possible to turn this off by setting this setting to false. This will make the simulation run faster but it will no longer be deterministic.
	bool mDeterministicSimulation;

	///@name These variables are mainly for debugging purposes, they allow turning on/off certain subsystems. You probably want to leave them alone.
	///@{

	/// Whether or not to use warm starting for constraints (initially applying previous frames impulses)
	bool mConstraintWarmStart;

	/// Whether or not to use the body pair cache, which removes the need for narrow phase collision detection when orientation between two bodies didn't change
	bool mUseBodyPairContactCache;

	/// Whether or not to reduce manifolds with similar contact normals into one contact manifold
	bool mUseManifoldReduction;

	/// If we split up large islands into smaller parallel batches of work (to improve performance)
	bool mUseLargeIslandSplitter;

	/// If objects can go to sleep or not
	bool mAllowSleeping;

	/// When false, we prevent collision against non-active (shared) edges. Mainly for debugging the algorithm.
	bool mCheckActiveEdges;

	///@}
};

#ifdef _WIN32
#pragma pack(pop)
#else
#pragma pack()
#endif // _WIN32

EXPORT void JPhysicsSystem_SetPhysicsSettingsStructure(PhysicsSystemItem* system, PhysicsSettings2* settings2, int structSizeToCheck)
{
	if (structSizeToCheck != sizeof(PhysicsSettings2))
	{
		char q[200];
		sprintf(q, "JPhysicsSystem_SetPhysicsSettings_All: structSizeToCheck != sizeof(PhysicsSettings2). %d %d", (int)structSizeToCheck, (int)sizeof(PhysicsSettings2));
		Fatal(q);
	}

	PhysicsSettings settings;

	settings.mMaxInFlightBodyPairs = settings2->mMaxInFlightBodyPairs;
	settings.mStepListenersBatchSize = settings2->mStepListenersBatchSize;
	settings.mStepListenerBatchesPerJob = settings2->mStepListenerBatchesPerJob;
	settings.mBaumgarte = settings2->mBaumgarte;
	settings.mSpeculativeContactDistance = settings2->mSpeculativeContactDistance;
	settings.mPenetrationSlop = settings2->mPenetrationSlop;
	settings.mLinearCastThreshold = settings2->mLinearCastThreshold;
	settings.mLinearCastMaxPenetration = settings2->mLinearCastMaxPenetration;
	settings.mManifoldToleranceSq = settings2->mManifoldToleranceSq;
	settings.mMaxPenetrationDistance = settings2->mMaxPenetrationDistance;
	settings.mBodyPairCacheMaxDeltaPositionSq = settings2->mBodyPairCacheMaxDeltaPositionSq;
	settings.mBodyPairCacheCosMaxDeltaRotationDiv2 = settings2->mBodyPairCacheCosMaxDeltaRotationDiv2;
	settings.mContactNormalCosMaxDeltaRotation = settings2->mContactNormalCosMaxDeltaRotation;
	settings.mContactPointPreserveLambdaMaxDistSq = settings2->mContactPointPreserveLambdaMaxDistSq;
	settings.mNumVelocitySteps = settings2->mNumVelocitySteps;
	settings.mNumPositionSteps = settings2->mNumPositionSteps;
	settings.mMinVelocityForRestitution = settings2->mMinVelocityForRestitution;
	settings.mTimeBeforeSleep = settings2->mTimeBeforeSleep;
	settings.mPointVelocitySleepThreshold = settings2->mPointVelocitySleepThreshold;
	settings.mDeterministicSimulation = settings2->mDeterministicSimulation;
	settings.mConstraintWarmStart = settings2->mConstraintWarmStart;
	settings.mUseBodyPairContactCache = settings2->mUseBodyPairContactCache;
	settings.mUseManifoldReduction = settings2->mUseManifoldReduction;
	settings.mUseLargeIslandSplitter = settings2->mUseLargeIslandSplitter;
	settings.mAllowSleeping = settings2->mAllowSleeping;
	settings.mCheckActiveEdges = settings2->mCheckActiveEdges;

	system->system.SetPhysicsSettings(settings);
}

EXPORT void JPhysicsSystem_SetGravity(PhysicsSystemItem* system, const Vector3& value)
{
	system->system.SetGravity(ConvertToVec3(value));
}

EXPORT void JPhysicsSystem_OptimizeBroadPhase(PhysicsSystemItem* system)
{
	system->system.OptimizeBroadPhase();
}

EXPORT void JPhysicsSystem_Update(PhysicsSystemItem* system, float deltaTime, int collisionSteps/*, int integrationSubSteps*/, bool debug)
{
	//if (integrationSubSteps > PhysicsUpdateContext::cMaxSubSteps)
	//	integrationSubSteps = PhysicsUpdateContext::cMaxSubSteps;

	//clear last contacts
	{
		for (auto bodyItem : system->bodiesContactsToClear)
			bodyItem->contacts.clear();
		system->bodiesContactsToClear.clear();
	}

	//add step listeners to active bodies
	{
		auto activeBodies = system->system.GetActiveBodiesUnsafe();
		auto count = system->system.GetNumActiveBodies();
		for (int n = 0; n < count; n++)
		{
			BodyItem* body = (BodyItem*)system->system.GetBody(activeBodies[n]).GetUserData();

			//update vehicle constraint of the body
			if (body != nullptr && body->constraintsVehicleAttached)
			{
				for (int n2 = 0; n2 < body->constraints.size(); n2++)
				{
					ConstraintItem* constraint = body->constraints[n2];

					if (constraint->constraint->GetType() == EConstraintType::Vehicle)
					{
						VehicleConstraintItem* vehicleConstraint = (VehicleConstraintItem*)constraint;

						if (!vehicleConstraint->stepListenerAdded)
						{
							VehicleConstraint* c2 = (VehicleConstraint*)vehicleConstraint->constraint.GetPtr();
							if (c2->GetVehicleBody()->IsActive() || c2->IsActive())
							{
								system->system.AddStepListener(c2);
								vehicleConstraint->stepListenerAdded = true;
							}
						}
					}
				}
			}
		}
	}

	//auto& constraints = system->system.GetConstraintsNoLock();
	//for (int n = 0; n < constraints.size(); n++)
	//{
	//	Constraint* c = constraints[n];
	//	if (c->GetType() == EConstraintType::Vehicle)
	//	{
	//		VehicleConstraint* c2 = (VehicleConstraint*)c;
	//		VehicleConstraintItem* item = (VehicleConstraintItem*)c2->GetUserData();

	//		if (c2->GetVehicleBody()->IsActive() || c2->IsActive())
	//		{
	//			if (!item->stepListenerAdded)
	//			{
	//				system->system.AddStepListener(c2);
	//				item->stepListenerAdded = true;
	//			}
	//		}
	//		else
	//		{
	//			if (item->stepListenerAdded)
	//			{
	//				system->system.RemoveStepListener(c2);
	//				item->stepListenerAdded = false;
	//			}
	//		}

	//		//if (c2->GetVehicleBody()->IsActive() || item->stepListenerMustBeAdded)
	//		////if (item->stepListenerMustBeAdded)
	//		//{
	//		//	if (!item->stepListenerAdded)
	//		//	{
	//		//		system->system.AddStepListener(c2);
	//		//		item->stepListenerAdded = true;
	//		//	}
	//		//}
	//		//else
	//		//{
	//		//	if (item->stepListenerAdded)
	//		//	{
	//		//		system->system.RemoveStepListener(c2);
	//		//		item->stepListenerAdded = false;
	//		//	}
	//		//}
	//	}
	//}

	auto gravity = system->system.GetGravity();
	
	for (const auto& pair : system->bodyCharacterModeById) //for each (auto pair in system->bodyCharacterModeById)
	{
		auto body = pair.second;

		auto character = body->character.GetPtr();
		if (character != nullptr)
		{
			//!!!!когда не обновлять?
			//!!!!slowly?

			Vec3 groundVelocity = character->GetGroundVelocity();
			Vec3 currentVelocity = character->GetLinearVelocity();

			bool inactive = character->GetGroundState() == CharacterVirtual::EGroundState::OnGround && groundVelocity == Vec3(0, 0, 0) && body->characterDesiredVelocity == Vector2::ZERO && currentVelocity.IsNearZero(0.3f * 0.3f);

			//!!!!

			//if ( character->GetGroundState() == CharacterVirtual::EGroundState::OnGround && groundVelocity == Vec3(0, 0, 0) && body->characterDesiredVelocity == Vector2::ZERO && character->GetLinearVelocity().IsNearZero(0.3f * 0.3f))
			//{
			//	//skipUpdate = true;

			//	body->characterWalkUpDownLastChange = 0;

			//	character->SetLinearVelocity(Vec3(0, 0, 0));
			//}
			//else
			//{ 

			//bool enableCharacterInertia = true;


			//!!!!
			//bool player_controls_horizontal_velocity = sControlMovementDuringJump || mCharacter->IsSupported();
			//if (player_controls_horizontal_velocity)
			//{
			//	// Smooth the player input
			//	mDesiredVelocity = sEnableCharacterInertia ? 0.25f * inMovementDirection * sCharacterSpeed + 0.75f * mDesiredVelocity : inMovementDirection * sCharacterSpeed;

			//	// True if the player intended to move
			//	mAllowSliding = !inMovementDirection.IsNearZero();
			//}
			//else
			//{
			//	// While in air we allow sliding
			//	mAllowSliding = true;
			//}

			Vec3 newVelocity;

			if (!inactive)
			{
				//!!!!0.1f?
				//!!!!abs?
				bool movingTowardsGround = (currentVelocity.GetZ() - groundVelocity.GetZ()) < 0.1f;

				if (character->GetGroundState() == CharacterVirtual::EGroundState::OnGround && movingTowardsGround )
				//if (character->GetGroundState() == CharacterVirtual::EGroundState::OnGround &&
				//	(enableCharacterInertia ? movingTowardsGround : !character->IsSlopeTooSteep(character->GetGroundNormal())))
				{
					newVelocity = groundVelocity;

					//// Jump
					//if (inJump && moving_towards_ground)
					//	new_velocity += sJumpSpeed * mCharacter->GetUp();
				}
				else
					newVelocity = Vec3(0, 0, currentVelocity.GetZ());

				newVelocity += gravity * deltaTime;

				//!!!!
				//!!!!может когда OnGround?
				bool playerControlsHorizontalVelocity = /*sControlMovementDuringJump ||*/ character->IsSupported();

				auto relativeVelocity2 = Vector2(currentVelocity.GetX() - groundVelocity.GetX(), currentVelocity.GetY() - groundVelocity.GetY());

				if (playerControlsHorizontalVelocity)
				{
					//auto relativeVelocity = currentVelocity - groundVelocity;
					//auto relativeVelocity2 = Vector2(relativeVelocity.GetX(), relativeVelocity.GetY());
					////auto relativeVelocity2 = body->characterLastEffenciveVelocity - Vector2(groundVelocity.GetX(), groundVelocity.GetY());

					auto v = relativeVelocity2 * 0.75f + body->characterDesiredVelocity * 0.25f;
					newVelocity += Vec3(v.x, v.y, 0);

					//without inertia
					//newVelocity += Vec3(body->characterDesiredVelocity.x, body->characterDesiredVelocity.y, 0);
				}
				else
				{
					//auto relativeVelocity = currentVelocity - groundVelocity;
					//auto relativeVelocity2 = Vector2(relativeVelocity.GetX(), relativeVelocity.GetY());

					//float factorX = 0.01f;
					//if (body->characterDesiredVelocity.x > 0 && body->characterDesiredVelocity.x < relativeVelocity2.x)
					//	factorX = 0.5f;
					//if (body->characterDesiredVelocity.x < 0 && body->characterDesiredVelocity.x > relativeVelocity2.x)
					//	factorX = 0.5f;

					//float factorY = 0.01f;
					//if (body->characterDesiredVelocity.y > 0 && body->characterDesiredVelocity.y < relativeVelocity2.y)
					//	factorY = 0.5f;
					//if (body->characterDesiredVelocity.y < 0 && body->characterDesiredVelocity.y > relativeVelocity2.y)
					//	factorY = 0.5f;

					//float vx = relativeVelocity2.x * (1.0f - factorX) + body->characterDesiredVelocity.x * factorX;
					//float vy = relativeVelocity2.y * (1.0f - factorY) + body->characterDesiredVelocity.y * factorY;
					//newVelocity += Vec3(vx, vy, 0);

					auto v = relativeVelocity2 * 0.99f + body->characterDesiredVelocity * 0.01f;
					newVelocity += Vec3(v.x, v.y, 0);


					//auto v = relativeVelocity2 * 0.98f + body->characterDesiredVelocity * 0.02f;

					//auto newVelocityPotential = newVelocity + Vec3(v.x, v.y, 0);
					//if (body->characterDesiredVelocity.x > 0 && newVelocityPotential.GetX() < body->characterDesiredVelocity.x)
					//	newVelocityPotential.SetX(min(newVelocityPotential.GetX(), body->characterDesiredVelocity.x));

					//newVelocity += Vec3(body->characterDesiredVelocity.x, body->characterDesiredVelocity.y, 0);

					//newVelocity += Vec3(currentVelocity.GetX(), currentVelocity.GetY(), 0);
				}
			}
			else
				newVelocity = Vec3(0, 0, 0);

			character->SetLinearVelocity(newVelocity);


			auto gravity2 = inactive ? Vec3(0, 0, 0) : gravity;
			IgnoreSingleBodyFilter bodyFilter(body->body->GetID());

			//auto oldPosition = character->GetPosition();

			RVec3 positionBeforeWalkUpDown;

			character->ExtendedUpdate(deltaTime,
				gravity2,
				*body->characterUpdateSettings,
				system->system.GetDefaultBroadPhaseLayerFilter(Layers::MOVING_AND_CHARACTER),
				system->system.GetDefaultLayerFilter(Layers::MOVING_AND_CHARACTER),
				bodyFilter,
				{ },
				tempAllocator,
				positionBeforeWalkUpDown);


			//fix sliding on dynamic body
			if (character->GetGroundState() == CharacterBase::EGroundState::OnGround && body->characterDesiredVelocity == Vector2::ZERO)
			{
				auto groundBodyID = character->GetGroundBodyID();
				BodyInterface& bodyInterface = body->system->GetBodyInterfaceNoLock();

				if (bodyInterface.IsActive(groundBodyID))
				{
					auto body2 = body->body;
					auto diffPosition = body2->GetPosition() - character->GetPosition();
					auto velocity = character->GetLinearVelocity();
					//auto diffVelocity = body2->GetLinearVelocity() - character->GetLinearVelocity();
					float thresholdPos = 0.03f;
					float thresholdVel = 0.2f;
					if (abs(diffPosition.GetX()) < thresholdPos && abs(diffPosition.GetY()) < thresholdPos && abs(diffPosition.GetZ()) < thresholdPos &&
						abs(velocity.GetX()) < thresholdVel && abs(velocity.GetY()) < thresholdVel && abs(velocity.GetZ()) < thresholdVel)
						//abs(diffVelocity.GetX()) < thresholdVel && abs(diffVelocity.GetY()) < thresholdVel && abs(diffVelocity.GetZ()) < thresholdVel)
					{
						character->SetPosition(body2->GetPosition());
						character->SetLinearVelocity(Vec3(0, 0, 0));
					}
				}
			}

			body->characterWalkUpDownLastChange = (float)(character->GetPosition().GetZ() - positionBeforeWalkUpDown.GetZ());


			//auto effectiveVelocity = (character->GetPosition() - oldPosition) / deltaTime;
			//body->characterLastEffenciveVelocity = Vector2(effectiveVelocity.GetX(), effectiveVelocity.GetY());

			//auto effectiveVelocity = (character->GetPosition() - oldPosition) / deltaTime;
			////if (body->characterWalkUpDownLastChange != 0)
			////	effectiveVelocity.SetZ(0);
			//character->SetLinearVelocity(Vec3(effectiveVelocity.GetX(), effectiveVelocity.GetY(), character->GetLinearVelocity().GetZ()));

			//auto effectiveVelocity = (character->GetPosition() - oldPosition) / deltaTime;
			//if (body->characterWalkUpDownLastChange != 0)
			//	effectiveVelocity.SetZ(0);
			//character->SetLinearVelocity(Vec3(effectiveVelocity));
		}
	}

	system->system.Update(deltaTime, collisionSteps, &tempAllocator, &job_system);
	//system->system.Update(deltaTime, collisionSteps, integrationSubSteps, &tempAllocator, &job_system);

	//process activated vehicles
	{
		for (VehicleConstraintItem* vehicleConstraint : system->vehiclesToActivate)
		{
			VehicleConstraint* c2 = (VehicleConstraint*)vehicleConstraint->constraint.GetPtr();

			if (c2->GetVehicleBody()->IsActive() || c2->IsActive())
			{
				if (!vehicleConstraint->stepListenerAdded)
				{
					system->system.AddStepListener(c2);
					vehicleConstraint->stepListenerAdded = true;
				}
			}
		}

		system->vehiclesToActivate.clear();
	}

	//deactivate vehicles. iterate vehicle step listeners
	{
		std::vector<PhysicsStepListener*> toRemove;

		PhysicsSystem::StepListeners& stepListeners = system->system.mStepListeners;
		for (int n = 0; n < stepListeners.size(); n++)
		{
			PhysicsStepListener* listener = stepListeners[n];
			if (listener->isVehiclePhysicsStepListener)
			{
				VehicleConstraint* c2 = (VehicleConstraint*)listener;

				if (!c2->GetVehicleBody()->IsActive() && !c2->IsActive())
				{
					VehicleConstraintItem* vehicleConstraint = (VehicleConstraintItem*)c2->GetUserData();
					vehicleConstraint->stepListenerAdded = false;
					toRemove.push_back(c2);
				}
			}
		}

		for (int n = 0; n < toRemove.size(); n++)
			system->system.RemoveStepListener(toRemove[n]);
	}


	//sync bodies by virtual character (body character mode)
	for (const auto& pair : system->bodyCharacterModeById) //for each (auto pair in system->bodyCharacterModeById)
	{
		auto body = pair.second;

		auto character = body->character.GetPtr();
		if (character != nullptr)
		{
			auto body2 = body->body;

			//!!!!slowly?
			if (body2->GetPosition() != character->GetPosition() || body2->GetLinearVelocity() != character->GetLinearVelocity())
			{
				BodyInterface& bodyInterface = body->system->GetBodyInterfaceNoLock();
				bodyInterface.SetPosition(body->id, character->GetPosition(), EActivation::Activate);
				bodyInterface.SetLinearVelocity(body->id, character->GetLinearVelocity());

				//ground velocity is not used inside the library, it is only for user
				character->UpdateGroundVelocity();
			}
		}
	}
}

EXPORT void JPhysicsSystem_GetActiveBodies(PhysicsSystemItem* system, int& count, uint*& bodies)
{
	count = system->system.GetNumActiveBodies();

	BodyIDVector list;
	list.reserve(count);
	system->system.GetActiveBodies(list);

	if (list.size() != count)
		Fatal("JPhysicsSystem_GetActiveBodies: list.size() != count.");

	bodies = new uint[count];
	for (int n = 0; n < count; n++)
		bodies[n] = list[n].GetIndexAndSequenceNumber();
}

EXPORT void JPhysicsSystem_GetActiveBodiesFree(PhysicsSystemItem* system, uint* bodies)
{
	delete[] bodies;
}

//EXPORT void JPhysicsSystem_DebugDraw(PhysicsSystemItem* system, int& lineCount, DebugDrawLineItem*& lines)
//{
//	DebugRendererImpl debugRenderer;
//
//	BodyManager::DrawSettings drawSettings;
//	//drawSettings.mDrawGetSupportFunction = true;				///< Draw the GetSupport() function, used for convex collision detection	
//	//drawSettings.mDrawSupportDirection = true;					///< When drawing the support function, also draw which direction mapped to a specific support point
//	//drawSettings.mDrawGetSupportingFace = true;					///< Draw the faces that were found colliding during collision detection
//	drawSettings.mDrawShape = true;								///< Draw the shapes of all bodies
//	drawSettings.mDrawShapeWireframe = true;					///< When mDrawShape is true and this is true, the shapes will be drawn in wireframe instead of solid.
//	drawSettings.mDrawShapeColor = BodyManager::EShapeColor::MotionTypeColor; ///< Coloring scheme to use for shapes
//
//	//!!!!
//	//drawSettings.mDrawBoundingBox = true;						///< Draw a bounding box per body
//
//	//drawSettings.mDrawCenterOfMassTransform = true;				///< Draw the center of mass for each body
//	//drawSettings.mDrawWorldTransform = true;					///< Draw the world transform (which can be different than the center of mass) for each body
//	//drawSettings.mDrawVelocity = true;							///< Draw the velocity vector for each body
//	//drawSettings.mDrawMassAndInertia = true;					///< Draw the mass and inertia (as the box equivalent) for each body
//	//drawSettings.mDrawSleepStats = true;						///< Draw stats regarding the sleeping algorithm of each body
//
//	//DebugRendererImpl::LineItem item;
//	//item.from = Vector3D(0, 0, 0);
//	//item.to = Vector3D(10, 0, 0);
//	//item.color = Color(255, 0, 0, 255);
//	//debugRenderer.lines.push_back(item);
//
//	system->system.DrawBodies(drawSettings, &debugRenderer);
//
//	lineCount = debugRenderer.lines.size();
//	lines = new DebugDrawLineItem[lineCount];
//	for (int n = 0; n < lineCount; n++)
//		lines[n] = debugRenderer.lines[n];
//}
//
//EXPORT void JPhysicsSystem_DebugDrawFree(PhysicsSystemItem* system, DebugDrawLineItem* lines)
//{
//	delete[] lines;
//}


////EXPORT void JPhysicsSystem_DebugDraw(PhysicsSystemItem* system, int& lineCount, DebugRendererImpl::LineItem*& lines)
////{
////	DebugRendererImpl debugRenderer;
////
////	BodyManager::DrawSettings drawSettings;
////	//drawSettings.mDrawGetSupportFunction = true;				///< Draw the GetSupport() function, used for convex collision detection	
////	//drawSettings.mDrawSupportDirection = true;					///< When drawing the support function, also draw which direction mapped to a specific support point
////	//drawSettings.mDrawGetSupportingFace = true;					///< Draw the faces that were found colliding during collision detection
////	drawSettings.mDrawShape = true;								///< Draw the shapes of all bodies
////	drawSettings.mDrawShapeWireframe = true;					///< When mDrawShape is true and this is true, the shapes will be drawn in wireframe instead of solid.
////	drawSettings.mDrawShapeColor = BodyManager::EShapeColor::MotionTypeColor; ///< Coloring scheme to use for shapes
////
////	//!!!!
////	//drawSettings.mDrawBoundingBox = true;						///< Draw a bounding box per body
////
////	//drawSettings.mDrawCenterOfMassTransform = true;				///< Draw the center of mass for each body
////	//drawSettings.mDrawWorldTransform = true;					///< Draw the world transform (which can be different than the center of mass) for each body
////	//drawSettings.mDrawVelocity = true;							///< Draw the velocity vector for each body
////	//drawSettings.mDrawMassAndInertia = true;					///< Draw the mass and inertia (as the box equivalent) for each body
////	//drawSettings.mDrawSleepStats = true;						///< Draw stats regarding the sleeping algorithm of each body
////
////	//DebugRendererImpl::LineItem item;
////	//item.from = Vector3D(0, 0, 0);
////	//item.to = Vector3D(10, 0, 0);
////	//item.color = Color(255, 0, 0, 255);
////	//debugRenderer.lines.push_back(item);
////
////	system->system.DrawBodies(drawSettings, &debugRenderer);
////
////	lineCount = debugRenderer.lines.size();
////	lines = new DebugRendererImpl::LineItem[lineCount];
////	for (int n = 0; n < lineCount; n++)
////		lines[n] = debugRenderer.lines[n];
////}
////
////EXPORT void JPhysicsSystem_DebugDrawFree(PhysicsSystemItem* system, DebugRendererImpl::LineItem* lines)
////{
////	delete[] lines;
////}

EXPORT BodyItem* JCreateBody(PhysicsSystemItem* system, ShapeItem* shape, int motionType, float linearDamping, float angularDamping, const Vector3D& position, const Vector4& rotation, bool activate, float mass, const Vector3& centerOfMassOffset, /*bool centerOfMassManual, const Vector3& centerOfMassPosition, const Vector3& inertiaTensorFactor, */uint& resultBodyId, int motionQuality, bool characterMode)
{
	if (linearDamping < 0)
		linearDamping = 0;
	if (angularDamping < 0)
		angularDamping = 0;
	if (mass < 0)
		mass = 0;

	BodyInterface& bodyInterface = system->system.GetBodyInterfaceNoLock();

	auto motionType2 = (EMotionType)motionType;

	ObjectLayer objectLayer = (motionType2 != EMotionType::Static) ? Layers::MOVING : Layers::NON_MOVING;
	if (characterMode)
		objectLayer = Layers::CHARACTER;

	ShapeSettings* shapeToCreate = shape->compoundShapeSettings;

	if (centerOfMassOffset != Vector3::ZERO)
	{
		shape->offsetCenterOfMassShapeSettings = new OffsetCenterOfMassShapeSettings(ConvertToVec3(centerOfMassOffset), shape->compoundShapeSettings);
		shape->offsetCenterOfMassShapeSettings->SetEmbedded();

		shapeToCreate = shape->offsetCenterOfMassShapeSettings;
	}	

	BodyCreationSettings settings(shapeToCreate/*shape->compoundShapeSettings*/, ConvertToRVec3(position), ConvertToQuat(rotation), motionType2, objectLayer);

	settings.mMotionQuality = (EMotionQuality)motionQuality;
	settings.mMassPropertiesOverride.mMass = mass;

	settings.mOverrideMassProperties = EOverrideMassProperties::CalculateInertia;
	settings.mInertiaMultiplier = 1.0f;


	//if (centerOfMassOffset != Vector3::ZERO)
	//{
	//	//settings.mOverrideMassProperties = EOverrideMassProperties::MassAndInertiaProvided;
	//	//settings.mInertiaMultiplier = 1.0f;

	//	//settings.mMassPropertiesOverride.mInertia = Mat44::sScale(ConvertToVec3(Vector3D(0.01, 1, 1)));

	//	settings.mOverrideMassProperties = EOverrideMassProperties::MassAndInertiaProvided;
	//	settings.mInertiaMultiplier = 1.0f;

	//	const Shape* shape = settings.GetShape();

	//	settings.mMassPropertiesOverride = shape->GetMassProperties2(ConvertToVec3(centerOfMassOffset));
	//	//settings.mMassPropertiesOverride = ((CompoundShape*)settings.GetShape())->GetMassProperties2(ConvertToVec3(centerOfMassOffset));
	//	settings.mMassPropertiesOverride.ScaleToMass(mass);
	//	//settings.mMassPropertiesOverride.mInertia *= settings.mInertiaMultiplier;
	//	settings.mMassPropertiesOverride.mInertia(3, 3) = 1.0f;

	//	//settings.mMassPropertiesOverride.Translate(ConvertToVec3(centerOfMassOffset));
	//}
	//else
	//{
	//	settings.mOverrideMassProperties = EOverrideMassProperties::CalculateInertia;
	//	settings.mInertiaMultiplier = 1.0f;
	//}


	//if (centerOfMassOffset != Vector3::ZERO)
	//{
	//	settings.mOverrideMassProperties = EOverrideMassProperties::MassAndInertiaProvided;
	//	settings.mInertiaMultiplier = 1.0f;

	//	settings.mMassPropertiesOverride = settings.GetShape()->GetMassProperties();
	//	settings.mMassPropertiesOverride.ScaleToMass(mass);
	//	//settings.mMassPropertiesOverride.mInertia *= settings.mInertiaMultiplier;
	//	settings.mMassPropertiesOverride.mInertia(3, 3) = 1.0f;

	//	settings.mMassPropertiesOverride.Translate(ConvertToVec3(centerOfMassOffset));
	//}
	//else
	//{
	//	settings.mOverrideMassProperties = EOverrideMassProperties::CalculateInertia;
	//	settings.mInertiaMultiplier = 1.0f;
	//}


	////if (centerOfMassManual)
	////{
	////	settings.mOverrideMassProperties = EOverrideMassProperties::MassAndInertiaProvided;
	////	settings.mInertiaMultiplier = 1.0f;

	////	settings.mMassPropertiesOverride = settings.GetShape()->GetMassProperties();
	////	settings.mMassPropertiesOverride.ScaleToMass(mass);
	////	//settings.mMassPropertiesOverride.mInertia *= settings.mInertiaMultiplier;
	////	settings.mMassPropertiesOverride.mInertia(3, 3) = 1.0f;

	////	//!!!!
	////	settings.mMassPropertiesOverride.Translate(ConvertToVec3(Vector3D(0, 0, -2)));
	////	//settings.mMassPropertiesOverride.Translate(ConvertToVec3(centerOfMassPosition));

	////}
	////else
	////{
	////	settings.mOverrideMassProperties = EOverrideMassProperties::CalculateInertia;
	////	settings.mInertiaMultiplier = 1.0f;
	////}

	settings.mLinearDamping = linearDamping;
	settings.mAngularDamping = angularDamping;

	//!!!!
	//CollisionGroup mCollisionGroup; ///< The collision group this body belongs to (determines if two objects can collide)

	//!!!!также thresholds
	//bool mAllowSleeping = true; ///< If this body can go to sleep or not

	//float mMaxLinearVelocity = 500.0f; ///< Maximum linear velocity that this body can reach (m/s)
	//float mMaxAngularVelocity = 0.25f * JPH_PI * 60.0f; ///< Maximum angular velocity that this body can reach (rad/s)


	//!!!!
	//mAllowDynamicOrKinematic
	//mIsSensor


	auto body = new BodyItem();
	body->system = &system->system;
	body->systemItem = system;
	body->body = bodyInterface.CreateBody(settings);
	if (body->body == nullptr)
	{
		//max count limit
		return nullptr;
	}
	body->id = body->body->GetID();
	body->body->SetUserData((JPH::uint64)body);

	bodyInterface.AddBody(body->id, activate ? EActivation::Activate : EActivation::DontActivate);

	resultBodyId = body->id.GetIndexAndSequenceNumber();

	system->bodyById[resultBodyId] = body;
	if (characterMode)
		system->bodyCharacterModeById[resultBodyId] = body;
	//system->bodyById.insert(std::pair<uint, BodyItem*>(resultBodyId, body));


	if (characterMode)
	{
		Ref<CharacterVirtualSettings> settings = new CharacterVirtualSettings();

		settings->mShape = body->body->GetShape();// mStandingShape;

		//settings->mCharacterPadding = sCharacterPadding;
		//settings->mPenetrationRecoverySpeed = sPenetrationRecoverySpeed;
		//settings->mPredictiveContactDistance = sPredictiveContactDistance;

		body->character = new CharacterVirtual(settings, body->body->GetPosition(), body->body->GetRotation(), body->system);
		body->character->SetMass(mass);
		body->character->SetUp(Vec3(0, 0, 1));

		body->body->GetMotionProperties()->SetGravityFactor(0);

		body->characterUpdateSettings = new CharacterVirtual::ExtendedUpdateSettings();

		//body->characterUpdateSettings->mWalkStairsStepForwardTest = 0.3f;


		//!!!!
		//body->character->SetListener(this);
	}

	return body;
}

EXPORT void JDestroyBody(PhysicsSystemItem* system, BodyItem* body)
{
	//!!!!what to delete
	//JBody_SetCharacterMode(body, false);

	if (body->characterUpdateSettings != nullptr)
	{
		delete body->characterUpdateSettings;
		body->characterUpdateSettings = nullptr;
	}

	system->bodyById.erase(body->id.GetIndexAndSequenceNumber());
	if (body->character != nullptr)
		system->bodyCharacterModeById.erase(body->id.GetIndexAndSequenceNumber());
	system->bodiesContactsToClear.erase(body);

	BodyInterface& bodyInterface = body->system->GetBodyInterfaceNoLock();
	bodyInterface.RemoveBody(body->id);
	bodyInterface.DestroyBody(body->id);

	delete body;
}

EXPORT void JBody_GetData(BodyItem* body, Vector3D& position, Vector4& rotation, Vector3& linearVelocity, Vector3& angularVelocity, bool& active)
{
	auto body2 = body->body;

	position = ConvertToVector3D(body2->GetPosition());
	//position = ConvertToVector3D(body2->GetCenterOfMassPosition());

	rotation = ConvertToQuaternion(body2->GetRotation());
	linearVelocity = ConvertToVector3(body2->GetLinearVelocity());
	angularVelocity = ConvertToVector3(body2->GetAngularVelocity());
	active = body2->IsActive();
}

EXPORT void JBody_GetAABB(BodyItem* body, Vector3D& boundsMin, Vector3D& boundsMax)
{
	auto body2 = body->body;
	boundsMin = ConvertToVector3D(DVec3(body2->GetWorldSpaceBounds().mMin));
	boundsMax = ConvertToVector3D(DVec3(body2->GetWorldSpaceBounds().mMax));
}

EXPORT void JBody_GetShapeCenterOfMass(BodyItem* body, Vector3& centerOfMass)
{
	auto body2 = body->body;
	centerOfMass = ConvertToVector3(body2->GetShape()->GetCenterOfMass());
}

EXPORT void JBody_Activate(BodyItem* body)
{
	//!!!!character

	BodyInterface& bodyInterface = body->system->GetBodyInterfaceNoLock();
	bodyInterface.ActivateBody(body->id);
}

EXPORT void JBody_Deactivate(BodyItem* body)
{
	//!!!!character

	BodyInterface& bodyInterface = body->system->GetBodyInterfaceNoLock();
	bodyInterface.DeactivateBody(body->id);
}

EXPORT void JBody_SetLinearVelocity(BodyItem* body, const Vector3& value)
{
	auto body2 = body->body;
	if (!body2->IsStatic())
	{
		body2->SetLinearVelocityClamped(ConvertToVec3(value));

		if (body->character != nullptr)
			body->character->SetLinearVelocity(ConvertToVec3(value));
	}
}

EXPORT void JBody_SetAngularVelocity(BodyItem* body, const Vector3& value)
{
	auto body2 = body->body;
	if (!body2->IsStatic())
		body2->SetAngularVelocityClamped(ConvertToVec3(value));
}

EXPORT void JBody_SetFriction(BodyItem* body, float value)
{
	//!!!!character

	if (value < 0)
		value = 0;
	body->body->SetFriction(value);
}

EXPORT void JBody_SetRestitution(BodyItem* body, float value)
{
	//!!!!character

	if (value < 0)
		value = 0;
	if (value > 1)
		value = 1;
	body->body->SetRestitution(value);
}

EXPORT void JBody_SetLinearDamping(BodyItem* body, float value)
{
	//!!!!character

	if (value < 0)
		value = 0;
	auto body2 = body->body;
	if (!body2->IsStatic() && body2->GetMotionProperties() != nullptr)
		body2->GetMotionProperties()->SetLinearDamping(value);
}

EXPORT void JBody_SetAngularDamping(BodyItem* body, float value)
{
	//!!!!character

	if (value < 0)
		value = 0;
	auto body2 = body->body;
	if (!body2->IsStatic() && body2->GetMotionProperties() != nullptr)
		body2->GetMotionProperties()->SetAngularDamping(value);
}

EXPORT void JBody_SetGravityFactor(BodyItem* body, float value)
{
	//!!!!character

	auto body2 = body->body;
	if (!body2->IsStatic() && body2->GetMotionProperties() != nullptr && body->character == nullptr)
		body2->GetMotionProperties()->SetGravityFactor(value);
}

//EXPORT void JBody_SetShape(BodyItem* body, )
//{
//}

EXPORT void JBody_SetTransform(BodyItem* body, const Vector3D& position, const Vector4& rotation, bool activate)
{
	BodyInterface& bodyInterface = body->system->GetBodyInterfaceNoLock();
	bodyInterface.SetPositionAndRotation(body->id, ConvertToRVec3(position), ConvertToQuat(rotation), activate ? EActivation::Activate : EActivation::DontActivate);

	auto character = body->character.GetPtr();
	if (character != nullptr)
	{
		character->SetPosition(ConvertToRVec3(position));
		character->SetRotation(ConvertToQuat(rotation));
	}
}

EXPORT void JBody_ApplyForce(BodyItem* body, const Vector3& force, const Vector3& relativePosition)
{
	//!!!!character

	auto body2 = body->body;

	if (body2->IsDynamic() && body2->GetMotionProperties() != nullptr)
	{
		body2->AddForce(ConvertToVec3(force));
		if (relativePosition != Vector3::ZERO)
			body2->AddTorque(ConvertToVec3(relativePosition.crossProduct(force)));

		JBody_Activate(body);

		//void Body::AddForce(Vec3Arg inForce, RVec3Arg inPosition)
		//AddForce(inForce);
		//AddTorque(Vec3(inPosition - mPosition).Cross(inForce));
	}
}

EXPORT void JBody_SetMotionQuality(BodyItem* body, int motionQuality)
{
	BodyInterface& bodyInterface = body->system->GetBodyInterfaceNoLock();
	bodyInterface.SetMotionQuality(body->id, (EMotionQuality)motionQuality);
}

EXPORT void JBody_GetContacts(BodyItem* body, int& count, ContactItem*& buffer)
{
	//!!!!character

	body->subscribedToGetContacts = true;

	count = body->contacts.size();
	if (count != 0)
		buffer = &body->contacts[0];
	else
		buffer = nullptr;
}

EXPORT bool JBody_ContactsExist(BodyItem* body)
{
	//!!!!character

	body->subscribedToGetContacts = true;

	return body->contacts.size() != 0;
}

EXPORT void JBody_SetInverseInertia(BodyItem* body, const Vector3& diagonal, const Vector4& rotation)
{
	auto body2 = body->body;

	if (body2->IsDynamic() && body2->GetMotionProperties() != nullptr)
		body2->GetMotionProperties()->SetInverseInertia(ConvertToVec3(diagonal), ConvertToQuat(rotation));
}

EXPORT void JBody_GetWorldSpaceSurfaceNormal(BodyItem* body, uint subShapeID, const Vector3D& position, Vector3& normal)
{
	SubShapeID subShapeID2;
	subShapeID2.SetValue(subShapeID);

	normal = ConvertToVector3(body->body->GetWorldSpaceSurfaceNormal(subShapeID2, ConvertToRVec3(position)));
}

EXPORT void JBody_SetCharacterModeMaxStrength(BodyItem* body, float value)
{
	if (body->character != nullptr)
		body->character->SetMaxStrength(value);
}

EXPORT void JBody_SetCharacterModePredictiveContactDistance(BodyItem* body, float value)
{
	if (body->character != nullptr)
		body->character->mPredictiveContactDistance = value;
}

EXPORT void JBody_SetCharacterModeWalkUpHeight(BodyItem* body, float value)
{
	if (body->character != nullptr)
		body->characterUpdateSettings->mWalkStairsStepUp = Vec3(0, 0, value);
}

EXPORT void JBody_SetCharacterModeWalkDownHeight(BodyItem* body, float value)
{
	if (body->character != nullptr)
		body->characterUpdateSettings->mStickToFloorStepDown = Vec3(0, 0, -value);
}

EXPORT void JBody_SetCharacterModeMaxSlopeAngle(BodyItem* body, float value)
{
	if (body->character != nullptr)
		body->character->SetMaxSlopeAngle(value);
}

EXPORT void JBody_SetCharacterModeSetSupportingVolume(BodyItem* body, float value)
{
	if (body->character != nullptr)
		body->character->mSupportingVolume = JPH::Plane(Vec3::sAxisZ(), -value);
}

EXPORT void JBody_SetCharacterModeSetDesiredVelocity(BodyItem* body, const Vector2& value)
{
	//if (body->character != nullptr)
	body->characterDesiredVelocity = value;
}

EXPORT void JBody_GetCharacterModeData(BodyItem* body, CharacterBase::EGroundState& groundState, uint& groundBodyID, uint& groundBodySubShapeID, Vector3D& groundPosition, Vector3& groundNormal, Vector3& groundVelocity, float& walkUpDownLastChange)
{
	auto character = body->character.GetPtr();

	groundState = character->GetGroundState();
	groundBodyID = character->GetGroundBodyID().GetIndexAndSequenceNumber();
	groundBodySubShapeID = character->GetGroundSubShapeID().GetValue();
	groundPosition = ConvertToVector3D(character->GetGroundPosition());
	groundNormal = ConvertToVector3(character->GetGroundNormal());
	groundVelocity = ConvertToVector3(character->GetGroundVelocity());
	walkUpDownLastChange = body->characterWalkUpDownLastChange;
}

EXPORT ShapeItem* JCreateShape(bool mutableCompoundShape)
{
	auto item = new ShapeItem();

	if (mutableCompoundShape)
		item->compoundShapeSettings = new MutableCompoundShapeSettings();
	else
		item->compoundShapeSettings = new StaticCompoundShapeSettings();

	item->compoundShapeSettings->SetEmbedded();

	item->offsetCenterOfMassShapeSettings = NULL;

	return item;
}

EXPORT void JDestroyShape(ShapeItem* shape)
{
	//!!!!leaks?

	if (shape->offsetCenterOfMassShapeSettings)
		delete shape->offsetCenterOfMassShapeSettings;
	else
		delete shape->compoundShapeSettings;

	delete shape;
}

EXPORT bool JShape_IsValid(ShapeItem* shape)
{
	return shape->compoundShapeSettings->mSubShapes.size() != 0;
}

//EXPORT JPH::Shape* JCompoundShapeSettings_GetShape(CompoundShapeSettings* compoundShapeSettings)
//{
//	JPH::ShapeSettings::ShapeResult shapeResult = compoundShapeSettings->Create();
//	ShapeRefC shape = shapeResult.Get();
//	//return shapeResult.Get();
//}

EXPORT void JShape_AddSphere(ShapeItem* shape, const Vector3& position, const Vector4& rotation, float radius)
{
	if (radius < 0.001f)
		radius = 0.001f;

	//!!!!material. всем такое

	shape->compoundShapeSettings->AddShape(ConvertToVec3(position), ConvertToQuat(rotation), new SphereShape(radius));
}

EXPORT void JShape_AddBox(ShapeItem* shape, const Vector3& position, const Vector4& rotation, const Vector3& dimensions, float convexRadius)
{
	auto halfDimensions = dimensions * 0.5f;

	//float convexRadius = cDefaultConvexRadius;

	if (convexRadius > halfDimensions.x)
		convexRadius = halfDimensions.x;
	if (convexRadius > halfDimensions.y)
		convexRadius = halfDimensions.y;
	if (convexRadius > halfDimensions.z)
		convexRadius = halfDimensions.z;

	shape->compoundShapeSettings->AddShape(ConvertToVec3(position), ConvertToQuat(rotation), new BoxShape(ConvertToVec3(halfDimensions), convexRadius));
	//shape->compoundShapeSettings->AddShape(ConvertToVec3(position), rotation, new BoxShape(ConvertToVec3(dimensions) * 0.5f, convexRadius));
}

EXPORT void JShape_AddCapsule(ShapeItem* shape, const Vector3& position, const Vector4& rotation, float halfHeightOfCylinder, float radius)
{
	if (halfHeightOfCylinder < 0.001f)
		halfHeightOfCylinder = 0.001f;
	if (radius < 0.001f)
		radius = 0.001f;

	//axis is already applied to rotation

	shape->compoundShapeSettings->AddShape(ConvertToVec3(position), ConvertToQuat(rotation), new CapsuleShape(halfHeightOfCylinder, radius));
}

EXPORT void JShape_AddCylinder(ShapeItem* shape, const Vector3& position, const Vector4& rotation, float halfHeightOfCylinder, float radius, float convexRadius)
{
	//axis is already applied to rotation

	if (halfHeightOfCylinder < 0.001f)
		halfHeightOfCylinder = 0.001f;
	if (radius < 0.001f)
		radius = 0.001f;

	//float convexRadius = cDefaultConvexRadius;

	if (convexRadius > halfHeightOfCylinder)
		convexRadius = halfHeightOfCylinder;
	if (convexRadius > radius)
		convexRadius = radius;

	shape->compoundShapeSettings->AddShape(ConvertToVec3(position), ConvertToQuat(rotation), new CylinderShape(halfHeightOfCylinder, radius, convexRadius));
}

//EXPORT void JShape_AddCone(ShapeItem* shape, const Vector3& position, const Vector4& rotation, float halfHeightOfCylinder, float radius)
//{
//	//axis is already applied to rotation
//
//	float convexRadius = cDefaultConvexRadius;
//
//	//shape->compoundShapeSettings->AddShape(Convert(position), rotation, new ConeShape(halfHeightOfCylinder, radius, convexRadius));
//}

EXPORT void JShape_AddMesh(ShapeItem* shape, const Vector3& position, const Vector4& rotation, const Vector3& localScaling, Vector3* vertices, int vertexCount, int* indices, int indexCount)
{
	VertexList vertices2;
	vertices2.reserve(vertexCount);
	for (int n = 0; n < vertexCount; n++)
		vertices2.push_back(ConvertToFloat3(vertices[n] * localScaling));

	IndexedTriangleList triangles;
	auto triangleCount = indexCount / 3;
	for (int nTriangle = 0; nTriangle < triangleCount; nTriangle++)
	{
		auto index0 = indices[nTriangle * 3 + 0];
		auto index1 = indices[nTriangle * 3 + 1];
		auto index2 = indices[nTriangle * 3 + 2];

		//!!!!
		uint materialIndex = 0;

		triangles.push_back(IndexedTriangle(index0, index1, index2, materialIndex));
	}

	shape->compoundShapeSettings->AddShape(ConvertToVec3(position), ConvertToQuat(rotation), new MeshShapeSettings(vertices2, triangles));


	//auto triangleCount = indexCount / 3;

	//TriangleList triangles;
	//triangles.reserve(triangleCount);

	//for (int nTriangle = 0; nTriangle < triangleCount; nTriangle++)
	//{
	//	auto v0 = Convert(vertices[indices[nTriangle * 3 + 0]]);
	//	auto v1 = Convert(vertices[indices[nTriangle * 3 + 1]]);
	//	auto v2 = Convert(vertices[indices[nTriangle * 3 + 2]]);
	//	triangles.push_back(Triangle(v0, v1, v2));
	//}
}

EXPORT void JShape_AddConvexHull(ShapeItem* shape, const Vector3& position, const Vector4& rotation, const Vector3& localScaling, Vector3* points, int pointCount, float convexRadius)
{
	Array<Vec3> points2;
	points2.reserve(pointCount);
	for (int n = 0; n < pointCount; n++)
		points2.push_back(ConvertToVec3(points[n] * localScaling));

	//float convexRadius = cDefaultConvexRadius;

	shape->compoundShapeSettings->AddShape(ConvertToVec3(position), ConvertToQuat(rotation), new ConvexHullShapeSettings(points2, convexRadius));
}

///////////////////////////////////////////////////////////////////////////////////////////////////

enum class RayTestModeEnum
{
	One,
	OneClosest,
	OneForEach,
	OneClosestForEach,
	All
};

enum RayTestFlagsEnum
{
	RayTestFlagsEnum_None = 0,
	RayTestFlagsEnum_CalculateNormal = 1,

	//!!!!triangle id?
};

bool SortRayTestResultNative(const RayTestResultNative& v0, const RayTestResultNative& v1)
{
	return v0.distanceScale < v1.distanceScale;
};

EXPORT void JPhysicsSystem_RayTest(PhysicsSystemItem* system, const RayD& ray, int mode2, int flags2, int& resultCount, RayTestResultNative*& results, ShapeItem* testShape)
{
	auto mode = (RayTestModeEnum)mode2;
	auto flags = (RayTestFlagsEnum)flags2;


	//!!!!можно искать например только Dynamic

	//!!!!EarlyOutFraction

	//!!!!что-то фильтровать здесь


	RayCastSettings settings;
	//!!!!always need?
	settings.mBackFaceMode = EBackFaceMode::CollideWithBackFaces;
	//!!!!
	//settings.mTreatConvexAsSolid = false;


	class MyCollector : public CastRayCollector
	{
	public:
		PhysicsSystemItem* system;
		RayD ray;
		RayTestModeEnum mode;
		RayTestFlagsEnum flags;
		ShapeItem* testShape;
		ShapeSettings::ShapeResult testShapeResult;
		std::vector<RayTestResultNative> results;

		//!!!!?
		//std::set<uint> addedBodies;

		//

		virtual void AddHit(const RayCastResult& inResult) override
		{
			RayTestResultNative result2;

			result2.bodyId = inResult.mBodyID.GetIndexAndSequenceNumber();

			//!!!!shapeIndex
			result2.shapeIndex = 0;

			result2.distanceScale = inResult.mFraction;
			result2.position = ray.getPoint(result2.distanceScale);


			//!!!!
			//SubShapeID mSubShapeID2;						///< Sub shape ID of shape that we collided against

			//!!!!all params

			//!!!!нормаль можно в конце посчитать только для нужных
			if ((flags & RayTestFlagsEnum_CalculateNormal) != 0)
			{
				//!!!!impl
				if (testShape == nullptr)
				{
					BodyItem* body = system->bodyById.find(result2.bodyId)->second;
					result2.normal = ConvertToVector3(body->body->GetWorldSpaceSurfaceNormal(inResult.mSubShapeID2, ConvertToRVec3(result2.position)));
				}
				else
				{
					result2.normal = Vector3::ZERO;

					//!!!!wrong. need calculate from center of mass
					//RMat44 inv_com = GetInverseCenterOfMassTransform();
					//result2.normal = ConvertToVector3(testShapeResult.Get()->GetSurfaceNormal(inResult.mSubShapeID2, ConvertToVec3(result2.position)));



					////int shapeIndex = inResult.mSubShapeID2.GetValue();

					////const Array<CompoundShapeSettings::SubShapeSettings>& subShapes = testShape->compoundShapeSettings->mSubShapes;
					////if (shapeIndex >= 0 && shapeIndex < subShapes.size())
					////{
					////	const Shape* shape = subShapes[0].mShapePtr.GetPtr();
					////	//const Shape* shape = subShapes[inResult.mSubShapeID2.GetValue()].mShapePtr.GetPtr();
					////	result2.normal = ConvertToVector3(shape->GetSurfaceNormal(inResult.mSubShapeID2, ConvertToVec3(result2.position)));
					////}
					//////else
					//////	result2.normal = Vector3::ZERO;
				}
			}
			else
				result2.normal = Vector3::ZERO;

			//!!!!
			//!!!!DecodeSubShapeID
			result2.triangleIndexSource = 0;
			result2.triangleIndexProcessed = 0;


			if (mode == RayTestModeEnum::OneForEach || mode == RayTestModeEnum::OneClosestForEach)
			{
				//replace with with same body
				for (int n = 0; n < results.size(); n++)
				{
					if (results[n].bodyId == result2.bodyId)
					{
						if (mode == RayTestModeEnum::OneClosestForEach && result2.distanceScale < results[n].distanceScale)
							results[n] = result2;
						return;
					}
				}
			}

			results.push_back(result2);


			if (mode == RayTestModeEnum::One)
				ForceEarlyOut();


			//!!!!
			//if (body->IsSensor())
			//	return;


			//!!!!
			//// Test if this collision is closer than the previous one
			//if (inResult.mFraction < GetEarlyOutFraction())
			//{
			//	// Lock the body
			//	BodyLockRead lock(mPhysicsSystem.GetBodyLockInterfaceNoLock(), inResult.mBodyID);
			//	JPH_ASSERT(lock.Succeeded()); // When this runs all bodies are locked so this should not fail
			//	const Body* body = &lock.GetBody();

			//	if (body->IsSensor())
			//		return;

			//	// Test that we're not hitting a vertical wall
			//	RVec3 contact_pos = mRay.GetPointOnRay(inResult.mFraction);
			//	Vec3 normal = body->GetWorldSpaceSurfaceNormal(inResult.mSubShapeID2, contact_pos);
			//	if (normal.Dot(mUpDirection) > mCosMaxSlopeAngle)
			//	{
			//		// Update early out fraction to this hit
			//		UpdateEarlyOutFraction(inResult.mFraction);

			//		// Get the contact properties
			//		mBody = body;
			//		mSubShapeID2 = inResult.mSubShapeID2;
			//		mContactPosition = contact_pos;
			//		mContactNormal = normal;
			//	}
			//}
		}
	};

	MyCollector collector;
	collector.system = system;
	collector.ray = ray;
	collector.mode = mode;
	collector.flags = flags;
	collector.testShape = testShape;
	//capacity
	collector.results.reserve(32);


	if (testShape != nullptr)
	{
		//!!!!
		collector.testShapeResult = testShape->compoundShapeSettings->Create();

		//ShapeSettings::ShapeResult testShapeResult = testShape->compoundShapeSettings->Create();

		//!!!!
		SubShapeIDCreator subShapeIDCreator;

		collector.testShapeResult.Get()->CastRay(ConvertToRay(ray), settings, subShapeIDCreator, collector);
	}
	else
	{
		auto& query = system->system.GetNarrowPhaseQuery();
		query.CastRay(ConvertToRRay(ray), settings, collector);
	}

	if (collector.results.size() > 1 && mode != RayTestModeEnum::OneForEach)
	{
		std::sort(collector.results.begin(), collector.results.end(), SortRayTestResultNative);

		if (mode == RayTestModeEnum::OneClosest || mode == RayTestModeEnum::One)
			collector.results.resize(1);
	}

	//out
	resultCount = collector.results.size();
	if (resultCount > 0)
	{
		results = new RayTestResultNative[resultCount];
		for (int n = 0; n < resultCount; n++)
			results[n] = collector.results[n];
	}
	else
		results = nullptr;


	//!!!!object layer. например можно быстрее только динамические выбрать. где еще так

	//query.CastRay(const RRayCast & inRay, const RayCastSettings & inRayCastSettings, CastRayCollector & ioCollector, const BroadPhaseLayerFilter & inBroadPhaseLayerFilter = { }, const ObjectLayerFilter & inObjectLayerFilter = { }, const BodyFilter & inBodyFilter = { }, const ShapeFilter & inShapeFilter = { }) const;
}

EXPORT void JPhysicsSystem_RayTestFree(PhysicsSystemItem* system, RayTestResultNative* results)
{
	delete[] results;
}

///////////////////////////////////////////////////////////////////////////////////////////////////

enum class VolumeTestModeEnum
{
	One,
	OneClosest,
	OneForEach,
	OneClosestForEach,
	All
};

enum VolumeTestFlagsEnum
{
	VolumeTestFlagsEnum_None = 0,
};

bool SortVolumeTestResultNative(const VolumeTestResultNative& v0, const VolumeTestResultNative& v1)
{
	return v0.distanceScale < v1.distanceScale;
};

void VolumeTestCommon(PhysicsSystemItem* system, int mode2, int& resultCount, VolumeTestResultNative*& results, Shape& shape, const RMat44& transform, const Vector3D& direction)
{
	auto mode = (VolumeTestModeEnum)mode2;

	ShapeCastSettings settings;

	Vector3D direction2 = direction;
	//!!!!good?
	if (direction2 == Vector3D::ZERO)
		direction2 = Vector3D(0, 0, 0.001);

	RShapeCast shapeCast(&shape, Vec3::sReplicate(1.0f), transform, ConvertToVec3(direction2));

	//!!!!new
	settings.mActiveEdgeMode = EActiveEdgeMode::CollideWithAll;

	//!!!!optionally?
	settings.mBackFaceModeTriangles = EBackFaceMode::CollideWithBackFaces;
	settings.mBackFaceModeConvex = EBackFaceMode::CollideWithBackFaces;

	//bool mReturnDeepestPoint = false;
	//ECollectFacesMode			mCollectFacesMode = ECollectFacesMode::NoFaces;
	//float						mCollisionTolerance = cDefaultCollisionTolerance;
	//float						mPenetrationTolerance = cDefaultPenetrationTolerance;
	//Vec3						mActiveEdgeMovementDirection = Vec3::sZero();


	class MyCollector : public CastShapeCollector
	{
	public:
		PhysicsSystemItem* system;
		RShapeCast* shapeCast;
		VolumeTestModeEnum mode;
		std::vector<VolumeTestResultNative> results;
		//std::set<uint> addedBodies;

		//

		virtual void AddHit(const ShapeCastResult& inResult) override
		{
			//!!!!more data


			//using Face = StaticArray<Vec3, 32>;
			//Vec3 mContactPointOn1; ///< Contact point on the surface of shape 1 (in world space or relative to base offset)
			//Vec3 mContactPointOn2; ///< Contact point on the surface of shape 2 (in world space or relative to base offset). If the penetration depth is 0, this will be the same as mContactPointOn1.
			//Vec3 mPenetrationAxis; ///< Direction to move shape 2 out of collision along the shortest path (magnitude is meaningless, in world space). You can use -mPenetrationAxis.Normalized() as contact normal.
			//float mPenetrationDepth; ///< Penetration depth (move shape 2 by this distance to resolve the collision)
			//SubShapeID mSubShapeID1; ///< Sub shape ID that identifies the face on shape 1
			//SubShapeID mSubShapeID2; ///< Sub shape ID that identifies the face on shape 2
			//BodyID mBodyID2; ///< BodyID to which shape 2 belongs to
			//Face mShape1Face; ///< Colliding face on shape 1 (optional result, in world space or relative to base offset)
			//Face mShape2Face; ///< Colliding face on shape 2 (optional result, in world space or relative to base offset)

			//bool mIsBackFaceHit; ///< True if the shape was hit from the back side


			//!!!!
			//if (body->IsSensor())
			//	return;


			uint bodyId = inResult.mBodyID2.GetIndexAndSequenceNumber();

			VolumeTestResultNative result2;
			result2.bodyId = bodyId;
			result2.distanceScale = inResult.mFraction;
			result2.backFaceHit = inResult.mIsBackFaceHit ? 1 : 0;

			if (mode == VolumeTestModeEnum::OneForEach || mode == VolumeTestModeEnum::OneClosestForEach)
			{
				//replace with with same body
				for (int n = 0; n < results.size(); n++)
				{
					if (results[n].bodyId == result2.bodyId)
					{
						if (mode == VolumeTestModeEnum::OneClosestForEach && result2.distanceScale < results[n].distanceScale)
							results[n] = result2;
						return;
					}
				}
			}

			//if (addedBodies.find(bodyId) == addedBodies.end())
			//{

			//addedBodies.insert(bodyId);

			results.push_back(result2);

			if (mode == VolumeTestModeEnum::One)
				ForceEarlyOut();

			//}
		}
	};

	MyCollector collector;
	collector.system = system;
	collector.shapeCast = &shapeCast;
	collector.mode = mode;
	//capacity
	collector.results.reserve(32);

	auto& query = system->system.GetNarrowPhaseQuery();
	//!!!!baseOffset
	query.CastShape(shapeCast, settings, RVec3::sZero(), collector);

	if (collector.results.size() > 1 && mode != VolumeTestModeEnum::OneForEach)
	{
		std::sort(collector.results.begin(), collector.results.end(), SortVolumeTestResultNative);

		if (mode == VolumeTestModeEnum::OneClosest || mode == VolumeTestModeEnum::One)
			collector.results.resize(1);
	}

	//out
	resultCount = collector.results.size();
	if (resultCount > 0)
	{
		results = new VolumeTestResultNative[resultCount];
		for (int n = 0; n < resultCount; n++)
			results[n] = collector.results[n];
	}
	else
		results = nullptr;
}

EXPORT void JPhysicsSystem_VolumeTestSphere(PhysicsSystemItem* system, int mode2, int& resultCount, VolumeTestResultNative*& results, const Vector3D& direction, const Vector3D& position, float radius)
{
	//if (radius < 0)
	//	radius = -radius;
	if (radius < 0.0001f)
		radius = 0.0001f;

	SphereShape shape(radius);
	shape.SetEmbedded();

	auto transform = RMat44::sTranslation(ConvertToRVec3(position));

	VolumeTestCommon(system, mode2, resultCount, results, shape, transform, direction);
}

EXPORT void JPhysicsSystem_VolumeTestBox(PhysicsSystemItem* system, int mode2, int& resultCount, VolumeTestResultNative*& results, const Vector3D& direction, const Vector3D& center, const Vector4& axis, const Vector3& extents)
{
	float convexRadius = 0;//0.01f;
	//JPH_ASSERT(inConvexRadius >= 0.0f);
	//JPH_ASSERT(inHalfExtent.ReduceMin() >= inConvexRadius);

	BoxShape shape(ConvertToVec3(extents), convexRadius);
	shape.SetEmbedded();

	auto transform = RMat44::sRotationTranslation(ConvertToQuat(axis), ConvertToRVec3(center));

	VolumeTestCommon(system, mode2, resultCount, results, shape, transform, direction);
}

EXPORT void JPhysicsSystem_VolumeTestCapsule(PhysicsSystemItem* system, int mode2, int& resultCount, VolumeTestResultNative*& results, const Vector3D& direction, const Vector3D& position, const Vector4& rotation, float height, float radius)
{
	if (height < 0)
		height = 0;
	//if (radius < 0)
	//	radius = -radius;
	if (radius < 0.0001f)
		radius = 0.0001f;

	if (height < 0.0001f)
	{
		SphereShape shape(radius);
		shape.SetEmbedded();
		auto transform = RMat44::sRotationTranslation(ConvertToQuat(rotation), ConvertToRVec3(position));
		VolumeTestCommon(system, mode2, resultCount, results, shape, transform, direction);
	}
	else
	{
		CapsuleShape shape(height * 0.5f, radius);
		//CapsuleShape shape(abs(height) * 0.5f, radius);
		shape.SetEmbedded();
		auto transform = RMat44::sRotationTranslation(ConvertToQuat(rotation), ConvertToRVec3(position));
		VolumeTestCommon(system, mode2, resultCount, results, shape, transform, direction);
	}
}

EXPORT void JPhysicsSystem_VolumeTestCylinder(PhysicsSystemItem* system, int mode2, int& resultCount, VolumeTestResultNative*& results, const Vector3D& direction, const Vector3D& position, const Vector4& rotation, float height, float radius)
{
	if (height < 0.0001f)
		height = 0.0001f;
	if (radius < 0.0001f)
		radius = 0.0001f;
	float halfHeight = height * 0.5f;
	//float halfHeight = abs(height) * 0.5f;

	//!!!!check
	float convexRadius = 0;
	//if (convexRadius > halfHeight)
	//	convexRadius = halfHeight;
	//if (convexRadius > radius)
	//	convexRadius = radius;

	CylinderShape shape(halfHeight, radius, convexRadius);
	shape.SetEmbedded();

	auto transform = RMat44::sRotationTranslation(ConvertToQuat(rotation), ConvertToRVec3(position));

	VolumeTestCommon(system, mode2, resultCount, results, shape, transform, direction);
}


//EXPORT void JPhysicsSystem_VolumeTest(PhysicsSystemItem* system, const RayD& ray, int mode2, int flags2, int& resultCount, VolumeTestResultNative*& results)
//{
//	if (sizeof(VolumeTestResultNative) != 4)
//		Fatal("JPhysicsSystem_VolumeTest: sizeof(VolumeTestResultNative) != 4.");
//
//
//	auto mode = (RayTestModeEnum)mode2;
//	auto flags = (FlagsEnum)flags2;
//
//
//	//!!!!можно искать например только Dynamic
//
//	//!!!!EarlyOutFraction
//
//	//!!!!что-то фильтровать здесь
//
//
//	RayCastSettings settings;
//	//!!!!always need?
//	settings.mBackFaceMode = EBackFaceMode::CollideWithBackFaces;
//	//!!!!
//	//settings.mTreatConvexAsSolid = false;
//
//
//	class MyCollector : public CastRayCollector
//	{
//	public:
//		PhysicsSystemItem* system;
//		RayD ray;
//		RayTestModeEnum mode;
//		FlagsEnum flags;
//		std::vector<RayTestResultNative> results;
//
//		//!!!!?
//		//std::set<uint> addedBodies;
//
//		//
//
//		virtual void AddHit(const RayCastResult& inResult) override
//		{
//			RayTestResultNative result2;
//			result2.bodyId = inResult.mBodyID.GetIndexAndSequenceNumber();
//
//			//!!!!shapeIndex
//			result2.shapeIndex = 0;
//
//			result2.distanceScale = inResult.mFraction;
//			result2.position = ray.getPoint(result2.distanceScale);
//
//
//			//!!!!
//			//SubShapeID mSubShapeID2;						///< Sub shape ID of shape that we collided against
//
//			//!!!!all params
//
//			//!!!!нормаль можно в конце посчитать только для нужных
//			if ((flags & FlagsEnum_CalculateNormal) != 0)
//			{
//				BodyItem* body = system->bodyById.find(result2.bodyId)->second;
//				result2.normal = ConvertToVector3(body->body->GetWorldSpaceSurfaceNormal(inResult.mSubShapeID2, ConvertToRVec3(result2.position)));
//			}
//			else
//				result2.normal = Vector3::ZERO;
//
//			//!!!!
//			result2.triangleIndexSource = 0;
//			result2.triangleIndexProcessed = 0;
//
//
//			if (mode == RayTestModeEnum::OneForEach || mode == RayTestModeEnum::OneClosestForEach)
//			{
//				//replace with with same body
//				for (int n = 0; n < results.size(); n++)
//				{
//					if (results[n].bodyId == result2.bodyId)
//					{
//						if (mode == RayTestModeEnum::OneClosestForEach && result2.distanceScale < results[n].distanceScale)
//							results[n] = result2;
//						return;
//					}
//				}
//			}
//
//			results.push_back(result2);
//
//
//
//			//!!!!
//			//if (body->IsSensor())
//			//	return;
//
//
//			//!!!!
//			//// Test if this collision is closer than the previous one
//			//if (inResult.mFraction < GetEarlyOutFraction())
//			//{
//			//	// Lock the body
//			//	BodyLockRead lock(mPhysicsSystem.GetBodyLockInterfaceNoLock(), inResult.mBodyID);
//			//	JPH_ASSERT(lock.Succeeded()); // When this runs all bodies are locked so this should not fail
//			//	const Body* body = &lock.GetBody();
//
//			//	if (body->IsSensor())
//			//		return;
//
//			//	// Test that we're not hitting a vertical wall
//			//	RVec3 contact_pos = mRay.GetPointOnRay(inResult.mFraction);
//			//	Vec3 normal = body->GetWorldSpaceSurfaceNormal(inResult.mSubShapeID2, contact_pos);
//			//	if (normal.Dot(mUpDirection) > mCosMaxSlopeAngle)
//			//	{
//			//		// Update early out fraction to this hit
//			//		UpdateEarlyOutFraction(inResult.mFraction);
//
//			//		// Get the contact properties
//			//		mBody = body;
//			//		mSubShapeID2 = inResult.mSubShapeID2;
//			//		mContactPosition = contact_pos;
//			//		mContactNormal = normal;
//			//	}
//			//}
//
//			if (mode == RayTestModeEnum::One)
//				ForceEarlyOut();
//		}
//	};
//
//	MyCollector collector;
//	collector.system = system;
//	collector.ray = ray;
//	collector.mode = mode;
//	collector.flags = flags;
//	//capacity
//	collector.results.reserve(32);
//
//	auto& query = system->system.GetNarrowPhaseQuery();
//	query.CastRay(ConvertToRRay(ray), settings, collector);
//
//	if (collector.results.size() > 1 && mode != RayTestModeEnum::OneForEach)
//	{
//		std::sort(collector.results.begin(), collector.results.end(), SortRayTestResultNative);
//
//		if (mode == RayTestModeEnum::OneClosest || mode == RayTestModeEnum::One)
//			collector.results.resize(1);
//	}
//
//	//out
//	resultCount = collector.results.size();
//	if (resultCount > 0)
//	{
//		results = new VolumeTestResultNative[resultCount];
//		for (int n = 0; n < resultCount; n++)
//			results[n] = collector.results[n];
//	}
//	else
//		results = nullptr;
//}

EXPORT void JPhysicsSystem_VolumeTestFree(PhysicsSystemItem* system, VolumeTestResultNative* results)
{
	delete[] results;
}


ValidateResult MyContactListener::OnContactValidate(const Body& inBody1, const Body& inBody2, RVec3Arg inBaseOffset, const CollideShapeResult& inCollisionResult)
{
	BodyItem* body1 = (BodyItem*)inBody1.GetUserData();
	BodyItem* body2 = (BodyItem*)inBody2.GetUserData();

	if (body1 != nullptr && body2 != nullptr)
	{
		for (int n1 = 0; n1 < body1->constraints.size(); n1++)
		{
			auto c1 = body1->constraints[n1];
			for (int n2 = 0; n2 < body2->constraints.size(); n2++)
			{
				auto c2 = body2->constraints[n2];
				if (c1 == c2 && !c1->collisionsBetweenLinkedBodies)
					return ValidateResult::RejectAllContactsForThisBodyPair;
			}
		}
	}

	// Allows you to ignore a contact before it is created (using layers to not make objects collide is cheaper!)
	return ValidateResult::AcceptAllContactsForThisBodyPair;
}

void MyContactListener::AddContact(const Body& inBody1, const Body& inBody2, const ContactManifold& inManifold, ContactSettings& ioSettings)
{
	BodyItem* body1 = (BodyItem*)inBody1.GetUserData();
	BodyItem* body2 = (BodyItem*)inBody2.GetUserData();
	if (body1 == nullptr || body2 == nullptr)
		return;

	if (body1->subscribedToGetContacts)
	{
		ContactItem contact;
		contact.body2ID = inBody2.GetID().GetIndexAndSequenceNumber();
		contact.normal = ConvertToVector3(inManifold.mWorldSpaceNormal);
		contact.penetrationDepth = inManifold.mPenetrationDepth;
		//!!!!
		contact.subShapeID1 = inManifold.mSubShapeID1.GetValue();
		contact.subShapeID2 = inManifold.mSubShapeID2.GetValue();
		contact.contactPointCount = min(inManifold.mRelativeContactPointsOn1.size(), (uint)4);
		for (uint n = 0; n < contact.contactPointCount; n++)
		{
			contact.contactPointsOn1[n] = ConvertToVector3D(inManifold.GetWorldSpaceContactPointOn1(n));
			contact.contactPointsOn2[n] = ConvertToVector3D(inManifold.GetWorldSpaceContactPointOn2(n));
		}

		//!!!!need mutex? // Note that this is called from a job so whatever you do here needs to be thread safe.
		addMutex.lock();
		body1->contacts.push_back(contact);
		body1->systemItem->bodiesContactsToClear.insert(body1);
		addMutex.unlock();
	}

	if (body2->subscribedToGetContacts)
	{
		ContactItem contact;
		contact.body2ID = inBody1.GetID().GetIndexAndSequenceNumber();
		contact.normal = -ConvertToVector3(inManifold.mWorldSpaceNormal);
		contact.penetrationDepth = -inManifold.mPenetrationDepth;
		//!!!!
		contact.subShapeID1 = inManifold.mSubShapeID2.GetValue();
		contact.subShapeID2 = inManifold.mSubShapeID1.GetValue();
		contact.contactPointCount = min(inManifold.mRelativeContactPointsOn1.size(), (uint)4);
		for (uint n = 0; n < contact.contactPointCount; n++)
		{
			contact.contactPointsOn1[n] = ConvertToVector3D(inManifold.GetWorldSpaceContactPointOn2(n));
			contact.contactPointsOn2[n] = ConvertToVector3D(inManifold.GetWorldSpaceContactPointOn1(n));
		}

		//!!!!need mutex? // Note that this is called from a job so whatever you do here needs to be thread safe.
		addMutex.lock();
		body2->contacts.push_back(contact);
		body1->systemItem->bodiesContactsToClear.insert(body2);
		addMutex.unlock();
	}


	//if (body1->subscribedToGetContacts)
	//{
	//	ContactsItem contact;
	//	contact.worldPositionOn1 = ConvertToVector3D(inManifold.GetWorldSpaceContactPointOn1(0));
	//	contact.worldPositionOn2 = ConvertToVector3D(inManifold.GetWorldSpaceContactPointOn2(0));
	//	contact.body2Id = inBody2.GetID().GetIndexAndSequenceNumber();

	//	//!!!!need mutex? // Note that this is called from a job so whatever you do here needs to be thread safe.
	//	addMutex.lock();
	//	body1->contacts.push_back(contact);
	//	body1->systemItem->bodiesContactsToClear.insert(body1);
	//	addMutex.unlock();
	//}

	//if (body2->subscribedToGetContacts)
	//{
	//	ContactsItem contact;
	//	contact.worldPositionOn1 = ConvertToVector3D(inManifold.GetWorldSpaceContactPointOn2(0));
	//	contact.worldPositionOn2 = ConvertToVector3D(inManifold.GetWorldSpaceContactPointOn1(0));
	//	contact.body2Id = inBody1.GetID().GetIndexAndSequenceNumber();

	//	//!!!!need mutex? // Note that this is called from a job so whatever you do here needs to be thread safe.
	//	addMutex.lock();
	//	body2->contacts.push_back(contact);
	//	body1->systemItem->bodiesContactsToClear.insert(body2);
	//	addMutex.unlock();
	//}
}

void MyContactListener::OnContactAdded(const Body& inBody1, const Body& inBody2, const ContactManifold& inManifold, ContactSettings& ioSettings)
{
	AddContact(inBody1, inBody2, inManifold, ioSettings);
}

void MyContactListener::OnContactPersisted(const Body& inBody1, const Body& inBody2, const ContactManifold& inManifold, ContactSettings& ioSettings)
{
	//!!!!check doublicates?

	AddContact(inBody1, inBody2, inManifold, ioSettings);
}

void MyContactListener::OnContactRemoved(const SubShapeIDPair& inSubShapePair)
{
}

EXPORT ConstraintItem* JCreateConstraintSixDOF(PhysicsSystemItem* system, BodyItem* bodyA, BodyItem* bodyB, bool transformInWorldSpace, Vector3D& positionA, Vector3& axisXA, Vector3& axisYA, Vector3D& positionB, Vector3& axisXB, Vector3& axisYB, PhysicsAxisMode linearAxisX, Vector2& linearLimitX, PhysicsAxisMode linearAxisY, Vector2& linearLimitY, PhysicsAxisMode linearAxisZ, Vector2& linearLimitZ, PhysicsAxisMode angularAxisX, Vector2& angularLimitX, PhysicsAxisMode angularAxisY, Vector2& angularLimitY, PhysicsAxisMode angularAxisZ, Vector2& angularLimitZ, float linearXFriction, float linearYFriction, float linearZFriction, float angularXFriction, float angularYFriction, float angularZFriction)
{
	SixDOFConstraintSettings settings;
	settings.SetEmbedded();

	settings.mSpace = transformInWorldSpace ? EConstraintSpace::WorldSpace : EConstraintSpace::LocalToBodyCOM;
	settings.mPosition1 = ConvertToRVec3(positionA);
	settings.mAxisX1 = ConvertToVec3(axisXA);
	settings.mAxisY1 = ConvertToVec3(axisYA);
	settings.mPosition2 = ConvertToRVec3(positionB);
	settings.mAxisX2 = ConvertToVec3(axisXB);
	settings.mAxisY2 = ConvertToVec3(axisYB);

	//settings.mPosition1 = settings.mPosition2 = ConvertToRVec3(position);
	//settings.mAxisX1 = settings.mAxisX2 = ConvertToVec3(axisX);
	//settings.mAxisY1 = settings.mAxisY2 = ConvertToVec3(axisY);


	if (linearAxisX == PhysicsAxisMode::Locked)
		settings.MakeFixedAxis(SixDOFConstraintSettings::TranslationX);
	else if (linearAxisX == PhysicsAxisMode::Limited)
		settings.SetLimitedAxis(SixDOFConstraintSettings::TranslationX, linearLimitX.x, linearLimitX.y);
	else
		settings.MakeFreeAxis(SixDOFConstraintSettings::TranslationX);

	if (linearAxisY == PhysicsAxisMode::Locked)
		settings.MakeFixedAxis(SixDOFConstraintSettings::TranslationY);
	else if (linearAxisY == PhysicsAxisMode::Limited)
		settings.SetLimitedAxis(SixDOFConstraintSettings::TranslationY, linearLimitY.x, linearLimitY.y);
	else
		settings.MakeFreeAxis(SixDOFConstraintSettings::TranslationY);

	if (linearAxisZ == PhysicsAxisMode::Locked)
		settings.MakeFixedAxis(SixDOFConstraintSettings::TranslationZ);
	else if (linearAxisZ == PhysicsAxisMode::Limited)
		settings.SetLimitedAxis(SixDOFConstraintSettings::TranslationZ, linearLimitZ.x, linearLimitZ.y);
	else
		settings.MakeFreeAxis(SixDOFConstraintSettings::TranslationZ);

	if (angularAxisX == PhysicsAxisMode::Locked)
		settings.MakeFixedAxis(SixDOFConstraintSettings::RotationX);
	else if (angularAxisX == PhysicsAxisMode::Limited)
		settings.SetLimitedAxis(SixDOFConstraintSettings::RotationX, angularLimitX.x, angularLimitX.y);
	else
		settings.MakeFreeAxis(SixDOFConstraintSettings::RotationX);

	if (angularAxisY == PhysicsAxisMode::Locked)
		settings.MakeFixedAxis(SixDOFConstraintSettings::RotationY);
	else if (angularAxisY == PhysicsAxisMode::Limited)
		settings.SetLimitedAxis(SixDOFConstraintSettings::RotationY, angularLimitY.x, angularLimitY.y);
	else
		settings.MakeFreeAxis(SixDOFConstraintSettings::RotationY);

	if (angularAxisZ == PhysicsAxisMode::Locked)
		settings.MakeFixedAxis(SixDOFConstraintSettings::RotationZ);
	else if (angularAxisZ == PhysicsAxisMode::Limited)
		settings.SetLimitedAxis(SixDOFConstraintSettings::RotationZ, angularLimitZ.x, angularLimitZ.y);
	else
		settings.MakeFreeAxis(SixDOFConstraintSettings::RotationZ);

	settings.mMaxFriction[0] = linearXFriction;
	settings.mMaxFriction[1] = linearYFriction;
	settings.mMaxFriction[2] = linearZFriction;
	settings.mMaxFriction[3] = angularXFriction;
	settings.mMaxFriction[4] = angularYFriction;
	settings.mMaxFriction[5] = angularZFriction;


	//float floatMax = 10000000000.0f;

	//if (linearLimitX.x < -floatMax && linearLimitX.y > floatMax)
	//	settings.MakeFreeAxis(SixDOFConstraintSettings::TranslationX);
	//else
	//	settings.SetLimitedAxis(SixDOFConstraintSettings::TranslationX, linearLimitX.x, linearLimitX.y);

	//if (linearLimitY.x < -floatMax && linearLimitY.y > floatMax)
	//	settings.MakeFreeAxis(SixDOFConstraintSettings::TranslationY);
	//else
	//	settings.SetLimitedAxis(SixDOFConstraintSettings::TranslationY, linearLimitY.x, linearLimitY.y);

	//if (linearLimitZ.x < -floatMax && linearLimitZ.y > floatMax)
	//	settings.MakeFreeAxis(SixDOFConstraintSettings::TranslationZ);
	//else
	//	settings.SetLimitedAxis(SixDOFConstraintSettings::TranslationZ, linearLimitZ.x, linearLimitZ.y);


	//if (angularLimitX.x < -floatMax && angularLimitX.y > floatMax)
	//	settings.MakeFreeAxis(SixDOFConstraintSettings::RotationX);
	//else
	//	settings.SetLimitedAxis(SixDOFConstraintSettings::RotationX, angularLimitX.x, angularLimitX.y);

	//if (angularLimitY.x < -floatMax && angularLimitY.y > floatMax)
	//	settings.MakeFreeAxis(SixDOFConstraintSettings::RotationY);
	//else
	//	settings.SetLimitedAxis(SixDOFConstraintSettings::RotationY, angularLimitY.x, angularLimitY.y);

	//if (angularLimitZ.x < -floatMax && angularLimitZ.y > floatMax)
	//	settings.MakeFreeAxis(SixDOFConstraintSettings::RotationZ);
	//else
	//	settings.SetLimitedAxis(SixDOFConstraintSettings::RotationZ, angularLimitZ.x, angularLimitZ.y);


	auto constraint = new SixDOFConstraintItem();
	//constraint->type = ConstraintTypeEnum::Fixed;
	constraint->system = &system->system;
	constraint->constraint = settings.Create(*bodyA->body, *bodyB->body);
	constraint->bodyA = bodyA;
	constraint->bodyB = bodyB;
	system->system.AddConstraint(constraint->constraint);
	bodyA->constraints.push_back(constraint);
	bodyB->constraints.push_back(constraint);
	constraint->constraint->SetUserData((uint64_t)constraint);

	return constraint;
}

EXPORT ConstraintItem* JCreateConstraintFixed(PhysicsSystemItem* system, BodyItem* bodyA, BodyItem* bodyB, bool transformInWorldSpace, Vector3D& positionA, Vector3& axisXA, Vector3& axisYA, Vector3D& positionB, Vector3& axisXB, Vector3& axisYB)
//EXPORT ConstraintItem* JCreateConstraintFixed(PhysicsSystemItem* system, BodyItem* bodyA, BodyItem* bodyB, Vector3D& position, Vector3& axisX, Vector3& axisY)
{
	FixedConstraintSettings settings;
	settings.SetEmbedded();

	settings.mSpace = transformInWorldSpace ? EConstraintSpace::WorldSpace : EConstraintSpace::LocalToBodyCOM;
	settings.mPoint1 = ConvertToRVec3(positionA);
	settings.mAxisX1 = ConvertToVec3(axisXA);
	settings.mAxisY1 = ConvertToVec3(axisYA);
	settings.mPoint2 = ConvertToRVec3(positionB);
	settings.mAxisX2 = ConvertToVec3(axisXB);
	settings.mAxisY2 = ConvertToVec3(axisYB);

	//settings.mPoint1 = settings.mPoint2 = ConvertToRVec3(position);
	//settings.mAxisX1 = settings.mAxisX2 = ConvertToVec3(axisX);
	//settings.mAxisY1 = settings.mAxisY2 = ConvertToVec3(axisY);
	////settings.mAutoDetectPoint = true;

	auto constraint = new TwoBodyConstraintItem();
	constraint->system = &system->system;
	constraint->constraint = settings.Create(*bodyA->body, *bodyB->body);
	constraint->bodyA = bodyA;
	constraint->bodyB = bodyB;
	system->system.AddConstraint(constraint->constraint);
	bodyA->constraints.push_back(constraint);
	bodyB->constraints.push_back(constraint);
	constraint->constraint->SetUserData((uint64_t)constraint);
	constraint->collisionsBetweenLinkedBodies = false;

	return constraint;
}

EXPORT VehicleConstraintItem* JCreateConstraintVehicle(PhysicsSystemItem* system, BodyItem* body, int wheelCount, VehicleWheelSettings* wheelsSettings, float frontWheelAntiRollBarStiffness, float rearWheelAntiRollBarStiffness, float maxPitchRollAngle, float engineMaxTorque, float engineMinRPM, float engineMaxRPM, bool transmissionAuto, int transmissionGearRatiosCount, double* transmissionGearRatios, int transmissionReverseGearRatiosCount, double* transmissionReverseGearRatios, float transmissionSwitchTime, float transmissionClutchReleaseTime, float transmissionSwitchLatency, float transmissionShiftUpRPM, float transmissionShiftDownRPM, float transmissionClutchStrength, /*bool frontWheelDrive, bool rearWheelDrive, float frontWheelDifferentialRatio, float frontWheelDifferentialLeftRightSplit, float frontWheelDifferentialLimitedSlipRatio, float frontWheelDifferentialEngineTorqueRatio, float rearWheelDifferentialRatio, float rearWheelDifferentialLeftRightSplit, float rearWheelDifferentialLimitedSlipRatio, float rearWheelDifferentialEngineTorqueRatio,*/ float maxSlopeAngleInRadians, bool tracks, int antiRollbarsCount, float* antiRollbars, float differentialLimitedSlipRatio, int engineNormalizedTorqueCount, float* engineNormalizedTorque, float engineInertia, float engineAngularDamping, int differentialsCount, float* differentials, int trackDrivenWheel, float trackInertia, float trackAngularDamping, float trackMaxBrakeTorque, float trackDifferentialRatio)
{
	bool wheels = !tracks;

	VehicleConstraintSettings settings;
	settings.SetEmbedded();

	//common settings
	settings.mUp = Vec3(0, 0, 1);
	settings.mForward = Vec3(1, 0, 0);
	settings.mMaxPitchRollAngle = maxPitchRollAngle;
	if (abs(settings.mMaxPitchRollAngle - JPH_PI) < 0.001f)
		settings.mMaxPitchRollAngle = JPH_PI;

	//create wheels
	for (int nWheel = 0; nWheel < wheelCount; nWheel++)
	{
		VehicleWheelSettings* wheelSettings = wheelsSettings + nWheel;

		//Wheels mode
		if (wheels)
		{
			WheelSettingsWV* s = new WheelSettingsWV;

			s->mPosition = ConvertToVec3(wheelSettings->position);
			s->mSuspensionDirection = ConvertToVec3(wheelSettings->direction);

			s->mSteeringAxis = Vec3(0, 0, 1);
			s->mWheelForward = Vec3(1, 0, 0);
			s->mWheelUp = Vec3(0, 0, 1);

			s->mSuspensionMinLength = wheelSettings->suspensionMinLength;
			s->mSuspensionMaxLength = wheelSettings->suspensionMaxLength;
			s->mSuspensionPreloadLength = wheelSettings->suspensionPreloadLength;

			//!!!!now also exists StiffnessAndDamping mode

			s->mSuspensionSpring.mFrequency = wheelSettings->suspensionFrequency;
			s->mSuspensionSpring.mDamping = wheelSettings->suspensionDamping;
			s->mRadius = wheelSettings->radius;
			s->mWidth = wheelSettings->width;

			//Wheeled specific
			s->mInertia = wheelSettings->inertia;
			s->mAngularDamping = wheelSettings->angularDamping;
			s->mMaxSteerAngle = wheelSettings->maxSteerAngle;
			s->mLongitudinalFriction.Clear();
			for (int n = 0; n < wheelSettings->LongitudinalFrictionCount; n++)
				s->mLongitudinalFriction.AddPoint(wheelSettings->LongitudinalFrictionData[n * 2 + 0], wheelSettings->LongitudinalFrictionData[n * 2 + 1]);
			s->mLateralFriction.Clear();
			for (int n = 0; n < wheelSettings->LateralFrictionCount; n++)
				s->mLateralFriction.AddPoint(wheelSettings->LateralFrictionData[n * 2 + 0], wheelSettings->LateralFrictionData[n * 2 + 1]);
			s->mMaxBrakeTorque = wheelSettings->maxBrakeTorque;
			s->mMaxHandBrakeTorque = wheelSettings->maxHandBrakeTorque;

			settings.mWheels.push_back(s);
		}
		
		//Tracks mode
		if (tracks)
		{
			WheelSettingsTV* s = new WheelSettingsTV;

			s->mPosition = ConvertToVec3(wheelSettings->position);
			s->mSuspensionDirection = ConvertToVec3(wheelSettings->direction);

			s->mSteeringAxis = Vec3(0, 0, 1);
			s->mWheelForward = Vec3(1, 0, 0);
			s->mWheelUp = Vec3(0, 0, 1);

			s->mSuspensionMinLength = wheelSettings->suspensionMinLength;
			s->mSuspensionMaxLength = wheelSettings->suspensionMaxLength;
			s->mSuspensionPreloadLength = wheelSettings->suspensionPreloadLength;

			//!!!!now also exists StiffnessAndDamping mode

			s->mSuspensionSpring.mFrequency = wheelSettings->suspensionFrequency;
			s->mSuspensionSpring.mDamping = wheelSettings->suspensionDamping;
			s->mRadius = wheelSettings->radius;
			s->mWidth = wheelSettings->width;

			//Tracked specific
			s->mLongitudinalFriction = wheelSettings->trackLongitudinalFriction;
			s->mLateralFriction = wheelSettings->trackLateralFriction;

			settings.mWheels.push_back(s);
		}
	}

	//anti rollbars
	for (int n = 0; n < antiRollbarsCount; n++)
	{
		VehicleAntiRollBar bar;
		bar.mLeftWheel = (int)antiRollbars[n * 3 + 0];
		bar.mRightWheel = (int)antiRollbars[n * 3 + 1];
		bar.mStiffness = antiRollbars[n * 3 + 2];
		settings.mAntiRollBars.push_back(bar);
	}

	//differentials for Wheels mode
	if (wheels)
	{
		VehicleControllerSettings* controllerSettings = new WheeledVehicleControllerSettings();
		settings.mController = controllerSettings;

		WheeledVehicleControllerSettings* controllerSettings2 = (WheeledVehicleControllerSettings*)controllerSettings;

		//differential
		for (int n = 0; n < differentialsCount; n++)
		{
			VehicleDifferentialSettings d;
			d.mLeftWheel = differentials[n * 6 + 0];
			d.mRightWheel = differentials[n * 6 + 1];
			d.mDifferentialRatio = differentials[n * 6 + 2];
			d.mLeftRightSplit = differentials[n * 6 + 3];
			d.mLimitedSlipRatio = differentials[n * 6 + 4];
			d.mEngineTorqueRatio = differentials[n * 6 + 5];
			controllerSettings2->mDifferentials.push_back(d);
		}

		//normalize mEngineTorqueRatio
		if (controllerSettings2->mDifferentials.size() != 0)
		{
			float sum = 0.0f;
			for (int n = 0; n < controllerSettings2->mDifferentials.size(); n++)
				sum += controllerSettings2->mDifferentials[n].mEngineTorqueRatio;
			if (sum <= 0)
				sum = 1;
			for (int n = 0; n < controllerSettings2->mDifferentials.size(); n++)
				controllerSettings2->mDifferentials[n].mEngineTorqueRatio = controllerSettings2->mDifferentials[n].mEngineTorqueRatio / sum;
		}

		controllerSettings2->mDifferentialLimitedSlipRatio = differentialLimitedSlipRatio;
	}

	//tracks for Tracks mode
	if(tracks)
	{
		VehicleControllerSettings* controllerSettings = new TrackedVehicleControllerSettings();
		settings.mController = controllerSettings;

		TrackedVehicleControllerSettings* controllerSettings2 = (TrackedVehicleControllerSettings*)controllerSettings;

		for (int t = 0; t < 2; t++)
		{
			auto& track = controllerSettings2->mTracks[t];

			if (t == 0)
			{
				track.mDrivenWheel = trackDrivenWheel;
				for (int n = 0; n < wheelCount / 2; n++)
					track.mWheels.push_back(n);
			}
			else
			{
				int rightStartIndex = wheelCount / 2;
				track.mDrivenWheel = rightStartIndex + trackDrivenWheel;
				for (int n = 0; n < wheelCount / 2; n++)
					track.mWheels.push_back(rightStartIndex + n);
			}

			track.mInertia = trackInertia;
			track.mAngularDamping = trackAngularDamping;
			track.mMaxBrakeTorque = trackMaxBrakeTorque;
			track.mDifferentialRatio = trackDifferentialRatio;
		}
	}

	//constraint
	auto constraint = new VehicleConstraintItem();
	constraint->system = &system->system;
	constraint->Wheels = wheels;
	constraint->Tracks = tracks;
	VehicleConstraint* vehicleConstraint = new VehicleConstraint(*body->body, settings);
	constraint->constraint = vehicleConstraint;
	constraint->body = body;
	system->system.AddConstraint(constraint->constraint);
	body->constraints.push_back(constraint);
	body->constraintsVehicleAttached = true;
	constraint->constraint->SetUserData((uint64_t)constraint);
	constraint->collisionsBetweenLinkedBodies = false;

	//constraint collision tester
	VehicleCollisionTesterCastCylinder* collisionTester = new VehicleCollisionTesterCastCylinder(Layers::MOVING);
	constraint->collisionTester = collisionTester;
	vehicleConstraint->SetVehicleCollisionTester(constraint->collisionTester);


	////double maxWidth = 0;
	////for (int n = 0; n < wheelCount; n++)
	////{
	////	VehicleWheelSettings* wheelSettings = wheelsSettings + n;
	////	if (wheelSettings->width > maxWidth)
	////		maxWidth = wheelSettings->width;
	////}
	////VehicleCollisionTesterCastSphere* collisionTester = new VehicleCollisionTesterCastSphere(Layers::MOVING, maxWidth * 0.5f, Vec3::sAxisZ(), maxSlopeAngleInRadians);
	////double maxRadius = 0;
	////for (int n = 0; n < wheelCount; n++)
	////{
	////	VehicleWheelSettings* wheelSettings = wheelsSettings + n;
	////	if (wheelSettings->radius > maxRadius)
	////		maxRadius = wheelSettings->radius;
	////}
	////VehicleCollisionTesterCastSphere* collisionTester = new VehicleCollisionTesterCastSphere(Layers::MOVING, maxRadius, Vec3::sAxisZ(), maxSlopeAngleInRadians);
	////VehicleCollisionTesterCastSphereMultiRadius* collisionTester = new VehicleCollisionTesterCastSphereMultiRadius(Layers::MOVING, Vec3::sAxisZ(), maxSlopeAngleInRadians);
	////for (int n = 0; n < wheelCount; n++)
	////{
	////	VehicleWheelSettings* wheelSettings = wheelsSettings + n;
	////	collisionTester->mRadiuses.push_back(wheelSettings->radius);
	////}
	////constraint->collisionTester = new VehicleCollisionTesterRay(Layers::MOVING, Vec3::sAxisZ(), inMaxSlopeAngle);
	////VehicleWheelSettings* wheelSettings = wheelsSettings + 0;
	////constraint->collisionTester = new VehicleCollisionTesterCastSphere(Layers::MOVING, wheelSettings->radius, Vec3::sAxisZ(), inMaxSlopeAngle);


	//engine, transmission

	//Wheels specific
	if (wheels)
	{
		WheeledVehicleController* controller2 = static_cast<WheeledVehicleController*>(vehicleConstraint->GetController());

		//engine
		auto& engine = controller2->GetEngine();
		engine.mMaxTorque = engineMaxTorque;
		engine.mMinRPM = engineMinRPM;
		engine.mMaxRPM = engineMaxRPM;
		engine.mNormalizedTorque.Clear();
		for (int n = 0; n < engineNormalizedTorqueCount; n++)
			engine.mNormalizedTorque.AddPoint(engineNormalizedTorque[n * 2 + 0], engineNormalizedTorque[n * 2 + 1]);
		engine.mInertia = engineInertia;
		engine.mAngularDamping = engineAngularDamping;

		//transmission
		auto& tr = controller2->GetTransmission();
		tr.mMode = transmissionAuto ? ETransmissionMode::Auto : ETransmissionMode::Manual;
		tr.mGearRatios.clear();
		for (int n = 0; n < transmissionGearRatiosCount; n++)
			tr.mGearRatios.push_back((float)transmissionGearRatios[n]);
		tr.mReverseGearRatios.clear();
		for (int n = 0; n < transmissionReverseGearRatiosCount; n++)
			tr.mReverseGearRatios.push_back((float)transmissionReverseGearRatios[n]);
		tr.mSwitchTime = transmissionSwitchTime;
		tr.mClutchReleaseTime = transmissionClutchReleaseTime;
		tr.mSwitchLatency = transmissionSwitchLatency;
		tr.mShiftUpRPM = transmissionShiftUpRPM;
		tr.mShiftDownRPM = transmissionShiftDownRPM;
		tr.mClutchStrength = transmissionClutchStrength;
	}

	//Tracks specific
	if(tracks)
	{
		TrackedVehicleController* controller2 = static_cast<TrackedVehicleController*>(vehicleConstraint->GetController());

		//engine
		auto& engine = controller2->GetEngine();
		engine.mMaxTorque = engineMaxTorque;
		engine.mMinRPM = engineMinRPM;
		engine.mMaxRPM = engineMaxRPM;
		engine.mNormalizedTorque.Clear();
		for (int n = 0; n < engineNormalizedTorqueCount; n++)
			engine.mNormalizedTorque.AddPoint(engineNormalizedTorque[n * 2 + 0], engineNormalizedTorque[n * 2 + 1]);
		engine.mInertia = engineInertia;
		engine.mAngularDamping = engineAngularDamping;

		//transmission
		auto& tr = controller2->GetTransmission();
		tr.mMode = transmissionAuto ? ETransmissionMode::Auto : ETransmissionMode::Manual;
		tr.mGearRatios.clear();
		for (int n = 0; n < transmissionGearRatiosCount; n++)
			tr.mGearRatios.push_back((float)transmissionGearRatios[n]);
		tr.mReverseGearRatios.clear();
		for (int n = 0; n < transmissionReverseGearRatiosCount; n++)
			tr.mReverseGearRatios.push_back((float)transmissionReverseGearRatios[n]);
		tr.mSwitchTime = transmissionSwitchTime;
		tr.mClutchReleaseTime = transmissionClutchReleaseTime;
		tr.mSwitchLatency = transmissionSwitchLatency;
		tr.mShiftUpRPM = transmissionShiftUpRPM;
		tr.mShiftDownRPM = transmissionShiftDownRPM;
		tr.mClutchStrength = transmissionClutchStrength;
	}

	////system->system.AddStepListener(vehicleConstraint);

	return constraint;
}

EXPORT void JDestroyConstraint(PhysicsSystemItem* system, ConstraintItem* constraint)
{
	constraint->OnDestroy();
	constraint->system->RemoveConstraint(constraint->constraint);
	delete constraint;
}

EXPORT void JConstraint_Draw(ConstraintItem* constraint, float drawSize, int& lineCount, DebugDrawLineItem*& lines)
{
	auto c = constraint->constraint;

	//!!!!use one renderer from system. clear before use
	DebugRendererImpl renderer;
	c->SetDrawConstraintSize(drawSize);
	c->DrawConstraint(&renderer);
	c->DrawConstraintLimits(&renderer);
	c->DrawConstraintReferenceFrame(&renderer);

	lineCount = renderer.lines.size();
	lines = new DebugDrawLineItem[lineCount];
	for (int n = 0; n < lineCount; n++)
		lines[n] = renderer.lines[n];
}

EXPORT void JConstraint_DrawFree(ConstraintItem* constraint, DebugDrawLineItem* lines)
{
	delete[] lines;
}

EXPORT void JConstraint_SetCollisionsBetweenLinkedBodies(ConstraintItem* constraint, bool value)
{
	constraint->collisionsBetweenLinkedBodies = value;
}

EXPORT void JConstraint_SetNumVelocityStepsOverride(ConstraintItem* constraint, int value)
{
	constraint->constraint->SetNumVelocityStepsOverride(value);
}

EXPORT void JConstraint_SetNumPositionStepsOverride(ConstraintItem* constraint, int value)
{
	constraint->constraint->SetNumPositionStepsOverride(value);
}

EXPORT void JConstraint_SetSimulate(ConstraintItem* constraint, bool value)
{
	constraint->constraint->SetEnabled(value);
}

EXPORT void JSixDOFConstraint_SetLimit(ConstraintItem* constraint, int axis, float min, float max)
{
	SixDOFConstraint* c = (SixDOFConstraint*)constraint->constraint.GetPtr();
	auto axis2 = (SixDOFConstraint::EAxis)axis;
	c->mLimitMin[axis2] = min;
	c->mLimitMax[axis2] = max;
	if (axis2 >= SixDOFConstraint::EAxis::RotationX)
		c->UpdateRotationLimits();
}

EXPORT void JSixDOFConstraint_SetFriction(ConstraintItem* constraint, int axis, float value)
{
	SixDOFConstraint* c = (SixDOFConstraint*)constraint->constraint.GetPtr();
	auto axis2 = (SixDOFConstraint::EAxis)axis;
	c->SetMaxFriction(axis2, value);
}

EXPORT void JSixDOFConstraint_SetMotor(SixDOFConstraintItem* constraint, int axis, int mode, float frequency, float damping, float limitMin, float limitMax, float target)
{
	SixDOFConstraint* c = (SixDOFConstraint*)constraint->constraint.GetPtr();
	auto axis2 = (SixDOFConstraint::EAxis)axis;
	auto mode2 = (EMotorState)mode;

	auto& s = c->GetMotorSettings(axis2);

	//!!!!now exists StiffnessAndDamping mode

	s.mSpringSettings.mFrequency = frequency;
	s.mSpringSettings.mDamping = damping;

	auto isLinear = axis >= 0 && axis <= 2;

	if (isLinear)
	{
		s.mMinForceLimit = limitMin;
		s.mMaxForceLimit = limitMax;
	}
	else
	{
		s.mMinTorqueLimit = limitMin;
		s.mMaxTorqueLimit = limitMax;
	}

	c->SetMotorState(axis2, mode2);

	if (mode2 == EMotorState::Velocity && isLinear)
	{
		c->mTargetVelocity.SetComponent(axis, target);
		//auto v = c->GetTargetVelocityCS();
		//v.SetComponent(axis, target);
		//c->SetTargetVelocityCS(v);
	}

	if (mode2 == EMotorState::Position && isLinear)
	{
		c->mTargetPosition.SetComponent(axis, target);
		//auto v = c->GetTargetPositionCS();
		//v.SetComponent(axis, target);
		//c->SetTargetPositionCS(v);
	}

	if (mode2 == EMotorState::Velocity && !isLinear)
	{
		if (axis2 == SixDOFConstraint::EAxis::RotationX)
			constraint->angularXVelocity = target;
		else if (axis2 == SixDOFConstraint::EAxis::RotationY)
			constraint->angularYVelocity = target;
		else if (axis2 == SixDOFConstraint::EAxis::RotationZ)
			constraint->angularZVelocity = target;
		//if (axis2 == SixDOFConstraint::EAxis::RotationX)
		//	constraint->angularXVelocity = DegreesToRadians(target);
		//else if (axis2 == SixDOFConstraint::EAxis::RotationY)
		//	constraint->angularYVelocity = DegreesToRadians(target);
		//else if (axis2 == SixDOFConstraint::EAxis::RotationZ)
		//	constraint->angularZVelocity = DegreesToRadians(target);

		auto v = Vec3(constraint->angularXVelocity, constraint->angularYVelocity, constraint->angularZVelocity);
		auto v2 = c->mConstraintToBody1 * (c->mConstraintToBody2.Conjugated() * v);
		c->SetTargetAngularVelocityCS(v2);

		//auto v2 = c->mConstraintToBody2 * (c->mConstraintToBody1.Conjugated() * v);
	}

	if (mode2 == EMotorState::Position && !isLinear)
	{
		if (axis2 == SixDOFConstraint::EAxis::RotationX)
			constraint->angularXPosition = target;
		else if (axis2 == SixDOFConstraint::EAxis::RotationY)
			constraint->angularYPosition = target;
		else if (axis2 == SixDOFConstraint::EAxis::RotationZ)
			constraint->angularZPosition = target;
		//if (axis2 == SixDOFConstraint::EAxis::RotationX)
		//	constraint->angularXPosition = DegreesToRadians(target);
		//else if (axis2 == SixDOFConstraint::EAxis::RotationY)
		//	constraint->angularYPosition = DegreesToRadians(target);
		//else if (axis2 == SixDOFConstraint::EAxis::RotationZ)
		//	constraint->angularZPosition = DegreesToRadians(target);

		c->SetTargetOrientationCS(Quat::sEulerAngles(Vec3(constraint->angularXPosition, constraint->angularYPosition, constraint->angularZPosition)));
	}
}

EXPORT void JVehicleConstraint_SetDriverInput(VehicleConstraintItem* constraint, float forward, float leftTracksOnly, float right, float brake, float handBrake, bool activateBody)
{
	VehicleConstraint* c = (VehicleConstraint*)constraint->constraint.GetPtr();

	if (activateBody && !constraint->body->body->IsActive())
		JBody_Activate(constraint->body);

	if (constraint->Wheels)
	{
		WheeledVehicleController* controller = static_cast<WheeledVehicleController*>(c->GetController());
		controller->SetDriverInput(forward, right, brake, handBrake);
	}

	if (constraint->Tracks)
	{
		TrackedVehicleController* controller = static_cast<TrackedVehicleController*>(c->GetController());
		controller->SetDriverInput(forward, leftTracksOnly, right, brake);
	}


	////constraint->forward = forward;
	//////constraint->left = left;
	////constraint->right = right;
	////constraint->brake = brake;
	////constraint->handBrake = handBrake;

	//VehicleConstraint* c = (VehicleConstraint*)constraint->constraint.GetPtr();
	//WheeledVehicleController* controller = static_cast<WheeledVehicleController*>(c->GetController());

	//if (activateBody && !constraint->body->body->IsActive())
	//	JBody_Activate(constraint->body);

	//// Pass the input on to the constraint
	//controller->SetDriverInput(forward, right, brake, handBrake);

	////// Pass the input on to the constraint
	////controller->SetDriverInput(constraint->forward, constraint->right, constraint->brake, constraint->handBrake);
}

//EXPORT void JVehicleConstraint_SetStepListenerMustBeAdded(VehicleConstraintItem* constraint, bool value)
//{
//	constraint->stepListenerMustBeAdded = value;
//}

EXPORT void JVehicleConstraint_GetData(VehicleConstraintItem* constraint, VehicleWheelData* wheelsData, bool& active, int& currentGear, bool& isSwitchingGear, float& currentRPM)
{
	VehicleConstraint* c = (VehicleConstraint*)constraint->constraint.GetPtr();

	int wheelCount = c->GetWheels().size();
	for (int n = 0; n < wheelCount; n++)
	{
		Wheel* wheel = c->GetWheel(n);

		auto* settings = wheel->GetSettings();
		Vec3 local_wheel_pos = settings->mPosition + settings->mSuspensionDirection * wheel->GetSuspensionLength();
		wheelsData[n].Position = local_wheel_pos.GetZ();
		////slowly
		//wheelsData[n].Position = c->GetWheelLocalTransform(n, Vec3(0, -1, 0), Vec3(0, 0, 1)).GetTranslation().GetZ();

		//wheelsData[n].SuspensionLength = wheel->GetSuspensionLength();
		wheelsData[n].SteerAngle = wheel->GetSteerAngle();
		wheelsData[n].RotationAngle = wheel->GetRotationAngle();
		wheelsData[n].AngularVelocity = wheel->GetAngularVelocity();
		wheelsData[n].ContactBody = (uint)wheel->GetContactBodyID().GetIndexAndSequenceNumber();
	}

	active = c->IsActive();

	if (constraint->Wheels)
	{
		WheeledVehicleController* controller2 = static_cast<WheeledVehicleController*>(c->GetController());

		VehicleTransmission& transmission = controller2->GetTransmission();
		currentGear = transmission.GetCurrentGear();
		isSwitchingGear = transmission.IsSwitchingGear();

		currentRPM = controller2->GetEngine().GetCurrentRPM();
	}

	if (constraint->Tracks)
	{
		TrackedVehicleController* controller2 = static_cast<TrackedVehicleController*>(c->GetController());

		VehicleTransmission& transmission = controller2->GetTransmission();
		currentGear = transmission.GetCurrentGear();
		isSwitchingGear = transmission.IsSwitchingGear();

		currentRPM = controller2->GetEngine().GetCurrentRPM();
	}
}

////for debug
//EXPORT void JVehicleConstraint_GetData2(VehicleConstraintItem* constraint, Vector3D& wheel0, Vector3D& wheel1, Vector3D& wheel2, Vector3D& wheel3)
//{
//	VehicleConstraint* c = (VehicleConstraint*)constraint->constraint.GetPtr();
//
//	int wheelCount = c->GetWheels().size();
//	if (wheelCount == 4)
//	{
//		wheel0 = ConvertToVector3D(c->GetWheelWorldTransform(0, Vec3(0, -1, 0), Vec3(0, 0, 1)).GetTranslation());
//		wheel1 = ConvertToVector3D(c->GetWheelWorldTransform(1, Vec3(0, -1, 0), Vec3(0, 0, 1)).GetTranslation());
//		wheel2 = ConvertToVector3D(c->GetWheelWorldTransform(2, Vec3(0, -1, 0), Vec3(0, 0, 1)).GetTranslation());
//		wheel3 = ConvertToVector3D(c->GetWheelWorldTransform(3, Vec3(0, -1, 0), Vec3(0, 0, 1)).GetTranslation());
//	}
//	else
//	{
//		wheel0 = Vector3D::ZERO;
//		wheel1 = Vector3D::ZERO;
//		wheel2 = Vector3D::ZERO;
//		wheel3 = Vector3D::ZERO;
//	}
//}

//EXPORT void JVehicleConstraint_Update(VehicleConstraintItem* constraint)
//{
//	JBody_Activate(constraint->body);
//	//// On user input, assure that the car is active
//	//if (right != 0.0f || forward != 0.0f || brake != 0.0f || hand_brake != 0.0f)
//	//	mBodyInterface->ActivateBody(mCarBody->GetID());
//
//	VehicleConstraint* c = (VehicleConstraint*)constraint->constraint.GetPtr();
//	WheeledVehicleController* controller = static_cast<WheeledVehicleController*>(c->GetController());
//
//	//// Pass the input on to the constraint
//	//controller->SetDriverInput(constraint->forward, constraint->right, constraint->brake, constraint->handBrake);
//}

void MyBodyActivationListener::OnBodyActivated(const BodyID& inBodyID, std::uint64_t inBodyUserData)
{
	BodyItem* body = (BodyItem*)inBodyUserData;

	//update vehicle constraint of the body
	if( body != nullptr && body->constraintsVehicleAttached)
	{
		for (int n = 0; n < body->constraints.size(); n++)
		{
			ConstraintItem* constraint = body->constraints[n];

			if (constraint->constraint->GetType() == EConstraintType::Vehicle)
			{
				VehicleConstraintItem* vehicleConstraint = (VehicleConstraintItem*)constraint;
				if (!vehicleConstraint->stepListenerAdded)
				{
					system->vehiclesToActivateMutex.lock();
					system->vehiclesToActivate.insert(vehicleConstraint);
					system->vehiclesToActivateMutex.unlock();
				}
			}
		}
	}
}

void MyBodyActivationListener::OnBodyDeactivated(const BodyID& inBodyID, std::uint64_t inBodyUserData)
{
}

void PhysicsStepListenerForNewStepListeners::OnStep(float inDeltaTime, PhysicsSystem& inPhysicsSystem)
{
	for (VehicleConstraintItem* vehicleConstraint : system->vehiclesToActivate)
	{
		VehicleConstraint* vehicleConstraint2 = (VehicleConstraint*)vehicleConstraint->constraint.GetPtr();
		vehicleConstraint2->CallOnStep(inDeltaTime, inPhysicsSystem);
	}
}
