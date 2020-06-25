using System;

namespace SharpBgfx {
    /// <summary>
    /// Contains platform-specific data used to hook into the bgfx library.
    /// </summary>
    public struct PlatformData {
        /// <summary>
        /// EGL native display type.
        /// </summary>
        public IntPtr DisplayType;

        /// <summary>
        /// Platform window handle.
        /// </summary>
        public IntPtr WindowHandle;

        /// <summary>
        /// Device context to use instead of letting the library create its own.
        /// </summary>
        public IntPtr Context;

        /// <summary>
        /// Backbuffer pointer to use instead of letting the library create its own.
        /// </summary>
        public IntPtr Backbuffer;

        /// <summary>
        /// Depth-stencil pointer to use instead of letting the library create its own.
        /// </summary>
        public IntPtr BackbufferDepthStencil;
    }

    /// <summary>
    /// Exposes internal API data for interop scenarios.
    /// </summary>
    public struct InternalData {
        /// <summary>
        /// Pointer to internal Bgfx capabilities structure. Use <see cref="Bgfx.GetCaps"/> instead.
        /// </summary>
        public IntPtr Caps;

        /// <summary>
        /// The underlying API's device context (OpenGL, Direct3D, etc).
        /// </summary>
        public IntPtr Context;
    }
}
