// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The component for creation of the fullscreen rendering effect.
	/// </summary>
	[EditorNewObjectSettings( typeof( NewObjectSettingsEffect ) )]
	[EditorSettingsCell( typeof( Component_RenderingEffect_CodeGenerated_SettingsCell ) )]
	public class Component_RenderingEffect_CodeGenerated : Component_RenderingEffect, IComponent_EditorUpdateWhenDocumentModified
	{
		const bool shaderGenerationCompile = true;
		const bool shaderGenerationEnable = true;
		//!!!!
		const bool shaderGenerationPrintLog = false;
		//const bool shaderGenerationPrintLog = true;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		ReferenceField<double> _intensity = 1;
		/// <summary>
		/// The intensity of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Category( "Effect" )]
		[Range( 0, 1 )]
		public Reference<double> Intensity
		{
			get
			{
				if( _intensity.BeginGet() )
					Intensity = _intensity.Get( this );
				return _intensity.value;
			}
			set
			{
				if( _intensity.BeginSet( ref value ) )
				{
					try { IntensityChanged?.Invoke( this ); }
					finally { _intensity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_CodeGenerated> IntensityChanged;

		//!!!!name: ShaderFile
		//!!!!выбирать как ресурс. это типа как Simple effect делать. указать шейдер. чтобы без программирования.
		//!!!!!!!а как параметры? будут из COmponent_Property выставляться?
		//Shader
		const string shaderDefault = @"Base\Shaders\Effects\CodeGenerated_fs.sc";
		ReferenceField<string> _shader = shaderDefault;
		/// <summary>
		/// The shader associated with the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( shaderDefault )]
		[Category( "Effect" )]
		public Reference<string> Shader
		{
			get
			{
				if( _shader.BeginGet() )
					Shader = _shader.Get( this );
				return _shader.value;
			}
			set
			{
				if( _shader.BeginSet( ref value ) )
				{
					try
					{
						ShaderChanged?.Invoke( this );
						//!!!!update? где еще так
					}
					finally { _shader.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Shader"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_CodeGenerated> ShaderChanged;

		//Color
		ReferenceField<ColorValue> _color = new ColorValue( 1, 1, 1 );
		/// <summary>
		/// Resulting color of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> Color
		{
			get
			{
				if( _color.BeginGet() )
					Color = _color.Get( this );
				return _color.value;
			}
			set
			{
				if( _color.BeginSet( ref value ) )
				{
					try { ColorChanged?.Invoke( this ); }
					finally { _color.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_CodeGenerated> ColorChanged;

		[Serialize]
		[Browsable( false )]
		[DefaultValue( true )]
		public bool EditorAutoUpdate { get; set; } = true;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a precalculated data of <see cref="Component_RenderingEffect_CodeGenerated"/>.
		/// </summary>
		public class CompiledDataCodeGenerated : CompiledData
		{
			//!!!!public

			public ShaderGenerator.ResultData vertexGeneratedCode;
			public ShaderGenerator.ResultData fragmentGeneratedCode;
			public GpuProgram vertexProgram;
			public GpuProgram fragmentProgram;

			//!!!!temp? надо ли знать о создателе? пока сделано, чтобы параметры материала получить
			public Component_RenderingEffect_CodeGenerated owner;

			public string error;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// A set of settings for creation <see cref="Component_RenderingEffect_CodeGenerated"/> in the editor.
		/// </summary>
		public class NewObjectSettingsEffect : NewObjectSettings
		{
			[DefaultValue( true )]
			[Category( "Options" )]
			[DisplayName( "Shader graph" )]
			public bool ShaderGraph { get; set; } = true;

			public override bool Creation( NewObjectCell.ObjectCreationContext context )
			{
				var newObject2 = (Component_RenderingEffect_CodeGenerated)context.newObject;

				if( ShaderGraph )
					newObject2.NewObjectCreateShaderGraph();

				return base.Creation( context );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			var result = Result as CompiledDataCodeGenerated;
			//!!!!так?
			if( result == null )
				return;

			if( result.vertexProgram == null || result.fragmentProgram == null )
				return;

			//!!!!валится если файла нет. или не указан.
			//!!!!!!как быстро проверять о том что файла нет? может из кеша шейдеров или типа того. как тогда обновлять
			//string shaderFile = Shader;
			//if( string.IsNullOrEmpty( shaderFile ) )
			//	return;

			//!!!!formats. т.е. эффект поддерживает и LDR и HDR. где еще
			//!!!!!можно по сути у эффекта указывать что он ожидает получить на входе. ну или это EffectsHDR, EffectsLDR
			var outputTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );

			context.SetViewport( outputTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			shader.CompiledVertexProgram = result.vertexProgram;
			shader.CompiledFragmentProgram = result.fragmentProgram;

			//shader.VertexProgramFileName = shaderFile;
			//shader.VertexProgramFunctionName = "main_vp";
			//shader.FragmentProgramFileName = shaderFile;
			//shader.FragmentProgramFunctionName = "main_fp";

			//{
			//	var defines = new List<GuiRenderer.ShaderItem.DefineItem>();

			//	//defines.Add( new GuiRenderer.ShaderItem.DefineItem( "FRAGMENT_CODE_BODY", "color = float4(0,1,0,1);" ) );

			//	if( defines.Count != 0 )
			//		shader.Defines = defines.ToArray();
			//}

			shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/,
				actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
			//shader.Parameters.Set( "0"/*"sourceTexture"*/, new GpuMaterialPass.TextureParameterValue( actualTexture,
			//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

			unsafe
			{
				Vector4F value = Vector4F.One;
				if( !Color.ReferenceSpecified )
					value = Color.Value.ToVector4F();
				shader.Parameters.Set( "u_paramColor", ParameterType.Vector4, 1, &value, sizeof( Vector4F ) );
			}

			//!!!!пока так
			if( result.fragmentGeneratedCode != null )
			{
				//!!!!copy code из MaterialStandard

				//var materialParameters = materialData.GetShaderParameters();
				var code = result.fragmentGeneratedCode;

				//foreach( var code in codes )
				//{
				if( code.parameters != null )
				{
					foreach( var item in code.parameters )
					{
						//!!!!надо ссылки сконвертить или как-то так? чтобы работало, если это в типе

						var value = item.component.GetValue();
						if( value != null )
						{
							//!!!!
							//convert to float type
							value = SimpleTypes.ConvertDoubleToFloat( value );
							//ColorValuePowered: convert to ColorValue
							if( value is ColorValuePowered )
								value = ( (ColorValuePowered)value ).ToColorValue();

							shader.Parameters/*container*/.Set( item.nameInShader, value );
						}
					}
				}

				if( code.textures != null )
				{
					foreach( var item in code.textures )
					{
						//!!!!надо ссылки сконвертить или как-то так? чтобы работало, если это в типе

						//!!!!не только 2D

						Component_Image texture = item.component.Texture;
						if( texture == null )
							texture = ResourceUtility.WhiteTexture2D;

						//!!!!options

						shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( item.textureRegister,
							texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

						////!!!!
						//var textureValue = new GpuMaterialPass.TextureParameterValue( texture,
						//	TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
						////!!!!
						//ParameterType parameterType = ParameterType.Texture2D;

						//shader.Parameters/*container*/.Set( item.textureRegister.ToString(), textureValue, parameterType );
						////shader.Parameters/*container*/.Set( item.nameInShader, textureValue, parameterType );
					}
				}
				//}
			}

			var size = actualTexture.Result.ResultSize;
			shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );

			shader.Parameters.Set( "intensity", (float)Intensity.Value );

			//!!!!auto parameters
			//if( result.fragmentGeneratedCode != null )
			{
				//!!!!не так потом

				shader.Parameters.Set( "param_viewportSize", new Vector2F( size.X, size.Y ) );

				//foreach( var item in result.fragmentGeneratedCode.autoConstantParameters )
				//{
				//}
			}

			context.RenderQuadToCurrentViewport( shader );

			//change actual texture
			context.DynamicTexture_Free( actualTexture );
			actualTexture = outputTexture;
		}

		string GetDisplayName()
		{
			//!!!!полный путь получать?
			string displayName;
			if( HierarchyController != null && HierarchyController.CreatedByResource != null )
				displayName = HierarchyController.CreatedByResource.Owner.Name;
			else
				displayName = Name;
			return displayName;
		}

		bool GenerateCode( CompiledDataCodeGenerated compiledData )
		{
			////vertex
			//{
			//	var properties = new List<string>();
			//	properties.Add( "PositionOffset" );

			//	var generator = new ShaderGenerator();
			//	var code = generator.Process( this, properties, "vertex_", out string error );

			//	//process error
			//	if( !string.IsNullOrEmpty( error ) )
			//	{
			//		//!!!!

			//		return false;
			//	}

			//	//print to log
			//	if( code != null && shaderGenerationPrintLog )
			//		code.PrintToLog( GetDisplayName() + ", Vertex shader" );

			//	compiledData.vertexGeneratedCode = code;
			//}

			//fragment
			{
				var properties = new List<(Component, Metadata.Property)>();
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Color ) )) );

				var generator = new ShaderGenerator();
				int textureRegisterCounter = 1;
				var code = generator.Process( properties, "fragment_", ref textureRegisterCounter, out string error );

				//process error
				if( !string.IsNullOrEmpty( error ) )
				{
					//!!!!

					return false;
				}

				//print to log
				if( code != null && shaderGenerationPrintLog )
					code.PrintToLog( GetDisplayName() + ", Fragment shader" );

				compiledData.fragmentGeneratedCode = code;
			}

			return true;
		}

		protected override void OnResultCompile()
		{
			base.OnResultCompile();

			if( !IsSupported )
				return;

			if( Result != null )
				return;

			var result = new CompiledDataCodeGenerated();

			//!!!!temp? надо ли знать о создателе? пока сделано, чтобы параметры материала получить
			result.owner = this;

			//!!!!
			EngineThreading.ExecuteFromMainThreadWait( delegate ()
			{
				//shader generation
				if( shaderGenerationCompile )
				{
					if( !GenerateCode( result ) )
						return;
				}

				//generate compile arguments
				var vertexDefines = new List<(string, string)>();
				var fragmentDefines = new List<(string, string)>();
				{
					//var generalDefines = new List<(string, string)>();
					//generalDefines.Add( ( "LIGHTTYPE_" + lightType.ToString().ToUpper(), "" ) );

					//vertexDefines.AddRange( generalDefines );
					//fragmentDefines.AddRange( generalDefines );

					if( shaderGenerationEnable )
					{
						////vertex
						//var vertexCode = result.vertexGeneratedCode;
						//if( vertexCode != null )
						//{
						//	if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
						//		vertexDefines.Add( ( "VERTEX_CODE_PARAMETERS", vertexCode.parametersBody ) );
						//	if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
						//		vertexDefines.Add( ( "VERTEX_CODE_SAMPLERS", vertexCode.samplersBody ) );
						//	if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
						//		vertexDefines.Add( ( "VERTEX_CODE_BODY", vertexCode.codeBody ) );
						//}

						//fragment
						var fragmentCode = result.fragmentGeneratedCode;
						if( fragmentCode != null )
						{
							if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
								fragmentDefines.Add( ("FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody) );
							if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
								fragmentDefines.Add( ("FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody) );
							if( !string.IsNullOrEmpty( fragmentCode.shaderScripts ) )
								fragmentDefines.Add( ("FRAGMENT_CODE_SHADER_SCRIPTS", fragmentCode.shaderScripts) );
							if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
								fragmentDefines.Add( ("FRAGMENT_CODE_BODY", fragmentCode.codeBody) );
						}
					}
				}

				{
					string shaderFileName = Shader;
					if( string.IsNullOrEmpty( shaderFileName ) )
						return;

					string error2;

					//vertex program
					var vertexProgram = GpuProgramManager.GetProgram( "RenderingEffect_CodeGenerated_Vertex_",
						GpuProgramType.Vertex, @"Base\Shaders\EffectsCommon_vs.sc", vertexDefines, out error2 );
					if( !string.IsNullOrEmpty( error2 ) )
					{
						result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
						Log.Warning( result.error );
						result.Dispose();
						return;
					}

					//fragment program
					var fragmentProgram = GpuProgramManager.GetProgram( "RenderingEffect_CodeGenerated_Fragment_",
						GpuProgramType.Fragment, shaderFileName, fragmentDefines, out error2 );
					if( !string.IsNullOrEmpty( error2 ) )
					{
						result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
						Log.Warning( result.error );
						result.Dispose();
						return;
					}

					result.vertexProgram = vertexProgram;
					result.fragmentProgram = fragmentProgram;

					//standard material architecture:
					//var pass = GpuMaterialPass.CreatePass( vertexProgram, fragmentProgram );
					//result.AllPasses.Add( pass );
					//pass.SourceBlendFactor = SceneBlendFactor.One;
					//pass.DestBlendFactor = SceneBlendFactor.Zero;
				}

			} );

			Result = result;
		}

		void NewObjectCreateShaderGraph()
		{
			var graph = CreateComponent<Component_FlowGraph>();
			graph.Name = "Shader graph";
			graph.Specialization = ReferenceUtility.MakeReference(
				MetadataManager.GetTypeOfNetType( typeof( Component_FlowGraphSpecialization_Shader ) ).Name + "|Instance" );

			var node = graph.CreateComponent<Component_FlowGraphNode>();
			node.Name = "Node " + Name;
			node.Position = new Vector2I( 10, -7 );
			node.ControlledObject = ReferenceUtility.MakeThisReference( node, this );

			if( Parent == null )
			{
				var toSelect = new Component[] { this, graph };
				EditorDocumentConfiguration = KryptonConfigGenerator.CreateEditorDocumentXmlConfiguration( toSelect, graph );
			}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow )
		{
			if( !createdFromNewObjectWindow )
				NewObjectCreateShaderGraph();
		}

		public void EditorUpdateWhenDocumentModified()
		{
			if( EditorAutoUpdate )
				ResultCompile();
		}
	}
}
