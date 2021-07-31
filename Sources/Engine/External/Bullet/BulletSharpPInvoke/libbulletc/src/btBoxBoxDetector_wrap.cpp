#include <BulletCollision/CollisionDispatch/btBoxBoxDetector.h>
#include <BulletCollision/CollisionShapes/btBoxShape.h>

#include "btBoxBoxDetector_wrap.h"

btBoxBoxDetector* btBoxBoxDetector_new(const btBoxShape* box1, const btBoxShape* box2)
{
	return new btBoxBoxDetector(box1, box2);
}

const btBoxShape* btBoxBoxDetector_getBox1(btBoxBoxDetector* obj)
{
	return obj->m_box1;
}

const btBoxShape* btBoxBoxDetector_getBox2(btBoxBoxDetector* obj)
{
	return obj->m_box2;
}

void btBoxBoxDetector_setBox1(btBoxBoxDetector* obj, const btBoxShape* value)
{
	obj->m_box1 = value;
}

void btBoxBoxDetector_setBox2(btBoxBoxDetector* obj, const btBoxShape* value)
{
	obj->m_box2 = value;
}
