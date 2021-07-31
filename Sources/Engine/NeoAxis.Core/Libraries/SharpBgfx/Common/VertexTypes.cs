using SharpBgfx;

namespace SharpBgfx.Common {
    public struct PosColorVertex {
        float x;
        float y;
        float z;
        uint abgr;

        public PosColorVertex (float x, float y, float z, uint abgr) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.abgr = abgr;
        }

        public static readonly VertexLayout Layout = new VertexLayout().Begin()
            .Add(VertexAttributeUsage.Position, 3, VertexAttributeType.Float)
            .Add(VertexAttributeUsage.Color0, 4, VertexAttributeType.UInt8, true)
            .End();
    }

    public struct PosNormalTexcoordVertex {
        float x;
        float y;
        float z;
        uint normal;
        float u;
        float v;

        public PosNormalTexcoordVertex (float x, float y, float z, uint normal, float u, float v) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.normal = normal;
            this.u = u;
            this.v = v;
        }

        public static readonly VertexLayout Layout = new VertexLayout().Begin()
            .Add(VertexAttributeUsage.Position, 3, VertexAttributeType.Float)
            .Add(VertexAttributeUsage.Normal, 4, VertexAttributeType.UInt8, true, true)
            .Add(VertexAttributeUsage.TexCoord0, 2, VertexAttributeType.Float)
            .End();
    }
}
