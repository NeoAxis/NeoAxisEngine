// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A shape in the form of a cylinder of a <see cref="Component_ParticleEmitter">particle system emitter</see>.
	/// </summary>
	[NewObjectDefaultName( "Cylinder Shape" )]
	public class Component_ParticleEmitterShape_Cylinder : Component_ParticleEmitterShape
	{
		/// <summary>
		/// The radius of the cylinder.
		/// </summary>
		[Serialize]
		[DefaultValue( "0.5" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _radius.BeginSet( ref value ) ) { try { RadiusChanged?.Invoke( this ); } finally { _radius.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<Component_ParticleEmitterShape_Cylinder> RadiusChanged;
		ReferenceField<double> _radius = 0.5;

		/// <summary>
		/// The height of the cylinder.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Height
		{
			get { if( _height.BeginGet() ) Height = _height.Get( this ); return _height.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _height.BeginSet( ref value ) ) { try { HeightChanged?.Invoke( this ); } finally { _height.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<Component_ParticleEmitterShape_Cylinder> HeightChanged;
		ReferenceField<double> _height = 1;

		/////////////////////////////////////////

		protected override void OnRender( Viewport viewport, Transform emitterTransform, bool solid, ref int verticesRendered )
		{
			base.OnRender( viewport, emitterTransform, solid, ref verticesRendered );

			Matrix4 t = emitterTransform.ToMatrix4();
			var local = Transform.Value;
			if( !local.IsIdentity )
				t *= local.ToMatrix4();

			viewport.Simple3DRenderer.AddCylinder( t, 0, Radius.Value, Height.Value, 16, solid );

			if( solid )
				verticesRendered += 16 * 2 + 2;
			else
			{
				verticesRendered += 16 * 8;
				verticesRendered += 16 * 2 * 8;
			}
		}

		protected override void OnGetLocalPosition( Random random, out Vector3 result )
		{
			var a = random.Next( MathEx.PI * 2 );
			var r = MathEx.Sqrt( random.NextFloat() ) * Radius;

			result.X = (float)( random.NextFloat() * Height.Value - Height.Value / 2 );
			result.Y = r * MathEx.Cos( a );
			result.Z = r * MathEx.Sin( a );
		}
	}
}
