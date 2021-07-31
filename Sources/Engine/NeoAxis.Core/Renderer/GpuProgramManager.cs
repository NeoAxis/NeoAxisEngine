// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using SharpBgfx;

namespace NeoAxis
{
	//!!!!threading

	/// <summary>
	/// GPU programs manager.
	/// </summary>
	public static class GpuProgramManager
	{
		static Dictionary<string, GpuProgram> programs = new Dictionary<string, GpuProgram>();
		static Dictionary<(GpuProgram, GpuProgram), GpuLinkedProgram> linkedPrograms = new Dictionary<(GpuProgram, GpuProgram), GpuLinkedProgram>();

		//!!!!пока так
		static Dictionary<string, UniformItem> uniforms = new Dictionary<string, UniformItem>();

		/////////////////////////////////////////

		class UniformItem
		{
			public UniformType type;//public Type type;
			public int arraySize;
			public Uniform uniform;
		}

		/////////////////////////////////////////

		internal static void Init()
		{
		}

		internal static void Shutdown()
		{
			foreach( var linkedProgram in linkedPrograms.Values.ToArray() )
				linkedProgram._Dispose();
			linkedPrograms.Clear();

			foreach( var program in programs.Values.ToArray() )
				program._Dispose();
			programs.Clear();
		}

		//!!!!
		//static double totalTime;

		static byte[] GetShaderCompiledCode( GpuProgramType type, string sourceFile, ICollection<(string, string)> defines, out string error )
		{
			error = "";


			//var t = DateTime.Now;


			//shaderc.dll

			var realSourceFile = VirtualPathUtility.GetRealPathByVirtual( sourceFile );

			//varyingdef
			string varyingFile;
			{
				//get varying file name
				var fileName = Path.GetFileName( realSourceFile );
				if( fileName.Contains( "_fs.sc" ) )
					fileName = fileName.Replace( "_fs.sc", "_var.sc" );
				else if( fileName.Contains( "_vs.sc" ) )
					fileName = fileName.Replace( "_vs.sc", "_var.sc" );
				varyingFile = Path.Combine( Path.GetDirectoryName( realSourceFile ), fileName );

				//default 'varying.def.sc'
				if( !File.Exists( varyingFile ) )
					varyingFile = Path.Combine( Path.GetDirectoryName( realSourceFile ), "varying.def.sc" );

				if( !File.Exists( varyingFile ) )
				{
					error = "Varying def file is not exists.";
					//error = string.Format( "Shader file \'{0}\'.\n\n", sourceFile ) + "Varying def file is not exists.";
					//error = string.Format( "Unable to compile shader \'{0}\'.\n\n", sourceFile ) + "Varying def file is not exists.";
					return null;
				}
			}

			ShaderCompiler.ShaderModel model = ShaderCompiler.ShaderModel.DX11_SM5;
			if( Bgfx.GetCurrentBackend() == RendererBackend.Direct3D11 || Bgfx.GetCurrentBackend() == RendererBackend.Direct3D12 )
				model = ShaderCompiler.ShaderModel.DX11_SM5;
			//if( Bgfx.GetCurrentBackend() == RendererBackend.Direct3D12 )
			//	model = ShaderCompiler.ShaderModel.DX12_SM6;
			//else if( Bgfx.GetCurrentBackend() == RendererBackend.Direct3D11 )
			//	model = ShaderCompiler.ShaderModel.DX11_SM5;
			else if( Bgfx.GetCurrentBackend() == RendererBackend.OpenGLES )
				model = ShaderCompiler.ShaderModel.OpenGLES;
			else if( Bgfx.GetCurrentBackend() == RendererBackend.Vulkan )
				model = ShaderCompiler.ShaderModel.Vulkan;
			else
				Log.Fatal( "GpuProgramManager: Shader model is not specified. Bgfx.GetCurrentBackend() == {0}.", Bgfx.GetCurrentBackend() );

			ShaderCompiler.Compile( model, (ShaderCompiler.ShaderType)type, realSourceFile, varyingFile, defines, out var data, out error );
			if( !string.IsNullOrEmpty( error ) )
			{
				//error = string.Format( "Shader file \'{0}\'.\n\n", sourceFile ) + error2;
				//error = string.Format( "Unable to compile shader \'{0}\'.\n\n", sourceFile ) + error2;
				return null;
			}

			//var tt = DateTime.Now - t;
			//totalTime += tt.TotalSeconds;
			//Log.Info( totalTime.ToString() );


			return data;



			//shaderc.exe

			//var realSourceFile = VirtualPathUtils.GetRealPathByVirtual( sourceFile );
			//var processFileName = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, "shaderc.exe" );

			//var tempOutFileName = VirtualPathUtils.GetRealPathByVirtual( "user:_TempShaderCompile.bin" );

			//if( File.Exists( tempOutFileName ) )
			//	File.Delete( tempOutFileName );

			//try
			//{
			//	var arguments = string.Format( "-f \"{0}\"", realSourceFile );
			//	arguments += string.Format( " -o \"{0}\"", tempOutFileName );
			//	arguments += " --platform windows";

			//	//defines
			//	string definesString = "";
			//	if( defines != null )
			//	{
			//		StringBuilder s = new StringBuilder();
			//		bool addSemicolon = false;
			//		foreach( var tuple in defines )
			//		{
			//			if( addSemicolon )
			//				s.Append( ";" );
			//			s.Append( tuple.Item1 );
			//			s.Append( "=" );
			//			if( string.IsNullOrEmpty( tuple.Item2 ) )
			//				s.Append( "1" );
			//			else
			//				s.Append( tuple.Item2 );
			//			addSemicolon = true;
			//		}
			//		definesString = s.ToString();
			//	}
			//	if( !string.IsNullOrEmpty( definesString ) )
			//	{
			//		//!!!!
			//		arguments += " --define \"" + definesString + "\"";
			//	}

			//	arguments += " --type " + type.ToString().ToLower();

			//	//varyingdef
			//	{
			//		var fileName = Path.GetFileName( realSourceFile );
			//		if( fileName.Contains( "_fs.sc" ) )
			//			fileName = fileName.Replace( "_fs.sc", "_var.sc" );
			//		else if( fileName.Contains( "_vs.sc" ) )
			//			fileName = fileName.Replace( "_vs.sc", "_var.sc" );
			//		var varyingFile = Path.Combine( Path.GetDirectoryName( realSourceFile ), fileName );
			//		if( File.Exists( varyingFile ) )
			//			arguments += string.Format( " --varyingdef \"{0}\"", varyingFile );
			//	}

			//	var profile = "";
			//	switch( type )
			//	{
			//	case GpuProgramType.Vertex: profile = "vs_5_0"; break;
			//	case GpuProgramType.Fragment: profile = "ps_5_0"; break;
			//	default: Log.Fatal( "GpuProgramManager: GetShaderCompiledCode: profile impl." ); break;
			//	}
			//	arguments += " -p " + profile;

			//	//!!!!
			//	arguments += " -O " + "0";

			//	var startInfo = new ProcessStartInfo();
			//	startInfo.FileName = processFileName;
			//	startInfo.Arguments = arguments;
			//	startInfo.UseShellExecute = false;
			//	startInfo.RedirectStandardOutput = true;
			//	startInfo.CreateNoWindow = true;

			//	var process = new Process();
			//	process.StartInfo = startInfo;
			//	process.Start();
			//	//process.WaitForExit();

			//	var consoleOutput = "";
			//	while( !process.StandardOutput.EndOfStream )
			//	{
			//		string line = process.StandardOutput.ReadLine();
			//		consoleOutput += line + "\n";
			//	}
			//	Log.Info( consoleOutput );

			//	//var process = Process.Start( processFileName, arguments );
			//	//process.WaitForExit();

			//	var result = File.ReadAllBytes( tempOutFileName );

			//	File.Delete( tempOutFileName );

			//	return result;
			//}
			//catch( Exception e )
			//{
			//	error = e.Message;
			//	return null;
			//}
		}

		public static GpuProgram GetProgram( string namePrefix, GpuProgramType type, string sourceFile, /*string entryPoint, */ICollection<(string, string)> defines, out string error )
		{
			error = "";

			if( Bgfx.GetCurrentBackend() == RendererBackend.Noop )
				return new GpuProgram( type, new Shader() );

			//!!!!
			EngineThreading.CheckMainThread();

			//!!!!
			string compileArguments2 = "";
			if( defines != null )
			{
				StringBuilder s = new StringBuilder();
				foreach( var tuple in defines )
				{
					s.Append( tuple.Item1 );
					s.Append( "[#=]" );
					if( string.IsNullOrEmpty( tuple.Item2 ) )
						s.Append( "1" );
					else
						s.Append( tuple.Item2 );
					s.Append( "[#R]" );
				}
				compileArguments2 = s.ToString();
			}

			//LongOperationCallbackManager.CallCallback( "GpuProgramCacheManager: AddProgram" );

			string key;
			{
				var builder = new StringBuilder( namePrefix.Length + sourceFile.Length + compileArguments2.Length + 30 );
				builder.Append( namePrefix );
				builder.Append( sourceFile );
				//builder.Append( entryPoint );
				builder.Append( type.ToString() );
				//keyBuilder.Append( profiles );
				builder.Append( compileArguments2 );
				//if( replaceStrings != null )
				//{
				//	foreach( KeyValuePair<string, string> replaceString in replaceStrings )
				//		keyBuilder.AppendFormat( "REPLACE{0}{1}", replaceString.Key, replaceString.Value );
				//}

				key = builder.ToString();
			}

			if( !programs.TryGetValue( key, out var program ) )
			{
				var data = GetShaderCompiledCode( type, sourceFile, defines, out var error2 );
				if( data == null )
				{
					error = string.Format( "Shader file \'{0}\'\r", sourceFile );
					error += string.Format( "Type \'{0}\'\r\r", type );

					error += "Defines:\r";
					if( defines != null )
					{
						foreach( var define in defines )
						{
							var value = define.Item2;
							if( string.IsNullOrEmpty( value ) )
								value = "1";
							if( value.Contains( '\r' ) )
								value = "\r" + value;

							error += string.Format( "{0} = {1}\r", define.Item1, value );
						}
					}

					error += "\r" + error2 + "\r";

					return null;
				}

				var realObject = new Shader( MemoryBlock.FromArray( data ) );
				program = new GpuProgram( type, realObject );


				//string programName = _old_GpuProgramManager.Instance.GetUniqueName( namePrefix );

				////Log.InvisibleInfo( string.Format( "Creating gpu program: {0}, {1}, {2}, {3}",
				////   programName, Path.GetFileName( sourceFile ), profiles, compileArguments ) );

				//string language = _old_GpuProgramManager.Instance.IsSyntaxSupported( "hlsl" ) ? "hlsl" : "hlsl2glsl";

				//program = _old_GpuProgramManager.Instance.CreateHighLevelProgram( programName, language, programType );
				//if( program == null )
				//{
				//	errorString = string.Format( "Unable to create high level program with name \"{0}\".", programName );
				//	return null;
				//}

				////!!!!!было. хотя, возможно, полезно для ускорения
				////string source;
				////{
				////	if( !cachedLoadedFiles.TryGetValue( sourceFile, out source ) )
				////	{
				////		try
				////		{
				////			using( Stream stream = VirtualFile.Open( sourceFile ) )
				////			{
				////				using( StreamReader streamReader = new StreamReader( stream ) )
				////				{
				////					source = streamReader.ReadToEnd();
				////				}
				////			}
				////		}
				////		catch( Exception e )
				////		{
				////			Log.Error( "{0} ({1}).", e.Message, sourceFile );
				////			program.Dispose();
				////			return null;
				////		}

				////		cachedLoadedFiles.Add( sourceFile, source );
				////	}
				////}

				////if( replaceStrings != null )
				////{
				////	foreach( KeyValuePair<string, string> replaceString in replaceStrings )
				////		source = source.Replace( replaceString.Key, replaceString.Value );
				////}


				//* from Evgeny

				//				//!!!! Для отладки шейдеров нам нужна ссылка на исходник.
				//				// Нужно ли вообще передавать уже считанный исходник, может быть достаточно имя файла ?
				//#if DEBUG
				//				program.SourceFile = sourceFile;
				//#else
				//				program.Source = source;
				//#endif
				//*/

				////!!!!new. need?
				//program.SourceFile = sourceFile;
				////!!!!было
				////program.Source = source;

				//program.EntryPoint = entryPoint;

				//foreach( string profile in profiles.Split() )
				//{
				//	if( _old_GpuProgramManager.Instance.IsSyntaxSupported( profile ) )
				//	{
				//		program.Target = profile;
				//		break;
				//	}
				//}

				//program.PreprocessorDefines = compileArguments2;

				////Compile program

				//Trace.Assert( lastRendererLogMessages.Count == 0 );
				//RendererWorld._InternalLogMessage += RendererWorld_InternalLogMessage;
				//RendererWorld._InvisibleInternalLogMessages = true;

				//bool error = false;

				//if( !program.Touch() )
				//	error = true;

				//if( program.HasCompileError() || error )
				//{
				//	string text = lastRendererLogMessages[ 0 ];

				//	{
				//		int clipIndex = text.IndexOf( "in D3D11HLSLProgram::loadFromSource" );
				//		if( clipIndex != -1 )
				//			text = text.Substring( 0, clipIndex );
				//	}
				//	{
				//		int clipIndex = text.IndexOf( "in CgProgram::loadFromSource" );
				//		if( clipIndex != -1 )
				//			text = text.Substring( 0, clipIndex );
				//	}

				//	text = text.Trim();

				//	errorString = text;
				//}

				//RendererWorld._InvisibleInternalLogMessages = false;
				//RendererWorld._InternalLogMessage -= RendererWorld_InternalLogMessage;
				//lastRendererLogMessages.Clear();

				//if( program.HasCompileError() || error )
				//{
				//	program.Dispose();
				//	return null;
				//}

				//!!!!!
				////set auto parameters
				//SetProgramAutoConstants( program.DefaultParameters );

				programs.Add( key, program );
			}

			return program;
		}

		public static GpuLinkedProgram GetLinkedProgram( GpuProgram vertexProgram, GpuProgram fragmentProgram )//, out string error )
		{
			//error = "";

			if( vertexProgram == null )
				Log.Fatal( "GpuProgramManager: GetLinkedProgram: vertexProgram == null." );
			if( fragmentProgram == null )
				Log.Fatal( "GpuProgramManager: GetLinkedProgram: fragmentProgram == null." );

			//!!!!
			EngineThreading.CheckMainThread();

			var key = (vertexProgram, fragmentProgram);
			if( !linkedPrograms.TryGetValue( key, out var program ) )
			{
				var programs = new GpuProgram[] { vertexProgram, fragmentProgram };
				var realObject = new Program( vertexProgram.RealObject, fragmentProgram.RealObject );
				program = new GpuLinkedProgram( programs, realObject );
				linkedPrograms.Add( key, program );
			}

			return program;
		}

		//static UniformType ConvertType( Type type )
		//{
		//	if( type == typeof( int ) )
		//		return UniformType.Int1;
		//	if( type == typeof( Vec4F ) )
		//		return UniformType.Vector4;
		//	if( type == typeof( Mat3F ) )
		//		return UniformType.Matrix3x3;
		//	if( type == typeof( Mat4F ) )
		//		return UniformType.Matrix4x4;
		//	Log.Fatal( "GpuBufferManager: ConvertType: Invalid type \'{0}\'.", type );
		//	return UniformType.Int1;
		//}

		////!!!!пока так
		//public Uniform RegisterUniform( string name, Type type, int arraySize )
		//{
		//	if( !uniforms.TryGetValue( name, out var item ) )
		//	{
		//		item = new UniformItem();
		//		item.uniform = new Uniform( name, ConvertType( type ), arraySize );
		//		item.type = type;
		//		item.arraySize = arraySize;
		//		uniforms.Add( name, item );
		//	}

		//	if( item.type != type || item.arraySize != arraySize )
		//		Log.Fatal( "GpuProgramManager: RegisterUniform: Uniforms with same name must have equal type and array size." );

		//	return item.uniform;
		//}

		public static Uniform RegisterUniform( string name, UniformType type, int arraySize )
		{
			if( !uniforms.TryGetValue( name, out var item ) )
			{
				item = new UniformItem();
				item.uniform = new Uniform( name, type, arraySize );
				item.type = type;
				item.arraySize = arraySize;
				uniforms.Add( name, item );
			}

			if( item.type != type || item.arraySize != arraySize )
				Log.Fatal( "GpuProgramManager: RegisterUniform: Uniforms with same name must have equal type and array size. Registered: name \'{0}\'; type \'{1}\'; array size \'{2}\'. Trying to register: type \'{3}\'; array size \'{4}\'", name, item.type, item.arraySize, type, arraySize );

			return item.uniform;
		}

		public static string GetGpuProgramCompilationErrorText( Component component, string gpuProgramError )
		{
			string result = "";
			result += "Unable to compile shader.\r";
			result += "\r";
			result += string.Format( "File name \'{0}\'\r", ComponentUtility.GetOwnedFileNameOfComponent( component ) );
			result += string.Format( "Component \'{0}\'\r", component );
			result += gpuProgramError + "\r";

			result = result.Replace( "\r", "\r\n" );

			return result;
		}

		public static ICollection<GpuProgram> Programs
		{
			get { return programs.Values; }
		}

		public static ICollection<GpuLinkedProgram> LinkedPrograms
		{
			get { return linkedPrograms.Values; }
		}
	}
}
