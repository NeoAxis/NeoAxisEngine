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
	/// Base class of simple rendering effects.
	/// </summary>
	[EditorSettingsCell( typeof( Component_RenderingEffect_Simple_SettingsCell ) )]
	public class Component_RenderingEffect_Simple : Component_RenderingEffect
	{
		const bool shaderGenerationCompile = true;
		const bool shaderGenerationEnable = true;
		//!!!!
		//const bool shaderGenerationPrintLog = false;
		const bool shaderGenerationPrintLog = true;

		static ESet<Type> shaderParameterTypesSupported;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//Intensity
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
		public event Action<Component_RenderingEffect_Simple> IntensityChanged;

		//!!!!name: ShaderFile
		//!!!!выбирать как ресурс. это типа как Simple effect делать. указать шейдер. чтобы без программирования.
		//!!!!!!!а как параметры? будут из COmponent_Property выставляться?
		//Shader
		//const string shaderDefault = "";
		//const string shaderDefault = @"Base\Shaders\Effects\ColorCorrection.hlsl";
		ReferenceField<string> _shader = @"Base\Shaders\Effects\Simple_fs.sc";
		/// <summary>
		/// The shader associated with the effect.
		/// </summary>
		[Serialize]
		//[DefaultValue( shaderDefault )]
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
		public event Action<Component_RenderingEffect_Simple> ShaderChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a precalculated data of <see cref="Component_RenderingEffect_Simple"/>.
		/// </summary>
		public class CompiledDataSimple : CompiledData
		{
			//!!!!public

			public GpuProgram vertexProgram;
			public GpuProgram fragmentProgram;

			//!!!!temp? надо ли знать о создателе? пока сделано, чтобы параметры материала получить
			public Component_RenderingEffect_Simple owner;

			public string error;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


		//!!!!
		public static bool CanBeShaderParameter( Type type )
		{
			if( shaderParameterTypesSupported == null )
			{
				shaderParameterTypesSupported = new ESet<Type>();

				shaderParameterTypesSupported.Add( typeof( bool ) );
				shaderParameterTypesSupported.Add( typeof( sbyte ) );
				shaderParameterTypesSupported.Add( typeof( byte ) );
				shaderParameterTypesSupported.Add( typeof( char ) );
				shaderParameterTypesSupported.Add( typeof( short ) );
				shaderParameterTypesSupported.Add( typeof( ushort ) );
				shaderParameterTypesSupported.Add( typeof( int ) );
				shaderParameterTypesSupported.Add( typeof( uint ) );
				//shaderParameterTypesSupported.Add( typeof( long ) );
				//shaderParameterTypesSupported.Add( typeof( ulong ) );
				shaderParameterTypesSupported.Add( typeof( float ) );
				shaderParameterTypesSupported.Add( typeof( double ) );

				shaderParameterTypesSupported.Add( typeof( Vector2F ) );
				shaderParameterTypesSupported.Add( typeof( RangeF ) );
				shaderParameterTypesSupported.Add( typeof( Vector3F ) );
				shaderParameterTypesSupported.Add( typeof( Vector4F ) );
				//shaderParameterTypesSupported.Add( typeof( BoundsF ) );
				shaderParameterTypesSupported.Add( typeof( QuaternionF ) );
				shaderParameterTypesSupported.Add( typeof( ColorValue ) );
				shaderParameterTypesSupported.Add( typeof( ColorValuePowered ) );

				shaderParameterTypesSupported.Add( typeof( SphericalDirectionF ) );

				shaderParameterTypesSupported.Add( typeof( Vector2I ) );
				shaderParameterTypesSupported.Add( typeof( Vector3I ) );
				shaderParameterTypesSupported.Add( typeof( Vector4I ) );

				shaderParameterTypesSupported.Add( typeof( RectangleF ) );
				shaderParameterTypesSupported.Add( typeof( RectangleI ) );
				shaderParameterTypesSupported.Add( typeof( DegreeF ) );
				shaderParameterTypesSupported.Add( typeof( RadianF ) );

				shaderParameterTypesSupported.Add( typeof( Vector2 ) );
				shaderParameterTypesSupported.Add( typeof( Range ) );
				shaderParameterTypesSupported.Add( typeof( RangeI ) );
				shaderParameterTypesSupported.Add( typeof( Vector3 ) );
				shaderParameterTypesSupported.Add( typeof( Vector4 ) );
				//shaderParameterTypesSupported.Add( typeof( Bounds ) );

				shaderParameterTypesSupported.Add( typeof( Quaternion ) );
				shaderParameterTypesSupported.Add( typeof( SphericalDirection ) );

				shaderParameterTypesSupported.Add( typeof( Rectangle ) );

				shaderParameterTypesSupported.Add( typeof( Degree ) );
				shaderParameterTypesSupported.Add( typeof( Radian ) );

				shaderParameterTypesSupported.Add( typeof( AnglesF ) );
				shaderParameterTypesSupported.Add( typeof( Angles ) );

				shaderParameterTypesSupported.Add( typeof( Matrix2F ) );
				shaderParameterTypesSupported.Add( typeof( Matrix2 ) );

				shaderParameterTypesSupported.Add( typeof( Matrix3F ) );
				shaderParameterTypesSupported.Add( typeof( Matrix3 ) );

				shaderParameterTypesSupported.Add( typeof( Matrix4F ) );
				shaderParameterTypesSupported.Add( typeof( Matrix4 ) );

				shaderParameterTypesSupported.Add( typeof( PlaneF ) );
				shaderParameterTypesSupported.Add( typeof( Plane ) );
			}

			return shaderParameterTypesSupported.Contains( type ) || typeof( Component_Image ).IsAssignableFrom( type );
		}

		//!!!!
		public static bool CanBeShaderParameter( object obj, Metadata.Property property, out object value )
		{
			if( property.Static )
			{
				value = null;
				return false;
			}
			if( property.HasIndexers )
			{
				value = null;
				return false;
			}
			//!!!!?
			if( !property.Browsable )
			{
				value = null;
				return false;
			}
			//!!!!что еще проверять


			value = property.GetValue( obj, null );
			if( value == null )
				return false;

			var reference = value as IReference;

			////!!!!check
			////Component_Texture default value
			//if( reference != null && !reference.ReferenceSpecified && MetadataManager.GetTypeOfNetType( typeof( Component_Texture ) ).IsAssignableFrom( property.TypeUnreferenced ) )
			//{
			//	//!!!!какую именно возвращать
			//	value = ResourceUtils.WhiteTexture2D;
			//	return true;
			//}

			if( reference != null )
			{
				value = reference.ValueAsObject;
				if( value == null )
				{
					//!!!!invalid reference?
					return false;
				}
			}

			var type = value.GetType();

			//!!!!поддержать все

			if( !CanBeShaderParameter( type ) )
				return false;

			return true;
		}

		public static string ToLowerFirstCharacter( string text )
		{
			if( text.Length > 0 )
				return char.ToLower( text[ 0 ] ).ToString() + text.Substring( 1 );
			return text;
		}

		protected virtual void OnSetShaderParameters( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, Component_Image actualTexture, CanvasRenderer.ShaderItem shader )
		{
		}

		public delegate void SetShaderParametersEventDelegate( Component_RenderingEffect_Simple sender, ViewportRenderingContext context,
			CanvasRenderer.ShaderItem shader );
		public event SetShaderParametersEventDelegate SetShaderParametersEvent;

		protected override void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			var result = Result as CompiledDataSimple;
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

			var size = actualTexture.Result.ResultSize;
			shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );

			//!!!!типы
			//bool, int, uint, dword, half, float, double
			//Vec2, Vec3, Vec4, VecxF, Mat2, 3, 4
			//Texture
			//Structs
			//arrays?
			//Structured Buffer
			//byte buffers

			//set shader parameters by properties
			foreach( var m in MetadataGetMembers() )
			{
				var p = m as Metadata.Property;
				if( p != null )
				{
					if( CanBeShaderParameter( this, p, out object value ) )
					{
						Component_Image texture = value as Component_Image;
						if( texture != null )
						{
							foreach( Component_Image.BindSettingsAttribute attr in
								p.GetCustomAttributes( typeof( Component_Image.BindSettingsAttribute ), true ) )
							{
								shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( attr.SamplerIndex,
									texture, attr.AddressingMode, attr.FilteringMin, attr.FilteringMag, attr.FilteringMip ) );
								break;
							}

							//GpuMaterialPass.TextureParameterValue textureValue = null;

							//int samplerIndex = -1;

							//foreach( Component_Image.BindSettingsAttribute attr in p.GetCustomAttributes(
							//	typeof( Component_Image.BindSettingsAttribute ), true ) )
							//{
							//	textureValue = new GpuMaterialPass.TextureParameterValue( texture,
							//		attr.AddressingMode, attr.FilteringMin, attr.FilteringMag, attr.FilteringMip );
							//	samplerIndex = attr.SamplerIndex;
							//	break;
							//}

							//if( samplerIndex != -1 )
							//	shader.Parameters.Set( samplerIndex.ToString(), textureValue );
						}
						else
						{
							//!!!!
							//convert to float type
							value = SimpleTypes.ConvertDoubleToFloat( value );
							//ColorValuePowered: convert to ColorValue
							if( value is ColorValuePowered )
								value = ( (ColorValuePowered)value ).ToColorValue();

							shader.Parameters.Set( p.Name, value );
							shader.Parameters.Set( ToLowerFirstCharacter( p.Name ), value );
						}
					}
				}
			}

			OnSetShaderParameters( context, frameData, actualTexture, shader );
			SetShaderParametersEvent?.Invoke( this, context, shader );

			context.RenderQuadToCurrentViewport( shader );

			//change actual texture
			context.DynamicTexture_Free( actualTexture );
			actualTexture = outputTexture;
		}

		protected override void OnResultCompile()
		{
			base.OnResultCompile();

			if( !IsSupported )
				return;

			if( Result != null )
				return;

			var result = new CompiledDataSimple();

			//!!!!temp? надо ли знать о создателе? пока сделано, чтобы параметры материала получить
			result.owner = this;

			//!!!!
			EngineThreading.ExecuteFromMainThreadWait( delegate ()
			{
				//generate compile arguments
				var vertexDefines = new List<(string, string)>();
				var fragmentDefines = new List<(string, string)>();
				{
					//var generalDefines = new List<Tuple<string, string>>();
					//generalDefines.Add( Tuple.Create( "LIGHTTYPE_" + lightType.ToString().ToUpper(), "" ) );

					//vertexDefines.AddRange( generalDefines );
					//fragmentDefines.AddRange( generalDefines );
				}

				{
					string shaderFileName = Shader;
					if( string.IsNullOrEmpty( shaderFileName ) )
						return;

					string error2;

					//vertex program
					var vertexProgram = GpuProgramManager.GetProgram( "RenderingEffect_Simple_Vertex_", GpuProgramType.Vertex,
						@"Base\Shaders\EffectsCommon_vs.sc", /*"main_vp", */vertexDefines, out error2 );
					if( !string.IsNullOrEmpty( error2 ) )
					{
						result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
						Log.Warning( result.error );
						result.Dispose();
						return;
					}

					//fragment program
					var fragmentProgram = GpuProgramManager.GetProgram( "RenderingEffect_Simple_Fragment_", GpuProgramType.Fragment,
						shaderFileName, /*"main_fp", */fragmentDefines, out error2 );
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

	}
}
