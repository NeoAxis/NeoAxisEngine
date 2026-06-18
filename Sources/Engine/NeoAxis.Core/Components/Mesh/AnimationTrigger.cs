// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents a trigger point for animation.
	/// </summary>
	[NewObjectDefaultName( "Trigger" )]
	public class AnimationTrigger : Component
	{
		/// <summary>
		/// The time to trigger.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> Time
		{
			get { if( _time.BeginGet() ) Time = _time.Get( this ); return _time.value; }
			set { if( _time.BeginSet( this, ref value ) ) { try { TimeChanged?.Invoke( this ); } finally { _time.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Time"/> property value changes.</summary>
		public event Action<AnimationTrigger> TimeChanged;
		ReferenceField<double> _time = 0.0;

		/// <summary>
		/// Any user data as string.
		/// </summary>
		[DefaultValue( "" )]
		public Reference<string> AnyDataTrigger
		{
			get { if( _anyDataTrigger.BeginGet() ) AnyDataTrigger = _anyDataTrigger.Get( this ); return _anyDataTrigger.value; }
			set { if( _anyDataTrigger.BeginSet( this, ref value ) ) { try { AnyDataTriggerChanged?.Invoke( this ); } finally { _anyDataTrigger.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AnyDataTrigger"/> property value changes.</summary>
		public event Action<AnimationTrigger> AnyDataTriggerChanged;
		ReferenceField<string> _anyDataTrigger = "";
	}
}
