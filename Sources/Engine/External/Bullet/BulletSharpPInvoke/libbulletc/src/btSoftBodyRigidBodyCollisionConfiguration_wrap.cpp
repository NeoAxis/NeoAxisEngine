#include <BulletSoftBody/btSoftBodyRigidBodyCollisionConfiguration.h>

#include "btSoftBodyRigidBodyCollisionConfiguration_wrap.h"

btSoftBodyRigidBodyCollisionConfiguration* btSoftBodyRigidBodyCollisionConfiguration_new()
{
	return new btSoftBodyRigidBodyCollisionConfiguration();
}

btSoftBodyRigidBodyCollisionConfiguration* btSoftBodyRigidBodyCollisionConfiguration_new2(
	const btDefaultCollisionConstructionInfo* constructionInfo)
{
	return new btSoftBodyRigidBodyCollisionConfiguration(*constructionInfo);
}
