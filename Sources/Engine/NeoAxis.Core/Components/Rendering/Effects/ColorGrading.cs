// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Screen effect of adding the color correction to the image.
	/// </summary>
	[DefaultOrderOfEffect( 8 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_ColorGrading : RenderingEffect_Simple
	{
		const string shaderDefault = @"Base\Shaders\Effects\ColorGrading_fs.sc";

		//

		public RenderingEffect_ColorGrading()
		{
			ShaderFile = shaderDefault;
		}

		////!!!!возможность: или белая, или черная текстуры подставлять вместо ""
		const string lookupTableDefault = @"Base\Images\Color grading LUTs\Sepia.png";
		/// <summary>
		/// The lookup texture (LUT) used.
		/// </summary>
		[DefaultValueReference( lookupTableDefault )]
		//[DefaultValue( "reference:" + lookupTableDefault )]
		//[DefaultValue( null )]
		[ImageComponent.BindSettings( TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None, 1 )]
		public Reference<ImageComponent> LookupTable
		{
			get { if( _lookupTable.BeginGet() ) LookupTable = _lookupTable.Get( this ); return _lookupTable.value; }
			set { if( _lookupTable.BeginSet( this, ref value ) ) { try { LookupTableChanged?.Invoke( this ); } finally { _lookupTable.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LookupTable"/> property value changes.</summary>
		public event Action<RenderingEffect_ColorGrading> LookupTableChanged;
		ReferenceField<ImageComponent> _lookupTable = new Reference<ImageComponent>( null, lookupTableDefault );

		/// <summary>
		/// The color multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[ColorValueNoAlpha]
		[ApplicableRangeColorValuePower( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<ColorValuePowered> Multiply
		{
			get { if( _multiply.BeginGet() ) Multiply = _multiply.Get( this ); return _multiply.value; }
			set { if( _multiply.BeginSet( this, ref value ) ) { try { MultiplyChanged?.Invoke( this ); } finally { _multiply.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Multiply"/> property value changes.</summary>
		public event Action<RenderingEffect_ColorGrading> MultiplyChanged;
		ReferenceField<ColorValuePowered> _multiply = new ColorValuePowered( 1, 1, 1 );

		/// <summary>
		/// The color addition.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Range( -1, 1 )]
		public Reference<Vector3> Add
		{
			get { if( _add.BeginGet() ) Add = _add.Get( this ); return _add.value; }
			set { if( _add.BeginSet( this, ref value ) ) { try { AddChanged?.Invoke( this ); } finally { _add.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Add"/> property value changes.</summary>
		public event Action<RenderingEffect_ColorGrading> AddChanged;
		ReferenceField<Vector3> _add = Vector3.Zero;

		////Add
		//ReferenceField<ColorValuePowered> _add = new ColorValuePowered( 1, 1, 1, 1, 0 );
		//[DefaultValue( "1 1 1; 0" )]
		//[ColorValueNoAlpha]
		//[ApplicableRangeColorValuePower( 0, 10, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<ColorValuePowered> Add
		//{
		//	get
		//	{
		//		if( _add.BeginGet() )
		//			Add = _add.Get( this );
		//		return _add.value;
		//	}
		//	set
		//	{
		//		if( _add.BeginSet( this, ref value ) )
		//		{
		//			try { AddChanged?.Invoke( this ); }
		//			finally { _add.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<RenderingEffect_ColorGrading> AddChanged;

		/////////////////////////////////////////

		protected override void OnSetShaderParameters( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ImageComponent actualTexture, CanvasRenderer.ShaderItem shader )
		{
			base.OnSetShaderParameters( context, frameData, actualTexture, shader );

			var texture = LookupTable.Value;
			shader.Parameters.Set( "useLookupTable", texture != null ? 1.0f : -1.0f );
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
