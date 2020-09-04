//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;

//namespace Accessibility
//{
//	[ComImport]
//	public interface IAccessible
//	{
//		object accParent
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[return: MarshalAs(UnmanagedType.IDispatch)]
//			get;
//		}

//		int accChildCount
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			get;
//		}

//		object accChild
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[return: MarshalAs(UnmanagedType.IDispatch)]
//			get;
//		}

//		string accName
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[return: MarshalAs(UnmanagedType.BStr)]
//			get;
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[param: In]
//			[param: MarshalAs(UnmanagedType.BStr)]
//			set;
//		}

//		string accValue
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[return: MarshalAs(UnmanagedType.BStr)]
//			get;
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[param: In]
//			[param: MarshalAs(UnmanagedType.BStr)]
//			set;
//		}

//		string accDescription
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[return: MarshalAs(UnmanagedType.BStr)]
//			get;
//		}

//		object accRole
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[return: MarshalAs(UnmanagedType.Struct)]
//			get;
//		}

//		object accState
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[return: MarshalAs(UnmanagedType.Struct)]
//			get;
//		}

//		string accHelp
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[return: MarshalAs(UnmanagedType.BStr)]
//			get;
//		}

//		int accHelpTopic
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			get;
//		}

//		string accKeyboardShortcut
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[return: MarshalAs(UnmanagedType.BStr)]
//			get;
//		}

//		object accFocus
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[return: MarshalAs(UnmanagedType.Struct)]
//			get;
//		}

//		object accSelection
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[return: MarshalAs(UnmanagedType.Struct)]
//			get;
//		}

//		string accDefaultAction
//		{
//			[MethodImpl(MethodImplOptions.InternalCall)]
//			[return: MarshalAs(UnmanagedType.BStr)]
//			get;
//		}

//		[MethodImpl(MethodImplOptions.InternalCall)]
//		void accSelect([In] int flagsSelect, [Optional] [In] [MarshalAs(UnmanagedType.Struct)] object varChild);

//		[MethodImpl(MethodImplOptions.InternalCall)]
//		void accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, [Optional] [In] [MarshalAs(UnmanagedType.Struct)] object varChild);

//		[MethodImpl(MethodImplOptions.InternalCall)]
//		[return: MarshalAs(UnmanagedType.Struct)]
//		object accNavigate([In] int navDir, [Optional] [In] [MarshalAs(UnmanagedType.Struct)] object varStart);

//		[MethodImpl(MethodImplOptions.InternalCall)]
//		[return: MarshalAs(UnmanagedType.Struct)]
//		object accHitTest([In] int xLeft, [In] int yTop);

//		[MethodImpl(MethodImplOptions.InternalCall)]
//		void accDoDefaultAction([Optional] [In] [MarshalAs(UnmanagedType.Struct)] object varChild);
//	}
//}
