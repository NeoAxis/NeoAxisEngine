// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Represents animation for the sprite.
	/// </summary>
	public class Component_SpriteAnimation : Component_Animation
	{
		/// <summary>
		/// The material of the animation.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<Component_SpriteAnimation> MaterialChanged;
		ReferenceField<Component_Material> _material;

		/////////////////////////////////////////

		protected virtual Component_SpriteAnimationFrame OnGetFrameByTime( double time )
		{
			//!!!!slowly

			//get reversed list to save the ability to overwrite part of basic animations by another animations
			foreach( var frame in GetComponents<Component_SpriteAnimationFrame>( true ) )
			{
				if( frame.Enabled )
				{
					var interval = frame.TimeInterval.Value;
					if( time >= interval.Minimum && time <= interval.Maximum )
						return frame;
				}
			}

			return null;
		}

		public delegate void GetFrameByTimeEventDelegate( Component_SpriteAnimation sender, double time, ref Component_SpriteAnimationFrame result );
		public event GetFrameByTimeEventDelegate GetFrameByTimeEvent;

		public Component_SpriteAnimationFrame GetFrameByTime( double time )
		{
			var result = OnGetFrameByTime( time );
			GetFrameByTimeEvent?.Invoke( this, time, ref result );
			return result;
		}
	}

	/// <summary>
	/// Represents a key frame for the sprite animation.
	/// </summary>
	public class Component_SpriteAnimationFrame : Component
	{
		/// <summary>
		/// Time interval of the frame.
		/// </summary>
		[DefaultValue( "0 0" )]
		public Reference<Range> TimeInterval
		{
			get { if( _timeInterval.BeginGet() ) TimeInterval = _timeInterval.Get( this ); return _timeInterval.value; }
			set { if( _timeInterval.BeginSet( ref value ) ) { try { TimeIntervalChanged?.Invoke( this ); } finally { _timeInterval.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TimeInterval"/> property value changes.</summary>
		public event Action<Component_SpriteAnimationFrame> TimeIntervalChanged;
		ReferenceField<Range> _timeInterval = new Range( 0, 0 );

		/// <summary>
		/// The material of the frame.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<Component_SpriteAnimationFrame> MaterialChanged;
		ReferenceField<Component_Material> _material;

		/// <summary>
		/// UV texture coordinates of the frame.
		/// </summary>
		[DefaultValue( "0 1 1 0" )]
		public Reference<Rectangle> UV
		{
			get { if( _uV.BeginGet() ) UV = _uV.Get( this ); return _uV.value; }
			set { if( _uV.BeginSet( ref value ) ) { try { UVChanged?.Invoke( this ); } finally { _uV.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UV"/> property value changes.</summary>
		public event Action<Component_SpriteAnimationFrame> UVChanged;
		ReferenceField<Rectangle> _uV = new Rectangle( 0, 1, 1, 0 );
	}

}
