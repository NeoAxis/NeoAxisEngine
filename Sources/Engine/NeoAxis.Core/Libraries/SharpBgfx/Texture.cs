using System;

namespace Internal.SharpBgfx {
    /// <summary>
    /// Represents a loaded texture.
    /// </summary>
    public unsafe sealed class Texture : IDisposable, IEquatable<Texture> {
        internal readonly ushort handle;

        /// <summary>
        /// The width of the texture.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the texture.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The depth of the texture, if 3D.
        /// </summary>
        public int Depth { get; private set; }

		//!!!!betauser
		/// <summary>
		/// Indicates whether the texture is a cubemap.
		/// </summary>
		public bool IsCubeMap { get; set; }
		///// <summary>
		///// Indicates whether the texture is a cubemap.
		///// </summary>
		//public bool IsCubeMap { get; private set; }

        /// <summary>
        /// The number of texture array layers (for 2D or cube textures).
        /// </summary>
        public int ArrayLayers { get; private set; }

        /// <summary>
        /// The number of mip levels in the texture.
        /// </summary>
        public int MipLevels { get; private set; }

        /// <summary>
        /// The number of bits per pixel.
        /// </summary>
        public int BitsPerPixel { get; private set; }

        /// <summary>
        /// The size of the entire texture, in bytes.
        /// </summary>
        public int SizeInBytes { get; private set; }

        /// <summary>
        /// The format of the image data.
        /// </summary>
        public TextureFormat Format { get; private set; }

        internal Texture (ushort handle, ref TextureInfo info) {
            this.handle = handle;

            Width = info.Width;
            Height = info.Height;
            Depth = info.Depth;
            ArrayLayers = info.Layers;
            MipLevels = info.MipCount;
            BitsPerPixel = info.BitsPerPixel;
            SizeInBytes = info.StorageSize;
            Format = info.Format;
            IsCubeMap = info.IsCubeMap;
        }

        /// <summary>
        /// Creates a new texture from a file loaded in memory.
        /// </summary>
        /// <param name="memory">The content of the file.</param>
        /// <param name="flags">Flags that control texture behavior.</param>
        /// <param name="skipMips">A number of top level mips to skip when parsing texture data.</param>
        /// <returns>The newly created texture.</returns>
        /// <remarks>
        /// This function supports textures in the following container formats:
        /// - DDS
        /// - KTX
        /// - PVR
        /// </remarks>
        public static Texture FromFile (MemoryBlock memory, TextureFlags flags = TextureFlags.None, int skipMips = 0) {
            TextureInfo info;
            var handle = NativeMethods.bgfx_create_texture(memory.ptr, flags, (byte)skipMips, out info);

            return new Texture(handle, ref info);
        }

        /// <summary>
        /// Creates a new 2D texture.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="hasMips">Indicates that texture contains full mip-map chain.</param>
        /// <param name="arrayLayers">Number of layers in texture array. Must be 1 if Texture2DArray caps flag not set.</param>
        /// <param name="format">The format of the texture data.</param>
        /// <param name="flags">Flags that control texture behavior.</param>
        /// <param name="memory">If not <c>null</c>, contains the texture's image data.</param>
        /// <returns>
        /// The newly created texture handle.
        /// </returns>
        public static Texture Create2D (int width, int height, bool hasMips, int arrayLayers, TextureFormat format, TextureFlags flags = TextureFlags.None, MemoryBlock? memory = null) {
            var info = new TextureInfo();
            NativeMethods.bgfx_calc_texture_size(ref info, (ushort)width, (ushort)height, 1, false, hasMips, (ushort)arrayLayers, format);

            var handle = NativeMethods.bgfx_create_texture_2d(info.Width, info.Height, hasMips, (ushort)arrayLayers, format, flags, memory == null ? null : memory.Value.ptr);
            return new Texture(handle, ref info);
        }

        /// <summary>
        /// Creates a new 2D texture that scales with backbuffer size.
        /// </summary>
        /// <param name="ratio">The amount to scale when the backbuffer resizes.</param>
        /// <param name="hasMips">Indicates that texture contains full mip-map chain.</param>
        /// <param name="arrayLayers">Number of layers in texture array. Must be 1 if Texture2DArray caps flag not set.</param>
        /// <param name="format">The format of the texture data.</param>
        /// <param name="flags">Flags that control texture behavior.</param>
        /// <returns>
        /// The newly created texture handle.
        /// </returns>
        public static Texture Create2D (BackbufferRatio ratio, bool hasMips, int arrayLayers, TextureFormat format, TextureFlags flags = TextureFlags.None) {
            var info = new TextureInfo {
                Format = format,
                Layers = (ushort)arrayLayers
            };

            var handle = NativeMethods.bgfx_create_texture_2d_scaled(ratio, hasMips, (ushort)arrayLayers, format, flags);
            return new Texture(handle, ref info);
        }

        /// <summary>
        /// Creates a new 3D texture.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="depth">The depth of the texture.</param>
        /// <param name="hasMips">Indicates that texture contains full mip-map chain.</param>
        /// <param name="format">The format of the texture data.</param>
        /// <param name="flags">Flags that control texture behavior.</param>
        /// <param name="memory">If not <c>null</c>, contains the texture's image data.</param>
        /// <returns>The newly created texture handle.</returns>
        public static Texture Create3D (int width, int height, int depth, bool hasMips, TextureFormat format, TextureFlags flags = TextureFlags.None, MemoryBlock? memory = null) {
            var info = new TextureInfo();
            NativeMethods.bgfx_calc_texture_size(ref info, (ushort)width, (ushort)height, (ushort)depth, false, hasMips, 1, format);

            var handle = NativeMethods.bgfx_create_texture_3d(info.Width, info.Height, info.Depth, hasMips, format, flags, memory == null ? null : memory.Value.ptr);
            return new Texture(handle, ref info);
        }

        /// <summary>
        /// Creates a new cube texture.
        /// </summary>
        /// <param name="size">The size of each cube face.</param>
        /// <param name="hasMips">Indicates that texture contains full mip-map chain.</param>
        /// <param name="arrayLayers">Number of layers in texture array. Must be 1 if Texture2DArray caps flag not set.</param>
        /// <param name="format">The format of the texture data.</param>
        /// <param name="flags">Flags that control texture behavior.</param>
        /// <param name="memory">If not <c>null</c>, contains the texture's image data.</param>
        /// <returns>
        /// The newly created texture handle.
        /// </returns>
        public static Texture CreateCube (int size, bool hasMips, int arrayLayers, TextureFormat format, TextureFlags flags = TextureFlags.None, MemoryBlock? memory = null) {
            var info = new TextureInfo();
            NativeMethods.bgfx_calc_texture_size(ref info, (ushort)size, (ushort)size, 1, true, hasMips, (ushort)arrayLayers, format);

            var handle = NativeMethods.bgfx_create_texture_cube(info.Width, hasMips, (ushort)arrayLayers, format, flags, memory == null ? null : memory.Value.ptr);
            return new Texture(handle, ref info);
        }

        /// <summary>
        /// Checks whether a texture with the given parameters would be considered valid.
        /// </summary>
        /// <param name="depth">The depth of the texture.</param>
        /// <param name="isCube"><c>true</c> if the texture contains a cubemap.</param>
        /// <param name="arrayLayers">Number of layers in texture array.</param>
        /// <param name="format">The format of the texture data.</param>
        /// <param name="flags">Flags that control texture behavior.</param>
        /// <returns></returns>
        public static bool IsValid (int depth, bool isCube, int arrayLayers, TextureFormat format, TextureFlags flags = TextureFlags.None) {
            return NativeMethods.bgfx_is_texture_valid((ushort)depth, isCube, (ushort)arrayLayers, format, flags);
        }

        /// <summary>
        /// Releases the texture.
        /// </summary>
        public void Dispose () {
            NativeMethods.bgfx_destroy_texture(handle);
        }

        /// <summary>
        /// Sets the name of the texture, for debug display purposes.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        public void SetName(string name) {
            NativeMethods.bgfx_set_texture_name(handle, name, int.MaxValue);
        }

        /// <summary>
        /// Updates the data in a 2D texture.
        /// </summary>
        /// <param name="arrayLayer">The layer in a texture array to update.</param>
        /// <param name="mipLevel">The mip level.</param>
        /// <param name="x">The X coordinate of the rectangle to update.</param>
        /// <param name="y">The Y coordinate of the rectangle to update.</param>
        /// <param name="width">The width of the rectangle to update.</param>
        /// <param name="height">The height of the rectangle to update.</param>
        /// <param name="memory">The new image data.</param>
        /// <param name="pitch">The pitch of the image data.</param>
        public void Update2D (int arrayLayer, int mipLevel, int x, int y, int width, int height, MemoryBlock memory, int pitch) {
            NativeMethods.bgfx_update_texture_2d(handle, (ushort)arrayLayer, (byte)mipLevel, (ushort)x, (ushort)y, (ushort)width, (ushort)height, memory.ptr, (ushort)pitch);
        }

        /// <summary>
        /// Updates the data in a 3D texture.
        /// </summary>
        /// <param name="mipLevel">The mip level.</param>
        /// <param name="x">The X coordinate of the volume to update.</param>
        /// <param name="y">The Y coordinate of the volume to update.</param>
        /// <param name="z">The Z coordinate of the volume to update.</param>
        /// <param name="width">The width of the volume to update.</param>
        /// <param name="height">The height of the volume to update.</param>
        /// <param name="depth">The depth of the volume to update.</param>
        /// <param name="memory">The new image data.</param>
        public void Update3D (int mipLevel, int x, int y, int z, int width, int height, int depth, MemoryBlock memory) {
            NativeMethods.bgfx_update_texture_3d(handle, (byte)mipLevel, (ushort)x, (ushort)y, (ushort)z, (ushort)width, (ushort)height, (ushort)depth, memory.ptr);
        }

        /// <summary>
        /// Updates the data in a cube texture.
        /// </summary>
        /// <param name="face">The cube map face to update.</param>
        /// <param name="arrayLayer">The layer in a texture array to update.</param>
        /// <param name="mipLevel">The mip level.</param>
        /// <param name="x">The X coordinate of the rectangle to update.</param>
        /// <param name="y">The Y coordinate of the rectangle to update.</param>
        /// <param name="width">The width of the rectangle to update.</param>
        /// <param name="height">The height of the rectangle to update.</param>
        /// <param name="memory">The new image data.</param>
        /// <param name="pitch">The pitch of the image data.</param>
        public void UpdateCube (CubeMapFace face, int arrayLayer, int mipLevel, int x, int y, int width, int height, MemoryBlock memory, int pitch) {
            NativeMethods.bgfx_update_texture_cube(handle, (ushort)arrayLayer, face, (byte)mipLevel, (ushort)x, (ushort)y, (ushort)width, (ushort)height, memory.ptr, (ushort)pitch);
        }

        /// <summary>
        /// Blits the contents of the texture to another texture.
        /// </summary>
        /// <param name="viewId">The view in which the blit will be ordered.</param>
        /// <param name="dest">The destination texture.</param>
        /// <param name="destX">The destination X position.</param>
        /// <param name="destY">The destination Y position.</param>
        /// <param name="sourceX">The source X position.</param>
        /// <param name="sourceY">The source Y position.</param>
        /// <param name="width">The width of the region to blit.</param>
        /// <param name="height">The height of the region to blit.</param>
        /// <remarks>The destination texture must be created with the <see cref="TextureFlags.BlitDestination"/> flag.</remarks>
        public void BlitTo (ushort viewId, Texture dest, int destX, int destY, int sourceX = 0, int sourceY = 0,
                            int width = ushort.MaxValue, int height = ushort.MaxValue) {
            BlitTo(viewId, dest, 0, destX, destY, 0, 0, sourceX, sourceY, 0, width, height, 0);
        }

        /// <summary>
        /// Blits the contents of the texture to another texture.
        /// </summary>
        /// <param name="viewId">The view in which the blit will be ordered.</param>
        /// <param name="dest">The destination texture.</param>
        /// <param name="destMip">The destination mip level.</param>
        /// <param name="destX">The destination X position.</param>
        /// <param name="destY">The destination Y position.</param>
        /// <param name="destZ">The destination Z position.</param>
        /// <param name="sourceMip">The source mip level.</param>
        /// <param name="sourceX">The source X position.</param>
        /// <param name="sourceY">The source Y position.</param>
        /// <param name="sourceZ">The source Z position.</param>
        /// <param name="width">The width of the region to blit.</param>
        /// <param name="height">The height of the region to blit.</param>
        /// <param name="depth">The depth of the region to blit.</param>
        /// <remarks>The destination texture must be created with the <see cref="TextureFlags.BlitDestination"/> flag.</remarks>
        public void BlitTo (ushort viewId, Texture dest, int destMip, int destX, int destY, int destZ,
                            int sourceMip = 0, int sourceX = 0, int sourceY = 0, int sourceZ = 0,
                            int width = ushort.MaxValue, int height = ushort.MaxValue, int depth = ushort.MaxValue) {
            NativeMethods.bgfx_blit(viewId, dest.handle, (byte)destMip, (ushort)destX, (ushort)destY, (ushort)destZ,
                handle, (byte)sourceMip, (ushort)sourceX, (ushort)sourceY, (ushort)sourceZ, (ushort)width, (ushort)height, (ushort)depth);
        }

        /// <summary>
        /// Blits the contents of the texture to another texture.
        /// </summary>
        /// <param name="encoder">The encoder used for threaded command submission.</param>
        /// <param name="viewId">The view in which the blit will be ordered.</param>
        /// <param name="dest">The destination texture.</param>
        /// <param name="destX">The destination X position.</param>
        /// <param name="destY">The destination Y position.</param>
        /// <param name="sourceX">The source X position.</param>
        /// <param name="sourceY">The source Y position.</param>
        /// <param name="width">The width of the region to blit.</param>
        /// <param name="height">The height of the region to blit.</param>
        /// <remarks>The destination texture must be created with the <see cref="TextureFlags.BlitDestination"/> flag.</remarks>
        public void BlitTo (Encoder encoder, ushort viewId, Texture dest, int destX, int destY, int sourceX = 0, int sourceY = 0,
                            int width = ushort.MaxValue, int height = ushort.MaxValue) {
            BlitTo(encoder, viewId, dest, 0, destX, destY, 0, 0, sourceX, sourceY, 0, width, height, 0);
        }

        /// <summary>
        /// Blits the contents of the texture to another texture.
        /// </summary>
        /// <param name="encoder">The encoder used for threaded command submission.</param>
        /// <param name="viewId">The view in which the blit will be ordered.</param>
        /// <param name="dest">The destination texture.</param>
        /// <param name="destMip">The destination mip level.</param>
        /// <param name="destX">The destination X position.</param>
        /// <param name="destY">The destination Y position.</param>
        /// <param name="destZ">The destination Z position.</param>
        /// <param name="sourceMip">The source mip level.</param>
        /// <param name="sourceX">The source X position.</param>
        /// <param name="sourceY">The source Y position.</param>
        /// <param name="sourceZ">The source Z position.</param>
        /// <param name="width">The width of the region to blit.</param>
        /// <param name="height">The height of the region to blit.</param>
        /// <param name="depth">The depth of the region to blit.</param>
        /// <remarks>The destination texture must be created with the <see cref="TextureFlags.BlitDestination"/> flag.</remarks>
        public void BlitTo (Encoder encoder, ushort viewId, Texture dest, int destMip, int destX, int destY, int destZ,
                            int sourceMip = 0, int sourceX = 0, int sourceY = 0, int sourceZ = 0,
                            int width = ushort.MaxValue, int height = ushort.MaxValue, int depth = ushort.MaxValue) {
            NativeMethods.bgfx_encoder_blit(encoder.ptr, viewId, dest.handle, (byte)destMip, (ushort)destX, (ushort)destY, (ushort)destZ,
                handle, (byte)sourceMip, (ushort)sourceX, (ushort)sourceY, (ushort)sourceZ, (ushort)width, (ushort)height, (ushort)depth);
        }

        /// <summary>
        /// Reads the contents of the texture and stores them in memory pointed to by <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The destination for the read image data.</param>
        /// <param name="mip">The mip level to read.</param>
        /// <returns>The frame number on which the result will be available.</returns>
        /// <remarks>The texture must have been created with the <see cref="TextureFlags.ReadBack"/> flag.</remarks>
        public int Read (IntPtr data, int mip) {
            return (int)NativeMethods.bgfx_read_texture(handle, data, (byte)mip);
        }

        /// <summary>
        /// Override internal texture with externally created texture.
        /// </summary>
        /// <param name="ptr">The native API texture pointer.</param>
        /// <returns>
        /// Native API pointer to the texture. If result is <see cref="IntPtr.Zero"/>, the texture is not yet
        /// created from the main thread.
        /// </returns>
        public IntPtr OverrideInternal (IntPtr ptr) {
            return NativeMethods.bgfx_override_internal_texture_ptr(handle, ptr);
        }

        /// <summary>
        /// Override internal texture by creating a new 2D texture.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="mipCount">The number of mip levels.</param>
        /// <param name="format">The format of the texture data.</param>
        /// <param name="flags">Flags that control texture behavior.</param>
        /// <returns>
        /// Native API pointer to the texture. If result is <see cref="IntPtr.Zero"/>, the texture is not yet
        /// created from the main thread.
        /// </returns>
        public IntPtr OverrideInternal (int width, int height, int mipCount, TextureFormat format, TextureFlags flags = TextureFlags.None) {
            Width = width;
            Height = height;
            MipLevels = mipCount;
            Format = format;
            return NativeMethods.bgfx_override_internal_texture(handle, (ushort)width, (ushort)height, (byte)mipCount, format, flags);
        }

        /// <summary>
        /// Returns a direct pointer to the texture memory.
        /// </summary>
        /// <returns>
        /// A pointer to the texture's memory. If result is <see cref="IntPtr.Zero"/> direct access is
        /// not supported. If the result is -1, the texture is pending creation.
        /// </returns>
        public IntPtr GetDirectAccess () {
            return NativeMethods.bgfx_get_direct_access_ptr(handle);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals (Texture other) {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(other, this))
                return true;

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
            return Equals(obj as Texture);
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
        public static bool operator == (Texture left, Texture right) {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

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
        public static bool operator != (Texture left, Texture right) {
            return !(left == right);
        }

        internal struct TextureInfo {
            public TextureFormat Format;
            public int StorageSize;
            public ushort Width;
            public ushort Height;
            public ushort Depth;
            public ushort Layers;
            public byte MipCount;
            public byte BitsPerPixel;
            public bool IsCubeMap;
        }
    }
}
