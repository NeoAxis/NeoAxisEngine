namespace System.ComponentModel.Design
{
	public sealed class DesignerActionPropertyItem : DesignerActionItem
	{
		public string MemberName
		{
			get
			{
				throw null;
			}
		}

		public IComponent RelatedComponent
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public DesignerActionPropertyItem(string memberName, string displayName, string category, string description)
			: base( displayName, category, "" )
		{
			throw null;
		}

		public DesignerActionPropertyItem(string memberName, string displayName)
			: base( displayName,"", "" )
		{
			throw null;
		}

		public DesignerActionPropertyItem(string memberName, string displayName, string category)
			: base( displayName, category, "" )
		{
			throw null;
		}
	}
}
