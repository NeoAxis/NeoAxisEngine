// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	static class ShaderCompiler
	{
		static bool libraryLoaded;

		/////////////////////////////////////////

		struct Wrapper
		{
			//#if DEBUG
			//			public const string library = "shaderc_debug";
			//#else
			public const string library = "shaderc";
			//#endif
			public const CallingConvention convention = CallingConvention.Cdecl;
		}

		/////////////////////////////////////////

		[DllImport( Wrapper.library, EntryPoint = "FreeOutString", CallingConvention = Wrapper.convention )]
		unsafe static extern void FreeOutString( IntPtr pointer );

		static string GetOutString( IntPtr pointer )
		{
			if( pointer != IntPtr.Zero )
			{
				string result = Marshal.PtrToStringUni( pointer );
				FreeOutString( pointer );
				return result;
			}
			else
				return null;
		}

		/////////////////////////////////////////

		[DllImport( Wrapper.library, EntryPoint = "ShaderC_New", CallingConvention = OgreWrapper.convention )]
		unsafe static extern IntPtr New( ShaderType shaderType, ShaderModel shaderModel, [MarshalAs( UnmanagedType.LPWStr )] string shaderFile,
			[MarshalAs( UnmanagedType.LPWStr )] string varyingFile, [MarshalAs( UnmanagedType.U1 )] bool optimize );

		[DllImport( Wrapper.library, EntryPoint = "ShaderC_Delete", CallingConvention = OgreWrapper.convention )]
		unsafe static extern void Delete( IntPtr instance );

		[DllImport( Wrapper.library, EntryPoint = "ShaderC_AddDefine", CallingConvention = OgreWrapper.convention )]
		unsafe static extern void AddDefine( IntPtr instance, [MarshalAs( UnmanagedType.LPWStr )] string name, [MarshalAs( UnmanagedType.LPWStr )] string value );

		[DllImport( Wrapper.library, EntryPoint = "ShaderC_Compile", CallingConvention = OgreWrapper.convention )]
		[return: MarshalAs( UnmanagedType.U1 )]
		unsafe static extern bool Compile( IntPtr instance );

		[DllImport( Wrapper.library, EntryPoint = "ShaderC_GetError", CallingConvention = OgreWrapper.convention )]
		unsafe static extern IntPtr GetError( IntPtr instance );

		[DllImport( Wrapper.library, EntryPoint = "ShaderC_GetResult", CallingConvention = OgreWrapper.convention )]
		unsafe static extern void GetResult( IntPtr instance, out IntPtr data, out int size );

		/////////////////////////////////////////

		public enum ShaderType
		{
			Vertex,
			Fragment,
			Compute,
		}

		/////////////////////////////////////////

		public enum ShaderModel
		{
			DX11_SM5,
			DX12_SM6,
			OpenGLES,
			Vulkan,
		}

		/////////////////////////////////////////

		public static void Compile( ShaderModel shaderModel, ShaderType shaderType, string shaderFile, string varyingFile, ICollection<(string, string)> defines, bool optimize, out byte[] compiledData, out string error )
		{
			compiledData = null;
			error = "";

#if !NO_LITE_DB
			//try get from shader cache
			if( EngineApp.InitSettings.UseShaderCache )
			{
				if( ShaderCache.GetFromCache( shaderModel, shaderType, VirtualPathUtility.GetVirtualPathByReal( shaderFile ), VirtualPathUtility.GetVirtualPathByReal( varyingFile ), defines, out compiledData ) )
					return;
			}
#endif

			//make sure you use precompiled shaders
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
				Log.Fatal( "Shader compilation on UWP is not supported. Compiled shaders must be already precompiled in the shader cache. Run your scenes on development machine to compile shaders." );

			if( !libraryLoaded )
			{
				NativeUtility.PreloadLibrary( Wrapper.library );
				libraryLoaded = true;
			}

			var instance = New( shaderType, shaderModel, shaderFile, varyingFile, optimize );

			if( defines != null )
			{
				foreach( var define in defines )
				{
					var value = define.Item2;
					if( string.IsNullOrEmpty( value ) )
						value = "1";
					AddDefine( instance, define.Item1, value );
				}
			}

			////!!!!
			//var time = DateTime.Now;


			bool success = Compile( instance );


			////!!!!
			//var d = "";
			//if( defines != null )
			//{
			//	foreach( var define in defines )
			//	{
			//		var value = define.Item2;
			//		if( string.IsNullOrEmpty( value ) )
			//			value = "1";
			//		d += " " + define.Item1 + "=" + value + "; ";
			//	}
			//}
			//Log.Info( ( DateTime.Now - time ).TotalSeconds.ToString() + " " + shaderType.ToString() + " " + System.IO.Path.GetFileName( shaderFile ) + " " + d );


			if( success )
			{
				GetResult( instance, out var data, out var size );
				compiledData = new byte[ size ];
				Marshal.Copy( data, compiledData, 0, size );
			}
			else
				error = GetOutString( GetError( instance ) );

			Delete( instance );

#if !NO_LITE_DB
			//write to shader cache
			if( EngineApp.InitSettings.UseShaderCache && optimize && success )
			{
				ShaderCache.AddToCache( shaderModel, shaderType, VirtualPathUtility.GetVirtualPathByReal( shaderFile ), VirtualPathUtility.GetVirtualPathByReal( varyingFile ), defines, compiledData );
			}
#endif
		}
	}
}
