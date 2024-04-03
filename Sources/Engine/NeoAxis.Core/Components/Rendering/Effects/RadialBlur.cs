// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
    /// <summary>
    /// Radial blur screen effect.
    /// </summary>
	[DefaultOrderOfEffect( 12 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_RadialBlur : RenderingEffect_Simple
	{
		const string shaderDefault = @"Base\Shaders\Effects\RadialBlur_fs.sc";

		public RenderingEffect_RadialBlur()
		{
			ShaderFile = shaderDefault;
		}

        /// <summary>
        /// The center of the effect.
        /// </summary>
		[Serialize]
		[DefaultValue( "0.5 0.5" )]
		[Range( 0, 1 )]
		public Reference<Vector2> Center
		{
			get { if( _center.BeginGet() ) Center = _center.Get( this ); return _center.value; }
			set { if( _center.BeginSet( this, ref value ) ) { try { CenterChanged?.Invoke( this ); } finally { _center.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Center"/> property value changes.</summary>
		public event Action<RenderingEffect_RadialBlur> CenterChanged;
		ReferenceField<Vector2> _center = new Vector2( 0.5, 0.5 );

        /// <summary>
        /// The amount of blur applied.
        /// </summary>
		[Serialize]
		[DefaultValue( 0.1 )]
		[Range( 0, 1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> BlurFactor
		{
			get { if( _blurFactor.BeginGet() ) BlurFactor = _blurFactor.Get( this ); return _blurFactor.value; }
			set { if( _blurFactor.BeginSet( this, ref value ) ) { try { BlurFactorChanged?.Invoke( this ); } finally { _blurFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurFactor"/> property value changes.</summary>
		public event Action<RenderingEffect_RadialBlur> BlurFactorChanged;
		ReferenceField<double> _blurFactor = 0.1;

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
