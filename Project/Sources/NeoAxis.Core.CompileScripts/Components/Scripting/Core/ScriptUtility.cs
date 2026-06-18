// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NeoAxis
{
	static class ScriptUtility
	{
		public static string FormatCompilationError( IEnumerable<Diagnostic> diagnostics, bool isDebug )
		{
			var failures = diagnostics.Where( d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error );

			var message = new StringBuilder();
			foreach( Diagnostic diagnostic in failures )
			{
				string error_location = "";
				if( diagnostic.Location.IsInSource )
				{
					var error_pos = diagnostic.Location.GetLineSpan().StartLinePosition;

					int error_line = error_pos.Line + 1;
					int error_column = error_pos.Character + 1;

					// the actual source contains an injected '#line' directive f compiled with debug symbols
					if( isDebug )
						error_line--;

					error_location = $"{diagnostic.Location.SourceTree.FilePath}({error_line},{ error_column}): ";
				}
				message.AppendLine( $"{error_location}error {diagnostic.Id}: {diagnostic.GetMessage()}" );
			}
			return message.ToString();
		}
	}
}