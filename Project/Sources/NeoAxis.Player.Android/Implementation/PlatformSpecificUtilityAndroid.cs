// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Internal;

namespace NeoAxis
{
	class PlatformSpecificUtilityAndroid : PlatformSpecificUtility
	{
		public override string GetExecutableDirectoryPath()
		{
			//!!!!
			return "";

			//// Android apps don’t have an “executable directory” in the desktop sense.
			//// If you need a base path, typically use internal files dir:
			//return Application.Context.FilesDir?.AbsolutePath ?? string.Empty;
		}

		//public override IntPtr LoadLibrary( string path )
		//{
		//	return IntPtr.Zero;
		//}

		static ClipboardManager? GetClipboardManager()
		{
			var ctx = Application.Context;
			return (ClipboardManager?)ctx.GetSystemService( Context.ClipboardService );
		}

		static void RunOnMainThread( Action action )
		{
			var looper = Looper.MyLooper();
			if( Looper.MyLooper() == looper )
				action();
			else
			{
				if( looper != null )
					new Handler( looper ).Post( action );
			}
		}

		public override async Task<string> GetClipboardTextAsync()
		{
			try
			{
				var cm = GetClipboardManager();
				if( cm == null )
					return string.Empty;

				string result = string.Empty;

				RunOnMainThread( () =>
				{
					if( !cm.HasPrimaryClip )
						return;

					var clip = cm.PrimaryClip;
					if( clip == null || clip.ItemCount <= 0 )
						return;

					var item = clip.GetItemAt( 0 );
					if( item == null )
						return;

					// CoerceToText handles plain text and some other clip types.
					var coerced = item.CoerceToText( Application.Context );
					result = coerced?.ToString() ?? string.Empty;
				} );

				return result;
			}
			catch
			{
				return string.Empty;
			}

			//try
			//{
			//	return CrossClipboard.Current.GetText();
			//}
			//catch
			//{
			//	return "";
			//}
		}

		public override void SetClipboardText( string text )
		{
			try
			{
				var cm = GetClipboardManager();
				if( cm == null )
					return;

				RunOnMainThread( () =>
				{
					var clip = ClipData.NewPlainText( "text", text ?? string.Empty );
					cm.PrimaryClip = clip;
				} );
			}
			catch
			{
				// ignore
			}

			//try
			//{
			//	CrossClipboard.Current.SetText( text );
			//}
			//catch { }
		}

		public override EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons = EMessageBoxButtons.OK )
		{
			//!!!!buttons, result

			Android.Util.Log.WriteLine( Android.Util.LogPriority.Debug, "MyApp", "MESSAGE:\r\n" + caption + ":" + text );

			return EDialogResult.OK;
		}
	}
}