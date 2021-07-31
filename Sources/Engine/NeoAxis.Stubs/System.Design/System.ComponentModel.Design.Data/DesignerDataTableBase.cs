using System.Collections;

namespace System.ComponentModel.Design.Data
{
	public abstract class DesignerDataTableBase
	{
		public ICollection Columns
		{
			get
			{
				throw null;
			}
		}

		public string Name
		{
			get
			{
				throw null;
			}
		}

		public string Owner
		{
			get
			{
				throw null;
			}
		}

		protected DesignerDataTableBase(string name)
		{
			throw null;
		}

		protected DesignerDataTableBase(string name, string owner)
		{
			throw null;
		}

		protected abstract ICollection CreateColumns();
	}
}
