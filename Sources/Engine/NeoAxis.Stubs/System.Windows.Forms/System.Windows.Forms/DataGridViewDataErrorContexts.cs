namespace System.Windows.Forms
{
	public enum DataGridViewDataErrorContexts
	{
		Formatting = 1,
		Display = 2,
		PreferredSize = 4,
		RowDeletion = 8,
		Parsing = 0x100,
		Commit = 0x200,
		InitialValueRestoration = 0x400,
		LeaveControl = 0x800,
		CurrentCellChange = 0x1000,
		Scroll = 0x2000,
		ClipboardContent = 0x4000
	}
}
