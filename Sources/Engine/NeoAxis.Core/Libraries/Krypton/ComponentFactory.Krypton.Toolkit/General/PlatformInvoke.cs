// *****************************************************************************
// 
//  © Component Factory Pty Ltd 2012. All rights reserved.
//	The software and associated documentation supplied hereunder are the 
//  proprietary information of Component Factory Pty Ltd, 17/267 Nepean Hwy, 
//  Seaford, Vic 3198, Australia and are supplied subject to licence terms.
// 
//
// *****************************************************************************

using System;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Drawing;
using System.Diagnostics;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
	public class PI
    {
        #region Constants
        public const int CS_DROPSHADOW = 0x00020000;
        public const int CS_SAVEBITS = 0x0800;

        public const int WS_CHILD = 0x40000000;
        public const int WS_POPUP = unchecked((int)0x80000000);
        public const int WS_CLIPSIBLINGS = 0x04000000;
        public const int WS_CLIPCHILDREN = 0x02000000;
        public const int WS_MINIMIZE = 0x20000000;
        public const int WS_MAXIMIZE = 0x01000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int WS_CAPTION = 0x00C00000;
        public const int WS_BORDER = 0x00800000;
        public const int PRF_CLIENT = 0x00000004;

        public const int WS_EX_TOPMOST = 0x00000008;
		public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_CONTROLPARENT = 0x00010000;
        public const int WS_EX_APPWINDOW = 0x00040000;
        public const int WS_EX_LAYERED = 0x00080000;
        public const int WS_EX_CLIENTEDGE = 0x00000200;
        public const int WS_EX_COMPOSITED = 0x02000000;
		public const int WS_EX_NOACTIVATE = 0x08000000;
        public const int WS_EX_RIGHT = 0x00001000;
        public const int WS_EX_LEFT = 0x00000000;
        public const int WS_EX_RTLREADING = 0x00002000;
        public const int WS_EX_LEFTSCROLLBAR = 0x00004000;

        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_CLOSE = 0xF060;
        public const int SC_RESTORE = 0xF120;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOWNA = 8;
		public const int WM_CREATE = 0x0001;
        public const int WM_DESTROY = 0x0002;
        public const int WM_NCDESTROY = 0x0082;
        public const int WM_MOVE = 0x0003;
		public const int WM_ACTIVATE = 0x0006;
		public const int WM_SETFOCUS = 0x0007;
        public const int WM_KILLFOCUS = 0x0008;
        public const int WM_SETREDRAW = 0x000B;
        public const int WM_SETTEXT = 0x000C;
        public const int WM_PAINT = 0x000F;
        public const int WM_ERASEBKGND = 0x0014;
        public const int WM_CTLCOLOR = 0x0019;
        public const int WM_ACTIVATEAPP = 0x001C;
        public const int WM_PRINTCLIENT = 0x0318;
        public const int WM_MOUSEACTIVATE = 0x0021;
        public const int WM_GETMINMAXINFO = 0x0024;
        public const int WM_WINDOWPOSCHANGING = 0x0046;
        public const int WM_WINDOWPOSCHANGED = 0x0047;
        public const int WM_HELP = 0x0053;
        public const int WM_NCCALCSIZE = 0x0083;
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_NCPAINT = 0x0085;
        public const int WM_NCACTIVATE = 0x0086;
        public const int WM_NCMOUSEMOVE = 0x00A0;
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int WM_NCLBUTTONUP = 0x00A2;
        public const int WM_NCLBUTTONDBLCLK = 0x00A3;
        public const int WM_NCRBUTTONDOWN = 0x00A4;
        public const int WM_NCRBUTTONUP = 0x00A5;
        public const int WM_NCRBUTTONDBLCLK = 0X00A6;
		public const int WM_NCMBUTTONDOWN = 0x00A7;
        public const int WM_NCMBUTTONUP = 0x00A8;
        public const int WM_NCMBUTTONDBLCLK = 0x00A9;
		public const int WM_NCXBUTTONDOWN = 0x00AB;
		public const int WM_NCXBUTTONUP = 0x00AC;
		public const int WM_NCXBUTTONDBLCLK = 0x00AD;
		public const int WM_SETCURSOR = 0x0020;
        public const int WM_KEYFIRST = 0x0100;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_CHAR = 0x0102;
        public const int WM_DEADCHAR = 0x0103;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;
        public const int WM_SYSCHAR = 0x0106;
        public const int WM_SYSDEADCHAR = 0x0107;
        public const int WM_KEYLAST = 0x0108;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int WM_HSCROLL = 0x0114;
        public const int WM_VSCROLL = 0x0115;
        public const int WM_INITMENU = 0x0116;
        public const int WM_CTLCOLOREDIT = 0x0133;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_CAPTURECHANGED = 0x0215;
        public const int WM_NCMOUSELEAVE = 0x02A2;
        public const int WM_MOUSELEAVE = 0x02A3;
        public const int WM_PRINT = 0x0317;
        public const int WM_CONTEXTMENU = 0x007B;
        public const int WS_MINIMIZEBOX = 0x00020000;
        public const int WS_MAXIMIZEBOX = 0x00010000;
        public const int MA_NOACTIVATE = 0x03;
        public const int EM_FORMATRANGE = 0x0439;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_FRAMECHANGED = 0x0020;
        public const int SWP_NOCOPYBITS = 0x0100;
        public const int SWP_NOOWNERZORDER = 0x0200;
        public const int SWP_SHOWWINDOW = 0x0040;
        public const int SWP_HIDEWINDOW = 0x0080;
        public const int RDW_INVALIDATE = 0x0001;
        public const int RDW_UPDATENOW = 0x0100;
        public const int RDW_FRAME = 0x0400;
        public const int DCX_WINDOW = 0x01;
        public const int DCX_CACHE = 0x02;
        public const int DCX_LOCKWINDOWUPDATE = 0x00000400;
        public const int DCX_CLIPSIBLINGS = 0x10;
        public const int DCX_INTERSECTRGN = 0x80;
        public const int TME_LEAVE = 0x0002;
        public const int TME_NONCLIENT = 0x0010;
        public const int HTNOWHERE = 0x00;
        public const int HTCLIENT = 0x01;
        public const int HTCAPTION = 0x02;
        public const int HTSYSMENU = 0x03;
        public const int HTGROWBOX = 0x04;
        public const int HTSIZE = 0x04;
        public const int HTMENU = 0x05;
        public const int HTLEFT = 0x0A;
        public const int HTRIGHT = 0x0B;
        public const int HTTOP = 0x0C;
        public const int HTTOPLEFT = 0x0D;
        public const int HTTOPRIGHT = 0x0E;
        public const int HTBOTTOM = 0x0F;
        public const int HTBOTTOMLEFT = 0x10;
        public const int HTBOTTOMRIGHT = 0x11;
        public const int HTBORDER = 0x12;
        public const int HTHELP = 0x15;
        public const int HTIGNORE = 0xFF;
        public const int HTTRANSPARENT = -1;
        public const int ULW_ALPHA = 0x00000002;
        public const int DEVICE_BITSPIXEL = 12;
        public const int DEVICE_PLANES = 14;
        public const int SRCCOPY = 0xCC0020;
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int GWL_HWNDPARENT = -8;
        public const int DTM_SETMCCOLOR = 0x1006;
        public const int DTT_COMPOSITED = 8192;
        public const int DTT_GLOWSIZE = 2048;
        public const int DTT_TEXTCOLOR = 1;
        public const int MCSC_BACKGROUND = 0;
        public const int PLANES = 14;
        public const int BITSPIXEL = 12;
		public const int LOGPIXELSX = 88;
		public const int LOGPIXELSY = 90;
		public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;

        //public const int GW_HWNDFIRST = 0;
        //public const int GW_HWNDLAST = 1;
        //public const int GW_HWNDNEXT = 2;
        //public const int GW_HWNDPREV = 3;
        //public const int GW_OWNER = 4;
        public const int GW_CHILD = 5;
        //public const int GW_ENABLEDPOPUP = 6;

        public static readonly IntPtr TRUE = new IntPtr(1);
        public static readonly IntPtr FALSE = new IntPtr(0);

        #endregion

        #region Static Methods
        public static int LOWORD(IntPtr value)
        {
            int int32 = ((int)value.ToInt64() & 0xFFFF);
            return (int32 > 32767) ? int32 - 65536 : int32;
        }

        public static int HIWORD(IntPtr value)
        {
            int int32 = (((int)value.ToInt64() >> 0x10) & 0xFFFF);
            return (int32 > 32767) ? int32 - 65536 : int32;
        }

        public static int LOWORD(int value)
        {
            return (value & 0xFFFF);
        }

        public static int HIWORD(int value)
        {
            return ((value >> 0x10) & 0xFFFF);
        }

        public static int MAKELOWORD(int value)
        {
            return (value & 0xFFFF);
        }

        public static int MAKEHIWORD(int value)
        {
            return ((value & 0xFFFF) << 0x10);
        }

		public static bool IsWine()
		{
			IntPtr hModule = GetModuleHandle("ntdll.dll");
			if (hModule == IntPtr.Zero)
				return false;
			return GetProcAddress(hModule, "wine_get_version") != IntPtr.Zero;
		}

        public static string GetClassName(IntPtr hWnd)
        {
            const int MaxClassName = 256;
            StringBuilder sb = new StringBuilder(MaxClassName);
            GetClassName(hWnd, sb, MaxClassName);
            return sb.ToString();
        }

        // We have this wrapper because casting IntPtr to int may
        // generate OverflowException when one of high 32 bits is set.
        public static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }
		#endregion

		#region Static User32
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern short VkKeyScan(char ch);
        
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr WindowFromPoint(PI.POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowLong", SetLastError = true)]
        internal static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        internal static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        //[SecurityCritical]
        public static int GetWindowLong(IntPtr hWnd, int nIndex)
        {
            int iResult = 0;
            IntPtr result = IntPtr.Zero;
            int error = 0;

            if (IntPtr.Size == 4)
            {
                iResult = GetWindowLong32(hWnd, nIndex);
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(iResult);
            }
            else
            {
                result = GetWindowLongPtr64(hWnd, nIndex);
                error = Marshal.GetLastWin32Error();
                iResult = IntPtrToInt32(result);
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                Debug.WriteLine("GetWindowLong failed.  Error = " + error);
                // throw new System.ComponentModel.Win32Exception(error);
            }

            return iResult;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
        internal static extern int SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
        internal static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        //[SecurityCritical]
        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
                return new IntPtr(SetWindowLongPtr32(hWnd, nIndex, dwNewLong.ToInt32()));
            else
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        //https://github.com/dockpanelsuite/dockpanelsuite/issues/68
        public static extern int ShowWindow(IntPtr hWnd, int cmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern ushort GetKeyState(int virtKey);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern uint SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndAfter, int X, int Y, int Width, int Height, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr rectUpdate, IntPtr hRgnUpdate, uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool RedrawWindow(IntPtr hWnd, ref PI.RECT rectUpdate, IntPtr hRgnUpdate, uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENTS tme);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hRgnClip, uint fdwOptions);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern void DisableProcessWindowsGhosting();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool AdjustWindowRectEx(ref RECT rect, int dwStyle, bool hasMenu, int dwExSytle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out]PI.POINTC pt, int cPoints);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName,int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern  IntPtr BeginPaint(IntPtr hwnd, ref PI.PAINTSTRUCT ps);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool EndPaint(IntPtr hwnd, ref PI.PAINTSTRUCT ps);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool InflateRect(ref RECT lprc, int dx, int dy);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool IsZoomed(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern bool IsIconic(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool GetUpdateRect(IntPtr hWnd, ref RECT rect, bool bErase);
        #endregion

        #region Static Gdi32
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern int BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern int ExcludeClipRect(IntPtr hDC, int x1, int y1, int x2, int y2);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern int IntersectClipRect(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateDIBSection(IntPtr hDC, BITMAPINFO pBMI, uint iUsage, int ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "SaveDC", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern int IntSaveDC(HandleRef hDC);

        [DllImport("gdi32.dll", EntryPoint = "RestoreDC", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern bool IntRestoreDC(HandleRef hDC, int nSavedDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern bool GetViewportOrgEx(HandleRef hDC, [In, Out]POINTC point);

        [DllImport("gdi32.dll", EntryPoint = "CreateRectRgn", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr IntCreateRectRgn(int x1, int y1, int x2, int y2);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern int GetClipRgn(HandleRef hDC, HandleRef hRgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern bool SetViewportOrgEx(HandleRef hDC, int x, int y, [In, Out]POINTC point);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern int GetRgnBox(HandleRef hRegion, ref RECT clipRect);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern int CombineRgn(HandleRef hRgn, HandleRef hRgn1, HandleRef hRgn2, int nCombineMode);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern int SelectClipRgn(HandleRef hDC, HandleRef hRgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern uint SetTextColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern uint SetBkColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr CreateSolidBrush(int crColor);
        #endregion

        #region Static DwmApi
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode)]
        public static extern void DwmIsCompositionEnabled(ref bool enabled);

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode)]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode)]
        public static extern int DwmDefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, out IntPtr result);
        #endregion

        #region Static Ole32
        [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
        public static extern void CoCreateGuid(ref GUIDSTRUCT guid);
        #endregion

        #region Static Uxtheme
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern bool IsAppThemed();

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern bool IsThemeActive();

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, String subAppName, String subIdList);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hDC, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref RECT pRect, ref DTTOPTS pOptions);
        #endregion

        #region Static Kernel32
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern short QueryPerformanceCounter(ref long var);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern short QueryPerformanceFrequency(ref long var);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);
		#endregion

		#region Structures
		[StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int cx;
            public int cy;

            public SIZE(Size size)
            {
                cx = size.Width;
                cy = size.Height;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public POINT(Point pt)
            {
                this.x = pt.X;
                this.y = pt.Y;
            }
            public Point ToPoint() { return new Point(x, y); }

#if DEBUG
            public override string ToString()
            {
                return "{x=" + x + ", y=" + y + "}";
            }
#endif
        }

        [StructLayout(LayoutKind.Sequential)]
        public class POINTC
        {
            public int x;
            public int y;

#if DEBUG
            public override string ToString()
            {
                return "{x=" + x + ", y=" + y + "}";
            }
#endif
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

			public RECT(int l, int t, int r, int b)
			{
				left = l;
				top = t;
				right = r;
				bottom = b;
			}

			public RECT(Rectangle r)
			{
				left = r.Left;
				top = r.Top;
				right = r.Right;
				bottom = r.Bottom;
			}

			public Rectangle ToRectangle()
			{
				return Rectangle.FromLTRB(left, top, right, bottom);
			}
		}

		[StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TRACKMOUSEEVENTS
        {
            public uint cbSize;
            public uint dwFlags;
            public IntPtr hWnd;
            public uint dwHoverTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NCCALCSIZE_PARAMS
        {
            public RECT rectProposed;
            public RECT rectBeforeMove;
            public RECT rectClientBeforeMove;
            public int lpPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GUIDSTRUCT
        {
            public ushort Data1;
            public ushort Data2;
            public ushort Data3;
            public ushort Data4;
            public ushort Data5;
            public ushort Data6;
            public ushort Data7;
            public ushort Data8;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
             public IntPtr hwnd;
             public int message;
             public IntPtr wParam;
             public IntPtr lParam;
             public uint time;
             public POINT pt;
         }

        [StructLayout(LayoutKind.Sequential)]
        public struct DTTOPTS
        {
            public int dwSize;
            public int dwFlags;
            public int crText;
            public int crBorder;
            public int crShadow;
            public int iTextShadowType;
            public POINT ptShadowOffset;
            public int iBorderSize;
            public int iFontPropId;
            public int iColorPropId;
            public int iStateId;
            public bool fApplyOverlay;
            public int iGlowSize;
            public int pfnDrawTextCallback;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class BITMAPINFO
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
            public byte bmiColors_rgbBlue;
            public byte bmiColors_rgbGreen;
            public byte bmiColors_rgbRed;
            public byte bmiColors_rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            private IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FORMATRANGE
        {
            public IntPtr hdc;
            public IntPtr hdcTarget;
            public RECT rc;
            public RECT rcPage;
            public CHARRANGE chrg;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;

            public static MINMAXINFO GetFrom(IntPtr lParam)
            {
                return (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            }
        }

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BLENDFUNCTION
		{
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}

		/// <summary>
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// </summary>            
            public RECT rcWork = new RECT();

            /// <summary>
            /// </summary>            
            public int dwFlags = 0;
        };

        #endregion
    }
}
