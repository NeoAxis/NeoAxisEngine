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
		class ScaleModeClass : ModeClass
		{
			Vector2 startMouseOffset;
			Vector2 startMousePosition;
			TransformOfObject[] initialObjectsTransform;
			Vector3 modifyPosition;
			Axes selectedAxes;
			string helpText = "";

			///////////////

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

			///////////////

			Quaternion GetRotation()
			{
				if( Owner.Objects.Count == 0 )
					return Quaternion.Identity;
				return Owner.Objects[ 0 ].Rotation;
			}

			void GetAxisOffsets( out Vector3 xOffset, out Vector3 yOffset, out Vector3 zOffset )
			{
				Vector3 position = Owner.GetPosition();
				Quaternion rotation = GetRotation();
				double size = GetSize();

				{
					Vector3 plus = position + rotation * new Vector3( size, 0, 0 );
					Vector3 minus = position - rotation * new Vector3( size, 0, 0 );
					Vector3 xPosition = ( ( GetCameraDistance( plus ) < GetCameraDistance( minus ) ) || Owner.SceneMode2D ) ? plus : minus;
					xOffset = xPosition - position;
				}

				{
					Vector3 plus = position + rotation * new Vector3( 0, size, 0 );
					Vector3 minus = position - rotation * new Vector3( 0, size, 0 );
					Vector3 yPosition = ( ( GetCameraDistance( plus ) < GetCameraDistance( minus ) ) || Owner.SceneMode2D ) ? plus : minus;
					yOffset = yPosition - position;
				}

				{
					Vector3 plus = position + rotation * new Vector3( 0, 0, size );
					Vector3 minus = position - rotation * new Vector3( 0, 0, size );
					Vector3 zPosition = ( ( GetCameraDistance( plus ) < GetCameraDistance( minus ) ) || Owner.SceneMode2D ) ? plus : minus;
					zOffset = zPosition - position;
				}
			}

			Axes GetAxesByMousePosition()
			{
				Trace.Assert( Owner.Objects.Count != 0 );

				Vector3 position = Owner.GetPosition();
				double size = GetSize();
				if( size == 0 )
					return new Axes( false, false, false );

				Vector3 xOffset, yOffset, zOffset;
				GetAxisOffsets( out xOffset, out yOffset, out zOffset );

				//XYZ
				{
					Vector3 p1 = position + xOffset * .5f;
					Vector3 p2 = position + yOffset * .5f;
					Vector3 p3 = position + zOffset * .5f;
					if( IsMouseOverTriangle( p3, p1, p2 ) )
						return new Axes( true, true, true );
				}

				//XY
				{
					Vector3 p1 = position + xOffset * .7f;
					Vector3 p2 = position + yOffset * .7f;
					Vector3 p3 = position + xOffset * .5f;
					Vector3 p4 = position + yOffset * .5f;
					if( IsMouseOverTriangle( p3, p1, p2 ) || IsMouseOverTriangle( p2, p4, p3 ) )
						return new Axes( true, true, false );
				}

				//XZ
				{
					Vector3 p1 = position + xOffset * .7f;
					Vector3 p2 = position + zOffset * .7f;
					Vector3 p3 = position + xOffset * .5f;
					Vector3 p4 = position + zOffset * .5f;
					if( IsMouseOverTriangle( p3, p1, p2 ) || IsMouseOverTriangle( p2, p4, p3 ) )
						return new Axes( true, false, true );
				}

				//YZ
				{
					Vector3 p1 = position + yOffset * .7f;
					Vector3 p2 = position + zOffset * .7f;
					Vector3 p3 = position + yOffset * .5f;
					Vector3 p4 = position + zOffset * .5f;
					if( IsMouseOverTriangle( p3, p1, p2 ) || IsMouseOverTriangle( p2, p4, p3 ) )
						return new Axes( false, true, true );
				}

				//X
				if( IsMouseNearLine( position, position + xOffset ) )
					return new Axes( true, false, false );

				//Y
				if( IsMouseNearLine( position, position + yOffset ) )
					return new Axes( false, true, false );

				//Z
				if( IsMouseNearLine( position, position + zOffset ) )
					return new Axes( false, false, true );

				return new Axes( false, false, false );
			}

			double GetCameraDistance( Vector3 pos )
			{
				return ( Viewport.CameraSettings.Position - pos ).Length();
			}

			internal protected override bool OnMouseOverAxis()
			{
				if( !Owner.IsAllowScale() )
					return false;
				Axes axes = GetAxesByMousePosition();
				return axes.TrueCount != 0;
			}

			protected override bool OnTryBeginModify()
			{
				if( !Owner.IsAllowScale() )
					return false;

				Axes axes = GetAxesByMousePosition();
				if( axes.TrueCount != 0 )
				{
					modifyPosition = Owner.GetPosition();
					Vector2 screenPosition;
					if( CameraSettings.ProjectToScreenCoordinates( modifyPosition, out screenPosition ) )
					{
						startMouseOffset = Viewport.MousePosition - screenPosition;
						startMousePosition = Viewport.MousePosition;
						//modifyScale = new Vec3( 1, 1, 1 );
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
						Owner.Objects[ n ].Scale = initialObjectsTransform[ n ].scale;
					}
				}
			}

			public override void OnMouseMove( Vector2 mouse )
			{
				base.OnMouseMove( mouse );

				if( modify_Activated && !Viewport.MouseRelativeMode )
				{
					UpdateCursorTransitionOnScreenBorder();

					Vector2 viewportSize = Owner.ViewportControl.Viewport.SizeInPixels.ToVector2();
					Vector2 mousePosition = mouse + cursorTransitionOnScreenBorderOffset / viewportSize;

					Vector2 screenPosition = mousePosition - startMouseOffset;
					Ray ray = CameraSettings.GetRayByScreenCoordinates( screenPosition );

					Vector3 position = modifyPosition;
					double size = GetSize();

					if( size == 0 )
						return;

					Vector3 xOffset, yOffset, zOffset;
					GetAxisOffsets( out xOffset, out yOffset, out zOffset );

					double offset = 0;

					if( !double.IsNaN( ray.Direction.X ) )
					{
						Vector2 pixelOffset = ( mousePosition - startMousePosition ) * viewportSize;
						double mouseOffsetAngle = Math.Atan2( -pixelOffset.Y, pixelOffset.X );

						if( selectedAxes.TrueCount == 3 )
						{
							offset = mousePosition.Y - startMousePosition.Y;
							offset *= -1;
							offset *= viewportSize.Y;
						}
						else
						{
							Vector2 screenModifyPosition;
							if( CameraSettings.ProjectToScreenCoordinates( modifyPosition, out screenModifyPosition ) )
							{
								screenModifyPosition *= viewportSize;

								offset = 0;

								if( selectedAxes.x )
								{
									Vector2 screenXPosition;
									CameraSettings.ProjectToScreenCoordinates( position + xOffset, out screenXPosition );
									if( screenXPosition.X != 0 && screenXPosition.Y != 0 )
									{
										screenXPosition *= viewportSize;
										Vector2 diff = screenXPosition - screenModifyPosition;
										double diffAngle = Math.Atan2( -diff.Y, diff.X );
										double angle = mouseOffsetAngle - diffAngle;
										angle = MathEx.RadianNormalize360( angle );
										double off = Math.Cos( angle ) * pixelOffset.Length();
										offset += off;
									}
								}

								if( selectedAxes.y )
								{
									Vector2 screenYPosition;
									CameraSettings.ProjectToScreenCoordinates( position + yOffset, out screenYPosition );
									if( screenYPosition.X != 0 && screenYPosition.Y != 0 )
									{
										screenYPosition *= viewportSize;
										Vector2 diff = screenYPosition - screenModifyPosition;
										double diffAngle = Math.Atan2( -diff.Y, diff.X );
										double angle = mouseOffsetAngle - diffAngle;
										angle = MathEx.RadianNormalize360( angle );
										double off = Math.Cos( angle ) * pixelOffset.Length();
										offset += off;
									}
								}

								if( selectedAxes.z )
								{
									Vector2 screenZPosition;
									CameraSettings.ProjectToScreenCoordinates( position + zOffset, out screenZPosition );
									if( screenZPosition.X != 0 && screenZPosition.Y != 0 )
									{
										screenZPosition *= viewportSize;
										Vector2 diff = screenZPosition - screenModifyPosition;
										double diffAngle = Math.Atan2( -diff.Y, diff.X );
										double angle = mouseOffsetAngle - diffAngle;
										angle = MathEx.RadianNormalize360( angle );
										double off = Math.Cos( angle ) * pixelOffset.Length();
										offset += off;
									}
								}
							}
						}

						const double scaleCoefficient = .01f;
						offset *= scaleCoefficient;


						double coef;
						if( offset > 0 )
						{
							double snap = Owner.GetSnapScale();
							if( snap != 0 )
							{
								offset += snap / 2;
								offset /= snap;
								offset = (int)offset;
								offset *= snap;
							}

							coef = offset + 1.0f;
						}
						else
						{
							double snap = Owner.GetSnapScale();
							if( snap != 0 )
							{
								offset -= snap / 2;
								offset /= snap;
								offset = (int)offset;
								offset *= snap;
							}

							coef = 1.0f / ( 1.0f - offset );
						}

						Vector3 scaleOffset = new Vector3( selectedAxes.x ? coef : 1, selectedAxes.y ? coef : 1, selectedAxes.z ? coef : 1 );

						//update objects
						if( initialObjectsTransform != null && Owner.Objects.Count == initialObjectsTransform.Length )
							Owner.OnScaleModeUpdateObjects( initialObjectsTransform, modifyPosition, scaleOffset );

						helpText = string.Format( "[{0} {1} {2}]", scaleOffset.X.ToString( "F2" ), scaleOffset.Y.ToString( "F2" ),
							scaleOffset.Z.ToString( "F2" ) );
					}
				}
			}

			public override bool OnIsMouseOverAxisToActivation()
			{
				if( !Owner.IsAllowScale() )
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

			public override void OnRender()
			{
				base.OnRender();

				if( !Owner.IsAllowScale() )
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

				//!!!!review. везде в этом файле
				//DebugGeometry.SetSpecialDepthSettings( false, true );

				Vector3 xOffset, yOffset, zOffset;
				GetAxisOffsets( out xOffset, out yOffset, out zOffset );

				//Arrows
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

					//X
					if( !drawShadows )
						DebugGeometry.SetColor( ( axes.x && axes.TrueCount != 3 ) ? yellow : red, false );//, true );
					AddLine( position, position + xOffset, lineThickness, drawShadowsFactor );
					AddSphere( new Sphere( position + xOffset, size / 80 ), lineThickness, drawShadowsFactor );
					AddSphere( new Sphere( position + xOffset, size / 40 ), lineThickness, drawShadowsFactor );

					if( !drawShadows )
						DebugGeometry.SetColor( axes.x && axes.y ? yellow : red, false );//, true );
					AddLine( position + xOffset * .7f, position + ( xOffset * .7f + yOffset * .7f ) / 2,
						 lineThickness, drawShadowsFactor );
					AddLine( position + xOffset * .5f, position + ( xOffset * .5f + yOffset * .5f ) / 2,
						 lineThickness, drawShadowsFactor );
					if( !drawShadows )
						DebugGeometry.SetColor( axes.x && axes.z ? yellow : red, false );//, true );
					AddLine( position + xOffset * .7f, position + ( xOffset * .7f + zOffset * .7f ) / 2,
						 lineThickness, drawShadowsFactor );
					AddLine( position + xOffset * .5f, position + ( xOffset * .5f + zOffset * .5f ) / 2,
						 lineThickness, drawShadowsFactor );

					//Y
					if( !drawShadows )
						DebugGeometry.SetColor( ( axes.y && axes.TrueCount != 3 ) ? yellow : green, false );//, true );
					AddLine( position, position + yOffset,
						 lineThickness, drawShadowsFactor );
					AddSphere( new Sphere( position + yOffset, size / 80 ), lineThickness, drawShadowsFactor );
					AddSphere( new Sphere( position + yOffset, size / 40 ), lineThickness, drawShadowsFactor );

					if( !drawShadows )
						DebugGeometry.SetColor( axes.y && axes.x ? yellow : green, false );//, true );
					AddLine( position + yOffset * .7f, position + ( yOffset * .7f + xOffset * .7f ) / 2,
						 lineThickness, drawShadowsFactor );
					AddLine( position + yOffset * .5f, position + ( yOffset * .5f + xOffset * .5f ) / 2,
						 lineThickness, drawShadowsFactor );
					if( !drawShadows )
						DebugGeometry.SetColor( axes.y && axes.z ? yellow : green, false );//, true );
					AddLine( position + yOffset * .7f, position + ( yOffset * .7f + zOffset * .7f ) / 2,
						 lineThickness, drawShadowsFactor );
					AddLine( position + yOffset * .5f, position + ( yOffset * .5f + zOffset * .5f ) / 2,
						 lineThickness, drawShadowsFactor );

					//Z
					if( !drawShadows )
						DebugGeometry.SetColor( ( axes.z && axes.TrueCount != 3 ) ? yellow : blue, false );//, true );
					AddLine( position, position + zOffset,
						lineThickness, drawShadowsFactor );
					AddSphere( new Sphere( position + zOffset, size / 80 ), lineThickness, drawShadowsFactor );
					AddSphere( new Sphere( position + zOffset, size / 40 ), lineThickness, drawShadowsFactor );

					if( !drawShadows )
						DebugGeometry.SetColor( axes.z && axes.x ? yellow : blue, false );//, true );
					AddLine( position + zOffset * .7f, position + ( zOffset * .7f + xOffset * .7f ) / 2,
						 lineThickness, drawShadowsFactor );
					AddLine( position + zOffset * .5f, position + ( zOffset * .5f + xOffset * .5f ) / 2,
						 lineThickness, drawShadowsFactor );
					if( !drawShadows )
						DebugGeometry.SetColor( axes.z && axes.y ? yellow : blue, false );//, true );
					AddLine( position + zOffset * .7f, position + ( zOffset * .7f + yOffset * .7f ) / 2,
						 lineThickness, drawShadowsFactor );
					AddLine( position + zOffset * .5f, position + ( zOffset * .5f + yOffset * .5f ) / 2,
						 lineThickness, drawShadowsFactor );
				}

				//Selected area
				if( axes.TrueCount >= 2 )
				{
					DebugGeometry.SetColor( new ColorValue( 1, 1, 0, .4f ), false );//, true );

					List<Vector3> vertices = new List<Vector3>();
					List<int> indices = new List<int>();

					if( axes.TrueCount == 3 )
					{
						vertices.Add( position + xOffset * .5f );
						vertices.Add( position + yOffset * .5f );
						vertices.Add( position + zOffset * .5f );

						indices.Add( 0 ); indices.Add( 1 ); indices.Add( 2 );
					}
					else
					{
						Vector3 offset1;
						Vector3 offset2;

						if( axes.x )
						{
							offset1 = xOffset;
							offset2 = axes.y ? yOffset : zOffset;
						}
						else
						{
							offset1 = yOffset;
							offset2 = zOffset;
						}

						vertices.Add( position + offset1 * .5f );
						vertices.Add( position + offset1 * .7f );
						vertices.Add( position + offset2 * .7f );
						vertices.Add( position + offset2 * .5f );

						indices.Add( 0 ); indices.Add( 1 ); indices.Add( 2 );
						indices.Add( 2 ); indices.Add( 3 ); indices.Add( 0 );
					}

					DebugGeometry.AddTriangles( vertices, indices, Matrix4.Identity, false, false );
				}

				//DebugGeometry.RestoreDefaultDepthSettings();
			}

			public override void OnRenderUI()
			{
				base.OnRenderUI();

				if( !Owner.IsAllowScale() )
					return;

				var renderer = Viewport.CanvasRenderer;
				Vector2 viewportSize = Viewport.SizeInPixels.ToVector2();

				Axes axes = new Axes( false, false, false );
				if( !Viewport.MouseRelativeMode )
				{
					if( !modify_Activated )
						axes = GetAxesByMousePosition();
					else
						axes = selectedAxes;
				}

				//update cursor
				if( scaleCursor != null && axes.TrueCount != 0 )
					ViewportControl.OneFrameChangeCursor = scaleCursor;

				Vector3 xOffset, yOffset, zOffset;
				GetAxisOffsets( out xOffset, out yOffset, out zOffset );

				Vector3 position = Owner.GetPosition();
				Quaternion rotation = GetRotation();
				double size = GetSize();
				if( size != 0 )
				{
					//draw axis names
					{
						double offsetY = ( ArrowPixelSize / 20.0f ) / viewportSize.Y;

						Vector2 screenPosition;

						if( CameraSettings.ProjectToScreenCoordinates( position + xOffset, out screenPosition ) )
						{
							Vector2 pos = new Vector2( screenPosition.X, screenPosition.Y - offsetY );
							ColorValue color = axes.x ? new ColorValue( 1, 1, 0 ) : new ColorValue( 1, 0, 0 );
							AddTextWithShadow( renderer, "x", pos, EHorizontalAlignment.Center, EVerticalAlignment.Bottom, color, 1 );
						}

						if( CameraSettings.ProjectToScreenCoordinates( position + yOffset, out screenPosition ) )
						{
							Vector2 pos = new Vector2( screenPosition.X, screenPosition.Y - offsetY );
							ColorValue color = axes.y ? new ColorValue( 1, 1, 0 ) : new ColorValue( 0, 1, 0 );
							AddTextWithShadow( renderer, "y", pos, EHorizontalAlignment.Center, EVerticalAlignment.Bottom, color, 1 );
						}

						if( CameraSettings.ProjectToScreenCoordinates( position + zOffset, out screenPosition ) )
						{
							Vector2 pos = new Vector2( screenPosition.X, screenPosition.Y - offsetY );
							ColorValue color = axes.z ? new ColorValue( 1, 1, 0 ) : new ColorValue( 0, 0, 1 );
							AddTextWithShadow( renderer, "z", pos, EHorizontalAlignment.Center, EVerticalAlignment.Bottom, color, 1 );
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
							AddTextWithShadow( renderer, helpText, pos, EHorizontalAlignment.Center, EVerticalAlignment.Bottom, new ColorValue( 1, 1, 0 ), 2 );
						}
					}
				}
			}
		}
	}
}
