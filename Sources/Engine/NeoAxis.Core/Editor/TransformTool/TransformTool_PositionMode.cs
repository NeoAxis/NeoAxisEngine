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
		class PositionModeClass : ModeClass
		{
			Vector3 startPosition;
			Vector2 startMouseOffset;
			TransformOfObject[] initialObjectsTransform;
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

			internal protected override bool OnMouseOverAxis()
			{
				if( !Owner.IsAllowMove() )
					return false;
				Axes axes = GetAxesByMousePosition();
				return axes.TrueCount != 0;
			}

			protected override bool OnTryBeginModify()
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

			public override void OnMouseMove( Vector2 mouse )
			{
				base.OnMouseMove( mouse );

				if( modify_Activated && !Viewport.MouseRelativeMode )
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

			double GetCameraDistance( Vector3 pos )
			{
				return ( Viewport.CameraSettings.Position - pos ).Length();
			}

			public override bool OnIsMouseOverAxisToActivation()
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

			public override void OnRender()
			{
				base.OnRender();

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

			public override void OnRenderUI()
			{
				base.OnRenderUI();

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
					////draw gizmo
					//{
					//   ColorValue yellow = new ColorValue( 1, 1, 0 );
					//   ColorValue red = new ColorValue( 1, 0, 0 );
					//   ColorValue green = new ColorValue( 0, 1, 0 );
					//   ColorValue blue = new ColorValue( 0, 0, 1 );
					//   ColorValue shadowColor = new ColorValue( 0, 0, 0, GizmoConfig.shadowIntensity );

					//   double lineThickness = GetLineWorldThickness( position );

					//   double arrowSize = size / 4.0f;
					//   //Quat rot = GetTransformRotation();

					//   bool xPlus;
					//   {
					//      Vec3 plus = position + rot * new Vec3( size, 0, 0 );
					//      Vec3 minus = position - rot * new Vec3( size, 0, 0 );
					//      xPlus = GetCameraDistance( plus ) < GetCameraDistance( minus );
					//   }
					//   bool yPlus;
					//   {
					//      Vec3 plus = position + rot * new Vec3( 0, size, 0 );
					//      Vec3 minus = position - rot * new Vec3( 0, size, 0 );
					//      yPlus = GetCameraDistance( plus ) < GetCameraDistance( minus );
					//   }
					//   bool zPlus;
					//   {
					//      Vec3 plus = position + rot * new Vec3( 0, 0, size );
					//      Vec3 minus = position - rot * new Vec3( 0, 0, size );
					//      zPlus = GetCameraDistance( plus ) < GetCameraDistance( minus );
					//   }

					//   double xSize = xPlus ? size : -size;
					//   double ySize = yPlus ? size : -size;
					//   double zSize = zPlus ? size : -size;

					//   //Arrows
					//   {
					//      List<RenderItem> items = new List<RenderItem>( 32 );
					//      ColorValue color;

					//      //X

					//      color = axes.x ? yellow : red;
					//      items.Add( new RenderItem( RenderItem.ItemTypes.Line,
					//         position + rot * new Vec3( size / 5, 0, 0 ),
					//         position + rot * new Vec3( size - arrowSize, 0, 0 ),
					//         color ) );
					//      items.Add( new RenderItem( RenderItem.ItemTypes.Cone,
					//         position + rot * new Vec3( size - arrowSize, 0, 0 ),
					//         position + rot * new Vec3( size, 0, 0 ),
					//         color, arrowSize / 6 ) );
					//      if( !xPlus )
					//      {
					//         items.Add( new RenderItem( RenderItem.ItemTypes.Line,
					//            position + rot * new Vec3( -size / 5, 0, 0 ),
					//            position + rot * new Vec3( -size, 0, 0 ),
					//            color, lineThickness ) );
					//      }

					//      color = axes.x && axes.y ? yellow : red;
					//      items.Add( new RenderItem( RenderItem.ItemTypes.Line,
					//         position + rot * new Vec3( xSize / 2, 0, 0 ),
					//         position + rot * new Vec3( xSize / 2, ySize / 2, 0 ),
					//         color, lineThickness ) );

					//      color = axes.x && axes.z ? yellow : red;
					//      items.Add( new RenderItem( RenderItem.ItemTypes.Line,
					//         position + rot * new Vec3( xSize / 2, 0, 0 ),
					//         position + rot * new Vec3( xSize / 2, 0, zSize / 2 ),
					//         color, lineThickness ) );

					//      //Y

					//      color = axes.y ? yellow : green;
					//      items.Add( new RenderItem( RenderItem.ItemTypes.Line,
					//         position + rot * new Vec3( 0, size / 5, 0 ),
					//         position + rot * new Vec3( 0, size - arrowSize, 0 ),
					//         color, lineThickness ) );
					//      items.Add( new RenderItem( RenderItem.ItemTypes.Cone,
					//         position + rot * new Vec3( 0, size - arrowSize, 0 ),
					//         position + rot * new Vec3( 0, size, 0 ),
					//         color, arrowSize / 6 ) );
					//      if( !yPlus )
					//      {
					//         items.Add( new RenderItem( RenderItem.ItemTypes.Line,
					//            position + rot * new Vec3( 0, -size / 5, 0 ),
					//            position + rot * new Vec3( 0, -size, 0 ),
					//            color, lineThickness ) );
					//      }

					//      color = axes.x && axes.y ? yellow : green;
					//      items.Add( new RenderItem( RenderItem.ItemTypes.Line,
					//         position + rot * new Vec3( 0, ySize / 2, 0 ),
					//         position + rot * new Vec3( xSize / 2, ySize / 2, 0 ),
					//         color, lineThickness ) );
					//      color = axes.y && axes.z ? yellow : green;
					//      items.Add( new RenderItem( RenderItem.ItemTypes.Line,
					//         position + rot * new Vec3( 0, ySize / 2, 0 ),
					//         position + rot * new Vec3( 0, ySize / 2, zSize / 2 ),
					//         color, lineThickness ) );

					//      //Z

					//      color = axes.z ? yellow : blue;
					//      items.Add( new RenderItem( RenderItem.ItemTypes.Line,
					//         position + rot * new Vec3( 0, 0, size / 5 ),
					//         position + rot * new Vec3( 0, 0, size - arrowSize ),
					//         color, lineThickness ) );
					//      items.Add( new RenderItem( RenderItem.ItemTypes.Cone,
					//         position + rot * new Vec3( 0, 0, size - arrowSize ),
					//         position + rot * new Vec3( 0, 0, size ),
					//         color, arrowSize / 6 ) );
					//      if( !zPlus )
					//      {
					//         items.Add( new RenderItem( RenderItem.ItemTypes.Line,
					//            position + rot * new Vec3( 0, 0, -size / 5 ),
					//            position + rot * new Vec3( 0, 0, -size ),
					//            color, lineThickness ) );
					//      }

					//      color = axes.x && axes.z ? yellow : blue;
					//      items.Add( new RenderItem( RenderItem.ItemTypes.Line,
					//         position + rot * new Vec3( 0, 0, zSize / 2 ),
					//         position + rot * new Vec3( xSize / 2, 0, zSize / 2 ),
					//         color, lineThickness ) );
					//      color = axes.y && axes.z ? yellow : blue;
					//      items.Add( new RenderItem( RenderItem.ItemTypes.Line,
					//         position + rot * new Vec3( 0, 0, zSize / 2 ),
					//         position + rot * new Vec3( 0, ySize / 2, zSize / 2 ),
					//         color, lineThickness ) );

					//      //sort by distance
					//      ListUtils.SelectionSort<RenderItem>( items, delegate( RenderItem item1, RenderItem item2 )
					//      {
					//         double distance1 = Math.Max( GetCameraDistance( item1.start ), GetCameraDistance( item1.end ) );
					//         double distance2 = Math.Max( GetCameraDistance( item2.start ), GetCameraDistance( item2.end ) );
					//         if( distance1 > distance2 )
					//            return -1;
					//         if( distance1 < distance2 )
					//            return 1;
					//         return 0;
					//      } );

					//      //render items
					//      for( int nDrawStep = 0; nDrawStep < 2; nDrawStep++ )
					//      {
					//         bool drawShadows = nDrawStep == 0;
					//         if( drawShadows && GizmoConfig.shadowIntensity == 0 )
					//            continue;

					//         foreach( RenderItem item in items )
					//         {
					//            ColorValue color2 = drawShadows ? shadowColor : item.color;

					//            switch( item.type )
					//            {
					//            case RenderItem.ItemTypes.Line:
					//               AddLine( renderer, item.start, item.end, color2, drawShadows );
					//               break;
					//            case RenderItem.ItemTypes.Cone:
					//               //AddCone( renderer, item.start, item.end, item.coneArrowSize, lineThickness, drawShadows );
					//               break;
					//            }
					//         }
					//      }
					//   }

					//   ////Selected square
					//   //if( axes.TrueCount == 2 )
					//   //{
					//   //   DebugGeometry.Color = new ColorValue( 1, 1, 0, .4f );

					//   //   List<Vec3> vertices = new List<Vec3>();
					//   //   List<int> indices = new List<int>();

					//   //   Vec3 offset1;
					//   //   Vec3 offset2;

					//   //   if( axes.x )
					//   //   {
					//   //      offset1 = Vec3.XAxis;
					//   //      offset2 = axes.y ? Vec3.YAxis : Vec3.ZAxis;
					//   //   }
					//   //   else
					//   //   {
					//   //      offset1 = Vec3.YAxis;
					//   //      offset2 = Vec3.ZAxis;
					//   //   }

					//   //   offset1 *= new Vec3( xSize / 2, ySize / 2, zSize / 2 );
					//   //   offset2 *= new Vec3( xSize / 2, ySize / 2, zSize / 2 );

					//   //   vertices.Add( position );
					//   //   vertices.Add( position + rot * offset1 );
					//   //   vertices.Add( position + rot * ( offset1 + offset2 ) );
					//   //   vertices.Add( position + rot * offset2 );

					//   //   indices.Add( 0 ); indices.Add( 1 ); indices.Add( 2 );
					//   //   indices.Add( 2 ); indices.Add( 3 ); indices.Add( 0 );

					//   //   DebugGeometry.AddVertexIndexBuffer( vertices, indices, Mat4.Identity, false, false );
					//   //}
					//}

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
		}
	}
}
