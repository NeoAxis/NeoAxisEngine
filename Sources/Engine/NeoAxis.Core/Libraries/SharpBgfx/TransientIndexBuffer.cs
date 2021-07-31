using System;

namespace SharpBgfx {
    /// <summary>
    /// Maintains a transient index buffer.
    /// </summary>
    /// <remarks>
    /// The contents of the buffer are valid for the current frame only.
    /// You must call SetVertexBuffer with the buffer or a leak could occur.
    /// </remarks>
    public unsafe struct TransientIndexBuffer : IEquatable<TransientIndexBuffer> {
        readonly IntPtr data;
        int size;
        int startIndex;
        readonly ushort handle;

        /// <summary>
        /// Represents an invalid handle.
        /// </summary>
        public static readonly TransientIndexBuffer Invalid = new TransientIndexBuffer();

        /// <summary>
        /// A pointer that can be filled with index data.
        /// </summary>
        public IntPtr Data { get { return data; } }

        /// <summary>
        /// The size of the buffer.
        /// </summary>
        public int Count { get { return size; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientIndexBuffer"/> struct.
        /// </summary>
        /// <param name="indexCount">The number of 16-bit indices that fit in the buffer.</param>
        public TransientIndexBuffer (int indexCount) {
            NativeMethods.bgfx_alloc_transient_index_buffer(out this, indexCount);
        }

        /// <summary>
        /// Gets the available space in the global transient index buffer.
        /// </summary>
        /// <param name="count">The number of 16-bit indices required.</param>
        /// <returns>The number of available indices.</returns>
        public static int GetAvailableSpace (int count) {
            return NativeMethods.bgfx_get_avail_transient_index_buffer(count);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals (TransientIndexBuffer other) {
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
            var other = obj as TransientIndexBuffer?;
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
            return string.Format("Count: {0}", Count);
        }

        /// <summary>
        /// Implements the equality operator.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>
        /// <c>true</c> if the two objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(TransientIndexBuffer left, TransientIndexBuffer right) {
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
        public static bool operator !=(TransientIndexBuffer left, TransientIndexBuffer right) {
            return !left.Equals(right);
        }
    }
}
