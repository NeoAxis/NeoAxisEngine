#if !NO_SERVER
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NeoAxis.CloudServer
{
	/// <summary>
	/// Utility class for the server app.
	/// </summary>
	public static class ServerUtility
	{
		public async static Task<bool> ExecuteBashCommandAsync( string command )
		{
			var processInfo = new ProcessStartInfo
			{
				FileName = "/bin/bash",
				Arguments = $"-c \"{command}\"",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using( var process = Process.Start( processInfo ) )
			{
				var output = process.StandardOutput.ReadToEnd();
				var error = process.StandardError.ReadToEnd();
				await process.WaitForExitAsync();

				if( process.ExitCode != 0 )
				{
					Console.WriteLine( $"Error: {error}" );
					return false;
				}
				else
				{
					Console.WriteLine( output );
					return true;
				}
			}
		}

		public static bool ExecuteBashCommand( string command )
		{
			var processInfo = new ProcessStartInfo
			{
				FileName = "/bin/bash",
				Arguments = $"-c \"{command}\"",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using( var process = Process.Start( processInfo ) )
			{
				var output = process.StandardOutput.ReadToEnd();
				var error = process.StandardError.ReadToEnd();
				process.WaitForExit();

				if( process.ExitCode != 0 )
				{
					Console.WriteLine( $"Error: {error}" );
					return false;
				}
				else
				{
					Console.WriteLine( output );
					return true;
				}
			}
		}
	}
}
#endif