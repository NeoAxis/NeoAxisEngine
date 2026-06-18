// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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
