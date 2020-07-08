// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using NeoAxis.Input;

namespace NeoAxis
{
	/// <summary>
	/// Base container class for other UI elements.
	/// </summary>
	public class UIContainer : UIControl
	{
		Viewport viewport;
		Vector2I cachedViewportSize;

		float aspectRatio = 1;

		internal UIControl capturedControl;
		internal UIControl focusedControl;

		string defaultCursor;
		string currentCursor;
		bool drawCursorWithPerPixelAccuracy = true;

		List<UIControl> cachedCoverControls = new List<UIControl>();

		internal Vector2 controlManagerMousePosition;

		//!!!!impl
		Transform transform3D;

		//

		//Keys

		public UIContainer( Viewport viewport )
		{
			this.viewport = viewport;

			//if( EngineApp.Instance != null && EngineApp.Instance.MainViewport.Viewport.GuiRenderer == guiRenderer )
			//{
			//   controlManagerMousePosition = EngineApp.Instance.MainViewport.MousePosition;
			//   lastMousePositionInsideBounds = true;
			//}
			//else
			//{
			controlManagerMousePosition = new Vector2F( -1, -1 );
			lastMousePositionInsideBounds = false;
			//}

			cachedViewportSize = viewport.SizeInPixels;
		}

		public Viewport Viewport
		{
			get { return viewport; }
		}

		void UpdateCachedCoverControls()
		{
			cachedCoverControls.Clear();
			int counter = 0;

			foreach( var control in GetComponents<UIControl>( false, true, true ) )
			{
				if( control.VisibleInHierarchy )
				{
					control.cachedIndexInHierarchyToImplementCovering = counter;
					counter++;

					if( control.CoverOtherControls != CoverOtherControlsEnum.None )
						cachedCoverControls.Add( control );
				}
			}
		}

		public bool IsControlCursorCoveredByOther( UIControl controlToCheck )
		{
			if( cachedCoverControls.Count != 0 )
			{
				foreach( var cachedControl in cachedCoverControls )
				{
					//check control before
					if( controlToCheck.cachedIndexInHierarchyToImplementCovering < cachedControl.cachedIndexInHierarchyToImplementCovering )
					{
						//check control is not child
						if( !controlToCheck.GetAllParents( false ).Contains( controlToCheck ) )
						{
							if( cachedControl.CoverOtherControls == CoverOtherControlsEnum.AllPreviousInHierarchy )
								return true;
							if( cachedControl.GetScreenRectangle().Contains( MousePosition ) )
								return true;
						}
					}
				}
			}
			return false;
		}

		public bool PerformKeyDown( KeyEvent e )
		{
			CheckCachedParameters();
			UpdateCachedCoverControls();

			if( capturedControl != null && capturedControl.ParentContainer == null )
				capturedControl = null;
			if( focusedControl != null && focusedControl.ParentContainer == null )
				focusedControl = null;

			if( focusedControl != null )
				return focusedControl.CallKeyDown( e );

			return CallKeyDown( e );
		}

		public bool PerformKeyPress( KeyPressEvent e )
		{
			CheckCachedParameters();
			UpdateCachedCoverControls();

			if( capturedControl != null && capturedControl.ParentContainer == null )
				capturedControl = null;
			if( focusedControl != null && focusedControl.ParentContainer == null )
				focusedControl = null;

			if( focusedControl != null )
				return focusedControl.CallKeyPress( e );

			return CallKeyPress( e );
		}

		public bool PerformKeyUp( KeyEvent e )
		{
			CheckCachedParameters();
			UpdateCachedCoverControls();

			if( capturedControl != null && capturedControl.ParentContainer == null )
				capturedControl = null;
			if( focusedControl != null && focusedControl.ParentContainer == null )
				focusedControl = null;

			if( focusedControl != null )
				return focusedControl.CallKeyUp( e );

			return CallKeyUp( e );
		}

		//Mouse

		public bool PerformMouseDown( EMouseButtons button )
		{
			CheckCachedParameters();
			UpdateCachedCoverControls();

			if( capturedControl != null && capturedControl.ParentContainer == null )
				capturedControl = null;
			if( focusedControl != null && focusedControl.ParentContainer == null )
				focusedControl = null;

			if( capturedControl != null )
				return capturedControl.CallMouseDown( button );

			//if( focused == null || focused.IsDestroyed )
			//   return false;
			//return focused.DoMouseDown( button );


			return CallMouseDown( button );
		}

		public bool PerformMouseUp( EMouseButtons button )
		{
			CheckCachedParameters();
			UpdateCachedCoverControls();

			if( capturedControl != null && capturedControl.ParentContainer == null )
				capturedControl = null;
			if( focusedControl != null && focusedControl.ParentContainer == null )
				focusedControl = null;

			if( capturedControl != null )
				return capturedControl.CallMouseUp( button );

			return CallMouseUp( button );
		}

		public bool PerformMouseDoubleClick( EMouseButtons button )
		{
			CheckCachedParameters();
			UpdateCachedCoverControls();

			if( capturedControl != null && capturedControl.ParentContainer == null )
				capturedControl = null;
			if( focusedControl != null && focusedControl.ParentContainer == null )
				focusedControl = null;

			if( capturedControl != null )
				return capturedControl.CallMouseDoubleClick( button );

			return CallMouseDoubleClick( button );
		}

		public void PerformMouseMove( Vector2 mouse )
		{
			CheckCachedParameters();
			UpdateCachedCoverControls();

			if( capturedControl != null && capturedControl.ParentContainer == null )
				capturedControl = null;
			if( focusedControl != null && focusedControl.ParentContainer == null )
				focusedControl = null;

			//if( capture != null )
			//{
			//   capture.DoMouseMove( capture.ScreenToLocal( mouse ) );
			//   mousePosition = mouse;
			//   return;
			//}

			controlManagerMousePosition = mouse;

			CallMouseMove( mouse );
		}

		public bool PerformMouseWheel( int delta )
		{
			CheckCachedParameters();
			UpdateCachedCoverControls();

			if( capturedControl != null && capturedControl.ParentContainer == null )
				capturedControl = null;
			if( focusedControl != null && focusedControl.ParentContainer == null )
				focusedControl = null;

			if( focusedControl != null )
				return focusedControl.CallMouseWheel( delta );

			return CallMouseWheel( delta );
		}

		public bool PerformJoystickEvent( JoystickInputEvent e )
		{
			CheckCachedParameters();
			UpdateCachedCoverControls();

			if( capturedControl != null && capturedControl.ParentContainer == null )
				capturedControl = null;
			if( focusedControl != null && focusedControl.ParentContainer == null )
				focusedControl = null;

			if( capturedControl != null )
				return capturedControl.CallJoystickEvent( e );

			return CallJoystickEvent( e );
		}

		public bool PerformTouchEvent( TouchEventData e )
		{
			CheckCachedParameters();
			UpdateCachedCoverControls();

			if( capturedControl != null && capturedControl.ParentContainer == null )
				capturedControl = null;
			if( focusedControl != null && focusedControl.ParentContainer == null )
				focusedControl = null;

			if( capturedControl != null )
				return capturedControl.CallTouchEvent( e );

			return CallTouchEvent( e );
		}

		public bool PerformSpecialInputDeviceEvent( InputEvent e )
		{
			CheckCachedParameters();
			UpdateCachedCoverControls();

			if( capturedControl != null && capturedControl.ParentContainer == null )
				capturedControl = null;
			if( focusedControl != null && focusedControl.ParentContainer == null )
				focusedControl = null;

			if( capturedControl != null )
				return capturedControl.CallSpecialInputDeviceEvent( e );

			return CallSpecialInputDeviceEvent( e );
		}

		internal override void OnUpdate_Before( float delta )
		{
			base.OnUpdate_Before( delta );

			CheckCachedParameters();
			UpdateCachedCoverControls();

			if( capturedControl != null && capturedControl.ParentContainer == null )
				capturedControl = null;
			if( focusedControl != null && focusedControl.ParentContainer == null )
				focusedControl = null;
		}

		////Tick

		//public void PerformUITick( double delta )
		//{
		//	CheckCachedParameters();
		//	UpdateTopMouseCoversControl();

		//	if( capturedControl != null && capturedControl.ParentContainer == null )
		//		capturedControl = null;
		//	if( focusedControl != null && focusedControl.ParentContainer == null )
		//		focusedControl = null;

		//	OnUITick( delta );

		//	//!!!!где еще вызывать это? и для каких типов объектов
		//}

		//Render

		//public void DoRender()
		//{
		//CheckCachedParameters();
		//UpdateTopMouseCoversControl();

		//CurrentCursor = defaultCursor;

		//было, не нужно
		//OnRenderWithChildren();
		//}

		void DrawCursor( CanvasRenderer renderer )
		{
			//hide cursor for mouse relative mode
			if( MouseRelativeMode )
				currentCursor = null;

			//!!!!!!focused иначе проверять
			//!!!!!!EngineApp.Instance.ApplicationWindow.Focused?
			if( !string.IsNullOrEmpty( currentCursor ) )//!!!!!EngineApp.Instance.ApplicationWindow.Focused/*IsWindowFocused()*/ && EngineApp.Instance.IsCreated && !LongOperationCallbackManager.DuringCallingCallback )
			{
				//!!!!Wait
				var textureIns = ResourceManager.LoadResource<Component_Image>( currentCursor );
				GpuTexture texture = textureIns?.Result;
				if( texture != null )
				{
					//!!!!было
					//texture._ThisIsGuiTexture = true;

					if( drawCursorWithPerPixelAccuracy )
					{
						Vector2 screenPixelSize = viewport.SizeInPixels.ToVector2();

						Vector2 m = MousePosition * screenPixelSize;
						Vector2 leftTop = m - texture.SourceSize.ToVector2F() * .5f;
						Vector2 rightBottom = m + texture.SourceSize.ToVector2F() * .5f;

						//!!!!!
						//per pixel alignment
						//if( this is UIContainerScreen )
						{
							leftTop = new Vector2( (int)( leftTop.X + .9999f ), (int)( leftTop.Y + .9999f ) );

							//!!!!
							//if( RenderSystem.Instance.IsDirect3D() )
							//	leftTop -= new Vec2( .5f, .5f );

							rightBottom = new Vector2( (int)( rightBottom.X + .9999f ), (int)( rightBottom.Y + .9999f ) );
							//!!!!
							//if( RenderSystem.Instance.IsDirect3D() )
							//	rightBottom -= new Vec2( .5f, .5f );
						}

						leftTop /= screenPixelSize;
						rightBottom /= screenPixelSize;

						Rectangle rectangle = new Rectangle( leftTop, rightBottom );
						renderer.AddQuad( rectangle, new Rectangle( 0, 0, 1, 1 ), textureIns, new ColorValue( 1, 1, 1 ), true );
					}
					else
					{
						double baseHeight = UIControlsWorld.ScaleByResolutionBaseHeight;
						Vector2 size = texture.SourceSize.ToVector2F() / new Vector2( baseHeight * renderer.AspectRatio, baseHeight );

						Rectangle rectangle = new Rectangle( -size / 2, size / 2 ) + MousePosition;

						renderer.AddQuad( rectangle, new Rectangle( 0, 0, 1, 1 ), textureIns, new ColorValue( 1, 1, 1 ), true );
					}
				}
			}
		}

		public void PerformRenderUI( CanvasRenderer renderer )
		{
			CheckCachedParameters();
			UpdateCachedCoverControls();

			OnRenderUIWithChildren_NonTopMostMode( renderer );
			OnRenderUIWithChildren_TopMostMode( renderer, false );

			//draw cursor
			DrawCursor( renderer );

			//before was in DoRender().
			CurrentCursor = defaultCursor;
		}

		public virtual void PlaySound( string name )
		{
			if( string.IsNullOrEmpty( name ) )
				return;

			var mode = Transform3D != null ? SoundModes.Mode3D : 0;
			Sound sound = SoundWorld.SoundCreate( name, mode );
			if( sound == null )
				return;

			//!!!!attachedToScene
			var channel = SoundWorld.SoundPlay( null, sound, EngineApp.DefaultSoundChannelGroup, 0.5, true );
			if( channel != null )
			{
				if( Transform3D != null )
				{
					channel.Position = Transform3D.Position;
					//channel.Velocity = xxx;
				}
				channel.Pause = false;
			}
		}

		public virtual void PlaySound( Component_Sound sound )
		{
			var mode = Transform3D != null ? SoundModes.Mode3D : 0;
			var sound2 = sound?.Result?.LoadSoundByMode( mode );
			if( sound2 == null )
				return;

			//!!!!attachedToScene
			var channel = SoundWorld.SoundPlay( null, sound2, EngineApp.DefaultSoundChannelGroup, 0.5, true );
			if( channel != null )
			{
				if( Transform3D != null )
				{
					channel.Position = Transform3D.Position;
					//channel.Velocity = xxx;
				}
				channel.Pause = false;
			}
		}

		public string DefaultCursor
		{
			get { return defaultCursor; }
			set { defaultCursor = value; }
		}

		public string CurrentCursor
		{
			get { return currentCursor; }
			set { currentCursor = value; }
		}

		public bool DrawCursorWithPerPixelAccuracy
		{
			get { return drawCursorWithPerPixelAccuracy; }
			set { drawCursorWithPerPixelAccuracy = value; }
		}

		//!!!!
		public void LostManagerFocus()
		{
			PerformMouseMove( new Vector2( .5f, .5f ) );
			PerformMouseUp( EMouseButtons.Left );
			PerformMouseUp( EMouseButtons.Right );
			focusedControl = null;
			capturedControl = null;
		}

		//public bool IsControlFocused()
		//{
		//	return focusedControl != null;
		//}

		public float AspectRatio
		{
			get { return aspectRatio; }
		}

		//internal abstract Vector2 GetSizeInPixels();

		internal virtual void CheckCachedParameters()
		{
			if( viewport.SizeInPixels != cachedViewportSize )
			{
				cachedViewportSize = viewport.SizeInPixels;
				ResetCachedScreenRectangleRecursive();
			}

			if( viewport.CanvasRenderer.AspectRatio != aspectRatio )
			{
				aspectRatio = viewport.CanvasRenderer.AspectRatio;
				ResetCachedScreenRectangleRecursive();
				//!!!!было
				//AfterUpdateAspectRatioRecursive();
			}
		}

		internal Vector2 ContainerGetMousePosition()
		{
			return controlManagerMousePosition;
		}

		public override bool MouseRelativeMode
		{
			get { return viewport.MouseRelativeMode; }
		}

		public Transform Transform3D
		{
			get { return transform3D; }
			set { transform3D = value; }
		}

		//internal abstract bool GetMouseRelativeMode();

		//public override bool MouseRelativeMode
		//{
		//	get { return GetMouseRelativeMode(); }
		//}
	}
}
