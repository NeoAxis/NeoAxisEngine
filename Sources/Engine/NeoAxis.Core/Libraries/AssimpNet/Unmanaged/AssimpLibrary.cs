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
using System.Numerics;
using System.Runtime.InteropServices;

namespace Internal.Assimp.Unmanaged
{
    /// <summary>
    /// Singleton that governs access to the unmanaged Assimp library functions.
    /// </summary>
    public sealed class AssimpLibrary : UnmanagedLibrary
    {
        private static readonly object s_sync = new object();

        /// <summary>
        /// Default name of the unmanaged library. Based on runtime implementation the prefix ("lib" on non-windows) and extension (.dll, .so, .dylib) will be appended automatically.
        /// </summary>
        private const string DefaultLibName = "assimp";

        private static AssimpLibrary s_instance;

        private bool m_enableVerboseLogging = false;

        /// <summary>
        /// Gets the AssimpLibrary instance.
        /// </summary>
        public static AssimpLibrary Instance
        {
            get
            {
                lock(s_sync)
                {
                    if(s_instance == null)
                        s_instance = CreateInstance();

                    return s_instance;
                }
            }
        }

        /// <summary>
        /// Gets if the Assimp unmanaged library supports multithreading. If it was compiled for single threading only,
        /// then it will not utilize multiple threads during import.
        /// </summary>
        public bool IsMultithreadingSupported => !((GetCompileFlags() & CompileFlags.SingleThreaded) == CompileFlags.SingleThreaded);

        private AssimpLibrary(string defaultLibName, Type[] unmanagedFunctionDelegateTypes)
            : base(defaultLibName, unmanagedFunctionDelegateTypes) { }

        private static AssimpLibrary CreateInstance()
        {
            return new AssimpLibrary(DefaultLibName, typeof(Functions).GetNestedTypes());
        }

        #region Import Methods

        /// <summary>
        /// Imports a file.
        /// </summary>
        /// <param name="file">Valid filename</param>
        /// <param name="flags">Post process flags specifying what steps are to be run after the import.</param>
        /// <param name="propStore">Property store containing config name-values, may be null.</param>
        /// <returns>Pointer to the unmanaged data structure.</returns>
        public IntPtr ImportFile(string file, PostProcessSteps flags, IntPtr propStore)
        {
            return ImportFile(file, flags, IntPtr.Zero, propStore);
        }

        /// <summary>
        /// Imports a file.
        /// </summary>
        /// <param name="file">Valid filename</param>
        /// <param name="flags">Post process flags specifying what steps are to be run after the import.</param>
        /// <param name="fileIO">Pointer to an instance of AiFileIO, a custom file IO system used to open the model and 
        /// any associated file the loader needs to open, passing NULL uses the default implementation.</param>
        /// <param name="propStore">Property store containing config name-values, may be null.</param>
        /// <returns>Pointer to the unmanaged data structure.</returns>
        public IntPtr ImportFile(string file, PostProcessSteps flags, IntPtr fileIO, IntPtr propStore)
        {
            LoadIfNotLoaded();

            Functions.aiImportFileExWithProperties func = GetFunction<Functions.aiImportFileExWithProperties>(FunctionNames.aiImportFileExWithProperties);

            var scenePtr = func(file, (uint) flags, fileIO, propStore);
            FixQuaternionsInSceneFromAssimp(scenePtr);
            return scenePtr;
        }

        /// <summary>
        /// Imports a scene from a stream. This uses the "aiImportFileFromMemory" function. The stream can be from anyplace,
        /// not just a memory stream. It is up to the caller to dispose of the stream.
        /// </summary>
        /// <param name="stream">Stream containing the scene data</param>
        /// <param name="flags">Post processing flags</param>
        /// <param name="formatHint">A hint to Assimp to decide which importer to use to process the data</param>
        /// <param name="propStore">Property store containing the config name-values, may be null.</param>
        /// <returns>Pointer to the unmanaged data structure.</returns>
        public IntPtr ImportFileFromStream(Stream stream, PostProcessSteps flags, string formatHint, IntPtr propStore)
        {
            LoadIfNotLoaded();

            Functions.aiImportFileFromMemoryWithProperties func = GetFunction<Functions.aiImportFileFromMemoryWithProperties>(FunctionNames.aiImportFileFromMemoryWithProperties);

            byte[] buffer = MemoryHelper.ReadStreamFully(stream, 0);

            var scenePtr = func(buffer, (uint) buffer.Length, (uint) flags, formatHint, propStore);
            FixQuaternionsInSceneFromAssimp(scenePtr);
            return scenePtr;
        }

        /// <summary>
        /// Releases the unmanaged scene data structure. This should NOT be used for unmanaged scenes that were marshaled
        /// from the managed scene structure - only for scenes whose memory was allocated by the native library!
        /// </summary>
        /// <param name="scene">Pointer to the unmanaged scene data structure.</param>
        public void ReleaseImport(IntPtr scene)
        {
            LoadIfNotLoaded();

            if(scene == IntPtr.Zero)
                return;

            Functions.aiReleaseImport func = GetFunction<Functions.aiReleaseImport>(FunctionNames.aiReleaseImport);

            func(scene);
        }

        /// <summary>
        /// Applies a post-processing step on an already imported scene.
        /// </summary>
        /// <param name="scene">Pointer to the unmanaged scene data structure.</param>
        /// <param name="flags">Post processing steps to run.</param>
        /// <returns>Pointer to the unmanaged scene data structure.</returns>
        public IntPtr ApplyPostProcessing(IntPtr scene, PostProcessSteps flags)
        {
            LoadIfNotLoaded();

            if(scene == IntPtr.Zero)
                return IntPtr.Zero;

            Functions.aiApplyPostProcessing func = GetFunction<Functions.aiApplyPostProcessing>(FunctionNames.aiApplyPostProcessing);

            FixQuaternionsInSceneToAssimp(scene);
            var scenePtr = func(scene, (uint) flags);
            FixQuaternionsInSceneFromAssimp(scenePtr);
            return scenePtr;
        }

        #endregion

        #region Export Methods

        /// <summary>
        /// Gets all supported export formats.
        /// </summary>
        /// <returns>Array of supported export formats.</returns>
        public ExportFormatDescription[] GetExportFormatDescriptions()
        {
            LoadIfNotLoaded();

            int count = (int) GetFunction<Functions.aiGetExportFormatCount>(FunctionNames.aiGetExportFormatCount)().ToUInt32();

            if(count == 0)
                return Array.Empty<ExportFormatDescription>();

            ExportFormatDescription[] descriptions = new ExportFormatDescription[count];

            Functions.aiGetExportFormatDescription func = GetFunction<Functions.aiGetExportFormatDescription>(FunctionNames.aiGetExportFormatDescription);
            Functions.aiReleaseExportFormatDescription releaseFunc = GetFunction<Functions.aiReleaseExportFormatDescription>(FunctionNames.aiReleaseExportFormatDescription);

            for(int i = 0; i < count; i++)
            {
                IntPtr formatDescPtr = func(new UIntPtr((uint) i));
                if(formatDescPtr != IntPtr.Zero)
                {
                    AiExportFormatDesc desc = MemoryHelper.Read<AiExportFormatDesc>(formatDescPtr);
                    descriptions[i] = new ExportFormatDescription(desc);

                    releaseFunc(formatDescPtr);
                }
            }

            return descriptions;
        }


        /// <summary>
        /// Exports the given scene to a chosen file format. Returns the exported data as a binary blob which you can embed into another data structure or file.
        /// </summary>
        /// <param name="scene">Scene to export, it is the responsibility of the caller to free this when finished.</param>
        /// <param name="formatId">Format id describing which format to export to.</param>
        /// <param name="preProcessing">Pre processing flags to operate on the scene during the export.</param>
        /// <returns>Exported binary blob, or null if there was an error.</returns>
        public ExportDataBlob ExportSceneToBlob(IntPtr scene, string formatId, PostProcessSteps preProcessing)
        {
            LoadIfNotLoaded();

            if(string.IsNullOrEmpty(formatId) || scene == IntPtr.Zero)
                return null;

            Functions.aiExportSceneToBlob exportBlobFunc = GetFunction<Functions.aiExportSceneToBlob>(FunctionNames.aiExportSceneToBlob);
            Functions.aiReleaseExportBlob releaseExportBlobFunc = GetFunction<Functions.aiReleaseExportBlob>(FunctionNames.aiReleaseExportBlob);

            FixQuaternionsInSceneToAssimp(scene);
            IntPtr blobPtr = exportBlobFunc(scene, formatId, (uint) preProcessing);
            FixQuaternionsInSceneFromAssimp(scene);

            if(blobPtr == IntPtr.Zero)
                return null;

            AiExportDataBlob blob = MemoryHelper.Read<AiExportDataBlob>(blobPtr);
            ExportDataBlob dataBlob = new ExportDataBlob(ref blob);
            releaseExportBlobFunc(blobPtr);

            return dataBlob;
        }

        /// <summary>
        /// Exports the given scene to a chosen file format and writes the result file(s) to disk.
        /// </summary>
        /// <param name="scene">The scene to export, which needs to be freed by the caller. The scene is expected to conform to Assimp's Importer output format. In short,
        /// this means the model data should use a right handed coordinate system, face winding should be counter clockwise, and the UV coordinate origin assumed to be upper left. If the input is different, specify the pre processing flags appropiately.</param>
        /// <param name="formatId">Format id describing which format to export to.</param>
        /// <param name="fileName">Output filename to write to</param>
        /// <param name="preProcessing">Pre processing flags - accepts any post processing step flag. In reality only a small subset are actually supported, e.g. to ensure the input
        /// conforms to the standard Assimp output format. Some may be redundant, such as triangulation, which some exporters may have to enforce due to the export format.</param>
        /// <returns>Return code specifying if the operation was a success.</returns>
        public ReturnCode ExportScene(IntPtr scene, string formatId, string fileName, PostProcessSteps preProcessing)
        {
            return ExportScene(scene, formatId, fileName, IntPtr.Zero, preProcessing);
        }

        /// <summary>
        /// Exports the given scene to a chosen file format and writes the result file(s) to disk.
        /// </summary>
        /// <param name="scene">The scene to export, which needs to be freed by the caller. The scene is expected to conform to Assimp's Importer output format. In short,
        /// this means the model data should use a right handed coordinate system, face winding should be counter clockwise, and the UV coordinate origin assumed to be upper left. If the input is different, specify the pre processing flags appropiately.</param>
        /// <param name="formatId">Format id describing which format to export to.</param>
        /// <param name="fileName">Output filename to write to</param>
        /// <param name="fileIO">Pointer to an instance of AiFileIO, a custom file IO system used to open the model and 
        /// any associated file the loader needs to open, passing NULL uses the default implementation.</param>
        /// <param name="preProcessing">Pre processing flags - accepts any post processing step flag. In reality only a small subset are actually supported, e.g. to ensure the input
        /// conforms to the standard Assimp output format. Some may be redundant, such as triangulation, which some exporters may have to enforce due to the export format.</param>
        /// <returns>Return code specifying if the operation was a success.</returns>
        public ReturnCode ExportScene(IntPtr scene, string formatId, string fileName, IntPtr fileIO, PostProcessSteps preProcessing)
        {
            LoadIfNotLoaded();

            if(string.IsNullOrEmpty(formatId) || scene == IntPtr.Zero)
                return ReturnCode.Failure;

            Functions.aiExportSceneEx exportFunc = GetFunction<Functions.aiExportSceneEx>(FunctionNames.aiExportSceneEx);

            FixQuaternionsInSceneToAssimp(scene);
            var ret = exportFunc(scene, formatId, fileName, fileIO, (uint) preProcessing);
            FixQuaternionsInSceneFromAssimp(scene);
            return ret;
        }

        /// <summary>
        /// Creates a modifyable copy of a scene, useful for copying the scene that was imported so its topology can be modified
        /// and the scene be exported.
        /// </summary>
        /// <param name="sceneToCopy">Valid scene to be copied</param>
        /// <returns>Modifyable copy of the scene</returns>
        public IntPtr CopyScene(IntPtr sceneToCopy)
        {
            if(sceneToCopy == IntPtr.Zero)
                return IntPtr.Zero;

            IntPtr copiedScene;

            Functions.aiCopyScene func = GetFunction<Functions.aiCopyScene>(FunctionNames.aiCopyScene);

            func(sceneToCopy, out copiedScene);

            return copiedScene;
        }

        #endregion

        #region Logging Methods

        /// <summary>
        /// Attaches a log stream callback to catch Assimp messages.
        /// </summary>
        /// <param name="logStreamPtr">Pointer to an instance of AiLogStream.</param>
        public void AttachLogStream(IntPtr logStreamPtr)
        {
            LoadIfNotLoaded();

            Functions.aiAttachLogStream func = GetFunction<Functions.aiAttachLogStream>(FunctionNames.aiAttachLogStream);

            func(logStreamPtr);
        }

        /// <summary>
        /// Enables verbose logging.
        /// </summary>
        /// <param name="enable">True if verbose logging is to be enabled or not.</param>
        public void EnableVerboseLogging(bool enable)
        {
            LoadIfNotLoaded();

            Functions.aiEnableVerboseLogging func = GetFunction<Functions.aiEnableVerboseLogging>(FunctionNames.aiEnableVerboseLogging);

            func(enable);

            m_enableVerboseLogging = enable;
        }

        /// <summary>
        /// Gets if verbose logging is enabled.
        /// </summary>
        /// <returns>True if verbose logging is enabled, false otherwise.</returns>
        public bool GetVerboseLoggingEnabled()
        {
            return m_enableVerboseLogging;
        }

        /// <summary>
        /// Detaches a logstream callback.
        /// </summary>
        /// <param name="logStreamPtr">Pointer to an instance of AiLogStream.</param>
        /// <returns>A return code signifying if the function was successful or not.</returns>
        public ReturnCode DetachLogStream(IntPtr logStreamPtr)
        {
            LoadIfNotLoaded();

            Functions.aiDetachLogStream func = GetFunction<Functions.aiDetachLogStream>(FunctionNames.aiDetachLogStream);

            return func(logStreamPtr);
        }

        /// <summary>
        /// Detaches all logstream callbacks currently attached to Assimp.
        /// </summary>
        public void DetachAllLogStreams()
        {
            LoadIfNotLoaded();

            Functions.aiDetachAllLogStreams func = GetFunction<Functions.aiDetachAllLogStreams>(FunctionNames.aiDetachAllLogStreams);

            func();
        }

        #endregion

        #region Import Properties Setters

        /// <summary>
        /// Create an empty property store. Property stores are used to collect import settings.
        /// </summary>
        /// <returns>Pointer to property store</returns>
        public IntPtr CreatePropertyStore()
        {
            LoadIfNotLoaded();

            Functions.aiCreatePropertyStore func = GetFunction<Functions.aiCreatePropertyStore>(FunctionNames.aiCreatePropertyStore);

            return func();
        }

        /// <summary>
        /// Deletes a property store.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        public void ReleasePropertyStore(IntPtr propertyStore)
        {
            LoadIfNotLoaded();

            if(propertyStore == IntPtr.Zero)
                return;

            Functions.aiReleasePropertyStore func = GetFunction<Functions.aiReleasePropertyStore>(FunctionNames.aiReleasePropertyStore);

            func(propertyStore);
        }

        /// <summary>
        /// Sets an integer property value.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        public void SetImportPropertyInteger(IntPtr propertyStore, string name, int value)
        {
            LoadIfNotLoaded();

            if(propertyStore == IntPtr.Zero || string.IsNullOrEmpty(name))
                return;

            Functions.aiSetImportPropertyInteger func = GetFunction<Functions.aiSetImportPropertyInteger>(FunctionNames.aiSetImportPropertyInteger);

            func(propertyStore, name, value);
        }

        /// <summary>
        /// Sets a float property value.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        public void SetImportPropertyFloat(IntPtr propertyStore, string name, float value)
        {
            LoadIfNotLoaded();

            if(propertyStore == IntPtr.Zero || string.IsNullOrEmpty(name))
                return;

            Functions.aiSetImportPropertyFloat func = GetFunction<Functions.aiSetImportPropertyFloat>(FunctionNames.aiSetImportPropertyFloat);

            func(propertyStore, name, value);
        }

        /// <summary>
        /// Sets a string property value.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        public void SetImportPropertyString(IntPtr propertyStore, string name, string value)
        {
            LoadIfNotLoaded();

            if(propertyStore == IntPtr.Zero || string.IsNullOrEmpty(name))
                return;

            Functions.aiSetImportPropertyString func = GetFunction<Functions.aiSetImportPropertyString>(FunctionNames.aiSetImportPropertyString);

            AiString str = new AiString();
            if(str.SetString(value))
                func(propertyStore, name, ref str);
        }

        /// <summary>
        /// Sets a matrix property value.
        /// </summary>
        /// <param name="propertyStore">Pointer to property store</param>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        public void SetImportPropertyMatrix(IntPtr propertyStore, string name, Matrix4x4 value)
        {
            LoadIfNotLoaded();

            if(propertyStore == IntPtr.Zero || string.IsNullOrEmpty(name))
                return;

            Functions.aiSetImportPropertyMatrix func = GetFunction<Functions.aiSetImportPropertyMatrix>(FunctionNames.aiSetImportPropertyMatrix);
            func(propertyStore, name, ref value);
        }

        #endregion

        #region Material Getters

        /// <summary>
        /// Retrieves a color value from the material property table.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <returns>The color if it exists. If not, the default Vector4 value is returned.</returns>
        public Vector4 GetMaterialColor(ref AiMaterial mat, string key, TextureType texType, uint texIndex)
        {
            LoadIfNotLoaded();

            Functions.aiGetMaterialColor func = GetFunction<Functions.aiGetMaterialColor>(FunctionNames.aiGetMaterialColor);

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = MemoryHelper.AllocateMemory(MemoryHelper.SizeOf<Vector4>());
                ReturnCode code = func(ref mat, key, (uint) texType, texIndex, ptr);
                Vector4 color = new Vector4();
                if(code == ReturnCode.Success && ptr != IntPtr.Zero)
                    color = MemoryHelper.Read<Vector4>(ptr);

                return color;
            }
            finally
            {
                if(ptr != IntPtr.Zero)
                    MemoryHelper.FreeMemory(ptr);
            }
        }

        /// <summary>
        /// Retrieves an array of float values with the specific key from the material.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <param name="floatCount">The maximum number of floats to read. This may not accurately describe the data returned, as it may not exist or be smaller. If this value is less than
        /// the available floats, then only the requested number is returned (e.g. 1 or 2 out of a 4 float array).</param>
        /// <returns>The float array, if it exists</returns>
        public float[] GetMaterialFloatArray(ref AiMaterial mat, string key, TextureType texType, uint texIndex, uint floatCount)
        {
            LoadIfNotLoaded();

            Functions.aiGetMaterialFloatArray func = GetFunction<Functions.aiGetMaterialFloatArray>(FunctionNames.aiGetMaterialFloatArray);

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = MemoryHelper.AllocateMemory(IntPtr.Size);
                ReturnCode code = func(ref mat, key, (uint) texType, texIndex, ptr, ref floatCount);
                float[] array = null;
                if(code == ReturnCode.Success && floatCount > 0)
                {
                    array = new float[floatCount];
                    MemoryHelper.Read<float>(ptr, array, 0, (int) floatCount);
                }
                return array;
            }
            finally
            {
                if(ptr != IntPtr.Zero)
                {
                    MemoryHelper.FreeMemory(ptr);
                }
            }
        }

        /// <summary>
        /// Retrieves an array of integer values with the specific key from the material.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <param name="intCount">The maximum number of integers to read. This may not accurately describe the data returned, as it may not exist or be smaller. If this value is less than
        /// the available integers, then only the requested number is returned (e.g. 1 or 2 out of a 4 float array).</param>
        /// <returns>The integer array, if it exists</returns>
        public int[] GetMaterialIntegerArray(ref AiMaterial mat, string key, TextureType texType, uint texIndex, uint intCount)
        {
            LoadIfNotLoaded();

            Functions.aiGetMaterialIntegerArray func = GetFunction<Functions.aiGetMaterialIntegerArray>(FunctionNames.aiGetMaterialIntegerArray);

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = MemoryHelper.AllocateMemory(IntPtr.Size);
                ReturnCode code = func(ref mat, key, (uint) texType, texIndex, ptr, ref intCount);
                int[] array = null;
                if(code == ReturnCode.Success && intCount > 0)
                {
                    array = new int[intCount];
                    MemoryHelper.Read<int>(ptr, array, 0, (int) intCount);
                }
                return array;
            }
            finally
            {
                if(ptr != IntPtr.Zero)
                {
                    MemoryHelper.FreeMemory(ptr);
                }
            }
        }

        /// <summary>
        /// Retrieves a material property with the specific key from the material.
        /// </summary>
        /// <param name="mat">Material to retrieve the property from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <returns>The material property, if found.</returns>
        public AiMaterialProperty GetMaterialProperty(ref AiMaterial mat, string key, TextureType texType, uint texIndex)
        {
            LoadIfNotLoaded();

            Functions.aiGetMaterialProperty func = GetFunction<Functions.aiGetMaterialProperty>(FunctionNames.aiGetMaterialProperty);

            IntPtr ptr;
            ReturnCode code = func(ref mat, key, (uint) texType, texIndex, out ptr);
            AiMaterialProperty prop = new AiMaterialProperty();
            if(code == ReturnCode.Success && ptr != IntPtr.Zero)
                prop = MemoryHelper.Read<AiMaterialProperty>(ptr);

            return prop;
        }

        /// <summary>
        /// Retrieves a string from the material property table.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="key">Ai mat key (base) name to search for</param>
        /// <param name="texType">Texture Type semantic, always zero for non-texture properties</param>
        /// <param name="texIndex">Texture index, always zero for non-texture properties</param>
        /// <returns>The string, if it exists. If not, an empty string is returned.</returns>
        public string GetMaterialString(ref AiMaterial mat, string key, TextureType texType, uint texIndex)
        {
            LoadIfNotLoaded();

            Functions.aiGetMaterialString func = GetFunction<Functions.aiGetMaterialString>(FunctionNames.aiGetMaterialString);

            AiString str;
            ReturnCode code = func(ref mat, key, (uint) texType, texIndex, out str);
            if(code == ReturnCode.Success)
                return str.GetString();

            return string.Empty;
        }

        /// <summary>
        /// Gets the number of textures contained in the material for a particular texture type.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="type">Texture Type semantic</param>
        /// <returns>The number of textures for the type.</returns>
        public uint GetMaterialTextureCount(ref AiMaterial mat, TextureType type)
        {
            LoadIfNotLoaded();

            Functions.aiGetMaterialTextureCount func = GetFunction<Functions.aiGetMaterialTextureCount>(FunctionNames.aiGetMaterialTextureCount);

            return func(ref mat, type);
        }

        /// <summary>
        /// Gets the texture filepath contained in the material.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="type">Texture type semantic</param>
        /// <param name="index">Texture index</param>
        /// <returns>The texture filepath, if it exists. If not an empty string is returned.</returns>
        public string GetMaterialTextureFilePath(ref AiMaterial mat, TextureType type, uint index)
        {
            LoadIfNotLoaded();

            Functions.aiGetMaterialTexture func = GetFunction<Functions.aiGetMaterialTexture>(FunctionNames.aiGetMaterialTexture);

            AiString str;
            TextureMapping mapping;
            uint uvIndex;
            float blendFactor;
            TextureOperation texOp;
            TextureWrapMode[] wrapModes = new TextureWrapMode[2];
            uint flags;

            ReturnCode code = func(ref mat, type, index, out str, out mapping, out uvIndex, out blendFactor, out texOp, wrapModes, out flags);

            if(code == ReturnCode.Success)
            {
                return str.GetString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets all values pertaining to a particular texture from a material.
        /// </summary>
        /// <param name="mat">Material to retrieve the data from</param>
        /// <param name="type">Texture type semantic</param>
        /// <param name="index">Texture index</param>
        /// <returns>Returns the texture slot struct containing all the information.</returns>
        public TextureSlot GetMaterialTexture(ref AiMaterial mat, TextureType type, uint index)
        {
            LoadIfNotLoaded();

            Functions.aiGetMaterialTexture func = GetFunction<Functions.aiGetMaterialTexture>(FunctionNames.aiGetMaterialTexture);

            AiString str;
            TextureMapping mapping;
            uint uvIndex;
            float blendFactor;
            TextureOperation texOp;
            TextureWrapMode[] wrapModes = new TextureWrapMode[2];
            uint flags;

            ReturnCode code = func(ref mat, type, index, out str, out mapping, out uvIndex, out blendFactor, out texOp, wrapModes, out flags);

            return new TextureSlot(str.GetString(), type, (int) index, mapping, (int) uvIndex, blendFactor, texOp, wrapModes[0], wrapModes[1], (int) flags);
        }

        #endregion

        #region Error and Info Methods

        /// <summary>
        /// Gets the last error logged in Assimp.
        /// </summary>
        /// <returns>The last error message logged.</returns>
        public string GetErrorString()
        {
            LoadIfNotLoaded();

            Functions.aiGetErrorString func = GetFunction<Functions.aiGetErrorString>(FunctionNames.aiGetErrorString);

            IntPtr ptr = func();

            if(ptr == IntPtr.Zero)
                return string.Empty;

            return Marshal.PtrToStringAnsi(ptr);
        }

        /// <summary>
        /// Checks whether the model format extension is supported by Assimp.
        /// </summary>
        /// <param name="extension">Model format extension, e.g. ".3ds"</param>
        /// <returns>True if the format is supported, false otherwise.</returns>
        public bool IsExtensionSupported(string extension)
        {
            LoadIfNotLoaded();

            Functions.aiIsExtensionSupported func = GetFunction<Functions.aiIsExtensionSupported>(FunctionNames.aiIsExtensionSupported);

            return func(extension);
        }

        /// <summary>
        /// Gets all the model format extensions that are currently supported by Assimp.
        /// </summary>
        /// <returns>Array of supported format extensions</returns>
        public string[] GetExtensionList()
        {
            LoadIfNotLoaded();

            Functions.aiGetExtensionList func = GetFunction<Functions.aiGetExtensionList>(FunctionNames.aiGetExtensionList);

            AiString aiString = new AiString();
            func(ref aiString);
            return aiString.GetString().Split(new string[] { "*", ";*" }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Gets a collection of importer descriptions that detail metadata and feature support for each importer.
        /// </summary>
        /// <returns>Collection of importer descriptions</returns>
        public ImporterDescription[] GetImporterDescriptions()
        {
            LoadIfNotLoaded();

            Functions.aiGetImportFormatCount funcGetCount = GetFunction<Functions.aiGetImportFormatCount>(FunctionNames.aiGetImportFormatCount);
            Functions.aiGetImportFormatDescription funcGetDescr = GetFunction<Functions.aiGetImportFormatDescription>(FunctionNames.aiGetImportFormatDescription);

            int count = (int) funcGetCount().ToUInt32();
            ImporterDescription[] descrs = new ImporterDescription[count];

            for(int i = 0; i < count; i++)
            {
                IntPtr descrPtr = funcGetDescr(new UIntPtr((uint) i));
                if(descrPtr != IntPtr.Zero)
                {
					//!!!!betauser
					unsafe
					{
						AiImporterDesc* descr = (AiImporterDesc*)descrPtr;
						descrs[ i ] = new ImporterDescription( descr );
					}

					//ref AiImporterDesc descr = ref MemoryHelper.AsRef<AiImporterDesc>(descrPtr);
     //               descrs[i] = new ImporterDescription(descr);
                }
            }

            return descrs;
        }

        /// <summary>
        /// Gets the memory requirements of the scene.
        /// </summary>
        /// <param name="scene">Pointer to the unmanaged scene data structure.</param>
        /// <returns>The memory information about the scene.</returns>
        public AiMemoryInfo GetMemoryRequirements(IntPtr scene)
        {
            LoadIfNotLoaded();

            Functions.aiGetMemoryRequirements func = GetFunction<Functions.aiGetMemoryRequirements>(FunctionNames.aiGetMemoryRequirements);

            AiMemoryInfo info = new AiMemoryInfo();
            if(scene != IntPtr.Zero)
            {
                func(scene, ref info);
            }

            return info;
        }

        #endregion

        #region Version Info

        /// <summary>
        /// Gets the Assimp legal info.
        /// </summary>
        /// <returns>String containing Assimp legal info.</returns>
        public string GetLegalString()
        {
            LoadIfNotLoaded();

            Functions.aiGetLegalString func = GetFunction<Functions.aiGetLegalString>(FunctionNames.aiGetLegalString);

            IntPtr ptr = func();

            if(ptr == IntPtr.Zero)
                return string.Empty;

            return Marshal.PtrToStringAnsi(ptr);
        }

        /// <summary>
        /// Gets the native Assimp DLL's minor version number.
        /// </summary>
        /// <returns>Assimp minor version number</returns>
        public uint GetVersionMinor()
        {
            LoadIfNotLoaded();

            Functions.aiGetVersionMinor func = GetFunction<Functions.aiGetVersionMinor>(FunctionNames.aiGetVersionMinor);

            return func();
        }

        /// <summary>
        /// Gets the native Assimp DLL's major version number.
        /// </summary>
        /// <returns>Assimp major version number</returns>
        public uint GetVersionMajor()
        {
            LoadIfNotLoaded();

            Functions.aiGetVersionMajor func = GetFunction<Functions.aiGetVersionMajor>(FunctionNames.aiGetVersionMajor);

            return func();
        }

        /// <summary>
        /// Gets the native Assimp DLL's revision version number.
        /// </summary>
        /// <returns>Assimp revision version number</returns>
        public uint GetVersionRevision()
        {
            LoadIfNotLoaded();

            Functions.aiGetVersionRevision func = GetFunction<Functions.aiGetVersionRevision>(FunctionNames.aiGetVersionRevision);

            return func();
        }

        /// <summary>
        /// Returns the branchname of the Assimp runtime.
        /// </summary>
        /// <returns>The current branch name.</returns>
        public string GetBranchName()
        {
            LoadIfNotLoaded();

            Functions.aiGetBranchName func = GetFunction<Functions.aiGetBranchName>(FunctionNames.aiGetBranchName);

            IntPtr ptr = func();

            if(ptr == IntPtr.Zero)
                return string.Empty;

            return Marshal.PtrToStringAnsi(ptr);
        }

        /// <summary>
        /// Gets the native Assimp DLL's current version number as "major.minor.revision" string. This is the
        /// version of Assimp that this wrapper is currently using.
        /// </summary>
        /// <returns>Unmanaged DLL version</returns>
        public string GetVersion()
        {
            uint major = GetVersionMajor();
            uint minor = GetVersionMinor();
            uint rev = GetVersionRevision();

            return $"{major}.{minor}.{rev}";
        }

        /// <summary>
        /// Gets the native Assimp DLL's current version number as a .NET version object.
        /// </summary>
        /// <returns>Unmanaged DLL version</returns>
        public Version GetVersionAsVersion()
        {
            return new Version((int) GetVersionMajor(), (int) GetVersionMinor(), 0, (int) GetVersionRevision());
        }

        /// <summary>
        /// Get the compilation flags that describe how the native Assimp DLL was compiled.
        /// </summary>
        /// <returns>Compilation flags</returns>
        public CompileFlags GetCompileFlags()
        {
            LoadIfNotLoaded();

            Functions.aiGetCompileFlags func = GetFunction<Functions.aiGetCompileFlags>(FunctionNames.aiGetCompileFlags);

            return (CompileFlags) func();
        }

        #endregion

        /// <summary>
        /// Gets an embedded texture.
        /// </summary>
        /// <param name="scene">Input asset.</param>
        /// <param name="filename">Texture path extracted from <see cref="GetMaterialString"/>.</param>
        /// <returns>An embedded texture, or nullptr.</returns>
        public IntPtr GetEmbeddedTexture(IntPtr scene, string filename)
        {
            LoadIfNotLoaded();

            if(scene == IntPtr.Zero)
                return IntPtr.Zero;

            Functions.aiGetEmbeddedTexture func = GetFunction<Functions.aiGetEmbeddedTexture>(FunctionNames.aiGetEmbeddedTexture);

            return func(scene, filename);
        }

        #region Function names 

        /// <summary>
        /// Defines all the unmanaged assimp C-function names.
        /// </summary>
        internal static class FunctionNames
        {

            #region Import Function Names

            public const string aiImportFile = "aiImportFile";
            public const string aiImportFileEx = "aiImportFileEx";
            public const string aiImportFileExWithProperties = "aiImportFileExWithProperties";
            public const string aiImportFileFromMemory = "aiImportFileFromMemory";
            public const string aiImportFileFromMemoryWithProperties = "aiImportFileFromMemoryWithProperties";
            public const string aiReleaseImport = "aiReleaseImport";
            public const string aiApplyPostProcessing = "aiApplyPostProcessing";

            #endregion

            #region Export Function Names

            public const string aiGetExportFormatCount = "aiGetExportFormatCount";
            public const string aiGetExportFormatDescription = "aiGetExportFormatDescription";
            public const string aiReleaseExportFormatDescription = "aiReleaseExportFormatDescription";
            public const string aiExportSceneToBlob = "aiExportSceneToBlob";
            public const string aiReleaseExportBlob = "aiReleaseExportBlob";
            public const string aiExportScene = "aiExportScene";
            public const string aiExportSceneEx = "aiExportSceneEx";
            public const string aiCopyScene = "aiCopyScene";

            #endregion

            #region Logging Function Names

            public const string aiAttachLogStream = "aiAttachLogStream";
            public const string aiEnableVerboseLogging = "aiEnableVerboseLogging";
            public const string aiDetachLogStream = "aiDetachLogStream";
            public const string aiDetachAllLogStreams = "aiDetachAllLogStreams";

            #endregion

            #region Import Properties Function Names

            public const string aiCreatePropertyStore = "aiCreatePropertyStore";
            public const string aiReleasePropertyStore = "aiReleasePropertyStore";
            public const string aiSetImportPropertyInteger = "aiSetImportPropertyInteger";
            public const string aiSetImportPropertyFloat = "aiSetImportPropertyFloat";
            public const string aiSetImportPropertyString = "aiSetImportPropertyString";
            public const string aiSetImportPropertyMatrix = "aiSetImportPropertyMatrix";

            #endregion

            #region Material Getters Function Names

            public const string aiGetMaterialColor = "aiGetMaterialColor";
            public const string aiGetMaterialFloatArray = "aiGetMaterialFloatArray";
            public const string aiGetMaterialIntegerArray = "aiGetMaterialIntegerArray";
            public const string aiGetMaterialProperty = "aiGetMaterialProperty";
            public const string aiGetMaterialString = "aiGetMaterialString";
            public const string aiGetMaterialTextureCount = "aiGetMaterialTextureCount";
            public const string aiGetMaterialTexture = "aiGetMaterialTexture";

            #endregion

            #region Error and Info Function Names

            public const string aiGetErrorString = "aiGetErrorString";
            public const string aiIsExtensionSupported = "aiIsExtensionSupported";
            public const string aiGetExtensionList = "aiGetExtensionList";
            public const string aiGetImportFormatCount = "aiGetImportFormatCount";
            public const string aiGetImportFormatDescription = "aiGetImportFormatDescription";
            public const string aiGetMemoryRequirements = "aiGetMemoryRequirements";

            #endregion

            #region Version Info Function Names

            public const string aiGetLegalString = "aiGetLegalString";
            public const string aiGetVersionMinor = "aiGetVersionMinor";
            public const string aiGetVersionMajor = "aiGetVersionMajor";
            public const string aiGetVersionRevision = "aiGetVersionRevision";
            public const string aiGetCompileFlags = "aiGetCompileFlags";
            public const string aiGetBranchName = "aiGetBranchName";

            #endregion

            public const string aiGetEmbeddedTexture = "aiGetEmbeddedTexture";
        }

        #endregion

        #region Function delegates

        /// <summary>
        /// Defines all of the delegates that represent the unmanaged assimp functions.
        /// </summary>
        internal static class Functions
        {

            #region Import Delegates

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiImportFile)]
            public delegate IntPtr aiImportFile([In, MarshalAs(UnmanagedType.LPUTF8Str)] string file, uint flags);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiImportFileEx)]
            public delegate IntPtr aiImportFileEx([In, MarshalAs(UnmanagedType.LPUTF8Str)] string file, uint flags, IntPtr fileIO);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiImportFileExWithProperties)]
            public delegate IntPtr aiImportFileExWithProperties([In, MarshalAs(UnmanagedType.LPUTF8Str)] string file, uint flag, IntPtr fileIO, IntPtr propStore);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiImportFileFromMemory)]
            public delegate IntPtr aiImportFileFromMemory(byte[] buffer, uint bufferLength, uint flags, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string formatHint);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiImportFileFromMemoryWithProperties)]
            public delegate IntPtr aiImportFileFromMemoryWithProperties(byte[] buffer, uint bufferLength, uint flags, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string formatHint, IntPtr propStore);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiReleaseImport)]
            public delegate void aiReleaseImport(IntPtr scene);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiApplyPostProcessing)]
            public delegate IntPtr aiApplyPostProcessing(IntPtr scene, uint Flags);

            #endregion

            #region Export Delegates

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetExportFormatCount)]
            public delegate UIntPtr aiGetExportFormatCount();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetExportFormatDescription)]
            public delegate IntPtr aiGetExportFormatDescription(UIntPtr index);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiReleaseExportFormatDescription)]
            public delegate void aiReleaseExportFormatDescription(IntPtr desc);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiExportSceneToBlob)]
            public delegate IntPtr aiExportSceneToBlob(IntPtr scene, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string formatId, uint preProcessing);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiReleaseExportBlob)]
            public delegate void aiReleaseExportBlob(IntPtr blobData);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiExportScene)]
            public delegate ReturnCode aiExportScene(IntPtr scene, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string formatId, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string fileName, uint preProcessing);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiExportSceneEx)]
            public delegate ReturnCode aiExportSceneEx(IntPtr scene, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string formatId, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string fileName, IntPtr fileIO, uint preProcessing);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiCopyScene)]
            public delegate void aiCopyScene(IntPtr sceneIn, out IntPtr sceneOut);

            #endregion

            #region Logging Delegates

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiAttachLogStream)]
            public delegate void aiAttachLogStream(IntPtr logStreamPtr);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiEnableVerboseLogging)]
            public delegate void aiEnableVerboseLogging([In, MarshalAs(UnmanagedType.Bool)] bool enable);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiDetachLogStream)]
            public delegate ReturnCode aiDetachLogStream(IntPtr logStreamPtr);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiDetachAllLogStreams)]
            public delegate void aiDetachAllLogStreams();

            #endregion

            #region Property Delegates

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiCreatePropertyStore)]
            public delegate IntPtr aiCreatePropertyStore();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiReleasePropertyStore)]
            public delegate void aiReleasePropertyStore(IntPtr propertyStore);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiSetImportPropertyInteger)]
            public delegate void aiSetImportPropertyInteger(IntPtr propertyStore, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string name, int value);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiSetImportPropertyFloat)]
            public delegate void aiSetImportPropertyFloat(IntPtr propertyStore, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string name, float value);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiSetImportPropertyString)]
            public delegate void aiSetImportPropertyString(IntPtr propertyStore, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string name, ref AiString value);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiSetImportPropertyMatrix)]
            public delegate void aiSetImportPropertyMatrix(IntPtr propertyStore, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string name, ref Matrix4x4 value);

            #endregion

            #region Material Delegates

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetMaterialColor)]
            public delegate ReturnCode aiGetMaterialColor(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string key, uint texType, uint texIndex, IntPtr colorOut);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetMaterialFloatArray)]
            public delegate ReturnCode aiGetMaterialFloatArray(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string key, uint texType, uint texIndex, IntPtr ptrOut, ref uint valueCount);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetMaterialIntegerArray)]
            public delegate ReturnCode aiGetMaterialIntegerArray(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string key, uint texType, uint texIndex, IntPtr ptrOut, ref uint valueCount);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetMaterialProperty)]
            public delegate ReturnCode aiGetMaterialProperty(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string key, uint texType, uint texIndex, out IntPtr propertyOut);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetMaterialString)]
            public delegate ReturnCode aiGetMaterialString(ref AiMaterial mat, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string key, uint texType, uint texIndex, out AiString str);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetMaterialTexture)]
            public delegate ReturnCode aiGetMaterialTexture(ref AiMaterial mat, TextureType type, uint index, out AiString path, out TextureMapping mapping, out uint uvIndex, out float blendFactor, out TextureOperation textureOp, [In, Out] TextureWrapMode[] wrapModes, out uint flags);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetMaterialTextureCount)]
            public delegate uint aiGetMaterialTextureCount(ref AiMaterial mat, TextureType type);

            #endregion

            #region Error and Info Delegates

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetErrorString)]
            public delegate IntPtr aiGetErrorString();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetExtensionList)]
            public delegate void aiGetExtensionList(ref AiString extensionsOut);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetMemoryRequirements)]
            public delegate void aiGetMemoryRequirements(IntPtr scene, ref AiMemoryInfo memoryInfo);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiIsExtensionSupported)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public delegate bool aiIsExtensionSupported([In, MarshalAs(UnmanagedType.LPUTF8Str)] string extension);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetImportFormatCount)]
            public delegate UIntPtr aiGetImportFormatCount();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetImportFormatDescription)]
            public delegate IntPtr aiGetImportFormatDescription(UIntPtr index);

            #endregion

            #region Version Info Delegates

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetLegalString)]
            public delegate IntPtr aiGetLegalString();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetVersionMinor)]
            public delegate uint aiGetVersionMinor();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetVersionMajor)]
            public delegate uint aiGetVersionMajor();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetVersionRevision)]
            public delegate uint aiGetVersionRevision();
            
            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetBranchName)]
            public delegate IntPtr aiGetBranchName();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetCompileFlags)]
            public delegate uint aiGetCompileFlags();

            #endregion

            [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedFunctionName(FunctionNames.aiGetEmbeddedTexture)]
            public delegate IntPtr aiGetEmbeddedTexture(IntPtr scene, [In, MarshalAs(UnmanagedType.LPUTF8Str)] string filename);
        }

        #endregion

        // Assimp's quaternions are WXYZ, C#'s are XYZW, we need to convert all of them.
        internal static Quaternion FixQuaternionFromAssimp(Quaternion quat) => new(quat.Y, quat.Z, quat.W, quat.X);
        internal static Quaternion FixQuaternionToAssimp(Quaternion quat) => new(quat.W, quat.X, quat.Y, quat.Z);
        internal static unsafe void FixQuaternionsInSceneFromAssimp(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return;

            var scene = (AiScene*)ptr;
            if (scene->NumAnimations == 0)
                return;

            for (uint i = 0; i < scene->NumAnimations; i++)
            {
                var anim = ((AiAnimation**)scene->Animations)[i];
                for (uint j = 0; j < anim->NumChannels; j++)
                {
                    var channel = ((AiNodeAnim**)anim->Channels)[j];
                    for (uint k = 0; k < channel->NumRotationKeys; k++)
                    {
                        ref var rotKey = ref ((QuaternionKey*)channel->RotationKeys)[k];
                        rotKey.Value = FixQuaternionFromAssimp(rotKey.Value);
                    }
                }
            }
        }
        internal static unsafe void FixQuaternionsInSceneToAssimp(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return;

            var scene = (AiScene*)ptr;
            if (scene->NumAnimations == 0)
                return;

            for (uint i = 0; i < scene->NumAnimations; i++)
            {
                var anim = ((AiAnimation**)scene->Animations)[i];
                for (uint j = 0; j < anim->NumChannels; j++)
                {
                    var channel = ((AiNodeAnim**)anim->Channels)[j];
                    for (uint k = 0; k < channel->NumRotationKeys; k++)
                    {
                        ref var rotKey = ref ((QuaternionKey*)channel->RotationKeys)[k];
                        rotKey.Value = FixQuaternionToAssimp(rotKey.Value);
                    }
                }
            }
        }
    }
}