using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 8)]
	public struct Padding
	{
		public static readonly Padding Empty;

		public int All
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int Bottom
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int Left
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int Right
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int Top
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int Horizontal
		{
			get
			{
				throw null;
			}
		}

		public int Vertical
		{
			get
			{
				throw null;
			}
		}

		public Size Size
		{
			get
			{
				throw null;
			}
		}

		public Padding(int all)
		{
			throw null;
		}

		public Padding(int left, int top, int right, int bottom)
		{
			throw null;
		}

		public static Padding Add(Padding p1, Padding p2)
		{
			throw null;
		}

		public static Padding Subtract(Padding p1, Padding p2)
		{
			throw null;
		}

		public override bool Equals(object other)
		{
			throw null;
		}

		public static Padding operator +(Padding p1, Padding p2)
		{
			throw null;
		}

		public static Padding operator -(Padding p1, Padding p2)
		{
			throw null;
		}

		public static bool operator ==(Padding p1, Padding p2)
		{
			throw null;
		}

		public static bool operator !=(Padding p1, Padding p2)
		{
			throw null;
		}

		public override int GetHashCode()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
