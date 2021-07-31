using System.Collections;

namespace System.ComponentModel.Design.Serialization
{
	public abstract class BasicDesignerLoader : DesignerLoader, IDesignerLoaderService
	{
		protected enum ReloadOptions
		{
			Default = 0,
			ModifyOnError = 1,
			Force = 2,
			NoFlush = 4
		}

		protected virtual bool Modified
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

		protected IDesignerLoaderHost LoaderHost
		{
			get
			{
				throw null;
			}
		}

		public override bool Loading
		{
			get
			{
				throw null;
			}
		}

		protected object PropertyProvider
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

		protected bool ReloadPending
		{
			get
			{
				throw null;
			}
		}

		protected BasicDesignerLoader()
		{
			throw null;
		}

		public override void BeginLoad(IDesignerLoaderHost host)
		{
			throw null;
		}

		public override void Dispose()
		{
			throw null;
		}

		public override void Flush()
		{
			throw null;
		}

		protected object GetService(Type serviceType)
		{
			throw null;
		}

		protected virtual void Initialize()
		{
			throw null;
		}

		protected virtual bool IsReloadNeeded()
		{
			throw null;
		}

		protected virtual void OnBeginLoad()
		{
			throw null;
		}

		protected virtual bool EnableComponentNotification(bool enable)
		{
			throw null;
		}

		protected virtual void OnBeginUnload()
		{
			throw null;
		}

		protected virtual void OnEndLoad(bool successful, ICollection errors)
		{
			throw null;
		}

		protected virtual void OnModifying()
		{
			throw null;
		}

		protected abstract void PerformFlush(IDesignerSerializationManager serializationManager);

		protected abstract void PerformLoad(IDesignerSerializationManager serializationManager);

		protected void Reload(ReloadOptions flags)
		{
			throw null;
		}

		protected virtual void ReportFlushErrors(ICollection errors)
		{
			throw null;
		}

		protected void SetBaseComponentClassName(string name)
		{
			throw null;
		}

		void IDesignerLoaderService.AddLoadDependency()
		{
			throw null;
		}

		void IDesignerLoaderService.DependentLoadComplete(bool successful, ICollection errorCollection)
		{
			throw null;
		}

		bool IDesignerLoaderService.Reload()
		{
			throw null;
		}
	}
}
