#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btCompoundShapeChild* btCompoundShapeChild_array_at(btCompoundShapeChild* a, int n);
	EXPORT btSoftBody_Node* btSoftBodyNodePtrArray_at(btSoftBodyNodePtrArray* obj, int n);
	EXPORT void btSoftBodyNodePtrArray_set(btSoftBodyNodePtrArray* obj, btSoftBody_Node* value, int index);
	EXPORT void btVector3_array_at(const btVector3* a, int n, btVector3* value);
	EXPORT void btVector3_array_set(btVector3* obj, int n, const btVector3* value);
	EXPORT btAlignedObjectArray_btVector3* btAlignedObjectArray_btVector3_new();
	EXPORT void btAlignedObjectArray_btVector3_at(btAlignedObjectArray_btVector3* obj, int n, btVector3* value);
	EXPORT void btAlignedObjectArray_btVector3_push_back(btAlignedObjectArray_btVector3* obj, const btVector3* value);
	EXPORT void btAlignedObjectArray_btVector3_push_back2(btAlignedObjectArray_btVector3* obj, const btVector4* value);
	EXPORT void btAlignedObjectArray_btVector3_set(btAlignedObjectArray_btVector3* obj, int n, const btVector3* value);
	EXPORT int btAlignedObjectArray_btVector3_size(btAlignedObjectArray_btVector3* obj);
	EXPORT void btAlignedObjectArray_btVector3_delete(btAlignedObjectArray_btVector3* obj);
#ifdef __cplusplus
}
#endif
