// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using NeoAxis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Internal
{
	/// <summary>
	/// Auxiliary class for working with processes.
	/// </summary>
	public static class ProcessUtility
	{
		public static int RunAndWait( string fileName, string arguments, out string result, IDictionary<string, string> environmentVariables = null, Action<string> errorDataReceivedCallback = null, Action<string> outputDataReceivedCallback = null )
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

				p.ErrorDataReceived += delegate ( object sender, DataReceivedEventArgs e )
				{
					if( e.Data != null )
					{
						errorBuilder.AppendLine( e.Data );
						if( errorDataReceivedCallback != null )
							errorDataReceivedCallback( e.Data );
					}
				};

				p.OutputDataReceived += delegate ( object sender, DataReceivedEventArgs e )
				{
					if( e.Data != null )
					{
						outputBuilder.AppendLine( e.Data );
						if( outputDataReceivedCallback != null )
							outputDataReceivedCallback( e.Data );
					}
				};

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

		public class ProcessResult
		{
			public int ExitCode;
			public string Output;
		}

		public static async Task<ProcessResult> RunAndWaitAsync( string fileName, string arguments, IDictionary<string, string> environmentVariables = null, Action<string> errorDataReceivedCallback = null, Action<string> outputDataReceivedCallback = null, CancellationToken cancellationToken = default )
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

				p.ErrorDataReceived += delegate ( object sender, DataReceivedEventArgs e )
				{
					if( e.Data != null )
					{
						errorBuilder.AppendLine( e.Data );
						if( errorDataReceivedCallback != null )
							errorDataReceivedCallback( e.Data );
					}
				};

				p.OutputDataReceived += delegate ( object sender, DataReceivedEventArgs e )
				{
					if( e.Data != null )
					{
						outputBuilder.AppendLine( e.Data );
						if( outputDataReceivedCallback != null )
							outputDataReceivedCallback( e.Data );
					}
				};

				p.BeginErrorReadLine();
				p.BeginOutputReadLine();

				await p.WaitForExitAsync( cancellationToken );
				if( cancellationToken.IsCancellationRequested )
				{
					try
					{
						p.Kill( true );
					}
					catch { }
					cancellationToken.ThrowIfCancellationRequested();
				}

				var result = new ProcessResult();
				result.ExitCode = p.ExitCode;
				result.Output = outputBuilder.ToString();
				var errors = errorBuilder.ToString();
				if( !string.IsNullOrWhiteSpace( errors ) )
					result.Output += errors;

				return result;
			}
		}
	}
}