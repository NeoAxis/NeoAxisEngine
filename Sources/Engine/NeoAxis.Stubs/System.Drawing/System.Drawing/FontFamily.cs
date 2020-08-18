using System.Drawing.Text;

namespace System.Drawing
{
	public sealed class FontFamily : MarshalByRefObject, IDisposable
	{
		public string Name
		{
			get
			{
				throw null;
			}
		}

		public static FontFamily[] Families
		{
			get
			{
				throw null;
			}
		}

		public static FontFamily GenericSansSerif
		{
			get
			{
				throw null;
			}
		}

		public static FontFamily GenericSerif
		{
			get
			{
				throw null;
			}
		}

		public static FontFamily GenericMonospace
		{
			get
			{
				throw null;
			}
		}

		public FontFamily(string name)
		{
			throw null;
		}

		public FontFamily(string name, FontCollection fontCollection)
		{
			throw null;
		}

		public FontFamily(GenericFontFamilies genericFamily)
		{
			throw null;
		}

		~FontFamily()
		{
			throw null;
		}

		public override bool Equals(object obj)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		public override int GetHashCode()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		public string GetName(int language)
		{
			throw null;
		}

		public static FontFamily[] GetFamilies(Graphics graphics)
		{
			throw null;
		}

		public bool IsStyleAvailable(FontStyle style)
		{
			throw null;
		}

		public int GetEmHeight(FontStyle style)
		{
			throw null;
		}

		public int GetCellAscent(FontStyle style)
		{
			throw null;
		}

		public int GetCellDescent(FontStyle style)
		{
			throw null;
		}

		public int GetLineSpacing(FontStyle style)
		{
			throw null;
		}
	}
}
