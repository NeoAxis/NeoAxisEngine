#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btBroadphasePair* btAlignedObjectArray_btBroadphasePair_at(btAlignedObjectArray_btBroadphasePair* obj, int n);
	EXPORT void btAlignedObjectArray_btBroadphasePair_push_back(btAlignedObjectArray_btBroadphasePair* obj, btBroadphasePair* val);
	EXPORT void btAlignedObjectArray_btBroadphasePair_resizeNoInitialize(btAlignedObjectArray_btBroadphasePair* obj, int newSize);
	EXPORT int btAlignedObjectArray_btBroadphasePair_size(btAlignedObjectArray_btBroadphasePair* obj);

	EXPORT btCollisionObject* btAlignedObjectArray_btCollisionObjectPtr_at(btAlignedObjectArray_btCollisionObjectPtr* obj, int n);
	EXPORT int btAlignedObjectArray_btCollisionObjectPtr_findLinearSearch2(btAlignedObjectArray_btCollisionObjectPtr* obj, btCollisionObject* key);
	EXPORT void btAlignedObjectArray_btCollisionObjectPtr_push_back(btAlignedObjectArray_btCollisionObjectPtr* obj, btCollisionObject* val);
	EXPORT void btAlignedObjectArray_btCollisionObjectPtr_resizeNoInitialize(btAlignedObjectArray_btCollisionObjectPtr* obj, int newSize);
	EXPORT int btAlignedObjectArray_btCollisionObjectPtr_size(btAlignedObjectArray_btCollisionObjectPtr* obj);

	EXPORT btIndexedMesh* btAlignedObjectArray_btIndexedMesh_at(btAlignedObjectArray_btIndexedMesh* obj, int n);
	EXPORT void btAlignedObjectArray_btIndexedMesh_push_back(btAlignedObjectArray_btIndexedMesh* obj, btIndexedMesh* val);
	EXPORT void btAlignedObjectArray_btIndexedMesh_resizeNoInitialize(btAlignedObjectArray_btIndexedMesh* obj, int newSize);
	EXPORT int btAlignedObjectArray_btIndexedMesh_size(btAlignedObjectArray_btIndexedMesh* obj);

	EXPORT btAlignedObjectArray_btPersistentManifoldPtr* btAlignedObjectArray_btPersistentManifoldPtr_new();
	EXPORT btPersistentManifold* btAlignedObjectArray_btPersistentManifoldPtr_at(btAlignedObjectArray_btPersistentManifoldPtr* obj, int n);
	EXPORT void btAlignedObjectArray_btPersistentManifoldPtr_push_back(btAlignedObjectArray_btPersistentManifoldPtr* obj, btPersistentManifold* val);
	EXPORT void btAlignedObjectArray_btPersistentManifoldPtr_resizeNoInitialize(btAlignedObjectArray_btPersistentManifoldPtr* obj, int newSize);
	EXPORT int btAlignedObjectArray_btPersistentManifoldPtr_size(btAlignedObjectArray_btPersistentManifoldPtr* obj);
	EXPORT void btAlignedObjectArray_btPersistentManifoldPtr_delete(btAlignedObjectArray_btPersistentManifoldPtr* obj);

	EXPORT btSoftBody* btAlignedObjectArray_btSoftBodyPtr_at(btAlignedObjectArray_btSoftBodyPtr* obj, int n);
	EXPORT void btAlignedObjectArray_btSoftBodyPtr_push_back(btAlignedObjectArray_btSoftBodyPtr* obj, btSoftBody* val);
	EXPORT void btAlignedObjectArray_btSoftBodyPtr_resizeNoInitialize(btAlignedObjectArray_btSoftBodyPtr* obj, int newSize);
	EXPORT int btAlignedObjectArray_btSoftBodyPtr_size(btAlignedObjectArray_btSoftBodyPtr* obj);

	EXPORT btSoftBody_Anchor* btAlignedObjectArray_btSoftBody_Anchor_at(btAlignedObjectArray_btSoftBody_Anchor* obj, int n);
	EXPORT void btAlignedObjectArray_btSoftBody_Anchor_push_back(btAlignedObjectArray_btSoftBody_Anchor* obj, btSoftBody_Anchor* val);
	EXPORT void btAlignedObjectArray_btSoftBody_Anchor_resizeNoInitialize(btAlignedObjectArray_btSoftBody_Anchor* obj, int newSize);
	EXPORT int btAlignedObjectArray_btSoftBody_Anchor_size(btAlignedObjectArray_btSoftBody_Anchor* obj);

	EXPORT btSoftBody_Cluster* btAlignedObjectArray_btSoftBody_ClusterPtr_at(btAlignedObjectArray_btSoftBody_ClusterPtr* obj, int n);
	EXPORT void btAlignedObjectArray_btSoftBody_ClusterPtr_push_back(btAlignedObjectArray_btSoftBody_ClusterPtr* obj, btSoftBody_Cluster* val);
	EXPORT void btAlignedObjectArray_btSoftBody_ClusterPtr_resizeNoInitialize(btAlignedObjectArray_btSoftBody_ClusterPtr* obj, int newSize);
	EXPORT int btAlignedObjectArray_btSoftBody_ClusterPtr_size(btAlignedObjectArray_btSoftBody_ClusterPtr* obj);

	EXPORT btSoftBody_Face* btAlignedObjectArray_btSoftBody_Face_at(btAlignedObjectArray_btSoftBody_Face* obj, int n);
	EXPORT void btAlignedObjectArray_btSoftBody_Face_push_back(btAlignedObjectArray_btSoftBody_Face* obj, btSoftBody_Face* val);
	EXPORT void btAlignedObjectArray_btSoftBody_Face_resizeNoInitialize(btAlignedObjectArray_btSoftBody_Face* obj, int newSize);
	EXPORT int btAlignedObjectArray_btSoftBody_Face_size(btAlignedObjectArray_btSoftBody_Face* obj);

	EXPORT btSoftBody_Joint* btAlignedObjectArray_btSoftBody_JointPtr_at(btAlignedObjectArray_btSoftBody_JointPtr** obj, int n);
	EXPORT void btAlignedObjectArray_btSoftBody_JointPtr_push_back(btAlignedObjectArray_btSoftBody_JointPtr** obj, btSoftBody_Joint* val);
	EXPORT void btAlignedObjectArray_btSoftBody_JointPtr_resizeNoInitialize(btAlignedObjectArray_btSoftBody_JointPtr** obj, int newSize);
	EXPORT int btAlignedObjectArray_btSoftBody_JointPtr_size(btAlignedObjectArray_btSoftBody_JointPtr** obj);

	EXPORT btSoftBody_Link* btAlignedObjectArray_btSoftBody_Link_at(btAlignedObjectArray_btSoftBody_Link* obj, int n);
	EXPORT void btAlignedObjectArray_btSoftBody_Link_push_back(btAlignedObjectArray_btSoftBody_Link* obj, btSoftBody_Link* val);
	EXPORT void btAlignedObjectArray_btSoftBody_Link_resizeNoInitialize(btAlignedObjectArray_btSoftBody_Link* obj, int newSize);
	EXPORT void btAlignedObjectArray_btSoftBody_Link_set(btAlignedObjectArray_btSoftBody_Link* obj, btSoftBody_Link* val, int index);
	EXPORT int btAlignedObjectArray_btSoftBody_Link_size(btAlignedObjectArray_btSoftBody_Link* obj);

	EXPORT btSoftBody_Material* btAlignedObjectArray_btSoftBody_MaterialPtr_at(btAlignedObjectArray_btSoftBody_MaterialPtr* obj, int n);
	EXPORT void btAlignedObjectArray_btSoftBody_MaterialPtr_push_back(btAlignedObjectArray_btSoftBody_MaterialPtr* obj, btSoftBody_Material* val);
	EXPORT void btAlignedObjectArray_btSoftBody_MaterialPtr_resizeNoInitialize(btAlignedObjectArray_btSoftBody_MaterialPtr* obj, int newSize);
	EXPORT int btAlignedObjectArray_btSoftBody_MaterialPtr_size(btAlignedObjectArray_btSoftBody_MaterialPtr* obj);

	EXPORT btSoftBody_Node* btAlignedObjectArray_btSoftBody_Node_at(btAlignedObjectArray_btSoftBody_Node* obj, int n);
	EXPORT int btAlignedObjectArray_btSoftBody_Node_index_of(btAlignedObjectArray_btSoftBody_Node* obj, btSoftBody_Node* val);
	EXPORT void btAlignedObjectArray_btSoftBody_Node_push_back(btAlignedObjectArray_btSoftBody_Node* obj, btSoftBody_Node* val);
	EXPORT void btAlignedObjectArray_btSoftBody_Node_resizeNoInitialize(btAlignedObjectArray_btSoftBody_Node* obj, int newSize);
	EXPORT int btAlignedObjectArray_btSoftBody_Node_size(btAlignedObjectArray_btSoftBody_Node* obj);

	EXPORT btSoftBody_Note* btAlignedObjectArray_btSoftBody_Note_at(btAlignedObjectArray_btSoftBody_Note* obj, int n);
	EXPORT int btAlignedObjectArray_btSoftBody_Note_size(btAlignedObjectArray_btSoftBody_Note* obj);

	EXPORT btSoftBody_Tetra* btAlignedObjectArray_btSoftBody_Tetra_at(btAlignedObjectArray_btSoftBody_Tetra* obj, int n);
	EXPORT void btAlignedObjectArray_btSoftBody_Tetra_push_back(btAlignedObjectArray_btSoftBody_Tetra* obj, btSoftBody_Tetra* val);
	EXPORT void btAlignedObjectArray_btSoftBody_Tetra_resizeNoInitialize(btAlignedObjectArray_btSoftBody_Tetra* obj, int newSize);
	EXPORT int btAlignedObjectArray_btSoftBody_Tetra_size(btAlignedObjectArray_btSoftBody_Tetra* obj);
#ifdef __cplusplus
}
#endif
