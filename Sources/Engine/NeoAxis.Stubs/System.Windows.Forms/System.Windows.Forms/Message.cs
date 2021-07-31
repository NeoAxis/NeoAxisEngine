using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 8)]
	public struct Message
	{
		public IntPtr HWnd
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

		public int Msg
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

		public IntPtr WParam
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

		public IntPtr LParam
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

		public IntPtr Result
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

		public object GetLParam(Type cls)
		{
			throw null;
		}

		public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
		{
			throw null;
		}

		public override bool Equals(object o)
		{
			throw null;
		}

		public static bool operator !=(Message a, Message b)
		{
			throw null;
		}

		public static bool operator ==(Message a, Message b)
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
