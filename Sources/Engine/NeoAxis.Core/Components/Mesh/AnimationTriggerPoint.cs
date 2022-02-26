//// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Runtime.InteropServices;
//using System.Reflection;
//using System.IO;

//namespace NeoAxis
//{
//	//!!!!
//	/// <summary>
//	/// Animation time trigger.
//	/// </summary>
//	public class AnimationTriggerPoint : Component
//	{
//		/// <summary>
//		/// The time of the trigger.
//		/// </summary>
//		public Reference<double> Time
//		{
//			get { if( _time.BeginGet() ) Time = _time.Get( this ); return _time.value; }
//			set { if( _time.BeginSet( ref value ) ) { try { TimeChanged?.Invoke( this ); } finally { _time.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Time"/> property value changes.</summary>
//		public event Action<AnimationTriggerPoint> TimeChanged;
//		ReferenceField<double> _time;
//	}
//}
