using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
	public class GridColumnStylesCollection : BaseCollection, IList, ICollection, IEnumerable
	{
		bool IList.IsFixedSize
		{
			get
			{
				throw null;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				throw null;
			}
		}

		object IList.this[int index]
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

		int ICollection.Count
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

		protected override ArrayList List
		{
			get
			{
				throw null;
			}
		}

		public DataGridColumnStyle this[int index]
		{
			get
			{
				throw null;
			}
		}

		public DataGridColumnStyle this[string columnName]
		{
			get
			{
				throw null;
			}
		}

		public DataGridColumnStyle this[PropertyDescriptor propertyDesciptor]
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

		int IList.Add(object value)
		{
			throw null;
		}

		void IList.Clear()
		{
			throw null;
		}

		bool IList.Contains(object value)
		{
			throw null;
		}

		int IList.IndexOf(object value)
		{
			throw null;
		}

		void IList.Insert(int index, object value)
		{
			throw null;
		}

		void IList.Remove(object value)
		{
			throw null;
		}

		void IList.RemoveAt(int index)
		{
			throw null;
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw null;
		}

		public virtual int Add(DataGridColumnStyle column)
		{
			throw null;
		}

		public void AddRange(DataGridColumnStyle[] columns)
		{
			throw null;
		}

		public void Clear()
		{
			throw null;
		}

		public bool Contains(PropertyDescriptor propertyDescriptor)
		{
			throw null;
		}

		public bool Contains(DataGridColumnStyle column)
		{
			throw null;
		}

		public bool Contains(string name)
		{
			throw null;
		}

		public int IndexOf(DataGridColumnStyle element)
		{
			throw null;
		}

		protected void OnCollectionChanged(CollectionChangeEventArgs e)
		{
			throw null;
		}

		public void Remove(DataGridColumnStyle column)
		{
			throw null;
		}

		public void RemoveAt(int index)
		{
			throw null;
		}

		public void ResetPropertyDescriptors()
		{
			throw null;
		}
	}
}
