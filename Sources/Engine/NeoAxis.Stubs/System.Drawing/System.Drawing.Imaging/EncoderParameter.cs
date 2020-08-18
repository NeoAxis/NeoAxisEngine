using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class EncoderParameter : IDisposable
	{
		public Encoder Encoder
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

		public EncoderParameterValueType Type
		{
			get
			{
				throw null;
			}
		}

		public EncoderParameterValueType ValueType
		{
			get
			{
				throw null;
			}
		}

		public int NumberOfValues
		{
			get
			{
				throw null;
			}
		}

		~EncoderParameter()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, byte value)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, byte value, bool undefined)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, short value)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, long value)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, int numerator, int denominator)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, long rangebegin, long rangeend)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, int numerator1, int demoninator1, int numerator2, int demoninator2)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, string value)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, byte[] value)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, byte[] value, bool undefined)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, short[] value)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, long[] value)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, int[] numerator, int[] denominator)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, long[] rangebegin, long[] rangeend)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, int[] numerator1, int[] denominator1, int[] numerator2, int[] denominator2)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, int NumberOfValues, int Type, int Value)
		{
			throw null;
		}

		public EncoderParameter(Encoder encoder, int numberValues, EncoderParameterValueType type, IntPtr value)
		{
			throw null;
		}
	}
}
