using System.Collections;

namespace System.ComponentModel.Design.Data
{
	public abstract class DesignerDataTable : DesignerDataTableBase
	{
		public ICollection Relationships
		{
			get
			{
				throw null;
			}
		}

		protected DesignerDataTable(string name)
			:base(name)
		{
			throw null;
		}

		protected DesignerDataTable(string name, string owner)
			:base(name, owner)
		{
			throw null;
		}

		protected abstract ICollection CreateRelationships();
	}
}
