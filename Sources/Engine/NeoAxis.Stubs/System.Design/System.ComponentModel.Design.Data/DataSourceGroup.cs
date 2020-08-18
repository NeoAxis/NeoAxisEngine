using System.Drawing;

namespace System.ComponentModel.Design.Data
{
	public abstract class DataSourceGroup
	{
		public abstract string Name
		{
			get;
		}

		public abstract Bitmap Image
		{
			get;
		}

		public abstract DataSourceDescriptorCollection DataSources
		{
			get;
		}

		public abstract bool IsDefault
		{
			get;
		}

		protected DataSourceGroup()
		{
			throw null;
		}
	}
}
