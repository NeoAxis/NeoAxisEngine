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
	//!!!!double

	return RRayCast(ConvertToRVec3(value.getOrigin()), ConvertToVec3(value.getDirection()));
}

RayCast ConvertToRay(const RayD& value)
{
	//!!!!double

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


// Layer that objects can be in, determines which other objects it can collide with
// Typically you at least want to have 1 layer for moving bodies and 1 layer for static bodies, but you can have more
// layers if you want. E.g. you could have a layer for high detail collision (which is not used by the physics simulation
// but only if you do collision testing).
namespace Layers
{
	static constexpr uint8 NON_MOVING = 0;
	static constexpr uint8 MOVING = 1;
	static constexpr uint8 NUM_LAYERS = 2;
};

// Function that determines if two object layers can collide
static bool MyObjectCanCollide(ObjectLayer inObject1, ObjectLayer inObject2)
{
	switch (inObject1)
	{
	case Layers::NON_MOVING:
		return inObject2 == Layers::MOVING; // Non moving only collides with moving
	case Layers::MOVING:
		return true; // Moving collides with everything
	default:
		JPH_ASSERT(false);
		return false;
	}
};

// Each broadphase layer results in a separate bounding volume tree in the broad phase. You at least want to have
// a layer for non-moving and moving objects to avoid having to update a tree full of static objects every frame.
// You can have a 1-on-1 mapping between object layers and broadphase layers (like in this case) but if you have
// many object layers you'll be creating many broad phase trees, which is not efficient. If you want to fine tune
// your broadphase layers define JPH_TRACK_BROADPHASE_STATS and look at the stats reported on the TTY.
namespace BroadPhaseLayers
{
	static constexpr BroadPhaseLayer NON_MOVING(0);
	static constexpr BroadPhaseLayer MOVING(1);
	static constexpr uint NUM_LAYERS(2);
};

// BroadPhaseLayerInterface implementation
// This defines a mapping between object and broadphase layers.
class BPLayerInterfaceImpl final : public BroadPhaseLayerInterface
{
public:
	BPLayerInterfaceImpl()
	{
		// Create a mapping table from object to broad phase layer
		mObjectToBroadPhase[Layers::NON_MOVING] = BroadPhaseLayers::NON_MOVING;
		mObjectToBroadPhase[Layers::MOVING] = BroadPhaseLayers::MOVING;
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
		default:													JPH_ASSERT(false); return "INVALID";
		}
	}
#endif // JPH_EXTERNAL_PROFILE || JPH_PROFILE_ENABLED

private:
	BroadPhaseLayer mObjectToBroadPhase[Layers::NUM_LAYERS];
};

// Function that determines if two broadphase layers can collide
static bool MyBroadPhaseCanCollide(ObjectLayer inLayer1, BroadPhaseLayer inLayer2)
{
	switch (inLayer1)
	{
	case Layers::NON_MOVING:
		return inLayer2 == BroadPhaseLayers::MOVING;
	case Layers::MOVING:
		return true;
	default:
		JPH_ASSERT(false);
		return false;
	}
}

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

//class MyBodyActivationListener : public BodyActivationListener
//{
//public:
//	virtual void OnBodyActivated(const BodyID& inBodyID, uint64 inBodyUserData) override
//	{
//	}
//
//	virtual void OnBodyDeactivated(const BodyID& inBodyID, uint64 inBodyUserData) override
//	{
//	}
//};

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
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct ContactsItem
{
	Vector3D worldPositionOn1;
	Vector3D worldPositionOn2;
	uint body2Id;
	//BodyItem* body2;
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
	//!!!!
	//LinearCurve longitudinalFriction;
	//LinearCurve lateralFriction;
	float maxBrakeTorque;// = 1500.0f;
	float maxHandBrakeTorque;// = 4000.0f;

	//Tracked specific
	//!!!!

};

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
	//MyBodyActivationListener body_activation_listener;
	MyContactListener contactListener;

	//need?
	std::map<uint, BodyItem*> bodyById;

	//DebugRendererImpl debugRenderer;
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class BodyItem
{
public:
	PhysicsSystem* system;
	Body* body;
	BodyID id;

	bool subscribedToGetContacts = false;
	std::vector<ContactsItem> contacts;

	std::vector<ConstraintItem*> constraints;
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class ShapeItem
{
public:
	CompoundShapeSettings* compoundShapeSettings;
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
	bool stepListenerAddedMustBeAdded = false;
	Ref<VehicleCollisionTester> collisionTester;

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
			//!!!!slowly
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
	Color Color;
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
		JPH_OVERRIDE_NEW_DELETE

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
		item.Color = inColor;
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

/// Collision tester that tests collision using a sphere cast
class VehicleCollisionTesterCastSphereMultiRadius : public VehicleCollisionTester
{
public:
	JPH_OVERRIDE_NEW_DELETE

		/// Constructor
		/// @param inObjectLayer Object layer to test collision with
		/// @param inUp World space up vector, used to avoid colliding with vertical walls.
		/// @param inRadius Radius of sphere
		/// @param inMaxSlopeAngle Max angle (rad) that is considered for colliding wheels. This is to avoid colliding with vertical walls.
		VehicleCollisionTesterCastSphereMultiRadius(ObjectLayer inObjectLayer, /*float inRadius, */Vec3Arg inUp = Vec3::sAxisY(), float inMaxSlopeAngle = DegreesToRadians(80.0f)) : mObjectLayer(inObjectLayer), /*mRadius(inRadius), */mUp(inUp), mCosMaxSlopeAngle(Cos(inMaxSlopeAngle)) { }

	// See: VehicleCollisionTester
	virtual bool				Collide(PhysicsSystem& inPhysicsSystem, uint inWheelIndex, RVec3Arg inOrigin, Vec3Arg inDirection, float inSuspensionMaxLength, const BodyID& inVehicleBodyID, Body*& outBody, SubShapeID& outSubShapeID, RVec3& outContactPosition, Vec3& outContactNormal, float& outSuspensionLength) const override;

public:
	std::vector<float> mRadiuses;

private:
	ObjectLayer					mObjectLayer;
	//float						mRadius;
	Vec3						mUp;
	float						mCosMaxSlopeAngle;
};

bool VehicleCollisionTesterCastSphereMultiRadius::Collide(PhysicsSystem& inPhysicsSystem, uint inWheelIndex, RVec3Arg inOrigin, Vec3Arg inDirection, float inSuspensionMaxLength, const BodyID& inVehicleBodyID, Body*& outBody, SubShapeID& outSubShapeID, RVec3& outContactPosition, Vec3& outContactNormal, float& outSuspensionLength) const
{
	DefaultBroadPhaseLayerFilter broadphase_layer_filter = inPhysicsSystem.GetDefaultBroadPhaseLayerFilter(mObjectLayer);
	DefaultObjectLayerFilter object_layer_filter = inPhysicsSystem.GetDefaultLayerFilter(mObjectLayer);
	IgnoreSingleBodyFilter body_filter(inVehicleBodyID);

	auto mRadius = mRadiuses[inWheelIndex];

	SphereShape sphere(mRadius);
	sphere.SetEmbedded();

	float cast_length = max(0.0f, inSuspensionMaxLength - mRadius);
	RShapeCast shape_cast(&sphere, Vec3::sReplicate(1.0f), RMat44::sTranslation(inOrigin), inDirection * cast_length);

	ShapeCastSettings settings;
	settings.mUseShrunkenShapeAndConvexRadius = true;
	settings.mReturnDeepestPoint = true;

	class MyCollector : public CastShapeCollector
	{
	public:
		MyCollector(PhysicsSystem& inPhysicsSystem, const RShapeCast& inShapeCast, Vec3Arg inUpDirection, float inCosMaxSlopeAngle) :
			mPhysicsSystem(inPhysicsSystem),
			mShapeCast(inShapeCast),
			mUpDirection(inUpDirection),
			mCosMaxSlopeAngle(inCosMaxSlopeAngle)
		{
		}

		virtual void		AddHit(const ShapeCastResult& inResult) override
		{
			// Test if this collision is closer than the previous one
			if (inResult.mFraction < GetEarlyOutFraction())
			{
				// Lock the body
				BodyLockRead lock(mPhysicsSystem.GetBodyLockInterfaceNoLock(), inResult.mBodyID2);
				JPH_ASSERT(lock.Succeeded()); // When this runs all bodies are locked so this should not fail
				const Body* body = &lock.GetBody();

				if (body->IsSensor())
					return;

				// Test that we're not hitting a vertical wall
				Vec3 normal = -inResult.mPenetrationAxis.Normalized();
				if (normal.Dot(mUpDirection) > mCosMaxSlopeAngle)
				{
					// Update early out fraction to this hit
					UpdateEarlyOutFraction(inResult.mFraction);

					// Get the contact properties
					mBody = body;
					mSubShapeID2 = inResult.mSubShapeID2;
					mContactPosition = mShapeCast.mCenterOfMassStart.GetTranslation() + inResult.mContactPointOn2;
					mContactNormal = normal;
				}
			}
		}

		// Configuration
		PhysicsSystem& mPhysicsSystem;
		const RShapeCast& mShapeCast;
		Vec3				mUpDirection;
		float				mCosMaxSlopeAngle;

		// Resulting closest collision
		const Body* mBody = nullptr;
		SubShapeID			mSubShapeID2;
		RVec3				mContactPosition;
		Vec3				mContactNormal;
	};

	MyCollector collector(inPhysicsSystem, shape_cast, mUp, mCosMaxSlopeAngle);
	inPhysicsSystem.GetNarrowPhaseQueryNoLock().CastShape(shape_cast, settings, shape_cast.mCenterOfMassStart.GetTranslation(), collector, broadphase_layer_filter, object_layer_filter, body_filter);
	if (collector.mBody == nullptr)
		return false;

	outBody = const_cast<Body*>(collector.mBody);
	outSubShapeID = collector.mSubShapeID2;
	outContactPosition = collector.mContactPosition;
	outContactNormal = collector.mContactNormal;
	outSuspensionLength = min(inSuspensionMaxLength, cast_length * collector.GetEarlyOutFraction() + mRadius);

	return true;
}

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
		Fatal("JPhysicsSystem_RayTest: sizeof(RayTestResultNative) != 56.");
	if (sizeof(VolumeTestResultNative) != 4)
		Fatal("JPhysicsSystem_RayTest: sizeof(RayTestResultNative) != 4.");
	if (sizeof(VehicleWheelSettings) != 18 * 4)
		Fatal("JPhysicsSystem_RayTest: sizeof(VehicleWheelSettings) != 18 * 4.");
	if (sizeof(VehicleWheelData) != 5 * 4)
		Fatal("JPhysicsSystem_RayTest: sizeof(VehicleWheelSettings) != 5 * 4.");

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

	system->system.Init(maxBodies, 0, maxBodyPairs, maxContactConstraints, system->broadPhaseLayerInterface, MyBroadPhaseCanCollide, MyObjectCanCollide);

	//// Note that this is called from a job so whatever you do here needs to be thread safe.
	//system->system.SetBodyActivationListener(&system->body_activation_listener);

	// Note that this is called from a job so whatever you do here needs to be thread safe.
	system->system.SetContactListener(&system->contactListener);
	system->contactListener.system = system;

	return system;
}

EXPORT void JDestroySystem(PhysicsSystemItem* system)
{
	//!!!!no leaks?

	delete system;
}

EXPORT void JPhysicsSystem_SetPhysicsSettings(PhysicsSystemItem* system, /*bool useDefault,*/
	int maxInFlightBodyPairs,
	int stepListenersBatchSize,
	int stepListenerBatchesPerJob,
	float baumgarte,
	float speculativeContactDistance,
	float penetrationSlop,
	float linearCastThreshold,
	float linearCastMaxPenetration,
	float manifoldToleranceSq,
	float maxPenetrationDistance,
	float bodyPairCacheMaxDeltaPositionSq,
	float bodyPairCacheCosMaxDeltaRotationDiv2,
	float contactNormalCosMaxDeltaRotation,
	float contactPointPreserveLambdaMaxDistSq,
	int numVelocitySteps,
	int numPositionSteps,
	float minVelocityForRestitution,
	float timeBeforeSleep,
	float pointVelocitySleepThreshold,
	bool constraintWarmStart,
	bool useBodyPairContactCache,
	bool useManifoldReduction,
	bool allowSleeping,
	bool checkActiveEdges)
{

	PhysicsSettings settings;

	//if (!useDefault)
	//{

	settings.mMaxInFlightBodyPairs = maxInFlightBodyPairs;
	settings.mStepListenersBatchSize = stepListenersBatchSize;
	settings.mStepListenerBatchesPerJob = stepListenerBatchesPerJob;
	settings.mBaumgarte = baumgarte;
	settings.mSpeculativeContactDistance = speculativeContactDistance;
	settings.mPenetrationSlop = penetrationSlop;
	settings.mLinearCastThreshold = linearCastThreshold;
	settings.mLinearCastMaxPenetration = linearCastMaxPenetration;
	settings.mManifoldToleranceSq = manifoldToleranceSq;
	settings.mMaxPenetrationDistance = maxPenetrationDistance;
	settings.mBodyPairCacheMaxDeltaPositionSq = bodyPairCacheMaxDeltaPositionSq;
	settings.mBodyPairCacheCosMaxDeltaRotationDiv2 = bodyPairCacheCosMaxDeltaRotationDiv2;
	settings.mContactNormalCosMaxDeltaRotation = contactNormalCosMaxDeltaRotation;
	settings.mContactPointPreserveLambdaMaxDistSq = contactPointPreserveLambdaMaxDistSq;
	settings.mNumVelocitySteps = numVelocitySteps;
	settings.mNumPositionSteps = numPositionSteps;
	settings.mMinVelocityForRestitution = minVelocityForRestitution;
	settings.mTimeBeforeSleep = timeBeforeSleep;
	settings.mPointVelocitySleepThreshold = pointVelocitySleepThreshold;
	settings.mConstraintWarmStart = constraintWarmStart;
	settings.mUseBodyPairContactCache = useBodyPairContactCache;
	settings.mUseManifoldReduction = useManifoldReduction;
	settings.mAllowSleeping = allowSleeping;
	settings.mCheckActiveEdges = checkActiveEdges;

	//}

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

EXPORT void JPhysicsSystem_Update(PhysicsSystemItem* system, float deltaTime, int collisionSteps, int integrationSubSteps)
{
	//!!!!slowly?
	for (std::map<uint, BodyItem*>::const_iterator it = system->bodyById.begin(); it != system->bodyById.end(); ++it)
		it->second->contacts.clear();

	if (integrationSubSteps > PhysicsUpdateContext::cMaxSubSteps)
		integrationSubSteps = PhysicsUpdateContext::cMaxSubSteps;

	// We need a temp allocator for temporary allocations during the physics update. We're
	// pre-allocating 10 MB to avoid having to do allocations during the physics update. 
	// B.t.w. 10 MB is way too much for this example but it is a typical value you can use.
	// If you don't want to pre-allocate you can also use TempAllocatorMalloc to fall back to
	// malloc / free.

	////max size
	//TempAllocatorImpl temp_allocator(10 * 1024 * 1024);

	auto& constraints = system->system.GetConstraintsNoLock();
	for (int n = 0; n < constraints.size(); n++)
	{
		Constraint* c = constraints[n];
		if (c->GetType() == EConstraintType::Vehicle)
		{
			VehicleConstraint* c2 = (VehicleConstraint*)c;
			VehicleConstraintItem* item = (VehicleConstraintItem*)c2->mAnyData;

			if (c2->GetVehicleBody()->IsActive() || item->stepListenerAddedMustBeAdded)
			{
				if (!item->stepListenerAdded)
				{
					system->system.AddStepListener(c2);
					item->stepListenerAdded = true;
				}
			}
			else
			{
				if (item->stepListenerAdded)
				{
					//!!!!slowly
					system->system.RemoveStepListener(c2);
					item->stepListenerAdded = false;
				}
			}
		}
	}

	system->system.Update(deltaTime, collisionSteps, integrationSubSteps, &tempAllocator, &job_system);
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

EXPORT BodyItem* JCreateBody(PhysicsSystemItem* system, ShapeItem* shape, int motionType, float linearDamping, float angularDamping, const Vector3D& position, const Vector4& rotation, bool activate, float mass, bool centerOfMassManual, const Vector3& centerOfMassPosition, const Vector3& inertiaTensorFactor, uint& resultBodyId, int motionQuality)
{
	if (linearDamping < 0)
		linearDamping = 0;
	if (angularDamping < 0)
		angularDamping = 0;
	if (mass < 0)
		mass = 0;

	BodyInterface& bodyInterface = system->system.GetBodyInterfaceNoLock();

	auto motionType2 = (EMotionType)motionType;

	//!!!!?
	ObjectLayer objectLayer = (motionType2 != EMotionType::Static) ? Layers::MOVING : Layers::NON_MOVING;

	BodyCreationSettings settings(shape->compoundShapeSettings, ConvertToRVec3(position), ConvertToQuat(rotation), motionType2, objectLayer);
	//BodyCreationSettings settings(shape->compoundShapeSettings->Create().Get(), Convert(position), rotation, motionType2, objectLayer);

	settings.mMotionQuality = (EMotionQuality)motionQuality;

	//!!!!centerOfMassManual, const Vector3& centerOfMassPosition, const Vector3& inertiaTensorFactor

	settings.mOverrideMassProperties = EOverrideMassProperties::CalculateInertia;// MassAndInertiaProvided;//CalculateInertia;
	settings.mInertiaMultiplier = 1.0f;
	settings.mMassPropertiesOverride.mMass = mass;

	//settings.mMotionType = (EMotionType)motionType;
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
	body->body = bodyInterface.CreateBody(settings);
	if (body->body == nullptr)
	{
		//max count limit
		return nullptr;
	}
	body->id = body->body->GetID();
	body->body->SetUserData((JPH::uint64)body);

	bodyInterface.AddBody(body->id, activate ? EActivation::Activate : EActivation::DontActivate);

	resultBodyId = (uint)body->id.GetIndexAndSequenceNumber();

	system->bodyById[resultBodyId] = body;
	//system->bodyById.insert(std::pair<uint, BodyItem*>(resultBodyId, body));

	return body;
}

EXPORT void JDestroyBody(PhysicsSystemItem* system, BodyItem* body)
{
	system->bodyById.erase(body->id.GetIndexAndSequenceNumber());

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

	//!!!!double
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
	BodyInterface& bodyInterface = body->system->GetBodyInterfaceNoLock();
	bodyInterface.ActivateBody(body->id);
}

EXPORT void JBody_Deactivate(BodyItem* body)
{
	BodyInterface& bodyInterface = body->system->GetBodyInterfaceNoLock();
	bodyInterface.DeactivateBody(body->id);
}

EXPORT void JBody_SetLinearVelocity(BodyItem* body, const Vector3& value)
{
	auto body2 = body->body;
	if (!body2->IsStatic())
		body2->SetLinearVelocityClamped(ConvertToVec3(value));
}

EXPORT void JBody_SetAngularVelocity(BodyItem* body, const Vector3& value)
{
	auto body2 = body->body;
	if (!body2->IsStatic())
		body2->SetAngularVelocityClamped(ConvertToVec3(value));
}

EXPORT void JBody_SetFriction(BodyItem* body, float value)
{
	if (value < 0)
		value = 0;
	body->body->SetFriction(value);
}

EXPORT void JBody_SetRestitution(BodyItem* body, float value)
{
	if (value < 0)
		value = 0;
	if (value > 1)
		value = 1;
	body->body->SetRestitution(value);
}

EXPORT void JBody_SetLinearDamping(BodyItem* body, float value)
{
	if (value < 0)
		value = 0;
	auto body2 = body->body;
	if (!body2->IsStatic() && body2->GetMotionProperties() != nullptr)
		body2->GetMotionProperties()->SetLinearDamping(value);
}

EXPORT void JBody_SetAngularDamping(BodyItem* body, float value)
{
	if (value < 0)
		value = 0;
	auto body2 = body->body;
	if (!body2->IsStatic() && body2->GetMotionProperties() != nullptr)
		body2->GetMotionProperties()->SetAngularDamping(value);
}

EXPORT void JBody_SetGravityFactor(BodyItem* body, float value)
{
	auto body2 = body->body;
	if (!body2->IsStatic() && body2->GetMotionProperties() != nullptr)
		body2->GetMotionProperties()->SetGravityFactor(value);
}

//EXPORT void JBody_SetShape(BodyItem* body, )
//{
//}

EXPORT void JBody_SetTransform(BodyItem* body, const Vector3D& position, const Vector4& rotation, bool activate)
{
	BodyInterface& bodyInterface = body->system->GetBodyInterfaceNoLock();
	bodyInterface.SetPositionAndRotation(body->id, ConvertToRVec3(position), ConvertToQuat(rotation), activate ? EActivation::Activate : EActivation::DontActivate);
}

EXPORT void JBody_ApplyForce(BodyItem* body, const Vector3& force, const Vector3& relativePosition)
{
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

EXPORT void JBody_GetContacts(BodyItem* body, int& count, ContactsItem*& buffer)
{
	body->subscribedToGetContacts = true;

	count = body->contacts.size();
	if (count != 0)
		buffer = &body->contacts[0];
	else
		buffer = nullptr;
}

EXPORT ShapeItem* JCreateShape(bool mutableCompoundShape)
{
	auto item = new ShapeItem();

	if (mutableCompoundShape)
		item->compoundShapeSettings = new MutableCompoundShapeSettings();
	else
		item->compoundShapeSettings = new StaticCompoundShapeSettings();

	item->compoundShapeSettings->SetEmbedded();

	return item;
}

EXPORT void JDestroyShape(ShapeItem* shape)
{
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
					result2.normal = Vector3::ZERO;
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
		ShapeSettings::ShapeResult shapeResult = testShape->compoundShapeSettings->Create();

		//!!!!
		SubShapeIDCreator subShapeIDCreator;

		shapeResult.Get()->CastRay(ConvertToRay(ray), settings, subShapeIDCreator, collector);

		//virtual void CastRay(const SubShapeIDCreator& inSubShapeIDCreator, CastRayCollector& ioCollector, const ShapeFilter& inShapeFilter = { }) const override;

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
	All
};

enum VolumeTestFlagsEnum
{
	VolumeTestFlagsEnum_None = 0,
};

void VolumeTestCommon(PhysicsSystemItem* system, int mode2, int& resultCount, VolumeTestResultNative*& results, Shape& shape, const RMat44& transform, const Vector3D& direction)
{
	if (sizeof(VolumeTestResultNative) != 4)
		Fatal("JPhysicsSystem_VolumeTest: sizeof(VolumeTestResultNative) != 4.");

	auto mode = (VolumeTestModeEnum)mode2;


	//!!!!scale?

	Vector3D direction2 = direction;
	//!!!!good?
	if (direction2 == Vector3D::ZERO)
		direction2 = Vector3D(0, 0, 0.001);

	RShapeCast shapeCast(&shape, Vec3::sReplicate(1.0f), transform, ConvertToVec3(direction2));

	ShapeCastSettings settings;
	//!!!!optionally?
	settings.mBackFaceModeTriangles = EBackFaceMode::CollideWithBackFaces;
	settings.mBackFaceModeConvex = EBackFaceMode::CollideWithBackFaces;
	//!!!!what else?


	class MyCollector : public CastShapeCollector
	{
	public:
		PhysicsSystemItem* system;
		RShapeCast* shapeCast;
		VolumeTestModeEnum mode;
		std::vector<VolumeTestResultNative> results;
		std::set<uint> addedBodies;

		//

		virtual void AddHit(const ShapeCastResult& inResult) override
		{
			//!!!!more data


			//!!!!mFraction полезен когда sweep


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

			if (addedBodies.find(bodyId) == addedBodies.end())
			{
				addedBodies.insert(bodyId);

				VolumeTestResultNative result2;
				result2.bodyId = bodyId;
				results.push_back(result2);

				if (mode == VolumeTestModeEnum::One)
					ForceEarlyOut();
			}
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

	if (collector.results.size() > 1 && mode == VolumeTestModeEnum::One)
		collector.results.resize(1);

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
		ContactsItem contact;
		contact.worldPositionOn1 = ConvertToVector3D(inManifold.GetWorldSpaceContactPointOn1(0));
		contact.worldPositionOn2 = ConvertToVector3D(inManifold.GetWorldSpaceContactPointOn2(0));
		contact.body2Id = inBody2.GetID().GetIndexAndSequenceNumber();

		//!!!!need mutex? // Note that this is called from a job so whatever you do here needs to be thread safe.
		addMutex.lock();
		body1->contacts.push_back(contact);
		addMutex.unlock();
	}

	if (body2->subscribedToGetContacts)
	{
		ContactsItem contact;
		contact.worldPositionOn1 = ConvertToVector3D(inManifold.GetWorldSpaceContactPointOn2(0));
		contact.worldPositionOn2 = ConvertToVector3D(inManifold.GetWorldSpaceContactPointOn1(0));
		contact.body2Id = inBody1.GetID().GetIndexAndSequenceNumber();

		//!!!!need mutex? // Note that this is called from a job so whatever you do here needs to be thread safe.
		addMutex.lock();
		body2->contacts.push_back(contact);
		addMutex.unlock();
	}


	//if (system->bodiesSubscribedToGetContacts.find(inBody1.GetID().GetIndexAndSequenceNumber()) != system->bodiesSubscribedToGetContacts.end())
	//{
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
	constraint->constraint->mAnyData = constraint;

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
	constraint->constraint->mAnyData = constraint;
	constraint->collisionsBetweenLinkedBodies = false;

	return constraint;
}

EXPORT VehicleConstraintItem* JCreateConstraintVehicle(PhysicsSystemItem* system, BodyItem* body, int wheelCount, VehicleWheelSettings* wheelsSettings, float frontWheelAntiRollBarStiffness, float rearWheelAntiRollBarStiffness, float maxPitchRollAngle, float engineMaxTorque, float engineMinRPM, float engineMaxRPM, bool transmissionAuto, int transmissionGearRatiosCount, double* transmissionGearRatios, int transmissionReverseGearRatiosCount, double* transmissionReverseGearRatios, float transmissionSwitchTime, float transmissionClutchReleaseTime, float transmissionSwitchLatency, float transmissionShiftUpRPM, float transmissionShiftDownRPM, float transmissionClutchStrength, bool frontWheelDrive, bool rearWheelDrive, float frontWheelDifferentialRatio, float frontWheelDifferentialLeftRightSplit, float frontWheelDifferentialLimitedSlipRatio, float frontWheelDifferentialEngineTorqueRatio, float rearWheelDifferentialRatio, float rearWheelDifferentialLeftRightSplit, float rearWheelDifferentialLimitedSlipRatio, float rearWheelDifferentialEngineTorqueRatio, float maxSlopeAngleInRadians)
{
	VehicleConstraintSettings settings;
	settings.SetEmbedded();

	settings.mUp = Vec3(0, 0, 1);
	settings.mForward = Vec3(1, 0, 0);
	settings.mMaxPitchRollAngle = maxPitchRollAngle;
	if (abs(settings.mMaxPitchRollAngle - JPH_PI) < 0.001f)
		settings.mMaxPitchRollAngle = JPH_PI;

	for (int n = 0; n < wheelCount; n++)
	{
		VehicleWheelSettings* wheelSettings = wheelsSettings + n;

		//!!!!удаляется?
		WheelSettingsWV* s = new WheelSettingsWV;

		s->mPosition = ConvertToVec3(wheelSettings->position);
		s->mDirection = ConvertToVec3(wheelSettings->direction);
		s->mSuspensionMinLength = wheelSettings->suspensionMinLength;
		s->mSuspensionMaxLength = wheelSettings->suspensionMaxLength;
		s->mSuspensionPreloadLength = wheelSettings->suspensionPreloadLength;
		s->mSuspensionFrequency = wheelSettings->suspensionFrequency;
		s->mSuspensionDamping = wheelSettings->suspensionDamping;
		s->mRadius = wheelSettings->radius;
		s->mWidth = wheelSettings->width;

		//Wheeled specific
		s->mInertia = wheelSettings->inertia;
		s->mAngularDamping = wheelSettings->angularDamping;
		s->mMaxSteerAngle = wheelSettings->maxSteerAngle;
		//!!!!
		//LinearCurve mLongitudinalFriction;
		//LinearCurve mLateralFriction;
		s->mMaxBrakeTorque = wheelSettings->maxBrakeTorque;
		s->mMaxHandBrakeTorque = wheelSettings->maxHandBrakeTorque;

		settings.mWheels.push_back(s);
	}

	// Anti rollbars
	if (frontWheelAntiRollBarStiffness != 0)
	{
		VehicleAntiRollBar bar;
		bar.mLeftWheel = 0;
		bar.mRightWheel = 1;
		bar.mStiffness = frontWheelAntiRollBarStiffness;
		settings.mAntiRollBars.push_back(bar);
	}
	if (rearWheelAntiRollBarStiffness != 0)
	{
		VehicleAntiRollBar bar;
		bar.mLeftWheel = 2;
		bar.mRightWheel = 3;
		bar.mStiffness = rearWheelAntiRollBarStiffness;
		settings.mAntiRollBars.push_back(bar);
	}
	//if (sAntiRollbar)
	//{
	//	settings.mAntiRollBars.resize(2);
	//	settings.mAntiRollBars[0].mLeftWheel = 0;
	//	settings.mAntiRollBars[0].mRightWheel = 1;
	//	settings.mAntiRollBars[1].mLeftWheel = 2;
	//	settings.mAntiRollBars[1].mRightWheel = 3;
	//}

	//!!!!delete?
	WheeledVehicleControllerSettings* controller = new WheeledVehicleControllerSettings();
	settings.mController = controller;

	// Differential
	if (frontWheelDrive)
	{
		VehicleDifferentialSettings d;
		d.mLeftWheel = 0;
		d.mRightWheel = 1;
		d.mDifferentialRatio = frontWheelDifferentialRatio;
		d.mLeftRightSplit = frontWheelDifferentialLeftRightSplit;
		d.mLimitedSlipRatio = frontWheelDifferentialLimitedSlipRatio;
		d.mEngineTorqueRatio = frontWheelDifferentialEngineTorqueRatio;
		controller->mDifferentials.push_back(d);
	}

	if (rearWheelDrive)
	{
		VehicleDifferentialSettings d;
		d.mLeftWheel = 2;
		d.mRightWheel = 3;
		d.mDifferentialRatio = rearWheelDifferentialRatio;
		d.mLeftRightSplit = rearWheelDifferentialLeftRightSplit;
		d.mLimitedSlipRatio = rearWheelDifferentialLimitedSlipRatio;
		d.mEngineTorqueRatio = rearWheelDifferentialEngineTorqueRatio;
		controller->mDifferentials.push_back(d);
	}

	//normalize mEngineTorqueRatio
	if (controller->mDifferentials.size() != 0)
	{
		float sum = 0.0f;
		for (int n = 0; n < controller->mDifferentials.size(); n++)
			sum += controller->mDifferentials[n].mEngineTorqueRatio;
		if (sum <= 0)
			sum = 1;
		for (int n = 0; n < controller->mDifferentials.size(); n++)
			controller->mDifferentials[n].mEngineTorqueRatio = controller->mDifferentials[n].mEngineTorqueRatio / sum;
	}
	//if (frontWheelDrive && rearWheelDrive)
	//{
	//	// Split engine torque
	//	controller->mDifferentials[0].mEngineTorqueRatio = controller->mDifferentials[1].mEngineTorqueRatio = 0.5f;
	//}

	auto constraint = new VehicleConstraintItem();
	constraint->system = &system->system;
	//!!!!удаляется?
	VehicleConstraint* vehicleConstraint = new VehicleConstraint(*body->body, settings);
	constraint->constraint = vehicleConstraint;
	constraint->body = body;
	system->system.AddConstraint(constraint->constraint);
	body->constraints.push_back(constraint);
	constraint->constraint->mAnyData = constraint;
	constraint->collisionsBetweenLinkedBodies = false;

	//collision tester

	//!!!!ray tester optionally?
	
	VehicleCollisionTesterCastSphereMultiRadius* collisionTester = new VehicleCollisionTesterCastSphereMultiRadius(Layers::MOVING, Vec3::sAxisZ(), maxSlopeAngleInRadians);
	for (int n = 0; n < wheelCount; n++)
	{
		VehicleWheelSettings* wheelSettings = wheelsSettings + n;
		collisionTester->mRadiuses.push_back(wheelSettings->radius);
	}
	constraint->collisionTester = collisionTester;

	//constraint->collisionTester = new VehicleCollisionTesterRay(Layers::MOVING, Vec3::sAxisZ(), inMaxSlopeAngle);
	//VehicleWheelSettings* wheelSettings = wheelsSettings + 0;
	//constraint->collisionTester = new VehicleCollisionTesterCastSphere(Layers::MOVING, wheelSettings->radius, Vec3::sAxisZ(), inMaxSlopeAngle);

	vehicleConstraint->SetVehicleCollisionTester(constraint->collisionTester);

	//engine
	WheeledVehicleController* controller2 = static_cast<WheeledVehicleController*>(vehicleConstraint->GetController());
	controller2->GetEngine().mMaxTorque = engineMaxTorque;
	controller2->GetEngine().mMinRPM = engineMinRPM;
	controller2->GetEngine().mMaxRPM = engineMaxRPM;

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
	tr.mShiftDownRPM = transmissionShiftDownRPM;

	////!!!!можно выключать когда неактивный
	//system->system.AddStepListener(vehicleConstraint);

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
	s.mFrequency = frequency;
	s.mDamping = damping;

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
			constraint->angularXVelocity = DegreesToRadians(target);
		else if (axis2 == SixDOFConstraint::EAxis::RotationY)
			constraint->angularYVelocity = DegreesToRadians(target);
		else if (axis2 == SixDOFConstraint::EAxis::RotationZ)
			constraint->angularZVelocity = DegreesToRadians(target);

		auto v = Vec3(constraint->angularXVelocity, constraint->angularYVelocity, constraint->angularZVelocity);
		auto v2 = c->mConstraintToBody1 * (c->mConstraintToBody2.Conjugated() * v);
		c->SetTargetAngularVelocityCS(v2);

		//auto v2 = c->mConstraintToBody2 * (c->mConstraintToBody1.Conjugated() * v);
	}

	if (mode2 == EMotorState::Position && !isLinear)
	{
		if (axis2 == SixDOFConstraint::EAxis::RotationX)
			constraint->angularXPosition = DegreesToRadians(target);
		else if (axis2 == SixDOFConstraint::EAxis::RotationY)
			constraint->angularYPosition = DegreesToRadians(target);
		else if (axis2 == SixDOFConstraint::EAxis::RotationZ)
			constraint->angularZPosition = DegreesToRadians(target);

		c->SetTargetOrientationCS(Quat::sEulerAngles(Vec3(constraint->angularXPosition, constraint->angularYPosition, constraint->angularZPosition)));
	}
}

EXPORT void JVehicleConstraint_SetDriverInput(VehicleConstraintItem* constraint, float forward, /*float left, */float right, float brake, float handBrake, bool activateBody)
{
	//constraint->forward = forward;
	////constraint->left = left;
	//constraint->right = right;
	//constraint->brake = brake;
	//constraint->handBrake = handBrake;

	VehicleConstraint* c = (VehicleConstraint*)constraint->constraint.GetPtr();
	WheeledVehicleController* controller = static_cast<WheeledVehicleController*>(c->GetController());

	if (activateBody && !constraint->body->body->IsActive())
		JBody_Activate(constraint->body);

	// Pass the input on to the constraint
	controller->SetDriverInput(forward, right, brake, handBrake);

	//// Pass the input on to the constraint
	//controller->SetDriverInput(constraint->forward, constraint->right, constraint->brake, constraint->handBrake);
}

EXPORT void JVehicleConstraint_SetStepListenerAddedMustBeAdded(VehicleConstraintItem* constraint, bool value)
{
	constraint->stepListenerAddedMustBeAdded = value;
}

EXPORT void JVehicleConstraint_GetData(VehicleConstraintItem* constraint, VehicleWheelData* wheelsData, bool& active)
{
	VehicleConstraint* c = (VehicleConstraint*)constraint->constraint.GetPtr();

	int wheelCount = c->GetWheels().size();
	for (int n = 0; n < wheelCount; n++)
	{
		Wheel* wheel = c->GetWheel(n);

		auto* settings = wheel->GetSettings();
		Vec3 local_wheel_pos = settings->mPosition + settings->mDirection * wheel->GetSuspensionLength();
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
