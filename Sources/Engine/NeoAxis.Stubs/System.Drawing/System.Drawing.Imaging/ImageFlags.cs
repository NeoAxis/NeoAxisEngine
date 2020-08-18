namespace System.Drawing.Imaging
{
	public enum ImageFlags
	{
		None = 0,
		Scalable = 1,
		HasAlpha = 2,
		HasTranslucent = 4,
		PartiallyScalable = 8,
		ColorSpaceRgb = 0x10,
		ColorSpaceCmyk = 0x20,
		ColorSpaceGray = 0x40,
		ColorSpaceYcbcr = 0x80,
		ColorSpaceYcck = 0x100,
		HasRealDpi = 0x1000,
		HasRealPixelSize = 0x2000,
		ReadOnly = 0x10000,
		Caching = 0x20000
	}
}
