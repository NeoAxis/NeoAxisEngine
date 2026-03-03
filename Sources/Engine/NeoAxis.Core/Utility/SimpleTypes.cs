// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

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
		public delegate object ReadTypeDelegate( ArrayDataReader reader );
		public delegate void WriteTypeDelegate( ArrayDataWriter writer, object value );

		//

		/// <summary>
		/// Represents an item of a type for <see cref="SimpleTypes"/>.
		/// </summary>
		public class TypeItem
		{
			Type type;
			ParseTypeDelegate parseFunction;
			object defaultValue;
			ReadTypeDelegate readFunction;
			WriteTypeDelegate writeFunction;

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

			public ReadTypeDelegate ReadFunction
			{
				get { return readFunction; }
			}

			public WriteTypeDelegate WriteFunction
			{
				get { return writeFunction; }
			}

			internal TypeItem( Type type, ParseTypeDelegate parseFunction, object defaultValue, ReadTypeDelegate readFunction, WriteTypeDelegate writeFunction )
			{
				this.type = type;
				this.parseFunction = parseFunction;
				this.defaultValue = defaultValue;
				this.readFunction = readFunction;
				this.writeFunction = writeFunction;
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
			}, "",
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadString();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (string)value );
			} );

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
			}, false,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadBoolean();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (bool)value );
			} );

			//sbyte
			RegisterType( typeof( sbyte ), delegate ( string value ) { return sbyte.Parse( value ); }, 0,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadSByte();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (sbyte)value );
			} );

			//byte
			RegisterType( typeof( byte ), delegate ( string value ) { return byte.Parse( value ); }, 0,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadByte();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (byte)value );
			} );

			//char
			RegisterType( typeof( char ), delegate ( string value ) { return char.Parse( value ); }, 0,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadChar();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (char)value );
			} );

			//short
			RegisterType( typeof( short ), delegate ( string value ) { return short.Parse( value ); }, 0,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadShort();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (short)value );
			} );

			//ushort
			RegisterType( typeof( ushort ), delegate ( string value ) { return ushort.Parse( value ); }, 0,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadUShort();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (ushort)value );
			} );

			//int
			RegisterType( typeof( int ), delegate ( string value ) { return int.Parse( value ); }, 0,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadInt();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (int)value );
			} );

			//uint
			RegisterType( typeof( uint ), delegate ( string value ) { return uint.Parse( value ); }, (uint)0,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadUInt();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (uint)value );
			} );

			//long
			RegisterType( typeof( long ), delegate ( string value ) { return long.Parse( value ); }, (long)0,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadLong();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (long)value );
			} );

			//ulong
			RegisterType( typeof( ulong ), delegate ( string value ) { return ulong.Parse( value ); }, (ulong)0,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadULong();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (ulong)value );
			} );

			//float
			RegisterType( typeof( float ), delegate ( string value ) { return float.Parse( value ); }, 0.0f,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadFloat();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (float)value );
			} );

			//double
			RegisterType( typeof( double ), delegate ( string value ) { return double.Parse( value ); }, 0.0,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadDouble();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (double)value );
			} );

			//decimal
			RegisterType( typeof( decimal ), delegate ( string value ) { return decimal.Parse( value ); }, (decimal)0.0,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadDecimal();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (decimal)value );
			} );

			//Vector2F
			RegisterType( typeof( Vector2F ), delegate ( string value ) { return Vector2F.Parse( value ); }, Vector2F.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadVector2F();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Vector2F)value );
			} );

			//RangeF
			RegisterType( typeof( RangeF ), delegate ( string value ) { return RangeF.Parse( value ); }, RangeF.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadRangeF();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (RangeF)value );
			} );

			//Vector3F
			RegisterType( typeof( Vector3F ), delegate ( string value ) { return Vector3F.Parse( value ); }, Vector3F.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadVector3F();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Vector3F)value );
			} );

			//Vector4F
			RegisterType( typeof( Vector4F ), delegate ( string value ) { return Vector4F.Parse( value ); }, Vector4F.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadVector4F();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Vector4F)value );
			} );

			//BoundsF
			RegisterType( typeof( BoundsF ), delegate ( string value ) { return BoundsF.Parse( value ); }, BoundsF.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadBoundsF();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (BoundsF)value );
			} );

			//QuaternionF
			RegisterType( typeof( QuaternionF ), delegate ( string value ) { return QuaternionF.Parse( value ); }, QuaternionF.Identity,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadQuaternionF();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (QuaternionF)value );
			} );

			//ColorValue
			RegisterType( typeof( ColorValue ), delegate ( string value ) { return ColorValue.Parse( value ); }, ColorValue.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadColorValue();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (ColorValue)value );
			} );

			//ColorValuePowered
			RegisterType( typeof( ColorValuePowered ), delegate ( string value ) { return ColorValuePowered.Parse( value ); }, ColorValuePowered.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadColorValuePowered();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (ColorValuePowered)value );
			} );

			//ColorPacked
			RegisterType( typeof( ColorByte ), delegate ( string value ) { return ColorByte.Parse( value ); }, ColorByte.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadColorByte();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (ColorByte)value );
			} );

			//SphereDirectionF
			RegisterType( typeof( SphericalDirectionF ), delegate ( string value ) { return SphericalDirectionF.Parse( value ); }, SphericalDirectionF.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadSphericalDirectionF();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (SphericalDirectionF)value );
			} );

			//Vector2I
			RegisterType( typeof( Vector2I ), delegate ( string value ) { return Vector2I.Parse( value ); }, Vector2I.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadVector2I();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Vector2I)value );
			} );

			//Vector3I
			RegisterType( typeof( Vector3I ), delegate ( string value ) { return Vector3I.Parse( value ); }, Vector3I.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadVector3I();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Vector3I)value );
			} );

			//Vector4I
			RegisterType( typeof( Vector4I ), delegate ( string value ) { return Vector4I.Parse( value ); }, Vector4I.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadVector4I();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Vector4I)value );
			} );

			//RectangleF
			RegisterType( typeof( RectangleF ), delegate ( string value ) { return RectangleF.Parse( value ); }, RectangleF.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadRectangleF();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (RectangleF)value );
			} );

			//RectangleI
			RegisterType( typeof( RectangleI ), delegate ( string value ) { return RectangleI.Parse( value ); }, RectangleI.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadRectangleI();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (RectangleI)value );
			} );

			//DegreeF
			RegisterType( typeof( DegreeF ), delegate ( string value ) { return DegreeF.Parse( value ); }, DegreeF.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadDegreeF();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (DegreeF)value );
			} );

			//RadianF
			RegisterType( typeof( RadianF ), delegate ( string value ) { return RadianF.Parse( value ); }, RadianF.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadRadianF();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (RadianF)value );
			} );

			//Vector2
			RegisterType( typeof( Vector2 ), delegate ( string value ) { return Vector2.Parse( value ); }, Vector2.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadVector2();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Vector2)value );
			} );

			//Range
			RegisterType( typeof( Range ), delegate ( string value ) { return Range.Parse( value ); }, Range.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadRange();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Range)value );
			} );

			//RangeI
			RegisterType( typeof( RangeI ), delegate ( string value ) { return RangeI.Parse( value ); }, RangeI.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadRangeI();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (RangeI)value );
			} );

			//Vector3
			RegisterType( typeof( Vector3 ), delegate ( string value ) { return Vector3.Parse( value ); }, Vector3.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadVector3();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Vector3)value );
			} );

			//Vector4
			RegisterType( typeof( Vector4 ), delegate ( string value ) { return Vector4.Parse( value ); }, Vector4.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadVector4();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Vector4)value );
			} );

			//Bounds
			RegisterType( typeof( Bounds ), delegate ( string value ) { return Bounds.Parse( value ); }, Bounds.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadBounds();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Bounds)value );
			} );

			//Quaternion
			RegisterType( typeof( Quaternion ), delegate ( string value ) { return Quaternion.Parse( value ); }, Quaternion.Identity,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadQuaternion();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Quaternion)value );
			} );

			//SphericalDirection
			RegisterType( typeof( SphericalDirection ), delegate ( string value ) { return SphericalDirection.Parse( value ); }, SphericalDirection.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadSphericalDirection();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (SphericalDirection)value );
			} );

			//Rectangle
			RegisterType( typeof( Rectangle ), delegate ( string value ) { return Rectangle.Parse( value ); }, Rectangle.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadRectangle();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Rectangle)value );
			} );

			//Degree
			RegisterType( typeof( Degree ), delegate ( string value ) { return Degree.Parse( value ); }, Degree.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadDegree();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Degree)value );
			} );

			//Radian
			RegisterType( typeof( Radian ), delegate ( string value ) { return Radian.Parse( value ); }, Radian.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadRadian();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Radian)value );
			} );

			//AnglesF
			RegisterType( typeof( AnglesF ), delegate ( string value ) { return AnglesF.Parse( value ); }, AnglesF.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadAnglesF();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (AnglesF)value );
			} );

			//Angles
			RegisterType( typeof( Angles ), delegate ( string value ) { return Angles.Parse( value ); }, Angles.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadAngles();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Angles)value );
			} );

			//Matrix2F
			RegisterType( typeof( Matrix2F ), delegate ( string value ) { return Matrix2F.Parse( value ); }, Matrix2F.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadMatrix2F();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Matrix2F)value );
			} );

			//Matrix2
			RegisterType( typeof( Matrix2 ), delegate ( string value ) { return Matrix2.Parse( value ); }, Matrix2.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadMatrix2();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Matrix2)value );
			} );

			//Matrix3F
			RegisterType( typeof( Matrix3F ), delegate ( string value ) { return Matrix3F.Parse( value ); }, Matrix3F.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadMatrix3F();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Matrix3F)value );
			} );

			//Matrix3
			RegisterType( typeof( Matrix3 ), delegate ( string value ) { return Matrix3.Parse( value ); }, Matrix3.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadMatrix3();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Matrix3)value );
			} );

			//Matrix4F
			RegisterType( typeof( Matrix4F ), delegate ( string value ) { return Matrix4F.Parse( value ); }, Matrix4F.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadMatrix4F();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Matrix4F)value );
			} );

			//Matrix4
			RegisterType( typeof( Matrix4 ), delegate ( string value ) { return Matrix4.Parse( value ); }, Matrix4.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadMatrix4();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Matrix4)value );
			} );

			//PlaneF
			RegisterType( typeof( PlaneF ), delegate ( string value ) { return PlaneF.Parse( value ); }, PlaneF.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadPlaneF();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (PlaneF)value );
			} );

			//Plane
			RegisterType( typeof( Plane ), delegate ( string value ) { return Plane.Parse( value ); }, Plane.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadPlane();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Plane)value );
			} );

			//Transform
			RegisterType( typeof( Transform ), delegate ( string value ) { return Transform.Parse( value ); }, Transform.Identity,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadTransform();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Transform)value );
			} );

			//SphereF
			RegisterType( typeof( SphereF ), delegate ( string value ) { return SphereF.Parse( value ); }, SphereF.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadSphereF();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (SphereF)value );
			} );

			//Sphere
			RegisterType( typeof( Sphere ), delegate ( string value ) { return Sphere.Parse( value ); }, Sphere.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadSphere();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (Sphere)value );
			} );

			//UIMeasureValueDouble
			RegisterType( typeof( UIMeasureValueDouble ), delegate ( string value ) { return UIMeasureValueDouble.Parse( value ); }, new UIMeasureValueDouble(),
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadUIMeasureValueDouble();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (UIMeasureValueDouble)value );
			} );

			//UIMeasureValueVector2
			RegisterType( typeof( UIMeasureValueVector2 ), delegate ( string value ) { return UIMeasureValueVector2.Parse( value ); }, new UIMeasureValueVector2(),
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadUIMeasureValueVector2();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (UIMeasureValueVector2)value );
			} );

			//UIMeasureValueRectangle
			RegisterType( typeof( UIMeasureValueRectangle ), delegate ( string value ) { return UIMeasureValueRectangle.Parse( value ); }, new UIMeasureValueRectangle(),
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadUIMeasureValueRectangle();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (UIMeasureValueRectangle)value );
			} );

			//RangeVector3F
			RegisterType( typeof( RangeVector3F ), delegate ( string value ) { return RangeVector3F.Parse( value ); }, RangeVector3F.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadRangeVector3F();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (RangeVector3F)value );
			} );

			//RangeColorValue
			RegisterType( typeof( RangeColorValue ), delegate ( string value ) { return RangeColorValue.Parse( value ); }, RangeColorValue.Zero,
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadRangeColorValue();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (RangeColorValue)value );
			} );

			//DateTime
			RegisterType( typeof( DateTime ), delegate ( string value ) { return DateTime.Parse( value ); }, new DateTime(),
			delegate ( ArrayDataReader reader )
			{
				return reader.ReadDateTime();
			},
			delegate ( ArrayDataWriter writer, object value )
			{
				writer.Write( (DateTime)value );
			} );



			////ObjectId
			//RegisterType( typeof( ObjectId ), delegate ( string value ) { return new ObjectId( value ); }, ObjectId.Empty,
			//delegate ( ArrayDataReader reader )
			//{
			//	return reader.ReadObjectId();
			//},
			//delegate ( ArrayDataWriter writer, object value )
			//{
			//	writer.Write( (ObjectId)value );
			//} );

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
			types.TryGetValue( type, out var item );
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
				//remove fatals?
				Log.Fatal( "SimpleTypesUtils: GetDefaultValue: Enum types are not supported." );
				return null;
			}

			TypeItem item;
			if( !types.TryGetValue( type, out item ) )
				return null;
			return item.DefaultValue;
		}

		public static void RegisterType( Type type, ParseTypeDelegate parseFunction, object defaultValue, ReadTypeDelegate readFunction, WriteTypeDelegate writeFunction )
		{
			//this copying for multithreading support
			var newTypes = new Dictionary<Type, TypeItem>( types );
			newTypes.Add( type, new TypeItem( type, parseFunction, defaultValue, readFunction, writeFunction ) );

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

		public static void Write( TypeItem type, ArrayDataWriter writer, object value )
		{
			type.WriteFunction( writer, value );
		}

		public static object Read( TypeItem type, ArrayDataReader reader, Type typeToRead )
		{
			return type.ReadFunction( reader );
		}

		public static TypeItem GetTypeItem( string typeName )
		{
			//!!!!slowly

			foreach( var typeItem in types.Values )
			{
				if( typeItem.Type.FullName == typeName )
					return typeItem;
			}

			return null;
		}

	}
}
