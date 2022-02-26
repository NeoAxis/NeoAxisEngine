// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using NeoAxis;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// This abstract class is designed to register plug-in assemblies.
	/// </summary>
	public abstract class AssemblyRegistration
	{
		public virtual void OnRegister() { }
		public virtual void OnUnregister() { }
		public virtual void OnSendMessageToAllAssemblies( string message, object param, ref object result ) { }
	}
}

namespace Internal
{
	/// <summary>
	/// An auxiliary class for working with .NET assemblies.
	/// </summary>
	public static class AssemblyUtility
	{
		internal static ESet<string> disableAssemblyRegistration;
		internal static ESet<string> disableNamespaceRegistration;
		static EDictionary<Assembly, RegisteredAssemblyItem> registeredAssemblies = new EDictionary<Assembly, RegisteredAssemblyItem>();

		/////////////////////////////////////////

		/// <summary>
		/// A class representing registered .NET assembly.
		/// </summary>
		public class RegisteredAssemblyItem
		{
			internal Assembly assembly;
			internal List<AssemblyRegistration> registrationObjects = new List<AssemblyRegistration>();
			internal string registerTypeNamesWithIncludedAssemblyName;

			//

			public Assembly Assembly
			{
				get { return assembly; }
			}

			public List<AssemblyRegistration> RegistrationObjects
			{
				get { return registrationObjects; }
			}

			public string RegisterTypeNamesWithIncludedAssemblyName
			{
				get { return registerTypeNamesWithIncludedAssemblyName; }
			}
		}

		/////////////////////////////////////////

		public static ICollection<RegisteredAssemblyItem> RegisteredAssemblies
		{
			get
			{
				lock( registeredAssemblies )
				{
					RegisteredAssemblyItem[] r = new RegisteredAssemblyItem[ registeredAssemblies.Count ];
					registeredAssemblies.Values.CopyTo( r, 0 );
					return r;
				}
			}
		}

		static Assembly LoadAssemblyByRealFileNameImpl( string realFileName, bool returnNullIfFileIsNotExists, bool loadWithoutLocking, Assembly reloadingOldAssembly )
		{
			string changedFileName = realFileName;

			if( !Path.IsPathRooted( realFileName ) )
				changedFileName = Path.Combine( VirtualFileSystem.Directories.Binaries, changedFileName );

			string extension = Path.GetExtension( changedFileName );
			if( string.IsNullOrEmpty( extension ) || ( extension.ToLower() != ".dll" && extension.ToLower() != ".exe" ) )
				changedFileName += ".dll";

			if( returnNullIfFileIsNotExists && !File.Exists( changedFileName ) )
				return null;

			//new
			{
				//get AssemblyName
				AssemblyName assemblyName = null;
				try
				{
					assemblyName = AssemblyName.GetAssemblyName( changedFileName );
				}
				catch { }
				if( assemblyName == null )
					Log.Fatal( "The assembly is not found \"{0}\". Unable to get assembly name.", changedFileName );

				//check already loaded
				if( reloadingOldAssembly == null )
				{
					foreach( Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies() )
					{
						if( string.Compare( assembly2.FullName, assemblyName.FullName, true ) == 0 )
							return assembly2;
					}
				}

				Assembly assembly = null;
				try
				{
					if( loadWithoutLocking )
						assembly = Assembly.Load( File.ReadAllBytes( changedFileName ) );
					else
						assembly = Assembly.Load( assemblyName );
				}
				catch( Exception e )
				{
					string firstError = string.Format( "Loading assembly failed \"{0}\". Error: {1}", changedFileName, e.Message );

					//check already loaded
					if( reloadingOldAssembly == null )
					{
						foreach( Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies() )
						{
							if( string.Compare( assembly2.FullName, assemblyName.FullName, true ) == 0 )
								return assembly2;
						}
					}

					//Assembly.LoadFrom
					{
						try
						{
							assembly = Assembly.LoadFrom( changedFileName );
						}
						catch { }
						if( assembly != null )
							return assembly;
					}

					//CurrentDomain.Load
					{
						try
						{
							byte[] rawAssembly = File.ReadAllBytes( changedFileName );
							assembly = AppDomain.CurrentDomain.Load( rawAssembly );
						}
						catch { }
						if( assembly != null )
							return assembly;
					}

					Log.Fatal( firstError );
					//Log.Fatal( "Loading assembly failed \"{0}\". Error: {1}", changedFileName, e.Message );
					return null;
				}

				return assembly;
			}

			//try
			//{
			//	Assembly assembly = Assembly.LoadFrom( changedFileName );
			//	return assembly;
			//}
			//catch( Exception e )
			//{
			//	try
			//	{
			//		AssemblyName assemblyName = AssemblyName.GetAssemblyName( changedFileName );
			//		if( assemblyName == null )
			//			Log.Fatal( "The assembly is not found \"{0}\".", changedFileName );

			//		{
			//			foreach( Assembly assembly in AppDomain.CurrentDomain.GetAssemblies() )
			//			{
			//				if( string.Compare( assembly.FullName, assemblyName.FullName, true ) == 0 )
			//					return assembly;
			//			}
			//		}

			//		{
			//			byte[] rawAssembly = File.ReadAllBytes( changedFileName );
			//			Assembly assembly = AppDomain.CurrentDomain.Load( rawAssembly );
			//			return assembly;
			//		}
			//	}
			//	catch
			//	{
			//		Log.Fatal( "Loading assembly failed \"{0}\". Error: {1}", changedFileName, e.Message );
			//		return null;
			//	}
			//}
		}

		//!!!!name registerTypeNamesWithIncludedAssemblyName
		public static Assembly LoadAssemblyByRealFileName( string realFileName, bool returnNullIfFileIsNotExists, bool registerAssembly = true,
			string registerTypeNamesWithIncludedAssemblyName = "", bool loadWithoutLocking = false, Assembly reloadingOldAssembly = null )
		{
			var assembly = LoadAssemblyByRealFileNameImpl( realFileName, returnNullIfFileIsNotExists, loadWithoutLocking, reloadingOldAssembly );
			if( assembly != null && registerAssembly )
			{
				if( reloadingOldAssembly != null )
					UnregisterAssembly( reloadingOldAssembly, assembly );
				RegisterAssembly( assembly, registerTypeNamesWithIncludedAssemblyName, reloadingOldAssembly );
			}
			return assembly;
		}

		public static RegisteredAssemblyItem GetRegisteredAssembly( Assembly assembly )
		{
			lock( registeredAssemblies )
			{
				RegisteredAssemblyItem item;
				if( registeredAssemblies.TryGetValue( assembly, out item ) )
					return item;
				return null;
			}
		}

		internal static void ParseDisableAssemblyNamespaceRegistration()
		{
			disableAssemblyRegistration = new ESet<string>();
			disableNamespaceRegistration = new ESet<string>();

			string realFileName = Path.Combine( VirtualFileSystem.Directories.Binaries, "NeoAxis.DefaultSettings.config" );
			if( File.Exists( realFileName ) )
			{
				var block = TextBlockUtility.LoadFromRealFile( realFileName );
				if( block != null )
				{
					{
						var groupBlock = block.FindChild( "DisableAssemblyRegistration" );
						if( groupBlock != null )
						{
							foreach( var itemBlock in groupBlock.Children )
							{
								if( itemBlock.Name == "Assembly" )
								{
									bool disable = true;

									var platform = itemBlock.GetAttribute( "Platform" );
									if( !string.IsNullOrEmpty( platform ) )
									{
										if( string.Compare( SystemSettings.CurrentPlatform.ToString(), platform, true ) != 0 )
											disable = false;
									}

									if( disable )
									{
										var name = itemBlock.GetAttribute( "Name" );

										disableAssemblyRegistration.AddWithCheckAlreadyContained( name );
									}
								}
							}
						}
					}

					{
						var groupBlock = block.FindChild( "DisableNamespaceRegistration" );
						if( groupBlock != null )
						{
							foreach( var itemBlock in groupBlock.Children )
							{
								if( itemBlock.Name == "Namespace" )
								{
									bool disable = true;

									var platform = itemBlock.GetAttribute( "Platform" );
									if( !string.IsNullOrEmpty( platform ) )
									{
										if( string.Compare( SystemSettings.CurrentPlatform.ToString(), platform, true ) != 0 )
											disable = false;
									}

									if( disable )
									{
										var name = itemBlock.GetAttribute( "Name" );

										disableNamespaceRegistration.AddWithCheckAlreadyContained( name );
									}
								}
							}
						}
					}

				}
			}
			else
				Log.Warning( "AssemblyUtility: ParseDisableAssemblyNamespaceRegistration: \"NeoAxis.DefaultSettings.config\" is not exists." );
		}

		public delegate void RegisterAssemblyEventDelegate( Assembly assembly, Assembly reloadingOldAssembly );
		public static event RegisterAssemblyEventDelegate RegisterAssemblyEvent;

		public delegate void UnregisterAssemblyEventDelegate( Assembly assembly, Assembly reloadingNewAssembly );
		public static event UnregisterAssemblyEventDelegate UnregisterAssemblyEvent;

		public static RegisteredAssemblyItem RegisterAssembly( Assembly assembly, string registerTypeNamesWithIncludedAssemblyName, Assembly reloadingOldAssembly = null )
		{
			lock( registeredAssemblies )
			{
				RegisteredAssemblyItem item2 = GetRegisteredAssembly( assembly );
				if( item2 != null )
					return item2;

				if( disableAssemblyRegistration == null )
					ParseDisableAssemblyNamespaceRegistration();

				RegisteredAssemblyItem item = new RegisteredAssemblyItem();
				item.assembly = assembly;
				item.registerTypeNamesWithIncludedAssemblyName = registerTypeNamesWithIncludedAssemblyName;
				registeredAssemblies.Add( assembly, item );

				string name = assembly.GetName().Name;
				//if( name.EndsWith( ".STUB" ) )
				//	name = name.Replace( ".STUB", "" );

				bool enabled = !disableAssemblyRegistration.Contains( name );
				Log.InvisibleInfo( "Register assembly: {0}{1}", ( enabled ? "" : "[DISABLED] " ), assembly.FullName );
				//Log.InvisibleInfo( "AssemblyUtils: RegisterAssembly: {0} {1}", ( enabled ? "" : "[DISABLED]" ), assembly.FullName );
				//Log.InvisibleInfo( "AssemblyUtils: RegisterAssembly: [{0}] {1}", ( enabled ? "ENABLED" : "DISABLED" ), assembly.FullName );

				if( enabled )
				{
					StartupTiming.CounterStart( "Register assembly \'" + name + "\'" );
					try
					{
						var exportedTypes = assembly.GetExportedTypes();

						//optimization
						var isBaseNetAssembly = assembly == typeof( string ).Assembly || assembly == typeof( Uri ).Assembly;

						MetadataManager.RegisterTypesOfAssembly( exportedTypes );

						if( !isBaseNetAssembly )
						{
							MetadataManager.RegisterAutoConvertItemsForAssembly( exportedTypes, false );

							//MetadataManager.RegisterExtensions( assembly, types );

							//AssemblyRegistration classes
							foreach( Type type in exportedTypes )
							{
								if( !type.IsAbstract && typeof( AssemblyRegistration ).IsAssignableFrom( type ) )
								{
									var constructor = type.GetConstructor( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[ 0 ] { }, null );
									var ins = (AssemblyRegistration)constructor.Invoke( new object[ 0 ] );

									item.registrationObjects.Add( ins );

									ins.OnRegister();
								}
							}

							if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
							{
								EditorUtility.RegisterEditorExtensions( assembly, false );
								ResourcesWindowItems.RegisterAssembly( exportedTypes );
							}
						}

						RegisterAssemblyEvent?.Invoke( assembly, reloadingOldAssembly );
					}
					catch( Exception e )
					{
						Log.Error( "Unable to register assembly \'" + name + "\'. " + e.Message );
					}
					finally
					{
						StartupTiming.CounterEnd( "Register assembly \'" + name + "\'" );
					}
				}

				return item;
			}
		}

		public static void UnregisterAssembly( Assembly assembly, Assembly reloadingNewAssembly = null )
		{
			lock( registeredAssemblies )
			{
				RegisteredAssemblyItem item = GetRegisteredAssembly( assembly );

				string name = assembly.GetName().Name;
				//if( name.EndsWith( ".STUB" ) )
				//	name = name.Replace( ".STUB", "" );

				bool enabled = !disableAssemblyRegistration.Contains( name );
				Log.InvisibleInfo( "Unregister assembly: {0}", assembly.FullName );

				if( enabled )
				{
					StartupTiming.CounterStart( "Unregister assembly \'" + name + "\'" );
					try
					{
						var exportedTypes = assembly.GetExportedTypes();

						UnregisterAssemblyEvent?.Invoke( assembly, reloadingNewAssembly );

						//optimization
						var isBaseNetAssembly = assembly == typeof( string ).Assembly || assembly == typeof( Uri ).Assembly;

						if( !isBaseNetAssembly )
						{
							MetadataManager.RegisterAutoConvertItemsForAssembly( exportedTypes, true );

							//MetadataManager.RegisterExtensions( assembly, types );

							//AssemblyRegistration classes
							if( item != null )
							{
								foreach( var ins in item.registrationObjects )
									ins.OnUnregister();
							}

							if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
							{
								EditorUtility.RegisterEditorExtensions( assembly, true );
								ResourcesWindowItems.UnregisterAssembly( assembly );
							}
						}

					}
					catch( Exception e )
					{
						Log.Error( "Unable to unregister assembly \'" + name + "\'. " + e.Message );
					}
					finally
					{
						StartupTiming.CounterEnd( "Unregister assembly \'" + name + "\'" );
					}
				}

				MetadataManager.UnregisterTypesOfAssembly( assembly );

				registeredAssemblies.Remove( assembly );
			}
		}

		public static void SendMessageToAllRegisteredAssemblies( string message, object param, ref object result )
		{
			foreach( var item in RegisteredAssemblies )
			{
				foreach( var obj in item.RegistrationObjects )
					obj.OnSendMessageToAllAssemblies( message, param, ref result );
			}
		}
	}
}