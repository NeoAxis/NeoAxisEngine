using System.Windows.Forms;

namespace System.ComponentModel.Design.Data
{
	public abstract class DataSourceProviderService
	{
		public abstract bool SupportsAddNewDataSource
		{
			get;
		}

		public abstract bool SupportsConfigureDataSource
		{
			get;
		}

		public abstract DataSourceGroupCollection GetDataSources();

		public abstract DataSourceGroup InvokeAddNewDataSource(IWin32Window parentWindow, FormStartPosition startPosition);

		public abstract bool InvokeConfigureDataSource(IWin32Window parentWindow, FormStartPosition startPosition, DataSourceDescriptor dataSourceDescriptor);

		public abstract object AddDataSourceInstance(IDesignerHost host, DataSourceDescriptor dataSourceDescriptor);

		public abstract void NotifyDataSourceComponentAdded(object dsc);

		protected DataSourceProviderService()
		{
			throw null;
		}
	}
}
