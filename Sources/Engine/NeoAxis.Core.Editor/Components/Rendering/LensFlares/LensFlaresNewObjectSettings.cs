// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	/// <summary>
	/// A set of settings for <see cref="LensFlares"/> creation in the editor.
	/// </summary>
	public class LensFlaresNewObjectSettings : NewObjectSettings
	{
		[DefaultValue( true )]
		[Category( "Options" )]
		public bool CreateDefaultFlares { get; set; } = true;

		public override bool Creation( NewObjectCell.ObjectCreationContext context )
		{
			var newObject2 = (LensFlares)context.newObject;

			if( CreateDefaultFlares )
				newObject2.CreateDefaultFlares();

			return base.Creation( context );
		}
	}
}
