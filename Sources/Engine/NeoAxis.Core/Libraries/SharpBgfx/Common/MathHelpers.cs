//using System;

//namespace Internal.SharpBgfx.Common {
//    static class MathHelpers {
//        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T> {
//            if (value.CompareTo(min) < 0)
//                return min;

//            if (value.CompareTo(max) > 0)
//                return max;

//            return value;
//        }

//        public static byte Lerp (byte start, byte end, float amount) {
//            return (byte)(start + (byte)(amount * (end - start)));
//        }
//    }
//}
