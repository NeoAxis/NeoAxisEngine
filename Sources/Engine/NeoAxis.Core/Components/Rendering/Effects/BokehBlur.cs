// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Bokeh blur screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 12.5 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_BokehBlur : RenderingEffect
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
		public event Action<RenderingEffect_BokehBlur> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// The radius of the blur.
		/// </summary>
		[Range( 0, 0.05, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DefaultValue( 0.01 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set { if( _radius.BeginSet( ref value ) ) { try { RadiusChanged?.Invoke( this ); } finally { _radius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<RenderingEffect_BokehBlur> RadiusChanged;
		ReferenceField<double> _radius = 0.01;

		/// <summary>
		/// The power of the effect.
		/// </summary>
		[DefaultValue( 2.0 )]
		[Range( 0, 8, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> Power
		{
			get { if( _power.BeginGet() ) Power = _power.Get( this ); return _power.value; }
			set { if( _power.BeginSet( ref value ) ) { try { PowerChanged?.Invoke( this ); } finally { _power.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Power"/> property value changes.</summary>
		public event Action<RenderingEffect_BokehBlur> PowerChanged;
		ReferenceField<double> _power = 2.0;

		public enum PatternEnum
		{
			VogelDisk,
			Hexagon,
		}

		/// <summary>
		/// The sampling pattern.
		/// </summary>
		[DefaultValue( PatternEnum.VogelDisk )]
		public Reference<PatternEnum> Pattern
		{
			get { if( _pattern.BeginGet() ) Pattern = _pattern.Get( this ); return _pattern.value; }
			set { if( _pattern.BeginSet( ref value ) ) { try { PatternChanged?.Invoke( this ); } finally { _pattern.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Pattern"/> property value changes.</summary>
		public event Action<RenderingEffect_BokehBlur> PatternChanged;
		ReferenceField<PatternEnum> _pattern = PatternEnum.VogelDisk;

		public enum SamplesEnum
		{
			_10,
			_20,
			_30,
			_40,
			_60,
			_80,
			_100,
		}

		/// <summary>
		/// The amount of samplers.
		/// </summary>
		[DefaultValue( SamplesEnum._40 )]
		public Reference<SamplesEnum> Samples
		{
			get { if( _samples.BeginGet() ) Samples = _samples.Get( this ); return _samples.value; }
			set { if( _samples.BeginSet( ref value ) ) { try { SamplesChanged?.Invoke( this ); } finally { _samples.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Samples"/> property value changes.</summary>
		public event Action<RenderingEffect_BokehBlur> SamplesChanged;
		ReferenceField<SamplesEnum> _samples = SamplesEnum._40;

		/////////////////////////////////////////

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline.IFrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			if( Intensity <= 0 )
				return;

			//create final
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				var shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\BokehBlur_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				//var patternTexture = ResourceManager.LoadResource<ImageComponent>( @"Base\Images\Lens flares\hexangle.png" );
				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"patternTexture"*/, patternTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "PATTERN_" + Pattern.Value.ToString().ToUpper() ) );
				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "SAMPLES", Samples.Value.ToString().ToUpper().Replace( "_", "" ) ) );

				shader.Parameters.Set( "intensity", (float)Intensity );

				var size = actualTexture.Result.ResultSize;
				float radiusX = (float)Radius * size.Y / size.X;

				var parameters = new Vector4F( radiusX, (float)Radius, (float)Power, 0 );
				shader.Parameters.Set( "bokehParameters", parameters );

				context.RenderQuadToCurrentViewport( shader );
			}

			//free old textures
			context.DynamicTexture_Free( actualTexture );

			//update actual texture
			actualTexture = finalTexture;
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
