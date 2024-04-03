// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents the curve in the scene.
	/// </summary>
	public class CurveInSpace : ObjectInSpace
	{
		bool needUpdateData;
		CachedData cachedData;

		bool needUpdateObjects;
		Mesh mesh;
		MeshInSpace meshInSpace;
		RigidBody collisionBody;

		/////////////////////////////////////////

		/// <summary>
		/// The type of the curve for position of the points.
		/// </summary>
		[DefaultValue( CurveTypeEnum.CubicSpline )]
		[Serialize]
		public Reference<CurveTypeEnum> CurveTypePosition
		{
			get { if( _curveTypePosition.BeginGet() ) CurveTypePosition = _curveTypePosition.Get( this ); return _curveTypePosition.value; }
			set { if( _curveTypePosition.BeginSet( this, ref value ) ) { try { CurveTypePositionChanged?.Invoke( this ); DataWasChanged(); } finally { _curveTypePosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurveTypePosition"/> property value changes.</summary>
		public event Action<CurveInSpace> CurveTypePositionChanged;
		ReferenceField<CurveTypeEnum> _curveTypePosition = CurveTypeEnum.CubicSpline;

		/// <summary>
		/// The type of the curve for rotation of the points.
		/// </summary>
		[DefaultValue( CurveTypeEnum.CubicSpline )]
		[Serialize]
		public Reference<CurveTypeEnum> CurveTypeRotation
		{
			get { if( _curveTypeRotation.BeginGet() ) CurveTypeRotation = _curveTypeRotation.Get( this ); return _curveTypeRotation.value; }
			set { if( _curveTypeRotation.BeginSet( this, ref value ) ) { try { CurveTypeRotationChanged?.Invoke( this ); DataWasChanged(); } finally { _curveTypeRotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurveTypeRotation"/> property value changes.</summary>
		public event Action<CurveInSpace> CurveTypeRotationChanged;
		ReferenceField<CurveTypeEnum> _curveTypeRotation = CurveTypeEnum.CubicSpline;

		/// <summary>
		/// The type of the curve for scale of the points.
		/// </summary>
		[DefaultValue( CurveTypeEnum.CubicSpline )]
		[Serialize]
		public Reference<CurveTypeEnum> CurveTypeScale
		{
			get { if( _curveTypeScale.BeginGet() ) CurveTypeScale = _curveTypeScale.Get( this ); return _curveTypeScale.value; }
			set { if( _curveTypeScale.BeginSet( this, ref value ) ) { try { CurveTypeScaleChanged?.Invoke( this ); DataWasChanged(); } finally { _curveTypeScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurveTypeScale"/> property value changes.</summary>
		public event Action<CurveInSpace> CurveTypeScaleChanged;
		ReferenceField<CurveTypeEnum> _curveTypeScale = CurveTypeEnum.CubicSpline;

		/// <summary>
		/// Time scale of the curve points.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Serialize]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> TimeScale
		{
			get { if( _timeScale.BeginGet() ) TimeScale = _timeScale.Get( this ); return _timeScale.value; }
			set { if( _timeScale.BeginSet( this, ref value ) ) { try { TimeScaleChanged?.Invoke( this ); DataWasChanged(); } finally { _timeScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TimeScale"/> property value changes.</summary>
		public event Action<CurveInSpace> TimeScaleChanged;
		ReferenceField<double> _timeScale = 1.0;

		/// <summary>
		/// Specifies the color of the curve.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Serialize]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set
			{
				if( _color.BeginSet( this, ref value ) )
				{
					try
					{
						ColorChanged?.Invoke( this );
						if( meshInSpace != null )
							meshInSpace.Color = Color;
					}
					finally { _color.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<CurveInSpace> ColorChanged;
		ReferenceField<ColorValue> _color = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// Whether to display the curve in the editor.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		public Reference<bool> DisplayCurveInEditor
		{
			get { if( _displayCurveInEditor.BeginGet() ) DisplayCurveInEditor = _displayCurveInEditor.Get( this ); return _displayCurveInEditor.value; }
			set { if( _displayCurveInEditor.BeginSet( this, ref value ) ) { try { DisplayCurveInEditorChanged?.Invoke( this ); DataWasChanged(); } finally { _displayCurveInEditor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayCurveInEditor"/> property value changes.</summary>
		public event Action<CurveInSpace> DisplayCurveInEditorChanged;
		ReferenceField<bool> _displayCurveInEditor = true;

		/// <summary>
		/// Whether to display the curve in the simulation.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		public Reference<bool> DisplayCurveInSimulation
		{
			get { if( _displayCurveInSimulation.BeginGet() ) DisplayCurveInSimulation = _displayCurveInSimulation.Get( this ); return _displayCurveInSimulation.value; }
			set { if( _displayCurveInSimulation.BeginSet( this, ref value ) ) { try { DisplayCurveInSimulationChanged?.Invoke( this ); DataWasChanged(); } finally { _displayCurveInSimulation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayCurveInSimulation"/> property value changes.</summary>
		public event Action<CurveInSpace> DisplayCurveInSimulationChanged;
		ReferenceField<bool> _displayCurveInSimulation = false;

		/// <summary>
		/// The thickness of the curve.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Thickness
		{
			get { if( _thickness.BeginGet() ) Thickness = _thickness.Get( this ); return _thickness.value; }
			set { if( _thickness.BeginSet( this, ref value ) ) { try { ThicknessChanged?.Invoke( this ); DataWasChanged(); } finally { _thickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Thickness"/> property value changes.</summary>
		public event Action<CurveInSpace> ThicknessChanged;
		ReferenceField<double> _thickness = 0.0;

		/// <summary>
		/// Whether to add caps to the geometry of the curve.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> Caps
		{
			get { if( _caps.BeginGet() ) Caps = _caps.Get( this ); return _caps.value; }
			set { if( _caps.BeginSet( this, ref value ) ) { try { CapsChanged?.Invoke( this ); DataWasChanged(); } finally { _caps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Caps"/> property value changes.</summary>
		public event Action<CurveInSpace> CapsChanged;
		ReferenceField<bool> _caps = true;

		/// <summary>
		/// The length of the segments.
		/// </summary>
		[DefaultValue( 0.2 )]
		[Range( 0.05, 1, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[Category( "Rendering" )]
		public Reference<double> SegmentsLength
		{
			get { if( _segmentsLength.BeginGet() ) SegmentsLength = _segmentsLength.Get( this ); return _segmentsLength.value; }
			set { if( _segmentsLength.BeginSet( this, ref value ) ) { try { SegmentsLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _segmentsLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SegmentsLength"/> property value changes.</summary>
		public event Action<CurveInSpace> SegmentsLengthChanged;
		ReferenceField<double> _segmentsLength = 0.2;

		/// <summary>
		/// The amount of segments around the circumference.
		/// </summary>
		[DefaultValue( 16 )]
		[Range( 3, 64, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Rendering" )]
		public Reference<int> SegmentsCircle
		{
			get { if( _segmentsCircle.BeginGet() ) SegmentsCircle = _segmentsCircle.Get( this ); return _segmentsCircle.value; }
			set { if( _segmentsCircle.BeginSet( this, ref value ) ) { try { SegmentsCircleChanged?.Invoke( this ); DataWasChanged(); } finally { _segmentsCircle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SegmentsCircle"/> property value changes.</summary>
		public event Action<CurveInSpace> SegmentsCircleChanged;
		ReferenceField<int> _segmentsCircle = 16;

		/// <summary>
		/// The material of the curve.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Rendering" )]
		public Reference<Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set { if( _material.BeginSet( this, ref value ) ) { try { MaterialChanged?.Invoke( this ); DataWasChanged(); } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<CurveInSpace> MaterialChanged;
		ReferenceField<Material> _material = null;

		/// <summary>
		/// The length of UV tile by length.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[DisplayName( "UV Tiles Length" )]
		[Category( "Rendering" )]
		public Reference<double> UVTilesLength
		{
			get { if( _uVTilesLength.BeginGet() ) UVTilesLength = _uVTilesLength.Get( this ); return _uVTilesLength.value; }
			set { if( _uVTilesLength.BeginSet( this, ref value ) ) { try { UVTilesLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _uVTilesLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVTilesLength"/> property value changes.</summary>
		public event Action<CurveInSpace> UVTilesLengthChanged;
		ReferenceField<double> _uVTilesLength = 1.0;

		/// <summary>
		/// The length of UV tile around the circumference.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DisplayName( "UV Tiles Circle" )]
		[Category( "Rendering" )]
		public Reference<double> UVTilesCircle
		{
			get { if( _uVTilesCircle.BeginGet() ) UVTilesCircle = _uVTilesCircle.Get( this ); return _uVTilesCircle.value; }
			set { if( _uVTilesCircle.BeginSet( this, ref value ) ) { try { UVTilesCircleChanged?.Invoke( this ); DataWasChanged(); } finally { _uVTilesCircle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVTilesCircle"/> property value changes.</summary>
		public event Action<CurveInSpace> UVTilesCircleChanged;
		ReferenceField<double> _uVTilesCircle = 1.0;

		/// <summary>
		/// Whether to flip UV coordinates.
		/// </summary>
		[DefaultValue( false )]
		[DisplayName( "UV Flip" )]
		[Category( "Rendering" )]
		public Reference<bool> UVFlip
		{
			get { if( _uVFlip.BeginGet() ) UVFlip = _uVFlip.Get( this ); return _uVFlip.value; }
			set { if( _uVFlip.BeginSet( this, ref value ) ) { try { UVFlipChanged?.Invoke( this ); DataWasChanged(); } finally { _uVFlip.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVFlip"/> property value changes.</summary>
		public event Action<CurveInSpace> UVFlipChanged;
		ReferenceField<bool> _uVFlip = false;

		/// <summary>
		/// The factor of maximum visibility distance. The maximum distance is calculated based on the size of the object on the screen.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> VisibilityDistanceFactor
		{
			get { if( _visibilityDistanceFactor.BeginGet() ) VisibilityDistanceFactor = _visibilityDistanceFactor.Get( this ); return _visibilityDistanceFactor.value; }
			set
			{
				if( _visibilityDistanceFactor.BeginSet( this, ref value ) )
				{
					try
					{
						VisibilityDistanceFactorChanged?.Invoke( this );
						if( mesh != null )
							mesh.VisibilityDistanceFactor = VisibilityDistanceFactor;
					}
					finally { _visibilityDistanceFactor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="VisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<CurveInSpace> VisibilityDistanceFactorChanged;
		ReferenceField<double> _visibilityDistanceFactor = 1.0;

		///// <summary>
		///// Maximum visibility range of the geometry.
		///// </summary>
		//[DefaultValue( 10000.0 )]
		//[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//[Category( "Rendering" )]
		//public Reference<double> VisibilityDistance
		//{
		//	get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
		//	set
		//	{
		//		if( _visibilityDistance.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				VisibilityDistanceChanged?.Invoke( this );
		//				if( mesh != null )
		//					mesh.VisibilityDistance = VisibilityDistance;
		//			}
		//			finally { _visibilityDistance.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		//public event Action<CurveInSpace> VisibilityDistanceChanged;
		//ReferenceField<double> _visibilityDistance = 10000.0;

		/// <summary>
		/// Whether to have a collision body.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Physics" )]
		public Reference<bool> Collision
		{
			get { if( _collision.BeginGet() ) Collision = _collision.Get( this ); return _collision.value; }
			set { if( _collision.BeginSet( this, ref value ) ) { try { CollisionChanged?.Invoke( this ); DataWasChanged(); } finally { _collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Collision"/> property value changes.</summary>
		public event Action<CurveInSpace> CollisionChanged;
		ReferenceField<bool> _collision = false;

		/// <summary>
		/// The physical material used by the rigid body.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Physics" )]
		public Reference<PhysicalMaterial> CollisionMaterial
		{
			get { if( _collisionMaterial.BeginGet() ) CollisionMaterial = _collisionMaterial.Get( this ); return _collisionMaterial.value; }
			set { if( _collisionMaterial.BeginSet( this, ref value ) ) { try { CollisionMaterialChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CollisionMaterial"/> property value changes.</summary>
		public event Action<CurveInSpace> CollisionMaterialChanged;
		ReferenceField<PhysicalMaterial> _collisionMaterial;

		//!!!!
		///// <summary>
		///// The type of friction applied on the rigid body.
		///// </summary>
		//[DefaultValue( PhysicalMaterial.FrictionModeEnum.Simple )]
		//[Category( "Physics" )]
		//public Reference<PhysicalMaterial.FrictionModeEnum> CollisionFrictionMode
		//{
		//	get { if( _collisionFrictionMode.BeginGet() ) CollisionFrictionMode = _collisionFrictionMode.Get( this ); return _collisionFrictionMode.value; }
		//	set { if( _collisionFrictionMode.BeginSet( this, ref value ) ) { try { CollisionFrictionModeChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionFrictionMode.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CollisionFrictionMode"/> property value changes.</summary>
		//public event Action<CurveInSpace> CollisionFrictionModeChanged;
		//ReferenceField<PhysicalMaterial.FrictionModeEnum> _collisionFrictionMode = PhysicalMaterial.FrictionModeEnum.Simple;

		/// <summary>
		/// The amount of friction applied on the rigid body.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		[Category( "Physics" )]
		public Reference<double> CollisionFriction
		{
			get { if( _collisionFriction.BeginGet() ) CollisionFriction = _collisionFriction.Get( this ); return _collisionFriction.value; }
			set { if( _collisionFriction.BeginSet( this, ref value ) ) { try { CollisionFrictionChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionFriction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CollisionFriction"/> property value changes.</summary>
		public event Action<CurveInSpace> CollisionFrictionChanged;
		ReferenceField<double> _collisionFriction = 0.5;

		//!!!!
		///// <summary>
		///// The amount of directional friction applied on the rigid body.
		///// </summary>
		//[DefaultValue( "1 1 1" )]
		//[Category( "Physics" )]
		//public Reference<Vector3> CollisionAnisotropicFriction
		//{
		//	get { if( _collisionAnisotropicFriction.BeginGet() ) CollisionAnisotropicFriction = _collisionAnisotropicFriction.Get( this ); return _collisionAnisotropicFriction.value; }
		//	set { if( _collisionAnisotropicFriction.BeginSet( this, ref value ) ) { try { CollisionAnisotropicFrictionChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionAnisotropicFriction.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CollisionAnisotropicFriction"/> property value changes.</summary>
		//public event Action<CurveInSpace> CollisionAnisotropicFrictionChanged;
		//ReferenceField<Vector3> _collisionAnisotropicFriction = Vector3.One;

		//!!!!
		///// <summary>
		///// The amount of friction applied when rigid body is spinning.
		///// </summary>
		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//[Category( "Physics" )]
		//public Reference<double> CollisionSpinningFriction
		//{
		//	get { if( _collisionSpinningFriction.BeginGet() ) CollisionSpinningFriction = _collisionSpinningFriction.Get( this ); return _collisionSpinningFriction.value; }
		//	set { if( _collisionSpinningFriction.BeginSet( this, ref value ) ) { try { CollisionSpinningFrictionChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionSpinningFriction.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CollisionSpinningFriction"/> property value changes.</summary>
		//public event Action<CurveInSpace> CollisionSpinningFrictionChanged;
		//ReferenceField<double> _collisionSpinningFriction = 0.5;

		//!!!!
		///// <summary>
		///// The amount of friction applied when rigid body is rolling.
		///// </summary>
		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//[Category( "Physics" )]
		//public Reference<double> CollisionRollingFriction
		//{
		//	get { if( _collisionRollingFriction.BeginGet() ) CollisionRollingFriction = _collisionRollingFriction.Get( this ); return _collisionRollingFriction.value; }
		//	set { if( _collisionRollingFriction.BeginSet( this, ref value ) ) { try { CollisionRollingFrictionChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionRollingFriction.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CollisionRollingFriction"/> property value changes.</summary>
		//public event Action<CurveInSpace> CollisionRollingFrictionChanged;
		//ReferenceField<double> _collisionRollingFriction = 0.5;

		/// <summary>
		/// The ratio of the final relative velocity to initial relative velocity of the rigid body after collision.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Category( "Physics" )]
		public Reference<double> CollisionRestitution
		{
			get { if( _collisionRestitution.BeginGet() ) CollisionRestitution = _collisionRestitution.Get( this ); return _collisionRestitution.value; }
			set { if( _collisionRestitution.BeginSet( this, ref value ) ) { try { CollisionRestitutionChanged?.Invoke( this ); UpdateCollisionMaterial(); } finally { _collisionRestitution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CollisionRestitution"/> property value changes.</summary>
		public event Action<CurveInSpace> CollisionRestitutionChanged;
		ReferenceField<double> _collisionRestitution;

		/// <summary>
		/// The time of the point.
		/// </summary>
		[Category( "Point" )]
		[DefaultValue( 0.0 )]
		[Serialize]
		public Reference<double> Time
		{
			get { if( _time.BeginGet() ) Time = _time.Get( this ); return _time.value; }
			set { if( _time.BeginSet( this, ref value ) ) { try { TimeChanged?.Invoke( this ); DataWasChanged(); } finally { _time.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Time"/> property value changes.</summary>
		public event Action<CurveInSpace> TimeChanged;
		ReferenceField<double> _time = 0.0;

		/// <summary>
		/// The curvature radius of the points for Rounded Line curve type.
		/// </summary>
		[Category( "Point" )]
		[DefaultValue( 1.0 )]
		[Range( 0.0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> RoundedLineCurvatureRadius
		{
			get { if( _roundedLineCurvatureRadius.BeginGet() ) RoundedLineCurvatureRadius = _roundedLineCurvatureRadius.Get( this ); return _roundedLineCurvatureRadius.value; }
			set { if( _roundedLineCurvatureRadius.BeginSet( this, ref value ) ) { try { RoundedLineCurvatureRadiusChanged?.Invoke( this ); DataWasChanged(); } finally { _roundedLineCurvatureRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RoundedLineCurvatureRadius"/> property value changes.</summary>
		public event Action<CurveInSpace> RoundedLineCurvatureRadiusChanged;
		ReferenceField<double> _roundedLineCurvatureRadius = 1.0;

		/////////////////////////////////////////

		/// <summary>
		/// Enumerates the types of curve.
		/// </summary>
		public enum CurveTypeEnum
		{
			CubicSpline,
			Bezier,
			Line,
			RoundedLine,
			BezierPath,
		}

		/////////////////////////////////////////

		/// <summary>
		/// Extracted source data of the curve.
		/// </summary>
		public class SourceData
		{
			public CurveTypeEnum CurveTypePosition;
			//public double RoundedLineCurvatureRadius;
			public CurveTypeEnum CurveTypeRotation;
			public CurveTypeEnum CurveTypeScale;
			public double TimeScale;
			/// <summary>
			/// Represents a point of <see cref="SourceData"/>.
			/// </summary>
			public struct Point
			{
				public Transform Transform;
				public double Time;
				public double RoundedLineCurvatureRadius;
			}
			public Point[] Points;

			//public bool Equals( SourceData data )
			//{
			//	if( CurveTypePosition != data.CurveTypePosition )
			//		return false;
			//	if( RoundedLineCurvatureRadius != data.RoundedLineCurvatureRadius )
			//		return false;
			//	if( CurveTypeRotation != data.CurveTypeRotation )
			//		return false;
			//	if( CurveTypeScale != data.CurveTypeScale )
			//		return false;
			//	if( TimeScale != data.TimeScale )
			//		return false;

			//	if( Points.Length != data.Points.Length )
			//		return false;
			//	for( int n = 0; n < Points.Length; n++ )
			//	{
			//		var p = Points[ n ];
			//		var p2 = data.Points[ n ];
			//		if( p.Transform != p2.Transform || p.Time != p2.Time )
			//			return false;
			//	}

			//	return true;
			//}
		}

		/////////////////////////////////////////

		/// <summary>
		/// Precalculated data of the curve.
		/// </summary>
		public class CachedData
		{
			public SourceData SourceData;

			public Curve PositionCurve;
			public Curve ForwardCurve;
			public Curve UpCurve;
			public Curve ScaleCurve;
			public Range TimeRange;

			public Curve PositionCurveNormalizedTime;
			public Curve ScaleCurveNormalizedTime;

			//

			public Vector3 GetPositionByNormalizedTime( double time )
			{
				return PositionCurveNormalizedTime.CalculateValueByTime( time );
			}

			public Vector3 GetScaleByNormalizedTime( double time )
			{
				return ScaleCurveNormalizedTime.CalculateValueByTime( time );
			}
		}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( RoundedLineCurvatureRadius ):
					if( CurveTypePosition.Value != CurveTypeEnum.RoundedLine )
						skip = true;
					break;

				case nameof( Caps ):
				case nameof( SegmentsLength ):
				case nameof( SegmentsCircle ):
				case nameof( Material ):
				case nameof( UVTilesLength ):
				case nameof( UVTilesCircle ):
				case nameof( UVFlip ):
				case nameof( VisibilityDistanceFactor ):
					if( Thickness.Value == 0 )
						skip = true;
					break;

				case nameof( Collision ):
					if( Thickness.Value == 0 )
						skip = true;
					break;

				case nameof( CollisionMaterial ):
					if( Thickness.Value == 0 || !Collision )
						skip = true;
					break;

				//!!!!
				//case nameof( CollisionFrictionMode ):
				//	if( Thickness.Value == 0 || !Collision || CollisionMaterial.Value != null )
				//		skip = true;
				//	break;

				case nameof( CollisionFriction ):
					if( Thickness.Value == 0 || !Collision || CollisionMaterial.Value != null )
						skip = true;
					break;

				//!!!!
				//case nameof( CollisionRollingFriction ):
				//case nameof( CollisionSpinningFriction ):
				//case nameof( CollisionAnisotropicFriction ):
				//	if( Thickness.Value == 0 || !Collision || CollisionFrictionMode.Value == PhysicalMaterial.FrictionModeEnum.Simple || CollisionMaterial.Value != null )
				//		skip = true;
				//	break;

				case nameof( CollisionRestitution ):
					if( Thickness.Value == 0 || !Collision || CollisionMaterial.Value != null )
						skip = true;
					break;

				}
			}
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			DataWasChanged();
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				if( !context.SceneDisplayDevelopmentDataInThisApplication )
					context2.disableShowingLabelForThisObject = true;

				//visualize handles for BezierPath
				if( EngineApp.IsEditor )
				{
					if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
					{
						if( CurveTypePosition.Value == CurveTypeEnum.BezierPath )
						{
							ColorValue color;
							if( context2.selectedObjects.Contains( this ) )
								color = ProjectSettings.Get.Colors.SelectedColor;
							else //if( context2.canSelectObjects.Contains( this ) )
								color = ProjectSettings.Get.Colors.CanSelectColor;
							color *= new ColorValue( 1, 1, 1, 0.5 );

							var renderer = context.Owner.Simple3DRenderer;
							renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

							//!!!!
							//CurveInSpacePointHandle

							var transform = TransformV;

							var offset = transform.Rotation.GetForward() * transform.Scale.MaxComponent();
							var handle1 = transform.Position - offset;
							var handle2 = transform.Position + offset;

							renderer.AddLine( transform.Position, handle1 );
							renderer.AddLine( transform.Position, handle2 );

							var size = transform.Scale.MaxComponent() * 0.025;
							renderer.AddSphere( handle1, size, 16, true );
							renderer.AddSphere( handle2, size, 16, true );
						}
					}
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = FindParent<Scene>();
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
				else
					scene.GetRenderSceneData -= Scene_GetRenderSceneData;

				UpdateData();
				UpdateObjects();
			}
		}

		public delegate void UpdateObjectsEventDelegate( CurveInSpace sender );
		public event UpdateObjectsEventDelegate UpdateObjectsEvent;

		void UpdateObjects()
		{
			DeleteCollisionObjects();
			DeleteRenderingObjects();

			if( EnabledInHierarchyAndIsInstance )
			{
				if( Thickness > 0 )
				{
					if( GetAllowDisplayCurve() )
						UpdateRenderingObjects();
					UpdateCollisionObjects();
				}
				SpaceBoundsUpdate();
			}

			UpdateObjectsEvent?.Invoke( this );

			needUpdateObjects = false;
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			var data = GetData();
			if( data != null && data.SourceData != null && data.SourceData.Points.Length != 0 )
			{
				var points = data.SourceData.Points;

				var bounds = new Bounds( points[ 0 ].Transform.Position );
				for( int n = 1; n < points.Length; n++ )
					bounds.Add( points[ n ].Transform.Position );
				bounds.Expand( Math.Max( Thickness * 0.5, 0.1 ) );
				newBounds = new SpaceBounds( bounds );
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( needUpdateData )
				UpdateData();
			if( needUpdateObjects )
				UpdateObjects();
		}

		protected virtual bool GetAllowDisplayCurve()
		{
			return true;
		}

		private void Scene_GetRenderSceneData( Scene scene, ViewportRenderingContext context )
		{
			var context2 = context.ObjectInSpaceRenderingContext;

			bool display = false;
			ColorValue color = Color;

			if( VisibleInHierarchy && GetAllowDisplayCurve() && Thickness == 0 )
			{
				if( ( context.SceneDisplayDevelopmentDataInThisApplication /*&& ParentScene.DisplayLights */) ||
					context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this )
				{
					if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
					{
						display = true;
						if( context2.selectedObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.SelectedColor;
						else
							color = ProjectSettings.Get.Colors.CanSelectColor;
					}

					if( EngineApp.IsEditor && DisplayCurveInEditor )
						display = true;
				}

				if( EngineApp.IsSimulation && DisplayCurveInSimulation )
					display = true;
			}

			if( display )
				RenderThickness0( context2, color );

			if( !context.SceneDisplayDevelopmentDataInThisApplication )
				context2.disableShowingLabelForThisObject = true;
		}

		public void RenderThickness0( RenderingContext context, ColorValue color )
		{
			var data = GetData();
			if( data != null && data.SourceData.Points.Length > 1 )
			{
				context.viewport.Simple3DRenderer.SetColor( color );

				//!!!!check visible in frustum

				//good?
				var step = data.TimeRange.Size / 20.0 / data.SourceData.Points.Length;

				var thickness = Thickness.Value;

				Vector3? lastPos = null;
				for( var t = data.TimeRange.Minimum; t < data.TimeRange.Maximum; t += step )
				{
					var pos = data.PositionCurve.CalculateValueByTime( t );
					if( lastPos.HasValue )
						context.viewport.Simple3DRenderer.AddLine( lastPos.Value, pos, thickness );
					lastPos = pos;
				}
				if( lastPos.HasValue )
					context.viewport.Simple3DRenderer.AddLine( lastPos.Value, data.PositionCurve.CalculateValueByTime( data.TimeRange.Maximum ), thickness );
			}
		}

		public Transform GetTransformByTime( double time )
		{
			var data = GetData();
			if( data != null )
				return GetTransformByTimeImpl( data, time, true, true, true );
			else
				return Transform;
		}

		public Vector3 GetPositionByTime( double time )
		{
			var data = GetData();
			if( data != null )
				return GetTransformByTimeImpl( data, time, true, false, false ).Position;
			else
				return Transform.Value.Position;
		}

		public Transform GetTransformBySceneTimeLooped()
		{
			var data = GetData();
			if( data != null )
			{
				var diff = data.TimeRange.Size;
				if( diff != 0 )
				{
					var sceneTime = ParentScene != null ? ParentScene.Time : 0;
					return GetTransformByTimeImpl( data, data.TimeRange.Minimum + sceneTime % diff, true, true, true );
				}
			}
			return Transform;
		}

		public Vector3 GetPositionBySceneTimeLooped()
		{
			var data = GetData();
			if( data != null )
			{
				var diff = data.TimeRange.Size;
				if( diff != 0 )
				{
					var sceneTime = ParentScene != null ? ParentScene.Time : 0;
					return GetTransformByTimeImpl( data, data.TimeRange.Minimum + sceneTime % diff, true, false, false ).Position;
				}
			}
			return Transform.Value.Position;
		}

		public Range GetTimeRange()
		{
			var data = GetData();
			return data != null ? data.TimeRange : Range.Zero;
		}

		public Curve CreateCurve( CurveTypeEnum type, bool allowBezierPath )
		{
			switch( type )
			{
			case CurveTypeEnum.CubicSpline: return new CurveCubicSpline();
			case CurveTypeEnum.Bezier: return new CurveBezier();
			case CurveTypeEnum.Line: return new CurveLine();
			case CurveTypeEnum.RoundedLine: return new CurveRoundedLine();
			case CurveTypeEnum.BezierPath:
				if( allowBezierPath )
					return new CurveBezierPath();
				else
					return new CurveCubicSpline();
			}

			Log.Fatal( "CurveInSpace: CreateCurve: no implementation." );
			return null;
		}

		SourceData GetSourceData()
		{
			var sourceData = new SourceData();
			sourceData.CurveTypePosition = CurveTypePosition;
			//sourceData.RoundedLineCurvatureRadius = RoundedLineCurvatureRadius;
			sourceData.CurveTypeRotation = CurveTypeRotation;
			sourceData.CurveTypeScale = CurveTypeScale;
			sourceData.TimeScale = TimeScale;

			var points = GetComponents<CurveInSpacePoint>( onlyEnabledInHierarchy: true );

			sourceData.Points = new SourceData.Point[ 1 + points.Length ];
			sourceData.Points[ 0 ] = new SourceData.Point() { Transform = Transform, Time = Time, RoundedLineCurvatureRadius = RoundedLineCurvatureRadius };
			for( int n = 0; n < points.Length; n++ )
				sourceData.Points[ n + 1 ] = new SourceData.Point() { Transform = points[ n ].Transform, Time = points[ n ].Time, RoundedLineCurvatureRadius = points[ n ].RoundedLineCurvatureRadius };

			//sort by time
			bool allPointsZeroTime = sourceData.Points.All( p => p.Time == 0 );
			if( !allPointsZeroTime )
			{
				CollectionUtility.MergeSort( sourceData.Points, delegate ( SourceData.Point a, SourceData.Point b )
				{
					if( a.Time < b.Time )
						return -1;
					if( a.Time > b.Time )
						return 1;
					return 0;
				} );
			}

			return sourceData;
		}

		virtual protected bool GetAllowUpdateData() { return true; }

		void UpdateData()
		{
			if( needUpdateData && EnabledInHierarchyAndIsInstance && GetAllowUpdateData() )
			{
				cachedData = null;

				var sourceData = GetSourceData();
				if( sourceData.Points.Length != 0 )
				{
					var data = new CachedData();
					data.SourceData = sourceData;
					data.PositionCurve = CreateCurve( CurveTypePosition, true );
					data.ForwardCurve = CreateCurve( CurveTypeRotation, false );
					data.UpCurve = CreateCurve( CurveTypeRotation, false );
					data.ScaleCurve = CreateCurve( CurveTypeScale, false );
					data.PositionCurveNormalizedTime = CreateCurve( CurveTypePosition, false );
					data.ScaleCurveNormalizedTime = CreateCurve( CurveTypeScale, false );

					bool allPointsZeroTime = sourceData.Points.All( p => p.Time == 0 );

					for( int n = 0; n < sourceData.Points.Length; n++ )
					{
						var point = sourceData.Points[ n ];

						double time = ( allPointsZeroTime ? n : point.Time ) * sourceData.TimeScale;

						var transform = point.Transform;

						object additionalData = null;

						var rounded = data.PositionCurve as CurveRoundedLine;
						if( rounded != null )
							additionalData = point.RoundedLineCurvatureRadius;

						var bezierPath = data.PositionCurve as CurveBezierPath;
						if( bezierPath != null )
						{
							//!!!!
							//CurveInSpacePointHandle

							var offset = transform.Rotation.GetForward() * transform.Scale.MaxComponent();
							var handle1 = transform.Position - offset;
							var handle2 = transform.Position + offset;

							additionalData = (handle1, handle2);
						}

						data.PositionCurve.AddPoint( time, transform.Position, additionalData );
						data.PositionCurveNormalizedTime.AddPoint( n, transform.Position, additionalData );

						transform.Rotation.GetForward( out var forward );
						transform.Rotation.GetUp( out var up );
						data.ForwardCurve.AddPoint( time, forward );
						data.UpCurve.AddPoint( time, up );

						data.ScaleCurve.AddPoint( time, transform.Scale );
						data.ScaleCurveNormalizedTime.AddPoint( n, transform.Scale );
					}

					data.TimeRange = new Range( data.PositionCurve.Points[ 0 ].Time, data.PositionCurve.Points[ sourceData.Points.Length - 1 ].Time );

					cachedData = data;
				}

				needUpdateData = false;
				needUpdateObjects = true;
			}
		}

		public CachedData GetData()
		{
			if( needUpdateData )
				UpdateData();
			return cachedData;
		}

		static Transform GetTransformByTimeImpl( CachedData data, double time, bool getPosition, bool getRotation, bool getScale )
		{
			var pos = Vector3.Zero;
			var rot = Quaternion.Identity;
			var scl = Vector3.One;

			if( getPosition )
				pos = data.PositionCurve.CalculateValueByTime( time );

			if( getRotation )
			{
				Vector3 forward = data.ForwardCurve.CalculateValueByTime( time );
				Vector3 up = data.UpCurve.CalculateValueByTime( time );

				forward.Normalize();
				var left = Vector3.Cross( up, forward );
				left.Normalize();
				up = Vector3.Cross( forward, left );
				up.Normalize();

				var mat = new Matrix3( forward, left, up );
				mat.ToQuaternion( out rot );
				rot.Normalize();
			}

			if( getScale )
				scl = data.ScaleCurve.CalculateValueByTime( time );

			return new Transform( pos, rot, scl );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow )
		{
			var point = CreateComponent<CurveInSpacePoint>();
			point.Name = point.BaseType.GetUserFriendlyNameForInstance();
			point.Transform = new Transform( Transform.Value.Position + new Vector3( 5, 0, 0 ), Quaternion.Identity );
		}

		public override void NewObjectSetDefaultConfigurationUpdate()
		{
			var point = GetComponent<CurveInSpacePoint>();
			if( point != null )
				point.Transform = new Transform( Transform.Value.Position + new Vector3( 5, 0, 0 ), Quaternion.Identity );
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "CurveInSpace" );
		}

		public virtual void DataWasChanged()
		{
			needUpdateData = true;
		}

		protected override void OnComponentAdded( Component component )
		{
			base.OnComponentAdded( component );

			if( component is CurveInSpacePoint )
				DataWasChanged();
		}

		protected override void OnComponentRemoved( Component component )
		{
			base.OnComponentRemoved( component );

			if( component is CurveInSpacePoint )
				DataWasChanged();
		}

		void GenerateMeshData( Mesh mesh, CollisionShape_Mesh collisionShape )
		{
			var data = GetData();
			var ownerPosition = TransformV.Position;
			var uvTilesLength = UVTilesLength.Value;
			var uvTilesCircle = UVTilesCircle.Value;
			var uvFlip = UVFlip.Value;
			var outsideDiameter = Thickness.Value;
			var points = data.SourceData.Points;
			var segmentsLength = SegmentsLength.Value;
			if( segmentsLength <= 0.001 )
				segmentsLength = 0.001;
			var segmentsCircle = SegmentsCircle.Value;
			if( segmentsCircle <= 3 )
				segmentsCircle = 3;
			var caps = Caps.Value;

			var circleSteps = segmentsCircle + 1;

			double totalLengthNoCurvature = 0;
			{
				for( int n = 1; n < points.Length; n++ )
					totalLengthNoCurvature += ( points[ n ].Transform.Position - points[ n - 1 ].Transform.Position ).Length();
				if( totalLengthNoCurvature <= 0.001 )
					totalLengthNoCurvature = 0.001;
			}

			//generate time steps
			List<double> timeSteps;
			{
				var stepDivide = 2.0;

				timeSteps = new List<double>( (int)( totalLengthNoCurvature / segmentsLength * stepDivide * 2 ) );

				for( int nPoint = 0; nPoint < points.Length - 1; nPoint++ )
				{
					var pointFrom = points[ nPoint ];
					var pointTo = points[ nPoint + 1 ];
					var timeFrom = (double)nPoint;
					var timeTo = (double)nPoint + 1;

					var length = ( pointTo.Transform.Position - pointFrom.Transform.Position ).Length();
					var timeStep = segmentsLength / length / stepDivide;

					var timeEnd = timeTo - timeStep * 0.1;
					for( double time = timeFrom; time < timeEnd; time += timeStep )
						timeSteps.Add( time );
				}
				timeSteps.Add( points.Length - 1 );
			}

			//detect skipped steps
			var skipSteps = new bool[ timeSteps.Count ];
			{
				double currentLength = 0;
				var previousPosition = data.GetPositionByNormalizedTime( 0 );
				var previousVector = ( data.GetPositionByNormalizedTime( 0.01 ) - previousPosition ).GetNormalize();
				var currentUp = points[ 0 ].Transform.Rotation.GetUp();
				if( currentUp.Equals( previousVector, 0.01 ) )
					currentUp = points[ 0 ].Transform.Rotation.GetLeft();
				Vector3 previousUsedScale = points[ 0 ].Transform.Scale;

				double lengthRemainder = 0;
				var previousUsedVector = previousVector;


				for( int nTime = 0; nTime < timeSteps.Count; nTime++ )
				{
					var time = timeSteps[ nTime ];
					var firstStep = nTime == 0;
					var lastStep = nTime == timeSteps.Count - 1;

					var position = data.GetPositionByNormalizedTime( time );

					var vector = position - previousPosition;
					double stepLength = 0;
					if( vector != Vector3.Zero )
						stepLength = vector.Normalize();
					else
						vector = previousVector;

					var rotation = Quaternion.LookAt( vector, currentUp );

					var scale = data.GetScaleByNormalizedTime( time );


					var skip = false;
					if( !firstStep && !lastStep )
					{
						if( lengthRemainder + stepLength < segmentsLength )
							skip = true;
						else
						{
							//!!!!нужно проверять scale vector, т.е. искривление поверхности. не сам scale

							//threshold 0.25, 0.01
							if( MathAlgorithms.GetVectorsAngle( vector, previousUsedVector ).InDegrees() < 0.25 && scale.Equals( previousUsedScale, 0.01 ) )
								skip = true;
							else
							{
								//don't skip previous step
								skipSteps[ nTime - 1 ] = false;
							}
						}
					}

					if( !skip )
						lengthRemainder = 0;
					else
						lengthRemainder += stepLength;

					currentLength += stepLength;
					previousPosition = position;
					previousVector = vector;
					if( !skip )
						previousUsedVector = vector;
					currentUp = rotation.GetUp();
					if( !skip )
						previousUsedScale = scale;

					skipSteps[ nTime ] = skip;
				}
			}

			//start calculation
			var actualLengthSteps = skipSteps.Count( v => false );
			var vertexCount = actualLengthSteps * circleSteps;
			if( caps )
				vertexCount += circleSteps * 2;

			var positions = new List<Vector3F>( vertexCount );
			var normals = new List<Vector3F>( vertexCount );
			var tangents = new List<Vector4F>( vertexCount );
			var texCoords = new List<Vector2F>( vertexCount );


			//calculate pipe vertices

			int lengthStepsAdded = 0;
			int firstStepVertexStart = 0;
			Quaternion firstStepRotation = Quaternion.Identity;
			int lastStepVertexStart = 0;
			Quaternion lastStepRotation = Quaternion.Identity;

			{
				double currentLength = 0;
				var previousPosition = data.GetPositionByNormalizedTime( 0 );
				var previousVector = ( data.GetPositionByNormalizedTime( 0.01 ) - previousPosition ).GetNormalize();
				var currentUp = points[ 0 ].Transform.Rotation.GetUp();
				if( currentUp.Equals( previousVector, 0.01 ) )
					currentUp = points[ 0 ].Transform.Rotation.GetLeft();
				//Vector3 previousUsedScale = points[ 0 ].Transform.Scale;

				//double lengthRemainder = 0;
				//var previousUsedVector = previousVector;

				for( int nTime = 0; nTime < timeSteps.Count; nTime++ )
				{
					var time = timeSteps[ nTime ];
					var firstStep = nTime == 0;
					var lastStep = nTime == timeSteps.Count - 1;

					var position = data.GetPositionByNormalizedTime( time );

					var vector = position - previousPosition;
					double stepLength = 0;
					if( vector != Vector3.Zero )
						stepLength = vector.Normalize();
					else
						vector = previousVector;

					currentLength += stepLength;

					if( currentUp.Equals( vector, 0.01 ) )
						currentUp = Quaternion.FromDirectionZAxisUp( vector ).GetUp();
					var rotation = Quaternion.LookAt( vector, currentUp );

					var scale = data.GetScaleByNormalizedTime( time );

					var skip = skipSteps[ nTime ];

					if( !skip )
					{
						if( firstStep )
						{
							firstStepVertexStart = positions.Count;
							firstStepRotation = rotation;
						}
						if( lastStep )
						{
							lastStepVertexStart = positions.Count;
							lastStepRotation = rotation;
						}

						for( int circleStep = 0; circleStep < circleSteps; circleStep++ )
						{
							var circleFactor = (double)circleStep / (double)( circleSteps - 1 );

							var p = new Vector2(
								Math.Cos( Math.PI * 2 * circleFactor ),
								Math.Sin( Math.PI * 2 * circleFactor ) );

							var offsetNotScaled = rotation * new Vector3( 0, p.X, p.Y );
							var offset = rotation * new Vector3( 0, p.X * scale.X * outsideDiameter * 0.5, p.Y * scale.Y * outsideDiameter * 0.5 );

							var pos = position - ownerPosition + offset;
							positions.Add( pos.ToVector3F() );

							var normal = offsetNotScaled.GetNormalize();
							var tangent = rotation.GetForward().Cross( normal );
							normals.Add( normal.ToVector3F() );
							tangents.Add( new Vector4F( tangent.ToVector3F(), -1 ) );

							var uv = new Vector2( currentLength * uvTilesLength, circleFactor * uvTilesCircle );
							if( uvFlip )
								uv = new Vector2( uv.Y, uv.X );
							texCoords.Add( uv.ToVector2F() );
						}

						lengthStepsAdded++;
					}


					//if( !skip )
					//	lengthRemainder = 0;
					//else
					//	lengthRemainder += stepLength;

					//currentLength += stepLength;
					previousPosition = position;
					previousVector = vector;
					//if( !skip )
					//	previousUsedVector = vector;
					currentUp = rotation.GetUp();
					//if( !skip )
					//	previousUsedScale = scale;
				}
			}

			//calculate caps vertices
			int capsStartVerticesStartIndex = 0;
			int capsEndVerticesStartIndex = 0;
			if( caps )
			{
				for( int nSide = 0; nSide < 2; nSide++ )
				{
					if( nSide == 0 )
						capsStartVerticesStartIndex = positions.Count;
					else
						capsEndVerticesStartIndex = positions.Count;

					int vertexStart;
					Quaternion rotation;
					Vector3 normal;
					Vector3 tangent;
					if( nSide == 0 )
					{
						vertexStart = firstStepVertexStart;
						rotation = firstStepRotation;
						normal = -firstStepRotation.GetForward();
						tangent = firstStepRotation.GetUp();
					}
					else
					{
						vertexStart = lastStepVertexStart;
						rotation = lastStepRotation;
						normal = lastStepRotation.GetForward();
						tangent = -lastStepRotation.GetUp();
					}

					var rotationInv = rotation.GetInverse();

					for( int circleStep = 0; circleStep < circleSteps; circleStep++ )
					{
						var pos = positions[ vertexStart + circleStep ];

						positions.Add( pos );
						normals.Add( normal.ToVector3F() );
						tangents.Add( new Vector4F( tangent.ToVector3F(), -1 ) );

						var posRotated = rotationInv * pos;
						var uv = new Vector2( posRotated.Y * uvTilesLength, posRotated.Z * uvTilesLength );
						texCoords.Add( uv.ToVector2F() );
					}
				}
			}

			//indices
			var lengthSegmentsAdded = lengthStepsAdded - 1;
			var indexCount = lengthSegmentsAdded * segmentsCircle * 6;
			if( caps )
				indexCount += ( segmentsCircle - 2 ) * 3 * 2;

			var indices = new int[ indexCount ];
			int currentIndex = 0;

			//calculate pipe indices
			for( int lengthStep = 0; lengthStep < lengthSegmentsAdded; lengthStep++ )
			{
				for( int circleStep = 0; circleStep < segmentsCircle; circleStep++ )
				{
					indices[ currentIndex++ ] = ( segmentsCircle + 1 ) * lengthStep + circleStep;
					indices[ currentIndex++ ] = ( segmentsCircle + 1 ) * lengthStep + circleStep + 1;
					indices[ currentIndex++ ] = ( segmentsCircle + 1 ) * ( lengthStep + 1 ) + circleStep + 1;
					indices[ currentIndex++ ] = ( segmentsCircle + 1 ) * ( lengthStep + 1 ) + circleStep + 1;
					indices[ currentIndex++ ] = ( segmentsCircle + 1 ) * ( lengthStep + 1 ) + circleStep;
					indices[ currentIndex++ ] = ( segmentsCircle + 1 ) * lengthStep + circleStep;
				}
			}

			//calculate caps indices
			if( caps )
			{
				for( int n = 1; n < segmentsCircle - 1; n++ )
				{
					indices[ currentIndex++ ] = capsStartVerticesStartIndex;
					indices[ currentIndex++ ] = capsStartVerticesStartIndex + n + 1;
					indices[ currentIndex++ ] = capsStartVerticesStartIndex + n;
				}
				for( int n = 1; n < segmentsCircle - 1; n++ )
				{
					indices[ currentIndex++ ] = capsEndVerticesStartIndex;
					indices[ currentIndex++ ] = capsEndVerticesStartIndex + n;
					indices[ currentIndex++ ] = capsEndVerticesStartIndex + n + 1;
				}
			}

			if( currentIndex != indexCount )
				Log.Fatal( "CurveInSpace: GenerateMeshData: currentIndex != indexCount." );
			foreach( var index in indices )
				if( index < 0 || index >= positions.Count )
					Log.Fatal( "CurveInSpace: GenerateMeshData: index < 0 || index >= positions.Count." );

			//init objects
			if( collisionShape != null )
			{
				collisionShape.Vertices = positions.ToArray();
				collisionShape.Indices = indices;
			}
			else
			{
				var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

				var vertices = new byte[ vertexSize * positions.Count ];
				unsafe
				{
					fixed( byte* pVertices = vertices )
					{
						StandardVertex.StaticOneTexCoord* pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;

						for( int n = 0; n < positions.Count; n++ )
						{
							pVertex->Position = positions[ n ];
							pVertex->Normal = normals[ n ];
							pVertex->Tangent = tangents[ n ];
							pVertex->Color = new ColorValue( 1, 1, 1, 1 );
							pVertex->TexCoord0 = texCoords[ n ];

							pVertex++;
						}
					}
				}

				//create mesh geometry
				if( vertexStructure.Length != 0 && vertices.Length != 0 && indices.Length != 0 )
				{
					var meshGeometry = mesh.CreateComponent<MeshGeometry>();
					meshGeometry.Name = "Mesh Geometry";
					meshGeometry.VertexStructure = vertexStructure;
					meshGeometry.Vertices = vertices;
					meshGeometry.Indices = indices;
					meshGeometry.Material = Material.Value;
				}
			}
		}

		void UpdateRenderingObjects()
		{
			DeleteRenderingObjects();

			bool display = false;
			if( EngineApp.IsEditor && DisplayCurveInEditor )
				display = true;
			else if( EngineApp.IsSimulation && DisplayCurveInSimulation )
				display = true;
			if( !display )
				return;

			var data = GetData();
			if( data != null && data.SourceData != null && data.SourceData.Points.Length > 1 )
			{
				//create mesh
				{
					mesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );
					GenerateMeshData( mesh, null );
					mesh.VisibilityDistanceFactor = VisibilityDistanceFactor;
					mesh.Enabled = true;
				}

				//create mesh in space
				{
					//need set ShowInEditor = false before AddComponent
					meshInSpace = ComponentUtility.CreateComponent<MeshInSpace>( null, false, false );
					meshInSpace.NetworkMode = NetworkModeEnum.False;
					meshInSpace.DisplayInEditor = false;
					AddComponent( meshInSpace, -1 );
					//meshInSpace = CreateComponent<MeshInSpace>();

					//meshInSpace.Name = "__Mesh In Space";
					meshInSpace.CanBeSelected = false;
					meshInSpace.SaveSupport = false;
					meshInSpace.CloneSupport = false;
					meshInSpace.Transform = new Transform( TransformV.Position );
					meshInSpace.Color = Color;
					meshInSpace.Mesh = mesh;
					meshInSpace.StaticShadows = true;
					meshInSpace.Enabled = true;
				}
			}
		}

		void DeleteRenderingObjects()
		{
			meshInSpace?.Dispose();
			meshInSpace = null;

			mesh?.Dispose();
			mesh = null;
		}

		void UpdateCollisionObjects()
		{
			DeleteCollisionObjects();

			if( !Collision )
				return;

			//!!!!use BodyItem

			//need set DisplayInEditor = false before AddComponent
			var body = ComponentUtility.CreateComponent<RigidBody>( null, false, false );
			body.NetworkMode = NetworkModeEnum.False;
			body.DisplayInEditor = false;
			AddComponent( body );
			//var group = scene.CreateComponent<GroupOfObjects>();

			//CollisionBody.Name = "__Collision Body";
			body.SaveSupport = false;
			body.CloneSupport = false;
			body.CanBeSelected = false;
			body.Transform = new Transform( TransformV.Position );

			var shape = body.CreateComponent<CollisionShape_Mesh>();
			shape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.TriangleMesh;
			shape.CheckValidData = false;
			shape.MergeEqualVerticesRemoveInvalidTriangles = false;

			GenerateMeshData( null, shape );

			UpdateCollisionMaterial();

			body.Enabled = true;

			collisionBody = body;
		}

		void UpdateCollisionMaterial()
		{
			if( collisionBody != null )
			{
				collisionBody.Material = CollisionMaterial;
				//!!!!
				//collisionBody.MaterialFrictionMode = CollisionFrictionMode;
				collisionBody.MaterialFriction = CollisionFriction;
				//!!!!
				//collisionBody.MaterialAnisotropicFriction = CollisionAnisotropicFriction;
				//collisionBody.MaterialSpinningFriction = CollisionSpinningFriction;
				//collisionBody.MaterialRollingFriction = CollisionRollingFriction;
				collisionBody.MaterialRestitution = CollisionRestitution;
			}
		}

		void DeleteCollisionObjects()
		{
			collisionBody?.Dispose();
			collisionBody = null;
		}
	}
}
