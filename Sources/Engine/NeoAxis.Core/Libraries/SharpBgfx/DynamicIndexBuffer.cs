using System;

namespace SharpBgfx {
    /// <summary>
    /// Represents a dynamically updateable index buffer.
    /// </summary>
    public unsafe struct DynamicIndexBuffer : IDisposable, IEquatable<DynamicIndexBuffer> {
        internal readonly ushort handle;

        /// <summary>
        /// Represents an invalid handle.
        /// </summary>
        public static readonly DynamicIndexBuffer Invalid = new DynamicIndexBuffer();

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicIndexBuffer"/> struct.
        /// </summary>
        /// <param name="indexCount">The number of indices that can fit in the buffer.</param>
        /// <param name="flags">Flags used to control buffer behavior.</param>
        public DynamicIndexBuffer (int indexCount, BufferFlags flags = BufferFlags.None) {
            handle = NativeMethods.bgfx_create_dynamic_index_buffer(indexCount, flags);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicIndexBuffer"/> struct.
        /// </summary>
        /// <param name="memory">The initial index data with which to populate the buffer.</param>
        /// <param name="flags">Flags used to control buffer behavior.</param>
        public DynamicIndexBuffer (MemoryBlock memory, BufferFlags flags = BufferFlags.None) {
            handle = NativeMethods.bgfx_create_dynamic_index_buffer_mem(memory.ptr, flags);
        }

        /// <summary>
        /// Updates the data in the buffer.
        /// </summary>
        /// <param name="startIndex">Index of the first index to update.</param>
        /// <param name="memory">The new index data with which to fill the buffer.</param>
        public void Update (int startIndex, MemoryBlock memory) {
            NativeMethods.bgfx_update_dynamic_index_buffer(handle, startIndex, memory.ptr);
        }

        /// <summary>
        /// Releases the index buffer.
        /// </summary>
        public void Dispose () {
            NativeMethods.bgfx_destroy_dynamic_index_buffer(handle);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals (DynamicIndexBuffer other) {
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
            var other = obj as DynamicIndexBuffer?;
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
        public static bool operator ==(DynamicIndexBuffer left, DynamicIndexBuffer right) {
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
        public static bool operator !=(DynamicIndexBuffer left, DynamicIndexBuffer right) {
            return !left.Equals(right);
        }
    }
}
