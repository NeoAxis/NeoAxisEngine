using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
	public class BindingContext : ICollection, IEnumerable
	{
		int ICollection.Count
		{
			get
			{
				throw null;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw null;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				throw null;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw null;
			}
		}

		public BindingManagerBase this[object dataSource]
		{
			get
			{
				throw null;
			}
		}

		public BindingManagerBase this[object dataSource, string dataMember]
		{
			get
			{
				throw null;
			}
		}

		public event CollectionChangeEventHandler CollectionChanged
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

		void ICollection.CopyTo(Array ar, int index)
		{
			throw null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw null;
		}

		public BindingContext()
		{
			throw null;
		}

		protected virtual void AddCore(object dataSource, BindingManagerBase listManager)
		{
			throw null;
		}

		protected virtual void ClearCore()
		{
			throw null;
		}

		public bool Contains(object dataSource)
		{
			throw null;
		}

		public bool Contains(object dataSource, string dataMember)
		{
			throw null;
		}

		protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent)
		{
			throw null;
		}

		protected virtual void RemoveCore(object dataSource)
		{
			throw null;
		}

		public static void UpdateBinding(BindingContext newBindingContext, Binding binding)
		{
			throw null;
		}
	}
}
