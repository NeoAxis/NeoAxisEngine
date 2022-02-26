// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// A helper class for working with .NET objects.
	/// </summary>
	public static class ObjectEx
	{
		public static object Select( object a, object b, bool pickA )
		{
			return pickA ? a : b;
		}

		static bool FindMethodWithEqualParameterTypes( object obj, string name, object[] parameters, out Metadata.Method methodVirtual, out MethodInfo methodNative )
		{
			methodVirtual = null;
			methodNative = null;

			var types = new Type[ parameters.Length ];
			for( int n = 0; n < parameters.Length; n++ )
			{
				if( parameters[ n ] == null )
					return false;
				types[ n ] = parameters[ n ].GetType();
			}

			var provider = obj as Metadata.IMetadataProvider;
			if( provider != null )
			{
				//virtual, public
				{
					string signature;
					var builder = new StringBuilder();
					builder.Append( "method:" );
					builder.Append( name );
					builder.Append( '(' );
					for( int n = 0; n < parameters.Length; n++ )
					{
						var type = MetadataManager.GetTypeOfNetType( types[ n ] );
						if( type == null )
							goto skip_virtual;

						if( n != 0 )
							builder.Append( ',' );
						builder.Append( type.Name );
					}
					builder.Append( ')' );
					signature = builder.ToString();

					methodVirtual = provider.MetadataGetMemberBySignature( signature ) as Metadata.Method;

					skip_virtual:;
				}

				if( methodVirtual == null )
				{
					//native, non public
					try
					{
						methodNative = obj.GetType().GetMethod( name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, types, null );
					}
					catch { }
				}
			}
			else
			{
				//native, public
				try
				{
					methodNative = obj.GetType().GetMethod( name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, types, null );
				}
				catch { }
			}

			return methodVirtual != null || methodNative != null;
		}

		static bool FindMethodWithoutEqualParameterTypes( object obj, string name, object[] parameters, out Metadata.Method methodVirtual, out MethodInfo methodNative, out object[] newParameters )
		{
			methodVirtual = null;
			methodNative = null;
			newParameters = null;

			//!!!!ref, out support
			//!!!!default parameters

			//virtual, public
			var provider = obj as Metadata.IMetadataProvider;
			if( provider != null )
			{
				foreach( var member in provider.MetadataGetMembers() )
				{
					var method = member as Metadata.Method;
					if( method != null && method.Name == name )
					{
						var inputParams = method.GetInputParameters();
						if( inputParams.Length == parameters.Length )
						{
							for( int nParam = 0; nParam < inputParams.Length; nParam++ )
							{
								var demandedType = inputParams[ nParam ].Type.GetNetType();
								var value = parameters[ nParam ];

								if( value != null )
								{
									if( !demandedType.IsAssignableFrom( value.GetType() ) && !MetadataManager.CanAutoConvertType( value.GetType(), demandedType ) )
										goto next_member;
								}
								else
								{
									if( demandedType.IsValueType )
										goto next_member;
								}
							}

							//found
							{
								methodVirtual = method;

								newParameters = new object[ inputParams.Length ];
								for( int nParam = 0; nParam < inputParams.Length; nParam++ )
								{
									var demandedType = inputParams[ nParam ].Type.GetNetType();
									var value = parameters[ nParam ];

									if( value != null )
									{
										if( demandedType == value.GetType() )
											newParameters[ nParam ] = value;
										else if( demandedType.IsAssignableFrom( value.GetType() ) )
											newParameters[ nParam ] = Convert.ChangeType( value, demandedType );
										else
											newParameters[ nParam ] = MetadataManager.AutoConvertValue( value, demandedType );
									}
								}

								break;
							}
						}
					}

					next_member:;
				}
			}

			//native
			if( methodVirtual == null )
			{
				var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
				if( provider == null )
					bindingFlags |= BindingFlags.Public;

				foreach( var method in obj.GetType().GetMethods( bindingFlags ) )
				{
					if( method.Name == name )
					{
						var inputParams = new List<ParameterInfo>();
						foreach( var p in method.GetParameters() )
						{
							if( !p.IsRetval )
								inputParams.Add( p );
						}

						if( inputParams.Count == parameters.Length )
						{
							for( int nParam = 0; nParam < inputParams.Count; nParam++ )
							{
								var demandedType = inputParams[ nParam ].ParameterType;
								var value = parameters[ nParam ];

								if( value != null )
								{
									if( !demandedType.IsAssignableFrom( value.GetType() ) && !MetadataManager.CanAutoConvertType( value.GetType(), demandedType ) )
										goto next_member2;
								}
								else
								{
									if( demandedType.IsValueType )
										goto next_member2;
								}
							}

							//found
							{
								methodNative = method;

								newParameters = new object[ inputParams.Count ];
								for( int nParam = 0; nParam < inputParams.Count; nParam++ )
								{
									var demandedType = inputParams[ nParam ].ParameterType;
									var value = parameters[ nParam ];

									if( value != null )
									{
										if( demandedType == value.GetType() )
											newParameters[ nParam ] = value;
										else if( demandedType.IsAssignableFrom( value.GetType() ) )
											newParameters[ nParam ] = Convert.ChangeType( value, demandedType );
										else
											newParameters[ nParam ] = MetadataManager.AutoConvertValue( value, demandedType );
									}
								}

								break;
							}
						}
					}

					next_member2:;
				}
			}

			return methodVirtual != null || methodNative != null;
		}

		static bool FindMethodWithoutParameters( object obj, string name, out Metadata.Method methodVirtual, out MethodInfo methodNative )
		{
			methodVirtual = null;
			methodNative = null;

			var provider = obj as Metadata.IMetadataProvider;
			if( provider != null )
			{
				//virtual, public
				methodVirtual = provider.MetadataGetMemberBySignature( $"method:{name}()" ) as Metadata.Method;

				if( methodVirtual == null )
				{
					//native, non public
					try
					{
						methodNative = obj.GetType().GetMethod( name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, Array.Empty<Type>(), null );
					}
					catch { }
				}
			}
			else
			{
				//native, public
				try
				{
					methodNative = obj.GetType().GetMethod( name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, Array.Empty<Type>(), null );
				}
				catch { }
			}

			return methodVirtual != null || methodNative != null;
		}

		internal static bool FindMethod( object obj, string name, object[] parameters, out Metadata.Method methodVirtual, out MethodInfo methodNative, out object[] newParameters )
		{
			if( parameters.Length != 0 )
			{
				if( FindMethodWithEqualParameterTypes( obj, name, parameters, out methodVirtual, out methodNative ) )
				{
					newParameters = parameters;
					return true;
				}

				if( FindMethodWithoutEqualParameterTypes( obj, name, parameters, out methodVirtual, out methodNative, out newParameters ) )
					return true;
			}
			else
			{
				if( FindMethodWithoutParameters( obj, name, out methodVirtual, out methodNative ) )
				{
					newParameters = Array.Empty<object>();
					return true;
				}
			}

			methodVirtual = null;
			methodNative = null;
			newParameters = null;
			return false;
		}

		internal static object MethodInvoke( object obj, Metadata.Method methodVirtual, MethodInfo methodNative, object[] parameters )
		{
			if( methodVirtual != null )
				return methodVirtual.Invoke( methodVirtual.Static ? null : obj, parameters );
			else
				return methodNative.Invoke( methodNative.IsStatic ? null : obj, parameters );
		}

		/// <summary>
		/// Calls the object method by name.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="name"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static object MethodInvoke( this object obj, string name, params object[] parameters ) //object[] parameters = null
		{
			if( parameters == null )
				parameters = Array.Empty<object>();

			if( FindMethod( obj, name, parameters, out var methodVirtual, out var methodNative, out var newParameters ) )
				return MethodInvoke( obj, methodVirtual, methodNative, newParameters );
			else
				return null;
		}

		/// <summary>
		/// Gets the value of the object property by name.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="name"></param>
		/// <param name="indexers"></param>
		/// <param name="unreferenceValue"></param>
		/// <returns></returns>
		public static object PropertyGet( this object obj, string name, object[] indexers = null, bool unreferenceValue = true )
		{
			if( indexers == null )
				indexers = Array.Empty<object>();

			object value = null;

			if( indexers.Length != 0 )
			{
				//with indexers

				var types = new Type[ indexers.Length ];
				for( int n = 0; n < indexers.Length; n++ )
				{
					if( indexers[ n ] == null )
						return false;
					types[ n ] = indexers[ n ].GetType();
				}

				//virtual
				var provider = obj as Metadata.IMetadataProvider;
				if( provider != null )
				{
					//public

					Metadata.Property propertyVirtual = null;

					string signature;
					var builder = new StringBuilder();
					builder.Append( "property:" );
					builder.Append( name );
					builder.Append( '[' );
					for( int n = 0; n < types.Length; n++ )
					{
						var type = MetadataManager.GetTypeOfNetType( types[ n ] );
						if( type == null )
							goto skip_virtual;

						if( n != 0 )
							builder.Append( ',' );
						builder.Append( type.Name );
					}
					builder.Append( ']' );
					signature = builder.ToString();

					propertyVirtual = provider.MetadataGetMemberBySignature( signature ) as Metadata.Property;
					skip_virtual:;

					if( propertyVirtual != null )
					{
						try
						{
							value = propertyVirtual.GetValue( propertyVirtual.Static ? null : obj, indexers );
						}
						catch { }
					}
					else
					{
						//non public
						try
						{
							var propertyNative = obj.GetType().GetProperty( name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, null, types, null );
							if( propertyNative != null )
								value = propertyNative.GetValue( propertyNative.GetMethod.IsStatic ? null : obj, indexers );
						}
						catch { }
					}
				}
				else
				{
					//native
					try
					{
						var propertyNative = obj.GetType().GetProperty( name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, null, types, null );
						if( propertyNative != null )
							value = propertyNative.GetValue( propertyNative.GetMethod.IsStatic ? null : obj, indexers );
					}
					catch { }
				}
			}
			else
			{
				//without indexers

				//virtual
				var provider = obj as Metadata.IMetadataProvider;
				if( provider != null )
				{
					//public
					var propertyVirtual = provider.MetadataGetMemberBySignature( $"property:{name}" ) as Metadata.Property;
					if( propertyVirtual != null )
					{
						try
						{
							value = propertyVirtual.GetValue( propertyVirtual.Static ? null : obj, indexers );
						}
						catch { }
					}
					else
					{
						//non public
						try
						{
							var propertyNative = obj.GetType().GetProperty( name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
							if( propertyNative != null )
								value = propertyNative.GetValue( propertyNative.GetMethod.IsStatic ? null : obj, indexers );
						}
						catch { }
					}
				}
				else
				{
					//native
					try
					{
						var propertyNative = obj.GetType().GetProperty( name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
						if( propertyNative != null )
							value = propertyNative.GetValue( propertyNative.GetMethod.IsStatic ? null : obj, indexers );
					}
					catch { }
				}
			}

			if( unreferenceValue )
				value = ReferenceUtility.GetUnreferencedValue( value );

			return value;
		}

		/// <summary>
		/// Gets the value of the object property by name.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="name"></param>
		/// <param name="indexers"></param>
		/// <param name="unreferenceValue"></param>
		/// <returns></returns>
		public static T PropertyGet<T>( this object obj, string name, object[] indexers = null, bool unreferenceValue = true )
		{
			var value = PropertyGet( obj, name, indexers, unreferenceValue );

			var expectedType = typeof( T );

			//auto convert types
			if( value != null && !expectedType.IsAssignableFrom( value.GetType() ) )
			{
				var newValue = MetadataManager.AutoConvertValue( value, expectedType );
				if( newValue == null )
					newValue = MetadataManager.AutoConvertValue( ReferenceUtility.GetUnreferencedValue( value ), expectedType );
				value = newValue;
			}
			//default for value types
			if( value == null && expectedType.IsValueType )
				value = Activator.CreateInstance( expectedType );

			return (T)value;
		}

		/// <summary>
		/// Sets the value of the object property by name.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="indexers"></param>
		/// <returns></returns>
		public static bool PropertySet( this object obj, string name, object value, object[] indexers = null )
		{
			if( indexers == null )
				indexers = Array.Empty<object>();

			Metadata.Property propertyVirtual = null;
			PropertyInfo propertyNative = null;

			if( indexers.Length != 0 )
			{
				//with indexers

				var types = new Type[ indexers.Length ];
				for( int n = 0; n < indexers.Length; n++ )
				{
					if( indexers[ n ] == null )
						return false;
					types[ n ] = indexers[ n ].GetType();
				}

				// virtual
				var provider = obj as Metadata.IMetadataProvider;
				if( provider != null )
				{
					//public

					string signature;
					var builder = new StringBuilder();
					builder.Append( "property:" );
					builder.Append( name );
					builder.Append( '[' );
					for( int n = 0; n < types.Length; n++ )
					{
						var type = MetadataManager.GetTypeOfNetType( types[ n ] );
						if( type == null )
							goto skip_virtual;

						if( n != 0 )
							builder.Append( ',' );
						builder.Append( type.Name );
					}
					builder.Append( ']' );
					signature = builder.ToString();

					propertyVirtual = provider.MetadataGetMemberBySignature( signature ) as Metadata.Property;

					skip_virtual:;

					if( propertyVirtual == null )
					{
						//non public
						try
						{
							propertyNative = obj.GetType().GetProperty( name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, null, types, null );
						}
						catch { }
					}
				}
				else
				{
					//native
					try
					{
						propertyNative = obj.GetType().GetProperty( name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, null, types, null );
					}
					catch { }
				}

			}
			else
			{
				//without indexers

				// virtual
				var provider = obj as Metadata.IMetadataProvider;
				if( provider != null )
				{
					//public
					propertyVirtual = provider.MetadataGetMemberBySignature( $"property:{name}" ) as Metadata.Property;
					if( propertyVirtual == null )
					{
						//non public
						try
						{
							propertyNative = obj.GetType().GetProperty( name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
						}
						catch { }
					}
				}
				else
				{
					//native
					try
					{
						propertyNative = obj.GetType().GetProperty( name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
					}
					catch { }
				}
			}

			if( propertyVirtual == null && propertyNative == null )
				return false;

			//!!!!Metadata.TypeInfo?
			Type expectedType = propertyVirtual != null ? propertyVirtual.Type.GetNetType() : propertyNative.PropertyType;
			var expectedTypeUnref = ReferenceUtility.GetUnreferencedType( expectedType );

			//value type is null
			if( value == null && expectedTypeUnref.IsValueType )
				return false;

			//convert
			if( value != null && !expectedType.IsAssignableFrom( value.GetType() ) )
			{
				if( !ReferenceUtility.IsReferenceType( value.GetType() ) )
				{
					//value is not Reference type

					//try to convert
					if( !expectedTypeUnref.IsAssignableFrom( value.GetType() ) )
					{
						var newValue = MetadataManager.AutoConvertValue( value, expectedTypeUnref );
						if( newValue == null )
							return false;
						value = newValue;
					}
				}
				else
				{
					//value is Reference type

					//specified Reference type must have equal type
					return false;
				}
			}

			//wrap in Reference type
			if( ReferenceUtility.IsReferenceType( expectedType ) )
			{
				if( value != null )
				{
					if( !ReferenceUtility.IsReferenceType( value.GetType() ) )
						value = ReferenceUtility.MakeReference( expectedTypeUnref, value, "" );
				}
				else
					value = ReferenceUtility.MakeReference( expectedTypeUnref, null, "" );
			}

			//set value
			try
			{
				if( propertyVirtual != null )
					propertyVirtual.SetValue( propertyVirtual.Static ? null : obj, value, indexers );
				else
					propertyNative.SetValue( propertyNative.GetMethod.IsStatic ? null : obj, value, indexers );
			}
			catch
			{
				return false;
			}

			return true;
		}
	}
}
