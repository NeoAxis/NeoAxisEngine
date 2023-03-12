// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary methods for working with the 2D physics engine.
	/// </summary>
	static class Physics2DUtility
	{
		public static Vector2 Convert( Internal.tainicom.Aether.Physics2D.Common.Vector2 v )
		{
			return new Vector2( v.X, v.Y );
		}

		public static Internal.tainicom.Aether.Physics2D.Common.Vector2 Convert( Vector2 v )
		{
			return new Internal.tainicom.Aether.Physics2D.Common.Vector2( (float)v.X, (float)v.Y );
		}
	}
}
