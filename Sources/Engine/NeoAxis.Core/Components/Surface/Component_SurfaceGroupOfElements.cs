// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Represents a group of elements for configuring <see cref="Component_Surface"/>.
	/// </summary>
	public class Component_SurfaceGroupOfElements : Component
	{
		/// <summary>
		/// The probability of choosing this group of elements from others when painting.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 10.0, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> Probability
		{
			get { if( _probability.BeginGet() ) Probability = _probability.Get( this ); return _probability.value; }
			set { if( _probability.BeginSet( ref value ) ) { try { ProbabilityChanged?.Invoke( this ); } finally { _probability.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Probability"/> property value changes.</summary>
		public event Action<Component_SurfaceGroupOfElements> ProbabilityChanged;
		ReferenceField<double> _probability = 1.0;

		/// <summary>
		/// The radius of the occupied area in which no other objects will be created.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> OccupiedAreaRadius
		{
			get { if( _occupiedAreaRadius.BeginGet() ) OccupiedAreaRadius = _occupiedAreaRadius.Get( this ); return _occupiedAreaRadius.value; }
			set { if( _occupiedAreaRadius.BeginSet( ref value ) ) { try { OccupiedAreaRadiusChanged?.Invoke( this ); } finally { _occupiedAreaRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OccupiedAreaRadius"/> property value changes.</summary>
		public event Action<Component_SurfaceGroupOfElements> OccupiedAreaRadiusChanged;
		ReferenceField<double> _occupiedAreaRadius = 1.0;

		//!!!!impl
		///// <summary>
		///// Whether to ignore occupied areas of other groups.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> OverlapWithOtherGroups
		//{
		//	get { if( _overlapWithOtherGroups.BeginGet() ) OverlapWithOtherGroups = _overlapWithOtherGroups.Get( this ); return _overlapWithOtherGroups.value; }
		//	set { if( _overlapWithOtherGroups.BeginSet( ref value ) ) { try { OverlapWithOtherGroupsChanged?.Invoke( this ); } finally { _overlapWithOtherGroups.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverlapWithOtherGroups"/> property value changes.</summary>
		//public event Action<Component_SurfaceGroupOfElements> OverlapWithOtherGroupsChanged;
		//ReferenceField<bool> _overlapWithOtherGroups = false;

		//!!!!impl
		//[DefaultValue( false )]
		//public Reference<bool> ApplyNormalOfBase
		//{
		//	get { if( _applyNormalOfBase.BeginGet() ) ApplyNormalOfBase = _applyNormalOfBase.Get( this ); return _applyNormalOfBase.value; }
		//	set { if( _applyNormalOfBase.BeginSet( ref value ) ) { try { ApplyNormalOfBaseChanged?.Invoke( this ); } finally { _applyNormalOfBase.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ApplyNormalOfBase"/> property value changes.</summary>
		//public event Action<Component_SurfaceGroupOfElements> ApplyNormalOfBaseChanged;
		//ReferenceField<bool> _applyNormalOfBase = false;

		/// <summary>
		/// The range of possible Z-axis object position displacements when painting.
		/// </summary>
		[DefaultValue( "0 0" )]
		[DisplayName( "Position Z Range" )]
		public Reference<Range> PositionZRange
		{
			get { if( _positionZRange.BeginGet() ) PositionZRange = _positionZRange.Get( this ); return _positionZRange.value; }
			set { if( _positionZRange.BeginSet( ref value ) ) { try { PositionZRangeChanged?.Invoke( this ); } finally { _positionZRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionZRange"/> property value changes.</summary>
		public event Action<Component_SurfaceGroupOfElements> PositionZRangeChanged;
		ReferenceField<Range> _positionZRange = new Range( 0, 0 );

		/// <summary>
		/// Whether to set the random value of objects rotation around its axis when painting.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> RotateAroundItsAxis
		{
			get { if( _rotateAroundItsAxis.BeginGet() ) RotateAroundItsAxis = _rotateAroundItsAxis.Get( this ); return _rotateAroundItsAxis.value; }
			set { if( _rotateAroundItsAxis.BeginSet( ref value ) ) { try { RotateAroundItsAxisChanged?.Invoke( this ); } finally { _rotateAroundItsAxis.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotateAroundItsAxis"/> property value changes.</summary>
		public event Action<Component_SurfaceGroupOfElements> RotateAroundItsAxisChanged;
		ReferenceField<bool> _rotateAroundItsAxis = false;

		/// <summary>
		/// The maximum vertical angle of objects.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<Degree> MaxIncline
		{
			get { if( _maxIncline.BeginGet() ) MaxIncline = _maxIncline.Get( this ); return _maxIncline.value; }
			set { if( _maxIncline.BeginSet( ref value ) ) { try { MaxInclineChanged?.Invoke( this ); } finally { _maxIncline.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxIncline"/> property value changes.</summary>
		public event Action<Component_SurfaceGroupOfElements> MaxInclineChanged;
		ReferenceField<Degree> _maxIncline = new Degree( 0.0 );

		/// <summary>
		/// The range of possible scaling of objects when painting.
		/// </summary>
		[DefaultValue( "1 1" )]
		public Reference<Range> ScaleRange
		{
			get { if( _scaleRange.BeginGet() ) ScaleRange = _scaleRange.Get( this ); return _scaleRange.value; }
			set { if( _scaleRange.BeginSet( ref value ) ) { try { ScaleRangeChanged?.Invoke( this ); } finally { _scaleRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ScaleRange"/> property value changes.</summary>
		public event Action<Component_SurfaceGroupOfElements> ScaleRangeChanged;
		ReferenceField<Range> _scaleRange = new Range( 1, 1 );

		/////////////////////////////////////////

		public Component_SurfaceElement GetElement( int index )
		{
			//!!!!slowly?

			var elements = GetComponents<Component_SurfaceElement>();
			if( index >= 0 && index < elements.Length )
				return elements[ index ];
			return null;
		}
	}
}
