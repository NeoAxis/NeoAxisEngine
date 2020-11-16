// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Tone mapping screen effect.
	/// </summary>
	[NewObjectDefaultName( "Tone Mapping" )]
	[DefaultOrderOfEffect( 3 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class Component_RenderingEffect_ToneMapping : Component_RenderingEffect
	{
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
			set { if( _intensity.BeginSet( ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ToneMapping> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		/// <summary>
		/// The input gamma of the tone mapping.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Serialize]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> GammaInput
		{
			get { if( _gammaInput.BeginGet() ) GammaInput = _gammaInput.Get( this ); return _gammaInput.value; }
			set { if( _gammaInput.BeginSet( ref value ) ) { try { GammaInputChanged?.Invoke( this ); } finally { _gammaInput.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GammaInput"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ToneMapping> GammaInputChanged;
		ReferenceField<double> _gammaInput = 0.5;

		/// <summary>
		/// The level of exposure.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Serialize]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> Exposure
		{
			get { if( _exposure.BeginGet() ) Exposure = _exposure.Get( this ); return _exposure.value; }
			set { if( _exposure.BeginSet( ref value ) ) { try { ExposureChanged?.Invoke( this ); } finally { _exposure.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Exposure"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ToneMapping> ExposureChanged;
		ReferenceField<double> _exposure = 1.0;

		public enum MethodEnum
		{
			Linear,
			ACES,
			Custom
		}

		/// <summary>
		/// The type of the tone mapping.
		/// </summary>
		[DefaultValue( MethodEnum.ACES )]
		public Reference<MethodEnum> Method
		{
			get { if( _method.BeginGet() ) Method = _method.Get( this ); return _method.value; }
			set { if( _method.BeginSet( ref value ) ) { try { MethodChanged?.Invoke( this ); } finally { _method.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Method"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ToneMapping> MethodChanged;
		ReferenceField<MethodEnum> _method = MethodEnum.ACES;

		//!!!!text editor form
		/// <summary>
		/// The shader code of the custom method.
		/// </summary>
		const string customCodeDefault = "vec3 method_custom(vec3 x){return x / ( vec3(1,1,1) + x * vec3(0.2126,0.7152,0.0722) );}";
		[DefaultValue( customCodeDefault )]
		public Reference<string> CustomCode
		{
			get { if( _customCode.BeginGet() ) CustomCode = _customCode.Get( this ); return _customCode.value; }
			set { if( _customCode.BeginSet( ref value ) ) { try { CustomCodeChanged?.Invoke( this ); } finally { _customCode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CustomCode"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ToneMapping> CustomCodeChanged;
		ReferenceField<string> _customCode = customCodeDefault;

		/// <summary>
		/// The output gamma of the tone mapping.
		/// </summary>
		[DefaultValue( 2.2 )]
		[Serialize]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> GammaOutput
		{
			get { if( _gammaOutput.BeginGet() ) GammaOutput = _gammaOutput.Get( this ); return _gammaOutput.value; }
			set { if( _gammaOutput.BeginSet( ref value ) ) { try { GammaOutputChanged?.Invoke( this ); } finally { _gammaOutput.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GammaOutput"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_ToneMapping> GammaOutputChanged;
		ReferenceField<double> _gammaOutput = 2.2;

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

		protected override void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			var newTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, PixelFormat.A8R8G8B8 );

			context.SetViewport( newTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			shader.FragmentProgramFileName = @"Base\Shaders\Effects\ToneMapping_fs.sc";

			shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
				TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

			shader.Parameters.Set( "u_tonemapping_parameters", new Vector4F( (float)Intensity, (float)GammaInput, (float)Exposure, (float)GammaOutput ) );

			shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( $"TONEMAPPING_METHOD_{Method.Value.ToString().ToUpper()}" ) );
			if( Method.Value == MethodEnum.Custom )
				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "CUSTOM_CODE", CustomCode.Value ) );

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
