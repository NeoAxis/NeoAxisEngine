using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Accessibility
{
	[ComImport]
	public class CAccPropServicesClass : IAccPropServices, CAccPropServices
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern CAccPropServicesClass();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void SetPropValue([In] ref byte pIDString, [In] uint dwIDStringLen, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.Struct)] object var);

		void IAccPropServices.SetPropValue([In] ref byte pIDString, [In] uint dwIDStringLen, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.Struct)] object var)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetPropValue
			this.SetPropValue(ref pIDString, dwIDStringLen, idProp, var);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void SetPropServer([In] ref byte pIDString, [In] uint dwIDStringLen, [In] ref Guid paProps, [In] int cProps, [In] [MarshalAs(UnmanagedType.Interface)] IAccPropServer pServer, [In] AnnoScope AnnoScope);

		void IAccPropServices.SetPropServer([In] ref byte pIDString, [In] uint dwIDStringLen, [In] ref Guid paProps, [In] int cProps, [In] [MarshalAs(UnmanagedType.Interface)] IAccPropServer pServer, [In] AnnoScope AnnoScope)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetPropServer
			this.SetPropServer(ref pIDString, dwIDStringLen, ref paProps, cProps, pServer, AnnoScope);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void ClearProps([In] ref byte pIDString, [In] uint dwIDStringLen, [In] ref Guid paProps, [In] int cProps);

		void IAccPropServices.ClearProps([In] ref byte pIDString, [In] uint dwIDStringLen, [In] ref Guid paProps, [In] int cProps)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ClearProps
			this.ClearProps(ref pIDString, dwIDStringLen, ref paProps, cProps);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void SetHwndProp([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.Struct)] object var);

		void IAccPropServices.SetHwndProp([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.Struct)] object var)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetHwndProp
			this.SetHwndProp(ref hwnd, idObject, idChild, idProp, var);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void SetHwndPropStr([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.LPWStr)] string str);

		void IAccPropServices.SetHwndPropStr([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.LPWStr)] string str)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetHwndPropStr
			this.SetHwndPropStr(ref hwnd, idObject, idChild, idProp, str);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void SetHwndPropServer([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [In] ref Guid paProps, [In] int cProps, [In] [MarshalAs(UnmanagedType.Interface)] IAccPropServer pServer, [In] AnnoScope AnnoScope);

		void IAccPropServices.SetHwndPropServer([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [In] ref Guid paProps, [In] int cProps, [In] [MarshalAs(UnmanagedType.Interface)] IAccPropServer pServer, [In] AnnoScope AnnoScope)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetHwndPropServer
			this.SetHwndPropServer(ref hwnd, idObject, idChild, ref paProps, cProps, pServer, AnnoScope);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void ClearHwndProps([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [In] ref Guid paProps, [In] int cProps);

		void IAccPropServices.ClearHwndProps([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [In] ref Guid paProps, [In] int cProps)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ClearHwndProps
			this.ClearHwndProps(ref hwnd, idObject, idChild, ref paProps, cProps);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void ComposeHwndIdentityString([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [Out] IntPtr ppIDString, out uint pdwIDStringLen);

		void IAccPropServices.ComposeHwndIdentityString([In] [ComAliasName("Accessibility.wireHWND")] ref _RemotableHandle hwnd, [In] uint idObject, [In] uint idChild, [Out] IntPtr ppIDString, out uint pdwIDStringLen)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ComposeHwndIdentityString
			this.ComposeHwndIdentityString(ref hwnd, idObject, idChild, ppIDString, out pdwIDStringLen);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void DecomposeHwndIdentityString([In] ref byte pIDString, [In] uint dwIDStringLen, [Out] [ComAliasName("Accessibility.wireHWND")] IntPtr phwnd, out uint pidObject, out uint pidChild);

		void IAccPropServices.DecomposeHwndIdentityString([In] ref byte pIDString, [In] uint dwIDStringLen, [Out] [ComAliasName("Accessibility.wireHWND")] IntPtr phwnd, out uint pidObject, out uint pidChild)
		{
			//ILSpy generated this explicit interface implementation from .override directive in DecomposeHwndIdentityString
			this.DecomposeHwndIdentityString(ref pIDString, dwIDStringLen, phwnd, out pidObject, out pidChild);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void SetHmenuProp([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.Struct)] object var);

		void IAccPropServices.SetHmenuProp([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.Struct)] object var)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetHmenuProp
			this.SetHmenuProp(ref hmenu, idChild, idProp, var);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void SetHmenuPropStr([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.LPWStr)] string str);

		void IAccPropServices.SetHmenuPropStr([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [In] Guid idProp, [In] [MarshalAs(UnmanagedType.LPWStr)] string str)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetHmenuPropStr
			this.SetHmenuPropStr(ref hmenu, idChild, idProp, str);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void SetHmenuPropServer([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [In] ref Guid paProps, [In] int cProps, [In] [MarshalAs(UnmanagedType.Interface)] IAccPropServer pServer, [In] AnnoScope AnnoScope);

		void IAccPropServices.SetHmenuPropServer([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [In] ref Guid paProps, [In] int cProps, [In] [MarshalAs(UnmanagedType.Interface)] IAccPropServer pServer, [In] AnnoScope AnnoScope)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetHmenuPropServer
			this.SetHmenuPropServer(ref hmenu, idChild, ref paProps, cProps, pServer, AnnoScope);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void ClearHmenuProps([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [In] ref Guid paProps, [In] int cProps);

		void IAccPropServices.ClearHmenuProps([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [In] ref Guid paProps, [In] int cProps)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ClearHmenuProps
			this.ClearHmenuProps(ref hmenu, idChild, ref paProps, cProps);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void ComposeHmenuIdentityString([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [Out] IntPtr ppIDString, out uint pdwIDStringLen);

		void IAccPropServices.ComposeHmenuIdentityString([In] [ComAliasName("Accessibility.wireHMENU")] ref _RemotableHandle hmenu, [In] uint idChild, [Out] IntPtr ppIDString, out uint pdwIDStringLen)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ComposeHmenuIdentityString
			this.ComposeHmenuIdentityString(ref hmenu, idChild, ppIDString, out pdwIDStringLen);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void DecomposeHmenuIdentityString([In] ref byte pIDString, [In] uint dwIDStringLen, [Out] [ComAliasName("Accessibility.wireHMENU")] IntPtr phmenu, out uint pidChild);

		void IAccPropServices.DecomposeHmenuIdentityString([In] ref byte pIDString, [In] uint dwIDStringLen, [Out] [ComAliasName("Accessibility.wireHMENU")] IntPtr phmenu, out uint pidChild)
		{
			//ILSpy generated this explicit interface implementation from .override directive in DecomposeHmenuIdentityString
			this.DecomposeHmenuIdentityString(ref pIDString, dwIDStringLen, phmenu, out pidChild);
		}
	}
}
