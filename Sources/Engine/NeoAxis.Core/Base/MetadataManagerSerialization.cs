// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	public static partial class MetadataManager
	{
		/// <summary>
		/// Provides the functionality of engine serialization.
		/// </summary>
		public static class Serialization
		{
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			/// <summary>
			/// Provides member info for engine serialization.
			/// </summary>
			public abstract class MemberData
			{
				public abstract Metadata.TypeInfo Type
				{
					get;
				}

				public abstract string ParameterName
				{
					get;
				}

				public abstract object GetValue( object obj );

				public abstract void GetDefaultValue( object obj, out bool specified, out object value );

				public abstract void SetValue( object obj, object value, bool fatalWhenReadOnly );
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			/// <summary>
			/// Provides a property data for engine serialization.
			/// </summary>
			public class MetadataPropertyData : MemberData
			{
				Metadata.Property property;

				//

				public MetadataPropertyData( Metadata.Property property )
				{
					this.property = property;
				}

				public Metadata.Property Property
				{
					get { return property; }
				}

				public override Metadata.TypeInfo Type
				{
					get { return property.Type; }
				}

				public override string ParameterName
				{
					get { return property.Name; }
				}

				public override object GetValue( object obj )
				{
					return property.GetValue( obj, null );
				}

				public override void GetDefaultValue( object obj, out bool specified, out object value )
				{
					var type = MetadataGetType( obj );
					type.GetPropertyDefaultValue( property, out specified, out value );
				}

				public override void SetValue( object obj, object value, bool fatalWhenReadOnly )
				{
					if( property.ReadOnly )
					{
						if( fatalWhenReadOnly )
							Log.Fatal( "MetadataManager: Serialization: MetadataPropertyData: SetValue: fatalWhenReadOnly == true." );
						return;
					}

					property.SetValue( obj, value, null );
				}
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			/// <summary>
			/// Provides a element data of container for engine serialization.
			/// </summary>
			public class ContainerElementData : MemberData
			{
				//!!!!public

				public string parameterName;
				public Metadata.TypeInfo type;
				public object value;

				//

				public ContainerElementData( string parameterName, Metadata.TypeInfo type, object value )
				{
					this.parameterName = parameterName;
					this.type = type;
					this.value = value;
				}

				public override Metadata.TypeInfo Type
				{
					get { return type; }
				}

				public override string ParameterName
				{
					get { return parameterName; }
				}

				public override object GetValue( object obj )
				{
					return value;
				}

				public override void GetDefaultValue( object obj, out bool specified, out object value )
				{
					//!!!!
					specified = false;
					value = null;
				}

				public override void SetValue( object obj, object value, bool fatalWhenReadOnly )
				{
					this.value = value;
				}
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			[MethodImpl( (MethodImplOptions)512 )]
			static void RegisterSimpleTypes()
			{
				foreach( SimpleTypes.TypeItem item in SimpleTypes.Types )
				{
					RegisterSerializableType( item.Type, true,
					delegate ( Metadata.LoadContext context, Metadata.TypeInfo type, string parameterName, MemberData memberOptional, TextBlock block, ref bool gotValue, ref object value, ref string error )
					{
						if( block.AttributeExists( parameterName ) )
						{
							gotValue = true;
							try
							{
								value = SimpleTypes.ParseValue( type.GetNetType(), block.GetAttribute( parameterName ) );
							}
							catch( Exception e )
							{
								error = e.Message;
							}
						}
					},
					delegate ( Metadata.SaveContext context, Metadata.TypeInfo type, string parameterName, TextBlock block, object value, bool defaultValueSpecified, object defaultValue, ref string error )
					{
						//string
						if( type.GetNetType() == typeof( string ) && string.IsNullOrEmpty( value as string ) && context.UseDefaultValue && string.IsNullOrEmpty( defaultValue as string ) )
							return;

						if( value != null )
						{
							//!!!!где еще также сделать

							//!!!!можно было бы еще конвертить в похожий тип. например float в double

							bool equal = false;
							if( context.UseDefaultValue && defaultValue != null )//if( defaultValue != null )
							{
								if( !( value is string ) && defaultValue is string )
								{
									equal = value.ToString() == (string)defaultValue;
								}
								else
								{
									if( value.GetType().IsValueType )
										equal = value.Equals( defaultValue );
									else
										equal = Equals( value, defaultValue );
								}
							}

							if( !equal )
								block.SetAttribute( parameterName, value.ToString() );
						}
						error = "";
					},
					delegate ( Metadata.CloneContext context, object sourceObject, object newObject, Metadata.Property property, ref object value )
					{
					} );
				}
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			[MethodImpl( (MethodImplOptions)512 )]
			static void RegisterEnum()
			{
				serializableTypes_Enum = new SerializableTypeItem();
				serializableTypes_Enum.oneStringSerialization = true;
				serializableTypes_Enum.load =
					delegate ( Metadata.LoadContext context, Metadata.TypeInfo type, string parameterName, MemberData memberOptional, TextBlock block, ref bool gotValue, ref object value, ref string error )
				{
					if( block.AttributeExists( parameterName ) )
					{
						gotValue = true;
						try
						{
							value = Enum.Parse( type.GetNetType(), block.GetAttribute( parameterName ) );
						}
						catch( Exception e )
						{
							error = e.Message;
						}
					}
				};
				serializableTypes_Enum.save =
					delegate ( Metadata.SaveContext context, Metadata.TypeInfo type, string parameterName, TextBlock block, object value, bool defaultValueSpecified, object defaultValue, ref string error )
				{
					if( value != null )
					{
						bool equal = false;
						if( context.UseDefaultValue && defaultValue != null )
						{
							if( defaultValue is string )
							{
								equal = value.ToString() == (string)defaultValue;
							}
							else if( defaultValue is int )
							{
								try
								{
									equal = (int)value == (int)defaultValue;
								}
								catch { }
							}
							else
							{
								equal = value.Equals( defaultValue );
								//	equal = Equals( value, defaultValue );
							}
						}

						if( !equal )
							block.SetAttribute( parameterName, value.ToString() );
					}
					error = "";
				};
				serializableTypes_Enum.clone =
					delegate ( Metadata.CloneContext context, object sourceObject, object newObject, Metadata.Property property, ref object value )
				{
				};
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			[MethodImpl( (MethodImplOptions)512 )]
			static void RegisterArray()
			{
				serializableTypes_Array = new SerializableTypeItem();
				serializableTypes_Array.load =
					delegate ( Metadata.LoadContext context, Metadata.TypeInfo type, string parameterName, MemberData memberOptional, TextBlock block, ref bool gotValue, ref object value, ref string error )
				{
					TextBlock b = block.FindChild( parameterName );
					if( b != null )
					{
						//!!!!slowly

						gotValue = true;
						int count = int.Parse( b.GetAttribute( "Length", "0" ) );

						//loading mode for read only properties. in this mode a new array will not created.
						bool noCreateArray = false;
						{
							var propertyData = memberOptional as MetadataPropertyData;
							if( propertyData != null && propertyData.Property.ReadOnly && value != null && ( (Array)value ).Length == count )
								noCreateArray = true;
						}

						//byte
						if( type.GetNetType() == typeof( byte[] ) && count != 0 )
						{
							string str = b.GetAttribute( ".elements", "" );
							if( !string.IsNullOrEmpty( str ) )
							{
								var newValue = Convert.FromBase64String( str );

								if( bool.Parse( b.GetAttribute( ".elementsZip", false.ToString() ) ) )
									newValue = IOUtility.Unzip( newValue );

								if( noCreateArray )
									Buffer.BlockCopy( newValue, 0, (Array)value, 0, count );
								else
									value = newValue;
								return;
							}
						}

						if( !noCreateArray )
							value = Array.CreateInstance( type.GetNetType().GetElementType(), count );

						if( count != 0 )
						{
							//int[]
							if( value is int[] )
							{
								string str = b.GetAttribute( ".elements", "" );
								if( !string.IsNullOrEmpty( str ) )
								{
									if( !StringUtility.StringToIntegers( str, (int[])value ) )
									{
										error = "Invalid format.";
										return;
									}
									return;
								}
							}

							//!!!!slowly

							MethodInfo methodSetValue = value.GetType().GetMethod( "SetValue", new Type[] { typeof( object ), typeof( int ) } );

							//most of simple types
							//!!!!!sbyte
							if( value is sbyte[] ||
								value is short[] || value is ushort[] ||
								/*value is int[] ||*/ value is uint[] ||
								value is long[] || value is ulong[] ||
								value is float[] || value is double[] )
							{
								string str = b.GetAttribute( ".elements", "" );
								if( !string.IsNullOrEmpty( str ) )
								{
									var strings = str.Split( new char[] { ' ' } );
									if( strings.Length != count )
									{
										error = "Invalid format.";
										return;
									}

									var elementNetType = value.GetType().GetElementType();
									var methodParse = elementNetType.GetMethod( "Parse", new Type[] { typeof( string ) } );
									for( int n = 0; n < strings.Length; n++ )
									{
										var s = strings[ n ];
										var v = methodParse.Invoke( null, new object[] { s } );
										methodSetValue.Invoke( value, new object[] { v, n } );
									}

									return;
								}
							}

							//char
							if( value is char[] )
							{
								string str = b.GetAttribute( ".elements", "" );
								if( !string.IsNullOrEmpty( str ) )
								{
									var strings = str.Split( new char[] { ' ' } );
									if( strings.Length != count )
									{
										error = "Invalid format.";
										return;
									}

									for( int n = 0; n < strings.Length; n++ )
									{
										//!!!!!check exception. где еще

										var s = strings[ n ];
										var v = (char)int.Parse( s );
										methodSetValue.Invoke( value, new object[] { v, n } );
									}

									return;
								}
							}

							//bool
							if( value is bool[] )
							{
								string str = b.GetAttribute( ".elements", "" );
								if( !string.IsNullOrEmpty( str ) )
								{
									if( str.Length != count )
									{
										error = "Invalid format.";
										return;
									}

									for( int n = 0; n < str.Length; n++ )
										methodSetValue.Invoke( value, new object[] { str[ n ] == '1', n } );

									return;
								}
							}

							//default implementation
							{
								var elementType = GetTypeOfNetType( value.GetType().GetElementType() );

								//check childs of TextBlock
								foreach( TextBlock elementBlock in b.Children )
								{
									int index;
									if( int.TryParse( elementBlock.Name, out index ) )
									{
										ContainerElementData member = new ContainerElementData( elementBlock.Name, elementType, null );

										if( !Serialization_LoadMemberRecursive( context, ref value, member, b, out var error2 ) )
										{
											error = error2;
											return;
										}

										//set
										methodSetValue.Invoke( value, new object[] { member.value, index } );
									}
								}

								//check attributes of TextBlock
								foreach( TextBlock.Attribute elementAttr in b.Attributes )
								{
									int index;
									if( int.TryParse( elementAttr.Name, out index ) )
									{
										ContainerElementData member = new ContainerElementData( elementAttr.Name, elementType, null );

										if( !Serialization_LoadMemberRecursive( context, ref value, member, b, out var error2 ) )
										{
											error = error2;
											return;
										}

										//set
										methodSetValue.Invoke( value, new object[] { member.value, index } );
									}
								}

							}
						}
					}
				};

				serializableTypes_Array.save =
					delegate ( Metadata.SaveContext context, Metadata.TypeInfo type, string parameterName, TextBlock block, object value, bool defaultValueSpecified, object defaultValue, ref string error )
				{
					error = "";

					if( value != null )
					{
						TextBlock b = block.AddChild( parameterName );

						//!!!!slowly

						PropertyInfo propertyCount = value.GetType().GetProperty( "Length" );
						MethodInfo methodGetValue = value.GetType().GetMethod( "GetValue", new Type[] { typeof( int ) } );

						int count = (int)propertyCount.GetValue( value, null );
						b.SetAttribute( "Length", count.ToString() );

						if( count != 0 )
						{
							//byte
							if( value is byte[] )
							{
								var value2 = (byte[])value;

								var zipped = IOUtility.Zip( value2, CompressionLevel.Fastest );
								if( zipped.Length < (int)( (float)value2.Length / 1.25 ) )
								{
									string str = Convert.ToBase64String( zipped, Base64FormattingOptions.None );
									b.SetAttribute( ".elements", str );
									b.SetAttribute( ".elementsZip", true.ToString() );
								}
								else
								{
									string str = Convert.ToBase64String( value2, Base64FormattingOptions.None );
									b.SetAttribute( ".elements", str );
								}

								return;
							}

							//!!!!int[] можно тоже зиповать как byte[]. на небольших значениях можно конвертить в ushort[], short[]

							//толку нет, почти такой же размер
							////special format for big int[]
							//if( value is int[] )
							//{
							//	var array = (int[])value;
							//	if( array.Length > 1000 )
							//	{
							//		//Base64_Int32 format
							//		b.SetAttribute( ".format", "Base64_Int32" );
							//		byte[] array2 = new byte[ array.Length * 4 ];
							//		Buffer.BlockCopy( array, 0, array2, 0, array2.Length );
							//		string str = Convert.ToBase64String( array2, Base64FormattingOptions.None );
							//		b.SetAttribute( ".elements", str );

							//		return;
							//	}
							//}

							//most of simple types
							if( value is sbyte[] ||
								value is short[] || value is ushort[] ||
								value is int[] || value is uint[] ||
								value is long[] || value is ulong[] ||
								value is float[] || value is double[] )
							{
								var builder = new StringBuilder( count * 3 );
								for( int n = 0; n < count; n++ )
								{
									var v = methodGetValue.Invoke( value, new object[] { n } );
									if( builder.Length != 0 )
										builder.Append( ' ' );
									builder.Append( v.ToString() );
								}
								b.SetAttribute( ".elements", builder.ToString() );

								return;
							}

							//char
							if( value is char[] )
							{
								var builder = new StringBuilder( count * 3 );
								for( int n = 0; n < count; n++ )
								{
									int v = (char)methodGetValue.Invoke( value, new object[] { n } );
									if( builder.Length != 0 )
										builder.Append( ' ' );
									builder.Append( v.ToString() );
								}
								b.SetAttribute( ".elements", builder.ToString() );

								return;
							}

							//bool
							if( value is bool[] )
							{
								var builder = new StringBuilder( count );
								for( int n = 0; n < count; n++ )
								{
									var v = (bool)methodGetValue.Invoke( value, new object[] { n } );
									builder.Append( v ? '1' : '0' );
								}
								b.SetAttribute( ".elements", builder.ToString() );

								return;
							}

							//default implementation
							for( int n = 0; n < count; n++ )
							{
								var elementType = GetTypeOfNetType( value.GetType().GetElementType() );
								object elementValue = methodGetValue.Invoke( value, new object[] { n } );

								if( elementValue != null )
								{
									var member = new ContainerElementData( n.ToString(), elementType, elementValue );
									//!!!!value странно
									if( !Serialization_SaveMemberRecursive( context, ref value, member, b, out var error2 ) )
									{
										error = error2;
										return;
									}
								}
							}
						}
					}
				};

				serializableTypes_Array.clone =
					delegate ( Metadata.CloneContext context, object sourceObject, object newObject, Metadata.Property property, ref object value )
					{
						//!!!!!!не только одноуровневые поддерживать
						if( value.GetType().GetArrayRank() != 1 )
							Log.Fatal( "Only one rank arrays supports cloning. value.GetType().GetArrayRank() != 1" );

						int length = ( (Array)value ).Length;
						object newValue = Array.CreateInstance( value.GetType().GetElementType(), length );

						if( length != 0 )
						{
							//!!!!!check

							if( value is sbyte[] || value is byte[] ||
								value is short[] || value is ushort[] ||
								value is int[] || value is uint[] ||
								value is long[] || value is ulong[] ||
								value is float[] || value is double[] ||
								value is char[] ||
								value is bool[] ||
								value is string[]
								)
							{
								Array.Copy( (Array)value, (Array)newValue, length );
								return;
							}

							//default implementation
							{
								MethodInfo getValueMethod = value.GetType().GetMethod( "GetValue", new Type[] { typeof( int ) } );
								MethodInfo setValueMethod = newValue.GetType().GetMethod( "SetValue", new Type[] { typeof( object ), typeof( int ) } );

								for( int n = 0; n < length; n++ )
								{
									object v = getValueMethod.Invoke( value, new object[] { n } );
									CloneObject( context, sourceObject, newObject, property, ref v );
									setValueMethod.Invoke( newValue, new object[] { v, n } );
								}
							}
						}

						value = newValue;
					};
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			static Type GetGenericArgumentInBaseTypes( Type type, Type findType, int index )
			{
				var t = type;
				do
				{
					if( t.IsGenericType && t.GetGenericTypeDefinition() == findType )
						return t.GetGenericArguments()[ index ];

					t = t.BaseType;
				} while( t != null );

				return null;
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			[MethodImpl( (MethodImplOptions)512 )]
			public static void RegisterListSet( Type containerType )
			{
				RegisterSerializableType( containerType, false,
				delegate ( Metadata.LoadContext context, Metadata.TypeInfo type, string parameterName, MemberData memberOptional, TextBlock block, ref bool gotValue, ref object value, ref string error )
				{
					TextBlock b = block.FindChild( parameterName );
					if( b != null )
					{
						gotValue = true;

						if( value == null )
						{
							//create new list
							//value = type.InvokeInstance( null );
							value = type.GetNetType().InvokeMember( "",
								BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
								null, null, null );
						}
						else
						{
							//clear
							type.GetNetType().GetMethod( "Clear", new Type[ 0 ] ).Invoke( value, new object[ 0 ] );
						}

						//!!!!!число по сути только списку
						int count = int.Parse( b.GetAttribute( "Count", "0" ) );
						if( count != 0 )
						{
							var referenceList = value as IReferenceList;

							Type elementNetType;
							if( referenceList != null )
								elementNetType = referenceList.GetItemType();
							else
								elementNetType = GetGenericArgumentInBaseTypes( value.GetType(), containerType, 0 );
							var elementType = GetTypeOfNetType( elementNetType );

							string addName;
							if( containerType == typeof( Stack<> ) )
								addName = "Push";
							else if( containerType == typeof( Queue<> ) )
								addName = "Enqueue";
							else
								addName = "Add";

							////!!!!new. ReferenceList fix
							//MethodInfo methodAdd = value.GetType().GetMethod( addName );
							MethodInfo methodAdd = value.GetType().GetMethod( addName, new Type[] { elementNetType } );

							//most of simple types
							if( elementNetType == typeof( sbyte ) || elementNetType == typeof( byte ) ||
								elementNetType == typeof( short ) || elementNetType == typeof( ushort ) ||
								elementNetType == typeof( int ) || elementNetType == typeof( uint ) ||
								elementNetType == typeof( long ) || elementNetType == typeof( ulong ) ||
								elementNetType == typeof( float ) || elementNetType == typeof( double ) )
							{
								string str = b.GetAttribute( ".elements", "" );
								if( !string.IsNullOrEmpty( str ) )
								{
									var strings = str.Split( new char[] { ' ' } );
									if( strings.Length != count )
									{
										error = "Invalid format.";
										return;
									}

									var methodParse = elementNetType.GetMethod( "Parse", new Type[] { typeof( string ) } );
									foreach( var s in strings )
									{
										var v = methodParse.Invoke( null, new object[] { s } );
										methodAdd.Invoke( value, new object[] { v } );
									}

									return;
								}
							}

							//char
							if( elementNetType == typeof( char ) )
							{
								string str = b.GetAttribute( ".elements", "" );
								if( !string.IsNullOrEmpty( str ) )
								{
									var strings = str.Split( new char[] { ' ' } );
									if( strings.Length != count )
									{
										error = "Invalid format.";
										return;
									}

									foreach( var s in strings )
									{
										//!!!!!check exception. где еще

										methodAdd.Invoke( value, new object[] { (char)int.Parse( s ) } );
									}

									return;
								}
							}

							//bool
							if( elementNetType == typeof( bool ) )
							{
								string str = b.GetAttribute( ".elements", "" );
								if( !string.IsNullOrEmpty( str ) )
								{
									if( str.Length != count )
									{
										error = "Invalid format.";
										return;
									}

									foreach( char c in str )
										methodAdd.Invoke( value, new object[] { c == '1' } );

									return;
								}
							}

							//default implementation
							{
								//List<> only set Count
								if( containerType == typeof( List<> ) || containerType == typeof( ReferenceList<> ) )
								{
									object defaultValue = null;
									if( referenceList != null )
										defaultValue = Activator.CreateInstance( referenceList.GetItemType() );
									if( elementNetType.IsValueType )
										defaultValue = Activator.CreateInstance( elementNetType );
									for( int n = 0; n < count; n++ )
										methodAdd.Invoke( value, new object[] { defaultValue } );
								}

								PropertyInfo propertyItem = null;

								if( containerType == typeof( List<> ) || containerType == typeof( ReferenceList<> ) )
									propertyItem = value.GetType().GetProperty( "Item" );

								//check childs of TextBlock
								foreach( TextBlock elementBlock in b.Children )
								{
									int index;
									if( int.TryParse( elementBlock.Name, out index ) )
									{
										ContainerElementData member = new ContainerElementData( elementBlock.Name, elementType, null );

										if( !Serialization_LoadMemberRecursive( context, ref value, member, b, out var error2 ) )
										{
											error = error2;
											return;
										}

										//set
										if( containerType == typeof( List<> ) || containerType == typeof( ReferenceList<> ) )
											propertyItem.SetValue( value, member.value, new object[] { index } );
										else
											methodAdd.Invoke( value, new object[] { member.value } );
									}
								}

								//check attributes of TextBlock
								foreach( TextBlock.Attribute elementAttr in b.Attributes )
								{
									int index;
									if( int.TryParse( elementAttr.Name, out index ) )
									{
										ContainerElementData member = new ContainerElementData( elementAttr.Name, elementType, null );

										if( !Serialization_LoadMemberRecursive( context, ref value, member, b, out var error2 ) )
										{
											error = error2;
											return;
										}

										//set
										if( containerType == typeof( List<> ) || containerType == typeof( ReferenceList<> ) )
											propertyItem.SetValue( value, member.value, new object[] { index } );
										else
											methodAdd.Invoke( value, new object[] { member.value } );
									}
								}

							}
						}
					}
				},
				delegate ( Metadata.SaveContext context, Metadata.TypeInfo type, string parameterName, TextBlock block, object value, bool defaultValueSpecified, object defaultValue, ref string error )
				{
					error = "";

					if( value != null )
					{
						TextBlock b = block.AddChild( parameterName );

						int count = (int)value.GetType().GetProperty( "Count" ).GetValue( value, null );
						b.SetAttribute( "Count", count.ToString() );

						//!!!!!можно делить на части если большие. везде так
						//!!!!!еще можно сохранять в более бинарном виде

						var referenceList = value as IReferenceList;

						Type elementNetType;
						if( referenceList != null )
							elementNetType = referenceList.GetItemType();
						else
							elementNetType = GetGenericArgumentInBaseTypes( value.GetType(), containerType, 0 );
						var elementType = GetTypeOfNetType( elementNetType );

						object valueEnumerate = value;
						//reverse order for Stack<>
						if( containerType == typeof( Stack<> ) )
						{
							valueEnumerate = value.GetType().GetMethod( "ToArray", new Type[ 0 ] ).Invoke( value, new object[ 0 ] );
							Array.Reverse( (Array)valueEnumerate );
						}

						//most of simple types
						if( elementNetType == typeof( sbyte ) || elementNetType == typeof( byte ) ||
							elementNetType == typeof( short ) || elementNetType == typeof( ushort ) ||
							elementNetType == typeof( int ) || elementNetType == typeof( uint ) ||
							elementNetType == typeof( long ) || elementNetType == typeof( ulong ) ||
							elementNetType == typeof( float ) || elementNetType == typeof( double ) ||
							elementNetType == typeof( char ) )
						{
							var builder = new StringBuilder( count * 3 );
							foreach( object v in (IEnumerable)valueEnumerate )
							{
								if( builder.Length != 0 )
									builder.Append( ' ' );
								builder.Append( v.ToString() );
							}
							b.SetAttribute( ".elements", builder.ToString() );

							return;
						}

						//bool
						if( elementNetType == typeof( bool ) )
						{
							var builder = new StringBuilder( count );
							foreach( bool v in (IEnumerable)valueEnumerate )
								builder.Append( v ? '1' : '0' );
							b.SetAttribute( ".elements", builder.ToString() );

							return;
						}

						//!!!!new
						//Component specific. disable saving Value property
						if( referenceList != null && typeof( Component ).IsAssignableFrom( referenceList.GetItemValueType() ) )
						//if( containerType == typeof( ReferenceList<> ) && typeof( Component ).IsAssignableFrom( elementNetType ) )
						{
							int n = 0;
							foreach( object elementValue in (IEnumerable)valueEnumerate )
							{
								var reference = (IReference)elementValue;
								if( reference.ReferenceSpecified )
								{
									var typeName = MetadataGetType( elementValue ).Name;
									var itemBlock = b.AddChild( n.ToString(), typeName );
									itemBlock.SetAttribute( "GetByReference", reference.GetByReference );
								}
								n++;
							}
							return;
						}

						//default implementation
						{
							int n = 0;
							foreach( object elementValue in (IEnumerable)valueEnumerate )
							{
								if( elementValue != null )
								{
									ContainerElementData member = new ContainerElementData( n.ToString(), elementType, elementValue );
									//!!!!value странно
									if( !Serialization_SaveMemberRecursive( context, ref valueEnumerate, member, b, out var error2 ) )
									{
										error = error2;
										return;
									}
								}

								n++;
							}
						}
					}
				},
				delegate ( Metadata.CloneContext context, object sourceObject, object newObject, Metadata.Property property, ref object value )
				{
					var referenceList = value as IReferenceList;

					object newValue = null;
					//!!!!new
					if( referenceList != null )
						newValue = property.GetValue( newObject, null );
					else
					{
						newValue = value.GetType().InvokeMember( "", BindingFlags.Public |
							BindingFlags.NonPublic | BindingFlags.CreateInstance |
							BindingFlags.Instance, null, null, null );
					}

					int count = (int)value.GetType().GetProperty( "Count" ).GetValue( value, null );
					if( count != 0 )
					{
						Type elementNetType;
						if( referenceList != null )
							elementNetType = referenceList.GetItemType();
						else
							elementNetType = GetGenericArgumentInBaseTypes( value.GetType(), containerType, 0 );

						string addName;
						if( containerType == typeof( Stack<> ) )
							addName = "Push";
						else if( containerType == typeof( Queue<> ) )
							addName = "Enqueue";
						else
							addName = "Add";

						////!!!!new. ReferenceList fix
						//MethodInfo methodAdd = newValue.GetType().GetMethod( addName );
						MethodInfo methodAdd = newValue.GetType().GetMethod( addName, new Type[] { elementNetType } );

						//!!!!!check

						foreach( object v2 in (IEnumerable)value )
						{
							object v = v2;
							CloneObject( context, sourceObject, newObject, property, ref v );
							methodAdd.Invoke( newValue, new object[] { v } );
						}
					}

					value = newValue;
				} );
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			[MethodImpl( (MethodImplOptions)512 )]
			public static void RegisterDictionary( Type containerType )
			{
				RegisterSerializableType( containerType, false,
				delegate ( Metadata.LoadContext context, Metadata.TypeInfo type, string parameterName, MemberData memberOptional, TextBlock block, ref bool gotValue, ref object value, ref string error )
				{
					TextBlock b = block.FindChild( parameterName );
					if( b != null )
					{
						gotValue = true;

						if( value == null )
						{
							//create new list
							//value = type.InvokeInstance( null );
							value = type.GetNetType().InvokeMember( "",
								BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
								null, null, null );
						}
						else
						{
							//clear
							type.GetNetType().GetMethod( "Clear", new Type[ 0 ] ).Invoke( value, new object[ 0 ] );
						}

						var keyNetType = GetGenericArgumentInBaseTypes( value.GetType(), containerType, 0 );
						var valueNetType = GetGenericArgumentInBaseTypes( value.GetType(), containerType, 1 );
						var keyType = GetTypeOfNetType( keyNetType );
						var valueType = GetTypeOfNetType( valueNetType );

						MethodInfo methodAdd = value.GetType().GetMethod( "Add", new Type[] { keyNetType, valueNetType } );

						foreach( var elementBlock in b.Children )
						{
							if( elementBlock.Name != ".element" )
								continue;

							object elementKey = null;
							object elementValue = null;

							//Key
							{
								ContainerElementData member = new ContainerElementData( "Key", keyType, null );

								if( !Serialization_LoadMemberRecursive( context, ref value, member, elementBlock, out var error2 ) )
								{
									error = error2;
									return;
								}

								elementKey = member.value;
							}

							//Value
							{
								ContainerElementData member = new ContainerElementData( "Value", valueType, null );

								if( !Serialization_LoadMemberRecursive( context, ref value, member, elementBlock, out var error2 ) )
								{
									error = error2;
									return;
								}

								elementValue = member.value;
							}

							//add
							methodAdd.Invoke( value, new object[] { elementKey, elementValue } );
						}
					}
				},
				delegate ( Metadata.SaveContext context, Metadata.TypeInfo type, string parameterName, TextBlock block, object value, bool defaultValueSpecified, object defaultValue, ref string error )
				{
					error = "";

					if( value != null )
					{
						TextBlock b = block.AddChild( parameterName );

						PropertyInfo propertyKey = null;
						PropertyInfo propertyValue = null;

						foreach( object pair2 in (IEnumerable)value )
						{
							object pair = pair2;

							if( propertyKey == null )
							{
								propertyKey = pair.GetType().GetProperty( "Key" );
								propertyValue = pair.GetType().GetProperty( "Value" );
							}
							var elementKey = propertyKey.GetValue( pair, new object[ 0 ] );
							var elementValue = propertyValue.GetValue( pair, new object[ 0 ] );

							//save data

							TextBlock elementBlock = b.AddChild( ".element" );

							//!!!!!slowly?

							{
								ContainerElementData member = new ContainerElementData( "Key", GetTypeOfNetType( elementKey.GetType() ), elementKey );
								//!!!!value странно. не юзается вроде как
								if( !Serialization_SaveMemberRecursive( context, ref pair, member, elementBlock, out var error2 ) )
								{
									error = error2;
									return;
								}
							}

							if( elementValue != null )
							{
								ContainerElementData member = new ContainerElementData( "Value", GetTypeOfNetType( elementValue.GetType() ), elementValue );
								//!!!!value странно. не юзается вроде как
								if( !Serialization_SaveMemberRecursive( context, ref pair, member, elementBlock, out var error2 ) )
								{
									error = error2;
									return;
								}
							}
						}
					}
				},
				delegate ( Metadata.CloneContext context, object sourceObject, object newObject, Metadata.Property property, ref object value )
				{
					//!!!!slowly

					var newValue = value.GetType().InvokeMember( "", BindingFlags.Public |
						BindingFlags.NonPublic | BindingFlags.CreateInstance |
						BindingFlags.Instance, null, null, null );

					int count = (int)value.GetType().GetProperty( "Count" ).GetValue( value, null );
					if( count != 0 )
					{
						//!!!!!check

						//!!!!new Type[]{}
						var methodAdd = newValue.GetType().GetMethod( "Add" );

						PropertyInfo propertyKey = null;
						PropertyInfo propertyValue = null;

						foreach( object pair in (IEnumerable)value )
						{
							if( propertyKey == null )
							{
								propertyKey = pair.GetType().GetProperty( "Key" );
								propertyValue = pair.GetType().GetProperty( "Value" );
							}
							var elementKey = propertyKey.GetValue( pair, new object[ 0 ] );
							var elementValue = propertyValue.GetValue( pair, new object[ 0 ] );

							object clonedKey = elementKey;
							CloneObject( context, sourceObject, newObject, property, ref clonedKey );

							object clonedValue = elementValue;
							if( clonedValue != null )
								CloneObject( context, sourceObject, newObject, property, ref clonedValue );

							//add
							methodAdd.Invoke( newValue, new object[] { clonedKey, clonedValue } );
						}
					}

					value = newValue;
				} );
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			//public static void RegisterReferenceList( Type containerType )
			//{
			//	RegisterSerializableType( containerType, false,
			//	delegate ( Metadata.LoadContext context, Metadata.TypeInfo type, string parameterName, MemberData memberOptional, TextBlock block, ref bool gotValue, ref object value, ref string error )
			//	{
			//		TextBlock b = block.FindChild( parameterName );
			//		if( b != null )
			//		{
			//			gotValue = true;

			//			if( value == null )
			//			{
			//				//create new list
			//				//value = type.InvokeInstance( null );
			//				value = type.GetNetType().InvokeMember( "",
			//					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
			//					null, null, null );
			//			}
			//			else
			//			{
			//				//clear
			//				type.GetNetType().GetMethod( "Clear", new Type[ 0 ] ).Invoke( value, new object[ 0 ] );
			//			}

			//			//!!!!!число по сути только списку
			//			int count = int.Parse( b.GetAttribute( "Count", "0" ) );
			//			if( count != 0 )
			//			{
			//				var elementNetType = GetGenericArgumentInBaseTypes( value.GetType(), containerType, 0 );
			//				var elementType = GetTypeOfNetType( elementNetType );

			//				string addName;
			//				//if( containerType == typeof( Stack<> ) )
			//				//	addName = "Push";
			//				//else if( containerType == typeof( Queue<> ) )
			//				//	addName = "Enqueue";
			//				//else
			//				addName = "Add";

			//				//!!!!new. ReferenceList fix
			//				MethodInfo methodAdd = value.GetType().GetMethod( addName );
			//				//MethodInfo methodAdd = value.GetType().GetMethod( addName, new Type[] { elementNetType } );

			//				//default implementation
			//				{
			//					//List<> only set Count
			//					if( containerType == typeof( List<> ) || containerType == typeof( ReferenceList<> ) )
			//					{
			//						object defaultValue = null;
			//						if( elementNetType.IsValueType )
			//							defaultValue = Activator.CreateInstance( elementNetType );
			//						for( int n = 0; n < count; n++ )
			//							methodAdd.Invoke( value, new object[] { defaultValue } );
			//					}

			//					PropertyInfo propertyItem = null;

			//					if( containerType == typeof( List<> ) || containerType == typeof( ReferenceList<> ) )
			//						propertyItem = value.GetType().GetProperty( "Item" );

			//					//check childs of TextBlock
			//					foreach( TextBlock elementBlock in b.Children )
			//					{
			//						int index;
			//						if( int.TryParse( elementBlock.Name, out index ) )
			//						{
			//							ContainerElementData member = new ContainerElementData( elementBlock.Name, elementType, null );

			//							string error2 = "";
			//							if( !Serialization_LoadMemberRecursive( context, ref value, member, b, out error2 ) )
			//							{
			//								//!!!!!
			//								Log.Fatal( "impl" );
			//							}

			//							//set
			//							if( containerType == typeof( List<> ) || containerType == typeof( ReferenceList<> ) )
			//								propertyItem.SetValue( value, member.value, new object[] { index } );
			//							else
			//								methodAdd.Invoke( value, new object[] { member.value } );
			//						}
			//					}

			//					//check attributes of TextBlock
			//					foreach( TextBlock.Attribute elementAttr in b.Attributes )
			//					{
			//						int index;
			//						if( int.TryParse( elementAttr.Name, out index ) )
			//						{
			//							ContainerElementData member = new ContainerElementData( elementAttr.Name, elementType, null );

			//							string error2 = "";
			//							if( !Serialization_LoadMemberRecursive( context, ref value, member, b, out error2 ) )
			//							{
			//								//!!!!!
			//								Log.Fatal( "impl" );
			//							}

			//							//set
			//							if( containerType == typeof( List<> ) || containerType == typeof( ReferenceList<> ) )
			//								propertyItem.SetValue( value, member.value, new object[] { index } );
			//							else
			//								methodAdd.Invoke( value, new object[] { member.value } );
			//						}
			//					}

			//				}
			//			}
			//		}
			//	},
			//	delegate ( Metadata.SaveContext context, Metadata.TypeInfo type, string parameterName, TextBlock block, object value, bool defaultValueSpecified, object defaultValue, ref string error )
			//	{
			//		error = "";

			//		if( value != null )
			//		{
			//			TextBlock b = block.AddChild( parameterName );

			//			int count = (int)value.GetType().GetProperty( "Count" ).GetValue( value, null );
			//			b.SetAttribute( "Count", count.ToString() );

			//			//!!!!!можно делить на части если большие. везде так
			//			//!!!!!еще можно сохранять в более бинарном виде

			//			var elementNetType = GetGenericArgumentInBaseTypes( value.GetType(), containerType, 0 );
			//			var elementType = GetTypeOfNetType( elementNetType );

			//			object valueEnumerate = value;
			//			////reverse order for Stack<>
			//			//if( containerType == typeof( Stack<> ) )
			//			//{
			//			//	valueEnumerate = value.GetType().GetMethod( "ToArray", new Type[ 0 ] ).Invoke( value, new object[ 0 ] );
			//			//	Array.Reverse( (Array)valueEnumerate );
			//			//}

			//			//!!!!new
			//			//Component specific. disable saving Value property
			//			if( typeof( Component ).IsAssignableFrom( elementNetType ) )
			//			{
			//				int n = 0;
			//				foreach( object elementValue in (IEnumerable)valueEnumerate )
			//				{
			//					var reference = (IReference)elementValue;
			//					if( reference.ReferenceSpecified )
			//					{
			//						//!!!!где еще так?
			//						string typeName = "";
			//						//if( !elementType.GetNetType().IsValueType )
			//						//{
			//						//!!!!
			//						var typeName2 = MetadataGetType( elementValue ).Name;
			//						//if( typeName2 != elementType.Name )
			//						typeName = typeName2;
			//						//}

			//						var itemBlock = b.AddChild( n.ToString(), typeName );
			//						itemBlock.SetAttribute( "GetByReference", reference.GetByReference );
			//					}

			//					n++;
			//				}

			//				return;
			//			}

			//			//default implementation
			//			{
			//				int n = 0;
			//				foreach( object elementValue in (IEnumerable)valueEnumerate )
			//				{
			//					if( elementValue != null )
			//					{
			//						ContainerElementData member = new ContainerElementData( n.ToString(), elementType, elementValue );
			//						string error2 = "";
			//						//!!!!value странно
			//						if( !Serialization_SaveMemberRecursive( context, ref valueEnumerate, member, b, out error2 ) )
			//						{
			//							//!!!!!
			//							Log.Fatal( "impl" );
			//						}
			//					}

			//					n++;
			//				}
			//			}
			//		}
			//	},
			//	delegate ( Metadata.CloneContext context, ref object value )
			//	{
			//		var newValue = value.GetType().InvokeMember( "", BindingFlags.Public |
			//			BindingFlags.NonPublic | BindingFlags.CreateInstance |
			//			BindingFlags.Instance, null, null, null );

			//		int count = (int)value.GetType().GetProperty( "Count" ).GetValue( value, null );
			//		if( count != 0 )
			//		{
			//			var elementNetType = GetGenericArgumentInBaseTypes( value.GetType(), containerType, 0 );

			//			string addName;
			//			//if( containerType == typeof( Stack<> ) )
			//			//	addName = "Push";
			//			//else if( containerType == typeof( Queue<> ) )
			//			//	addName = "Enqueue";
			//			//else
			//			addName = "Add";

			//			//!!!!new. ReferenceList fix
			//			MethodInfo methodAdd = newValue.GetType().GetMethod( addName );
			//			//MethodInfo methodAdd = newValue.GetType().GetMethod( addName, new Type[] { elementNetType } );

			//			//!!!!!check

			//			foreach( object v2 in (IEnumerable)value )
			//			{
			//				object v = v2;
			//				CloneObject( context, ref v );
			//				methodAdd.Invoke( newValue, new object[] { v } );
			//			}
			//		}

			//		value = newValue;
			//	} );
			//}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			[MethodImpl( (MethodImplOptions)512 )]
			static void RegisterReference()
			{
				RegisterSerializableType( typeof( Reference<> ), false,
				delegate ( Metadata.LoadContext context, Metadata.TypeInfo type, string parameterName, MemberData memberOptional, TextBlock block, ref bool gotValue, ref object value, ref string error )
				{
					error = "";

					var netType = type.GetNetType();
					Type valueType = netType.GetGenericArguments()[ 0 ];

					//simplified form
					var valueItem = GetSuitableSerializableType( valueType, out _ );
					if( valueItem != null && valueItem.OneStringSerialization )
					{
						if( block.AttributeExists( parameterName ) )
						{
							gotValue = true;
							value = type.InvokeInstance( null );

							bool gotValue2 = false;
							object v = null;

							valueItem.Load( context, GetTypeOfNetType( valueType ), parameterName, null, block, ref gotValue2, ref v, ref error );
							if( !string.IsNullOrEmpty( error ) )
								return;

							if( gotValue2 )
							{
								var p = netType.GetProperty( "Value" );
								p.SetValue( value, v, null );
							}

							return;
						}
					}

					//default behaviour
					TextBlock b = block.FindChild( parameterName );
					if( b != null )
					{
						gotValue = true;
						value = type.InvokeInstance( null );

						foreach( var member in type.MetadataGetMembers( new Metadata.GetMembersContext( false ) ) )
						{
							var property = member as Metadata.Property;
							if( property != null && !property.ReadOnly && !property.HasIndexers && !property.Static )
							{
								var type2 = property.Serializable;
								if( type2 == SerializeType.Enable || ( type2 == SerializeType.Auto && property.Browsable ) )
								{
									var memberData = new MetadataPropertyData( property );

									if( !Serialization_LoadMemberRecursive( context, ref value, memberData, b, out error ) )
										return;
								}
							}
						}
					}
				},
				delegate ( Metadata.SaveContext context, Metadata.TypeInfo type, string parameterName, TextBlock block, object value, bool defaultValueSpecified, object defaultValue, ref string error )
				{
					error = "";

					IReference reference = (IReference)value;
					if( reference == null )//must never happens
						return;

					//var netType = type.GetNetType();
					//Type valueType = netType.GetGenericArguments()[ 0 ];

					//!!!!slowly
					//var p2 = netType.GetProperty( "GetByReference" );
					//string getByReference = reference.GetByReference;// (string)p2.GetValue( value, null );
					bool referenceIsSpecified = !string.IsNullOrEmpty( reference.GetByReference );

					//check for default value == null
					if( defaultValueSpecified && defaultValue == null )
					{
						if( reference.ValueAsObject == null && !referenceIsSpecified )
							return;
					}

					//default value when reference is specified
					if( referenceIsSpecified && defaultValueSpecified )
					{
						var defaultValueReference = defaultValue as IReference;
						if( defaultValueReference != null && reference.GetByReference == defaultValueReference.GetByReference )
							return;
					}

					var skipSimplifiedForm = false;
					//DefaultValueReference attribute when value is empty but default value is not empty
					if( !referenceIsSpecified && reference.ValueAsObject == null && defaultValue != null )
					{
						var defaultValueReference = defaultValue as IReference;
						if( defaultValueReference != null && !string.IsNullOrEmpty( defaultValueReference.GetByReference ) )
							skipSimplifiedForm = true;
					}

					//simplified form
					if( !referenceIsSpecified && !skipSimplifiedForm )
					{
						var valueItem = GetSuitableSerializableType( reference.UnderlyingType, out _ );
						if( valueItem != null && valueItem.OneStringSerialization )
						{
							object v = reference.ValueAsObject;
							//!!!!slowly
							//var p = netType.GetProperty( "Value" );
							//object v = p.GetValue( value, null );

							if( v != null )
							{
								bool defaultValueSpecified2 = defaultValueSpecified;
								object defaultValue2 = defaultValue;

								valueItem.Save( context, GetTypeOfNetType( reference.UnderlyingType ), parameterName, block, v, defaultValueSpecified2, defaultValue2, ref error );
								if( !string.IsNullOrEmpty( error ) )
									return;
							}

							return;
						}
					}

					//default implementation
					{
						//!!!!для Reference<> default value сделать

						TextBlock memberBlock = block.AddChild( parameterName );

						//members
						foreach( var member in MetadataGetType( value ).MetadataGetMembers( new Metadata.GetMembersContext( false ) ) )
						{
							var property = member as Metadata.Property;
							if( property != null && !property.ReadOnly && !property.HasIndexers && !property.Static )
							{
								var type2 = property.Serializable;
								if( type2 == SerializeType.Enable || ( type2 == SerializeType.Auto && property.Browsable ) )
								{
									if( referenceIsSpecified )
									{
										if( property.Name != "GetByReference" )
											continue;
									}
									else
									{
										if( property.Name != "Value" )
											continue;
									}

									////!!!!new
									//if( typeof( Component ).IsAssignableFrom( reference.UnderlyingType ) && property.Name == "Value" )
									//	continue;

									MetadataPropertyData member2 = new MetadataPropertyData( property );

									if( !Serialization_SaveMemberRecursive( context, ref value, member2, memberBlock, out error ) )
										return;
								}
							}
						}
					}
				},
				delegate ( Metadata.CloneContext context, object sourceObject, object newObject, Metadata.Property property, ref object value )
				{
					var netType = value.GetType();
					Type valueType = netType.GetGenericArguments()[ 0 ];

					//!!!!slowly
					var newValue = netType.InvokeMember( "", BindingFlags.Public |
						BindingFlags.NonPublic | BindingFlags.CreateInstance |
						BindingFlags.Instance, null, null, null );

					//!!!!slowly
					var propertyGetByReference = netType.GetProperty( "GetByReference" );
					string getByReference = (string)propertyGetByReference.GetValue( value, null );

					bool referenceIsSpecified = !string.IsNullOrEmpty( getByReference );
					if( referenceIsSpecified )
					{
						//reference is specified
						propertyGetByReference.SetValue( newValue, getByReference, null );
					}
					else
					{
						//reference is not specified

						//!!!!slowly
						var propertyValue = netType.GetProperty( "Value" );
						object v = propertyValue.GetValue( value, null );
						CloneObject( context, sourceObject, newObject, property, ref v );
						propertyValue.SetValue( newValue, v, null );
					}

					value = newValue;

				} );
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			[MethodImpl( (MethodImplOptions)512 )]
			static void RegisterComponent()
			{
				//!!!!!

				RegisterSerializableType( typeof( Component ), true,
				delegate ( Metadata.LoadContext context, Metadata.TypeInfo type, string parameterName, MemberData memberOptional, TextBlock block, ref bool gotValue, ref object value, ref string error )
				{
					//if( block.AttributeExists( parameterName ) )
					//{
					//	//!!!!
					//	Log.Fatal( "impl" );

					//	//gotValue = true;
					//	//value = UIControl.ScaleValue.Parse( block.GetAttribute( parameterName ) );
					//	//!!!!error?
					//}
				},
				delegate ( Metadata.SaveContext context, Metadata.TypeInfo type, string parameterName, TextBlock block, object value, bool defaultValueSpecified, object defaultValue, ref string error )
				{
					//!!!!

					//if( value != null )
					////!!!!!так?
					////if( !Equals( value, SimpleTypesUtils.GetDefaultValue( parameterType ) ) )
					//{
					//	//!!!!
					//	//Log.Fatal( "impl" );

					//	//block.SetAttribute( parameterName, value.ToString() );
					//}
					error = "";
				},
				delegate ( Metadata.CloneContext context, object sourceObject, object newObject, Metadata.Property property, ref object value )
				{
					//replace by new cloned object
					Component c = value as Component;
					if( c != null )
					{
						Component newC;
						if( context.NewComponentsRedirection.TryGetValue( c, out newC ) )
							value = newC;
					}
				} );
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			//static void RegisterUIControlScaleValue()
			//{
			//	RegisterSerializableType( typeof( UIControl.ScaleValue ), true,
			//	delegate ( Metadata.LoadContext context, Metadata.TypeInfo type, string parameterName, MemberData memberOptional, TextBlock block, ref bool gotValue, ref object value, ref string error )
			//	{
			//		if( block.AttributeExists( parameterName ) )
			//		{
			//			gotValue = true;
			//			value = UIControl.ScaleValue.Parse( block.GetAttribute( parameterName ) );
			//			//!!!!error?
			//		}
			//	},
			//	delegate ( Metadata.SaveContext context, Metadata.TypeInfo type, string parameterName, TextBlock block, object value, bool defaultValueSpecified, object defaultValue, ref string error )
			//	{
			//		//!!!!!так?
			//		//if( !Equals( value, SimpleTypesUtils.GetDefaultValue( parameterType ) ) )
			//		{
			//			block.SetAttribute( parameterName, value.ToString() );
			//		}
			//		error = "";
			//	},
			//	delegate ( Metadata.CloneContext context, ref object value )
			//	{
			//	} );
			//}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			internal static void RegisterStandardSerializableTypes()
			{
				RegisterSimpleTypes();
				RegisterEnum();
				RegisterArray();

				RegisterListSet( typeof( List<> ) );
				RegisterListSet( typeof( ESet<> ) );
				RegisterListSet( typeof( HashSet<> ) );
				RegisterListSet( typeof( SortedSet<> ) );
				RegisterListSet( typeof( Stack<> ) );
				RegisterListSet( typeof( Queue<> ) );
				//RegisterReferenceList( typeof( ReferenceList<> ) );
				RegisterListSet( typeof( ReferenceList<> ) );

				RegisterDictionary( typeof( Dictionary<,> ) );
				RegisterDictionary( typeof( EDictionary<,> ) );
				RegisterDictionary( typeof( SortedList<,> ) );

				RegisterReference();
				RegisterComponent();
			}

			[MethodImpl( (MethodImplOptions)512 )]
			static bool Load_LoadMemberForRegisteredType( Metadata.LoadContext context, ref object obj, MemberData member, TextBlock block, out bool fullySerialized, out string error )
			{
				SerializableTypeItem item = GetSuitableSerializableType( member.Type.GetNetType(), out fullySerialized );
				if( item != null )
				{
					bool gotValue = false;

					object value = member.GetValue( obj );

					string error2 = "";
					item.load( context, member.Type, member.ParameterName, member, block, ref gotValue, ref value, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
					{
						error = error2;
						return false;
					}

					if( gotValue )
						member.SetValue( obj, value, false );
				}

				error = "";
				return true;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			static bool Load_LoadMemberForNotRegisteredType( Metadata.LoadContext context, ref object obj, MemberData member, TextBlock block, out string error )
			{
				//!!!!!?
				object currentValue = member.GetValue( obj );

				//create object
				object value = currentValue;

				//if( value != null )//!!!!!!так? а если default value не null?
				TextBlock memberBlock = block.FindChild( member.ParameterName );
				if( memberBlock != null )
				{
					string typeName = memberBlock.Data;

					if( !string.IsNullOrEmpty( typeName ) )
					{
						Metadata.TypeInfo type = MetadataManager.GetType( typeName );
						if( type == null )
						{
							error = $"Type with name \'{typeName}\' is not exists.";
							return false;
						}

						//!!!!!возможно нужно сделать, чтобы можно было юзать текуший объект. грузить только параметры.
						//!!!!!обрабатывать ошибки?
						value = type.InvokeInstance( null );

						//!!!!!new, раньше было внутри
					}
					else
					{
						if( value == null )
						{
							try
							{
								value = member.Type.InvokeInstance( null );
							}
							catch { }
						}
					}

					//!!!!!new, раньше было внутри
					if( value != null )
					{
						if( !LoadSerializableMembers( context, value, memberBlock, out error ) )
							return false;
					}
				}

				//set value
				bool skip = member.Type.GetNetType().IsValueType && value == null;
				//bool skip = ( member.Type.GetNetType().IsValueType && value == null ) || ( currentValue == value );
				if( !skip )
					member.SetValue( obj, value, false );

				error = "";
				return true;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public static bool Serialization_LoadMemberRecursive( Metadata.LoadContext context, ref object obj, MemberData member, TextBlock block, out string error )
			{
				//"ref obj" - for structures

				bool fullySerialized;
				if( !Load_LoadMemberForRegisteredType( context, ref obj, member, block, out fullySerialized, out error ) )
					return false;

				if( !fullySerialized )
				{
					if( !Load_LoadMemberForNotRegisteredType( context, ref obj, member, block, out error ) )
						return false;
				}

				return true;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public static bool IsMemberSerializable( Metadata.Member member )
			{
				var property = member as Metadata.Property;
				if( property != null && /*!property.ReadOnly &&*/ !property.HasIndexers && !property.Static )
				{
					var type = property.Serializable;
					if( type == SerializeType.Enable || ( type == SerializeType.Auto && property.Browsable && !property.ReadOnly ) )
					{
						//!!!!может еще что-то проверять

						//var memberData = new MetadataPropertyData( property );

						//if( !Serialization_LoadMemberRecursive( context, ref obj, memberData, block, out var error2 ) )
						//{
						//	error = error2 + $" Property \'{property.Name}\'.";
						//	return false;
						//}

						return true;
					}
				}

				//!!!!другое

				return false;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public static bool LoadSerializableMember( Metadata.LoadContext context, object obj, Metadata.Member member, TextBlock block, out string error )
			{
				var property = member as Metadata.Property;
				if( property != null && /*!property.ReadOnly &&*/ !property.HasIndexers && !property.Static )
				{
					var type = property.Serializable;
					if( type == SerializeType.Enable || ( type == SerializeType.Auto && property.Browsable && !property.ReadOnly ) )
					{
						var memberData = new MetadataPropertyData( property );

						if( !Serialization_LoadMemberRecursive( context, ref obj, memberData, block, out var error2 ) )
						{
							error = error2 + $" Property \'{property.Name}\'.";
							return false;
						}
					}
				}

				//!!!!другое

				error = "";
				return true;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public static bool LoadSerializableMembers( Metadata.LoadContext context, object obj, TextBlock block, out string error )
			{
				foreach( var member in MetadataGetMembers( obj, new Metadata.GetMembersContext( false ) ) )
				{
					if( !LoadSerializableMember( context, obj, member, block, out error ) )
						return false;
				}

				error = "";
				return true;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			static bool Save_SaveMemberForRegisteredType( Metadata.SaveContext context, ref object obj, MemberData member, TextBlock block, out bool fullySerialized, out string error )
			{
				SerializableTypeItem item = GetSuitableSerializableType( member.Type.GetNetType(), out fullySerialized );
				if( item != null )
				{
					object value = member.GetValue( obj );

					bool defaultValueSpecified = false;
					object defaultValue = null;
					if( context.UseDefaultValue )
					{
						member.GetDefaultValue( obj, out defaultValueSpecified, out defaultValue );
						//bool defaultValueSpecified = member.DefaultValueSpecified;
						//object defaultValue = member.DefaultValue;
					}

					string error2 = "";
					item.Save( context, member.Type, member.ParameterName, block, value, defaultValueSpecified, defaultValue, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
					{
						error = error2;
						return false;
					}
				}

				error = "";
				return true;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			static bool Save_SaveMemberForNotRegisteredType( Metadata.SaveContext context, ref object obj, MemberData member, TextBlock block, out string error )
			{
				object value = member.GetValue( obj );

				//!!!!!проверять default value можно

				if( value != null )//!!!!!!так? а если default value не null?
				{
					//!!!!где еще так?
					string typeName = "";
					if( !member.Type.GetNetType().IsValueType )
					{
						//!!!!
						var typeName2 = MetadataGetType( value ).Name;
						if( typeName2 != member.Type.Name )
							typeName = typeName2;
					}

					TextBlock memberBlock = block.FindChild( member.ParameterName );
					if( memberBlock == null )
						memberBlock = block.AddChild( member.ParameterName, typeName );

					if( !SaveSerializableMembers( context, value, memberBlock, out error ) )
						return false;
				}

				error = "";
				return true;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public static bool Serialization_SaveMemberRecursive( Metadata.SaveContext context, ref object obj, MemberData member, TextBlock block, out string error )
			{
				bool fullySerialized;
				if( !Save_SaveMemberForRegisteredType( context, ref obj, member, block, out fullySerialized, out error ) )
					return false;

				if( !fullySerialized )
				{
					if( !Save_SaveMemberForNotRegisteredType( context, ref obj, member, block, out error ) )
						return false;
				}

				error = "";
				return true;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public static bool SaveSerializableMember( Metadata.SaveContext context, object obj, Metadata.Member member, TextBlock block, out bool serialized, out string error )
			{
				serialized = false;

				var property = member as Metadata.Property;
				if( property != null && /*!property.ReadOnly &&*/ !property.HasIndexers && !property.Static )
				{
					var type = property.Serializable;
					if( type == SerializeType.Enable || ( type == SerializeType.Auto && property.Browsable && !property.ReadOnly ) )
					{
						bool skip = false;

						//Type Settings filter
						var component = obj as Component;
						if( component != null )
						{
							if( !component.TypeSettingsIsPublicMember( property ) )
								skip = true;
							//var baseComponentType = component.BaseType as Metadata.ComponentTypeInfo;
							//if( baseComponentType != null && ComponentUtility.TypeSettingsCheckHideObject( baseComponentType.BasedOnObject, true, member ) )
							//	skip = true;
						}

						if( !skip )
						{
							var memberData = new MetadataPropertyData( property );

							if( !Serialization_SaveMemberRecursive( context, ref obj, memberData, block, out var error2 ) )
							{
								error = error2 + $" Property \'{property.Name}\'.";
								return false;
							}

							serialized = true;
						}
					}
				}

				//!!!!другое

				error = "";
				return true;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public static bool SaveSerializableMembers( Metadata.SaveContext context, object obj, TextBlock block, out string error )
			{
				foreach( var member in MetadataGetMembers( obj, new Metadata.GetMembersContext( false ) ) )
				{
					if( !SaveSerializableMember( context, obj, member, block, out _, out error ) )
						return false;
				}

				error = "";
				return true;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public static void CloneMemberValues( Metadata.CloneContext context, object sourceObject, object newObject )
			{
				//members
				foreach( var member in MetadataGetMembers( sourceObject, new Metadata.GetMembersContext( false ) ) )
				{
					//!!!!!если разные классы. TypeInfo еще
					if( sourceObject.GetType() != newObject.GetType() )
						Log.Fatal( "Clone: Different types. obj.GetType() != newObject.GetType()." );

					//property
					var property = member as Metadata.Property;

					if( property != null && /*!property.ReadOnly &&*/ !property.HasIndexers && !property.Static )
					{
						CloneType type = property.Cloneable;
						if( type == CloneType.Auto )
						{
							if( property.Browsable && !property.ReadOnly )
							{
								//!!!!new
								bool isComponent = GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( property.TypeUnreferenced );
								if( isComponent )
									type = CloneType.Shallow;
								else
									type = CloneType.Deep;
								//type = CloneType.Deep;
							}
							else
								type = CloneType.Disable;
						}
						//Metadata.CloneType cloneType;
						//if( property.Cloneable != Metadata.CloneType.NotSpecified )
						//{
						//	cloneType = property.Cloneable;
						//}
						//else
						//{
						//	if( property.Browsable )
						//		cloneType = Metadata.CloneType.Deep;
						//	else
						//		cloneType = Metadata.CloneType.None;
						//}

						if( type != CloneType.Disable )
						{
							object value = property.GetValue( sourceObject, null );

							bool skip = false;

							//!!!!slowly?

							//skip "this:" references for CreateInstanceOfType mode when it is referenced upper than top component
							if( value != null )
							{
								var reference = value as IReference;
								if( context.TypeOfCloning == Metadata.CloneContext.TypeOfCloningEnum.CreateInstanceOfType &&
									reference != null && reference.ReferenceSpecified && sourceObject is Component )
								{
									//!!!!slowly

									ReferenceUtility.GetThisReferenceAmountOfToUpSteps( reference.GetByReference, out var isThis, out var amount );
									if( isThis )
									{
										var topObject = context.BaseTypeForCreationInstance.BasedOnObject;
										var objLevel = ComponentUtility.HowDeepChildInHierarchy( topObject, (Component)sourceObject );
										if( amount > objLevel )
											skip = true;
									}
								}
							}

							//convert "root:" references to full when referenced to another resource
							if( value != null )
							{
								var reference = value as IReference;
								if( context.TypeOfCloning == Metadata.CloneContext.TypeOfCloningEnum.CreateInstanceOfType &&
									reference != null && reference.ReferenceSpecified && sourceObject is Component )
								{
									//!!!!slowly?

									var referenceValue = reference.GetByReference;

									bool isRoot = referenceValue.Length >= 5 && string.Compare( referenceValue, 0, "root:", 0, 5 ) == 0;
									if( isRoot )
									{
										var sourceComponent = (Component)sourceObject;
										var newComponent = (Component)newObject;

										var res1 = ComponentUtility.GetResourceInstanceByComponent( sourceComponent );
										var res2 = ComponentUtility.GetResourceInstanceByComponent( newComponent );
										bool differentResources = res1 != res2;

										//!!!!
										//if( differentResources && res1 != null && res1.InstanceType == Resource.InstanceType.Resource && res1.Owner.LoadFromFile )
										if( differentResources && res1 != null && res1.Owner.LoadFromFile )
										{
											var newReferenceValue = res1.Owner.Name;
											if( referenceValue.Length > 5 )
												newReferenceValue += "|" + referenceValue.Substring( 5 );

											//set new value
											value = ReferenceUtility.MakeReference( reference.UnderlyingType, null, newReferenceValue );
										}
									}
								}
							}

							if( !skip )
							{
								if( type == CloneType.Deep )
									CloneObject( context, sourceObject, newObject, property, ref value );

								//!!!!not used
								////unreference (clear GetByReference property)
								//if( context.unreferenceProperties )
								//{
								//	if( reference != null )
								//		value = ReferenceUtils.CreateReference( reference.UnderlyingType, reference.ValueAsObject, null );
								//}

								//!!!!new
								if( !property.ReadOnly )
									property.SetValue( newObject, value, null );
								//property.SetValue( newObject, value, null );
							}
						}
					}

					//!!!!!другие типы мемберов
				}
			}

			[MethodImpl( (MethodImplOptions)512 )]
			static void CloneNotRegisteredType( Metadata.CloneContext context, ref object obj )
			{
				var cloneable = obj as ICloneable;
				if( cloneable != null )
					obj = cloneable.Clone();
				else
				{
					//!!!!!так?
					object newObject = GetTypeOfNetType( obj.GetType() ).InvokeInstance( null );
					CloneMemberValues( context, obj, newObject );

					obj = newObject;
				}
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public static void CloneObject( Metadata.CloneContext context, object sourceObject, object newObject, Metadata.Property property, ref object value )
			{
				if( value != null )
				{
					bool fullySerialized;
					var item = GetSuitableSerializableType( value.GetType(), out fullySerialized );
					//var item = GetSuitableSerializableType( valueType );
					if( item != null )
					{
						//registered type
						item.Clone( context, sourceObject, newObject, property, ref value );
					}
					if( !fullySerialized )//else
					{
						//not registered type
						CloneNotRegisteredType( context, /*sourceObject, newObject, */ref value );
					}
				}
			}
		}
	}
}
