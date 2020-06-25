using System;

namespace SharpBgfx {
    /// <summary>
    /// Represents a static vertex buffer.
    /// </summary>
    public unsafe struct VertexBuffer : IDisposable, IEquatable<VertexBuffer> {
        internal readonly ushort handle;

        /// <summary>
        /// Represents an invalid handle.
        /// </summary>
        public static readonly VertexBuffer Invalid = new VertexBuffer();

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBuffer"/> struct.
        /// </summary>
        /// <param name="memory">The vertex data with which to populate the buffer.</param>
        /// <param name="layout">The layout of the vertex data.</param>
        /// <param name="flags">Flags used to control buffer behavior.</param>
        public VertexBuffer (MemoryBlock memory, VertexLayout layout, BufferFlags flags = BufferFlags.None) {
            handle = NativeMethods.bgfx_create_vertex_buffer(memory.ptr, ref layout.data, flags);
        }

        /// <summary>
        /// Sets the name of the vertex buffer, for debug display purposes.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        public void SetName(string name) {
            NativeMethods.bgfx_set_vertex_buffer_name(handle, name, int.MaxValue);
        }

        /// <summary>
        /// Releases the vertex buffer.
        /// </summary>
        public void Dispose () {
            NativeMethods.bgfx_destroy_vertex_buffer(handle);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals (VertexBuffer other) {
            return handle == other.handle;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (object obj) {
            var other = obj as VertexBuffer?;
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
            return handle.GetHashCode();
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
        public static bool operator ==(VertexBuffer left, VertexBuffer right) {
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
        public static bool operator !=(VertexBuffer left, VertexBuffer right) {
            return !left.Equals(right);
        }
    }
}
