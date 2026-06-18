// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Content;
using Android.Views.InputMethods;
using Android.Runtime;
#if OPENGLES
using Android.Opengl;
#endif

// IMPORTANT: alias the generated resource class.
// If your generated resource class is not `NeoAxis.Player.Android.Resource`, change this alias accordingly.
using AppResource = NeoAxis.Player.Android.Resource;

namespace NeoAxis.Player.Android
{
	[Activity( 
		Label = "@string/app_name",
		Theme = "@android:style/Theme.DeviceDefault.NoActionBar", //Theme = "@style/AppTheme.NoActionBar", 
		MainLauncher = true, 
		ScreenOrientation = DefaultScreenOrientation, 
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize, 
		WindowSoftInputMode = SoftInput.AdjustResize 
	)]
	public class MainActivity : Activity, View.IOnTouchListener // AppCompatActivity, View.IOnTouchListener
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

		int GetResId( string name, string defType )
		{
			var id = Resources?.GetIdentifier( name, defType, PackageName ) ?? 0;
			if( id == 0 )
				throw new InvalidOperationException( $"Android resource not found: type='{defType}', name='{name}', package='{PackageName}'." );
			return id;
		}

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

			SetContentView( GetResId( "activity_main", "layout" ) );
			//SetContentView( AppResource.Layout.activity_main );

			//Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>( Resource.Id.toolbar );
			//SetSupportActionBar( toolbar );

#if VULKAN
			surfaceView = new VulkanView( this );

			//!!!!LinearLayout?
			var sceneHolder = (RelativeLayout)this.FindViewById( GetResId( "sceneHolder", "id" ) );
			//var sceneHolder = (RelativeLayout)this.FindViewById( AppResource.Id.sceneHolder );
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

			var sceneHolder = (RelativeLayout)FindViewById( GetResId( "sceneHolder", "id" ) );
			//var sceneHolder = (RelativeLayout)FindViewById( AppResource.Id.sceneHolder );
			sceneHolder.AddView( surfaceView );

			surfaceView.SetOnTouchListener( this );
#endif

			//engineMainThread = new Thread( EngineMainThreadMethod );
			//engineMainThread.Start();
		}

		public override bool OnCreateOptionsMenu( IMenu menu )
		{
			MenuInflater.Inflate( GetResId( "menu_main", "menu" ), menu );
			//MenuInflater.Inflate( AppResource.Menu.menu_main, menu );
			return true;
		}

		public override bool OnOptionsItemSelected( IMenuItem item )
		{
			int id = item.ItemId;
			if( id == GetResId( "action_settings", "id" ) )
				return true;
			//if( id == AppResource.Id.action_settings )
			//	return true;

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

					var inputMethodManager = (InputMethodManager)GetSystemService( Context.InputMethodService );

					if( currentSoftInput )
					{
						view.RequestFocus();
						// Explicitly show (avoids obsolete ToggleSoftInput on API 31+).
						inputMethodManager.ShowSoftInput( view, ShowFlags.Forced );
					}
					else
					{
						inputMethodManager.HideSoftInputFromWindow( view.WindowToken, HideSoftInputFlags.None );
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