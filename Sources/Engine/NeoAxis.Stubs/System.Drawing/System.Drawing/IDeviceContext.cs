namespace System.Drawing
{
	public interface IDeviceContext : IDisposable
	{
		IntPtr GetHdc();

		void ReleaseHdc();
	}
}
