// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Base class of a shape of a <see cref="ParticleEmitter">particle system emitter</see>.
	/// </summary>
	[NewObjectDefaultName( "Shape" )]
	public class ParticleEmitterShape : ParticleModule
	{
		/// <summary>
		/// The position, rotation and scale of the shape.
		/// </summary>
		[DefaultValue( "0 0 0; 0 0 0 1; 1 1 1" )]
		[Category( "Shape" )]
		public Reference<Transform> Transform
		{
			get { if( _transform.BeginGet() ) Transform = _transform.Get( this ); return _transform.value; }
			set
			{
				//!!!!new. так?
				//fix invalid value
				if( value.Value == null )
					value = new Reference<Transform>( NeoAxis.Transform.Identity, value.GetByReference );
				if( _transform.BeginSet( ref value ) )
				{
					try
					{
						TransformChanged?.Invoke( this );
						//OnTransformChanged();
					}
					finally { _transform.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Transform"/> property value changes.</summary>
		public event Action<ParticleEmitterShape> TransformChanged;
		ReferenceField<Transform> _transform = new Transform( Vector3.Zero, Quaternion.Identity, Vector3.One );

		/// <summary>
		/// The probability of choosing this shape from others when emitting particle.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 10.0, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Shape" )]
		public Reference<double> Probability
		{
			get { if( _probability.BeginGet() ) Probability = _probability.Get( this ); return _probability.value; }
			set { if( _probability.BeginSet( ref value ) ) { try { ProbabilityChanged?.Invoke( this ); /*ShouldRecompile();*/ } finally { _probability.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Probability"/> property value changes.</summary>
		public event Action<ParticleEmitterShape> ProbabilityChanged;
		ReferenceField<double> _probability = 1.0;

		/////////////////////////////////////////

		protected virtual void OnRender( Viewport viewport, Transform emitterTransform, bool solid, ref int verticesRendered )
		{
			Matrix4 t = emitterTransform.ToMatrix4();
			var local = Transform.Value;
			if( !local.IsIdentity )
				t *= local.ToMatrix4();

			var pos = t.GetTranslation();
			var rot = t.ToMatrix3();

			//Direction arrow
			{
				//!!!!TransformToolConfig

				var toolSize = viewport.Simple3DRenderer.GetThicknessByPixelSize( pos, ProjectSettings.Get.SceneEditor.TransformToolSizeScaled );
				double thickness = viewport.Simple3DRenderer.GetThicknessByPixelSize( pos, ProjectSettings.Get.SceneEditor.TransformToolLineThicknessScaled );
				var length = toolSize / 1.5 * 0.5;
				var headHeight = length / 4;

				viewport.Simple3DRenderer.AddArrow( pos, pos + rot * new Vector3( length, 0, 0 ), headHeight, 0, true, thickness );
			}
		}

		public delegate void RenderDelegate( ParticleEmitterShape sender, Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered );
		public event RenderDelegate Render;

		public void PerformRender( Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered )
		{
			OnRender( viewport, bodyTransform, solid, ref verticesRendered );
			Render?.Invoke( this, viewport, bodyTransform, solid, ref verticesRendered );
		}

		protected virtual void OnGetLocalPosition( FastRandom random, out Vector3 value )
		{
			value = Vector3.Zero;
		}

		public delegate void GetLocalPositionDelegate( ParticleEmitterShape sender, ref Vector3 value );
		public event GetLocalPositionDelegate GetLocalPosition;

		public void PerformGetLocalPosition( FastRandom random, out Vector3 value )
		{
			OnGetLocalPosition( random, out value );
			GetLocalPosition?.Invoke( this, ref value );
		}
	}
}
