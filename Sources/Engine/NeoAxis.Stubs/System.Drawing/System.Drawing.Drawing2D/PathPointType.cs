namespace System.Drawing.Drawing2D
{
	public enum PathPointType
	{
		Start = 0,
		Line = 1,
		Bezier = 3,
		PathTypeMask = 7,
		DashMode = 0x10,
		PathMarker = 0x20,
		CloseSubpath = 0x80,
		Bezier3 = 3
	}
}
