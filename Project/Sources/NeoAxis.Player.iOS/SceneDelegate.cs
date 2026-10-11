// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using CoreAnimation;
using Foundation;
using UIKit;

namespace NeoAxis.Player.iOS
{
	[Register( "SceneDelegate" )]
	public class SceneDelegate : UIResponder, IUIWindowSceneDelegate
	{
		[Export( "window" )]
		public UIKit.UIWindow? Window { get; set; }

		[Export( "scene:willConnectToSession:options:" )]
		public void WillConnect( UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions )
		{
			// Use this method to optionally configure and attach the UIWindow 'Window' to the provided UIWindowScene 'scene'.
			// Since we are not using a storyboard, the 'Window' property needs to be initialized and attached to the scene.
			// This delegate does not imply the connecting scene or session are new (see UIApplicationDelegate 'GetConfiguration' instead).
			if( scene is UIWindowScene windowScene )
			{
				Window ??= new UIKit.UIWindow( windowScene );

				//!!!!iOS: rendering view with CAMetalLayer for MoltenVK
				var view = new MetalView( Window.Bounds );
				var scale = windowScene.Screen.NativeScale;
				view.ContentScaleFactor = scale;
				view.Layer.ContentsScale = scale;
				var vc = new UIViewController();
				vc.View = view;

				PlatformFunctionalityIOS.screenSize = new Vector2I( (int)( view.Bounds.Width * scale ), (int)( view.Bounds.Height * scale ) );
				EngineApp.InitSettings.UseApplicationWindowHandle = view.Layer.Handle;

				Window.RootViewController = vc;
				Window.MakeKeyAndVisible();

				//start the engine after the window is shown
				CoreFoundation.DispatchQueue.MainQueue.DispatchAsync( () =>
				{
					Engine.InitEngine();
					Engine.engineInitialized = true;

					//!!!!iOS
					displayLink = CoreAnimation.CADisplayLink.Create( OnFrame );
					displayLink.AddToRunLoop( NSRunLoop.Main, NSRunLoopMode.Common );
				} );
			}
			}

		CoreAnimation.CADisplayLink? displayLink;

		void OnFrame()
		{
			Engine.ProcessInputEvents();
			EngineApp.CreatedWindowApplicationIdle( false );
		}

		[Export( "sceneDidDisconnect:" )]
		public void DidDisconnect( UIScene scene )
		{
			// Called as the scene is being released by the system.
			// This occurs shortly after the scene enters the background, or when its session is discarded.
			// Release any resources associated with this scene that can be re-created the next time the scene connects.
			// The scene may re-connect later, as its session was not neccessarily discarded (see UIApplicationDelegate `DidDiscardSceneSessions` instead).
		}

		[Export( "sceneDidBecomeActive:" )]
		public void DidBecomeActive( UIScene scene )
		{
			// Called when the scene has moved from an inactive state to an active state.
			// Use this method to restart any tasks that were paused (or not yet started) when the scene was inactive.
		}

		[Export( "sceneWillResignActive:" )]
		public void WillResignActive( UIScene scene )
		{
			// Called when the scene will move from an active state to an inactive state.
			// This may occur due to temporary interruptions (ex. an incoming phone call).
		}

		[Export( "sceneWillEnterForeground:" )]
		public void WillEnterForeground( UIScene scene )
		{
			// Called as the scene transitions from the background to the foreground.
			// Use this method to undo the changes made on entering the background.
		}

		[Export( "sceneDidEnterBackground:" )]
		public void DidEnterBackground( UIScene scene )
		{
			// Called as the scene transitions from the foreground to the background.
			// Use this method to save data, release shared resources, and store enough scene-specific state information
			// to restore the scene back to its current state.
		}
	}

	//!!!!iOS
	class MetalView : UIView
	{
		public MetalView( CoreGraphics.CGRect frame ) : base( frame ) 
		{
			MultipleTouchEnabled = true;
		}

		[Export( "layerClass" )]
		public static ObjCRuntime.Class GetLayerClass()
		{
			return new ObjCRuntime.Class( typeof( CoreAnimation.CAMetalLayer ) );
		}

		public override void TouchesBegan( NSSet touches, UIEvent? evt ) => AddTouches( touches, TouchData.ActionEnum.Down );
		public override void TouchesMoved( NSSet touches, UIEvent? evt ) => AddTouches( touches, TouchData.ActionEnum.Move );
		public override void TouchesEnded( NSSet touches, UIEvent? evt ) => AddTouches( touches, TouchData.ActionEnum.Up );
		public override void TouchesCancelled( NSSet touches, UIEvent? evt ) => AddTouches( touches, TouchData.ActionEnum.Up );

		void AddTouches( NSSet touches, TouchData.ActionEnum action )
		{
			var size = Bounds.Size;
			lock( Engine.inputEventQueue )
			{
				foreach( UITouch touch in touches )
				{
					var location = touch.LocationInView( this );
					var item = new Engine.TouchEventItem();
					item.Action = action;
					item.TouchHandle = touch.Handle;
					item.Position = new Vector2F( (float)( location.X / size.Width ), (float)( location.Y / size.Height ) );
					Engine.inputEventQueue.Enqueue( item );
				}
			}
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			var scale = ContentScaleFactor;
			var size = new Vector2I( (int)( Bounds.Width * scale ), (int)( Bounds.Height * scale ) );
			if( size == PlatformFunctionalityIOS.screenSize )
				return;
			PlatformFunctionalityIOS.screenSize = size;

			if( Engine.engineInitialized )
				RenderingSystem.ApplicationRenderTarget?.WindowMovedOrResized( size );
		}
	}
}
