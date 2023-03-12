// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Chromatic aberration screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 7.1 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_ChromaticAberration : RenderingEffect_Simple
	{
		const string shaderDefault = @"Base\Shaders\Effects\ChromaticAberration_fs.sc";

		//

		public RenderingEffect_ChromaticAberration()
		{
			ShaderFile = shaderDefault;
		}

		/// <summary>
		/// The strength of the effect.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> Amount
		{
			get { if( _amount.BeginGet() ) Amount = _amount.Get( this ); return _amount.value; }
			set { if( _amount.BeginSet( ref value ) ) { try { AmountChanged?.Invoke( this ); } finally { _amount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Amount"/> property value changes.</summary>
		public event Action<RenderingEffect_ChromaticAberration> AmountChanged;
		ReferenceField<double> _amount = 1.0;

		/////////////////////////////////////////

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
