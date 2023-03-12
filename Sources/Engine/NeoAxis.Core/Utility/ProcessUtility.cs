#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Internal
{
	/// <summary>
	/// Auxiliary class for working with processes.
	/// </summary>
	public static class ProcessUtility
	{
		public static int RunAndWait( string fileName, string arguments, out string result, IDictionary<string, string> environmentVariables = null )
		{
			var startInfo = new ProcessStartInfo( fileName, arguments )
			{
				CreateNoWindow = true,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false
			};

			if( environmentVariables != null )
			{
				foreach( var var in environmentVariables )
					startInfo.EnvironmentVariables.Add( var.Key, var.Value );
			}

			using( Process p = Process.Start( startInfo ) )
			{
				var outputBuilder = new StringBuilder();
				var errorBuilder = new StringBuilder();

				p.ErrorDataReceived += ( s, e ) => errorBuilder.AppendLine( e.Data );
				p.OutputDataReceived += ( s, e ) => outputBuilder.AppendLine( e.Data );
				p.BeginErrorReadLine();
				p.BeginOutputReadLine();

				p.WaitForExit();
				result = outputBuilder.ToString();
				var errors = errorBuilder.ToString();
				if( !string.IsNullOrWhiteSpace( errors ) )
					result += errors;
				return p.ExitCode;
			}
		}

		//internal static async Task<ProcessResult> RunAsync( string fileName, string arguments, int timeout, IDictionary<string, string> environmentVariables = null )
		//{
		//	var result = new ProcessResult();

		//	var startInfo = new ProcessStartInfo( fileName, arguments )
		//	{
		//		CreateNoWindow = true,
		//		RedirectStandardInput = true,
		//		RedirectStandardOutput = true,
		//		RedirectStandardError = true,
		//		UseShellExecute = false
		//	};

		//	if( environmentVariables != null )
		//	{
		//		foreach( var var in environmentVariables )
		//			startInfo.EnvironmentVariables.Add( var.Key, var.Value );
		//	}

		//	using( var process = new Process() { StartInfo = startInfo, EnableRaisingEvents = true } )
		//	{
		//		var outputBuilder = new StringBuilder();
		//		var outputCloseEvent = new TaskCompletionSource<bool>();

		//		process.OutputDataReceived += ( s, e ) =>
		//		{
		//			// output stream closed (the process terminated)
		//			if( string.IsNullOrEmpty( e.Data ) )
		//			{
		//				outputCloseEvent.SetResult( true );
		//			}
		//			else
		//			{
		//				outputBuilder.AppendLine( e.Data );
		//			}
		//		};

		//		var errorBuilder = new StringBuilder();
		//		var errorCloseEvent = new TaskCompletionSource<bool>();

		//		process.ErrorDataReceived += ( s, e ) =>
		//		{
		//			// error stream closed (the process terminated)
		//			if( string.IsNullOrEmpty( e.Data ) )
		//			{
		//				errorCloseEvent.SetResult( true );
		//			}
		//			else
		//			{
		//				errorBuilder.AppendLine( e.Data );
		//			}
		//		};

		//		bool isStarted;

		//		try
		//		{
		//			isStarted = process.Start();
		//		}
		//		catch( Exception exc )
		//		{
		//			// usually it occurs when an executable file is not found or is not executable
		//			result.Completed = true;
		//			result.ExitCode = -1;
		//			result.Output = exc.Message;

		//			isStarted = false;
		//		}

		//		if( isStarted )
		//		{
		//			// reads the output stream first and then waits because deadlocks are possible
		//			process.BeginOutputReadLine();
		//			process.BeginErrorReadLine();

		//			// creates task to wait for process exit using timeout
		//			var waitForExit = WaitForExitAsync( process, timeout );

		//			// create task to wait for process exit and closing all output streams
		//			var processTask = Task.WhenAll( waitForExit, outputCloseEvent.Task, errorCloseEvent.Task );

		//			// waits process completion and then checks it was not completed by timeout
		//			if( await Task.WhenAny( Task.Delay( timeout ), processTask ) == processTask && waitForExit.Result )
		//			{
		//				result.Completed = true;
		//				result.ExitCode = process.ExitCode;

		//				// adds process output if it was completed with error
		//				result.Output = $"{outputBuilder}{errorBuilder}";
		//			}
		//			else
		//			{
		//				try
		//				{
		//					// kill hung process
		//					process.Kill();
		//				}
		//				catch
		//				{
		//				}
		//			}
		//		}
		//	}

		//	return result;
		//}

		//static Task<bool> WaitForExitAsync( Process process, int timeout )
		//{
		//	return Task.Run( () => process.WaitForExit( timeout ) );
		//}

		//public struct ProcessResult
		//{
		//	public bool Completed;
		//	public int? ExitCode;
		//	public string Output;
		//}
	}
}
#endif