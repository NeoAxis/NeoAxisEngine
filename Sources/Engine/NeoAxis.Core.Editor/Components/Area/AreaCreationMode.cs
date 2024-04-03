// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	/// <summary>
	/// A class for providing the creation of a <see cref="Area"/> in the editor.
	/// </summary>
	public class AreaCreationMode : ObjectCreationMode
	{
		Rectangle? lastStartPointRectangle;

		//

		public AreaCreationMode( DocumentWindowWithViewport documentWindow, Component creatingObject )
			: base( documentWindow, creatingObject )
		{
			var point = CreatingObject.CreateComponent<AreaPoint>( enabled: false );
			point.Name = CreatingObject.Components.GetUniqueName( "Point", false, 1 );
			point.Transform = new Transform( CreatingObject.TransformV.Position, Quaternion.Identity );
			point.Enabled = true;
		}

		public new Area CreatingObject
		{
			get { return (Area)base.CreatingObject; }
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

		protected override bool OnMouseDown( Viewport viewport, EMouseButtons button )
		{
			if( button == EMouseButtons.Left )
			{
				bool overStartPoint = lastStartPointRectangle.HasValue && lastStartPointRectangle.Value.Contains( viewport.MousePosition );

				if( overStartPoint )
				{
					Finish( false );
					return true;
				}
				else
				{
					if( CalculatePointPosition( viewport, out var position, out var collidedWith ) )
					{
						var point = CreatingObject.CreateComponent<AreaPoint>( enabled: false );
						point.Name = CreatingObject.Components.GetUniqueName( "Point", false, 1 );
						point.Transform = new Transform( position, Quaternion.Identity );
						point.Enabled = true;

						return true;
					}
				}
			}

			return false;
		}

		protected override bool OnKeyDown( Viewport viewport, KeyEvent e )
		{
			if( e.Key == EKeys.Space || e.Key == EKeys.Return )
			{
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

			var points = CreatingObject.GetPointPositions();
			if( points.Length > 2 )
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

			lines.Add( "Specify points of the area by clicking." );
			lines.Add( "Press Space or Return to finish creation." );
		}

		public override void Finish( bool cancel )
		{
			if( !cancel )
			{
				//calculate area position
				var points = CreatingObject.GetPoints();
				if( points.Length != 0 )
				{
					var position = Vector3.Zero;
					foreach( var point in points )
						position += point.TransformV.Position;
					position /= points.Length;
					CreatingObject.Transform = new Transform( position, Quaternion.Identity );
				}

				////attach points to the area
				//foreach( var point in points )
				//	ObjectInSpace_Utility.Attach( CreatingObject, point );

				//select area and points
				var toSelect = new List<Component>();
				toSelect.Add( CreatingObject );
				toSelect.AddRange( points );
				EditorAPI2.SelectComponentsInMainObjectsWindow( DocumentWindow, toSelect.ToArray() );
			}

			base.Finish( cancel );
		}
	}
}
