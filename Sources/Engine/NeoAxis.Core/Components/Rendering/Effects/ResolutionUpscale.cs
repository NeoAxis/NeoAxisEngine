// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// The effect for adjusting the resolution upscale.
	/// </summary>
	[DefaultOrderOfEffect( 5.5 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_ResolutionUpscale : RenderingEffect
	{
		///// <summary>
		///// The intensity of the effect.
		///// </summary>
		//[Serialize]
		//[DefaultValue( 1.0 )]
		//[Range( 0, 1 )]
		//[Category( "Effect" )]
		//public Reference<double> Intensity
		//{
		//	get { if( _intensity.BeginGet() ) Intensity = _intensity.Get( this ); return _intensity.value; }
		//	set { if( _intensity.BeginSet( ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		//public event Action<RenderingEffect_ResolutionUpscale> IntensityChanged;
		//ReferenceField<double> _intensity = 1;

		public enum ModeEnum
		{
			Auto,
			Original,
			UltraQuality,
			Quality,
			Balanced,
			Performance
		}

		/// <summary>
		/// The quality mode of the effect.
		/// </summary>
		[DefaultValue( ModeEnum.Auto )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set { if( _mode.BeginSet( ref value ) ) { try { ModeChanged?.Invoke( this ); } finally { _mode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<RenderingEffect_ResolutionUpscale> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum.Auto;

		public enum TechniqueEnum
		{
			Auto,
			Bilinear,
			[DisplayNameEnum( "Lanczos 2" )]
			Lanczos2,
			[DisplayNameEnum( "AMD FSR 1.0" )]
			AMDFSR1
		}

		/// <summary>
		/// The technique of the effect.
		/// </summary>
		[DefaultValue( TechniqueEnum.Auto )]
		public Reference<TechniqueEnum> Technique
		{
			get { if( _technique.BeginGet() ) Technique = _technique.Get( this ); return _technique.value; }
			set { if( _technique.BeginSet( ref value ) ) { try { TechniqueChanged?.Invoke( this ); } finally { _technique.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Technique"/> property value changes.</summary>
		public event Action<RenderingEffect_ResolutionUpscale> TechniqueChanged;
		ReferenceField<TechniqueEnum> _technique = TechniqueEnum.Auto;

		/// <summary>
		/// The multiplier to the mipmapping bias.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		public Reference<double> AffectMipBias
		{
			get { if( _affectMipBias.BeginGet() ) AffectMipBias = _affectMipBias.Get( this ); return _affectMipBias.value; }
			set { if( _affectMipBias.BeginSet( ref value ) ) { try { AffectMipBiasChanged?.Invoke( this ); } finally { _affectMipBias.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectMipBias"/> property value changes.</summary>
		public event Action<RenderingEffect_ResolutionUpscale> AffectMipBiasChanged;
		ReferenceField<double> _affectMipBias = 1.0;

		/////////////////////////////////////////

		[Browsable( false )]
		public ModeEnum ModeAfterLoading = ModeEnum.Auto;
		[Browsable( false )]
		public TechniqueEnum TechniqueAfterLoading = TechniqueEnum.AMDFSR1;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( AffectMipBias ):
					if( Mode.Value == ModeEnum.Original || Technique.Value != TechniqueEnum.AMDFSR1 )
						skip = true;
					break;
				}
			}
		}

		public ModeEnum GetMode()
		{
			var result = Mode.Value;
			if( result == ModeEnum.Auto )
			{
				if( SystemSettings.LimitedDevice )
					result = ModeEnum.UltraQuality;
				else
					result = ModeEnum.Original;
			}
			return result;
		}

		public TechniqueEnum GetTechnique()
		{
			var result = Technique.Value;
			if( result == TechniqueEnum.Auto )
			{
				if( SystemSettings.LimitedDevice )
					result = TechniqueEnum.Lanczos2;
				else
					result = TechniqueEnum.AMDFSR1;
			}

			//FSR is not supported on mobile
			if( SystemSettings.LimitedDevice && result == TechniqueEnum.AMDFSR1 )
				result = TechniqueEnum.Lanczos2;

			return result;
		}

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			ModeAfterLoading = Mode;
			TechniqueAfterLoading = Technique;

			return true;
		}

		public virtual Vector2 GetResolutionMultiplier()
		{
			switch( GetMode() )
			{
			case ModeEnum.UltraQuality: return new Vector2( 1.0 / 1.3, 1.0 / 1.3 );
			case ModeEnum.Quality: return new Vector2( 1.0 / 1.5, 1.0 / 1.5 );
			case ModeEnum.Balanced: return new Vector2( 1.0 / 1.7, 1.0 / 1.7 );
			case ModeEnum.Performance: return new Vector2( 0.5, 0.5 );
			}
			return Vector2.One;
		}

		public virtual double GetMipBias()
		{
			var result = 0.0;
			if( GetTechnique() == TechniqueEnum.AMDFSR1 )
			{
				switch( GetMode() )
				{
				case ModeEnum.UltraQuality: result = -0.38; break;
				case ModeEnum.Quality: result = -0.58; break;
				case ModeEnum.Balanced: result = -0.79; break;
				case ModeEnum.Performance: result = -1.0; break;
				}
			}
			result *= AffectMipBias.Value;
			return result;
		}

		public virtual double GetSharpnessMultiplier()
		{
			switch( GetMode() )
			{
			case ModeEnum.UltraQuality: return 1.1;
			case ModeEnum.Quality: return 1.16666;
			case ModeEnum.Balanced: return 1.23333;
			case ModeEnum.Performance: return 1.3;
			}
			return 1.0;
		}

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline.IFrameData frameData, ref ImageComponent actualTexture )
		{
			if( GetMode() != ModeEnum.Original && context.Owner.SizeInPixels != actualTexture.Result.ResultSize )
			{
				switch( GetTechnique() )
				{
				case TechniqueEnum.Bilinear:
					RenderBilinear( context, frameData, ref actualTexture );
					break;
				case TechniqueEnum.Lanczos2:
					RenderLanczos2( context, frameData, ref actualTexture );
					break;
				case TechniqueEnum.AMDFSR1:
					RenderAMDFSR1( context, frameData, ref actualTexture );
					break;
				}
			}
		}

		void RenderBilinear( ViewportRenderingContext context, RenderingPipeline.IFrameData frameData, ref ImageComponent actualTexture )
		{
			var rescaledTexture = context.RenderTarget2D_Alloc( context.Owner.SizeInPixels, actualTexture.Result.ResultFormat );

			//copy to scene texture
			{
				context.SetViewport( rescaledTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Copy_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

				context.RenderQuadToCurrentViewport( shader );
			}

			context.DynamicTexture_Free( actualTexture );

			actualTexture = rescaledTexture;
		}

		unsafe void RenderLanczos2( ViewportRenderingContext context, RenderingPipeline.IFrameData frameData, ref ImageComponent actualTexture )
		{
			var rescaledTexture = context.RenderTarget2D_Alloc( context.Owner.SizeInPixels, actualTexture.Result.ResultFormat );

			//copy to scene texture
			{
				context.SetViewport( rescaledTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\Lanczos2_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

				var inputSize = actualTexture.Result.ResultSize;
				var outputSize = context.Owner.SizeInPixels;

				shader.Parameters.Set( "viewportSize", new Vector4F( outputSize.X, outputSize.Y, 1.0f / (float)outputSize.X, 1.0f / (float)outputSize.Y ) );
				shader.Parameters.Set( "inputSize", new Vector4F( inputSize.X, inputSize.Y, 1.0f / (float)inputSize.X, 1.0f / (float)inputSize.Y ) );

				context.RenderQuadToCurrentViewport( shader );
			}

			context.DynamicTexture_Free( actualTexture );

			actualTexture = rescaledTexture;
		}

		static float ARcpF1( float a ) { return 1.0f / a; }
		static float AF1_( double a ) { return (float)a; }
		static float AU1_AF1( float a ) { return a; }
		//unsafe static uint AU1_AF1( float a ) { return *(uint*)&a; }
		//static float AExp2F1( float a ) { return MathEx.Pow( 2, a ); }

		static void FsrEasuCon(
			ref Vector4F con0,
			ref Vector4F con1,
			ref Vector4F con2,
			ref Vector4F con3,
			float inputViewportInPixelsX,
			float inputViewportInPixelsY,
			float inputSizeInPixelsX,
			float inputSizeInPixelsY,
			float outputSizeInPixelsX,
			float outputSizeInPixelsY )
		{
			con0[ 0 ] = AU1_AF1( inputViewportInPixelsX * ARcpF1( outputSizeInPixelsX ) );
			con0[ 1 ] = AU1_AF1( inputViewportInPixelsY * ARcpF1( outputSizeInPixelsY ) );
			con0[ 2 ] = AU1_AF1( AF1_( 0.5 ) * inputViewportInPixelsX * ARcpF1( outputSizeInPixelsX ) - AF1_( 0.5 ) );
			con0[ 3 ] = AU1_AF1( AF1_( 0.5 ) * inputViewportInPixelsY * ARcpF1( outputSizeInPixelsY ) - AF1_( 0.5 ) );

			con1[ 0 ] = AU1_AF1( ARcpF1( inputSizeInPixelsX ) );
			con1[ 1 ] = AU1_AF1( ARcpF1( inputSizeInPixelsY ) );

			con1[ 2 ] = AU1_AF1( AF1_( 1.0 ) * ARcpF1( inputSizeInPixelsX ) );
			con1[ 3 ] = AU1_AF1( AF1_( -1.0 ) * ARcpF1( inputSizeInPixelsY ) );

			con2[ 0 ] = AU1_AF1( AF1_( -1.0 ) * ARcpF1( inputSizeInPixelsX ) );
			con2[ 1 ] = AU1_AF1( AF1_( 2.0 ) * ARcpF1( inputSizeInPixelsY ) );
			con2[ 2 ] = AU1_AF1( AF1_( 1.0 ) * ARcpF1( inputSizeInPixelsX ) );
			con2[ 3 ] = AU1_AF1( AF1_( 2.0 ) * ARcpF1( inputSizeInPixelsY ) );
			con3[ 0 ] = AU1_AF1( AF1_( 0.0 ) * ARcpF1( inputSizeInPixelsX ) );
			con3[ 1 ] = AU1_AF1( AF1_( 4.0 ) * ARcpF1( inputSizeInPixelsY ) );
			con3[ 2 ] = con3[ 3 ] = 0;
		}

		unsafe void RenderAMDFSR1( ViewportRenderingContext context, RenderingPipeline.IFrameData frameData, ref ImageComponent actualTexture )
		{

			//!!!!sRGB на входе?

			//!!!!format?


			var hdr = false;
			var bUseRcas = true;

			var rescaledTexture = context.RenderTarget2D_Alloc( context.Owner.SizeInPixels, actualTexture.Result.ResultFormat );

			//pass 1, upscale
			{
				context.SetViewport( rescaledTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\FSR\FSR_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

				var inputSize = actualTexture.Result.ResultSize;
				var outputSize = context.Owner.SizeInPixels;

				var parameters = new Vector4F[ 5 ];
				FsrEasuCon( ref parameters[ 0 ], ref parameters[ 1 ], ref parameters[ 2 ], ref parameters[ 3 ], inputSize.X, inputSize.Y, inputSize.X, inputSize.Y, outputSize.X, outputSize.Y );
				parameters[ 4 ].X = ( hdr && !bUseRcas ) ? 1 : 0;
				shader.Parameters.Set( "fsrParameters", parameters );

				var size = outputSize;
				shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );

				context.RenderQuadToCurrentViewport( shader );
			}

			context.DynamicTexture_Free( actualTexture );

			actualTexture = rescaledTexture;




			//pass 2 doing in Sharpen effect


			//var sharpedTexture = context.RenderTarget2D_Alloc( context.Owner.SizeInPixels, rescaledTexture.Result.ResultFormat );

			////pass 2, sharpening
			//{
			//	context.SetViewport( sharpedTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			//	CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//	shader.FragmentProgramFileName = @"Base\Shaders\Effects\FSR\FSR_Pass_fs.sc";

			//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, rescaledTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

			//	shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "SAMPLE_RCAS" ) );
			//	//if( Denoise )
			//	//	shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "FSR_RCAS_DENOISE" ) );

			//	var parameters = new Vector4F[ 5 ];
			//	float rcasAttenuation = 1.0f - (float)Sharpness;
			//	//if( rcasAttenuation < 0 )
			//	//	rcasAttenuation = 0;
			//	FsrRcasCon( ref parameters[ 0 ], rcasAttenuation );
			//	parameters[ 4 ].X = hdr ? 1 : 0;
			//	shader.Parameters.Set( "fsrParameters", parameters );

			//	var size = context.Owner.SizeInPixels;
			//	shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );

			//	context.RenderQuadToCurrentViewport( shader );
			//}

			//context.DynamicTexture_Free( rescaledTexture );
			//actualTexture = sharpedTexture;


			////!!!!надо ли копировать еще раз


			////!!!!format
			//var rescaledTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, ImageComponent.TypeEnum._2D, context.Owner.SizeInPixels, actualTexture.Result.ResultFormat, 0, false );

			////!!!!TEMP чтобы обновить view number counter
			//context.SetComputeView();

			//zzzzzz;

			////!!!!
			//Bgfx.SetComputeImage( 0, rescaledTexture.Result.GetRealObject( true ), 0, ComputeBufferAccess.Write, TextureFormat.RGBA32F );
			////Bgfx.SetComputeImage( 0, texture.Result.GetRealObject( true ), 0, ComputeBufferAccess.Write, TextureFormat.R32F );
			//// R16F );
			////Bgfx.SetComputeImage( 0, texture.Result.GetRealObject( false ), 0, ComputeBufferAccess.Write, TextureFormat.R32F );// R16F );

			////Bgfx.SetComputeBuffer( 0, (DynamicVertexBuffer)destVertexBuffer.GetNativeObject(), ComputeBufferAccess.Write );


			//zzzzzz;

			////set u_fsrParameters
			//unsafe
			//{
			//	var inputSize = actualTexture.Result.ResultSize;
			//	var outputSize = context.Owner.SizeInPixels;

			//	IVec4* parameters = stackalloc IVec4[ 5 ];
			//	NativeUtility.ZeroMemory( parameters, sizeof( IVec4 ) * 5 );

			//	FsrEasuCon( out parameters[ 0 ], out parameters[ 1 ], out parameters[ 2 ], out parameters[ 3 ], inputSize.X, inputSize.Y, inputSize.X, inputSize.Y, outputSize.X, outputSize.Y );

			//	//!!!!
			//	//DXGI_FORMAT fmt = ( hdr ? DXGI_FORMAT_R10G10B10A2_UNORM : DXGI_FORMAT_R8G8B8A8_UNORM );

			//	//!!!!
			//	var hdr = true;

			//	//!!!!
			//	var bUseRcas = true;
			//	//ImGui::Checkbox("FSR 1.0 Sharpening", &m_state.bUseRcas);

			//	parameters[ 4 ].v0 = ( hdr && !bUseRcas ) ? 1 : 0;
			//	//parameters[ 4 ].v0 = (uint)( ( hdr && !bUseRcas ) ? 1 : 0 );

			//	var uniform = GpuProgramManager.RegisterUniform( "u_fsrParameters", UniformType.Vector4, 5 );
			//	Bgfx.SetUniform( uniform, parameters, 5 );
			//}

			//zzzz;

			////!!!!
			//Bgfx.Dispatch( context.CurrentViewNumber, computeProgram.Value, 1, 1, 1, DiscardFlags.All );



			//zzzzzz;

			////copy to scene texture
			//{
			//	context.SetViewport( rescaledTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			//	CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//	shader.FragmentProgramFileName = @"Base\Shaders\Effects\FSR\FSR_Pass.sc";

			//	zzzzz;
			//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

			//	context.RenderQuadToCurrentViewport( shader );
			//}

			//context.DynamicTexture_Free( actualTexture );

			//actualTexture = rescaledTexture;



			////var rescaledTexture = context.RenderTarget2D_Alloc( context.Owner.SizeInPixels, actualTexture.Result.ResultFormat );

			//////copy to scene texture
			////{
			////	context.SetViewport( rescaledTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			////	CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			////	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			////	shader.FragmentProgramFileName = @"Base\Shaders\Effects\FSR\FSR_Pass.sc";

			////	zzzzz;
			////	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

			////	context.RenderQuadToCurrentViewport( shader );
			////}

			////context.DynamicTexture_Free( actualTexture );

			////actualTexture = rescaledTexture;


		}

		public override bool LimitedDevicesSupport
		{
			get { return true; }
		}
	}
}
