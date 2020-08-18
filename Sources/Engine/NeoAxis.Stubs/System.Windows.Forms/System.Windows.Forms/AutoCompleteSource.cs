namespace System.Windows.Forms
{
	public enum AutoCompleteSource
	{
		FileSystem = 1,
		HistoryList = 2,
		RecentlyUsedList = 4,
		AllUrl = 6,
		AllSystemSources = 7,
		FileSystemDirectories = 0x20,
		CustomSource = 0x40,
		None = 0x80,
		ListItems = 0x100
	}
}
