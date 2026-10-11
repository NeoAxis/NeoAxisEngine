// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a test mode in the editor for pathfinding component.
	/// </summary>
	public class PathfindingTestMode : SceneEditorWorkareaMode
	{
		Pathfinding owner;

		bool pathTest;
		Vector3 startPosition;
		Vector3 endPosition;
		Pathfinding.FindPathContext.PathPoint[] path;
		string error;
		bool found;
		double time;
		double timeLastUpdateTime;

		/////////////////////////////////////////

		public PathfindingTestMode( ISceneEditor documentWindow, Pathfinding owner )
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
				if( GetPositionByCursor( viewport, out startPosition ) )
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

		bool GetPositionByCursor( Viewport viewport, out Vector3 pos )
		{

			//can find by nav mesh, but need prepare octree. but then can't select when cursor little bit outside geometry

			var context = new SceneEditorGetMouseOverObjectToSelectByClickContext();
			context.CheckOnlyObjectsWithEnabledSelectionByCursorFlag = false;

			DocumentWindow.GetMouseOverObjectToSelectByClick( context );

			var found = false;
			var foundPosition = Vector3.Zero;

			var resultObjectComponent = context.ResultObject as Component;
			if( resultObjectComponent != null )
			{
				//PathfindingGeometryTag
				{
					var geometryTag = resultObjectComponent.GetComponent<PathfindingGeometryTag>( false, true );
					//Terrain
					if( geometryTag == null && resultObjectComponent.Parent?.Parent != null )
						geometryTag = resultObjectComponent.Parent.Parent.GetComponent<PathfindingGeometryTag>( false, true );

					if( geometryTag != null )
					{
						pos = context.ResultPosition.HasValue ? context.ResultPosition.Value : Vector3.Zero;

						found = true;
						foundPosition = pos;

						//return true;
					}
				}

				////PathfindingGeometry
				//{
				//	var geometry = resultObjectComponent as PathfindingGeometry;
				//	if( geometry != null && !geometry.Dynamic && geometry.Walkable )
				//	{
				//		geometry.GetGeometry( out var vertices, out var indices );

				//		var ray = viewport.CameraSettings.GetRayByScreenCoordinates( viewport.MousePosition );

				//		var found = false;
				//		var nearestDistanceScale = 0.0;

				//		for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
				//		{
				//			var index0 = indices[ nTriangle * 3 + 0 ];
				//			var index1 = indices[ nTriangle * 3 + 1 ];
				//			var index2 = indices[ nTriangle * 3 + 2 ];

				//			var vertex0 = vertices[ index0 ];
				//			var vertex1 = vertices[ index1 ];
				//			var vertex2 = vertices[ index2 ];

				//			if( MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex1, ref vertex2, ref ray, out var scale ) )
				//			{
				//				if( !found || scale < nearestDistanceScale )
				//				{
				//					found = true;
				//					nearestDistanceScale = scale;
				//				}
				//			}
				//		}

				//		if( found )
				//		{
				//			pos = ray.GetPointOnRay( nearestDistanceScale );
				//			return true;
				//		}
				//	}
				//}
			}

			//find PathfindingGeometry
			{
				var ray = viewport.CameraSettings.GetRayByScreenCoordinates( viewport.MousePosition );

				var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, MetadataManager.GetTypeOfNetType( typeof( PathfindingGeometry ) ), true, ray );
				DocumentWindow.Scene.GetObjectsInSpace( item );

				foreach( var resultItem in item.Result )
				{
					var geometry = resultItem.Object as PathfindingGeometry;
					if( geometry != null && !geometry.Dynamic && geometry.Walkable )
					{
						geometry.GetGeometry( out var vertices, out var indices );

						//var found = false;
						//var nearestDistanceScale = 0.0;

						for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
						{
							var index0 = indices[ nTriangle * 3 + 0 ];
							var index1 = indices[ nTriangle * 3 + 1 ];
							var index2 = indices[ nTriangle * 3 + 2 ];

							var vertex0 = vertices[ index0 ];
							var vertex1 = vertices[ index1 ];
							var vertex2 = vertices[ index2 ];

							if( MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex1, ref vertex2, ref ray, out var scale ) )
							{
								var pos2 = ray.GetPointOnRay( scale );

								if( !found || ( pos2 - ray.Origin ).Length() < ( foundPosition - ray.Origin ).Length() )
								{
									found = true;
									foundPosition = pos2;
								}

								//if( !found || scale < nearestDistanceScale )
								//{
								//	found = true;
								//	nearestDistanceScale = scale;
								//}
							}
						}

						//if( found )
						//{
						//	pos = ray.GetPointOnRay( nearestDistanceScale );
						//	return true;
						//}
					}
				}
			}

			if( found )
			{
				pos = foundPosition;
				return true;
			}

			pos = Vector3.Zero;
			return false;
		}

		protected override void OnTick( Viewport viewport, double delta )
		{
			if( pathTest )
			{
				found = false;
				path = null;
				error = null;

				if( GetPositionByCursor( viewport, out endPosition ) )
				{
					var startTime = EngineApp.GetSystemTime();

					var context = new Pathfinding.FindPathContext();
					context.Start = startPosition;
					context.End = endPosition;

					owner.FindPath( context, true );

					found = context.Path != null;
					path = context.Path;
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
			if( found && ( path[ path.Length - 1 ].Position - endPosition ).Length() > 1f )
				return true;
			return false;
		}

		protected override void OnUpdateBeforeOutput( Viewport viewport )
		{
			var renderer = viewport.Simple3DRenderer;

			Vector3 offset = new Vector3( 0, 0, 0.1 );
			Vector3 offset2 = new Vector3( 0, 0, 0.1 );

			if( pathTest )
			{
				if( found )
				{
					renderer.SetColor( IsPartialFound() ? new ColorValue( 1, 0, 0 ) : new ColorValue( 0, 1, 0 ) );
					for( int n = 0; n < path.Length - 1; n++ )
					{
						Vector3 point1 = path[ n ].Position + offset;
						Vector3 point2 = path[ n + 1 ].Position + offset;
						renderer.AddLine( point1, point2, .07 );
					}

					renderer.SetColor( IsPartialFound() ? new ColorValue( 1, 0, 0 ) : new ColorValue( 1, 1, 0 ) );
					for( int n = 0; n < path.Length; n++ )
					{
						ref var p = ref path[ n ];
						Vector3 point = p.Position + offset;
						var turn = p.Turn;

						if( turn )
							renderer.AddSphere( new Sphere( point, .15 ), 16, true );
						else
							renderer.AddLine( point, point + offset2, 0.04 );
					}
				}

				renderer.SetColor( new ColorValue( 0, 0, 1 ) );
				renderer.AddArrow( startPosition + new Vector3( 0, 0, 2 ), startPosition + offset, 0.6, 0.2, true, .07 );

				//show end position and arrow between start and end
				if( GetPositionByCursor( viewport, out endPosition ) )
				{
					renderer.SetColor( new ColorValue( 1, 0, 0 ) );
					renderer.AddArrow( endPosition + new Vector3( 0, 0, 2 ), endPosition + offset, 0.6, 0.2, true, .07 );

					if( !found )
					{
						renderer.SetColor( new ColorValue( 1, 0, 0 ) );
						renderer.AddArrow( startPosition + offset, endPosition + offset, 0.6, 0.2, true, .07 );
					}
				}
			}
			else
			{
				//show end position and arrow between start and end
				if( GetPositionByCursor( viewport, out var pos ) )
				{
					renderer.SetColor( new ColorValue( 1, 0, 0 ) );
					renderer.AddArrow( pos + new Vector3( 0, 0, 2 ), pos + offset, 0.6, 0.2, true, .07 );
				}
			}
		}

		string Translate( string text )
		{
			return text;
			//return ToolsLocalization.Translate( "RecastTestArea", text );
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
					lines.Add( string.Format( Translate( "Points in the path: {0}." ), path.Length ) );
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
