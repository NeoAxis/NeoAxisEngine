using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 8)]
	public struct LinkArea
	{
		public class LinkAreaConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				throw null;
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				throw null;
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				throw null;
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				throw null;
			}

			public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
			{
				throw null;
			}

			public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
			{
				throw null;
			}

			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				throw null;
			}

			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				throw null;
			}

			public LinkAreaConverter()
			{
				throw null;
			}
		}

		public int Start
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

		public int Length
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

		public bool IsEmpty
		{
			get
			{
				throw null;
			}
		}

		public LinkArea(int start, int length)
		{
			throw null;
		}

		public override bool Equals(object o)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		public static bool operator ==(LinkArea linkArea1, LinkArea linkArea2)
		{
			throw null;
		}

		public static bool operator !=(LinkArea linkArea1, LinkArea linkArea2)
		{
			throw null;
		}

		public override int GetHashCode()
		{
			throw null;
		}
	}
}
