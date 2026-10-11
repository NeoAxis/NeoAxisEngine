// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a test mode in the editor for traffic systems component.
	/// </summary>
	public class TrafficSystemTestMode : SceneEditorWorkareaMode
	{
		TrafficSystem owner;

		bool pathTest;
		GetPositionByCursorResult startPosition;
		GetPositionByCursorResult endPosition;
		TrafficSystem.FindPathContext pathContext;
		//TrafficSystem.FindPathContext.PathItem[] path;
		string error;
		bool found;
		double time;
		double timeLastUpdateTime;

		/////////////////////////////////////////

		public TrafficSystemTestMode( ISceneEditor documentWindow, TrafficSystem owner )
			: base( documentWindow )
		{
			this.owner = owner;
		}

		protected override bool OnKeyDown( Viewport viewport, KeyEvent e )
		{
			if( e.Key == EKeys.Escape || e.Key == EKeys.Space )
			{
				DocumentWindow.ResetWorkareaMode();
				return true;
			}

			return base.OnKeyDown( viewport, e );
		}

		protected override bool OnMouseDown( Viewport viewport, EMouseButtons button )
		{
			if( button == EMouseButtons.Left )
			{
				startPosition = GetPositionByCursor( viewport );
				if( startPosition != null )
					pathTest = true;
				return true;
			}

			return false;
		}

		protected override bool OnMouseUp( Viewport viewport, EMouseButtons button )
		{
			if( button == EMouseButtons.Left && pathTest )
			{
				pathTest = false;
				return true;
			}

			return false;
		}

		class GetPositionByCursorResult
		{
			public Vector3 Position;
			public Road Road;
			public int Lane;
			public double TimeOnCurve;
		}

		GetPositionByCursorResult GetPositionByCursor( Viewport viewport )
		{
			var context = new SceneEditorGetMouseOverObjectToSelectByClickContext();
			context.CheckOnlyObjectsWithEnabledSelectionByCursorFlag = false;

			DocumentWindow.GetMouseOverObjectToSelectByClick( context );

			var ray = viewport.CameraSettings.GetRayByScreenCoordinates( viewport.MousePosition );

			var resultItems = owner.GetRoadsByRay( ray );
			if( resultItems.Length > 0 )
			{
				var resultItem0 = resultItems[ 0 ];

				var result = new GetPositionByCursorResult();
				result.Road = resultItem0.Road;
				result.Position = ray.GetPointOnRay( resultItem0.Scale );
				result.Lane = resultItem0.Lane;
				result.TimeOnCurve = resultItem0.TimeOnCurve;

				return result;
			}

			return null;
		}

		protected override void OnTick( Viewport viewport, double delta )
		{
			if( pathTest )
			{
				found = false;
				pathContext = null;
				error = null;

				endPosition = GetPositionByCursor( viewport );
				if( startPosition != null && endPosition != null )
				{
					var startTime = EngineApp.GetSystemTime();

					var context = new TrafficSystem.FindPathContext();
					context.StartPosition = startPosition.Position;
					context.StartRoad = startPosition.Road;
					context.StartLane = startPosition.Lane;
					context.StartTimeOnCurve = startPosition.TimeOnCurve;
					context.EndPosition = endPosition.Position;
					context.EndRoad = endPosition.Road;
					context.EndLane = endPosition.Lane;
					context.EndTimeOnCurve = endPosition.TimeOnCurve;

					owner.FindPath( context, true );

					found = context.Path != null;
					pathContext = context;
					//path = context.Path;
					error = context.Error;

					var endTime = EngineApp.GetSystemTime();

					if( ( EngineApp.GetSystemTime() - timeLastUpdateTime ) > 0.25 )
					{
						timeLastUpdateTime = EngineApp.GetSystemTime();
						time = endTime - startTime;
					}
				}
			}
		}

		bool IsPartialFound()
		{
			//right now no partial path support

			////if( found && ( path[ path.Length - 1 ].Position - endPosition ).Length() > 1f )
			////	return true;

			return false;
		}

		void HighlightLane( Viewport viewport, Road road, int lane )
		{
			var renderer = viewport.Simple3DRenderer;

			var roadData = road.GetRoadData();
			if( roadData != null )
			{
				var laneCurves = roadData.GetLaneCurves();
				if( lane >= 0 && lane < laneCurves.Length )
				{
					var laneCurve = laneCurves[ lane ];
					var forward = roadData.IsForwardLane( lane );

					var arrowCounter = 0;
					Vector3? previousPos = null;
					for( int n = 0; n < laneCurve.Curve.Length; n++ )
					{
						var pos = laneCurve.Curve[ n ].Position;

						if( previousPos.HasValue )
						{
							if( arrowCounter % 10 == 0 )
							{
								if( forward )
									renderer.AddArrow( previousPos.Value, pos, 0.5, 0.1, true, -1 );
								else
									renderer.AddArrow( pos, previousPos.Value, 0.5, 0.1, true, -1 );
							}
							else
								renderer.AddLine( previousPos.Value, pos, -1 );
						}

						previousPos = pos;
						arrowCounter++;
					}
				}
			}
		}

		protected override void OnUpdateBeforeOutput( Viewport viewport )
		{
			var renderer = viewport.Simple3DRenderer;

			var offset = new Vector3( 0, 0, 0.1 );
			var offset2 = new Vector3( 0, 0, 0.1 );

			if( pathTest )
			{
				if( startPosition != null )
				{
					renderer.SetColor( ProjectSettings.Get.Colors.CanSelectColor, false );

					//highlight cursor position
					renderer.AddArrow( startPosition.Position + new Vector3( 0, 0, 2 ), startPosition.Position + offset, 0.6, 0.2, true, .07 );

					//highlight lane
					if( !found )
						HighlightLane( viewport, startPosition.Road, startPosition.Lane );
				}

				//show end position and arrow between start and end
				if( endPosition != null )
				{
					if( found )
						renderer.SetColor( new ColorValue( 0, 1, 0 ), false );
					else
						renderer.SetColor( new ColorValue( 1, 0, 0 ), false );

					//highlight cursor position
					renderer.AddArrow( endPosition.Position + new Vector3( 0, 0, 2 ), endPosition.Position + offset, 0.6, 0.2, true, .07 );

					//highlight lane
					if( !found )
						HighlightLane( viewport, endPosition.Road, endPosition.Lane );
				}

				//show path
				if( found && startPosition != null && endPosition != null && pathContext != null )
				{
					const double thickness = 0.1;

					renderer.SetColor( IsPartialFound() ? new ColorValue( 1, 0, 0 ) : new ColorValue( 0, 1, 0 ), false );

					var positions = owner.FindPathCalculatePositions( pathContext, 4, 1 );
					for( int n = 0; n < positions.Count - 1; n++ )
					{
						renderer.AddLine( positions[ n ], positions[ n + 1 ], thickness );
						renderer.AddLine( positions[ n ], positions[ n ] + new Vector3( 0, 0, 1 ), thickness );
					}
				}
			}
			else
			{
				//show start position and arrow between start and end
				var cursorPosition = GetPositionByCursor( viewport );
				if( cursorPosition != null )
				{
					renderer.SetColor( ProjectSettings.Get.Colors.CanSelectColor, false );

					//highlight cursor position
					renderer.AddArrow( cursorPosition.Position + new Vector3( 0, 0, 2 ), cursorPosition.Position + offset, 0.6, 0.2, true, .07 );

					//highlight lane
					HighlightLane( viewport, cursorPosition.Road, cursorPosition.Lane );
				}
			}
		}

		string Translate( string text )
		{
			return text;
			//return ToolsLocalization.Translate( "TrafficSystemTestMode", text );
		}

		protected override void OnGetTextInfoCenterBottomCorner( List<string> lines )
		{
			base.OnGetTextInfoCenterBottomCorner( lines );

			if( pathTest )
			{
				//we check if the path will lead us close enough to where we wanted
				if( found )
				{
					if( IsPartialFound() )
						lines.Add( Translate( "The path was found, but did not reach the endpoint." ) );
					else
						lines.Add( Translate( "The path was found." ) );
				}
				else
				{
					lines.Add( Translate( "The path was not found." ) );
					if( !string.IsNullOrEmpty( error ) )
						lines.Add( "Error: " + error );
				}

				lines.Add( "" );
				lines.Add( string.Format( Translate( "Search time: {0} seconds." ), time.ToString( "F8" ) ) );
				if( found )
					lines.Add( string.Format( Translate( "Points in the path: {0}." ), pathContext.Path.Length ) );
			}
			else
			{
				lines.Add( "Pathfinding Test Mode" );
				lines.Add( "" );
				lines.Add( Translate( "Specify the start and end points by clicking and holding the mouse button." ) );
			}
		}
	}
}
