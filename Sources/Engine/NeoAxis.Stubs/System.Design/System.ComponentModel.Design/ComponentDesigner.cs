using System.Collections;

namespace System.ComponentModel.Design
{
	public class ComponentDesigner : ITreeDesigner, IDesigner, IDisposable, IDesignerFilter, IComponentInitializer
	{
		protected sealed class ShadowPropertyCollection
		{
			public object this[string propertyName]
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

			public bool Contains(string propertyName)
			{
				throw null;
			}
		}

		public virtual DesignerActionListCollection ActionLists
		{
			get
			{
				throw null;
			}
		}

		public virtual ICollection AssociatedComponents
		{
			get
			{
				throw null;
			}
		}

		public IComponent Component
		{
			get
			{
				throw null;
			}
		}

		protected bool Inherited
		{
			get
			{
				throw null;
			}
		}

		protected virtual IComponent ParentComponent
		{
			get
			{
				throw null;
			}
		}

		protected virtual InheritanceAttribute InheritanceAttribute
		{
			get
			{
				throw null;
			}
		}

		protected ShadowPropertyCollection ShadowProperties
		{
			get
			{
				throw null;
			}
		}

		public virtual DesignerVerbCollection Verbs
		{
			get
			{
				throw null;
			}
		}

		ICollection Children
		{
			get
			{
				throw null;
			}
		}

		IDesigner Parent
		{
			get
			{
				throw null;
			}
		}

		ICollection ITreeDesigner.Children
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		IDesigner ITreeDesigner.Parent
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected InheritanceAttribute InvokeGetInheritanceAttribute(ComponentDesigner toInvoke)
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		~ComponentDesigner()
		{
			throw null;
		}

		protected virtual void Dispose(bool disposing)
		{
			throw null;
		}

		public virtual void DoDefaultAction()
		{
			throw null;
		}

		public virtual void Initialize(IComponent component)
		{
			throw null;
		}

		public virtual void InitializeExistingComponent(IDictionary defaultValues)
		{
			throw null;
		}

		public virtual void InitializeNewComponent(IDictionary defaultValues)
		{
			throw null;
		}

		public virtual void InitializeNonDefault()
		{
			throw null;
		}

		protected virtual object GetService(Type serviceType)
		{
			throw null;
		}

		public virtual void OnSetComponentDefaults()
		{
			throw null;
		}

		protected virtual void PostFilterAttributes(IDictionary attributes)
		{
			throw null;
		}

		protected virtual void PostFilterEvents(IDictionary events)
		{
			throw null;
		}

		protected virtual void PostFilterProperties(IDictionary properties)
		{
			throw null;
		}

		protected virtual void PreFilterAttributes(IDictionary attributes)
		{
			throw null;
		}

		protected virtual void PreFilterEvents(IDictionary events)
		{
			throw null;
		}

		protected virtual void PreFilterProperties(IDictionary properties)
		{
			throw null;
		}

		protected void RaiseComponentChanged(MemberDescriptor member, object oldValue, object newValue)
		{
			throw null;
		}

		protected void RaiseComponentChanging(MemberDescriptor member)
		{
			throw null;
		}

		void IDesignerFilter.PostFilterAttributes(IDictionary attributes)
		{
			throw null;
		}

		void IDesignerFilter.PostFilterEvents(IDictionary events)
		{
			throw null;
		}

		void IDesignerFilter.PostFilterProperties(IDictionary properties)
		{
			throw null;
		}

		void IDesignerFilter.PreFilterAttributes(IDictionary attributes)
		{
			throw null;
		}

		void IDesignerFilter.PreFilterEvents(IDictionary events)
		{
			throw null;
		}

		void IDesignerFilter.PreFilterProperties(IDictionary properties)
		{
			throw null;
		}

		public ComponentDesigner()
		{
			throw null;
		}
	}
}
