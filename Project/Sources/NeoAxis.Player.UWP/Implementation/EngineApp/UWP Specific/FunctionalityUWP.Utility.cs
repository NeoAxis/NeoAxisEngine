// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using Windows.UI.Popups;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	partial class PlatformFunctionalityUWP : PlatformFunctionality
	{
		///////////////////////////////////////////

		[DllImport( "psapi.dll", CharSet = CharSet.Unicode )]
		static unsafe extern bool EnumProcessModules( IntPtr hProcess, IntPtr* lphModule, uint cb, uint* lpcbNeeded );

		[DllImport( "psapi.dll", CharSet = CharSet.Unicode )]
		static extern uint GetModuleFileNameEx( IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, uint nSize );

		///////////////////////////////////////////

		Stopwatch stopwatch;

		public override double GetSystemTime()
		{
			if( stopwatch == null )
			{
				stopwatch = new Stopwatch();
				Debug.Assert( Stopwatch.IsHighResolution );
				stopwatch.Start();
			}

			return stopwatch.Elapsed.TotalSeconds;
		}

		public override string[] GetNativeModuleNames()
		{
			//copy from WindowsPlatformFunctionality

			string[] result;

			unsafe
			{
				try
				{
					uint needBytes;
					if( EnumProcessModules( Process.GetCurrentProcess().Handle,
						null, 0, &needBytes ) )
					{
						int count = (int)needBytes / (int)sizeof( IntPtr );
						IntPtr* array = (IntPtr*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Utility, (int)needBytes ).ToPointer();

						uint needBytes2;
						if( EnumProcessModules( Process.GetCurrentProcess().Handle,
							array, needBytes, &needBytes2 ) )
						{
							if( needBytes2 < needBytes )
								count = (int)needBytes2 / (int)sizeof( IntPtr );

							result = new string[ count ];

							StringBuilder stringBuilder = new StringBuilder( 2048 );

							for( int n = 0; n < count; n++ )
							{
								stringBuilder.Length = 0;
								GetModuleFileNameEx( Process.GetCurrentProcess().Handle,
									array[ n ], stringBuilder, (uint)stringBuilder.Capacity );

								result[ n ] = stringBuilder.ToString();
							}
						}
						else
							result = new string[ 0 ];

						NativeUtility.Free( (IntPtr)array );
					}
					else
						result = new string[ 0 ];
				}
				catch
				{
					result = new string[ 0 ];
				}
			}

			return result;


			//// it seems it works, but it's cross-platform?
			//var wp = new WindowsPlatformFunctionality();
			//return wp.GetNativeModuleNames();
		}

		public override bool ShowMessageBoxYesNoQuestion( string text, string caption )
		{
			MessageDialog dialog = new MessageDialog( text, caption );
			dialog.Options = MessageDialogOptions.None;

			dialog.Commands.Add( new UICommand( "Yes" ) );
			dialog.Commands.Add( new UICommand( "No" ) );

			dialog.DefaultCommandIndex = 0;
			dialog.CancelCommandIndex = (uint)dialog.Commands.Count - 1;

			var command = dialog.ShowAsync().GetResults();
			return command.Label == "Yes";
		}

		public override void GetSystemLanguage( out string name, out string englishName )
		{
			name = CultureInfo.CurrentUICulture.Name;
			englishName = CultureInfo.CurrentUICulture.EnglishName;
		}

		public override IntPtr CallSpecialPlatformSpecificMethod( string message, IntPtr param )
		{
			return IntPtr.Zero;
		}
	}
}
