// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// This class reads and provides data from XML documentation files of assemblies.
	/// </summary>
	public static class XmlDocumentationFiles
	{
		static Dictionary<string, string> memberSummaries = new Dictionary<string, string>( 2048 );
		static ESet<Assembly> parsedAssemblies = new ESet<Assembly>();
		static ESet<string> triedLoadFiles = new ESet<string>();

		//

		public static void Load( string xmlFile )
		{
			if( triedLoadFiles.Contains( xmlFile ) )
				return;
			triedLoadFiles.Add( xmlFile );

			var path = Path.Combine( VirtualFileSystem.Directories.Binaries, xmlFile );
			if( File.Exists( path ) )
			{
				try
				{
					var document = new XmlDocument();
					document.Load( path );

					foreach( XmlNode docNode in document.SelectNodes( "doc" ) )
					{
						foreach( XmlNode membersNode in docNode.SelectNodes( "members" ) )
						{
							foreach( XmlNode childNode in membersNode.ChildNodes )
							{
								if( childNode.Name == "member" )
								{
									var name = childNode.Attributes[ "name" ].Value;

									XmlNode summaryNode = null;
									try
									{
										summaryNode = childNode.SelectSingleNode( "summary" );
									}
									catch { }

									if( summaryNode != null )
									{
										string summary = null;
										if( summaryNode.InnerText != null )
										{
											//Occurs when the < see cref = "P:NeoAxis.Component.Enabled" /> property value changes.

											summary = summaryNode.InnerXml.Trim();

											//remove <see> tags
											try
											{
												do
												{
													var startIndex = summary.IndexOf( "<see " );
													if( startIndex == -1 )
														break;

													{
														int endIndex = summary.IndexOf( "/>", startIndex + 1 );
														if( endIndex != -1 )
														{
															var str = summary.Substring( startIndex, endIndex - startIndex + 2 );

															var v1 = str.LastIndexOf( '.' );
															var v2 = str.LastIndexOf( '"' );
															if( v1 == -1 || v2 == -1 )
																break;
															var newStr = str.Substring( v1 + 1, v2 - v1 - 1 );

															newStr = TypeUtility.DisplayNameAddSpaces( newStr );

															summary = summary.Replace( str, newStr );

															continue;
														}
													}

													{
														int endIndex = summary.IndexOf( ">", startIndex + 1 );
														int endIndex2 = summary.IndexOf( "</see>", startIndex + 1 );
														if( endIndex != -1 && endIndex2 != -1 )
														{
															var s = summary;
															summary = s.Substring( 0, startIndex ) + s.Substring( endIndex + 1, endIndex2 - endIndex - 1 ) + s.Substring( endIndex2 + "</see>".Length );
															continue;
														}
													}

													break;

												} while( true );
											}
											catch { }

											//summary = summaryNode.InnerText.Trim();
										}

										if( !string.IsNullOrEmpty( summary ) )
											memberSummaries[ name ] = summary;
									}
								}
							}
						}
					}

				}
				catch( Exception e )
				{
					Log.Warning( "XmlDocumentationData: Load: Unable to parse file \'{0}\'. Error: \'{1}\'", path, e.Message );
				}
			}
		}

		public static void PreloadBaseAssemblies()
		{
			Load( "NeoAxis.Core.xml" );
			Load( "NeoAxis.CoreExtension.xml" );

			//CheckMissingSummaries();
		}

		internal static void LoadFileForType( Metadata.TypeInfo type )
		{
			try
			{
				var assembly = type.GetNetType().Assembly;
				if( assembly == null )
					return;
				if( parsedAssemblies.Contains( assembly ) )
					return;
				parsedAssemblies.Add( assembly );

				var fileName = Path.GetFileName( assembly.Location );
				if( !string.IsNullOrEmpty( fileName ) )
				{
					var xmlFileName = Path.ChangeExtension( fileName, ".xml" );
					Load( xmlFileName );
				}
			}
			catch { }
		}

		public static string GetMemberSummary( string member )
		{
			if( memberSummaries.TryGetValue( member, out var value ) )
				return value;
			return null;
		}

		public static string GetTypeId( Metadata.TypeInfo type )
		{
			LoadFileForType( type );

			return "T:" + type.Name;
		}

		public static string GetMemberId( Metadata.Member member )
		{
			var type = member.Owner as Metadata.TypeInfo;
			if( type != null )
			{
				LoadFileForType( type );

				{
					var p = member as Metadata.Property;
					if( p != null )
					{
						var netP = p as Metadata.NetTypeInfo.NetProperty;
						if( netP != null && netP.NetMember is FieldInfo )
							return "F:" + type.Name + "." + p.Name;
						else
							return "P:" + type.Name + "." + p.Name;
					}
				}

				{
					var m = member as Metadata.Method;
					if( m != null )
					{
						var b = new StringBuilder( "M:" + type.Name + "." + m.Name );

						bool paramWasAdded = false;
						for( int n = 0; n < m.Parameters.Length; n++ )
						{
							var p = m.Parameters[ n ];
							if( !p.ReturnValue )
							{
								if( paramWasAdded )
									b.Append( ',' );
								else
									b.Append( '(' );

								if( !p.ReturnValue )
								{
									//!!!!check

									if( p.Output )
										b.Append( "out " );
									else if( p.ByReference )
										b.Append( "ref " );
								}
								b.Append( p.Type.Name );

								paramWasAdded = true;
							}
						}
						if( paramWasAdded )
							b.Append( ')' );

						return b.ToString();
					}
				}

				{
					var e = member as Metadata.Event;
					if( e != null )
						return "E:" + type.Name + "." + e.Name;
				}
			}

			return null;
		}

		static void CheckMissingSummaries()
		{
			//ESet<string> toSkip = new ESet<string>();
			//toSkip.Add( "NeoAxis.Component.T property: Enabled" );
			//toSkip.Add( "NeoAxis.Component.T property: Name" );

			bool titleWasAdded = false;

			ESet<Metadata.Member> processed = new ESet<Metadata.Member>( 8000 );

			foreach( var type in MetadataManager.NetTypes.ToArray() )
			{
				if( typeof( Component ).IsAssignableFrom( type.GetNetType() ) && type.GetNetType().IsPublic )
				{
					Metadata.GetMembersContext context = new Metadata.GetMembersContext();
					context.filter = false;
					foreach( var member in type.MetadataGetMembers( context ) )
					{
						string missingText = null;

						var property = member as Metadata.Property;
						if( property != null && property.Browsable && !processed.Contains( property ) )
						{
							processed.AddWithCheckAlreadyContained( property );

							var id = GetMemberId( member );
							if( !string.IsNullOrEmpty( id ) )
							{
								if( string.IsNullOrEmpty( GetMemberSummary( id ) ) )
								{
									var text = type.Name + " " + "property: " + member.Name;
									//if( !toSkip.Contains( text ) )
									missingText = text;
								}
							}
						}

						//!!!!events


						if( missingText != null )
						{
							if( !titleWasAdded )
							{
								Log.InvisibleInfo( "-----------------------------" );
								Log.InvisibleInfo( "Missing type descriptions:" );
								titleWasAdded = true;
							}

							Log.InvisibleInfo( missingText );
						}
					}
				}
			}

			if( titleWasAdded )
				Log.InvisibleInfo( "-----------------------------" );
		}
	}
}