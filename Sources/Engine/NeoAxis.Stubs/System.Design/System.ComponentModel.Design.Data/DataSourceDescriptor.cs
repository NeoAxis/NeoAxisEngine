using System.Drawing;

namespace System.ComponentModel.Design.Data
{
	public abstract class DataSourceDescriptor
	{
		public abstract string Name
		{
			get;
		}

		public abstract Bitmap Image
		{
			get;
		}

		public abstract string TypeName
		{
			get;
		}

		public abstract bool IsDesignable
		{
			get;
		}

		protected DataSourceDescriptor()
		{
			throw null;
		}
	}
}
