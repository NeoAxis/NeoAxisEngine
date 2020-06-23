#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btDefaultSoftBodySolver* btDefaultSoftBodySolver_new();
	EXPORT void btDefaultSoftBodySolver_copySoftBodyToVertexBuffer(btDefaultSoftBodySolver* obj, const btSoftBody* softBody, btVertexBufferDescriptor* vertexBuffer);
#ifdef __cplusplus
}
#endif
