using System;

namespace SharpBgfx {
    /// <summary>
    /// Maintains a transient vertex buffer.
    /// </summary>
    /// <remarks>
    /// The contents of the buffer are valid for the current frame only.
    /// You must call SetVertexBuffer with the buffer or a leak could occur.
    /// </remarks>
    public unsafe struct TransientVertexBuffer : IEquatable<TransientVertexBuffer> {
        readonly IntPtr data;
        int size;
        int startVertex;
        ushort stride;
        readonly ushort handle;
        ushort decl;

        /// <summary>
        /// Represents an invalid handle.
        /// </summary>
        public static readonly TransientVertexBuffer Invalid = new TransientVertexBuffer();

        /// <summary>
        /// A pointer that can be filled with vertex data.
        /// </summary>
        public IntPtr Data { get { return data; } }

        /// <summary>
        /// The size of the buffer.
        /// </summary>
        public int Count { get { return size; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientVertexBuffer"/> struct.
        /// </summary>
        /// <param name="vertexCount">The number of vertices that fit in the buffer.</param>
        /// <param name="layout">The layout of the vertex data.</param>
        public TransientVertexBuffer (int vertexCount, VertexLayout layout) {
            NativeMethods.bgfx_alloc_transient_vertex_buffer(out this, vertexCount, ref layout.data);
        }

        /// <summary>
        /// Gets the available space in the global transient vertex buffer.
        /// </summary>
        /// <param name="count">The number of vertices required.</param>
        /// <param name="layout">The layout of each vertex.</param>
        /// <returns>The number of available vertices.</returns>
        public static int GetAvailableSpace (int count, VertexLayout layout) {
            return NativeMethods.bgfx_get_avail_transient_vertex_buffer(count, ref layout.data);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals (TransientVertexBuffer other) {
            return handle == other.handle && data == other.data;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (object obj) {
            var other = obj as TransientVertexBuffer?;
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
            return handle.GetHashCode() >> 13 ^ data.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString () {
            return string.Format("Handle: {0}", handle);
        }

        /// <summary>
        /// Implements the equality operator.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>
        /// <c>true</c> if the two objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(TransientVertexBuffer left, TransientVertexBuffer right) {
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
        public static bool operator !=(TransientVertexBuffer left, TransientVertexBuffer right) {
            return !left.Equals(right);
        }
    }
}
