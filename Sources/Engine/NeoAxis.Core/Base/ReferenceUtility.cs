// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Helper class for working with references.
	/// </summary>
	public static class ReferenceUtility
	{
		//!!!!смысла нет, хотя может надо будет
		//static TypeOrItsMembersCanReferencedToComponentTypeData typeOrItsMembersCanReferencedToComponentTypeData = new TypeOrItsMembersCanReferencedToComponentTypeData();

		//

		public static bool IsReferenceType( Type type )
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Reference<> );
		}

		//!!!!new: не надо, IReference
		//public static bool IsReference( object obj )
		//{
		//	return IsReferenceType( obj.GetType() );
		//	//var t = reference.GetType();
		//	//return t.IsGenericType && t.GetGenericTypeDefinition() == typeof( Reference<> );
		//}

		//!!!!new
		// If the type provided is not a Reference Type, return null.
		// Otherwise, returns the underlying type of the Reference type
		public static Type GetUnderlyingType( Type referenceType )
		{
			if( referenceType == null )
				throw new ArgumentNullException( "referenceType" );
			//!!!!not approved, need sleep
			//if( referenceType == null )
			//	throw new ArgumentNullException( "It should be reference type!", nameof( referenceType ) );

			//!!!!not approved, need sleep
			//if( !IsReferenceType( referenceType ) )
			//	throw new ArgumentException( "It should be reference type!", nameof( referenceType ) );

			Type result = null;
			if( referenceType.IsGenericType && !referenceType.IsGenericTypeDefinition )
			{
				// instantiated generic type only                
				Type genericType = referenceType.GetGenericTypeDefinition();
				if( object.ReferenceEquals( genericType, typeof( Reference<> ) ) )
					result = referenceType.GetGenericArguments()[ 0 ];
			}
			return result;
		}

		//!!!!new. оставить оба GetUnderlyingType и GetUnreferencedType?
		public static Type GetUnreferencedType( Type type )
		{
			if( IsReferenceType( type ) )
				return GetUnderlyingType( type );
			return type;
		}

		//!!!!new
		public static Metadata.TypeInfo GetUnreferencedType( Metadata.TypeInfo type )
		{
			var netType = type.GetNetType();
			if( IsReferenceType( netType ) )
				return MetadataManager.GetTypeOfNetType( GetUnderlyingType( netType ) );
			return type;
		}

		public static object GetUnreferencedValue( object value )
		{
			var iReference = value as IReference;
			if( iReference != null )
				return iReference.ValueAsObject;
			return value;
		}

		public static IReference MakeReference( Type underlyingType, object value, string getByReference )
		{
			//!!!!new
			if( value == null && underlyingType.IsValueType )
				value = Activator.CreateInstance( underlyingType );

			if( typeof( IReference ).IsAssignableFrom( underlyingType ) )
				return (IReference)Activator.CreateInstance( underlyingType, value, getByReference );
			else
			{
				var refType = typeof( Reference<> ).MakeGenericType( new[] { underlyingType } );
				return (IReference)Activator.CreateInstance( refType, value, getByReference );
			}
		}

		public static IReference MakeReference( Type underlyingType, string getByReference )
		{
			return MakeReference( underlyingType, null, getByReference );
		}

		//public static IReference MakeReference( Metadata.TypeInfo underlyingType, object value, string getByReference )
		//{
		//	return MakeReference( underlyingType.GetNetType(), value, getByReference );
		//}

		//public static IReference MakeReference( Metadata.TypeInfo underlyingType, string getByReference )
		//{
		//	return MakeReference( underlyingType.GetNetType(), null, getByReference );
		//}

		public static Reference<T> MakeReference<T>( T value, string getByReference )
		{
			var underlyingType = typeof( T );

			//!!!!new
			if( value == null && underlyingType.IsValueType )
				value = (T)Activator.CreateInstance( underlyingType );

			if( typeof( IReference ).IsAssignableFrom( underlyingType ) )
				return (Reference<T>)Activator.CreateInstance( underlyingType, value, getByReference );
			else
			{
				var refType = typeof( Reference<> ).MakeGenericType( new[] { underlyingType } );
				return (Reference<T>)Activator.CreateInstance( refType, value, getByReference );
			}
		}

		public static ReferenceNoValue MakeReference( string getByReference )
		{
			return new ReferenceNoValue( getByReference );
		}

		public static Reference<T> MakeReference<T>( string getByReference )
		{
			return new Reference<T>( default, getByReference );
		}

		public static void CalculateThisReference( Component from, Component to, string memberNameOrPath, out string referenceValue, out char addSeparator )
		{
			//!!!!slowly

			var result = new StringBuilder();
			result.Append( "this:" );
			addSeparator = '\0';

			if( from != to )
			{
				List<Component> pathTo = new List<Component>();
				var c = to;
				do
				{
					pathTo.Add( c );
					c = c.Parent;
				} while( c != null );
				pathTo.Reverse();

				int endIndex;

				//go up
				c = from;
				do
				{
					endIndex = pathTo.IndexOf( c );
					if( endIndex != -1 )
						break;

					if( addSeparator != '\0' )
						result.Append( addSeparator );
					//if( result.Length != 0 )
					//	result.Append( '\\' );
					result.Append( ".." );
					addSeparator = '\\';

					c = c.Parent;
				} while( c != null );

				if( endIndex == -1 )
				{
					//!!!!error?
					referenceValue = "this:";
					addSeparator = '\0';
					return;
					//return "this:";
				}

				//go down
				for( int n = endIndex + 1; n < pathTo.Count; n++ )
				{
					c = pathTo[ n ];

					if( addSeparator != '\0' )
						result.Append( addSeparator );
					result.Append( c.GetPathFromParent() );
					addSeparator = '\\';
				}
			}

			//add member
			if( !string.IsNullOrEmpty( memberNameOrPath ) )
			{
				if( addSeparator != '\0' )
					result.Append( addSeparator );
				//if( result.Length != 0 )
				//	result.Append( "\\" );
				result.Append( memberNameOrPath );
				addSeparator = '\\';
			}

			referenceValue = result.ToString();
		}

		public static string CalculateThisReference( Component from, Component to, string memberNameOrPath = "" )
		{
			CalculateThisReference( from, to, memberNameOrPath, out string referenceValue, out char addSeparator );
			return referenceValue;
		}

		public static ReferenceNoValue MakeThisReference( Component from, Component to, string memberNameOrPath = "" )
		{
			return new ReferenceNoValue( CalculateThisReference( from, to, memberNameOrPath ) );
		}

		public static Reference<T> MakeThisReference<T>( Component from, Component to, string memberNameOrPath = "" )
		{
			return new Reference<T>( default, CalculateThisReference( from, to, memberNameOrPath ) );
		}

		public static void CalculateRootReference( Component to, string memberNameOrPath, out string referenceValue, out char addSeparator )
		{
			var result = new StringBuilder();
			result.Append( "root:" );
			addSeparator = '\0';

			//path
			var namePath = to.GetPathFromRoot();
			if( namePath != "" )
			{
				if( addSeparator != '\0' )
					result.Append( addSeparator );
				result.Append( namePath );
				addSeparator = '\\';
			}

			//add member
			if( !string.IsNullOrEmpty( memberNameOrPath ) )
			{
				if( addSeparator != '\0' )
					result.Append( addSeparator );
				result.Append( memberNameOrPath );
				addSeparator = '\\';
			}

			referenceValue = result.ToString();
		}

		public static string CalculateRootReference( Component to, string memberNameOrPath = "" )
		{
			CalculateRootReference( to, memberNameOrPath, out string referenceValue, out char addSeparator );
			return referenceValue;
		}

		public static ReferenceNoValue MakeRootReference( Component to, string memberNameOrPath = "" )
		{
			return new ReferenceNoValue( CalculateRootReference( to, memberNameOrPath ) );
		}

		public static Reference<T> MakeRootReference<T>( Component to, string memberNameOrPath = "" )
		{
			return new Reference<T>( default, CalculateRootReference( to, memberNameOrPath ) );
		}

		public static void CalculateResourceReference( Component to, string memberNameOrPath, out string referenceValue, out char addSeparator )
		{
			var resourceInstance = to.ParentRoot?.HierarchyController.CreatedByResource;
			if( resourceInstance != null )
			{
				var result = new StringBuilder();
				result.Append( resourceInstance.Owner.Name );
				addSeparator = '|';

				//path
				var namePath = to.GetPathFromRoot();
				if( namePath != "" )
				{
					if( addSeparator != '\0' )
						result.Append( addSeparator );
					result.Append( namePath );
					addSeparator = '\\';
				}

				//add member
				if( !string.IsNullOrEmpty( memberNameOrPath ) )
				{
					if( addSeparator != '\0' )
						result.Append( addSeparator );
					result.Append( memberNameOrPath );
					addSeparator = '\\';
				}

				referenceValue = result.ToString();
			}
			else
			{
				referenceValue = "";
				addSeparator = '\0';
			}
		}

		public static string CalculateResourceReference( Component to, string memberNameOrPath = "" )
		{
			CalculateResourceReference( to, memberNameOrPath, out string referenceValue, out char addSeparator );
			return referenceValue;
		}

		public static ReferenceNoValue MakeResourceReference( Component to, string memberNameOrPath = "" )
		{
			return new ReferenceNoValue( CalculateResourceReference( to, memberNameOrPath ) );
		}

		public static Reference<T> MakeResourceReference<T>( Component to, string memberNameOrPath = "" )
		{
			return new Reference<T>( default, CalculateResourceReference( to, memberNameOrPath ) );
		}

		//!!!!new

		static int GetEqualPart( IList<string> strings )
		{
			var minimumLength = strings.Min( x => x.Length );
			int commonChars;
			for( commonChars = 0; commonChars < minimumLength; commonChars++ )
			{
				if( strings.Select( x => x[ commonChars ] ).Distinct().Count() > 1 )
					break;
			}
			return commonChars;
			//return strings[ 0 ].Substring( 0, commonChars );
		}

		public static void CalculateRelativePathReference( Component from, Component to, string memberNameOrPath, out string referenceValue, out char addSeparator )
		{
			//!!!!slowly

			var result = new StringBuilder();
			result.Append( "relative:" );

			var fromResource = ComponentUtility.GetOwnedFileNameOfComponent( from );
			var toResource = ComponentUtility.GetOwnedFileNameOfComponent( to );
			if( fromResource != toResource )
			{
				var fromResourceFolder = PathUtility.GetDirectoryName( fromResource );
				var toResourceFolder = PathUtility.GetDirectoryName( toResource );

				int commonChars = GetEqualPart( new string[] { fromResourceFolder, toResourceFolder } );

				var path = "";

				//go up (dots)
				{
					var c = fromResourceFolder.Substring( commonChars ).Split( new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries ).Length;
					for( int n = 0; n < c; n++ )
						path = Path.Combine( path, ".." );
				}

				//go down
				{
					var s = toResource.Substring( commonChars );
					if( s.Length != 0 && ( s[ 0 ] == '\\' || s[ 0 ] == '/' ) )
						s = s.Substring( 1 );

					//can't use Path.Combine, invalid character exception
					if( path.Length != 0 && ( path[ 0 ] == '\\' || path[ 0 ] == '/' ) )
						path += "\\";
					path += s;
					//path = Path.Combine( path, s );
				}

				result.Append( path );
			}

			result.Append( "|" );
			addSeparator = '\0';

			//path
			var namePath = to.GetPathFromRoot();
			if( namePath != "" )
			{
				if( addSeparator != '\0' )
					result.Append( addSeparator );
				result.Append( namePath );
				addSeparator = '\\';
			}

			//add member
			if( !string.IsNullOrEmpty( memberNameOrPath ) )
			{
				if( addSeparator != '\0' )
					result.Append( addSeparator );
				result.Append( memberNameOrPath );
				addSeparator = '\\';
			}

			referenceValue = result.ToString();
		}

		public static string CalculateRelativePathReference( Component from, Component to, string memberNameOrPath = "" )
		{
			CalculateRelativePathReference( from, to, memberNameOrPath, out string referenceValue, out char addSeparator );
			return referenceValue;
		}

		public static ReferenceNoValue MakeRelativePathReference( Component from, Component to, string memberNameOrPath = "" )
		{
			return new ReferenceNoValue( CalculateRelativePathReference( from, to, memberNameOrPath ) );
		}

		public static Reference<T> MakeRelativePathReference<T>( Component from, Component to, string memberNameOrPath = "" )
		{
			return new Reference<T>( default, CalculateRelativePathReference( from, to, memberNameOrPath ) );
		}

		public static void CalculateRelativePathReference( Component from, string to, string memberNameOrPath, out string referenceValue, out char addSeparator )
		{
			//!!!!slowly

			var result = new StringBuilder();
			result.Append( "relative:" );

			var fromResource = ComponentUtility.GetOwnedFileNameOfComponent( from );
			var toResource = to;
			if( fromResource != toResource )
			{
				var fromResourceFolder = PathUtility.GetDirectoryName( fromResource );
				var toResourceFolder = PathUtility.GetDirectoryName( toResource );

				int commonChars = GetEqualPart( new string[] { fromResourceFolder, toResourceFolder } );

				var path = "";

				//go up (dots)
				{
					var c = fromResourceFolder.Substring( commonChars ).Split( new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries ).Length;
					for( int n = 0; n < c; n++ )
						path = Path.Combine( path, ".." );
				}

				//go down
				{
					var s = toResource.Substring( commonChars );
					if( s.Length != 0 && ( s[ 0 ] == '\\' || s[ 0 ] == '/' ) )
						s = s.Substring( 1 );

					//can't use Path.Combine, invalid character exception
					if( path.Length != 0 && path[ path.Length - 1 ] != '\\' && path[ path.Length - 1 ] != '/' )
						path += "\\";
					path += s;
					//path = Path.Combine( path, s );
				}

				result.Append( path );
			}

			result.Append( "|" );
			addSeparator = '\0';

			//add member
			if( !string.IsNullOrEmpty( memberNameOrPath ) )
			{
				if( addSeparator != '\0' )
					result.Append( addSeparator );
				result.Append( memberNameOrPath );
				addSeparator = '\\';
			}

			referenceValue = result.ToString();
		}

		public static string CalculateRelativePathReference( Component from, string to, string memberNameOrPath = "" )
		{
			CalculateRelativePathReference( from, to, memberNameOrPath, out string referenceValue, out char addSeparator );
			return referenceValue;
		}

		public static ReferenceNoValue MakeRelativePathReference( Component from, string to, string memberNameOrPath = "" )
		{
			return new ReferenceNoValue( CalculateRelativePathReference( from, to, memberNameOrPath ) );
		}

		public static Reference<T> MakeRelativePathReference<T>( Component from, string to, string memberNameOrPath = "" )
		{
			return new Reference<T>( default, CalculateRelativePathReference( from, to, memberNameOrPath ) );
		}

		public static bool EqualsReferences( string v1, string v2 )
		{
			//!!!!slowly?

			v1 = v1.Replace( '/', '\\' );
			v2 = v2.Replace( '/', '\\' );

			return v1 == v2;
		}

		public static bool CanMakeReferenceToObjectWithType( Metadata.TypeInfo expectedType, Metadata.TypeInfo objectType )//, bool allowConvertAnyToAndFromSystemObject )
		{
			var unrefObjectType = GetUnreferencedType( objectType );

			if( expectedType.IsAssignableFrom( unrefObjectType ) )
				return true;

			//!!!!new. for ICollection
			if( expectedType.GetNetType().IsAssignableFrom( unrefObjectType.GetNetType() ) )
				return true;

			if( MetadataManager.CanAutoConvertType( unrefObjectType.GetNetType(), expectedType.GetNetType() ) )
				return true;

			////System.Object works as any type. any type can be converted to System.Object and back.
			////if( allowConvertAnyToAndFromSystemObject )
			////{
			//var typeSystemObject = MetadataManager.GetTypeOfNetType( typeof( object ) );
			//if( expectedType == typeSystemObject || unrefObjectType == typeSystemObject )
			//	return true;
			////}

			return false;
		}

		public static void GetThisReferenceAmountOfToUpSteps( string referenceValue, out bool isThis, out int amount )
		{
			if( referenceValue.Length >= 5 && string.Compare( referenceValue, 0, "this:", 0, 5 ) == 0 )
			{
				//this:..
				//0123456

				//this:..\..\..
				//0123456789012

				int result = 0;

				int position = 5;

				again:
				bool foundDots = position + 2 <= referenceValue.Length && string.Compare( referenceValue, position, "..", 0, 2 ) == 0;
				if( foundDots )
				{
					result++;
					position += 2;

					bool foundSlash = position + 1 <= referenceValue.Length &&
						( referenceValue[ position ] == '\\' || referenceValue[ position ] == '/' );
					if( foundSlash )
					{
						position++;
						goto again;
					}
				}

				isThis = true;
				amount = result;
			}
			else
			{
				isThis = false;
				amount = 0;
			}
		}

		//!!!!смысла нет, хотя может надо будет

		//class TypeOrItsMembersCanReferencedToComponentTypeData
		//{
		//	public EDictionary<Assembly, bool> assembliesToSkip = new EDictionary<Assembly, bool>();
		//	public EDictionary<Type, bool> typesToSkip = new EDictionary<Type, bool>();
		//}

		//static List<Type> GetTypesWithGenericArguments( Type type )
		//{
		//	var result = new List<Type>();
		//	result.Add( type );
		//	try
		//	{
		//		result.AddRange( type.GetGenericArguments() );
		//	}
		//	catch { }
		//	return result;
		//}

		//static bool? TypeOrItsMembersCanReferencedToComponentType_IsSkipped( Type type )
		//{
		//	var data = typeOrItsMembersCanReferencedToComponentTypeData;

		//	var assembly = type.Assembly;
		//	bool skipAssembly;
		//	if( !data.assembliesToSkip.TryGetValue( assembly, out skipAssembly ) )
		//	{
		//		var coreAssembly = typeof( ReferenceUtils ).Assembly;
		//		skipAssembly = assembly != coreAssembly && !assembly.GetReferencedAssemblies().Contains( coreAssembly.GetName() );
		//		data.assembliesToSkip[ assembly ] = skipAssembly;
		//	}

		//	if( skipAssembly )
		//		return true;
		//	if( data.typesToSkip.TryGetValue( type, out bool skip ) )
		//		return skip;
		//	return null;
		//}

		//internal static bool TypeOrItsMembersCanReferencedToComponentType( Type type )
		//{
		//	var data = typeOrItsMembersCanReferencedToComponentTypeData;
		//	lock( data )
		//	{
		//		var assembly = type.Assembly;
		//		bool skipAssembly;
		//		if( !data.assembliesToSkip.TryGetValue( assembly, out skipAssembly ) )
		//		{
		//			var coreAssembly = typeof( ReferenceUtils ).Assembly;
		//			skipAssembly = assembly != coreAssembly && !assembly.GetReferencedAssemblies().Contains( coreAssembly.GetName() );
		//			data.assembliesToSkip[ assembly ] = skipAssembly;

		//			if( !skipAssembly )
		//			{
		//				//parse types

		//				ESet<Type> typesToConsider;
		//				try
		//				{
		//					typesToConsider = new ESet<Type>( assembly.GetExportedTypes() );
		//				}
		//				catch
		//				{
		//					return false;
		//				}

		//				//prepare reference map
		//				var allReferencesToType = new Dictionary<Type, Type[]>();
		//				{
		//					foreach( var type2 in typesToConsider )
		//					{
		//						ESet<Type> allReferenced = new ESet<Type>();
		//						if( type2.BaseType != null )
		//							allReferenced.AddRangeWithCheckAlreadyContained( GetTypesWithGenericArguments( type2.BaseType ) );
		//						foreach( var type3 in type2.GetNestedTypes( BindingFlags.Public ) )
		//							allReferenced.AddRangeWithCheckAlreadyContained( GetTypesWithGenericArguments( type3 ) );
		//						try
		//						{
		//							allReferenced.AddRangeWithCheckAlreadyContained( type2.GetGenericArguments() );
		//						}
		//						catch { }

		//						foreach( var member in MetadataManager.GetTypeOfNetType( type2 ).MetadataGetMembers() )
		//						{
		//							if( member is Metadata.Property )
		//							{
		//								//property
		//								var property = (Metadata.Property)member;
		//								if( !property.HasIndexers )
		//								{
		//									var type3 = property.Type.GetNetType();
		//									allReferenced.AddRangeWithCheckAlreadyContained( GetTypesWithGenericArguments( type3 ) );
		//								}
		//							}
		//							else if( member is Metadata.Method )
		//							{
		//								//method
		//								var method = (Metadata.Method)member;
		//								if( method.Parameters.Length == 1 && method.GetReturnParameters().Length == 1 )
		//								{
		//									var type3 = method.GetReturnParameters()[ 0 ].Type.GetNetType();
		//									allReferenced.AddRangeWithCheckAlreadyContained( GetTypesWithGenericArguments( type3 ) );
		//								}
		//							}
		//						}

		//						allReferencesToType[ type2 ] = allReferenced.ToArray();
		//					}
		//				}

		//				{
		//					again:;

		//					ESet<Type> newSet = new ESet<Type>( typesToConsider.Count );

		//					foreach( var type2 in typesToConsider )
		//					{
		//						////!!!!!
		//						//if( typeof( Angles ) == type2 )
		//						//	Log.Info( "dgdg" );

		//						//check is Component type
		//						if( typeof( Component ).IsAssignableFrom( type2 ) )
		//						{
		//							data.typesToSkip[ type2 ] = false;
		//						}
		//						else
		//						{
		//							//check referenced types

		//							bool? containsReferenced;
		//							{
		//								var allReferencedTypes = allReferencesToType[ type2 ];

		//								//check all referenced are not contains referenced to Component type

		//								containsReferenced = false;

		//								foreach( var type3 in allReferencedTypes )
		//								{
		//									var skip = TypeOrItsMembersCanReferencedToComponentType_IsSkipped( type3 );
		//									//if( data.typesToSkip.TryGetValue( type3, out bool skip ) )
		//									if( skip != null )
		//									{
		//										if( !skip.Value )
		//										{
		//											containsReferenced = true;
		//											break;
		//										}
		//									}
		//									else
		//									{
		//										//not finished. put to next iteration.
		//										containsReferenced = null;
		//										break;
		//									}
		//								}
		//							}

		//							if( containsReferenced != null )
		//							{
		//								//considered
		//								data.typesToSkip[ type2 ] = !containsReferenced.Value;
		//							}
		//							else
		//							{
		//								//not considers. put to next iteration.
		//								newSet.Add( type2 );
		//							}
		//						}
		//					}

		//					//again
		//					if( newSet.Count != typesToConsider.Count )
		//					{
		//						//one more iteration
		//						typesToConsider = newSet;
		//						goto again;
		//					}

		//					//add all not finished as "no skip".
		//					foreach( var type2 in newSet )
		//						data.typesToSkip[ type2 ] = true;// false;
		//				}

		//			}
		//		}
		//		if( skipAssembly )
		//			return false;

		//		{
		//			data.typesToSkip.TryGetValue( type, out bool skip );
		//			if( skip )
		//				return false;
		//		}

		//		return true;
		//	}
		//}

		public static void ParseReference( string reference, out string type, out string content )
		{
			int splitIndex = reference.IndexOf( ':' );
			if( splitIndex != -1 )
			{
				var referenceType2 = reference.Substring( 0, splitIndex );

				var type2 = MetadataManager.GetReferenceType( referenceType2 );
				if( type2 != null )
				{
					type = referenceType2;
					content = reference.Substring( splitIndex + 1 );
				}
				else
				{
					type = "resource";
					content = reference;
				}
			}
			else
			{
				type = "resource";
				content = reference;
			}
		}

		public static bool ConvertRelativePathToResource( string reference, Component from, out string result )
		{
			ParseReference( reference, out var type, out var content );
			if( type == "relative" )
			{
				string resourceName2;
				string path;
				{
					int splitIndex = content.IndexOf( '|' );
					if( splitIndex != -1 )
					{
						//with '|'
						resourceName2 = content.Substring( 0, splitIndex );
						path = content.Substring( splitIndex + 1 );
					}
					else
					{
						//resource name without '|'
						resourceName2 = content;
						path = "";
					}
				}

				var fromResource = ComponentUtility.GetOwnedFileNameOfComponent( from );
				var fromResourceFolder = "";
				if( !string.IsNullOrEmpty( fromResource ) )
					fromResourceFolder = PathUtility.GetDirectoryName( fromResource );

				var resourceName = "";
				{
					//!!!!

					resourceName = PathUtility.Combine( fromResourceFolder, resourceName2 );

					if( resourceName2.Contains( ".." ) )
					{
						//remove dots
						resourceName = VirtualPathUtility.GetVirtualPathByReal( Path.GetFullPath( VirtualPathUtility.GetRealPathByVirtual( resourceName ) ) );
					}
				}

				result = resourceName;
				if( !string.IsNullOrEmpty( path ) )
					result += "|" + path;
				return true;
			}

			result = "";
			return false;
		}

		//!!!!
		//public static bool ConvertRelativePathToResource( string reference, string from, out string result )
		//{
		//}

	}
}
