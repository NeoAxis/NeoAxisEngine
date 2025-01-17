// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Represents a group of elements for configuring <see cref="Surface"/>.
	/// </summary>
	public class SurfaceGroupOfElements : Component
	{
		/// <summary>
		/// The probability of choosing this group of elements from others when painting.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 10.0, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> Probability
		{
			get { if( _probability.BeginGet() ) Probability = _probability.Get( this ); return _probability.value; }
			set { if( _probability.BeginSet( this, ref value ) ) { try { ProbabilityChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _probability.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Probability"/> property value changes.</summary>
		public event Action<SurfaceGroupOfElements> ProbabilityChanged;
		ReferenceField<double> _probability = 1.0;

		/// <summary>
		/// The radius of the occupied area in which no other objects will be created.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> OccupiedAreaRadius
		{
			get { if( _occupiedAreaRadius.BeginGet() ) OccupiedAreaRadius = _occupiedAreaRadius.Get( this ); return _occupiedAreaRadius.value; }
			set { if( _occupiedAreaRadius.BeginSet( this, ref value ) ) { try { OccupiedAreaRadiusChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _occupiedAreaRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OccupiedAreaRadius"/> property value changes.</summary>
		public event Action<SurfaceGroupOfElements> OccupiedAreaRadiusChanged;
		ReferenceField<double> _occupiedAreaRadius = 1.0;

		//!!!!impl
		///// <summary>
		///// Whether to ignore occupied areas of other groups.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> OverlapWithOtherGroups
		//{
		//	get { if( _overlapWithOtherGroups.BeginGet() ) OverlapWithOtherGroups = _overlapWithOtherGroups.Get( this ); return _overlapWithOtherGroups.value; }
		//	set { if( _overlapWithOtherGroups.BeginSet( this, ref value ) ) { try { OverlapWithOtherGroupsChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _overlapWithOtherGroups.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverlapWithOtherGroups"/> property value changes.</summary>
		//public event Action<SurfaceGroupOfElements> OverlapWithOtherGroupsChanged;
		//ReferenceField<bool> _overlapWithOtherGroups = false;

		//public enum VerticalAlignmentEnum
		//{
		//	
		//}

		//VerticalAlignment

		/// <summary>
		/// The range of possible Z-axis object position displacements when painting.
		/// </summary>
		[DefaultValue( "0 0" )]
		[DisplayName( "Position Z Range" )]
		public Reference<Range> PositionZRange
		{
			get { if( _positionZRange.BeginGet() ) PositionZRange = _positionZRange.Get( this ); return _positionZRange.value; }
			set { if( _positionZRange.BeginSet( this, ref value ) ) { try { PositionZRangeChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _positionZRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionZRange"/> property value changes.</summary>
		public event Action<SurfaceGroupOfElements> PositionZRangeChanged;
		ReferenceField<Range> _positionZRange = new Range( 0, 0 );

		/// <summary>
		/// Whether to orient elements depending by the normal of the surface (ground).
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> RotateBySurfaceNormal
		{
			get { if( _rotateBySurfaceNormal.BeginGet() ) RotateBySurfaceNormal = _rotateBySurfaceNormal.Get( this ); return _rotateBySurfaceNormal.value; }
			set { if( _rotateBySurfaceNormal.BeginSet( this, ref value ) ) { try { RotateBySurfaceNormalChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _rotateBySurfaceNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotateBySurfaceNormal"/> property value changes.</summary>
		public event Action<SurfaceGroupOfElements> RotateBySurfaceNormalChanged;
		ReferenceField<bool> _rotateBySurfaceNormal = false;

		/// <summary>
		/// Whether to set the random value of object rotation around its axis when painting.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> RotateAroundItsAxis
		{
			get { if( _rotateAroundItsAxis.BeginGet() ) RotateAroundItsAxis = _rotateAroundItsAxis.Get( this ); return _rotateAroundItsAxis.value; }
			set { if( _rotateAroundItsAxis.BeginSet( this, ref value ) ) { try { RotateAroundItsAxisChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _rotateAroundItsAxis.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotateAroundItsAxis"/> property value changes.</summary>
		public event Action<SurfaceGroupOfElements> RotateAroundItsAxisChanged;
		ReferenceField<bool> _rotateAroundItsAxis = false;

		/// <summary>
		/// The maximum vertical angle of objects.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<Degree> MaxIncline
		{
			get { if( _maxIncline.BeginGet() ) MaxIncline = _maxIncline.Get( this ); return _maxIncline.value; }
			set { if( _maxIncline.BeginSet( this, ref value ) ) { try { MaxInclineChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _maxIncline.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxIncline"/> property value changes.</summary>
		public event Action<SurfaceGroupOfElements> MaxInclineChanged;
		ReferenceField<Degree> _maxIncline = new Degree( 0.0 );

		/// <summary>
		/// The range of possible scaling of objects when painting.
		/// </summary>
		[DefaultValue( "1 1" )]
		public Reference<Range> ScaleRange
		{
			get { if( _scaleRange.BeginGet() ) ScaleRange = _scaleRange.Get( this ); return _scaleRange.value; }
			set { if( _scaleRange.BeginSet( this, ref value ) ) { try { ScaleRangeChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _scaleRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ScaleRange"/> property value changes.</summary>
		public event Action<SurfaceGroupOfElements> ScaleRangeChanged;
		ReferenceField<Range> _scaleRange = new Range( 1, 1 );

		/// <summary>
		/// The step of the tiling when this regular alignment mode is enabled.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> RegularAlignment
		{
			get { if( _regularAlignment.BeginGet() ) RegularAlignment = _regularAlignment.Get( this ); return _regularAlignment.value; }
			set { if( _regularAlignment.BeginSet( this, ref value ) ) { try { RegularAlignmentChanged?.Invoke( this ); ShouldRecompileSurface(); } finally { _regularAlignment.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RegularAlignment"/> property value changes.</summary>
		public event Action<SurfaceGroupOfElements> RegularAlignmentChanged;
		ReferenceField<double> _regularAlignment = 0.0;

		/////////////////////////////////////////

		public SurfaceElement GetElement( int index )
		{
			//!!!!slowly?

			var elements = GetComponents<SurfaceElement>();
			if( index >= 0 && index < elements.Length )
				return elements[ index ];
			return null;
		}

		[Browsable( false )]
		public Surface ParentSurface
		{
			get { return Parent as Surface; }
		}

		public void ShouldRecompileSurface()
		{
			var surface = ParentSurface;
			if( surface != null )
				surface.ShouldRecompile = true;
		}

		protected override void OnAddedToParent()
		{
			base.OnAddedToParent();

			ShouldRecompileSurface();
		}

		protected override void OnRemovedFromParent( Component oldParent )
		{
			base.OnRemovedFromParent( oldParent );

			var surface = oldParent as Surface;
			if( surface != null )
				surface.ShouldRecompile = true;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			ShouldRecompileSurface();
		}
	}
}
