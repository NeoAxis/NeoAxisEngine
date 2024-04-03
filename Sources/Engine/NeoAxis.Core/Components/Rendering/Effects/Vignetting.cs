// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Rendering effect to show darkened corners on the screen.
	/// </summary>
	[DefaultOrderOfEffect( 13 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_Vignetting : RenderingEffect_Simple
	{
		const string shaderDefault = @"Base\Shaders\Effects\Vignetting_fs.sc";
		const string noiseTextureDefault = @"Base\Images\Noise.png";

		//!!!!добавить опционально noise, чтобы если очень небольшой перепад градиента цвета, то чтобы размазывалось


		/// <summary>
		/// The color of the darkened corners.
		/// </summary>
		[Serialize]
		[DefaultValue( "0 0 0" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( this, ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<RenderingEffect_Vignetting> ColorChanged;
		ReferenceField<ColorValue> _color = new ColorValue( 0, 0, 0 );

		/// <summary>
		/// The radius of the vignetting.
		/// </summary>
		[Serialize]
		[DefaultValue( 2.0 )]
		[Range( 0, 10 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set { if( _radius.BeginSet( this, ref value ) ) { try { RadiusChanged?.Invoke( this ); } finally { _radius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<RenderingEffect_Vignetting> RadiusChanged;
		ReferenceField<double> _radius = 2;

		/// <summary>
		/// The range of noise.
		/// </summary>
		[Serialize]
		[DefaultValue( "1 1" )]
		[Range( 0, 2 )]
		public Reference<Range> NoiseRange
		{
			get { if( _noiseRange.BeginGet() ) NoiseRange = _noiseRange.Get( this ); return _noiseRange.value; }
			set { if( _noiseRange.BeginSet( this, ref value ) ) { try { NoiseRangeChanged?.Invoke( this ); } finally { _noiseRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="NoiseRange"/> property value changes.</summary>
		public event Action<RenderingEffect_Vignetting> NoiseRangeChanged;
		ReferenceField<Range> _noiseRange = new Range( 1, 1 );

		public RenderingEffect_Vignetting()
		{
			ShaderFile = shaderDefault;
		}

		protected override void OnSetShaderParameters( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ImageComponent actualTexture, CanvasRenderer.ShaderItem shader )
		{
			base.OnSetShaderParameters( context, frameData, actualTexture, shader );

			var noiseTexture = ResourceManager.LoadResource<ImageComponent>( noiseTextureDefault );
			if( noiseTexture == null )
				noiseTexture = ResourceUtility.WhiteTexture2D;

			var gpuNoiseTexture = noiseTexture.Result;

			shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"noiseTexture"*/, noiseTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

			{
				var size = actualTexture.Result.ResultSize;
				shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );
			}

			{
				var size = gpuNoiseTexture.ResultSize;
				shader.Parameters.Set( "noiseTextureSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );
			}

		}

		public override bool LimitedDevicesSupport
		{
			get { return true; }
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "ScreenEffect", true );
		}
	}
}
