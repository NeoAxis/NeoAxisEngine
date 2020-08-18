using System.Runtime.InteropServices;

namespace Accessibility
{
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 8)]
	public struct _RemotableHandle
	{
		public int fContext;

		public __MIDL_IWinTypes_0009 u;
	}
}
