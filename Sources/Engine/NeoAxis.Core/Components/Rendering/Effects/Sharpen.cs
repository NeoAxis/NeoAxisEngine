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
    /// Screen effect for adding sharpness to the image.
    /// </summary>
	[DefaultOrderOfEffect( 6 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class Component_RenderingEffect_Sharpen : Component_RenderingEffect_Simple
	{
		const string shaderDefault = @"Base\Shaders\Effects\Sharpen_fs.sc";

		public Component_RenderingEffect_Sharpen()
		{
			Shader = shaderDefault;
		}

        /// <summary>
        /// The strength of the sharpness.
        /// </summary>
		[Serialize]
		[DefaultValue( 0.5 )]
		[Range( 0, 2 )]
		public Reference<double> SharpStrength
		{
			get { if( _sharpStrength.BeginGet() ) SharpStrength = _sharpStrength.Get( this ); return _sharpStrength.value; }
			set { if( _sharpStrength.BeginSet( ref value ) ) { try { SharpStrengthChanged?.Invoke( this ); } finally { _sharpStrength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SharpStrength"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Sharpen> SharpStrengthChanged;
		ReferenceField<double> _sharpStrength = 0.5;

		/// <summary>
		/// The threshold at which the sharpness will be clamped.
		/// </summary>
		[Serialize]
		[DefaultValue( .035 )]
		[Range( 0, 1 )]
		public Reference<double> SharpClamp
		{
			get { if( _sharpClamp.BeginGet() ) SharpClamp = _sharpClamp.Get( this ); return _sharpClamp.value; }
			set { if( _sharpClamp.BeginSet( ref value ) ) { try { SharpClampChanged?.Invoke( this ); } finally { _sharpClamp.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SharpClamp"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Sharpen> SharpClampChanged;
		ReferenceField<double> _sharpClamp = .035;

        /// <summary>
        /// The offset bias of the sharpness.
        /// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> OffsetBias
		{
			get { if( _offsetBias.BeginGet() ) OffsetBias = _offsetBias.Get( this ); return _offsetBias.value; }
			set { if( _offsetBias.BeginSet( ref value ) ) { try { OffsetBiasChanged?.Invoke( this ); } finally { _offsetBias.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OffsetBias"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Sharpen> OffsetBiasChanged;
		ReferenceField<double> _offsetBias = 1;
	}
}
