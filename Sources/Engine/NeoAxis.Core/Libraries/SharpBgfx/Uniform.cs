using System;
using System.Runtime.InteropServices;

namespace SharpBgfx {
    /// <summary>
    /// Represents a shader uniform.
    /// </summary>
    public unsafe struct Uniform : IDisposable, IEquatable<Uniform> {
        internal readonly ushort handle;

        /// <summary>
        /// Represents an invalid handle.
        /// </summary>
        public static readonly Uniform Invalid = new Uniform();

        /// <summary>
        /// The name of the uniform.
        /// </summary>
        public string Name {
            get {
                Info info;
                NativeMethods.bgfx_get_uniform_info(handle, out info);
                return Marshal.PtrToStringAnsi(new IntPtr(info.name));
            }
        }

        /// <summary>
        /// The type of the data represented by the uniform.
        /// </summary>
        public UniformType Type {
            get {
                Info info;
                NativeMethods.bgfx_get_uniform_info(handle, out info);
                return info.type;
            }
        }

        /// <summary>
        /// Size of the array, if the uniform is an array type.
        /// </summary>
        public int ArraySize {
            get {
                Info info;
                NativeMethods.bgfx_get_uniform_info(handle, out info);
                return info.arraySize;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Uniform"/> struct.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <param name="type">The type of data represented by the uniform.</param>
        /// <param name="arraySize">Size of the array, if the uniform is an array type.</param>
        /// <remarks>
        /// Predefined uniform names:
        /// u_viewRect vec4(x, y, width, height) - view rectangle for current view.
        /// u_viewTexel vec4 (1.0/width, 1.0/height, undef, undef) - inverse width and height
        /// u_view mat4 - view matrix
        /// u_invView mat4 - inverted view matrix
        /// u_proj mat4 - projection matrix
        /// u_invProj mat4 - inverted projection matrix
        /// u_viewProj mat4 - concatenated view projection matrix
        /// u_invViewProj mat4 - concatenated inverted view projection matrix
        /// u_model mat4[BGFX_CONFIG_MAX_BONES] - array of model matrices.
        /// u_modelView mat4 - concatenated model view matrix, only first model matrix from array is used.
        /// u_modelViewProj mat4 - concatenated model view projection matrix.
        /// u_alphaRef float - alpha reference value for alpha test.
        /// </remarks>
        public Uniform (string name, UniformType type, int arraySize = 1) {
            handle = NativeMethods.bgfx_create_uniform(name, type, (ushort)arraySize);
        }

        /// <summary>
        /// Releases the uniform.
        /// </summary>
        public void Dispose () {
            NativeMethods.bgfx_destroy_uniform(handle);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals (Uniform other) {
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
            var other = obj as Uniform?;
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
        public static bool operator ==(Uniform left, Uniform right) {
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
        public static bool operator !=(Uniform left, Uniform right) {
            return !left.Equals(right);
        }

        internal struct Info {
            public fixed sbyte name[256];
            public UniformType type;
            public ushort arraySize;
        }
    }
}
