// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.ComponentModel;
using Internal.tainicom.Aether.Physics2D.Dynamics;

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
