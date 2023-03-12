// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace NeoAxis
{
	static class DebugUtil
	{
		public const int
			FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,
			FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,
			FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,
			FORMAT_MESSAGE_DEFAULT = FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_SYSTEM;

		[DllImport( "kernel32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode )]
		public static extern int GetUserDefaultLCID();
		[DllImport( "kernel32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode )]
		public static extern int FormatMessage( int dwFlags, HandleRef lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, HandleRef arguments );

		/// <devdoc>
		///   Duplicated here from ClientUtils not to depend on that code because this class is to be
		///   compiled into System.Drawing and System.Windows.Forms.
		/// </devdoc>
		private static bool IsCriticalException( Exception ex )
		{
			return
				//ex is NullReferenceException ||
				ex is StackOverflowException ||
				ex is OutOfMemoryException ||
				ex is System.Threading.ThreadAbortException;
		}

		//
		// WARNING: Your PInvoke function needs to have the DllImport.SetLastError=true for this method
		// to work properly.  From the MSDN:
		// GetLastWin32Error exposes the Win32 GetLastError API method from Kernel32.DLL. This method exists 
		// because it is not safe to make a direct platform invoke call to GetLastError to obtain this information. 
		// If you want to access this error code, you must call GetLastWin32Error rather than writing your own 
		// platform invoke definition for GetLastError and calling it. The common language runtime can make 
		// internal calls to APIs that overwrite the operating system maintained GetLastError.
		//
		// You can only use this method to obtain error codes if you apply the System.Runtime.InteropServices.DllImportAttribute
		// to the method signature and set the SetLastError field to true.
		//              
		//[SuppressMessage("Microsoft.Interoperability", "CA1404:CallGetLastErrorImmediatelyAfterPInvoke")]
		public static string GetLastErrorStr()
		{
			int MAX_SIZE = 255;
			StringBuilder buffer = new StringBuilder( MAX_SIZE );
			string message = String.Empty;
			int err = 0;

			try
			{
				err = Marshal.GetLastWin32Error();

				int retVal = FormatMessage(
					FORMAT_MESSAGE_DEFAULT,
					new HandleRef( null, IntPtr.Zero ),
					err,
					GetUserDefaultLCID(),
					buffer,
					MAX_SIZE,
					new HandleRef( null, IntPtr.Zero ) );

				message = retVal != 0 ? buffer.ToString() : "<error returned>";
			}
			catch( Exception ex )
			{
				if( IsCriticalException( ex ) )
				{
					throw;  //rethrow critical exception.
				}
				message = ex.ToString();
			}

			return String.Format( System.Globalization.CultureInfo.CurrentCulture, "0x{0:x8} - {1}", err, message );
		}
	}
}
