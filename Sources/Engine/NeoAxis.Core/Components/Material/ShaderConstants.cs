// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Provides access to constants in shaders.
	/// </summary>
	public static class ShaderConstants
	{
		[ShaderGenerationAutoConstant( typeof( Vector3 ), "u_viewportOwnerCameraPosition" )]
		public static Vector3 ViewportOwnerCameraPosition
		{
			get { return Vector3.Zero; }
		}

		[ShaderGenerationAutoConstant( typeof( double ), "u_viewportOwnerNearClipDistance" )]
		public static double ViewportOwnerNearClipDistance
		{
			get { return 0; }
		}

		[ShaderGenerationAutoConstant( typeof( double ), "u_viewportOwnerFarClipDistance" )]
		public static double ViewportOwnerFarClipDistance
		{
			get { return 0; }
		}

		[ShaderGenerationAutoConstant( typeof( double ), "u_viewportOwnerFieldOfView" )]
		public static double ViewportOwnerFieldOfView
		{
			get { return 0; }
		}

		[ShaderGenerationAutoConstant( typeof( double ), "u_viewportOwnerShadowFarDistance" )]
		public static double ViewportOwnerShadowFarDistance
		{
			get { return 0; }
		}

		[ShaderGenerationAutoConstant( typeof( Vector2 ), "u_viewportSize" )]
		public static Vector2 ViewportSize//!!!!Vec2I?
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets the engine time.
		/// </summary>
		[ShaderGenerationFunction( "u_engineTime" )]
		public static double EngineTime
		{
			get { return 0; }
		}

		/// <summary>
		/// Gets the wind speed vector of the scene.
		/// </summary>
		[ShaderGenerationFunction( "u_windSpeed" )]
		public static Vector2 WindSpeed
		{
			get { return Vector2.Zero; }
		}

		[ShaderGenerationFunction( "u_viewportOwnerDebugMode" )]
		public static int ViewportOwnerDebugMode
		{
			get { return 0; }
		}

		[ShaderGenerationFunction( "u_emissiveMaterialsFactor" )]
		public static double EmissiveMaterialsFactor
		{
			get { return 0; }
		}

		[ShaderGenerationFunction( "u_cameraExposure" )]
		public static double CameraExposure
		{
			get { return 0; }
		}

		[ShaderGenerationFunction( "u_displacementScale" )]
		public static double DisplacementScale
		{
			get { return 0; }
		}

		[ShaderGenerationFunction( "u_displacementMaxSteps" )]
		public static int DisplacementMaxSteps
		{
			get { return 0; }
		}

		[ShaderGenerationFunction( "u_removeTextureTiling" )]
		public static double RemoveTextureTiling
		{
			get { return 0; }
		}

		[ShaderGenerationFunction( "(u_provideColorDepthTextureCopy > 0.0)" )]
		public static bool ProvideColorDepthTextureCopy
		{
			get { return false; }
		}

		[ShaderGenerationFunction( "u_viewportOwnerCameraDirection" )]
		public static Vector3 ViewportOwnerCameraDirection
		{
			get { return Vector3.Zero; }
		}

		[ShaderGenerationFunction( "u_viewportOwnerCameraUp" )]
		public static Vector3 ViewportOwnerCameraUp
		{
			get { return Vector3.Zero; }
		}

		/// <summary>
		/// Gets the multiplier of shadow visibility distance depending of object visibility distance.
		/// </summary>
		[ShaderGenerationFunction( "u_shadowObjectVisibilityDistanceFactor" )]
		public static double ShadowObjectVisibilityDistanceFactor
		{
			get { return 0; }
		}

		[ShaderGenerationFunction( "cameraPosition" )]
		public static Vector3 CameraPosition
		{
			get { return Vector3.Zero; }
		}

		[ShaderGenerationFunction( "u_temperature" )]
		public static double Temparature
		{
			get { return 0; }
		}

		[ShaderGenerationFunction( "u_precipitationFalling" )]
		public static double PrecipitationFalling
		{
			get { return 0; }
		}

		[ShaderGenerationFunction( "u_precipitationFallen" )]
		public static double PrecipitationFallen
		{
			get { return 0; }
		}

		[ShaderGenerationFunction( "u_timeOfDay" )]
		public static double TimeOfDay
		{
			get { return 0; }
		}

		///////////////////////////////////////////////

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
		public static Vector3 VertexPosition
		{
			get { return Vector3.Zero; }
		}

		/// <summary>
		/// Gets normal of a vertex. Works only in vertex shaders.
		/// </summary>
		[ShaderGenerationFunction( "normalLocal" )]
		public static Vector3 VertexNormal
		{
			get { return Vector3.Zero; }
		}

		/// <summary>
		/// Gets tangent vector of a vertex. Works only in vertex shaders.
		/// </summary>
		[ShaderGenerationFunction( "tangentLocal" )]
		public static Vector4 VertexTangent
		{
			get { return Vector4.Zero; }
		}

		/// <summary>
		/// Indicates whether a primitive is front or back facing. Works only in fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "gl_FrontFacing" )]
		public static bool IsFrontFace
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the window-relative coordinates of the current fragment.
		/// </summary>
		[ShaderGenerationFunction( "fragCoord" )]//[ShaderGenerationFunction( "getFragCoord()" )]
		public static Vector4 FragmentCoordinates
		{
			get { return Vector4.Zero; }
		}

		/// <summary>
		/// Gets world position of the fragment. Works only in fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "worldPosition" )]//[ShaderGenerationFunction( "v_worldPosition_depth.xyz" )]
		public static Vector3 FragmentWorldPosition
		{
			get { return Vector3.Zero; }
		}

		/// <summary>
		/// Gets world normal of the fragment. Works only in fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "inputWorldNormal" )]//[ShaderGenerationFunction( "normalize(v_worldNormal)" )]
		public static Vector3 FragmentWorldNormal
		{
			get { return Vector3.Zero; }
		}

		/// <summary>
		/// Gets texture coordinate 0 of the vertex or of the fragment. Works only in vertex and fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "texCoord0" )]
		public static Vector2 TexCoord0
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets texture coordinate 1 of the vertex or of the fragment. Works only in vertex and fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "texCoord1" )]
		public static Vector2 TexCoord1
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets texture coordinate 2 of the vertex or of the fragment. Works only in vertex and fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "texCoord2" )]
		public static Vector2 TexCoord2
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets texture coordinate unwrapped UV of the vertex or of the fragment. Works only in vertex and fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "unwrappedUV" )]
		public static Vector2 UnwrappedUV
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets texture coordinate 0 of the fragment before displacement. Works only in fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "texCoord0BeforeDisplacement" )]
		public static Vector2 TexCoord0BeforeDisplacement
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets texture coordinate 1 of the fragment before displacement. Works only in fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "texCoord1BeforeDisplacement" )]
		public static Vector2 TexCoord1BeforeDisplacement
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets texture coordinate 2 of the fragment before displacement. Works only in fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "texCoord2BeforeDisplacement" )]
		public static Vector2 TexCoord2BeforeDisplacement
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets texture coordinate unwrapped UV of the fragment before displacement. Works only in fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "unwrappedUVBeforeDisplacement" )]
		public static Vector2 UnwrappedUVBeforeDisplacement
		{
			get { return Vector2.Zero; }
		}

		/// <summary>
		/// Gets color 0 of the vertex or of the fragment. Works only in vertex and fragment shaders.
		/// </summary>
		[ShaderGenerationFunction( "color0" )]
		public static Vector4 Color0
		{
			get { return Vector4.Zero; }
		}

		/// <summary>
		/// Gets material instance parameter 1.
		/// </summary>
		[ShaderGenerationFunction( "instanceParameter1" )]
		public static Vector4 InstanceParameter1
		{
			get { return Vector4.Zero; }
		}

		/// <summary>
		/// Gets material instance parameter 2.
		/// </summary>
		[ShaderGenerationFunction( "instanceParameter2" )]
		public static Vector4 InstanceParameter2
		{
			get { return Vector4.Zero; }
		}
	}
}
