// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Generates a persistent, cross-platform hardware fingerprint without third-party or WMI dependencies.
	/// Utilizes native WinAPI (Firmware tables, Volume serial) and Registry on Windows,
	/// systemd machine-id on Linux, and IOKit on macOS.
	/// </summary>
	public static class HardwareFingerprint
	{
		static string cachedFingerprint;

		/// <summary>
		/// Retrieves the SHA-256 hardware identifier hash. Caches the result in memory.
		/// </summary>
		public static string GetUniqueHardwareId()
		{
			if( string.IsNullOrEmpty( cachedFingerprint ) )
			{
				string rawIdentifier;

				if( RuntimeInformation.IsOSPlatform( OSPlatform.Windows ) )
					rawIdentifier = GetWindowsHardwareProfile();
				else if( RuntimeInformation.IsOSPlatform( OSPlatform.Linux ) )
					rawIdentifier = GetLinuxIdentifier();
				else if( RuntimeInformation.IsOSPlatform( OSPlatform.OSX ) )
					rawIdentifier = GetMacOsIdentifier();
				else
				{
					// Fallback for unsupported operating systems
					rawIdentifier = "UNKNOWN:" + Environment.MachineName;
				}

				using var sha256 = SHA256.Create();
				byte[] hashBytes = sha256.ComputeHash( Encoding.UTF8.GetBytes( rawIdentifier ) );

				cachedFingerprint = "V1_" + Convert.ToHexString( hashBytes );
			}

			return cachedFingerprint;
		}

		///////////////////////////////////////////////
		// Windows Implementation

		[SupportedOSPlatform( "windows" )]
		private static string GetWindowsHardwareProfile()
		{
			var builder = new StringBuilder();

			builder.Append( "SMBIOS:" ).Append( GetSmbiosUuid() ).Append( ';' );
			builder.Append( "VOL:" ).Append( GetSystemVolumeSerial() ).Append( ';' );
			builder.Append( "CPU:" ).Append( Environment.ProcessorCount ).Append( '-' ).Append( RuntimeInformation.ProcessArchitecture ).Append( ';' );
			builder.Append( "REG:" ).Append( GetWindowsMachineGuid() ).Append( ';' );

			return builder.ToString();
		}

		/// <summary>
		/// Reads the hardware UUID directly from SMBIOS tables via WinAPI without WMI.
		/// </summary>
		[SupportedOSPlatform( "windows" )]
		private static string GetSmbiosUuid()
		{
			try
			{
				// 'RSMB' provider signature in ASCII integer representation
				const uint rsmbSignature = 0x52534D42;

				// Query the required buffer size
				uint bufferSize = GetSystemFirmwareTable( rsmbSignature, 0, IntPtr.Zero, 0 );
				if( bufferSize == 0 )
					return "NO_SMBIOS";

				byte[] buffer = new byte[ bufferSize ];
				unsafe
				{
					fixed( byte* pBuffer = buffer )
					{
						uint bytesWritten = GetSystemFirmwareTable( rsmbSignature, 0, (IntPtr)pBuffer, bufferSize );
						if( bytesWritten == 0 )
							return "NO_SMBIOS";
					}
				}

				// Parse SMBIOS table for Type 1 (System Information) structure containing the system UUID
				// Header offset in RawSMBIOSData is 8 bytes
				int cursor = 8;
				while( cursor < buffer.Length - 4 )
				{
					byte tableType = buffer[ cursor ];
					byte length = buffer[ cursor + 1 ];

					if( length < 4 || cursor + length > buffer.Length )
						break;

					// Type 1: System Information structure
					if( tableType == 1 && length >= 0x19 )
					{
						// UUID offset is 0x08 inside Type 1 table, length 16 bytes
						byte[] guidBytes = new byte[ 16 ];
						Buffer.BlockCopy( buffer, cursor + 8, guidBytes, 0, 16 );
						return new Guid( guidBytes ).ToString();
					}

					// Move to the unformatted string section following the formatted table
					cursor += length;
					while( cursor < buffer.Length - 1 && !( buffer[ cursor ] == 0 && buffer[ cursor + 1 ] == 0 ) )
						cursor++;
					cursor += 2; // Skip trailing double-null terminator
				}
			}
			catch
			{
				// Access violation or unexpected firmware structure fallback
			}

			return "UNKNOWN_SMBIOS";
		}

		/// <summary>
		/// Reads the filesystem serial number of the Windows system drive.
		/// </summary>
		[SupportedOSPlatform( "windows" )]
		private static string GetSystemVolumeSerial()
		{
			try
			{
				string systemRoot = Path.GetPathRoot( Environment.SystemDirectory ) ?? "C:\\";
				if( !systemRoot.EndsWith( "\\" ) )
					systemRoot += "\\";

				if( GetVolumeInformationW( systemRoot, null, 0, out uint serialNumber, out _, out _, null, 0 ) )
					return serialNumber.ToString( "X8" );
			}
			catch
			{
				// Native call failure fallback
			}

			return "UNKNOWN_VOL";
		}

		[SupportedOSPlatform( "windows" )]
		private static string GetWindowsMachineGuid()
		{
			try
			{
				using var baseKey = RegistryKey.OpenBaseKey( RegistryHive.LocalMachine, RegistryView.Registry64 );
				using var subKey = baseKey.OpenSubKey( @"SOFTWARE\Microsoft\Cryptography" );
				return subKey?.GetValue( "MachineGuid" )?.ToString() ?? "UNKNOWN_GUID";
			}
			catch
			{
				return "UNKNOWN_GUID";
			}
		}

		[DllImport( "kernel32.dll", SetLastError = true )]
		private static extern uint GetSystemFirmwareTable( uint firmwareTableProviderSignature, uint firmwareTableId, IntPtr pFirmwareTableBuffer, uint bufferSize );

		[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
		private static extern bool GetVolumeInformationW( string lpRootPathName, StringBuilder lpVolumeNameBuffer, uint nVolumeNameSize, out uint lpVolumeSerialNumber, out uint lpMaximumComponentLength, out uint lpFileSystemFlags, StringBuilder lpFileSystemNameBuffer, uint nFileSystemNameSize );


		///////////////////////////////////////////////
		// Linux Implementation

		private static string GetLinuxIdentifier()
		{
			// /etc/machine-id is generated by systemd on system installation and readable without root
			string[] candidatePaths = { "/etc/machine-id", "/var/lib/dbus/machine-id" };

			foreach( var path in candidatePaths )
			{
				try
				{
					if( File.Exists( path ) )
					{
						string id = File.ReadAllText( path ).Trim();
						if( !string.IsNullOrEmpty( id ) )
							return "LNX:" + id;
					}
				}
				catch
				{
					// Proceed to the next candidate path if access is restricted
				}
			}

			return "LNX:" + Environment.MachineName;
		}


		///////////////////////////////////////////////
		// macOS Implementation

		private static string GetMacOsIdentifier()
		{
			try
			{
				// Queries the hardware UUID via the IOKit registry tool without requiring elevated rights
				var psi = new ProcessStartInfo
				{
					FileName = "ioreg",
					Arguments = "-rd1 -c IOPlatformExpertDevice",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				using var process = Process.Start( psi );
				if( process != null )
				{
					string output = process.StandardOutput.ReadToEnd();
					process.WaitForExit( 1000 );

					const string searchKey = "\"IOPlatformUUID\" = \"";
					int index = output.IndexOf( searchKey, StringComparison.OrdinalIgnoreCase );
					if( index != -1 )
					{
						int start = index + searchKey.Length;
						int end = output.IndexOf( '"', start );
						if( end != -1 )
							return "MAC:" + output.Substring( start, end - start );
					}
				}
			}
			catch
			{
				// Tool execution failure fallback
			}

			return "MAC:" + Environment.MachineName;
		}
	}
}
