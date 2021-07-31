namespace SharpBgfx.Common {
    // not a robust implementation; just for use with samples
    public struct Vector4 {
        public static readonly Vector4 Zero = new Vector4();
        public static readonly Vector4 UnitX = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
        public static readonly Vector4 UnitY = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
        public static readonly Vector4 UnitZ = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);

        public float X, Y, Z, W;

        public Vector4 (float value) {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }

        public Vector4 (Vector3 v3, float w) {
            X = v3.X;
            Y = v3.Y;
            Z = v3.Z;
            W = w;
        }

        public Vector4 (float x, float y, float z, float w) {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}
