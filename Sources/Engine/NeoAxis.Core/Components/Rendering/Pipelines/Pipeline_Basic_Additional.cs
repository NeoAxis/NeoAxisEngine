// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	public partial class Component_RenderingPipeline_Basic
	{
		void DisplayObjectInSpaceBounds( ViewportRenderingContext context, FrameData frameData )
		{
			var context2 = context.objectInSpaceRenderingContext;
			var viewport = context.Owner;
			var scene = viewport.AttachedScene;

			var objects = new List<(Component_ObjectInSpace, float)>( frameData.ObjectInSpaces.Count );
			for( int n = 0; n < frameData.ObjectInSpaces.Count; n++ )
			{
				ref var data = ref frameData.ObjectInSpaces.Data[ n ];

				if( data.InsideFrustum )
				{
					var obj = data.ObjectInSpace;
					var center = obj.SpaceBounds.CalculatedBoundingBox.GetCenter();
					var distanceSquared = ( center - viewport.CameraSettings.Position ).LengthSquared();
					//var distanceSquared = ( obj.TransformV.Position - viewport.CameraSettings.Position ).LengthSquared();
					objects.Add( (obj, (float)distanceSquared) );
				}
			}

			CollectionUtility.MergeSort( objects, delegate ( (Component_ObjectInSpace, float) item1, (Component_ObjectInSpace, float) item2 )
			{
				var distanceSquared1 = item1.Item2;
				var distanceSquared2 = item2.Item2;
				if( distanceSquared1 < distanceSquared2 )
					return -1;
				if( distanceSquared1 > distanceSquared2 )
					return 1;
				return 0;
			}, true );

			int counter = 0;

			foreach( var item in objects )
			{
				var obj = item.Item1;

				ColorValue color = ProjectSettings.Get.SceneShowObjectInSpaceBoundsColor;
				viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

				var bounds = obj.SpaceBounds.CalculatedBoundingBox;

				double lineThickness = 0;
				//precalculate line thickness
				if( bounds.GetSize().MaxComponent() < 10 )
					lineThickness = viewport.Simple3DRenderer.GetThicknessByPixelSize( bounds.GetCenter(), ProjectSettings.Get.LineThickness );

				viewport.Simple3DRenderer.AddBounds( bounds, false, lineThickness );

				counter++;
				if( counter >= context2.displayObjectInSpaceBoundsMax )
					break;
			}
		}

		void DisplayPhysicalObjects( ViewportRenderingContext context, FrameData frameData )
		{
			var scene = context.Owner.AttachedScene;
			var viewport = context.Owner;
			var context2 = context.objectInSpaceRenderingContext;

			if( ( scene.GetDisplayDevelopmentDataInThisApplication() && scene.DisplayPhysicalObjects ) ||
				context2.selectedObjects.Count != 0 || context2.canSelectObjects.Count != 0 || context2.objectToCreate != null )
			{
				var objects = new List<(Component_IPhysicalObject, float)>( frameData.ObjectInSpaces.Count );
				for( int n = 0; n < frameData.ObjectInSpaces.Count; n++ )
				{
					ref var data = ref frameData.ObjectInSpaces.Data[ n ];

					if( data.InsideFrustum )
					{
						var obj = data.ObjectInSpace;
						if( obj is Component_IPhysicalObject physicalObject )
						{
							var center = obj.SpaceBounds.CalculatedBoundingBox.GetCenter();
							var distanceSquared = ( center - viewport.CameraSettings.Position ).LengthSquared();
							//var distanceSquared = ( obj.TransformV.Position - viewport.CameraSettings.Position ).LengthSquared();
							objects.Add( (physicalObject, (float)distanceSquared) );
						}
					}
				}

				CollectionUtility.MergeSort( objects, delegate ( (Component_IPhysicalObject, float) item1, (Component_IPhysicalObject, float) item2 )
				{
					var distanceSquared1 = item1.Item2;
					var distanceSquared2 = item2.Item2;
					if( distanceSquared1 < distanceSquared2 )
						return -1;
					if( distanceSquared1 > distanceSquared2 )
						return 1;
					return 0;
				}, true );

				int counterCount = 0;
				int counterVertices = 0;

				foreach( var item in objects )
				{
					var obj = item.Item1;

					bool show = ( scene.GetDisplayDevelopmentDataInThisApplication() && scene.DisplayPhysicalObjects ) ||
						context2.selectedObjects.Contains( obj ) || context2.canSelectObjects.Contains( obj ) || context2.objectToCreate == obj;
					if( show )
					{
						obj.Render( context, out var verticesRendered );

						counterCount++;
						if( counterCount >= context2.displayPhysicalObjectsMaxCount )
							break;
						counterVertices += verticesRendered;
						if( counterVertices >= context2.displayPhysicalObjectsMaxVertices )
							break;
					}
				}
			}
		}

		void SortObjectInSpaceLabels( ViewportRenderingContext context, FrameData frameData )
		{
			CollectionUtility.MergeSort( context.Owner.LastFrameScreenLabels,
				delegate ( Viewport.LastFrameScreenLabelItem item1, Viewport.LastFrameScreenLabelItem item2 )
			{
				if( item1.DistanceToCamera > item2.DistanceToCamera )
					return -1;
				if( item1.DistanceToCamera < item2.DistanceToCamera )
					return 1;
				return 0;
			}, true );
		}

		void DisplayObjectInSpaceLabels( ViewportRenderingContext context, FrameData frameData )
		{
			var triangles = new List<CanvasRenderer.TriangleVertex>( context.Owner.LastFrameScreenLabels.Count * 6 );

			foreach( var label in context.Owner.LastFrameScreenLabels )
			{
				if( label.Color.Alpha > 0 )
				{
					var rect = label.ScreenRectangle.ToRectangleF();

					var v0 = new CanvasRenderer.TriangleVertex( rect.LeftTop, label.Color, new Vector2F( 0, 0 ) );
					var v1 = new CanvasRenderer.TriangleVertex( rect.RightTop, label.Color, new Vector2F( 1, 0 ) );
					var v2 = new CanvasRenderer.TriangleVertex( rect.RightBottom, label.Color, new Vector2F( 1, 1 ) );
					var v3 = new CanvasRenderer.TriangleVertex( rect.LeftBottom, label.Color, new Vector2F( 0, 1 ) );

					triangles.Add( v0 );
					triangles.Add( v1 );
					triangles.Add( v2 );
					triangles.Add( v2 );
					triangles.Add( v3 );
					triangles.Add( v0 );
				}
			}

			if( triangles.Count != 0 )
			{
				var texture = ResourceManager.LoadResource<Component_Image>( "Base\\UI\\Images\\Circle.png" );
				context.Owner.CanvasRenderer.AddTriangles( triangles, texture, true );
			}
		}
	}
}
