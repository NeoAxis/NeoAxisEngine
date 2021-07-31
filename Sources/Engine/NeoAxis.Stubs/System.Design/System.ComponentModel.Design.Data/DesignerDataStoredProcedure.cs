using System.Collections;

namespace System.ComponentModel.Design.Data
{
	public abstract class DesignerDataStoredProcedure
	{
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

		public ICollection Parameters
		{
			get
			{
				throw null;
			}
		}

		protected DesignerDataStoredProcedure(string name)
		{
			throw null;
		}

		protected DesignerDataStoredProcedure(string name, string owner)
		{
			throw null;
		}

		protected abstract ICollection CreateParameters();
	}
}
