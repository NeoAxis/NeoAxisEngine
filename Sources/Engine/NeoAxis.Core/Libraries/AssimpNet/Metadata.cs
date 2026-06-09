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
using System.Collections.Generic;
using Internal.Assimp.Unmanaged;
using System.Numerics;

namespace Internal.Assimp
{
    /// <summary>
    /// Represents a container for holding metadata, representing as key-value pairs.
    /// </summary>
    public sealed class Metadata : Dictionary<string, Metadata.Entry>, IMarshalable<Metadata, AiMetadata>
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="Metadata"/> class.
        /// </summary>
        public Metadata() { }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<Metadata, AiMetadata>.IsNativeBlittable => true;

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<Metadata, AiMetadata>.ToNative(IntPtr thisPtr, out AiMetadata nativeValue)
        {
            nativeValue = new AiMetadata();
            nativeValue.NumProperties = (uint) Count;

            AiString[] keys = new AiString[Count];
            AiMetadataEntry[] entries = new AiMetadataEntry[Count];
            int index = 0;
            foreach(KeyValuePair<string, Entry> kv in this)
            {
                AiMetadataEntry entry = new AiMetadataEntry();
                entry.DataType = kv.Value.DataType;

                switch(kv.Value.DataType)
                {
                    case MetaDataType.Bool:
                        entry.Data = MemoryHelper.AllocateMemory(sizeof(bool));
                        bool boolValue = (bool) kv.Value.Data;
                        MemoryHelper.Write<bool>(entry.Data, boolValue);
                        break;
                    case MetaDataType.Float:
                        entry.Data = MemoryHelper.AllocateMemory(sizeof(float));
                        float floatValue = (float) kv.Value.Data;
                        MemoryHelper.Write<float>(entry.Data, floatValue);
                        break;
                    case MetaDataType.Double:
                        entry.Data = MemoryHelper.AllocateMemory(sizeof(double));
                        double doubleValue = (double) kv.Value.Data;
                        MemoryHelper.Write<double>(entry.Data, doubleValue);
                        break;
                    case MetaDataType.Int32:
                        entry.Data = MemoryHelper.AllocateMemory(sizeof(int));
                        int intValue = (int) kv.Value.Data;
                        MemoryHelper.Write<int>(entry.Data, intValue);
                        break;
                    case MetaDataType.String:
                        entry.Data = MemoryHelper.AllocateMemory(MemoryHelper.SizeOf<AiString>());
                        AiString aiStringValue = new AiString(kv.Value.Data as string);
                        MemoryHelper.Write<AiString>(entry.Data, aiStringValue);
                        break;
                    case MetaDataType.UInt64:
                        entry.Data = MemoryHelper.AllocateMemory(sizeof(ulong));
                        ulong uint64Value = (ulong) kv.Value.Data;
                        MemoryHelper.Write<ulong>(entry.Data, uint64Value);
                        break;
                    case MetaDataType.Vector3:
                        entry.Data = MemoryHelper.AllocateMemory(MemoryHelper.SizeOf<Vector3>());
                        Vector3 vectorValue = (Vector3) kv.Value.Data;
                        MemoryHelper.Write<Vector3>(entry.Data, vectorValue);
                        break;
                }

                keys[index] = new AiString(kv.Key);
                entries[index] = entry;
                index++;
            }

            nativeValue.keys = MemoryHelper.ToNativeArray<AiString>(keys);
            nativeValue.Values = MemoryHelper.ToNativeArray<AiMetadataEntry>(entries);
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<Metadata, AiMetadata>.FromNative(in AiMetadata nativeValue)
        {
            Clear();

            if(nativeValue.NumProperties == 0 || nativeValue.keys == IntPtr.Zero || nativeValue.Values == IntPtr.Zero)
                return;

            AiString[] keys = MemoryHelper.FromNativeArray<AiString>(nativeValue.keys, (int) nativeValue.NumProperties);
            AiMetadataEntry[] entries = MemoryHelper.FromNativeArray<AiMetadataEntry>(nativeValue.Values, (int) nativeValue.NumProperties);

            for(int i = 0; i < nativeValue.NumProperties; i++)
            {
                string key = keys[i].GetString();
                AiMetadataEntry entry = entries[i];

                if(string.IsNullOrEmpty(key) || entry.Data == IntPtr.Zero)
                    continue;

                object data = null;
                switch(entry.DataType)
                {
                    case MetaDataType.Bool:
                        data = MemoryHelper.Read<bool>(entry.Data);
                        break;
                    case MetaDataType.Float:
                        data = MemoryHelper.Read<float>(entry.Data);
                        break;
                    case MetaDataType.Double:
                        data = MemoryHelper.Read<double>(entry.Data);
                        break;
                    case MetaDataType.Int32:
                        data = MemoryHelper.Read<int>(entry.Data);
                        break;
                    case MetaDataType.String:
                        AiString aiString = MemoryHelper.Read<AiString>(entry.Data);
                        data = aiString.GetString();
                        break;
                    case MetaDataType.UInt64:
                        data = MemoryHelper.Read<ulong>(entry.Data);
                        break;
                    case MetaDataType.Vector3:
                        data = MemoryHelper.Read<Vector3>(entry.Data);
                        break;
                }

                if(data != null)
                    Add(key, new Entry(entry.DataType, data));
            }
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="IMarshalable{Metadata, AiMetadata}.ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative)
        {
            if(nativeValue == IntPtr.Zero)
                return;

            AiMetadata aiMetadata = MemoryHelper.MarshalStructure<AiMetadata>(nativeValue);

            if(aiMetadata.keys != IntPtr.Zero)
                MemoryHelper.FreeMemory(aiMetadata.keys);

            if(aiMetadata.Values != IntPtr.Zero)
            {
                AiMetadataEntry[] entries = MemoryHelper.FromNativeArray<AiMetadataEntry>(aiMetadata.Values, (int) aiMetadata.NumProperties);

                foreach(AiMetadataEntry entry in entries)
                {
                    if(entry.Data != IntPtr.Zero)
                        MemoryHelper.FreeMemory(entry.Data);
                }

                MemoryHelper.FreeMemory(aiMetadata.Values);
            }

            if(freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

		#endregion



		//!!!!betauser

		/// <summary>
		/// Represents an entry in a metadata container.
		/// </summary>
		public readonly struct Entry : IEquatable<Entry>
		{
			/// <summary>
			/// Gets the type of metadata.
			/// </summary>
			public MetaDataType DataType { get; }

			/// <summary>
			/// Gets the metadata data stored in this entry.
			/// </summary>
			public object Data { get; }

			/// <summary>
			/// Constructs a new instance of the <see cref="Entry"/> struct.
			/// </summary>
			/// <param name="dataType">Type of metadata.</param>
			/// <param name="data">Metadata data stored in this entry.</param>
			public Entry( MetaDataType dataType, object data )
			{
				DataType = dataType;
				Data = data;
			}

			/// <summary>
			/// Gets the data as the specified type. If it cannot be casted to the type, then null is returned.
			/// </summary>
			/// <typeparam name="T">Type to cast the data to.</typeparam>
			/// <returns>Casted data or null.</returns>
			public T? DataAs<T>() where T : struct
			{
				Type dataTypeType = null;
				switch( DataType )
				{
				case MetaDataType.Bool:
					dataTypeType = typeof( bool );
					break;
				case MetaDataType.Float:
					dataTypeType = typeof( float );
					break;
				case MetaDataType.Double:
					dataTypeType = typeof( double );
					break;
				case MetaDataType.Int32:
					dataTypeType = typeof( int );
					break;
				case MetaDataType.String:
					dataTypeType = typeof( string );
					break;
				case MetaDataType.UInt64:
					dataTypeType = typeof( ulong );
					break;
				case MetaDataType.Vector3:
					dataTypeType = typeof( Vector3 );
					break;
				}

				if( dataTypeType == typeof( T ) )
					return (T)Data;

				return null;
			}

			/// <inheritdoc/>
			public bool Equals( Entry other ) => DataType == other.DataType && Equals( Data, other.Data );

			/// <inheritdoc/>
			public override bool Equals( object obj ) => obj is Entry other && Equals( other );

			/// <inheritdoc/>
			public override int GetHashCode()
			{
				unchecked
				{
					return ( (int)DataType * 397 ) ^ ( Data != null ? Data.GetHashCode() : 0 );
				}
			}

			/// <inheritdoc/>
			public static bool operator ==( Entry left, Entry right ) => left.Equals( right );

			/// <inheritdoc/>
			public static bool operator !=( Entry left, Entry right ) => !left.Equals( right );
		}


		///// <summary>
		///// Represents an entry in a metadata container.
		///// </summary>
		///// <param name="DataType">Type of metadata.</param>
		///// <param name="Data">Metadata data stored in this entry.</param>
		//public readonly record struct Entry( MetaDataType DataType, object Data ) : IEquatable<Entry>
		//{
		//	/// <summary>
		//	/// Gets the data as the specified type. If it cannot be casted to the type, then null is returned.
		//	/// </summary>
		//	/// <typeparam name="T">Type to cast the data to.</typeparam>
		//	/// <returns>Casted data or null.</returns>
		//	public T? DataAs<T>() where T : struct
		//	{
		//		Type dataTypeType = null;
		//		switch( DataType )
		//		{
		//		case MetaDataType.Bool:
		//			dataTypeType = typeof( bool );
		//			break;
		//		case MetaDataType.Float:
		//			dataTypeType = typeof( float );
		//			break;
		//		case MetaDataType.Double:
		//			dataTypeType = typeof( double );
		//			break;
		//		case MetaDataType.Int32:
		//			dataTypeType = typeof( int );
		//			break;
		//		case MetaDataType.String:
		//			dataTypeType = typeof( string );
		//			break;
		//		case MetaDataType.UInt64:
		//			dataTypeType = typeof( ulong );
		//			break;
		//		case MetaDataType.Vector3:
		//			dataTypeType = typeof( Vector3 );
		//			break;
		//		}

		//		if( dataTypeType == typeof( T ) )
		//			return (T)Data;

		//		return null;
		//	}
		//}

	}
}
