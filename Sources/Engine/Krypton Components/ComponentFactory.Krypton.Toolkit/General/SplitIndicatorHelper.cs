using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace ComponentFactory.Krypton.Toolkit
{
    internal class SplitIndicatorHelper
    {
        [ThreadStatic]
        static SplitIndicatorHelper _instance;

        public static SplitIndicatorHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SplitIndicatorHelper();
                return _instance;
            }
        }

        public void DrawReversibleSplitter(IntPtr handle, Rectangle bounds)
        {
            DrawReversibleObject(handle, bounds, CreateSplitBrush());
        }

        public void DrawReversibleLine(IntPtr handle, Rectangle bounds)
        {
            DrawReversibleObject(handle, bounds, CreateLineBrush());
        }

        public void DrawReversibleLine(IntPtr handle, Point startPoint, Point endPoint)
        {
            Rectangle bounds;
            if (startPoint.Y == endPoint.Y)
                bounds = new Rectangle(startPoint.X, startPoint.Y, endPoint.X - startPoint.X, 2);
            else
                bounds = new Rectangle(startPoint.X, startPoint.Y, 2, endPoint.Y - startPoint.Y);
            DrawReversibleObject(handle, bounds, CreateLineBrush());
        }

        public void DrawReversibleFrame(IntPtr handle, Rectangle bounds)
        {
            DrawReversibleFrame(handle, bounds, CreateLineBrush());
        }

        protected void DrawReversibleObject(IntPtr handle, Rectangle bounds, IntPtr splitBrush)
        {
            Rectangle r = bounds;
            IntPtr dc = GetDCEx(handle, IntPtr.Zero, DCX_CACHE | DCX_LOCKWINDOWUPDATE);
            IntPtr saveBrush = SelectObject(dc, splitBrush);
            PatBlt(dc, r.X, r.Y, r.Width, r.Height, PATINVERT);
            SelectObject(dc, saveBrush);
            DeleteObject(splitBrush);
            ReleaseDC(handle, dc);
        }

        const int frameWidth = 2;

        protected void DrawReversibleFrame(IntPtr handle, Rectangle bounds, IntPtr splitBrush)
        {
            Rectangle r = bounds;
            IntPtr dc = GetDCEx(handle, IntPtr.Zero, DCX_CACHE | DCX_LOCKWINDOWUPDATE);
            IntPtr saveBrush = SelectObject(dc, splitBrush);
            PatBlt(dc, r.X, r.Y, frameWidth, r.Height, PATINVERT);
            PatBlt(dc, r.X + frameWidth, r.Y, r.Width - frameWidth * 2, frameWidth, PATINVERT);
            PatBlt(dc, r.X + frameWidth, r.Bottom - frameWidth, r.Width - frameWidth * 2, frameWidth, PATINVERT);
            PatBlt(dc, r.Right - frameWidth, r.Y, frameWidth, r.Height, PATINVERT);
            SelectObject(dc, saveBrush);
            DeleteObject(splitBrush);
            ReleaseDC(handle, dc);
        }

        #region API

        IntPtr CreateLineBrush()
        {
            short[] grayPattern = new short[8];
            for (int i = 0; i < 2; i++)
                grayPattern[i] = 0xff;
            IntPtr hBitmap = CreateBitmap(2, 2, 1, 1, grayPattern);
            LOGBRUSH lb = new LOGBRUSH(BS_PATTERN, 0, hBitmap);
            IntPtr brush = CreateBrushIndirect(lb);
            DeleteObject(hBitmap);
            return brush;
        }

        IntPtr CreateSplitBrush()
        {
            short[] grayPattern = new short[8];
            for (int i = 0; i < 8; i++)
                grayPattern[i] = (short)(0x5555 << (i & 1));
            IntPtr hBitmap = CreateBitmap(8, 8, 1, 1, grayPattern);
            LOGBRUSH lb = new LOGBRUSH(BS_PATTERN, 0, hBitmap);
            IntPtr brush = CreateBrushIndirect(lb);
            DeleteObject(hBitmap);
            return brush;
        }

        [StructLayout(LayoutKind.Sequential)]
        class LOGBRUSH
        {
            public LOGBRUSH(int style, int color, IntPtr hatch)
            {
                this.Style = style;
                this.Color = color;
                this.Hatch = hatch;
            }
            public int Style;
            public int Color;
            public IntPtr Hatch;
        }

        const int DCX_WINDOW = 0x00000001;
        const int DCX_CACHE = 0x00000002;
        const int BS_PATTERN = 3;
        const int PATINVERT = 0x005A0049;
        const int DCX_LOCKWINDOWUPDATE = 0x00000400;

        [DllImport("GDI32.dll", CharSet = CharSet.Unicode)]
        static extern bool PatBlt(IntPtr hdc, int left, int top, int width, int height, int rop);

        [DllImport("GDI32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr CreateBrushIndirect(LOGBRUSH lb);

        [DllImport("GDI32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr CreateBitmap(int width, int height, int planes, int bitsPerPixel, short[] lpvBits);

        [DllImport("GDI32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        [DllImport("GDI32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr GetDCEx(IntPtr hwnd, IntPtr clip, int flags);
        #endregion
    }
}
