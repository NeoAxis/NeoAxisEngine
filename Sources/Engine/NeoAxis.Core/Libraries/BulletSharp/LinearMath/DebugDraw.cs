/*
 * C# / XNA  port of Bullet (c) 2011 Mark Neale <xexuxjy@hotmail.com>
 *
 * Bullet Continuous Collision Detection and Physics Library
 * Copyright (c) 2003-2008 Erwin Coumans  http://www.bulletphysics.com/
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose, 
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *	claim that you wrote the original software. If you use this software
 *	in a product, an acknowledgment in the product documentation would be
 *	appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *	misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System;
using System.Security;
using System.Runtime.InteropServices;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public abstract class DebugDraw : IDebugDraw//, IDisposable
	{
		internal IntPtr _native;

		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawAabbUnmanagedDelegate([In] ref BVector3 from, [In] ref BVector3 to, [In] ref BVector3 color);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawArcUnmanagedDelegate([In] ref BVector3 center, [In] ref BVector3 normal, [In] ref BVector3 axis, double radiusA, double radiusB,
			double minAngle, double maxAngle, ref BVector3 color, bool drawSect, double stepDegrees);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawBoxUnmanagedDelegate([In] ref BVector3 bbMin, [In] ref BVector3 bbMax, [In] ref BMatrix trans, [In] ref BVector3 color);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawCapsuleUnmanagedDelegate(double radius, double halfHeight, int upAxis, [In] ref BMatrix transform, [In] ref BVector3 color);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawConeUnmanagedDelegate(double radius, double height, int upAxis, [In] ref BMatrix transform, [In] ref BVector3 color);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawContactPointUnmanagedDelegate([In] ref BVector3 pointOnB, [In] ref BVector3 normalOnB, double distance, int lifeTime, [In] ref BVector3 color);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawCylinderUnmanagedDelegate(double radius, double halfHeight, int upAxis, [In] ref BMatrix transform, [In] ref BVector3 color);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawLineUnmanagedDelegate([In] ref BVector3 from, [In] ref BVector3 to, [In] ref BVector3 color);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawPlaneUnmanagedDelegate([In] ref BVector3 planeNormal, double planeConst, [In] ref BMatrix transform, [In] ref BVector3 color);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawSphereUnmanagedDelegate(double radius, [In] ref BMatrix transform, [In] ref BVector3 color);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawSpherePatchUnmanagedDelegate([In] ref BVector3 center, [In] ref BVector3 up, [In] ref BVector3 axis, double radius,
			double minTh, double maxTh, double minPs, double maxPs, [In] ref BVector3 color, double stepDegrees);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawTransformUnmanagedDelegate([In] ref BMatrix transform, double orthoLen);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void DrawTriangleUnmanagedDelegate([In] ref BVector3 v0, [In] ref BVector3 v1, [In] ref BVector3 v2, [In] ref BVector3 color, double alpha);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate void SimpleCallback(int x);
		[UnmanagedFunctionPointer(Native.Conv), SuppressUnmanagedCodeSecurity]
		delegate DebugDrawModes GetDebugModeUnmanagedDelegate();

		DrawAabbUnmanagedDelegate _drawAabb;
		DrawArcUnmanagedDelegate _drawArc;
		DrawBoxUnmanagedDelegate _drawBox;
		DrawCapsuleUnmanagedDelegate _drawCapsule;
		DrawConeUnmanagedDelegate _drawCone;
		DrawContactPointUnmanagedDelegate _drawContactPoint;
		DrawCylinderUnmanagedDelegate _drawCylinder;
		DrawLineUnmanagedDelegate _drawLine;
		DrawPlaneUnmanagedDelegate _drawPlane;
		DrawSphereUnmanagedDelegate _drawSphere;
		DrawSpherePatchUnmanagedDelegate _drawSpherePatch;
		DrawTransformUnmanagedDelegate _drawTransform;
		DrawTriangleUnmanagedDelegate _drawTriangle;
		GetDebugModeUnmanagedDelegate _getDebugMode;
		SimpleCallback _cb;

		internal static IntPtr CreateWrapper(IDebugDraw value, bool weakReference)
		{
			DrawAabbUnmanagedDelegate a = new DrawAabbUnmanagedDelegate(value.DrawAabb);
			/*
			_drawArc = new DrawArcUnmanagedDelegate(DrawArc);
			_drawBox = new DrawBoxUnmanagedDelegate(DrawBox);
			_drawCapsule = new DrawCapsuleUnmanagedDelegate(DrawCapsule);
			_drawCone = new DrawConeUnmanagedDelegate(DrawCone);
			_drawContactPoint = new DrawContactPointUnmanagedDelegate(DrawContactPoint);
			_drawCylinder = new DrawCylinderUnmanagedDelegate(DrawCylinder);
			_drawLine = new DrawLineUnmanagedDelegate(DrawLine);
			_drawPlane = new DrawPlaneUnmanagedDelegate(DrawPlane);
			_drawSphere = new DrawSphereUnmanagedDelegate(DrawSphere);
			_drawSpherePatch = new DrawSpherePatchUnmanagedDelegate(DrawSpherePatch);
			_drawTransform = new DrawTransformUnmanagedDelegate(DrawTransform);
			_drawTriangle = new DrawTriangleUnmanagedDelegate(DrawTriangle);
			_getDebugMode = new GetDebugModeUnmanagedDelegate(GetDebugModeUnmanaged);
			_cb = new SimpleCallback(SimpleCallbackUnmanaged);

			_native = btIDebugDrawWrapper_new(
				GCHandle.ToIntPtr(GCHandle.Alloc(this)),
				Marshal.GetFunctionPointerForDelegate(_drawAabb),
				Marshal.GetFunctionPointerForDelegate(_drawArc),
				Marshal.GetFunctionPointerForDelegate(_drawBox),
				Marshal.GetFunctionPointerForDelegate(_drawCapsule),
				Marshal.GetFunctionPointerForDelegate(_drawCone),
				Marshal.GetFunctionPointerForDelegate(_drawContactPoint),
				Marshal.GetFunctionPointerForDelegate(_drawCylinder),
				Marshal.GetFunctionPointerForDelegate(_drawLine),
				Marshal.GetFunctionPointerForDelegate(_drawPlane),
				Marshal.GetFunctionPointerForDelegate(_drawSphere),
				Marshal.GetFunctionPointerForDelegate(_drawSpherePatch),
				Marshal.GetFunctionPointerForDelegate(_drawTransform),
				Marshal.GetFunctionPointerForDelegate(_drawTriangle),
				Marshal.GetFunctionPointerForDelegate(_getDebugMode),
				Marshal.GetFunctionPointerForDelegate(_cb));
			*/
			return IntPtr.Zero;
		}

		internal static IntPtr GetUnmanaged(IDebugDraw debugDrawer)
		{
			if (debugDrawer == null)
			{
				return IntPtr.Zero;
			}

			if (debugDrawer is DebugDraw)
			{
				return (debugDrawer as DebugDraw)._native;
			}

			//if (ObjectTable.Contains(debugDraw))
			//	return ObjectTable.GetUnmanagedObject(debugDraw);

			throw new NotImplementedException();
			//GCHandle handle = GCHandle.Alloc(debugDrawer);
			//IntPtr wrapper = btIDebugDrawWrapper_new(GCHandle.ToIntPtr(handle), IntPtr.Zero);
			//ObjectTable.Add(debugDraw, wrapper);
			//return wrapper;
		}

		internal static IDebugDraw GetManaged(IntPtr debugDrawer)
		{
			if (debugDrawer == IntPtr.Zero)
			{
				return null;
			}

			//if (ObjectTable.Contains(debugDrawer)
			//	return ObjectTable.GetObject<IDebugDraw^>(debugDrawer);

			IntPtr handle = btIDebugDrawWrapper_getGCHandle(debugDrawer);
			return GCHandle.FromIntPtr(handle).Target as IDebugDraw;
		}
		
		void SimpleCallbackUnmanaged(int x)
		{
			throw new NotImplementedException();
		}

		DebugDrawModes GetDebugModeUnmanaged()
		{
			return DebugMode;
		}

		internal void InitTarget(IDebugDraw target)
		{
			_drawAabb = new DrawAabbUnmanagedDelegate(target.DrawAabb);
			_drawArc = new DrawArcUnmanagedDelegate(target.DrawArc);
			_drawBox = new DrawBoxUnmanagedDelegate(target.DrawBox);
			_drawCapsule = new DrawCapsuleUnmanagedDelegate(target.DrawCapsule);
			_drawCone = new DrawConeUnmanagedDelegate(target.DrawCone);
			_drawContactPoint = new DrawContactPointUnmanagedDelegate(target.DrawContactPoint);
			_drawCylinder = new DrawCylinderUnmanagedDelegate(target.DrawCylinder);
			_drawLine = new DrawLineUnmanagedDelegate(target.DrawLine);
			_drawPlane = new DrawPlaneUnmanagedDelegate(target.DrawPlane);
			_drawSphere = new DrawSphereUnmanagedDelegate(target.DrawSphere);
			_drawSpherePatch = new DrawSpherePatchUnmanagedDelegate(target.DrawSpherePatch);
			_drawTransform = new DrawTransformUnmanagedDelegate(target.DrawTransform);
			_drawTriangle = new DrawTriangleUnmanagedDelegate(target.DrawTriangle);
			_getDebugMode = new GetDebugModeUnmanagedDelegate(GetDebugModeUnmanaged);
			_cb = new SimpleCallback(SimpleCallbackUnmanaged);

			_native = btIDebugDrawWrapper_new(
				GCHandle.ToIntPtr(GCHandle.Alloc(this)),
				Marshal.GetFunctionPointerForDelegate(_drawAabb),
				Marshal.GetFunctionPointerForDelegate(_drawArc),
				Marshal.GetFunctionPointerForDelegate(_drawBox),
				Marshal.GetFunctionPointerForDelegate(_drawCapsule),
				Marshal.GetFunctionPointerForDelegate(_drawCone),
				Marshal.GetFunctionPointerForDelegate(_drawContactPoint),
				Marshal.GetFunctionPointerForDelegate(_drawCylinder),
				Marshal.GetFunctionPointerForDelegate(_drawLine),
				Marshal.GetFunctionPointerForDelegate(_drawPlane),
				Marshal.GetFunctionPointerForDelegate(_drawSphere),
				Marshal.GetFunctionPointerForDelegate(_drawSpherePatch),
				Marshal.GetFunctionPointerForDelegate(_drawTransform),
				Marshal.GetFunctionPointerForDelegate(_drawTriangle),
				Marshal.GetFunctionPointerForDelegate(_getDebugMode),
				Marshal.GetFunctionPointerForDelegate(_cb));
		}

		internal DebugDraw(IDebugDraw target)
		{
			InitTarget(target);
		}

		public DebugDraw()
		{
			InitTarget(this);
		}

		public abstract void DrawLine(ref BVector3 from, ref BVector3 to, ref BVector3 color);
		public abstract void Draw3dText(ref BVector3 location, String textString);
		public abstract void ReportErrorWarning(String warningString);
		public abstract DebugDrawModes DebugMode { get; set; }

		public void DrawLine(BVector3 from, BVector3 to, BVector3 color)
		{
			DrawLine(ref from, ref to, ref color);
		}

		public virtual void DrawLine(ref BVector3 from, ref BVector3 to, ref BVector3 fromColor, ref BVector3 toColor)
		{
			DrawLine(ref from, ref to, ref fromColor);
		}

		public virtual void DrawAabb(ref BVector3 from, ref BVector3 to, ref BVector3 color)
		{
			BVector3 halfExtents = (to - from) * 0.5f;
			BVector3 center = (to + from) * 0.5f;
			int i, j;

			BVector3 edgecoord = new BVector3(1.0f, 1.0f, 1.0f), pa, pb;
			for (i = 0; i < 4; i++)
			{
				for (j = 0; j < 3; j++)
				{
					pa = new BVector3(edgecoord.X * halfExtents.X, edgecoord.Y * halfExtents.Y,
						   edgecoord.Z * halfExtents.Z);
					pa += center;

					int othercoord = j % 3;
					edgecoord[othercoord] *= -1.0f;
					pb = new BVector3(edgecoord.X * halfExtents.X, edgecoord.Y * halfExtents.Y,
							edgecoord.Z * halfExtents.Z);
					pb += center;

					DrawLine(ref pa, ref pb, ref color);
				}
				edgecoord = new BVector3(-1.0f, -1.0f, -1.0f);
				if (i < 3)
				{
					edgecoord[i] *= -1.0f;
				}
			}
		}

		public virtual void DrawArc(ref BVector3 center, ref BVector3 normal, ref BVector3 axis, double radiusA, double radiusB, double minAngle, double maxAngle,
			ref BVector3 color, bool drawSect)
		{
			DrawArc(ref center, ref normal, ref axis, radiusA, radiusB, minAngle, maxAngle, ref color, drawSect, 10f);
		}

		public virtual void DrawArc(ref BVector3 center, ref BVector3 normal, ref BVector3 axis, double radiusA, double radiusB, double minAngle, double maxAngle,
			ref BVector3 color, bool drawSect, double stepDegrees)
		{
			BVector3 vx = axis;
			BVector3 vy = BVector3.Cross(normal, axis);
			double step = stepDegrees * MathUtil.SIMD_RADS_PER_DEG;
			int nSteps = (int)((maxAngle - minAngle) / step);
			if (nSteps == 0)
			{
				nSteps = 1;
			}
			BVector3 prev = center + radiusA * vx * (double)System.Math.Cos(minAngle) + radiusB * vy * (double)System.Math.Sin(minAngle);
			if (drawSect)
			{
				DrawLine(ref center, ref prev, ref color);
			}
			for (int i = 1; i <= nSteps; i++)
			{
				double angle = minAngle + (maxAngle - minAngle) * i / nSteps;
				BVector3 next = center + radiusA * vx * (double)System.Math.Cos(angle) + radiusB * vy * (double)System.Math.Sin(angle);
				DrawLine(ref prev, ref next, ref color);
				prev = next;
			}
			if (drawSect)
			{
				DrawLine(ref center, ref prev, ref color);
			}
		}

		public virtual void DrawBox(ref BVector3 bbMin, ref BVector3 bbMax, ref BVector3 color)
		{
			//Vector3 p1 = bbMin;
			BVector3 p2 = new BVector3(bbMax.X, bbMin.Y, bbMin.Z);
			BVector3 p3 = new BVector3(bbMax.X, bbMax.Y, bbMin.Z);
			BVector3 p4 = new BVector3(bbMin.X, bbMax.Y, bbMin.Z);
			BVector3 p5 = new BVector3(bbMin.X, bbMin.Y, bbMax.Z);
			BVector3 p6 = new BVector3(bbMax.X, bbMin.Y, bbMax.Z);
			//Vector3 p7 = bbMax;
			BVector3 p8 = new BVector3(bbMin.X, bbMax.Y, bbMax.Z);

			DrawLine(ref bbMin, ref p2, ref color);
			DrawLine(ref p2, ref p3, ref color);
			DrawLine(ref p3, ref p4, ref color);
			DrawLine(ref p4, ref bbMin, ref color);

			DrawLine(ref bbMin, ref p5, ref color);
			DrawLine(ref p2, ref p6, ref color);
			DrawLine(ref p3, ref bbMax, ref color);
			DrawLine(ref p4, ref p8, ref color);

			DrawLine(ref p5, ref p6, ref color);
			DrawLine(ref p6, ref bbMax, ref color);
			DrawLine(ref bbMax, ref p8, ref color);
			DrawLine(ref p8, ref p5, ref color);
		}

		public virtual void DrawBox(ref BVector3 bbMin, ref BVector3 bbMax, ref BMatrix trans, ref BVector3 color)
		{
			BVector3 p1, p2, p3, p4, p5, p6, p7, p8;
			BVector3 point = bbMin;
			BVector3.TransformCoordinate(ref point, ref trans, out p1);
			point.X = bbMax.X;
			BVector3.TransformCoordinate(ref point, ref trans, out p2);
			point.Y = bbMax.Y;
			BVector3.TransformCoordinate(ref point, ref trans, out p3);
			point.X = bbMin.X;
			BVector3.TransformCoordinate(ref point, ref trans, out p4);
			point.Z = bbMax.Z;
			BVector3.TransformCoordinate(ref point, ref trans, out p8);
			point.X = bbMax.X;
			BVector3.TransformCoordinate(ref point, ref trans, out p7);
			point.Y = bbMin.Y;
			BVector3.TransformCoordinate(ref point, ref trans, out p6);
			point.X = bbMin.X;
			BVector3.TransformCoordinate(ref point, ref trans, out p5);

			DrawLine(ref p1, ref p2, ref color);
			DrawLine(ref p2, ref p3, ref color);
			DrawLine(ref p3, ref p4, ref color);
			DrawLine(ref p4, ref p1, ref color);

			DrawLine(ref p1, ref p5, ref color);
			DrawLine(ref p2, ref p6, ref color);
			DrawLine(ref p3, ref p7, ref color);
			DrawLine(ref p4, ref p8, ref color);

			DrawLine(ref p5, ref p6, ref color);
			DrawLine(ref p6, ref p7, ref color);
			DrawLine(ref p7, ref p8, ref color);
			DrawLine(ref p8, ref p5, ref color);
		}

		public virtual void DrawCapsule(double radius, double halfHeight, int upAxis, ref BMatrix transform, ref BVector3 color)
		{
			BVector3 capStart = BVector3.Zero;
			capStart[upAxis] = -halfHeight;

			BVector3 capEnd = BVector3.Zero;
			capEnd[upAxis] = halfHeight;

			// Draw the ends
			BMatrix childTransform = transform;
			childTransform.Origin = BVector3.TransformCoordinate(capStart, transform);
			DrawSphere(radius, ref childTransform, ref color);

			childTransform.Origin = BVector3.TransformCoordinate(capEnd, transform);
			DrawSphere(radius, ref childTransform, ref color);

			// Draw some additional lines
			BVector3 start = transform.Origin;
			BMatrix basis = transform.Basis;

			capStart[(upAxis + 1) % 3] = radius;
			capEnd[(upAxis + 1) % 3] = radius;
			DrawLine(start + BVector3.TransformCoordinate(capStart, basis), start + BVector3.TransformCoordinate(capEnd, basis), color);

			capStart[(upAxis + 1) % 3] = -radius;
			capEnd[(upAxis + 1) % 3] = -radius;
			DrawLine(start + BVector3.TransformCoordinate(capStart, basis), start + BVector3.TransformCoordinate(capEnd, basis), color);

			capStart[(upAxis + 2) % 3] = radius;
			capEnd[(upAxis + 2) % 3] = radius;
			DrawLine(start + BVector3.TransformCoordinate(capStart, basis), start + BVector3.TransformCoordinate(capEnd, basis), color);

			capStart[(upAxis + 2) % 3] = -radius;
			capEnd[(upAxis + 2) % 3] = -radius;
			DrawLine(start + BVector3.TransformCoordinate(capStart, basis), start + BVector3.TransformCoordinate(capEnd, basis), color);
		}

		public virtual void DrawCone(double radius, double height, int upAxis, ref BMatrix transform, ref BVector3 color)
		{
			BVector3 start = transform.Origin;

			BVector3 offsetHeight = BVector3.Zero;
			offsetHeight[upAxis] = height * 0.5f;
			BVector3 offsetRadius = BVector3.Zero;
			offsetRadius[(upAxis + 1) % 3] = radius;

			BVector3 offset2Radius = BVector3.Zero;
			offsetRadius[(upAxis + 2) % 3] = radius;

			BMatrix basis = transform.Basis;
			BVector3 from = start + BVector3.TransformCoordinate(offsetHeight, basis);
			BVector3 to = start + BVector3.TransformCoordinate(-offsetHeight, basis);
			DrawLine(from, to + offsetRadius, color);
			DrawLine(from, to - offsetRadius, color);
			DrawLine(from, to + offset2Radius, color);
			DrawLine(from, to - offset2Radius, color);
		}

		public virtual void DrawContactPoint(ref BVector3 pointOnB, ref BVector3 normalOnB, double distance, int lifeTime, ref BVector3 color)
		{
			BVector3 to = pointOnB + normalOnB * 1; // distance
			DrawLine(ref pointOnB, ref to, ref color);
		}

		public virtual void DrawCylinder(double radius, double halfHeight, int upAxis, ref BMatrix transform, ref BVector3 color)
		{
			BVector3 start = transform.Origin;
			BMatrix basis = transform.Basis;
			BVector3 offsetHeight = BVector3.Zero;
			offsetHeight[upAxis] = halfHeight;
			BVector3 offsetRadius = BVector3.Zero;
			offsetRadius[(upAxis + 1) % 3] = radius;
			DrawLine(start + BVector3.TransformCoordinate(offsetHeight + offsetRadius, basis), start + BVector3.TransformCoordinate(-offsetHeight + offsetRadius, basis), color);
			DrawLine(start + BVector3.TransformCoordinate(offsetHeight - offsetRadius, basis), start + BVector3.TransformCoordinate(-offsetHeight - offsetRadius, basis), color);
		}

		public virtual void DrawPlane(ref BVector3 planeNormal, double planeConst, ref BMatrix transform, ref BVector3 color)
		{
			BVector3 planeOrigin = planeNormal * planeConst;
			BVector3 vec0, vec1;
			PlaneSpace1(ref planeNormal, out vec0, out vec1);
			const double vecLen = 100f;
			BVector3 pt0 = planeOrigin + vec0 * vecLen;
			BVector3 pt1 = planeOrigin - vec0 * vecLen;
			BVector3 pt2 = planeOrigin + vec1 * vecLen;
			BVector3 pt3 = planeOrigin - vec1 * vecLen;
			BVector3.TransformCoordinate(ref pt0, ref transform, out pt0);
			BVector3.TransformCoordinate(ref pt1, ref transform, out pt1);
			BVector3.TransformCoordinate(ref pt2, ref transform, out pt2);
			BVector3.TransformCoordinate(ref pt3, ref transform, out pt3);
			DrawLine(ref pt0, ref pt1, ref color);
			DrawLine(ref pt2, ref pt3, ref color);
		}

		public virtual void DrawSphere(double radius, ref BMatrix transform, ref BVector3 color)
		{
			BVector3 start = transform.Origin;
			BMatrix basis = transform.Basis;

			BVector3 xoffs = BVector3.TransformCoordinate(new BVector3(radius, 0, 0), basis);
			BVector3 yoffs = BVector3.TransformCoordinate(new BVector3(0, radius, 0), basis);
			BVector3 zoffs = BVector3.TransformCoordinate(new BVector3(0, 0, radius), basis);

			BVector3 xn = start - xoffs;
			BVector3 xp = start + xoffs;
			BVector3 yn = start - yoffs;
			BVector3 yp = start + yoffs;
			BVector3 zn = start - zoffs;
			BVector3 zp = start + zoffs;

			// XY
			DrawLine(ref xn, ref yp, ref color);
			DrawLine(ref yp, ref xp, ref color);
			DrawLine(ref xp, ref yn, ref color);
			DrawLine(ref yn, ref xn, ref color);

			// XZ
			DrawLine(ref xn, ref zp, ref color);
			DrawLine(ref zp, ref xp, ref color);
			DrawLine(ref xp, ref zn, ref color);
			DrawLine(ref zn, ref xn, ref color);

			// YZ
			DrawLine(ref yn, ref zp, ref color);
			DrawLine(ref zp, ref yp, ref color);
			DrawLine(ref yp, ref zn, ref color);
			DrawLine(ref zn, ref yn, ref color);
		}

		public virtual void DrawSphere(ref BVector3 p, double radius, ref BVector3 color)
		{
			BMatrix tr = BMatrix.Translation(p);
			DrawSphere(radius, ref tr, ref color);
		}

		public virtual void DrawSpherePatch(ref BVector3 center, ref BVector3 up, ref BVector3 axis, double radius,
			double minTh, double maxTh, double minPs, double maxPs, ref BVector3 color)
		{
			DrawSpherePatch(ref center, ref up, ref axis, radius, minTh, maxTh, minPs, maxPs, ref color, 10.0f);
		}

		public virtual void DrawSpherePatch(ref BVector3 center, ref BVector3 up, ref BVector3 axis, double radius,
			double minTh, double maxTh, double minPs, double maxPs, ref BVector3 color, double stepDegrees)
		{
			BVector3[] vA;
			BVector3[] vB;
			BVector3[] pvA, pvB, pT;
			BVector3 npole = center + up * radius;
			BVector3 spole = center - up * radius;
			BVector3 arcStart = BVector3.Zero;
			double step = stepDegrees * MathUtil.SIMD_RADS_PER_DEG;
			BVector3 kv = up;
			BVector3 iv = axis;

			BVector3 jv = BVector3.Cross(kv, iv);
			bool drawN = false;
			bool drawS = false;
			if (minTh <= -MathUtil.SIMD_HALF_PI)
			{
				minTh = -MathUtil.SIMD_HALF_PI + step;
				drawN = true;
			}
			if (maxTh >= MathUtil.SIMD_HALF_PI)
			{
				maxTh = MathUtil.SIMD_HALF_PI - step;
				drawS = true;
			}
			if (minTh > maxTh)
			{
				minTh = -MathUtil.SIMD_HALF_PI + step;
				maxTh = MathUtil.SIMD_HALF_PI - step;
				drawN = drawS = true;
			}
			int n_hor = (int)((maxTh - minTh) / step) + 1;
			if (n_hor < 2) n_hor = 2;
			double step_h = (maxTh - minTh) / (n_hor - 1);
			bool isClosed;
			if (minPs > maxPs)
			{
				minPs = -MathUtil.SIMD_PI + step;
				maxPs = MathUtil.SIMD_PI;
				isClosed = true;
			}
			else if ((maxPs - minPs) >= MathUtil.SIMD_PI * 2f)
			{
				isClosed = true;
			}
			else
			{
				isClosed = false;
			}
			int n_vert = (int)((maxPs - minPs) / step) + 1;
			if (n_vert < 2) n_vert = 2;

			vA = new BVector3[n_vert];
			vB = new BVector3[n_vert];
			pvA = vA; pvB = vB;

			double step_v = (maxPs - minPs) / (double)(n_vert - 1);
			for (int i = 0; i < n_hor; i++)
			{
				double th = minTh + i * step_h;
				double sth = radius * (double)System.Math.Sin(th);
				double cth = radius * (double)System.Math.Cos(th);
				for (int j = 0; j < n_vert; j++)
				{
					double psi = minPs + (double)j * step_v;
					double sps = (double)System.Math.Sin(psi);
					double cps = (double)System.Math.Cos(psi);
					pvB[j] = center + cth * cps * iv + cth * sps * jv + sth * kv;
					if (i != 0)
					{
						DrawLine(ref pvA[j], ref pvB[j], ref color);
					}
					else if (drawS)
					{
						DrawLine(ref spole, ref pvB[j], ref color);
					}
					if (j != 0)
					{
						DrawLine(ref pvB[j - 1], ref pvB[j], ref color);
					}
					else
					{
						arcStart = pvB[j];
					}
					if ((i == (n_hor - 1)) && drawN)
					{
						DrawLine(ref npole, ref pvB[j], ref color);
					}
					if (isClosed)
					{
						if (j == (n_vert - 1))
						{
							DrawLine(ref arcStart, ref pvB[j], ref color);
						}
					}
					else
					{
						if (((i == 0) || (i == (n_hor - 1))) && ((j == 0) || (j == (n_vert - 1))))
						{
							DrawLine(ref center, ref pvB[j], ref color);
						}
					}
				}
				pT = pvA; pvA = pvB; pvB = pT;
			}
		}

		public virtual void DrawTriangle(ref BVector3 v0, ref BVector3 v1, ref BVector3 v2, ref BVector3 n0, ref BVector3 n1, ref BVector3 n2, ref BVector3 color, double alpha)
		{
			DrawTriangle(ref v0, ref v1, ref v2, ref color, alpha);
		}

		public virtual void DrawTriangle(ref BVector3 v0, ref BVector3 v1, ref BVector3 v2, ref BVector3 color, double alpha)
		{
			DrawLine(ref v0, ref v1, ref color);
			DrawLine(ref v1, ref v2, ref color);
			DrawLine(ref v2, ref v0, ref color);
		}

		public virtual void DrawTransform(ref BMatrix transform, double orthoLen)
		{
			BVector3 start = transform.Origin;
			BMatrix basis = transform.Basis;

			BVector3 ortho = new BVector3(orthoLen, 0, 0);
			BVector3 colour = new BVector3(0.7f, 0, 0);
			BVector3 temp;
			BVector3.TransformCoordinate(ref ortho, ref basis, out temp);
			temp += start;
			DrawLine(ref start, ref temp, ref colour);

			ortho.X = 0;
			ortho.Y = orthoLen;
			colour.X = 0;
			colour.Y = 0.7f;
			BVector3.TransformCoordinate(ref ortho, ref basis, out temp);
			temp += start;
			DrawLine(ref start, ref temp, ref colour);

			ortho.Y = 0;
			ortho.Z = orthoLen;
			colour.Y = 0;
			colour.Z = 0.7f;
			BVector3.TransformCoordinate(ref ortho, ref basis, out temp);
			temp += start;
			DrawLine(ref start, ref temp, ref colour);
		}

		public static void PlaneSpace1(ref BVector3 n, out BVector3 p, out BVector3 q)
		{
			if (System.Math.Abs(n.Z) > MathUtil.SIMDSQRT12)
			{
				// choose p in y-z plane
				double a = n.Y * n.Y + n.Z * n.Z;
				double k = MathUtil.RecipSqrt(a);
				p = new BVector3(0, -n.Z * k, n.Y * k);
				// set q = n x p
				q = new BVector3(a * k, -n.X * p.Z, n.X * p.Y);
			}
			else
			{
				// choose p in x-y plane
				double a = n.X * n.X + n.Y * n.Y;
				double k = MathUtil.RecipSqrt(a);
				p = new BVector3(-n.Y * k, n.X * k, 0);
				// set q = n x p
				q = new BVector3(-n.Z * p.Y, n.Z * p.X, a * k);
			}
		}
	}
}
