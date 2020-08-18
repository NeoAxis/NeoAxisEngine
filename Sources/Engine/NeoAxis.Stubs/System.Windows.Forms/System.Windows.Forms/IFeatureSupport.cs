namespace System.Windows.Forms
{
	public interface IFeatureSupport
	{
		bool IsPresent(object feature);

		bool IsPresent(object feature, Version minimumVersion);

		Version GetVersionPresent(object feature);
	}
}
