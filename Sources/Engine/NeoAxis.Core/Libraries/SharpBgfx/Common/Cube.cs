using SharpBgfx;

namespace SharpBgfx.Common {
    public static class Cube {
        public static VertexBuffer CreateVertexBuffer () {
            return new VertexBuffer(MemoryBlock.FromArray(vertices), PosColorVertex.Layout);
        }

        public static IndexBuffer CreateIndexBuffer () {
            return new IndexBuffer(MemoryBlock.FromArray(indices));
        }

        static readonly PosColorVertex[] vertices = {
            new PosColorVertex(-1.0f,  1.0f,  1.0f, 0xff000000),
            new PosColorVertex( 1.0f,  1.0f,  1.0f, 0xff0000ff),
            new PosColorVertex(-1.0f, -1.0f,  1.0f, 0xff00ff00),
            new PosColorVertex( 1.0f, -1.0f,  1.0f, 0xff00ffff),
            new PosColorVertex(-1.0f,  1.0f, -1.0f, 0xffff0000),
            new PosColorVertex( 1.0f,  1.0f, -1.0f, 0xffff00ff),
            new PosColorVertex(-1.0f, -1.0f, -1.0f, 0xffffff00),
            new PosColorVertex( 1.0f, -1.0f, -1.0f, 0xffffffff)
        };

        static readonly ushort[] indices = {
            0, 1, 2, // 0
            1, 3, 2,
            4, 6, 5, // 2
            5, 6, 7,
            0, 2, 4, // 4
            4, 2, 6,
            1, 5, 3, // 6
            5, 7, 3,
            0, 4, 1, // 8
            4, 5, 1,
            2, 3, 6, // 10
            6, 3, 7
        };
    }
}
