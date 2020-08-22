using System;

namespace SharpBgfx {
    /// <summary>
    /// Specifies the supported rendering backend APIs.
    /// </summary>
    public enum RendererBackend {
        /// <summary>
        /// No backend given.
        /// </summary>
        Noop,

        /// <summary>
        /// Direct3D 9
        /// </summary>
        Direct3D9,

        /// <summary>
        /// Direct3D 11
        /// </summary>
        Direct3D11,

        /// <summary>
        /// Direct3D 12
        /// </summary>
        Direct3D12,

        /// <summary>
        /// PlayStation 4's GNM
        /// </summary>
        GNM,

        /// <summary>
        /// Apple Metal.
        /// </summary>
        Metal,

        /// <summary>
        /// OpenGL ES
        /// </summary>
        OpenGLES,

        /// <summary>
        /// OpenGL
        /// </summary>
        OpenGL,

        /// <summary>
        /// Vulkan
        /// </summary>
        Vulkan,

        WebGPU,

        /// <summary>
        /// Used during initialization; specifies that the library should
        /// pick the best renderer for the running hardware and OS.
        /// </summary>
        Default
    }

    /// <summary>
    /// Specifies vertex attribute usages.
    /// </summary>
    public enum VertexAttributeUsage {
        /// <summary>
        /// Position data.
        /// </summary>
        Position,

        /// <summary>
        /// Normals.
        /// </summary>
        Normal,

        /// <summary>
        /// Tangents.
        /// </summary>
        Tangent,

        /// <summary>
        /// Bitangents.
        /// </summary>
        Bitangent,

        /// <summary>
        /// First color channel.
        /// </summary>
        Color0,

        /// <summary>
        /// Second color channel.
        /// </summary>
        Color1,

        /// <summary>
        /// Third color channel.
        /// </summary>
        Color2,

        /// <summary>
        /// Fourth color channel.
        /// </summary>
        Color3,

        /// <summary>
        /// Indices.
        /// </summary>
        Indices,

        /// <summary>
        /// Animation weights.
        /// </summary>
        Weight,

        /// <summary>
        /// First texture coordinate channel (arbitrary data).
        /// </summary>
        TexCoord0,

        /// <summary>
        /// Second texture coordinate channel (arbitrary data).
        /// </summary>
        TexCoord1,

        /// <summary>
        /// Third texture coordinate channel (arbitrary data).
        /// </summary>
        TexCoord2,

        /// <summary>
        /// Fourth texture coordinate channel (arbitrary data).
        /// </summary>
        TexCoord3,

        /// <summary>
        /// Fifth texture coordinate channel (arbitrary data).
        /// </summary>
        TexCoord4,

        /// <summary>
        /// Sixth texture coordinate channel (arbitrary data).
        /// </summary>
        TexCoord5,

        /// <summary>
        /// Seventh texture coordinate channel (arbitrary data).
        /// </summary>
        TexCoord6,

        /// <summary>
        /// Eighth texture coordinate channel (arbitrary data).
        /// </summary>
        TexCoord7
    }

    /// <summary>
    /// Specifies data types for vertex attributes.
    /// </summary>
    public enum VertexAttributeType {
        /// <summary>
        /// One-byte unsigned integer.
        /// </summary>
        UInt8,

        /// <summary>
        /// 10-bit unsigned integer.
        /// </summary>
        /// <remarks>
        /// Availability depends on Caps flags.
        /// </remarks>
        UInt10,

        /// <summary>
        /// Two-byte signed integer.
        /// </summary>
        Int16,

        /// <summary>
        /// Two-byte float.
        /// </summary>
        /// <remarks>
        /// Availability depends on Caps flags.
        /// </remarks>
        Half,

        /// <summary>
        /// Four-byte float.
        /// </summary>
        Float
    }

    /// <summary>
    /// Specifies the format of a texture's data.
    /// </summary>
    /// <remarks>
    /// Check Caps flags for hardware format support.
    /// </remarks>
    public enum TextureFormat {
        /// <summary>
        /// Block compression with three color channels, 1 bit alpha.
        /// </summary>
        BC1,

        /// <summary>
        /// Block compression with three color channels, 4 bits alpha.
        /// </summary>
        BC2,

        /// <summary>
        /// Block compression with three color channels, 8 bits alpha.
        /// </summary>
        BC3,

        /// <summary>
        /// Block compression for 1-channel color.
        /// </summary>
        BC4,

        /// <summary>
        /// Block compression for 2-channel color.
        /// </summary>
        BC5,

        /// <summary>
        /// Block compression for three-channel HDR color.
        /// </summary>
        BC6H,

        /// <summary>
        /// Highest quality block compression.
        /// </summary>
        BC7,

        /// <summary>
        /// Original ETC block compression.
        /// </summary>
        ETC1,

        /// <summary>
        /// Improved ETC block compression (no alpha).
        /// </summary>
        ETC2,

        /// <summary>
        /// Improved ETC block compression with full alpha.
        /// </summary>
        ETC2A,

        /// <summary>
        /// Improved ETC block compression with 1-bit punchthrough alpha.
        /// </summary>
        ETC2A1,

        /// <summary>
        /// PVRTC1 compression (2 bits per pixel)
        /// </summary>
        PTC12,

        /// <summary>
        /// PVRTC1 compression (4 bits per pixel)
        /// </summary>
        PTC14,

        /// <summary>
        /// PVRTC1 compression with alpha (2 bits per pixel)
        /// </summary>
        PTC12A,

        /// <summary>
        /// PVRTC1 compression with alpha (4 bits per pixel)
        /// </summary>
        PTC14A,

        /// <summary>
        /// PVRTC2 compression with alpha (2 bits per pixel)
        /// </summary>
        PTC22,

        /// <summary>
        /// PVRTC2 compression with alpha (4 bits per pixel)
        /// </summary>
        PTC24,

        /// <summary>
        /// ATC RGB (4 bits per pixel)
        /// </summary>
        ATC,

        /// <summary>
        /// ATCE RGBA with explicit alpha (8 bits per pixel)
        /// </summary>
        ATCE,

        /// <summary>
        /// ATCE RGBA with interpolated alpha (8 bits per pixel)
        /// </summary>
        ATCI,

        /// <summary>
        /// ASTC 4x4 8.0 bpp
        /// </summary>
        ASTC4x4,

        /// <summary>
        /// ASTC 5x5 5.12 bpp
        /// </summary>
        ASTC5x5,

        /// <summary>
        /// ASTC 6x6 3.56 bpp
        /// </summary>
        ASTC6x6,

        /// <summary>
        /// ASTC 8x5 3.20 bpp
        /// </summary>
        ASTC8x5,

        /// <summary>
        /// ASTC 8x6 2.67 bpp
        /// </summary>
        ASTC8x6,

        /// <summary>
        /// ASTC 10x5 2.56 bpp
        /// </summary>
        ASTC10x5,

        /// <summary>
        /// Unknown texture format.
        /// </summary>
        Unknown,

        /// <summary>
        /// 1-bit single channel.
        /// </summary>
        R1,

        /// <summary>
        /// 8-bit single channel (alpha).
        /// </summary>
        A8,

        /// <summary>
        /// 8-bit single channel.
        /// </summary>
        R8,

        /// <summary>
        /// 8-bit single channel (integer).
        /// </summary>
        R8I,

        /// <summary>
        /// 8-bit single channel (unsigned).
        /// </summary>
        R8U,

        /// <summary>
        /// 8-bit single channel (signed).
        /// </summary>
        R8S,

        /// <summary>
        /// 16-bit single channel.
        /// </summary>
        R16,

        /// <summary>
        /// 16-bit single channel (integer).
        /// </summary>
        R16I,

        /// <summary>
        /// 16-bit single channel (unsigned).
        /// </summary>
        R16U,

        /// <summary>
        /// 16-bit single channel (float).
        /// </summary>
        R16F,

        /// <summary>
        /// 16-bit single channel (signed).
        /// </summary>
        R16S,

        /// <summary>
        /// 32-bit single channel (integer).
        /// </summary>
        R32I,

        /// <summary>
        /// 32-bit single channel (unsigned).
        /// </summary>
        R32U,

        /// <summary>
        /// 32-bit single channel (float).
        /// </summary>
        R32F,

        /// <summary>
        /// 8-bit two channel.
        /// </summary>
        RG8,

        /// <summary>
        /// 8-bit two channel (integer).
        /// </summary>
        RG8I,

        /// <summary>
        /// 8-bit two channel (unsigned).
        /// </summary>
        RG8U,

        /// <summary>
        /// 8-bit two channel (signed).
        /// </summary>
        RG8S,

        /// <summary>
        /// 16-bit two channel.
        /// </summary>
        RG16,

        /// <summary>
        /// 16-bit two channel (integer).
        /// </summary>
        RG16I,

        /// <summary>
        /// 16-bit two channel (unsigned).
        /// </summary>
        RG16U,

        /// <summary>
        /// 16-bit two channel (float).
        /// </summary>
        RG16F,

        /// <summary>
        /// 16-bit two channel (signed).
        /// </summary>
        RG16S,

        /// <summary>
        /// 32-bit two channel (integer).
        /// </summary>
        RG32I,

        /// <summary>
        /// 32-bit two channel (unsigned).
        /// </summary>
        RG32U,

        /// <summary>
        /// 32-bit two channel (float).
        /// </summary>
        RG32F,

        /// <summary>
        /// 8-bit three channel.
        /// </summary>
        RGB8,

        /// <summary>
        /// 8-bit three channel (integer).
        /// </summary>
        RGB8I,

        /// <summary>
        /// 8-bit three channel (unsigned).
        /// </summary>
        RGB8U,

        /// <summary>
        /// 8-bit three channel (signed).
        /// </summary>
        RGB8S,

        /// <summary>
        /// 9-bit three channel floating point with shared 5-bit exponent.
        /// </summary>
        RGB9E5F,

        /// <summary>
        /// 8-bit BGRA color.
        /// </summary>
        BGRA8,

        /// <summary>
        /// 8-bit RGBA color.
        /// </summary>
        RGBA8,

        /// <summary>
        /// 8-bit RGBA color (integer).
        /// </summary>
        RGBA8I,

        /// <summary>
        /// 8-bit RGBA color (unsigned).
        /// </summary>
        RGBA8U,

        /// <summary>
        /// 8-bit RGBA color (signed).
        /// </summary>
        RGBA8S,

        /// <summary>
        /// 16-bit RGBA color.
        /// </summary>
        RGBA16,

        /// <summary>
        /// 16-bit RGBA color (integer).
        /// </summary>
        RGBA16I,

        /// <summary>
        /// 16-bit RGBA color (unsigned).
        /// </summary>
        RGBA16U,

        /// <summary>
        /// 16-bit RGBA color (float).
        /// </summary>
        RGBA16F,

        /// <summary>
        /// 16-bit RGBA color (signed).
        /// </summary>
        RGBA16S,

        /// <summary>
        /// 32-bit RGBA color (integer).
        /// </summary>
        RGBA32I,

        /// <summary>
        /// 32-bit RGBA color (unsigned).
        /// </summary>
        RGBA32U,

        /// <summary>
        /// 32-bit RGBA color (float).
        /// </summary>
        RGBA32F,

        /// <summary>
        /// 5-6-6 color.
        /// </summary>
        R5G6B5,

        /// <summary>
        /// 4-bit RGBA color.
        /// </summary>
        RGBA4,

        /// <summary>
        /// 5-bit RGB color with 1-bit alpha.
        /// </summary>
        RGB5A1,

        /// <summary>
        /// 10-bit RGB color with 2-bit alpha.
        /// </summary>
        RGB10A2,

        /// <summary>
        /// 11-11-10 color (float).
        /// </summary>
        RG11B10F,

        /// <summary>
        /// Unknown depth format.
        /// </summary>
        UnknownDepth,

        /// <summary>
        /// 16-bit depth.
        /// </summary>
        D16,

        /// <summary>
        /// 24-bit depth.
        /// </summary>
        D24,

        /// <summary>
        /// 24-bit depth, 8-bit stencil.
        /// </summary>
        D24S8,

        /// <summary>
        /// 32-bit depth.
        /// </summary>
        D32,

        /// <summary>
        /// 16-bit depth (float).
        /// </summary>
        D16F,

        /// <summary>
        /// 24-bit depth (float).
        /// </summary>
        D24F,

        /// <summary>
        /// 32-bit depth (float).
        /// </summary>
        D32F,

        /// <summary>
        /// 8-bit stencil.
        /// </summary>
        D0S8,

        /// <summary>
        /// The count of all texture formats. Not a valid value.
        /// </summary>
        Count
    }

    /// <summary>
    /// Specifies the type of uniform data.
    /// </summary>
    public enum UniformType {
        /// <summary>
        /// Texture sampler.
        /// </summary>
        Sampler,

        /// <summary>
        /// 4D vector.
        /// </summary>
        Vector4 = 2,

        /// <summary>
        /// 3x3 matrix.
        /// </summary>
        Matrix3x3,

        /// <summary>
        ///4x4 matrix.
        /// </summary>
        Matrix4x4
    }

    /// <summary>
    /// Specifies various settings to change during a reset call.
    /// </summary>
    [Flags]
    public enum ResetFlags {
        /// <summary>
        /// No features to change.
        /// </summary>
        None = 0,

        /// <summary>
        /// Not yet supported.
        /// </summary>
        Fullscreen = 0x1,

        /// <summary>
        /// Enable 2x multisampling.
        /// </summary>
        MSAA2x = 0x10,

        /// <summary>
        /// Enable 4x multisampling.
        /// </summary>
        MSAA4x = 0x20,

        /// <summary>
        /// Enable 8x multisampling.
        /// </summary>
        MSAA8x = 0x30,

        /// <summary>
        /// Enable 16x multisampling.
        /// </summary>
        MSAA16x = 0x40,

        /// <summary>
        /// Enable v-sync.
        /// </summary>
        Vsync = 0x80,

        /// <summary>
        /// Use the maximum anisotropic filtering level available.
        /// </summary>
        MaxAnisotropy = 0x100,

        /// <summary>
        /// Begin screen capture.
        /// </summary>
        Capture = 0x200,

        /// <summary>
        /// Flush all commands to the device after rendering.
        /// </summary>
        FlushAfterRender = 0x2000,

        /// <summary>
        /// Flip the backbuffer immediately after rendering for reduced latency.
        /// Only useful when multithreading is disabled.
        /// </summary>
        FlipAfterRender = 0x4000,

        /// <summary>
        /// Write data to the backbuffer in non-linear sRGB format.
        /// </summary>
        SrgbBackbuffer = 0x8000,

        /// <summary>
        /// Enable HDR10 rendering.
        /// </summary>
        HDR10 = 0x10000,

        /// <summary>
        /// Enable High-DPI rendering.
        /// </summary>
        HighDPI = 0x20000,

        /// <summary>
        /// Enables depth clamping.
        /// </summary>
        DepthClamp = 0x40000,

        /// <summary>
        /// Suspends rendering.
        /// </summary>
        Suspend = 0x80000
    }

    /// <summary>
    /// Specifies various debug options.
    /// </summary>
    [Flags]
    public enum DebugFeatures {
        /// <summary>
        /// Don't enable any debugging features.
        /// </summary>
        None = 0,

        /// <summary>
        /// Force wireframe rendering.
        /// </summary>
        Wireframe = 0x1,

        /// <summary>
        /// When set, all rendering calls are skipped. This is useful when profiling to
        /// find bottlenecks between the CPU and GPU.
        /// </summary>
        InfinitelyFastHardware = 0x2,

        /// <summary>
        /// Display internal statistics.
        /// </summary>
        DisplayStatistics = 0x4,

        /// <summary>
        /// Display debug text.
        /// </summary>
        DisplayText = 0x8,

        /// <summary>
        /// Enable the internal library performance profiler.
        /// </summary>
        Profiler = 0x10
    }

    /// <summary>
    /// Specifies flags for clearing surfaces.
    /// </summary>
    [Flags]
    public enum ClearTargets : short {
        /// <summary>
        /// Don't clear anything.
        /// </summary>
        None = 0,

        /// <summary>
        /// Clear the color channels.
        /// </summary>
        Color = 0x1,

        /// <summary>
        /// Clear the depth buffer.
        /// </summary>
        Depth = 0x2,

        /// <summary>
        /// Clear the stencil buffer.
        /// </summary>
        Stencil = 0x4,

        /// <summary>
        /// Discard the first color framebuffer.
        /// </summary>
        DiscardColor0 = 0x8,

        /// <summary>
        /// Discard the second color framebuffer.
        /// </summary>
        DiscardColor1 = 0x10,

        /// <summary>
        /// Discard the third color framebuffer.
        /// </summary>
        DiscardColor2 = 0x20,

        /// <summary>
        /// Discard the fourth color framebuffer.
        /// </summary>
        DiscardColor3 = 0x40,

        /// <summary>
        /// Discard the fifth color framebuffer.
        /// </summary>
        DiscardColor4 = 0x80,

        /// <summary>
        /// Discard the sixth color framebuffer.
        /// </summary>
        DiscardColor5 = 0x100,

        /// <summary>
        /// Discard the seventh color framebuffer.
        /// </summary>
        DiscardColor6 = 0x200,

        /// <summary>
        /// Discard the eighth color framebuffer.
        /// </summary>
        DiscardColor7 = 0x400,

        /// <summary>
        /// Discard the depth buffer.
        /// </summary>
        DiscardDepth = 0x800,

        /// <summary>
        /// Discard the stencil buffer.
        /// </summary>
        DiscardStencil = 0x1000,
    }

    /// <summary>
    /// Specifies various capabilities supported by the rendering device.
    /// </summary>
    [Flags]
    public enum DeviceFeatures : long {
        /// <summary>
        /// No extra features supported.
        /// </summary>
        None = 0,

        /// <summary>
        /// Device supports alpha to coverage.
        /// </summary>
        AlphaToCoverage = 0x1,

        /// <summary>
        /// Device supports independent blending of simultaneous render targets.
        /// </summary>
        BlendIndependent = 0x2,

        /// <summary>
        /// Device supports compute shaders.
        /// </summary>
        Compute = 0x4,

        /// <summary>
        /// Device supports conservative rasterization.
        /// </summary>
        ConservativeRasterization = 0x8,

        /// <summary>
        /// Device supports indirect drawing via GPU buffers.
        /// </summary>
        DrawIndirect = 0x10,

        /// <summary>
        /// Fragment shaders can access depth values.
        /// </summary>
        FragmentDepth = 0x20,

        /// <summary>
        /// Device supports ordering of fragment output.
        /// </summary>
        FragmentOrdering = 0x40,

        /// <summary>
        /// Read/write framebuffer attachments are supported.
        /// </summary>
        FramebufferRW = 0x80,

        /// <summary>
        /// A graphics debugger is present.
        /// </summary>
        GraphicsDebugger = 0x100,

        /// <summary>
        /// Devices supports HDR10 rendering.
        /// </summary>
        HDR10 = 0x400,

        /// <summary>
        /// Device supports high-DPI rendering.
        /// </summary>
        HighDPI = 0x800,

        /// <summary>
        /// Device supports 32-bit indices.
        /// </summary>
        Index32 = 0x1000,

        /// <summary>
        /// Device supports instancing.
        /// </summary>
        Instancing = 0x2000,

        /// <summary>
        /// Device supports occlusion queries.
        /// </summary>
        OcclusionQuery = 0x4000,

        /// <summary>
        /// Device supports multithreaded rendering.
        /// </summary>
        RendererMultithreaded = 0x8000,

        /// <summary>
        /// Indicates whether the device can render to multiple swap chains.
        /// </summary>
        SwapChain = 0x10000,

        /// <summary>
        /// Device supports 2D texture arrays.
        /// </summary>
        Texture2DArray = 0x20000,

        /// <summary>
        /// Device supports 3D textures.
        /// </summary>
        Texture3D = 0x40000,

        /// <summary>
        /// Device supports texture blits.
        /// </summary>
        TextureBlit = 0x80000,

        /// <summary>
        /// Device supports other texture comparison modes.
        /// </summary>
        TextureCompareExtended = 0x100000,

        /// <summary>
        /// Device supports "Less than or equal to" texture comparison mode.
        /// </summary>
        TextureCompareLessEqual = 0x200000,

        ///// <summary>
        ///// Device supports all texture comparison modes.
        ///// </summary>
        //TextureCompareAll = TextureCompareLessEqual | TextureCompareExtended,

        /// <summary>
        /// Device supports cubemap texture arrays.
        /// </summary>
        TextureCubeArray = 0x400000,

        /// <summary>
        /// Device supports directly accessing texture data.
        /// </summary>
        TextureDirectAccess = 0x800000,

        /// <summary>
        /// Device supports reading back texture data.
        /// </summary>
        TextureReadBack = 0x1000000,

        /// <summary>
        /// Device supports 16-bit floats as vertex attributes.
        /// </summary>
        VertexAttributeHalf = 0x2000000,

        /// <summary>
        /// UInt10 vertex attributes are supported.
        /// </summary>
        VertexAttributeUInt10 = 0x4000000,

        /// <summary>
        /// Devices supports rendering with VertexID only.
        /// </summary>
        VertexID = 0x8000000
    }

    /// <summary>
    /// Indicates the level of support for a specific texture format.
    /// </summary>
    [Flags]
    public enum TextureFormatSupport {
        /// <summary>
        /// The format is unsupported.
        /// </summary>
        Unsupported = 0x0,

        /// <summary>
        /// The format is supported for 2D color data and operations.
        /// </summary>
        Color2D = 0x1,

        /// <summary>
        /// The format is supported for 2D sRGB operations.
        /// </summary>
        Srgb2D = 0x2,

        /// <summary>
        /// The format is supported for 2D textures through library emulation.
        /// </summary>
        Emulated2D = 0x4,

        /// <summary>
        /// The format is supported for 3D color data and operations.
        /// </summary>
        Color3D = 0x8,

        /// <summary>
        /// The format is supported for 3D sRGB operations.
        /// </summary>
        Srgb3D = 0x10,

        /// <summary>
        /// The format is supported for 3D textures through library emulation.
        /// </summary>
        Emulated3D = 0x20,

        /// <summary>
        /// The format is supported for cube color data and operations.
        /// </summary>
        ColorCube = 0x40,

        /// <summary>
        /// The format is supported for cube sRGB operations.
        /// </summary>
        SrgbCube = 0x80,

        /// <summary>
        /// The format is supported for cube textures through library emulation.
        /// </summary>
        EmulatedCube = 0x100,

        /// <summary>
        /// The format is supported for vertex texturing.
        /// </summary>
        Vertex = 0x200,

        /// <summary>
        /// The format is supported for compute image operations.
        /// </summary>
        Image = 0x400,

        /// <summary>
        /// The format is supported for framebuffers.
        /// </summary>
        Framebuffer = 0x800,

        /// <summary>
        /// The format is supported for MSAA framebuffers.
        /// </summary>
        FramebufferMSAA = 0x1000,

        /// <summary>
        /// The format is supported for MSAA sampling.
        /// </summary>
        MSAA = 0x2000,

        /// <summary>
        /// The format supports auto-generated mipmaps.
        /// </summary>
        MipsAutogen = 0x4000
    }

    /// <summary>
    /// Specifies various texture flags.
    /// </summary>
    [Flags]
    public enum TextureFlags : long {
        /// <summary>
        /// No flags set.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Mirror the texture in the U coordinate.
        /// </summary>
        MirrorU = 0x00000001,

        /// <summary>
        /// Clamp the texture in the U coordinate.
        /// </summary>
        ClampU = 0x00000002,

        /// <summary>
        /// Use a border color for addresses outside the range in the U coordinate.
        /// </summary>
        BorderU = 0x00000003,

        /// <summary>
        /// Mirror the texture in the V coordinate.
        /// </summary>
        MirrorV = 0x00000004,

        /// <summary>
        /// Clamp the texture in the V coordinate.
        /// </summary>
        ClampV = 0x00000008,

        /// <summary>
        /// Use a border color for addresses outside the range in the V coordinate.
        /// </summary>
        BorderV = 0x0000000c,

        /// <summary>
        /// Mirror the texture in the W coordinate.
        /// </summary>
        MirrorW = 0x00000010,

        /// <summary>
        /// Clamp the texture in the W coordinate.
        /// </summary>
        ClampW = 0x00000020,

        /// <summary>
        /// Use a border color for addresses outside the range in the W coordinate.
        /// </summary>
        BorderW = 0x00000030,

        /// <summary>
        /// Mirror the texture in the U,V, and W coordinates.
        /// </summary>
        MirrorUVW = MirrorU | MirrorV | MirrorW,

        /// <summary>
        /// Clamp the texture in the U,V, and W coordinates.
        /// </summary>
        ClampUVW = ClampU | ClampV | ClampW,

        /// <summary>
        /// Use a border color for addresses outside the range in the U,V, and W coordinates.
        /// </summary>
        BorderUVW = BorderU | BorderV | BorderW,

        /// <summary>
        /// Use point filtering for texture minification.
        /// </summary>
        MinFilterPoint = 0x00000040,

        /// <summary>
        /// Use anisotropic filtering for texture minification.
        /// </summary>
        MinFilterAnisotropic = 0x00000080,

        /// <summary>
        /// Use point filtering for texture magnification.
        /// </summary>
        MagFilterPoint = 0x00000100,

        /// <summary>
        /// Use anisotropic filtering for texture magnification.
        /// </summary>
        MagFilterAnisotropic = 0x00000200,

        /// <summary>
        /// Use point filtering for texture mipmaps.
        /// </summary>
        MipFilterPoint = 0x00000400,

        /// <summary>
        /// Use point filtering for minification, magnification, and texture mipmaps.
        /// </summary>
        FilterPoint = MinFilterPoint | MagFilterPoint | MipFilterPoint,

        /// <summary>
        /// Use a "less than" operator when comparing textures.
        /// </summary>
        CompareLess = 0x00010000,

        /// <summary>
        /// Use a "less than or equal" operator when comparing textures.
        /// </summary>
        CompareLessEqual = 0x00020000,

        /// <summary>
        /// Use an equality operator when comparing textures.
        /// </summary>
        CompareEqual = 0x00030000,

        /// <summary>
        /// Use a "greater than or equal" operator when comparing textures.
        /// </summary>
        CompareGreaterEqual = 0x00040000,

        /// <summary>
        /// Use a "greater than" operator when comparing textures.
        /// </summary>
        CompareGreater = 0x00050000,

        /// <summary>
        /// Use an inequality operator when comparing textures.
        /// </summary>
        CompareNotEqual = 0x00060000,

        /// <summary>
        /// Never compare two textures as equal.
        /// </summary>
        CompareNever = 0x00070000,

        /// <summary>
        /// Always compare two textures as equal.
        /// </summary>
        CompareAlways = 0x00080000,

        /// <summary>
        /// Sample stencil instead of depth.
        /// </summary>
        SampleStencil = 0x100000,

        /// <summary>
        /// Perform MSAA sampling on the texture.
        /// </summary>
        MSAASample = 0x800000000,

        /// <summary>
        /// The texture will be used as a render target.
        /// </summary>
        RenderTarget = 0x1000000000,

        /// <summary>
        /// The render target texture support 2x multisampling.
        /// </summary>
        RenderTargetMultisample2x = 0x2000000000,

        /// <summary>
        /// The render target texture support 4x multisampling.
        /// </summary>
        RenderTargetMultisample4x = 0x3000000000,

        /// <summary>
        /// The render target texture support 8x multisampling.
        /// </summary>
        RenderTargetMultisample8x = 0x4000000000,

        /// <summary>
        /// The render target texture support 16x multisampling.
        /// </summary>
        RenderTargetMultisample16x = 0x5000000000,

        /// <summary>
        /// The texture is only writeable (render target).
        /// </summary>
        RenderTargetWriteOnly = 0x8000000000,

        /// <summary>
        /// Texture is the target of compute shader writes.
        /// </summary>
        ComputeWrite = 0x100000000000,

        /// <summary>
        /// Texture data is in non-linear sRGB format.
        /// </summary>
        Srgb = 0x200000000000,

        /// <summary>
        /// Texture can be used as the destination of a blit operation.
        /// </summary>
        BlitDestination = 0x400000000000,

        /// <summary>
        /// Texture data can be read back.
        /// </summary>
        ReadBack = 0x800000000000
    }

    /// <summary>
    /// Describes access rights for a compute buffer.
    /// </summary>
    public enum ComputeBufferAccess {
        /// <summary>
        /// The buffer can only be read.
        /// </summary>
        Read,

        /// <summary>
        /// The buffer can only be written to.
        /// </summary>
        Write,

        /// <summary>
        /// The buffer can be read and written.
        /// </summary>
        ReadWrite
    }

    /// <summary>
    /// Flags that control frame buffer resolve.
    /// </summary>
    public enum ResolveFlags : byte {
        /// <summary>
        /// No particular flags specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Automatically generate mipmaps.
        /// </summary>
        AutoGenMips = 0x1
    }

    /// <summary>
    /// Addresses a particular face of a cube map.
    /// </summary>
    public enum CubeMapFace : byte {
        /// <summary>
        /// The right face.
        /// </summary>
        Right,

        /// <summary>
        /// The left face.
        /// </summary>
        Left,

        /// <summary>
        /// The top face.
        /// </summary>
        Top,

        /// <summary>
        /// The bottom face.
        /// </summary>
        Bottom,

        /// <summary>
        /// The front face.
        /// </summary>
        Front,

        /// <summary>
        /// The back face.
        /// </summary>
        Back
    }

    /// <summary>
    /// Specifies known vendor IDs.
    /// </summary>
    public enum Vendor {
        /// <summary>
        /// No vendor specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Special flag to use platform's software rasterizer, if available.
        /// </summary>
        SoftwareRasterizer = 0x1,

        /// <summary>
        /// AMD
        /// </summary>
        AMD = 0x1002,

        /// <summary>
        /// Intel
        /// </summary>
        Intel = 0x8086,

        /// <summary>
        /// NVIDIA
        /// </summary>
        Nvidia = 0x10de,

        /// <summary>
        /// Microsoft
        /// </summary>
        Microsoft = 0x1414
    }

    /// <summary>
    /// Specifies scaling relative to the size of the backbuffer.
    /// </summary>
    public enum BackbufferRatio {
        /// <summary>
        /// Surface is equal to the backbuffer size.
        /// </summary>
        Equal,

        /// <summary>
        /// Surface is half the backbuffer size.
        /// </summary>
        Half,

        /// <summary>
        /// Surface is a quater of the backbuffer size.
        /// </summary>
        Quater,

        /// <summary>
        /// Surface is an eighth of the backbuffer size.
        /// </summary>
        Eighth,

        /// <summary>
        /// Surface is a sixteenth of the backbuffer size.
        /// </summary>
        Sixteenth,

        /// <summary>
        /// Surface is double the backbuffer size.
        /// </summary>
        Double
    }

    /// <summary>
    /// Specifies various flags that control vertex and index buffer behavior.
    /// </summary>
    [Flags]
    public enum BufferFlags : short {
        /// <summary>
        /// No flags specified.
        /// </summary>
        None,

        /// <summary>
        /// Specifies the format of data in a compute buffer as being 8x1.
        /// </summary>
        ComputeFormat8x1 = 0x1,

        /// <summary>
        /// Specifies the format of data in a compute buffer as being 8x2.
        /// </summary>
        ComputeFormat8x2 = 0x2,

        /// <summary>
        /// Specifies the format of data in a compute buffer as being 8x4.
        /// </summary>
        ComputeFormat8x4 = 0x3,

        /// <summary>
        /// Specifies the format of data in a compute buffer as being 16x1.
        /// </summary>
        ComputeFormat16x1 = 0x4,

        /// <summary>
        /// Specifies the format of data in a compute buffer as being 16x2.
        /// </summary>
        ComputeFormat16x2 = 0x5,

        /// <summary>
        /// Specifies the format of data in a compute buffer as being 16x4.
        /// </summary>
        ComputeFormat16x4 = 0x6,

        /// <summary>
        /// Specifies the format of data in a compute buffer as being 32x1.
        /// </summary>
        ComputeFormat32x1 = 0x7,

        /// <summary>
        /// Specifies the format of data in a compute buffer as being 32x2.
        /// </summary>
        ComputeFormat32x2 = 0x8,

        /// <summary>
        /// Specifies the format of data in a compute buffer as being 32x4.
        /// </summary>
        ComputeFormat32x4 = 0x9,

        /// <summary>
        /// Specifies the type of data in a compute buffer as being unsigned integers.
        /// </summary>
        ComputeTypeInt = 0x10,

        /// <summary>
        /// Specifies the type of data in a compute buffer as being signed integers.
        /// </summary>
        ComputeTypeUInt = 0x20,

        /// <summary>
        /// Specifies the type of data in a compute buffer as being floating point values.
        /// </summary>
        ComputeTypeFloat = 0x30,

        /// <summary>
        /// Buffer will be read by a compute shader.
        /// </summary>
        ComputeRead = 0x100,

        /// <summary>
        /// Buffer will be written into by a compute shader. It cannot be accessed by the CPU.
        /// </summary>
        ComputeWrite = 0x200,

        /// <summary>
        /// Buffer is the source of indirect draw commands.
        /// </summary>
        DrawIndirect = 0x400,

        /// <summary>
        /// Buffer will resize on update if a different quantity of data is passed. If this flag is not set
        /// the data will be trimmed to fit in the existing buffer size. Effective only for dynamic buffers.
        /// </summary>
        AllowResize = 0x800,

        /// <summary>
        /// Buffer is using 32-bit indices. Useful only for index buffers.
        /// </summary>
        Index32 = 0x1000,

        /// <summary>
        /// Buffer will be read and written by a compute shader.
        /// </summary>
        ComputeReadWrite = ComputeRead | ComputeWrite
    }

    /// <summary>
    /// Specifies various error types that can be reported by bgfx.
    /// </summary>
    public enum ErrorType {
        /// <summary>
        /// A debug check failed; the program can safely continue, but the issue should be investigated.
        /// </summary>
        DebugCheck,

        /// <summary>
        /// The program tried to compile an invalid shader.
        /// </summary>
        InvalidShader,

        /// <summary>
        /// An error occurred during bgfx library initialization.
        /// </summary>
        UnableToInitialize,

        /// <summary>
        /// Failed while trying to create a texture.
        /// </summary>
        UnableToCreateTexture,

        /// <summary>
        /// The graphics device was lost and the library was unable to recover.
        /// </summary>
        DeviceLost
    }

    /// <summary>
    /// Specifies debug text colors.
    /// </summary>
    public enum DebugColor {
        /// <summary>
        /// Black.
        /// </summary>
        Black,

        /// <summary>
        /// Blue.
        /// </summary>
        Blue,

        /// <summary>
        /// Green.
        /// </summary>
        Green,

        /// <summary>
        /// Cyan.
        /// </summary>
        Cyan,

        /// <summary>
        /// Red.
        /// </summary>
        Red,

        /// <summary>
        /// Magenta.
        /// </summary>
        Magenta,

        /// <summary>
        /// Brown.
        /// </summary>
        Brown,

        /// <summary>
        /// Light gray.
        /// </summary>
        LightGray,

        /// <summary>
        /// Dark gray.
        /// </summary>
        DarkGray,

        /// <summary>
        /// Light blue.
        /// </summary>
        LightBlue,

        /// <summary>
        /// Light green.
        /// </summary>
        LightGreen,

        /// <summary>
        /// Light cyan.
        /// </summary>
        LightCyan,

        /// <summary>
        /// Light red.
        /// </summary>
        LightRed,

        /// <summary>
        /// Light magenta.
        /// </summary>
        LightMagenta,

        /// <summary>
        /// Yellow.
        /// </summary>
        Yellow,

        /// <summary>
        /// White.
        /// </summary>
        White
    }

    /// <summary>
    /// Specifies results of an occlusion query.
    /// </summary>
    public enum OcclusionQueryResult {
        /// <summary>
        /// Objects are invisible.
        /// </summary>
        Invisible,

        /// <summary>
        /// Objects are visible.
        /// </summary>
        Visible,

        /// <summary>
        /// Result is not ready or is unknown.
        /// </summary>
        NoResult
    }

    /// <summary>
    /// Specifies results of manually rendering a single frame.
    /// </summary>
    public enum RenderFrameResult {
        /// <summary>
        /// No device context has been created yet.
        /// </summary>
        NoContext,

        /// <summary>
        /// The frame was rendered.
        /// </summary>
        Render,

        /// <summary>
        /// The internal semaphore timed out; rendering was skipped.
        /// </summary>
        Timeout,

        /// <summary>
        /// Rendering is done; the program should exit.
        /// </summary>
        Exiting
    }

    /// <summary>
    /// Specifies possible sorting modes for a view.
    /// </summary>
    public enum ViewMode {
        /// <summary>
        /// Default sorting method.
        /// </summary>
        Default,

        /// <summary>
        /// Do each draw in the order it is issued.
        /// </summary>
        Sequential,

        /// <summary>
        /// Sort each draw by depth, ascending.
        /// </summary>
        DepthAscending,

        /// <summary>
        /// Sort each draw by depth, descending.
        /// </summary>
        DepthDescending
    }

    //!!!!betauser
    public enum TopologyConvert
    {
        TriListFlipWinding,
        TriStripFlipWinding,
        TriListToLineList,
        TriStripToTriList,
        LineStripToLineList,
    }

    /// <summary>
    /// Specifies possible primitive topologies.
    /// </summary>
    public enum Topology {
        /// <summary>
        /// List of triangles.
        /// </summary>
        TriangleList,

        /// <summary>
        /// Strip of triangles.
        /// </summary>
        TriangleStrip,

        /// <summary>
        /// List of lines.
        /// </summary>
        LineList,

        /// <summary>
        /// Strip of lines.
        /// </summary>
        LineStrip,

        /// <summary>
        /// List of points.
        /// </summary>
        PointList
    }

    //!!!!betauser
    /// <summary>
    /// Rendering state discard. When state is preserved in submit, rendering states can be discarded on a finer grain.
    /// </summary>
    [Flags]
    public enum DiscardFlags : byte
    {
        None                   = (0x00), //!< Preserve everything.
        Bindings = (0x01), //!< Discard texture sampler and buffer bindings.
        IndexBuffer = (0x02), //!< Discard index buffer.
        InstanceData = ( 0x04 ), //!< Discard instance data.
        State = (0x08), //!< Discard state.
        Transform = (0x10), //!< Discard transform.
        VertexStreams = (0x20), //!< Discard vertex streams.
        All = (0xff), //!< Discard all states.
    }

}
