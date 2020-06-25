using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpBgfx {
    /// <summary>
    /// Delegate type for callback functions.
    /// </summary>
    /// <param name="userData">User-provided data to the original allocation call.</param>
    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ReleaseCallback (IntPtr userData);

    /// <summary>
    /// Represents a block of memory managed by the graphics API.
    /// </summary>
    public unsafe struct MemoryBlock : IEquatable<MemoryBlock> {
        internal readonly DataPtr* ptr;

        /// <summary>
        /// Represents an invalid handle.
        /// </summary>
        public static readonly MemoryBlock Invalid = new MemoryBlock();

        /// <summary>
        /// The pointer to the raw data.
        /// </summary>
        public IntPtr Data {
            get { return ptr == null ? IntPtr.Zero : ptr->Data; }
        }

        /// <summary>
        /// The size of the block, in bytes.
        /// </summary>
        public int Size {
            get { return ptr == null ? 0 : ptr->Size; }
        }

        MemoryBlock (DataPtr* ptr) {
            this.ptr = ptr;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryBlock"/> struct.
        /// </summary>
        /// <param name="size">The size of the block, in bytes.</param>
        public MemoryBlock (int size) {
            ptr = NativeMethods.bgfx_alloc(size);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryBlock"/> struct.
        /// </summary>
        /// <param name="data">A pointer to the initial data to copy into the new block.</param>
        /// <param name="size">The size of the block, in bytes.</param>
        public MemoryBlock (IntPtr data, int size) {
            ptr = NativeMethods.bgfx_copy(data, size);
        }

        /// <summary>
        /// Copies a managed array into a native graphics memory block.
        /// </summary>
        /// <typeparam name="T">The type of data in the array.</typeparam>
        /// <param name="data">The array to copy.</param>
        /// <returns>The native memory block containing the copied data.</returns>
        public static MemoryBlock FromArray<T>(T[] data) where T : struct {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException("data");

            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var block = new MemoryBlock(gcHandle.AddrOfPinnedObject(), Marshal.SizeOf<T>() * data.Length);

            gcHandle.Free();
            return block;
        }

        /// <summary>
        /// Creates a reference to the given data.
        /// </summary>
        /// <typeparam name="T">The type of data in the array.</typeparam>
        /// <param name="data">The array to reference.</param>
        /// <returns>The native memory block referring to the data.</returns>
        /// <remarks>
        /// The array must not be modified for at least 2 rendered frames.
        /// </remarks>
        public static MemoryBlock MakeRef<T>(T[] data) where T : struct {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException("data");

            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            return MakeRef(gcHandle.AddrOfPinnedObject(), Marshal.SizeOf<T>() * data.Length, GCHandle.ToIntPtr(gcHandle), ReleaseHandleCallback);
        }

        /// <summary>
        /// Makes a reference to the given memory block.
        /// </summary>
        /// <param name="data">A pointer to the memory.</param>
        /// <param name="size">The size of the memory block.</param>
        /// <param name="userData">Arbitrary user data passed to the release callback.</param>
        /// <param name="callback">A function that will be called when the data is ready to be released.</param>
        /// <returns>A new memory block referring to the given data.</returns>
        /// <remarks>
        /// The memory referred to by the returned memory block must not be modified
        /// or released until the callback fires.
        /// </remarks>
        public static MemoryBlock MakeRef (IntPtr data, int size, IntPtr userData, ReleaseCallback callback) {
            return new MemoryBlock(NativeMethods.bgfx_make_ref_release(data, size, Marshal.GetFunctionPointerForDelegate(callback), userData));
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals (MemoryBlock other) {
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
            var other = obj as MemoryBlock?;
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
            return new IntPtr(ptr).GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString () {
            return string.Format("Size: {0}", Size);
        }

        /// <summary>
        /// Implements the equality operator.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>
        /// <c>true</c> if the two objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(MemoryBlock left, MemoryBlock right) {
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
        public static bool operator !=(MemoryBlock left, MemoryBlock right) {
            return !left.Equals(right);
        }

#pragma warning disable 649
        internal struct DataPtr {
            public IntPtr Data;
            public int Size;
        }
#pragma warning restore 649

        static ReleaseCallback ReleaseHandleCallback = ReleaseHandle;
        static void ReleaseHandle (IntPtr userData) {
            var handle = GCHandle.FromIntPtr(userData);
            handle.Free();
        }
    }
}
