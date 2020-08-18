namespace System.Windows.Forms.Design
{
	public enum SelectionRules
	{
		None = 0,
		Moveable = 0x10000000,
		Visible = 0x40000000,
		Locked = int.MinValue,
		TopSizeable = 1,
		BottomSizeable = 2,
		LeftSizeable = 4,
		RightSizeable = 8,
		AllSizeable = 0xF
	}
}
