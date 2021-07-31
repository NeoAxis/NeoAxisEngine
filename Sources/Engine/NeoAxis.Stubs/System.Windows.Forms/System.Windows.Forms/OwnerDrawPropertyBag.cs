using System.Drawing;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
	[Serializable]
	public class OwnerDrawPropertyBag : MarshalByRefObject, ISerializable
	{
		public Font Font
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

		public Color ForeColor
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

		public Color BackColor
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

		protected OwnerDrawPropertyBag(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}

		public virtual bool IsEmpty()
		{
			throw null;
		}

		public static OwnerDrawPropertyBag Copy(OwnerDrawPropertyBag value)
		{
			throw null;
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			throw null;
		}
	}
}
