// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using ComponentFactory.Krypton.Toolkit;
using NeoAxis.Widget;
using System.Runtime.InteropServices;

namespace NeoAxis.Editor
{
	public partial class TransformTool
	{
		class RotationModeClass : ModeClass
		{
			TransformOfObject[] initialObjectsTransform;
			Vector3 modifyPosition;
			Axis selectedAxis = Axis.None;
			Vector2 selectedScreenLinePointInPixels;
			Radian selectedScreenLineAngle;
			Vector2 mouseStartPosition;
			//Vec2 mouseStartPositionInPixels;
			string helpText = "";

			///////////////

			enum Axis
			{
				None,
				X,
				Y,
				Z,
				Radius,
				InnerCircle,
			}

			///////////////

			Axis GetAxisByMousePosition( out Vector2 screenLinePointInPixels, out Radian screenLineAngle )
			{
				screenLinePointInPixels = Vector2.Zero;
				screenLineAngle = 0;

				Vector2 viewportSize = Owner.ViewportControl.Viewport.SizeInPixels.ToVector2();

				Trace.Assert( Owner.Objects.Count != 0 );

				Vector3 position = Owner.GetPosition();
				double radius = GetSize();
				if( radius == 0 )
					return Axis.None;

				const double step = MathEx.PI / 64;

				double innerRadius = radius * .75f;
				double cameraDistance = GetCameraDistance( position );

				Vector3 oldPos = Vector3.Zero;

				Quaternion startRotation = GetStartObjectsRotation();

				//X
				for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
				{
					Vector3 pos = position + startRotation * ( new Vector3( 0, Math.Sin( angle ), Math.Cos( angle ) ) * innerRadius );
					if( angle != 0 && GetCameraDistance( pos ) <= cameraDistance &&
						IsMouseNearLine( oldPos, pos, out screenLinePointInPixels, out screenLineAngle, out _ ) )
					{
						return Axis.X;
					}
					oldPos = pos;
				}

				//Y
				for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
				{
					Vector3 pos = position + startRotation * ( new Vector3( Math.Sin( angle ), 0, Math.Cos( angle ) ) * innerRadius );
					if( angle != 0 && GetCameraDistance( pos ) <= cameraDistance &&
						IsMouseNearLine( oldPos, pos, out screenLinePointInPixels, out screenLineAngle, out _ ) )
					{
						return Axis.Y;
					}
					oldPos = pos;
				}

				//Z
				for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
				{
					Vector3 pos = position + startRotation * ( new Vector3( Math.Sin( angle ),
						Math.Cos( angle ), 0 ) * innerRadius );
					if( angle != 0 && ( GetCameraDistance( pos ) <= cameraDistance || Owner.SceneMode2D ) &&
						IsMouseNearLine( oldPos, pos, out screenLinePointInPixels, out screenLineAngle, out _ ) )
					{
						return Axis.Z;
					}
					oldPos = pos;
				}

				Vector2 mouseInPixels = Owner.viewportControl.Viewport.MousePosition * viewportSize;

				Vector2 screenPosition;
				if( CameraSettings.ProjectToScreenCoordinates( position, out screenPosition ) )
				{
					Vector2 screenPositionInPixels = screenPosition * viewportSize;
					double mouseDistanceInPixels = ( mouseInPixels - screenPositionInPixels ).Length();

					//Radius
					if( Math.Abs( mouseDistanceInPixels - ArrowPixelSize ) < SelectNearPixels && !Owner.SceneMode2D )
					{
						Vector2 diffPixels = mouseInPixels - screenPositionInPixels;
						Vector2 direction = diffPixels.GetNormalize();

						screenLinePointInPixels = ( screenPositionInPixels + direction * ArrowPixelSize );
						screenLineAngle = Math.Atan2( diffPixels.Y, diffPixels.X ) + MathEx.PI / 2;

						return Axis.Radius;
					}

					//Inner circle
					if( mouseDistanceInPixels < ( ArrowPixelSize ) * .75f + SelectNearPixels )
						return Axis.InnerCircle;
				}

				return Axis.None;
			}

			double GetCameraDistance( Vector3 pos )
			{
				return ( CameraSettings.Position - pos ).Length();
			}

			internal protected override bool OnMouseOverAxis()
			{
				if( !Owner.IsAllowRotate() )
					return false;
				Vector2 screenLinePointInPixels;
				Radian screenLineAngle;
				Axis axis = GetAxisByMousePosition( out screenLinePointInPixels, out screenLineAngle );
				return axis != Axis.None;
			}

			protected override bool OnTryBeginModify()
			{
				if( !Owner.IsAllowRotate() )
					return false;

				Vector2 screenLinePointInPixels;
				Radian screenLineAngle;
				Axis axis = GetAxisByMousePosition( out screenLinePointInPixels, out screenLineAngle );

				if( axis != Axis.None )
				{
					modifyPosition = Owner.GetPosition();

					Vector2 screenPosition;
					if( CameraSettings.ProjectToScreenCoordinates( modifyPosition, out screenPosition ) )
					{
						Vector2 viewportSize = Owner.ViewportControl.Viewport.SizeInPixels.ToVector2();

						selectedAxis = axis;
						selectedScreenLinePointInPixels = screenLinePointInPixels;
						selectedScreenLineAngle = screenLineAngle;
						mouseStartPosition = Viewport.MousePosition;
						//mouseStartPositionInPixels = EngineApp.Instance.MousePosition * viewportSize;
						helpText = "";
						initialObjectsTransform = new TransformOfObject[ Owner.Objects.Count ];
						for( int n = 0; n < Owner.Objects.Count; n++ )
						{
							TransformToolObject obj = Owner.Objects[ n ];
							initialObjectsTransform[ n ] = new TransformOfObject( obj.Position, obj.Rotation, obj.Scale );
						}
						return true;
					}
				}

				return base.OnTryBeginModify();
			}

			protected override void OnCommitModify()
			{
				base.OnCommitModify();
				initialObjectsTransform = null;
			}

			internal protected override void OnCancelModify()
			{
				base.OnCancelModify();
				RestoreObjectsTransform();
				initialObjectsTransform = null;
			}

			void RestoreObjectsTransform()
			{
				if( initialObjectsTransform != null && Owner.Objects.Count == initialObjectsTransform.Length )
				{
					for( int n = 0; n < Owner.Objects.Count; n++ )
					{
						Owner.Objects[ n ].Position = initialObjectsTransform[ n ].position;
						Owner.Objects[ n ].Rotation = initialObjectsTransform[ n ].rotation;
					}
				}
			}

			Quaternion GetStartObjectsRotation()
			{
				if( Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
				{
					if( Owner.Objects.Count != 0 )
					{
						if( modify_Activated )
							return initialObjectsTransform[ 0 ].rotation;
						else
							return Owner.Objects[ 0 ].Rotation;
					}
				}
				return Quaternion.Identity;
			}

			void SnapAngle( ref Radian angle )
			{
				//snapping
				Degree snapDegree = Owner.GetSnapRotate();
				if( snapDegree != 0 )
				{
					Radian snap = snapDegree.InRadians();
					angle += snap / 2;
					angle /= snap;
					angle = (int)angle;
					angle *= snap;
				}
			}

			public override void OnMouseMove( Vector2 mouse )
			{
				base.OnMouseMove( mouse );

				if( modify_Activated && !owner.viewportControl.Viewport.MouseRelativeMode )
				{
					UpdateCursorTransitionOnScreenBorder();

					Vector2 viewportSize = Owner.ViewportControl.Viewport.SizeInPixels.ToVector2();
					var camera = owner.viewportControl.Viewport.CameraSettings;
					Vector2 mouseInPixels = owner.viewportControl.Viewport.MousePosition * viewportSize + cursorTransitionOnScreenBorderOffset;

					Vector2 mouseStartPositionInPixels = mouseStartPosition * viewportSize;
					Vector2 offset = mouseInPixels - mouseStartPositionInPixels;
					Quaternion startRotation = GetStartObjectsRotation();
					double scaleFactor = ProjectSettings.Get.TransformToolRotationSensitivity / 150;

					Quaternion rotationOffset = Quaternion.Identity;

					if( selectedAxis == Axis.X || selectedAxis == Axis.Y || selectedAxis == Axis.Z ||
						selectedAxis == Axis.Radius )
					{
						Radian angle;
						{
							Matrix2 m = Matrix2.FromRotate( selectedScreenLineAngle );
							Vector2 localOffset = m * offset;
							angle = localOffset.X * scaleFactor;
							SnapAngle( ref angle );
						}

						string zeroStr = 0.0f.ToString( "F2" );

						switch( selectedAxis )
						{
						case Axis.X:
							rotationOffset = new Quaternion( new Vector3( Math.Sin( -angle / 2 ), 0, 0 ), Math.Cos( -angle / 2 ) );
							rotationOffset = startRotation * rotationOffset.GetNormalize() * startRotation.GetInverse();
							helpText = string.Format( "[{0} {1} {2}]", ( (double)angle.InDegrees() ).ToString( "F2" ), zeroStr, zeroStr );
							break;

						case Axis.Y:
							rotationOffset = new Quaternion( new Vector3( 0, Math.Sin( angle / 2 ), 0 ), Math.Cos( angle / 2 ) );
							rotationOffset = startRotation * rotationOffset.GetNormalize() * startRotation.GetInverse();
							helpText = string.Format( "[{0} {1} {2}]", zeroStr, ( (double)-angle.InDegrees() ).ToString( "F2" ), zeroStr );
							break;

						case Axis.Z:
							rotationOffset = new Quaternion( new Vector3( 0, 0, Math.Sin( -angle / 2 ) ), Math.Cos( -angle / 2 ) );
							rotationOffset = startRotation * rotationOffset.GetNormalize() * startRotation.GetInverse();
							helpText = string.Format( "[{0} {1} {2}]", zeroStr, zeroStr, ( (double)angle.InDegrees() ).ToString( "F2" ) );
							break;

						case Axis.Radius:
							Vector3 cameraRight = Vector3.Cross( camera.Direction, camera.Up );
							Matrix3 cameraRotation = new Matrix3( camera.Direction, -cameraRight, camera.Up );
							Matrix3 m = cameraRotation * Matrix3.FromRotateByX( -angle ) * cameraRotation.GetInverse();
							rotationOffset = m.ToQuaternion();
							break;
						}
					}
					else if( selectedAxis == Axis.InnerCircle )
					{
						Radian angle1 = offset.X * scaleFactor;
						Radian angle2 = offset.Y * scaleFactor;
						SnapAngle( ref angle1 );
						SnapAngle( ref angle2 );

						Vector3 cameraRight = Vector3.Cross( camera.Direction, camera.Up );
						Matrix3 cameraRotation = new Matrix3( camera.Direction, -cameraRight, camera.Up );

						Matrix3 m = Matrix3.FromRotateByZ( -angle1 ) * Matrix3.FromRotateByY( angle2 );
						Matrix3 m2 = cameraRotation * m * cameraRotation.GetInverse();
						rotationOffset = m2.ToQuaternion();
					}

					rotationOffset.Normalize();

					//update objects
					if( initialObjectsTransform != null && Owner.Objects.Count == initialObjectsTransform.Length )
						Owner.OnRotationModeUpdateObjects( initialObjectsTransform, modifyPosition, rotationOffset );
				}
			}

			public override bool OnIsMouseOverAxisToActivation()
			{
				if( !Owner.IsAllowRotate() )
					return false;

				Axis axis = Axis.None;
				if( !Viewport.MouseRelativeMode )
				{
					if( !modify_Activated )
					{
						Vector2 dummy1;
						Radian dummy2;
						axis = GetAxisByMousePosition( out dummy1, out dummy2 );
					}
					else
						axis = selectedAxis;
				}

				Vector3 position;
				if( !modify_Activated )
					position = Owner.GetPosition();
				else
					position = modifyPosition;
				double lineThickness = GetLineWorldThickness( position );
				double radius = GetSize();
				if( radius == 0 )
					return false;

				return axis != Axis.None;
			}

			public override void OnRender()
			{
				base.OnRender();

				if( !Owner.IsAllowRotate() )
					return;

				ColorValue yellow = new ColorValue( 1, 1, 0 );
				ColorValue red = new ColorValue( 1, 0, 0 );
				ColorValue green = new ColorValue( 0, 1, 0 );
				ColorValue blue = new ColorValue( 0, 0, 1 );
				ColorValue gray = new ColorValue( .66f, .66f, .66f );
				ColorValue darkGray = new ColorValue( .33f, .33f, .33f );
				ColorValue shadowColor = new ColorValue( 0, 0, 0, ProjectSettings.Get.TransformToolShadowIntensity );

				Axis axis = Axis.None;
				if( !Viewport.MouseRelativeMode )
				{
					if( !modify_Activated )
					{
						Vector2 dummy1;
						Radian dummy2;
						axis = GetAxisByMousePosition( out dummy1, out dummy2 );
					}
					else
						axis = selectedAxis;
				}

				Vector3 position;
				if( !modify_Activated )
					position = Owner.GetPosition();
				else
					position = modifyPosition;
				double lineThickness = GetLineWorldThickness( position );
				double radius = GetSize();
				if( radius == 0 )
					return;

				//DebugGeometry.SetSpecialDepthSettings( false, true );

				const double step = MathEx.PI / 64;

				double innerRadius = radius * .75f;
				double cameraDistance = GetCameraDistance( position );
				Vector3 cameraRight = Vector3.Cross( CameraSettings.Direction, CameraSettings.Up );
				Vector3 oldPos = Vector3.Zero;
				Quaternion startRotation = GetStartObjectsRotation();

				for( int nDrawStep = 0; nDrawStep < 5; nDrawStep++ )
				{
					bool drawShadows = nDrawStep <= 3;
					if( drawShadows && ProjectSettings.Get.TransformToolShadowIntensity == 0 )
						continue;

					var drawShadowsFactor = 0.0;
					if( drawShadows )
						drawShadowsFactor = ( (double)nDrawStep + 1.0 ) / 4.0;

					if( drawShadows )
						DebugGeometry.SetColor( shadowColor * new ColorValue( 1, 1, 1, 0.25 ), false );//, true );

					//Fill inner circle
					if( axis == Axis.InnerCircle && !drawShadows )
					{
						if( !drawShadows )
							DebugGeometry.SetColor( new ColorValue( 1, 1, 0, .4f ), false );//, true );
																							//DebugGeometry.Color = new ColorValue( .2f, .2f, .2f, .33f );

						List<Vector3> vertices = new List<Vector3>();

						for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
						{
							Vector3 pos = position;
							pos += cameraRight * Math.Cos( angle ) * innerRadius;
							pos += CameraSettings.Up * Math.Sin( angle ) * innerRadius;

							if( angle != 0 )
							{
								int verticesCount = vertices.Count;

								vertices.Add( position );
								vertices.Add( oldPos );
								vertices.Add( pos );
							}
							oldPos = pos;
						}

						DebugGeometry.AddTriangles( vertices, Matrix4.Identity, false, false );
					}

					//!!!!тут?
					//render axes of objects
					if( !drawShadows && Owner.Objects.Count <= 30 )
					{
						foreach( TransformToolObject gizmoObject in Owner.Objects )
						{
							var size = radius / 3;
							double alpha = 1;// 0.333;
							double thickness = GetLineWorldThickness( gizmoObject.Position );

							Matrix4 transform = new Matrix4( gizmoObject.Rotation.ToMatrix3() * Matrix3.FromScale( new Vector3( size, size, size ) ), gizmoObject.Position );

							var headHeight = size / 4;
							DebugGeometry.SetColor( new ColorValue( 1, 0, 0, alpha ), false );//, true );
							DebugGeometry.AddArrow( transform * Vector3.Zero, transform * Vector3.XAxis, headHeight, 0, true, thickness );
							DebugGeometry.SetColor( new ColorValue( 0, 1, 0, alpha ), false );//, true );
							DebugGeometry.AddArrow( transform * Vector3.Zero, transform * Vector3.YAxis, headHeight, 0, true, thickness );
							DebugGeometry.SetColor( new ColorValue( 0, 0, 1, alpha ), false );//, true );
							DebugGeometry.AddArrow( transform * Vector3.Zero, transform * Vector3.ZAxis, headHeight, 0, true, thickness );
						}
					}

					//Inner circle
					{
						List<Vector3> points = new List<Vector3>( 64 );
						for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
						{
							Vector3 pos = position;
							pos += cameraRight * Math.Cos( angle ) * innerRadius;
							pos += CameraSettings.Up * Math.Sin( angle ) * innerRadius;
							points.Add( pos );
						}
						if( !drawShadows )
							DebugGeometry.SetColor( darkGray, false );//, true );
						AddPolygonalChain( points.ToArray(), lineThickness, drawShadowsFactor );
					}
					//if( !drawShadows )
					//   DebugGeometry.Color = darkGray;
					//for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
					//{
					//   Vec3 pos = position;
					//   pos += cameraRight * Math.Cos( angle ) * innerRadius;
					//   pos += camera.Up * Math.Sin( angle ) * innerRadius;
					//   if( angle != 0 )
					//      AddLine( oldPos, pos, lineThickness, drawShadows );
					//   oldPos = pos;
					//}

					//Radius
					if( !Owner.SceneMode2D )
					{
						List<Vector3> points = new List<Vector3>( 64 );
						for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
						{
							Vector3 pos = position;
							pos += cameraRight * Math.Cos( angle ) * radius;
							pos += CameraSettings.Up * Math.Sin( angle ) * radius;
							points.Add( pos );
						}
						if( !drawShadows )
							DebugGeometry.SetColor( ( axis == Axis.Radius ) ? yellow : gray, false );//, true );
						AddPolygonalChain( points.ToArray(), lineThickness, drawShadowsFactor );
					}
					//if( !drawShadows )
					//   DebugGeometry.Color = ( axis == Axis.Radius ) ? yellow : gray;
					//for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
					//{
					//   Vec3 pos = position;
					//   pos += cameraRight * Math.Cos( angle ) * radius;
					//   pos += camera.Up * Math.Sin( angle ) * radius;
					//   if( angle != 0 )
					//      AddLine( oldPos, pos, lineThickness, drawShadows );
					//   oldPos = pos;
					//}

					//X
					{
						List<Vector3> points = new List<Vector3>( 64 );
						List<bool> skipLines = new List<bool>( 64 );
						for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
						{
							Vector3 pos = position + startRotation * ( new Vector3( 0, Math.Sin( angle ),
								Math.Cos( angle ) ) * innerRadius );
							points.Add( pos );
							bool skip = GetCameraDistance( pos ) > cameraDistance;
							skipLines.Add( skip );
							//if( angle != 0 && GetCameraDistance( pos ) <= cameraDistance )
							//   points.Add( pos );
						}
						if( !drawShadows )
							DebugGeometry.SetColor( ( axis == Axis.X ) ? yellow : red, false );//, true );

						List<Vector3> newPoints = new List<Vector3>( 64 );
						for( int n = 0; n < points.Count; n++ )
						{
							if( !skipLines[ n ] )
							{
								newPoints.Add( points[ n ] );
							}
							else
							{
								if( newPoints.Count != 0 )
								{
									AddPolygonalChain( newPoints.ToArray(), lineThickness, drawShadowsFactor );
									newPoints.Clear();
								}
							}
						}
						if( newPoints.Count != 0 )
						{
							AddPolygonalChain( newPoints.ToArray(), lineThickness, drawShadowsFactor );
							newPoints.Clear();
						}
					}
					//for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
					//{
					//   Vec3 pos = position + startRotation * ( new Vec3( 0, Math.Sin( angle ),
					//      Math.Cos( angle ) ) * innerRadius );
					//   if( angle != 0 && GetCameraDistance( pos ) <= cameraDistance )
					//      AddLine( oldPos, pos, lineThickness, drawShadows );
					//   oldPos = pos;
					//}

					//Y
					{
						List<Vector3> points = new List<Vector3>( 64 );
						List<bool> skipLines = new List<bool>( 64 );
						for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
						{
							Vector3 pos = position + startRotation * ( new Vector3( Math.Sin( angle ), 0,
								Math.Cos( angle ) ) * innerRadius );
							points.Add( pos );
							bool skip = GetCameraDistance( pos ) > cameraDistance;
							skipLines.Add( skip );
							//if( angle != 0 && GetCameraDistance( pos ) <= cameraDistance )
							//   points.Add( pos );
						}
						if( !drawShadows )
							DebugGeometry.SetColor( ( axis == Axis.Y ) ? yellow : green, false );//, true );

						List<Vector3> newPoints = new List<Vector3>( 64 );
						for( int n = 0; n < points.Count; n++ )
						{
							if( !skipLines[ n ] )
							{
								newPoints.Add( points[ n ] );
							}
							else
							{
								if( newPoints.Count != 0 )
								{
									AddPolygonalChain( newPoints.ToArray(), lineThickness, drawShadowsFactor );
									newPoints.Clear();
								}
							}
						}
						if( newPoints.Count != 0 )
						{
							AddPolygonalChain( newPoints.ToArray(), lineThickness, drawShadowsFactor );
							newPoints.Clear();
						}
					}
					//if( !drawShadows )
					//   DebugGeometry.Color = ( axis == Axis.Y ) ? yellow : green;
					//for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
					//{
					//   Vec3 pos = position + startRotation * ( new Vec3( Math.Sin( angle ), 0,
					//      Math.Cos( angle ) ) * innerRadius );
					//   if( angle != 0 && GetCameraDistance( pos ) <= cameraDistance )
					//      AddLine( oldPos, pos, lineThickness, drawShadows );
					//   oldPos = pos;
					//}

					//Z
					{
						List<Vector3> points = new List<Vector3>( 64 );
						List<bool> skipLines = new List<bool>( 64 );
						for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
						{
							Vector3 pos = position + startRotation * ( new Vector3( Math.Sin( angle ),
								Math.Cos( angle ), 0 ) * innerRadius );
							points.Add( pos );
							bool skip = GetCameraDistance( pos ) > cameraDistance;
							if( Owner.SceneMode2D )
								skip = false;
							skipLines.Add( skip );
							//if( angle != 0 && GetCameraDistance( pos ) <= cameraDistance )
							//   points.Add( pos );
						}
						if( !drawShadows )
							DebugGeometry.SetColor( ( axis == Axis.Z ) ? yellow : blue, false );//, true );

						List<Vector3> newPoints = new List<Vector3>( 64 );
						for( int n = 0; n < points.Count; n++ )
						{
							if( !skipLines[ n ] )
							{
								newPoints.Add( points[ n ] );
							}
							else
							{
								if( newPoints.Count != 0 )
								{
									AddPolygonalChain( newPoints.ToArray(), lineThickness, drawShadowsFactor );
									newPoints.Clear();
								}
							}
						}
						if( newPoints.Count != 0 )
						{
							AddPolygonalChain( newPoints.ToArray(), lineThickness, drawShadowsFactor );
							newPoints.Clear();
						}
					}
					//if( !drawShadows )
					//   DebugGeometry.Color = ( axis == Axis.Z ) ? yellow : blue;
					//for( double angle = 0; angle <= MathEx.PI * 2 + step * .5f; angle += step )
					//{
					//   Vec3 pos = position + startRotation * ( new Vec3( Math.Sin( angle ),
					//      Math.Cos( angle ), 0 ) * innerRadius );
					//   if( angle != 0 && GetCameraDistance( pos ) <= cameraDistance )
					//      AddLine( oldPos, pos, lineThickness, drawShadows );
					//   oldPos = pos;
					//}

					//Arrows
					if( axis != Axis.None && axis != Axis.InnerCircle )
					{
						Plane plane = new Plane();
						switch( axis )
						{
						case Axis.X: plane = Plane.FromPointAndNormal( position, GetStartObjectsRotation() * Vector3.XAxis ); break;
						case Axis.Y: plane = Plane.FromPointAndNormal( position, GetStartObjectsRotation() * Vector3.YAxis ); break;
						case Axis.Z: plane = Plane.FromPointAndNormal( position, GetStartObjectsRotation() * Vector3.ZAxis ); break;
						case Axis.Radius: plane = Plane.FromPointAndNormal( position, CameraSettings.Direction ); break;
						}

						Vector2 mouse;
						if( modify_Activated || initialObjectsTransform != null )
							mouse = mouseStartPosition;
						else
							mouse = Viewport.MousePosition;
						Ray ray = CameraSettings.GetRayByScreenCoordinates( mouse );

						Vector3 planeIntersection;
						if( plane.Intersects( ray, out planeIntersection ) )
						{
							Vector3 direction = ( planeIntersection - position ).GetNormalize();
							Vector3 arrowCenter;
							if( axis == Axis.Radius )
								arrowCenter = position + direction * radius;
							else
								arrowCenter = position + direction * innerRadius;

							double arrowLength = radius * .3f;

							Vector3 tangent = Vector3.Cross( direction, plane.Normal );
							Vector3 arrowPosition1 = arrowCenter + tangent * arrowLength;
							Vector3 arrowPosition2 = arrowCenter - tangent * arrowLength;

							if( !drawShadows )
								DebugGeometry.SetColor( yellow, false );//, true );

							{
								Vector3 direction1 = ( arrowPosition1 - arrowCenter ).GetNormalize();
								AddCone(
									arrowCenter + direction1 * arrowLength / 2,
									arrowPosition1,
									arrowLength / 8, lineThickness, drawShadowsFactor );
							}
							{
								Vector3 direction2 = ( arrowPosition2 - arrowCenter ).GetNormalize();
								AddCone(
									arrowCenter + direction2 * arrowLength / 2,
									arrowPosition2,
									arrowLength / 8, lineThickness, drawShadowsFactor );
							}
						}
					}
				}

				//DebugGeometry.RestoreDefaultDepthSettings();
			}

			public override void OnRenderUI()
			{
				base.OnRenderUI();

				if( !Owner.IsAllowRotate() )
					return;

				var renderer = Viewport.CanvasRenderer;
				Vector2 viewportSize = Owner.ViewportControl.Viewport.SizeInPixels.ToVector2();

				Axis axis = Axis.None;
				if( !Viewport.MouseRelativeMode )
				{
					if( !modify_Activated && !modify_Prepare )
					{
						Vector2 screenLinePointInPixels;
						Radian screenLineAngle;
						axis = GetAxisByMousePosition( out screenLinePointInPixels, out screenLineAngle );
					}
					else
						axis = selectedAxis;
				}

				//update cursor
				if( rotateCursor != null && axis != Axis.None )
					ViewportControl.OneFrameChangeCursor = rotateCursor;

				//draw help text
				if( modify_Activated && !string.IsNullOrEmpty( helpText ) )
				{
					Vector2 screenPosition;
					if( CameraSettings.ProjectToScreenCoordinates( Owner.GetPosition(), out screenPosition ) )
					{
						double offsetY = ( ArrowPixelSize / viewportSize.Y ) * 1.2f;
						Vector2 pos = new Vector2( screenPosition.X, screenPosition.Y - offsetY );
						AddTextWithShadow( renderer, helpText, pos, EHorizontalAlignment.Center, EVerticalAlignment.Bottom,
							new ColorValue( 1, 1, 0 ), 2 );
					}
				}
			}
		}
	}
}
