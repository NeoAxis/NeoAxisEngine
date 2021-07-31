#include <BulletSoftBody/btDefaultSoftBodySolver.h>
#include <BulletSoftBody/btSoftBody.h>

#include "btDefaultSoftBodySolver_wrap.h"

btDefaultSoftBodySolver* btDefaultSoftBodySolver_new()
{
	return new btDefaultSoftBodySolver();
}

void btDefaultSoftBodySolver_copySoftBodyToVertexBuffer(btDefaultSoftBodySolver* obj,
	const btSoftBody* softBody, btVertexBufferDescriptor* vertexBuffer)
{
	obj->copySoftBodyToVertexBuffer(softBody, vertexBuffer);
}
