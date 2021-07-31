using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
	public abstract class BindingManagerBase
	{
		public BindingsCollection Bindings
		{
			get
			{
				throw null;
			}
		}

		public abstract object Current
		{
			get;
		}

		public abstract int Position
		{
			get;
			set;
		}

		public bool IsBindingSuspended
		{
			get
			{
				throw null;
			}
		}

		public abstract int Count
		{
			get;
		}

		public event BindingCompleteEventHandler BindingComplete
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

		public event EventHandler CurrentChanged
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

		public event EventHandler CurrentItemChanged
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

		public event BindingManagerDataErrorEventHandler DataError
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

		public event EventHandler PositionChanged
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

		public BindingManagerBase()
		{
			throw null;
		}

		public virtual PropertyDescriptorCollection GetItemProperties()
		{
			throw null;
		}

		protected virtual PropertyDescriptorCollection GetItemProperties(Type listType, int offset, ArrayList dataSources, ArrayList listAccessors)
		{
			throw null;
		}

		public abstract void CancelCurrentEdit();

		public abstract void EndCurrentEdit();

		public abstract void AddNew();

		public abstract void RemoveAt(int index);

		protected abstract void UpdateIsBinding();

		public abstract void SuspendBinding();

		public abstract void ResumeBinding();

		protected void PullData()
		{
			throw null;
		}

		protected void PushData()
		{
			throw null;
		}
	}
}
