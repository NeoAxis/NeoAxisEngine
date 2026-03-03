#if !NO_UI_WEB_BROWSER
namespace Internal.Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Internal.Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to receive geolocation updates. The methods of this
    /// class will be called on the browser process UI thread.
    /// </summary>
    public abstract unsafe partial class CefGetGeolocationCallback
    {
        private void on_location_update(cef_get_geolocation_callback_t* self, cef_geoposition_t* position)
        {
            CheckSelf(self);

            var mPosition = CefGeoposition.FromNative(position);

            OnLocationUpdate(mPosition);
        }
        
        /// <summary>
        /// Called with the 'best available' location information or, if the location
        /// update failed, with error information.
        /// </summary>
        protected abstract void OnLocationUpdate(CefGeoposition position);
    }
}

#endif