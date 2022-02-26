// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A shape in the form of a box of a <see cref="ParticleEmitter">particle system emitter</see>.
	/// </summary>
	[NewObjectDefaultName( "Box Shape" )]
	public class ParticleEmitterShape_Box : ParticleEmitterShape
	{
		/// <summary>
		/// The size of the box.
		/// </summary>
		[Serialize]
		[DefaultValue( "1 1 1" )]
		public Reference<Vector3> Dimensions
		{
			get { if( _dimensions.BeginGet() ) Dimensions = _dimensions.Get( this ); return _dimensions.value; }
			set { if( _dimensions.BeginSet( ref value ) ) { try { DimensionsChanged?.Invoke( this ); } finally { _dimensions.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Dimensions"/> property value changes.</summary>
		public event Action<ParticleEmitterShape_Box> DimensionsChanged;
		ReferenceField<Vector3> _dimensions = new Vector3( 1, 1, 1 );

		/////////////////////////////////////////

		protected override void OnRender( Viewport viewport, Transform emitterTransform, bool solid, ref int verticesRendered )
		{
			base.OnRender( viewport, emitterTransform, solid, ref verticesRendered );

			Matrix4 t = emitterTransform.ToMatrix4();
			var local = Transform.Value;
			if( !local.IsIdentity )
				t *= local.ToMatrix4();

			var d = Dimensions.Value;
			viewport.Simple3DRenderer.AddBox( new Box( new Bounds( -d * .5, d * .5 ) ) * t, solid );
			verticesRendered += solid ? 8 : 96;
		}

		protected override void OnGetLocalPosition( FastRandom random, out Vector3 result )
		{
			var d = Dimensions.Value;
			result.X = random.Next( d.X ) - d.X * 0.5;
			result.Y = random.Next( d.Y ) - d.Y * 0.5;
			result.Z = random.Next( d.Z ) - d.Z * 0.5;
		}
	}
}
