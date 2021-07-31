//using SharpBgfx;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Runtime.InteropServices;

//namespace SharpBgfx.Common {
//    public interface IUniformGroup {
//        void SubmitPerDrawUniforms ();
//    }

//    public class RenderStateGroup {
//        public RenderState State;
//        public uint BlendFactorRgba;
//        public StencilFlags FrontFace;
//        public StencilFlags BackFace;

//        public RenderStateGroup (RenderState state, uint blendFactor, StencilFlags frontFace, StencilFlags backFace) {
//            State = state;
//            BlendFactorRgba = blendFactor;
//            FrontFace = frontFace;
//            BackFace = backFace;
//        }
//    }

//    public class Mesh : IDisposable {
//        VertexLayout vertexDecl;
//        List<MeshGroup> groups;

//        public Mesh (MemoryBlock vertices, VertexLayout decl, ushort[] indices) {
//            var group = new MeshGroup();
//            group.VertexBuffer = new VertexBuffer(vertices, decl);
//            group.IndexBuffer = new IndexBuffer(MemoryBlock.FromArray(indices));

//            vertexDecl = decl;
//            groups = new List<MeshGroup> { group };
//        }

//        internal Mesh (VertexLayout decl, List<MeshGroup> groups) {
//            vertexDecl = decl;
//            this.groups = groups;
//        }

//        public unsafe void Submit (byte viewId, Program program, Matrix4x4* transform, RenderStateGroup renderStateGroup, IUniformGroup uniforms) {
//            Submit(viewId, program, transform, renderStateGroup, uniforms, null, default(Uniform));
//        }

//        public unsafe void Submit (byte viewId, Program program, Matrix4x4* transform, RenderStateGroup renderStateGroup, IUniformGroup uniforms, Texture texture, Uniform textureSampler) {
//            foreach (var group in groups) {
//                if (uniforms != null)
//                    uniforms.SubmitPerDrawUniforms();

//                if (texture != null)
//                    Bgfx.SetTexture(0, textureSampler, texture);

//                Bgfx.SetTransform((float*)transform);
//                Bgfx.SetIndexBuffer(group.IndexBuffer);
//                Bgfx.SetVertexBuffer(0, group.VertexBuffer);
//                Bgfx.SetRenderState(renderStateGroup.State, (int)renderStateGroup.BlendFactorRgba);
//                Bgfx.SetStencil(renderStateGroup.FrontFace, renderStateGroup.BackFace);
//                Bgfx.Submit(viewId, program);
//            }
//        }

//        public void Dispose () {
//            foreach (var group in groups) {
//                group.VertexBuffer.Dispose();
//                group.IndexBuffer.Dispose();
//            }

//            groups.Clear();
//        }
//    }

//    class MeshGroup {
//        public VertexBuffer VertexBuffer {
//            get;
//            set;
//        }

//        public IndexBuffer IndexBuffer {
//            get;
//            set;
//        }

//        public Collection<Primitive> Primitives {
//            get;
//            private set;
//        }

//        public MeshGroup () {
//            Primitives = new Collection<Primitive>();
//        }
//    }

//#pragma warning disable 649  // Field 'Primitive.StartIndex' is never assigned to, and will always have its default value 0
//    [StructLayout(LayoutKind.Sequential)]
//    struct Primitive {
//        public int StartIndex;
//        public int IndexCount;
//        public int StartVertex;
//        public int VertexCount;
//    }
//#pragma warning restore 649
//}
