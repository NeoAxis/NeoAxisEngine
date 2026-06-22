// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Tone mapping screen effect.
	/// </summary>
	[NewObjectDefaultName( "Tone Mapping" )]
	[DefaultOrderOfEffect( 3 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_ToneMapping : RenderingEffect
	{
		public static double GlobalBrightness;
		public static double GlobalExposure = 1;

		/// <summary>
		/// The intensity of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Effect" )]
		public Reference<double> Intensity
		{
			get { if( _intensity.BeginGet() ) Intensity = _intensity.Get( this ); return _intensity.value; }
			set { if( _intensity.BeginSet( this, ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<RenderingEffect_ToneMapping> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// The brightness of the tone mapping.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( -1, 1 )]
		public Reference<double> Brightness
		{
			get { if( _brightness.BeginGet() ) Brightness = _brightness.Get( this ); return _brightness.value; }
			set { if( _brightness.BeginSet( this, ref value ) ) { try { BrightnessChanged?.Invoke( this ); } finally { _brightness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Brightness"/> property value changes.</summary>
		public event Action<RenderingEffect_ToneMapping> BrightnessChanged;
		ReferenceField<double> _brightness = 0.0;

		/// <summary>
		/// The level of exposure.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Serialize]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> Exposure
		{
			get { if( _exposure.BeginGet() ) Exposure = _exposure.Get( this ); return _exposure.value; }
			set { if( _exposure.BeginSet( this, ref value ) ) { try { ExposureChanged?.Invoke( this ); } finally { _exposure.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Exposure"/> property value changes.</summary>
		public event Action<RenderingEffect_ToneMapping> ExposureChanged;
		ReferenceField<double> _exposure = 1.0;

		public enum MethodEnum
		{
			Neutral,
			ACES,
			Linear,
			Custom
		}

		/// <summary>
		/// The type of the tone mapping.
		/// </summary>
		[DefaultValue( MethodEnum.Neutral )] //ACES 
		public Reference<MethodEnum> Method
		{
			get { if( _method.BeginGet() ) Method = _method.Get( this ); return _method.value; }
			set { if( _method.BeginSet( this, ref value ) ) { try { MethodChanged?.Invoke( this ); } finally { _method.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Method"/> property value changes.</summary>
		public event Action<RenderingEffect_ToneMapping> MethodChanged;
		ReferenceField<MethodEnum> _method = MethodEnum.Neutral;// ACES;

		//!!!!text editor form
		/// <summary>
		/// The shader code of the custom method.
		/// </summary>
		const string customCodeDefault = "vec3 method_custom(vec3 x){return x / ( vec3(1,1,1) + x * vec3(0.2126,0.7152,0.0722) );}";
		[DefaultValue( customCodeDefault )]
		public Reference<string> CustomCode
		{
			get { if( _customCode.BeginGet() ) CustomCode = _customCode.Get( this ); return _customCode.value; }
			set { if( _customCode.BeginSet( this, ref value ) ) { try { CustomCodeChanged?.Invoke( this ); } finally { _customCode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CustomCode"/> property value changes.</summary>
		public event Action<RenderingEffect_ToneMapping> CustomCodeChanged;
		ReferenceField<string> _customCode = customCodeDefault;

		///// <summary>
		///// The gamma of the tone mapping.
		///// </summary>
		//[DefaultValue( 2.2 )]
		//[Serialize]
		//[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> Gamma
		//{
		//	get { if( _gamma.BeginGet() ) Gamma = _gamma.Get( this ); return _gamma.value; }
		//	set { if( _gamma.BeginSet( this, ref value ) ) { try { GammaChanged?.Invoke( this ); } finally { _gamma.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Gamma"/> property value changes.</summary>
		//public event Action<RenderingEffect_ToneMapping> GammaChanged;
		//ReferenceField<double> _gamma = 2.2;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( CustomCode ):
					if( Method.Value != MethodEnum.Custom )
						skip = true;
					break;
				}
			}
		}

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			if( Intensity <= 0 )
				return;

			var newTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, PixelFormat.A8R8G8B8 );

			context.SetViewport( newTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			shader.FragmentProgramFileName = @"Base\Shaders\Effects\ToneMapping_fs.sc";

			shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
				TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

			var brightness = Brightness + GlobalBrightness;
			var exposure = Exposure * GlobalExposure;
			shader.Parameters.Set( "u_tonemapping_parameters", new Vector4( Intensity, brightness, exposure, 0 ).ToVector4F() );

			shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( $"TONEMAPPING_METHOD_{Method.Value.ToString().ToUpper()}" ) );
			if( Method.Value == MethodEnum.Custom )
				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "CUSTOM_CODE", CustomCode.Value ) );
			if( RenderingSystem.AccurateSrgbCorrection )
				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "ACCURATE_SRGB_CORRECTION" ) );

			context.RenderQuadToCurrentViewport( shader );

			//free old texture
			context.DynamicTexture_Free( actualTexture );

			actualTexture = newTexture;
		}

		public override bool LimitedDevicesSupport
		{
			get { return true; }
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "ToneMapping", true );
		}
	}
}
