// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Posterization screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 8.1 )]
	public class Component_RenderingEffect_Posterize : Component_RenderingEffect_Simple
	{
		const string shaderDefault = @"Base\Shaders\Effects\Posterize_fs.sc";

		//

		public Component_RenderingEffect_Posterize()
		{
			Shader = shaderDefault;
		}

		[DefaultValue( 10.0 )]
		[Range( 1, 20 )]
		public Reference<double> Levels
		{
			get { if( _levels.BeginGet() ) Levels = _levels.Get( this ); return _levels.value; }
			set { if( _levels.BeginSet( ref value ) ) { try { LevelsChanged?.Invoke( this ); } finally { _levels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Levels"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_Posterize> LevelsChanged;
		ReferenceField<double> _levels = 10.0;

		/////////////////////////////////////////

		public override bool LimitedDevicesSupport
		{
			get { return true; }
		}
	}
}
