// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Internal.SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// A class to manage GPU programs.
	/// </summary>
	public static class GpuProgramManager
	{
		static Dictionary<string, GpuProgram> programs = new Dictionary<string, GpuProgram>();
		static Dictionary<(GpuProgram, GpuProgram), GpuLinkedProgram> linkedPrograms = new Dictionary<(GpuProgram, GpuProgram), GpuLinkedProgram>();

		static Dictionary<string, UniformItem> uniforms = new Dictionary<string, UniformItem>();

		/////////////////////////////////////////

		class UniformItem
		{
			public UniformType type;//public Type type;
			public int arraySize;
			public Uniform uniform;
		}

		/////////////////////////////////////////

		public class GetProgramItem
		{
			public string NamePrefix { get; }
			public GpuProgramType Type { get; }
			public string SourceFile { get; }
			/*string entryPoint, */
			public ICollection<(string, string)> DefinesSource { get; }
			public bool Optimize { get; }

			public GpuProgram Program;
			public string Error;

			List<(string, string)> definesOutput;

			//

			public GetProgramItem( string namePrefix, GpuProgramType type, string sourceFile, /*string entryPoint, */ICollection<(string, string)> defines, bool optimize )
			{
				NamePrefix = namePrefix;
				Type = type;
				SourceFile = sourceFile;
				/*string entryPoint, */
				DefinesSource = defines;
				Optimize = optimize;
			}

			internal string GetKey()
			{
				//!!!!
				var builder = new StringBuilder( NamePrefix.Length + SourceFile.Length + 1000 );// + compileArguments.Length + 30 );

				switch( Type )
				{
				case GpuProgramType.Vertex: builder.Append( "V-" ); break;
				case GpuProgramType.Fragment: builder.Append( "F-" ); break;
				case GpuProgramType.Compute: builder.Append( "C-" ); break;
				}
				builder.Append( NamePrefix );
				builder.Append( SourceFile );
				builder.Append( '-' );
				builder.Append( Optimize );
				builder.Append( '-' );

				if( DefinesOutput != null )
				{

					//!!!!GC DefinesOutput

					foreach( var tuple in DefinesOutput )
					{
						builder.Append( tuple.Item1 );
						builder.Append( "[#=]" );
						if( string.IsNullOrEmpty( tuple.Item2 ) )
							builder.Append( "1" );
						else
							builder.Append( tuple.Item2 );
						builder.Append( "[#R]" );
					}
				}

				var result = builder.ToString();
				return result;


				//var compileArguments = "";
				//if( DefinesOutput != null )
				//{
				//	var s = new StringBuilder();
				//	foreach( var tuple in DefinesOutput )
				//	{
				//		s.Append( tuple.Item1 );
				//		s.Append( "[#=]" );
				//		if( string.IsNullOrEmpty( tuple.Item2 ) )
				//			s.Append( "1" );
				//		else
				//			s.Append( tuple.Item2 );
				//		s.Append( "[#R]" );
				//	}
				//	compileArguments = s.ToString();
				//}

				//var builder = new StringBuilder( NamePrefix.Length + SourceFile.Length + compileArguments.Length + 30 );
				//builder.Append( NamePrefix );
				//builder.Append( SourceFile );
				////builder.Append( entryPoint );
				//builder.Append( Type.ToString() );
				////keyBuilder.Append( profiles );
				//builder.Append( compileArguments );
				//builder.Append( Optimize.ToString() );
				////if( replaceStrings != null )
				////{
				////	foreach( KeyValuePair<string, string> replaceString in replaceStrings )
				////		keyBuilder.AppendFormat( "REPLACE{0}{1}", replaceString.Key, replaceString.Value );
				////}

				//return builder.ToString();
			}

			static List<(string, string)> globalDefines;

			List<(string, string)> GetGlobalDefines()
			{
				if( globalDefines == null )
				{
					var list = new List<(string, string)>();

					switch( RenderingSystem.ShadowTechnique )
					{
					case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.Simple:
						list.Add( ("GLOBAL_SHADOW_TECHNIQUE_SIMPLE", "1") );
						break;
					case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.PercentageCloserFiltering4:
						list.Add( ("GLOBAL_SHADOW_TECHNIQUE_PCF4", "1") );
						break;
					case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.PercentageCloserFiltering8:
						list.Add( ("GLOBAL_SHADOW_TECHNIQUE_PCF8", "1") );
						break;
					case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.PercentageCloserFiltering12:
						list.Add( ("GLOBAL_SHADOW_TECHNIQUE_PCF12", "1") );
						break;
					case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.PercentageCloserFiltering16:
						list.Add( ("GLOBAL_SHADOW_TECHNIQUE_PCF16", "1") );
						break;
					case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.PercentageCloserFiltering22:
						list.Add( ("GLOBAL_SHADOW_TECHNIQUE_PCF22", "1") );
						break;
					case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.PercentageCloserFiltering32:
						list.Add( ("GLOBAL_SHADOW_TECHNIQUE_PCF32", "1") );
						break;

						//case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.ContactHardening:
						//	list.Add( ("GLOBAL_SHADOW_TECHNIQUE_CHS", "1") );
						//	break;
						//case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.ExponentialVarianceShadowMaps:
						//	list.Add( ("GLOBAL_SHADOW_TECHNIQUE_EVSM", "1") );
						//	break;
					}

					if( RenderingSystem.DebugMode )
						list.Add( ("GLOBAL_DEBUG_MODE", "1") );

					if( RenderingSystem.LightMask )
						list.Add( ("GLOBAL_LIGHT_MASK", "1") );

					if( RenderingSystem.LightGrid )
						list.Add( ("GLOBAL_LIGHT_GRID", "1") );

					list.Add( ("DISPLACEMENT_MAX_STEPS", RenderingSystem.DisplacementMaxSteps.ToString()) );

					if( RenderingSystem.RemoveTextureTiling )
						list.Add( ("GLOBAL_REMOVE_TEXTURE_TILING", "1") );

					if( RenderingSystem.MotionVector )
						list.Add( ("GLOBAL_MOTION_VECTOR", "1") );

					//if( RenderingSystem.IndirectLightingFullMode )
					//	list.Add( ("GLOBAL_INDIRECT_LIGHTING_FULL_MODE", "1") );

					list.Add( ("GLOBAL_CUT_VOLUME_MAX_AMOUNT", RenderingSystem.CutVolumeMaxAmount.ToString()) );

					if( RenderingSystem.FadeByVisibilityDistance )
						list.Add( ("GLOBAL_FADE_BY_VISIBILITY_DISTANCE", "1") );

					if( RenderingSystem.Fog )
						list.Add( ("GLOBAL_FOG", "1") );

					if( RenderingSystem.SmoothLOD )
						list.Add( ("GLOBAL_SMOOTH_LOD", "1") );

					if( RenderingSystem.NormalMapping )
						list.Add( ("GLOBAL_NORMAL_MAPPING", "1") );

					if( RenderingSystem.SkeletalAnimation )
						list.Add( ("GLOBAL_SKELETAL_ANIMATION", "1") );

					if( RenderingSystem.VoxelLOD )
					{
						list.Add( ("GLOBAL_VOXEL_LOD", "1") );
						list.Add( ("GLOBAL_VOXEL_LOD_MAX_STEPS", RenderingSystem.VoxelLODMaxSteps.ToString()) );
					}

					//if( RenderingSystem.VirtualizedGeometry )
					//{
					//	list.Add( ("GLOBAL_VIRTUALIZED_GEOMETRY", "1") );
					//	//list.Add( ("GLOBAL_VIRTUALIZED_GEOMETRY_MAX_STEPS", RenderingSystem.VirtualizedGeometryMaxSteps.ToString()) );
					//}

					list.Add( ("GLOBAL_MATERIAL_SHADING", ( (int)RenderingSystem.MaterialShading ).ToString()) );

					list.Add( ("SHADOW_TEXTURE_FORMAT_" + RenderingSystem.ShadowTextureFormat.ToString().ToUpper(), "1") );

					if( RenderingSystem.GlobalIllumination )
					{
						list.Add( ("GLOBAL_GI", "1") );
						//!!!!
						//list.Add( ("GLOBAL_VOXEL_LOD_MAX_STEPS", RenderingSystem.VoxelLODMaxSteps.ToString()) );
					}

					if( RenderingSystem.EnvironmentMapMixing )
						list.Add( ("GLOBAL_ENVIRONMENT_MAP_MIXING", "1") );

					//if( RenderingSystem.Tessellation )
					//	list.Add( ("GLOBAL_TESSELLATION", "1") );

					globalDefines = list;
				}
				return globalDefines;
			}

			public ICollection<(string, string)> DefinesOutput
			{
				get
				{
					if( definesOutput == null )
					{
						var list = new List<(string, string)>( 32 );

						//shader type
						list.Add( (Type.ToString().ToUpper(), "1") );

						//global settings
						list.AddRange( GetGlobalDefines() );

						//{
						//	switch( RenderingSystem.ShadowTechnique )
						//	{
						//	case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.Simple:
						//		list.Add( ("GLOBAL_SHADOW_TECHNIQUE_SIMPLE", "1") );
						//		break;
						//	case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.PercentageCloserFiltering4:
						//		list.Add( ("GLOBAL_SHADOW_TECHNIQUE_PCF4", "1") );
						//		break;
						//	case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.PercentageCloserFiltering8:
						//		list.Add( ("GLOBAL_SHADOW_TECHNIQUE_PCF8", "1") );
						//		break;
						//	case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.PercentageCloserFiltering12:
						//		list.Add( ("GLOBAL_SHADOW_TECHNIQUE_PCF12", "1") );
						//		break;
						//	case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.PercentageCloserFiltering16:
						//		list.Add( ("GLOBAL_SHADOW_TECHNIQUE_PCF16", "1") );
						//		break;
						//		//case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.ContactHardening:
						//		//	list.Add( ("GLOBAL_SHADOW_TECHNIQUE_CHS", "1") );
						//		//	break;
						//		//case ProjectSettingsPage_Rendering.ShadowTechniqueEnum.ExponentialVarianceShadowMaps:
						//		//	list.Add( ("GLOBAL_SHADOW_TECHNIQUE_EVSM", "1") );
						//		//	break;
						//	}

						//	if( RenderingSystem.DebugMode )
						//		list.Add( ("GLOBAL_DEBUG_MODE", "1") );

						//	if( RenderingSystem.LightMask )
						//		list.Add( ("GLOBAL_LIGHT_MASK_SUPPORT", "1") );

						//	list.Add( ("DISPLACEMENT_MAX_STEPS", RenderingSystem.DisplacementMaxSteps.ToString()) );

						//	if( RenderingSystem.RemoveTextureTiling )
						//		list.Add( ("GLOBAL_REMOVE_TEXTURE_TILING", "1") );

						//	if( RenderingSystem.MotionVector )
						//		list.Add( ("GLOBAL_MOTION_VECTOR", "1") );

						//	//if( RenderingSystem.IndirectLightingFullMode )
						//	//	list.Add( ("GLOBAL_INDIRECT_LIGHTING_FULL_MODE", "1") );

						//	list.Add( ("GLOBAL_CUT_VOLUME_MAX_AMOUNT", RenderingSystem.CutVolumeMaxAmount.ToString()) );

						//	if( RenderingSystem.FadeByVisibilityDistance )
						//		list.Add( ("GLOBAL_FADE_BY_VISIBILITY_DISTANCE", "1") );

						//	if( RenderingSystem.Fog )
						//		list.Add( ("GLOBAL_FOG", "1") );

						//	if( RenderingSystem.SmoothLOD )
						//		list.Add( ("GLOBAL_SMOOTH_LOD", "1") );

						//	if( RenderingSystem.NormalMapping )
						//		list.Add( ("GLOBAL_NORMAL_MAPPING", "1") );

						//	if( RenderingSystem.SkeletalAnimation )
						//		list.Add( ("GLOBAL_SKELETAL_ANIMATION", "1") );

						//	if( RenderingSystem.VoxelLOD )
						//	{
						//		list.Add( ("GLOBAL_VOXEL_LOD", "1") );
						//		list.Add( ("GLOBAL_VOXEL_LOD_MAX_STEPS", RenderingSystem.VoxelLODMaxSteps.ToString()) );
						//	}

						//	//if( RenderingSystem.VirtualizedGeometry )
						//	//{
						//	//	list.Add( ("GLOBAL_VIRTUALIZED_GEOMETRY", "1") );
						//	//	//list.Add( ("GLOBAL_VIRTUALIZED_GEOMETRY_MAX_STEPS", RenderingSystem.VirtualizedGeometryMaxSteps.ToString()) );
						//	//}

						//	list.Add( ("GLOBAL_MATERIAL_SHADING", ( (int)RenderingSystem.MaterialShading ).ToString()) );

						//	list.Add( ("SHADOW_TEXTURE_FORMAT_" + RenderingSystem.ShadowTextureFormat.ToString().ToUpper(), "1") );
						//}

						if( DefinesSource != null )
							list.AddRange( DefinesSource );

						definesOutput = list;
					}

					return definesOutput;
				}
			}
		}

		/////////////////////////////////////////

		internal static void Init()
		{
		}

		internal static void Shutdown()
		{
			lock( linkedPrograms )
			{
				foreach( var linkedProgram in linkedPrograms.Values.ToArray() )
					linkedProgram._Dispose();
				linkedPrograms.Clear();
			}

			lock( programs )
			{
				foreach( var program in programs.Values.ToArray() )
					program._Dispose();
				programs.Clear();
			}
		}

		static byte[] GetShaderCompiledCode( GpuProgramType type, string sourceFile, ICollection<(string, string)> defines, bool optimize, out string error )
		{
			error = "";

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
			if( Bgfx.GetCurrentBackend() == RendererBackend.Direct3D12 )
				model = ShaderCompiler.ShaderModel.DX12;
			else if( Bgfx.GetCurrentBackend() == RendererBackend.Direct3D11 )
				model = ShaderCompiler.ShaderModel.DX11_SM5;
			else if( Bgfx.GetCurrentBackend() == RendererBackend.OpenGLES )
				model = ShaderCompiler.ShaderModel.OpenGLES;
			else if( Bgfx.GetCurrentBackend() == RendererBackend.Vulkan )
				model = ShaderCompiler.ShaderModel.Vulkan;
			else
				Log.Fatal( "GpuProgramManager: Shader model is not specified. Bgfx.GetCurrentBackend() == {0}.", Bgfx.GetCurrentBackend() );

			ShaderCompiler.Compile( model, (ShaderCompiler.ShaderType)type, realSourceFile, varyingFile, defines, optimize, out var data, out error );
			if( !string.IsNullOrEmpty( error ) )
			{
				//error = string.Format( "Shader file \'{0}\'.\n\n", sourceFile ) + error2;
				//error = string.Format( "Unable to compile shader \'{0}\'.\n\n", sourceFile ) + error2;
				return null;
			}

			return data;
		}

		public static GpuProgram GetProgram( GetProgramItem item, out string error )
		//public static GpuProgram GetProgram( string namePrefix, GpuProgramType type, string sourceFile, /*string entryPoint, */ICollection<(string, string)> defines, bool optimize, out string error )
		{
			error = "";

			if( Bgfx.GetCurrentBackend() == RendererBackend.Noop )
			{
				lock( programs )
					return new GpuProgram( item.Type, new Shader() );
			}

			var key = item.GetKey();

			//LongOperationCallbackManager.CallCallback( "GpuProgramCacheManager: AddProgram" );

			GpuProgram program;
			lock( programs )
				programs.TryGetValue( key, out program );

			if( program == null )
			{
				var data = GetShaderCompiledCode( item.Type, item.SourceFile, item.DefinesOutput, item.Optimize, out var error2 );
				if( data == null )
				{
					error = string.Format( "Shader file \'{0}\'\r", item.SourceFile );
					error += string.Format( "Type \'{0}\'\r\r", item.Type );

					error += "Defines:\r";
					if( item.DefinesOutput != null )
					{
						foreach( var define in item.DefinesOutput )
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

				lock( programs )
				{
					var realObject = new Shader( MemoryBlock.FromArray( data ) );
					program = new GpuProgram( item.Type, realObject );

					programs.Add( key, program );
				}
			}

			return program;
		}

		public static GpuProgram GetProgram( string namePrefix, GpuProgramType type, string sourceFile, /*string entryPoint, */ICollection<(string, string)> defines, bool optimize, out string error )
		{
			return GetProgram( new GetProgramItem( namePrefix, type, sourceFile, defines, optimize ), out error );
		}

		public static void GetPrograms( ICollection<GetProgramItem> items )
		{
			//remove equal items
			var items2 = new Dictionary<string, GetProgramItem>( items.Count );
			foreach( var item in items )
				items2[ item.GetKey() ] = item;

#if DEBUG
			foreach( var item in items2.Values )
				GetProgram( item, out item.Error );
#else
			Parallel.ForEach( items2.Values.ToArray(), delegate ( GetProgramItem item )
			{
				GetProgram( item, out item.Error );
				//GetProgram( item, out _ );
			} );
#endif
		}

		public static GpuLinkedProgram GetLinkedProgram( GpuProgram vertexProgram, GpuProgram fragmentProgram )
		{
			if( vertexProgram == null )
				Log.Fatal( "GpuProgramManager: GetLinkedProgram: vertexProgram == null." );
			if( fragmentProgram == null )
				Log.Fatal( "GpuProgramManager: GetLinkedProgram: fragmentProgram == null." );

			lock( linkedPrograms )
			{
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
		}

		public static Uniform RegisterUniform( string name, UniformType type, int arraySize )
		{
			lock( uniforms )
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

		public static GpuProgram[] GetPrograms()
		{
			lock( programs )
				return programs.Values.ToArray();
		}

		public static int ProgramCount
		{
			get
			{
				lock( programs )
					return programs.Values.Count;
			}
		}

		public static GpuLinkedProgram[] GetLinkedPrograms()
		{
			lock( linkedPrograms )
				return linkedPrograms.Values.ToArray();
		}

		public static int LinkedProgramCount
		{
			get
			{
				lock( linkedPrograms )
					return linkedPrograms.Values.Count;
			}
		}
	}
}
