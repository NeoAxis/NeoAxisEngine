namespace System.Windows.Forms.ComponentModel.Com2Interop
{
	public interface ICom2PropertyPageDisplayService
	{
		void ShowPropertyPage(string title, object component, int dispid, Guid pageGuid, IntPtr parentHandle);
	}
}
