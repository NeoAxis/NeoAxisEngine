using System.Collections;
using System.Runtime.Serialization;

namespace System.ComponentModel.Design
{
	[Serializable]
	public sealed class ExceptionCollection : Exception
	{
		public ArrayList Exceptions
		{
			get
			{
				throw null;
			}
		}

		public ExceptionCollection(ArrayList exceptions)
		{
			throw null;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}
	}
}
