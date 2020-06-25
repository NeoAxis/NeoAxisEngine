namespace SharpBgfx {
    /// <summary>
    /// Describes the layout of data in a vertex stream.
    /// </summary>
    public sealed class VertexLayout {
        internal Data data;

        /// <summary>
        /// The stride of a single vertex using this layout.
        /// </summary>
        public int Stride {
            get { return data.Stride; }
        }

        /// <summary>
        /// Starts a stream of vertex attribute additions to the layout.
        /// </summary>
        /// <param name="backend">The rendering backend with which to associate the attributes.</param>
        /// <returns>This instance, for use in a fluent API.</returns>
        public VertexLayout Begin (RendererBackend backend = RendererBackend.Noop) {
            NativeMethods.bgfx_vertex_decl_begin(ref data, backend);
            return this;
        }

        /// <summary>
        /// Starts a stream of vertex attribute additions to the layout.
        /// </summary>
        /// <param name="attribute">The kind of attribute to add.</param>
        /// <param name="count">The number of elements in the attribute (1, 2, 3, or 4).</param>
        /// <param name="type">The type of data described by the attribute.</param>
        /// <param name="normalized">if set to <c>true</c>, values will be normalized from a 0-255 range to 0.0 - 0.1 in the shader.</param>
        /// <param name="asInt">if set to <c>true</c>, the attribute is packaged as an integer in the shader.</param>
        /// <returns>
        /// This instance, for use in a fluent API.
        /// </returns>
        public VertexLayout Add (VertexAttributeUsage attribute, int count, VertexAttributeType type, bool normalized = false, bool asInt = false) {
            NativeMethods.bgfx_vertex_decl_add(ref data, attribute, (byte)count, type, normalized, asInt);
            return this;
        }

        /// <summary>
        /// Skips the specified number of bytes in the vertex stream.
        /// </summary>
        /// <param name="count">The number of bytes to skip.</param>
        /// <returns>This instance, for use in a fluent API.</returns>
        public VertexLayout Skip (int count) {
            NativeMethods.bgfx_vertex_decl_skip(ref data, (byte)count);
            return this;
        }

        /// <summary>
        /// Marks the end of the vertex stream.
        /// </summary>
        /// <returns>This instance, for use in a fluent API.</returns>
        public VertexLayout End () {
            NativeMethods.bgfx_vertex_decl_end(ref data);
            return this;
        }

        /// <summary>
        /// Gets the byte offset of a particular attribute in the layout.
        /// </summary>
        /// <param name="attribute">The attribute for which to get the offset.</param>
        /// <returns>The offset of the attribute, in bytes.</returns>
        public unsafe int GetOffset (VertexAttributeUsage attribute) {
            fixed (Data* ptr = &data)
                return ptr->Offset[(int)attribute];
        }

        /// <summary>
        /// Determines whether the layout contains the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to check/</param>
        /// <returns><c>true</c> if the layout contains the attribute; otherwise, <c>false</c>.</returns>
        public unsafe bool HasAttribute (VertexAttributeUsage attribute) {
            fixed (Data* ptr = &data)
                return ptr->Attributes[(int)attribute] != ushort.MaxValue;
        }

        internal unsafe struct Data {
            const int MaxAttribCount = 18;

            public uint Hash;
            public ushort Stride;
            public fixed ushort Offset[MaxAttribCount];
            public fixed ushort Attributes[MaxAttribCount];
        }
    }
}
