// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;
using System.Runtime.CompilerServices;


//!!!!
//exits from the road
//fence to roads
//railroads
//lods, sectors
//parking lots
//affect terrain
//railroad intersection with road
//per lane settings. traffik direction
//traffik
//good lane shader
//road ends points
//stone road, footpath examples
//https://github.com/MicroGSD/RoadArchitect
//Bezier by default?
//aliasing in markup when looking across
//intervals, modifiers on curve, on lane
//each connector for each lane?

//don't create walking markings when no crossing


namespace NeoAxis
{
	/// <summary>
	/// Represents a road in the scene.
	/// </summary>
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Road\Road", 10510 )]
	[SettingsCell( typeof( RoadSettingsCell ) )]
#endif
	public class Road : CurveInSpace
	{
		bool sceneIsReady;

		RoadData roadData;
		bool needUpdateRoadData;

		LogicalData logicalData;
		//bool logicalDataUpdating;
		RoadUtility.VisualData visualData;
		bool needUpdateLogicalData;
		bool needUpdateLogicalDataAfterEndModifyingTransformTool;
		bool needUpdateVisualData;

		//

		const string roadTypeDefault = @"Content\Constructors\Roads\Default Road\Default Road.roadtype";

		/// <summary>
		/// The type of the road.
		/// </summary>
		[DefaultValueReference( roadTypeDefault )]
		//[Category( "General" )]
		public Reference<RoadType> RoadType
		{
			get { if( _roadType.BeginGet() ) RoadType = _roadType.Get( this ); return _roadType.value; }
			set { if( _roadType.BeginSet( this, ref value ) ) { try { RoadTypeChanged?.Invoke( this ); DataWasChanged(); } finally { _roadType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RoadType"/> property value changes.</summary>
		public event Action<Road> RoadTypeChanged;
		ReferenceField<RoadType> _roadType = new Reference<RoadType>( null, roadTypeDefault );

		/// <summary>
		/// The amount of lanes.
		/// </summary>
		[DefaultValue( 2 )]
		[Range( 1, 20 )]
		//[Category( "General" )]
		public Reference<int> Lanes
		{
			get { if( _lanes.BeginGet() ) Lanes = _lanes.Get( this ); return _lanes.value; }
			set { if( _lanes.BeginSet( this, ref value ) ) { try { LanesChanged?.Invoke( this ); DataWasChanged(); } finally { _lanes.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Lanes"/> property value changes.</summary>
		public event Action<Road> LanesChanged;
		ReferenceField<int> _lanes = 2;

		/// <summary>
		/// Whether to have a collision body.
		/// </summary>
		[DefaultValue( true )]
		//[Category( "Physics" )]
		public Reference<bool> RoadCollision
		{
			get { if( _roadCollision.BeginGet() ) RoadCollision = _roadCollision.Get( this ); return _roadCollision.value; }
			set { if( _roadCollision.BeginSet( this, ref value ) ) { try { RoadCollisionChanged?.Invoke( this ); DataWasChanged(); } finally { _roadCollision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RoadCollision"/> property value changes.</summary>
		public event Action<Road> RoadCollisionChanged;
		ReferenceField<bool> _roadCollision = true;

		//!!!!при обновлении может только удалять visual data
		/// <summary>
		/// The base color and opacity multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Display" )]
		public Reference<ColorValue> ColorMultiplier
		{
			get { if( _colorMultiplier.BeginGet() ) ColorMultiplier = _colorMultiplier.Get( this ); return _colorMultiplier.value; }
			set { if( _colorMultiplier.BeginSet( this, ref value ) ) { try { ColorMultiplierChanged?.Invoke( this ); DataWasChanged(); } finally { _colorMultiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ColorMultiplier"/> property value changes.</summary>
		public event Action<Road> ColorMultiplierChanged;
		ReferenceField<ColorValue> _colorMultiplier = ColorValue.One;

		/// <summary>
		/// Whether to display roadway border in the editor.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Display" )]
		public Reference<bool> DisplayRoadwayBorderInEditor
		{
			get { if( _displayRoadwayBorderInEditor.BeginGet() ) DisplayRoadwayBorderInEditor = _displayRoadwayBorderInEditor.Get( this ); return _displayRoadwayBorderInEditor.value; }
			set { if( _displayRoadwayBorderInEditor.BeginSet( this, ref value ) ) { try { DisplayRoadwayBorderInEditorChanged?.Invoke( this ); } finally { _displayRoadwayBorderInEditor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayRoadwayBorderInEditor"/> property value changes.</summary>
		public event Action<Road> DisplayRoadwayBorderInEditorChanged;
		ReferenceField<bool> _displayRoadwayBorderInEditor = false;

		/// <summary>
		/// Whether to display roadway border in the simulation.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Display" )]
		public Reference<bool> DisplayRoadwayBorderInSimulation
		{
			get { if( _displayRoadwayBorderInSimulation.BeginGet() ) DisplayRoadwayBorderInSimulation = _displayRoadwayBorderInSimulation.Get( this ); return _displayRoadwayBorderInSimulation.value; }
			set { if( _displayRoadwayBorderInSimulation.BeginSet( this, ref value ) ) { try { DisplayRoadwayBorderInSimulationChanged?.Invoke( this ); } finally { _displayRoadwayBorderInSimulation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayRoadwayBorderInSimulation"/> property value changes.</summary>
		public event Action<Road> DisplayRoadwayBorderInSimulationChanged;
		ReferenceField<bool> _displayRoadwayBorderInSimulation = false;

		/// <summary>
		/// Whether to display lane curves in the editor.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Display" )]
		public Reference<bool> DisplayLaneCurvesInEditor
		{
			get { if( _displayLaneCurvesInEditor.BeginGet() ) DisplayLaneCurvesInEditor = _displayLaneCurvesInEditor.Get( this ); return _displayLaneCurvesInEditor.value; }
			set { if( _displayLaneCurvesInEditor.BeginSet( this, ref value ) ) { try { DisplayLaneCurvesInEditorChanged?.Invoke( this ); } finally { _displayLaneCurvesInEditor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayLaneCurvesInEditor"/> property value changes.</summary>
		public event Action<Road> DisplayLaneCurvesInEditorChanged;
		ReferenceField<bool> _displayLaneCurvesInEditor = false;

		/// <summary>
		/// Whether to display lane curves in the simulation.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Display" )]
		public Reference<bool> DisplayLaneCurvesInSimulation
		{
			get { if( _displayLaneCurvesInSimulation.BeginGet() ) DisplayLaneCurvesInSimulation = _displayLaneCurvesInSimulation.Get( this ); return _displayLaneCurvesInSimulation.value; }
			set { if( _displayLaneCurvesInSimulation.BeginSet( this, ref value ) ) { try { DisplayLaneCurvesInSimulationChanged?.Invoke( this ); } finally { _displayLaneCurvesInSimulation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayLaneCurvesInSimulation"/> property value changes.</summary>
		public event Action<Road> DisplayLaneCurvesInSimulationChanged;
		ReferenceField<bool> _displayLaneCurvesInSimulation = false;

		/// <summary>
		/// The modifier of the first point of the road.
		/// </summary>
		[DefaultValue( RoadPoint.ModifiersEnum.None )]
		[Category( "Road Point" )]
		public Reference<RoadPoint.ModifiersEnum> PointModifiers
		{
			get { if( _pointModifiers.BeginGet() ) PointModifiers = _pointModifiers.Get( this ); return _pointModifiers.value; }
			set { if( _pointModifiers.BeginSet( this, ref value ) ) { try { PointModifiersChanged?.Invoke( this ); DataWasChanged(); } finally { _pointModifiers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PointModifiers"/> property value changes.</summary>
		public event Action<Road> PointModifiersChanged;
		ReferenceField<RoadPoint.ModifiersEnum> _pointModifiers = RoadPoint.ModifiersEnum.None;

		//!!!!
		//[DefaultValue( -1.0 )]
		//[Category( "Road Point" )]
		//public Reference<double> PointOverpassSupportHeight
		//{
		//	get { if( _pointOverpassSupportHeight.BeginGet() ) PointOverpassSupportHeight = _pointOverpassSupportHeight.Get( this ); return _pointOverpassSupportHeight.value; }
		//	set { if( _pointOverpassSupportHeight.BeginSet( this, ref value ) ) { try { PointOverpassSupportHeightChanged?.Invoke( this ); DataWasChanged(); } finally { _pointOverpassSupportHeight.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="PointOverpassSupportHeight"/> property value changes.</summary>
		//public event Action<Road> PointOverpassSupportHeightChanged;
		//ReferenceField<double> _pointOverpassSupportHeight = -1.0;

		///// <summary>
		///// Whether to add special mesh to the point.
		///// </summary>
		//[Category( "Road Point" )]
		//[DefaultValue( RoadPoint.SpecialtyEnum.None )]
		//public Reference<RoadPoint.SpecialtyEnum> Specialty
		//{
		//	get { if( _specialty.BeginGet() ) Specialty = _specialty.Get( this ); return _specialty.value; }
		//	set { if( _specialty.BeginSet( this, ref value ) ) { try { SpecialtyChanged?.Invoke( this ); DataWasChanged(); } finally { _specialty.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Specialty"/> property value changes.</summary>
		//public event Action<Road> SpecialtyChanged;
		//ReferenceField<RoadPoint.SpecialtyEnum> _specialty = RoadPoint.SpecialtyEnum.None;

		////!!!!нужна поддержка null значений
		//[Category( "Road Point" )]
		//[DefaultValue( -1.0 )]
		//public Reference<double> OverpassSupportHeight
		//{
		//	get { if( _overpassSupportHeight.BeginGet() ) OverpassSupportHeight = _overpassSupportHeight.Get( this ); return _overpassSupportHeight.value; }
		//	set { if( _overpassSupportHeight.BeginSet( this, ref value ) ) { try { OverpassSupportHeightChanged?.Invoke( this ); } finally { _overpassSupportHeight.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassSupportHeight"/> property value changes.</summary>
		//public event Action<Road> OverpassSupportHeightChanged;
		//ReferenceField<double> _overpassSupportHeight = -1.0;

		/// <summary>
		/// The age factor of the road.
		/// </summary>
		[DefaultValue( 0.5 )]//0.0 )]
		[Range( 0, 1 )]
		public Reference<double> Age
		{
			get { if( _age.BeginGet() ) Age = _age.Get( this ); return _age.value; }
			set { if( _age.BeginSet( this, ref value ) ) { try { AgeChanged?.Invoke( this ); NeedUpdateVisualData(); } finally { _age.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Age"/> property value changes.</summary>
		public event Action<Road> AgeChanged;
		ReferenceField<double> _age = 0.5;//0.0;


		[Serialize]
		[Browsable( false )]
		[DefaultValue( true )]
		public bool EditorAutoUpdate { get; set; } = true;

		///////////////////////////////////////////////

		public class RoadData
		{
			bool disposed;

			public Road Owner;
			public Scene Scene;

			public RoadType RoadType;
			public int RoadTypeVersion;

			public int Lanes;

			public Point[] Points;
			public Curve PositionCurve;
			//public Curve Forward;
			public Curve UpCurve;
			//public Curve ScaleCurve;
			//public double Length;
			//public Range TimeRange;

			public RoadModifier.PredefinedModifiersEnum PredefinedModifiers;
			//public double OverpassSupportHeight;

			CurveData[] borderCurves;
			CurveData[] laneCurves;

			Bounds? bounds;

			//!!!!lanes
			OctreeContainer closestCurveTimeToPositionOctree;
			Bounds[] closestCurveTimeToPositionOctreeBounds;
			RangeF[] closestCurveTimeToPositionOctreeTimeIntervals;

			/////////////////////

			public class Point
			{
				public Transform Transform;
				internal double _curveTimeOnlyToSort;

				public RoadPoint.ModifiersEnum Modifiers;
				//!!!!public double OverpassColumnHeight;
				public double RoundedLineCurvatureRadius;

				public double TimeOnCurve;
			}

			/////////////////////

			public class CurveData
			{
				public (double Time, Vector3 Position, Quaternion Rotation)[] Curve;
			}

			/////////////////////

			public RoadData( Road owner )
			{
				Owner = owner;
				Scene = Owner.ParentScene;
			}

			internal void Dispose()
			{
				closestCurveTimeToPositionOctree?.Dispose();
				closestCurveTimeToPositionOctree = null;
				disposed = true;
			}

			internal void GenerateCurveData()
			{
				//generate Points

				var pointComponents = Owner.GetComponents<RoadPoint>( onlyEnabledInHierarchy: true );

				Points = new Point[ 1 + pointComponents.Length ];
				Points[ 0 ] = new Point() { Transform = Owner.Transform, _curveTimeOnlyToSort = Owner.Time, Modifiers = Owner.PointModifiers/*, OverpassColumnHeight = Owner.PointOverpassSupportHeight*/, RoundedLineCurvatureRadius = Owner.RoundedLineCurvatureRadius };
				for( int n = 0; n < pointComponents.Length; n++ )
				{
					var pointComponent = pointComponents[ n ];
					Points[ n + 1 ] = new Point() { Transform = pointComponent.Transform, _curveTimeOnlyToSort = pointComponent.Time, Modifiers = pointComponent.Modifiers/*, OverpassColumnHeight = pointComponent.OverpassSupportHeight*/, RoundedLineCurvatureRadius = pointComponent.RoundedLineCurvatureRadius };
				}

				//sort by time
				bool allPointsZeroTime = Points.All( p => p._curveTimeOnlyToSort == 0 );
				if( !allPointsZeroTime )
				{
					CollectionUtility.MergeSort( Points, delegate ( Point a, Point b )
					{
						if( a._curveTimeOnlyToSort < b._curveTimeOnlyToSort )
							return -1;
						if( a._curveTimeOnlyToSort > b._curveTimeOnlyToSort )
							return 1;
						return 0;
					} );
				}

				//generate curves

				PositionCurve = Owner.CreateCurve( Owner.CurveTypePosition, true );
				//!!!!right?
				UpCurve = Owner.CreateCurve( Owner.CurveTypeRotation, true );
				//Forward = new CurveLine();
				//UpCurve = new CurveLine();
				//ScaleCurve = new CurveLine();

				for( int n = 0; n < Points.Length; n++ )
				{
					var point = Points[ n ];

					point.TimeOnCurve = n;

					var time = n;
					//var time = (double)n / (double)Points.Length;

					//double time = ( allPointsZeroTime ? n : point.Time ) * Owner.TimeScale;

					var transform = point.Transform;

					object additionalData = null;


					var rounded = PositionCurve as CurveRoundedLine;
					if( rounded != null )
						additionalData = point.RoundedLineCurvatureRadius;

					var bezierPath = PositionCurve as CurveBezierPath;
					if( bezierPath != null )
					{
						//!!!!
						//CurveInSpacePointHandle

						var offset = transform.Rotation.GetForward() * transform.Scale.MaxComponent();
						var handle1 = transform.Position - offset;
						var handle2 = transform.Position + offset;

						additionalData = (handle1, handle2);
					}

					//transform.Rotation.GetForward( out var forward );
					transform.Rotation.GetUp( out var up );
					//curveData.Forward.AddPoint( time, forward );
					//curveData.Up.AddPoint( time, up );

					PositionCurve.AddPoint( time, point.Transform.Position, additionalData );
					UpCurve.AddPoint( time, up );

					//ScaleCurve.AddPoint( time, new Vector3( 1, 1, 1 ) );
					//ScaleCurve.AddPoint( time, new Vector3( point.Lanes, point.Lanes, point.Lanes ) );
					//ScaleCurve.AddPoint( time, point.Transform.Scale );
				}

				//calculate Length
				//if( Points.Length > 1 )
				//	curveData.TimeRange = new Range( PositionCurve.Points[ 0 ].Time, PositionCurve.Points[ Points.Length - 1 ].Time );


				//road modifiers
				foreach( var modifier in Owner.GetComponents<RoadModifier>( onlyEnabledInHierarchy: true ) )
				{
					PredefinedModifiers |= modifier.PredefinedModifiers.Value;

					//if( modifier.PredefinedModifiers.Value.HasFlag( RoadModifier.PredefinedModifiersEnum.Overpass ) )
					//	OverpassSupportHeight = modifier.OverpassSupportHeight;
				}
			}

			//public void GetClosestCurveTimeToPosition( Vector3 position, double timeStep, out double timeOnCurve )
			//{

			//	//!!!!с какой стороны двигаться и как долго

			//	//!!!!можно было бы детализировать на отрезке. даже рекурсивно

			//	//!!!!slowly


			//	var closestTime = 0.0;
			//	var closestDistanceSquared = double.MaxValue;


			//	var maxTime = LastPoint.TimeOnCurve;
			//	var maxTime2 = maxTime + timeStep;

			//	for( double time = 0; time <= maxTime2; time += timeStep )
			//	{
			//		var time2 = time;
			//		if( time2 > maxTime )
			//			time2 = maxTime;

			//		var trPosition = GetPositionByTime( time2 );

			//		var distanceSquared = ( trPosition - position ).LengthSquared();
			//		if( distanceSquared < closestDistanceSquared )
			//		{
			//			closestTime = time;
			//			closestDistanceSquared = distanceSquared;
			//		}
			//	}

			//	timeOnCurve = closestTime;
			//}

			[MethodImpl( (MethodImplOptions)512 )]
			public bool GetClosestCurveTimeToPosition( Vector3 position, double maxDistance, double distanceAccuracyThreshold, out double timeOnCurve )
			{

				//!!!!опционально указывать lane

				//!!!!can add interpolation

				//!!!!maybe do several iteractions with smallar maxDistance

				if( !disposed )
				{
					//generate octree
					if( closestCurveTimeToPositionOctree == null )
					{

						//!!!!good?
						var stepMultiplier = RoadType.WayToUse.Value == RoadType.WayToUseEnum.Driving ? 16.0 : 4.0;


						var resultBounds = new List<Bounds>( 256 );
						var resultIntervals = new List<RangeF>( 256 );

						var initSettings = new OctreeContainer.InitSettings();
						initSettings.InitialOctreeBounds = GetBounds().GetExpanded( 0.1 );
						closestCurveTimeToPositionOctree = new OctreeContainer( initSettings );

						var timeSteps = GetCurveTimeSteps( stepMultiplier, null );

						for( int n = 0; n < timeSteps.Count - 1; n++ )
						{
							var timeFrom = timeSteps[ n ];
							var timeTo = timeSteps[ n + 1 ];

							var timeLength = timeTo - timeFrom;
							var timeStep = timeLength / 10;

							GetPositionByTime( timeFrom, out var pFrom );
							var b = new Bounds( pFrom );
							//var b = new Bounds( GetPositionByTime( timeFrom ) );
							for( var time = timeFrom + timeStep; time <= timeTo + 0.00001; time += timeStep )
							{
								var t = time;
								if( t > timeTo )
									t = timeTo;
								GetPositionByTime( t, out var p );
								b.Add( ref p );
								//b.Add( GetPositionByTime( t ) );
							}

							closestCurveTimeToPositionOctree.AddObject( b, 1 );
							resultBounds.Add( b );
							resultIntervals.Add( new Range( timeFrom, timeTo ).ToRangeF() );
						}

						closestCurveTimeToPositionOctreeBounds = resultBounds.ToArray();
						closestCurveTimeToPositionOctreeTimeIntervals = resultIntervals.ToArray();



						//void AddInterval( Bounds b, RangeF timeInterval )
						//{
						//	closestCurveTimeToPositionOctree.AddObject( b, 1 );
						//	resultIntervals.Add( timeInterval );
						//}

						////foreach( var point in Points )
						////	AddPoint( point.Transform.Position, point.TimeOnCurve );

						//var timeIntervals = new Queue<Range>();
						//for( int n = 0; n < Points.Length - 1; n++ )
						//	timeIntervals.Enqueue( new Range( Points[ n ].TimeOnCurve, Points[ n + 1 ].TimeOnCurve ) );

						//while( timeIntervals.Count != 0 )
						//{
						//	var timeInterval = timeIntervals.Dequeue();

						//	var p1 = GetPositionByTime( timeInterval.Minimum );
						//	var p2 = GetPositionByTime( timeInterval.Maximum );

						//	if( ( p2 - p1 ).LengthSquared() > stepLength * stepLength )
						//	{
						//		var centerTime = timeInterval.GetCenter();
						//		var centerPosition = GetPositionByTime( centerTime );

						//		AddPoint( centerPosition, centerTime );

						//		timeIntervals.Enqueue( new Range( timeInterval.Minimum, centerTime ) );
						//		timeIntervals.Enqueue( new Range( centerTime, timeInterval.Maximum ) );
						//	}
						//}

						//closestCurveTimeToPositionOctreeTimeIntervals = resultIntervals.ToArray();




						//void AddPoint( Vector3 pos, double time )
						//{
						//	closestCurveTimeToPositionOctree.AddObject( new Bounds( pos ), 1 );
						//	resultPoints.Add( (float)time );
						//}

						//foreach( var point in Points )
						//	AddPoint( point.Transform.Position, point.TimeOnCurve );

						//var timeIntervals = new Queue<Range>();
						//for( int n = 0; n < Points.Length - 1; n++ )
						//	timeIntervals.Enqueue( new Range( Points[ n ].TimeOnCurve, Points[ n + 1 ].TimeOnCurve ) );

						//while( timeIntervals.Count != 0 )
						//{
						//	var timeInterval = timeIntervals.Dequeue();

						//	var p1 = GetPositionByTime( timeInterval.Minimum );
						//	var p2 = GetPositionByTime( timeInterval.Maximum );

						//	if( ( p2 - p1 ).LengthSquared() > stepLength * stepLength )
						//	{
						//		var centerTime = timeInterval.GetCenter();
						//		var centerPosition = GetPositionByTime( centerTime );

						//		AddPoint( centerPosition, centerTime );

						//		timeIntervals.Enqueue( new Range( timeInterval.Minimum, centerTime ) );
						//		timeIntervals.Enqueue( new Range( centerTime, timeInterval.Maximum ) );
						//	}
						//}

						//closestCurveTimeToPositionOctreePoints = resultPoints.ToArray();
					}

					//get from octree
					{
						var closestTime = 0.0;
						var closestDistanceSquared = double.MaxValue;

						var sphere = new Sphere( position, maxDistance );

						var indexes = closestCurveTimeToPositionOctree.GetObjects( sphere, 1, OctreeContainer.ModeEnum.All );
						if( indexes.Length != 0 )
						{
							foreach( var intervalIndex in indexes )
							{
								ref var bounds = ref closestCurveTimeToPositionOctreeBounds[ intervalIndex ];
								if( bounds.GetPointDistanceSquared( ref position ) < closestDistanceSquared )
								{
									var timeInterval = closestCurveTimeToPositionOctreeTimeIntervals[ intervalIndex ];
									var timeFrom = timeInterval.Minimum;
									var timeTo = timeInterval.Maximum;

									GetPositionByTime( timeFrom, out var p1 );
									GetPositionByTime( timeTo, out var p2 );
									var distanceSquared1 = ( p1 - position ).LengthSquared();
									var distanceSquared2 = ( p2 - position ).LengthSquared();

									if( distanceSquared1 < closestDistanceSquared )
									{
										closestTime = timeFrom;
										closestDistanceSquared = distanceSquared1;
									}
									if( distanceSquared2 < closestDistanceSquared )
									{
										closestTime = timeTo;
										closestDistanceSquared = distanceSquared2;
									}

									var length = bounds.GetSize().Length() * 1.1;
									if( length != 0 )
									{
										var steps = (int)( length / distanceAccuracyThreshold ) + 1;

										var timeLength = timeTo - timeFrom;
										var timeStep = timeLength / steps;

										//for( var time = timeFrom; time <= timeTo + 0.00001; time += timeStep )
										for( var time = timeFrom + timeStep; time < timeTo; time += timeStep )
										{
											var t = time;
											//if( t > timeTo )
											//	break;

											GetPositionByTime( t, out var p );
											var distanceSquared = ( p - position ).LengthSquared();
											if( distanceSquared < closestDistanceSquared )
											{
												closestTime = t;
												closestDistanceSquared = distanceSquared;
											}
										}
									}

									//var projected = MathAlgorithms.ProjectPointToLine( p1, p2, position );

									//var b = new Bounds( p1 );
									//b.Add( ref p2 );
									//b.Expand( 0.001 );
									//if( b.Contains( ref projected ) && projected != p1 )
									//{
									//	var d1 = ( projected - p1 ).Length();
									//	var total = ( p2 - p1 ).Length();
									//	if( total == 0 )
									//		total = 0.000001;
									//	var scale = MathEx.Saturate( d1 / total );
									//	var interpolatedTime = MathEx.Lerp( timeInterval.Minimum, timeInterval.Maximum, scale );

									//	{
									//		var distanceSquared = ( projected - position ).LengthSquared();
									//		if( distanceSquared < closestDistanceSquared )
									//		{
									//			closestTime = interpolatedTime;
									//			closestDistanceSquared = distanceSquared;
									//		}
									//	}

									//	//{
									//	//	var p = GetPositionByTime( interpolatedTime );
									//	//	var distanceSquared = ( p - position ).LengthSquared();
									//	//	if( distanceSquared < closestDistanceSquared )
									//	//	{
									//	//		closestTime = interpolatedTime;
									//	//		closestDistanceSquared = distanceSquared;
									//	//	}
									//	//}
									//}



									//if( interpolationSteps > 1 )
									//{
									//	var timeFrom = timeInterval.Minimum;
									//	var timeTo = timeInterval.Maximum;

									//	var timeLength = timeTo - timeFrom;
									//	var timeStep = timeLength / interpolationSteps;

									//	for( var time = timeFrom + timeStep; time <= timeTo + 0.00001; time += timeStep )
									//	{
									//		var t = time;
									//		if( t > timeTo )
									//			t = timeTo;

									//		var p = GetPositionByTime( t );
									//		var distanceSquared = ( p - position ).LengthSquared();
									//		if( distanceSquared < closestDistanceSquared )
									//		{
									//			closestTime = t;
									//			closestDistanceSquared = distanceSquared;
									//		}
									//	}


									//	//var projected = MathAlgorithms.ProjectPointToLine( p1, p2, position );

									//	//var b = new Bounds( p1 );
									//	//b.Add( ref p2 );
									//	//b.Expand( 0.001 );
									//	//if( b.Contains( ref projected ) && projected != p1 )
									//	//{
									//	//	var d1 = ( projected - p1 ).Length();
									//	//	var total = ( p2 - p1 ).Length();
									//	//	if( total == 0 )
									//	//		total = 0.000001;
									//	//	var scale = MathEx.Saturate( d1 / total );
									//	//	var interpolatedTime = MathEx.Lerp( timeInterval.Minimum, timeInterval.Maximum, scale );

									//	//	{
									//	//		var distanceSquared = ( projected - position ).LengthSquared();
									//	//		if( distanceSquared < closestDistanceSquared )
									//	//		{
									//	//			closestTime = interpolatedTime;
									//	//			closestDistanceSquared = distanceSquared;
									//	//		}
									//	//	}

									//	//	//{
									//	//	//	var p = GetPositionByTime( interpolatedTime );
									//	//	//	var distanceSquared = ( p - position ).LengthSquared();
									//	//	//	if( distanceSquared < closestDistanceSquared )
									//	//	//	{
									//	//	//		closestTime = interpolatedTime;
									//	//	//		closestDistanceSquared = distanceSquared;
									//	//	//	}
									//	//	//}
									//	//}

									//}
									//else
									//{
									//	var p1 = GetPositionByTime( timeInterval.Minimum );
									//	var p2 = GetPositionByTime( timeInterval.Maximum );
									//	var distanceSquared1 = ( p1 - position ).LengthSquared();
									//	var distanceSquared2 = ( p2 - position ).LengthSquared();

									//	if( distanceSquared1 < closestDistanceSquared )
									//	{
									//		closestTime = timeInterval.Minimum;
									//		closestDistanceSquared = distanceSquared1;
									//	}
									//	if( distanceSquared2 < closestDistanceSquared )
									//	{
									//		closestTime = timeInterval.Maximum;
									//		closestDistanceSquared = distanceSquared2;
									//	}
									//}
								}
							}


							//var p1 = GetPositionByTime( timeInterval.Minimum );
							//var p2 = GetPositionByTime( timeInterval.Maximum );
							//var distanceSquared1 = ( p1 - position ).LengthSquared();
							//var distanceSquared2 = ( p2 - position ).LengthSquared();


							//if( distanceSquared1 < closestDistanceSquared || distanceSquared2 < closestDistanceSquared )
							//{
							//	if( interpolationSteps > 1 )
							//	{
							//		var timeFrom = timeInterval.Minimum;
							//		var timeTo = timeInterval.Maximum;

							//		var timeLength = timeTo - timeFrom;
							//		var timeStep = timeLength / interpolationSteps;

							//		for( var time = timeFrom + timeStep; time <= timeTo + 0.00001; time += timeStep )
							//		{
							//			var t = time;
							//			if( t > timeTo )
							//				t = timeTo;

							//			var p = GetPositionByTime( t );
							//			var distanceSquared = ( p - position ).LengthSquared();
							//			if( distanceSquared < closestDistanceSquared )
							//			{
							//				closestTime = t;
							//				closestDistanceSquared = distanceSquared;
							//			}
							//		}


							//		//var projected = MathAlgorithms.ProjectPointToLine( p1, p2, position );

							//		//var b = new Bounds( p1 );
							//		//b.Add( ref p2 );
							//		//b.Expand( 0.001 );
							//		//if( b.Contains( ref projected ) && projected != p1 )
							//		//{
							//		//	var d1 = ( projected - p1 ).Length();
							//		//	var total = ( p2 - p1 ).Length();
							//		//	if( total == 0 )
							//		//		total = 0.000001;
							//		//	var scale = MathEx.Saturate( d1 / total );
							//		//	var interpolatedTime = MathEx.Lerp( timeInterval.Minimum, timeInterval.Maximum, scale );

							//		//	{
							//		//		var distanceSquared = ( projected - position ).LengthSquared();
							//		//		if( distanceSquared < closestDistanceSquared )
							//		//		{
							//		//			closestTime = interpolatedTime;
							//		//			closestDistanceSquared = distanceSquared;
							//		//		}
							//		//	}

							//		//	//{
							//		//	//	var p = GetPositionByTime( interpolatedTime );
							//		//	//	var distanceSquared = ( p - position ).LengthSquared();
							//		//	//	if( distanceSquared < closestDistanceSquared )
							//		//	//	{
							//		//	//		closestTime = interpolatedTime;
							//		//	//		closestDistanceSquared = distanceSquared;
							//		//	//	}
							//		//	//}
							//		//}
							//	}
							//	else
							//	{
							//		if( distanceSquared1 < closestDistanceSquared )
							//		{
							//			closestTime = timeInterval.Minimum;
							//			closestDistanceSquared = distanceSquared1;
							//		}
							//		if( distanceSquared2 < closestDistanceSquared )
							//		{
							//			closestTime = timeInterval.Maximum;
							//			closestDistanceSquared = distanceSquared2;
							//		}
							//	}
							//}


							//foreach( var pointIndex in indexes )
							//{
							//	var pointTime = closestCurveTimeToPositionOctreePoints[ pointIndex ];
							//	var pointPosition = GetPositionByTime( pointTime );

							//	var distanceSquared = ( pointPosition - position ).LengthSquared();
							//	if( distanceSquared < closestDistanceSquared )
							//	{
							//		closestTime = pointTime;
							//		closestDistanceSquared = distanceSquared;
							//	}
							//}

							timeOnCurve = closestTime;
							return true;
						}
					}
				}

				timeOnCurve = 0;
				return false;
			}

			//public bool GetCurveTimeByPosition( Vector3 position, double maxDistanceToCurve, out double timeOnCurve )
			//{

			//	//!!!!slowly

			//too big step
			//	var step = 0.05;
			//	var maxTime = LastPoint.TimeOnCurve;
			//	var maxTime2 = maxTime + step;

			//	Vector3? previousPosition = null;

			//	for( double time = 0; time <= maxTime2; time += step )
			//	{
			//		var time2 = time;
			//		if( time2 > maxTime )
			//			time2 = maxTime;

			//		var trPosition = GetPositionByTime( time2 );

			//		if( previousPosition.HasValue )
			//		{
			//			var projected = MathAlgorithms.ProjectPointToLine( previousPosition.Value, trPosition, position );

			//			var b = new Bounds( previousPosition.Value );
			//			b.Add( trPosition );
			//			b.Expand( 0.001 );
			//			if( b.Contains( projected ) && ( projected - position ).LengthSquared() <= maxDistanceToCurve * maxDistanceToCurve )
			//			{
			//				timeOnCurve = time2;
			//				return true;
			//			}
			//		}

			//		previousPosition = trPosition;
			//	}

			//	timeOnCurve = 0;
			//	return false;
			//}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void GetPositionByTime( double time, out Vector3 result )
			{
				PositionCurve.CalculateValueByTime( time, out result );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void GetUpByTime( double time, out Vector3 result )
			{
				UpCurve.CalculateValueByTime( time, out result );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void GetDirectionByTime( double time, out Vector3 result )
			{
				//!!!!0.01

				if( time < 0.01 )
				{
					GetPositionByTime( 0.01, out var v1 );
					GetPositionByTime( 0, out var v2 );
					Vector3.Subtract( ref v1, ref v2, out result );
					//dir = GetPositionByTime( 0.01 ) - GetPositionByTime( 0 );
				}
				else if( time > LastPoint.TimeOnCurve - 0.01 )
				{
					GetPositionByTime( LastPoint.TimeOnCurve, out var v1 );
					GetPositionByTime( LastPoint.TimeOnCurve - 0.01, out var v2 );
					Vector3.Subtract( ref v1, ref v2, out result );
					//dir = GetPositionByTime( LastPoint.TimeOnCurve ) - GetPositionByTime( LastPoint.TimeOnCurve - 0.01 );
				}
				else
				{
					GetPositionByTime( time + 0.01, out var v1 );
					GetPositionByTime( time - 0.01, out var v2 );
					Vector3.Subtract( ref v1, ref v2, out result );
					//dir = GetPositionByTime( time + 0.01 ) - GetPositionByTime( time - 0.01 );
				}

				result.Normalize();


				//Vector3 dir;

				//if( time < 0.01 )
				//	dir = GetPositionByTime( 0.01 ) - GetPositionByTime( 0 );
				//else if( time > LastPoint.TimeOnCurve - 0.01 )
				//	dir = GetPositionByTime( LastPoint.TimeOnCurve ) - GetPositionByTime( LastPoint.TimeOnCurve - 0.01 );
				//else
				//	dir = GetPositionByTime( time + 0.01 ) - GetPositionByTime( time - 0.01 );

				//return dir.GetNormalize();
			}

			public Point LastPoint
			{
				get { return Points[ Points.Length - 1 ]; }
			}

			public double GetLaneOffset( int laneIndex )
			{
				var laneWidth = RoadType.LaneWidth.Value;
				if( Lanes > 1 )
					return laneWidth * 0.5 * ( Lanes - 1 ) - laneWidth * laneIndex;
				return 0;
			}

			public List<double> GetCurveTimeSteps( double stepMultiplier, Range? timeClipRange )
			{
				var points = Points;
				var segmentsLength = RoadType.SegmentsLength.Value;
				if( segmentsLength <= 0.001 )
					segmentsLength = 0.001;

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

					var firstAdded = false;

					for( int nPoint = 0; nPoint < points.Length - 1; nPoint++ )
					{
						var pointFrom = points[ nPoint ];
						var pointTo = points[ nPoint + 1 ];
						var timeFrom = pointFrom.TimeOnCurve;
						var timeTo = pointTo.TimeOnCurve;

						var length = ( pointTo.Transform.Position - pointFrom.Transform.Position ).Length();
						var timeStep = segmentsLength / length / stepDivide;
						timeStep *= stepMultiplier;

						var timeEnd = timeTo - timeStep * 0.1;
						for( double time = timeFrom; time < timeEnd; time += timeStep )
						{
							if( timeClipRange.HasValue )
							{
								if( time > timeClipRange.Value.Minimum )
								{
									if( !firstAdded )
									{
										timeSteps.Add( timeClipRange.Value.Minimum );
										firstAdded = true;
									}

									if( time < timeClipRange.Value.Maximum )
										timeSteps.Add( time );
								}
							}
							else
								timeSteps.Add( time );
						}
					}

					if( timeClipRange.HasValue )
						timeSteps.Add( timeClipRange.Value.Maximum );
					else
						timeSteps.Add( points[ points.Length - 1 ].TimeOnCurve );
				}

				return timeSteps;
			}

			public CurveData[] GetBorderCurves()
			{
				if( borderCurves == null )
				{
					////var lanes = Owner.Lanes.Value;
					var laneWidth = RoadType.LaneWidth.Value;
					var sidewalkEdgeWidth = RoadType.RoadsideEdgeWidth.Value;
					////var sidewalkWidth = RoadType.RoadsideWidth.Value;
					//var points = Points;
					////var ownerPosition = Owner.TransformV.Position;
					//var segmentsLength = RoadType.SegmentsLength.Value;
					//if( segmentsLength <= 0.001 )
					//	segmentsLength = 0.001;

					var timeSteps = GetCurveTimeSteps( 4.0, null );


					//!!!!можно сразу все посчитать за один обход


					//!!!!
					borderCurves = new CurveData[ 2 ];


					for( int side = 0; side < 2; side++ )
					{
						var item = new CurveData();

						var lanes = Owner.Lanes.Value;
						var width = lanes * laneWidth + sidewalkEdgeWidth * 2;
						var widthOffset = ( side == 0 ? width : -width ) * 0.5;

						var list = new List<(double time, Vector3 position, Quaternion rotation)>( timeSteps.Count );

						for( int nTime = 0; nTime < timeSteps.Count; nTime++ )
						{
							var time = timeSteps[ nTime ];

							GetPositionByTime( time, out var pos );
							GetDirectionByTime( time, out var dir );
							GetUpByTime( time, out var up );

							var rot = Quaternion.LookAt( dir, up );

							pos += rot * new Vector3( 0, widthOffset, 0 );

							list.Add( (time, pos, rot) );
						}

						//list.Add( (0, Points[ 0 ].Transform.Position) );
						//list.Add( (1, Points[ 1 ].Transform.Position) );

						item.Curve = list.ToArray();

						borderCurves[ side ] = item;
					}

				}

				return borderCurves;
			}

			public CurveData[] GetLaneCurves()
			{
				if( laneCurves == null )
				{
					var timeSteps = GetCurveTimeSteps( 4.0, null );

					laneCurves = new CurveData[ Lanes ];
					for( int laneIndex = 0; laneIndex < Lanes; laneIndex++ )
					{
						var lane = new CurveData();

						var list = new List<(double time, Vector3 position, Quaternion rotation)>( timeSteps.Count );

						for( int nTime = 0; nTime < timeSteps.Count; nTime++ )
						{
							var time = timeSteps[ nTime ];

							GetPositionByTime( time, out var pos );
							GetDirectionByTime( time, out var dir );
							GetUpByTime( time, out var up );

							var rot = Quaternion.LookAt( dir, up );

							pos += rot * new Vector3( 0, GetLaneOffset( laneIndex ), 0 );

							list.Add( (time, pos, rot) );
						}

						lane.Curve = list.ToArray();

						laneCurves[ laneIndex ] = lane;
					}
				}

				return laneCurves;
			}

			public Bounds GetBounds()
			{
				if( !bounds.HasValue )
				{
					var result = Bounds.Cleared;

					foreach( var borderCurve in GetBorderCurves() )
					{
						if( borderCurve.Curve != null )
						{
							foreach( var point in borderCurve.Curve )
								result.Add( point.Position );
						}
					}

					if( result.IsCleared() )
						result = new Bounds( -1, -1, -1, 1, 1, 1 );
					bounds = result;
				}
				return bounds.Value;
			}
		}

		///////////////////////////////////////////////

		public class LogicalData
		{
			public Road Owner;
			public Scene Scene;
			public RoadData RoadData;

			public ESet<RoadNode.LogicalData> ConnectedNodes = new ESet<RoadNode.LogicalData>();
			//public Dictionary<LogicalData, List<ConnectedRoadItem>> ConnectedRoads = new Dictionary<LogicalData, List<ConnectedRoadItem>>();

			Range[] timeClipRanges;

			public List<Scene.PhysicsWorldClass.Body> CollisionBodies = new List<Scene.PhysicsWorldClass.Body>();

			/////////////////////

			public LogicalData( Road owner )
			{
				Owner = owner;
				Scene = Owner.ParentScene;

				//!!!!когда изменилось

				RoadData = owner.GetRoadData();
			}

			public virtual void OnDelete()
			{
				if( ConnectedNodes.Count != 0 )
				{
					//!!!!с этим зацикливается
					//foreach( var node in ConnectedNodes )
					//{
					//	node.OnConnectedRoadDelete( RoadData );

					//	////node.ConnectedNodes.Remove( this );
					//	////node.Owner.NeedUpdate();
					//}

					ConnectedNodes.Clear();
				}

				DestroyCollisionBodies();
			}

			public void DropTimeClipRanges()
			{
				timeClipRanges = null;
			}

			public Range[] GetTimeClipRanges()
			{
				if( timeClipRanges == null )
				{
					//get modifiers from connected nodes
					var modifiers = new List<Modifier>();
					foreach( var node in ConnectedNodes )
						node.GetModifiersForRoad( RoadData, modifiers );

					var timeClipRanges = new List<Range>();
					{
						var range = new Range( 0, RoadData.Points[ RoadData.Points.Length - 1 ].TimeOnCurve );
						var splitRanges = new List<Range>();

						//get range clipped by min max and get split ranges
						foreach( var modifier in modifiers )
						{
							if( modifier.MinTime.HasValue )
								range.Minimum = Math.Max( range.Minimum, modifier.MinTime.Value );
							if( modifier.MaxTime.HasValue )
								range.Maximum = Math.Min( range.Maximum, modifier.MaxTime.Value );
							if( modifier.TimeClipRange.HasValue )
								splitRanges.Add( modifier.TimeClipRange.Value );
						}

						timeClipRanges.Add( range );

						//split ranges
						foreach( var rangeToSplit in splitRanges )
						{
							var newList = new List<Range>();

							foreach( var r in timeClipRanges )
							{
								if( rangeToSplit.Minimum > r.Minimum && rangeToSplit.Maximum < r.Maximum )
								{
									newList.Add( new Range( r.Minimum, rangeToSplit.Minimum ) );
									newList.Add( new Range( rangeToSplit.Maximum, r.Maximum ) );
								}
								else
									newList.Add( r );
							}

							timeClipRanges = newList;
						}
					}

					this.timeClipRanges = timeClipRanges.ToArray();
				}

				return timeClipRanges;
			}

			//////////////////////////////////////////////

			//public class MeshData
			//{
			//	public List<RoadUtility.RoadGeometryGenerator.MeshGeometryItem> MeshGeometries = new List<RoadUtility.RoadGeometryGenerator.MeshGeometryItem>();
			//}

			//////////////////////////////////////////////

			[MethodImpl( (MethodImplOptions)512 )]
			public RoadUtility.MeshData GenerateMeshData( bool generateCollision )
			{
				var meshData = new RoadUtility.MeshData();

				//road geometry
				if( RoadData.Points.Length > 1 )
				{
					var manager = Scene.GetComponent<RoadManager>();

					DropTimeClipRanges();
					var timeClipRanges = GetTimeClipRanges();

					foreach( var timeClipRange in timeClipRanges )
					{
						var timeSteps = RoadData.GetCurveTimeSteps( 1, timeClipRange );

						double totalLength = 0;
						{
							var previous = Vector3.Zero;
							for( int n = 0; n < timeSteps.Count; n++ )
							{
								var time = timeSteps[ n ];

								RoadData.GetPositionByTime( time, out var v );
								if( n != 0 )
									totalLength += ( v - previous ).Length();
								previous = v;
							}
						}

						var geometryGenerator = new RoadUtility.RoadGeometryGenerator();
						geometryGenerator.Mode = RoadUtility.RoadGeometryGenerator.ModeEnum.Road;
						geometryGenerator.RoadType = Owner.RoadType;
						geometryGenerator.Lanes = Owner.Lanes.Value;
						geometryGenerator.PositionCurve = RoadData.PositionCurve;
						geometryGenerator.UpCurve = RoadData.UpCurve;

						var ownerPosition = Owner.TransformV.Position;

						//surface
						{
							geometryGenerator.GenerateRoadMeshData( ownerPosition, timeSteps, totalLength, RoadUtility.RoadGeometryGenerator.GeneratePartEnum.Surface, meshData.MeshGeometries );

							//!!!!
							//mesh.CastShadows = existsOverpass;
						}

						//markup
						if( RoadData.RoadType.MarkupMaterial.ReferenceSpecified && ( manager == null || manager.DisplayMarkup ) )
						{
							//markup
							if( !generateCollision )
							{
								for( int laneIndex = 0; laneIndex < RoadData.Lanes; laneIndex++ )
								{
									var markupOffset = RoadData.GetLaneOffset( laneIndex );

									var laneWidth = RoadData.RoadType.LaneWidth.Value;

									{
										var markupRoadside = laneIndex == 0;
										var markupSolid = markupRoadside;

										//!!!!
										if( RoadData.Lanes >= 4 && laneIndex == RoadData.Lanes / 2 )
											markupSolid = true;

										geometryGenerator.GenerateRoadMeshData( ownerPosition, timeSteps, totalLength, RoadUtility.RoadGeometryGenerator.GeneratePartEnum.Markup, meshData.MeshGeometries, markupOffset + laneWidth * 0.5, markupRoadside, markupSolid );
									}

									if( laneIndex == RoadData.Lanes - 1 )
									{
										var markupRoadside = true;
										var markupSolid = true;
										geometryGenerator.GenerateRoadMeshData( ownerPosition, timeSteps, totalLength, RoadUtility.RoadGeometryGenerator.GeneratePartEnum.Markup, meshData.MeshGeometries, markupOffset - laneWidth * 0.5, markupRoadside, markupSolid );
									}
								}
							}
						}

						//ShoulderLeft
						if( RoadData.PredefinedModifiers.HasFlag( RoadModifier.PredefinedModifiersEnum.ShoulderLeft ) )
							geometryGenerator.GenerateRoadMeshData( ownerPosition, timeSteps, totalLength, RoadUtility.RoadGeometryGenerator.GeneratePartEnum.ShoulderLeft, meshData.MeshGeometries );

						//ShoulderRight
						if( RoadData.PredefinedModifiers.HasFlag( RoadModifier.PredefinedModifiersEnum.ShoulderRight ) )
							geometryGenerator.GenerateRoadMeshData( ownerPosition, timeSteps, totalLength, RoadUtility.RoadGeometryGenerator.GeneratePartEnum.ShoulderRight, meshData.MeshGeometries );

						//SidewalkLeft
						if( RoadData.PredefinedModifiers.HasFlag( RoadModifier.PredefinedModifiersEnum.SidewalkLeft ) )
						{
							for( int n = 0; n < 5; n++ )
								geometryGenerator.GenerateRoadMeshData( ownerPosition, timeSteps, totalLength, RoadUtility.RoadGeometryGenerator.GeneratePartEnum.SidewalkLeft1 + n, meshData.MeshGeometries );
						}

						//SidewalkRight
						if( RoadData.PredefinedModifiers.HasFlag( RoadModifier.PredefinedModifiersEnum.SidewalkRight ) )
						{
							for( int n = 0; n < 5; n++ )
								geometryGenerator.GenerateRoadMeshData( ownerPosition, timeSteps, totalLength, RoadUtility.RoadGeometryGenerator.GeneratePartEnum.SidewalkRight1 + n, meshData.MeshGeometries );
						}


						//!!!!
#if ___

						//overpass
						if( PredefinedModifiers.HasFlag( RoadModifier.PredefinedModifiersEnum.Overpass ) )
						{
							var mesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );

							//if( existsOverpass )//points.FirstOrDefault( p => p.Specialty == RoadPoint.SpecialtyEnum.Overpass ) != null )
							{
								var ranges = new List<(int, int)>();
								int? started = null;
								int count = 0;
								for( int n = 0; n < Points.Length; n++ )
								{
									//var point = points[ n ];
									//if( point.Specialty == RoadPoint.SpecialtyEnum.Overpass )
									{
										if( started == null )
											started = n;
										count++;
									}
									//else
									//{
									//	if( started.HasValue )
									//	{
									//		ranges.Add( (started.Value, count) );
									//		started = null;
									//		count = 0;
									//	}
									//}
								}
								if( started.HasValue )
									ranges.Add( (started.Value, count) );

								foreach( var range in ranges )
								{
									Owner.GenerateRoadMeshData( mesh, timeClipRange, timeSteps, GeneratePartEnum.OverpassTopLeft, range.Item1, range.Item2 );
									Owner.GenerateRoadMeshData( mesh, timeClipRange, timeSteps, GeneratePartEnum.OverpassTopRight, range.Item1, range.Item2 );
									Owner.GenerateRoadMeshData( mesh, timeClipRange, timeSteps, GeneratePartEnum.OverpassSideLeft, range.Item1, range.Item2 );
									Owner.GenerateRoadMeshData( mesh, timeClipRange, timeSteps, GeneratePartEnum.OverpassSideRight, range.Item1, range.Item2 );
									Owner.GenerateRoadMeshData( mesh, timeClipRange, timeSteps, GeneratePartEnum.OverpassBottom, range.Item1, range.Item2 );
								}
							}

							if( mesh.Components.Count != 0 )
							{
								mesh.VisibilityDistanceFactor = RoadType.VisibilityDistanceFactor;
								mesh.CastShadows = true;
								mesh.Enabled = true;

								result.MeshesToDispose.Add( mesh );
								result.MeshObjects.Add( (mesh, new Transform( Owner.TransformV.Position ), false/*, MeshData.MarkedPartEnum.Overpass*/) );
							}
							else
							{
								mesh.Dispose();
							}
						}
#endif

					}

				}

				//!!!!
#if ___

				foreach( var point in Points )
				{
					if( point.Modifiers.HasFlag( RoadPoint.ModifiersEnum.OverpassSupport ) )
					{
						var height = point.OverpassColumnHeight;

						if( height < 0 )
						{
							//!!!!impl

							height = point.Transform.Position.Z - RoadType.OverpassHeight;

							//height = 5;
						}

						if( height > 0 )
						{
							var topMesh = RoadType.OverpassSupportTopMesh.Value;
							var bottomMesh = RoadType.OverpassSupportBottomMesh.Value;

							var dir = GetDirectionByTime( point.TimeOnCurve );
							dir.Z = 0;
							dir.Normalize();

							var rot = Quaternion.LookAt( dir, Vector3.ZAxis );

							var top = point.Transform.Position - new Vector3( 0, 0, RoadType.OverpassHeight );
							var bottom = point.Transform.Position - new Vector3( 0, 0, RoadType.OverpassHeight + height );

							if( topMesh != null )
							{
								var tr = new Transform( top, rot );
								result.MeshObjects.Add( (topMesh, tr, true/*, MeshData.MarkedPartEnum.OverpassSupport*/) );
							}

							if( bottomMesh != null )
							{
								var tr = new Transform( bottom, rot );
								result.MeshObjects.Add( (bottomMesh, tr, true/*, MeshData.MarkedPartEnum.OverpassSupport*/) );
							}

							if( RoadType.OverpassSupportColumnRadius.Value > 0 )
							{
								var mesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );

								//!!!!по другому. UV Tiles

								var geometry = mesh.CreateComponent<MeshGeometry_Cylinder>();

								geometry.Radius = RoadType.OverpassSupportColumnRadius.Value;
								//!!!!
								geometry.Height = ( top.Z - bottom.Z ) - 0.2;

								geometry.Material = RoadType.OverpassSupportColumnMaterial;

								mesh.VisibilityDistanceFactor = RoadType.VisibilityDistanceFactor;
								//mesh.CastShadows = false;
								mesh.Enabled = true;

								result.MeshesToDispose.Add( mesh );

								var pos = ( top + bottom ) / 2;

								result.MeshObjects.Add( (mesh, new Transform( pos ), false/*, MeshData.MarkedPartEnum.OverpassSupport*/) );
							}

						}
					}
				}
#endif

				return meshData;
			}

			public void UpdateCollisionBodies()
			{
				DestroyCollisionBodies();

				if( !Owner.RoadCollision.Value )
					return;
				if( EditorAPI.IsAnyTransformToolInModifyingMode() )
				{
					Owner.needUpdateLogicalDataAfterEndModifyingTransformTool = true;
					return;
				}
				if( !RoadData.RoadType.SurfaceMaterial.ReferenceOrValueSpecified )
					return;

				//!!!!пока всё сразу. меши эстакад и другие меши юзать "Collision Definition"

				//!!!!можно проще создавать коллижен, т.к. не нужно текстурных координат

				var manager = Scene.GetComponent<RoadManager>();
				if( manager == null || manager.Collision )
				{
					var meshData = GenerateMeshData( true );
					var ownerPosition = Owner.TransformV.Position;

					var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

					var totalVertices = 0;
					var totalIndices = 0;
					foreach( var item in meshData.MeshGeometries )
					{
						totalVertices += item.Vertices.Length / vertexSize;
						totalIndices += item.Indices.Length;
					}
					var vertices = new List<Vector3F>( totalVertices );
					var indices = new List<int>( totalIndices );

					foreach( var item in meshData.MeshGeometries )
					{
						var startVertex = vertices.Count;

						var vertexCount = item.Vertices.Length / vertexSize;
						for( int n = 0; n < vertexCount; n++ )
						{
							unsafe
							{
								fixed( byte* pVertices = item.Vertices )
								{
									var v = *(Vector3F*)( pVertices + n * vertexSize );
									vertices.Add( v );
								}
							}
						}

						foreach( var index in item.Indices )
							indices.Add( index + startVertex );
					}

					//create rigid body
					if( vertices.Count != 0 && indices.Count != 0 )
					{
						var shapeData = new RigidBody();
						//body.Transform = new Transform( ownerPosition );

						var meshShape = shapeData.CreateComponent<CollisionShape_Mesh>();
						meshShape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.TriangleMesh;
						meshShape.CheckValidData = false;
						//!!!!
						meshShape.MergeEqualVerticesRemoveInvalidTriangles = false;

						meshShape.Vertices = vertices.ToArray();
						meshShape.Indices = indices.ToArray();

						//!!!!
						{
							var roadType = RoadData.RoadType;

							shapeData.Material = roadType.SurfaceCollisionMaterial;
							//!!!!
							//body.MaterialFrictionMode = roadType.SurfaceCollisionFrictionMode;
							shapeData.MaterialFriction = roadType.SurfaceCollisionFriction;
							//!!!!
							//body.MaterialAnisotropicFriction = roadType.SurfaceCollisionAnisotropicFriction;
							//body.MaterialSpinningFriction = roadType.SurfaceCollisionSpinningFriction;
							//body.MaterialRollingFriction = roadType.SurfaceCollisionRollingFriction;
							shapeData.MaterialRestitution = roadType.SurfaceCollisionRestitution;
						}
						//UpdateCollisionMaterial();

						var shapeItem = Scene.PhysicsWorld.AllocateShape( shapeData, Vector3.One );
						if( shapeItem != null )
						{
							var bodyItem = Scene.PhysicsWorld.CreateRigidBodyStatic( shapeItem, true, this, ownerPosition, QuaternionF.Identity );
							if( bodyItem != null )
								CollisionBodies.Add( bodyItem );
						}


						////need set ShowInEditor = false before AddComponent
						//var body = ComponentUtility.CreateComponent<RigidBody>( null, false, false );
						//body.DisplayInEditor = false;
						//Owner.AddComponent( body, -1 );
						////var group = scene.CreateComponent<GroupOfObjects>();

						////CollisionBody.Name = "__Collision Body";
						//body.SaveSupport = false;
						//body.CloneSupport = false;
						//body.CanBeSelected = false;
						////!!!!
						////body.SpaceBoundsUpdateEvent += CollisionBody_SpaceBoundsUpdateEvent;

						//body.Transform = new Transform( ownerPosition );

						//var shape = body.CreateComponent<CollisionShape_Mesh>();
						//shape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.TriangleMesh;
						//shape.CheckValidData = false;
						////!!!!
						//shape.MergeEqualVerticesRemoveInvalidTriangles = false;

						//shape.Vertices = vertices.ToArray();
						//shape.Indices = indices.ToArray();

						//UpdateCollisionMaterial();

						//body.Enabled = true;

						//CollisionBodies.Add( body );
					}
				}
			}

			public void UpdateCollisionMaterial()
			{
				//var roadType = RoadData.RoadType;

				//!!!!impl


				//foreach( var body in CollisionBodies )
				//{
				//	body.Material = roadType.SurfaceCollisionMaterial;
				//	//!!!!
				//	//body.MaterialFrictionMode = roadType.SurfaceCollisionFrictionMode;
				//	body.MaterialFriction = roadType.SurfaceCollisionFriction;
				//	//!!!!
				//	//body.MaterialAnisotropicFriction = roadType.SurfaceCollisionAnisotropicFriction;
				//	//body.MaterialSpinningFriction = roadType.SurfaceCollisionSpinningFriction;
				//	//body.MaterialRollingFriction = roadType.SurfaceCollisionRollingFriction;
				//	body.MaterialRestitution = roadType.SurfaceCollisionRestitution;
				//}


				//foreach( var body in CollisionBodies )
				//{
				//	body.Material = roadType.SurfaceCollisionMaterial;
				//	//!!!!
				//	//body.MaterialFrictionMode = roadType.SurfaceCollisionFrictionMode;
				//	body.MaterialFriction = roadType.SurfaceCollisionFriction;
				//	//!!!!
				//	//body.MaterialAnisotropicFriction = roadType.SurfaceCollisionAnisotropicFriction;
				//	//body.MaterialSpinningFriction = roadType.SurfaceCollisionSpinningFriction;
				//	//body.MaterialRollingFriction = roadType.SurfaceCollisionRollingFriction;
				//	body.MaterialRestitution = roadType.SurfaceCollisionRestitution;
				//}
			}

			//private void CollisionBody_SpaceBoundsUpdateEvent( ObjectInSpace obj, ref SpaceBounds newBounds )
			//{
			//	newBounds = Owner.SpaceBounds;
			//}

			public void DestroyCollisionBodies()
			{
				if( CollisionBodies.Count != 0 )
				{
					foreach( var body in CollisionBodies )
						body.Dispose();
					CollisionBodies.Clear();
				}

				//foreach( var body in CollisionBodies )
				//{
				//	//body.SpaceBoundsUpdateEvent -= CollisionBody_SpaceBoundsUpdateEvent;
				//	body.Dispose();
				//}
				//CollisionBodies.Clear();
			}

			public ESet<Crossroad.CrossroadLogicalData.ConnectedRoadItem> GetConnectedRoadsInInterval( Range timeRange, Crossroad.CrossroadLogicalData skipCrossroad )
			//public ESet<(LogicalData, Crossroad.CrossroadLogicalData.ConnectedRoadItem)> GetConnectedRoadsInInterval( Range timeRange, Crossroad.CrossroadLogicalData skipCrossroad )
			{
				var result = new ESet<Crossroad.CrossroadLogicalData.ConnectedRoadItem>();
				//var result = new ESet<(LogicalData, Crossroad.CrossroadLogicalData.ConnectedRoadItem)>();

				foreach( var connectedNode in ConnectedNodes )
				{
					var crossroad = connectedNode as Crossroad.CrossroadLogicalData;
					if( crossroad != null && crossroad != skipCrossroad )
					{
						var addCrossroad = false;
						foreach( var crossroadRoad in crossroad.ConnectedRoads )
						{
							if( crossroadRoad.ConnectedRoad == RoadData )
							{
								if( crossroadRoad.TimeOnCurve >= timeRange.Minimum && crossroadRoad.TimeOnCurve <= timeRange.Maximum )
								{
									addCrossroad = true;
									break;
								}
							}
						}

						if( addCrossroad )
						{
							foreach( var connectedRoad in crossroad.ConnectedRoads )
							{
								if( connectedRoad.ConnectedRoad != RoadData )
								{
									result.AddWithCheckAlreadyContained( connectedRoad );

									//var connectedRoadLogicalData = connectedRoad.ConnectedRoad.Owner.GetLogicalData();
									//if( connectedRoadLogicalData != null )
									//	result.AddWithCheckAlreadyContained( (connectedRoadLogicalData, connectedRoad) );// crossroad) );
								}
							}
						}
					}
				}

				return result;
			}
		}

		///////////////////////////////////////////////

		public class Modifier
		{
			public double? MinTime;
			public double? MaxTime;
			public Range? TimeClipRange;
		}

		///////////////////////////////////////////////

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow )
		{
			var point = CreateComponent<RoadPoint>();
			point.Name = point.BaseType.GetUserFriendlyNameForInstance();
			point.Transform = new Transform( Transform.Value.Position + new Vector3( 1, 0, 0 ), Quaternion.Identity );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Time ):
				//case nameof( CurveTypePosition ):
				//case nameof( RoundedLineCurvatureRadius ):

				//!!!!
				case nameof( CurveTypeRotation ):
				case nameof( CurveTypeScale ):

				case nameof( TimeScale ):
				case nameof( Color ):
				case nameof( Thickness ):
				case nameof( DisplayCurveInEditor ):
				case nameof( DisplayCurveInSimulation ):
					skip = true;
					break;

					//case nameof( OverpassSupportHeight ):
					//	if( Specialty.Value != RoadPoint.SpecialtyEnum.Overpass )
					//		skip = true;
					//	break;
				}
			}
		}

		public RoadData GetRoadData()
		{
			if( ( roadData == null || needUpdateRoadData ) && EnabledInHierarchyAndIsInstance /*&& canCreate*/ && ParentScene != null )
			{
				//dispose old
				if( roadData != null )
					roadData.Dispose();

				roadData = new RoadData( this );

				roadData.RoadType = RoadType;
				if( roadData.RoadType == null )
					roadData.RoadType = new RoadType();
				roadData.Lanes = Lanes;

				roadData.RoadTypeVersion = roadData.RoadType.Version;

				////var componentType = logicalData.RoadType.BaseType as Metadata.ComponentTypeInfo;
				////if( componentType != null )
				////{
				////	var roadObject = componentType.BasedOnObject as Road;
				////	if( roadObject != null )
				////		logicalData.RoadTypeVersion2 = roadObject.Version;
				////}

				roadData.GenerateCurveData();

				needUpdateRoadData = false;

				//need update nodes
				if( EnabledInHierarchyAndIsInstance )
				{
					var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, SpaceBounds.BoundingBox );
					roadData.Scene.GetObjectsInSpace( getObjectsItem );

					foreach( var item in getObjectsItem.Result )
					{
						var roadNode = item.Object as RoadNode;
						if( roadNode != null )
						{
							//!!!!более точно выбирать список. сейчас все в bounds

							var crossroad = roadNode as Crossroad;
							if( crossroad != null && crossroad.WayToUse.Value != roadData.RoadType.WayToUse.Value )
								continue;

							roadNode.NeedUpdate();
						}
					}
				}
			}

			return roadData;
		}

		public LogicalData GetLogicalData( bool canCreate = true )
		{
			if( logicalData == null && EnabledInHierarchyAndIsInstance && canCreate && ParentScene != null )
			//if( !logicalDataUpdating /*logicalData == null */ && EnabledInHierarchyAndIsInstance && canCreate && ParentScene != null )
			{
				//logicalDataUpdating = true;
				//try
				//{

				logicalData = new LogicalData( this );

				//inform road nodes in the bounds to connect this road
				{
					var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, SpaceBounds.BoundingBox );
					logicalData.Scene.GetObjectsInSpace( getObjectsItem );

					foreach( var item in getObjectsItem.Result )
					{
						var roadNode = item.Object as RoadNode;
						if( roadNode != null )
						{
							var data = roadNode.GetLogicalData( false );
							if( data != null )
							{
								//!!!!
								var crossroadData = data as Crossroad.CrossroadLogicalData;
								if( crossroadData != null )
								{
									var containsThisRoad = false;
									foreach( var roadItem in crossroadData.ConnectedRoads )
									{
										if( roadItem.ConnectedRoad.Owner == this )
										{
											containsThisRoad = true;
											break;
										}
									}

									if( containsThisRoad )
									{
										//!!!!
										if( !crossroadData.Valid )
										{
											logicalData = null;
											goto end;
										}

										logicalData.ConnectedNodes.Add( data );
									}
								}

								//!!!!было
								//data.Owner.NeedUpdate();//ConnectRoads( logicalData );
							}
						}
					}
				}

				if( sceneIsReady )
					logicalData.UpdateCollisionBodies();

				//}
				//finally
				//{
				//	logicalDataUpdating = false;
				//}
			}

			end:;
			return logicalData;
		}

		void DeleteLogicalData()
		{
			if( logicalData != null )
			{
				logicalData.OnDelete();
				logicalData = null;
			}
		}

		public RoadUtility.VisualData GetVisualData()
		{
			if( visualData == null )
			{
				GetLogicalData();

				if( logicalData != null && ParentScene != null && logicalData.RoadData.RoadType.SurfaceMaterial.ReferenceOrValueSpecified )
				{
					if( !RoadUtility.UpdateVisualDataInModifyingMode && EditorAPI.IsAnyTransformToolInModifyingMode() )
					{
						needUpdateLogicalDataAfterEndModifyingTransformTool = true;
						return null;
					}

					visualData = new RoadUtility.VisualData( this );

					var manager = logicalData.Scene.GetComponent<RoadManager>();
					if( manager == null || manager.Display )
					{
						var meshData = logicalData.GenerateMeshData( false );

						var tr = new Transform( TransformV.Position );
						visualData.CreateObjectsFromMeshData( meshData, tr, ColorMultiplier );

						//imperfections
						if( Age > 0 )
						{
							var surface = logicalData.RoadData.RoadType.AgeImperfection.Value;
							if( surface != null && surface.Result != null )
							{
								foreach( var geometry in meshData.MeshGeometries )
								{
									if( geometry.Part == RoadUtility.RoadGeometryGenerator.GeneratePartEnum.Surface )
									{
										var items = RoadUtility.CalculateSurfaceObjects( surface.Result, TransformV.Position, Age, geometry );
										if( items != null && items.Length != 0 )
											visualData.CreateGroupOfObjectsSubGroup( surface.Result, items );
									}
								}

								//var items = RoadUtility.CalculateSurfaceObjects( surface.Result, meshData, TransformV.Position, Age );
								//if( items != null && items.Length != 0 )
								//	visualData.CreateGroupOfObjectsSubGroup( surface.Result, items );
							}
						}


						//var meshNoShadows = ComponentUtility.CreateComponent<Mesh>( null, true, false );
						//meshNoShadows.CastShadows = false;

						//var meshWithShadows = ComponentUtility.CreateComponent<Mesh>( null, true, false );
						//meshNoShadows.CastShadows = true;

						//var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

						//foreach( var item in meshData.MeshGeometries )
						//{
						//	var mesh = item.CastShadows ? meshWithShadows : meshNoShadows;

						//	var meshGeometry = mesh.CreateComponent<MeshGeometry>();
						//	meshGeometry.VertexStructure = vertexStructure;
						//	meshGeometry.Vertices = item.Vertices;
						//	meshGeometry.Indices = item.Indices;
						//	meshGeometry.Material = item.Material;
						//}

						////!!!!
						////!!!!where else
						////mesh.VisibilityDistanceFactor = RoadType.VisibilityDistanceFactor;

						//for( int n = 0; n < 2; n++ )
						//{
						//	var mesh = n == 0 ? meshNoShadows : meshWithShadows;

						//	if( mesh.GetComponents<MeshGeometry>().Length != 0 )
						//	{
						//		mesh.MergeGeometriesWithEqualVertexStructureAndMaterial();
						//		mesh.Enabled = true;
						//		visualData.MeshesToDispose.Add( mesh );

						//		var tr = new Transform( TransformV.Position );
						//		visualData.CreateMeshObject( mesh, tr, false, null, ColorMultiplier );
						//	}
						//	else
						//		mesh.Dispose();
						//}
					}
				}
			}
			return visualData;
		}

		public void DeleteVisualData()
		{
			visualData?.Dispose();
			visualData = null;
			needUpdateVisualData = false;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = ParentScene;
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
				{
					scene.EnabledInHierarchyChanged += Scene_EnabledInHierarchyChanged;
					scene.ViewportUpdateBefore += Scene_ViewportUpdateBefore;
#if !DEPLOY
					TransformToolUtility.AllInstances_ModifyCommit += TransformTool_AllInstances_ModifyCommit;
					TransformToolUtility.AllInstances_ModifyCancel += TransformTool_AllInstances_ModifyCancel;
#endif
					if( logicalData == null )
						Update();
				}
				else
				{
					scene.EnabledInHierarchyChanged -= Scene_EnabledInHierarchyChanged;
					scene.ViewportUpdateBefore -= Scene_ViewportUpdateBefore;
#if !DEPLOY
					TransformToolUtility.AllInstances_ModifyCommit -= TransformTool_AllInstances_ModifyCommit;
					TransformToolUtility.AllInstances_ModifyCancel -= TransformTool_AllInstances_ModifyCancel;
#endif
					Update();
				}
			}

			if( !EnabledInHierarchy )
			{
				roadData?.Dispose();
				roadData = null;
			}
		}

#if !DEPLOY
		private void TransformTool_AllInstances_ModifyCommit( ITransformTool sender )
		{
			if( needUpdateLogicalDataAfterEndModifyingTransformTool )
			{
				NeedUpdateLogicalData();
				//Update();
				needUpdateLogicalDataAfterEndModifyingTransformTool = false;
			}
		}

		private void TransformTool_AllInstances_ModifyCancel( ITransformTool sender )
		{
			if( needUpdateLogicalDataAfterEndModifyingTransformTool )
			{
				NeedUpdateLogicalData();
				//Update();
				needUpdateLogicalDataAfterEndModifyingTransformTool = false;
			}
		}
#endif

		public void Update()
		{
			DeleteVisualData();
			DeleteLogicalData();

			if( EnabledInHierarchyAndIsInstance )
			{
				GetLogicalData();
				SpaceBoundsUpdate();
			}

			needUpdateLogicalData = false;
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			//!!!!use RoadData.Bounds?

			var roadData = GetRoadData();
			if( roadData != null && roadData.Points.Length != 0 )
			{
				var bounds = new Bounds( roadData.Points[ 0 ].Transform.Position );
				for( int n = 1; n < roadData.Points.Length; n++ )
					bounds.Add( roadData.Points[ n ].Transform.Position );
				//!!!!
				bounds.Expand( 0.5 );
				//bounds.Expand( data.RoadType.OutsideDiameter * 0.5 );
				newBounds = new SpaceBounds( bounds );
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EditorAutoUpdate && EnabledInHierarchyAndIsInstance )
			{
				if( logicalData != null )
				{

					//!!!!
					////!!!!при ините лишний раз вызывается
					//if( EngineApp.IsEditor )
					//{
					//	if( logicalData.RoadType.Version != logicalData.RoadTypeVersion )
					//	{
					//		needUpdate = true;
					//	}
					//}


					////var componentType = logicalData.RoadType.BaseType as Metadata.ComponentTypeInfo;
					////if( componentType != null )
					////{
					////	var roadObject = componentType.BasedOnObject as Road;
					////	if( roadObject != null )
					////	{
					////		if( logicalData.RoadTypeVersion2 != roadObject.Version )
					////			needUpdate = true;
					////	}
					////}
				}

				if( needUpdateLogicalData )
					Update();
				if( needUpdateVisualData )
					DeleteVisualData();
			}
		}

		private void Scene_EnabledInHierarchyChanged( Component obj )
		{
			if( obj.EnabledInHierarchyAndIsInstance )
			{
				if( needUpdateLogicalData )
					Update();

				var logicalData = GetLogicalData();
				logicalData?.UpdateCollisionBodies();

				sceneIsReady = true;
			}
		}

		private void Scene_ViewportUpdateBefore( Scene scene, Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		{
			//!!!!лучше всё же в OnGetRenderSceneData
			//хотя SpaceBounds видимо больше должен быть, учитывать габариты мешей
			//когда долго не видим объект, данные можно удалять. лишь бы не было что создается/удаляется каждый раз

			//!!!!может VisibleInHierarchy менять флаг в объектах

			if( VisibleInHierarchy )
				GetVisualData();
			else
				DeleteVisualData();
		}

		void DisplayRoadwayBorder( ViewportRenderingContext context )
		{
			var context2 = context.ObjectInSpaceRenderingContext;
			var renderer = context2.viewport.Simple3DRenderer;

			var roadData = GetRoadData();
			if( roadData != null && renderer != null )
			{
				ColorValue color;
				if( context2.selectedObjects.Contains( this ) )
					color = ProjectSettings.Get.Colors.SelectedColor;
				else if( context2.canSelectObjects.Contains( this ) )
					color = ProjectSettings.Get.Colors.CanSelectColor;
				else
					color = new ColorValue( 1, 1, 0, 0.8 );// ProjectSettings.Get.General.SceneShowVolumeColor;
				renderer.SetColor( color );

				var items = roadData.GetBorderCurves();

				foreach( var item in items )
				{
					Vector3? previousPos = null;
					for( int n = 0; n < item.Curve.Length; n++ )
					{
						var pos = item.Curve[ n ].Position;

						if( previousPos.HasValue )
						{
							renderer.AddLineThin( previousPos.Value, pos );
							//renderer.AddLine( previousPos.Value, pos );
						}

						previousPos = pos;
					}
				}

				var item0 = items[ 0 ];
				var item1 = items[ 1 ];
				renderer.AddLineThin( item0.Curve[ 0 ].Position, item1.Curve[ 0 ].Position );
				renderer.AddLineThin( item0.Curve[ item0.Curve.Length - 1 ].Position, item1.Curve[ item1.Curve.Length - 1 ].Position );
				//renderer.AddLine( item0.Curve[ 0 ].Position, item1.Curve[ 0 ].Position );
				//renderer.AddLine( item0.Curve[ item0.Curve.Length - 1 ].Position, item1.Curve[ item1.Curve.Length - 1 ].Position );
			}
		}

		void DisplayLaneCurves( ViewportRenderingContext context )
		{
			var context2 = context.ObjectInSpaceRenderingContext;
			var renderer = context2.viewport.Simple3DRenderer;

			var roadData = GetRoadData();
			if( roadData != null && renderer != null )
			{
				foreach( var lane in roadData.GetLaneCurves() )
				{
					//center line
					{
						renderer.SetColor( new ColorValue( 1, 1, 0, 0.8 ) );

						Vector3? previousPos = null;
						for( int n = 0; n < lane.Curve.Length; n++ )
						{
							var pos = lane.Curve[ n ].Position;

							if( previousPos.HasValue )
							{
								renderer.AddLineThin( previousPos.Value, pos );
								//renderer.AddLine( previousPos.Value, pos );
							}

							previousPos = pos;
						}
					}

					//////borders
					////{
					////	var offsetLength = logicalData.RoadType.LaneWidth.Value * 0.497;

					////	renderer.SetColor( new ColorValue( 1, 1, 0, 0.2 ) );

					////	for( int nBorder = 0; nBorder < 2; nBorder++ )
					////	{
					////		Vector3? previousPos = null;
					////		for( int n = 0; n < lane.Curve.Length; n++ )
					////		{
					////			var pos = lane.Curve[ n ].Position;
					////			var rot = lane.Curve[ n ].Rotation;

					////			if( nBorder == 0 )
					////				pos += rot.GetLeft() * offsetLength;
					////			else
					////				pos += rot.GetRight() * offsetLength;

					////			if( previousPos.HasValue )
					////				renderer.AddLine( previousPos.Value, pos );

					////			previousPos = pos;
					////		}
					////	}
					////}

					//connectors
					renderer.AddSphere( lane.Curve[ 0 ].Position, 0.1, 16, true );
					renderer.AddSphere( lane.Curve[ lane.Curve.Length - 1 ].Position, 0.1, 16, true );
				}
			}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			//!!!!тут?
			////!!!!не сразу обновляет если только тут
			//GetVisualData();

			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				//display roadway border
				{
					var display = false;
					if( EngineApp.IsEditor && DisplayRoadwayBorderInEditor )
						display = true;
					if( EngineApp.IsSimulation && DisplayRoadwayBorderInSimulation )
						display = true;
					if( EngineApp.IsEditor )
						if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this )
							display = true;

					//check for child point selected
					if( !display && context2.selectedObjects.Count != 0 )
					{
						foreach( var point in GetComponents<RoadPoint>() )
						{
							if( context2.selectedObjects.Contains( point ) )
								display = true;
						}
					}

					if( display )
						DisplayRoadwayBorder( context );
				}

				//display curve lanes
				{
					var display = false;
					if( EngineApp.IsEditor && DisplayLaneCurvesInEditor )
						display = true;
					if( EngineApp.IsSimulation && DisplayLaneCurvesInSimulation )
						display = true;

					//!!!!если попал в фрустум

					if( display )
						DisplayLaneCurves( context );
				}

				//debug lines
				var visualData = GetVisualData();
				if( visualData != null )
				{
					var renderer = context.Owner.Simple3DRenderer;

					//!!!!если попал в фрустум

					if( visualData.DebugLines.Count != 0 )
					{
						renderer.SetColor( new ColorValue( 1, 0, 0 ) );
						foreach( var line in visualData.DebugLines )
							renderer.AddLine( line );
					}
				}
			}
		}

		protected override bool GetAllowDisplayCurve()
		{
			return false;
		}

		public override void DataWasChanged()
		{
			base.DataWasChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				needUpdateRoadData = true;
				needUpdateLogicalData = true;
			}
		}

		public void NeedUpdateLogicalData()
		{
			needUpdateLogicalData = true;
		}

		public void NeedUpdateVisualData()
		{
			needUpdateVisualData = true;
		}

		//protected override bool OnIsVisualStatic()
		//{
		//	return true;
		//}
	}
}
