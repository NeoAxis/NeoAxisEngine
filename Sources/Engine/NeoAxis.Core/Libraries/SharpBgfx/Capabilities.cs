using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpBgfx {
    /// <summary>
    /// Contains information about the capabilities of the rendering device.
    /// </summary>
    public unsafe struct Capabilities {
        Caps* data;

        /// <summary>
        /// The currently active rendering backend API.
        /// </summary>
        public RendererBackend Backend {
            get { return data->Backend; }
        }

        /// <summary>
        /// A set of extended features supported by the device.
        /// </summary>
        public DeviceFeatures SupportedFeatures {
            get { return data->Supported; }
        }

        /// <summary>
        /// The maximum number of draw calls in a single frame.
        /// </summary>
        public int MaxDrawCalls {
            get { return (int)data->MaxDrawCalls; }
        }

        /// <summary>
        /// The maximum number of texture blits in a single frame.
        /// </summary>
        public int MaxBlits {
            get { return (int)data->MaxBlits; }
        }

        /// <summary>
        /// The maximum size of a texture, in pixels.
        /// </summary>
        public int MaxTextureSize {
            get { return (int)data->MaxTextureSize; }
        }

        /// <summary>
        /// The maximum layers in a texture.
        /// </summary>
        public int MaxTextureLayers {
            get { return (int)data->MaxTextureLayers; }
        }

        /// <summary>
        /// The maximum number of render views supported.
        /// </summary>
        public int MaxViews {
            get { return (int)data->MaxViews; }
        }

        /// <summary>
        /// The maximum number of frame buffers that can be allocated.
        /// </summary>
        public int MaxFramebuffers {
            get { return (int)data->MaxFramebuffers; }
        }

        /// <summary>
        /// The maximum number of attachments to a single framebuffer.
        /// </summary>
        public int MaxFramebufferAttachments {
            get { return (int)data->MaxFramebufferAttachements; }
        }

        /// <summary>
        /// The maximum number of programs that can be allocated.
        /// </summary>
        public int MaxPrograms {
            get { return (int)data->MaxPrograms; }
        }

        /// <summary>
        /// The maximum number of shaders that can be allocated.
        /// </summary>
        public int MaxShaders {
            get { return (int)data->MaxShaders; }
        }

        /// <summary>
        /// The maximum number of textures that can be allocated.
        /// </summary>
        public int MaxTextures {
            get { return (int)data->MaxTextures; }
        }

        /// <summary>
        /// The maximum number of texture samplers that can be allocated.
        /// </summary>
        public int MaxTextureSamplers {
            get { return (int)data->MaxTextureSamplers; }
        }

        /// <summary>
        /// The maximum number of compute bindings that can be allocated.
        /// </summary>
        public int MaxComputeBindings {
            get { return (int)data->MaxComputeBindings; }
        }

        /// <summary>
        /// The maximum number of vertex declarations that can be allocated.
        /// </summary>
        public int MaxVertexDecls {
            get { return (int)data->MaxVertexDecls; }
        }

        /// <summary>
        /// The maximum number of vertex streams that can be used.
        /// </summary>
        public int MaxVertexStreams {
            get { return (int)data->MaxVertexStreams; }
        }

        /// <summary>
        /// The maximum number of index buffers that can be allocated.
        /// </summary>
        public int MaxIndexBuffers {
            get { return (int)data->MaxIndexBuffers; }
        }

        /// <summary>
        /// The maximum number of vertex buffers that can be allocated.
        /// </summary>
        public int MaxVertexBuffers {
            get { return (int)data->MaxVertexBuffers; }
        }

        /// <summary>
        /// The maximum number of dynamic index buffers that can be allocated.
        /// </summary>
        public int MaxDynamicIndexBuffers {
            get { return (int)data->MaxDynamicIndexBuffers; }
        }

        /// <summary>
        /// The maximum number of dynamic vertex buffers that can be allocated.
        /// </summary>
        public int MaxDynamicVertexBuffers {
            get { return (int)data->MaxDynamicVertexBuffers; }
        }

        /// <summary>
        /// The maximum number of uniforms that can be used.
        /// </summary>
        public int MaxUniforms {
            get { return (int)data->MaxUniforms; }
        }

        /// <summary>
        /// The maximum number of occlusion queries that can be used.
        /// </summary>
        public int MaxOcclusionQueries {
            get { return (int)data->MaxOcclusionQueries; }
        }

        /// <summary>
        /// The maximum number of encoder threads.
        /// </summary>
        public int MaxEncoders {
            get { return (int)data->MaxEncoders; }
        }

        /// <summary>
        /// The amount of transient vertex buffer space reserved.
        /// </summary>
        public int TransientVertexBufferSize {
            get { return (int)data->TransientVbSize; }
        }

        /// <summary>
        /// The amount of transient index buffer space reserved.
        /// </summary>
        public int TransientIndexBufferSize {
            get { return (int)data->TransientIbSize; }
        }

        /// <summary>
        /// Indicates whether depth coordinates in NDC range from -1 to 1 (true) or 0 to 1 (false).
        /// </summary>
        public bool HomogeneousDepth {
            get { return data->HomogeneousDepth != 0; }
        }

        /// <summary>
        /// Indicates whether the coordinate system origin is at the bottom left or top left.
        /// </summary>
        public bool OriginBottomLeft {
            get { return data->OriginBottomLeft != 0; }
        }

        /// <summary>
        /// Details about the currently active graphics adapter.
        /// </summary>
        public Adapter CurrentAdapter {
            get { return new Adapter((Vendor)data->VendorId, data->DeviceId); }
        }

        /// <summary>
        /// A list of all graphics adapters installed on the system.
        /// </summary>
        public AdapterCollection Adapters {
            get { return new AdapterCollection(data->GPUs, data->GPUCount); }
        }

        static Capabilities() {
            //Debug.Assert(Caps.TextureFormatCount == (int)TextureFormat.Count);
        }

        internal Capabilities (Caps* data) {
            this.data = data;
        }

        /// <summary>
        /// Checks device support for a specific texture format.
        /// </summary>
        /// <param name="format">The format to check.</param>
        /// <returns>The level of support for the given format.</returns>
        public TextureFormatSupport CheckTextureSupport (TextureFormat format) {
            return (TextureFormatSupport)data->Formats[(int)format];
        }

        /// <summary>
        /// Provides access to a collection of adapters.
        /// </summary>
        public unsafe struct AdapterCollection : IReadOnlyList<Adapter> {
            ushort* data;
            int count;

            /// <summary>
            /// Accesses the element at the specified index.
            /// </summary>
            /// <param name="index">The index of the element to retrieve.</param>
            /// <returns>The element at the given index.</returns>
            public Adapter this[int index] {
                get { return new Adapter((Vendor)data[index * 2], data[index * 2 + 1]); }
            }

            /// <summary>
            /// The number of elements in the collection.
            /// </summary>
            public int Count {
                get { return count; }
            }

            internal AdapterCollection (ushort* data, int count) {
                this.data = data;
                this.count = count;
            }

            /// <summary>
            /// Gets an enumerator for the collection.
            /// </summary>
            /// <returns>A collection enumerator.</returns>
            public Enumerator GetEnumerator () {
                return new Enumerator(this);
            }

            IEnumerator<Adapter> IEnumerable<Adapter>.GetEnumerator () {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator () {
                return GetEnumerator();
            }

            /// <summary>
            /// Implements an enumerator for an AdapterCollection.
            /// </summary>
            public struct Enumerator : IEnumerator<Adapter> {
                AdapterCollection collection;
                int index;

                /// <summary>
                /// The current enumerated item.
                /// </summary>
                public Adapter Current {
                    get { return collection[index]; }
                }

                object IEnumerator.Current {
                    get { return Current; }
                }

                internal Enumerator (AdapterCollection collection) {
                    this.collection = collection;
                    index = -1;
                }

                /// <summary>
                /// Advances to the next item in the sequence.
                /// </summary>
                /// <returns><c>true</c> if there are more items in the collection; otherwise, <c>false</c>.</returns>
                public bool MoveNext () {
                    var newIndex = index + 1;
                    if (newIndex >= collection.Count)
                        return false;

                    index = newIndex;
                    return true;
                }

                /// <summary>
                /// Empty; does nothing.
                /// </summary>
                public void Dispose () {
                }

                /// <summary>
                /// Not implemented.
                /// </summary>
                public void Reset () {
                    throw new NotImplementedException();
                }
            }
        }

#pragma warning disable 649
        internal unsafe struct Caps {
            public RendererBackend Backend;
            public DeviceFeatures Supported;
            public ushort VendorId;
            public ushort DeviceId;
            public byte HomogeneousDepth;
            public byte OriginBottomLeft;
            public byte GPUCount;

            public fixed ushort GPUs[8];

            public uint MaxDrawCalls;
            public uint MaxBlits;
            public uint MaxTextureSize;
            public uint MaxTextureLayers;
            public uint MaxViews;
            public uint MaxFramebuffers;
            public uint MaxFramebufferAttachements;
            public uint MaxPrograms;
            public uint MaxShaders;
            public uint MaxTextures;
            public uint MaxTextureSamplers;
            public uint MaxComputeBindings;
            public uint MaxVertexDecls;
            public uint MaxVertexStreams;
            public uint MaxIndexBuffers;
            public uint MaxVertexBuffers;
            public uint MaxDynamicIndexBuffers;
            public uint MaxDynamicVertexBuffers;
            public uint MaxUniforms;
            public uint MaxOcclusionQueries;
            public uint MaxEncoders;
            public uint TransientVbSize;
            public uint TransientIbSize;

            public fixed ushort Formats[(int)TextureFormat.Count];
        }
#pragma warning restore 649
    }

    /// <summary>
    /// Contains details about an installed graphics adapter.
    /// </summary>
    public struct Adapter {
        /// <summary>
        /// Represents the default adapter for the system.
        /// </summary>
        public static readonly Adapter Default = new Adapter(Vendor.None, 0);

        /// <summary>
        /// The IHV that published the adapter.
        /// </summary>
        public readonly Vendor Vendor;

        /// <summary>
        /// A vendor-specific identifier for the adapter type.
        /// </summary>
        public readonly int DeviceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Adapter"/> struct.
        /// </summary>
        /// <param name="vendor">The vendor.</param>
        /// <param name="deviceId">The device ID.</param>
        public Adapter (Vendor vendor, int deviceId) {
            Vendor = vendor;
            DeviceId = deviceId;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString () {
            return string.Format("Vendor: {0}, Device: {0}", Vendor, DeviceId);
        }
    }

    /// <summary>
    /// Contains various performance metrics tracked by the library.
    /// </summary>
    public unsafe struct PerfStats {
        Stats* data;

        /// <summary>
        /// CPU time between two <see cref="Bgfx.Frame"/> calls.
        /// </summary>
        public long CpuTimeFrame {
            get { return data->CpuTimeFrame; }
        }

        /// <summary>
        /// CPU frame start time.
        /// </summary>
        public long CpuTimeStart {
            get { return data->CpuTimeBegin; }
        }

        /// <summary>
        /// CPU frame end time.
        /// </summary>
        public long CpuTimeEnd {
            get { return data->CpuTimeEnd; }
        }

        /// <summary>
        /// CPU timer frequency.
        /// </summary>
        public long CpuTimerFrequency {
            get { return data->CpuTimerFrequency; }
        }

        /// <summary>
        /// Elapsed CPU time.
        /// </summary>
        public TimeSpan CpuElapsed {
            get { return TimeSpan.FromSeconds((double)(CpuTimeEnd - CpuTimeStart) / CpuTimerFrequency); }
        }

        /// <summary>
        /// GPU frame start time.
        /// </summary>
        public long GpuTimeStart {
            get { return data->GpuTimeBegin; }
        }

        /// <summary>
        /// GPU frame end time.
        /// </summary>
        public long GpuTimeEnd {
            get { return data->GpuTimeEnd; }
        }

        /// <summary>
        /// GPU timer frequency.
        /// </summary>
        public long GpuTimerFrequency {
            get { return data->GpuTimerFrequency; }
        }

        /// <summary>
        /// Elapsed GPU time.
        /// </summary>
        public TimeSpan GpuElapsed {
            get { return TimeSpan.FromSeconds((double)(GpuTimeEnd - GpuTimeStart) / GpuTimerFrequency); }
        }

        /// <summary>
        /// Time spent waiting for the render thread.
        /// </summary>
        public long WaitingForRender {
            get { return data->WaitRender; }
        }

        /// <summary>
        /// Time spent waiting for the submit thread.
        /// </summary>
        public long WaitingForSubmit {
            get { return data->WaitSubmit; }
        }

        /// <summary>
        /// The number of draw calls submitted.
        /// </summary>
        public int DrawCallsSubmitted {
            get { return data->NumDraw; }
        }

        /// <summary>
        /// The number of compute calls submitted.
        /// </summary>
        public int ComputeCallsSubmitted {
            get { return data->NumCompute; }
        }

        /// <summary>
        /// The number of blit calls submitted.
        /// </summary>
        public int BlitCallsSubmitted {
            get { return data->NumBlit; }
        }

        /// <summary>
        /// Maximum observed GPU driver latency.
        /// </summary>
        public int MaxGpuLatency {
            get { return data->MaxGpuLatency; }
        }

        /// <summary>
        /// Number of allocated dynamic index buffers.
        /// </summary>
        public int DynamicIndexBufferCount {
            get { return data->NumDynamicIndexBuffers; }
        }

        /// <summary>
        /// Number of allocated dynamic vertex buffers.
        /// </summary>
        public int DynamicVertexBufferCount {
            get { return data->NumDynamicVertexBuffers; }
        }

        /// <summary>
        /// Number of allocated frame buffers.
        /// </summary>
        public int FrameBufferCount {
            get { return data->NumFrameBuffers; }
        }

        /// <summary>
        /// Number of allocated index buffers.
        /// </summary>
        public int IndexBufferCount {
            get { return data->NumIndexBuffers; }
        }

        /// <summary>
        /// Number of allocated occlusion queries.
        /// </summary>
        public int OcclusionQueryCount {
            get { return data->NumOcclusionQueries; }
        }

        /// <summary>
        /// Number of allocated shader programs.
        /// </summary>
        public int ProgramCount {
            get { return data->NumPrograms; }
        }

        /// <summary>
        /// Number of allocated shaders.
        /// </summary>
        public int ShaderCount {
            get { return data->NumShaders; }
        }

        /// <summary>
        /// Number of allocated textures.
        /// </summary>
        public int TextureCount {
            get { return data->NumTextures; }
        }

        /// <summary>
        /// Number of allocated uniforms.
        /// </summary>
        public int UniformCount {
            get { return data->NumUniforms; }
        }

        /// <summary>
        /// Number of allocated vertex buffers.
        /// </summary>
        public int VertexBufferCount {
            get { return data->NumVertexBuffers; }
        }

        /// <summary>
        /// Number of allocated vertex declarations.
        /// </summary>
        public int VertexDeclarationCount {
            get { return data->NumVertexDecls; }
        }

        /// <summary>
        /// The amount of memory used by textures.
        /// </summary>
        public long TextureMemoryUsed {
            get { return data->TextureMemoryUsed; }
        }

        /// <summary>
        /// The amount of memory used by render targets.
        /// </summary>
        public long RenderTargetMemoryUsed {
            get { return data->RtMemoryUsed; }
        }

        /// <summary>
        /// The number of transient vertex buffers used.
        /// </summary>
        public int TransientVertexBuffersUsed {
            get { return data->TransientVbUsed; }
        }

        /// <summary>
        /// The number of transient index buffers used.
        /// </summary>
        public int TransientIndexBuffersUsed {
            get { return data->TransientIbUsed; }
        }

        /// <summary>
        /// Maximum available GPU memory.
        /// </summary>
        public long MaxGpuMemory {
            get { return data->GpuMemoryMax; }
        }

        /// <summary>
        /// The amount of GPU memory currently in use.
        /// </summary>
        public long GpuMemoryUsed {
            get { return data->GpuMemoryUsed; }
        }

        /// <summary>
        /// The width of the back buffer.
        /// </summary>
        public int BackbufferWidth {
            get { return data->Width; }
        }

        /// <summary>
        /// The height of the back buffer.
        /// </summary>
        public int BackbufferHeight {
            get { return data->Height; }
        }

        /// <summary>
        /// The width of the debug text buffer.
        /// </summary>
        public int TextBufferWidth {
            get { return data->TextWidth; }
        }

        /// <summary>
        /// The height of the debug text buffer.
        /// </summary>
        public int TextBufferHeight {
            get { return data->TextHeight; }
        }

        /// <summary>
        /// Gets a collection of statistics for each rendering view.
        /// </summary>
        public ViewStatsCollection Views {
            get { return new ViewStatsCollection(data->ViewStats, data->NumViews); }
        }

        static PerfStats() {
            Debug.Assert(Stats.NumTopologies == Enum.GetValues(typeof(Topology)).Length);
        }

        internal PerfStats (Stats* data) {
            this.data = data;
        }

        /// <summary>
        /// Gets the number of primitives rendered with the given topology.
        /// </summary>
        /// <param name="topology">The topology whose primitive count should be returned.</param>
        /// <returns>The number of primitives rendered.</returns>
        public int GetPrimitiveCount(Topology topology) {
            return (int)data->NumPrims[(int)topology];
        }

        /// <summary>
        /// Contains perf metrics for a single rendering view.
        /// </summary>
        public struct ViewStats {
            ViewStatsNative* data;

            /// <summary>
            /// The name of the view.
            /// </summary>
            public string Name {
                get { return new string(data->Name); }
            }

            /// <summary>
            /// The amount of CPU time elapsed during processing of this view.
            /// </summary>
            public long CpuTimeElapsed {
                get { return (long)data->CpuTimeElapsed; }
            }

            /// <summary>
            /// The amount of GPU time elapsed during processing of this view.
            /// </summary>
            public long GpuTimeElapsed {
                get { return (long)data->GpuTimeElapsed; }
            }

            internal ViewStats(ViewStatsNative* data) {
                this.data = data;
            }
        }

        /// <summary>
        /// Provides access to a collection of view statistics.
        /// </summary>
        public struct ViewStatsCollection : IReadOnlyList<ViewStats> {
            ViewStatsNative* data;
            int count;

            /// <summary>
            /// Accesses the element at the specified index.
            /// </summary>
            /// <param name="index">The index of the element to retrieve.</param>
            /// <returns>The element at the given index.</returns>
            public ViewStats this[int index] {
                get { return new ViewStats(data + index); }
            }

            /// <summary>
            /// The number of elements in the collection.
            /// </summary>
            public int Count {
                get { return count; }
            }

            internal ViewStatsCollection(ViewStatsNative* data, int count) {
                this.data = data;
                this.count = count;
            }

            /// <summary>
            /// Gets an enumerator for the collection.
            /// </summary>
            /// <returns>A collection enumerator.</returns>
            public Enumerator GetEnumerator() {
                return new Enumerator(this);
            }

            IEnumerator<ViewStats> IEnumerable<ViewStats>.GetEnumerator() {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            /// <summary>
            /// Implements an enumerator for a ViewStatsCollection.
            /// </summary>
            public struct Enumerator : IEnumerator<ViewStats> {
                ViewStatsCollection collection;
                int index;

                /// <summary>
                /// The current enumerated item.
                /// </summary>
                public ViewStats Current {
                    get { return collection[index]; }
                }

                object IEnumerator.Current {
                    get { return Current; }
                }

                internal Enumerator(ViewStatsCollection collection) {
                    this.collection = collection;
                    index = -1;
                }

                /// <summary>
                /// Advances to the next item in the sequence.
                /// </summary>
                /// <returns><c>true</c> if there are more items in the collection; otherwise, <c>false</c>.</returns>
                public bool MoveNext() {
                    var newIndex = index + 1;
                    if (newIndex >= collection.Count)
                        return false;

                    index = newIndex;
                    return true;
                }

                /// <summary>
                /// Empty; does nothing.
                /// </summary>
                public void Dispose() {
                }

                /// <summary>
                /// Not implemented.
                /// </summary>
                public void Reset() {
                    throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Contains perf metrics for a single encoder instance.
        /// </summary>
        public struct EncoderStats {
            EncoderStatsNative* data;

            /// <summary>
            /// CPU frame start time.
            /// </summary>
            public long CpuTimeStart {
                get { return data->CpuTimeBegin; }
            }

            /// <summary>
            /// CPU frame end time.
            /// </summary>
            public long CpuTimeEnd {
                get { return data->CpuTimeEnd; }
            }

            internal EncoderStats (EncoderStatsNative* data) {
                this.data = data;
            }
        }

        /// <summary>
        /// Provides access to a collection of encoder statistics.
        /// </summary>
        public struct EncoderStatsCollection : IReadOnlyList<EncoderStats> {
            EncoderStatsNative* data;
            int count;

            /// <summary>
            /// Accesses the element at the specified index.
            /// </summary>
            /// <param name="index">The index of the element to retrieve.</param>
            /// <returns>The element at the given index.</returns>
            public EncoderStats this[int index] {
                get { return new EncoderStats(data + index); }
            }

            /// <summary>
            /// The number of elements in the collection.
            /// </summary>
            public int Count {
                get { return count; }
            }

            internal EncoderStatsCollection (EncoderStatsNative* data, int count) {
                this.data = data;
                this.count = count;
            }

            /// <summary>
            /// Gets an enumerator for the collection.
            /// </summary>
            /// <returns>A collection enumerator.</returns>
            public Enumerator GetEnumerator () {
                return new Enumerator(this);
            }

            IEnumerator<EncoderStats> IEnumerable<EncoderStats>.GetEnumerator () {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator () {
                return GetEnumerator();
            }

            /// <summary>
            /// Implements an enumerator for an EncoderStatsCollection.
            /// </summary>
            public struct Enumerator : IEnumerator<EncoderStats> {
                EncoderStatsCollection collection;
                int index;

                /// <summary>
                /// The current enumerated item.
                /// </summary>
                public EncoderStats Current {
                    get { return collection[index]; }
                }

                object IEnumerator.Current {
                    get { return Current; }
                }

                internal Enumerator (EncoderStatsCollection collection) {
                    this.collection = collection;
                    index = -1;
                }

                /// <summary>
                /// Advances to the next item in the sequence.
                /// </summary>
                /// <returns><c>true</c> if there are more items in the collection; otherwise, <c>false</c>.</returns>
                public bool MoveNext () {
                    var newIndex = index + 1;
                    if (newIndex >= collection.Count)
                        return false;

                    index = newIndex;
                    return true;
                }

                /// <summary>
                /// Empty; does nothing.
                /// </summary>
                public void Dispose () {
                }

                /// <summary>
                /// Not implemented.
                /// </summary>
                public void Reset () {
                    throw new NotImplementedException();
                }
            }
        }

#pragma warning disable 649
        internal struct ViewStatsNative {
            public fixed char Name[256];
            public ushort View;
            public ulong CpuTimeElapsed;
            public ulong GpuTimeElapsed;
        }

        internal struct EncoderStatsNative {
            public long CpuTimeBegin;
            public long CpuTimeEnd;
        }

        internal struct Stats {
            public const int NumTopologies = 5;

            public long CpuTimeFrame;
            public long CpuTimeBegin;
            public long CpuTimeEnd;
            public long CpuTimerFrequency;
            public long GpuTimeBegin;
            public long GpuTimeEnd;
            public long GpuTimerFrequency;
            public long WaitRender;
            public long WaitSubmit;
            public int NumDraw;
            public int NumCompute;
            public int NumBlit;
            public int MaxGpuLatency;
            public ushort NumDynamicIndexBuffers;
            public ushort NumDynamicVertexBuffers;
            public ushort NumFrameBuffers;
            public ushort NumIndexBuffers;
            public ushort NumOcclusionQueries;
            public ushort NumPrograms;
            public ushort NumShaders;
            public ushort NumTextures;
            public ushort NumUniforms;
            public ushort NumVertexBuffers;
            public ushort NumVertexDecls;
            public long TextureMemoryUsed;
            public long RtMemoryUsed;
            public int TransientVbUsed;
            public int TransientIbUsed;
            public fixed uint NumPrims[NumTopologies];
            public long GpuMemoryMax;
            public long GpuMemoryUsed;
            public ushort Width;
            public ushort Height;
            public ushort TextWidth;
            public ushort TextHeight;
            public ushort NumViews;
            public ViewStatsNative* ViewStats;
            public byte NumEncoders;
            public EncoderStatsNative* EncoderStats;
        }
#pragma warning restore 649
    }

    /// <summary>
    /// Contains various settings used to initialize the library.
    /// </summary>
    public class InitSettings {
        /// <summary>
        /// The backend API to use for rendering.
        /// </summary>
        public RendererBackend Backend { get; set; }

        /// <summary>
        /// The adapter on which to create the device.
        /// </summary>
        public Adapter Adapter { get; set; }

        /// <summary>
        /// Enable debugging with the device.
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Enable profling with the device.
        /// </summary>
        public bool Profiling { get; set; }

        /// <summary>
        /// The initial texture format of the screen.
        /// </summary>
        public TextureFormat Format { get; set; }

        /// <summary>
        /// The initial width of the screen.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The initial height of the screen.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Various flags that control creation of the device.
        /// </summary>
        public ResetFlags ResetFlags { get; set; }

        /// <summary>
        /// The number of backbuffers to create.
        /// </summary>
        public int BackBufferCount { get; set; }

        /// <summary>
        /// The maximum allowed frame latency, or zero if you don't care.
        /// </summary>
        public int MaxFrameLatency { get; set; }

        /// <summary>
        /// A set of handlers for various library callbacks.
        /// </summary>
        public ICallbackHandler CallbackHandler { get; set; }

        /// <summary>
        /// Optional platform-specific initialization data.
        /// </summary>
        public PlatformData PlatformData { get; set; }

        /// <summary>
        /// Initializes a new intance of the <see cref="InitSettings"/> class.
        /// </summary>
        unsafe public InitSettings () {
            Native native;
            NativeMethods.bgfx_init_ctor(&native);

            Backend = native.Backend;
            Adapter = new Adapter((Vendor)native.VendorId, native.DeviceId);
            Debug = native.Debug != 0;
            Profiling = native.Profiling != 0;
            Format = native.Resolution.Format;
            Width = (int)native.Resolution.Width;
            Height = (int)native.Resolution.Height;
            ResetFlags = (ResetFlags)native.Resolution.Flags;
            BackBufferCount = native.Resolution.NumBackBuffers;
            MaxFrameLatency = native.Resolution.MaxFrameLatency;
            PlatformData = native.PlatformData;
        }

        /// <summary>
        /// Initializes a new intance of the <see cref="InitSettings"/> class.
        /// </summary>
        /// <param name="width">The initial width of the screen.</param>
        /// <param name="height">The initial height of the screen.</param>
        /// <param name="resetFlags">Various flags that control creation of the device.</param>
        public InitSettings (int width, int height, ResetFlags resetFlags = ResetFlags.None)
            : this() {

            Width = width;
            Height = height;
            ResetFlags = resetFlags;
        }

        internal struct ResolutionNative {
            public TextureFormat Format;
            public uint Width;
            public uint Height;
            public uint Flags;
            public byte NumBackBuffers;
            public byte MaxFrameLatency;
        }

        internal struct InitLimits {
            public ushort MaxEncoders;
            public uint TransientVbSize;
            public uint TransientIbSize;
        }

        internal struct Native {
            public RendererBackend Backend;
            public ushort VendorId;
            public ushort DeviceId;
            public byte Debug;
            public byte Profiling;
            public PlatformData PlatformData;
            public ResolutionNative Resolution;
            public InitLimits Limits;
            public IntPtr Callbacks;
            public IntPtr Allocator;
        }
    }
}
