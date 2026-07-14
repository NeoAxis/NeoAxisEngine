using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	// experimental activation.
	/*
	public enum ActivateOptionsEnum
	{
		None = 0,
		DesignMode = 0x1,
		NoErrorUI = 0x2,
		NoSplashScreen = 0x4,
	}

	[ComImport]
	[Guid( "2e941141-7f97-4756-ba1d-9decde894a3d" )]
	[InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
	public interface IApplicationActivationManager
	{
		IntPtr ActivateApplication( [In] String appUserModelId, [In] String arguments, [In] ActivateOptionsEnum options, [Out] out UInt32 processId );

		IntPtr ActivateForFile( [In] String appUserModelId, [In] IntPtr itemArray, [In] String verb, [Out] out UInt32 processId );

		IntPtr ActivateForProtocol( [In] String appUserModelId, [In] IntPtr itemArray, [Out] out UInt32 processId );
	}

	public enum AppxPackageArchitecture
	{
		x86 = 0,
		Arm = 5,
		x64 = 9,
		Neutral = 11,
		Arm64 = 12
	}

	[ComImport]
	[Guid( "45BA127D-10A8-46EA-8AB7-56EA9078943C" )]
	public class ApplicationActivationManager : IApplicationActivationManager
	{
		[MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
		public extern IntPtr ActivateApplication( [In] String appUserModelId, [In] String arguments, [In] ActivateOptionsEnum options, [Out] out UInt32 processId );

		[MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
		public extern IntPtr ActivateForFile( [In] String appUserModelId, [In] IntPtr itemArray, [In] String verb, [Out] out UInt32 processId );

		[MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
		public extern IntPtr ActivateForProtocol( [In] String appUserModelId, [In] IntPtr itemArray, [Out] out UInt32 processId );
	}
	*/
}
