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
using Assimp.Unmanaged;

namespace Assimp
{
    /// <summary>
    /// Describes morph-based keyframe animations for a single mesh or a group of meshes.
    /// </summary>
    public sealed class MeshMorphAnimationChannel : IMarshalable<MeshMorphAnimationChannel, AiMeshMorphAnim>
    {
        private String m_name;
        private List<MeshMorphKey> m_meshMorphkeys;

        /// <summary>
        /// Gets or sets the name of the mesh to be animated. Empty strings are not allowed,
        /// animation meshes need to be named (not necessarily uniquely, the name can basically
        /// serve as a wildcard to select a group of meshes with similar animation setup).
        /// </summary>
        public String Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets the number of mesh morph keys in this animation channel. There will always be at least one key.
        /// </summary>
        public int MeshMorphKeyCount
        {
            get
            {
                return m_meshMorphkeys.Count;
            }
        }

        /// <summary>
        /// Gets if this animation channel has mesh keys - this should always be true.
        /// </summary>
        public bool HasMeshMorphKeys
        {
            get
            {
                return m_meshMorphkeys.Count > 0;
            }
        }

        /// <summary>
        /// Gets the mesh morph keyframes of the animation. This should not be null.
        /// </summary>
        public List<MeshMorphKey> MeshMorphKeys
        {
            get
            {
                return m_meshMorphkeys;
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MeshMorphAnimationChannel" /> class.
        /// </summary>
        public MeshMorphAnimationChannel()
        {
            m_name = String.Empty;
            m_meshMorphkeys = new List<MeshMorphKey>();
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<MeshMorphAnimationChannel, AiMeshMorphAnim>.IsNativeBlittable { get { return true; } }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<MeshMorphAnimationChannel, AiMeshMorphAnim>.FromNative(in AiMeshMorphAnim nativeValue)
        {
            m_name = AiString.GetString(nativeValue.Name); //Avoid struct copy
            m_meshMorphkeys.Clear();

            if(nativeValue.NumKeys > 0 && nativeValue.Keys != IntPtr.Zero)
                m_meshMorphkeys.AddRange(MemoryHelper.FromNativeArray<MeshMorphKey, AiMeshMorphKey>(nativeValue.Keys, (int) nativeValue.NumKeys, false));
        }

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<MeshMorphAnimationChannel, AiMeshMorphAnim>.ToNative(IntPtr thisPtr, out AiMeshMorphAnim nativeValue)
        {
            nativeValue.Name = new AiString(m_name);
            nativeValue.NumKeys = (uint) MeshMorphKeyCount;
            nativeValue.Keys = IntPtr.Zero;

            if(nativeValue.NumKeys > 0)
                nativeValue.Keys = MemoryHelper.ToNativeArray<MeshMorphKey, AiMeshMorphKey>(m_meshMorphkeys.ToArray(), false);
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="IMarshalable{MeshMorphAnimationChannel, AiMeshMorphAnim}.ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative)
        {
            if(nativeValue == IntPtr.Zero)
                return;

            AiMeshMorphAnim aiMeshMorphAnim = MemoryHelper.Read<AiMeshMorphAnim>(nativeValue);

            if(aiMeshMorphAnim.NumKeys > 0 && aiMeshMorphAnim.Keys != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiMeshMorphKey>(aiMeshMorphAnim.Keys, (int) aiMeshMorphAnim.NumKeys, MeshMorphKey.FreeNative, false);

            if(freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
