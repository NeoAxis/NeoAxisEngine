// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
		public Reference<string> AnyData
		{
			get { if( _anyData.BeginGet() ) AnyData = _anyData.Get( this ); return _anyData.value; }
			set { if( _anyData.BeginSet( this, ref value ) ) { try { AnyDataChanged?.Invoke( this ); } finally { _anyData.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AnyData"/> property value changes.</summary>
		public event Action<AnimationTrigger> AnyDataChanged;
		ReferenceField<string> _anyData = "";
	}
}
