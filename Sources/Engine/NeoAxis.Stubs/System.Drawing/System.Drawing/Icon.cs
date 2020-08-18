using System.IO;
using System.Runtime.Serialization;

namespace System.Drawing
{
	[Serializable]
	public sealed class Icon : MarshalByRefObject, ISerializable, ICloneable, IDisposable
	{
		public IntPtr Handle
		{
			get
			{
				throw null;
			}
		}

		public int Height
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

		public int Width
		{
			get
			{
				throw null;
			}
		}

		public Icon(string fileName)
		{
			throw null;
		}

		public Icon(string fileName, Size size)
		{
			throw null;
		}

		public Icon(string fileName, int width, int height)
		{
			throw null;
		}

		public Icon(Icon original, Size size)
		{
			throw null;
		}

		public Icon(Icon original, int width, int height)
		{
			throw null;
		}

		public Icon(Type type, string resource)
		{
			throw null;
		}

		public Icon(Stream stream)
		{
			throw null;
		}

		public Icon(Stream stream, Size size)
		{
			throw null;
		}

		public Icon(Stream stream, int width, int height)
		{
			throw null;
		}

		public static Icon ExtractAssociatedIcon(string filePath)
		{
			throw null;
		}

		public object Clone()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		~Icon()
		{
			throw null;
		}

		public static Icon FromHandle(IntPtr handle)
		{
			throw null;
		}

		public void Save(Stream outputStream)
		{
			throw null;
		}

		public Bitmap ToBitmap()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			throw null;
		}
	}
}
