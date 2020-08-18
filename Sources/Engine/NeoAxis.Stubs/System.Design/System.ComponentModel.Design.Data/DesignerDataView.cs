namespace System.ComponentModel.Design.Data
{
	public abstract class DesignerDataView : DesignerDataTableBase
	{
		protected DesignerDataView(string name)
			:base(name)
		{
			throw null;
		}

		protected DesignerDataView(string name, string owner)
			:base(name, owner)
		{
			throw null;
		}
	}
}
