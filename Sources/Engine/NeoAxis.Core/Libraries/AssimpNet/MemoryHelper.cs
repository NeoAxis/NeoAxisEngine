/*
* Copyright (c) 2012-2020 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using NeoAxis;

namespace Internal.Assimp
{
    /// <summary>
    /// Delegate for performing unmanaged memory cleanup.
    /// </summary>
    /// <param name="nativeValue">Location in unmanaged memory of the value to cleanup</param>
    /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise</param>
    public delegate void FreeNativeDelegate(IntPtr nativeValue, bool freeNative);

    /// <summary>
    /// Helper static class containing functions that aid dealing with unmanaged memory to managed memory conversions.
    /// </summary>
    public static class MemoryHelper
    {
        private static Dictionary<Type, INativeCustomMarshaler> s_customMarshalers = new Dictionary<Type, INativeCustomMarshaler>();
        private static Dictionary<object, GCHandle> s_pinnedObjects = new Dictionary<object, GCHandle>();

        #region Marshaling Interop

        /// <summary>
        /// Marshals an array of managed values to a c-style unmanaged array (void*). This also can optionally marshal to
        /// an unmanaged array of pointers (void**).
        /// </summary>
        /// <typeparam name="Managed">Managed type</typeparam>
        /// <typeparam name="Native">Native type</typeparam>
        /// <param name="managedColl">Collection of managed values</param>
        /// <param name="arrayOfPointers">True if the pointer is an array of pointers, false otherwise.</param>
        /// <returns>Pointer to unmanaged memory</returns>
        public static IntPtr ToNativeArray<Managed, Native>(IReadOnlyCollection<Managed> managedColl, bool arrayOfPointers = false)
            where Managed : class, IMarshalable<Managed, Native>, new()
            where Native : struct
        {
            if(managedColl == null || managedColl.Count == 0)
                return IntPtr.Zero;

            bool isNativeBlittable = IsNativeBlittable<Managed, Native>(managedColl);
            int sizeofNative = (isNativeBlittable) ? SizeOf<Native>() : MarshalSizeOf<Native>();

            //If the pointer is a void** we need to step by the pointer size, otherwise it's just a void* and step by the type size.
            int stride = (arrayOfPointers) ? IntPtr.Size : sizeofNative;
            IntPtr nativeArray = AllocateMemory(managedColl.Count * (arrayOfPointers ? IntPtr.Size : sizeofNative));

            IntPtr currPos = nativeArray;
            foreach(Managed managedValue in managedColl)
            {
                //Setup unmanaged data - do the actual ToNative later on, that way we can pass the thisPtr if the object is a pointer type.
                Native nativeValue = default(Native);

                //If array of pointers, each entry is a pointer so allocate memory, fill it, and write pointer to array, 
                //otherwise just write the data to the array location
                if(arrayOfPointers)
                {
                    IntPtr ptr = IntPtr.Zero;

                    //If managed value is null, write out a NULL ptr rather than wasting our time here
                    if(managedValue != null)
                    {
                        ptr = AllocateMemory(sizeofNative);

                        managedValue.ToNative(ptr, out nativeValue);

                        if(isNativeBlittable)
                        {
                            Write<Native>(ptr, nativeValue);
                        }
                        else
                        {
                            MarshalPointer<Native>(nativeValue, ptr);
                        }
                    }

                    Write<IntPtr>(currPos, ptr);
                }
                else
                {
                    managedValue.ToNative(IntPtr.Zero, out nativeValue);

                    if(isNativeBlittable)
                    {
                        Write<Native>(currPos, nativeValue);
                    }
                    else
                    {
                        MarshalPointer<Native>(nativeValue, currPos);
                    }
                }

                currPos += stride;
            }

            return nativeArray;
        }

        /// <summary>
        /// Marshals an array of managed values from a c-style unmanaged array (void*). This also can optionally marshal from 
        /// an unmanaged array of pointers (void**).
        /// </summary>
        /// <typeparam name="Managed">Managed type</typeparam>
        /// <typeparam name="Native">Native type</typeparam>
        /// <param name="nativeArray">Pointer to unmanaged memory</param>
        /// <param name="length">Number of elements to marshal</param>
        /// <param name="arrayOfPointers">True if the pointer is an array of pointers, false otherwise.</param>
        /// <returns>Marshaled managed values</returns>
        public static Managed[] FromNativeArray<Managed, Native>(IntPtr nativeArray, int length, bool arrayOfPointers = false)
            where Managed : class, IMarshalable<Managed, Native>, new()
            where Native : struct
        {
            if(nativeArray == IntPtr.Zero || length == 0)
                return Array.Empty<Managed>();

            //If the pointer is a void** we need to step by the pointer size, otherwise it's just a void* and step by the type size.
            int stride = (arrayOfPointers) ? IntPtr.Size : MarshalSizeOf<Native>();
            Managed[] managedArray = new Managed[length];

            for(int i = 0; i < length; i++)
            {
                IntPtr currPos = nativeArray + stride * i;

                //If pointer is a void**, read the current position to get the proper pointer
                if(arrayOfPointers)
                    currPos = Read<IntPtr>(currPos);

                Managed managedValue = Activator.CreateInstance<Managed>();

                //Marshal structure from the currentPointer position
                Native nativeValue;

                if(managedValue.IsNativeBlittable)
                {
                    nativeValue = Read<Native>(currPos);
                }
                else
                {
                    MarshalStructure<Native>(currPos, out nativeValue);
                }

                //Populate managed data
                managedValue.FromNative(nativeValue);

                managedArray[i] = managedValue;
            }

            return managedArray;
        }

        /// <summary>
        /// Marshals an array of blittable structs to a c-style unmanaged array (void*). This should not be used on non-blittable types
        /// that require marshaling by the runtime (e.g. has MarshalAs attributes).
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="managedArray">Managed array of structs</param>
        /// <returns>Pointer to unmanaged memory</returns>
        public static IntPtr ToNativeArray<T>(T[] managedArray) where T : struct
        {
            if(managedArray == null || managedArray.Length == 0)
                return IntPtr.Zero;

            IntPtr ptr = AllocateMemory(SizeOf<T>() * managedArray.Length);

            Write<T>(ptr, managedArray, 0, managedArray.Length);

            return ptr;
        }

        /// <summary>
        /// Marshals a list of blittable structs to a c-style unmanaged array (void*). This should not be used on non-blittable types
        /// that require marshaling by the runtime (e.g. has MarshalAs attributes).
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="managedArray">Managed list of structs</param>
        /// <returns>Pointer to unmanaged memory</returns>
        public static IntPtr ToNativeArray<T>(List<T> managedArray) where T : struct
        {
            if(managedArray == null || managedArray.Count == 0)
                return IntPtr.Zero;

            IntPtr ptr = AllocateMemory(SizeOf<T>() * managedArray.Count);

            Write<T>(ptr, managedArray, 0, managedArray.Count);

            return ptr;
        }

        /// <summary>
        /// Marshals an array of blittable structs from a c-style unmanaged array (void*). This should not be used on non-blittable types
        /// that require marshaling by the runtime (e.g. has MarshalAs attributes).
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="nativeArray">Pointer to unmanaged memory</param>
        /// <param name="length">Number of elements to read</param>
        /// <returns>Managed array</returns>
        public static T[] FromNativeArray<T>(IntPtr nativeArray, int length) where T : struct
        {
            if(nativeArray == IntPtr.Zero || length == 0)
                return Array.Empty<T>();

            T[] managedArray = new T[length];

            Read<T>(nativeArray, managedArray, 0, length);

            return managedArray;
        }

        /// <summary>
        /// Frees an unmanaged array and performs cleanup for each value. Optionally can free an array of pointers. This can be used on any type that can be
        /// marshaled into unmanaged memory.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="nativeArray">Pointer to unmanaged memory</param>
        /// <param name="length">Number of elements to free</param>
        /// <param name="action">Delegate that performs the necessary cleanup</param>
        /// <param name="arrayOfPointers">True if the pointer is an array of pointers, false otherwise.</param>
        public static void FreeNativeArray<T>(IntPtr nativeArray, int length, FreeNativeDelegate action, bool arrayOfPointers = false) where T : struct
        {
            if(nativeArray == IntPtr.Zero || length == 0 || action == null)
                return;

            //If the pointer is a void** we need tp step by the pointer eize, otherwise its just a void* and step by the type size
            int stride = (arrayOfPointers) ? IntPtr.Size : MarshalSizeOf<T>();

            for(int i = 0; i < length; i++)
            {
                IntPtr currPos = nativeArray + stride * i;

                //If pointer is a void**, read the current position to get the proper pointer
                if(arrayOfPointers)
                    currPos = Read<IntPtr>(currPos);

                //Invoke cleanup
                action(currPos, arrayOfPointers);
            }

            FreeMemory(nativeArray);
        }

        /// <summary>
        /// Marshals a managed value to unmanaged memory.
        /// </summary>
        /// <typeparam name="Managed">Managed type</typeparam>
        /// <typeparam name="Native">Unmanaged type</typeparam>
        /// <param name="managedValue">Managed value to marshal</param>
        /// <returns>Pointer to unmanaged memory</returns>
        public static IntPtr ToNativePointer<Managed, Native>(Managed managedValue)
            where Managed : class, IMarshalable<Managed, Native>, new()
            where Native : struct
        {

            if(managedValue == null)
                return IntPtr.Zero;

            int sizeofNative = (managedValue.IsNativeBlittable) ? SizeOf<Native>() : MarshalSizeOf<Native>();

            //Allocate memory
            IntPtr ptr = AllocateMemory(sizeofNative);

            //Setup unmanaged data
            Native nativeValue;
            managedValue.ToNative(ptr, out nativeValue);

            if(managedValue.IsNativeBlittable)
            {
                Write<Native>(ptr, nativeValue);
            }
            else
            {
                MarshalPointer<Native>(nativeValue, ptr);
            }

            return ptr;
        }

        /// <summary>
        /// Marshals a managed value from unmanaged memory.
        /// </summary>
        /// <typeparam name="Managed">Managed type</typeparam>
        /// <typeparam name="Native">Unmanaged type</typeparam>
        /// <param name="ptr">Pointer to unmanaged memory</param>
        /// <returns>The marshaled managed value</returns>
        public static Managed FromNativePointer<Managed, Native>(IntPtr ptr)
            where Managed : class, IMarshalable<Managed, Native>, new()
            where Native : struct
        {

            if(ptr == IntPtr.Zero)
                return null;

            Managed managedValue = Activator.CreateInstance<Managed>();

            //Marshal pointer to structure
            Native nativeValue;

            if(managedValue.IsNativeBlittable)
            {
                Read<Native>(ptr, out nativeValue);
            }
            else
            {
                MarshalStructure<Native>(ptr, out nativeValue);
            }

            //Populate managed value
            managedValue.FromNative(nativeValue);

            return managedValue;
        }

        /// <summary>
        /// Convienence method for marshaling a pointer to a structure. Only use if the type is not blittable, otherwise
        /// use the read methods for blittable types.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="ptr">Pointer to marshal</param>
        /// <param name="value">The marshaled structure</param>
        public static void MarshalStructure<T>(IntPtr ptr, out T value) where T : struct
        {
            if(ptr == IntPtr.Zero)
                value = default(T);

            Type type = typeof(T);

            INativeCustomMarshaler marshaler;
            if (HasNativeCustomMarshaler(type, out marshaler))
            {
                value = (T)marshaler.MarshalNativeToManaged(ptr);
                return;
            }

            value = Marshal.PtrToStructure<T>(ptr);
        }

        /// <summary>
        /// Convienence method for marshaling a pointer to a structure. Only use if the type is not blittable, otherwise
        /// use the read methods for blittable types.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="ptr">Pointer to marshal</param>
        /// <returns>The marshaled structure</returns>
        public static T MarshalStructure<T>(IntPtr ptr) where T : struct
        {
            if(ptr == IntPtr.Zero)
                return default(T);

            Type type = typeof(T);

            INativeCustomMarshaler marshaler;
            if (HasNativeCustomMarshaler(type, out marshaler))
                return (T) marshaler.MarshalNativeToManaged(ptr);

            return Marshal.PtrToStructure<T>(ptr);
        }

        /// <summary>
        /// Convienence method for marshaling a structure to a pointer. Only use if the type is not blittable, otherwise
        /// use the write methods for blittable types.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="value">Struct to marshal</param>
        /// <param name="ptr">Pointer to unmanaged chunk of memory which must be allocated prior to this call</param>
        public static void MarshalPointer<T>(in T value, IntPtr ptr) where T : struct
        {
            if (ptr == IntPtr.Zero)
                return;

            INativeCustomMarshaler marshaler;
            if (HasNativeCustomMarshaler(typeof(T), out marshaler))
            {
                marshaler.MarshalManagedToNative((object)value, ptr);
                return;
            }

            Marshal.StructureToPtr<T>(value, ptr, true);
        }

        /// <summary>
        /// Computes the size of the struct type using Marshal SizeOf. Only use if the type is not blittable, thus requiring marshaling by the runtime,
        /// (e.g. has MarshalAs attributes), otherwise use the SizeOf methods for blittable types.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns>Size of the struct in bytes.</returns>
        public static unsafe int MarshalSizeOf<T>() where T : struct
        {
            Type type = typeof(T);

            INativeCustomMarshaler marshaler;
            if (HasNativeCustomMarshaler(type, out marshaler))
                return marshaler.NativeDataSize;

            return Marshal.SizeOf<T>();
        }

#endregion

        #region Memory Interop (Shared code from other Projects)

        /// <summary>
        /// Allocates unmanaged memory. This memory should only be freed by this helper.
        /// </summary>
        /// <param name="sizeInBytes">Size to allocate</param>
        /// <param name="alignment">Alignment of the memory, by default aligned along 16-byte boundary.</param>
        /// <returns>Pointer to the allocated unmanaged memory.</returns>
        public static unsafe IntPtr AllocateMemory(int sizeInBytes, int alignment = 16)
        {
            int mask = alignment - 1;
            IntPtr rawPtr = Marshal.AllocHGlobal(sizeInBytes + mask + IntPtr.Size);
            long ptr = (long) ((byte*) rawPtr + sizeof(void*) + mask) & ~mask;
            ((IntPtr*) ptr)[-1] = rawPtr;

            return new IntPtr(ptr);
        }

        /// <summary>
        /// Frees unmanaged memory that was allocated by this helper.
        /// </summary>
        /// <param name="memoryPtr">Pointer to unmanaged memory to free.</param>
        public static unsafe void FreeMemory(IntPtr memoryPtr)
        {
            if(memoryPtr == IntPtr.Zero)
                return;

            Marshal.FreeHGlobal(((IntPtr*) memoryPtr)[-1]);
        }

        /// <summary>
        /// Reads a stream until the end is reached into a byte array. Based on
        /// <a href="http://www.yoda.arachsys.com/csharp/readbinary.html">Jon Skeet's implementation</a>.
        /// It is up to the caller to dispose of the stream.
        /// </summary>
        /// <param name="stream">Stream to read all bytes from</param>
        /// <param name="initialLength">Initial buffer length, default is 32K</param>
        /// <returns>The byte array containing all the bytes from the stream</returns>
        public static byte[] ReadStreamFully(Stream stream, int initialLength)
        {
            if(initialLength < 1)
            {
                initialLength = 32768; //Init to 32K if not a valid initial length
            }

            byte[] buffer = new byte[initialLength];
            int position = 0;
            int chunk;

            while((chunk = stream.Read(buffer, position, buffer.Length - position)) > 0)
            {
                position += chunk;

                //If we reached the end of the buffer check to see if there's more info
                if(position == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    //If -1 we reached the end of the stream
                    if(nextByte == -1)
                    {
                        return buffer;
                    }

                    //Not at the end, need to resize the buffer
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[position] = (byte) nextByte;
                    buffer = newBuffer;
                    position++;
                }
            }

            //Trim the buffer before returning
            byte[] toReturn = new byte[position];
            Array.Copy(buffer, toReturn, position);
            return toReturn;
        }

        /// <summary>
        /// Clears the memory to zero.
        /// </summary>
        /// <param name="memoryPtr">Pointer to the memory.</param>
        /// <param name="sizeInBytesToClear">Number of bytes, starting from the memory pointer, to clear.</param>
        public static unsafe void ClearMemory(IntPtr memoryPtr, int sizeInBytesToClear)
        {
			//!!!!betauser
			NativeUtility.ZeroMemory( memoryPtr, sizeInBytesToClear );
            //Unsafe.InitBlockUnaligned(memoryPtr.ToPointer(), 0, (uint) sizeInBytesToClear);
        }

        /// <summary>
        /// Computes the size of the struct type.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns>Size of the struct in bytes.</returns>
        public static unsafe int SizeOf<T>() where T : struct
        {
			//!!!!betauser
			return Marshal.SizeOf( typeof( T ) );
            //return Unsafe.SizeOf<T>();
        }

		//!!!!betauser
        ///// <summary>
        ///// Casts the pointer into a by-ref value of the specified type.
        ///// </summary>
        ///// <typeparam name="T">Struct type.</typeparam>
        ///// <param name="pSrc">Memory location.</param>
        ///// <returns>By-ref value.</returns>
        //public static unsafe ref T AsRef<T>(IntPtr pSrc) where T : struct
        //{
        //    return ref Unsafe.AsRef<T>(pSrc.ToPointer());
        //}

        /// <summary>
        /// Computes the size of the struct array.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="array">Array of structs</param>
        /// <returns>Total size, in bytes, of the array's contents.</returns>
        public static int SizeOf<T>(T[] array) where T : struct
        {
			//!!!!betauser
			return array == null ? 0 : array.Length * Marshal.SizeOf( typeof( T ) );
			//return array == null ? 0 : array.Length * Unsafe.SizeOf<T>();
		}

        /// <summary>
        /// Performs a memcopy that copies data from the memory pointed to by the source pointer to the memory pointer by the destination pointer.
        /// </summary>
        /// <param name="pDest">Destination memory location</param>
        /// <param name="pSrc">Source memory location</param>
        /// <param name="sizeInBytesToCopy">Number of bytes to copy</param>
        public static unsafe void CopyMemory(IntPtr pDest, IntPtr pSrc, int sizeInBytesToCopy)
        {
            Buffer.MemoryCopy(pSrc.ToPointer(), pDest.ToPointer(), (long) sizeInBytesToCopy, (long) sizeInBytesToCopy);
        }

        /// <summary>
        /// Reads data from the memory location into the array.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pSrc">Pointer to memory location</param>
        /// <param name="data">Array to store the copied data</param>
        /// <param name="startIndexInArray">Zero-based element index to start writing data to in the element array.</param>
        /// <param name="count">Number of elements to copy</param>
        public static unsafe void Read<T>(IntPtr pSrc, T[] data, int startIndexInArray, int count) where T : struct
        {
            ReadOnlySpan<T> src = new ReadOnlySpan<T>(pSrc.ToPointer(), count);
            Span<T> dst = new Span<T>(data, startIndexInArray, count);
            src.CopyTo(dst);
        }

        /// <summary>
        /// Reads a single element from the memory location.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pSrc">Pointer to memory location</param>
        /// <returns>The read value</returns>
        public static unsafe T Read<T>(IntPtr pSrc) where T : struct
        {
			//!!!!betauser
			var buffer = new ReadOnlySpan<byte>( (void*)pSrc, Marshal.SizeOf( typeof( T ) ) );
			return MemoryMarshal.Read<T>( buffer );
			//return (T)Marshal.PtrToStructure( pSrc, typeof( T ) );

            //return Unsafe.ReadUnaligned<T>(pSrc.ToPointer());
        }

        /// <summary>
        /// Reads a single element from the memory location.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pSrc">Pointer to memory location</param>
        /// <param name="value">The read value.</param>
        public static unsafe void Read<T>(IntPtr pSrc, out T value) where T : struct
        {
			//!!!!betauser
			var buffer = new ReadOnlySpan<byte>( (void*)pSrc, Marshal.SizeOf( typeof( T ) ) );
			value = MemoryMarshal.Read<T>( buffer );
			//value = (T)Marshal.PtrToStructure( pSrc, typeof( T ) );

			//value = Unsafe.ReadUnaligned<T>(pSrc.ToPointer());
		}

		/// <summary>
		/// Writes data from the array to the memory location.
		/// </summary>
		/// <typeparam name="T">Struct type</typeparam>
		/// <param name="pDest">Pointer to memory location</param>
		/// <param name="data">Array containing data to write</param>
		/// <param name="startIndexInArray">Zero-based element index to start reading data from in the element array.</param>
		/// <param name="count">Number of elements to copy</param>
		public static unsafe void Write<T>(IntPtr pDest, T[] data, int startIndexInArray, int count) where T : struct
        {
            ReadOnlySpan<T> src = new ReadOnlySpan<T>(data, startIndexInArray, count);
            Span<T> dst = new Span<T>(pDest.ToPointer(), count);
            src.CopyTo(dst);
        }

        /// <summary>
        /// Writes data from the list to the memory location.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pDest">Pointer to memory location</param>
        /// <param name="data">List containing data to write</param>
        /// <param name="startIndexInArray">Zero-based element index to start reading data from in the element array.</param>
        /// <param name="count">Number of elements to copy</param>
        public static unsafe void Write<T>(IntPtr pDest, List<T> data, int startIndexInArray, int count) where T : struct
        {
#if NET5_0_OR_GREATER
            ReadOnlySpan<T> src = CollectionsMarshal.AsSpan(data).Slice(startIndexInArray, count);
            Span<T> dst = new Span<T>(pDest.ToPointer(), count);
            src.CopyTo(dst);
#else
			//!!!!betauser

			int sizeT = Marshal.SizeOf( typeof( T ) );
			for( int i = 0; i < count; i++ )
			{
				Marshal.StructureToPtr( data[ startIndexInArray + i ], pDest + i * sizeT, false );
			}

			//int sizeT = Unsafe.SizeOf<T>();
			//         for (int i = 0; i < count; i++)
			//         {
			//             Unsafe.Write((void*)(pDest + i * sizeT), data[i]);
			//         }
#endif
		}

		/// <summary>
		/// Writes a single element to the memory location.
		/// </summary>
		/// <typeparam name="T">Struct type</typeparam>
		/// <param name="pDest">Pointer to memory location</param>
		/// <param name="data">The value to write</param>
		public static unsafe void Write<T>(IntPtr pDest, in T data) where T : struct
        {
			//!!!!betauser
			Marshal.StructureToPtr( data, pDest, false );

			//Unsafe.WriteUnaligned<T>(pDest.ToPointer(), data);
		}

		#endregion

		#region Misc

		//Helper for asking if the IMarshalable's in the array have native structs that are blittable.
		private static bool IsNativeBlittable<Managed, Native>(IReadOnlyCollection<Managed> managedColl)
            where Managed : class, IMarshalable<Managed, Native>, new()
            where Native : struct
        {

            if(managedColl == null || managedColl.Count == 0)
                return false;

            foreach(Managed managedValue in managedColl)
            {
                if(managedValue != null)
                    return managedValue.IsNativeBlittable;
            }

            return false;
        }

        //Helper for getting a native custom marshaler
        private static bool HasNativeCustomMarshaler(Type type, out INativeCustomMarshaler marshaler)
        {
            marshaler = null;

            if (type == null)
                return false;

            lock(s_customMarshalers)
            {
                if(!s_customMarshalers.TryGetValue(type, out marshaler))
                {
                    marshaler = type.GetCustomAttribute<NativeCustomMarshalerAttribute>(false)?.Marshaler;
                    s_customMarshalers.Add(type, marshaler);
                }
            }

            return marshaler != null;
        }

        #endregion
    }
}
