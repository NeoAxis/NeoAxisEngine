namespace System.Windows.Forms
{
	public enum DataGridViewCellStyleScopes
	{
		None = 0,
		Cell = 1,
		Column = 2,
		Row = 4,
		DataGridView = 8,
		ColumnHeaders = 0x10,
		RowHeaders = 0x20,
		Rows = 0x40,
		AlternatingRows = 0x80
	}
}
