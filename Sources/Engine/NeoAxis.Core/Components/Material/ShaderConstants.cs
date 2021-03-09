// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Provides access to constants in shaders.
	/// </summary>
	public static class ShaderConstants
	{
		[ShaderGenerationAutoConstantAttribute( typeof( Vector3 ), "u_viewportOwnerCameraPosition" )]
		public static Vector3 ViewportOwnerCameraPosition
		{
			get { return Vector3.Zero; }
		}

		[ShaderGenerationAutoConstantAttribute( typeof( double ), "u_viewportOwnerNearClipDistance" )]
		public static double ViewportOwnerNearClipDistance
		{
			get { return 0; }
		}

		[ShaderGenerationAutoConstantAttribute( typeof( double ), "u_viewportOwnerFarClipDistance" )]
		public static double ViewportOwnerFarClipDistance
		{
			get { return 0; }
		}

		[ShaderGenerationAutoConstantAttribute( typeof( double ), "u_viewportOwnerFieldOfView" )]
		public static double ViewportOwnerFieldOfView
		{
			get { return 0; }
		}

		[ShaderGenerationAutoConstantAttribute( typeof( double ), "u_viewportOwnerShadowFarDistance" )]
		public static double ViewportOwnerShadowFarDistance
		{
			get { return 0; }
		}

		[ShaderGenerationAutoConstantAttribute( typeof( Vector2 ), "u_viewportSize" )]
		public static Vector2 ViewportSize//!!!!Vec2I?
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets world matrix. Works only in vertex shaders.
		/// </summary>
		[ShaderGenerationFunction( "worldMatrix" )]
		public static Matrix4 WorldMatrix
		{
			get { return Matrix4.Zero; }
		}

		/// <summary>
		/// Gets position of a vertex. Works only in vertex shaders.
		/// </summary>
		[ShaderGenerationFunction( "positionLocal" )]
		public static Vector3 Position
		{
			get { return Vector3.Zero; }
		}

		/// <summary>
		/// Gets normal of a vertex. Works only in vertex shaders.
		/// </summary>
		[ShaderGenerationFunction( "normalLocal" )]
		public static Vector3 Normal
		{
			get { return Vector3.Zero; }
		}

		/// <summary>
		/// Gets tangent vector of a vertex. Works only in vertex shaders.
		/// </summary>
		[ShaderGenerationFunction( "tangentLocal" )]
		public static Vector4 Tangent
		{
			get { return Vector4.Zero; }
		}

		/// <summary>
		/// Gets texture coordinate 0 of a vertex. Works only in vertex and fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "c_texCoord0" )]
		public static Vector2 TexCoord0
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets texture coordinate 1 of a vertex. Works only in vertex and fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "c_texCoord1" )]
		public static Vector2 TexCoord1
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets texture coordinate 2 of a vertex. Works only in vertex and fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "c_texCoord2" )]
		public static Vector2 TexCoord2
		{
			get { return Vector2.Zero; }
		}

		///// <summary>
		///// Gets texture coordinate 3 of a vertex. Works only in vertex and fragment shaders.
		///// </summary>
		//[ShaderGenerationFunction( "c_texCoord3" )]
		//public static Vector2 TexCoord3
		//{
		//	get { return Vector2.Zero; }
		//}

		/// <summary>
		/// Gets texture coordinate unwrapped UV of a vertex. Works only in vertex and fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "c_unwrappedUV" )]
		public static Vector2 UnwrappedUV
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets color 0 of a vertex. Works only in vertex and fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "c_color0" )]
		public static Vector4 Color0
		{
			get { return Vector4.Zero; }
		}

		/// <summary>
		/// Indicates whether a primitive is front or back facing. Works only in fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "(gl_FrontFacing == false)" )]//[ShaderGenerationFunction( "gl_FrontFacing" )]
		public static bool IsFrontFace
		{
			get { return false; }
		}
	}
}
