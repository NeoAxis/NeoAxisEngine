namespace System.Windows.Forms
{
	public enum ListViewHitTestLocations
	{
		None = 1,
		AboveClientArea = 0x100,
		BelowClientArea = 0x10,
		LeftOfClientArea = 0x40,
		RightOfClientArea = 0x20,
		Image = 2,
		StateImage = 0x200,
		Label = 4
	}
}
