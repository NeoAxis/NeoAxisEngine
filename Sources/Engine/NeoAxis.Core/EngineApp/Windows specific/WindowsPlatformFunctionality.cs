// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !ANDROID && !IOS && !WEB && !UWP
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
//using System.Drawing;
using System.IO;
using DirectInput;
using NeoAxis;
using System.Reflection;
using System.Linq;

namespace Internal//NeoAxis
{
	class WindowsPlatformFunctionality : PlatformFunctionality
	{
		static WindowsPlatformFunctionality instance;

		const string applicationWindowClassName = "NeoAxisEngineWindowClass";
		const int suspendModeTimerID = 31692;

		IntPtr backgroundBrush;

		//created window
		WndProcDelegate applicationWindowProc;
		static bool applicationWindowProcNeedCallDefWindowProc = true;

		//bool perfCounterInitialized;
		//long perfCounterStartTime;
		//long perfCounterFrequency;
		//uint perfCounterStartTick;
		//long perfCounterLastTime;

		KeyInfo[] keysInfo = new KeyInfo[ EngineApp.GetEKeysMaxIndex() + 1 ];

		bool suspendModeTimerCreated;

		bool intoMenuLoop;
		bool resizingMoving;

		IntPtr hCursorArrow;

		Dictionary<string, IntPtr> loadedSystemCursors = new Dictionary<string, IntPtr>();

		Vector2 lastMousePositionForMouseMoveDelta;
		Vector2 lastMousePositionForCheckMouseOutsideWindow;
		bool mustIgnoreOneMouseMoveAtRelativeMode;

		double maxFPSLastRenderTime;

		bool goingToWindowedMode;
		bool goingToFullScreenMode;
		bool goingToChangeWindowRectangle;

		static List<SystemSettings.DisplayInfo> tempScreenList = new List<SystemSettings.DisplayInfo>();

		bool createdWindow_UpdateShowSystemCursor;

		///////////////////////////////////////////

		struct KeyInfo
		{
			public int keyCode;

			public KeyInfo( int keyCode )
			{
				this.keyCode = keyCode;
			}
		}

		///////////////////////////////////////////

		//[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		//static extern bool GetProcessAffinityMask( IntPtr hProcess, out uint lpProcessAffinityMask, out uint lpSystemAffinityMask );
		//[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		//static extern bool GetProcessAffinityMask( IntPtr hProcess, out IntPtr lpProcessAffinityMask, out IntPtr lpSystemAffinityMask );

		//[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		//static extern bool SetProcessAffinityMask( IntPtr hProcess, IntPtr dwProcessAffinityMask );

		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		static extern IntPtr GetCurrentThread();

		//[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		//static extern IntPtr SetThreadAffinityMask( IntPtr hThread, ref uint dwThreadAffinityMask );
		//[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		//static extern IntPtr SetThreadAffinityMask( IntPtr hThread, IntPtr dwThreadAffinityMask );

		[DllImport( "kernel32.dll" )]
		static extern bool QueryPerformanceCounter( ref Int64 performanceCount );
		[DllImport( "kernel32.dll" )]
		static extern bool QueryPerformanceFrequency( ref Int64 frequency );

		[DllImport( "kernel32.dll" )]
		static extern uint GetTickCount();

		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		static extern IntPtr GetModuleHandle( string modName );

		///////////////////////////////////////////

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern bool SetWindowText( IntPtr hWnd, string text );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		static extern bool SetWindowPos( IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags );
		const int SWP_NOSIZE = 0x0001;
		const int SWP_NOMOVE = 0x0002;
		const int SWP_NOZORDER = 0x0004;
		const int SWP_NOREDRAW = 0x0008;
		const int SWP_NOACTIVATE = 0x0010;
		const int SWP_FRAMECHANGED = 0x0020;
		const int SWP_SHOWWINDOW = 0x0040;
		const int SWP_HIDEWINDOW = 0x0080;
		const int SWP_NOCOPYBITS = 0x0100;
		const int SWP_NOOWNERZORDER = 0x0200;
		const int SWP_NOSENDCHANGING = 0x0400;

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );
		const int SW_HIDE = 0;
		const int SW_SHOWNORMAL = 1;
		const int SW_NORMAL = 1;
		const int SW_SHOWMINIMIZED = 2;
		const int SW_SHOWMAXIMIZED = 3;
		const int SW_MAXIMIZE = 3;
		const int SW_SHOWNOACTIVATE = 4;
		const int SW_SHOW = 5;
		const int SW_MINIMIZE = 6;
		const int SW_RESTORE = 9;

		[DllImport( "user32.dll", EntryPoint = "IsWindowVisible" )]
		static extern bool _IsWindowVisible( IntPtr hWnd );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		static extern bool GetWindowRect( IntPtr hWnd, out RectangleI rect );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		static extern bool GetClientRect( IntPtr hWnd, out RectangleI rect );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		static extern unsafe bool InvalidateRect( IntPtr hWnd, RectangleI* rect, bool erase );

		[DllImport( "user32.dll" )]
		static extern bool IsIconic( IntPtr hWnd );

		[DllImport( "user32.dll" )]
		static extern IntPtr GetParent( IntPtr hWnd );

		[DllImport( "user32.dll" )]
		static extern bool IsZoomed( IntPtr hWnd );

		[DllImport( "user32.dll" )]
		static extern int ShowCursor( int show );

		[DllImport( "user32.dll" )]
		static extern bool ClipCursor( IntPtr lpRect );

		[DllImport( "user32.dll" )]
		static extern int GetSystemMetrics( int nIndex );
		const int SM_CXSCREEN = 0;
		const int SM_CYSCREEN = 1;
		const int SM_CXSMICON = 49;
		const int SM_CYSMICON = 50;
		const int SM_CMONITORS = 80;

		[DllImport( "user32.dll" )]
		static extern short GetKeyState( int nVirtKey );

		const int VK_LBUTTON = 0x01;
		const int VK_RBUTTON = 0x02;
		const int VK_MBUTTON = 0x04;
		const int VK_XBUTTON1 = 0x05;
		const int VK_XBUTTON2 = 0x06;

		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern bool WaitMessage();

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern IntPtr SendMessage( IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam );

		const int ICON_SMALL = 0;
		const int ICON_BIG = 1;


		[DllImport( "user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode )]
		static extern bool _DestroyWindow( IntPtr hWnd );

		delegate IntPtr WndProcDelegate( IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam );

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
		struct WNDCLASS
		{
			public int style;
			public WndProcDelegate lpfnWndProc;
			public int cbClsExtra;
			public int cbWndExtra;
			public IntPtr hInstance;
			public IntPtr hIcon;
			public IntPtr hCursor;
			public IntPtr hbrBackground;
			public string lpszMenuName;
			public string lpszClassName;
		}

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		static extern short RegisterClass( ref WNDCLASS lpWndClass );

		const int CS_DBLCLKS = 0x0008;

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		static extern short UnregisterClass( string lpClassName, IntPtr hInstance );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern IntPtr LoadCursor( IntPtr hInstance, int lpCursorName );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern bool DestroyCursor( IntPtr hCursor );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		static extern IntPtr LoadCursorFromFile( string lpFileName );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		static extern IntPtr SetCursor( IntPtr hCursor );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		static extern IntPtr SetCapture( IntPtr hCursor );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		static extern bool ReleaseCapture();

		const int IDC_ARROW = 0x7f00;

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		static extern IntPtr GetForegroundWindow();

		[DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
		private static extern int GetWindowThreadProcessId( IntPtr handle, out int processId );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		static extern bool GetCursorPos( out Vector2I lpPoint );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		static extern bool SetCursorPos( int x, int y );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		static extern bool ClientToScreen( IntPtr hWnd, ref Vector2I lpPoint );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		static extern bool ScreenToClient( IntPtr hWnd, ref Vector2I lpPoint );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		static extern IntPtr CreateWindowEx( uint exStyle, string lpszClassName,
			string lpszWindowName, uint style, int x, int y, int width, int height,
			IntPtr hWndParent, IntPtr hMenu, IntPtr hInst, IntPtr pvParam );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern IntPtr DefWindowProc( IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam );

		const uint WS_OVERLAPPED = 0x00000000;
		const uint WS_POPUP = 0x80000000;
		const uint WS_BORDER = 0x00800000;
		const uint WS_SYSMENU = 0x00080000;
		const uint WS_CAPTION = 0x00C00000;
		const uint WS_THICKFRAME = 0x00040000;
		const uint WS_MINIMIZEBOX = 0x00020000;
		const uint WS_MAXIMIZEBOX = 0x00010000;
		const uint WS_VISIBLE = 0x10000000;
		const uint WS_CHILD = 0x40000000;
		const uint WS_MAXIMIZE = 0x01000000;

		const uint WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU |
			WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;

		const int WS_EX_TOPMOST = 0x00000008;

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern bool SetForegroundWindow( IntPtr hWnd );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern IntPtr SetFocus( IntPtr hWnd );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern IntPtr GetFocus();

		[StructLayout( LayoutKind.Sequential )]
		struct MSG
		{
			public IntPtr hwnd;
			public uint message;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public int pt_x;
			public int pt_y;
		}

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern bool PeekMessage( ref MSG msg, IntPtr hWnd,
			uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern bool TranslateMessage( ref MSG lpMsg );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern int DispatchMessage( ref MSG lpMsg );

		const uint PM_REMOVE = 0x0001;
		const uint WM_SETFOCUS = 0x0007;
		const uint WM_KILLFOCUS = 0x0008;
		const uint WM_QUIT = 0x0012;
		const uint WM_SETICON = 0x0080;
		const uint WM_DESTROY = 0x0002;
		const uint WM_SIZE = 0x0005;
		const uint WM_ACTIVATE = 0x0006;
		const uint WM_PAINT = 0x000F;
		const uint WM_CLOSE = 0x0010;
		const uint WM_ERASEBKGND = 0x0014;
		const uint WM_MOUSEMOVE = 0x0200;
		const uint WM_NCMOUSEMOVE = 0x00A0;
		const uint WM_LBUTTONDOWN = 0x0201;
		const uint WM_LBUTTONUP = 0x0202;
		const uint WM_LBUTTONDBLCLK = 0x0203;
		const uint WM_RBUTTONDOWN = 0x0204;
		const uint WM_RBUTTONUP = 0x0205;
		const uint WM_RBUTTONDBLCLK = 0x0206;
		const uint WM_MBUTTONDOWN = 0x0207;
		const uint WM_MBUTTONUP = 0x0208;
		const uint WM_MBUTTONDBLCLK = 0x0209;
		const uint WM_XBUTTONDOWN = 0x020B;
		const uint WM_XBUTTONUP = 0x020C;
		const uint WM_XBUTTONDBLCLK = 0x020D;
		const uint WM_MOUSEWHEEL = 0x020A;
		const uint WM_MOUSEHOVER = 0x02A1;
		const uint WM_MOUSELEAVE = 0x02A3;
		const uint WM_NCMOUSEHOVER = 0x02A0;
		const uint WM_NCMOUSELEAVE = 0x02A2;
		const uint WM_KEYDOWN = 0x0100;
		const uint WM_KEYUP = 0x0101;
		const uint WM_CHAR = 0x0102;
		const uint WM_DEADCHAR = 0x0103;
		const uint WM_SYSKEYDOWN = 0x0104;
		const uint WM_SYSKEYUP = 0x0105;
		const uint WM_SYSCHAR = 0x0106;
		const uint WM_SYSDEADCHAR = 0x0107;
		const uint WM_IME_SETCONTEXT = 0x0281;
		const uint WM_IME_NOTIFY = 0x0282;
		const uint WM_IME_CONTROL = 0x0283;
		const uint WM_IME_COMPOSITIONFULL = 0x0284;
		const uint WM_IME_COMPOSITION = 0x010F;
		const uint WM_IME_SELECT = 0x0285;
		const uint WM_IME_CHAR = 0x0286;
		const uint WM_IME_REQUEST = 0x0288;
		const uint WM_IME_KEYDOWN = 0x0290;
		const uint WM_IME_KEYUP = 0x0291;
		const uint WM_GETMINMAXINFO = 0x0024;
		const uint WM_ENTERMENULOOP = 0x0211;
		const uint WM_EXITMENULOOP = 0x0212;
		const uint WM_ENTERSIZEMOVE = 0x231;
		const uint WM_EXITSIZEMOVE = 0x0232;
		const uint WM_TIMER = 0x0113;
		const uint WM_SETCURSOR = 0x0020;

		const int XBUTTON1 = 0x0001;
		const int XBUTTON2 = 0x0002;

		const int WA_INACTIVE = 0;
		const int WA_ACTIVE = 1;
		const int WA_CLICKACTIVE = 2;

		static IntPtr HWND_NOTOPMOST = new IntPtr( -2 );
		static IntPtr HWND_TOP = IntPtr.Zero;
		static IntPtr HWND_TOPMOST = new IntPtr( -1 );

		[DllImport( "user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Unicode )]
		static extern IntPtr SetWindowLongPtr32( IntPtr hWnd, int nIndex, IntPtr dwNewLong );
		[DllImport( "user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Unicode )]
		static extern IntPtr SetWindowLongPtr64( IntPtr hWnd, int nIndex, IntPtr dwNewLong );

		static IntPtr SetWindowLong( IntPtr hWnd, int nIndex, IntPtr dwNewLong )
		{
			if( IntPtr.Size == 4 )
				return SetWindowLongPtr32( hWnd, nIndex, dwNewLong );
			else
				return SetWindowLongPtr64( hWnd, nIndex, dwNewLong );
		}

		[DllImport( "user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Unicode )]
		static extern IntPtr SetWindowLongPtr32ForWndProc( IntPtr hWnd, int nIndex,
			WndProcDelegate dwNewLong );
		[DllImport( "user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Unicode )]
		static extern IntPtr SetWindowLongPtr64ForWndProc( IntPtr hWnd, int nIndex,
			WndProcDelegate dwNewLong );

		static IntPtr SetWindowLongForWndProc( IntPtr hWnd, int nIndex, WndProcDelegate dwNewLong )
		{
			if( IntPtr.Size == 4 )
				return SetWindowLongPtr32ForWndProc( hWnd, nIndex, dwNewLong );
			else
				return SetWindowLongPtr64ForWndProc( hWnd, nIndex, dwNewLong );
		}

		const int GWL_STYLE = -16;
		const int GWL_EXSTYLE = -20;
		const int GWLP_WNDPROC = -4;

		static IntPtr GetWindowLong( IntPtr hWnd, int nIndex )
		{
			if( IntPtr.Size == 4 )
				return GetWindowLong32( hWnd, nIndex );
			else
				return GetWindowLongPtr64( hWnd, nIndex );
		}

		[DllImport( "user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Unicode )]
		static extern IntPtr GetWindowLong32( IntPtr hWnd, int nIndex );
		[DllImport( "user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Unicode )]
		static extern IntPtr GetWindowLongPtr64( IntPtr hWnd, int nIndex );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern void PostQuitMessage( int nExitCode );

		[StructLayout( LayoutKind.Sequential )]
		struct MINMAXINFO
		{
			public Vector2I ptReserved;
			public Vector2I ptMaxSize;
			public Vector2I ptMaxPosition;
			public Vector2I ptMinTrackSize;
			public Vector2I ptMaxTrackSize;
		}

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true )]
		static extern IntPtr GetDC( IntPtr hWnd );
		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true )]
		static extern int ReleaseDC( IntPtr hWnd, IntPtr hDC );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		static extern IntPtr SetTimer( IntPtr hWnd, IntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc );
		[DllImport( "user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		public static extern bool KillTimer( IntPtr hWnd, IntPtr idEvent );

		[DllImport( "user32.dll" )]
		static extern IntPtr GetDesktopWindow();




		[StructLayout( LayoutKind.Sequential )]
		public struct RECT
		{
			public int Left, Top, Right, Bottom;
		}

		[StructLayout( LayoutKind.Sequential )]
		struct PAINTSTRUCT
		{
			public IntPtr hdc;
			public bool fErase;
			public RECT rcPaint;
			public bool fRestore;
			public bool fIncUpdate;
			[MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )] public byte[] rgbReserved;
		}

		[DllImport( "user32.dll" )]
		static extern IntPtr BeginPaint( IntPtr hwnd, out PAINTSTRUCT lpPaint );

		[DllImport( "user32.dll" )]
		static extern int FillRect( IntPtr hDC, [In] ref RECT lprc, IntPtr hbr );

		[DllImport( "user32.dll" )]
		static extern bool EndPaint( IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint );

		[DllImport( "gdi32.dll" )]
		static extern IntPtr CreateBitmap( int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel, IntPtr lpvBits );

		enum TernaryRasterOperations : uint
		{
			SRCCOPY = 0x00CC0020,
			SRCPAINT = 0x00EE0086,
			SRCAND = 0x008800C6,
			SRCINVERT = 0x00660046,
			SRCERASE = 0x00440328,
			NOTSRCCOPY = 0x00330008,
			NOTSRCERASE = 0x001100A6,
			MERGECOPY = 0x00C000CA,
			MERGEPAINT = 0x00BB0226,
			PATCOPY = 0x00F00021,
			PATPAINT = 0x00FB0A09,
			PATINVERT = 0x005A0049,
			DSTINVERT = 0x00550009,
			BLACKNESS = 0x00000042,
			WHITENESS = 0x00FF0062,
			CAPTUREBLT = 0x40000000
		}

		[DllImport( "gdi32.dll", EntryPoint = "BitBlt", SetLastError = true )]
		[return: MarshalAs( UnmanagedType.Bool )]
		static extern bool BitBlt( [In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop );

		[DllImport( "gdi32.dll" )]
		static extern bool StretchBlt( IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, TernaryRasterOperations dwRop );

		[DllImport( "gdi32.dll", EntryPoint = "SelectObject" )]
		public static extern IntPtr SelectObject( [In] IntPtr hdc, [In] IntPtr hgdiobj );

		[DllImport( "gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true )]
		static extern IntPtr CreateCompatibleDC( [In] IntPtr hdc );

		[DllImport( "gdi32.dll", EntryPoint = "DeleteDC" )]
		public static extern bool DeleteDC( [In] IntPtr hdc );


		//!!!!

		[DllImport( "gdi32.dll" )]
		static extern int SetTextColor( IntPtr hdc, int crColor );

		[DllImport( "gdi32.dll" )]
		static extern int SetBkColor( IntPtr hdc, int crColor );


		private const uint DT_TOP = 0x00000000;
		private const uint DT_LEFT = 0x00000000;
		private const uint DT_RIGHT = 0x00000002;
		private const uint DT_CENTER = 0x00000001;
		private const uint DT_VCENTER = 0x00000004;
		private const uint DT_BOTTOM = 0x00000008;
		private const uint DT_SINGLELINE = 0x00000020;

		[DllImport( "user32.dll" )]
		static extern int DrawTextEx( IntPtr hdc, StringBuilder lpchText, int cchText, ref RECT lprc, uint dwDTFormat, IntPtr/*ref DRAWTEXTPARAMS*/ lpDTParams );

		///////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		struct DEVMODE
		{
			public const int CCHDEVICENAME = 32;
			public const int CCHFORMNAME = 32;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME )]
			public string dmDeviceName;
			public short dmSpecVersion;
			public short dmDriverVersion;
			public short dmSize;
			public short dmDriverExtra;
			public int dmFields;

			public short dmOrientation;
			public short dmPaperSize;
			public short dmPaperLength;
			public short dmPaperWidth;

			public short dmScale;
			public short dmCopies;
			public short dmDefaultSource;
			public short dmPrintQuality;
			public short dmColor;
			public short dmDuplex;
			public short dmYResolution;
			public short dmTTOption;
			public short dmCollate;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME )]
			public string dmFormName;
			public short dmLogPixels;
			public short dmBitsPerPel;
			public int dmPelsWidth;
			public int dmPelsHeight;
			public int dmDisplayFlags;
			public int dmDisplayFrequency;

			public int dmICMMethod;
			public int dmICMIntent;
			public int dmMediaType;
			public int dmDitherType;
			public int dmReserved1;
			public int dmReserved2;
			public int dmPanningWidth;
			public int dmPanningHeight;

			public int dmPositionX; // Using a PointL Struct does not work 
			public int dmPositionY;
		}

		const int DM_BITSPERPEL = 0x00040000;
		const int DM_PELSWIDTH = 0x00080000;
		const int DM_PELSHEIGHT = 0x00100000;
		const int DM_DISPLAYFLAGS = 0x00200000;
		const int DM_DISPLAYFREQUENCY = 0x00400000;
		const int CDS_TEST = 0x00000002;
		const int CDS_FULLSCREEN = 0x00000004;
		const int DISP_CHANGE_SUCCESSFUL = 0;

		[DllImport( "user32.dll" )]
		static extern int ChangeDisplaySettings( ref DEVMODE devMode, int flags );
		[DllImport( "user32.dll" )]
		static extern int ChangeDisplaySettings( IntPtr devMode, int flags );
		[DllImport( "user32.dll" )]
		static extern int EnumDisplaySettings( IntPtr deviceName, int modeNum, out DEVMODE devMode );

		///////////////////////////////////////////

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern int MessageBox( IntPtr hWnd, string text, string caption, int type );
		const int MB_YESNO = 0x00000004;
		const int MB_OK = 0x00000000;
		const int MB_ICONEXCLAMATION = 0x00000030;

		const int IDYES = 6;

		///////////////////////////////////////////

		const int BITSPIXEL = 12;

		[DllImport( "gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true )]
		static extern IntPtr CreateSolidBrush( uint crColor );
		[DllImport( "gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true )]
		static extern bool DeleteObject( IntPtr hObject );
		[DllImport( "gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true )]
		static extern int GetDeviceCaps( IntPtr hDC, int nIndex );

		///////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
		public struct RAMP
		{
			[MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
			public ushort[] red;
			[MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
			public ushort[] green;
			[MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
			public ushort[] blue;
		}

		[DllImport( "gdi32.dll" )]
		static extern int GetDeviceGammaRamp( IntPtr hdc, out RAMP ramp );
		[DllImport( "gdi32.dll" )]
		static extern int SetDeviceGammaRamp( IntPtr hdc, ref RAMP ramp );

		///////////////////////////////////////////

		[DllImport( "psapi.dll", CharSet = CharSet.Unicode )]
		static unsafe extern bool EnumProcessModules( IntPtr hProcess,
			IntPtr* lphModule, uint cb, uint* lpcbNeeded );

		[DllImport( "psapi.dll", CharSet = CharSet.Unicode )]
		static extern uint GetModuleFileNameEx( IntPtr hProcess,
			IntPtr hModule, StringBuilder lpFilename, uint nSize );

		///////////////////////////////////////////

		const uint GCS_RESULTSTR = 0x0800;

		[DllImport( "imm32.dll", CharSet = CharSet.Unicode )]
		static extern IntPtr ImmGetContext( IntPtr hWnd );

		[DllImport( "imm32.dll", CharSet = CharSet.Unicode )]
		static unsafe extern int ImmGetCompositionString( IntPtr hIMC,
			uint dwIndex, void* lpBuf, uint dwBufLen );

		[DllImport( "imm32.dll", CharSet = CharSet.Unicode )]
		static extern bool ImmReleaseContext( IntPtr hWnd, IntPtr hIMC );

		///////////////////////////////////////////

		const int MONITORINFOF_PRIMARY = 0x00000001;

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
		struct MONITORINFOEX
		{
			public int cbSize;
			public RectangleI rcMonitor;
			public RectangleI rcWork;
			public int dwFlags;
			public unsafe fixed sbyte szDeviceName[ 32 ];
		}

		delegate bool MonitorEnumDelegate( IntPtr hMonitor, IntPtr hdcMonitor, ref RectangleI lprcMonitor, IntPtr dwData );

		[DllImport( "user32.dll" )]
		static extern bool EnumDisplayMonitors( IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData );

		[DllImport( "user32.dll" )]
		static extern bool GetMonitorInfo( IntPtr hMonitor, ref MONITORINFOEX lpmi );

		[DllImport( "user32.dll" )]
		static extern bool SetProcessDPIAware();

		///////////////////////////////////////////

		[Flags]
		enum DwmWindowAttribute : uint
		{
			NCRenderingEnabled = 1,
			NCRenderingPolicy,
			TransitionsForceDisabled,
			AllowNCPaint,
			CaptionButtonBounds,
			NonClientRtlLayout,
			ForceIconicRepresentation,
			Flip3DPolicy,
			ExtendedFrameBounds,
			HasIconicBitmap,
			DisallowPeek,
			ExcludedFromPeek,
			Cloak,
			Cloaked,
			FreezeRepresentation,
			PassiveUpdateMode,
			UseHostBackdropBrush,
			UseImmersiveDarkMode = 20,
			WindowCornerPreference = 33,
			BorderColor,
			CaptionColor,
			TextColor,
			VisibleFrameBorderThickness,
			Last
		}

		[DllImport( "dwmapi.dll", PreserveSig = true )]
		static extern int DwmSetWindowAttribute( IntPtr hwnd, DwmWindowAttribute attr, ref int attrValue, int attrSize );

		///////////////////////////////////////////

		//public static EngineApp App
		//{
		//	get { return EngineApp.Instance; }
		//}

		///////////////////////////////////////////

		public WindowsPlatformFunctionality()
		{
			instance = this;

			if( SystemSettings.OSVersion.Major >= 6 )
			{
				try
				{
					SetProcessDPIAware();
				}
				catch { }
			}
		}

		public override Vector2I GetScreenSize()
		{
			return new Vector2I( GetSystemMetrics( SM_CXSCREEN ), GetSystemMetrics( SM_CYSCREEN ) );
		}

		public override int GetScreenBitsPerPixel()
		{
			int bpp = 32;

			IntPtr screenDC = GetDC( IntPtr.Zero );
			if( screenDC != IntPtr.Zero )
			{
				bpp = GetDeviceCaps( screenDC, BITSPIXEL );
				ReleaseDC( IntPtr.Zero, screenDC );
			}

			return bpp;
		}

		//public override int GetMonitorCount()
		//{
		//   return GetSystemMetrics( SM_CMONITORS );
		//}

		//public override double GetSystemTime()
		//{
		//	//!!!!never call

		//	long time = Stopwatch.GetTimestamp();
		//	double elapsedSeconds = time * ( 1.0 / Stopwatch.Frequency );
		//	return elapsedSeconds;

		//	//!!!!slow

		//	//if( !perfCounterInitialized )
		//	//{
		//	//	QueryPerformanceFrequency( ref perfCounterFrequency );
		//	//	QueryPerformanceCounter( ref perfCounterStartTime );
		//	//	perfCounterStartTick = GetTickCount();
		//	//	perfCounterInitialized = true;
		//	//}

		//	//long currentTime = 0;
		//	//{
		//	//	Process Proc = Process.GetCurrentProcess();
		//	//	long oldAffinityMask = (long)Proc.ProcessorAffinity;
		//	//	long affinityMask = oldAffinityMask;
		//	//	affinityMask &= 0x0001; // use only first processor
		//	//	Proc.ProcessorAffinity = (IntPtr)affinityMask;

		//	//	QueryPerformanceCounter( ref currentTime );

		//	//	Proc.ProcessorAffinity = (IntPtr)oldAffinityMask;

		//	//	//IntPtr mask = (IntPtr)1;

		//	//	////find the lowest core that this process uses
		//	//	//IntPtr processMask;
		//	//	//IntPtr systemMask;
		//	//	//if( GetProcessAffinityMask( Process.GetCurrentProcess().Handle, out processMask, out systemMask ) )
		//	//	//{
		//	//	//	while( ( (int)mask & (int)processMask ) == 0 )
		//	//	//		mask = (IntPtr)( (int)mask << 1 );
		//	//	//}

		//	//	//IntPtr thread = GetCurrentThread();
		//	//	//IntPtr oldMask = SetThreadAffinityMask( thread, mask );
		//	//	//QueryPerformanceCounter( ref currentTime );
		//	//	//SetThreadAffinityMask( thread, oldMask );
		//	//}

		//	//long newTime = currentTime - perfCounterStartTime;

		//	//// scale by 1000 for milliseconds
		//	//uint newTicks = (uint)( 1000 * newTime / perfCounterFrequency );

		//	//// detect and compensate for performance counter leaps
		//	//// (surprisingly common, see Microsoft KB: Q274323)
		//	//uint check = GetTickCount() - perfCounterStartTick;
		//	//int msecOff = (int)( newTicks - check );
		//	//if( msecOff < -100 || msecOff > 100 )
		//	//{
		//	//	// We must keep the timer running forward :)
		//	//	long adjust = Math.Min( msecOff * perfCounterFrequency / 1000, newTime - perfCounterLastTime );
		//	//	perfCounterStartTime += adjust;
		//	//	newTime -= adjust;
		//	//}

		//	//// Record last time for adjust
		//	//perfCounterLastTime = newTime;

		//	//return (double)newTime / perfCounterFrequency;
		//}

		public override Vector2I GetSmallIconSize()
		{
			return new Vector2I( GetSystemMetrics( SM_CXSMICON ), GetSystemMetrics( SM_CYSMICON ) );
		}

		//public override void SetProcessAffinityMask( IntPtr mask )
		//{
		//   SetProcessAffinityMask( Process.GetCurrentProcess().Handle, mask );
		//}

		public override IntPtr CreatedWindow_CreateWindow()
		{
			//backgroundBrush
			backgroundBrush = CreateSolidBrush( 0 );

			//load arrow cursor
			hCursorArrow = LoadCursor( IntPtr.Zero, IDC_ARROW );

			//register window class
			applicationWindowProc = CreatedWindow_ApplicationWindowProc;

			WNDCLASS wndClass = new WNDCLASS();
			wndClass.style = CS_DBLCLKS;
			wndClass.lpfnWndProc = applicationWindowProc;
			wndClass.hInstance = GetModuleHandle( null );
			wndClass.hCursor = IntPtr.Zero;
			wndClass.lpszClassName = applicationWindowClassName;
			wndClass.hbrBackground = backgroundBrush;

			if( RegisterClass( ref wndClass ) == 0 )
			{
				Log.Fatal( "EngineApp: WindowCreate: RegisterClass failed." );
				return IntPtr.Zero;
			}

			//create window

			bool showMaximized = !EngineApp.FullscreenEnabled &&
				EngineApp.InitSettings.CreateWindowState.Value == EngineApp.WindowStateEnum.Maximized &&
				!EngineApp.InitSettings.MultiMonitorMode.Value;
			bool showMinimized = EngineApp.InitSettings.CreateWindowState.Value == EngineApp.WindowStateEnum.Minimized;
			//bool showMaximized = !App.FullScreenEnabled && App.VideoMode == GetScreenSize() && !EngineApp.InitializationParameters.MultiMonitorMode;

			Vector2I position;
			Vector2I size;
			{
				if( showMaximized )
				{
					size = new Vector2I( 800, 600 );
					position = ( GetScreenSize() - size ) / 2;
				}
				else
				{
					//!!!!!EngineApp.InitializationParameters.MultiMonitorMode.Value? не False?
					if( !EngineApp.FullscreenEnabled || EngineApp.InitSettings.MultiMonitorMode.Value )
						position = EngineApp.InitSettings.CreateWindowPosition.Value;
					else
						position = Vector2I.Zero;
					size = EngineApp.InitSettings.CreateWindowSize.Value;
				}
			}

			uint style = 0;
			uint exStyle = 0;
			{
				if( !showMaximized )
					style |= WS_VISIBLE;

				if( EngineApp.FullscreenEnabled )
					style |= WS_POPUP;
				else
					style |= WS_OVERLAPPEDWINDOW;

				if( EngineApp.FullscreenEnabled && !Debugger.IsAttached )
					exStyle |= WS_EX_TOPMOST;
			}

			IntPtr windowHandle = CreateWindowEx( exStyle, applicationWindowClassName,
				EngineApp.CreatedInsideEngineWindow.Title, style, position.X, position.Y, size.X, size.Y, IntPtr.Zero,
				IntPtr.Zero, GetModuleHandle( null ), IntPtr.Zero );

			if( SystemSettings.DarkMode )
				SetDarkMode( windowHandle, true );

			if( showMaximized )
				ShowWindow( windowHandle, SW_SHOWMAXIMIZED );
			if( showMinimized )
				ShowWindow( windowHandle, SW_SHOWMINIMIZED );

			SetForegroundWindow( windowHandle );
			SetFocus( windowHandle );

			SetTimer( windowHandle, (IntPtr)suspendModeTimerID, 10, IntPtr.Zero );
			suspendModeTimerCreated = true;

			return windowHandle;
		}

		public override void CreatedWindow_DestroyWindow()
		{
			intoMenuLoop = false;

			if( suspendModeTimerCreated )
			{
				KillTimer( EngineApp.ApplicationWindowHandle, (IntPtr)suspendModeTimerID );
				suspendModeTimerCreated = false;
			}

			_DestroyWindow( EngineApp.ApplicationWindowHandle );

			UnregisterClass( applicationWindowClassName, GetModuleHandle( null ) );
			applicationWindowProc = null;

			foreach( IntPtr hCursor in loadedSystemCursors.Values )
			{
				if( hCursor != IntPtr.Zero )
					DestroyCursor( hCursor );
			}
			loadedSystemCursors.Clear();

			//destroy backgroundBrush
			if( backgroundBrush != IntPtr.Zero )
			{
				DeleteObject( backgroundBrush );
				backgroundBrush = IntPtr.Zero;
			}
		}

		static bool GetEKeyByKeyCode( int keyCode, out EKeys eKey )
		{
			if( Enum.IsDefined( typeof( EKeys ), (int)keyCode ) )
			{
				eKey = (EKeys)(int)keyCode;
				return true;
			}
			else
			{
				eKey = EKeys.Cancel;
				return false;
			}
		}

		static int HIWORD( int n )
		{
			return ( ( n >> 0x10 ) & 0xffff );
		}

		static int LOWORD( int n )
		{
			return ( n & 0xffff );
		}

		static void RemovePendingMessage( IntPtr hWnd, uint message )
		{
			MSG msg = new MSG();
			while( PeekMessage( ref msg, hWnd, message, message, PM_REMOVE ) )
			{
			}
		}

		static IntPtr CreatedWindow_ApplicationWindowProc( IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam )
		{
			//try
			//{

			if( EngineApp.Instance == null || Log.FatalActivated )
				return DefWindowProc( hWnd, message, wParam, lParam );

			bool processMessageByEngine = true;
			EngineApp.PerformWindowsWndProcEvent( message, wParam, lParam, ref processMessageByEngine );

			Viewport viewport = null;
			if( !RenderingSystem.Disposed && RenderingSystem.ApplicationRenderTarget != null )
				viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;

			if( processMessageByEngine )
			{
				switch( message )
				{
				case WM_ERASEBKGND:
					break;

				case WM_CLOSE:
					if( EngineApp.CreatedInsideEngineWindow.HideOnClose )
						ShowWindow( hWnd, SW_HIDE );
					else
						PostQuitMessage( 0 );
					return IntPtr.Zero;

				case WM_ENTERSIZEMOVE:
					instance.resizingMoving = true;
					return IntPtr.Zero;

				case WM_EXITSIZEMOVE:
					instance.resizingMoving = false;
					return IntPtr.Zero;

				case WM_SIZE:
					if( !instance.goingToWindowedMode && !instance.goingToFullScreenMode && !instance.goingToChangeWindowRectangle )
					{
						EngineApp.CreatedWindowProcessResize();
						return IntPtr.Zero;
					}
					break;

				case WM_GETMINMAXINFO:
					unsafe
					{
						MINMAXINFO* info = (MINMAXINFO*)lParam;
						var size = new Vector2I( 100, 100 );
						if( ProjectSettings.Initialized )
						{
							size = ProjectSettings.Get.General.WindowSizeMinimal.Value;
							if( ProjectSettings.Get.General.WindowSizeApplySystemFontScale )
								size = ( size.ToVector2() * SystemSettings.DPIScale ).ToVector2I();
							info->ptMinTrackSize = size;
						}
						info->ptMinTrackSize = size;
					}
					return IntPtr.Zero;
				}

				if( EngineApp.Created && !EngineApp.Closing )
				{
					switch( message )
					{

					case WM_SETFOCUS:
						{
							instance.mustIgnoreOneMouseMoveAtRelativeMode = true;
							instance.CreatedWindow_UpdateShowSystemCursor( true );
							return IntPtr.Zero;
						}
					//break;

					case WM_KILLFOCUS:
						{
							instance.mustIgnoreOneMouseMoveAtRelativeMode = true;
							instance.CreatedWindow_UpdateShowSystemCursor( true );
							return IntPtr.Zero;
						}
					//break;

					case WM_ACTIVATE:
						{
							if( !instance.goingToWindowedMode && !instance.goingToFullScreenMode && !instance.goingToChangeWindowRectangle )
							{
								bool activate = LOWORD( (int)wParam ) != WA_INACTIVE;

								instance.mustIgnoreOneMouseMoveAtRelativeMode = true;

								if( activate )
								{
									if( EngineApp.FullscreenEnabled )
									{
										if( EngineApp.FullscreenSize != instance.GetScreenSize() )
											EngineApp.MustChangeVideoMode();
									}
								}
								else
								{
									if( viewport.MouseRelativeMode )
									{
										ReleaseCapture();
										ClipCursor( IntPtr.Zero );
									}

									//!!!!просто обновляем. ведь внутри и так проверка есть. еще можно зафорсить. но не будет ли так, 
									//что отключится слишком быстро? нужно там где включает ставить флаг -> ChangeVUdeMode.
									EngineApp.EnginePauseUpdateState( false, true );
									//if( App.SuspendWorkingWhenApplicationIsNotActive )
									//   App.DoSystemPause( true, true );

									if( EngineApp.FullscreenEnabled )
									{
										instance.SetWindowState( WindowState.Minimized );

										if( !EngineApp.NeedExit )
											SystemSettings.RestoreVideoMode();
									}
								}
								return IntPtr.Zero;
							}
						}
						break;

					case WM_ENTERMENULOOP:
						{
							if( viewport.MouseRelativeMode )
							{
								ReleaseCapture();
								ClipCursor( IntPtr.Zero );
							}

							//if( App.SuspendWorkingWhenApplicationIsNotActive )
							//   App.DoSystemPause( true, true );
							instance.intoMenuLoop = true;
							EngineApp.EnginePauseUpdateState( false, true );

							return IntPtr.Zero;
						}
					//break;

					case WM_EXITMENULOOP:
						instance.intoMenuLoop = false;
						return IntPtr.Zero;

					case WM_LBUTTONDOWN:
					case WM_LBUTTONDBLCLK:
						{
							EMouseButtons button = EMouseButtons.Left;
							bool handled = false;
							viewport.PerformMouseDown( button, ref handled );
							if( message == WM_LBUTTONDBLCLK )
							{
								bool handled2 = false;
								viewport.PerformMouseDoubleClick( button, ref handled2 );
							}
							return IntPtr.Zero;
						}
					//break;

					case WM_RBUTTONDOWN:
					case WM_RBUTTONDBLCLK:
						{
							EMouseButtons button = EMouseButtons.Right;
							bool handled = false;
							viewport.PerformMouseDown( button, ref handled );
							if( message == WM_RBUTTONDBLCLK )
							{
								bool handled2 = false;
								viewport.PerformMouseDoubleClick( button, ref handled2 );
							}
							return IntPtr.Zero;
						}
					//break;

					case WM_MBUTTONDOWN:
					case WM_MBUTTONDBLCLK:
						{
							EMouseButtons button = EMouseButtons.Middle;
							bool handled = false;
							viewport.PerformMouseDown( button, ref handled );
							if( message == WM_MBUTTONDBLCLK )
							{
								bool handled2 = false;
								viewport.PerformMouseDoubleClick( button, ref handled2 );
							}
							return IntPtr.Zero;
						}
					//break;

					case WM_XBUTTONDOWN:
					case WM_XBUTTONDBLCLK:
						{
							int b = HIWORD( (int)wParam );
							if( b == XBUTTON1 )
							{
								EMouseButtons button = EMouseButtons.XButton1;
								bool handled = false;
								viewport.PerformMouseDown( button, ref handled );
								if( message == WM_XBUTTONDBLCLK )
								{
									bool handled2 = false;
									viewport.PerformMouseDoubleClick( button, ref handled2 );
								}
								return new IntPtr( 1 );
							}
							else if( b == XBUTTON2 )
							{
								EMouseButtons button = EMouseButtons.XButton2;
								bool handled = false;
								viewport.PerformMouseDown( button, ref handled );
								if( message == WM_XBUTTONDBLCLK )
								{
									bool handled2 = false;
									viewport.PerformMouseDoubleClick( button, ref handled2 );
								}
								return new IntPtr( 1 );
							}
						}
						break;

					case WM_LBUTTONUP:
						{
							bool handled = false;
							viewport.PerformMouseUp( EMouseButtons.Left, ref handled );
							return IntPtr.Zero;
						}

					case WM_RBUTTONUP:
						{
							bool handled = false;
							viewport.PerformMouseUp( EMouseButtons.Right, ref handled );
							return IntPtr.Zero;
						}

					case WM_MBUTTONUP:
						{
							bool handled = false;
							viewport.PerformMouseUp( EMouseButtons.Middle, ref handled );
							return IntPtr.Zero;
						}

					case WM_XBUTTONUP:
						{
							int b = HIWORD( (int)wParam );
							if( b == XBUTTON1 )
							{
								bool handled = false;
								viewport.PerformMouseUp( EMouseButtons.XButton1, ref handled );
								return new IntPtr( 1 );
							}
							else if( b == XBUTTON2 )
							{
								bool handled = false;
								viewport.PerformMouseUp( EMouseButtons.XButton2, ref handled );
								return new IntPtr( 1 );
							}
						}
						break;

					case WM_MOUSEWHEEL:
						{
							int delta = (short)HIWORD( (int)wParam.ToInt64() );
							bool handled = false;
							viewport.PerformMouseWheel( delta, ref handled );
							return IntPtr.Zero;
						}
					//break;

					case WM_MOUSEMOVE:
					case WM_NCMOUSEMOVE:
						EngineApp.CreatedInsideEngineWindow.ProcessMouseMoveEvent();
						return IntPtr.Zero;

					case WM_SYSKEYDOWN:
					case WM_KEYDOWN:
						{
							int keyCode = (int)wParam;

							EKeys eKey;
							if( GetEKeyByKeyCode( keyCode, out eKey ) )
							{
								//support Alt+F4 in mouse relative mode. Alt+F4 is disabled during captured cursor.
								if( viewport.MouseRelativeMode )
								{
									if( eKey == EKeys.F4 && viewport.IsKeyPressed( EKeys.Alt ) )
									{
										EngineApp.NeedExit = true;
										return IntPtr.Zero;
									}
								}

								KeyEvent keyEvent = new KeyEvent( eKey );

								instance.keysInfo[ (int)keyEvent.Key ] = new KeyInfo( keyCode );

								bool handled = false;
								bool suppressKeyPress = false;

								//EKeys.LShift, EKeys.RShift
								if( eKey == EKeys.Shift )
								{
									for( int n = 0; n < 2; n++ )
									{
										EKeys eKey2 = ( n == 0 ) ? EKeys.LShift : EKeys.RShift;
										if( GetKeyState( (int)eKey2 ) < 0 )
										{
											instance.keysInfo[ (int)eKey2 ] = new KeyInfo( (int)eKey2 );

											KeyEvent keyEvent2 = new KeyEvent( eKey2 );
											viewport.PerformKeyDown( keyEvent2, ref handled );
											if( keyEvent2.SuppressKeyPress )
												suppressKeyPress = true;
										}
									}
								}

								//EKeys.LControl, EKeys.RControl
								if( eKey == EKeys.Control )
								{
									for( int n = 0; n < 2; n++ )
									{
										EKeys eKey2 = ( n == 0 ) ? EKeys.LControl : EKeys.RControl;
										if( GetKeyState( (int)eKey2 ) < 0 )
										{
											instance.keysInfo[ (int)eKey2 ] = new KeyInfo( (int)eKey2 );

											KeyEvent keyEvent2 = new KeyEvent( eKey2 );
											viewport.PerformKeyDown( keyEvent2, ref handled );
											if( keyEvent2.SuppressKeyPress )
												suppressKeyPress = true;
										}
									}
								}

								//EKeys.LAlt, EKeys.RAlt
								if( eKey == EKeys.Alt )
								{
									for( int n = 0; n < 2; n++ )
									{
										EKeys eKey2 = ( n == 0 ) ? EKeys.LAlt : EKeys.RAlt;
										if( GetKeyState( (int)eKey2 ) < 0 )
										{
											instance.keysInfo[ (int)eKey2 ] = new KeyInfo( (int)eKey2 );

											KeyEvent keyEvent2 = new KeyEvent( eKey2 );
											viewport.PerformKeyDown( keyEvent2, ref handled );
											if( keyEvent2.SuppressKeyPress )
												suppressKeyPress = true;
										}
									}
								}

								viewport.PerformKeyDown( keyEvent, ref handled );
								if( keyEvent.SuppressKeyPress )
									suppressKeyPress = true;

								if( suppressKeyPress )
								{
									RemovePendingMessage( hWnd, WM_CHAR );
									RemovePendingMessage( hWnd, WM_SYSCHAR );
									RemovePendingMessage( hWnd, WM_IME_CHAR );
								}

								if( !handled && EngineApp.InitSettings.AllowChangeScreenVideoMode )
								{
									if( viewport.IsKeyPressed( EKeys.Alt ) && eKey == EKeys.Return )
									{
										EngineApp.SetFullscreenMode( !EngineApp.FullscreenEnabled, EngineApp.FullscreenSize );
										//App.FullScreen = !App.FullScreen;
										handled = true;
									}
								}

								if( handled )
									return IntPtr.Zero;
							}
						}
						break;

					case WM_SYSKEYUP:
					case WM_KEYUP:
						{
							int keyCode = (int)wParam;

							EKeys eKey;
							if( GetEKeyByKeyCode( keyCode, out eKey ) )
							{
								bool handled = false;

								//EKeys.LShift, EKeys.RShift
								if( eKey == EKeys.Shift )
								{
									for( int n = 0; n < 2; n++ )
									{
										EKeys eKey2 = ( n == 0 ) ? EKeys.LShift : EKeys.RShift;
										if( GetKeyState( (int)eKey2 ) >= 0 )
											viewport.PerformKeyUp( new KeyEvent( eKey2 ), ref handled );
									}
								}

								//EKeys.LControl, EKeys.RControl
								if( eKey == EKeys.Control )
								{
									for( int n = 0; n < 2; n++ )
									{
										EKeys eKey2 = ( n == 0 ) ? EKeys.LControl : EKeys.RControl;
										if( GetKeyState( (int)eKey2 ) >= 0 )
											viewport.PerformKeyUp( new KeyEvent( eKey2 ), ref handled );
									}
								}

								//EKeys.LAlt, EKeys.RAlt
								if( eKey == EKeys.Alt )
								{
									for( int n = 0; n < 2; n++ )
									{
										EKeys eKey2 = ( n == 0 ) ? EKeys.LAlt : EKeys.RAlt;
										if( GetKeyState( (int)eKey2 ) >= 0 )
											viewport.PerformKeyUp( new KeyEvent( eKey2 ), ref handled );
									}
								}

								KeyEvent keyEvent = new KeyEvent( eKey );
								viewport.PerformKeyUp( keyEvent, ref handled );

								if( handled )
									return IntPtr.Zero;
							}
						}
						break;

					case WM_SYSCHAR:
					case WM_CHAR:
						{
							char keyChar = (char)wParam;

							KeyPressEvent keyPressEvent = new KeyPressEvent( keyChar );
							bool handled = false;
							viewport.PerformKeyPress( keyPressEvent, ref handled );
							if( handled )
								return IntPtr.Zero;
						}
						break;

					case WM_IME_COMPOSITION:
						if( ( (uint)lParam & GCS_RESULTSTR ) != 0 )
						{
							unsafe
							{
								IntPtr context = ImmGetContext( hWnd );
								if( context != IntPtr.Zero )
								{
									int bytes = ImmGetCompositionString( context, GCS_RESULTSTR, null, 0 );
									if( bytes > 0 )
									{
										char[] characters = new char[ bytes / 2 ];
										fixed( char* pCharacters = characters )
										{
											ImmGetCompositionString( context, GCS_RESULTSTR, pCharacters, (uint)bytes );
										}

										foreach( char character in characters )
										{
											if( character != 0 )
											{
												KeyPressEvent keyPressEvent = new KeyPressEvent( character );
												bool handled = false;
												viewport.PerformKeyPress( keyPressEvent, ref handled );
											}
										}
									}

									ImmReleaseContext( hWnd, context );
								}
							}

							return IntPtr.Zero;
						}
						break;

					case WM_TIMER:
						if( (int)wParam == suspendModeTimerID )
						{
							if( EngineApp.DrawSplashScreen != ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled )
							{
								unsafe
								{
									InvalidateRect( hWnd, null, false );
								}
							}

							if( !IsAllowApplicationIdle() )
								EngineApp.CreatedWindowApplicationIdle( true );

							return IntPtr.Zero;
						}
						break;

					case WM_PAINT:

#if !DEPLOY

						RECT MakeRECT( int left, int top, int width, int height )
						{
							RECT r = new RECT();
							r.Left = left;
							r.Top = top;
							r.Right = left + width;
							r.Bottom = top + height;
							return r;
						}

						////draw splash screen
						//var drawSplashScreen = EngineApp.DrawSplashScreen;

						//if( drawSplashScreen != ProjectSettingsPage_General.EngineSplashScreenStyleEnum.Disabled )
						//{
						//var hdc = BeginPaint( hWnd, out var ps );

						////int COLOR_WINDOW = 3;// 5;
						////FillRect( hdc, ref ps.rcPaint, (IntPtr)( COLOR_WINDOW + 1 ) );
						////FillRect( hdc, out ps.rcPaint, (HBRUSH)( COLOR_WINDOW + 1 ) );

						//using( var bitmap = EngineInfo.GetSplashLogoImage( drawSplashScreen ) )
						//{
						//	var screenRect = instance.CreatedWindow_GetClientRectangle();
						//	var screenSize = screenRect.Size;
						//	var center = screenRect.GetCenter();

						//	Bitmap bitmap2 = null;

						//	var imageSize = new Vector2I( bitmap.Size.Width, bitmap.Size.Height );
						//	if( imageSize.X > screenSize.X )
						//	{
						//		double factor = (double)screenSize.X / (double)imageSize.X;
						//		imageSize = ( imageSize.ToVector2() * factor ).ToVector2I();
						//		bitmap2 = Win32Utility.ResizeImage( bitmap, imageSize.X, imageSize.Y );
						//	}

						//	var splashBitmap = ( bitmap2 ?? bitmap ).GetHbitmap();
						//	if( splashBitmap != IntPtr.Zero )
						//	{
						//		var hdcMem = CreateCompatibleDC( hdc );
						//		var oldBitmap = SelectObject( hdcMem, splashBitmap );

						//		var destRect = new System.Drawing.Rectangle( center.X - imageSize.X / 2, center.Y - imageSize.Y / 2, imageSize.X, imageSize.Y );

						//		//draw background
						//		{
						//			var color = drawSplashScreen == ProjectSettingsPage_General.EngineSplashScreenStyleEnum.WhiteBackground ? Color.White : Color.Black;

						//			//!!!!
						//			//color = Color.Gray;

						//			var brush = CreateSolidBrush( (uint)ColorTranslator.ToWin32( color ) );

						//			if( destRect.Left > 0 )
						//			{
						//				RECT r = MakeRECT( 0, 0, destRect.Left, screenSize.Y );
						//				FillRect( hdc, ref r, brush );
						//			}
						//			if( destRect.Right < screenSize.X )
						//			{
						//				RECT r = MakeRECT( destRect.Right, 0, screenSize.X - destRect.Right, screenSize.Y );
						//				FillRect( hdc, ref r, brush );
						//			}
						//			if( destRect.Top > 0 )
						//			{
						//				RECT r = MakeRECT( 0, 0, screenSize.X, destRect.Top );
						//				FillRect( hdc, ref r, brush );
						//			}
						//			if( destRect.Bottom < screenSize.Y )
						//			{
						//				RECT r = MakeRECT( 0, destRect.Bottom, screenSize.X, screenSize.Y - destRect.Bottom );
						//				FillRect( hdc, ref r, brush );
						//			}

						//			DeleteObject( brush );
						//		}

						//		////!!!!
						//		//{
						//		//	var text = new StringBuilder( "Test test" );

						//		//	var rect = MakeRECT( 0, 0, 500, 500 );
						//		//	var format = DT_CENTER | DT_VCENTER | DT_SINGLELINE;

						//		//	SetTextColor( hdc, ColorTranslator.ToWin32( Color.FromArgb( 255, 0, 0 ) ) );

						//		//	DrawTextEx( hdc, text, text.Length, ref rect, format, IntPtr.Zero );
						//		//}

						//		//if( stretch )
						//		//{
						//		//	StretchBlt( hdc, destRect.Left, destRect.Top, destRect.Width, destRect.Height, hdcMem, 0, 0, bitmap.Size.Width, bitmap.Size.Height, TernaryRasterOperations.SRCCOPY );
						//		//}
						//		//else
						//		//{

						//		BitBlt( hdc, destRect.Left, destRect.Top, destRect.Width, destRect.Height, hdcMem, 0, 0, TernaryRasterOperations.SRCCOPY );

						//		//}

						//		SelectObject( hdcMem, oldBitmap );
						//		DeleteDC( hdcMem );

						//	}

						//	bitmap2?.Dispose();
						//}

						//EndPaint( hWnd, ref ps );

						//	return IntPtr.Zero;
						//}

						if( SystemSettings.CommandLineParameters.TryGetValue( "-server", out var projectServer ) && projectServer == "1" && RenderingSystem.BackendNull )
						{
							int ToWin32( int r, int g, int b )
							{
								return ( b << 16 ) | ( g << 8 ) | r;
							}

							var hdc = BeginPaint( hWnd, out var ps );

							{
								var screenRect = instance.CreatedWindow_GetClientRectangle();
								var screenSize = screenRect.Size;

								//var textColor = Color.White;
								//var backColor = Color.Black;

								//draw background
								{
									var brush = CreateSolidBrush( (uint)ToWin32( 0, 0, 0 ) );
									//var brush = CreateSolidBrush( (uint)ColorTranslator.ToWin32( backColor ) );

									var rect = MakeRECT( 0, 0, screenSize.X, screenSize.Y );
									FillRect( hdc, ref rect, brush );

									DeleteObject( brush );
								}

								//draw text
								{
									var text = new StringBuilder( "Server mode" );

									var rect = MakeRECT( 10, 10, screenSize.X, screenSize.Y );
									var format = DT_LEFT | DT_TOP;
									//var format = DT_CENTER | DT_VCENTER;// | DT_SINGLELINE;

									SetTextColor( hdc, ToWin32( 255, 255, 255 ) );
									SetBkColor( hdc, ToWin32( 0, 0, 0 ) );

									//SetTextColor( hdc, ColorTranslator.ToWin32( textColor ) );
									//SetBkColor( hdc, ColorTranslator.ToWin32( backColor ) );

									DrawTextEx( hdc, text, text.Length, ref rect, format, IntPtr.Zero );
								}
							}

							EndPaint( hWnd, ref ps );

							return IntPtr.Zero;
						}


#endif

						if( EngineApp.insideRunMessageLoop && EngineApp.EnginePaused && !instance.resizingMoving &&
							!instance.intoMenuLoop && !instance.goingToWindowedMode &&
							!instance.goingToFullScreenMode && !instance.goingToChangeWindowRectangle )
						{
							EngineApp.CreatedWindowApplicationIdle( false );
						}
						break;
					}
				}
			}

			if( applicationWindowProcNeedCallDefWindowProc )
				return DefWindowProc( hWnd, message, wParam, lParam );
			else
				return IntPtr.Zero;
			//}
			//catch( Exception e )
			//{
			//	Log.Fatal( "WindowsPlatformFunctionality: CreatedWindow_ApplicationWindowProc: Exception: " + e.Message );
			//	return IntPtr.Zero;
			//}
		}

		public override void CreatedWindow_ProcessMessageEvents()
		{
			MSG msg = new MSG();

			while( PeekMessage( ref msg, IntPtr.Zero, 0, 0, PM_REMOVE ) )
			{
				if( msg.message == WM_QUIT )
				{
					EngineApp.NeedExit = true;
					break;
				}

				TranslateMessage( ref msg );
				DispatchMessage( ref msg );
			}
		}

		public override void CreatedWindow_RunMessageLoop()
		{
			while( true )
			{
				MSG msg = new MSG();

				if( PeekMessage( ref msg, IntPtr.Zero, 0, 0, PM_REMOVE ) )
				{
					if( msg.message == WM_QUIT )
						break;

					TranslateMessage( ref msg );
					DispatchMessage( ref msg );

					continue;
				}
				else
				{
					if( EngineApp.NeedExit )
						break;

					if( IsAllowApplicationIdle() )
					{
						if( EngineApp.RenderVideoToFileData == null )
							EngineApp.UpdateEngineTime();
						double time = EngineApp.EngineTime;
						bool needSleep = EngineApp.MaxFPS != 0 && time < maxFPSLastRenderTime + 1.0f / EngineApp.MaxFPS;

						if( needSleep )
						{
							Thread.Sleep( 1 );
						}
						else
						{
							maxFPSLastRenderTime = time;

							//finish switching to windowed mode
							if( goingToWindowedMode )
							{
								SetWindowBorderStyle( PlatformFunctionality.WindowBorderStyle.Sizeable );
								//!!!!было .VideoMode
								Vector2I pos = ( GetScreenSize() - EngineApp.FullscreenSize ) / 2;
								//!!!!было .VideoMode
								Vector2I size = EngineApp.FullscreenSize - new Vector2I( 1, 1 );
								SetWindowPos( EngineApp.ApplicationWindowHandle, HWND_NOTOPMOST, pos.X, pos.Y, size.X, size.Y, 0 );
								goingToWindowedMode = false;
								EngineApp.CreatedWindowProcessResize();
							}

							//finish switching to fullscreen mode
							if( goingToFullScreenMode )
							{
								bool topMost = !Debugger.IsAttached;
								SetWindowPos( EngineApp.ApplicationWindowHandle, topMost ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0,
									EngineApp.FullscreenSize.X, EngineApp.FullscreenSize.Y, 0 );
								goingToFullScreenMode = false;
								EngineApp.CreatedWindowProcessResize();
							}

							EngineApp.CreatedWindowApplicationIdle( false );
						}
					}
					else
						WaitMessage();
				}
			}

		}

		static bool IsAllowApplicationIdle()
		{
			bool needIdle = true;

			if( EngineApp.EnginePauseWhenApplicationIsNotActive ) //!!!!new
			{
				if( IsIconic( EngineApp.ApplicationWindowHandle ) )
					needIdle = false;
			}

			//!!!!!так?
			if( EngineApp.FullscreenEnabled && GetForegroundWindow() != EngineApp.ApplicationWindowHandle )
				needIdle = false;

			if( EngineApp.EnginePaused )
				needIdle = false;

			return needIdle;
		}

		public override bool IsIntoMenuLoop()
		{
			return intoMenuLoop;
		}

		public override void MessageLoopWaitMessage()
		{
			WaitMessage();
		}

		public override bool IsWindowInitialized()
		{
			//!!!!!что тут?
			return EngineApp.ApplicationWindowHandle != IntPtr.Zero;
		}

		public override void CreatedWindow_UpdateWindowTitle( string title )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
				SetWindowText( EngineApp.ApplicationWindowHandle, title );
		}

		static Assembly GetAssemblyByName( string name )
		{
			return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault( assembly => assembly.GetName().Name == name );
		}

		static Assembly winFormsAssembly;

		static Type GetIconType()
		{
			if( winFormsAssembly == null )
				winFormsAssembly = GetAssemblyByName( "System.Drawing.Common" );
			return winFormsAssembly.GetType( "System.Drawing.Icon" );
		}

		//save icon objects in memory. maybe no sense
		static object currentIcon;
		static object currentSmallIcon;

		[DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Auto )]
		static extern IntPtr LoadImage( IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad );

		const uint IMAGE_ICON = 1;

		const uint LR_LOADFROMFILE = 0x00000010;

		public override void CreatedWindow_UpdateWindowIcon( /*object smallIcon,*/ object icon, string iconFilePath )
		{
			//use icon object (System.Drawing.Icon)
			if( icon != null )
			{
				//make small icon
				object smallIcon = null;

				var smallIconSize = EngineApp.GetSystemSmallIconSize();

				try
				{
					//!!!!
					//temporary uses Winforms. can do resize by means native code

					var iconType = GetIconType();
					var constructor = iconType.GetConstructor( new Type[] { iconType, typeof( int ), typeof( int ) } );
					smallIcon = constructor.Invoke( new object[] { icon, smallIconSize.X, smallIconSize.Y } );

					//var smallIcon = new Icon( icon, new Size( smallIconSize.X, smallIconSize.Y ) );
				}
				catch { }

				if( smallIcon != null )
				{
					var handle = (IntPtr)ObjectEx.PropertyGet( smallIcon, "Handle" );
					SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_SMALL, handle );
					//SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_SMALL, smallIcon.Handle );
				}
				else
					SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_SMALL, IntPtr.Zero );

				if( icon != null )
				{
					var handle = (IntPtr)ObjectEx.PropertyGet( icon, "Handle" );
					SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_BIG, handle );
					//SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_BIG, icon.Handle );
				}
				else
					SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_BIG, IntPtr.Zero );

				currentIcon = icon;
				currentSmallIcon = smallIcon;
			}

			//use file path to icon
			if( !string.IsNullOrEmpty( iconFilePath ) )
			{
				try
				{
					{
						var smallIconSize = EngineApp.GetSystemSmallIconSize();

						var handle = LoadImage( IntPtr.Zero, iconFilePath, IMAGE_ICON, smallIconSize.X, smallIconSize.Y, LR_LOADFROMFILE );
						if( handle != IntPtr.Zero )
							SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_SMALL, handle );
					}

					{
						var handle = LoadImage( IntPtr.Zero, iconFilePath, IMAGE_ICON, 0, 0, LR_LOADFROMFILE );
						if( handle != IntPtr.Zero )
							SendMessage( EngineApp.ApplicationWindowHandle, WM_SETICON, (IntPtr)ICON_BIG, handle );
					}
				}
				catch { }
			}
		}

		public override RectangleI CreatedWindow_GetWindowRectangle()
		{
			RectangleI windowRect;
			GetWindowRect( EngineApp.ApplicationWindowHandle, out windowRect );
			return windowRect;
		}

		//!!!!?
		public override RectangleI CreatedWindow_GetClientRectangle()
		{
			RectangleI clientRect;
			GetClientRect( EngineApp.ApplicationWindowHandle, out clientRect );
			return clientRect;
		}

		//void SetWindowPosition( Vec2i position )
		//{
		//   SetWindowPos( App.ApplicationWindow.Handle, IntPtr.Zero, position.X, position.Y, 0, 0, SWP_NOSIZE );
		//}

		//!!!!!
		public override void CreatedWindow_SetWindowSize( Vector2I size )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
				SetWindowPos( EngineApp.ApplicationWindowHandle, IntPtr.Zero, 0, 0, size.X, size.Y, SWP_NOMOVE );
		}

		public override bool CreatedWindow_IsWindowActive()
		{
			if( GetForegroundWindow() != EngineApp.ApplicationWindowHandle )
				return false;
			if( GetWindowState() == WindowState.Minimized )
				return false;

			return true;
		}

		public override bool IsWindowVisible()
		{
			IntPtr h = EngineApp.ApplicationWindowHandle;
			while( h != IntPtr.Zero )
			{
				if( !_IsWindowVisible( h ) )
					return false;
				h = GetParent( h );
			}
			return true;
		}

		public override WindowState GetWindowState()
		{
			if( IsIconic( EngineApp.ApplicationWindowHandle ) )
				return WindowState.Minimized;
			if( IsZoomed( EngineApp.ApplicationWindowHandle ) )
				return WindowState.Maximized;
			return WindowState.Normal;
		}

		public override void SetWindowState( WindowState value )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
			{
				int show;
				if( value == WindowState.Maximized )
					show = SW_MAXIMIZE;//SW_SHOWMAXIMIZED;
				else if( value == WindowState.Minimized )
					show = SW_MINIMIZE;//before SW_SHOWMINIMIZED;
				else
					show = SW_RESTORE;//before SW_SHOWNORMAL;
				ShowWindow( EngineApp.ApplicationWindowHandle, show );
			}
		}

		void SetWindowBorderStyle( WindowBorderStyle value )
		{
			if( IntPtr.Size == 8 )
			{
				ulong style = (ulong)GetWindowLong( EngineApp.ApplicationWindowHandle, GWL_STYLE );

				if( value == WindowBorderStyle.None )
				{
					unchecked
					{
						style &= ~(ulong)WS_OVERLAPPEDWINDOW;
						style |= ( (ulong)WS_POPUP );
					}
				}
				else if( value == WindowBorderStyle.Sizeable )
				{
					unchecked
					{
						style &= ~(ulong)WS_POPUP;
						style |= ( (ulong)WS_OVERLAPPEDWINDOW );
					}
				}

				SetWindowLong( EngineApp.ApplicationWindowHandle, GWL_STYLE, (IntPtr)style );
			}
			else
			{
				uint style = (uint)GetWindowLong( EngineApp.ApplicationWindowHandle, GWL_STYLE );

				if( value == WindowBorderStyle.None )
				{
					unchecked
					{
						style &= ~(uint)WS_OVERLAPPEDWINDOW;
						style |= ( (uint)WS_POPUP );
					}
				}
				else if( value == WindowBorderStyle.Sizeable )
				{
					unchecked
					{
						style &= ~(uint)WS_POPUP;
						style |= ( (uint)WS_OVERLAPPEDWINDOW );
					}
				}

				SetWindowLong( EngineApp.ApplicationWindowHandle, GWL_STYLE, (IntPtr)(int)style );
			}
		}

		public override void SetWindowVisible( bool value )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
			{
				int show;
				if( value )
					show = SW_SHOW;
				else
					show = SW_HIDE;
				ShowWindow( EngineApp.ApplicationWindowHandle, show );
			}
		}

		//void SetWindowTopMost( bool value )
		//{
		//   SetWindowPos( App.WindowHandle, value ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0,
		//      0, 0, SWP_NOMOVE | SWP_NOSIZE );
		//}

		public override Vector2 CreatedWindow_GetMousePosition()
		{
			if( EngineApp.ApplicationWindowHandle == IntPtr.Zero )
				return Vector2F.Zero;

			Vector2I position;
			GetCursorPos( out position );
			ScreenToClient( EngineApp.ApplicationWindowHandle, ref position );

			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;
			return new Vector2F(
				(float)position.X / (float)( viewport.SizeInPixels.X - viewport.SizeInPixels.X % 2 ),
				(float)position.Y / (float)( viewport.SizeInPixels.Y - viewport.SizeInPixels.Y % 2 ) );
		}

		public override void CreatedWindow_SetMousePosition( Vector2 value )
		{
			if( EngineApp.ApplicationWindowHandle != IntPtr.Zero )
			{
				Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;
				Vector2I position = new Vector2I(
					(int)(float)( value.X * (float)viewport.SizeInPixels.X ),
					(int)(float)( value.Y * (float)viewport.SizeInPixels.Y ) );

				Vector2I globalPosition = position;
				ClientToScreen( EngineApp.ApplicationWindowHandle, ref globalPosition );
				SetCursorPos( globalPosition.X, globalPosition.Y );

				lastMousePositionForMouseMoveDelta = value;
				lastMousePositionForCheckMouseOutsideWindow = value;
			}
		}

		public override bool IsFocused()
		{
			if( EngineApp.ApplicationWindowHandle == IntPtr.Zero )
				return false;
			return GetFocus() == EngineApp.ApplicationWindowHandle;
		}

		public override bool ApplicationIsActivated()
		{
			var activatedHandle = GetForegroundWindow();
			if( activatedHandle == IntPtr.Zero )
				return false;// No window is currently activated

			var procId = Process.GetCurrentProcess().Id;
			int activeProcId;
			GetWindowThreadProcessId( activatedHandle, out activeProcId );

			if( activeProcId != procId )
				return false;

			if( GetWindowState() == WindowState.Minimized )
				return false;

			return true;
		}

		public override void CreatedWindow_UpdateShowSystemCursor( bool forceUpdate )
		{
			bool show = EngineApp.IsRealShowSystemCursor();

			if( createdWindow_UpdateShowSystemCursor != show || forceUpdate )
			{
				createdWindow_UpdateShowSystemCursor = show;

				if( show )
				{
					while( ShowCursor( 1 ) < 0 ) { }
				}
				else
				{
					while( ShowCursor( 0 ) >= 0 ) { }
				}
			}
		}

		public override IntPtr GetSystemCursorByFileName( string virtualFileName )
		{
			IntPtr hCursor = IntPtr.Zero;

			if( !string.IsNullOrEmpty( virtualFileName ) )
			{
				if( !loadedSystemCursors.TryGetValue( virtualFileName, out hCursor ) )
				{
					hCursor = IntPtr.Zero;

					string realFileName;
					if( Path.IsPathRooted( virtualFileName ) )
						realFileName = virtualFileName;
					else
						realFileName = VirtualPathUtility.GetRealPathByVirtual( virtualFileName );

					if( File.Exists( realFileName ) )
					{
						//load from real file system
						hCursor = LoadCursorFromFile( realFileName );
					}
					else
					{
						//load from virtual file system

						string tempRealFileName = VirtualPathUtility.GetRealPathByVirtual(
							string.Format( "user:_Temp_{0}", Path.GetFileName( virtualFileName ) ) );

						try
						{
							string directoryName = Path.GetDirectoryName( tempRealFileName );
							if( !Directory.Exists( directoryName ) )
								Directory.CreateDirectory( directoryName );

							byte[] data;

							using( VirtualFileStream stream = VirtualFile.Open( virtualFileName ) )
							{
								data = new byte[ stream.Length ];
								if( stream.Read( data, 0, (int)stream.Length ) != stream.Length )
									throw new Exception();
							}

							File.WriteAllBytes( tempRealFileName, data );

							hCursor = LoadCursorFromFile( tempRealFileName );

							File.Delete( tempRealFileName );
						}
						catch { }
					}

					loadedSystemCursors.Add( virtualFileName, hCursor );
				}
			}

			return hCursor;
		}

		public override void CreatedWindow_UpdateSystemCursorFileName()
		{
			var hCursor = GetSystemCursorByFileName( EngineApp.SystemCursorFileName );
			if( hCursor == IntPtr.Zero )
				hCursor = hCursorArrow;

			SetCursor( hCursor );
		}

		public unsafe override bool InitDirectInputMouseDevice()
		{
			IDirectInput* alreadyCreatedDirectInput = null;
			{
				WindowsInputDeviceManager windowsInputDeviceManager = InputDeviceManager.Instance as WindowsInputDeviceManager;
				if( windowsInputDeviceManager != null )
					alreadyCreatedDirectInput = windowsInputDeviceManager.DirectInput;
			}

			if( !DirectInputMouseDevice.Init( EngineApp.ApplicationWindowHandle, alreadyCreatedDirectInput ) )
				return false;

			return true;
		}

		public override void ShutdownDirectInputMouseDevice()
		{
			DirectInputMouseDevice.Shutdown();
		}

		public unsafe override void CreatedWindow_UpdateInputDevices()
		{
			//!!!!!всё тут проверить

			//!!!!//ничего не обновлять, если отрублены?

			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;

			//!!!!new
			if( EngineApp.CreatedInsideEngineWindow != null && IsFocused() )
				CreatedWindow_UpdateShowSystemCursor( false );

			//mouse buttons
			{
				//!!!!

				//List<ValueTuple<EMouseButtons, int>> buttons = new List<(EMouseButtons, int)>();
				//buttons.Add( (EMouseButtons.Left, VK_LBUTTON) );
				//buttons.Add( (EMouseButtons.Right, VK_RBUTTON) );
				//buttons.Add( (EMouseButtons.Middle, VK_MBUTTON) );
				//buttons.Add( (EMouseButtons.XButton1, VK_XBUTTON1) );
				//buttons.Add( (EMouseButtons.XButton2, VK_XBUTTON2) );
				//foreach( var tuple in buttons )
				//{
				//	if( viewport.IsMouseButtonPressed( tuple.Item1 ) && GetKeyState( tuple.Item2 ) >= 0 )
				//	{
				//		bool handled = false;
				//		viewport.PerformMouseUp( tuple.Item1, ref handled );
				//	}
				//}


				if( viewport.IsMouseButtonPressed( EMouseButtons.Left ) && GetKeyState( VK_LBUTTON ) >= 0 )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.Left, ref handled );
				}
				if( viewport.IsMouseButtonPressed( EMouseButtons.Right ) && GetKeyState( VK_RBUTTON ) >= 0 )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.Right, ref handled );
				}
				if( viewport.IsMouseButtonPressed( EMouseButtons.Middle ) && GetKeyState( VK_MBUTTON ) >= 0 )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.Middle, ref handled );
				}
				if( viewport.IsMouseButtonPressed( EMouseButtons.XButton1 ) && GetKeyState( VK_XBUTTON1 ) >= 0 )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.XButton1, ref handled );
				}
				if( viewport.IsMouseButtonPressed( EMouseButtons.XButton2 ) && GetKeyState( VK_XBUTTON2 ) >= 0 )
				{
					bool handled = false;
					viewport.PerformMouseUp( EMouseButtons.XButton2, ref handled );
				}
			}

			//keys
			foreach( EKeys eKey in Viewport.AllKeys )
			{
				if( viewport.IsKeyPressed( eKey ) )
				{
					KeyInfo keyInfo = keysInfo[ (int)eKey ];

					if( keyInfo.keyCode != 0 )
					{
						if( GetKeyState( keyInfo.keyCode ) >= 0 )
						{
							KeyEvent keyEvent = new KeyEvent( eKey );
							bool handled = false;
							viewport.PerformKeyUp( keyEvent, ref handled );
						}
					}
				}
			}

			//mouse outside window client rectangle
			if( !viewport.MouseRelativeMode )
			{
				Vector2 mouse = viewport.MousePosition;

				if( mouse.X < 0 || mouse.X >= 1 || mouse.Y < 0 || mouse.Y >= 1 )
				{
					if( !mouse.Equals( lastMousePositionForCheckMouseOutsideWindow, .0001f ) )
					{
						lastMousePositionForCheckMouseOutsideWindow = mouse;
						EngineApp.CreatedInsideEngineWindow.ProcessMouseMoveEvent();
					}
				}
			}

			//mouse relative mode
			if( viewport.MouseRelativeMode )
			{
				//!!!!в EngineViewportControl тоже?
				//clip cursor by window rectangle
				if( IsFocused() )
				{
					SetCapture( EngineApp.ApplicationWindowHandle );

					RectangleI rectangle = CreatedWindow_GetWindowRectangle();
					rectangle.Left += 1;
					rectangle.Top += 1;
					rectangle.Right -= 1;
					rectangle.Bottom -= 1;
					ClipCursor( (IntPtr)( &rectangle ) );
				}

				if( DirectInputMouseDevice.Instance != null )
				{
					DirectInputMouseDevice.State state = DirectInputMouseDevice.Instance.GetState();
					if( state.Position.X != 0 || state.Position.Y != 0 )
					{
						Vector2F offset = new Vector2F(
							(float)(int)state.Position.X / viewport.SizeInPixels.X,
							(float)(int)state.Position.Y / viewport.SizeInPixels.Y );

						viewport.PerformMouseMove( offset );

						if( !EngineApp.Closing && IsFocused() )
							CreatedWindow_SetMousePosition( new Vector2F( .5f, .5f ) );
						//App.MousePosition = new Vec2( .5f, .5f );
					}
				}
				else
				{
					//!!!!надо ли

					if( !EngineApp.Closing && IsFocused() )
						CreatedWindow_SetMousePosition( new Vector2F( .5f, .5f ) );
					//App.MousePosition = new Vec2( .5f, .5f );
				}
			}
		}

		public override bool IsKeyLocked( EKeys key )
		{
			int keyState = GetKeyState( (int)key );
			if( key != EKeys.Insert && key != EKeys.Capital )
				return ( keyState & 0x8001 ) != 0;
			return ( keyState & 1 ) != 0;
		}

		public override string[] GetNativeModuleNames()
		{
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
		}

		public override List<Vector2I> GetVideoModes()
		{
			List<Vector2I> videoModes = new List<Vector2I>();

			uint mask = DM_BITSPERPEL | DM_PELSWIDTH | DM_PELSHEIGHT;
			DEVMODE deviceMode;

			int bpp = GetScreenBitsPerPixel();

			for( int n = 0; EnumDisplaySettings( IntPtr.Zero, n, out deviceMode ) != 0; n++ )
			{
				if( ( deviceMode.dmFields & mask ) == mask )
				{
					if( ChangeDisplaySettings( ref deviceMode, CDS_TEST | CDS_FULLSCREEN ) == DISP_CHANGE_SUCCESSFUL )
					{
						if( deviceMode.dmBitsPerPel != bpp )
							continue;

						Vector2I mode = new Vector2I( deviceMode.dmPelsWidth, deviceMode.dmPelsHeight );

						if( !videoModes.Contains( mode ) )
							videoModes.Add( mode );
					}
				}
			}

			return videoModes;
		}

		int GetMaximumFrequencyByVideoMode( Vector2I mode )
		{
			int maxFrequency = 0;

			uint mask = DM_BITSPERPEL | DM_PELSWIDTH | DM_PELSHEIGHT | DM_DISPLAYFREQUENCY;
			DEVMODE deviceMode;

			int bpp = GetScreenBitsPerPixel();

			for( int n = 0; EnumDisplaySettings( IntPtr.Zero, n, out deviceMode ) != 0; n++ )
			{
				if( ( deviceMode.dmFields & mask ) == mask )
				{
					if( ChangeDisplaySettings( ref deviceMode, CDS_TEST | CDS_FULLSCREEN ) == DISP_CHANGE_SUCCESSFUL )
					{
						if( mode.X == deviceMode.dmPelsWidth &&
							mode.Y == deviceMode.dmPelsHeight &&
							bpp == deviceMode.dmBitsPerPel )
						{
							if( deviceMode.dmDisplayFrequency > maxFrequency )
								maxFrequency = deviceMode.dmDisplayFrequency;
						}
					}
				}
			}

			return maxFrequency;
		}

		public override bool ChangeVideoMode( Vector2I mode )
		{
			int frequency = 0;
			//on some drivers a bug in ChangeDisplaySettings(CDC_TEST) 
			//if( EngineApp.InitSettings.CreateWindowFullscreenAllowChangeDisplayFrequency )
			frequency = GetMaximumFrequencyByVideoMode( mode );

			DEVMODE deviceMode = new DEVMODE();
			deviceMode.dmSize = (short)Marshal.SizeOf( typeof( DEVMODE ) );
			deviceMode.dmPelsWidth = mode.X;
			deviceMode.dmPelsHeight = mode.Y;
			deviceMode.dmBitsPerPel = (short)GetScreenBitsPerPixel();
			deviceMode.dmFields = DM_PELSWIDTH | DM_PELSHEIGHT | DM_BITSPERPEL;
			if( frequency != 0 )
			{
				deviceMode.dmDisplayFrequency = frequency;
				deviceMode.dmFields |= DM_DISPLAYFREQUENCY;
			}

			if( ChangeDisplaySettings( ref deviceMode, CDS_FULLSCREEN ) != DISP_CHANGE_SUCCESSFUL )
				return false;

			return true;
		}

		public override void RestoreVideoMode()
		{
			ChangeDisplaySettings( IntPtr.Zero, 0 );
		}

		public override void SetGamma( float value )
		{
			RAMP gamma;
			IntPtr hWnd = GetDesktopWindow();
			IntPtr hdc = GetDC( hWnd );
			if( GetDeviceGammaRamp( hdc, out gamma ) != 0 )
			{
				for( int n = 0; n < 256; n++ )
				{
					ushort g = (ushort)( 255 * (ushort)( 255.0f * MathEx.Pow( (float)n / 255.0f, 1.0f / value ) ) );
					gamma.red[ n ] = g;
					gamma.green[ n ] = g;
					gamma.blue[ n ] = g;
				}
				if( SetDeviceGammaRamp( hdc, ref gamma ) == 0 )
				{
					//Error
					//ReleaseDC(hWnd, hdc);
					//return;
				}
			}

			ReleaseDC( hWnd, hdc );
		}

		public override bool ShowMessageBoxYesNoQuestion( string text, string caption )
		{
			int result = MessageBox( EngineApp.ApplicationWindowHandle, text, caption, MB_YESNO | MB_ICONEXCLAMATION );
			if( result == IDYES )
				return true;
			return false;
		}

		public override void CreatedWindow_OnMouseRelativeModeChange()
		{
			Viewport viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];//App.CreatedInsideEngineWindow.Viewport;

			if( viewport.MouseRelativeMode )
			{
				if( DirectInputMouseDevice.Instance != null )
					DirectInputMouseDevice.Instance.GetState();
			}
			else
			{
				ReleaseCapture();
				ClipCursor( IntPtr.Zero );
			}

			mustIgnoreOneMouseMoveAtRelativeMode = true;
		}

		public override void CreatedWindow_UpdateMouseRelativeMove( out Vector2 delta )
		{
			if( !mustIgnoreOneMouseMoveAtRelativeMode )
			{
				delta = CreatedWindow_GetMousePosition() - lastMousePositionForMouseMoveDelta;
			}
			else
			{
				mustIgnoreOneMouseMoveAtRelativeMode = false;
				delta = Vector2F.Zero;
			}
		}

		public override IntPtr CallPlatformSpecificMethod( string message, IntPtr param )
		{
			return IntPtr.Zero;
		}

		public override void ProcessChangingVideoMode()
		{
			if( EngineApp.FullscreenEnabled )
			{
				goingToFullScreenMode = true;

				//minimize window
				SetWindowState( WindowState.Minimized );

				//!!!!!так?
				//change video mode
				if( !SystemSettings.ChangeVideoMode( EngineApp.FullscreenSize ) )
					return;
				//было
				//App.lastFullScreenWindowSize = App.FullScreenSize;

				//update window
				bool topMost = !Debugger.IsAttached;
				SetWindowBorderStyle( WindowBorderStyle.None );
				SetWindowState( WindowState.Normal );
				SetWindowPos( EngineApp.ApplicationWindowHandle, topMost ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, EngineApp.FullscreenSize.X, EngineApp.FullscreenSize.Y, 0 );
			}
			else
			{
				goingToWindowedMode = true;

				//!!!!!так?
				//change video mode
				SystemSettings.RestoreVideoMode();
			}
		}

		unsafe static bool MonitorEnumProc( IntPtr hMonitor, IntPtr hdcMonitor, ref RectangleI lprcMonitor, IntPtr dwData )
		{
			MONITORINFOEX info = new MONITORINFOEX();
			info.cbSize = sizeof( MONITORINFOEX );
			if( GetMonitorInfo( hMonitor, ref info ) )
			{
				SystemSettings.DisplayInfo displayInfo = new SystemSettings.DisplayInfo( new string( info.szDeviceName ), info.rcMonitor,
					info.rcWork, ( info.dwFlags & MONITORINFOF_PRIMARY ) != 0 );
				tempScreenList.Add( displayInfo );
			}

			return true;
		}

		public override IList<SystemSettings.DisplayInfo> GetAllDisplays()
		{
			List<SystemSettings.DisplayInfo> result = new List<SystemSettings.DisplayInfo>();

			lock( tempScreenList )
			{
				IntPtr hdc = GetDC( IntPtr.Zero );
				if( hdc != IntPtr.Zero )
				{
					tempScreenList.Clear();

					EnumDisplayMonitors( hdc, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero );
					ReleaseDC( IntPtr.Zero, hdc );

					result.AddRange( tempScreenList );
					tempScreenList.Clear();
				}
			}

			if( result.Count == 0 )
			{
				RectangleI area = new RectangleI( Vector2I.Zero, GetScreenSize() );
				SystemSettings.DisplayInfo info = new SystemSettings.DisplayInfo( "Primary", area, area, true );
				result.Add( info );
			}

			return result;
		}

		public override void CreatedWindow_SetWindowRectangle( RectangleI rectangle )
		{
			goingToChangeWindowRectangle = true;
			if( GetWindowState() == WindowState.Maximized )
				SetWindowState( WindowState.Normal );
			SetWindowPos( EngineApp.ApplicationWindowHandle, IntPtr.Zero, rectangle.Left, rectangle.Top, rectangle.Size.X, rectangle.Size.Y, 0 );
			goingToChangeWindowRectangle = false;

			//!!!!!так?
			EngineApp.CreatedWindowProcessResize();
		}

		public override void GetSystemLanguage( out string name, out string englishName )
		{
			name = CultureInfo.CurrentUICulture.Name;
			englishName = CultureInfo.CurrentUICulture.EnglishName;
		}

		public void SetDarkMode( IntPtr handle, bool enable )
		{
			try
			{
				int value = enable ? 1 : 0;
				DwmSetWindowAttribute( handle, DwmWindowAttribute.UseImmersiveDarkMode, ref value, 4 );
			}
			catch { }
		}
	}
}
#endif