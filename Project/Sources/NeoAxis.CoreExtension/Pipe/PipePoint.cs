// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents a point of the pipe.
	/// </summary>
	public class PipePoint : CurveInSpacePoint
	{
		public enum SpecialtyEnum
		{
			None,
			OpenHole,
			Cap,
			Socket,
			Holder,
		}

		/// <summary>
		/// Whether to add special mesh to the point.
		/// </summary>
		[DefaultValue( SpecialtyEnum.None )]
		public Reference<SpecialtyEnum> Specialty
		{
			get { if( _specialty.BeginGet() ) Specialty = _specialty.Get( this ); return _specialty.value; }
			set { if( _specialty.BeginSet( ref value ) ) { try { SpecialtyChanged?.Invoke( this ); DataWasChanged(); } finally { _specialty.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Specialty"/> property value changes.</summary>
		public event Action<PipePoint> SpecialtyChanged;
		ReferenceField<SpecialtyEnum> _specialty = SpecialtyEnum.None;
	}
}
