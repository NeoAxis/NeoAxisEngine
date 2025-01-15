#if !NO_UI_WEB_BROWSER
namespace Internal.Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class CefVersionMismatchException : CefRuntimeException
    {
        public CefVersionMismatchException(string message)
            : base(message)
        {
        }
    }
}

#endif