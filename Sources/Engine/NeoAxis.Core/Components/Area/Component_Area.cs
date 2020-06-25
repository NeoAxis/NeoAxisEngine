// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents an area in the scene defined by the set of points.
	/// </summary>
	[ObjectCreationMode( typeof( CreationModeArea ) )]
	public class Component_Area : Component_ObjectInSpace
	{

		/////////////////////////////////////////

		/// <summary>
		/// A class for providing the creation of a <see cref="Component_Area"/> in the editor.
		/// </summary>
		public class CreationModeArea : ObjectCreationMode
		{
			Rectangle? lastStartPointRectangle;

			//


			public CreationModeArea( DocumentWindowWithViewport documentWindow, Component creatingObject )
				: base( documentWindow, creatingObject )
			{
				var point = CreatingObject.CreateComponent<Component_AreaPoint>( enabled: false );
				point.Name = CreatingObject.Components.GetUniqueName( "Point", false, 1 );
				point.Transform = new Transform( CreatingObject.TransformV.Position, Quaternion.Identity );
				point.Enabled = true;
			}

			public new Component_Area CreatingObject
			{
				get { return (Component_Area)base.CreatingObject; }
			}

			protected virtual bool CalculatePointPosition( Viewport viewport, out Vector3 position, out Component_ObjectInSpace collidedWith )
			{
				if( !viewport.MouseRelativeMode )
				{
					var sceneDocumentWindow = DocumentWindow as Component_Scene_DocumentWindow;
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
							var point = CreatingObject.CreateComponent<Component_AreaPoint>( enabled: false );
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

			protected override void OnGetTextInfoRightBottomCorner( List<string> lines )
			{
				base.OnGetTextInfoRightBottomCorner( lines );

				lines.Add( "Specify points of the area." );
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
					//	Component_ObjectInSpace_Utility.Attach( CreatingObject, point );

					//select area and points
					var toSelect = new List<Component>();
					toSelect.Add( CreatingObject );
					toSelect.AddRange( points );
					EditorAPI.SelectComponentsInMainObjectsWindow( DocumentWindow, toSelect.ToArray() );
				}

				base.Finish( cancel );
			}
		}

		/////////////////////////////////////////

		public Component_AreaPoint[] GetPoints()
		{
			return GetComponents<Component_AreaPoint>();
		}

		public Vector3[] GetPointPositions()
		{
			var points = GetPoints();
			var result = new Vector3[ points.Length ];
			for( int n = 0; n < points.Length; n++ )
				result[ n ] = points[ n ].TransformV.Position;
			return result;
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			var bounds = Bounds.Cleared;
			foreach( var p in GetPoints() )
				bounds.Add( p.TransformV.Position );
			if( !bounds.IsCleared() )
				newBounds = new SpaceBounds( bounds );
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			//if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				//display editor selection
				{
					var context2 = context.objectInSpaceRenderingContext;

					//bool show = ( ParentScene.GetDisplayDevelopmentDataInThisApplication() && ParentScene.DisplayAreas ) || context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.dragDropCreateObject == this;
					//if( show )
					if( CheckNeedShowDevelopmentDataOfAreaAndPoints( context, mode ) )
					{
						var pointPositions = GetPointPositions();

						//!!!!count limit

						var viewport = context.Owner;
						var renderer = viewport.Simple3DRenderer;
						if( renderer != null )
						{
							ColorValue color;
							if( context2.selectedObjects.Contains( this ) )
								color = ProjectSettings.Get.SelectedColor;
							else if( context2.canSelectObjects.Contains( this ) )
								color = ProjectSettings.Get.CanSelectColor;
							else
								color = ProjectSettings.Get.SceneShowAreaColor;

							renderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

							for( int n = 0; n < pointPositions.Length; n++ )
							{
								var from = pointPositions[ n ];
								var to = pointPositions[ ( n + 1 ) % pointPositions.Length ];
								renderer.AddLine( from, to );
							}
						}
					}
				}
			}
		}

		public bool CheckNeedShowDevelopmentDataOfAreaAndPoints( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				if( ParentScene.GetDisplayDevelopmentDataInThisApplication() && ParentScene.DisplayAreas )
					return true;

				var context2 = context.objectInSpaceRenderingContext;

				if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this )
					return true;

				foreach( var point in GetPoints() )
				{
					if( context2.selectedObjects.Contains( point ) || context2.canSelectObjects.Contains( point ) || context2.objectToCreate == point )
						return true;
				}
			}

			return false;
		}

		protected override bool OnEnabledSelectionByCursor()
		{
			if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() || !ParentScene.DisplayLabels )
				return false;
			return base.OnEnabledSelectionByCursor();
		}

	}
}
