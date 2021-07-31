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

	/// <summary>
	/// Auxiliary class for working with <see cref="ParameterContainer"/>.
	/// </summary>
	public static class ParameterTypeUtility
	{
		static Dictionary<Type, ParameterType> typeByClassType;

		//

		static ParameterTypeUtility()
		{
			typeByClassType = new Dictionary<Type, ParameterType>();
			foreach( ParameterType type in Enum.GetValues( typeof( ParameterType ) ) )
			{
				Type classType2 = GetClassType( type );

				//!!!!why so special? why GpuTexture? because can't detect type of texture. special code for detection is used.
				//skip textures
				if( classType2 != null && classType2 != typeof( GpuTexture ) )
					typeByClassType[ classType2 ] = type;
			}
		}

		public static bool CanConvertToByteArray( ParameterType type )
		{
			if( type == ParameterType.Unknown || type == ParameterType.String || type == ParameterType.Object )
				return false;
			if( IsTexture( type ) )
				return false;
			return true;
		}

		public static bool IsTexture( ParameterType type )
		{
			return /*type == ParameterType.Texture1D || */type == ParameterType.Texture2D || type == ParameterType.Texture3D || type == ParameterType.TextureCube;
		}

		public static int GetElementSizeInBytes( ParameterType type )
		{
			//!!!!can be faster, use indexed array

			unsafe
			{
				switch( type )
				{
				case ParameterType.Boolean: return 1;
				case ParameterType.Byte: return 1;

				case ParameterType.Integer: return 4;
				case ParameterType.Vector2I: return 8;
				case ParameterType.RectangleI: return 16;
				case ParameterType.RangeI: return 8;
				case ParameterType.Vector3I: return 12;
				case ParameterType.Vector4I: return 16;

				case ParameterType.Float: return 4;
				case ParameterType.Radian: return 4;
				case ParameterType.Degree: return 4;
				case ParameterType.Vector2: return 8;
				case ParameterType.Range: return 8;
				case ParameterType.SphericalDirection: return 8;
				case ParameterType.Vector3: return 12;
				case ParameterType.Angles: return 12;
				case ParameterType.Vector4: return 16;
				case ParameterType.ColorValue: return 16;
				case ParameterType.Rectangle: return 16;
				case ParameterType.Quaternion: return 16;
				case ParameterType.Bounds: return sizeof( BoundsF );
				case ParameterType.Box: return sizeof( BoxF );
				case ParameterType.Capsule: return sizeof( Capsule );
				case ParameterType.Cone: return sizeof( Cone );
				case ParameterType.Line: return sizeof( Line3 );
				case ParameterType.Plane: return sizeof( PlaneF );
				case ParameterType.Ray: return sizeof( RayF );
				case ParameterType.Matrix2x2: return 2 * 2 * 4;
				//case ParameterType.Matrix2x3: return 2 * 3 * 4;
				//case ParameterType.Matrix2x4: return 2 * 4 * 4;
				//case ParameterType.Matrix3x2: return 3 * 2 * 4;
				case ParameterType.Matrix3x3: return 3 * 3 * 4;
				//case ParameterType.Matrix3x4: return 3 * 4 * 4;
				//case ParameterType.Matrix4x2: return 4 * 2 * 4;
				//case ParameterType.Matrix4x3: return 4 * 3 * 4;
				case ParameterType.Matrix4x4: return 4 * 4 * 4;

					//case ParameterType.Double: return 8;
					//case ParameterType.RadianD: return 8;
					//case ParameterType.DegreeD: return 8;
					//case ParameterType.Vector2D: return 16;
					//case ParameterType.RangeD: return 16;
					//case ParameterType.SphereDirectionD: return 16;
					//case ParameterType.Vector3D: return 24;
					//case ParameterType.AnglesD: return sizeof( Angles );
					//case ParameterType.Vector4D: return sizeof( Vec4F );
					////case AnyDataParameterType.ColorValueD: return 32;
					//case ParameterType.RectangleD: return 32;
					//case ParameterType.QuaternionD: return sizeof( Quat );
					//case ParameterType.BoundsD: return sizeof( Bounds );
					//case ParameterType.BoxD: return sizeof( Box );
					//case ParameterType.CapsuleD: return sizeof( Capsule );
					//case ParameterType.ConeD: return sizeof( Cone );
					//case ParameterType.LineD: return sizeof( Line3 );
					//case ParameterType.PlaneD: return sizeof( Plane );
					//case ParameterType.RayD: return sizeof( Ray );
					//case ParameterType.Matrix2x2D: return 2 * 2 * 8;
					//case ParameterType.Matrix2x3D: return 2 * 3 * 8;
					//case ParameterType.Matrix2x4D: return 2 * 4 * 8;
					//case ParameterType.Matrix3x2D: return 3 * 2 * 8;
					//case ParameterType.Matrix3x3D: return 3 * 3 * 8;
					//case ParameterType.Matrix3x4D: return 3 * 4 * 8;
					//case ParameterType.Matrix4x2D: return 4 * 2 * 8;
					//case ParameterType.Matrix4x3D: return 4 * 3 * 8;
					//case ParameterType.Matrix4x4D: return 4 * 4 * 8;
				}

				return 0;
			}
		}

		public static Type GetClassType( ParameterType type )
		{
			switch( type )
			{
			case ParameterType.Object: return typeof( object );
			case ParameterType.String: return typeof( string );
			case ParameterType.Boolean: return typeof( bool );
			case ParameterType.Byte: return typeof( byte );

			case ParameterType.Integer: return typeof( int );
			case ParameterType.Vector2I: return typeof( Vector2I );
			case ParameterType.RectangleI: return typeof( RectangleI );
			case ParameterType.RangeI: return typeof( RangeI );
			case ParameterType.Vector3I: return typeof( Vector3I );
			case ParameterType.Vector4I: return typeof( Vector4I );

			case ParameterType.Float: return typeof( float );
			case ParameterType.Radian: return typeof( RadianF );
			case ParameterType.Degree: return typeof( DegreeF );
			case ParameterType.Vector2: return typeof( Vector2F );
			case ParameterType.Range: return typeof( RangeF );
			case ParameterType.SphericalDirection: return typeof( SphericalDirectionF );
			case ParameterType.Vector3: return typeof( Vector3F );
			case ParameterType.Angles: return typeof( Angles );
			case ParameterType.Vector4: return typeof( Vector4F );
			case ParameterType.ColorValue: return typeof( ColorValue );
			case ParameterType.Rectangle: return typeof( RectangleF );
			case ParameterType.Quaternion: return typeof( QuaternionF );
			case ParameterType.Bounds: return typeof( BoundsF );
			case ParameterType.Box: return typeof( BoxF );
			case ParameterType.Capsule: return typeof( Capsule );
			case ParameterType.Cone: return typeof( Cone );
			case ParameterType.Line: return typeof( Line3 );
			case ParameterType.Plane: return typeof( PlaneF );
			case ParameterType.Ray: return typeof( RayF );
			case ParameterType.Matrix2x2: return typeof( Matrix2F );
			//case AnyDataParameterType.Matrix2x3: return typeof( string );
			//case AnyDataParameterType.Matrix2x4: return typeof( string );
			//case AnyDataParameterType.Matrix3x2: return typeof( string );
			case ParameterType.Matrix3x3: return typeof( Matrix3F );
			//case AnyDataParameterType.Matrix3x4: return typeof( string );
			//case AnyDataParameterType.Matrix4x2: return typeof( string );
			//case AnyDataParameterType.Matrix4x3: return typeof( string );
			case ParameterType.Matrix4x4: return typeof( Matrix4F );

			//case ParameterType.Double: return typeof( double );
			//case ParameterType.RadianD: return typeof( Radian );
			//case ParameterType.DegreeD: return typeof( Degree );
			//case ParameterType.Vector2D: return typeof( Vec2 );
			//case ParameterType.RangeD: return typeof( Range );
			//case ParameterType.SphereDirectionD: return typeof( SphereDir );
			//case ParameterType.Vector3D: return typeof( Vec3 );
			//case ParameterType.AnglesD: return typeof( Angles );
			//case ParameterType.Vector4D: return typeof( Vec4 );
			////case AnyDataParameterType.ColorValueD: return typeof( ColorValueD );
			//case ParameterType.RectangleD: return typeof( Rect );
			//case ParameterType.QuaternionD: return typeof( Quat );
			//case ParameterType.BoundsD: return typeof( Bounds );
			//case ParameterType.BoxD: return typeof( Box );
			//case ParameterType.CapsuleD: return typeof( Capsule );
			//case ParameterType.ConeD: return typeof( Cone );
			//case ParameterType.LineD: return typeof( Line3 );
			//case ParameterType.PlaneD: return typeof( Plane );
			//case ParameterType.RayD: return typeof( Ray );
			//case ParameterType.Matrix2x2D: return typeof( Mat2 );
			////case AnyDataParameterType.Matrix2x3D: return typeof( string );
			////case AnyDataParameterType.Matrix2x4D: return typeof( string );
			////case AnyDataParameterType.Matrix3x2D: return typeof( string );
			//case ParameterType.Matrix3x3D: return typeof( Mat3 );
			////case AnyDataParameterType.Matrix3x4D: return typeof( string );
			////case AnyDataParameterType.Matrix4x2D: return typeof( string );
			////case AnyDataParameterType.Matrix4x3D: return typeof( string );
			//case ParameterType.Matrix4x4D: return typeof( Mat4 );

			//case ParameterType.Texture1D: return typeof( GpuTexture );
			case ParameterType.Texture2D: return typeof( GpuTexture );
			case ParameterType.Texture3D: return typeof( GpuTexture );
			case ParameterType.TextureCube: return typeof( GpuTexture );
			}

			return null;
		}

		public static ParameterType DetectTypeByValue( object value )
		{
			if( value is string )
				return ParameterType.String;

			GpuTexture texture = value as GpuTexture;
			if( texture != null )
			{
				switch( texture.TextureType )
				{
				//case Component_Texture.TypeEnum._1D: return ParameterType.Texture1D;
				case Component_Image.TypeEnum._2D: return ParameterType.Texture2D;
				case Component_Image.TypeEnum._3D: return ParameterType.Texture3D;
				case Component_Image.TypeEnum.Cube: return ParameterType.TextureCube;
				}
			}

			return DetectTypeByClassType( value.GetType() );
		}

		public static ParameterType DetectTypeByClassType( Type classType )
		{
			ParameterType result;
			if( typeByClassType.TryGetValue( classType, out result ) )
				return result;
			return ParameterType.Unknown;
		}
	}
}
