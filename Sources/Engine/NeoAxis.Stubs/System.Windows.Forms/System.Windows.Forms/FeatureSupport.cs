namespace System.Windows.Forms
{
	public abstract class FeatureSupport : IFeatureSupport
	{
		public static bool IsPresent(string featureClassName, string featureConstName)
		{
			throw null;
		}

		public static bool IsPresent(string featureClassName, string featureConstName, Version minimumVersion)
		{
			throw null;
		}

		public static Version GetVersionPresent(string featureClassName, string featureConstName)
		{
			throw null;
		}

		public virtual bool IsPresent(object feature)
		{
			throw null;
		}

		public virtual bool IsPresent(object feature, Version minimumVersion)
		{
			throw null;
		}

		public abstract Version GetVersionPresent(object feature);

		protected FeatureSupport()
		{
			throw null;
		}
	}
}
