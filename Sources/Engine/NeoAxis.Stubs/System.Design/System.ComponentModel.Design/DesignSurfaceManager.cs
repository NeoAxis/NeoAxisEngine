namespace System.ComponentModel.Design
{
	public class DesignSurfaceManager : IServiceProvider, IDisposable
	{
		public virtual DesignSurface ActiveDesignSurface
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

		public DesignSurfaceCollection DesignSurfaces
		{
			get
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

		public event ActiveDesignSurfaceChangedEventHandler ActiveDesignSurfaceChanged
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

		public event DesignSurfaceEventHandler DesignSurfaceCreated
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

		public event DesignSurfaceEventHandler DesignSurfaceDisposed
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

		public event EventHandler SelectionChanged
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

		public DesignSurfaceManager()
		{
			throw null;
		}

		public DesignSurfaceManager(IServiceProvider parentProvider)
		{
			throw null;
		}

		public DesignSurface CreateDesignSurface()
		{
			throw null;
		}

		public DesignSurface CreateDesignSurface(IServiceProvider parentProvider)
		{
			throw null;
		}

		protected virtual DesignSurface CreateDesignSurfaceCore(IServiceProvider parentProvider)
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

		public object GetService(Type serviceType)
		{
			throw null;
		}
	}
}
