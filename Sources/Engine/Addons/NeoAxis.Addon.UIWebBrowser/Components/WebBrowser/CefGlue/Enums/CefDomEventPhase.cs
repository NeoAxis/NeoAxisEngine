#if !NO_UI_WEB_BROWSER
//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_dom_event_phase_t.
//
namespace Internal.Xilium.CefGlue
{
    using System;

    /// <summary>
    /// DOM event processing phases.
    /// </summary>
    public enum CefDomEventPhase
    {
        Unknown = 0,
        Capturing,
        AtTarget,
        Bubbling,
    }
}

#endif