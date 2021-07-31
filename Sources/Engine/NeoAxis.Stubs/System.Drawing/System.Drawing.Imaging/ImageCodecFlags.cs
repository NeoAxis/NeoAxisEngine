namespace System.Drawing.Imaging
{
	public enum ImageCodecFlags
	{
		Encoder = 1,
		Decoder = 2,
		SupportBitmap = 4,
		SupportVector = 8,
		SeekableEncode = 0x10,
		BlockingDecode = 0x20,
		Builtin = 0x10000,
		System = 0x20000,
		User = 0x40000
	}
}
