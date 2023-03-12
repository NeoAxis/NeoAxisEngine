// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.ComponentModel;

namespace NeoAxis
{
	//!!!!
	/// <summary>
	/// Base class for rigid and soft body components.
	/// </summary>
	public abstract class PhysicalBody : ObjectInSpace, IPhysicalObject
	{
		//public abstract void UpdateDataFromPhysicsEngine();

		//public abstract void RenderPhysicalObject( ViewportRenderingContext context, out int verticesRendered );
	}
}
