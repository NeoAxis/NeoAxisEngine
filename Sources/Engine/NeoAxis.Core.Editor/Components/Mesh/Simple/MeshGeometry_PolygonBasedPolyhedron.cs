// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	/// <summary>
	/// A class for providing the creation of a <see cref="MeshGeometry_PolygonBasedPolyhedron"/> in the editor.
	/// </summary>
	public class MeshGeometry_PolygonBasedPolyhedronCreationMode : ObjectCreationMode
	{
		Rectangle? lastStartPointRectangle;

		bool heightStage;
		Vector3 heightStagePosition;

		//

		public MeshGeometry_PolygonBasedPolyhedronCreationMode( DocumentWindowWithViewport documentWindow, Component creatingObject )
			: base( documentWindow, creatingObject )
		{
			var position = CreatingObject.TransformV.Position;
			if( CalculatePointPosition( documentWindow.Viewport, out var position2, out _ ) )
				position = position2;

			var point = MeshGeometry.CreateComponent<MeshGeometry_PolygonBasedPolyhedron_Point>( enabled: false );
			point.Name = MeshGeometry.Components.GetUniqueName( "Point", false, 1 );
			point.Transform = new Transform( position, Quaternion.Identity );
			point.Enabled = true;
		}

		public MeshGeometry_PolygonBasedPolyhedron MeshGeometry
		{
			get
			{
				if( CreatingObject != null )
				{
					var mesh = CreatingObject.GetComponent<Mesh>();
					if( mesh != null )
						return mesh.GetComponent<MeshGeometry_PolygonBasedPolyhedron>();
				}
				return null;
			}
		}

		public new ObjectInSpace CreatingObject
		{
			get { return (ObjectInSpace)base.CreatingObject; }
		}

		protected virtual bool CalculatePointPosition( Viewport viewport, out Vector3 position, out ObjectInSpace collidedWith )
		{
			if( !viewport.MouseRelativeMode )
			{
				var sceneDocumentWindow = DocumentWindow as SceneEditor;
				if( sceneDocumentWindow != null )
				{
					var result = sceneDocumentWindow.CalculateCreateObjectPositionUnderCursor( viewport );
					if( result.found )
					{
						position = result.position;
						collidedWith = result.collidedWith;
						return true;
					}
				}
			}

			position = Vector3.Zero;
			collidedWith = null;
			return false;
		}

		void HeightStageStart( Viewport viewport )
		{
			heightStage = true;
			if( CalculatePointPosition( viewport, out var position, out _ ) )
				heightStagePosition = position;
		}

		protected override bool OnMouseDown( Viewport viewport, EMouseButtons button )
		{
			if( button == EMouseButtons.Left )
			{
				bool overStartPoint = !heightStage && lastStartPointRectangle.HasValue && lastStartPointRectangle.Value.Contains( viewport.MousePosition );

				if( heightStage )
				{
					Finish( false );
					return true;
				}
				else if( overStartPoint )
				{
					HeightStageStart( viewport );
					return true;
				}
				else
				{
					var points = MeshGeometry.GetPoints();

					if( !viewport.MouseRelativeMode )
					{
						if( points.Length >= 3 )
						{
							var plane = MeshGeometry.GetPolygonPlaneByPoints();
							var ray = viewport.CameraSettings.GetRayByScreenCoordinates( viewport.MousePosition );

							if( plane.Intersects( ray, out double scale ) )
							{
								var position = ray.GetPointOnRay( scale );

								var point = MeshGeometry.CreateComponent<MeshGeometry_PolygonBasedPolyhedron_Point>( enabled: false );
								point.Name = MeshGeometry.Components.GetUniqueName( "Point", false, 1 );
								point.Transform = new Transform( position, Quaternion.Identity );
								point.Enabled = true;

								return true;
							}
						}
						else
						{
							if( CalculatePointPosition( viewport, out var position, out var collidedWith ) )
							{
								var point = MeshGeometry.CreateComponent<MeshGeometry_PolygonBasedPolyhedron_Point>( enabled: false );
								point.Name = MeshGeometry.Components.GetUniqueName( "Point", false, 1 );
								point.Transform = new Transform( position, Quaternion.Identity );
								point.Enabled = true;

								//detect Clockwise
								var points2 = MeshGeometry.GetPointPositions();
								if( points2.Length == 3 )
								{
									var normal = Plane.FromPoints( points2[ 0 ], points2[ 1 ], points2[ 2 ] ).Normal;

									var d1 = ( points2[ 0 ] - viewport.CameraSettings.Position ).Length();
									var d2 = ( ( points2[ 0 ] + normal ) - viewport.CameraSettings.Position ).Length();

									if( d1 < d2 )
										MeshGeometry.Clockwise = true;
								}

								return true;
							}
						}
					}
				}
			}

			return false;
		}

		protected override void OnMouseMove( Viewport viewport, Vector2 mouse )
		{
			base.OnMouseMove( viewport, mouse );

			if( heightStage )
			{
				var ray = viewport.CameraSettings.GetRayByScreenCoordinates( mouse );

				var distance = ( MathAlgorithms.ProjectPointToLine( ray.Origin, ray.GetEndPoint(), heightStagePosition ) - heightStagePosition ).Length();

				MeshGeometry.Height = distance;
			}
		}

		protected override bool OnKeyDown( Viewport viewport, KeyEvent e )
		{
			if( e.Key == EKeys.Space || e.Key == EKeys.Return )
			{
				if( !heightStage )
					HeightStageStart( viewport );
				else
					Finish( false );
				return true;
			}

			if( e.Key == EKeys.Escape )
			{
				Finish( true );
				return true;
			}

			return false;
		}

		protected override void OnUpdateBeforeOutput( Viewport viewport )
		{
			base.OnUpdateBeforeOutput( viewport );

			lastStartPointRectangle = null;

			var points = MeshGeometry.GetPointPositions();
			if( !heightStage && points.Length > 2 )
			{
				if( viewport.CameraSettings.ProjectToScreenCoordinates( points[ 0 ], out var screenPosition ) )
				{
					var pos = points[ 0 ];

					Vector2 maxSize = new Vector2( 20, 20 );
					Vector2 minSize = new Vector2( 5, 5 );
					double maxDistance = 100;

					double distance = ( pos - viewport.CameraSettings.Position ).Length();
					if( distance < maxDistance )
					{
						Vector2 sizeInPixels = Vector2.Lerp( maxSize, minSize, distance / maxDistance );
						Vector2 screenSize = sizeInPixels / viewport.SizeInPixels.ToVector2();
						screenSize *= 1.5;

						var rect = new Rectangle( screenPosition - screenSize * .5, screenPosition + screenSize * .5 );

						ColorValue color;
						if( !viewport.MouseRelativeMode && rect.Contains( viewport.MousePosition ) )
							color = new ColorValue( 1, 1, 0, 0.5 );
						else
							color = new ColorValue( 1, 1, 1, 0.3 );

						viewport.CanvasRenderer.AddQuad( rect, color );

						lastStartPointRectangle = rect;
					}
				}
			}
		}

		protected override void OnGetTextInfoCenterBottomCorner( List<string> lines )
		{
			base.OnGetTextInfoCenterBottomCorner( lines );

			if( heightStage )
			{
				lines.Add( "Now specify the height of the object." );
				lines.Add( "Press Space, Return or click mouse button to finish creation." );
			}
			else
			{
				lines.Add( "Specify points of the object by clicking." );
				lines.Add( "Press Space or Return to finish creation of the points." );
			}
		}

		public override void Finish( bool cancel )
		{
			if( !cancel )
			{
				//calculate mesh in space position
				var points = MeshGeometry.GetPointPositions();
				if( points.Length != 0 )
				{
					var position = Vector3.Zero;
					foreach( var point in points )
						position += point;
					position /= points.Length;
					CreatingObject.Transform = new Transform( position, Quaternion.Identity );
				}

				//attach points to the mesh in space
				foreach( var point in MeshGeometry.GetPoints() )
					ObjectInSpaceUtility.Attach( CreatingObject, point, TransformOffset.ModeEnum.Elements );

				//select meshin space and points
				var toSelect = new List<Component>();
				toSelect.Add( CreatingObject );
				//toSelect.AddRange( points );
				EditorAPI2.SelectComponentsInMainObjectsWindow( DocumentWindow, toSelect.ToArray() );

				//update mesh
				MeshGeometry?.ShouldRecompileMesh();
			}

			base.Finish( cancel );
		}
	}
}
#endif
