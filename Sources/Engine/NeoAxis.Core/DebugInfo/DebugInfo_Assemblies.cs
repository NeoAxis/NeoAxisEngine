//// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;
//using System.Text;
//using System.Reflection;

//namespace NeoAxis
//{
//	public class DebugInfoPage_Assemblies : DebugInfoPage
//	{
//		public override string Title
//		{
//			get { return "Assemblies"; }
//		}

//		public override string Content
//		{
//			get
//			{
//				var lines = new List<string>();

//				var assemblies = AppDomain.CurrentDomain.GetAssemblies();

//				var resultAssemblyNames = new List<AssemblyName>( assemblies.Length );
//				{
//					var remainingAssemblies = new List<Assembly>( assemblies );

//					while( true )
//					{
//						Assembly notReferencedAssembly = null;
//						{
//							foreach( Assembly assembly in remainingAssemblies )
//							{
//								var referencedAssemblies = assembly.GetReferencedAssemblies();

//								foreach( Assembly a in remainingAssemblies )
//								{
//									if( assembly == a )
//										continue;

//									AssemblyName aName = a.GetName();

//									foreach( AssemblyName referencedAssembly in referencedAssemblies )
//									{
//										if( referencedAssembly.Name == aName.Name )
//											goto nextAssembly;
//									}
//								}

//								notReferencedAssembly = assembly;
//								break;

//								nextAssembly:;
//							}
//						}

//						if( notReferencedAssembly != null )
//						{
//							remainingAssemblies.Remove( notReferencedAssembly );
//							resultAssemblyNames.Add( notReferencedAssembly.GetName() );
//						}
//						else
//						{
//							//no exists not referenced assemblies
//							foreach( Assembly assembly in remainingAssemblies )
//								resultAssemblyNames.Add( assembly.GetName() );
//							break;
//						}
//					}
//				}

//				foreach( AssemblyName assemblyName in resultAssemblyNames )
//					lines.Add( string.Format( "{0}, {1}", assemblyName.Name, assemblyName.Version ) );

//				return LinesToString( lines );
//			}
//		}
//	}
//}
