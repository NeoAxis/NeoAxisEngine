#if !NO_UI_WEB_BROWSER
namespace Internal.Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Internal.Xilium.CefGlue.Interop;

    /// <summary>
    /// Generic callback interface used for asynchronous continuation.
    /// </summary>
    public sealed unsafe partial class CefCallback
    {
        /// <summary>
        /// Continue processing.
        /// </summary>
        public void Continue()
        {
            cef_callback_t.cont(_self);
        }

        /// <summary>
        /// Cancel processing.
        /// </summary>
        public void Cancel()
        {
            cef_callback_t.cancel(_self);
        }
    }
}

#endif