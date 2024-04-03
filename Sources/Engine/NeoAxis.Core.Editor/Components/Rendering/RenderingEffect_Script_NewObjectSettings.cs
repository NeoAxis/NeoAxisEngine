// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	/// <summary>
	/// A set of settings for creation <see cref="RenderingEffect_Script"/> in the editor.
	/// </summary>
	public class RenderingEffect_Script_NewObjectSettings : NewObjectSettings
	{
		[DefaultValue( true )]
		[Category( "Options" )]
		[DisplayName( "Shader graph" )]
		public bool ShaderGraph { get; set; } = true;

		public override bool Creation( NewObjectCell.ObjectCreationContext context )
		{
			var newObject2 = (RenderingEffect_Script)context.newObject;

			if( ShaderGraph )
				newObject2.NewObjectCreateShaderGraph();

			return base.Creation( context );
		}
	}
}
