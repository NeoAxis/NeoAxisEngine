using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Accessibility
{
	[ComImport]
	public interface IAccPropServices
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		void SetPropValue([In] ref byte pIDString, [In] uint dwIDStringLen, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.Struct)] object var);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void SetPropServer([In] ref byte pIDString, [In] uint dwIDStringLen, [In] ref Guid paProps, [In] int cProps, [In] [MarshalAs(UnmanagedType.Interface)] IAccPropServer pServer, [In] AnnoScope AnnoScope);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ClearProps([In] ref byte pIDString, [In] uint dwIDStringLen, [In] ref Guid paProps, [In] int cProps);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void SetHwndProp([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.Struct)] object var);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void SetHwndPropStr([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.LPWStr)] string str);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void SetHwndPropServer([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [In] ref Guid paProps, [In] int cProps, [In] [MarshalAs(UnmanagedType.Interface)] IAccPropServer pServer, [In] AnnoScope AnnoScope);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ClearHwndProps([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [In] ref Guid paProps, [In] int cProps);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ComposeHwndIdentityString([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [Out] IntPtr ppIDString, out uint pdwIDStringLen);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void DecomposeHwndIdentityString([In] ref byte pIDString, [In] uint dwIDStringLen, [Out] [ComAliasName("Accessibility.wireHWND")] IntPtr phwnd, out uint pidObject, out uint pidChild);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void SetHmenuProp([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.Struct)] object var);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void SetHmenuPropStr([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.LPWStr)] string str);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void SetHmenuPropServer([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [In] ref Guid paProps, [In] int cProps, [In] [MarshalAs(UnmanagedType.Interface)] IAccPropServer pServer, [In] AnnoScope AnnoScope);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ClearHmenuProps([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [In] ref Guid paProps, [In] int cProps);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ComposeHmenuIdentityString([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [Out] IntPtr ppIDString, out uint pdwIDStringLen);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void DecomposeHmenuIdentityString([In] ref byte pIDString, [In] uint dwIDStringLen, [Out] [ComAliasName("Accessibility.wireHMENU")] IntPtr phmenu, out uint pidChild);
	}
}
