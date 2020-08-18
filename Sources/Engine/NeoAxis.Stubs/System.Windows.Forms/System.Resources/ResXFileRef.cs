using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace System.Resources
{
	[Serializable]
	public class ResXFileRef
	{
		public class Converter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				throw null;
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				throw null;
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				throw null;
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				throw null;
			}

			public Converter()
			{
				throw null;
			}
		}

		public string FileName
		{
			get
			{
				throw null;
			}
		}

		public string TypeName
		{
			get
			{
				throw null;
			}
		}

		public Encoding TextFileEncoding
		{
			get
			{
				throw null;
			}
		}

		public ResXFileRef(string fileName, string typeName)
		{
			throw null;
		}

		public ResXFileRef(string fileName, string typeName, Encoding textFileEncoding)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
