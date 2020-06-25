using System;

namespace SharpBgfx {
    /// <summary>
    /// An interface for encoding a list of commands from multiple threads.
    /// Dispose of the encoder to finish submitting calls from the current thread.
    /// </summary>
    public unsafe struct Encoder : IDisposable, IEquatable<Encoder> {
        internal readonly IntPtr ptr;

        internal Encoder (IntPtr ptr) {
            this.ptr = ptr;
        }

        /// <summary>
        /// Sets a marker that can be used for debugging purposes.
        /// </summary>
        /// <param name="marker">The user-defined name of the marker.</param>
        public void SetDebugMarker (string marker) {
            NativeMethods.bgfx_encoder_set_marker(ptr, marker);
        }

        /// <summary>
        /// Set rendering states used to draw primitives.
        /// </summary>
        /// <param name="state">The set of states to set.</param>
        public void SetRenderState (RenderState state) {
            NativeMethods.bgfx_encoder_set_state(ptr, (ulong)state, 0);
        }

        /// <summary>
        /// Set rendering states used to draw primitives.
        /// </summary>
        /// <param name="state">The set of states to set.</param>
        /// <param name="colorRgba">The color used for "factor" blending modes.</param>
        public void SetRenderState (RenderState state, int colorRgba) {
            NativeMethods.bgfx_encoder_set_state(ptr, (ulong)state, colorRgba);
        }

        /// <summary>
        /// Sets stencil test state.
        /// </summary>
        /// <param name="frontFace">The stencil state to use for front faces.</param>
        public void SetStencil (StencilFlags frontFace) {
            SetStencil(frontFace, StencilFlags.None);
        }

        /// <summary>
        /// Sets stencil test state.
        /// </summary>
        /// <param name="frontFace">The stencil state to use for front faces.</param>
        /// <param name="backFace">The stencil state to use for back faces.</param>
        public void SetStencil (StencilFlags frontFace, StencilFlags backFace) {
            NativeMethods.bgfx_encoder_set_stencil(ptr, (uint)frontFace, (uint)backFace);
        }

        /// <summary>
        /// Sets the scissor rectangle to use for clipping primitives.
        /// </summary>
        /// <param name="x">The X coordinate of the scissor rectangle.</param>
        /// <param name="y">The Y coordinate of the scissor rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <returns>
        /// An index into the scissor cache to allow reusing the rectangle in other calls.
        /// </returns>
        public int SetScissor (int x, int y, int width, int height) {
            return NativeMethods.bgfx_encoder_set_scissor(ptr, (ushort)x, (ushort)y, (ushort)width, (ushort)height);
        }

        /// <summary>
        /// Sets a scissor rectangle from the cache.
        /// </summary>
        /// <param name="cacheIndex">The index of the cached scissor rectangle, or -1 to unset.</param>
        public void SetScissor (int cacheIndex = -1) {
            NativeMethods.bgfx_encoder_set_scissor_cached(ptr, (ushort)cacheIndex);
        }

        /// <summary>
        /// Sets the model transform to use for drawing primitives.
        /// </summary>
        /// <param name="matrix">A pointer to one or more matrices to set.</param>
        /// <param name="count">The number of matrices in the array.</param>
        /// <returns>An index into the matrix cache to allow reusing the matrix in other calls.</returns>
        public int SetTransform (float* matrix, int count = 1) {
            return NativeMethods.bgfx_encoder_set_transform(ptr, matrix, (ushort)count);
        }

        /// <summary>
        /// Sets a model transform from the cache.
        /// </summary>
        /// <param name="cacheIndex">The index of the cached matrix.</param>
        /// <param name="count">The number of matrices to set from the cache.</param>
        public void SetTransform (int cacheIndex, int count = 1) {
            NativeMethods.bgfx_encoder_set_transform_cached(ptr, cacheIndex, (ushort)count);
        }

        /// <summary>
        /// Sets the value of a uniform parameter.
        /// </summary>
        /// <param name="uniform">The uniform to set.</param>
        /// <param name="value">A pointer to the uniform's data.</param>
        /// <param name="arraySize">The size of the data array, if the uniform is an array.</param>
        public void SetUniform (Uniform uniform, float value, int arraySize = 1) {
            NativeMethods.bgfx_encoder_set_uniform(ptr, uniform.handle, &value, (ushort)arraySize);
        }

        /// <summary>
        /// Sets the value of a uniform parameter.
        /// </summary>
        /// <param name="uniform">The uniform to set.</param>
        /// <param name="value">A pointer to the uniform's data.</param>
        /// <param name="arraySize">The size of the data array, if the uniform is an array.</param>
        public void SetUniform (Uniform uniform, void* value, int arraySize = 1) {
            NativeMethods.bgfx_encoder_set_uniform(ptr, uniform.handle, value, (ushort)arraySize);
        }

        /// <summary>
        /// Sets the value of a uniform parameter.
        /// </summary>
        /// <param name="uniform">The uniform to set.</param>
        /// <param name="value">A pointer to the uniform's data.</param>
        /// <param name="arraySize">The size of the data array, if the uniform is an array.</param>
        public void SetUniform (Uniform uniform, IntPtr value, int arraySize = 1) {
            NativeMethods.bgfx_encoder_set_uniform(ptr, uniform.handle, value.ToPointer(), (ushort)arraySize);
        }

        /// <summary>
        /// Sets a texture to use for drawing primitives.
        /// </summary>
        /// <param name="textureUnit">The texture unit to set.</param>
        /// <param name="sampler">The sampler uniform.</param>
        /// <param name="texture">The texture to set.</param>
        public void SetTexture (byte textureUnit, Uniform sampler, Texture texture) {
            NativeMethods.bgfx_encoder_set_texture(ptr, textureUnit, sampler.handle, texture.handle, uint.MaxValue);
        }

        /// <summary>
        /// Sets a texture to use for drawing primitives.
        /// </summary>
        /// <param name="textureUnit">The texture unit to set.</param>
        /// <param name="sampler">The sampler uniform.</param>
        /// <param name="texture">The texture to set.</param>
        /// <param name="flags">Sampling flags that override the default flags in the texture itself.</param>
        public void SetTexture (byte textureUnit, Uniform sampler, Texture texture, TextureFlags flags) {
            NativeMethods.bgfx_encoder_set_texture(ptr, textureUnit, sampler.handle, texture.handle, (uint)flags);
        }

        /// <summary>
        /// Sets the index buffer to use for drawing primitives.
        /// </summary>
        /// <param name="indexBuffer">The index buffer to set.</param>
        public void SetIndexBuffer (IndexBuffer indexBuffer) {
            NativeMethods.bgfx_encoder_set_index_buffer(ptr, indexBuffer.handle, 0, -1);
        }

        /// <summary>
        /// Sets the index buffer to use for drawing primitives.
        /// </summary>
        /// <param name="indexBuffer">The index buffer to set.</param>
        /// <param name="firstIndex">The first index in the buffer to use.</param>
        /// <param name="count">The number of indices to pull from the buffer.</param>
        public void SetIndexBuffer (IndexBuffer indexBuffer, int firstIndex, int count) {
            NativeMethods.bgfx_encoder_set_index_buffer(ptr, indexBuffer.handle, firstIndex, count);
        }

        /// <summary>
        /// Sets the vertex buffer to use for drawing primitives.
        /// </summary>
        /// <param name="stream">The index of the vertex stream to set.</param>
        /// <param name="vertexBuffer">The vertex buffer to set.</param>
        public void SetVertexBuffer (int stream, VertexBuffer vertexBuffer) {
            NativeMethods.bgfx_encoder_set_vertex_buffer(ptr, (byte)stream, vertexBuffer.handle, 0, -1);
        }

        /// <summary>
        /// Sets the vertex buffer to use for drawing primitives.
        /// </summary>
        /// <param name="stream">The index of the vertex stream to set.</param>
        /// <param name="vertexBuffer">The vertex buffer to set.</param>
        /// <param name="firstVertex">The index of the first vertex to use.</param>
        /// <param name="count">The number of vertices to pull from the buffer.</param>
        public void SetVertexBuffer (int stream, VertexBuffer vertexBuffer, int firstVertex, int count) {
            NativeMethods.bgfx_encoder_set_vertex_buffer(ptr, (byte)stream, vertexBuffer.handle, firstVertex, count);
        }

        /// <summary>
        /// Sets the index buffer to use for drawing primitives.
        /// </summary>
        /// <param name="indexBuffer">The index buffer to set.</param>
        public void SetIndexBuffer (DynamicIndexBuffer indexBuffer) {
            NativeMethods.bgfx_encoder_set_dynamic_index_buffer(ptr, indexBuffer.handle, 0, -1);
        }

        /// <summary>
        /// Sets the index buffer to use for drawing primitives.
        /// </summary>
        /// <param name="indexBuffer">The index buffer to set.</param>
        /// <param name="firstIndex">The first index in the buffer to use.</param>
        /// <param name="count">The number of indices to pull from the buffer.</param>
        public void SetIndexBuffer (DynamicIndexBuffer indexBuffer, int firstIndex, int count) {
            NativeMethods.bgfx_encoder_set_dynamic_index_buffer(ptr, indexBuffer.handle, firstIndex, count);
        }

        /// <summary>
        /// Sets the vertex buffer to use for drawing primitives.
        /// </summary>
        /// <param name="stream">The index of the vertex stream to set.</param>
        /// <param name="vertexBuffer">The vertex buffer to set.</param>
        public void SetVertexBuffer (int stream, DynamicVertexBuffer vertexBuffer) {
            NativeMethods.bgfx_encoder_set_dynamic_vertex_buffer(ptr, (byte)stream, vertexBuffer.handle, 0, -1);
        }

        /// <summary>
        /// Sets the vertex buffer to use for drawing primitives.
        /// </summary>
        /// <param name="stream">The index of the vertex stream to set.</param>
        /// <param name="vertexBuffer">The vertex buffer to set.</param>
        /// <param name="startVertex">The index of the first vertex to use.</param>
        /// <param name="count">The number of vertices to pull from the buffer.</param>
        public void SetVertexBuffer (int stream, DynamicVertexBuffer vertexBuffer, int startVertex, int count) {
            NativeMethods.bgfx_encoder_set_dynamic_vertex_buffer(ptr, (byte)stream, vertexBuffer.handle, startVertex, count);
        }

        /// <summary>
        /// Sets the index buffer to use for drawing primitives.
        /// </summary>
        /// <param name="indexBuffer">The index buffer to set.</param>
        public void SetIndexBuffer (TransientIndexBuffer indexBuffer) {
            NativeMethods.bgfx_encoder_set_transient_index_buffer(ptr, ref indexBuffer, 0, -1);
        }

        /// <summary>
        /// Sets the index buffer to use for drawing primitives.
        /// </summary>
        /// <param name="indexBuffer">The index buffer to set.</param>
        /// <param name="firstIndex">The first index in the buffer to use.</param>
        /// <param name="count">The number of indices to pull from the buffer.</param>
        public void SetIndexBuffer (TransientIndexBuffer indexBuffer, int firstIndex, int count) {
            NativeMethods.bgfx_encoder_set_transient_index_buffer(ptr, ref indexBuffer, firstIndex, count);
        }

        /// <summary>
        /// Sets the vertex buffer to use for drawing primitives.
        /// </summary>
        /// <param name="stream">The index of the vertex stream to set.</param>
        /// <param name="vertexBuffer">The vertex buffer to set.</param>
        public void SetVertexBuffer (int stream, TransientVertexBuffer vertexBuffer) {
            NativeMethods.bgfx_encoder_set_transient_vertex_buffer(ptr, (byte)stream, ref vertexBuffer, 0, -1);
        }

        /// <summary>
        /// Sets the vertex buffer to use for drawing primitives.
        /// </summary>
        /// <param name="stream">The index of the vertex stream to set.</param>
        /// <param name="vertexBuffer">The vertex buffer to set.</param>
        /// <param name="firstVertex">The index of the first vertex to use.</param>
        /// <param name="count">The number of vertices to pull from the buffer.</param>
        public void SetVertexBuffer (int stream, TransientVertexBuffer vertexBuffer, int firstVertex, int count) {
            NativeMethods.bgfx_encoder_set_transient_vertex_buffer(ptr, (byte)stream, ref vertexBuffer, firstVertex, count);
        }

        /// <summary>
        /// Sets the number of auto-generated vertices for use with gl_VertexID.
        /// </summary>
        /// <param name="count">The number of auto-generated vertices.</param>
        public void SetVertexCount (int count) {
            NativeMethods.bgfx_encoder_set_vertex_count(ptr, count);
        }

        /// <summary>
        /// Sets instance data to use for drawing primitives.
        /// </summary>
        /// <param name="instanceData">The instance data.</param>
        /// <param name="start">The starting offset in the buffer.</param>
        /// <param name="count">The number of entries to pull from the buffer.</param>
        public void SetInstanceDataBuffer (ref InstanceDataBuffer instanceData, int start = 0, int count = -1) {
            NativeMethods.bgfx_encoder_set_instance_data_buffer(ptr, ref instanceData.data, (uint)start, (uint)count);
        }

        /// <summary>
        /// Sets instance data to use for drawing primitives.
        /// </summary>
        /// <param name="vertexBuffer">The vertex buffer containing instance data.</param>
        /// <param name="firstVertex">The index of the first vertex to use.</param>
        /// <param name="count">The number of vertices to pull from the buffer.</param>
        public void SetInstanceDataBuffer (VertexBuffer vertexBuffer, int firstVertex, int count) {
            NativeMethods.bgfx_encoder_set_instance_data_from_vertex_buffer(ptr, vertexBuffer.handle, firstVertex, count);
        }

        /// <summary>
        /// Sets instance data to use for drawing primitives.
        /// </summary>
        /// <param name="vertexBuffer">The vertex buffer containing instance data.</param>
        /// <param name="firstVertex">The index of the first vertex to use.</param>
        /// <param name="count">The number of vertices to pull from the buffer.</param>
        public void SetInstanceDataBuffer (DynamicVertexBuffer vertexBuffer, int firstVertex, int count) {
            NativeMethods.bgfx_encoder_set_instance_data_from_dynamic_vertex_buffer(ptr, vertexBuffer.handle, firstVertex, count);
        }

        /// <summary>
        /// Marks a view as "touched", ensuring that its background is cleared even if nothing is rendered.
        /// </summary>
        /// <param name="id">The index of the view to touch.</param>
        /// <returns>The number of draw calls.</returns>
        public int Touch (ushort id) {
            return NativeMethods.bgfx_encoder_touch(ptr, id);
        }

        /// <summary>
        /// Submits the current batch of primitives for rendering.
        /// </summary>
        /// <param name="id">The index of the view to submit.</param>
        /// <param name="program">The program with which to render.</param>
        /// <param name="depth">A depth value to use for sorting the batch.</param>
        /// <param name="preserveState"><c>true</c> to preserve internal draw state after the call.</param>
        /// <returns>The number of draw calls.</returns>
        public int Submit (ushort id, Program program, int depth = 0, bool preserveState = false) {
            return NativeMethods.bgfx_encoder_submit(ptr, id, program.handle, depth, preserveState);
        }

        /// <summary>
        /// Submits the current batch of primitives for rendering.
        /// </summary>
        /// <param name="id">The index of the view to submit.</param>
        /// <param name="program">The program with which to render.</param>
        /// <param name="query">An occlusion query to use as a predicate during rendering.</param>
        /// <param name="depth">A depth value to use for sorting the batch.</param>
        /// <param name="preserveState"><c>true</c> to preserve internal draw state after the call.</param>
        /// <returns>The number of draw calls.</returns>
        public int Submit (ushort id, Program program, OcclusionQuery query, int depth = 0, bool preserveState = false) {
            return NativeMethods.bgfx_encoder_submit_occlusion_query(ptr, id, program.handle, query.handle, depth, preserveState);
        }

        /// <summary>
        /// Submits an indirect batch of drawing commands to be used for rendering.
        /// </summary>
        /// <param name="id">The index of the view to submit.</param>
        /// <param name="program">The program with which to render.</param>
        /// <param name="indirectBuffer">The buffer containing drawing commands.</param>
        /// <param name="startIndex">The index of the first command to process.</param>
        /// <param name="count">The number of commands to process from the buffer.</param>
        /// <param name="depth">A depth value to use for sorting the batch.</param>
        /// <param name="preserveState"><c>true</c> to preserve internal draw state after the call.</param>
        /// <returns>The number of draw calls.</returns>
        public int Submit (ushort id, Program program, IndirectBuffer indirectBuffer, int startIndex = 0, int count = 1, int depth = 0, bool preserveState = false) {
            return NativeMethods.bgfx_encoder_submit_indirect(ptr, id, program.handle, indirectBuffer.handle, (ushort)startIndex, (ushort)count, depth, preserveState);
        }

        /// <summary>
        /// Sets a texture mip as a compute image.
        /// </summary>
        /// <param name="stage">The buffer stage to set.</param>
        /// <param name="texture">The texture to set.</param>
        /// <param name="mip">The index of the mip level within the texture to set.</param>
        /// <param name="format">The format of the buffer data.</param>
        /// <param name="access">Access control flags.</param>
        public void SetComputeImage (byte stage, Texture texture, byte mip, ComputeBufferAccess access, TextureFormat format = TextureFormat.Unknown) {
            NativeMethods.bgfx_encoder_set_image(ptr, stage, texture.handle, mip, format, access);
        }

        /// <summary>
        /// Sets an index buffer as a compute resource.
        /// </summary>
        /// <param name="stage">The resource stage to set.</param>
        /// <param name="buffer">The buffer to set.</param>
        /// <param name="access">Access control flags.</param>
        public void SetComputeBuffer (byte stage, IndexBuffer buffer, ComputeBufferAccess access) {
            NativeMethods.bgfx_encoder_set_compute_index_buffer(ptr, stage, buffer.handle, access);
        }

        /// <summary>
        /// Sets a verterx buffer as a compute resource.
        /// </summary>
        /// <param name="stage">The resource stage to set.</param>
        /// <param name="buffer">The buffer to set.</param>
        /// <param name="access">Access control flags.</param>
        public void SetComputeBuffer (byte stage, VertexBuffer buffer, ComputeBufferAccess access) {
            NativeMethods.bgfx_encoder_set_compute_vertex_buffer(ptr, stage, buffer.handle, access);
        }

        /// <summary>
        /// Sets a dynamic index buffer as a compute resource.
        /// </summary>
        /// <param name="stage">The resource stage to set.</param>
        /// <param name="buffer">The buffer to set.</param>
        /// <param name="access">Access control flags.</param>
        public void SetComputeBuffer (byte stage, DynamicIndexBuffer buffer, ComputeBufferAccess access) {
            NativeMethods.bgfx_encoder_set_compute_dynamic_index_buffer(ptr, stage, buffer.handle, access);
        }

        /// <summary>
        /// Sets a dynamic vertex buffer as a compute resource.
        /// </summary>
        /// <param name="stage">The resource stage to set.</param>
        /// <param name="buffer">The buffer to set.</param>
        /// <param name="access">Access control flags.</param>
        public void SetComputeBuffer (byte stage, DynamicVertexBuffer buffer, ComputeBufferAccess access) {
            NativeMethods.bgfx_encoder_set_compute_dynamic_vertex_buffer(ptr, stage, buffer.handle, access);
        }

        /// <summary>
        /// Sets an indirect buffer as a compute resource.
        /// </summary>
        /// <param name="stage">The resource stage to set.</param>
        /// <param name="buffer">The buffer to set.</param>
        /// <param name="access">Access control flags.</param>
        public void SetComputeBuffer (byte stage, IndirectBuffer buffer, ComputeBufferAccess access) {
            NativeMethods.bgfx_encoder_set_compute_indirect_buffer(ptr, stage, buffer.handle, access);
        }

        /// <summary>
        /// Dispatches a compute job.
        /// </summary>
        /// <param name="id">The index of the view to dispatch.</param>
        /// <param name="program">The shader program to use.</param>
        /// <param name="xCount">The size of the job in the first dimension.</param>
        /// <param name="yCount">The size of the job in the second dimension.</param>
        /// <param name="zCount">The size of the job in the third dimension.</param>
        public void Dispatch (ushort id, Program program, int xCount = 1, int yCount = 1, int zCount = 1) {
            NativeMethods.bgfx_encoder_dispatch(ptr, id, program.handle, (uint)xCount, (uint)yCount, (uint)zCount);
        }

        /// <summary>
        /// Dispatches an indirect compute job.
        /// </summary>
        /// <param name="id">The index of the view to dispatch.</param>
        /// <param name="program">The shader program to use.</param>
        /// <param name="indirectBuffer">The buffer containing drawing commands.</param>
        /// <param name="startIndex">The index of the first command to process.</param>
        /// <param name="count">The number of commands to process from the buffer.</param>
        public void Dispatch (ushort id, Program program, IndirectBuffer indirectBuffer, int startIndex = 0, int count = 1) {
            NativeMethods.bgfx_encoder_dispatch_indirect(ptr, id, program.handle, indirectBuffer.handle, (ushort)startIndex, (ushort)count);
        }

        /// <summary>
        /// Discards all previously set state for the draw call.
        /// </summary>
        public void Discard () {
            NativeMethods.bgfx_encoder_discard(ptr);
        }

        /// <summary>
        /// Finishes submission of commands from this encoder.
        /// </summary>
        public void Dispose () {
            NativeMethods.bgfx_end(ptr);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals (Encoder other) {
            return ptr == other.ptr;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (object obj) {
            var other = obj as Encoder?;
            if (other == null)
                return false;

            return Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode () {
            return ptr.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString () {
            return ptr.ToString();
        }

        /// <summary>
        /// Implements the equality operator.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>
        /// <c>true</c> if the two objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator == (Encoder left, Encoder right) {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the inequality operator.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>
        /// <c>true</c> if the two objects are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator != (Encoder left, Encoder right) {
            return !left.Equals(right);
        }
    }
}
