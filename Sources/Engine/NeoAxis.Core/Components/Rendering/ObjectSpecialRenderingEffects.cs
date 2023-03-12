// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	[Editor.HCExpandable]
	public abstract class ObjectSpecialRenderingEffect
	{
		[Serialize]
		[DefaultValue( true )]
		public bool Enabled { get; set; } = true;
	}

	public class ObjectSpecialRenderingEffect_Outline : ObjectSpecialRenderingEffect
	{
		/// <summary>
		/// The index of the group to draw objects of the same group together.
		/// </summary>
		[Serialize]
		[DefaultValue( 0 )]
		[Range( 0, 20 )]
		public int Group { get; set; }

		/// <summary>
		/// The size multiplier of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 5, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public double Scale { get; set; } = 1.0;

		/// <summary>
		/// The color of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( "1 1 1" )]
		public ColorValue Color { get; set; } = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// Any data provided by the user. The value is provided to shaders and can be useful for making changes to the Outline effect.
		/// </summary>
		[Serialize]
		[DefaultValue( "0 0 0 0" )]
		public Vector4 AnyData { get; set; }

		//

		public override string ToString()
		{
			return "Outline";
		}
	}
}
