namespace System.ComponentModel.Design
{
	public class DesignerActionMethodItem : DesignerActionItem
	{
		public virtual string MemberName
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

		public virtual bool IncludeAsDesignerVerb
		{
			get
			{
				throw null;
			}
		}

		public DesignerActionMethodItem(DesignerActionList actionList, string memberName, string displayName, string category, string description, bool includeAsDesignerVerb)
			: base( displayName, category, "" )
		{
			throw null;
		}

		public DesignerActionMethodItem(DesignerActionList actionList, string memberName, string displayName)
			: base( displayName, "", "" )
		{
			throw null;
		}

		public DesignerActionMethodItem(DesignerActionList actionList, string memberName, string displayName, bool includeAsDesignerVerb)
			: base( displayName, "", "" )
		{
			throw null;
		}

		public DesignerActionMethodItem(DesignerActionList actionList, string memberName, string displayName, string category)
			: base( displayName, category, "" )
		{
			throw null;
		}

		public DesignerActionMethodItem(DesignerActionList actionList, string memberName, string displayName, string category, bool includeAsDesignerVerb)
			: base( displayName, category, "" )
		{
			throw null;
		}

		public DesignerActionMethodItem(DesignerActionList actionList, string memberName, string displayName, string category, string description)
			: base( displayName, category, "" )
		{
			throw null;
		}

		public virtual void Invoke()
		{
			throw null;
		}
	}
}
