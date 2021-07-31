#include <BulletDynamics/Featherstone/btMultiBodyLinkCollider.h>

#include "btMultiBodyLinkCollider_wrap.h"

btMultiBodyLinkCollider* btMultiBodyLinkCollider_new(btMultiBody* multiBody, int link)
{
	return new btMultiBodyLinkCollider(multiBody, link);
}

int btMultiBodyLinkCollider_getLink(btMultiBodyLinkCollider* obj)
{
	return obj->m_link;
}

btMultiBody* btMultiBodyLinkCollider_getMultiBody(btMultiBodyLinkCollider* obj)
{
	return obj->m_multiBody;
}

void btMultiBodyLinkCollider_setLink(btMultiBodyLinkCollider* obj, int value)
{
	obj->m_link = value;
}

void btMultiBodyLinkCollider_setMultiBody(btMultiBodyLinkCollider* obj, btMultiBody* value)
{
	obj->m_multiBody = value;
}

btMultiBodyLinkCollider* btMultiBodyLinkCollider_upcast(btCollisionObject* colObj)
{
	return btMultiBodyLinkCollider::upcast(colObj);
}
