using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Accessibility
{
	[ComImport]
	public interface IAccPropServer
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetPropValue([In] ref byte pIDString, [In] uint dwIDStringLen, [In] Guid idProp, [MarshalAs(UnmanagedType.Struct)] out object pvarValue, out int pfHasProp);
	}
}
