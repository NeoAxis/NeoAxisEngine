#include <BulletCollision/CollisionShapes/btEmptyShape.h>

#include "btEmptyShape_wrap.h"

btEmptyShape* btEmptyShape_new()
{
	return new btEmptyShape();
}
