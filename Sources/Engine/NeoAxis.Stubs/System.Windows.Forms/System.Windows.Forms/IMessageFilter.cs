namespace System.Windows.Forms
{
	public interface IMessageFilter
	{
		bool PreFilterMessage(ref Message m);
	}
}
