using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpBgfx {
    /// <summary>
    /// Provides an interface for programs to respond to callbacks from the bgfx library.
    /// </summary>
    public interface ICallbackHandler {
        /// <summary>
        /// Called when an error occurs in the library.
        /// </summary>
        /// <param name="fileName">The name of the source file in which the message originated.</param>
        /// <param name="line">The line number in which the message originated.</param>
        /// <param name="errorType">The type of error that occurred.</param>
        /// <param name="message">Message string detailing what went wrong.</param>
        /// <remarks>
        /// If the error type is not <see cref="ErrorType.DebugCheck"/>, bgfx is in an
        /// unrecoverable state and the application should terminate.
        ///
        /// This method can be called from any thread.
        /// </remarks>
        void ReportError (string fileName, int line, ErrorType errorType, string message);

        /// <summary>
        /// Called to print debug messages.
        /// </summary>
        /// <param name="fileName">The name of the source file in which the message originated.</param>
        /// <param name="line">The line number in which the message originated.</param>
        /// <param name="format">The message format string.</param>
        /// <param name="args">A pointer to format arguments.</param>
        /// <remarks>This method can be called from any thread.</remarks>
        void ReportDebug (string fileName, int line, string format, IntPtr args);

        /// <summary>
        /// Called when a profiling region is entered.
        /// </summary>
        /// <param name="name">The name of the region.</param>
        /// <param name="color">The color of the region.</param>
        /// <param name="filePath">The path of the source file containing the region.</param>
        /// <param name="line">The line number on which the region was started.</param>
        void ProfilerBegin (string name, int color, string filePath, int line);

        /// <summary>
        /// Called when a profiling region is ended.
        /// </summary>
        void ProfilerEnd ();

        /// <summary>
        /// Queries the size of a cache item.
        /// </summary>
        /// <param name="id">The cache entry ID.</param>
        /// <returns>The size of the cache item, or 0 if the item is not found.</returns>
        int GetCachedSize (long id);

        /// <summary>
        /// Retrieves an entry from the cache.
        /// </summary>
        /// <param name="id">The cache entry ID.</param>
        /// <param name="data">A pointer that should be filled with data from the cache.</param>
        /// <param name="size">The size of the memory block pointed to be <paramref name="data"/>.</param>
        /// <returns><c>true</c> if the item is found in the cache; otherwise, <c>false</c>.</returns>
        bool GetCacheEntry (long id, IntPtr data, int size);

        /// <summary>
        /// Saves an entry in the cache.
        /// </summary>
        /// <param name="id">The cache entry ID.</param>
        /// <param name="data">A pointer to the data to save in the cache.</param>
        /// <param name="size">The size of the memory block pointed to be <paramref name="data"/>.</param>
        void SetCacheEntry (long id, IntPtr data, int size);

        /// <summary>
        /// Save a captured screenshot.
        /// </summary>
        /// <param name="path">The path at which to save the image.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="pitch">The number of bytes between lines in the image.</param>
        /// <param name="data">A pointer to the image data to save.</param>
        /// <param name="size">The size of the image memory.</param>
        /// <param name="flipVertical"><c>true</c> if the image origin is bottom left instead of top left; otherwise, <c>false</c>.</param>
        void SaveScreenShot (string path, int width, int height, int pitch, IntPtr data, int size, bool flipVertical);

        /// <summary>
        /// Notifies that a frame capture has begun.
        /// </summary>
        /// <param name="width">The width of the capture surface.</param>
        /// <param name="height">The height of the capture surface.</param>
        /// <param name="pitch">The number of bytes between lines in the captured frames.</param>
        /// <param name="format">The format of captured frames.</param>
        /// <param name="flipVertical"><c>true</c> if the image origin is bottom left instead of top left; otherwise, <c>false</c>.</param>
        void CaptureStarted (int width, int height, int pitch, TextureFormat format, bool flipVertical);

        /// <summary>
        /// Notifies that a frame capture has finished.
        /// </summary>
        void CaptureFinished ();

        /// <summary>
        /// Notifies that a frame has been captured.
        /// </summary>
        /// <param name="data">A pointer to the frame data.</param>
        /// <param name="size">The size of the frame data.</param>
        void CaptureFrame (IntPtr data, int size);
    }

    unsafe struct CallbackShim {
        IntPtr vtbl;
        IntPtr reportError;
        IntPtr reportDebug;
        IntPtr profilerBegin;
        IntPtr profilerBeginLiteral;
        IntPtr profilerEnd;
        IntPtr getCachedSize;
        IntPtr getCacheEntry;
        IntPtr setCacheEntry;
        IntPtr saveScreenShot;
        IntPtr captureStarted;
        IntPtr captureFinished;
        IntPtr captureFrame;

        public static unsafe IntPtr CreateShim (ICallbackHandler handler) {
            if (handler == null)
                return IntPtr.Zero;

            if (savedDelegates != null)
                throw new InvalidOperationException("Callbacks should only be initialized once; bgfx can only deal with one set at a time.");

            var memory = Marshal.AllocHGlobal(Marshal.SizeOf<CallbackShim>());
            var shim = (CallbackShim*)memory;
            var saver = new DelegateSaver(handler, shim);

            // the shim uses the unnecessary ctor slot to act as a vtbl pointer to itself,
            // so that the same block of memory can act as both bgfx_callback_interface_t and bgfx_callback_vtbl_t
            shim->vtbl = memory + IntPtr.Size;

            // cache the data so we can free it later
            shimMemory = memory;
            savedDelegates = saver;

            return memory;
        }

        public static void FreeShim () {
            if (savedDelegates == null)
                return;

            savedDelegates = null;
            Marshal.FreeHGlobal(shimMemory);
        }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void ReportErrorHandler (IntPtr thisPtr, string fileName, ushort line, ErrorType errorType, string message);

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void ReportDebugHandler (IntPtr thisPtr, string fileName, ushort line, string format, IntPtr args);

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void ProfilerBeginHandler (IntPtr thisPtr, sbyte* name, int abgr, sbyte* filePath, ushort line);

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void ProfilerEndHandler (IntPtr thisPtr);

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int GetCachedSizeHandler (IntPtr thisPtr, long id);

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate bool GetCacheEntryHandler (IntPtr thisPtr, long id, IntPtr data, int size);

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void SetCacheEntryHandler (IntPtr thisPtr, long id, IntPtr data, int size);

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void SaveScreenShotHandler (IntPtr thisPtr, string path, int width, int height, int pitch, IntPtr data, int size, [MarshalAs(UnmanagedType.U1)] bool flipVertical);

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void CaptureStartedHandler (IntPtr thisPtr, int width, int height, int pitch, TextureFormat format, [MarshalAs(UnmanagedType.U1)] bool flipVertical);

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void CaptureFinishedHandler (IntPtr thisPtr);

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void CaptureFrameHandler (IntPtr thisPtr, IntPtr data, int size);

        // We're creating delegates to a user's interface methods; we're then converting those delegates
        // to native pointers and passing them into native code. If we don't save the references to the
        // delegates in managed land somewhere, the GC will think they're unreferenced and clean them
        // up, leaving native holding a bag of pointers into nowhere land.
        class DelegateSaver {
            ICallbackHandler handler;
            ReportErrorHandler reportError;
            ReportDebugHandler reportDebug;
            ProfilerBeginHandler profilerBegin;
            ProfilerBeginHandler profilerBeginLiteral;
            ProfilerEndHandler profilerEnd;
            GetCachedSizeHandler getCachedSize;
            GetCacheEntryHandler getCacheEntry;
            SetCacheEntryHandler setCacheEntry;
            SaveScreenShotHandler saveScreenShot;
            CaptureStartedHandler captureStarted;
            CaptureFinishedHandler captureFinished;
            CaptureFrameHandler captureFrame;

            public unsafe DelegateSaver (ICallbackHandler handler, CallbackShim* shim) {
                this.handler = handler;
                reportError = ReportError;
                reportDebug = ReportDebug;
                profilerBegin = ProfilerBegin;
                profilerBeginLiteral = ProfilerBegin;
                profilerEnd = ProfilerEnd;
                getCachedSize = GetCachedSize;
                getCacheEntry = GetCacheEntry;
                setCacheEntry = SetCacheEntry;
                saveScreenShot = SaveScreenShot;
                captureStarted = CaptureStarted;
                captureFinished = CaptureFinished;
                captureFrame = CaptureFrame;

                shim->reportError = Marshal.GetFunctionPointerForDelegate(reportError);
                shim->reportDebug = Marshal.GetFunctionPointerForDelegate(reportDebug);
                shim->profilerBegin = Marshal.GetFunctionPointerForDelegate(profilerBegin);
                shim->profilerBeginLiteral = Marshal.GetFunctionPointerForDelegate(profilerBeginLiteral);
                shim->profilerEnd = Marshal.GetFunctionPointerForDelegate(profilerEnd);
                shim->getCachedSize = Marshal.GetFunctionPointerForDelegate(getCachedSize);
                shim->getCacheEntry = Marshal.GetFunctionPointerForDelegate(getCacheEntry);
                shim->setCacheEntry = Marshal.GetFunctionPointerForDelegate(setCacheEntry);
                shim->saveScreenShot = Marshal.GetFunctionPointerForDelegate(saveScreenShot);
                shim->captureStarted = Marshal.GetFunctionPointerForDelegate(captureStarted);
                shim->captureFinished = Marshal.GetFunctionPointerForDelegate(captureFinished);
                shim->captureFrame = Marshal.GetFunctionPointerForDelegate(captureFrame);
            }

            void ReportError (IntPtr thisPtr, string fileName, ushort line, ErrorType errorType, string message) {
                handler.ReportError(fileName, line, errorType, message);
            }

            void ReportDebug (IntPtr thisPtr, string fileName, ushort line, string format, IntPtr args) {
                handler.ReportDebug(fileName, line, format, args);
            }

            void ProfilerBegin (IntPtr thisPtr, sbyte* name, int color, sbyte* filePath, ushort line) {
                handler.ProfilerBegin(new string(name), color, new string(filePath), line);
            }

            void ProfilerEnd (IntPtr thisPtr) {
                handler.ProfilerEnd();
            }

            int GetCachedSize (IntPtr thisPtr, long id) {
                return handler.GetCachedSize(id);
            }

            bool GetCacheEntry (IntPtr thisPtr, long id, IntPtr data, int size) {
                return handler.GetCacheEntry(id, data, size);
            }

            void SetCacheEntry (IntPtr thisPtr, long id, IntPtr data, int size) {
                handler.SetCacheEntry(id, data, size);
            }

            void SaveScreenShot (IntPtr thisPtr, string path, int width, int height, int pitch, IntPtr data, int size, bool flipVertical) {
                handler.SaveScreenShot(path, width, height, pitch, data, size, flipVertical);
            }

            void CaptureStarted (IntPtr thisPtr, int width, int height, int pitch, TextureFormat format, bool flipVertical) {
                handler.CaptureStarted(width, height, pitch, format, flipVertical);
            }

            void CaptureFinished (IntPtr thisPtr) {
                handler.CaptureFinished();
            }

            void CaptureFrame (IntPtr thisPtr, IntPtr data, int size) {
                handler.CaptureFrame(data, size);
            }
        }

        static IntPtr shimMemory;
        static DelegateSaver savedDelegates;
    }
}
