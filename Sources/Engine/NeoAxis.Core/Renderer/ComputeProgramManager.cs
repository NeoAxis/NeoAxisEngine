// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Text;
using System.Collections.Generic;
using Internal.SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// A class to manage and caching compute programs.
	/// </summary>
	public static class ComputeProgramManager
	{
		static Dictionary<GpuProgram, Program> programs = new Dictionary<GpuProgram, Program>();

		//

		//!!!!always fatal on error?

		public static Program GetProgram( string namePrefix, string sourceFile, ICollection<(string, string)> defines, bool optimize )//, out string error )
		{
			var gpuProgram = GpuProgramManager.GetProgram( new GpuProgramManager.GetProgramItem( namePrefix, GpuProgramType.Compute, sourceFile, defines, optimize ), out var error );

			if( !string.IsNullOrEmpty( error ) )
				Log.Fatal( "ComputeProgramManager: GetProgram: " + error );

			//if( !string.IsNullOrEmpty( error ) )
			//	return new Program();
			//Log.Fatal( "ComputeProgramManager: GetProgram: " + error );

			lock( programs )
			{
				if( !programs.TryGetValue( gpuProgram, out var program ) )
				{
					program = new Program( gpuProgram.RealObject );
					programs[ gpuProgram ] = program;
				}
				return program;
			}
		}
	}
}
