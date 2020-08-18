using System.Runtime.Serialization;

namespace System.Drawing.Printing
{
	[Serializable]
	public class InvalidPrinterException : SystemException
	{
		public InvalidPrinterException(PrinterSettings settings)
		{
			throw null;
		}

		protected InvalidPrinterException(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}
	}
}
