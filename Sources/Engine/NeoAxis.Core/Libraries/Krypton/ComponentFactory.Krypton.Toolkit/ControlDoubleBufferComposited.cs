// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace ComponentFactory.Krypton.Toolkit
{
    public static class ControlDoubleBufferComposited
    {
        public interface IDoubleBufferComposited
        {
        }

        //

        static void Update( Control control, bool enable )
        {
            if( control.Parent != null )
                Update( control.Parent, enable );

            if( control is IDoubleBufferComposited )
            {
                int style = PI.GetWindowLong( control.Handle, PI.GWL_EXSTYLE );

                int newStyle;
                if( enable )
                    newStyle = style | PI.WS_EX_COMPOSITED;
                else
                    newStyle = style & ~PI.WS_EX_COMPOSITED;

                if( newStyle != style )
                    PI.SetWindowLong( control.Handle, PI.GWL_EXSTYLE, (IntPtr)newStyle );

                //if( enable )
                //    PI.SetWindowLong( control.Handle, PI.GWL_EXSTYLE, (IntPtr)( style | PI.WS_EX_COMPOSITED ) );
                //else
                //    PI.SetWindowLong( control.Handle, PI.GWL_EXSTYLE, (IntPtr)( style & ~PI.WS_EX_COMPOSITED ) );
            }
        }

        public static void DisableComposited( Control control )
        {
            Update( control, false );
        }

        public static void RestoreComposited( Control control )
        {
            Update( control, true );
        }

        ////this methods must be called when one of parent was changed. to update window style
        //public static void UpdateComposited( Control control )
        //{

        //    Update( control, true );

        //}

    }
}
