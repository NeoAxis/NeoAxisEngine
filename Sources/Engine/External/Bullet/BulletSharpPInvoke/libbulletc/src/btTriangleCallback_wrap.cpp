#include <BulletCollision/CollisionShapes/btTriangleCallback.h>

#include "conversion.h"
#include "btTriangleCallback_wrap.h"

btInternalTriangleIndexCallbackWrapper::btInternalTriangleIndexCallbackWrapper(p_btInternalTriangleIndexCallback_internalProcessTriangleIndex internalProcessTriangleIndexCallback)
{
	_internalProcessTriangleIndexCallback = internalProcessTriangleIndexCallback;
}

void btInternalTriangleIndexCallbackWrapper::internalProcessTriangleIndex(btVector3* triangle,
	int partId, int triangleIndex)
{
	_internalProcessTriangleIndexCallback(triangle, partId, triangleIndex);
}


btTriangleCallbackWrapper::btTriangleCallbackWrapper(p_btTriangleCallback_processTriangle processTriangleCallback)
{
	_processTriangleCallback = processTriangleCallback;
}

void btTriangleCallbackWrapper::processTriangle(btVector3* triangle, int partId,
	int triangleIndex)
{
	_processTriangleCallback(triangle, partId, triangleIndex);
}


btTriangleCallbackWrapper* btTriangleCallbackWrapper_new(p_btTriangleCallback_processTriangle processTriangleCallback)
{
	return new btTriangleCallbackWrapper(processTriangleCallback);
}

void btTriangleCallback_delete(btTriangleCallback* obj)
{
	delete obj;
}


btInternalTriangleIndexCallbackWrapper* btInternalTriangleIndexCallbackWrapper_new(
	p_btInternalTriangleIndexCallback_internalProcessTriangleIndex internalProcessTriangleIndexCallback)
{
	return new btInternalTriangleIndexCallbackWrapper(internalProcessTriangleIndexCallback);
}

void btInternalTriangleIndexCallback_delete(btInternalTriangleIndexCallback* obj)
{
	delete obj;
}
