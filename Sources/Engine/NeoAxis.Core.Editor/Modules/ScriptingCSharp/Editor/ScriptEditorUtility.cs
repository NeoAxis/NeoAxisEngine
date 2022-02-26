// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NeoAxis.Editor
{
	static class ScriptEditorUtility
	{
		static string GetNetTypeName( Type type )
		{
			var result = MetadataManager.GetNetTypeName( type, true, false );
			result = result.Replace( "&", "" );
			return result;
		}

		static string GetFieldText( FieldInfo f )
		{
			var result = "";

			if( f.IsPublic )
				result += "public ";
			else if( f.IsFamily )
				result += "protected ";

			if( f.IsStatic )
				result += "static ";

			result += GetNetTypeName( f.FieldType );

			result += " ";
			result += f.Name;
			result += ";";

			return result;
		}

		static string GetPropertyText( PropertyInfo p )
		{
			var result = "";

			var getMethod = p.GetGetMethod( true );
			var setMethod = p.GetSetMethod( true );
			if( getMethod != null )
			{
				if( getMethod.IsPublic )
					result += "public ";
				else if( getMethod.IsFamily )
					result += "protected ";

				if( getMethod.IsVirtual )
					result += "virtual ";

				if( getMethod.IsStatic )
					result += "static ";
			}

			result += GetNetTypeName( p.PropertyType );
			result += " ";

			var indexParameters = p.GetIndexParameters();
			if( indexParameters.Length != 0 )
			{
				//public Component this[ string nameOrPath ] { get; }

				result += "this[ ";

				for( int n = 0; n < indexParameters.Length; n++ )
				{
					var index = indexParameters[ n ];
					if( n != 0 )
						result += ", ";

					var t = index.ParameterType;

					bool isByReference = t.IsByRef;
					if( isByReference )
						t = t.GetElementType();

					if( index.IsOut )
						result += "out ";
					else if( isByReference )
						result += "ref ";

					result += GetNetTypeName( index.ParameterType );
					result += " ";
					result += index.Name;

					//!!!!default value
				}

				result += " ]";
			}
			else
				result += p.Name;

			result += " {";
			if( getMethod != null )
				result += " get;";
			if( setMethod != null )
				result += " set;";

			result += " }";

			return result;
		}

		static string GetMethodText( MethodBase m )
		{
			bool isOperator = m.IsSpecialName && m.Name.Length > 3 && m.Name.Substring( 0, 3 ) == "op_";

			StringBuilder b = new StringBuilder();

			if( m.IsPublic )
				b.Append( "public " );
			else if( m.IsFamily )
				b.Append( "protected " );

			if( m.IsVirtual )
				b.Append( "virtual " );

			if( m.IsStatic )
				b.Append( "static " );

			var parameters = m.GetParameters();

			//return param
			{
				//!!!!может быть несколько

				ParameterInfo returnParam = null;
				foreach( var p in parameters )
				{
					if( p.IsRetval )
					{
						returnParam = p;
						break;
					}
				}
				if( returnParam != null )
					b.Append( GetNetTypeName( returnParam.ParameterType ) );
				else
					b.Append( "void" );
			}

			b.Append( " " );

			string name = m.IsConstructor ? "Constructor" : m.Name;
			if( isOperator && name.Length > 3 && name.Substring( 0, 3 ) == "op_" )
				name = name.Substring( 3 );
			b.Append( name );
			//b.Append( Constructor ? "Constructor" : Name );

			b.Append( '(' );
			if( parameters.Length != 0 )
			{
				b.Append( ' ' );

				bool paramsWasAdded = false;
				for( int n = 0; n < parameters.Length; n++ )
				{
					var p = parameters[ n ];
					if( !p.IsRetval )
					{
						if( paramsWasAdded )
							b.Append( ", " );
						//b.Append( paramsWasAdded ? ", " : " " );

						var t = p.ParameterType;

						bool isByReference = t.IsByRef;
						if( isByReference )
							t = t.GetElementType();

						if( p.IsOut )
							b.Append( "out " );
						else if( isByReference )
							b.Append( "ref " );
						b.Append( GetNetTypeName( p.ParameterType ) );
						b.Append( ' ' );
						b.Append( p.Name );

						//!!!!default value

						paramsWasAdded = true;
					}
				}

				b.Append( ' ' );
			}
			b.Append( ");" );

			return b.ToString();
		}

		static string GetEventText( EventInfo evt )//, out RangeI nameRange )
		{
			var result = "";

			var method = evt.GetAddMethod( true );
			if( method != null )
			{
				if( method.IsPublic )
					result += "public ";
				else if( method.IsFamily )
					result += "protected ";

				if( method.IsVirtual )
					result += "virtual ";

				if( method.IsStatic )
					result += "static ";
			}

			result += "event ";
			result += GetNetTypeName( evt.EventHandlerType );
			result += " ";

			//int nameRangeStart = result.Length;
			result += evt.Name;
			//int nameRangeEnd = result.Length;
			result += ";";

			//nameRange = new RangeI( nameRangeStart, nameRangeEnd );
			return result;
		}

		static string GetAttributesText( IList<CustomAttributeData> attributes, string linePrefix )
		{
			var result = new StringBuilder();

			foreach( var attr in attributes )
			{
				var s = linePrefix + "\t\t[";

				var name = attr.AttributeType.Name;
				if( name.Length > 9 && name.Substring( name.Length - 9 ) == "Attribute" )
					name = name.Substring( 0, name.Length - 9 );
				s += name;

				if( attr.ConstructorArguments.Count != 0 )
				{
					s += "(";

					var first = true;
					foreach( var p in attr.ConstructorArguments )
					{
						if( !first )
							s += ",";
						s += " ";

						if( p.Value != null )
						{
							var s2 = "";
							if( p.ArgumentType == typeof( string ) )
								s2 = "\"" + p.Value.ToString() + "\"";
							else if( p.ArgumentType == typeof( bool ) )
								s2 = (bool)p.Value ? "true" : "false";
							else if( p.ArgumentType.IsEnum )
							{
								//!!!!impl Flags

								var name2 = Enum.GetName( p.ArgumentType, p.Value );
								if( name2 == null )
									name2 = p.Value.ToString();
								s2 = p.ArgumentType.Name + "." + name2;

								//s2 = p.ArgumentType.Name + "." + Enum.GetName( p.ArgumentType, p.Value ).ToString();
							}
							else
								s2 = p.Value.ToString();

							s += s2;
						}
						else
							s += "null";

						first = false;
					}

					s += " )";
				}

				s += "]";

				result.AppendLine( s );
			}

			return result.ToString();
		}

		static Type[] GetDeclaredInterfaces( Type t )
		{
			var allInterfaces = t.GetInterfaces();
			var baseInterfaces = Enumerable.Empty<Type>();
			if( t.BaseType != null )
				baseInterfaces = t.BaseType.GetInterfaces();
			return allInterfaces.Except( baseInterfaces ).ToArray();
		}

		static string GetMetadataText2( Type type, bool addNamespace, string linePrefix, ISymbol selectSymbol, out int outSelectLine )//RangeI outSelectRange )
		{
			outSelectLine = 0;
			//outSelectRange = RangeI.Zero;

			var result = new StringBuilder();

			//!!!!пока доки берутся через XmlDocumentationFiles. потом нужно для всех .NET типов суппорт
			var engineType = MetadataManager.GetTypeOfNetType( type );
			var netMemberSummary = new Dictionary<MemberInfo, string>();

			if( engineType != null )
			{
				foreach( var member in engineType.MetadataGetMembers() )
				{
					var id = XmlDocumentationFiles.GetMemberId( member );
					if( !string.IsNullOrEmpty( id ) )
					{
						var summary = XmlDocumentationFiles.GetMemberSummary( id );
						if( !string.IsNullOrEmpty( summary ) )
						{
							//!!!!глючит
							//if( summary.Contains( "Calls " ) )
							//{
							//	Log.Info( "Dfdfg" );
							//}

							//
							try
							{
								summary = Regex.Replace( summary, "<.*?>", string.Empty );
							}
							catch { }

							var method = member as Metadata.NetTypeInfo.NetMethod;
							if( method != null )
								netMemberSummary[ method.NetMember ] = summary;

							var property = member as Metadata.NetTypeInfo.NetProperty;
							if( property != null )
								netMemberSummary[ property.NetMember ] = summary;

							var evn = member as Metadata.NetTypeInfo.NetEvent;
							if( evn != null )
								netMemberSummary[ evn.NetMember ] = summary;
						}
					}
				}
			}

			if( addNamespace )
			{
				result.AppendLine( linePrefix + "namespace " + type.Namespace );
				result.AppendLine( linePrefix + "{" );
			}

			if( engineType != null )
			{
				var id = XmlDocumentationFiles.GetTypeId( engineType );
				if( !string.IsNullOrEmpty( id ) )
				{
					var summary = XmlDocumentationFiles.GetMemberSummary( id );
					if( !string.IsNullOrEmpty( summary ) )
					{
						result.AppendLine( linePrefix + "\t// " + summary );

						//result.AppendLine( linePrefix + "\t//" );
						//result.AppendLine( linePrefix + "\t// Summary:" );
						//result.AppendLine( linePrefix + "\t//    " + "text" );
					}
				}
			}

			{
				var line = "\t";

				if( type.IsPublic || type.IsNestedPublic )
					line += "public ";
				else if( type.IsNestedFamily )
					line += "protected ";

				if( type.IsAbstract && type.IsSealed )
					line += "static ";
				if( type.IsAbstract )
					line += "abstract ";
				else if( type.IsSealed )
					line += "sealed ";

				if( type.IsValueType )
					line += "struct ";
				else if( type.IsClass )
					line += "class ";
				else if( type.IsInterface )
					line += "interface ";

				line += type.Name;

				var baseType = type.BaseType;
				var interfaces = GetDeclaredInterfaces( type );//type.GetInterfaces();
				if( baseType != null && baseType != typeof( object ) && baseType != typeof( ValueType ) || interfaces.Length != 0 )
				{
					line += " :";

					bool more = false;

					if( baseType != null && baseType != typeof( object ) && baseType != typeof( ValueType ) )
					{
						line += " " + baseType.Name;
						more = true;
					}

					foreach( var iface in interfaces )
					{
						////!!!!
						//if( iface.DeclaringType != baseType )
						//	continue;

						if( more )
							line += ",";
						line += " " + GetNetTypeName( iface );
						more = true;
					}
				}

				result.AppendLine( linePrefix + line );
			}

			result.AppendLine( linePrefix + "\t{" );

			var addEmptyLine = false;

			var fields = type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
			CollectionUtility.MergeSort( fields, delegate ( FieldInfo m1, FieldInfo m2 )
			{
				if( m1.IsStatic && !m2.IsStatic )
					return -1;
				if( !m1.IsStatic && m2.IsStatic )
					return 1;
				return 0;
			} );

			foreach( var netField in fields )
			{
				if( !netField.IsFamily && !netField.IsPublic )
					continue;

				result.AppendLine( linePrefix + "\t\t//" );
				if( netMemberSummary.TryGetValue( netField, out var summary ) )
					result.AppendLine( linePrefix + "\t\t// " + summary );

				GetAttributesText( netField.GetCustomAttributesData(), linePrefix );

				var line = linePrefix + "\t\t";
				line += GetFieldText( netField );
				result.AppendLine( line );

				var fieldSymbol = selectSymbol as IFieldSymbol;
				if( fieldSymbol != null && fieldSymbol.Name == netField.Name )
					outSelectLine = result.ToString().Count( f => f == '\n' );// + 1;

				addEmptyLine = true;
			}

			if( addEmptyLine )
				result.AppendLine( linePrefix + "\t\t" );
			addEmptyLine = false;

			var constructors = type.GetConstructors( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
			CollectionUtility.MergeSort( constructors, delegate ( ConstructorInfo m1, ConstructorInfo m2 )
			{
				if( m1.IsStatic && !m2.IsStatic )
					return -1;
				if( !m1.IsStatic && m2.IsStatic )
					return 1;
				return 0;
			} );

			foreach( var netMethod in constructors )
			{
				if( !netMethod.IsFamily && !netMethod.IsPublic )
					continue;
				//if( netMethod.IsSpecialName )
				//{
				//	if( netMethod.Name.Length > 4 && ( netMethod.Name.Substring( 0, 4 ) == "get_" || netMethod.Name.Substring( 0, 4 ) == "set_" ) )
				//		continue;
				//	if( netMethod.Name.Length > 4 && netMethod.Name.Substring( 0, 4 ) == "add_" )
				//		continue;
				//	if( netMethod.Name.Length > 7 && netMethod.Name.Substring( 0, 7 ) == "remove_" )
				//		continue;
				//}
				//if( netMethod.GetBaseDefinition() != netMethod )
				//	continue;

				result.AppendLine( linePrefix + "\t\t//" );
				if( netMemberSummary.TryGetValue( netMethod, out var summary ) )
					result.AppendLine( linePrefix + "\t\t// " + summary );

				GetAttributesText( netMethod.GetCustomAttributesData(), linePrefix );

				var line = linePrefix + "\t\t";
				line += GetMethodText( netMethod );
				result.AppendLine( line );

				var methodSymbol = selectSymbol as IMethodSymbol;
				if( methodSymbol != null && methodSymbol.Name == netMethod.Name )
					outSelectLine = result.ToString().Count( f => f == '\n' );// + 1;

				addEmptyLine = true;
			}

			if( addEmptyLine )
				result.AppendLine( linePrefix + "\t\t" );
			addEmptyLine = false;

			var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
			CollectionUtility.MergeSort( properties, delegate ( PropertyInfo m1, PropertyInfo m2 )
			{
				var getMethod1 = m1.GetGetMethod( true );
				var getMethod2 = m2.GetGetMethod( true );
				if( getMethod1 != null && getMethod2 != null )
				{
					if( getMethod1.IsStatic && !getMethod2.IsStatic )
						return -1;
					if( !getMethod1.IsStatic && getMethod2.IsStatic )
						return 1;
				}
				return 0;
			} );

			foreach( var netProperty in properties )
			{
				var getMethod = netProperty.GetGetMethod( true );
				if( getMethod != null )
				{
					if( !getMethod.IsFamily && !getMethod.IsPublic )
						continue;
				}

				result.AppendLine( linePrefix + "\t\t//" );
				if( netMemberSummary.TryGetValue( netProperty, out var summary ) )
					result.AppendLine( linePrefix + "\t\t// " + summary );

				GetAttributesText( netProperty.GetCustomAttributesData(), linePrefix );

				var line = linePrefix + "\t\t";
				line += GetPropertyText( netProperty );
				result.AppendLine( line );

				var propertySymbol = selectSymbol as IPropertySymbol;
				if( propertySymbol != null && propertySymbol.Name == netProperty.Name )
					outSelectLine = result.ToString().Count( f => f == '\n' );// + 1;

				addEmptyLine = true;
			}

			if( addEmptyLine )
				result.AppendLine( linePrefix + "\t\t" );
			addEmptyLine = false;

			var events = type.GetEvents( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
			CollectionUtility.MergeSort( events, delegate ( EventInfo m1, EventInfo m2 )
			{
				var method1 = m1.GetAddMethod( true );
				var method2 = m2.GetAddMethod( true );
				if( method1 != null && method2 != null )
				{
					if( method1.IsStatic && !method2.IsStatic )
						return -1;
					if( !method1.IsStatic && method2.IsStatic )
						return 1;
				}
				return 0;
			} );

			foreach( var netEvent in events )
			{
				var method = netEvent.GetAddMethod( true );
				if( method != null )
				{
					if( !method.IsFamily && !method.IsPublic )
						continue;
				}

				result.AppendLine( linePrefix + "\t\t//" );
				if( netMemberSummary.TryGetValue( netEvent, out var summary ) )
					result.AppendLine( linePrefix + "\t\t// " + summary );

				GetAttributesText( netEvent.GetCustomAttributesData(), linePrefix );

				int textOffset = result.Length;

				var line = linePrefix + "\t\t";
				line += GetEventText( netEvent );//, out var nameRange );
				result.AppendLine( line );

				var eventSymbol = selectSymbol as IEventSymbol;
				if( eventSymbol != null && eventSymbol.Name == netEvent.Name )
				{
					outSelectLine = result.ToString().Count( f => f == '\n' );// + 1;

					//outSelectRange = new RangeI( nameRange.Minimum + textOffset, nameRange.Maximum + textOffset );

					//var text = editor2.Document.Text;
					//if( text.Length >= span.Start )
					//{
					//	var text2 = text.Substring( 0, span.Start );
					//	int line = text2.Count( f => f == '\n' ) + 1;
					//	editor2.ScrollToLine( line );
					//}

					//!!!!
					//eventSymbol.Name
				}

				addEmptyLine = true;
			}

			if( addEmptyLine )
				result.AppendLine( linePrefix + "\t\t" );
			addEmptyLine = false;

			var methods = type.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
			CollectionUtility.MergeSort( methods, delegate ( MethodInfo m1, MethodInfo m2 )
			{
				if( m1.IsStatic && !m2.IsStatic )
					return -1;
				if( !m1.IsStatic && m2.IsStatic )
					return 1;
				return 0;
			} );

			foreach( var netMethod in methods )
			{
				if( !netMethod.IsFamily && !netMethod.IsPublic )
					continue;
				if( netMethod.IsSpecialName )
				{
					if( netMethod.Name.Length > 4 && ( netMethod.Name.Substring( 0, 4 ) == "get_" || netMethod.Name.Substring( 0, 4 ) == "set_" ) )
						continue;
					if( netMethod.Name.Length > 4 && netMethod.Name.Substring( 0, 4 ) == "add_" )
						continue;
					if( netMethod.Name.Length > 7 && netMethod.Name.Substring( 0, 7 ) == "remove_" )
						continue;
				}
				if( netMethod.GetBaseDefinition() != netMethod )
					continue;

				result.AppendLine( linePrefix + "\t\t//" );
				if( netMemberSummary.TryGetValue( netMethod, out var summary ) )
					result.AppendLine( linePrefix + "\t\t// " + summary );

				GetAttributesText( netMethod.GetCustomAttributesData(), linePrefix );

				var line = linePrefix + "\t\t";
				line += GetMethodText( netMethod );
				result.AppendLine( line );

				//!!!!override methods support
				var methodSymbol = selectSymbol as IMethodSymbol;
				if( methodSymbol != null && methodSymbol.Name == netMethod.Name )
					outSelectLine = result.ToString().Count( f => f == '\n' );// + 1;

				addEmptyLine = true;
			}

			//nested types
			bool existsNestedTypes = false;
			foreach( var nestedType in type.GetNestedTypes() )
			{
				if( typeof( Delegate ).IsAssignableFrom( nestedType ) )
					continue;
				if( typeof( Enum ).IsAssignableFrom( nestedType ) )
					continue;

				result.AppendLine( linePrefix + "\t" );

				var text = GetMetadataText2( nestedType, false, linePrefix + "\t", null, out _ );
				result.Append( text );

				existsNestedTypes = true;
			}
			if( existsNestedTypes )
				result.AppendLine( linePrefix );

			result.AppendLine( linePrefix + "\t}" );
			if( addNamespace )
				result.AppendLine( linePrefix + "}" );

			return result.ToString();
		}

		public static string GetMetadataText( Type type, ISymbol selectSymbol, out int selectLine )//RangeI outSelectRange )
		{
			selectLine = 0;
			//outSelectRange = RangeI.Zero;

			try
			{
				return GetMetadataText2( type, true, "", selectSymbol, out selectLine );// outSelectRange );
			}
			catch( Exception e )
			{
				return "Exception: " + e.Message;
			}
		}
	}
}
