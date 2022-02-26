// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
    public static class KryptonCursors
    {
        [DllImport( "user32.dll" )]
        static extern IntPtr LoadCursor( IntPtr hInstance, int lpCursorName );

        /////////////////////////////////////////

        static Cursor InitCursor( int id, ref Cursor field )
        {
            if( field == null )
            {
                var handle = LoadCursor( IntPtr.Zero, id );
                if( handle != IntPtr.Zero )
                    field = new Cursor( handle );
            }
            return field;
        }

        public static Cursor Default
        {
            get { return Cursors.Default; }
        }

        const int IDC_HAND = 32649;
        static Cursor hand;
        public static Cursor Hand
        {
            get { return InitCursor( IDC_HAND, ref hand ); }
        }

        const int IDC_SIZEWE = 32644;
        static Cursor sizeWE;
        public static Cursor SizeWE
        {
            get { return InitCursor( IDC_SIZEWE, ref sizeWE ); }
        }
        public static Cursor VSplit
        {
            get { return SizeWE; }
        }

        const int IDC_SIZENS = 32645;
        static Cursor sizeNS;
        public static Cursor SizeNS
        {
            get { return InitCursor( IDC_SIZENS, ref sizeNS ); }
        }
        public static Cursor HSplit
        {
            get { return SizeNS; }
        }

        const int IDC_SIZEALL = 32646;
        static Cursor sizeAll;
        public static Cursor SizeAll
        {
            get { return InitCursor( IDC_SIZEALL, ref sizeAll ); }
        }

        const int IDC_SIZENESW = 32643;
        static Cursor sizeNESW;
        public static Cursor SizeNESW
        {
            get { return InitCursor( IDC_SIZENESW, ref sizeNESW ); }
        }

        const int IDC_SIZENWSE = 32642;
        static Cursor sizeNWSE;
        public static Cursor SizeNWSE
        {
            get { return InitCursor( IDC_SIZENWSE, ref sizeNWSE ); }
        }
    }
}
