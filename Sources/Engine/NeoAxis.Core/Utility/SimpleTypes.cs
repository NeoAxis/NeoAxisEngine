// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Class for unified work with simple types. Mainly used for converting to string and back.
	/// </summary>
	public static class SimpleTypes
	{
		static Dictionary<Type, TypeItem> types = new Dictionary<Type, TypeItem>();

		static Dictionary<Type, Func<object, object>> convertDoubleToFloatTypes = new Dictionary<Type, Func<object, object>>();

		//

		public delegate object ParseTypeDelegate( string value );

		//

		/// <summary>
		/// Represents an item of a type for <see cref="SimpleTypes"/>.
		/// </summary>
		public class TypeItem
		{
			internal Type type;
			internal ParseTypeDelegate parseFunction;
			internal object defaultValue;

			//

			public Type Type
			{
				get { return type; }
			}

			public ParseTypeDelegate ParseFunction
			{
				get { return parseFunction; }
			}

			public object DefaultValue
			{
				get { return defaultValue; }
			}

			internal TypeItem( Type type, ParseTypeDelegate parseFunction, object defaultValue )
			{
				this.type = type;
				this.parseFunction = parseFunction;
				this.defaultValue = defaultValue;
			}
		}

		//

		static SimpleTypes()
		{
			//string
			RegisterType( typeof( string ), delegate ( string value )
			{
				if( value == null )
				{
					return "";
					//throw new Exception( "GetSimpleTypeValue: string type, value = null" );
				}
				return value;
			}, "" );

			//bool
			RegisterType( typeof( bool ), delegate ( string value )
			{
				string lower = value.ToLower();
				if( value == "1" || lower == "yes" || lower == "true" )
					return true;
				else if( value == "0" || lower == "no" || lower == "false" )
					return false;
				else
					return bool.Parse( value );
			}, false );

			//sbyte
			RegisterType( typeof( sbyte ), delegate ( string value ) { return sbyte.Parse( value ); }, 0 );

			//byte
			RegisterType( typeof( byte ), delegate ( string value ) { return byte.Parse( value ); }, 0 );

			//char
			RegisterType( typeof( char ), delegate ( string value ) { return char.Parse( value ); }, 0 );

			//short
			RegisterType( typeof( short ), delegate ( string value ) { return short.Parse( value ); }, 0 );

			//ushort
			RegisterType( typeof( ushort ), delegate ( string value ) { return ushort.Parse( value ); }, 0 );

			//int
			RegisterType( typeof( int ), delegate ( string value ) { return int.Parse( value ); }, 0 );

			//uint
			RegisterType( typeof( uint ), delegate ( string value ) { return uint.Parse( value ); }, (uint)0 );

			//long
			RegisterType( typeof( long ), delegate ( string value ) { return long.Parse( value ); }, (long)0 );

			//ulong
			RegisterType( typeof( ulong ), delegate ( string value ) { return ulong.Parse( value ); }, (ulong)0 );

			//float
			RegisterType( typeof( float ), delegate ( string value ) { return float.Parse( value ); }, 0.0f );

			//double
			RegisterType( typeof( double ), delegate ( string value ) { return double.Parse( value ); }, 0.0 );

			//decimal
			RegisterType( typeof( decimal ), delegate ( string value ) { return decimal.Parse( value ); }, (decimal)0.0 );

			//Vec2
			RegisterType( typeof( Vector2F ), delegate ( string value ) { return Vector2F.Parse( value ); }, Vector2F.Zero );

			//Range
			RegisterType( typeof( RangeF ), delegate ( string value ) { return RangeF.Parse( value ); }, RangeF.Zero );

			//Vec3
			RegisterType( typeof( Vector3F ), delegate ( string value ) { return Vector3F.Parse( value ); }, Vector3F.Zero );

			//Vec4
			RegisterType( typeof( Vector4F ), delegate ( string value ) { return Vector4F.Parse( value ); }, Vector4F.Zero );

			//Bounds
			RegisterType( typeof( BoundsF ), delegate ( string value ) { return BoundsF.Parse( value ); }, BoundsF.Zero );

			//Quat
			RegisterType( typeof( QuaternionF ), delegate ( string value ) { return QuaternionF.Parse( value ); }, QuaternionF.Identity );

			//ColorValue
			RegisterType( typeof( ColorValue ), delegate ( string value ) { return ColorValue.Parse( value ); }, ColorValue.Zero );

			//ColorValuePowered
			RegisterType( typeof( ColorValuePowered ), delegate ( string value ) { return ColorValuePowered.Parse( value ); }, ColorValuePowered.Zero );

			//ColorPacked
			RegisterType( typeof( ColorByte ), delegate ( string value ) { return ColorByte.Parse( value ); }, ColorByte.Zero );

			//SphereDir
			RegisterType( typeof( SphericalDirectionF ), delegate ( string value ) { return SphericalDirectionF.Parse( value ); }, SphericalDirectionF.Zero );

			//Vec2I
			RegisterType( typeof( Vector2I ), delegate ( string value ) { return Vector2I.Parse( value ); }, Vector2I.Zero );

			//Vec3I
			RegisterType( typeof( Vector3I ), delegate ( string value ) { return Vector3I.Parse( value ); }, Vector3I.Zero );

			//Vec4I
			RegisterType( typeof( Vector4I ), delegate ( string value ) { return Vector4I.Parse( value ); }, Vector4I.Zero );

			//Rect
			RegisterType( typeof( RectangleF ), delegate ( string value ) { return RectangleF.Parse( value ); }, RectangleF.Zero );

			//RectI
			RegisterType( typeof( RectangleI ), delegate ( string value ) { return RectangleI.Parse( value ); }, RectangleI.Zero );

			//Degree
			RegisterType( typeof( DegreeF ), delegate ( string value ) { return DegreeF.Parse( value ); }, DegreeF.Zero );

			//Radian
			RegisterType( typeof( RadianF ), delegate ( string value ) { return RadianF.Parse( value ); }, RadianF.Zero );

			//Vec2D
			RegisterType( typeof( Vector2 ), delegate ( string value ) { return Vector2.Parse( value ); }, Vector2.Zero );

			//RangeD
			RegisterType( typeof( Range ), delegate ( string value ) { return Range.Parse( value ); }, Range.Zero );

			//RangeI
			RegisterType( typeof( RangeI ), delegate ( string value ) { return RangeI.Parse( value ); }, RangeI.Zero );

			//Vec3D
			RegisterType( typeof( Vector3 ), delegate ( string value ) { return Vector3.Parse( value ); }, Vector3.Zero );

			//Vec4D
			RegisterType( typeof( Vector4 ), delegate ( string value ) { return Vector4.Parse( value ); }, Vector4.Zero );

			//BoundsD
			RegisterType( typeof( Bounds ), delegate ( string value ) { return Bounds.Parse( value ); }, Bounds.Zero );

			//QuatD
			RegisterType( typeof( Quaternion ), delegate ( string value ) { return Quaternion.Parse( value ); }, Quaternion.Identity );

			//SphereDirD
			RegisterType( typeof( SphericalDirection ), delegate ( string value ) { return SphericalDirection.Parse( value ); }, SphericalDirection.Zero );

			//RectD
			RegisterType( typeof( Rectangle ), delegate ( string value ) { return Rectangle.Parse( value ); }, Rectangle.Zero );

			//DegreeD
			RegisterType( typeof( Degree ), delegate ( string value ) { return Degree.Parse( value ); }, Degree.Zero );

			//RadianD
			RegisterType( typeof( Radian ), delegate ( string value ) { return Radian.Parse( value ); }, Radian.Zero );

			//Angles
			RegisterType( typeof( AnglesF ), delegate ( string value ) { return AnglesF.Parse( value ); }, AnglesF.Zero );

			//AnglesD
			RegisterType( typeof( Angles ), delegate ( string value ) { return Angles.Parse( value ); }, Angles.Zero );

			//Mat2F
			RegisterType( typeof( Matrix2F ), delegate ( string value ) { return Matrix2F.Parse( value ); }, Matrix2F.Zero );

			//Mat2D
			RegisterType( typeof( Matrix2 ), delegate ( string value ) { return Matrix2.Parse( value ); }, Matrix2.Zero );

			//Mat3F
			RegisterType( typeof( Matrix3F ), delegate ( string value ) { return Matrix3F.Parse( value ); }, Matrix3F.Zero );

			//Mat3D
			RegisterType( typeof( Matrix3 ), delegate ( string value ) { return Matrix3.Parse( value ); }, Matrix3.Zero );

			//Mat4F
			RegisterType( typeof( Matrix4F ), delegate ( string value ) { return Matrix4F.Parse( value ); }, Matrix4F.Zero );

			//Mat4D
			RegisterType( typeof( Matrix4 ), delegate ( string value ) { return Matrix4.Parse( value ); }, Matrix4.Zero );

			//PlaneF
			RegisterType( typeof( PlaneF ), delegate ( string value ) { return PlaneF.Parse( value ); }, PlaneF.Zero );

			//PlaneD
			RegisterType( typeof( Plane ), delegate ( string value ) { return Plane.Parse( value ); }, Plane.Zero );

			//Transform
			RegisterType( typeof( Transform ), delegate ( string value ) { return Transform.Parse( value ); }, Transform.Identity );

			RegisterType( typeof( SphereF ), delegate ( string value ) { return SphereF.Parse( value ); }, SphereF.Zero );
			RegisterType( typeof( Sphere ), delegate ( string value ) { return Sphere.Parse( value ); }, Sphere.Zero );

			//UIMeasureValueDouble
			RegisterType( typeof( UIMeasureValueDouble ), delegate ( string value ) { return UIMeasureValueDouble.Parse( value ); }, new UIMeasureValueDouble() );

			//UIMeasureValueVec2
			RegisterType( typeof( UIMeasureValueVector2 ), delegate ( string value ) { return UIMeasureValueVector2.Parse( value ); }, new UIMeasureValueVector2() );

			//UIMeasureValueRect
			RegisterType( typeof( UIMeasureValueRectangle ), delegate ( string value ) { return UIMeasureValueRectangle.Parse( value ); }, new UIMeasureValueRectangle() );

			RegisterType( typeof( RangeVector3F ), delegate ( string value ) { return RangeVector3F.Parse( value ); }, RangeVector3F.Zero );
			RegisterType( typeof( RangeColorValue ), delegate ( string value ) { return RangeColorValue.Parse( value ); }, RangeColorValue.Zero );

			//no Parse methods. This is complex structures. This is not simple types? or just can't parse?
			//Box
			//Capsule
			//Cone
			//Line3
			//Line2
			//Ray
			//Frustum?

			//half?

			RegisterConvertDoubleToFloatTypes();
		}

		public static ICollection<TypeItem> Types
		{
			get { return types.Values; }
		}

		public static bool IsSimpleType( Type type )
		{
			if( typeof( Enum ).IsAssignableFrom( type ) )
				return true;

			return types.ContainsKey( type );
		}

		public static TypeItem GetTypeItem( Type type )
		{
			TypeItem item;
			types.TryGetValue( type, out item );
			return item;
		}

		/// <summary>
		/// Returns null means this is not simple type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static object ParseValue( Type type, string value )
		{
			if( typeof( Enum ).IsAssignableFrom( type ) )
				return Enum.Parse( type, value );

			TypeItem item;
			if( !types.TryGetValue( type, out item ) )
				return null;
			return item.ParseFunction( value );
		}

		public static bool TryParseValue<T>( string value, out T resultValue, out string error )
		{
			try
			{
				resultValue = (T)ParseValue( typeof( T ), value );
				error = null;
				return true;
			}
			catch( Exception e )
			{
				resultValue = default;
				error = e.Message;
				return false;
			}
		}

		public static object GetDefaultValue( Type type )
		{
			if( typeof( Enum ).IsAssignableFrom( type ) )
			{
				Log.Fatal( "SimpleTypesUtils: GetDefaultValue: Enum types are not supported." );
				return null;
			}

			TypeItem item;
			if( !types.TryGetValue( type, out item ) )
				return null;
			return item.DefaultValue;
		}

		public static void RegisterType( Type type, ParseTypeDelegate parseFunction, object defaultValue )
		{
			//this is multithreading support
			Dictionary<Type, TypeItem> newTypes = new Dictionary<Type, TypeItem>( types );
			newTypes.Add( type, new TypeItem( type, parseFunction, defaultValue ) );
			//update
			types = newTypes;

			//types.Add( type, new TypeItem( type, parseFunction, defaultValue ) );
		}

		public static Dictionary<Type, Func<object, object>> ConvertDoubleToFloatTypes
		{
			get { return convertDoubleToFloatTypes; }
		}

		static void RegisterConvertDoubleToFloatTypes()
		{
			convertDoubleToFloatTypes.Add( typeof( double ), delegate ( object p ) { return (float)(double)p; } );
			convertDoubleToFloatTypes.Add( typeof( Vector2 ), delegate ( object p ) { return ( (Vector2)p ).ToVector2F(); } );
			convertDoubleToFloatTypes.Add( typeof( Range ), delegate ( object p ) { return ( (Range)p ).ToRangeF(); } );
			convertDoubleToFloatTypes.Add( typeof( Vector3 ), delegate ( object p ) { return ( (Vector3)p ).ToVector3F(); } );
			convertDoubleToFloatTypes.Add( typeof( Vector4 ), delegate ( object p ) { return ( (Vector4)p ).ToVector4F(); } );
			convertDoubleToFloatTypes.Add( typeof( Bounds ), delegate ( object p ) { return ( (Bounds)p ).ToBoundsF(); } );
			convertDoubleToFloatTypes.Add( typeof( Quaternion ), delegate ( object p ) { return ( (Quaternion)p ).ToQuaternionF(); } );
			convertDoubleToFloatTypes.Add( typeof( SphericalDirection ), delegate ( object p ) { return ( (SphericalDirection)p ).ToSphericalDirectionF(); } );
			convertDoubleToFloatTypes.Add( typeof( Rectangle ), delegate ( object p ) { return ( (Rectangle)p ).ToRectangleF(); } );
			convertDoubleToFloatTypes.Add( typeof( Degree ), delegate ( object p ) { return ( (Degree)p ).ToDegreeF(); } );
			convertDoubleToFloatTypes.Add( typeof( Radian ), delegate ( object p ) { return ( (Radian)p ).ToRadianF(); } );
			convertDoubleToFloatTypes.Add( typeof( Angles ), delegate ( object p ) { return ( (Angles)p ).ToAnglesF(); } );
			convertDoubleToFloatTypes.Add( typeof( Matrix2 ), delegate ( object p ) { return ( (Matrix2)p ).ToMatrix2F(); } );
			convertDoubleToFloatTypes.Add( typeof( Matrix3 ), delegate ( object p ) { return ( (Matrix3)p ).ToMatrix3F(); } );
			convertDoubleToFloatTypes.Add( typeof( Matrix4 ), delegate ( object p ) { return ( (Matrix4)p ).ToMatrix4F(); } );
			convertDoubleToFloatTypes.Add( typeof( Plane ), delegate ( object p ) { return ( (Plane)p ).ToPlaneF(); } );
		}

		public static object ConvertDoubleToFloat( object value )
		{
			if( convertDoubleToFloatTypes.TryGetValue( value.GetType(), out Func<object, object> func ) )
				return func( value );
			return value;
		}
	}
}
