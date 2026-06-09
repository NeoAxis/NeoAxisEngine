// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Screen effect of adding the rain.
	/// </summary>
	[DefaultOrderOfEffect( 8.3 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_Precipitation : RenderingEffect//_Simple
	{
		/// <summary>
		/// Whether to get intensity of the effect from Scene.PrecipitationFalling property.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Effect" )]
		public Reference<bool> GetSettingsFromScene
		{
			get { if( _getSettingsFromScene.BeginGet() ) GetSettingsFromScene = _getSettingsFromScene.Get( this ); return _getSettingsFromScene.value; }
			set { if( _getSettingsFromScene.BeginSet( this, ref value ) ) { try { GetSettingsFromSceneChanged?.Invoke( this ); } finally { _getSettingsFromScene.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GetSettingsFromScene"/> property value changes.</summary>
		public event Action<RenderingEffect_Precipitation> GetSettingsFromSceneChanged;
		ReferenceField<bool> _getSettingsFromScene = true;

		/// <summary>
		/// The intensity of the effect.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Effect" )]
		public Reference<double> Intensity
		{
			get { if( _intensity.BeginGet() ) Intensity = _intensity.Get( this ); return _intensity.value; }
			set { if( _intensity.BeginSet( this, ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<RenderingEffect_Precipitation> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// The color multiplier of the effect.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValuePowered> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( this, ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<RenderingEffect_Precipitation> ColorChanged;
		ReferenceField<ColorValuePowered> _color = ColorValuePowered.One;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Intensity ):
					if( GetSettingsFromScene )
						skip = true;
					break;
				}
			}
		}

		double GetIntensity()
		{
			if( GetSettingsFromScene )
			{
				var scene = ParentRoot as Scene;
				if( scene != null )
					return scene.PrecipitationFalling;
				else
					return 0;
			}
			else
				return Intensity;
		}

		protected override void OnRender( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			var intensity = GetIntensity();
			if( intensity <= 0 )
				return;

			var newTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );

			context.SetViewport( newTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			var shader = new CanvasRenderer.ShaderItem();
			shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			shader.FragmentProgramFileName = @"Base\Shaders\Effects\Precipitation_fs.sc";

			shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

			shader.Parameters.Set( "intensity", (float)intensity );
			shader.Parameters.Set( "precipitationMultiplier", new Vector4F( Color.Value.ToVector3F(), 0 ) );

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
			return new ScreenLabelInfo( "ScreenEffect", true );
		}
	}
}
