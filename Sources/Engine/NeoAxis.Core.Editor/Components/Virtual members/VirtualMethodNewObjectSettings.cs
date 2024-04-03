// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	/// <summary>
	/// A set of settings for <see cref="VirtualMethod"/> creation in the editor.
	/// </summary>
	public class VirtualMethodNewObjectSettings : NewObjectSettings
	{
		[DefaultValue( true )]
		[Category( "Options" )]
		[DisplayName( "Flow Graph" )]
		public bool FlowGraph { get; set; } = true;

		public override bool Creation( NewObjectCell.ObjectCreationContext context )
		{
			var method = (VirtualMethod)context.newObject;

			if( FlowGraph )
				method.NewObjectCreateFlowGraph();

			return base.Creation( context );
		}
	}
}
#endif
