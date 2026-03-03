#if !NO_UI_WEB_BROWSER
//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_geoposition_error_code_t.
//
namespace Internal.Xilium.CefGlue
{
    /// <summary>
    /// Geoposition error codes.
    /// </summary>
    public enum CefGeopositionErrorCode
    {
        None = 0,
        PermissionDenied,
        PositionUnavailable,
        Timeout,
    }
}

#endif