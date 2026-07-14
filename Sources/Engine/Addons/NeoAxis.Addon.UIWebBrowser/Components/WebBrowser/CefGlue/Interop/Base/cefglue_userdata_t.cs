#if !NO_UI_WEB_BROWSER
namespace Internal.Xilium.CefGlue.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe partial struct cefglue_userdata_t
    {
        internal cef_base_t _base;
    }
}

#endif