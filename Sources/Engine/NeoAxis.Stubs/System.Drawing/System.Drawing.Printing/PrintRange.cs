namespace System.Drawing.Printing
{
	[Serializable]
	public enum PrintRange
	{
		AllPages = 0,
		SomePages = 2,
		Selection = 1,
		CurrentPage = 0x400000
	}
}
