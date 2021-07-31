namespace System.Windows.Forms
{
	public enum TreeViewHitTestLocations
	{
		None = 1,
		Image = 2,
		Label = 4,
		Indent = 8,
		AboveClientArea = 0x100,
		BelowClientArea = 0x200,
		LeftOfClientArea = 0x800,
		RightOfClientArea = 0x400,
		RightOfLabel = 0x20,
		StateImage = 0x40,
		PlusMinus = 0x10
	}
}
