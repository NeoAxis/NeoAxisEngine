namespace System.Windows.Forms
{
	public class ListControlConvertEventArgs : ConvertEventArgs
	{
		public object ListItem
		{
			get
			{
				throw null;
			}
		}

		public ListControlConvertEventArgs(object value, Type desiredType, object listItem)
			:base(value, desiredType)
		{
			throw null;
		}
	}
}
