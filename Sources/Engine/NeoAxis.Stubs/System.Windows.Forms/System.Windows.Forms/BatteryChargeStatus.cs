namespace System.Windows.Forms
{
	public enum BatteryChargeStatus
	{
		High = 1,
		Low = 2,
		Critical = 4,
		Charging = 8,
		NoSystemBattery = 0x80,
		Unknown = 0xFF
	}
}
