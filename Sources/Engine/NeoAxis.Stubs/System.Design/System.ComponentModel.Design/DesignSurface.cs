using System.Collections;
using System.ComponentModel.Design.Serialization;

namespace System.ComponentModel.Design
{
	public class DesignSurface : IDisposable, IServiceProvider
	{
		public IContainer ComponentContainer
		{
			get
			{
				throw null;
			}
		}

		public bool IsLoaded
		{
			get
			{
				throw null;
			}
		}

		public ICollection LoadErrors
		{
			get
			{
				throw null;
			}
		}

		public bool DtelLoading
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

		protected ServiceContainer ServiceContainer
		{
			get
			{
				throw null;
			}
		}

		public object View
		{
			get
			{
				throw null;
			}
		}

		public event EventHandler Disposed
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

		public event EventHandler Flushed
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

		public event LoadedEventHandler Loaded
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

		public event EventHandler Loading
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

		public event EventHandler Unloaded
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

		public event EventHandler Unloading
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

		public event EventHandler ViewActivated
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

		public DesignSurface()
		{
			throw null;
		}

		public DesignSurface(IServiceProvider parentProvider)
		{
			throw null;
		}

		public DesignSurface(Type rootComponentType)
		{
			throw null;
		}

		public DesignSurface(IServiceProvider parentProvider, Type rootComponentType)
		{
			throw null;
		}

		public void BeginLoad(DesignerLoader loader)
		{
			throw null;
		}

		public void BeginLoad(Type rootComponentType)
		{
			throw null;
		}

		public INestedContainer CreateNestedContainer(IComponent owningComponent)
		{
			throw null;
		}

		public INestedContainer CreateNestedContainer(IComponent owningComponent, string containerName)
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

		public void Flush()
		{
			throw null;
		}

		public object GetService(Type serviceType)
		{
			throw null;
		}

		protected virtual void OnLoaded(LoadedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnLoading(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnUnloaded(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnUnloading(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnViewActivate(EventArgs e)
		{
			throw null;
		}
	}
}
