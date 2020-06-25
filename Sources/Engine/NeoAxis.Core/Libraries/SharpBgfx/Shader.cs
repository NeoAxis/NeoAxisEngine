using System;
using System.Collections.Generic;

namespace SharpBgfx {
    /// <summary>
    /// Represents a single compiled shader component.
    /// </summary>
    public unsafe struct Shader : IDisposable, IEquatable<Shader> {
        Uniform[] uniforms;
        internal readonly ushort handle;

        /// <summary>
        /// Represents an invalid handle.
        /// </summary>
        public static readonly Shader Invalid = new Shader();

        /// <summary>
        /// The set of uniforms exposed by the shader.
        /// </summary>
        public IReadOnlyList<Uniform> Uniforms {
            get {
                if (uniforms == null) {
                    var count = NativeMethods.bgfx_get_shader_uniforms(handle, null, 0);
                    uniforms = new Uniform[count];
                    NativeMethods.bgfx_get_shader_uniforms(handle, uniforms, count);
                }

                return uniforms;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> struct.
        /// </summary>
        /// <param name="memory">The compiled shader memory.</param>
        public Shader (MemoryBlock memory) {
            handle = NativeMethods.bgfx_create_shader(memory.ptr);
            uniforms = null;
        }

        /// <summary>
        /// Releases the shader.
        /// </summary>
        public void Dispose () {
            NativeMethods.bgfx_destroy_shader(handle);
        }

        /// <summary>
        /// Sets the name of the shader, for debug display purposes.
        /// </summary>
        /// <param name="name">The name of the shader.</param>
        public void SetName(string name) {
            NativeMethods.bgfx_set_shader_name(handle, name, int.MaxValue);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals (Shader other) {
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
            var other = obj as Shader?;
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
        public static bool operator ==(Shader left, Shader right) {
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
        public static bool operator !=(Shader left, Shader right) {
            return !left.Equals(right);
        }
    }
}
