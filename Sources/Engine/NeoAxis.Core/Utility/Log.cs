// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Internal;

namespace NeoAxis
{
	/// <summary>
	/// Class for working with application logs.
	/// </summary>
	public static class Log
	{
		static string dumpRealFileName;
		static bool logDirectoryCreated;
		static Thread mainThread;
		static object lockObject = new object();

		static List<CachedItem> cachedList = new List<CachedItem>();

		static bool fatalActivated;

		///////////////////////////////////////////

		/// <summary>
		/// Provides access to event handlers of <see cref="Log"/>.
		/// </summary>
		public static class Handlers
		{
			public delegate void LogDelegate( string text, ref bool dumpToLogFile );
			public delegate void LogHandledDelegate( string text, ref bool handled, ref bool dumpToLogFile );
			public delegate void LogFatalHandledDelegate( string text, string createdLogFilePath, ref bool handled );

			public static event LogDelegate InvisibleInfoHandler;
			public static event LogDelegate InfoHandler;
			public static event LogHandledDelegate WarningHandler;
			public static event LogHandledDelegate ErrorHandler;
			public static event LogFatalHandledDelegate FatalHandler;

			internal static void InvisibleInfo( string text, ref bool dumpToLogFile )
			{
				if( InvisibleInfoHandler != null )
					InvisibleInfoHandler( text, ref dumpToLogFile );
			}

			internal static void Info( string text, ref bool dumpToLogFile )
			{
				if( InfoHandler != null )
					InfoHandler( text, ref dumpToLogFile );
			}

			internal static void Warning( string text, ref bool handled, ref bool dumpToLogFile )
			{
				if( WarningHandler != null )
					WarningHandler( text, ref handled, ref dumpToLogFile );
			}

			internal static void Error( string text, ref bool handled, ref bool dumpToLogFile )
			{
				if( ErrorHandler != null )
					ErrorHandler( text, ref handled, ref dumpToLogFile );
			}

			internal static void Fatal( string text, string createdLogFilePath, ref bool handled )
			{
				if( FatalHandler != null )
					FatalHandler( text, createdLogFilePath, ref handled );
			}
		}

		///////////////////////////////////////////

		enum LogType
		{
			InvisibleInfo,
			Info,
			Warning,
			Error,
		}

		///////////////////////////////////////////

		struct CachedItem
		{
			public LogType type;
			public string text;

			public CachedItem( LogType type, string text )
			{
				this.type = type;
				this.text = text;
			}
		}

		///////////////////////////////////////////

		internal static void Init( Thread mainThread, string dumpRealFileName )
		{
			Log.mainThread = mainThread;
			Log.dumpRealFileName = dumpRealFileName;

			if( !string.IsNullOrEmpty( dumpRealFileName ) )
			{
				if( File.Exists( dumpRealFileName ) )
					File.Delete( dumpRealFileName );
			}

			Log.DumpToFile( "Log started: " + DateTime.Now.ToString() + "\r\n" );
		}

		/// <summary>
		/// Logging to the file.
		/// </summary>
		/// <param name="message"></param>
		public static void InvisibleInfo( object message )
		{
			var text = message != null ? message.ToString() : "Null";

			lock( lockObject )
			{
				Thread thread = Thread.CurrentThread;
				if( mainThread == null || thread == mainThread )
				{
					bool dumpToLogFile = true;
					Handlers.InvisibleInfo( text, ref dumpToLogFile );
					if( dumpToLogFile )
						DumpToFile( "Info: " + text + "\r\n" );
				}
				else
					cachedList.Add( new CachedItem( LogType.InvisibleInfo, text ) );
			}

			//lock( lockObject )
			//{
			//   DumpToFile( "Info: " + text + "\r\n" );

			//   Thread thread = Thread.CurrentThread;
			//   if( mainThread == null || thread == mainThread )
			//      Handlers.InvisibleInfo( text );
			//   else
			//      cachedList.Add( new CachedItem( LogType.InvisibleInfo, text ) );
			//}
		}

		public static void Info( object message )
		{
			var text = message != null ? message.ToString() : "Null";

			lock( lockObject )
			{
				Thread thread = Thread.CurrentThread;
				if( mainThread == null || thread == mainThread )
				{
					bool dumpToLogFile = true;
					Handlers.Info( text, ref dumpToLogFile );
					if( dumpToLogFile )
						DumpToFile( "Info: " + text + "\r\n" );
				}
				else
					cachedList.Add( new CachedItem( LogType.Info, text ) );
			}

			//lock( lockObject )
			//{
			//   DumpToFile( "Info: " + text + "\r\n" );

			//   Thread thread = Thread.CurrentThread;
			//   if( mainThread == null || thread == mainThread )
			//      Handlers.Info( text );
			//   else
			//      cachedList.Add( new CachedItem( LogType.Info, text ) );
			//}
		}

		public static void Info( string message )
		{
			Info( (object)message );
		}

		public static void Warning( object message )
		{
			var text = message != null ? message.ToString() : "Null";

			lock( lockObject )
			{
				Thread thread = Thread.CurrentThread;
				if( mainThread == null || thread == mainThread )
				{
					bool dumpToLogFile = true;
					HandlersWarning( text, ref dumpToLogFile );
					if( dumpToLogFile )
						DumpToFile( "Warning: " + text + "\r\n" );
				}
				else
					cachedList.Add( new CachedItem( LogType.Warning, text ) );
			}

			//lock( lockObject )
			//{
			//   DumpToFile( "Warning: " + text + "\r\n" );

			//   Thread thread = Thread.CurrentThread;
			//   if( mainThread == null || thread == mainThread )
			//      HandlersWarning( text );
			//   else
			//      cachedList.Add( new CachedItem( LogType.Warning, text ) );
			//}
		}

		public static void Warning( string message )
		{
			Warning( (object)message );
		}

		static void HandlersWarning( string text, ref bool dumpToLogFile )
		{
			bool handled = false;
			Handlers.Warning( text, ref handled, ref dumpToLogFile );
			if( !handled )
				LogPlatformFunctionality.Instance.ShowMessageBox( text, "Warning" );
		}

		//static void HandlersWarning( string text )
		//{
		//   bool handled = false;
		//   Handlers.Warning( text, ref handled );
		//   if( !handled )
		//      PlatformFunctionality.Get().ShowMessageBox( text, "Warning" );
		//}

		public static void Error( object message )
		{
			var text = message != null ? message.ToString() : "Null";

			lock( lockObject )
			{
				Thread thread = Thread.CurrentThread;
				if( mainThread == null || thread == mainThread )
				{
					bool dumpToLogFile = true;
					HandlersError( text, ref dumpToLogFile );
					if( dumpToLogFile )
						DumpToFile( "Error: " + text + "\r\n" + GetStackTrace() );
				}
				else
					cachedList.Add( new CachedItem( LogType.Error, text ) );
			}
		}

		public static void Error( string message )
		{
			Error( (object)message );
		}

		static void HandlersError( string text, ref bool dumpToLogFile )
		{
			bool handled = false;
			Handlers.Error( text, ref handled, ref dumpToLogFile );
			if( !handled )
				LogPlatformFunctionality.Instance.ShowMessageBox( text, "Error" );
		}

		static string GetStackTrace()
		{
			try
			{
				string stackTrace = "";
				stackTrace = Environment.StackTrace;
				if( stackTrace == null )
					return "";
				return stackTrace;
			}
			catch { }
			return "";
		}

		public static void Fatal( object message )
		{
			var text = message != null ? message.ToString() : "Null";

			fatalActivated = true;

			lock( lockObject )
			{
				bool handled = false;

				DumpToFile( "Fatal: " + text + "\r\n" + GetStackTrace() );

				string copyLogFileName = FatalErrorCopyLogFile();
				if( copyLogFileName != null )
					text = "Fatal log file created: " + copyLogFileName + ".\t\n\t\n" + text;

				Handlers.Fatal( text, copyLogFileName, ref handled );

				if( !handled )
				{
					string messageBoxText = text + "\r\n\r\n\r\n" + GetStackTrace();
					LogPlatformFunctionality.Instance.ShowMessageBox( messageBoxText, "Fatal" );//: Exception" );

					if( AfterFatal != null )
						AfterFatal();

					Process process = Process.GetCurrentProcess();
					process.Kill();
				}
			}
		}

		public static void Fatal( string message )
		{
			Fatal( (object)message );
		}

		public delegate void AfterFatalDelegate();
		public static event AfterFatalDelegate AfterFatal;

		public static void FatalAsException( object message )
		{
			var text = message != null ? message.ToString() : "Null";

			lock( lockObject )
			{
				bool handled = false;

				DumpToFile( "Exception: " + text );

				string copyLogFileName = FatalErrorCopyLogFile();
				if( copyLogFileName != null )
					text = "Fatal log file created: " + copyLogFileName + ".\t\n\t\n" + text;

				Handlers.Fatal( text, copyLogFileName, ref handled );

				if( !handled )
				{
					//if( PlatformInfo.Platform == PlatformInfo.Platforms.Android )
					//{
					//   ( (AndroidPlatformFunctionality)PlatformFunctionality.Get() ).Fatal( text );
					//   return;
					//}
					//else
					//{
					LogPlatformFunctionality.Instance.ShowMessageBox( text, "Fatal: Exception" );

					AfterFatal?.Invoke();

					Process process = Process.GetCurrentProcess();
					process.Kill();
					//}
				}
			}
		}

		//InvisibleInfo

		public static void InvisibleInfo( string format, object arg0, object arg1, object arg2, object arg3, object arg4 )
		{
			InvisibleInfo( (object)string.Format( format, arg0, arg1, arg2, arg3, arg4 ) );
		}

		public static void InvisibleInfo( string format, object arg0, object arg1, object arg2, object arg3 )
		{
			InvisibleInfo( (object)string.Format( format, arg0, arg1, arg2, arg3 ) );
		}

		public static void InvisibleInfo( string format, object arg0, object arg1, object arg2 )
		{
			InvisibleInfo( (object)string.Format( format, arg0, arg1, arg2 ) );
		}

		public static void InvisibleInfo( string format, object arg0, object arg1 )
		{
			InvisibleInfo( (object)string.Format( format, arg0, arg1 ) );
		}

		public static void InvisibleInfo( string format, object arg0 )
		{
			InvisibleInfo( (object)string.Format( format, arg0 ) );
		}

		//public static void InvisibleInfo( string format, params object[] args )
		//{
		//	InvisibleInfo( (object)string.Format( format, args ) );
		//}

		//Info

		public static void Info( string format, object arg0, object arg1, object arg2, object arg3, object arg4 )
		{
			Info( (object)string.Format( format, arg0, arg1, arg2, arg3, arg4 ) );
		}

		public static void Info( string format, object arg0, object arg1, object arg2, object arg3 )
		{
			Info( (object)string.Format( format, arg0, arg1, arg2, arg3 ) );
		}

		public static void Info( string format, object arg0, object arg1, object arg2 )
		{
			Info( (object)string.Format( format, arg0, arg1, arg2 ) );
		}

		public static void Info( string format, object arg0, object arg1 )
		{
			Info( (object)string.Format( format, arg0, arg1 ) );
		}

		public static void Info( string format, object arg0 )
		{
			Info( (object)string.Format( format, arg0 ) );
		}

		//public static void Info( string format, params object[] args )
		//{
		//	Info( (object)string.Format( format, args ) );
		//}

		//Warning

		public static void Warning( string format, object arg0, object arg1, object arg2, object arg3, object arg4 )
		{
			Warning( (object)string.Format( format, arg0, arg1, arg2, arg3, arg4 ) );
		}

		public static void Warning( string format, object arg0, object arg1, object arg2,
			object arg3 )
		{
			Warning( (object)string.Format( format, arg0, arg1, arg2, arg3 ) );
		}

		public static void Warning( string format, object arg0, object arg1, object arg2 )
		{
			Warning( (object)string.Format( format, arg0, arg1, arg2 ) );
		}

		public static void Warning( string format, object arg0, object arg1 )
		{
			Warning( (object)string.Format( format, arg0, arg1 ) );
		}

		public static void Warning( string format, object arg0 )
		{
			Warning( (object)string.Format( format, arg0 ) );
		}

		//public static void Warning( string format, params object[] args )
		//{
		//	Warning( (object)string.Format( format, args ) );
		//}


		//Error

		//!!!!need?

		public static void Error( string format, object arg0, object arg1, object arg2, object arg3, object arg4 )
		{
			Error( (object)string.Format( format, arg0, arg1, arg2, arg3, arg4 ) );
		}

		public static void Error( string format, object arg0, object arg1, object arg2, object arg3 )
		{
			Error( (object)string.Format( format, arg0, arg1, arg2, arg3 ) );
		}

		public static void Error( string format, object arg0, object arg1, object arg2 )
		{
			Error( (object)string.Format( format, arg0, arg1, arg2 ) );
		}

		public static void Error( string format, object arg0, object arg1 )
		{
			Error( (object)string.Format( format, arg0, arg1 ) );
		}

		public static void Error( string format, object arg0 )
		{
			Error( (object)string.Format( format, arg0 ) );
		}

		//public static void Error( string format, params object[] args )
		//{
		//	Error( (object)string.Format( format, args ) );
		//}

		//Fatal

		public static void Fatal( string format, object arg0, object arg1, object arg2, object arg3, object arg4 )
		{
			Fatal( (object)string.Format( format, arg0, arg1, arg2, arg3, arg4 ) );
		}

		public static void Fatal( string format, object arg0, object arg1, object arg2, object arg3 )
		{
			Fatal( (object)string.Format( format, arg0, arg1, arg2, arg3 ) );
		}

		public static void Fatal( string format, object arg0, object arg1, object arg2 )
		{
			Fatal( (object)string.Format( format, arg0, arg1, arg2 ) );
		}

		public static void Fatal( string format, object arg0, object arg1 )
		{
			Fatal( (object)string.Format( format, arg0, arg1 ) );
		}

		public static void Fatal( string format, object arg0 )
		{
			Fatal( (object)string.Format( format, arg0 ) );
		}

		//public static void Fatal( string format, params object[] args )
		//{
		//	Fatal( (object)string.Format( format, args ) );
		//}

		public static string DumpRealFileName
		{
			get { return dumpRealFileName; }
		}

		public static void DumpToFile( string text )
		{
			if( string.IsNullOrEmpty( dumpRealFileName ) )
				return;

			if( !logDirectoryCreated )
			{
				logDirectoryCreated = true;
				try
				{
					string directoryName = Path.GetDirectoryName( dumpRealFileName );
					if( !string.IsNullOrEmpty( directoryName ) )
						Directory.CreateDirectory( directoryName );
				}
				catch { }
			}

			string text2 = string.Format( "[{0}] {1}",
				DateTime.Now.ToString( "HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo ),
				text );

			lock( lockObject )
			{
				try
				{
					using( StreamWriter writer = new StreamWriter( dumpRealFileName, true ) )
					{
						writer.Write( text2 );
					}
				}
				catch { }
			}
		}

		static string FatalErrorCopyLogFile()
		{
			if( string.IsNullOrEmpty( dumpRealFileName ) )
				return null;

			string time = DateTime.Now.ToString( "dd-MM-yyyy_HH-mm-ss" );

			string destFileName = Path.GetDirectoryName( Log.DumpRealFileName ) + "/FatalError_" +
				Path.GetFileNameWithoutExtension( Log.DumpRealFileName ) + "_" + time +
				Path.GetExtension( Log.DumpRealFileName );

			if( !File.Exists( destFileName ) )
			{
				try
				{
					File.Copy( Log.DumpRealFileName, destFileName );
				}
				catch
				{
					return null;
				}
			}
			else
				return null;

			return destFileName;
		}

		public static void FlushCachedLog()
		{
			lock( lockObject )
			{
				while( cachedList.Count != 0 )
				{
					CachedItem item = cachedList[ 0 ];
					cachedList.RemoveAt( 0 );

					switch( item.type )
					{
					case LogType.InvisibleInfo:
						{
							bool dumpToLogFile = true;
							Handlers.InvisibleInfo( item.text, ref dumpToLogFile );
							if( dumpToLogFile )
								DumpToFile( "Info: " + item.text + "\r\n" );
						}
						break;

					case LogType.Info:
						{
							bool dumpToLogFile = true;
							Handlers.Info( item.text, ref dumpToLogFile );
							if( dumpToLogFile )
								DumpToFile( "Info: " + item.text + "\r\n" );
						}
						break;

					case LogType.Warning:
						{
							bool dumpToLogFile = true;
							HandlersWarning( item.text, ref dumpToLogFile );
							if( dumpToLogFile )
								DumpToFile( "Warning: " + item.text + "\r\n" );
						}
						break;

					case LogType.Error:
						{
							bool dumpToLogFile = true;
							HandlersError( item.text, ref dumpToLogFile );
							if( dumpToLogFile )
								DumpToFile( "Error: " + item.text + "\r\n" + GetStackTrace() );
						}
						break;
					}
				}
			}
		}

		public static bool FatalActivated
		{
			get { return fatalActivated; }
		}

		public static Thread MainThread
		{
			get { return mainThread; }
		}
	}
}
