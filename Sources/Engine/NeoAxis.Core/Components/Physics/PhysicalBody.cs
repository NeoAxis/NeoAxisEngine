// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.ComponentModel;
using Internal.BulletSharp;
using Internal.BulletSharp.SoftBody;

namespace NeoAxis
{
	/// <summary>
	/// Base class for rigid and soft body components.
	/// </summary>
	public abstract class PhysicalBody : ObjectInSpace, IPhysicalObject
	{
		public abstract void UpdateDataFromPhysicsEngine();

		[Browsable( false )]
		public abstract CollisionObject BulletBody { get; }

		public abstract void Render( ViewportRenderingContext context, out int verticesRendered );
	}
}
