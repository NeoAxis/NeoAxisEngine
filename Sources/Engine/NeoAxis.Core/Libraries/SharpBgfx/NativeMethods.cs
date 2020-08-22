using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpBgfx {
    [SuppressUnmanagedCodeSecurity]
    unsafe static class NativeMethods {
#pragma warning disable IDE1006 // Naming Styles

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_update_texture_2d (ushort handle, ushort layer, byte mip, ushort x, ushort y, ushort width, ushort height, MemoryBlock.DataPtr* memory, ushort pitch);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_update_texture_3d (ushort handle, byte mip, ushort x, ushort y, ushort z, ushort width, ushort height, ushort depth, MemoryBlock.DataPtr* memory);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_update_texture_cube (ushort handle, ushort layer, CubeMapFace side, byte mip, ushort x, ushort y, ushort width, ushort height, MemoryBlock.DataPtr* memory, ushort pitch);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bgfx_get_avail_transient_index_buffer (int num);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bgfx_get_avail_transient_vertex_buffer (int num, ref VertexLayout.Data decl);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bgfx_get_avail_instance_data_buffer (int num, ushort stride);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_alloc_transient_index_buffer (out TransientIndexBuffer tib, int num);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_alloc_transient_vertex_buffer (out TransientVertexBuffer tvb, int num, ref VertexLayout.Data decl);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool bgfx_alloc_transient_buffers (out TransientVertexBuffer tvb, ref VertexLayout.Data decl, ushort numVertices, out TransientIndexBuffer tib, ushort numIndices);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_alloc_instance_data_buffer (out InstanceDataBuffer.NativeStruct ptr, int num, ushort stride);

        //!!!!betauser
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bgfx_dispatch( ushort id, ushort program, uint numX, uint numY, uint numZ, [MarshalAs( UnmanagedType.U1 )] DiscardFlags flags );
        //[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int bgfx_dispatch (ushort id, ushort program, uint numX, uint numY, uint numZ);

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern int bgfx_dispatch_indirect( ushort id, ushort program, ushort indirectHandle, ushort start, ushort num, [MarshalAs( UnmanagedType.U1 )] DiscardFlags flags );
        //[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int bgfx_dispatch_indirect (ushort id, ushort program, ushort indirectHandle, ushort start, ushort num);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_texture (byte stage, ushort sampler, ushort texture, uint flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_image (byte stage, ushort texture, byte mip, TextureFormat format, ComputeBufferAccess access);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_compute_index_buffer (byte stage, ushort handle, ComputeBufferAccess access);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_compute_vertex_buffer (byte stage, ushort handle, ComputeBufferAccess access);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_compute_dynamic_index_buffer (byte stage, ushort handle, ComputeBufferAccess access);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_compute_dynamic_vertex_buffer (byte stage, ushort handle, ComputeBufferAccess access);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_compute_indirect_buffer (byte stage, ushort handle, ComputeBufferAccess access);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_frame_buffer (ushort width, ushort height, TextureFormat format, TextureFlags flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_frame_buffer_name(ushort handle, [MarshalAs(UnmanagedType.LPStr)] string name, int len);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_frame_buffer_scaled (BackbufferRatio ratio, TextureFormat format, TextureFlags flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_frame_buffer_from_handles (byte count, ushort* handles, [MarshalAs(UnmanagedType.U1)] bool destroyTextures);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_frame_buffer_from_attachment (byte count, FrameBuffer.NativeAttachment* attachment, [MarshalAs(UnmanagedType.U1)] bool destroyTextures);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_frame_buffer_from_nwh (IntPtr nwh, ushort width, ushort height, TextureFormat format, TextureFormat depthFormat);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_destroy_frame_buffer (ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_view_frame_buffer (ushort id, ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_program (ushort vsh, ushort fsh, [MarshalAs(UnmanagedType.U1)] bool destroyShaders);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_compute_program (ushort csh, [MarshalAs(UnmanagedType.U1)] bool destroyShaders);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_destroy_program (ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Capabilities.Caps* bgfx_get_caps ();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_vertex_layout_begin (ref VertexLayout.Data decl, RendererBackend backend);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_vertex_layout_add (ref VertexLayout.Data decl, VertexAttributeUsage attribute, byte count, VertexAttributeType type, [MarshalAs(UnmanagedType.U1)] bool normalized, [MarshalAs(UnmanagedType.U1)] bool asInt);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_vertex_layout_skip (ref VertexLayout.Data decl, byte count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_vertex_layout_end (ref VertexLayout.Data decl);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_vertex_buffer (MemoryBlock.DataPtr* memory, ref VertexLayout.Data decl, BufferFlags flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_vertex_buffer_name(ushort handle, [MarshalAs(UnmanagedType.LPStr)] string name, int len);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_destroy_vertex_buffer (ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_uniform ([MarshalAs(UnmanagedType.LPStr)] string name, UniformType type, ushort arraySize);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_get_uniform_info (ushort handle, out Uniform.Info info);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_destroy_uniform (ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_texture (MemoryBlock.DataPtr* mem, TextureFlags flags, byte skip, out Texture.TextureInfo info);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_texture_2d (ushort width, ushort _height, [MarshalAs(UnmanagedType.U1)] bool hasMips, ushort numLayers, TextureFormat format, TextureFlags flags, MemoryBlock.DataPtr* mem);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_texture_2d_scaled (BackbufferRatio ratio, [MarshalAs(UnmanagedType.U1)] bool hasMips, ushort numLayers, TextureFormat format, TextureFlags flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_texture_3d (ushort width, ushort _height, ushort _depth, [MarshalAs(UnmanagedType.U1)] bool hasMips, TextureFormat format, TextureFlags flags, MemoryBlock.DataPtr* mem);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_texture_cube (ushort size, [MarshalAs(UnmanagedType.U1)] bool hasMips, ushort numLayers, TextureFormat format, TextureFlags flags, MemoryBlock.DataPtr* mem);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_texture_name(ushort handle, [MarshalAs(UnmanagedType.LPStr)] string name, int len);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bgfx_get_direct_access_ptr (ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_destroy_texture (ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_calc_texture_size (ref Texture.TextureInfo info, ushort width, ushort height, ushort depth, [MarshalAs(UnmanagedType.U1)] bool cubeMap, [MarshalAs(UnmanagedType.U1)] bool hasMips, ushort numLayers, TextureFormat format);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool bgfx_is_texture_valid (ushort depth, [MarshalAs(UnmanagedType.U1)] bool cubeMap, ushort numLayers, TextureFormat format, TextureFlags flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_shader (MemoryBlock.DataPtr* memory);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_get_shader_uniforms (ushort handle, Uniform[] uniforms, ushort max);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_shader_name(ushort handle, [MarshalAs(UnmanagedType.LPStr)] string name, int len);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_destroy_shader (ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern MemoryBlock.DataPtr* bgfx_alloc (int size);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern MemoryBlock.DataPtr* bgfx_copy (IntPtr data, int size);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern MemoryBlock.DataPtr* bgfx_make_ref_release (IntPtr data, int size, IntPtr releaseFn, IntPtr userData);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_index_buffer (MemoryBlock.DataPtr* memory, BufferFlags flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_index_buffer_name(ushort handle, [MarshalAs(UnmanagedType.LPStr)] string name, int len);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_destroy_index_buffer (ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_dynamic_index_buffer (int indexCount, BufferFlags flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_dynamic_index_buffer_mem (MemoryBlock.DataPtr* memory, BufferFlags flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_update_dynamic_index_buffer (ushort handle, int startIndex, MemoryBlock.DataPtr* memory);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_destroy_dynamic_index_buffer (ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_dynamic_vertex_buffer (int vertexCount, ref VertexLayout.Data decl, BufferFlags flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_dynamic_vertex_buffer_mem (MemoryBlock.DataPtr* memory, ref VertexLayout.Data decl, BufferFlags flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_update_dynamic_vertex_buffer (ushort handle, int startVertex, MemoryBlock.DataPtr* memory);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_destroy_dynamic_vertex_buffer (ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_indirect_buffer (int size);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_destroy_indirect_buffer (ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_image_swizzle_bgra8 (IntPtr dst, int width, int height, int pitch, IntPtr src);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_image_rgba8_downsample_2x2 (IntPtr dst, int width, int height, int pitch, IntPtr src);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_platform_data (ref PlatformData data);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern InternalData* bgfx_get_internal_data ();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern RenderFrameResult bgfx_render_frame (int timeoutMs);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bgfx_override_internal_texture_ptr (ushort handle, IntPtr ptr);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bgfx_override_internal_texture (ushort handle, ushort width, ushort height, byte numMips, TextureFormat format, TextureFlags flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern PerfStats.Stats* bgfx_get_stats ();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern RendererBackend bgfx_get_renderer_type ();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_shutdown ();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_reset (int width, int height, ResetFlags flags, TextureFormat format);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bgfx_frame ([MarshalAs(UnmanagedType.U1)] bool capture);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_debug (DebugFeatures flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_marker ([MarshalAs(UnmanagedType.LPStr)] string marker);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_dbg_text_clear (byte color, [MarshalAs(UnmanagedType.U1)] bool smallText);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bgfx_set_transform (float* matrix, ushort count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_stencil (uint frontFace, uint backFace);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bgfx_touch (ushort id);

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern int bgfx_submit( ushort id, ushort programHandle, int depth, [MarshalAs( UnmanagedType.U1 )] DiscardFlags flags );
        //[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int bgfx_submit (ushort id, ushort programHandle, int depth, [MarshalAs(UnmanagedType.U1)] bool preserveState);

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern int bgfx_submit_occlusion_query( ushort id, ushort programHandle, ushort queryHandle, int depth, [MarshalAs( UnmanagedType.U1 )] DiscardFlags flags );
        //[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int bgfx_submit_occlusion_query (ushort id, ushort programHandle, ushort queryHandle, int depth, [MarshalAs(UnmanagedType.U1)] bool preserveState);

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern int bgfx_submit_indirect( ushort id, ushort programHandle, ushort indirectHandle, ushort start, ushort num, int depth, [MarshalAs( UnmanagedType.U1 )] DiscardFlags flags );
        //[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int bgfx_submit_indirect (ushort id, ushort programHandle, ushort indirectHandle, ushort start, ushort num, int depth, [MarshalAs(UnmanagedType.U1)] bool preserveState);

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern void bgfx_discard( [MarshalAs( UnmanagedType.U1 )] DiscardFlags flags );
        //[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern void bgfx_discard ();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_view_name (ushort id, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_view_rect (ushort id, ushort x, ushort y, ushort width, ushort height);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_view_rect_auto (ushort id, ushort x, ushort y, BackbufferRatio ratio);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_view_scissor (ushort id, ushort x, ushort y, ushort width, ushort height);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_view_clear (ushort id, ClearTargets flags, uint rgba, float depth, byte stencil);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_view_clear_mrt (ushort id, ClearTargets flags, float depth, byte stencil, byte rt0, byte rt1, byte rt2, byte rt3, byte rt4, byte rt5, byte rt6, byte rt7);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_palette_color (byte index, float* color);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_view_mode (ushort id, ViewMode mode);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_view_transform (ushort id, float* view, float* proj);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_request_screen_shot (ushort handle, [MarshalAs(UnmanagedType.LPStr)] string filePath);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_state (ulong state, int rgba);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_vertex_pack (float* input, [MarshalAs(UnmanagedType.U1)] bool inputNormalized, VertexAttributeUsage attribute, ref VertexLayout.Data decl, IntPtr data, int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_vertex_unpack (float* output, VertexAttributeUsage attribute, ref VertexLayout.Data decl, IntPtr data, int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_vertex_convert (ref VertexLayout.Data destDecl, IntPtr destData, ref VertexLayout.Data srcDecl, IntPtr srcData, int num);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_weld_vertices (ushort* output, ref VertexLayout.Data decl, IntPtr data, ushort num, float epsilon);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte bgfx_get_supported_renderers (byte max, RendererBackend[] backends);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte* bgfx_get_renderer_name (RendererBackend backend);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_init_ctor (InitSettings.Native* ptr);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool bgfx_init (InitSettings.Native* ptr);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_dbg_text_printf (ushort x, ushort y, byte color, [MarshalAs(UnmanagedType.LPStr)] string format, [MarshalAs(UnmanagedType.LPStr)] string args);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_dbg_text_printf (ushort x, ushort y, byte color, byte* format, IntPtr args);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_dbg_text_image (ushort x, ushort y, ushort width, ushort height, IntPtr data, ushort pitch);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_index_buffer (ushort handle, int firstIndex, int count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_dynamic_index_buffer (ushort handle, int firstIndex, int count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_vertex_buffer (byte stream, ushort handle, int startVertex, int count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_dynamic_vertex_buffer (byte stream, ushort handle, int startVertex, int count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_uniform (ushort handle, void* value, ushort arraySize);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_transform_cached (int cache, ushort num);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_set_scissor (ushort x, ushort y, ushort width, ushort height);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_scissor_cached (ushort cache);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_transient_vertex_buffer (byte stream, ref TransientVertexBuffer tvb, int startVertex, int numVertices);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_transient_index_buffer (ref TransientIndexBuffer tib, int startIndex, int numIndices);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_vertex_count (int numVertices);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_instance_count (int numInstances);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_instance_data_buffer (ref InstanceDataBuffer.NativeStruct idb, uint start, uint num);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_instance_data_from_vertex_buffer (ushort handle, int startVertex, int count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_instance_data_from_dynamic_vertex_buffer (ushort handle, int startVertex, int count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_reset_view (ushort id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_blit (ushort id, ushort dst, byte dstMip, ushort dstX, ushort dstY, ushort dstZ, ushort src,
                                             byte srcMip, ushort srcX, ushort srcY, ushort srcZ, ushort width, ushort height, ushort depth);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint bgfx_read_texture (ushort handle, IntPtr data, byte mip);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_get_texture (ushort handle, byte attachment);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_create_occlusion_query ();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_destroy_occlusion_query (ushort handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern OcclusionQueryResult bgfx_get_result (ushort handle, int* pixels);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_set_condition (ushort handle, [MarshalAs(UnmanagedType.U1)] bool visible);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bgfx_vsnprintf(sbyte* str, IntPtr count, [MarshalAs(UnmanagedType.LPStr)] string format, IntPtr argList);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bgfx_begin ();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_end (IntPtr encoder);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_marker (IntPtr encoder, [MarshalAs(UnmanagedType.LPStr)] string marker);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_state (IntPtr encoder, ulong state, int rgba);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_condition (IntPtr encoder, ushort handle, [MarshalAs(UnmanagedType.U1)] bool visible);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_stencil(IntPtr encoder, uint frontFace, uint backFace);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort bgfx_encoder_set_scissor(IntPtr encoder, ushort x, ushort y, ushort width, ushort height);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_scissor_cached(IntPtr encoder, ushort cache);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bgfx_encoder_set_transform(IntPtr encoder, float* matrix, ushort count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_transform_cached(IntPtr encoder, int cache, ushort num);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_uniform(IntPtr encoder, ushort handle, void* value, ushort arraySize);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_index_buffer(IntPtr encoder, ushort handle, int firstIndex, int count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_dynamic_index_buffer(IntPtr encoder, ushort handle, int firstIndex, int count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_vertex_buffer(IntPtr encoder, byte stream, ushort handle, int startVertex, int count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_dynamic_vertex_buffer(IntPtr encoder, byte stream, ushort handle, int startVertex, int count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_transient_vertex_buffer(IntPtr encoder, byte stream, ref TransientVertexBuffer tvb, int startVertex, int numVertices);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_transient_index_buffer(IntPtr encoder, ref TransientIndexBuffer tib, int startIndex, int numIndices);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_vertex_count (IntPtr encoder, int numVertices);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_instance_data_buffer(IntPtr encoder, ref InstanceDataBuffer.NativeStruct idb, uint start, uint num);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_instance_data_from_vertex_buffer(IntPtr encoder, ushort handle, int startVertex, int count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_instance_data_from_dynamic_vertex_buffer(IntPtr encoder, ushort handle, int startVertex, int count);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_texture(IntPtr encoder, byte stage, ushort sampler, ushort texture, uint flags);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bgfx_encoder_touch(IntPtr encoder, ushort id);

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern int bgfx_encoder_submit( IntPtr encoder, ushort id, ushort programHandle, int depth, [MarshalAs( UnmanagedType.U1 )] DiscardFlags flags );
        //[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int bgfx_encoder_submit(IntPtr encoder, ushort id, ushort programHandle, int depth, [MarshalAs(UnmanagedType.U1)] bool preserveState);

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern int bgfx_encoder_submit_occlusion_query( IntPtr encoder, ushort id, ushort programHandle, ushort queryHandle, int depth, [MarshalAs( UnmanagedType.U1 )] DiscardFlags flags );
        //[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int bgfx_encoder_submit_occlusion_query(IntPtr encoder, ushort id, ushort programHandle, ushort queryHandle, int depth, [MarshalAs(UnmanagedType.U1)] bool preserveState);

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern int bgfx_encoder_submit_indirect( IntPtr encoder, ushort id, ushort programHandle, ushort indirectHandle, ushort start, ushort num, int depth, [MarshalAs( UnmanagedType.U1 )] DiscardFlags flags );
        //[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int bgfx_encoder_submit_indirect(IntPtr encoder, ushort id, ushort programHandle, ushort indirectHandle, ushort start, ushort num, int depth, [MarshalAs(UnmanagedType.U1)] bool preserveState);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_image(IntPtr encoder, byte stage, ushort texture, byte mip, TextureFormat format, ComputeBufferAccess access);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_compute_index_buffer(IntPtr encoder, byte stage, ushort handle, ComputeBufferAccess access);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_compute_vertex_buffer(IntPtr encoder, byte stage, ushort handle, ComputeBufferAccess access);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_compute_dynamic_index_buffer(IntPtr encoder, byte stage, ushort handle, ComputeBufferAccess access);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_compute_dynamic_vertex_buffer(IntPtr encoder, byte stage, ushort handle, ComputeBufferAccess access);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_set_compute_indirect_buffer(IntPtr encoder, byte stage, ushort handle, ComputeBufferAccess access);

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern int bgfx_encoder_dispatch( IntPtr encoder, ushort id, ushort program, uint numX, uint numY, uint numZ, [MarshalAs( UnmanagedType.U1 )] DiscardFlags flags );
        //[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int bgfx_encoder_dispatch(IntPtr encoder, ushort id, ushort program, uint numX, uint numY, uint numZ);

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern int bgfx_encoder_dispatch_indirect( IntPtr encoder, ushort id, ushort program, ushort indirectHandle, ushort start, ushort num, [MarshalAs( UnmanagedType.U1 )] DiscardFlags flags );
        //[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int bgfx_encoder_dispatch_indirect(IntPtr encoder, ushort id, ushort program, ushort indirectHandle, ushort start, ushort num);

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern void bgfx_encoder_discard( IntPtr encoder, [MarshalAs( UnmanagedType.U1 )] DiscardFlags flags );
        //[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern void bgfx_encoder_discard(IntPtr encoder);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bgfx_encoder_blit(IntPtr encoder, ushort id, ushort dst, byte dstMip, ushort dstX, ushort dstY, ushort dstZ, ushort src,
                                                    byte srcMip, ushort srcX, ushort srcY, ushort srcZ, ushort width, ushort height, ushort depth);

        //!!!!betauser
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint bgfx_topology_convert(TopologyConvert conversation, IntPtr dst, uint dstSize, IntPtr indices, uint numIndices, [MarshalAs(UnmanagedType.U1)] bool index32);

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern void bgfx_set_triple_buffering();

        //!!!!betauser
        //!!!!temp
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public unsafe static extern void bgfx_get_gpu_description( int bufferSize, byte* result );

        //!!!!betauser
        [DllImport( DllName, CallingConvention = CallingConvention.Cdecl )]
        public static extern IntPtr bgfx_call_special( [MarshalAs( UnmanagedType.LPStr )] string name, IntPtr parameter1, IntPtr parameter2,
            IntPtr parameter3, IntPtr parameter4 );

#pragma warning restore IDE1006 // Naming Styles

        //!!!!betauser
        public const string DllName = "bgfx";
//#if DEBUG
//        public const string DllName = "bgfx_debug.dll";
//#else
//        public const string DllName = "bgfx.dll";
//#endif
    }
}
