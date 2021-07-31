// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Text;
using System.ComponentModel;

namespace ComponentFactory.Krypton.Toolkit
{
    public static class KryptonWinFormsUtility
    {
        ////!!!!может еще что-то вроверять там где вызывается
        //public static bool IsControlVisibleInHierarchy( Control control )
        //{
        //	if( !control.Visible )
        //		return false;
        //	if( control.Parent != null )
        //		return IsControlVisibleInHierarchy( control.Parent );
        //	else
        //		return true;
        //}

        //public static bool IsDesignerHosted( Control control )
        //{
        //	var eUserControl = control as EUserControl;
        //	if( eUserControl != null )
        //		return eUserControl.IsDesignerHosted;

        //	if( LicenseManager.UsageMode == LicenseUsageMode.Designtime )
        //		return true;
        //	Control ctrl = control;
        //	while( ctrl != null )
        //	{
        //		if( ( ctrl.Site != null ) && ctrl.Site.DesignMode )
        //			return true;
        //		ctrl = ctrl.Parent;
        //	}
        //	return false;
        //}

        //[DllImport( "user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Unicode )]
        //static extern IntPtr GetWindowLong( IntPtr hWnd, int nIndex );

        //public static void InvalidateParentComposedStyleControl( Control control )
        //{
        //	Control found = null;

        //	Control p = control;
        //	while( p != null )
        //	{
        //		ulong style = (ulong)GetWindowLong( p.Handle, -20 );//*GWL_EXSTYLE
        //		if( ( style & 0x02000000 ) != 0 )//WS_EX_COMPOSITED
        //		{
        //			found = p;
        //			break;
        //		}
        //		p = p.Parent;
        //	}

        //	found?.Invalidate();
        //}

        /////////////////////////////////////////

        [DllImport( "user32.dll" )]
        static extern bool LockWindowUpdate( IntPtr hWndLock );

        public static void LockFormUpdate( Control form )
        {
            if( form != null )
                LockWindowUpdate( form.Handle );
            else
                LockWindowUpdate( IntPtr.Zero );
        }

        /////////////////////////////////////////

        public static Action editorFormStartTemporaryLockUpdateAction;

        public static void EditorFormStartTemporaryLockUpdate()
        {
            editorFormStartTemporaryLockUpdateAction?.Invoke();
        }

    }
}
