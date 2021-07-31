#include <BulletCollision/Gimpact/btCompoundFromGimpact.h>

#include "btCompoundFromGimpact_wrap.h"

btCompoundShape* btCompoundFromGImpact_btCreateCompoundFromGimpactShape(const btGImpactMeshShape* gimpactMesh, btScalar depth)
{
	return btCreateCompoundFromGimpactShape(gimpactMesh, depth);
}
