namespace System.Windows.Forms
{
	public enum CloseReason
	{
		None,
		WindowsShutDown,
		MdiFormClosing,
		UserClosing,
		TaskManagerClosing,
		FormOwnerClosing,
		ApplicationExitCall
	}
}
