using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using System;

//Port from https://github.com/JakeWharton/ProcessPhoenix/blob/master/process-phoenix/src/main/java/com/jakewharton/processphoenix/ProcessPhoenix.java

namespace Com.JakeWharton.ProcessPhoenix
{
	[Activity( Process = ":phoenix", Theme = "@android:style/Theme.Translucent.NoTitleBar" )]
	public class ProcessPhoenix : Activity
	{
		private const string KEY_RESTART_INTENT = "phoenix_restart_intent";

		///**
		// * Call to restart the application process using the {@linkplain Intent#CATEGORY_DEFAULT default}
		// * activity as an intent.
		// * <p>
		// * Behavior of the current process after invoking this method is undefined.
		// */
		//public static void TriggerRebirth( Context context )
		//{
		//	TriggerRebirth( context, GetRestartIntent( context ) );
		//}

		/**
		 * Call to restart the application process using the specified intent.
		 * <p>
		 * Behavior of the current process after invoking this method is undefined.
		 */
		public static void TriggerRebirth( Context context, Intent nextIntent )
		{
			Intent intent = new Intent( context, typeof( ProcessPhoenix ) );
			intent.AddFlags( ActivityFlags.NewTask ); // In case we are called with non-Activity context.
			intent.PutExtra( KEY_RESTART_INTENT, nextIntent );
			context.StartActivity( intent );
			if( context is Activity )
			{
				( (Activity)context ).Finish();
			}
			Java.Lang.Runtime.GetRuntime().Exit( 0 ); // Kill kill kill!
		}

		//private static Intent GetRestartIntent( Context context )
		//{
		//	Intent defaultIntent = new Intent( Intent.ActionMain, null );
		//	defaultIntent.AddFlags( ActivityFlags.NewTask | ActivityFlags.ClearTask );
		//	defaultIntent.AddCategory( Intent.CategoryDefault );

		//	string packageName = context.PackageName;
		//	PackageManager packageManager = context.PackageManager;
		//	foreach( ResolveInfo resolveInfo in packageManager.QueryIntentActivities( defaultIntent, 0 ) )
		//	{
		//		ActivityInfo activityInfo = resolveInfo.ActivityInfo;
		//		if( activityInfo.PackageName.Equals( packageName ) )
		//		{
		//			defaultIntent.SetComponent( new ComponentName( packageName, activityInfo.Name ) );
		//			return defaultIntent;
		//		}
		//	}

		//	throw new Java.Lang.IllegalStateException( "Unable to determine default activity for "
		//		+ packageName
		//		+ ". Does an activity specify the DEFAULT category in its intent filter?" );
		//}

		protected override void OnCreate( Bundle savedInstanceState )
		{
			base.OnCreate( savedInstanceState );

			Intent intent = (Intent)Intent.GetParcelableExtra( KEY_RESTART_INTENT );
			StartActivity( intent );
			Finish();
			Java.Lang.Runtime.GetRuntime().Exit( 0 ); // Kill kill kill!
		}

		/**
		 * Checks if the current process is a temporary Phoenix Process.
		 * This can be used to avoid initialisation of unused resources or to prevent running code that
		 * is not multi-process ready.
		 *
		 * @return true if the current process is a temporary Phoenix Process
		 */
		public static bool IsPhoenixProcess( Context context )
		{
			int currentPid = Process.MyPid();
			ActivityManager manager = (ActivityManager)context.GetSystemService( Context.ActivityService );
			foreach( ActivityManager.RunningAppProcessInfo processInfo in manager.RunningAppProcesses )
			{
				if( processInfo.Pid == currentPid && processInfo.ProcessName.EndsWith( ":phoenix" ) )
				{
					return true;
				}
			}
			return false;
		}
	}
}

