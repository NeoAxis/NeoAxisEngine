// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if OPENGLES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Nio;
using Javax.Microedition.Khronos.Opengles;

namespace NeoAxis.Player.Android
{
	class Renderer : Java.Lang.Object, GLSurfaceView.IRenderer
	{
		volatile bool surfaceResized;

		//bool needRestartEngine;

		/////////////////////////////////////////

		public void OnSurfaceCreated( IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config )
		{
			//needRestartEngine = true;
		}

		public void OnSurfaceChanged( IGL10 gl, int width, int height )
		{
			PlatformFunctionalityAndroid.screenSize = new Vector2I( width, height );
			surfaceResized = true;
		}

		public void OnDrawFrame( IGL10 gl )
		{
			////restart engine
			//if( needRestartEngine )
			//{
			//	if( Engine.engineInitialized )
			//	{
			//		//!!!!restart event

			//		//UI thread can be changed when initially app ran with disabled screen. update it.
			//		VirtualFileSystem.SetMainThread( Thread.CurrentThread );

			//		Engine.ShutdownEngine();

			//		Engine.engineInitialized = false;
			//	}

			//	needRestartEngine = false;
			//}

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
			Engine.ProcessTouchEvents();

			//engine tick and render
			EngineApp.CreatedWindowApplicationIdle( false );

			if( EngineApp.NeedExit )
				Java.Lang.JavaSystem.Exit( 0 );

		}
	}
}

#endif