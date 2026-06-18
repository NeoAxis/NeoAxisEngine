// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System.ComponentModel;
using Internal.nkast.Aether.Physics2D.Dynamics;

namespace NeoAxis
{
	/// <summary>
	/// Base class for rigid and soft body 2D components.
	/// </summary>
	public abstract class PhysicalBody2D : ObjectInSpace, IPhysicalObject
	{
		public abstract void UpdateDataFromPhysicsEngine();

		[Browsable( false )]
		public abstract Body Physics2DBody { get; }

		public abstract void RenderPhysicalObject( ViewportRenderingContext context, out int verticesRendered );
	}
}
