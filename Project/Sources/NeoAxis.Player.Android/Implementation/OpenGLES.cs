// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if OPENGLES
using System;
using System.Collections.Generic;
using System.Threading;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;

namespace NeoAxis.Player.Android
{
	public class RendererClass : Java.Lang.Object, GLSurfaceView.IRenderer
	{
		volatile bool surfaceResized;

		//!!!!better to recreate gpu resources, but harder
		//restart app after surface recreate
		public bool needRestartEngine;

		/////////////////////////////////////////

		public void OnSurfaceCreated( IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config )
		{
			Log.InvisibleInfo( "Renderer: OnSurfaceCreated." );

			if( Engine.engineInitialized )
				needRestartEngine = true;
		}

		public void OnSurfaceChanged( IGL10 gl, int width, int height )
		{
			PlatformFunctionalityAndroid.screenSize = new Vector2I( width, height );
			surfaceResized = true;
		}

		public void OnDrawFrame( IGL10 gl )
		{
			//restart app after surface recreate
			if( needRestartEngine )
			{
				needRestartEngine = false;

				if( Engine.engineInitialized )
				{
					Engine.activity.RestartApp();
					return;

					////UI thread can be changed when initially app ran with disabled screen. update it.
					//VirtualFileSystem.SetMainThread( Thread.CurrentThread );

					//Engine.ShutdownEngine();

					//Engine.engineInitialized = false;
				}
			}

			//init engine
			if( !Engine.engineInitialized )
			{
				try
				{
					Engine.InitEngine();
				}
				catch( Exception e )
				{
					Log.FatalAsException( e.ToString() );
					return;
				}
				Engine.engineInitialized = true;
			}

			//UI thread can be changed when initially app ran with disabled screen. update it.
			VirtualFileSystem.SetMainThread( Thread.CurrentThread );

			//update screen size
			if( surfaceResized )
			{
				RenderingSystem.ApplicationRenderTarget?.WindowMovedOrResized( PlatformFunctionalityAndroid.screenSize );
				surfaceResized = false;
			}

			//process input
			Engine.ProcessInputEvents();
			Engine.activity.UpdateSoftInput();

			//engine tick and render
			EngineApp.CreatedWindowApplicationIdle( false );

			//update screen settings
			Engine.activity.UpdateScreenOrientation();

			if( EngineApp.NeedExit )
				Java.Lang.JavaSystem.Exit( 0 );
		}
	}
}
#endif