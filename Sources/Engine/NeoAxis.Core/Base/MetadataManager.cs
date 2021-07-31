// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Specifies a manager for working with engine metadata.
	/// </summary>
	public static partial class MetadataManager
	{
		//!!!!threading

		//!!!!!посмотреть не накапливается ли. значит это нужно наглядно показывать. записывать как счетчик. графиком выводить. виды записывателей.
		//!!!!!тут вроде как складируются еще генерики не дефинишены
		static Dictionary<Type, Metadata.NetTypeInfo> netTypes = new Dictionary<Type, Metadata.NetTypeInfo>();
		static Dictionary<string, Metadata.NetTypeInfo> netTypeByName = new Dictionary<string, Metadata.NetTypeInfo>();

		static Dictionary<string, ReferenceType> referenceTypes = new Dictionary<string, ReferenceType>();
		//optimization
		static ReferenceType referenceTypeResource;

		static EDictionary<Type, SerializableTypeItem> serializableTypes = new EDictionary<Type, SerializableTypeItem>();
		static SerializableTypeItem serializableTypes_Enum;
		static SerializableTypeItem serializableTypes_Array;

		static object lockObject = new object();

		static Dictionary<Type, string> netTypeShortNames;

		static Dictionary<(Type, Type), AutoConvertTypeItem> autoConvertTypes = new Dictionary<(Type, Type), AutoConvertTypeItem>();

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a data of supported reference type.
		/// </summary>
		public class ReferenceType
		{
			internal string name;
			internal GetValueDelegate getValueFunction;
			internal GetMemberDelegate getMemberFunction;

			//

			public delegate void GetValueDelegate( Type expectedType, object owner, string referenceContent, out object value );
			public delegate void GetMemberDelegate( Type expectedType, object owner, string referenceContent, out object outObject, out Metadata.Member outMember );

			public string Name
			{
				get { return name; }
			}

			public GetValueDelegate GetValueFunction
			{
				get { return getValueFunction; }
			}

			public GetMemberDelegate GetMemberFunction
			{
				get { return getMemberFunction; }
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents an item of serializable type.
		/// </summary>
		public class SerializableTypeItem
		{
			internal Type type;
			internal bool oneStringSerialization;
			internal LoadDelegate load;
			internal SaveDelegate save;
			internal CloneDelegate clone;

			//

			public delegate void LoadDelegate( Metadata.LoadContext context, Metadata.TypeInfo type, string parameterName, Serialization.MemberData memberOptional, TextBlock block, ref bool gotValue, ref object value, ref string error );
			public delegate void SaveDelegate( Metadata.SaveContext context, Metadata.TypeInfo type, string parameterName, TextBlock block, object value,
				bool defaultValueSpecified, object defaultValue, ref string error );
			public delegate void CloneDelegate( Metadata.CloneContext context, object sourceObject, object newObject, Metadata.Property property, ref object value );

			//

			public Type Type
			{
				get { return type; }
			}

			public bool OneStringSerialization
			{
				get { return oneStringSerialization; }
			}

			public LoadDelegate Load
			{
				get { return load; }
			}

			public SaveDelegate Save
			{
				get { return save; }
			}

			public CloneDelegate Clone
			{
				get { return clone; }
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents an item of autoconvertable type.
		/// </summary>
		public class AutoConvertTypeItem
		{
			public delegate object ConvertDelegate( object value );

			ConvertDelegate convertFunction;
			MethodInfo convertMethod;
			ConstructorInfo convertConstructor;

			public AutoConvertTypeItem( ConvertDelegate convertFunction )
			{
				this.convertFunction = convertFunction;
			}

			public AutoConvertTypeItem( MethodInfo convertMethod )
			{
				this.convertMethod = convertMethod;
			}

			public AutoConvertTypeItem( ConstructorInfo convertConstructor )
			{
				this.convertConstructor = convertConstructor;
			}

			public ConvertDelegate ConvertFunction
			{
				get { return convertFunction; }
			}

			public MethodInfo ConvertMethod
			{
				get { return convertMethod; }
			}

			public ConstructorInfo ConvertConstructor
			{
				get { return convertConstructor; }
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		static MetadataManager()
		{
			netTypeShortNames = new Dictionary<Type, string>();
			netTypeShortNames[ typeof( string ) ] = "string";
			netTypeShortNames[ typeof( bool ) ] = "bool";
			netTypeShortNames[ typeof( byte ) ] = "byte";
			netTypeShortNames[ typeof( sbyte ) ] = "sbyte";
			netTypeShortNames[ typeof( char ) ] = "char";
			netTypeShortNames[ typeof( decimal ) ] = "decimal";
			netTypeShortNames[ typeof( double ) ] = "double";
			netTypeShortNames[ typeof( float ) ] = "float";
			netTypeShortNames[ typeof( int ) ] = "int";
			netTypeShortNames[ typeof( uint ) ] = "uint";
			netTypeShortNames[ typeof( long ) ] = "long";
			netTypeShortNames[ typeof( ulong ) ] = "ulong";
			netTypeShortNames[ typeof( object ) ] = "object";
			netTypeShortNames[ typeof( short ) ] = "short";
			netTypeShortNames[ typeof( ushort ) ] = "ushort";

			AssemblyUtility.RegisterAssembly( Assembly.GetExecutingAssembly(), "" );
			RegisterStandardReferenceTypes();
			Serialization.RegisterStandardSerializableTypes();

			RegisterStandardAutoConvertItems();
		}

		public static ICollection<Metadata.NetTypeInfo> NetTypes
		{
			get
			{
				lock( lockObject )
				{
					//!!!!!array can be cached
					Metadata.NetTypeInfo[] array = new Metadata.NetTypeInfo[ netTypes.Count ];
					netTypes.Values.CopyTo( array, 0 );
					return array;
				}
			}
		}

		//internal static ESet<string> added = new ESet<string>();

		internal static void RegisterTypesOfAssembly( Assembly assembly, Type[] types = null )
		{
			lock( lockObject )
			{
				if( types == null )
					types = assembly.GetExportedTypes();

				foreach( var type in types )//assembly.GetExportedTypes() )
				{
					//skip disabled types by namespace name
					if( AssemblyUtility.disableNamespaceRegistration == null )
						AssemblyUtility.ParseDisableAssemblyNamespaceRegistration();
					if( AssemblyUtility.disableNamespaceRegistration.Contains( type.Namespace ) )
						continue;

					//added.AddWithCheckAlreadyContained( type.Namespace );

					GetTypeOfNetType( type );//, false );
				}
			}
		}

		public static string GetNetTypeShortName( Type type )
		{
			string str;
			if( netTypeShortNames.TryGetValue( type, out str ) )
				return str;
			return null;
		}

		//!!!!так?
		public static string GetNetTypeName( Type type, bool displayName, bool addNamespaceDeclaringTypeAssemblyName )
		//static string GetNetTypeName( Type type, bool displayName )
		{
			//!!!!slowly там где юзается?

			if( displayName )
			{
				var shortName = GetNetTypeShortName( type );
				if( shortName != null )
					return shortName;
			}

			//!!!!!out ref

			var name = new StringBuilder( 256 );

			//base name
			{
				//!!!!!
				if( addNamespaceDeclaringTypeAssemblyName )
				{
					//!!!!new
					var type2 = type;
					if( type.IsArray )
						type2 = type.GetElementType();

					if( type2.IsNested && type2.DeclaringType != null )//!!!!! && !type.IsGenericType )
					{
						name.Append( GetNetTypeName( type2.DeclaringType, displayName, addNamespaceDeclaringTypeAssemblyName ) );
						//baseName = GetTypeOfNetType( type.DeclaringType ).Name;
					}
					else
					{
						//!!!maybe slowly. NetTypeInfo.Namespace is faster.
						name.Append( type2.Namespace );
					}
					name.Append( '.' );
				}

				//name.Append( baseName );
			}

			if( type.IsGenericType )
			{
				int lastIndex = type.Name.LastIndexOf( "`" );
				if( lastIndex != -1 )
				{
					name.Append( type.Name.Substring( 0, lastIndex ) );

					name.Append( '<' );
					Type[] types = type.GetGenericArguments();
					for( int n = 0; n < types.Length; n++ )
					{
						if( n != 0 )
							name.Append( ',' );

						//!!!!!так?
						if( type.IsGenericTypeDefinition )
							name.Append( types[ n ].Name );
						else
							name.Append( GetNetTypeName( types[ n ], displayName, addNamespaceDeclaringTypeAssemblyName ) );
					}
					name.Append( '>' );
				}
			}
			else
			{
				name.Append( type.Name );
			}

			if( addNamespaceDeclaringTypeAssemblyName )
			{
				var a = AssemblyUtility.GetRegisteredAssembly( type.Assembly );
				if( a != null && !string.IsNullOrEmpty( a.RegisterTypeNamesWithIncludedAssemblyName ) )
				{
					//!!!! :
					name.Append( a.RegisterTypeNamesWithIncludedAssemblyName );
					name.Append( ':' );
					name.Append( name );
					//name.AppendFormat( "{0}:{1}", a.RegisterTypeNamesWithIncludedAssemblyName, name );
				}
			}

			return name.ToString();

			//string baseName = "";

			////!!!!!
			//if( addNamespaceDeclaringTypeAssemblyName )
			//{
			//	if( type.IsNested && type.DeclaringType != null )//!!!!! && !type.IsGenericType )
			//	{
			//		baseName = GetNetTypeName( type.DeclaringType, displayName, addNamespaceDeclaringTypeAssemblyName );
			//		//baseName = GetTypeOfNetType( type.DeclaringType ).Name;
			//	}
			//	else
			//	{
			//		//!!!maybe slowly. NetTypeInfo.Namespace is faster.
			//		baseName = type.Namespace;
			//	}
			//	baseName += ".";
			//}

			////!!!!!out ref

			//string name = baseName;

			//if( type.IsGenericType )
			//{
			//	int lastIndex = type.Name.LastIndexOf( "`" );
			//	if( lastIndex != -1 )
			//	{
			//		name += type.Name.Substring( 0, lastIndex );

			//		name += "<";
			//		Type[] types = type.GetGenericArguments();
			//		for( int n = 0; n < types.Length; n++ )
			//		{
			//			if( n != 0 )
			//				name += ",";

			//			//!!!!!так?
			//			if( type.IsGenericTypeDefinition )
			//				name += types[ n ].Name;
			//			else
			//				name += GetNetTypeName( types[ n ], displayName, addNamespaceDeclaringTypeAssemblyName );
			//		}
			//		name += ">";
			//	}
			//}
			//else
			//{
			//	name += type.Name;
			//}

			//if( addNamespaceDeclaringTypeAssemblyName )
			//{
			//	var a = AssemblyUtility.GetRegisteredAssembly( type.Assembly );
			//	if( a != null && !string.IsNullOrEmpty( a.RegisterTypeNamesWithIncludedAssemblyName ) )
			//	{
			//		//!!!! :
			//		name = string.Format( "{0}:{1}", a.RegisterTypeNamesWithIncludedAssemblyName, name );
			//	}
			//}

			//return name;
		}

		public static Metadata.NetTypeInfo GetTypeOfNetType( Type type )//, bool canRegisterAssembly = true )
		{
			//!!!!! TODO: test for support refs return types https://docs.microsoft.com/ru-ru/dotnet/csharp/programming-guide/classes-and-structs/ref-returns
			//if( type.IsByRef )
			//	Log.Info( "MetadataManager: GetTypeOfNetType: type.IsByRef == true." );

			lock( lockObject )
			{
				//check already registered
				{
					Metadata.NetTypeInfo info2;
					if( netTypes.TryGetValue( type, out info2 ) )
						return info2;
				}

				//register base type
				Metadata.TypeInfo baseType = null;
				if( type.BaseType != null )
					baseType = GetTypeOfNetType( type.BaseType );

				//!!!!!только цельными сборками?
				//register assembly
				//if( canRegisterAssembly )//&& !assemblies.Contains( type.Assembly ) )
				AssemblyUtility.RegisterAssembly( type.Assembly, "" );

				Metadata.NetTypeInfo info;
				if( !netTypes.TryGetValue( type, out info ) )
				{
					EDictionary<string, long> enumElements = null;
					bool enumFlags = false;
					if( type.IsEnum )
					{
						enumElements = new EDictionary<string, long>();

						Type underlyingType = Enum.GetUnderlyingType( type );

						try
						{
							foreach( var value in type.GetEnumValues() )
							{
								long v = 0;

								//!!!!slowly

								unchecked
								{
									if( underlyingType == typeof( byte ) )
										v = (byte)value;
									else if( underlyingType == typeof( sbyte ) )
										v = (sbyte)value;
									else if( underlyingType == typeof( short ) )
										v = (short)value;
									else if( underlyingType == typeof( ushort ) )
										v = (ushort)value;
									else if( underlyingType == typeof( int ) )
										v = (int)value;
									else if( underlyingType == typeof( uint ) )
										v = (uint)value;
									else if( underlyingType == typeof( long ) )
										v = (long)value;
									else if( underlyingType == typeof( ulong ) )
										v = (long)(ulong)value;
								}

								//!!!!не так ведь. если два имени разных, но значения одинаковые, то не сработает

								var name2 = type.GetEnumName( value );
								enumElements[ name2 ] = v;
							}
						}
						catch { }

						if( type.GetCustomAttributes( typeof( FlagsAttribute ), false ).Length != 0 )
							enumFlags = true;
					}

					var name = GetNetTypeName( type, false, true );
					var displayName = GetNetTypeName( type, true, false );


					//!!!!
					Metadata.TypeClassification classification;
					if( typeof( Component ).IsAssignableFrom( type ) )
						classification = Metadata.TypeClassification.Component;
					else
					{
						if( type.IsEnum )
							classification = Metadata.TypeClassification.Enumeration;
						else
						{
							if( typeof( Delegate ).IsAssignableFrom( type ) )
								classification = Metadata.TypeClassification.Delegate;
							else
							{
								//!!!!!value types?

								classification = Metadata.TypeClassification.Class;
							}
						}
					}

					info = new Metadata.NetTypeInfo( name, displayName, baseType, classification, enumElements, enumFlags, type );

					netTypes[ type ] = info;
					netTypeByName[ name ] = info;

					try
					{

						info.InitMembers();

					}
					catch( Exception /*e*/ )
					{
						//!!!!
						//Log.Info( e.Message );
					}
				}

				//!!!!new
				//register generic parameters
				if( type.IsGenericType )
				{
					foreach( var t in type.GetGenericArguments() )
						GetTypeOfNetType( t );
				}

				//info.InitAutoCreatedInstance();

				return info;
			}
		}

		public static Metadata.NetTypeInfo GetTypeOfNetType( string typeName )
		{
			lock( lockObject )
			{
				Metadata.NetTypeInfo type;
				netTypeByName.TryGetValue( typeName, out type );
				return type;
			}
		}

		//при вызове тип уже готов. после он будет инстанситься

		public static Metadata.TypeInfo GetType( string typeName )
		{
			//!!!!threading

			if( string.IsNullOrEmpty( typeName ) )
				return null;

			//!!!!!override events. Begin, End. End - чтобы добавить?

			//get by class name
			{
				//!!!!а может еще имя файла сборки?
				var type = GetTypeOfNetType( typeName );
				if( type != null )
					return type;
			}

			//get by resource name
			{
				string fileName;
				string pathInside;
				{
					int index = typeName.IndexOf( '|' );
					if( index != -1 )
					{
						fileName = typeName.Substring( 0, index );
						pathInside = typeName.Substring( index + 1 );
					}
					else
					{
						fileName = typeName;
						pathInside = "";
					}
				}

				//!!!!threading
				//!!!!new
				//get if already loaded or during loading
				var res = ResourceManager.GetByName( fileName );
				if( res != null )
				{
					var ins = res.PrimaryInstance;
					if( ins != null )
						return ins.MetadataGetType( pathInside );
				}

				//!!!!new
				if( ResourceManager.GetByName( fileName ) != null || VirtualFile.Exists( fileName ) )
				//if( VirtualFile.Exists( fileName ) )
				{
					//!!!!только такой вариант создания?
					//!!!!!так сразу?
					//!!!!!threading
					//!!!!error

					var ins = ResourceManager.LoadResource( fileName, true );
					if( ins != null )
						return ins.MetadataGetType( pathInside );
				}

				////!!!!slowly
				////!!!!!так?
				//if( VirtualFile.Exists( fileName ) )
				//{
				//	//!!!!только такой вариант создания?
				//	//!!!!!так сразу?
				//	//!!!!!threading
				//	//!!!!error

				//	var ins = ResourceManager.LoadResource( fileName, true );
				//	if( ins == null )
				//	{
				//		//!!!!!
				//		Log.Fatal( "impl" );
				//	}

				//	return ins.MetadataGetType( pathInside );
				//}
			}

			//!!!!new types

			return null;
		}

		public static Metadata.TypeInfo MetadataGetType( object obj )
		{
			Metadata.IMetadataProvider provider = obj as Metadata.IMetadataProvider;
			if( provider != null )
			{
				var type = provider.BaseType;

				//!!!!new. strange, but works. without it crashes if select "object" for Type in _Property.
				if( type == null )
					type = GetTypeOfNetType( typeof( object ) );

				return type;

				//return provider.BaseType;
			}
			else
				return GetTypeOfNetType( obj.GetType() );
		}

		public static IEnumerable<Metadata.Member> MetadataGetMembers( object obj, Metadata.GetMembersContext context = null )
		{
			Metadata.IMetadataProvider provider = obj as Metadata.IMetadataProvider;
			if( provider != null )
			{
				foreach( var m in provider.MetadataGetMembers( context ) )
					yield return m;
			}
			else
			{
				var type = GetTypeOfNetType( obj.GetType() );
				foreach( var m in type.MetadataGetMembers( context ) )
					yield return m;
			}
		}

		public static Metadata.Member MetadataGetMemberBySignature( object obj, string signature, Metadata.GetMembersContext context = null )
		{
			Metadata.IMetadataProvider provider = obj as Metadata.IMetadataProvider;
			if( provider != null )
			{
				return provider.MetadataGetMemberBySignature( signature, context );
			}
			else
			{
				var type = GetTypeOfNetType( obj.GetType() );
				return type.MetadataGetMemberBySignature( signature, context );
			}
		}

		////по идее генериком можно еще
		//public static Metadata.Member MetadataGetMemberByName( object obj, Metadata.MemberType type, string name )
		//{
		//	bool includeBaseTypes = true;

		//	Metadata.IMetadataProvider provider = obj as Metadata.IMetadataProvider;
		//	if( provider != null )
		//	{
		//		return provider.MetadataGetMemberByName( type, name, includeBaseTypes );
		//	}
		//	else
		//	{
		//		var type2 = GetTypeOfNetType( obj.GetType() );
		//		x;
		//		return type.MetadataGetMemberByName( type2, name, includeBaseTypes );
		//	}
		//}

		//!!!!по сути вспомогательный метод
		//!!!!constructor parameters
		//public static object InvokeInstance( string typeName )
		//{
		//	var type = GetType( typeName );
		//	if( type == null )
		//	{
		//		//!!!!!
		//		Log.Fatal( "impl" );
		//	}

		//	return type.InvokeInstance( null );
		//}

		/////////////////////////////////////////

		public static ICollection<ReferenceType> ReferenceTypes
		{
			get { return referenceTypes.Values; }
		}

		public static ReferenceType GetReferenceType( string name )
		{
			ReferenceType type;
			referenceTypes.TryGetValue( name, out type );
			return type;
		}

		public static ReferenceType RegisterReferenceType( string name, bool canOverridePrevious, ReferenceType.GetValueDelegate getValueFunction, ReferenceType.GetMemberDelegate getMemberFunction )
		{
			name = name.ToLower();

			if( !canOverridePrevious && GetReferenceType( name ) != null )
				Log.Fatal( $"ComponentManager: RegisterReferenceType: The reference type with name \'{name}\' is already registered." );

			ReferenceType type = new ReferenceType();
			type.name = name;
			type.getValueFunction = getValueFunction;
			type.getMemberFunction = getMemberFunction;
			referenceTypes[ name ] = type;
			return type;
		}

		//!!!!надо ли GetReferenceValue?
		static object GetReferenceValue( Type expectedType, object startObject, string path, Metadata.TypeInfo startTypeFromStaticMember )
		{
			//!!!!!slowly

			object current = startObject;
			Metadata.TypeInfo currentTypeFromStaticMember = startTypeFromStaticMember;

			//!!!!пустых между быть не может
			string[] strs = path.Split( new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries );
			for( int nStr = 0; nStr < strs.Length; nStr++ )
			{
				var str = strs[ nStr ];
				bool lastStr = nStr == strs.Length - 1;

				if( str == ".." )
				{
					var c = current as Component;
					if( c != null )
					{
						current = c.Parent;
						currentTypeFromStaticMember = null;

						//!!!!
						if( current == null )
						{
							//!!!!invalid reference
							return null;
						}
					}
					else
					{
						//!!!!invalid reference
						return null;
					}
				}
				else
				{
					//!!!!Methods, etc

					if( currentTypeFromStaticMember != null )
					{
						//static

						//specialization for ReferenceValueType_Member
						if( lastStr && typeof( ReferenceValueType_Member ).IsAssignableFrom( expectedType ) )
						{
							//get member

							//!!!!slowly
							//!!!!может что-то еще проверять?
							bool isMethod = str.Length > 7 && str.Substring( 0, 7 ) == "method:";
							bool isProperty = str.Length > 9 && str.Substring( 0, 9 ) == "property:";
							bool isEvent = str.Length > 6 && str.Substring( 0, 6 ) == "event:";
							if( isMethod || isProperty || isEvent )
							{
								var m = currentTypeFromStaticMember.MetadataGetMemberBySignature( str );
								if( m != null )
								{
									if( isMethod && typeof( ReferenceValueType_Method ).IsAssignableFrom( expectedType ) && m is Metadata.Method )
										return new ReferenceValueType_Method( (Metadata.Method)m, null );
									if( isProperty && typeof( ReferenceValueType_Property ).IsAssignableFrom( expectedType ) && m is Metadata.Property )
										return new ReferenceValueType_Property( (Metadata.Property)m, null );
									if( isEvent && typeof( ReferenceValueType_Event ).IsAssignableFrom( expectedType ) && m is Metadata.Event )
										return new ReferenceValueType_Event( (Metadata.Event)m, null );
									return new ReferenceValueType_Member( m, null );
								}
							}
						}

						//default behavior
						{
							var property = currentTypeFromStaticMember.MetadataGetMemberBySignature( "property:" + str ) as Metadata.Property;
							Metadata.Method method = null;
							if( property == null )
								method = currentTypeFromStaticMember.MetadataGetMemberBySignature( "method:" + str + "()" ) as Metadata.Method;

							if( property != null || method != null )
							{
								object value;
								if( property != null )
									value = property.GetValue( null, null );
								else
									value = method.Invoke( null, new object[ 0 ] );

								if( value != null )
								{
									//extract Reference<T>
									var iReference = value as IReference;
									if( iReference != null )
									{
										value = iReference.ValueAsObject;
										if( value == null )
										{
											//!!!!invalid reference?
											return null;
										}
									}

									current = value;
									currentTypeFromStaticMember = null;
								}
								else
								{
									//!!!!invalid reference?
									return null;
								}
							}
							else
							{
								//!!!!invalid reference
								return null;
							}
						}
					}
					else
					{
						//instance

						if( str.Length > 0 && str[ 0 ] == '$' )
						{
							//component

							var c = current as Component;
							if( c != null )
							{
								Component child = null;
								if( Component.ComponentSet.ParsePathNameWithIndex( str, out string name, out int nameIndex ) )
									child = c.Components.GetByNameWithIndex( name, nameIndex );
								//var child = c.Components[ str ];

								if( child != null )
								{
									current = child;
									currentTypeFromStaticMember = null;
								}
								else
								{
									//!!!!invalid reference
									return null;
								}
							}
							else
							{
								//!!!!invalid reference
								return null;
							}
						}
						else
						{
							//property, method

							//!!!!
							//specialization for ReferenceValueType_Member
							if( lastStr && typeof( ReferenceValueType_Member ).IsAssignableFrom( expectedType ) )
							{
								//get member

								//!!!!slowly
								//!!!!может что-то еще проверять?
								bool isMethod = str.Length > 7 && str.Substring( 0, 7 ) == "method:";
								bool isProperty = str.Length > 9 && str.Substring( 0, 9 ) == "property:";
								bool isEvent = str.Length > 6 && str.Substring( 0, 6 ) == "event:";
								if( isMethod || isProperty || isEvent )
								{
									var m = MetadataGetMemberBySignature( current, str );
									if( m != null )
									{
										if( isMethod && typeof( ReferenceValueType_Method ).IsAssignableFrom( expectedType ) && m is Metadata.Method )
											return new ReferenceValueType_Method( (Metadata.Method)m, current );
										if( isProperty && typeof( ReferenceValueType_Property ).IsAssignableFrom( expectedType ) && m is Metadata.Property )
											return new ReferenceValueType_Property( (Metadata.Property)m, current );
										if( isEvent && typeof( ReferenceValueType_Event ).IsAssignableFrom( expectedType ) && m is Metadata.Event )
											return new ReferenceValueType_Event( (Metadata.Event)m, current );
										return new ReferenceValueType_Member( m, current );
									}
								}
							}

							//default behavior
							{
								Metadata.Property property = null;
								Metadata.Method method = null;
								property = MetadataGetMemberBySignature( current, "property:" + str ) as Metadata.Property;
								if( property == null )
									method = MetadataGetMemberBySignature( current, "method:" + str + "()" ) as Metadata.Method;

								if( property != null || method != null )
								{
									object value;
									if( property != null )
										value = property.GetValue( current, null );
									else
										value = method.Invoke( current, new object[ 0 ] );

									if( value != null )
									{
										//extract Reference<T>
										var iReference = value as IReference;
										if( iReference != null )
										{
											value = iReference.ValueAsObject;
											if( value == null )
											{
												//!!!!invalid reference?
												return null;
											}
										}

										current = value;
										currentTypeFromStaticMember = null;
									}
									else
									{
										//!!!!invalid reference?
										return null;
									}
								}
								else
								{
									//!!!!invalid reference?
									return null;
								}
							}
						}
					}
				}
			}

			//specialization for Metadata.TypeInfo
			if( typeof( Metadata.TypeInfo ).IsAssignableFrom( expectedType ) )
			{
				if( currentTypeFromStaticMember != null )
					return currentTypeFromStaticMember;
				else
				{
					var component = current as Component;
					if( component != null )
					{
						var type = component.GetProvidedType();
						if( type != null )
							return type;
					}
				}
			}

			return current;
		}

		//!!!!надо ли GetReferenceValue?
		static void GetMemberProperty( Type expectedType, object startObject, string path, Metadata.TypeInfo startTypeFromStaticMember, out object outObject, out Metadata.Member outMember )
		{
			outObject = null;
			outMember = null;

			//!!!!!slowly

			object current = startObject;
			Metadata.TypeInfo currentTypeFromStaticMember = startTypeFromStaticMember;

			//!!!!
			object lastObject = null;
			Metadata.Member lastMember = null;

			//!!!!пустых между быть не может
			string[] strs = path.Split( new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries );
			//foreach( var str in strs )
			for( int nStr = 0; nStr < strs.Length; nStr++ )
			{
				var str = strs[ nStr ];
				bool isLast = nStr == strs.Length - 1;

				if( str == ".." )
				{
					var c = current as Component;
					if( c != null )
					{
						current = c.Parent;
						currentTypeFromStaticMember = null;

						//!!!!
						if( current == null )
						{
							//!!!!invalid reference
							return;
						}
					}
					else
					{
						//!!!!invalid reference
						return;
					}
				}
				else
				{
					//!!!!!Methods, etc

					if( currentTypeFromStaticMember != null )
					{
						//static

						var property = currentTypeFromStaticMember.MetadataGetMemberBySignature( "property:" + str ) as Metadata.Property;
						Metadata.Method method = null;
						if( property == null )
							method = currentTypeFromStaticMember.MetadataGetMemberBySignature( "method:" + str + "()" ) as Metadata.Method;

						if( property != null || method != null )
						{
							//!!!!только тут?
							lastObject = null;//current;
							if( property != null )
								lastMember = property;
							else
								lastMember = method;

							object value;
							if( property != null )
								value = property.GetValue( null, null );
							else
								value = method.Invoke( null, new object[ 0 ] );

							if( value != null )
							{
								//extract Reference<T>
								var iReference = value as IReference;
								if( iReference != null )
								{
									value = iReference.ValueAsObject;
									if( value == null )
									{
										//!!!!invalid reference?
										return;
									}
								}

								current = value;
								currentTypeFromStaticMember = null;
							}
							else
							{
								if( !isLast )
									return;
								////!!!!invalid reference?
								//return;
							}
						}
						else
						{
							//!!!!invalid reference
							return;
						}
					}
					else
					{
						//instance

						if( str.Length > 0 && str[ 0 ] == '$' )
						{
							//component

							var c = current as Component;
							if( c != null )
							{
								Component child = null;
								if( Component.ComponentSet.ParsePathNameWithIndex( str, out string name, out int nameIndex ) )
									child = c.Components.GetByNameWithIndex( name, nameIndex );
								//var child = c.Components[ str ];

								if( child != null )
								{
									current = child;
									currentTypeFromStaticMember = null;
								}
								else
								{
									//!!!!invalid reference
									return;
								}
							}
							else
							{
								//!!!!invalid reference
								return;
							}
						}
						else
						{
							//property, method

							Metadata.Property property = null;
							Metadata.Method method = null;
							property = MetadataGetMemberBySignature( current, "property:" + str ) as Metadata.Property;
							if( property == null )
								method = MetadataGetMemberBySignature( current, "method:" + str + "()" ) as Metadata.Method;

							if( property != null || method != null )
							{
								//!!!!только тут?
								lastObject = current;
								if( property != null )
									lastMember = property;
								else
									lastMember = method;

								object value;
								if( property != null )
									value = property.GetValue( current, null );
								else
									value = method.Invoke( current, new object[ 0 ] );

								if( value != null )
								{
									//extract Reference<T>
									var iReference = value as IReference;
									if( iReference != null )
									{
										value = iReference.ValueAsObject;
										if( value == null )
										{
											//!!!!invalid reference?
											return;
										}
									}

									current = value;
									currentTypeFromStaticMember = null;
								}
								else
								{
									if( !isLast )
										return;
									////!!!!invalid reference?
									//return;
								}
							}
							else
							{
								//by component child name

							}
						}
					}
				}
			}

			outObject = lastObject;
			outMember = lastMember;
			//return current;
		}

		static void RegisterStandardReferenceTypes()
		{
			//!!!!тут будет загружаться ресурс сразу
			//!!!!можно по идее опциональным параметром поставить как именно загружать. хотя это уже усложнение. без учета оптимизаций - загружается то, что понадобится. т.е. всё.

			//!!!!можно указать внутри файла-компонента какой-то объект вложенный.
			//!!!!множественно вложенные ссылки. они по свойствам вглубь забираются
			//!!!!скрипт камеры простым делегатом сделать -> ссылка 'вызвать функцию/метод/еще что-то'

			//resource
			referenceTypeResource = RegisterReferenceType( "resource", false,
				delegate ( Type expectedType, object owner, string referenceContent, out object value )
				{
					value = null;

					string resourceName;
					string path;
					{
						int splitIndex = referenceContent.IndexOf( '|' );
						if( splitIndex != -1 )
						{
							//with '|'
							resourceName = referenceContent.Substring( 0, splitIndex );
							path = referenceContent.Substring( splitIndex + 1 );
						}
						else
						{
							//resource name without '|'
							resourceName = referenceContent;
							path = "";
						}
					}

					//!!!!!так?

					//get by class name
					{
						var type = GetTypeOfNetType( resourceName );
						if( type != null )
						{
							value = GetReferenceValue( expectedType, null, path, type );
							return;
						}
					}

					//!!!!может код сделать как в public static Metadata.TypeInfo GetType( string typeName )

					if( ResourceManager.GetByName( resourceName ) != null || VirtualFile.Exists( resourceName ) )
					{
						//!!!!threading что тут?
						//!!!!!!separate?
						var ins = ResourceManager.LoadResource( resourceName, true );
						if( ins != null )
						{
							var v = ins.ResultObject;
							if( v != null )
								value = GetReferenceValue( expectedType, v, path, null );
						}
					}
				},
				delegate ( Type expectedType, object owner, string referenceContent, out object outObject, out Metadata.Member outMember )
				{
					outObject = null;
					outMember = null;

					string resourceName;
					string path;
					{
						int splitIndex = referenceContent.IndexOf( '|' );
						if( splitIndex != -1 )
						{
							//with '|'
							resourceName = referenceContent.Substring( 0, splitIndex );
							path = referenceContent.Substring( splitIndex + 1 );
						}
						else
						{
							//resource name without '|'
							resourceName = referenceContent;
							path = "";
						}
					}

					//get by class name
					{
						var type = GetTypeOfNetType( resourceName );
						if( type != null )
						{
							GetMemberProperty( expectedType, null, path, type, out outObject, out outMember );
							return;
						}
					}

					if( ResourceManager.GetByName( resourceName ) != null || VirtualFile.Exists( resourceName ) )
					{
						//!!!!threading что тут?
						//!!!!!!separate?
						var ins = ResourceManager.LoadResource( resourceName, true );
						if( ins != null )
						{
							var v = ins.ResultObject;
							if( v != null )
								GetMemberProperty( expectedType, v, path, null, out outObject, out outMember );
						}
					}
				} );

			//this
			RegisterReferenceType( "this", false,
				delegate ( Type expectedType, object owner, string referenceContent, out object value )
				{
					value = GetReferenceValue( expectedType, owner, referenceContent, null );
				},
				delegate ( Type expectedType, object owner, string referenceContent, out object outObject, out Metadata.Member outMember )
				{
					GetMemberProperty( expectedType, owner, referenceContent, null, out outObject, out outMember );
				} );

			//root
			RegisterReferenceType( "root", false,
				delegate ( Type expectedType, object owner, string referenceContent, out object value )
				{
					object owner2;
					Component c = owner as Component;
					if( c != null )
						owner2 = c.ParentRoot;
					else
						owner2 = owner;
					value = GetReferenceValue( expectedType, owner2, referenceContent, null );
				},
				delegate ( Type expectedType, object owner, string referenceContent, out object outObject, out Metadata.Member outMember )
				{
					object owner2;
					Component c = owner as Component;
					if( c != null )
						owner2 = c.ParentRoot;
					else
						owner2 = owner;
					GetMemberProperty( expectedType, owner2, referenceContent, null, out outObject, out outMember );
				} );

			//relative
			RegisterReferenceType( "relative", false,
				delegate ( Type expectedType, object owner, string referenceContent, out object value )
				{
					//!!!!slowly

					value = null;

					var ownerComponent = owner as Component;
					if( ownerComponent == null )
						return;
					var fromResource = ComponentUtility.GetOwnedFileNameOfComponent( ownerComponent );
					if( string.IsNullOrEmpty( fromResource ) )
						return;

					var fromDirectory = PathUtility.GetDirectoryName( fromResource );

					string resourceName2;
					string path;
					{
						int splitIndex = referenceContent.IndexOf( '|' );
						if( splitIndex != -1 )
						{
							//with '|'
							resourceName2 = referenceContent.Substring( 0, splitIndex );
							path = referenceContent.Substring( splitIndex + 1 );
						}
						else
						{
							//resource name without '|'
							resourceName2 = referenceContent;
							path = "";
						}
					}

					var resourceName = "";
					{
						//!!!!slowly

						resourceName = PathUtility.Combine( fromDirectory, resourceName2 );

						if( resourceName2.Contains( ".." ) )
						{
							//remove dots
							resourceName = VirtualPathUtility.GetVirtualPathByReal( Path.GetFullPath( VirtualPathUtility.GetRealPathByVirtual( resourceName ) ) );
						}
					}

					//get by class name
					{
						var type = GetTypeOfNetType( resourceName );
						if( type != null )
						{
							value = GetReferenceValue( expectedType, null, path, type );
							return;
						}
					}

					//!!!!может код сделать как в public static Metadata.TypeInfo GetType( string typeName )

					if( ResourceManager.GetByName( resourceName ) != null || VirtualFile.Exists( resourceName ) )
					{
						//!!!!threading что тут?
						//!!!!!!separate?
						var ins = ResourceManager.LoadResource( resourceName, true );
						if( ins != null )
						{
							var v = ins.ResultObject;
							if( v != null )
								value = GetReferenceValue( expectedType, v, path, null );
						}
					}
				},
				delegate ( Type expectedType, object owner, string referenceContent, out object outObject, out Metadata.Member outMember )
				{
					outObject = null;
					outMember = null;

					//!!!!slowly

					var ownerComponent = owner as Component;
					if( ownerComponent == null )
						return;
					var fromResource = ComponentUtility.GetOwnedFileNameOfComponent( ownerComponent );
					if( string.IsNullOrEmpty( fromResource ) )
						return;

					var fromDirectory = PathUtility.GetDirectoryName( fromResource );

					string resourceName2;
					string path;
					{
						int splitIndex = referenceContent.IndexOf( '|' );
						if( splitIndex != -1 )
						{
							//with '|'
							resourceName2 = referenceContent.Substring( 0, splitIndex );
							path = referenceContent.Substring( splitIndex + 1 );
						}
						else
						{
							//resource name without '|'
							resourceName2 = referenceContent;
							path = "";
						}
					}

					var resourceName = "";
					{
						//!!!!slowly

						resourceName = PathUtility.Combine( fromDirectory, resourceName2 );

						if( resourceName2.Contains( ".." ) )
						{
							resourceName = VirtualPathUtility.GetVirtualPathByReal( Path.GetFullPath( VirtualPathUtility.GetRealPathByVirtual( resourceName ) ) );
						}
					}

					//get by class name
					{
						var type = GetTypeOfNetType( resourceName );
						if( type != null )
						{
							GetMemberProperty( expectedType, null, path, type, out outObject, out outMember );
							return;
						}
					}

					if( ResourceManager.GetByName( resourceName ) != null || VirtualFile.Exists( resourceName ) )
					{
						//!!!!threading что тут?
						//!!!!!!separate?
						var ins = ResourceManager.LoadResource( resourceName, true );
						if( ins != null )
						{
							var v = ins.ResultObject;
							if( v != null )
								GetMemberProperty( expectedType, v, path, null, out outObject, out outMember );
						}
					}
				} );

			//!!!!!какие еще виды ссылок? или куда?
		}

		public delegate void GetValueByReferenceOverrideDelegate( Type expectedType, object owner, ref string getByReference );//, ref bool handled, ref object result );
		public static event GetValueByReferenceOverrideDelegate GetValueByReferenceOverride;

		//!!!!!?
		//!!!!ожидаемый тип TypeInfo
		public static object GetValueByReference( Type expectedType, object owner, string getByReference )
		{
			if( string.IsNullOrEmpty( getByReference ) )
			{
				if( expectedType.IsValueType )
					return GetTypeOfNetType( expectedType ).InvokeInstance( null );
				else
					return null;
				//Log.Fatal( "MetadataManager: GetValueByReference: string.IsNullOrEmpty( getByReference )." );
			}

			//override behaviour
			{
				GetValueByReferenceOverride?.Invoke( expectedType, owner, ref getByReference );//, ref handled, ref value );

				//var handled = false;
				//object value = null;
				//GetValueByReferenceOverride?.Invoke( expectedType, owner, getByReference, ref handled, ref value );
				//if( handled )
				//{
				//	xx;//old code
				//	//default value types
				//	if( value == null && expectedType.IsValueType )
				//		value = Activator.CreateInstance( expectedType );
				//	//Auto convert types
				//	if( value != null && !expectedType.IsAssignableFrom( value.GetType() ) )
				//	{
				//		var newValue = AutoConvertValue( value, expectedType );
				//		if( newValue == null )
				//			newValue = AutoConvertValue( ReferenceUtils.GetUnreferencedValue( value ), expectedType );
				//		if( newValue != null )
				//			value = newValue;
				//	}

				//	return value;
				//}
			}

			//!!!!
			//specialization for ReferenceValueType_Resource
			if( typeof( ReferenceValueType_Resource ).IsAssignableFrom( expectedType ) )//if( expectedType == typeof( ReferenceValueType_Resource ) )
				return new ReferenceValueType_Resource( getByReference );

			//!!!!slowly
			//!!!!еще можно быстрее получать тип ссылки, т.к. их мало

			//!!!!проверки


			ReferenceType referenceType;
			//string referenceType;
			string referenceContent;
			{
				int splitIndex = getByReference.IndexOf( ':' );
				if( splitIndex != -1 )
				{
					var referenceType2 = getByReference.Substring( 0, splitIndex );

					var type2 = GetReferenceType( referenceType2 );
					if( type2 != null )
					{
						referenceType = type2;//referenceType2;
						referenceContent = getByReference.Substring( splitIndex + 1 );
					}
					else
					{
						referenceType = referenceTypeResource;
						//referenceType = "resource";
						referenceContent = getByReference;
					}
				}
				else
				{
					referenceType = referenceTypeResource;
					//referenceType = "resource";
					referenceContent = getByReference;
				}
			}

			////!!!!пока так
			////add reference type if not specified
			//{
			//	int splitIndex = getByReference.IndexOf( ':' );
			//	if( splitIndex != -1 )
			//	{
			//		var referenceType2 = getByReference.Substring( 0, splitIndex );

			//		var type2 = GetReferenceType( referenceType2 );
			//		if( type2 == null )
			//		{
			//			getByReference = "resource:" + getByReference;
			//		}
			//	}
			//	else
			//	{
			//		getByReference = "resource:" + getByReference;
			//	}
			//}

			//string referenceType;
			//string referenceContent;
			//{
			//	int splitIndex = getByReference.IndexOf( ':' );
			//	if( splitIndex != -1 )
			//	{
			//		referenceType = getByReference.Substring( 0, splitIndex );
			//		referenceContent = getByReference.Substring( splitIndex + 1 );
			//	}
			//	else
			//	{
			//		//!!!!!
			//		//'resource' type if no reference type specified
			//		referenceType = "resource";
			//		referenceContent = getByReference;
			//	}
			//}

			//never happen
			//if( string.IsNullOrEmpty( referenceType ) )
			//{
			//	if( expectedType.IsValueType )
			//		return GetTypeOfNetType( expectedType ).InvokeInstance( null );
			//	else
			//		return null;
			//	//Log.Fatal( "impl Invalid reference format: " + getByReference );
			//}

			var type = referenceType;// GetReferenceType( referenceType );

			//never happen
			//if( type == null )
			//{
			//	if( expectedType.IsValueType )
			//		return GetTypeOfNetType( expectedType ).InvokeInstance( null );
			//	else
			//		return null;
			//	//Log.Fatal( "impl Invalid reference format: Unknown reference type" + getByReference );
			//}

			{
				//!!!!out error
				object value;
				type.GetValueFunction( expectedType, owner, referenceContent, out value );

				//auto convert types
				if( value != null && !expectedType.IsAssignableFrom( value.GetType() ) )
				{
					var newValue = AutoConvertValue( value, expectedType );
					if( newValue == null )
						newValue = AutoConvertValue( ReferenceUtility.GetUnreferencedValue( value ), expectedType );
					value = newValue;
				}
				//default for value types
				if( value == null && expectedType.IsValueType )
					value = Activator.CreateInstance( expectedType );

				return value;
			}
		}

		//!!!!!?
		//!!!!ожидаемый тип TypeInfo
		public static void GetMemberByReference( Type expectedType, object owner, string getByReference, out object outObject, out Metadata.Member outMember )
		{
			if( string.IsNullOrEmpty( getByReference ) )
			{
				outObject = null;
				outMember = null;
				return;
				//Log.Fatal( "MetadataManager: GetMemberByReference: string.IsNullOrEmpty( getByReference )." );
			}

			//!!!!slowly
			//!!!!еще можно быстрее получать тип ссылки, т.к. их мало

			//!!!!проверки


			ReferenceType referenceType;
			//string referenceType;
			string referenceContent;
			{
				int splitIndex = getByReference.IndexOf( ':' );
				if( splitIndex != -1 )
				{
					var referenceType2 = getByReference.Substring( 0, splitIndex );

					var type2 = GetReferenceType( referenceType2 );
					if( type2 != null )
					{
						referenceType = type2;// referenceType2;
						referenceContent = getByReference.Substring( splitIndex + 1 );
					}
					else
					{
						referenceType = referenceTypeResource;
						//referenceType = "resource";
						referenceContent = getByReference;
					}
				}
				else
				{
					referenceType = referenceTypeResource;
					//referenceType = "resource";
					referenceContent = getByReference;
				}
			}

			////!!!!пока так
			////add reference type if not specified
			//{
			//	int splitIndex = getByReference.IndexOf( ':' );
			//	if( splitIndex != -1 )
			//	{
			//		var referenceType2 = getByReference.Substring( 0, splitIndex );

			//		var type2 = GetReferenceType( referenceType2 );
			//		if( type2 == null )
			//		{
			//			getByReference = "resource:" + getByReference;
			//		}
			//	}
			//	else
			//	{
			//		getByReference = "resource:" + getByReference;
			//	}
			//}

			//string referenceType;
			//string referenceContent;
			//{
			//	int splitIndex = getByReference.IndexOf( ':' );
			//	if( splitIndex != -1 )
			//	{
			//		referenceType = getByReference.Substring( 0, splitIndex );
			//		referenceContent = getByReference.Substring( splitIndex + 1 );
			//	}
			//	else
			//	{
			//		//!!!!
			//		Log.Fatal( "ff" );

			//		//!!!!!
			//		//'resource' type if no reference type specified
			//		referenceType = "resource";
			//		referenceContent = getByReference;
			//	}
			//}

			//never happen
			//if( string.IsNullOrEmpty( referenceType ) )
			//{
			//	outObject = null;
			//	outMember = null;
			//	return;
			//	//Log.Fatal( "impl Invalid reference format: " + getByReference );
			//}

			var type = referenceType;// GetReferenceType( referenceType );

			//never happen
			//if( type == null )
			//{
			//	outObject = null;
			//	outMember = null;
			//	return;
			//	//Log.Fatal( "impl Invalid reference format: Unknown reference type" + getByReference );
			//}

			//!!!!out error
			type.GetMemberFunction( expectedType, owner, referenceContent, out outObject, out outMember );

			//xx;old code
			////default value types
			//if( value == null && expectedType.IsValueType )
			//	value = Activator.CreateInstance( expectedType );
			////Auto convert types
			//if( value != null && !expectedType.IsAssignableFrom( value.GetType() ) )
			//{
			//	var newValue = AutoConvertValue( value, expectedType );
			//	if( newValue == null )
			//	{
			//		//!!!!temp
			//		Log.Fatal( "TEMP. Unable to auto convert from {0} to {1}.", value.GetType(), expectedType );
			//	}
			//	value = newValue;
			//}

			//return value;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void RegisterSerializableType( Type type, bool oneStringSerialization, SerializableTypeItem.LoadDelegate load,
			SerializableTypeItem.SaveDelegate save, SerializableTypeItem.CloneDelegate clone )
		{
			lock( lockObject )
			{
				if( serializableTypes.ContainsKey( type ) )
					return;
				SerializableTypeItem item = new SerializableTypeItem();
				item.type = type;
				item.oneStringSerialization = oneStringSerialization;
				item.load = load;
				item.save = save;
				item.clone = clone;
				serializableTypes[ type ] = item;
			}
		}

		public static SerializableTypeItem[] GetSerializableTypes()
		{
			lock( lockObject )
			{
				SerializableTypeItem[] array = new SerializableTypeItem[ serializableTypes.Count ];
				serializableTypes.Values.CopyTo( array, 0 );
				return array;
			}
		}

		public static SerializableTypeItem GetSuitableSerializableType( Type type, out bool fullySerialized )
		{
			lock( lockObject )
			{
				fullySerialized = false;

				if( type.IsEnum )
				{
					fullySerialized = true;
					return serializableTypes_Enum;
				}
				if( type.IsArray )
					return serializableTypes_Array;

				Type t = type;
				if( type.IsGenericType )
					t = type.GetGenericTypeDefinition();

				bool upLevel = true;

				do
				{
					SerializableTypeItem item;
					if( serializableTypes.TryGetValue( t, out item ) )
					{
						if( upLevel )
							fullySerialized = true;
						return item;
					}
					t = t.BaseType;

					//!!!!new
					if( t != null && t.IsGenericType )
						t = t.GetGenericTypeDefinition();

					upLevel = false;

				} while( t != null );

				return null;
			}
		}

		public static void RegisterAutoConvertItem( Type from, Type to, AutoConvertTypeItem.ConvertDelegate convertFunction )
		{
			if( from == to )
				return;

			lock( lockObject )
			{
				var item = new AutoConvertTypeItem( convertFunction );
				autoConvertTypes[ (from, to) ] = item;
			}
		}

		public static void RegisterAutoConvertItem( Type from, Type to, MethodInfo convertMethod )
		{
			lock( lockObject )
			{
				var item = new AutoConvertTypeItem( convertMethod );
				autoConvertTypes[ (from, to) ] = item;
			}
		}

		public static void RegisterAutoConvertItem( Type from, Type to, ConstructorInfo convertConstructor )
		{
			lock( lockObject )
			{
				var item = new AutoConvertTypeItem( convertConstructor );
				autoConvertTypes[ (from, to) ] = item;
			}
		}

		static void RegisterStandardAutoConvertItems()
		{
			//!!!!двойное конвертирование? т.е. через что-то. видать, так нельзя, а то всё через строки можно будет.

			//sbyte
			RegisterAutoConvertItem( typeof( sbyte ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(sbyte)value; } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( byte ), delegate ( object value ) { return (byte)(sbyte)value; } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( char ), delegate ( object value ) { return (char)(sbyte)value; } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( short ), delegate ( object value ) { return (short)(sbyte)value; } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( ushort ), delegate ( object value ) { return (ushort)(sbyte)value; } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( int ), delegate ( object value ) { return (int)(sbyte)value; } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( uint ), delegate ( object value ) { return (uint)(sbyte)value; } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( long ), delegate ( object value ) { return (long)(sbyte)value; } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( ulong ), delegate ( object value ) { return (ulong)(sbyte)value; } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( float ), delegate ( object value ) { return (float)(sbyte)value; } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( double ), delegate ( object value ) { return (double)(sbyte)value; } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( decimal ), delegate ( object value ) { return (double)(sbyte)value; } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( sbyte ), delegate ( object value ) { return sbyte.Parse( (string)value ); } );
			RegisterAutoConvertItem( typeof( sbyte ), typeof( bool ), delegate ( object value ) { return ( (sbyte)value ) != 0; } );


			//byte
			RegisterAutoConvertItem( typeof( byte ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(byte)value; } );
			RegisterAutoConvertItem( typeof( byte ), typeof( byte ), delegate ( object value ) { return (byte)(byte)value; } );
			RegisterAutoConvertItem( typeof( byte ), typeof( char ), delegate ( object value ) { return (char)(byte)value; } );
			RegisterAutoConvertItem( typeof( byte ), typeof( short ), delegate ( object value ) { return (short)(byte)value; } );
			RegisterAutoConvertItem( typeof( byte ), typeof( ushort ), delegate ( object value ) { return (ushort)(byte)value; } );
			RegisterAutoConvertItem( typeof( byte ), typeof( int ), delegate ( object value ) { return (int)(byte)value; } );
			RegisterAutoConvertItem( typeof( byte ), typeof( uint ), delegate ( object value ) { return (uint)(byte)value; } );
			RegisterAutoConvertItem( typeof( byte ), typeof( long ), delegate ( object value ) { return (long)(byte)value; } );
			RegisterAutoConvertItem( typeof( byte ), typeof( ulong ), delegate ( object value ) { return (ulong)(byte)value; } );
			RegisterAutoConvertItem( typeof( byte ), typeof( float ), delegate ( object value ) { return (float)(byte)value; } );
			RegisterAutoConvertItem( typeof( byte ), typeof( double ), delegate ( object value ) { return (double)(byte)value; } );
			RegisterAutoConvertItem( typeof( byte ), typeof( decimal ), delegate ( object value ) { return (double)(byte)value; } );
			RegisterAutoConvertItem( typeof( byte ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( byte ), delegate ( object value ) { return byte.Parse( (string)value ); } );
			RegisterAutoConvertItem( typeof( byte ), typeof( bool ), delegate ( object value ) { return ( (byte)value ) != 0; } );

			//char
			RegisterAutoConvertItem( typeof( char ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(char)value; } );
			RegisterAutoConvertItem( typeof( char ), typeof( byte ), delegate ( object value ) { return (byte)(char)value; } );
			RegisterAutoConvertItem( typeof( char ), typeof( char ), delegate ( object value ) { return (char)(char)value; } );
			RegisterAutoConvertItem( typeof( char ), typeof( short ), delegate ( object value ) { return (short)(char)value; } );
			RegisterAutoConvertItem( typeof( char ), typeof( ushort ), delegate ( object value ) { return (ushort)(char)value; } );
			RegisterAutoConvertItem( typeof( char ), typeof( int ), delegate ( object value ) { return (int)(char)value; } );
			RegisterAutoConvertItem( typeof( char ), typeof( uint ), delegate ( object value ) { return (uint)(char)value; } );
			RegisterAutoConvertItem( typeof( char ), typeof( long ), delegate ( object value ) { return (long)(char)value; } );
			RegisterAutoConvertItem( typeof( char ), typeof( ulong ), delegate ( object value ) { return (ulong)(char)value; } );
			RegisterAutoConvertItem( typeof( char ), typeof( float ), delegate ( object value ) { return (float)(char)value; } );
			RegisterAutoConvertItem( typeof( char ), typeof( double ), delegate ( object value ) { return (double)(char)value; } );
			RegisterAutoConvertItem( typeof( char ), typeof( decimal ), delegate ( object value ) { return (double)(char)value; } );
			RegisterAutoConvertItem( typeof( char ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( char ), delegate ( object value ) { return char.Parse( (string)value ); } );
			RegisterAutoConvertItem( typeof( char ), typeof( bool ), delegate ( object value ) { return ( (char)value ) != 0; } );

			//short
			RegisterAutoConvertItem( typeof( short ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(short)value; } );
			RegisterAutoConvertItem( typeof( short ), typeof( byte ), delegate ( object value ) { return (byte)(short)value; } );
			RegisterAutoConvertItem( typeof( short ), typeof( char ), delegate ( object value ) { return (char)(short)value; } );
			RegisterAutoConvertItem( typeof( short ), typeof( short ), delegate ( object value ) { return (short)(short)value; } );
			RegisterAutoConvertItem( typeof( short ), typeof( ushort ), delegate ( object value ) { return (ushort)(short)value; } );
			RegisterAutoConvertItem( typeof( short ), typeof( int ), delegate ( object value ) { return (int)(short)value; } );
			RegisterAutoConvertItem( typeof( short ), typeof( uint ), delegate ( object value ) { return (uint)(short)value; } );
			RegisterAutoConvertItem( typeof( short ), typeof( long ), delegate ( object value ) { return (long)(short)value; } );
			RegisterAutoConvertItem( typeof( short ), typeof( ulong ), delegate ( object value ) { return (ulong)(short)value; } );
			RegisterAutoConvertItem( typeof( short ), typeof( float ), delegate ( object value ) { return (float)(short)value; } );
			RegisterAutoConvertItem( typeof( short ), typeof( double ), delegate ( object value ) { return (double)(short)value; } );
			RegisterAutoConvertItem( typeof( short ), typeof( decimal ), delegate ( object value ) { return (double)(short)value; } );
			RegisterAutoConvertItem( typeof( short ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( short ), delegate ( object value ) { return short.Parse( (string)value ); } );
			RegisterAutoConvertItem( typeof( short ), typeof( bool ), delegate ( object value ) { return ( (short)value ) != 0; } );

			//ushort
			RegisterAutoConvertItem( typeof( ushort ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(ushort)value; } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( byte ), delegate ( object value ) { return (byte)(ushort)value; } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( char ), delegate ( object value ) { return (char)(ushort)value; } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( short ), delegate ( object value ) { return (short)(ushort)value; } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( ushort ), delegate ( object value ) { return (ushort)(ushort)value; } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( int ), delegate ( object value ) { return (int)(ushort)value; } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( uint ), delegate ( object value ) { return (uint)(ushort)value; } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( long ), delegate ( object value ) { return (long)(ushort)value; } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( ulong ), delegate ( object value ) { return (ulong)(ushort)value; } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( float ), delegate ( object value ) { return (float)(ushort)value; } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( double ), delegate ( object value ) { return (double)(ushort)value; } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( decimal ), delegate ( object value ) { return (double)(ushort)value; } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( ushort ), delegate ( object value ) { return ushort.Parse( (string)value ); } );
			RegisterAutoConvertItem( typeof( ushort ), typeof( bool ), delegate ( object value ) { return ( (ushort)value ) != 0; } );

			//int
			RegisterAutoConvertItem( typeof( int ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(int)value; } );
			RegisterAutoConvertItem( typeof( int ), typeof( byte ), delegate ( object value ) { return (byte)(int)value; } );
			RegisterAutoConvertItem( typeof( int ), typeof( char ), delegate ( object value ) { return (char)(int)value; } );
			RegisterAutoConvertItem( typeof( int ), typeof( short ), delegate ( object value ) { return (short)(int)value; } );
			RegisterAutoConvertItem( typeof( int ), typeof( ushort ), delegate ( object value ) { return (ushort)(int)value; } );
			RegisterAutoConvertItem( typeof( int ), typeof( int ), delegate ( object value ) { return (int)(int)value; } );
			RegisterAutoConvertItem( typeof( int ), typeof( uint ), delegate ( object value ) { return (uint)(int)value; } );
			RegisterAutoConvertItem( typeof( int ), typeof( long ), delegate ( object value ) { return (long)(int)value; } );
			RegisterAutoConvertItem( typeof( int ), typeof( ulong ), delegate ( object value ) { return (ulong)(int)value; } );
			RegisterAutoConvertItem( typeof( int ), typeof( float ), delegate ( object value ) { return (float)(int)value; } );
			RegisterAutoConvertItem( typeof( int ), typeof( double ), delegate ( object value ) { return (double)(int)value; } );
			RegisterAutoConvertItem( typeof( int ), typeof( decimal ), delegate ( object value ) { return (double)(int)value; } );
			RegisterAutoConvertItem( typeof( int ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( int ), delegate ( object value ) { return int.Parse( (string)value ); } );
			RegisterAutoConvertItem( typeof( int ), typeof( bool ), delegate ( object value ) { return ( (int)value ) != 0; } );

			//uint
			RegisterAutoConvertItem( typeof( uint ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(uint)value; } );
			RegisterAutoConvertItem( typeof( uint ), typeof( byte ), delegate ( object value ) { return (byte)(uint)value; } );
			RegisterAutoConvertItem( typeof( uint ), typeof( char ), delegate ( object value ) { return (char)(uint)value; } );
			RegisterAutoConvertItem( typeof( uint ), typeof( short ), delegate ( object value ) { return (short)(uint)value; } );
			RegisterAutoConvertItem( typeof( uint ), typeof( ushort ), delegate ( object value ) { return (ushort)(uint)value; } );
			RegisterAutoConvertItem( typeof( uint ), typeof( int ), delegate ( object value ) { return (int)(uint)value; } );
			RegisterAutoConvertItem( typeof( uint ), typeof( uint ), delegate ( object value ) { return (uint)(uint)value; } );
			RegisterAutoConvertItem( typeof( uint ), typeof( long ), delegate ( object value ) { return (long)(uint)value; } );
			RegisterAutoConvertItem( typeof( uint ), typeof( ulong ), delegate ( object value ) { return (ulong)(uint)value; } );
			RegisterAutoConvertItem( typeof( uint ), typeof( float ), delegate ( object value ) { return (float)(uint)value; } );
			RegisterAutoConvertItem( typeof( uint ), typeof( double ), delegate ( object value ) { return (double)(uint)value; } );
			RegisterAutoConvertItem( typeof( uint ), typeof( decimal ), delegate ( object value ) { return (double)(uint)value; } );
			RegisterAutoConvertItem( typeof( uint ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( uint ), delegate ( object value ) { return uint.Parse( (string)value ); } );
			RegisterAutoConvertItem( typeof( uint ), typeof( bool ), delegate ( object value ) { return ( (uint)value ) != 0; } );

			//long
			RegisterAutoConvertItem( typeof( long ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(long)value; } );
			RegisterAutoConvertItem( typeof( long ), typeof( byte ), delegate ( object value ) { return (byte)(long)value; } );
			RegisterAutoConvertItem( typeof( long ), typeof( char ), delegate ( object value ) { return (char)(long)value; } );
			RegisterAutoConvertItem( typeof( long ), typeof( short ), delegate ( object value ) { return (short)(long)value; } );
			RegisterAutoConvertItem( typeof( long ), typeof( ushort ), delegate ( object value ) { return (ushort)(long)value; } );
			RegisterAutoConvertItem( typeof( long ), typeof( int ), delegate ( object value ) { return (int)(long)value; } );
			RegisterAutoConvertItem( typeof( long ), typeof( uint ), delegate ( object value ) { return (uint)(long)value; } );
			RegisterAutoConvertItem( typeof( long ), typeof( long ), delegate ( object value ) { return (long)(long)value; } );
			RegisterAutoConvertItem( typeof( long ), typeof( ulong ), delegate ( object value ) { return (ulong)(long)value; } );
			RegisterAutoConvertItem( typeof( long ), typeof( float ), delegate ( object value ) { return (float)(long)value; } );
			RegisterAutoConvertItem( typeof( long ), typeof( double ), delegate ( object value ) { return (double)(long)value; } );
			RegisterAutoConvertItem( typeof( long ), typeof( decimal ), delegate ( object value ) { return (double)(long)value; } );
			RegisterAutoConvertItem( typeof( long ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( long ), delegate ( object value ) { return long.Parse( (string)value ); } );
			RegisterAutoConvertItem( typeof( long ), typeof( bool ), delegate ( object value ) { return ( (long)value ) != 0; } );

			//ulong
			RegisterAutoConvertItem( typeof( ulong ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(ulong)value; } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( byte ), delegate ( object value ) { return (byte)(ulong)value; } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( char ), delegate ( object value ) { return (char)(ulong)value; } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( short ), delegate ( object value ) { return (short)(ulong)value; } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( ushort ), delegate ( object value ) { return (ushort)(ulong)value; } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( int ), delegate ( object value ) { return (int)(ulong)value; } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( uint ), delegate ( object value ) { return (uint)(ulong)value; } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( long ), delegate ( object value ) { return (long)(ulong)value; } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( ulong ), delegate ( object value ) { return (ulong)(ulong)value; } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( float ), delegate ( object value ) { return (float)(ulong)value; } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( double ), delegate ( object value ) { return (double)(ulong)value; } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( decimal ), delegate ( object value ) { return (double)(ulong)value; } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( ulong ), delegate ( object value ) { return ulong.Parse( (string)value ); } );
			RegisterAutoConvertItem( typeof( ulong ), typeof( bool ), delegate ( object value ) { return ( (ulong)value ) != 0; } );

			//float
			RegisterAutoConvertItem( typeof( float ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(float)value; } );
			RegisterAutoConvertItem( typeof( float ), typeof( byte ), delegate ( object value ) { return (byte)(float)value; } );
			RegisterAutoConvertItem( typeof( float ), typeof( char ), delegate ( object value ) { return (char)(float)value; } );
			RegisterAutoConvertItem( typeof( float ), typeof( short ), delegate ( object value ) { return (short)(float)value; } );
			RegisterAutoConvertItem( typeof( float ), typeof( ushort ), delegate ( object value ) { return (ushort)(float)value; } );
			RegisterAutoConvertItem( typeof( float ), typeof( int ), delegate ( object value ) { return (int)(float)value; } );
			RegisterAutoConvertItem( typeof( float ), typeof( uint ), delegate ( object value ) { return (uint)(float)value; } );
			RegisterAutoConvertItem( typeof( float ), typeof( long ), delegate ( object value ) { return (long)(float)value; } );
			RegisterAutoConvertItem( typeof( float ), typeof( ulong ), delegate ( object value ) { return (ulong)(float)value; } );
			RegisterAutoConvertItem( typeof( float ), typeof( float ), delegate ( object value ) { return (float)(float)value; } );
			RegisterAutoConvertItem( typeof( float ), typeof( double ), delegate ( object value ) { return (double)(float)value; } );
			RegisterAutoConvertItem( typeof( float ), typeof( decimal ), delegate ( object value ) { return (double)(float)value; } );
			RegisterAutoConvertItem( typeof( float ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( float ), delegate ( object value ) { return float.Parse( (string)value ); } );
			RegisterAutoConvertItem( typeof( float ), typeof( bool ), delegate ( object value ) { return ( (float)value ) != 0; } );

			//double
			RegisterAutoConvertItem( typeof( double ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(double)value; } );
			RegisterAutoConvertItem( typeof( double ), typeof( byte ), delegate ( object value ) { return (byte)(double)value; } );
			RegisterAutoConvertItem( typeof( double ), typeof( char ), delegate ( object value ) { return (char)(double)value; } );
			RegisterAutoConvertItem( typeof( double ), typeof( short ), delegate ( object value ) { return (short)(double)value; } );
			RegisterAutoConvertItem( typeof( double ), typeof( ushort ), delegate ( object value ) { return (ushort)(double)value; } );
			RegisterAutoConvertItem( typeof( double ), typeof( int ), delegate ( object value ) { return (int)(double)value; } );
			RegisterAutoConvertItem( typeof( double ), typeof( uint ), delegate ( object value ) { return (uint)(double)value; } );
			RegisterAutoConvertItem( typeof( double ), typeof( long ), delegate ( object value ) { return (long)(double)value; } );
			RegisterAutoConvertItem( typeof( double ), typeof( ulong ), delegate ( object value ) { return (ulong)(double)value; } );
			RegisterAutoConvertItem( typeof( double ), typeof( float ), delegate ( object value ) { return (float)(double)value; } );
			RegisterAutoConvertItem( typeof( double ), typeof( double ), delegate ( object value ) { return (double)(double)value; } );
			RegisterAutoConvertItem( typeof( double ), typeof( decimal ), delegate ( object value ) { return (double)(double)value; } );
			RegisterAutoConvertItem( typeof( double ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( double ), delegate ( object value ) { return double.Parse( (string)value ); } );
			RegisterAutoConvertItem( typeof( double ), typeof( bool ), delegate ( object value ) { return ( (double)value ) != 0; } );

			//decimal
			RegisterAutoConvertItem( typeof( decimal ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(decimal)value; } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( byte ), delegate ( object value ) { return (byte)(decimal)value; } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( char ), delegate ( object value ) { return (char)(decimal)value; } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( short ), delegate ( object value ) { return (short)(decimal)value; } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( ushort ), delegate ( object value ) { return (ushort)(decimal)value; } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( int ), delegate ( object value ) { return (int)(decimal)value; } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( uint ), delegate ( object value ) { return (uint)(decimal)value; } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( long ), delegate ( object value ) { return (long)(decimal)value; } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( ulong ), delegate ( object value ) { return (ulong)(decimal)value; } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( float ), delegate ( object value ) { return (float)(decimal)value; } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( double ), delegate ( object value ) { return (double)(decimal)value; } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( decimal ), delegate ( object value ) { return (double)(decimal)value; } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( decimal ), delegate ( object value ) { return decimal.Parse( (string)value ); } );
			RegisterAutoConvertItem( typeof( decimal ), typeof( bool ), delegate ( object value ) { return ( (decimal)value ) != 0; } );

			////xxx
			//RegisterAutoConvertItem( typeof( xxx ), typeof( sbyte ), delegate ( object value ) { return (sbyte)(xxx)value; } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( byte ), delegate ( object value ) { return (byte)(xxx)value; } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( char ), delegate ( object value ) { return (char)(xxx)value; } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( short ), delegate ( object value ) { return (short)(xxx)value; } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( ushort ), delegate ( object value ) { return (ushort)(xxx)value; } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( int ), delegate ( object value ) { return (int)(xxx)value; } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( uint ), delegate ( object value ) { return (uint)(xxx)value; } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( long ), delegate ( object value ) { return (long)(xxx)value; } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( ulong ), delegate ( object value ) { return (ulong)(xxx)value; } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( float ), delegate ( object value ) { return (float)(xxx)value; } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( double ), delegate ( object value ) { return (double)(xxx)value; } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( decimal ), delegate ( object value ) { return (double)(xxx)value; } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			//RegisterAutoConvertItem( typeof( string ), typeof( xxx ), delegate ( object value ) { return xxx.Parse( (string)value ); } );
			//RegisterAutoConvertItem( typeof( xxx ), typeof( bool ), delegate ( object value ) { return ( (xxx)value ) != 0; } );

			//bool
			RegisterAutoConvertItem( typeof( bool ), typeof( sbyte ), delegate ( object value ) { return (sbyte)( ( (bool)value ) ? 1 : 0 ); } );
			RegisterAutoConvertItem( typeof( bool ), typeof( byte ), delegate ( object value ) { return (byte)( ( (bool)value ) ? 1 : 0 ); } );
			RegisterAutoConvertItem( typeof( bool ), typeof( char ), delegate ( object value ) { return (char)( ( (bool)value ) ? 1 : 0 ); } );
			RegisterAutoConvertItem( typeof( bool ), typeof( short ), delegate ( object value ) { return (short)( ( (bool)value ) ? 1 : 0 ); } );
			RegisterAutoConvertItem( typeof( bool ), typeof( ushort ), delegate ( object value ) { return (ushort)( ( (bool)value ) ? 1 : 0 ); } );
			RegisterAutoConvertItem( typeof( bool ), typeof( int ), delegate ( object value ) { return (int)( ( (bool)value ) ? 1 : 0 ); } );
			RegisterAutoConvertItem( typeof( bool ), typeof( uint ), delegate ( object value ) { return (uint)( ( (bool)value ) ? 1 : 0 ); } );
			RegisterAutoConvertItem( typeof( bool ), typeof( long ), delegate ( object value ) { return (long)( ( (bool)value ) ? 1 : 0 ); } );
			RegisterAutoConvertItem( typeof( bool ), typeof( ulong ), delegate ( object value ) { return (ulong)( ( (bool)value ) ? 1 : 0 ); } );
			RegisterAutoConvertItem( typeof( bool ), typeof( float ), delegate ( object value ) { return (float)( ( (bool)value ) ? 1 : 0 ); } );
			RegisterAutoConvertItem( typeof( bool ), typeof( double ), delegate ( object value ) { return (double)( ( (bool)value ) ? 1 : 0 ); } );
			RegisterAutoConvertItem( typeof( bool ), typeof( decimal ), delegate ( object value ) { return (double)( ( (bool)value ) ? 1 : 0 ); } );
			RegisterAutoConvertItem( typeof( bool ), typeof( string ), delegate ( object value ) { return value.ToString(); } );
			RegisterAutoConvertItem( typeof( string ), typeof( bool ), delegate ( object value ) { return bool.Parse( (string)value ); } );
		}

		public static void RegisterAutoConvertItemsForAssembly( Assembly assembly, Type[] types = null )
		{
			if( types == null )
				types = assembly.GetExportedTypes();

			foreach( var type in types )//assembly.GetExportedTypes() )
			{
				//methods
				foreach( var method in type.GetMethods() )
				{
					//optimization
					if( method.ReturnType == typeof( void ) )
						continue;

					bool exists = false;
					foreach( var attr in method.GetCustomAttributes<AutoConvertTypeAttribute>() )
					{
						exists = true;
						break;
					}

					if( exists )
					{
						//!!!!checks

						if( method.IsStatic )
							RegisterAutoConvertItem( method.GetParameters()[ 0 ].ParameterType, method.ReturnType, method );
						else
							RegisterAutoConvertItem( type, method.ReturnType, method );
					}
				}

				//constructors
				foreach( var constructor in type.GetConstructors() )
				{
					bool exists = false;
					foreach( var attr in constructor.GetCustomAttributes<AutoConvertTypeAttribute>() )
					{
						exists = true;
						break;
					}

					if( exists )
					{
						//!!!!checks

						RegisterAutoConvertItem( constructor.GetParameters()[ 0 ].ParameterType, type, constructor );
					}
				}
			}
		}

		public static bool CanAutoConvertType( Type from, Type to )
		{
			lock( lockObject )
			{
				return autoConvertTypes.ContainsKey( (from, to) );
			}
		}

		public static object AutoConvertValue( object value, Type destinationType )
		{
			lock( lockObject )//!!!!slowly?
			{
				if( autoConvertTypes.TryGetValue( (value.GetType(), destinationType), out AutoConvertTypeItem item ) )
				{
					if( item.ConvertFunction != null )
						return item.ConvertFunction( value );
					else if( item.ConvertMethod != null )
					{
						var method = item.ConvertMethod;
						if( method.IsStatic )
							return method.Invoke( null, new object[] { value } );
						else
							return method.Invoke( value, new object[ 0 ] );
					}
					else
						return item.ConvertConstructor.Invoke( new object[] { value } );
				}
				else
					return null;
			}
		}

		//public static object ConvertDefaultValueToDemandType( Type demandedType, object value )
		//{
		//	if( value == null )
		//		return null;

		//	Type unrefDemandedType = ReferenceUtils.GetUnreferencedType( demandedType );

		//	//convert from string
		//	if( value is string && unrefDemandedType != typeof( string ) )
		//	{
		//		var item = SimpleTypesUtils.GetTypeItem( unrefDemandedType );
		//		if( item != null )
		//			return item.ParseFunction( (string)value );
		//		//else
		//		//{
		//		//	//!!!!
		//		//	Log.Fatal( "impl: ConvertDefaultValueToDemandType 1" );
		//		//}
		//	}

		//	//!!!!можно было бы еще конвертить в похожий тип. например float в double

		//	//!!!!
		//	//if( demandedType != value.GetType() )
		//	//{
		//	//	//!!!!
		//	//	Log.Fatal( "impl: ConvertDefaultValueToDemandType 2" );
		//	//}

		//	return value;
		//}

		public static void RegisterExtensions( Assembly assembly, Type[] types = null )
		{
			if( types == null )
				types = assembly.GetExportedTypes();

			foreach( var type in types )
			{
				if( !type.IsAbstract && typeof( MetadataExtensions ).IsAssignableFrom( type ) )
				{
					var constructor = type.GetConstructor( new Type[ 0 ] );
					MetadataExtensions obj = (MetadataExtensions)constructor.Invoke( new object[ 0 ] );
					obj.Register();
				}
			}
		}
	}
}
