// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NeoAxis.Editor;

namespace NeoAxis.Addon.Pathfinding
{
	/// <summary>
	/// Represents a test mode in the editor for pathfinding component.
	/// </summary>
	public class Component_Pathfinding_TestMode : Component_Scene_DocumentWindow.WorkareaModeClass_Scene
	{
		Component_Pathfinding owner;

		public Component_Pathfinding_TestMode( Component_Scene_DocumentWindow documentWindow, Component_Pathfinding owner )
			: base( documentWindow )
		{
			this.owner = owner;
		}

		//[Config( "RecastTestArea", "stepSize" )]
		//public static float stepSize = 1;
		//[Config( "RecastTestArea", "polygonPickExtents" )]
		//public static float polygonPickExtents = 2;
		//[Config( "RecastTestArea", "maxPolygonPath" )]
		//public static int maxPolygonPath = 512;
		//[Config( "RecastTestArea", "maxSmoothPath" )]
		//public static int maxSmoothPath = 4096;
		//[Config( "RecastTestArea", "maxSteerPoints" )]
		//public static int maxSteerPoints = 16;

		bool pathTest;
		Vector3 startPosition;
		Vector3 endPosition;
		Vector3[] path;
		string error;
		bool found;
		double time;
		double timeLastUpdateTime;

		//public TestToolTypes toolType = TestToolTypes.DirectPathfind;

		/////////////////////////////////////////

		//public enum TestToolTypes
		//{
		//	None,
		//	DirectPathfind,
		//}

		/////////////////////////////////////////

		//protected override bool OnKeyDown( KeyEvent e )
		//{
		//	if( e.Key == EKeys.Escape )
		//	{
		//		MapEditorInterface.Instance.FunctionalityArea = null;
		//		return true;
		//	}

		//	return base.OnKeyDown( e );
		//}

		protected override bool OnMouseDown( Viewport viewport, EMouseButtons button )
		{
			if( button == EMouseButtons.Left )
			{
				//if( toolType == TestToolTypes.DirectPathfind )
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
			var context = new Component_Scene_DocumentWindow.GetMouseOverObjectToSelectByClickContext();
			context.CheckOnlyObjectsWithEnabledSelectionByCursorFlag = false;

			//MethodInvoke is used to remove reference to the WinForms dll
			ObjectEx.MethodInvoke( DocumentWindow, "GetMouseOverObjectToSelectByClick", context );
			//DocumentWindow.GetMouseOverObjectToSelectByClick_ForExistsContext( context );

			var resultObjectComponent = context.ResultObject as Component;
			if( resultObjectComponent != null )
			{
				//!!!!так? может в GetMouseOverObjectForSelection указывать фильтр делегатом

				//!!!!проверять что Walkable Area?

				var geometry = resultObjectComponent.GetComponent<Component_Pathfinding_GeometryTag>( false, true );
				if( geometry != null )
				{
					pos = context.ResultPosition.HasValue ? context.ResultPosition.Value : Vector3.Zero;
					return true;
				}
			}

			//RayCastResult[] results = PhysicsWorld.Instance.RayCastPiercing( ray, (int)ContactGroup.CastOnlyCollision );
			//foreach( RayCastResult result in results )
			//{
			//	Radian angle = MathUtils.GetVectorsAngle( result.Normal, ray.Direction );
			//	if( angle > Math.PI / 2 )
			//	{
			//		pos = result.Position;
			//		return true;
			//	}
			//}
			//}

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

					//if( stepSize < .1f )
					//   stepSize = .1f;
					//if( polygonPickExtents < .001f )
					//   polygonPickExtents = .001f;
					//MathFunctions.Clamp( ref maxPolygonPath, 1, 65536 );
					//MathFunctions.Clamp( ref maxSmoothPath, 1, 65536 );
					//MathFunctions.Clamp( ref maxSteerPoints, 1, 256 );

					var context = new Component_Pathfinding.FindPathContext();
					context.Start = startPosition;
					context.End = endPosition;

					owner.FindPath( context );

					found = context.Path != null;
					path = context.Path;
					error = context.Error;

					//found = owner.FindPath( startPosition, endPosition,
					//	stepSize, new Vec3( polygonPickExtents, polygonPickExtents, polygonPickExtents ),
					//	maxPolygonPath, maxSmoothPath, maxSteerPoints, out path );

					var endTime = EngineApp.GetSystemTime();

					if( ( EngineApp.GetSystemTime() - timeLastUpdateTime ) > 0.25 )
					{
						timeLastUpdateTime = EngineApp.GetSystemTime();
						time = endTime - startTime;
					}
				}
			}
		}

		bool IsFutileFound()
		{
			if( found && ( path[ path.Length - 1 ] - endPosition ).Length() > 1f )
				return true;
			return false;
		}

		protected override void OnUpdateBeforeOutput( Viewport viewport )
		{
			//3D
			//if( toolType == TestToolTypes.DirectPathfind )
			{
				var renderer = viewport.Simple3DRenderer;

				Vector3 offset = new Vector3( 0, 0, 0.1 );

				if( pathTest )
				{
					if( found )
					{
						renderer.SetColor( IsFutileFound() ? new ColorValue( 1, 0, 0 ) : new ColorValue( 0, 1, 0 ) );
						for( int n = 0; n < path.Length - 1; n++ )
						{
							Vector3 point1 = path[ n ] + offset;
							Vector3 point2 = path[ n + 1 ] + offset;
							renderer.AddLine( point1, point2, .07 );
						}

						renderer.SetColor( IsFutileFound() ? new ColorValue( 1, 0, 0 ) : new ColorValue( 1, 1, 0 ) );
						for( int n = 0; n < path.Length; n++ )
						{
							Vector3 point = path[ n ] + offset;
							renderer.AddSphere( new Sphere( point, .15 ), 16, true );
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
							renderer.AddArrow( startPosition, endPosition, 0, 0, true );
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

			////UI
			//if( toolType == TestToolTypes.DirectPathfind )
			//{
			//	if( pathTest )
			//	{
			//		List<string> lines = new List<string>();

			//		lines.Add( string.Format( Translate( "Time: {0} seconds" ), time.ToString( "F8" ) ) );

			//		//we check if the path will lead us close enough to where we wanted
			//		if( found )
			//		{
			//			if( IsFutileFound() )
			//				lines.Add( Translate( "Path found, but didn't reach close enough to end point." ) );
			//			else
			//				lines.Add( Translate( "Path found." ) );
			//		}
			//		else
			//			lines.Add( Translate( "Path not found." ) );

			//		if( found )
			//			lines.Add( string.Format( Translate( "Points: {0}" ), path.Length ) );

			//		//!!!!
			//		viewport.CanvasRenderer.AddTextLines( lines, new Vec2( 0.5, 0.5 ), EHorizontalAlign.Left, EVerticalAlign.Top, 0, new ColorValue( 1, 1, 1 ) );
			//		//MapEditorEngineApp.Instance.AddTextLinesWithShadow( renderer, lines, new Rect( .05f, .075f, 1, 1 ),
			//		//	HorizontalAlign.Left, VerticalAlign.Top, new ColorValue( 1, 1, 0 ) );
			//	}
			//	else
			//	{
			//		string text = Translate( "Specify start and end points with the mouse." );

			//		//!!!!
			//		viewport.CanvasRenderer.AddText( text, new Vec2( 0.5, 0.5 ), EHorizontalAlign.Left, EVerticalAlign.Top, new ColorValue( 1, 1, 1 ) );

			//		//MapEditorEngineApp.Instance.AddTextWithShadow( renderer, text, new Vec2( .5f, .5f ), HorizontalAlign.Center,
			//		//	VerticalAlign.Center, new ColorValue( 1, 1, 0 ) );
			//	}
			//}
		}

		//!!!!
		string Translate( string text )
		{
			return text;
			//return ToolsLocalization.Translate( "RecastTestArea", text );
		}

		protected override void OnGetTextInfoCenterBottomCorner( List<string> lines )
		{
			base.OnGetTextInfoCenterBottomCorner( lines );

			//UI
			//if( toolType == TestToolTypes.DirectPathfind )
			//{

			if( pathTest )
			{
				//we check if the path will lead us close enough to where we wanted
				if( found )
				{
					if( IsFutileFound() )
						lines.Add( Translate( "The path was found, but did not get close enough to the end point." ) );
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
				lines.Add( Translate( "Specify start and end points by clicking and holding mouse button." ) );
			}

			//}
		}
	}
}
