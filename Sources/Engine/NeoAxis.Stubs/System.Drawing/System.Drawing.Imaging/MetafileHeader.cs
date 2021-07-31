using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class MetafileHeader
	{
		public MetafileType Type
		{
			get
			{
				throw null;
			}
		}

		public int MetafileSize
		{
			get
			{
				throw null;
			}
		}

		public int Version
		{
			get
			{
				throw null;
			}
		}

		public float DpiX
		{
			get
			{
				throw null;
			}
		}

		public float DpiY
		{
			get
			{
				throw null;
			}
		}

		public Rectangle Bounds
		{
			get
			{
				throw null;
			}
		}

		public MetaHeader WmfHeader
		{
			get
			{
				throw null;
			}
		}

		public int EmfPlusHeaderSize
		{
			get
			{
				throw null;
			}
		}

		public int LogicalDpiX
		{
			get
			{
				throw null;
			}
		}

		public int LogicalDpiY
		{
			get
			{
				throw null;
			}
		}

		public bool IsWmf()
		{
			throw null;
		}

		public bool IsWmfPlaceable()
		{
			throw null;
		}

		public bool IsEmf()
		{
			throw null;
		}

		public bool IsEmfOrEmfPlus()
		{
			throw null;
		}

		public bool IsEmfPlus()
		{
			throw null;
		}

		public bool IsEmfPlusDual()
		{
			throw null;
		}

		public bool IsEmfPlusOnly()
		{
			throw null;
		}

		public bool IsDisplay()
		{
			throw null;
		}
	}
}
