namespace System.Windows.Forms
{
	public class TypeValidationEventArgs : EventArgs
	{
		public bool Cancel
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

		public bool IsValidInput
		{
			get
			{
				throw null;
			}
		}

		public string Message
		{
			get
			{
				throw null;
			}
		}

		public object ReturnValue
		{
			get
			{
				throw null;
			}
		}

		public Type ValidatingType
		{
			get
			{
				throw null;
			}
		}

		public TypeValidationEventArgs(Type validatingType, bool isValidInput, object returnValue, string message)
		{
			throw null;
		}
	}
}
