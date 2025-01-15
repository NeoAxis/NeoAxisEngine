#if !NO_UI_WEB_BROWSER
//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_return_value_t.
//
namespace Internal.Xilium.CefGlue
{
    /// <summary>
    /// Return value types.
    /// </summary>
    public enum CefReturnValue
    {
        /// <summary>
        /// Cancel immediately.
        /// </summary>
        Cancel = 0,

        /// <summary>
        /// Continue immediately.
        /// </summary>
        Continue,

        /// <summary>
        /// Continue asynchronously (usually via a callback).
        /// </summary>
        ContinueAsync,
    }
}

#endif