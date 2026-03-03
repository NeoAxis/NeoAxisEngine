#if !NO_SERVER
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using NeoAxis;

namespace NeoAxis.CloudServer
{
	/// <summary>
	/// The basic methods of the server.
	/// </summary>
	public static class BasicCommands
	{
		[CloudMethod( CloudUserRole.Developer )]
		public static void StopAction( string id )
		{
			Actions.Stop( id );
		}

		/// <summary>
		/// Deletes all collections in the database.
		/// </summary>
		/// <exception cref="Exception"></exception>
		[CloudMethod( CloudUserRole.Developer, AddToCommands = true )]
		[Description( "Delete all collections in the database." )]
		public static void DatabaseDeleteAll()
		{
			var database = CloudFunctionsServer.ServerNode.CloudFunctions.DatabaseImpl;
			if( database == null )
				throw new Exception( "Database is not initialized." );
			database.DeleteAll();
		}

		/// <summary>
		/// Clears save strings records in the database.
		/// </summary>
		/// <exception cref="Exception"></exception>
		[CloudMethod( CloudUserRole.Developer, AddToCommands = true )]
		[Description( "Clear save strings records." )]
		public static void DatabaseClearSaveStrings()
		{
			var database = CloudFunctionsServer.ServerNode.CloudFunctions.DatabaseImpl;
			if( database == null )
				throw new Exception( "Database is not initialized." );
			database.ClearSaveStrings();
		}

		[CloudMethod( CloudUserRole.Developer, AddToCommands = true )]
		[Description( "Show Docker version in the Logs page." )]
		public static void DockerGetVersion()
		{
			Console.WriteLine( "Getting Docker version..." );
			ServerUtility.ExecuteBashCommand( "docker --version" );
		}

		[CloudMethod( CloudUserRole.Developer, AddToCommands = true )]
		[Description( "Install Docker." )]
		public static void DockerInstall()
		{
			Console.WriteLine( "Installing Docker..." );

			//script from https://github.com/docker/docker-install/
			if( !ServerUtility.ExecuteBashCommand( "curl -fsSL https://get.docker.com -o get-docker.sh" ) )
				return;
			if( !ServerUtility.ExecuteBashCommand( "sh get-docker.sh" ) )
				return;

			Console.WriteLine( "Installing Docker END." );
		}

		[CloudMethod( CloudUserRole.Developer, AddToCommands = true )]
		[Description( "Show Python 3 version in the Logs page." )]
		public static void PythonGetVersion()
		{
			Console.WriteLine( "Getting Python 3 version..." );
			ServerUtility.ExecuteBashCommand( "python3 --version" );
		}

		//not tested
		//[CloudMethod( CloudUserRole.Developer, AddToCommands = true )]
		//[Description( "Install Python 3." )]
		//public static void PythonInstall()
		//{
		//	Console.WriteLine( "Installing Python 3..." );

		//	// Download and install Python 3 using apt-get (for Debian/Ubuntu)
		//	// You may want to adjust this for other distributions
		//	if( !ServerUtility.ExecuteBashCommand( "sudo apt-get update" ) )
		//		return;
		//	if( !ServerUtility.ExecuteBashCommand( "sudo apt-get install -y python3 python3-pip" ) )
		//		return;

		//	Console.WriteLine( "Installing Python 3 END." );

		//	//!!!!

		//	Console.WriteLine( "Installing virtualenv for Python 3..." );

		//	// Ensure pip is installed, then install virtualenv
		//	if( !ServerUtility.ExecuteBashCommand( "python3 -m pip install --upgrade pip" ) )
		//		return;
		//	if( !ServerUtility.ExecuteBashCommand( "python3 -m pip install virtualenv" ) )
		//		return;

		//	Console.WriteLine( "Installing virtualenv for Python 3 END." );
		//}

	}
}
#endif