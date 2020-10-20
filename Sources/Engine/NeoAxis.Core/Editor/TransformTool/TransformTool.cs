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
	/// <summary>
	/// Object implemetation of <see cref="TransformTool"/>.
	/// </summary>
	public abstract class TransformToolObject
	{
		object controlledObject;

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

		public virtual void OnModifyBegin() { }
		public virtual void OnModifyCommit() { }
		public virtual void OnModifyCancel() { }
	}

	/// <summary>
	/// A tool for editing the transformation of objects.
	/// </summary>
	public class TransformTool
	{
		EngineViewportControl viewportControl;

		List<TransformToolObject> objects = new List<TransformToolObject>();
		ModeEnum mode;
		CoordinateSystemModeEnum coordinateSystemMode = CoordinateSystemModeEnum.World;
		ModeClass[] modeObjects = new ModeClass[ 5 ];

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

		//!!!!!!

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		static extern IntPtr LoadCursorFromFile( string lpFileName );

		public TransformTool( EngineViewportControl viewportControl )
		{
			this.viewportControl = viewportControl;

			modeObjects[ 0 ] = new ModeClass();
			modeObjects[ 1 ] = new PositionModeClass();
			modeObjects[ 2 ] = new RotationModeClass();
			modeObjects[ 3 ] = new ScaleModeClass();
			modeObjects[ 4 ] = new ModeClass();
			foreach( var modeObject in modeObjects )
				modeObject.owner = this;

			Mode = ModeEnum.None;

			try
			{
				if( moveCursor == null )
				{
					var hCursor = PlatformFunctionality.Get().GetSystemCursorByFileName( @"Base\UI\Cursors\Move.cur" );
					if( hCursor != IntPtr.Zero )
						moveCursor = new Cursor( hCursor );
					else
						moveCursor = new Cursor( new MemoryStream( NeoAxis.Properties.Resources.MoveCursor ) );
				}

				if( rotateCursor == null )
				{
					var hCursor = PlatformFunctionality.Get().GetSystemCursorByFileName( @"Base\UI\Cursors\Rotate.cur" );
					if( hCursor != IntPtr.Zero )
						rotateCursor = new Cursor( hCursor );
					else
						rotateCursor = new Cursor( new MemoryStream( NeoAxis.Properties.Resources.RotateCursor ) );
				}

				if( scaleCursor == null )
				{
					var hCursor = PlatformFunctionality.Get().GetSystemCursorByFileName( @"Base\UI\Cursors\Scale.cur" );
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
			get { return (double)ProjectSettings.Get.TransformToolSizeScaled; }
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

		class ModeClass
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
				return DebugGeometry.GetThicknessByPixelSize( position, ProjectSettings.Get.TransformToolLineThicknessScaled );
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

			protected void AddLine( Vector3 start, Vector3 end, double thickness, bool drawShadows )
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
					if( drawShadows )
					{
						//!!!!в конфиге настраивать?

						double length = ( end - start ).Length();
						length += thickness2;
						thickness2 *= 2.7f;
						//thickness2 *= 2.5f;// 3;

						var center = ( end + start ) / 2;
						start2 = center + ( start - center ).GetNormalize() * length / 2;
						end2 = center + ( end - center ).GetNormalize() * length / 2;
					}

					Viewport.Simple3DRenderer.AddLine( start2, end2, thickness2 );
				}
				else
				{
					Viewport.Simple3DRenderer.AddLine( start, end );
				}
			}

			protected void AddCone( Vector3 from, Vector3 to, double radius, double lineThickness, bool drawShadows )
			{
				Vector3 from2 = from;
				Vector3 to2 = to;
				double radius2 = radius;
				if( drawShadows )
				{
					Vector3 direction2 = ( to2 - from2 ).GetNormalize();
					from2 -= direction2 * lineThickness;
					to2 += direction2 * lineThickness * 3;
					radius2 += lineThickness * 1.5f;
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

			protected void AddSphere( Sphere sphere, double lineThickness, bool drawShadows )
			{
				double radius2 = sphere.Radius;
				if( drawShadows )
					radius2 += lineThickness;

				DebugGeometry.AddSphere( sphere.Origin, radius2, 32, true );

				//Vec3[] positions;
				//int[] indices;
				//SimpleMeshGenerator.GenerateSphere( radius2, 10, 10, false, out positions, out indices );

				//Mat4 transform = Mat4.FromTranslate( sphere.Origin );
				//DebugGeometry.AddVertexIndexBuffer( positions, indices, transform, false, true );
			}

			protected void AddPolygonalChain( Vector3[] points, double radius, bool drawShadows )
			{
				if( points.Length < 2 )
					return;

				double radius2 = radius;
				if( drawShadows )
					radius2 *= 2.7f;

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
		public event EventHandler ModeChanged;

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

		public delegate void ChangeMofidyStateDelegate();
		public event ChangeMofidyStateDelegate ModifyBegin;
		public event ChangeMofidyStateDelegate ModifyCommit;
		public event ChangeMofidyStateDelegate ModifyCancel;

		void OnModifyBegin()
		{
			foreach( TransformToolObject obj in Objects )
				obj.OnModifyBegin();

			ModifyBegin?.Invoke();
		}

		void OnModifyCommit()
		{
			foreach( TransformToolObject obj in Objects )
				obj.OnModifyCommit();

			ModifyCommit?.Invoke();
		}

		void OnModifyCancel()
		{
			foreach( TransformToolObject obj in Objects )
				obj.OnModifyCancel();

			ModifyCancel?.Invoke();
		}

		double GetSnapMove()
		{
			if( viewportControl.Viewport.IsKeyPressed( EKeys.Control ) )
				return ProjectSettings.Get.SceneEditorStepMovement;
			else
				return 0;
			//return TransformToolConfig.movementSnapping ? TransformToolConfig.movementSnappingValue : 0;
		}

		Degree GetSnapRotate()
		{
			if( viewportControl.Viewport.IsKeyPressed( EKeys.Control ) )
				return ProjectSettings.Get.SceneEditorStepRotation;
			else
				return 0;
			//return TransformToolConfig.rotationSnapping ? TransformToolConfig.rotationSnappingValue : 0;
		}

		double GetSnapScale()
		{
			if( viewportControl.Viewport.IsKeyPressed( EKeys.Control ) )
				return ProjectSettings.Get.SceneEditorStepScaling;
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
						position + rot * new Vector3( xSize / 2, ySize / 2, 0 ),
						color, lineThickness ) );

					if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
					{
						color = axes.x && axes.z ? yellow : red;
						items.Add( new RenderItem( RenderItem.ItemType.Line,
							position + rot * new Vector3( xSize / 2, 0, 0 ),
							position + rot * new Vector3( xSize / 2, 0, zSize / 2 ),
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
						position + rot * new Vector3( xSize / 2, ySize / 2, 0 ),
						color, lineThickness ) );
					if( !Owner.SceneMode2D || Owner.CoordinateSystemMode == CoordinateSystemModeEnum.Local )
					{
						color = axes.y && axes.z ? yellow : green;
						items.Add( new RenderItem( RenderItem.ItemType.Line,
							position + rot * new Vector3( 0, ySize / 2, 0 ),
							position + rot * new Vector3( 0, ySize / 2, zSize / 2 ),
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
							position + rot * new Vector3( xSize / 2, 0, zSize / 2 ),
							color, lineThickness ) );
						color = axes.y && axes.z ? yellow : blue;
						items.Add( new RenderItem( RenderItem.ItemType.Line,
							position + rot * new Vector3( 0, 0, zSize / 2 ),
							position + rot * new Vector3( 0, ySize / 2, zSize / 2 ),
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
					for( int nDrawStep = 0; nDrawStep < 2; nDrawStep++ )
					{
						bool drawShadows = nDrawStep == 0;
						if( drawShadows && ProjectSettings.Get.TransformToolShadowIntensity == 0 )
							continue;

						if( drawShadows )
							DebugGeometry.SetColor( shadowColor, false );//, true );
						foreach( RenderItem item in items )
						{
							if( !drawShadows )
								DebugGeometry.SetColor( item.color, false );//, true );
							switch( item.type )
							{
							case RenderItem.ItemType.Line:
								AddLine( item.start, item.end, lineThickness, drawShadows );
								break;
							case RenderItem.ItemType.Cone:
								AddCone( item.start, item.end, item.coneArrowSize, lineThickness, drawShadows );
								break;
							}
						}
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

		///////////////////////////////////////////

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

				for( int nDrawStep = 0; nDrawStep < 2; nDrawStep++ )
				{
					bool drawShadows = nDrawStep == 0;
					if( drawShadows && ProjectSettings.Get.TransformToolShadowIntensity == 0 )
						continue;

					if( drawShadows )
						DebugGeometry.SetColor( shadowColor, false );//, true );

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
						AddPolygonalChain( points.ToArray(), lineThickness, drawShadows );
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
						AddPolygonalChain( points.ToArray(), lineThickness, drawShadows );
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
									AddPolygonalChain( newPoints.ToArray(), lineThickness, drawShadows );
									newPoints.Clear();
								}
							}
						}
						if( newPoints.Count != 0 )
						{
							AddPolygonalChain( newPoints.ToArray(), lineThickness, drawShadows );
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
									AddPolygonalChain( newPoints.ToArray(), lineThickness, drawShadows );
									newPoints.Clear();
								}
							}
						}
						if( newPoints.Count != 0 )
						{
							AddPolygonalChain( newPoints.ToArray(), lineThickness, drawShadows );
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
									AddPolygonalChain( newPoints.ToArray(), lineThickness, drawShadows );
									newPoints.Clear();
								}
							}
						}
						if( newPoints.Count != 0 )
						{
							AddPolygonalChain( newPoints.ToArray(), lineThickness, drawShadows );
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
									arrowLength / 8, lineThickness, drawShadows );
							}
							{
								Vector3 direction2 = ( arrowPosition2 - arrowCenter ).GetNormalize();
								AddCone(
									arrowCenter + direction2 * arrowLength / 2,
									arrowPosition2,
									arrowLength / 8, lineThickness, drawShadows );
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

		///////////////////////////////////////////

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
				for( int nDrawStep = 0; nDrawStep < 2; nDrawStep++ )
				{
					bool drawShadows = nDrawStep == 0;
					if( drawShadows && ProjectSettings.Get.TransformToolShadowIntensity == 0 )
						continue;

					if( drawShadows )
						DebugGeometry.SetColor( shadowColor, false );//, true );

					//X
					if( !drawShadows )
						DebugGeometry.SetColor( ( axes.x && axes.TrueCount != 3 ) ? yellow : red, false );//, true );
					AddLine( position, position + xOffset, lineThickness, drawShadows );
					AddSphere( new Sphere( position + xOffset, size / 80 ), lineThickness, drawShadows );
					AddSphere( new Sphere( position + xOffset, size / 40 ), lineThickness, drawShadows );

					if( !drawShadows )
						DebugGeometry.SetColor( axes.x && axes.y ? yellow : red, false );//, true );
					AddLine( position + xOffset * .7f, position + ( xOffset * .7f + yOffset * .7f ) / 2,
						 lineThickness, drawShadows );
					AddLine( position + xOffset * .5f, position + ( xOffset * .5f + yOffset * .5f ) / 2,
						 lineThickness, drawShadows );
					if( !drawShadows )
						DebugGeometry.SetColor( axes.x && axes.z ? yellow : red, false );//, true );
					AddLine( position + xOffset * .7f, position + ( xOffset * .7f + zOffset * .7f ) / 2,
						 lineThickness, drawShadows );
					AddLine( position + xOffset * .5f, position + ( xOffset * .5f + zOffset * .5f ) / 2,
						 lineThickness, drawShadows );

					//Y
					if( !drawShadows )
						DebugGeometry.SetColor( ( axes.y && axes.TrueCount != 3 ) ? yellow : green, false );//, true );
					AddLine( position, position + yOffset,
						 lineThickness, drawShadows );
					AddSphere( new Sphere( position + yOffset, size / 80 ), lineThickness, drawShadows );
					AddSphere( new Sphere( position + yOffset, size / 40 ), lineThickness, drawShadows );

					if( !drawShadows )
						DebugGeometry.SetColor( axes.y && axes.x ? yellow : green, false );//, true );
					AddLine( position + yOffset * .7f, position + ( yOffset * .7f + xOffset * .7f ) / 2,
						 lineThickness, drawShadows );
					AddLine( position + yOffset * .5f, position + ( yOffset * .5f + xOffset * .5f ) / 2,
						 lineThickness, drawShadows );
					if( !drawShadows )
						DebugGeometry.SetColor( axes.y && axes.z ? yellow : green, false );//, true );
					AddLine( position + yOffset * .7f, position + ( yOffset * .7f + zOffset * .7f ) / 2,
						 lineThickness, drawShadows );
					AddLine( position + yOffset * .5f, position + ( yOffset * .5f + zOffset * .5f ) / 2,
						 lineThickness, drawShadows );

					//Z
					if( !drawShadows )
						DebugGeometry.SetColor( ( axes.z && axes.TrueCount != 3 ) ? yellow : blue, false );//, true );
					AddLine( position, position + zOffset,
						 lineThickness, drawShadows );
					AddSphere( new Sphere( position + zOffset, size / 80 ), lineThickness, drawShadows );
					AddSphere( new Sphere( position + zOffset, size / 40 ), lineThickness, drawShadows );

					if( !drawShadows )
						DebugGeometry.SetColor( axes.z && axes.x ? yellow : blue, false );//, true );
					AddLine( position + zOffset * .7f, position + ( zOffset * .7f + xOffset * .7f ) / 2,
						 lineThickness, drawShadows );
					AddLine( position + zOffset * .5f, position + ( zOffset * .5f + xOffset * .5f ) / 2,
						 lineThickness, drawShadows );
					if( !drawShadows )
						DebugGeometry.SetColor( axes.z && axes.y ? yellow : blue, false );//, true );
					AddLine( position + zOffset * .7f, position + ( zOffset * .7f + yOffset * .7f ) / 2,
						 lineThickness, drawShadows );
					AddLine( position + zOffset * .5f, position + ( zOffset * .5f + yOffset * .5f ) / 2,
						 lineThickness, drawShadows );
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

		//public TransformToolObject GetObjectByControlledObject( object controlledObject )
		//{
		//	foreach( var obj in Objects )
		//	{
		//		if( ReferenceEquals( obj.ControlledObject, controlledObject ) )
		//			return obj;
		//	}
		//	return null;
		//}

		bool SceneMode2D
		{
			get
			{
				var scene = viewportControl.Viewport.AttachedScene;
				if( scene != null )
					return scene.Mode.Value == Component_Scene.ModeEnum._2D;
				return false;
			}
		}
	}
}
