#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using Internal;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Object implemetation of <see cref="TransformTool"/>.
	/// </summary>
	public abstract class TransformToolObject
	{
		object controlledObject;
		//bool modifying;

		//

		protected TransformToolObject( object controlledObject )
		{
			this.controlledObject = controlledObject;
		}

		public object ControlledObject { get { return controlledObject; } }
		//public abstract object ControlledObject { get; }

		public virtual bool IsAllowMove() { return false; }
		public virtual bool IsAllowRotate() { return false; }
		public virtual bool IsAllowScale() { return false; }

		public abstract Vector3 Position { get; set; }
		public abstract Quaternion Rotation { get; set; }
		public abstract Vector3 Scale { get; set; }

		//public bool Modifying
		//{
		//	get { return modifying; }
		//}

		public virtual void OnModifyBegin() { }
		public virtual void OnModifyCommit() { }
		public virtual void OnModifyCancel() { }
		//public virtual void OnModifyBegin() { modifying = true; }
		//public virtual void OnModifyCommit() { modifying = false; }
		//public virtual void OnModifyCancel() { modifying = false; }
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Config for <see cref="TransformTool"/>.
	/// </summary>
	public static class TransformToolConfig
	{
		//[EngineConfig( "TransformTool", "movementSnapping" )]
		//public static bool movementSnapping;
		//[EngineConfig( "TransformTool", "rotationSnapping" )]
		//public static bool rotationSnapping;
		//[EngineConfig( "TransformTool", "scalingSnapping" )]
		//public static bool scalingSnapping;

		//[EngineConfig( "TransformTool", "movementSnappingValue" )]
		//public static float movementSnappingValue = .1f;
		//[EngineConfig( "TransformTool", "rotationSnappingValue" )]
		//public static Degree rotationSnappingValue = 5;
		//[EngineConfig( "TransformTool", "scalingSnappingValue" )]
		//public static float scalingSnappingValue = .1f;

		//[EngineConfig( "TransformTool", "moveObjectsUsingLocalCoordinates" )]
		//public static bool moveObjectsUsingLocalCoordinates;
		[EngineConfig( "TransformTool", "moveObjectsDuringRotation" )]
		public static bool moveObjectsDuringRotation = true;
		[EngineConfig( "TransformTool", "moveObjectsDuringScaling" )]
		public static bool moveObjectsDuringScaling;

		//[EngineConfig( "TransformTool", "rotationSensitivity" )]
		//public static float rotationSensitivity = 1;
		//[EngineConfig( "TransformTool", "size" )]
		//public static int size = 140;
		//[EngineConfig( "TransformTool", "lineThickness" )]
		//public static float lineThickness = 2;
		//[EngineConfig( "TransformTool", "shadowIntensity" )]
		//public static float shadowIntensity = .3f;//!!!! .07f;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// A tool for editing the transformation of objects.
	/// </summary>
	public partial class TransformTool
	{
		EngineViewportControl viewportControl;

		List<TransformToolObject> objects = new List<TransformToolObject>();
		ModeEnum mode;
		CoordinateSystemModeEnum coordinateSystemMode = CoordinateSystemModeEnum.World;
		ModeClass[] modeObjects = new ModeClass[ 6 ];

		bool active = true;

		static Cursor moveCursor;
		static Cursor rotateCursor;
		static Cursor scaleCursor;

		///////////////////////////////////////////

		/// <summary>
		/// Defines the position in space of an object for <see cref="TransformTool"/>.
		/// </summary>
		public struct TransformOfObject
		{
			public Vector3 position;
			public Quaternion rotation;
			public Vector3 scale;

			public TransformOfObject( Vector3 position, Quaternion rotation, Vector3 scale )
			{
				this.position = position;
				this.rotation = rotation;
				this.scale = scale;
			}
		}

		///////////////////////////////////////////

		public TransformTool( EngineViewportControl viewportControl )
		{
			this.viewportControl = viewportControl;
			viewportControl.TransformTool = this;

			modeObjects[ 0 ] = new ModeClass();
			modeObjects[ 1 ] = new PositionModeClass();
			modeObjects[ 2 ] = new RotationModeClass();
			modeObjects[ 3 ] = new PositionRotationModeClass();
			modeObjects[ 4 ] = new ScaleModeClass();
			modeObjects[ 5 ] = new ModeClass();
			foreach( var modeObject in modeObjects )
				modeObject.owner = this;

			Mode = ModeEnum.None;

			try
			{
				if( moveCursor == null )
				{
					var hCursor = PlatformFunctionality.Instance.GetSystemCursorByFileName( @"Base\UI\Cursors\Move.cur" );
					if( hCursor != IntPtr.Zero )
						moveCursor = new Cursor( hCursor );
					else
						moveCursor = new Cursor( new MemoryStream( NeoAxis.Properties.Resources.MoveCursor ) );
				}

				if( rotateCursor == null )
				{
					var hCursor = PlatformFunctionality.Instance.GetSystemCursorByFileName( @"Base\UI\Cursors\Rotate.cur" );
					if( hCursor != IntPtr.Zero )
						rotateCursor = new Cursor( hCursor );
					else
						rotateCursor = new Cursor( new MemoryStream( NeoAxis.Properties.Resources.RotateCursor ) );
				}

				if( scaleCursor == null )
				{
					var hCursor = PlatformFunctionality.Instance.GetSystemCursorByFileName( @"Base\UI\Cursors\Scale.cur" );
					if( hCursor != IntPtr.Zero )
						scaleCursor = new Cursor( hCursor );
					else
						scaleCursor = new Cursor( new MemoryStream( NeoAxis.Properties.Resources.ScaleCursor ) );
				}
			}
			catch { }
		}

		//!!!!Dispose?

		static double ArrowPixelSize
		{
			get { return (double)ProjectSettings.Get.SceneEditor.TransformToolSizeScaled; }
		}

		static double SelectNearPixels
		{
			get { return ArrowPixelSize * .07f; }
		}

		//static double TangentLinePixelSize
		//{
		//   get { return ArrowPixelSize * 0.3f; }
		//}

		///////////////////////////////////////////

		public enum ModeEnum
		{
			None,
			Position,
			Rotation,
			PositionRotation,
			Scale,
			Undefined
		}

		///////////////////////////////////////////

		public enum CoordinateSystemModeEnum
		{
			World,
			Local
		}

		///////////////////////////////////////////

		public class ModeClass
		{
			internal TransformTool owner;

			protected Vector2 modifyStartPos;
			protected bool modify_Prepare;
			protected bool modify_Activated;
			protected Screen cursorTransitionOnScreenBorderScreen;
			protected Vector2 cursorTransitionOnScreenBorderOffset;

			public TransformTool Owner
			{
				get { return owner; }
			}

			//Convenient properties
			public EngineViewportControl ViewportControl
			{
				get { return owner.ViewportControl; }
			}
			public Viewport Viewport
			{
				get { return owner.ViewportControl.Viewport; }
			}
			public Viewport.CameraSettingsClass CameraSettings
			{
				get { return owner.viewportControl.Viewport.CameraSettings; }
			}
			public Simple3DRenderer DebugGeometry
			{
				get { return owner.viewportControl.Viewport.Simple3DRenderer; }
			}

			public virtual bool OnKeyDown( KeyEvent e )
			{
				if( e.Key == EKeys.Escape && modify_Prepare )
				{
					OnCancelModify();
					return true;
				}
				return false;
			}

			public virtual bool OnKeyUp( KeyEvent e ) { return false; }

			public virtual bool OnMouseDown( EMouseButtons button )
			{
				if( button == EMouseButtons.Left && Owner.Objects.Count != 0 )
				{
					if( OnTryBeginModify() )
					{
						modify_Prepare = true;
						modifyStartPos = Viewport.MousePosition;
						return true;
					}
				}

				return false;
			}

			public virtual void OnUpdateInitialObjectsTransform() { }

			public virtual bool OnMouseUp( EMouseButtons button )
			{
				if( button == EMouseButtons.Left && modify_Prepare )
				{
					OnCommitModify();
					return true;
				}

				if( button == EMouseButtons.Right && modify_Prepare )
				{
					OnCancelModify();
					return true;
				}

				return false;
			}

			public virtual bool OnMouseDoubleClick( EMouseButtons button ) { return false; }

			public virtual void OnMouseMove( Vector2 mouse )
			{
				if( modify_Prepare && !modify_Activated )
				{
					Vector2 viewportSize = Owner.ViewportControl.Viewport.SizeInPixels.ToVector2();
					Vector2 diffPixels = ( mouse - modifyStartPos ) * viewportSize;
					if( Math.Abs( diffPixels.X ) >= 3 || Math.Abs( diffPixels.Y ) >= 3 )
					{
						//start modify

						ViewportControl.Capture = true;

						modify_Activated = true;
						cursorTransitionOnScreenBorderScreen = Screen.FromPoint( Cursor.Position );
						cursorTransitionOnScreenBorderOffset = Vector2.Zero;

						if( ( Form.ModifierKeys & Keys.Shift ) != 0 )//if( EngineApp.Instance.IsKeyPressed( EKeys.Shift ) )
							Owner.OnCloneAndSelectObjects();

						Owner.OnModifyBegin();
					}
				}
			}

			public virtual void OnTick( double delta ) { }
			public virtual bool OnIsMouseOverAxisToActivation() { return false; }
			public virtual void OnRender() { }
			public virtual void OnRenderUI() { }

			public bool IsMouseNearLine( Vector3 start, Vector3 end, out Vector2 projectedScreenPointInPixels, out Radian projectedScreenAngle, out double distance )
			{
				projectedScreenPointInPixels = Vector2.Zero;
				projectedScreenAngle = 0;
				distance = 0;

				Vector2 viewportSize = Viewport.SizeInPixels.ToVector2();
				Vector2 mouseInPixels = Viewport.MousePosition * viewportSize;

				Vector2 screenStart;
				if( !CameraSettings.ProjectToScreenCoordinates( start, out screenStart ) )
					return false;
				Vector2 screenEnd;
				if( !CameraSettings.ProjectToScreenCoordinates( end, out screenEnd ) )
					return false;

				Vector2 screenStartInPixels = screenStart * viewportSize;
				Vector2 screenEndInPixels = screenEnd * viewportSize;

				Rectangle rect = new Rectangle( screenStartInPixels );
				rect.Add( screenEndInPixels );
				rect.Expand( SelectNearPixels );

				if( !rect.Contains( mouseInPixels ) )
					return false;

				projectedScreenPointInPixels = MathAlgorithms.ProjectPointToLine( screenStartInPixels, screenEndInPixels, mouseInPixels );

				/*double */
				distance = ( mouseInPixels - projectedScreenPointInPixels ).Length();
				if( distance > SelectNearPixels )
					return false;

				Vector2 screenDiff = screenEndInPixels - screenStartInPixels;
				projectedScreenAngle = Math.Atan2( screenDiff.Y, screenDiff.X );

				return true;
			}

			public bool IsMouseNearLine( Vector3 start, Vector3 end )
			{
				return IsMouseNearLine( start, end, out _, out _, out _ );
			}

			double Sign( Vector2 p1, Vector2 p2, Vector2 p3 )
			{
				return ( p1.X - p3.X ) * ( p2.Y - p3.Y ) - ( p2.X - p3.X ) * ( p1.Y - p3.Y );
			}

			bool PointInTriangle( Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3 )
			{
				bool b1 = Sign( pt, v1, v2 ) < 0;
				bool b2 = Sign( pt, v2, v3 ) < 0;
				bool b3 = Sign( pt, v3, v1 ) < 0;
				return ( ( b1 == b2 ) && ( b2 == b3 ) );
			}

			public bool IsMouseOverTriangle( Vector3 p1, Vector3 p2, Vector3 p3 )
			{
				Vector2 screenPosition1;
				if( !CameraSettings.ProjectToScreenCoordinates( p1, out screenPosition1 ) )
					return false;
				Vector2 screenPosition2;
				if( !CameraSettings.ProjectToScreenCoordinates( p2, out screenPosition2 ) )
					return false;
				Vector2 screenPosition3;
				if( !CameraSettings.ProjectToScreenCoordinates( p3, out screenPosition3 ) )
					return false;

				Vector2 mouse = Viewport.MousePosition;
				return PointInTriangle( mouse, screenPosition1, screenPosition2, screenPosition3 );
			}

			public double GetSize()
			{
				Trace.Assert( Owner.Objects.Count != 0 );

				return DebugGeometry.GetThicknessByPixelSize( Owner.GetPosition(), ArrowPixelSize );
			}

			protected virtual bool OnTryBeginModify() { return false; }

			protected virtual void OnCommitModify()
			{
				bool commit = modify_Activated;

				if( modify_Activated )
					ViewportControl.Capture = false;

				modify_Activated = false;
				modify_Prepare = false;

				if( commit )
					Owner.OnModifyCommit();
			}

			internal protected virtual void OnCancelModify()
			{
				bool cancel = modify_Activated;

				if( modify_Activated )
					ViewportControl.Capture = false;

				modify_Activated = false;
				modify_Prepare = false;

				if( cancel )
					Owner.OnModifyCancel();
			}

			public bool Modify_Activated
			{
				get { return modify_Activated; }
			}

			internal protected virtual bool OnMouseOverAxis() { return false; }

			protected void UpdateCursorTransitionOnScreenBorder()
			{
				System.Drawing.Point p = Cursor.Position;
				Vector2I position = new Vector2I( p.X, p.Y );

				System.Drawing.Rectangle r = cursorTransitionOnScreenBorderScreen.Bounds;
				RectangleI rectangle = new RectangleI( r.Left, r.Top, r.Right, r.Bottom );

				if( position.X < rectangle.Left + 3 )
				{
					Cursor.Position = new System.Drawing.Point( rectangle.Right - 6, position.Y );
					cursorTransitionOnScreenBorderOffset -= new Vector2( rectangle.Size.X, 0 );
				}
				if( position.X > rectangle.Right - 3 )
				{
					Cursor.Position = new System.Drawing.Point( rectangle.Left + 6, position.Y );
					cursorTransitionOnScreenBorderOffset += new Vector2( rectangle.Size.X, 0 );
				}
				if( position.Y < rectangle.Top + 3 )
				{
					Cursor.Position = new System.Drawing.Point( position.X, rectangle.Bottom - 6 );
					cursorTransitionOnScreenBorderOffset -= new Vector2( 0, rectangle.Size.Y );
				}
				if( position.Y > rectangle.Bottom - 3 )
				{
					Cursor.Position = new System.Drawing.Point( position.X, rectangle.Top + 6 );
					cursorTransitionOnScreenBorderOffset += new Vector2( 0, rectangle.Size.Y );
				}
			}

			protected double GetLineWorldThickness( Vector3 position )
			{
				return DebugGeometry.GetThicknessByPixelSize( position, ProjectSettings.Get.SceneEditor.TransformToolLineThicknessScaled );
			}

			//protected void AddLine( Vec3 start, Vec3 end, ColorValue color, bool drawShadows )
			//{
			//   Vec2 start2D;
			//   Vec2 end2D;
			//   if( !CameraSettings.ProjectToScreenCoordinates( start, out start2D ) )
			//      return;
			//   if( !CameraSettings.ProjectToScreenCoordinates( end, out end2D ) )
			//      return;

			//   if( GizmoConfig.lineThickness != 0 )
			//   {
			//      Vec2[] points = new Vec2[ 2 ];
			//      points[ 0 ] = start2D;
			//      points[ 1 ] = end2D;

			//      Vec2 viewportSize = Owner.ViewportControl.Viewport.SizeInPixels.ToVec2();
			//      Vec2 thickness = GizmoConfig.lineThickness / viewportSize;
			//      GuiRenderer.TriangleVertex[] vertices = Thick2DLineGenerator.GenerateTriangles( points, thickness, color );
			//      renderer.AddTriangles( vertices );
			//   }
			//   else
			//   {
			//      renderer.AddLine( start2D, end2D, color );
			//   }
			//}

			protected void AddLine( Vector3 start, Vector3 end, double thickness, double drawShadowsFactor )
			{
				//!!!!как-то странно тени рисуются, вроде как неправильно
				//!!!!!!второй вариант рисование теней: с помощью постэффекта. сначала всё в текстуру, потом её обработать
				//!!!!!!!!тени потом тоже через DebugGeometry. везде так

				if( thickness != 0 )
				{
					//!!!!тоже в debug geometry. только рисовать нужно группой

					Vector3 start2 = start;
					Vector3 end2 = end;
					double thickness2 = thickness;
					if( drawShadowsFactor != 0 )
					{
						//!!!!в конфиге настраивать?

						double length = ( end - start ).Length();
						length -= thickness2;
						//length += thickness2;
						thickness2 *= 2.7 * drawShadowsFactor;// 2.7f;
															  //thickness2 *= 2.3 * drawShadowsFactor;// 2.7f;
															  //thickness2 *= 2.5f;// 3;

						var center = ( end + start ) / 2;
						start2 = center + ( start - center ).GetNormalize() * length / 2;
						end2 = center + ( end - center ).GetNormalize() * length / 2;
					}

					Viewport.Simple3DRenderer.AddLine( start2, end2, thickness2 );
				}
				else
					Viewport.Simple3DRenderer.AddLine( start, end );
			}

			protected void AddCone( Vector3 from, Vector3 to, double radius, double lineThickness, double drawShadowsFactor )
			{
				Vector3 from2 = from;
				Vector3 to2 = to;
				double radius2 = radius;
				if( drawShadowsFactor != 0 )
				{
					Vector3 direction2 = ( to2 - from2 ).GetNormalize();
					from2 -= direction2 * lineThickness * 0.4 * drawShadowsFactor;
					to2 += direction2 * lineThickness * 3 * drawShadowsFactor;
					radius2 += lineThickness * 1.5f * drawShadowsFactor;
				}

				double length = ( to2 - from2 ).Length();
				Matrix4 t = new Matrix4( Quaternion.FromDirectionZAxisUp( to2 - from2 ).ToMatrix3(), from2 );
				DebugGeometry.AddCone( t, 0, SimpleMeshGenerator.ConeOrigin.Bottom, radius2, length, 32, 32, true );

				//double length = ( to2 - from2 ).Length();

				//Vec3 direction = to2 - from2;
				//if( direction == Vec3.Zero )
				//	direction = Vec3.XAxis;
				//direction.Normalize();

				////var rotation = Mat3.LookAt( to2 - from2, Vec3.ZAxis );
				////double length = direction.Normalize();

				//Vec3[] positions;
				//int[] indices;
				//SimpleMeshGenerator.GenerateCone( 0, SimpleMeshGenerator.ConeOrigin.Bottom, radius2, length, 32, true, true, out positions, out indices );

				////Mat4 transform = new Mat4( rotation, from2 );
				//Quat rotation = Quat.FromDirectionZAxisUp( direction );
				//Mat4 transform = new Mat4( rotation.ToMat3(), from2 );

				//DebugGeometry.AddVertexIndexBuffer( positions, indices, transform, false, true );
			}

			protected void AddSphere( Sphere sphere, double lineThickness, double drawShadowsFactor )
			{
				double radius2 = sphere.Radius;
				if( drawShadowsFactor != 0 )
					radius2 += lineThickness * drawShadowsFactor;

				DebugGeometry.AddSphere( sphere.Center, radius2, 32, true );

				//Vec3[] positions;
				//int[] indices;
				//SimpleMeshGenerator.GenerateSphere( radius2, 10, 10, false, out positions, out indices );

				//Mat4 transform = Mat4.FromTranslate( sphere.Origin );
				//DebugGeometry.AddVertexIndexBuffer( positions, indices, transform, false, true );
			}

			protected void AddPolygonalChain( Vector3[] points, double radius, double drawShadowsFactor )
			{
				if( points.Length < 2 )
					return;

				double radius2 = radius;
				if( drawShadowsFactor != 0 )
					radius2 *= 2.5 * drawShadowsFactor;//2.7f;

				Vector3 center = Vector3.Zero;
				foreach( Vector3 point in points )
					center += point;
				center /= points.Length;

				Vector3[] localPoints = new Vector3[ points.Length ];
				for( int n = 0; n < points.Length; n++ )
					localPoints[ n ] = points[ n ] - center;

				Vector3[] positions;
				int[] indices;
				SimpleMeshGenerator.GeneratePolygonalChain( localPoints, radius2, out positions, out indices );

				Matrix4 transform = Matrix4.FromTranslate( center );
				DebugGeometry.AddTriangles( positions, indices, transform, false, true );

				//for( int n = 0; n < points.Length - 1; n++ )
				//{
				//   Vec3 p1 = points[ n ];
				//   Vec3 p2 = points[ n + 1 ];
				//   AddLine( p1, p2, thickness, drawShadows );
				//}
			}

			public double GetFontSize()
			{
				double fontSizeInPixels = 14.0;
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
				{
					try
					{
						fontSizeInPixels *= DpiHelper.Default.DpiScaleFactor;
					}
					catch { }
				}
				fontSizeInPixels = (int)fontSizeInPixels;

				var renderer = ViewportControl.Viewport.CanvasRenderer;

				int height = renderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y;
				float screenCellSize = (float)fontSizeInPixels / (float)height;
				float demandFontHeight = screenCellSize;// * GetZoom();

				return demandFontHeight;
			}

			protected void AddTextWithShadow( CanvasRenderer renderer, string text, Vector2 position, EHorizontalAlignment horizontalAlign,
				EVerticalAlignment verticalAlign, ColorValue color, double shadowOffsetInPixels )
			{
				Vector2 shadowOffset = shadowOffsetInPixels / Owner.ViewportControl.Viewport.SizeInPixels.ToVector2();
				renderer.AddText( null, GetFontSize(), text, position + shadowOffset, horizontalAlign, verticalAlign, new ColorValue( 0, 0, 0, color.Alpha / 2.5f ) );
				renderer.AddText( null, GetFontSize(), text, position, horizontalAlign, verticalAlign, color );
			}
		}

		///////////////////////////////////////////

		public EngineViewportControl ViewportControl
		{
			get { return viewportControl; }
		}

		public ModeEnum Mode
		{
			get { return mode; }
			set
			{
				if( Modifying )
					return;
				if( mode == value )
					return;
				mode = value;

				ModeChanged?.Invoke( this, new EventArgs() );
			}
		}

		public CoordinateSystemModeEnum CoordinateSystemMode
		{
			get { return coordinateSystemMode; }
			set
			{
				if( coordinateSystemMode == value )
					return;
				coordinateSystemMode = value;

				//!!!!
				ModeChanged?.Invoke( this, new EventArgs() );
			}
		}

		//!!!!
		public event System.EventHandler ModeChanged;

		public List<TransformToolObject> Objects
		{
			get { return objects; }
		}

		//!!!!а надо ли кнопки? не шорткатами ли?
		//!!!!!!есть Escape, видать, не переопределяемый
		public void PerformKeyDown( KeyEvent e, ref bool handled )
		{
			if( !Active )
				return;
			if( modeObjects[ (int)mode ].OnKeyDown( e ) )
				handled = true;
		}

		//!!!!а надо ли кнопки? не шорткатами ли?
		public void PerformKeyUp( KeyEvent e, ref bool handled )
		{
			if( !Active )
				return;
			if( modeObjects[ (int)mode ].OnKeyUp( e ) )
				handled = true;
		}

		public void PerformMouseDown( EMouseButtons button, ref bool handled )
		{
			if( !Active )
				return;
			if( modeObjects[ (int)mode ].OnMouseDown( button ) )
				handled = true;
		}

		public void PerformUpdateInitialObjectsTransform()
		{
			if( !Active )
				return;
			modeObjects[ (int)mode ].OnUpdateInitialObjectsTransform();
		}

		public void PerformMouseUp( EMouseButtons button, ref bool handled )
		{
			if( !Active )
				return;
			if( modeObjects[ (int)mode ].OnMouseUp( button ) )
				handled = true;
		}

		public void PerformMouseDoubleClick( EMouseButtons button, ref bool handled )
		{
			if( !Active )
				return;
			if( modeObjects[ (int)mode ].OnMouseDoubleClick( button ) )
				handled = true;
		}

		public void PerformMouseMove( Vector2 mouse )
		{
			if( Active )
				modeObjects[ (int)mode ].OnMouseMove( mouse );
		}

		public void PerformTick( double delta )
		{
			if( Active )
				modeObjects[ (int)mode ].OnTick( delta );
		}

		public bool IsMouseOverAxisToActivation()
		{
			if( Active )
				return modeObjects[ (int)mode ].OnIsMouseOverAxisToActivation();
			return false;
		}

		public void PerformRender()
		{
			if( Active )
				modeObjects[ (int)mode ].OnRender();
		}

		public void PerformOnRenderUI()
		{
			if( Active )
				modeObjects[ (int)mode ].OnRenderUI();
		}

		Vector3 GetPosition()
		{
			Trace.Assert( Objects.Count != 0 );
			Vector3 pos = Vector3.Zero;
			foreach( TransformToolObject obj in Objects )
				pos += obj.Position;
			pos = pos / (double)Objects.Count;

			//add offset when position very close to near clip plane.
			Plane plane = viewportControl.Viewport.CameraSettings.Frustum.Planes[ 0 ];
			double distance = plane.GetDistance( pos );
			if( distance > 0 && distance < .1f )
				pos += plane.Normal * ( .1f - distance );

			return pos;
		}

		//было, но не юзалось
		//public delegate void PickObjectByCursorDelegate( ref TransformToolObject gizmoObject );
		//public event PickObjectByCursorDelegate PickObjectByCursor;

		//было, но не юзалось
		//TransformToolObject OnPickObjectByCursor()
		//{
		//	TransformToolObject gizmoObject = null;
		//	if( PickObjectByCursor != null )
		//		PickObjectByCursor( ref gizmoObject );
		//	return gizmoObject;
		//}

		public delegate void ChangeMofidyStateDelegate( TransformTool sender );
		public event ChangeMofidyStateDelegate ModifyBegin;
		public event ChangeMofidyStateDelegate ModifyCommit;
		public event ChangeMofidyStateDelegate ModifyCancel;
		public static event ChangeMofidyStateDelegate AllInstances_ModifyBegin;
		public static event ChangeMofidyStateDelegate AllInstances_ModifyCommit;
		public static event ChangeMofidyStateDelegate AllInstances_ModifyCancel;

		void OnModifyBegin()
		{
			foreach( TransformToolObject obj in Objects )
				obj.OnModifyBegin();

			ModifyBegin?.Invoke( this );
			AllInstances_ModifyBegin?.Invoke( this );
		}

		void OnModifyCommit()
		{
			foreach( TransformToolObject obj in Objects )
				obj.OnModifyCommit();

			ModifyCommit?.Invoke( this );
			AllInstances_ModifyCommit?.Invoke( this );
		}

		void OnModifyCancel()
		{
			foreach( TransformToolObject obj in Objects )
				obj.OnModifyCancel();

			ModifyCancel?.Invoke( this );
			AllInstances_ModifyCancel?.Invoke( this );
		}

		double GetSnapMove()
		{
			if( viewportControl.Viewport.IsKeyPressed( EKeys.Control ) )
				return ProjectSettings.Get.SceneEditor.SceneEditorStepMovement;
			else
				return 0;
			//return TransformToolConfig.movementSnapping ? TransformToolConfig.movementSnappingValue : 0;
		}

		Degree GetSnapRotate()
		{
			if( viewportControl.Viewport.IsKeyPressed( EKeys.Control ) )
				return ProjectSettings.Get.SceneEditor.SceneEditorStepRotation;
			else
				return 0;
			//return TransformToolConfig.rotationSnapping ? TransformToolConfig.rotationSnappingValue : 0;
		}

		double GetSnapScale()
		{
			if( viewportControl.Viewport.IsKeyPressed( EKeys.Control ) )
				return ProjectSettings.Get.SceneEditor.SceneEditorStepScaling;
			else
				return 0;
			//return TransformToolConfig.scalingSnapping ? TransformToolConfig.scalingSnappingValue : 0;
		}

		public delegate void CloneAndSelectObjectsDelegate();
		public event CloneAndSelectObjectsDelegate CloneAndSelectObjects;

		void OnCloneAndSelectObjects()
		{
			if( CloneAndSelectObjects != null )
				CloneAndSelectObjects();
		}

		bool IsAllowMove()
		{
			foreach( TransformToolObject obj in Objects )
				if( obj.IsAllowMove() )
					return true;
			return false;
		}

		bool IsAllowRotate()
		{
			foreach( TransformToolObject obj in Objects )
				if( obj.IsAllowRotate() )
					return true;
			return false;
		}

		bool IsAllowScale()
		{
			foreach( TransformToolObject obj in Objects )
				if( obj.IsAllowScale() )
					return true;
			return false;
		}

		///////////////////////////////////////////

		public bool Active
		{
			get { return active; }
			set { active = value; }
		}

		public bool Modifying
		{
			get
			{
				foreach( ModeClass modeClass in modeObjects )
				{
					if( modeClass.Modify_Activated )
						return true;
				}
				return false;
			}
		}

		public void DoCancelModify()
		{
			if( modeObjects[ (int)mode ].Modify_Activated )
				modeObjects[ (int)mode ].OnCancelModify();
		}

		protected virtual void OnPositionModeUpdateObjects( TransformOfObject[] initialObjectsTransform, Vector3 positionOffset )
		{
			for( int n = 0; n < Objects.Count; n++ )
			{
				TransformOfObject initialTransform = initialObjectsTransform[ n ];
				Objects[ n ].Position = initialTransform.position + positionOffset;
				Objects[ n ].Rotation = initialTransform.rotation;
			}
		}

		protected virtual void OnRotationModeUpdateObjects( TransformOfObject[] initialObjectsTransform, Vector3 modifyPosition, Quaternion rotationOffset )
		{
			for( int n = 0; n < Objects.Count; n++ )
			{
				TransformOfObject initialTransform = initialObjectsTransform[ n ];
				if( TransformToolConfig.moveObjectsDuringRotation )
				{
					Vector3 offset = initialTransform.position - modifyPosition;
					Objects[ n ].Position = modifyPosition + offset * rotationOffset;
				}
				Objects[ n ].Rotation = rotationOffset * initialTransform.rotation;
			}
		}

		protected virtual void OnScaleModeUpdateObjects( TransformOfObject[] initialObjectsTransform, Vector3 modifyPosition, Vector3 scaleOffset )
		{
			for( int n = 0; n < Objects.Count; n++ )
			{
				if( TransformToolConfig.moveObjectsDuringScaling )
				{
					Vector3 offset = initialObjectsTransform[ n ].position - modifyPosition;
					Objects[ n ].Position = modifyPosition + offset * scaleOffset;
				}
				Objects[ n ].Scale = initialObjectsTransform[ n ].scale * scaleOffset;
			}
		}

		public bool MouseOverAxis
		{
			get { return modeObjects[ (int)mode ].OnMouseOverAxis(); }
		}

		bool SceneMode2D
		{
			get
			{
				var scene = viewportControl.Viewport.AttachedScene;
				if( scene != null )
					return scene.Mode.Value == Scene.ModeEnum._2D;
				return false;
			}
		}

		//public TransformToolObject GetObjectByControlledObject( object controlledObject )
		//{
		//	//!!!!slowly

		//	foreach( var obj in Objects )
		//	{
		//		if( ReferenceEquals( obj.ControlledObject, controlledObject ) )
		//			return obj;
		//	}
		//	return null;
		//}

	}
}

#endif