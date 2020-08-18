namespace System.ComponentModel.Design
{
	public class DesignerActionService : IDisposable
	{
		public event DesignerActionListsChangedEventHandler DesignerActionListsChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public DesignerActionService(IServiceProvider serviceProvider)
		{
			throw null;
		}

		public void Add(IComponent comp, DesignerActionListCollection designerActionListCollection)
		{
			throw null;
		}

		public void Add(IComponent comp, DesignerActionList actionList)
		{
			throw null;
		}

		public void Clear()
		{
			throw null;
		}

		public bool Contains(IComponent comp)
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		protected virtual void Dispose(bool disposing)
		{
			throw null;
		}

		public DesignerActionListCollection GetComponentActions(IComponent component)
		{
			throw null;
		}

		public virtual DesignerActionListCollection GetComponentActions(IComponent component, ComponentActionsType type)
		{
			throw null;
		}

		protected virtual void GetComponentDesignerActions(IComponent component, DesignerActionListCollection actionLists)
		{
			throw null;
		}

		protected virtual void GetComponentServiceActions(IComponent component, DesignerActionListCollection actionLists)
		{
			throw null;
		}

		public void Remove(IComponent comp)
		{
			throw null;
		}

		public void Remove(DesignerActionList actionList)
		{
			throw null;
		}

		public void Remove(IComponent comp, DesignerActionList actionList)
		{
			throw null;
		}
	}
}
