using System.Collections;

namespace System.ComponentModel.Design.Serialization
{
	public class DesignerSerializationManager : IDesignerSerializationManager, IServiceProvider
	{
		public IContainer Container
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

		public IList Errors
		{
			get
			{
				throw null;
			}
		}

		public bool PreserveNames
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

		public object PropertyProvider
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

		public bool RecycleInstances
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

		public bool ValidateRecycledTypes
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

		ContextStack Context
		{
			get
			{
				throw null;
			}
		}

		PropertyDescriptorCollection Properties
		{
			get
			{
				throw null;
			}
		}

		ContextStack IDesignerSerializationManager.Context
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		PropertyDescriptorCollection IDesignerSerializationManager.Properties
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public event EventHandler SessionCreated
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

		public event EventHandler SessionDisposed
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

		event ResolveNameEventHandler ResolveName
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

		event EventHandler SerializationComplete
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

		event ResolveNameEventHandler IDesignerSerializationManager.ResolveName
		{
			add
			{
				throw new NotImplementedException();
			}

			remove
			{
				throw new NotImplementedException();
			}
		}

		event EventHandler IDesignerSerializationManager.SerializationComplete
		{
			add
			{
				throw new NotImplementedException();
			}

			remove
			{
				throw new NotImplementedException();
			}
		}

		public DesignerSerializationManager()
		{
			throw null;
		}

		public DesignerSerializationManager(IServiceProvider provider)
		{
			throw null;
		}

		protected virtual object CreateInstance( Type type, ICollection arguments, string name, bool addToContainer)
		{
			throw null;
		}

		public IDisposable CreateSession()
		{
			throw null;
		}

		public object GetSerializer( Type objectType, Type serializerType )
		{
			throw null;
		}

		protected virtual object GetService( Type serviceType )
		{
			throw null;
		}

		protected virtual Type GetType(string typeName)
		{
			throw null;
		}

		public Type GetRuntimeType(string typeName)
		{
			throw null;
		}

		protected virtual void OnResolveName( ResolveNameEventArgs e )
		{
			throw null;
		}

		protected virtual void OnSessionCreated( EventArgs e )
		{
			throw null;
		}

		protected virtual void OnSessionDisposed( EventArgs e )
		{
			throw null;
		}

		void IDesignerSerializationManager.AddSerializationProvider( IDesignerSerializationProvider provider )
		{
			throw null;
		}

		object IDesignerSerializationManager.CreateInstance( Type type, ICollection arguments, string name, bool addToContainer)
		{
			throw null;
		}

		object IDesignerSerializationManager.GetInstance(string name)
		{
			throw null;
		}

		string IDesignerSerializationManager.GetName(object value)
		{
			throw null;
		}

		object IDesignerSerializationManager.GetSerializer( Type objectType, Type serializerType )
		{
			throw null;
		}

		Type IDesignerSerializationManager.GetType(string typeName)
		{
			throw null;
		}

		void IDesignerSerializationManager.RemoveSerializationProvider( IDesignerSerializationProvider provider )
		{
			throw null;
		}

		void IDesignerSerializationManager.ReportError(object errorInformation)
		{
			throw null;
		}

		void IDesignerSerializationManager.SetName(object instance, string name)
		{
			throw null;
		}

		object IServiceProvider.GetService( Type serviceType )
		{
			throw null;
		}
	}
}
