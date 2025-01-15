#if !NO_UI_WEB_BROWSER
//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_text_style_t.
//
namespace Internal.Xilium.CefGlue
{
    using System;

    /// <summary>
    /// Text style types. Should be kepy in sync with gfx::TextStyle.
    /// </summary>
    public enum CefTextStyle
    {
        Bold,
        Italic,
        Strike,
        DiagonalStrike,
        Underline,
    }
}

#endif