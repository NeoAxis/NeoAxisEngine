using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Accessibility
{
	[ComImport]
	public interface IAccIdentity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetIdentityString([In] uint dwIDChild, [Out] IntPtr ppIDString, out uint pdwIDStringLen);
	}
}
