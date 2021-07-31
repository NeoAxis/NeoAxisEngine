using System.Runtime.Serialization;

namespace System.Drawing
{
	[Serializable]
	public sealed class Font : MarshalByRefObject, ICloneable, ISerializable, IDisposable
	{
		public FontFamily FontFamily
		{
			get
			{
				throw null;
			}
		}

		public bool Bold
		{
			get
			{
				throw null;
			}
		}

		public byte GdiCharSet
		{
			get
			{
				throw null;
			}
		}

		public bool GdiVerticalFont
		{
			get
			{
				throw null;
			}
		}

		public bool Italic
		{
			get
			{
				throw null;
			}
		}

		public string Name
		{
			get
			{
				throw null;
			}
		}

		public string OriginalFontName
		{
			get
			{
				throw null;
			}
		}

		public bool Strikeout
		{
			get
			{
				throw null;
			}
		}

		public bool Underline
		{
			get
			{
				throw null;
			}
		}

		public FontStyle Style
		{
			get
			{
				throw null;
			}
		}

		public float Size
		{
			get
			{
				throw null;
			}
		}

		public float SizeInPoints
		{
			get
			{
				throw null;
			}
		}

		public GraphicsUnit Unit
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

		public bool IsSystemFont
		{
			get
			{
				throw null;
			}
		}

		public string SystemFontName
		{
			get
			{
				throw null;
			}
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			throw null;
		}

		public Font(Font prototype, FontStyle newStyle)
		{
			throw null;
		}

		public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit)
		{
			throw null;
		}

		public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
		{
			throw null;
		}

		public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
		{
			throw null;
		}

		public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
		{
			throw null;
		}

		public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
		{
			throw null;
		}

		public Font(FontFamily family, float emSize, FontStyle style)
		{
			throw null;
		}

		public Font(FontFamily family, float emSize, GraphicsUnit unit)
		{
			throw null;
		}

		public Font(FontFamily family, float emSize)
		{
			throw null;
		}

		public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit)
		{
			throw null;
		}

		public Font(string familyName, float emSize, FontStyle style)
		{
			throw null;
		}

		public Font(string familyName, float emSize, GraphicsUnit unit)
		{
			throw null;
		}

		public Font(string familyName, float emSize)
		{
			throw null;
		}

		public static Font FromHfont(IntPtr hfont)
		{
			throw null;
		}

		public static Font FromLogFont(object lf)
		{
			throw null;
		}

		public static Font FromLogFont(object lf, IntPtr hdc)
		{
			throw null;
		}

		public static Font FromHdc(IntPtr hdc)
		{
			throw null;
		}

		public object Clone()
		{
			throw null;
		}

		~Font()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		public override bool Equals(object obj)
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

		public void ToLogFont(object logFont)
		{
			throw null;
		}

		public void ToLogFont(object logFont, Graphics graphics)
		{
			throw null;
		}

		public IntPtr ToHfont()
		{
			throw null;
		}

		public float GetHeight(Graphics graphics)
		{
			throw null;
		}

		public float GetHeight()
		{
			throw null;
		}

		public float GetHeight(float dpi)
		{
			throw null;
		}
	}
}
