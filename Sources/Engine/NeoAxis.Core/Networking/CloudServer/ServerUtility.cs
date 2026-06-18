#if !NO_SERVER
// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace NeoAxis.CloudServer
{
	/// <summary>
	/// Utility class for the server app.
	/// </summary>
	public static class ServerUtility
	{
		static Task WaitForExitAsync( Process process, CancellationToken cancellationToken = default )
		{
			// If already exited, complete synchronously.
			if( process.HasExited )
				return Task.CompletedTask;

			var tcs = new TaskCompletionSource<object>( TaskCreationOptions.RunContinuationsAsynchronously );

			void Handler( object sender, EventArgs args )
			{
				process.Exited -= Handler;
				tcs.TrySetResult( null );
			}

			process.EnableRaisingEvents = true;
			process.Exited += Handler;

			// Re-check in case it exited between HasExited and subscribing.
			if( process.HasExited )
			{
				process.Exited -= Handler;
				return Task.CompletedTask;
			}

			if( cancellationToken.CanBeCanceled )
			{
				cancellationToken.Register( () =>
				{
					process.Exited -= Handler;
					tcs.TrySetCanceled( cancellationToken );
				} );
			}

			return tcs.Task;
		}

		public async static Task<bool> ExecuteBashCommandAsync( string command, CancellationToken cancellationToken = default )
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

				//!!!!new. test

				// Read streams asynchronously to avoid potential deadlocks with large output.
				var outputTask = process.StandardOutput.ReadToEndAsync();
				var errorTask = process.StandardError.ReadToEndAsync();

				await WaitForExitAsync( process, cancellationToken ).ConfigureAwait( false );

				var output = await outputTask.ConfigureAwait( false );
				var error = await errorTask.ConfigureAwait( false );

				//var output = process.StandardOutput.ReadToEnd();
				//var error = process.StandardError.ReadToEnd();
				//await process.WaitForExitAsync();

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