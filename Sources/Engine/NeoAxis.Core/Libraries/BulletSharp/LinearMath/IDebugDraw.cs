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
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using Internal.BulletSharp.Math;
using System;

namespace Internal.BulletSharp
{
	public interface IDebugDraw
	{
		void DrawLine(ref BVector3 from, ref BVector3 to, ref BVector3 fromColor);
		void DrawLine(ref BVector3 from, ref BVector3 to, ref BVector3 fromColor, ref BVector3 toColor);
		void DrawBox(ref BVector3 bbMin, ref BVector3 bbMax, ref BVector3 color);
		void DrawBox(ref BVector3 bbMin, ref BVector3 bbMax, ref BMatrix trans, ref BVector3 color);
		void DrawSphere(ref BVector3 p, double radius, ref BVector3 color);
		void DrawSphere(double radius, ref BMatrix transform, ref BVector3 color);
		void DrawTriangle(ref BVector3 v0, ref BVector3 v1, ref BVector3 v2, ref BVector3 n0, ref BVector3 n1, ref BVector3 n2, ref BVector3 color, double alpha);
		void DrawTriangle(ref BVector3 v0, ref BVector3 v1, ref BVector3 v2, ref BVector3 color, double alpha);
		void DrawContactPoint(ref BVector3 pointOnB, ref BVector3 normalOnB, double distance, int lifeTime, ref BVector3 color);
		void ReportErrorWarning(String warningString);
		void Draw3dText(ref BVector3 location, String textString);

		DebugDrawModes DebugMode { get; set; }

		void DrawAabb(ref BVector3 from, ref BVector3 to, ref BVector3 color);
		void DrawTransform(ref BMatrix transform, double orthoLen);
		void DrawArc(ref BVector3 center, ref BVector3 normal, ref BVector3 axis, double radiusA, double radiusB, double minAngle, double maxAngle,
			ref BVector3 color, bool drawSect);
		void DrawArc(ref BVector3 center, ref BVector3 normal, ref BVector3 axis, double radiusA, double radiusB, double minAngle, double maxAngle,
			ref BVector3 color, bool drawSect, double stepDegrees);
		void DrawSpherePatch(ref BVector3 center, ref BVector3 up, ref BVector3 axis, double radius,
			double minTh, double maxTh, double minPs, double maxPs, ref BVector3 color);
		void DrawSpherePatch(ref BVector3 center, ref BVector3 up, ref BVector3 axis, double radius,
			double minTh, double maxTh, double minPs, double maxPs, ref BVector3 color, double stepDegrees);
		void DrawCapsule(double radius, double halfHeight, int upAxis, ref BMatrix transform, ref BVector3 color);
		void DrawCylinder(double radius, double halfHeight, int upAxis, ref BMatrix transform, ref BVector3 color);
		void DrawCone(double radius, double height, int upAxis, ref BMatrix transform, ref BVector3 color);
		void DrawPlane(ref BVector3 planeNormal, double planeConst, ref BMatrix transform, ref BVector3 color);
	}

	[Flags]
	public enum DebugDrawModes
	{
		None = 0,
		DrawWireframe = 1,
		DrawAabb = 2,
		DrawFeaturesText = 4,
		DrawContactPoints = 8,
		NoDeactivation = 16,
		NoHelpText = 32,
		DrawText = 64,
		ProfileTimings = 128,
		EnableSatComparison = 256,
		DisableBulletLCP = 512,
		EnableCCD = 1024,
		DrawConstraints = (1 << 11),
		DrawConstraintLimits = (1 << 12),
		DrawFastWireframe = (1 << 13),
		DrawNormals = (1 << 14),
		All = DrawWireframe | DrawAabb | DrawFeaturesText | DrawContactPoints | DrawText | DrawConstraints | DrawConstraintLimits,
		MaxDebugDrawMode
	}
}
