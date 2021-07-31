#include "main.h"

#ifndef BT_IDEBUG_DRAW__H
#define p_btIDebugDraw_drawAabb void*
#define p_btIDebugDraw_drawArc void*
#define p_btIDebugDraw_drawBox void*
#define p_btIDebugDraw_drawCapsule void*
#define p_btIDebugDraw_drawCone void*
#define p_btIDebugDraw_drawContactPoint void*
#define p_btIDebugDraw_drawCylinder void*
#define p_btIDebugDraw_drawLine void*
#define p_btIDebugDraw_drawPlane void*
#define p_btIDebugDraw_drawSphere void*
#define p_btIDebugDraw_drawSpherePatch void*
#define p_btIDebugDraw_drawTransform void*
#define p_btIDebugDraw_drawTriangle void*
#define p_btIDebugDraw_getDebugMode void*
#define pSimpleCallback void*

#define btIDebugDrawWrapper void
#else
typedef void (*p_btIDebugDraw_drawAabb)(const btVector3* from, const btVector3* to,
	const btVector3* color);
typedef void (*p_btIDebugDraw_drawArc)(const btVector3* center, const btVector3* normal,
	const btVector3* axis, btScalar radiusA, btScalar radiusB, btScalar minAngle, btScalar maxAngle,
	const btVector3* color, bool drawSect, btScalar stepDegrees);
typedef void (*p_btIDebugDraw_drawBox)(const btVector3* bbMin, const btVector3* bbMax,
	const btTransform* trans, const btVector3* color);
typedef void (*p_btIDebugDraw_drawCapsule)(btScalar radius, btScalar halfHeight,
	int upAxis, const btTransform* transform, const btVector3* color);
typedef void (*p_btIDebugDraw_drawCone)(btScalar radius, btScalar height, int upAxis,
	const btTransform* transform, const btVector3* color);
typedef void (*p_btIDebugDraw_drawContactPoint)(const btVector3* PointOnB, const btVector3* normalOnB,
	btScalar distance, int lifeTime, const btVector3* color);
typedef void (*p_btIDebugDraw_drawCylinder)(btScalar radius, btScalar halfHeight,
	int upAxis, const btTransform* transform, const btVector3* color);
typedef void (*p_btIDebugDraw_drawLine)(const btVector3* from, const btVector3* to,
	const btVector3* color);
typedef void (*p_btIDebugDraw_drawPlane)(const btVector3* planeNormal, btScalar planeConst,
	const btTransform* transform, const btVector3* color);
typedef void (*p_btIDebugDraw_drawSphere)(btScalar radius, const btTransform* transform,
	const btVector3* color);
typedef void (*p_btIDebugDraw_drawSpherePatch)(const btVector3* center, const btVector3* up,
	const btVector3* axis, btScalar radius, btScalar minTh, btScalar maxTh, btScalar minPs,
	btScalar maxPs, const btVector3* color, btScalar stepDegrees);
typedef void (*p_btIDebugDraw_drawTransform)(const btTransform* transform, btScalar orthoLen);
typedef void (*p_btIDebugDraw_drawTriangle)(const btVector3* v0, const btVector3* v1,
	const btVector3* v2, const btVector3* color, btScalar __unnamed4);
typedef int (*p_btIDebugDraw_getDebugMode)();
typedef void (*pSimpleCallback)(int x);

class btIDebugDrawWrapper : public btIDebugDraw
{
private:
	p_btIDebugDraw_drawAabb _drawAabbCallback;
	p_btIDebugDraw_drawArc _drawArcCallback;
	p_btIDebugDraw_drawBox _drawBoxCallback;
	p_btIDebugDraw_drawCapsule _drawCapsuleCallback;
	p_btIDebugDraw_drawCone _drawConeCallback;
	p_btIDebugDraw_drawContactPoint _drawContactPointCallback;
	p_btIDebugDraw_drawCylinder _drawCylinderCallback;
	p_btIDebugDraw_drawLine _drawLineCallback;
	p_btIDebugDraw_drawPlane _drawPlaneCallback;
	p_btIDebugDraw_drawSphere _drawSphereCallback;
	p_btIDebugDraw_drawSpherePatch _drawSpherePatchCallback;
	p_btIDebugDraw_drawTransform _drawTransformCallback;
	p_btIDebugDraw_drawTriangle _drawTriangleCallback;
	p_btIDebugDraw_getDebugMode _getDebugModeCallback;

public:
	void* _debugDrawGCHandle;
	void* getGCHandle();

	pSimpleCallback _cb;

	btIDebugDrawWrapper(void* debugDrawGCHandle,
		p_btIDebugDraw_drawAabb drawAabbCallback, p_btIDebugDraw_drawArc drawArcCallback,
		p_btIDebugDraw_drawBox drawBoxCallback, p_btIDebugDraw_drawCapsule drawCapsuleCallback,
		p_btIDebugDraw_drawCone drawConeCallback, p_btIDebugDraw_drawContactPoint drawContactPointCallback,
		p_btIDebugDraw_drawCylinder drawCylinderCallback, p_btIDebugDraw_drawLine drawLineCallback,
		p_btIDebugDraw_drawPlane drawPlaneCallback, p_btIDebugDraw_drawSphere drawSphereCallback,
		p_btIDebugDraw_drawSpherePatch drawSpherePatchCallback, p_btIDebugDraw_drawTransform drawTransformCallback,
		p_btIDebugDraw_drawTriangle drawTriangleCallback, p_btIDebugDraw_getDebugMode getDebugModeCallback,
		pSimpleCallback cb);

	virtual void draw3dText(const btVector3& location, const char* textString);
	virtual void drawAabb(const btVector3& from, const btVector3& to, const btVector3& color);
	virtual void drawArc(const btVector3& center, const btVector3& normal, const btVector3& axis,
		btScalar radiusA, btScalar radiusB, btScalar minAngle, btScalar maxAngle, const btVector3& color,
		bool drawSect, btScalar stepDegrees);
	virtual void drawArc(const btVector3& center, const btVector3& normal, const btVector3& axis,
		btScalar radiusA, btScalar radiusB, btScalar minAngle, btScalar maxAngle,
		const btVector3& color, bool drawSect);
	virtual void drawBox(const btVector3& bbMin, const btVector3& bbMax, const btVector3& color);
	virtual void drawBox(const btVector3& bbMin, const btVector3& bbMax, const btTransform& trans,
		const btVector3& color);
	virtual void drawCapsule(btScalar radius, btScalar halfHeight, int upAxis, const btTransform& transform,
		const btVector3& color);
	virtual void drawCone(btScalar radius, btScalar height, int upAxis, const btTransform& transform,
		const btVector3& color);
	virtual void drawContactPoint(const btVector3& PointOnB, const btVector3& normalOnB,
		btScalar distance, int lifeTime, const btVector3& color);
	virtual void drawCylinder(btScalar radius, btScalar halfHeight, int upAxis, const btTransform& transform,
		const btVector3& color);
	virtual void drawLine(const btVector3& from, const btVector3& to, const btVector3& color);
	virtual void drawPlane(const btVector3& planeNormal, btScalar planeConst, const btTransform& transform,
		const btVector3& color);
	virtual void drawSphere(const btVector3& p, btScalar radius, const btVector3& color);
	virtual void drawSphere(btScalar radius, const btTransform& transform, const btVector3& color);
	virtual void drawSpherePatch(const btVector3& center, const btVector3& up, const btVector3& axis,
		btScalar radius, btScalar minTh, btScalar maxTh, btScalar minPs, btScalar maxPs,
		const btVector3& color, btScalar stepDegrees);
	virtual void drawSpherePatch(const btVector3& center, const btVector3& up, const btVector3& axis, btScalar radius,
		btScalar minTh, btScalar maxTh, btScalar minPs, btScalar maxPs, const btVector3& color);
	virtual void drawTransform(const btTransform& transform, btScalar orthoLen);
	virtual void drawTriangle(const btVector3& v0, const btVector3& v1, const btVector3& v2,
		const btVector3& color, btScalar __unnamed4);
	virtual void drawTriangle(const btVector3& v0, const btVector3& v1, const btVector3& v2,
		const btVector3&, const btVector3&, const btVector3&, const btVector3& color, btScalar alpha);

	virtual void baseDrawAabb(const btVector3& from, const btVector3& to, const btVector3& color);
	virtual void baseDrawCone(btScalar radius, btScalar height, int upAxis, const btTransform& transform, const btVector3& color);
	virtual void baseDrawCylinder(btScalar radius, btScalar halfHeight, int upAxis, const btTransform& transform, const btVector3& color);
	virtual void baseDrawSphere(const btVector3& p, btScalar radius, const btVector3& color);
	virtual void baseDrawTriangle(const btVector3& v0, const btVector3& v1, const btVector3& v2, const btVector3& color, btScalar);
	virtual void baseDrawTriangle(const btVector3& v0, const btVector3& v1, const btVector3& v2,
		const btVector3&, const btVector3&, const btVector3&, const btVector3& color, btScalar alpha);

	virtual void reportErrorWarning(const char* warningString);

	virtual void setDebugMode(int debugMode);
	virtual int	getDebugMode() const;

	// Never called from Bullet
	//virtual void drawLine(const btVector3& from, const btVector3& to, const btVector3& fromColor,
	//	const btVector3& toColor);
};
#endif

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btIDebugDrawWrapper* btIDebugDrawWrapper_new(void* debugDrawGCHandle,
		p_btIDebugDraw_drawAabb drawAabbCallback, p_btIDebugDraw_drawArc drawArcCallback,
		p_btIDebugDraw_drawBox drawBoxCallback,
		p_btIDebugDraw_drawCapsule drawCapsuleCallback, p_btIDebugDraw_drawCone drawConeCallback,
		p_btIDebugDraw_drawContactPoint drawContactPointCallback, p_btIDebugDraw_drawCylinder drawCylinderCallback,
		p_btIDebugDraw_drawLine drawLineCallback,
		p_btIDebugDraw_drawPlane drawPlaneCallback, p_btIDebugDraw_drawSphere drawSphereCallback,
		p_btIDebugDraw_drawSpherePatch drawSpherePatchCallback,
		p_btIDebugDraw_drawTransform drawTransformCallback, p_btIDebugDraw_drawTriangle drawTriangleCallback,
		p_btIDebugDraw_getDebugMode getDebugModeCallback, pSimpleCallback cb);
	EXPORT void* btIDebugDrawWrapper_getGCHandle(btIDebugDrawWrapper* obj);

	EXPORT void btIDebugDraw_delete(btIDebugDraw* obj);
#ifdef __cplusplus
}
#endif
