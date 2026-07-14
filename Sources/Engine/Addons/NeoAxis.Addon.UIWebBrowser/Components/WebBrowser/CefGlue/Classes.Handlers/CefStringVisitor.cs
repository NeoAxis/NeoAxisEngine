#if !NO_UI_WEB_BROWSER
namespace Internal.Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Internal.Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to receive string values asynchronously.
    /// </summary>
    public abstract unsafe partial class CefStringVisitor
    {
        private void visit(cef_string_visitor_t* self, cef_string_t* @string)
        {
            CheckSelf(self);

            Visit(cef_string_t.ToString(@string));
        }

        /// <summary>
        /// Method that will be executed.
        /// </summary>
        protected abstract void Visit(string value);

    }
}

#endif