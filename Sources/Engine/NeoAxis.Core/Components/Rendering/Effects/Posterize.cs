// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Posterization screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 8.1 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_Posterize : RenderingEffect_Simple
	{
		const string shaderDefault = @"Base\Shaders\Effects\Posterize_fs.sc";

		//

		public RenderingEffect_Posterize()
		{
			ShaderFile = shaderDefault;
		}

		/// <summary>
		/// The number of levels.
		/// </summary>
		[DefaultValue( 10.0 )]
		[Range( 1, 20 )]
		public Reference<double> Levels
		{
			get { if( _levels.BeginGet() ) Levels = _levels.Get( this ); return _levels.value; }
			set { if( _levels.BeginSet( ref value ) ) { try { LevelsChanged?.Invoke( this ); } finally { _levels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Levels"/> property value changes.</summary>
		public event Action<RenderingEffect_Posterize> LevelsChanged;
		ReferenceField<double> _levels = 10.0;

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
