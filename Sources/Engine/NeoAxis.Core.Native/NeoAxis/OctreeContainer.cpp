// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "NeoAxisCoreNative.h"
#include "OctreeContainer.h"
#include "MaskedOcclusionCulling.h"
#include <mutex>
#include <thread>
#include <atomic>
#include <memory>

using namespace Ogre;

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class OctreeContainer;

int GetObjectsRaySortCompare( const void* a, const void* b );

enum class ThreadingModeEnum
{
	SingleThreaded,
	BackgroundThread,
	//MultiBackgroundThreads,
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//void GetBoundsGeometryVertices(const BoundsD& bounds, Vector3* positions)
//{
//	positions[0] = Vector3(bounds.maximum.x, bounds.minimum.y, bounds.minimum.z);
//	positions[1] = Vector3(bounds.maximum.x, bounds.minimum.y, bounds.maximum.z);
//	positions[2] = Vector3(bounds.maximum.x, bounds.maximum.y, bounds.minimum.z);
//	positions[3] = Vector3(bounds.maximum.x, bounds.maximum.y, bounds.maximum.z);
//	positions[4] = Vector3(bounds.minimum.x, bounds.minimum.y, bounds.minimum.z);
//	positions[5] = Vector3(bounds.minimum.x, bounds.minimum.y, bounds.maximum.z);
//	positions[6] = Vector3(bounds.minimum.x, bounds.maximum.y, bounds.minimum.z);
//	positions[7] = Vector3(bounds.minimum.x, bounds.maximum.y, bounds.maximum.z);
//}

static const unsigned int boundsGeometryIndices[36] = {
	0, 3, 1,
	0, 2, 3,
	3, 6, 7,
	3, 2, 6,
	1, 7, 5,
	1, 3, 7,
	4, 7, 6,
	4, 5, 7,
	1, 4, 0,
	5, 4, 1,
	4, 2, 0,
	4, 6, 2 };

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class OctreeContainer
{
public:

	class Node;

	/////////////////////////////////////////////

	class ObjectData
	{
	public:
		int index;
		enum Flags
		{
			Flags_Free = 1 << 0,
			Flags_InsideOctreeBounds = 1 << 1,
			Flags_OutsideOctreeBounds = 1 << 2,
			//Flags_AddedToWorld = 1 << 3,
		};
		int/*Flags*/ flags;
		uint groupMask;
		BoundsD bounds;
		BoundsI nodeBoundsIndexes;
		Vector3D boundsCenter;
		Vector3D boundsHalfSize;

		//inside octree data

		//key for Node::bigObjects
		struct NodeBigObjectsListIteratorItem
		{
			Node* node;
			//bool fullyContains;
			std::list<ObjectData*>::iterator listIterator;
		};
		std::vector<NodeBigObjectsListIteratorItem> nodeBigObjectsListIterators;

		//key for Node::objectsIntersectsNodeBounds
		struct NodeObjectsIntersectsNodeBoundsListIteratorItem
		{
			Node* node;
			std::list<ObjectData*>::iterator listIterator;
		};
		std::vector<NodeObjectsIntersectsNodeBoundsListIteratorItem> nodeObjectsIntersectsNodeBoundsListIterators;

		//outside octree data
		std::list<ObjectData*>::iterator objectsOutsideOctreeListIterator;

		//bool getObjectsChecked;

		ObjectData()
		{
			flags = 0;
			groupMask = 0;
			bounds.setCleared();
			//getObjectsChecked = false;
			nodeBoundsIndexes.setCleared();
		}

		~ObjectData()
		{
			if( nodeBigObjectsListIterators.size() != 0 )
				Fatal( "OctreeContainer: Node: nodeBigObjectsListIterators.size() != 0." );
			if( nodeObjectsIntersectsNodeBoundsListIterators.size() != 0 )
				Fatal( "OctreeContainer: Node: nodeObjectsIntersectsNodeBoundsListIterators.size() != 0." );
		}
	};

	/////////////////////////////////////////////

	class Node
	{
	public:
		OctreeContainer* owner;
		Node* parent;

		int childrenCount;
		BoundsI childrenIndexBounds[ 8 ];
		BoundsD childrenBounds[ 8 ];
		bool needCreateChildrenNodes;
		Node* children[ 8 ];

		BoundsI indexBounds;
		bool endNode;
		BoundsD bounds;
		Vector3D boundsCenter;
		Vector3D boundsHalfSize;

		//list of the objects which is fully contains the bounds of the node.
		std::list<ObjectData*> bigObjects;

		//list of the objects which intersects the bounds of the node. if children nodes are created, this list will empty. 
		//The objects will placed in children nodes.
		std::list<ObjectData*> objectsIntersectsNodeBounds;

		bool needCheckForDeletion;

		Node( OctreeContainer* owner, Node* parent, const BoundsI& indexBounds );
		~Node();
	};


	/////////////////////////////////////////////

	struct DebugRenderLine
	{
		Vector3D start;
		Vector3D end;
		ColourValue color;

		DebugRenderLine( const Vector3D& start, const Vector3D& end, const ColourValue& color )
		{
			this->start = start;
			this->end = end;
			this->color = color;
		}
	};

	/////////////////////////////////////////////

	enum GetObjectsTypes
	{
		GetObjectsTypes_Bounds,
		GetObjectsTypes_Sphere,
		GetObjectsTypes_Box,
		//GetObjectsTypes_Frustum,
		GetObjectsTypes_Planes,
		GetObjectsTypes_Ray,
	};

	///////////////////////////////////////////

	enum ModeEnum
	{
		ModeEnum_All,
		ModeEnum_One,
	};

	/////////////////////////////////////////////

	struct GetObjectsExtensionData
	{
		//int Mode;
		void* occlusionCullingBuffer;
		Vector3D cameraPosition;
		Ogre::Matrix4 viewProjectionMatrix;
		//can use flags
		int occlusionCullingBufferCullNodes;
		int occlusionCullingBufferCullObjects;
	};

	/////////////////////////////////////////////

	struct GetObjectsInputData
	{
		uint groupMask;
		GetObjectsTypes type;
		BoundsD bounds;
		Vector3D sphereCenter;
		double sphereRadius;
		OBBD box;
		int planeCount;
		PlaneD* planes;
		int planesUseAdditionalBounds;
		RayD ray;
		ModeEnum mode;
		GetObjectsExtensionData* extensionData;
	};

	/////////////////////////////////////////////

	struct GetObjectsContext
	{
		std::vector<bool>* getObjectsChecked;
		std::vector<int>* getObjectsCheckedList2;//std::vector<int> getObjectsCheckedList;
		GetObjectsExtensionData* extensionData;
	};

	/////////////////////////////////////////////

	struct GetObjectsRayOutputData
	{
		int objectIndex;
		float/*double*/ distanceNormalized;

		GetObjectsRayOutputData( int objectIndex, float/*double*/ distanceNormalized )
		{
			this->objectIndex = objectIndex;
			this->distanceNormalized = distanceNormalized;
		}
	};

	/////////////////////////////////////////////

	struct ThreadingQueueItem
	{
		enum class CommandEnum
		{
			AddObject,
			RemoveObject,
			UpdateObjectBounds,
			UpdateObjectGroupMask,
		};

		CommandEnum command;
		int objectIndex;
		BoundsD bounds;
		uint groupMask;
	};

	/////////////////////////////////////////////

	//initial settings
	BoundsD initialOctreeBounds;
	int amountOfObjectsOutsideOctreeBoundsToRebuld;
	Vector3D octreeBoundsRebuildExpand;
	Vector3D minNodeSize;
	int objectCountThresholdToCreateChildNodes;
	int maxNodeCount;
	ThreadingModeEnum threadingMode;

	Vector3D minNodeSizeInv;

	//objects
	std::vector<ObjectData*> objects;//!!!!may be without pointers?
	std::stack<int> objectsFreeIndexes;
	int totalObjectCount;

	//octree data
	Node* rootNode;
	BoundsD nodesBounds;
	Vector3I nodesCount;
	int totalNodeCount;

	//objects outside octree bounds
	std::list<ObjectData*> objectsOutsideOctree;

	std::stack<std::vector<bool>*> getObjectsFreeCheckedLists;
	std::mutex getObjectsFreeCheckedListsMutex;
	std::stack<std::vector<int>*> getObjectsFreeCheckedLists2;
	std::mutex getObjectsFreeCheckedListsMutex2;

	std::mutex updateMutex;

	//!!!!use concurrent queue? but what about other fields
	//!!!!can add to queue without mutex because all calls from one thread?
	std::queue<ThreadingQueueItem> threadingQueue;
	std::atomic<bool> backgroundThreadInWork = false;
	std::atomic<bool> backgroundThreadNeedExit = false;

	std::mutex threadingModeMutex;
	std::thread backgroundThread;

	std::atomic<double> engineTimeToGetStatistics;
	std::atomic<double> lastRebuildTime;

	/////////////////////////////////////////////

	OctreeContainer( const BoundsD& initialOctreeBounds, int amountOfObjectsOutsideOctreeBoundsToRebuld, const Vector3D& octreeBoundsRebuildExpand, const Vector3D& minNodeSize, int objectCountThresholdToCreateChildNodes, int maxNodeCount, ThreadingModeEnum threadingMode, int getObjectsInputDataSize, double engineTimeToGetStatistics )
	{

		////!!!!
		//threadingMode = ThreadingModeEnum::SingleThreaded;


		if( getObjectsInputDataSize != sizeof( GetObjectsInputData ) )
			Fatal( "OctreeContainer: Constructor: getObjectsInputDataSize != sizeof( GetObjectsInputData )." );

		this->initialOctreeBounds = initialOctreeBounds;
		this->amountOfObjectsOutsideOctreeBoundsToRebuld = amountOfObjectsOutsideOctreeBoundsToRebuld;
		this->octreeBoundsRebuildExpand = octreeBoundsRebuildExpand;
		this->minNodeSize = minNodeSize;
		this->objectCountThresholdToCreateChildNodes = objectCountThresholdToCreateChildNodes;
		this->maxNodeCount = maxNodeCount;
		this->threadingMode = threadingMode;
		this->engineTimeToGetStatistics = engineTimeToGetStatistics;

		minNodeSizeInv = 1.0f / minNodeSize;

		rootNode = NULL;
		totalObjectCount = 0;
		totalNodeCount = 0;

		RebuildTree( true );

		if (threadingMode == ThreadingModeEnum::BackgroundThread)
			backgroundThread = std::thread{ &OctreeContainer::BackgroundThreadFunction, this };
	}

	~OctreeContainer()
	{
		if (threadingMode == ThreadingModeEnum::BackgroundThread)
		{
			WaitForExecuteAllBackgroundTasks();
			backgroundThreadNeedExit = true;
			backgroundThread.join();
		}

		while (getObjectsFreeCheckedLists.size() != 0)
		{
			delete getObjectsFreeCheckedLists.top();
			getObjectsFreeCheckedLists.pop();
		}

		while (getObjectsFreeCheckedLists2.size() != 0)
		{
			delete getObjectsFreeCheckedLists2.top();
			getObjectsFreeCheckedLists2.pop();
		}

		//remove all objects
		for( int objectIndex = 0; objectIndex < objects.size(); objectIndex++ )
		{
			if( objects[ objectIndex ]->flags != ObjectData::Flags_Free )
				RemoveObject( objectIndex );
		}

		DestroyTree();

		for( int n = 0; n < objects.size(); n++ )
			delete objects[ n ];
	}

	//void UpdateSettings( int amountOfObjectsOutsideOctreeBoundsToRebuld, const Vector3D& octreeBoundsRebuildExpand, const Vector3D& minNodeSize, int objectCountThresholdToCreateChildNodes, int maxNodeCount, ThreadingModeEnum threadingMode )//, bool forceTreeRebuild )
	//{
	//	this->amountOfObjectsOutsideOctreeBoundsToRebuld = amountOfObjectsOutsideOctreeBoundsToRebuld;
	//	this->octreeBoundsRebuildExpand = octreeBoundsRebuildExpand;
	//	this->objectCountThresholdToCreateChildNodes = objectCountThresholdToCreateChildNodes;
	//	this->maxNodeCount = maxNodeCount;
	//	this->minNodeSize = minNodeSize;
	//	this->threadingMode = threadingMode;

	//	RebuildTree( false );


	//	//bool rebuild = false;

	//	//this->amountOfObjectsOutsideOctreeBoundsToRebuld = amountOfObjectsOutsideOctreeBoundsToRebuld;
	//	//this->octreeBoundsRebuildExpand = octreeBoundsRebuildExpand;
	//	//this->objectCountThresholdToCreateChildNodes = objectCountThresholdToCreateChildNodes;
	//	//this->maxNodeCount = maxNodeCount;
	//	//if( this->minNodeSize != minNodeSize )
	//	//{
	//	//	this->minNodeSize = minNodeSize;
	//	//	rebuild = true;
	//	//}

	//	//if( objectsOutsideOctree.size() > amountOfObjectsOutsideOctreeBoundsToRebuld )
	//	//	rebuild = true;

	//	//if( rebuild || forceTreeRebuild )
	//	//	RebuildTree( false );
	//}

	void EnableNeedCreateChildrenNodes( Node* node )
	{
		if( node->needCreateChildrenNodes )
			Fatal( "OctreeContainer: EnableNeedCreateChildrenNodes: node->needCreateChildrenNodes == true." );
		node->needCreateChildrenNodes = true;

		//move objects to the children nodes.

		std::vector<ObjectData*> objectsOfNode;
		objectsOfNode.reserve( node->objectsIntersectsNodeBounds.size() );
		for( std::list<ObjectData*>::iterator it = node->objectsIntersectsNodeBounds.begin(); 
			it != node->objectsIntersectsNodeBounds.end(); it++ )
		{
			objectsOfNode.push_back( *it );
		}

		//remove intersect objects from the node
		for( int nObject = 0; nObject < objectsOfNode.size(); nObject++ )
		{
			ObjectData* objectData = objectsOfNode[ nObject ];

			std::vector<ObjectData::NodeObjectsIntersectsNodeBoundsListIteratorItem> notDeletedItems;
			notDeletedItems.reserve( objectData->nodeObjectsIntersectsNodeBoundsListIterators.size() );

			for( int n = 0; n < objectData->nodeObjectsIntersectsNodeBoundsListIterators.size(); n++ )
			{
				ObjectData::NodeObjectsIntersectsNodeBoundsListIteratorItem& item = 
					objectData->nodeObjectsIntersectsNodeBoundsListIterators[ n ];
				if( item.node == node )
				{
					//remove the object from the node
					node->objectsIntersectsNodeBounds.erase( item.listIterator );
				}
				else
					notDeletedItems.push_back( item );
			}

			//remove deleted items from the list
			objectData->nodeObjectsIntersectsNodeBoundsListIterators.swap( notDeletedItems );
		}

		if( node->objectsIntersectsNodeBounds.size() != 0 )
			Fatal( "OctreeContainer: AddObjectToNodesRecursive: node->objectsIntersectsNodeBounds.size() != 0." );

		//add objects to the children nodes
		for( int nObject = 0; nObject < objectsOfNode.size(); nObject++ )
		{
			ObjectData* objectData = objectsOfNode[ nObject ];

			for( int nChild = 0; nChild < node->childrenCount; nChild++ )
			{
				if( node->childrenBounds[ nChild ].intersects( objectData->bounds ) )
				{
					Node* childNode = node->children[ nChild ];
					if( !childNode )
					{
						childNode = new Node( this, node, node->childrenIndexBounds[ nChild ] );
						node->children[ nChild ] = childNode;
					}
					AddObjectToNodesRecursive( childNode, objectData );
				}
			}
		}
	}

	void AddObjectToNodesRecursive( Node* node, ObjectData* objectData )
	{
		Vector3D objectBoundsSize = objectData->bounds.getSize();
		Vector3D nodeBoundsSize = node->bounds.getSize();

		bool bigObject = objectBoundsSize.x > nodeBoundsSize.x || objectBoundsSize.y > nodeBoundsSize.y || objectBoundsSize.z > nodeBoundsSize.z;
		if( bigObject )
		//if( objectData->bounds.contains( node->bounds ) )
		{
			//object bounds contains node bounds.

			node->bigObjects.push_back( objectData );

			ObjectData::NodeBigObjectsListIteratorItem item;
			item.node = node;
			//item.contains = objectData->bounds.contains( node->bounds );
			item.listIterator = node->bigObjects.end();
			item.listIterator--;
			objectData->nodeBigObjectsListIterators.push_back( item );
		}
		else
		{
			//object bounds itersects node bounds.

			//need create children nodes?
			if( !node->needCreateChildrenNodes && 
				node->objectsIntersectsNodeBounds.size() >= objectCountThresholdToCreateChildNodes && !node->endNode && 
				totalNodeCount < maxNodeCount )
			{
				EnableNeedCreateChildrenNodes( node );
			}

			if( node->needCreateChildrenNodes )
			{
				for( int nChild = 0; nChild < node->childrenCount; nChild++ )
				{
					if( node->childrenBounds[ nChild ].intersects( objectData->bounds ) )
					{
						Node* childNode = node->children[ nChild ];
						if( !childNode )
						{
							childNode = new Node( this, node, node->childrenIndexBounds[ nChild ] );
							node->children[ nChild ] = childNode;
						}
						AddObjectToNodesRecursive( childNode, objectData );
					}
				}
			}
			else
			{
				node->objectsIntersectsNodeBounds.push_back( objectData );

				ObjectData::NodeObjectsIntersectsNodeBoundsListIteratorItem item;
				item.node = node;
				item.listIterator = node->objectsIntersectsNodeBounds.end();
				item.listIterator--;
				objectData->nodeObjectsIntersectsNodeBoundsListIterators.push_back( item );
			}
		}
	}

	void GetEndNodeRangeByBoundsNotClamped( const BoundsD& bounds, BoundsI& result )
	{
		result.minimum.x = (int)( ( bounds.minimum.x - nodesBounds.minimum.x ) * minNodeSizeInv.x );
		result.minimum.y = (int)( ( bounds.minimum.y - nodesBounds.minimum.y ) * minNodeSizeInv.y );
		result.minimum.z = (int)( ( bounds.minimum.z - nodesBounds.minimum.z ) * minNodeSizeInv.z );
		result.maximum.x = (int)( ( bounds.maximum.x - nodesBounds.minimum.x ) * minNodeSizeInv.x );
		result.maximum.y = (int)( ( bounds.maximum.y - nodesBounds.minimum.y ) * minNodeSizeInv.y );
		result.maximum.z = (int)( ( bounds.maximum.z - nodesBounds.minimum.z ) * minNodeSizeInv.z );
	}

	void AddObjectToWorld( ObjectData* objectData )
	{
		//objectData->flags = ObjectData::Flags_AddedToWorld;
		objectData->flags = 0;

		//inside octree bounds
		if( nodesBounds.intersects( objectData->bounds ) )
		{
			objectData->flags |= ObjectData::Flags_InsideOctreeBounds;
			AddObjectToNodesRecursive( rootNode, objectData );
		}

		//outside octree bounds
		if( !nodesBounds.contains( objectData->bounds ) )
		{
			objectsOutsideOctree.push_back( objectData );

			objectData->flags |= ObjectData::Flags_OutsideOctreeBounds;
			objectData->objectsOutsideOctreeListIterator = objectsOutsideOctree.end();
			objectData->objectsOutsideOctreeListIterator--;
		}
	}

	void DeleteEmptyNodesCheckedForDeletionRecursive( Node* node )
	{
		node->needCheckForDeletion = false;

		if( node->needCreateChildrenNodes )
		{
			bool allChildredDeleted = true;
			for( int nChild = 0; nChild < node->childrenCount; nChild++ )
			{
				Node* childNode = node->children[ nChild ];
				if( childNode )
				{
					if( childNode->needCheckForDeletion )
					{
						DeleteEmptyNodesCheckedForDeletionRecursive( childNode );

						if( !childNode->needCreateChildrenNodes && childNode->bigObjects.size() == 0 && 
							childNode->objectsIntersectsNodeBounds.size() == 0 )
						{
							delete childNode;
							node->children[ nChild ] = NULL;
						}
						else
							allChildredDeleted = false;
					}
					else
						allChildredDeleted = false;
				}
			}

			if( allChildredDeleted && node->bigObjects.size() == 0 && node->objectsIntersectsNodeBounds.size() == 0 )
				node->needCreateChildrenNodes = false;
		}
	}

	FORCEINLINE void SetNeedCheckForDeletionWithAllParents( Node* node )
	{
		Node* n = node;
		do
		{
			if( n->needCheckForDeletion )
				break;
			n->needCheckForDeletion = true;
			n = n->parent;
		}while( n != NULL );
	}

	void RemoveObjectFromWorld( ObjectData* objectData )
	{
		//inside octree bounds
		if( ( objectData->flags & ObjectData::Flags_InsideOctreeBounds ) != 0 )
		{
			//remove from Node::bigObjects
			for( int n = 0; n < objectData->nodeBigObjectsListIterators.size(); n++ )
			{
				ObjectData::NodeBigObjectsListIteratorItem& item = objectData->nodeBigObjectsListIterators[ n ];
				Node* node = item.node;

				node->bigObjects.erase( item.listIterator );

				SetNeedCheckForDeletionWithAllParents( node );
			}
			objectData->nodeBigObjectsListIterators.clear();

			//remove from Node::objectsIntersectsNodeBounds
			for( int n = 0; n < objectData->nodeObjectsIntersectsNodeBoundsListIterators.size(); n++ )
			{
				ObjectData::NodeObjectsIntersectsNodeBoundsListIteratorItem& item = 
					objectData->nodeObjectsIntersectsNodeBoundsListIterators[ n ];
				Node* node = item.node;

				node->objectsIntersectsNodeBounds.erase( item.listIterator );

				SetNeedCheckForDeletionWithAllParents( node );
			}
			objectData->nodeObjectsIntersectsNodeBoundsListIterators.clear();
		}

		//outside octree bounds
		if( ( objectData->flags & ObjectData::Flags_OutsideOctreeBounds ) != 0 )
			objectsOutsideOctree.erase( objectData->objectsOutsideOctreeListIterator );
	}

	int AddObject( const Vector3D& boundsMin, const Vector3D& boundsMax, uint groupMask )
	{
		BoundsD bounds(boundsMin, boundsMax);

		if (threadingMode == ThreadingModeEnum::BackgroundThread)
			threadingModeMutex.lock();

		//expand the objects array
		if( objectsFreeIndexes.size() == 0 )
		{
			int objectIndex = objects.size();

			ObjectData* objectData = new ObjectData();
			objectData->index = objectIndex;
			objectData->flags = ObjectData::Flags_Free;

			objects.push_back( objectData );

			objectsFreeIndexes.push( objectIndex );
		}

		//get index for the object
		int objectIndex = objectsFreeIndexes.top();
		objectsFreeIndexes.pop();

		if (threadingMode == ThreadingModeEnum::SingleThreaded)
		{
			//fill data
			ObjectData* objectData = objects[objectIndex];
			objectData->groupMask = groupMask;
			objectData->bounds = bounds;
			objectData->boundsCenter = bounds.getCenter();
			objectData->boundsHalfSize = bounds.getMaximum() - objectData->boundsCenter;

			GetEndNodeRangeByBoundsNotClamped(bounds, objectData->nodeBoundsIndexes);
			objectData->flags = 0;

			totalObjectCount++;

			AddObjectToWorld(objectData);
		}

		if (threadingMode == ThreadingModeEnum::BackgroundThread)
		{
			ThreadingQueueItem item;
			item.command = ThreadingQueueItem::CommandEnum::AddObject;
			item.objectIndex = objectIndex;
			item.bounds = bounds;
			item.groupMask = groupMask;
			threadingQueue.push(item);

			threadingModeMutex.unlock();
		}

		return objectIndex;
	}

	void RemoveObject( int objectIndex )
	{
		if (threadingMode == ThreadingModeEnum::SingleThreaded)
		{
#if _DEBUG
			if (objectIndex < 0 || objectIndex >= objects.size())
				Fatal("OctreeContainer: RemoveObject: Invalid object index. objectIndex < 0 || objectIndex >= objects.size().");
			if (objects[objectIndex]->flags == ObjectData::Flags_Free)
				Fatal("OctreeContainer: RemoveObject: Invalid object index. objects[ objectIndex ]->flags == ObjectData::Flags_Free.");
#endif

			ObjectData* objectData = objects[objectIndex];

			totalObjectCount--;

			RemoveObjectFromWorld(objectData);
			////delete empty nodes
			//if (rootNode->needCheckForDeletion)
			//	DeleteEmptyNodesCheckedForDeletionRecursive(rootNode);

			//free index
			objects[objectIndex]->flags = ObjectData::Flags_Free;
			objectsFreeIndexes.push(objectIndex);
		}
		else
		{
			ThreadingQueueItem item;
			item.command = ThreadingQueueItem::CommandEnum::RemoveObject;
			item.objectIndex = objectIndex;
			//item.bounds = BoundsD::BOUNDSD_ZERO;
			//item.groupMask = 0;
			threadingModeMutex.lock();
			threadingQueue.push(item);
			threadingModeMutex.unlock();
		}
	}

	void UpdateObjectBounds(int objectIndex, const Vector3D& boundsMin, const Vector3D& boundsMax)
	{
		if (threadingMode == ThreadingModeEnum::SingleThreaded)
		{
#if _DEBUG
			if (objectIndex < 0 || objectIndex >= objects.size())
				Fatal("OctreeContainer: UpdateObject: Invalid object index. objectIndex < 0 || objectIndex >= objects.size().");
			if (objects[objectIndex]->flags == ObjectData::Flags_Free)
				Fatal("OctreeContainer: UpdateObject: Invalid object index. objects[ objectIndex ]->flags == ObjectData::Flags_Free.");
#endif

			ObjectData* objectData = objects[objectIndex];

			if (objectData->bounds.minimum == boundsMin && objectData->bounds.maximum == boundsMax)
				return;

			BoundsD bounds(boundsMin, boundsMax);

			BoundsI newNodeBoundsIndexes;
			GetEndNodeRangeByBoundsNotClamped(bounds, newNodeBoundsIndexes);
			bool equalNodeIndexes = objectData->nodeBoundsIndexes == newNodeBoundsIndexes;

			if (!equalNodeIndexes)
				RemoveObjectFromWorld(objectData);

			//objectData->groupMask = groupMask;// (uint)(1 << group);
			objectData->bounds = bounds;
			objectData->boundsCenter = bounds.getCenter();
			objectData->boundsHalfSize = bounds.getMaximum() - objectData->boundsCenter;
			objectData->nodeBoundsIndexes = newNodeBoundsIndexes;

			if (!equalNodeIndexes)
				AddObjectToWorld(objectData);

			////delete empty nodes
			//if (rootNode->needCheckForDeletion)
			//	DeleteEmptyNodesCheckedForDeletionRecursive(rootNode);
		}
		else
		{
			BoundsD bounds(boundsMin, boundsMax);

			ThreadingQueueItem item;
			item.command = ThreadingQueueItem::CommandEnum::UpdateObjectBounds;
			item.objectIndex = objectIndex;
			item.bounds = bounds;
			//item.groupMask = 0;//groupMask;
			threadingModeMutex.lock();
			threadingQueue.push(item);
			threadingModeMutex.unlock();
		}
	}

	void UpdateObjectGroupMask(int objectIndex, uint groupMask)
	{
		if (threadingMode == ThreadingModeEnum::SingleThreaded)
		{
#if _DEBUG
			if (objectIndex < 0 || objectIndex >= objects.size())
				Fatal("OctreeContainer: UpdateObject: Invalid object index. objectIndex < 0 || objectIndex >= objects.size().");
			if (objects[objectIndex]->flags == ObjectData::Flags_Free)
				Fatal("OctreeContainer: UpdateObject: Invalid object index. objects[ objectIndex ]->flags == ObjectData::Flags_Free.");
#endif

			ObjectData* objectData = objects[objectIndex];
			objectData->groupMask = groupMask;
		}
		else
		{
			ThreadingQueueItem item;
			item.command = ThreadingQueueItem::CommandEnum::UpdateObjectGroupMask;
			item.objectIndex = objectIndex;
			//item.bounds = BoundsD::BOUNDSD_ZERO;
			item.groupMask = groupMask;
			threadingModeMutex.lock();
			threadingQueue.push(item);
			threadingModeMutex.unlock();
		}
	}

	/////////////////////////////////////////////

	class GetObjectsCheckShape
	{
	public:
		GetObjectsTypes type;

		virtual ~GetObjectsCheckShape() {}

		virtual bool Intersects( const BoundsD& bounds, const Vector3D& boundsCenter, const Vector3D& boundsHalfSize ) = 0;
		virtual bool Contains( const BoundsD& bounds, const Vector3D& boundsCenter, const Vector3D& boundsHalfSize ) = 0;

		virtual bool Intersects(const BoundsD& bounds ) = 0;
		virtual bool Contains(const BoundsD& bounds ) = 0;

		//check fully inside bounds
		virtual bool InsideBounds( const BoundsD& bounds ) = 0;
	};

	/////////////////////////////////////////////

	class GetObjectsCheckShape_Bounds : public GetObjectsCheckShape
	{
	public:
		BoundsD getObjectsBounds;

		GetObjectsCheckShape_Bounds()
		{
			type = GetObjectsTypes_Bounds;
		}

		virtual bool Intersects( const BoundsD& bounds, const Vector3D& boundsCenter, const Vector3D& boundsHalfSize )
		{
			return getObjectsBounds.intersects( bounds );
		}

		virtual bool Contains( const BoundsD& bounds, const Vector3D& boundsCenter, const Vector3D& boundsHalfSize )
		{
			//TO DO: никогда не будет contains если одна из сторон getObjectsBounds меньше размера ноды
			//  где еще также сделать?
			return getObjectsBounds.contains( bounds );
		}

		virtual bool Intersects(const BoundsD& bounds )
		{
			return getObjectsBounds.intersects(bounds);
		}

		virtual bool Contains(const BoundsD& bounds )
		{
			//TO DO: никогда не будет contains если одна из сторон getObjectsBounds меньше размера ноды
			//  где еще также сделать?
			return getObjectsBounds.contains(bounds);
		}

		virtual bool InsideBounds( const BoundsD& bounds )
		{
			return bounds.contains( getObjectsBounds );
		}
	};

	/////////////////////////////////////////////

	class GetObjectsCheckShape_Sphere : public GetObjectsCheckShape
	{
	public:
		SphereD getObjectsSphere;
		BoundsD boundsOutsideSphere;
		BoundsD boundsInsideSphere;

		GetObjectsCheckShape_Sphere()
		{
			type = GetObjectsTypes_Sphere;
		}

		virtual bool Intersects( const BoundsD& bounds, const Vector3D& boundsCenter, const Vector3D& boundsHalfSize )
		{
			if( boundsOutsideSphere.intersects( bounds ) )
			{
				AxisAlignedBoxD axisAlignedBox( bounds.minimum, bounds.maximum );
				return getObjectsSphere.intersects( axisAlignedBox );
			}
			return false;
		}

		virtual bool Contains( const BoundsD& bounds, const Vector3D& boundsCenter, const Vector3D& boundsHalfSize )
		{
			return boundsInsideSphere.contains( bounds );
		}

		virtual bool Intersects(const BoundsD& bounds )
		{
			if (boundsOutsideSphere.intersects(bounds))
			{
				AxisAlignedBoxD axisAlignedBox(bounds.minimum, bounds.maximum);
				return getObjectsSphere.intersects(axisAlignedBox);
			}
			return false;
		}

		virtual bool Contains(const BoundsD& bounds )
		{
			return boundsInsideSphere.contains(bounds);
		}

		virtual bool InsideBounds( const BoundsD& bounds )
		{
			return bounds.contains( boundsOutsideSphere );
		}
	};

	/////////////////////////////////////////////

	class GetObjectsCheckShape_Box : public GetObjectsCheckShape
	{
	public:
		OBBD getObjectsBox;
		BoundsD boundsOutsideBox;

		GetObjectsCheckShape_Box()
		{
			type = GetObjectsTypes_Box;
		}

		virtual bool Intersects( const BoundsD& bounds, const Vector3D& boundsCenter, const Vector3D& boundsHalfSize )
		{
			if( boundsOutsideBox.intersects( bounds ) )
				return getObjectsBox.intersects( bounds );
			return false;
		}

		virtual bool Contains( const BoundsD& bounds, const Vector3D& boundsCenter, const Vector3D& boundsHalfSize )
		{
			return getObjectsBox.contains( bounds );
		}

		virtual bool Intersects(const BoundsD& bounds )
		{
			if (boundsOutsideBox.intersects(bounds))
				return getObjectsBox.intersects(bounds);
			return false;
		}

		virtual bool Contains(const BoundsD& bounds )
		{
			return getObjectsBox.contains(bounds);
		}

		virtual bool InsideBounds( const BoundsD& bounds )
		{
			return bounds.contains( boundsOutsideBox );
		}
	};

	/////////////////////////////////////////////

	class GetObjectsCheckShape_Planes : public GetObjectsCheckShape
	{
	public:
		int planeCount;
		PlaneD* planes;
		bool additionalBoundsInitialized;
		BoundsD additionalOutsideBounds;

		GetObjectsCheckShape_Planes()
		{
			type = GetObjectsTypes_Planes;
		}

		virtual bool Intersects( const BoundsD& bounds, const Vector3D& boundsCenter, const Vector3D& boundsHalfSize )
		{
			if( !additionalBoundsInitialized || additionalOutsideBounds.intersects( bounds ) )
			{
				for( int n = 0; n < planeCount; n++ )
				{
					if( planes[ n ].getSide( boundsCenter, boundsHalfSize ) == PlaneD::POSITIVE_SIDE )
						return false;
				}
				return true;
			}
			return false;
		}

		virtual bool Contains( const BoundsD& bounds, const Vector3D& boundsCenter, const Vector3D& boundsHalfSize )
		{
			for( int n = 0; n < planeCount; n++ )
			{
				if( planes[ n ].getSide( boundsCenter, boundsHalfSize ) != PlaneD::NEGATIVE_SIDE )
					return false;
			}
			return true;
		}

		virtual bool Intersects(const BoundsD& bounds )
		{
			if (!additionalBoundsInitialized || additionalOutsideBounds.intersects(bounds))
			{
				Vector3D boundsCenter = bounds.getCenter();
				Vector3D boundsHalfSize = bounds.getMaximum() - boundsCenter;

				for (int n = 0; n < planeCount; n++)
				{
					if (planes[n].getSide(boundsCenter, boundsHalfSize) == PlaneD::POSITIVE_SIDE)
						return false;
				}
				return true;
			}
			return false;
		}

		virtual bool Contains(const BoundsD& bounds )
		{
			Vector3D boundsCenter = bounds.getCenter();
			Vector3D boundsHalfSize = bounds.getMaximum() - boundsCenter;

			for (int n = 0; n < planeCount; n++)
			{
				if (planes[n].getSide(boundsCenter, boundsHalfSize) != PlaneD::NEGATIVE_SIDE)
					return false;
			}
			return true;
		}

		virtual bool InsideBounds( const BoundsD& bounds )
		{
			if( additionalBoundsInitialized && bounds.contains( additionalOutsideBounds ) )
				return true;
			return false;
		}
	};

	/////////////////////////////////////////////

	//!!!!может можно склеить ExtensionDataIntersects и ExtensionDataContains

	bool ExtensionDataIntersects( GetObjectsContext* context, const BoundsD& bounds, bool isNode )
	{
		OctreeContainer::GetObjectsExtensionData* data = context->extensionData;
		if (data != nullptr && (data->occlusionCullingBufferCullNodes && isNode || data->occlusionCullingBufferCullObjects && !isNode))
		{
			MaskedOcclusionCulling* buffer = (MaskedOcclusionCulling*)data->occlusionCullingBuffer;
			float* modelToClipMatrix = (float*)&data->viewProjectionMatrix;

			auto minRelativeD = bounds.minimum - data->cameraPosition;
			auto maxRelativeD = bounds.maximum - data->cameraPosition;
			auto minRelative = Vector3(minRelativeD.x, minRelativeD.y, minRelativeD.z);
			auto maxRelative = Vector3(maxRelativeD.x, maxRelativeD.y, maxRelativeD.z);

			Vector3 positions[8];
			positions[0] = Vector3(maxRelative.x, minRelative.y, minRelative.z);
			positions[1] = Vector3(maxRelative.x, minRelative.y, maxRelative.z);
			positions[2] = Vector3(maxRelative.x, maxRelative.y, minRelative.z);
			positions[3] = Vector3(maxRelative.x, maxRelative.y, maxRelative.z);
			positions[4] = Vector3(minRelative.x, minRelative.y, minRelative.z);
			positions[5] = Vector3(minRelative.x, minRelative.y, maxRelative.z);
			positions[6] = Vector3(minRelative.x, maxRelative.y, minRelative.z);
			positions[7] = Vector3(minRelative.x, maxRelative.y, maxRelative.z);

			//GetBoundsGeometryVertices(bounds, positions);

			////for (int n = 0; n < 8; n++)
			////{
			////	Vector3 pos = data->viewProjectionMatrix * positions[n];
			////	pos.z = 1.0f / pos.z;
			////	positions[n] = pos;
			////	//positions[n] = data->viewProjectionMatrix * positions[n];
			////}

			////Bounds b = Bounds(positions[0]);
			////for (int n = 1; n < 8; n++)
			////	b.add(positions[n]);


			//works with bugs when BACKFACE_CW

			//!!!!CLIP_PLANE_ALL?

			MaskedOcclusionCulling::CullingResult result = buffer->TestTriangles((float*)&positions[0], &boundsGeometryIndices[0], 12, modelToClipMatrix, MaskedOcclusionCulling::BACKFACE_NONE, MaskedOcclusionCulling::CLIP_PLANE_SIDES/*CLIP_PLANE_ALL*/, MaskedOcclusionCulling::VertexLayout(12, 4, 8));

			//MaskedOcclusionCulling::CullingResult result = buffer->TestTriangles((float*)&positions[0], &boundsGeometryIndices[0], 12, modelToClipMatrix, MaskedOcclusionCulling::BACKFACE_CW, MaskedOcclusionCulling::CLIP_PLANE_ALL, MaskedOcclusionCulling::VertexLayout(12, 4, 8));
			// 
			////MaskedOcclusionCulling::CullingResult result = buffer->TestRect(b.minimum.x, b.minimum.y, b.maximum.x, b.maximum.y, b.maximum.z);

			return (result & MaskedOcclusionCulling::OCCLUDED) == 0;

			////if (result & MaskedOcclusionCulling::OCCLUDED)//if (result == MaskedOcclusionCulling::OCCLUDED)
			////	return false;
			////else
			////	return true;
		}

		return true;
	}

	bool ExtensionDataContains( GetObjectsContext* context, const BoundsD& bounds )
	{
		OctreeContainer::GetObjectsExtensionData* data = context->extensionData;
		if (data != nullptr )
		{
			//!!!!slowly
			
			//!!!!модифицированный TestRect

			return false;
		}

		return true;
	}

	void GetObjectsNodeRecursive( GetObjectsContext* context, Node* node, GetObjectsCheckShape* checkShape, bool skipShapeCheck, uint groupMask, ModeEnum mode, int* outputArray, int outputArraySize, int& resultCount, int& outputArrayIndex, int& nodesProcessed )
	{
		nodesProcessed++;

		//bigObjects
		for( std::list<ObjectData*>::iterator it = node->bigObjects.begin(); it != node->bigObjects.end(); it++ )
		{
			ObjectData* objectData = *it;

			if( (groupMask & objectData->groupMask) != 0 && !(*context->getObjectsChecked)[objectData->index] )
			{
				if( skipShapeCheck || checkShape->Intersects( objectData->bounds, objectData->boundsCenter, objectData->boundsHalfSize ) && ExtensionDataIntersects( context, objectData->bounds, false ) )
				{
					if (mode == ModeEnum_One && resultCount != 0)
						break;

					//add object to the output array
					resultCount++;
					if( outputArray != NULL && outputArrayIndex < outputArraySize )
					{
						outputArray[ outputArrayIndex ] = objectData->index;
						outputArrayIndex++;
					}

					if (mode == ModeEnum_One)
						break;
				}

				(*context->getObjectsChecked)[objectData->index] = true;
				context->getObjectsCheckedList2->push_back(objectData->index);
			}
		}

		//objectsIntersectsNodeBounds
		for( std::list<ObjectData*>::iterator it = node->objectsIntersectsNodeBounds.begin(); it != node->objectsIntersectsNodeBounds.end(); it++ )
		{
			ObjectData* objectData = *it;

			if ( (groupMask & objectData->groupMask) != 0 && !(*context->getObjectsChecked)[objectData->index])
			{
				if( skipShapeCheck || checkShape->Intersects( objectData->bounds, objectData->boundsCenter, objectData->boundsHalfSize ) && ExtensionDataIntersects( context, objectData->bounds, false ) )
				{
					if (mode == ModeEnum_One && resultCount != 0)
						break;

					//add object to the output array
					resultCount++;
					if( outputArray != NULL && outputArrayIndex < outputArraySize )
					{
						outputArray[ outputArrayIndex ] = objectData->index;
						outputArrayIndex++;
					}
					
					if (mode == ModeEnum_One )
						break;
				}

				(*context->getObjectsChecked)[objectData->index] = true;
				context->getObjectsCheckedList2->push_back(objectData->index);
			}
		}

		//enumerate children nodes
		if( node->needCreateChildrenNodes && ( mode == ModeEnum_One && resultCount == 0 || mode == ModeEnum_All ) )
		{
			for( int nChild = 0; nChild < node->childrenCount; nChild++ )
			{
				Node* childNode = node->children[ nChild ];

				//BoundsI z = childNode->indexBounds;
				//z.maximum.x--;
				//z.maximum.y--;
				//z.maximum.z--;
				//if( childNode && childNode->indexBoundsMinusOne.intersects( indexBounds ) )
				//if( childNode && childNode->indexBounds.intersects( indexBounds ) )

				if( childNode && ( skipShapeCheck || checkShape->Intersects( childNode->bounds, childNode->boundsCenter, childNode->boundsHalfSize ) && ExtensionDataIntersects( context, childNode->bounds, true ) ) )
				{
					bool childSkipShapeCheck;
					if( !skipShapeCheck )
						childSkipShapeCheck = checkShape->Contains( childNode->bounds, childNode->boundsCenter, childNode->boundsHalfSize ) && ExtensionDataContains( context, childNode->bounds );
					else
						childSkipShapeCheck = true;

					GetObjectsNodeRecursive( context, childNode, checkShape, childSkipShapeCheck, groupMask, mode, outputArray, outputArraySize, 
						resultCount, outputArrayIndex, nodesProcessed );
				}
			}
		}
	}

	void CheckForRebuildTree()
	{
		if( objectsOutsideOctree.size() > amountOfObjectsOutsideOctreeBoundsToRebuld )
			RebuildTree( false );
	}

	std::vector<bool>* GetObjectsFreeCheckedList()
	{
		getObjectsFreeCheckedListsMutex.lock();

		if (getObjectsFreeCheckedLists.size() == 0)
			getObjectsFreeCheckedLists.push(new std::vector<bool>());

		std::vector<bool>* result = getObjectsFreeCheckedLists.top();
		getObjectsFreeCheckedLists.pop();
		if (result->size() < objects.size())
			result->resize(objects.size(), false);

		getObjectsFreeCheckedListsMutex.unlock();

		return result;
	}

	void FreeGetObjectsFreeCheckedList(std::vector<bool>* list)
	{
		getObjectsFreeCheckedListsMutex.lock();
		getObjectsFreeCheckedLists.push(list);
		getObjectsFreeCheckedListsMutex.unlock();
	}

	std::vector<int>* GetObjectsFreeCheckedList2()
	{
		getObjectsFreeCheckedListsMutex2.lock();

		if (getObjectsFreeCheckedLists2.size() == 0)
			getObjectsFreeCheckedLists2.push(new std::vector<int>());

		std::vector<int>* result = getObjectsFreeCheckedLists2.top();
		getObjectsFreeCheckedLists2.pop();

		getObjectsFreeCheckedListsMutex2.unlock();

		return result;
	}

	void FreeGetObjectsFreeCheckedList2(std::vector<int>* list)
	{
		list->clear();
		getObjectsFreeCheckedListsMutex2.lock();
		getObjectsFreeCheckedLists2.push(list);
		getObjectsFreeCheckedListsMutex2.unlock();
	}

	int GetObjects( const GetObjectsInputData& inputData, int* outputArray, int outputArraySize )
	{
		GetObjectsContext context;//GetObjectsContext* context = new GetObjectsContext();
		context.getObjectsChecked = GetObjectsFreeCheckedList();
		context.getObjectsCheckedList2 = GetObjectsFreeCheckedList2();//context->getObjectsCheckedList.reserve(objects.size());
		context.extensionData = inputData.extensionData;

		////rebuild tree if need
		//CheckForRebuildTree();

		GetObjectsCheckShape* checkShape = NULL;

		switch( inputData.type )
		{
		case GetObjectsTypes_Bounds:
			{
				GetObjectsCheckShape_Bounds* checkShapeBounds = new GetObjectsCheckShape_Bounds();
				checkShapeBounds->getObjectsBounds = inputData.bounds;
				checkShape = checkShapeBounds;
			}
			break;

		case GetObjectsTypes_Sphere:
			{
				GetObjectsCheckShape_Sphere* checkShapeSphere = new GetObjectsCheckShape_Sphere();

				SphereD s = SphereD( inputData.sphereCenter, inputData.sphereRadius );
				float r = s.getRadius();

				checkShapeSphere->getObjectsSphere = s;
				checkShapeSphere->boundsOutsideSphere = 
					BoundsD( s.getCenter() - Vector3D( r, r, r ), s.getCenter() + Vector3D( r, r, r ) );

				float v = s.getRadius() * 0.57735026918f;//MathFunctions.Pow( 1.0f / 3.0f, .5f );
				checkShapeSphere->boundsInsideSphere = BoundsD( 
					s.getCenter() - Vector3D( v, v, v ),
					s.getCenter() + Vector3D( v, v, v ) );

				checkShape = checkShapeSphere;
			}
			break;

		case GetObjectsTypes_Box:
			{
				GetObjectsCheckShape_Box* checkShapeBox = new GetObjectsCheckShape_Box();
				checkShapeBox->getObjectsBox = inputData.box;
				checkShapeBox->boundsOutsideBox = inputData.box.toBoundsD();
				checkShape = checkShapeBox;
			}
			break;

		//case GetObjectsTypes_Frustum:
		//	{
		//		GetObjectsCheckShape_Frustum* checkShapeFrustum = new GetObjectsCheckShape_Frustum();
		//		checkShapeFrustum->getObjectsFrustum = inputData.frustum;
		//		checkShape = checkShapeFrustum;
		//	}
		//	break;

		case GetObjectsTypes_Planes:
			{
				GetObjectsCheckShape_Planes* checkShapePlanes = new GetObjectsCheckShape_Planes();
				checkShapePlanes->planeCount = inputData.planeCount;
				checkShapePlanes->planes = inputData.planes;
				checkShapePlanes->additionalBoundsInitialized = inputData.planesUseAdditionalBounds != 0;
				checkShapePlanes->additionalOutsideBounds = inputData.bounds;
				checkShape = checkShapePlanes;
			}
			break;

		default:
			Fatal( "OctreeContainer: GetObjects: inputData.type is not implemented." );
		}

		int resultCount = 0;
		int outputArrayIndex = 0;

		//check objects placed inside octree bounds
		if( checkShape->Intersects( nodesBounds ) )//nodesBounds.intersects( bounds ) )
		{
			//BoundsI indexBounds;
			//GetEndNodeRangeByBoundsNotClamped( bounds, indexBounds );

			bool skipBoundsCheck = checkShape->Contains( nodesBounds );
			int nodesProcessed = 0;
			GetObjectsNodeRecursive( &context, rootNode, checkShape, skipBoundsCheck, inputData.groupMask, inputData.mode, outputArray, outputArraySize, 
				resultCount, outputArrayIndex, nodesProcessed );
		}

		//check objects placed outside octree bounds
		if( !checkShape->InsideBounds( nodesBounds ) )
		{
			for( std::list<ObjectData*>::iterator it = objectsOutsideOctree.begin(); it != objectsOutsideOctree.end(); it++ )
			{
				ObjectData* objectData = *it;

				if ( (inputData.groupMask & objectData->groupMask) != 0 && !(*context.getObjectsChecked)[objectData->index])
				{
					if( checkShape->Intersects( objectData->bounds, objectData->boundsCenter, objectData->boundsHalfSize ) && ExtensionDataIntersects( &context, objectData->bounds, false ))
					{
						if (inputData.mode == ModeEnum_One && resultCount != 0)
							break;

						//add object to the output array
						resultCount++;
						if( outputArray != NULL && outputArrayIndex < outputArraySize )
						{
							outputArray[ outputArrayIndex ] = objectData->index;
							outputArrayIndex++;
						}

						if (inputData.mode == ModeEnum_One)
							break;
					}

					(*context.getObjectsChecked)[objectData->index] = true;
					context.getObjectsCheckedList2->push_back(objectData->index);
				}
			}
		}

		//clear checked list and free
		for( int n = 0; n < context.getObjectsCheckedList2->size(); n++ )
			(*context.getObjectsChecked)[(*context.getObjectsCheckedList2)[n]] = false;
		FreeGetObjectsFreeCheckedList(context.getObjectsChecked);
		FreeGetObjectsFreeCheckedList2(context.getObjectsCheckedList2);

		delete checkShape;
		//delete context;

		return resultCount;
	}

	void GetObjectsRayNodeRecursive( GetObjectsContext* context, Node* node, const RayD& ray, uint groupMask, const BoundsD& rayBounds,
		GetObjectsRayOutputData* outputArray, int outputArraySize, int& resultCount, int& outputArrayIndex, int& nodesProcessed )
	{
		nodesProcessed++;

		//bigObjects
		for( std::list<ObjectData*>::iterator it = node->bigObjects.begin(); it != node->bigObjects.end(); it++ )
		{
			ObjectData* objectData = *it;

			if ( (groupMask & objectData->groupMask) != 0 && !(*context->getObjectsChecked)[objectData->index])
			{
				if( rayBounds.intersects( objectData->bounds ) )
				{
					AxisAlignedBoxD axisAlignedBox( objectData->bounds.minimum, objectData->bounds.maximum );
					std::pair<bool, Real> pair = ray.intersects( axisAlignedBox );
					if( pair.first )
					{
						//add object to the output array
						resultCount++;
						if( outputArray != NULL && outputArrayIndex < outputArraySize )
						{
							outputArray[ outputArrayIndex ] = GetObjectsRayOutputData( objectData->index, pair.second );
							outputArrayIndex++;
						}
					}
				}

				(*context->getObjectsChecked)[objectData->index] = true;
				context->getObjectsCheckedList2->push_back(objectData->index);
			}
		}

		//objectsIntersectsNodeBounds
		for( std::list<ObjectData*>::iterator it = node->objectsIntersectsNodeBounds.begin(); 
			it != node->objectsIntersectsNodeBounds.end(); it++ )
		{
			ObjectData* objectData = *it;

			if ( (groupMask & objectData->groupMask) != 0 && !(*context->getObjectsChecked)[objectData->index])
			{
				if( rayBounds.intersects( objectData->bounds ) )
				{
					AxisAlignedBoxD axisAlignedBox( objectData->bounds.minimum, objectData->bounds.maximum );
					std::pair<bool, Real> pair = ray.intersects( axisAlignedBox );
					if( pair.first )
					{
						//add object to the output array
						resultCount++;
						if( outputArray != NULL && outputArrayIndex < outputArraySize )
						{
							outputArray[ outputArrayIndex ] = GetObjectsRayOutputData( objectData->index, pair.second );
							outputArrayIndex++;
						}
					}
				}

				(*context->getObjectsChecked)[objectData->index] = true;
				context->getObjectsCheckedList2->push_back(objectData->index);
			}
		}

		//enumerate children nodes
		if( node->needCreateChildrenNodes )
		{
			for( int nChild = 0; nChild < node->childrenCount; nChild++ )
			{
				Node* childNode = node->children[ nChild ];
				if( childNode && rayBounds.intersects( childNode->bounds ))
				{
					AxisAlignedBoxD axisAlignedBox( childNode->bounds.minimum, childNode->bounds.maximum );
					std::pair<bool, Real> pair = ray.intersects( axisAlignedBox );
					if( pair.first )
					{
						GetObjectsRayNodeRecursive(context, childNode, ray, groupMask, rayBounds, outputArray, outputArraySize,
							resultCount, outputArrayIndex, nodesProcessed );
					}
				}
			}
		}
	}

	int GetObjectsRay( const GetObjectsInputData& inputData, GetObjectsRayOutputData* outputArray, int outputArraySize )
	{
		GetObjectsContext context;//GetObjectsContext* context = new GetObjectsContext();
		context.getObjectsChecked = GetObjectsFreeCheckedList();
		context.getObjectsCheckedList2 = GetObjectsFreeCheckedList2();//context->getObjectsCheckedList.reserve(objects.size());
		context.extensionData = inputData.extensionData;

		////rebuild tree if need
		//CheckForRebuildTree();

		int resultCount = 0;
		int outputArrayIndex = 0;

		Vector3D rayTo = inputData.ray.getOrigin() + inputData.ray.getDirection();
		BoundsD rayBounds( inputData.ray.getOrigin() );
		rayBounds.add( rayTo );

		bool contains = nodesBounds.contains( rayBounds );//nodesBounds.contains( inputData.ray.getOrigin() ) && nodesBounds.contains( rayTo );
		bool intersects;
		if( contains )
			intersects = true;
		else
			intersects = inputData.ray.intersects( AxisAlignedBoxD( nodesBounds.minimum, nodesBounds.maximum ) ).first;

		if( intersects )
		{
			int nodesProcessed = 0;
			GetObjectsRayNodeRecursive( &context, rootNode, inputData.ray, inputData.groupMask, rayBounds, outputArray, 
				outputArraySize, resultCount, outputArrayIndex, nodesProcessed );
		}

		if( !contains )
		{
			//check objects placed outside octree bounds

			for( std::list<ObjectData*>::iterator it = objectsOutsideOctree.begin(); it != objectsOutsideOctree.end(); it++ )
			{
				ObjectData* objectData = *it;

				if ( (inputData.groupMask & objectData->groupMask) != 0 && !(*context.getObjectsChecked)[objectData->index])
				{
					if( rayBounds.intersects( objectData->bounds ) )
					{
						AxisAlignedBoxD axisAlignedBox( objectData->bounds.minimum, objectData->bounds.maximum );
						std::pair<bool, Real> pair = inputData.ray.intersects( axisAlignedBox );
						if( pair.first )
						{
							//add object to the output array
							resultCount++;
							if( outputArray != NULL && outputArrayIndex < outputArraySize )
							{
								outputArray[ outputArrayIndex ] = GetObjectsRayOutputData( objectData->index, pair.second );
								outputArrayIndex++;
							}
						}
					}

					(*context.getObjectsChecked)[objectData->index] = true;
					context.getObjectsCheckedList2->push_back(objectData->index);
				}
			}
		}

		//sort
		if( outputArray != NULL )
		{
			int count = min( resultCount, outputArraySize );
			qsort( outputArray, count, sizeof( GetObjectsRayOutputData ), GetObjectsRaySortCompare );
		}

		//clear checked list and free
		for (int n = 0; n < context.getObjectsCheckedList2->size(); n++)
			(*context.getObjectsChecked)[(*context.getObjectsCheckedList2)[n]] = false;
		FreeGetObjectsFreeCheckedList(context.getObjectsChecked);
		FreeGetObjectsFreeCheckedList2(context.getObjectsCheckedList2);

		//delete context;

		return resultCount;
	}

	void DestroyTree()
	{
		if( rootNode != NULL )
		{
			for( int objectIndex = 0; objectIndex < objects.size(); objectIndex++ )
			{
				ObjectData* objectData = objects[ objectIndex ];
				if( objectData->flags != ObjectData::Flags_Free )
				{
					objectData->flags = 0;
					objectData->nodeBigObjectsListIterators.clear();
					objectData->nodeObjectsIntersectsNodeBoundsListIterators.clear();
				}
			}

			objectsOutsideOctree.clear();

			delete rootNode;
			rootNode = NULL;
		}
	}

	BoundsD GetNeededOctreeBounds( bool firstTreeCreate )
	{
		BoundsD bounds;
		if( firstTreeCreate )
			bounds = initialOctreeBounds;
		else
			bounds = BoundsD::BOUNDSD_CLEARED;

		//add object bounds
		for( int objectIndex = 0; objectIndex < objects.size(); objectIndex++ )
		{
			ObjectData* objectData = objects[ objectIndex ];
			if( objectData->flags != ObjectData::Flags_Free )
				bounds.add( objectData->bounds );
		}

		if( !bounds.isCleared() && !firstTreeCreate )
			bounds.expand( octreeBoundsRebuildExpand );
		if( bounds.isCleared() )
			bounds = initialOctreeBounds;

		return bounds;
	}

	void RebuildTree( bool firstTreeCreate )
	{
		DestroyTree();

		BoundsD neededBounds = GetNeededOctreeBounds( firstTreeCreate );

		Vector3D countFloat = neededBounds.getSize() / minNodeSize;
		nodesCount = Vector3I( (int)countFloat.x + 1, (int)countFloat.y + 1, (int)countFloat.z + 1 );
		Vector3D newSize = nodesCount.toVector3D() * minNodeSize;
		nodesBounds = BoundsD( neededBounds.getCenter() - newSize / 2, neededBounds.getCenter() + newSize / 2 );

		rootNode = new Node( this, NULL, BoundsI( Vector3I::ZERO, nodesCount ) );

		for( int objectIndex = 0; objectIndex < objects.size(); objectIndex++ )
		{
			ObjectData* objectData = objects[ objectIndex ];
			if( objectData->flags != ObjectData::Flags_Free )
				AddObjectToWorld( objectData );
		}

		//delete free lists for GetObjects
		while (getObjectsFreeCheckedLists.size() != 0)
		{
			delete getObjectsFreeCheckedLists.top();
			getObjectsFreeCheckedLists.pop();
		}
		while (getObjectsFreeCheckedLists2.size() != 0)
		{
			delete getObjectsFreeCheckedLists2.top();
			getObjectsFreeCheckedLists2.pop();
		}

		lastRebuildTime = engineTimeToGetStatistics.load();
	}

	void AddBounds( const BoundsD& bounds, const ColourValue& color, std::vector<DebugRenderLine>& lines )
	{
		const Vector3D& min = bounds.getMinimum();
		const Vector3D& max = bounds.getMaximum();

		lines.push_back( DebugRenderLine( Vector3D( min.x, min.y, min.z ), Vector3D( max.x, min.y, min.z ), color ) );
		lines.push_back( DebugRenderLine( Vector3D( max.x, min.y, min.z ), Vector3D( max.x, max.y, min.z ), color ) );
		lines.push_back( DebugRenderLine( Vector3D( max.x, max.y, min.z ), Vector3D( min.x, max.y, min.z ), color ) );
		lines.push_back( DebugRenderLine( Vector3D( min.x, max.y, min.z ), Vector3D( min.x, min.y, min.z ), color ) );

		lines.push_back( DebugRenderLine( Vector3D( min.x, min.y, max.z ), Vector3D( max.x, min.y, max.z ), color ) );
		lines.push_back( DebugRenderLine( Vector3D( max.x, min.y, max.z ), Vector3D( max.x, max.y, max.z ), color ) );
		lines.push_back( DebugRenderLine( Vector3D( max.x, max.y, max.z ), Vector3D( min.x, max.y, max.z ), color ) );
		lines.push_back( DebugRenderLine( Vector3D( min.x, max.y, max.z ), Vector3D( min.x, min.y, max.z ), color ) );

		lines.push_back( DebugRenderLine( Vector3D( min.x, min.y, min.z ), Vector3D( min.x, min.y, max.z ), color ) );
		lines.push_back( DebugRenderLine( Vector3D( max.x, min.y, min.z ), Vector3D( max.x, min.y, max.z ), color ) );
		lines.push_back( DebugRenderLine( Vector3D( max.x, max.y, min.z ), Vector3D( max.x, max.y, max.z ), color ) );
		lines.push_back( DebugRenderLine( Vector3D( min.x, max.y, min.z ), Vector3D( min.x, max.y, max.z ), color ) );
	}

	void GetDebugRenderLinesNodeRecursive( Node* node, std::vector<DebugRenderLine>& lines )
	{
		Vector3D offset = minNodeSize * .003f;
		BoundsD bounds = node->bounds;
		bounds.expand( -offset );
		AddBounds( bounds, ColourValue( 0, 0, 1 ), lines );

		for( int nChild = 0; nChild < 8; nChild++ )
		{
			Node* childNode = node->children[ nChild ];
			if( childNode )
				GetDebugRenderLinesNodeRecursive( childNode, lines );
		}
	}

	void GetDebugRenderLines( void** outputData, int* outputDataItemCount )
	{
		std::vector<DebugRenderLine> lines;
		lines.reserve( 4096 );

		GetDebugRenderLinesNodeRecursive( rootNode, lines );

		//draw total bounds of objects
		{
			BoundsD totalBounds;
			GetOctreeBoundsWithBoundsOfObjectsOutsideOctree( totalBounds );
			AddBounds( totalBounds, ColourValue( 0, 0, 1 ), lines );
		}

		//draw nodesBounds
		for( int n = 0; n < 5; n++ )
		{
			BoundsD bounds = nodesBounds;
			bounds.expand( (float)n * .08f );
			AddBounds( bounds, ColourValue( 1, 0, 0 ), lines );
		}

		uint8* data = new uint8[ lines.size() * sizeof( DebugRenderLine ) ];
		if( lines.size() != 0 )
			memcpy( data, &lines[ 0 ], lines.size() * sizeof( DebugRenderLine ) );
		*outputData = data;
		*outputDataItemCount = lines.size();
	}

	void GetStatistics( int* objectCount, BoundsD* octreeBounds, int* octreeNodeCount, double* timeSinceLastFullRebuild)
	{
		*objectCount = this->totalObjectCount;
		if( rootNode )
			*octreeBounds = this->nodesBounds;
		else
			*octreeBounds = BoundsD::BOUNDSD_CLEARED;
		*octreeNodeCount = this->totalNodeCount;
		*timeSinceLastFullRebuild = engineTimeToGetStatistics.load() - lastRebuildTime.load();
	}

	void GetOctreeBoundsWithBoundsOfObjectsOutsideOctree( BoundsD& bounds )
	{
		bounds = nodesBounds;
		for( std::list<ObjectData*>::iterator it = objectsOutsideOctree.begin(); it != objectsOutsideOctree.end(); it++ )
		{
			ObjectData* objectData = *it;
			bounds.add( objectData->bounds );
		}
	}

	bool IsBackgroundThreadBusy()
	{
		threadingModeMutex.lock();
		bool result = backgroundThreadInWork.load() || threadingQueue.size() != 0;
		threadingModeMutex.unlock();
		return result;
	}

	void WaitForExecuteAllBackgroundTasks()
	{
		while(IsBackgroundThreadBusy())
			std::this_thread::sleep_for(std::chrono::milliseconds(0));
	}

	void DoAllThingsForSingleThreadedMode()
	{
		updateMutex.lock();

		//delete empty nodes
		if (rootNode->needCheckForDeletion)
			DeleteEmptyNodesCheckedForDeletionRecursive(rootNode);

		//rebuild tree if need
		CheckForRebuildTree();

		updateMutex.unlock();
	}

	void BackgroundThreadProcessItem(const ThreadingQueueItem& item)
	{
		auto objectIndex = item.objectIndex;

		switch (item.command)
		{
		case ThreadingQueueItem::CommandEnum::AddObject:
		{
			auto& bounds = item.bounds;
			ObjectData* objectData = objects[objectIndex];
			objectData->groupMask = item.groupMask;// (uint)(1 << group);
			objectData->bounds = bounds;
			objectData->boundsCenter = bounds.getCenter();
			objectData->boundsHalfSize = bounds.getMaximum() - objectData->boundsCenter;

			GetEndNodeRangeByBoundsNotClamped(bounds, objectData->nodeBoundsIndexes);
			objectData->flags = 0;

			totalObjectCount++;

			AddObjectToWorld(objectData);
		}
		break;


		case ThreadingQueueItem::CommandEnum::RemoveObject:
		{
#if _DEBUG
			if (objectIndex < 0 || objectIndex >= objects.size())
				Fatal("OctreeContainer: RemoveObject: Invalid object index. objectIndex < 0 || objectIndex >= objects.size().");
			if (objects[objectIndex]->flags == ObjectData::Flags_Free)
				Fatal("OctreeContainer: RemoveObject: Invalid object index. objects[ objectIndex ]->flags == ObjectData::Flags_Free.");
#endif

			ObjectData* objectData = objects[objectIndex];

			totalObjectCount--;

			RemoveObjectFromWorld(objectData);

			////delete empty nodes
			//if (rootNode->needCheckForDeletion)
			//	DeleteEmptyNodesCheckedForDeletionRecursive(rootNode);

			//free index
			objects[objectIndex]->flags = ObjectData::Flags_Free;
			objectsFreeIndexes.push(objectIndex);
		}
		break;


		case ThreadingQueueItem::CommandEnum::UpdateObjectBounds:
		{
#if _DEBUG
			if (objectIndex < 0 || objectIndex >= objects.size())
				Fatal("OctreeContainer: UpdateObject: Invalid object index. objectIndex < 0 || objectIndex >= objects.size().");
			if (objects[objectIndex]->flags == ObjectData::Flags_Free)
				Fatal("OctreeContainer: UpdateObject: Invalid object index. objects[ objectIndex ]->flags == ObjectData::Flags_Free.");
#endif

			ObjectData* objectData = objects[objectIndex];

			auto& bounds = item.bounds;
			if (objectData->bounds == bounds)
				return;
			//if (objectData->bounds.minimum == boundsMin && objectData->bounds.maximum == boundsMax)
			//	return;

			//BoundsD bounds(boundsMin, boundsMax);

			BoundsI newNodeBoundsIndexes;
			GetEndNodeRangeByBoundsNotClamped(bounds, newNodeBoundsIndexes);
			bool equalNodeIndexes = objectData->nodeBoundsIndexes == newNodeBoundsIndexes;

			if (!equalNodeIndexes)
				RemoveObjectFromWorld(objectData);

			//objectData->groupMask = groupMask;// (uint)(1 << group);
			objectData->bounds = bounds;
			objectData->boundsCenter = bounds.getCenter();
			objectData->boundsHalfSize = bounds.getMaximum() - objectData->boundsCenter;
			objectData->nodeBoundsIndexes = newNodeBoundsIndexes;

			if (!equalNodeIndexes)
				AddObjectToWorld(objectData);

			////delete empty nodes
			//if (rootNode->needCheckForDeletion)
			//	DeleteEmptyNodesCheckedForDeletionRecursive(rootNode);
		}
		break;


		case ThreadingQueueItem::CommandEnum::UpdateObjectGroupMask:
		{
#if _DEBUG
			if (objectIndex < 0 || objectIndex >= objects.size())
				Fatal("OctreeContainer: UpdateObject: Invalid object index. objectIndex < 0 || objectIndex >= objects.size().");
			if (objects[objectIndex]->flags == ObjectData::Flags_Free)
				Fatal("OctreeContainer: UpdateObject: Invalid object index. objects[ objectIndex ]->flags == ObjectData::Flags_Free.");
#endif

			ObjectData* objectData = objects[objectIndex];
			objectData->groupMask = item.groupMask;
		}
		break;

		}
	}

	void BackgroundThreadFunction()
	{
		try
		{
			while (!backgroundThreadNeedExit.load())
			{
				std::vector<ThreadingQueueItem> items;

				//!!!!?
				//threadingModeMutex.try_lock()

				threadingModeMutex.lock();
				if (threadingQueue.size() != 0)
				{
					backgroundThreadInWork = true;

					items.reserve(threadingQueue.size());
					while (threadingQueue.size() != 0)
					{
						items.push_back(threadingQueue.front());
						threadingQueue.pop();
					}
				}
				threadingModeMutex.unlock();

				if (items.size() != 0)
				{
					for (int n = 0; n < items.size(); n++)
						BackgroundThreadProcessItem(items[n]);

					//post process actions

					//delete empty nodes
					if (rootNode->needCheckForDeletion)
						DeleteEmptyNodesCheckedForDeletionRecursive(rootNode);

					//rebuild tree if need
					CheckForRebuildTree();

					backgroundThreadInWork = false;
				}

				std::this_thread::sleep_for(std::chrono::milliseconds(0));


			//	bool itemProcessedInThisUpdate = false;

			//again:;
			//	bool itemFound = false;
			//	ThreadingQueueItem item;


			//	//!!!!get all items with one lock

			//	//!!!!?
			//	//threadingModeMutex.try_lock()

			//	threadingModeMutex.lock();
			//	if (threadingQueue.size() != 0)
			//	{
			//		backgroundThreadInWork = true;
			//		itemFound = true;
			//		itemProcessedInThisUpdate = true;
			//		item = threadingQueue.front();
			//		threadingQueue.pop();
			//	}
			//	threadingModeMutex.unlock();

			//	if (itemFound)
			//	{
			//		BackgroundThreadProcessItem(item);
			//		goto again;
			//	}

			//	//post process actions
			//	if (itemProcessedInThisUpdate)
			//	{
			//		//delete empty nodes
			//		if (rootNode->needCheckForDeletion)
			//			DeleteEmptyNodesCheckedForDeletionRecursive(rootNode);

			//		//rebuild tree if need
			//		CheckForRebuildTree();
			//	}

			//	backgroundThreadInWork = false;

			//	std::this_thread::sleep_for(std::chrono::milliseconds(0));
			}
		}
		catch(...)
		{
			Fatal("OctreeContainer: BackgroundThreadFunction: Exception.");
		}
	}

};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

OctreeContainer::Node::Node( OctreeContainer* owner, Node* parent, const BoundsI& indexBounds )
{
	this->owner = owner;
	this->parent = parent;
	this->indexBounds = indexBounds;
	endNode = false;
	needCreateChildrenNodes = false;
	childrenCount = 0;
	memset( childrenIndexBounds, 0, sizeof( childrenIndexBounds ) );
	memset( childrenBounds, 0, sizeof( childrenBounds ) );
	memset( children, 0, sizeof( children ) );
	needCheckForDeletion = false;

	Vector3I size = indexBounds.getSize();
	if( size.x <= 0 || size.y <= 0 || size.z <= 0 )
		Fatal( "OctreeContainer: Node: Constructor: size.x <= 0 || size.y <= 0 || size.z <= 0." );

	bool divX = size.x > 1;
	bool divY = size.y > 1;
	bool divZ = size.z > 1;

	if( divX || divY || divZ )
	{
		//calculate childrenCount, childrenIndexBounds

		std::vector<BoundsI> nodesBounds;
		nodesBounds.push_back( indexBounds );

		Vector3I half = indexBounds.getMinimum() + size / 2;

		if( divX )
		{
			std::vector<BoundsI> n;
			n.reserve( nodesBounds.size() * 2 );
			for( int z = 0; z < nodesBounds.size(); z++ )
			{
				BoundsI b = nodesBounds[ z ];
				n.push_back( BoundsI( b.getMinimum(), Vector3I( half.x, b.getMaximum().y, b.getMaximum().z ) ) );
				n.push_back( BoundsI( Vector3I( half.x, b.getMinimum().y, b.getMinimum().z ), b.getMaximum() ) );
			}
			nodesBounds.swap( n );//nodesBounds = n;
		}
		if( divY )
		{
			std::vector<BoundsI> n;
			n.reserve( nodesBounds.size() * 2 );
			for( int z = 0; z < nodesBounds.size(); z++ )
			{
				BoundsI b = nodesBounds[ z ];
				n.push_back( BoundsI( b.getMinimum(), Vector3I( b.getMaximum().x, half.y, b.getMaximum().z ) ) );
				n.push_back( BoundsI( Vector3I( b.getMinimum().x, half.y, b.getMinimum().z ), b.getMaximum() ) );
			}
			nodesBounds.swap( n );//nodesBounds = n;
		}
		if( divZ )
		{
			std::vector<BoundsI> n;
			n.reserve( nodesBounds.size() * 2 );
			for( int z = 0; z < nodesBounds.size(); z++ )
			{
				BoundsI b = nodesBounds[ z ];
				n.push_back( BoundsI( b.getMinimum(), Vector3I( b.getMaximum().x, b.getMaximum().y, half.z ) ) );
				n.push_back( BoundsI( Vector3I( b.getMinimum().x, b.getMinimum().y, half.z ), b.getMaximum() ) );
			}
			nodesBounds.swap( n );//nodesBounds = n;
		}

		childrenCount = nodesBounds.size();
		for( int n = 0; n < childrenCount; n++ )
			childrenIndexBounds[ n ] = nodesBounds[ n ];

		//calculate childrenBounds
		for( int n = 0; n < childrenCount; n++ )
		{
			BoundsI childIndexBounds = childrenIndexBounds[ n ];
			childrenBounds[ n ] = BoundsD(
				owner->nodesBounds.getMinimum() + childIndexBounds.getMinimum().toVector3D() * owner->minNodeSize,
				owner->nodesBounds.getMinimum() + childIndexBounds.getMaximum().toVector3D() * owner->minNodeSize );
		}
	}
	else
	{
		endNode = true;
	}

	bounds = BoundsD(
		owner->nodesBounds.getMinimum() + indexBounds.getMinimum().toVector3D() * owner->minNodeSize,
		owner->nodesBounds.getMinimum() + indexBounds.getMaximum().toVector3D() * owner->minNodeSize );
	boundsCenter = bounds.getCenter();
	boundsHalfSize = bounds.getMaximum() - boundsCenter;

	owner->totalNodeCount++;
}

OctreeContainer::Node::~Node()
{
	owner->totalNodeCount--;

	//if( objectsIntersectsNodeBounds.size() != 0 )
	//	Fatal( "OctreeContainer: OctreeContainer: Node: ~Node: objectsIntersectsNodeBounds.size() != 0." );
	//if( bigObjects.size() != 0 )
	//	Fatal( "OctreeContainer: OctreeContainer: Node: ~Node: bigObjects.size() != 0." );

	for( int n = 0; n < 8; n++ )
	{
		Node* node = children[ n ];
		if( node )
			delete node;
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

int GetObjectsRaySortCompare( const void* a, const void* b )
{
	const OctreeContainer::GetObjectsRayOutputData& objectData1 = *(OctreeContainer::GetObjectsRayOutputData*)a;
	const OctreeContainer::GetObjectsRayOutputData& objectData2 = *(OctreeContainer::GetObjectsRayOutputData*)b;
	if( objectData1.distanceNormalized < objectData2.distanceNormalized )
		return -1;
	if( objectData1.distanceNormalized > objectData2.distanceNormalized )
		return 1;
	return 0;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

EXPORT OctreeContainer* OctreeContainer_New( const BoundsD& initialOctreeBounds, int amountOfObjectsOutsideOctreeBoundsToRebuld, const Vector3D& octreeBoundsRebuildExpand, const Vector3D& minNodeSize, int objectCountThresholdToCreateChildNodes, int maxNodeCount, ThreadingModeEnum threadingMode, int getObjectsInputDataSize, double engineTimeToGetStatistics)
{
	return new OctreeContainer(initialOctreeBounds, amountOfObjectsOutsideOctreeBoundsToRebuld, octreeBoundsRebuildExpand, minNodeSize, objectCountThresholdToCreateChildNodes, maxNodeCount, threadingMode, getObjectsInputDataSize, engineTimeToGetStatistics);
}

EXPORT void OctreeContainer_Delete( OctreeContainer* container )
{
	//if (container->threadingMode == ThreadingModeEnum::SingleThreaded)
	//	container->DoAllThingsForSingleThreadedMode();
	//else
	//	container->WaitForExecuteAllBackgroundTasks();

	delete container;
}

//EXPORT void OctreeContainer_UpdateSettings( OctreeContainer* container, int amountOfObjectsOutsideOctreeBoundsToRebuld, const Vector3D& octreeBoundsRebuildExpand, const Vector3D& minNodeSize, int objectCountThresholdToCreateChildNodes, int maxNodeCount, ThreadingModeEnum threadingMode)//, bool forceTreeRebuild )
//{
//	container->updateMutex.lock();
//	container->UpdateSettings(amountOfObjectsOutsideOctreeBoundsToRebuld, octreeBoundsRebuildExpand, minNodeSize, objectCountThresholdToCreateChildNodes, maxNodeCount, threadingMode);// , forceTreeRebuild);
//	container->updateMutex.unlock();
//}

//EXPORT void OctreeContainer_RebuildTree( OctreeContainer* container )
//{
//	container->updateMutex.lock();
//	container->RebuildTree( false );
//	container->updateMutex.unlock();
//}

EXPORT int OctreeContainer_AddObject( OctreeContainer* container, const Vector3D& boundsMin, const Vector3D& boundsMax, uint groupMask )
{
	auto result = container->AddObject(boundsMin, boundsMax, groupMask);
	return result;
}

EXPORT void OctreeContainer_RemoveObject( OctreeContainer* container, int objectIndex )
{
	container->RemoveObject( objectIndex );
}

EXPORT void OctreeContainer_UpdateObjectBounds(OctreeContainer* container, int objectIndex, const Vector3D& boundsMin, const Vector3D& boundsMax)
{
	container->UpdateObjectBounds(objectIndex, boundsMin, boundsMax);
}

EXPORT void OctreeContainer_UpdateObjectGroupMask(OctreeContainer* container, int objectIndex, uint groupMask)
{
	container->UpdateObjectGroupMask(objectIndex, groupMask);
}

//it called from multiple threads
EXPORT int OctreeContainer_GetObjects( OctreeContainer* container, const OctreeContainer::GetObjectsInputData& inputData, int* outputArray, int outputArraySize )
{
	if (container->threadingMode == ThreadingModeEnum::SingleThreaded)
		container->DoAllThingsForSingleThreadedMode();
	else
		container->WaitForExecuteAllBackgroundTasks();

	auto result = container->GetObjects(inputData, outputArray, outputArraySize);
	return result;
}

//it called from multiple threads
EXPORT int OctreeContainer_GetObjectsRay( OctreeContainer* container, const OctreeContainer::GetObjectsInputData& inputData, OctreeContainer::GetObjectsRayOutputData* outputArray, int outputArraySize )
{
	if (container->threadingMode == ThreadingModeEnum::SingleThreaded)
		container->DoAllThingsForSingleThreadedMode();
	else
		container->WaitForExecuteAllBackgroundTasks();

	auto result = container->GetObjectsRay(inputData, outputArray, outputArraySize);
	return result;
}

EXPORT void OctreeContainer_GetDebugRenderLines( OctreeContainer* container, void** outputData, int* outputDataItemCount )
{
	if (container->threadingMode == ThreadingModeEnum::SingleThreaded)
		container->DoAllThingsForSingleThreadedMode();
	else
		container->WaitForExecuteAllBackgroundTasks();

	container->GetDebugRenderLines( outputData, outputDataItemCount );
}

EXPORT void OctreeContainer_Free( OctreeContainer* container, void* data )
{
	uint8* pointer = (uint8*)data;
	delete[] pointer;
}

EXPORT void OctreeContainer_GetStatistics( OctreeContainer* container, int* objectCount, BoundsD* octreeBounds, int* octreeNodeCount, double* timeSinceLastFullRebuild)
{
	if (container->threadingMode == ThreadingModeEnum::SingleThreaded)
		container->DoAllThingsForSingleThreadedMode();
	else
		container->WaitForExecuteAllBackgroundTasks();

	container->GetStatistics(objectCount, octreeBounds, octreeNodeCount, timeSinceLastFullRebuild);
}

EXPORT void OctreeContainer_GetOctreeBoundsWithBoundsOfObjectsOutsideOctree( OctreeContainer* container, BoundsD& bounds )
{
	if (container->threadingMode == ThreadingModeEnum::SingleThreaded)
		container->DoAllThingsForSingleThreadedMode();
	else
		container->WaitForExecuteAllBackgroundTasks();

	container->GetOctreeBoundsWithBoundsOfObjectsOutsideOctree( bounds );
}

EXPORT void OctreeContainer_SetEngineTimeToGetStatistics(OctreeContainer* container, double engineTime)
{
	container->engineTimeToGetStatistics = engineTime;
}
