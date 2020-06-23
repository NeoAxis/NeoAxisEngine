#include "main.h"

#ifndef BT_MOTIONSTATE_H
#define p_btMotionState_getWorldTransform void*
#define p_btMotionState_setWorldTransform void*
#define btMotionStateWrapper void
#else
typedef void (*p_btMotionState_getWorldTransform)(btTransform* worldTrans);
typedef void (*p_btMotionState_setWorldTransform)(const btTransform* worldTrans);

class btMotionStateWrapper : public btMotionState
{
private:
	p_btMotionState_getWorldTransform _getWorldTransformCallback;
	p_btMotionState_setWorldTransform _setWorldTransformCallback;

public:
	btMotionStateWrapper(p_btMotionState_getWorldTransform getWorldTransformCallback,
		p_btMotionState_setWorldTransform setWorldTransformCallback);

	virtual void getWorldTransform(btTransform& worldTrans) const;
	virtual void setWorldTransform(const btTransform& worldTrans);
};
#endif

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btMotionStateWrapper* btMotionStateWrapper_new(p_btMotionState_getWorldTransform getWorldTransformCallback,
		p_btMotionState_setWorldTransform setWorldTransformCallback);

	EXPORT void btMotionState_getWorldTransform(btMotionState* obj, btTransform* worldTrans);
	EXPORT void btMotionState_setWorldTransform(btMotionState* obj, const btTransform* worldTrans);
	EXPORT void btMotionState_delete(btMotionState* obj);
#ifdef __cplusplus
}
#endif
