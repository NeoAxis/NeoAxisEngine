// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.Collections.ObjectModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	//!!!!

	//!!!!!!name? KnownParameterType?
	/// <summary>
	/// A list of parameter types that are supported in the <see cref="ParameterContainer"/>.
	/// </summary>
	public enum ParameterType
	{
		//must be save in native

		Unknown,
		Object,
		String,

		Boolean,
		Byte,

		Integer,
		Vector2I,
		RectangleI,
		RangeI,
		Vector3I,
		Vector4I,

		Float,
		Radian,
		Degree,
		Vector2,
		Range,
		SphericalDirection,
		Vector3,
		Angles,
		Vector4,
		ColorValue,
		Rectangle,
		Quaternion,
		Bounds,
		Box,
		Capsule,
		Cone,
		Line,
		Plane,
		Ray,
		Matrix2x2,
		//Matrix2x3,
		//Matrix2x4,
		//Matrix3x2,
		Matrix3x3,
		//Matrix3x4,
		//Matrix4x2,
		//Matrix4x3,
		Matrix4x4,

		//!!!!лишние убрать

		//Double,
		//RadianD,
		//DegreeD,
		//Vector2D,
		//RangeD,
		//SphereDirectionD,
		//Vector3D,
		//AnglesD,
		//Vector4D,
		////ColorValueD,
		//RectangleD,
		//QuaternionD,
		//BoundsD,
		//BoxD,
		//CapsuleD,
		//ConeD,
		//LineD,
		//PlaneD,
		//RayD,
		//Matrix2x2D,
		//Matrix2x3D,
		//Matrix2x4D,
		//Matrix3x2D,
		//Matrix3x3D,
		//Matrix3x4D,
		//Matrix4x2D,
		//Matrix4x3D,
		//Matrix4x4D,

		//Texture1D,
		Texture2D,
		Texture3D,
		TextureCube,

		//!!!!Shader?
		//!!!!что еще?


	}
}
