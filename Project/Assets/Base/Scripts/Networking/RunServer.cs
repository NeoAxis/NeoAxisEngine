// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using NeoAxis;

namespace Project
{
	/// <summary>
	/// Implements a functionality of making a server from the Player app.
	/// </summary>
	public static class RunServer
	{
		static Process process;

		/////////////////////////////////////////

		public static bool Running
		{
			get { return process != null; }
		}

		public static Process Process
		{
			get { return process; }
		}

		public static string TextStatus
		{
			get { return Running ? "Running" : "Stopped"; }
		}

		public static bool Start( int port, string password, bool rendering, string sceneName, out string error )
		{
			Stop();

			error = "";

#if WINDOWS
			try
			{
				var fileName = Path.Combine( VirtualFileSystem.Directories.Binaries, "NeoAxis.Player.exe" );

				var arguments = $"-server 1 -networkMode Direct -serverPort {port}";

				if( !string.IsNullOrEmpty( password ) )
				{
					var base64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( password ) );
					arguments += $" -password \"{base64}\"";
				}

				if( !rendering )
					arguments += " -rendererBackend Noop";

				if( !string.IsNullOrEmpty( sceneName ) )
					arguments += string.Format( " -play \"{0}\"", sceneName );

				arguments += " -soundSystem null";

				arguments += " -windowed 1";
				if( !rendering )
					arguments += " -windowState Minimized";
				else
					arguments += " -windowState Normal";
				//arguments += " -windowPosition \"10 10\" -windowSize \"954 754\"";

				process = Process.Start( new ProcessStartInfo( fileName, arguments ) { UseShellExecute = true } );
				//process = Process.Start( fileName, arguments );
			}
			catch( Exception e )
			{
				Stop();

				error = e.Message;
				return false;
			}
#endif

			return true;
		}

		public static void Stop()
		{
#if WINDOWS
			try
			{
				if( process != null )
				{
					//!!!!maybe send signal to app, wait to close by app

					process.Kill( true );

					process = null;
				}
			}
			catch { }
#endif
		}

		public static void ChangeScene( string sceneName )
		{
			//!!!!impl
			//WM_COPYDATA
		}

		public static void Update()
		{
			if( process != null && process.HasExited )
				process = null;
		}
	}
}
