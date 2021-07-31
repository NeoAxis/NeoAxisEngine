using System;

namespace SharpBgfx {
    /// <summary>
    /// Represents a compiled and linked shader program.
    /// </summary>
    public struct Program : IDisposable, IEquatable<Program> {
        internal readonly ushort handle;

        /// <summary>
        /// Represents an invalid handle.
        /// </summary>
        public static readonly Program Invalid = new Program();

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> struct.
        /// </summary>
        /// <param name="vertexShader">The vertex shader.</param>
        /// <param name="fragmentShader">The fragment shader.</param>
        /// <param name="destroyShaders">if set to <c>true</c>, the shaders will be released after creating the program.</param>
        public Program (Shader vertexShader, Shader fragmentShader, bool destroyShaders = false) {
            handle = NativeMethods.bgfx_create_program(vertexShader.handle, fragmentShader.handle, destroyShaders);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> struct.
        /// </summary>
        /// <param name="computeShader">The compute shader.</param>
        /// <param name="destroyShaders">if set to <c>true</c>, the compute shader will be released after creating the program.</param>
        public Program (Shader computeShader, bool destroyShaders = false) {
            handle = NativeMethods.bgfx_create_compute_program(computeShader.handle, destroyShaders);
        }

        /// <summary>
        /// Releases the program.
        /// </summary>
        public void Dispose () {
            NativeMethods.bgfx_destroy_program(handle);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals (Program other) {
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
            var other = obj as Program?;
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
        public static bool operator ==(Program left, Program right) {
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
        public static bool operator !=(Program left, Program right) {
            return !left.Equals(right);
        }
    }
}
