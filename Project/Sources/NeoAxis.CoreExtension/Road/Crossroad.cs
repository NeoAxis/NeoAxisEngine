// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A logical element to make crossroads.
	/// </summary>
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Road\Crossroad", 10540 )]
#endif
	public class Crossroad : RoadNode
	{
		/// <summary>
		/// The type of the crossroad.
		/// </summary>
		[DefaultValue( RoadType.WayToUseEnum.Driving )]
		public Reference<RoadType.WayToUseEnum> WayToUse
		{
			get { if( _wayToUse.BeginGet() ) WayToUse = _wayToUse.Get( this ); return _wayToUse.value; }
			set { if( _wayToUse.BeginSet( this, ref value ) ) { try { WayToUseChanged?.Invoke( this ); DataWasChanged(); } finally { _wayToUse.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WayToUse"/> property value changes.</summary>
		public event Action<Crossroad> WayToUseChanged;
		ReferenceField<RoadType.WayToUseEnum> _wayToUse = RoadType.WayToUseEnum.Driving;

		//!!!!может быть несколько радиусов кривизны для каждой пары

		/// <summary>
		/// The curvature radius between neighboring roads.
		/// </summary>
		[Range( 0, 50, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DefaultValue( 5.0 )]
		public Reference<double> CurvatureRadius
		{
			get { if( _curvatureRadius.BeginGet() ) CurvatureRadius = _curvatureRadius.Get( this ); return _curvatureRadius.value; }
			set { if( _curvatureRadius.BeginSet( this, ref value ) ) { try { CurvatureRadiusChanged?.Invoke( this ); DataWasChanged(); } finally { _curvatureRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurvatureRadius"/> property value changes.</summary>
		public event Action<Crossroad> CurvatureRadiusChanged;
		ReferenceField<double> _curvatureRadius = 5.0;

		/// <summary>
		/// Whether to visualize crossing markings.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> CrossingMarkings
		{
			get { if( _crossingMarkings.BeginGet() ) CrossingMarkings = _crossingMarkings.Get( this ); return _crossingMarkings.value; }
			set { if( _crossingMarkings.BeginSet( this, ref value ) ) { try { CrossingMarkingsChanged?.Invoke( this ); DataWasChanged(); } finally { _crossingMarkings.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrossingMarkings"/> property value changes.</summary>
		public event Action<Crossroad> CrossingMarkingsChanged;
		ReferenceField<bool> _crossingMarkings = false;

		/// <summary>
		/// Whether to add traffic lights.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> TrafficLights
		{
			get { if( _trafficLights.BeginGet() ) TrafficLights = _trafficLights.Get( this ); return _trafficLights.value; }
			set { if( _trafficLights.BeginSet( this, ref value ) ) { try { TrafficLightsChanged?.Invoke( this ); DataWasChanged(); } finally { _trafficLights.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrafficLights"/> property value changes.</summary>
		public event Action<Crossroad> TrafficLightsChanged;
		ReferenceField<bool> _trafficLights = false;

		[DefaultValue( "0 -1 0" )]
		public Reference<Vector3> TrafficLightsLocalOffset
		{
			get { if( _trafficLightsLocalOffset.BeginGet() ) TrafficLightsLocalOffset = _trafficLightsLocalOffset.Get( this ); return _trafficLightsLocalOffset.value; }
			set { if( _trafficLightsLocalOffset.BeginSet( this, ref value ) ) { try { TrafficLightsLocalOffsetChanged?.Invoke( this ); DataWasChanged(); } finally { _trafficLightsLocalOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrafficLightsLocalOffset"/> property value changes.</summary>
		public event Action<Crossroad> TrafficLightsLocalOffsetChanged;
		ReferenceField<Vector3> _trafficLightsLocalOffset = new Vector3( 0, -1, 0 );

		//[DefaultValue( 0.0 )]
		//public Reference<double> TrafficLightsOverrideDistance
		//{
		//	get { if( _trafficLightsOverrideDistance.BeginGet() ) TrafficLightsOverrideDistance = _trafficLightsOverrideDistance.Get( this ); return _trafficLightsOverrideDistance.value; }
		//	set { if( _trafficLightsOverrideDistance.BeginSet( this, ref value ) ) { try { TrafficLightsOverrideDistanceChanged?.Invoke( this ); DataWasChanged(); } finally { _trafficLightsOverrideDistance.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TrafficLightsOverrideDistance"/> property value changes.</summary>
		//public event Action<Crossroad> TrafficLightsOverrideDistanceChanged;
		//ReferenceField<double> _trafficLightsOverrideDistance = 0.0;

		/// <summary>
		/// Whether to display logical data in the editor.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> DisplayLogicalDataInEditor
		{
			get { if( _displayLogicalDataInEditor.BeginGet() ) DisplayLogicalDataInEditor = _displayLogicalDataInEditor.Get( this ); return _displayLogicalDataInEditor.value; }
			set { if( _displayLogicalDataInEditor.BeginSet( this, ref value ) ) { try { DisplayLogicalDataInEditorChanged?.Invoke( this ); } finally { _displayLogicalDataInEditor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayLogicalDataInEditor"/> property value changes.</summary>
		public event Action<Crossroad> DisplayLogicalDataInEditorChanged;
		ReferenceField<bool> _displayLogicalDataInEditor = false;

		/// <summary>
		/// Whether to display logical data in the simulation.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> DisplayLogicalDataInSimulation
		{
			get { if( _displayLogicalDataInSimulation.BeginGet() ) DisplayLogicalDataInSimulation = _displayLogicalDataInSimulation.Get( this ); return _displayLogicalDataInSimulation.value; }
			set { if( _displayLogicalDataInSimulation.BeginSet( this, ref value ) ) { try { DisplayLogicalDataInSimulationChanged?.Invoke( this ); } finally { _displayLogicalDataInSimulation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayLogicalDataInSimulation"/> property value changes.</summary>
		public event Action<Crossroad> DisplayLogicalDataInSimulationChanged;
		ReferenceField<bool> _displayLogicalDataInSimulation = false;

		///////////////////////////////////////////////

		public class CrossroadLogicalData : LogicalData
		{
			public List<ConnectedRoadItem> ConnectedRoads = new List<ConnectedRoadItem>();
			public bool Valid;
			public List<ConnectedRoadItem> SortedRoads;

			//public List<Road.RoadData> WalkingRoads = new List<Road.RoadData>();

			/////////////////////

			public class ConnectedRoadItem
			{
				public CrossroadLogicalData Owner;

				public Road.RoadData ConnectedRoad;//public Road.LogicalData ConnectedRoad;

				public ItemTypeEnum ItemType;
				public double TimeOnCurve;
				public bool ForwardDirection;

				public ConnectedRoadItem CrossingEndItem;

				//

				public enum ItemTypeEnum
				{
					StartPointInside,
					EndPointInside,
					CrossingStart,
					CrossingEnd,
				}

				public enum PositionOnEdge
				{
					Left,
					Center,
					Right,
				}

				public ConnectedRoadItem( CrossroadLogicalData owner )
				{
					Owner = owner;
				}

				public void GetSurfacePositionOnEdge( PositionOnEdge positionOnEdge, out Vector3 position, out Vector3 direction, out Vector3 up )
				{
					ConnectedRoad.GetPositionByTime( TimeOnCurve, out var centerPosition );
					ConnectedRoad.GetDirectionByTime( TimeOnCurve, out var directionForward );
					ConnectedRoad.GetUpByTime( TimeOnCurve, out up );

					direction = directionForward;
					if( !ForwardDirection )
						direction = -direction;

					var roadType = ConnectedRoad.RoadType;
					var laneWidth = roadType.LaneWidth.Value;
					var roadsideEdgeWidth = roadType.RoadsideEdgeWidth.Value;

					var lanes = ConnectedRoad.Owner.Lanes.Value;
					var width = lanes * laneWidth + roadsideEdgeWidth * 2;

					var rotation = Quaternion.LookAt( direction, up );

					var widthOffset = 0.0;
					if( positionOnEdge != PositionOnEdge.Center )
						widthOffset = ( positionOnEdge == PositionOnEdge.Left ? width : -width ) * 0.5;

					double heightOffset = 0;

					var offset = rotation * new Vector3( 0, widthOffset, heightOffset );
					position = centerPosition + offset;
				}
			}

			/////////////////////

			public CrossroadLogicalData( RoadNode owner )
				: base( owner )
			{
			}

			public new Crossroad Owner
			{
				get { return (Crossroad)base.Owner; }
			}

			public override void OnDelete()
			{
				if( ConnectedRoads.Count != 0 )
				{
					foreach( var roadItem in ConnectedRoads )
					{
						var road = roadItem.ConnectedRoad;

						road.Owner.NeedUpdateLogicalData();

						//road.ConnectedNodes.Remove( this );
					}
					ConnectedRoads.Clear();
				}

				//WalkingRoads.Clear();

				base.OnDelete();
			}

			public override void OnConnectedRoadDelete( Road.RoadData roadData )
			{
				base.OnConnectedRoadDelete( roadData );

//!!!!never used?

again:;
				for( int n = 0; n < ConnectedRoads.Count; n++ )
				{
					if( ConnectedRoads[ n ].ConnectedRoad == roadData )
					{
						ConnectedRoads.RemoveAt( n );

						//!!!!new
						Owner.NeedUpdate();

						goto again;
					}
				}
				//ConnectedRoads.Remove( roadData );
			}

			public override void GetModifiersForRoad( Road.RoadData roadData, List<Road.Modifier> modifiers )
			{
				base.GetModifiersForRoad( roadData, modifiers );

				foreach( var road in ConnectedRoads )
				{
					if( road.ConnectedRoad == roadData )
					{
						switch( road.ItemType )
						{
						case ConnectedRoadItem.ItemTypeEnum.StartPointInside:
							{
								var modifier = new Road.Modifier();
								modifier.MinTime = road.TimeOnCurve;
								modifiers.Add( modifier );
							}
							break;

						case ConnectedRoadItem.ItemTypeEnum.EndPointInside:
							{
								var modifier = new Road.Modifier();
								modifier.MaxTime = road.TimeOnCurve;
								modifiers.Add( modifier );
							}
							break;

						case ConnectedRoadItem.ItemTypeEnum.CrossingStart:
							{
								var modifier = new Road.Modifier();
								modifier.TimeClipRange = new Range( road.TimeOnCurve, road.CrossingEndItem.TimeOnCurve );
								modifiers.Add( modifier );
							}
							break;
						}
					}
				}
			}

			//////////////////////////////////////////////

			//public class MeshData
			//{
			//	public List<RoadUtility.RoadGeometryGenerator.MeshGeometryItem> MeshGeometries = new List<RoadUtility.RoadGeometryGenerator.MeshGeometryItem>();
			//}

			//////////////////////////////////////////////

			//public class SideCurve
			//{
			//	public List<Point> Points = new List<Point>( 256 );

			//	public struct Point
			//	{
			//		public Vector3 Position;
			//		public Quaternion Rotation;
			//	}
			//}

			//////////////////////////////////////////////

			public class RoadConnection
			{
				public ConnectedRoadItem Road;
				public ConnectedRoadItem NextRoad;

				public RoadUtility.RoadGeometryGenerator GeometryGenerator;

				public List<double> TimeSteps;
				public double TotalLength;

				//

				public List<double> GetCurveTimeSteps( Curve curvePosition, double stepMultiplier )//, Range? timeClipRange )
				{
					//var points = Points;

					var segmentsLength = Road.ConnectedRoad.RoadType.SegmentsLength.Value;
					if( segmentsLength <= 0.001 )
						segmentsLength = 0.001;

					double totalLengthNoCurvature = 0;
					{
						for( int n = 1; n < curvePosition.Points.Count; n++ )
							totalLengthNoCurvature += ( curvePosition.Points[ n ].Value - curvePosition.Points[ n - 1 ].Value ).Length();
						if( totalLengthNoCurvature <= 0.001 )
							totalLengthNoCurvature = 0.001;
					}

					//generate time steps
					List<double> timeSteps;
					{
						var stepDivide = 2.0;

						timeSteps = new List<double>( Math.Max( (int)( totalLengthNoCurvature / segmentsLength * stepDivide * 2 ), 4 ) );

						for( int nPoint = 0; nPoint < curvePosition.Points.Count - 1; nPoint++ )
						{
							var pointFrom = curvePosition.Points[ nPoint ];
							var pointTo = curvePosition.Points[ nPoint + 1 ];
							var timeFrom = pointFrom.Time;
							var timeTo = pointTo.Time;

							var length = ( pointTo.Value - pointFrom.Value ).Length();
							var timeStep = segmentsLength / length / stepDivide;
							timeStep *= stepMultiplier;

							var timeEnd = timeTo - timeStep * 0.1;
							for( double time = timeFrom; time < timeEnd; time += timeStep )
								timeSteps.Add( time );
						}

						timeSteps.Add( curvePosition.Points[ curvePosition.Points.Count - 1 ].Time );
					}

					return timeSteps;
				}
			}

			public RoadUtility.MeshData GenerateMeshData( bool generateCollision )
			{
				var meshData = new RoadUtility.MeshData();// generateCollision );

				var cylinder = Owner.GetCylinder();
				var bounds2 = cylinder.ToBounds().ToRectangle();
				var ownerPosition = Owner.TransformV.Position;
				//var center = cylinder.GetCenter();
				var radius = cylinder.Radius;


				//SimpleMeshGenerator.GenerateSegmentedPlane( 2, new Vector2( radius * 2.5, radius * 2.5 ), new Vector2I( 16, 16 ), Vector2.Zero, Vector2.Zero, Vector2.Zero, out var surfaceVertices, out _, out _, out _, out var surfaceIndices, out _ );

				//for( int n = 0; n < surfaceVertices.Length; n++ )
				//	surfaceVertices[ n ] = surfaceVertices[ n ] + center;


				var perimeter = new List<Vector3>( 1024 );

				var roadConnections = new RoadConnection[ SortedRoads.Count ];

				for( int nRoad = 0; nRoad < SortedRoads.Count; nRoad++ )
				{
					var road = SortedRoads[ nRoad ];
					var nextRoad = SortedRoads[ ( nRoad + 1 ) % SortedRoads.Count ];

					road.GetSurfacePositionOnEdge( ConnectedRoadItem.PositionOnEdge.Left, out var roadCornerPosition, out var roadCornerDirection, out var roadCornerUp );
					nextRoad.GetSurfacePositionOnEdge( ConnectedRoadItem.PositionOnEdge.Right, out var nextRoadCornerPosition, out var nextRoadCornerDirection, out var nextRoadCornerUp );

					var curvatureRadius = Owner.CurvatureRadius.Value;

					if( !MathAlgorithms.IntersectRayRay( roadCornerPosition.ToVector2(), roadCornerPosition.ToVector2() + roadCornerDirection.ToVector2(), nextRoadCornerPosition.ToVector2(), nextRoadCornerPosition.ToVector2() + nextRoadCornerDirection.ToVector2(), out var intersectionPoint2 ) )
					{
						intersectionPoint2 = ( roadCornerPosition.ToVector2() + nextRoadCornerPosition.ToVector2() ) * 0.5;
					}

					var distance = ( intersectionPoint2 - roadCornerPosition.ToVector2() ).Length();
					var intersectionPoint = new Vector3( intersectionPoint2, ( roadCornerPosition + roadCornerDirection * distance ).Z );

					var curvePosition = new CurveRoundedLine();
					curvePosition.AddPoint( new Curve.Point( 0, roadCornerPosition ) );
					//!!!!a bit bigger radius to fix gaps
					curvePosition.AddPoint( new Curve.Point( 0.5, intersectionPoint, curvatureRadius * 1.1 ) );
					//curvePosition.AddPoint( new Curve.Point( 0.5, intersectionPoint, curvatureRadius ) );
					curvePosition.AddPoint( new Curve.Point( 1, nextRoadCornerPosition ) );

					var curvePosition2 = new CurveRoundedLine();
					curvePosition2.AddPoint( new Curve.Point( 0, roadCornerPosition ) );
					curvePosition2.AddPoint( new Curve.Point( 0.5, intersectionPoint, curvatureRadius ) );
					curvePosition2.AddPoint( new Curve.Point( 1, nextRoadCornerPosition ) );

					//!!!!
					var curveUp = new CurveCubicSpline();
					curveUp.AddPoint( new Curve.Point( 0, roadCornerUp ) );
					//!!!!
					curveUp.AddPoint( new Curve.Point( 0.5, ( roadCornerUp + nextRoadCornerUp ) * 0.5 ) );
					//curveUp.AddPoint( new Curve.Point( 0.5, intersectionPoint, curvatureRadius ) );
					curveUp.AddPoint( new Curve.Point( 1, nextRoadCornerUp ) );


					//!!!!step. шаги нужны только на месте кривизны
					//!!!!lods
					var step = 1.0 / 100.0;


					for( var time = 0.0; time < 0.9999; time += step )
						perimeter.Add( curvePosition.CalculateValueByTime( time ) );
					perimeter.Add( nextRoadCornerPosition );

					var roadConnection = new RoadConnection();
					roadConnections[ nRoad ] = roadConnection;
					roadConnection.Road = road;
					roadConnection.NextRoad = nextRoad;

					var geometryGenerator = new RoadUtility.RoadGeometryGenerator();
					roadConnection.GeometryGenerator = geometryGenerator;
					geometryGenerator.Mode = RoadUtility.RoadGeometryGenerator.ModeEnum.Crossroad;
					geometryGenerator.RoadType = roadConnection.Road.ConnectedRoad.RoadType;
					geometryGenerator.PositionCurve = curvePosition2;
					geometryGenerator.UpCurve = curveUp;

					//!!!!
					roadConnection.TimeSteps = roadConnection.GetCurveTimeSteps( curvePosition, 0.25 );
					//roadConnection.TimeSteps = roadConnection.GetCurveTimeSteps( curvePosition, 1.0 );
					//roadConnection.TimeSteps = roadConnection.GetCurveTimeSteps( curvePosition, 4.0 );

					//calculate TotalLength
					{
						var previous = Vector3.Zero;
						for( int n = 0; n < roadConnection.TimeSteps.Count; n++ )
						{
							var time = roadConnection.TimeSteps[ n ];

							geometryGenerator.GetPositionByTime( time, out var v );
							if( n != 0 )
								roadConnection.TotalLength += ( v - previous ).Length();
							previous = v;
						}
					}
				}


				var perimeterAverage = Vector3.Zero;
				for( int n = 0; n < perimeter.Count; n++ )
					perimeterAverage += perimeter[ n ];
				perimeterAverage /= perimeter.Count;

				var surfaceVertices = new Vector3[ 1 + perimeter.Count ];
				surfaceVertices[ 0 ] = perimeterAverage;
				for( int n = 0; n < perimeter.Count; n++ )
					surfaceVertices[ 1 + n ] = perimeter[ n ];

				var surfaceIndices = new int[ perimeter.Count * 3 ];
				for( int n = 0; n < perimeter.Count; n++ )
				{
					surfaceIndices[ n * 3 + 0 ] = 0;
					surfaceIndices[ n * 3 + 1 ] = 1 + ( n + 1 ) % perimeter.Count;
					surfaceIndices[ n * 3 + 2 ] = 1 + n;
				}


				//var clipVertices = new Vector3[ perimeter.Count * 2 + 2 ];
				//var clipIndices = new List<int>( perimeter.Count * 12 );

				//var centerTop = center + new Vector3( 0, 0, 1000 );
				//var centerBottom = center - new Vector3( 0, 0, 1000 );

				//clipVertices[ 0 ] = centerTop;
				//clipVertices[ 1 ] = centerBottom;
				//for( int n = 0; n < perimeter.Count; n++ )
				//{
				//	var p = perimeter[ n ];
				//	clipVertices[ 2 + n ] = p + new Vector3( 0, 0, 1000 );
				//	clipVertices[ 2 + perimeter.Count + n ] = p - new Vector3( 0, 0, 1000 );
				//}

				////!!!!
				////MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles

				//for( int n = 0; n < perimeter.Count; n++ )
				//{
				//	clipIndices.Add( 0 );
				//	clipIndices.Add( 2 + ( n + 1 ) % perimeter.Count );
				//	clipIndices.Add( 2 + n );

				//	clipIndices.Add( 1 );
				//	clipIndices.Add( 2 + perimeter.Count + n );
				//	clipIndices.Add( 2 + perimeter.Count + ( n + 1 ) % perimeter.Count );

				//	clipIndices.Add( 2 + n );
				//	clipIndices.Add( 2 + perimeter.Count + ( n + 1 ) % perimeter.Count );
				//	clipIndices.Add( 2 + perimeter.Count + n );

				//	clipIndices.Add( 2 + perimeter.Count + ( n + 1 ) % perimeter.Count );
				//	clipIndices.Add( 2 + n );
				//	clipIndices.Add( 2 + ( n + 1 ) % perimeter.Count );
				//}


				//var operand1 = new Internal.Net3dBool.Solid( RoadUtility.Convert( surfaceVertices ), surfaceIndices );
				//var operand2 = new Internal.Net3dBool.Solid( RoadUtility.Convert( clipVertices ), clipIndices.ToArray() );
				//var modeller = new Internal.Net3dBool.BooleanModeller( operand1, operand2 );

				//var booleanResult = modeller.GetIntersection();

				////replace with new arrays
				//surfaceVertices = RoadUtility.Convert( booleanResult.getVertices() );
				//surfaceIndices = booleanResult.getIndices();


				////calculate position Z
				//unsafe
				//{
				//	var cornerLines = new Line3[ SortedRoads.Count ];
				//	for( int nRoad = 0; nRoad < SortedRoads.Count; nRoad++ )
				//	{
				//		var road = SortedRoads[ nRoad ];
				//		road.GetSurfacePositionOnEdge( ConnectedRoadItem.PositionOnEdge.Left, out var pos1, out _ );
				//		road.GetSurfacePositionOnEdge( ConnectedRoadItem.PositionOnEdge.Right, out var pos2, out _ );
				//		cornerLines[ nRoad ] = new Line3( pos1, pos2 );
				//	}


				//	//!!!!slowly. threading


				//	for( int nVertex = 0; nVertex < surfaceVertices.Length; nVertex++ )
				//	{
				//		var v = surfaceVertices[ nVertex ];

				//		var distances = stackalloc double[ cornerLines.Length ];
				//		var positions = stackalloc Vector3[ cornerLines.Length ];
				//		var totalDistance = 0.0;

				//		for( int n = 0; n < cornerLines.Length; n++ )
				//		{
				//			ref var line = ref cornerLines[ n ];

				//			var projected = MathAlgorithms.ProjectPointToLine( line.Start, line.End, v );

				//			var b = new Bounds( line.Start );
				//			b.Add( line.End );
				//			if( b.Contains( projected ) )
				//			{
				//				var distance = ( projected - v ).Length();
				//				distances[ n ] = distance;
				//				positions[ n ] = projected;
				//				totalDistance += distance;
				//			}
				//		}

				//		if( totalDistance == 0 )
				//			totalDistance = 0.0001;

				//		//calculate factors
				//		var factors = stackalloc double[ cornerLines.Length ];
				//		for( int n = 0; n < cornerLines.Length; n++ )
				//			factors[ n ] = 1.0 - distances[ n ] / totalDistance;

				//		//normalize factors
				//		var totalFactor = 0.0;
				//		for( int n = 0; n < cornerLines.Length; n++ )
				//			totalFactor += factors[ n ];
				//		if( totalFactor == 0 )
				//			totalFactor = 0.0001;
				//		for( int n = 0; n < cornerLines.Length; n++ )
				//			factors[ n ] = factors[ n ] / totalFactor;

				//		//calculate position z
				//		var newPositionZ = 0.0;
				//		for( int n = 0; n < cornerLines.Length; n++ )
				//			newPositionZ += positions[ n ].Z * factors[ n ];
				//		v.Z = newPositionZ;

				//		surfaceVertices[ nVertex ] = v;
				//	}
				//}

				//surface
				if( surfaceIndices.Length != 0 )
				{
					var firstRoad = SortedRoads[ 0 ].ConnectedRoad;
					var firstRoadType = firstRoad.RoadType;

					var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

					double uvTilesLength = 0;
					//if( generatePart == GeneratePartEnum.Surface )
					uvTilesLength = firstRoadType.UVTilesLength.Value;
					//else if( generatePart == GeneratePartEnum.Markup )
					//	uvTilesLength = markupSolid ? 0.0 : 1.0;
					//else if( generatePart == GeneratePartEnum.RoadsideLeft || generatePart == GeneratePartEnum.RoadsideRight )
					//	uvTilesLength = roadType.RoadsideUVTilesLength.Value;
					//else //if( generatePart == GeneratePartEnum.Overpass )
					//	uvTilesLength = roadType.OverpassUVTilesLength.Value;


					//!!!!
					//calculate normals
					var normalItems = new CalculateNormalItem[ surfaceVertices.Length ];
					{
						for( int nTriangle = 0; nTriangle < surfaceIndices.Length / 3; nTriangle++ )
						{
							var index0 = surfaceIndices[ nTriangle * 3 + 0 ];
							var index1 = surfaceIndices[ nTriangle * 3 + 1 ];
							var index2 = surfaceIndices[ nTriangle * 3 + 2 ];

							var v0 = surfaceVertices[ index0 ];
							var v1 = surfaceVertices[ index1 ];
							var v2 = surfaceVertices[ index2 ];

							var normal = MathAlgorithms.CalculateTriangleNormal( v0, v1, v2 ).ToVector3F();

							ref var item0 = ref normalItems[ index0 ];
							item0.Sum += normal;
							item0.Count++;

							ref var item1 = ref normalItems[ index1 ];
							item1.Sum += normal;
							item1.Count++;

							ref var item2 = ref normalItems[ index2 ];
							item2.Sum += normal;
							item2.Count++;
						}
					}

					var normals = new Vector3F[ surfaceVertices.Length ];
					for( int n = 0; n < surfaceVertices.Length; n++ )
					{
						ref var item = ref normalItems[ n ];
						normals[ n ] = ( item.Sum / item.Count ).GetNormalize();
					}

					var vertices = new byte[ vertexSize * surfaceVertices.Length ];
					var positions = new Vector3F[ surfaceVertices.Length ];
					unsafe
					{
						fixed( byte* pVertices = vertices )
						{
							var pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;

							for( int n = 0; n < surfaceVertices.Length; n++ )
							{
								pVertex->Position = ( surfaceVertices[ n ] - ownerPosition ).ToVector3F();
								positions[ n ] = pVertex->Position;

								pVertex->Normal = normals[ n ];
								//!!!!right?
								pVertex->Tangent = new Vector4F( QuaternionF.LookAt( new Vector3F( 1, 0, 0 ), pVertex->Normal ).GetForward(), -1 );
								pVertex->Color = new ColorValue( 1, 1, 1 );
								pVertex->TexCoord0 = pVertex->Position.ToVector2() * (float)uvTilesLength;

								pVertex++;
							}
						}
					}

					//add to result
					{
						var meshGeometryItem = new RoadUtility.RoadGeometryGenerator.MeshGeometryItem();
						meshGeometryItem.Part = RoadUtility.RoadGeometryGenerator.GeneratePartEnum.Surface;
						meshGeometryItem.Vertices = vertices;
						meshGeometryItem.Positions = positions;
						meshGeometryItem.Indices = surfaceIndices;
						meshGeometryItem.Material = firstRoadType.SurfaceMaterial;
						meshData.MeshGeometries.Add( meshGeometryItem );
					}
				}

				//generate shoulder, sidewalk, etc
				for( int nRoad = 0; nRoad < SortedRoads.Count; nRoad++ )
				{
					var roadConnection = roadConnections[ nRoad ];
					var road = roadConnection.Road;
					var nextRoad = roadConnection.NextRoad;

					if( road.ConnectedRoad.RoadType == nextRoad.ConnectedRoad.RoadType )
					{
						//markup
						var markupOffset = road.ConnectedRoad.RoadType.RoadsideEdgeWidth.Value;
						roadConnection.GeometryGenerator.GenerateRoadMeshData( ownerPosition, roadConnection.TimeSteps, roadConnection.TotalLength, RoadUtility.RoadGeometryGenerator.GeneratePartEnum.Markup, meshData.MeshGeometries, -markupOffset, true, true );

						//Shoulder
						if( road.ConnectedRoad.PredefinedModifiers.HasFlag( RoadModifier.PredefinedModifiersEnum.ShoulderLeft ) && nextRoad.ConnectedRoad.PredefinedModifiers.HasFlag( RoadModifier.PredefinedModifiersEnum.ShoulderRight ) )
						{
							roadConnection.GeometryGenerator.GenerateRoadMeshData( ownerPosition, roadConnection.TimeSteps, roadConnection.TotalLength, RoadUtility.RoadGeometryGenerator.GeneratePartEnum.ShoulderLeft, meshData.MeshGeometries );
						}

						//Sidewalk
						if( road.ConnectedRoad.PredefinedModifiers.HasFlag( RoadModifier.PredefinedModifiersEnum.SidewalkLeft ) && nextRoad.ConnectedRoad.PredefinedModifiers.HasFlag( RoadModifier.PredefinedModifiersEnum.SidewalkRight ) )
						{
							for( int n = 0; n < 5; n++ )
							{
								roadConnection.GeometryGenerator.GenerateRoadMeshData( ownerPosition, roadConnection.TimeSteps, roadConnection.TotalLength, RoadUtility.RoadGeometryGenerator.GeneratePartEnum.SidewalkLeft1 + n, meshData.MeshGeometries );
							}
						}
					}
				}

				//generate crossing markings
				if( Owner.CrossingMarkings && !generateCollision )
				{
					//!!!!потом перенести в RoadUtility потому что не только для перекрестков

					//!!!!каждой дороге своя разметка
					RoadType firstRoadType = null;
					if( SortedRoads.Count != 0 )
					{
						var road = SortedRoads[ 0 ];
						firstRoadType = road.ConnectedRoad.RoadType;
					}

					var geometryWithSurface = meshData.MeshGeometries.FirstOrDefault( g => g.Part == RoadUtility.RoadGeometryGenerator.GeneratePartEnum.Surface );

					if( geometryWithSurface != null )
					{
						//make octree for ray case from Surface mesh data
						using( var meshTest = new MeshTest( geometryWithSurface.Positions, geometryWithSurface.Indices ) )
						{
							var walkingVertices = new List<(Vector3, Vector3F, Vector2F)>( 128 );
							var walkingIndices = new List<int>( 128 );

							//get connected walking roads
							{
								var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, cylinder.ToBounds() );
								Scene.GetObjectsInSpace( getObjectsItem );

								foreach( var item in getObjectsItem.Result ) //foreach( var walkingRoad in WalkingRoads )
								{
									var walkingRoad = item.Object as Road;
									if( walkingRoad != null )
									{
										var walkingRoadData = walkingRoad.GetRoadData();
										if( walkingRoadData != null && walkingRoadData.RoadType.WayToUse.Value == RoadType.WayToUseEnum.Walking )
										{
											var timeSteps = walkingRoadData.GetCurveTimeSteps( 2, null );

											var started = false;
											var borderPositions1 = new List<(Vector3, Vector3F, Vector2F)>( 32 );
											var borderPositions2 = new List<(Vector3, Vector3F, Vector2F)>( 32 );
											var firstP1 = Vector3.Zero;

											var previousPos = Vector3.Zero;

											for( int nTimeStep = 0; nTimeStep < timeSteps.Count; nTimeStep++ )
											{
												var time = timeSteps[ nTimeStep ];
												walkingRoadData.GetPositionByTime( time, out var centerPos );

												if( nTimeStep != 0 )
												{
													var laneWidth = walkingRoadData.RoadType.LaneWidth.Value;
													var sidewalkEdgeWidth = walkingRoadData.RoadType.RoadsideEdgeWidth.Value;

													var lanes = walkingRoadData.Owner.Lanes.Value;
													var width = lanes * laneWidth + sidewalkEdgeWidth * 2;
													var widthOffsetSide0 = width * 0.5;
													var widthOffsetSide1 = -width * 0.5;

													var collided = false;

													if( ( ownerPosition - centerPos ).LengthSquared() < radius * radius )
													{
														var dir = ( centerPos - previousPos ).GetNormalize();
														var rot = Quaternion.LookAt( dir, Vector3.ZAxis );

														var pos1 = centerPos + rot * new Vector3( 0, widthOffsetSide0, 0 );
														var pos2 = centerPos + rot * new Vector3( 0, widthOffsetSide1, 0 );

														var localPos1 = ( pos1 - ownerPosition ).ToVector3F();
														var localRay1 = new RayF( localPos1 + new Vector3F( 0, 0, 100 ), new Vector3F( 0, 0, -200 ) );
														var rayResults1 = meshTest.RayCast( localRay1, MeshTest.Mode.OneClosest, false );

														var localPos2 = ( pos2 - ownerPosition ).ToVector3F();
														var localRay2 = new RayF( localPos2 + new Vector3F( 0, 0, 100 ), new Vector3F( 0, 0, -200 ) );
														var rayResults2 = meshTest.RayCast( localRay2, MeshTest.Mode.OneClosest, false );

														if( rayResults1.Length != 0 && rayResults2.Length != 0 )
														{
															var rayResult1 = rayResults1[ 0 ];
															var rayResult2 = rayResults2[ 0 ];

															var p1 = localRay1.GetPointOnRay( rayResult1.Scale ).ToVector3() + ownerPosition;
															var normal1 = rayResult1.Normal;

															var p2 = localRay2.GetPointOnRay( rayResult2.Scale ).ToVector3() + ownerPosition;
															var normal2 = rayResult2.Normal;

															if( !started )
																firstP1 = p1;

															var distance = 0.0;
															if( p1 != firstP1 )
																distance = ( p1 - firstP1 ).Length();

															//!!!!
															var uvFactor = 5.0f;
															var uvOffsetAlong = (float)( distance * uvFactor );

															var texCoord1 = new Vector2F( 0, uvOffsetAlong );

															var texCoord2 = new Vector2F( (float)( p2 - p1 ).Length() * uvFactor, uvOffsetAlong );

															borderPositions1.Add( (p1, normal1, texCoord1) );
															borderPositions2.Add( (p2, normal2, texCoord2) );

															collided = true;
														}
													}

													if( collided )
													{
														if( !started )
															started = true;
													}
													else
													{
														if( started )
														{
															//var startOffset = 0;
															//var count = borderPositions1.Count;

															var clampCount = 1;
															var startOffset = clampCount;
															var count = borderPositions1.Count - clampCount * 2 - clampCount;

															//clamp partial pieces
															var lastTexCoord = borderPositions1[ startOffset + count - 1 ].Item3;
															if( ( lastTexCoord.Y % 1.0f ) < 0.5f )
																count--;

															if( count >= 2 )
															{
																var currentVertexIndex = walkingVertices.Count;

																for( int n = 0; n < count; n++ )
																{
																	walkingVertices.Add( borderPositions1[ startOffset + n ] );
																	walkingVertices.Add( borderPositions2[ startOffset + n ] );

																	if( n > 0 )
																	{
																		walkingIndices.Add( currentVertexIndex + n * 2 + 1 );
																		walkingIndices.Add( currentVertexIndex + n * 2 + 0 );
																		walkingIndices.Add( currentVertexIndex + ( n - 1 ) * 2 + 1 );

																		walkingIndices.Add( currentVertexIndex + ( n - 1 ) * 2 + 0 );
																		walkingIndices.Add( currentVertexIndex + ( n - 1 ) * 2 + 1 );
																		walkingIndices.Add( currentVertexIndex + n * 2 + 0 );
																	}
																}
															}
														}

														started = false;
														borderPositions1.Clear();
														borderPositions2.Clear();
													}
												}

												previousPos = centerPos;
											}
										}
									}
								}
							}


							if( walkingIndices.Count > 0 )
							{
								var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

								var vertices = new byte[ vertexSize * walkingVertices.Count ];
								var positions = new Vector3F[ walkingVertices.Count ];

								unsafe
								{
									fixed( byte* pVertices = vertices )
									{
										var pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;

										for( int n = 0; n < walkingVertices.Count; n++ )
										{
											var walkingVertex = walkingVertices[ n ];
											var pos = walkingVertex.Item1;
											var normal = walkingVertex.Item2;
											var texCoord = walkingVertex.Item3;

											pVertex->Position = ( pos - ownerPosition ).ToVector3F();
											positions[ n ] = pVertex->Position;

											pVertex->Normal = normal;

											//!!!!right?
											pVertex->Tangent = new Vector4F( QuaternionF.LookAt( new Vector3F( 1, 0, 0 ), pVertex->Normal ).GetForward(), -1 );

											pVertex->Color = new ColorValue( 1, 1, 1 );
											pVertex->TexCoord0 = texCoord;

											pVertex++;
										}
									}
								}

								//add to result
								{
									var meshGeometryItem = new RoadUtility.RoadGeometryGenerator.MeshGeometryItem();
									meshGeometryItem.Part = RoadUtility.RoadGeometryGenerator.GeneratePartEnum.Markup;
									meshGeometryItem.Vertices = vertices;
									meshGeometryItem.Positions = positions;
									meshGeometryItem.Indices = walkingIndices.ToArray();
									meshGeometryItem.Material = firstRoadType.CrossingMarkingsMaterial;
									meshData.MeshGeometries.Add( meshGeometryItem );
								}

							}

						}
					}
				}

				return meshData;
			}
		}

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( TrafficLightsLocalOffset ):
					//case nameof( TrafficLightsOverrideDistance ):
					if( !TrafficLights )
						skip = true;
					break;
				}
			}
		}

		bool IsInsideCylinder( Vector3 position )
		{
			var cylinder = GetCylinder();
			var tr = Transform.Value;
			return ( position.ToVector2() - tr.Position.ToVector2() ).Length() <= cylinder.Radius && position.Z >= cylinder.Point1.Z && position.Z <= cylinder.Point2.Z;
		}

		protected override LogicalData OnCalculateLogicalData()
		{
			var logicalData = new CrossroadLogicalData( this );

			var cylinder = GetCylinder();
			var bounds2 = cylinder.ToBounds().ToRectangle();
			var center = cylinder.GetCenter();
			var radius = cylinder.Radius;

			//get connected roads
			{
				var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, cylinder.ToBounds() );
				logicalData.Scene.GetObjectsInSpace( getObjectsItem );

				foreach( var item in getObjectsItem.Result )
				{
					var road = item.Object as Road;
					if( road != null )
					{
						var roadData = road.GetRoadData();
						//var roadData = road.GetLogicalData();

						if( roadData != null && roadData.RoadType.WayToUse.Value == WayToUse.Value )//&& data.GetCurveTimeByPosition( pointPosition, maxDistanceToCurve, out var timeOnCurve ) )
						{
							//double timeStep;
							//{
							//	double totalLengthNoCurvature = 0;
							//	{
							//		for( int n = 1; n < roadData.Points.Length; n++ )
							//			totalLengthNoCurvature += ( roadData.Points[ n ].Transform.Position - roadData.Points[ n - 1 ].Transform.Position ).Length();
							//		if( totalLengthNoCurvature <= 0.001 )
							//			totalLengthNoCurvature = 0.001;
							//	}

							//	var totalTime = roadData.LastPoint.TimeOnCurve;

							//	//!!!!
							//	var stepLength = 0.5;

							//	//!!!!? totalTime
							//	timeStep = stepLength / totalLengthNoCurvature * totalTime;
							//}

							var startPosition = roadData.Points[ 0 ].Transform.Position;
							var endPosition = roadData.LastPoint.Transform.Position;

							var startInside = IsInsideCylinder( startPosition );
							var endInside = IsInsideCylinder( endPosition );

							if( startInside || endInside )
							{
								if( startInside != endInside )
								{
									var roadItem = new CrossroadLogicalData.ConnectedRoadItem( logicalData );
									roadItem.ConnectedRoad = roadData;

									//calculate time with radius distance to center of crossroad

									if( startInside )
									{
										var dir = ( startPosition - center ).GetNormalize();
										var splitPosition = center + dir * cylinder.Radius;

										roadData.GetClosestCurveTimeToPosition( splitPosition, radius, 0.5, out var timeOnCurve );
										//roadData.GetClosestCurveTimeToPosition( splitPosition, timeStep, out var timeOnCurve );

										roadItem.ItemType = CrossroadLogicalData.ConnectedRoadItem.ItemTypeEnum.StartPointInside;
										roadItem.TimeOnCurve = timeOnCurve;
										roadItem.ForwardDirection = false;
									}
									else
									{
										var dir = ( endPosition - center ).GetNormalize();
										var splitPosition = center + dir * cylinder.Radius;

										roadData.GetClosestCurveTimeToPosition( splitPosition, radius, 0.5, out var timeOnCurve );
										//roadData.GetClosestCurveTimeToPosition( splitPosition, timeStep, out var timeOnCurve );

										roadItem.ItemType = CrossroadLogicalData.ConnectedRoadItem.ItemTypeEnum.EndPointInside;
										roadItem.TimeOnCurve = timeOnCurve;
										roadItem.ForwardDirection = true;
									}

									logicalData.ConnectedRoads.Add( roadItem );
									//!!!!было
									//roadData.ConnectedNodes.AddWithCheckAlreadyContained( logicalData );


									////!!!!угол или линия

									//var positionOnCurve = roadData.GetPositionByTime( timeOnCurve );

									////add to this road
									//{
									//	var connectedRoadItem = new ConnectedRoadItem();
									//	connectedRoadItem.ConnectedRoad = roadData;

									//	connectedRoadItem.ThisRoad_PointIndex = nPoint;
									//	connectedRoadItem.ThisRoad_TimeOnCurve = point.TimeOnCurve;
									//	connectedRoadItem.ThisRoad_Position = pointPosition;

									//	connectedRoadItem.ConnectedRoad_TimeOnCurve = timeOnCurve;
									//	connectedRoadItem.ConnectedRoad_Position = positionOnCurve;

									//	AddConnectedRoad( roadData, connectedRoadItem );
									//}

									////add to connected road
									//{
									//	var connectedRoadItem = new ConnectedRoadItem();
									//	connectedRoadItem.ConnectedRoad = this;

									//	connectedRoadItem.ThisRoad_TimeOnCurve = timeOnCurve;
									//	connectedRoadItem.ThisRoad_Position = positionOnCurve;

									//	connectedRoadItem.ConnectedRoad_PointIndex = nPoint;
									//	connectedRoadItem.ConnectedRoad_TimeOnCurve = point.TimeOnCurve;
									//	connectedRoadItem.ConnectedRoad_Position = pointPosition;

									//	roadData.AddConnectedRoad( this, connectedRoadItem );
									//}


									//!!!!? где еще так
									//need update visual data of the connected road
									roadData.Owner.DeleteVisualData();
								}
							}
							else
							{
								//check for crossing road

								roadData.GetClosestCurveTimeToPosition( center, radius, 0.5, out var centerTimeOnCurve );
								//roadData.GetClosestCurveTimeToPosition( center, timeStep, out var centerTimeOnCurve );
								roadData.GetPositionByTime( centerTimeOnCurve, out var positionOnCurve );

								var diff = positionOnCurve - center;
								if( diff.Length() < radius && positionOnCurve.Z > cylinder.Point1.Z && positionOnCurve.Z < cylinder.Point2.Z )
								{
									roadData.GetDirectionByTime( centerTimeOnCurve, out var dir );


									CrossroadLogicalData.ConnectedRoadItem roadItemStart;
									CrossroadLogicalData.ConnectedRoadItem roadItemEnd;

									{
										var roadItem = new CrossroadLogicalData.ConnectedRoadItem( logicalData );
										roadItemStart = roadItem;
										roadItem.ConnectedRoad = roadData;

										var splitPosition = center - dir * cylinder.Radius;
										roadData.GetClosestCurveTimeToPosition( splitPosition, radius, 0.5, out var timeOnCurve );
										//roadData.GetClosestCurveTimeToPosition( splitPosition, timeStep, out var timeOnCurve );

										roadItem.ItemType = CrossroadLogicalData.ConnectedRoadItem.ItemTypeEnum.CrossingStart;
										roadItem.TimeOnCurve = timeOnCurve;
										roadItem.ForwardDirection = true;

										logicalData.ConnectedRoads.Add( roadItem );
										//!!!!было
										//roadData.ConnectedNodes.AddWithCheckAlreadyContained( logicalData );
									}

									{
										var roadItem = new CrossroadLogicalData.ConnectedRoadItem( logicalData );
										roadItemEnd = roadItem;
										roadItem.ConnectedRoad = roadData;

										var splitPosition = center + dir * cylinder.Radius;
										roadData.GetClosestCurveTimeToPosition( splitPosition, radius, 0.5, out var timeOnCurve );
										//roadData.GetClosestCurveTimeToPosition( splitPosition, timeStep, out var timeOnCurve );

										roadItem.ItemType = CrossroadLogicalData.ConnectedRoadItem.ItemTypeEnum.CrossingEnd;
										roadItem.TimeOnCurve = timeOnCurve;
										roadItem.ForwardDirection = false;

										logicalData.ConnectedRoads.Add( roadItem );
										//!!!!было
										//roadData.ConnectedNodes.AddWithCheckAlreadyContained( logicalData );
									}

									roadItemStart.CrossingEndItem = roadItemEnd;

									//!!!!? где еще так
									//need update visual data of the connected road
									roadData.Owner.DeleteVisualData();
								}
							}
						}

						//if( roadData != null && roadData.RoadType.WayToUse.Value == RoadType.WayToUseEnum.Walking )
						//{
						//	logicalData.WalkingRoads.Add( roadData );
						//}

					}
				}
			}

			//calculate road inputs

			logicalData.SortedRoads = new List<CrossroadLogicalData.ConnectedRoadItem>();
			foreach( var road in logicalData.ConnectedRoads )
				logicalData.SortedRoads.Add( road );//.Value );

			CollectionUtility.SelectionSort( logicalData.SortedRoads, delegate ( CrossroadLogicalData.ConnectedRoadItem item1, CrossroadLogicalData.ConnectedRoadItem item2 )
			{
				item1.GetSurfacePositionOnEdge( CrossroadLogicalData.ConnectedRoadItem.PositionOnEdge.Center, out var pos1, out _, out _ );
				var p1 = pos1 - center;
				var angle1 = Math.Atan2( p1.Y, p1.X );

				item2.GetSurfacePositionOnEdge( CrossroadLogicalData.ConnectedRoadItem.PositionOnEdge.Center, out var pos2, out _, out _ );
				var p2 = pos2 - center;
				var angle2 = Math.Atan2( p2.Y, p2.X );

				if( angle1 < angle2 )
					return 1;
				if( angle1 > angle2 )
					return -1;
				return 0;
			} );

			if( logicalData.SortedRoads.Count > 1 )
				logicalData.Valid = true;

			//need update connected roads
			if( logicalData.Valid )
			{
				foreach( var road in logicalData.ConnectedRoads )
					road.ConnectedRoad.Owner.NeedUpdateLogicalData();
			}

			return logicalData;
		}

		public Cylinder GetCylinder()
		{
			var tr = Transform.Value;
			var v = new Vector3( 0, 0, tr.Scale.Z * 0.5 );
			return new Cylinder( tr.Position - v, tr.Position + v, Math.Max( tr.Scale.X, tr.Scale.Y ) * 0.5 );
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			newBounds = new SpaceBounds( GetCylinder().ToBounds() );
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				//!!!!slowly? where else. #if !DEPLOY
				bool show = ( context.SceneDisplayDevelopmentDataInThisApplication && ParentScene.DisplayVolumes );// || context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
				if( EngineApp.IsEditor )
					if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this )
						show = true;

				if( EngineApp.IsEditor && DisplayLogicalDataInEditor )
					show = true;
				if( EngineApp.IsSimulation && DisplayLogicalDataInSimulation )
					show = true;

				if( show )
				{
					//!!!!фрустум порверять

					var logicalData = (CrossroadLogicalData)GetLogicalData( false );
					if( logicalData != null )
					{
						ColorValue color;
						if( context2.selectedObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.SelectedColor;
						else if( context2.canSelectObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.CanSelectColor;
						else
							color = ProjectSettings.Get.Colors.SceneShowVolumeColor;

						if( color.Alpha != 0 )
						{
							var renderer = context2.viewport.Simple3DRenderer;

							renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							renderer.AddCylinder( GetCylinder(), 64 );

							foreach( var road in logicalData.SortedRoads )
							{
								road.ConnectedRoad.GetPositionByTime( road.TimeOnCurve, out var pos );
								renderer.AddSphere( new Sphere( pos, 0.2 ), 32, true );

								road.ConnectedRoad.GetDirectionByTime( road.TimeOnCurve, out var dir );
								if( !road.ForwardDirection )
									dir = -dir;

								renderer.AddArrow( pos, pos + dir * 4 );

								{
									road.GetSurfacePositionOnEdge( CrossroadLogicalData.ConnectedRoadItem.PositionOnEdge.Left, out var left, out _, out _ );
									road.GetSurfacePositionOnEdge( CrossroadLogicalData.ConnectedRoadItem.PositionOnEdge.Right, out var right, out _, out _ );
									renderer.AddLineThin( left, right );
								}
							}
						}
					}
				}
			}

			//foreach( var road in logicalData.ConnectedRoads )
			//{
			//	renderer.SetColor( new ColorValue( 1, 1, 0, 0.8 ) );
			//	renderer.AddLine( TransformV.Position, road.Value.ConnectedRoad.Owner.TransformV.Position );
			//}
		}

		struct CalculateNormalItem
		{
			public Vector3F Sum;
			public int Count;
		}

		protected override void OnCalculateVisualData( RoadUtility.VisualData visualData )
		{
			base.OnCalculateVisualData( visualData );

			var logicalData = (CrossroadLogicalData)GetLogicalData();
			if( !logicalData.Valid )
				return;
			if( !logicalData.SortedRoads[ 0 ].ConnectedRoad.RoadType.SurfaceMaterial.ReferenceOrValueSpecified )
				return;


			var meshData = logicalData.GenerateMeshData( false );

			var tr = new Transform( TransformV.Position );
			visualData.CreateObjectsFromMeshData( meshData, tr, ColorMultiplier );

			var firstRoad = logicalData.SortedRoads[ 0 ].ConnectedRoad;

			//imperfections
			if( firstRoad.Owner.Age > 0 )
			{
				var surface = firstRoad.RoadType.AgeImperfection.Value;
				if( surface != null && surface.Result != null )
				{
					foreach( var geometry in meshData.MeshGeometries )
					{
						if( geometry.Part == RoadUtility.RoadGeometryGenerator.GeneratePartEnum.Surface )
						{
							var items = RoadUtility.CalculateSurfaceObjects( surface.Result, TransformV.Position, firstRoad.Owner.Age, geometry );
							if( items != null && items.Length != 0 )
								visualData.CreateGroupOfObjectsSubGroup( surface.Result, items );
						}
					}

					//var items = RoadUtility.CalculateSurfaceObjects( surface.Result, meshData, TransformV.Position, firstRoad.Owner.Age );
					//if( items != null && items.Length != 0 )
					//	visualData.CreateGroupOfObjectsSubGroup( surface.Result, items );
				}
			}

			//traffic lights
			if( TrafficLights )
			{
				var baseMesh = firstRoad.RoadType.TrafficLightBaseMesh.Value;
				var baseMeshWalkingOnly = firstRoad.RoadType.TrafficLightBaseMeshWalkingOnly.Value ?? baseMesh;
				if( baseMesh != null )
				{
					//!!!!левостороннее движение


					for( int nRoad = 0; nRoad < logicalData.SortedRoads.Count; nRoad++ )
					{
						var road = logicalData.SortedRoads[ nRoad ];

						//var nextRoad = logicalData.SortedRoads[ ( nRoad + 1 ) % logicalData.SortedRoads.Count ];
						//var previousRoad = logicalData.SortedRoads[ ( nRoad + logicalData.SortedRoads.Count - 1 ) % logicalData.SortedRoads.Count ];

						road.GetSurfacePositionOnEdge( CrossroadLogicalData.ConnectedRoadItem.PositionOnEdge.Right, out var roadRightCornerPosition, out var roadRightCornerDirection, out var roadRightCornerUp );

						road.GetSurfacePositionOnEdge( CrossroadLogicalData.ConnectedRoadItem.PositionOnEdge.Left, out var roadLeftCornerPosition, out var roadLeftCornerDirection, out var roadLeftCornerUp );

						//road.GetSurfacePositionOnEdge( CrossroadLogicalData.ConnectedRoadItem.PositionOnEdge.Left, out var roadCornerPosition, out var roadCornerDirection, out var roadCornerUp );

						//nextRoad.GetSurfacePositionOnEdge( CrossroadLogicalData.ConnectedRoadItem.PositionOnEdge.Right, out var nextRoadCornerPosition, out var nextRoadCornerDirection, out var nextRoadCornerUp );

						////!!!!left?
						//previousRoad.GetSurfacePositionOnEdge( CrossroadLogicalData.ConnectedRoadItem.PositionOnEdge.Right, out var previousRoadCornerPosition, out var previousRoadCornerDirection, out var previousRoadCornerUp );

						//if( !MathAlgorithms.IntersectRayRay( roadCornerPosition.ToVector2(), roadCornerPosition.ToVector2() + roadCornerDirection.ToVector2(), nextRoadCornerPosition.ToVector2(), nextRoadCornerPosition.ToVector2() + nextRoadCornerDirection.ToVector2(), out var intersectionPoint2 ) )
						//{
						//	intersectionPoint2 = ( roadCornerPosition.ToVector2() + nextRoadCornerPosition.ToVector2() ) * 0.5;
						//}

						//var distance = ( intersectionPoint2 - roadCornerPosition.ToVector2() ).Length();
						//var intersectionPoint = new Vector3( intersectionPoint2, ( roadCornerPosition + roadCornerDirection * distance ).Z );


						var alongRoadDirection = roadRightCornerDirection;// nextRoadCornerDirection;
						var alongRoadRotation = Quaternion.FromDirectionZAxisUp( alongRoadDirection );

						//right side. driving and walking
						{

							//calculate world position of the object
							var pos = roadRightCornerPosition;//nextRoadCornerPosition;

							//var overrideDistance = TrafficLightsOverrideDistance.Value;
							//if( overrideDistance != 0 )
							//{
							//	var ray = new Ray2( nextRoadCornerDirection.ToVector2(), intersectionPoint.ToVector2() );
							//	var circle = new Circle( tr.Position.ToVector2(), overrideDistance );

							//	if( circle.Intersects( ray, out var scale1, out var scale2 ) )
							//	{
							//		var scale = Math.Min( scale1, scale2 );
							//		var intersection = ray.GetPointOnRay( scale );

							//		//!!!!pos.Z
							//		pos = new Vector3( intersection, pos.Z );
							//	}
							//}

							//add local offset
							pos += alongRoadRotation * TrafficLightsLocalOffset.Value;

							//!!!!
							//var verticalOffset = height on sidewalk;

							var tr2 = new Transform( pos, Quaternion.Identity, Vector3.One );

							//!!!!collision: it is not visual data
							//!!!!collision: about road manager update


							var additionalItems = new List<MeshInSpace.AdditionalItem>();
							{
								//driving
								var drivingMesh = firstRoad.RoadType.TrafficLightDrivingMesh.Value;
								if( drivingMesh != null )
								{
									var rot = Quaternion.LookAt( -roadRightCornerDirection, Vector3.ZAxis );
									var item = new MeshInSpace.AdditionalItem( drivingMesh, Vector3.Zero, rot, Vector3.One, ColorValue.One );
									additionalItems.Add( item );
								}

								//walking
								var walkingMesh = firstRoad.RoadType.TrafficLightWalkingMesh.Value;
								if( walkingMesh != null )
								{
									var dir = roadLeftCornerPosition - roadRightCornerPosition;
									var rot = Quaternion.LookAt( dir, Vector3.ZAxis );
									var item = new MeshInSpace.AdditionalItem( walkingMesh, Vector3.Zero, rot, Vector3.One, ColorValue.One );
									additionalItems.Add( item );
								}
							}

							visualData.CreateMeshObject( baseMesh, tr2, ColorValue.One, additionalItems.ToArray(), Collision );
						}

						//left side. walking only
						{
							//calculate world position of the object
							var pos = roadLeftCornerPosition;

							//add local offset
							pos += alongRoadRotation * ( TrafficLightsLocalOffset.Value * new Vector3( 1, -1, 1 ) );

							var tr2 = new Transform( pos, Quaternion.Identity, Vector3.One );

							var additionalItems = new List<MeshInSpace.AdditionalItem>();
							{
								//walking
								var walkingMesh = firstRoad.RoadType.TrafficLightWalkingMesh.Value;
								if( walkingMesh != null )
								{
									var dir = roadRightCornerPosition - roadLeftCornerPosition;
									var rot = Quaternion.LookAt( dir, Vector3.ZAxis );
									var item = new MeshInSpace.AdditionalItem( walkingMesh, Vector3.Zero, rot, Vector3.One, ColorValue.One );
									additionalItems.Add( item );

									//{
									//	var rot = Quaternion.LookAt( -roadLeftCornerDirection, Vector3.ZAxis );
									//	var item = new MeshInSpace.AdditionalItem( walkingMesh, Vector3.Zero, rot, Vector3.One, ColorValue.One );
									//	additionalItems.Add( item );
									//}
								}
							}

							visualData.CreateMeshObject( baseMeshWalkingOnly, tr2, ColorValue.One, additionalItems.ToArray(), Collision );
						}
					}

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



			//foreach( var item in meshData.MeshObjects )
			//	visualData.CreateMeshObject( item.mesh, item.transform, item.useGroupOfObjects, null, ColorMultiplier );

			//foreach( var m in meshData.MeshesToDispose )
			//	visualData.MeshesToDispose.Add( m );



			//for( int nTriangle = 0; nTriangle < surfaceIndices2.Length / 3; nTriangle++ )
			//{
			//	ref var v0 = ref surfaceVertices2[ surfaceIndices2[ nTriangle * 3 + 0 ] ];
			//	ref var v1 = ref surfaceVertices2[ surfaceIndices2[ nTriangle * 3 + 1 ] ];
			//	ref var v2 = ref surfaceVertices2[ surfaceIndices2[ nTriangle * 3 + 2 ] ];

			//	visualData.DebugLines.Add( new Line3( v0, v1 ) );
			//	visualData.DebugLines.Add( new Line3( v1, v2 ) );
			//	visualData.DebugLines.Add( new Line3( v2, v0 ) );
			//}



			//!!!!может для отладки показывать как road border

			//for( int n = 0; n < perimeter.Count; n++ )
			//{
			//	var v1 = perimeter[ n ];
			//	var v2 = perimeter[ ( n + 1 ) % perimeter.Count ];
			//	visualData.DebugLines.Add( new Line3( v1, v2 ) );
			//}



			//for( int nRoad = 0; nRoad < logicalData.SortedRoads.Count; nRoad++ )
			//{
			//	var road = logicalData.SortedRoads[ nRoad ];

			//	var p = road.ConnectedRoad.GetPositionByTime( road.TimeOnCurve );

			//	var r = 0.1;
			//	visualData.DebugLines.Add( new Line3( p - new Vector3( r, 0, 0 ), p + new Vector3( r, 0, 0 ) ) );
			//	visualData.DebugLines.Add( new Line3( p - new Vector3( 0, r, 0 ), p + new Vector3( 0, r, 0 ) ) );
			//	visualData.DebugLines.Add( new Line3( p - new Vector3( 0, 0, r ), p + new Vector3( 0, 0, r ) ) );
			//}


			//var roadItem = logicalData.SortedRoads[ 0 ];
			//var roadData = roadItem.ConnectedRoad;

			//var line = new Line3( TransformV.Position, TransformV.Position + new Vector3( 10, 0, 0 ) );
			//visualData.DebugLines.Add( line );

		}

		protected override void OnCalculateCollisionBodies()
		{
			base.OnCalculateCollisionBodies();

			var logicalData = (CrossroadLogicalData)GetLogicalData();
			if( !logicalData.Valid )
				return;
			if( !logicalData.SortedRoads[ 0 ].ConnectedRoad.RoadType.SurfaceMaterial.ReferenceOrValueSpecified )
				return;

			//!!!!
			//Log.Info( "OnCalculateCollisionBodies: " + Name );


			//!!!!пока всё сразу. меши эстакад и другие меши юзать "Collision Definition"


			var meshData = logicalData.GenerateMeshData( true );
			var ownerPosition = TransformV.Position;

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

			//foreach( var item in meshData.MeshObjects )
			//{
			//	var meshResult = item.mesh.Result;
			//	if( meshResult != null )
			//	{
			//		var startVertex = vertices.Count;

			//		foreach( var vertex in meshResult.ExtractedVerticesPositions )
			//			vertices.Add( ( item.transform * vertex - ownerPosition ).ToVector3F() );

			//		foreach( var index in meshResult.ExtractedIndices )
			//			indices.Add( index + startVertex );
			//	}

			//	//CreateMeshObject( item.mesh, item.transform, item.useGroupOfObjects );
			//}


			//create rigid body
			if( vertices.Count != 0 && indices.Count != 0 )
			{
				var shapeData = new RigidBody();
				//shapeData.Transform = new Transform( ownerPosition );

				var meshShape = shapeData.CreateComponent<CollisionShape_Mesh>();
				meshShape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.TriangleMesh;
				meshShape.CheckValidData = false;
				//!!!!?
				meshShape.MergeEqualVerticesRemoveInvalidTriangles = false;

				meshShape.Vertices = vertices.ToArray();
				meshShape.Indices = indices.ToArray();

				{
					var firstRoad = logicalData.SortedRoads[ 0 ].ConnectedRoad;
					var firstRoadType = firstRoad.RoadType;

					shapeData.Material = firstRoadType.SurfaceCollisionMaterial;
					//!!!!
					//body.MaterialFrictionMode = firstRoadType.SurfaceCollisionFrictionMode;
					shapeData.MaterialFriction = firstRoadType.SurfaceCollisionFriction;
					//!!!!
					//body.MaterialAnisotropicFriction = firstRoadType.SurfaceCollisionAnisotropicFriction;
					//body.MaterialSpinningFriction = firstRoadType.SurfaceCollisionSpinningFriction;
					//body.MaterialRollingFriction = firstRoadType.SurfaceCollisionRollingFriction;
					shapeData.MaterialRestitution = firstRoadType.SurfaceCollisionRestitution;
				}
				//UpdateCollisionMaterial();

				var shapeItem = logicalData.Scene.PhysicsWorld.AllocateShape( shapeData, Vector3.One );
				if( shapeItem != null )
				{
					var bodyItem = logicalData.Scene.PhysicsWorld.CreateRigidBodyStatic( shapeItem, true, this, ownerPosition, QuaternionF.Identity );
					if( bodyItem != null )
						logicalData.CollisionBodies.Add( bodyItem );
				}


				////need set ShowInEditor = false before AddComponent
				//var body = ComponentUtility.CreateComponent<RigidBody>( null, false, false );
				//body.DisplayInEditor = false;
				//AddComponent( body, -1 );
				////var group = scene.CreateComponent<GroupOfObjects>();

				////CollisionBody.Name = "__Collision Body";
				//body.SaveSupport = false;
				//body.CloneSupport = false;
				//body.CanBeSelected = false;
				////!!!!?
				////body.SpaceBoundsUpdateEvent += CollisionBody_SpaceBoundsUpdateEvent;

				//body.Transform = new Transform( ownerPosition );

				//var shape = body.CreateComponent<CollisionShape_Mesh>();
				//shape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.TriangleMesh;
				//shape.CheckValidData = false;
				////!!!!?
				//shape.MergeEqualVerticesRemoveInvalidTriangles = false;

				//shape.Vertices = vertices.ToArray();
				//shape.Indices = indices.ToArray();

				//{
				//	var firstRoad = logicalData.SortedRoads[ 0 ].ConnectedRoad;
				//	var firstRoadType = firstRoad.RoadType;

				//	body.Material = firstRoadType.SurfaceCollisionMaterial;
				//	//!!!!
				//	//body.MaterialFrictionMode = firstRoadType.SurfaceCollisionFrictionMode;
				//	body.MaterialFriction = firstRoadType.SurfaceCollisionFriction;
				//	//!!!!
				//	//body.MaterialAnisotropicFriction = firstRoadType.SurfaceCollisionAnisotropicFriction;
				//	//body.MaterialSpinningFriction = firstRoadType.SurfaceCollisionSpinningFriction;
				//	//body.MaterialRollingFriction = firstRoadType.SurfaceCollisionRollingFriction;
				//	body.MaterialRestitution = firstRoadType.SurfaceCollisionRestitution;
				//}
				////UpdateCollisionMaterial();

				//body.Enabled = true;

				//logicalData.CollisionBodies.Add( body );
			}

			//foreach( var m in meshData.MeshesToDispose )
			//	m.Dispose();
		}

		//void UpdateCollisionMaterial()
		//{
		//	if( CollisionBody != null )
		//	{
		//		CollisionBody.Material = PipeType.CollisionMaterial;
		//		CollisionBody.MaterialFrictionMode = PipeType.CollisionFrictionMode;
		//		CollisionBody.MaterialFriction = PipeType.CollisionFriction;
		//		CollisionBody.MaterialAnisotropicFriction = PipeType.CollisionAnisotropicFriction;
		//		CollisionBody.MaterialSpinningFriction = PipeType.CollisionSpinningFriction;
		//		CollisionBody.MaterialRollingFriction = PipeType.CollisionRollingFriction;
		//		CollisionBody.MaterialRestitution = PipeType.CollisionRestitution;
		//	}
		//}

		//private void CollisionBody_SpaceBoundsUpdateEvent( ObjectInSpace obj, ref SpaceBounds newBounds )
		//{
		//	newBounds = Owner.SpaceBounds;
		//}

	}
}
