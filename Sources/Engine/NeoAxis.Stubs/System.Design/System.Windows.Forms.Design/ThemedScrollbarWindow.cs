using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design
{
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 8)]
	public struct ThemedScrollbarWindow
	{
		public IntPtr Handle;

		public ThemedScrollbarMode Mode;
	}
}
