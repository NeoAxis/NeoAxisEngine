// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents an area in the scene defined by the set of points.
	/// </summary>
#if !DEPLOY
	[ObjectCreationMode( "NeoAxis.Editor.AreaCreationMode" )]
	[AddToResourcesWindow( @"Base\Scene objects\Areas\Area", 0 )]
#endif
	public class Area : ObjectInSpace
	{
		public AreaPoint[] GetPoints()
		{
			return GetComponents<AreaPoint>();
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

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			//if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				//display editor selection
				{
					var context2 = context.ObjectInSpaceRenderingContext;

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
								color = ProjectSettings.Get.Colors.SelectedColor;
							else if( context2.canSelectObjects.Contains( this ) )
								color = ProjectSettings.Get.Colors.CanSelectColor;
							else
								color = ProjectSettings.Get.Colors.SceneShowAreaColor;

							renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

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
				if( context.SceneDisplayDevelopmentDataInThisApplication/*ParentScene.GetDisplayDevelopmentDataInThisApplication()*/ && ParentScene.DisplayAreas )
					return true;

				var context2 = context.ObjectInSpaceRenderingContext;

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

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Area" );
		}
	}
}
