// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

[assembly: SupportedOSPlatform( "browser" )]
namespace NeoAxis.Player.Web
{
	public static class Program
	{
		public static Uri BaseAddress { get; internal set; }
		internal static bool surfaceResized = false;
		static int framesRendered;

		public static async Task Main( string[] args )
		{
			System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo( "en-US" );
			System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo( "en-US" );

			if( Debugger.IsAttached )
			{
				await Main2();
			}
			else
			{
				try
				{
					await Main2();
				}
				catch( Exception e )
				{
					Log.FatalAsException( e.ToString() );
				}
			}
		}

		[UnmanagedCallersOnly]
		public static int Frame( double time, nint userData )
		{
			if( surfaceResized )
			{
				surfaceResized = false;
				RenderingSystem.ApplicationRenderTarget?.WindowMovedOrResized( PlatformFunctionalityWeb.screenSize );
			}

			//!!!!
			//EngineApp.CreatedWindowApplicationIdle( false );

			if( framesRendered < 5 )
			{
				framesRendered++;
				if( framesRendered == 5 )
					Interop.HideLogo();
			}
			// //process input
			Engine.ProcessInputEvents();

			// //engine tick and render
			EngineApp.CreatedWindowApplicationIdle( false );

			// //update screen settings
			//Engine.UpdateScreenOrientation();

			//EngineApp.DoTick();

			if( EngineApp.NeedExit )
				return 0;
			return 1;
		}

		static async Task Main2()
		{
			Interop.Initialize();

			var client = new HttpClient();
			client.BaseAddress = BaseAddress;
			client.Timeout = new TimeSpan( 100, 0, 0 );

			if( await Engine.InitEngine( client ) )
			{
				VirtualFileSystem.SetMainThread( Thread.CurrentThread );

				unsafe
				{
					Emscripten.RequestAnimationFrameLoop( (delegate* unmanaged< double, nint, int >)&Frame, nint.Zero );
				}

				while( !EngineApp.NeedExit )
				{
					await Task.Yield();
				}
			}
			else
			{
				EngineApp.Shutdown();

				VirtualFileSystem.Shutdown();
			}
		}
	}
}
