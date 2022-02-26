using System.Runtime.InteropServices;

namespace Internal.BulletSharp
{
    public static class Native
    {
        public const string Dll = "libbulletc";
        public const CallingConvention Conv = CallingConvention.Cdecl;
    }
}
