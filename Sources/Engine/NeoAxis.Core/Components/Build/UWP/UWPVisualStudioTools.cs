//// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using Microsoft.VisualStudio.Setup.Configuration;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace NeoAxis
//{
//	//alternative implementation:
//	// https://github.com/cake-build/cake/issues/1369
//	// see also https://github.com/cake-build/cake/blob/develop/src/Cake.Common/Tools/MSBuild/MSBuildResolver.cs


//	internal static class UWPVisualStudioTools
//	{
//		//public static string GetMsBuildInstallPath()
//		//{
//		//	var instance = LocateVisualStudioInstance( "15", new HashSet<string>( new[] { "Microsoft.Component.MSBuild" } ) ) as ISetupInstance2;
//		//	var installationPath = instance.GetInstallationPath();
//		//	return Path.Combine( installationPath, "MSBuild", "15.0" );
//		//}

//		public static string GetVisualStudioInstallPath()
//		{
//			var instance = LocateVisualStudioInstance( "15" ) as ISetupInstance2;
//			var installationPath = instance.GetInstallationPath();
//			return Path.Combine( installationPath, "Common7", "IDE" );
//		}

//		static ISetupConfiguration GetSetupConfiguration()
//		{
//			ISetupConfiguration setupConfiguration = null;

//			try
//			{
//#if !W__!!__INDOWS_UWP //!!!! Need to fix FxConverter. support ClassInterfaceAttribute ?
//				setupConfiguration = new SetupConfiguration();
//#endif
//			}
//			catch( COMException comException ) when( comException.HResult == unchecked((int)0x80040154) )
//			{
//				Console.WriteLine( "COM registration is missing, Visual Studio may not be installed correctly" );
//				throw;
//			}

//			return setupConfiguration;
//		}

//		static IEnumerable<ISetupInstance> EnumerateVisualStudioInstances()
//		{
//			var setupConfiguration = GetSetupConfiguration() as ISetupConfiguration2;

//			var instanceEnumerator = setupConfiguration.EnumAllInstances();
//			var instances = new ISetupInstance[ 3 ];

//			var instancesFetched = 0;
//			instanceEnumerator.Next( instances.Length, instances, out instancesFetched );

//			if( instancesFetched == 0 )
//			{
//				throw new Exception( "There were no instances of Visual Studio 15.0 or later found." );
//			}

//			do
//			{
//				for( var index = 0; index < instancesFetched; index++ )
//				{
//					yield return instances[ index ];
//				}

//				instanceEnumerator.Next( instances.Length, instances, out instancesFetched );
//			}
//			while( instancesFetched != 0 );
//		}

//		static ISetupInstance LocateVisualStudioInstance( string vsProductVersion, HashSet<string> requiredPackageIds = null )
//		{
//			var instances = EnumerateVisualStudioInstances().Where( ( instance ) => instance.GetInstallationVersion().StartsWith( vsProductVersion ) );

//			var instanceFoundWithInvalidState = false;

//			foreach( ISetupInstance2 instance in instances.OrderByDescending( i => i.GetInstallationVersion() ) )
//			{
//				//HACK: we should ignore Build Tools. We need only Visual Studio !
//				if( instance.GetDisplayName().Contains( "Build Tools" ) )
//					continue;

//				if( requiredPackageIds != null )
//				{
//					var packages = instance.GetPackages().Where( ( package ) => requiredPackageIds.Contains( package.GetId() ) );
//					if( packages.Count() != requiredPackageIds.Count )
//						continue;
//				}

//				const InstanceState minimumRequiredState = InstanceState.Local | InstanceState.Registered;

//				var state = instance.GetState();
//				if( ( state & minimumRequiredState ) == minimumRequiredState )
//					return instance;

//				Console.WriteLine( $"An instance matching the specified requirements but had an invalid state. (State: {state})" );
//				instanceFoundWithInvalidState = true;
//			}

//			throw new Exception( instanceFoundWithInvalidState ?
//				"An instance matching the specified requirements was found but it was in an invalid state." :
//				"There were no instances of Visual Studio 15.0 or later found that match the specified requirements." );
//		}
//	}
//}