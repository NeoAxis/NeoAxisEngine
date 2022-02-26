// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace NeoAxis
{
	// The entry point for the app.
	public class AppViewSourceUWP : IFrameworkViewSource
	{
		Action initCallback;
		Action runCallback;
		Action exitCallback;

		public AppViewSourceUWP( Action initCallback, Action runCallback, Action exitCallback )
		{
			this.initCallback = initCallback;
			this.runCallback = runCallback;
			this.exitCallback = exitCallback;
		}

		public IFrameworkView CreateView()
		{
			return new AppViewUWP( initCallback, runCallback, exitCallback );
		}
	}

	/// <summary>
	/// The IFrameworkView connects the app with Windows and handles application lifecycle events.
	/// </summary>
	public class AppViewUWP : IFrameworkView
	{
		Action initCallback;
		Action runCallback;
		Action exitCallback;

		public AppViewUWP( Action initCallback, Action runCallback, Action exitCallback )
		{
			this.initCallback = initCallback;
			this.runCallback = runCallback;
			this.exitCallback = exitCallback;
		}

		/// <summary>
		/// The first method called when the IFrameworkView is being created.
		/// Use this method to subscribe for Windows shell events and to initialize your app.
		/// </summary>
		public void Initialize( CoreApplicationView applicationView )
		{
			applicationView.Activated += this.OnViewActivated;

			// Register event handlers for app lifecycle.
			CoreApplication.Suspending += CoreApplication_OnSuspending;
			CoreApplication.Resuming += CoreApplication_OnResuming;
			CoreApplication.Exiting += CoreApplication_Exiting;
		}

		/// <summary>
		/// Called when the CoreWindow object is created (or re-created).
		/// </summary>
		public void SetWindow( CoreWindow window )
		{
		}

		/// <summary>
		/// The Load method can be used to initialize scene resources or to load a
		/// previously saved app state.
		/// </summary>
		public void Load( string entryPoint )
		{
			initCallback?.Invoke();
		}

		/// <summary>
		/// This method is called after the window becomes active. It oversees the
		/// update, draw, and present loop, and also oversees window message processing.
		/// </summary>
		public void Run()
		{
			try
			{
				runCallback?.Invoke();
			}
			finally
			{
				exitCallback?.Invoke();
			}
		}

		/// <summary>
		/// Terminate events do not cause Uninitialize to be called. It will be called if your IFrameworkView
		/// class is torn down while the app is in the foreground.
		/// </summary>
		public void Uninitialize()
		{
		}

		/// <summary>
		/// Called when the app is prelaunched. Use this method to load resources ahead of time
		/// and enable faster launch times.
		/// </summary>
		public void OnLaunched( LaunchActivatedEventArgs args )
		{
			if( args.PrelaunchActivated )
			{
				// TODO: Insert code to preload resources here.
			}
		}

		/// <summary>
		/// Called when the app view is activated. Activates the app's CoreWindow.
		/// </summary>
		private void OnViewActivated( CoreApplicationView sender, IActivatedEventArgs args )
		{
			// command line test
			// https://docs.microsoft.com/en-us/uwp/api/windows.applicationmodel.activation.commandlineactivatedeventargs
			switch( args.Kind )
			{
				case ActivationKind.Launch:
					var launchArgs = args as LaunchActivatedEventArgs;
					var activationArgString = launchArgs.Arguments;
					break;
				// A new ActivationKind for console activation of a windowed app.
				case ActivationKind.CommandLineLaunch:
					CommandLineActivatedEventArgs cmdLineArgs = args as CommandLineActivatedEventArgs;
					CommandLineActivationOperation operation = cmdLineArgs.Operation;
					var cmdLineString = operation.Arguments;
					var activationPath = operation.CurrentDirectoryPath;
					break;
			}
			//

			// Run() won't start until the CoreWindow is activated.
			sender.CoreWindow.Activate();
		}

		private void CoreApplication_OnSuspending( object sender, SuspendingEventArgs args )
		{
			// Save app state asynchronously after requesting a deferral. Holding a deferral
			// indicates that the application is busy performing suspending operations. Be
			// aware that a deferral may not be held indefinitely; after about five seconds,
			// the app will be forced to exit.
			var deferral = args.SuspendingOperation.GetDeferral();

			Task.Run( () =>
			{
				// TODO: Insert code here to save your app state.
				// EngineApp.Suspend();

				deferral.Complete();
			} );
		}

		private void CoreApplication_OnResuming( object sender, object args )
		{
			// Restore any data or state that was unloaded on suspend. By default, data
			// and state are persisted when resuming from suspend. Note that this event
			// does not occur if the app was previously terminated.

			// TODO: Insert code here to load your app state.
			// EngineApp.Resume();
		}

		private void CoreApplication_Exiting( object sender, object e )
		{
		}
	}
}
