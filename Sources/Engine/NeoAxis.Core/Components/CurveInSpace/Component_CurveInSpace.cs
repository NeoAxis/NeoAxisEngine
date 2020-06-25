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
	public class Component_CurveInSpace : Component_ObjectInSpace
	{
		CachedData cachedData;

		/////////////////////////////////////////

		/// <summary>
		/// The type of the curve for position of the points.
		/// </summary>
		[DefaultValue( CurveTypeEnum.CubicSpline )]
		[Serialize]
		public Reference<CurveTypeEnum> CurveTypePosition
		{
			get { if( _curveTypePosition.BeginGet() ) CurveTypePosition = _curveTypePosition.Get( this ); return _curveTypePosition.value; }
			set { if( _curveTypePosition.BeginSet( ref value ) ) { try { CurveTypePositionChanged?.Invoke( this ); } finally { _curveTypePosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurveTypePosition"/> property value changes.</summary>
		public event Action<Component_CurveInSpace> CurveTypePositionChanged;
		ReferenceField<CurveTypeEnum> _curveTypePosition = CurveTypeEnum.CubicSpline;

		/// <summary>
		/// The type of the curve for rotation of the points.
		/// </summary>
		[DefaultValue( CurveTypeEnum.CubicSpline )]
		[Serialize]
		public Reference<CurveTypeEnum> CurveTypeRotation
		{
			get { if( _curveTypeRotation.BeginGet() ) CurveTypeRotation = _curveTypeRotation.Get( this ); return _curveTypeRotation.value; }
			set { if( _curveTypeRotation.BeginSet( ref value ) ) { try { CurveTypeRotationChanged?.Invoke( this ); } finally { _curveTypeRotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurveTypeRotation"/> property value changes.</summary>
		public event Action<Component_CurveInSpace> CurveTypeRotationChanged;
		ReferenceField<CurveTypeEnum> _curveTypeRotation = CurveTypeEnum.CubicSpline;

		/// <summary>
		/// The type of the curve for scale of the points.
		/// </summary>
		[DefaultValue( CurveTypeEnum.CubicSpline )]
		[Serialize]
		public Reference<CurveTypeEnum> CurveTypeScale
		{
			get { if( _curveTypeScale.BeginGet() ) CurveTypeScale = _curveTypeScale.Get( this ); return _curveTypeScale.value; }
			set { if( _curveTypeScale.BeginSet( ref value ) ) { try { CurveTypeScaleChanged?.Invoke( this ); } finally { _curveTypeScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurveTypeScale"/> property value changes.</summary>
		public event Action<Component_CurveInSpace> CurveTypeScaleChanged;
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
			set { if( _timeScale.BeginSet( ref value ) ) { try { TimeScaleChanged?.Invoke( this ); } finally { _timeScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TimeScale"/> property value changes.</summary>
		public event Action<Component_CurveInSpace> TimeScaleChanged;
		ReferenceField<double> _timeScale = 1.0;

		//[DefaultValue( true )]
		//[Serialize]
		//public Reference<bool> AlwaysDisplayInEditor
		//{
		//	get { if( _alwaysDisplayInEditor.BeginGet() ) AlwaysDisplayInEditor = _alwaysDisplayInEditor.Get( this ); return _alwaysDisplayInEditor.value; }
		//	set { if( _alwaysDisplayInEditor.BeginSet( ref value ) ) { try { AlwaysDisplayInEditorChanged?.Invoke( this ); } finally { _alwaysDisplayInEditor.EndSet(); } } }
		//}
		//public event Action<Component_CurveInSpace> AlwaysDisplayInEditorChanged;
		//ReferenceField<bool> _alwaysDisplayInEditor = true;

		//[DefaultValue( false )]
		//[Serialize]
		//public Reference<bool> AlwaysDisplayInSimulation
		//{
		//	get { if( _alwaysDisplayInSimulation.BeginGet() ) AlwaysDisplayInSimulation = _alwaysDisplayInSimulation.Get( this ); return _alwaysDisplayInSimulation.value; }
		//	set { if( _alwaysDisplayInSimulation.BeginSet( ref value ) ) { try { AlwaysDisplayInSimulationChanged?.Invoke( this ); } finally { _alwaysDisplayInSimulation.EndSet(); } } }
		//}
		//public event Action<Component_CurveInSpace> AlwaysDisplayInSimulationChanged;
		//ReferenceField<bool> _alwaysDisplayInSimulation = false;

		/// <summary>
		/// Specifies the color of the curve.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Serialize]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Component_CurveInSpace> ColorChanged;
		ReferenceField<ColorValue> _color = new ColorValue( 1, 1, 1 );

		[DefaultValue( 0.0 )]
		[Range( 0, 1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> Thickness
		{
			get { if( _thickness.BeginGet() ) Thickness = _thickness.Get( this ); return _thickness.value; }
			set { if( _thickness.BeginSet( ref value ) ) { try { ThicknessChanged?.Invoke( this ); } finally { _thickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Thickness"/> property value changes.</summary>
		public event Action<Component_CurveInSpace> ThicknessChanged;
		ReferenceField<double> _thickness = 0.0;

		/// <summary>
		/// Whether to display the curve in the editor.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		public Reference<bool> DisplayCurveInEditor
		{
			get { if( _displayCurveInEditor.BeginGet() ) DisplayCurveInEditor = _displayCurveInEditor.Get( this ); return _displayCurveInEditor.value; }
			set { if( _displayCurveInEditor.BeginSet( ref value ) ) { try { DisplayCurveInEditorChanged?.Invoke( this ); } finally { _displayCurveInEditor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayCurveInEditor"/> property value changes.</summary>
		public event Action<Component_CurveInSpace> DisplayCurveInEditorChanged;
		ReferenceField<bool> _displayCurveInEditor = true;

		/// <summary>
		/// Whether to display the curve in the simulation.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		public Reference<bool> DisplayCurveInSimulation
		{
			get { if( _displayCurveInSimulation.BeginGet() ) DisplayCurveInSimulation = _displayCurveInSimulation.Get( this ); return _displayCurveInSimulation.value; }
			set { if( _displayCurveInSimulation.BeginSet( ref value ) ) { try { DisplayCurveInSimulationChanged?.Invoke( this ); } finally { _displayCurveInSimulation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayCurveInSimulation"/> property value changes.</summary>
		public event Action<Component_CurveInSpace> DisplayCurveInSimulationChanged;
		ReferenceField<bool> _displayCurveInSimulation = false;

		/// <summary>
		/// The time of the point.
		/// </summary>
		[Category( "Point" )]
		[DefaultValue( 0.0 )]
		[Serialize]
		public Reference<double> Time
		{
			get { if( _time.BeginGet() ) Time = _time.Get( this ); return _time.value; }
			set { if( _time.BeginSet( ref value ) ) { try { TimeChanged?.Invoke( this ); } finally { _time.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Time"/> property value changes.</summary>
		public event Action<Component_CurveInSpace> TimeChanged;
		ReferenceField<double> _time = 0.0;

		/////////////////////////////////////////

		/// <summary>
		/// Enumerates the types of curve.
		/// </summary>
		public enum CurveTypeEnum
		{
			CubicSpline,
			Bezier,
			Line,
			//RoundedLine,
		}

		/////////////////////////////////////////

		/// <summary>
		/// Extracted source data of the curve.
		/// </summary>
		public class SourceData
		{
			public CurveTypeEnum curveTypePosition;
			public CurveTypeEnum curveTypeRotation;
			public CurveTypeEnum curveTypeScale;
			public double timeScale;
			/// <summary>
			/// Represents a point of <see cref="SourceData"/>.
			/// </summary>
			public struct Point
			{
				public Transform transform;
				public double time;
			}
			public Point[] points;

			public bool Equals( SourceData data )
			{
				if( curveTypePosition != data.curveTypePosition )
					return false;
				if( curveTypeRotation != data.curveTypeRotation )
					return false;
				if( curveTypeScale != data.curveTypeScale )
					return false;
				if( timeScale != data.timeScale )
					return false;

				if( points.Length != data.points.Length )
					return false;
				for( int n = 0; n < points.Length; n++ )
				{
					var p = points[ n ];
					var p2 = data.points[ n ];
					if( p.transform != p2.transform || p.time != p2.time )
						return false;
				}

				return true;
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// Precalculated data of the curve.
		/// </summary>
		public class CachedData
		{
			public SourceData sourceData;

			public Curve positionCurve;
			public Curve forwardCurve;
			public Curve upCurve;
			public Curve scaleCurve;
			public Range timeRange;
		}

		/////////////////////////////////////////

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			base.OnGetRenderSceneData( context, mode );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;

				if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() )
					context2.disableShowingLabelForThisObject = true;
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = FindParent<Component_Scene>();
			if( scene != null )
			{
				if( EnabledInHierarchy )
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
				else
					scene.GetRenderSceneData -= Scene_GetRenderSceneData;
			}
		}

		private void Scene_GetRenderSceneData( Component_Scene scene, ViewportRenderingContext context )
		{
			var context2 = context.objectInSpaceRenderingContext;

			bool display = false;
			ColorValue color = Color;

			if( VisibleInHierarchy )
			{
				if( ( ParentScene.GetDisplayDevelopmentDataInThisApplication() /*&& ParentScene.DisplayLights */) ||
					context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this )
				{
					if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
					{
						display = true;
						if( context2.selectedObjects.Contains( this ) )
							color = ProjectSettings.Get.SelectedColor;
						else
							color = ProjectSettings.Get.CanSelectColor;
					}

					if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor && DisplayCurveInEditor )
						display = true;
				}

				if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation && DisplayCurveInSimulation )
					display = true;
			}

			if( display )
				Render( context2, color );

			if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() )
				context2.disableShowingLabelForThisObject = true;
		}

		public void Render( RenderingContext context, ColorValue color )
		{
			var data = GetData();
			if( data != null && data.sourceData.points.Length > 1 )
			{
				context.viewport.Simple3DRenderer.SetColor( color );

				//!!!!check visible in frustum

				//good?
				var step = data.timeRange.Size / 20.0 / data.sourceData.points.Length;

				var thickness = Thickness.Value;

				Vector3? lastPos = null;
				for( var t = data.timeRange.Minimum; t < data.timeRange.Maximum; t += step )
				{
					var pos = data.positionCurve.CalculateValueByTime( t );
					if( lastPos.HasValue )
						context.viewport.Simple3DRenderer.AddLine( lastPos.Value, pos, thickness );
					lastPos = pos;
				}
				if( lastPos.HasValue )
					context.viewport.Simple3DRenderer.AddLine( lastPos.Value, data.positionCurve.CalculateValueByTime( data.timeRange.Maximum ), thickness );
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
				var diff = data.timeRange.Size;
				if( diff != 0 )
				{
					var sceneTime = ParentScene != null ? ParentScene.Time : 0;
					return GetTransformByTimeImpl( data, data.timeRange.Minimum + sceneTime % diff, true, true, true );
				}
			}
			return Transform;
		}

		public Vector3 GetPositionBySceneTimeLooped()
		{
			var data = GetData();
			if( data != null )
			{
				var diff = data.timeRange.Size;
				if( diff != 0 )
				{
					var sceneTime = ParentScene != null ? ParentScene.Time : 0;
					return GetTransformByTimeImpl( data, data.timeRange.Minimum + sceneTime % diff, true, false, false ).Position;
				}
			}
			return Transform.Value.Position;
		}

		public Range GetTimeRange()
		{
			var data = GetData();
			return data != null ? data.timeRange : Range.Zero;
		}

		Curve CreateCurve( CurveTypeEnum type )
		{
			switch( type )
			{
			case CurveTypeEnum.CubicSpline: return new CurveCubicSpline();
			case CurveTypeEnum.Bezier: return new CurveBezier();
			case CurveTypeEnum.Line: return new CurveLine();
			}

			Log.Fatal( "Component_CurveInSpace: CreateCurve: no implementation." );
			return null;
		}

		SourceData GetSourceData()
		{
			var sourceData = new SourceData();
			sourceData.curveTypePosition = CurveTypePosition;
			sourceData.curveTypeRotation = CurveTypeRotation;
			sourceData.curveTypeScale = CurveTypeScale;
			sourceData.timeScale = TimeScale;

			var points = GetComponents<Component_CurveInSpacePoint>( onlyEnabledInHierarchy: true );

			sourceData.points = new SourceData.Point[ 1 + points.Length ];
			sourceData.points[ 0 ] = new SourceData.Point() { transform = Transform, time = Time };
			for( int n = 0; n < points.Length; n++ )
				sourceData.points[ n + 1 ] = new SourceData.Point() { transform = points[ n ].Transform, time = points[ n ].Time };

			//sort by time
			bool allPointsZeroTime = sourceData.points.All( p => p.time == 0 );
			if( !allPointsZeroTime )
			{
				CollectionUtility.MergeSort( sourceData.points, delegate ( SourceData.Point a, SourceData.Point b )
				{
					if( a.time < b.time )
						return -1;
					if( a.time > b.time )
						return 1;
					return 0;
				} );
			}

			return sourceData;
		}

		public CachedData GetData()
		{
			var data = cachedData;

			//!!!!slowly. int version для каждой точки

			var sourceData = GetSourceData();
			bool update = data == null || !data.sourceData.Equals( sourceData );

			if( update )
			{
				if( sourceData.points.Length != 0 )
				{
					data = new CachedData();
					data.sourceData = sourceData;
					data.positionCurve = CreateCurve( CurveTypePosition );
					data.forwardCurve = CreateCurve( CurveTypeRotation );
					data.upCurve = CreateCurve( CurveTypeRotation );
					data.scaleCurve = CreateCurve( CurveTypeScale );

					bool allPointsZeroTime = sourceData.points.All( p => p.time == 0 );

					for( int n = 0; n < sourceData.points.Length; n++ )
					{
						var point = sourceData.points[ n ];

						double time = ( allPointsZeroTime ? n : point.time ) * sourceData.timeScale;

						var transform = point.transform;

						data.positionCurve.AddPoint( time, transform.Position );

						transform.Rotation.GetForward( out var forward );
						transform.Rotation.GetUp( out var up );
						data.forwardCurve.AddPoint( time, forward );
						data.upCurve.AddPoint( time, up );

						data.scaleCurve.AddPoint( time, transform.Scale );
					}

					data.timeRange = new Range( data.positionCurve.Points[ 0 ].Time, data.positionCurve.Points[ sourceData.points.Length - 1 ].Time );
				}

				cachedData = data;
			}

			return data;
		}

		static Transform GetTransformByTimeImpl( CachedData data, double time, bool getPosition, bool getRotation, bool getScale )
		{
			var pos = Vector3.Zero;
			var rot = Quaternion.Identity;
			var scl = Vector3.One;

			if( getPosition )
				pos = data.positionCurve.CalculateValueByTime( time );

			if( getRotation )
			{
				Vector3 forward = data.forwardCurve.CalculateValueByTime( time );
				Vector3 up = data.upCurve.CalculateValueByTime( time );

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
				scl = data.scaleCurve.CalculateValueByTime( time );

			return new Transform( pos, rot, scl );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow )
		{
			var point = CreateComponent<Component_CurveInSpacePoint>();
			point.Name = point.BaseType.GetUserFriendlyNameForInstance();
			point.Transform = new Transform( Transform.Value.Position + new Vector3( 1, 0, 0 ), Quaternion.Identity );
		}

		public override void NewObjectSetDefaultConfigurationUpdate()
		{
			var point = GetComponent<Component_CurveInSpacePoint>();
			if( point != null )
				point.Transform = new Transform( Transform.Value.Position + new Vector3( 1, 0, 0 ), Quaternion.Identity );
		}
	}
}
