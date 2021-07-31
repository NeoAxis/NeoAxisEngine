// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A shape in the form of a sphere of a <see cref="Component_ParticleEmitter">particle system emitter</see>.
	/// </summary>
	[NewObjectDefaultName( "Sphere Shape" )]
	public class Component_ParticleEmitterShape_Sphere : Component_ParticleEmitterShape
	{
		/// <summary>
		/// The radius of the sphere.
		/// </summary>
		[DefaultValue( 0.5f )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<float> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set { if( _radius.BeginSet( ref value ) ) { try { RadiusChanged?.Invoke( this ); } finally { _radius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<Component_ParticleEmitterShape_Sphere> RadiusChanged;
		ReferenceField<float> _radius = 0.5f;

		//!!!!
		//[DefaultValue( 0.5f )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<float> Thickness
		//{
		//	get { if( _thickness.BeginGet() ) Thickness = _thickness.Get( this ); return _thickness.value; }
		//	set { if( _thickness.BeginSet( ref value ) ) { try { ThicknessChanged?.Invoke( this ); } finally { _thickness.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Thickness"/> property value changes.</summary>
		//public event Action<Component_ParticleEmitterShape_Sphere> ThicknessChanged;
		//ReferenceField<float> _thickness = 0.5f;

		/////////////////////////////////////////

		protected override void OnRender( Viewport viewport, Transform emitterTransform, bool solid, ref int verticesRendered )
		{
			base.OnRender( viewport, emitterTransform, solid, ref verticesRendered );

			Matrix4 t = emitterTransform.ToMatrix4();
			var local = Transform.Value;
			if( !local.IsIdentity )
				t *= local.ToMatrix4();

			viewport.Simple3DRenderer.AddSphere( t, Radius.Value, 16, solid );

			if( solid )
			{
				int segments = 16;
				int hSegments = segments;
				int vSegments = ( segments + 1 ) / 2 * 2;
				verticesRendered += hSegments * ( vSegments - 1 ) + 2;
			}
			else
				verticesRendered += 16 * 3 * 8;
		}

		protected override void OnGetLocalPosition( Random random, out Vector3 result )
		{
			var u = random.NextFloat();
			var v = random.NextFloat();
			var theta = u * 2.0f * MathEx.PI;
			var phi = MathEx.Acos( 2.0f * v - 1.0f );
			var r = MathEx.Pow( random.NextFloat(), 1.0f / 3.0f );//var r = Math.Cbrt( random.NextFloat() );
			var sinTheta = MathEx.Sin( theta );
			var cosTheta = MathEx.Cos( theta );
			var sinPhi = MathEx.Sin( phi );
			var cosPhi = MathEx.Cos( phi );
			var x = r * sinPhi * cosTheta;
			var y = r * sinPhi * sinTheta;
			var z = r * cosPhi;
			result = new Vector3F( x, y, z ) * Radius;
		}
	}
}
