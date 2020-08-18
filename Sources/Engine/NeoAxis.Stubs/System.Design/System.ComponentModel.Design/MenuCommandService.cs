using System.Collections;

namespace System.ComponentModel.Design
{
	public class MenuCommandService : IMenuCommandService, IDisposable
	{
		public virtual DesignerVerbCollection Verbs
		{
			get
			{
				throw null;
			}
		}

		public event MenuCommandsChangedEventHandler MenuCommandsChanged
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

		public MenuCommandService(IServiceProvider serviceProvider)
		{
			throw null;
		}

		public virtual void AddCommand(MenuCommand command)
		{
			throw null;
		}

		public virtual void AddVerb(DesignerVerb verb)
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

		protected void EnsureVerbs()
		{
			throw null;
		}

		public MenuCommand FindCommand(CommandID commandID)
		{
			throw null;
		}

		protected MenuCommand FindCommand(Guid guid, int id)
		{
			throw null;
		}

		protected ICollection GetCommandList(Guid guid)
		{
			throw null;
		}

		protected object GetService(Type serviceType)
		{
			throw null;
		}

		public virtual bool GlobalInvoke(CommandID commandID)
		{
			throw null;
		}

		public virtual bool GlobalInvoke(CommandID commandId, object arg)
		{
			throw null;
		}

		protected virtual void OnCommandsChanged(MenuCommandsChangedEventArgs e)
		{
			throw null;
		}

		public virtual void RemoveCommand(MenuCommand command)
		{
			throw null;
		}

		public virtual void RemoveVerb(DesignerVerb verb)
		{
			throw null;
		}

		public virtual void ShowContextMenu(CommandID menuID, int x, int y)
		{
			throw null;
		}
	}
}
