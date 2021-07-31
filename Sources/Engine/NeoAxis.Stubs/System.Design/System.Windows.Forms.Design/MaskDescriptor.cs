using System.Globalization;

namespace System.Windows.Forms.Design
{
	public abstract class MaskDescriptor
	{
		public abstract string Mask
		{
			get;
		}

		public abstract string Name
		{
			get;
		}

		public abstract string Sample
		{
			get;
		}

		public abstract Type ValidatingType
		{
			get;
		}

		public virtual CultureInfo Culture
		{
			get
			{
				throw null;
			}
		}

		public static bool IsValidMaskDescriptor(MaskDescriptor maskDescriptor)
		{
			throw null;
		}

		public static bool IsValidMaskDescriptor(MaskDescriptor maskDescriptor, out string validationErrorDescription)
		{
			throw null;
		}

		public override bool Equals(object maskDescriptor)
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

		protected MaskDescriptor()
		{
			throw null;
		}
	}
}
