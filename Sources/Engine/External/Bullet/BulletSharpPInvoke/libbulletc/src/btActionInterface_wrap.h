#include "main.h"

#ifndef _BT_ACTION_INTERFACE_H
#define p_btActionInterface_debugDraw void*
#define p_btActionInterface_updateAction void*
#define btActionInterfaceWrapper void
#else
typedef void (*p_btActionInterface_debugDraw)(btIDebugDraw* debugDrawer);
typedef void (*p_btActionInterface_updateAction)(btCollisionWorld* collisionWorld,
	btScalar deltaTimeStep);

class btActionInterfaceWrapper : public btActionInterface
{
private:
	p_btActionInterface_debugDraw _debugDrawCallback;
	p_btActionInterface_updateAction _updateActionCallback;

public:
	btActionInterfaceWrapper(p_btActionInterface_debugDraw debugDrawCallback, p_btActionInterface_updateAction updateActionCallback);

	virtual void debugDraw(btIDebugDraw* debugDrawer);
	virtual void updateAction(btCollisionWorld* collisionWorld, btScalar deltaTimeStep);
};
#endif

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btActionInterfaceWrapper* btActionInterfaceWrapper_new(p_btActionInterface_debugDraw debugDrawCallback,
		p_btActionInterface_updateAction updateActionCallback);

	EXPORT void btActionInterface_debugDraw(btActionInterface* obj, btIDebugDraw* debugDrawer);
	EXPORT void btActionInterface_updateAction(btActionInterface* obj, btCollisionWorld* collisionWorld, btScalar deltaTimeStep);
	EXPORT void btActionInterface_delete(btActionInterface* obj);
#ifdef __cplusplus
}
#endif
