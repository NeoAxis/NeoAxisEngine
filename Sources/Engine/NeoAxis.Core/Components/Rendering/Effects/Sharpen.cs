// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// A screen effect for adding sharpness to the image.
	/// </summary>
	[DefaultOrderOfEffect( 6 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_Sharpen : RenderingEffect
	{
		/// <summary>
		/// The intensity of the effect.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Effect" )]
		public Reference<double> Intensity
		{
			get { if( _intensity.BeginGet() ) Intensity = _intensity.Get( this ); return _intensity.value; }
			set { if( _intensity.BeginSet( ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<RenderingEffect_Sharpen> IntensityChanged;
		ReferenceField<double> _intensity = 1.0;

		/// <summary>
		/// The strength of the sharpness.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 2 )]
		public Reference<double> Strength
		{
			get { if( _strength.BeginGet() ) Strength = _strength.Get( this ); return _strength.value; }
			set { if( _strength.BeginSet( ref value ) ) { try { StrengthChanged?.Invoke( this ); } finally { _strength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Strength"/> property value changes.</summary>
		public event Action<RenderingEffect_Sharpen> StrengthChanged;
		ReferenceField<double> _strength = 1.0;

		//[DefaultValue( false )]
		//public Reference<bool> Denoise
		//{
		//	get { if( _denoise.BeginGet() ) Denoise = _denoise.Get( this ); return _denoise.value; }
		//	set { if( _denoise.BeginSet( ref value ) ) { try { DenoiseChanged?.Invoke( this ); } finally { _denoise.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Denoise"/> property value changes.</summary>
		//public event Action<RenderingEffect_ResolutionUpscale> DenoiseChanged;
		//ReferenceField<bool> _denoise = false;

		/////////////////////////////////////////

		[Browsable( false )]
		public double StrengthAfterLoading = 1.0;

		/////////////////////////////////////////

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			StrengthAfterLoading = Strength;

			return true;
		}

		//static float AU1_AF1( float a ) { return a; }
		////unsafe static uint AU1_AF1( float a ) { return *(uint*)&a; }
		//static float AExp2F1( float a ) { return MathEx.Pow( 2, a ); }

		//static void FsrRcasCon(
		//	ref Vector4F con,
		//	// The scale is {0.0 := maximum, to N>0, where N is the number of stops (halving) of the reduction of sharpness}.
		//	float sharpness )
		//{
		//	// Transform from stops to linear value.
		//	sharpness = AExp2F1( -sharpness );
		//	//var hSharp = new Vector2F( sharpness, sharpness );
		//	con[ 0 ] = AU1_AF1( sharpness );
		//	//con[ 1 ] = AU1_AH2_AF2( hSharp );
		//	con[ 2 ] = 0;
		//	con[ 3 ] = 0;
		//}

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline.IFrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			if( Intensity <= 0 || Strength <= 0 )
				return;

			var sharpedTexture = context.RenderTarget2D_Alloc( context.Owner.SizeInPixels, actualTexture.Result.ResultFormat );

			context.SetViewport( sharpedTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			shader.FragmentProgramFileName = @"Base\Shaders\Effects\Sharpen_fs.sc";

			shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

			var upscaleFactor = 1.0;
			{
				var pipeline = context.RenderingPipeline as RenderingPipeline_Basic;
				if( pipeline != null )
				{
					var effect = pipeline.GetSceneEffect<RenderingEffect_ResolutionUpscale>();
					if( effect != null )
						upscaleFactor = effect.GetSharpnessMultiplier();
				}
			}

			var size = context.Owner.SizeInPixels;
			shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );

			shader.Parameters.Set( "intensity", (float)Intensity );

			shader.Parameters.Set( "sharpStrength", (float)( Strength * 0.75 * upscaleFactor ) );


			//from FSR
			//if( !SystemSettings.MobileDevice )
			//{
			//	//FSR technique of the offset. is not supported on mobile

			//	var s = Strength.Value * 0.7 * upscaleFactor;
			//	if( s == 0 )
			//		s = 0.00001;
			//	var rcasAttenuation = 1.0 / s - 1.0;

			//	//y = 1 / (x + 1)
			//	//y * (x + 1 ) = 1
			//	//x + 1 = 1 / y
			//	//x = 1 / y - 1

			//	//float rcasAttenuation = 1.0f - (float)Strength;
			//	//if( rcasAttenuation < 0 )
			//	//	rcasAttenuation = 0;

			//	//if( Denoise )
			//	//	shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "FSR_RCAS_DENOISE" ) );

			//	var hdr = false;
			//	var parameters = new Vector4F[ 5 ];
			//	FsrRcasCon( ref parameters[ 0 ], (float)rcasAttenuation );
			//	parameters[ 4 ].X = hdr ? 1 : 0;
			//	shader.Parameters.Set( "fsrParameters", parameters );
			//}


			context.RenderQuadToCurrentViewport( shader );

			context.DynamicTexture_Free( actualTexture );
			actualTexture = sharpedTexture;
		}

		public override bool LimitedDevicesSupport
		{
			get { return true; }
		}

		//old technique

		//const string shaderDefault = @"Base\Shaders\Effects\Sharpen_fs.sc";

		//public RenderingEffect_Sharpen()
		//{
		//	Shader = shaderDefault;
		//}

		//      /// <summary>
		//      /// The strength of the sharpness.
		//      /// </summary>
		//[Serialize]
		//[DefaultValue( 0.5 )]
		//[Range( 0, 2 )]
		//public Reference<double> SharpStrength
		//{
		//	get { if( _sharpStrength.BeginGet() ) SharpStrength = _sharpStrength.Get( this ); return _sharpStrength.value; }
		//	set { if( _sharpStrength.BeginSet( ref value ) ) { try { SharpStrengthChanged?.Invoke( this ); } finally { _sharpStrength.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SharpStrength"/> property value changes.</summary>
		//public event Action<RenderingEffect_Sharpen> SharpStrengthChanged;
		//ReferenceField<double> _sharpStrength = 0.5;

		///// <summary>
		///// The threshold at which the sharpness will be clamped.
		///// </summary>
		//[Serialize]
		//[DefaultValue( .035 )]
		//[Range( 0, 1 )]
		//public Reference<double> SharpClamp
		//{
		//	get { if( _sharpClamp.BeginGet() ) SharpClamp = _sharpClamp.Get( this ); return _sharpClamp.value; }
		//	set { if( _sharpClamp.BeginSet( ref value ) ) { try { SharpClampChanged?.Invoke( this ); } finally { _sharpClamp.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SharpClamp"/> property value changes.</summary>
		//public event Action<RenderingEffect_Sharpen> SharpClampChanged;
		//ReferenceField<double> _sharpClamp = .035;

		//      /// <summary>
		//      /// The offset bias of the sharpness.
		//      /// </summary>
		//[Serialize]
		//[DefaultValue( 1.0 )]
		//[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> OffsetBias
		//{
		//	get { if( _offsetBias.BeginGet() ) OffsetBias = _offsetBias.Get( this ); return _offsetBias.value; }
		//	set { if( _offsetBias.BeginSet( ref value ) ) { try { OffsetBiasChanged?.Invoke( this ); } finally { _offsetBias.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OffsetBias"/> property value changes.</summary>
		//public event Action<RenderingEffect_Sharpen> OffsetBiasChanged;
		//ReferenceField<double> _offsetBias = 1;
	}
}
