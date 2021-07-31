namespace System.ComponentModel.Design
{
	public abstract class UndoEngine : IDisposable
	{
		protected class UndoUnit
		{
			public string Name
			{
				get
				{
					throw null;
				}
			}

			public virtual bool IsEmpty
			{
				get
				{
					throw null;
				}
			}

			protected UndoEngine UndoEngine
			{
				get
				{
					throw null;
				}
			}

			public UndoUnit(UndoEngine engine, string name)
			{
				throw null;
			}

			public virtual void Close()
			{
				throw null;
			}

			public virtual void ComponentAdded(ComponentEventArgs e)
			{
				throw null;
			}

			public virtual void ComponentAdding(ComponentEventArgs e)
			{
				throw null;
			}

			public virtual void ComponentChanged(ComponentChangedEventArgs e)
			{
				throw null;
			}

			public virtual void ComponentChanging(ComponentChangingEventArgs e)
			{
				throw null;
			}

			public virtual void ComponentRemoved(ComponentEventArgs e)
			{
				throw null;
			}

			public virtual void ComponentRemoving(ComponentEventArgs e)
			{
				throw null;
			}

			public virtual void ComponentRename(ComponentRenameEventArgs e)
			{
				throw null;
			}

			protected object GetService(Type serviceType)
			{
				throw null;
			}

			public override string ToString()
			{
				throw null;
			}

			public void Undo()
			{
				throw null;
			}

			protected virtual void UndoCore()
			{
				throw null;
			}
		}

		public bool UndoInProgress
		{
			get
			{
				throw null;
			}
		}

		public bool Enabled
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

		public event EventHandler Undoing
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

		public event EventHandler Undone
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

		protected UndoEngine(IServiceProvider provider)
		{
			throw null;
		}

		protected abstract void AddUndoUnit(UndoUnit unit);

		protected virtual UndoUnit CreateUndoUnit(string name, bool primary)
		{
			throw null;
		}

		protected virtual void DiscardUndoUnit(UndoUnit unit)
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

		protected object GetRequiredService(Type serviceType)
		{
			throw null;
		}

		protected object GetService(Type serviceType)
		{
			throw null;
		}

		protected virtual void OnUndoing(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnUndone(EventArgs e)
		{
			throw null;
		}
	}
}
