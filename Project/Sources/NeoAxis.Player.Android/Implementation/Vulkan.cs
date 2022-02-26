// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if VULKAN

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
	//!!!!
	class VulkanView : SurfaceView, ISurfaceHolderCallback
	{
		protected IntPtr nativeWindow = IntPtr.Zero;
		//!!!!
		bool appPaused = true;

		//!!!!
		System.Threading.Timer _tickTimer;

		//!!!!
		static readonly int _renderDueTime = (int)Math.Ceiling( 1000.0f / 60.0f );

		///////////////////////////////////////////////

		[DllImport( "android" )]
		internal static unsafe extern IntPtr ANativeWindow_fromSurface( IntPtr jniEnv, IntPtr handle );

		//!!!!вызывать
		[DllImport( "android" )]
		internal static unsafe extern void ANativeWindow_release( IntPtr window );

		///////////////////////////////////////////////

		public VulkanView( Context context )
			: base( context )
		{
			SetWillNotDraw( false );
			Holder.AddCallback( this );
		}

		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();

			//!!!!
			//aNativeWindow = NativeMethods.ANativeWindow_fromSurface( JniEnvironment.EnvironmentPointer, Holder.Surface.Handle );

			//zzzzzz;

			//var z = this.WindowId.Handle;

		}

		protected override void OnSizeChanged( int w, int h, int oldw, int oldh )
		{
			base.OnSizeChanged( w, h, oldw, oldh );

			//!!!!в двух местах
			PlatformFunctionalityAndroid.screenSize = new Vector2I( w, h );

			//!!!!по таймеру обновлять?
			//Invalidate();
		}

		//!!!!
		//protected override void OnDraw( Canvas canvas )
		//{
		//	base.OnDraw( canvas );

		//	//!!!!
		//	Draw();
		//}

		//!!!!
		void Tick()
		{
			Draw();
		}

		void Draw()
		{
			//!!!!

			//if( aNativeWindow == IntPtr.Zero )
			//	aNativeWindow = ANativeWindow_fromSurface( JniEnvironment.EnvironmentPointer, Holder.Surface.Handle );

			//!!!!
			//if( aNativeWindow == IntPtr.Zero )
			//	return;

			//engine initialization
			if( !Engine.engineInitialized )
			{
				try
				{
					//!!!!
					EngineApp.InitSettings.UseApplicationWindowHandle = nativeWindow;

					//EngineApp.InitSettings.UseApplicationWindowHandle = this.WindowId.Handle;

					Engine.InitEngine();
				}
				catch( Exception e )
				{
					Log.FatalAsException( e.ToString() );
					return;
				}
				Engine.engineInitialized = true;
			}

			//if( !Engine.engineInitialized )
			//	return;

			//UI thread can be changed when initially app ran with disabled screen. update it.
			VirtualFileSystem.SetMainThread( Thread.CurrentThread );

			//process input
			Engine.ProcessTouchEvents();

			//engine tick and render
			EngineApp._CreatedWindow_ApplicationIdle( false );

			if( EngineApp.NeedExit )
			{
				Java.Lang.JavaSystem.Exit( 0 );
				//Engine.mainActivity?.FinishAffinity();
			}


			//while( true )
			//{
			//	EngineUpdate();

			//	//!!!!zero?
			//	Thread.Sleep( 0 );
			//}

			//!!!!

			//SharpBgfx.Bgfx.ManuallyRenderFrame();

			//while( SharpBgfx.Bgfx.ManuallyRenderFrame() != SharpBgfx.RenderFrameResult.Exiting )
			//{
			//}

			//#if _FF

			//if( !MainActivity.created )
			//	return;

			////!!!!temp
			//if( !mainThreadInitialized )
			//{
			//	mainThreadInitialized = true;
			//	VirtualFileSystem._SetMainThread( Thread.CurrentThread );
			//}

			//if( EngineApp.NeedExit )
			//	return;
			//EngineUpdate();
			//#endif

		}

		public void SurfaceCreated( ISurfaceHolder holder )
		{
			if( nativeWindow == IntPtr.Zero )
				nativeWindow = ANativeWindow_fromSurface( JniEnvironment.EnvironmentPointer, Holder.Surface.Handle );
			appPaused = false;

			//_gameTimer.Start();
			_tickTimer = new System.Threading.Timer( state =>
			{
				Application.SynchronizationContext.Send( _ => { if( !appPaused ) Tick(); }, state );

				_tickTimer?.Change( _renderDueTime, Timeout.Infinite );

			}, null, _renderDueTime, Timeout.Infinite );
		}

		public void SurfaceDestroyed( ISurfaceHolder holder )
		{
			//!!!!

			if( _tickTimer != null )
			{
				_tickTimer.Change( Timeout.Infinite, Timeout.Infinite );
				_tickTimer.Dispose();
				_tickTimer = null;
			}

			//!!!!
			//if( nativeWindow != IntPtr.Zero )
			//{
			//	ANativeWindow_release( nativeWindow );
			//	nativeWindow = IntPtr.Zero;
			//}

			appPaused = true;
		}

		public void SurfaceChanged( ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height )
		{
			//!!!!в двух местах
			//PlatformFunctionalityAndroid.screenSize = new Vector2I( width, height );
		}

	}

}

#endif