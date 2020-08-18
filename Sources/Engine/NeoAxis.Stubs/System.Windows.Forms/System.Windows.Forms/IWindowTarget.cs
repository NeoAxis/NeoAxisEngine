namespace System.Windows.Forms
{
	public interface IWindowTarget
	{
		void OnHandleChange(IntPtr newHandle);

		void OnMessage(ref Message m);
	}
}
