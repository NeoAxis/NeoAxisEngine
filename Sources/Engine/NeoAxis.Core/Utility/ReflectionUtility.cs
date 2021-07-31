// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary class for working with .NET reflection.
	/// </summary>
	public static class ReflectionUtility
	{
		public static ConstructorInfo GetSuitableConstructor( Type classType, object[] constructorParams, bool checkNonPublic )
		{
			int iterations = checkNonPublic ? 2 : 1;
			for( int iter = 0; iter < iterations; iter++ )
			{
				BindingFlags flags;
				if( iter == 0 )
					flags = BindingFlags.Public | BindingFlags.Instance;
				else
					flags = BindingFlags.NonPublic | BindingFlags.Instance;

				var constructors = classType.GetConstructors( flags );
				foreach( var constructor in constructors )
				{
					var parameters = constructor.GetParameters();
					if( parameters.Length == constructorParams.Length )
					{
						for( int n = 0; n < parameters.Length; n++ )
						{
							var type = parameters[ n ].ParameterType;
							var value = constructorParams[ n ];

							if( value != null )
							{
								if( !type.IsAssignableFrom( value.GetType() ) )
									goto next;
							}
							else
							{
								if( type.IsValueType )
									goto next;
							}
						}

						return constructor;
					}

					next:;
				}
			}

			return null;
		}

		//!!!!

		// PropertyInfo/FieldInfo extensions.

		internal static void SetValue( this MemberInfo member, object obj, object value )
		{
			if( member.MemberType == MemberTypes.Property )
				( (PropertyInfo)member ).SetValue( obj, value, null );
			else if( member.MemberType == MemberTypes.Field )
				( (FieldInfo)member ).SetValue( obj, value );
			else
				throw new Exception( "Property must be of type FieldInfo or PropertyInfo" );
		}

		internal static object GetValue( this MemberInfo member, object obj )
		{
			if( member.MemberType == MemberTypes.Property )
				return ( (PropertyInfo)member ).GetValue( obj, null );
			else if( member.MemberType == MemberTypes.Field )
				return ( (FieldInfo)member ).GetValue( obj );
			else
				throw new Exception( "Property must be of type FieldInfo or PropertyInfo" );
		}

		internal static Type GetUnderlyingType( this MemberInfo member )
		{
			switch( member.MemberType )
			{
			case MemberTypes.Field:
				return ( (FieldInfo)member ).FieldType;
			case MemberTypes.Property:
				return ( (PropertyInfo)member ).PropertyType;
			case MemberTypes.Event:
				return ( (EventInfo)member ).EventHandlerType;
			default:
				throw new ArgumentException( "MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", "member" );
			}
		}

		public static bool PropertyIsStatic( PropertyInfo property )
		{
			var accessors = property.GetAccessors();
			if( accessors.Length == 0 )
				accessors = property.GetAccessors( true );
			return accessors.Length != 0 && accessors[ 0 ].IsStatic;
		}
	}
}
