// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents the point of the fence in the scene.
	/// </summary>
	public class FencePoint : CurveInSpacePoint
	{
		public enum SpecialtyEnum
		{
			None,
			NoPost,
		}

		/// <summary>
		/// Whether to add special mesh to the point.
		/// </summary>
		[DefaultValue( SpecialtyEnum.None )]
		public Reference<SpecialtyEnum> Specialty
		{
			get { if( _specialty.BeginGet() ) Specialty = _specialty.Get( this ); return _specialty.value; }
			set { if( _specialty.BeginSet( this, ref value ) ) { try { SpecialtyChanged?.Invoke( this ); DataWasChanged(); } finally { _specialty.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Specialty"/> property value changes.</summary>
		public event Action<FencePoint> SpecialtyChanged;
		ReferenceField<SpecialtyEnum> _specialty = SpecialtyEnum.None;
	}
}
