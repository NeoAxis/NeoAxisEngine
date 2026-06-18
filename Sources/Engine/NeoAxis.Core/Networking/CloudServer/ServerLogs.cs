#if !NO_SERVER
// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using NeoAxis.Networking;

namespace NeoAxis.CloudServer
{
	public static class ServerLogs
	{
		static bool initialized;
		static object lockObject = new object();
		static bool logsDirectoryCreated;

		///////////////////////////////////////////////

		public static string LogsDirectory
		{
			get { return Path.Combine( CloudServerProcessUtility.CommandLineParameters.ProjectDirectory, "ServerLogs" ); }
		}

		public static void Init()
		{
			initialized = true;
			Write( "Common", "App initialization" );
		}

		static string GetLogsFileNameForNow( DateTime now )
		{
			return Path.Combine( LogsDirectory, now.ToString( "yyyy-MM-dd" ) + ".log" );
		}

		public static void Write( string group, string text )
		{
			if( !initialized )
				return;

			try
			{
				var now = DateTime.UtcNow;

				var fileName = GetLogsFileNameForNow( now );

				if( !logsDirectoryCreated )
				{
					var directory = Path.GetDirectoryName( fileName );
					if( !Directory.Exists( directory ) )
						Directory.CreateDirectory( directory );
					logsDirectoryCreated = true;
				}

				var max = 16;
				var group2 = group;
				if( group2.Length > max )
					group2 = group2.Substring( 0, max );
				else
					group2 = group2.PadRight( max, ' ' );

				var line = now.ToString( "yyyy-MM-dd-HH-mm-ss-fff" ) + " | " + group2 + " | " + text;

				lock( lockObject )
					File.AppendAllText( fileName, line + "\r\n" );
			}
			catch { }
		}
	}
}
#endif