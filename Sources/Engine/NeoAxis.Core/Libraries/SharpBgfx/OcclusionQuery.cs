using System;

namespace SharpBgfx {
    /// <summary>
    /// Represents an occlusion query.
    /// </summary>
    public unsafe struct OcclusionQuery : IDisposable, IEquatable<OcclusionQuery> {
        internal readonly ushort handle;

        /// <summary>
        /// Represents an invalid handle.
        /// </summary>
        public static readonly OcclusionQuery Invalid = new OcclusionQuery();

        /// <summary>
        /// Gets the result of the query.
        /// </summary>
        public OcclusionQueryResult Result {
            get { return NativeMethods.bgfx_get_result(handle, null); }
        }

        /// <summary>
        /// Gets the number of pixels that passed the test. Only valid
        /// if <see cref="Result"/> is also valid.
        /// </summary>
        public int PassingPixels {
            get {
                int pixels = 0;
                NativeMethods.bgfx_get_result(handle, &pixels);
                return pixels;
            }
        }

        OcclusionQuery (ushort handle) {
            this.handle = handle;
        }

        /// <summary>
        /// Creates a new query.
        /// </summary>
        /// <returns>The new occlusion query.</returns>
        public static OcclusionQuery Create() {
            return new OcclusionQuery(NativeMethods.bgfx_create_occlusion_query());
        }

        /// <summary>
        /// Releases the query.
        /// </summary>
        public void Dispose () {
            NativeMethods.bgfx_destroy_occlusion_query(handle);
        }

        /// <summary>
        /// Sets the condition for which the query should check.
        /// </summary>
        /// <param name="visible"><c>true</c> for visible; <c>false</c> for invisible.</param>
        public void SetCondition (bool visible) {
            NativeMethods.bgfx_set_condition(handle, visible);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals (OcclusionQuery other) {
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
            var other = obj as OcclusionQuery?;
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
        public static bool operator == (OcclusionQuery left, OcclusionQuery right) {
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
        public static bool operator != (OcclusionQuery left, OcclusionQuery right) {
            return !left.Equals(right);
        }
    }
}
