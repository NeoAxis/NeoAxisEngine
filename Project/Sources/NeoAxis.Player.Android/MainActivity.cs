// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
#if OPENGLES
using Android.Opengl;
#endif

namespace NeoAxis.Player.Android
{
	[Activity( Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = orientation, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize )]
	public class MainActivity : AppCompatActivity, View.IOnTouchListener
	{

		//general settings
		const ScreenOrientation orientation = ScreenOrientation.Unspecified;// ScreenOrientation.ReverseLandscape;
		bool fullscreen = true;
		bool keepScreenOn = true;


#if VULKAN
		VulkanView surfaceView;
#else
		GLSurfaceView glSurfaceView;
#endif

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
			RelativeLayout sceneHolder = (RelativeLayout)this.FindViewById( Resource.Id.sceneHolder );
			sceneHolder.AddView( surfaceView );

			surfaceView.SetOnTouchListener( this );
#else
			glSurfaceView = new GLSurfaceView( this );
			glSurfaceView.SetEGLContextClientVersion( 3 );

			//it's just recommendation, is not works for any device
			glSurfaceView.PreserveEGLContextOnPause = true;

			//!!!!hdr
			//glSurfaceView.SetEGLConfigChooser( 8, 8, 8, 8, 24, 8 );

			var renderer = new Renderer();
			glSurfaceView.SetRenderer( renderer );

			RelativeLayout sceneHolder = (RelativeLayout)this.FindViewById( Resource.Id.sceneHolder );
			sceneHolder.AddView( glSurfaceView );

			glSurfaceView.SetOnTouchListener( this );

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
			glSurfaceView.OnPause();
#endif
		}

		protected override void OnResume()
		{
			base.OnResume();

#if OPENGLES
			glSurfaceView.OnResume();
#endif
		}

		public bool OnTouch( View v, MotionEvent e )
		{
			//MotionEvent properties become invalid when OnTouch is ended

			if( Engine.engineInitialized )
			{
				lock( Engine.touchEventsQueue )
				{
					if( Engine.touchEventsQueue.Count < 200 )
					{
						var item = new Engine.TouchEventItem();

						//it can't work. MotionEvent properties become invalid when OnTouch is ended
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

						Engine.touchEventsQueue.Enqueue( item );
					}
				}

				return true;
			}

			return false;
		}

		//!!!!
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

		//	//!!!!temp
		//	Log.Info( "Engine has been initialized." );

		//	engineInitialized = true;

		//	//!!!!как корректно выходить?

		//	while( true )
		//	{
		//		EngineUpdate();

		//		//!!!!ноль?
		//		Thread.Sleep( 0 );
		//	}
		//}

	}
}