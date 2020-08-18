using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 8)]
	public struct ImeModeConversion
	{
		public static Dictionary<ImeMode, ImeModeConversion> ImeModeConversionBits
		{
			get
			{
				throw null;
			}
		}

		public static bool IsCurrentConversionTableSupported
		{
			get
			{
				throw null;
			}
		}
	}
}
