// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.ComponentModel;
using BulletSharp;
using BulletSharp.SoftBody;

namespace NeoAxis
{
	/// <summary>
	/// Base class for rigid and soft body components.
	/// </summary>
	public abstract class Component_PhysicalBody : Component_ObjectInSpace, Component_IPhysicalObject
	{
		public abstract void UpdateDataFromPhysicsEngine();

		[Browsable( false )]
		public abstract CollisionObject BulletBody { get; }

		public abstract void Render( ViewportRenderingContext context, out int verticesRendered );
	}
}
