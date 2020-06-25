//using SharpBgfx;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;

//namespace SharpBgfx.Common {
//    public static class ResourceLoader {
//        static readonly string RootPath = "../Assets/";

//        static string GetShaderPath() {
//            var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

//            switch (Bgfx.GetCurrentBackend()) {
//                case RendererBackend.Direct3D11:
//                case RendererBackend.Direct3D12:
//                    return Path.Combine(basePath, "bin", "dx11");

//                case RendererBackend.OpenGL:
//                    return Path.Combine(basePath, "bin", "glsl");

//                case RendererBackend.OpenGLES:
//                    return Path.Combine(RootPath, "Shaders/bin/gles/");

//                case RendererBackend.Direct3D9:
//                    return Path.Combine(RootPath, "Shaders/bin/dx9/");

//                default:
//                    throw new InvalidOperationException("Unknown renderer backend type.");
//            }
//        }

//        public static Shader LoadShader(string name) {
//            var path = Path.Combine(GetShaderPath(), name) + ".bin";
//            var mem = MemoryBlock.FromArray(File.ReadAllBytes(path));
//            return new Shader(mem);
//        }

//        public static Program LoadProgram(string vsName, string fsName) {
//            var vsh = LoadShader(vsName);
//            var fsh = LoadShader(fsName);

//            return new Program(vsh, fsh, true);
//        }

//        public static Program LoadProgram(string csName) {
//            var csh = LoadShader(csName);
//            return new Program(csh, true);
//        }

//        public static Texture LoadTexture(string name) {
//            var path = Path.Combine(RootPath, "textures/", name);
//            var mem = MemoryBlock.FromArray(File.ReadAllBytes(path));
//            return Texture.FromFile(mem, TextureFlags.None, 0);
//        }

//        public unsafe static Mesh LoadMesh(string fileName) {
//            var path = Path.Combine(RootPath, "meshes/", fileName);
//            var groups = new List<MeshGroup>();
//            var group = new MeshGroup();
//            VertexLayout layout = null;

//            using (var reader = new MemoryReader(File.ReadAllBytes(path))) {
//                while (!reader.Done) {
//                    var tag = reader.Read<uint>();
//                    if (tag == ChunkTagVB) {
//                        // skip bounding volume info
//                        reader.Skip(BoundingVolumeSize);

//                        layout = ReadVertexLayout(reader);

//                        var vertexCount = reader.Read<ushort>();
//                        var vertexData = reader.ReadArray<byte>(vertexCount * layout.Stride);
//                        group.VertexBuffer = new VertexBuffer(MemoryBlock.FromArray(vertexData), layout);
//                    }
//                    else if (tag == ChunkTagIB) {
//                        var indexCount = reader.Read<uint>();
//                        var indexData = reader.ReadArray<ushort>((int)indexCount);

//                        group.IndexBuffer = new IndexBuffer(MemoryBlock.FromArray(indexData));
//                    }
//                    else if (tag == ChunkTagPri) {
//                        // skip material name
//                        var len = reader.Read<ushort>();
//                        reader.Skip(len);

//                        // read primitive data
//                        var count = reader.Read<ushort>();
//                        for (int i = 0; i < count; i++) {
//                            // skip name
//                            len = reader.Read<ushort>();
//                            reader.Skip(len);

//                            var prim = reader.Read<Primitive>();
//                            group.Primitives.Add(prim);

//                            // skip bounding volumes
//                            reader.Skip(BoundingVolumeSize);
//                        }

//                        groups.Add(group);
//                        group = new MeshGroup();
//                    }
//                }
//            }
//            return new Mesh(layout, groups);
//        }

//        static VertexLayout ReadVertexLayout(MemoryReader reader) {
//            var layout = new VertexLayout();
//            layout.Begin();

//            var attributeCount = reader.Read<byte>();
//            var stride = reader.Read<ushort>();

//            for (int i = 0; i < attributeCount; i++) {
//                var e = reader.Read<VertexElement>();
//                var usage = attributeUsageMap[e.Attrib];
//                layout.Add(usage, e.Count, attributeTypeMap[e.AttribType], e.Normalized != 0, e.AsInt != 0);

//                if (layout.GetOffset(usage) != e.Offset)
//                    throw new InvalidOperationException("Invalid mesh data; vertex attribute offset mismatch.");
//            }

//            layout.End();
//            if (layout.Stride != stride)
//                throw new InvalidOperationException("Invalid mesh data; vertex layout stride mismatch.");

//            return layout;
//        }

//        static uint MakeFourCC(char a, char b, char c, char d) {
//            return a | ((uint)b << 8) | ((uint)c << 16) | ((uint)d << 24);
//        }

//        const int BoundingVolumeSize = 26 * sizeof(float);
//        static readonly uint ChunkTagVB = MakeFourCC('V', 'B', ' ', '\x1');
//        static readonly uint ChunkTagIB = MakeFourCC('I', 'B', ' ', '\0');
//        static readonly uint ChunkTagPri = MakeFourCC('P', 'R', 'I', '\0');

//        [StructLayout(LayoutKind.Sequential, Pack = 1)]
//        struct VertexElement {
//            public ushort Offset;
//            public ushort Attrib;
//            public byte Count;
//            public ushort AttribType;
//            public byte Normalized;
//            public byte AsInt;
//        }

//        static readonly Dictionary<ushort, VertexAttributeType> attributeTypeMap = new Dictionary<ushort, VertexAttributeType> {
//            { 0x1, VertexAttributeType.UInt8 },
//            { 0x2, VertexAttributeType.Int16 },
//            { 0x3, VertexAttributeType.Half },
//            { 0x4, VertexAttributeType.Float }
//        };

//        static readonly Dictionary<ushort, VertexAttributeUsage> attributeUsageMap = new Dictionary<ushort, VertexAttributeUsage> {
//            { 0x1,  VertexAttributeUsage.Position },
//            { 0x2,  VertexAttributeUsage.Normal },
//            { 0x3,  VertexAttributeUsage.Tangent },
//            { 0x4,  VertexAttributeUsage.Bitangent },
//            { 0x5,  VertexAttributeUsage.Color0 },
//            { 0x6,  VertexAttributeUsage.Color1 },
//            { 0x18, VertexAttributeUsage.Color2 },
//            { 0x19, VertexAttributeUsage.Color3 },
//            { 0xe,  VertexAttributeUsage.Indices },
//            { 0xf,  VertexAttributeUsage.Weight },
//            { 0x10, VertexAttributeUsage.TexCoord0 },
//            { 0x11, VertexAttributeUsage.TexCoord1 },
//            { 0x12, VertexAttributeUsage.TexCoord2 },
//            { 0x13, VertexAttributeUsage.TexCoord3 },
//            { 0x14, VertexAttributeUsage.TexCoord4 },
//            { 0x15, VertexAttributeUsage.TexCoord5 },
//            { 0x16, VertexAttributeUsage.TexCoord6 },
//            { 0x17, VertexAttributeUsage.TexCoord7 }
//        };
//    }

//    unsafe class MemoryReader : IDisposable {
//        GCHandle handle;
//        byte* ptr;
//        byte* end;

//        public bool Done {
//            get { return ptr >= end; }
//        }

//        public MemoryReader(byte[] memory) {
//            handle = GCHandle.Alloc(memory, GCHandleType.Pinned);
//            ptr = (byte*)handle.AddrOfPinnedObject();
//            end = ptr + memory.Length;
//        }

//        public T Read<T>() {
//            T result = Unsafe.Read<T>(ptr);
//            ptr += Unsafe.SizeOf<T>();
//            return result;
//        }

//        public T[] ReadArray<T>(int count) {
//            var result = new T[count];
//            var asBytes = Unsafe.As<byte[]>(result);

//            int byteCount = count * Unsafe.SizeOf<T>();
//            fixed (void* dest = asBytes)
//                Unsafe.CopyBlock(dest, ptr, (uint)byteCount);

//            ptr += byteCount;
//            return result;
//        }

//        public void Skip(int bytes) {
//            ptr += bytes;
//        }

//        public void Dispose() {
//            handle.Free();
//        }
//    }
//}
