using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace SharpBgfx.Common {
    class Window {
        ConcurrentQueue<WindowEvent> queue = new ConcurrentQueue<WindowEvent>();
        WNDPROC wndproc;
        IntPtr hwnd;

        public IntPtr Handle => hwnd;

        public Window(string title, int width, int height) {
            var instance = Kernel32.GetModuleHandleW(null);
            var icon = User32.LoadIconW(IntPtr.Zero, IDI.APPLICATION);
            wndproc = WndProc;

            // register the window class
            var wndclass = new WNDCLASSEXW {
                cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
                style = CS.HREDRAW | CS.VREDRAW | CS.OWNDC,
                hInstance = instance,
                hIcon = icon,
                hIconSm = icon,
                hCursor = User32.LoadCursorW(IntPtr.Zero, IDC.ARROW),
                lpszClassName = ClassName,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndproc)
            };

            var atom = User32.RegisterClassExW(ref wndclass);
            if (atom == 0)
                throw new InvalidOperationException("Failed to register window class");

            // size the client area appropriately
            var styles = WS.OVERLAPPEDWINDOW;
            var rect = new RECT { left = 0, top = 0, right = width, bottom = height };
            if (User32.AdjustWindowRectEx(ref rect, styles, 0, 0)) {
                width = rect.right - rect.left;
                height = rect.bottom - rect.top;
            }

            // create the window
            hwnd = User32.CreateWindowExW(
                0,
                new IntPtr(atom),
                title,
                styles,
                CW.USEDEFAULT,
                CW.USEDEFAULT,
                width,
                height,
                IntPtr.Zero,
                IntPtr.Zero,
                instance,
                IntPtr.Zero
            );
            if (hwnd == IntPtr.Zero)
                throw new InvalidOperationException("Failed to create window");
        }

        public void Show() {
            User32.ShowWindow(hwnd, SW.SHOWDEFAULT);
            User32.UpdateWindow(hwnd);
        }

        public WindowEvent? Poll() {
            WindowEvent ev;
            if (queue.TryDequeue(out ev))
                return ev;

            return null;
        }

        public int RunMessageLoop() {
            MSG msg;
            while (User32.GetMessage(out msg, IntPtr.Zero, 0, 0) != 0) {
                User32.TranslateMessage(ref msg);
                User32.DispatchMessage(ref msg);
            }

            return TOINT32(msg.wParam);
        }

        public void Close() {
            User32.PostMessage(hwnd, WM.GAME_DONE, IntPtr.Zero, IntPtr.Zero);
        }

        unsafe IntPtr WndProc(IntPtr hwnd, WM msg, IntPtr wparam, IntPtr lparam) {
            switch (msg) {
                case WM.ERASEBKGND:
                    // yep, we handled it
                    return TRUE;

                case WM.PAINT:
                    User32.ValidateRect(hwnd, IntPtr.Zero);
                    return IntPtr.Zero;

                case WM.SIZE:
                    queue.Enqueue(new WindowEvent(lparam));
                    return IntPtr.Zero;
                    
                case WM.CLOSE:
                    // instead of closing the window now, post an exit message
                    // the game thread will finish up and send WM_GAME_DONE
                    queue.Enqueue(new WindowEvent(WindowEventType.Exit));
                    return IntPtr.Zero;

                case WM.GAME_DONE:
                    User32.DestroyWindow(hwnd);
                    return IntPtr.Zero;

                case WM.DESTROY:
                    User32.PostQuitMessage(0);
                    return IntPtr.Zero;
            }

            return User32.DefWindowProcW(hwnd, msg, wparam, lparam);
        }
        
        static int TOINT32(IntPtr param) => unchecked((int)(long)param);
        internal static int LOWORD(IntPtr param) => unchecked((short)(long)param);
        internal static int HIWORD(IntPtr param) => unchecked((short)((long)param >> 16));
        
        const string ClassName = "GameWindow";

        static readonly IntPtr TRUE = new IntPtr(1);
    }
    
    struct WindowEvent {
        public readonly WindowEventType Type;
        public readonly int Width;
        public readonly int Height;

        public WindowEvent(WindowEventType type) : this() {
            Type = type;
        }

        public WindowEvent(IntPtr lparam) : this(WindowEventType.Size) {
            Width = Window.LOWORD(lparam);
            Height = Window.HIWORD(lparam);
        }
    }

    enum WindowEventType {
        Exit,
        Size
    }
}
