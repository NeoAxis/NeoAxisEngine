using System.Drawing;
using System.IO;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
	[Serializable]
	public sealed class Cursor : IDisposable, ISerializable
	{
		public static Rectangle Clip
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

		public static Cursor Current
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

		public IntPtr Handle
		{
			get
			{
				throw null;
			}
		}

		public Point HotSpot
		{
			get
			{
				throw null;
			}
		}

		public static Point Position
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

		public Size Size
		{
			get
			{
				throw null;
			}
		}

		public object Tag
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

		public Cursor(IntPtr handle)
		{
			throw null;
		}

		public Cursor(string fileName)
		{
			throw null;
		}

		public Cursor(Type type, string resource)
		{
			throw null;
		}

		public Cursor(Stream stream)
		{
			throw null;
		}

		public IntPtr CopyHandle()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		public void Draw(Graphics g, Rectangle targetRect)
		{
			throw null;
		}

		public void DrawStretched(Graphics g, Rectangle targetRect)
		{
			throw null;
		}

		~Cursor()
		{
			throw null;
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			throw null;
		}

		public static void Hide()
		{
			throw null;
		}

		public static void Show()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		public static bool operator ==(Cursor left, Cursor right)
		{
			throw null;
		}

		public static bool operator !=(Cursor left, Cursor right)
		{
			throw null;
		}

		public override int GetHashCode()
		{
			throw null;
		}

		public override bool Equals(object obj)
		{
			throw null;
		}
	}
}
