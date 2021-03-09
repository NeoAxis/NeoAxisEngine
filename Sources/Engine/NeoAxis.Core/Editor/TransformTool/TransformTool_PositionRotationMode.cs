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
		class PositionRotationModeClass : ModeClass
		{
			Vector3 startPosition;
			Vector2 startMouseOffset;
			TransformOfObject[] initialObjectsTransform;
			Axes selectedAxes;
			string helpText = "";

			Vector3 modifyPosition;
			Axis selectedAxis = Axis.None;
			Vector2 selectedScreenLinePointInPixels;
			Radian selectedScreenLineAngle;
			Vector2 mouseStartPosition;
			//Vec2 mouseStartPositionInPixels;
			//string helpText = "";

			bool modifyIsPosition;

			///////////////

			//move
			struct Axes
			{
				public bool x;
				public bool y;
				public bool z;

				public Axes( bool x, bool y, bool z )
				{
					this.x = x;
					this.y = y;
					this.z = z;
				}

				public int TrueCount
				{
					get { return ( x ? 1 : 0 ) + ( y ? 1 : 0 ) + ( z ? 1 : 0 ); }
				}
			}

			//rotate
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

			struct RenderItem
			{
				public enum ItemType
				{
					Line,
					Cone
				}
				public ItemType type;
				public Vector3 start;
				public Vector3 end;
				public ColorValue color;
				public double coneArrowSize;

				public RenderItem( ItemType type, Vector3 start, Vector3 end, ColorValue color, double coneArrowSize )
				{
					this.type = type;
					this.start = start;
					this.end = end;
					this.color = color;
					this.coneArrowSize = coneArrowSize;
				}

				public RenderItem( ItemType type, Vector3 start, Vector3 end, ColorValue color )
				{
					this.type = type;
					this.start = start;
					this.end = end;
					this.color = color;
					this.coneArrowSize = 0;
				}
			}

			///////////////

			Quaternion GetTransformRotation()
			{
				if( Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
				{
					if( Owner.Objects.Count != 0 )
						return Owner.Objects[ 0 ].Rotation;
				}
				return Quaternion.Identity;
			}

			Axes GetAxesByMousePosition()
			{
				Trace.Assert( Owner.Objects.Count != 0 );

				Vector3 position = Owner.GetPosition();
				double size = GetSize();

				if( size == 0 )
					return new Axes( false, false, false );

				Quaternion rot = GetTransformRotation();

				bool xPlus;
				{
					Vector3 plus = position + rot * new Vector3( size, 0, 0 );
					Vector3 minus = position - rot * new Vector3( size, 0, 0 );
					if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
						xPlus = GetCameraDistance( plus ) < GetCameraDistance( minus );
					else
						xPlus = true;
				}
				bool yPlus;
				{
					Vector3 plus = position + rot * new Vector3( 0, size, 0 );
					Vector3 minus = position - rot * new Vector3( 0, size, 0 );
					if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
						yPlus = GetCameraDistance( plus ) < GetCameraDistance( minus );
					else
						yPlus = true;
				}
				bool zPlus;
				{
					Vector3 plus = position + rot * new Vector3( 0, 0, size );
					Vector3 minus = position - rot * new Vector3( 0, 0, size );
					if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
						zPlus = GetCameraDistance( plus ) < GetCameraDistance( minus );
					else
						zPlus = true;
				}
				double xSize = xPlus ? size : -size;
				double ySize = yPlus ? size : -size;
				double zSize = zPlus ? size : -size;

				//XY, XZ, YZ
				Vector3 pX = position + rot * new Vector3( xSize / 2, 0, 0 );
				Vector3 pY = position + rot * new Vector3( 0, ySize / 2, 0 );
				Vector3 pZ = position + rot * new Vector3( 0, 0, zSize / 2 );
				Vector3 pXY = position + rot * new Vector3( xSize / 2, ySize / 2, 0 );
				Vector3 pXZ = position + rot * new Vector3( xSize / 2, 0, zSize / 2 );
				Vector3 pYZ = position + rot * new Vector3( 0, ySize / 2, zSize / 2 );
				//XY
				if( IsMouseOverTriangle( position, pX, pXY ) || IsMouseOverTriangle( pXY, pY, position ) )
					return new Axes( true, true, false );
				if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
				{
					//XZ
					if( IsMouseOverTriangle( position, pX, pXZ ) || IsMouseOverTriangle( pXZ, pZ, position ) )
						return new Axes( true, false, true );
					//YZ
					if( IsMouseOverTriangle( position, pY, pYZ ) || IsMouseOverTriangle( pYZ, pZ, position ) )
						return new Axes( false, true, true );
				}

				{
					double foundDistance = double.MaxValue;
					Axes foundAxes = new Axes( false, false, false );

					double distance;

					//X
					if( IsMouseNearLine( position + rot * new Vector3( size / 5, 0, 0 ), position + rot * new Vector3( size, 0, 0 ), out _, out _, out distance ) )
					{
						if( distance < foundDistance )
						{
							foundDistance = distance;
							foundAxes = new Axes( true, false, false );
						}
					}
					if( !xPlus )
					{
						if( IsMouseNearLine( position + rot * new Vector3( -size / 5, 0, 0 ), position + rot * new Vector3( -size, 0, 0 ), out _, out _, out distance ) )
						{
							if( distance < foundDistance )
							{
								foundDistance = distance;
								foundAxes = new Axes( true, false, false );
							}
						}
					}

					//Y
					if( IsMouseNearLine( position + rot * new Vector3( 0, size / 5, 0 ), position + rot * new Vector3( 0, size, 0 ), out _, out _, out distance ) )
					{
						if( distance < foundDistance )
						{
							foundDistance = distance;
							foundAxes = new Axes( false, true, false );
						}
					}

					if( !yPlus )
					{
						if( IsMouseNearLine( position + rot * new Vector3( 0, -size / 5, 0 ), position + rot * new Vector3( 0, -size, 0 ), out _, out _, out distance ) )
						{
							if( distance < foundDistance )
							{
								foundDistance = distance;
								foundAxes = new Axes( false, true, false );
							}
						}
					}

					//Z
					if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
					{
						if( IsMouseNearLine( position + rot * new Vector3( 0, 0, size / 5 ), position + rot * new Vector3( 0, 0, size ), out _, out _, out distance ) )
						{
							if( distance < foundDistance )
							{
								foundDistance = distance;
								foundAxes = new Axes( false, false, true );
							}
						}

						if( !zPlus )
						{
							if( IsMouseNearLine( position + rot * new Vector3( 0, 0, -size / 5 ), position + rot * new Vector3( 0, 0, -size ), out _, out _, out distance ) )
							{
								if( distance < foundDistance )
								{
									foundDistance = distance;
									foundAxes = new Axes( false, false, true );
								}
							}
						}
					}

					if( foundAxes.TrueCount != 0 )
						return foundAxes;
				}


				////X
				//if( IsMouseNearLine( position + rot * new Vector3( size / 5, 0, 0 ), position + rot * new Vector3( size, 0, 0 ) ) )
				//	return new Axes( true, false, false );
				//if( !xPlus )
				//{
				//	if( IsMouseNearLine( position + rot * new Vector3( -size / 5, 0, 0 ), position + rot * new Vector3( -size, 0, 0 ) ) )
				//		return new Axes( true, false, false );
				//}

				////Y
				//if( IsMouseNearLine( position + rot * new Vector3( 0, size / 5, 0 ), position + rot * new Vector3( 0, size, 0 ) ) )
				//	return new Axes( false, true, false );
				//if( !yPlus )
				//{
				//	if( IsMouseNearLine( position + rot * new Vector3( 0, -size / 5, 0 ), position + rot * new Vector3( 0, -size, 0 ) ) )
				//		return new Axes( false, true, false );
				//}

				////Z
				//if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
				//{
				//	if( IsMouseNearLine( position + rot * new Vector3( 0, 0, size / 5 ), position + rot * new Vector3( 0, 0, size ) ) )
				//		return new Axes( false, false, true );
				//	if( !zPlus )
				//	{
				//		if( IsMouseNearLine( position + rot * new Vector3( 0, 0, -size / 5 ), position + rot * new Vector3( 0, 0, -size ) ) )
				//			return new Axes( false, false, true );
				//	}
				//}

				return new Axes( false, false, false );
			}

			Quaternion GetStartObjectsRotation()
			{
				if( Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
				{
					if( Owner.Objects.Count != 0 )
					{
						if( modify_Activated )//&& modifyIsPosition )
							return initialObjectsTransform[ 0 ].rotation;
						else
							return Owner.Objects[ 0 ].Rotation;
					}
				}
				return Quaternion.Identity;
			}

			Axis GetAxisByMousePosition( out Vector2 screenLinePointInPixels, out Radian screenLineAngle )
			{
				screenLinePointInPixels = Vector2.Zero;
				screenLineAngle = 0;

				//overlap by position mode
				if( GetAxesByMousePosition().TrueCount != 0 )
					return Axis.None;

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

				return Axis.None;
			}

			internal protected override bool OnMouseOverAxis()
			{
				return Position_OnMouseOverAxis() || Rotation_OnMouseOverAxis();
			}

			bool Position_OnMouseOverAxis()
			{
				if( !Owner.IsAllowMove() )
					return false;
				Axes axes = GetAxesByMousePosition();
				return axes.TrueCount != 0;
			}

			bool Rotation_OnMouseOverAxis()
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
				if( Position_OnTryBeginModify() )
				{
					modifyIsPosition = true;
					return true;
				}

				if( Rotation_OnTryBeginModify() )
				{
					modifyIsPosition = false;
					return true;
				}

				return false;
			}

			bool Position_OnTryBeginModify()
			{
				if( !Owner.IsAllowMove() )
					return false;

				Axes axes = GetAxesByMousePosition();
				if( axes.TrueCount != 0 )
				{
					startPosition = Owner.GetPosition();
					Vector2 screenPosition;
					if( CameraSettings.ProjectToScreenCoordinates( startPosition, out screenPosition ) )
					{
						startMouseOffset = Viewport.MousePosition - screenPosition;
						selectedAxes = axes;
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

				return false;
			}

			public override void OnUpdateInitialObjectsTransform()
			{
				initialObjectsTransform = new TransformOfObject[ Owner.Objects.Count ];
				for( int n = 0; n < Owner.Objects.Count; n++ )
				{
					TransformToolObject obj = Owner.Objects[ n ];
					initialObjectsTransform[ n ] = new TransformOfObject( obj.Position, obj.Rotation, obj.Scale );
				}
			}

			bool Rotation_OnTryBeginModify()
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

				return false;
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

				Position_OnMouseMove();
				Rotation_OnMouseMove();
			}

			void Position_OnMouseMove()
			{
				if( modify_Activated && !Viewport.MouseRelativeMode && modifyIsPosition )
				{
					Vector2 screenPosition = Viewport.MousePosition - startMouseOffset;
					Ray ray = CameraSettings.GetRayByScreenCoordinates( screenPosition );

					if( !double.IsNaN( ray.Direction.X ) )
					{
						Quaternion rot = GetTransformRotation();

						Plane xPlane = Plane.FromVectors( rot * new Vector3( 0, 1, 0 ),
							rot * new Vector3( 0, 0, 1 ), startPosition );
						Plane yPlane = Plane.FromVectors( rot * new Vector3( 1, 0, 0 ),
							rot * new Vector3( 0, 0, 1 ), startPosition );
						Plane zPlane = Plane.FromVectors( rot * new Vector3( 1, 0, 0 ),
							rot * new Vector3( 0, 1, 0 ), startPosition );

						Vector3 offset = Vector3.Zero;

						if( selectedAxes.x && selectedAxes.TrueCount == 1 )
						{
							double scale;
							if( zPlane.Intersects( ray, out scale ) )
							{
								Vector3 p = MathAlgorithms.ProjectPointToLine( startPosition,
									startPosition + rot * new Vector3( 1, 0, 0 ), ray.GetPointOnRay( scale ) );
								offset += p - startPosition;
							}
						}

						if( selectedAxes.y && selectedAxes.TrueCount == 1 )
						{
							double scale;
							if( zPlane.Intersects( ray, out scale ) )
							{
								Vector3 p = MathAlgorithms.ProjectPointToLine( startPosition,
									startPosition + rot * new Vector3( 0, 1, 0 ), ray.GetPointOnRay( scale ) );
								offset += p - startPosition;
							}
						}

						if( selectedAxes.z && selectedAxes.TrueCount == 1 )
						{
							Vector3 dir = CameraSettings.Direction;

							bool xMore = Math.Abs( CameraSettings.Direction.X ) > Math.Abs( CameraSettings.Direction.Y );
							Plane plane = xMore ? xPlane : yPlane;

							double scale;
							if( plane.Intersects( ray, out scale ) )
							{
								Vector3 p = MathAlgorithms.ProjectPointToLine( startPosition,
									startPosition + rot * new Vector3( 0, 0, 1 ), ray.GetPointOnRay( scale ) );
								offset += p - startPosition;
							}
						}

						if( selectedAxes.x && selectedAxes.y )
						{
							double scale;
							if( zPlane.Intersects( ray, out scale ) )
							{
								Vector3 p1 = MathAlgorithms.ProjectPointToLine( startPosition,
									startPosition + rot * new Vector3( 1, 0, 0 ), ray.GetPointOnRay( scale ) );
								Vector3 p2 = MathAlgorithms.ProjectPointToLine( startPosition,
									startPosition + rot * new Vector3( 0, 1, 0 ), ray.GetPointOnRay( scale ) );
								offset += p1 - startPosition;
								offset += p2 - startPosition;
							}
						}

						if( selectedAxes.x && selectedAxes.z )
						{
							double scale;
							if( yPlane.Intersects( ray, out scale ) )
							{
								Vector3 p1 = MathAlgorithms.ProjectPointToLine( startPosition,
									startPosition + rot * new Vector3( 1, 0, 0 ), ray.GetPointOnRay( scale ) );
								Vector3 p2 = MathAlgorithms.ProjectPointToLine( startPosition,
									startPosition + rot * new Vector3( 0, 0, 1 ), ray.GetPointOnRay( scale ) );
								offset += p1 - startPosition;
								offset += p2 - startPosition;
							}
						}

						if( selectedAxes.y && selectedAxes.z )
						{
							double scale;
							if( xPlane.Intersects( ray, out scale ) )
							{
								Vector3 p1 = MathAlgorithms.ProjectPointToLine( startPosition,
									startPosition + rot * new Vector3( 0, 1, 0 ), ray.GetPointOnRay( scale ) );
								Vector3 p2 = MathAlgorithms.ProjectPointToLine( startPosition,
									startPosition + rot * new Vector3( 0, 0, 1 ), ray.GetPointOnRay( scale ) );
								offset += p1 - startPosition;
								offset += p2 - startPosition;
							}
						}

						//snap
						offset *= rot.GetInverse();
						double snap = Owner.GetSnapMove();
						if( snap != 0 )
						{
							Vector3 newOffset = offset;

							Vector3 snapVec = new Vector3( snap, snap, snap );
							newOffset += snapVec / 2;
							newOffset /= snapVec;
							newOffset = new Vector3I( (int)newOffset.X, (int)newOffset.Y,
								(int)newOffset.Z ).ToVector3();
							newOffset *= snapVec;

							if( selectedAxes.x )
								offset.X = newOffset.X;
							if( selectedAxes.y )
								offset.Y = newOffset.Y;
							if( selectedAxes.z )
								offset.Z = newOffset.Z;
						}
						offset *= rot;

						//update objects
						if( initialObjectsTransform != null && Owner.Objects.Count == initialObjectsTransform.Length )
							Owner.OnPositionModeUpdateObjects( initialObjectsTransform, offset );

						helpText = string.Format( "[{0} {1} {2}]", offset.X.ToString( "F2" ), offset.Y.ToString( "F2" ),
							offset.Z.ToString( "F2" ) );
					}
				}
			}

			void Rotation_OnMouseMove()
			{
				if( modify_Activated && !owner.viewportControl.Viewport.MouseRelativeMode && !modifyIsPosition )
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

					if( selectedAxis == Axis.Z )
					{
						Radian angle;
						{
							Matrix2 m = Matrix2.FromRotate( selectedScreenLineAngle );
							Vector2 localOffset = m * offset;
							angle = localOffset.X * scaleFactor;
							SnapAngle( ref angle );
						}

						string zeroStr = 0.0f.ToString( "F2" );

						rotationOffset = new Quaternion( new Vector3( 0, 0, Math.Sin( -angle / 2 ) ), Math.Cos( -angle / 2 ) );
						rotationOffset = startRotation * rotationOffset.GetNormalize() * startRotation.GetInverse();
						helpText = string.Format( "[{0} {1} {2}]", zeroStr, zeroStr, ( (double)angle.InDegrees() ).ToString( "F2" ) );
					}

					rotationOffset.Normalize();

					//update objects
					if( initialObjectsTransform != null && Owner.Objects.Count == initialObjectsTransform.Length )
						Owner.OnRotationModeUpdateObjects( initialObjectsTransform, modifyPosition, rotationOffset );
				}
			}

			double GetCameraDistance( Vector3 pos )
			{
				return ( Viewport.CameraSettings.Position - pos ).Length();
			}

			public override bool OnIsMouseOverAxisToActivation()
			{
				return Position_OnIsMouseOverAxisToActivation() || Rotation_OnIsMouseOverAxisToActivation();
			}

			bool Position_OnIsMouseOverAxisToActivation()
			{
				if( !Owner.IsAllowMove() )
					return false;

				Axes axes = new Axes( false, false, false );
				if( !Viewport.MouseRelativeMode )
				{
					if( !modify_Activated )
						axes = GetAxesByMousePosition();
					else
						axes = selectedAxes;
				}

				Vector3 position = Owner.GetPosition();
				double lineThickness = GetLineWorldThickness( position );
				double size = GetSize();
				if( size == 0 )
					return false;

				return axes.TrueCount != 0;
			}

			bool Rotation_OnIsMouseOverAxisToActivation()
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

				Rotation_Render();
				Position_Render();
			}

			void Position_Render()
			{
				if( !Owner.IsAllowMove() )
					return;

				ColorValue yellow = new ColorValue( 1, 1, 0 );
				ColorValue red = new ColorValue( 1, 0, 0 );
				ColorValue green = new ColorValue( 0, 1, 0 );
				ColorValue blue = new ColorValue( 0, 0, 1 );
				ColorValue shadowColor = new ColorValue( 0, 0, 0, ProjectSettings.Get.TransformToolShadowIntensity );

				Axes axes = new Axes( false, false, false );
				if( !Viewport.MouseRelativeMode )
				{
					if( !modify_Activated )
						axes = GetAxesByMousePosition();
					else
						axes = selectedAxes;
				}

				Vector3 position = Owner.GetPosition();
				double lineThickness = GetLineWorldThickness( position );
				double size = GetSize();
				if( size == 0 )
					return;

				//DebugGeometry.SetSpecialDepthSettings( false, true );

				double arrowSize = size / 4.0f;
				Quaternion rot = GetTransformRotation();

				bool xPlus;
				{
					Vector3 plus = position + rot * new Vector3( size, 0, 0 );
					Vector3 minus = position - rot * new Vector3( size, 0, 0 );
					if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
						xPlus = GetCameraDistance( plus ) < GetCameraDistance( minus );
					else
						xPlus = true;
				}
				bool yPlus;
				{
					Vector3 plus = position + rot * new Vector3( 0, size, 0 );
					Vector3 minus = position - rot * new Vector3( 0, size, 0 );
					if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
						yPlus = GetCameraDistance( plus ) < GetCameraDistance( minus );
					else
						yPlus = true;
				}
				bool zPlus;
				{
					Vector3 plus = position + rot * new Vector3( 0, 0, size );
					Vector3 minus = position - rot * new Vector3( 0, 0, size );
					if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
						zPlus = GetCameraDistance( plus ) < GetCameraDistance( minus );
					else
						zPlus = true;
				}

				double xSize = xPlus ? size : -size;
				double ySize = yPlus ? size : -size;
				double zSize = zPlus ? size : -size;

				double size2 = size - lineThickness;
				double xSize2 = xPlus ? size2 : -size2;
				double ySize2 = yPlus ? size2 : -size2;
				double zSize2 = zPlus ? size2 : -size2;

				//Arrows
				{
					List<RenderItem> items = new List<RenderItem>( 32 );
					ColorValue color;

					//X

					color = axes.x ? yellow : red;
					items.Add( new RenderItem( RenderItem.ItemType.Line,
						position + rot * new Vector3( size / 5, 0, 0 ),
						position + rot * new Vector3( size - arrowSize, 0, 0 ),
						color ) );
					items.Add( new RenderItem( RenderItem.ItemType.Cone,
						position + rot * new Vector3( size - arrowSize, 0, 0 ),
						position + rot * new Vector3( size, 0, 0 ),
						color, arrowSize / 6 ) );
					if( !xPlus )
					{
						items.Add( new RenderItem( RenderItem.ItemType.Line,
							position + rot * new Vector3( -size / 5, 0, 0 ),
							position + rot * new Vector3( -size, 0, 0 ),
							color, lineThickness ) );
					}

					color = axes.x && axes.y ? yellow : red;
					items.Add( new RenderItem( RenderItem.ItemType.Line,
						position + rot * new Vector3( xSize / 2, 0, 0 ),
						position + rot * new Vector3( xSize / 2, ySize2 / 2, 0 ),
						color, lineThickness ) );
					if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
					{
						color = axes.x && axes.z ? yellow : red;
						items.Add( new RenderItem( RenderItem.ItemType.Line,
							position + rot * new Vector3( xSize / 2, 0, 0 ),
							position + rot * new Vector3( xSize / 2, 0, zSize2 / 2 ),
							color, lineThickness ) );
					}

					//Y

					color = axes.y ? yellow : green;
					items.Add( new RenderItem( RenderItem.ItemType.Line,
						position + rot * new Vector3( 0, size / 5, 0 ),
						position + rot * new Vector3( 0, size - arrowSize, 0 ),
						color, lineThickness ) );
					items.Add( new RenderItem( RenderItem.ItemType.Cone,
						position + rot * new Vector3( 0, size - arrowSize, 0 ),
						position + rot * new Vector3( 0, size, 0 ),
						color, arrowSize / 6 ) );
					if( !yPlus )
					{
						items.Add( new RenderItem( RenderItem.ItemType.Line,
							position + rot * new Vector3( 0, -size / 5, 0 ),
							position + rot * new Vector3( 0, -size, 0 ),
							color, lineThickness ) );
					}

					color = axes.x && axes.y ? yellow : green;
					items.Add( new RenderItem( RenderItem.ItemType.Line,
						position + rot * new Vector3( 0, ySize / 2, 0 ),
						position + rot * new Vector3( xSize2 / 2, ySize / 2, 0 ),
						color, lineThickness ) );
					if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
					{
						color = axes.y && axes.z ? yellow : green;
						items.Add( new RenderItem( RenderItem.ItemType.Line,
							position + rot * new Vector3( 0, ySize / 2, 0 ),
							position + rot * new Vector3( 0, ySize / 2, zSize2 / 2 ),
							color, lineThickness ) );
					}

					//Z

					if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
					{
						color = axes.z ? yellow : blue;
						items.Add( new RenderItem( RenderItem.ItemType.Line,
							position + rot * new Vector3( 0, 0, size / 5 ),
							position + rot * new Vector3( 0, 0, size - arrowSize ),
							color, lineThickness ) );
						items.Add( new RenderItem( RenderItem.ItemType.Cone,
							position + rot * new Vector3( 0, 0, size - arrowSize ),
							position + rot * new Vector3( 0, 0, size ),
							color, arrowSize / 6 ) );
						if( !zPlus )
						{
							items.Add( new RenderItem( RenderItem.ItemType.Line,
								position + rot * new Vector3( 0, 0, -size / 5 ),
								position + rot * new Vector3( 0, 0, -size ),
								color, lineThickness ) );
						}

						color = axes.x && axes.z ? yellow : blue;
						items.Add( new RenderItem( RenderItem.ItemType.Line,
							position + rot * new Vector3( 0, 0, zSize / 2 ),
							position + rot * new Vector3( xSize2 / 2, 0, zSize / 2 ),
							color, lineThickness ) );
						color = axes.y && axes.z ? yellow : blue;
						items.Add( new RenderItem( RenderItem.ItemType.Line,
							position + rot * new Vector3( 0, 0, zSize / 2 ),
							position + rot * new Vector3( 0, ySize2 / 2, zSize / 2 ),
							color, lineThickness ) );
					}

					//sort by distance
					CollectionUtility.SelectionSort( items, delegate ( RenderItem item1, RenderItem item2 )
					{
						double distance1 = Math.Max( GetCameraDistance( item1.start ), GetCameraDistance( item1.end ) );
						double distance2 = Math.Max( GetCameraDistance( item2.start ), GetCameraDistance( item2.end ) );
						if( distance1 > distance2 )
							return -1;
						if( distance1 < distance2 )
							return 1;
						return 0;
					} );

					//render items
					for( int nDrawStep = 0; nDrawStep < 5; nDrawStep++ )
					{
						bool drawShadows = nDrawStep <= 3;
						if( drawShadows && ProjectSettings.Get.TransformToolShadowIntensity == 0 )
							continue;

						var drawShadowsFactor = 0.0;
						if( drawShadows )
							drawShadowsFactor = ( (double)nDrawStep + 1.0 ) / 4.0;

						//if( drawShadows )
						//	DebugGeometry.EnableNonOverlappingGroup();

						if( drawShadows )
							DebugGeometry.SetColor( shadowColor * new ColorValue( 1, 1, 1, 0.25 ), false );//, true );
						foreach( RenderItem item in items )
						{
							if( !drawShadows )
								DebugGeometry.SetColor( item.color, false );//, true );
							switch( item.type )
							{
							case RenderItem.ItemType.Line:
								AddLine( item.start, item.end, lineThickness, drawShadowsFactor );
								break;
							case RenderItem.ItemType.Cone:
								AddCone( item.start, item.end, item.coneArrowSize, lineThickness, drawShadowsFactor );
								break;
							}
						}

						//if( drawShadows )
						//	DebugGeometry.DisableNonOverlappingGroup();
					}
				}

				//Selected square
				if( axes.TrueCount == 2 )
				{
					DebugGeometry.SetColor( new ColorValue( 1, 1, 0, .4f ), false );//, true );

					List<Vector3> vertices = new List<Vector3>();
					List<int> indices = new List<int>();

					Vector3 offset1;
					Vector3 offset2;

					if( axes.x )
					{
						offset1 = Vector3.XAxis;
						offset2 = axes.y ? Vector3.YAxis : Vector3.ZAxis;
					}
					else
					{
						offset1 = Vector3.YAxis;
						offset2 = Vector3.ZAxis;
					}

					offset1 *= new Vector3( xSize / 2, ySize / 2, zSize / 2 );
					offset2 *= new Vector3( xSize / 2, ySize / 2, zSize / 2 );

					vertices.Add( position );
					vertices.Add( position + rot * offset1 );
					vertices.Add( position + rot * ( offset1 + offset2 ) );
					vertices.Add( position + rot * offset2 );

					indices.Add( 0 ); indices.Add( 1 ); indices.Add( 2 );
					indices.Add( 2 ); indices.Add( 3 ); indices.Add( 0 );

					DebugGeometry.AddTriangles( vertices, indices,
						Matrix4.Identity, false, false );
				}

				//DebugGeometry.RestoreDefaultDepthSettings();
			}

			void Rotation_Render()
			{
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
					if( !modify_Activated || modifyIsPosition )
					{
						Vector2 dummy1;
						Radian dummy2;
						axis = GetAxisByMousePosition( out dummy1, out dummy2 );
					}
					else
						axis = selectedAxis;
				}

				Vector3 position;
				if( !modify_Activated || modifyIsPosition )
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

					////!!!!тут?
					////render axes of objects
					//if( !drawShadows && Owner.Objects.Count <= 30 )
					//{
					//	foreach( TransformToolObject gizmoObject in Owner.Objects )
					//	{
					//		var size = radius / 3;
					//		double alpha = 1;// 0.333;
					//		double thickness = GetLineWorldThickness( gizmoObject.Position );

					//		Matrix4 transform = new Matrix4( gizmoObject.Rotation.ToMatrix3() * Matrix3.FromScale( new Vector3( size, size, size ) ), gizmoObject.Position );

					//		var headHeight = size / 4;
					//		DebugGeometry.SetColor( new ColorValue( 1, 0, 0, alpha ), false );//, true );
					//		DebugGeometry.AddArrow( transform * Vector3.Zero, transform * Vector3.XAxis, headHeight, 0, true, thickness );
					//		DebugGeometry.SetColor( new ColorValue( 0, 1, 0, alpha ), false );//, true );
					//		DebugGeometry.AddArrow( transform * Vector3.Zero, transform * Vector3.YAxis, headHeight, 0, true, thickness );
					//		DebugGeometry.SetColor( new ColorValue( 0, 0, 1, alpha ), false );//, true );
					//		DebugGeometry.AddArrow( transform * Vector3.Zero, transform * Vector3.ZAxis, headHeight, 0, true, thickness );
					//	}
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

					////Arrows
					//if( axis != Axis.None && axis != Axis.InnerCircle )
					//{
					//	Plane plane = new Plane();
					//	switch( axis )
					//	{
					//	case Axis.X: plane = Plane.FromPointAndNormal( position, GetStartObjectsRotation() * Vector3.XAxis ); break;
					//	case Axis.Y: plane = Plane.FromPointAndNormal( position, GetStartObjectsRotation() * Vector3.YAxis ); break;
					//	case Axis.Z: plane = Plane.FromPointAndNormal( position, GetStartObjectsRotation() * Vector3.ZAxis ); break;
					//	case Axis.Radius: plane = Plane.FromPointAndNormal( position, CameraSettings.Direction ); break;
					//	}

					//	Vector2 mouse;
					//	if( modify_Activated || initialObjectsTransform != null )
					//		mouse = mouseStartPosition;
					//	else
					//		mouse = Viewport.MousePosition;
					//	Ray ray = CameraSettings.GetRayByScreenCoordinates( mouse );

					//	Vector3 planeIntersection;
					//	if( plane.Intersects( ray, out planeIntersection ) )
					//	{
					//		Vector3 direction = ( planeIntersection - position ).GetNormalize();
					//		Vector3 arrowCenter;
					//		if( axis == Axis.Radius )
					//			arrowCenter = position + direction * radius;
					//		else
					//			arrowCenter = position + direction * innerRadius;

					//		double arrowLength = radius * .3f;

					//		Vector3 tangent = Vector3.Cross( direction, plane.Normal );
					//		Vector3 arrowPosition1 = arrowCenter + tangent * arrowLength;
					//		Vector3 arrowPosition2 = arrowCenter - tangent * arrowLength;

					//		if( !drawShadows )
					//			DebugGeometry.SetColor( yellow, false );//, true );

					//		{
					//			Vector3 direction1 = ( arrowPosition1 - arrowCenter ).GetNormalize();
					//			AddCone(
					//				arrowCenter + direction1 * arrowLength / 2,
					//				arrowPosition1,
					//				arrowLength / 8, lineThickness, drawShadows );
					//		}
					//		{
					//			Vector3 direction2 = ( arrowPosition2 - arrowCenter ).GetNormalize();
					//			AddCone(
					//				arrowCenter + direction2 * arrowLength / 2,
					//				arrowPosition2,
					//				arrowLength / 8, lineThickness, drawShadows );
					//		}
					//	}
					//}
				}

				//DebugGeometry.RestoreDefaultDepthSettings();
			}

			public override void OnRenderUI()
			{
				base.OnRenderUI();

				Rotation_RenderUI();
				Position_RenderUI();
			}

			void Position_RenderUI()
			{
				if( !Owner.IsAllowMove() )
					return;

				Vector2 viewportSize = Viewport.SizeInPixels.ToVector2();
				var renderer = Viewport.CanvasRenderer;

				Axes axes = new Axes( false, false, false );
				if( !Viewport.MouseRelativeMode )
				{
					if( !modify_Activated )
						axes = GetAxesByMousePosition();
					else
						axes = selectedAxes;
				}

				Vector3 position = Owner.GetPosition();
				Quaternion rot = GetTransformRotation();
				double size = GetSize();

				//update cursor
				if( moveCursor != null && axes.TrueCount != 0 )
					ViewportControl.OneFrameChangeCursor = moveCursor;

				if( size != 0 )
				{
					//draw axis names
					{
						Vector3 xPlus = position + rot * new Vector3( size, 0, 0 );
						Vector3 yPlus = position + rot * new Vector3( 0, size, 0 );
						Vector3 zPlus = position + rot * new Vector3( 0, 0, size );
						double offsetY = ( ArrowPixelSize / 20.0f ) / viewportSize.Y;

						Vector2 screenPosition;

						if( CameraSettings.ProjectToScreenCoordinates( xPlus, out screenPosition ) )
						{
							Vector2 pos = new Vector2( screenPosition.X, screenPosition.Y - offsetY );
							ColorValue color = axes.x ? new ColorValue( 1, 1, 0 ) : new ColorValue( 1, 0, 0 );
							AddTextWithShadow( renderer, "x", pos, EHorizontalAlignment.Center, EVerticalAlignment.Bottom, color, 1 );
						}

						if( CameraSettings.ProjectToScreenCoordinates( yPlus, out screenPosition ) )
						{
							Vector2 pos = new Vector2( screenPosition.X, screenPosition.Y - offsetY );
							ColorValue color = axes.y ? new ColorValue( 1, 1, 0 ) : new ColorValue( 0, 1, 0 );
							AddTextWithShadow( renderer, "y", pos, EHorizontalAlignment.Center, EVerticalAlignment.Bottom, color, 1 );
						}

						if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
						{
							if( CameraSettings.ProjectToScreenCoordinates( zPlus, out screenPosition ) )
							{
								Vector2 pos = new Vector2( screenPosition.X, screenPosition.Y - offsetY );
								ColorValue color = axes.z ? new ColorValue( 1, 1, 0 ) : new ColorValue( 0, 0, 1 );
								AddTextWithShadow( renderer, "z", pos, EHorizontalAlignment.Center, EVerticalAlignment.Bottom, color, 1 );
							}
						}
					}

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

			void Rotation_RenderUI()
			{
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
