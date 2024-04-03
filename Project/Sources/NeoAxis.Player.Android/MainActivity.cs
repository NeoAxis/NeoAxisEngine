// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Content;
using Android.Views.InputMethods;
using Android.Runtime;
using System.Collections.Generic;
#if OPENGLES
using Android.Opengl;
#endif

namespace NeoAxis.Player.Android
{
	[Activity( Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = DefaultScreenOrientation, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize, WindowSoftInputMode = SoftInput.AdjustResize )]
	public class MainActivity : AppCompatActivity, View.IOnTouchListener
	{
		//general settings
		//defined by default screen orientation. you can override orientation in the scene, use Screen Orientation property
		public const ScreenOrientation DefaultScreenOrientation = ScreenOrientation.Unspecified;// ScreenOrientation.UserLandscape;

		bool fullscreen = true;
		bool keepScreenOn = true;

#if VULKAN
		VulkanView surfaceView;
#else
		GLSurfaceView surfaceView;
#endif

		RendererClass renderer;

		bool currentSoftInput;

		//

		protected override void OnCreate( Bundle savedInstanceState )
		{
			base.OnCreate( savedInstanceState );

			Engine.activity = this;

			//apply general settings
			if( fullscreen )
				Window.AddFlags( WindowManagerFlags.Fullscreen );
			if( keepScreenOn )
				Window.AddFlags( WindowManagerFlags.KeepScreenOn );

			StartupTiming.TotalStart();

			SetContentView( Resource.Layout.activity_main );

			//Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>( Resource.Id.toolbar );
			//SetSupportActionBar( toolbar );

#if VULKAN
			surfaceView = new VulkanView( this );

			//!!!!LinearLayout?
			var sceneHolder = (RelativeLayout)this.FindViewById( Resource.Id.sceneHolder );
			sceneHolder.AddView( surfaceView );

			surfaceView.SetOnTouchListener( this );
#else
			surfaceView = new GLSurfaceView( this );
			surfaceView.SetEGLContextClientVersion( 3 );

			//it's just recommendation, is not works for any device
			//right now after recreate surface event the engine app will be restarted
			surfaceView.PreserveEGLContextOnPause = true;

			//hdr
			//glSurfaceView.SetEGLConfigChooser( 8, 8, 8, 8, 24, 8 );

			renderer = new RendererClass();
			surfaceView.SetRenderer( renderer );

			var sceneHolder = (RelativeLayout)FindViewById( Resource.Id.sceneHolder );
			sceneHolder.AddView( surfaceView );

			surfaceView.SetOnTouchListener( this );
#endif

			//engineMainThread = new Thread( EngineMainThreadMethod );
			//engineMainThread.Start();
		}

		public override bool OnCreateOptionsMenu( IMenu menu )
		{
			MenuInflater.Inflate( Resource.Menu.menu_main, menu );
			return true;
		}

		public override bool OnOptionsItemSelected( IMenuItem item )
		{
			int id = item.ItemId;
			if( id == Resource.Id.action_settings )
				return true;

			return base.OnOptionsItemSelected( item );
		}

		protected override void OnPause()
		{
			base.OnPause();

			EngineApp.EnginePauseUpdateState( false, true );

#if OPENGLES
			surfaceView.OnPause();
#endif
		}

		protected override void OnResume()
		{
			base.OnResume();

#if OPENGLES
			surfaceView.OnResume();
#endif
		}

#if VULKAN
		public VulkanView VulkanView SurfaceView
		{
			get { return surfaceView; }
		}
#else
		public GLSurfaceView SurfaceView
		{
			get { return surfaceView; }
		}
#endif

		public RendererClass Renderer
		{
			get { return renderer; }
		}

		//void EngineMainThreadMethod()
		//{
		//	try
		//	{
		//		InitEngine();
		//	}
		//	catch( Exception e )
		//	{
		//		Log.FatalAsException( e.ToString() );
		//		return;
		//	}

		//	engineInitialized = true;

		//	while( true )
		//	{
		//		EngineUpdate();
		//		Thread.Sleep( 0 );
		//	}
		//}

		public void RestartApp()
		{
			Log.InvisibleInfo( "Restarting the app." );

			var intent = new Intent( this, typeof( MainActivity ) );
			intent.AddFlags( ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask );
			intent.AddCategory( Intent.CategoryDefault );

			Com.JakeWharton.ProcessPhoenix.ProcessPhoenix.TriggerRebirth( this, intent );
		}

		public void UpdateSoftInput()
		{
			var requiredSoftInput = false;
			var viewport = RenderingSystem.ApplicationRenderTarget?.Viewports[ 0 ];
			if( viewport != null )
			{
				var focusedControl = viewport.UIContainer?.FocusedControl;
				if( focusedControl != null )
				{
					var edit = focusedControl as UIEdit;
					if( edit != null && !edit.ReadOnlyInHierarchy )
						requiredSoftInput = true;
				}
			}

			try
			{
				if( requiredSoftInput != currentSoftInput )
				{
					currentSoftInput = requiredSoftInput;

					var view = SurfaceView;

					if( currentSoftInput )
					{
						var inputMethodManager = (InputMethodManager)GetSystemService( Context.InputMethodService );
						view.RequestFocus();
						inputMethodManager.ShowSoftInput( view, 0 );
						inputMethodManager.ToggleSoftInput( ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly );
					}
					else
					{
						//var currentFocus = CurrentFocus;
						//if( currentFocus != null )
						//{

						var inputMethodManager = (InputMethodManager)GetSystemService( Context.InputMethodService );
						inputMethodManager.HideSoftInputFromWindow( view.WindowToken, HideSoftInputFlags.None );

						//}
					}
				}
			}
			catch { }
		}

		public void UpdateScreenOrientation()
		{
			var orientation = DefaultScreenOrientation;

			if( Project.PlayScreen.Instance != null )
			{
				var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
				var scene = viewport.AttachedScene;
				if( scene != null )
				{
					switch( scene.ScreenOrientation.Value )
					{
					case Scene.ScreenOrientationEnum.Landscape:
						orientation = ScreenOrientation.UserLandscape;
						break;
					case Scene.ScreenOrientationEnum.Portrait:
						orientation = ScreenOrientation.UserPortrait;
						break;
					}
				}
			}

			RequestedOrientation = orientation;
		}

		public bool OnTouch( View v, MotionEvent e )
		{
			//MotionEvent properties become invalid when OnTouch is ended

			if( Engine.engineInitialized )
			{
				lock( Engine.inputEventQueue )
				{
					if( Engine.inputEventQueue.Count < 200 )
					{
						var item = new Engine.TouchEventItem();

						//it can't work. MotionEvent properties become invalid when OnTouch call is ended
						//item.View = v;
						//item.MotionEvent = e;

						item.Action = e.Action;
						item.ActionIndex = e.ActionIndex;
						item.ActionMasked = e.ActionMasked;

						item.PointersPosition = new Vector2F[ e.PointerCount ];
						item.PointersId = new int[ e.PointerCount ];

						for( int n = 0; n < item.PointersPosition.Length; n++ )
						{
							item.PointersPosition[ n ] = new Vector2F( e.GetX( n ), e.GetY( n ) );
							item.PointersId[ n ] = e.GetPointerId( n );
						}

						Engine.inputEventQueue.Enqueue( item );
					}
				}

				return true;
			}

			return false;
		}

		bool KeyDown( Keycode keyCode, global::Android.Views.KeyEvent e )
		{
			if( Engine.engineInitialized )
			{
				lock( Engine.inputEventQueue )
				{
					if( Engine.inputEventQueue.Count < 200 )
					{
						var item = new Engine.KeyDownEventItem();

						item.Character = (char)e.UnicodeChar;

						switch( e.KeyCode )
						{
						case Keycode.Del: item.KeyCode = EKeys.Back/*Delete*/; break;
						case Keycode.Enter: item.KeyCode = EKeys.Return; break;
						}

						Engine.inputEventQueue.Enqueue( item );
					}
				}

				return true;
			}

			return false;
		}

		public override bool OnKeyDown( [GeneratedEnum] Keycode keyCode, global::Android.Views.KeyEvent e )
		{
			//Log.Info( "unicode: " + ( (char)e.UnicodeChar ).ToString() + ", code: " + e.KeyCode.ToString() );

			if( KeyDown( keyCode, e ) )
				return true;

			return base.OnKeyDown( keyCode, e );
		}

		public override bool OnKeyUp( [GeneratedEnum] Keycode keyCode, global::Android.Views.KeyEvent e )
		{
			if( Engine.engineInitialized )
			{
				return true;
			}

			return base.OnKeyUp( keyCode, e );
		}

		public override bool OnKeyMultiple( [GeneratedEnum] Keycode keyCode, int repeatCount, global::Android.Views.KeyEvent e )
		{
			//!!!!need?

			//var result = false;
			//for( int n = 0; n < repeatCount; n++ )
			//{
			//	if( KeyDown( keyCode, e ) )
			//		result = true;
			//}
			//if( result )
			//	return true;

			return base.OnKeyMultiple( keyCode, repeatCount, e );
		}

		public override bool OnKeyLongPress( [GeneratedEnum] Keycode keyCode, global::Android.Views.KeyEvent e )
		{
			return base.OnKeyLongPress( keyCode, e );
		}

		public override bool OnKeyShortcut( [GeneratedEnum] Keycode keyCode, global::Android.Views.KeyEvent e )
		{
			return base.OnKeyShortcut( keyCode, e );
		}
	}
}