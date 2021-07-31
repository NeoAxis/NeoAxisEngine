using System.Drawing.Text;

namespace System.Drawing
{
	public sealed class StringFormat : MarshalByRefObject, ICloneable, IDisposable
	{
		public StringFormatFlags FormatFlags
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

		public StringAlignment Alignment
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

		public StringAlignment LineAlignment
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

		public HotkeyPrefix HotkeyPrefix
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

		public StringTrimming Trimming
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

		public static StringFormat GenericDefault
		{
			get
			{
				throw null;
			}
		}

		public static StringFormat GenericTypographic
		{
			get
			{
				throw null;
			}
		}

		public StringDigitSubstitute DigitSubstitutionMethod
		{
			get
			{
				throw null;
			}
		}

		public int DigitSubstitutionLanguage
		{
			get
			{
				throw null;
			}
		}

		public StringFormat()
		{
			throw null;
		}

		public StringFormat(StringFormatFlags options)
		{
			throw null;
		}

		public StringFormat(StringFormatFlags options, int language)
		{
			throw null;
		}

		public StringFormat(StringFormat format)
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		public object Clone()
		{
			throw null;
		}

		public void SetMeasurableCharacterRanges(CharacterRange[] ranges)
		{
			throw null;
		}

		public void SetTabStops(float firstTabOffset, float[] tabStops)
		{
			throw null;
		}

		public float[] GetTabStops(out float firstTabOffset)
		{
			throw null;
		}

		public void SetDigitSubstitution(int language, StringDigitSubstitute substitute)
		{
			throw null;
		}

		~StringFormat()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
