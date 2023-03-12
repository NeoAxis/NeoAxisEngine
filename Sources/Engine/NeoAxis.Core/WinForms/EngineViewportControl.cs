// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
#if WINDOWS
using DirectInput;
#endif
using Internal.ComponentFactory.Krypton.Toolkit;
using System.IO;
using System.Diagnostics;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a viewport of the engine for Windows Forms application.
	/// </summary>
	public partial class EngineViewportControl : EUserControl
	{
		static List<EngineViewportControl> allInstances = new List<EngineViewportControl>();

		//render window and camera
		internal RenderWindow renderWindow;
		internal Viewport viewport;

		//!!!!!лучше чтобы окно и виевпорт не пересоздавались. тогда в них всё останется. чувак не будет париться, чтобы заного параметры выставлять или типа того
		bool needCreateRenderWindow;
		bool allowCreateRenderWindow = true;
		internal bool disableRecreationRenderWindow;

		//!!!!!
		//update render target
		float automaticUpdateFPS = 60;//!!!!30; //!!!!! = 0?
		Timer updateTimer;
		double lastRenderTime;

		Cursor oneFrameChangeCursor;

		//!!!!temp?
		//bool invalidated;

		bool loaded;

		double splashScreenStartTime;

		static Cursor hidedCursor;

		int paintBackgroundCounter;

		bool insideTryRender;

		[Browsable( false )]
		public object TransformTool { get; set; }

		/////////////////////////////////////////

		public EngineViewportControl()
		{
			InitializeComponent();
		}

		public static List<EngineViewportControl> AllInstances
		{
			get { return allInstances; }
		}

		//!!!!
		[Browsable( false )]
		public bool AllowCreateRenderWindow
		{
			get { return allowCreateRenderWindow; }
			set { allowCreateRenderWindow = value; }
		}

		[Browsable( false )]
		public bool Visible
		{
			get { return base.Visible; }
			set { base.Visible = value; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			needCreateRenderWindow = true;
			//!!!!тут создавать?
			//!!!!!что-то opengl иногда при создании ошибку дает
			//if( CreateRenderTarget() )
			//	needCreateRenderWindow = false;

			float interval = 10;
			// ( automaticUpdateFPS != 0 ) ? ( ( 1.0f / automaticUpdateFPS ) * 1000.0f ) : 100;
			//float interval = ( automaticUpdateFPS != 0 ) ? ( ( 1.0f / automaticUpdateFPS ) * 1000.0f ) : 100;
			updateTimer = new Timer();
			updateTimer.Interval = (int)interval;
			updateTimer.Tick += updateTimer_Tick;
			updateTimer.Enabled = true;

			//!!!!
			//WinFormsAppWorld.renderTargetUserControls.Add( this );

			loaded = true;

			allInstances.Add( this );
		}

		public delegate void DestroyEventDelegate( EngineViewportControl sender );
		public event DestroyEventDelegate DestroyEvent;

		protected override void OnDestroy()
		{
			allInstances.Remove( this );

			if( updateTimer != null )
			{
				updateTimer.Dispose();
				updateTimer = null;
			}

			DestroyEvent?.Invoke( this );

			DestroyRenderTarget();

			//!!!!
			//WinFormsAppWorld.renderTargetUserControls.Remove( this );

			base.OnDestroy();
		}

		void updateTimer_Tick( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;
			if( !IsHandleCreated )
				return;
			//!!!!надо ли?
			if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
				return;

			//!!!!

			if( automaticUpdateFPS == 0 )
				UpdateInput();

			////!!!!
			////if( WinFormsAppWorld.DuringWarningOrErrorMessageBox )
			////	return;

			//	if( viewport != null )
			//	UpdateKeysAndMouseButtons();

			////!!!!
			////if( automaticUpdateFPS != 0 )
			////{
			////	if( !invalidated )//!!!!temp?
			////	{
			////		invalidated = true;
			////		Invalidate();
			////	}
			////}

			////!!!!!так? может проверки какие
			//if( viewport != null && viewport.MouseRelativeMode )
			//	MouseRelativeModeResetPosition();
		}

		public delegate void ViewportCreatedDelegate( EngineViewportControl sender );
		public event ViewportCreatedDelegate ViewportCreated;

		public delegate void ViewportDestroyedDelegate( EngineViewportControl sender );
		public event ViewportDestroyedDelegate ViewportDestroyed;

		bool CreateRenderTarget()
		{
			DestroyRenderTarget();

			if( EngineApp.Instance == null )
				return false;

			Vector2I size = new Vector2I( ClientRectangle.Size.Width, ClientRectangle.Size.Height );
			if( size.X < 1 || size.Y < 1 )
				return false;

			renderWindow = RenderingSystem.CreateRenderWindow( Handle, size );
			if( renderWindow == null )
				return false;

			viewport = renderWindow.AddViewport( true, true );
			viewport.MousePositionSetImpl = MousePositionSet;
			viewport.MouseRelativeModeSetImpl = MouseRelativeModeSet;

			ViewportCreated?.Invoke( this );

			//apply DpiScaleFactor to canvas default font size
			var defaultSize = 0.017;
			try
			{
				defaultSize *= DpiHelper.Default.DpiScaleFactor;
			}
			catch { }
			viewport.CanvasRenderer.DefaultFontSize = defaultSize;
			//viewport.CanvasRenderer.DefaultFontSize = 0.02 * DpiHelper.Default.DpiScaleFactor;
			//var fontHeight = viewport.CanvasRenderer.DefaultFontSize * DpiHelper.Default.DpiScaleFactor;
			//viewport.CanvasRenderer.DefaultFont = EngineFontManager.Instance.LoadFont( "Default", fontHeight );
			//if( viewport.CanvasRenderer.DefaultFont == null )
			//	viewport.CanvasRenderer.DefaultFont = EngineFontManager.Instance.LoadFont( "Default", "English", fontHeight );

			return true;
		}

		//!!!!new: public
		public void DestroyRenderTarget()
		{
			if( renderWindow != null )
			{
				ViewportDestroyed?.Invoke( this );

				renderWindow.Dispose();
				viewport = null;
				renderWindow = null;

				////!!!!new
				//needCreateRenderWindow = true;
			}
		}

		protected void PerformResize()
		{
			//update render window
			if( renderWindow != null )
			{
				Vector2I size = new Vector2I( ClientRectangle.Size.Width, ClientRectangle.Size.Height );
				if( size.X >= 1 && size.Y >= 1 )
					renderWindow.WindowMovedOrResized( size );// false );
			}
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( !ParentFormResizing )
				PerformResize();

			paintBackgroundCounter = 3;

			//!!!!?
			//Invalidate();
		}

		protected override void OnParentFormResizeEnd( EventArgs e )
		{
			base.OnParentFormResizeEnd( e );

			PerformResize();
		}

		protected override void OnPaintBackground( PaintEventArgs e )
		{
			if( paintBackgroundCounter > 0 )
				paintBackgroundCounter--;

			if( renderWindow == null || ParentFormResizing || paintBackgroundCounter != 0 )//!!!!|| WinFormsAppWorld.DuringWarningOrErrorMessageBox )
				base.OnPaintBackground( e );
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

#if !DEPLOY
			if( Viewport != null )
			{
				var drawSplashScreen = DrawSplashScreen;
				if( drawSplashScreen != ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled )
				{
					var color = drawSplashScreen == ProjectSettingsPage_General.EngineSplashScreenStyleEnum.BlackBackground ? Color.Black : Color.White;
					using( var brush = new SolidBrush( color ) )
					{
						//e.Graphics.FillRectangle( brush, ClientRectangle );

						var screenSize = Viewport.SizeInPixels;
						var center = new Vector2( screenSize.X / 2, screenSize.Y / 2 );

						using( var image = EngineInfo.GetSplashLogoImage( drawSplashScreen ) )
						{
							var imageSize = new Vector2F( image.Width, image.Height );

							var destRect = new System.Drawing.RectangleF( (float)center.X - imageSize.X / 2, (float)center.Y - imageSize.Y / 2, imageSize.X, imageSize.Y );

							if( destRect.Left > 0 )
								e.Graphics.FillRectangle( brush, 0, 0, destRect.Left, screenSize.Y );
							if( destRect.Right < screenSize.X )
								e.Graphics.FillRectangle( brush, destRect.Right, 0, screenSize.X - destRect.Right, screenSize.Y );
							if( destRect.Top > 0 )
								e.Graphics.FillRectangle( brush, 0, 0, screenSize.X, destRect.Top );
							if( destRect.Bottom < screenSize.Y )
								e.Graphics.FillRectangle( brush, 0, destRect.Bottom, screenSize.X, screenSize.Y - destRect.Bottom );

							e.Graphics.DrawImage( image, destRect, new System.Drawing.RectangleF( 0, 0, imageSize.X, imageSize.Y ), GraphicsUnit.Pixel );
						}
					}
				}
			}
#endif

			////!!!!new. надо ли. поставлено т.к. валилось в UpdateTransformToolObjects на "foreach( var obj in transformTool.Objects )" в OnPaint, в Tick
			//if( !loaded )
			//	return;
			//if( EngineApp.Instance == null )
			//	return;
			////!!!!!
			////if( WinFormsAppWorld.DuringWarningOrErrorMessageBox )
			////	return;
			////if( RenderSystem.IsDeviceLostByTestCooperativeLevel() )
			////	return;


			////!!!!в OnPaint рисовать?

			////create render window
			//if( needCreateRenderWindow && allowCreateRenderWindow && EngineApp.Instance != null && EngineApp.Instance.IsCreated )
			//{
			//	if( CreateRenderTarget() )
			//		needCreateRenderWindow = false;
			//}

			////!!!!где еще такое RendererWorld.ViewportsDuringUpdate.Contains(viewport)

			//if( renderWindow != null && !RendererWorld.ViewportsDuringUpdate.Contains( viewport ) )
			//{
			//	double time = EngineApp.Instance.EngineTime;
			//	if( lastRenderTime == 0 )
			//		lastRenderTime = time;

			//	double step = time - lastRenderTime;
			//	lastRenderTime = time;

			//	//!!!!так?
			//	OneFrameChangeCursor = Cursors.Default;

			//	//!!!!так ли. где еще
			//	viewport.PerformTick( step );

			//	//!!!!
			//	////tick and entity world tick
			//	//if( WinFormsAppEngineApp.Instance != null )
			//	//	WinFormsAppEngineApp.Instance.DoTick();

			//	//!!!!!было
			//	//if( renderWindow.Size.X != 0 && renderWindow.Size.Y != 0 )
			//	//	camera.AspectRatio = (float)renderWindow.Size.X / (float)renderWindow.Size.Y;
			//	//if( renderWindow.Size.X != 0 && renderWindow.Size.Y != 0 )
			//	//	guiRenderer.AspectRatio = (float)renderWindow.Size.X / (float)renderWindow.Size.Y;

			//	//update
			//	//!!!!так?
			//	viewport.Update();
			//	//!!!!wait vsync?
			//	viewport.Parent.SwapBuffers( true );
			//	//renderWindow.Update( true );

			//	//!!!!так?
			//	if( Cursor != OneFrameChangeCursor )
			//		Cursor = OneFrameChangeCursor;
			//	OneFrameChangeCursor = Cursors.Default;
			//}

			//invalidated = false;
		}

		//!!!!!ниже много

		//public delegate void RenderDelegate( RenderTargetUserControl sender, Camera camera );
		//public event RenderDelegate Render;

		//public delegate void PostRenderDelegate( RenderTargetUserControl sender );
		//public event PostRenderDelegate PostRender;

		//public delegate void RenderUIDelegate( RenderTargetUserControl sender, GuiRenderer renderer );
		//public event RenderUIDelegate RenderUI;

		//protected virtual void OnRender( Camera camera )
		//{
		//	if( Render != null )
		//		Render( this, camera );

		//	if( controlManager != null )
		//		controlManager.DoRender();
		//}

		//protected virtual void OnRenderUI( GuiRenderer renderer )
		//{
		//	if( RenderUI != null )
		//		RenderUI( this, renderer );

		//	if( Map.Instance != null )
		//		Map.Instance.DoRenderUI( renderer );

		//	if( controlManager != null )
		//		controlManager.DoRenderUI( guiRenderer );
		//}

		//protected virtual void OnPostRender()
		//{
		//	if( PostRender != null )
		//		PostRender( this );
		//}

		bool GetEKeyByKeyCode( Keys keyCode, out EKeys eKey )
		{
			if( Enum.IsDefined( typeof( EKeys ), (int)keyCode ) )
			{
				eKey = (EKeys)(int)keyCode;
				return true;
			}
			else
			{
				eKey = EKeys.Cancel;
				return false;
			}
		}

		// alternatively we can use https://msdn.microsoft.com/en-us/library/system.windows.forms.control.previewkeydown.aspx
		protected override bool IsInputKey( Keys keyData )
		{
			switch( keyData & ~Keys.Shift )
			{
			case Keys.Left:
			case Keys.Right:
			case Keys.Up:
			case Keys.Down:
				return true;
			}
			return base.IsInputKey( keyData );
		}

		protected override void OnKeyDown( KeyEventArgs e )
		{
			base.OnKeyDown( e );

			//!!!!перед base.OnKeyDown вызывать? или в KeyUp иначе?
			if( viewport != null )
			{
				EKeys eKey;
				if( GetEKeyByKeyCode( e.KeyCode, out eKey ) )
				{
					KeyEvent keyEvent = new KeyEvent( eKey );
					bool handled = false;
					viewport.PerformKeyDown( keyEvent, ref handled );
					if( handled )
						e.Handled = true;
					if( keyEvent.SuppressKeyPress )
						e.SuppressKeyPress = true;
				}
			}
		}

		protected override void OnKeyPress( KeyPressEventArgs e )
		{
			base.OnKeyPress( e );

			if( viewport != null )
			{
				KeyPressEvent keyEvent = new KeyPressEvent( e.KeyChar );
				bool handled = false;
				viewport.PerformKeyPress( keyEvent, ref handled );
				if( handled )
					e.Handled = true;
			}
		}

		protected override void OnKeyUp( KeyEventArgs e )
		{
			if( viewport != null )
			{
				EKeys eKey;
				if( GetEKeyByKeyCode( e.KeyCode, out eKey ) )
				{
					KeyEvent keyEvent = new KeyEvent( eKey );
					bool handled = false;
					viewport.PerformKeyUp( keyEvent, ref handled );
					if( handled )
						e.Handled = true;
					if( keyEvent.SuppressKeyPress )
						e.SuppressKeyPress = true;
				}
			}

			base.OnKeyUp( e );
		}

		EMouseButtons GetEMouseButtonByMouseButton( MouseButtons button )
		{
			if( button == MouseButtons.Left )
				return EMouseButtons.Left;
			else if( button == MouseButtons.Right )
				return EMouseButtons.Right;
			else if( button == MouseButtons.Middle )
				return EMouseButtons.Middle;
			else if( button == MouseButtons.XButton1 )
				return EMouseButtons.XButton1;
			else
				return EMouseButtons.XButton2;
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );

			if( viewport != null )
			{
				PerformMouseMove();
				bool handled = false;
				viewport.PerformMouseDown( GetEMouseButtonByMouseButton( e.Button ), ref handled );
			}
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			if( viewport != null )
			{
				PerformMouseMove();
				bool handled = false;
				viewport.PerformMouseUp( GetEMouseButtonByMouseButton( e.Button ), ref handled );
			}
		}

		protected override void OnMouseDoubleClick( MouseEventArgs e )
		{
			base.OnMouseDoubleClick( e );

			if( viewport != null )
			{
				PerformMouseMove();
				bool handled = false;
				viewport.PerformMouseDoubleClick( GetEMouseButtonByMouseButton( e.Button ), ref handled );
			}
		}

		protected override void OnMouseWheel( MouseEventArgs e )
		{
			base.OnMouseWheel( e );

			if( viewport != null )
			{
				PerformMouseMove();
				bool handled = false;
				viewport.PerformMouseWheel( e.Delta, ref handled );
			}
		}

		public void PerformMouseMove()
		{
			if( viewport != null )
			{
				Vector2 result;
				if( viewport.MouseRelativeMode )
				{
					result = Vector2.Zero;

#if WINDOWS
					if( DirectInputMouseDevice.Instance != null )
					{
						DirectInputMouseDevice.State state = DirectInputMouseDevice.Instance.GetState();
						if( state.Position.X != 0 || state.Position.Y != 0 )
						{
							//!!!!strange, but works

							if( DirectInputMouseDevice.Instance.IsWindowAcquired )
							{
								result = new Vector2( state.Position.X, state.Position.Y );
							}
							else
							{
								result = new Vector2F(
									(float)state.Position.X / viewport.SizeInPixels.X,
									(float)state.Position.Y / viewport.SizeInPixels.Y );
							}
						}
					}
#endif
				}
				else
					result = GetFloatMousePosition();

				viewport.PerformMouseMove( result );
			}
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			if( viewport != null )
				PerformMouseMove();
		}

		protected override void OnMouseEnter( EventArgs e )
		{
			base.OnMouseEnter( e );

			if( viewport != null && viewport.MouseRelativeMode )
				UpdateShowCursor( false );
		}

		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );

			if( viewport != null )
				PerformMouseMove();

			if( viewport != null && viewport.MouseRelativeMode )
				UpdateShowCursor( true );
		}

		void MousePositionSet( Vector2 mouse )
		{
			Cursor.Position = PointToScreen( new Point(
				(int)( (double)ClientRectangle.Size.Width * mouse.X ),
				(int)( (double)ClientRectangle.Size.Height * mouse.Y ) ) );
		}

		void MouseRelativeModeResetPosition()
		{
			MousePositionSet( new Vector2( .5, .5 ) );
		}

		void MouseRelativeModeSet( bool enable )
		{
			if( enable )
			{
				UpdateShowCursor( false );

				if( Viewport != null )
					Viewport.MouseRelativeMode = true;
				Capture = true;

				//!!!!!
#if WINDOWS
				DirectInputMouseDevice.Instance?.Acquire( Handle );
#endif
			}
			else
			{
				//!!!!!
#if WINDOWS
				DirectInputMouseDevice.Instance?.Unacquire();
#endif

				if( Viewport != null )
					Viewport.MouseRelativeMode = false;
				Capture = false;

				UpdateShowCursor( true );
			}
		}

		protected override void OnLeave( EventArgs e )
		{
			if( viewport != null )
				viewport.KeysAndMouseButtonUpAll();
			lastRenderTime = 0;

			base.OnLeave( e );
		}

		//!!!!name
		/// <summary>
		/// If zero, then no automatic updates.
		/// </summary>
		[Browsable( false )]
		public float AutomaticUpdateFPS
		{
			get { return automaticUpdateFPS; }
			set
			{
				if( automaticUpdateFPS == value )
					return;
				automaticUpdateFPS = value;

				if( updateTimer != null )
				{
					float interval = ( automaticUpdateFPS != 0 ) ? ( ( 1.0f / automaticUpdateFPS ) * 1000.0f ) : 100;
					updateTimer.Interval = (int)interval;
				}
			}
		}

		Vector2 GetFloatMousePosition()
		{
			Point localPosition = PointToClient( MousePosition );
			Size size = ClientRectangle.Size;
			if( size.Width == 0 || size.Height == 0 )
				return Vector2.Zero;
			return new Vector2(
				(double)localPosition.X / (double)size.Width,
				(double)localPosition.Y / (double)size.Height );
		}

		[Browsable( false )]
		public RenderWindow RenderWindow
		{
			get { return renderWindow; }
		}

		[Browsable( false )]
		public Viewport Viewport
		{
			get { return viewport; }
		}

		[Browsable( false )]
		public Cursor OneFrameChangeCursor
		{
			get { return oneFrameChangeCursor; }
			set { oneFrameChangeCursor = value; }
		}

		[DllImport( "user32.dll" )]
		static extern short GetKeyState( int nVirtKey );

		const int VK_LBUTTON = 0x01;
		const int VK_RBUTTON = 0x02;
		const int VK_MBUTTON = 0x04;
		const int VK_XBUTTON1 = 0x05;
		const int VK_XBUTTON2 = 0x06;

		void UpdateKeysAndMouseButtons()
		{
			//mouse buttons
			{
				List<ValueTuple<EMouseButtons, int>> buttons = new List<(EMouseButtons, int)>();
				buttons.Add( (EMouseButtons.Left, VK_LBUTTON) );
				buttons.Add( (EMouseButtons.Right, VK_RBUTTON) );
				buttons.Add( (EMouseButtons.Middle, VK_MBUTTON) );
				buttons.Add( (EMouseButtons.XButton1, VK_XBUTTON1) );
				buttons.Add( (EMouseButtons.XButton2, VK_XBUTTON2) );

				foreach( var tuple in buttons )
				{
					if( viewport.IsMouseButtonPressed( tuple.Item1 ) && GetKeyState( tuple.Item2 ) >= 0 )
					{
						bool handled = false;
						viewport.PerformMouseUp( tuple.Item1, ref handled );
					}
				}
			}

			//keys
			foreach( EKeys eKey in Viewport.AllKeys )
			{
				if( viewport.IsKeyPressed( eKey ) )
				{
					int keyCode = (int)eKey;
					if( Enum.IsDefined( typeof( EKeys ), keyCode ) && GetKeyState( keyCode ) >= 0 )
					{
						KeyEvent keyEvent = new KeyEvent( eKey );
						bool handled = false;
						viewport.PerformKeyUp( keyEvent, ref handled );
					}
				}
			}
		}

		public void UpdateInput()
		{
			//!!!!!так? может проверки какие

			if( viewport != null )
			{
				UpdateKeysAndMouseButtons();

				if( viewport.MouseRelativeMode )
					MouseRelativeModeResetPosition();
			}
		}

		public bool IsAllowRender()
		{
			if( IsDisposed )
				return false;

			if( EngineApp.Instance == null )
				return false;

			//!!!!new. надо ли. поставлено т.к. валилось в UpdateTransformToolObjects на "foreach( var obj in transformTool.Objects )" в OnPaint, в Tick
			if( !loaded )
				return false;

			//if( WinFormsAppWorld.DuringWarningOrErrorMessageBox )
			//	return;
			//if( RenderSystem.IsDeviceLostByTestCooperativeLevel() )
			//	return;

			try
			{
				if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
					return false;

				if( !WinFormsUtility.IsPhysicalVisibleCheckBy5Points( this ) )
					return false;
			}
			catch
			{
				return false;
			}

			return true;
		}

		public bool IsTimeToUpdate()
		{
			double time = EngineApp.GetSystemTime();
			double step = time - lastRenderTime;

			float invFPS = 0;
			if( automaticUpdateFPS != 0 )
				invFPS = 1.0f / automaticUpdateFPS;

			if( step >= invFPS )
				return true;

			return false;
		}

		[Browsable( false )]
		public Viewport.CameraSettingsClass OverrideCameraSettings { get; set; }

		public void TryRender()
		{
			if( DrawSplashScreen != ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled )
				return;

			if( !IsAllowRender() )
			{
				////!!!!new
				//DestroyRenderTarget();

				return;
			}

			if( EditorAPI.ApplicationDoEventsIsAdditionalCall )
				return;
			if( insideTryRender )
				return;

			insideTryRender = true;
			try
			{
				UpdateInput();


				//!!!!в OnPaint рисовать?

				//create render window
				if( needCreateRenderWindow && allowCreateRenderWindow && !disableRecreationRenderWindow && EngineApp.Instance != null && EngineApp.Created )
				{
					if( CreateRenderTarget() )
						needCreateRenderWindow = false;
				}

				//!!!!где еще такое RendererWorld.ViewportsDuringUpdate.Contains(viewport)

				if( renderWindow != null && !RenderingSystem.ViewportsDuringUpdate.Contains( viewport ) )
				{
					//!!!!?
					double time = EngineApp.GetSystemTime();
					//double time = EngineApp.Instance.EngineTime;
					if( lastRenderTime == 0 )
						lastRenderTime = time;

					double step = time - lastRenderTime;

					float invFPS = 0;
					if( automaticUpdateFPS != 0 )
						invFPS = 1.0f / automaticUpdateFPS;

					if( automaticUpdateFPS == 0 || step >= invFPS )
					{
						lastRenderTime = time;

						OneFrameChangeCursor = viewport.MouseRelativeMode ? GetHidedCursor() : Cursors.Default;
						//OneFrameChangeCursor = Cursors.Default;

						EngineApp.DoTick();

						//!!!!так ли. где еще
						if( DrawSplashScreen == ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled )
							viewport.PerformTick( (float)step );

						//!!!!
						////tick and entity world tick
						//if( WinFormsAppEngineApp.Instance != null )
						//	WinFormsAppEngineApp.Instance.DoTick();

						//!!!!!было
						//if( renderWindow.Size.X != 0 && renderWindow.Size.Y != 0 )
						//	camera.AspectRatio = (float)renderWindow.Size.X / (float)renderWindow.Size.Y;
						//if( renderWindow.Size.X != 0 && renderWindow.Size.Y != 0 )
						//	guiRenderer.AspectRatio = (float)renderWindow.Size.X / (float)renderWindow.Size.Y;

						//update
						//!!!!так?
						viewport.Update( true, OverrideCameraSettings );// false );

						//!!!!возможно надо свапать только раз. делать цепь из всех существующих.

						//!!!!wait vsync?
						//viewport.Parent.SwapBuffers( false );
						//!!!!
						//Log.Fatal( "viewport.Parent.SwapBuffers( true )" );
						//viewport.Parent.SwapBuffers( true );
						//renderWindow.Update( true );

						//!!!!так?
						if( Cursor != OneFrameChangeCursor )
							Cursor = OneFrameChangeCursor;

						OneFrameChangeCursor = viewport.MouseRelativeMode ? GetHidedCursor() : Cursors.Default;
						//OneFrameChangeCursor = Cursors.Default;

						WinFormsUtility.InvalidateParentComposedStyleControl( this );
					}
				}
			}
			finally
			{
				insideTryRender = false;
			}
		}

		//public override string ToString()
		//{
		//	return GetType().Name + " : Parent=" + Parent?.ToString();
		//}

		[DllImport( "user32.dll" )]
		static extern int ShowCursor( int show );

		void UpdateShowCursor( bool show )
		{
			if( show )
			{
				while( ShowCursor( 1 ) < 0 ) { }
			}
			else
			{
				while( ShowCursor( 0 ) >= 0 ) { }
			}
		}

		[Browsable( false )]
		public virtual bool IsWidget
		{
			get { return false; }
		}

		[Browsable( false )]
		internal ProjectSettingsPage_General.EngineSplashScreenStyleEnum DrawSplashScreen
		{
			get
			{
				if( IsWidget )//&& !ProjectSettings.Get.CustomizeSplashScreen )
				{
					var result = ProjectSettings.Get.General.EngineSplashScreenStyle.Value;

					if( EngineApp.EngineTime != 0 )
					{
						if( splashScreenStartTime == 0 )
							splashScreenStartTime = EngineApp.EngineTime;

						//!!!!
						double totalTime = 3.0;
						//double totalTime = ProjectSettings.Get.EngineSplashScreenTime.Value;

						//double totalTime = EngineApp.IsProPlan ? ProjectSettings.Get.EngineSplashScreenTime.Value : ProjectSettings.Get.EngineSplashScreenTimeReadOnly;
						if( EngineApp.EngineTime - splashScreenStartTime > totalTime )
							result = ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled;
					}
					//else
					//	return true;

					return result;
				}

				return ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled;
			}
		}

		static Cursor GetHidedCursor()
		{
			if( hidedCursor == null )
			{
				var stream = new MemoryStream( Properties.Resources.EmptyCursor );
				hidedCursor = new Cursor( stream );
			}
			return hidedCursor;
		}

	}
}
#endif
