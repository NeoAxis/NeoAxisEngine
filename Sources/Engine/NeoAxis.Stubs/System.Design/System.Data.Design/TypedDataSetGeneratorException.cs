using System.Collections;
using System.Runtime.Serialization;

namespace System.Data.Design
{
	[Serializable]
	public class TypedDataSetGeneratorException : DataException
	{
		public IList ErrorList
		{
			get
			{
				throw null;
			}
		}

		protected TypedDataSetGeneratorException(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}

		public TypedDataSetGeneratorException()
		{
			throw null;
		}

		public TypedDataSetGeneratorException(string message)
		{
			throw null;
		}

		public TypedDataSetGeneratorException(string message, Exception innerException)
		{
			throw null;
		}

		public TypedDataSetGeneratorException(IList list)
		{
			throw null;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}
	}
}
