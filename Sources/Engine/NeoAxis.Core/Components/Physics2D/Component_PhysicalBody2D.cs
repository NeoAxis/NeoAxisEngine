// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.ComponentModel;
using tainicom.Aether.Physics2D.Dynamics;

namespace NeoAxis
{
	/// <summary>
	/// Base class for rigid and soft body 2D components.
	/// </summary>
	public abstract class Component_PhysicalBody2D : Component_ObjectInSpace, Component_IPhysicalObject
	{
		public abstract void UpdateDataFromPhysicsEngine();

		[Browsable( false )]
		public abstract Body Physics2DBody { get; }

		public abstract void Render( ViewportRenderingContext context, out int verticesRendered );
	}
}
