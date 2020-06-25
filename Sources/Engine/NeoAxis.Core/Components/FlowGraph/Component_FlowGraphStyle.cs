// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using NeoAxis.Editor;
using System.Drawing.Design;

namespace NeoAxis
{
	/// <summary>
	/// The graphical style of the flow graph.
	/// </summary>
	public abstract class Component_FlowGraphStyle : Component
	{
		public abstract void RenderBackground( Component_FlowGraph_DocumentWindow window );
		public abstract void RenderForeground( Component_FlowGraph_DocumentWindow window );
		public abstract void RenderReference( Component_FlowGraph_DocumentWindow window, Vector2 from, bool fromInput, Vector2 to, ColorValue color, out bool mouseOver );
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The default style of the flow graph.
	/// </summary>
	public class Component_FlowGraphStyle_Default : Component_FlowGraphStyle
	{
		static Component_FlowGraphStyle_Default instance;
		public static Component_FlowGraphStyle_Default Instance
		{
			get
			{
				if( instance == null )
				{
					instance = new Component_FlowGraphStyle_Default();
					instance.Name = "Default";
				}
				return instance;
			}
		}

		/////////////////////////////////////////

		public override void RenderBackground( Component_FlowGraph_DocumentWindow window )
		{
			RenderBackgroundGrid( window );

			//!!!!было
			//bool processed = false;
			//RenderBackgroundEvent?.Invoke( this, window, ref processed );
			//if( !processed )
			//	RenderBackgroundDefault( window );
		}

		public override void RenderForeground( Component_FlowGraph_DocumentWindow window )
		{
			RenderSpecializationName( window );
		}

		//!!!!было
		//public delegate void RenderBackgroundEventDelegate( Component_FlowchartStyle sender, Component_Flowchart_DocumentWindow window, ref bool processed );
		//public event RenderBackgroundEventDelegate RenderBackgroundEvent;

		public virtual void RenderBackgroundGrid( Component_FlowGraph_DocumentWindow window )
		{
			var viewport = window.ViewportControl.Viewport;
			var renderer = viewport.CanvasRenderer;

			RectangleI visibleCells = window.GetVisibleCells();

			//draw background
			renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new ColorValue( .17f, .17f, .17f ) );

			//draw grid
			if( window.GetZoom() > .5f )
			{
				var lines = new List<CanvasRenderer.LineItem>( 256 );

				{
					ColorValue color = new ColorValue( .2f, .2f, .2f );
					for( int x = visibleCells.Left; x <= visibleCells.Right; x++ )
					{
						if( x % 10 != 0 )
						{
							var floatX = (float)window.ConvertUnitToScreenX( x );
							lines.Add( new CanvasRenderer.LineItem( new Vector2F( floatX, 0 ), new Vector2F( floatX, 1 ), color ) );
						}
					}
					for( int y = visibleCells.Top; y <= visibleCells.Bottom; y++ )
					{
						if( y % 10 != 0 )
						{
							var floatY = (float)window.ConvertUnitToScreenY( y );
							lines.Add( new CanvasRenderer.LineItem( new Vector2F( 0, floatY ), new Vector2F( 1, floatY ), color ) );
						}
					}
				}

				{
					ColorValue color = new ColorValue( .1f, .1f, .1f );
					for( int x = visibleCells.Left; x <= visibleCells.Right; x++ )
					{
						if( x % 10 == 0 )
						{
							var floatX = (float)window.ConvertUnitToScreenX( x );
							lines.Add( new CanvasRenderer.LineItem( new Vector2F( floatX, 0 ), new Vector2F( floatX, 1 ), color ) );
						}
					}
					for( int y = visibleCells.Top; y <= visibleCells.Bottom; y++ )
					{
						if( y % 10 == 0 )
						{
							var floatY = (float)window.ConvertUnitToScreenY( y );
							lines.Add( new CanvasRenderer.LineItem( new Vector2F( 0, floatY ), new Vector2F( 1, floatY ), color ) );
						}
					}
				}

				viewport.CanvasRenderer.AddLines( lines );
			}
		}

		public virtual void RenderSpecializationName( Component_FlowGraph_DocumentWindow window )
		{
			var viewport = window.ViewportControl.Viewport;
			var specialization = window.FlowGraph.Specialization.Value;
			string specializationName = specialization != null ? specialization.Name : "Default";
			var text = string.Format( "{0} specialization", specializationName );
			window.AddTextWithShadow( text, new Vector2( 0.99, 0.99 ), EHorizontalAlignment.Right, EVerticalAlignment.Bottom,
				new ColorValue( 0.7, 0.7, 0.7, 0.7 ) );
		}

		static double LineSegmentPointDistance( Vector2 v, Vector2 w, Vector2 p )
		{
			// Return minimum distance between line segment vw and point p
			double l2 = ( v - w ).LengthSquared();// length_squared( v, w );  // i.e. |w-v|^2 -  avoid a sqrt
			if( l2 == 0.0f )
				return ( p - v ).Length();
			// distance( p, v );   // v == w case
			// Consider the line extending the segment, parameterized as v + t (w - v).
			// We find projection of point p onto the line. 
			// It falls where t = [(p-v) . (w-v)] / |w-v|^2
			double t = Vector2.Dot( p - v, w - v ) / l2;
			if( t < 0.0f )
				return ( p - v ).Length();// distance( p, v );       // Beyond the 'v' end of the segment
			else if( t > 1.0f )
				return ( p - w ).Length();// distance( p, w );  // Beyond the 'w' end of the segment
			Vector2 projection = v + t * ( w - v );  // Projection falls on the segment
			return ( p - projection ).Length();// distance( p, projection );
		}

		//!!!!для не прямоугольных нод видать не такие параметры
		public override void RenderReference( Component_FlowGraph_DocumentWindow window, Vector2 from, bool fromInput, Vector2 to, ColorValue color, out bool mouseOver )
		{
			var viewport = window.ViewportControl.Viewport;

			mouseOver = false;

			//!!!!
			if( ( from - to ).Length() < 0.5 )
			//if( ( from - to ).Length() < 0.5 )
			{
				viewport.CanvasRenderer.AddLine( window.ConvertUnitToScreen( from ), window.ConvertUnitToScreen( to ), color /* colorMultiplier*/ );
				mouseOver = false;
				return;
			}

			//!!!!!!если за пределами экрана, то не проверять. в других местах тоже.
			Vector2 mousePositionInUnits = window.ConvertScreenToUnit( viewport.MousePosition, true );

			double diff = to.X - from.X + 4;
			if( fromInput )
				diff = -diff;

			var sidePointsDistance = Math.Sqrt( Math.Abs( diff ) * 2 ) * 1.3;
			if( sidePointsDistance > 15 )
				sidePointsDistance = 15;
			if( sidePointsDistance < 3 )
				sidePointsDistance = 3;

			if( fromInput )
			{
				if( from.X - to.X > 0 && from.X - to.X < 4 )
				{
					var d = 4 - ( from.X - to.X );
					if( d < 1 )
						d = 1;
					sidePointsDistance /= d;
				}
			}
			else
			{
				if( to.X - from.X > 0 && to.X - from.X < 4 )
				{
					var d = 4 - ( to.X - from.X );
					if( d < 1 )
						d = 1;
					sidePointsDistance /= d;
				}
			}

			//var realDiffAbs = Math.Abs( to.X - from.X );
			//if( realDiffAbs < 2 )
			//{
			//	sidePointsDistance /= 
			//}

			//if( to.X - from.X > 0 && to.X - from.X < 3 )
			//{
			//	sidePointsDistance -= 3 - ( to.X - from.X );
			//	//sidePointsDistance = 4;
			//}

			//double sidePointsDistance;
			//if( diff > 0 )
			//{
			//	sidePointsDistance = Math.Sqrt( diff * 2 ) * 1.3;
			//	if( sidePointsDistance > 15 )
			//		sidePointsDistance = 15;
			//	if( sidePointsDistance < 4 )
			//		sidePointsDistance = 4;

			//	//sidePointsDistance = diff / 2;
			//	//if( diff < 4 )
			//	//	sidePointsDistance = diff;
			//	//sidePointsDistance = diff / 3;
			//	//if( sidePointsDistance < 4 )
			//	//	sidePointsDistance = 4;
			//}
			//else
			//{
			//	sidePointsDistance = Math.Sqrt( -diff * 2 ) * 1.3;
			//	if( sidePointsDistance > 15 )
			//		sidePointsDistance = 15;
			//	if( sidePointsDistance < 4 )
			//		sidePointsDistance = 4;
			//}

			if( fromInput )
				sidePointsDistance = -sidePointsDistance;

			//double sidePointsDistance = 5;
			//if( to.X < from.X )
			//	sidePointsDistance = ( from.X - to.X ) / 3;
			//if( sidePointsDistance < 5 )
			//	sidePointsDistance = 5;
			//if( to.X - from.X < 5 && to.X - from.X > 0 )
			//	sidePointsDistance = to.X - from.X;
			//if( to.X == from.X )
			//	sidePointsDistance = 2;
			//if( from.X - to.X == 1 )
			//	sidePointsDistance = 3;
			//if( from.X - to.X == 2 )
			//	sidePointsDistance = 4;
			//if( fromInput )
			//	sidePointsDistance = -sidePointsDistance;

			Vector2[] inputPoints = new Vector2[ 4 ];
			inputPoints[ 0 ] = from;
			inputPoints[ 1 ] = from + new Vector2( sidePointsDistance, 0 );
			inputPoints[ 2 ] = to + new Vector2( -sidePointsDistance, 0 );
			inputPoints[ 3 ] = to;

			double[] inputPointsDoubles = new double[ inputPoints.Length * 2 ];
			for( int n = 0; n < inputPoints.Length; n++ )
			{
				inputPointsDoubles[ n * 2 + 0 ] = inputPoints[ n ].X;
				inputPointsDoubles[ n * 2 + 1 ] = inputPoints[ n ].Y;
			}

			//!!!!!менять количество в зависимости от масштаба
			//!!!!!нелинейно менять количество. на загруглениях больше точек.
			//!!!!!или от растяжения количество точек
			//!!!!!или найти сплайну, которая сама считает нужное количество точек
			double pointsOnCurveD = 10 + Math.Abs( from.X - to.X ) / 1.5 + Math.Abs( from.Y - to.Y ) / 1.5;
			//int pointsOnCurve = 25;
			if( window.GetZoom() < .5f )
				pointsOnCurveD /= 2;
			if( window.GetZoom() < .25f )
				pointsOnCurveD /= 2;
			if( window.GetZoom() > 1 )
				pointsOnCurveD *= window.GetZoom();
			var pointsOnCurve = (int)pointsOnCurveD;

			double[] outPointsDoubles = new double[ pointsOnCurve * 2 ];
			BezierCurve2D.Bezier2D( inputPointsDoubles, pointsOnCurve, outPointsDoubles );

			Vector2[] outPoints = new Vector2[ pointsOnCurve ];
			for( int n = 0; n < pointsOnCurve; n++ )
				outPoints[ n ] = new Vector2( (float)outPointsDoubles[ n * 2 + 0 ], (float)outPointsDoubles[ n * 2 + 1 ] );

			var lines = new List<CanvasRenderer.LineItem>( outPoints.Length );

			Vector2 previousPos = Vector2.Zero;
			Vector2 previousPosInUnits = Vector2.Zero;
			for( int n = 0; n < outPoints.Length; n++ )
			{
				Vector2 positionInUnits = outPoints[ n ];

				if( n != 0 && !mouseOver )
				{
					////!!!!!или только за Title цеплять
					//if( rect.IsContainsPoint( renderTargetUserControl1.GetFloatMousePosition() ) )
					//   mouseOverObjects.Add( node );

					const float minDistance = 0.3f;// 0.25f;
					if( LineSegmentPointDistance( previousPosInUnits, positionInUnits, mousePositionInUnits ) < minDistance )
						mouseOver = true;
				}

				Vector2 pos = window.ConvertUnitToScreen( positionInUnits );
				if( n != 0 )
				{
					lines.Add( new CanvasRenderer.LineItem( previousPos.ToVector2F(), pos.ToVector2F(), color /* colorMultiplier*/ ) );
					//viewport.CanvasRenderer.AddLine( previousPos, pos, color /* colorMultiplier*/ );
				}
				previousPos = pos;
				previousPosInUnits = positionInUnits;
			}

			viewport.CanvasRenderer.AddLines( lines );
		}
	}
}
